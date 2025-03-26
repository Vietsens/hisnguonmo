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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Linq;

namespace Inventec.Common.Sqlite
{
    public class SQLiteDatabase
    {
        String dbConnection;
        String dbFile;

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="dbFile">The File containing the DB</param>
        public SQLiteDatabase(String dbFile)
        {
            this.dbConnection = String.Format("Data Source={0};Version=3;", dbFile);
            this.dbFile = dbFile;
            CheckNotExistsOrCreateDB();

        }

        /// <summary>
        ///     Single Param Constructor for specifying advanced connection options.
        /// </summary>
        /// <param name="connectionOpts">A dictionary containing all desired options and their values</param>
        private SQLiteDatabase(Dictionary<String, String> connectionOpts)
        {
            String str = "";
            foreach (KeyValuePair<String, String> row in connectionOpts)
            {
                str += String.Format("{0}={1}; ", row.Key, row.Value);
            }
            str = str.Trim().Substring(0, str.Length - 1);
            dbConnection = str;
        }

        private void CheckNotExistsOrCreateDB()
        {
            //Kiểm tra file DB sqlite đã được tạo chưa
            //Trường hợp chưa tồn tại thì thực hiện tạo file DB, tạo bảng HIS_DATA_STORE
            if (!System.IO.File.Exists(this.dbFile))
            {
                //System.Windows.Forms.MessageBox.Show(this.dbFile);
                int idSeperator = this.dbFile.LastIndexOf('\\');
                //System.Windows.Forms.MessageBox.Show(idSeperator + "");
                if (idSeperator >= 0)
                {
                    string path = this.dbFile.Substring(0, idSeperator);
                    //System.Windows.Forms.MessageBox.Show(path);
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                }
                SQLiteConnection.CreateFile(this.dbFile);
                //System.Windows.Forms.MessageBox.Show("Finisf CheckNotExistsOrCreateDB");
            }
        }

        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            SQLiteConnection cnn;
            string mess = "";
            try
            {
                cnn = new SQLiteConnection(dbConnection);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
                //if (dt == null || dt.Rows.Count == 0)
                //{
                //    for (int i = 0; i < 5; i++)
                //    {
                //        if (i == 4)
                //        {
                //            mess += "cnn.ConnectionString = " + cnn.ConnectionString + "____sql = " + sql + "____";
                //            throw new Exception(mess + ", i = " + i);
                //        }
                //        else
                //        {
                //            dt = Select(sql);
                //            if (dt != null && dt.Rows.Count > 0)
                //            {
                //                return dt;
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable Select(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(dbConnection);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            }
            catch (Exception e)
            {
                //throw new Exception(e.Message);
            }
            return dt;
        }

        public List<T> GetList<T>(string tablename)
        {
            return GetList<T>(tablename, true, "");
        }

        public List<T> GetList<T>(string tablename, string where)
        {
            return GetList<T>(tablename, true, where);
        }

        public List<T> GetList<T>(string tablename, bool isRemoveSpecialPropertyType, string where)
        {
            List<T> results = new List<T>();
            using (SQLiteConnection connect = new SQLiteConnection(dbConnection))
            {
                connect.Open();
                using (SQLiteCommand fmd = connect.CreateCommand())
                {
                    fmd.CommandText = @"SELECT * FROM " + tablename + (String.IsNullOrEmpty(where) ? "" : " where 1 = 1 AND " + where);
                    SQLiteDataReader r = fmd.ExecuteReader();
                    Type type = typeof(T);
                    System.Reflection.PropertyInfo[] propertyInfoOrderFields = type.GetProperties();

                    string propertyName = "";
                    string propertyType = "";
                    string valueDT = "";
                    try
                    {
                        while (r.Read())
                        {
                            T t = (T)Activator.CreateInstance(type);
                            if (propertyInfoOrderFields != null && propertyInfoOrderFields.Count() > 0)
                            {
                                foreach (var pr in propertyInfoOrderFields)
                                {
                                    bool valid = true;
                                    if (isRemoveSpecialPropertyType)
                                        valid = valid && (!pr.PropertyType.ToString().Contains(SystemTypes.ICollection) && !pr.PropertyType.ToString().Contains(SystemTypes.DataModel));
                                    if (valid)
                                    {
                                        propertyName = pr.Name;
                                        propertyType = pr.PropertyType.FullName;
                                        valueDT = (r[pr.Name] ?? "").ToString();
                                        if (r[pr.Name] == null || r[pr.Name] == DBNull.Value)
                                        {
                                            pr.SetValue(t, null);
                                        }
                                        else if (pr.PropertyType.FullName == SystemTypes.String)
                                        {
                                            pr.SetValue(t, (string)(r[pr.Name] ?? ""));
                                        }
                                        else if (pr.PropertyType.FullName == SystemTypes.Int16Nullable)
                                        {
                                            pr.SetValue(t, Convert.ToInt16(r[pr.Name]));
                                        }
                                        else if (pr.PropertyType.FullName == SystemTypes.Int64Nullable)
                                        {
                                            pr.SetValue(t, Convert.ToInt64(r[pr.Name]));
                                        }
                                        else if (pr.PropertyType.FullName == SystemTypes.DecimalNullable)
                                        {
                                            pr.SetValue(t, Convert.ToDecimal(r[pr.Name]));
                                        }
                                        else
                                        {
                                            pr.SetValue(t, r[pr.Name]);
                                        }
                                    }
                                }
                            }
                            results.Add(t);
                        }

                        connect.Close();
                    }
                    catch (Exception ex)
                    {
                        connect.Close();
                        throw new Exception("tablename = " + tablename + "__propertyName = " + propertyName + "__" + "propertyType = " + propertyType + "__" + "valueDT = " + valueDT + "__", ex);
                    }
                }
            }

            return results;
        }

