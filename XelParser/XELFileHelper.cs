

using Microsoft.SqlServer.XEvent.XELite;
using System.Collections.Concurrent;
using System.Globalization;
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
                catch (Exception ex) when (ex is not OperationCanceledException)
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
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            errors.Add($"{inputFileName} Error processing XEL file: {ex.Message}");
        }
        
        Console.WriteLine($"Parsing {inputFileName} completed.");

        var outputFilePath = Path.Combine(OutputFolder, $"{inputFileName}.txt");
        var sanitizedContent = SanitizeSpecialCharacters(sb.ToString());
        await File.WriteAllTextAsync(outputFilePath, sanitizedContent, Encoding.UTF8);
    }

    /// <summary>
    /// 高複雜度特殊字元處理：標準化 Unicode、消除零寬字元、壓縮空白、
    /// 並將不可見控制字元轉換為可追蹤的跳脫序列。
    /// </summary>
    /// <param name="input">原始文字內容。</param>
    /// <returns>完成特殊字元清理後的內容。</returns>
    private static string SanitizeSpecialCharacters(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        // 將內容先做 Compatibility Decomposition + Composition，統一全形/相容字型差異
        var normalized = input.Normalize(NormalizationForm.FormKC);
        var sb = new StringBuilder(normalized.Length + 64);

        var removedZeroWidthCount = 0;
        var escapedControlCount = 0;
        var collapsedWhitespaceCount = 0;

        var previousWasWhitespace = false;
        var previousWasLineBreak = false;

        foreach (var rune in normalized.EnumerateRunes())
        {
            var codePoint = rune.Value;

            // 移除常見零寬與排版控制字元
            if (codePoint is 0x200B or 0x200C or 0x200D or 0x2060 or 0xFEFF)
            {
                removedZeroWidthCount++;
                continue;
            }

            if (codePoint == '\r' || codePoint == '\n')
            {
                if (!previousWasLineBreak)
                {
                    sb.Append('\n');
                }

                previousWasLineBreak = true;
                previousWasWhitespace = false;
                continue;
            }

            previousWasLineBreak = false;

            // 統一 tab 與各類 Unicode 空白為一般空白，並壓縮連續空白
            if (Rune.IsWhiteSpace(rune) || codePoint == '\t')
            {
                if (!previousWasWhitespace)
                {
                    sb.Append(' ');
                }
                else
                {
                    collapsedWhitespaceCount++;
                }

                previousWasWhitespace = true;
                continue;
            }

            previousWasWhitespace = false;

            var category = Rune.GetUnicodeCategory(rune);

            // 將不可見控制字元或格式控制字元轉為 \u{XXXX} 形式，避免資料遺失又方便追蹤
            if (category is UnicodeCategory.Control or UnicodeCategory.Format or UnicodeCategory.OtherNotAssigned)
            {
                sb.Append("\\u{");
                sb.Append(codePoint.ToString("X"));
                sb.Append('}');
                escapedControlCount++;
                continue;
            }

            // 常見「智慧標點」映射為 ASCII，減少後續系統轉碼問題
            switch (codePoint)
            {
                case 0x2018:
                case 0x2019:
                case 0x2032:
                    sb.Append('\'');
                    continue;
                case 0x201C:
                case 0x201D:
                case 0x2033:
                    sb.Append('"');
                    continue;
                case 0x2013:
                case 0x2014:
                case 0x2212:
                    sb.Append('-');
                    continue;
                case 0x2026:
                    sb.Append("...");
                    continue;
            }

            sb.Append(rune);
        }

        var processed = sb.ToString().Trim();

        // 將 3 行以上空行收斂為最多 2 行，避免輸出檔案被稀釋
        processed = CollapseExcessiveBlankLines(processed, maxConsecutiveBlankLines: 2);

        // 附加處理摘要，方便追查來源資料品質
        if (removedZeroWidthCount > 0 || escapedControlCount > 0 || collapsedWhitespaceCount > 0)
        {
            processed += Environment.NewLine;
            processed += Environment.NewLine;
            processed += $"[SanitizeSummary] RemovedZeroWidth={removedZeroWidthCount}; EscapedControl={escapedControlCount}; CollapsedWhitespace={collapsedWhitespaceCount}";
        }

        return processed;
    }

    /// <summary>
    /// 收斂過多連續空白行。
    /// </summary>
    /// <param name="input">輸入內容。</param>
    /// <param name="maxConsecutiveBlankLines">允許的最大連續空白行數。</param>
    /// <returns>收斂後的文字內容。</returns>
    private static string CollapseExcessiveBlankLines(string input, int maxConsecutiveBlankLines)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var lines = input.Split('\n');
        var sb = new StringBuilder(input.Length);
        var blankCount = 0;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].TrimEnd('\r');
            var isBlank = string.IsNullOrWhiteSpace(line);

            if (isBlank)
            {
                blankCount++;
                if (blankCount > maxConsecutiveBlankLines)
                {
                    continue;
                }
            }
            else
            {
                blankCount = 0;
            }

            sb.Append(line);
            if (i < lines.Length - 1)
            {
                sb.Append('\n');
            }
        }

        return sb.ToString();
    }
}
