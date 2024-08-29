using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite
{
    public class Table
    {
        public Table()
        { }
        public Table(string tableName, List<Column> columns)
        {
            this.TableName = tableName;
            this.Columns = columns;
        }

        public string TableName { get; set; }
        public List<Column> Columns { get; set; }
    }
}
