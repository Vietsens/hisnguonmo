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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TrackImmunizationHistoryInfor.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TrackImmunizationHistoryInfor
{
    public partial class frmTrackImmunizationHistoryInfor : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;


        List<HIS_VACCINATION> _Vaccitions { get; set; }
        List<HIS_MEDICINE> _Medicines { get; set; }

        List<V_HIS_EXP_MEST_MEDICINE_5> expMestMedicine5s { get; set; }
        V_HIS_PATIENT _currentPatient { get; set; }

        int positionHandle = -1;

        long? _PatientId = 0;


        public frmTrackImmunizationHistoryInfor()
        {
            InitializeComponent();
        }

        public frmTrackImmunizationHistoryInfor(Inventec.Desktop.Common.Modules.Module _Module)
            : base(_Module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _Module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTrackImmunizationHistoryInfor_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                ValidateAssignee();
                dtRequestTime.DateTime = DateTime.Now;
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

                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ReSetData();
                Search(txtTreatmentCode.Text.Trim());
            }
        }

        private void Search(string code)
        {
            try
            {
                MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                if (!string.IsNullOrEmpty(code))
                {
                    string str = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    txtTreatmentCode.Text = str;
                    filter.PATIENT_CODE__EXACT = str;
                }
                else
                    return;
                filter.IS_ACTIVE = 1;

                _currentPatient = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();

                if (_currentPatient != null)
                {
                    this._PatientId = _currentPatient.ID;

                    LoadExpMestMedicines(this._PatientId, 0);//lich su tiem


                    txtPatientName.Text = _currentPatient.VIR_PATIENT_NAME;
                    if (_currentPatient.IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtDob.Text = _currentPatient.DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        txtDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_currentPatient.DOB);
                    }
                    txtGenderName.Text = _currentPatient.GENDER_NAME;
                    txtAddress.Text = _currentPatient.VIR_ADDRESS;
                    txtRelativeName.Text = _currentPatient.RELATIVE_NAME;
                    txtRelativePhone.Text = _currentPatient.RELATIVE_PHONE;
                }
                else
                {
                    txtPatientName.Text = "";
                    txtDob.Text = "";
                    txtGenderName.Text = "";
                    txtAddress.Text = "";
                    txtRelativeName.Text = "";
                    txtRelativePhone.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchVaccintion(string code)
        {
            try
            {
                MOS.Filter.HisVaccinationViewFilter filter = new MOS.Filter.HisVaccinationViewFilter();
                if (!string.IsNullOrEmpty(code))
                {
                    string str = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    txtVaccintionCode.Text = str;
                    filter.VACCINATION_CODE__EXACT = str;
                }
                else
                    return;

                filter.IS_ACTIVE = 1;

                var data = new BackendAdapter(new CommonParam()).Get<List<V_HIS_VACCINATION>>("api/HisVaccination/GetView", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();

                if (data != null)
                {
                    LoadExpMestMedicines(0, data.ID);//lich su tiem

                    LoadVaccinatitonMety(data.VACCINATION_EXAM_ID);

                    _VaccinationExam = GetVaccinationExamById(data.VACCINATION_EXAM_ID);

                    SearchByPatientId(data.PATIENT_ID);
                }
                else
                {
                    txtPatientName.Text = "";
                    txtDob.Text = "";
                    txtGenderName.Text = "";
                    txtAddress.Text = "";
                    txtRelativeName.Text = "";
                    txtRelativePhone.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMedicines(long? patientId, long VaccinId)
        {
            try
            {
                _Vaccitions = new List<HIS_VACCINATION>();
                _Medicines = new List<HIS_MEDICINE>();

                gridControlData.DataSource = null;


                HisExpMestMedicineView5Filter filter = new HisExpMestMedicineView5Filter();
                if (patientId > 0)
                {
                    filter.PATIENT_ID = patientId;
                }
                else if (VaccinId > 0)
                {
                    filter.TDL_VACCINATION_ID = VaccinId;
                }

                filter.ORDER_FIELD = "EXECUTE_TIME";
                filter.ORDER_DIRECTION = "ASC";
                expMestMedicine5s = new BackendAdapter(new CommonParam())
                       .Get<List<V_HIS_EXP_MEST_MEDICINE_5>>("api/HisExpMestMedicine/GetView5",
                       ApiConsumers.MosConsumer, filter, new CommonParam());
                List<ExpMestMedicineADO> expMestMedicineADOs = new List<ExpMestMedicineADO>();

                if (expMestMedicine5s != null && expMestMedicine5s.Count > 0)
                {
                    var expMestMedicineGroup = expMestMedicine5s
                        .GroupBy(o => o.TDL_MEDICINE_TYPE_ID);
                    foreach (var g in expMestMedicineGroup)
                    {
                        int index = 1;
                        foreach (var item in g)
                        {
                            ExpMestMedicineADO ado = new ExpMestMedicineADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineADO>(ado, item);
                            ado.AMOUNT = item.AMOUNT;
                            ado.TDL_MEDICINE_TYPE_ID = item.TDL_MEDICINE_TYPE_ID;
                            ado.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID;
                            ado.VaccinationIndex = index;
                            expMestMedicineADOs.Add(ado);
                            index++;
                        }
                    }

                    MOS.Filter.HisMedicineFilter _mediFilter = new HisMedicineFilter();
                    _mediFilter.IDs = expMestMedicine5s.Select(p => p.MEDICINE_ID ?? 0).Distinct().ToList();
                    _Medicines = new BackendAdapter(new CommonParam())
                        .Get<List<HIS_MEDICINE>>("api/HisMedicine/Get",
                        ApiConsumers.MosConsumer, _mediFilter, new CommonParam());

                    MOS.Filter.HisVaccinationFilter _vacFilter = new HisVaccinationFilter();
                    _vacFilter.IDs = expMestMedicine5s.Select(p => p.TDL_VACCINATION_ID ?? 0).Distinct().ToList();
                    _Vaccitions = new BackendAdapter(new CommonParam())
                        .Get<List<HIS_VACCINATION>>("api/HisVaccination/Get",
                        ApiConsumers.MosConsumer, _vacFilter, new CommonParam());

                    //MOS.Filter.HisVaccinationExamFilter _vacExamFilter = new HisVaccinationExamFilter();
                    //_vacFilter.IDs = expMestMedicine5s.Select(p => p.TDL_VACCINATION_ID ?? 0).Distinct().ToList();
                    //_Vaccitions = new BackendAdapter(new CommonParam())
                    //    .Get<List<HIS_VACCINATION_EXAM>>("api/HisVaccination/Get",
                    //    ApiConsumers.MosConsumer, _vacFilter, new CommonParam());

                }
                gridControlData.DataSource = expMestMedicineADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<D_HIS_MEDI_STOCK_1> mediStocks;
        private void LoadVaccin(long? mediStockId)
        {
            try
            {
                mediStocks = new List<D_HIS_MEDI_STOCK_1>(); ;
                if (mediStockId.HasValue)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock1Filter filter = new MOS.Filter.DHisMediStock1Filter();
                    filter.MEDI_STOCK_IDs = new List<long> { mediStockId.Value };
                    filter.IS_VACCINE = true;
                    mediStocks = new BackendAdapter(param).Get<List<D_HIS_MEDI_STOCK_1>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, param);
                    WaitingManager.Hide();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "Mã thuốc", 150, 1, true));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "Tên thuốc", 250, 2, true));
                columnInfos.Add(new ColumnInfo("AMOUNT", "Số lượng", 100, 3, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, true, 500);
                ControlEditorLoader.Load(cboMedicine, mediStocks, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtVaccintionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ReSetData();
                    SearchVaccintion(txtVaccintionCode.Text.Trim());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_EXP_MEST_MEDICINE_5 dataRow = (V_HIS_EXP_MEST_MEDICINE_5)((System.Collections.IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "RQ_TIME")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.REQUEST_TIME);
                    }
                    else if (e.Column.FieldName == "EXECUTE_TIME_DISPLAY")
                    {
                        if (dataRow.EXECUTE_TIME.HasValue)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.EXECUTE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        var dataMedi = _Medicines.FirstOrDefault(p => p.ID == dataRow.MEDICINE_ID);
                        if (dataMedi != null)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataMedi.EXPIRED_DATE ?? 0);
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                    else if (e.Column.FieldName == "PACKAGE_NUMBER")
                    {
                        var dataMedi = _Medicines.FirstOrDefault(p => p.ID == dataRow.MEDICINE_ID);
                        if (dataMedi != null)
                        {
                            e.Value = dataMedi.PACKAGE_NUMBER;
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                    else if (e.Column.FieldName == "VACCINTION_CODE")
                    {
                        var dataVacc = _Vaccitions.FirstOrDefault(p => p.ID == dataRow.TDL_VACCINATION_ID);
                        if (dataVacc != null)
                        {
                            e.Value = dataVacc.VACCINATION_CODE;
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();

                CommonParam param = new CommonParam();
                bool success = false;

                HisVaccinationAssignSDO hisVaccinationAssignSDO = new HisVaccinationAssignSDO();

                hisVaccinationAssignSDO.RequestLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                hisVaccinationAssignSDO.RequestUsername = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                hisVaccinationAssignSDO.VaccinationExamId = _VaccinationExam.ID;


                long? vaccinationId = this._currentVaccination != null ? this._currentVaccination.ID : 0;


                List<VaccinationMetySDO> vaccinationMetyADOs = new List<VaccinationMetySDO>();
                if (cboMedicine.EditValue != null)
                {
                    VaccinationMetySDO sdo = new VaccinationMetySDO();
                    sdo.Amount = spinEditLieuDung.Value;
                    sdo.MediStockId = this._mediStockId ?? 0;
                    sdo.MedicineTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicine.EditValue.ToString());
                    sdo.PatientTypeId = this._patientTypeId ?? 0;
                    vaccinationMetyADOs.Add(sdo);
                }

                hisVaccinationAssignSDO.VaccinationMeties = vaccinationMetyADOs;
                hisVaccinationAssignSDO.WorkingRoomId = this.currentModule.RoomId;
                hisVaccinationAssignSDO.VaccinationId = vaccinationId;
                hisVaccinationAssignSDO.RequestTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtRequestTime.DateTime) ?? 0;

                //AssignUpdate
                //AssignCreate

                VaccinationResultSDO vaccination = new BackendAdapter(param)
                   .Post<VaccinationResultSDO>("api/HisVaccination/AssignCreate", ApiConsumers.MosConsumer, hisVaccinationAssignSDO, param);
                WaitingManager.Hide();
                if (vaccination != null)
                {
                    LoadExpMestMedicines(this._PatientId, vaccination.Vaccinations.FirstOrDefault().ID);

                    success = true;
                }
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var dataRow = (ExpMestMedicineADO)gridViewData.GetFocusedRow();
                if (dataRow != null)
                {
                    //TODO // Gan nguoc lai de them moi mui tiem
                    LoadVaccinatitonMety(dataRow.VACCINATION_EXAM_ID);

                    _VaccinationExam = GetVaccinationExamById(dataRow.VACCINATION_EXAM_ID);

                    SearchByPatientId(dataRow.PATIENT_ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchByPatientId(long patientId)
        {
            try
            {
                _currentVaccination = this._Vaccitions.FirstOrDefault(p => p.VACCINATION_EXAM_ID == _VaccinationExam.ID);

                txtVaccintionCode.Text = _currentVaccination != null ? _currentVaccination.VACCINATION_CODE : "";

                MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                filter.ID = patientId;
                filter.IS_ACTIVE = 1;

                _currentPatient = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();

                if (_currentPatient != null)
                {
                    txtPatientName.Text = _currentPatient.VIR_PATIENT_NAME;
                    if (_currentPatient.IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtDob.Text = _currentPatient.DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        txtDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_currentPatient.DOB);
                    }
                    txtGenderName.Text = _currentPatient.GENDER_NAME;
                    txtAddress.Text = _currentPatient.VIR_ADDRESS;
                    txtRelativeName.Text = _currentPatient.RELATIVE_NAME;
                    txtRelativePhone.Text = _currentPatient.RELATIVE_PHONE;
                }
                else
                {
                    txtPatientName.Text = "";
                    txtDob.Text = "";
                    txtGenderName.Text = "";
                    txtAddress.Text = "";
                    txtRelativeName.Text = "";
                    txtRelativePhone.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        long? _mediStockId = 0;
        long? _patientTypeId = 0;
        V_HIS_VACCINATION_EXAM _VaccinationExam;
        HIS_VACCINATION _currentVaccination;

        private void LoadVaccinatitonMety(long vaccintionExamId)
        {
            try
            {
                _mediStockId = 0;
                _patientTypeId = 0;

                V_HIS_EXP_MEST_MEDICINE_5 expMestMedicine5 = expMestMedicine5s != null
                    ? expMestMedicine5s.FirstOrDefault(o => o.VACCINATION_EXAM_ID == vaccintionExamId) : null;

                if (expMestMedicine5 != null)
                {
                    _mediStockId = expMestMedicine5.TDL_MEDI_STOCK_ID;

                    _patientTypeId = expMestMedicine5.PATIENT_TYPE_ID;
                }

                LoadVaccin(_mediStockId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_VACCINATION_EXAM GetVaccinationExamById(long id)
        {
            V_HIS_VACCINATION_EXAM result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisVaccinationExamViewFilter filter = new HisVaccinationExamViewFilter();
                filter.ID = id;
                List<V_HIS_VACCINATION_EXAM> vaccinationExams = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_VACCINATION_EXAM>>("api/HisVaccinationExam/GetView", ApiConsumers.MosConsumer, filter, param);
                if (vaccinationExams != null && vaccinationExams.Count == 1)
                {
                    result = vaccinationExams[0];
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ValidateAssignee()
        {
            try
            {
                ValidationVaccinationCode();
                ValidationMedicine();
                ValidateRequestTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMedicine()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule icdMainRule = new GridLookupEditWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtMedicineCode;
                icdMainRule.cbo = cboMedicine;
                icdMainRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtMedicineCode, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationVaccinationCode()
        {
            try
            {
                ControlEditValidationRule icdMainRule = new ControlEditValidationRule();
                icdMainRule.editor = txtVaccintionCode;
                icdMainRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtVaccintionCode, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateRequestTime()
        {
            try
            {
                ControlEditValidationRule icdMainRule = new ControlEditValidationRule();
                icdMainRule.editor = dtRequestTime;
                icdMainRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtRequestTime, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtMedicineCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim();
                    LoadMedicineToCombo(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMedicineToCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMedicine.Focus();
                    cboMedicine.ShowPopup();
                }
                else
                {
                    var data = this.mediStocks.Where(o => o.MEDICINE_TYPE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMedicine.EditValue = data[0].ID;
                            cboMedicine.Properties.Buttons[1].Visible = true;
                            spinEditLieuDung.Focus();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.MEDICINE_TYPE_CODE == searchCode);
                            if (search != null)
                            {
                                cboMedicine.EditValue = search.ID;
                                cboMedicine.Properties.Buttons[1].Visible = true;
                                spinEditLieuDung.Focus();
                            }
                            else
                            {
                                cboMedicine.EditValue = null;
                                cboMedicine.Focus();
                                cboMedicine.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboMedicine.EditValue = null;
                        cboMedicine.Focus();
                        cboMedicine.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicine_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMedicine.Properties.Buttons[1].Visible = false;
                    cboMedicine.EditValue = null;
                    txtMedicineCode.Text = "";
                    txtMedicineCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMedicine.EditValue != null)
                    {
                        var data = this.mediStocks.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicine.EditValue.ToString()));
                        if (data != null)
                        {
                            txtMedicineCode.Text = data.MEDICINE_TYPE_CODE;
                            cboMedicine.Properties.Buttons[1].Visible = true;
                            spinEditLieuDung.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicine_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMedicine.EditValue != null)
                    {
                        var data = this.mediStocks.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMedicine.EditValue.ToString()));
                        if (data != null)
                        {
                            txtMedicineCode.Text = data.MEDICINE_TYPE_CODE;
                            cboMedicine.Properties.Buttons[1].Visible = true;
                            spinEditLieuDung.Focus();
                        }
                    }
                }
                else
                {
                    cboMedicine.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinEditLieuDung_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtRequestTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReSetData()
        {
            try
            {
                mediStocks = new List<D_HIS_MEDI_STOCK_1>();

                txtMedicineCode.Text = "";
                cboMedicine.EditValue = null;
                cboMedicine.Properties.DataSource = null;
                spinEditLieuDung.EditValue = 1;
                dtRequestTime.DateTime = DateTime.Now;
                gridControlData.DataSource = null;
                txtPatientName.Text = "";
                txtGenderName.Text = "";
                txtDob.Text = "";
                txtAddress.Text = "";
                txtRelativeName.Text = "";
                txtRelativePhone.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnClose_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
