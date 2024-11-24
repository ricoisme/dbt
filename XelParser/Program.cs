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

var xelParser = new XELFileHelper(inputFolder, outputFolder);
await xelParser.ProcessAsync();

Console.WriteLine("XelParser Done");
