using ArchivingService.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using static System.Formats.Asn1.AsnWriter;

namespace ArchivingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private ISqlSugarClient _srcsqlSugarClient;
        private ISqlSugarClient _tarsqlSugarClient;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory
            , IConfiguration configuration)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _srcsqlSugarClient = CreateSqlClient("sourceDB");
            _tarsqlSugarClient = CreateSqlClient("targetDB");
            var retentionMonth = _configuration.GetValue<int>("RetentionMonth");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                var sourceTables =
                 await _srcsqlSugarClient.Queryable<SourceTable>()
                  .With(SqlWith.NoLock)
                  .Where(c => c.Active)
                  .ToListAsync();

                foreach (var sourceTable in sourceTables)
                {
                    if (!TableExistsInBothClients(sourceTable.TableName))
                    {
                        continue;
                    }

                    var processStat = await _srcsqlSugarClient
                        .Queryable<ProcessState>()
                        .With(SqlWith.NoLock)
                        .Where(c => c.SourceTableId == sourceTable.Id).FirstAsync();

                    var (startDate, endDate) =
                       GetDateRange(sourceTable, retentionMonth, processStat);
                    Console.WriteLine($"StartDate:{startDate}, EndDate:{endDate}");

                    var query = sourceTable.KeyQuery
                        .Replace("$startDate$", startDate)
                        .Replace("$endDate$", endDate);
                    var fullTableName = $"{sourceTable.SchemaName}.{sourceTable.TableName}";

                    try
                    {
                        var dt = await _srcsqlSugarClient.Ado
                        .GetDataTableAsync(query);

                        if (dt == null || dt?.Rows?.Count == 0)
                        {
                            processStat.CompleteDate = DateTime.Now;
                            await UpsertProcessStatAsync(processStat);
                            Console.WriteLine($"no data in {fullTableName}");
                            continue;
                        }

                        var dcs = _srcsqlSugarClient.Utilities.DataTableToDictionaryList(dt);

                        if (sourceTable.Archive)
                        {
                            var rowCopied = await ArchiveDataAsync(dcs, fullTableName, sourceTable.DataCopyBatchSize, query);
                            processStat.RowsCopied = rowCopied;
                        }

                        if (sourceTable.Purge)
                        {
                            var rowPks = dt.AsEnumerable()
                            .Select(s => new Dictionary<string, object>
                            {
                        { "OrderLineID", s.Field<int>("OrderLineID") }
                            }).ToList();

                            var rowPurged = await PurgeDataAsync(rowPks, fullTableName, sourceTable.DataCopyBatchSize);
                            processStat.RowsPurged = rowPurged;
                        }
                        processStat.CompleteDate = DateTime.Now;
                        await UpsertProcessStatAsync(processStat);
                    }
                    catch (SqlException ex)
                    {
                        await HandleErrorAsync(ex, processStat);
                    }

                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private ISqlSugarClient CreateSqlClient(string configId)
        {
            var srcScope = _serviceScopeFactory.CreateScope();
            var client = srcScope.ServiceProvider.GetService<ISqlSugarClient>();
            (client as SqlSugarClient).ChangeDatabase(configId);
            return client;
        }

        private bool TableExistsInBothClients(string tableName)
        {
            var isExistsInSource = CheckTableExists(tableName, _srcsqlSugarClient);
            var isExistsInTarget = CheckTableExists(tableName, _tarsqlSugarClient);
            return isExistsInSource && isExistsInTarget;
        }

        private bool CheckTableExists(string tableName, ISqlSugarClient sqlSugarClient)
        {
            var result = sqlSugarClient.DbMaintenance.IsAnyTable(tableName, false);
            if (!result)
            {
                Console.WriteLine($"{tableName} is not exists in {sqlSugarClient.CurrentConnectionConfig.ConfigId}");
            }
            return result;
        }

        private (string startDate, string endDate) GetDateRange(SourceTable sourceTable
            , int retentionMonth, ProcessState? processStat)
        {
            var endDate = DateTime.Now.AddMonths(-retentionMonth).ToString("yyyy-MM-dd");
            var startDate = "2013-01-01"; // Default start date           

            if (processStat == null)
            {
                processStat = new ProcessState
                {
                    SourceTableId = sourceTable.Id,
                    CreateDate = DateTime.Now
                };
            }
            else
            {
                startDate = GetStartDate(processStat);
                endDate = GetEndDate(processStat, retentionMonth);
            }

            processStat.LastArchivedDate = DateTime.Parse(startDate);
            processStat.LastPurgedDate = DateTime.Parse(startDate);

            return (startDate, endDate);
        }

        private string GetStartDate(ProcessState processStat)
        {
            var possibleStartDate = processStat.LastArchivedDate.AddDays(1).ToString("yyyy-MM-dd");
            if (!processStat.CompleteDate.HasValue)
                possibleStartDate = processStat.LastArchivedDate.ToString("yyyy-MM-dd");
            return possibleStartDate;
        }

        private string GetEndDate(ProcessState processStat, int retentionMonth)
        {
            var possibleEndDate = processStat.LastArchivedDate.AddDays(2);
            if (!processStat.CompleteDate.HasValue)
            {
                possibleEndDate = processStat.LastArchivedDate.AddDays(1);
            }
            var cutoffDate = DateTime.Now.AddMonths(-retentionMonth);

            return possibleEndDate > cutoffDate
                ? cutoffDate.ToString("yyyy-MM-dd")
                : possibleEndDate.ToString("yyyy-MM-dd");
        }

        private async Task<int> ArchiveDataAsync(List<Dictionary<string, object>> data, string fullTableName, int batchSize, string query)
        {
            var rowCopied = 0;
            var dt = await _tarsqlSugarClient.Ado
                       .GetDataTableAsync(query);
            var targetData = _tarsqlSugarClient.Utilities.DataTableToDictionaryList(dt);

            var missingItems =
                FindMissingItems(data, targetData, "OrderLineID");
            if (missingItems == null || missingItems?.Any() == false)
            {
                return rowCopied;
            }

            await _tarsqlSugarClient.Utilities.PageEachAsync(
                missingItems, batchSize,
                  async pageList =>
                  {
                      rowCopied += await _tarsqlSugarClient
                      .Insertable(pageList).AS(fullTableName)
                      .ExecuteCommandAsync();
                  });
            return rowCopied;
        }

        private List<Dictionary<string, object>> FindMissingItems(
        List<Dictionary<string, object>> sourceList,
        List<Dictionary<string, object>> targetList,
        string key)
        {
            var targetKeys = targetList.Select(d => d[key]).ToHashSet();

            return sourceList.Where(d => !targetKeys.Contains(d[key])).ToList();
        }

        private async Task<int> PurgeDataAsync(List<Dictionary<string, object>> rowPks, string fullTableName, int batchSize)
        {
            var rowPurged = 0;
            await _srcsqlSugarClient.Utilities.PageEachAsync(
               rowPks, batchSize, async pageList =>
               {
                   rowPurged += await _srcsqlSugarClient
                               .Deleteable<object>().AS(fullTableName)
                               .WhereColumns(pageList)
                               .ExecuteCommandAsync();
               });
            return rowPurged;
        }

        async Task UpsertProcessStatAsync(ProcessState entity)
        {
            if (entity.Id == 0)
            {
                await _srcsqlSugarClient.Insertable(entity)
                 .IgnoreColumns("Id")
                 .ExecuteCommandAsync();
            }
            else
            {
                await _srcsqlSugarClient.Updateable(entity)
                    .Where(c => c.Id == entity.Id)
                    .IgnoreColumns("Id")
                    .ExecuteCommandAsync();
            }
        }

        private async Task HandleErrorAsync(SqlException ex, ProcessState processStat)
        {
            processStat.CompleteDate = null;
            await UpsertProcessStatAsync(processStat);
            Console.WriteLine(ex.Message);
        }
    }
}
