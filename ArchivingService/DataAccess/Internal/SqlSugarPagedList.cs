
using System.Collections.Generic;

namespace SqlSugar;

/// <summary>
/// 分頁
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class SqlSugarPagedList<TEntity>
    where TEntity : new()
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總筆數
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// 當前頁資料
    /// </summary>
    public IEnumerable<TEntity> Items { get; set; }

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPrevPages { get; set; }

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPages { get; set; }
}

/// <summary>
/// pageed list
/// </summary>
public class PagedModel : SqlSugarPagedList<object>
{
}