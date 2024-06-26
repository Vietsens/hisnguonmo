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
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using MPS.Processor.Mps000449.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MPS.Processor.Mps000449.ADO;
namespace MPS.Processor.Mps000449
{
    class Mps000449Processor : AbstractProcessor
    {
        private Mps000449PDO rdo;
        List<KskGeneralAdo> KskGeneralAdos;
        List<DhstAdo> DhstAdos;
        List<SereServTeinAdo> SereServTeinAdos;
        public Mps000449Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            if (rdoBase != null && rdoBase is Mps000449PDO)
            {
                rdo = (Mps000449PDO)rdoBase;
            }
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
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                SetSingleKey();
                SetBarcodeKey();
                ProcessListData();
                ProcessListInput();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "ListSereServ02", rdo._ListSereServ02);
                objectTag.AddObjectData(store, "ListSereServXetNghiem", rdo._ListSereServXetNghiem);
                objectTag.AddObjectData(store, "ListSereServ", rdo._ListSereServ);
                objectTag.AddObjectData(store, "ListSereServTein", SereServTeinAdos);
                objectTag.AddObjectData(store, "Dhst", DhstAdos);
                objectTag.AddObjectData(store, "KSKGeneral", KskGeneralAdos);
                objectTag.AddObjectData(store, "ServiceReq", rdo.ServiceReq);
                objectTag.AddObjectData(store, "KskRank", rdo._ListHealthExamRank);
                objectTag.AddObjectData(store, "TestIndex", rdo._ListTestIndex);
                objectTag.AddObjectData(store, "SereServExt", rdo._ListSereServExt);

                objectTag.AddRelationship(store, "ServiceReq", "Dhst", "ID", "SERVICE_REQ_ID");
                objectTag.AddRelationship(store, "ServiceReq", "KSKGeneral", "ID", "SERVICE_REQ_ID");

                objectTag.AddRelationship(store, "ServiceReq", "ListSereServ02", "TREATMENT_ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "ServiceReq", "ListSereServTein", "TREATMENT_ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "ServiceReq", "ListSereServXetNghiem", "TREATMENT_ID", "TDL_TREATMENT_ID");
                objectTag.AddRelationship(store, "ListSereServXetNghiem", "ListSereServTein", "ID", "SERE_SERV_ID");


                objectTag.AddRelationship(store, "KSKGeneral", "Dhst", "DHST_ID", "ID");
                objectTag.AddRelationship(store, "KSKGeneral", "KskRank", "HEALTH_EXAM_RANK_ID", "ID");

                objectTag.AddRelationship(store, "ListSereServTein", "TestIndex", "TEST_INDEX_ID", "ID");

