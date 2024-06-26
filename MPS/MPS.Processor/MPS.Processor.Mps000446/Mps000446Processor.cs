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
using HIS.Desktop.Common.BankQrCode;
using Inventec.Common.QRCoder;
using Inventec.Core;
using MPS.Processor.Mps000446.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000446
{
    public class Mps000446Processor : AbstractProcessor
    {
        Mps000446PDO rdo;

        private List<SereServADO> ListSereServTotal = new List<SereServADO>();
        private List<SereServADO> ServiceReqGroupTotals = new List<SereServADO>();
        private List<SereServADO> ServiceTypeGroupTotals = new List<SereServADO>();

        public Mps000446Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000446PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                SetSingleKey();
                SetBarcodeKey();
                SetQrCode();
                ProcessGroupSereServ();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ServiceTotals", ListSereServTotal);
                objectTag.AddObjectData(store, "ServiceReqGroupTotals", ServiceReqGroupTotals);
                objectTag.AddObjectData(store, "ServiceTypeGroupTotals", ServiceTypeGroupTotals);

                objectTag.AddObjectData(store, "SereServs", rdo.VHisSereServs);

                objectTag.AddRelationship(store, "ServiceTypeGroupTotals", "ServiceTotals", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceTypeGroupTotals", "ServiceReqGroupTotals", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceReqGroupTotals", "ServiceTotals", "SERVICE_REQ_ID", "SERVICE_REQ_ID");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.HisTreatment != null && !String.IsNullOrWhiteSpace(rdo.HisTreatment.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.HisTreatment.TREATMENT_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add("TREATMENT_CODE_BAR", barcodeTreatmentCode);
                }

                if (rdo.HisTreatment != null && !String.IsNullOrWhiteSpace(rdo.HisTreatment.TDL_PATIENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.HisTreatment.TDL_PATIENT_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add("PATIENT_CODE_BAR", barcodeTreatmentCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetQrCode()
        {
            try
            {
                if (rdo.TransReq != null && rdo.ListHisConfigPaymentQrCode != null && rdo.ListHisConfigPaymentQrCode.Count > 0)
                {
                    var data = QrCodeProcessor.CreateQrImage(rdo.TransReq, rdo.ListHisConfigPaymentQrCode);
                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            SetSingleKey(new KeyValue(item.Key, item.Value));
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
                if (this.rdo.HisTreatment != null)
                {
                    AddObjectKeyIntoListkey(this.rdo.HisTreatment, false);
                    SetSingleKey(new KeyValue(Mps000446ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisTreatment.TDL_HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000446ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisTreatment.TDL_HEIN_CARD_TO_TIME ?? 0)));
                }
                AddObjectKeyIntoListkey(rdo.TransReq, false);
                if (this.rdo.VHisSereServs != null && this.rdo.VHisSereServs.Count > 0)
                {
                    var listRequestRoomName = rdo.VHisSereServs.Select(o => o.REQUEST_ROOM_NAME).ToList();
                    if (listRequestRoomName != null && listRequestRoomName.Distinct().Count() == 1)
                        SetSingleKey(new KeyValue(Mps000446ExtendSingleKey.REQUEST_ROOM_NAME, listRequestRoomName[0]));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGroupSereServ()
        {
            try
            {
                if (rdo.HisSereServ != null && rdo.HisSereServ.Count > 0)
                {
                    ListSereServTotal.AddRange(from r in rdo.HisSereServ select new SereServADO(r, rdo.HisRoom, rdo.HisService, rdo.HisServiceType));

                    ListSereServTotal = ListSereServTotal.OrderBy(o => o.SERVICE_TYPE_NUM_ORDER ?? 9999).ThenBy(o => o.TDL_SERVICE_NAME).ThenBy(o => o.ID).ToList();

                    List<SereServADO> SereServTotal = new List<SereServADO>();

                    var serviceReqs = ListSereServTotal.GroupBy(o => new { o.SERVICE_REQ_ID, o.TDL_SERVICE_TYPE_ID }).ToList();
                    var serviceTypes = serviceReqs.GroupBy(o => o.Key.TDL_SERVICE_TYPE_ID).ToList();

                    long countType = 1;
                    long count = 1;
                    foreach (var sType in serviceTypes)
                    {
                        SereServADO sTypeAdo = new SereServADO(sType.First().First());
                        sTypeAdo.TOTAL_PRICE_SERVICE_GROUP = sType.Sum(o => o.Sum(s => s.TOTAL_PRICE_SERVICE_GROUP));
                        sTypeAdo.TOTAL_HEIN_PRICE_SERVICE_GROUP = sType.Sum(o => o.Sum(s => s.TOTAL_HEIN_PRICE_SERVICE_GROUP));
                        sTypeAdo.TOTAL_PATIENT_PRICE_SERVICE_GROUP = sType.Sum(o => o.Sum(s => s.TOTAL_PATIENT_PRICE_SERVICE_GROUP));
                        sTypeAdo.STT = countType;
                        ServiceTypeGroupTotals.Add(sTypeAdo);
                        countType++;

                        var reqs = serviceReqs.Where(o => o.Key.TDL_SERVICE_TYPE_ID == sType.Key).ToList();
                        if (sType.Key == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)//sắp xếp lại với xét nghiệm
                        {
                            reqs = reqs.OrderBy(o => o.First().SERVICE_TYPE_NUM_ORDER ?? 9999).ThenBy(o => o.First().TDL_EXECUTE_DEPARTMENT_ID).ToList();
                        }

                        foreach (var req in reqs)
                        {
                            SereServADO reqAdo = new SereServADO(req.First());
                            reqAdo.TOTAL_PRICE_SERVICE_GROUP = req.Sum(o => o.TOTAL_PRICE_SERVICE_GROUP);
                            reqAdo.TOTAL_HEIN_PRICE_SERVICE_GROUP = req.Sum(o => o.TOTAL_HEIN_PRICE_SERVICE_GROUP);
                            reqAdo.TOTAL_PATIENT_PRICE_SERVICE_GROUP = req.Sum(o => o.TOTAL_PATIENT_PRICE_SERVICE_GROUP);
                            reqAdo.STT = count;
                            ServiceReqGroupTotals.Add(reqAdo);
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

                    if (ListSereServTotal.Count == SereServTotal.Count)
                    {
                        ListSereServTotal = SereServTotal;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
