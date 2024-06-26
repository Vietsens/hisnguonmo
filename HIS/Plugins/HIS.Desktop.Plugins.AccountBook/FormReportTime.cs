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
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisAccountBookList
{
    public partial class FormReportTime : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Base.Mrs00249Filter filter = new Base.Mrs00249Filter();
        MRS.SDO.CreateReportSDO reportSdo = new MRS.SDO.CreateReportSDO();
        CommonParam param = new CommonParam();
        bool success = false;
        #endregion

        #region Construct
        public FormReportTime()
        {
            InitializeComponent();
        }

        public FormReportTime(MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK reportData)
            : this()
        {
            try
            {
                var reportType = Base.GlobalStore.ReportType;
                if (reportType != null)
                {
                    reportSdo.ReportTypeCode = reportType.REPORT_TYPE_CODE;
                    reportSdo.ReportTemplateCode = Base.GlobalStore.ReportTemplate.REPORT_TEMPLATE_CODE;
                }
                reportSdo.BranchId = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                reportSdo.ReportName = "Báo cáo chi tiết " + reportData.ACCOUNT_BOOK_NAME;
                filter.ACCOUNT_BOOK_ID = reportData.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void FormReportTime_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultTime();
                SetIcon();
                LoadKeysFromlanguage();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //filter
                this.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__FROM_REPORT_TIME",
                    Base.ResourceLanguageManager.LanguageFormReportTime,
                    cultureLang);
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormReportTime.layoutControl1.Text", Base.ResourceLanguageManager.LanguageFormReportTime, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormReportTime.btnSave.Text", Base.ResourceLanguageManager.LanguageFormReportTime, LanguageManager.GetCulture());
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("FormReportTime.lciCreateTimeFrom.Text", Base.ResourceLanguageManager.LanguageFormReportTime, LanguageManager.GetCulture());
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("FormReportTime.lciCreateTimeTo.Text", Base.ResourceLanguageManager.LanguageFormReportTime, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormReportTime.bar1.Text", Base.ResourceLanguageManager.LanguageFormReportTime, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("FormReportTime.barButtonItemSave.Caption", Base.ResourceLanguageManager.LanguageFormReportTime, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                reportSdo.Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                reportSdo.Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                SetFilter(ref filter);
                reportSdo.Filter = filter;
                success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(ApiConsumer.MrsRequestUriStore.MRS_REPORT_CREATE, ApiConsumer.ApiConsumers.MrsConsumer, reportSdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion

                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
        }

        private void SetFilter(ref Base.Mrs00249Filter filter)
        {
            try
            {
                filter.BRANCH_ID = UCHisAccountBookList.branchId;
                filter.CREATE_TIME_FROM = (long)Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtCreateTimeFrom.DateTime);
                filter.CREATE_TIME_TO = (long)Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtCreateTimeTo.DateTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultTime()
        {
            try
            {
                dtCreateTimeFrom.EditValue = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 1);
                dtCreateTimeTo.EditValue = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 23, 59, 59);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (this.dtCreateTimeFrom.DateTime != System.DateTime.MinValue)
                    filter.CREATE_TIME_FROM = (long)Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtCreateTimeFrom.DateTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (this.dtCreateTimeTo.DateTime != System.DateTime.MinValue)
                    filter.CREATE_TIME_TO = (long)Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtCreateTimeTo.DateTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled) btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
