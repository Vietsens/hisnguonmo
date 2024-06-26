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
using AutoMapper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using MOS.SDO;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.HisCareSum.Resources;
using DevExpress.Utils.Menu;
using SDA.EFMODEL.DataModels;
using HIS.Desktop.Plugins.HisCareSum.ADO;
using HIS.Desktop.Plugins.HisCareSum.Config;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.HisCareSum
{
    public partial class frmHisCareSum : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        List<HIS_CARE> lstCare;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_CARE_SUM currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS_CARE_SUM printCareSum;
        //V_HIS_TREATMENT currentHisTreatment;
        V_HIS_TREATMENT currentTreatment;
        List<HIS_AWARENESS> lstHisAwareness = new List<HIS_AWARENESS>();
        List<HIS_CARE_TYPE> lstHisCareType = new List<HIS_CARE_TYPE>();
        List<HIS_CARE> listCares;
        long treatmentId = 0;
        List<HIS_ICD> hisICD = new List<HIS_ICD>();
        //List<MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO> lstCareViewPrintADO;
        //List<MPS.Processor.Mps000151.PDO.Mps000151PDO.CareDetailViewPrintADO> lstCareDetailViewPrintADO;
        List<HIS_CARE> lstHisCareByTreatment;
        HIS.UC.Icd.IcdProcessor icdProcessor = null;
        //internal List<MPS.Processor.Mps000151.PDO.CreatorADO> _CreatorADOs = null;
        UserControl ucIcd = null;
        private bool IsTreatmentList;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.HisCareSum";
        SDA_CONFIG_APP _currentConfigApp;
        SDA_CONFIG_APP_USER currentConfigAppUser;
        ConfigADO _ConfigADO;
        HisTreatmentBedRoomLViewFilter DataTransferTreatmentBedRoomFilter { get; set; }
        #endregion

        #region Construct
        public frmHisCareSum(Inventec.Desktop.Common.Modules.Module moduleData, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                try
                {
                    pagingGrid = new PagingGrid();
                    this.moduleData = moduleData;
                    gridControlFormList.ToolTipController = toolTipControllerGrid;

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

        public frmHisCareSum(Inventec.Desktop.Common.Modules.Module moduleData, long treatmentId, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                try
                {
                    pagingGrid = new PagingGrid();
                    this.moduleData = moduleData;
                    this.treatmentId = treatmentId;
                    gridControlFormList.ToolTipController = toolTipControllerGrid;

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

        public frmHisCareSum(Inventec.Desktop.Common.Modules.Module moduleData, long treatmentId, bool IsTreatmentList, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                try
                {
                    this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
                    pagingGrid = new PagingGrid();
                    this.moduleData = moduleData;
                    this.treatmentId = treatmentId;
                    this.IsTreatmentList = IsTreatmentList;
                    gridControlFormList.ToolTipController = toolTipControllerGrid;

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
        private void frmHisCareSum_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                LoaddataToComboChuanDoanTD();
                MeShow();
                InitMenuToButtonPrint();

                if (currentTreatment.IS_PAUSE == 1 || IsTreatmentList)
                {
                    if (HisConfigCFG.AllowUpdatingAfterLockingTreatment == "1")
                    {
                        btnEdit.Enabled = false;
                        btnAdd.Enabled = true;
                        btnRefresh.Enabled = true;
                    }
                    else
                    {
                        btnEdit.Enabled = false;
                        btnAdd.Enabled = false;
                        btnRefresh.Enabled = false;
                    }
                }
                LoadConfigHisAcc();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisCareSum.Resources.Lang", typeof(HIS.Desktop.Plugins.HisCareSum.frmHisCareSum).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNumOrder.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColNumOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNumOrder.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColNumOrder.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdText.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColIcdText.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIcdText.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColIcdText.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisCareSum.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboIcds.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisCareSum.cboIcds.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkIcds.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisCareSum.chkIcds.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisCareSum.txtIcdText.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciIcdMainText.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.lciIcdMainText.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.lciIcdText.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.Text = Inventec.Common.Resource.Get.Value("frmHisCareSum.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                dtFrom.EditValue = DateTime.Now;
                dtTo.EditValue = DateTime.Now;
                btnAdd.Enabled = true;
                btnEdit.Enabled = false;
                txtIcdText.Text = "";
                txtThuTu.EditValue = null;
                txtSubCode.Text = "";
                //HIS.UC.Icd.ADO.IcdInputADO inputAdo = new UC.Icd.ADO.IcdInputADO();
                if (icdProcessor != null && ucIcd != null)
                {
                    icdProcessor.Reload(ucIcd, null);
                    icdProcessor.FocusControl(ucIcd);
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
        private void SetDefaultFocus()
        {
            try
            {
                dtFrom.EditValue = DateTime.Now;
                dtTo.EditValue = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        //private void InitTabIndex()
        //{
        //    try
        //    {
        //        dicOrderTabIndexControl.Add("lkTreatmentId", 0);
        //        dicOrderTabIndexControl.Add("spNumOrder", 1);
        //        dicOrderTabIndexControl.Add("lkIcdId", 2);
        //        dicOrderTabIndexControl.Add("txtIcdText", 3);
        //        dicOrderTabIndexControl.Add("txtIcdMainText", 4);
        //        dicOrderTabIndexControl.Add("txtIcdSubCode", 5);


        //        if (dicOrderTabIndexControl != null)
        //        {
        //            foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
        //            {
        //                SetTabIndexToControl(itemOrderTab, lcEditorInfo);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        //{
        //    bool success = false;
        //    try
        //    {
        //        if (!layoutControlEditor.IsInitialized) return success;
        //        layoutControlEditor.BeginUpdate();
        //        try
        //        {
        //            foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
        //            {
        //                DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
        //                if (lci != null && lci.Control != null)
        //                {
        //                    BaseEdit be = lci.Control as BaseEdit;
        //                    if (be != null)
        //                    {
        //                        //Cac control dac biet can fix khong co thay doi thuoc tinh enable
        //                        if (itemOrderTab.Key.Contains(be.Name))
        //                        {
        //                            be.TabIndex = itemOrderTab.Value;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        finally
        //        {
        //            layoutControlEditor.EndUpdate();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //    return success;
        //}

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                LoadPaging();
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
        private void LoadPaging()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                List<MOS.EFMODEL.DataModels.HIS_CARE_SUM> apiResult = null;
                HisCareSumFilter filter = new HisCareSumFilter();
                filter.TREATMENT_ID = treatmentId;
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_CARE_SUM>>(HisRequestUriStore.MOSV_HIS_CARE_SUM_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    dnNavigation.DataSource = apiResult;
                    gridviewFormList.GridControl.DataSource = apiResult;
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

        private void SetFilterNavBar(ref HisCareSumFilter filter)
        {
            try
            {
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_CARE_SUM)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_CARE_SUM)gridviewFormList.GetFocusedRow();
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
                    MOS.EFMODEL.DataModels.HIS_CARE_SUM pData = (MOS.EFMODEL.DataModels.HIS_CARE_SUM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "ICD_MAIN")
                    {
                        e.Value = pData.ICD_NAME;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }

                    gridControlFormList.RefreshDataSource();
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_CARE_SUM)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    printCareSum = rowData;
                    ChangedDataRow(rowData);
                    FillDataToGridControlCare();
                    //Set focus vào control editor đầu tiên
                    //SetFocusEditor();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_CARE_SUM)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        currentData = rowData;
                        ChangedDataRow(rowData);
                        FillDataToGridControlCare();
                        //Set focus vào control editor đầu tiên
                        //SetFocusEditor();
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
                this.currentData = (MOS.EFMODEL.DataModels.HIS_CARE_SUM)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.HIS_CARE_SUM>)[dnNavigation.Position];
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_CARE_SUM data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    btnAdd.Enabled = false;
                    btnEdit.Enabled = true;

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);

                    if (currentTreatment.IS_PAUSE == 1 || IsTreatmentList)
                    {
                        if (HisConfigCFG.AllowUpdatingAfterLockingTreatment == "1")
                        {
                            btnEdit.Enabled = true;
                            btnAdd.Enabled = false;
                            btnRefresh.Enabled = true;
                        }
                        else
                        {
                            btnEdit.Enabled = false;
                            btnAdd.Enabled = false;
                            btnRefresh.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_CARE_SUM data)
        {
            try
            {
                HIS.UC.Icd.ADO.IcdInputADO inputAdo = new UC.Icd.ADO.IcdInputADO();
                inputAdo.ICD_NAME = data.ICD_NAME;
                inputAdo.ICD_CODE = data.ICD_CODE;
                txtIcdText.Text = data.ICD_TEXT;
                if (data.NUM_ORDER == null)
                {
                    txtThuTu.EditValue = null;
                }
                else
                {
                    txtThuTu.EditValue = data.NUM_ORDER;
                }
                txtSubCode.Text = data.ICD_SUB_CODE;

                if (icdProcessor != null && ucIcd != null)
                {
                    icdProcessor.Reload(ucIcd, inputAdo);
                }
                labelCode.Text = currentTreatment.TDL_PATIENT_CODE;
                labelName.Text = currentTreatment.TDL_PATIENT_NAME;
                labelGender.Text = currentTreatment.TDL_PATIENT_GENDER_NAME;
                if (currentTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                {
                    labelDOB.Text = currentTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                }
                else
                {
                    labelDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentTreatment.TDL_PATIENT_DOB).ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadInfoLoadForm()
        {
            try
            {
                HIS.UC.Icd.ADO.IcdInputADO inputAdo = new UC.Icd.ADO.IcdInputADO();
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = treatmentId;
                currentTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                if (currentTreatment != null)
                {
                    txtIcdText.Text = currentTreatment.ICD_TEXT;
                    inputAdo.ICD_CODE = currentTreatment.ICD_CODE;
                    txtSubCode.Text = currentTreatment.ICD_SUB_CODE;
                    inputAdo.ICD_NAME = currentTreatment.ICD_NAME;
                    labelCode.Text = currentTreatment.TDL_PATIENT_CODE;
                    labelName.Text = currentTreatment.TDL_PATIENT_NAME;
                    labelGender.Text = currentTreatment.TDL_PATIENT_GENDER_NAME;
                    if (currentTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        labelDOB.Text = currentTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        labelDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentTreatment.TDL_PATIENT_DOB).ToString();
                    }
                }

                if (icdProcessor != null && ucIcd != null)
                {
                    icdProcessor.Reload(ucIcd, inputAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                txtSubCode.Text = "";
                txtIcdText.Text = "";
                txtThuTu.EditValue = null;
                if (icdProcessor != null && ucIcd != null)
                {
                    icdProcessor.Reload(ucIcd, null);
                    icdProcessor.FocusControl(ucIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_CARE_SUM currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisCareSumFilter filter = new HisCareSumFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CARE_SUM>>(HisRequestUriStore.MOSV_HIS_CARE_SUM_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                var listIcd = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.Width = 424;
                ado.Height = 24;
                ado.IsColor = true;
                ado.DataIcds = listIcd;
                ado.AutoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.Desktop.Plugins.AutoCheckIcd") == 1;
                ucIcd = (UserControl)icdProcessor.Run(ado);

                if (ucIcd != null)
                {
                    this.layoutControlICd.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void NextForcusSubIcd()
        {
            try
            {
                txtSubCode.Focus();
                txtSubCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Print_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                printCareSum = null;
                printCareSum = new HIS_CARE_SUM();
                printCareSum = (HIS_CARE_SUM)gridviewFormList.GetFocusedRow();
                onClickPrintCare();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Create_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                var row = (HIS_CARE_SUM)gridviewFormList.GetFocusedRow();

                if (row != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    if (DataTransferTreatmentBedRoomFilter != null)
                        listArgs.Add(DataTransferTreatmentBedRoomFilter);
                    CallModule callModule = new CallModule(CallModule.CareCreate, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    WaitingManager.Hide();
                }
                FillDataToGridControlCare();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                //btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                //btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
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
                FillDataToGridControlCare();
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_CARE_SUM)gridviewFormList.GetFocusedRow();

                if (rowData != null)
                {
                    if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSV_HIS_CARE_SUM_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            gridControlCare.DataSource = null;
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
                if (!bbtnReset.Enabled) return;
                SetDefaultValue();
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

        private void txtThuTu_KeyDown(object sender, KeyEventArgs e)
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
                if (icdProcessor != null && ucIcd != null)
                {
                    var icdValidation = (bool)icdProcessor.ValidationIcd(ucIcd);
                    if (!icdValidation)
                        return;
                }
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_CARE_SUM updateDTO = new MOS.EFMODEL.DataModels.HIS_CARE_SUM();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    if (this.ActionType == GlobalVariables.ActionEdit)
                    {
                        LoadCurrent(this.currentData.ID, ref updateDTO);
                    }
                }

                UpdateDTOFromDataForm(ref updateDTO);
                WaitingManager.Hide();
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CARE_SUM>(HisRequestUriStore.MOSV_HIS_CARE_SUM_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_CARE_SUM>(HisRequestUriStore.MOSV_HIS_CARE_SUM_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        //UpdateRowDataAfterEdit(resultData);
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_CARE_SUM data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_CARE_SUM) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_CARE_SUM)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_CARE_SUM>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_CARE_SUM currentDTO)
        {
            try
            {
                currentDTO.TREATMENT_ID = treatmentId;

                if (icdProcessor != null && ucIcd != null)
                {
                    var icd = (HIS.UC.Icd.ADO.IcdInputADO)icdProcessor.GetValue(ucIcd);
                    if (icd != null)
                    {
                        currentDTO.ICD_NAME = icd.ICD_NAME;
                        currentDTO.ICD_CODE = icd.ICD_CODE;
                    }
                }

                currentDTO.ICD_TEXT = txtIcdText.Text;
                if (txtThuTu.Value == 0)
                {
                    currentDTO.NUM_ORDER = null;
                }
                else
                {
                    currentDTO.NUM_ORDER = (long)txtThuTu.Value;
                }

                currentDTO.ICD_SUB_CODE = txtSubCode.Text;
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
                ValidationSingleControl1();
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

        private void ValidationSingleControl1()
        {
            try
            {
                ValidatetxtThuTu validate = new ValidatetxtThuTu();
                validate.txtThuTu = txtThuTu;
                //validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                validate.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(txtThuTu, validate);
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

                LoadInfoLoadForm();

                //Load du lieu
                FillDataToGridControl();
                //FillDataToGridControlCare();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

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
                dtFrom.EditValue = DateTime.Now;
                dtTo.EditValue = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Print
        void onClickPrintCare()
        {
            try
            {
                PrintTypeCare type = new PrintTypeCare();
                type = PrintTypeCare.IN_KET_QUA_CHAM_SOC_TONG_HOP;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal enum PrintTypeCare
        {
            IN_KET_QUA_CHAM_SOC_TONG_HOP,
            IN_PHIEU_CHAM_SOC_QY7,
            IN_KET_QUA_CHAM_SOC_CAP_I,
        }

        void PrintProcess(PrintTypeCare printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeCare.IN_KET_QUA_CHAM_SOC_TONG_HOP:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuChamSocTongHop_MPS000151, DelegateRunPrinterCare);
                        break;
                    case PrintTypeCare.IN_PHIEU_CHAM_SOC_QY7:
                        richEditorMain.RunPrintTemplate("Mps000229", DelegateRunPrinterCare);
                        break;
                    case PrintTypeCare.IN_KET_QUA_CHAM_SOC_CAP_I:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuChamSocCapI_MPS000427, DelegateRunPrinterCare);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinterCare(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuChamSocTongHop_MPS000151:
                        LoadBieuMauPhieuYCKetQuaChamSocTongHop(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000229":
                        LoadBieuMauPhieuQy7(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuChamSocCapI_MPS000427:
                        LoadBieuMauPhieuYCKetQuaChamSocCapI(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadBieuMauPhieuYCKetQuaChamSocCapI(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                listCares = new List<HIS_CARE>();
                int[] selectRows = gridViewCare.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        listCares.Add((HIS_CARE)gridViewCare.GetRow(selectRows[i]));
                    }
                }

                if (listCares == null || listCares.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dữ liệu in", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                HIS_TREATMENT Treatment = new HIS_TREATMENT();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatmentId;
                Treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                if (listCares != null && listCares.Count > 0)
                {
                    foreach (var item in listCares)
                    {
                        WaitingManager.Show();

                        CommonParam paramGet = new CommonParam();
                        MOS.Filter.HisDhstFilter hisDHSTFilter = new HisDhstFilter();
                        hisDHSTFilter.CARE_ID = item.ID;
                        List<MOS.EFMODEL.DataModels.HIS_DHST> hisDHST = new BackendAdapter(paramGet).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, hisDHSTFilter, paramGet);

                        var chartControl1 = ChartDhstProcess.GenerateChartImage(LoadListChartADO(hisDHST));
                        var chartControl2 = ChartDhstProcess.GenerateChartImageAll(LoadListChartADO(hisDHST));

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), printTypeCode, this.moduleData.RoomId);


                        long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY__HIS_DESKTOP_PLUGINS_CARE_IS_PRINT_MERGE));
                        if (keyPrintMerge == 1)
                        {
                            string uniqueTime = "";// (_TrackingPrints != null && _TrackingPrints.Count > 0) ? _TrackingPrints[0].TRACKING_TIME + "" : "";
                            inputADO.MergeCode = String.Format("{0}_{1}_{2}", printTypeCode, uniqueTime, (Treatment != null ? Treatment.TREATMENT_CODE : ""));
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.MergeCode), inputADO.MergeCode));


                        MPS.Processor.Mps000427.PDO.Mps000427PDO mps000427RDO = new MPS.Processor.Mps000427.PDO.Mps000427PDO(
                           item,
                           Treatment,
                           hisDHST,
                           ChartDhstProcess.GetChartImage(chartControl1, 0),
                           ChartDhstProcess.GetChartImage(chartControl1, 1),
                           ChartDhstProcess.GetChartImageAll(chartControl2)
                           );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (chkSign.Checked)
                        {
                            if (chkPrintDocumentSigned.Checked)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000427RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, "") { EmrInputADO = inputADO };
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000427RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000427RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000427RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                            }
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private List<ChartADO> LoadListChartADO(List<HIS_DHST> lstCurrentDhst)
        {
            List<ChartADO> lstChartAdo = new List<ChartADO>();

            List<HIS_DHST> lst = lstCurrentDhst.OrderByDescending(o => o.EXECUTE_TIME).ToList();
            foreach (var dhst in lst)
            {
                ChartADO chartAdo = new ChartADO();
                chartAdo.Date = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dhst.EXECUTE_TIME.ToString());
                chartAdo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dhst.EXECUTE_TIME ?? 0);
                chartAdo.PULSE = dhst.PULSE;
                chartAdo.TEMPERATURE = dhst.TEMPERATURE;
                lstChartAdo.Add(chartAdo);
            }
            return lstChartAdo;
        }

        private void LoadBieuMauPhieuYCKetQuaChamSocTongHop(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                //this._CreatorADOs = new List<MPS.Processor.Mps000151.PDO.CreatorADO>();
                listCares = new List<HIS_CARE>();
                int[] selectRows = gridViewCare.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        listCares.Add((HIS_CARE)gridViewCare.GetRow(selectRows[i]));
                    }
                }

                if (listCares == null || listCares.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dữ liệu in", "Thông báo");
                    return;
                }
                WaitingManager.Show();
                var currentPatient = HIS.Desktop.Print.PrintGlobalStore.getPatient(treatmentId);
                //var departmentTran = HIS.Desktop.Print.PrintGlobalStore.getDepartmentTran(treatmentId);
                //currentHisTreatment = PrintGlobalStore.getTreatment(treatmentId);

                LoadDataToRadioGroupAwareness(ref lstHisAwareness);
                //LoadDataToGridControlCareDetail(ref lstHisCareType);
                //AddCareData(treatmentId);
                //
                //HIS_ICD icd = new HIS_ICD();
                //if (currentHisTreatment != null)
                //{
                //    icd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ICD_CODE == currentHisTreatment.ICD_CODE).FirstOrDefault();
                //}

                MPS.Processor.Mps000151.PDO.Mps000151PDO.Mps000151ADO mps000151ADO = new MPS.Processor.Mps000151.PDO.Mps000151PDO.Mps000151ADO();
                if (printCareSum != null)
                {
                    mps000151ADO.ICD_TEXT = printCareSum.ICD_TEXT;
                    mps000151ADO.ICD_NAME = printCareSum.ICD_NAME;
                    mps000151ADO.ICD_CODE = printCareSum.ICD_CODE;
                    mps000151ADO.ICD_SUB_CODE = printCareSum.ICD_SUB_CODE;
                    //mps000151ADO.ICD_ID = printCareSum.ICD_ID;
                }

                MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                bedFilter.TREATMENT_ID = treatmentId;
                bedFilter.ORDER_FIELD = "CREATE_TIME";
                bedFilter.ORDER_DIRECTION = "DESC";
                var _TreatmentBedRoom = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, bedFilter, null);
                if (_TreatmentBedRoom != null && _TreatmentBedRoom.Count > 0)
                {
                    var bed = _TreatmentBedRoom.Where(o => o.BED_ID.HasValue).OrderByDescending(o => o.OUT_TIME.HasValue).FirstOrDefault();
                    if (bed != null)
                    {
                        var lastBed = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED>().FirstOrDefault(o => o.ID == bed.BED_ID);
                        mps000151ADO.BED_CODE = lastBed != null ? lastBed.BED_CODE : "";
                        mps000151ADO.BED_NAME = lastBed != null ? lastBed.BED_NAME : "";
                    }
                    else
                    {
                        mps000151ADO.BED_CODE = _TreatmentBedRoom.FirstOrDefault().BED_CODE;
                        mps000151ADO.BED_NAME = _TreatmentBedRoom.FirstOrDefault().BED_NAME;
                    }
                }

                lstHisCareByTreatment = new List<HIS_CARE>();
                lstHisCareByTreatment = listCares.OrderBy(o => o.EXECUTE_TIME).ToList();

                CommonParam paramGet = new CommonParam();
                MOS.Filter.HisDhstFilter hisDHSTFilter = new HisDhstFilter();
                hisDHSTFilter.CARE_IDs = listCares.Select(o => o.ID).ToList();
                List<MOS.EFMODEL.DataModels.HIS_DHST> hisDHST = new BackendAdapter(paramGet).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, hisDHSTFilter, paramGet);

                mps000151ADO.DEPARTMENT_NAME = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.moduleData.RoomId).DepartmentName;
                mps000151ADO.ROOM_NAME = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.moduleData.RoomId).RoomName;
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_TreatmentBedRoom != null ? _TreatmentBedRoom.FirstOrDefault().TREATMENT_CODE : ""), printTypeCode);


                long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY__HIS_DESKTOP_PLUGINS_CARE_IS_PRINT_MERGE));
                if (keyPrintMerge == 1)
                {
                    string uniqueTime = "";// (_TrackingPrints != null && _TrackingPrints.Count > 0) ? _TrackingPrints[0].TRACKING_TIME + "" : "";
                    inputADO.MergeCode = String.Format("{0}_{1}_{2}", printTypeCode, uniqueTime, (currentTreatment != null ? currentTreatment.TREATMENT_CODE : ""));
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.MergeCode), inputADO.MergeCode));


                MPS.Processor.Mps000151.PDO.Mps000151PDO mps000151RDO = new MPS.Processor.Mps000151.PDO.Mps000151PDO(
                    currentPatient,
                    mps000151ADO,
                    lstHisCareByTreatment,
                    hisDHST,
                    BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>(),
                    LoadDataCareDetailView(lstHisCareByTreatment),
                    BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE).GENDER_CODE,
                    BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE).GENDER_CODE
                    //this._CreatorADOs
                    );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (chkSign.Checked)
                {
                    if (chkPrintDocumentSigned.Checked)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000151RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000151RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000151RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000151RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        internal void LoadDataToRadioGroupAwareness(ref List<MOS.EFMODEL.DataModels.HIS_AWARENESS> lstHisAwareness)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisAwarenessFilter hisAwarenessFilter = new MOS.Filter.HisAwarenessFilter();
                lstHisAwareness = new BackendAdapter(param).Get<List<HIS_AWARENESS>>(ApiConsumer.HisRequestUriStore.HIS_AWARENESS_GET, ApiConsumers.MosConsumer, hisAwarenessFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadDataToGridControlCareDetail(ref List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE> lstHisCareType)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisCareTypeFilter hisCareTypeFilter = new MOS.Filter.HisCareTypeFilter();
                lstHisCareType = new BackendAdapter(param).Get<List<HIS_CARE_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_CARE_TYPE_GET, ApiConsumers.MosConsumer, hisCareTypeFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_CARE_DETAIL> LoadDataCareDetailView(List<HIS_CARE> hisCare)
        {
            List<V_HIS_CARE_DETAIL> result = new List<V_HIS_CARE_DETAIL>();
            try
            {
                if (hisCare != null && hisCare.Count > 0)
                {
                    foreach (var item in hisCare)
                    {
                        CommonParam paramGet = new CommonParam();
                        MOS.Filter.HisCareDetailViewFilter hisCareDetailFilter = new MOS.Filter.HisCareDetailViewFilter();
                        hisCareDetailFilter.CARE_ID = item.ID;

                        List<V_HIS_CARE_DETAIL> lstHisCareDetail = new BackendAdapter(paramGet).Get<List<V_HIS_CARE_DETAIL>>(ApiConsumer.HisRequestUriStore.HIS_CARE_DETAIL_GETVIEW, ApiConsumers.MosConsumer, hisCareDetailFilter, paramGet);
                        if (lstHisCareDetail != null)
                        {
                            result.AddRange(lstHisCareDetail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_CARE_DETAIL>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //private void AddCareData(long currentTreatmentId)
        //{
        //    try
        //    {
        //        if (currentTreatmentId > 0)
        //        {
        //            WaitingManager.Show();
        //            this.lstCareViewPrintADO = new List<MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO>();
        //            this.lstCareDetailViewPrintADO = new List<MPS.Processor.Mps000151.PDO.Mps000151PDO.CareDetailViewPrintADO>();

        //            #region ------
        //            for (int i = 0; i < 18; i++)
        //            {
        //                MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO careViewPrintSDO = new MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO();
        //                switch (i)
        //                {
        //                    case 0:
        //                        careViewPrintSDO.CARE_TITLE1 = "Ngày tháng";
        //                        break;
        //                    case 1:
        //                        careViewPrintSDO.CARE_TITLE1 = "Giờ";
        //                        break;
        //                    case 2:
        //                        careViewPrintSDO.CARE_TITLE1 = "Ý thức";
        //                        break;
        //                    case 3:
        //                        careViewPrintSDO.CARE_TITLE1 = "Da, niêm mạc";
        //                        break;
        //                    case 4:
        //                        careViewPrintSDO.CARE_TITLE2 = "Mạch (lần/phút)";
        //                        careViewPrintSDO.CARE_TITLE1 = "Dấu hiệu sinh tồn";
        //                        break;
        //                    case 5:
        //                        careViewPrintSDO.CARE_TITLE1 = "Dấu hiệu sinh tồn";
        //                        careViewPrintSDO.CARE_TITLE2 = "Nhiệt độ (độ C)";
        //                        break;
        //                    case 6:
        //                        careViewPrintSDO.CARE_TITLE1 = "Dấu hiệu sinh tồn";
        //                        careViewPrintSDO.CARE_TITLE2 = "Huyết áp (mmHg)";
        //                        break;
        //                    case 7:
        //                        careViewPrintSDO.CARE_TITLE1 = "Dấu hiệu sinh tồn";
        //                        careViewPrintSDO.CARE_TITLE2 = "Nhịp thở (lần/phút)";
        //                        break;
        //                    case 8:
        //                        careViewPrintSDO.CARE_TITLE1 = "Nước tiểu (ml)";
        //                        break;
        //                    case 9:
        //                        careViewPrintSDO.CARE_TITLE1 = "Phân (g)";
        //                        break;
        //                    case 10:
        //                        careViewPrintSDO.CARE_TITLE1 = "Cân nặng (kg)";
        //                        break;
        //                    case 11:
        //                        careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
        //                        careViewPrintSDO.CARE_TITLE2 = "Thuốc thường quy";
        //                        break;
        //                    case 12:
        //                        careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
        //                        careViewPrintSDO.CARE_TITLE2 = "Thuốc bổ sung";
        //                        break;
        //                    case 13:
        //                        careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
        //                        careViewPrintSDO.CARE_TITLE2 = "Xét nghiệm";
        //                        break;
        //                    case 14:
        //                        careViewPrintSDO.CARE_TITLE1 = "Thực hiện y lệnh";
        //                        careViewPrintSDO.CARE_TITLE2 = "Chế độ ăn";
        //                        break;
        //                    case 15:
        //                        careViewPrintSDO.CARE_TITLE1 = "Vệ sinh/thay quần áo-ga";
        //                        break;
        //                    case 16:
        //                        careViewPrintSDO.CARE_TITLE1 = "HD nội quy";
        //                        break;
        //                    case 17:
        //                        careViewPrintSDO.CARE_TITLE1 = "Giáo dục sức khỏe";
        //                        break;
        //                    default:
        //                        break;
        //                }

        //                careViewPrintSDO.CARE_1 = "";
        //                careViewPrintSDO.CARE_2 = "";
        //                careViewPrintSDO.CARE_3 = "";
        //                careViewPrintSDO.CARE_4 = "";
        //                careViewPrintSDO.CARE_5 = "";
        //                careViewPrintSDO.CARE_6 = "";
        //                careViewPrintSDO.CARE_7 = "";
        //                careViewPrintSDO.CARE_8 = "";
        //                careViewPrintSDO.CARE_9 = "";
        //                careViewPrintSDO.CARE_10 = "";
        //                careViewPrintSDO.CARE_11 = "";
        //                careViewPrintSDO.CARE_12 = "";
        //                this.lstCareViewPrintADO.Add(careViewPrintSDO);
        //            }
        //            #endregion

        //            CommonParam paramGet = new CommonParam();
        //            //MOS.Filter.HisCareFilter hisCareFilter = new MOS.Filter.HisCareFilter();
        //            //hisCareFilter.TREATMENT_ID = currentTreatmentId;
        //            //hisCareFilter.CARE_SUM_ID = printCareSum.ID;
        //            lstHisCareByTreatment = new List<HIS_CARE>();
        //            lstHisCareByTreatment = listCares.OrderBy(o => o.EXECUTE_TIME).ToList();

        //            //List<MOS.EFMODEL.DataModels.HIS_DHST> _hisDHSTs = new List<HIS_DHST>();
        //            if (lstHisCareByTreatment != null && lstHisCareByTreatment.Count > 0)
        //            {
        //                lstHisCareByTreatment = lstHisCareByTreatment.Skip(0).Take(6).ToList();
        //                //List<long> careIds = lstHisCareByTreatment.Select(p => p.ID).ToList();
        //                //MOS.Filter.HisDhstFilter hisDHSTFilter = new HisDhstFilter();
        //                //hisDHSTFilter.CARE_IDs = careIds;

        //                //_hisDHSTs = new BackendAdapter(paramGet).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, hisDHSTFilter, paramGet);

        //                foreach (var item in lstHisCareByTreatment)
        //                {

        //                    MOS.Filter.HisDhstFilter hisDHSTFilter = new HisDhstFilter();
        //                    hisDHSTFilter.CARE_ID = item.ID;

        //                    List<MOS.EFMODEL.DataModels.HIS_DHST> hisDHST = new BackendAdapter(paramGet).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, hisDHSTFilter, paramGet);
        //                    if (hisDHST != null && hisDHST.Count > 0)
        //                    {
        //                        item.HIS_DHST = hisDHST.FirstOrDefault();
        //                    }
        //                }
        //            }
        //            for (int i = 0; i < lstCareViewPrintADO.Count; i++)
        //            {
        //                #region -----
        //                switch (i)
        //                {
        //                    case 0:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], Inventec.Common.DateTime.Convert.TimeNumberToDateString(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0));
        //                        }
        //                        break;
        //                    case 1:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0) == null ? "" : Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0).Value.ToString("HH:mm"));
        //                        }
        //                        break;
        //                    case 2:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            var awarenest = lstHisAwareness.FirstOrDefault(o => o.ID == lstHisCareByTreatment[j].AWARENESS_ID);
        //                            //if (awarenest != null)
        //                            //{
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].AWARENESS);
        //                            //}
        //                        }
        //                        break;
        //                    case 3:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].MUCOCUTANEOUS);//da\
        //                        }
        //                        break;
        //                    case 4:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            if (lstHisCareByTreatment[j].HIS_DHST != null)
        //                            {
        //                                System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                                pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HIS_DHST.PULSE.ToString());//mach 
        //                            }
        //                        }
        //                        break;
        //                    case 5:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            if (lstHisCareByTreatment[j].HIS_DHST != null)
        //                            {
        //                                System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                                pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HIS_DHST.TEMPERATURE.ToString());//nhiet do
        //                            }
        //                        }
        //                        break;
        //                    case 6:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            if (lstHisCareByTreatment[j].HIS_DHST != null)
        //                            {
        //                                System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                                string strBloodPressure = "";
        //                                strBloodPressure += (lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MAX.HasValue ? (lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MAX).ToString() : "");
        //                                strBloodPressure += (lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MIN.HasValue ? "/" + (lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MIN).ToString() : "");
        //                                pi.SetValue(lstCareViewPrintADO[i], strBloodPressure);//huyet ap
        //                            }
        //                        }
        //                        break;
        //                    case 7:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            if (lstHisCareByTreatment[j].HIS_DHST != null)
        //                            {
        //                                System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                                pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HIS_DHST.BREATH_RATE.ToString());//nhip tho
        //                            }
        //                        }
        //                        break;
        //                    case 8:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].URINE);//Nước tiểu (ml)
        //                        }
        //                        break;
        //                    case 9:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].DEJECTA);//Phân (g) (ml)
        //                        }
        //                        break;
        //                    case 10:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)//Cân nặng
        //                        {
        //                            if (lstHisCareByTreatment[j].HIS_DHST != null)
        //                            {
        //                                System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                                var weight = lstHisCareByTreatment[j].HIS_DHST.WEIGHT.ToString();
        //                                pi.SetValue(lstCareViewPrintADO[i], weight.Trim() == "0" ? "" : weight);
        //                            }
        //                        }

        //                        //for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        //{
        //                        //    System.Reflection.PropertyInfo pi = typeof(EXE.SDO.CareViewPrintSDO).GetProperty("CARE_" + (j + 1));
        //                        //    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].WEIGHT.ToString());//can nang
        //                        //}
        //                        break;
        //                    case 11:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_MEDICINE == 1 ? "X" : "");//Thuốc thường quy
        //                        }
        //                        break;
        //                    case 12:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_ADD_MEDICINE == 1 ? "X" : "");//Thuốc bổ sung
        //                        }
        //                        break;
        //                    case 13:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_TEST == 1 ? "X" : "");//Xét nghiệm
        //                        }
        //                        break;
        //                    case 14:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].NUTRITION);//Chế độ ăn
        //                        }
        //                        break;
        //                    case 15:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].SANITARY);//Vệ sinh/thay quần áo-ga
        //                        }
        //                        break;
        //                    case 16:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].TUTORIAL);//HD nội quy
        //                        }
        //                        break;
        //                    case 17:
        //                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
        //                            pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].EDUCATION);//Giáo dục sức khỏe
        //                        }
        //                        break;
        //                    default:
        //                        break;
        //                }
        //                #endregion
        //            }

        //            MPS.Processor.Mps000151.PDO.CreatorADO _creator = new MPS.Processor.Mps000151.PDO.CreatorADO();
        //            for (int k = 0; k < lstHisCareByTreatment.Count; k++)
        //            {
        //                #region -----
        //                System.Reflection.PropertyInfo piCreator = typeof(MPS.Processor.Mps000151.PDO.CreatorADO).GetProperty("CREATOR_" + (k + 1));
        //                piCreator.SetValue(_creator, lstHisCareByTreatment[k].CREATOR);

        //                var userName = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(p => p.LOGINNAME == lstHisCareByTreatment[k].CREATOR).USERNAME;
        //                System.Reflection.PropertyInfo piUser = typeof(MPS.Processor.Mps000151.PDO.CreatorADO).GetProperty("USER_NAME_" + (k + 1));
        //                piUser.SetValue(_creator, userName);

        //                //Add Theo dõi - Chăm sóc
        //                MOS.Filter.HisCareDetailViewFilter hisCareDetailFilter = new MOS.Filter.HisCareDetailViewFilter();
        //                hisCareDetailFilter.CARE_ID = lstHisCareByTreatment[k].ID;

        //                List<MOS.EFMODEL.DataModels.V_HIS_CARE_DETAIL> lstHisCareDetail = new BackendAdapter(paramGet).Get<List<V_HIS_CARE_DETAIL>>(ApiConsumer.HisRequestUriStore.HIS_CARE_DETAIL_GETVIEW, ApiConsumers.MosConsumer, hisCareDetailFilter, paramGet);


        //                if (lstHisCareDetail != null && lstHisCareDetail.Count > 0)
        //                {
        //                    var careTypeIds = lstHisCareDetail.Select(o => o.CARE_TYPE_ID).Distinct().ToArray();
        //                    foreach (var caty in lstHisCareDetail)
        //                    {
        //                        if (!this.lstCareDetailViewPrintADO.Any(o => o.CARE_TYPE_ID == caty.CARE_TYPE_ID))
        //                        {
        //                            MPS.Processor.Mps000151.PDO.Mps000151PDO.CareDetailViewPrintADO careDetailViewPrintSDO = new MPS.Processor.Mps000151.PDO.Mps000151PDO.CareDetailViewPrintADO();
        //                            careDetailViewPrintSDO.CARE_TYPE_ID = caty.CARE_TYPE_ID;
        //                            careDetailViewPrintSDO.CARE_TITLE = "Theo dõi - Chăm sóc";
        //                            careDetailViewPrintSDO.CARE_DETAIL = caty.CARE_TYPE_NAME;
        //                            careDetailViewPrintSDO.CARE_DETAIL_1 = "";
        //                            careDetailViewPrintSDO.CARE_DETAIL_2 = "";
        //                            careDetailViewPrintSDO.CARE_DETAIL_3 = "";
        //                            careDetailViewPrintSDO.CARE_DETAIL_4 = "";
        //                            careDetailViewPrintSDO.CARE_DETAIL_5 = "";
        //                            careDetailViewPrintSDO.CARE_DETAIL_6 = "";
        //                            this.lstCareDetailViewPrintADO.Add(careDetailViewPrintSDO);
        //                        }
        //                    }


        //                    foreach (var item in this.lstCareDetailViewPrintADO)
        //                    {
        //                        var careDetailForOnes = lstHisCareDetail.Where(o => o.CARE_TYPE_ID == item.CARE_TYPE_ID).ToList();
        //                        if (careDetailForOnes != null && careDetailForOnes.Count > 0)
        //                        {
        //                            System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000151.PDO.Mps000151PDO.CareDetailViewPrintADO).GetProperty("CARE_DETAIL_" + (k + 1));
        //                            pi.SetValue(item, careDetailForOnes[0].CONTENT);
        //                        }
        //                    }
        //                }
        //                #endregion
        //            }
        //            this._CreatorADOs.Add(_creator);

        //            int countCaTyPrint = 6 - this.lstCareDetailViewPrintADO.Count;
        //            if (countCaTyPrint > 0)
        //            {
        //                for (int i = 0; i < countCaTyPrint; i++)
        //                {
        //                    MPS.Processor.Mps000151.PDO.Mps000151PDO.CareDetailViewPrintADO careDetailViewPrintSDO = new MPS.Processor.Mps000151.PDO.Mps000151PDO.CareDetailViewPrintADO();
        //                    careDetailViewPrintSDO.CARE_TITLE = "Theo dõi - Chăm sóc";
        //                    careDetailViewPrintSDO.CARE_DETAIL_1 = "";
        //                    careDetailViewPrintSDO.CARE_DETAIL_2 = "";
        //                    careDetailViewPrintSDO.CARE_DETAIL_3 = "";
        //                    careDetailViewPrintSDO.CARE_DETAIL_4 = "";
        //                    careDetailViewPrintSDO.CARE_DETAIL_5 = "";
        //                    careDetailViewPrintSDO.CARE_DETAIL_6 = "";
        //                    this.lstCareDetailViewPrintADO.Add(careDetailViewPrintSDO);
        //                }
        //            }
        //            WaitingManager.Hide();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        #endregion

        private void GetStringIcds(string delegateIcdCodes, string delegateIcdNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateIcdNames))
                {
                    txtIcdText.Text = delegateIcdNames;
                }
                if (!string.IsNullOrEmpty(delegateIcdCodes))
                {
                    txtSubCode.Text = delegateIcdCodes;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();

                    HIS.Desktop.ADO.SecondaryIcdADO sereservInTreatmentADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcds, txtSubCode.Text, txtIcdText.Text);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(sereservInTreatmentADO);
                    CallModule callModule = new CallModule(CallModule.SecondaryIcd, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                listCares = new List<HIS_CARE>();
                int[] selectRows = gridViewCare.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        listCares.Add((HIS_CARE)gridViewCare.GetRow(selectRows[i]));
                    }
                }

                if (listCares.Count >= 1 && listCares != null)
                {
                    long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY__HIS_DESKTOP_PLUGINS_CARE_IS_PRINT_MERGE));
                    if (keyPrintMerge == 1 && listCares.Count != 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Khi bật cấu hình in gộp phiếu chăm sóc chỉ được phép chọn 1 bản ghi phiếu chăm sóc để in", "Thông báo");
                        return;
                    }
                    onClickPrintCare();
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phiếu chăm sóc");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewCare_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    //long careSumId = 0;                   
                    var creator = gridViewCare.GetRowCellValue(e.RowHandle, "CREATOR");
                    var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                    if (e.Column.FieldName == "Delete")
                    {
                        if (loginName.Equals(creator))
                        {
                            e.RepositoryItem = DeleteE;
                        }
                        else
                        {
                            e.RepositoryItem = DeleteD;
                        }
                    }

                    if (e.Column.FieldName == "Edit")
                    {
                        if (loginName.Equals(creator))
                        {
                            e.RepositoryItem = EditE;
                        }
                        else
                        {
                            e.RepositoryItem = EditD;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCare_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_CARE data = (MOS.EFMODEL.DataModels.HIS_CARE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "EXECUTE_LOGINNAME_USERNAME")
                        {
                            e.Value = data.EXECUTE_LOGINNAME + " - " + data.EXECUTE_USERNAME;
                        }
                        else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "HAS_TEST_DISPLAY")
                        {
                            e.Value = (data.HAS_TEST ?? 0) == 1 ? "X" : "";
                        }
                        else if (e.Column.FieldName == "HAS_MEDICINE_DISPLAY")
                        {
                            e.Value = (data.HAS_MEDICINE ?? 0) == 1 ? "X" : "";
                        }
                        else if (e.Column.FieldName == "HAS_ADD_MEDICINE_DISPLAY")
                        {
                            e.Value = (data.HAS_ADD_MEDICINE ?? 0) == 1 ? "X" : "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControlCare()
        {
            try
            {
                gridControlCare.DataSource = null;
                lstCare = new List<HIS_CARE>();
                CommonParam param = new CommonParam();
                HisCareFilter filter = new HisCareFilter();
                filter.TREATMENT_ID = this.treatmentId;
                filter.CARE_SUM_ID = currentData.ID;
                if (dtFrom.DateTime != null && dtFrom.DateTime != DateTime.MinValue) { filter.EXECUTE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtFrom.DateTime.ToString("yyyyMMdd") + "000000"); }
                if (dtTo.DateTime != null && dtTo.DateTime != DateTime.MinValue) { filter.EXECUTE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtTo.DateTime.ToString("yyyyMMdd") + "235959"); }
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var cares = new BackendAdapter(param).Get<List<HIS_CARE>>(ApiConsumer.HisRequestUriStore.HIS_CARE_GET, ApiConsumers.MosConsumer, filter, param);

                if (cares != null && cares.Count > 0)
                {
                    gridControlCare.BeginUpdate();
                    gridControlCare.DataSource = cares;
                    gridControlCare.EndUpdate();
                    lstCare = cares;
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EditE_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (HIS_CARE)gridViewCare.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    if (DataTransferTreatmentBedRoomFilter != null)
                        listArgs.Add(DataTransferTreatmentBedRoomFilter);
                    CallModule callModule = new CallModule(CallModule.CareCreate, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                FillDataToGridControlCare();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DeleteE_Click(object sender, EventArgs e)
        {
            bool success = false;
            CommonParam param = new CommonParam();
            try
            {
                var row = (HIS_CARE)gridViewCare.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (row != null)
                    {
                        success = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_CARE_DELETE, ApiConsumer.ApiConsumers.MosConsumer, row, param);
                        WaitingManager.Show();
                        if (success)
                        {
                            FillDataToGridControlCare();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this, param, success);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                Inventec.Desktop.Common.LibraryMessage.MessageUtil.SetMessage(param, Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }
        }

        private void gridViewCare_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSubCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtSubCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSubCode_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();
                string strIcdNames = "";
                if (!String.IsNullOrEmpty(currentValue))
                {
                    string seperate = ";";
                    string strWrongIcdCodes = "";
                    string[] periodSeparators = new string[1];
                    periodSeparators[0] = seperate;
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = txtSubCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (seperate + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (seperate + itemCode);
                            }
                        }
                        strIcdNames += seperate;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format(ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = txtSubCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = txtSubCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }

                            txtSubCode.Focus();
                            txtSubCode.Select(startPositionWarm, lenghtPositionWarm);
                        }
                    }
                }
                SetCheckedIcdsToControl(txtSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
                txtIcdText.Text = processIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtSubCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string processIcdNameChanged(string oldIcdNames, string newIcdNames)
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

        public void InitMenuToButtonPrint()
        {
            try
            {
                long UsingFormVersion = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(SdaConfigKeys.USING_FORM_VERSION));

                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemPhieuChamSoc = new DXMenuItem("Phiếu chăm sóc", new EventHandler(OnClickInPhieuChamSoc));
                itemPhieuChamSoc.Tag = PrintTypeCare.IN_KET_QUA_CHAM_SOC_TONG_HOP;
                menu.Items.Add(itemPhieuChamSoc);

                DXMenuItem itemInChamSocQy7 = new DXMenuItem("Phiếu chăm sóc _ Y lệnh", new EventHandler(OnClickInPhieuChamSoc));
                itemInChamSocQy7.Tag = PrintTypeCare.IN_PHIEU_CHAM_SOC_QY7;
                menu.Items.Add(itemInChamSocQy7);

                SAR_PRINT_TYPE checkMps = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuChamSocCapI_MPS000427);

                if (checkMps != null && checkMps.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && UsingFormVersion == 2)
                {
                    DXMenuItem itemPhieuChamSocCapI = new DXMenuItem("Phiếu chăm sóc cấp 1", new EventHandler(OnClickInPhieuChamSoc));
                    itemPhieuChamSocCapI.Tag = PrintTypeCare.IN_KET_QUA_CHAM_SOC_CAP_I;
                    menu.Items.Add(itemPhieuChamSocCapI);
                }

                btnCboPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuChamSoc(object sender, EventArgs e)
        {
            try
            {
                var bbtnItem = sender as DXMenuItem;
                PrintTypeCare type = (PrintTypeCare)(bbtnItem.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBieuMauPhieuQy7(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                List<HIS_CARE> _CareChecks = new List<HIS_CARE>();
                if (gridViewCare.RowCount > 0)
                {
                    for (int i = 0; i < gridViewCare.SelectedRowsCount; i++)
                    {
                        if (gridViewCare.GetSelectedRows()[i] >= 0)
                        {
                            _CareChecks.Add((HIS_CARE)gridViewCare.GetRow(gridViewCare.GetSelectedRows()[i]));
                        }
                    }
                }
                if (_CareChecks == null || _CareChecks.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dữ liệu in", "Thông báo");
                    return;
                }




                WaitingManager.Show();
                var _Treatment = PrintGlobalStore.getTreatment(this.treatmentId);
                MOS.Filter.HisTreatmentBedRoomViewFilter filter = new HisTreatmentBedRoomViewFilter();
                filter.TREATMENT_ID = this.treatmentId;
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                V_HIS_TREATMENT_BED_ROOM _TreatmetnbedRoom = new V_HIS_TREATMENT_BED_ROOM();
                var TreatmetnbedRooms = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filter, null);
                if (TreatmetnbedRooms != null && TreatmetnbedRooms.Count > 0)
                {
                    _TreatmetnbedRoom = TreatmetnbedRooms.FirstOrDefault();
                }

                if (_CareChecks != null && _CareChecks.Count > 0)
                {
                    _CareChecks = _CareChecks.OrderBy(p => p.EXECUTE_TIME).ToList();
                }
                var mps000229ADO = new MPS.Processor.Mps000229.PDO.Mps000229ADO();
                mps000229ADO.DEPARTMENT_NAME = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.currentModuleBase.RoomId).DepartmentName;
                //Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode, moduleData.RoomId);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode);

                long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY__HIS_DESKTOP_PLUGINS_CARE_IS_PRINT_MERGE));
                if (keyPrintMerge == 1)
                {
                    string uniqueTime = "";// (_TrackingPrints != null && _TrackingPrints.Count > 0) ? _TrackingPrints[0].TRACKING_TIME + "" : "";
                    inputADO.MergeCode = String.Format("{0}_{1}_{2}", printTypeCode, uniqueTime, (_Treatment != null ? _Treatment.TREATMENT_CODE : ""));
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.MergeCode), inputADO.MergeCode));


                MPS.Processor.Mps000229.PDO.Mps000229PDO mps000229RDO = new MPS.Processor.Mps000229.PDO.Mps000229PDO(
                  _Treatment,
                  _CareChecks,
                  _TreatmetnbedRoom,
                  mps000229ADO
                    );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (chkSign.Checked)
                {
                    if (chkPrintDocumentSigned.Checked)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000229RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000229RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000229RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000229RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void btnCboPrint_Click(object sender, EventArgs e)
        {
            btnCboPrint.ShowDropDown();
            ProcessPrint();
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "CREATE_CARE")
                    {
                        if (currentTreatment.IS_PAUSE != 1 && !IsTreatmentList)
                        {
                            e.RepositoryItem = Create;
                        }
                        else if (currentTreatment.IS_PAUSE == 1 && HisConfigCFG.AllowUpdatingAfterLockingTreatment == "1")
                        {
                            e.RepositoryItem = Create;
                        }
                        else
                        {
                            e.RepositoryItem = CreateDisable;
                        }
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (currentTreatment.IS_PAUSE != 1 && !IsTreatmentList)
                        {
                            e.RepositoryItem = btnGEdit;
                        }
                        else if (currentTreatment.IS_PAUSE == 1 && HisConfigCFG.AllowUpdatingAfterLockingTreatment == "1")
                        {
                            e.RepositoryItem = btnGEdit;
                        }
                        else
                        {
                            e.RepositoryItem = DeleteDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #region<Lưu trạng thái checkbox vào máy trạm>
        private void ProcessPrint()
        {
            try
            {
                ConfigADO ado = new ConfigADO();
                //richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                if (chkSign.Checked)
                {
                    ado.IsSign = "1";
                    //richEditorMain.RunPrintTemplate("Mps000178", DelegateRunPrinter);
                }

                if (chkPrintDocumentSigned.Checked)
                {
                    ado.IsPrintDocumentSigned = "1";
                    // richEditorMain.RunPrintTemplate("Mps000001", DelegateRunPrinter);
                }

                if (this._ConfigADO != null && (this._ConfigADO.IsSign != ado.IsSign || this._ConfigADO.IsPrintDocumentSigned != ado.IsPrintDocumentSigned))
                {
                    string value = Newtonsoft.Json.JsonConvert.SerializeObject(ado);

                    //Update cònig
                    SDA_CONFIG_APP_USER configAppUserUpdate = new SDA_CONFIG_APP_USER();
                    configAppUserUpdate.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    configAppUserUpdate.VALUE = value;
                    configAppUserUpdate.CONFIG_APP_ID = _currentConfigApp.ID;
                    if (currentConfigAppUser != null)
                        configAppUserUpdate.ID = currentConfigAppUser.ID;
                    string api = configAppUserUpdate.ID > 0 ? "api/SdaConfigAppUser/Update" : "api/SdaConfigAppUser/Create";
                    CommonParam param = new CommonParam();
                    var UpdateResult = new BackendAdapter(param).Post<SDA_CONFIG_APP_USER>(
                            api, ApiConsumers.SdaConsumer, configAppUserUpdate, param);

                    //if (UpdateResult != null)
                    //{
                    //    success = true;
                    //}

                    MessageManager.Show(this.ParentForm, param, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadConfigHisAcc()
        {
            try
            {
                CommonParam param = new CommonParam();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SDA.Filter.SdaConfigAppFilter configAppFilter = new SDA.Filter.SdaConfigAppFilter();
                configAppFilter.APP_CODE_ACCEPT = GlobalVariables.APPLICATION_CODE;
                configAppFilter.KEY = "CONFIG_KEY__HIS_PLUGINS_CARE_SUM__IS_SIGN_IS_PRINT_DOCUMENT_SIGNED";

                _currentConfigApp = new BackendAdapter(param).Get<List<SDA_CONFIG_APP>>("api/SdaConfigApp/Get", ApiConsumers.SdaConsumer, configAppFilter, param).FirstOrDefault();

                string key = "";
                if (_currentConfigApp != null)
                {
                    key = _currentConfigApp.DEFAULT_VALUE;
                    SDA.Filter.SdaConfigAppUserFilter appUserFilter = new SDA.Filter.SdaConfigAppUserFilter();
                    appUserFilter.LOGINNAME = loginName;
                    appUserFilter.CONFIG_APP_ID = _currentConfigApp.ID;
                    currentConfigAppUser = new BackendAdapter(param).Get<List<SDA_CONFIG_APP_USER>>("api/SdaConfigAppUser/Get", ApiConsumers.SdaConsumer, appUserFilter, param).FirstOrDefault();
                    if (currentConfigAppUser != null)
                    {
                        key = currentConfigAppUser.VALUE;
                    }
                }

                if (!string.IsNullOrEmpty(key))
                {
                    _ConfigADO = (ConfigADO)Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigADO>(key);
                    if (_ConfigADO != null)
                    {
                        if (_ConfigADO.IsSign == "1")
                            chkSign.Checked = true;
                        else
                            chkSign.Checked = false;
                        if (_ConfigADO.IsPrintDocumentSigned == "1")
                            chkPrintDocumentSigned.Checked = true;
                        else
                            chkPrintDocumentSigned.Checked = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSign.Checked == true)
                chkPrintDocumentSigned.Enabled = true;
            else
            {
                chkPrintDocumentSigned.Checked = false;
                chkPrintDocumentSigned.Enabled = false;
            }
        }

        private void gridViewCare_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var data = (HIS_CARE)this.gridViewCare.GetFocusedRow();
                if (data != null && data.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                {
                    if (e.Column.FieldName == "Edit")
                    {
                        EditE_Click(null, null);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        DeleteE_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


    }
}
