using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.MD
{
    public class ListData
    {
        [XmlArray("Invoices")]
        public List<Invoice_BM> Datas { get; set; }

        public string GetXML()
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer sr = new XmlSerializer(typeof(ListData));
            sr.Serialize(ms, this, ns);

            ms.Position = 0;
            return System.Text.Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
