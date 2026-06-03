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
using Inventec.Common.QRCoder;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000515.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MPS.Processor.Mps000515
{
    class Mps000515Processor : AbstractProcessor
    {
        Mps000515PDO rdo;

        public Mps000515Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000515PDO)rdoBase;
        }

        /// <summary>
        /// Phiếu gộp dịch vụ khám nhiều phòng.
        /// Header tái sử dụng pattern Mps000001 (thông tin BN / đối tượng / điều trị),
        /// body chuyển từ 1 phòng sang danh sách N phòng khám đã đăng ký.
        /// </summary>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                ProcessPrintLogData();
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                SetBarcodeKey();
                SetSingleKey();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ExamRoom", rdo.ExamRooms);

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
                if (rdo.currentPatient != null && !String.IsNullOrEmpty(rdo.currentPatient.PATIENT_CODE))
                {
                    dicImage.Add(Mps000515ExtendSingleKey.BARCODE_PATIENT_CODE_STR, CreateBarcode(rdo.currentPatient.PATIENT_CODE));
                }

                if (rdo.currentTreatment != null && !String.IsNullOrEmpty(rdo.currentTreatment.TREATMENT_CODE))
                {
                    dicImage.Add(Mps000515ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, CreateBarcode(rdo.currentTreatment.TREATMENT_CODE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Inventec.Common.BarcodeLib.Barcode CreateBarcode(string value)
        {
            Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode(value);
            barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
            barcode.Width = 120;
            barcode.Height = 40;
            barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
            barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
            barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
            barcode.IncludeLabel = true;
            return barcode;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.currentPatient != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.currentPatient, false);
                    SetQrCodePatient();
                }

                if (rdo.currentTreatment != null)
                {
                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentTreatment, false);
                }

                if (rdo.PatyAlterBhyt != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);
                    SetSingleKey(new KeyValue(Mps000515ExtendSingleKey.FROM_DATE_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000515ExtendSingleKey.TO_DATE_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000515ExtendSingleKey.HEIN_CARD_NUMBER_SEPERATOR,
                        GlobalQuery.TrimHeinCardNumber(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER)));
                    SetSingleKey(new KeyValue(Mps000515ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.PatyAlterBhyt.ADDRESS));
                }

                SetSingleKey(new KeyValue(Mps000515ExtendSingleKey.LOGIN_USER_NAME,
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                SetSingleKey(new KeyValue(Mps000515ExtendSingleKey.LOGIN_LOGIN_NAME,
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()));

                SetSingleKey(new KeyValue(Mps000515ExtendSingleKey.GATE, rdo.Gate));
                SetSingleKey(new KeyValue(Mps000515ExtendSingleKey.PRINT_TIME_FULL_STR,
                    GlobalQuery.GetCurrentTimeSeparateBeginTime(DateTime.Now)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetQrCodePatient()
        {
            try
            {
                List<string> infos = new List<string>();
                infos.Add(rdo.currentPatient.PATIENT_CODE);
                infos.Add(rdo.currentPatient.VIR_PATIENT_NAME);
                infos.Add(rdo.currentPatient.GENDER_NAME);
                infos.Add(rdo.currentPatient.DOB.ToString().Length >= 4 ? rdo.currentPatient.DOB.ToString().Substring(0, 4) : "");
                infos.Add(rdo.currentTreatment != null ? rdo.currentTreatment.TREATMENT_CODE : "");

                string totalInfo = string.Join("\t", infos);
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(totalInfo, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);
                SetSingleKey(Mps000515ExtendSingleKey.QRCODE_PATIENT, qrCodeImage);
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
                if (rdo.currentTreatment != null)
                {
                    log = "Mã điều trị: " + rdo.currentTreatment.TREATMENT_CODE;
                }
                if (rdo.ExamRooms != null)
                {
                    log += " , Số phòng khám: " + rdo.ExamRooms.Count;
                }
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
                if (rdo != null && rdo.currentTreatment != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.currentTreatment.TREATMENT_CODE;
                    string roomCodes = "";
                    if (rdo.ExamRooms != null && rdo.ExamRooms.Count > 0)
                    {
                        roomCodes = "ROOM_CODES:" + string.Join("-", rdo.ExamRooms
                            .Select(o => o.ROOM_CODE)
                            .Where(o => !string.IsNullOrEmpty(o)));
                    }
                    result = String.Format("{0} {1} {2}", printTypeCode, treatmentCode, roomCodes);
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
