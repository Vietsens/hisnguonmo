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
using MPS.Processor.Mps000016.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;

namespace MPS.Processor.Mps000016
{
    class Mps000016Processor : AbstractProcessor
    {
        Mps000016PDO rdo;
        public Mps000016Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000016PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000016ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReqPrint.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000016ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                SetSingleKey();
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

        void SetSingleKey()
        {
            try
            {

                if (rdo.lstDepartmentTran != null && rdo.lstDepartmentTran.Count > 0)
                {
                    V_HIS_DEPARTMENT_TRAN departmentTran = new V_HIS_DEPARTMENT_TRAN();
                    departmentTran = rdo.lstDepartmentTran.FirstOrDefault();
                    AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(departmentTran);
                }
                SetSingleKey(new KeyValue(Mps000016ExtendSingleKey.BED_ROOM_NAME, rdo.bebRoomName));
                SetSingleKey(new KeyValue(Mps000016ExtendSingleKey.SERVICE_NAME, rdo.sereServs.SERVICE_NAME));
                //SetSingleKey(new KeyValue(Mps000016ExtendSingleKey.CONCLUDE, sereServs.CONCLUDE));
                //SetSingleKey(new KeyValue(Mps000016ExtendSingleKey.DESCRIPTION, sereServs.DESCRIPTION));
                //SetSingleKey(new KeyValue(Mps000016ExtendSingleKey.VIR_PATIENT_NAME, sereServs.VIR_PATIENT_NAME));

                AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(rdo.sereServs);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReqPrint);
                AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(rdo.TranPaties, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient, false);

                if (rdo.Patient != null)
                {
                    SetSingleKey(new KeyValue(Mps000016ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB)));
                    SetSingleKey(new KeyValue(Mps000016ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.Patient.DOB))));
                    SetSingleKey((new KeyValue(Mps000016ExtendSingleKey.CMND_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Patient.CMND_DATE ?? 0))));
                    SetSingleKey((new KeyValue(Mps000016ExtendSingleKey.D_O_B, rdo.Patient.DOB.ToString().Substring(0, 4))));
                    SetSingleKey((new KeyValue(Mps000016ExtendSingleKey.GENDER_MALE, rdo.Patient.GENDER_CODE == rdo.genderCode__Male ? "X" : "")));
                    SetSingleKey((new KeyValue(Mps000016ExtendSingleKey.GENDER_FEMALE, rdo.Patient.GENDER_CODE == rdo.genderCode__FeMale ? "X" : "")));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
