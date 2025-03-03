using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.CopyFilter
{
    public class CopyFilter
    {
        private static List<string> FilterNotCopys = new List<string>() { "FTHIS000001" };

        public static string CopyFilterProcess(V_SAR_RETY_FOFI config, string JsonOutput, string JsonFilter)
        {
            try
            {
                if (FilterNotCopys.Contains(config.FORM_FIELD_CODE))
                {
                    return null;
                }

                //"{\"DATE_TIME\":null,\"DEPARTMENT_IDs\":[22,173],\"ROOM_IDs\":[\"444\",\"85\"]}"
                var arrSplitJsonOutput = JsonOutput.Split(new string[] { "\"" }, StringSplitOptions.None);

                Dictionary<string, object> arrSplitJsonFilter = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonFilter);
                if (arrSplitJsonOutput.Length > 1)
                {
                    if (arrSplitJsonFilter != null)
                    {
                        if (arrSplitJsonFilter.ContainsKey(arrSplitJsonOutput[1]))
                        {
                            var value = arrSplitJsonFilter[arrSplitJsonOutput[1]];
                            if (value != null)
                            {
                                return Newtonsoft.Json.JsonConvert.SerializeObject(value);
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return null;
        }

    }
}
