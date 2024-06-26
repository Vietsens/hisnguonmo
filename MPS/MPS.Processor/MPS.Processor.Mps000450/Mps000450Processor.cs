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
using MPS.Processor.Mps000450.ADO;
using MPS.Processor.Mps000450.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000450
{
    class Mps000450Processor : AbstractProcessor
    {
        Mps000450PDO rdo;

        List<V_HIS_SERE_SERV> _ServiceTypes { get; set; }
        List<ServiceReqAdo> ServiceReqAdos { get; set; }
        TreatmentAdo TreatmentAdos { get; set; }
        List<SereServResultAdo> SereServResultAdos { get; set; }

        public Mps000450Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000450PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                SetBarcodeKey();
                
                SetImageKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                
                //objectTag.AddObjectData(store, "Treatment", TreatmentAdos);
                ////objectTag.AddObjectData(store, "ServiceType", _ServiceTypes);
                //objectTag.AddObjectData(store, "SereServ", rdo._KSK_SereServs);
                ////objectTag.AddObjectData(store, "ServiceReq", ServiceReqAdos);
                //objectTag.AddObjectData(store, "BedLog", rdo._KSK_BedLogs);
                //objectTag.AddObjectData(store, "PatientTypeAlter", rdo._KSK_PatientTypeAlters);
                //objectTag.AddObjectData(store, "SereServExt", rdo._KSK_SereServExts);
                //objectTag.AddObjectData(store, "Dhst", rdo._KSK_Dhsts);
                //objectTag.AddObjectData(store, "SereServTeins", rdo._KSK_SereServTeins);
                ////objectTag.AddObjectData(store, "SereServResult", SereServResultAdos);

                ////objectTag.AddRelationship(store, "Treatment", "ServiceType", "ID", "TDL_TREATMENT_ID");
                //objectTag.AddRelationship(store, "Treatment", "ServiceReq", "ID", "TREATMENT_ID");
                //objectTag.AddRelationship(store, "Treatment", "BedLog", "ID", "TREATMENT_ID");
                //objectTag.AddRelationship(store, "Treatment", "PatientTypeAlter", "ID", "TREATMENT_ID");
                //objectTag.AddRelationship(store, "Treatment", "SereServ", "ID", "TDL_TREATMENT_ID");
                //objectTag.AddRelationship(store, "ServiceType", "SereServ", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                //objectTag.AddRelationship(store, "Treatment", "Dhst", "ID", "TREATMENT_ID");

                //objectTag.AddRelationship(store, "SereServ", "SereServExt", "ID", "SERE_SERV_ID");
                //objectTag.AddRelationship(store, "SereServ", "SereServTeins", "ID", "SERE_SERV_ID");
                //objectTag.AddRelationship(store, "SereServ", "SereServResult", "ID", "SERE_SERV_ID");

                //if (rdo._KSK_HealthExamRank == null)
                //{
                //    rdo._KSK_HealthExamRank = new List<HIS_HEALTH_EXAM_RANK>();
                //}

                //objectTag.AddObjectData(store, "KskRank", rdo._KSK_HealthExamRank);
                //objectTag.AddObjectData(store, "Patient", rdo._KSK_Patients);

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
               
                TreatmentAdos = new TreatmentAdo();
                if (rdo._KSK_Treatments != null )
                {
                    
                        TreatmentAdo ado = new TreatmentAdo();
                        Inventec.Common.Mapper.DataObjectMapper.Map<TreatmentAdo>(ado, rdo._KSK_Treatments);
                        if (rdo._KSK_Patients != null )
                        {
                            var patient = rdo._KSK_Patients;
                            if (patient != null)
                            {
                                ado.EMAIL = patient.EMAIL;
                            }
                        }

                        TreatmentAdos = ado;
                        AddObjectKeyIntoListkey<TreatmentAdo>(TreatmentAdos, false);
                }

                if (rdo._KSK_Patients != null)
                {
                    AddObjectKeyIntoListkey<HIS_PATIENT>(rdo._KSK_Patients, false);
                }
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetBarcodeKey()
        {
            try
            {
                if (TreatmentAdos != null)
                {

                    if (!String.IsNullOrEmpty(TreatmentAdos.TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(TreatmentAdos.TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;

                        dicImage.Add(Mps000450ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);

                        string treatmentCode = TreatmentAdos.TREATMENT_CODE.Substring(TreatmentAdos.TREATMENT_CODE.Length - 5, 5);
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment5 = new Inventec.Common.BarcodeLib.Barcode(treatmentCode);
                        barcodeTreatment5.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment5.IncludeLabel = false;
                        barcodeTreatment5.Width = 120;
                        barcodeTreatment5.Height = 40;
                        barcodeTreatment5.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment5.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment5.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment5.IncludeLabel = true;

                        dicImage.Add(Mps000450ExtendSingleKey.TREATMENT_CODE_BAR_5, barcodeTreatment5);
                    }

                    if (!string.IsNullOrWhiteSpace(rdo._KSK_Patients.PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo._KSK_Patients.PATIENT_CODE);
                        barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatient.IncludeLabel = false;
                        barcodePatient.Width = 120;
                        barcodePatient.Height = 40;
                        barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatient.IncludeLabel = true;

                        dicImage.Add(Mps000450ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatient);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetImageKey()
        {
            try
            {
                if (!string.IsNullOrEmpty(TreatmentAdos.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(TreatmentAdos, TreatmentAdos.TDL_PATIENT_AVATAR_URL);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetSingleImage(TreatmentAdo key, string imageUrl)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                if (stream != null)
                {
                    key.PATIENT_AVATAR_URL = stream.ToArray();
                }
                else
                {
                    key.PATIENT_AVATAR_URL = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                if (TreatmentAdos != null )
                {
                    var Treatment = TreatmentAdos;

                    log = "TREATMENT_CODE: " + Treatment.TREATMENT_CODE;

                    decimal SereServCount = 0;

                    foreach (var item in rdo._KSK_SereServs.ToList())
                    {
                        SereServCount += item.AMOUNT;
                    }

                    log += " SERE_SERV: " + SereServCount;
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
                if (TreatmentAdos != null)
                {
                    var Treatment = TreatmentAdos;

                    string treatmentCode = "TREATMENT_CODE: " + Treatment.TREATMENT_CODE;

                    decimal SereServCount = 0;

                    foreach (var item in rdo._KSK_SereServs.ToList())
                    {
                        SereServCount += item.AMOUNT;
                    }
                    string SereServ = " SERE_SERV: " + SereServCount;

                    result = String.Format("{0} {1} {2}", printTypeCode, treatmentCode, SereServ);
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
