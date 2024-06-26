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
using His.UC.CreateReport;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReportAll
{
    public partial class frmMainReport : HIS.Desktop.Utility.FormBase
    {
        public delegate void Refresh_Click();

        SAR.EFMODEL.DataModels.SAR_REPORT_TYPE reportType = new SAR.EFMODEL.DataModels.SAR_REPORT_TYPE();
        His.UC.CreateReport.MainCreateReport MainCreateReport = new His.UC.CreateReport.MainCreateReport();
        His.UC.CreateReport.MainCreateReport1 MainCreateReport1 = new His.UC.CreateReport.MainCreateReport1();
        public Refresh_Click delegateSearch;
        string vlCustomFormType = "";

        int height = 165;
        SAR.EFMODEL.DataModels.V_SAR_REPORT reportPrevious;
        public frmMainReport()
        {
            InitializeComponent();
        }

        public frmMainReport(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE data, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            try
            {
                this.reportType = data;
                this.SetDelegateForUCFormType();
                this.MainPrint_Load();
                InitializeComponent();
                vlCustomFormType = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ReportCreate.CustomFormType");
                if (vlCustomFormType == "1")
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    //neu cao hon man hinh se thu nho lai vua nhin
                    if (height >= SystemInformation.WorkingArea.Size.Height)
                    {
                        height = SystemInformation.WorkingArea.Size.Height - 100;
                    }
                    this.Height = height;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDelegateForUCFormType()
        {
            HIS.UC.FormType.FormTypeDelegate.ProcessFormType = ProcessFormTypeDelegate;
            HIS.UC.FormType.FormTypeDelegate.GetCashierUser = AcsUserCashierGet.Get;
            HIS.UC.FormType.FormTypeDelegate.GetSaleUser = AcsUserSaleGet.Get;
            HIS.UC.FormType.FormTypeDelegate.PagingGet = PagingGet.Get;
        }

        private void MainPrint_Load()
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                HIS.UC.FormType.GenerateRDO generateRDO = new HIS.UC.FormType.GenerateRDO();
                generateRDO.Report = this.reportPrevious;
                generateRDO.DetailData = this.reportType;
                var formCreate = vlCustomFormType == "1" ? MainCreateReport1.Generate(param, generateRDO) : MainCreateReport.Generate(param, generateRDO);
                formCreate.Dock = DockStyle.Fill;
                //lay chieu cao cua uc
                if (formCreate.Controls != null && formCreate.Controls.Count > 0)
                {
                    foreach (var item in formCreate.Controls)
                    {
                        if (item.GetType() == typeof(DevExpress.XtraLayout.LayoutControl))
                        {
                            var controls = (DevExpress.XtraLayout.LayoutControl)item;
                            if (controls.Controls != null && controls.Controls.Count > 0)
                            {
                                foreach (var control in controls.Controls)
                                {
                                    if (control.GetType() == typeof(DevExpress.XtraEditors.XtraScrollableControl))
                                    {
                                        var xtraControl = (DevExpress.XtraEditors.XtraScrollableControl)control;
                                        if (xtraControl.Controls != null && xtraControl.Controls.Count > 0)
                                        {
                                            foreach (Control Editor in xtraControl.Controls)
                                            {
                                                height += Editor.Height;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //CreateReportDelegate.ProcessCreateReport = ProcessCreateReportDelegate;
                CreateReportDelegate.ProcessCreateReportViewAway = ProcessCreateReportDelegateViewAway;
                CreateReportDelegate.DelegateStatusReport = DelegateStatusReport;

                this.Controls.Add(formCreate);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private SAR_REPORT DelegateStatusReport(long Id)
        {
            try
            {
                CommonParam param = new CommonParam();
                SarReportFilter filter = new SarReportFilter();
                filter.ID = Id;
                SAR_REPORT result = new BackendAdapter(param).Get<List<SAR_REPORT>>("api/SarReport/Get", ApiConsumer.ApiConsumers.SarConsumer, filter, param).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private bool ProcessCreateReportDelegate(object sarReport)
        {
            try
            {
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param).Post<bool>(MrsRequestUriStore.MRS_REPORT_CREATE, ApiConsumer.ApiConsumers.MrsConsumer, sarReport, param);
                if (result)
                {
                    if (delegateSearch != null)
                    {
                        delegateSearch();
                    }
                    this.Close();
                }
                #region Show message
                MessageManager.Show(this, param, result);
                #endregion
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private SAR_REPORT ProcessCreateReportDelegateViewAway(object sarReport)
        {
            try
            {
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param).Post<SAR_REPORT>(MrsRequestUriStore.MRS_REPORT_CREATE_REQ, ApiConsumers.MrsConsumer, sarReport, param);
                #region Show message
                MessageManager.Show(this, param, result != null && result.ID != 0);
                #endregion
                if (result != null)
                {
                    if (delegateSearch != null)
                    {
                        delegateSearch();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }


        }
    }
}
