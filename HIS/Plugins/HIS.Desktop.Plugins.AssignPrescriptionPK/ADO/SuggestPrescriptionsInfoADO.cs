using MOS.SDO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ADO
{
    public class SuggestPrescriptionsInfoADO
    {
        [JsonProperty("ai_suggestion")]
        public AISuggestionData ai_suggestion { get; set; }
    }
}
