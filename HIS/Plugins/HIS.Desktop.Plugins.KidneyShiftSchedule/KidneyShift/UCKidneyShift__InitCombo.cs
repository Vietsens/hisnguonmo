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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.KidneyShiftSchedule.ADO;
using HIS.Desktop.Plugins.KidneyShiftSchedule.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KidneyShiftSchedule.KidneyShift
{
    public partial class UCKidneyShift : UserControlBase
    {
        private async Task InitComboUser()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitComboUser. 1");
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboUser, datas, controlEditorADO);

                if (this.currentHisTreatment != null && !String.IsNullOrEmpty(this.currentHisTreatment.PREVIOUS_END_LOGINNAME))
                {
                    this.cboUser.EditValue = this.currentHisTreatment.PREVIOUS_END_LOGINNAME;
                    this.txtLoginName.Text = this.currentHisTreatment.PREVIOUS_END_LOGINNAME;
                }
                else
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var data = datas.Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).FirstOrDefault();
                    if (data != null)
                    {
                        this.cboUser.EditValue = data.LOGINNAME;
                        this.txtLoginName.Text = data.LOGINNAME;
                    }
                }

                //- Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
                //- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn       
                //- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
                this.cboUser.Enabled = (HisConfigCFG.ShowRequestUser == GlobalVariables.CommonStringTrue);
                this.txtLoginName.Enabled = (HisConfigCFG.ShowRequestUser == GlobalVariables.CommonStringTrue);
                Inventec.Common.Logging.LogSystem.Debug("InitComboUser. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDayOfWeek(DevExpress.XtraEditors.GridLookUpEdit cboDofWeek)
        {
            try
            {
                var data = new DayofWeekADO().DayofWeekADOs;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DayofWeek", "", 150, 1));
                Inventec.Common.Controls.EditorLoader.ControlEditorADO controlEditorADO = new ControlEditorADO("DayofWeek", "Day", columnInfos, false, 150);
                ControlEditorLoader.Load(cboDofWeek, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCa(DevExpress.XtraEditors.GridLookUpEdit cboCa)
        {
            try
            {
                var data = new CaADO().CaADOs;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Value", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Value", "Value", columnInfos, false, 150);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                ControlEditorLoader.Load(cboCa, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientType(List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(this.cboPatientType, (data != null ? data.OrderBy(o => o.PRIORITY).ToList() : null), controlEditorADO);
                }
                else
                    ControlEditorLoader.Load(this.cboPatientType, this.currentPatientTypeWithPatientTypeAlter, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitComboExpmestTemplate()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE> datas = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>>("api/HisExpMestTemplate/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_TEMPLATE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_TEMPLATE_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_TEMPLATE_NAME", "ID", columnInfos, false, 400);
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var expMestTemplates = datas.Where(o => (o.CREATOR == loginName || (o.IS_PUBLIC ?? -1) == GlobalVariables.CommonNumberTrue) && o.IS_KIDNEY == GlobalVariables.CommonNumberTrue && o.IS_ACTIVE == GlobalVariables.CommonNumberTrue).ToList();
                ControlEditorLoader.Load(cboExpMestTemplateForAdd, expMestTemplates, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExecuteRoom(DevExpress.XtraEditors.GridLookUpEdit cboExecuteRoom)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> executeRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();

                executeRooms = (executeRooms != null && executeRooms.Count > 0) ? executeRooms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.DEPARTMENT_ID == requestRoom.DEPARTMENT_ID && ((o.IS_KIDNEY ?? -1) == 1)).ToList() : executeRooms;
                //TODO
                // order tăng dần theo num_order
                if (executeRooms != null && executeRooms.Count > 0)
                {
                    var serviceGroup1s = executeRooms.Where(u => u.NUM_ORDER != null).OrderBy(u => u.NUM_ORDER).ThenBy(o => o.EXECUTE_ROOM_NAME);
                    var serviceGroup2s = executeRooms.Where(u => u.NUM_ORDER == null).OrderBy(o => o.EXECUTE_ROOM_NAME);
                    var serviceGroupConcats = serviceGroup1s.Concat(serviceGroup2s);
                    cboExecuteRoom.Properties.DataSource = serviceGroupConcats.ToList();
                }
                else
                {
                    cboExecuteRoom.Properties.DataSource = null;
                }
                cboExecuteRoom.Properties.ValueMember = "ROOM_ID";
                cboExecuteRoom.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboExecuteRoom.Properties.View.Columns.AddField("EXECUTE_ROOM_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboExecuteRoom.Properties.View.Columns.AddField("EXECUTE_ROOM_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "Tên";
                cboExecuteRoom.Properties.PopupFormWidth = 320;
                cboExecuteRoom.Properties.View.OptionsView.ShowColumnHeaders = false;
                //cboExecuteRoom.Properties.View.OptionsSelection.MultiSelect = true;
                if (executeRooms != null && executeRooms.Count > 0)
                {
                    cboExecuteRoom.EditValue = executeRooms[0].ROOM_ID;
                    txtExecuteRoom.Text = executeRooms[0].EXECUTE_ROOM_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBedRoom()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM> bedRooms = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>())
                {
                    bedRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    bedRooms = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>>("api/HisBedRoom/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (bedRooms != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_BED_ROOM), bedRooms, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                bedRooms = (bedRooms != null && bedRooms.Count > 0) ? bedRooms.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.DEPARTMENT_ID == this.currentDepartment.ID).ToList() : bedRooms;


                if (bedRooms != null && bedRooms.Count > 0)
                {
                    cboBedroomForPatientInBedroom.Properties.DataSource = bedRooms;
                }
                else
                {
                    cboBedroomForPatientInBedroom.Properties.DataSource = null;
                }

                cboBedroomForPatientInBedroom.Properties.DisplayMember = "BED_ROOM_NAME";
                cboBedroomForPatientInBedroom.Properties.ValueMember = "ROOM_ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboBedroomForPatientInBedroom.Properties.View.Columns.AddField("BED_ROOM_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboBedroomForPatientInBedroom.Properties.View.Columns.AddField("BED_ROOM_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "Tên";
                cboBedroomForPatientInBedroom.Properties.PopupFormWidth = 320;
                cboBedroomForPatientInBedroom.Properties.View.OptionsView.ShowColumnHeaders = false;

                var broom = bedRooms != null ? bedRooms.Where(o => o.ROOM_ID == this.requestRoom.ID).FirstOrDefault() : null;
                cboBedroomForPatientInBedroom.EditValue = broom != null ? (long?)broom.ROOM_ID : null;
                txtBedroomForPatientInBedroom.Text = broom != null ? broom.BED_ROOM_CODE : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboService()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();

                Inventec.Common.Logging.LogSystem.Debug("Du lieu services truoc khi loc count = " + (services != null ? services.Count : 0));
                services = (services != null && services.Count > 0) ? services.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && ((o.IS_KIDNEY ?? -1) == 1) ) .ToList() : services;
                Inventec.Common.Logging.LogSystem.Debug("Du lieu services sau khi loc count = " + (services != null ? services.Count : 0));
                // order tăng dần theo num_order
                   if (services != null && services.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("InitComboService. 1");
                    var serviceGroup1s = services.Where(u => u.NUM_ORDER != null).OrderBy(u => u.NUM_ORDER).ThenBy(o => o.SERVICE_NAME);
                    var serviceGroup2s = services.Where(u => u.NUM_ORDER == null).OrderBy(o => o.SERVICE_NAME);
                    var serviceGroupConcats = serviceGroup1s.Concat(serviceGroup2s);
                    cboServiceForAdd.Properties.DataSource = serviceGroupConcats.ToList();
                    Inventec.Common.Logging.LogSystem.Debug("serviceGroupConcats.count=" + serviceGroupConcats.Count());
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("InitComboService. 2");
                    cboServiceForAdd.Properties.DataSource = null;
                }

                cboServiceForAdd.Properties.DisplayMember = "SERVICE_NAME";
                cboServiceForAdd.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboServiceForAdd.Properties.View.Columns.AddField("SERVICE_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboServiceForAdd.Properties.View.Columns.AddField("SERVICE_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 300;
                col3.Caption = "Tên";
                cboServiceForAdd.Properties.PopupFormWidth = 420;
                cboServiceForAdd.Properties.View.OptionsView.ShowColumnHeaders = false;
                if (services != null && services.Count > 0)
                {
                    cboServiceForAdd.EditValue = services[0].ID;
                    txtServiceForAdd.Text = services[0].SERVICE_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMachine(DevExpress.XtraEditors.GridLookUpEdit cboMachine, bool initial)
        {
            try
            {
                List<HIS_MACHINE> machines = BackendDataWorker.Get<HIS_MACHINE>();
                this.currentMachines = null;
                if (this.cboExecuteRoom.EditValue != null)
                {
                    string filterRoomId = String.Format(",{0},", this.cboExecuteRoom.EditValue);
                    this.currentMachines = (machines != null && machines.Count > 0) ? machines.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && ((o.IS_KIDNEY ?? -1) == 1) && !String.IsNullOrWhiteSpace(o.ROOM_IDS) && String.Format(",{0},", o.ROOM_IDS).Contains(filterRoomId)).ToList() : machines;
                }

                if (this.currentMachines != null && this.currentMachines.Count > 0)
                {
                    cboMachine.Properties.DataSource = this.currentMachines;
                }
                else
                {
                    cboMachine.Properties.DataSource = null;
                }
                if (initial)
                {
                    cboMachine.Properties.DisplayMember = "MACHINE_NAME";
                    cboMachine.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboMachine.Properties.View.Columns.AddField("MACHINE_CODE");
                    col2.VisibleIndex = 1;
                    col2.Width = 100;
                    col2.Caption = "Mã";
                    DevExpress.XtraGrid.Columns.GridColumn col3 = cboMachine.Properties.View.Columns.AddField("MACHINE_NAME");
                    col3.VisibleIndex = 2;
                    col3.Width = 200;
                    col3.Caption = "Tên";
                    cboMachine.Properties.PopupFormWidth = 320;
                    cboMachine.Properties.View.OptionsView.ShowColumnHeaders = false;
                    //cboExecuteRoom.Properties.View.OptionsSelection.MultiSelect = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
