using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.HisMultiGetString
{
    public class AosGetString
    {
        public static List<DataGet> Get(string value, string key)
        {
            List<DataGet> datasuft = null;
            try
            {
                if (value == null) return new List<DataGet>();

                else if (value == "AOS_ACCOUNT_TYPE") datasuft = Config.AosFormTypeConfig.AosAccountTpye.Select(o => new DataGet { ID = o.ID, CODE = o.ACCOUNT_TYPE_CODE, NAME = o.ACCOUNT_TYPE_NAME }).ToList();
                else if (value == "AOS_BANK_ACCOUNT_TYPE")
                {
                    var lstAos = Config.AosFormTypeConfig.AosAccountTpye.Where(o => !string.IsNullOrWhiteSpace(o.BANK_CODE)).ToList();
                    if (lstAos != null)
                        datasuft = lstAos.Select(o => new DataGet { ID = o.ID, CODE = o.ACCOUNT_TYPE_CODE, NAME = o.ACCOUNT_TYPE_NAME }).ToList();
                }

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
