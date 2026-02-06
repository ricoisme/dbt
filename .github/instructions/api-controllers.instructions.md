---
applyTo: "**/Controllers/**/*.cs"
description: "ASP.NET Core Web API Controller 指引"
---

# ASP.NET Core Web API Controller 指引

在本專案中開發 API Controller 時，請遵循以下準則：

## Controller 基本規範

### 命名與繼承
- Controller 類別名稱：使用 `{EntityName}Controller` 格式（例如：`DiscountController`）
- 繼承自 `ControllerBase`（不是 `Controller`，因為這是 API 專案）
- 使用 `[ApiController]` 屬性標記

### 路由屬性
```csharp
[ApiController]
[Route("api/[controller]")]
public class DiscountController : ControllerBase
{
    // Controller 實作
}
```

## Action 方法規範

### 命名慣例
- 使用動詞或動詞片語：`GetUser`, `CreateOrder`, `UpdateProduct`, `DeleteItem`
- 表達清楚的業務意圖

### 非同步方法
- **所有 Action 方法都必須是非同步的**
- 方法名稱使用 `Async` 後綴（選擇性，但建議用於明確性）
- 回傳 `Task<IActionResult>` 或 `Task<ActionResult<T>>`

### HTTP 動詞屬性
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<UserDto>> GetUserAsync(int id)
{
    // 實作
}

[HttpPost]
public async Task<ActionResult<UserDto>> CreateUserAsync([FromBody] CreateUserRequest request)
{
    // 實作
}

[HttpPut("{id}")]
public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserRequest request)
{
    // 實作
}

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUserAsync(int id)
{
    // 實作
}
```

## 回傳類型與狀態碼

### 使用 ActionResult<T>
```csharp
[HttpGet("{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<UserDto>> GetUserAsync(int id)
{
    var user = await _userService.GetUserByIdAsync(id);
    
    if (user == null)
        return NotFound();
    
    return Ok(user);
}
```

### ProducesResponseType
- 為每個可能的 HTTP 狀態碼添加 `[ProducesResponseType]` 屬性
- 幫助 Swagger/OpenAPI 產生正確的文件

## 模型驗證

### 自動驗證
```csharp
[HttpPost]
public async Task<ActionResult<UserDto>> CreateUserAsync([FromBody] CreateUserRequest request)
{
    // [ApiController] 會自動驗證模型
    // 如果驗證失敗，會自動回傳 400 BadRequest
    
    var user = await _userService.CreateUserAsync(request);
    return CreatedAtAction(nameof(GetUserAsync), new { id = user.Id }, user);
}
```

## 相依性注入
- 使用建構函式注入
- 將服務標記為 `readonly`

```csharp
public class DiscountController : ControllerBase
{
    private readonly IMarketingDiscountService _discountService;
    private readonly ILogger<DiscountController> _logger;

    public DiscountController(
        IMarketingDiscountService discountService,
        ILogger<DiscountController> logger)
    {
        _discountService = discountService;
        _logger = logger;
    }
}
```

## 錯誤處理
- 使用標準的 HTTP 狀態碼
- 提供有意義的錯誤訊息
- 記錄錯誤資訊

```csharp
try
{
    var result = await _service.ProcessAsync(request);
    return Ok(result);
}
catch (NotFoundException ex)
{
    _logger.LogWarning(ex, "資源未找到");
    return NotFound(ex.Message);
}
catch (ValidationException ex)
{
    _logger.LogWarning(ex, "驗證失敗");
    return BadRequest(ex.Message);
}
catch (Exception ex)
{
    _logger.LogError(ex, "處理請求時發生未預期的錯誤");
    return StatusCode(500, "伺服器內部錯誤");
}
```

## XML 文件註解
```csharp
/// <summary>
/// 取得指定使用者的詳細資訊
/// </summary>
/// <param name="id">使用者 ID</param>
/// <returns>使用者詳細資訊</returns>
/// <response code="200">成功回傳使用者資訊</response>
/// <response code="404">找不到指定的使用者</response>
[HttpGet("{id}")]
[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<UserDto>> GetUserAsync(int id)
{
    // 實作
}
```
