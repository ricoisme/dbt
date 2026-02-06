

using Microsoft.SqlServer.XEvent.XELite;
using System.Collections.Concurrent;
using System.Text;

namespace XelParser;

/// <summary>
/// 提供 XEL (Extended Events Log) 檔案的處理功能，將 XEL 檔案轉換為文字格式。
/// </summary>
internal sealed class XELFileHelper
{
    /// <summary>
    /// 取得或設定輸入資料夾的路徑，用於讀取 XEL 檔案。
    /// </summary>
    public string InputFolder { get; set; }
    
    /// <summary>
    /// 取得或設定輸出資料夾的路徑，用於儲存轉換後的文字檔案。
    /// </summary>
    public string OutputFolder { get; set; }

    /// <summary>
    /// 初始化 <see cref="XELFileHelper"/> 類別的新執行個體。
    /// </summary>
    /// <param name="inputFolder">包含 XEL 檔案的輸入資料夾路徑。</param>
    /// <param name="outputFolder">用於儲存轉換後文字檔案的輸出資料夾路徑。</param>
    public XELFileHelper(string inputFolder, string outputFolder)
    {
        InputFolder = inputFolder;
        OutputFolder = outputFolder;
    }

    /// <summary>
    /// 非同步處理輸入資料夾中的所有 XEL 檔案，並將其內容轉換為文字格式儲存至輸出資料夾。
    /// </summary>
    /// <remarks>
    /// 此方法會使用 <see cref="Task.WhenAll(System.Collections.Generic.IEnumerable{Task})"/> 平行處理多個檔案以提升效能。
    /// 如果輸入資料夾不存在，方法會直接返回。如果輸出資料夾不存在，將會自動建立。
    /// 處理過程中發生的錯誤會被收集並在處理完成後顯示。
    /// </remarks>
    /// <returns>代表非同步作業的工作。</returns>
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

        var tasks = files.Select(file => ProcessFileAsync(file, errors));
        await Task.WhenAll(tasks);

        // 顯示所有錯誤訊息（所有工作完成後再取快照）
        var errorSnapshot = errors.ToArray();
        if (errorSnapshot.Length > 0)
        {
            Console.WriteLine("Errors occurred during processing:");
            foreach (var error in errorSnapshot)
            {
                Console.WriteLine(error);
            }
        }
        else
        {
            Console.WriteLine("Processing completed with no errors.");
        }
    }

    /// <summary>
    /// 非同步處理單一 XEL 檔案。
    /// </summary>
    /// <param name="file">要處理的檔案路徑。</param>
    /// <param name="errors">用於收集錯誤訊息的並行集合。</param>
    /// <returns>代表非同步作業的工作。</returns>
    private async Task ProcessFileAsync(string file, ConcurrentBag<string> errors)
    {
        if (!File.Exists(file))
        {
            return;
        }

        var inputFileName = Path.GetFileName(file);
        var xeStream = new XEFileEventStreamer(file);
        var sb = new StringBuilder();
        Console.WriteLine($"Parsing {inputFileName} started...");
        
        try
        {
            await xeStream.ReadEventStream(xevent =>
            {
                try
                {
                    sb.AppendLine(xevent.ToString());
                }
                catch (EndOfStreamException eosEx)
                {
                    errors.Add($"{inputFileName} End of stream reached unexpectedly: {eosEx.Message}");
                }
                catch (Exception ex) when (ex is not OperationCanceledException and not TaskCanceledException)
                {
                    errors.Add($"{inputFileName} Error processing event: {ex.Message}");
                }
                return Task.CompletedTask;
            }, CancellationToken.None);
        }
        catch (EndOfStreamException eosEx)
        {
            errors.Add($"{inputFileName} End of stream reached unexpectedly: {eosEx.Message}");
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not TaskCanceledException)
        {
            errors.Add($"{inputFileName} Error processing XEL file: {ex.Message}");
        }
        
        Console.WriteLine($"Parsing {inputFileName} completed.");

        var outputFilePath = Path.Combine(OutputFolder, $"{inputFileName}.txt");
        await File.WriteAllTextAsync(outputFilePath, sb.ToString(), Encoding.UTF8);
    }
}
