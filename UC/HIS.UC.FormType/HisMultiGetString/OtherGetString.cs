using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.HisMultiGetString
{
    class OtherGetString
    {
        public static List<DataGet> Get(string value, string key)
        {
            List<DataGet> datasuft = null;
            try
            {
                if (value == null) return new List<DataGet>();

                else if (value == "HeinLiveAreaData") datasuft = Config.OtherFormTypeConfig.HeinLiveAreas.Select(o => new DataGet { ID = 1, CODE = o.HeinLiveCode, NAME = o.HeinLiveName }).ToList();

                else if (value == "HeinRightRouteTypeData") datasuft = Config.OtherFormTypeConfig.HeinRightRouteTypes.Select(o => new DataGet { ID = 1, CODE = o.HeinRightRouteTypeCode, NAME = o.HeinRightRouteTypeName }).ToList();

                //else if (value == "INPUT_DATA") datasuft = Config.OtherFormTypeConfig.HeinRightRouteTypes.Select(o => new DataGet { ID = 1, CODE = o.HeinRightRouteTypeCode, NAME = o.HeinRightRouteTypeName }).ToList();

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
