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
using MPS.Processor.Mps000091.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000091
{
    class Mps000091Processor : AbstractProcessor
    {
        Mps000091PDO rdo;
        public Mps000091Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000091PDO)rdoBase;
        }
        private void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.depositReq.TREATMENT_CODE);
                barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatmentCode.IncludeLabel = false;
                barcodeTreatmentCode.Width = 120;
                barcodeTreatmentCode.Height = 40;
                barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatmentCode.IncludeLabel = true;

                dicImage.Add(Mps000091ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatmentCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetBarcodeKey();
                this.SetSingleKey();
                SetSingleKeyQrCode();
                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
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

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = LogDataTransaction(rdo.depositReq.TREATMENT_CODE, rdo.depositReq.DEPOSIT_REQ_CODE, "");
                log += "SoTien: " + rdo.depositReq.AMOUNT;
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
                if (rdo != null && rdo.depositReq != null)
                    result = String.Format("{0}_{1}_{2}", "Mps000091", rdo.depositReq.TREATMENT_CODE, rdo.depositReq.DEPOSIT_REQ_CODE);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void SetSingleKeyQrCode()
        {
            try
            {
                if (rdo.transReq != null && rdo.lstConfig != null && rdo.lstConfig.Count > 0)
                {
                    var data = HIS.Desktop.Common.BankQrCode.QrCodeProcessor.CreateQrImage(rdo.transReq, rdo.lstConfig);
                    foreach (var item in data)
                    {
                        SetSingleKey(new KeyValue(item.Key, item.Value));
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
                if (rdo.depositReq != null)
                {
                    SetSingleKey(new KeyValue(Mps000091ExtendSingleKey.STR_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundMax4(rdo.depositReq.AMOUNT)));
                    SetSingleKey(new KeyValue(Mps000091ExtendSingleKey.STR_AMOUNT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(rdo.depositReq.AMOUNT).ToString())));
                }
                if (rdo.patyAlter != null)
                {
                    SetSingleKey(new KeyValue(Mps000091ExtendSingleKey.HEIN_CARD_FROM_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.patyAlter.HEIN_CARD_FROM_TIME.ToString())));
                    SetSingleKey(new KeyValue(Mps000091ExtendSingleKey.HEIN_CARD_TO_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.patyAlter.HEIN_CARD_TO_TIME.ToString())));
                }
                AddObjectKeyIntoListkey<PatientADO>(rdo.currentPatient);
                AddObjectKeyIntoListkey<V_HIS_DEPOSIT_REQ>(rdo.depositReq, false);
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.patyAlter, false);
                if (rdo.transReq != null)
                    SetSingleKey(new KeyValue(Mps000091ExtendSingleKey.PAYMENT_AMOUNT, rdo.transReq.AMOUNT));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
