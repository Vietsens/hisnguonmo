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
        public AISuggestionData AI_Suggestion { get; set; }
    }

    public class AISuggestionData
    {
        public List<MediMatyTypeADO> SuggestedPrescriptions { get; set; }
        public string Explanation { get; set; }
    }
}
