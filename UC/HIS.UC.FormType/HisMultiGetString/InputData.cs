using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.HisMultiGetString
{
    class InputData
    {
        public static List<DataGet> Get(string _jsonOutPut)
        {
            List<DataGet> datasuft = null;
            try
            {
                if (_jsonOutPut == null) return new List<DataGet>();
                //_jsonOutPut = "\"INPUT_DATA\":{0},\"DATA\":\"value 1,value 2\"";
                string toJson = "{" + (_jsonOutPut ?? "").Replace("{", "\"").Replace("}", "\"") + "}";

                Dictionary<string, string> dicFilter = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(toJson);
                foreach (var item in dicFilter.Where(o => o.Key.Contains("DATA")))
                {

                    //string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(dicFilter["DATA"]);
                    //datasuft = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataGet>>(jsonData);
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        string[] listName = item.Value.Split(',');
                        if (listName.Length < 2)
                        {
                            continue;
                        }
                        datasuft = new List<DataGet>();
                        for (int i = 0; i < listName.Length; i++)
                        {
                            DataGet dt = new DataGet();
                            dt.ID = i + 1;
                            dt.CODE = string.Format("{0:000}", i + 1);
                            dt.NAME = listName[i];
                            datasuft.Add(dt);
                        }

                    }

                }


                //datasuft = datasuft.OrderBy(o => o.CODE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return datasuft;
        }

    }
}
