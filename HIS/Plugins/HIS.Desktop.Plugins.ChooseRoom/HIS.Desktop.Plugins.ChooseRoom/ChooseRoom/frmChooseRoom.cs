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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ChooseRoom.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ChooseRoom.ChooseRoom
{
    public partial class frmChooseRoom : HIS.Desktop.Utility.FormBase
    {
        RefeshReference refeshReference;
        List<RoomADO> currentUserRooms = null;
        List<RoomADO> currentUserRoomsByBranch = null;
        List<MOS.EFMODEL.DataModels.HIS_BRANCH> currentBranchs;
        List<MOS.EFMODEL.DataModels.HIS_DESK> currentDesks;
        List<MOS.EFMODEL.DataModels.HIS_WORKING_SHIFT> currentWorkingShifts;
        List<EmployeeADO> currentNurses;
        bool statecheckColumn = false;
        bool MustChooseWorkingShift;
        /// <summary>
        /// Trong trường hợp cấu hình hệ thống trên có giá trị 1, thì phần loại phòng là "Buồng" sẽ hiển thị gộp lại theo khoa. Cụ thể:
        ///- Với loại phòng là "Buồng bệnh" thì mỗi khoa sẽ chỉ hiển thị 1 buồng đầu tiên (ID nhỏ nhất) của khoa đó, và "Tên phòng" sửa lại hiển thị theo định dạng: X Y
        ///Trong đó:
        ///+ X là tên loại phòng (room_type_name)
        ///+ Y là tên khoa (department_name)
        /// </summary>
        string groupRoomOption;
        int positionHandleControl = -1;

        public frmChooseRoom(Inventec.Desktop.Common.Modules.Module module)
            : this(null, null)
        {
        }

        public frmChooseRoom(Inventec.Desktop.Common.Modules.Module module, RefeshReference _refeshReference)
            : base(module)
        {
            InitializeComponent();
            this.refeshReference = _refeshReference;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.AddBarManager(this.barManager2);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmChooseRoom_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                //this.SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
                this.MustChooseWorkingShift = HisConfigs.Get<string>("HIS.Desktop.Plugins.ChooseRoom.MustChooseWorkingShift") == GlobalVariables.CommonStringTrue;
                this.groupRoomOption = HisConfigs.Get<string>("HIS.Desktop.Plugins.ChooseRoom.GroupRoomOption");
                this.SetDefaultData();

                this.InitBranchCombo();
                this.InitDeskCombo();

                this.InitWorkingShiftCombo();

                this.InitNurseCombo(BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>());

                this.SetDefaultStateByWorkplace();

                this.SetCheckAllColumn(this.statecheckColumn);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ChooseRoom.Resources.Lang", typeof(frmChooseRoom).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNurse.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseRoom.cboNurse.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboWorkingShift.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseRoom.cboWorkingShift.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseRoom.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChoice.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.btnChoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBranch.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseRoom.cboBranch.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmChooseRoom.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCheck.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnCheck.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRoomCode.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRoomName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRoomTypeName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDepartmentName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDesk.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnDesk.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboHisDesk.NullText = Inventec.Common.Resource.Get.Value("frmChooseRoom.repositoryItemcboHisDesk.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBranch.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.lciBranch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForWorkingShift.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.lciForWorkingShift.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFortxtLoginName.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.lciFortxtLoginName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.barButtonItem__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__UncheckAll.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.barButtonItem__UncheckAll.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ChooseRoom.Resources.Lang", typeof(HIS.Desktop.Plugins.ChooseRoom.ChooseRoom.frmChooseRoom).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseRoom.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChoice.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.btnChoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBranch.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseRoom.cboBranch.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmChooseRoom.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCheck.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnCheck.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRoomCode.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRoomName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRoomTypeName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDepartmentName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDesk.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnDesk.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBranch.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.lciBranch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFortxtLoginName.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.lciFortxtLoginName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModuleBase != null)
                {
                    this.Text = this.currentModuleBase.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckAllColumn(bool state)
        {
            try
            {
                this.gridColumnCheck.ImageAlignment = StringAlignment.Center;
                this.gridColumnCheck.Image = (state ? this.imageCollection1.Images[1] : this.imageCollection1.Images[0]);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDefaultData()
        {
            try
            {
                CommonParam param = new CommonParam();
                var rooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                var userRoomByUsers = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == loginName && (o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)).ToList();
                if (userRoomByUsers != null)
                {
                    var roomIds = userRoomByUsers.Select(o => o.ROOM_ID).Distinct().ToList();
                    this.currentUserRooms = (from m in rooms where roomIds.Contains(m.ID) select new RoomADO(m)).Distinct().ToList();

                    if (this.groupRoomOption == "1")
                    {
                        List<RoomADO> listRemoves = new List<RoomADO>();

                        var roomByTypeBuongs = this.currentUserRooms != null ? this.currentUserRooms.Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG).ToList() : null;
                        if (roomByTypeBuongs != null && roomByTypeBuongs.Count > 0)
                        {
                            var roomGroupByDepartments = roomByTypeBuongs.GroupBy(o => o.DEPARTMENT_ID, o => o).ToDictionary(o => o.Key, t => t.ToList());
                            if (roomGroupByDepartments != null && roomGroupByDepartments.Count > 0)
                            {
                                foreach (var item in roomGroupByDepartments)
                                {
                                    if (item.Value.Count > 1)
                                    {
                                        var minId = item.Value.Min(k => k.ID);
                                        listRemoves.AddRange(item.Value.Where(l => l.ID != minId).ToList());

                                        var roomIdMin = this.currentUserRooms.FirstOrDefault(f => f.ID == minId);
                                        roomIdMin.ROOM_NAME = String.Format("{0} {1}", roomIdMin.ROOM_TYPE_NAME, roomIdMin.DEPARTMENT_NAME);
                                    }
                                }

                                if (listRemoves.Count > 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.currentUserRooms.Count", this.currentUserRooms.Count)
                                        + Inventec.Common.Logging.LogUtil.TraceData("this.listRemoves.Count", listRemoves.Count));
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listRemoves), listRemoves));
                                    this.currentUserRooms = this.currentUserRooms.Where(m => !listRemoves.Exists(d => d.ID == m.ID)).ToList();
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.currentUserRooms(afterremove).Count", this.currentUserRooms.Count));
                                }
                            }
                        }
                    }

                    this.currentUserRoomsByBranch = new List<RoomADO>();
                    this.currentUserRoomsByBranch.AddRange(this.currentUserRooms);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void FillDataToTileControl()
        {
            try
            {
                this.gridControlRooms.DataSource = null;
                this.btnChoice.Enabled = false;

                long branchId = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBranch.EditValue ?? "0").ToString());
                var departmerts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().Where(o => o.BRANCH_ID == branchId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (departmerts == null || departmerts.Count == 0)
                    throw new ArgumentNullException("departmerts is null");

                var departmentIds = departmerts.Select(o => o.ID).ToArray();

                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());
                var query = this.currentUserRoomsByBranch.AsQueryable();
                query = query.Where(o => o.ROOM_CODE.ToLower().Contains(keyword)
                        || Inventec.Common.String.Convert.UnSignVNese(o.ROOM_NAME.ToLower()).Contains(keyword)
                        || o.ROOM_TYPE_CODE.ToLower().Contains(keyword)
                        || Inventec.Common.String.Convert.UnSignVNese(o.ROOM_TYPE_NAME.ToLower()).Contains(keyword)
                        || Inventec.Common.String.Convert.UnSignVNese(o.DEPARTMENT_NAME.ToLower()).Contains(keyword));
                query = query.Where(o => departmentIds.Contains(o.DEPARTMENT_ID) && o.BRANCH_ID == branchId);
                if (Inventec.Common.TypeConvert.Parse.ToInt64((this.cboDepartment.EditValue ?? "0").ToString()) > 0)
                {
                    query = query.Where(o => o.DEPARTMENT_ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboDepartment.EditValue ?? "0").ToString()));
                }

                var userRooms = query
                    .OrderBy(o => o.ROOM_TYPE_NAME)
                    .ThenBy(o => o.DEPARTMENT_NAME)
                    .ThenBy(o => o.ROOM_NAME).Distinct().ToList();

                bool isAutoFocus = false;
                //Nếu người dùng chỉ được cấu hình 1 phòng làm việc => tự động chọn phòng đó luôn
                if (userRooms.Count == 1)
                {
                    foreach (var ur in userRooms)
                    {
                        ur.IsChecked = true;
                    }
                    isAutoFocus = true;
                }

                this.gridControlRooms.DataSource = userRooms;
                this.btnChoice.Enabled = (userRooms != null && userRooms.Count > 0);
                if (isAutoFocus)
                {
                    this.gridViewRooms.FocusedRowHandle = 0;
                    btnChoice.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitBranchCombo()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BRANCH_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 250);
                //var branchIds = this.currentUserRoomsByBranch.Select(o => o.BRANCH_ID).Distinct().ToList();
                //this.currentBranchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().Where(o => branchIds != null && branchIds.Contains(o.ID)).ToList();
                this.currentBranchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>();
                ControlEditorLoader.Load(this.cboBranch, this.currentBranchs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDeskCombo()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DESK_NAME", "", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DESK_NAME", "ID", columnInfos, false, 200);

                this.currentDesks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DESK>();
                if (this.currentDesks != null)
                {
                    this.currentDesks = this.currentDesks.Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue).ToList();
                }
                ControlEditorLoader.Load(this.repositoryItemcboHisDesk, this.currentDesks, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitWorkingShiftCombo()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("WORKING_SHIFT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("WORKING_SHIFT_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORKING_SHIFT_NAME", "ID", columnInfos, false, 250);
                //var branchIds = this.currentUserRoomsByBranch.Select(o => o.BRANCH_ID).Distinct().ToList();
                //this.currentBranchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().Where(o => branchIds != null && branchIds.Contains(o.ID)).ToList();
                this.currentWorkingShifts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORKING_SHIFT>();
                if (this.currentWorkingShifts != null)
                {
                    this.currentWorkingShifts = this.currentWorkingShifts.Where(o => o.IS_ACTIVE == 1).ToList();
                }
                ControlEditorLoader.Load(this.cboWorkingShift, this.currentWorkingShifts, controlEditorADO);

                this.lciForWorkingShift.AppearanceItemCaption.ForeColor = this.MustChooseWorkingShift ? System.Drawing.Color.Maroon : System.Drawing.Color.Black;
                if (this.MustChooseWorkingShift)
                {
                    ValidationSingleControl(cboWorkingShift, dxValidationProviderControl);
                }
                if (WorkPlace.WorkInfoSDO != null && WorkPlace.WorkInfoSDO.WorkingShiftId > 0)
                {
                    this.cboWorkingShift.EditValue = WorkPlace.WorkInfoSDO.WorkingShiftId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitNurseCombo(bool isRun)
        {
            try
            {
                if (isRun)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("LOGINNAME", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("USERNAME", "", 200, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                    var nurses = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>();
                    var users = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                    if (nurses != null)
                    {
                        this.currentNurses = (from m in nurses where m.IS_ACTIVE == 1 && m.IS_NURSE == 1 select new EmployeeADO(m, users)).ToList();
                    }
                    ControlEditorLoader.Load(this.cboNurse, this.currentNurses, controlEditorADO);

                    if (WorkPlace.WorkInfoSDO != null && !String.IsNullOrEmpty(WorkPlace.WorkInfoSDO.NurseLoginName))
                    {
                        this.cboNurse.EditValue = WorkPlace.WorkInfoSDO.NurseLoginName;
                        this.txtLoginName.EditValue = WorkPlace.WorkInfoSDO.NurseLoginName;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDepartmentCombo(long branchId)
        {
            try
            {
                var departments = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().Where(o => o.BRANCH_ID == branchId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 90, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 350, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 440);
                ControlEditorLoader.Load(this.cboDepartment, departments, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultStateByWorkplace()
        {
            try
            {
                if (WorkPlace.WorkPlaceSDO != null && WorkPlace.WorkPlaceSDO.Count > 0)
                {
                    foreach (var item in WorkPlace.WorkPlaceSDO)
                    {
                        var us = this.currentUserRoomsByBranch.FirstOrDefault(o => o.ID == item.RoomId);
                        if (us != null)
                        {
                            us.IsChecked = true;
                            us.DESK_ID = item.DeskId;
                        }
                        //else
                        //    us.IsChecked = false;                        
                    }
                }

                var branch = this.currentBranchs.SingleOrDefault(o => o.ID == BranchDataWorker.GetCurrentBranchId());
                if (branch == null && this.currentBranchs != null && this.currentBranchs.Count > 0)
                {
                    branch = this.currentBranchs[0];
                }
                if (branch != null)
                {
                    this.cboBranch.EditValue = branch.ID;
                    this.InitDepartmentCombo(branch.ID);
                    this.currentUserRoomsByBranch = this.currentUserRooms.Where(o => o.BRANCH_ID == branch.ID).ToList();
                    this.FillDataToTileControl();
                    this.cboBranch.Enabled = false;
                    txtKeyword.Focus();
                    txtKeyword.SelectAll();
                }
                else
                {
                    this.cboBranch.Enabled = true;
                    this.cboBranch.Focus();
                    this.cboBranch.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.gridViewRooms.FocusedRowHandle = 0;
                    this.gridViewRooms.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewRooms.FocusedRowHandle = 0;
                    this.gridViewRooms.Focus();
                }
                //else if (e.KeyCode == Keys.F8)
                //{
                //    Inventec.Common.WitAI.Form1 f1 = new Inventec.Common.WitAI.Form1(UpdateInputAfterRegconizeText);
                //    f1.ShowDialog();
                //}
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.FillDataToTileControl();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewRooms_CustomColumnGroup(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "ROOM_TYPE_NAME")
                {
                    e.Result = 1;
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewRooms_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;
                if (e.Column.FieldName == "ROOM_TYPE_NAME" && e.IsForGroupRow)
                {
                    string rowValue = Convert.ToString(view.GetGroupRowValue(e.GroupRowHandle, e.Column));
                    e.DisplayText = "" + rowValue;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewRooms_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var room = (RoomADO)this.gridViewRooms.GetFocusedRow();
                if (room != null)
                {
                    var roomInList = this.currentUserRoomsByBranch.FirstOrDefault(o => o.ID == room.ID);
                    if (roomInList != null)
                    {
                        roomInList.IsChecked = room.IsChecked;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewRooms_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }

                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    else if (hi.InColumnPanel)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            statecheckColumn = !statecheckColumn;
                            this.SetCheckAllColumn(statecheckColumn);
                            this.GridCheckChange(statecheckColumn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridCheckChange(bool checkedAll)
        {
            try
            {
                foreach (var item in this.currentUserRoomsByBranch)
                {
                    item.IsChecked = checkedAll;
                }
                this.gridViewRooms.BeginUpdate();
                this.gridViewRooms.GridControl.DataSource = this.currentUserRoomsByBranch;
                this.gridViewRooms.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRooms_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (this.gridViewRooms.IsEditing)
                        this.gridViewRooms.CloseEditor();

                    if (this.gridViewRooms.FocusedRowModified)
                        this.gridViewRooms.UpdateCurrentRow();

                    var room = (RoomADO)this.gridViewRooms.GetFocusedRow();
                    if (room != null)
                    {
                        this.gridViewRooms.BeginUpdate();
                        var roomAlls = this.gridControlRooms.DataSource as List<RoomADO>;
                        if (roomAlls != null)
                        {
                            foreach (var ro in roomAlls)
                            {
                                if (ro.ID == room.ID)
                                {
                                    ro.IsChecked = !ro.IsChecked;
                                }
                            }

                            var roomInList = this.currentUserRoomsByBranch.FirstOrDefault(o => o.ID == room.ID);
                            if (roomInList != null)
                            {
                                roomInList.IsChecked = room.IsChecked;
                            }

                            this.gridViewRooms.GridControl.DataSource = roomAlls;
                        }
                        this.gridViewRooms.EndUpdate();
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    btnChoice_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewRooms_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (Inventec.Common.TypeConvert.Parse.ToBoolean(this.gridViewRooms.GetRowCellValue(e.RowHandle, "IsChecked").ToString() ?? "") == true)
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else
                    e.Appearance.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridControlRooms_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                GridControl grid = sender as GridControl;
                GridView view = grid.FocusedView as GridView;
                if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
                {
                    if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && view.FocusedColumn.VisibleIndex == 0))
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        grid.SelectNextControl(btnChoice, e.Modifiers == Keys.None, false, false, true);
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    if (this.gridViewRooms.IsEditing)
                        this.gridViewRooms.CloseEditor();

                    if (this.gridViewRooms.FocusedRowModified)
                        this.gridViewRooms.UpdateCurrentRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRooms_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = Convert.ToString(this.gridViewRooms.GetGroupRowValue(e.RowHandle, this.gridColumnRoomTypeName) ?? "");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboWorkingShift_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLoginName.Focus();
                    txtLoginName.SelectAll();
                }
                else
                {
                    this.cboWorkingShift.ShowPopup();
                    if (this.MustChooseWorkingShift)
                        PopupLoader.SelectFirstRowPopup(this.cboWorkingShift);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBranch_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboBranch.EditValue != null)
                    {
                        var branch = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBranch.EditValue ?? "0").ToString()));
                        if (branch != null)
                        {
                            this.InitDepartmentCombo(branch.ID);
                            this.cboDepartment.EditValue = null;
                            this.cboDepartment.Properties.Buttons[1].Visible = false;
                            this.ResetStateUserRoomData();
                            this.FillDataToTileControl();
                        }
                    }
                    this.txtKeyword.Focus();
                    this.txtKeyword.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ResetStateUserRoomData()
        {
            try
            {
                if (this.currentUserRoomsByBranch != null && this.currentUserRoomsByBranch.Count > 0)
                {
                    foreach (var ur in this.currentUserRoomsByBranch)
                    {
                        ur.IsChecked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboBranch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboBranch.EditValue != null)
                    {
                        var branch = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBranch.EditValue ?? "0").ToString()));
                        if (branch != null)
                        {
                            this.InitDepartmentCombo(branch.ID);
                            this.cboDepartment.EditValue = null;
                            this.cboDepartment.Properties.Buttons[1].Visible = false;
                            this.ResetStateUserRoomData();
                            this.FillDataToTileControl();
                        }
                        this.txtKeyword.Focus();
                        this.txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private bool Check(CommonParam param, ref List<long> roomIdCheckeds, ref List<RoomADO> roomSelecteds)
        {
            bool valid = true;
            try
            {

                roomSelecteds = this.currentUserRoomsByBranch.Where(o => o.IsChecked).ToList();
                roomIdCheckeds = roomSelecteds != null ? roomSelecteds.Select(o => o.ID).ToList() : null;
                if (roomIdCheckeds == null || roomIdCheckeds.Count == 0)
                {
                    param.Messages.Add(ResourceMessage.KhongChonPhongLamViec);
                    valid = false;
                }

                var branchCounts = this.currentUserRoomsByBranch.Where(o => o.IsChecked).Select(o => o.BRANCH_ID).Distinct().Count();
                if (branchCounts > 1)
                {
                    param.Messages.Add(ResourceMessage.DuLieuPhongDangChonThuocNhieuChiNhanh);
                    valid = false;
                }
                valid = valid && this.dxValidationProviderControl.Validate();
                string warning = "";
                if (!valid)
                {
                    if (this.ModuleControls == null || this.ModuleControls.Count == 0)
                    {
                        ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                        this.ModuleControls = controlProcess.GetControls(this);
                    }

                    GetMessageErrorControlInvalidProcess getMessageErrorControlInvalidProcess = new Utility.GetMessageErrorControlInvalidProcess();
                    getMessageErrorControlInvalidProcess.Run(this, this.dxValidationProviderControl, this.ModuleControls, param);

                    warning = param.GetMessage();

                    //if (!String.IsNullOrEmpty(warning))
                    //    MessageBox.Show(warning, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (!valid)
                {
                    MessageManager.Show(param, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return valid;
        }

        private void ChangeLockButtonWhileProcess(bool isLock)
        {
            try
            {
                this.btnChoice.Enabled = isLock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChoice_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<long> roomIdCheckeds = new List<long>();
                List<RoomADO> roomSelecteds = new List<RoomADO>();
                if (this.currentUserRoomsByBranch != null)
                {
                    if (this.Check(param, ref roomIdCheckeds, ref roomSelecteds))
                    {
                        this.ChangeLockButtonWhileProcess(false);
                        WaitingManager.Show();

                        WorkInfoSDO workInfoSDO = new WorkInfoSDO();
                        if (roomSelecteds != null && roomSelecteds.Count > 0)
                        {
                            workInfoSDO.Rooms = (from m in roomSelecteds
                                                 select new RoomSDO() { RoomId = m.ID, DeskId = m.DESK_ID }).ToList();
                        }

                        //workInfoSDO.RoomIds = roomIdCheckeds;

                        if (cboNurse.EditValue != null)
                        {
                            workInfoSDO.NurseLoginName = (string)cboNurse.EditValue;
                            workInfoSDO.NurseUserName = (string)cboNurse.Text;
                        }

                        if (cboWorkingShift.EditValue != null)
                            workInfoSDO.WorkingShiftId = (long)cboWorkingShift.EditValue;
                        WorkPlace.WorkPlaceSDO = new BackendAdapter(param).Post<List<WorkPlaceSDO>>(HisRequestUriStore.TOKEN__UPDATE_WORK_PLACE_INFO, ApiConsumer.ApiConsumers.MosConsumer, workInfoSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (WorkPlace.WorkPlaceSDO != null && WorkPlace.WorkPlaceSDO.Count > 0)
                        {
                            WorkPlace.WorkInfoSDO = workInfoSDO;
                            GlobalVariables.CurrentRoomTypeIds = WorkPlace.WorkPlaceSDO.Select(o => o.RoomTypeId).ToList();
                            GlobalVariables.CurrentRoomTypeId = WorkPlace.WorkPlaceSDO.FirstOrDefault().RoomTypeId;
                            var roomTypes = BackendDataWorker.Get<HIS_ROOM_TYPE>().Where(o => GlobalVariables.CurrentRoomTypeIds.Contains(o.ID)).ToList();
                            if (roomTypes != null && roomTypes.Count > 0)
                            {
                                GlobalVariables.CurrentRoomTypeCode = roomTypes[0].ROOM_TYPE_CODE;
                                GlobalVariables.CurrentRoomTypeCodes = roomTypes.Select(o => o.ROOM_TYPE_CODE).ToList();
                            }
                            if (BranchDataWorker.GetCurrentBranchId() == 0 || BranchDataWorker.GetCurrentBranchId() != WorkPlace.GetBranchId())
                            {
                                //Luu chi nhanh dang chon vao registry
                                HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.ChangeBranch(WorkPlace.GetBranchId());
                            }
                            if (this.refeshReference != null)
                                this.refeshReference();
                            WaitingManager.Hide();
                            this.Close();
                        }
                        else
                        {
                            if (param.Messages.Count == 0)
                                param.Messages.Add(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat));

                            WaitingManager.Hide();
                            MessageManager.Show(param, false);
                            GlobalVariables.CurrentRoomTypeId = 0;
                            GlobalVariables.CurrentRoomTypeIds = null;
                            GlobalVariables.CurrentRoomTypeCodes = null;
                            GlobalVariables.CurrentRoomTypeCode = "";
                            WorkPlace.WorkInfoSDO = null;
                            WorkPlace.WorkPlaceSDO = null;
                            LogSystem.Warn("Goi api Token.UpdateWorkplace khong thanh cong" + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentUserRooms), currentUserRooms));
                        }
                        this.ChangeLockButtonWhileProcess(true);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void bbtnCtrlF1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.currentUserRoomsByBranch.ForEach(o => o.IsChecked = false);
                gridControlRooms.RefreshDataSource();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ReLoadServicePatyInBranchInRam()
        {
            try
            {
                WaitingManager.Show();
                LogSystem.Debug("ReLoadServicePatyInBranchInRam => ServicePaty begin load");
                //Xóa dữ liệu chính sách giá theo chi nhánh cũ
                HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Reset<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();

                //Khởi tạo lại dữ liệu chính sách giá theo chi nhánh ->lưu vào ram
                var newServicePatyInBranch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();
                LogSystem.Debug("ReLoadServicePatyInBranchInRam => ServicePaty end load");
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void frmChooseRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (!String.IsNullOrEmpty((this.cboBranch.EditValue ?? "").ToString()))
                    {
                        var branch = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBranch.EditValue ?? "").ToString()));
                        if (branch != null)
                        {
                            this.FillDataToTileControl();
                        }
                        this.cboDepartment.Properties.Buttons[1].Visible = true;
                    }
                    this.txtKeyword.Focus();
                    this.txtKeyword.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty((this.cboBranch.EditValue ?? "").ToString()))
                    {
                        var branch = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBranch.EditValue ?? "").ToString()));
                        if (branch != null)
                        {
                            this.FillDataToTileControl();
                        }
                        this.cboDepartment.Properties.Buttons[1].Visible = true;
                        this.txtKeyword.Focus();
                        this.txtKeyword.SelectAll();
                    }
                }
                else
                {
                    this.cboDepartment.ShowPopup();
                    PopupLoader.SelectFirstRowPopup(this.cboDepartment);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboDepartment.EditValue = null;
                    this.cboDepartment.Properties.Buttons[1].Visible = false;
                    this.FillDataToTileControl();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewRooms_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                gridControlRooms.BeginUpdate();
                var userRoomADO = (RoomADO)this.gridViewRooms.GetFocusedRow();
                if (userRoomADO != null)
                {
                    userRoomADO.IsChecked = !userRoomADO.IsChecked;
                    this.gridControlRooms.RefreshDataSource();
                }
                gridControlRooms.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #region Shortcut
        private void bbtnCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.btnChoice_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        private void dxValidationProviderControl_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNurse_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboNurse.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == (this.cboNurse.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                    }
                }
                else
                {
                    this.cboNurse.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.InitNurseCombo(!BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>());

                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboNurse.EditValue = null;
                        this.FocusShowpopup(this.cboNurse, false);
                    }
                    else
                    {
                        var data = currentNurses
                            .Where(o => o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboNurse.EditValue = searchResult[0].LOGINNAME;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;
                        }
                        else
                        {
                            this.cboNurse.EditValue = null;
                            this.FocusShowpopup(this.cboNurse, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNurse_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboNurse.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == (this.cboNurse.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNurse_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if ((e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Combo || e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
                    && (!BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>()
                    || cboNurse.Properties.DataSource == null))
                {
                    this.InitNurseCombo(true);
                    this.FocusShowpopup(this.cboNurse, false);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRooms_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                RoomADO data = view.GetFocusedRow() as RoomADO;

                if (view.FocusedColumn.FieldName == "DESK_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        this.FillDataIntoDeskCombo(data, editor);
                        //List<HIS_DESK> dataSource = (List<V_HIS_EXECUTE_ROOM>)editor.Properties.DataSource;
                        editor.EditValue = data.DESK_ID;
                        //long executeRoomId = SetDefaultExcuteRoom(dataSource);
                        //data.TDL_EXECUTE_ROOM_ID = executeRoomDefault;
                        //editor.EditValue = executeRoomId;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoDeskCombo(RoomADO data, GridLookUpEdit deskCombo)
        {
            try
            {
                if (deskCombo != null)
                {

                    List<MOS.EFMODEL.DataModels.HIS_DESK> desks = (currentDesks != null && currentDesks.Count > 0) ? currentDesks.Where(o => o.ROOM_ID == data.ID).ToList() : null;

                    this.InitComboDesk(deskCombo, desks);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDesk(GridLookUpEdit cboDesk, List<MOS.EFMODEL.DataModels.HIS_DESK> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DESK_NAME", "", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DESK_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboDesk, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
