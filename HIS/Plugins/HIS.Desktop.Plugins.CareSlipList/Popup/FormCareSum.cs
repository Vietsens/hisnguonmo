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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.LocalStorage.SdaConfigKey;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraEditors;
using AutoMapper;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.CareSlipList.CareSlipList;

namespace HIS.Desktop.Plugins.CareSlipList.Popup
{
    public partial class FormCareSum : Form
    {

        #region declare
        MOS.SDO.HisCareSumSDO careSumSdo;
        MOS.EFMODEL.DataModels.V_HIS_CARE_SUM VCareSum;
        MOS.EFMODEL.DataModels.HIS_CARE_SUM careSum;

        internal int action = -1;
        List<HIS_ICD> hisICD = new List<HIS_ICD>();
        internal HIS_ICD currentIcd { get; set; }
        int positionHandle = -1;
        internal FormCareSlipList frmCareSlipList;
        internal List<MPS.ADO.HisCareCheck> careCheckProcessing;
        // MOS.SDO.HisCareSumSDO careSumSdo;
        List<V_HIS_TREATMENT> treatment = new List<V_HIS_TREATMENT>();
        long treatmentId;
        DelegateRefeshData refreshData;
        #endregion

        #region contructor
        public FormCareSum(DelegateRefeshData _refreshData, long treatmentId)
        {
            InitializeComponent();
            this.action = GlobalVariables.ActionAdd;
            this.treatmentId = treatmentId;
            this.refreshData = _refreshData;
        }

        public FormCareSum(V_HIS_CARE_SUM _CareSum, DelegateRefeshData _refreshData)
        {
            InitializeComponent();
            this.action = GlobalVariables.ActionEdit;
            this.refreshData = _refreshData;
            VCareSum = _CareSum;
            if (VCareSum != null)
            {
                loadDataToControl();

            }
        }

        private void FormCareSum_Load(object sender, EventArgs e)
        {
            LoadDataToCommonInfo();
            LoaddataToComboChuanDoanTD();
            ValidateForm();

            if (this.treatmentId > 0)
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = treatmentId;
                var listTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                var lstTreatment = listTreatment.FirstOrDefault();
                if (string.IsNullOrEmpty(lstTreatment.ICD_MAIN_TEXT))
                {
                    txtIcdCode.Text = lstTreatment.ICD_CODE;
                    cboIcds.EditValue = lstTreatment.ICD_ID;
                    txtIcdText.Text = lstTreatment.ICD_TEXT;
                }
                else
                {
                    txtIcdCode.Text = lstTreatment.ICD_CODE;
                    chkIcds.Checked = true;
                    txtIcds.Text = lstTreatment.ICD_MAIN_TEXT;
                    cboIcds.EditValue = lstTreatment.ICD_ID;
                    txtIcdText.Text = lstTreatment.ICD_TEXT;
                }
            }
        }
        #endregion

        #region Method

