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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000399.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000399.ADO;

namespace MPS.Processor.Mps000399
{
    public partial class Mps000399Processor : AbstractProcessor
    {
        private PatientADO patientADO { get; set; }
        Mps000399PDO rdo;
        public Mps000399Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000399PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.Treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000399ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                DataInputProcess();
                ProcessSingleKey();
                SetBarcodeKey();

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

        void ProcessSingleKey()
        {
            try
            {
                SetSingleKey(new KeyValue(Mps000399ExtendSingleKey.RATIO_STR, rdo.SingleKeyValue.RatioText));
                SetSingleKey(new KeyValue(Mps000399ExtendSingleKey.EXECUTE_DEPARTMENT_NAME, rdo.SingleKeyValue.ExecuteDepartmentName));
                SetSingleKey(new KeyValue(Mps000399ExtendSingleKey.EXECUTE_ROOM_NAME, rdo.SingleKeyValue.ExecuteRoomName));

                if (rdo.Treatment != null)
                {
                    if (rdo.Treatment.CLINICAL_IN_TIME != null)
                    {

                        SetSingleKey((new KeyValue(Mps000399ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value))));

                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000399ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.IN_TIME))));
                    }
                }

                if (!String.IsNullOrEmpty(rdo.Treatment.TRANSFER_IN_MEDI_ORG_CODE))
                {
                    SetSingleKey((new KeyValue(Mps000399ExtendSingleKey.ICD_NGT_TEXT, rdo.Treatment.TRANSFER_IN_ICD_NAME)));
                }
                SetSingleKey((new KeyValue(Mps000399ExtendSingleKey.ICD_DEPARTMENT_TRAN, rdo.SingleKeyValue.Icd_Name)));

                SetSingleKey((new KeyValue(Mps000399ExtendSingleKey.LOGIN_USER_NAME, rdo.SingleKeyValue.Username)));
                SetSingleKey((new KeyValue(Mps000399ExtendSingleKey.LOGIN_NAME, rdo.SingleKeyValue.LoginName)));
                SetSingleKey((new KeyValue(Mps000399ExtendSingleKey.END_DEPARTMENT_NAME, rdo.SingleKeyValue.END_DEPARTMENT_NAME)));


                if (rdo._currentPatient != null)
                {
                    SetSingleKey((new KeyValue(Mps000399ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo._currentPatient.DOB))));
                }

                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.Treatment, false);
                AddObjectKeyIntoListkey<PatientADO>(patientADO, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
