using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Design.Template3
{
    public class SearchADO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public SearchADO(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
