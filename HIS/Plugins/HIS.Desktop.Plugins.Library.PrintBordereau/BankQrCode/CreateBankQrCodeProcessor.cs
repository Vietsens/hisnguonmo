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
using Inventec.Common.Adapter;
using Inventec.Common.BankQrCode;
using Inventec.Common.BankQrCode.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.BankQrCode
{
    class CreateBankQrCodeProcessor
    {
        List<HIS_SERE_SERV> SereServs { get; set; }
        V_HIS_TREATMENT Treatment { get; set; }
        string ConfigInfo;
        string ConnectConfigInfo;
        long RoomId;

        public CreateBankQrCodeProcessor(long roomId, List<HIS_SERE_SERV> _sereServs, V_HIS_TREATMENT _treatment)
        {
            this.SereServs = _sereServs;
            this.Treatment = _treatment;
            this.ConfigInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.QrCodeBillInfoCFG);
            this.ConnectConfigInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.QRCodeConnectInfoCFG);
            this.RoomId = roomId;
        }

        public HIS_TRANS_REQ Create()
        {
            HIS_TRANS_REQ result = null;
            try
            {
                if (this.SereServs == null || this.Treatment == null)
                {
                    return result;
                }

                decimal totalPriceCheck = Math.Round(this.SereServs.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0), 0, MidpointRounding.AwayFromZero);

                HIS_TRANS_REQ oldReq = null;
                List<HIS_SESE_TRANS_REQ> apiSeseResult = null;
                CommonParam param = new CommonParam();
                HisTransReqFilter filter = new HisTransReqFilter();
                filter.TREATMENT_ID = this.Treatment.ID;
                filter.IS_CANCEL = false;
                List<HIS_TRANS_REQ> listTransReq = new BackendAdapter(param).Get<List<HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumers.MosConsumer, filter, param);
                if (listTransReq != null && listTransReq.Count > 0)
                {
                    //kiểm tra yêu cầu đã được tạo hay chưa
                    listTransReq = listTransReq.Where(o => o.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__REQUEST).ToList();

                    if (listTransReq != null && listTransReq.Count > 0)
                    {
                        HisSeseTransReqFilter seseTransReqFilter = new HisSeseTransReqFilter();
                        seseTransReqFilter.TRANS_REQ_IDs = listTransReq.Select(s => s.ID).ToList();
                        apiSeseResult = new BackendAdapter(param).Get<List<HIS_SESE_TRANS_REQ>>("api/HisSeseTransReq/Get", ApiConsumers.MosConsumer, seseTransReqFilter, param);

                        foreach (var item in listTransReq)
                        {
                            if (apiSeseResult != null && apiSeseResult.Count > 0)
                            {
                                var ssReq = apiSeseResult.Where(o => o.TRANS_REQ_ID == item.ID).ToList();
                                if (ssReq != null && ssReq.Count > 0 && this.SereServs.Count == ssReq.Count
                                    && this.SereServs.Exists(e => ssReq.Select(s => s.SERE_SERV_ID).Contains(e.ID))
                                    && ssReq.Exists(e => this.SereServs.Select(s => s.ID).Contains(e.SERE_SERV_ID)))
                                {
                                    oldReq = item;
                                    break;
                                }
                            }
                        }
                    }
                }

                //Nếu có dữ liệu yêu cầu thanh toán ứng với số tiền thanh toán thì kiểm tra mã QR
                //Nếu chưa có thì tạo. Nếu có thì trả lại.
                if (oldReq != null && !String.IsNullOrWhiteSpace(oldReq.BANK_QR_CODE))
                {
                    result = oldReq;
                }
                else
                {
                    result = ProcessCreateQR(oldReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private HIS_TRANS_REQ ProcessCreateQR(HIS_TRANS_REQ oldReq)
        {
            HIS_TRANS_REQ result = null;
            try
            {
                //tạo mới nếu chưa có
                if (oldReq == null || oldReq.ID <= 0)
                {
                    oldReq = CreateTransReq();
                }

                if (oldReq != null && oldReq.AMOUNT > 0 && !String.IsNullOrWhiteSpace(oldReq.TRANS_REQ_CODE))
                {
                    BankQrCodeInputADO inputData = new BankQrCodeInputADO();
                    inputData.Amount = oldReq.AMOUNT;
                    inputData.TransactionCode = oldReq.TRANS_REQ_CODE;
                    inputData.SystemConfig = this.ConnectConfigInfo;

                    BankQrCodeProcessor bankQrCode = new BankQrCodeProcessor(inputData);
                    ResultQrCode qrData = bankQrCode.GetQrCode(ProvinceType.BIDV);
                    if (qrData != null && String.IsNullOrWhiteSpace(qrData.Message))
                    {
                        oldReq.BANK_QR_CODE = qrData.Data;
                        CreateThreadUpdateTransReq(oldReq);
                        result = oldReq;
                    }
                    else if (qrData != null)
                    {
                        XtraMessageBox.Show(String.Format("Tạo mã QR thanh toán thất bại. {0}", qrData.Message));
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Tạo yêu cầu thanh toán thất bại");
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => oldReq), oldReq));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private HIS_TRANS_REQ CreateTransReq()
        {
            HIS_TRANS_REQ result = null;
            HIS_CASHIER_ROOM cashierRoom = null;
            long accountBookId = 0;
            string cashierLoginname = "";
            string cashierUsername = "";

            if (CheckConfig(ref cashierRoom, ref accountBookId, ref cashierLoginname, ref cashierUsername))
            {
                result = new HIS_TRANS_REQ();

                HisTransReqBillSDO TransReqBillSDO = this.MapTranToReq(this.SereServs, this.Treatment, this.RoomId, cashierRoom, accountBookId, cashierLoginname, cashierUsername);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => TransReqBillSDO), TransReqBillSDO));
                CommonParam param = new CommonParam();
                result = new BackendAdapter(param).Post<HIS_TRANS_REQ>("api/HisTransReq/CreateBill", ApiConsumers.MosConsumer, TransReqBillSDO, param);
            }
            return result;
        }

        private bool CheckConfig(ref HIS_CASHIER_ROOM cashierRoom, ref long accountBookId, ref string cashierLoginname, ref string cashierUsername)
        {
            if (String.IsNullOrWhiteSpace(this.ConfigInfo))
            {
                XtraMessageBox.Show("Chưa thiết lập cấu hình hệ thống tạo yêu cầu thanh toán.");
                return false;
            }

            string[] cfArrr = this.ConfigInfo.Split('|');
            if (cfArrr.Length < 3)
            {
                XtraMessageBox.Show("Cấu hình hệ thống tạo yêu cầu thanh toán thiết lập sai giá trị vui lòng kiểm tra lại.");
                return false;
            }

            cashierRoom = BackendDataWorker.Get<HIS_CASHIER_ROOM>().FirstOrDefault(o => o.CASHIER_ROOM_CODE.ToLower() == cfArrr[0].Trim().ToLower());
            if (cashierRoom == null)
            {
                XtraMessageBox.Show("Cấu hình hệ thống tạo yêu cầu thanh toán không có thông tin phòng thu ngân.");
                return false;
            }

            string[] cashier = cfArrr[1].Split('-');
            if (cashier == null || cashier.Length < 2)
            {
                XtraMessageBox.Show("Cấu hình hệ thống tạo yêu cầu thanh toán không có thông tin thu ngân.");
                return false;
            }

            cashierLoginname = cashier[0].Trim();
            cashierUsername = cashier[1].Trim();

            HIS_ACCOUNT_BOOK accountBook = BackendDataWorker.Get<HIS_ACCOUNT_BOOK>().FirstOrDefault(o => o.ACCOUNT_BOOK_CODE.ToLower() == cfArrr[2].Trim().ToLower());
            if (accountBook == null)
            {
                XtraMessageBox.Show("Cấu hình hệ thống tạo yêu cầu thanh toán không có thông tin sổ biên lai/hóa đơn.");
                return false;
            }

            accountBookId = accountBook.ID;

            return true;
        }

        private HisTransReqBillSDO MapTranToReq(List<HIS_SERE_SERV> _sereServs, V_HIS_TREATMENT _treatment, long roomId, HIS_CASHIER_ROOM room, long accoutBookId, string loginname, string userName)
        {
            HisTransReqBillSDO req = new HisTransReqBillSDO();
            decimal amount = 0;
            if (_sereServs != null && _sereServs.Count > 0)
            {
                amount = _sereServs.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
            }

            req.PayAmount = amount;
            req.RequestRoomId = roomId;
            req.TransReq = new HIS_TRANS_REQ();
            req.TransReq.ACCOUNT_BOOK_ID = accoutBookId;

            req.TransReq.CASHIER_ROOM_ID = room.ID;
            req.TransReq.CASHIER_LOGINNAME = loginname;
            req.TransReq.CASHIER_USERNAME = userName;

            req.TransReq.AMOUNT = amount;

            if (_treatment != null)
            {
                req.TransReq.BUYER_ACCOUNT_NUMBER = _treatment.TDL_PATIENT_ACCOUNT_NUMBER;
                req.TransReq.BUYER_ADDRESS = _treatment.TDL_PATIENT_ADDRESS;
                req.TransReq.BUYER_NAME = _treatment.TDL_PATIENT_NAME;
                req.TransReq.BUYER_ORGANIZATION = _treatment.TDL_PATIENT_WORK_PLACE;
                req.TransReq.BUYER_TAX_CODE = _treatment.TDL_PATIENT_TAX_CODE;
                req.TransReq.TREATMENT_ID = _treatment.ID;
            }

            DateTime today = DateTime.Now;
            DateTime? newday = today.AddDays(7);
            long? TransactionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(newday);

            req.TransReq.EXPIRY_TIME = TransactionTime.Value;
            req.TransReq.PAY_FORM_ID = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK;

            if (_sereServs != null && _sereServs.Count > 0)
                req.SeseTransReqs = (from r in _sereServs select new HIS_SESE_TRANS_REQ() { SERE_SERV_ID = r.ID, PRICE = r.VIR_TOTAL_PATIENT_PRICE ?? 0 }).ToList();

            return req;
        }

        private void CreateThreadUpdateTransReq(HIS_TRANS_REQ oldReq)
        {
            Thread transReq = new Thread(UpdateQrCode);
            try
            {
                transReq.Start(oldReq);
            }
            catch (Exception ex)
            {
                transReq.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateQrCode(object data)
        {
            try
            {
                if (data != null && data.GetType() == typeof(HIS_TRANS_REQ))
                {
                    HIS_TRANS_REQ update = data as HIS_TRANS_REQ;
                    HisTransReqBankInfoSDO sdo = new HisTransReqBankInfoSDO();
                    sdo.TransReqId = update.ID;
                    sdo.BankQrCode = update.BANK_QR_CODE;
                    CommonParam param = new CommonParam();
                    var apiResult = new BackendAdapter(param).Post<HIS_TRANS_REQ>("api/HisTransReq/UpdateBankInfo", ApiConsumers.MosConsumer, sdo, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
