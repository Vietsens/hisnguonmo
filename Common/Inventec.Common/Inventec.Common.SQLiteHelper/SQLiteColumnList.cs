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

    public class SQLiteColumnList : IList<SQLiteColumn>
    {
        List<SQLiteColumn> _lst = new List<SQLiteColumn>();

        private void CheckColumnName(string colName)
        {
            for (int i = 0; i < _lst.Count; i++)
            {
                if (_lst[i].ColumnName == colName)
                    throw new Exception("Column name of \"" + colName + "\" is already existed.");
            }
        }

        public int IndexOf(SQLiteColumn item)
        {
            return _lst.IndexOf(item);
        }

        public void Insert(int index, SQLiteColumn item)
        {
            CheckColumnName(item.ColumnName);

            _lst.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _lst.RemoveAt(index);
        }

        public SQLiteColumn this[int index]
        {
            get
            {
                return _lst[index];
            }
            set
            {
                if (_lst[index].ColumnName != value.ColumnName)
                {
                    CheckColumnName(value.ColumnName);
                }

                _lst[index] = value;
            }
        }

        public void Add(SQLiteColumn item)
        {
            CheckColumnName(item.ColumnName);

            _lst.Add(item);
        }

        public void Clear()
        {
            _lst.Clear();
        }

        public bool Contains(SQLiteColumn item)
        {
            return _lst.Contains(item);
        }

        public void CopyTo(SQLiteColumn[] array, int arrayIndex)
        {
            _lst.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _lst.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(SQLiteColumn item)
        {
            return _lst.Remove(item);
        }

        public IEnumerator<SQLiteColumn> GetEnumerator()
        {
            return _lst.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _lst.GetEnumerator();
        }
    }

}
