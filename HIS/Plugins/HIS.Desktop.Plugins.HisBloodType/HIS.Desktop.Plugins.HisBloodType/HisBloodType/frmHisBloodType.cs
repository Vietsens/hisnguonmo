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
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisBloodType.Validtion;
using HIS.Desktop.Plugins.HisBloodType;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.HisBloodType
{
    public partial class frmHisBloodType : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS_SERVICE service = new HIS_SERVICE();
        List<MOS.EFMODEL.DataModels.HIS_SERVICE> his_SERVICE;
        #endregion
        HIS_BLOOD_TYPE a = new HIS_BLOOD_TYPE();
        #region Construct
        public frmHisBloodType(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                //gridControlFormList.ToolTipController = toolTipControllerGrid;

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
        private void frmHisBloodType_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBloodType.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBloodType.frmHisBloodType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn6.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn22.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn21.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn13.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn14.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn15.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn18.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn19.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn20.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisBloodType.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCPNG.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.chkCPNG.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboParent.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPacking.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboPacking.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDVT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboDVT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboVolume.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboVolume.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBloodTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciBloodTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBloodTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciBloodTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpPrice.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciImpPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAlertExpiredDate.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciAlertExpiredDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciElement.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciElement.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInternalPrice.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciInternalPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpVatRatio.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciImpVatRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinOrder.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciHeinLimitPriceOld.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitPriceOld.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciHeinLimitRatioOld.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitRatioOld.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciHeinLimitPrice.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciHeinLimitRatio.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciHeinLimitPriceInTime.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitPriceInTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciHeinLimitPriceIntrTime.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitPriceIntrTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkBBHC.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.chkBBHC.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
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
                txtKeyword.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);

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
                dicOrderTabIndexControl.Add("txtBloodTypeCode", 0);
                dicOrderTabIndexControl.Add("txtBloodTypeName", 1);
                dicOrderTabIndexControl.Add("lkServiceId", 2);
                dicOrderTabIndexControl.Add("lkParentId", 3);
                dicOrderTabIndexControl.Add("chkIsLeaf", 4);
                dicOrderTabIndexControl.Add("spNumOrder", 5);
                dicOrderTabIndexControl.Add("spImpPrice", 6);
                dicOrderTabIndexControl.Add("spImpVatRatio", 7);
                dicOrderTabIndexControl.Add("lkPackingTypeId", 8);
                dicOrderTabIndexControl.Add("spInternalPrice", 9);
                dicOrderTabIndexControl.Add("spAlertExpiredDate", 10);
                dicOrderTabIndexControl.Add("spAlertHSD", 11);
                dicOrderTabIndexControl.Add("lkBloodVolumeId", 12);
                dicOrderTabIndexControl.Add("txtElement", 13);


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

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboHeinServiceTypeId();
                InitComboServiceUnit();
                InitComboPackingTypeId();
                InitComboBloodGroupId();
                InitComboBloodVolumeId();
                InitComboParentID();
                InitComboPreparation();
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboHeinServiceTypeId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisHeinServiceTypeFilter filter = new HisHeinServiceTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_HEIN_SERVICE_TYPE>>("api/HisHeinServiceType/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboServiceUnit()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceUnitFilter filter = new HisServiceUnitFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_SERVICE_UNIT>>("api/HisServiceUnit/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDVT, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboPackingTypeId()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PACKING_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PACKING_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PACKING_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPacking, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PACKING_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPreparation()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PREPARATIONS_BLOOD_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PREPARATIONS_BLOOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PREPARATIONS_BLOOD_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPreparation, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PREPARATIONS_BLOOD>().Where(o=>o.IS_ACTIVE == 1).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboBloodGroupId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBloodGroupFilter filter = new HisBloodGroupFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_BLOOD_GROUP>>("api/HisBloodGroup/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BLOOD_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboGroup, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboBloodVolumeId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBloodVolumeFilter filter = new HisBloodVolumeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_BLOOD_VOLUME>>("api/HisBloodVolume/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("VOLUME", "", 350, 1));
                //columnInfos.Add(new ColumnInfo("BLOOD_VOLUME_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("VOLUME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboVolume, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboParentID()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBloodTypeFilter filter = new HisBloodTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_BLOOD_TYPE>>(HisRequestUriStore.MOSHIS_BLOOD_TYPE_GET, ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BLOOD_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboParent, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void SetFilterNavBar(ref HisBloodTypeViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    currentData = data;
                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                    SetEnableNeed();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    txtBloodTypeCode.Text = data.BLOOD_TYPE_CODE;
                    txtBloodTypeName.Text = data.BLOOD_TYPE_NAME;
                    cboPreparation.EditValue = data.PREPARATIONS_BLOOD_ID;
                    //lkServiceId.EditValue = data.SERVICE_ID;
                    //lkParentId.EditValue = data.PARENT_ID;
                    //chkIsLeaf.Checked = (data.IS_LEAF == 1 ? true : false);
                    spNumOrder.EditValue = data.NUM_ORDER;
                    spImpPrice.EditValue = data.IMP_PRICE;
                    spImpVatRatio.EditValue = data.IMP_VAT_RATIO * 100;
                    cboPacking.EditValue = data.PACKING_TYPE_ID;
                    spInternalPrice.EditValue = data.INTERNAL_PRICE;
                    spAlertExpiredDate.EditValue = data.ALERT_EXPIRED_DATE;
                    spAlertHSD.EditValue = data.WARNING_DAY;
                    cboGroup.EditValue = data.BLOOD_GROUP_ID;
                    cboVolume.EditValue = data.BLOOD_VOLUME_ID;
                    cboDVT.EditValue = data.SERVICE_UNIT_ID;
                    txtServiceBHYTName.Text = data.HEIN_SERVICE_BHYT_NAME;
                    txtServiceBHYTCode.Text = data.HEIN_SERVICE_BHYT_CODE;
                    cboType.EditValue = data.HEIN_SERVICE_TYPE_ID;
                    txtElement.Text = data.ELEMENT;
                    cboParent.EditValue = data.PARENT_ID;
                    chkCPNG.Checked = data.IS_OUT_PARENT_FEE == 1 ? true : false;
                    chkIsRedBloodCells.Checked = data.IS_RED_BLOOD_CELLS == 1;
                    // get HIS_SERVICE
                    MOS.Filter.HisServiceViewFilter serviceViewFilter = new HisServiceViewFilter();
                    serviceViewFilter.ID = data.SERVICE_ID;
                    var service = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumer.ApiConsumers.MosConsumer, serviceViewFilter, new CommonParam()).FirstOrDefault();
                    if (service != null)
                    {
                        txtHeinLimitPriceOld.Text = service.HEIN_LIMIT_PRICE_OLD != null ? string.Format("{0:0.####}", (service.HEIN_LIMIT_PRICE_OLD ?? 0)) : "";
                        txtHeinLimitPrice.Text = service.HEIN_LIMIT_PRICE != null ? string.Format("{0:0.####}", (service.HEIN_LIMIT_PRICE ?? 0)) : "";
                        txtHeinLimitRatioOld.Text = service.HEIN_LIMIT_RATIO_OLD != null ? string.Format("{0:0.####}", (service.HEIN_LIMIT_RATIO_OLD ?? 0) * 100) : "";
                        txtHeinLimitRatio.Text = service.HEIN_LIMIT_RATIO != null ? string.Format("{0:0.####}", (service.HEIN_LIMIT_RATIO ?? 0) * 100) : "";

                        chkBBHC.Checked = service.MUST_BE_CONSULTED==1?true:false;
                        dtHeinLimitPriceInTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(service.HEIN_LIMIT_PRICE_IN_TIME ?? 0);
                        dtHeinLimitPriceIntrTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(service.HEIN_LIMIT_PRICE_INTR_TIME ?? 0);
                        txtHeinOrder.Text = service.HEIN_ORDER;
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

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtBloodTypeCode.Focus();
                            txtBloodTypeCode.SelectAll();
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBloodTypeFilter filter = new HisBloodTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>>(HisRequestUriStore.MOSHIS_BLOOD_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                LoaddataToTreeList(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ClearErrorWaring()
        {
            try
            {
                dxValidationProviderEditorInfo.RemoveControlError(txtBloodTypeCode);
                dxValidationProviderEditorInfo.RemoveControlError(txtBloodTypeName);
                dxValidationProviderEditorInfo.RemoveControlError(txtHeinLimitPriceOld);
                dxValidationProviderEditorInfo.RemoveControlError(txtHeinLimitRatioOld);
                dxValidationProviderEditorInfo.RemoveControlError(txtHeinLimitPrice);
                dxValidationProviderEditorInfo.RemoveControlError(txtHeinLimitRatio);
                dxValidationProviderEditorInfo.RemoveControlError(dtHeinLimitPriceInTime);
                dxValidationProviderEditorInfo.RemoveControlError(dtHeinLimitPriceIntrTime);
                dxValidationProviderEditorInfo.RemoveControlError(cboVolume);
                dxValidationProviderEditorInfo.RemoveControlError(cboDVT);
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
                ClearErrorWaring();
                //LoaddataToTreeList(this);
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
                ClearErrorWaring();
                SetEnableNeed();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableNeed()
        {
            try
            {
                txtHeinLimitRatio.Enabled = true;
                txtHeinLimitRatioOld.Enabled = true;
                txtHeinLimitPrice.Enabled = true;
                txtHeinLimitPriceOld.Enabled = true;
                dtHeinLimitPriceIntrTime.Enabled = true;
                dtHeinLimitPriceInTime.Enabled = true;
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

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE updateDTO = new MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE();
                MOS.EFMODEL.DataModels.HIS_SERVICE service = new MOS.EFMODEL.DataModels.HIS_SERVICE();
                if (cboType.EditValue != null) service.HEIN_SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboType.EditValue ?? "0").ToString());
                service.HEIN_SERVICE_BHYT_CODE = txtServiceBHYTCode.Text;
                service.HEIN_SERVICE_BHYT_NAME = txtServiceBHYTName.Text;
                service.HEIN_ORDER = txtHeinOrder.Text;
                if (!string.IsNullOrEmpty(txtHeinLimitPrice.Text))
                {
                    service.HEIN_LIMIT_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(txtHeinLimitPrice.Text);
                }
                if (!string.IsNullOrEmpty(txtHeinLimitPriceOld.Text))
                {
                    service.HEIN_LIMIT_PRICE_OLD = Inventec.Common.TypeConvert.Parse.ToDecimal(txtHeinLimitPriceOld.Text);
                }
                if (!string.IsNullOrEmpty(txtHeinLimitRatio.Text))
                {
                    service.HEIN_LIMIT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal(txtHeinLimitRatio.Text) / 100;
                }
                if (!string.IsNullOrEmpty(txtHeinLimitRatioOld.Text))
                {
                    service.HEIN_LIMIT_RATIO_OLD = Inventec.Common.TypeConvert.Parse.ToDecimal(txtHeinLimitRatioOld.Text) / 100;
                }

                if (dtHeinLimitPriceInTime.EditValue != null && dtHeinLimitPriceInTime.DateTime != DateTime.MinValue)
                    service.HEIN_LIMIT_PRICE_IN_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtHeinLimitPriceInTime.EditValue).ToString("yyyyMMddHHmm") + "00");

                if (dtHeinLimitPriceIntrTime.EditValue != null && dtHeinLimitPriceIntrTime.DateTime != DateTime.MinValue)
                    service.HEIN_LIMIT_PRICE_INTR_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                       Convert.ToDateTime(dtHeinLimitPriceIntrTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (cboDVT.EditValue != null) service.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDVT.EditValue ?? "0").ToString());
                service.IS_OUT_PARENT_FEE = (short)(chkCPNG.Checked == true ? 1 : 0);
                service.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU;
                service.MUST_BE_CONSULTED = (short)(chkBBHC.Checked == true ? 1 : 0);
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    if (this.ActionType == GlobalVariables.ActionEdit)
                    {
                        LoadCurrent(this.currentData.ID, ref updateDTO);
                    }
                }

                UpdateDTOFromDataForm(ref updateDTO);
                updateDTO.HIS_SERVICE = service;
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>(HisRequestUriStore.MOSHIS_BLOOD_TYPE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        LoaddataToTreeList(this);
                        ResetFormData();
                    }
                }
                else
                {
                    updateDTO.HIS_SERVICE.ID = currentData.SERVICE_ID;
                    updateDTO.HIS_SERVICE.SERVICE_TYPE_ID = currentData.SERVICE_TYPE_ID;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>(HisRequestUriStore.MOSHIS_BLOOD_TYPE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        LoaddataToTreeList(this);
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<HIS_BLOOD_TYPE>();
                    BackendDataWorker.Reset<V_HIS_BLOOD_TYPE>();
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

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE currentDTO)
        {
            try
            {
                currentDTO.BLOOD_TYPE_CODE = txtBloodTypeCode.Text.Trim();
                currentDTO.BLOOD_TYPE_NAME = txtBloodTypeName.Text.Trim();
                if (spNumOrder.EditValue != null)
                {
                    currentDTO.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64((spNumOrder.EditValue ?? "0").ToString());
                }
                else
                {
                    currentDTO.NUM_ORDER = null;
                }
                if (spImpPrice.EditValue != null)
                {
                    currentDTO.IMP_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal((spImpPrice.EditValue ?? "0").ToString());
                }
                else
                {
                    currentDTO.IMP_PRICE = null;
                }
                if (spImpVatRatio.EditValue != null)
                {
                    currentDTO.IMP_VAT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal((spImpVatRatio.EditValue ?? "0").ToString()) / 100;
                }
                else
                {
                    currentDTO.IMP_VAT_RATIO = null;
                }

                if (cboPacking.EditValue != null) currentDTO.PACKING_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPacking.EditValue ?? "0").ToString());
                if (spInternalPrice.EditValue != null)
                {
                    currentDTO.INTERNAL_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal((spInternalPrice.EditValue ?? "0").ToString());
                }
                else
                {
                    currentDTO.INTERNAL_PRICE = null;
                }
                if (spAlertExpiredDate.EditValue != null)
                {
                    currentDTO.ALERT_EXPIRED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64((spAlertExpiredDate.EditValue ?? "0").ToString());
                }
                else
                {
                    currentDTO.ALERT_EXPIRED_DATE = null;
                }
                if (spAlertHSD.EditValue != null)
                {
                    currentDTO.WARNING_DAY = Inventec.Common.TypeConvert.Parse.ToInt64((spAlertHSD.EditValue ?? "0").ToString());
                }
                else
                {
                    currentDTO.WARNING_DAY = null;
                }
                if (cboGroup.EditValue != null) currentDTO.BLOOD_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboGroup.EditValue ?? "0").ToString());
                else
                    currentDTO.BLOOD_GROUP_ID = null;
                if (cboVolume.EditValue != null) currentDTO.BLOOD_VOLUME_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboVolume.EditValue ?? "0").ToString());
                else
                    currentDTO.BLOOD_VOLUME_ID = 0;
                if (cboPreparation.EditValue != null) currentDTO.PREPARATIONS_BLOOD_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPreparation.EditValue ?? "0").ToString());
                else
                    currentDTO.PREPARATIONS_BLOOD_ID = null;
                currentDTO.ELEMENT = txtElement.Text.Trim();
                if (cboParent.EditValue != null) currentDTO.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboParent.EditValue ?? "0").ToString());
                else currentDTO.PARENT_ID = null;
                if (chkIsRedBloodCells.Checked)
                {
                    currentDTO.IS_RED_BLOOD_CELLS = 1;
                }
                else
                {
                    currentDTO.IS_RED_BLOOD_CELLS = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtBloodTypeCode);
                ValidationSingleControl(txtBloodTypeName);

                ValidationSingleControl(cboDVT);
                ValidationSingleControl(cboVolume);
                ValidationSingleControl1(spNumOrder);
                ValidationSingleControl1(spImpPrice);
                ValidationSingleControl1(spImpVatRatio);
                //ValidationSingleControl(lkPackingTypeId);
                ValidationSingleControl1(spInternalPrice);
                ValidationSingleControl1(spAlertExpiredDate);
                ValidationSingleControl1(spAlertHSD);
                //ValidationSingleControl(lkBloodGroupId);
                //ValidationSingleControl(lkBloodVolumeId);
                //ValidationSingleControl(txtElement);
                ValidateHeinServiceTypeBhyt();
                ValidateHeinLimitPriceDateTime();
                ValidateHeinLimit();
                ValidateHeinLimitRatioOld();
                ValidateHeinLimitRatio();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
        void ValidateHeinServiceTypeBhyt()
        {
            try
            {
                HeinServiceTypeBhytValidationRule heinServiceTypeBhytValidationRule = new HeinServiceTypeBhytValidationRule();
                heinServiceTypeBhytValidationRule.txtHeinServiceTypeBhytCode = this.txtServiceBHYTCode;
                heinServiceTypeBhytValidationRule.txtHeinServiceTypeBhytName = this.txtServiceBHYTName;
                heinServiceTypeBhytValidationRule.ErrorText = "Nêu đã nhập mã bhyt thì cần nhập tên bhyt";
                heinServiceTypeBhytValidationRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(this.txtServiceBHYTCode, heinServiceTypeBhytValidationRule);
                this.dxValidationProviderEditorInfo.SetValidationRule(this.txtServiceBHYTName, heinServiceTypeBhytValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidateHeinLimitPriceDateTime()
        {
            try
            {
                HeinLimitPriceDateTimeValidationRule heinLimitPriceDateTimeValidationRule = new HeinLimitPriceDateTimeValidationRule();
                heinLimitPriceDateTimeValidationRule.dtHeinLimitPriceInTime = this.dtHeinLimitPriceInTime;
                heinLimitPriceDateTimeValidationRule.dtHeinLimitPriceIntrTime = this.dtHeinLimitPriceIntrTime;
                heinLimitPriceDateTimeValidationRule.ErrorText = "Chỉ nhập thời gian theo ngày vào viện hoặc thời gian chỉ định";
                heinLimitPriceDateTimeValidationRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(this.dtHeinLimitPriceInTime, heinLimitPriceDateTimeValidationRule);
                this.dxValidationProviderEditorInfo.SetValidationRule(this.dtHeinLimitPriceIntrTime, heinLimitPriceDateTimeValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidateHeinLimit()
        {
            try
            {
                HeinLimitValidationRule heinLimitValidationRule = new HeinLimitValidationRule();
                heinLimitValidationRule.txtHeinLimitPrice = this.txtHeinLimitPrice;
                heinLimitValidationRule.txtHeinLimitRatio = this.txtHeinLimitRatio;
                heinLimitValidationRule.txtHeinLimitPriceOld = this.txtHeinLimitPriceOld;
                heinLimitValidationRule.txtHeinLimitRatioOld = this.txtHeinLimitRatioOld;
                heinLimitValidationRule.ErrorText = "Chỉ nhập giá hoặc tỉ lệ trần";
                heinLimitValidationRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(this.txtHeinLimitPrice, heinLimitValidationRule);
                this.dxValidationProviderEditorInfo.SetValidationRule(this.txtHeinLimitPriceOld, heinLimitValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidateHeinLimitRatio()
        {
            try
            {
                HeinLimitRatioValidationRule heinLimitRatioValidationRule = new HeinLimitRatioValidationRule();
                heinLimitRatioValidationRule.txtHeinLimitRatio = this.txtHeinLimitRatio;
                heinLimitRatioValidationRule.ErrorText = "Giá trị trong khoảng 0 - 100";
                heinLimitRatioValidationRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(this.txtHeinLimitRatio, heinLimitRatioValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidateHeinLimitRatioOld()
        {
            try
            {
                HeinLimitRatioValidationRule heinLimitRatioOldValidationRule = new HeinLimitRatioValidationRule();
                heinLimitRatioOldValidationRule.txtHeinLimitRatio = this.txtHeinLimitRatioOld;
                heinLimitRatioOldValidationRule.ErrorText = "Giá trị trong khoảng 0 - 100";
                heinLimitRatioOldValidationRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(this.txtHeinLimitRatioOld, heinLimitRatioOldValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

                //Set enable control
                SetEnableNeed();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                LoaddataToTreeList(this);

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

        private void spImpVatRatio_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtBloodTypeCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBloodTypeName.Focus();
                    txtBloodTypeName.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtBloodTypeName_KeyUp(object sender, KeyEventArgs e)
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
                    spImpPrice.Focus();
                    spImpPrice.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spImpPrice_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spImpVatRatio.Focus();
                    spImpVatRatio.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spImpVatRatio_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spInternalPrice.Focus();
                    spInternalPrice.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spInternalPrice_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinLimitPriceOld.Focus();
                    txtHeinLimitPriceOld.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spAlertExpiredDate_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spAlertHSD.Focus();
                    spAlertHSD.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtElement_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboVolume.Focus();
                    if (cboVolume.EditValue == null)
                    {
                        cboVolume.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboVolume_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboParent.Focus();
                    if (cboParent.EditValue == null)
                    {
                        cboParent.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboParent_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGroup.Focus();
                    if (cboGroup.EditValue == null)
                    {
                        cboGroup.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboGroup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDVT.Focus();
                    if (cboDVT.EditValue == null)
                    {
                        cboDVT.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboDVT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPacking.Focus();
                    if (cboPacking.EditValue == null)
                    {
                        cboPacking.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboPacking_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboType.Focus();
                    if (cboType.EditValue == null)
                    {
                        cboType.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtServiceBHYTCode.Focus();
                    txtServiceBHYTCode.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                //bool result = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                   Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
                   Resources.ResourceMessage.HeThongTBCuaSoThongBao,
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    V_HIS_BLOOD_TYPE rowData = data as V_HIS_BLOOD_TYPE;
                    HisBloodTypeFilter filter = new HisBloodTypeFilter();
                    filter.ID = rowData.ID;
                    var pdata = new BackendAdapter(param).Get<List<HIS_BLOOD_TYPE>>(HisRequestUriStore.MOSHIS_BLOOD_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_BLOOD_TYPE_DELETE, ApiConsumers.MosConsumer, pdata.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<V_HIS_BLOOD_TYPE>();
                            BackendDataWorker.Reset<HIS_BLOOD_TYPE>();
                            LoaddataToTreeList(this);
                            currentData = ((List<V_HIS_BLOOD_TYPE>)treeList1.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool result = false;
            HIS_BLOOD_TYPE success = new HIS_BLOOD_TYPE();
            //bool notHandler = false;
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_BLOOD_TYPE rowData = data as V_HIS_BLOOD_TYPE;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HisBloodTypeFilter filter = new HisBloodTypeFilter();
                    filter.ID = rowData.ID;
                    var islock = new BackendAdapter(param).Get<List<HIS_BLOOD_TYPE>>(HisRequestUriStore.MOSHIS_BLOOD_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BLOOD_TYPE>(HisRequestUriStore.MOSHIS_BLOOD_TYPE_CHANGE_LOCK, ApiConsumers.MosConsumer, islock.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<V_HIS_BLOOD_TYPE>();
                        BackendDataWorker.Reset<HIS_BLOOD_TYPE>();
                        result = true;
                        LoaddataToTreeList(this);
                    }
                    MessageManager.Show(this, param, result);
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
            bool result = false;
            HIS_BLOOD_TYPE success = new HIS_BLOOD_TYPE();
            //bool notHandler = false;
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_BLOOD_TYPE rowData = data as V_HIS_BLOOD_TYPE;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HisBloodTypeFilter filter = new HisBloodTypeFilter();
                    filter.ID = rowData.ID;
                    var islock = new BackendAdapter(param).Get<List<HIS_BLOOD_TYPE>>(HisRequestUriStore.MOSHIS_BLOOD_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BLOOD_TYPE>(HisRequestUriStore.MOSHIS_BLOOD_TYPE_CHANGE_LOCK, ApiConsumers.MosConsumer, islock.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<V_HIS_BLOOD_TYPE>();
                        BackendDataWorker.Reset<HIS_BLOOD_TYPE>();
                        result = true;
                        LoaddataToTreeList(this);
                    }
                    MessageManager.Show(this, param, result);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_Click(object sender, EventArgs e)
        {
            try
            {
                InitComboParentID();
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_BLOOD_TYPE rowData = data as V_HIS_BLOOD_TYPE;
                if (rowData != null)
                {
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeList1_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(e.Node);
                if (data != null && data is V_HIS_BLOOD_TYPE)
                {
                    V_HIS_BLOOD_TYPE rowData = data as V_HIS_BLOOD_TYPE;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLockT : LockT);

                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = DeleteE;
                            else
                                e.RepositoryItem = DeleteD;
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    V_HIS_BLOOD_TYPE pData = (V_HIS_BLOOD_TYPE)e.Row;
                    //var pData = data as V_HIS_PAAN_SERVICE_TYPE;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (pData == null || this.treeList1 == null) return;

                    if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.CREATE_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.Value = Inventec.Common.Resource.Get.Value("frmHisBloodType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                            else
                                e.Value = Inventec.Common.Resource.Get.Value("frmHisBloodType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.MODIFY_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua MODIFY_TIME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IMP_VAT_RATIO_STR")
                    {
                        try
                        {
                            e.Value = pData.IMP_VAT_RATIO * 100;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua MODIFY_TIME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "CPNG")
                    {
                        try
                        {
                            e.Value = pData.IS_OUT_PARENT_FEE == 1 ? true : false;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot 'CPNG'", ex);
                        }
                    }
                    else if (e.Column.FieldName == "LaKhoiHongCau")
                    {
                        try
                        {
                            e.Value = pData.IS_RED_BLOOD_CELLS == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot 'LaKhoiHongCau'", ex);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoaddataToTreeList(frmHisBloodType control)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodTypeViewFilter filter = new MOS.Filter.HisBloodTypeViewFilter();

                filter.KEY_WORD = txtKeyword.Text;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_BLOOD_TYPE>>(HisRequestUriStore.MOSHIS_BLOOD_TYPE_GETVIEW, ApiConsumers.MosConsumer, filter, param).ToList();
                if (currentDataStore != null)
                {
                    treeList1.KeyFieldName = "ID";
                    treeList1.ParentFieldName = "PARENT_ID";

                    treeList1.DataSource = currentDataStore;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(e.Node);
                if (data != null && data is V_HIS_BLOOD_TYPE)
                {
                    V_HIS_BLOOD_TYPE rowData = data as V_HIS_BLOOD_TYPE;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
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

        private void treeList1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    InitComboParentID();
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    V_HIS_BLOOD_TYPE rowData = data as V_HIS_BLOOD_TYPE;
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

        private void chkCPNG_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsRedBloodCells.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceBHYTName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinOrder.Focus();
                    txtHeinOrder.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtServiceBHYTCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtServiceBHYTName.Focus();
                    txtServiceBHYTName.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtHeinOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCPNG.Focus();
                    chkCPNG.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitPriceOld_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtHeinLimitRatioOld_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinLimitPrice.Focus();
                    txtHeinLimitPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinLimitRatio.Focus();
                    txtHeinLimitRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtHeinLimitPriceInTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinLimitPriceInTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void dtHeinLimitPriceInTime_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cboVolume_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboVolume.Properties.Buttons[1].Visible = false;
                    cboVolume.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboParent.Properties.Buttons[1].Visible = false;
                    cboParent.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboGroup.Properties.Buttons[1].Visible = false;
                    cboGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDVT_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDVT.Properties.Buttons[1].Visible = false;
                    cboDVT.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPacking_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPacking.Properties.Buttons[1].Visible = false;
                    cboPacking.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboType.Properties.Buttons[1].Visible = false;
                    cboType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboVolume_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboVolume.EditValue != null)
                    {
                        cboVolume.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboParent.EditValue != null)
                    {
                        cboParent.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGroup_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboGroup.EditValue != null)
                    {
                        cboGroup.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDVT_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDVT.EditValue != null)
                    {
                        cboDVT.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPacking_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPacking.EditValue != null)
                    {
                        cboPacking.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboType.EditValue != null)
                    {
                        cboType.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setEnableSpinHein1(bool set, TextEdit sp, ChangingEventArgs e)
        {
            try
            {
                if (e.NewValue != null && !string.IsNullOrEmpty(e.NewValue.ToString()))
                {
                    txtHeinLimitPriceOld.EditValue = null;
                    txtHeinLimitPrice.EditValue = null;
                    txtHeinLimitPriceOld.Enabled = set;
                    txtHeinLimitPrice.Enabled = set;
                }
                else
                {
                    txtHeinLimitPrice.Enabled = !set;
                    txtHeinLimitPriceOld.Enabled = !set;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void setEnableSpinRatio1(bool set, TextEdit sp, ChangingEventArgs e)
        {
            try
            {
                if (e.NewValue != null && !string.IsNullOrEmpty(e.NewValue.ToString()))
                {
                    txtHeinLimitRatioOld.EditValue = null;
                    txtHeinLimitRatio.EditValue = null;
                    txtHeinLimitRatioOld.Enabled = set;
                    txtHeinLimitRatio.Enabled = set;
                }
                else
                {
                    txtHeinLimitRatio.Enabled = !set;
                    txtHeinLimitRatioOld.Enabled = !set;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void setEnableSpinHein(bool set)
        {
            try
            {
                txtHeinLimitPriceOld.Enabled = set;
                txtHeinLimitPrice.Enabled = set;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void setEnableSpinRatio(bool set)
        {
            try
            {
                txtHeinLimitRatioOld.Enabled = set;
                txtHeinLimitRatio.Enabled = set;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void setEnableDtIntr(bool set, ChangingEventArgs e)
        {
            try
            {
                if (e.NewValue != null && !string.IsNullOrEmpty(e.NewValue.ToString()))
                {
                    dtHeinLimitPriceIntrTime.EditValue = null;
                    dtHeinLimitPriceIntrTime.Enabled = set;
                }
                else
                {
                    dtHeinLimitPriceIntrTime.Enabled = !set;
                    dtHeinLimitPriceIntrTime.Enabled = !set;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void setEnableDtIn(bool set, ChangingEventArgs e)
        {
            try
            {
                if (e.NewValue != null && !string.IsNullOrEmpty(e.NewValue.ToString()))
                {
                    dtHeinLimitPriceInTime.EditValue = null;
                    dtHeinLimitPriceInTime.Enabled = set;
                }
                else
                {
                    dtHeinLimitPriceInTime.Enabled = !set;
                    dtHeinLimitPriceInTime.Enabled = !set;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitPriceOld_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {
                setEnableSpinRatio1(false, txtHeinLimitPriceOld, e);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitPrice_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {

                setEnableSpinRatio1(false, txtHeinLimitPrice, e);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitRatioOld_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {
                setEnableSpinHein1(false, txtHeinLimitRatioOld, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitRatio_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {

                setEnableSpinHein1(false, txtHeinLimitRatio, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinLimitPriceInTime_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {
                setEnableDtIntr(false, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinLimitPriceIntrTime_EditValueChanging(object sender, ChangingEventArgs e)
        {

        }

        private void txtHeinLimitPriceOld_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtHeinLimitRatioOld.Enabled)
                    {
                        txtHeinLimitRatioOld.Focus();
                        txtHeinLimitRatioOld.SelectAll();
                    }
                    else
                    {
                        txtHeinLimitPrice.Focus();
                        txtHeinLimitPrice.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitRatioOld_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtHeinLimitPrice.Enabled)
                    {
                        txtHeinLimitPrice.Focus();
                        txtHeinLimitPrice.SelectAll();
                    }
                    else
                    {
                        txtHeinLimitRatio.Focus();
                        txtHeinLimitRatio.SelectAll();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitPrice_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtHeinLimitRatio.Enabled)
                    {
                        txtHeinLimitRatio.Focus();
                        txtHeinLimitRatio.SelectAll();
                    }
                    else
                    {
                        dtHeinLimitPriceInTime.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLimitRatio_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtHeinLimitPriceInTime.Enabled)
                    {
                        dtHeinLimitPriceInTime.Focus();
                    }
                    else
                    {
                        dtHeinLimitPriceIntrTime.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinLimitPriceIntrTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtHeinLimitPriceIntrTime.EditValue != null)
                    {
                        spAlertExpiredDate.Focus();
                        spAlertExpiredDate.SelectAll();
                    }
                    else
                    {
                        dtHeinLimitPriceIntrTime.Focus();
                        dtHeinLimitPriceIntrTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtHeinLimitPriceInTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtHeinLimitPriceInTime.EditValue != null)
                    {
                        if (dtHeinLimitPriceIntrTime.Enabled)
                        {
                            dtHeinLimitPriceIntrTime.Focus();
                        }
                        else
                        {
                            spAlertExpiredDate.Focus();
                            spAlertExpiredDate.SelectAll();
                        }
                    }
                    else
                    {
                        dtHeinLimitPriceInTime.Focus();
                        dtHeinLimitPriceInTime.ShowPopup();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisImportBloodType", this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId, listArgs);
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsRedBloodCells_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBBHC.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkBBHC_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                        btnEdit.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
            
        }

        private void spAlertHSD_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtElement.Focus();
                    txtElement.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboPreparation_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboPreparation.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