        public ArrayList GetTables()
        {
            ArrayList list = new ArrayList();

            // executes query that select names of all tables in master table of the database
            String query = "SELECT name FROM sqlite_master " +
                    "WHERE type = 'table'" +
                    "ORDER BY 1";
            try
            {
                DataTable table = GetDataTable(query);

                // Return all table names in the ArrayList
                foreach (DataRow row in table.Rows)
                {
                    list.Add(row.ItemArray[0].ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            cnn.Close();

            return rowsUpdated;
        }

        public bool ExecuteWithData<T>(List<T> data, string tableName)
        {
            return ExecuteWithData<T>(data, tableName);
        }

        public bool ExecuteWithData<T>(List<T> data, string tableName, bool isRemoveSpecialPropertyType)
        {
            bool success = false;
            string columns = "";
            string values = "";
            string cmdText = "";
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(dbConnection);
                cnn.Open();
                using (var cmd = new SQLiteCommand(cnn))
                {
                    using (var transaction = cnn.BeginTransaction())
                    {
                        Type type = typeof(T);
                        System.Reflection.PropertyInfo[] propertyInfoOrderFields = type.GetProperties();
                        if (propertyInfoOrderFields != null && propertyInfoOrderFields.Count() > 0)
                        {
                            foreach (var pr in propertyInfoOrderFields)
                            {
                                bool valid = true;
                                if (isRemoveSpecialPropertyType)
                                    valid = valid && (!pr.PropertyType.ToString().Contains(SystemTypes.ICollection) && !pr.PropertyType.ToString().Contains(SystemTypes.DataModel));
                                if (valid)
                                {
                                    columns += String.Format(" {0},", pr.Name);
                                }
                            }
                            columns = columns.Substring(0, columns.Length - 1);
                        }

                        foreach (var item in data)
                        {
                            values = "";
                            if (propertyInfoOrderFields != null && propertyInfoOrderFields.Count() > 0)
                            {
                                foreach (var pr in propertyInfoOrderFields)
                                {
                                    bool valid = true;
                                    if (isRemoveSpecialPropertyType)
                                        valid = valid && (!pr.PropertyType.ToString().Contains(SystemTypes.ICollection) && !pr.PropertyType.ToString().Contains(SystemTypes.DataModel));
                                    if (valid)
                                    {
                                        if (pr.GetValue(item) == null)
                                        {
                                            values += " NULL,";
                                        }
                                        else
                                        {
                                            if (pr.PropertyType == typeof(string))
                                            {
                                                values += String.Format(" '{0}',", ((string)pr.GetValue(item)).Replace("'", "''"));
                                            }
                                            else
                                            {
                                                values += String.Format(" {0},", ConvertNumber.NumberToNumberRoundAuto(Convert.ToDecimal(pr.GetValue(item)), 0));
                                            }
                                        }
                                    }
                                }
                                values = values.Substring(0, values.Length - 1);
                            }

                            cmdText = String.Format("INSERT INTO {0} ({1}) VALUES ({2});", tableName, columns, values);
                            cmd.CommandText = cmdText;

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        success = true;
                    }
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Insert du lieu vao bang {0} fail. Co the do cau truc bang da bi thay doi, can tao lai bang de co cau truc bang moi nhat. ____cmdText:{1}", tableName, cmdText), ex);
            }

            return success;
        }

        /// <summary>
        ///     Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="sql">The query to run.</param>
        /// <returns>A string.</returns>
        public string ExecuteScalar(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            object value = mycommand.ExecuteScalar();
            cnn.Close();
            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(String tableName, Dictionary<String, Object> data, String where)
        {
            String vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1)
            {
                foreach (KeyValuePair<String, Object> val in data)
                {
                    if (val.Value is string)
                    {
                        vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                    }
                    else
                    {
                        vals += String.Format(" {0} = {1},", val.Key.ToString(), val.Value.ToString());
                    }
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            //try
            //{
            if (String.IsNullOrEmpty(where))
            {
                where = "1 = 1";
            }
            this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            //}
            //catch
            //{
            //    returnCode = false;
            //}
            return returnCode;
        }

        public void DropTable(string table)
        {
            this.ExecuteNonQuery(string.Format("drop table if exists {0}", table));
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            //try
            //{
            this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            //}
            //catch (Exception fail)
            //{
            //    //Inventec.Common.Logging.LogSystem.Warn(fail.Message);
            //    returnCode = false;
            //}

            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Insert(String tableName, Dictionary<String, Object> data)
        {
            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<String, Object> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                if (val.Value is string)
                {
                    values += String.Format(" '{0}',", val.Value);
                }
                else
                {
                    values += String.Format(" {0},", val.Value);
                }
            }

            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);

            //try
            //{
            this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
            //}
            //catch (Exception fail)
            //{
            //    //Inventec.Common.Logging.LogSystem.Warn(fail.Message);
            //    returnCode = false;
            //}

            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearDB()
        {
            DataTable tables;
            try
            {
                tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows)
                {
                    this.ClearTable(table["NAME"].ToString());
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearTable(String table)
        {
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0};", table));
                try
                {
                    //this.ExecuteNonQuery(String.Format("delete from sqlite_sequence where name='{0}';", table));
                }
                catch { }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void WriteLog(string filePath, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(message);
            // flush every 20 seconds as you do it
            File.AppendAllText(filePath + "log.txt", sb.ToString());
            sb.Clear();
        }
    }
}
