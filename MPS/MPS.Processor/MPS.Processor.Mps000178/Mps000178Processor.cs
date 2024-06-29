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
using MPS.Processor.Mps000178.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000178
{
    class Mps000178Processor : AbstractProcessor
    {
        Mps000178PDO rdo;

        public Mps000178Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000178PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentPatient.PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000178ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);


                Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.treatment4.TREATMENT_CODE);
                barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatmentCode.IncludeLabel = false;
                barcodeTreatmentCode.Width = 120;
                barcodeTreatmentCode.Height = 40;
                barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatmentCode.IncludeLabel = true;
                dicImage.Add(Mps000178ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);
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
                SetSingleKey();


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

        private void SetSingleKey()
        {
            try
            {
                if (rdo.currentPatient != null)
                {
                    SetSingleKey(new KeyValue(Mps000178ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentPatient.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000178ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.currentPatient.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000178ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.currentPatient.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000178ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.currentPatient.DOB)));
                    SetSingleKey(new KeyValue(Mps000178ExtendSingleKey.DOB_STR, rdo.currentPatient.DOB.ToString().Substring(0, 4)));

                    if (!String.IsNullOrEmpty(rdo.currentPatient.AVATAR_URL))
                    {
                        MemoryStream ms = Inventec.Fss.Client.FileDownload.GetFile(rdo.currentPatient.AVATAR_URL);
                        if (ms != null)
                        {
                            SetSingleKey(new KeyValue(Mps000178ExtendSingleKey.IMAGE_AVATAR, ms.ToArray()));
                            ms.Dispose();
                        }
                    }
                }

                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.currentPatient);

                if (rdo.patientTypeAlter != null)
                {
                    SetSingleKey(new KeyValue(Mps000178ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.patientTypeAlter.ADDRESS));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.currentPatient, false);
                }
                if (rdo.treatment4 != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_4>(rdo.treatment4, false);
                }
                if(rdo.departmentTran != null)
                {
                    SetSingleKey(new KeyValue(Mps000178ExtendSingleKey.TRAN_DEPARTMENT_NAME, rdo.departmentTran.DEPARTMENT_NAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
