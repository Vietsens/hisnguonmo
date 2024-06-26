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
using MOS.EFMODEL.DataModels;
//using HIS.Desktop.Plugins.HisHeinServiceType.Properties;

namespace HIS.Desktop.Plugins.HisHeinServiceType
{
    public partial class HisHeinServiceTypeForm : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE currentData;
        MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE resultData;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        int positionHandle = -1;

        #endregion

        public HisHeinServiceTypeForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
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

        public HisHeinServiceTypeForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();
                //pagingGrid = new PagingGrid();
                currentModule = module;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Loadform
        private void HisHeinServiceTypeForm_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
                InitRestoreLayoutGridViewFromXml(this.gridView1);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void MeShow()
        {
            SetDefaultValue();

            FillDatagctFormList();

            SetCaptionByLanguageKey();

            InitTabIndex();

            ValidateForm();

            SetDefaultFocus();
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
                ValidationSingleControl(txtName);
                ValidationSingleControl(txtCode);
                ValidationSingleControl(txtBHYTCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisHeinServiceType.Resource.Lang", typeof(HIS.Desktop.Plugins.HisHeinServiceType.HisHeinServiceTypeForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.layoutControl1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.bar2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.barButtonItem3.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.barButtonItem1.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.layoutControl5.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.layoutControl3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.layoutControl4.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.grlSTT.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclCode.Caption = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.gclCode.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclName.Caption = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.gclName.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclNumberOrder.Caption = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.gclNumberOrder.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclBHYTCODE.Caption = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.gclBHYTCODE.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.btnReset.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.layoutControl2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.btnFind.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.layoutControlItem2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.layoutControlItem3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.layoutControlItem13.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.bar1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("HisHeinServiceTypeForm.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>> apiResult = null;
                HisHeinServiceTypeFilter filter = new HisHeinServiceTypeFilter();
                //  SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>>(HIS.Desktop.Plugins.HisHeinServiceType.HisRequestUriStore.MOSHIS_HEIN_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>)apiResult.Data;
                    List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE> data1 = new List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>();
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            if (item.HEIN_SERVICE_TYPE_NAME.ToLower().Contains(txtFind.Text.ToLower()) || item.HEIN_SERVICE_TYPE_CODE.Contains(txtFind.Text))
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

        private void SetDefaultValue()
        {
            try
            {
                spNum.EditValue = null;
                SpParentNumOrder.EditValue = null;
                txtFind.Text = "";
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

        private void RefeshDataAfterSave(MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE data)
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

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE currentDTO)
        {
            try
            {
                //currentDTO.MACHINE_CODE = txtCode.Text.Trim();
                //currentDTO.MACHINE_NAME = txtName.Text.Trim();
                //currentDTO.SERIAL_NUMBER = txtSeri.Text.Trim();
                //currentDTO.MACHINE_GROUP_CODE = txtMachineGroupCode.Text.Trim();
                //if (cboSource.SelectedIndex==0)
                //{
                //    currentDTO.SOURCE_CODE = "1";
                //}
                //else if (cboSource.SelectedIndex==1)
                //{
                //    currentDTO.SOURCE_CODE = "2";
                //}
                //else if (cboSource.SelectedIndex==2)
                //{
                //       currentDTO.SOURCE_CODE = "3";
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisHeinServiceTypeFilter filter = new HisHeinServiceTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>>(HIS.Desktop.Plugins.HisHeinServiceType.HisRequestUriStore.MOSHIS_HEIN_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                positionHandle = -1;
                txtCode.Text = "";
                txtName.Text = "";
                txtFind.Text = "";
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
                SetFocusEditor();
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
                    MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE pData = (MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
					else if (e.Column.FieldName == "IN_TIME")
					{
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.BHYT_CODE_IN_TIME ?? 0);
						//else if (pData.SOURCE_CODE=="2")
						//{
						//    e.Value = "Xã hội hóa";
						//}
						//else if (pData.SOURCE_CODE=="3")
						//{
						//    e.Value = "Khác";
						//}
						}
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
                MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE data = null;
                if (e.RowHandle > -1)
                {
                    data = (MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
                this.currentData = (MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE)gridView1.GetFocusedRow();
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE data)
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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    txtCode.Text = data.HEIN_SERVICE_TYPE_CODE;
                    txtName.Text = data.HEIN_SERVICE_TYPE_NAME;
                    txtBHYTCode.Text = data.BHYT_CODE;
                    if (data.NUM_ORDER != null)
                    {
                        spNum.EditValue = data.NUM_ORDER;
                    }
                    else
                    {
                        spNum.EditValue = null;
                    }

                    if (data.PARENT_NUM_ORDER.HasValue)
                    {
                        SpParentNumOrder.EditValue = data.PARENT_NUM_ORDER;
                    }
                    else
                    {
                        SpParentNumOrder.EditValue = null;
                    }
                    txtOldBhytCode.Text = data.OLD_BHYT_CODE;
                    if (data.BHYT_CODE_IN_TIME != null)
                        dteInTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.BHYT_CODE_IN_TIME ?? 0) ?? DateTime.Now;
                    else
                        dteInTime.EditValue = null;
                    //txtSeri.Text = data.SERIAL_NUMBER;
                    //txtMachineGroupCode.Text = data.MACHINE_GROUP_CODE;
                    //if (data.SOURCE_CODE == "1")
                    //{
                    //    cboSource.SelectedIndex = 0;
                    //}
                    //else if (data.SOURCE_CODE=="2")
                    //{
                    //    cboSource.SelectedIndex = 1;
                    //}
                    //else if (data.SOURCE_CODE=="3")
                    //{
                    //    cboSource.SelectedIndex = 2;
                    //}
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                Boolean success = false;
                HIS_HEIN_SERVICE_TYPE updateDTO = new HIS_HEIN_SERVICE_TYPE();
                if (!dxValidationProvider1.Validate()) return;
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }

                if (spNum.EditValue != null && spNum.EditValue != "")
                {
                    updateDTO.NUM_ORDER = Convert.ToInt64(spNum.EditValue);
                }
                else
                {
                    updateDTO.NUM_ORDER = null;
                }

                if (SpParentNumOrder.EditValue != null && SpParentNumOrder.Value > 0)
                {
                    updateDTO.PARENT_NUM_ORDER = Convert.ToInt64(SpParentNumOrder.Value);
                }
                else
                {
                    updateDTO.PARENT_NUM_ORDER = null;
                }

                updateDTO.BHYT_CODE = txtBHYTCode.Text;
                updateDTO.OLD_BHYT_CODE = txtOldBhytCode.Text;
                if (dteInTime.EditValue != null && dteInTime.DateTime != DateTime.MinValue)
                    updateDTO.BHYT_CODE_IN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteInTime.DateTime);

				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                var resultData = new BackendAdapter(param).Post<HIS_HEIN_SERVICE_TYPE>(HIS.Desktop.Plugins.HisHeinServiceType.HisRequestUriStore.MOSHIS_HEIN_SERVICE_TYPE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);

				Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData));
                if (resultData != null)
                {
                    success = true;
                    FillDatagctFormList();
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
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }
        #endregion
    }
}
