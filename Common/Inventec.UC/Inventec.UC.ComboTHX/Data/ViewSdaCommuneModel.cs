using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX.Data
{
    internal class ViewSdaCommuneModel : SDA.EFMODEL.DataModels.V_SDA_COMMUNE
    {
        public long PROVINCE_ID { get; set; }
        public string PROVINCE_CODE { get; set; }
        public string PROVINCE_NAME { get; set; }
        public string DISTRICT_INITIAL_NAME { get; set; }

        public ViewSdaCommuneModel()
        {
        }
    }
}
