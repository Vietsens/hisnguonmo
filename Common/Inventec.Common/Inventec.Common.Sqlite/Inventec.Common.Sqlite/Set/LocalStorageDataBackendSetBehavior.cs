using Inventec.Common.Sqlite.Base;
using Inventec.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite
{
    class LocalStorageDataBackendSetBehavior : BussinessBase, IDelegacyListT
    {
        object data;
        string tableNameOrType;
        internal LocalStorageDataBackendSetBehavior(CommonParam param, object _data, string _tableNameOrType)
            : base()
        {
            this.data = _data;
            this.tableNameOrType = _tableNameOrType;
        }

        List<T> IDelegacyListT.Execute<T>()
        {
            try
            {
                List<T> result = default(List<T>);
                List<T> temp = (List<T>)data;

                SQLiteDatabase sQLiteDatabase;
                if (!String.IsNullOrEmpty(SqliteConfig.SQLITE_DATA_SOURCE__CUSTOM))
                    sQLiteDatabase = new SQLiteDatabase(SqliteConfig.SQLITE_DATA_SOURCE__CUSTOM);
                else
                    sQLiteDatabase = new SQLiteDatabase();

                //ArrayList dataTables = sQLiteDatabase.GetTables();
                //var tbl = (dataTables.ToArray().FirstOrDefault(o => ((o ?? "").ToString()) == result.GetType().ToString()) ?? "").ToString();
                //if (tbl != null)
                //{
                //    DataTable findData = sQLiteDatabase
                //        .GetDataTable(
                //            "select * from " + tbl
                //            );
                //    if (findData != null && findData.Rows.Count > 0)
                //    {
                //        //  DataTable dtTable = GetEmployeeDataTable();
                //        //  List<Employee> employeeList = dtTable.DataTableToList<Employee>();
                //        //Dictionary<String, Object> dataDic = new Dictionary<string, Object>();
                //        //dataDic.Add(ColumnConfig.COLUMN_NAME__VALUE, data);
                //        //sQLiteDatabase.Update(tbl, dataDic, ColumnConfig.COLUMN_NAME__KEY + " = " + key);
                //    }
                //    else
                //    {
                //        foreach (var item in temp)
                //        {
                //            foreach (var prop in item.GetType().GetProperties())
                //            {
                //                Dictionary<String, Object> dataDic = new Dictionary<string, Object>();
                //                dataDic.Add(prop.Name, prop.GetValue(item, null));
                //                sQLiteDatabase.Insert(tbl, dataDic);
                //            }
                //        }
                //    }
                //    //if (findData != null && findData.Rows.Count > 0)
                //    //{
                //    //    result = findData.DataTableToList<T>();
                //    //}
                //}
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return default(List<T>);
            }
        }
    }
}
