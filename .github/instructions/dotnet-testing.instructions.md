---
applyTo: "**/*.Tests.csproj,**/Tests/**/*.cs,**/*Tests.cs"
description: "MSTest 測試專案指引"
---

# .NET 測試專案指引

在本專案中撰寫測試時，請遵循以下準則：

## 測試框架
- 使用 **MSTest** 作為測試框架
- 測試專案應參照 `Microsoft.NET.Test.Sdk`、`MSTest.TestAdapter` 和 `MSTest.TestFramework`

## 測試結構與命名
1. **測試類別命名**：使用 `[TestClass]` 屬性標記，類別名稱應為 `{ClassName}Tests` 格式
2. **測試方法命名**：使用 `[TestMethod]` 屬性標記，方法名稱應清楚描述測試意圖
   - 遵循模式：`MethodName_Scenario_ExpectedBehavior`
   - 範例：`CalculateDiscount_ValidOrder_ReturnsCorrectAmount`

## AAA 模式
所有測試應遵循 Arrange-Act-Assert (AAA) 模式：
```csharp
[TestMethod]
public void GetUser_ValidId_ReturnsUser()
{
    // Arrange（準備）
    var userId = 1;
    var expectedName = "Test User";

    // Act（執行）
    var result = _service.GetUser(userId);

    // Assert（驗證）
    Assert.IsNotNull(result);
    Assert.AreEqual(expectedName, result.Name);
}
```

## 測試覆蓋率目標
- 新功能的單元測試覆蓋率應 ≥ 90%
- 關鍵業務邏輯必須達到 100% 覆蓋率
- 使用 `dotnet test --collect:"XPlat Code Coverage"` 來收集覆蓋率報告

## 斷言最佳實踐
- 優先使用具體的斷言方法：`Assert.AreEqual`, `Assert.IsTrue`, `Assert.ThrowsException`
- 為每個斷言提供有意義的錯誤訊息
- 每個測試應專注於一個核心行為

## 測試資料管理
- 使用 `[TestInitialize]` 進行測試前的設定
- 使用 `[TestCleanup]` 進行測試後的清理
- 避免測試之間共享狀態

## 執行測試
```powershell
# 執行所有測試
dotnet test

# 執行特定測試專案
dotnet test DBT.Tests/DBT.Tests.csproj

# 產生測試報告
dotnet test --logger "trx;LogFileName=test_results.trx"
```
