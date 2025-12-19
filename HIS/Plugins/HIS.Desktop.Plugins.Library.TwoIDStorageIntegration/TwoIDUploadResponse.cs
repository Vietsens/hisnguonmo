using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.TwoIDStorageIntegration
{
    internal class TwoIDUploadResponse
    {
        public bool status { get; set; }
        public List<TwoIDUploadItem> data { get; set; }

        public string GetUrl(string type)
        {
            return data?.FirstOrDefault(x => x.type == type)?.url;
        }
    }
    public class TwoIDUploadItem
    {
        public string type { get; set; }
        public string url { get; set; }
    }
}
