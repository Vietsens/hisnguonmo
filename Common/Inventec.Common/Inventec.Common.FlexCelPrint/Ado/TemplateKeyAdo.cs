using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCelPrint.Ado
{
    class TemplateKeyAdo
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string IsObject { get; set; }

        public TemplateKeyAdo(string name, string value)
        {
            this.Name = name;
            this.Value = value;
            if (Name.Contains('.'))
            {
                IsObject = "b";
            }
            else
            {
                IsObject = "a";
            }
        }

        public TemplateKeyAdo()
        {
        }
    }
}
