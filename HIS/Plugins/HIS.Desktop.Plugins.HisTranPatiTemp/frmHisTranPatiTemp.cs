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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTranPatiTemp
{
    public partial class frmHisTranPatiTemp : FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;
        HIS_TRAN_PATI_TEMP currentData;
        List<HIS_MEDI_ORG> lstMediOrg;
        List<HIS_TRAN_PATI_REASON> lstTranPatiReason;
        List<HIS_TRAN_PATI_TECH> lstTranpatiTech;
        List<HIS_TRAN_PATI_FORM> lstTranpatiForm;

        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        int positionHandle = -1;


        #endregion

        public frmHisTranPatiTemp(Inventec.Desktop.Common.Modules.Module moduleData, HIS_TRAN_PATI_TEMP _currentData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.currentData = _currentData;
        }

        private void frmHisTranPatiTemp_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        public void MeShow()
        {
            SetDefaultValue();

            LoadDataToComboBox();

            if (currentData != null)
            {
                if (currentData.ID > 0)
                    this.ActionType = GlobalVariables.ActionEdit;

                FillDataToEditor(currentData);
            }

            EnableControlChanged(this.ActionType);

            FillDatagctFormList();

            //SetCaptionByLanguageKey();


            InitTabIndex();

            ValidateForm();

            SetDefaultFocus();
        }


        #region getDataCombo

        void GetDataMediOrg()
        {
            try
            {
                HisMediOrgFilter mediOrgFilter = new HisMediOrgFilter();
                mediOrgFilter.IS_ACTIVE = 1;
                this.lstMediOrg = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_ORG>>("api/HisMediOrg/Get", ApiConsumers.MosConsumer, mediOrgFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void GetDataTranpatiReason()
        {
            try
            {
                HisTranPatiReasonFilter tranPatiReasonFilter = new HisTranPatiReasonFilter();
                tranPatiReasonFilter.IS_ACTIVE = 1;
                this.lstTranPatiReason = new BackendAdapter(new CommonParam()).Get<List<HIS_TRAN_PATI_REASON>>("api/HisTranPatiReason/Get", ApiConsumers.MosConsumer, tranPatiReasonFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void GetDataTranpatiTech()
        {
            try
            {
                HisTranPatiTechFilter tranPatiTechFilter = new HisTranPatiTechFilter();
                tranPatiTechFilter.IS_ACTIVE = 1;
                this.lstTranpatiTech = new BackendAdapter(new CommonParam()).Get<List<HIS_TRAN_PATI_TECH>>("api/HisTranPatiTech/Get", ApiConsumers.MosConsumer, tranPatiTechFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void GetDataTranpatiForm()
        {
            try
            {
                HisTranPatiFormFilter tranPatiFormFilter = new HisTranPatiFormFilter();
                tranPatiFormFilter.IS_ACTIVE = 1;
                this.lstTranpatiForm = new BackendAdapter(new CommonParam()).Get<List<HIS_TRAN_PATI_FORM>>("api/HisTranPatiForm/Get", ApiConsumers.MosConsumer, tranPatiFormFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void LoadDataToComboBox()
        {
            try
            {

                Task dataMediOrg = new Task(() => GetDataMediOrg());
                Task dataTranpatiReason = new Task(() => GetDataTranpatiReason());
                Task dataTranpatiForm = new Task(() => GetDataTranpatiForm());
                Task dataTranpatiTech = new Task(() => GetDataTranpatiTech());

                dataMediOrg.Start();
                dataTranpatiReason.Start();
                dataTranpatiForm.Start();
                dataTranpatiTech.Start();

                Task.WaitAll(dataMediOrg, dataTranpatiReason, dataTranpatiForm, dataTranpatiTech);

                InitCombo(cboMediOrg, this.lstMediOrg, "MEDI_ORG_CODE", "MEDI_ORG_NAME", "ID");
                InitCombo(cboReason, this.lstTranPatiReason, "TRAN_PATI_REASON_CODE", "TRAN_PATI_REASON_NAME", "ID");
                InitCombo(cboForm, this.lstTranpatiForm, "TRAN_PATI_FORM_CODE", "TRAN_PATI_FORM_NAME", "ID");
                InitCombo(cboTech, this.lstTranpatiTech, "TRAN_PATI_TECH_CODE", "TRAN_PATI_TECH_NAME", "ID");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void InitCombo(GridLookUpEdit cbo, object data, string DisplayCode, string DisplayMember, string ValueMember)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(DisplayCode, "", 100, 1));
                columnInfos.Add(new ColumnInfo(DisplayMember, "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(DisplayMember, ValueMember, columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditor(HIS_TRAN_PATI_TEMP data)
        {
            try
            {
                txtCode.Text = data.TRAN_PATI_TEMP_CODE;
                txtName.Text = data.TRAN_PATI_TEMP_NAME;
                cboMediOrg.EditValue = lstMediOrg.Where(o => o.MEDI_ORG_CODE == data.MEDI_ORG_CODE).First().ID;
                cboForm.EditValue = data.TRAN_PATI_FORM_ID;
                cboReason.EditValue = data.TRAN_PATI_REASON_ID;
                cboTech.EditValue = data.TRAN_PATI_TECH_ID;
                txtDirection.Text = data.TREATMENT_DIRECTION;
                txtCondition.Text = data.PATIENT_CONDITION;
                txtMethod.Text = data.TREATMENT_METHOD;
                txtVehicle.Text = data.TRANSPORT_VEHICLE;
                txtUsedMedicine.Text = data.USED_MEDICINE;
                txtTransporter.Text = data.TRANSPORTER;
                chkIsPublic.Checked = data.IS_PUBLIC == 1;
                lblAddress.Text = !string.IsNullOrEmpty(data.MEDI_ORG_CODE) ? lstMediOrg.Where(o => o.MEDI_ORG_CODE == data.MEDI_ORG_CODE).First().ADDRESS : "";


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtKey.Focus();
                txtKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtName, 100);
                ValidationSingleControl(txtCode, 20);
                ValidationSingleControl(cboForm, txtForm);
                ValidationSingleControl(cboMediOrg, txtMediOrg);
                ValidationSingleControl(cboReason, txtReason);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(TextEdit txt, int maxLength)
        {
            try
            {
                ValidateMaxLength validRule = new ValidateMaxLength();
                validRule.maxlength = maxLength;
                validRule.txtEdit = txt;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, TextEdit txt)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                dicOrderTabIndexControl.Add("txtName", 1);
                dicOrderTabIndexControl.Add("txtCode", 0);
                dicOrderTabIndexControl.Add("cboMediOrg", 2);
                dicOrderTabIndexControl.Add("cboReason", 3);
                dicOrderTabIndexControl.Add("cboTech", 4);
                dicOrderTabIndexControl.Add("cboForm", 5);
                dicOrderTabIndexControl.Add("txtDirection", 6);
                dicOrderTabIndexControl.Add("txtCondition", 7);
                dicOrderTabIndexControl.Add("txtMethod", 8);
                dicOrderTabIndexControl.Add("txtVehicle", 9);
                dicOrderTabIndexControl.Add("txtUsedMedicine", 10);
                dicOrderTabIndexControl.Add("txtTransporter", 11);
                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, layoutControl1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDatagctFormList()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnSave.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;

                txtKey.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<HIS_TRAN_PATI_TEMP>> apiResult = null;
                HisTranPatiTempFilter filter = new HisTranPatiTempFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                if (currentData != null && currentData.ID > 0)
                    filter.ID = currentData.ID;


                gridViewTranPatiTemp.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_TRAN_PATI_TEMP>>("api/HisTranPatiTemp/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<HIS_TRAN_PATI_TEMP>)apiResult.Data;
                    if (data != null)
                    {

                        gridViewTranPatiTemp.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewTranPatiTemp.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisTranPatiTempFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKey.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

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
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTranPatiTemp_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_TRAN_PATI_TEMP pData = (HIS_TRAN_PATI_TEMP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "IS_PUBLIC_DISPLAY")
                    {
                        e.Value = pData.IS_PUBLIC == 1;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediOrg_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit dt = sender as GridLookUpEdit;
                if (dt != null)
                {
                    var mediOrg = lstMediOrg.Where(o => o.ID == Convert.ToInt64(dt.EditValue));
                    txtMediOrg.Text = mediOrg != null ? mediOrg.First().MEDI_ORG_CODE : "";
                    lblAddress.Text = mediOrg != null ? mediOrg.First().ADDRESS : "";

                }
                else
                {
                    txtMediOrg.Text = "";
                    lblAddress.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboReason_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit dt = sender as GridLookUpEdit;
                if (dt != null)
                {
                    var tranpatiReason = lstTranPatiReason.Where(o => o.ID == Convert.ToInt64(dt.EditValue));
                    txtReason.Text = tranpatiReason != null ? tranpatiReason.First().TRAN_PATI_REASON_CODE : "";
                }
                else
                {
                    txtReason.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboForm_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit dt = sender as GridLookUpEdit;
                if (dt != null)
                {
                    var tranpatiForm = lstTranpatiForm.Where(o => o.ID == Convert.ToInt64(dt.EditValue));
                    txtForm.Text = tranpatiForm != null ? tranpatiForm.First().TRAN_PATI_FORM_CODE : "";
                }
                else
                {
                    txtForm.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTech_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit dt = sender as GridLookUpEdit;
                if (dt != null)
                {
                    var tranpatiTech = lstTranpatiTech.Where(o => o.ID == Convert.ToInt64(dt.EditValue));
                    txtTech.Text = tranpatiTech != null ? tranpatiTech.First().TRAN_PATI_TECH_CODE : "";
                }
                else
                {
                    txtTech.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMediOrg_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                TextEdit txt = sender as TextEdit;
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txt.Text))
                    {
                        var mediOrg = lstMediOrg.Where(o => o.MEDI_ORG_CODE == txt.Text);
                        cboMediOrg.EditValue = mediOrg != null ? (long?)mediOrg.First().ID : null;
                    }
                    else
                    {
                        cboMediOrg.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                TextEdit txt = sender as TextEdit;
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txt.Text))
                    {
                        var tranpatiReason = lstTranPatiReason.Where(o => o.TRAN_PATI_REASON_CODE == txt.Text);
                        cboReason.EditValue = tranpatiReason != null ? (long?)tranpatiReason.First().ID : null;
                    }
                    else
                    {
                        cboReason.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                TextEdit txt = sender as TextEdit;
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txt.Text))
                    {
                        var tranpatiForm = lstTranpatiForm.Where(o => o.TRAN_PATI_FORM_CODE == txt.Text);
                        cboForm.EditValue = tranpatiForm != null ? (long?)tranpatiForm.First().ID : null;
                    }
                    else
                    {
                        cboForm.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTech_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                TextEdit txt = sender as TextEdit;
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txt.Text))
                    {
                        var tranpatiTech = lstTranpatiTech.Where(o => o.TRAN_PATI_TECH_CODE == txt.Text);
                        cboTech.EditValue = tranpatiTech != null ? (long?)tranpatiTech.First().ID : null;
                    }
                    else
                    {
                        cboTech.ShowPopup();
                    }
                }
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
                FillDatagctFormList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTranPatiTemp_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (HIS_TRAN_PATI_TEMP)gridViewTranPatiTemp.GetFocusedRow();
                if (this.currentData != null)
                {

                    ChangedDataRow(this.currentData);
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFocusEditor()
        {
            try
            {
                txtCode.Focus();
                txtCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ChangedDataRow(HIS_TRAN_PATI_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditor(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnSave.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = new HIS_TRAN_PATI_TEMP();
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                txtCode.Text = "";
                txtName.Text = "";
                txtKey.Text = "";
                txtForm.Text = "";
                txtTech.Text = "";
                txtReason.Text = "";
                txtMediOrg.Text = "";
                lblAddress.Text = "";

                cboReason.EditValue = null;
                cboForm.EditValue = null;
                cboMediOrg.EditValue = null;
                cboTech.EditValue = null;

                txtCondition.Text = null;
                txtDirection.Text = null;
                txtMethod.Text = null;
                txtTransporter.Text = null;
                txtVehicle.Text = null;
                txtUsedMedicine.Text = null;

                chkIsPublic.Checked = false;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                //ResetFormData();
                SetFocusEditor();
                FillDatagctFormList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnSave.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                HIS_TRAN_PATI_TEMP updateDTO = new HIS_TRAN_PATI_TEMP();

                if (this.currentData != null)
                {
                    updateDTO = currentData;
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<HIS_TRAN_PATI_TEMP>("api/HisTranPatiTemp/Create", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {

                        success = true;
                        FillDatagctFormList();
                        txtCode.Text = "";
                        txtName.Text = "";
                        //RefeshDataAfterSave(resultData);
                        btnCancel_Click(null, null);

                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<HIS_TRAN_PATI_TEMP>("api/HisTranPatiTemp/Update", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDatagctFormList();
                        btnCancel_Click(null, null);
                        //RefeshDataAfterSave(resultData);
                    }
                }

                if (success)
                {
                    FillDatagctFormList();
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_TRAN_PATI_TEMP updateDTO)
        {
            try
            {
                updateDTO.TRAN_PATI_TEMP_CODE = txtCode.Text.Trim();
                updateDTO.TRAN_PATI_TEMP_NAME = txtName.Text.Trim();
                updateDTO.IS_PUBLIC = chkIsPublic.Checked ? (short)1 : (short)0;
                updateDTO.MEDI_ORG_CODE = lstMediOrg.Where(o => o.ID == Convert.ToInt64(cboMediOrg.EditValue)).First().MEDI_ORG_CODE;
                updateDTO.MEDI_ORG_NAME = lstMediOrg.Where(o => o.ID == Convert.ToInt64(cboMediOrg.EditValue)).First().MEDI_ORG_NAME;
                updateDTO.TRAN_PATI_REASON_ID = cboReason.EditValue != null ? Convert.ToInt64(cboReason.EditValue) : 0;
                if (cboTech.EditValue != null)
                {
                    updateDTO.TRAN_PATI_TECH_ID = Convert.ToInt64(cboTech.EditValue);
                }
                else
                {
                    updateDTO.TRAN_PATI_TECH_ID = null;
                }

                updateDTO.TRAN_PATI_FORM_ID = cboForm.EditValue != null ? Convert.ToInt64(cboForm.EditValue) : 0;

                updateDTO.TREATMENT_DIRECTION = txtDirection.Text.Trim();
                updateDTO.PATIENT_CONDITION = txtCondition.Text.Trim();
                updateDTO.TREATMENT_METHOD = txtMethod.Text.Trim();
                updateDTO.TRANSPORT_VEHICLE = txtVehicle.Text.Trim();
                updateDTO.USED_MEDICINE = txtUsedMedicine.Text.Trim();
                updateDTO.TRANSPORTER = txtTransporter.Text.Trim();

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
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (HIS_TRAN_PATI_TEMP)gridViewTranPatiTemp.GetFocusedRow();
                    if (rowData != null)
                    {

                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisTranPatiTemp/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            this.ActionType = 1;
                            txtName.Text = "";
                            txtCode.Text = "";
                            EnableControlChanged(this.ActionType);
                            FillDatagctFormList();
                        }
                        MessageManager.Show(this, param, success);
                        btnCancel_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtName.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMediOrg.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;
            HIS_TRAN_PATI_TEMP success = new HIS_TRAN_PATI_TEMP();
            //bool notHandler = false;
            try
            {

                HIS_TRAN_PATI_TEMP data = (HIS_TRAN_PATI_TEMP)gridViewTranPatiTemp.GetFocusedRow();
                if (XtraMessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRAN_PATI_TEMP>("api/HisTranPatiTemp/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        rs = true;
                        FillDatagctFormList();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion
                    btnCancel_Click(null, null);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonUnlock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;
            HIS_CARER_CARD success = new HIS_CARER_CARD();
            //bool notHandler = false;

            try
            {

                HIS_TRAN_PATI_TEMP data = (HIS_TRAN_PATI_TEMP)gridViewTranPatiTemp.GetFocusedRow();
                if (XtraMessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_CARER_CARD>("api/HisTranPatiTemp/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        rs = true;
                        FillDatagctFormList();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion
                    btnCancel_Click(null, null);
                }

            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTranPatiTemp_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                HIS_TRAN_PATI_TEMP data = null;
                if (e.RowHandle > -1)
                {
                    data = (HIS_TRAN_PATI_TEMP)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryItemButtonLock : repositoryItemButtonUnlock);
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryItemButtonDelete : repositoryItemButtonDeleteDis);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDatagctFormList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
