using ArchivingService;

var builder = Host.CreateApplicationBuilder(args);
var srcConnectionString = builder.Configuration.GetConnectionString("sourceDB");
var srcConfig = new SqlSugar.ConnectionConfig
{
    DbType = SqlSugar.DbType.SqlServer,
    ConnectionString = srcConnectionString,
    IsAutoCloseConnection = true,
    ConfigId = "sourceDB"
};
var tarConnectionString = builder.Configuration.GetConnectionString("targetDB");
var tarConfig = new SqlSugar.ConnectionConfig
{
    DbType = SqlSugar.DbType.SqlServer,
    ConnectionString = tarConnectionString,
    IsAutoCloseConnection = true,
    ConfigId = "targetDB"
};
builder.Services.AddSqlSugar(new[] { srcConfig, tarConfig });

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
