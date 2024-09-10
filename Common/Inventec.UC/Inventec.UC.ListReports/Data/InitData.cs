using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Data
{
    public class InitData
    {
        internal Inventec.Token.ClientSystem.ClientTokenManager clientToken;
        internal Inventec.Common.WebApiClient.ApiConsumer sarconsumer;
        internal Inventec.Common.WebApiClient.ApiConsumer sdaconsumer;
        internal Inventec.Common.WebApiClient.ApiConsumer acsconsumer;
        internal long numPage;
        internal string nameIcon;
        internal bool isAdmin;
        public CultureInfo Language { get; set; }
        public ProcessCopy ProcessCopy { get; set; }

        public InitData(Inventec.Common.WebApiClient.ApiConsumer sarConsumer, Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Inventec.Common.WebApiClient.ApiConsumer acsComsumer, Inventec.Token.ClientSystem.ClientTokenManager clientTokenManager, long NumPaging, string NameIcon, CultureInfo Language, bool _isAdmin)
        {
            try
            {
                this.sarconsumer = sarConsumer;
                this.sdaconsumer = sdaConsumer;
                this.acsconsumer = acsComsumer;
                this.clientToken = clientTokenManager;
                this.numPage = NumPaging;
                this.nameIcon = NameIcon;
                this.Language = Language;
                this.isAdmin = _isAdmin;
                Inventec.UC.ListReports.Base.LanguageManager.Init(this.Language);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
