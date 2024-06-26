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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000105.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MPS.Processor.Mps000105
{
    public class Mps000105Processor : AbstractProcessor
    {
        Mps000105PDO rdo;

        List<V_HIS_SERE_SERV> _SereServByTypes = new List<V_HIS_SERE_SERV>();

        public Mps000105Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000105PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.PatientCode);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000105ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "sereServPrints", rdo._ListSereServ);
                objectTag.AddObjectData(store, "ExecuteRooms", this._SereServByTypes);
                objectTag.AddRelationship(store, "ExecuteRooms", "sereServPrints", "TDL_EXECUTE_ROOM_ID", "TDL_EXECUTE_ROOM_ID");
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
                this._SereServByTypes = new List<V_HIS_SERE_SERV>();
                decimal totalPrice = 0;
                decimal totalPatientPrice = 0;
                if (this.rdo._ListSereServ != null && this.rdo._ListSereServ.Count > 0)
                {
                    var groups = this.rdo._ListSereServ.GroupBy(p => p.TDL_EXECUTE_ROOM_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in groups)
                    {
                        this._SereServByTypes.Add(item.FirstOrDefault());
                    }
                    totalPrice = this.rdo._ListSereServ.Sum(s => s.VIR_TOTAL_PRICE ?? 0);
                    totalPatientPrice = this.rdo._ListSereServ.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                }
                if (this.rdo._Transaction != null)
                {

                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    string temp = this.rdo._Transaction.TDL_PATIENT_DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(this.rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice)));

                    string totalPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice));
                    string totalPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PRICE_TEXT, totalPriceText));
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPriceText)));

                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice)));

                    string totalPatientPriceString = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPatientPrice));
                    string totalPatientPriceText = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(totalPriceString);
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT, totalPatientPriceText));
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.VIR_TOTAL_PATIENT_PRICE_TEXT_UPPER_FIRST, Inventec.Common.String.Convert.UppercaseFirst(totalPatientPriceText)));
                    rdo.PatientCode = this.rdo._Transaction.TDL_PATIENT_CODE;
                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(this.rdo._Transaction, false);
                }

                if (this.rdo._ServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(this.rdo._ServiceReq, false);
                }
                if (this.rdo._PatyAlterBhyt != null)
                {
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, GlobalQuery.SetHeinCardNumberDisplayByNumber(this.rdo._PatyAlterBhyt.HEIN_CARD_NUMBER)));
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_FROM_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_TO_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(this.rdo._PatyAlterBhyt, false);
                }
                else if (this.rdo._HeinCard != null)
                {
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, GlobalQuery.SetHeinCardNumberDisplayByNumber(this.rdo._HeinCard.HeinCardNumber)));
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_FROM_TIME_STR, this.rdo._HeinCard.FromTime));
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.HEIN_CARD_TO_TIME_STR, this.rdo._HeinCard.ToTime));
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.HEIN_MEDI_ORG_CODE, this.rdo._HeinCard.MediOrgCode));
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.HEIN_ADDRESS, this.rdo._HeinCard.Address));
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.RATIO_TEXT, this.rdo._HeinCard.RatioText));
                }

                if (!String.IsNullOrEmpty(rdo.ratioText))
                {
                    SetSingleKey(new KeyValue(Mps000105ExtendSingleKey.RATIO_TEXT, this.rdo.ratioText));
                }

                if (this.rdo.patient != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(this.rdo.patient, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}

