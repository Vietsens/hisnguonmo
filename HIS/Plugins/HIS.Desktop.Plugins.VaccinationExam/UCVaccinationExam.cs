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
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.VaccinationExam.ADO;
using Inventec.Core;
using DevExpress.XtraEditors;
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.VaccinationExam.Base;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.VaccinationExam.Processors;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;

namespace HIS.Desktop.Plugins.VaccinationExam
{
    public partial class UCVaccinationExam : UserControlBase
    {
        long roomId;
        long roomTypeId;
        int rowCount = 0;
        int dataTotal = 0;
        long idDhst;
        int numPageSize;
        V_HIS_VACCINATION_EXAM vaccinationExam;
        V_HIS_VACCINATION_EXAM vaccinationExam_ForProcess;
        List<HIS_VACC_EXAM_RESULT> vaccExamResults;
        public int positionHandle = -1;
        List<V_HIS_EXP_MEST_MEDICINE_5> expMestMedicine5s { get; set; }
        internal V_HIS_SERVICE_REQ HisServiceReqView { get; set; }
        List<V_HIS_MEST_ROOM> mestRooms { get; set; }
        EnumUtil.ACTION? actionAssign { get; set; }
        VaccinationResultSDO VaccinationResult;
        List<V_HIS_VACC_APPOINTMENT> VaccAppointmentResult;

        public List<HIS_VACCINE_TYPE> VaccineTypes;
        Inventec.Desktop.Common.Modules.Module CurrentModule;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
		private int key;
        private List<V_HIS_VACCINATION> lstVaccination { get; set; }
        private V_HIS_VACCINATION currentVaccination { get; set; }
        PopupMenuProcessor popupMenuProcessor = null;

