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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000315.ADO;
using MPS.Processor.Mps000315.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000315
{
    class Mps000315Processor : AbstractProcessor
    {
        Mps000315PDO rdo;

        List<V_HIS_SERE_SERV> _ServiceTypes { get; set; }
        List<ServiceReqAdo> ServiceReqAdos { get; set; }
        List<TreatmentAdo> TreatmentAdos { get; set; }
        List<SereServResultAdo> SereServResultAdos { get; set; }
        HIS_KSK_GENERAL hkg { get; set; }
        public Mps000315Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000315PDO)rdoBase;
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
                SetBarcodeKey();
                SetSingleKey();
                SetImageKey();
                this.SetSignatureKeyImageByCFG();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "Treatment", TreatmentAdos);
                objectTag.AddObjectData(store, "ServiceType", _ServiceTypes);
                if (rdo._KSK_SereServs == null)
                {
                    rdo._KSK_SereServs = new List<V_HIS_SERE_SERV>();
                }
                objectTag.AddObjectData(store, "SereServ", rdo._KSK_SereServs);
                objectTag.AddObjectData(store, "ServiceReq", ServiceReqAdos);
                if (rdo._KSK_BedLogs == null)
                {
                    rdo._KSK_BedLogs = new List<V_HIS_BED_LOG>();
                }
                objectTag.AddObjectData(store, "BedLog", rdo._KSK_BedLogs);
                if (rdo._KSK_PatientTypeAlters == null)
                {
                    rdo._KSK_PatientTypeAlters = new List<V_HIS_PATIENT_TYPE_ALTER>();
                }
                objectTag.AddObjectData(store, "PatientTypeAlter", rdo._KSK_PatientTypeAlters);
                if (rdo._KSK_SereServExts == null)
                {
                    rdo._KSK_SereServExts = new List<HIS_SERE_SERV_EXT>();
                }
                objectTag.AddObjectData(store, "SereServExt", rdo._KSK_SereServExts);
                if (rdo._KSK_Dhsts == null)
                {
                    rdo._KSK_Dhsts = new List<V_HIS_DHST>();
                }
                objectTag.AddObjectData(store, "Dhst", rdo._KSK_Dhsts);
                if (rdo._KSK_SereServTeins == null)
                {
                    rdo._KSK_SereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                }
                objectTag.AddObjectData(store, "SereServTeins", rdo._KSK_SereServTeins);
                objectTag.AddObjectData(store, "SereServResult", SereServResultAdos);
                if(rdo._KskDriver == null)
                {
                    rdo._KskDriver = new List<HIS_KSK_DRIVER>();
                }
                objectTag.AddObjectData(store, "KskDriver", rdo._KskDriver);

                //objectTag.AddRelationship(store, "Treatment", "ServiceType", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "Treatment", "ServiceReq", "ID", "TREATMENT_ID");
                objectTag.AddRelationship(store, "Treatment", "BedLog", "ID", "TREATMENT_ID");
                objectTag.AddRelationship(store, "Treatment", "PatientTypeAlter", "ID", "TREATMENT_ID");
                objectTag.AddRelationship(store, "Treatment", "SereServ", "ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "ServiceType", "SereServ", "TDL_SERVICE_TYPE_ID", "TDL_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "Treatment", "Dhst", "ID", "TREATMENT_ID");

                objectTag.AddRelationship(store, "SereServ", "SereServExt", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "SereServ", "SereServTeins", "ID", "SERE_SERV_ID");
                objectTag.AddRelationship(store, "SereServ", "SereServResult", "ID", "SERE_SERV_ID");

                objectTag.AddRelationship(store, "KskDriver", "ServiceReq", "SERVICE_REQ_ID", "ID");
                objectTag.AddRelationship(store, "KskDriver", "Treatment", "TDL_TREATMENT_ID", "ID");

                objectTag.AddRelationship(store, "KskGeneral", "Dhst", "DHST_ID", "ID");


                if (rdo._KSK_HealthExamRank == null)
                {
                    rdo._KSK_HealthExamRank = new List<HIS_HEALTH_EXAM_RANK>();
                }

                objectTag.AddObjectData(store, "KskRank", rdo._KSK_HealthExamRank);
                objectTag.AddObjectData(store, "Patient", rdo._KSK_Patients);
                objectTag.AddObjectData(store, "KskGeneral", rdo._KskGeneral);

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
                _ServiceTypes = new List<V_HIS_SERE_SERV>();
                if (rdo._KSK_SereServs != null && rdo._KSK_SereServs.Count > 0)
                {
                    var dataGroups = rdo._KSK_SereServs.GroupBy(p => p.TDL_SERVICE_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        _ServiceTypes.Add(item.FirstOrDefault());
                    }
                    _ServiceTypes = _ServiceTypes.OrderBy(p => p.TDL_SERVICE_TYPE_ID).ToList();

                }

                ServiceReqAdos = new List<ServiceReqAdo>();
                if (rdo._KSK_ServiceReqs != null && rdo._KSK_ServiceReqs.Count > 0)
                {
                    foreach (var item in rdo._KSK_ServiceReqs)
                    {
                        ServiceReqAdo ado = new ServiceReqAdo();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqAdo>(ado, item);
                        if (rdo._KSK_HealthExamRank != null && rdo._KSK_HealthExamRank.Count > 0 && item.HEALTH_EXAM_RANK_ID.HasValue)
                        {
                            var rank = rdo._KSK_HealthExamRank.FirstOrDefault(o => o.ID == item.HEALTH_EXAM_RANK_ID);
                            if (rank != null)
                            {
                                ado.HEALTH_EXAM_RANK_NAME = rank.HEALTH_EXAM_RANK_NAME;
                            }
                        }

                        ServiceReqAdos.Add(ado);
                    }
                    //
                    if(rdo._KSK_ServiceReqs.Count == 1)
                        AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo._KSK_ServiceReqs[0], false);
                }
                //set single key
                if (rdo._KskGeneral != null && rdo._KskGeneral.Count > 0)
                {
                    if (rdo._KskGeneral.Count == 1)
                        AddObjectKeyIntoListkey<HIS_KSK_GENERAL>(rdo._KskGeneral[0], false);
                }

                TreatmentAdos = new List<TreatmentAdo>();
                if (rdo._KSK_Treatments != null && rdo._KSK_Treatments.Count > 0)
                {
                    foreach (var item in rdo._KSK_Treatments)
                    {
                        TreatmentAdo ado = new TreatmentAdo();
                        Inventec.Common.Mapper.DataObjectMapper.Map<TreatmentAdo>(ado, item);
                        if (rdo._KSK_Patients != null && rdo._KSK_Patients.Count > 0)
                        {
                            var patient = rdo._KSK_Patients.FirstOrDefault(o => o.ID == item.PATIENT_ID);
                            if (patient != null)
                            {
                                ado.EMAIL = patient.EMAIL;
                            }
                        }

                        TreatmentAdos.Add(ado);
                    }
                }

                SereServResultAdos = new List<SereServResultAdo>();
                if (rdo._KSK_SereServExts != null && rdo._KSK_SereServExts.Count > 0)
                {
                    foreach (var item in rdo._KSK_SereServExts)
                    {
                        SereServResultAdo ado = new SereServResultAdo();
                        ado.SERE_SERV_ID = item.SERE_SERV_ID;
                        ado.CONCLUDE = item.CONCLUDE;
                        ado.DESCRIPTION = item.DESCRIPTION;
                        ado.NOTE = item.NOTE;
                        SereServResultAdos.Add(ado);
                    }
                }

                SereServResultAdos = new List<SereServResultAdo>();
                if (rdo._KSK_SereServTeins != null && rdo._KSK_SereServTeins.Count > 0)
                {
                    rdo._KSK_SereServTeins = rdo._KSK_SereServTeins.OrderBy(o => o.SERE_SERV_ID).ThenBy(o => o.NUM_ORDER ?? 9999).ToList();
                    foreach (var item in rdo._KSK_SereServTeins)
                    {
                        SereServResultAdo ado = new SereServResultAdo();
                        ado.SERE_SERV_ID = item.SERE_SERV_ID;
                        ado.VALUE = item.VALUE;
                        ado.TEST_INDEX_NAME = item.TEST_INDEX_NAME;
                        ado.TEST_INDEX_CODE = item.TEST_INDEX_CODE;
                        //ado.DESCRIPTION = item.DESCRIPTION;
                        SereServResultAdos.Add(ado);
                    }
                }
                // AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo._KSK_Treatments);
                
                if (rdo._KSK_Dhsts != null)
                {
                    SetSingleKey((new KeyValue(Mps000315ExtendSingleKey.DHST_LOGINNAME, rdo._KSK_Dhsts.FirstOrDefault().EXECUTE_LOGINNAME)));
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
                //Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                //barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodeTreatment.IncludeLabel = false;
                //barcodeTreatment.Width = 120;
                //barcodeTreatment.Height = 40;
                //barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodeTreatment.IncludeLabel = true;

                // dicImage.Add(Mps000315ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                if (TreatmentAdos != null && TreatmentAdos.Count > 0)
                {
                    foreach (var item in TreatmentAdos)
                    {
                        if (!string.IsNullOrEmpty(item.TDL_PATIENT_AVATAR_URL))
                        {
                            SetSingleImage(item, item.TDL_PATIENT_AVATAR_URL);
                        }
                    }
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
                if (TreatmentAdos != null && TreatmentAdos.Count == 1)
                {
                    var Treatment = TreatmentAdos.FirstOrDefault();

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
                if (TreatmentAdos != null && TreatmentAdos.Count == 1)
                {
                    var Treatment = TreatmentAdos.FirstOrDefault();

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
