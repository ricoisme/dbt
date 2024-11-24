

using Microsoft.SqlServer.XEvent.XELite;
using System.Collections.Concurrent;
using System.Text;

namespace XelParser;

internal sealed class XELFileHelper
{
    public string InputFolder { get; set; }
    public string OutputFolder { get; set; }

    public XELFileHelper(string inputFolder, string outputFolder)
    {
        InputFolder = inputFolder;
        OutputFolder = outputFolder;
    }

    public async Task ProcessAsync()
    {
        if (!Directory.Exists(InputFolder))
        {
            return;
        }

        if (!Directory.Exists(OutputFolder))
        {
            Directory.CreateDirectory(OutputFolder);
        }

        var errors = new ConcurrentBag<string>();
        var files = Directory.GetFiles(InputFolder);

        Parallel.ForEach(files, file =>
        {
            if (!File.Exists(file))
            {
                return;
            }

            var inputFileName = Path.GetFileName(file);
            var extension = Path.GetExtension(file);

            var xeStream = new XEFileEventStreamer(file);
            var sb = new StringBuilder();
            Console.WriteLine($"Parsing {inputFileName} started...");
            try
            {
                xeStream.ReadEventStream(xevent =>
                {
                    try
                    {
                        sb.AppendLine(xevent.ToString());
                    }
                    catch (EndOfStreamException eosEx)
                    {
                        errors.Add($"{inputFileName} End of stream reached unexpectedly: {eosEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"{inputFileName} Error processing event: {ex.Message}");
                    }
                    return Task.CompletedTask;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (EndOfStreamException eosEx)
            {
                errors.Add($"{inputFileName} End of stream reached unexpectedly: {eosEx.Message}");
            }
            catch (Exception ex)
            {
                errors.Add($"{inputFileName} Error processing XEL file: {ex.Message}");
            }
            Console.WriteLine("Parsing completed.");

            var outputFilePath = Path.Combine(OutputFolder, $"{inputFileName}.txt");
            File.WriteAllText(outputFilePath, sb.ToString(), Encoding.UTF8);
        });

        // 顯示所有錯誤訊息
        if (errors.Count > 0)
        {
            Console.WriteLine("Errors occurred during processing:");
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }
        else
        {
            Console.WriteLine("Processing completed with no errors.");
        }

        //foreach (var file in files)
        //{
        //    if (!File.Exists(file))
        //    {
        //        continue;
        //    }

        //    var inputFileName = Path.GetFileName(file);
        //    var extension = Path.GetExtension(file);

        //    var xeStream = new XEFileEventStreamer(file);
        //    var sb = new StringBuilder();
        //    Console.WriteLine($"Parsing {inputFileName} started...");
        //    try
        //    {
        //        await xeStream.ReadEventStream(xevent =>
        //        {
        //            try
        //            {
        //                //Console.WriteLine(xevent);
        //                sb.AppendLine(xevent.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Error processing event: {ex.Message}");
        //            }
        //            return Task.CompletedTask;
        //        }, CancellationToken.None);
        //    }
        //    catch (EndOfStreamException eosEx)
        //    {
        //        Console.WriteLine($"End of stream reached unexpectedly: {eosEx.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error processing XEL file: {ex.Message}");
        //    }
        //    Console.WriteLine("Parsing completed.");
        //    var outputFilePath = Path.Combine(OutputFolder, $"{inputFileName}.txt");
        //    await File.WriteAllTextAsync(outputFilePath, sb.ToString(), Encoding.UTF8);
        //}
    }
}
