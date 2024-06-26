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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTestTotal
{
    class Mps000316Processor
    {
        List<V_HIS_SERVICE_REQ> ListServiceReqTests;
        List<HIS_SERE_SERV> ListSereServs;
        List<HIS_SERE_SERV_EXT> ListSereServsExts;
        List<HIS_EXP_MEST_MEDICINE> ListExpMestMedis;
        List<HIS_EXP_MEST_MATERIAL> ListExpMestMaterials;
        List<V_HIS_SERE_SERV_TEIN> ListSereServTeins;
        V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter;
        HIS_DHST Dhst;
        V_HIS_SERVICE_REQ ServiceReq;
        List<V_HIS_SERVICE_REQ> ServiceReqsList;
        List<V_HIS_SERVICE_REQ> ListServiceReqChild;
        List<V_HIS_TEST_INDEX_RANGE> TestIndex;
        List<V_HIS_SERVICE> Service;
        V_HIS_PATIENT Patient;
        List<V_HIS_SERVICE_REQ> ListServiceReqDonKs;
        List<V_HIS_ROOM> ListRoomHKs;
        List<V_HIS_SERVICE_REQ> ListServiceReqHks;
        List<V_HIS_SERVICE_REQ> listHisServiceReq;
        List<V_HIS_EXP_MEST_MEDICINE> listHisExpMestMedicine;
        List<V_HIS_EXP_MEST_MATERIAL> listHisExpMestMaterial;
        List<HIS_SERVICE_REQ_METY> listHisServiceReqMety;
        List<HIS_SERVICE_REQ_MATY> listHisServiceReqMaty;
        MPS.Processor.Mps000316.PDO.PrescriptionADO prescriptionADO = new MPS.Processor.Mps000316.PDO.PrescriptionADO();


        V_HIS_TREATMENT Treatment;
        MPS.ProcessorBase.PrintConfig.PreviewType? previewType;

        public Mps000316Processor(string printTypeCode, string fileName, ref bool result, HIS_SERVICE_REQ serviceReq, V_HIS_TREATMENT treatment, long? roomid, MPS.ProcessorBase.PrintConfig.PreviewType? previewType)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, roomid);

                Treatment = treatment;
                this.previewType = previewType;

                ProcessDataBeforePrint(serviceReq);

                MPS.Processor.Mps000316.PDO.Mps000316PDO mps000316PDO = new MPS.Processor.Mps000316.PDO.Mps000316PDO(
                    Patient,
                    treatment,
                    PatientTypeAlter,
                    ServiceReq,
                    Dhst,
                    ListServiceReqTests,
                    ListSereServTeins,
                    TestIndex,
                    Service,
                    ListSereServs,
                    ListSereServsExts,
                    ListExpMestMedis,
                    ListExpMestMaterials,
                    ListServiceReqDonKs,
                    ListRoomHKs,
                    ListServiceReqHks,
                    prescriptionADO
                );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                WaitingManager.Hide();

                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000316PDO, previewType ?? MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000316PDO, previewType ?? MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ProcessDataBeforePrint(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                GetViewServiceReqList(serviceReq);
                ThreadGetCommonData(serviceReq);

                ThreadGetDetailData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region DetailData
        private void ThreadGetDetailData()
        {
            Thread test = new Thread(GetServiceReqTest);
            Thread ext = new Thread(GetSereServExt);
            Thread tein = new Thread(GetSereServTeins);
            Thread medi = new Thread(GetMedicineAndMaterials);
            Thread DonK = new Thread(GetServiceReqDonK);
            Thread RoomHk = new Thread(GetRoomHKs);
            try
            {
                test.Start();
                ext.Start();
                tein.Start();
                medi.Start();
                DonK.Start();
                RoomHk.Start();

                test.Join();
                ext.Join();
                tein.Join();
                medi.Join();
                DonK.Join();
                RoomHk.Join();
            }
            catch (Exception ex)
            {
                test.Abort();
                ext.Abort();
                tein.Abort();
                medi.Abort();
                DonK.Abort();
                RoomHk.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetRoomHKs()
        {
            try
            {
                List<long> lstRoomID = new List<long>();
                if (ServiceReqsList != null && ServiceReqsList.Count > 0)
                {
                    this.ListServiceReqHks = ServiceReqsList.Where(o => o.APPOINTMENT_TIME != null).ToList();
                }

                if (Treatment != null && !String.IsNullOrEmpty(Treatment.APPOINTMENT_EXAM_ROOM_IDS))
                {
                    var lstRoomId = Treatment.APPOINTMENT_EXAM_ROOM_IDS.Split(',').ToList();

                    foreach (var item in lstRoomId)
                    {

                        lstRoomID.Add(long.Parse(item));
                    }
                }

                if (lstRoomID != null && lstRoomID.Count > 0)
                {
                    var Rooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => lstRoomID.Distinct().ToList().Exists(p => p == o.ID)).ToList();
                    if (Rooms != null && Rooms.Count > 0)
                    {
                        this.ListRoomHKs = Rooms;
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void GetServiceReqDonK()
        {
            try
            {
                if (this.Treatment != null)
                {
                    if(ServiceReqsList != null && ServiceReqsList.Count > 0)
                    {
                        ListServiceReqDonKs = ServiceReqsList.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceReqTest()
        {
            try
            {
                if (ListServiceReqChild != null && ListServiceReqChild.Count > 0)
                {
                    var testReqs = ListServiceReqChild.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList();
                    if (testReqs != null && testReqs.Count > 0)
                    {
                        this.ListServiceReqTests = testReqs;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServTeins()
        {
            try
            {
                if (this.ListSereServs != null && this.ListSereServs.Count > 0)
                {
                    this.ListSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                    int skip = 0;
                    while (this.ListSereServs.Count - skip > 0)
                    {
                        CommonParam param = new CommonParam();
                        var lstid = this.ListSereServs.Skip(skip).Take(100).ToList();
                        skip += 100;
                        MOS.Filter.HisSereServTeinViewFilter teinFilter = new MOS.Filter.HisSereServTeinViewFilter();
                        teinFilter.SERE_SERV_IDs = lstid.Select(s => s.ID).ToList();
                        var SereServTeins = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumer.ApiConsumers.MosConsumer, teinFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (SereServTeins != null && SereServTeins.Count > 0)
                        {
                            this.ListSereServTeins.AddRange(SereServTeins);
                        }
                    }
                }

                this.TestIndex = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServExt()
        {
            try
            {
                if (this.ListSereServs != null && this.ListSereServs.Count > 0)
                {
                    this.ListSereServsExts = new List<HIS_SERE_SERV_EXT>();
                    int skip = 0;
                    while (this.ListSereServs.Count - skip > 0)
                    {
                        CommonParam param = new CommonParam();
                        var lstid = this.ListSereServs.Skip(skip).Take(100).ToList();
                        skip += 100;
                        MOS.Filter.HisSereServExtFilter extFilter = new MOS.Filter.HisSereServExtFilter();
                        extFilter.SERE_SERV_IDs = lstid.Select(s => s.ID).ToList();
                        var sereServsExt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, extFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (sereServsExt != null && sereServsExt.Count > 0)
                        {
                            this.ListSereServsExts.AddRange(sereServsExt);
                        }
                    }
                }

                Service = BackendDataWorker.Get<V_HIS_SERVICE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetMedicineAndMaterials()
        {
            try
            {
                if (this.ListSereServs != null && this.ListSereServs.Count > 0)
                {
                    ThreadGetDataBySereServ();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region CommonData
        private void ThreadGetDataBySereServ()
        {
            Thread ExpMestMedis = new Thread(GetExpMestMedis);
            Thread ExpMestMaterials = new Thread(GetExpMestMaterials);
            try
            {
                ExpMestMedis.Start();
                ExpMestMaterials.Start();
                ExpMestMedis.Join();
                ExpMestMaterials.Join();
            }
            catch (Exception ex)
            {
                ExpMestMedis.Abort();
                ExpMestMaterials.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetExpMestMaterials()
        {
            try
            {
                if (this.ListSereServs != null && this.ListSereServs.Count > 0)
                {                   
                    this.ListExpMestMaterials = new List<HIS_EXP_MEST_MATERIAL>();
                    int skip = 0;
                    var lstMate = ListSereServs.Where(o => o.EXP_MEST_MATERIAL_ID != null).Select(s => s.EXP_MEST_MATERIAL_ID ?? 0).Distinct().ToList();
                    if (lstMate.Count > 1 || (lstMate.Count == 1 && lstMate.First() != 0))
                    {
                        while (lstMate.Count - skip > 0)
                        {
                            CommonParam param = new CommonParam();
                            var lstid = lstMate.Skip(skip).Take(100).ToList();
                            skip += 100;
                            MOS.Filter.HisExpMestMaterialFilter materFilter = new MOS.Filter.HisExpMestMaterialFilter();
                            materFilter.IDs = lstid;
                            var expMestMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, materFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                            if (expMestMaterial != null && expMestMaterial.Count > 0)
                            {
                                this.ListExpMestMaterials.AddRange(expMestMaterial);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetExpMestMedis()
        {
            try
            {
                if (this.ListSereServs != null && this.ListSereServs.Count > 0)
                {
                    this.ListExpMestMedis = new List<HIS_EXP_MEST_MEDICINE>();
                    int skip = 0;
                    var lstMedi = ListSereServs.Where(o => o.EXP_MEST_MEDICINE_ID != null).Select(s => s.EXP_MEST_MEDICINE_ID ?? 0).Distinct().ToList();
                    if (lstMedi.Count > 1 || (lstMedi.Count == 1 && lstMedi.First() != 0))
                    {
                        while (lstMedi.Count - skip > 0)
                        {
                            CommonParam param = new CommonParam();
                            var lstid = lstMedi.Skip(skip).Take(100).ToList();
                            skip += 100;
                            MOS.Filter.HisExpMestMedicineFilter mediFilter = new MOS.Filter.HisExpMestMedicineFilter();
                            mediFilter.IDs = lstid;
                            var expMestMedi = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                            if (expMestMedi != null && expMestMedi.Count > 0)
                            {
                                this.ListExpMestMedis.AddRange(expMestMedi);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetCommonData(HIS_SERVICE_REQ serviceReq)
        {
            Thread req = new Thread(GetViewSr);
            Thread dhst = new Thread(GetDhst);
            Thread reqChild = new Thread(GetServiceReqChild);
            Thread SereServTein = new Thread(GetPatientTypeAlter);
            Thread patient = new Thread(GetViewPatient);
            Thread createADO = new Thread(CreateADO);
            try
            {
                req.Start(serviceReq);
                dhst.Start(serviceReq);
                reqChild.Start(serviceReq);
                SereServTein.Start(serviceReq);
                patient.Start(serviceReq);
                createADO.Start(serviceReq);
                req.Join();
                dhst.Join();
                reqChild.Join();
                SereServTein.Join();
                patient.Join();
                createADO.Join();
            }
            catch (Exception ex)
            {
                req.Abort();
                dhst.Abort();
                reqChild.Abort();
                SereServTein.Abort();
                patient.Abort();
                createADO.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetViewSr(object obj)
        {
            try
            {
                HIS_SERVICE_REQ sr = (HIS_SERVICE_REQ)obj;
                if (sr != null)
                {
                    if (ServiceReqsList != null && ServiceReqsList.Count > 0)
                    {
                        this.ServiceReq = ServiceReqsList.FirstOrDefault(o=>o.ID == sr.ID);
                    };

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDhst(object obj)
        {
            try
            {
                HIS_SERVICE_REQ sr = (HIS_SERVICE_REQ)obj;
                if (sr != null && sr.DHST_ID.HasValue)
                {
                    MOS.Filter.HisDhstFilter dhstFilter = new MOS.Filter.HisDhstFilter();
                    dhstFilter.ID = sr.DHST_ID;
                    var lstDhst = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumer.ApiConsumers.MosConsumer, dhstFilter, null);
                    if (lstDhst != null && lstDhst.Count > 0)
                    {
                        this.Dhst = lstDhst.FirstOrDefault();
                    };

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateADO(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var req = (HIS_SERVICE_REQ)obj;
                    CommonParam param = new CommonParam();
                    List<long> listFilterIDS = new List<long>();
                    listFilterIDS.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT);
                    listFilterIDS.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK);
                    listFilterIDS.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT);
                    if (ServiceReqsList != null && ServiceReqsList.Count > 0)
                    {
                        listHisServiceReq = ServiceReqsList.Where(o => listFilterIDS.Exists(p => p == o.SERVICE_REQ_TYPE_ID)).ToList();
                    }
                    if (listHisServiceReq != null && listHisServiceReq.Count > 0)
                    {
                        List<HIS_SERVICE_REQ> lstServiceReq = new List<HIS_SERVICE_REQ>();
                        foreach (var item in listHisServiceReq)
                        {
                            HIS_SERVICE_REQ sdo = new HIS_SERVICE_REQ();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(sdo, item);
                            lstServiceReq.Add(sdo);
                        }
                        prescriptionADO.HisServiceReq = lstServiceReq;
                        if (listHisServiceReq != null && listHisServiceReq.Count > 0)
                        {
                            CreateThreadLoadData(listHisServiceReq.Select(o => o.ID).ToList());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadData(List<long> serviceReqIds)
        {

            Thread mec = new Thread(new ParameterizedThreadStart(Getmec));
            Thread material = new Thread(new ParameterizedThreadStart(Getmaterial));
            Thread met = new Thread(new ParameterizedThreadStart(Getmet));
            Thread maty = new Thread(new ParameterizedThreadStart(Getmaty));
            try
            {
                mec.Start(serviceReqIds);
                material.Start(serviceReqIds);
                met.Start(serviceReqIds);
                maty.Start(serviceReqIds);
                mec.Join();
                material.Join();
                met.Join();
                maty.Join();
            }
            catch (Exception ex)
            {
                mec.Abort();
                material.Abort();
                met.Abort();
                maty.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Getmaty(object obj)
        {
            try
            {
                List<long> ids = obj as List<long>;
                CommonParam param5 = new CommonParam();
                MOS.Filter.HisServiceReqMatyFilter reqMatyFilter = new MOS.Filter.HisServiceReqMatyFilter();
                reqMatyFilter.SERVICE_REQ_IDs = ids;
                listHisServiceReqMaty = new Inventec.Common.Adapter.BackendAdapter(param5).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumer.ApiConsumers.MosConsumer, reqMatyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param5);
                prescriptionADO.VHisServiceReqMaty = listHisServiceReqMaty;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Getmet(object obj)
        {
            try
            {
                List<long> ids = obj as List<long>;
                CommonParam param4 = new CommonParam();
                MOS.Filter.HisServiceReqMetyFilter reqMetyFilter = new MOS.Filter.HisServiceReqMetyFilter();
                reqMetyFilter.SERVICE_REQ_IDs = ids;
                listHisServiceReqMety = new Inventec.Common.Adapter.BackendAdapter(param4).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumer.ApiConsumers.MosConsumer, reqMetyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param4);
                prescriptionADO.VHisServiceReqMety = listHisServiceReqMety;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Getmaterial(object obj)
        {
            try
            {
                List<long> ids = obj as List<long>;
                CommonParam param3 = new CommonParam();
                MOS.Filter.HisExpMestMaterialViewFilter materialFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                materialFilter.TDL_SERVICE_REQ_IDs = ids;
                listHisExpMestMaterial = new Inventec.Common.Adapter.BackendAdapter(param3).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param3);
                prescriptionADO.VHisExpMestMaterial = listHisExpMestMaterial;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Getmec(object obj)
        {
            try
            {
                List<long> ids = obj as List<long>; 
                CommonParam param2 = new CommonParam();
                MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                medicineFilter.TDL_SERVICE_REQ_IDs = ids;
                listHisExpMestMedicine = new Inventec.Common.Adapter.BackendAdapter(param2).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param2);
                prescriptionADO.VHisExpMestMedicine = listHisExpMestMedicine;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetPatientTypeAlter(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var req = (HIS_SERVICE_REQ)obj;
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientTypeAlterViewAppliedFilter patyFilter = new MOS.Filter.HisPatientTypeAlterViewAppliedFilter();
                    patyFilter.TreatmentId = req.TREATMENT_ID;
                    patyFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 00000000000000;
                    this.PatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetApplied", ApiConsumer.ApiConsumers.MosConsumer, patyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceReqChild(object obj)
        {
            try
            {
                if (obj != null)
                {
                    CommonParam param = new CommonParam();
                    var req = (HIS_SERVICE_REQ)obj;
                    List<V_HIS_SERVICE_REQ> serviceReqsTmp = null;
                    if (ServiceReqsList != null && ServiceReqsList.Count > 0)
                    {
                        if((ServiceReq != null && ServiceReq.TDL_KSK_CONTRACT_ID.HasValue) || (Treatment != null && Treatment.TDL_KSK_CONTRACT_ID.HasValue))
                            serviceReqsTmp = ServiceReqsList;
                        else
                            serviceReqsTmp = ServiceReqsList.Where(o => o.PARENT_ID == req.ID).ToList();
                    }
                    if (serviceReqsTmp != null && serviceReqsTmp.Count > 0)
                    {
                        this.ListServiceReqChild = serviceReqsTmp;

                        MOS.Filter.HisSereServFilter ssFilter = new MOS.Filter.HisSereServFilter();
                        ssFilter.SERVICE_REQ_IDs = serviceReqsTmp.Select(s => s.ID).ToList();
                        this.ListSereServs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        Inventec.Common.Logging.LogSystem.Info("count :" + ListSereServs.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetViewServiceReqList(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var req = (HIS_SERVICE_REQ)obj;
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqViewFilter filter = new MOS.Filter.HisServiceReqViewFilter();
                    filter.TREATMENT_ID = req.TREATMENT_ID;
                    ServiceReqsList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetViewPatient(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var req = (HIS_SERVICE_REQ)obj;
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                    filter.ID = req.TDL_PATIENT_ID;
                    var patient = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (patient != null && patient.Count == 1)
                    {
                        this.Patient = patient.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
