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
using MPS.Processor.Mps000104.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MPS.Processor.Mps000104
{
    class Mps000104Processor : AbstractProcessor
    {
        Mps000104PDO rdo;
        public Mps000104Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000104PDO)rdoBase;
        }

        

        public void SetBarcodeKey()
        {
            try
            {
                //Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentHisTreatment.TREATMENT_CODE);
                //barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodeTreatmentCode.IncludeLabel = false;
                //barcodeTreatmentCode.Width = 120;
                //barcodeTreatmentCode.Height = 40;
                //barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodeTreatmentCode.IncludeLabel = true;

                //dicImage.Add(Mps000104ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);

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


                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        class CustomerFuncRownumberData : TFlexCelUserFunction
        {
            public CustomerFuncRownumberData()
            {
            }
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                long result = 0;
                try
                {
                    long rownumber = Convert.ToInt64(parameters[0]);
                    result = (rownumber + 1);
                }
                catch (Exception ex)
                {
                    LogSystem.Debug(ex);
                }

                return result;
            }
        }

        void ProcessSingleKey()
        {
            try
            {
                if (rdo.hisBill != null)
                {
                    string amount = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.rdo.hisBill.AMOUNT));
                    string amountSeparate = Inventec.Common.Number.Convert.NumberToStringRoundMax4(this.rdo.hisBill.AMOUNT);
                    SetSingleKey(new KeyValue(Mps000104ExtendSingleKey.AMOUNT, amountSeparate));
                    SetSingleKey(new KeyValue(Mps000104ExtendSingleKey.AMOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(amount)));
                    SetSingleKey(new KeyValue(Mps000104ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, UppercaseFirst(Inventec.Common.String.Convert.CurrencyToVneseString(amount))));

                    string exemptionSeparate = Inventec.Common.Number.Convert.NumberToStringRoundMax4((this.rdo.hisBill.EXEMPTION ?? 0) * 100 / this.rdo.hisBill.AMOUNT);
                    SetSingleKey(new KeyValue(Mps000104ExtendSingleKey.HEIN_RATIO, exemptionSeparate));

                }
                AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO);
                AddObjectKeyIntoListkey<V_HIS_BILL>(rdo.hisBill, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

    }
}
