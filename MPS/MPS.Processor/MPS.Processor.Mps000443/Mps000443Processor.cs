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
using MPS.ProcessorBase.Core;
using System;
using MPS.Processor.Mps000443.PDO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using MPS.Processor.Mps000443.PDO;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000443
{
    public partial class Mps000443Processor : AbstractProcessor
    {
        Mps000443PDO rdo;

        public Mps000443Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000443PDO)rdoBase;
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
                SetBarcodeKey();
                SetSingleKey();

                if (rdo.ListhisVaccApp == null || rdo.ListhisVaccApp.Count < 1)
                {
                    rdo.ListhisVaccApp = new List<V_HIS_VACC_APPOINTMENT>();
                }
                objectTag.AddObjectData(store, "vaccAppointment", rdo.ListhisVaccApp);

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
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.TDL_PATIENT_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vaccinationExam.TDL_PATIENT_DOB)));
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.TDL_PATIENT_NAME, rdo.vaccinationExam.TDL_PATIENT_NAME));
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.TDL_PATIENT_CODE, rdo.vaccinationExam.TDL_PATIENT_CODE));
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.TDL_PATIENT_GENDER_NAME, rdo.vaccinationExam.TDL_PATIENT_GENDER_NAME));
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.TDL_PATIENT_ADDRESS, rdo.vaccinationExam.TDL_PATIENT_ADDRESS));
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.EXECUTE_ROOM_NAME, rdo.vaccinationExam.EXECUTE_ROOM_NAME));
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.EXECUTE_ROOM_CODE, rdo.vaccinationExam.EXECUTE_ROOM_CODE));
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.VACCINATION_EXAM_CODE, rdo.vaccinationExam.VACCINATION_EXAM_CODE));
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.EXECUTE_LOGINNAME, rdo.vaccinationExam.EXECUTE_LOGINNAME));
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.EXECUTE_USERNAME, rdo.vaccinationExam.EXECUTE_USERNAME));
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.ADVISE, rdo.vaccinationExam.ADVISE));
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.NOTE, rdo.vaccinationExam.NOTE));
                long apPointmentTime = rdo.vaccinationExam.APPOINTMENT_TIME ?? 0;
                SetSingleKey(new KeyValue(Mps000443ExtendSingleKey.APPOINTMENT_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(apPointmentTime)));


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (!String.IsNullOrEmpty(rdo.vaccinationExam.VACCINATION_EXAM_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.vaccinationExam.VACCINATION_EXAM_CODE);
                        barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatientCode.IncludeLabel = false;
                        barcodePatientCode.Width = 120;
                        barcodePatientCode.Height = 40;
                        barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatientCode.IncludeLabel = true;

                        dicImage.Add(Mps000443ExtendSingleKey.VACCINATION_EXAM_CODE_BAR, barcodePatientCode);
                    }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
