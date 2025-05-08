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
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.UC.ExamTreatmentFinish.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.CustomControl;
using Inventec.Desktop.CustomControl.CustomGrid;
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

namespace HIS.UC.ExamTreatmentFinish.EndTypeForm
{
    public partial class FormTransfer : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private int positionHandle = -1;
        private HIS_TREATMENT hisTreatment { get; set; }
        //private MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO;
        Action<HisTreatmentFinishSDO> actEdited;
        //internal GetString MyGetData;
        Inventec.Desktop.Common.Modules.Module moduleData;

        private List<HIS_TRAN_PATI_FORM> HisTranPatiForms;
        private List<HIS_TRAN_PATI_REASON> HisTranPatiReasons;
        private List<HIS_TRAN_PATI_TECH> HisTranPatiTechs;

        List<HIS_MEDI_ORG> listMediOrg { get; set; }
        private List<HIS_TRAN_PATI_TEMP> listDataTranPatiTemp { get; set; }
        public List<AcsUserADO> lstReAcsUserADO { get; private set; }

        internal const string ModuleLink_HisTranPatiTemp = "HIS.Desktop.Plugins.HisTranPatiTemp";
        List<V_HIS_EMPLOYEE> selected = new List<V_HIS_EMPLOYEE>();
        HIS_TREATMENT_EXT currentTreatmentExt = new HIS_TREATMENT_EXT();
        #endregion

