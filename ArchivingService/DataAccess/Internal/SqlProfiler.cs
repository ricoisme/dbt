namespace SqlSugar;

/// <summary>
/// SqlSugar SQL Statement helper
/// desc：在需要顯示SQL Statement 的，如 Startup，将
/// App.PrintToMiniProfiler("SqlSugar1", "Info", sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
/// 替換
/// App.PrintToMiniProfiler("SqlSugar", "Info", SqlProfiler.ParameterFormat(sql, pars));
/// </summary>
public class SqlProfiler
{
    /// <summary>
    /// 參數格式化並顯示完整SQL Statement
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="pars"></param>
    /// <returns></returns>
    public static string ParameterFormat(string sql, SugarParameter[] pars)
    {

        //反向替换，因 SqlSugar 多表的filter導致替換不完整  如 @TenantId1  @TenantId10
        for (var i = pars.Length - 1; i >= 0; i--)
        {
            if (pars[i].DbType == System.Data.DbType.String
                || pars[i].DbType == System.Data.DbType.DateTime
                || pars[i].DbType == System.Data.DbType.Date
                || pars[i].DbType == System.Data.DbType.Time
                || pars[i].DbType == System.Data.DbType.DateTime2
                || pars[i].DbType == System.Data.DbType.DateTimeOffset
                || pars[i].DbType == System.Data.DbType.Guid
                || pars[i].DbType == System.Data.DbType.VarNumeric
                || pars[i].DbType == System.Data.DbType.AnsiStringFixedLength
                || pars[i].DbType == System.Data.DbType.AnsiString
                || pars[i].DbType == System.Data.DbType.StringFixedLength)
            {
                sql = sql.Replace(pars[i].ParameterName, "'" + pars[i].Value?.ToString() + "'");
            }
            else if (pars[i].DbType == System.Data.DbType.Boolean)
            {
                sql = sql.Replace(pars[i].ParameterName, Convert.ToBoolean(pars[i].Value) ? "1" : "0");
            }
            else
            {
                sql = sql.Replace(pars[i].ParameterName, pars[i].Value?.ToString());
            }
        }

        return sql;
    }

    /// <summary>
    /// 參數格式化並顯示完整SQL Statement
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="pars"></param>
    /// <returns></returns>
    public static string ParameterFormat(string sql, object pars)
    {
        var param = (SugarParameter[])pars;
        return ParameterFormat(sql, param);
    }
}