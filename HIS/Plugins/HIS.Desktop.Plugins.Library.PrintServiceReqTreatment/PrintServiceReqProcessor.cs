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
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintServiceReqTreatment
{
    public class PrintServiceReqTreatmentProcessor
    {
        public Action<DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned { get; set; }
        private bool printNow;
        private List<V_HIS_SERVICE_REQ> _ServiceReqs { get; set; }
        //private HIS_TREATMENT _CurrentTreatment { get; set; }
        private List<V_HIS_PATIENT_TYPE_ALTER> _PatientTypeAlter { get; set; }
        private V_HIS_ROOM _vHisRoom { get; set; }
        private long _TreatmentId;
        private List<V_HIS_SERVICE_REQ> _vHisServiceReqs { get; set; }
        private List<HIS_TREATMENT> _ListTreatment;
        private List<HIS_SERE_SERV> _SereServs { get; set; }
        private List<HIS_CONFIG> lstConfig { get; set; }
        private HIS_TRANS_REQ transReq { get; set; }
        public PrintServiceReqTreatmentProcessor(List<V_HIS_SERVICE_REQ> _vhisServiceReqs, long roomId)
        {
            try
            {
                this._vHisServiceReqs = _vhisServiceReqs;
                if (_vhisServiceReqs != null && _vhisServiceReqs.Count > 0)
                {
                    this._TreatmentId = _vhisServiceReqs.FirstOrDefault().TREATMENT_ID;
                }
                this._vHisRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == roomId);
                Config.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public PrintServiceReqTreatmentProcessor(List<HIS_TREATMENT> _listTreatment, long roomId)
        {
            try
            {
                this._ListTreatment = _listTreatment;
                this._vHisRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == roomId);
                Config.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Sử dụng cấu hình để in ngay ("HIS.Desktop.Plugins.Library.PrintServiceReqTreatment.Mps")
        /// </summary>
        public void Print()
        {
            try
            {
                Print(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.mps));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// lưu in chỉ định
        /// </summary>
        /// <param name="PrintMany"></param>
        public void SaveNPrint()
        {
            try
            {
                WaitingManager.Show();
                this.printNow = true;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                InCacPhieuChiDinhProcess(richEditorMain);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// in ngay
        /// </summary>
        /// <param name="PrintTypeCode">mã in</param>
        public void Print(string PrintTypeCode)
        {
            try
            {
                Print(PrintTypeCode, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PrintTypeCode">Mã in (8,10,11)</param>
        /// <param name="PrintNow">true/false</param>
        public void Print(string PrintTypeCode, bool PrintNow)
        {
            try
            {
                this.printNow = PrintNow;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);

                richEditorMain.RunPrintTemplate(PrintTypeCode, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InCacPhieuChiDinhProcess(Inventec.Common.RichEditor.RichEditorStore richEditorMain)
        {
            try
            {
                richEditorMain.RunPrintTemplate(PrintTypeCodeStore.IN__HUONG_DAN_CLS__KHAM, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                if (ProcessDataBeforePrint())
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeStore.IN__HUONG_DAN_CLS__KHAM:
                            new InCacPhieuChiDinh(printTypeCode, fileName, this._ServiceReqs, this._SereServs, this._ListTreatment, this._PatientTypeAlter, this._vHisRoom, printNow, ref result, lstConfig, transReq, DlgSendResultSigned);
                            break;

                        default:
                            break;
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool ProcessDataBeforePrint()
        {
            bool result = false;
            try
            {
                if (this._TreatmentId > 0)
                {
                    _ListTreatment = new List<HIS_TREATMENT>();
                    _PatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();

                    MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                    treatmentFilter.ID = this._TreatmentId;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        _ListTreatment.Add(apiResult.FirstOrDefault());

                        HisPatientTypeAlterViewAppliedFilter _patientTypeAlterFilter = new HisPatientTypeAlterViewAppliedFilter();
                        _patientTypeAlterFilter.TreatmentId = this._TreatmentId;
                        _patientTypeAlterFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                        var patyalRs = new BackendAdapter(new CommonParam()).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, _patientTypeAlterFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (patyalRs != null)
                        {
                            _PatientTypeAlter.Add(patyalRs);
                        }
                    }

                    if (this._vHisServiceReqs != null && this._vHisServiceReqs.Count > 0)
                    {
                        this._ServiceReqs = this._vHisServiceReqs.Where(p =>
                            p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN
                            && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                            && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                            && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM
                            && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                            && p.START_TIME == null).ToList();
                        GetDataPrintQrCode();
                        if (this._ServiceReqs != null && this._ServiceReqs.Count > 0)
                        {
                            HisSereServFilter ssFilter = new HisSereServFilter();
                            ssFilter.TREATMENT_ID = this._TreatmentId;
                            ssFilter.SERVICE_REQ_IDs = this._ServiceReqs.Select(s => s.ID).ToList();
                            ssFilter.HAS_EXECUTE = true;
                            this._SereServs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        }
                    }
                }
                else if (_ListTreatment != null && _ListTreatment.Count > 0)
                {
                    if (_PatientTypeAlter == null) _PatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
                    if (_ServiceReqs == null) _ServiceReqs = new List<V_HIS_SERVICE_REQ>();
                    if (_SereServs == null) _SereServs = new List<HIS_SERE_SERV>();

                    int skip = 0;
                    while (_ListTreatment.Count - skip > 0)
                    {
                        var lstIds = _ListTreatment.Skip(skip).Take(100).ToList();
                        skip += 100;
                        HisPatientTypeAlterViewFilter _patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                        _patientTypeAlterFilter.TREATMENT_IDs = lstIds.Select(s => s.ID).ToList();
                        var lstPatyAlter = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT_TYPE_ALTER>>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_VIEW, ApiConsumer.ApiConsumers.MosConsumer, _patientTypeAlterFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (lstPatyAlter != null && lstPatyAlter.Count > 0)
                        {
                            var grouptreat = lstPatyAlter.GroupBy(g => g.TREATMENT_ID).OrderByDescending(o => o.First().LOG_TIME).Select(s => s.First()).ToList();
                            if (grouptreat != null && grouptreat.Count > 0)
                                _PatientTypeAlter.AddRange(grouptreat);
                        }

                        HisServiceReqViewFilter reqFilter = new HisServiceReqViewFilter();
                        reqFilter.TREATMENT_IDs = lstIds.Select(s => s.ID).ToList();
                        reqFilter.HAS_EXECUTE = true;
                        var lstReq = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, reqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (lstReq != null && lstReq.Count > 0)
                        {
                            this._ServiceReqs.AddRange(lstReq);
                        }

                        HisSereServFilter ssFilter = new HisSereServFilter();
                        ssFilter.TREATMENT_IDs = lstIds.Select(s => s.ID).ToList();
                        ssFilter.HAS_EXECUTE = true;
                        var lstSS = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (lstSS != null && lstSS.Count > 0)
                        {
                            this._SereServs.AddRange(lstSS);
                        }
                    }
                    this._ServiceReqs = this._ServiceReqs.Where(p =>
                           p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN
                           && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                           && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                           && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM
                           && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                           && p.START_TIME == null).ToList();
                }

                if (this._ServiceReqs != null && this._ServiceReqs.Count > 0)
                {
                    this._ServiceReqs = this._ServiceReqs.OrderBy(p => p.ESTIMATE_TIME_FROM ?? 99999999999999).ToList();
                    result = true;
                }
                else
                    Inventec.Common.Logging.LogSystem.Error("Du lieu rong");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        private void GetDataPrintQrCode()
        {
            try
            {
                lstConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();
                if (lstConfig != null && lstConfig.Count > 0 && _ServiceReqs != null && _ServiceReqs.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisTransReqFilter filter = new HisTransReqFilter();
                    filter.TREATMENT_ID = _vHisServiceReqs.First().TREATMENT_ID;
                    var transReqLst = new Inventec.Common.Adapter.BackendAdapter(param)
                      .Get<List<MOS.EFMODEL.DataModels.HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (transReqLst != null && transReqLst.Count > 0)
                        transReqLst = transReqLst.Where(o => o.TRANS_REQ_TYPE == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_SERVICE).ToList();
                    if (transReqLst != null && transReqLst.Count > 0)
                        transReq = transReqLst.OrderByDescending(o => o.CREATE_TIME).ToList()[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal GetDefaultHeinRatio(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            decimal result = 0;
            try
            {
                result = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
