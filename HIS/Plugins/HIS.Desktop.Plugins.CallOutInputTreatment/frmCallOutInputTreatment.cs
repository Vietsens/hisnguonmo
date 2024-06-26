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
using AutoMapper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallOutInputTreatment
{
    public partial class frmCallOutInputTreatment : HIS.Desktop.Utility.FormBase
    {
        #region Variable
        RefeshReference RefeshReference;
        int positionHandleControl = -1;
        internal Inventec.Desktop.Common.Modules.Module module;
        PatientTypeDepartmentADO TreatmentLogSDO;
        int ActionType = 0;
        private WaitDialogForm waitLoad = null;
        #endregion

        #region Construction
        public frmCallOutInputTreatment(Inventec.Desktop.Common.Modules.Module module, RefeshReference RefeshReference, PatientTypeDepartmentADO TreatmentLogSDO)
		:base(module)
        {
            try
            {
                InitializeComponent();
                this.module = module;
                this.RefeshReference = RefeshReference;
                this.TreatmentLogSDO = TreatmentLogSDO;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        public frmCallOutInputTreatment(Inventec.Desktop.Common.Modules.Module module, RefeshReference RefeshReference)
		:base(module)
        {
            try
            {
                this.module = module;
                this.RefeshReference = RefeshReference;
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmCallOutInputTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataToComboDepartment(this.cboDepartment);
                LoadDataToComboDepartment(this.cboNextDepartment);
                txtDepartmentCode.Enabled = cboDepartment.Enabled = false;
                LoadDataForm(TreatmentLogSDO);
                SetCaptionByLanguageKey();
                btnSave.Enabled = (TreatmentLogSDO != null && TreatmentLogSDO.IS_ACTIVE == 0) ? false : true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void LoadDataForm(PatientTypeDepartmentADO TreatmentLogSDO)
        {
            if (this.TreatmentLogSDO != null && this.TreatmentLogSDO.departmentTran != null)
            {
                ActionType = GlobalVariables.ActionEdit;
                dtLogTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.TreatmentLogSDO.LOG_TIME);
                if (this.TreatmentLogSDO.departmentTran.NEXT_DEPARTMENT_ID > 0)
                {
                    txtNextDepartmentCode.Enabled = cboNextDepartment.Enabled = !(this.TreatmentLogSDO.departmentTran.IS_RECEIVE == IMSys.DbConfig.HIS_RS.HIS_DEPARTMENT_TRAN.IS_RECEIVE__TRUE);

                    txtNextDepartmentCode.Text = this.TreatmentLogSDO.departmentTran.NEXT_DEPARTMENT_CODE;
                    cboNextDepartment.EditValue = this.TreatmentLogSDO.departmentTran.NEXT_DEPARTMENT_ID;
                    lblReceive.Text = (this.TreatmentLogSDO.departmentTran.IS_RECEIVE == IMSys.DbConfig.HIS_RS.HIS_DEPARTMENT_TRAN.IS_RECEIVE__TRUE ? "Đã tiếp nhận" : "Chưa tiếp nhận");
                    dtLogTime.Focus();
                    dtLogTime.Update();
                }
                else
                {
                    txtNextDepartmentCode.Enabled = cboNextDepartment.Enabled = false;
                    dtLogTime.Focus();
                }
                txtDepartmentCode.Text = this.TreatmentLogSDO.departmentTran.DEPARTMENT_CODE;
                cboDepartment.EditValue = this.TreatmentLogSDO.departmentTran.DEPARTMENT_ID;
            }
            else
            {
                txtDepartmentCode.Text = TreatmentLogSDO.departmentTran.DEPARTMENT_CODE;
                cboDepartment.EditValue = TreatmentLogSDO.departmentTran.DEPARTMENT_ID;
                ActionType = GlobalVariables.ActionAdd;
                dtLogTime.EditValue = DateTime.Now;
                dtLogTime.Update();
                txtDepartmentCode.SelectAll();
                txtDepartmentCode.Focus();
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallOutInputTreatment.Resources.Lang", typeof(HIS.Desktop.Plugins.CallOutInputTreatment.frmCallOutInputTreatment).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoLeave.Properties.Caption = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.chkAutoLeave.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNextDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.cboNextDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barSave.Caption = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.barSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCallOutInputTreatment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event
        private void LoadDepartmentCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDepartment.EditValue = null;
                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();
                }
                else
                {
                    var data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE.Contains(searchCode)).ToList();
                    List<HIS_DEPARTMENT> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.DEPARTMENT_CODE == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboDepartment.EditValue = result[0].ID;
                        txtDepartmentCode.Text = result[0].DEPARTMENT_CODE;
                        txtNextDepartmentCode.Focus();
                        txtNextDepartmentCode.SelectAll();
                    }
                    else
                    {
                        cboDepartment.EditValue = null;
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadDepartmentCombo(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_DEPARTMENT data = BackendDataWorker.Get<HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                        }
                    }
                    txtNextDepartmentCode.Focus();
                    txtNextDepartmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_DEPARTMENT data = BackendDataWorker.Get<HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                        }
                        txtNextDepartmentCode.Focus();
                        txtNextDepartmentCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadNextDepartmentCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboNextDepartment.EditValue = null;
                    cboNextDepartment.Focus();
                    cboNextDepartment.ShowPopup();
                }
                else
                {
                    var data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE.Contains(searchCode)).ToList();
                    List<HIS_DEPARTMENT> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.DEPARTMENT_CODE == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboNextDepartment.EditValue = result[0].ID;
                        txtNextDepartmentCode.Text = result[0].DEPARTMENT_CODE;
                        dtLogTime.Focus();
                    }
                    else
                    {
                        cboNextDepartment.EditValue = null;
                        cboNextDepartment.Focus();
                        cboNextDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNextDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadNextDepartmentCombo(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNextDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboNextDepartment.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_DEPARTMENT data = BackendDataWorker.Get<HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboNextDepartment.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtNextDepartmentCode.Text = data.DEPARTMENT_CODE;
                        }
                        dtLogTime.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNextDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboNextDepartment.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_DEPARTMENT data = BackendDataWorker.Get<HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboNextDepartment.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtNextDepartmentCode.Text = data.DEPARTMENT_CODE;
                        }
                    }
                    dtLogTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtLogTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAutoLeave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void chkAutoLeave_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        void UpdateDepartmentTranFromDataForm(ref HisDepartmentTranSDO ServiceReqData)
        {
            try
            {
                if (ServiceReqData.HisDepartmentTran == null)
                    ServiceReqData.HisDepartmentTran = new MOS.EFMODEL.DataModels.HIS_DEPARTMENT_TRAN();

                if (cboNextDepartment.EditValue != null)
                    ServiceReqData.HisDepartmentTran.NEXT_DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboNextDepartment.EditValue ?? "").ToString());

                if (cboDepartment.EditValue != null)
                    ServiceReqData.HisDepartmentTran.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString());

                ServiceReqData.AutoLeaveRoom = chkAutoLeave.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled == true)
                    btnSave_Click(null, null);
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
                bool success = false;
                long treatmentId = 0;
                CommonParam param = new CommonParam();
                try
                {
                    this.positionHandleControl = -1;
                    if (!dxValidationProvider1.Validate())
                        return;

                    treatmentId = TreatmentLogSDO.TREATMENT_ID;
                    if (treatmentId <= 0)
                    {
                        Inventec.Common.Logging.LogSystem.Error("GetSummaryByModuleType is null. TreatmentId=" + treatmentId);
                        return;
                    }

                    WaitingManager.Show();
                    dtLogTime.Update();

                    if (ActionType == GlobalVariables.ActionAdd)
                    {
                        this.TreatmentLogSDO = new PatientTypeDepartmentADO();
                        this.TreatmentLogSDO.LOG_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                        this.TreatmentLogSDO.departmentTran = new MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN();
                        this.TreatmentLogSDO.departmentTran.IN_OUT = IMSys.DbConfig.HIS_RS.HIS_DEPARTMENT_TRAN.IN_OUT__OUT;
                        this.TreatmentLogSDO.departmentTran.TREATMENT_ID = treatmentId;
                        this.TreatmentLogSDO.departmentTran.LOG_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                        this.TreatmentLogSDO.TREATMENT_ID = treatmentId;
                    }
                    HisDepartmentTranSDO HisDepartmentTranSDO = new HisDepartmentTranSDO();

                    TreatmentLogSDO.type = 2;
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN, MOS.EFMODEL.DataModels.HIS_DEPARTMENT_TRAN>();
                    HisDepartmentTranSDO.HisDepartmentTran = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN, MOS.EFMODEL.DataModels.HIS_DEPARTMENT_TRAN>(this.TreatmentLogSDO.departmentTran);

                    UpdateDepartmentTranFromDataForm(ref HisDepartmentTranSDO);
                    if (ActionType == GlobalVariables.ActionAdd)
                    {
                        var rs = new BackendAdapter(param).Post<HisDepartmentTranSDO>(HisRequestUriStore.HIS_TREATMENT_LOG_CREATE_DEPARTMENT_TRAN, ApiConsumers.MosConsumer, HisDepartmentTranSDO, param);
                        if (rs != null)
                        {
                            success = true;
                        }
                    }
                    else if (ActionType == GlobalVariables.ActionEdit)
                    {
                        HisDepartmentTranSDO.HisDepartmentTran.LOG_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                        var rs = new BackendAdapter(param).Post<HisDepartmentTranSDO>(HisRequestUriStore.HIS_TREATMENT_LOG_UPDATE_DEPARTMENT_TRAN, ApiConsumers.MosConsumer, HisDepartmentTranSDO, param);
                        if (rs != null)
                        {
                            success = true;
                        }
                    }

                    WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Fatal(ex);
                    MessageUtil.SetParam(param, LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
                }

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (success)
                {
                    if (RefeshReference != null)
                        RefeshReference();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }
        #endregion
        public static void LoadDataToComboDepartment(DevExpress.XtraEditors.LookUpEdit cboDepartment)
        {
            try
            {
                cboDepartment.Properties.DataSource = BackendDataWorker.Get<HIS_DEPARTMENT>();
                cboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboDepartment.Properties.ValueMember = "ID";
                cboDepartment.Properties.ForceInitialize();
                cboDepartment.Properties.Columns.Clear();
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_CODE", "", 100));
                cboDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_NAME", "", 200));
                cboDepartment.Properties.ShowHeader = false;
                cboDepartment.Properties.ImmediatePopup = true;
                cboDepartment.Properties.DropDownRows = 10;
                cboDepartment.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
