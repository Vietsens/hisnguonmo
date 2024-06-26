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
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000430.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000430
{
    public class Mps000430Processor : AbstractProcessor
    {
        Mps000430PDO rdo;

        internal List<SererServADO> LstSererServADO = new List<SererServADO>();
        internal List<SererServADO> LstSsExecuteRoomADO = new List<SererServADO>();
        internal List<SererServADO> LstSsServiceReqADO = new List<SererServADO>();
        internal List<SererServADO> LstSsServiceTypeADO = new List<SererServADO>();

        public Mps000430Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000430PDO)rdoBase;
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                SetBarcodeKey();

                SetSingleKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);

                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "SereServ", this.LstSererServADO);
                objectTag.AddObjectData(store, "ExecuteRoom", this.LstSsExecuteRoomADO);
                objectTag.AddObjectData(store, "ServiceType", this.LstSsServiceTypeADO);
                objectTag.AddObjectData(store, "ServiceReq", this.LstSsServiceReqADO);

                objectTag.AddRelationship(store, "ServiceType", "SereServ", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceType", "ExecuteRoom", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ExecuteRoom", "SereServ", "TDL_EXECUTE_ROOM_ID", "TDL_EXECUTE_ROOM_ID");

                objectTag.AddRelationship(store, "ServiceType", "ServiceReq", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceReq", "SereServ", "SERVICE_REQ_ID", "SERVICE_REQ_ID");

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                LstSererServADO = new List<SererServADO>();

                var lstSereServDeposi = new List<HIS_SERE_SERV_DEPOSIT>();
                var HisSereServ = new List<HIS_SERE_SERV>();

                List<long> lstSereServId = new List<long>();
                List<long> lstSereServIdFinal = new List<long>();
                List<long> lstSereServDepositId = new List<long>();
                decimal Amount = 0;

                if (rdo._SeseDepoRepay != null && rdo._SeseDepoRepay.Count > 0)
                {
                    lstSereServDepositId = rdo._SeseDepoRepay.Select(o => o.SERE_SERV_DEPOSIT_ID).Distinct().ToList();
                }

                if (rdo._SereServDeposit != null && rdo._SereServDeposit.Count > 0)
                {
                    List<long> depositIds = new List<long>();
                    var listDpRp = rdo._SereServDeposit.Where(o => lstSereServDepositId.Contains(o.ID)).ToList();
                    if (listDpRp != null && listDpRp.Count > 0)
                    {
                        depositIds = listDpRp.Select(s => s.DEPOSIT_ID).Distinct().ToList();
                    }

                    lstSereServDeposi = rdo._SereServDeposit.Where(o => depositIds.Contains(o.DEPOSIT_ID) && !lstSereServDepositId.Contains(o.ID)).ToList();
                }

                if (lstSereServDeposi != null && lstSereServDeposi.Count > 0)
                {
                    lstSereServIdFinal = lstSereServDeposi.Select(o => o.SERE_SERV_ID).Distinct().ToList();
                }

                if (lstSereServIdFinal != null && lstSereServIdFinal.Count > 0 && rdo._SereServ != null && rdo._SereServ.Count > 0)
                {
                    lstSereServIdFinal = lstSereServIdFinal.Distinct().ToList();
                    HisSereServ = rdo._SereServ.Where(o => lstSereServIdFinal.Contains(o.ID)).ToList();
                }

                if (HisSereServ != null && HisSereServ.Count > 0)
                {
                    foreach (var item in HisSereServ)
                    {
                        var checkSereServDeposit = rdo._SereServDeposit != null && rdo._SereServDeposit.Count > 0 ? rdo._SereServDeposit.FirstOrDefault(o => o.SERE_SERV_ID == item.ID) : new HIS_SERE_SERV_DEPOSIT();
                        var checkSereServBill = rdo._SereServBill != null && rdo._SereServBill.Count > 0 ? rdo._SereServBill.FirstOrDefault(o => o.SERE_SERV_ID == item.ID) : new HIS_SERE_SERV_BILL();

                        if (checkSereServDeposit != null)
                        {
                            if (checkSereServBill != null)
                            {
                                Amount += checkSereServDeposit.AMOUNT;
                                if (checkSereServBill.HIS_TRANSACTION != null && (checkSereServBill.HIS_TRANSACTION.KC_AMOUNT == null || checkSereServBill.HIS_TRANSACTION.KC_AMOUNT == 0))
                                {
                                    Amount += checkSereServBill.PRICE;
                                }
                            }
                            else
                            {
                                Amount += checkSereServDeposit.AMOUNT;
                            }
                        }
                        else
                        {
                            if (checkSereServBill != null)
                            {
                                Amount += checkSereServBill.PRICE;
                            }
                        }

                        SererServADO SererServADO = new Mps000430.SererServADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<SererServADO>(SererServADO, item);

                        var serviceType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == item.TDL_SERVICE_TYPE_ID);
                        if (serviceType != null)
                        {
                            SererServADO.SERVICE_TYPE_CODE = serviceType.SERVICE_TYPE_CODE;
                            SererServADO.SERVICE_TYPE_NAME = serviceType.SERVICE_TYPE_NAME;
                            SererServADO.SERVICE_TYPE_NUM_ORDER = serviceType.NUM_ORDER;
                        }

                        V_HIS_SERVICE service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (service != null && service.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                        {
                            SererServADO.SERVICE_NUM_ORDER = service.NUM_ORDER;
                            if (service.PARENT_ID.HasValue)
                            {
                                V_HIS_SERVICE parent = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == service.PARENT_ID.Value);
                                if (parent != null)
                                {
                                    SererServADO.SERVICE_PARENT_NUM_ORDER = parent.NUM_ORDER ?? -1;
                                }
                            }
                        }
                        else
                        {
                            SererServADO.SERVICE_NUM_ORDER = -1;
                            SererServADO.SERVICE_PARENT_NUM_ORDER = -1;
                        }

                        if (rdo.ListHeinServiceType != null && rdo.ListHeinServiceType.Count > 0)
                        {
                            var HeinServiceType = rdo.ListHeinServiceType.FirstOrDefault(o => o.ID == item.TDL_HEIN_SERVICE_TYPE_ID);
                            if (HeinServiceType != null)
                            {
                                SererServADO.NUM_ORDER = HeinServiceType.NUM_ORDER;
                                SererServADO.VIR_PARENT_NUM_ORDER = HeinServiceType.VIR_PARENT_NUM_ORDER;
                                SererServADO.HEIN_SERVICE_TYPE_NAME = HeinServiceType.HEIN_SERVICE_TYPE_NAME;
                            }
                        }

                        if (rdo.ListRoom != null && rdo.ListRoom.Count > 0)
                        {
                            var executeRoom = rdo.ListRoom.FirstOrDefault(o => o.ID == item.TDL_EXECUTE_ROOM_ID);
                            if (executeRoom != null)
                            {
                                SererServADO.EXECUTE_ROOM_CODE = executeRoom.ROOM_CODE;
                                SererServADO.EXECUTE_ROOM_NAME = executeRoom.ROOM_NAME;
                            }

                            var requestRoom = rdo.ListRoom.FirstOrDefault(o => o.ID == item.TDL_REQUEST_ROOM_ID);
                            if (requestRoom != null)
                            {
                                SererServADO.REQUEST_ROOM_CODE = requestRoom.ROOM_CODE;
                                SererServADO.REQUEST_ROOM_NAME = requestRoom.ROOM_NAME;
                            }
                        }

                        LstSererServADO.Add(SererServADO);
                    }
                }

                string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(Amount));
                string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountStr);
                SetSingleKey(new KeyValue(Mps000430ExtendSingleKey.AMOUNT, Amount));
                SetSingleKey(new KeyValue(Mps000430ExtendSingleKey.AMOUNT_TEXT, amountText));
                SetSingleKey(new KeyValue(Mps000430ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountText)));

                LstSererServADO = LstSererServADO.OrderBy(o => o.SERVICE_TYPE_NUM_ORDER ?? 9999).ThenBy(o => o.TDL_SERVICE_NAME).ThenBy(o => o.ID).ToList();
                List<SererServADO> SereServTotal = new List<SererServADO>();

                var groupSererServ = LstSererServADO.GroupBy(o => new { o.TDL_EXECUTE_ROOM_ID, o.TDL_SERVICE_TYPE_ID }).ToList();
                foreach (var item in groupSererServ)
                {
                    SererServADO SererServADO = new SererServADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<SererServADO>(SererServADO, item.First());

                    LstSsExecuteRoomADO.Add(SererServADO);
                }

                var serviceReqs = LstSererServADO.GroupBy(o => new { o.SERVICE_REQ_ID, o.TDL_SERVICE_TYPE_ID }).ToList();
                var serviceTypes = serviceReqs.GroupBy(o => o.Key.TDL_SERVICE_TYPE_ID).ToList();

                long countType = 1;
                long count = 1;
                foreach (var sType in serviceTypes)
                {
                    SererServADO SererServADO = new SererServADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<SererServADO>(SererServADO, sType.First().First());
                    SererServADO.STT = countType;
                    LstSsServiceTypeADO.Add(SererServADO);
                    countType++;

                    var reqs = serviceReqs.Where(o => o.Key.TDL_SERVICE_TYPE_ID == sType.Key).ToList();
                    if (sType.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)//sắp xếp lại với xét nghiệm
                    {
                        reqs = reqs.OrderBy(o => o.First().SERVICE_TYPE_NUM_ORDER ?? 9999).ThenBy(o => o.First().TDL_EXECUTE_DEPARTMENT_ID).ToList();
                    }

                    foreach (var req in reqs)
                    {
                        SererServADO reqADO = new SererServADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<SererServADO>(reqADO, req.First());
                        reqADO.STT = count;
                        LstSsServiceReqADO.Add(reqADO);
                        count++;

                        //sắp xếp lại với xét nghiệm
                        if (req.Key.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                        {
                            SereServTotal.AddRange(req.OrderByDescending(o => o.SERVICE_PARENT_NUM_ORDER).ThenByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.ID).ToList());
                        }
                        else
                        {
                            SereServTotal.AddRange(req);
                        }
                    }
                }

                if (LstSererServADO.Count == SereServTotal.Count)
                {
                    LstSererServADO = SereServTotal;
                }

                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo._Treatment, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatientTypeAlter, false);
            }
            catch (Exception ex)
            {
                LstSererServADO = new List<SererServADO>();
                LstSsExecuteRoomADO = new List<SererServADO>();
                LstSsServiceTypeADO = new List<SererServADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo._Treatment != null)
                {
                    if (!String.IsNullOrEmpty(rdo._Treatment.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo._Treatment.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000430ExtendSingleKey.BARCODE_TREATMENT_CODE, barcodeTreatment);
                    }

                    if (!string.IsNullOrWhiteSpace(rdo._Treatment.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo._Treatment.TDL_PATIENT_CODE);
                        barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatient.IncludeLabel = false;
                        barcodePatient.Width = 120;
                        barcodePatient.Height = 40;
                        barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatient.IncludeLabel = true;

                        dicImage.Add(Mps000430ExtendSingleKey.BARCODE_PATIENT_CODE, barcodePatient);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = "Mã điều trị: " + rdo._Treatment.TREATMENT_CODE;
                log += " , Mã bệnh nhân: " + rdo._Treatment.TDL_PATIENT_CODE;
                log += "";
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                string treatmentCode = "TREATMENT_CODE:" + rdo._Treatment.TREATMENT_CODE;
                string transactionDepositCode = "TRANSACTION_CODE:";
                string transactionRepayCode = "TRANSACTION_CODE:";

                if (this.rdo._Transaction != null && this.rdo._Transaction.Count > 0)
                {
                    var depositTran = this.rdo._Transaction.FirstOrDefault(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU);
                    if (depositTran != null)
                    {
                        transactionDepositCode += depositTran.TRANSACTION_CODE;
                    }

                    var reapayTran = this.rdo._Transaction.FirstOrDefault(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU);
                    if (reapayTran != null)
                    {
                        transactionRepayCode += reapayTran.TRANSACTION_CODE;
                    }
                }

                string countSereServ = this.LstSererServADO.Count.ToString();
                //string serviceReqCode = "SERVICE_REQ_CODE:" + rdo._Treatment.TDL_PATIENT_CODE;

                if (rdo != null && rdo._Treatment != null)
                    result = String.Format("{0} {1} {2} {3} {4}", this.printTypeCode, treatmentCode, transactionRepayCode, transactionDepositCode, "CountSereServ:" + countSereServ);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
