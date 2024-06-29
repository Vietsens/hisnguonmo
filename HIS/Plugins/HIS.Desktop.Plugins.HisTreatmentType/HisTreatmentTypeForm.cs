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
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.ViewInfo;
using MOS.Filter;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.TreatmentType;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.HisTreatmentType
{
    public partial class HisTreatmentTypeForm : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE currentData;
        MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE resultData;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        int positionHandle = -1;

        #endregion

        public HisTreatmentTypeForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
            : base(module)
        {

            InitializeComponent();
            currentModule = module;
            this.delegateSelect = delegateData;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public HisTreatmentTypeForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {

            try
            {
                InitializeComponent();
                //pagingGrid = new PagingGrid();
                currentModule = module;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Loadform
        private void HisTreatmentTypeForm_Load(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = false;
                MeShow();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void MeShow()
        {
            LoadCombo();
            LoadComboRequiredService();

            SetDefaultValue();

            FillDatagctFormList();

            SetCaptionByLanguageKey();

            InitTabIndex();

            ValidateForm();

            SetDefaultFocus();
        }

        private void LoadCombo()
        {
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("HeinTreatmentTypeName", "", 100, 1));
            columnInfos.Add(new ColumnInfo("HeinTreatmentTypeCode", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("HeinTreatmentTypeCode", "HeinTreatmentTypeName", columnInfos, false, 350);
            ControlEditorLoader.Load(cboHeinTreatmentType, MOS.LibraryHein.Bhyt.HeinTreatmentType.HeinTreatmentTypeStore.Get(), controlEditorADO);
        }
        private void LoadComboRequiredService()
        {
            try
            {
                var listRequiredService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_LEAF == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboRequiredService, listRequiredService, controlEditorADO);
                cboRequiredService.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultFocus()
        {
            try
            {
                txtFind.Text = "";
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
                ValidationSingleControl(cboHeinTreatmentType);
                ValidationMaxLength(txtEndCodePrefix);
                //ValidationSingleControl(txtCode);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxLength(TextEdit txt)
        {
            try
            {
                ValidateMaxLength validRule = new ValidateMaxLength();
                validRule.txt = txt;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txt, validRule);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = this.currentModule.text;
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
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>> apiResult = null;
                HisTreatmentTypeFilter filter = new HisTreatmentTypeFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>>(HIS.Desktop.Plugins.HisTreatmentType.HisRequestUriStore.MOSHIS_TREATMENT_TYPE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>)apiResult.Data;
                    List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> data1 = new List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            if (item.TREATMENT_TYPE_NAME.ToLower().Contains(txtFind.Text.ToLower()) || item.TREATMENT_TYPE_CODE.ToLower().Contains(txtFind.Text.ToLower()))
                            {
                                data1.Add(item);
                            }
                        }
                        gridView1.GridControl.DataSource = data1;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisTreatmentTypeFilter filter)
        {
            filter.KEY_WORD = txtFind.Text;
        }

        private void SetDefaultValue()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentType.Resource.Lang", typeof(HIS.Desktop.Plugins.HisTreatmentType.HisTreatmentTypeForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.layoutControl1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.bar2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.barButtonItem3.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.barButtonItem1.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.barButtonItem2.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.btnSave.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.layoutControl3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.layoutControl4.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.grlSTT.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclCode.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.gclCode.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclName.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.gclName.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclHeinCode.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.grclHeinCode.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.btnReset.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //   this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.layoutControl2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.btnFind.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.layoutControlItem2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.layoutControlItem3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.c.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.c.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.bar1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCanhbao.Properties.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.CanhBao.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkChan.Properties.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.Chan.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                txtFind.Text = "";
                this.chkCanhbaoFeeDebt.Properties.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.chkCanhbaoFeeDebt.Properties.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkChanFeeDebt.Properties.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.chkChanFeeDebt.Properties.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkKhongKTFeeDebt.Properties.Caption = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.chkKhongKTFeeDebt.Properties.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.layoutControlItem27.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("HisTreatmentTypeForm.layoutControlItem27.OptionsToolTip.ToolTip", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region event
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

        private void ResetFormData()
        {
            try
            {
                if (!layoutControl1.IsInitialized) return;
                layoutControl1.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl1.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtCode.Focus();

                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl1.EndUpdate();


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshDataAfterSave(MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE data)
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(data);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE currentDTO)
        {
            try
            {
                currentDTO.TREATMENT_TYPE_CODE = txtCode.Text.Trim();
                currentDTO.TREATMENT_TYPE_NAME = txtName.Text.Trim();
                currentDTO.END_CODE_PREFIX = txtEndCodePrefix.Text.Trim();
                currentDTO.HEIN_TREATMENT_TYPE_CODE = cboHeinTreatmentType.EditValue.ToString();
                if (cboRequiredService.EditValue != null)
                    currentDTO.REQUIRED_SERVICE_ID = (long)cboRequiredService.EditValue;
                else
                    currentDTO.REQUIRED_SERVICE_ID = null;
                currentDTO.IS_ALLOW_RECEPTION = chkAllowReception.Checked ? (short)1 : (short)0;
                currentDTO.IS_NOT_ALLOW_UNPAUSE = chkIS_NOT_ALLOW_UNPAUSE.Checked ? (short)1 : (short)0;
                currentDTO.ALLOW_HOSPITALIZE_WHEN_PRES = chkAllowHospitalizeWhenPres.Checked ? (short)1 : (short)0;
                currentDTO.IS_NOT_ALLOW_SHARE_BED = chkIsNotAllowShareBed.Checked ? (short)1 : (short)0;
                currentDTO.IS_REQUIRED_SERVICE_BED = chkIsRequiredServiceBed.Checked ? (short?)1 : null;
                //if (chkUnfinishedDeposit.Checked)
                //{
                //    currentDTO.IS_DIS_DEPOSIT = 1;
                //}
                //else
                //{
                //    currentDTO.IS_DIS_DEPOSIT = null;
                //}
                //Set value Tùy chọn không hiển thị nút “Tạm ứng DV” ở màn hình viện phí
                if (chkAlwaysDisableServiceDeposit.Checked)
                {
                    currentDTO.DIS_SERVICE_DEPOSIT_OPTION = 1;
                }
                else if (chkDisableFinishedServiceDeposit.Checked)
                {
                    currentDTO.DIS_SERVICE_DEPOSIT_OPTION = 2;
                }
                else
                {
                    currentDTO.DIS_SERVICE_DEPOSIT_OPTION = null;
                }
                //Set value Tùy chọn không hiển thị nút “Tạm ứng” ở màn hình viện phí
                if (chkAlwaysDisableDeposit.Checked)
                {
                    currentDTO.DIS_DEPOSIT_OPTION = 1;
                }
                else if (chkDisableFinishedDeposit.Checked)
                {
                    currentDTO.DIS_DEPOSIT_OPTION = 2;
                }
                else
                {
                    currentDTO.DIS_DEPOSIT_OPTION = null;
                }

                if (chkCanhbao.Checked)
                {
                    currentDTO.UNSIGN_DOC_FINISH_OPTION = 1;
                }
                else if (chkChan.Checked)
                {
                    currentDTO.UNSIGN_DOC_FINISH_OPTION = 2;
                }
                else
                {
                    currentDTO.UNSIGN_DOC_FINISH_OPTION = null;
                }
                //if (chkUnfinishedServiceDeposit.Checked)
                //{
                //    currentDTO.IS_DIS_SERVICE_DEPOSIT = 1;
                //}
                //else
                //{
                //    currentDTO.IS_DIS_SERVICE_DEPOSIT = null;
                //}
                if (chkIsDisServiceRepay.Checked)
                {
                    currentDTO.IS_DIS_SERVICE_REPAY = 1;
                }
                else
                {
                    currentDTO.IS_DIS_SERVICE_REPAY = null;
                }
                if (chkCanhbaoTime.Checked)
                {
                    currentDTO.TRANS_TIME_OUT_TIME_OPTION = 1;
                }
                else if (chkChanTime.Checked)
                {
                    currentDTO.TRANS_TIME_OUT_TIME_OPTION = 2;
                }
                else
                {
                    currentDTO.TRANS_TIME_OUT_TIME_OPTION = null;
                }

                if (chkCanhbaoFeeDebt.Checked)
                {
                    currentDTO.FEE_DEBT_OPTION = 1;
                }
                else if (chkChanFeeDebt.Checked)
                {
                    currentDTO.FEE_DEBT_OPTION = 2;
                }
                else if (chkKhongKTFeeDebt.Checked)
                {
                    currentDTO.FEE_DEBT_OPTION = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentTypeFilter filter = new HisTreatmentTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>>(HIS.Desktop.Plugins.HisTreatmentType.HisRequestUriStore.MOSHIS_TREATMENT_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {

                btnSave.Enabled = false;
                positionHandle = -1;
                txtCode.Text = "";
                txtName.Text = "";
                chkCanhbao.Checked = false;
                chkChan.Checked = false;
                chkCanhbaoTime.Checked = false;
                chkChanTime.Checked = false;
                chkAllowReception.Checked = false;
                txtFind.Text = "";
                cboHeinTreatmentType.EditValue = null;
                chkKhongKTFeeDebt.Checked = false;
                chkChanFeeDebt.Checked = false;
                chkCanhbaoFeeDebt.Checked = false;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
                SetFocusEditor();
                this.chkCanhbao.Properties.Caption = "Cảnh báo";
                this.chkChan.Properties.Caption = "Chặn";
                chkAlwaysDisableServiceDeposit.Checked = false;
                chkDisableFinishedServiceDeposit.Checked = false;
                chkIsDisServiceRepay.Checked = false;
                chkAlwaysDisableDeposit.Checked = false;
                chkDisableFinishedDeposit.Checked = false;
                FillDatagctFormList();

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

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE pData = (MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "AllowReception")
                    {
                        e.Value = pData.IS_ALLOW_RECEPTION == 1;
                    }
                    else if (e.Column.FieldName == "IS_NOT_ALLOW_UNPAUSE_STR")
                    {
                        e.Value = pData.IS_NOT_ALLOW_UNPAUSE == 1;
                    }
                    else if (e.Column.FieldName == "AllowHospitalizeWhenPres")
                    {
                        e.Value = pData.ALLOW_HOSPITALIZE_WHEN_PRES == 1;
                    }
                    else if (e.Column.FieldName == "IS_NOT_ALLOW_SHARE_BED_STR")
                    {
                        e.Value = pData.IS_NOT_ALLOW_SHARE_BED == 1;
                    }

                    //else if (e.Column.FieldName=="SOURCE")
                    //{
                    //if (pData.SOURCE_CODE=="1")
                    //{
                    //    e.Value = "Ngân sách";
                    //}
                    //else if (pData.SOURCE_CODE=="2")
                    //{
                    //    e.Value = "Xã hội hóa";
                    //}
                    //else if (pData.SOURCE_CODE=="3")
                    //{
                    //    e.Value = "Khác";
                    //}
                    // }

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
                MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE data = null;
                if (e.RowHandle > -1)
                {
                    data = (MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnLock : btnUnLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDeleteEnable : btnDeleteDisable);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE)gridView1.GetFocusedRow();
                if (this.currentData != null)
                {
                    btnSave.Enabled = true;
                    ChangedDataRow(this.currentData);
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE data)
        {
            try
            {
                if (data != null)
                {

                    txtCode.Text = data.TREATMENT_TYPE_CODE;
                    txtName.Text = data.TREATMENT_TYPE_NAME;
                    txtEndCodePrefix.Text = data.END_CODE_PREFIX;
                    cboHeinTreatmentType.EditValue = data.HEIN_TREATMENT_TYPE_CODE;
                    cboRequiredService.EditValue = data.REQUIRED_SERVICE_ID;
                    chkAllowReception.Checked = data.IS_ALLOW_RECEPTION == 1;
                    chkIS_NOT_ALLOW_UNPAUSE.Checked = data.IS_NOT_ALLOW_UNPAUSE == 1;
                    chkAllowHospitalizeWhenPres.Checked = data.ALLOW_HOSPITALIZE_WHEN_PRES == 1;
                    chkIsNotAllowShareBed.Checked = data.IS_NOT_ALLOW_SHARE_BED == 1;
                    chkIsRequiredServiceBed.Checked= data.IS_REQUIRED_SERVICE_BED == 1;
                    //chkUnfinishedServiceDeposit.Checked = data.IS_DIS_SERVICE_DEPOSIT == 1;
                    chkIsDisServiceRepay.Checked = data.IS_DIS_SERVICE_REPAY == 1;
                    //chkUnfinishedDeposit.Checked = data.IS_DIS_DEPOSIT == 1;
                    //Tạm ứng DV
                    if (data.DIS_SERVICE_DEPOSIT_OPTION == 1)
                    {
                        chkAlwaysDisableServiceDeposit.Checked = true;
                    }
                    else if (data.DIS_SERVICE_DEPOSIT_OPTION == 2)
                    {
                        chkDisableFinishedServiceDeposit.Checked = true;
                    }
                    else
                    {
                        chkAlwaysDisableServiceDeposit.Checked = false;
                        chkDisableFinishedServiceDeposit.Checked = false;
                    }
                    //Tạm ứng
                    if (data.DIS_DEPOSIT_OPTION == 1)
                    {
                        chkAlwaysDisableDeposit.Checked = true;
                    }
                    else if (data.DIS_DEPOSIT_OPTION == 2)
                    {
                        chkDisableFinishedDeposit.Checked = true;
                    }
                    else
                    {
                        chkAlwaysDisableDeposit.Checked = false;
                        chkDisableFinishedDeposit.Checked = false;
                    }
                    if (data.UNSIGN_DOC_FINISH_OPTION == 1)
                    {
                        chkCanhbao.Checked = true;
                    }
                    else if (data.UNSIGN_DOC_FINISH_OPTION == 2)
                    {
                        chkChan.Checked = true;
                    }
                    else
                    {
                        chkCanhbao.Checked = false;
                        chkChan.Checked = false;
                    }
                    if (data.TRANS_TIME_OUT_TIME_OPTION == 1)
                    {
                        chkCanhbaoTime.Checked = true;
                    }
                    else if (data.TRANS_TIME_OUT_TIME_OPTION == 2)
                    {
                        chkChanTime.Checked = true;
                    }
                    else
                    {
                        chkCanhbaoTime.Checked = false;
                        chkChanTime.Checked = false;
                    }

                    if (data.FEE_DEBT_OPTION == 1)
                    {
                        chkCanhbaoFeeDebt.Checked = true;
                    }
                    else if (data.FEE_DEBT_OPTION == 2)
                    {
                        chkChanFeeDebt.Checked = true;
                    }
                    else if (data.FEE_DEBT_OPTION == null)
                    {
                        chkKhongKTFeeDebt.Checked = true;
                    }
                    else
                    {
                        chkCanhbaoFeeDebt.Checked = false;
                        chkChanFeeDebt.Checked = false;
                        chkKhongKTFeeDebt.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

        private void txtCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtName.Focus();
            }
        }

        private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnReset.Focus();
            }
        }

        private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
                gridView1.Focus();
            }
        }
        #endregion

        #region ShortCut
        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnReset.Enabled)
            {
                btnReset_Click(null, null);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind_Click(null, null);
        }
        #endregion

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnSave.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE updateDTO = new MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>(HisRequestUriStore.MOSHIS_TREATMENT_TYPE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDatagctFormList();
                    RefeshDataAfterSave(resultData);
                }
                if (success)
                {
                    BackendDataWorker.Reset<HIS_TREATMENT_TYPE>();
                    SetFocusEditor();
                }
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboHeinTreatmentType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAllowReception.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkIS_NOT_ALLOW_UNPAUSE_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAllowHospitalizeWhenPres.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkAllowReception_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIS_NOT_ALLOW_UNPAUSE.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAllowHospitalizeWhenPres_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsNotAllowShareBed.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCanhbao_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkChan.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkChan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                try
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        chkCanhbaoTime.Focus();
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCanhbao_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (chkCanhbao.Checked)
                {
                    chkChan.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkChan_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (chkChan.Checked)
                {
                    chkCanhbao.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRequiredService_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRequiredService.Text.Trim() == "")
                {
                    cboRequiredService.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboRequiredService_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRequiredService.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRequiredService_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                cboRequiredService.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCanhbaoTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCanhbaoTime.Checked)
                {
                    chkChanTime.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCanhbaoTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkChanTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkChanTime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkChanTime.Checked)
                {
                    chkCanhbaoTime.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkChanTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                try
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        chkCanhbaoTime.Focus();
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkUnfinishedServiceDeposit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkDisableFinishedServiceDeposit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }

        private void chkFinishedServiceDeposit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAlwaysDisableDeposit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }

        private void chkUnfinishedDeposit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkDisableFinishedDeposit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }

        private void chkFinishedDeposit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCanhbao.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }

        private void chkIsDisServiceRepay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAlwaysDisableServiceDeposit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }

        private void chkUnfinishedServiceDeposit_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAlwaysDisableServiceDeposit.Checked)
                {
                    chkDisableFinishedServiceDeposit.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkFinishedServiceDeposit_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDisableFinishedServiceDeposit.Checked)
                {
                    chkAlwaysDisableServiceDeposit.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkUnfinishedDeposit_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAlwaysDisableDeposit.Checked)
                {
                    chkDisableFinishedDeposit.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkFinishedDeposit_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDisableFinishedDeposit.Checked)
                {
                    chkAlwaysDisableDeposit.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsNotAllowShareBed_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsDisServiceRepay.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCanhbaoFeeDebt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCanhbaoFeeDebt.Checked)
                {
                    chkChanFeeDebt.Checked = false;
                    chkKhongKTFeeDebt.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCanhbaoFeeDebt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkChanFeeDebt.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkChanFeeDebt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkChanFeeDebt.Checked)
                {
                    chkCanhbaoFeeDebt.Checked = false;
                    chkKhongKTFeeDebt.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkChanFeeDebt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhongKTFeeDebt.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongKTFeeDebt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkKhongKTFeeDebt.Checked)
                {
                    chkCanhbaoFeeDebt.Checked = false;
                    chkChanFeeDebt.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongKTFeeDebt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
