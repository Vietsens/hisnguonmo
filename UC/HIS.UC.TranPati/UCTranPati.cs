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
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;
using HIS.UC.TranPati.ADO;
using Inventec.Desktop.CustomControl;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using System.Text.RegularExpressions;

namespace HIS.UC.TranPati
{
    public partial class UCTranPati : UserControl
    {
        private List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG> HisMediOrgs;
        private List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON> HisTranPatiReasons;
        private List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> HisTranPatiForms;
        private List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH> HisTranPatiTechs;
        private DelegateNextFocus NextFocus;
        private int positionHandle = -1;
        private long treatmentId;
        List<MediOrgADO> listMediOrg { get; set; }

        #region ctor
        public UCTranPati()
        {
            InitializeComponent();
        }

        public UCTranPati(ADO.TranPatiInitADO data)
            : this()
        {
            try
            {
                if (data != null)
                {
                    SetTextHolder(data.IsTextHolder);
                    if (data.TranPatiData != null)
                    {
                        FillCurrentData(data.TranPatiData);
                    }
                    else
                    {
                        FillDataToForm(null);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Load
        private void SetTextHolder(bool p)
        {
            try
            {
                lciDauHieuLamSang.TextVisible = !p;
                lciHuongDieuTri.TextVisible = !p;
                lciKetQuaXetNghiem.TextVisible = !p;
                lciNguoiHoTong.TextVisible = !p;
                //lciPhuongPhapSuDung.TextVisible = !p;
                lciPhuongTienVanChuyen.TextVisible = !p;
                lciTinhTrangNguoiBenh.TextVisible = !p;
                lciThuocDaKeChoBenhNhan.TextVisible = !p;
                lciPhuongPhapDieuTri.TextVisible = !p;

                //txtDauHieuLamSang.Properties.NullValuePromptShowForEmptyValue = p;
                //txtDauHieuLamSang.Properties.ShowNullValuePromptWhenFocused = p;
                //txtHuongDieuTri.Properties.NullValuePromptShowForEmptyValue = p;
                //txtHuongDieuTri.Properties.ShowNullValuePromptWhenFocused = p;
                //txtNguoiHoTong.Properties.NullValuePromptShowForEmptyValue = p;
                //txtNguoiHoTong.Properties.ShowNullValuePromptWhenFocused = p;
                //txtPPKTThuoc.Properties.NullValuePromptShowForEmptyValue = p;
                ////txtPPKTThuoc.Properties.ShowNullValuePromptWhenFocused = p;
                //txtPhuongTienVanChuyen.Properties.NullValuePromptShowForEmptyValue = p;
                //txtPhuongTienVanChuyen.Properties.ShowNullValuePromptWhenFocused = p;
                //txtTinhTrangNguoiBenh.Properties.NullValuePromptShowForEmptyValue = p;
                //txtTinhTrangNguoiBenh.Properties.ShowNullValuePromptWhenFocused = p;
                //txtXetNghiem.Properties.NullValuePromptShowForEmptyValue = p;
                //txtXetNghiem.Properties.ShowNullValuePromptWhenFocused = p;

                txtDauHieuLamSang.Properties.NullValuePrompt = p ? GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_DAU_HIEU_LAM_SANG") : "";
                txtHuongDieuTri.Properties.NullValuePrompt = p ? GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_HUONG_DIEU_TRI") : "";
                txtNguoiHoTong.Properties.NullValuePrompt = p ? GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_NGUOI_HO_TONG") : "";
                //txtPPKTThuoc.Properties.NullValuePrompt = p ? GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_PHUONG_PHAP_SU_DUNG") : "";
                txtPhuongTienVanChuyen.Properties.NullValuePrompt = p ? GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_PHUONG_TIEN_VAN_CHUYEN") : "";
                txtTinhTrangNguoiBenh.Properties.NullValuePrompt = p ? GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_TINH_TRANG_NGUOI_BENH") : "";
                txtXetNghiem.Properties.NullValuePrompt = p ? GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_KET_QUA_XET_NGHIEM") : "";
                mmUsedMedicine.Properties.NullValuePrompt = p ? "Thuốc đã kê cho bệnh nhân" : "";
                mmPPDieuTri.Properties.NullValuePrompt = p ? "Phương pháp điều trị" : "";

                txtDauHieuLamSang.Properties.NullValuePromptShowForEmptyValue = p;
                txtHuongDieuTri.Properties.NullValuePromptShowForEmptyValue = p;
                txtNguoiHoTong.Properties.NullValuePromptShowForEmptyValue = p;
                txtPhuongTienVanChuyen.Properties.NullValuePromptShowForEmptyValue = p;
                txtTinhTrangNguoiBenh.Properties.NullValuePromptShowForEmptyValue = p;
                txtXetNghiem.Properties.NullValuePromptShowForEmptyValue = p;
                mmUsedMedicine.Properties.NullValuePromptShowForEmptyValue = p;
                mmPPDieuTri.Properties.NullValuePromptShowForEmptyValue = p;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCTranPati_Load(object sender, EventArgs e)
        {
            try
            {
                LoadKeysFromlanguage();
                ValidateForm();


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
                //layout
                this.lciDauHieuLamSang.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_DAU_HIEU_LAM_SANG");
                this.lciHuongDieuTri.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_HUONG_DIEU_TRI");
                this.lciKetQuaXetNghiem.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_KET_QUA_XET_NGHIEM");
                this.lciMediOrg.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_MEDI_ORG");
                //this.lciMediOrgAddress.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_MEDI_ORG_ADDRESS");
                this.lciNguoiHoTong.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_NGUOI_HO_TONG");
                //this.lciPhuongPhapSuDung.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_PHUONG_PHAP_SU_DUNG");
                this.lciPhuongTienVanChuyen.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_PHUONG_TIEN_VAN_CHUYEN");
                this.lciTinhTrangNguoiBenh.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_TINH_TRANG_NGUOI_BENH");
                this.lciTranPatiForm.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_TRAN_PATI_FORM");
                this.lciTranPatiReason.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_TRAN_PATI_REASON");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetStringFromKey(string key)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(key))
                {
                    result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceMessage.LanguageUCTranPati, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void ValidateForm()
        {
            try
            {
                ValidationTransferMediOrg();
                ValidationTranPatiReason();
                ValidationTranPatiForm();
                ValidationControlMaxLength(txtDauHieuLamSang, 3000);
                ValidationControlMaxLength(txtHuongDieuTri, 3000);
                ValidationControlMaxLength(txtNguoiHoTong, 200);
                ValidationControlMaxLength(txtTinhTrangNguoiBenh, 3000);
                ValidationControlMaxLength(txtPhuongTienVanChuyen, 3000);
                ValidationControlMaxLength(txtXetNghiem, 3000);
                ValidationControlMaxLength(mmUsedMedicine, 3000);
                ValidationControlMaxLength(mmPPDieuTri, 3000);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.ErrorText = Resources.ResourceMessage.TruongDuLieuVuotQuaKyTu;
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(control, validate);
        }

        private void ValidationTransferMediOrg()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validate = new GridLookupEditWithTextEditValidationRule();
                validate.cbo = this.cboMediOrgName;
                validate.txtTextEdit = this.txtMediOrgCode;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtMediOrgCode, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationTranPatiReason()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validate = new GridLookupEditWithTextEditValidationRule();
                validate.cbo = this.cboTranPatiReason;
                validate.txtTextEdit = this.txtTranPatiReason;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtTranPatiReason, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationTranPatiForm()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validate = new GridLookupEditWithTextEditValidationRule();
                validate.cbo = this.cboTranPatiForm;
                validate.txtTextEdit = this.txtTranPatiForm;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtTranPatiForm, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null) return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null) return;

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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataGridLookUpEdit(DevExpress.XtraEditors.GridLookUpEdit comboEdit, string code, string captionCode, string name, string captionName, string value, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(code, captionCode, 100, 1));
                columnInfos.Add(new ColumnInfo(name, captionName, 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(name, value, columnInfos, false, 300);
                ControlEditorLoader.Load(comboEdit, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillCurrentData(ADO.TranPatiDataSourcesADO input)
        {
            try
            {
                if (input != null)
                {
                    this.NextFocus = input.DelegateNextFocus;
                    this.HisMediOrgs = input.HisMediOrgs;
                    this.HisTranPatiForms = input.HisTranPatiForms;
                    this.HisTranPatiReasons = input.HisTranPatiReasons;
                    this.HisTranPatiTechs = input.HisTranPatiTechs;

                    LoadDataToCombo();

                    if (input.CurrentHisTreatment != null)
                    {
                        this.treatmentId = input.CurrentHisTreatment.ID;
                        MOS.SDO.HisTreatmentFinishSDO sdo = new MOS.SDO.HisTreatmentFinishSDO();
                        sdo.ClinicalNote = input.CurrentHisTreatment.CLINICAL_NOTE;
                        sdo.PatientCondition = input.CurrentHisTreatment.PATIENT_CONDITION;
                        sdo.SubclinicalResult = input.CurrentHisTreatment.SUBCLINICAL_RESULT;
                        sdo.TranPatiFormId = input.CurrentHisTreatment.TRAN_PATI_FORM_ID;
                        sdo.TranPatiTechId = input.CurrentHisTreatment.TRAN_PATI_TECH_ID;
                        sdo.TranPatiReasonId = input.CurrentHisTreatment.TRAN_PATI_REASON_ID;
                        sdo.TransferOutMediOrgCode = input.CurrentHisTreatment.MEDI_ORG_CODE;
                        sdo.TransferOutMediOrgName = input.CurrentHisTreatment.MEDI_ORG_NAME;
                        sdo.Transporter = input.CurrentHisTreatment.TRANSPORTER;
                        sdo.TransportVehicle = input.CurrentHisTreatment.TRANSPORT_VEHICLE;
                        sdo.TreatmentDirection = input.CurrentHisTreatment.TREATMENT_DIRECTION;
                        sdo.TreatmentMethod = input.CurrentHisTreatment.TREATMENT_METHOD;
                        FillDataToForm(sdo);
                    }

                    if (!String.IsNullOrEmpty(input.UsedMedicine))
                    {
                        mmUsedMedicine.Text = input.UsedMedicine;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToForm(MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO)
        {
            try
            {
                if (currentTreatmentFinishSDO != null)
                {
                    var mediOrgName = HisMediOrgs.FirstOrDefault(o => o.MEDI_ORG_CODE == currentTreatmentFinishSDO.TransferOutMediOrgCode);
                    if (mediOrgName != null)
                    {
                        //lblMediOrgAddress.Text = mediOrgName.ADDRESS;
                        txtMediOrgCode.Text = mediOrgName.MEDI_ORG_CODE;
                        cboMediOrgName.EditValue = mediOrgName.ID;
                        cboMediOrgName.Properties.Buttons[1].Visible = true;
                    }

                    var hisTranPatiReason = HisTranPatiReasons.FirstOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiReasonId);
                    if (hisTranPatiReason != null)
                    {
                        cboTranPatiReason.EditValue = hisTranPatiReason.ID;
                        cboTranPatiReason.Properties.Buttons[1].Visible = true;
                        txtTranPatiReason.Text = hisTranPatiReason.TRAN_PATI_REASON_CODE;
                    }

                    var hisTranPatiForm = HisTranPatiForms.FirstOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiFormId);
                    if (hisTranPatiForm != null)
                    {
                        cboTranPatiForm.EditValue = hisTranPatiForm.ID;
                        cboTranPatiForm.Properties.Buttons[1].Visible = true;
                        txtTranPatiForm.Text = hisTranPatiForm.TRAN_PATI_FORM_CODE;
                    }

                    var hisTranPatiTech = HisTranPatiTechs.FirstOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiTechId);
                    if (hisTranPatiTech != null)
                    {
                        cboTranPatiTech.EditValue = hisTranPatiTech.ID;
                        cboTranPatiTech.Properties.Buttons[1].Visible = true;
                        txtTranPatiTech.Text = hisTranPatiTech.TRAN_PATI_TECH_CODE;
                    }

                    txtDauHieuLamSang.Text = currentTreatmentFinishSDO.ClinicalNote;
                    txtXetNghiem.Text = currentTreatmentFinishSDO.SubclinicalResult;
                    txtTinhTrangNguoiBenh.Text = currentTreatmentFinishSDO.PatientCondition;
                    txtPhuongTienVanChuyen.Text = currentTreatmentFinishSDO.TransportVehicle;
                    txtNguoiHoTong.Text = currentTreatmentFinishSDO.Transporter;
                    //txtPPKTThuoc.Text = currentTreatmentFinishSDO.TreatmentMethod;
                    txtHuongDieuTri.Text = currentTreatmentFinishSDO.TreatmentDirection;
                    mmPPDieuTri.Text = currentTreatmentFinishSDO.TreatmentMethod;
                    LoadExpMestMedicine(this.treatmentId);
                }
                else
                {
                    //lblMediOrgAddress.Text = "";
                    txtMediOrgCode.Text = "";
                    cboMediOrgName.EditValue = null;
                    cboMediOrgName.Properties.Buttons[1].Visible = false;
                    cboTranPatiReason.EditValue = null;
                    cboTranPatiReason.Properties.Buttons[1].Visible = false;
                    txtTranPatiReason.Text = "";
                    cboTranPatiForm.EditValue = null;
                    cboTranPatiForm.Properties.Buttons[1].Visible = false;
                    cboTranPatiTech.EditValue = null;
                    txtTranPatiTech.Text = "";
                    txtTranPatiForm.Text = "";
                    txtDauHieuLamSang.Text = "";
                    txtXetNghiem.Text = "";
                    txtTinhTrangNguoiBenh.Text = "";
                    txtPhuongTienVanChuyen.Text = "";
                    txtNguoiHoTong.Text = "";
                    //txtPPKTThuoc.Text = "";
                    txtHuongDieuTri.Text = "";
                    mmUsedMedicine.Text = "";
                    mmPPDieuTri.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void LoadExpMestMedicine(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter filter = new HisExpMestMedicineViewFilter();
                filter.TDL_TREATMENT_ID = treatmentId;
                filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, filter, param);
                mmUsedMedicine.Text = "";
                if (listExpMestMedicine != null && listExpMestMedicine.Count > 0)
                {
                    var listExpMestMedicineGroups = listExpMestMedicine.GroupBy(o => o.MEDICINE_TYPE_ID);
                    foreach (var listExpMestMedicineGroup in listExpMestMedicineGroups)
                    {
                        string conentra = !String.IsNullOrEmpty(listExpMestMedicineGroup.First().CONCENTRA) ? " (" + listExpMestMedicineGroup.First().CONCENTRA + ")" : "";
                        mmUsedMedicine.Text += String.Format("{0}{1};", listExpMestMedicineGroup.First().MEDICINE_TYPE_NAME, conentra);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataToCombo()
        {
            try
            {
                string ma = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_MA");
                string ten = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_TEN");

                DataToComboBenhVien(HisMediOrgs, cboMediOrgName);

                LoadDataGridLookUpEdit(cboTranPatiReason,
                    "TRAN_PATI_REASON_CODE", ma, "TRAN_PATI_REASON_NAME", ten, "ID", HisTranPatiReasons);
                LoadDataGridLookUpEdit(cboTranPatiForm,
                    "TRAN_PATI_FORM_CODE", ma, "TRAN_PATI_FORM_NAME", ten, "ID", HisTranPatiForms);
                LoadDataGridLookUpEdit(cboTranPatiTech,
                    "TRAN_PATI_TECH_CODE", ma, "TRAN_PATI_TECH_NAME", ten, "ID", HisTranPatiTechs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        private void DataToComboBenhVien(List<HIS_MEDI_ORG> dataMediOrg, CustomGridLookUpEditWithFilterMultiColumn cbo)
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
                //columnInfos.Add(new ColumnInfo("ICD_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ID", columnInfos, false, 250);
                //ControlEditorLoader.Load(cbo, dataIcds, controlEditorADO);

                listMediOrg = new List<MediOrgADO>();
                var listMediOrgTemp = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>().OrderBy(o => o.MEDI_ORG_CODE).ToList();
                if (listMediOrgTemp != null && listMediOrgTemp.Count > 0)
                {
                    foreach (var item in listMediOrgTemp)
                    {
                        MediOrgADO mediOrgADO = new MediOrgADO(item);
                        Inventec.Common.Mapper.DataObjectMapper.Map<MediOrgADO>(mediOrgADO, item);
                        mediOrgADO.MEDI_ORG_NAME_UNSIGN = convertToUnSign3(item.MEDI_ORG_NAME);
                        listMediOrg.Add(mediOrgADO);
                    }
                }

                cbo.Properties.DataSource = listMediOrg;
                cbo.Properties.DisplayMember = "MEDI_ORG_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = new GridColumn();

                aColumnCode = cbo.Properties.View.Columns.AddField("MEDI_ORG_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("MEDI_ORG_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("MEDI_ORG_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["MEDI_ORG_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Internal
        internal void Reload(ADO.TranPatiDataSourcesADO input)
        {
            try
            {
                if (input != null)
                {
                    FillCurrentData(input);
                }
                else
                {
                    FillDataToForm(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void FocusControl()
        {
            try
            {
                txtMediOrgCode.Focus();
                txtMediOrgCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ReadOnly(bool isReadOnly)
        {
            try
            {
                txtMediOrgCode.ReadOnly = isReadOnly;
                cboMediOrgName.ReadOnly = isReadOnly;
                cboTranPatiReason.ReadOnly = isReadOnly;
                txtTranPatiReason.ReadOnly = isReadOnly;
                cboTranPatiForm.ReadOnly = isReadOnly;
                txtTranPatiForm.ReadOnly = isReadOnly;
                cboTranPatiTech.ReadOnly = isReadOnly;
                txtTranPatiTech.ReadOnly = isReadOnly;
                txtDauHieuLamSang.ReadOnly = isReadOnly;
                txtXetNghiem.ReadOnly = isReadOnly;
                txtTinhTrangNguoiBenh.ReadOnly = isReadOnly;
                txtPhuongTienVanChuyen.ReadOnly = isReadOnly;
                txtNguoiHoTong.ReadOnly = isReadOnly;
                //txtPPKTThuoc.ReadOnly = isReadOnly;
                txtHuongDieuTri.ReadOnly = isReadOnly;
                mmPPDieuTri.ReadOnly = isReadOnly;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValue(object data)
        {
            try
            {
                if (data is ADO.TranPatiDataSourcesADO)
                {
                    FillCurrentData((ADO.TranPatiDataSourcesADO)data);
                }
                else
                {
                    FillDataToForm(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal object GetValue()
        {
            object result = null;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate()) return null;

                MOS.SDO.HisTreatmentFinishSDO outPut = new MOS.SDO.HisTreatmentFinishSDO();

                outPut.TreatmentId = treatmentId;

                if (cboTranPatiReason.EditValue != null)
                {
                    outPut.TranPatiReasonId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString());
                }
                else
                    outPut.TranPatiReasonId = null;

                if (cboTranPatiForm.EditValue != null)
                {
                    outPut.TranPatiFormId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString());
                }
                else
                    outPut.TranPatiFormId = null;

                if (HisMediOrgs != null)
                {
                    var data = HisMediOrgs.FirstOrDefault(o => o.MEDI_ORG_CODE == txtMediOrgCode.Text);
                    if (data != null)
                    {
                        outPut.TransferOutMediOrgCode = data.MEDI_ORG_CODE;
                        outPut.TransferOutMediOrgName = data.MEDI_ORG_NAME;
                    }
                }

                if (cboTranPatiTech.EditValue != null)
                {
                    outPut.TranPatiTechId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiTech.EditValue.ToString());
                }
                else
                    outPut.TranPatiTechId = null;

                outPut.ClinicalNote = txtDauHieuLamSang.Text;
                outPut.SubclinicalResult = txtXetNghiem.Text;
                outPut.PatientCondition = txtTinhTrangNguoiBenh.Text;
                outPut.TransportVehicle = txtPhuongTienVanChuyen.Text;
                outPut.Transporter = txtNguoiHoTong.Text;
                outPut.UsedMedicine = mmUsedMedicine.Text;
                outPut.TreatmentMethod = mmPPDieuTri.Text;
                outPut.TreatmentDirection = txtHuongDieuTri.Text;
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
        #endregion

        #region Event
        private void txtMediOrgCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (HisMediOrgs != null && HisMediOrgs.Count > 0)
                    {
                        bool showCbo = true;
                        if (!String.IsNullOrEmpty(txtMediOrgCode.Text.Trim()))
                        {
                            string code = txtMediOrgCode.Text.Trim();
                            var listData = HisMediOrgs.Where(o => o.MEDI_ORG_CODE.Contains(code)).ToList();
                            var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.MEDI_ORG_CODE == code).ToList() : listData) : null;
                            if (result != null && result.Count > 0)
                            {
                                showCbo = false;
                                txtMediOrgCode.Text = result.First().MEDI_ORG_CODE;
                                cboMediOrgName.EditValue = result.First().ID;
                                cboMediOrgName.Properties.Buttons[1].Visible = true;
                                //lblMediOrgAddress.Text = result.First().ADDRESS;
                                ProcessLevelOfMediOrg();
                                txtTranPatiReason.Focus();
                                txtTranPatiReason.SelectAll();
                            }
                        }
                        if (showCbo)
                        {
                            cboMediOrgName.Focus();
                            cboMediOrgName.ShowPopup();
                        }
                    }
                    else
                    {
                        txtTranPatiReason.Focus();
                        txtTranPatiReason.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediOrgName_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtMediOrgCode.Text = "";
                    cboMediOrgName.EditValue = null;
                    cboMediOrgName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediOrgName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboMediOrgName.EditValue != null && HisMediOrgs != null)
                    {
                        var data = HisMediOrgs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediOrgName.EditValue.ToString()));
                        if (data != null)
                        {
                            txtMediOrgCode.Text = data.MEDI_ORG_CODE;
                            cboMediOrgName.Properties.Buttons[1].Visible = true;
                            //lblMediOrgAddress.Text = data.ADDRESS;
                            ProcessLevelOfMediOrg();
                            txtTranPatiReason.Focus();
                            txtTranPatiReason.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediOrgName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMediOrgName.EditValue != null && HisMediOrgs != null)
                    {
                        var data = HisMediOrgs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediOrgName.EditValue.ToString()));
                        if (data != null)
                        {
                            txtMediOrgCode.Text = data.MEDI_ORG_CODE;
                            cboMediOrgName.Properties.Buttons[1].Visible = true;
                            //lblMediOrgAddress.Text = data.ADDRESS;
                            ProcessLevelOfMediOrg();
                            txtTranPatiReason.Focus();
                            txtTranPatiReason.SelectAll();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboMediOrgName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Khi nhập thông tin chuyển tuyến, căn cứ vào tuyến của viện mà người dùng đang làm việc (LEVEL_CODE của HIS_BRANCH mà người dùng chọn làm việc) với tuyến của viện mà người dùng nhập "Nơi chuyển đến" để tự động điền "Hình thức chuyển" (LEVEL_CODE của HIS_MEDI_ORG), theo công thức sau:

        ///                - Nếu L2 - L1 = 1 --> chọn "Hình thức chuyển" mã "01" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE)
        ///                - Nếu L2 - L1 > 1 --> chọn "Hình thức chuyển" mã "02" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE)
        ///                - Nếu L2 - L1 < 0 --> chọn "Hình thức chuyển" mã "03" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG)
        ///                - Nếu L2 - L1 = 0 --> chọn "Hình thức chuyển" mã "04" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN)

        ///                Trong đó:
        ///                - LEVEL_CODE của "Tuyến của viện mà người dùng đang làm việc" là L1
        ///                - LEVEL_CODE của "Nơi chuyển đến" là L2

        ///                Lưu ý:
        ///                Hệ thống cũ, dữ liệu LEVEL_CODE của HIS_MEDI_ORG đang lưu dưới dạng text (TW, T, H, X), để tránh việc update cache có thể ảnh hưởng đến hiệu năng, lúc xử lý cần "if-else" để xử lý được với dữ liệu cũ, cụ thể cần check LEVEL_CODE của HIS_MEDI_ORG, gán lại giá trị trước khi tính toán:
        ///                - Nếu LEVEL_CODE = TW --> LEVEL_CODE = 1
        ///                - Nếu LEVEL_CODE = T --> LEVEL_CODE = 2
        ///                - Nếu LEVEL_CODE = H --> LEVEL_CODE = 3
        ///                - Nếu LEVEL_CODE = X --> LEVEL_CODE = 4
        ///                - Khác: --> giữ nguyên giá trị
        /// </summary>
        private void ProcessLevelOfMediOrg()
        {
            try
            {
                string lvBranch = FixWrongLevelCode(BranchDataWorker.Branch.HEIN_LEVEL_CODE);

                if (!String.IsNullOrEmpty(txtMediOrgCode.Text) && cboMediOrgName.EditValue != null)
                {
                    var mediTrans = listMediOrg.Where(o => o.MEDI_ORG_CODE == txtMediOrgCode.Text).FirstOrDefault();
                    if (mediTrans != null)
                    {
                        string lvTrans = FixWrongLevelCode(mediTrans.LEVEL_CODE);

                        int iLvBranch = int.Parse(lvBranch);
                        int iLvTrans = int.Parse(lvTrans);
                        int iKq = iLvBranch - iLvTrans;
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM tranPatiDefault = null;
                        if (iKq == 1)
                        {
                            tranPatiDefault = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>().Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE).FirstOrDefault();
                        }
                        else if (iKq > 1)
                        {
                            tranPatiDefault = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>().Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE).FirstOrDefault();
                        }
                        else if (iKq < 0)
                        {
                            tranPatiDefault = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>().Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG).FirstOrDefault();
                        }
                        else if (iKq == 0)
                        {
                            tranPatiDefault = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>().Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN).FirstOrDefault();
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Branch.HEIN_LEVEL_CODE", BranchDataWorker.Branch.HEIN_LEVEL_CODE) + Inventec.Common.Logging.LogUtil.TraceData("mediTrans.LEVEL_CODE", mediTrans.LEVEL_CODE) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tranPatiDefault), tranPatiDefault));

                        cboTranPatiForm.EditValue = tranPatiDefault != null ? (long?)tranPatiDefault.ID : null;
                        txtTranPatiForm.Text = tranPatiDefault != null ? tranPatiDefault.TRAN_PATI_FORM_CODE : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string FixWrongLevelCode(string code)
        {
            string rs = "";
            try
            {
                if (code == "TW")
                {
                    rs = "1";
                }
                else if (code == "T")
                {
                    rs = "2";
                }
                else if (code == "H")
                {
                    rs = "3";
                }
                else if (code == "X")
                {
                    rs = "4";
                }
                else
                    rs = code;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private void txtTranPatiReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (HisTranPatiReasons != null)
                    {
                        bool showCbo = true;
                        if (!String.IsNullOrEmpty(txtTranPatiReason.Text.Trim()))
                        {
                            string code = txtTranPatiReason.Text.Trim();
                            var listData = HisTranPatiReasons.Where(o => o.TRAN_PATI_REASON_CODE.Contains(code)).ToList();
                            var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TRAN_PATI_REASON_CODE == code).ToList() : listData) : null;
                            if (result != null && result.Count > 0)
                            {
                                showCbo = false;
                                txtTranPatiReason.Text = result.First().TRAN_PATI_REASON_CODE;
                                cboTranPatiReason.EditValue = result.First().ID;
                                cboTranPatiReason.Properties.Buttons[1].Visible = true;
                                txtTranPatiForm.Focus();
                                txtTranPatiForm.SelectAll();
                            }
                        }
                        if (showCbo)
                        {
                            cboTranPatiReason.Focus();
                            cboTranPatiReason.ShowPopup();
                        }
                    }
                    else
                    {
                        txtTranPatiForm.Focus();
                        txtTranPatiForm.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiReason_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtTranPatiReason.Text = "";
                    cboTranPatiReason.EditValue = null;
                    cboTranPatiReason.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiReason_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboTranPatiReason.EditValue != null && HisTranPatiReasons != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = HisTranPatiReasons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiReason.Text = data.TRAN_PATI_REASON_CODE;
                            cboTranPatiReason.Properties.Buttons[1].Visible = true;
                            txtTranPatiForm.Focus();
                            txtTranPatiForm.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTranPatiReason.EditValue != null && HisTranPatiReasons != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = HisTranPatiReasons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiReason.Text = data.TRAN_PATI_REASON_CODE;
                            cboTranPatiReason.Properties.Buttons[1].Visible = true;
                            txtTranPatiForm.Focus();
                            txtTranPatiForm.SelectAll();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboTranPatiReason.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTranPatiForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (HisTranPatiForms != null)
                    {
                        bool showCbo = true;
                        if (!String.IsNullOrEmpty(txtTranPatiForm.Text.Trim()))
                        {
                            string code = txtTranPatiForm.Text.Trim();
                            var listData = HisTranPatiForms.Where(o => o.TRAN_PATI_FORM_CODE.Contains(code)).ToList();
                            var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TRAN_PATI_FORM_CODE == code).ToList() : listData) : null;
                            if (result != null && result.Count > 0)
                            {
                                showCbo = false;
                                txtTranPatiForm.Text = result.First().TRAN_PATI_FORM_CODE;
                                cboTranPatiForm.EditValue = result.First().ID;
                                cboTranPatiForm.Properties.Buttons[1].Visible = true;
                                cboTranPatiTech.Focus();
                                cboTranPatiTech.SelectAll();
                            }
                        }
                        if (showCbo)
                        {
                            cboTranPatiForm.Focus();
                            cboTranPatiForm.ShowPopup();
                        }
                    }
                    else
                    {
                        txtTranPatiTech.Focus();
                        txtTranPatiTech.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiForm_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtTranPatiForm.Text = "";
                    cboTranPatiForm.EditValue = null;
                    cboTranPatiForm.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboTranPatiForm.EditValue != null && HisTranPatiForms != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = HisTranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiForm.Text = data.TRAN_PATI_FORM_CODE;
                            cboTranPatiForm.Properties.Buttons[1].Visible = true;
                            txtTranPatiTech.Focus();
                            txtTranPatiTech.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTranPatiForm.EditValue != null && HisTranPatiForms != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = HisTranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiForm.Text = data.TRAN_PATI_FORM_CODE;
                            cboTranPatiForm.Properties.Buttons[1].Visible = true;
                            txtTranPatiTech.Focus();
                            txtTranPatiTech.SelectAll();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboTranPatiForm.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNguoiHoTong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    if (this.NextFocus != null)
                    {
                        this.NextFocus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiTech_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTranPatiTech.EditValue != null && HisTranPatiForms != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH data = HisTranPatiTechs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiTech.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiTech.Text = data.TRAN_PATI_TECH_CODE;
                            cboTranPatiTech.Properties.Buttons[1].Visible = true;
                            txtDauHieuLamSang.Focus();
                            txtDauHieuLamSang.SelectAll();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboTranPatiTech.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiTech_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboTranPatiTech.EditValue != null && HisTranPatiForms != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH data = HisTranPatiTechs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiTech.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiTech.Text = data.TRAN_PATI_TECH_CODE;
                            cboTranPatiTech.Properties.Buttons[1].Visible = true;
                            txtDauHieuLamSang.Focus();
                            txtDauHieuLamSang.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTranPatiTech.EditValue = null;
                    txtTranPatiTech.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTranPatiTech_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTranPatiTech.EditValue != null)
                {
                    cboTranPatiTech.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboTranPatiTech.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTranPatiTech_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (HisTranPatiTechs != null)
                    {
                        bool showCbo = true;
                        if (!String.IsNullOrEmpty(txtTranPatiTech.Text.Trim()))
                        {
                            string code = txtTranPatiTech.Text.Trim();
                            var listData = HisTranPatiTechs.Where(o => o.TRAN_PATI_TECH_CODE.Contains(code)).ToList();
                            var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TRAN_PATI_TECH_CODE == code).ToList() : listData) : null;
                            if (result != null && result.Count > 0)
                            {
                                showCbo = false;
                                txtTranPatiTech.Text = result.First().TRAN_PATI_TECH_CODE;
                                cboTranPatiTech.EditValue = result.First().ID;
                                cboTranPatiTech.Properties.Buttons[1].Visible = true;
                                cboTranPatiTech.Focus();
                                cboTranPatiTech.SelectAll();
                            }
                        }
                        if (showCbo)
                        {
                            cboTranPatiTech.Focus();
                            cboTranPatiTech.ShowPopup();
                        }
                    }
                    else
                    {
                        txtDauHieuLamSang.Focus();
                        txtDauHieuLamSang.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion


    }
}
