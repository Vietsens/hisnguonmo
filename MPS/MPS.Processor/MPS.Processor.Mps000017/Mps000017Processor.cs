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
using AutoMapper;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000017.PDO;
using MPS.ProcessorBase.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000017
{
    public class Mps000017Processor : AbstractProcessor
    {
        Mps000017PDO rdo;
        public Mps000017Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000017PDO)rdoBase;
        }

        public void SetBarcodeKey()
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

                dicImage.Add(Mps000017ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReqPrint.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000017ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                    MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = new MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN();
                    departmentTran = rdo.lstDepartmentTran.FirstOrDefault();
                    AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(departmentTran);
                }

                SetSingleKey(new KeyValue(Mps000017ExtendSingleKey.BED_ROOM_NAME, rdo.bebRoomName));
                SetSingleKey(new KeyValue(Mps000017ExtendSingleKey.SERVICE_NAME, rdo.sereServs.SERVICE_NAME));

                AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(rdo.sereServs);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ServiceReqPrint);
                AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(rdo.TranPaties, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
                if (rdo.Patient != null)
                {
                    SetSingleKey(new KeyValue(Mps000017ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB)));
                    SetSingleKey(new KeyValue(Mps000017ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.Patient.DOB))));
                    SetSingleKey((new KeyValue(Mps000017ExtendSingleKey.CMND_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Patient.CMND_DATE ?? 0))));
                    SetSingleKey((new KeyValue(Mps000017ExtendSingleKey.D_O_B, rdo.Patient.DOB.ToString().Substring(0, 4))));
                    SetSingleKey((new KeyValue(Mps000017ExtendSingleKey.GENDER_MALE, rdo.Patient.GENDER_CODE == rdo.genderCode__Male ? "X" : "")));
                    SetSingleKey((new KeyValue(Mps000017ExtendSingleKey.GENDER_FEMALE, rdo.Patient.GENDER_CODE == rdo.genderCode__FeMale ? "X" : "")));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
