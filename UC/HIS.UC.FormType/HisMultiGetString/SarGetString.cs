using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.HisMultiGetString
{
    class SarGetString
    {
        public static List<DataGet> Get(string value, string key)
        {
            List<DataGet> datasuft = null;
            try
            {
                if (value == null) return new List<DataGet>();

                else if (value == "SAR_FORM_TYPE") datasuft = Config.SarFormTypeConfig.SarFormType.Select(o => new DataGet { ID = o.ID, CODE = o.FORM_TYPE_CODE, NAME = o.FORM_TYPE_NAME }).ToList();
               
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
