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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ServiceRoom.ADO;
using Inventec.Common.Controls.EditorLoader;
using MOS.Filter;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.ApiConsumer;

namespace HIS.UC.ServiceRoom
{
    public partial class UCRoomExamService : UserControl
    {
        public async Task InitLoadAsync(RoomExamServiceInitADO ado)
        {
            try
            {
                txtRoomCode.Text = "";
                cboRoom.EditValue = null;
                txtExamServiceCode.Text = "";
                cboExamService.EditValue = null;

                if (ado.LHisRoomCounters == null || ado.LHisRoomCounters.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    HisRoomCounterLViewFilter exetuteFilter = new HisRoomCounterLViewFilter();
                    exetuteFilter.IS_EXAM = true;
                    exetuteFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    exetuteFilter.BRANCH_ID = WorkPlace.GetBranchId();
                    var dataExecuteRooms = await new Inventec.Common.Adapter.BackendAdapter(param).GetAsync<List<L_HIS_ROOM_COUNTER>>("api/HisRoom/GetCounterLView", ApiConsumers.MosConsumer, exetuteFilter, param);
                    if (dataExecuteRooms != null && dataExecuteRooms.Count > 0)
                    {
                        var roomIdActivesV2 = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => (p.IS_RESTRICT_PATIENT_TYPE == null
                            || ado.RoomIdByPatientTypeRooms.Contains(p.ID))).Select(p => p.ID).ToList();

                        string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Register.IsShowingExamRoomInDepartment");
                        if (key.Trim() == "1")
                        {
                            List<long> _RoomIdByDepartments = new List<long>();//20939
                            if (ado.RoomId > 0)
                            {
                                var roomById = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == ado.RoomId);
                                if (roomById != null)
                                    _RoomIdByDepartments = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => p.DEPARTMENT_ID == roomById.DEPARTMENT_ID).Select(p => p.ID).ToList();
                            }

                            dataExecuteRooms = dataExecuteRooms.Where(p => p.IS_PAUSE_ENCLITIC != 1
                            && roomIdActivesV2.Contains(p.ROOM_ID)
                            && _RoomIdByDepartments.Contains(p.ROOM_ID)
                            ).ToList();
                        }
                        else
                        {
                            dataExecuteRooms = dataExecuteRooms.Where(p => p.IS_PAUSE_ENCLITIC != 1
                            && roomIdActivesV2.Contains(p.ROOM_ID)
                            ).ToList();
                        }
                    }
                    ado.LHisRoomCounters = dataExecuteRooms;
                }                

