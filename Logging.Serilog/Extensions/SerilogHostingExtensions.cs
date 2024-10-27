#if !NET5_0
using Microsoft.AspNetCore.Builder;
#endif

using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;


/// <summary>
/// Serilog 擴充
/// </summary>
[ExcludeFromCodeCoverage]
public static class SerilogHostingExtensions
{

    /// <summary>
    /// 使用預設擴充
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configAction"></param>
    /// <returns></returns>
    public static IHostBuilder UseSerilogDefault(this IHostBuilder builder, Action<LoggerConfiguration> configAction = default)
    {
        // 是否單文件環境
        var isSingleFileEnvironment = string.IsNullOrWhiteSpace(Assembly.GetEntryAssembly().Location);

        builder.UseSerilog((context, configuration) =>
        {
            // 讀取設定檔
            var config = configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext();

            if (configAction != null) configAction.Invoke(config);
            else
            {
                // 判斷是否有設定輸出
                var hasWriteTo = context.Configuration["Serilog:WriteTo:0:Name"];
                if (hasWriteTo == null)
                {
                    config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                      .WriteTo.File(Path.Combine(!isSingleFileEnvironment ? AppDomain.CurrentDomain.BaseDirectory : AppContext.BaseDirectory, "logs", "application.log"), LogEventLevel.Information, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null, encoding: Encoding.UTF8);
                }
            }
        });

        return builder;
    }


    /// <summary>
    /// 使用預設擴充
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configAction"></param>
    /// <returns></returns>
    public static WebApplicationBuilder UseSerilogDefault(this WebApplicationBuilder builder, Action<LoggerConfiguration> configAction = default)
    {
        builder.Host.UseSerilogDefault(configAction);

        return builder;
    }

    public static IServiceCollection AddSerilog(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.AddSerilog(config =>
        {
            config.ReadFrom.Configuration(configurationManager);
        });
        return services;
    }
}

