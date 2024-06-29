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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using System;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisServiceCondition.HisServiceCondition;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.HisServiceCondition.Validations;
using HIS.Desktop.Plugins.HisServiceCondition.Resources;
using System.Globalization;

namespace HIS.Desktop.Plugins.HisServiceCondition
{
    public partial class frmHisServiceCondition : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION currentData;
        MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION currentDataRow;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region Construct
        public frmHisServiceCondition(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                //gridControlConditionalService.ToolTipController = toolTipControllerGrid;

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
        #endregion

        #region Methods
        private void frmHisServiceCondition_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                // Inital Combo Service
                InitComboService("");

                //Load du lieu
                FillDataToGridControl();

                // Reset Controls
                ResetFormData();

                //Fill data into datasource combo
                FillDataToControlsForm(null);

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
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
                txtSearch.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION data)
        {
            try
            {
                if (data != null)
                {
                    ResetFormData();
                    FillDataToControlsForm(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    this.currentData = data;
                    positionHandle = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControlsForm(MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION data)
        {
            try
            {
                if (data != null)
                {
                    txtServiceCode.Text = data.SERVICE_CONDITION_CODE;
                    txtServiceName.Text = data.SERVICE_CONDITION_NAME;
                    if (data.HEIN_RATIO != null)
                    {
                        string value = ((data.HEIN_RATIO ?? 0) * 100).ToString();
                        int index = value.IndexOf(',');
                        if (value.Length > index + 4)
                            value = value.Substring(0, index+3);
                        spinEditHeinRatio.Value = Inventec.Common.TypeConvert.Parse.ToDecimal(value);
                    }
                    //if (data.HEIN_RATIO >= 1)
                    //    spinEditHeinRatio.Value = 100;
                    //else
                    //    spinEditHeinRatio.Value = Inventec.Common.TypeConvert.Parse.ToInt64((data.HEIN_RATIO * 100).ToString().Replace(',', '0')) / 1000;
                    if (data.HEIN_PRICE != null)
                    {
                        spinEditHeinPrice.Value = data.HEIN_PRICE.Value;
                        //spinEditHeinPrice.Text = data.HEIN_PRICE.Value.ToString("N", new CultureInfo("en-US"));
                    }
                    gridLookUpEditService.EditValue = data.SERVICE_ID;
                    var service = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == data.SERVICE_ID).FirstOrDefault();
                    if (service != null)
                    {
                        txtService.Text = service.SERVICE_CODE;
                    }
                    else
                    {
                        txtService.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboService(string service)
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == 1 && o.SERVICE_CODE.ToUpper() == service.Trim().ToString().ToUpper()).ToList();

                if (data == null || data.Count == 0)
                    data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == 1 && o.SERVICE_CODE.ToUpper().Contains(service.Trim().ToString().ToUpper())).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(gridLookUpEditService, data, controlEditorADO);
                //if (data != null && data.Count == 1)
                //    gridLookUpEditService.EditValue = data.First().SERVICE_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                InitComboService(txtService.Text.Trim());
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
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlConditionalService);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION>> apiResult = null;
                HisServiceConditionFilter filter = new HisServiceConditionFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                dnNavigation.DataSource = null;
                gridViewConditionalService.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION>>(HisRequestUriStore.SERVICE_CONDTION_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        currentDataRow = data.First();
                        dnNavigation.DataSource = data;
                        gridViewConditionalService.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewConditionalService.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref MOS.Filter.HisServiceConditionFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisServiceCondition.Resources.Lang", typeof(HIS.Desktop.Plugins.HisServiceCondition.frmHisServiceCondition).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCode.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.gridColumnCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnName.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.gridColumnName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.gridColumnCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEditTime.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.gridColumnEditTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEditor.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.gridColumnEditor.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfoItem.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciServiceCode.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.lciServiceCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciServiceName.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.lciServiceName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisServiceCondition.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                dicOrderTabIndexControl.Add("txtServiceCode", 0);
                dicOrderTabIndexControl.Add("txtServiceName", 1);
                dicOrderTabIndexControl.Add("spinEditHeinRatio", 2);
                dicOrderTabIndexControl.Add("spinEditHeinPrice", 3);
                dicOrderTabIndexControl.Add("txtService", 4);

                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
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

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtServiceCode, 20, true);
                ValidationSingleControl(spinEditHeinRatio, 3, false);
                ValidationSingleControl(txtServiceName, 1000, true);
                ValidationSingleControl(gridLookUpEditService, 500, true);
                ValidationSingleControl(txtService, 25, true);
               // ValidationspnNumberLimitValue();
                ValidationspnNumberLimitDecimalNo();
                ValidationspinNumberHeinPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationspnNumberLimitDecimalNo()
        {
            try
            {
                ValidationSpinDemacialNo FromRule = new ValidationSpinDemacialNo();
                FromRule.spnNumberLimitDemacialNo = spinEditHeinRatio;
                //FromRule.ErrorText = FromRule.Errtext;
                FromRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(spinEditHeinRatio, FromRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationspinNumberHeinPrice()
        {
            try
            {
                ValidationSpinHeinPrice FromRule = new ValidationSpinHeinPrice();
                FromRule.spnNumber = spinEditHeinPrice;
                //FromRule.ErrorText = FromRule.Errtext;
                FromRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(spinEditHeinPrice, FromRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationspnNumberLimitValue()
        {
            try
            {
                Validation FromRule = new Validation();
                FromRule.spnNumberLimitValue = spinEditHeinRatio;
                FromRule.ErrorText = ResourceMessage.GiaTriNamNgoaiKhoangChoPhep;
                FromRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(spinEditHeinRatio, FromRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, int maxlength, bool Isrequired)
        {
            try
            {
                ControlMaxLengthValidationRule validRule = new ControlMaxLengthValidationRule();
                validRule.editor = control;
                validRule.maxLength = maxlength;
                validRule.IsRequired = Isrequired;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void SetDefaultFocus()
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetFocusEditor()
        {
            try
            {
                //TODO

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
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();

                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceConditionFilter filter = new HisServiceConditionFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION>>(HisRequestUriStore.SERVICE_CONDTION_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION currentDTO)
        {
            try
            {
                currentDTO.SERVICE_CONDITION_CODE = txtServiceCode.Text.Trim();
                if (spinEditHeinRatio.Text != "")
                    currentDTO.HEIN_RATIO = spinEditHeinRatio.Value / 100;
                else currentDTO.HEIN_RATIO = null;
                if (!string.IsNullOrEmpty(spinEditHeinPrice.Text))
                {
                    currentDTO.HEIN_PRICE = spinEditHeinPrice.Value;
                }
                else 
                {
                    currentDTO.HEIN_PRICE = null;
                }
                
                currentDTO.SERVICE_CONDITION_NAME = txtServiceName.Text.Trim();
                currentDTO.SERVICE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUpEditService.EditValue.ToString());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION)gridViewConditionalService.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION>(rowData, data);
                    gridViewConditionalService.RefreshRow(gridViewConditionalService.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string Tien(string XXX)
        {
            string KetQua = "";
            int DoDai = XXX.Length;
            for (int i = DoDai - 1; i > -1; i--)
            {
                KetQua = XXX[i] + KetQua;
                if ((DoDai - i == 3 && DoDai > 3) || (DoDai - i == 6 && DoDai > 6))
                    KetQua = "." + KetQua;
            }
            return KetQua;
        }

        #endregion

        #region Buttons Handle
        private void btnEdit_Click(object sender, EventArgs e)
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
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION updateDTO = new MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION>(HisRequestUriStore.SERVICE_CONDTION_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION>(HisRequestUriStore.SERVICE_CONDTION_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<HIS_SERVICE_CONDITION>();
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                this.currentData = null;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                SetFocusEditor();
                txtServiceCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            WaitingManager.Show();
            List<object> sendObj = new List<object>() { this.currentDataRow.ID };
            CallModule("HIS.Desktop.Plugins.HisServiceConditionImport", sendObj);
            WaitingManager.Hide();
        }

        private void CallModule(string moduleLink, List<object> data)
        {
            try
            {
                CallModule callModule = new CallModule(moduleLink, 0, 0, data);
                //CallModule callModule = new CallModule(moduleLink, currentModule.RoomId, currentModule.RoomTypeId, data);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var rowData = (MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION)gridViewConditionalService.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.SERVICE_CONDTION_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_SERVICE_CONDITION>();
                            FillDataToGridControl();
                        }
                        MessageManager.Show(this, param, success);
                    }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridViewConditionalService_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION)gridViewConditionalService.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowData), rowData));
                    currentDataRow = rowData;
                    ChangedDataRow(rowData);
                }
                Inventec.Common.Logging.LogSystem.Warn("Log 1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        

        private void gridViewConditionalService_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    short? isactive = (short?)view.GetRowCellValue(e.RowHandle, "IS_ACTIVE");
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (isactive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGUnLock : btnGLock);

                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        try
                        {
                            if (isactive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = btnGEnableDelete;
                            else
                                e.RepositoryItem = btnGDisableDelete;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewConditionalService_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION pData = (MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME) ?? "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFIER_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME) ?? "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "HEIN_RATIO_STR")
                    {
                        try
                        {
                            //if (pData.HEIN_RATIO >= 1)
                            //    e.Value = 100;
                            //else
                            //    e.Value = pData.HEIN_RATIO != null ? (Inventec.Common.TypeConvert.Parse.ToInt64((pData.HEIN_RATIO * 100).ToString().Replace(',', '0')) / 1000).ToString() : "";
                            if (pData.HEIN_RATIO != null)
                                e.Value = (Inventec.Common.TypeConvert.Parse.ToFloat((pData.HEIN_RATIO * 100).ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "HEIN_PRICE_STR")
                    {
                        try
                        {
                            if (pData.HEIN_PRICE != null)
                                e.Value = Tien(Convert.ToInt64(pData.HEIN_PRICE).ToString());
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "STATUS")
                    {
                        try
                        {
                            if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "SERVICE_ID_STR")
                    {
                        try
                        {
                            var service = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == pData.SERVICE_ID).FirstOrDefault();
                            if (service != null)
                            {
                                e.Value = service.SERVICE_NAME;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewConditionalService_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    short? isactive = (short?)view.GetRowCellValue(e.RowHandle, "IS_ACTIVE");
                    if (e.Column.FieldName == "STATUS")
                    {
                        if (isactive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            e.Appearance.ForeColor = Color.Red;
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_SERVICE_CONDITION result = new HIS_SERVICE_CONDITION();
            bool success = false;
            try
            {

                HIS_SERVICE_CONDITION data = (HIS_SERVICE_CONDITION)gridViewConditionalService.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_CONDITION>(HisRequestUriStore.HIS_SERVICE_CONDITION_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_SERVICE_CONDITION result = new HIS_SERVICE_CONDITION();
            bool success = false;
            try
            {

                HIS_SERVICE_CONDITION data = (HIS_SERVICE_CONDITION)gridViewConditionalService.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_CONDITION>(HisRequestUriStore.HIS_SERVICE_CONDITION_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtServiceName.Focus();
                    txtServiceName.Select();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtServiceName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinEditHeinRatio.Focus();
                    spinEditHeinRatio.Select();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtService_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (!String.IsNullOrWhiteSpace(txtService.Text))
                    //{
                    //    var service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE.ToUpper() == txtService.Text.ToUpper());
                    //    if (service != null)
                    //    {
                    //        gridLookUpEditService.EditValue = service.ID;
                    //        txtService.Text = service.SERVICE_CODE;
                    //        if (ActionType == GlobalVariables.ActionAdd)
                    //        {
                    //            btnAdd.Focus();
                    //        }
                    //        if (ActionType == GlobalVariables.ActionEdit)
                    //        {
                    //            btnEdit.Focus();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        gridLookUpEditService.Focus();
                    //        gridLookUpEditService.ShowPopup();
                    //    }
                    //}
                    //else
                    //{
                    //    gridLookUpEditService.Focus();
                    //    gridLookUpEditService.ShowPopup();
                    //}
                    //var rs = BackendAdapter.
                    if (txtService.Text != "")
                    {
                        var rs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == 1 && o.SERVICE_CODE.ToUpper() == txtService.Text.Trim().ToString().ToUpper()).ToList();
                        if (rs != null && rs.Count == 1)
                        {
                            V_HIS_SERVICE dataCurrent = new V_HIS_SERVICE();
                            dataCurrent = rs.FirstOrDefault();
                            //gridLookUpEditService.Text = dataCurrent.SERVICE_NAME;
                            gridLookUpEditService.EditValue = dataCurrent.ID;
                            txtService.Text = dataCurrent.SERVICE_CODE;

                            if (ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled == true)
                            {
                                btnAdd.Focus();
                                btnAdd.Select();
                            }
                            if (ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled == true)
                            {
                                btnEdit.Select();
                                btnEdit.Focus();
                            }
                            //gridLookUpEditService.Text = dataCurrent.SERVICE_NAME;
                        }
                    }
                    else
                    {
                        InitComboService(txtService.Text.Trim());
                        gridLookUpEditService.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEditService_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (gridLookUpEditService.EditValue != null && gridLookUpEditService.EditValue.ToString() != "")
                    {
                        var service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == (long)gridLookUpEditService.EditValue);
                        if (service != null)
                        {
                            txtService.Text = service.SERVICE_CODE;
                            if (ActionType == GlobalVariables.ActionAdd)
                            {
                                btnAdd.Focus();
                            }
                            if (ActionType == GlobalVariables.ActionEdit)
                            {
                                btnEdit.Enabled = true;
                                btnEdit.Focus();
                            }
                        }
                        else
                        {
                            gridLookUpEditService.Focus();
                            //cboRoom.ShowPopup();
                        }
                    }
                    else
                    {
                        gridLookUpEditService.Focus();
                        //cboRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEditService_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    gridLookUpEditService.EditValue = null;
                    txtService.Text = "";
                    txtService.Focus();
                    InitComboService("");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewConditionalService.Focus();
                    gridViewConditionalService.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION)gridViewConditionalService.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtService_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                InitComboService("");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinEditHeinRatio_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    spinEditHeinPrice.Focus();
                    spinEditHeinPrice.Select();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }

        private void gridLookUpEditService_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled == true)
                    {
                        btnAdd.Focus();
                        btnAdd.Select();
                    }
                    if (ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled == true)
                    {
                        btnEdit.Select();
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void spinEditHeinPrice_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    txtService.Focus();
                    txtService.Select();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }

        private void spinEditHeinPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    txtService.Focus();
                    txtService.SelectAll();
                }
                else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