                ado.HisServiceRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>().Where(o => o.BRANCH_ID == WorkPlace.GetBranchId() && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH && o.IS_ACTIVE == 1).ToList();
                //Lấy danh sách các service & room đang hoạt động (IS_ACTIVE = 1), sau đó lọc các HisServiceRooms
                var roomIdActives = ado.LHisRoomCounters.Select(o => o.ROOM_ID).ToList();
                var serviceIdActives = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH && o.IS_ACTIVE == 1).Select(o => o.ID).ToList();
                ado.HisServiceRooms = ado.HisServiceRooms.Where(o =>
                    roomIdActives != null && roomIdActives.Contains(o.ROOM_ID)
                    && serviceIdActives != null
                    && serviceIdActives.Contains(o.SERVICE_ID)).ToList();

                SetDataInit(ado);
                if (ado != null)
                {
                    //this.VisibilityControl();
                    this.InitComboRoom();
                    this.InitComboExamService();
                    this.InitData();
                    this.Init();
                    this.SetCaptionByLanguageKey();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<long> GetPatientTypeRoom1(long _patientTypeId)
        {
            List<long> _roomIdByPatientTypeRooms = new List<long>();
            try
            {
                MOS.Filter.HisPatientTypeRoomFilter _Filter = new MOS.Filter.HisPatientTypeRoomFilter();
                _Filter.PATIENT_TYPE_ID = _patientTypeId;
                _Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var datas = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ROOM>>("api/HisPatientTypeRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, _Filter, null);
                if (datas != null && datas.Count > 0)
                {
                    _roomIdByPatientTypeRooms = datas.Select(p => p.ROOM_ID).Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return _roomIdByPatientTypeRooms;
        }

        public void InitLoad(RoomExamServiceInitADO ado)
        {
            try
            {
                txtRoomCode.Text = "";
                cboRoom.EditValue = null;
                txtExamServiceCode.Text = "";
                cboExamService.EditValue = null;
                SetDataInit(ado);
                if (ado != null)
                {
                    this.VisibilityControl();
                    this.InitComboRoom();
                    this.InitComboExamService();
                    this.InitData();
                    this.Init();
                    this.SetCaptionByLanguageKey();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void Init()
        {
            try
            {
                if (this.sereServExam != null)
                {
                    if (!this.dicExecuteRoom.ContainsKey(this.sereServExam.TDL_EXECUTE_ROOM_ID))
                        return;
                    if (!this.dicRoomService.ContainsKey(this.sereServExam.TDL_EXECUTE_ROOM_ID))
                        return;
                    var room = this.dicExecuteRoom[this.sereServExam.TDL_EXECUTE_ROOM_ID];
                    var service = this.dicRoomService[this.sereServExam.TDL_EXECUTE_ROOM_ID].FirstOrDefault(o => o.SERVICE_ID == this.sereServExam.SERVICE_ID);
                    if (service != null)
                    {
                        this.serviceId = service.SERVICE_ID;
                    }
                    this.cboRoom.EditValue = room.ROOM_ID;

                    this.sereServExam = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void VisibilityControl()
        {
            try
            {
                if (this.isInit)
                {
                    this.lciBtnDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    this.lciBtnDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExamService()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 60, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 280, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", columnInfos, false, 340);
                ControlEditorLoader.Load(this.cboExamService, this.currentServiceRooms, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboRoom()
        {
            try
            {
                if (this.roomExamServiceInitADO.HisExecuteRooms != null && this.roomExamServiceInitADO.HisExecuteRooms.Count > 0)
                {
                    InitComboRoom(this.roomExamServiceInitADO.HisExecuteRooms);
                }
                else if (this.roomExamServiceInitADO.LHisRoomCounters != null && this.roomExamServiceInitADO.LHisRoomCounters.Count > 0)
                {
                    InitComboRoom(this.roomExamServiceInitADO.LHisRoomCounters);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitComboRoom(List<MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER> executeRooms)
        {
            try
            {
                this.cboRoom.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
                executeRooms = executeRooms ?? new List<MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER>();
                this.roomExts = (from m in executeRooms select new RoomExtADO(m)).OrderByDescending(o => o.NUM_ORDER).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "Mã", 100, 1, true));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "Tên", 400, 2, true));
                columnInfos.Add(new ColumnInfo("TOTAL_TODAY_SERVICE_REQ", "Tổng", 100, 3, true));
                columnInfos.Add(new ColumnInfo("TOTAL_NEW_SERVICE_REQ", "Chưa", 100, 4, true));
                columnInfos.Add(new ColumnInfo("TOTAL_WAIT_TODAY_SERVICE_REQ", "CLS", 100, 5, true));
                columnInfos.Add(new ColumnInfo("TOTAL_OPEN_SERVICE_REQ", "Đã", 100, 6, true));
                columnInfos.Add(new ColumnInfo("MAX_REQ_BHYT_BY_DAY", "Tối đa", 100, 7, true));
                columnInfos.Add(new ColumnInfo("RESPONSIBLE_USERNAME", "Tên bác sỹ", 350, 8, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, true, 1250);
                ControlEditorLoader.Load(this.cboRoom, this.roomExts, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitComboRoom(List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM_1> executeRooms)
        {
            try
            {
                this.cboRoom.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
                executeRooms = executeRooms ?? new List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM_1>();
                this.roomExts = (from m in executeRooms select new RoomExtADO(m)).OrderByDescending(o => o.NUM_ORDER).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 100, 1, true));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 500, 2, true));
                columnInfos.Add(new ColumnInfo("AMOUNT_COMBO", "", 100, 3, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, false, 700);
                ControlEditorLoader.Load(this.cboRoom, this.roomExts, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
