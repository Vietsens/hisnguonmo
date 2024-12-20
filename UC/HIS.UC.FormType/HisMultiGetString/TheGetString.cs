using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.HisMultiGetString
{
    public class TheGetString
    {
        public static List<DataGet> Get(string value, string key)
        {
            List<DataGet> datasuft = null;
            try
            {
                if (value == null) return new List<DataGet>();

                else if (value == "THE_BRANCH")
                {
                    datasuft = Config.TheFormTypeConfig.TheBranchs.Select(o => new DataGet { ID = o.ID, CODE = o.BRANCH_CODE, NAME = o.BRANCH_NAME }).ToList();
                }
                else if (value == "THE_BANK_BRANCH")
                {
                    datasuft = Config.TheFormTypeConfig.TheBranchs.Where(p => !string.IsNullOrWhiteSpace(p.BANK_BRANCH_CODE)).GroupBy(p => p.BANK_BRANCH_CODE).Select(o => new DataGet { ID = o.First().ID, CODE = o.First().BANK_BRANCH_CODE, NAME = o.First().BANK_NAME }).ToList();
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
