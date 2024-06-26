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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.TreatmentFinish.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.CustomControl.CustomGrid;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TreatmentFinish.CloseTreatment
{
    public partial class FormTransfer : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private int positionHandle = -1;
        private MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment { get; set; }
        private MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO { get; set; }
        private TreatmentEndInputADO treatmentEndInputADO;
        internal delegate void GetString(MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO);
        internal GetString MyGetData;

        List<MediOrgADO> listMediOrg { get; set; }
        private List<HIS_TRAN_PATI_TEMP> listDataTranPatiTemp { get; set; }
        public List<AcsUserADO> lstReAcsUserADO { get; private set; }

        private List<HIS_TRAN_PATI_FORM> HisTranPatiForms;
        private List<HIS_TRAN_PATI_REASON> HisTranPatiReasons;
        private List<HIS_TRAN_PATI_TECH> HisTranPatiTechs;

        internal const string ModuleLink_HisTranPatiTemp = "HIS.Desktop.Plugins.HisTranPatiTemp";
        List<V_HIS_EMPLOYEE> selected = new List<V_HIS_EMPLOYEE>();
        #endregion

        #region Construct
        public FormTransfer()
        {
            InitializeComponent();
        }

        public FormTransfer(TreatmentEndInputADO _treatmentEndInputADO)
            : this()
        {
            try
            {
                if (_treatmentEndInputADO != null)
                {
                    this.treatmentEndInputADO = _treatmentEndInputADO;
                    this.hisTreatment = this.treatmentEndInputADO.Treatment;               
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormTransfer_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                InitComboTransporterLoginNameCheck();
                LoadKeysFromlanguage();
                LoadDataToCombo();
                LoadDataTocboUser();
                SetDefaultValueControl();
                loadDataTranPatiOld(this.treatmentEndInputADO.Treatment);//Lấy thông tin chuyển viện cũ
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void loadDataTranPatiOld(MOS.EFMODEL.DataModels.HIS_TREATMENT treatment)
        {
            try
            {
                if (treatment != null)
                {
                    if (!String.IsNullOrEmpty(treatment.MEDI_ORG_CODE))
                    {
                        var mediOrgName = listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == treatment.MEDI_ORG_CODE);
                        if (mediOrgName != null)
                        {
                            cboMediOrgName.EditValue = mediOrgName.MEDI_ORG_CODE;
                            txtMediOrgCode.Text = mediOrgName.MEDI_ORG_CODE;
                            lblMediOrgAddress.Text = mediOrgName.ADDRESS;
                        }
                    }
                    if (treatment.TRAN_PATI_FORM_ID.HasValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM hisTranPatiForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == treatment.TRAN_PATI_FORM_ID);
                        if (hisTranPatiForm != null)
                        {
                            cboTranPatiForm.EditValue = hisTranPatiForm.ID;
                            txtTranPatiForm.Text = hisTranPatiForm.TRAN_PATI_FORM_CODE;
                        }
                    }

                    if (treatment.TRAN_PATI_REASON_ID.HasValue)
                    {
                        var tranPatiReason = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == treatment.TRAN_PATI_REASON_ID);
                        if (tranPatiReason != null)
                        {
                            cboTranPatiReason.EditValue = tranPatiReason.ID;
                            txtTranPatiReason.Text = tranPatiReason.TRAN_PATI_REASON_CODE;
                        }
                    }
                    txtDauHieuLamSang.Text = treatment.CLINICAL_NOTE;
                    txtXetNghiem.Text = treatment.SUBCLINICAL_RESULT;
                    txtTinhTrangNguoiBenh.Text = treatment.PATIENT_CONDITION;
                    txtPhuongTienVanChuyen.Text = treatment.TRANSPORT_VEHICLE;
                    if (treatment != null && !string.IsNullOrEmpty(treatment.TRANSPORTER_LOGINNAMES))
                    {
                        var oldSelecteds = treatment.TRANSPORTER_LOGINNAMES.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                            if (oldSelecteds != null && oldSelecteds.Count > 0)
                            {
                                selected = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => oldSelecteds.Contains(o.LOGINNAME)).ToList();
                                gridCheckMark.SelectAll(selected);

                                string displayText = String.Join(", ", selected.Select(s => s.TDL_USERNAME).ToList());
                                cboTransporterLoginName.Text = displayText;
                            }
                        }
                    }
                    txtPPKTThuoc.Text = treatment.TREATMENT_METHOD;
                    txtHuongDieuTri.Text = treatment.TREATMENT_DIRECTION;
                }
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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
                Resources.ResourceLanguageManager.LanguageFormTransfer = new ResourceManager("HIS.UC.TreatmentFinish.Resources.Lang", typeof(FormTransfer).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormTransfer.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.cboTranPatiTech.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.cboTranPatiTech.Properties.NullText", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormTransfer.bar1.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("FormTransfer.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormTransfer.btnSave.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.cboTranPatiForm.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.cboTranPatiForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.cboTranPatiReason.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.cboTranPatiReason.Properties.NullText", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.cboMediOrgName.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.cboMediOrgName.Properties.NullText", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.lciMediOrg.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciMediOrg.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.lciTranPatiReason.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciTranPatiReason.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.lciKetQuaXetNghiem.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciKetQuaXetNghiem.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.lciPhuongTienVanChuyen.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciPhuongTienVanChuyen.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.lciMediOrgAddress.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciMediOrgAddress.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.lciTranPatiForm.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciTranPatiForm.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.lciDauHieuLamSang.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciDauHieuLamSang.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.lciHuongDieuTri.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciHuongDieuTri.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.lciTinhTrangNguoiBenh.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciTinhTrangNguoiBenh.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.lciPhuongPhapSuDung.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciPhuongPhapSuDung.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FormTransfer.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.lciTranPatiTemp.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciTranPatiTemp.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.cboTranPatiTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.cboTranPatiTemp.Properties.NullText", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.btnSaveTranPatiTemp.Text = Inventec.Common.Resource.Get.Value("FormTransfer.btnSaveTranPatiTemp.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormTransfer.Text", Resources.ResourceLanguageManager.LanguageFormTransfer, LanguageManager.GetCulture());
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

        private void LoadDataToCombo()
        {
            try
            {
                string ma = CommonBaseEditor.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_MA", Resources.ResourceLanguageManager.LanguageFormTransfer);
                string ten = CommonBaseEditor.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_TEN", Resources.ResourceLanguageManager.LanguageFormTransfer);

                listMediOrg = new List<MediOrgADO>();
                var listMediOrgTemp = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>().OrderBy(o => o.MEDI_ORG_CODE).ToList();
                if (listMediOrgTemp != null && listMediOrgTemp.Count > 0)
                {
                    foreach (var item in listMediOrgTemp)
                    {
                        MediOrgADO mediOrgADO = new MediOrgADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MediOrgADO>(mediOrgADO, item);
                        mediOrgADO.MEDI_ORG_NAME__UNSIGN = convertToUnSign3(item.MEDI_ORG_NAME);
                        listMediOrg.Add(mediOrgADO);
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_ORG_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_ORG_NAME", "Tên", 200, 2));
                columnInfos.Add(new ColumnInfo("MEDI_ORG_NAME__UNSIGN", "Tên", 200, -1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_ORG_NAME", "MEDI_ORG_CODE", columnInfos, false, 300);
                ControlEditorLoader.Load(cboMediOrgName, listMediOrg, controlEditorADO);

                this.HisTranPatiForms = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>();
                this.HisTranPatiReasons = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>();
                this.HisTranPatiTechs = BackendDataWorker.Get<HIS_TRAN_PATI_TECH>();

                CommonBaseEditor.LoadDataGridLookUpEdit(cboTranPatiReason,
                    "TRAN_PATI_REASON_CODE", ma, "TRAN_PATI_REASON_NAME", ten, "ID", this.HisTranPatiReasons);
                CommonBaseEditor.LoadDataGridLookUpEdit(cboTranPatiForm,
                    "TRAN_PATI_FORM_CODE", ma, "TRAN_PATI_FORM_NAME", ten, "ID", this.HisTranPatiForms);
                CommonBaseEditor.LoadDataGridLookUpEdit(cboTranPatiTech,
                   "TRAN_PATI_TECH_CODE", ma, "TRAN_PATI_TECH_NAME", ten, "ID", this.HisTranPatiTechs);
                
                LoadDataToComboTranPatiTemp(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboTranPatiTemp(bool isGetData)
        {
            try
            {
                if (isGetData || listDataTranPatiTemp == null || listDataTranPatiTemp.Count == 0)
                {
                    listDataTranPatiTemp = GetList_HIS_TRAN_PATI_TEMP().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                                                                && o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE
                                                                                && (o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || o.IS_PUBLIC == 1)).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TRAN_PATI_TEMP_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("TRAN_PATI_TEMP_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TRAN_PATI_TEMP_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboTranPatiTemp, listDataTranPatiTemp, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_TRAN_PATI_TEMP> GetList_HIS_TRAN_PATI_TEMP()
        {
            List<HIS_TRAN_PATI_TEMP> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTranPatiTempFilter filter = new HisTranPatiTempFilter();
                result = new BackendAdapter(param).Get<List<HIS_TRAN_PATI_TEMP>>("api/HisTranPatiTemp/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetDefaultValueControl()
        {
            try
            {
                currentTreatmentFinishSDO = this.treatmentEndInputADO.HisTreatmentFinishSDO;
                if (currentTreatmentFinishSDO != null)
                {
                    var mediOrgName = listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == currentTreatmentFinishSDO.TransferOutMediOrgCode);
                    if (mediOrgName != null)
                    {
                        cboMediOrgName.EditValue = mediOrgName.MEDI_ORG_CODE;
                        txtMediOrgCode.Text = mediOrgName.MEDI_ORG_CODE;
                        lblMediOrgAddress.Text = mediOrgName.ADDRESS;
                    }

                    var hisTranPatiReason = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiReasonId);
                    if (hisTranPatiReason != null)
                    {
                        cboTranPatiReason.EditValue = hisTranPatiReason.ID;
                        txtTranPatiReason.Text = hisTranPatiReason.TRAN_PATI_REASON_CODE;
                    }

                    var hisTranPatiForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>().SingleOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiFormId);
                    if (hisTranPatiForm != null)
                    {
                        cboTranPatiForm.EditValue = hisTranPatiForm.ID;
                        txtTranPatiForm.Text = hisTranPatiForm.TRAN_PATI_FORM_CODE;
                    }

                    var tranPatiTech = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH>().SingleOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiTechId);
                    if (tranPatiTech != null)
                    {
                        cboTranPatiTech.EditValue = tranPatiTech.ID;
                        txtTranPatiTechCode.Text = tranPatiTech.TRAN_PATI_TECH_CODE;
                    }

                    txtDauHieuLamSang.Text = currentTreatmentFinishSDO.ClinicalNote;
                    txtXetNghiem.Text = currentTreatmentFinishSDO.SubclinicalResult;
                    txtTinhTrangNguoiBenh.Text = currentTreatmentFinishSDO.PatientCondition;
                    txtPhuongTienVanChuyen.Text = currentTreatmentFinishSDO.TransportVehicle;
                    if (currentTreatmentFinishSDO != null && !string.IsNullOrEmpty(currentTreatmentFinishSDO.TransporterLoginnames))
                    {
                        var oldSelecteds = currentTreatmentFinishSDO.TransporterLoginnames.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                            if (oldSelecteds != null && oldSelecteds.Count > 0)
                            {
                                selected = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => oldSelecteds.Contains(o.LOGINNAME)).ToList();
                                gridCheckMark.SelectAll(selected);

                                string displayText = String.Join(", ", selected.Select(s => s.TDL_USERNAME).ToList());
                                cboTransporterLoginName.Text = displayText;
                            }
                        }
                    }
                    txtPPKTThuoc.Text = currentTreatmentFinishSDO.TreatmentMethod;
                    txtHuongDieuTri.Text = currentTreatmentFinishSDO.TreatmentDirection;
                }
                txtMediOrgCode.Focus();
                txtMediOrgCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Validation
        private void ValidateForm()
        {
            try
            {
                CommonBaseEditor.ValidateGridLookupWithTextEdit(this.cboMediOrgName, this.txtMediOrgCode, this.dxValidationProviderControl);
                CommonBaseEditor.ValidateGridLookupWithTextEdit(this.cboTranPatiReason, this.txtTranPatiReason, this.dxValidationProviderControl);
                CommonBaseEditor.ValidateGridLookupWithTextEdit(this.cboTranPatiForm, this.txtTranPatiForm, this.dxValidationProviderControl);

                ValidationControlMaxLength(txtDauHieuLamSang, 500);
                ValidationControlMaxLength(txtHuongDieuTri, 200);
                //ValidationControlMaxLength(txtNguoiHoTong, 50);
                ValidationControlMaxLength(txtTinhTrangNguoiBenh, 3000);
                ValidationControlMaxLength(txtPhuongTienVanChuyen, 200);
                ValidationControlMaxLength(txtXetNghiem, 500);
                ValidationControlMaxLength(txtPPKTThuoc, 200);
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
            this.dxValidationProviderControl.SetValidationRule(control, validate);
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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
        #endregion

        #region Event
        private void txtMediOrgCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtMediOrgCode.Text.Trim()))
                    {
                        string code = txtMediOrgCode.Text.Trim();
                        var listData = listMediOrg.Where(o => o.MEDI_ORG_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.MEDI_ORG_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtMediOrgCode.Text = result.First().MEDI_ORG_CODE;
                            cboMediOrgName.EditValue = result.First().MEDI_ORG_CODE;
                            lblMediOrgAddress.Text = result.First().ADDRESS;
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
                    if (cboMediOrgName.EditValue != null)
                    {
                        var data = listMediOrg.SingleOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgName.EditValue.ToString());
                        if (data != null)
                        {
                            txtMediOrgCode.Text = data.MEDI_ORG_CODE;
                            lblMediOrgAddress.Text = data.ADDRESS;
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
                    if (cboMediOrgName.EditValue != null)
                    {
                        var data = listMediOrg.SingleOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgName.EditValue.ToString());
                        if (data != null)
                        {
                            txtMediOrgCode.Text = data.MEDI_ORG_CODE;
                            lblMediOrgAddress.Text = data.ADDRESS;
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
        ///                 Sửa chức năng "Tiếp đón" (tiếp đón 1 và tiếp đón 2):
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
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtTranPatiReason.Text.Trim()))
                    {
                        string code = txtTranPatiReason.Text.Trim();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>().Where(o => o.TRAN_PATI_REASON_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TRAN_PATI_REASON_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtTranPatiReason.Text = result.First().TRAN_PATI_REASON_CODE;
                            cboTranPatiReason.EditValue = result.First().ID;
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
                    if (cboTranPatiReason.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiReason.Text = data.TRAN_PATI_REASON_CODE;
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
                    if (cboTranPatiReason.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiReason.Text = data.TRAN_PATI_REASON_CODE;
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
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtTranPatiForm.Text.Trim()))
                    {
                        string code = txtTranPatiForm.Text.Trim();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>().Where(o => o.TRAN_PATI_FORM_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TRAN_PATI_FORM_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtTranPatiForm.Text = result.First().TRAN_PATI_FORM_CODE;
                            cboTranPatiForm.EditValue = result.First().ID;
                            txtTranPatiTechCode.Focus();
                            txtTranPatiTechCode.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboTranPatiForm.Focus();
                        cboTranPatiForm.ShowPopup();
                    }
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
                    if (cboTranPatiForm.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiForm.Text = data.TRAN_PATI_FORM_CODE;
                            txtTranPatiTechCode.Focus();
                            txtTranPatiTechCode.SelectAll();
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
                if (e.KeyCode == Keys.Down)
                {
                    cboTranPatiForm.ShowPopup();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (cboTranPatiForm.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiForm.Text = data.TRAN_PATI_FORM_CODE;
                            txtTranPatiTechCode.Focus();
                            txtTranPatiTechCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProviderControl.Validate()) return;

                currentTreatmentFinishSDO.TreatmentId = hisTreatment.ID;

                if (cboTranPatiReason.EditValue != null)
                {
                    currentTreatmentFinishSDO.TranPatiReasonId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString());
                }
                else
                    currentTreatmentFinishSDO.TranPatiReasonId = null;

                if (cboTranPatiForm.EditValue != null)
                {
                    currentTreatmentFinishSDO.TranPatiFormId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString());
                }
                else
                    currentTreatmentFinishSDO.TranPatiFormId = null;

                if (cboTranPatiTech.EditValue != null)
                {
                    currentTreatmentFinishSDO.TranPatiTechId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiTech.EditValue.ToString());
                }
                else
                    currentTreatmentFinishSDO.TranPatiTechId = null;

                if (cboMediOrgName.EditValue != null)
                {
                    var data = listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgName.EditValue.ToString());
                    if (data != null)
                    {
                        currentTreatmentFinishSDO.TransferOutMediOrgCode = data.MEDI_ORG_CODE;
                        currentTreatmentFinishSDO.TransferOutMediOrgName = data.MEDI_ORG_NAME;
                    }
                    else
                    {
                        currentTreatmentFinishSDO.TransferOutMediOrgCode = null;
                        currentTreatmentFinishSDO.TransferOutMediOrgName = null;
                    }
                }

                currentTreatmentFinishSDO.ClinicalNote = txtDauHieuLamSang.Text;
                currentTreatmentFinishSDO.SubclinicalResult = txtXetNghiem.Text;
                currentTreatmentFinishSDO.PatientCondition = txtTinhTrangNguoiBenh.Text;
                currentTreatmentFinishSDO.TransportVehicle = txtPhuongTienVanChuyen.Text;
                currentTreatmentFinishSDO.TransporterLoginnames = selected != null && selected.Count > 0 ? string.Join(";", selected.Select(o => o.LOGINNAME)) : null;
                currentTreatmentFinishSDO.Transporter = selected != null && selected.Count > 0 ? string.Join(";", selected.Select(o => o.TDL_USERNAME)) : null;
                currentTreatmentFinishSDO.TreatmentMethod = txtPPKTThuoc.Text;
                currentTreatmentFinishSDO.TreatmentDirection = txtHuongDieuTri.Text;
                if (cboLoginName.EditValue != null)
                {
                    currentTreatmentFinishSDO.TranPatiHospitalLoginname = cboLoginName.EditValue.ToString();
                    currentTreatmentFinishSDO.TranPatiHospitalUsername = cboLoginName.Text.ToString();
                }
                if (MyGetData != null)
                    MyGetData(currentTreatmentFinishSDO);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Shotcut
        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
        #endregion

        private void cboMediOrgName_Properties_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboMediOrgName.ClosePopup();
                    cboMediOrgName.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboMediOrgName.ClosePopup();
                }
                else
                    cboMediOrgName.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtTranPatiTechCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtTranPatiTechCode.Text.Trim()))
                    {
                        string code = txtTranPatiTechCode.Text.Trim();
                        var listData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_TECH>().Where(o => o.TRAN_PATI_TECH_CODE.ToUpper().Contains(code.ToUpper())).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TRAN_PATI_TECH_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtTranPatiTechCode.Text = result.First().TRAN_PATI_TECH_CODE;
                            cboTranPatiTech.EditValue = result.First().ID;
                            txtDauHieuLamSang.Focus();
                            txtDauHieuLamSang.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboTranPatiTech.Focus();
                        cboTranPatiTech.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTranPatiTech_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtTranPatiTechCode.Text = "";
                    cboTranPatiTech.EditValue = null;
                    txtTranPatiTechCode.Focus();
                    txtTranPatiTechCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTranPatiTech_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboTranPatiTech.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_TECH>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiTech.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiTechCode.Text = data.TRAN_PATI_TECH_CODE;
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

        private void cboTranPatiTech_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTranPatiTech.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_TECH>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiTech.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiTechCode.Text = data.TRAN_PATI_TECH_CODE;
                        }
                    }
                    txtDauHieuLamSang.Focus();
                    txtDauHieuLamSang.SelectAll();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboTranPatiReason.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSaveTranPatiTemp_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == ModuleLink_HisTranPatiTemp).FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink :" + ModuleLink_HisTranPatiTemp);
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module '" + ModuleLink_HisTranPatiTemp + "' is not plugins");

                HIS_TRAN_PATI_TEMP data = new HIS_TRAN_PATI_TEMP();
                var TranPatiTemp = cboTranPatiTemp.EditValue != null ? this.listDataTranPatiTemp.FirstOrDefault(o => o.ID == Convert.ToInt64(cboTranPatiTemp.EditValue)) : null;
                if (TranPatiTemp != null)
                    data = TranPatiTemp;
                var mediOrg = cboMediOrgName.EditValue != null ? this.listMediOrg.FirstOrDefault(o => o.ID == Convert.ToInt64(cboMediOrgName.EditValue)) : null;
                if (mediOrg != null)
                {
                    data.MEDI_ORG_CODE = mediOrg.MEDI_ORG_CODE;
                    data.MEDI_ORG_NAME = mediOrg.MEDI_ORG_NAME;
                }
                var tranPatiReason = cboTranPatiReason.EditValue != null ? this.HisTranPatiReasons.FirstOrDefault(o => o.ID == Convert.ToInt64(cboTranPatiReason.EditValue)) : null;
                if (tranPatiReason != null)
                    data.TRAN_PATI_REASON_ID = tranPatiReason.ID;
                var tranPatiForm = cboTranPatiForm.EditValue != null ? this.HisTranPatiForms.FirstOrDefault(o => o.ID == Convert.ToInt64(cboTranPatiForm.EditValue)) : null;
                if (tranPatiForm != null)
                    data.TRAN_PATI_FORM_ID = tranPatiForm.ID;
                var tranPatiTech = cboTranPatiTech.EditValue != null ? this.HisTranPatiTechs.FirstOrDefault(o => o.ID == Convert.ToInt64(cboTranPatiTech.EditValue)) : null;
                if (tranPatiTech != null)
                    data.TRAN_PATI_TECH_ID = tranPatiTech.ID;

                data.TREATMENT_DIRECTION = txtHuongDieuTri.Text;
                data.PATIENT_CONDITION = txtTinhTrangNguoiBenh.Text;
                data.TREATMENT_METHOD = txtPPKTThuoc.Text;
                data.TRANSPORT_VEHICLE = txtPhuongTienVanChuyen.Text;
                //data.USED_MEDICINE = txtUsedMedicine.Text;
                //data.TRANSPORTER = txtNguoiHoTong.Text;

                List<object> listArgs = new List<object>();
                listArgs.Add(moduleData);
                listArgs.Add(data);

                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("ModuleData is null");

                WaitingManager.Hide();
                ((Form)extenceInstance).ShowDialog();
                LoadDataToComboTranPatiTemp(true);
                if (cboTranPatiTemp.EditValue != null)
                {
                    var dataLoad = this.listDataTranPatiTemp.Where(o => o.ID == Convert.ToInt64(cboTranPatiTemp.EditValue)).FirstOrDefault();
                    if (dataLoad != null)
                    {
                        txtTranPatiTemp.Text = dataLoad.TRAN_PATI_TEMP_CODE;
                        FillDataToControls_ByTranPatiTemp(dataLoad);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControls_ByTranPatiTemp(HIS_TRAN_PATI_TEMP data)
        {
            try
            {
                if (data == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("FillDataToControls_ByTranPatiTemp(): data is null!");
                    return;
                }
                var mediOrg = this.listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == data.MEDI_ORG_CODE);
                cboMediOrgName.EditValue = mediOrg != null ? mediOrg.MEDI_ORG_CODE : null;
                txtMediOrgCode.Text = mediOrg != null ? mediOrg.MEDI_ORG_CODE : "";
                lblMediOrgAddress.Text = mediOrg != null ? mediOrg.ADDRESS : "";
                var tranPatiReason = this.HisTranPatiReasons.FirstOrDefault(o => o.ID == data.TRAN_PATI_REASON_ID);
                cboTranPatiReason.EditValue = tranPatiReason != null ? (long?)tranPatiReason.ID : null;
                txtTranPatiReason.Text = tranPatiReason != null ? tranPatiReason.TRAN_PATI_REASON_CODE : "";
                var tranPatiForm = this.HisTranPatiForms.FirstOrDefault(o => o.ID == data.TRAN_PATI_FORM_ID);
                cboTranPatiForm.EditValue = tranPatiForm != null ? (long?)tranPatiForm.ID : null;
                txtTranPatiForm.Text = tranPatiForm != null ? tranPatiForm.TRAN_PATI_FORM_CODE : "";
                var tranPatiTech = this.HisTranPatiTechs.FirstOrDefault(o => o.ID == data.TRAN_PATI_TECH_ID);
                cboTranPatiTech.EditValue = tranPatiTech != null ? (long?)tranPatiTech.ID : null;
                txtTranPatiTechCode.Text = tranPatiTech != null ? tranPatiTech.TRAN_PATI_TECH_CODE : "";

                txtHuongDieuTri.Text = data.TREATMENT_DIRECTION;
                txtTinhTrangNguoiBenh.Text = data.PATIENT_CONDITION;
                txtPPKTThuoc.Text = data.TREATMENT_METHOD;
                txtPhuongTienVanChuyen.Text = data.TRANSPORT_VEHICLE;
                //txtUsedMedicine.Text = data.USED_MEDICINE;
                ///txtNguoiHoTong.Text = data.TRANSPORTER;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiTemp_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTranPatiTemp.EditValue != null)
                {
                    cboTranPatiTemp.Properties.Buttons[1].Visible = true;
                    var data = this.listDataTranPatiTemp.Where(o => o.ID == Convert.ToInt64(cboTranPatiTemp.EditValue)).FirstOrDefault();
                    if (data != null)
                    {
                        txtTranPatiTemp.Text = data.TRAN_PATI_TEMP_CODE;
                        FillDataToControls_ByTranPatiTemp(data);
                    }
                }
                else
                {
                    txtTranPatiTemp.Text = "";
                    cboTranPatiTemp.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiTemp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtTranPatiTemp.Text = "";
                    cboTranPatiTemp.EditValue = null;
                    cboTranPatiTemp.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTranPatiTemp_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTranPatiTemp.Text.Trim()))
                    {
                        string code = txtTranPatiTemp.Text.Trim().ToLower();
                        var listData = this.listDataTranPatiTemp.Where(o => o.TRAN_PATI_TEMP_CODE != null && o.TRAN_PATI_TEMP_CODE.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TRAN_PATI_TEMP_CODE.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            txtTranPatiTemp.Text = result.First().TRAN_PATI_TEMP_CODE;
                            cboTranPatiTemp.EditValue = result.First().ID;
                            cboTranPatiTemp.Properties.Buttons[1].Visible = true;
                        }
                        else
                        {
                            cboTranPatiTemp.Focus();
                            cboTranPatiTemp.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDataTocboUser()
        {
            try
            {
                this.lstReAcsUserADO = new List<AcsUserADO>();
                var acsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                       <ACS.EFMODEL.DataModels.ACS_USER>().Where(p => !string.IsNullOrEmpty(p.USERNAME) && p.IS_ACTIVE == 1).OrderBy(o => o.USERNAME).ToList();
                var Employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().Where(o=>o.IS_ACTIVE == 1);
                foreach (var item in acsUser)
                {
                    AcsUserADO ado = new AcsUserADO(item);

                    var VhisEmployee = Employees.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME && o.IS_DOCTOR == 1 && o.IS_ACTIVE == 1 && o.IS_DELETE == 0);
                    if (VhisEmployee != null)
                    {
                        ado.DOB = VhisEmployee.DOB;
                        ado.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(VhisEmployee.DOB ?? 0);
                        ado.DIPLOMA = VhisEmployee.DIPLOMA;
                        ado.DEPARTMENT_CODE = VhisEmployee.DEPARTMENT_CODE;
                        ado.DEPARTMENT_ID = VhisEmployee.DEPARTMENT_ID;
                        ado.DEPARTMENT_NAME = VhisEmployee.DEPARTMENT_NAME;
                        this.lstReAcsUserADO.Add(ado);
                    }

                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "Họ tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 400);
                ControlEditorLoader.Load(cboLoginName, this.lstReAcsUserADO.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
                cboLoginName.Properties.ImmediatePopup = true;

                if (this.hisTreatment != null)
                {
                    cboLoginName.EditValue = hisTreatment.TRAN_PATI_HOSPITAL_LOGINNAME;
                }

                InitComboTransporterLoginName(Employees.ToList());
                if (this.hisTreatment != null && !string.IsNullOrEmpty(hisTreatment.TRANSPORTER_LOGINNAMES))
                {
                    var oldSelecteds = hisTreatment.TRANSPORTER_LOGINNAMES.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                        if (oldSelecteds != null && oldSelecteds.Count > 0)
                        {
                            selected = Employees.Where(o => oldSelecteds.Contains(o.LOGINNAME)).ToList();
                            gridCheckMark.SelectAll(selected);
                            string displayText = String.Join(", ", selected.Select(s => s.TDL_USERNAME).ToList());
                            cboTransporterLoginName.Text = displayText;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtLoginName.Text))
                    {
                        var data = lstReAcsUserADO.FirstOrDefault(o => o.LOGINNAME == txtLoginName.Text.Trim());
                        if (data != null)
                        {
                            cboLoginName.EditValue = data.LOGINNAME;
                            cboLoginName.Focus();
                        }
                        else
                        {
                            cboLoginName.Focus();
                            cboLoginName.ShowPopup();
                        }
                    }
                    else
                    {
                        cboLoginName.Focus();
                        cboLoginName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoginName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboLoginName.EditValue != null)
                    txtLoginName.Text = cboLoginName.EditValue.ToString();
                else
                    txtLoginName.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoginName_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboLoginName.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTransporterLoginName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTransporterLoginName.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                    }
                    cboTransporterLoginName.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitComboTransporterLoginNameCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboTransporterLoginName.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboTransporterLoginName.Properties.Tag = gridCheck;
                cboTransporterLoginName.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Event_Check(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                selected = new List<V_HIS_EMPLOYEE>();
                if (gridCheckMark != null)
                {
                    List<V_HIS_EMPLOYEE> erSelectedNews = new List<V_HIS_EMPLOYEE>();
                    foreach (V_HIS_EMPLOYEE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.TDL_USERNAME);
                            erSelectedNews.Add(er);
                        }
                    }
                    this.selected = new List<V_HIS_EMPLOYEE>();
                    this.selected.AddRange(erSelectedNews);
                }
                this.cboTransporterLoginName.Text = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTransporterLoginName(List<V_HIS_EMPLOYEE> data)
        {
            try
            {
                try
                {
                    cboTransporterLoginName.Properties.DataSource = data;
                    cboTransporterLoginName.Properties.DisplayMember = "TDL_USERNAME";
                    cboTransporterLoginName.Properties.ValueMember = "LOGINNAME";

                    cboTransporterLoginName.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
                    cboTransporterLoginName.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
                    cboTransporterLoginName.Properties.View.OptionsView.ShowAutoFilterRow = true;
                    cboTransporterLoginName.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
                    cboTransporterLoginName.Properties.View.OptionsView.ShowDetailButtons = false;
                    cboTransporterLoginName.Properties.View.OptionsView.ShowGroupPanel = false;
                    cboTransporterLoginName.Properties.View.OptionsView.ShowIndicator = false;


                    DevExpress.XtraGrid.Columns.GridColumn column = cboTransporterLoginName.Properties.View.Columns.AddField("LOGINNAME");
                    column.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                    column.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
                    column.VisibleIndex = 1;
                    column.Width = 150;
                    column.Caption = "Tên đăng nhập";

                    DevExpress.XtraGrid.Columns.GridColumn column1 = cboTransporterLoginName.Properties.View.Columns.AddField("TDL_USERNAME");
                    column1.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                    column1.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
                    column1.VisibleIndex = 2;
                    column1.Width = 250;
                    column1.Caption = "Họ tên";
                    cboTransporterLoginName.Properties.View.OptionsView.ShowColumnHeaders = true;
                    cboTransporterLoginName.Properties.View.OptionsSelection.MultiSelect = true;
                    cboTransporterLoginName.Properties.ImmediatePopup = true;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTransporterLoginName_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.selected != null && this.selected.Count > 0)
                {
                    foreach (var item in this.selected)
                    {
                        roomName += item.TDL_USERNAME + ", ";

                    }

                }

                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }
    }
}
