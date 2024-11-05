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
using DevExpress.Utils.Menu;
using HIS.Common.Treatment;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using HIS.UC.MenuPrint.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau
{
    public partial class PrintBordereauProcessor
    {
        public void InitData(BordereauInitData data)
        {
            try
            {
                if (data != null)
                {
                    if (data.DepartmentTrans != null)
                    {
                        this.DepartmentTrans = data.DepartmentTrans;
                    }
                    if (data.Patient != null)
                    {
                        this.Patient = data.Patient;
                    }
                    if (data.Treatment != null)
                    {
                        this.Treatment = data.Treatment;
                    }
                    if (data.SereServs != null)
                    {
                        this.SereServs = data.SereServs;
                    }
                    if (data.TreatmentFees != null)
                    {
                        this.TreatmentFees = data.TreatmentFees;
                    }
                    if (data.PatientTypeAlter != null)
                    {
                        this.PatientTypeAlter = data.PatientTypeAlter;
                    }
                    if (!String.IsNullOrEmpty(data.UserNameReturnResult))
                    {
                        this.UserNameReturnResult = data.UserNameReturnResult;
                    }
                    if (data.SereServDeposits != null)
                    {
                        this.SereServDeposits = data.SereServDeposits; //do cho nay thi phai
                    }
                    if (data.SeseDepoRepays != null)
                    {
                        this.SeseDepoRepays = data.SeseDepoRepays;
                    }
                    if (data.CurrentDepartmentId.HasValue)
                    {
                        this.CurrentDepartmentId = data.CurrentDepartmentId;
                    }
                }

                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.KEY_IsPrintPrescriptionNoThread) == "1")
                {
                    LoadSereServ();
                    LoadPatientTypeAlter();
                    LoadTreatment();
                    LoadDepartmentTran();
                }
                else
                {
                    List<Action> methods = new List<Action>();
                    methods.Add(LoadSereServ);
                    methods.Add(LoadPatientTypeAlter);
                    methods.Add(LoadTreatment);
                    methods.Add(LoadDepartmentTran);
                    ThreadCustomManager.MultipleThreadWithJoin(methods);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                WaitingManager.Show();
                List<Action> methods = new List<Action>();
                methods.Add(LoadPatient);
                methods.Add(LoadTreatment);
                methods.Add(LoadTreatmentFee);
                methods.Add(LoadDepartmentTran);
                methods.Add(LoadSereServBill);
                methods.Add(LoadSereServDeposit);
                methods.Add(LoadSeseDepoRepay);
                methods.Add(GetDataPrintQrCode);
                ThreadCustomManager.MultipleThreadWithJoin(methods);

                if (!String.IsNullOrEmpty(this.Treatment.MEDI_ORG_CODE))
                {
                    this.StatusTreatmentOut = "CV";
                }
                else
                {
                    if (SereServs != null && SereServs.Count > 0)
                    {
                        var sereServExam = SereServs.FirstOrDefault(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH);
                        var sereServMedi = SereServs.FirstOrDefault(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                        var sereServCLS = SereServs.FirstOrDefault(o =>
                            o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN);
                        this.StatusTreatmentOut = (sereServMedi == null && sereServExam != null && sereServCLS != null)
                            || (sereServMedi == null && sereServExam != null && sereServCLS == null) ? "CLS" : "";
                    }
                }

                if (this.Treatment != null)
                {
                    if (this.Treatment.TREATMENT_DAY_COUNT.HasValue)
                    {
                        this.TotalDayTreatment = (long)this.Treatment.TREATMENT_DAY_COUNT.Value;
                    }
                    else
                    {
                        if (this.Treatment.OUT_TIME.HasValue)
                        {
                            if (PatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                                this.TotalDayTreatment = HIS.Common.Treatment.Calculation.DayOfTreatment(this.Treatment.CLINICAL_IN_TIME, this.Treatment.OUT_TIME, this.Treatment.TREATMENT_END_TYPE_ID, this.Treatment.TREATMENT_RESULT_ID, PatientTypeEnum.TYPE.BHYT) ?? 0;
                            else
                                this.TotalDayTreatment = HIS.Common.Treatment.Calculation.DayOfTreatment(this.Treatment.CLINICAL_IN_TIME, this.Treatment.OUT_TIME, this.Treatment.TREATMENT_END_TYPE_ID, this.Treatment.TREATMENT_RESULT_ID, PatientTypeEnum.TYPE.THU_PHI) ?? 0;
                        }
                    }
                }

                var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);
                if (room != null)
                {
                    this.DepartmentName = room.DEPARTMENT_NAME;
                    this.RoomName = room.ROOM_NAME;
                }

                this.Rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
                this.HeinServiceTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();
                this.Services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadTreatment()
        {
            try
            {
                if (this.Treatment == null)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.ID = this.treatmentId;
                    this.Treatment = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                    if (this.Treatment == null)
                        throw new Exception("Khong lay duoc du lieu Treatment");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDepartmentTran()
        {
            try
            {
                if (this.DepartmentTrans == null)
                {
                    CommonParam param = new CommonParam();
                    HisDepartmentTranViewFilter departmentTranFilter = new HisDepartmentTranViewFilter();
                    departmentTranFilter.TREATMENT_ID = treatmentId;
                    this.DepartmentTrans = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departmentTranFilter, param).OrderBy(o => o.DEPARTMENT_IN_TIME ?? 9999999999999999).ToList();
                    if (this.DepartmentTrans == null)
                        throw new Exception("Khong lay duoc du lieu DepartmentTrans");
                }
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
                if (this.SereServBills == null)
                {
                    CommonParam param = new CommonParam();
                    HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
                    sereServBillFilter.TDL_TREATMENT_ID = treatmentId;
                    sereServBillFilter.IS_NOT_CANCEL = true;
                    this.SereServBills = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, sereServBillFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServDeposit()
        {
            try
            {
                if (this.SereServDeposits == null)
                {
                    CommonParam param = new CommonParam();
                    HisSereServDepositFilter sereServDepositFilter = new HisSereServDepositFilter();
                    sereServDepositFilter.TDL_TREATMENT_ID = treatmentId;
                    sereServDepositFilter.IS_CANCEL = false;
                    this.SereServDeposits = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, sereServDepositFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSeseDepoRepay()
        {
            try
            {
                if (this.SeseDepoRepays == null)
                {
                    CommonParam param = new CommonParam();
                    HisSeseDepoRepayFilter seseDepoRepayFilter = new HisSeseDepoRepayFilter();
                    seseDepoRepayFilter.TDL_TREATMENT_ID = treatmentId;
                    seseDepoRepayFilter.IS_CANCEL = false;
                    this.SeseDepoRepays = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumers.MosConsumer, seseDepoRepayFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        class ConfigInfo
        {
            public string BANK { get; set; }
            public string VALUE { get; set; }
        }
        private void GetDataPrintQrCode()
        {
            try
            {
                CommonParam param = new CommonParam();
                
                var currentRoom = BackendDataWorker.Get<HIS_ROOM>().Where(s => s.ID == this.roomId && !string.IsNullOrEmpty(s.QR_CONFIG_JSON)).FirstOrDefault();
                if (currentRoom != null)
                {
                    if (this.lstConfig == null)
                        lstConfig = new List<HIS_CONFIG>();
                    lstConfig.Clear();
                    ConfigInfo _config = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigInfo>(currentRoom.QR_CONFIG_JSON);
                    HIS_CONFIG _cf = new HIS_CONFIG();
                    if (string.IsNullOrWhiteSpace(_config.BANK))
                        Inventec.Common.Logging.LogSystem.Warn("Cau hinh thieu thong tin ngan hang.roomID: ("+roomId+"). Cau hinh: "+currentRoom.QR_CONFIG_JSON);
                    _cf.KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", _config.BANK.Trim());
                    _cf.VALUE = _config.VALUE;
                    lstConfig.Add(_cf);
                }
                else
                {

                    //neu cau hinh phong lam viec k co QR
                    lstConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();
                    
                }
                if (lstConfig != null && lstConfig.Count > 0 && this.Treatment != null)
                {
                    HisTransReqFilter filter = new HisTransReqFilter();
                    filter.TREATMENT_ID = Treatment.ID;
                    var transReqLst = new Inventec.Common.Adapter.BackendAdapter(param)
                      .Get<List<MOS.EFMODEL.DataModels.HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (transReqLst != null && transReqLst.Count > 0)
                    {
                        var transReqLstType3 = transReqLst.Where(o => o.TRANS_REQ_TYPE == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_TREATMENT
                                                                    && o.TRANS_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__CANCEL).ToList();
                        if (transReqLstType3 != null && transReqLstType3.Count > 0)
                            transReq = transReqLstType3.OrderByDescending(o => o.CREATE_TIME).ToList()[0];

                        var transReqLstType2 = transReqLst.Where(o => o.TRANS_REQ_TYPE == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_SERVICE).ToList();
                        if (transReqLstType2 != null && transReqLstType2.Count > 0)
                        {
                            var transReq2Tmp = transReqLstType2.OrderByDescending(o => o.CREATE_TIME).ToList()[0];
                            if (transReq2Tmp != null && transReq2Tmp.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__REQUEST)
                                transReq2 = transReq2Tmp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadTransactionView()
        {
            try
            {
                if (this.Treatment != null)
                {
                    CommonParam param = new CommonParam();
                    HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                    tranFilter.TREATMENT_ID = this.Treatment.ID;
                    tranFilter.HAS_SALE_TYPE_ID = false;
                    tranFilter.IS_CANCEL = false;
                    this.Transactions = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadTransaction()
        {
            try
            {
                if (this.Treatment != null)
                {
                    CommonParam param = new CommonParam();
                    HisTransactionFilter tranFilter = new HisTransactionFilter();
                    tranFilter.TREATMENT_ID = this.Treatment.ID;
                    tranFilter.HAS_SALE_TYPE_ID = false;
                    tranFilter.IS_CANCEL = false;
                    this.ListTransaction = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, tranFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadPatient()
        {
            try
            {
                if (this.Patient == null)
                {
                    CommonParam param = new CommonParam();
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = patientId;
                    var apiResult = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        this.Patient = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatmentFee()
        {
            try
            {
                if (this.TreatmentFees == null)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFeeViewFilter treatmentFeeFilter = new HisTreatmentFeeViewFilter();
                    treatmentFeeFilter.ID = treatmentId;
                    this.TreatmentFees = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, treatmentFeeFilter, param);
                }
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
                if (this.SereServs == null || this.SereServs.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    HisSereServFilter filter = new HisSereServFilter();
                    filter.TREATMENT_ID = this.treatmentId;
                    this.SereServs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientTypeAlter()
        {
            try
            {
                if (this.PatientTypeAlter == null)
                {
                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = treatmentId;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    this.PatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckBordereauType(ref bool isBHYT, ref bool isVienPhi)
        {
            try
            {
                if (this.SereServs != null && this.SereServs.Count > 0)
                {
                    long patientTypeIdBHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                    var sereServ_BHYT = this.SereServs.Where(o =>
                        o.PATIENT_TYPE_ID == patientTypeIdBHYT).ToList();

                    if (sereServ_BHYT != null && sereServ_BHYT.Count > 0)
                    {
                        isBHYT = true;
                    }

                    var sereServ_VienPhi = this.SereServs.Where(
                        o =>
                            (o.PATIENT_TYPE_ID != patientTypeIdBHYT)
                            || (o.PATIENT_TYPE_ID == patientTypeIdBHYT && ((o.HEIN_LIMIT_PRICE != null && o.HEIN_LIMIT_PRICE <= o.VIR_PRICE) ? (o.VIR_PRICE - o.HEIN_LIMIT_PRICE) : 0) > 0)
                            || (o.PATIENT_TYPE_ID == patientTypeIdBHYT && (o.VIR_TOTAL_HEIN_PRICE ?? 0) == 0)).ToList();
                    if (sereServ_VienPhi != null && sereServ_VienPhi.Count > 0)
                    {
                        isVienPhi = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private Dictionary<string, string> GetMpsReplaceFromCFG(string mpsReplaceCFG)
        {
            Dictionary<string, string> dic = null;
            try
            {
                if (!String.IsNullOrEmpty(mpsReplaceCFG))
                {
                    string[] mpsReplaceCFGArr = mpsReplaceCFG.Split('|');
                    for (int i = 0; i < mpsReplaceCFGArr.Length; i++)
                    {
                        string[] itemMpsReplace = Regex.Split(mpsReplaceCFGArr[i], "->");
                        if (itemMpsReplace.Length != 2)
                        {
                            Inventec.Common.Logging.LogSystem.Error("Loi dinh dang cau hinh Replace: " + mpsReplaceCFGArr[i]);
                            continue;
                            //return null;
                        }

                        if (dic == null)
                            dic = new Dictionary<string, string>();

                        dic[itemMpsReplace[0].Trim()] = itemMpsReplace[1].Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                dic = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dic;
        }

        /// <summary>
        /// Load in mặc định và thay đổi mps theo key cấu hình
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="mpsCode"></param>
        private void LoadPrintDefaultWithReplaceMps(Dictionary<string, string> dic, string mpsCode)
        {
            try
            {
                if (dic != null && dic.ContainsKey(mpsCode))
                    this.RunPrint(dic[mpsCode]);
                else
                    this.RunPrint(mpsCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Load in mặc định và thay đổi mps theo key cấu hình V2// KTra xem 2 mã in cũ được đổi thành mã in mới giống nhau k?
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="mpsCode"></param>
        private bool LoadPrintDefaultWithReplaceMpsV2(Dictionary<string, string> dic, string mpsCode, string mpsCode2)
        {
            bool result = false;
            try
            {
                if (dic != null
                    && dic.ContainsKey(mpsCode)
                    && dic.ContainsKey(mpsCode2)
                    && dic[mpsCode] == dic[mpsCode2])
                    result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
