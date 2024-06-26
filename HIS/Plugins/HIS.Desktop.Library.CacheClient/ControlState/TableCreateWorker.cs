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
using Inventec.Core;
using RDCACHE.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Library.CacheClient.ControlState
{
    public class TableCreateWorker
    {
        const string seperate = ",";
        public TableCreateWorker() { }

        internal bool CreateTableNew<T>(string tableName)
        {
            bool valid = true;
            TableCreateProcess sqliteDataBaseCreate = new TableCreateProcess();
            if (!TableCheck.CheckExistsTable(tableName))
            {
                string scriptCreateTable = "create table if not exists {0} ({1})";
                string scriptGenerateColumns = "";
                string tempGenerateColumn = "{0} {1}";

                Type type = typeof(T);
                System.Reflection.PropertyInfo[] propertyInfoOrderFields = type.GetProperties();
                if (propertyInfoOrderFields != null && propertyInfoOrderFields.Count() > 0)
                {
                    int dem = 0;
                    foreach (var pr in propertyInfoOrderFields)
                    {
                        if (!pr.PropertyType.ToString().Contains(SystemTypes.ICollection) && !pr.PropertyType.ToString().Contains(SystemTypes.DataModel))
                        {
                            scriptGenerateColumns += ((dem > 0 ? seperate : "") + String.Format(tempGenerateColumn, pr.Name, sqliteDataBaseCreate.GetSqliteType(pr.PropertyType)));
                            if (pr.PropertyType.FullName == SystemTypes.Decimal || pr.PropertyType.FullName == SystemTypes.Int16 || pr.PropertyType.FullName == SystemTypes.Int64)
                            {
                                scriptGenerateColumns += " NOT NULL";
                            }

                            dem++;
                        }
                    }
                }

                valid = sqliteDataBaseCreate.CreateTable(String.Format(scriptCreateTable, tableName, scriptGenerateColumns));
                if (valid)
                {
                    Inventec.Common.Logging.LogSystem.Info("create table " + tableName + " success");
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("create table " + tableName + " fail____" + String.Format(scriptCreateTable, tableName, scriptGenerateColumns));
                }
            }
            return valid;
        }
    }
}
