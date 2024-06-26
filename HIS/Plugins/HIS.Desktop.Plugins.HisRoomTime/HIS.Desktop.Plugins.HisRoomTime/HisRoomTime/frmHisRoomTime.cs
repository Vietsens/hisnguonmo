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
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Plugins.HisRoomTime.Helpers;

namespace HIS.Desktop.Plugins.HisRoomTime
{
    public partial class frmHisRoomTime : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME currentData;
        List<HIS_ROOM_TIME> currentListData = new List<HIS_ROOM_TIME>();
        List<HourADO> hourAdo;
        List<DayADO> dayAdo;
        List<DayADO> _DaySelecteds;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        V_HIS_EXECUTE_ROOM currentExecuteRoom;
        
        #endregion

        #region Construct
        public frmHisRoomTime(Inventec.Desktop.Common.Modules.Module moduleData)
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

        public frmHisRoomTime(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_EXECUTE_ROOM _executeRoom)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                this.currentExecuteRoom = _executeRoom;
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
        private void frmHisRoomTime_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisRoomTime.Resources.Lang", typeof(HIS.Desktop.Plugins.HisRoomTime.frmHisRoomTime).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmHisRoomTime.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisRoomTime.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColSampleRoomCode.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColSampleRoomCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColSampleRoomCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColSampleRoomCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColSampleRoomName.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColSampleRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColSampleRoomName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColSampleRoomName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisRoomTime.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisRoomTime.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisRoomTime.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisRoomTime.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisRoomTime.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisRoomTime.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lkRoomTypeId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisRoomTime.lkRoomTypeId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciRoomTypeId.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.lciRoomTypeId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciDay.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciRoomType.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisRoomTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void ResetCombo(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                    cbo.Enabled = false;
                    cbo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                _DaySelecteds = new List<DayADO>();
                foreach (DayADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _DaySelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    //gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
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
                //dicOrderTabIndexControl.Add("txtSampleRoomCode", 0);
                //dicOrderTabIndexControl.Add("txtSampleRoomName", 1);
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
                InitComboRoomTypeSearch();
                InitComboRoomTypeId();
                InitComboRoom();
                //InitComboDay();
                LoadComboStatusHour();
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboRoomTypeId()
        {
            try
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM_TYPE>();
                data = data != null ? data.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboRoomType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRoomTypeSearch()
        {
            try
            {
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM_TYPE>();
                data = data != null ? data.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboRoomTypeSearch, data, controlEditorADO);
                cboRoomTypeSearch.EditValue = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (cboRoomType.EditValue != null) {
                    var roomTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomType.EditValue.ToString());
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>();
                    data = data != null ? data.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ROOM_TYPE_ID == roomTypeId).ToList() : null;
                    if (data != null && data.Count > 0)
                    {
                        data = data.Where(o => o.IS_RESTRICT_TIME == 1 || o.IS_RESTRICT_TIME != 1).ToList();
                    }

                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 350);
                    ControlEditorLoader.Load(cboRoom, data, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDay()
        {
            try
            {
                dayAdo = new List<DayADO>();

                for (short i = 1; i < 8; i++)
                {

                    DayADO ado = new DayADO();
                    ado.Day = i;
                    if (i != 1)
                    {
                        ado.Name = "Thứ " + i.ToString();
                    }
                    else
                    {
                        ado.Name = "Chủ nhật";
                    }
                    dayAdo.Add(ado);
                }
                ////CommonParam param = new CommonParam();
                ////HisDepartmentFilter filter = new HisDepartmentFilter();
                ////filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                ////var data = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("Name", "", 300, 1));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Day", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboDay, dayAdo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboStatusHour()
        {
            try
            {
                //Load dộng thời gian từ hàm cs hỗ trợ
                //dùng cú phspd truy vấn linq để khởi  tạo dữ liệu
                //hourAdo = new List<HourADO>();
                //hourAdo.Add(new HourADO(0000, "000000", "00:00 AM"));
                //hourAdo.Add(new HourADO(0015, "001500", "00:15 AM")); hourAdo.Add(new HourADO(1215, "121500", "12:15 PM"));
                //hourAdo.Add(new HourADO(0030, "003000", "00:30 AM")); hourAdo.Add(new HourADO(1230, "123000", "12:30 PM"));
                //hourAdo.Add(new HourADO(0045, "004500", "00:45 AM")); hourAdo.Add(new HourADO(1245, "124500", "12:45 PM"));
                //hourAdo.Add(new HourADO(0100, "010000", "1:00 AM")); hourAdo.Add(new HourADO(1300, "130000", "1:00 PM"));
                //hourAdo.Add(new HourADO(0115, "011500", "1:15 AM")); hourAdo.Add(new HourADO(1315, "131500", "1:15 PM"));
                //hourAdo.Add(new HourADO(0130, "013000", "1:30 AM")); hourAdo.Add(new HourADO(1330, "133000", "1:30 PM"));
                //hourAdo.Add(new HourADO(0145, "014500", "1:45 AM")); hourAdo.Add(new HourADO(1345, "134500", "1:45 PM"));
                //hourAdo.Add(new HourADO(0200, "020000", "2:00 AM")); hourAdo.Add(new HourADO(1400, "140000", "2:00 PM"));
                //hourAdo.Add(new HourADO(0215, "021500", "2:15 AM")); hourAdo.Add(new HourADO(1415, "141500", "2:15 PM"));
                //hourAdo.Add(new HourADO(0230, "023000", "2:30 AM")); hourAdo.Add(new HourADO(1430, "143000", "2:30 PM"));
                //hourAdo.Add(new HourADO(0245, "024500", "2:45 AM")); hourAdo.Add(new HourADO(1445, "144500", "2:45 PM"));
                //hourAdo.Add(new HourADO(0300, "030000", "3:00 AM")); hourAdo.Add(new HourADO(1500, "150000", "3:00 PM"));
                //hourAdo.Add(new HourADO(0315, "031500", "3:15 AM")); hourAdo.Add(new HourADO(1515, "151500", "3:15 PM"));
                //hourAdo.Add(new HourADO(0330, "033000", "3:30 AM")); hourAdo.Add(new HourADO(1530, "153000", "3:30 PM"));
                //hourAdo.Add(new HourADO(0345, "034500", "3:45 AM")); hourAdo.Add(new HourADO(1545, "154500", "3:45 PM"));
                //hourAdo.Add(new HourADO(0400, "040000", "4:00 AM")); hourAdo.Add(new HourADO(1600, "160000", "4:00 PM"));
                //hourAdo.Add(new HourADO(0415, "041500", "4:15 AM")); hourAdo.Add(new HourADO(1615, "161500", "4:15 PM"));
                //hourAdo.Add(new HourADO(0430, "043000", "4:30 AM")); hourAdo.Add(new HourADO(1630, "163000", "4:30 PM"));
                //hourAdo.Add(new HourADO(0445, "044500", "4:45 AM")); hourAdo.Add(new HourADO(1645, "164500", "4:45 PM"));
                //hourAdo.Add(new HourADO(0500, "050000", "5:00 AM")); hourAdo.Add(new HourADO(1700, "170000", "5:00 PM"));
                //hourAdo.Add(new HourADO(0515, "051500", "5:15 AM")); hourAdo.Add(new HourADO(1715, "171500", "5:15 PM"));
                //hourAdo.Add(new HourADO(0530, "053000", "5:30 AM")); hourAdo.Add(new HourADO(1730, "173000", "5:30 PM"));
                //hourAdo.Add(new HourADO(0545, "054500", "5:45 AM")); hourAdo.Add(new HourADO(1745, "174500", "5:45 PM"));
                //hourAdo.Add(new HourADO(0600, "060000", "6:00 AM")); hourAdo.Add(new HourADO(1800, "180000", "6:00 PM"));
                //hourAdo.Add(new HourADO(0615, "061500", "6:15 AM")); hourAdo.Add(new HourADO(1815, "181500", "6:15 PM"));
                //hourAdo.Add(new HourADO(0630, "063000", "6:30 AM")); hourAdo.Add(new HourADO(1830, "183000", "6:30 PM"));
                //hourAdo.Add(new HourADO(0645, "064500", "6:45 AM")); hourAdo.Add(new HourADO(1845, "184500", "6:45 PM"));
                //hourAdo.Add(new HourADO(0700, "070000", "7:00 AM")); hourAdo.Add(new HourADO(1900, "190000", "7:00 PM"));
                //hourAdo.Add(new HourADO(0715, "071500", "7:15 AM")); hourAdo.Add(new HourADO(1915, "191500", "7:15 PM"));
                //hourAdo.Add(new HourADO(0730, "073000", "7:30 AM")); hourAdo.Add(new HourADO(1930, "193000", "7:30 PM"));
                //hourAdo.Add(new HourADO(0745, "074500", "7:45 AM")); hourAdo.Add(new HourADO(1945, "194500", "7:45 PM"));
                //hourAdo.Add(new HourADO(0800, "080000", "8:00 AM")); hourAdo.Add(new HourADO(2000, "200000", "8:00 PM"));
                //hourAdo.Add(new HourADO(0815, "081500", "8:15 AM")); hourAdo.Add(new HourADO(2015, "201500", "8:15 PM"));
                //hourAdo.Add(new HourADO(0830, "083000", "8:30 AM")); hourAdo.Add(new HourADO(2030, "203000", "8:30 PM"));
                //hourAdo.Add(new HourADO(0845, "084500", "8:45 AM")); hourAdo.Add(new HourADO(2045, "204500", "8:45 PM"));
                //hourAdo.Add(new HourADO(0900, "090000", "9:00 AM")); hourAdo.Add(new HourADO(2100, "210000", "9:00 PM"));
                //hourAdo.Add(new HourADO(0915, "091500", "9:15 AM")); hourAdo.Add(new HourADO(2115, "211500", "9:15 PM"));
                //hourAdo.Add(new HourADO(0930, "093000", "9:30 AM")); hourAdo.Add(new HourADO(2130, "213000", "9:30 PM"));
                //hourAdo.Add(new HourADO(0945, "094500", "9:45 AM")); hourAdo.Add(new HourADO(2145, "214500", "9:45 PM"));
                //hourAdo.Add(new HourADO(1000, "100000", "10:00 AM")); hourAdo.Add(new HourADO(2200, "220000", "10:00 PM"));
                //hourAdo.Add(new HourADO(1015, "101500", "10:15 AM")); hourAdo.Add(new HourADO(2215, "221500", "10:15 PM"));
                //hourAdo.Add(new HourADO(1030, "103000", "10:30 AM")); hourAdo.Add(new HourADO(2230, "223000", "10:30 PM"));
                //hourAdo.Add(new HourADO(1045, "104500", "10:45 AM")); hourAdo.Add(new HourADO(2245, "224500", "10:45 PM"));
                //hourAdo.Add(new HourADO(1100, "110000", "11:00 AM")); hourAdo.Add(new HourADO(2300, "230000", "11:00 PM"));
                //hourAdo.Add(new HourADO(1115, "111500", "11:15 AM")); hourAdo.Add(new HourADO(2315, "231500", "11:15 PM"));
                //hourAdo.Add(new HourADO(1130, "113000", "11:30 AM")); hourAdo.Add(new HourADO(2330, "233000", "11:30 PM"));
                //hourAdo.Add(new HourADO(1145, "114500", "11:45 AM")); hourAdo.Add(new HourADO(2345, "234500", "11:45 PM"));
                //hourAdo.Add(new HourADO(1200, "120000", "12:00 PM"));

                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("HourName", "", 200, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("HourName", "HourString", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboFromTime, hourAdo, controlEditorADO);
                //ControlEditorLoader.Load(cboToTime, hourAdo, controlEditorADO);
                //ControlEditorLoader.Load(cboHourTo, status, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
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
                LoadListHisRoomTime();

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize,this.gridControlFormList);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME>> apiResult = null;
                HisRoomTimeViewFilter filter = new HisRoomTimeViewFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME>>(HisRequestUriStore.MOSHIS_ROOM_TIME_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME>)apiResult.Data;
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

        private void SetFilterNavBar(ref HisRoomTimeViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
                if (this.currentExecuteRoom != null)
                {
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentExecuteRoom.ROOM_ID && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL);
                    if (room != null)
                    {
                        filter.ROOM_ID = room.ID;
                    }
                }

                if (cboRoomTypeSearch.EditValue != null)
                {
                    filter.ROOM_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomTypeSearch.EditValue.ToString());
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME)gridviewFormList.GetFocusedRow();
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
                    MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME pData = (MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                    else if (e.Column.FieldName == "FROM_TIME_STR")
                    {
                        try
                        {
                            string nowStrFr = Inventec.Common.DateTime.Get.Now().ToString();
                            string timeFr = nowStrFr.Substring(0, 8) + pData.FROM_TIME;

                            DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(timeFr));

                            if (dt != null)
                            {
                                e.Value = dt.Value.ToString("HH:mm:ss");
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }

                    }

                    else if (e.Column.FieldName == "TO_TIME_STR")
                    {
                        try
                        {
                            string nowStrTo = Inventec.Common.DateTime.Get.Now().ToString();
                            string timeTo = nowStrTo.Substring(0, 8) + pData.TO_TIME;

                            DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(timeTo));

                            if (dt != null)
                            {
                                e.Value = dt.Value.ToString("HH:mm:ss");
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }

                    else if (e.Column.FieldName == "DAY_STR")
                    {
                        try
                        {
                            if (dayAdo != null && dayAdo.Count > 0)
                            {
                                e.Value = dayAdo.FirstOrDefault(o => o.Day == pData.DAY).Name;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la ngoai dinh suat DAY_STR", ex);
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
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME)gridviewFormList.GetFocusedRow();
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
                this.currentData = (MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME>)[dnNavigation.Position];
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME data)
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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME data)
        {
            try
            {
                if (data != null)
                {
                    cboRoomType.EditValue = data.ROOM_TYPE_ID;
                    cboRoom.EditValue = data.ROOM_ID;
                    txtRoom.Text = data.ROOM_CODE;
                    txtName.Text = data.ROOM_TIME_NAME;
                    string nowStrFr = Inventec.Common.DateTime.Get.Now().ToString();
                    string timeFr = nowStrFr.Substring(0, 8) + data.FROM_TIME;
                    string nowStrTo = Inventec.Common.DateTime.Get.Now().ToString();
                    string timeTo = nowStrTo.Substring(0, 8) + data.TO_TIME;

                    dtFromTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(timeFr));
                    dtToTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64(timeTo));

                    ResetCombo(cboDay);

                    GridCheckMarksSelection gridCheckMarkDay = cboDay.Properties.Tag as GridCheckMarksSelection;

                    _DaySelecteds = new List<DayADO>();
                    _DaySelecteds = dayAdo.Where(o => o.Day == data.DAY).ToList();

                    if (_DaySelecteds != null && _DaySelecteds.Count > 0)
                    {
                        gridCheckMarkDay.SelectAll(_DaySelecteds);
                    }

                    cboDay.Enabled = false;
                    txtRoom.Enabled = false;
                    cboRoom.Enabled = false;

                    //dtFromTime.EditValue = data.FROM_TIME;
                    //dtToTime.EditValue = data.TO_TIME;
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_ROOM_TIME currentDTO)
        {
            try
            {
                currentDTO = this.currentListData.Where(o => o.ID == currentId).FirstOrDefault();
                if (currentDTO == null)
                {
                    LoadListHisRoomTime();
                    currentDTO = this.currentListData.Where(o => o.ID == currentId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadListHisRoomTime()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisRoomTimeFilter filter = new HisRoomTimeFilter();
                this.currentListData = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_ROOM_TIME>>(HisRequestUriStore.MOSHIS_ROOM_TIME_GET, ApiConsumers.MosConsumer, filter, param);
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
                btnCancel_Click(null, null);
                //SetDefaultValue();
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
                CommonParam param = new CommonParam();
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME)gridviewFormList.GetFocusedRow();
                HIS_ROOM_TIME data = new HIS_ROOM_TIME();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ROOM_TIME>(data, rowData);
                if (rowData != null)
                {
                    bool success = false;

                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_ROOM_TIME_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                    if (success)
                    {
                        FillDataToGridControl();
                        currentData = ((List<V_HIS_ROOM_TIME>)gridControlFormList.DataSource).FirstOrDefault();
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
                ResetCombo(cboDay);
                SetFocusEditor();
                txtRoom.Enabled = true;
                cboRoom.Enabled = true;
                cboDay.Enabled = true;
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

                if (dtFromTime.EditValue != null && dtToTime.EditValue != null && dtFromTime.DateTime > dtToTime.DateTime)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian từ không được lớn hơn thời gian đến", "Thông báo");
                    return;
                }



                MOS.EFMODEL.DataModels.HIS_ROOM_TIME updateDTO = new MOS.EFMODEL.DataModels.HIS_ROOM_TIME();
                //if (this.currentData != null && this.currentData.ID > 0)
                //{
                //    LoadCurrent(this.currentData.ID, ref updateDTO);
                //}

                if (this.ActionType == GlobalVariables.ActionAdd)
                {

                    if (_DaySelecteds == null || _DaySelecteds.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn thứ", "Thông báo");
                        return;
                    }

                    WaitingManager.Show();
                    List<HIS_ROOM_TIME> roomTimes=new List<HIS_ROOM_TIME>();
                    foreach (var item in _DaySelecteds)
                    {
                        HIS_ROOM_TIME roomTime = new HIS_ROOM_TIME();
                        roomTime.DAY = item.Day;
                        roomTime.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboRoom.EditValue.ToString() ?? "0");
                        if (dtFromTime.EditValue != null)
                            roomTime.FROM_TIME = dtFromTime.DateTime.ToString("HHmmsss");
                        else
                            roomTime.FROM_TIME = "000000";

                        if (dtToTime.EditValue != null)
                            roomTime.TO_TIME = dtToTime.DateTime.ToString("HHmmsss");
                        else
                            roomTime.TO_TIME = "235959";
                        roomTime.ROOM_TIME_NAME = txtName.Text.Trim();

                        roomTimes.Add(roomTime);
                    }

                    var resultData = new BackendAdapter(param).Post<List<MOS.EFMODEL.DataModels.HIS_ROOM_TIME>>("api/HisRoomTime/CreateList", ApiConsumers.MosConsumer, roomTimes, param);
                    if (resultData != null)
                    {
                        success = true;
                    }
                }
                else
                {
                    WaitingManager.Show();
                    if (this.currentData != null && this.currentData.ID > 0)
                    {
                        LoadCurrent(this.currentData.ID, ref updateDTO);
                    }
                    UpdateDTOFromDataForm(ref updateDTO);

                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ROOM_TIME>(HisRequestUriStore.MOSHIS_ROOM_TIME_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                    }
                }

                //if (ActionType == GlobalVariables.ActionAdd)
                //{
                //    updateDTO.IS_ACTIVE = 1;
                //    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ROOM_TIME>(HisRequestUriStore.MOSHIS_ROOM_TIME_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                //    if (resultData != null)
                //    {
                //        success = true;
                //        FillDataToGridControl();
                //        ResetFormData();
                //    }
                //}
                //else
                //{
                //    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ROOM_TIME>(HisRequestUriStore.MOSHIS_ROOM_TIME_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                //    if (resultData != null)
                //    {
                //        success = true;
                //        //UpdateRowDataAfterEdit(resultData);
                //        FillDataToGridControl();
                //    }
                //}

                if (success)
                {
                    FillDataToGridControl();
                    ResetFormData();
                    ResetCombo(cboDay);
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME) is null");
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_ROOM_TIME>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_ROOM_TIME currentDTO)
        {
            try
            {
                currentDTO.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboRoom.EditValue.ToString() ?? "0");
                if (dtFromTime.EditValue != null)
                    currentDTO.FROM_TIME = dtFromTime.DateTime.ToString("HHmmsss");
                else
                    currentDTO.FROM_TIME = "000000";

                if (dtToTime.EditValue != null)
                    currentDTO.TO_TIME = dtToTime.DateTime.ToString("HHmmsss");
                else
                    currentDTO.TO_TIME = "235959";
                currentDTO.ROOM_TIME_NAME = txtName.Text.Trim();
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
                ValidationSingleControl(cboRoomType);
                ValidationSingleControl(cboRoom);
                ValidateGridLookupWithTextEdit(cboRoom, txtRoom);
                //ValidationSingleControl(cboDay);
                ValidationControlTextEditRoomTimeName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditRoomTimeName()
        {
            try
            {
                TextEditValidationRule validRule = new TextEditValidationRule();
                validRule.txtControl = this.txtName;
                validRule.maxLength = 200;
                validRule.isRequired = false;
                dxValidationProviderEditorInfo.SetValidationRule(txtName, validRule);
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
                InitComboDay();
                InitCheck(cboDay, SelectionGrid__Status);
                InitCombo(cboDay, dayAdo, "Name", "Day");
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

                if (this.currentExecuteRoom != null)
                {
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentExecuteRoom.ROOM_ID && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL);
                    if (room != null)
                    {
                        cboRoomType.EditValue = room.ROOM_TYPE_ID;
                        cboRoom.EditValue = room.ID;
                        txtRoom.Text = room != null ? room.ROOM_CODE : "";
                    }
                }
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
                    btnEdit.Focus();
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
                {
                    btnAdd.Focus();
                    btnAdd_Click(null, null);
                }
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
                btnRefresh.Focus();
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
            HIS_ROOM_TIME success = new HIS_ROOM_TIME();
            //bool notHandler = false;
            try
            {

                V_HIS_ROOM_TIME data = (V_HIS_ROOM_TIME)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_ROOM_TIME data1 = new HIS_ROOM_TIME();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_ROOM_TIME>(HisRequestUriStore.MOSHIS_ROOM_TIME_CHANGELOCK, ApiConsumers.MosConsumer, data1.ID, param);
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
            HIS_ROOM_TIME success = new HIS_ROOM_TIME();
            //bool notHandler = false;
            try
            {

                V_HIS_ROOM_TIME data = (V_HIS_ROOM_TIME)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_ROOM_TIME data1 = new HIS_ROOM_TIME();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_ROOM_TIME>(HisRequestUriStore.MOSHIS_ROOM_TIME_CHANGELOCK, ApiConsumers.MosConsumer, data1.ID, param);
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

                    V_HIS_ROOM_TIME data = (V_HIS_ROOM_TIME)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLock : Lock);

                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : Delete);

                    }

                    if (e.Column.FieldName == "ConfigSTT")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGConfigSTT : btnGEmpty);
                        
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
                    V_HIS_ROOM_TIME data = (V_HIS_ROOM_TIME)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

        private void gridControlFormList_Click(object sender, EventArgs e)
        {

        }

        private void cboRoomType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    if (cboRoomType.EditValue != null)
                    {
                        txtRoom.Focus();
                        txtRoom.SelectAll();
                    }
                    else
                    {
                        cboRoomType.Focus();
                        cboRoomType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtRoom.Text))
                    {
                        cboRoom.EditValue = null;
                        cboRoom.Focus();
                        cboRoom.ShowPopup();
                    }
                    else
                    {
                        List<V_HIS_ROOM> searchs = null;
                        var listData1 = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ROOM_CODE.ToUpper().Contains(txtRoom.Text.ToUpper()) && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.ROOM_CODE.ToUpper() == txtRoom.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtRoom.Text = searchs[0].ROOM_CODE;
                            cboRoom.EditValue = searchs[0].ID;
                            cboDay.Focus();
                            cboDay.ShowPopup();
                        }
                        else
                        {
                            cboRoom.Focus();
                            cboRoom.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboRoom.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<V_HIS_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtRoom.Text = data.ROOM_CODE;
                            cboDay.Focus();
                            cboDay.ShowPopup();
                        }

                        e.Handled = true;

                    }
                    else
                    {
                        cboRoom.Focus();
                        cboRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRoom.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<V_HIS_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtRoom.Text = data.ROOM_CODE;
                            cboDay.Focus();
                            cboDay.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtFromTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtToTime.Focus();
                    dtToTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtToTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtName.Focus();
                    txtName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
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

        private void cboRoomType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRoomType.EditValue != null)
                {
                    InitComboRoom();
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
                    cboRoomTypeSearch.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDay_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dayName = "";
                if (_DaySelecteds != null && _DaySelecteds.Count > 0)
                {
                    foreach (var item in _DaySelecteds)
                    {
                        dayName += item.Name + ", ";
                    }
                }

                e.DisplayText = dayName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDay_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtFromTime.Focus();
                    dtFromTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDay_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboDay.Enabled = false;
                cboDay.Enabled = true;
                cboDay.Focus();
                //dtFromTime.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnGConfigSTT_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CallModuleHisNumOrderBlock();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallModuleHisNumOrderBlock()
        {
            try
            {
                var row = (V_HIS_ROOM_TIME)gridviewFormList.GetFocusedRow();

                if (row != null)
                {
                    HIS_ROOM_TIME data = new HIS_ROOM_TIME();
                    LoadCurrent(row.ID, ref data);
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    listArgs.Add(data);
                    ModuleCaller callModule = new ModuleCaller(ModuleCaller.HisNumOrderBlock, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    WaitingManager.Hide();
                }
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        

    }
}
