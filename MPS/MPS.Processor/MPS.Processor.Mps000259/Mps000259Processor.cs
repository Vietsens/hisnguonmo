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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using MPS.Processor.Mps000259.PDO;
using MPS.ProcessorBase.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000259
{
    public class Mps000259Processor : AbstractProcessor
    {
        List<Mps000259SDO> listSereServ;
        List<Mps000259SDO> listSereServType;

        Mps000259PDO rdo;
        public Mps000259Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000259PDO)rdoBase;
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
                ProcessListSereServ();
                if (listSereServType != null && listSereServType.Count > 0)
                {
                    listSereServType = listSereServType.OrderBy(o => o.Type).ToList();
                }
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "SereServ", listSereServ);
                objectTag.AddObjectData(store, "SereServType", listSereServType);
                objectTag.AddRelationship(store, "SereServType", "SereServ", "Type", "Type");

                var listSereServGroupServiceType = listSereServ.GroupBy(g => g.TDL_SERVICE_TYPE_ID).Select(s => new Mps000259SDO { TDL_SERVICE_TYPE_ID = s.First().TDL_SERVICE_TYPE_ID, SERVICE_TYPE_NAME = s.First().SERVICE_TYPE_NAME }).ToList();

                listSereServGroupServiceType = listSereServGroupServiceType.OrderBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                objectTag.AddObjectData(store, "SereServData", listSereServ);
                objectTag.AddObjectData(store, "SereServTypeGroup", listSereServGroupServiceType);
                objectTag.AddRelationship(store, "SereServTypeGroup", "SereServData", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
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
                if (rdo.HisTreatment != null)
                {
                    if (!String.IsNullOrEmpty(rdo.HisTreatment.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.HisTreatment.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000259ExtendSingleKey.BARCODE_TREATMENT_CODE, barcodeTreatment);
                    }

                    if (!string.IsNullOrWhiteSpace(rdo.HisTreatment.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.HisTreatment.TDL_PATIENT_CODE);
                        barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatient.IncludeLabel = false;
                        barcodePatient.Width = 120;
                        barcodePatient.Height = 40;
                        barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatient.IncludeLabel = true;

                        dicImage.Add(Mps000259ExtendSingleKey.BARCODE_PATIENT_CODE, barcodePatient);
                    }
                }

                if (rdo._Transaction != null)
                {
                    if (!String.IsNullOrWhiteSpace(rdo._Transaction.TRANSACTION_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTransaction = new Inventec.Common.BarcodeLib.Barcode(rdo._Transaction.TRANSACTION_CODE);
                        barcodeTransaction.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTransaction.IncludeLabel = false;
                        barcodeTransaction.Width = 120;
                        barcodeTransaction.Height = 40;
                        barcodeTransaction.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTransaction.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTransaction.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTransaction.IncludeLabel = true;

                        dicImage.Add(Mps000259ExtendSingleKey.BARCODE_TRANSACTION_CODE, barcodeTransaction);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListSereServ()
        {
            try
            {
                if (rdo.listSereServBill != null && rdo.listSereServBill.Count > 0 && rdo.listSereServBill.Any(a => a.TDL_AMOUNT.HasValue))
                {
                    listSereServ = new List<Mps000259SDO>();
                    listSereServType = new List<Mps000259SDO>();
                    var dataSereServGroups = rdo.listSereServBill.GroupBy(p => new { p.TDL_SERVICE_ID, p.TDL_SERVICE_CODE, p.TDL_SERVICE_NAME, p.PRICE, p.VAT_RATIO }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataSereServGroups)
                    {
                        var sdo = new Mps000259SDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000259SDO>(sdo, item.FirstOrDefault());
                        sdo.AMOUNT = item.Sum(p => (p.TDL_AMOUNT ?? 0));
                        sdo.TDL_AMOUNT = item.Sum(p => p.TDL_AMOUNT);
                        if (item.Count > 1)
                        {
                            sdo.TDL_TOTAL_HEIN_PRICE = item.Sum(p => p.TDL_TOTAL_HEIN_PRICE);
                            sdo.TDL_TOTAL_PATIENT_PRICE = item.Sum(p => p.TDL_TOTAL_PATIENT_PRICE);
                            sdo.TDL_TOTAL_PATIENT_PRICE_BHYT = item.Sum(p => p.TDL_TOTAL_PATIENT_PRICE_BHYT);
                        }
                        if (item.FirstOrDefault().MEDICINE_ID.HasValue)
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 2);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000259SDO() { Type = 2, SERVICE_TYPE_NAME = item.FirstOrDefault().SERVICE_TYPE_NAME });
                            }
                            sdo.Type = 2;
                        }
                        else if (item.FirstOrDefault().MATERIAL_ID.HasValue)
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 3);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000259SDO() { Type = 3, SERVICE_TYPE_NAME = item.FirstOrDefault().SERVICE_TYPE_NAME });
                            }
                            sdo.Type = 3;
                        }
                        else
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 1);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000259SDO() { Type = 1, SERVICE_TYPE_NAME = "Dịch vụ" });
                            }
                            sdo.Type = 1;
                        }

                        if (item.First().TDL_HEIN_LIMIT_PRICE.HasValue && (item.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G || item.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH))
                        {
                            sdo.PRICE_FEE = item.First().TDL_HEIN_LIMIT_PRICE;
                        }
                        else if (item.First().TDL_LIMIT_PRICE.HasValue)
                        {
                            sdo.PRICE_FEE = item.First().TDL_LIMIT_PRICE;
                        }
                        else if (item.First().TDL_PATIENT_TYPE_ID == rdo.Mps000259ADO.PatientTypeBHYT
                            || item.First().TDL_PATIENT_TYPE_ID == rdo.Mps000259ADO.PatientTypeVP)
                        {
                            sdo.PRICE_FEE = item.First().TDL_REAL_PRICE ?? 0;
                        }
                        else
                        {
                            sdo.PRICE_FEE = 0;
                        }

                        sdo.PRICE_SERVICE = (item.First().TDL_REAL_PRICE ?? 0) - (sdo.PRICE_FEE ?? 0);

                        //Y nghia la co chech lech thi tach ra
                        if (sdo.PRICE_SERVICE == 0 && item.First().TDL_HEIN_LIMIT_PRICE.HasValue && (item.First().TDL_REAL_PRICE ?? 0) > item.First().TDL_HEIN_LIMIT_PRICE)
                        {
                            //khi có chênh lệch thì phần chênh lệch chỉ dồn sang khi là dịch vụ khám hoặc giường.
                            if (item.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G || item.First().TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                            {
                                sdo.PRICE_FEE = item.First().TDL_HEIN_LIMIT_PRICE;
                                sdo.PRICE_SERVICE = (item.First().TDL_REAL_PRICE ?? 0) - sdo.PRICE_FEE;
                            }
                        }

                        sdo.TOTAL_PRICE_SERVICE = (sdo.PRICE_SERVICE ?? 0) * item.Sum(s => s.TDL_AMOUNT);
                        sdo.TOTAL_PRICE_FEE = (sdo.PRICE_FEE ?? 0) * item.Sum(s => s.TDL_AMOUNT);

                        listSereServ.Add(sdo);
                    }

                    var mediMateNotBill = rdo.listSereServ != null ? rdo.listSereServ.Where(o => (o.VIR_TOTAL_PATIENT_PRICE ?? 0) == 0).ToList() : null;
                    if (mediMateNotBill != null && mediMateNotBill.Count > 0)
                    {
                        var Groups = mediMateNotBill.GroupBy(p => new { p.SERVICE_ID, p.TDL_SERVICE_CODE, p.TDL_SERVICE_NAME, p.PRICE, p.VAT_RATIO, p.IS_EXPEND }).Select(p => p.ToList()).ToList();
                        foreach (var item in Groups)
                        {
                            //if (item.FirstOrDefault().IS_EXPEND == 1) continue;
                            if (!rdo.ShowExpend && item.FirstOrDefault().IS_EXPEND == 1) continue;

                            var sdo = new Mps000259SDO(item.FirstOrDefault(), rdo.Mps000259ADO);
                            if (item.Count > 1)
                            {
                                sdo.AMOUNT = item.Sum(p => p.AMOUNT);
                                sdo.TDL_AMOUNT = item.Sum(p => p.AMOUNT);
                                sdo.TDL_TOTAL_HEIN_PRICE = item.Sum(p => p.VIR_TOTAL_HEIN_PRICE);
                                sdo.TDL_TOTAL_PATIENT_PRICE = item.Sum(p => p.VIR_TOTAL_PATIENT_PRICE);
                                sdo.TDL_TOTAL_PATIENT_PRICE_BHYT = item.Sum(p => p.VIR_TOTAL_PATIENT_PRICE_BHYT);
                            }
                            if (item.FirstOrDefault().MEDICINE_ID.HasValue)
                            {
                                var type = listSereServType.FirstOrDefault(o => o.Type == 2);
                                if (type == null)
                                {
                                    listSereServType.Add(new Mps000259SDO() { Type = 2, SERVICE_TYPE_NAME = item.FirstOrDefault().SERVICE_TYPE_NAME });
                                }
                                sdo.Type = 2;
                            }
                            else if (item.FirstOrDefault().MATERIAL_ID.HasValue)
                            {
                                var type = listSereServType.FirstOrDefault(o => o.Type == 3);
                                if (type == null)
                                {
                                    listSereServType.Add(new Mps000259SDO() { Type = 3, SERVICE_TYPE_NAME = item.FirstOrDefault().SERVICE_TYPE_NAME });
                                }
                                sdo.Type = 3;
                            }
                            else
                            {
                                var type = listSereServType.FirstOrDefault(o => o.Type == 1);
                                if (type == null)
                                {
                                    listSereServType.Add(new Mps000259SDO() { Type = 1, SERVICE_TYPE_NAME = "Dịch vụ" });
                                }
                                sdo.Type = 1;
                            }
                            listSereServ.Add(sdo);
                        }
                    }
                }
                else if (rdo.listSereServ != null && rdo.listSereServ.Count > 0)
                {
                    #region --- Sua Moi Theo #15257 ---
                    listSereServ = new List<Mps000259SDO>();
                    listSereServType = new List<Mps000259SDO>();
                    var dataSereServGroups = rdo.listSereServ.GroupBy(p => new { p.SERVICE_ID, p.TDL_SERVICE_CODE, p.TDL_SERVICE_NAME, p.PRICE, p.VAT_RATIO, p.IS_EXPEND }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataSereServGroups)
                    {
                        //if (item.FirstOrDefault().IS_EXPEND == 1) continue;
                        if (!rdo.ShowExpend && item.FirstOrDefault().IS_EXPEND == 1) continue;

                        var sdo = new Mps000259SDO(item.FirstOrDefault(), rdo.Mps000259ADO);
                        //Inventec.Common.Mapper.DataObjectMapper.Map<Mps000259SDO>(sdo, item.FirstOrDefault());
                        if (item.Count > 1)
                        {
                            sdo.AMOUNT = item.Sum(p => p.AMOUNT);
                            sdo.TDL_AMOUNT = item.Sum(p => p.AMOUNT);
                            sdo.TDL_TOTAL_HEIN_PRICE = item.Sum(p => p.VIR_TOTAL_HEIN_PRICE);
                            sdo.TDL_TOTAL_PATIENT_PRICE = item.Sum(p => p.VIR_TOTAL_PATIENT_PRICE);
                            sdo.TDL_TOTAL_PATIENT_PRICE_BHYT = item.Sum(p => p.VIR_TOTAL_PATIENT_PRICE_BHYT);
                        }
                        if (item.FirstOrDefault().MEDICINE_ID.HasValue)
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 2);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000259SDO() { Type = 2, SERVICE_TYPE_NAME = item.FirstOrDefault().SERVICE_TYPE_NAME });
                            }
                            sdo.Type = 2;
                        }
                        else if (item.FirstOrDefault().MATERIAL_ID.HasValue)
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 3);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000259SDO() { Type = 3, SERVICE_TYPE_NAME = item.FirstOrDefault().SERVICE_TYPE_NAME });
                            }
                            sdo.Type = 3;
                        }
                        else
                        {
                            var type = listSereServType.FirstOrDefault(o => o.Type == 1);
                            if (type == null)
                            {
                                listSereServType.Add(new Mps000259SDO() { Type = 1, SERVICE_TYPE_NAME = "Dịch vụ" });
                            }
                            sdo.Type = 1;
                        }
                        listSereServ.Add(sdo);
                    }
                    #endregion

                    #region --- Code Old ---
                    //listSereServ = new List<Mps000259SDO>();
                    //listSereServType = new List<Mps000259SDO>();
                    //foreach (var item in rdo.listSereServ)
                    //{
                    //    if (item.IS_EXPEND == 1) continue;

                    //    var sdo = new Mps000259SDO();
                    //    Inventec.Common.Mapper.DataObjectMapper.Map<Mps000259SDO>(sdo, item);
                    //    if (item.MEDICINE_ID.HasValue)
                    //    {
                    //        var type = listSereServType.FirstOrDefault(o => o.Type == 2);
                    //        if (type == null)
                    //        {
                    //            listSereServType.Add(new Mps000259SDO() { Type = 2, SERVICE_TYPE_NAME = item.SERVICE_TYPE_NAME });
                    //        }
                    //        sdo.Type = 2;
                    //    }
                    //    else if (item.MATERIAL_ID.HasValue)
                    //    {
                    //        var type = listSereServType.FirstOrDefault(o => o.Type == 3);
                    //        if (type == null)
                    //        {
                    //            listSereServType.Add(new Mps000259SDO() { Type = 3, SERVICE_TYPE_NAME = item.SERVICE_TYPE_NAME });
                    //        }
                    //        sdo.Type = 3;
                    //    }
                    //    else
                    //    {
                    //        var type = listSereServType.FirstOrDefault(o => o.Type == 1);
                    //        if (type == null)
                    //        {
                    //            listSereServType.Add(new Mps000259SDO() { Type = 1, SERVICE_TYPE_NAME = "Dịch vụ" });
                    //        }
                    //        sdo.Type = 1;
                    //    }
                    //    listSereServ.Add(sdo);
                    //}
                    #endregion
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
                if (rdo._Transaction != null)
                {
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    string temp = rdo._Transaction.TDL_PATIENT_DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT)));
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT));
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountStr);
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.AMOUNT_TEXT, amountText));
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountText)));
                    decimal amountAfterExem = rdo._Transaction.AMOUNT - (rdo._Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem)));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountAfterExemStr);
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountAfterExemText)));
                    decimal ratio = ((rdo._Transaction.EXEMPTION ?? 0) * 100) / rdo._Transaction.AMOUNT;
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.EXEMPTION_RATIO, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ratio)));

                    //Ket Chuyen, Can Thu
                    if (rdo._Transaction.KC_AMOUNT.HasValue)
                    {
                        string kcAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.KC_AMOUNT.Value));
                        SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.KC_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(kcAmountText)));
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._CanThu_Amount));
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._CanThu_Amount)));
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(ctAmountText)));
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.DESCRIPTION, rdo._Transaction.DESCRIPTION));
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.EXEMPTION_REASON, rdo._Transaction.EXEMPTION_REASON));

                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._Transaction.CREATE_TIME ?? 0)));

                    string depositAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo.depositAmpount));
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.TU_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo.depositAmpount)));
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.TU_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(depositAmountText)));

                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false);
                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.HisTreatment, false);
                    SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.RATIO_TEXT, rdo.RatioText));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo._patient, false);
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatientTypeAlter, false);

                    if (rdo.HisTreatment != null)
                    {
                        var dayOftreatment = HIS.Common.Treatment.Calculation.DayOfTreatment(rdo.HisTreatment.IN_TIME, rdo.HisTreatment.OUT_TIME, rdo.HisTreatment.TREATMENT_END_TYPE_ID, rdo.HisTreatment.TREATMENT_RESULT_ID, rdo._PatientTypeAlter != null && rdo._PatientTypeAlter.PATIENT_TYPE_ID == 1 ? HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT : HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);

                        SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.SO_NGAY_DIEU_TRI, dayOftreatment));

                        if (rdo.HisTreatment.CLINICAL_IN_TIME.HasValue && rdo.HisTreatment.OUT_TIME.HasValue)
                        {
                            DateTime dtIn = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rdo.HisTreatment.CLINICAL_IN_TIME.Value) ?? DateTime.Now;

                            DateTime dtOut = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rdo.HisTreatment.OUT_TIME.Value) ?? DateTime.Now;

                            TimeSpan ts = new TimeSpan();

                            ts = (TimeSpan)(dtOut - dtIn);
                            int day = ts.Days;
                            int gio = 0;
                            if (ts.Hours > 0)
                            {
                                gio = ts.Hours;
                                if (ts.Minutes > 0)
                                {
                                    gio = gio + 1;
                                }
                            }
                            string soNgayDT = "";
                            if (day > 0)
                            {
                                soNgayDT = day + " ngày ";
                            }
                            if (gio > 0)
                            {
                                if (day > 0)
                                {
                                    soNgayDT = soNgayDT + "- " + gio + " giờ";
                                }
                                else
                                {
                                    soNgayDT = gio + " giờ";
                                }
                            }
                            //Tinh ngay gio dieu tri 2h3'==>3h
                            SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.SO_NGAY_GIO_DIEU_TRI, soNgayDT));//
                        }
                    }

                    if (rdo.ListDepartment != null && rdo.ListDepartment.Count > 0 && rdo.HisTreatment != null)
                    {
                        HIS_DEPARTMENT department = null;
                        if (rdo.HisTreatment.END_DEPARTMENT_ID.HasValue)
                        {
                            department = rdo.ListDepartment.FirstOrDefault(o => o.ID == rdo.HisTreatment.END_DEPARTMENT_ID.Value);
                        }
                        else if (rdo.HisTreatment.LAST_DEPARTMENT_ID.HasValue)
                        {
                            department = rdo.ListDepartment.FirstOrDefault(o => o.ID == rdo.HisTreatment.LAST_DEPARTMENT_ID.Value);
                        }

                        if (department != null)
                        {
                            SetSingleKey(new KeyValue(Mps000259ExtendSingleKey.TREAT_DEPARTMENT_NAME, department.DEPARTMENT_NAME));
                        }
                    }

                    var transactionInfo = JsonConvert.DeserializeObject<TransactionInfoSDO>(rdo._Transaction.TRANSACTION_INFO);
                    AddObjectKeyIntoListkey<TransactionInfoSDO>(transactionInfo, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
