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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000148.ADO;
using MPS.Processor.Mps000148.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000148
{
    public class Mps000148Processor : AbstractProcessor
    {
        List<Mps000148ADO> serviceAdos = new List<Mps000148ADO>();
        Mps000148PDO rdo;
        public Mps000148Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000148PDO)rdoBase;
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

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                ProcessListData();
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "Services1", serviceAdos);
                objectTag.AddObjectData(store, "Services2", serviceAdos);
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

        void ProcessSingleKey()
        {
            try
            {
                if (rdo._Transaction != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false);
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT));
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(amountStr)));
                    decimal amountAfterExem = rdo._Transaction.AMOUNT - (rdo._Transaction.EXEMPTION ?? 0);
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.AMOUNT_AFTER_EXEMPTION, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem)));
                    string amountAfterExemStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(amountAfterExem));
                    string amountAfterExemText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(amountAfterExemStr);
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT, amountAfterExemText));
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.AMOUNT_AFTER_EXEMPTION_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(amountAfterExemText)));

                    string amountAwayZeroStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(Math.Round(rdo._Transaction.AMOUNT, 0, MidpointRounding.AwayFromZero)));
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.AMOUNT_AWAY_ZERO_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.CurrencyToVneseString(amountAwayZeroStr)));

                    decimal canthu = rdo._Transaction.AMOUNT - (rdo._Transaction.KC_AMOUNT ?? 0) - (rdo._Transaction.EXEMPTION ?? 0);
                    if ((rdo._Transaction.TDL_BILL_FUND_AMOUNT ?? 0) > 0)
                    {
                        canthu = canthu - (rdo._Transaction.TDL_BILL_FUND_AMOUNT ?? 0);
                    }

                    string ctAmountText = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu));
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.CT_AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(canthu)));
                    SetSingleKey(new KeyValue(Mps000148ExtendSingleKey.CT_AMOUNT_TEXT_UPPER_FIRST, Inventec.Common.Number.Convert.NumberToStringRoundAuto(canthu, 4)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessListData()
        {
            try
            {
                if (rdo._ListSereServBill != null && rdo._ListSereServBill.Count > 0)
                {
                    Mps000148ADO vpAdo = null;
                    Mps000148ADO bhytAdo = null;
                    foreach (var ssBill in rdo._ListSereServBill)
                    {
                        if (ssBill.TDL_AMOUNT.HasValue)
                        {
                            LogSystem.Info("ssbill.TDL_AMOUNT");
                            if (ssBill.TDL_PATIENT_TYPE_ID == rdo._PatientTypeId_Bhyt)
                            {
                                if (bhytAdo == null)
                                {
                                    bhytAdo = new Mps000148ADO();
                                    bhytAdo.SERVICE_NAME = "Chênh lệch BHYT";
                                    bhytAdo.SERVICE_UNIT_NAME = "Lần";
                                    bhytAdo.TOTAL_PRICE = 0;
                                }

                                bhytAdo.TOTAL_PRICE += ssBill.PATIENT_BHYT_PRICE;

                                if ((ssBill.PATIENT_PAY_PRICE ?? 0) > 0)
                                {
                                    if (vpAdo == null)
                                    {
                                        vpAdo = new Mps000148ADO();
                                        vpAdo.SERVICE_NAME = "Bệnh nhân tự túc";
                                        vpAdo.SERVICE_UNIT_NAME = "Lần";
                                        vpAdo.TOTAL_PRICE = 0;
                                    }
                                    vpAdo.TOTAL_PRICE += (ssBill.PATIENT_PAY_PRICE ?? 0);
                                }
                            }
                            else
                            {
                                if (vpAdo == null)
                                {
                                    vpAdo = new Mps000148ADO();
                                    vpAdo.SERVICE_NAME = "Bệnh nhân tự túc";
                                    vpAdo.SERVICE_UNIT_NAME = "Lần";
                                    vpAdo.TOTAL_PRICE = 0;
                                }
                                vpAdo.TOTAL_PRICE += ssBill.PRICE;

                            }
                        }
                        else
                        {
                            var ss = rdo._ListSereServ != null ? rdo._ListSereServ.FirstOrDefault(o => o.ID == ssBill.SERE_SERV_ID) : null;
                            if (ss == null)
                            {
                                continue;
                            }
                            else if (ss.PATIENT_TYPE_ID == rdo._PatientTypeId_Bhyt)
                            {
                                if (bhytAdo == null)
                                {
                                    bhytAdo = new Mps000148ADO();
                                    bhytAdo.SERVICE_NAME = "Chênh lệch BHYT";
                                    bhytAdo.SERVICE_UNIT_NAME = "Lần";
                                    bhytAdo.TOTAL_PRICE = 0;
                                }

                                if (ss.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                                {
                                    bhytAdo.TOTAL_PRICE += ssBill.PRICE;
                                }
                                else
                                {
                                    decimal bhyt_price = (ss.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                                    bhytAdo.TOTAL_PRICE += bhyt_price;

                                    if (ssBill.PRICE > bhyt_price)
                                    {
                                        if (vpAdo == null)
                                        {
                                            vpAdo = new Mps000148ADO();
                                            vpAdo.SERVICE_NAME = "Bệnh nhân tự túc";
                                            vpAdo.SERVICE_UNIT_NAME = "Lần";
                                            vpAdo.TOTAL_PRICE = 0;
                                        }
                                        vpAdo.TOTAL_PRICE += (ssBill.PRICE - bhyt_price);
                                    }
                                }
                            }
                            else
                            {
                                if (vpAdo == null)
                                {
                                    vpAdo = new Mps000148ADO();
                                    vpAdo.SERVICE_NAME = "Bệnh nhân tự túc";
                                    vpAdo.SERVICE_UNIT_NAME = "Lần";
                                    vpAdo.TOTAL_PRICE = 0;
                                }
                                vpAdo.TOTAL_PRICE += ssBill.PRICE;
                            }
                        }
                    }

                    if (vpAdo != null)
                    {
                        vpAdo.AMOUNT = 1;
                        vpAdo.PRICE = vpAdo.TOTAL_PRICE;
                        serviceAdos.Add(vpAdo);
                    }
                    if (bhytAdo != null)
                    {
                        bhytAdo.AMOUNT = 1;
                        bhytAdo.PRICE = bhytAdo.TOTAL_PRICE;
                        serviceAdos.Add(bhytAdo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (!String.IsNullOrEmpty(rdo._Transaction.TRANSACTION_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTransactionCode = new Inventec.Common.BarcodeLib.Barcode(rdo._Transaction.TRANSACTION_CODE);
                    barcodeTransactionCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTransactionCode.IncludeLabel = false;
                    barcodeTransactionCode.Width = 120;
                    barcodeTransactionCode.Height = 40;
                    barcodeTransactionCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTransactionCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTransactionCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTransactionCode.IncludeLabel = true;

                    dicImage.Add(Mps000148ExtendSingleKey.TRANSACTION_CODE_BAR, barcodeTransactionCode);
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
                log = LogDataTransaction(rdo._Transaction.TREATMENT_CODE, rdo._Transaction.TRANSACTION_CODE, "");
                log += "SoTien: " + rdo._Transaction.AMOUNT;
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
                if (rdo != null && rdo._Transaction != null)
                    result = String.Format("{0}_{1}_{2}_{3}", rdo._Transaction.TREATMENT_CODE, rdo._Transaction.TRANSACTION_CODE, rdo._Transaction.ACCOUNT_BOOK_CODE, printTypeCode);
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
