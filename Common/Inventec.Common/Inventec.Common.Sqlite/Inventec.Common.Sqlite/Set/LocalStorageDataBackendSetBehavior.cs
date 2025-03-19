/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
