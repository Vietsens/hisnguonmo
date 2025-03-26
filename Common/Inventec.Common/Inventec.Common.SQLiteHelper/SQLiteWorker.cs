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
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.SQLiteHelper
{
    public class SQLiteWorker
    {
        String dbConnection;
        String dbFile;

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="dbFile">The File containing the DB</param>
        public SQLiteWorker(String dbFile)
        {
            this.dbConnection = String.Format("Data Source={0}", dbFile);
            this.dbFile = dbFile;
            CheckNotExistsOrCreateDB();
        }

        /// <summary>
        ///     Single Param Constructor for specifying advanced connection options.
        /// </summary>
        /// <param name="connectionOpts">A dictionary containing all desired options and their values</param>
        private SQLiteWorker(Dictionary<String, String> connectionOpts)
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

        #region DB Info
        SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(dbConnection);
        }

        public DataTable ShowDatabase()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        dt = sh.ShowDatabase();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        public DataTable GetTableList()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        dt = sh.GetTableList();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        public DataTable GetTableStatus()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        dt = sh.GetTableStatus();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        public DataTable GetTableStatus(string tableName)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        dt = sh.GetColumnStatus(tableName);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }
        #endregion

        #region Query
        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(String tableName, Dictionary<String, Object> dicData, Dictionary<string, object> dicCond)
        {
            bool rs = false;
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.Update(tableName, dicData, dicCond);
                        conn.Close();
                        rs = true;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return rs;
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(String tableName, String where)
        {
            bool rs = false;
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.Execute(String.Format("delete from {0} where {1};", tableName, where));
                        conn.Close();
                        rs = true;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return rs;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Insert(String tableName, Dictionary<String, Object> data)
        {
            bool rs = false;
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.Insert(tableName, data);
                        conn.Close();
                        rs = true;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return rs;
        }

        public DataTable Select(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(dbConnection);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandType = CommandType.Text;
                mycommand.CommandText = sql;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;

            //DataTable dt = null;
            //try
            //{
            //    using (SQLiteConnection conn = CreateConnection())
            //    {
            //        using (SQLiteCommand cmd = new SQLiteCommand())
            //        {
            //            cmd.Connection = conn;
            //            conn.Open();

            //            SQLiteHelper sh = new SQLiteHelper(cmd);
            //            dt = sh.Select(sql);
            //            conn.Close();
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw new Exception(e.Message);
            //}
            //return dt;
        }

        public DataTable Select(string sql, Dictionary<string, object> dicParameters = null)
        {
            DataTable dt = null;
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        dt = sh.Select(sql, dicParameters);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        public DataTable Select(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            DataTable dt = null;
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        dt = sh.Select(sql, parameters);
                        conn.Close();
                    }
                }
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
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(dbConnection);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                mycommand.CommandType = CommandType.Text;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
                if (dt == null || dt.Rows.Count == 0)
                {
                    throw new Exception("GetDataTable data return empty. sql = " + sql + ", ConnectionString = " + cnn.ConnectionString);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        public void Execute(string sql)
        {
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    conn.DefaultTimeout = 3;
                    conn.BusyTimeout = 3;
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.Execute(sql);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Execute(string sql, Dictionary<string, object> dicParameters = null)
        {
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.Execute(sql, dicParameters);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Execute(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.Execute(sql, parameters);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public object ExecuteScalar(string sql)
        {
            object rs = null;
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        rs = sh.ExecuteScalar(sql);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return rs;
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> dicParameters = null)
        {
            object rs = null;
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        rs = sh.ExecuteScalar(sql, dicParameters);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return rs;
        }

        public object ExecuteScalar(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            object rs = null;
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        rs = sh.ExecuteScalar(sql, parameters);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return rs;
        }

        public dataType ExecuteScalar<dataType>(string sql, Dictionary<string, object> dicParameters = null)
        {
            dataType rs = default(dataType);
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        rs = sh.ExecuteScalar<dataType>(sql, dicParameters);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return rs;
        }

        public dataType ExecuteScalar<dataType>(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            dataType rs = default(dataType);
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        rs = sh.ExecuteScalar<dataType>(sql, parameters);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return rs;
        }

        public dataType ExecuteScalar<dataType>(string sql)
        {
            dataType rs = default(dataType);
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        rs = sh.ExecuteScalar<dataType>(sql);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return rs;
        }
        #endregion

        #region Utilities
        public string Escape(string data)
        {
            string rs = default(string);
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        rs = sh.Escape(data);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return rs;
        }

        public void CreateTable(SQLiteTable table)
        {
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.CreateTable(table);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void RenameTable(string tableFrom, string tableTo)
        {
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.RenameTable(tableFrom, tableTo);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void CopyAllData(string tableFrom, string tableTo)
        {
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.CopyAllData(tableFrom, tableTo);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DropTable(string table)
        {
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.DropTable(table);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void UpdateTableStructure(string targetTable, SQLiteTable newStructure)
        {
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.UpdateTableStructure(targetTable, newStructure);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void AttachDatabase(string database, string alias)
        {
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.AttachDatabase(database, alias);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DetachDatabase(string alias)
        {
            try
            {
                using (SQLiteConnection conn = CreateConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();

                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        sh.DetachDatabase(alias);
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion
    }
}
