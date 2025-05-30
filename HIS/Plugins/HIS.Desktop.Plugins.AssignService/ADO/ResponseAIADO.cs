using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignService.ADO
{
    internal class ResponseAIADO
    {
        [JsonProperty("assignservices")]
        public List<object> AssignServices { get; set; }  

        [JsonProperty("ai_suggestion")]
        public AISuggestion AISuggestion { get; set; }
    }

    public class AISuggestion
    {
        [JsonProperty("Chỉ định gợi ý")]
        public List<AISuggestionItem> ChiDinhGoiY { get; set; }

        [JsonProperty("Giải thích")]
        public string Note { get; set; }

        [JsonProperty("Kết thúc")]
        public string Warning { get; set; }
    }

    public class AISuggestionItem
    {
        [JsonProperty("Mã dịch vụ")]
        public string TDL_SERVICE_CODE { get; set; }

        [JsonProperty("Tên dịch vụ")]
        public string TDL_SERVICE_NAME { get; set; }

        [JsonProperty("Số lượng")]
        public decimal AMOUNT { get; set; }

        [JsonProperty("Ghi chú")]
        public string InstructionNote { get; set; }

        [JsonProperty("Hao phí")]
        public bool IsExpend { get; set; }
    }
}
