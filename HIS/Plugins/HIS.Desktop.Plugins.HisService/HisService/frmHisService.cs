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
using MOS.SDO;
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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utility;
namespace HIS.Desktop.Plugins.HisService
{
    public partial class frmHisService : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region Construct
        public frmHisService(Inventec.Desktop.Common.Modules.Module moduleData)
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
        private void frmHisService_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisService.Resources.Lang", typeof(HIS.Desktop.Plugins.HisService.frmHisService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisService.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn22.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn28.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn28.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn28.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn18.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn19.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn20.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn20.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn20.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn21.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn27.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn27.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn27.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn26.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn26.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn26.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn25.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn25.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn25.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn24.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn24.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn24.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn23.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn23.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn23.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn17.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn13.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn14.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisService.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisService.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisService.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisService.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisService.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisService.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisService.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisService.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcdCm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboIcdCm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkCPNgoaiGoi.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisService.chkCPNgoaiGoi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMethod.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboMethod.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboParent.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBillOption.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboBillOption.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDVT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboDVT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisService.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisService.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisService.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisService.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisService.lciTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisService.lciTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmHisService.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spCogss.Text = Inventec.Common.Resource.Get.Value("frmHisService.spCogss.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spEstimateDurations.Text = Inventec.Common.Resource.Get.Value("frmHisService.spEstimateDurations.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkUpdateAll.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisService.chkUpdateAll.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkUpdateOnly.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisService.chkUpdateOnly.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmHisService
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisService.Resources.Lang", typeof(frmHisService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSearchType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboSearchType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisService.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisService.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisService.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisService.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisService.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisService.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisService.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn34.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn33.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn32.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn22.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn28.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn28.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn28.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn31.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn31.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn31.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn30.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn30.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn30.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn29.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn29.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn29.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn18.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn19.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn20.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn20.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn20.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn21.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn27.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn27.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn27.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn26.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn26.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn26.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn25.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn25.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn25.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn24.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn24.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn24.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn23.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn23.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn23.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn17.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn13.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.treeListColumn14.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisService.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisService.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPackage.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboPackage.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkChiDinh.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisService.chkChiDinh.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkUpdateOnly.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisService.chkUpdateOnly.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkUpdateAll.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisService.chkUpdateAll.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcdCm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboIcdCm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCPNgoaiGoi.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisService.chkCPNgoaiGoi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMethod.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboMethod.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboParent.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBillOption.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboBillOption.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDVT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisService.cboDVT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisService.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisService.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisService.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisService.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisService.lciTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisService.lciTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmHisService.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spCogss.Text = Inventec.Common.Resource.Get.Value("frmHisService.spCogss.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spEstimateDurations.Text = Inventec.Common.Resource.Get.Value("frmHisService.spEstimateDurations.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem30.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem30.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem23.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("frmHisService.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                txtMisuSerTypeCode.EditValue = null;
                txtMisuSerTypeName.EditValue = null;
                txtMisuSerTypeCode.Focus();
                //txtMisuSerTypeCode.SelectAll();
                txtKeyword.EditValue = null;
                spCogs.EditValue = null;
                spEstimateDuration.EditValue = null;
                spNumOrder.EditValue = null;
                //cboBhyt.EditValue = null;
                cboDVT.EditValue = null;
                cboType.EditValue = null;
                cboParent.EditValue = null;
                cboBillOption.EditValue = null;
                cboBillOption.EditValue = null;
                cboPatientType.EditValue = null;
                cboGroup.EditValue = null;
                cboIcdCm.EditValue = null;
                cboMethod.EditValue = null;
                cboServiceType.EditValue = null;
                txtBHYTCode.EditValue = null;
                txtBHYTName.EditValue = null;
                txtBHYTStt.EditValue = null;
                spHeinNew.EditValue = null;
                spHeinOld.EditValue = null;
                spRatioNew.EditValue = null;
                spRatioOld.EditValue = null;
                dtInTime.EditValue = null;
                dtIntrTime.EditValue = null;
                chkCPNgoaiGoi.EditValue = null;
                chkChiDinh.EditValue = null;
                chkUpdateAll.EditValue = null;
                chkUpdateOnly.EditValue = null;
                SetEnablePTTT(true);
                txtSpecialityCode.EditValue = null;
                txtSpecialityCode.Enabled = true;
                cboPackage.EditValue = null;
                cboPackage.Enabled = true;
                txtPackagePrice.EditValue = null;
                txtPackagePrice.Enabled = false;
                txtPackagePrice.Validating += null;
                dxErrorProvider.ClearErrors();
                layoutControlItem35.AppearanceItemCaption.ForeColor = Color.Transparent;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                //ResetFormData();
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
                dicOrderTabIndexControl.Add("txtMisuSerTypeCode", 0);
                dicOrderTabIndexControl.Add("txtMisuSerTypeName", 1);
                dicOrderTabIndexControl.Add("lkServiceId", 2);
                dicOrderTabIndexControl.Add("lkParentId", 3);
                dicOrderTabIndexControl.Add("chkIsLeaf", 4);
                dicOrderTabIndexControl.Add("spNumOrder", 5);
                dicOrderTabIndexControl.Add("spImpPrice", 6);
                dicOrderTabIndexControl.Add("spImpVatRatio", 7);
                dicOrderTabIndexControl.Add("lkPackingTypeId", 8);
                dicOrderTabIndexControl.Add("spInternalPrice", 9);
                dicOrderTabIndexControl.Add("spAlertExpiredDate", 10);
                dicOrderTabIndexControl.Add("lkBloodGroupId", 11);
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
                InitComboSearchServiceType();
                InitComboServiceUnit();
                InitComboParentID();
                InitComboIcdCm();
                InitComboServiceType();
                InitComboPtttGroup();
                //InitComboBillOption();
                LoadComboStatus();
                LoadComboPatientType();
                InitComboMethod();
                InitComboPackage();

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

        private void InitComboServiceType()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).OrderByDescending(o => o.MODIFY_TIME).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboServiceType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboSearchServiceType()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboSearchType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitComboIcdCm()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_ICD_CM>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ICD_CM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ICD_CM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_CM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboIcdCm, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPtttGroup()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_PTTT_GROUP>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboGroup, data, controlEditorADO);
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

