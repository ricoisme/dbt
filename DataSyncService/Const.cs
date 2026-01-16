using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncService
{
    /// <summary>
    /// 定義資料同步服務使用的常數
    /// </summary>
    internal static class Const
    {
        /// <summary>
        /// 用於查詢所有舊版本預存程序的 SQL 陳述式
        /// </summary>
        /// <remarks>
        /// 此 SQL 會查詢 INFORMATION_SCHEMA.ROUTINES 並產生刪除符合版本命名模式的預存程序之陳述式
        /// </remarks>
        internal const string GetDropAllOldSps = @"SELECT CONCAT( 'drop proc ',SPECIFIC_SCHEMA,'.',SPECIFIC_NAME,' ;') as [statement]
  FROM INFORMATION_SCHEMA.ROUTINES
 WHERE ROUTINE_TYPE = 'PROCEDURE'
 and (SPECIFIC_NAME like '%_$version$%')";

        /// <summary>
        /// 用於查詢所有使用者定義的資料表類型的 SQL 陳述式
        /// </summary>
        /// <remarks>
        /// 此 SQL 會查詢 sys.types 並產生刪除符合版本命名模式的使用者定義類型之陳述式
        /// </remarks>
        internal const string GetDropAllUserTableTypes = @"select CONCAT('drop type ', name,' ;') from sys.types
where is_user_defined = 1
and name like '%_$version$%'";
    }
}
