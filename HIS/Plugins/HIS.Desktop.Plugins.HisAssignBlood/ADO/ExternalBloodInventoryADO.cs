using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisAssignBlood.ADO
{
    public class ExternalBloodInventoryADO
    {
        public string ABO { get; set; }
        public string Rh { get; set; }
        public string ElementID { get; set; }
        public string ElementName { get; set; }
        public int Volume { get; set; }
        public int Quantity { get; set; }
    }
    public class MinhTamInventoryRequest
    {
        public string Rh { get; set; }
        public string ABO { get; set; }
        public string ElementID { get; set; }
        public int Volume { get; set; }
    }

    public class MinhTamInventoryResponse
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
        public List<MinhTamInventoryInfo> InventoryInfo { get; set; }
    }

    public class MinhTamInventoryInfo
    {
        public string ABO { get; set; }
        public string ElementID { get; set; }
        public string ElementName { get; set; }
        public int Quantity { get; set; }
        public string Rh { get; set; }
        public int Volume { get; set; }
    }
    public class ExternalBloodTypeRowADO
    {
        public long BLOOD_TYPE_ID { get; set; }
        public string BLOOD_TYPE_CODE { get; set; }
        public string BLOOD_TYPE_NAME { get; set; }

        // Mapping theo yêu cầu
        public string ElementID { get; set; }   // HIS_BLOOD_TYPE.ELEMENT
        public int? Volume { get; set; }        // HIS_BLOOD_VOLUME.VOLUME

        // phục vụ search giống code gốc
        public string SERVICE_NAME_HIDDEN { get; set; }
        public string SERVICE_CODE_HIDDEN { get; set; }
    }
}
