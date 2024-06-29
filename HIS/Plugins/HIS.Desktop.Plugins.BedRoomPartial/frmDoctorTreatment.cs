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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    public partial class frmDoctorTreatment : Form
    {
        string DoctorLogin { get; set; }
        DelegateRefreshData RefreshData { get; set; }
        L_HIS_TREATMENT_BED_ROOM treatmentBedRoomRow {get;set;}
        List<V_HIS_EMPLOYEE> lstEmployee { get; set; }
        public frmDoctorTreatment(L_HIS_TREATMENT_BED_ROOM treatmentBedRoomRow, DelegateRefreshData RefreshData)
        {
            InitializeComponent();

            try
            {
                SetIcon();
                this.treatmentBedRoomRow = treatmentBedRoomRow;
                this.DoctorLogin = treatmentBedRoomRow.DOCTOR_LOGINNAME;
                this.RefreshData = RefreshData;
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
        private void frmDoctorTreatment_Load(object sender, EventArgs e)
        {
            InitComboEmployee();
        }
        private void InitComboEmployee()
        {
            try
            {
                lstEmployee = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DOCTOR == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ và tên", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(cboEmployee, lstEmployee, controlEditorADO);
                cboEmployee.Properties.ImmediatePopup = true;
                cboEmployee.Properties.PopupFormMinSize = new Size(400, cboEmployee.Properties.PopupFormMinSize.Height);
                if (!string.IsNullOrEmpty(DoctorLogin) && lstEmployee.FirstOrDefault(o => o.LOGINNAME == DoctorLogin) != null)
                {
                    cboEmployee.EditValue = DoctorLogin;
                }
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
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                MOS.SDO.UpdateDoctorInfoSDO sdo = new MOS.SDO.UpdateDoctorInfoSDO();
                sdo.TreatmentId = treatmentBedRoomRow.TREATMENT_ID;
                if (cboEmployee.EditValue != null)
                {
                    sdo.DoctorLoginname = cboEmployee.EditValue.ToString();
                    sdo.DoctorUsername = lstEmployee.FirstOrDefault(o => o.LOGINNAME == sdo.DoctorLoginname).TDL_USERNAME;
                }
                var success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateDoctorInfo", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                WaitingManager.Hide();
                if (success != null && success.ID > 0)
                {
                    this.RefreshData();
                    this.Close();
                }
                else
                {
                    MessageManager.Show(this.ParentForm, param, success != null && success.ID > 0);
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void cboEmployee_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (cboEmployee.EditValue != null && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                cboEmployee.EditValue = null;
        }
    }
}