        private void InitComboMethod()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPtttMethodFilter filter = new HisPtttMethodFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_PTTT_METHOD>>("api/HisPtttMethod/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_METHOD_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMethod, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPackage()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPackageFilter filter = new HisPackageFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_PACKAGE>>("api/HisPackage/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PACKAGE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PACKAGE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PACKAGE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPackage, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBillOption()
        {
            try
            {
                List<BillOption> billOption = new List<BillOption>();
                billOption.Add(new BillOption(1, "Hóa đơn thường"));
                billOption.Add(new BillOption(2, "Hóa đơn dịch vụ"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("billName", "", 235, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("billName", "id", columnInfos, false, 235);
                ControlEditorLoader.Load(cboBillOption, billOption, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboPatientType()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Hóa đơn thường"));
                status.Add(new Status(2, "Tách chênh lệch vào hóa đơn dịch vụ"));
                status.Add(new Status(3, "Hóa đơn dịch vụ"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBillOption, status, controlEditorADO);
                //cboBill.EditValue = status[0].id;
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
                var data = BackendDataWorker.Get<V_HIS_SERVICE>().OrderByDescending(o => o.MODIFY_TIME).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboParent, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        private void SetFilterNavBar(ref HisServiceViewFilter filter)
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_SERVICE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;

                    EnableControlChanged(this.ActionType);

                    dtInTime.Enabled = true;
                    dtIntrTime.Enabled = true;
                    setEnableSpinHein(true);
                    setEnableSpinRatio(true);

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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_SERVICE data)
        {
            try
            {
                if (data != null)
                {
                    txtMisuSerTypeCode.Text = data.SERVICE_CODE;
                    txtMisuSerTypeName.Text = data.SERVICE_NAME;
                    spNumOrder.EditValue = data.NUM_ORDER;
                    spCogs.EditValue = data.COGS;
                    spEstimateDuration.EditValue = data.ESTIMATE_DURATION;
                    cboParent.EditValue = data.PARENT_ID;
                    cboDVT.EditValue = data.SERVICE_UNIT_ID;
                    cboServiceType.EditValue = data.SERVICE_TYPE_ID;
                    txtBHYTCode.Text = data.HEIN_SERVICE_BHYT_CODE;
                    txtBHYTName.Text = data.HEIN_SERVICE_BHYT_NAME;
                    txtBHYTStt.Text = data.HEIN_ORDER;
                    CheckPTTT(data);
                    spHeinNew.EditValue = data.HEIN_LIMIT_PRICE;
                    spHeinOld.EditValue = data.HEIN_LIMIT_PRICE_OLD;
                    spRatioNew.EditValue = data.HEIN_LIMIT_RATIO * 100;
                    spRatioOld.EditValue = data.HEIN_LIMIT_RATIO_OLD * 100;
                    dtInTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.HEIN_LIMIT_PRICE_IN_TIME ?? 0);
                    dtIntrTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.HEIN_LIMIT_PRICE_INTR_TIME ?? 0);
                    //cboBhyt.EditValue = data.HEIN_SERVICE_BHYT_ID;
                    cboType.EditValue = data.HEIN_SERVICE_TYPE_ID;

                    if (data.BILL_OPTION == null)
                    {
                        cboBillOption.EditValue = 1;
                    }
                    else if (data.BILL_OPTION == 1)
                    {
                        cboBillOption.EditValue = 2;
                    }
                    else if (data.BILL_OPTION == 2)
                    {
                        cboBillOption.EditValue = 3;
                    }
                    cboPackage.EditValue = data.PACKAGE_ID;
                    txtPackagePrice.EditValue = data.PACKAGE_PRICE;
                    txtSpecialityCode.EditValue = data.SPECIALITY_CODE;
                    cboPatientType.EditValue = data.BILL_PATIENT_TYPE_ID;
                    chkCPNgoaiGoi.Checked = data.IS_OUT_PARENT_FEE == 1 ? true : false;
                    chkChiDinh.Checked = data.IS_MULTI_REQUEST == 1 ? true : false;
                    //cboBillOption.EditValue = ((data.BILL_OPTION == null) ? 1 : 2);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckPTTT(MOS.EFMODEL.DataModels.V_HIS_SERVICE data)
        {
            try
            {

                if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT || data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                {
                    SetEnablePTTT(true);
                    cboGroup.EditValue = data.PTTT_GROUP_ID;
                    cboIcdCm.EditValue = data.ICD_CM_ID;
                    cboMethod.EditValue = data.PTTT_METHOD_ID;
                }
                else
                {
                    cboGroup.EditValue = null;
                    cboIcdCm.EditValue = null;
                    cboMethod.EditValue = null;
                    SetEnablePTTT(false);
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
                            if (fomatFrm != lci.Control as DevExpress.XtraEditors.CheckEdit)
                            {
                                fomatFrm.Text = "";
                                fomatFrm.EditValue = null;
                            }
                            //fomatFrm.Enabled = true;
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_SERVICE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceFilter filter = new HisServiceFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE>>(HisRequestUriStore.MOSHIS_SERVICE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                chkUpdateAll.Enabled = (action == GlobalVariables.ActionEdit);
                chkUpdateOnly.Enabled = (action == GlobalVariables.ActionEdit);
                cboServiceType.Enabled = (action == GlobalVariables.ActionAdd);
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

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool result = false;
            HIS_SERVICE success = new HIS_SERVICE();
            //bool notHandler = false;
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE>(HisRequestUriStore.MOSHIS_SERVICE_CHANGE_LOCK, ApiConsumers.MosConsumer, rowData.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
                        result = true;
                        LoaddataToTreeList(this);
                        InitComboParentID();
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
            HIS_SERVICE success = new HIS_SERVICE();
            //bool notHandler = false;
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE>(HisRequestUriStore.MOSHIS_SERVICE_CHANGE_LOCK, ApiConsumers.MosConsumer, rowData.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
                        result = true;
                        LoaddataToTreeList(this);
                        InitComboParentID();
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

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                //ResetFormData();
                //cboServiceType.Enabled = true;
                dtIntrTime.Enabled = true;
                dtInTime.Enabled = true;
                cboServiceType.Enabled = true;
                setEnableSpinHein(true);
                setEnableSpinRatio(true);
                //LoaddataToTreeList();
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
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_SERVICE_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<V_HIS_SERVICE>();
                            LoaddataToTreeList(this);
                            currentData = ((List<V_HIS_SERVICE>)treeList1.DataSource).FirstOrDefault();
                            InitComboParentID();
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                btnRefesh_Click(null, null);
                SetFocusEditor();
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

        private bool checkPriceRatio()
        {
            bool check = false;
            try
            {
                if ((spHeinNew.EditValue != null || spHeinOld.EditValue != null) && (spRatioNew.EditValue != null || spRatioOld.EditValue != null))
                {
                    check = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return check = false;
            }
            return check;
        }

        private bool CheckServiceTypeParent()
        {
            bool result = false;
            try
            {
                if (cboServiceType.EditValue != null && cboParent.EditValue != null)
                {
                    var serviceTypeParent = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString() ?? "0"));
                    var serviceParent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboParent.EditValue.ToString() ?? "0"));
                    if (serviceTypeParent != null && serviceParent != null && serviceTypeParent.ID == serviceParent.SERVICE_TYPE_ID)
                    {
                        result = true;
                    }
                }
                if (cboParent.EditValue == null)
                    result = true;
            }
            catch (Exception ex)
            {
                return false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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

                if (cboPackage.EditValue != null && txtPackagePrice.EditValue == null)
                {
                    dxErrorProvider.SetError(txtPackagePrice, "Chưa nhập trường dữ liệu bắt buộc", ErrorType.Warning);
                    //txtPackagePrice.Validating += txtPackagePrice_Validating;
                    return;
                }
                else if (cboPackage.EditValue != null && txtPackagePrice.EditValue != null)
                {
                    dxErrorProvider.ClearErrors();
                }

                if (!CheckServiceTypeParent())
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại. Loại dịch vụ của cha phải giống loại dịch vụ của con", "Thông báo");
                    return;
                }


                if (string.IsNullOrEmpty(txtBHYTCode.Text) && string.IsNullOrEmpty(txtBHYTName.Text))
                {

                }
                else if (string.IsNullOrEmpty(txtBHYTCode.Text) || string.IsNullOrEmpty(txtBHYTName.Text))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Phải nhập đầy đủ mã và tên DV BHYT", "Thông báo");
                    return;
                }

                if (dtInTime.EditValue != null && dtIntrTime.EditValue != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chỉ cho phép nhập thời gian theo ngày vào viện hoặc chỉ định, không cho phép nhập cả 2", "Thông báo");
                    return;
                }

                if (checkPriceRatio())
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chỉ cho phép nhập giá hoặc tỉ lệ, không cho phép nhập cả 2", "Thông báo");
                    return;
                }

                WaitingManager.Show();
                //HisMisuServiceTypeSDO sdo = new HisMisuServiceTypeSDO();
                MOS.EFMODEL.DataModels.HIS_SERVICE updateDTO = new MOS.EFMODEL.DataModels.HIS_SERVICE();



                if (ActionType == GlobalVariables.ActionEdit)
                {
                    if (this.currentData != null && this.currentData.ID > 0)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE>(updateDTO, this.currentData);
                    }
                }

                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<HIS_SERVICE>(HisRequestUriStore.MOSHIS_SERVICE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        LoaddataToTreeList(this);
                        btnRefesh_Click(null, null);
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
                        InitComboParentID();
                    }
                }
                else
                {
                    HisServiceSDO sdo = new HisServiceSDO();
                    sdo.HisService = updateDTO;
                    if (!chkUpdateAll.Checked && !chkUpdateAll.Checked)
                    {
                        sdo.UpdateSereServ = null;
                    }
                    if (chkUpdateAll.Checked)
                    {
                        sdo.UpdateSereServ = true;
                    }
                    else if (!chkUpdateAll.Checked)
                    {
                        sdo.UpdateSereServ = false;
                    }

                    var resultData = new BackendAdapter(param).Post<HIS_SERVICE>(HisRequestUriStore.MOSHIS_SERVICE_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                    if (resultData != null)
                    {

                        success = true;
                        btnRefesh_Click(null, null);
                        LoaddataToTreeList(this);
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
                        InitComboParentID();
                    }
                }

                if (success)
                {
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


        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_SERVICE currentDTO)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtMisuSerTypeCode.Text))
                    currentDTO.SERVICE_CODE = txtMisuSerTypeCode.Text.Trim();
                if (!String.IsNullOrEmpty(txtMisuSerTypeName.Text))
                    currentDTO.SERVICE_NAME = txtMisuSerTypeName.Text.Trim();
                if (spNumOrder.EditValue != null) currentDTO.NUM_ORDER = (long)spNumOrder.Value;
                if (cboParent.EditValue != null)
                {
                    currentDTO.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboParent.EditValue ?? "0").ToString());
                }
                else
                {
                    currentDTO.PARENT_ID = null;
                }

                if (cboMethod.EditValue != null) currentDTO.PTTT_METHOD_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMethod.EditValue ?? "0").ToString());
                else
                    currentDTO.PTTT_METHOD_ID = null;

                if (cboType.EditValue != null) currentDTO.HEIN_SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboType.EditValue ?? "0").ToString());
                else currentDTO.HEIN_SERVICE_TYPE_ID = null;

                if (cboDVT.EditValue != null) currentDTO.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDVT.EditValue ?? "0").ToString());

                if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBillOption.EditValue ?? "0").ToString()) == 1) currentDTO.BILL_OPTION = null;
                else if (cboBillOption.EditValue == null)
                    currentDTO.BILL_OPTION = null;
                else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBillOption.EditValue ?? "0").ToString()) == 2)
                    currentDTO.BILL_OPTION = 1;
                else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBillOption.EditValue ?? "0").ToString()) == 3)
                    currentDTO.BILL_OPTION = 2;

                if (cboPatientType.EditValue != null) currentDTO.BILL_PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? "0").ToString());
                else currentDTO.BILL_PATIENT_TYPE_ID = null;

                if (spCogs.EditValue != null) currentDTO.COGS = (decimal)spCogs.Value;
                if (spEstimateDuration.EditValue != null) currentDTO.ESTIMATE_DURATION = (decimal)spEstimateDuration.Value;
                currentDTO.IS_OUT_PARENT_FEE = (short)(chkCPNgoaiGoi.Checked ? 1 : 0);
                currentDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                if (cboGroup.EditValue != null) currentDTO.PTTT_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboGroup.EditValue ?? "0").ToString());
                else currentDTO.PTTT_GROUP_ID = null;

                if (cboIcdCm.EditValue != null) currentDTO.ICD_CM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboIcdCm.EditValue ?? "0").ToString());
                else currentDTO.ICD_CM_ID = null;

                if (cboServiceType.EditValue != null) currentDTO.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

                currentDTO.HEIN_SERVICE_BHYT_CODE = txtBHYTCode.Text;
                currentDTO.HEIN_SERVICE_BHYT_NAME = txtBHYTName.Text;
                currentDTO.HEIN_ORDER = txtBHYTStt.Text;
                if (spRatioNew.Enabled && spRatioNew.Enabled)
                {
                    if (spRatioNew.EditValue != null) currentDTO.HEIN_LIMIT_RATIO = (decimal)spRatioNew.Value / 100;
                    if (spRatioOld.EditValue != null) currentDTO.HEIN_LIMIT_RATIO_OLD = (decimal)spRatioOld.Value / 100;
                }
                else
                {
                    currentDTO.HEIN_LIMIT_RATIO = null;
                    currentDTO.HEIN_LIMIT_RATIO_OLD = null;
                }

                if (spHeinNew.Enabled && spHeinOld.Enabled)
                {
                    if (spHeinNew.EditValue != null) currentDTO.HEIN_LIMIT_PRICE = (decimal)spHeinNew.Value;
                    if (spHeinOld.EditValue != null) currentDTO.HEIN_LIMIT_PRICE_OLD = (decimal)spHeinOld.Value;
                }
                else
                {
                    currentDTO.HEIN_LIMIT_PRICE = null;
                    currentDTO.HEIN_LIMIT_PRICE_OLD = null;
                }
                if (dtInTime.DateTime != null && dtInTime.DateTime != DateTime.MinValue)
                {
                    currentDTO.HEIN_LIMIT_PRICE_IN_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtInTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else
                {
                    currentDTO.HEIN_LIMIT_PRICE_IN_TIME = null;
                }

                if (dtIntrTime.DateTime != null && dtIntrTime.DateTime != DateTime.MinValue)
                {
                    currentDTO.HEIN_LIMIT_PRICE_INTR_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtIntrTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else
                {
                    currentDTO.HEIN_LIMIT_PRICE_INTR_TIME = null;
                }

                if (txtPackagePrice.Enabled)
                {
                    if (txtPackagePrice.EditValue != null) currentDTO.PACKAGE_PRICE = (decimal)txtPackagePrice.Value;
                }
                else
                {
                    currentDTO.PACKAGE_PRICE = null;
                }

                if (txtSpecialityCode.Enabled)
                {
                    if (txtSpecialityCode.EditValue != null) currentDTO.SPECIALITY_CODE = txtSpecialityCode.Text;
                }

                if (cboPackage.EditValue != null) currentDTO.PACKAGE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPackage.EditValue ?? "0").ToString());

                currentDTO.IS_MULTI_REQUEST = (short)(chkChiDinh.Checked ? 1 : 0);
            }
            catch (Exception ex)
            {
                currentDTO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtMisuSerTypeCode);
                ValidationSingleControl(txtMisuSerTypeName);
                ValidationSingleControl(cboServiceType);
                ValidationSingleControl(cboDVT);
                ValidationSingleControl1(spNumOrder);
                ValidationSingleControl1(spCogs);
                ValidationSingleControl1(spEstimateDuration);
                ValidationSingleControl2(spRatioNew);
                ValidationSingleControl2(spRatioOld);
                ValidationSingleControl1(spHeinNew);
                ValidationSingleControl1(txtPackagePrice);
                ValidationSingleControl1(spHeinOld);
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
                ValidateSpin2 validate = new ValidateSpin2();
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

        private void ValidationSingleControl2(SpinEdit control)
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
                btnRefesh_Click(null, null);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                LoaddataToTreeList(this);

                //Load ngon ngu label control
                //SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
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

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_SERVICE data = (V_HIS_SERVICE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                                e.Appearance.ForeColor = Color.Red;
                            else
                                e.Appearance.ForeColor = Color.Green;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMisuSerTypeName.Focus();
                    txtMisuSerTypeName.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
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

        private void spNumOrder_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkChiDinh.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spEstimateDuration_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spNumOrder.Focus();
                    spNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spCogs_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spEstimateDuration.Focus();
                    spEstimateDuration.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboBhyt_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBillOption.Focus();
                    if (cboBillOption.EditValue == null)
                    {
                        cboBillOption.ShowPopup();
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
                    if (Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString() ?? "0") == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString() ?? "0") == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                    {
                        cboMethod.Focus();
                        if (cboMethod.EditValue == null)
                        {
                            cboMethod.ShowPopup();
                        }
                    }
                    else
                    {

                        spHeinOld.Focus();
                        spHeinOld.SelectAll();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDVT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (ActionType == GlobalVariables.ActionEdit)
                    {
                        if (!txtSpecialityCode.Enabled)
                        {
                            cboType.Focus();
                            if (cboType.EditValue == null)
                            {
                                cboType.ShowPopup();
                            }
                        }
                        else
                        {
                            txtSpecialityCode.Focus();
                            txtSpecialityCode.SelectAll();
                        }
                    }
                    else
                    {
                        cboServiceType.Focus();
                        if (cboServiceType.EditValue == null)
                        {
                            cboServiceType.ShowPopup();
                        }
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
                    txtBHYTCode.Focus();
                    txtBHYTCode.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void treeList1_Click(object sender, EventArgs e)
        {
            try
            {
                //InitComboParentID();
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                if (rowData != null)
                {
                    currentData = rowData;
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
                if (data != null && data is V_HIS_SERVICE)
                {
                    V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
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

                    else if (e.Column.FieldName == "ChinhSachGia")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = btn_ChinhSachGia_Enable;
                            else
                                e.RepositoryItem = btn_ChinhSachGia_Disable;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }


                    else if (e.Column.FieldName == "PhongXuLy")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = btn_PhongXuLy_Enable;
                            else
                                e.RepositoryItem = btn_PhongXuLy_Disable;
                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    else if (e.Column.FieldName == "BaoCao")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = btn_BaoCao_Enable;
                            else
                                e.RepositoryItem = btn_BaoCao_Disable;
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
                    V_HIS_SERVICE pData = (V_HIS_SERVICE)e.Row;
                    //var pData = data as V_HIS_SERVICE;
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

                    else if (e.Column.FieldName == "HEIN_LIMIT_PRICE_IN_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.HEIN_LIMIT_PRICE_IN_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao HEIN_LIMIT_PRICE_IN_TIME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "PACKAGE_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.ID == pData.PACKAGE_ID);
                            if (data != null)
                            {
                                e.Value = data.PACKAGE_NAME;
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao HEIN_LIMIT_PRICE_IN_TIME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "HEIN_LIMIT_PRICE_INTR_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.HEIN_LIMIT_PRICE_INTR_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao HEIN_LIMIT_PRICE_INTR_TIME_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "PTTT_GROUP_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == pData.PTTT_GROUP_ID);
                            if (data != null)
                            {
                                e.Value = data.PTTT_GROUP_NAME;
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua PTTT_GROUP_NAME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "PTTT_METHOD_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == pData.PTTT_METHOD_ID);
                            if (data != null)
                            {
                                e.Value = data.PTTT_METHOD_NAME;
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua PTTT_METHOD_NAME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "ICD_CM_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_ICD_CM>().FirstOrDefault(o => o.ID == pData.ICD_CM_ID);
                            if (data != null)
                            {
                                e.Value = data.ICD_CM_NAME;
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua ICD_CM_NAME", ex);
                        }
                    }

                    else if (e.Column.FieldName == "HEIN_LIMIT_RATIO_STR")
                    {
                        try
                        {
                            e.Value = pData.HEIN_LIMIT_RATIO * 100;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }

                    else if (e.Column.FieldName == "HEIN_LIMIT_RATIO_OLD_STR")
                    {
                        try
                        {
                            e.Value = pData.HEIN_LIMIT_RATIO_OLD * 100;

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
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
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }
                    //else if (e.Column.FieldName == "ACTIVE_ITEM")
                    //{
                    //    try
                    //    {
                    //        if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuServiceType.HoatDong", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //        else
                    //            e.Value = Inventec.Common.Resource.Get.Value("frmHisMisuServiceType.TamKhoa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error(ex);
                    //    }
                    //}
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

                    else if (e.Column.FieldName == "BILL_OPTION_STR")
                    {
                        try
                        {
                            if (pData.BILL_OPTION == null)
                                e.Value = "Hóa đơn thường";
                            else if (pData.BILL_OPTION == 1)
                                e.Value = "Tách chênh lệch vào hóa đơn dịch vụ";
                            else if (pData.BILL_OPTION == 2)
                                e.Value = "Hóa đơn dịch vụ";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BILL_OPTION", ex);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoaddataToTreeList(frmHisService control)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceViewFilter filter = new HisServiceViewFilter();
                //filter.IS_LEAF = true;
                if (cboSearchType.EditValue != null)
                {
                    filter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboSearchType.EditValue.ToString() ?? ""));
                }
                filter.KEY_WORD = txtKeyword.Text.Trim();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (currentDataStore != null && currentDataStore.Count > 0)
                {
                    //filter.IsTree = true;
                    currentDataStore = currentDataStore.Where(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC).ToList();

                    if (currentDataStore != null)
                    {
                        treeList1.KeyFieldName = "ID";
                        treeList1.ParentFieldName = "PARENT_ID";
                        treeList1.DataSource = currentDataStore;
                    }
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
                if (data != null && data is V_HIS_SERVICE)
                {
                    V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "ACTIVE_ITEM")
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
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                    if (rowData != null)
                    {
                        currentData = rowData;
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBillOption_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPatientType.Focus();
                    if (cboPatientType.EditValue == null)
                    {
                        cboPatientType.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPatientType_KeyUp(object sender, KeyEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboParent_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboParent.Properties.Buttons[1].Visible = true;
                    cboParent.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboType.Properties.Buttons[1].Visible = true;
                    cboType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBillOption_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBillOption.Properties.Buttons[1].Visible = true;
                    cboBillOption.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPatientType.Properties.Buttons[1].Visible = true;
                    cboPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_KeyUp(object sender, KeyEventArgs e)
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

        private void cboMethod_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMethod.Properties.Buttons[1].Visible = true;
                    cboMethod.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCPNgoaiGoi_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkUpdateAll.Enabled)
                    {
                        chkUpdateAll.Focus();
                    }
                    else
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
                        if (e.KeyCode == Keys.Space)
                        {
                            chkCPNgoaiGoi.Checked = !chkCPNgoaiGoi.Checked;
                        }
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtBHYTCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBHYTName.Focus();
                    txtBHYTName.SelectAll();
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
                    txtBHYTStt.Focus();
                    txtBHYTStt.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtBHYTStt_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPackage.Focus();
                    if (cboPackage.EditValue == null)
                    {
                        cboPackage.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }


        }

        private void cboGroup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboIcdCm.Focus();
                    if (cboIcdCm.EditValue == null)
                    {
                        cboIcdCm.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcdCm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spHeinOld.Focus();
                    spHeinOld.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spHeinOld_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spHeinNew.Focus();
                    spHeinNew.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spHeinNew_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spRatioNew.Enabled)
                    {
                        spRatioOld.Focus();
                        spRatioOld.SelectAll();
                    }
                    else
                    {
                        dtInTime.Focus();
                        if (dtInTime.EditValue == null)
                        {
                            dtInTime.ShowPopup();
                        }
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spRatioOld_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spRatioNew.Focus();
                    spRatioNew.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spRatioNew_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtInTime.Focus();
                    if (dtInTime.EditValue == null)
                    {
                        dtInTime.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtIntrTime.Enabled)
                    {
                        dtIntrTime.Focus();
                        if (dtIntrTime.EditValue == null)
                        {
                            dtIntrTime.ShowPopup();
                        }
                    }
                    else
                    {
                        spCogs.Focus();
                        spCogs.SelectAll();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntrTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spCogs.Focus();
                    spCogs.SelectAll();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboServiceType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtSpecialityCode.Enabled)
                    {
                        txtSpecialityCode.Focus();
                        txtSpecialityCode.SelectAll();
                    }
                    else
                    {
                        cboType.Focus();
                        if (cboType.EditValue == null)
                        {
                            cboType.ShowPopup();
                        }
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboGroup_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboGroup.Properties.Buttons[1].Visible = true;
                    cboGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboIcdCm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboIcdCm.Properties.Buttons[1].Visible = true;
                    cboIcdCm.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void checkEdit1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkUpdateOnly.Focus();
                }
                if (e.KeyCode == Keys.Space)
                {
                    chkUpdateAll.Checked = !chkUpdateAll.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void checkEdit2_KeyUp(object sender, KeyEventArgs e)
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
                if (e.KeyCode == Keys.Space)
                {
                    chkUpdateOnly.Checked = !chkUpdateOnly.Checked;
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServiceType.EditValue != null)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString() ?? "0") == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString() ?? "0") == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                    {
                        SetEnablePTTT(true);
                    }
                    else
                    {
                        SetEnablePTTT(false);
                    }

                    if (Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString() ?? "0") == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                    {
                        txtSpecialityCode.Enabled = true;
                    }
                    else
                    {
                        txtSpecialityCode.Enabled = false;
                        txtPackagePrice.EditValue = null;
                    }
                }
                else
                {
                    SetEnablePTTT(true);
                    txtSpecialityCode.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetEnablePTTT(bool set)
        {
            try
            {

                cboMethod.Enabled = set;
                cboGroup.Enabled = set;
                cboIcdCm.Enabled = set;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkChiDinh_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCPNgoaiGoi.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkUpdateAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkUpdateAll.Checked)
                {
                    chkUpdateOnly.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkUpdateOnly_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkUpdateOnly.Checked)
                {
                    chkUpdateAll.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInTime_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void dtIntrTime_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void setEnableSpinHein1(bool set, SpinEdit sp, ChangingEventArgs e)
        {
            try
            {
                if (e.NewValue != null && !string.IsNullOrEmpty(e.NewValue.ToString()))
                {
                    spHeinOld.EditValue = null;
                    spHeinNew.EditValue = null;
                    spHeinNew.Enabled = set;
                    spHeinOld.Enabled = set;
                }
                else
                {
                    spHeinNew.Enabled = !set;
                    spHeinOld.Enabled = !set;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void setEnableSpinRatio1(bool set, SpinEdit sp, ChangingEventArgs e)
        {
            try
            {
                if (e.NewValue != null && !string.IsNullOrEmpty(e.NewValue.ToString()))
                {
                    spRatioOld.EditValue = null;
                    spRatioNew.EditValue = null;
                    spRatioNew.Enabled = set;
                    spRatioOld.Enabled = set;
                }
                else
                {
                    spRatioNew.Enabled = !set;
                    spRatioOld.Enabled = !set;
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
                spHeinNew.Enabled = set;
                spHeinOld.Enabled = set;
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
                spRatioNew.Enabled = set;
                spRatioOld.Enabled = set;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spHeinOld_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {
                setEnableSpinRatio1(false, spHeinOld, e);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spHeinNew_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {

                setEnableSpinRatio1(false, spHeinNew, e);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSearchType.Properties.Buttons[1].Visible = true;
                    cboSearchType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spRatioOld_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {
                setEnableSpinHein1(false, spRatioOld, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spRatioNew_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {

                setEnableSpinHein1(false, spRatioNew, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInTime_EditValueChanging(object sender, ChangingEventArgs e)
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

        private void dtIntrTime_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {
                setEnableDtIn(false, e);
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
                    dtIntrTime.EditValue = null;
                    dtIntrTime.Enabled = set;
                }
                else
                {
                    dtIntrTime.Enabled = !set;
                    dtIntrTime.Enabled = !set;
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
                    dtInTime.EditValue = null;
                    dtInTime.Enabled = set;
                }
                else
                {
                    dtInTime.Enabled = !set;
                    dtInTime.Enabled = !set;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSpecialityCode_KeyUp(object sender, KeyEventArgs e)
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPackage_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtPackagePrice.Enabled)
                    {
                        txtPackagePrice.Focus();
                        txtPackagePrice.SelectAll();
                    }
                    else
                    {
                        cboBillOption.Focus();
                        if (cboBillOption.EditValue == null)
                        {
                            cboBillOption.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPackage_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPackage.EditValue != null)
                {
                    txtPackagePrice.Enabled = true;
                    layoutControlItem35.AppearanceItemCaption.ForeColor = Color.Maroon;
                    //ValidationSingleControl(txtPackagePrice);

                }
                else
                {
                    txtPackagePrice.Enabled = false;
                    txtPackagePrice.EditValue = null;
                    layoutControlItem35.AppearanceItemCaption.ForeColor = Color.Transparent;
                    dxValidationProviderEditorInfo.RemoveControlError(txtPackagePrice);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtPackagePrice_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBillOption.Focus();
                    if (cboBillOption.EditValue == null)
                    {
                        cboBillOption.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridLookUpEdit1_Properties_ButtonClick_1(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPackage.Properties.Buttons[1].Visible = true;
                    cboPackage.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btn_ChinhSachGia_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();

                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE row = data as V_HIS_SERVICE;
                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisServicePatyList").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisServicePatyList'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }
                        ((Form)extenceInstance).ShowDialog();
                        WaitingManager.Hide();
                    }
                    else
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btn_BaoCao_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();

                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE row = data as V_HIS_SERVICE;
                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisServiceRetyCat").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisServiceRetyCat'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }
                        ((UserControl)extenceInstance).Show();
                        WaitingManager.Hide();
                    }
                    else
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btn_PhongXuLy_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();

                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE row = data as V_HIS_SERVICE;
                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RoomService").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RoomService'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }
                        ((UserControl)extenceInstance).Show();
                        WaitingManager.Hide();
                    }
                    else
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
       
    }
}
