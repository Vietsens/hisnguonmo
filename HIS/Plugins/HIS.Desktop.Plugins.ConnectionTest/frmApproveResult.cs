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
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using LIS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.Message;
using LIS.SDO;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class frmApproveResult : Form
    {
        LisSampleADO ado;
        Action<LIS_SAMPLE> action;
        Action<bool> IsShowMessage;
        bool IsClickSave;
        public frmApproveResult(LisSampleADO ado, Action<LIS_SAMPLE> action, Action<bool> IsShowMessage = default(Action<bool>))
        {
            InitializeComponent();
            try
            {
                this.ado = ado;
                this.action = action;
                this.IsShowMessage = IsShowMessage;
                SetIconFrm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmApproveResult_Load(object sender, EventArgs e)
        {
            try
            {
                dteApprove.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                long? ResultApproveTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteApprove.DateTime);
                if (ado != null && ResultApproveTime != null)
                {
                    if (ado.APPROVAL_TIME != null && ado.APPROVAL_TIME > ResultApproveTime)
                    {
                        XtraMessageBox.Show(String.Format("Thời gian duyệt kết quả phải lớn hơn thời gian nhận mẫu {0}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ado.APPROVAL_TIME ?? 0)), "Thông báo");
                        return;
                    }
                    else if (ado.SAMPLE_TIME != null && ado.SAMPLE_TIME > ResultApproveTime)
                    {
                        XtraMessageBox.Show(String.Format("Thời gian duyệt kết quả phải lớn hơn thời gian lấy mẫu {0}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ado.SAMPLE_TIME ?? 0)), "Thông báo");
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    LisSampleApproveResultSDO sdo = new LisSampleApproveResultSDO();
                    sdo.SampleId = ado.ID;
                    sdo.ApproveTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteApprove.DateTime);
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/ApproveResult", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                    this.action(curentSTT);
                    IsClickSave = true;
                    WaitingManager.Hide();
                    #region Show message

                    MessageManager.Show(this.ParentForm, param, curentSTT != null);
                    #endregion
                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                    if (curentSTT != null)
                        this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void frmApproveResult_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (IsShowMessage != null)
                    IsShowMessage(IsClickSave);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
