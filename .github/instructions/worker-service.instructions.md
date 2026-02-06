---
applyTo: "**/Worker.cs,**/*Service/Program.cs"
description: ".NET Worker Service 背景服務指引"
---

# .NET Worker Service 背景服務指引

在本專案中開發 Worker Service 時，請遵循以下準則：

## Worker Service 基本架構

### Program.cs 設定
```csharp
var builder = Host.CreateApplicationBuilder(args);

// 註冊 Worker Service
builder.Services.AddHostedService<Worker>();

// 配置日誌（使用 Serilog）
builder.Services.AddSerilogLogging(builder.Configuration);

var host = builder.Build();
await host.RunAsync();
```

## Worker 類別實作

### 基本結構
```csharp
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    public Worker(
        ILogger<Worker> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker Service 開始執行於: {Time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DoWorkAsync(stoppingToken);
                
                // 設定執行間隔
                var delay = _configuration.GetValue<int>("WorkerDelaySeconds", 60);
                await Task.Delay(TimeSpan.FromSeconds(delay), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Worker Service 正在停止");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "執行工作時發生錯誤");
                // 發生錯誤後等待一段時間再重試
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("開始執行工作");
        
        // 實際工作邏輯
        
        _logger.LogInformation("工作執行完成");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker Service 正在優雅關閉");
        await base.StopAsync(cancellationToken);
    }
}
```

## 最佳實踐

### 1. 取消權杖處理
- **始終尊重 CancellationToken**：在所有非同步操作中傳遞並檢查取消權杖
- **優雅關閉**：確保在收到停止信號時能夠正確清理資源

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        // 檢查是否已要求取消
        if (stoppingToken.IsCancellationRequested)
            break;

        await ProcessItemAsync(stoppingToken);
    }
}
```

### 2. 錯誤處理
- **捕獲並記錄所有例外**：防止服務因未處理的例外而終止
- **實作重試機制**：對於暫時性錯誤，應該重試而不是立即失敗
- **區分 OperationCanceledException**：這是正常的關閉流程，不應記錄為錯誤

### 3. 日誌記錄
- **使用結構化日誌**：使用 Serilog 並包含相關的上下文資訊
- **記錄關鍵事件**：啟動、停止、錯誤和重要的業務事件
- **適當的日誌層級**：
  - `LogInformation`：正常操作
  - `LogWarning`：可恢復的錯誤或異常情況
  - `LogError`：需要關注的錯誤
  - `LogDebug`：詳細的除錯資訊

```csharp
_logger.LogInformation("處理項目 {ItemId} 於 {ProcessTime}", itemId, DateTime.UtcNow);
_logger.LogWarning("項目 {ItemId} 處理失敗，將重試", itemId);
_logger.LogError(ex, "處理項目 {ItemId} 時發生嚴重錯誤", itemId);
```

### 4. 設定管理
- **使用 appsettings.json**：將可配置的值放在設定檔中
- **支援環境特定設定**：使用 `appsettings.Development.json` 和 `appsettings.Production.json`
- **驗證設定**：在啟動時驗證必要的設定值

```json
{
  "WorkerDelaySeconds": 60,
  "BatchSize": 100,
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }
}
```

### 5. 相依性注入
- **註冊服務**：在 `Program.cs` 中註冊所有相依的服務
- **使用 Scoped 服務**：對於需要的服務，建立 Scope 來使用

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IMyService>();
            await service.DoWorkAsync(stoppingToken);
        }
        
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
    }
}
```

## 效能考量

### 1. 避免阻塞
- 使用非同步 API（`async`/`await`）
- 不要使用 `Thread.Sleep`，改用 `Task.Delay`

### 2. 記憶體管理
- 及時釋放資源，使用 `using` 語句
- 對於長時間執行的服務，注意記憶體洩漏

### 3. 批次處理
- 對於大量資料處理，使用批次處理來平衡效能和資源使用

## 監控與健康檢查

```csharp
// 在 Program.cs 中添加健康檢查
builder.Services.AddHealthChecks()
    .AddCheck<WorkerHealthCheck>("worker_health_check");
```

## 部署考量

### Windows Service
```powershell
# 發佈為自包含應用程式
dotnet publish -c Release -r win-x64 --self-contained

# 安裝為 Windows 服務
sc.exe create MyWorkerService binPath="C:\path\to\worker.exe"
```

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "WorkerService.dll"]
```

### Systemd (Linux)
```ini
[Unit]
Description=My Worker Service

[Service]
Type=notify
ExecStart=/path/to/worker
Restart=always

[Install]
WantedBy=multi-user.target
```
