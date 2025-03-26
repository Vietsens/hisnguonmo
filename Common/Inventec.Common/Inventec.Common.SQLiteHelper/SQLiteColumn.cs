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
using System.Text;

namespace Inventec.Common.SQLiteHelper
{
    public class SQLiteColumn
    {
        public string ColumnName = "";
        public bool PrimaryKey = false;
        public ColType ColDataType = ColType.Text;
        public bool AutoIncrement = false;
        public bool NotNull = false;
        public string DefaultValue = "";

        public SQLiteColumn()
        { }

        public SQLiteColumn(string colName)
        {
            ColumnName = colName;
            PrimaryKey = false;
            ColDataType = ColType.Text;
            AutoIncrement = false;
        }

        public SQLiteColumn(string colName, ColType colDataType)
        {
            ColumnName = colName;
            PrimaryKey = false;
            ColDataType = colDataType;
            AutoIncrement = false;
        }

        public SQLiteColumn(string colName, bool autoIncrement)
        {
            ColumnName = colName;

            if (autoIncrement)
            {
                PrimaryKey = true;
                ColDataType = ColType.Integer;
                AutoIncrement = true;
            }
            else
            {
                PrimaryKey = false;
                ColDataType = ColType.Text;
                AutoIncrement = false;
            }
        }

        public SQLiteColumn(string colName, ColType colDataType, bool primaryKey, bool autoIncrement, bool notNull, string defaultValue)
        {
            ColumnName = colName;

            if (autoIncrement)
            {
                PrimaryKey = true;
                ColDataType = ColType.Integer;
                AutoIncrement = true;
            }
            else
            {
                PrimaryKey = primaryKey;
                ColDataType = colDataType;
                AutoIncrement = false;
                NotNull = notNull;
                DefaultValue = defaultValue;
            }
        }
    }
}
