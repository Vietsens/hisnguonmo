using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite
{
    public class SqliteConfig
    {
        internal const string SQLITE_DATA_SOURCE = "InventecSqliteDB.s3db";//Sqlite Datasource name
        internal const string TABLE_SPACE__LOCAL_DATA = "TBL__LOCAL_DATA";//Sqlite local datatable name
        internal const string TABLE_SPACE__BACKEND_DATA = "TBL__BACKEND_DATA";//Sqlite backend datatable name
        internal const string TABLE_SPACE__CONFIG_DATA = "TBL__CONFIG_DATA";//Sqlite config datatable name

        public static string SQLITE_DATA_SOURCE__CUSTOM = "";//Sqlite Datasource name custom
    }
}