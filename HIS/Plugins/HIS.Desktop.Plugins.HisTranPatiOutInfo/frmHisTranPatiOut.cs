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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisTranPatiOutInfo.ProcessLoadDataCombo;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTranPatiOutInfo
{
    public partial class frmHisTranPatiOutInfo : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;

        internal IcdProcessor icdProcessorTo;
        internal UserControl ucIcdToTranfer;

        internal HIS_TREATMENT currentTreatment { get; set; }
        HIS_BRANCH currentBranch = new HIS_BRANCH();
        List<HIS_MEDI_ORG> VHisHeinMediOrg = new List<HIS_MEDI_ORG>();
        List<HIS_TRAN_PATI_FORM> VHisTranPatiForm = new List<HIS_TRAN_PATI_FORM>();
        List<V_HIS_EMPLOYEE> selected = new List<V_HIS_EMPLOYEE>();

        public frmHisTranPatiOutInfo()
        {
            InitializeComponent();
        }

        public frmHisTranPatiOutInfo(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                InitUcIcdToTranfer();
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

        private void InitUcIcdToTranfer()
        {
            try
            {
                icdProcessorTo = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DelegateNextFocus = DelegateNextFocusICD;
                ado.Width = 440;
                ado.Height = 24;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>();

                this.ucIcdToTranfer = (UserControl)icdProcessorTo.Run(ado);

                if (this.ucIcdToTranfer != null)
                {
                    this.layoutControlUcIcdToTranfer.Controls.Add(this.ucIcdToTranfer);
                    this.ucIcdToTranfer.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void DelegateNextFocusICD()
        {
            try
            {
                txtIcdExtraName.Focus();
                txtIcdExtraName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void frmHisTranPatiOutInfo_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCurrentBranch();
                VHisHeinMediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>();
                var VHisTranPatiReason = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>();
                VHisTranPatiForm = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>();
                InitComboTransporterLoginNameCheck();
                SetCaptionByLanguageKey();
                
                //ToTranfer
                ProcessLoad.LoadDataToComboMediOrg(cboMediOrgNameTo, VHisHeinMediOrg);
                ProcessLoad.LoadDataToComboTranPatiReason(cboTranPatiReasonTo, VHisTranPatiReason);
                ProcessLoad.LoadDataToComboTranPatiForm(cboTranPatiFormTo, VHisTranPatiForm);

                LoadDataTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentBranch()
        {
            var _workPlace = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModuleBase.RoomId);
            CommonParam param = new CommonParam();
            HisBranchFilter filter = new HisBranchFilter();
            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
            var VHisBranch = new BackendAdapter(param).Get<List<HIS_BRANCH>>("api/HisBranch/Get", ApiConsumers.MosConsumer, filter, param);
            if (VHisBranch != null && VHisBranch.Count > 0)
                currentBranch = VHisBranch.FirstOrDefault();

        }

        private void LoadDataTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                filter.ID = this.treatmentId;
                this.currentTreatment = new HIS_TREATMENT();
                this.currentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (this.currentTreatment != null && this.currentTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                {
                    //Review
                    // if (this.currentTreatment.TRANSFER_IN_CMKT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.TRANSFER_IN_CMKT__TT)
                    // {
                    FillDataToControlTranPatiToTranfer(this.currentTreatment);
                    // }
                }
                else
                {
                    btnEdit.Enabled = false;
                    SetDefaultValueControlToTranPati();
                }
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisTranPatiOutInfo.Resources.Lang", typeof(HIS.Desktop.Plugins.HisTranPatiOutInfo.frmHisTranPatiOutInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Edit.Caption = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.barButtonItem__Edit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.barButtonItem__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Cancel.Caption = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.barButtonItem__Cancel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlUcIcdToTranfer.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControlUcIcdToTranfer.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediOrgNameTo.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.cboMediOrgNameTo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTranPatiFormTo.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.cboTranPatiFormTo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTranPatiReasonTo.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.cboTranPatiReasonTo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtIcdExtraName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.txtIcdExtraNames.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisTranPatiOut.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControlTranPatiToTranfer(HIS_TREATMENT data)
        {
            try
            {
                if (data != null)
                {
                    if (!String.IsNullOrEmpty(data.MEDI_ORG_CODE))
                    {
                        cboMediOrgNameTo.EditValue = data.MEDI_ORG_CODE;
                        txtMediOrgCodeTo.Text = data.MEDI_ORG_CODE;
                    }

                    MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON hisTranPatiReason = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == data.TRAN_PATI_REASON_ID);
                    if (hisTranPatiReason != null)
                    {
                        cboTranPatiReasonTo.EditValue = hisTranPatiReason.ID;
                        txtTranPatiReasonTo.Text = hisTranPatiReason.TRAN_PATI_REASON_CODE;
                    }

                    MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM hisTranPatiForm = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().SingleOrDefault(o => o.ID == data.TRAN_PATI_FORM_ID);
                    if (hisTranPatiForm != null)
                    {
                        cboTranPatiFormTo.EditValue = hisTranPatiForm.ID;
                        txtTranPatiFormTo.Text = hisTranPatiForm.TRAN_PATI_FORM_CODE;
                    }

                    //ICD
                    IcdInputADO inputIcd = new IcdInputADO();
                    inputIcd.ICD_NAME = data.ICD_NAME;
                    inputIcd.ICD_CODE = data.ICD_CODE;
                    if (ucIcdToTranfer != null)
                    {
                        icdProcessorTo.Reload(ucIcdToTranfer, inputIcd);
                    }
                    txtIcdExtraName.Text = data.ICD_TEXT;
                    txtIcdExtraCode.Text = data.ICD_SUB_CODE;

                    txtDauHieuLamSang.Text = data.CLINICAL_NOTE;
                    txtXetNghiem.Text = data.SUBCLINICAL_RESULT;
                    txtPPKTThuoc.Text = data.TREATMENT_METHOD;
                    txtTinhTrangNguoiBenh.Text = data.PATIENT_CONDITION;
                    txtHuongDieuTri.Text = data.TREATMENT_DIRECTION;
                    txtPhuongTienVanChuyen.Text = data.TRANSPORT_VEHICLE;
                    //txtNguoiHoTong.Text = data.TRANSPORTER;
                    var Employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == 1);
                    InitComboTransporterLoginName(Employees.ToList());
                    if (data != null && !string.IsNullOrEmpty(data.TRANSPORTER_LOGINNAMES))
                    {
                        var oldSelecteds = data.TRANSPORTER_LOGINNAMES.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        GridCheckMarksSelection gridCheckMark = cboTransporterLoginName.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.ClearSelection(cboTransporterLoginName.Properties.View);
                            if (oldSelecteds != null && oldSelecteds.Count > 0)
                            {
                                List<V_HIS_EMPLOYEE> seleceds = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => oldSelecteds.Contains(o.LOGINNAME)).ToList();
                                gridCheckMark.SelectAll(seleceds);

                                string displayText = String.Join(", ", seleceds.Select(s => s.TDL_USERNAME).ToList());
                                cboTransporterLoginName.Text = displayText;
                            }
                        }
                    }
                    lblSoChuyenVien.Text = data.OUT_CODE;

                    SetReadOnlyControlToTranPati(true);
                    btnEdit.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediOrgCodeTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    ProcessLoad.LoadNoiDKKCBBDCombo(strValue, false, cboMediOrgNameTo, txtMediOrgCodeTo, null, txtTranPatiReasonTo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediOrgNameTo_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMediOrgNameTo.EditValue = null;
                    txtMediOrgCodeTo.Text = "";
                    //lblMediOrgAddress.Text = "";
                    txtMediOrgCodeTo.Focus();
                    txtMediOrgCodeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediOrgNameTo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMediOrgNameTo.EditValue != null)
                    {
                        HIS_MEDI_ORG data = BackendDataWorker.Get<HIS_MEDI_ORG>().SingleOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgNameTo.EditValue);
                        if (data != null)
                        {
                            txtMediOrgCodeTo.Text = data.MEDI_ORG_CODE;
                            // lblMediOrgAddress.Text = data.Address;
                            txtTranPatiReasonTo.Focus();
                            txtTranPatiReasonTo.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediOrgNameTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMediOrgNameTo.EditValue != null)
                    {
                        HIS_MEDI_ORG data = BackendDataWorker.Get<HIS_MEDI_ORG>().SingleOrDefault(o => o.MEDI_ORG_CODE == txtMediOrgCodeTo.Text);
                        if (data != null)
                        {
                            txtMediOrgCodeTo.Text = data.MEDI_ORG_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTranPatiReasonTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    ProcessLoad.LoadComboTranPatiReason(strValue, false, cboTranPatiReasonTo, txtTranPatiReasonTo, txtTranPatiFormTo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTranPatiReasonTo_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTranPatiReasonTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTranPatiReasonTo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTranPatiReasonTo.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == (long)cboTranPatiReasonTo.EditValue);
                        if (data != null)
                        {
                            txtTranPatiReasonTo.Text = data.TRAN_PATI_REASON_CODE;
                            txtTranPatiFormTo.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTranPatiReasonTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTranPatiReasonTo.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == (long)cboTranPatiReasonTo.EditValue);
                        if (data != null)
                        {
                            cboTranPatiFormTo.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTranPatiFormTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    ProcessLoad.LoadComboTranPatiForm(strValue, false, cboTranPatiFormTo, txtTranPatiFormTo, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTranPatiFormTo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTranPatiFormTo.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().SingleOrDefault(o => o.ID == (long)cboTranPatiFormTo.EditValue);
                        if (data != null)
                        {
                            txtTranPatiFormTo.Text = data.TRAN_PATI_FORM_CODE;
                            if (ucIcdToTranfer != null)
                            {
                                icdProcessorTo.FocusControl(ucIcdToTranfer);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdTextTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDauHieuLamSang.Focus();
                    txtDauHieuLamSang.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetReadOnlyControlToTranPati(bool isReadOnly)
        {
            try
            {
                txtMediOrgCodeTo.ReadOnly = isReadOnly;
                cboMediOrgNameTo.ReadOnly = isReadOnly;
                txtTranPatiFormTo.ReadOnly = isReadOnly;
                cboTranPatiFormTo.ReadOnly = isReadOnly;
                txtTranPatiReasonTo.ReadOnly = isReadOnly;
                cboTranPatiReasonTo.ReadOnly = isReadOnly;
                txtIcdExtraName.ReadOnly = isReadOnly;
                txtIcdExtraCode.ReadOnly = isReadOnly;
                lblSoChuyenVien.Enabled = !isReadOnly;
                if (ucIcdToTranfer != null)
                {
                    icdProcessorTo.ReadOnly(ucIcdToTranfer, isReadOnly);
                }
                txtDauHieuLamSang.ReadOnly = isReadOnly;
                txtXetNghiem.ReadOnly = isReadOnly;
                txtPPKTThuoc.ReadOnly = isReadOnly;
                txtTinhTrangNguoiBenh.ReadOnly = isReadOnly;
                txtHuongDieuTri.ReadOnly = isReadOnly;
                txtPhuongTienVanChuyen.ReadOnly = isReadOnly;
                lblNguoiVanChuyen.Enabled = !isReadOnly;
                //txtNguoiHoTong.ReadOnly = isReadOnly;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControlToTranPati()
        {
            try
            {
                txtMediOrgCodeTo.Text = null;
                cboMediOrgNameTo.EditValue = null;
                txtTranPatiFormTo.Text = null;
                cboTranPatiFormTo.EditValue = null;
                txtTranPatiReasonTo.Text = null;
                cboTranPatiReasonTo.EditValue = null;
                txtIcdExtraName.Text = null;
                txtIcdExtraCode.Text = null;
                if (ucIcdToTranfer != null)
                {
                    icdProcessorTo.Reload(ucIcdToTranfer, null);
                }
                txtDauHieuLamSang.Text = null;
                txtXetNghiem.Text = null;
                txtPPKTThuoc.Text = null;
                txtTinhTrangNguoiBenh.Text = null;
                txtHuongDieuTri.Text = null;
                txtPhuongTienVanChuyen.Text = null;
                cboTransporterLoginName = null;
                //txtNguoiHoTong.Text = null;
                SetReadOnlyControlToTranPati(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                txtMediOrgCodeTo.Focus();
                txtMediOrgCodeTo.SelectAll();
                btnCancel.Enabled = true;
                btnEdit.Enabled = false;
                btnSave.Enabled = true;

                //1. Readonly tất cả control
                SetReadOnlyControlToTranPati(false);
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
                //Review
                bool success = false;
                CommonParam param = new CommonParam();
                HIS_TREATMENT _treatmentUpdate = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(_treatmentUpdate, this.currentTreatment);
                MOS.SDO.HisTreatmentTranPatiSDO sdoUpdate = new MOS.SDO.HisTreatmentTranPatiSDO();
                sdoUpdate.IsTranIn = false;
                sdoUpdate.HisTreatment = new HIS_TREATMENT();
                if (cboTranPatiReasonTo.EditValue != null)
                {
                    _treatmentUpdate.TRAN_PATI_REASON_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboTranPatiReasonTo.EditValue ?? "0").ToString());
                }
                if (cboTranPatiFormTo.EditValue != null)
                {
                    _treatmentUpdate.TRAN_PATI_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboTranPatiFormTo.EditValue ?? "0").ToString());
                }
                if (ucIcdToTranfer != null)
                {
                    var OjecIcd = icdProcessorTo.GetValue(ucIcdToTranfer);
                    if (OjecIcd is IcdInputADO)
                    {
                        _treatmentUpdate.ICD_NAME = ((IcdInputADO)OjecIcd).ICD_NAME;
                        _treatmentUpdate.ICD_CODE = ((IcdInputADO)OjecIcd).ICD_CODE;

                        sdoUpdate.IcdName = ((IcdInputADO)OjecIcd).ICD_NAME;
                        sdoUpdate.IcdCode = ((IcdInputADO)OjecIcd).ICD_CODE;
                    }
                }
                _treatmentUpdate.ICD_SUB_CODE = txtIcdExtraCode.Text;
                _treatmentUpdate.ICD_TEXT = txtIcdExtraName.Text;

                sdoUpdate.IcdSubCode = txtIcdExtraCode.Text;
                sdoUpdate.IcdText = txtIcdExtraName.Text;

                HIS_MEDI_ORG mediOrgData = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == txtMediOrgCodeTo.Text.Trim());
                if (mediOrgData != null)
                {
                    _treatmentUpdate.MEDI_ORG_CODE = mediOrgData.MEDI_ORG_CODE;
                    _treatmentUpdate.MEDI_ORG_NAME = mediOrgData.MEDI_ORG_NAME;
                }
                _treatmentUpdate.OUT_CODE = lblSoChuyenVien.Text;
                _treatmentUpdate.ICD_TEXT = txtIcdExtraName.Text;
                _treatmentUpdate.ICD_SUB_CODE = txtIcdExtraCode.Text;
                _treatmentUpdate.CLINICAL_NOTE = txtDauHieuLamSang.Text;
                _treatmentUpdate.SUBCLINICAL_RESULT = txtXetNghiem.Text;
                _treatmentUpdate.TREATMENT_METHOD = txtPPKTThuoc.Text;
                _treatmentUpdate.PATIENT_CONDITION = txtTinhTrangNguoiBenh.Text;
                _treatmentUpdate.TREATMENT_DIRECTION = txtHuongDieuTri.Text;
                _treatmentUpdate.TRANSPORT_VEHICLE = txtPhuongTienVanChuyen.Text;
                //_treatmentUpdate.TRANSPORTER = txtNguoiHoTong.Text;
                _treatmentUpdate.TRANSPORTER_LOGINNAMES = selected != null && selected.Count > 0 ? string.Join(";", selected.Select(o => o.LOGINNAME)) : null;
                _treatmentUpdate.TRANSPORTER = selected != null && selected.Count > 0 ? string.Join(";", selected.Select(o => o.TDL_USERNAME)) : null;
                sdoUpdate.HisTreatment = _treatmentUpdate;

                var rs = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateTranPatiInfo", ApiConsumers.MosConsumer, sdoUpdate, param);
                if (rs != null)
                {
                    success = true;
                    this.currentTreatment = rs;
                    FillDataToControlTranPatiToTranfer(this.currentTreatment);
                    //Nếu thành công
                    btnEdit.Enabled = true;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    //Disable
                    SetReadOnlyControlToTranPati(true);
                }

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void CodeOld()
        //{
        //    bool success = false;
        //    CommonParam param = new CommonParam();
        //    //Lưu
        //    MOS.EFMODEL.DataModels.HIS_TRAN_PATI tranPatiUpdate = new HIS_TRAN_PATI();
        //    Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI, MOS.EFMODEL.DataModels.HIS_TRAN_PATI>();
        //    tranPatiUpdate = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI, MOS.EFMODEL.DataModels.HIS_TRAN_PATI>(tranPatiToTranfer);
        //    if (cboTranPatiReasonTo.EditValue != null)
        //    {
        //        tranPatiUpdate.TRAN_PATI_REASON_ID = (long)cboTranPatiReasonTo.EditValue;
        //    }
        //    if (cboTranPatiFormTo.EditValue != null)
        //    {
        //        tranPatiUpdate.TRAN_PATI_FORM_ID = (long)cboTranPatiFormTo.EditValue;
        //    }
        //    if (ucIcdToTranfer != null)
        //    {
        //        var OjecIcd = icdProcessorTo.GetValue(ucIcdToTranfer);
        //        if (OjecIcd is IcdInputADO)
        //        {
        //            tranPatiUpdate.ICD_ID = ((IcdInputADO)OjecIcd).ICD_ID;
        //            tranPatiUpdate.ICD_MAIN_TEXT = ((IcdInputADO)OjecIcd).ICD_MAIN_TEXT;
        //        }
        //    }

        //    HIS_MEDI_ORG mediOrgData = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == txtMediOrgCodeTo.Text.Trim());
        //    if (mediOrgData != null)
        //    {
        //        tranPatiUpdate.MEDI_ORG_CODE = mediOrgData.MEDI_ORG_CODE;
        //        tranPatiUpdate.MEDI_ORG_NAME = mediOrgData.MEDI_ORG_NAME;
        //    }
        //    else
        //    {
        //        tranPatiUpdate.MEDI_ORG_CODE = "";
        //        tranPatiUpdate.MEDI_ORG_NAME = "";
        //    }
        //    tranPatiUpdate.ICD_TEXT = txtIcdExtraName.Text;
        //    tranPatiUpdate.ICD_SUB_CODE = txtIcdExtraCode.Text;
        //    tranPatiUpdate.CLINICAL_NOTE = txtDauHieuLamSang.Text;
        //    tranPatiUpdate.SUBCLINICAL_RESULT = txtXetNghiem.Text;
        //    tranPatiUpdate.TREATMENT_METHOD = txtPPKTThuoc.Text;
        //    tranPatiUpdate.PATIENT_CONDITION = txtTinhTrangNguoiBenh.Text;
        //    tranPatiUpdate.TREATMENT_DIRECTION = txtHuongDieuTri.Text;
        //    tranPatiUpdate.TRANSPORT_VEHICLE = txtPhuongTienVanChuyen.Text;
        //    tranPatiUpdate.TRANSPORTER = txtNguoiHoTong.Text;


        //    var rs = new BackendAdapter(param).Post<HIS_TRAN_PATI>(HisRequestUriStore.HIS_TRAN_PATI_UPDATE, ApiConsumers.MosConsumer, tranPatiUpdate, param);
        //    if (rs != null)
        //    {
        //        success = true;
        //        tranPatiToTranfer = new V_HIS_TRAN_PATI();
        //        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI>(tranPatiToTranfer, rs);
        //        if (tranPatiToTranfer != null)
        //        {
        //            FillDataToControlTranPatiToTranfer(tranPatiToTranfer);
        //            //Nếu thành công
        //            btnEdit.Enabled = true;
        //            btnSave.Enabled = false;
        //            btnCancel.Enabled = false;
        //            //Disable
        //            SetReadOnlyControlToTranPati(true);
        //        }
        //    }

        //    #region Show message
        //    MessageManager.Show(this.ParentForm, param, success);
        //    SessionManager.ProcessTokenLost(param);
        //    #endregion
        //}

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                btnCancel.Enabled = false;
                btnSave.Enabled = false;
                //Load lại dữ liệu trên textEdit
                if (this.currentTreatment != null)
                {
                    btnEdit.Enabled = true;
                    FillDataToControlTranPatiToTranfer(this.currentTreatment);
                }
                else
                {
                    btnEdit.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Edit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnEdit.Enabled)
                    return;
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnSave.Enabled)
                    return;
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Cancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnCancel.Enabled)
                    return;
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdExtraNames_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    if (txtIcdExtraName.ReadOnly)
                        return;
                    WaitingManager.Show();

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SecondaryIcd' is not plugins");

                    HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcds, txtIcdExtraCode.Text, txtIcdExtraName.Text);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(secondaryIcdADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new NullReferenceException("Khoi tao moduleData that bai. extenceInstance = null");

                    WaitingManager.Hide();
                    ((Form)extenceInstance).Show(this);
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdatetxtIcdExtraName(HIS_ICD dataIcd)
        {
            try
            {
                if (dataIcd != null)
                {
                    txtIcdExtraName.Text = txtIcdExtraName.Text + dataIcd.ICD_CODE + " - " + dataIcd.ICD_NAME + ", ";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetStringIcds(string delegateIcdCodes, string delegateIcdNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateIcdNames))
                {
                    txtIcdExtraName.Text = delegateIcdNames;
                }
                if (!string.IsNullOrEmpty(delegateIcdCodes))
                {
                    txtIcdExtraCode.Text = delegateIcdCodes;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdExtraName.Focus();
                    txtIcdExtraName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();
                string strIcdNames = "";
                if (!String.IsNullOrEmpty(currentValue))
                {
                    string seperate = ";";
                    string strWrongIcdCodes = "";
                    string[] periodSeparators = new string[1];
                    periodSeparators[0] = seperate;
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = txtIcdExtraCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (seperate + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (seperate + itemCode);
                            }
                        }
                        strIcdNames += seperate;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format("Không tìm thấy Icd tương ứng với các mã " + strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = txtIcdExtraCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = txtIcdExtraCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            txtIcdExtraCode.Focus();
                            txtIcdExtraCode.Select(startPositionWarm, lenghtPositionWarm);
                        }
                    }
                }
                SetCheckedIcdsToControl(txtIcdExtraCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdExtraName.Text == txtIcdExtraName.Properties.NullValuePrompt ? "" : txtIcdExtraName.Text);
                txtIcdExtraName.Text = processIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdExtraName.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdExtraCode.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string processIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void txtIcdExtraName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdExtraCode.SelectAll();
                    txtIcdExtraCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraName_Leave(object sender, EventArgs e)
        {
            try
            {
                btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediOrgNameTo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMediOrgNameTo != null)
                {
                    int L2;
                    int L1 = Convert.ToInt32(this.currentBranch.HEIN_LEVEL_CODE);
                    var curentMediOrg = VHisHeinMediOrg.Where(mo => mo.MEDI_ORG_CODE.Contains(cboMediOrgNameTo.EditValue.ToString())).ToList();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => curentMediOrg), curentMediOrg));
                    if (curentMediOrg != null && curentMediOrg.Count > 0)
                    {
                        var mediOrg = curentMediOrg.FirstOrDefault();
                        //Check cho hệ thống cũ
                        if (mediOrg.LEVEL_CODE.Contains("TW"))
                            L2 = 1;
                        else if (mediOrg.LEVEL_CODE.Contains("T"))
                            L2 = 2;
                        else if (mediOrg.LEVEL_CODE.Contains("H"))
                            L2 = 3;
                        else if (mediOrg.LEVEL_CODE.Contains("X"))
                            L2 = 4;
                        else L2 = Convert.ToInt32(mediOrg.LEVEL_CODE);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => L1), L1));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => L2), L2));

                        if (L1 - L2 == 1)
                        {
                            cboTranPatiFormTo.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE;
                            txtTranPatiFormTo.Text = VHisTranPatiForm.Where(pa => pa.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE).FirstOrDefault().TRAN_PATI_FORM_CODE;
                        }
                        else if (L1 - L2 > 1)
                        {
                            cboTranPatiFormTo.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE;
                            txtTranPatiFormTo.Text = VHisTranPatiForm.Where(pa => pa.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE).FirstOrDefault().TRAN_PATI_FORM_CODE;
                        }
                        else if (L1 - L2 < 0)
                        {
                            cboTranPatiFormTo.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG;
                            txtTranPatiFormTo.Text = VHisTranPatiForm.Where(pa => pa.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG).FirstOrDefault().TRAN_PATI_FORM_CODE;
                        }
                        else if (L1 - L2 == 0)
                        {
                            cboTranPatiFormTo.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN;
                            txtTranPatiFormTo.Text = VHisTranPatiForm.Where(pa => pa.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN).FirstOrDefault().TRAN_PATI_FORM_CODE;
                        }
                    }
                }
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
