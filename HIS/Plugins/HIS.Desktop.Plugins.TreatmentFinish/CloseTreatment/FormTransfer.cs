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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TreatmentFinish.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.CustomControl;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using Inventec.Desktop.CustomControl.CustomGrid;
using HIS.Desktop.Plugins.TreatmentFinish.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.TreatmentFinish.CloseTreatment
{
    public partial class FormTransfer : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private int positionHandle = -1;
        private MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment { get; set; }
        private MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO { get; set; }
        internal FormTreatmentFinish Form;
        internal delegate void GetString(MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO);
        internal GetString MyGetData;

        List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG> listMediOrg { get; set; }
        private List<HIS_TRAN_PATI_TEMP> listDataTranPatiTemp { get; set; }
        public List<AcsUserADO> lstReAcsUserADO { get; private set; }
        List<V_HIS_EMPLOYEE> selected = new List<V_HIS_EMPLOYEE>();

        Inventec.Desktop.Common.Modules.Module moduleDataTransfer;
        #endregion

        #region Construct
        public FormTransfer()
        {
            InitializeComponent();
        }

        public FormTransfer(Inventec.Desktop.Common.Modules.Module _moduleDataTransfer, MOS.EFMODEL.DataModels.HIS_TREATMENT treatment)
            : base(_moduleDataTransfer)
        {
            InitializeComponent();
            try
            {
                this.hisTreatment = treatment;
                this.moduleDataTransfer = _moduleDataTransfer;
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

                SetCaptionByLanguageKey();
                SetIcon();
                InitComboTransporterLoginNameCheck();
                LoadDataToCombo();

                if (this.hisTreatment != null)
                {
                    loadDataTranPatiOld(hisTreatment);//Lấy thông tin chuyển viện cũ
                }
                LoadDataTocboUser();
                SetDefaultValueControl();

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void loadDataTranPatiOld(MOS.EFMODEL.DataModels.HIS_TREATMENT treatment) // load trong db
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
                            cboMediOrgName.EditValue = mediOrgName.ID;
                            txtMediOrgCode.Text = mediOrgName.MEDI_ORG_CODE;
                            lblMediOrgAddress.Text = mediOrgName.ADDRESS;
                        }
                    }
                    if (treatment.TRAN_PATI_FORM_ID.HasValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM hisTranPatiForm = Base.GlobalStore.HisTranPatiForms.SingleOrDefault(o => o.ID == treatment.TRAN_PATI_FORM_ID);
                        if (hisTranPatiForm != null)
                        {
                            cboTranPatiForm.EditValue = hisTranPatiForm.ID;
                            txtTranPatiForm.Text = hisTranPatiForm.TRAN_PATI_FORM_CODE;
                        }
                    }

                    if (treatment.TRAN_PATI_REASON_ID.HasValue)
                    {
                        var tranPatiReason = Base.GlobalStore.HisTranPatiReasons.FirstOrDefault(o => o.ID == treatment.TRAN_PATI_REASON_ID);
                        if (tranPatiReason != null)
                        {
                            cboTranPatiReason.EditValue = tranPatiReason.ID;
                            txtTranPatiReason.Text = tranPatiReason.TRAN_PATI_REASON_CODE;
                        }
                    }

                    if (treatment.TRAN_PATI_TECH_ID.HasValue)
                    {
                        var tranPatiTech = Base.GlobalStore.TranPatiTechs.FirstOrDefault(o => o.ID == treatment.TRAN_PATI_TECH_ID);
                        if (tranPatiTech != null)
                        {
                            cboLyDoChuyenMon.EditValue = tranPatiTech.ID;
                            txtLyDoChuyenMon.Text = tranPatiTech.TRAN_PATI_TECH_CODE;
                        }
                    }
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
                                List<V_HIS_EMPLOYEE> seleceds = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => oldSelecteds.Contains(o.LOGINNAME)).ToList();
                                gridCheckMark.SelectAll(seleceds);
                            }
                        }
                    }
                    if (treatment != null && !string.IsNullOrEmpty(treatment.TRANSPORTER))
                    {
                        var splt = treatment.TRANSPORTER.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in splt)
                        {
                            if (selected.FirstOrDefault(o => !string.IsNullOrEmpty(o.LOGINNAME) && o.TDL_USERNAME == item.Trim()) != null)
                                continue;
                            selected.Add(new V_HIS_EMPLOYEE() { TDL_USERNAME = item });
                        }
                        txtTransporterLoginName.Text = treatment.TRANSPORTER;
                    }
                    if (selected != null && selected.Count > 0)
                    {
                        selected = selected.Distinct(new CompareTransfer()).ToList();
                        string displayText = String.Join(", ", selected.Select(s => s.TDL_USERNAME).ToList());
                        cboTransporterLoginName.Text = displayText;
                    }
                    txtPPKTThuoc.Text = treatment.TREATMENT_METHOD;
                    txtHuongDieuTri.Text = treatment.TREATMENT_DIRECTION;
                    txtUsedMedicine.Text = GetUsedMedicine(treatment.ID);
                    //qtcode
                    txtClinicalSigns.Text = treatment.CLINICAL_SIGNS;
                    //qtcode
                    if (string.IsNullOrEmpty(txtUsedMedicine.Text)
                        && !string.IsNullOrEmpty(treatment.USED_MEDICINE))
                        txtUsedMedicine.Text = treatment.USED_MEDICINE;

                    if (treatment.VALID_1_YEAR == 1)
                    {
                        chk1Year.Checked = true;
                    }
                    else
                    {
                        chk1Year.Checked = false;
                    }
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentFinish.Resources.Lang", typeof(FormTransfer).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormTransfer.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveTranPatiTemp.Text = Inventec.Common.Resource.Get.Value("FormTransfer.btnSaveTranPatiTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTranPatiTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.cboTranPatiTemp.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormTransfer.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("FormTransfer.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLyDoChuyenMon.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.cboLyDoChuyenMon.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormTransfer.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTranPatiForm.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.cboTranPatiForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTranPatiReason.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.cboTranPatiReason.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediOrgName.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.cboMediOrgName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMediOrg.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciMediOrg.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTranPatiReason.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciTranPatiReason.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPhuongTienVanChuyen.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciPhuongTienVanChuyen.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //qtcode
                this.lciClinicalSigns.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciClinicalSigns.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //qtcode
                this.lciMediOrgAddress.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciMediOrgAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHuongDieuTri.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciHuongDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPhuongPhapSuDung.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciPhuongPhapSuDung.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FormTransfer.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTranPatiForm.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciTranPatiForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("FormTransfer.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("FormTransfer.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTinhTrangNguoiBenh.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciTinhTrangNguoiBenh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTranPatiTemp.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciTranPatiTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTransfer.layoutControlItem8.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("FormTransfer.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLoginName.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.cboLoginName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSurgeryName.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTransfer.txtSurgeryName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormTransfer.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DataToComboBenhVien(List<HIS_MEDI_ORG> dataMediOrg, CustomGridLookUpEditWithFilterMultiColumn cbo)
        {
            try
            {
                List<MediOrgADO> listADO = new List<MediOrgADO>();
                foreach (var item in dataMediOrg)
                {
                    MediOrgADO medi = new MediOrgADO(item);
                    listADO.Add(medi);
                }

                cbo.Properties.DataSource = listADO;
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

        private void LoadDataToCombo()
        {
            try
            {
                string ma = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_MA");
                string ten = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_TEN");

                listMediOrg = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>().OrderBy(o => o.MEDI_ORG_CODE).ToList();
                DataToComboBenhVien(listMediOrg, cboMediOrgName);

                Base.GlobalStore.LoadDataGridLookUpEdit(cboTranPatiReason,
                    "TRAN_PATI_REASON_CODE", ma, "TRAN_PATI_REASON_NAME", ten, "ID", Base.GlobalStore.HisTranPatiReasons);
                Base.GlobalStore.LoadDataGridLookUpEdit(cboTranPatiForm,
                    "TRAN_PATI_FORM_CODE", ma, "TRAN_PATI_FORM_NAME", ten, "ID", Base.GlobalStore.HisTranPatiForms);
                Base.GlobalStore.LoadDataGridLookUpEdit(this.cboLyDoChuyenMon,
                   "TRAN_PATI_TECH_CODE", ma, "TRAN_PATI_TECH_NAME", ten, "ID", Base.GlobalStore.TranPatiTechs);

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
                result = new BackendAdapter(param).Get<List<HIS_TRAN_PATI_TEMP>>("api/HisTranPatiTemp/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetDefaultValueControl() // load lưu tạm
        {
            try
            {
                chk1Year.Checked = false;
                currentTreatmentFinishSDO = Form.hisTreatmentFinishSDO_process;
                if (currentTreatmentFinishSDO != null)
                {
                    if (currentTreatmentFinishSDO.ClinicalSigns != null)
                    {
                        txtClinicalSigns.Text = currentTreatmentFinishSDO.ClinicalSigns;
                    }
                    var mediOrgName = listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == currentTreatmentFinishSDO.TransferOutMediOrgCode);
                    if (mediOrgName != null)
                    {
                        cboMediOrgName.EditValue = mediOrgName.ID;
                        txtMediOrgCode.Text = mediOrgName.MEDI_ORG_CODE;
                        lblMediOrgAddress.Text = mediOrgName.ADDRESS;
                    }

                    var hisTranPatiReason = Base.GlobalStore.HisTranPatiReasons.SingleOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiReasonId);
                    if (hisTranPatiReason != null)
                    {
                        cboTranPatiReason.EditValue = hisTranPatiReason.ID;
                        txtTranPatiReason.Text = hisTranPatiReason.TRAN_PATI_REASON_CODE;
                    }
                    var hisTranPatiTech = Base.GlobalStore.TranPatiTechs.SingleOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiTechId);
                    if (hisTranPatiTech != null)
                    {
                        cboLyDoChuyenMon.EditValue = hisTranPatiTech.ID;
                        txtLyDoChuyenMon.Text = hisTranPatiTech.TRAN_PATI_TECH_CODE;
                    }

                    var hisTranPatiForm = Base.GlobalStore.HisTranPatiForms.SingleOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiFormId);
                    if (hisTranPatiForm != null)
                    {
                        cboTranPatiForm.EditValue = hisTranPatiForm.ID;
                        txtTranPatiForm.Text = hisTranPatiForm.TRAN_PATI_FORM_CODE;
                    }
                    txtTinhTrangNguoiBenh.Text = currentTreatmentFinishSDO.PatientCondition;
                    txtPhuongTienVanChuyen.Text = currentTreatmentFinishSDO.TransportVehicle;

                    //txtNguoiHoTong.Text = currentTreatmentFinishSDO.Transporter;

                    if (!string.IsNullOrEmpty(currentTreatmentFinishSDO.TransporterLoginnames))
                    {
                        var oldSelecteds = currentTreatmentFinishSDO.TransporterLoginnames.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                            if (oldSelecteds != null && oldSelecteds.Count > 0)
                            {
                                List<V_HIS_EMPLOYEE> seleceds = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => oldSelecteds.Contains(o.LOGINNAME)).ToList();
                                gridCheckMark.SelectAll(seleceds);

                            }
                        }
                    }
                    if (currentTreatmentFinishSDO != null && !string.IsNullOrEmpty(currentTreatmentFinishSDO.Transporter))
                    {
                        var splt = currentTreatmentFinishSDO.Transporter.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in splt)
                        {
                            if (selected.FirstOrDefault(o => !string.IsNullOrEmpty(o.LOGINNAME) && o.TDL_USERNAME == item.Trim()) != null)
                                continue;
                            selected.Add(new V_HIS_EMPLOYEE() { TDL_USERNAME = item });
                        }
                        txtTransporterLoginName.Text = currentTreatmentFinishSDO.Transporter;
                    }
                    if (selected != null && selected.Count > 0)
                    {
                        selected = selected.Distinct(new CompareTransfer()).ToList();
                        string displayText = String.Join(", ", selected.Select(s => s.TDL_USERNAME).ToList());
                        cboTransporterLoginName.Text = displayText;
                    }
                    txtPPKTThuoc.Text = currentTreatmentFinishSDO.TreatmentMethod;
                    txtHuongDieuTri.Text = currentTreatmentFinishSDO.TreatmentDirection;

                    if (currentTreatmentFinishSDO.TreatmentId > 0)
                        txtUsedMedicine.Text = GetUsedMedicine(currentTreatmentFinishSDO.TreatmentId);
                    if (string.IsNullOrEmpty(txtUsedMedicine.Text)
                        && !string.IsNullOrEmpty(currentTreatmentFinishSDO.UsedMedicine))
                        txtUsedMedicine.Text = currentTreatmentFinishSDO.UsedMedicine;

                    if (!string.IsNullOrEmpty(currentTreatmentFinishSDO.SurgeryName))
                    {
                        txtSurgeryName.Text = currentTreatmentFinishSDO.SurgeryName;
                    }
                    if (currentTreatmentFinishSDO.SurgeryBeginTime != null)
                    {
                        dtStart.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatmentFinishSDO.SurgeryBeginTime ?? 0);
                    }
                    if (currentTreatmentFinishSDO.SurgeryEndTime != null)
                    {
                        dtFinish.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatmentFinishSDO.SurgeryEndTime ?? 0);
                    }

                    if (currentTreatmentFinishSDO.Valid1Year == true)
                    {
                        chk1Year.Checked = true;
                    }
                    else
                    {
                        chk1Year.Checked = false;
                    }
                }
                txtMediOrgCode.Focus();
                txtMediOrgCode.SelectAll();
                txtSurgeryName.Properties.NullText = "Nhấn F1 để chọn dịch vụ";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetUsedMedicine(long treatmentId)
        {
            string rs = "";
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter filter = new HisExpMestMedicineViewFilter();
                filter.TDL_TREATMENT_ID = treatmentId;
                filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;

                var expMestMedicine = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                if (expMestMedicine != null && expMestMedicine.Count > 0)
                {
                    var expMestMedicineGroup = expMestMedicine.GroupBy(o => o.MEDICINE_TYPE_ID).ToList();
                    foreach (var group in expMestMedicineGroup)
                    {
                        rs += group.First().MEDICINE_TYPE_NAME + (!string.IsNullOrEmpty(group.First().CONCENTRA) ? "(" + group.First().CONCENTRA + ");" : ";");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }

            return rs;
        }

        #region Validation
        private void ValidateForm()
        {
            try
            {
                ValidationTransferMediOrg();
                ValidationTranPatiReason();
                ValidationTranPatiForm();
                ValidationMaxLength(txtPPKTThuoc, 3000);
                ValidationMaxLength(txtHuongDieuTri, 3000);
                ValidationMaxLength(txtTinhTrangNguoiBenh, 3000);
                ValidationMaxLength(txtPhuongTienVanChuyen, 3000);
                //qtcode
                ValidationMaxLength(txtClinicalSigns, 3000);
                ValidateTextEdit(txtClinicalSigns);
                //qtcode
                //ValidationMaxLength(txtNguoiHoTong, 200);
                ValidationMaxLength(txtUsedMedicine, 3000);
                ValidateTextEdit(txtHuongDieuTri);
                ValidateTextEdit(txtPhuongTienVanChuyen);
                ValidateTextEdit(txtPPKTThuoc);
                ValidateGridLookupWithTextEditSpecial(this.cboTransporterLoginName, txtTransporterLoginName, this.dxValidationProvider);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationTransferMediOrg()
        {
            try
            {
                Validation.TransferMediOrgValidationRule icdMainRule = new Validation.TransferMediOrgValidationRule();
                icdMainRule.txtMediOrgCode = txtMediOrgCode;
                icdMainRule.cboMediOrgName = cboMediOrgName;
                icdMainRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                icdMainRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtMediOrgCode, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationTranPatiReason()
        {
            try
            {
                Validation.TranPatiReasonValidationRule tranPatiReason = new Validation.TranPatiReasonValidationRule();
                tranPatiReason.txtTranPatiReason = txtTranPatiReason;
                tranPatiReason.cboTranPatiReason = cboTranPatiReason;
                tranPatiReason.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                tranPatiReason.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtTranPatiReason, tranPatiReason);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationTranPatiForm()
        {
            try
            {
                Validation.TranPatiFormValidationRule icdMainRule = new Validation.TranPatiFormValidationRule();
                icdMainRule.txtTranPatiForm = txtTranPatiForm;
                icdMainRule.cboTranPatiForm = cboTranPatiForm;
                icdMainRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                icdMainRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtTranPatiForm, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void ValidateGridLookupWithTextEditSpecial(GridLookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                GridLookupEditWithTextEditValidationRuleSpecial validRule = new GridLookupEditWithTextEditValidationRuleSpecial();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxLength(MemoEdit memoEdit, int? maxLength)
        {
            try
            {
                Validation.ValidateMaxLength maxLengthValid = new Validation.ValidateMaxLength();
                maxLengthValid.memoEdit = memoEdit;
                maxLengthValid.maxLength = maxLength;
                maxLengthValid.ErrorText = "Dữ liệu vượt quá độ dài cho phép";
                maxLengthValid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(memoEdit, maxLengthValid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                            cboMediOrgName.EditValue = result.First().ID;
                            lblMediOrgAddress.Text = result.First().ADDRESS;
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
                        var data = listMediOrg.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediOrgName.EditValue.ToString()));
                        if (data != null)
                        {
                            txtMediOrgCode.Text = data.MEDI_ORG_CODE;
                            lblMediOrgAddress.Text = data.ADDRESS;
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
                        var data = listMediOrg.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediOrgName.EditValue.ToString()));
                        if (data != null)
                        {
                            txtMediOrgCode.Text = data.MEDI_ORG_CODE;
                            lblMediOrgAddress.Text = data.ADDRESS;
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
                        var listData = Base.GlobalStore.HisTranPatiReasons.Where(o => o.TRAN_PATI_REASON_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TRAN_PATI_REASON_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtTranPatiReason.Text = result.First().TRAN_PATI_REASON_CODE;
                            cboTranPatiReason.EditValue = result.First().ID;
                            txtLyDoChuyenMon.Focus();
                            txtLyDoChuyenMon.SelectAll();
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
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = Base.GlobalStore.HisTranPatiReasons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiReason.Text = data.TRAN_PATI_REASON_CODE;
                            txtLyDoChuyenMon.Focus();
                            txtLyDoChuyenMon.SelectAll();
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
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = Base.GlobalStore.HisTranPatiReasons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiReason.Text = data.TRAN_PATI_REASON_CODE;
                            txtLyDoChuyenMon.Focus();
                            txtLyDoChuyenMon.SelectAll();
                        }
                    }
                    else
                    {
                        txtLyDoChuyenMon.Focus();
                        txtLyDoChuyenMon.SelectAll();
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
                        var listData = Base.GlobalStore.HisTranPatiForms.Where(o => o.TRAN_PATI_FORM_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TRAN_PATI_FORM_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtTranPatiForm.Text = result.First().TRAN_PATI_FORM_CODE;
                            cboTranPatiForm.EditValue = result.First().ID;
                            txtHuongDieuTri.Focus();
                            txtHuongDieuTri.SelectAll();
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
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = Base.GlobalStore.HisTranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiForm.Text = data.TRAN_PATI_FORM_CODE;
                            //qtcode
                            txtClinicalSigns.Focus();
                            txtClinicalSigns.SelectAll();
                            //qtcode
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
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = Base.GlobalStore.HisTranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiForm.Text = data.TRAN_PATI_FORM_CODE;
                            txtHuongDieuTri.Focus();
                            txtHuongDieuTri.SelectAll();
                        }
                    }
                    else
                    {
                        txtClinicalSigns.Focus();
                        txtClinicalSigns.SelectAll();
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
                if (!dxValidationProvider.Validate()) return;

                currentTreatmentFinishSDO.TreatmentId = hisTreatment.ID;

                MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON hisTranPatiReason = Base.GlobalStore.HisTranPatiReasons.SingleOrDefault(o => o.ID == (long)cboTranPatiReason.EditValue);
                if (cboTranPatiReason.EditValue != null)
                {
                    currentTreatmentFinishSDO.TranPatiReasonId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString());
                }
                else
                    currentTreatmentFinishSDO.TranPatiReasonId = null;

                if (cboLyDoChuyenMon.EditValue != null)
                {
                    currentTreatmentFinishSDO.TranPatiTechId = Inventec.Common.TypeConvert.Parse.ToInt64(cboLyDoChuyenMon.EditValue.ToString());
                }
                else
                    currentTreatmentFinishSDO.TranPatiTechId = null;

                if (cboTranPatiForm.EditValue != null)
                {
                    currentTreatmentFinishSDO.TranPatiFormId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString());
                }
                else
                    currentTreatmentFinishSDO.TranPatiFormId = null;
                var data = listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == txtMediOrgCode.Text);
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

                currentTreatmentFinishSDO.PatientCondition = txtTinhTrangNguoiBenh.Text;
                currentTreatmentFinishSDO.TransportVehicle = txtPhuongTienVanChuyen.Text;
                List<string> lstLoginNames = new List<string>();
                if (selected != null && selected.Count > 0)
                {
                    var lst = selected.Where(o => !string.IsNullOrEmpty(o.LOGINNAME));
                    if (lst != null && lst.Count() > 0)
                    {
                        lstLoginNames = lst.Select(o => o.LOGINNAME).ToList();
                    }
                }
                currentTreatmentFinishSDO.TransporterLoginnames = lstLoginNames != null && lstLoginNames.Count > 0 ? string.Join(";", lstLoginNames) : null;
                currentTreatmentFinishSDO.Transporter = selected != null && selected.Count > 0 ? string.Join(";", selected.Select(o => o.TDL_USERNAME)) : null;
                currentTreatmentFinishSDO.TreatmentMethod = txtPPKTThuoc.Text;
                currentTreatmentFinishSDO.TreatmentDirection = txtHuongDieuTri.Text;
                currentTreatmentFinishSDO.UsedMedicine = txtUsedMedicine.Text;
                if (cboLoginName.EditValue != null)
                {
                    currentTreatmentFinishSDO.TranPatiHospitalLoginname = cboLoginName.EditValue.ToString();
                    currentTreatmentFinishSDO.TranPatiHospitalUsername = cboLoginName.Text.ToString();
                }
                if (!string.IsNullOrEmpty(txtSurgeryName.Text))
                {
                    currentTreatmentFinishSDO.SurgeryName = txtSurgeryName.Text.Trim();
                }
                else
                {
                    currentTreatmentFinishSDO.SurgeryName = null;
                }
                if (!string.IsNullOrEmpty(dtStart.Text))
                {
                    currentTreatmentFinishSDO.SurgeryBeginTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime)dtStart.EditValue);
                }
                else
                {
                    currentTreatmentFinishSDO.SurgeryBeginTime = null;
                }
                if (!string.IsNullOrEmpty(dtFinish.Text))
                {
                    currentTreatmentFinishSDO.SurgeryEndTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime)dtFinish.EditValue);
                }
                else
                {
                    currentTreatmentFinishSDO.SurgeryEndTime = null;
                }
                if (chk1Year.Checked == true)
                {
                    currentTreatmentFinishSDO.Valid1Year = true;
                }
                else
                {
                    currentTreatmentFinishSDO.Valid1Year = false;
                }
                //qtcode
                currentTreatmentFinishSDO.ClinicalSigns = txtClinicalSigns.Text.Trim(); // lưu ở lưu tạm
                //qtcode

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
        private void barButtonItemShowSurgMisu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ListSurgMisuByTreatment").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ListSurgMisuByTreatment'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.moduleDataTransfer.RoomId;
                    moduleData.RoomTypeId = this.moduleDataTransfer.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    listArgs.Add(hisTreatment.ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateLoadPTTT)ProcessLoadPTTT);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void ProcessLoadPTTT(string namePTTT, DateTime? startTime, DateTime? finishTime)
        {
            try
            {
                txtSurgeryName.Text = namePTTT;
                //dtStart.DateTime = startTime ?? DateTime.MinValue;
                if (startTime != null)
                {
                    dtStart.DateTime = startTime ?? DateTime.MinValue;
                }
                if (finishTime != null)
                {
                    dtFinish.DateTime = finishTime ?? DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLyDoChuyenMon_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtLyDoChuyenMon.Text.Trim()))
                    {
                        string code = txtLyDoChuyenMon.Text.Trim();
                        var listData = Base.GlobalStore.TranPatiTechs.Where(o => o.TRAN_PATI_TECH_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TRAN_PATI_TECH_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtLyDoChuyenMon.Text = result.First().TRAN_PATI_TECH_CODE;
                            cboLyDoChuyenMon.EditValue = result.First().ID;
                            txtTranPatiForm.Focus();
                            txtTranPatiForm.SelectAll();
                            cboLyDoChuyenMon.Properties.Buttons[1].Visible = false;
                        }
                    }
                    if (showCbo)
                    {
                        cboLyDoChuyenMon.Focus();
                        cboLyDoChuyenMon.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLyDoChuyenMon_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboLyDoChuyenMon.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH data = Base.GlobalStore.TranPatiTechs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboLyDoChuyenMon.EditValue.ToString()));
                        if (data != null)
                        {
                            txtLyDoChuyenMon.Text = data.TRAN_PATI_TECH_CODE;
                            txtTranPatiForm.Focus();
                            txtTranPatiForm.SelectAll();
                            cboLyDoChuyenMon.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLyDoChuyenMon_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLyDoChuyenMon.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH data = Base.GlobalStore.TranPatiTechs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboLyDoChuyenMon.EditValue.ToString()));
                        if (data != null)
                        {
                            txtLyDoChuyenMon.Text = data.TRAN_PATI_TECH_CODE;
                            txtTranPatiForm.Focus();
                            txtTranPatiForm.SelectAll();
                            cboLyDoChuyenMon.Properties.Buttons[1].Visible = true;
                        }
                    }
                    else
                    {
                        txtTranPatiForm.Focus();
                        txtTranPatiForm.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboLyDoChuyenMon.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLyDoChuyenMon_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboLyDoChuyenMon.EditValue = null;
                    txtLyDoChuyenMon.Text = "";
                    cboLyDoChuyenMon.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediOrgName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                long L1 = 0;
                long L2 = 0;
                long BranchWorkID = (long)HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId();
                var BranchWork = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == BranchWorkID);
                if (BranchWork != null && !string.IsNullOrEmpty(BranchWork.HEIN_LEVEL_CODE))
                {
                    L1 = Convert.ToInt64(BranchWork.HEIN_LEVEL_CODE ?? "");
                }
                var MediOrg = this.listMediOrg.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.
                    Parse.ToInt64(cboMediOrgName.EditValue.ToString() ?? ""));
                if (MediOrg != null && !String.IsNullOrEmpty(MediOrg.LEVEL_CODE))
                {
                    if (MediOrg.LEVEL_CODE.Contains("TW"))
                    {
                        L2 = 1;
                    }
                    else if (MediOrg.LEVEL_CODE.Contains("T"))
                    {
                        L2 = 2;
                    }
                    else if (MediOrg.LEVEL_CODE.Contains("H"))
                    {
                        L2 = 3;
                    }
                    else if (MediOrg.LEVEL_CODE.Contains("X"))
                    {
                        L2 = 4;
                    }
                    else
                    {
                        L2 = Convert.ToInt64(MediOrg.LEVEL_CODE ?? "");
                    }
                }
                if ((L1 - L2) == 1)
                {
                    cboTranPatiForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE;
                    txtTranPatiForm.Text = Base.GlobalStore.HisTranPatiForms.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE).TRAN_PATI_FORM_CODE;
                }
                else if ((L1 - L2) > 1)
                {
                    cboTranPatiForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE;
                    txtTranPatiForm.Text = Base.GlobalStore.HisTranPatiForms.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE).TRAN_PATI_FORM_CODE;
                }
                else if ((L1 - L2) < 0)
                {
                    cboTranPatiForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG;
                    txtTranPatiForm.Text = Base.GlobalStore.HisTranPatiForms.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG).TRAN_PATI_FORM_CODE;
                }
                else if ((L1 - L2) == 0)
                {
                    cboTranPatiForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN;
                    txtTranPatiForm.Text = Base.GlobalStore.HisTranPatiForms.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN).TRAN_PATI_FORM_CODE;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveTranPatiTemp_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == CallModule.HisTranPatiTemp).FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink :" + CallModule.HisTranPatiTemp);
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module '" + CallModule.HisTranPatiTemp + "' is not plugins");

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
                var tranPatiReason = cboTranPatiReason.EditValue != null ? Base.GlobalStore.HisTranPatiReasons.FirstOrDefault(o => o.ID == Convert.ToInt64(cboTranPatiReason.EditValue)) : null;
                if (tranPatiReason != null)
                    data.TRAN_PATI_REASON_ID = tranPatiReason.ID;
                var tranPatiForm = cboTranPatiForm.EditValue != null ? Base.GlobalStore.HisTranPatiForms.FirstOrDefault(o => o.ID == Convert.ToInt64(cboTranPatiForm.EditValue)) : null;
                if (tranPatiForm != null)
                    data.TRAN_PATI_FORM_ID = tranPatiForm.ID;
                var tranPatiTech = cboLyDoChuyenMon.EditValue != null ? Base.GlobalStore.TranPatiTechs.FirstOrDefault(o => o.ID == Convert.ToInt64(cboLyDoChuyenMon.EditValue)) : null;
                if (tranPatiTech != null)
                    data.TRAN_PATI_TECH_ID = tranPatiTech.ID;

                data.TREATMENT_DIRECTION = txtHuongDieuTri.Text;
                data.PATIENT_CONDITION = txtTinhTrangNguoiBenh.Text;
                data.TREATMENT_METHOD = txtPPKTThuoc.Text;
                data.TRANSPORT_VEHICLE = txtPhuongTienVanChuyen.Text;
                data.USED_MEDICINE = txtUsedMedicine.Text;
                //data.TRANSPORTER = txtNguoiHoTong.Text;

                List<object> listArgs = new List<object>();
                listArgs.Add(this.moduleDataTransfer);
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
                cboMediOrgName.EditValue = mediOrg != null ? (long?)mediOrg.ID : null;
                txtMediOrgCode.Text = mediOrg != null ? mediOrg.MEDI_ORG_CODE : "";
                lblMediOrgAddress.Text = mediOrg != null ? mediOrg.ADDRESS : "";
                var tranPatiReason = Base.GlobalStore.HisTranPatiReasons.FirstOrDefault(o => o.ID == data.TRAN_PATI_REASON_ID);
                cboTranPatiReason.EditValue = tranPatiReason != null ? (long?)tranPatiReason.ID : null;
                txtTranPatiReason.Text = tranPatiReason != null ? tranPatiReason.TRAN_PATI_REASON_CODE : "";
                var tranPatiForm = Base.GlobalStore.HisTranPatiForms.FirstOrDefault(o => o.ID == data.TRAN_PATI_FORM_ID);
                cboTranPatiForm.EditValue = tranPatiForm != null ? (long?)tranPatiForm.ID : null;
                txtTranPatiForm.Text = tranPatiForm != null ? tranPatiForm.TRAN_PATI_FORM_CODE : "";
                var tranPatiTech = Base.GlobalStore.TranPatiTechs.FirstOrDefault(o => o.ID == data.TRAN_PATI_TECH_ID);
                cboLyDoChuyenMon.EditValue = tranPatiTech != null ? (long?)tranPatiTech.ID : null;
                txtLyDoChuyenMon.Text = tranPatiTech != null ? tranPatiTech.TRAN_PATI_TECH_CODE : "";

                txtHuongDieuTri.Text = data.TREATMENT_DIRECTION;
                txtTinhTrangNguoiBenh.Text = data.PATIENT_CONDITION;

                txtPPKTThuoc.Text = data.TREATMENT_METHOD;
                txtPhuongTienVanChuyen.Text = data.TRANSPORT_VEHICLE;
                txtUsedMedicine.Text = data.USED_MEDICINE;
                //txtNguoiHoTong.Text = data.TRANSPORTER;
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
                var Employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == 1);
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
                        }
                    }
                }
                if (currentTreatmentFinishSDO != null && !string.IsNullOrEmpty(currentTreatmentFinishSDO.Transporter))
                {
                    var splt = currentTreatmentFinishSDO.Transporter.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in splt)
                    {
                        if (selected.FirstOrDefault(o => !string.IsNullOrEmpty(o.LOGINNAME) && o.TDL_USERNAME == item.Trim()) != null)
                            continue;
                        selected.Add(new V_HIS_EMPLOYEE() { TDL_USERNAME = item });
                    }
                    txtTransporterLoginName.Text = currentTreatmentFinishSDO.Transporter;
                }
                if (selected != null && selected.Count > 0)
                {
                    selected = selected.Distinct(new CompareTransfer()).ToList();
                    string displayText = String.Join(", ", selected.Select(s => s.TDL_USERNAME).ToList());
                    cboTransporterLoginName.Text = displayText;
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
                    this.cboTransporterLoginName.Focus();
                    txtTransporterLoginName.Text = null;
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
                txtTransporterLoginName.Text = roomName;
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
                List<V_HIS_EMPLOYEE> employeeFreeTextList = null;
                if (selected != null && selected.Count > 0)
                    employeeFreeTextList = selected.Where(o => string.IsNullOrEmpty(o.LOGINNAME)).ToList();
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
                if (employeeFreeTextList != null && employeeFreeTextList.Count > 0)
                {
                    foreach (var item in employeeFreeTextList)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(item.TDL_USERNAME);
                    }
                    this.selected.AddRange(employeeFreeTextList);
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => selected.Count), selected.Count));
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

        private void ValidateTextEdit(TextEdit txt)
        {
            try
            {
                ValidateTxtRule valid = new ValidateTxtRule();
                valid.textEdit = txt;
                valid.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txt, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTransporterLoginName_Leave(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtTransporterLoginName.Text.Trim()))
                {
                    selected = new List<V_HIS_EMPLOYEE>();
                    cboTransporterLoginName.EditValue = null;
                }
                else
                {
                    if (selected != null && selected.Count > 0)
                        selected = selected.Where(o => !string.IsNullOrEmpty(o.LOGINNAME)).ToList();
                    var splitNames = txtTransporterLoginName.Text.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in splitNames)
                    {
                        if (selected == null || selected.Count == 0 || selected.FirstOrDefault(o => o.TDL_USERNAME == item.Trim()) == null)
                        {
                            if (selected == null)
                                selected = new List<V_HIS_EMPLOYEE>();
                            selected.Add(new V_HIS_EMPLOYEE() { TDL_USERNAME = item.Trim() });
                        }
                    }
                    txtTransporterLoginName.Text = string.Join(", ", selected.Select(o => o.TDL_USERNAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTransporterLoginName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Combo)
                {
                    cboTransporterLoginName.Focus();
                    cboTransporterLoginName.ShowPopup();
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    selected = new List<V_HIS_EMPLOYEE>();
                    cboTransporterLoginName.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                    }
                    cboTransporterLoginName.Focus();
                    txtTransporterLoginName.Text = null;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTransporterLoginName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtTransporterLoginName.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class CompareTransfer : IEqualityComparer<V_HIS_EMPLOYEE>
    {
        public bool Equals(V_HIS_EMPLOYEE x, V_HIS_EMPLOYEE y)
        {
            return
                x.LOGINNAME == y.LOGINNAME &&
                x.TDL_USERNAME == y.TDL_USERNAME &&
                x.ID == y.ID;
        }

        public int GetHashCode(V_HIS_EMPLOYEE x)
        {
            return (!string.IsNullOrEmpty(x.LOGINNAME) ? x.LOGINNAME.GetHashCode() : 0) +
                (!string.IsNullOrEmpty(x.TDL_USERNAME) ? x.TDL_USERNAME.GetHashCode() : 0) +
                (x.ID != null ? x.ID.GetHashCode() : 0);
        }
    }
}
