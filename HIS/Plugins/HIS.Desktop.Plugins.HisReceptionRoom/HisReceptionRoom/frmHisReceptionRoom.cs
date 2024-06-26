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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisReceptionRoom.Validation;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utilities.Extensions;
using System.Text;
using DevExpress.XtraEditors.Repository;
using ACS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisReceptionRoom
{
    public partial class frmHisReceptionRoom : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;

        List<HIS_PATIENT_TYPE> listPatientType;
        List<HIS_PATIENT_TYPE> bodyPatientTypeSelecteds;
        List<HIS_ROOM> hisroom = new List<HIS_ROOM>();
        #endregion

        #region Construct
        public frmHisReceptionRoom(Inventec.Desktop.Common.Modules.Module moduleData)
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
        private void frmHisReceptionRoom_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisReceptionRoom.Resources.Lang", typeof(HIS.Desktop.Plugins.HisReceptionRoom.frmHisReceptionRoom).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColReceptionRoomCode.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColReceptionRoomCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColReceptionRoomCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColReceptionRoomCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColReceptionRoomName.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColReceptionRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColReceptionRoomName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColReceptionRoomName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPause.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.chkPause.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAllowNoIcd.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.chkAllowNoIcd.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboArea.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.cboArea.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lkRoomTypeId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.lkRoomTypeId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSampleRoomCode.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.lciSampleRoomCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSampleRoomName.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.lciSampleRoomName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciRoomTypeId.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.lciRoomTypeId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.layoutControlItem15.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.layoutControlItem16.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisReceptionRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                txtKeyword.Text = "";
                chkIsRestrictExecuteRoom.Checked = false;
                chkAllowNoIcd.Checked = false;
                cboPatientType.EditValue = null;
                ResetFormData();
                EnableControlChanged(this.ActionType);
                //cboPatientType.Properties.Buttons[1].Visible = false;

                if (cboSCREEN.EditValue != null)
                {
                    cboSCREEN.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboSCREEN.Properties.Buttons[1].Visible = false;
                }

                GridCheckMarksSelection gridCheckMarkPart = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                try
                {
                    gridCheckMarkPart.ClearSelection(cboPatientType.Properties.View);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug(ex);
                }

                cboPatientType.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
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
                //dicOrderTabIndexControl.Add("txtReceptionRoomCode", 0);
                //dicOrderTabIndexControl.Add("txtReceptionRoomName", 1);
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
                //InitComboAreaId();
                InitComboDepartmentId();
                InitComboPatientType();
                InitComboCashierRoom();
                InitComboDepositAccountBook();
                InitComboSCREEN();
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCashierRoom()
        {
            try
            {
                HIS_DEPARTMENT currentDepartment = new HIS_DEPARTMENT();
                if (cboDepartment.EditValue != null)
                {
                    currentDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "0").ToString())).FirstOrDefault();
                }
                var data = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.BRANCH_ID == currentDepartment.BRANCH_ID).ToList();
                Inventec.Common.Logging.LogSystem.Debug("InitComboCashierRoom(): data.Count" + data.Count);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CASHIER_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCashierRoom, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDepositAccountBook()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_ACCOUNT_BOOK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_FOR_DEPOSIT == 1).ToList();
                Inventec.Common.Logging.LogSystem.Debug("InitComboDepositAccountBook(): data.Count" + data.Count);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDepositAccountBook, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboSCREEN()
        {
            try
            {
                var data = BackendDataWorker.Get<ACS_MODULE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var data_ = BackendDataWorker.Get<ACS_MODULE_GROUP>().Where(o => o.MODULE_GROUP_CODE == "MHC").ToList();
                if (data_ != null && data_.Count > 0)
                {
                    List<ACS_MODULE> lst = new List<ACS_MODULE>();
                    foreach (ACS_MODULE item in data)
                    {
                        foreach (var item_ in data_)
                        {
                            if (item.MODULE_GROUP_ID == item_.ID)
                            {
                                lst.Add(item);
                            }
                        }
                    }
                    if (lst != null && lst.Count > 0)
                    {
                        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                        columnInfos.Add(new ColumnInfo("MODULE_NAME", "", 300, 1));

                        ControlEditorADO controlEditorADO = new ControlEditorADO("MODULE_NAME", "MODULE_LINK", columnInfos, false, 300);
                        ControlEditorLoader.Load(cboSCREEN, lst, controlEditorADO);
                    }
                }
               

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboPatientType()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboPatientType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboPatientType);
                cboPatientType.Properties.Tag = gridCheck;
                cboPatientType.Properties.View.OptionsSelection.MultiSelect = true;

                CommonParam param = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                listPatientType = new List<HIS_PATIENT_TYPE>();
                listPatientType = data;
                if (listPatientType != null)
                {
                    cboPatientType.Properties.DataSource = listPatientType;
                    cboPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                    cboPatientType.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboPatientType.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                    col2.VisibleIndex = 1;
                    col2.Width = 100;
                    col2.Caption = "";
                    DevExpress.XtraGrid.Columns.GridColumn col3 = cboPatientType.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                    col3.VisibleIndex = 2;
                    col3.Width = 200;
                    col3.Caption = "";

                    cboPatientType.Properties.PopupFormWidth = 200;
                    cboPatientType.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboPatientType.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboPatientType.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboPatientType(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                bodyPatientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                if (gridCheckMark != null)
                {
                    foreach (HIS_PATIENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.PATIENT_TYPE_NAME + ",";
                    }

                    cboPatientType.Text = typeName;
                    cboPatientType.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboRoomTypeId()
        {
            //try
            //{
            //    CommonParam param = new CommonParam();
            //    HisRoomTypeFilter filter = new HisRoomTypeFilter();
            //    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
            //    var data = new BackendAdapter(param).Get<List<HIS_ROOM_TYPE>>("api/HisRoomType/Get", ApiConsumers.MosConsumer, filter, null).ToList();
            //    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            //    columnInfos.Add(new ColumnInfo("ROOM_TYPE_CODE", "", 100, 1));
            //    columnInfos.Add(new ColumnInfo("ROOM_TYPE_NAME", "", 250, 2));
            //    ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_TYPE_NAME", "ID", columnInfos, false, 350);
            //    ControlEditorLoader.Load(lkRoomTypeId, data, controlEditorADO);
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void InitComboDepartmentId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDepartment, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboAreaId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisAreaFilter filter = new HisAreaFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //filter.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "0").ToString());
                //var area = BackendDataWorker.Get<HIS_AREA>().Where(o => o.DEPARTMENT_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "0").ToString()) && o.IS_ACTIVE ==1).ToList();
                var data = new BackendAdapter(param).Get<List<HIS_AREA>>("api/HisArea/Get", ApiConsumers.MosConsumer, filter, null).Where(o => o.DEPARTMENT_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "0").ToString()) || o.DEPARTMENT_ID == null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("AREA_CODE", "", 10, 1));
                columnInfos.Add(new ColumnInfo("AREA_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("AREA_NAME", "ID", columnInfos, false, 110);
                ControlEditorLoader.Load(cboArea, data, controlEditorADO);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM>> apiResult = null;
                HisReceptionRoomViewFilter filter = new HisReceptionRoomViewFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM>>(HisRequestUriStore.MOSHIS_RECEPTION_ROOM_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Error("" + Inventec.Common.Logging.LogUtil.TraceData("DATA_____________________________" + Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));

                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM>)apiResult.Data;
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

        private void SetFilterNavBar(ref HisReceptionRoomViewFilter filter)
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
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM)gridviewFormList.GetFocusedRow();
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
                    MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM pData = (MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    else if (e.Column.FieldName == "IS_PAUSE_CHECK")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_PAUSE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la IS_PAUSE ", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_ALLOW_NO_ICD_CHK")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_ALLOW_NO_ICD == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la IS_ALLOW_NO_ICDK", ex);
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
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM)gridviewFormList.GetFocusedRow();
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
                this.currentData = (MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM>)[dnNavigation.Position];
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM data)
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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM data)
        {
            try
            {
                if (data != null)
                {
                    txtReceptionRoomCode.Text = data.RECEPTION_ROOM_CODE;
                    txtReceptionRoomName.Text = data.RECEPTION_ROOM_NAME;
                    //lkRoomTypeId.EditValue = data.ROOM_TYPE_ID;
                    cboDepartment.EditValue = data.DEPARTMENT_ID;
                    GridCheckMarksSelection gridCheckMarkPatientType = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkPatientType.ClearSelection(cboPatientType.Properties.View);

                    GridCheckMarksSelection gridCheckMark = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                    if (!String.IsNullOrWhiteSpace(data.PATIENT_TYPE_IDS) && cboPatientType.Properties.Tag != null)
                    {
                        ProcessSelectBusiness(data.PATIENT_TYPE_IDS, gridCheckMark);
                    }

                    cboArea.EditValue = data.AREA_ID;
                    cboCashierRoom.EditValue = data.DEFAULT_CASHIER_ROOM_ID;
                    cboDepositAccountBook.EditValue = data.DEPOSIT_ACCOUNT_BOOK_ID;
                    chkPause.Checked = (data.IS_PAUSE == 1 ? true : false);
                    chkIsRestrictExecuteRoom.Checked = (data.IS_RESTRICT_EXECUTE_ROOM == 1 ? true : false);
                    chkAllowNoIcd.Checked = (data.IS_ALLOW_NO_ICD == 1 ? true : false);
                    //hisroom = BackendDataWorker.Get<HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();


                    CommonParam param = new CommonParam();
                    HisRoomFilter filter = new HisRoomFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.ID = data.ROOM_ID;
                    hisroom = new BackendAdapter(param).Get<List<HIS_ROOM>>("api/HisRoom/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                    if (hisroom != null && hisroom.Count > 0)
                    {
                        cboSCREEN.EditValue = hisroom.FirstOrDefault().SCREEN_SAVER_MODULE_LINK;
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
                            txtReceptionRoomCode.Focus();
                            txtReceptionRoomCode.SelectAll();
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                    cboCashierRoom.Properties.DataSource = null;
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_RECEPTION_ROOM currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisReceptionRoomFilter filter = new HisReceptionRoomFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_RECEPTION_ROOM>>(HisRequestUriStore.MOSHIS_RECEPTION_ROOM_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                txtReceptionRoomCode.ReadOnly = !(action == GlobalVariables.ActionAdd);
                cboDepartment.ReadOnly = !(action == GlobalVariables.ActionAdd);
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
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM)gridviewFormList.GetFocusedRow();
                CommonParam param = new CommonParam();
                HisReceptionRoomFilter filter = new HisReceptionRoomFilter();
                filter.ID = rowData.ID;
                var data = new BackendAdapter(param).Get<System.Collections.Generic.List<HIS_RECEPTION_ROOM>>(HisRequestUriStore.MOSHIS_RECEPTION_ROOM_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (rowData != null)
                {
                    bool success = false;

                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_RECEPTION_ROOM_DELETE, ApiConsumers.MosConsumer, data, param);
                    if (success)
                    {
                        FillDataToGridControl();
                        currentData = ((List<V_HIS_RECEPTION_ROOM>)gridControlFormList.DataSource).FirstOrDefault();
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

                HisReceptionRoomSDO sdo = new HisReceptionRoomSDO();
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_RECEPTION_ROOM updateDTO = new MOS.EFMODEL.DataModels.HIS_RECEPTION_ROOM();
                HIS_ROOM room = new HIS_ROOM();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                sdo.HisReceptionRoom = updateDTO;

                room.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TD;

                if (cboDepartment.EditValue != null) room.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "0").ToString());

                if (cboArea.EditValue != null) room.AREA_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboArea.EditValue ?? "0").ToString());
                if (cboCashierRoom.EditValue != null)
                {
                    room.DEFAULT_CASHIER_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboCashierRoom.EditValue ?? "0").ToString());
                }
                if (cboDepositAccountBook.EditValue != null)
                {
                    room.DEPOSIT_ACCOUNT_BOOK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDepositAccountBook.EditValue ?? "0").ToString());
                }
                if (chkPause.EditValue != null) room.IS_PAUSE = (short)(chkPause.Checked ? 1 : 0);
                if (chkAllowNoIcd.EditValue != null) room.IS_ALLOW_NO_ICD = (short)(chkAllowNoIcd.Checked ? 1 : 0);
                if (chkIsRestrictExecuteRoom.EditValue != null) room.IS_RESTRICT_EXECUTE_ROOM = (short)(chkIsRestrictExecuteRoom.Checked ? 1 : 0);

                if (cboSCREEN.EditValue != null)
                {
                    room.SCREEN_SAVER_MODULE_LINK = cboSCREEN.EditValue.ToString();
                }
                else
                {
                    room.SCREEN_SAVER_MODULE_LINK = null;
                }

                sdo.HisRoom = room;

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_RECEPTION_ROOM>(HisRequestUriStore.MOSHIS_RECEPTION_ROOM_CREATE, ApiConsumers.MosConsumer, sdo, param);

                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    sdo.HisRoom.ID = currentData.ROOM_ID;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_RECEPTION_ROOM>(HisRequestUriStore.MOSHIS_RECEPTION_ROOM_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                    if (resultData != null)
                    {
                        success = true;
                        //UpdateRowDataAfterEdit(resultData);
                        FillDataToGridControl();
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM) is null");
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_RECEPTION_ROOM currentDTO)
        {
            try
            {
                currentDTO.RECEPTION_ROOM_CODE = txtReceptionRoomCode.Text.Trim();
                currentDTO.RECEPTION_ROOM_NAME = txtReceptionRoomName.Text.Trim();
                //if (lkRoomTypeId.EditValue != null) currentDTO.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkRoomTypeId.EditValue ?? "0").ToString());
                if (!string.IsNullOrEmpty(cboPatientType.Text.ToString()))
                {
                    GridCheckMarksSelection gridCheckMarkBusiness = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusiness != null && gridCheckMarkBusiness.SelectedCount > 0)
                    {
                        List<string> codes = new List<string>();
                        foreach (HIS_PATIENT_TYPE rv in gridCheckMarkBusiness.Selection)
                        {
                            if (rv != null && !codes.Contains(rv.ID.ToString()))
                                codes.Add(rv.ID.ToString());
                        }

                        currentDTO.PATIENT_TYPE_IDS = String.Join(",", codes);
                    }
                }
                else
                {
                    currentDTO.PATIENT_TYPE_IDS = null;
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
                ValidationtxtReceptionRoomCode();
                ValidationSingleControl(txtReceptionRoomName);
                //ValidationSingleControl(lkRoomTypeId);
                ValidationSingleControl(cboDepartment);
                //ValidationSingleControl(cboArea);
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

        private void ValidationtxtReceptionRoomCode()
        {
            ValidMaxlengthtxtReceptionRoomCode validRule = new ValidMaxlengthtxtReceptionRoomCode();
            validRule.txtReceptionRoomCode = this.txtReceptionRoomCode;
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProviderEditorInfo.SetValidationRule(txtReceptionRoomCode, validRule);

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
            HIS_RECEPTION_ROOM success = new HIS_RECEPTION_ROOM();
            //bool notHandler = false;
            try
            {

                V_HIS_RECEPTION_ROOM data = (V_HIS_RECEPTION_ROOM)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_RECEPTION_ROOM data1 = new HIS_RECEPTION_ROOM();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_RECEPTION_ROOM>(HisRequestUriStore.MOSHIS_RECEPTION_ROOM_CHANGELOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
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
            HIS_RECEPTION_ROOM success = new HIS_RECEPTION_ROOM();
            //bool notHandler = false;
            try
            {

                V_HIS_RECEPTION_ROOM data = (V_HIS_RECEPTION_ROOM)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_RECEPTION_ROOM data1 = new HIS_RECEPTION_ROOM();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_RECEPTION_ROOM>(HisRequestUriStore.MOSHIS_RECEPTION_ROOM_CHANGELOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
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

                    V_HIS_RECEPTION_ROOM data = (V_HIS_RECEPTION_ROOM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLock : Lock);

                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : Delete);

                    }
                    else if (e.Column.FieldName == "RestrictExecuteRoom")
                    {
                        try
                        {
                            if (data.IS_ACTIVE == IS_ACTIVE_TRUE && data.IS_RESTRICT_EXECUTE_ROOM == 1)
                                e.RepositoryItem = Btn_RestrictExecuteRoom_Enable;
                            else
                                e.RepositoryItem = Btn_RestrictExecuteRoom_Disable;

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

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_RECEPTION_ROOM data = (V_HIS_RECEPTION_ROOM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            e.Appearance.ForeColor = Color.Red;
                        else
                            e.Appearance.ForeColor = Color.Green;
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
                    txtReceptionRoomName.Focus();
                    txtReceptionRoomName.SelectAll();
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
                    cboDepartment.Focus();
                    if (cboDepartment.EditValue == null)
                    {
                        cboDepartment.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void lkRoomTypeId_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepartment.Focus();
                    if (cboDepartment.EditValue == null)
                    {
                        cboDepartment.ShowPopup();
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
                    cboArea.Focus();
                    if (cboArea.EditValue == null)
                    {
                        cboArea.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }
        private void cboArea_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboCashierRoom.Focus();
                    if (cboCashierRoom.EditValue == null)
                    {
                        cboCashierRoom.ShowPopup();
                    }
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkPause_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkPause.Checked = !chkPause.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsRestrictExecuteRoom.Focus();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void Btn_RestrictExecuteRoom_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_RECEPTION_ROOM)gridviewFormList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    CallModule callModule = new CallModule(CallModule.HisExroRoom, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    this.Hide();

                    this.ParentForm.WindowState = FormWindowState.Normal;

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboDepartment.Text.Trim()))
            {
                InitComboAreaId();
                InitComboCashierRoom();
                if (cboCashierRoom == null)
                {
                    cboCashierRoom.Properties.Buttons[1].Visible = false;
                }
            }
        }

        private void chkIsRestrictExecuteRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkIsRestrictExecuteRoom.Checked = !chkIsRestrictExecuteRoom.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    chkAllowNoIcd.Focus();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkAllowNoIcd_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkAllowNoIcd.Checked = !chkAllowNoIcd.Checked;
                }
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
                Inventec.Common.Logging.LogSession.Warn(ex);
            }

        }

        private void cboArea_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboArea.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboArea_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboArea.EditValue != null)
                {
                    cboArea.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboArea.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkPause.Focus();
                }//e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboPatientType.Properties.View);
                    cboPatientType.EditValue = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSelectBusiness(string p, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_PATIENT_TYPE> ds = cboPatientType.Properties.DataSource as List<HIS_PATIENT_TYPE>;
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_PATIENT_TYPE> selects = new List<HIS_PATIENT_TYPE>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID.ToString() == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashierRoom_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCashierRoom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashierRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCashierRoom.EditValue != null)
                {
                    cboCashierRoom.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboCashierRoom.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashierRoom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboCashierRoom.EditValue != null)
                    {
                        cboCashierRoom.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        cboCashierRoom.Properties.Buttons[1].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashierRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepositAccountBook.Focus();
                    if (cboDepositAccountBook.EditValue == null)
                    {
                        cboDepositAccountBook.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositAccountBook_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboSCREEN.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewComboCashierRoom_CustomDrawEmptyForeground(object sender, CustomDrawEventArgs e)
        {
            if (cboDepartment.EditValue == null)
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (view.RowCount != 0) return;

                StringFormat drawFormat = new StringFormat();

                drawFormat.Alignment = StringAlignment.Center;
                drawFormat.LineAlignment = StringAlignment.Center;

                //e.Appearance.Font
                Font font = new Font("Arial", 12);
                e.Graphics.DrawString("Chưa chọn khoa phòng!", font, SystemBrushes.ControlDark, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), drawFormat);
            }
            else
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (view.RowCount != 0) return;

                StringFormat drawFormat = new StringFormat();

                drawFormat.Alignment = StringAlignment.Center;
                drawFormat.LineAlignment = StringAlignment.Center;

                //e.Appearance.Font
                Font font = new Font("Arial", 12);
                e.Graphics.DrawString("Không có dữ liệu!", font, SystemBrushes.ControlDark, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), drawFormat);
            }
        }

        private void gridViewComboDepositAccountBook_CustomDrawEmptyForeground(object sender, CustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view.RowCount != 0) return;

            StringFormat drawFormat = new StringFormat();

            drawFormat.Alignment = StringAlignment.Center;
            drawFormat.LineAlignment = StringAlignment.Center;

            //e.Appearance.Font
            Font font = new Font("Arial", 12);
            e.Graphics.DrawString("Không có dữ liệu!", font, SystemBrushes.ControlDark, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), drawFormat);
        }

        private void cboDepositAccountBook_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboDepositAccountBook.EditValue != null)
                    {
                        cboDepositAccountBook.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        cboDepositAccountBook.Properties.Buttons[1].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositAccountBook_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepositAccountBook.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDepositAccountBook.EditValue != null)
                {
                    cboDepositAccountBook.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboDepositAccountBook.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSCREEN_KeyUp(object sender, KeyEventArgs e)
        {

            try
            {
                try
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        chkPause.Focus();
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

        private void cboSCREEN_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSCREEN.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSCREEN_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboSCREEN.EditValue != null)
                    {
                        cboSCREEN.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        cboSCREEN.Properties.Buttons[1].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSCREEN_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                try
                {
                    if (cboSCREEN.EditValue != null)
                    {
                        cboSCREEN.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        cboSCREEN.Properties.Buttons[1].Visible = false;
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
    }
}
