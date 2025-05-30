using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ADO
{
    public class AISuggestionData
    {
        [JsonProperty("Đơn thuốc gợi ý")]
        public List<MediMatySuggestionsADO> MediMatySuggestions { get; set; }
        [JsonProperty("Giải thích")]
        public string Explanation { get; set; }
        [JsonProperty("Kết thúc")]
        public string End { get; set; }
    }
}
