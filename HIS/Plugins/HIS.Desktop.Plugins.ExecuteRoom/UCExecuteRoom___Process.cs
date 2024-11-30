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
using System.Collections;
using Inventec.Desktop.Plugins.ExecuteRoom.Delegate;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.UC.TreeSereServ7V2;
using HIS.Desktop.Plugins.ExecuteRoom.Base;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using AutoMapper;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using EMR.Filter;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.LanguageManager;
using System.Threading;

namespace HIS.Desktop.Plugins.ExecuteRoom
{
    public partial class UCExecuteRoom : UserControlBase
    {
        private void CancelFinish(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (HisConfigCFG.AutoDeleteEmrDocumentWhenEditReq == "1" && HisConfigCFG.IsHasConnectionEmr)
                {
                    EmrDocumentViewFilter emrDocumentFilter = new EmrDocumentViewFilter();
                    emrDocumentFilter.TREATMENT_CODE__EXACT = serviceReqInput.TDL_TREATMENT_CODE;
                    emrDocumentFilter.IS_DELETE = false;
                    emrDocumentFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                    var documents = new BackendAdapter(new CommonParam()).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, emrDocumentFilter, null);
                    if (documents != null && documents.Count() > 0)
                    {
                        var checkServiceReqCode = "SERVICE_REQ_CODE:" + serviceReqInput.SERVICE_REQ_CODE;
                        var resultEmrDocumentLast = documents.Where(o => !string.IsNullOrEmpty(o.HIS_CODE) && o.HIS_CODE.Contains(checkServiceReqCode));
                        if (resultEmrDocumentLast != null && resultEmrDocumentLast.Count() > 0)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Y lệnh đã tồn tại văn bản ký, tiếp tục sẽ tự động Xóa văn bản ký hiện tại. Bạn có muốn tiếp tục?", Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                                return;

                            WaitingManager.Show();
                            foreach (var item in resultEmrDocumentLast)
                            {
                                var result = new BackendAdapter(new CommonParam()).Post<bool>("api/EmrDocument/Delete", ApiConsumers.EmrConsumer, item.ID, null);
                            }
                            WaitingManager.Hide();

                        }
                    }
                }

                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                var serviceReq = new BackendAdapter(param)
                .Post<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UNFINISH, ApiConsumers.MosConsumer, serviceReqInput.ID, param);
                if (serviceReq != null)
                {
                    success = true;
                    //Reload data
                    foreach (var item in serviceReqs)
                    {
                        if (item.ID == serviceReq.ID)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<L_HIS_SERVICE_REQ>(item, serviceReq);
                            break;
                        }
                    }
                    gridControlServiceReq.RefreshDataSource();
                    SetTextButtonExecute(serviceReq);
                }

                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadLoadDataDefault(L_HIS_SERVICE_REQ serviceReqInput)
        {
            Thread serviceReq = new Thread(() => SendServiceReq(serviceReqInput));
            try
            {
                serviceReq.Start();
                //serviceReq.Join();
             
            }
            catch (Exception ex)
            {
                serviceReq.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SendServiceReq(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                if (serviceReqInput != null)
                {
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisServiceReq/RequestOrder", ApiConsumers.MosConsumer, serviceReqInput.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }
                WaitingManager.Hide();
                if (success == false)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqInput.ID), serviceReqInput.ID));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void StartEvent(ref bool isStart, L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                //Bắt đầu
                #region 52227
                if (serviceReqInput.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && (HisConfigCFG.SignWarningOption == "1" || HisConfigCFG.SignWarningOption == "2") && HisConfigCFG.IsHasConnectionEmr)
                {
                    EmrDocumentCountByHisCodeFilter documentCountFilter = new EmrDocumentCountByHisCodeFilter();
                    documentCountFilter.TreatmentCodeExact = serviceReqInput.TDL_TREATMENT_CODE;
                    documentCountFilter.HisCode = string.Format("SERVICE_REQ_CODE:{0}", serviceReqInput.SERVICE_REQ_CODE);
                    documentCountFilter.HasSigner = true;
                    documentCountFilter.NotReject = true;
                    var rs = new BackendAdapter(new CommonParam()).Get<long?>("api/EmrDocument/CountByHisCode", ApiConsumers.EmrConsumer, documentCountFilter, null);
                    if (rs == null || rs <= 0)
                    {
                        if (HisConfigCFG.SignWarningOption == "1")
                        {
                            MessageBox.Show("Y lệnh chưa có văn bản ký điện tử không cho phép xử lý", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            return;
                        }
                        else if (HisConfigCFG.SignWarningOption == "2")
                        {
                            if (MessageBox.Show("Y lệnh chưa có văn bản ký điện tử. Bạn có muốn tiếp tục xử lý?", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                                return;
                        }
                    }
                }
                #endregion

                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                L_HIS_SERVICE_REQ serviceReqResult = new BackendAdapter(param)
                .Post<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_START, ApiConsumers.MosConsumer, serviceReqInput.ID, param);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                
                switch (serviceReqInput.SERVICE_REQ_TYPE_ID)
                       {
                          case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH:
                              {
                                break;
                              }
                          case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT:
                              {
                                  break;
                              }
                          case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM:
                              {
                                  break;
                              }
                          case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT:
                              {
                                  break;
                              }
                          case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G:
                              {
                                  break;
                              }
                          case IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN:
                              {
                                  break;
                              }
                          default:
                              {
                                  if (HisConfigCFG.SendToExtWhenStart != null)
                                  {
                                      CreateThreadLoadDataDefault(serviceReqInput);
                                  }
                                  break;
                              }
                       }
                if (serviceReqResult == null)
                {
                    bool IsShowMessErr = true;
                    if (param.MessageCodes.Contains("HisServiceReq_KhongChoPhepBatDauKhiThieuVienPhi"))
                    {
                        if (HisConfigCFG.IsUsingExecuteRoomPayment)
                        {
                            var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);
                            if (room.DEPOSIT_ACCOUNT_BOOK_ID != null && room.DEFAULT_CASHIER_ROOM_ID != null)
                            {
                                HisCardFilter filter = new HisCardFilter();
                                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                                filter.PATIENT_ID = serviceReqInput.TDL_PATIENT_ID;
                                var cards = new BackendAdapter(new CommonParam()).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumers.MosConsumer, filter, new CommonParam());
                                if (cards != null && cards.Count > 0)
                                {
                                    IsShowMessErr = false;
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("{0} Bạn có muốn đóng tiền không?", param.GetMessage()), Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    {
                                        WaitingManager.Show();
                                        EpaymentDepositSD sd = new EpaymentDepositSD();
                                        sd.RequestRoomId = roomId;
                                        sd.ServiceReqIds = new List<long>() { serviceReqInput.ID };
                                        sd.CardServiceCode = null;
                                        if (serviceReqInput.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                                        {
                                            sd.IncludeAttachment = true;
                                        }
                                        else
                                        {
                                            sd.IncludeAttachment = false;
                                        }
                                        CommonParam paramEpay = new CommonParam();
                                        this.epaymentDepositResultSDO = new BackendAdapter(paramEpay).Post<EpaymentDepositResultSDO>("api/HisTransaction/EpaymentDeposit", ApiConsumers.MosConsumer, sd, paramEpay);
                                        WaitingManager.Hide();
                                        if (this.epaymentDepositResultSDO != null)
                                        {
                                            this.treatmentId = serviceReqInput.TREATMENT_ID;
                                            Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), LocalStorage.LocalData.GlobalVariables.TemnplatePathFolder);
                                            richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__MPS000102, ProcessPrintMps000102);
                                            param = new CommonParam();
                                            serviceReqResult = new BackendAdapter(param)
                .Post<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_START, ApiConsumers.MosConsumer, serviceReqInput.ID, param);
                                        }
                                        else
                                        {
                                            ResultManager.ShowMessage(paramEpay, false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (IsShowMessErr)
                    {
                        #region Show message
                        ResultManager.ShowMessage(param, null);
                        Inventec.Common.Logging.LogSystem.Warn("StartEvent fail:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqResult), serviceReqResult));
                        #endregion
                        return;
                    }
                }

                if (serviceReqResult != null)
                {
                    long dtFrom = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "000000");
                    long dtTo = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "232359");
                    if (dtFrom <= serviceReqResult.INTRUCTION_TIME && serviceReqResult.INTRUCTION_TIME <= dtTo)
                    {
                        if (serviceReqResult.EXECUTE_LOGINNAME != null)
                        {
                            if (serviceReqResult.EXECUTE_LOGINNAME.Equals(loginName))
                            {
                                LoadServiceReqCount(false, 1);
                            }
                        }

                    }
                    //LoadServiceReqCount(false, 1);
                    foreach (var item in serviceReqs)
                    {
                        if (currentHisServiceReq != null && item.ID == currentHisServiceReq.ID)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<L_HIS_SERVICE_REQ>(item, serviceReqResult);
                            break;
                        }
                    }
                    if (currentHisServiceReq != null && serviceReqInput.ID == currentHisServiceReq.ID)
                        currentHisServiceReq.SERVICE_REQ_STT_ID = serviceReqResult.SERVICE_REQ_STT_ID;
                    gridControlServiceReq.RefreshDataSource();
                    SetTextButtonExecute(currentHisServiceReq);
                    LoadSereServCount();
                    isStart = true;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private bool ProcessPrintMps000102(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.ID = this.treatmentId;
                var treatmentFees = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                HisPatientTypeAlterViewFilter filterPatienTypeAlter = new HisPatientTypeAlterViewFilter();
                filterPatienTypeAlter.TREATMENT_ID = treatmentId;
                var patientTypeAlter = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filterPatienTypeAlter, param).OrderByDescending(o => o.ID).ThenByDescending(o => o.LOG_TIME).FirstOrDefault();

                HisPatientViewFilter filterPatient = new HisPatientViewFilter();
                filterPatient.ID = treatmentFees != null ? treatmentFees.PATIENT_ID : 0;
                var patientPrint = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, filterPatient, param).FirstOrDefault();

                if (this.epaymentDepositResultSDO != null && this.epaymentDepositResultSDO.SereServDeposit != null && this.epaymentDepositResultSDO.SereServDeposit.Count > 0 && this.epaymentDepositResultSDO.Transaction != null)
                {
                    V_HIS_TRANSACTION transactionPrint = new V_HIS_TRANSACTION();
                    List<HIS_SERE_SERV_DEPOSIT> ssDepositPrint = new List<HIS_SERE_SERV_DEPOSIT>();
                    if (this.epaymentDepositResultSDO.Transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                    {
                        transactionPrint = this.epaymentDepositResultSDO.Transaction;
                    }
                    if (transactionPrint == null)
                        return result;
                    ssDepositPrint = this.epaymentDepositResultSDO.SereServDeposit.Where(o => o.DEPOSIT_ID == transactionPrint.ID).ToList();

                    //chỉ định chưa có thời gian ra viện nên chưa cso số ngày điều trị
                    long? totalDay = null;
                    string departmentName = "";

                    //sử dụng SereServs để hiển thị thêm dịch vụ thanh toán cha
                    List<V_HIS_SERE_SERV> sereServs = new List<V_HIS_SERE_SERV>();
                    if (this.epaymentDepositResultSDO.SereServs != null && this.epaymentDepositResultSDO.SereServs.Count > 0)
                    {
                        sereServs = this.epaymentDepositResultSDO.SereServs.Where(o => ssDepositPrint.Exists(e => e.SERE_SERV_ID == o.ID)).ToList();
                    }
                    var SERVICE_REPORT_ID__HIGHTECH = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;

                    var sereServHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT_ID__HIGHTECH).ToList();
                    var sereServHitechADOs = PriceBHYTSereServAdoProcess(sereServHitechs);
                    //các sereServ trong nhóm vật tư
                    var SERVICE_REPORT__MATERIAL_VTTT_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT;
                    var sereServVTTTs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT__MATERIAL_VTTT_ID && o.IS_OUT_PARENT_FEE != null).ToList();
                    var sereServVTTTADOs = PriceBHYTSereServAdoProcess(sereServVTTTs);
                    var sereServNotHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID != SERVICE_REPORT_ID__HIGHTECH).ToList();

                    var servicePatyPrpos = listServices;
                    //Cộng các sereServ trong gói vào dv ktc
                    foreach (var sereServHitech in sereServHitechADOs)
                    {
                        List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServVTTTInKtcADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                        var sereServVTTTInKtcs = sereServs.Where(o => o.PARENT_ID == sereServHitech.ID && o.IS_OUT_PARENT_FEE == null).ToList();
                        sereServVTTTInKtcADOs = PriceBHYTSereServAdoProcess(sereServVTTTInKtcs);
                        if (sereServHitech.PRICE_POLICY != 0)
                        {
                            var servicePatyPrpo = servicePatyPrpos.Where(o => o.ID == sereServHitech.SERVICE_ID && o.BILL_PATIENT_TYPE_ID == sereServHitech.PATIENT_TYPE_ID && o.PACKAGE_PRICE == sereServHitech.PRICE_POLICY).ToList();
                            if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                            {
                                sereServHitech.VIR_PRICE = sereServHitech.PRICE;
                            }
                        }
                        else
                            sereServHitech.VIR_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);

                        sereServHitech.VIR_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);
                        sereServHitech.VIR_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);

                        decimal totalHeinPrice = 0;
                        foreach (var sereServVTTTInKtcADO in sereServVTTTInKtcADOs)
                        {
                            totalHeinPrice += sereServVTTTInKtcADO.AMOUNT * sereServVTTTInKtcADO.PRICE_BHYT;
                        }
                        sereServHitech.PRICE_BHYT += totalHeinPrice;
                        sereServHitech.HEIN_LIMIT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.HEIN_LIMIT_PRICE);

                        sereServHitech.VIR_TOTAL_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                        sereServHitech.VIR_TOTAL_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        sereServHitech.VIR_TOTAL_PATIENT_PRICE = sereServHitech.VIR_TOTAL_PRICE - sereServHitech.VIR_TOTAL_HEIN_PRICE;
                        sereServHitech.SERVICE_UNIT_NAME = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == sereServHitech.TDL_SERVICE_UNIT_ID).SERVICE_UNIT_NAME;
                    }

                    //Lọc các sereServ không nằm trong dịch vụ ktc và vật tư thay thế
                    //
                    var sereServDeleteADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    foreach (var sereServVTTTADO in sereServVTTTADOs)
                    {
                        var sereServADODelete = sereServHitechADOs.Where(o => o.ID == sereServVTTTADO.PARENT_ID).ToList();
                        if (sereServADODelete.Count == 0)
                        {
                            sereServDeleteADOs.Add(sereServVTTTADO);
                        }
                    }

                    foreach (var sereServDelete in sereServDeleteADOs)
                    {
                        sereServVTTTADOs.Remove(sereServDelete);
                    }
                    var sereServVTTTIds = sereServVTTTADOs.Select(o => o.ID);
                    sereServNotHitechs = sereServNotHitechs.Where(o => !sereServVTTTIds.Contains(o.ID)).ToList();
                    var sereServNotHitechADOs = PriceBHYTSereServAdoProcess(sereServNotHitechs);
                    string ratio_text = "";
                    if (patientTypeAlter != null)
                    {
                        ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.LEVEL_CODE, patientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";
                    }
                    MPS.Processor.Mps000102.PDO.PatientADO patientAdo = new MPS.Processor.Mps000102.PDO.PatientADO(patientPrint);

                    if (sereServNotHitechADOs != null && sereServNotHitechADOs.Count > 0)
                    {
                        sereServNotHitechADOs = sereServNotHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    if (sereServHitechADOs != null && sereServHitechADOs.Count > 0)
                    {
                        sereServHitechADOs = sereServHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    if (sereServVTTTADOs != null && sereServVTTTADOs.Count > 0)
                    {
                        sereServVTTTADOs = sereServVTTTADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    V_HIS_SERVICE_REQ firsExamRoom = new V_HIS_SERVICE_REQ();
                    if (treatmentFees.TDL_FIRST_EXAM_ROOM_ID.HasValue)
                    {
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == treatmentFees.TDL_FIRST_EXAM_ROOM_ID);
                        if (room != null)
                        {
                            firsExamRoom.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                        }
                    }
                    MPS.Processor.Mps000102.PDO.Mps000102PDO mps000102RDO = new MPS.Processor.Mps000102.PDO.Mps000102PDO(
                            patientAdo,
                            patientTypeAlter,
                            departmentName,

                            sereServNotHitechADOs,
                            sereServHitechADOs,
                            sereServVTTTADOs,

                            null,//bản tin chuyển khoa, mps lấy ramdom thời gian vào khoa khi chỉ định tạm thời chưa cần
                            treatmentFees,

                            BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(),
                            transactionPrint,
                            ssDepositPrint,
                            totalDay,
                            ratio_text,
                            firsExamRoom
                            );
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatmentFees != null ? treatmentFees.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> PriceBHYTSereServAdoProcess(List<V_HIS_SERE_SERV> sereServs)
        {
            List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
            try
            {
                foreach (var item in sereServs)
                {
                    MPS.Processor.Mps000102.PDO.SereServGroupPlusADO sereServADO = new MPS.Processor.Mps000102.PDO.SereServGroupPlusADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>(sereServADO, item);

                    if (sereServADO.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
                    {
                        sereServADO.PRICE_BHYT = 0;
                    }
                    else
                    {
                        if (sereServADO.HEIN_LIMIT_PRICE != null && sereServADO.HEIN_LIMIT_PRICE > 0)
                            sereServADO.PRICE_BHYT = (item.HEIN_LIMIT_PRICE ?? 0);
                        else
                            sereServADO.PRICE_BHYT = item.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    }

                    sereServADOs.Add(sereServADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADOs;
        }
        private void DisposeData()
        {
            try
            {
                currentHisServiceReq = null;
                serviceReqRightClick = null;
                serviceReqs = null;
                sereServ7s = null;
                sereServ6s = null;
                currentPatientTypeAlter = null;
                ssTreeProcessor = null;
                p1 = p2 = p3 = p4 = p5 = p6 = p7 = p8 = p9 = p10 = p11 = p12 = p13 = p14 = p15 = p16 = null;
                ucTreeSereServ7.Dispose();
                u1.Dispose(); u2.Dispose(); u3.Dispose(); u4.Dispose(); u5.Dispose(); u6.Dispose(); u7.Dispose(); u8.Dispose(); u9.Dispose(); u10.Dispose(); u11.Dispose(); u12.Dispose(); u13.Dispose(); u14.Dispose(); u15.Dispose(); u16.Dispose();
                executeRoomPopupMenuProcessor = null;
                clienttManager = null;
                ServiceReqCurrentTreatment = null;
                SereServCurrentTreatment = null;
                selectedPatientTypeList = null;
                patientTypeList = null;
                serviceSelecteds = null;
                listServices = null;
                _sereServRowMenu = null;
                CheckServiceExecuteGroup = null;
                lstExecuteRoom = null;
                lstPatientPrioty = null;
                lstServiceRoom = null;
                lstPatientType = null;
                deskList = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
