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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.SDO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.HisTestIndex
{
    public partial class frmHisTestIndex : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        List<HIS_TEST_INDEX_GROUP> TestIndexGroupList;
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<V_HIS_MATERIAL_TYPE> materialTypeSubs = null;
        #endregion

        #region Construct
        public frmHisTestIndex(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

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

        #region Private method
        private void frmHisTestIndex_Load(object sender, EventArgs e)
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

        private void SetCaptionByLanguageKey()
        {
            try
            {

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisTestIndex.Resources.Lang", typeof(HIS.Desktop.Plugins.HisTestIndex.frmHisTestIndex).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTestIndexCode.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColTestIndexCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTestIndexCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColTestIndexCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTestIndexName.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColTestIndexName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTestIndexName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColTestIndexName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn6.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());                            
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisTestIndex.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisTestIndex.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTestIndexUnit.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisTestIndex.cboTestIndexUnit.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkTestServiceTypeId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisTestIndex.lkTestServiceTypeId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTestIndexGroupID.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisTestIndex.cboTestIndexGroupID.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.licTestIndexGroupID.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.licTestIndexGroupID.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSampleRoomCode.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.lciSampleRoomCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSampleRoomName.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.lciSampleRoomName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRoomTypeId.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.lciRoomTypeId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.layoutControlItem17.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsToCalculateEgfr.ToolTip = Inventec.Common.Resource.Get.Value("frmHisTestIndex.chkIsToCalculateEgfr.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciResultsGroupA.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.lciResultsGroupA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciResultsGroupB.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.lciResultsGroupB.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciResultsGroupAB.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.lciResultsGroupAB.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciResultsGroupO.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.lciResultsGroupO.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciResultsRHCong.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.lciResultsRHCong.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciResultsRHTru.Text = Inventec.Common.Resource.Get.Value("frmHisTestIndex.lciResultsRHTru.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
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
                ResetFormData();
                EnableControlChanged(this.ActionType);
                this.currentData = null;
                cboTestServiceType.EditValue = null;
                dxValidationProviderEditorInfo.RemoveControlError(spinNormalAmount);
                dxValidationProviderEditorInfo.RemoveControlError(txtTestIndexCode);
                dxValidationProviderEditorInfo.RemoveControlError(txtTestIndexName);
                dxValidationProviderEditorInfo.RemoveControlError(lkTestServiceTypeId);
                dxValidationProviderEditorInfo.RemoveControlError(cboTestIndexUnit);
                dxValidationProviderEditorInfo.RemoveControlError(txtResultsGroupA);
                dxValidationProviderEditorInfo.RemoveControlError(txtResultsGroupB);
                dxValidationProviderEditorInfo.RemoveControlError(txtResultsGroupAB);
                dxValidationProviderEditorInfo.RemoveControlError(txtResultsGroupO);
                dxValidationProviderEditorInfo.RemoveControlError(txtResultsRHPlus);
                dxValidationProviderEditorInfo.RemoveControlError(txtResultsRHMinus);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                //dicOrderTabIndexControl.Add("txtTestIndexCode", 0);
                //dicOrderTabIndexControl.Add("txtTestIndexName", 1);
                //dicOrderTabIndexControl.Add("lkRoomId", 2);


                //if (dicOrderTabIndexControl != null)
                //{
                //    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                //    {
                //        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                //    }
                //}
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

        private void FillDataToControlsForm()
        {
            try
            {
                this.materialTypeSubs = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_ACTIVE == 1 && o.IS_CHEMICAL_SUBSTANCE == 1).ToList();
                InitComboMaterialType(this.materialTypeSubs);
                InitComboTestServiceTypeId();
                InitComboTestIndexUnitId();
                InitComboTestServiceTypeSearch();
                InitTestIndexGroupID();

                AddItemsToComboBox(cboIndexType, new List<string> { "Albumin niệu", "Protein niệu", "Creatinin niệu" });
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddItemsToComboBox(ComboBoxEdit comboBox, List<string> items)
        {
            comboBox.Properties.Items.Clear();

            foreach (var item in items)
            {
                comboBox.Properties.Items.Add(item);
            }
        }
        private void InitComboMaterialType(List<V_HIS_MATERIAL_TYPE> materialTypes)
        {
            try
            {

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMaterialType, materialTypes, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboTestServiceTypeId()
        {
            try
            {
                var data = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == 1 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN || o.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__XN)).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(lkTestServiceTypeId, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTestIndexUnitId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTestIndexUnitFilter filter = new HisTestIndexUnitFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_TEST_INDEX_UNIT>>("api/HisTestIndexUnit/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TEST_INDEX_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TEST_INDEX_UNIT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TEST_INDEX_UNIT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboTestIndexUnit, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTestServiceTypeSearch()
        {
            try
            {
                var data = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == 1 && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboTestServiceType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitTestIndexGroupID()
        {
            try
            {
                TestIndexGroupList = new List<HIS_TEST_INDEX_GROUP>();
                CommonParam param = new CommonParam();
                HisTestIndexGroupFilter filter = new HisTestIndexGroupFilter();
                TestIndexGroupList = new BackendAdapter(param).Get<List<HIS_TEST_INDEX_GROUP>>("api/HisTestIndexGroup/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TEST_INDEX_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TEST_INDEX_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TEST_INDEX_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboTestIndexGroupID, TestIndexGroupList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
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
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX>> apiResult = null;
                HisTestIndexViewFilter filter = new HisTestIndexViewFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX>>(HisRequestUriStore.MOSHIS_TEST_INDEX_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX>)apiResult.Data;
                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewFormList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisTestIndexViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
                if (cboTestServiceType.EditValue != null)
                {
                    List<long> serviceIds = new List<long>();
                    serviceIds.Add(Inventec.Common.TypeConvert.Parse.ToInt64(cboTestServiceType.EditValue.ToString()));
                    filter.SERVICE_IDs = serviceIds;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX)gridviewFormList.GetFocusedRow();
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

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX)gridviewFormList.GetFocusedRow();
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

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX pData = (MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "TEST_INDEX_GROUP_ID_STR")
                    {

                        e.Value = this.TestIndexGroupList != null && this.TestIndexGroupList.Count > 0 ? this.TestIndexGroupList.Where(o => o.ID == pData.TEST_INDEX_GROUP_ID).FirstOrDefault().TEST_INDEX_GROUP_NAME : "";

                    }
                    else if (e.Column.FieldName == "IS_TO_CALCULATE_EGFR_STR")
                    {
                        e.Value = pData.IS_TO_CALCULATE_EGFR == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_HBSAG_STR")
                    {
                        e.Value = pData.IS_HBSAG == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_HCV_STR")
                    {
                        e.Value = pData.IS_HCV == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_HIV_STR")
                    {
                        e.Value = pData.IS_HIV == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_BLOOD_ABO_STR")
                    {
                        e.Value = pData.IS_BLOOD_ABO == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_BLOOD_RH_STR")
                    {
                        e.Value = pData.IS_BLOOD_RH == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "TEST_SERVICE_TYPE_NAME")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == pData.TEST_SERVICE_TYPE_ID);
                            if (data != null)
                            {
                                e.Value = data.SERVICE_NAME;
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
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
                    else if (e.Column.FieldName == "NORMATION_AMOUNT_DISPLAY")
                    {
                        if (pData.NORMATION_AMOUNT.HasValue)
                        {
                            e.Value = pData.NORMATION_AMOUNT + " " + pData.SERVICE_UNIT_NAME + "/" + "Lần";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX)gridviewFormList.GetFocusedRow();

                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX>)[dnNavigation.Position];
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX data)
        {
            try
            {
                if (data != null)
                {
                    
                    txtConversionRate.EditValue = data.CONVERT_RATIO_TYPE;
                    txtDefaultResult.Text = data.DEFAULT_VALUE;
                    txtBHYTCode.Text = data.BHYT_CODE;
                    txtBHYTName.Text = data.BHYT_NAME;
                    txtTestIndexCode.Text = data.TEST_INDEX_CODE;
                    txtTestIndexName.Text = data.TEST_INDEX_NAME;
                    lkTestServiceTypeId.EditValue = data.TEST_SERVICE_TYPE_ID;
                    cboTestIndexUnit.EditValue = data.TEST_INDEX_UNIT_ID;
                    cboTestIndexGroupID.EditValue = data.TEST_INDEX_GROUP_ID;
                    cboMaterialType.EditValue = data.MATERIAL_TYPE_ID;
                    txtMaterialTypeCode.Text = data.MATERIAL_TYPE_CODE;
                    spinNormalAmount.EditValue = data.NORMATION_AMOUNT;
                    spNumOrder.EditValue = data.NUM_ORDER;
                    chkNotShowService.Checked = data.IS_NOT_SHOW_SERVICE == 1 ? true : false;
                    chkCsBatThuong.Checked = data.IS_IMPORTANT == 1 ? true : false;
                    chkIsToCalculateEgfr.Checked = data.IS_TO_CALCULATE_EGFR == 1 ? true : false;
                    chkHbsAg.Checked = data.IS_HBSAG == 1 ? true : false;
                    chkHCV.Checked = data.IS_HCV == 1 ? true : false;
                    chkHIV.Checked = data.IS_HIV == 1 ? true : false;
                    chkABO.Checked = data.IS_BLOOD_ABO == 1 ? true : false;
                    chkRH.Checked = data.IS_BLOOD_RH == 1 ? true : false;
                    chkIsTestHamonyBlood.Checked = data.IS_TEST_HARMONY_BLOOD == 1;
                    labelControl1.Text = !String.IsNullOrWhiteSpace(data.SERVICE_UNIT_NAME) ? data.SERVICE_UNIT_NAME + "/Lần" : "";
                    spinNormalAmount.Enabled = data.MATERIAL_TYPE_ID.HasValue ? true : false;
                    txtResultsGroupA.Text = data.RESULT_BLOOD_A;
                    txtResultsGroupB.Text = data.RESULT_BLOOD_B;
                    txtResultsGroupAB.Text = data.RESULT_BLOOD_AB;
                    txtResultsGroupO.Text = data.RESULT_BLOOD_O;
                    txtResultsRHPlus.Text = data.RESULT_BLOOD_RH_PLUS;
                    txtResultsRHMinus.Text = data.RESULT_BLOOD_RH_MINUS;
                    if (data.TEST_INDEX_TYPE == 1)
                    {
                        cboIndexType.SelectedIndex = 0;
                    }
                    else if (data.TEST_INDEX_TYPE == 2)
                    {           
                        cboIndexType.SelectedIndex = 1;
                    }
                    else if (data.TEST_INDEX_TYPE == 3)
                    {
                        cboIndexType.SelectedIndex = 2;
                    }
                    else 
                    {
                        cboIndexType.EditValue = "";
                    }

                    if (data.CONVERT_RATIO_MLCT != null)
                    {
                        txtMLCT.Text = Convert.ToDecimal(data.CONVERT_RATIO_MLCT).ToString();
                    }
                    else
                    {
                        txtMLCT.Text = ""; 
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
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
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            txtTestIndexCode.Focus();
                            txtTestIndexCode.SelectAll();
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_TEST_INDEX currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTestIndexFilter filter = new HisTestIndexFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TEST_INDEX>>(HisRequestUriStore.MOSHIS_TEST_INDEX_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        #region Button handler
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

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                //FillDataToGridControl();
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
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX)gridviewFormList.GetFocusedRow();
                CommonParam param = new CommonParam();
                HisTestIndexViewFilter filter = new HisTestIndexViewFilter();
                filter.ID = rowData.ID;
                var data = new BackendAdapter(param).Get<System.Collections.Generic.List<HIS_TEST_INDEX>>(HisRequestUriStore.MOSHIS_TEST_INDEX_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (rowData != null)
                {
                    bool success = false;

                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_TEST_INDEX_DELETE, ApiConsumers.MosConsumer, data, param);
                    if (success)
                    {
                        BackendDataWorker.Reset<V_HIS_TEST_INDEX>();
                        FillDataToGridControl();
                        currentData = ((List<V_HIS_TEST_INDEX>)gridControlFormList.DataSource).FirstOrDefault();
                    }
                    MessageManager.Show(this, param, success);
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
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                SetFocusEditor();
                cboTestServiceType.EditValue = null;
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

                
                MOS.EFMODEL.DataModels.HIS_TEST_INDEX updateDTO = new MOS.EFMODEL.DataModels.HIS_TEST_INDEX();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                //UpdateDTOFromDataForm(ref updateDTO);
                if (!UpdateDTOFromDataForm(ref updateDTO))   
                {
                    return;
                }
                WaitingManager.Show();
                if (cboTestIndexUnit.EditValue != null) updateDTO.TEST_INDEX_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboTestIndexUnit.EditValue ?? "0").ToString());
                else
                    updateDTO.TEST_INDEX_UNIT_ID = null;

                if (lkTestServiceTypeId.EditValue != null) updateDTO.TEST_SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkTestServiceTypeId.EditValue ?? "0").ToString());

                if (cboTestIndexGroupID.EditValue != null) updateDTO.TEST_INDEX_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboTestIndexGroupID.EditValue ?? "0").ToString());
                else
                    updateDTO.TEST_INDEX_GROUP_ID = null;

                if (cboMaterialType.EditValue != null) updateDTO.MATERIAL_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMaterialType.EditValue ?? "0").ToString());
                else
                    updateDTO.MATERIAL_TYPE_ID = null;

                if (spinNormalAmount.EditValue != null)
                    updateDTO.NORMATION_AMOUNT = spinNormalAmount.Value;
                else
                    updateDTO.NORMATION_AMOUNT = null;


                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_TEST_INDEX>(HisRequestUriStore.MOSHIS_TEST_INDEX_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_TEST_INDEX>(HisRequestUriStore.MOSHIS_TEST_INDEX_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        //UpdateRowDataAfterEdit(resultData);
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<V_HIS_TEST_INDEX>();
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX) is null");
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_TEST_INDEX currentDTO)
        {
            try
            {
                currentDTO.DEFAULT_VALUE = txtDefaultResult.Text.Trim();
                currentDTO.TEST_INDEX_CODE = txtTestIndexCode.Text.Trim();
                currentDTO.TEST_INDEX_NAME = txtTestIndexName.Text.Trim();
                currentDTO.BHYT_CODE = txtBHYTCode.Text.Trim();
                currentDTO.BHYT_NAME = txtBHYTName.Text.Trim();
                currentDTO.RESULT_BLOOD_A = txtResultsGroupA.Text.Trim();
                currentDTO.RESULT_BLOOD_B = txtResultsGroupB.Text.Trim();
                currentDTO.RESULT_BLOOD_AB = txtResultsGroupAB.Text.Trim();
                currentDTO.RESULT_BLOOD_O = txtResultsGroupO.Text.Trim();
                currentDTO.RESULT_BLOOD_RH_PLUS = txtResultsRHPlus.Text.Trim();
                currentDTO.RESULT_BLOOD_RH_MINUS = txtResultsRHMinus.Text.Trim();

                // mới thêm
                if (cboIndexType.SelectedIndex == 0)
                {
                    currentDTO.TEST_INDEX_TYPE = 1;
                }
                else if (cboIndexType.SelectedIndex == 1)
                {
                    currentDTO.TEST_INDEX_TYPE = 2;
                }
                else if (cboIndexType.SelectedIndex == 2)
                {
                    currentDTO.TEST_INDEX_TYPE = 3;
                }
                else
                {
                    currentDTO.TEST_INDEX_TYPE = null;
                }

                decimal? ConversionRate = null;
                if (!string.IsNullOrEmpty(txtConversionRate.Text))
                {
                    ConversionRate = Convert.ToDecimal(txtConversionRate.Text.Trim());
                }
                currentDTO.CONVERT_RATIO_TYPE = ConversionRate;
                //đến đây


                decimal? mLCT = null;
                if (!string.IsNullOrEmpty(txtMLCT.Text))
                {
                    if (!ValidMaxlengthTextMLCT(txtMLCT, 20))
                    {
                        return false;
                    }
                    mLCT = Convert.ToDecimal(txtMLCT.Text.Trim());
                }
                currentDTO.CONVERT_RATIO_MLCT = mLCT;

                if (spNumOrder.EditValue != null)
                    currentDTO.NUM_ORDER = (long)spNumOrder.Value;
                else
                    currentDTO.NUM_ORDER = null;

                if (chkNotShowService.Checked)
                {
                    currentDTO.IS_NOT_SHOW_SERVICE = 1;
                }
                else
                {
                    currentDTO.IS_NOT_SHOW_SERVICE = null;
                }

                if (chkCsBatThuong.Checked)
                    currentDTO.IS_IMPORTANT = 1;
                else
                    currentDTO.IS_IMPORTANT = null;

                if (chkIsToCalculateEgfr.Checked)
                {
                    currentDTO.IS_TO_CALCULATE_EGFR = 1;
                }
                else
                {
                    currentDTO.IS_TO_CALCULATE_EGFR = null;
                }
                if (chkHbsAg.Checked)
                {
                    currentDTO.IS_HBSAG = 1;
                }
                else
                {
                    currentDTO.IS_HBSAG = null;
                }
                if (chkHCV.Checked)
                {
                    currentDTO.IS_HCV = 1;
                }
                else
                {
                    currentDTO.IS_HCV = null;
                }
                if (chkHIV.Checked)
                {
                    currentDTO.IS_HIV = 1;
                }
                else
                {
                    currentDTO.IS_HIV = null;
                }
                if (chkABO.Checked)
                {
                    currentDTO.IS_BLOOD_ABO = 1;
                }
                else
                {
                    currentDTO.IS_BLOOD_ABO = null;
                }
                if (chkRH.Checked)
                {
                    currentDTO.IS_BLOOD_RH = 1;
                }
                else
                {
                    currentDTO.IS_BLOOD_RH = null;
                }
                if (chkIsTestHamonyBlood.Checked)
                {
                    currentDTO.IS_TEST_HARMONY_BLOOD = 1;
                }
                else
                {
                    currentDTO.IS_TEST_HARMONY_BLOOD = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return true;
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidateMaxLength(txtTestIndexCode, 20);
                ValidateMaxLength(txtTestIndexName, 300);
                //ValidMaxlengthTextMLCT(txtMLCT, 20);
                //ValidMaxlengthText(txtMLCT, 20);
                ValidateMaxLengthTextEdit(txtResultsGroupA, 500);
                ValidateMaxLengthTextEdit(txtResultsGroupB, 500);
                ValidateMaxLengthTextEdit(txtResultsGroupAB, 500);
                ValidateMaxLengthTextEdit(txtResultsGroupO, 500);
                ValidateMaxLengthTextEdit(txtResultsRHPlus, 500);
                ValidateMaxLengthTextEdit(txtResultsRHMinus, 500);
                ValidateMaxLengthTextEdit(txtDefaultResult, 200);

                ValidationSingleControl(lkTestServiceTypeId);
                //ValidationSingleControl(cboTestIndexUnit);
                ValidationSingleControl1(spNumOrder);
                ValidationMaterialTypeAndNormalAmount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);     
            }
        }


        public string ValidateMaxLengDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            int decimalIndex = input.IndexOf(',');
            if (decimalIndex == -1 || decimalIndex == input.Length - 1)
            {
                return string.Empty;
            }

            return input.Substring(decimalIndex + 1);
        }

        private void ValidateMaxLengthTextEdit(TextEdit textEdit, long maxlength)
        {
            ValidTextEditMaxLenght validate = new ValidTextEditMaxLenght();
            validate.textEdit = textEdit;
            validate.maxlength = maxlength;
            validate.ErrorType = ErrorType.Warning;
            this.dxValidationProviderEditorInfo.SetValidationRule(textEdit, validate);
        }


        private bool ValidMaxlengthTextMLCT(ButtonEdit txtMLCT, int maxDecimalLength)
        {
            try
            {
                string input = txtMLCT.Text;
                string decimalPart = ValidateMaxLengDecimal(input);  // Tách phần thập phân

                if (decimalPart.Length > maxDecimalLength)
                {
                    MessageBox.Show("Phần thập phân chỉ được phép nhập tối đa 20 ký tự.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return true; 
        }

        private void ValidMaxlengthText(ButtonEdit txtMLCT, int maxlength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = txtMLCT;
            validate.maxLength = maxlength;
            validate.ErrorType = ErrorType.Warning;
            this.dxValidationProviderEditorInfo.SetValidationRule(txtMLCT, validate);
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
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
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl1(SpinEdit control)
        {
            try
            {
                ValidateSpin1 validate = new ValidateSpin1();
                validate.spin = control;
                //validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                validate.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaterialTypeAndNormalAmount()
        {
            try
            {
                ValidateMaterialTypeAndNormalAmount validate = new ValidateMaterialTypeAndNormalAmount();
                validate.spin = spinNormalAmount;
                validate.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(spinNormalAmount, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateMaxLength(TextEdit textedit, int length)
        {
            ValidMaxlength validate = new ValidMaxlength();
            validate.textedit = textedit;
            validate.Maxlength = length;
            validate.ErrorType = ErrorType.Warning;
            this.dxValidationProviderEditorInfo.SetValidationRule(textedit, validate);

        }

        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                FillDataToGridControl();

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
        #endregion

        #region Shortcut
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

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
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
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
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
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_TEST_INDEX success = new HIS_TEST_INDEX();
            //bool notHandler = false;
            try
            {

                V_HIS_TEST_INDEX data = (V_HIS_TEST_INDEX)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_TEST_INDEX data1 = new HIS_TEST_INDEX();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TEST_INDEX>(HisRequestUriStore.MOSHIS_TEST_INDEX_CHANGELOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<V_HIS_TEST_INDEX>();
                        FillDataToGridControl();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Lock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_TEST_INDEX success = new HIS_TEST_INDEX();
            //bool notHandler = false;
            try
            {

                V_HIS_TEST_INDEX data = (V_HIS_TEST_INDEX)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_TEST_INDEX data1 = new HIS_TEST_INDEX();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TEST_INDEX>(HisRequestUriStore.MOSHIS_TEST_INDEX_CHANGELOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<V_HIS_TEST_INDEX>();
                        FillDataToGridControl();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    var isActive = (short?)view.GetRowCellValue(e.RowHandle, "IS_ACTIVE");
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLock : Lock);

                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : Delete);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var isActive = (short?)view.GetRowCellValue(e.RowHandle, "IS_ACTIVE");
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtSampleRoomCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestIndexName.Focus();
                    txtTestIndexName.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtSampleRoomName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    lkTestServiceTypeId.Focus();
                    if (lkTestServiceTypeId.EditValue == null)
                    {
                        lkTestServiceTypeId.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spNumOrder.Focus();
                    spNumOrder.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spNumOrder_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBHYTCode.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        public void RefreshData()
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
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)RefreshData);
                if (this.moduleData != null)
                {
                    CallModule callModule = new CallModule(CallModule.HisImportTestIndex, moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisImportTestIndex, 0, 0, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBHYTCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBHYTName.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }


        }

        private void txtBHYTName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTestIndexGroupID.Focus();
                    if (cboTestIndexGroupID.EditValue == null)
                    {
                        cboTestIndexGroupID.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }


        }

        private void chkNotShowService_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.chkCsBatThuong.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCsBatThuong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsTestHamonyBlood.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTestServiceType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void lkTestServiceTypeId_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboTestIndexUnit.Focus();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboTestIndexGroupID_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtMaterialTypeCode.Focus();
                    txtMaterialTypeCode.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsToCalculateEgfr_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMLCT.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTestIndexGroupID_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTestIndexGroupID.Properties.Buttons[1].Visible = true;
                    cboTestIndexGroupID.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaterialTypeCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtMaterialTypeCode.Text) && this.materialTypeSubs != null && this.materialTypeSubs.Count() > 0)
                    {
                        var materialTypes = this.materialTypeSubs.Where(o => o.MATERIAL_TYPE_CODE.ToLower().Equals(txtMaterialTypeCode.Text.Trim().ToLower())).ToList();
                        if (materialTypes != null && materialTypes.Count == 1)
                        {
                            txtMaterialTypeCode.Text = materialTypes[0].MATERIAL_TYPE_CODE;
                            cboMaterialType.EditValue = materialTypes[0].ID;
                            labelControl1.Text = materialTypes[0].SERVICE_UNIT_NAME + "/Lần";
                            spinNormalAmount.Enabled = true;

                            cboMaterialType.Focus();
                            e.Handled = true;

                        }
                        else
                        {
                            spinNormalAmount.Enabled = false;
                            spinNormalAmount.EditValue = null;
                            cboMaterialType.Focus();
                            cboMaterialType.ShowPopup();
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        spinNormalAmount.Enabled = false;
                        spinNormalAmount.EditValue = null;
                        cboMaterialType.Focus();
                        cboMaterialType.ShowPopup();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMaterialType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMaterialType.EditValue = null;
                    txtMaterialTypeCode.Text = "";
                    spinNormalAmount.EditValue = null;
                    spinNormalAmount.Enabled = false;
                    labelControl1.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMaterialType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinNormalAmount.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNormationAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    //chkNotShowService.Focus();
                    e.Handled = true;
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

        private void cboMaterialType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMaterialType.EditValue != null)
                    {
                        var materialType = this.materialTypeSubs.FirstOrDefault(o => o.ID == Convert.ToInt64(cboMaterialType.EditValue.ToString()));
                        if (materialType != null)
                        {
                            txtMaterialTypeCode.Text = materialType.MATERIAL_TYPE_CODE;
                            labelControl1.Text = materialType.SERVICE_UNIT_NAME + "/Lần";
                            spinNormalAmount.Enabled = true;
                        }
                        else
                        {
                            spinNormalAmount.Enabled = false;
                            spinNormalAmount.EditValue = null;
                        }
                    }
                    else
                    {
                        spinNormalAmount.Enabled = false;
                        spinNormalAmount.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestIndexGroupID_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtMaterialTypeCode.Focus();
                    txtMaterialTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNormationAmount_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkNotShowService.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinNormalAmount_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkNotShowService.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void lkTestServiceTypeId_Closed(object sender, ClosedEventArgs e)
        {

        }

        private void cboMaterialType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinNormalAmount.Enabled)
                    {
                        spinNormalAmount.Focus();
                    }
                    else
                    {
                        chkNotShowService.Focus();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMaterialType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spinNormalAmount.Enabled = cboMaterialType.EditValue != null ? true : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkRH_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsTestHamonyBlood.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHbsAg_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHCV.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHCV_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHIV.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHIV_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsToCalculateEgfr.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkABO_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkRH.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsTestHamonyBlood_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHbsAg.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMLCT_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') && ((sender as TextEdit).Text.Contains('.') || (sender as TextEdit).Text.Contains(',')))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') && (sender as TextEdit).Text.Length == 0)
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                e.KeyChar = ',';
                if ((sender as TextEdit).Text.Contains(","))
                {
                    e.Handled = true;
                }
            }
        }          

        private void txtMLCT_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkABO.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsToCalculateEgfr_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsToCalculateEgfr.Checked)
                {
                    txtMLCT.Enabled = true;
                }
                else
                {
                    txtMLCT.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkABO_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkABO.Checked)
                {
                    txtResultsGroupA.Enabled = true;
                    txtResultsGroupB.Enabled = true;
                    txtResultsGroupAB.Enabled = true;
                    txtResultsGroupO.Enabled = true;
                }
                else
                {
                    txtResultsGroupA.Enabled = false;
                    txtResultsGroupB.Enabled = false;
                    txtResultsGroupAB.Enabled = false;
                    txtResultsGroupO.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkRH_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkRH.Checked)
                {
                    txtResultsRHPlus.Enabled = true;
                    txtResultsRHMinus.Enabled = true;
                }
                else
                {
                    txtResultsRHPlus.Enabled = false;
                    txtResultsRHMinus.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTestIndexUnit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if(e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTestIndexUnit.EditValue = null;
                    cboTestIndexUnit.Text = "";
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIndexType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboIndexType.SelectedIndex == 0 || cboIndexType.SelectedIndex == 1 || cboIndexType.SelectedIndex == 2)
            {
                txtConversionRate.Enabled = true;
            }
            else
            {
                txtConversionRate.Enabled = false;
            }
        }

        private void txtConversionRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') && ((sender as TextEdit).Text.Contains('.') || (sender as TextEdit).Text.Contains(',')))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') && (sender as TextEdit).Text.Length == 0)
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                e.KeyChar = ',';
                if ((sender as TextEdit).Text.Contains(","))
                {
                    e.Handled = true;
                }
            }
        }

        private void cboIndexType_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboIndexType.Text))
            {
                txtConversionRate.Enabled = false;
                txtConversionRate.Text = string.Empty;
            }
        }
        //CONVERT_RATIO_TYPE tỉ lệ quy đổi loại chỉ số chỉ số lưu vô đây
        //TEST_INDEX_TYPE loại chỉ số lưu vào đây
    }
}
