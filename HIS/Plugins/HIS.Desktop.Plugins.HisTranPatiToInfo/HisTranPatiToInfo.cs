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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisTranPatiToInfo.ProcessLoadDataCombo;
using HIS.Desktop.Plugins.HisTranPatiToInfo.Validation;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.HisTranPatiToInfo
{
    public partial class frmHisTranPatiToInfo : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;
        int action;

        internal IcdProcessor icdProcessor;
        internal UserControl ucIcdFromTranfer;
        internal UserControl ucIcdSubFromTranfer;
        internal SecondaryIcdProcessor secondaryIcdProcessor;

        HIS.Desktop.Common.DelegateRefreshData _dlgRef;

        int positionHandleControl = -1;
        HIS_BRANCH currentBranch = new HIS_BRANCH();
        List<HIS_MEDI_ORG> VHisHeinMediOrg = new List<HIS_MEDI_ORG>();
        List<HIS_TRAN_PATI_FORM> VHisTranPatiForm = new List<HIS_TRAN_PATI_FORM>();

        internal HIS_TREATMENT currentTreatment { get; set; }

        public frmHisTranPatiToInfo()
        {
            InitializeComponent();
        }

        public frmHisTranPatiToInfo(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                SetIconFrm();
                InitUcIcd();
                InitUcIcdSub();
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

        public frmHisTranPatiToInfo(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId, HIS.Desktop.Common.DelegateRefreshData _dlg)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                this._dlgRef = _dlg;
                SetIconFrm();
                InitUcIcd();
                InitUcIcdSub();
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

        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DelegateNextFocus = DelegateNextFocusIcd;
                ado.Width = 892;
                ado.Height = 24;
                ado.LabelTextSize = 118;
                ado.MinSize = 181;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>();

                this.ucIcdFromTranfer = (UserControl)icdProcessor.Run(ado);
                if (this.ucIcdFromTranfer != null)
                {
                    this.layoutControlIcd.Controls.Add(this.ucIcdFromTranfer);
                    this.ucIcdFromTranfer.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcdSub()
        {
            try
            {
                secondaryIcdProcessor = new SecondaryIcdProcessor();
                SecondaryIcdInitADO ado = new SecondaryIcdInitADO();
                ado.DelegateNextFocus = DelegateNextFocusIcd;
                ado.Width = 877;
                ado.Height = 24;
                ado.TextSize = 118;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.HisIcds = BackendDataWorker.Get<HIS_ICD>();

                this.ucIcdSubFromTranfer = (UserControl)secondaryIcdProcessor.Run(ado);
                if (this.ucIcdSubFromTranfer != null)
                {
                    this.layoutControlIcdSub.Controls.Add(this.ucIcdSubFromTranfer);
                    this.ucIcdSubFromTranfer.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                if (this.icdProcessor != null && this.ucIcdFromTranfer != null)
                {
                    var icdValue = this.icdProcessor.GetValue(this.ucIcdFromTranfer);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }
        private void DelegateNextFocusIcd()
        {
            try
            {
                btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisTranPatiToInfo_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCurrentBranch();
                VHisHeinMediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>();
                var VHisTranPatiReason = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>();
                VHisTranPatiForm = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>();
                //FromTranfer
                ProcessLoad.LoadDataToComboMediOrg(cboMediOrgName, VHisHeinMediOrg);
                ProcessLoad.LoadDataToComboTranPatiReason(cboTranPatiReason, VHisTranPatiReason);
                ProcessLoad.LoadDataToComboTranPatiForm(cboTranPatiForm, VHisTranPatiForm);
                ProcessLoad.LoadDataToComboChuyenTuyen(cboChuyenTuyen);
                ProcessLoad.LoadDataToComboDanhGiaChuyenTuyen(cboTransferInReviews);
                SetCaptionByLanguageKey();

                LoadDataDefaultTreatment();

                //if (this.currentTreatment != null && this.currentTreatment.TRANSFER_IN_CMKT == IMSys.DbConfig.HIS_RS.HIS_TREATMENT.TRANSFER_IN_CMKT__DT)
                if (this.currentTreatment != null)
                {
                    this.action = GlobalVariables.ActionEdit;
                    FillDataToControlFromTranPati(this.currentTreatment);
                }
                else
                {
                    this.action = GlobalVariables.ActionAdd;
                    SetDefaultValueControlFromTranPati();
                }
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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisTranPatiToInfo.Resources.Lang", typeof(HIS.Desktop.Plugins.HisTranPatiToInfo.frmHisTranPatiToInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Edit.Caption = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.barButtonItem__Edit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.barButtonItem__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Cancel.Caption = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.barButtonItem__Cancel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Delete.Caption = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.barButtonItem__Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlIcd.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.layoutControlIcd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlIcdSub.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.layoutControlIcdSub.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediOrgName.Properties.NullText = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.cboMediOrgName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTranPatiForm.Properties.NullText = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.cboTranPatiForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTranPatiReason.Properties.NullText = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.cboTranPatiReason.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciMediOrgCode.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.lciMediOrgCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.Text = Inventec.Common.Resource.Get.Value("HisTranPatiToInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataDefaultTreatment()
        {
            try
            {
                this.currentTreatment = new HIS_TREATMENT();
                if (this.treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                    filter.ID = this.treatmentId;
                    this.currentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControlFromTranPati(HIS_TREATMENT data)
        {
            try
            {
                if (data != null)
                {
                    cboMediOrgName.EditValue = data.TRANSFER_IN_MEDI_ORG_CODE;
                    txtMediOrgCode.Text = data.TRANSFER_IN_MEDI_ORG_CODE;
                    txtSoChuyenVien.Text = data.TRANSFER_IN_CODE;

                    if (data.TRANSFER_IN_CMKT.HasValue)
                    {
                        cboChuyenTuyen.EditValue = data.TRANSFER_IN_CMKT;
                    }
                    else
                    {
                        cboChuyenTuyen.EditValue = null;
                    }
                    if (data.TRANSFER_IN_REVIEWS.HasValue)
                    {
                        cboTransferInReviews.EditValue = data.TRANSFER_IN_REVIEWS;

                    }
                    else
                    {
                        cboTransferInReviews.EditValue = null;
                    }
                    MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON hisTranPatiReason = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == data.TRANSFER_IN_REASON_ID);//TRANSFER_IN_REASON_ID
                    if (hisTranPatiReason != null)
                    {
                        cboTranPatiReason.EditValue = hisTranPatiReason.ID;
                        txtTranPatiReason.Text = hisTranPatiReason.TRAN_PATI_REASON_CODE;
                    }

                    MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM hisTranPatiForm = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().SingleOrDefault(o => o.ID == data.TRANSFER_IN_FORM_ID);//TRANSFER_IN_FORM_ID
                    if (hisTranPatiForm != null)
                    {
                        cboTranPatiForm.EditValue = hisTranPatiForm.ID;
                        txtTranPatiForm.Text = hisTranPatiForm.TRAN_PATI_FORM_CODE;
                    }

                    //ICD
                    IcdInputADO inputIcd = new IcdInputADO();
                    inputIcd.ICD_NAME = data.TRANSFER_IN_ICD_NAME;
                    inputIcd.ICD_CODE = data.TRANSFER_IN_ICD_CODE;
                    if (ucIcdFromTranfer != null)
                    {
                        icdProcessor.Reload(ucIcdFromTranfer, inputIcd);
                    }
                    //ICDSub
                    SecondaryIcdDataADO secondaryIcdData = new SecondaryIcdDataADO();
                    secondaryIcdData.ICD_SUB_CODE = data.TRANSFER_IN_ICD_SUB_CODE;
                    secondaryIcdData.ICD_TEXT = data.TRANSFER_IN_ICD_TEXT;
                    if (ucIcdSubFromTranfer != null)
                    {
                        secondaryIcdProcessor.Reload(ucIcdSubFromTranfer, secondaryIcdData);
                    }

                    if (data.TRANSFER_IN_TIME_FROM != null)
                    {
                        dtFromTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TRANSFER_IN_TIME_FROM ?? 0) ?? DateTime.Now;
                    }
                    else
                    {
                        dtFromTime.EditValue = null;
                    }
                    if (data.TRANSFER_IN_TIME_TO != null)
                    {
                        dtToTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TRANSFER_IN_TIME_TO ?? 0) ?? DateTime.Now;
                    }
                    else
                    {
                        dtToTime.EditValue = null;
                    }
                    SetReadOnlyControlFromTranPati(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediOrgCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    ProcessLoad.LoadNoiDKKCBBDCombo(strValue, false, cboMediOrgName, txtMediOrgCode, null, txtSoChuyenVien);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediOrgName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMediOrgName.EditValue != null)
                    {
                        cboMediOrgName.Properties.Buttons[1].Visible = true;
                        HIS_MEDI_ORG data = BackendDataWorker.Get<HIS_MEDI_ORG>().SingleOrDefault(o => o.MEDI_ORG_CODE == cboMediOrgName.EditValue);
                        if (data != null)
                        {
                            txtMediOrgCode.Text = data.MEDI_ORG_CODE;
                            // lblMediOrgAddress.Text = data.Address;
                            txtSoChuyenVien.Focus();
                            txtSoChuyenVien.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediOrgName_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMediOrgName.EditValue = null;
                    txtMediOrgCode.Text = "";
                    //lblMediOrgAddress.Text = "";
                    txtMediOrgCode.Focus();
                    txtMediOrgCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediOrgName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMediOrgName.EditValue != null)
                    {
                        HIS_MEDI_ORG data = BackendDataWorker.Get<HIS_MEDI_ORG>().SingleOrDefault(o => o.MEDI_ORG_CODE == txtMediOrgCode.Text);
                        if (data != null)
                        {
                            txtMediOrgCode.Text = data.MEDI_ORG_CODE;
                            //lblMediOrgAddress.Text = data.Address;
                            //txtDauHieuLamSang.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTranPatiReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    ProcessLoad.LoadComboTranPatiReason(strValue, false, cboTranPatiReason, txtTranPatiReason, txtTranPatiForm);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTranPatiReason_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTranPatiReason.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTranPatiReason_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTranPatiReason.EditValue != null)
                    {
                        cboTranPatiReason.Properties.Buttons[1].Visible = true;
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == (long)cboTranPatiReason.EditValue);//Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiReason.EditValue.ToString() ?? ""));
                        if (data != null)
                        {
                            txtTranPatiReason.Text = data.TRAN_PATI_REASON_CODE;
                            txtTranPatiForm.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTranPatiReason_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTranPatiReason.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON data = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == (long)cboTranPatiReason.EditValue);
                        if (data != null)
                        {
                            cboTranPatiForm.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTranPatiForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    ProcessLoad.LoadComboTranPatiForm(strValue, false, cboTranPatiForm, txtTranPatiForm, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTranPatiForm_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTranPatiForm.EditValue != null)
                    {
                        cboTranPatiForm.Properties.Buttons[1].Visible = true;
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM data = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().SingleOrDefault(o => o.ID == (long)cboTranPatiForm.EditValue);// Inventec.Common.TypeConvert.Parse.ToInt64(cboTranPatiForm.EditValue.ToString() ?? "")); ;
                        if (data != null)
                        {
                            txtTranPatiForm.Text = data.TRAN_PATI_FORM_CODE;
                            //if (ucIcdFromTranfer != null)
                            //{
                            //    icdProcessor.FocusControl(ucIcdFromTranfer);
                            //}
                            cboChuyenTuyen.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Visible = false;
                btnSave.Visible = true;
                btnCancel.Enabled = true;
                //todo
                //1. Readonly tất cả control
                SetReadOnlyControlFromTranPati(false);
                //if (tranPatiFromTranfer.MEDI_ORG_CODE == null || tranPatiFromTranfer.MEDI_ORG_NAME == null)
                //{
                //    txtMediOrgCode.ReadOnly = true;
                //    cboMediOrgName.ReadOnly = true;
                //}
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
                bool success = false;
                CommonParam param = new CommonParam();

                bool valid = secondaryIcdProcessor.GetValidate(ucIcdSubFromTranfer);
                if (!valid) return;
                //Lưu

                HIS_TREATMENT _treatmentupdate = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(_treatmentupdate, this.currentTreatment);
                MOS.SDO.HisTreatmentTranPatiSDO sdoUpdate = new MOS.SDO.HisTreatmentTranPatiSDO();
                sdoUpdate.IsTranIn = true;
                sdoUpdate.HisTreatment = new HIS_TREATMENT();
                if (!txtMediOrgCode.ReadOnly)
                {
                    //this.positionHandleControl = -1;
                    //if (!dxValidationProvider1.Validate())
                    //    return;

                    HIS_MEDI_ORG mediOrgData = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == txtMediOrgCode.Text.Trim());
                    if (mediOrgData != null)
                    {
                        _treatmentupdate.TRANSFER_IN_MEDI_ORG_CODE = mediOrgData.MEDI_ORG_CODE;
                        _treatmentupdate.TRANSFER_IN_MEDI_ORG_NAME = mediOrgData.MEDI_ORG_NAME;
                    }
                    else
                    {
                        //DevExpress.XtraEditors.XtraMessageBox.Show("Mã " + txtMediOrgCode.Text.ToString() + " dữ liệu không xác định", "Thông báo");
                        //txtMediOrgCode.Focus();
                        //txtMediOrgCode.SelectAll();
                        _treatmentupdate.TRANSFER_IN_MEDI_ORG_CODE = "";
                        _treatmentupdate.TRANSFER_IN_MEDI_ORG_NAME = "";
                        //return;
                    }
                }
                if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                {
                    _treatmentupdate.TRANSFER_IN_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    _treatmentupdate.TRANSFER_IN_TIME_FROM = null;
                }
                if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                {
                    _treatmentupdate.TRANSFER_IN_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "235959");
                }
                else
                {
                    _treatmentupdate.TRANSFER_IN_TIME_TO = null;
                }
                if (cboTranPatiReason.EditValue != null)
                {
                    _treatmentupdate.TRANSFER_IN_REASON_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboTranPatiReason.EditValue ?? "0").ToString());
                }
                else
                    _treatmentupdate.TRANSFER_IN_REASON_ID = null;

                if (cboChuyenTuyen.EditValue != null)
                {
                    _treatmentupdate.TRANSFER_IN_CMKT = Inventec.Common.TypeConvert.Parse.ToInt64((cboChuyenTuyen.EditValue ?? "0").ToString());
                }
                else
                    _treatmentupdate.TRANSFER_IN_CMKT = null;

                if (cboTranPatiForm.EditValue != null)
                {
                    _treatmentupdate.TRANSFER_IN_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboTranPatiForm.EditValue ?? "0").ToString());
                }
                else
                    _treatmentupdate.TRANSFER_IN_FORM_ID = null;

                if (ucIcdFromTranfer != null)
                {
                    var OjecIcd = icdProcessor.GetValue(ucIcdFromTranfer);
                    if (OjecIcd is IcdInputADO)
                    {
                        _treatmentupdate.TRANSFER_IN_ICD_NAME = ((IcdInputADO)OjecIcd).ICD_NAME;
                        _treatmentupdate.TRANSFER_IN_ICD_CODE = ((IcdInputADO)OjecIcd).ICD_CODE;
                    }
                    else
                    {
                        _treatmentupdate.TRANSFER_IN_ICD_NAME = null;
                        _treatmentupdate.TRANSFER_IN_ICD_CODE = null;
                    }
                }
                var OjecIcdSub = secondaryIcdProcessor.GetValue(ucIcdSubFromTranfer);
                if (OjecIcdSub is SecondaryIcdDataADO)
                {
                    _treatmentupdate.TRANSFER_IN_ICD_SUB_CODE = ((SecondaryIcdDataADO)OjecIcdSub).ICD_SUB_CODE;
                    _treatmentupdate.TRANSFER_IN_ICD_TEXT = ((SecondaryIcdDataADO)OjecIcdSub).ICD_TEXT;
                }
                else
                {
                    _treatmentupdate.TRANSFER_IN_ICD_SUB_CODE = null;
                    _treatmentupdate.TRANSFER_IN_ICD_TEXT = null;
                }

                if (cboTransferInReviews.EditValue != null)
                {
                    sdoUpdate.TransferInReviews = Convert.ToInt16(cboTransferInReviews.EditValue.ToString());
                }
                else
                {
                    sdoUpdate.TransferInReviews = null;
                }

                _treatmentupdate.TRANSFER_IN_CODE = txtSoChuyenVien.Text;
                sdoUpdate.HisTreatment = _treatmentupdate;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("sdoUpdate___:", sdoUpdate));
                var outPut = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateTranPatiInfo", ApiConsumers.MosConsumer, sdoUpdate, param);
                if (outPut != null)
                {
                    success = true;
                    this.currentTreatment = outPut;
                    FillDataToControlFromTranPati(this.currentTreatment);
                    if (_dlgRef != null)
                    {
                        _dlgRef();
                    }
                    //Nếu thành công
                    btnEdit.Visible = true;
                    btnSave.Visible = false;
                    btnCancel.Enabled = false;
                    //Disable
                    SetReadOnlyControlFromTranPati(true);
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

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                btnCancel.Enabled = false;
                btnEdit.Visible = true;
                btnSave.Visible = false;
                //Load lại dữ liệu trên textEdit
                if (this.currentTreatment != null)
                {
                    SetDefaultValueControlFromTranPati();
                    FillDataToControlFromTranPati(this.currentTreatment);
                }
                else
                {
                    SetDefaultValueControlFromTranPati();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetReadOnlyControlFromTranPati(bool isReadOnly)
        {
            try
            {
                dtFromTime.ReadOnly = isReadOnly;
                dtToTime.ReadOnly = isReadOnly;
                txtMediOrgCode.ReadOnly = isReadOnly;
                cboMediOrgName.ReadOnly = isReadOnly;
                cboMediOrgName.Properties.Buttons[1].Visible = !isReadOnly;
                cboTranPatiForm.Properties.Buttons[1].Visible = !isReadOnly;
                cboTranPatiReason.Properties.Buttons[1].Visible = !isReadOnly;
                txtTranPatiForm.ReadOnly = isReadOnly;
                cboTranPatiForm.ReadOnly = isReadOnly;
                txtTranPatiReason.ReadOnly = isReadOnly;
                cboTranPatiReason.ReadOnly = isReadOnly;
                txtSoChuyenVien.ReadOnly = isReadOnly;
                cboChuyenTuyen.ReadOnly = isReadOnly;
                cboChuyenTuyen.Properties.Buttons[1].Visible = !isReadOnly;
                cboTransferInReviews.ReadOnly = isReadOnly;
                cboTransferInReviews.Properties.Buttons[1].Visible = !isReadOnly;
                if (ucIcdFromTranfer != null)
                {
                    icdProcessor.ReadOnly(ucIcdFromTranfer, isReadOnly);
                }
                if (ucIcdSubFromTranfer != null)
                {
                    secondaryIcdProcessor.ReadOnly(ucIcdSubFromTranfer, isReadOnly);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControlFromTranPati()
        {
            try
            {
                dtFromTime.EditValue = null;
                dtToTime.EditValue = null;
                txtMediOrgCode.Text = null;
                cboMediOrgName.EditValue = null;
                txtTranPatiForm.Text = null;
                cboTranPatiForm.EditValue = null;
                txtTranPatiReason.Text = null;
                cboTranPatiReason.EditValue = null;
                if (ucIcdFromTranfer != null)
                {
                    icdProcessor.Reload(ucIcdFromTranfer, null);
                }
                if (ucIcdSubFromTranfer != null)
                {
                    secondaryIcdProcessor.Reload(ucIcdSubFromTranfer, null);
                }
                SetReadOnlyControlFromTranPati(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnEdit.Focus();
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
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Delete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnDelete_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediOrgCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtMediOrgCode.Text != null)
                    {

                    }
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

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
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

        private void ValidationCboMediOrg()
        {
            try
            {
                LookupEditWithTextEditValidationRule icdMainRule = new LookupEditWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtMediOrgCode;
                icdMainRule.cbo = cboMediOrgName;
                icdMainRule.ErrorText = "Thiếu trường dữ liệu bắt buộc";// MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtMediOrgCode, icdMainRule);
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
                string strError = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
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

        private void cboMediOrgName_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete && !cboMediOrgName.ReadOnly)
                {
                    cboMediOrgName.EditValue = null;
                    txtMediOrgCode.Text = "";
                    cboMediOrgName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiReason_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete && !cboTranPatiReason.ReadOnly)
                {
                    cboTranPatiReason.EditValue = null;
                    txtTranPatiReason.Text = "";
                    cboTranPatiReason.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTranPatiForm_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete && !cboTranPatiForm.ReadOnly)
                {
                    cboTranPatiForm.EditValue = null;
                    txtTranPatiForm.Text = "";
                    cboTranPatiForm.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChuyenTuyen_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboChuyenTuyen.EditValue = null;
                    this.cboChuyenTuyen.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChuyenTuyen_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboChuyenTuyen.EditValue != null)
                    {
                        this.cboChuyenTuyen.Properties.Buttons[1].Visible = true;
                        if (ucIcdFromTranfer != null)
                        {
                            icdProcessor.FocusControl(ucIcdFromTranfer);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSoChuyenVien_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTranPatiReason.Focus();
                    txtTranPatiReason.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChuyenTuyen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboChuyenTuyen.EditValue != null)
                    {
                        if (ucIcdFromTranfer != null)
                        {
                            icdProcessor.FocusControl(ucIcdFromTranfer);
                        }
                    }
                    else
                    {
                        cboChuyenTuyen.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtFromTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                dtToTime.Focus();
                dtToTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtToTime_Closed(object sender, ClosedEventArgs e)
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

        private void cboMediOrgName_TextChanged(object sender, EventArgs e)
        {
            if (cboMediOrgName != null)
            {
                int L2;
                int L1 = Convert.ToInt32(this.currentBranch.HEIN_LEVEL_CODE);
                var curentMediOrg = VHisHeinMediOrg.Where(mo => mo.MEDI_ORG_CODE.Contains(cboMediOrgName.EditValue.ToString())).ToList();
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

                    if (L2 - L1 == 1)
                    {
                        cboTranPatiForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE;
                        txtTranPatiForm.Text = VHisTranPatiForm.Where(pa => pa.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE).FirstOrDefault().TRAN_PATI_FORM_CODE;
                    }
                    else if (L2 - L1 > 1)
                    {
                        cboTranPatiForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE;
                        txtTranPatiForm.Text = VHisTranPatiForm.Where(pa => pa.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE).FirstOrDefault().TRAN_PATI_FORM_CODE;
                    }
                    else if (L2 - L1 < 0)
                    {
                        cboTranPatiForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG;
                        txtTranPatiForm.Text = VHisTranPatiForm.Where(pa => pa.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG).FirstOrDefault().TRAN_PATI_FORM_CODE;
                    }
                    else if (L2 - L1 == 0)
                    {
                        cboTranPatiForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN;
                        txtTranPatiForm.Text = VHisTranPatiForm.Where(pa => pa.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN).FirstOrDefault().TRAN_PATI_FORM_CODE;
                    }
                }
            }
        }

        private void cboTransferInReviews_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboTransferInReviews.EditValue = null;
                    this.cboTransferInReviews.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTransferInReviews_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTransferInReviews.EditValue != null)
                {
                    this.cboTransferInReviews.Properties.Buttons[1].Visible = true;
                }   
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
