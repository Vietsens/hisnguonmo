using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ADO
{
    public class MediMatySuggestionsADO
    {
        public MediMatySuggestionsADO(MediMatySuggestionsADO ado)
        {
                
        }      

        [JsonProperty("Mã thuốc")]
        public string MEDICINE_TYPE_CODE { get; set; }
        [JsonProperty("Tên thuốc")]
        public string MEDICINE_TYPE_NAME { get; set; }
        [JsonProperty("Đơn vị tính")]
        public string SERVICE_UNIT_CODE_NAME { get; set; }
        [JsonProperty("Cách dùng")]
        public string HTU_CODE_NAME { get; set; }
        [JsonProperty("Kiểu kê đơn")]
        public int DataType { get; set; }
        [JsonProperty("Số lượng")]
        public decimal AMOUNT { get; set; }
        [JsonProperty("Sáng")]
        public decimal Morning { get; set; }
        [JsonProperty("Trưa")]
        public decimal Noon { get; set; }
        [JsonProperty("Chiều")]
        public decimal Afternoon { get; set; }
        [JsonProperty("Tối")]
        public decimal Evening { get; set; }
        [JsonProperty("Hướng dẫn sử dụng")]
        public string TUTORIAL { get; set; }
        [JsonProperty("Hao phí")]
        public bool IsExpend { get; set; }
    }
}
