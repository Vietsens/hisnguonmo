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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.RegisterV2.Run2;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterV2.Register
{
    class ServiceRequestRegisterExamBehavior : ServiceRequestRegisterBehaviorBase, IServiceRequestRegisterExam
    {
        HisServiceReqExamRegisterResultSDO result = null;
        List<ServiceReqDetailSDO> serviceReqDetailSDOs;

        internal ServiceRequestRegisterExamBehavior(CommonParam param, UCRegister ucServiceRequestRegiter, HisPatientSDO patientData)
            : base(param, ucServiceRequestRegiter)
        {
            this.registerNumber = ucServiceRequestRegiter.registerNumber;
            this.priority = (this.serviceReqInfoValue.IsPriority ? (GlobalVariables.HAS_PRIORITY) : 0);
            this.priorityNumber = this.serviceReqInfoValue.PriorityNumber;
            this.isNotRequireFee = (this.serviceReqInfoValue.IsNotRequireFee == true ? (short?)1 : null);
            this.serviceReqDetailSDOs = ucServiceRequestRegiter.serviceReqDetailSDOs;
        }

        HisServiceReqExamRegisterResultSDO IServiceRequestRegisterExam.Run()
        {
            HisServiceReqExamRegisterSDO serviceReqExamRegister = new HisServiceReqExamRegisterSDO();
            serviceReqExamRegister.HisPatientProfile = new HisPatientProfileSDO();

            //Process common data
            base.InitBase();

            //#3590
            if (!string.IsNullOrEmpty(patientProfile.HisPatientTypeAlter.LIVE_AREA_CODE))
            {
                Inventec.Common.Logging.LogSystem.Debug("Thong bao khu vuc");
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                ResourceMessage.BanCoMuonNhapThongTinKhuVuc,
                ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    // focus vào nơi sống.
                    this.ucRequestService.isShowMess = true;
                    return null;
                }
                Inventec.Desktop.Common.Message.WaitingManager.Show();
            }

            if (CheckMaMS())
            {
                this.ucRequestService.isShowMess = false;
                return null;
            }

            if (patientProfile.HisPatientTypeAlter != null
                && patientProfile.HisPatientTypeAlter.ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT
                && this.intructionTime > 0
                && patientProfile.HisPatientTypeAlter.HEIN_CARD_TO_TIME != null
                && this.treatmentTypeId != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
            {
                long _intructionDate = Inventec.Common.TypeConvert.Parse.ToInt64(this.intructionTime.ToString().Substring(0, 8) + "000000");
                if (_intructionDate > patientProfile.HisPatientTypeAlter.HEIN_CARD_TO_TIME)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Thong bao han the");
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                ResourceMessage.ThoiGianYLenhLonHonThoiGianHanDenCuaTheBHYT,
                ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao);
                    this.ucRequestService.isShowMess = true;
                    return null;
                }
            }

            serviceReqExamRegister.HisPatientProfile = patientProfile;
            serviceReqExamRegister.RequestRoomId = (this.currentModule != null ? this.currentModule.RoomId : 0);
            serviceReqExamRegister.Note = this.NOTE;
            //serviceReqExamRegister.IsAutoCreateBillForNonBhyt = chkAutoCreateBill;
            serviceReqExamRegister.IsAutoCreateDepositForNonBhyt = chkAutoDeposit || chkAutoCreateBill;
            serviceReqExamRegister.IsUsingEpayment = chkAutoPay;
            serviceReqExamRegister.IsExamOnline = chkExamOnline;
            if (chkAutoCreateBill)
            {
                if (GlobalVariables.DefaultPayformRequest.HasValue && GlobalVariables.DefaultPayformRequest.Value > 0)
                {
                    serviceReqExamRegister.PayFormId = GlobalVariables.DefaultPayformRequest.Value;
                }
                else
                {
                    serviceReqExamRegister.PayFormId = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TM;
                }

                if (GlobalVariables.AuthorityAccountBook != null && GlobalVariables.AuthorityAccountBook.AccountBookId.HasValue)
                {
                    serviceReqExamRegister.AccountBookId = GlobalVariables.AuthorityAccountBook.AccountBookId;
                    serviceReqExamRegister.CashierLoginName = GlobalVariables.AuthorityAccountBook.CashierLoginName;
                    serviceReqExamRegister.CashierUserName = GlobalVariables.AuthorityAccountBook.CashierUserName;
                    serviceReqExamRegister.CashierWorkingRoomId = GlobalVariables.AuthorityAccountBook.CashierWorkingRoomId;
                }
            }
            else if (chkAutoDeposit)
            {
                serviceReqExamRegister.CashierWorkingRoomId = this.cashierRoom_RoomId;
                if (GlobalVariables.SessionInfo != null)
                {
                    serviceReqExamRegister.AccountBookId = GlobalVariables.SessionInfo.DepositAccountBook.ID;
                    serviceReqExamRegister.CashierLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    serviceReqExamRegister.CashierUserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    if (GlobalVariables.SessionInfo.PayForm != null)
                    {
                        serviceReqExamRegister.PayFormId = GlobalVariables.SessionInfo.PayForm.ID;
                    }
                    if (GlobalVariables.SessionInfo.DepositAccountBook.IS_NOT_GEN_TRANSACTION_ORDER == (short)1)
                    {
                        serviceReqExamRegister.TransNumOrder = GlobalVariables.SessionInfo.NextDepositNumOrder;
                    }
                }
            }

            List<long> serviceIds = new List<long>();
            List<long> _RoomIds = new List<long>();
            //Process examserviceReq from input data
            this.ProcessExamServiceRequestData(ref serviceReqExamRegister, ref serviceIds, ref _RoomIds);
            ServiceReqDetailSDO appointmentExamService = null;
            if (serviceReqExamRegister.ServiceReqDetails != null && serviceReqExamRegister.ServiceReqDetails.Count > 0 && patientData != null)
            {
                //lấy ra y lệnh có phòng hoặc dịch vụ ứng với hẹn khám
                appointmentExamService = serviceReqExamRegister.ServiceReqDetails.FirstOrDefault(o => o.ServiceId == patientData.AppointmentExamServiceId || o.RoomId == (patientData.AppointmentExamRoomIds != null ? patientData.AppointmentExamRoomIds.First() : 0));
            }

            //đổi dịch vụ hẹn khám thì thông báo
            if (patientData != null && patientData.AppointmentTime.HasValue && patientData.AppointmentExamServiceId.HasValue &&
                patientData.AppointmentExamRoomIds != null && patientData.AppointmentExamRoomIds.Count > 0 &&
                patientData.NumOrderIssueId.HasValue && patientData.NextExamNumOrder.HasValue &&
                (appointmentExamService == null || (appointmentExamService.RoomId != patientData.AppointmentExamRoomIds.First())))
            {
                Inventec.Common.Logging.LogSystem.Debug("Thong bao doi cau hinh");
                var executeRooms = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>();
                var executeRoom = (executeRooms != null && executeRooms.Count > 0) ? executeRooms.Where(t => t.ROOM_ID == patientData.AppointmentExamRoomIds.First()).FirstOrDefault() : null;

                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                                   string.Format(ResourceMessage.DoiDichVuHenKhamSeDoiSttDaCap, Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientData.AppointmentTime.Value), (executeRoom != null ? executeRoom.EXECUTE_ROOM_NAME : "")),
                                   ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    this.ucRequestService.isShowMess = true;
                    return null;
                }
                else
                {
                    appointmentExamService.NumOrder = patientData.NextExamNumOrder;
                    appointmentExamService.NumOrderIssueId = patientData.NumOrderIssueId;
                }
                Inventec.Desktop.Common.Message.WaitingManager.Show();
            }

            // #9680
            if (_RoomIds != null && _RoomIds.Count > 0 && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsWarningOverExamBhyt
                && this.patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT
                && this.treatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
            {
                HisSereServBhytOutpatientExamFilter _HisSereServBhytOutpatientExamFilter = new HisSereServBhytOutpatientExamFilter();
                _HisSereServBhytOutpatientExamFilter.ROOM_IDs = _RoomIds;
                _HisSereServBhytOutpatientExamFilter.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(this.intructionTime.ToString().Substring(0, 8) + "000000");
                var dataSereServs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetSereServBhytOutpatientExam", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, _HisSereServBhytOutpatientExamFilter, null);
                if (dataSereServs != null && dataSereServs.Count > 0)
                {
                    MOS.Filter.HisExecuteRoomFilter executeRoomFilter = new HisExecuteRoomFilter();
                    executeRoomFilter.ROOM_IDs = _RoomIds;
                    var dataExecuteRooms = new BackendAdapter(new CommonParam()).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, executeRoomFilter, null);
                    if (dataExecuteRooms != null && dataExecuteRooms.Count > 0)
                    {
                        foreach (var itemRoom in dataExecuteRooms)
                        {
                            var coutSS = dataSereServs.Count(p => p.TDL_EXECUTE_ROOM_ID == itemRoom.ROOM_ID);
                            if (itemRoom.MAX_REQ_BHYT_BY_DAY.HasValue && coutSS >= itemRoom.MAX_REQ_BHYT_BY_DAY)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Thong bao vuot");
                                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                                string.Format(ResourceMessage.VuotQuaLuotKhamBHYTTrongNgay, itemRoom.EXECUTE_ROOM_NAME, itemRoom.MAX_REQ_BHYT_BY_DAY),
                                ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                {
                                    this.ucRequestService.isShowMess = true;
                                    return null;
                                }
                                Inventec.Desktop.Common.Message.WaitingManager.Show();
                            }
                        }
                    }
                }
            }

            //xuandv Neu Co dk Kham Qu tong dai
            if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsDangKyQuaTongDai == "1")
            {
                serviceReqExamRegister.IsNoExecute = true;
            }
            //#981
            List<HIS_SERE_SERV> sereServWithMinDurations = GetSereServWithMinDuration(this.patientId, serviceIds);
            if (sereServWithMinDurations != null && sereServWithMinDurations.Count > 0)
            {
                string sereServMinDurationStr = "";
                foreach (var item in sereServWithMinDurations)
                {
                    sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                }

                Inventec.Common.Logging.LogSystem.Debug("Thong bao dich vu");
                if (MessageBox.Show(string.Format(ResourceMessage.CanhBaoDichVuDaDuocChiDinhTrongKhoangThoiGianCauHinh, sereServMinDurationStr), ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    this.ucRequestService.isShowMess = true;
                    Inventec.Common.Logging.LogSystem.Warn("Cac dich vu sau co thoi gian chi dinh nam trong khoang thoi gian khong cho phep, ____" + sereServMinDurationStr);
                    return null;
                }
                else
                {
                    this.ucRequestService.isShowMess = false;
                }
            }

            if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckExamination)
            {
                if (serviceIds != null && serviceIds.Count > 0)
                {
                    //TODO
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Thong bao cong kham");
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                                ResourceMessage.BenhNhanChuaChonCongKham,
                                ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        this.ucRequestService.isShowMess = true;
                        return null;
                    }
                    Inventec.Desktop.Common.Message.WaitingManager.Show();
                }
            }
            
            //Execute call api
            result = (HisServiceReqExamRegisterResultSDO)base.RunBase(serviceReqExamRegister, this.ucRequestService);

            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
            if (result != null && (result.HisPatientProfile == null || result.ServiceReqs == null || result.ServiceReqs.Count == 0))
            {
                Inventec.Common.Logging.LogSystem.Warn("Goi api dang ky tiep don thanh cong, tuy nhien du lieu tra ve khong hop le, Dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqExamRegister), serviceReqExamRegister) + ", Dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
            }
            else if (result != null)
            {
                if (result.SereServs != null && result.SereServs.Count > 0)
                {
                    this.ucRequestService.serviceReqPrintIds = result.SereServs.Where(o => serviceIds.Contains(o.SERVICE_ID)).Select(o => o.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                }
                else
                    this.ucRequestService.serviceReqPrintIds = result.ServiceReqs.Select(o => o.ID).Distinct().ToList();
            }
            else
            {
                Inventec.Common.Logging.LogSystem.Warn("Goi api dang ky tiep don that bai, Dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqExamRegister), serviceReqExamRegister) + ", Dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
            }

            return result;
        }

        void ProcessExamServiceRequestData(ref HisServiceReqExamRegisterSDO ServiceReqData, ref List<long> serviceIds, ref List<long> _roomIds)
        {
            try
            {
                if (ServiceReqData.ServiceReqDetails == null)
                    ServiceReqData.ServiceReqDetails = new List<ServiceReqDetailSDO>();
                if (this.serviceReqDetailSDOs != null && this.serviceReqDetailSDOs.Count > 0)
                {
                    ServiceReqData.ServiceReqDetails.AddRange(this.serviceReqDetailSDOs);
                }
                foreach (var item in ServiceReqData.ServiceReqDetails)
                {
                    if (HisConfigCFG.IsSetPrimaryPatientType == "2" && (!item.PrimaryPatientTypeId.HasValue || item.PrimaryPatientTypeId <= 0))
                    {
                        if (item.PatientTypeId != this.patientProfile.HisPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID)
                        {
                            item.PrimaryPatientTypeId = this.patientProfile.HisPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID;
                        }
                    }
                    serviceIds.Add(item.ServiceId);
                    _roomIds.Add(item.RoomId ?? 0);
                }

                //this.ServiceAttachForServicePrimary(ServiceReqData, ref ServiceReqData);
                ServiceReqData.Priority = priority;
                if (this.priorityNumber.HasValue)
                {
                    ServiceReqData.NumOrder = this.priorityNumber;
                }
                ServiceReqData.InstructionTime = intructionTime;
                ServiceReqData.IsNotRequireFee = isNotRequireFee;
                ServiceReqData.PriorityTypeId = this.priorityTypeId;
                ServiceReqData.RequestLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                ServiceReqData.RequestUserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

                if (ServiceReqData.ServiceReqDetails != null && ServiceReqData.ServiceReqDetails.Count > 0)
                {
                    foreach (var item in ServiceReqData.ServiceReqDetails)
                    {
                        if (this.otherPaySourceId > 0)
                        {
                            item.OtherPaySourceId = this.otherPaySourceId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ServiceAttachForServicePrimary(HisServiceReqExamRegisterSDO serviceReqExamRegisterSDO, ref HisServiceReqExamRegisterSDO result)
        {
            try
            {
                List<long> serviceIds = serviceReqExamRegisterSDO.ServiceReqDetails.Where(o => o.ServiceId > 0).Select(o => o.ServiceId).ToList();

                var primaryPatientTypes = serviceReqExamRegisterSDO.ServiceReqDetails.Where(o => o.PrimaryPatientTypeId.HasValue);
                long? primaryPatientTypeId = (primaryPatientTypes != null && primaryPatientTypes.Count() > 0) ? primaryPatientTypes.Select(o => o.PrimaryPatientTypeId).FirstOrDefault() : null;

                List<HIS_SERVICE_FOLLOW> sfs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_FOLLOW>();

                List<HIS_SERVICE_FOLLOW> serviceFollows = sfs != null ? sfs.Where(o => serviceReqExamRegisterSDO != null && serviceReqExamRegisterSDO.ServiceReqDetails.Exists(t => t.ServiceId == o.SERVICE_ID)).ToList() : null;

                if (serviceFollows != null && serviceFollows.Count > 0)
                {
                    List<ServiceReqDetailSDO> serviceReqDetailsAdd = new List<ServiceReqDetailSDO>();
                    long defaultPatientTypeId = serviceReqExamRegisterSDO.HisPatientProfile.HisPatientTypeAlter.PATIENT_TYPE_ID;
                    foreach (ServiceReqDetailSDO sdo in serviceReqExamRegisterSDO.ServiceReqDetails)
                    {
                        List<HIS_SERVICE_FOLLOW> follows = serviceFollows.Where(t => t.SERVICE_ID == sdo.ServiceId).ToList();

                        if (follows != null && follows.Count > 0)
                        {
                            StringBuilder serviceFollow = new StringBuilder();
                            StringBuilder currentService = new StringBuilder();
                            foreach (HIS_SERVICE_FOLLOW f in follows)
                            {
                                V_HIS_SERVICE_PATY servicePaty = null;
                                if (BranchDataWorker.DicServicePatyInBranch != null && BranchDataWorker.DicServicePatyInBranch.ContainsKey(f.FOLLOW_ID))
                                {
                                    servicePaty = BranchDataWorker.ServicePatyWithPatientType(f.FOLLOW_ID, defaultPatientTypeId)
                                                .OrderByDescending(m => m.MODIFY_TIME).FirstOrDefault();
                                }

                                long? patientTypeId = null;
                                if (servicePaty != null)
                                {
                                    patientTypeId = defaultPatientTypeId;
                                    Inventec.Common.Logging.LogSystem.Debug("ServiceAttachForServicePrimary____"
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeId), patientTypeId));
                                }
                                else
                                {
                                    V_HIS_SERVICE_PATY otherServicePaty = BranchDataWorker.ServicePatyWithListPatientType(f.FOLLOW_ID, GlobalStore.PatientTypeIdAllows).OrderByDescending(m => m.MODIFY_TIME).FirstOrDefault();

                                    var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();

                                    patientTypeId = otherServicePaty != null ? (long?)otherServicePaty.PATIENT_TYPE_ID : null;

                                    var patientTypeIdPlus = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && GlobalStore.PatientTypeIdAllows.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                                    if (patientTypeIdPlus != null && patientTypeIdPlus.Count > 0 && (otherServicePaty != null && !String.IsNullOrEmpty(otherServicePaty.INHERIT_PATIENT_TYPE_IDS) && patientTypeIdPlus.Exists(k => k.ID != patientTypeId)))
                                    {
                                        patientTypeId = patientTypeIdPlus.First().ID;
                                    }
                                    Inventec.Common.Logging.LogSystem.Debug("ServiceAttachForServicePrimary____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => otherServicePaty), otherServicePaty)
                                         + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdPlus), patientTypeIdPlus)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeId), patientTypeId));
                                    //patientTypeId = otherServicePaty != null ? new Nullable<long>(otherServicePaty.PATIENT_TYPE_ID) : null;
                                }

                                if (patientTypeId.HasValue)
                                {
                                    ServiceReqDetailSDO attach = new ServiceReqDetailSDO();
                                    attach.ServiceId = f.FOLLOW_ID;
                                    attach.Amount = f.AMOUNT;
                                    attach.IsExpend = f.IS_EXPEND;
                                    attach.PatientTypeId = patientTypeId.Value;
                                    if (HisConfigCFG.IsSetPrimaryPatientType == "2" && (!primaryPatientTypeId.HasValue || primaryPatientTypeId <= 0))
                                    {
                                        if (attach.PatientTypeId != this.patientProfile.HisPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID)
                                        {
                                            attach.PrimaryPatientTypeId = this.patientProfile.HisPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID;
                                        }
                                    }
                                    serviceReqDetailsAdd.Add(attach);
                                }
                                else
                                {
                                    var lstService = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();
                                    serviceFollow.Append(lstService.SingleOrDefault(o => o.ID == f.SERVICE_ID).SERVICE_NAME).Append(",");
                                    currentService.Append(lstService.SingleOrDefault(o => o.ID == sdo.ServiceId).SERVICE_NAME).Append(",");
                                }
                            }

                            if (!String.IsNullOrEmpty(serviceFollow.ToString()) || !String.IsNullOrEmpty(currentService.ToString()))
                            {
                                MessageManager.Show(string.Format(ResourceMessage.DichVuDinhKemDichVuChuaCoChinhSachGia, serviceFollow.ToString(), currentService.ToString()));
                            }
                        }
                    }

                    if (serviceReqDetailsAdd != null && serviceReqDetailsAdd.Count > 0)
                    {
                        serviceReqExamRegisterSDO.ServiceReqDetails.AddRange(serviceReqDetailsAdd);
                    }
                    result = serviceReqExamRegisterSDO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_SERE_SERV> GetSereServWithMinDuration(long patientId, List<long> serviceIds)
        {
            List<HIS_SERE_SERV> results = new List<HIS_SERE_SERV>();
            try
            {
                if (serviceIds == null || serviceIds.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong truyen danh sach serviceids");
                    return null;
                }
                List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>()
     .Where(o => serviceIds.Contains(o.ID) && o.MIN_DURATION.HasValue).ToList();

                if (services != null && services.Count > 0)
                {

                    List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                    foreach (var item in services)
                    {
                        ServiceDuration sd = new ServiceDuration();
                        sd.MinDuration = item.MIN_DURATION.Value;
                        sd.ServiceId = item.ID;
                        serviceDurations.Add(sd);
                    }
                    CommonParam param = new CommonParam();
                    HisSereServMinDurationFilter hisSereServMinDurationFilter = new HisSereServMinDurationFilter();
                    hisSereServMinDurationFilter.ServiceDurations = serviceDurations;
                    hisSereServMinDurationFilter.PatientId = patientId;
                    hisSereServMinDurationFilter.InstructionTime = this.intructionTime;
                    results = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, hisSereServMinDurationFilter, param); if (results != null && results.Count > 0)
                    {

                        var listSereServResultTemp = from SereServResult in results
                                                     group SereServResult by SereServResult.SERVICE_ID into g
                                                     orderby g.Key
                                                     select g.FirstOrDefault();
                        results = listSereServResultTemp.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                results = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return results;
        }

    }
}
