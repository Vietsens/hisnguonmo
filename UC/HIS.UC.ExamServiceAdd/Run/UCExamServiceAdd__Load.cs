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
using MOS.SDO;
using HIS.UC.ExamServiceAdd.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.HisExamServiceAdd.Config;

namespace HIS.UC.ExamServiceAdd.Run
{

    public partial class UCExamServiceAdd : UserControl
    {
        private void LoadDataCboExecuteRoom()
        {
            try
            {
                long branchId = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                if (examServiceAddInitADO == null && branchId <= 0)// || !examServiceAddInitADO.roomId.HasValue)
                    throw new Exception("branchId null");
                List<V_HIS_EXECUTE_ROOM> roomExams = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(
                    o => o.IS_EXAM == 1 && o.IS_ACTIVE == 1
                        && o.IS_PAUSE_ENCLITIC != 1
                        && o.ROOM_ID != examServiceAddInitADO.roomId
                        )
                        .ToList();// && o.ROOM_ID == examServiceAddInitADO.roomId).ToList();
                if(chkIsBranch.Checked)
				{
                    roomExams = roomExams.Where(o => o.BRANCH_ID == branchId).ToList();
				}                    
                if (chk_IsDepartment.Checked)
                {
                    var dataRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == examServiceAddInitADO.roomId);
                    roomExams = roomExams.Where(o => o.DEPARTMENT_ID == dataRoom.DEPARTMENT_ID).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboExecuteRoom, roomExams, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCboExamService(V_HIS_EXECUTE_ROOM executeRoom)
        {
            try
            {
                cboExamService.EditValue = null;
                txtService.Text = "";

                if (executeRoom == null)
                {
                    return;
                }
                IsAllowNotChooseService = false;
                if (executeRoom.ALLOW_NOT_CHOOSE_SERVICE == 1)
                {
                    IsAllowNotChooseService = true;
                    this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.layoutControlItem9.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    RemoveValidateControl(cboPatientType);
                    RemoveValidateControl(txtService);
                }
                else
                {
                    this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.layoutControlItem9.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    ValidateSingleControl(cboPatientType);
                    ValidateGridLookupWithTextEdit(cboExamService, txtService);
                }

                List<long> servicePatyIds = this.currentServicePatys.Select(o => o.SERVICE_ID).Distinct().ToList();

                Inventec.Common.Logging.LogSystem.Debug("LoadDataCboExamService 1");
                List<long> serviceIds = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == 1).Select(p => p.ID).ToList();
                Inventec.Common.Logging.LogSystem.Debug("LoadDataCboExamService 2");
                serviceRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(
                            o => o.ROOM_ID == executeRoom.ROOM_ID
                            && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                            && servicePatyIds.Contains(o.SERVICE_ID)
                            && serviceIds.Contains(o.SERVICE_ID)
                            && o.IS_ACTIVE == 1).ToList();
                if(chkIsBranch.Checked)
				{
                    serviceRooms = serviceRooms.Where(o=>o.BRANCH_ID == HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId()).ToList();

                }                    
                Inventec.Common.Logging.LogSystem.Debug("LoadDataCboExamService 3");
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboExamService, serviceRooms, controlEditorADO);

                if (serviceRooms != null && serviceRooms.Count == 1)
                {
                    cboExamService.EditValue = serviceRooms[0].SERVICE_ID;
                    txtService.Text = serviceRooms[0].SERVICE_CODE;
                }
                else if (serviceRooms != null && serviceRooms.Count > 1)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisRoomFilter filter = new MOS.Filter.HisRoomFilter();
                    filter.IDs = this.serviceRooms.Select(o => o.ROOM_ID).ToList();
                    var checkDfRoom = new BackendAdapter(param).Get<List<HIS_ROOM>>("api/HisRoom/Get", ApiConsumers.MosConsumer, filter, param);
                    if (checkDfRoom != null && checkDfRoom.Count > 0)
                    {
                        HIS_ROOM currentRoom = checkDfRoom.First();
                        var lstServiceRoom = this.serviceRooms.Where(o => o.SERVICE_ID == currentRoom.DEFAULT_SERVICE_ID);
                        if (lstServiceRoom != null && lstServiceRoom.Count() > 0)
                        {
                            txtService.Text = lstServiceRoom.FirstOrDefault().SERVICE_CODE;
                            cboExamService.EditValue = lstServiceRoom.FirstOrDefault().SERVICE_ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadExamRoom(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboExecuteRoom.Focus();
                    cboExecuteRoom.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.EXECUTE_ROOM_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboExecuteRoom.EditValue = data[0].ROOM_ID;
                            LoadDataCboExamService(data[0]);
                            txtService.Focus();
                            txtService.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EXECUTE_ROOM_CODE == searchCode);
                            if (search != null)
                            {
                                cboExecuteRoom.EditValue = search.ROOM_ID;
                                LoadDataCboExamService(search);
                                txtService.Focus();
                                txtService.SelectAll();
                            }
                            else
                            {
                                cboExecuteRoom.EditValue = null;
                                cboExecuteRoom.Focus();
                                cboExecuteRoom.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboExecuteRoom.EditValue = null;
                        cboExecuteRoom.Focus();
                        cboExecuteRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadService(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboExamService.Focus();
                    cboExamService.ShowPopup();
                    gridLookUpEdit2View.FocusedRowHandle = 0;
                }
                else
                {
                    var data = serviceRooms.Where(o => o.SERVICE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboExamService.EditValue = data[0].SERVICE_ID;
                            cboPatientType.Focus();
                            cboPatientType.ShowPopup();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.SERVICE_CODE == searchCode);
                            if (search != null)
                            {
                                cboExamService.EditValue = search.SERVICE_ID;
                                cboPatientType.Focus();
                                cboPatientType.ShowPopup();
                            }
                            else
                            {
                                cboExamService.EditValue = null;
                                cboExamService.Focus();
                                cboExamService.ShowPopup();
                                gridLookUpEdit2View.FocusedRowHandle = 0;
                            }
                        }
                    }
                    else
                    {
                        cboExamService.EditValue = null;
                        cboExamService.Focus();
                        cboExamService.ShowPopup();
                        gridLookUpEdit2View.FocusedRowHandle = 0;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadSereServ()
        {
            try
            {
                if (examServiceAddInitADO.ServiceReqId.HasValue)
                {
                    CommonParam param = new CommonParam();
                    HisSereServFilter sereServFilter = new HisSereServFilter();
                    sereServFilter.SERVICE_REQ_ID = examServiceAddInitADO.ServiceReqId.Value;
                    sereServ = new BackendAdapter(param)
                 .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentPatientTypeAlter()
        {
            try
            {
                if (!examServiceAddInitADO.treatmentId.HasValue)
                {
                    Inventec.Common.Logging.LogSystem.Error("treatmentId is null");
                }

                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = examServiceAddInitADO.treatmentId.Value;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                currentPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);


                List<V_HIS_PATIENT_TYPE_ALLOW> patientTypeAllows = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALLOW>();
                if (patientTypeAllows == null || patientTypeAllows.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach doi tuong chuyen doi");
                    return;
                }
                //List<long> currentPatientTypeIds;
                //List<V_HIS_SERVICE_PATY> servicePatys { get; set; }
                currentPatientTypeIds = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == currentPatientTypeAlter.PATIENT_TYPE_ID).Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList();
                if (currentPatientTypeIds == null || currentPatientTypeIds.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay doi tuong thanh toan chuyen doi. currentPatientTypeAlter.PATIENT_TYPE_ID : " + currentPatientTypeAlter.PATIENT_TYPE_ID);
                    return;
                }



                currentServicePatys = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                                        .Where(t => (currentPatientTypeIds.Contains(t.PATIENT_TYPE_ID) || BranchDataWorker.CheckPatientTypeInherit(t.INHERIT_PATIENT_TYPE_IDS, currentPatientTypeIds.ToList()))
                                            && t.IS_ACTIVE == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonNumberTrue
                                           
                    //&& t.SERVICE_ID == serviceId
                                            ).ToList();
                if(chkIsBranch.Checked)
				{
                    currentServicePatys = currentServicePatys.Where(t=> t.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()).ToList();

                }                    
                if (currentServicePatys == null || currentServicePatys.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach chinh sach gia dich vu");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public async Task LoadCboPatientTypePrimary(long serviceId)
        {
            try
            {
                var selectedService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == serviceId).FirstOrDefault();
                var selectedPatientTypeID = cboPatientType.EditValue != null ? Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()) : 0;

                List<long> patientTypeIds = new List<long>();

                List<V_HIS_PATIENT_TYPE_ALLOW> patientTypeAllows = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALLOW>();
                if (patientTypeAllows == null || patientTypeAllows.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach doi tuong chuyen doi");
                    return;
                }

                patientTypeIds = patientTypeAllows.Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList();
                if (patientTypeIds == null || patientTypeIds.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay doi tuong thanh toan chuyen doi. currentPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID : " + currentPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID);
                    return;
                }

                var servicePatys = BranchDataWorker.ServicePatyWithListPatientType(serviceId, patientTypeIds);
                if (servicePatys == null || servicePatys.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach chinh sach gia dich vu");
                    return;
                }

                patientTypeIds = servicePatys.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();



                var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                var patientTypeIdPlusAfterFilter = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && patientTypeIds.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                if (patientTypeIdPlusAfterFilter != null && patientTypeIdPlusAfterFilter.Count > 0)
                {
                    patientTypeIds.AddRange(patientTypeIdPlusAfterFilter.Select(o => o.ID));
                }
                patientTypeIds = patientTypeIds.Distinct().ToList();


                long currentPatientTypeID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString());
                List<HIS_PATIENT_TYPE> patientTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                                                    .Where(o => o.IS_ADDITION == (short)1
                                                        && patientTypeIds.Contains(o.ID)
                                                        && o.ID != currentPatientTypeID).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPrimaryPatientType, patientTypes, controlEditorADO);

                if (HisConfig.IsSetPrimaryPatientType == "2")
                {
                    if (currentPatientTypeAlter != null)
                    {
                        cboPrimaryPatientType.EditValue = patientTypes != null && patientTypes.Exists(t => t.ID == currentPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID) ? (long?)currentPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID : null;
                        cboPrimaryPatientType.Enabled = false;
                    }
                }
                else if (HisConfig.IsSetPrimaryPatientType == "1"
                        && selectedService != null
                        && selectedService.BILL_PATIENT_TYPE_ID > 0)
                {
                    if (selectedService.BILL_PATIENT_TYPE_ID != selectedPatientTypeID
                        && CheckAPPLIED_PATIENT_TYPE_IDS(selectedService, selectedPatientTypeID)
                        && CheckAPPLIED_PATIENT_CLASSIFY_IDS(selectedService, currentTreatment.TDL_PATIENT_CLASSIFY_ID ?? -1))
                    {
                        cboPrimaryPatientType.EditValue = patientTypes != null && patientTypes.Exists(t => t.ID == selectedService.BILL_PATIENT_TYPE_ID) ? (long?)selectedService.BILL_PATIENT_TYPE_ID : null;
                        if (selectedService.IS_NOT_CHANGE_BILL_PATY == 1)
                            cboPrimaryPatientType.Enabled = false;
                        else
                            cboPrimaryPatientType.Enabled = true;
                    }
                    else
                    {
                        cboPrimaryPatientType.EditValue = null;
                        cboPrimaryPatientType.Enabled = true;
                    }
                }
                else if (sereServ != null)
                {
                    cboPrimaryPatientType.EditValue = patientTypes != null && patientTypes.Exists(t => t.ID == sereServ.PRIMARY_PATIENT_TYPE_ID) ? (long?)sereServ.PRIMARY_PATIENT_TYPE_ID : null;
                }
                else
                {
                    cboPrimaryPatientType.EditValue = null;
                }
                // Set cboPrimaryPatientType.EditValue = null khi Datasource không chứa giá trị hiện tại
                if (cboPrimaryPatientType.EditValue != null)
                {
                    long editValue = Inventec.Common.TypeConvert.Parse.ToInt64(cboPrimaryPatientType.EditValue.ToString());
                    if (patientTypes != null && patientTypes.Exists(o => o.ID == editValue))
                    {
                        //Do nothing!
                    }
                    else
                    {
                        cboPrimaryPatientType.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool CheckAPPLIED_PATIENT_TYPE_IDS(V_HIS_SERVICE service, long id)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrEmpty(service.APPLIED_PATIENT_TYPE_IDS))
                {
                    result = true;
                }
                else if (id > 0)
                {
                    string[] _str = service.APPLIED_PATIENT_TYPE_IDS.Split(',');
                    foreach (var item in _str)
                    {
                        if (id == Inventec.Common.TypeConvert.Parse.ToInt64(item))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private bool CheckAPPLIED_PATIENT_CLASSIFY_IDS(V_HIS_SERVICE service, long id)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrEmpty(service.APPLIED_PATIENT_CLASSIFY_IDS))
                {
                    result = true;
                }
                else if (id > 0)
                {
                    string[] _str = service.APPLIED_PATIENT_CLASSIFY_IDS.Split(',');
                    foreach (var item in _str)
                    {
                        if (id == Inventec.Common.TypeConvert.Parse.ToInt64(item))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public async Task LoadCboPatientType(long serviceId)
        {
            try
            {
                List<long> patientTypeIds = new List<long>();

                //List<V_HIS_PATIENT_TYPE_ALLOW> patientTypeAllows = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALLOW>();
                //if (patientTypeAllows == null || patientTypeAllows.Count == 0)
                //{
                //    Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach doi tuong chuyen doi");
                //    return;
                //}

                //var patientTypeIdRaws = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == currentPatientTypeAlter.PATIENT_TYPE_ID).Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList();
                ////Inventec.Common.Logging.LogSystem.Debug("LoadCboPatientType___0:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIds), patientTypeIds) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeAlter.PATIENT_TYPE_ID), currentPatientTypeAlter.PATIENT_TYPE_ID)
                ////  );
                ////if (patientTypeIds == null || patientTypeIds.Count == 0)
                ////{
                ////    Inventec.Common.Logging.LogSystem.Error("Khong tim thay doi tuong thanh toan chuyen doi. currentPatientTypeAlter.PATIENT_TYPE_ID : " + currentPatientTypeAlter.PATIENT_TYPE_ID);
                ////    return;
                ////}


                var servicePatys = currentServicePatys.Where(t => t.SERVICE_ID == serviceId).ToList();

                if (servicePatys == null || servicePatys.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong tim thay danh sach chinh sach gia dich vu");
                    cboPatientType.Properties.DataSource = null;
                    cboPatientType.EditValue = null;
                    return;
                }

                patientTypeIds = servicePatys.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList();

                //var servicePatyByService = servicePatys.Where(p => p.SERVICE_ID == serviceId).ToList();
                var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                var patientTypeIdPlusAfterFilter = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && patientTypeIds.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                if (patientTypeIdPlusAfterFilter != null && patientTypeIdPlusAfterFilter.Count > 0)
                {
                    patientTypeIds.AddRange(patientTypeIdPlusAfterFilter.Select(o => o.ID));
                }
                patientTypeIds = patientTypeIds.Where(k => currentPatientTypeIds.Contains(k)).Distinct().ToList();

   
                //patientTypeIds = patientTypeIds.Where(o =>
                //   servicePatyByService.Exists(k => k.PATIENT_TYPE_ID == o || BranchDataWorker.CheckPatientTypeInherit(k.INHERIT_PATIENT_TYPE_IDS, patientTypeIds))).ToList();

                //patientTypeIds = patientTypeIds.Where(o =>
                //    servicePatys.Where(p => p.SERVICE_ID == serviceId).Select(k => k.PATIENT_TYPE_ID).Contains(o)).ToList();//Old
                List<HIS_PATIENT_TYPE> patientTypes = patientTypeAll.Where(o => patientTypeIds.Contains(o.ID)).ToList();

                List<HIS_PATIENT_TYPE> patientTypesNew = new List<HIS_PATIENT_TYPE>();
                this.hisPatientType = new List<HIS_PATIENT_TYPE>();

                if (patientTypes != null && patientTypes.Count > 0)
                {
                    this.hisPatientType.AddRange(patientTypes);
                    patientTypesNew.AddRange(patientTypes);

                    if (chkKhongHuongBHYT.Checked)
                    {
                        var delete = hisPatientType.FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.UC.HisExamServiceAdd.Config.HisConfig.PatientTypeCode__BHYT);

                        var check = patientTypesNew.Where(o => o.ID == delete.ID).ToList();
                        if (check != null && check.Count > 0)
                        {
                            patientTypesNew.Remove(delete);
                        }
                    }
                }


                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPatientType, patientTypesNew, controlEditorADO);

                if (sereServ != null)
                {
                    cboPatientType.EditValue = patientTypesNew != null && patientTypesNew.Exists(t => t.ID == sereServ.PATIENT_TYPE_ID) ? (long?)sereServ.PATIENT_TYPE_ID : null;
                }
                else if (currentPatientTypeAlter != null)
                {
                    cboPatientType.EditValue = patientTypesNew != null && patientTypesNew.Exists(t => t.ID == currentPatientTypeAlter.PATIENT_TYPE_ID) ? (long?)currentPatientTypeAlter.PATIENT_TYPE_ID : null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitControlEnabled()
        {
            try
            {

                if (examServiceAddInitADO != null)
                {
                    memNote.Text = examServiceAddInitADO.Note;
                    if (!examServiceAddInitADO.IsMainExam)
                    {
                        chkIsPrimary.Enabled = false;
                    }
                    else
                    {
                        chkKetThucKhamHienTai.Enabled = false;
                    }
                    
                    if (examServiceAddInitADO.IsNotRequiredFee)
                        chkNotRequireFeeNonBHYT.Checked = true;
                    else
                        chkNotRequireFeeNonBHYT.Checked = false;

                    if (lciFinishTime.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    {
                        dtFinishTime.EditValue = null;
                        return;
                    }
                    if (examServiceAddInitADO.FinishTime.HasValue)
                    {
                        dtFinishTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(examServiceAddInitADO.FinishTime.Value) ?? DateTime.Now;
                    }
                    else
                    {
                        dtFinishTime.DateTime = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientType()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPrimaryPatientType()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