        public UCVaccinationExam(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                this.CurrentModule = moduleData;
                if (moduleData != null)
                {
                    this.roomId = moduleData.RoomId;
                    this.roomTypeId = moduleData.RoomTypeId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            InitializeComponent();
        }

        private void gridViewVaccinationExam_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_VACCINATION_EXAM dataRow = (V_HIS_VACCINATION_EXAM)((System.Collections.IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "TRANGTHAI_IMG")
                    {
                        //Chua xu ly: mau trang
                        //dang xu ly: mau vang
                        //Da ket thuc: mau den

                        var statusId = dataRow.VACCINATION_EXAM_STT_ID;
                        if (statusId == 1)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                        else if (statusId == 2)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        else if (statusId == 3)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        else
                        {
                            e.Value = imageListIcon.Images[6];
                        }
                    }
                    else if (e.Column.FieldName == "REQUEST_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.REQUEST_TIME);
                    }
                    else if (e.Column.FieldName == "EXECUTE_TIME_DISPLAY")
                    {
                        if (dataRow.EXECUTE_TIME.HasValue)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.EXECUTE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        if (dataRow.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            e.Value = dataRow.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        else
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.TDL_PATIENT_DOB.ToString());
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCVaccinationExam_Load(object sender, EventArgs e)
        {
            try
            {
                InitControlState();
                LoadDataToCombo();
                LoadDataDefault();
                DHSTLoadDataDefault();
                LoadVaccExamResullt(null);
                FillDataToGridVaccinationExam();
                ValidateVaccinInfo();
                ValidateAppointment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DHSTLoadDataDefault()
        {
            try
            {
                idDhst = 0;
                dtExecuteTime.DateTime = DateTime.Now;
                spinBloodPressureMin.EditValue = null;
                spinBloodPressureMax.EditValue = null;
                spinBreathRate.EditValue = null;
                spinHeight.EditValue = null;
                spinChest.EditValue = null;
                spinBelly.EditValue = null;
                spinPulse.EditValue = null;
                spinTemperature.EditValue = null;
                spinWeight.EditValue = null;
                spinSPO2.EditValue = null;
                txtNote.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridVaccinationExam();
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
                if(radioGroupConclude.SelectedIndex == 1 || radioGroupConclude.SelectedIndex ==2)
				{
                    layoutControlItem30.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationSingleControl(spinTemperature, dxValidationProvider2);
                }
				else
				{
                    layoutControlItem30.AppearanceItemCaption.ForeColor = Color.Black;
                    dxValidationProvider2.SetValidationRule(spinTemperature,null);
                }
                this.positionHandle = -1;
                if (!dxValidationProvider2.Validate())
                    return;

                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                HisVaccinationExamTreatSDO hisVaccinationExamTreatSDO = new HisVaccinationExamTreatSDO();
                this.CreateExecuteSDO(ref hisVaccinationExamTreatSDO);

                HIS_DHST dhst = ProcessDhst(ref hisVaccinationExamTreatSDO);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dữ liệu: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => dhst), dhst));

                hisVaccinationExamTreatSDO.Dhst = dhst;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisVaccinationExamTreatSDO), hisVaccinationExamTreatSDO));
                bool result = new BackendAdapter(param)
                    .Post<bool>("api/HisVaccinationExam/Treat", ApiConsumers.MosConsumer, hisVaccinationExamTreatSDO, param);
                WaitingManager.Hide();
                if (result)
                {
                    success = true;
                    //Lay gia tri moi nhat
                    gridControlVaccinationExam.RefreshDataSource();

                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_VACCINATION_EXAM>(vaccinationExam, GetVaccinationExamById(vaccinationExam.ID));
                    if (vaccinationExam != null && vaccinationExam.CONCLUDE == 1)
                    {
                        btnAssignee.Enabled = true;
                        btnAppointment.Enabled = true;
                    }
                    else
                    {
                        btnAssignee.Enabled = false;
                        btnAppointment.Enabled = false;
                    }
                }

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_DHST ProcessDhst(ref HisVaccinationExamTreatSDO hisVaccinationExamTreatSDO)
        {
            HIS_DHST outPut = new HIS_DHST();
            try
            {
                hisVaccinationExamTreatSDO.Dhst = new HIS_DHST();

                if (idDhst > 0)
                {
                    outPut.ID = idDhst;
                }
                if (dtExecuteTime.EditValue != null)
                    outPut.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExecuteTime.DateTime);
                Inventec.Common.Logging.LogSystem.Debug("Thoi gian" + dtExecuteTime.DateTime);
                if (spinBloodPressureMax.EditValue != null)
                    outPut.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMax.Value.ToString());
                if (spinBloodPressureMin.EditValue != null)
                    outPut.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMin.Value.ToString());
                if (spinBreathRate.EditValue != null)
                    outPut.BREATH_RATE = Inventec.Common.Number.Get.RoundCurrency(spinBreathRate.Value, 2);
                if (spinHeight.EditValue != null)
                    outPut.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinHeight.Value, 2);
                if (spinChest.EditValue != null)
                    outPut.CHEST = Inventec.Common.Number.Get.RoundCurrency(spinChest.Value, 2);
                if (spinBelly.EditValue != null)
                    outPut.BELLY = Inventec.Common.Number.Get.RoundCurrency(spinBelly.Value, 2);
                if (spinPulse.EditValue != null)
                    outPut.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spinPulse.Value.ToString());
                if (spinTemperature.EditValue != null)
                    outPut.TEMPERATURE = Inventec.Common.Number.Get.RoundCurrency(spinTemperature.Value, 2);
                if (spinWeight.EditValue != null)
                    outPut.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinWeight.Value, 2);
                if (spinSPO2.EditValue != null)
                    outPut.SPO2 = Inventec.Common.Number.Get.RoundCurrency(spinSPO2.Value, 2) / 100;
                outPut.NOTE = txtNote.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dữ liệu OUTPUT: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => outPut), outPut));
            return outPut;
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVaccinationExam_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    ProcessReloadDataForNewExecute();
                    this.vaccinationExam = (V_HIS_VACCINATION_EXAM)gridViewVaccinationExam.GetFocusedRow();
                    LoadDataFromVaccinationExam(vaccinationExam);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessReloadDataForNewExecute()
        {
            try
            {
                DHSTLoadDataDefault();
                tabbedControlGroup1.SelectedTabPage = lcgAssign;
                this.VaccinationResult = null;
                this.VaccAppointmentResult = null;
                btnAppointment.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewVaccinationMety_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "ACTION")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewlVaccinationMety.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repositoryItemButtonEditAdd;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = repositoryItemButtonEditMinus;
                    }
                }
                else if (e.Column.FieldName == gc_VaccineType.FieldName)
                {
                    e.RepositoryItem = repositoryItemCboVaccineType;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var vaccinationMetyADOs = gridControlVaccinationMety.DataSource as List<ExpMestMedicineADO>;
                ExpMestMedicineADO vaccinationMetyADO = new ExpMestMedicineADO();
                vaccinationMetyADO.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                vaccinationMetyADO.VACCINE_TURN = 1;
                vaccinationMetyADO.AMOUNT = 1;
                if (vaccinationExam != null)
                {
                    vaccinationMetyADO.PATIENT_TYPE_ID = vaccinationExam.PATIENT_TYPE_ID;
                }
                vaccinationMetyADOs.Add(vaccinationMetyADO);
                gridControlVaccinationMety.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditMinus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var vaccinationMetyADOs = gridControlVaccinationMety.DataSource as List<ExpMestMedicineADO>;
                var vaccinationMetyADO = gridViewlVaccinationMety.GetFocusedRow() as ExpMestMedicineADO;
                if (vaccinationMetyADO != null)
                {
                    vaccinationMetyADOs.Remove(vaccinationMetyADO);
                    gridControlVaccinationMety.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadAcsUser(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMediStockCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadMediStock(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAcsUser_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAcsUser.EditValue != null)
                    {
                        List<ACS.EFMODEL.DataModels.ACS_USER> acsUsers = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>();

                        ACS_USER data = acsUsers != null ? acsUsers.FirstOrDefault(o => o.LOGINNAME == cboAcsUser.EditValue.ToString()) : null;
                        if (data != null)
                        {
                            txtLoginname.Text = data.LOGINNAME;
                            cboAcsUser.Properties.Buttons[1].Visible = true;
                            btnSave.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMediStockName.EditValue != null)
                    {
                        cboPatientType.Focus();
                        cboPatientType.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAcsUser_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAcsUser.Properties.Buttons[1].Visible = false;
                    cboAcsUser.EditValue = null;
                    txtLoginname.Text = "";
                    txtLoginname.Focus();
                    txtLoginname.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMediStockName.Properties.Buttons[1].Visible = false;
                    cboMediStockName.EditValue = null;
                    txtMediStockCode.Text = "";
                    txtMediStockCode.Focus();
                    txtMediStockCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVaccinationMety_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ExpMestMedicineADO data = view.GetFocusedRow() as ExpMestMedicineADO;
                if (view.FocusedColumn.FieldName == "TDL_MEDICINE_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        if (data.TDL_MEDICINE_TYPE_ID > 0)
                        {
                            editor.EditValue = data.TDL_MEDICINE_TYPE_ID;
                            editor.Properties.Buttons[1].Visible = true;
                            editor.ButtonClick += reposityButtonClick;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void reposityButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridLookUpEdit editor = sender as GridLookUpEdit;
                    if (editor != null)
                    {
                        editor.EditValue = null;
                        editor.Properties.Buttons[1].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignee_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAssignee.Enabled)
                {
                    return;
                }
                CommonParam param = new CommonParam();
                bool success = false;
                this.ValidateAssignee();

                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (this.CheckAssignee())
                {
                    WaitingManager.Show();
                    HisVaccinationAssignSDO hisVaccinationAssignSDO = new HisVaccinationAssignSDO();
                    this.CreateAssigneeSDO(ref hisVaccinationAssignSDO);
                    string requestUri = actionAssign == EnumUtil.ACTION.CREATE ? "api/HisVaccination/AssignCreate" : "api/HisVaccination/AssignUpdate";
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("hisVaccinationAssignSDO", hisVaccinationAssignSDO));
                    this.VaccinationResult = new BackendAdapter(param).Post<VaccinationResultSDO>(requestUri, ApiConsumers.MosConsumer, hisVaccinationAssignSDO, param);
                    WaitingManager.Hide();
                    if (VaccinationResult != null)
                    {
                        success = true;
                        long vaccinationId = VaccinationResult.Vaccinations != null && VaccinationResult.Vaccinations.Count > 0 ? VaccinationResult.Vaccinations[0].ID : 0;
                        UpdateVaccinationMety(vaccinationId);
                        LoadVaccinationMetyByPatient(vaccinationExam.PATIENT_ID);
                        LoadOldVaction(vaccinationExam);
                        actionAssign = EnumUtil.ACTION.UPDATE;
                        btnAssignee.Text = "Sửa đơn";
                        EnabledComboMediStock();
                        if (chkPrintAssign.Checked)
                        {
                            SetUpToPrint();
                            ProcessPrintAssign(true);
                        }
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateVaccinationMety(long vaccinationId)
        {
            try
            {
                List<ExpMestMedicineADO> expMestMedicines = gridControlVaccinationMety.DataSource as List<ExpMestMedicineADO>;
                if (expMestMedicines != null && expMestMedicines.Count > 0 && vaccinationId > 0)
                {
                    expMestMedicines.ForEach(o => o.TDL_VACCINATION_ID = vaccinationId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVaccinationMetyByPatient_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
            {
                ExpMestMedicineADO dataRow = (ExpMestMedicineADO)((System.Collections.IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                }
                else if (e.Column.FieldName == "EXECUTE_TIME_DISPLAY")
                {
                    if (dataRow.EXECUTE_TIME.HasValue)
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.EXECUTE_TIME ?? 0);
                }
            }
            else
            {
                e.Value = null;
            }
        }

        private void dtRequestTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAssignee.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPatientType.Properties.Buttons[1].Visible = false;
                    cboPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStockName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                InitExpMestMedicineGrid();
                long? mediStockId = null;
                if (cboMediStockName.EditValue != null)
                {
                    mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStockName.EditValue.ToString());
                    V_HIS_MEST_ROOM mestRoom = this.mestRooms.FirstOrDefault(o => o.MEDI_STOCK_ID == mediStockId);
                    txtMediStockCode.Text = mestRoom.MEDI_STOCK_CODE;
                    cboMediStockName.Properties.Buttons[1].Visible = true;
                }
                LoadMedicineByMediStock(mediStockId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPatientType.EditValue == null)
                    {
                        cboPatientType.ShowPopup();
                    }
                    else
                    {
                        dtRequestTime.Focus();
                        dtRequestTime.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (cboPatientType.EditValue != null)
                {
                    dtRequestTime.Focus();
                    dtRequestTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPatientType.EditValue != null)
                {
                    cboPatientType.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnConnectBloodPressure_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_DHST data = HIS.Desktop.Plugins.Library.ConnectBloodPressure.ConnectBloodPressureProcessor.GetData();
                if (data != null)
                {
                    if (data.EXECUTE_TIME != null)
                        dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtExecuteTime.EditValue = DateTime.Now;

                    if (data.BLOOD_PRESSURE_MAX.HasValue)
                    {
                        spinBloodPressureMax.EditValue = data.BLOOD_PRESSURE_MAX;
                    }
                    if (data.BLOOD_PRESSURE_MIN.HasValue)
                    {
                        spinBloodPressureMin.EditValue = data.BLOOD_PRESSURE_MIN;
                    }
                    if (data.BREATH_RATE.HasValue)
                    {
                        spinBreathRate.EditValue = data.BREATH_RATE;
                    }
                    if (data.HEIGHT.HasValue)
                    {
                        spinHeight.EditValue = data.HEIGHT;
                    }
                    if (data.CHEST.HasValue)
                    {
                        spinChest.EditValue = data.CHEST;
                    }
                    if (data.BELLY.HasValue)
                    {
                        spinBelly.EditValue = data.BELLY;
                    }
                    if (data.PULSE.HasValue)
                    {
                        spinPulse.EditValue = data.PULSE;
                    }
                    if (data.TEMPERATURE.HasValue)
                    {
                        spinTemperature.EditValue = data.TEMPERATURE;
                    }
                    if (data.WEIGHT.HasValue)
                    {
                        spinWeight.EditValue = data.WEIGHT;
                    }
                    if (!String.IsNullOrWhiteSpace(data.NOTE))
                    {
                        txtNote.Text = data.NOTE;
                    }
                    if (data.SPO2.HasValue)
                        spinSPO2.Value = (data.SPO2.Value * 100);
                    else
                        spinSPO2.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DHSTFillDataToBmiAndLeatherArea()
        {
            try
            {
                decimal bmi = 0;
                if (spinHeight.EditValue != null && spinHeight.Value != 0)
                {
                    bmi = (spinWeight.Value) / ((spinHeight.Value / 100) * (spinHeight.Value / 100));
                }
                double leatherArea = 0.007184 * Math.Pow((double)spinHeight.Value, 0.725) * Math.Pow((double)spinWeight.Value, 0.425);
                lblBMI.Text = Math.Round(bmi, 2) + "";
                lblLeatherArea.Text = Math.Round(leatherArea, 2) + "";
                if (bmi < 16)
                {
                    lblBmiDisplayText.Text = "(Gầy độ III)";
                }
                else if (16 <= bmi && bmi < 17)
                {
                    lblBmiDisplayText.Text = "(Gầy độ II)";
                }
                else if (17 <= bmi && bmi < (decimal)18.5)
                {
                    lblBmiDisplayText.Text = "(Gầy độ I)";
                }
                else if ((decimal)18.5 <= bmi && bmi < 25)
                {
                    lblBmiDisplayText.Text = "(Bình thường)";
                }
                else if (25 <= bmi && bmi < 30)
                {
                    lblBmiDisplayText.Text = "(Thừa cân)";
                }
                else if (30 <= bmi && bmi < 35)
                {
                    lblBmiDisplayText.Text = "(Béo phì độ I)";
                }
                else if (35 <= bmi && bmi < 40)
                {
                    lblBmiDisplayText.Text = "(Béo phì độ II)";
                }
                else if (40 < bmi)
                {
                    lblBmiDisplayText.Text = "(Béo phì độ III)";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DHSTFillDataToBmiAndLeatherArea();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinPulse_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBloodPressureMax.Focus();
                    spinBloodPressureMax.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodPressureMax_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBloodPressureMin.Focus();
                    spinBloodPressureMin.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodPressureMin_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinWeight.Focus();
                    spinWeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinWeight_EditValueChanged_1(object sender, EventArgs e)
        {
            try
            {
                DHSTFillDataToBmiAndLeatherArea();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinWeight_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinHeight.Focus();
                    spinHeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSPO2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTemperature.Focus();
                    spinTemperature.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTemperature_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBreathRate.Focus();
                    spinBreathRate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBreathRate_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinChest.Focus();
                    spinChest.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChest_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBelly.Focus();
                    spinBelly.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBelly_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHeight_Leave(object sender, EventArgs e)
        {
            try
            {
                spinSPO2.Focus();
                spinSPO2.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinHeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSPO2.Focus();
                    spinSPO2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider2_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnCallPatient_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var VaccinationExam = (V_HIS_VACCINATION_EXAM)gridViewVaccinationExam.GetFocusedRow();
                if (VaccinationExam != null)
                {
                    UpdateDicCallPatient(VaccinationExam);
                    LoadCallPatientByThread(VaccinationExam);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintAssign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintAssign.Name && o.MODULE_LINK == this.CurrentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintAssign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintAssign.Name;
                    csAddOrUpdate.VALUE = (chkPrintAssign.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = this.CurrentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintAppointment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintAppointment.Name && o.MODULE_LINK == this.CurrentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintAppointment.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintAppointment.Name;
                    csAddOrUpdate.VALUE = (chkPrintAppointment.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = this.CurrentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintAssign_Click(object sender, EventArgs e)
        {
           
        }

        private void btnAppointment_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAppointment.Enabled)
                {
                    return;
                }
                CommonParam param = new CommonParam();
                bool success = false;

                this.positionHandle = -1;
                if (!dxValidationProvider3.Validate())
                    return;
                if (this.CheckAppointment())
                {
                    WaitingManager.Show();
                    HisVaccinationAppointmentSDO hisVaccinationAppointmentSDO = new HisVaccinationAppointmentSDO();
                    this.CreateAppointmentSDO(ref hisVaccinationAppointmentSDO);
                    string requestUri = "api/HisVaccinationExam/Appointment";
                    this.VaccAppointmentResult = new BackendAdapter(param).Post<List<V_HIS_VACC_APPOINTMENT>>(requestUri, ApiConsumers.MosConsumer, hisVaccinationAppointmentSDO, param);
                    WaitingManager.Hide();
                    if (VaccAppointmentResult != null)
                    {
                        success = true;
                        btnPrintAppointment.Enabled = true;
                        if (chkPrintAppointment.Checked)
                        {
                            ProcessPrintAppointment(true);
                        }
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrintAppointment_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.VaccAppointmentResult != null && this.VaccAppointmentResult.Count > 0)
                {
                    ProcessPrintAppointment(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnAddAppointmentVac_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var vaccinationMetyADOs = gridControlAppointmentVacc.DataSource as List<AppointmentVaccineADO>;
                AppointmentVaccineADO vaccinationMetyADO = new AppointmentVaccineADO();
                vaccinationMetyADO.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                vaccinationMetyADOs.Add(vaccinationMetyADO);
                gridControlAppointmentVacc.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnRemoveAppointmentVacc_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var vaccinationMetyADOs = gridControlAppointmentVacc.DataSource as List<AppointmentVaccineADO>;
                var vaccinationMetyADO = gridViewAppointmentVacc.GetFocusedRow() as AppointmentVaccineADO;
                if (vaccinationMetyADO != null)
                {
                    vaccinationMetyADOs.Remove(vaccinationMetyADO);
                    gridControlAppointmentVacc.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAppointmentVacc_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "ACTION")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewAppointmentVacc.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repositoryItemBtnAddAppointmentVac;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = repositoryItemBtnRemoveAppointmentVacc;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAppointmentVacc_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                AppointmentVaccineADO data = view.GetFocusedRow() as AppointmentVaccineADO;
                if (view.FocusedColumn.FieldName == "VACCINE_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null && editor != null)
                    {
                        if (data.VACCINE_TYPE_ID > 0)
                        {
                            editor.EditValue = data.VACCINE_TYPE_ID;
                            editor.Properties.Buttons[1].Visible = true;
                            editor.ButtonClick += reposityButtonClick;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider3_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCboVaccineType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    var cbo = sender as HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit;
                    if (cbo.EditValue != null)
                    {
                        var row = gridViewAppointmentVacc.GetFocusedRow() as AppointmentVaccineADO;
                        row.VACCINE_TYPE_ID = (long)cbo.EditValue;
                        gridControlAppointmentVacc.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAppointmentVacc_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var row = (AppointmentVaccineADO)gridViewAppointmentVacc.GetFocusedRow();
                if (view.FocusedColumn.FieldName == gc_VaccineType.FieldName && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null && row != null)
                    {
                        editor.EditValue = row.VACCINE_TYPE_ID;
                        editor.Properties.Buttons[1].Visible = true;
                        editor.ButtonClick += reposityButtonClick;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAppointmentVacc_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == gc_VaccineType.FieldName)
                {
                    if (e.Value != null && this.VaccineTypes != null)
                    {
                        long id = Convert.ToInt64(e.Value);
                        var type = this.VaccineTypes.FirstOrDefault(o => o.ID == id);
                        if (type != null)
                        {
                            e.DisplayText = type.VACCINE_TYPE_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void gridViewVaccExamResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            try
            {
                int[] selectRows = gridViewVaccExamResult.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    radioGroupConclude.Properties.Items[1].Enabled = false;
                    radioGroupConclude.SelectedIndex = 2;
                }
				else
				{
                    radioGroupConclude.Properties.Items[1].Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

		private void btnTienSuBenhNhan_Click(object sender, EventArgs e)
		{
            try
            {
                key = 1;
                OpenModuleTextLibrary(mmPtPathologicalHistory.Text, "tiensubenhtiemchung");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

		private void btnTienSuDiUng_Click(object sender, EventArgs e)
		{
            try
            {
                key = 2;
                OpenModuleTextLibrary(mmPtAllergicHistory.Text, "tiensudiungtiemchung");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OpenModuleTextLibrary(string content, string hashtag)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TextLibrary").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TextLibrary");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    TextLibraryInfoADO ado = new TextLibraryInfoADO();
                    ado.Content = content;
                    ado.Hashtag = hashtag;
                    listArgs.Add(ado);
                    listArgs.Add((HIS.Desktop.Common.DelegateDataTextLib)ProcessDataTextLib);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.CurrentModule.RoomId, this.CurrentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                    WaitingManager.Hide();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataTextLib(MOS.EFMODEL.DataModels.HIS_TEXT_LIB textLib)
        {
            try
            {
                switch (key)
                {
                    case 1:
                        this.mmPtPathologicalHistory.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 2:
                        this.mmPtAllergicHistory.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

		private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			try
			{
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_VACCINATION dataRow = (V_HIS_VACCINATION)((System.Collections.IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "REQUEST_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.REQUEST_TIME) +" - "+ dataRow.REQUEST_ROOM_CODE;
                    }
                    else if (e.Column.FieldName == "REQUEST_LOGINNAME_USERNAME")
                    {
                        e.Value = dataRow.REQUEST_LOGINNAME + " - " + dataRow.REQUEST_USERNAME;
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

		private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
		{
            try
            {
                if (e.Column.FieldName != "btnEdit" || e.Column.FieldName != "btnPrint")
                {
                    EnableControlVacMety(true);
                    ShowDataToVaccMety();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        private void EnableControlVacMety(bool IsEnable)
		{
			try
			{
                txtMediStockCode.ReadOnly = IsEnable;
                cboMediStockName.ReadOnly = IsEnable;
                cboPatientType.ReadOnly = IsEnable;
                dtRequestTime.ReadOnly = IsEnable;
                gridViewlVaccinationMety.OptionsBehavior.Editable = !IsEnable;
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

		private void btnNew_Click(object sender, EventArgs e)
		{
			try
			{
                ResetAssign();
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        private void ResetAssign()
		{
            try
            {
                btnAssignee.Enabled = btnAssignee.Enabled;
                EnableControlVacMety(false);
                actionAssign = EnumUtil.ACTION.CREATE;
                btnAssignee.Text = "Lưu đơn";
                EnabledComboMediStock();
                cboMediStockName.EditValue = null;
                txtMediStockCode.Text = "";
                cboPatientType.EditValue = vaccinationExam.PATIENT_TYPE_ID;
                dtRequestTime.DateTime = DateTime.Now;
                InitExpMestMedicineGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

		private void repbtnEditEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
                EnableControlVacMety(false);
                ShowDataToVaccMety();


            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        private void ShowDataToVaccMety()
		{
			try
			{
                cboMediStockName.Enabled = true;
                txtMediStockCode.Enabled = true;
                currentVaccination = (V_HIS_VACCINATION)gridView1.GetFocusedRow();
                var expMestMedicineTempGroups = expMestMedicine5s != null
                   ? expMestMedicine5s.Where(o => o.VACCINATION_EXAM_ID == vaccinationExam.ID && o.TDL_VACCINATION_ID == currentVaccination.ID)
                   .GroupBy(o => new { o.TDL_MEDICINE_TYPE_ID }).ToList() : null;
                if (expMestMedicineTempGroups != null && expMestMedicineTempGroups.Count > 0)
                {
                    if (currentVaccination.REQUEST_TIME > 0)
                    {
                        dtRequestTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentVaccination.REQUEST_TIME) ?? DateTime.Now;
                    }
                    actionAssign = EnumUtil.ACTION.UPDATE;

                    V_HIS_EXP_MEST_MEDICINE_5 expMestMedicine5 = expMestMedicineTempGroups.First().First();

                    V_HIS_MEDI_STOCK mediStock = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDI_STOCK>()
                       .FirstOrDefault(o => o.ID == expMestMedicine5.TDL_MEDI_STOCK_ID);
                    if (mediStock != null)
                    {
                        cboMediStockName.Enabled = false;
                        txtMediStockCode.Enabled = false;
                        cboMediStockName.EditValue = mediStock.ID;
                        txtMediStockCode.Text = mediStock.MEDI_STOCK_CODE;
                    }
                    if (expMestMedicine5.PATIENT_TYPE_ID.HasValue)
                    {
                        cboPatientType.EditValue = expMestMedicine5.PATIENT_TYPE_ID;
                    }
                    List<ExpMestMedicineADO> expMestMedicineADOs = new List<ExpMestMedicineADO>();
                    foreach (var g in expMestMedicineTempGroups)
                    {
                        ExpMestMedicineADO ado = new ExpMestMedicineADO();
                        ado.TDL_MEDICINE_TYPE_ID = g.First().TDL_MEDICINE_TYPE_ID;
                        ado.AMOUNT = g.Sum(o => o.AMOUNT);
                        ado.VACCINE_TURN = g.Sum(o => o.VACCINE_TURN ?? 0);
                        ado.TDL_VACCINATION_ID = g.First().TDL_VACCINATION_ID;
                        if (expMestMedicineTempGroups.IndexOf(g) == 0)
                            ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        else
                            ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;

                        V_HIS_EXP_MEST_MEDICINE mety = new V_HIS_EXP_MEST_MEDICINE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(mety, g.First());
                        ado.AMOUNT = g.Sum(o => o.AMOUNT);
                        expMestMedicineADOs.Add(ado);
                    }
                    gridControlVaccinationMety.DataSource = expMestMedicineADOs;
                    btnAssignee.Text = "Sửa đơn";
                }
                else
                {
                    ResetAssign();
                }
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

		private void repbtnPrintEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
		{
			try
			{
                SetUpToPrint();
                if (this.VaccinationResult != null)
                {
                    ProcessPrintAssign(false);
                }
                else
                {
                    XtraMessageBox.Show("Chưa có thông tin đơn");
                }
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        private void SetUpToPrint()
		{
			try
			{
                currentVaccination = (V_HIS_VACCINATION)gridView1.GetFocusedRow();
                var expMestMedicineTempGroups = expMestMedicine5s != null
                   ? expMestMedicine5s.Where(o => o.VACCINATION_EXAM_ID == vaccinationExam.ID && o.TDL_VACCINATION_ID == currentVaccination.ID)
                   .GroupBy(o => new { o.TDL_MEDICINE_TYPE_ID }).ToList() : null;
                if (expMestMedicineTempGroups != null && expMestMedicineTempGroups.Count > 0)
                {                   
                    V_HIS_EXP_MEST_MEDICINE_5 expMestMedicine5 = expMestMedicineTempGroups.First().First();

                    V_HIS_MEDI_STOCK mediStock = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDI_STOCK>()
                       .FirstOrDefault(o => o.ID == expMestMedicine5.TDL_MEDI_STOCK_ID);
                
                    this.VaccinationResult = new MOS.SDO.VaccinationResultSDO();
                    List<long> vaccinationIds = expMestMedicineTempGroups.Select(s => s.First().TDL_VACCINATION_ID ?? 0).ToList();
                    if (vaccinationIds != null && vaccinationIds.Count > 0)
                    {
                        CommonParam paparam = new CommonParam();
                        HisVaccinationViewFilter vaccFilter = new HisVaccinationViewFilter();
                        vaccFilter.IDs = vaccinationIds.Distinct().ToList();
                        this.VaccinationResult.Vaccinations = lstVaccination = new BackendAdapter(paparam).Get<List<V_HIS_VACCINATION>>("api/HisVaccination/GetView", ApiConsumers.MosConsumer, vaccFilter, paparam);
                    }
                    this.VaccinationResult.Medicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                    foreach (var g in expMestMedicineTempGroups)
                    {
                        ExpMestMedicineADO ado = new ExpMestMedicineADO();
                        ado.TDL_MEDICINE_TYPE_ID = g.First().TDL_MEDICINE_TYPE_ID;
                        ado.AMOUNT = g.Sum(o => o.AMOUNT);
                        ado.TDL_VACCINATION_ID = g.First().TDL_VACCINATION_ID;
                        if (expMestMedicineTempGroups.IndexOf(g) == 0)
                            ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        else
                            ado.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;

                        V_HIS_EXP_MEST_MEDICINE mety = new V_HIS_EXP_MEST_MEDICINE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(mety, g.First());
                        ado.AMOUNT = g.Sum(o => o.AMOUNT);
                        this.VaccinationResult.Medicines.Add(mety);
                    }
                }
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    
                    V_HIS_VACCINATION data = (V_HIS_VACCINATION)view.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "Delete")
                    {

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.VACCINATION_STT_ID), data.VACCINATION_STT_ID));
                        if ((data.REQUEST_LOGINNAME == loginName || CheckAdmin.IsAdmin(loginName)) && data.VACCINATION_STT_ID != IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH
                            && (this.vaccinationExam != null ? this.vaccinationExam.VACCINATION_EXAM_STT_ID !=3 : true))
                        {
                            
                            Inventec.Common.Logging.LogSystem.Debug("IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH), IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH));
                            e.RepositoryItem = repositoryDeleteE;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryDeleteD;
                        }
                      

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repositoryDeleteE_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var data = (V_HIS_VACCINATION)gridView1.GetFocusedRow();
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HisVaccinationSDO hisVacciSDO = new HisVaccinationSDO();
                    hisVacciSDO.Id = data.ID;
                    hisVacciSDO.RequestRoomId = this.roomId;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dữ liệu: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => hisVacciSDO), hisVacciSDO));
                    bool result = new BackendAdapter(param)
                        .Post<bool>("api/HisVaccination/Delete", ApiConsumers.MosConsumer, hisVacciSDO, param);
                    WaitingManager.Hide();
                    if (result)
                    {
                        success = true;
                        //Lay gia tri moi nhat


                        var data_ = gridControl1.DataSource as List<V_HIS_VACCINATION>;
                        if (data_ != null && data_.Count > 0)
                        {
                            V_HIS_VACCINATION delete = new V_HIS_VACCINATION();
                            delete = data_.FirstOrDefault(o => o.ID == data.ID);
                            data_.Remove(delete);
                            gridControl1.DataSource = data_;
                        }
                    }
                    gridControl1.RefreshDataSource();
                    MessageManager.Show(this.ParentForm, param, success);
                } 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridViewVaccinationExam_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                var position = Cursor.Position;
                if (hi.InRowCell)
                {
                    int rowHandle = gridViewVaccinationExam.GetVisibleRowHandle(hi.RowHandle);

                    this.vaccinationExam_ForProcess = (V_HIS_VACCINATION_EXAM)gridViewVaccinationExam.GetRow(rowHandle);

                    //gridViewVaccinationExam.OptionsSelection.EnableAppearanceFocusedCell = true;
                    //gridViewVaccinationExam.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }
                    popupMenuProcessor = new PopupMenuProcessor(this.vaccinationExam_ForProcess, barManager1, VaccinationExam_MouseRightClick, (RefeshReference)BtnSearch);//(RefeshReference)BtnSearch
                    popupMenuProcessor.InitMenu(position);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnSearch()
        {
            try
            {
                if (btnFind.Enabled)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #region Tooltip
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        int lastRowHandle = -1;

        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlVaccinationExam)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlVaccinationExam.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "TRANGTHAI_IMG")
                            {
                                long serviceReqSTTId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "VACCINATION_EXAM_STT_ID") ?? "").ToString());
                                if (serviceReqSTTId == 1)
                                {
                                    text = "Chưa xử lý";
                                }
                                else if (serviceReqSTTId == 2)
                                {
                                    text = "Đang xử lý";
                                }
                                else if (serviceReqSTTId == 3)
                                {
                                    text = "Kết thúc";
                                }

                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridViewVaccExamResult_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                int[] selectRows = gridViewVaccExamResult.GetSelectedRows();
                if (gridViewVaccExamResult.FocusedColumn.FieldName == "Note" && !selectRows.Contains(gridViewVaccExamResult.FocusedRowHandle))
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnInPhieuKham_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessInPhieuKham(this.vaccinationExam);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessInPhieuKham(V_HIS_VACCINATION_EXAM VaccinationExam)
        {
            try
            {
                bool? isBaby = null; 
                if (vaccinationExam != null)
                {
                    isBaby = false;
                    var requestDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vaccinationExam.REQUEST_DATE);
                    var dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vaccinationExam.TDL_PATIENT_DOB);
                    if (requestDate != null && dob != null)
                    {
                        var resultDate = requestDate - dob.Value.Date;
                        if (resultDate.Value.Days + 1 < 30)
                        {
                            isBaby = true;
                        }
                    }
                }
                if (isBaby == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("VaccinationExam is null!");
                }
                else if (isBaby == false)
                {
                    ProcessPrintPhieuKham_MPS475(false);
                }
                else if (isBaby == true)
                {
                    ProcessPrintPhieuKham_MPS474(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnKetThuc_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessKetThuc(this.vaccinationExam);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessKetThuc(V_HIS_VACCINATION_EXAM VaccinationExam)
        {
            try
            {
                if (VaccinationExam != null)
                {
                    bool success = false;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    var apiResult = new BackendAdapter(param).Post<HIS_VACCINATION_EXAM>("api/HisVaccinationExam/Finish", ApiConsumers.MosConsumer, VaccinationExam.ID, param);
                    if (apiResult != null)
                    {
                        success = true;
                        FillDataToGridVaccinationExam();
                        btnSave.Enabled = false;
                        btnKetThuc.Enabled = false;
                        btnAssignee.Enabled = false;
                        btnNew.Enabled = false;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void radioGroupTestHbsAg_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioGroupTestHbsAg.SelectedIndex == 1)//check 'Có'
                {
                    radioGroupPositiveResult.Enabled = true;
                }
                else
                {
                    radioGroupPositiveResult.SelectedIndex = -1;
                    radioGroupPositiveResult.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void radioGroupSpecialistExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioGroupSpecialistExam.SelectedIndex == 1)//check 'Có'
                {
                    txtSpecialistDepartment.Enabled = true;
                    cboSpecialistDepartment.Enabled = true;
                    txtSpecialistReason.Enabled = true;
                    txtSpecialistResult.Enabled = true;
                    txtSpecialistConclude.Enabled = true;
                }
                else
                {
                    txtSpecialistDepartment.Enabled = false;
                    cboSpecialistDepartment.Enabled = false;
                    txtSpecialistReason.Enabled = false;
                    txtSpecialistResult.Enabled = false;
                    txtSpecialistConclude.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSpecialistDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboSpecialistDepartment.EditValue != null)
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o=>o.ID == Convert.ToInt64(cboSpecialistDepartment.EditValue)).FirstOrDefault();

                    if (data != null)
                    {
                        txtSpecialistDepartment.Text = data.DEPARTMENT_CODE;
                        cboSpecialistDepartment.Properties.Buttons[1].Visible = true;
                    }
                }
                else
                {
                    txtSpecialistDepartment.Text = "";
                    cboSpecialistDepartment.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSpecialistDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSpecialistDepartment.Properties.Buttons[1].Visible = false;
                    cboSpecialistDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSpecialistDepartment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadSpecialistDepartment(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MouseUpUncheckRadio(ref object sender, ref MouseEventArgs e)
        {
            try
            {
                var rg = sender as RadioGroup;
                var vi = rg.GetViewInfo() as RadioGroupViewInfo;
                if (vi == null) return;
                int clickIndex = vi.GetItemIndexByPoint(e.Location);
                if (clickIndex == rg.SelectedIndex)
                {
                    rg.SelectedIndex = -1;
                    vi.HitIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void radioGroupRabiesWoundRank_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                MouseUpUncheckRadio(ref sender, ref e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void radioGroupRabiesAnimalStatus_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                MouseUpUncheckRadio(ref sender, ref e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void radioGroupPositiveResult_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                MouseUpUncheckRadio(ref sender,ref e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
