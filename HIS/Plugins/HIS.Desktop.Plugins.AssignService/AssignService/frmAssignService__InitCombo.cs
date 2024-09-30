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
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.AssignService.Config;
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

namespace HIS.Desktop.Plugins.AssignService.AssignService
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
                this.cboUser.Enabled = (HisConfigCFG.ShowRequestUser == commonString__true);
                this.txtLoginName.Enabled = (HisConfigCFG.ShowRequestUser == commonString__true);

                ////Nguoi Tu Van            
                List<AcsUserADO> consultantUserData = new List<AcsUserADO>();

                //var consultantUserHasEmp = (dataUsers != null ? dataUsers.Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).ToList() : null);
                //Inventec.Common.Logging.LogSystem.Debug("InitComboUser.5.1__consultantUserHasEmp.count =" + (consultantUserHasEmp != null ? consultantUserHasEmp.Count : 0));
                consultantUserData = (from o in dataUsers
                                      join k in employees on o.LOGINNAME.ToUpper() equals k.LOGINNAME.ToUpper()
                                      where
                                       (ckTK.Checked ? k.DEPARTMENT_ID == currentDepartment.ID : true)
                                      select new AcsUserADO(o, k)
                                            ).ToList();
                var cultantUserData_ = consultantUserData.Where(o => o.IS_ACTIVE == 1 && o.IS_ACTIVE_EMPLOYEE == 1).ToList();
                Inventec.Common.Logging.LogSystem.Debug("InitComboUser.6__consultantUserData.count =" + (consultantUserData != null ? consultantUserData.Count : 0));

                List<ColumnInfo> columnInfoConsultants = new List<ColumnInfo>();
                columnInfoConsultants.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 100, 1));
                columnInfoConsultants.Add(new ColumnInfo("USERNAME", "Họ tên", 300, 2));
                columnInfoConsultants.Add(new ColumnInfo("DEPARTMENT_NAME", "Khoa phòng", 250, 3));
                ControlEditorADO controlEditorConsultantADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfoConsultants, true, 750);
                ControlEditorLoader.Load(this.cboConsultantUser, cultantUserData_, controlEditorConsultantADO);

                Inventec.Common.Logging.LogSystem.Debug("InitComboUser.7");

                var oneConsultant = (consultantUserData != null && consultantUserData.Count > 0) ? consultantUserData.Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).FirstOrDefault() : null;
                if (oneConsultant != null)
                {
                    this.cboConsultantUser.EditValue = oneConsultant.LOGINNAME;
                    this.txtConsultantLoginname.Text = oneConsultant.LOGINNAME;
                }
                Inventec.Common.Logging.LogSystem.Debug("InitComboUser.8");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessComboConsultant()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessComboConsultant.1");
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                List<ACS.EFMODEL.DataModels.ACS_USER> dataUsers = new List<ACS.EFMODEL.DataModels.ACS_USER>();

                dataUsers = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                Inventec.Common.Logging.LogSystem.Debug("ProcessComboConsultant.2__dataUsers.count =" + (dataUsers != null ? dataUsers.Count : 0));

                var employees = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>();

                ////Nguoi Tu Van            
                List<AcsUserADO> consultantUserData = new List<AcsUserADO>();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                consultantUserData = (from o in dataUsers
                                      join k in employees on o.LOGINNAME.ToUpper() equals k.LOGINNAME.ToUpper()
                                      where
                                       (ckTK.Checked ? k.DEPARTMENT_ID == currentDepartment.ID : true)
                                      select new AcsUserADO(o, k)
                                            ).ToList();
                var consultantUserData_ = consultantUserData.Where(o => o.IS_ACTIVE == 1 && o.IS_ACTIVE_EMPLOYEE == 1).ToList();
                Inventec.Common.Logging.LogSystem.Debug("ProcessComboConsultant.4__consultantUserData.count =" + (consultantUserData != null ? consultantUserData.Count : 0));

                List<ColumnInfo> columnInfoConsultants = new List<ColumnInfo>();
                columnInfoConsultants.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 100, 1));
                columnInfoConsultants.Add(new ColumnInfo("USERNAME", "Họ tên", 300, 2));
                columnInfoConsultants.Add(new ColumnInfo("DEPARTMENT_NAME", "Khoa phòng", 250, 3));
                ControlEditorADO controlEditorConsultantADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfoConsultants, true, 750);
                ControlEditorLoader.Load(this.cboConsultantUser, consultantUserData_, controlEditorConsultantADO);

                Inventec.Common.Logging.LogSystem.Debug("ProcessComboConsultant.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientType(GridLookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, (data != null ? data.OrderBy(o => o.PRIORITY).ToList() : null), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
        private void InitComboRepositoryTestSampleType(List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TEST_SAMPLE_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repSampleType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task InitComboRepositoryPatientType(List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(this.repositoryItemcboPatientType_TabService, (data != null ? data.OrderBy(o => o.PRIORITY).ToList() : null), controlEditorADO);
                    ControlEditorLoader.Load(this.repositoryItemCboPatientTypeReadOnly, (data != null ? data.OrderBy(o => o.PRIORITY).ToList() : null), controlEditorADO);
                }
                else
                {
                    ControlEditorLoader.Load(this.repositoryItemcboPatientType_TabService, this.currentPatientTypeWithPatientTypeAlter, controlEditorADO);
                    ControlEditorLoader.Load(this.repositoryItemCboPatientTypeReadOnly, this.currentPatientTypeWithPatientTypeAlter, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRepositoryCondition(List<MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION> data)
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("SERVICE_CONDITION_CODE", "", 100, 1));
                //columnInfos.Add(new ColumnInfo("SERVICE_CONDITION_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_CONDITION_NAME", "ID", columnInfos, false, 350);
                //if (data != null && data.Count > 0)
                //{
                //    ControlEditorLoader.Load(this.repositoryItemcboCondition, (data != null ? data.ToList() : null), controlEditorADO);
                //}
                //else
                //{
                //    ControlEditorLoader.Load(this.repositoryItemcboCondition, serviceConditions, controlEditorADO);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRepositoryCondition(GridLookUpEdit cbo, List<MOS.EFMODEL.DataModels.HIS_SERVICE_CONDITION> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CONDITION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_CONDITION_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_CONDITION_NAME", "ID", columnInfos, false, 350);
                if (data != null && data.Count > 0)
                {
                    ControlEditorLoader.Load(cbo, (data != null ? data.ToList() : null), controlEditorADO);
                }
                else
                {
                    ControlEditorLoader.Load(cbo, null, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitComboPrimaryPatientType(List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(this.repositoryItemCboPrimaryPatientType, (data != null ? data.OrderBy(o => o.PRIORITY).ToList() : null), controlEditorADO);
                }
                else
                    ControlEditorLoader.Load(this.repositoryItemCboPrimaryPatientType, this.currentPatientTypeWithPatientTypeAlter, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboKsk()
		{
			try
			{
                PatientKskCode = HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK");
                if (currentTreatment != null && currentTreatment.TDL_PATIENT_TYPE_ID !=null)
				{
                    List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataPatientType = null;
                    if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>())
                    {
                        dataPatientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                    }
                    else
                    {
                        CommonParam paramCommon = new CommonParam();
                        dynamic filter = new System.Dynamic.ExpandoObject();
                        dataPatientType = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                        if (dataPatientType != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE), dataPatientType, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                    }

                    if (dataPatientType != null && dataPatientType.Count > 0)
                    {
                        dataPatientType = dataPatientType.Where(o => o.IS_ACTIVE == 1).ToList();
                    }
                    if (dataPatientType.FirstOrDefault(o => o.ID == currentTreatment.TDL_PATIENT_TYPE_ID).PATIENT_TYPE_CODE == HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK"))
					{
						List<ColumnInfo> columnInfos = new List<ColumnInfo>();
						columnInfos.Add(new ColumnInfo("KSK_CODE", "", 100, 1));
						columnInfos.Add(new ColumnInfo("KSK_NAME", "", 250, 2));
						ControlEditorADO controlEditorADO = new ControlEditorADO("KSK_NAME", "ID", columnInfos, false, 350);

                        List<MOS.EFMODEL.DataModels.HIS_KSK> data = null;
                        if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_KSK>())
                        {
                            data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_KSK>();
                        }
                        else
                        {
                            CommonParam paramCommon = new CommonParam();
                            dynamic filter = new System.Dynamic.ExpandoObject();
                            data = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_KSK>>("api/HisKsk/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                            if (data != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_KSK), data, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                        }

                        if (data != null && data.Count > 0)
                        {
                            data = data.ToList().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && (o.KSK_CONTRACT_ID == currentTreatment.TDL_KSK_CONTRACT_ID || o.KSK_CONTRACT_ID == null)).ToList();
                            ControlEditorLoader.Load(this.cboKsk, data.ToList(), controlEditorADO);
                        }                    						
					}
				}                    
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex); ;
			}
		}

        private async Task InitComboExecuteGroup()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitComboExecuteGroup. 1");

                List<MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP> datas = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP>>("api/HisExecuteGroup/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EXECUTE_GROUP), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_GROUP_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboExecuteGroup, datas, controlEditorADO);
                Inventec.Common.Logging.LogSystem.Debug("InitComboExecuteGroup. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    //this.exroRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXRO_ROOM>().Where(o => o.ROOM_ID == currentWorkingRoom.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

        private void InitGridCheckMarksSelectionServiceGroup()
        {
            try
            {
                //this.cboServiceGroup.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboServiceGroup_CustomDisplayText);
                //GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboServiceGroup.Properties);
                //gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(cboServiceGroup__SelectionChange);
                //cboServiceGroup.Properties.Tag = gridCheck;
                //cboServiceGroup.Properties.View.OptionsSelection.MultiSelect = true;
                //GridCheckMarksSelection gridCheckMark = cboServiceGroup.Properties.Tag as GridCheckMarksSelection;
                ////if (gridCheckMark != null)
                ////    gridCheckMark.ClearSelection(cboServiceGroup.Properties.View);
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
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField("SERVICE_GROUP_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "Mã nhóm";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cbo.Properties.View.Columns.AddField("SERVICE_GROUP_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "Tên nhóm";

                cbo.Properties.PopupFormWidth = 320;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = false;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                //GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //{
                //    gridCheckMark.ClearSelection(cbo.Properties.View);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        //Load nhóm dịch vụ, chỉ load ra các nhóm dịch vụ do người dùng tạo hoặc is_public = 1
        private async Task InitComboServiceGroup()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitComboServiceGroup. 1");

                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                List<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP> serviceGroups = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>())
                {
                    serviceGroups = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    serviceGroups = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>>("api/HisServiceGroup/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (serviceGroups != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP), serviceGroups, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                serviceGroups = (serviceGroups != null && serviceGroups.Count > 0) ?
                    serviceGroups.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE &&
                        ((o.IS_PUBLIC ?? -1) == 1 || o.CREATOR.ToLower() == loginName.ToLower()))
                        .ToList()
                : serviceGroups;

                // order tăng dần theo num_order
                if (serviceGroups != null && serviceGroups.Count > 0)
                {
                    var serviceGroup1s = serviceGroups.Where(u => u.NUM_ORDER != null).OrderBy(u => u.NUM_ORDER).ThenBy(o => o.SERVICE_GROUP_NAME);
                    var serviceGroup2s = serviceGroups.Where(u => u.NUM_ORDER == null).OrderBy(o => o.SERVICE_GROUP_NAME);
                    var serviceGroupConcats = serviceGroup1s.Concat(serviceGroup2s);
                    //cboServiceGroup.Properties.DataSource = serviceGroupConcats.ToList();
                    var sgDatas = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    if (sgDatas != null && sgDatas.Count > 0)
                        this.workingServiceGroupADOs = (from m in serviceGroupConcats
                                                        select new ServiceGroupADO()
                                                        {
                                                            ID = m.ID,
                                                            SERVICE_GROUP_CODE = m.SERVICE_GROUP_CODE,
                                                            SERVICE_GROUP_NAME = m.SERVICE_GROUP_NAME,
                                                            PARENT_SERVICE_ID = m.PARENT_SERVICE_ID,
                                                            IS_PUBLIC = m.IS_PUBLIC,
                                                            NUM_ORDER = m.NUM_ORDER,
                                                            SERVICE_GROUP_NAME__UNSIGN = Inventec.Common.String.Convert.UnSignVNese2(m.SERVICE_GROUP_NAME)
                                                        }).ToList();
                }
                else
                {
                    //cboServiceGroup.Properties.DataSource = null;
                }

                //InitCombo(cboServiceGroup, serviceGroups, "SERVICE_GROUP_NAME", "ID");
                InitComboServiceRoom();
                this.selectedSeviceGroups = null;
                Inventec.Common.Logging.LogSystem.Debug("InitComboServiceGroup. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit GenerateRepositoryItemCheckEdit()
        {
            DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            return repositoryItemCheckEdit1;
        }

        private void AddFieldColumnIntoComboRoomExt(string FieldName, string Caption, int Width, int VisibleIndex, bool FixedWidth, DevExpress.Data.UnboundColumnType? UnboundType = null, bool allowEdit = false)
        {
            DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
            col2.FieldName = FieldName;
            col2.Caption = Caption;
            col2.Width = Width;
            col2.VisibleIndex = VisibleIndex;
            col2.OptionsColumn.FixedWidth = FixedWidth;
            if (UnboundType != null)
                col2.UnboundType = UnboundType.Value;
            col2.OptionsColumn.AllowEdit = allowEdit;
            if (FieldName == "IsChecked")
            {
                col2.ColumnEdit = GenerateRepositoryItemCheckEdit();
                col2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                col2.OptionsFilter.AllowFilter = false;
                col2.OptionsFilter.AllowAutoFilter = false;
                col2.ImageAlignment = StringAlignment.Center;
                col2.Image = imageCollection1.Images[0];
                col2.OptionsColumn.AllowEdit = false;
            }

            gridViewContainerRoom.Columns.Add(col2);
        }

        public void InitComboServiceRoom()
        {
            try
            {
                popupHeight = (this.workingServiceGroupADOs != null && this.workingServiceGroupADOs.Count > 15) ? 400 : 200;
                gridViewContainerRoom.BeginUpdate();
                gridViewContainerRoom.Columns.Clear();
                gridViewContainerRoom.OptionsView.ShowColumnHeaders = false;
                popupControlContainerRoom.Width = 400;
                popupControlContainerRoom.Height = popupHeight;
                int columnIndex = 1;
                AddFieldColumnIntoComboRoomExt("IsChecked", " ", 30, columnIndex++, true, null, true);
                AddFieldColumnIntoComboRoomExt("SERVICE_GROUP_CODE", "Mã", 90, columnIndex++, true);
                AddFieldColumnIntoComboRoomExt("SERVICE_GROUP_NAME", "Tên", 270, columnIndex++, true);

                gridViewContainerRoom.GridControl.DataSource = this.workingServiceGroupADOs;

                gridViewContainerRoom.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewContainerRoom.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Load gói dịch vụ, chỉ load ra các gói dịch vụ có is_active = 1
        private async Task InitComboPackage()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitComboPackage. 1");

                List<HIS_PACKAGE> packages = null;
                if (BackendDataWorker.IsExistsKey<HIS_PACKAGE>())
                {
                    packages = BackendDataWorker.Get<HIS_PACKAGE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    List<HIS_PACKAGE> lstPackage = await new BackendAdapter(paramCommon).GetAsync<List<HIS_PACKAGE>>("api/HisPackage/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (lstPackage != null) BackendDataWorker.UpdateToRam(typeof(HIS_PACKAGE), lstPackage, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                    packages = lstPackage;
                }

                packages = packages != null ? packages.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NOT_FIXED_SERVICE != (short)1).ToList() : packages;

                // order tăng dần theo num_order
                if (packages != null && packages.Count > 0)
                {
                    packages = packages.OrderBy(o => o.PACKAGE_NAME).ToList();
                    cboPackage.Properties.DataSource = packages;
                }
                else
                {
                    cboPackage.Properties.DataSource = null;
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PACKAGE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PACKAGE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PACKAGE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPackage, packages, controlEditorADO);

                Inventec.Common.Logging.LogSystem.Debug("InitComboPackage. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitPackageDetail()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitPackageDetail. 1");

                if (!BackendDataWorker.IsExistsKey<V_HIS_PACKAGE_DETAIL>())
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    List<V_HIS_PACKAGE_DETAIL> lstPackageDetail = await new BackendAdapter(paramCommon).GetAsync<List<V_HIS_PACKAGE_DETAIL>>("api/HisPackageDetail/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (lstPackageDetail != null) BackendDataWorker.UpdateToRam(typeof(V_HIS_PACKAGE_DETAIL), lstPackageDetail, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                Inventec.Common.Logging.LogSystem.Debug("InitPackageDetail. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridParticipants()
        {
            try
            {


                var datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().ToList();
                //var datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM>().Select(o => o.DEPARTMENT_ID).ToList();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
