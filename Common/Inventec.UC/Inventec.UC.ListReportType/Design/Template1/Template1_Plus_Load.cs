using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.Design.Template1
{
    internal partial class Template1
    {
        private void Template1_Load(object sender, EventArgs e)
        {
            try
            {
                Language();
                SetDefaultControl();
                btnRefresh_Click(null, null);
                //btnSearch_Click(null, null);
                //txtKeyword.Focus();
                //txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Language()
        {
            try
            {
                gridColumn1.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_TYPE_GRID_COLUMN_1", Resources.ResourceLanguageManager.LanguageListReportType, Base.LanguageManager.GetCulture());
                gridColumn2.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_TYPE_GRID_COLUMN_2", Resources.ResourceLanguageManager.LanguageListReportType, Base.LanguageManager.GetCulture());
                gridColumn3.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_TYPE_GRID_COLUMN_3", Resources.ResourceLanguageManager.LanguageListReportType, Base.LanguageManager.GetCulture());
                gridColumn4.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_TYPE_GRID_COLUMN_4", Resources.ResourceLanguageManager.LanguageListReportType, Base.LanguageManager.GetCulture());
                gridColumn5.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_TYPE_GRID_COLUMN_5", Resources.ResourceLanguageManager.LanguageListReportType, Base.LanguageManager.GetCulture());
                gridColumn6.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_TYPE_GRID_COLUMN_6", Resources.ResourceLanguageManager.LanguageListReportType, Base.LanguageManager.GetCulture());
                gridColumn7.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_TYPE_GRID_COLUMN_7", Resources.ResourceLanguageManager.LanguageListReportType, Base.LanguageManager.GetCulture());
                gridColumn8.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_TYPE_GRID_COLUMN_8", Resources.ResourceLanguageManager.LanguageListReportType, Base.LanguageManager.GetCulture());
                // btnSearch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_TYPE_BTN_SEARCH", Resources.ResourceLanguageManager.LanguageListReportType, Base.LanguageManager.GetCulture());
                //btnRefresh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_TYPE_BTN_REFRESH", Resources.ResourceLanguageManager.LanguageListReportType, Base.LanguageManager.GetCulture());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
