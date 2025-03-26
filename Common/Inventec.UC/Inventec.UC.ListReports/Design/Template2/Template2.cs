/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ListReports.Base;
using DevExpress.XtraNavBar;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using Inventec.UC.ListReports.Config;

namespace Inventec.UC.ListReports.Design.Template2
{
    internal partial class Template2 : UserControl, IFormCallBack
    {
        public PagingGrid pagingGrid;
        private WaitDialogForm waitLoad;
        int rowCount = 0;

        private ProcessHasException _HasException;

        private List<SAR.EFMODEL.DataModels.V_SAR_REPORT> ListReport = new List<SAR.EFMODEL.DataModels.V_SAR_REPORT>();

        public Template2(Data.InitData data)
        {
            InitializeComponent();
            try
            {
                ApiConsumerStore.SarConsumer = data.sarconsumer;
                ApiConsumerStore.SdaConsumer = data.sdaconsumer;
                ApiConsumerStore.AcsConsumer = data.acsconsumer;
                TokenClientStore.ClientTokenManager = data.clientToken;
                GlobalStore.NumberPage = data.numPage <= 0 ? 100 : data.numPage;
                GlobalStore.pathFileIcon = data.nameIcon;
                GlobalStore.isAdmin = data.isAdmin;
                Base.BusinessBase.TokenCheck();
                pagingGrid = new PagingGrid();
                //Config.Loader.RefreshConfig();

                Inventec.UC.ListReports.Base.ResouceManager.ResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Template2_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
                Language();
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
                nbGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_NB_GROUP_CREATE_TIME", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                nbGroupCreateUser.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_NB_GROUP_CREATE_USER", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                nbGroupStatus.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_NB_GROUP_STATUS", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                lblTimeFrom.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_LBL_TIME_FROM", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                lblTimeTo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_LBL_TIME_TO", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                checkAll.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_CHECK_ALL", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                checkICreate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_CHECK_CREATE", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                checkIReceive.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_CHECK_RECEIVE", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                checkNoProcess.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_CHECK_NO_PROCESS", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                checkProcessing.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_CHECK_PROCESSING", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                checkFinish.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_CHECK_FINISH", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                checkCancel.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_CHECK_CANCEL", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                checkError.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_CHECK_ERROR", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnSearch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_SEARCH", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                //btnRefresh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_REFRESH", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn1.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN1", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn8.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN8", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn9.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN9", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn10.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN10", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn11.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN11", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn12.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN12", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn13.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN13", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn14.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN14", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn19.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN15", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn16.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN16", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn17.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN17", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn18.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN18", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn15.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN19", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn20.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN20", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn23.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN23", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn24.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN24", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn25.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN25", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn26.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN26", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn27.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN27", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn28.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN28", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                gridColumn29.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_GRD_COLUMN29", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnEditReport.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_EDIT_REPORT", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnDeleteReport.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_DELETE_REPORT", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnPrivateReport.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_PUBLIC_REPORT", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnSendReport.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_SEND_REPORT", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnDownloadReport.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_DOWNLOAD_REPORT", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnPrintReport.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_PRINT_REPORT", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnEditReportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_EDIT_REPORT_DISABLE", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnDeleteReportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_DELETE_REPORT_DISABLE", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnPrivateReportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_PUBLIC_REPORT_DISABLE", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnSendReportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_SEND_REPORT_DISABLE", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnDownloadReportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_DOWNLOAD_REPORT_DISABLE", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());
                btnPrintReportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_REPORT_BTN_PRINT_REPORT_DISABLE", Resources.ResourceLanguageManager.LanguageListReport, Base.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlListReports_Click(object sender, EventArgs e)
        {

        }
      

       

    }
}
