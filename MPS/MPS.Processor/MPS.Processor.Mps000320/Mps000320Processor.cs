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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000320.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000320
{
    class Mps000320Processor : AbstractProcessor
    {
        Mps000320PDO rdo;
        public Mps000320Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000320PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                SetSingleKey();
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListKidney", rdo.listAdo);
                //barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (this.rdo.ExecuteRoom != null)
                {
                    AddObjectKeyIntoListkey(this.rdo.ExecuteRoom, false);
                }
                if (this.rdo._ServiceReqs != null && this.rdo._ServiceReqs.Count > 0)
                {
                    var Group = this.rdo._ServiceReqs.GroupBy(g => new { g.KIDNEY_SHIFT }).ToList();
                    rdo.listAdo = new List<Mps000320ADO>();
                    this.rdo._Machines = this.rdo._Machines.OrderBy(o => o.MACHINE_CODE).ToList();
                    
                    foreach (var mc in this.rdo._Machines)
                    {
                        Mps000320ADO mps000320ADO = new Mps000320ADO();
                        mps000320ADO.MACHINE_CA1 = mc.MACHINE_CODE;
                        mps000320ADO.MACHINE_CA2 = mc.MACHINE_CODE;
                        mps000320ADO.MACHINE_CA3 = mc.MACHINE_CODE;
                        mps000320ADO.MACHINE_CA4 = mc.MACHINE_CODE;
                        mps000320ADO.MACHINE_CA5 = mc.MACHINE_CODE;
                        rdo.listAdo.Add(mps000320ADO);
                    }

                    foreach (var group in Group)
                    {
                        var svreqs = group.ToList();
                        switch (group.Key.KIDNEY_SHIFT)
                        {
                            case 1:
                                foreach (var item in rdo.listAdo)
                                {
                                    var mc = this.rdo._Machines.Where(o => o.MACHINE_CODE == item.MACHINE_CA1).FirstOrDefault();
                                    var svreqsDetail = svreqs.Where(o => o.KIDNEY_SHIFT == group.Key.KIDNEY_SHIFT && mc != null && (o.MACHINE_ID ?? 0) == mc.ID).FirstOrDefault();
                                    if (svreqsDetail != null)
                                    {
                                        item.KIDNEY_SHIFT_CA1 = svreqsDetail.KIDNEY_SHIFT ?? 0;
                                        item.KIDNEY_SHIFT_STR_CA1 = ("Ca " + (svreqsDetail.KIDNEY_SHIFT ?? 0) + "");
                                        item.KIDNEY_TIMES_CA1 = svreqsDetail.KIDNEY_TIMES;
                                        item.PATIENT_NAME_CA1 = svreqsDetail.TDL_PATIENT_NAME;
                                        item.PATIENT_DOB_CA1 = svreqsDetail.TDL_PATIENT_DOB;
                                    }
                                }
                                break;
                            case 2:
                                foreach (var item in rdo.listAdo)
                                {
                                    var mc = this.rdo._Machines.Where(o => o.MACHINE_CODE == item.MACHINE_CA2).FirstOrDefault();
                                    var svreqsDetail = svreqs.Where(o => o.KIDNEY_SHIFT == group.Key.KIDNEY_SHIFT && mc != null && (o.MACHINE_ID ?? 0) == mc.ID).FirstOrDefault();
                                    if (svreqsDetail != null)
                                    {
                                        item.KIDNEY_SHIFT_CA2 = svreqsDetail.KIDNEY_SHIFT ?? 0;
                                        item.KIDNEY_SHIFT_STR_CA2 = ("Ca " + (svreqsDetail.KIDNEY_SHIFT ?? 0) + "");
                                        item.KIDNEY_TIMES_CA2 = svreqsDetail.KIDNEY_TIMES;
                                        item.PATIENT_NAME_CA2 = svreqsDetail.TDL_PATIENT_NAME;
                                        item.PATIENT_DOB_CA2 = svreqsDetail.TDL_PATIENT_DOB;
                                    }
                                }
                                break;
                            case 3:
                                foreach (var item in rdo.listAdo)
                                {
                                    var mc = this.rdo._Machines.Where(o => o.MACHINE_CODE == item.MACHINE_CA3).FirstOrDefault();
                                    var svreqsDetail = svreqs.Where(o => o.KIDNEY_SHIFT == group.Key.KIDNEY_SHIFT && mc != null && (o.MACHINE_ID ?? 0) == mc.ID).FirstOrDefault();
                                    if (svreqsDetail != null)
                                    {
                                        item.KIDNEY_SHIFT_CA3 = svreqsDetail.KIDNEY_SHIFT ?? 0;
                                        item.KIDNEY_SHIFT_STR_CA3 = ("Ca " + (svreqsDetail.KIDNEY_SHIFT ?? 0) + "");
                                        item.KIDNEY_TIMES_CA3 = svreqsDetail.KIDNEY_TIMES;
                                        item.PATIENT_NAME_CA3 = svreqsDetail.TDL_PATIENT_NAME;
                                        item.PATIENT_DOB_CA3 = svreqsDetail.TDL_PATIENT_DOB;
                                    }
                                }
                                break;
                            case 4:
                                foreach (var item in rdo.listAdo)
                                {
                                    var mc = this.rdo._Machines.Where(o => o.MACHINE_CODE == item.MACHINE_CA4).FirstOrDefault();
                                    var svreqsDetail = svreqs.Where(o => o.KIDNEY_SHIFT == group.Key.KIDNEY_SHIFT && mc != null && (o.MACHINE_ID ?? 0) == mc.ID).FirstOrDefault();
                                    if (svreqsDetail != null)
                                    {
                                        item.KIDNEY_SHIFT_CA4 = svreqsDetail.KIDNEY_SHIFT ?? 0;
                                        item.KIDNEY_SHIFT_STR_CA4 = ("Ca " + (svreqsDetail.KIDNEY_SHIFT ?? 0) + "");
                                        item.KIDNEY_TIMES_CA4 = svreqsDetail.KIDNEY_TIMES;
                                        item.PATIENT_NAME_CA4 = svreqsDetail.TDL_PATIENT_NAME;
                                        item.PATIENT_DOB_CA4 = svreqsDetail.TDL_PATIENT_DOB;
                                    }
                                }
                                break;
                            case 5:
                                foreach (var item in rdo.listAdo)
                                {
                                    var mc = this.rdo._Machines.Where(o => o.MACHINE_CODE == item.MACHINE_CA5).FirstOrDefault();
                                    var svreqsDetail = svreqs.Where(o => o.KIDNEY_SHIFT == group.Key.KIDNEY_SHIFT && mc != null && (o.MACHINE_ID ?? 0) == mc.ID).FirstOrDefault();
                                    if (svreqsDetail != null)
                                    {
                                        item.KIDNEY_SHIFT_CA5 = svreqsDetail.KIDNEY_SHIFT ?? 0;
                                        item.KIDNEY_SHIFT_STR_CA5 = ("Ca " + (svreqsDetail.KIDNEY_SHIFT ?? 0) + "");
                                        item.KIDNEY_TIMES_CA5 = svreqsDetail.KIDNEY_TIMES;
                                        item.PATIENT_NAME_CA5 = svreqsDetail.TDL_PATIENT_NAME;
                                        item.PATIENT_DOB_CA5 = svreqsDetail.TDL_PATIENT_DOB;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                string timeDisplay = "";
                if (this.rdo.Day != null && this.rdo.Day != DateTime.MinValue && this.rdo.Thu > 0)
                {
                    timeDisplay = GenerateThu(this.rdo.Thu) + Inventec.Common.DateTime.Convert.SystemDateTimeToDateSeparateString(this.rdo.Day);
                }
                else if (this.rdo.FromTime != null && this.rdo.FromTime != DateTime.MinValue && this.rdo.ToTime != null && this.rdo.ToTime != DateTime.MinValue)
                {
                    timeDisplay = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(this.rdo.FromTime) + " - " + Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(this.rdo.ToTime);
                }
                SetSingleKey(new KeyValue(Mps000320ExtendSingleKey.DISPLAY_TIME__FROM_TO, timeDisplay));
                rdo.listAdo = rdo.listAdo.OrderBy(o => o.MACHINE_CA1).ThenBy(t => t.PATIENT_NAME_CA1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string GenerateThu(int thu)
        {
            string rs = "";
            switch (thu)
            {
                case 2:
                    rs = "THỨ HAI ";
                    break;
                case 3:
                    rs = "THỨ BA ";
                    break;
                case 4:
                    rs = "THỨ TƯ ";
                    break;
                case 5:
                    rs = "THỨ NĂM ";
                    break;
                case 6:
                    rs = "THỨ SÁU ";
                    break;
                case 7:
                    rs = "THỨ BẢY ";
                    break;
                case 8:
                    rs = "CHỦ NHẬT ";
                    break;
                default:
                    break;
            }
            return rs;
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

                //    dicImage.Add(Mps000320ExtendSingleKey.TRANSACTION_CODE_BAR, barcodeTransactionCode);
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

                //    dicImage.Add(Mps000320ExtendSingleKey.EXP_MEST_CODE_BAR, barcodeExpMestCode);
                //}
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
                log = LogDataExpMest(rdo._ServiceReqs.First().TDL_TREATMENT_CODE, "", "");
                log += "Detail: " + String.Join("", rdo._ServiceReqs.Select(o => o.TDL_PATIENT_NAME + " - " + o.KIDNEY_TIMES).ToArray());
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
                if (rdo != null)
                {
                    if (rdo._ServiceReqs != null && rdo._ServiceReqs.First() != null)
                    {
                        result = String.Format("{0}_{1}_{2}_{3}", this.printTypeCode, rdo._ServiceReqs.First().TDL_TREATMENT_CODE, rdo._ServiceReqs.First().EXECUTE_ROOM_ID, rdo._ServiceReqs.First().INTRUCTION_TIME);
                    }
                }
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
