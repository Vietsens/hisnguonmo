using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.HisMultiGetString
{
    class SdaGetString
    {
        public static List<DataGet> Get(string value, string key)
        {
            List<DataGet> datasuft = null;
            try
            {
                if (value == null) return new List<DataGet>();

                else if (value == "SDA_DISTRICT") datasuft = Config.SdaFormTypeConfig.SdaDistrict.Select(o => new DataGet { ID = o.ID, CODE = o.DISTRICT_CODE, NAME = o.DISTRICT_NAME, PARENT = o.PROVINCE_ID }).ToList();


                else if (value == "SDA_COMMUNE") datasuft = Config.SdaFormTypeConfig.SdaCommune.Select(o => new DataGet { ID = o.ID, CODE = o.COMMUNE_CODE, NAME = o.COMMUNE_NAME, PARENT = o.DISTRICT_ID }).ToList();


                else if (value == "SDA_PROVINCE") datasuft = Config.SdaFormTypeConfig.SdaProvince.Select(o => new DataGet { ID = o.ID, CODE = o.PROVINCE_CODE, NAME = o.PROVINCE_NAME }).ToList();

                datasuft = datasuft.OrderBy(o => o.NAME).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return datasuft;
        }
    }
}