        private void loadDataToControl()
        {
            try
            {
                if (!String.IsNullOrEmpty(VCareSum.ICD_MAIN_TEXT))
                {
                    chkIcds.Checked = true;
                    cboIcds.EditValue = (long)VCareSum.ICD_ID;
                    txtIcds.Text = VCareSum.ICD_MAIN_TEXT;
                }
                else
                {
                    cboIcds.EditValue = (long)VCareSum.ICD_ID;
                }
                txtIcdCode.Text = VCareSum.ICD_CODE;
                txtIcdText.Text = VCareSum.ICD_TEXT;
                txtThuTu.Text = Convert.ToString(VCareSum.NUM_ORDER);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCommonInfo()
        {
            try
            {
                long _treatmentId = 0;
                if (this.action == GlobalVariables.ActionAdd)
                {
                    _treatmentId = treatmentId;
                }
                else if (this.action == GlobalVariables.ActionEdit)
                {
                    _treatmentId = VCareSum.TREATMENT_ID;
                }

                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = _treatmentId;
                var listTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                //var lstBedRoomTran = listTreatment;//.Where(o => o.ID != this.currentModule.RoomId).ToList();
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    lblTreatmentCode.Text = listTreatment.FirstOrDefault().TREATMENT_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoaddataToComboChuanDoanTD()
        {

            try
            {
                hisICD = new List<HIS_ICD>();
                HisIcdFilter filter = new HisIcdFilter();
                //treatmentFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                hisICD = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_ICD>>(ApiConsumer.HisRequestUriStore.HIS_ICD_GET, ApiConsumers.MosConsumer, filter, null);
                cboIcds.Properties.DataSource = hisICD;
                cboIcds.Properties.DisplayMember = "ICD_NAME";
                cboIcds.Properties.ValueMember = "ID";
                cboIcds.Properties.ForceInitialize();
                cboIcds.Properties.Columns.Clear();
                cboIcds.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ICD_CODE", "", 50));
                cboIcds.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ICD_NAME", "", 100));
                cboIcds.Properties.ShowHeader = false;
                cboIcds.Properties.ImmediatePopup = true;
                cboIcds.Properties.DropDownRows = 10;
                cboIcds.Properties.PopupWidth = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdCombo(string searchCode, bool isExpand, FormCareSum control)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    control.cboIcds.EditValue = null;
                    control.cboIcds.Focus();
                    control.cboIcds.ShowPopup();
                }
                else
                {
                    var data = hisICD.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            control.cboIcds.EditValue = data[0].ID;
                            control.txtIcdCode.Text = data[0].ICD_CODE;
                            control.chkIcds.Focus();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.ICD_CODE == searchCode);
                            if (search != null)
                            {
                                control.cboIcds.EditValue = search.ID;
                                control.txtIcdCode.Text = search.ICD_CODE;
                                control.chkIcds.Focus();
                            }
                            else
                            {
                                control.cboIcds.EditValue = null;
                                control.cboIcds.Focus();
                                control.cboIcds.ShowPopup();
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

        private void addDataToSumCareSdo()
        {
            try
            {
                careSumSdo = new MOS.SDO.HisCareSumSDO();
                careSumSdo.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt32(txtThuTu.EditValue.ToString()); //Convert.ToInt32(txtThuTu.Text);
                if (careCheckProcessing != null && careCheckProcessing.Count > 0)
                {
                    List<long> careIds = careCheckProcessing.Select(o => o.ID).ToList();
                    careSumSdo.CareIds = careIds;
                    careSumSdo.TREATMENT_ID = treatmentId;

                    if (string.IsNullOrEmpty(txtIcds.Text))
                    {
                        if (cboIcds != null)
                        {
                            careSumSdo.ICD_ID = (long)cboIcds.EditValue;
                        }
                        if (!string.IsNullOrEmpty(txtIcdText.Text))
                        {
                            careSumSdo.ICD_TEXT = txtIcdText.Text;
                        }
                    }
                    else
                    {
                        careSumSdo.ICD_MAIN_TEXT = txtIcds.Text;
                        if (cboIcds != null)
                        {
                            careSumSdo.ICD_ID = (long)cboIcds.EditValue;
                        }
                        if (!string.IsNullOrEmpty(txtIcdText.Text))
                        {
                            careSumSdo.ICD_TEXT = txtIcdText.Text;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addDataToSumCare()
        {
            try
            {
                careSumSdo = new MOS.SDO.HisCareSumSDO();
                careSum.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt32(txtThuTu.EditValue.ToString()); //Convert.ToInt32(txtThuTu.Text);

                if (string.IsNullOrEmpty(txtIcds.Text))
                {
                    if (cboIcds != null)
                    {
                        careSum.ICD_ID = (long)cboIcds.EditValue;
                    }
                    if (!string.IsNullOrEmpty(txtIcdText.Text))
                    {
                        careSum.ICD_TEXT = txtIcdText.Text;
                    }
                }
                else
                {
                    careSum.ICD_MAIN_TEXT = txtIcds.Text;
                    if (cboIcds != null)
                    {
                        careSum.ICD_ID = (long)cboIcds.EditValue;
                    }
                    if (!string.IsNullOrEmpty(txtIcdText.Text))
                    {
                        careSum.ICD_TEXT = txtIcdText.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event control
        private void btnSave_Click(object sender, EventArgs e)
        {

            bool result = false;
            CommonParam param = new CommonParam();
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (this.action == GlobalVariables.ActionView)
                {
                    MessageBox.Show("Phiếu đã tổng hợp");
                    return;
                }
                if (this.action == GlobalVariables.ActionAdd)
                {
                    addDataToSumCareSdo();
                    var outPut = new BackendAdapter(param).Post<V_HIS_CARE_SUM>(ApiConsumer.HisRequestUriStore.HIS_CARE_SUM_CREATE, ApiConsumers.MosConsumer, careSumSdo, null);
                    if (outPut != null)
                    {
                        this.refreshData();
                        result = true;
                        this.action = GlobalVariables.ActionView;
                        this.Close();
                    }
                }
                else if (this.action == GlobalVariables.ActionEdit)
                {
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_CARE_SUM, MOS.EFMODEL.DataModels.HIS_CARE_SUM>();
                    careSum = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_CARE_SUM, MOS.EFMODEL.DataModels.HIS_CARE_SUM>(this.VCareSum);
                    addDataToSumCare();
                    var outPut = new BackendAdapter(param).Post<V_HIS_CARE_SUM>(ApiConsumer.HisRequestUriStore.HIS_CARE_SUM_UPDATE, ApiConsumers.MosConsumer, careSum, null);
                    if (outPut != null)
                    {
                        result = true;
                        this.action = GlobalVariables.ActionView;
                        this.Close();


                    }
                }
                #region Show message
                ResultManager.ShowMessage(param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIcd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcds.Checked == true)
                {
                    cboIcds.Visible = false;
                    txtIcds.Visible = true;
                    txtIcds.Text = cboIcds.Text;
                    txtIcds.Focus();
                    txtIcds.SelectAll();
                }
                else if (chkIcds.Checked == false)
                {
                    txtIcds.Visible = false;
                    cboIcds.Visible = true;
                    txtIcds.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadIcdCombo(strValue, false, this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboIcds.Text != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_ICD data = hisICD.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtIcdCode.Text = data.ICD_CODE;
                            chkIcds.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboIcds.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_ICD data = hisICD.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtIcdCode.Text = data.ICD_CODE;
                            chkIcds.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIcds_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdText.Focus();
                    txtIcdText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIcds_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcds.Checked == true)
                {
                    cboIcds.Visible = false;
                    txtIcds.Visible = true;
                    txtIcds.Text = cboIcds.Text;
                    txtIcds.Focus();
                    txtIcds.SelectAll();
                }
                else if (chkIcds.Checked == false)
                {
                    txtIcds.Visible = false;
                    cboIcds.Visible = true;
                    txtIcds.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcds_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIcds.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtThuTu.Focus();
                    txtThuTu.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click_1(null, null);
        }
        private void btnSave_Click_1(object sender, EventArgs e)
        {
            bool result = false;
            CommonParam param = new CommonParam();
            try
            {

                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (this.action == GlobalVariables.ActionView)
                {
                    MessageBox.Show("Phiếu đã tổng hợp");
                    return;
                }
                if (this.action == GlobalVariables.ActionAdd)
                {
                    //V_HIS_CARE_SUM data = new V_HIS_CARE_SUM();
                    // Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_CARE_SUM>(data, this.VCareSum);
                    //addDataToSumCareAdd();
                    //V_HIS_CARE_SUM data = new V_HIS_CARE_SUM();
                    //data.TREATMENT_ID = treatmentId;
                    addDataToSumCareSdo();
                    var outPut = new BackendAdapter(param).Post<V_HIS_CARE_SUM>(ApiConsumer.HisRequestUriStore.HIS_CARE_SUM_CREATE, ApiConsumers.MosConsumer, careSumSdo, null);
                    if (outPut != null)
                    {
                        this.refreshData();
                        result = true;
                        this.action = GlobalVariables.ActionView;
                        this.Close();

                    }
                }
                else if (this.action == GlobalVariables.ActionEdit)
                {
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_CARE_SUM, MOS.EFMODEL.DataModels.HIS_CARE_SUM>();
                    careSum = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_CARE_SUM, MOS.EFMODEL.DataModels.HIS_CARE_SUM>(this.VCareSum);
                    addDataToSumCare();
                    var outPut = new BackendAdapter(param).Post<V_HIS_CARE_SUM>(ApiConsumer.HisRequestUriStore.HIS_CARE_SUM_UPDATE, ApiConsumers.MosConsumer, careSum, null);
                    if (outPut != null)
                    {
                        this.refreshData();
                        result = true;
                        this.action = GlobalVariables.ActionView;
                        this.Close();
                    }
                }
                #region Show message
                ResultManager.ShowMessage(param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
                if (result)
                {
                    this.frmCareSlipList.Init();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Validate


        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidNumber()
        {
            try
            {
                TextNumberValidate validateNumber = new TextNumberValidate();
                validateNumber.txtTextEdit = txtThuTu;
                validateNumber.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validateNumber.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtThuTu, validateNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateForm()
        {
            try
            {
                ValidNumber();
                ValidateLookupWithTextEdit(cboIcds, txtIcdCode, dxValidationProvider1);
                //ValidationSingleControl(txtIcds, dxValidationProvider1);       
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        #endregion
    }
}
