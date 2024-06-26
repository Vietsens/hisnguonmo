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
using System.Threading.Tasks;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisSurgServiceType.Resources;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.HisSurgServiceType.HisSurgServiceType
{
    public partial class frmHisSurgServiceType : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        internal long serviceId;
        internal long surgServiceId;
        List<V_HIS_SURG_SERVICE_TYPE> ListSurgser;
        #endregion

        #region Construct
        public frmHisSurgServiceType(Inventec.Desktop.Common.Modules.Module moduleData)
		:base(moduleData)
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
        private void frmHisSurgServiceType_Load(object sender, EventArgs e)
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisSurgServiceType.Resources.Lang", typeof(HIS.Desktop.Plugins.HisSurgServiceType.HisSurgServiceType.frmHisSurgServiceType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn6.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboICD.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.cboICD.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkPtttGroupId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.lkPtttGroupId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCha.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.cboCha.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.cboLoaiDV.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLoaiDV.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.cboBHYT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkServiceId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.lkServiceId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsLeaf.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.chkIsLeaf.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSurgServiceTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.lciSurgServiceTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSurgServiceTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.lciSurgServiceTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciServiceId.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.lciServiceId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsLeaf.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.lciIsLeaf.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spCogs.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.spCogs.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.treeListColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmHisSurgServiceType.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                spGia.EditValue = null;
                spTime.EditValue = null;
                spNumOrder.EditValue = null;
                spDinhMuc.EditValue = null;
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
            //try
            //{
            //    dicOrderTabIndexControl.Add("txtSurgServiceTypeCode", 0);
            //    dicOrderTabIndexControl.Add("txtSurgServiceTypeName", 1);
            //    dicOrderTabIndexControl.Add("lkServiceId", 2);
            //    dicOrderTabIndexControl.Add("lkParentId", 3);
            //    dicOrderTabIndexControl.Add("chkIsLeaf", 4);
            //    dicOrderTabIndexControl.Add("spNumOrder", 5);
            //    dicOrderTabIndexControl.Add("lkPtttGroupId", 6);


            //    if (dicOrderTabIndexControl != null)
            //    {
            //        foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
            //        {
            //            SetTabIndexToControl(itemOrderTab, lcEditorInfo);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        //private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        //{
        //bool success = false;
        //try
        //{
        //    if (!layoutControlEditor.IsInitialized) return success;
        //    layoutControlEditor.BeginUpdate();
        //    try
        //    {
        //        foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
        //        {
        //            DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
        //            if (lci != null && lci.Control != null)
        //            {
        //                BaseEdit be = lci.Control as BaseEdit;
        //                if (be != null)
        //                {
        //                    //Cac control dac biet can fix khong co thay doi thuoc tinh enable
        //                    if (itemOrderTab.Key.Contains(be.Name))
        //                    {
        //                        be.TabIndex = itemOrderTab.Value;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        layoutControlEditor.EndUpdate();
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Inventec.Common.Logging.LogSystem.Warn(ex);
        //}

        //return success;
        //}

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboServiceId();
                InitComboParentId();
                InitComboPtttGroupId();
                InitComboLoaiDV();
                InitComboLoaiPatientType();
                InitComboICDCM();
                initComboBill();
                InitComboMethod();
                InitComboLoaiBH();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboServiceId()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(lkServiceId, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboLoaiDV()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_BHYT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_BHYT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboLoaiBH, BackendDataWorker.Get<HIS_HEIN_SERVICE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboLoaiBH()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboLoaiDV, BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboLoaiPatientType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>(), controlEditorADO);
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
                var a = BackendDataWorker.Get<HIS_PTTT_METHOD>();
                var data = a.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(); ;
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
        private void InitComboParentId()
        {
            try
            {
                var SurgServiceTypeParents = BackendDataWorker.Get<V_HIS_SURG_SERVICE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SURG_SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SURG_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SURG_SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCha, SurgServiceTypeParents, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboPtttGroupId()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(lkPtttGroupId, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PTTT_GROUP>(), controlEditorADO);
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
        //public void FillDataToGridControl()
        //{
        //    try
        //    {
        //        WaitingManager.Show();

        //        LoadPaging(new CommonParam(0, (int)ConfigApplications.NumPageSize));

        //        CommonParam param = new CommonParam();
        //        param.Limit = rowCount;
        //        param.Count = dataTotal;
        //        ucPaging.Init(LoadPaging, param);
        //        WaitingManager.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Error(ex);
        //        WaitingManager.Hide();
        //    }
        //}

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        //private void LoadPaging(object param)
        //{
        //    try
        //    {
        //        startPage = ((CommonParam)param).Start ?? 0;
        //        int limit = ((CommonParam)param).Limit ?? 0;
        //        CommonParam paramCommon = new CommonParam(startPage, limit);
        //        Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE>> apiResult = null;
        //        HisSurgServiceTypeViewFilter filter = new HisSurgServiceTypeViewFilter();
        //        SetFilterNavBar(ref filter);
        //        dnNavigation.DataSource = null;
        //        gridviewFormList.BeginUpdate();
        //        apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE>>(HisRequestUriStore.MOSV_HIS_SURG_SERVICE_TYPE_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
        //        if (apiResult != null)
        //        {
        //            var data = (List<MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE>)apiResult.Data;
        //            if (data != null)
        //            {
        //                dnNavigation.DataSource = data;
        //                gridviewFormList.GridControl.DataSource = data;
        //                rowCount = (data == null ? 0 : data.Count);
        //                dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
        //            }
        //        }
        //        gridviewFormList.EndUpdate();

        //        #region Process has exception
        //        SessionManager.ProcessTokenLost(paramCommon);
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Error(ex);
        //    }
        //}

        private void SetFilterNavBar(ref HisSurgServiceTypeViewFilter filter)
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
                //else if (e.KeyCode == Keys.Down)
                //{
                //    gridviewFormList.Focus();
                //    gridviewFormList.FocusedRowHandle = 0;
                //    var rowData = (MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE)gridviewFormList.GetFocusedRow();
                //    if (rowData != null)
                //    {
                //        ChangedDataRow(rowData);
                //    }
                //}
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
                //else if (e.KeyCode == Keys.Down)
                //{
                //    gridviewFormList.Focus();
                //    gridviewFormList.FocusedRowHandle = 0;
                //    var rowData = (MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE)gridviewFormList.GetFocusedRow();
                //    if (rowData != null)
                //    {
                //        ChangedDataRow(rowData);
                //    }
                //}
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
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE pData = (MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
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
                    else if (e.Column.FieldName == "IS_LEAF_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_LEAF == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la khoa lam sang IS_CLINICAL_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "PARENT_NAME")
                    {
                        try
                        {
                            e.Value = pData.PARENT_ID;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    //gridControlFormList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var rowData = (MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE)gridviewFormList.GetFocusedRow();
        //        if (rowData != null)
        //        {
        //            ChangedDataRow(rowData);

        //            //Set focus vào control editor đầu tiên
        //            SetFocusEditor();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            var rowData = (MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE)gridviewFormList.GetFocusedRow();
        //            if (rowData != null)
        //            {
        //                ChangedDataRow(rowData);

        //                //Set focus vào control editor đầu tiên
        //                SetFocusEditor();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void dnNavigation_PositionChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        this.currentData = (MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE>)[dnNavigation.Position];
        //        if (this.currentData != null)
        //        {
        //            ChangedDataRow(this.currentData);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    // btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    serviceId = data.SERVICE_ID;
                    surgServiceId = data.ID;
                    txtICD.EditValue = data.ICD_CM_CODE;
                    cboICD.EditValue = data.ICD_CM_ID;
                    txtSurgServiceTypeCode.Text = data.SURG_SERVICE_TYPE_CODE;
                    txtSurgServiceTypeName.Text = data.SURG_SERVICE_TYPE_NAME;
                    lkServiceId.EditValue = data.SERVICE_UNIT_ID;
                    cboLoaiDV.EditValue = data.HEIN_SERVICE_TYPE_ID;
                    cboPatientType.EditValue = data.BILL_PATIENT_TYPE_ID;
                    cboMethod.EditValue = data.PTTT_METHOD_ID;
                    cboLoaiBH.EditValue = data.HEIN_SERVICE_ID;
                    if (data.BILL_OPTION == null)
                    {
                        cboHoaDon.EditValue = 1;
                    }
                    else if (data.BILL_OPTION == 1)
                    {
                        cboHoaDon.EditValue = 2;
                    }
                    else if (data.BILL_OPTION == 2)
                    {
                        cboHoaDon.EditValue = 3;
                    }
                    spGia.EditValue = data.COGS;
                    spDinhMuc.EditValue = data.MAX_EXPEND;
                    spTime.EditValue = data.ESTIMATE_DURATION;
                    cboCha.EditValue = data.PARENT_ID;
                    var search = (cboCha.Properties.DataSource as List<V_HIS_SURG_SERVICE_TYPE>).FirstOrDefault(o => o.ID == (data.PARENT_ID ?? 0));
                    chkIsLeaf.Checked = (data.IS_MULTI_REQUEST == 1 ? true : false);
                    chkOutPack.Checked = (data.IS_OUT_PARENT_FEE == 1 ? true : false);
                    spNumOrder.EditValue = data.NUM_ORDER;
                    lkPtttGroupId.EditValue = data.PTTT_GROUP_ID;

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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSurgServiceTypeFilter filter = new HisSurgServiceTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE>>(HisRequestUriStore.MOSV_HIS_SURG_SERVICE_TYPE_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                //FillDataToGridControl();
                LoaddataToTreeList(this);
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
                LoaddataToTreeList(this);
                //FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        var rowData = (MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE)gridviewFormList.GetFocusedRow();
        //        if (rowData != null)
        //        {
        //            bool success = false;
        //            CommonParam param = new CommonParam();
        //            success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSV_HIS_SURG_SERVICE_TYPE_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
        //            if (success)
        //            {
        //                //FillDataToGridControl();
        //            }
        //            MessageManager.Show(this, param, success);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                txtSurgServiceTypeCode.Focus();
                //SetFocusEditor();
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
                MOS.SDO.HisSurgServiceTypeSDO surgServiceSDO = new MOS.SDO.HisSurgServiceTypeSDO();
                MOS.SDO.HisSurgServiceTypeSDO ResultsurgServiceSDO = new MOS.SDO.HisSurgServiceTypeSDO();

                surgServiceSDO.HisService = SetDataService();
                surgServiceSDO.HisSurgServiceType = SetDataSurgService();
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    ResultsurgServiceSDO = new BackendAdapter(param).Post<HisSurgServiceTypeSDO>("api/HisSurgServiceType/Create", ApiConsumers.MosConsumer, surgServiceSDO, param);
                }

                else
                {
                    if (serviceId > 0 && surgServiceId > 0)
                    {
                        surgServiceSDO.HisService.ID = serviceId;
                        surgServiceSDO.HisSurgServiceType.ID = surgServiceId;
                        ResultsurgServiceSDO = new BackendAdapter(param).Post<HisSurgServiceTypeSDO>("api/HisSurgServiceType/Update", ApiConsumers.MosConsumer, surgServiceSDO, param);
                    }
                }
                if (ResultsurgServiceSDO != null)
                {
                    success = true;
                    //FillDataToGridControl();
                    LoaddataToTreeList(this);
                    ResetFormData();
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
        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE currentDTO)
        {
            try
            {
                currentDTO.SURG_SERVICE_TYPE_CODE = txtSurgServiceTypeCode.Text.Trim();
                currentDTO.SURG_SERVICE_TYPE_NAME = txtSurgServiceTypeName.Text.Trim();
                if (lkServiceId.EditValue != null) currentDTO.SERVICE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkServiceId.EditValue ?? "0").ToString());
                //if (lkParentId.EditValue != null) currentDTO.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkParentId.EditValue ?? "0").ToString());
                currentDTO.IS_LEAF = (short)(chkIsLeaf.Checked ? 1 : 0);
                currentDTO.NUM_ORDER = (long)spNumOrder.Value;
                if (lkPtttGroupId.EditValue != null) currentDTO.PTTT_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkPtttGroupId.EditValue ?? "0").ToString());

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
                ValidationSingleControl(txtSurgServiceTypeCode);
                ValidationSingleControl(txtSurgServiceTypeName);
                ValidationSingleControl(lkServiceId);
                //ValidationSingleControl(cboLoaiDV);
                //ValidationSingleControl(cboPatientType);
                //ValidationSingleControl(spNumOrder);
                //ValidationSingleControl(lkPtttGroupId);

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
                validRule.ErrorText = String.Format(ResourceMessage.TruongDuLieuBatBuoc);
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
                validRule.ErrorText = String.Format(ResourceMessage.TruongDuLieuBatBuoc);
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
                validRule.ErrorText = String.Format(ResourceMessage.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
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

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                //FillDataToGridControl();
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
                return;
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

        //private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        //{
        //    try
        //    {
        //        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
        //        if (e.RowHandle >= 0)
        //        {

        //            V_HIS_SURG_SERVICE_TYPE data = (V_HIS_SURG_SERVICE_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
        //            if (e.Column.FieldName == "Lock")
        //            {
        //                e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnlock : btnLock);

        //            }
        //            if (e.Column.FieldName == "Delete")
        //            {
        //                try
        //                {
        //                    if (data.IS_ACTIVE == IS_ACTIVE_TRUE)
        //                        e.RepositoryItem = btnGEdit;
        //                    else
        //                        e.RepositoryItem = btnGEdit_Disable;

        //                }
        //                catch (Exception ex)
        //                {

        //                    Inventec.Common.Logging.LogSystem.Error(ex);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    CommonParam param = new CommonParam();
        //    V_HIS_SURG_SERVICE_TYPE hisSurgServiceType = new V_HIS_SURG_SERVICE_TYPE();
        //    bool notHandler = false;
        //    try
        //    {
        //        V_HIS_SURG_SERVICE_TYPE dataDepartment = (V_HIS_SURG_SERVICE_TYPE)gridviewFormList.GetFocusedRow();
        //        if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //        {
        //            V_HIS_SURG_SERVICE_TYPE data1 = new V_HIS_SURG_SERVICE_TYPE();
        //            data1.ID = dataDepartment.ID;
        //            WaitingManager.Show();
        //            hisSurgServiceType = new BackendAdapter(param).Post<V_HIS_SURG_SERVICE_TYPE>("api/HisSurgServiceType/ChangeLock", ApiConsumers.MosConsumer, data1, param);
        //            WaitingManager.Hide();
        //            if (hisSurgServiceType != null) FillDataToGridControl();
        //            btnEdit.Enabled = false;
        //        }
        //        notHandler = true;
        //        MessageManager.Show(this.ParentForm, param, notHandler);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    CommonParam param = new CommonParam();
        //    V_HIS_SURG_SERVICE_TYPE hisSurgServiceType = new V_HIS_SURG_SERVICE_TYPE();
        //    bool notHandler = false;
        //    try
        //    {
        //        V_HIS_SURG_SERVICE_TYPE dataDepartment = (V_HIS_SURG_SERVICE_TYPE)gridviewFormList.GetFocusedRow();
        //        if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //        {
        //            V_HIS_SURG_SERVICE_TYPE data1 = new V_HIS_SURG_SERVICE_TYPE();
        //            data1.ID = dataDepartment.ID;
        //            WaitingManager.Show();
        //            hisSurgServiceType = new BackendAdapter(param).Post<V_HIS_SURG_SERVICE_TYPE>("api/HisSurgServiceType/ChangeLock", ApiConsumers.MosConsumer, data1, param);
        //            WaitingManager.Hide();
        //            if (hisSurgServiceType != null) FillDataToGridControl();
        //            btnEdit.Enabled = true;
        //        }
        //        notHandler = true;
        //        MessageManager.Show(this.ParentForm, param, notHandler);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void txtSurgServiceTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSurgServiceTypeName.Focus();
                    txtSurgServiceTypeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtSurgServiceTypeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    lkServiceId.Focus();
                    lkServiceId.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void lkServiceId_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (lkServiceId.EditValue != null)
                    {
                        cboPatientType.Focus();
                        cboPatientType.ShowPopup();
                    }
                    else
                    {
                        lkServiceId.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkServiceId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lkServiceId.EditValue == null)
                    {
                        lkServiceId.Focus();
                        lkServiceId.ShowPopup();
                    }
                    else
                    {
                        cboPatientType.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiDV_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPatientType.EditValue != null)
                    {
                        cboLoaiDV.Focus();
                    }
                    else
                    {
                        cboPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiDV_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboLoaiBH.Focus();
                    cboLoaiBH.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBHYT_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

        }

        private void cboBHYT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboBHYT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLoaiDV.EditValue != null)
                    {
                        cboCha.Focus();
                        //spGia.SelectAll();
                    }
                    else
                    {
                        cboLoaiDV.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkParentId_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    spGia.Focus();
                    spGia.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkParentId_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLoaiDV.EditValue != null)
                    {
                        spGia.Focus();
                        spGia.SelectAll();
                    }
                    else
                    {
                        //lkParentId.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spGia_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spDinhMuc.Focus();
                    spDinhMuc.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsLeaf.Properties.FullFocusRect = true;
                    chkIsLeaf.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsLeaf_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkOutPack.Focus();

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    lkPtttGroupId.Focus();
                    lkPtttGroupId.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkPtttGroupId_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    btnAdd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkPtttGroupId_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lkPtttGroupId.EditValue != null)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        lkPtttGroupId.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiDV_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLoaiDV.EditValue == null)
                    {
                        cboLoaiDV.Focus();
                        cboLoaiDV.ShowPopup();
                    }
                    else
                    {
                        cboLoaiBH.Focus();
                        cboLoaiBH.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBHYT_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void lkPtttGroupId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lkPtttGroupId.EditValue != null)
                    {
                        btnAdd.Focus();
                    }
                }
                else
                {
                    lkPtttGroupId.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCha_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtICD.Focus();
                    txtICD.SelectAll();
                }
                //if (e.CloseMode == PopupCloseMode.Normal)
                //{
                //    txtICD.Focus();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCha_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCha.EditValue != null)
                    {
                        spGia.Focus();
                        spGia.SelectAll();
                    }
                    else
                    {
                        cboCha.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoaddataToTreeList(frmHisSurgServiceType control)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSurgServiceTypeViewFilter filter = new MOS.Filter.HisSurgServiceTypeViewFilter();
                filter.KEY_WORD = txtKeyword.Text;
                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_SURG_SERVICE_TYPE>>("api/HisSurgServiceType/GetView", ApiConsumers.MosConsumer, filter, param).ToList();
                if (currentDataStore != null)
                {

                    TreeListSurgSer.KeyFieldName = "ID";
                    TreeListSurgSer.ParentFieldName = "PARENT_ID";
                    TreeListSurgSer.DataSource = currentDataStore;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal static void CreateChildNode(TreeListNode rootNode, V_HIS_SURG_SERVICE_TYPE hisDataStore, List<V_HIS_SURG_SERVICE_TYPE> hisDataStores, frmHisSurgServiceType control)
        {
            try
            {
                var listChilds = hisDataStores.Where(o => o.PARENT_ID == hisDataStore.ID).ToList();
                if (listChilds != null && listChilds.Count > 0)
                {
                    foreach (var itemChild in listChilds)
                    {
                        TreeListNode childNode = control.TreeListSurgSer.AppendNode(
                        new object[] { itemChild.IS_ACTIVE, itemChild.IS_DELETE, itemChild.SURG_SERVICE_TYPE_NAME, itemChild.SURG_SERVICE_TYPE_CODE, itemChild.SERVICE_UNIT_NAME, itemChild.HEIN_SERVICE_BHYT_NAME, itemChild.COGS, itemChild.ESTIMATE_DURATION, itemChild.PTTT_GROUP_NAME },
                        rootNode, itemChild);
                        CreateChildNode(childNode, itemChild, hisDataStores, control);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TreeListSurgSer_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            SurgServiceTypeADO data = new SurgServiceTypeADO();
            try
            {
                //SurgServiceTypeADO data = new SurgServiceTypeADO();
                if (data != null)
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        e.NodeImageIndex = 0;
                    else
                        e.NodeImageIndex = 1;
                }
                else
                    e.NodeImageIndex = -2;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TreeListSurgSer_GetStateImage(object sender, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            SurgServiceTypeADO data = new SurgServiceTypeADO();
            try
            {
                if (data != null)
                {
                    e.NodeImageIndex = 2;
                }
                else
                    e.NodeImageIndex = -2;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TreeListSurgSer_StateImageClick(object sender, DevExpress.XtraTreeList.NodeClickEventArgs e)
        {
            SurgServiceTypeADO data = new SurgServiceTypeADO();
            try
            {
                try
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    Inventec.Common.Adapter.BackendAdapter adapter = new Inventec.Common.Adapter.BackendAdapter(param);
                    success = adapter.Post<bool>("api/HisSurgServiceType/Delete", ApiConsumers.MosConsumer, data, param);
                    WaitingManager.Hide();
                    if (success)
                    {
                        this.ListSurgser.RemoveAll(o => o.ID == data.ID);
                        //this.medicineTypeProcessor.Reload(this.ucMedicineType, this.medicineTypes);
                    }

                    MessageManager.Show(param, success);
                    SessionManager.ProcessTokenLost(param);
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

        private void TreeListSurgSer_SelectImageClick(object sender, DevExpress.XtraTreeList.NodeClickEventArgs e)
        {
            SurgServiceTypeADO data = new SurgServiceTypeADO();
            try
            {
                bool success = false;
                //MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE medicineType = new HIS_MEDICINE_TYPE();
                //AutoMapper.Mapper.CreateMap<UC.MedicineType.ADO.MedicineTypeADO, HIS_MEDICINE_TYPE>();
                //medicineType = AutoMapper.Mapper.Map<HIS_MEDICINE_TYPE>(data);

                MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE vmedicineType = new V_HIS_SURG_SERVICE_TYPE();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SURG_SERVICE_TYPE>(vmedicineType, data);
                MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE result = null;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                Inventec.Common.Adapter.BackendAdapter adapter = new Inventec.Common.Adapter.BackendAdapter(param);
                result = adapter.Post<MOS.EFMODEL.DataModels.V_HIS_SURG_SERVICE_TYPE>("api/HisSurgServiceType/ChangeLock", ApiConsumers.MosConsumer, vmedicineType, param);

                WaitingManager.Hide();

                if (result != null)
                {
                    success = true;
                    if (result.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        Parallel.ForEach(this.ListSurgser.Where(f => f.ID == result.ID), l => l.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    }
                    else
                    {
                        Parallel.ForEach(this.ListSurgser.Where(f => f.ID == result.ID), l => l.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE);
                    }
                    //Reload
                    LoaddataToTreeList(this);
                    //this.medicineTypeProcessor.Reload(this.ucMedicineType, this.medicineTypes);
                }

                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_SURG_SERVICE_TYPE hisMediStock = new V_HIS_SURG_SERVICE_TYPE();
            bool notHandler = false;
            try
            {
                WaitingManager.Show();
                var data = TreeListSurgSer.GetDataRecordByNode(TreeListSurgSer.FocusedNode);
                V_HIS_SURG_SERVICE_TYPE dataMediStock = data as V_HIS_SURG_SERVICE_TYPE;
                if (MessageBox.Show(String.Format(ResourceMessage.KhoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_SURG_SERVICE_TYPE data1 = new V_HIS_SURG_SERVICE_TYPE();
                    data1.ID = dataMediStock.ID;
                    WaitingManager.Show();
                    hisMediStock = new BackendAdapter(param).Post<V_HIS_SURG_SERVICE_TYPE>("api/HisSurgServiceType/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisMediStock != null)
                    {
                        LoaddataToTreeList(this);
                        notHandler = true;
                    }

                    btnEdit.Enabled = false;
                    MessageManager.Show(this.ParentForm, param, notHandler);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_SURG_SERVICE_TYPE hisMediStock = new V_HIS_SURG_SERVICE_TYPE();
            bool notHandler = false;
            try
            {
                WaitingManager.Show();
                var data = TreeListSurgSer.GetDataRecordByNode(TreeListSurgSer.FocusedNode);
                V_HIS_SURG_SERVICE_TYPE dataMediStock = data as V_HIS_SURG_SERVICE_TYPE;
                if (MessageBox.Show(String.Format(ResourceMessage.MoKhoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_SURG_SERVICE_TYPE data1 = new V_HIS_SURG_SERVICE_TYPE();
                    data1.ID = dataMediStock.ID;
                    WaitingManager.Show();
                    hisMediStock = new BackendAdapter(param).Post<V_HIS_SURG_SERVICE_TYPE>("api/HisSurgServiceType/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisMediStock != null)
                    {
                        LoaddataToTreeList(this);
                        btnEdit.Enabled = false;
                        notHandler = true;
                    }
                    MessageManager.Show(this.ParentForm, param, notHandler);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TreeListSurgSer_Click(object sender, EventArgs e)
        {
            try
            {
                var data = TreeListSurgSer.GetDataRecordByNode(TreeListSurgSer.FocusedNode);
                V_HIS_SURG_SERVICE_TYPE rowData = data as V_HIS_SURG_SERVICE_TYPE;
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

        private void TreeListSurgSer_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = TreeListSurgSer.GetDataRecordByNode(e.Node);
                if (data != null && data is V_HIS_SURG_SERVICE_TYPE)
                {
                    V_HIS_SURG_SERVICE_TYPE rowData = data as V_HIS_SURG_SERVICE_TYPE;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (rowData.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnlock : btnLock);

                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IS_ACTIVE_TRUE)
                                e.RepositoryItem = btnDelete;
                            else
                                e.RepositoryItem = btnDelete_Disable;
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

        private void TreeListSurgSer_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    V_HIS_SURG_SERVICE_TYPE pData = e.Row as V_HIS_SURG_SERVICE_TYPE;
                    if (pData == null || this.TreeListSurgSer == null) return;
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
                    else if (e.Column.FieldName == "IS_LEAF_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_MULTI_REQUEST == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi cho phep nhap chi dinh lon hon 1 IS_LEAF_STR", ex);
                        }
                    }

                    else if (e.Column.FieldName == "CPNG")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_OUT_PARENT_FEE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi CPNG", ex);
                        }
                    }
                    else if (e.Column.FieldName == "BILL_OTPION_STR")
                    {
                        try
                        {
                            e.Value = pData.BILL_OPTION;
                            if (pData.BILL_OPTION == null)
                            {
                                e.Value = "Hóa đơn thường";
                            }
                            else if (pData.BILL_OPTION == 1)
                            {
                                e.Value = "Tách tiền chênh lệch vào hóa đơn dịch vụ";
                            }
                            else if (pData.BILL_OPTION == 2)
                            {
                                e.Value = "Hóa đơn dịch vụ";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("BILL_OTPION_STR", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var data = TreeListSurgSer.GetDataRecordByNode(TreeListSurgSer.FocusedNode);
                V_HIS_SURG_SERVICE_TYPE rowData = data as V_HIS_SURG_SERVICE_TYPE;
                if (MessageBox.Show(String.Format(ResourceMessage.XoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisSurgServiceType/Delete", ApiConsumers.MosConsumer, rowData, param);
                        if (success)
                        {
                            LoaddataToTreeList(this);
                        }
                        MessageManager.Show(this, param, success);
                    }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public static void LoadICDCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.LookUpEdit cboICDCM, DevExpress.XtraEditors.TextEdit txtICDCM, DevExpress.XtraEditors.GridLookUpEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboICDCM.EditValue = null;
                    cboICDCM.Focus();
                    cboICDCM.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                }
                else
                {
                    //var data = HisDataLocalStore.HisCareers.Where(o => o.CAREER_CODE.Contains(searchCode)).ToList();
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD_CM>().Where(o => o.ICD_CM_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboICDCM.EditValue = data[0].ID;
                            txtICDCM.Text = data[0].ICD_CM_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                        }
                        else
                        {
                            cboICDCM.EditValue = null;
                            cboICDCM.Focus();
                            cboICDCM.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                        }
                    }
                    else
                    {
                        cboICDCM.EditValue = null;
                        cboICDCM.Focus();
                        cboICDCM.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboNgheNghiep);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtICD_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    if (txtICD.Text != null)
                    {
                        string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                        LoadICDCombo(strValue, false, cboICD, txtICD, cboMethod);
                        //cboRh.Focus();
                    }
                    else
                    {
                        if (cboMethod.EditValue != null)
                        {
                            cboMethod.Focus();
                        }
                        else
                        {
                            cboMethod.Focus();
                            cboMethod.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboICD_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboICD.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_ICD_CM rh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD_CM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboICD.EditValue.ToString()));
                        if (rh != null)
                        {
                            txtICD.Text = rh.ICD_CM_CODE;
                            //cboRh.EditValue = rh.ID;
                            cboMethod.Focus();
                        }
                    }
                    else
                    {
                        if (cboMethod.EditValue != null)
                        {
                            cboMethod.Focus();
                        }
                        else
                        {
                            cboMethod.Focus();
                            cboMethod.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboICD_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadICDCombo(strValue, false, cboICD, txtICD, cboMethod);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboICDCM()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ICD_CM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ICD_CM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_CM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboICD, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD_CM>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void initComboBill()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Hóa đơn thường"));
                status.Add(new Status(2, "Tách tiền chênh lệch vào hóa đơn dịch vụ"));
                status.Add(new Status(3, "Hóa đơn dịch vụ"));

                List<Inventec.Common.Controls.EditorLoader.ColumnInfo> columnInfos = new List<Inventec.Common.Controls.EditorLoader.ColumnInfo>();
                columnInfos.Add(new Inventec.Common.Controls.EditorLoader.ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboHoaDon, status, controlEditorADO);
                cboHoaDon.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spDinhMuc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spTime.Focus();
                    spTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkPtttGroupId_Closed_1(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboHoaDon.Focus();
                    cboHoaDon.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCha_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCha.EditValue == null)
                    {
                        cboCha.ShowPopup();
                    }
                    else
                    {
                        txtICD.Focus();
                        txtICD.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkPtttGroupId_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lkPtttGroupId.EditValue == null)
                    {
                        lkPtttGroupId.Focus();
                    }
                    else
                    {
                        if (lkPtttGroupId.EditValue == "")
                        {
                            lkPtttGroupId.EditValue = null;
                            lkPtttGroupId.ShowPopup();
                        }
                        else
                        {
                            cboHoaDon.Focus();
                            cboHoaDon.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHoaDon_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (btnAdd.Enabled == true)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboLoaiDV.Focus();
                    cboLoaiDV.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPatientType.EditValue == null)
                    {
                        cboPatientType.ShowPopup();
                    }
                    else
                    {
                        cboLoaiDV.Focus();
                        cboLoaiDV.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHoaDon_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHoaDon.EditValue == null)
                    {
                        cboHoaDon.ShowPopup();
                    }
                    else
                    {
                        if (cboHoaDon.EditValue == "")
                        {
                            cboHoaDon.EditValue = null;
                            cboHoaDon.ShowPopup();
                        }
                        else
                        {
                            if (btnAdd.Enabled == true)
                            {
                                btnAdd.Focus();
                            }
                            else
                            {
                                btnEdit.Focus();
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

        private void cboLoaiDV_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboLoaiDV.Properties.Buttons[1].Visible = true;
                    cboLoaiDV.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCha_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCha.Properties.Buttons[1].Visible = true;
                    cboCha.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkPtttGroupId_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    lkPtttGroupId.Properties.Buttons[1].Visible = true;
                    lkPtttGroupId.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHoaDon_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHoaDon.Properties.Buttons[1].Visible = true;
                    cboHoaDon.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    spGia.Focus();
                    spGia.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (cboMethod.EditValue != null)
                {
                    spGia.Focus();
                    spGia.SelectAll();
                }
                else
                {
                    cboMethod.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void chkOutPack_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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


        private void cboLoaiBH_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboLoaiBH.Properties.Buttons[1].Visible = true;
                    cboLoaiBH.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiBH_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboCha.Focus();
                    cboCha.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiBH_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLoaiBH.EditValue == null)
                    {
                        cboLoaiBH.Focus();
                        cboLoaiBH.ShowPopup();
                    }
                    else
                    {
                        cboCha.Focus();
                        cboCha.ShowPopup();
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
