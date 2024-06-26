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
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.UpdateExamServiceReq.Resources;
using HIS.Desktop.Plugins.UpdateExamServiceReq.Base;
using HIS.Desktop.Plugins.UpdateExamServiceReq.Config;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.UpdateExamServiceReq
{

    public partial class frmUpdateExamServiceReq : HIS.Desktop.Utility.FormBase
    {
        List<V_HIS_SERVICE> ListHisSerVice_ = new List<V_HIS_SERVICE>();
        public void LoadDataToControl()
        {
            try
            {
                //Load Combo
                LoadComboExamRoom();
                //Load patientType

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private V_HIS_PATIENT_TYPE_ALTER GetCurrentPatientTypeAlter(long treatmentId)
        {
            V_HIS_PATIENT_TYPE_ALTER result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = treatmentId;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;

                Inventec.Common.Logging.LogSystem.Debug("HIS_PATIENT_TYPE_ALTER_GET_APPLIED____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                result = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadComboExamRoom()
        {
            try
            {
                List<V_HIS_EXECUTE_ROOM> roomExams = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.IS_EXAM == 1 && o.IS_ACTIVE == 1 && o.BRANCH_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId() && (o.IS_PAUSE_ENCLITIC == null || o.IS_PAUSE_ENCLITIC != 1)).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ROOM_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboRoom, roomExams, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboServiceRoom(long roomId)
        {
            try
            {

                cboExamServiceReq.EditValue = null;
                txtServiceCode.Text = "";
                List<long> serviceIdActives = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == 1).Select(p => p.ID).ToList();
                serviceRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_ID == roomId && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH && serviceIdActives.Contains(o.SERVICE_ID)).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboExamServiceReq, serviceRooms, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientType(long serviceId)
        {
            try
            {
                List<V_HIS_SERVICE_PATY> servicePatys = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_PATY>();
                if (servicePatys == null || servicePatys.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => servicePatys), servicePatys));
                    return;
                }

                long branchId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId();
                if (branchId <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => branchId), branchId));
                    return;
                }
                if (currentPatientTypeAlter == null)
                {
                    return;
                }
                List<HIS_PATIENT_TYPE_ALLOW> patientTypeAllows = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE_ALLOW>(false, true)
                    .Where(o => o.PATIENT_TYPE_ID == currentPatientTypeAlter.PATIENT_TYPE_ID && o.IS_ACTIVE == 1).ToList();

                List<long> patientTypeAllowIds = patientTypeAllows != null ? patientTypeAllows.Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList() : new List<long>();

                var servicePatyInBranchs = servicePatys.Where(o => o.BRANCH_ID == branchId).ToList();
                if (servicePatyInBranchs != null && servicePatyInBranchs.Count > 0)
                {
                    var arrPatientTypeIds = servicePatyInBranchs.Where(o => o.SERVICE_ID == serviceId).Select(o => o.PATIENT_TYPE_ID).ToList();
                    if (arrPatientTypeIds != null && arrPatientTypeIds.Count > 0)
                    {
                        var allowPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                                                                        && (arrPatientTypeIds.Contains(o.ID) || o.ID == treatment.TDL_PATIENT_TYPE_ID)).ToList();

                        if (allowPatientTypes != null && allowPatientTypes.Count > 0)
                        {
                            this.PatientTypes = new List<HIS_PATIENT_TYPE>();

                            V_HIS_SERE_SERV_6 data = new V_HIS_SERE_SERV_6();
                            if (sereServExams != null && sereServExams.Count > 0)
                            {
                                data = sereServExams.FirstOrDefault();
                            }

                            foreach (var item in allowPatientTypes)
                            {
                                if (!patientTypeAllowIds.Contains(item.ID))
                                    continue;

                                V_HIS_SERVICE_PATY appliedServicePaty = null;

                                var intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.DateTime);
                                appliedServicePaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePatyInBranchs, branchId, data.TDL_EXECUTE_ROOM_ID, data.TDL_REQUEST_ROOM_ID, data.TDL_REQUEST_DEPARTMENT_ID, (long)intructionTime, this.treatment.IN_TIME, data.SERVICE_ID, item.ID, null, null, null, null);
                                if (data.PACKAGE_ID.HasValue && appliedServicePaty == null)
                                {
                                    appliedServicePaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePatyInBranchs, branchId, data.TDL_EXECUTE_ROOM_ID, data.TDL_REQUEST_ROOM_ID, data.TDL_REQUEST_DEPARTMENT_ID, (long)intructionTime, this.treatment.IN_TIME, data.SERVICE_ID, item.ID, null, null, data.PACKAGE_ID, null);
                                }

                                if (appliedServicePaty != null)
                                {
                                    this.PatientTypes.Add(item);
                                }
                            }
                        }

                        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                        columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                        ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                        ControlEditorLoader.Load(cboPatientType, PatientTypes, controlEditorADO);

                        HIS_PATIENT_TYPE patientType = null;
                        if (cboPatientType.EditValue != null)
                        {
                            long patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                            if (PatientTypes != null && PatientTypes.Count == 0)
                            {
                                patientType = PatientTypes.FirstOrDefault(o => o.ID == patientTypeId);
                            }
                        }
                        if (patientType == null)
                        {
                            txtPatientTypeCode.Text = "";
                            cboPatientType.EditValue = null;
                        }
                    }
                }
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
                if (cboExamServiceReq.EditValue != null)
                {
                    long serviceId = Inventec.Common.TypeConvert.Parse.ToInt64((cboExamServiceReq.EditValue ?? 0).ToString());
                    LoadPrimaryPatientType(serviceId);

                }
                else
                {
                    txtPrimaryPatientTypeCode.Text = "";
                    cboPrimaryPatientType.EditValue = null;
                    cboPrimaryPatientType.Properties.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        long oldPrimaryPatientType = 0;
        private void LoadPrimaryPatientType(long serviceId)
        {
            try
            {
                List<V_HIS_PATIENT_TYPE_ALLOW> patientTypeAllows = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALLOW>();

                List<long> patientTypeIds = patientTypeAllows != null ? patientTypeAllows.Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList() : null;
                if (patientTypeIds == null)
                    patientTypeIds = new List<long>();

                List<V_HIS_SERVICE_PATY> servicePatys = BranchDataWorker.ServicePatyWithListPatientType(serviceId, patientTypeIds);

                patientTypeIds = servicePatys != null ? servicePatys.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList() : new List<long>();

                var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var patientTypeIdPlusAfterFilter = patientTypeAll.Where(o => o.BASE_PATIENT_TYPE_ID != null && patientTypeIds.Contains(o.BASE_PATIENT_TYPE_ID.Value)).ToList();
                if (patientTypeIdPlusAfterFilter != null && patientTypeIdPlusAfterFilter.Count > 0)
                {
                    patientTypeIds.AddRange(patientTypeIdPlusAfterFilter.Select(o => o.ID));
                }
                patientTypeIds = patientTypeIds.Distinct().ToList();
                long currentPatientTypeID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString());
                this.PatientTypes_cboPrimaryPatientTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                                                            .Where(o => o.IS_ADDITION == (short)1
                                                                && patientTypeIds.Contains(o.ID)
                                                                && o.ID != currentPatientTypeID).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPrimaryPatientType, this.PatientTypes_cboPrimaryPatientTypes, controlEditorADO);

                // Set cboPrimaryPatientType.EditValue = null khi Datasource không chứa giá trị hiện tại

                Inventec.Common.Logging.LogSystem.Debug("cboPrimaryPatientType.EditValue____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cboPrimaryPatientType.EditValue), cboPrimaryPatientType.EditValue));
                if (cboPrimaryPatientType.EditValue != null)
                {
                    long editValue = Inventec.Common.TypeConvert.Parse.ToInt64(cboPrimaryPatientType.EditValue.ToString());
                    if (editValue != null && editValue > 0)
                    {
                        this.oldPrimaryPatientType = editValue;
                    }
                    if (this.PatientTypes_cboPrimaryPatientTypes != null && this.PatientTypes_cboPrimaryPatientTypes.Exists(o => o.ID == editValue))
                    {
                        //Do nothing!
                    }
                    else
                    {

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => editValue), editValue));
                        cboPrimaryPatientType.EditValue = null;
                    }

                }
                else
                {
                    if (this.PatientTypes_cboPrimaryPatientTypes != null && this.PatientTypes_cboPrimaryPatientTypes.Exists(o => o.ID == this.oldPrimaryPatientType))
                    {
                        cboPrimaryPatientType.EditValue = this.oldPrimaryPatientType;
                    }
                }
                if (HisConfigCFG.IS_SET_PRIMARY_PATIENT_TYPE == "2" && check_)
                {
                    long? oldPrimaryPatientType = this.PatientTypes_cboPrimaryPatientTypes.Exists(t => t.ID == currentPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID) ? (long?)currentPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID : null;
                    this.oldPrimaryPatientType = oldPrimaryPatientType ?? 0;
                }
                if (cboExamServiceReq.EditValue != null && HisConfigCFG.IS_SET_PRIMARY_PATIENT_TYPE == "1")
                {
                    long? oldPrimaryPatientType = this.PatientTypes_cboPrimaryPatientTypes.Exists(t => t.ID == currentPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID) ? (long?)currentPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID : null;
                    this.oldPrimaryPatientType = oldPrimaryPatientType ?? 0;
                    long serviceid = long.Parse(cboExamServiceReq.EditValue.ToString());
                    var data = ListHisSerVice_.FirstOrDefault(o => o.ID == serviceid);
                    if (data != null)
                    {
                        if (data.BILL_PATIENT_TYPE_ID != null
                            && data.BILL_PATIENT_TYPE_ID != long.Parse(cboPatientType.EditValue.ToString())
                            && PatientTypes_cboPrimaryPatientTypes.Exists(o => o.ID == data.BILL_PATIENT_TYPE_ID)
                            && LoadAppliedPatientType(long.Parse(cboPatientType.EditValue.ToString()), serviceid))
                        {
                            cboPrimaryPatientType.EditValue = data.BILL_PATIENT_TYPE_ID;
                        }
                        else
                        {
                            cboPrimaryPatientType.EditValue = null;
                        }

                    }
                    else
                    {
                        if (this.oldPrimaryPatientType != null && this.PatientTypes_cboPrimaryPatientTypes != null && this.PatientTypes_cboPrimaryPatientTypes.Exists(o => o.ID == this.oldPrimaryPatientType))
                        {
                            cboPrimaryPatientType.EditValue = this.oldPrimaryPatientType > 0 ? (long?)this.oldPrimaryPatientType : null;
                        }
                    }
                    //if (cboExamServiceReq.EditValue != null)
                    //{
                    if (data != null)
                    {
                        if (data.IS_NOT_CHANGE_BILL_PATY == 1)
                        {
                            cboPrimaryPatientType.Enabled = false;
                            txtPrimaryPatientTypeCode.Enabled = false;
                        }
                        else
                        {
                            cboPrimaryPatientType.Enabled = true;
                            txtPrimaryPatientTypeCode.Enabled = true;
                        }
                        // }
                    }
                }
                else if (this.oldPrimaryPatientType != null && this.PatientTypes_cboPrimaryPatientTypes != null && this.PatientTypes_cboPrimaryPatientTypes.Exists(o => o.ID == this.oldPrimaryPatientType))
                {
                    cboPrimaryPatientType.EditValue = this.oldPrimaryPatientType > 0 ? (long?)this.oldPrimaryPatientType : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool LoadAppliedPatientType(long patientTypeId, long serviceId)
        {
            bool success = false;
            try
            {
                if (serviceId > 0 && patientTypeId > 0)
                {
                    var checkService = ListHisSerVice_.Find(o => o.ID == serviceId);
                    if (checkService != null && (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_TYPE_IDS) || IsContainString(checkService.APPLIED_PATIENT_TYPE_IDS, patientTypeId.ToString()))
                        && (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_CLASSIFY_IDS) || IsContainString(checkService.APPLIED_PATIENT_CLASSIFY_IDS, this.treatment.TDL_PATIENT_CLASSIFY_ID != null ? this.treatment.TDL_PATIENT_CLASSIFY_ID.ToString() : "-1")))
                    {
                        success = true;
                    }


                }
                return success;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }
        private bool IsContainString(string arrStr, string str)
        {
            bool result = false;
            try
            {
                if (arrStr.Contains(","))
                {
                    var arr = arrStr.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        result = arr[i] == str;
                        if (result) break;
                    }
                }
                else
                {
                    result = arrStr == str;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void FillDataToConTrol()
        {
            try
            {
                string code = txtServiceReqCode.Text.Trim();
                if (code.Length < 12 && checkDigit(code))
                {
                    code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    txtServiceReqCode.Text = code;
                }

                if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                {
                    WaitingManager.Show();
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.SERVICE_REQ_CODE__EXACT = code;
                    var apiResult = new BackendAdapter(new CommonParam())
                        .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, new CommonParam());
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        serviceReq = apiResult.FirstOrDefault();
                    }
                    WaitingManager.Hide();
                    LoadDataFromServiceReq(serviceReq, true);

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataFromServiceReq(HIS_SERVICE_REQ serviceReq, bool check)
        {
            try
            {
                if (serviceReq != null)
                {
                    List<Action> methods = new List<Action>();
                    methods.Add(LoadTreatment);
                    methods.Add(LoadSereServBill);
                    methods.Add(LoadSereServ);
                    methods.Add(LoadSereServDeposit);
                    Inventec.Common.ThreadCustom.ThreadCustomManager.MultipleThreadWithJoin(methods);

                    //if ((!HisConfigCFG.IS_ALLOW_EDIT_EXAM_HAS_BILL) && this.CheckServiceIsPay())
                    //{
                    //    MessageBox.Show(ResourceMessage.DichVuDaThanhToan);
                    //    ResetDataToDefault(false);
                    //    if (this.ModuleExeType == Base.ModuleExecuteType.TYPE.FROM_EXECUTE_ROOM
                    //        || this.ModuleExeType == Base.ModuleExecuteType.TYPE.FROM_MODULE_OTHER)
                    //        this.Close();
                    //    return;
                    //}

                    InitEnableControl();
                    txtServiceReqCode.Text = serviceReq.SERVICE_REQ_CODE;
                    var stt = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>().FirstOrDefault(o => o.ID == serviceReq.SERVICE_REQ_STT_ID);
                    lblStatus.Text = stt != null ? stt.SERVICE_REQ_STT_NAME : "";
                    lblPatientName.Text = serviceReq.TDL_PATIENT_NAME;
                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.TDL_PATIENT_DOB);
                    cboRoom.EditValue = serviceReq.EXECUTE_ROOM_ID;
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.EXECUTE_ROOM_ID);
                    txtRoomCode.Text = room != null ? room.ROOM_CODE : "";
                    chkPrioritize.CheckState = serviceReq.PRIORITY == 1 ? CheckState.Checked : CheckState.Unchecked;

                    chkThuSau.CheckState = serviceReq.IS_NOT_REQUIRE_FEE == 1 ? CheckState.Checked : CheckState.Unchecked;
                    dtInstructionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME) ?? DateTime.Now;

                    LoadComboServiceRoom(serviceReq.EXECUTE_ROOM_ID);
                    txtRoomCode.Focus();
                    txtRoomCode.SelectAll();

                    //Doi tuong thanh toan hien tai
                    currentPatientTypeAlter = GetCurrentPatientTypeAlter(serviceReq.TREATMENT_ID);

                    if (sereServExams != null && sereServExams.Count > 0)
                    {
                        cboExamServiceReq.EditValue = sereServExams[0].SERVICE_ID;
                        txtServiceCode.Text = sereServExams[0].TDL_SERVICE_CODE;
                        LoadPatientType(sereServExams[0].SERVICE_ID);
                        HIS_PATIENT_TYPE patientType = PatientTypes.FirstOrDefault(o => o.ID == sereServExams[0].PATIENT_TYPE_ID);
                        if (patientType != null)
                        {
                            txtPatientTypeCode.Text = patientType.PATIENT_TYPE_CODE;
                            cboPatientType.EditValue = patientType.ID;
                        }
                    }
                    #region Nếu Key 'MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE' =2 thì: Lấy Phụ thu theo ĐT bệnh nhân và không cho sửa lại

                    if (HisConfigCFG.IS_SET_PRIMARY_PATIENT_TYPE == "2")
                    {
                        cboPrimaryPatientType.Enabled = false;
                        txtPrimaryPatientTypeCode.Enabled = false;
                        this._isAllowEnableCboPrimaryPatientTypes = false;
                    }

                    if (HisConfigCFG.IS_SET_PRIMARY_PATIENT_TYPE == "2" && check)
                    {
                        if (currentPatientTypeAlter != null && this.PatientTypes_cboPrimaryPatientTypes != null)
                        {
                            long? primaryPatientTypeID = this.PatientTypes_cboPrimaryPatientTypes.Exists(t => t.ID == currentPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID) ? (long?)currentPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID : null;
                            cboPrimaryPatientType.EditValue = primaryPatientTypeID;
                            cboPrimaryPatientType.Enabled = false;
                            txtPrimaryPatientTypeCode.Enabled = false;
                            this._isAllowEnableCboPrimaryPatientTypes = false;
                        }
                        else
                        {
                            cboPrimaryPatientType.EditValue = null;
                        }
                    }
                    else if (sereServExams != null && sereServExams.Count > 0)
                    {
                        cboPrimaryPatientType.EditValue = sereServExams[0].PRIMARY_PATIENT_TYPE_ID;
                    }
                    else
                    {
                        cboPrimaryPatientType.EditValue = null;
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = serviceReq.TREATMENT_ID;
                treatment = new BackendAdapter(new CommonParam())
            .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServBill()
        {
            try
            {
                //Load sereServ bill
                HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
                sereServBillFilter.TDL_TREATMENT_ID = serviceReq.TREATMENT_ID;
                SereServBills = new BackendAdapter(new CommonParam())
                .Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, sereServBillFilter, new CommonParam()).Where(o => o.IS_CANCEL != 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadSereServDeposit()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServDepositFilter filter = new HisSereServDepositFilter();
                filter.TDL_TREATMENT_ID = serviceReq.TREATMENT_ID; ;
                filter.IS_CANCEL = false;
                SereServDeposits = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServ()
        {
            try
            {
                MOS.Filter.HisSereServView6Filter sereServFilter = new MOS.Filter.HisSereServView6Filter();
                sereServFilter.SERVICE_REQ_ID = serviceReq.ID;
                sereServExams = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_6>>("api/HisSereServ/GetView6", ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, null);
                if (sereServExams != null && sereServExams.Count > 0)
                {
                    sereServExams = sereServExams.Where(o => o.IS_NO_EXECUTE != 1).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckServiceIsPay()
        {
            bool result = false;
            try
            {
                if (SereServBills != null && SereServBills.Count > 0 && sereServExams != null && sereServExams.Count == 1)
                {
                    HIS_SERE_SERV_BILL sereServBill = SereServBills.FirstOrDefault(o => o.SERE_SERV_ID == sereServExams[0].ID);
                    if (sereServBill != null)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckServiceIsDeposit()
        {
            bool result = false;
            try
            {
                if (SereServDeposits != null && SereServDeposits.Count > 0 && sereServExams != null && sereServExams.Count == 1)
                {
                    HIS_SERE_SERV_DEPOSIT sereServDeposit = SereServDeposits.FirstOrDefault(o => o.SERE_SERV_ID == sereServExams[0].ID);
                    if (sereServDeposit != null)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private HisServiceReqExamChangeSDO SetDataToExamServiceReqChangeSDO()
        {
            HisServiceReqExamChangeSDO examServiceReqChange = new HisServiceReqExamChangeSDO();
            try
            {
                if (serviceReq != null)
                {
                    examServiceReqChange.CurrentServiceReqId = serviceReq.ID;
                }

                if (dtInstructionTime.EditValue != null)
                    examServiceReqChange.InstructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.DateTime) ?? 0;
                examServiceReqChange.Priority = chkPrioritize.Checked ? true : false;
                examServiceReqChange.RequestRoomId = roomId;
                examServiceReqChange.RoomId = Inventec.Common.TypeConvert.Parse.ToInt64(cboRoom.EditValue.ToString()); ;
                examServiceReqChange.ServiceId = Inventec.Common.TypeConvert.Parse.ToInt64(cboExamServiceReq.EditValue.ToString());
                examServiceReqChange.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                examServiceReqChange.PrimaryPatientTypeId = cboPrimaryPatientType.EditValue != null ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(cboPrimaryPatientType.EditValue.ToString()) : null;
                examServiceReqChange.IsNotRequireFee = chkThuSau.Checked;
                examServiceReqChange.IsCopyOldInfo = chkCopyExamOldContent.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return examServiceReqChange;
        }

        internal void LoadExamRoom(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboRoom.Focus();
                    cboRoom.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.EXECUTE_ROOM_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboRoom.EditValue = data[0].ROOM_ID;
                            LoadComboServiceRoom(data[0].ROOM_ID);
                            txtServiceCode.Focus();
                            txtServiceCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EXECUTE_ROOM_CODE == searchCode);
                            if (search != null)
                            {
                                cboRoom.EditValue = search.ROOM_ID;
                                LoadComboServiceRoom(search.ROOM_ID);
                                txtServiceCode.Focus();
                                txtServiceCode.SelectAll();
                            }
                            else
                            {
                                cboRoom.EditValue = null;
                                cboRoom.Focus();
                                cboRoom.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboRoom.EditValue = null;
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

        internal void LoadService(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboExamServiceReq.Focus();
                    cboExamServiceReq.ShowPopup();
                }
                else
                {
                    var data = serviceRooms.Where(o => o.SERVICE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboExamServiceReq.EditValue = data[0].SERVICE_ID;
                            LoadPatientType(data[0].SERVICE_ID);
                            txtPatientTypeCode.Focus();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.SERVICE_CODE == searchCode);
                            if (search != null)
                            {
                                cboExamServiceReq.EditValue = search.SERVICE_ID;
                                LoadPatientType(search.SERVICE_ID);
                                txtPatientTypeCode.Focus();
                            }
                            else
                            {
                                cboExamServiceReq.EditValue = null;
                                cboExamServiceReq.Focus();
                                cboExamServiceReq.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboExamServiceReq.EditValue = null;
                        cboExamServiceReq.Focus();
                        cboExamServiceReq.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SearchPatientType(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPatientType.Focus();
                    cboPatientType.ShowPopup();
                }
                else
                {
                    var data = PatientTypes.Where(o => o.PATIENT_TYPE_CODE != null
                                                    && o.PATIENT_TYPE_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPatientType.EditValue = data[0].ID;
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PATIENT_TYPE_CODE == searchCode);
                            if (search != null)
                            {
                                cboPatientType.EditValue = search.ID;
                            }
                            else
                            {
                                cboPatientType.Focus();
                                cboPatientType.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboPatientType.Focus();
                        cboPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SearchPrimaryPatientType(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPrimaryPatientType.Focus();
                    cboPrimaryPatientType.ShowPopup();
                }
                else
                {
                    var data = this.PatientTypes_cboPrimaryPatientTypes.Where(o => o.PATIENT_TYPE_CODE != null
                                                                        && o.PATIENT_TYPE_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPrimaryPatientType.EditValue = data[0].ID;
                            txtPrimaryPatientTypeCode.Text = data[0].PATIENT_TYPE_CODE;
                            dtInstructionTime.Focus();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PATIENT_TYPE_CODE == searchCode);
                            if (search != null)
                            {
                                cboPrimaryPatientType.EditValue = search.ID;
                                txtPrimaryPatientTypeCode.Text = search.PATIENT_TYPE_CODE;
                                dtInstructionTime.Focus();
                            }
                            else
                            {
                                cboPrimaryPatientType.Focus();
                                cboPrimaryPatientType.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboPrimaryPatientType.Focus();
                        cboPrimaryPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ResetDataToDefault(bool? serviceReqReset = true)
        {
            try
            {
                if (serviceReqReset == true)
                    txtServiceReqCode.Text = "";
                txtRoomCode.Text = "";
                txtServiceCode.Text = "";
                cboRoom.EditValue = null;
                cboExamServiceReq.EditValue = null;
                sereServExams = null;
                serviceRooms = null;
                lblStatus.Text = "";
                lblPatientName.Text = "";
                lblDob.Text = "";
                txtPatientTypeCode.Text = "";
                cboPatientType.EditValue = null;
                chkPrioritize.CheckState = CheckState.Unchecked;
                chkThuSau.CheckState = CheckState.Unchecked;
                serviceReq = null;
                SereServBills = null;
                dtInstructionTime.EditValue = null;
                InitEnableControl();
                btnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitEnableControl()
        {
            try
            {
                if (serviceReq != null
                  && (serviceReq.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                  || serviceReq.REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                  || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                  || serviceReq.REQUEST_ROOM_ID == roomId
                  || serviceReq.EXECUTE_ROOM_ID == roomId)
                  && (serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                    || HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "1"
                    || HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "2"
                    && serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                    && serviceReq.IS_NO_EXECUTE != 1
                    )
                {
                    txtRoomCode.Enabled = true;
                    cboRoom.Enabled = true;
                    txtServiceCode.Enabled = true;
                    cboExamServiceReq.Enabled = true;
                    cboPatientType.Enabled = true;
                    if (this._isAllowEnableCboPrimaryPatientTypes)
                    {
                        cboPrimaryPatientType.Enabled = true;
                        txtPrimaryPatientTypeCode.Enabled = true;
                    }
                    dtInstructionTime.Enabled = true;
                    txtPatientTypeCode.Enabled = true;
                    chkPrioritize.Enabled = true;
                    btnSave.Enabled = true;

                }
                else
                {
                    txtRoomCode.Enabled = false;
                    cboRoom.Enabled = false;
                    txtServiceCode.Enabled = false;
                    cboExamServiceReq.Enabled = false;
                    dtInstructionTime.Enabled = false;
                    chkPrioritize.Enabled = false;
                    txtPatientTypeCode.Enabled = false;
                    cboPatientType.Enabled = false;
                    cboPrimaryPatientType.Enabled = false;
                    txtPrimaryPatientTypeCode.Enabled = false;
                    btnSave.Enabled = false;
                }

                if (this.ModuleExeType == ModuleExecuteType.TYPE.FROM_EXECUTE_ROOM)
                {
                    txtRoomCode.Enabled = false;
                    cboRoom.Enabled = false;
                    txtServiceReqCode.Enabled = false;
                    btnFind.Enabled = false;
                    btnRefesh.Enabled = false;
                }
                else if (this.ModuleExeType == ModuleExecuteType.TYPE.FROM_MODULE_OTHER)
                {
                    txtServiceReqCode.Enabled = false;
                    btnFind.Enabled = false;
                    btnRefesh.Enabled = false;
                }
                if (sereServExams == null || sereServExams.Count == 0)
                {                 
                    btnSave.ToolTip = "Không cho phép sửa yêu cầu dịch vụ khám đối với các chỉ định khám không có chứa công khám";
                    btnSave.ForeColor = Color.DarkGray;
                    isSave = false;
                    dtInstructionTime.Enabled = false;
                    txtRoomCode.Enabled = false;
                    cboRoom.Enabled = false;
                    txtServiceCode.Enabled = false;
                    cboExamServiceReq.Enabled = false;
                    txtPatientTypeCode.Enabled = false;
                    cboPatientType.Enabled = false;
                }
                else
                {
                    btnSave.ToolTip = "";
                    btnSave.ForeColor = new Color();
                    isSave = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }
    }
}
