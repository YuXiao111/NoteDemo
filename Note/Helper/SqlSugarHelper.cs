using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Helper
{
    public class SqlSugarHelper 
    {
        public static SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = "datasource=System.db",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = true 
        },
      db => {
          db.Aop.OnLogExecuting = (sql, pars) =>
          {
              Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));
          };
      });
    }
}
