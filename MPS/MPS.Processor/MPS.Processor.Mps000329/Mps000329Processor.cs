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
using FlexCel.Report;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000329.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000329
{
    class Mps000329Processor : AbstractProcessor
    {
        Mps000329PDO rdo;
        Dictionary<long, string> dicTreatmentBedRoomNow;
        List<HIS_RATION_TIME> __curentRationTimes;
        Dictionary<long, V_HIS_SERVICE> dicService;

        public Mps000329Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000329PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                //SetBarcodeKey();
                SetSingleKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "Services", rdo.listAdo);
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
                if (rdo._vSereServs != null && rdo._vSereServs.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._vSereServs.Count), rdo._vSereServs.Count));
                    dicService = BackendDataWorker.Get<V_HIS_SERVICE>().ToDictionary(o => o.ID, o => o);
                    rdo.listAdo = new List<Mps000329ADO>();
                    var groupSereServ = rdo._vSereServs.GroupBy(o => new { o.TDL_TREATMENT_ID, o.TDL_PATIENT_NAME, o.TDL_INTRUCTION_DATE, o.TDL_REQUEST_DEPARTMENT_ID }).OrderBy(o => o.Key.TDL_INTRUCTION_DATE).OrderBy(o => o.Key.TDL_REQUEST_DEPARTMENT_ID).ThenBy(o => o.Key.TDL_PATIENT_NAME).ToArray();

                    GetRationTimes();
                    dicTreatmentBedRoomNow = new Dictionary<long, string>();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("groupSereServ.Count", groupSereServ.Count()));
                    foreach (var group in groupSereServ)
                    {
                        Mps000329ADO Mps000329ADO = new Mps000329ADO();
                        var listSS = group.ToList();
                        Mps000329ADO.PATIENT_CODE = listSS.First().TDL_PATIENT_CODE;
                        Mps000329ADO.PATIENT_NAME = listSS.First().TDL_PATIENT_NAME;
                        Mps000329ADO.TREATMENT_CODE = listSS.First().TDL_TREATMENT_CODE;
                        Mps000329ADO.INTRUCTION_DATE = listSS.First().TDL_INTRUCTION_DATE;
                        Mps000329ADO.DOB = listSS.First().TDL_PATIENT_DOB;
                        Mps000329ADO.DESCRIPTION = "";//TODO                   
                        Mps000329ADO.BED_NAME = GetTreatmentBedRoomByPatient(listSS.First().TDL_TREATMENT_ID ?? 0);
                        Mps000329ADO.SERVICE_GROUP_NAME = GetServiceGroupByTreatment(listSS.First().TDL_TREATMENT_ID ?? 0);//TODO

                        int demRT = 0;
                        foreach (var rtime in this.__curentRationTimes)
                        {
                            var ss1 = listSS.Where(o => o.RATION_TIME_ID == rtime.ID).FirstOrDefault();
                            if (ss1 == null) continue;

                            var sv = dicService.ContainsKey(ss1.SERVICE_ID) ? dicService[ss1.SERVICE_ID] : null;
                            if (sv == null) continue;
                            string strBua = ss1.TDL_SERVICE_NAME + " (" + (sv != null ? (long?)(ss1.AMOUNT * sv.CAPACITY ?? 0) + sv.SERVICE_UNIT_NAME : "") + ")";
                            switch (demRT)
                            {
                                case 0:
                                    Mps000329ADO.BUA1 = strBua;
                                    break;
                                case 1:
                                    Mps000329ADO.BUA2 = strBua;
                                    break;
                                case 2:
                                    Mps000329ADO.BUA3 = strBua;
                                    break;
                                case 3:
                                    Mps000329ADO.BUA4 = strBua;
                                    break;
                                case 4:
                                    Mps000329ADO.BUA5 = strBua;
                                    break;
                                default:
                                    break;
                            }

                            demRT++;
                        }

                        rdo.listAdo.Add(Mps000329ADO);
                    }

                    if (rdo.SingleKeyValue == null)
                        rdo.SingleKeyValue = new SingleKeyValue();
                    rdo.SingleKeyValue.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                    rdo.SingleKeyValue.SUM_AMOUNT = rdo._vSereServs.Sum(o => o.AMOUNT);
                    int demRT1 = 0;
                    foreach (var rtime in this.__curentRationTimes)
                    {
                        var ssSums1 = rdo._vSereServs.Where(o => o.RATION_TIME_ID == rtime.ID).ToList();
                        if (ssSums1 == null || ssSums1.Count == 0)
                            continue;

                        var sv1 = dicService.ContainsKey(ssSums1.First().SERVICE_ID) ? dicService[ssSums1.First().SERVICE_ID] : null;
                        string SUM_CAPACITY_1 = (long?)ssSums1.Sum(o => o.AMOUNT * GetCapacityOfService(o.SERVICE_ID)) + " " + (sv1 != null ? sv1.SERVICE_UNIT_NAME : "");
                        decimal SUM_AMOUNT_1 = ssSums1.Sum(o => o.AMOUNT);
                        switch (demRT1)
                        {
                            case 0:
                                rdo.SingleKeyValue.SUM_AMOUNT_1 = SUM_AMOUNT_1;
                                rdo.SingleKeyValue.SUM_CAPACITY_1 = SUM_CAPACITY_1;
                                break;
                            case 1:
                                rdo.SingleKeyValue.SUM_AMOUNT_2 = SUM_AMOUNT_1;
                                rdo.SingleKeyValue.SUM_CAPACITY_2 = SUM_CAPACITY_1;
                                break;
                            case 2:
                                rdo.SingleKeyValue.SUM_AMOUNT_3 = SUM_AMOUNT_1;
                                rdo.SingleKeyValue.SUM_CAPACITY_3 = SUM_CAPACITY_1;
                                break;
                            case 3:
                                rdo.SingleKeyValue.SUM_AMOUNT_4 = SUM_AMOUNT_1;
                                rdo.SingleKeyValue.SUM_CAPACITY_4 = SUM_CAPACITY_1;
                                break;
                            case 4:
                                rdo.SingleKeyValue.SUM_AMOUNT_5 = SUM_AMOUNT_1;
                                rdo.SingleKeyValue.SUM_CAPACITY_5 = SUM_CAPACITY_1;
                                break;
                            default:
                                break;
                        }

                        demRT1++;
                    }

                }
                AddObjectKeyIntoListkey<SingleKeyValue>(rdo.SingleKeyValue, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        long GetCapacityOfService(long serviceId)
        {
            var sv = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == serviceId).FirstOrDefault();
            return sv != null ? sv.CAPACITY ?? 0 : 0;
        }

        private void GetRationTimes()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisRationTimeFilter roomTimeFilter = new MOS.Filter.HisRationTimeFilter();
                roomTimeFilter.IS_ACTIVE = 1;
                roomTimeFilter.ORDER_FIELD = "RATION_TIME_CODE";
                roomTimeFilter.ORDER_DIRECTION = "ASC";
                this.__curentRationTimes = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_RATION_TIME>>("api/HisRationTime/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, roomTimeFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string GetServiceGroupByTreatment(long treatmentId)
        {
            string dataResult = String.Empty;
            try
            {
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dataResult;
        }

        string GetTreatmentBedRoomByPatient(long treatmentId)
        {
            string dataResult = String.Empty;
            try
            {
                if (dicTreatmentBedRoomNow.ContainsKey(treatmentId))
                {
                    dicTreatmentBedRoomNow.TryGetValue(treatmentId, out dataResult);
                    return dataResult;
                }

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisTreatmentBedRoomViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                treatFilter.TREATMENT_ID = treatmentId;
                treatFilter.ORDER_DIRECTION = "DESC";
                treatFilter.ORDER_FIELD = "ADD_TIME";

                var resultTreatmentBedRooms = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_TREATMENT_BED_ROOM>>("/api/HisTreatmentBedRoom/GetView", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, paramCommon, treatFilter, 0, null);

                //dataResult = (resultTreatmentBedRooms != null && resultTreatmentBedRooms.Count > 0) ? resultTreatmentBedRooms[0].BED_ROOM_NAME + (!String.IsNullOrEmpty(resultTreatmentBedRooms[0].BED_NAME) ? " - " + resultTreatmentBedRooms[0].BED_NAME : "") : "";
                dataResult = (resultTreatmentBedRooms != null && resultTreatmentBedRooms.Count > 0) ? resultTreatmentBedRooms[0].BED_NAME : "";

                dicTreatmentBedRoomNow.Add(treatmentId, dataResult);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dataResult;
        }

        private void SetBarcodeKey()
        {
            try
            {
                //if (rdo._ServiceReqs != null && !String.IsNullOrEmpty(rdo.Transaction.TRANSACTION_CODE))
                //{
                //    Inventec.Common.BarcodeLib.Barcode barcodeTransactionCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Transaction.TRANSACTION_CODE);
                //    barcodeTransactionCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //    barcodeTransactionCode.IncludeLabel = false;
                //    barcodeTransactionCode.Width = 120;
                //    barcodeTransactionCode.Height = 40;
                //    barcodeTransactionCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //    barcodeTransactionCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //    barcodeTransactionCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //    barcodeTransactionCode.IncludeLabel = true;

                //    dicImage.Add(Mps000329ExtendSingleKey.TRANSACTION_CODE_BAR, barcodeTransactionCode);
                //}

                //if (rdo._SaleExpMest != null && !String.IsNullOrEmpty(rdo._SaleExpMest.EXP_MEST_CODE))
                //{
                //    Inventec.Common.BarcodeLib.Barcode barcodeExpMestCode = new Inventec.Common.BarcodeLib.Barcode(rdo._SaleExpMest.EXP_MEST_CODE);
                //    barcodeExpMestCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //    barcodeExpMestCode.IncludeLabel = false;
                //    barcodeExpMestCode.Width = 120;
                //    barcodeExpMestCode.Height = 40;
                //    barcodeExpMestCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //    barcodeExpMestCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //    barcodeExpMestCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //    barcodeExpMestCode.IncludeLabel = true;

                //    dicImage.Add(Mps000329ExtendSingleKey.EXP_MEST_CODE_BAR, barcodeExpMestCode);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //public override string ProcessPrintLogData()
        //{
        //    string log = "";
        //    try
        //    {
        //        //log = LogDataExpMest(rdo._ServiceReqs.First().TDL_TREATMENT_CODE, "", "");
        //        //log += "Detail: " + String.Join("", rdo._ServiceReqs.Select(o => o.TDL_PATIENT_NAME + " - " + o.KIDNEY_TIMES).ToArray());
        //    }
        //    catch (Exception ex)
        //    {
        //        log = "";
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //    return log;
        //}

        //public override string ProcessUniqueCodeData()
        //{
        //    string result = "";
        //    try
        //    {
        //        if (rdo != null)
        //        {
        //            //if (rdo._ServiceReqs != null && rdo._ServiceReqs.First() != null)
        //            //{
        //            //    result = String.Format("{0}_{1}_{2}_{3}", this.printTypeCode, rdo._ServiceReqs.First().TDL_TREATMENT_CODE, rdo._ServiceReqs.First().EXECUTE_ROOM_ID, rdo._ServiceReqs.First().INTRUCTION_TIME);
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = "";
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //    return result;
        //}
    }
}
