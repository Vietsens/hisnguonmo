using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.ADO
{
    public class SupplierADO : HIS_SUPPLIER
    {
        public string SUPPLIER_NAME_UNSIGN { get; set; }
        public bool isChecked { get; set; }
    }
}
