
using System;
using System.Linq;
using SqlSugar;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// SqlSugar extension
/// </summary>
public static class SqlSugarServiceCollectionExtensions
{
    /// <summary>
    /// 使用 SqlSugar extension
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <param name="buildAction"></param>
    /// <returns></returns>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, ConnectionConfig config, Action<ISqlSugarClient> buildAction = default)
    {
        return services.AddSqlSugar(new ConnectionConfig[] { config }, buildAction);
    }

    /// <summary>
    /// 使用 SqlSugar extension
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configs"></param>
    /// <param name="buildAction"></param>
    /// <returns></returns>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, ConnectionConfig[] configs, Action<ISqlSugarClient> buildAction = default)
    {
        Action<ISqlSugarClient> defaultClient = (client) =>
        {
            client.Ado.CommandTimeOut = 30;
            client.Aop.OnError = (exp) =>
            {
                var sql = UtilMethods.GetNativeSql(exp.Sql, exp.Parametres as SugarParameter[]);
                Console.WriteLine(sql);
                Console.WriteLine($"{exp.GetBaseException()}");
            };

#if !RELEASE
            client.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql);
                if (pars?.Any() == true)
                {
                    Console.WriteLine(SqlProfiler.ParameterFormat(sql, pars));
                    //Console.WriteLine(buildAction.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                }
            };
#endif
        };

        buildAction = buildAction ?? defaultClient;
        // 註冊 SqlSugar client
        services.AddScoped<ISqlSugarClient>(u =>
        {
            var sqlSugarClient = new SqlSugarClient(configs.ToList());
            buildAction?.Invoke(sqlSugarClient);

            return sqlSugarClient;
        });

        // 註冊非泛型 repository
        services.AddScoped<ISqlSugarRepository, SqlSugarRepository>();

        // 註冊 SqlSugar repository
        services.AddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));

        return services;
    }
}