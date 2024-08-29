using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Data
{
    public class DataInit3
    {
        public string SdaUri;

        public string AppCode;

        public long PageNum;

        public string Filter;

        public DataInit3(string sdaUrl, string appCode, long pageNum, string filter)
        {
            try
            {
                this.SdaUri = sdaUrl;
                this.AppCode = appCode;
                this.PageNum = pageNum;
                this.Filter = filter;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
