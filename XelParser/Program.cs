using Microsoft.Extensions.Configuration;
using System.Text;
using XelParser;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = Encoding.UTF8;

var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
var inputFolder = configuration.GetSection("InputFolder").Value;
var outputFolder = configuration.GetSection("OutputFolder").Value;

if (string.IsNullOrWhiteSpace(inputFolder))
{
    throw new InvalidOperationException("InputFolder configuration is missing or empty in appsettings.json.");
}

if (string.IsNullOrWhiteSpace(outputFolder))
{
    throw new InvalidOperationException("OutputFolder configuration is missing or empty in appsettings.json.");
}

var xelParser = new XELFileHelper(inputFolder, outputFolder);

// 支援 Ctrl+C 取消操作
using var cts = new CancellationTokenSource();
ConsoleCancelEventHandler cancelHandler = (sender, e) =>
{
    e.Cancel = true;
    try
    {
        cts.Cancel();
        Console.WriteLine("\nCancellation requested...");
    }
    catch (ObjectDisposedException)
    {
        // CancellationTokenSource 已被釋放，忽略
    }
};

Console.CancelKeyPress += cancelHandler;
try
{
    await xelParser.ProcessAsync(cts.Token);
}
finally
{
    Console.CancelKeyPress -= cancelHandler;
}

Console.WriteLine("XelParser Done");
