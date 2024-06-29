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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000111.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Number;
using System.Drawing;
namespace MPS.Processor.Mps000111
{
    public class Mps000111Processor : AbstractProcessor
    {
        Mps000111PDO rdo;
        List<SereServADO> _SereServADOs { get; set; }
        List<ServiceTypeADO> listType = new List<ServiceTypeADO>();
        List<ServiceTypeADO> listgroupType = new List<ServiceTypeADO>();

        List<ServiceTypeADO> listTypeSvt = new List<ServiceTypeADO>();
        public Mps000111Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000111PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetBarcodeKey();
                SetSingleKey();
                ProcessSereServADO();
                ProcessGroupData();

                var data = rdo._ListTranSaction.FirstOrDefault(o => o.ID == (rdo._Transaction.RELATE_TRANSACTION_ID ?? 0));
                if(data != null)
                {
                    AddObjectKeyIntoListkeyWithPrefix<V_HIS_TRANSACTION>(data, "RELATE_", true);
                }

                objectTag.AddObjectData(store, "SereServ", _SereServADOs ?? new List<SereServADO>());
                objectTag.AddObjectData(store, "ServiceType", listType);
                objectTag.AddObjectData(store, "ServiceTypeGroup", listgroupType);
                objectTag.AddRelationship(store, "ServiceTypeGroup", "ServiceType", "TYPE", "TYPE");

