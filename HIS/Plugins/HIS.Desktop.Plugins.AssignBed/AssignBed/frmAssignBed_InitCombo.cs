using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignBed.ADO;
using HIS.Desktop.Plugins.AssignBed.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignBed.AssignBed
{
    public partial class frmAssignBed : HIS.Desktop.Utility.FormBase
    {
        private async Task InitComboUser()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitComboUser.1");
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                List<ACS.EFMODEL.DataModels.ACS_USER> dataUsers = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                {
                    dataUsers = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataUsers = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (dataUsers != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), dataUsers, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                Inventec.Common.Logging.LogSystem.Debug("InitComboUser.2__dataUsers.count =" + (dataUsers != null ? dataUsers.Count : 0));

                var employees = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>();
                datas = dataUsers != null ? dataUsers.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                //#26000 combobox "Người chỉ định" chỉ hiển thị các tài khoản nhân viên có thông tin "Chứng chỉ hành nghề" (DIPLOMA trong his_employee khác null)
                if (HisConfigCFG.IsReqUserMustHaveDiploma && datas != null && datas.Count > 0)
                {
                    var EmployeeHasDiplomaList = employees
                        .Where(o => !String.IsNullOrEmpty(o.DIPLOMA))
                        .Select(t => t.LOGINNAME)
                        .Distinct().ToList();

                    datas = EmployeeHasDiplomaList != null && EmployeeHasDiplomaList.Count() > 0
                        ? datas.Where(o => EmployeeHasDiplomaList.Contains(o.LOGINNAME)).ToList()
                        : datas;
                }

                if (HisConfigCFG.IsShowingInTheSameDepartment && datas != null && datas.Count > 0 && this.currentModule != null)
                {
                    var currentRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    Inventec.Common.Logging.LogSystem.Debug("current department" + currentRoom.DEPARTMENT_ID);
                    var EmployeeIndepartmentList = employees
                        .Where(o => o.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID)
                        .Select(t => t.LOGINNAME)
                        .Distinct().ToList();

                    datas = EmployeeIndepartmentList != null && EmployeeIndepartmentList.Count() > 0
                        ? datas.Where(o => EmployeeIndepartmentList.Contains(o.LOGINNAME)).ToList()
                        : null;
                }
                Inventec.Common.Logging.LogSystem.Debug("InitComboUser.3");
                //Nguoi chi dinh
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboUser, datas, controlEditorADO);
                Inventec.Common.Logging.LogSystem.Debug("InitComboUser.4");
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var oneUser = (datas != null ? datas.Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).FirstOrDefault() : null);


                if (this.previusTreatmentId > 0 && this.currentHisTreatment != null)
                {
                    this.cboUser.EditValue = this.currentHisTreatment.PREVIOUS_END_LOGINNAME;
                    this.txtLoginName.Text = this.currentHisTreatment.PREVIOUS_END_LOGINNAME;
                }
                else if (oneUser != null)
                {
                    this.cboUser.EditValue = oneUser.LOGINNAME;
                    this.txtLoginName.Text = oneUser.LOGINNAME;
                }
                Inventec.Common.Logging.LogSystem.Debug("InitComboUser.5");

                //Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
                //- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn
                //- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
                this.cboUser.Enabled = (HisConfigCFG.ShowRequestUser == "1");
                this.txtLoginName.Enabled = (HisConfigCFG.ShowRequestUser == "1");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdToControl(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    var icd = this.currentIcds.Where(p => p.ICD_CODE == (icdCode)).FirstOrDefault();
                    if (icd != null)
                    {
                        txtIcdCode.Text = icd.ICD_CODE;
                        cboIcds.EditValue = icd.ID;
                        if ((isAutoCheckIcd) || (!String.IsNullOrEmpty(icdName) && (icdName ?? "").Trim().ToLower() != (icd.ICD_NAME ?? "").Trim().ToLower()))
                        {
                            chkEditIcd.Checked = (HisConfigCFG.AutoCheckIcd != "2");
                            txtIcdMainText.Text = icdName;
                        }
                        else
                        {
                            chkEditIcd.Checked = false;
                            txtIcdMainText.Text = icd.ICD_NAME;
                        }
                    }
                    else
                    {
                        txtIcdCode.Text = null;
                        cboIcds.EditValue = null;
                        txtIcdMainText.Text = null;
                        chkEditIcd.Checked = false;
                    }
                }
                else if (!string.IsNullOrEmpty(icdName))
                {
                    chkEditIcd.Checked = (HisConfigCFG.AutoCheckIcd != "2");
                    txtIcdMainText.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToIcdSub(string icdSubCode, string icdText)
        {
            try
            {
                this.txtIcdSubCode.Text = icdSubCode;
                this.txtIcdText.Text = icdText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadCauHinhIn()
        {
            try
            {
                lstLoaiPhieu = new List<LoaiPhieuInADO>()
                {
                    new LoaiPhieuInADO("gridView7_1", "Phiếu yêu cầu dịch vụ",true),
                    new LoaiPhieuInADO("gridView7_2", "Hướng dẫn bệnh nhân"),
                    new LoaiPhieuInADO("gridView7_3", "Yêu cầu thanh toán QR"),
                    new LoaiPhieuInADO("gridView7_4", "Phiếu yêu cầu tổng hợp")
                };

                gridView7.BeginUpdate();
                gridView7.GridControl.DataSource = lstLoaiPhieu;
                gridView7.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataToCashierRoom()
        {
            try
            {
                List<V_HIS_CASHIER_ROOM> cashierRooms;
                if (WorkPlace.GetRoomIds() != null && WorkPlace.GetRoomIds().Count > 0)
                {
                    if (!BackendDataWorker.IsExistsKey<V_HIS_CASHIER_ROOM>())
                    {
                        CommonParam paramCommon = new CommonParam();
                        MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
                        cashierRooms = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>>("api/HisCashierRoom/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                        if (cashierRooms != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM), cashierRooms, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                    }

                    cashierRooms = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().Where(o => WorkPlace.GetRoomIds().Contains(o.ROOM_ID)).ToList();
                }
                else
                {
                    cashierRooms = new List<V_HIS_CASHIER_ROOM>();
                }
                cboCashierRoom.Properties.DataSource = cashierRooms;
                cboCashierRoom.Properties.DisplayMember = "CASHIER_ROOM_NAME";
                cboCashierRoom.Properties.ValueMember = "ID";
                cboCashierRoom.Properties.ForceInitialize();
                cboCashierRoom.Properties.Columns.Clear();
                cboCashierRoom.Properties.Columns.Add(new LookUpColumnInfo("CASHIER_ROOM_CODE", "", 50));
                cboCashierRoom.Properties.Columns.Add(new LookUpColumnInfo("CASHIER_ROOM_NAME", "", 200));
                cboCashierRoom.Properties.ShowHeader = false;
                cboCashierRoom.Properties.ImmediatePopup = true;
                cboCashierRoom.Properties.DropDownRows = 10;
                cboCashierRoom.Properties.PopupWidth = 250;
                // đặt giá trị mặc định cho phòng thu ngân
                if (cashierRooms != null && cashierRooms.Count > 0)
                {
                    cboCashierRoom.EditValue = cashierRooms.FirstOrDefault().ID;
                }
                else
                {
                    cboCashierRoom.EditValue = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        async Task LoadServiceSameToRAM()
        {
            try
            {
                if (!BackendDataWorker.IsExistsKey<V_HIS_SERVICE_SAME>())
                {
                    MOS.Filter.HisServiceSameViewFilter serviceSameViewFilter = new HisServiceSameViewFilter();
                    serviceSameViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    this.currentServiceSames = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_SERVICE_SAME>>("api/HisServiceSame/GetView", ApiConsumer.ApiConsumers.MosConsumer, serviceSameViewFilter, null);

                    if (this.currentServiceSames != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_SAME), this.currentServiceSames, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                else
                {
                    this.currentServiceSames = BackendDataWorker.Get<V_HIS_SERVICE_SAME>();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitComboExecuteRoom(GridLookUpEdit excuteRoomCombo, List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> data)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> executeRoomFilters = ProcessExecuteRoom();
                data = (executeRoomFilters != null && executeRoomFilters.Count > 0 && data != null && data.Count > 0) ? data.Where(p => executeRoomFilters.Select(o => o.ID).Distinct().Contains(p.ID)
                    || p.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, false, 350);
                ControlEditorLoader.Load(excuteRoomCombo, data, controlEditorADO);
                //executeRoomDefault = SetDefaultExcuteRoom(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> ProcessExecuteRoom()
        {
            this.currentExecuteRooms = new List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
            CommonParam param = new CommonParam();
            long instructionDate = 0;
            List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> executeRoomAlls = null;
            try
            {
                // không cho phép chỉ định dịch vụ vào các phòng đang tạm ngừng chỉ định Feature #10457
                executeRoomAlls = this.allDataExecuteRooms.Where(o => (o.IS_PAUSE_ENCLITIC == null || o.IS_PAUSE_ENCLITIC != 1) && (o.IS_PAUSE == null || o.IS_PAUSE != 1) && o.IS_ACTIVE == 1).ToList();
                //+ "Phòng đó phải không giới hạn thời gian hoạt động (IS_RESTRICT_TIME trong HIS_ROOM null)"                HOẶC "Phòng đó có giới hạn thời gian hoạt động và thời gian chỉ định nằm trong danh sách thời gian hoạt động của phòng đấy(có trong bảng HIS_ROOM_TIME)"

                List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> roomWithRoomTimeFilter = new List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                // phòng không giới hạn thời gian hoạt động
                List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> roomIsRestrictTimes = executeRoomAlls.Where(o => o.IS_RESTRICT_TIME == null).ToList();
                roomWithRoomTimeFilter.AddRange(roomIsRestrictTimes);

                //phòng có giới hạn thời gian hoạt động
                List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> roomIsNotRestrictTimes = executeRoomAlls.Where(o => o.IS_RESTRICT_TIME != null).ToList();
                DateTime dayOfWeekInstructionTimeDt = DateTime.Now;
                if (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0)
                {
                    instructionDate = Convert.ToInt64((this.intructionTimeSelecteds.First().ToString()).Substring(8, 6));
                    dayOfWeekInstructionTimeDt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now;
                }
                int dayOfWeekInstructionTime = (int)dayOfWeekInstructionTimeDt.DayOfWeek + 1;

                if (this.roomTimes != null && this.roomTimes.Count > 0)
                {
                    foreach (var executeRoom in roomIsNotRestrictTimes)
                    {
                        var bExistsRoomTime = this.roomTimes.Exists(o => o.ROOM_ID == executeRoom.ROOM_ID && o.DAY == dayOfWeekInstructionTime && Convert.ToInt64(o.FROM_TIME) <= instructionDate && instructionDate <= Convert.ToInt64(o.TO_TIME) && ConvertDayOfWeek(dayOfWeekInstructionTimeDt, o.DAY));
                        if (bExistsRoomTime)
                        {
                            roomWithRoomTimeFilter.Add(executeRoom);
                        }
                    }
                }

                // + Nếu phòng đang người dùng đang làm việc có check "Giới hạn chỉ định phòng thực hiện" (IS_RESTRICT_EXECUTE_ROOM trong HIS_ROOM), thì lọc tiếp, chỉ lấy các phòng nằm trong danh sách các phòng xử lý mà phòng đang làm việc được phép chỉ định (lấy theo bảng HIS_EXRO_ROOM với IS_ALLOW_REQUEST = 1)

                if (roomWithRoomTimeFilter != null && roomWithRoomTimeFilter.Count > 0)
                {
                    var currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    if (currentWorkingRoom != null && currentWorkingRoom.IS_RESTRICT_EXECUTE_ROOM == 1)
                    {
                        roomWithRoomTimeFilter = (this.exroRooms != null && this.exroRooms.Count > 0) ?
                            roomWithRoomTimeFilter.Where(o => exroRooms.Exists(e => e.EXECUTE_ROOM_ID == o.ID && e.IS_ALLOW_REQUEST == 1)).ToList()
                            : new List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                    }
                    this.currentExecuteRooms.AddRange(roomWithRoomTimeFilter);
                }
            }
            catch (Exception ex)
            {
                this.currentExecuteRooms = new List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return this.currentExecuteRooms;
        }

        private async Task InitComboExecuteRoom()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> executeRooms = new List<V_HIS_EXECUTE_ROOM>();
                executeRooms = ProcessExecuteRoom();
                Action myaction = () => {

                    if (this.IsTreatmentInBedRoom)
                    {
                        ProcessAddBedRoomToExecuteRoom(null, ref executeRooms);
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.repositoryItemcboExcuteRoom_TabService, executeRooms, controlEditorADO);
                //executeRoomDefault = SetDefaultExcuteRoom(executeRooms);

                ControlEditorLoader.Load(this.repositoryItemcboExcuteRoomPlus_TabService, executeRooms, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessAddBedRoomToExecuteRoom(List<long> roomIds, ref List<V_HIS_EXECUTE_ROOM> executeRooms)
        {
            try
            {
                if (executeRooms != null)
                {
                    var allBedRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    if (this.currentDepartment != null)
                    {
                        allBedRoom = allBedRoom.Where(o => o.DEPARTMENT_ID == this.currentDepartment.ID).ToList();
                    }

                    if (roomIds != null && roomIds.Count > 0)
                    {
                        allBedRoom = allBedRoom.Where(o => roomIds.Contains(o.ROOM_ID)).ToList();
                    }

                    if (allBedRoom != null && allBedRoom.Count > 0)
                    {
                        executeRooms.AddRange((from m in allBedRoom
                                               select new V_HIS_EXECUTE_ROOM()
                                               {
                                                   EXECUTE_ROOM_CODE = m.BED_ROOM_CODE,
                                                   EXECUTE_ROOM_NAME = m.BED_ROOM_NAME,
                                                   ROOM_ID = m.ROOM_ID,
                                                   IS_SURGERY = m.IS_SURGERY,
                                                   IS_ACTIVE = m.IS_ACTIVE,
                                                   BHYT_LIMIT = m.BHYT_LIMIT,
                                                   DEPARTMENT_CODE = m.DEPARTMENT_CODE,
                                                   DEPARTMENT_ID = m.DEPARTMENT_ID,
                                                   DEPARTMENT_NAME = m.DEPARTMENT_NAME,
                                                   G_CODE = m.G_CODE,
                                                   IS_PAUSE = m.IS_PAUSE,
                                                   IS_RESTRICT_EXECUTE_ROOM = m.IS_RESTRICT_EXECUTE_ROOM,
                                                   IS_RESTRICT_REQ_SERVICE = m.IS_RESTRICT_REQ_SERVICE,
                                                   ROOM_TYPE_CODE = m.ROOM_TYPE_CODE,
                                                   ROOM_TYPE_NAME = m.ROOM_TYPE_NAME,
                                                   ROOM_TYPE_ID = m.ROOM_TYPE_ID,
                                                   SPECIALITY_CODE = m.SPECIALITY_CODE,
                                                   SPECIALITY_ID = m.SPECIALITY_ID,
                                                   SPECIALITY_NAME = m.SPECIALITY_NAME
                                               }).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ConvertDayOfWeek(DateTime dayOfWeekInstructionTimeDt, int dayInRoomTime)
        {
            bool result = false;
            try
            {
                int dayOfWeekInstructionTime = (int)dayOfWeekInstructionTimeDt.DayOfWeek;
                if (dayOfWeekInstructionTime == 0 && dayInRoomTime == 1)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 1 && dayInRoomTime == 2)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 2 && dayInRoomTime == 3)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 3 && dayInRoomTime == 4)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 4 && dayInRoomTime == 5)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 5 && dayInRoomTime == 6)
                {
                    result = true;
                }
                else if (dayOfWeekInstructionTime == 6 && dayInRoomTime == 7)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InitComboSampleType(GridLookUpEdit cbo)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TEST_SAMPLE_TYPE_NAME", "ID", columnInfos, false, 250);

                ControlEditorLoader.Load(cbo, dataListTestSampleType, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillSampleType(SereServADO data, GridLookUpEdit sampleTypeCombo)
        {
            try
            {
                if (((HisConfigCFG.IntegrationVersionValue == "1" && HisConfigCFG.IntegrationOptionValue != "1") || (HisConfigCFG.IntegrationVersionValue == "2" && HisConfigCFG.IntegrationTypeValue != "1")) && data.SERVICE_TYPE_ID > 0 && serviceTypeIdSplitReq != null && serviceTypeIdSplitReq.Count > 0 && serviceTypeIdSplitReq.Exists(o => o == data.SERVICE_TYPE_ID))
                {
                    InitComboSampleType(sampleTypeCombo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }


}
