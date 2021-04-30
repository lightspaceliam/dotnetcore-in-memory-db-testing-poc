using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Entity.Services.Tests.Helpers
{
    public class SqliteCommandInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            if (command.CommandText.Contains("nvarchar(max)"))
            {
                //  When you set a string length greater then 4000,
                //  "nvarchar(max)" will be set. This is not yet handled
                //  by the frameworks translation between MS SQL & SQLite.
                command.CommandText = command.CommandText.Replace("nvarchar(max)", "nvarchar(8000)");
            }
            return base.NonQueryExecuting(command, eventData, result);
        }
    }
}
