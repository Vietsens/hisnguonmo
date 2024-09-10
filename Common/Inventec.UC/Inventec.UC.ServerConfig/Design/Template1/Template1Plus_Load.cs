using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ServerConfig.Design.Template1
{
    internal partial class Template1
    {
        private void Template1_Load(object sender, EventArgs e)
        {
            try
            {
                Data.DataStore.LoadSystemConfigKey();
                SetListDataShow();
                ListElement = Data.DataStore.SystemConfigXLM.GetElements();
                gridControlSystemConfig.DataSource = ListData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetListDataShow()
        {
            try
            {
                Data.DataShow dataacs = new Data.DataShow() { KEYCODE = "Inventec.Token.ClientSystem.Acs.Base.Uri", VALUE = Data.DataAppConfig.ACS_BASE_URI, DEFAULT_VALUE = "", DESCRIPTION = "Đia chỉ cấu hình kết nối đến Resource server xác thực ACS" };
                ListData.Add(dataacs);
                Data.DataShow data__Fss = new Data.DataShow() { KEYCODE = "fss.uri.base", VALUE = Data.DataAppConfig.FSS_BASE_URI, DEFAULT_VALUE = "", DESCRIPTION = "Đia chỉ cấu hình kết nối đến Resource server quản lý file tập trung FSS" };
                ListData.Add(data__Fss);
                foreach (var item in Data.DataStore.SystemConfigXLM.GetElements())
                {
                    //string value = string.IsNullOrEmpty(item.Value.ToString()) ? item.DefaultValue.ToString() : item.Value.ToString();
                    Data.DataShow data = new Data.DataShow() { KEYCODE = item.KeyCode, VALUE = item.Value.ToString(), DEFAULT_VALUE = item.DefaultValue.ToString(), DESCRIPTION = item.Tutorial };
                    ListData.Add(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
