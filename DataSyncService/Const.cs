using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncService
{
    internal static class Const
    {
        internal const string GetDropAllOldSps = @"SELECT CONCAT( 'drop proc ',SPECIFIC_SCHEMA,'.',SPECIFIC_NAME,' ;') as [statement]
  FROM INFORMATION_SCHEMA.ROUTINES
 WHERE ROUTINE_TYPE = 'PROCEDURE'
 and (SPECIFIC_NAME like '%_$version$%')";
    }
}
