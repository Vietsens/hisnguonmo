using Inventec.Common.Sqlite.Base;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite
{
    class LocalStorageDataGetBehavior : BussinessBase, IDelegacy
    {
        string key;
        string tableNameOrType;
        internal LocalStorageDataGetBehavior(CommonParam param, string _key, string _tableNameOrType)
            : base()
        {
            this.key = _key;
            this.tableNameOrType = _tableNameOrType;
        }

        object IDelegacy.Run()
        {
            try
            {
                object result = null;

                SQLiteDatabase sQLiteDatabase;
                if (!String.IsNullOrEmpty(SqliteConfig.SQLITE_DATA_SOURCE__CUSTOM))
                    sQLiteDatabase = new SQLiteDatabase(SqliteConfig.SQLITE_DATA_SOURCE__CUSTOM);
                else
                    sQLiteDatabase = new SQLiteDatabase();

                DataTable resultData = sQLiteDatabase.GetDataTable(
                    "select * from " + this.tableNameOrType +
                    " where " + ColumnConfig.COLUMN_NAME__KEY + " = " + key
                    );

                if (resultData != null && resultData.Rows.Count > 0)
                {
                    result = resultData.Rows[0][ColumnConfig.COLUMN_NAME__VALUE];
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