        #region Construct
        public FormTransfer(MOS.EFMODEL.DataModels.HIS_TREATMENT treatment, Action<HisTreatmentFinishSDO> _actEdited)
        {
            InitializeComponent();
            try
            {
                this.hisTreatment = treatment;
                this.actEdited = _actEdited;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormTransfer(Inventec.Desktop.Common.Modules.Module _moduleData, MOS.EFMODEL.DataModels.HIS_TREATMENT treatment, Action<HisTreatmentFinishSDO> _actEdited,HIS_TREATMENT_EXT treatmentExt)
        {
            InitializeComponent();
            try
            {
                this.moduleData = _moduleData;
                this.hisTreatment = treatment;
                this.actEdited = _actEdited;
                this.currentTreatmentExt = treatmentExt;
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
                HisTranPatiForms = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>();
                HisTranPatiReasons = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>();
                HisTranPatiTechs = BackendDataWorker.Get<HIS_TRAN_PATI_TECH>();

                //LoadKeysFromlanguage();
                SetCaptionByLanguageKey();
                LoadDataToCombo();
                //LoadDataTreatmentExt(hisTreatment);
                if (this.hisTreatment != null)
                {
                    loadDataTranPatiOld(hisTreatment);//Lấy thông tin chuyển viện cũ
                }
                LoadDataTocboUser();
                //SetDefaultValueControl();

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDataTreatmentExt(HIS_TREATMENT treatment)
        {
            try
            {
                if (treatment == null) return;
                List<HIS_TREATMENT_EXT> listTreatmentExt = null;
                MOS.Filter.HisTreatmentExtFilter filter = new MOS.Filter.HisTreatmentExtFilter();
                filter.TREATMENT_ID = treatment.ID;
                listTreatmentExt = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT_EXT>>("api/HisTreatmentExt/Get", ApiConsumers.MosConsumer, filter, null);

                if (listTreatmentExt != null) currentTreatmentExt = listTreatmentExt.FirstOrDefault();
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
                            cboMediOrgName.EditValue = mediOrgName.ID;
                            txtMediOrgCode.Text = mediOrgName.MEDI_ORG_CODE;
                            lblMediOrgAddress.Text = mediOrgName.ADDRESS;
                        }
                    }

                    if (treatment.TRAN_PATI_FORM_ID.HasValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM hisTranPatiForm = HisTranPatiForms.SingleOrDefault(o => o.ID == treatment.TRAN_PATI_FORM_ID);
                        if (hisTranPatiForm != null)
                        {
                            cboTranPatiForm.EditValue = hisTranPatiForm.ID;
                            txtTranPatiForm.Text = hisTranPatiForm.TRAN_PATI_FORM_CODE;
                        }
                    }

                    if (treatment.TRAN_PATI_REASON_ID.HasValue)
                    {
                        var tranPatiReason = HisTranPatiReasons.FirstOrDefault(o => o.ID == treatment.TRAN_PATI_REASON_ID);
                        if (tranPatiReason != null)
                        {
                            cboTranPatiReason.EditValue = tranPatiReason.ID;
                            txtTranPatiReason.Text = tranPatiReason.TRAN_PATI_REASON_CODE;
                        }
                    }

                    if (treatment.TRAN_PATI_TECH_ID.HasValue)
                    {
                        var tranPatiTech = HisTranPatiTechs.FirstOrDefault(o => o.ID == treatment.TRAN_PATI_TECH_ID);
                        if (tranPatiTech != null)
                        {
                            cboLyDoChuyenMon.EditValue = tranPatiTech.ID;
                            txtLyDoChuyenMon.Text = tranPatiTech.TRAN_PATI_TECH_CODE;
                        }
                    }
                    memPttt.Text = treatment.SURGERY_NAME;
                    if (treatment.SURGERY_BEGIN_TIME.HasValue)
                        dteBegin.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatment.SURGERY_BEGIN_TIME ?? 0).Value;

                    if (treatment.SURGERY_END_TIME.HasValue)
                        dteEnd.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatment.SURGERY_END_TIME ?? 0).Value;
                    chkValid1Year.Checked = treatment.VALID_1_YEAR == 1;
                    MOS.Filter.HisServiceReqFilter srFilter = new MOS.Filter.HisServiceReqFilter();
                    srFilter.TREATMENT_ID = treatment.ID;
                    srFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                    srFilter.ORDER_DIRECTION = "ASC";
                    srFilter.ORDER_FIELD = "INTRUCTION_TIME";
                    List<HIS_SERVICE_REQ> serviceReqs = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, srFilter, null);
                    HIS_SERVICE_REQ examServiceReq = serviceReqs != null ? serviceReqs.FirstOrDefault() : null;
                    if (examServiceReq != null)
                    {
                        txtSubclinicalResult.Text = examServiceReq.SUBCLINICAL;
                        //txtPPKTThuoc.Text = examServiceReq.TREATMENT_INSTRUCTION;
                    }
                    txtPPKTThuoc.Text = treatment.TREATMENT_METHOD;
                    Inventec.Common.Logging.LogSystem.Error("UCTREATMENT___________");
                    txtClinicalNote.Text = treatment.CLINICAL_SIGNS;
                    //if (!string.IsNullOrEmpty(treatment.SUBCLINICAL_RESULT))
                    //{
                    //    txtSubclinicalResult.Text = treatment.SUBCLINICAL_RESULT;
                    //}
                    txtTinhTrangNguoiBenh.Text = treatment.PATIENT_CONDITION;
                    txtPhuongTienVanChuyen.Text = treatment.TRANSPORT_VEHICLE;
                    if (treatment != null && !string.IsNullOrEmpty(treatment.TRANSPORTER))
                    {
                        var oldSelecteds = treatment.TRANSPORTER.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                        List<V_HIS_EMPLOYEE> newListEm = new List<V_HIS_EMPLOYEE>();
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                            if (oldSelecteds != null && oldSelecteds.Count > 0)
                            {
                                foreach(var item in oldSelecteds)
                                {
                                    V_HIS_EMPLOYEE seleceds = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => item.Equals(o.TDL_USERNAME)).FirstOrDefault();
                                    if (seleceds != null)
                                    {
                                        newListEm.Add(seleceds);
                                        selected.Add(seleceds);
                                        
                                    }
                                    else
                                    {

                                        var newEmployee = new V_HIS_EMPLOYEE
                                        {
                                            LOGINNAME = "",
                                            TDL_USERNAME = item,
                                            IS_ACTIVE = 1
                                        };
                                        selected.Add(newEmployee);
                                    }
                                        
                                }

                                gridCheckMark.SelectAll(selected);
                                string displayText = string.Join(", ", selected.Select(s => s.TDL_USERNAME).Distinct());
                                buttonEdit1.Text = displayText;
                            }
                        }
                    }
                    //if (!string.IsNullOrEmpty(treatment.TREATMENT_METHOD))
                    //{
                    //    txtPPKTThuoc.Text = treatment.TREATMENT_METHOD;
                    //}

                    txtHuongDieuTri.Text = treatment.TREATMENT_DIRECTION;
                    txtUsedMedicine.Text = treatment.USED_MEDICINE;
                    //txtUsedMedicine.Text = GetUsedMedicine(treatment.ID);
                    //if (string.IsNullOrEmpty(txtUsedMedicine.Text)
                    //    && !string.IsNullOrEmpty(treatment.USED_MEDICINE))
                    //    txtUsedMedicine.Text = treatment.USED_MEDICINE;
                    Inventec.Common.Logging.LogSystem.Debug("____Treatment: " + Inventec.Common.Logging.LogUtil.TraceData("Treatmemt:", treatment));
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
                //layout
                //this.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__TEXT");
                //this.btnSave.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__CLOSE_TREATMENT_SAVE");
                //this.lciHuongDieuTri.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_HUONG_DIEU_TRI");
                //this.lciMediOrg.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_MEDI_ORG");
                //this.lciMediOrgAddress.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_MEDI_ORG_ADDRESS");
                //this.lciNguoiHoTong.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_NGUOI_HO_TONG");
                //this.lciPhuongPhapSuDung.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_PHUONG_PHAP_SU_DUNG");
                //this.lciPhuongTienVanChuyen.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_PHUONG_TIEN_VAN_CHUYEN");
                //this.lciTinhTrangNguoiBenh.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_TINH_TRANG_NGUOI_BENH");
                //this.lciTranPatiForm.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_TRAN_PATI_FORM");
                //this.lciTranPatiReason.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__LCI_TRAN_PATI_REASON");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormTransfer
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.ExamTreatmentFinish.Resources.Lang", typeof(FormTransfer).Assembly);

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
                this.lciMediOrgAddress.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciMediOrgAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPhuongPhapSuDung.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciPhuongPhapSuDung.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());               
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FormTransfer.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTranPatiForm.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciTranPatiForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("FormTransfer.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("FormTransfer.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTinhTrangNguoiBenh.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciTinhTrangNguoiBenh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSubclinicalResult.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciSubclinicalResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHuongDieuTri.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciHuongDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciClinicalNote.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciClinicalNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTranPatiTemp.Text = Inventec.Common.Resource.Get.Value("FormTransfer.lciTranPatiTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
                //columnInfos.Add(new ColumnInfo("ICD_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ID", columnInfos, false, 250);
                //ControlEditorLoader.Load(cbo, dataIcds, controlEditorADO);
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
                string ma = "Mã";
                string ten = "Tên";

                listMediOrg = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>().OrderBy(o => o.MEDI_ORG_CODE).ToList();
                DataToComboBenhVien(listMediOrg, cboMediOrgName);

                LoadDataGridLookUpEdit(cboTranPatiReason, "TRAN_PATI_REASON_CODE", ma, "TRAN_PATI_REASON_NAME", ten, "ID", HisTranPatiReasons);
                LoadDataGridLookUpEdit(cboTranPatiForm, "TRAN_PATI_FORM_CODE", ma, "TRAN_PATI_FORM_NAME", ten, "ID", HisTranPatiForms);
                LoadDataGridLookUpEdit(this.cboLyDoChuyenMon, "TRAN_PATI_TECH_CODE", ma, "TRAN_PATI_TECH_NAME", ten, "ID", HisTranPatiTechs);

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

        private void LoadDataGridLookUpEdit(DevExpress.XtraEditors.GridLookUpEdit comboEdit, string code, string captionCode, string name, string captionName, string value, object data)
        {
            try
            {
                comboEdit.Properties.DataSource = data;
                comboEdit.Properties.DisplayMember = name;
                comboEdit.Properties.ValueMember = value;
                comboEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                comboEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                comboEdit.Properties.ImmediatePopup = true;
                comboEdit.ForceInitialize();
                comboEdit.Properties.View.Columns.Clear();
                comboEdit.Properties.PopupFormSize = new System.Drawing.Size(300, 250);

                GridColumn aColumnCode = comboEdit.Properties.View.Columns.AddField(code);
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = comboEdit.Properties.View.Columns.AddField(name);
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void SetDefaultValueControl()
        //{
        //    try
        //    {
        //        if (currentTreatmentFinishSDO != null)
        //        {
        //            var mediOrgName = listMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == currentTreatmentFinishSDO.TransferOutMediOrgCode);
        //            if (mediOrgName != null)
        //            {
        //                cboMediOrgName.EditValue = mediOrgName.MEDI_ORG_CODE;
        //                txtMediOrgCode.Text = mediOrgName.MEDI_ORG_CODE;
        //                lblMediOrgAddress.Text = mediOrgName.ADDRESS;
        //            }

        //            var hisTranPatiReason = HisTranPatiReasons.SingleOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiReasonId);
        //            if (hisTranPatiReason != null)
        //            {
        //                cboTranPatiReason.EditValue = hisTranPatiReason.ID;
        //                txtTranPatiReason.Text = hisTranPatiReason.TRAN_PATI_REASON_CODE;
        //            }
        //            var hisTranPatiTech = HisTranPatiTechs.SingleOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiTechId);
        //            if (hisTranPatiTech != null)
        //            {
        //                cboLyDoChuyenMon.EditValue = hisTranPatiTech.ID;
        //                txtLyDoChuyenMon.Text = hisTranPatiTech.TRAN_PATI_TECH_CODE;
        //            }

        //            var hisTranPatiForm = HisTranPatiForms.SingleOrDefault(o => o.ID == currentTreatmentFinishSDO.TranPatiFormId);
        //            if (hisTranPatiForm != null)
        //            {
        //                cboTranPatiForm.EditValue = hisTranPatiForm.ID;
        //                txtTranPatiForm.Text = hisTranPatiForm.TRAN_PATI_FORM_CODE;
        //            }
        //            txtTinhTrangNguoiBenh.Text = currentTreatmentFinishSDO.PatientCondition;
        //            txtPhuongTienVanChuyen.Text = currentTreatmentFinishSDO.TransportVehicle;
        //            txtNguoiHoTong.Text = currentTreatmentFinishSDO.Transporter;
        //            txtPPKTThuoc.Text = currentTreatmentFinishSDO.TreatmentMethod;
        //            txtHuongDieuTri.Text = currentTreatmentFinishSDO.TreatmentDirection;
        //            if (currentTreatmentFinishSDO.TreatmentId > 0)
        //                txtUsedMedicine.Text = GetUsedMedicine(currentTreatmentFinishSDO.TreatmentId);
        //            if (string.IsNullOrEmpty(txtUsedMedicine.Text)
        //                && !string.IsNullOrEmpty(currentTreatmentFinishSDO.UsedMedicine))
        //                txtUsedMedicine.Text = currentTreatmentFinishSDO.UsedMedicine;
        //        }
        //        else
        //        {
        //            txtMediOrgCode.Text = "";
        //            cboMediOrgName.EditValue = null;
        //            cboMediOrgName.Properties.Buttons[1].Visible = false;
        //            cboTranPatiReason.EditValue = null;
        //            cboTranPatiReason.Properties.Buttons[1].Visible = false;
        //            txtTranPatiReason.Text = "";
        //            cboTranPatiForm.EditValue = null;
        //            cboTranPatiForm.Properties.Buttons[1].Visible = false;
        //            txtTranPatiForm.Text = "";
        //            txtTinhTrangNguoiBenh.Text = "";
        //            txtPhuongTienVanChuyen.Text = "";
        //            txtNguoiHoTong.Text = "";
        //            txtHuongDieuTri.Text = "";
        //        }

        //        txtMediOrgCode.Focus();
        //        txtMediOrgCode.SelectAll();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private string GetUsedMedicine(long treatmentId)
        {
            string rs = "";
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter filter = new HisExpMestMedicineViewFilter();
                filter.TDL_TREATMENT_ID = treatmentId;
                filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;

                var expMestMedicine = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);

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
                ValidationControlMaxLength(txtPPKTThuoc, 3000);
                ValidationControlMaxLength(txtHuongDieuTri, 3000);
                ValidationControlMaxLength(txtTinhTrangNguoiBenh, 3000);
                ValidationControlMaxLength(txtPhuongTienVanChuyen, 3000);
                //ValidationControlMaxLength(txtNguoiHoTong, 200);
                ValidationControlMaxLength(txtUsedMedicine, 3000);
                ValidationControlMaxLength(txtClinicalNote, 3000);
                ValidationControlMaxLength(txtSubclinicalResult, 3000);

                ValidationRequired(txtClinicalNote);
                ValidationRequired(txtHuongDieuTri);
                ValidationRequired(txtPPKTThuoc);
                ValidationRequired(txtPhuongTienVanChuyen);
                ValidationRequired(buttonEdit1);
                ValidationRequiredCombo(cboTransporterLoginName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationRequiredCombo(GridLookUpEdit cboTransporterLoginName)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validate = new GridLookupEditWithTextEditValidationRule();
                validate.cbo = cboTransporterLoginName;
                validate.txtTextEdit = cboTransporterLoginName;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(cboTransporterLoginName, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationRequired(BaseEdit control)
        {
            try
            {
                
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = control;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(control, validate);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                this.dxValidationProvider.SetValidationRule(this.txtMediOrgCode, validate);
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
                GridLookupEditWithTextEditValidationRule validate = new GridLookupEditWithTextEditValidationRule();
                validate.cbo = this.cboTranPatiReason;
                validate.txtTextEdit = this.txtTranPatiReason;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(this.txtTranPatiReason, validate);
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
                GridLookupEditWithTextEditValidationRule validate = new GridLookupEditWithTextEditValidationRule();
                validate.cbo = this.cboTranPatiForm;
                validate.txtTextEdit = this.txtTranPatiForm;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(this.txtTranPatiForm, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.ErrorText = String.Format(Resources.ResourceMessage.TruongDuLieuVuotQuaKyTu, maxLength);
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProvider.SetValidationRule(control, validate);
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
                        var listData = HisTranPatiReasons.Where(o => o.TRAN_PATI_REASON_CODE.Contains(code)).ToList();
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
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = HisTranPatiReasons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString()));
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
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = HisTranPatiReasons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiReason.Text = data.TRAN_PATI_REASON_CODE;
                            txtLyDoChuyenMon.Focus();
                            txtLyDoChuyenMon.SelectAll();
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
                        var listData = HisTranPatiForms.Where(o => o.TRAN_PATI_FORM_CODE.Contains(code)).ToList();
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
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = HisTranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiForm.Text = data.TRAN_PATI_FORM_CODE;
                            txtHuongDieuTri.Focus();
                            txtHuongDieuTri.SelectAll();
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
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = HisTranPatiForms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString()));
                        if (data != null)
                        {
                            txtTranPatiForm.Text = data.TRAN_PATI_FORM_CODE;
                            txtHuongDieuTri.Focus();
                            txtHuongDieuTri.SelectAll();
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
                buttonEdit1_Validated(buttonEdit1, null);
                this.positionHandle = -1;
                if (!dxValidationProvider.Validate()) return;

                if (actEdited == null) return;

                HisTreatmentFinishSDO currentTreatmentFinishSDO = new HisTreatmentFinishSDO();
                currentTreatmentFinishSDO.TreatmentId = hisTreatment.ID;

                MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON hisTranPatiReason = HisTranPatiReasons.SingleOrDefault(o => o.ID == (long)cboTranPatiReason.EditValue);
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
                currentTreatmentFinishSDO.TransporterLoginnames = selected != null && selected.Count > 0 ? string.Join(";", selected.Where(s=> !string.IsNullOrEmpty(s.LOGINNAME)).Select(o=>o.LOGINNAME).Distinct()) : null;
                currentTreatmentFinishSDO.Transporter = selected != null && selected.Count > 0 ? string.Join(";", selected.Select(o => o.TDL_USERNAME).Distinct()) : null;
                currentTreatmentFinishSDO.TreatmentMethod = txtPPKTThuoc.Text;
                currentTreatmentFinishSDO.TreatmentDirection = txtHuongDieuTri.Text;
                currentTreatmentFinishSDO.UsedMedicine = txtUsedMedicine.Text;
                currentTreatmentFinishSDO.ClinicalSigns = txtClinicalNote.Text;
                currentTreatmentFinishSDO.SubclinicalResult = txtSubclinicalResult.Text;
                if (cboLoginName.EditValue != null)
                {
                    currentTreatmentFinishSDO.TranPatiHospitalLoginname = cboLoginName.EditValue.ToString();
                    currentTreatmentFinishSDO.TranPatiHospitalUsername = cboLoginName.Text.ToString();
                }
                currentTreatmentFinishSDO.SurgeryName = memPttt.Text.Trim();
                if(dteBegin.EditValue != null && dteBegin.DateTime != DateTime.MinValue)
                    currentTreatmentFinishSDO.SurgeryBeginTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteBegin.DateTime);
                if (dteEnd.EditValue != null && dteEnd.DateTime != DateTime.MinValue)
                    currentTreatmentFinishSDO.SurgeryEndTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteEnd.DateTime);
                currentTreatmentFinishSDO.Valid1Year = chkValid1Year.Checked;
                actEdited(currentTreatmentFinishSDO);
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
                        var listData = HisTranPatiTechs.Where(o => o.TRAN_PATI_TECH_CODE.Contains(code)).ToList();
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
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH data = HisTranPatiTechs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboLyDoChuyenMon.EditValue.ToString()));
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
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH data = HisTranPatiTechs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboLyDoChuyenMon.EditValue.ToString()));
                        if (data != null)
                        {
                            txtLyDoChuyenMon.Text = data.TRAN_PATI_TECH_CODE;
                            txtTranPatiForm.Focus();
                            txtTranPatiForm.SelectAll();
                            cboLyDoChuyenMon.Properties.Buttons[1].Visible = true;
                        }
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
                    txtTranPatiForm.Text = HisTranPatiForms.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE).TRAN_PATI_FORM_CODE;
                }
                else if ((L1 - L2) > 1)
                {
                    cboTranPatiForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE;
                    txtTranPatiForm.Text = HisTranPatiForms.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE).TRAN_PATI_FORM_CODE;
                }
                else if ((L1 - L2) < 0)
                {
                    cboTranPatiForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG;
                    txtTranPatiForm.Text = HisTranPatiForms.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG).TRAN_PATI_FORM_CODE;
                }
                else if ((L1 - L2) == 0)
                {
                    cboTranPatiForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN;
                    txtTranPatiForm.Text = HisTranPatiForms.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN).TRAN_PATI_FORM_CODE;
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
                var tranPatiTech = cboLyDoChuyenMon.EditValue != null ? this.HisTranPatiTechs.FirstOrDefault(o => o.ID == Convert.ToInt64(cboLyDoChuyenMon.EditValue)) : null;
                if (tranPatiTech != null)
                    data.TRAN_PATI_TECH_ID = tranPatiTech.ID;

                data.TREATMENT_DIRECTION = txtHuongDieuTri.Text;
                data.PATIENT_CONDITION = txtTinhTrangNguoiBenh.Text;
                data.TREATMENT_METHOD = txtPPKTThuoc.Text;
                data.TRANSPORT_VEHICLE = txtPhuongTienVanChuyen.Text;
                data.USED_MEDICINE = txtUsedMedicine.Text;
                //data.TRANSPORTER = txtNguoiHoTong.Text;

                List<object> listArgs = new List<object>();
                listArgs.Add(this.moduleData);
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
                var tranPatiReason = this.HisTranPatiReasons.FirstOrDefault(o => o.ID == data.TRAN_PATI_REASON_ID);
                cboTranPatiReason.EditValue = tranPatiReason != null ? (long?)tranPatiReason.ID : null;
                txtTranPatiReason.Text = tranPatiReason != null ? tranPatiReason.TRAN_PATI_REASON_CODE : "";
                var tranPatiForm = this.HisTranPatiForms.FirstOrDefault(o => o.ID == data.TRAN_PATI_FORM_ID);
                cboTranPatiForm.EditValue = tranPatiForm != null ? (long?)tranPatiForm.ID : null;
                txtTranPatiForm.Text = tranPatiForm != null ? tranPatiForm.TRAN_PATI_FORM_CODE : "";
                var tranPatiTech = this.HisTranPatiTechs.FirstOrDefault(o => o.ID == data.TRAN_PATI_TECH_ID);
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
                //var Employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().Where(o=>o.IS_ACTIVE == 1);
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
                if (hisTreatment != null && !string.IsNullOrEmpty(hisTreatment.TRANSPORTER))
                {
                    var oldSelecteds = hisTreatment.TRANSPORTER.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                    List<V_HIS_EMPLOYEE> newListEm = new List<V_HIS_EMPLOYEE>();
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                        if (oldSelecteds != null && oldSelecteds.Count > 0)
                        {
                            foreach (var item in oldSelecteds)
                            {
                                V_HIS_EMPLOYEE seleceds = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => item.Equals(o.TDL_USERNAME)).FirstOrDefault();
                                if (seleceds != null)
                                {
                                    newListEm.Add(seleceds);
                                    selected.Add(seleceds);

                                }
                                else
                                {

                                    var newEmployee = new V_HIS_EMPLOYEE
                                    {
                                        LOGINNAME = "",
                                        TDL_USERNAME = item,
                                        IS_ACTIVE = 1
                                    };
                                    selected.Add(newEmployee);
                                }

                            }

                            gridCheckMark.SelectAll(selected);
                            string displayText = string.Join(", ", selected.Select(s => s.TDL_USERNAME).Distinct());
                            buttonEdit1.Text = displayText;
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
                    this.cboTransporterLoginName.Focus();
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
                List<V_HIS_EMPLOYEE> listOldSelected = null;
                if(selected!= null && selected.Count> 0){
                    listOldSelected = selected.Where(s => string.IsNullOrEmpty(s.LOGINNAME)).Distinct().ToList();
                }

                selected = new List<V_HIS_EMPLOYEE>();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                

                
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
                //if (listOldSelected != null && listOldSelected.Count > 0)
                //{
                //    selected.AddRange(listOldSelected);
                //}
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => selected.Count), selected.Count));
                this.buttonEdit1.Text = String.Join(", ", this.selected.Select(s => s.TDL_USERNAME).Distinct().ToList());
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

        private void cboTransporterLoginName_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {

                string displayText = "";

                if (this.selected != null && this.selected.Count > 0)
                {
                    displayText = String.Join(", ", this.selected.Select(s => s.TDL_USERNAME).Distinct().ToList());
                }

                e.DisplayText = displayText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        

        private void cboTransporterLoginName_EditValueChanging(object sender, ChangingEventArgs e)
        {
            
        }
        private List<V_HIS_EMPLOYEE> Employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == 1).ToList();
        private void cboTransporterLoginName_ProcessNewValue(object sender, DevExpress.XtraEditors.Controls.ProcessNewValueEventArgs e)
        {
            try
            {
                //var Employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == 1).ToList();
                if (!string.IsNullOrEmpty(e.DisplayValue.ToString()))
                {
                    // Tách các giá trị được nhập vào
                    var enteredValues = e.DisplayValue.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    // Danh sách để chứa các đối tượng mới
                    List<AcsUserADO> newUsers = new List<AcsUserADO>();
                    List<V_HIS_EMPLOYEE> newEmployees = new List<V_HIS_EMPLOYEE>();
                    string loginname = "user";
                    int index = 0;
                    foreach (var value in enteredValues)
                    {
                        var trimmedValue = value.Trim();
                        if (!string.IsNullOrEmpty(trimmedValue))
                        {
                            // Kiểm tra nếu giá trị nhập vào không có trong danh sách
                            var newUser = lstReAcsUserADO.FirstOrDefault(emp => emp.USERNAME.Equals(trimmedValue, StringComparison.OrdinalIgnoreCase));
                            var ExitsEmployee = Employees.FirstOrDefault(s => !string.IsNullOrEmpty(s.TDL_USERNAME) && s.TDL_USERNAME.Equals(trimmedValue));
                            if (ExitsEmployee == null)
                            {
                                // Tạo đối tượng mới và thêm vào danh sách lstReAcsUserADO
                                var newUserAcs = new AcsUserADO
                                {
                                    LOGINNAME = loginname+index,
                                    USERNAME = trimmedValue,
                                    IS_ACTIVE = 1
                                };
                                newUsers.Add(newUserAcs);
                                //lstReAcsUserADO.Add(newUserAcs);

                                // Tạo đối tượng mới và thêm vào danh sách Employees
                                var newEmployee = new V_HIS_EMPLOYEE
                                {
                                    LOGINNAME = "",
                                    TDL_USERNAME = trimmedValue,
                                    IS_ACTIVE = 1
                                };
                                newEmployees.Add(newEmployee);
                                Employees.Add(newEmployee);
                                if (this.selected == null)
                                {
                                    this.selected = new List<V_HIS_EMPLOYEE>();
                                }
                                // Thêm vào danh sách selected
                                this.selected.Add(newEmployee);
                                index++;
                            }
                            else if (ExitsEmployee != null)
                            {
                                this.selected.Add(ExitsEmployee);
                            }
                        }
                    }
                    GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                    //gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                    gridCheckMark.SelectAll(selected);
                    string displayText = String.Join(", ", selected.Select(s => s.TDL_USERNAME).Distinct().ToList());
                    cboTransporterLoginName.Text = displayText;

                    
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTransporterLoginName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //var gridLookUpEdit = sender as GridLookUpEdit;
                //var view = gridLookUpEdit.Properties.View;
    
                //if (view != null)
                //{
                //    var selectedHandles = view.GetSelectedRows();
                //    this.selected = selectedHandles.Select(view.GetRow).OfType<V_HIS_EMPLOYEE>().ToList();
        
                //    string displayText = String.Join(", ", this.selected.Select(s => s.TDL_USERNAME).Distinct().ToList());
                //    gridLookUpEdit.Text = displayText;
                //}
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void buttonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
                {
                    
                    cboTransporterLoginName.ShowPopup();
                }
                else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    
                    cboTransporterLoginName.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                    }
                    this.cboTransporterLoginName.Focus();
                    buttonEdit1.Text = "";
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTransporterLoginName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (this.selected != null)
                {
                    string displayText = String.Join(", ", selected.Select(s => s.TDL_USERNAME).Distinct().ToList());
                    buttonEdit1.Text = displayText;
                }
            }
            catch (Exception ex)
            {
                
                 Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void buttonEdit_EditValueChanged(object sender, EventArgs e)
        {

        }
        private bool isUpdatingValue = false;
        private void buttonEdit1_Validated(object sender, EventArgs e)
        {

            try
            {
                if (isUpdatingValue)
                    return;

                var edit = sender as DevExpress.XtraEditors.ButtonEdit;
                if (edit != null && !string.IsNullOrEmpty(edit.Text))
                {
                    try
                    {
                        selected.Clear();
                        isUpdatingValue = true; // Đặt cờ để ngăn chặn vòng lặp sự kiện

                        // Tách các giá trị được nhập vào
                        var enteredValues = edit.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                     .Select(v => v.Trim()) // Loại bỏ khoảng trắng thừa
                                                     .Distinct() // Loại bỏ các giá trị trùng lặp
                                                     .ToList();

                        //int index = 1;
                        foreach (var value in enteredValues)
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                // Kiểm tra nếu giá trị nhập vào không có trong danh sách
                                var employee = Employees.FirstOrDefault(s => !string.IsNullOrEmpty(s.TDL_USERNAME) && s.TDL_USERNAME.Equals(value));
                                if (employee != null && !selected.Contains(employee))
                                {
                                    // Thêm vào danh sách selected nếu chưa có
                                    selected.Add(employee);
                                }
                                else
                                {
                                    var newEmployee = new V_HIS_EMPLOYEE
                                    {
                                        LOGINNAME = "",
                                        TDL_USERNAME = value,
                                        IS_ACTIVE = 1
                                    };
                                    selected.Add(newEmployee);
                                }

                            }
                        }

                        edit.Text = string.Join(", ", selected.Select(s => s.TDL_USERNAME).Distinct());
                    }
                    finally
                    {
                        isUpdatingValue = false; // Reset cờ sau khi hoàn thành
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void buttonEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter)
                {
                    txtHuongDieuTri.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void memPttt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ListSurgMisuByTreatment").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ListSurgMisuByTreatment");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null && hisTreatment != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(hisTreatment.ID);
                        listArgs.Add(new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT });
                        listArgs.Add((HIS.Desktop.Common.DelegateLoadPTTT)UpdateData);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateData(string namePTTT, DateTime? startTime, DateTime? finishTime)
        {
            try
            {
                memPttt.Text = namePTTT;
                if (startTime.HasValue)
                    dteBegin.DateTime = startTime.Value;
                else
                    dteBegin.EditValue = null;
                if (finishTime.HasValue)
                    dteEnd.DateTime = finishTime.Value;
                else
                    dteEnd.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
