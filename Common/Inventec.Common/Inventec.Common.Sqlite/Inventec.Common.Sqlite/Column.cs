using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite
{
    public class Column
    {
        public Column() { }
        public Column(string columnName, string columnType)
        {
            this.ColumnName = columnName;
            this.ColumnType = columnType;
        }

        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
    }
}
