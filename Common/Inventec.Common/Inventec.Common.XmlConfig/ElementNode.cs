using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.XmlConfig
{
    public class ElementNode
    {
        public ElementNode() { }
        public string KeyCode { get; set; }
        public string Title { get; set; }
        public object Value { get; set; }
        public object DefaultValue { get; set; }
        public string ValueType { get; set; }
        public string ValueTypeDescription { get; set; }
        public object ValueAllowMin { get; set; }
        public object ValueAllowMax { get; set; }
        public object ValueAllowIn { get; set; }
        public string Tutorial { get; set; }
        public long CreateTime { get; set; }
        public long ModifyTime { get; set; }
        public string Creator { get; set; }
        public string Modifier { get; set; }
    }
}
