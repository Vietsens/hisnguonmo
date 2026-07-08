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
using HIS.Desktop.LocalStorage.ConfigApplication;
//using HIS.Desktop.Plugins.Library.PrintBordereau.BankQrCode;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000504.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.MpsBehavior.Mps000504
{
    class Mps000504Behavior : MpsDataBase, ILoad
    {
        public Mps000504Behavior(long? roomId, V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter, List<HIS_SERE_SERV> _sereServs,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment,
            V_HIS_PATIENT _patient, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _roomName, string _userNameReturnResult,
            List<HIS_SERE_SERV_BILL> listSsBill, HIS_TRANS_REQ _transReq, List<HIS_CONFIG> _lstConfig, bool IsActionButtonPrintBill, long fromDateReq, long toDateReq)
            : base(roomId, _treatment)
        {
            this.SereServs = _sereServs;
            this.DepartmentTrans = _departmentTrans;
            this.TreatmentFees = _treamentFees;
            this.Treatment = _treatment;
            this.Rooms = _rooms;
            this.Services = _services;
            this.HeinServiceTypes = _heinServiceTypes;
            this.TotalDayTreatment = _totalDayTreatment;
            this.StatusTreatmentOut = _statusTreatmentOut;
            this.DepartmentName = _departmentName;
            this.UserNameReturnResult = _userNameReturnResult;
            this.CurrentPatientTypeAlter = currentPatientTypeAlter;
            this.RoomName = _roomName;
            this.Patient = _patient;
            this.SereServBills = listSsBill;
            this.transReq2 = _transReq;
            this.lstConfig = _lstConfig;
            this.IsActionButtonPrintBill = IsActionButtonPrintBill;
            this.fromDateReq = fromDateReq;
            this.toDateReq = toDateReq;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            bool result = false;
            try
            {
                V_HIS_TREATMENT_FEE treatment = this.TreatmentFees.FirstOrDefault(o => o.ID == this.Treatment.ID);

                CommonParam param = new CommonParam();
                List<V_HIS_SERE_SERV> VHisSereServs = new List<V_HIS_SERE_SERV>();

                // LẤY THEO TREATMENT
                if (this.Treatment != null)
                {
                    HisSereServViewFilter ssViewFilter = new HisSereServViewFilter();
                    ssViewFilter.TREATMENT_ID = this.Treatment.ID;

                    // Nếu class filter có sẵn field FROM/TO thì dùng, còn không thì lọc bằng LINQ bên dưới.
                    // Ví dụ (nếu có):
                    // ssViewFilter.TDL_INTRUCTION_TIME_FROM = fromDateReq;
                    // ssViewFilter.TDL_INTRUCTION_TIME_TO   = toDateReq;

                    VHisSereServs = new Inventec.Common.Adapter.BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>>(
                            "api/HisSereServ/GetView",
                            ApiConsumer.ApiConsumers.MosConsumer,
                            ssViewFilter,
                            param
                        );
                }

                if (VHisSereServs == null || VHisSereServs.Count == 0)
                {
                    if (IsActionButtonPrintBill)
                        XtraMessageBox.Show("Không có dịch vụ cần thanh toán!", "Thông báo");
                    return result;
                }

                // LỌC THEO KHOẢNG THỜI GIAN TDL_INTRUCTION_TIME
                VHisSereServs = VHisSereServs
                    .Where(o => o.TDL_INTRUCTION_TIME >= fromDateReq
                             && o.TDL_INTRUCTION_TIME <= toDateReq)
                    .ToList();

                if (VHisSereServs == null || VHisSereServs.Count == 0)
                {
                    if (IsActionButtonPrintBill)
                        XtraMessageBox.Show("Không có dịch vụ trong khoảng thời gian đã chọn!", "Thông báo");
                    return result;
                }

                // Tạo PDO với danh sách đã lọc + khoảng thời gian
                Mps000504PDO rdo = new Mps000504PDO(treatment, VHisSereServs, fromDateReq, toDateReq);

                rdo.SurchargePayforms = this.SurchargePayforms; // PTTK 2656 - mục 4.2.8

                // PTTK 2883 - muc 2: nap input cho pipeline gom nhom theo khoa/phong (ExeRoom)
                // de temp 6556 (bang ke theo KHOA) dung duoc cac key ReqExeDepaRoom/ReqExeRoom/...ExeRoom
                LoadExeRoomInput(rdo);

                #region Run Print
                PrintCustomShow<Mps000504PDO> printShow = new PrintCustomShow<Mps000504PDO>(
                    printTypeCode,
                    fileName,
                    rdo,
                    returnEventPrint,
                    this.isPreview
                );
                result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId);
                #endregion
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// PTTK 2883 - muc 2: nap du lieu cho pipeline gom nhom chi phi theo khoa/phong xu ly (ExeRoom)
        /// cua Mps000504 — tuong tu Mps000304Behavior. Chi truyen cac dich vu trong khoang loc
        /// [fromDateReq, toDateReq] (TDL_INTRUCTION_TIME). Neu loi thi bo qua — bieu in van chay
        /// voi danh sach phang nhu cu (cac key ...ExeRoom se rong).
        /// </summary>
        private void LoadExeRoomInput(Mps000504PDO rdo)
        {
            try
            {
                if (this.SereServs == null || this.SereServs.Count == 0 || this.Treatment == null)
                    return;

                // Chi lay dich vu trong khoang thoi gian loc
                List<HIS_SERE_SERV> sereServFilters = this.SereServs
                    .Where(o => o.TDL_INTRUCTION_TIME >= fromDateReq && o.TDL_INTRUCTION_TIME <= toDateReq)
                    .ToList();
                if (sereServFilters.Count == 0)
                    return;

                CommonParam param = new CommonParam();

                MPS.Processor.Mps000504.PDO.PatientTypeCFG patientTypeCFG = new MPS.Processor.Mps000504.PDO.PatientTypeCFG();
                patientTypeCFG.PATIENT_TYPE__BHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                patientTypeCFG.PATIENT_TYPE__FEE = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;

                MPS.Processor.Mps000504.PDO.HisConfigValue hisConfigValue = new MPS.Processor.Mps000504.PDO.HisConfigValue();
                hisConfigValue.IsPriceWithDifference = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.IS_PRICE_WITH_DIFFERENCE)) == 1;
                hisConfigValue.IsNotSameDepartment = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.MOS__BHYT__CALC_MATERIAL_PACKAGE_PRICE_OPTION)) == 1;
                hisConfigValue.IsGroupReqDepartment = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.IS_GROUP_REQUEST_DEPARTMENT)) == 1;
                hisConfigValue.IsGroupHeinServiceByUseTime = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(SdaConfigKey.ConfigKey_IsGroupHeinServiceByUseTime) == 1;

                HisSereServExtFilter sereServExtFilter = new HisSereServExtFilter();
                sereServExtFilter.SERE_SERV_IDs = sereServFilters.Select(o => o.ID).ToList();
                List<HIS_SERE_SERV_EXT> sereServExts = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServExtFilter, param);

                HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
                patientTypeAlterFilter.TREATMENT_ID = this.Treatment.ID;
                patientTypeAlterFilter.ORDER_FIELD = "LOG_TIME";
                patientTypeAlterFilter.ORDER_DIRECTION = "ASC";
                List<HIS_PATIENT_TYPE_ALTER> patientTypeAlters = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterFilter, param);

                long isShowMedicineLine = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.IS_SHOW_MEDICINE_LINE));
                List<HIS_MEDICINE_TYPE> medicineTypes = null;
                List<HIS_MEDICINE_LINE> medicineLines = null;
                List<HIS_SERVICE_REQ> serviceReqs = null;
                if (isShowMedicineLine == 1)
                {
                    medicineTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE>();
                    medicineLines = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_LINE>();
                }
                if (isShowMedicineLine == 1 || hisConfigValue.IsGroupHeinServiceByUseTime)
                {
                    HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.TREATMENT_ID = this.Treatment.ID;
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                    serviceReqs = new Inventec.Common.Adapter.BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, param);
                }

                rdo.TreatmentView = this.Treatment;
                rdo.SereServs = sereServFilters;
                rdo.SereServExts = sereServExts;
                rdo.HeinServiceTypes = this.HeinServiceTypes;
                rdo.Services = this.Services;
                rdo.Rooms = this.Rooms;
                rdo.Departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();
                rdo.materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                rdo.medicineTypes = medicineTypes;
                rdo.MedicineLines = medicineLines;
                rdo.ServiceReqs = serviceReqs;
                rdo.PatientTypeAlterAlls = patientTypeAlters;
                rdo.CurrentPatyAlter = this.CurrentPatientTypeAlter;
                rdo.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                rdo.TreatmentTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                rdo.PatientTypeCFG = patientTypeCFG;
                rdo.HisConfigValue = hisConfigValue;
                rdo.HisServiceUnit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>();
                rdo.ListOtherPaySource = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