                //objectTag.AddRelationship(store, "ListSereServ02", "SereServExt", "ID", "SERE_SERV_ID");
                //objectTag.AddRelationship(store, "ListSereServ", "ListSereServTein", "ID", "SERE_SERV_ID");
                //objectTag.AddRelationship(store, "ListSereServ02", "ListSereServTein", "ID", "SERE_SERV_ID");


                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessListInput()
        {
            try
            {
                #region KSK
                KskGeneralAdos = new List<KskGeneralAdo>();
                if (rdo.KSKGeneral != null && rdo.KSKGeneral.Count > 0)
                {
                    foreach (var item in rdo.KSKGeneral)
                    {
                        KskGeneralAdo ado = new KskGeneralAdo();
                        Inventec.Common.Mapper.DataObjectMapper.Map<KskGeneralAdo>(ado, item);
                        if (item.HEALTH_EXAM_RANK_ID.HasValue)
                        {
                            var rank = rdo._ListHealthExamRank.FirstOrDefault(o => o.ID == item.HEALTH_EXAM_RANK_ID);
                            if (rank != null)
                            {
                                ado.HEALTH_EXAM_RANK_NAME = rank.HEALTH_EXAM_RANK_NAME;
                            }
                        }
                        KskGeneralAdos.Add(ado);
                    }
                }

                if (KskGeneralAdos != null && KskGeneralAdos.Count > 0)
                {
                    foreach (var item in rdo.ServiceReq)
                    {
                        var checkKsk = KskGeneralAdos.FirstOrDefault(o => o.SERVICE_REQ_ID == item.ID);
                        if (checkKsk == null)
                        {
                            KskGeneralAdo ado = new KskGeneralAdo();
                            ado.SERVICE_REQ_ID = item.ID;
                            KskGeneralAdos.Add(ado);
                        }
                    }

                }
                else
                {
                    if (rdo.ServiceReq != null && rdo.ServiceReq.Count > 0)
                    {
                        foreach (var item in rdo.ServiceReq)
                        {
                            KskGeneralAdo ado = new KskGeneralAdo();
                            ado.SERVICE_REQ_ID = item.ID;
                            KskGeneralAdos.Add(ado);
                        }
                    }
                }
                #endregion
                #region Dhst
                DhstAdos = new List<DhstAdo>();
                if (rdo.ServiceReq != null && rdo.ServiceReq.Count > 0)
                {
                    foreach (var item in rdo.ServiceReq)
                    {
                        if (KskGeneralAdos != null && KskGeneralAdos.Count > 0)
                        {
                            var check = KskGeneralAdos.FirstOrDefault(o => o.SERVICE_REQ_ID == item.ID);
                            if (check != null)
                            {
                                if (check.DHST_ID.HasValue)
                                {
                                    var dhstCheck = rdo._ListDhst.FirstOrDefault(o => o.ID == check.DHST_ID);
                                    if (dhstCheck != null)
                                    {
                                        DhstAdo ado = new DhstAdo();
                                        Inventec.Common.Mapper.DataObjectMapper.Map<DhstAdo>(ado, dhstCheck);
                                        ado.SERVICE_REQ_ID = item.ID;
                                        DhstAdos.Add(ado);
                                    }
                                }
                                else
                                {
                                    DhstAdo ado = new DhstAdo();
                                    ado.SERVICE_REQ_ID = item.ID;
                                    DhstAdos.Add(ado);
                                }
                            }
                        }
                        else
                        {
                            DhstAdo ado = new DhstAdo();
                            ado.SERVICE_REQ_ID = item.ID;
                            DhstAdos.Add(ado);
                        }
                    }
                }
                #endregion
                #region SereServTein
                SereServTeinAdos = new List<SereServTeinAdo>();
                if (rdo._ListTestIndex != null && rdo._ListTestIndex.Count > 0)
                {
                    foreach (var item in rdo._ListSereServTein)
                    {
                        SereServTeinAdo ado = new SereServTeinAdo();
                        Inventec.Common.Mapper.DataObjectMapper.Map<SereServTeinAdo>(ado, item);
                        if (item.TEST_INDEX_ID.HasValue)
                        {
                            var checkTest = rdo._ListTestIndex.FirstOrDefault(o => o.ID == item.TEST_INDEX_ID);
                            if (checkTest != null)
                            {
                                ado.TEST_INDEX_NAME = checkTest.TEST_INDEX_NAME;                                
                            }
                        }
                        SereServTeinAdos.Add(ado);
                    }
                }
                #endregion
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
                if (rdo.ServiceReq != null)
                {
                    //if (!String.IsNullOrEmpty(rdo.ServiceReq.TDL_TREATMENT_CODE))
                    //{
                    //    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.TDL_TREATMENT_CODE);
                    //    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    //    barcodeTreatmentCode.IncludeLabel = false;
                    //    barcodeTreatmentCode.Width = 120;
                    //    barcodeTreatmentCode.Height = 40;
                    //    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    //    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    //    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    //    barcodeTreatmentCode.IncludeLabel = true;

                    //    dicImage.Add(Mps000449ExtendSingleKey.TREATMENT_CODE_BARCODE, barcodeTreatmentCode);
                    //}

                    //if (!String.IsNullOrEmpty(rdo.ServiceReq.SERVICE_REQ_CODE))
                    //{
                    //    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.SERVICE_REQ_CODE);
                    //    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    //    barcodeTreatmentCode.IncludeLabel = false;
                    //    barcodeTreatmentCode.Width = 120;
                    //    barcodeTreatmentCode.Height = 40;
                    //    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    //    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    //    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    //    barcodeTreatmentCode.IncludeLabel = true;

                    //    dicImage.Add(Mps000449ExtendSingleKey.SERVICE_REQ_CODE_BARCODE, barcodeTreatmentCode);
                    //}

                    //if (!String.IsNullOrEmpty(rdo.ServiceReq.TDL_PATIENT_CODE))
                    //{
                    //    Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.ServiceReq.TDL_PATIENT_CODE);
                    //    barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    //    barcodePatient.IncludeLabel = false;
                    //    barcodePatient.Width = 120;
                    //    barcodePatient.Height = 40;
                    //    barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    //    barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    //    barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    //    barcodePatient.IncludeLabel = true;

                    //    dicImage.Add(Mps000449ExtendSingleKey.PATIENT_CODE_BARCODE, barcodePatient);
                    //}
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
                if (rdo.ServiceReq != null && rdo.ServiceReq.Count == 1)
                {
                    log = LogDataServiceReq(rdo.ServiceReq.FirstOrDefault().TDL_TREATMENT_CODE, rdo.ServiceReq.FirstOrDefault().SERVICE_REQ_CODE, "");
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
                if (rdo != null)
                {
                    if (rdo.ServiceReq != null && rdo.ServiceReq.Count == 1)
                    {
                        result = String.Format("{0}_{1}_{2}_{3}", this.printTypeCode, rdo.ServiceReq.FirstOrDefault().TDL_TREATMENT_CODE, rdo.ServiceReq.FirstOrDefault().EXECUTE_ROOM_ID, rdo.ServiceReq.FirstOrDefault().INTRUCTION_TIME);
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessListData()
        {
            try
            {

                if (rdo._ListSereServ != null && rdo._ListSereServ.Count > 0)
                {
                    List<long> listServiceTypeIds = new List<long>();
                    listServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH);
                    listServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU);
                    listServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                    listServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                    listServiceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN);
                    rdo._ListSereServ02 = rdo._ListSereServ.Where(o => !listServiceTypeIds.Contains(o.TDL_SERVICE_TYPE_ID)).ToList();
                    rdo._ListSereServXetNghiem = rdo._ListSereServ.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();
                }
                else
                {
                    rdo._ListSereServ = new List<HIS_SERE_SERV>();
                }

                if (rdo._ListSereServ02 == null)
                {
                    rdo._ListSereServ02 = new List<HIS_SERE_SERV>();
                }

                if (rdo._ListSereServXetNghiem == null)
                {
                    rdo._ListSereServXetNghiem = new List<HIS_SERE_SERV>();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
