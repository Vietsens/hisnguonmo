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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.URI;
using EMR.SDO;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;

namespace EMR.Desktop.Plugins.EmrApproveViewer
{
    public partial class frmEmrApproveViewer : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long viewerId;
        V_EMR_VIEWER currentViewer;
        HIS.Desktop.Common.DelegateRefreshData dlg;

        public frmEmrApproveViewer(Inventec.Desktop.Common.Modules.Module module, long viewerId, HIS.Desktop.Common.DelegateRefreshData dlg)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.viewerId = viewerId;
                this.dlg = dlg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmEmrApproveViewer_Load(object sender, EventArgs e)
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.Text = this.currentModule.text;
                FillDataToControl(this.viewerId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControl(long id)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                EmrViewerViewFilter filter = new EmrViewerViewFilter();
                filter.ID = id;

                var rsApi = new BackendAdapter(param).Get<List<V_EMR_VIEWER>>(EMR.URI.EmrViewer.GET_VIEW, ApiConsumers.EmrConsumer, filter, param);
                if (rsApi != null)
                {
                    this.currentViewer = rsApi.FirstOrDefault();
                    lblRequestName.Text = this.currentViewer.REQUEST_LOGINNAME + " - " + this.currentViewer.REQUEST_USERNAME;
                    lblRequestFinishTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentViewer.REQUEST_FINISH_TIME);
                    lblRequestDepartment.Text = this.currentViewer.DEPARTMENT_NAME;
                    lblPatientName.Text = this.currentViewer.LAST_NAME + this.currentViewer.FIRST_NAME;
                    lblPatientGenderName.Text = this.currentViewer.GENDER_NAME;
                    lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentViewer.DOB);
                    lblTreatmentCode.Text = this.currentViewer.TREATMENT_CODE;
                    dtFinishTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentViewer.REQUEST_FINISH_TIME) ?? new DateTime();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.currentViewer != null)
                {
                    if (dtFinishTime.EditValue == null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa nhập thời gian xem");
                        return;
                    }

                    bool success = false;
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();

                    EMR_VIEWER data = new EMR_VIEWER();
                    data.ID = this.currentViewer.ID;
                    data.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinishTime.DateTime);
                    data.APPROVAL_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    data.APPROVAL_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

                    var rsApi = new BackendAdapter(param).Post<EMR_VIEWER>("api/EmrViewer/Approve", ApiConsumers.EmrConsumer, data, param);
                    if (rsApi != null)
                    {
                        success = true;
                        if (this.dlg != null)
                            this.dlg();
                        this.Close();
                    }

                    WaitingManager.Hide();

                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.currentViewer != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();

                    EMR_VIEWER data = new EMR_VIEWER();
                    data.ID = this.currentViewer.ID;

                    var rsApi = new BackendAdapter(param).Post<EMR_VIEWER>("api/EmrViewer/Reject", ApiConsumers.EmrConsumer, data, param);
                    if (rsApi != null)
                    {
                        success = true;
                        if (this.dlg != null)
                            this.dlg();
                        this.Close();
                    }

                    WaitingManager.Hide();

                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