                objectTag.AddObjectData(store, "ListTransaction", rdo._ListTranSaction != null && rdo._ListTranSaction.Count > 0 ? rdo._ListTranSaction.Where(o => o.IS_CANCEL != 1 && o.SALE_TYPE_ID == null).ToList() : new List<V_HIS_TRANSACTION>());
                objectTag.AddObjectData(store, "SSGroupByType", listTypeSvt);
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo._Transaction != null)
                {

                    if (!String.IsNullOrEmpty(rdo._Transaction.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo._Transaction.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000111ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);

                        //string treatmentCode = rdo._Transaction.TREATMENT_CODE.Substring(rdo._Transaction.TREATMENT_CODE.Length - 5, 5);
                        //Inventec.Common.BarcodeLib.Barcode barcodeTreatment5 = new Inventec.Common.BarcodeLib.Barcode(treatmentCode);
                        //barcodeTreatment5.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        //barcodeTreatment5.IncludeLabel = false;
                        //barcodeTreatment5.Width = 120;
                        //barcodeTreatment5.Height = 40;
                        //barcodeTreatment5.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        //barcodeTreatment5.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        //barcodeTreatment5.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        //barcodeTreatment5.IncludeLabel = true;

                        //dicImage.Add(Mps000111ExtendSingleKey.TREATMENT_CODE_BAR_5, barcodeTreatment5);
                    }

                    if (!string.IsNullOrWhiteSpace(rdo._Transaction.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo._Transaction.TDL_PATIENT_CODE);
                        barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatient.IncludeLabel = false;
                        barcodePatient.Width = 120;
                        barcodePatient.Height = 40;
                        barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatient.IncludeLabel = true;

                        dicImage.Add(Mps000111ExtendSingleKey.PATIENT_CODE_BAR, barcodePatient);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSereServADO()
        {
            try
            {
                _SereServADOs = new List<SereServADO>();
                if (rdo._ListSereServ != null && rdo._ListSereServ.Count > 0)
                {
                    var dataSereServ = rdo._ListSereServ
                        //.Where(o => o.PATIENT_TYPE_ID == rdo._PatientTypeId)
                        .ToList();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataSereServ), dataSereServ));
                    if (dataSereServ != null && dataSereServ.Count > 0)
                    {
                        foreach (var item in dataSereServ)
                        {
                            var dataPatientTypeName = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.ID == item.PATIENT_TYPE_ID).FirstOrDefault().PATIENT_TYPE_NAME;
                            SereServADO ado = new SereServADO(item);
                            ado.PATIENT_TYPE_NAME = dataPatientTypeName;
                            _SereServADOs.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                decimal totalRepay = 0;
                decimal totalDepositService = 0;
                decimal totalDeposit = 0;
                long? repayNumOrder = null;
                if (rdo._ListTranSaction != null && rdo._ListTranSaction.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._ListTranSaction), rdo._ListTranSaction));
                    if (rdo._ListSeseDepoRepay != null && rdo._ListSeseDepoRepay.Count > 0)
                    {
                        var idRepay = rdo._ListSeseDepoRepay.Select(o => o.REPAY_ID).ToList();
                        totalRepay = rdo._ListTranSaction.Where(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU
                           && o.IS_CANCEL != 1 && !idRepay.Contains(o.ID)
                           ).ToList().Sum(s => s.AMOUNT);

                        var idDepoSere = rdo._ListSeseDepoRepay.Select(o => o.SERE_SERV_DEPOSIT_ID).ToList();
                        totalDepositService = rdo._ListSereServDeposit.Where(o => o.IS_CANCEL != 1 && (!idDepoSere.Contains(o.ID)
                           || (rdo._ListSeseDepoRepay.Exists(p=>p.SERE_SERV_DEPOSIT_ID == o.ID && p.IS_CANCEL == 1)))).ToList().Sum(s => s.AMOUNT);
                    }
                    else
                    {
                        totalRepay = rdo._ListTranSaction.Where(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU
                           && o.IS_CANCEL != 1
                           ).ToList().Sum(s => s.AMOUNT);

                        totalDepositService = rdo._ListSereServDeposit.Where(o => (o.IS_CANCEL != 1)
                           ).ToList().Sum(s => s.AMOUNT);
                    }
                    var listRepayNumOrder = rdo._ListTranSaction.Where(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU
                       && o.IS_CANCEL != 1 && (o.TDL_SESE_DEPO_REPAY_COUNT == 0 || o.TDL_SESE_DEPO_REPAY_COUNT == null) && o.TRANSACTION_TIME >= rdo._Transaction.TRANSACTION_TIME).ToList();
                    if (listRepayNumOrder != null && listRepayNumOrder.Count() > 0)
                    {
                        repayNumOrder = listRepayNumOrder.OrderBy(o => o.TRANSACTION_TIME).First().NUM_ORDER;
                    }
                    if (rdo._ListSereServDeposit != null && rdo._ListSereServDeposit.Count > 0)
                    {
                        var idDepo = rdo._ListSereServDeposit.Select(o => o.DEPOSIT_ID).ToList();
                        totalDeposit = rdo._ListTranSaction.Where(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU
                                 && o.IS_CANCEL != 1 && !idDepo.Contains(o.ID)
                                 ).ToList().Sum(s => s.AMOUNT);
                    }
                    else
                    {
                        totalDeposit = rdo._ListTranSaction.Where(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU
                                                        && o.IS_CANCEL != 1).ToList().Sum(s => s.AMOUNT);
                    }
                }

                SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.TOTAL_REPAY_AMOUNT, totalRepay));
                SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, totalDeposit));
                SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.TOTAL_DEPOSIT_SERVICE_AMOUNT, totalDepositService));
                SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.REPAY_NUM_ORDER, repayNumOrder));
                if (rdo._Transaction != null)
                {
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    string temp = rdo._Transaction.TDL_PATIENT_DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_NUM, rdo._Transaction.AMOUNT));
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToString(rdo._Transaction.AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                    //string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT));

                    string amountText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo._Transaction.AMOUNT, 0);
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_TEXT, rdo._Transaction.AMOUNT));
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, amountText));
                    decimal amountAfterExem = rdo._Transaction.AMOUNT - (rdo._Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToString(amountAfterExem, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_NUM, amountAfterExem));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.Number.Convert.NumberToStringRoundAuto(amountAfterExem, 4);
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, amountAfterExemText));
                    if (rdo._Transaction.AMOUNT > 0)
                    {
                        decimal ratio = ((rdo._Transaction.EXEMPTION ?? 0) * 100) / rdo._Transaction.AMOUNT;
                        SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));
                    }
                    //Ket Chuyen, Can Thu
                    if (rdo._Transaction.KC_AMOUNT.HasValue)
                    {
                        string kcAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.KC_AMOUNT.Value));
                        SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.KC_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo._Transaction.KC_AMOUNT.Value, 4)));
                    }

                    decimal canthu = rdo._Transaction.AMOUNT - (rdo._Transaction.KC_AMOUNT ?? 0) - (rdo._Transaction.EXEMPTION ?? 0);
                    if (rdo._ListBillFund != null && rdo._ListBillFund.Count > 0)
                    {
                        canthu = canthu - rdo._ListBillFund.Sum(s => s.AMOUNT);
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu));
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu)));
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.Number.Convert.NumberToStringRoundAuto(canthu, 4)));

                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._Transaction.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false);
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.DESCRIPTION, rdo._Transaction.DESCRIPTION));
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.EXEMPTION_REASON, rdo._Transaction.EXEMPTION_REASON));
                }

                string name = "";
                string result = "";
                string transDescriptionNoDeposit = "";
                string transDescriptionV2 = "";

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._ListSereServ), rdo._ListSereServ));
                if (rdo._ListSereServ != null && rdo._ListSereServ.Count > 0)
                {
                    var totalBhyt = rdo._ListSereServ.Where(o => o.PATIENT_TYPE_ID == rdo._PatientTypeId).ToList().Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    var totalNd = rdo._ListSereServ.Where(o => o.PATIENT_TYPE_ID != rdo._PatientTypeId).ToList().Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.TOTAL_AMOUNT_BHYT, totalBhyt));
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.TOTAL_AMOUNT_ND, totalNd));

                    string totalBhytText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalBhyt));
                    string totalNdText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalNd));
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.TOTAL_AMOUNT_BHYT_UPPER_TEXT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalBhyt, 4)));
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.TOTAL_AMOUNT_ND_UPPER_TEXT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalNd, 4)));

                    decimal totalPrice = rdo._ListSereServ.Sum(o => o.VIR_TOTAL_PRICE ?? 0);
                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToString(totalPrice, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));

                    if (rdo._ListSereServ.Count == 1)
                    {
                        var reqRoomID = rdo._ListSereServ.First().TDL_REQUEST_ROOM_ID;
                        var excuteRoomId = rdo._ListSereServ.FirstOrDefault().TDL_EXECUTE_ROOM_ID;
                        var serviceId = rdo._ListSereServ.FirstOrDefault().SERVICE_ID;
                        var examExcuteRoomName = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == excuteRoomId).FirstOrDefault().ROOM_NAME;
                        var examExcuteRoomCode = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == excuteRoomId).FirstOrDefault().ROOM_CODE;
                        var description = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == serviceId).FirstOrDefault().DESCRIPTION;
                        SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.EXAM_EXECUTE_ROOM_CODE, examExcuteRoomCode));
                        SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.EXAM_EXECUTE_ROOM_NAME, examExcuteRoomName));
                        SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.EXAM_SERVICE_DESCRIPTION, description));

                        var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().First(o => o.ID == reqRoomID);
                        if (reqRoom != null)
                        {
                            SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.REQUEST_ROOM_CODE, reqRoom.ROOM_CODE));
                            SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.REQUEST_ROOM_NAME, reqRoom.ROOM_NAME));
                        }
                        var serviceTypes = BackendDataWorker.Get<HIS_SERVICE_TYPE>().ToList();
                        if (serviceTypes != null && serviceTypes.Count > 0)
                        {
                            name = serviceTypes.FirstOrDefault(o => o.ID == rdo._ListSereServ.FirstOrDefault().TDL_SERVICE_TYPE_ID).SERVICE_TYPE_NAME;
                        }
                        SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.TRANS_DESCRIPTION, name + "[" + Inventec.Common.Number.Convert.NumberToString((decimal)rdo._ListSereServ.First().VIR_TOTAL_PATIENT_PRICE) + "]"));
                    }

                    if (rdo._ListSereServ.Count > 1)
                    {
                        long excuteRoomId = 0;
                        long serviceId = 0;
                        var reqRoomID = rdo._ListSereServ.First().TDL_REQUEST_ROOM_ID;
                        var excuteRoomIds = rdo._ListSereServ.Where(p => p.TDL_SERVICE_TYPE_ID == 1).OrderByDescending(o => o.TDL_INTRUCTION_TIME).ToList();
                        if (excuteRoomIds != null && excuteRoomIds.Count > 0)
                        {
                            excuteRoomId = excuteRoomIds.FirstOrDefault().TDL_EXECUTE_ROOM_ID;
                            serviceId = excuteRoomIds.FirstOrDefault().SERVICE_ID;
                            var examExcuteRoomName = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == excuteRoomId).FirstOrDefault().ROOM_NAME;
                            var examExcuteRoomCode = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == excuteRoomId).FirstOrDefault().ROOM_CODE;
                            var description = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == serviceId).FirstOrDefault().DESCRIPTION;
                            SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.EXAM_EXECUTE_ROOM_CODE, examExcuteRoomCode));
                            SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.EXAM_EXECUTE_ROOM_NAME, examExcuteRoomName));
                            SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.EXAM_SERVICE_DESCRIPTION, description));
                        }
                        var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().First(o => o.ID == reqRoomID);
                        if (reqRoom != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("ROOM_CODE" + reqRoom.ROOM_CODE);
                            Inventec.Common.Logging.LogSystem.Debug("ROOM_NAME" + reqRoom.ROOM_NAME);
                            SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.REQUEST_ROOM_CODE, reqRoom.ROOM_CODE));
                        }

                        var groups = rdo._ListSereServ.OrderBy(p => p.TDL_SERVICE_TYPE_ID).GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();

                        var serviceTypes = BackendDataWorker.Get<HIS_SERVICE_TYPE>().ToList();
                        List<string> temp = new List<string>();
                        if (groups != null && groups.Count > 0)
                        {
                            foreach (var itemgroup in groups)
                            {
                                decimal total = 0;
                                foreach (var item in itemgroup)
                                {
                                    if (item.VIR_TOTAL_PATIENT_PRICE.HasValue)
                                    {
                                        total = total + item.VIR_TOTAL_PATIENT_PRICE.Value;
                                    }
                                }

                                if (serviceTypes != null && serviceTypes.Count > 0)
                                {
                                    name = serviceTypes.FirstOrDefault(o => o.ID == itemgroup.FirstOrDefault().TDL_SERVICE_TYPE_ID).SERVICE_TYPE_NAME;
                                }

                                temp.Add(name + "[" + Inventec.Common.Number.Convert.NumberToString(total) + "]");
                            }
                        }

                        result = String.Join(", ", temp);
                        Inventec.Common.Logging.LogSystem.Debug("result" + result);
                    }

                    List<HIS_SERE_SERV> listSereServDetail = new List<HIS_SERE_SERV>();

                    // Loc cac dich vu theo dich vu da tam ung de lay duoc cac dich vu chua tam ung
                    if (rdo._ListSereServDeposit != null && rdo._ListSereServDeposit.Count > 0)
                    {
                        listSereServDetail = rdo._ListSereServ.Where(o => rdo._ListSereServDeposit.All(k => k.SERE_SERV_ID != o.ID)).ToList();
                    }
                    else
                    {
                        listSereServDetail = rdo._ListSereServ;
                    }

                    if (listSereServDetail != null && listSereServDetail.Count > 0)
                    {
                        var groups = listSereServDetail.OrderBy(p => p.TDL_SERVICE_TYPE_ID).GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                        var serviceTypes = BackendDataWorker.Get<HIS_SERVICE_TYPE>().ToList();
                        List<string> temp = new List<string>();
                        if (groups != null && groups.Count > 0)
                        {
                            foreach (var itemgroup in groups)
                            {
                                string typeName = "Khác";
                                var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == itemgroup.Key);
                                if (serviceType != null)
                                {
                                    typeName = serviceType.SERVICE_TYPE_NAME;
                                }

                                temp.Add(string.Format("{0}[{1}]", typeName, Inventec.Common.Number.Convert.NumberToString(itemgroup.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0))));
                            }
                        }

                        transDescriptionNoDeposit = String.Join(", ", temp);
                    }

                    var groupSereServs = rdo._ListSereServ.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                    //sắp xếp theo loại
                    var serviceTypeOrders = BackendDataWorker.Get<HIS_SERVICE_TYPE>().OrderBy(o => o.NUM_ORDER ?? 99999).ToList();
                    List<string> descriptionV2 = new List<string>();
                    foreach (var sety in serviceTypeOrders)
                    {
                        List<HIS_SERE_SERV> grSereServ = groupSereServs.Where(o => o.Key == sety.ID).SelectMany(o => o).ToList();
                        if (grSereServ != null && grSereServ.Count > 0)
                        {
                            if (sety.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC)
                            {
                                List<long> serviceId = grSereServ.Select(s => s.SERVICE_ID).Distinct().ToList();
                                List<V_HIS_SERVICE> lisService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => serviceId.Contains(o.ID)).ToList();
                                if (lisService != null && lisService.Count > 0)
                                {
                                    var serviceParent = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => lisService.Exists(e => e.PARENT_ID == o.ID)).ToList();
                                    var groupParent = lisService.GroupBy(o => o.PARENT_ID).ToList();

                                    if (serviceParent != null && serviceParent.Count > 0)
                                    {
                                        //sắp xếp theo nhóm cha
                                        serviceParent = serviceParent.OrderBy(o => o.NUM_ORDER).ToList();
                                        foreach (var paren in serviceParent)
                                        {
                                            var listSv = groupParent.Where(o => o.Key == paren.ID).SelectMany(s => s).ToList();
                                            var lstSS = grSereServ.Where(o => listSv.Exists(e => e.ID == o.SERVICE_ID)).ToList();
                                            if (lstSS != null && lstSS.Count > 0)
                                            {
                                                descriptionV2.Add(string.Format("Khác: {0}({1})", paren.SERVICE_NAME, Inventec.Common.Number.Convert.NumberToString(lstSS.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0))));
                                            }
                                        }
                                    }

                                    //thêm dịch vu không có nhóm cha
                                    if (groupParent.Exists(o => !o.Key.HasValue))
                                    {
                                        var lstSS = grSereServ.Where(o => groupParent.Where(c => !c.Key.HasValue).SelectMany(s => s).Select(s => s.ID).Contains(o.SERVICE_ID)).ToList();
                                        descriptionV2.Add(string.Format("{0}({1})", sety.SERVICE_TYPE_NAME, Inventec.Common.Number.Convert.NumberToString(lstSS.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0))));
                                    }
                                }
                                else
                                {
                                    descriptionV2.Add(string.Format("{0}({1})", sety.SERVICE_TYPE_NAME, Inventec.Common.Number.Convert.NumberToString(grSereServ.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0))));
                                }
                            }
                            else
                            {
                                descriptionV2.Add(string.Format("{0}({1})", sety.SERVICE_TYPE_NAME, Inventec.Common.Number.Convert.NumberToString(grSereServ.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0))));
                            }
                        }
                    }

                    transDescriptionV2 = String.Join("; ", descriptionV2);
                }

                SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.TRANS_DESCRIPTION, result));
                SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.TRANS_DESCRIPTION_NO_DEPOSIT, transDescriptionNoDeposit));
                SetSingleKey(new KeyValue("TRANS_DESCRIPTION_V2", transDescriptionV2));
                if (rdo._PatientTypeAlter != null)
                {
                    var ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(rdo._PatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, rdo._PatientTypeAlter.HEIN_CARD_NUMBER, rdo._PatientTypeAlter.LEVEL_CODE, rdo._PatientTypeAlter.RIGHT_ROUTE_CODE);
                    if (ratio.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.HEIN_RATIO_100, ratio.Value * 100));
                    }

                    SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.HEIN_CARD_ADDRESS, rdo._PatientTypeAlter.ADDRESS));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatientTypeAlter, false);
                }

                if (rdo._Patient != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo._Patient, false);
                }

                if (rdo._DepartmentTran != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo._DepartmentTran, false);
                }

                SetSingleKey(new KeyValue(Mps000111ExtendSingleKey.KEY_THU_PHI, rdo.KeyThuPhi));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGroupData()
        {
            try
            {
                if (rdo != null && rdo._ListSereServ != null && rdo._ListSereServ.Count > 0)
                {
                    listType = new List<ServiceTypeADO>();
                    listgroupType = new List<ServiceTypeADO>();
                    var listData = (from n in rdo._ListSereServ select new ServiceTypeADO(n)).ToList();
                    var groupByType = listData.GroupBy(o => new { o.TYPE, o.SERVICE_TYPE_ID }).ToList();

                    foreach (var item in groupByType)
                    {
                        ServiceTypeADO ado = new ServiceTypeADO(item.First());
                        ado.AMOUNT = item.Sum(s => s.AMOUNT ?? 0);
                        ado.TOTAL_PRICE = item.Sum(s => s.TOTAL_PRICE ?? 0);
                        ado.PRICE = null;
                        listType.Add(ado);
                    }

                    if (listType.Count > 0)
                    {
                        listgroupType = listType.GroupBy(o => o.TYPE).Select(s => s.First()).ToList();
                        listgroupType = listgroupType.OrderByDescending(o => o.TYPE).ToList();
                    }

                    listType = listType.OrderBy(o => o.SERVICE_NUM_ORDER ?? 99999).ToList();

                    var listDataSvt = (from n in rdo._ListSereServ select new ServiceTypeADO(n)).ToList();
                    var groupBySvType = listData.GroupBy(o => new { o.SERVICE_TYPE_ID }).ToList();
                    foreach (var item in groupBySvType)
                    {
                        ServiceTypeADO ado = new ServiceTypeADO(item.First());
                        ado.AMOUNT = item.Sum(s => s.AMOUNT ?? 0);
                        ado.VIR_TOTAL_HEIN_PRICE = item.Sum(s => s.VIR_TOTAL_HEIN_PRICE ?? 0);
                        ado.VIR_TOTAL_PATIENT_PRICE = item.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                        ado.VIR_TOTAL_PATIENT_PRICE_BHYT = item.Sum(s => s.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                        ado.TOTAL_OTHER_SOURCE_PRICE = item.Sum(s => (s.OTHER_SOURCE_PRICE ?? 0) * (1 + (s.VAT_RATIO ?? 0)) * (1 + (s.AMOUNT ?? 0)));
                        ado.PRICE = null;
                        listTypeSvt.Add(ado);
                    }
                    listTypeSvt = listTypeSvt.OrderBy(o => o.SERVICE_NUM_ORDER ?? 99999).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public static class AgeUtil
    {
        public static string CalculateFullAge(long ageNumber)
        {
            string tuoi;
            string cboAge;
            try
            {
                DateTime dtNgSinh = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ageNumber));
                TimeSpan diff = DateTime.Now - dtNgSinh;
                long tongsogiay = diff.Ticks;
                if (tongsogiay < 0)
                {
                    tuoi = "";
                    cboAge = "Tuổi";
                    return "";
                }
                DateTime newDate = new DateTime(tongsogiay);

                int nam = newDate.Year - 1;
                int thang = newDate.Month - 1;
                int ngay = newDate.Day - 1;
                int gio = newDate.Hour;
                int phut = newDate.Minute;
                int giay = newDate.Second;

                if (nam > 0)
                {
                    tuoi = nam.ToString();
                    cboAge = "Tuổi";
                }
                else
                {
                    if (thang > 0)
                    {
                        tuoi = thang.ToString();
                        cboAge = "Tháng";
                    }
                    else
                    {
                        if (ngay > 0)
                        {
                            tuoi = ngay.ToString();
                            cboAge = "ngày";
                        }
                        else
                        {
                            tuoi = "";
                            cboAge = "Giờ";
                        }
                    }
                }
                return tuoi + " " + cboAge;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }
}
