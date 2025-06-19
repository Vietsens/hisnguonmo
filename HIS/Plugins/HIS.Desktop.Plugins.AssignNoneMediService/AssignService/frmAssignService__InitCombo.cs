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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.AssignNoneMediService.ADO;
using HIS.Desktop.Plugins.AssignNoneMediService.Config;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNoneMediService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        private List<HIS_TEST_SAMPLE_TYPE> dataListTestSampleType;

        //Load người chỉ định
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetExroRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                V_HIS_ROOM currentWorkingRoom = null;
                if (cboAssignRoom.EditValue != null)
                {
                    currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == Convert.ToInt64(cboAssignRoom.EditValue));
                }
                else
                {
                    currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == this.currentModule.RoomId);
                }
                if (currentWorkingRoom != null)
                {
                    CommonParam paramGet = new CommonParam();
                    MOS.Filter.HisExroRoomFilter exroRoomFilter = new MOS.Filter.HisExroRoomFilter();
                    exroRoomFilter.ROOM_ID = currentWorkingRoom.ID;
                    exroRoomFilter.IS_ACTIVE = 1;
                    this.exroRooms = new BackendAdapter(paramGet).Get<List<HIS_EXRO_ROOM>>("api/HisExroRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, exroRoomFilter, paramGet);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => paramGet), paramGet));
                    dteCommonParam = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(paramGet.Now) ?? DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        /// <summary>
        /// hàm add thêm HIS_BED_ROOM vào V_HIS_EXECUTE_ROOM để hiển thị được đúng phòng xử lý theo ROOM_ID
        /// chỉ add buồng thuộc khoa để không ảnh hưởng chỉ định hiện tại
        /// 
        /// </summary>
        /// <param name="roomIds">phòng theo dịch vụ phòng</param>
        /// <param name="executeRooms"></param>
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

    }
}
