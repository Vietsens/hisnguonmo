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
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.Plugins.HisMediStock.Resources;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Utilities.Extensions;
using System.Text;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.Plugins.HisMediStock.Validation;
using Newtonsoft.Json;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraGrid;
using System.Data;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMediStock.HisMediStock
{
    public partial class frmHisMediStock : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int roomID = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK currentData;
        List<HIS_MEDI_STOCK> mediStock { get; set; }
        List<string> arrControlEnableNotChange = new List<string>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        internal long roomId;
        internal long mediStockId;
        //List<HIS_IMP_MEST_TYPE> dataImpMestType { get; set; }
        List<HIS_IMP_MEST_TYPE> dataAutoImp { get; set; }
        List<HIS_IMP_MEST_TYPE> dataAutoApproveImp { get; set; }
        List<HIS_EXP_MEST_TYPE> dataExpMestType { get; set; }

        List<HIS_IMP_MEST_TYPE> ImpMestTypeApproveSelecteds;
        List<HIS_IMP_MEST_TYPE> ImpMestTypeImpSelecteds;

        List<HIS_EXP_MEST_TYPE> ExpMestTypeApproveSelecteds;
        List<HIS_EXP_MEST_TYPE> ExpMestTypeExpSelecteds;
        List<HIS_PATIENT_CLASSIFY> ListHisPatientClassify;
        bool isEnableShowDrugStore;
        List<HIS_CONFIG> lstKey = new List<HIS_CONFIG>();
        List<HIS_CONFIG> lstValueQR = new List<HIS_CONFIG>();
        List<HIS_CONFIG> lstConfig = new List<HIS_CONFIG>();
        List<HIS.Desktop.Plugins.HisMediStock.ADO.HisMediStockADO.ItemConfig> itemsToAdd;
        HIS_CONFIG config = null;
        List<V_HIS_CASHIER_ROOM> cashierRoom = new List<V_HIS_CASHIER_ROOM>();
        #endregion

        #region Construct
        public frmHisMediStock(Inventec.Desktop.Common.Modules.Module moduleData)
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
        private void frmHisMediStock_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
                LoaddataToTreeList(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoaddataToTreeList(frmHisMediStock control)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMediStockViewFilter mediStockViewFilter = new MOS.Filter.HisMediStockViewFilter();
                mediStockViewFilter.ORDER_FIELD = "MODIFY_TIME";
                mediStockViewFilter.ORDER_DIRECTION = "DESC";
                mediStockViewFilter.KEY_WORD = txtKeyword.Text;
                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_MEDI_STOCK>>("api/HisMediStock/GetView", ApiConsumers.MosConsumer, mediStockViewFilter, param).ToList();
                if (currentDataStore != null)
                {

                    treeListDataStore.KeyFieldName = "ID";
                    treeListDataStore.ParentFieldName = "PARENT_ID";
                    treeListDataStore.DataSource = currentDataStore;
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMediStock.Resources.Lang", typeof(HIS.Desktop.Plugins.HisMediStock.HisMediStock.frmHisMediStock).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn23.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn20.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn27.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn28.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.treeListColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboParent.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMediStock.cboParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartMentName.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMediStock.cboDepartMentName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsAllowImpSupplier.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIsAllowImpSupplier.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsBusiness.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIsBusiness.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsCabinet.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIsCabinet.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsAutoCreateChmsImp.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIsAutoCreateChmsImp.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkShowDDT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkShowDDT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsUsingPlanningTransDF.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIsUsingPlanningTransDF.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsUsingPlanningTransDF.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIsUsingPlanningTransDF.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsShowInPatientReturnPres.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIsShowInPatientReturnPres.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.chkIS_FOR_REJECTED_MOBA.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIS_FOR_REJECTED_MOBA.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIS_FOR_REJECTED_MOBA.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIS_FOR_REJECTED_MOBA.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.chkIS_MOBA_CHANGE_AMOUNT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIS_MOBA_CHANGE_AMOUNT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIS_MOBA_CHANGE_AMOUNT.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIS_MOBA_CHANGE_AMOUNT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.chkIsDrugstore.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIsDrugstore.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsDrugstore.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIsDrugstore.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.chkIsGoodsRestrict.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkIsGoodsRestrict.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMediStockCode.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.lciMediStockCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMediStockName.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.lciMediStockName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciParentId.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.lciParentId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsAllowImpSupplier.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.lciIsAllowImpSupplier.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsBusiness.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.lciIsBusiness.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsCabinet.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.lciIsCabinet.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsAutoCreateChmsImp.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.lciIsAutoCreateChmsImp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsGoodsRestrict.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.lciIsGoodsRestrict.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeadCode.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.lciHeadCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNotHeadCode.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.lciNotHeadCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmHisMediStock.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.chkNotAllowMedicine.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkNotAllowMedicine.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotAllowMedicine.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkNotAllowMedicine.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotAllowMaterial.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkNotAllowMaterial.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotAllowMaterial.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediStock.chkNotAllowMaterial.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

                this.isEnableShowDrugStore = false;
                chkIsShowDrugStore.Checked = false;
                chkIsShowDrugStore.Properties.Appearance.ForeColor = Color.Gray;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        /// 
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

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboDepartment();
                InitComboParent();

                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboParent()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMediStockViewFilter mediStockFilter = new HisMediStockViewFilter();
                var mediStock = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_MEDI_STOCK>>(
                   "api/HisMediStock/GetView",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    mediStockFilter,
                    param);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboParent, mediStock, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo

        private void InitComboDepartment()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDepartMentName, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void SetFilterNavBar(ref HisMediStockViewFilter filter)
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
                    var data = treeListDataStore.GetDataRecordByNode(treeListDataStore.FocusedNode);
                    V_HIS_MEDI_STOCK rowData = data as V_HIS_MEDI_STOCK;
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                    {
                        btnEdit.Enabled = false;
                    }
                    //btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK data)
        {
            try
            {
                if (data != null)
                {
                    var Room = BackendDataWorker.Get<HIS_ROOM>().Where(o => o.ID == data.ROOM_ID).FirstOrDefault();
                    //CommonParam paramCommon = new CommonParam();
                    //MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                    //var resultData = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_ROOM>>("api/HisRoom/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    //var Room = resultData.Where(o => o.ID == data.ROOM_ID).FirstOrDefault();
                    if (Room != null)
                    {
                        cboRooomTN.EditValue = Room.DEFAULT_CASHIER_ROOM_ID;
                        if (cboRooomTN.EditValue != null)
                        {
                            cboRooomTN.Properties.Buttons[0].Visible = true;
                        }
                    }
                    txtMediStockCode.Text = data.MEDI_STOCK_CODE;
                    txtMediStockName.Text = data.MEDI_STOCK_NAME;
                    txtHeadCode.Text = data.BHYT_HEAD_CODE;
                    txtNotHeadCode.Text = data.NOT_IN_BHYT_HEAD_CODE;
                    txtTLQR.Text = data.QR_CONFIG_JSON;
                    cboDepartMentName.EditValue = data.DEPARTMENT_ID;
                    cboParent.EditValue = data.PARENT_ID;
                    chkIsAllowImpSupplier.Checked = (data.IS_ALLOW_IMP_SUPPLIER == 1 ? true : false);
                    chkIsBusiness.Checked = (data.IS_BUSINESS == 1 ? true : false);
                    chkIsBusiness.ReadOnly = true;
                    chkIsCabinet.Checked = (data.IS_CABINET == 1 ? true : false);
                    chkIsCabinet.ReadOnly = true;
                    chkIsAutoCreateChmsImp.Checked = (data.IS_AUTO_CREATE_CHMS_IMP == 1 ? true : false);
                    chkIsAutoCreateReImp.Checked = (data.IS_AUTO_CREATE_REUSABLE_IMP == 1 ? true : false);
                    chkIsGoodsRestrict.Checked = (data.IS_GOODS_RESTRICT == 1 ? true : false);
                    chkOdd.Checked = (data.IS_ODD == 1 ? true : false);
                    chkIsBlood.Checked = (data.IS_BLOOD == 1 ? true : false);
                    chkIsNewMedicine.Checked = (data.IS_NEW_MEDICINE == 1 ? true : false);
                    chkIsTraditionalMedicine.Checked = (data.IS_TRADITIONAL_MEDICINE == 1 ? true : false);
                    chkIsDrugstore.Checked = (data.IS_DRUG_STORE == 1 ? true : false);
                    chkIsShowDrugStore.Checked = (data.IS_SHOW_DRUG_STORE == 1 ? true : false);
                    chkIsShowInPatientReturnPres.Checked = (data.IS_SHOW_INPATIENT_RETURN_PRES == 1 ? true : false);
                    chkShowDDT.Checked = (data.IS_SHOW_DDT == 1 ? true : false);
                    chkIsUsingPlanningTransDF.Checked = (data.IS_PLANNING_TRANS_AS_DEFAULT == 1 ? true : false);
                    chkNotAllowMedicine.Checked = (data.DO_NOT_IMP_MEDICINE == 1 ? true : false);
                    chkNotAllowMaterial.Checked = (data.DO_NOT_IMP_MATERIAL == 1 ? true : false);
                    cboCabinetManageOption.SelectedIndex = data.CABINET_MANAGE_OPTION.HasValue ? (data.CABINET_MANAGE_OPTION.Value - 1) : -1;

                    chkIS_FOR_REJECTED_MOBA.Checked = (data.IS_FOR_REJECTED_MOBA == 1 ? true : false);
                    chkIS_MOBA_CHANGE_AMOUNT.Checked = (data.IS_MOBA_CHANGE_AMOUNT == 1 ? true : false);
                    chkIsExpend.Checked = (data.IS_EXPEND == 1 ? true : false);
                    chkIsShowAnticipate.Checked = (data.IS_SHOW_ANTICIPATE == 1 ? true : false);
                    CommonParam param = new CommonParam();

                    GridCheckMarksSelection gridCheckMarkExpApprove = cboExpMestTypeApprove.Properties.Tag as GridCheckMarksSelection;
                    GridCheckMarksSelection gridCheckMarkExpMestExp = cboExpMestTypeExp.Properties.Tag as GridCheckMarksSelection;
                    GridCheckMarksSelection gridCheckMarkImpMestApprove = cboImpMestTypeApprove.Properties.Tag as GridCheckMarksSelection;
                    GridCheckMarksSelection gridCheckMarkImpMestImp = cboImpMestTypeImp.Properties.Tag as GridCheckMarksSelection;
                    GridCheckMarksSelection gridCheckMarkPhanloaiBenhNhan = cboPhanLoaiBenhNhan.Properties.Tag as GridCheckMarksSelection;


                    gridCheckMarkExpMestExp.ClearSelection(cboExpMestTypeExp.Properties.View);
                    gridCheckMarkImpMestImp.ClearSelection(cboImpMestTypeImp.Properties.View);
                    gridCheckMarkExpApprove.ClearSelection(cboExpMestTypeApprove.Properties.View);
                    gridCheckMarkImpMestApprove.ClearSelection(cboImpMestTypeApprove.Properties.View);
                    gridCheckMarkPhanloaiBenhNhan.ClearSelection(cboPhanLoaiBenhNhan.Properties.View);

                    if (data.PATIENT_CLASSIFY_IDS != null)
                    {
                        var data_ = data.PATIENT_CLASSIFY_IDS.Split(',');
                        ListHisPatientClassify = new List<HIS_PATIENT_CLASSIFY>();
                        foreach (var item in data_)
                        {

                            HIS_PATIENT_CLASSIFY HisPatientClassify = new HIS_PATIENT_CLASSIFY();
                            HisPatientClassify = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().FirstOrDefault(o => o.ID == long.Parse(item));
                            //HisPatientClassify.ID =  long.Parse(item);
                            ListHisPatientClassify.Add(HisPatientClassify);
                        }
                        gridCheckMarkPhanloaiBenhNhan.SelectAll(ListHisPatientClassify);

                    }


                    HisMediStockImtyFilter imtyFilter = new HisMediStockImtyFilter();
                    imtyFilter.MEDI_STOCK_ID = data.ID;
                    var listMediStockImty = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK_IMTY>>("api/HisMediStockImty/Get", ApiConsumers.MosConsumer, imtyFilter, param).ToList();
                    if (listMediStockImty != null && listMediStockImty.Count > 0)
                    {
                        ImpMestTypeApproveSelecteds = new List<HIS_IMP_MEST_TYPE>();
                        ImpMestTypeImpSelecteds = new List<HIS_IMP_MEST_TYPE>();
                        foreach (var item in listMediStockImty)
                        {
                            if (item.IS_AUTO_APPROVE == 1)
                            {
                                ImpMestTypeApproveSelecteds.Add(dataAutoApproveImp.FirstOrDefault(o => o.ID == item.IMP_MEST_TYPE_ID));
                            }
                            if (item.IS_AUTO_EXECUTE == 1)
                            {
                                ImpMestTypeImpSelecteds.Add(dataAutoImp.FirstOrDefault(o => o.ID == item.IMP_MEST_TYPE_ID));
                            }
                        }
                        gridCheckMarkImpMestApprove.SelectAll(ImpMestTypeApproveSelecteds);
                        gridCheckMarkImpMestImp.SelectAll(ImpMestTypeImpSelecteds);
                    }
                    else
                    {

                        ImpMestTypeImpSelecteds = new List<HIS_IMP_MEST_TYPE>();
                        ImpMestTypeApproveSelecteds = new List<HIS_IMP_MEST_TYPE>();
                    }

                    HisMediStockExtyFilter extyFilter = new HisMediStockExtyFilter();
                    extyFilter.MEDI_STOCK_ID = data.ID;
                    var listMediStockExty = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK_EXTY>>("api/HisMediStockExty/Get", ApiConsumers.MosConsumer, extyFilter, param).ToList();
                    if (listMediStockExty != null && listMediStockExty.Count > 0)
                    {
                        ExpMestTypeApproveSelecteds = new List<HIS_EXP_MEST_TYPE>();
                        ExpMestTypeExpSelecteds = new List<HIS_EXP_MEST_TYPE>();
                        foreach (var item in listMediStockExty)
                        {
                            if (item.IS_AUTO_APPROVE == 1)
                            {
                                ExpMestTypeApproveSelecteds.Add(dataExpMestType.FirstOrDefault(o => o.ID == item.EXP_MEST_TYPE_ID));
                            }
                            if (item.IS_AUTO_EXECUTE == 1)
                            {
                                ExpMestTypeExpSelecteds.Add(dataExpMestType.FirstOrDefault(o => o.ID == item.EXP_MEST_TYPE_ID));
                            }
                        }

                        gridCheckMarkExpMestExp.SelectAll(ExpMestTypeExpSelecteds);
                        gridCheckMarkExpApprove.SelectAll(ExpMestTypeApproveSelecteds);
                    }
                    else
                    {
                        ExpMestTypeApproveSelecteds = new List<HIS_EXP_MEST_TYPE>();
                        ExpMestTypeExpSelecteds = new List<HIS_EXP_MEST_TYPE>();
                    }
                }
                //cboImpMestTypeImp.ShowPopup();
                //cboImpMestTypeImp.ClosePopup();
                cboExpMestTypeApprove.Focus();
                cboExpMestTypeExp.Focus();
                cboImpMestTypeApprove.Focus();
                cboImpMestTypeImp.Focus();
                txtMediStockCode.Focus();
                txtMediStockCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void EventcboExpMestTypeApprove()
        //{
        //    try
        //    {
        //        cboExpMestTypeApprove.ShowPopup();
        //        cboExpMestTypeApprove.ClosePopup();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //}

        //private void EventcboExpMestTypeExp()
        //{
        //    try
        //    {
        //        cboExpMestTypeExp.ShowPopup();
        //        cboExpMestTypeExp.ClosePopup();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //}

        //private void EventtxtMediStockCode()
        //{
        //    try
        //    {
        //        txtMediStockCode.Focus();
        //        txtMediStockCode.SelectAll();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //}

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>

        private void ResetFormData()
        {
            try
            {
                currentData = null;
                txtMediStockCode.Text = "";
                txtMediStockName.Text = "";
                txtHeadCode.Text = "";
                txtNotHeadCode.Text = "";
                txtTLQR.Text = "";
                cboRooomTN.EditValue = null;
                cboDepartMentName.EditValue = null;
                cboParent.EditValue = null;
                GridCheckMarksSelection gridCheckMark = cboExpMestTypeApprove.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboExpMestTypeApprove.Properties.View);
                    cboExpMestTypeApprove.Text = "";
                }

                GridCheckMarksSelection gridCheckMark1 = cboExpMestTypeExp.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark1 != null)
                {
                    gridCheckMark1.ClearSelection(cboExpMestTypeExp.Properties.View);
                    cboExpMestTypeExp.Text = "";
                }

                GridCheckMarksSelection gridCheckMark2 = cboImpMestTypeApprove.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark2 != null)
                {
                    gridCheckMark2.ClearSelection(cboImpMestTypeApprove.Properties.View);
                    cboImpMestTypeApprove.Text = "";
                }

                GridCheckMarksSelection gridCheckMark3 = cboImpMestTypeImp.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark3 != null)
                {
                    gridCheckMark3.ClearSelection(cboImpMestTypeImp.Properties.View);
                    cboImpMestTypeImp.Text = "";
                }

                GridCheckMarksSelection gridCheckMark4 = cboPhanLoaiBenhNhan.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark4 != null)
                {
                    gridCheckMark4.ClearSelection(cboPhanLoaiBenhNhan.Properties.View);
                    cboPhanLoaiBenhNhan.Text = "";
                    cboPhanLoaiBenhNhan.EditValue = null;
                    ListHisPatientClassify.Clear();
                    ListHisPatientClassify = new List<HIS_PATIENT_CLASSIFY>();

                }

                chkIsGoodsRestrict.CheckState = CheckState.Unchecked;
                chkIsAllowImpSupplier.CheckState = CheckState.Unchecked;
                chkIsAutoCreateChmsImp.CheckState = CheckState.Checked;
                chkIsAutoCreateReImp.CheckState = CheckState.Unchecked;
                chkIsBusiness.CheckState = CheckState.Unchecked;
                chkIsCabinet.CheckState = CheckState.Unchecked;
                chkOdd.CheckState = CheckState.Unchecked;
                chkIsBlood.CheckState = CheckState.Unchecked;
                chkIsNewMedicine.CheckState = CheckState.Unchecked;
                chkIsTraditionalMedicine.CheckState = CheckState.Unchecked;
                chkIsDrugstore.CheckState = CheckState.Unchecked;
                chkIsShowInPatientReturnPres.CheckState = CheckState.Unchecked;
                chkIS_FOR_REJECTED_MOBA.CheckState = CheckState.Unchecked;
                chkIS_MOBA_CHANGE_AMOUNT.CheckState = CheckState.Unchecked;
                chkIsShowDrugStore.CheckState = CheckState.Unchecked;
                chkIsExpend.CheckState = CheckState.Unchecked;
                chkIsShowAnticipate.CheckState = CheckState.Unchecked;
                txtTLQR.Properties.Buttons[0].Visible = false;
                cboRooomTN.Properties.Buttons[1].Visible = false;
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                chkIsCabinet.ReadOnly = chkIsBusiness.ReadOnly = false;
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
                FillDataToControlsForm();
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
                MOS.SDO.HisMediStockSDO mediStockSDO = new MOS.SDO.HisMediStockSDO();
                MOS.SDO.HisMediStockSDO mediStockResultSDO = new MOS.SDO.HisMediStockSDO();

                mediStockSDO.HisRoom = SetDataRoom();
                mediStockSDO.HisMediStock = SetDataMediStock();

                Dictionary<long, HIS_MEDI_STOCK_EXTY> dicMediStockExty = new Dictionary<long, HIS_MEDI_STOCK_EXTY>();
                Dictionary<long, HIS_MEDI_STOCK_IMTY> dicMediStockImty = new Dictionary<long, HIS_MEDI_STOCK_IMTY>();




                foreach (var item in ExpMestTypeApproveSelecteds)
                {
                    if (!dicMediStockExty.ContainsKey(item.ID))
                    {
                        HIS_MEDI_STOCK_EXTY MediStockExty = new HIS_MEDI_STOCK_EXTY();
                        MediStockExty.EXP_MEST_TYPE_ID = item.ID;
                        MediStockExty.IS_AUTO_APPROVE = 1;
                        dicMediStockExty[item.ID] = MediStockExty;
                    }
                }
                foreach (var item in ExpMestTypeExpSelecteds)
                {
                    if (dicMediStockExty.ContainsKey(item.ID))
                    {
                        dicMediStockExty[item.ID].IS_AUTO_EXECUTE = 1;
                    }
                    else
                    {
                        HIS_MEDI_STOCK_EXTY MediStockExty = new HIS_MEDI_STOCK_EXTY();
                        MediStockExty.EXP_MEST_TYPE_ID = item.ID;
                        MediStockExty.IS_AUTO_EXECUTE = 1;
                        dicMediStockExty[item.ID] = MediStockExty;
                    }
                }

                foreach (var item in ImpMestTypeApproveSelecteds)
                {
                    if (!dicMediStockImty.ContainsKey(item.ID))
                    {
                        HIS_MEDI_STOCK_IMTY MediStockImty = new HIS_MEDI_STOCK_IMTY();
                        MediStockImty.IMP_MEST_TYPE_ID = item.ID;
                        MediStockImty.IS_AUTO_APPROVE = 1;
                        dicMediStockImty[item.ID] = MediStockImty;
                    }
                }
                foreach (var item in ImpMestTypeImpSelecteds)
                {
                    if (dicMediStockImty.ContainsKey(item.ID))
                    {
                        dicMediStockImty[item.ID].IS_AUTO_EXECUTE = 1;
                    }
                    else
                    {
                        HIS_MEDI_STOCK_IMTY MediStockImty = new HIS_MEDI_STOCK_IMTY();
                        MediStockImty.IMP_MEST_TYPE_ID = item.ID;
                        MediStockImty.IS_AUTO_EXECUTE = 1;
                        dicMediStockImty[item.ID] = MediStockImty;
                    }
                }

                mediStockSDO.HisMediStockExtys = dicMediStockExty.Select(o => o.Value).ToList();
                mediStockSDO.HisMediStockImtys = dicMediStockImty.Select(o => o.Value).ToList();

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    mediStockResultSDO = new BackendAdapter(param)
                    .Post<MOS.SDO.HisMediStockSDO>("api/HisMediStock/Create", ApiConsumers.MosConsumer, mediStockSDO, param);

                }
                else
                {
                    if (roomId > 0 && mediStockId > 0)
                    {
                        if (cboParent.Text == txtMediStockName.Text)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Cha trùng tên của chính nó", "Thông báo");
                            return;
                        }
                        mediStockSDO.HisRoom.ID = roomId;
                        mediStockSDO.HisMediStock.ID = mediStockId;
                        mediStockResultSDO = new BackendAdapter(param)
                        .Post<HisMediStockSDO>("api/HisMediStock/Update", ApiConsumers.MosConsumer, mediStockSDO, param);
                    }
                }
                LogSystem.Error("Đầu vào api/HisMediStock: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediStockSDO), mediStockSDO));
                if (mediStockResultSDO != null)
                {
                    success = true;
                    LoaddataToTreeList(this);
                    ResetFormData();
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

        private HIS_MEDI_STOCK SetDataMediStock()
        {
            HIS_MEDI_STOCK mediStock = new HIS_MEDI_STOCK();
            try
            {
                if (!String.IsNullOrEmpty(txtMediStockCode.Text))
                    mediStock.MEDI_STOCK_CODE = txtMediStockCode.Text;

                if (!String.IsNullOrEmpty(txtMediStockName.Text))
                    mediStock.MEDI_STOCK_NAME = txtMediStockName.Text;

                if (!String.IsNullOrEmpty(txtHeadCode.Text))
                    mediStock.BHYT_HEAD_CODE = txtHeadCode.Text;

                if (!String.IsNullOrEmpty(txtNotHeadCode.Text))
                    mediStock.NOT_IN_BHYT_HEAD_CODE = txtNotHeadCode.Text;

                mediStock.IS_GOODS_RESTRICT = (chkIsGoodsRestrict.Checked ? (short?)1 : null);
                mediStock.IS_ALLOW_IMP_SUPPLIER = (chkIsAllowImpSupplier.Checked ? (short?)1 : null);
                mediStock.IS_BLOOD = (chkIsBlood.Checked ? (short?)1 : null);
                mediStock.IS_TRADITIONAL_MEDICINE = (chkIsTraditionalMedicine.Checked ? (short?)1 : null);
                mediStock.IS_SHOW_INPATIENT_RETURN_PRES = (chkIsShowInPatientReturnPres.Checked ? (short?)1 : null);
                mediStock.IS_NEW_MEDICINE = (chkIsNewMedicine.Checked ? (short?)1 : null);
                //mediStock.IS_AUTO_EXECUTE_IMP = (short)(chkAutoImport.Checked ? 1 : 0);
                //mediStock.IS_AUTO_APPROVE_IMP = (short)(chkIsAutoApproveImp.Checked ? 1 : 0);
                mediStock.IS_AUTO_CREATE_CHMS_IMP = (chkIsAutoCreateChmsImp.Checked ? (short?)1 : null);
                mediStock.IS_AUTO_CREATE_REUSABLE_IMP = (chkIsAutoCreateReImp.Checked ? (short?)1 : null);
                // mediStock.IS_AUTO_EXECUTE_EXP = (short)(chkIsAutoExecuteExp.Checked ? 1 : 0);
                mediStock.IS_BUSINESS = (chkIsBusiness.Checked ? (short?)1 : null);
                mediStock.IS_CABINET = (chkIsCabinet.Checked ? (short?)1 : null);
                mediStock.IS_ODD = (chkOdd.Checked ? (short?)1 : null);
                mediStock.IS_SHOW_DDT = (chkShowDDT.Checked ? (short?)1 : null);
                mediStock.IS_PLANNING_TRANS_AS_DEFAULT = (chkIsUsingPlanningTransDF.Checked ? (short?)1 : null);
                if (chkIsCabinet.Checked && cboCabinetManageOption.SelectedIndex >= 0)
                {
                    mediStock.CABINET_MANAGE_OPTION = (short)(cboCabinetManageOption.SelectedIndex + 1);
                }

                mediStock.IS_FOR_REJECTED_MOBA = (chkIS_FOR_REJECTED_MOBA.Checked ? (short?)1 : null);

                mediStock.IS_MOBA_CHANGE_AMOUNT = (chkIS_MOBA_CHANGE_AMOUNT.Checked ? (short?)1 : null);
                mediStock.IS_SHOW_ANTICIPATE = (chkIsShowAnticipate.Checked ? (short?)1 : null);

                if (cboParent.EditValue != null)
                    mediStock.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboParent.EditValue.ToString());
                if (ListHisPatientClassify != null)
                {
                    mediStock.PATIENT_CLASSIFY_IDS = String.Join(",", ListHisPatientClassify.Select(o => o.ID).ToList()).ToString();
                }

                if (chkIsDrugstore.Checked)
                {
                    mediStock.IS_DRUG_STORE = 1;
                }
                else
                {
                    mediStock.IS_DRUG_STORE = null;
                }
                if (chkIsShowDrugStore.Checked)
                {
                    mediStock.IS_SHOW_DRUG_STORE = 1;
                }
                else
                {
                    mediStock.IS_SHOW_DRUG_STORE = null;
                }
                mediStock.IS_EXPEND = (chkIsExpend.Checked ? (short?)1 : null);
                mediStock.DO_NOT_IMP_MEDICINE = (chkNotAllowMedicine.Checked ? (short?)1 : null);
                mediStock.DO_NOT_IMP_MATERIAL = (chkNotAllowMaterial.Checked ? (short?)1 : null);

            }
            catch (Exception ex)
            {
                mediStock = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return mediStock;
        }

        private HIS_ROOM SetDataRoom()
        {
            HIS_ROOM Parent = new HIS_ROOM();
            try
            {
                //if (cboParent.EditValue != null) Parent = Inventec.Common.TypeConvert.Parse.ToInt64((cboParent.EditValue ?? "0").ToString());
                if (!string.IsNullOrEmpty(txtTLQR.Text))
                {
                    Parent.QR_CONFIG_JSON = txtTLQR.Text.Trim();
                }
                if (cboRooomTN.EditValue == null )
                {
                    Parent.DEFAULT_CASHIER_ROOM_ID = null;
                }
                else 
                {
                    Parent.DEFAULT_CASHIER_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboRooomTN.EditValue.ToString());
                }
                if (cboDepartMentName.EditValue != null)
                    Parent.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartMentName.EditValue.ToString());
            }
            catch (Exception ex)
            {
                Parent = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return Parent;
        }

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_MEDI_STOCK) is null");
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
                ValidationSingleControl(txtMediStockCode);
                ValidationSingleControl(txtMediStockName);
                ValidationSingleControl(cboDepartMentName);

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
                validRule.ErrorText = String.Format(ResourceMessage.ChuaNhapTruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl()
        {
            try
            {
                ValidMaxlengthtxtMediStockCode validRule = new ValidMaxlengthtxtMediStockCode();
                validRule.txtMediStockCode = this.txtMediStockCode;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(txtMediStockCode, validRule);
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

                //LOAD phòng tn
                LoadDataToComboCashierRoom();
                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();

                //Fill data into datasource expMest ImpMest

                dataAutoImp = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT).ToList();
                dataAutoApproveImp = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL).ToList();

                dataExpMestType = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>().Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL && o.ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT).ToList();
                //dataAutoApproveExp = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>();

                InitImpMestTypeApproveCheck();
                LoadDataToComboImpMestTypeApprove();
                InitImpMestTypeImpCheck();
                LoadDataToComboImpMestTypeImp();

                InitExpMestTypeApproveCheck();
                LoadDataToComboExpMestTypeApprove();
                InitExpMestTypeExpCheck();
                LoadDataToComboExpMestTypeExp();
                chkIsAutoCreateChmsImp.CheckState = CheckState.Checked;
                LoadDataToComboPhanLoaiBenhNhan();
                InitPhanLoaiBenhNhan();
                txtTLQR.Properties.Buttons[0].Visible = false;
                cboRooomTN.Properties.Buttons[1].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region cbo check Imty Exty
        private void InitExpMestTypeExpCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboExpMestTypeExp.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ExpMestTypeExp);
                cboExpMestTypeExp.Properties.Tag = gridCheck;
                cboExpMestTypeExp.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboExpMestTypeExp.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboExpMestTypeExp.Properties.View);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__ExpMestTypeExp(object sender, EventArgs e)
        {
            try
            {
                ExpMestTypeExpSelecteds = new List<HIS_EXP_MEST_TYPE>();
                StringBuilder sb = new StringBuilder();
                foreach (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {

                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.EXP_MEST_TYPE_NAME.ToString());
                        ExpMestTypeExpSelecteds.Add(rv);
                    }
                }
                cboExpMestTypeExp.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboExpMestTypeExp()
        {
            try
            {
                if (dataExpMestType != null)
                {
                    cboExpMestTypeExp.Properties.DataSource = dataExpMestType;
                    cboExpMestTypeExp.Properties.DisplayMember = "EXP_MEST_TYPE_NAME";
                    cboExpMestTypeExp.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboExpMestTypeExp.Properties.View.Columns.AddField("EXP_MEST_TYPE_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 300;
                    col2.Caption = "";
                    cboExpMestTypeExp.Properties.PopupFormWidth = 300;
                    cboExpMestTypeExp.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboExpMestTypeExp.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboExpMestTypeExp.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboExpMestTypeExp.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitExpMestTypeApproveCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboExpMestTypeApprove.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ExpMestTypeApprove);
                cboExpMestTypeApprove.Properties.Tag = gridCheck;
                cboExpMestTypeApprove.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboExpMestTypeApprove.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboExpMestTypeApprove.Properties.View);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__ExpMestTypeApprove(object sender, EventArgs e)
        {
            try
            {
                ExpMestTypeApproveSelecteds = new List<HIS_EXP_MEST_TYPE>();
                StringBuilder sb = new StringBuilder();
                foreach (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.EXP_MEST_TYPE_NAME.ToString());
                        ExpMestTypeApproveSelecteds.Add(rv);
                    }
                }
                cboExpMestTypeApprove.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboExpMestTypeApprove()
        {
            try
            {

                if (dataExpMestType != null)
                {
                    cboExpMestTypeApprove.Properties.DataSource = dataExpMestType;
                    cboExpMestTypeApprove.Properties.DisplayMember = "EXP_MEST_TYPE_NAME";
                    cboExpMestTypeApprove.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboExpMestTypeApprove.Properties.View.Columns.AddField("EXP_MEST_TYPE_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 300;
                    col2.Caption = "";
                    cboExpMestTypeApprove.Properties.PopupFormWidth = 300;
                    cboExpMestTypeApprove.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboExpMestTypeApprove.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboExpMestTypeApprove.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboExpMestTypeApprove.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitImpMestTypeImpCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboImpMestTypeImp.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ImpMestTypeImp);
                cboImpMestTypeImp.Properties.Tag = gridCheck;
                cboImpMestTypeImp.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboImpMestTypeImp.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboImpMestTypeImp.Properties.View);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__ImpMestTypeImp(object sender, EventArgs e)
        {
            try
            {
                ImpMestTypeImpSelecteds = new List<HIS_IMP_MEST_TYPE>();
                StringBuilder sb = new StringBuilder();
                foreach (MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.IMP_MEST_TYPE_NAME.ToString());
                        ImpMestTypeImpSelecteds.Add(rv);
                    }
                }
                cboExpMestTypeExp.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboImpMestTypeImp()
        {
            try
            {
                if (dataAutoImp != null)
                {
                    cboImpMestTypeImp.Properties.DataSource = dataAutoImp;
                    cboImpMestTypeImp.Properties.DisplayMember = "IMP_MEST_TYPE_NAME";
                    cboImpMestTypeImp.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboImpMestTypeImp.Properties.View.Columns.AddField("IMP_MEST_TYPE_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 300;
                    col2.Caption = "";
                    cboImpMestTypeImp.Properties.PopupFormWidth = 300;
                    cboImpMestTypeImp.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboImpMestTypeImp.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboImpMestTypeImp.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboImpMestTypeImp.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitImpMestTypeApproveCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboImpMestTypeApprove.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ImpMestTypeApprove);
                cboImpMestTypeApprove.Properties.Tag = gridCheck;
                cboImpMestTypeApprove.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboImpMestTypeApprove.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboImpMestTypeApprove.Properties.View);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__ImpMestTypeApprove(object sender, EventArgs e)
        {
            try
            {
                ImpMestTypeApproveSelecteds = new List<HIS_IMP_MEST_TYPE>();
                StringBuilder sb = new StringBuilder();
                foreach (MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.IMP_MEST_TYPE_NAME.ToString());
                        ImpMestTypeApproveSelecteds.Add(rv);
                    }
                }
                cboImpMestTypeApprove.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboImpMestTypeApprove()
        {
            try
            {
                if (dataAutoApproveImp != null)
                {
                    cboImpMestTypeApprove.Properties.DataSource = dataAutoApproveImp;
                    cboImpMestTypeApprove.Properties.DisplayMember = "IMP_MEST_TYPE_NAME";
                    cboImpMestTypeApprove.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboImpMestTypeApprove.Properties.View.Columns.AddField("IMP_MEST_TYPE_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 300;
                    col2.Caption = "";
                    cboImpMestTypeApprove.Properties.PopupFormWidth = 300;
                    cboImpMestTypeApprove.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboImpMestTypeApprove.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboImpMestTypeApprove.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboImpMestTypeApprove.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void LoadDataToComboCashierRoom()
        {
            try
            {
                //long branchId;
                //branchId = HIS.Desktop.LocalStorage.Branch.BranchWorker.GetBranchId(); // WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId;
                //var userRoomIds = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                //    && o.BRANCH_ID == branchId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN).Select(s => s.ROOM_ID).ToList();
                //LoadDataCashierRoom();
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>>("api/HisCashierRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                cashierRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>();
                cashierRoom = cashierRoom.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CASHIER_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboRooomTN, cashierRoom, controlEditorADO);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCashierRoom()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadHisCashierRoom => 1");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                var result =  new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>>("api/HisCashierRoom/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM), result, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                var result1 =  new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>>("api/HisCashierRoom/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                if (result1 != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM), result1, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                Inventec.Common.Logging.LogSystem.Info("async Task LoadHisCashierRoom => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDataToComboPhanLoaiBenhNhan()
        {
            try
            {
                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>().Where(o => o.IS_ACTIVE == 1).ToList();
                if (data != null && data.Count > 0)
                {
                    cboPhanLoaiBenhNhan.Properties.DataSource = data.ToList();
                    cboPhanLoaiBenhNhan.Properties.DisplayMember = "PATIENT_CLASSIFY_NAME";
                    cboPhanLoaiBenhNhan.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboPhanLoaiBenhNhan.Properties.View.Columns.AddField("PATIENT_CLASSIFY_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 300;
                    col2.Caption = "";
                    cboPhanLoaiBenhNhan.Properties.PopupFormWidth = 300;
                    cboPhanLoaiBenhNhan.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboPhanLoaiBenhNhan.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboPhanLoaiBenhNhan.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboPhanLoaiBenhNhan.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitPhanLoaiBenhNhan()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboPhanLoaiBenhNhan.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__PhanLoaiBenhNhan);
                cboPhanLoaiBenhNhan.Properties.Tag = gridCheck;
                cboPhanLoaiBenhNhan.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboPhanLoaiBenhNhan.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboPhanLoaiBenhNhan.Properties.View);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__PhanLoaiBenhNhan(object sender, EventArgs e)
        {
            try
            {
                ListHisPatientClassify = new List<HIS_PATIENT_CLASSIFY>();
                StringBuilder sb = new StringBuilder();
                foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.PATIENT_CLASSIFY_NAME.ToString());
                        ListHisPatientClassify.Add(rv);
                    }
                }
                cboPhanLoaiBenhNhan.Text = sb.ToString();
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

        private void treeListDataStore_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    V_HIS_MEDI_STOCK pData = e.Row as V_HIS_MEDI_STOCK;
                    if (pData == null || this.treeListDataStore == null) return;
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
                    else if (e.Column.FieldName == "IS_ALLOW_IMP_SUPPLIER_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_ALLOW_IMP_SUPPLIER == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi cho phep nhap tu NCC IS_ALLOW_IMP_SUPPLIER_STR", ex);
                        }

                    }
                    else if (e.Column.FieldName == "IS_BLOOD_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_BLOOD == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_BLOOD_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_TRADITIONAL_MEDICINE_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_TRADITIONAL_MEDICINE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_TRADITIONAL_MEDICINE_STR", ex);
                        }
                    }

                        //quầy thuốc
                    else if (e.Column.FieldName == "IS_DRUGSTORE_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_DRUG_STORE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_DRUGSTORE_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_NEW_MEDICINE_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_NEW_MEDICINE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_NEW_MEDICINE_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_ODD_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_ODD == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi cho phep nhap tu NCC IS_ODD_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_BUSINESS_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_BUSINESS == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri kho kinh doanh IS_BUSINESS_STR", ex);
                        }
                    }


                    //else if (e.Column.FieldName == "IS_AUTO_APPROVE_EXP_STR")
                    //{
                    //    try
                    //    {
                    //        e.Value = pData != null && pData.IS_AUTO_APPROVE_EXP == 1 ? true : false;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_AUTO_APPROVE_EXP_STR", ex);
                    //    }
                    //}
                    //else if (e.Column.FieldName == "IS_AUTO_EXECUTE_EXP_STR")
                    //{
                    //    try
                    //    {
                    //        e.Value = pData != null && pData.IS_AUTO_EXECUTE_EXP == 1 ? true : false;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_AUTO_EXECUTE_EXP_STR", ex);
                    //    }
                    //}
                    else if (e.Column.FieldName == "IS_CABINET_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_CABINET == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_CABINET_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_AUTO_CREATE_CHMS_IMP_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_AUTO_CREATE_CHMS_IMP == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_AUTO_CREATE_CHMS_IMP_STR", ex);
                        }
                    }
                    //else if (e.Column.FieldName == "IS_AUTO_APPROVE_IMP_STR")
                    //{
                    //    try
                    //    {
                    //        e.Value = pData != null && pData.IS_AUTO_APPROVE_IMP == 1 ? true : false;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_AUTO_APPROVE_IMP_STR", ex);
                    //    }
                    //}
                    else if (e.Column.FieldName == "IS_GOODS_RESTRICT_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_GOODS_RESTRICT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_GOODS_RESTRICT_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_ACTIVE_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_PAUSE_STR")
                    {
                        try
                        {
                            //e.Value = pData != null && pData.IS_PAUSE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri IS_PAUSE_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_USING_PLANNING_TRANS_AS_DEFAULT_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_PLANNING_TRANS_AS_DEFAULT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri kho kinh doanh IS_PLANNING_TRANS_AS_DEFAULT", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_FOR_REJECTED_MOBA_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_FOR_REJECTED_MOBA == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri kho kinh doanh IS_FOR_REJECTED_MOBA", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_MOBA_CHANGE_AMOUNT_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_MOBA_CHANGE_AMOUNT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri kho kinh doanh IS_MOBA_CHANGE_AMOUNT", ex);
                        }
                    }
                    //treeListDataStore.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListDataStore_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeListDataStore.GetDataRecordByNode(e.Node);
                if (data != null && data is V_HIS_MEDI_STOCK)
                {
                    V_HIS_MEDI_STOCK rowData = data as V_HIS_MEDI_STOCK;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (rowData.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnLock : btnLock);

                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IS_ACTIVE_TRUE)
                                e.RepositoryItem = btnDELETE;
                            else
                                e.RepositoryItem = btnDELETE_Disable;
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

        private void btnLock_Click(object sender, EventArgs e)
        {
            {
                CommonParam param = new CommonParam();
                bool notHandler = false;
                try
                {
                    var data = treeListDataStore.GetDataRecordByNode(treeListDataStore.FocusedNode);
                    V_HIS_MEDI_STOCK dataMediStock = data as V_HIS_MEDI_STOCK;
                    if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        V_HIS_MEDI_STOCK data1 = new V_HIS_MEDI_STOCK();
                        data1.ID = dataMediStock.ID;
                        WaitingManager.Show();
                        var rsApi = new BackendAdapter(param).Post<HIS_MEDI_STOCK>("api/HisMediStock/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                        if (rsApi != null)
                        {
                            LoaddataToTreeList(this);
                            btnEdit.Enabled = false;
                            notHandler = true;
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, notHandler);
                    }
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool notHandler = false;
            try
            {

                var data = treeListDataStore.GetDataRecordByNode(treeListDataStore.FocusedNode);
                V_HIS_MEDI_STOCK dataMediStock = data as V_HIS_MEDI_STOCK;
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonMoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    V_HIS_MEDI_STOCK data1 = new V_HIS_MEDI_STOCK();
                    data1.ID = dataMediStock.ID;
                    var rsApi = new BackendAdapter(param).Post<HIS_MEDI_STOCK>("api/HisMediStock/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (rsApi != null)
                    {
                        LoaddataToTreeList(this);
                        btnEdit.Enabled = true;
                        notHandler = true;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, notHandler);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDELETE_Click(object sender, EventArgs e)
        {
            try
            {

                var data = treeListDataStore.GetDataRecordByNode(treeListDataStore.FocusedNode);
                //V_HIS_MEDI_STOCK rowData 
                V_HIS_MEDI_STOCK rowData = data as V_HIS_MEDI_STOCK;
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

                    if (rowData != null)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisMediStock/Delete", ApiConsumers.MosConsumer, rowData, param);
                        if (success)
                        {
                            LoaddataToTreeList(this);
                            FillDataToControlsForm();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListDataStore_Click(object sender, EventArgs e)
        {
            try
            {
                //var data = treeListDataStore.GetDataRecordByNode(treeListDataStore.FocusedNode);
                //V_HIS_MEDI_STOCK rowData = data as V_HIS_MEDI_STOCK;
                //if (rowData != null)
                //{
                //    roomId = rowData.ROOM_ID;
                //    mediStockId = rowData.ID;
                //    ChangedDataRow(rowData);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMediStockCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMediStockName.Focus();
                    txtMediStockName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMediStockName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepartMentName.Focus();
                    cboDepartMentName.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartMentName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDepartMentName.EditValue != null && cboDepartMentName.EditValue != cboDepartMentName.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_DEPARTMENT gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartMentName.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtHeadCode.Focus();
                            txtNotHeadCode.SelectAll();
                        }
                    }
                    else
                    {
                        txtHeadCode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartMentName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDepartMentName.EditValue != null)
                    {
                        txtHeadCode.Focus();
                    }
                    else
                    {
                        cboDepartMentName.ShowPopup();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboParent.EditValue != null && cboParent.EditValue != cboParent.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_MEDI_STOCK gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboParent.EditValue.ToString()));
                        if (gt != null || cboParent.EditValue != null)
                        {
                            cboParent.Properties.Buttons[1].Visible = true;
                            chkIsAllowImpSupplier.Focus();
                        }
                    }
                    else
                    {
                        chkIsAllowImpSupplier.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboParent.EditValue != null)
                    {
                        chkIsAllowImpSupplier.Properties.FullFocusRect = true;
                        chkIsAllowImpSupplier.Focus();
                    }
                    else
                    {
                        cboParent.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAllowImpSupplier_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkNotAllowMedicine.Properties.FullFocusRect = true;
                    chkNotAllowMedicine.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsBusiness_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsExpend.Properties.FullFocusRect = true;
                    chkIsExpend.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAutoExecuteExp_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsCabinet.Properties.FullFocusRect = true;
                    chkIsCabinet.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsCabinet_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsNewMedicine.Properties.FullFocusRect = true;
                    chkIsNewMedicine.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAutoCreateChmsImp_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsAutoCreateReImp.Properties.FullFocusRect = true;
                    chkIsAutoCreateReImp.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAutoApproveImp_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsGoodsRestrict.Properties.FullFocusRect = true;
                    chkIsGoodsRestrict.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsGoodsRestrict_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsShowInPatientReturnPres.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeadCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNotHeadCode.Focus();
                    txtNotHeadCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNotHeadCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboParent.Focus();
                    cboParent.EditValue = null;
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

        private void cboParent_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void chkOdd_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsBlood.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsNewMedicine_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsTraditionalMedicine.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsTraditionalMedicine_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsDrugstore.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsDrugstore_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkShowDDT.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsBlood_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsNewMedicine.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestTypeApprove_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE rv in gridCheckMark.Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.EXP_MEST_TYPE_NAME.ToString());
                    }
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestTypeExp_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                if (gridCheckMark.Selection != null && gridCheckMark.Selection.Count > 0)
                {
                    foreach (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE rv in gridCheckMark.Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.EXP_MEST_TYPE_NAME.ToString());
                        }
                    }
                    e.DisplayText = sb.ToString();
                }
                else
                {
                    e.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboImpMestTypeApprove_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE rv in gridCheckMark.Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.IMP_MEST_TYPE_NAME.ToString());
                    }
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboImpMestTypeImp_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE rv in gridCheckMark.Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.IMP_MEST_TYPE_NAME.ToString());
                    }
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkShowDDT_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsUsingPlanningTransDF.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListDataStore_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraTreeList.TreeListHitInfo hi = treeListDataStore.CalcHitInfo(e.Location);
                if (hi.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Cell)
                {
                    currentData = null;
                    var data = treeListDataStore.GetDataRecordByNode(hi.Node);
                    V_HIS_MEDI_STOCK rowData = data as V_HIS_MEDI_STOCK;
                    if (rowData != null)
                    {
                        currentData = rowData;
                        roomId = rowData.ROOM_ID;
                        mediStockId = rowData.ID;
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArg = new List<object>();

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisImportMediStock", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArg);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsUsingPlanningTransDF_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsAutoCreateChmsImp.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsCabinet_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                cboCabinetManageOption.Enabled = chkIsCabinet.Checked;
                if (chkIsCabinet.Checked)
                {
                    if (currentData != null && currentData.CABINET_MANAGE_OPTION.HasValue)
                    {
                        cboCabinetManageOption.SelectedIndex = (currentData.CABINET_MANAGE_OPTION.Value - 1);
                    }
                    else
                    {
                        cboCabinetManageOption.SelectedIndex = 0;
                    }
                }
                else
                {
                    cboCabinetManageOption.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsShowInPatientReturnPres_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIS_MOBA_CHANGE_AMOUNT.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIS_MOBA_CHANGE_AMOUNT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIS_FOR_REJECTED_MOBA.Focus();
                    chkIS_FOR_REJECTED_MOBA.SelectAll();
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIS_FOR_REJECTED_MOBA_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsShowAnticipate.Focus();
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhanLoaiBenhNhan_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null)
                {
                    e.DisplayText = "";
                    return;
                }
                else
                {
                    foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY rv in gridCheckMark.Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.PATIENT_CLASSIFY_NAME.ToString());
                        }
                    }
                    e.DisplayText = sb.ToString();
                }


                //e.DisplayText = "";
                //string phanloai = "";
                //if (ListHisPatientClassify != null && ListHisPatientClassify.Count > 0)
                //{
                //    foreach (var item in ListHisPatientClassify)
                //    {
                //        phanloai += item.PATIENT_CLASSIFY_NAME + ", ";
                //    }
                //}
                //e.DisplayText = phanloai;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhanLoaiBenhNhan_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCabinetManageOption.Enabled == true)
                    {
                        cboCabinetManageOption.Focus();
                        cboCabinetManageOption.SelectAll();
                    }
                    else
                    {
                        SaveProcess();
                        FillDataToControlsForm();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCabinetManageOption_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SaveProcess();
                    FillDataToControlsForm();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhanLoaiBenhNhan_Closed(object sender, ClosedEventArgs e)
        {
            if (ListHisPatientClassify != null && ListHisPatientClassify.Count > 0)
            {

            }
            else
            {
                cboPhanLoaiBenhNhan.EditValue = null;
                cboPhanLoaiBenhNhan.Text = "";
            }
        }

        private void chkIsTraditionalMedicine_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkIsDrugstore_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsDrugstore.Checked)
                {
                    isEnableShowDrugStore = true;
                    chkIsShowDrugStore.Properties.Appearance.ForeColor = Color.Black;
                }
                else
                {
                    isEnableShowDrugStore = false;
                    chkIsShowDrugStore.Checked = false;
                    chkIsShowDrugStore.Properties.Appearance.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsShowDrugStore_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!isEnableShowDrugStore)
                {
                    chkIsShowDrugStore.Checked = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsExpend_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkShowDDT.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAutoCreateReImp_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsGoodsRestrict.Properties.FullFocusRect = true;
                    chkIsGoodsRestrict.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkNotAllowMedicine_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkNotAllowMaterial.Properties.FullFocusRect = true;
                    chkNotAllowMaterial.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkNotAllowMaterial_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkOdd.Properties.FullFocusRect = true;
                    chkOdd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsShowDrugStore_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsBusiness.Properties.FullFocusRect = true;
                    chkIsBusiness.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsShowAnticipate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPhanLoaiBenhNhan.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void setInfoQR(string value)
        {
            try
            {
                txtTLQR.Text = value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnEditTTQR_Click(object sender, EventArgs e)
        {
            try
            {
                //frmSetTTQR mediStock = new frmSetTTQR(setInfoQR);
                //mediStock.ShowDialog();

                Rectangle buttonPosition = new Rectangle(btnSave.Bounds.X, btnSave.Bounds.Y, btnSave.Bounds.Width, btnSave.Bounds.Height);
                popupControlContainerTLQR.ShowPopup(new Point(buttonPosition.X + 480, buttonPosition.Bottom + btnSave.Bounds.Height + 400));
                GetConFig();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
           
        }
        private void GetConFig()
        {
            try
            {
                lstKey.Clear();
                lstValueQR.Clear();
                CommonParam param = new CommonParam();
                lstConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();
                if (lstConfig != null && lstConfig.Count > 0)
                {
                    int count = 0;
                    foreach (var item in lstConfig)
                    {
                        HIS_CONFIG cf = new HIS_CONFIG();
                        cf.ID = count++;


                        string value = item.KEY;
                        int index = value.IndexOf("Info");
                        if (index > 0)
                        {
                            var shotkey = value.Substring(0, index);
                            string[] parts = shotkey.Split('.');
                            if (parts.Length > 0)
                            {
                                cf.KEY = parts[parts.Length - 1]; // Lấy phần cuối cùng sau khi tách
                            }
                        }
                        cf.VALUE = item.VALUE;
                        lstKey.Add(cf);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToGrid()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KEY", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("KEY", "KEY", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(repositoryItemCboBank, lstKey, controlEditorADO);

                DataTable table = new DataTable();
                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("Value", typeof(object));

                // Thêm dữ liệu cho 2 dòng "Ngân hàng" và "Cấu hình"
                table.Rows.Add("Ngân hàng", null);
                table.Rows.Add("Giá trị cấu hình", "");

                gridControlSetTTQR.DataSource = table;

                if (!string.IsNullOrEmpty(txtTLQR.Text))
                {
                    HIS.Desktop.Plugins.HisMediStock.ADO.HisMediStockADO.ItemConfig config = Newtonsoft.Json.JsonConvert.DeserializeObject<HIS.Desktop.Plugins.HisMediStock.ADO.HisMediStockADO.ItemConfig>(txtTLQR.Text);
                    if (config.BANK != null && config.VALUE != null)
                    {
                        gridViewSetTTQR.SetRowCellValue(0, "Value", config.BANK);
                        gridViewSetTTQR.SetRowCellValue(1, "Value", config.VALUE);
                        if (!string.IsNullOrEmpty(config.BANK))
                        {
                            repositoryItemCboBank.Properties.Buttons[1].Visible = true;
                        }
                        KEY = config.BANK;
                        VALUE = config.VALUE;
                    }
                    else if (config.BANK != null)
                    {
                        var cbocf = lstKey.FirstOrDefault(s => s.KEY == config.BANK);
                        if (cbocf != null)
                        {
                            gridViewSetTTQR.SetRowCellValue(0, "Value", cbocf.KEY);
                            gridViewSetTTQR.SetRowCellValue(1, "Value", cbocf.VALUE);
                            if (!string.IsNullOrEmpty(cbocf.KEY))
                            {
                                repositoryItemCboBank.Properties.Buttons[1].Visible = true;
                            }
                        }
                        KEY = cbocf.KEY;
                        VALUE = cbocf.VALUE;
                    }
                }

                gridViewSetTTQR.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None;
                gridViewSetTTQR.OptionsBehavior.Editable = true;
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public string KEY { get; set; }
        public string VALUE { get; set; }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.KEY))
                {
                    var dataRow = gridViewSetTTQR.GetDataRow(1);
                    var result = new
                    {
                        BANK = this.KEY,
                        VALUE = dataRow[1]
                    };

                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                    txtTLQR.Text = jsonString.ToString();
                }
                PopupContainerBarControl control = popupControlContainerTLQR.Parent as PopupContainerBarControl;
                control.ClosePopup();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridViewSetTTQR_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                try
                {
                    if (e.RowHandle == 0 && e.Column.FieldName == "Value")
                    {
                        e.RepositoryItem = repositoryItemCboBank;
                    }
                    if (e.RowHandle == 1 && e.Column.FieldName == "Value")
                    {
                        e.RepositoryItem = txtConfigValue;
                    }

                }
                catch (Exception ex)
                {

                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCboBank_Closed_1(object sender, ClosedEventArgs e)
        {
            try
            {
                GridLookUpEdit grid = gridViewSetTTQR.ActiveEditor as GridLookUpEdit;

                if (grid != null)
                {
                    var selectedBank = grid.EditValue.ToString();
                    KEY = selectedBank;
                    if (!string.IsNullOrEmpty(selectedBank))
                    {
                        repositoryItemCboBank.Properties.Buttons[1].Visible = true;
                        // Tìm cấu hình tương ứng trong listConfig
                        var config = lstKey.FirstOrDefault(c => c.KEY.Contains(selectedBank));

                        if (config != null)
                        {
                            // Cập nhật giá trị cấu hình vào ô "Cấu hình"
                            gridViewSetTTQR.SetRowCellValue(1, "Value", config.VALUE);
                            VALUE = config.VALUE;
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCboBank_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    repositoryItemCboBank.Properties.Buttons[1].Visible = false;
                    gridViewSetTTQR.SetRowCellValue(0, "Value", "");
                    gridViewSetTTQR.SetRowCellValue(1, "Value", "");
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTLQR_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtTLQR.Properties.Buttons[0].Visible = false;
                    txtTLQR.Text = null;
                }
            }
             catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTLQR_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtTLQR.Text))
                    txtTLQR.Properties.Buttons[0].Visible = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRooomTN_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                 if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRooomTN.Properties.Buttons[1].Visible = false;
                    cboRooomTN.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRooomTN_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRooomTN.EditValue != null)
                    cboRooomTN.Properties.Buttons[1].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
