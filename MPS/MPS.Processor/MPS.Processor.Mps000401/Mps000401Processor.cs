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
using FlexCel.Report;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000401.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000401
{
    public class Mps000401Processor : AbstractProcessor
    {
        internal Mps000401PDO rdo;
        public Mps000401Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000401PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo._Treatment.TDL_PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000401ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo._Treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000401ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                Inventec.Common.Logging.LogSystem.Debug("Mps000401 ------ ProcessData----1");

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                SetBarcodeKey();

                SetSingleKey();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.templateType), this.templateType));
                result = (this.templateType == ProcessorBase.PrintConfig.TemplateType.Excel) ? ProcessDataExcel() : ((this.templateType == ProcessorBase.PrintConfig.TemplateType.Word) ? ProcessDataWord() : ProcessDataXtraReport());

                Inventec.Common.Logging.LogSystem.Debug("Mps000401 ------ ProcessData----2");
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private bool ProcessDataExcel()
        {
            bool success = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private bool ProcessDataXtraReport()
        {
            bool success = false;
            try
            {
                Inventec.Common.XtraReportExport.ProcessSingleTag singleTag = new Inventec.Common.XtraReportExport.ProcessSingleTag();
                Inventec.Common.XtraReportExport.ProcessObjectTag objectTag = new Inventec.Common.XtraReportExport.ProcessObjectTag();

                success = xtraReportStore.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                success = success && singleTag.ProcessData(xtraReportStore, singleValueDictionary);
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private bool ProcessDataWord()
        {
            bool success = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessDataWord.1");
                Inventec.Common.TemplaterExport.ProcessSingleTag singleTag = new Inventec.Common.TemplaterExport.ProcessSingleTag();
                Inventec.Common.TemplaterExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.TemplaterExport.ProcessBarCodeTag();
                Inventec.Common.TemplaterExport.ProcessObjectTag objectTag = new Inventec.Common.TemplaterExport.ProcessObjectTag();

                success = templaterExportStore.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                success = success && objectTag.AddObjectData(templaterExportStore, rdo.Patient);
                Inventec.Common.Logging.LogSystem.Debug("ProcessDataWord.2");
                if (rdo.PatientTypeAlter != null)
                    success = success && objectTag.AddObjectData(templaterExportStore, rdo.PatientTypeAlter);
                if (rdo._WorkPlaceSDO != null)
                    success = success && objectTag.AddObjectData(templaterExportStore, rdo._WorkPlaceSDO);
                if (singleValueDictionary != null && singleValueDictionary.Count > 0)
                    success = success && singleTag.ProcessData(templaterExportStore, singleValueDictionary);
                Inventec.Common.Logging.LogSystem.Debug("ProcessDataWord.3");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo._Treatment != null)
                {
                   
                }
                if (rdo.Patient != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
                    SetSingleKey(new KeyValue(Mps000401ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB)));
                    SetSingleKey(new KeyValue(Mps000401ExtendSingleKey.D_O_B, rdo.Patient.DOB.ToString().Substring(0, 4)));
                    SetSingleKey(new KeyValue(Mps000401ExtendSingleKey.PHONE, rdo.Patient.PHONE));
                }
              
                if (rdo._WorkPlaceSDO != null)
                {
                    SetSingleKey(new KeyValue(Mps000401ExtendSingleKey.DEPARTMENT_NAME, rdo._WorkPlaceSDO.DepartmentName));
                    SetSingleKey(new KeyValue(Mps000401ExtendSingleKey.ROOM_NAME, rdo._WorkPlaceSDO.RoomName));
                    AddObjectKeyIntoListkey<Mps000401SingleKey>(rdo._WorkPlaceSDO, false);
                }
                if (rdo.PatientTypeAlter != null)
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatientTypeAlter);
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
                log = "Mã điều trị: " + rdo._Treatment.TREATMENT_CODE;
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
                if (rdo != null && rdo._Treatment != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo._Treatment.TREATMENT_CODE;
                    result = String.Format("{0} {1}", printTypeCode, treatmentCode);
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
