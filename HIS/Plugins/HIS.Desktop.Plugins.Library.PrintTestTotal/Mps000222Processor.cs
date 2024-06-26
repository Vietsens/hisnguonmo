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
    class Mps000222Processor
    {
        private long TreatmentId;
        private long? RoomId;
        private V_HIS_PATIENT_TYPE_ALTER VHisPatientTypeAlter;
        private List<V_HIS_SERVICE_REQ> VHisServiceReqExams; //thông tin khám
        private List<V_HIS_SERVICE_REQ> VHisServiceReqTests; //thông tin xét nghiệm
        private List<V_HIS_SERVICE> HisServices; //các dịch vụ xét nghiệm để gom nhóm
        private List<HIS_SERE_SERV> HisSereServs;
        private List<V_HIS_SERE_SERV_TEIN> VHisSereServTeins;
        private List<HIS_DIIM_TYPE> HisDiimType;
        private List<HIS_FUEX_TYPE> HisFuexType;
        private MPS.Processor.Mps000222.PDO.Mps000222SDO Mps000222SDO;
        private HIS_DHST hisDhst = new HIS_DHST();
        private bool BARCODE_NO_ZERO = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Library.Print.BacodeNoZero") == "1";
        private List<MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO> ListMedicine = new List<MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO>();

        public Mps000222Processor(string printTypeCode, string fileName, ref bool result, long? roomid, V_HIS_TREATMENT treatment)
        {
            try
            {
                WaitingManager.Show();
                this.TreatmentId = treatment.ID;
                this.RoomId = roomid;

                if (ProcessDataBeforePrint())
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, roomid);

                    if (BARCODE_NO_ZERO && treatment != null)
                    {
                        treatment.TREATMENT_CODE = ProcessDeleteZeroFromCode(treatment.TREATMENT_CODE);
                    }

                    MPS.Processor.Mps000222.PDO.Mps000222PDO mps000222PDO = new MPS.Processor.Mps000222.PDO.Mps000222PDO(
                       treatment,
                       VHisServiceReqExams,
                       VHisServiceReqTests,
                       BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                       HisServices,
                       HisSereServs,
                       VHisSereServTeins,
                       Mps000222SDO,
                       hisDhst,
                       VHisPatientTypeAlter,
                       ListMedicine,
                       HisDiimType,
                       HisFuexType);

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    WaitingManager.Hide();
                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000222PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000222PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string ProcessDeleteZeroFromCode(string p)
        {
            string result = p;
            try
            {
                if (!String.IsNullOrWhiteSpace(p))
                {
                    int i = 0;
                    for (; i < p.Length; i++)
                    {
                        if (p[i] != '0')
                            break;
                    }
                    result = p.Substring(i);
                }
            }
            catch (Exception ex)
            {
                result = p;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool ProcessDataBeforePrint()
        {
            bool result = true;
            Thread serviceReq = new Thread(GetServiceReq);
            Thread Service = new Thread(GetService);
            Thread SereServTein = new Thread(GetSereServTein);
            Thread MpsSdo = new Thread(GetServiceCLS);

            Thread DiimType = new Thread(GetDiimType);
            Thread FuexType = new Thread(GetFuexType);
            Thread ListMedicine = new Thread(GetPres);
            try
            {
                if (this.TreatmentId <= 0 && this.RoomId <= 0)
                {
                    return false;
                }
                serviceReq.Start();
                Service.Start();
                SereServTein.Start();
                MpsSdo.Start();
                ListMedicine.Start();
                DiimType.Start();
                FuexType.Start();

                serviceReq.Join();
                Service.Join();
                SereServTein.Join();
                MpsSdo.Join();
                ListMedicine.Join();
                DiimType.Join();
                FuexType.Join();
            }
            catch (Exception ex)
            {
                serviceReq.Abort();
                Service.Abort();
                SereServTein.Abort();
                MpsSdo.Abort();
                ListMedicine.Abort();
                DiimType.Abort();
                FuexType.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void GetPres(object obj)
        {
            try
            {
                CommonParam param = new CommonParam();

                MOS.Filter.HisExpMestViewFilter filter = new MOS.Filter.HisExpMestViewFilter();
                filter.REQ_ROOM_ID = this.RoomId;
                filter.TDL_TREATMENT_ID = this.TreatmentId;
                filter.EXP_MEST_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT, IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK, IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT };
                var VPrescription = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (VPrescription != null && VPrescription.Count > 0)
                {
                    MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                    medicineFilter.EXP_MEST_IDs = VPrescription.Select(s => s.ID).ToList();
                    var lstmedicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (lstmedicine != null && lstmedicine.Count > 0)
                    {
                        if (ListMedicine == null) ListMedicine = new List<MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO>();
                        foreach (var item in lstmedicine)
                        {
                            MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO>(sdo, item);
                            sdo.Type = 1;
                            ListMedicine.Add(sdo);
                        }
                    }

                    MOS.Filter.HisExpMestMaterialViewFilter materialFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                    materialFilter.EXP_MEST_IDs = VPrescription.Select(s => s.ID).ToList();
                    var lstmaterial = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (lstmaterial != null && lstmaterial.Count > 0)
                    {
                        if (ListMedicine == null) ListMedicine = new List<MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO>();
                        foreach (var item in lstmaterial)
                        {
                            MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO>(sdo, item);
                            sdo.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                            sdo.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                            sdo.MEDICINE_NUM_ORDER = item.MATERIAL_NUM_ORDER;
                            sdo.MEDICINE_TYPE_NUM_ORDER = item.MATERIAL_TYPE_NUM_ORDER;
                            sdo.Type = 2;
                            ListMedicine.Add(sdo);
                        }
                    }

                    MOS.Filter.HisServiceReqFilter sfilter = new MOS.Filter.HisServiceReqFilter();
                    //filter.EXECUTE_ROOM_ID = this.RoomId;
                    sfilter.TREATMENT_ID = this.TreatmentId;
                    sfilter.SERVICE_REQ_TYPE_IDs =new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK};
                    var serviceReqs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, sfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    var medicineType = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    var materialType = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    MOS.Filter.HisServiceReqMetyFilter medFilter = new MOS.Filter.HisServiceReqMetyFilter();
                    medFilter.SERVICE_REQ_IDs = serviceReqs.Select(s => s.ID).ToList();
                    var lstmety = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumer.ApiConsumers.MosConsumer, medFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (lstmety != null && lstmety.Count > 0)
                    {
                        if (ListMedicine == null) ListMedicine = new List<MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO>();
                        foreach (var item in lstmety)
                        {
                            MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO>(sdo, item);
                            var checkMedicineType = medicineType.FirstOrDefault(o=>o.ID == item.MEDICINE_TYPE_ID);
                            if (checkMedicineType != null) {
                                sdo.MEDICINE_TYPE_CODE = checkMedicineType.MEDICINE_TYPE_CODE;
                                sdo.MEDICINE_TYPE_NAME = checkMedicineType.MEDICINE_TYPE_NAME;
                                sdo.ACTIVE_INGR_BHYT_CODE = checkMedicineType.ACTIVE_INGR_BHYT_CODE;
                                sdo.ACTIVE_INGR_BHYT_NAME = checkMedicineType.ACTIVE_INGR_BHYT_NAME;
                            }
                            sdo.SERVICE_UNIT_NAME = item.UNIT_NAME;
                            sdo.Type = 1;
                            ListMedicine.Add(sdo);
                        }
                    }

                    MOS.Filter.HisServiceReqMatyFilter matFilter = new MOS.Filter.HisServiceReqMatyFilter();
                    matFilter.SERVICE_REQ_IDs = serviceReqs.Select(s => s.ID).ToList();
                    var lstmaty = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumer.ApiConsumers.MosConsumer, matFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (lstmaty != null && lstmaty.Count > 0)
                    {
                        if (ListMedicine == null) ListMedicine = new List<MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO>();
                        foreach (var item in lstmaty)
                        {
                            MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO sdo = new MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000222.PDO.Mps000222PDO.ExpMestMedicineSDO>(sdo, item);
                            var checkMaterialType = materialType.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                            if (checkMaterialType != null)
                            {
                                sdo.MEDICINE_TYPE_CODE = checkMaterialType.MATERIAL_TYPE_CODE;
                                sdo.MEDICINE_TYPE_NAME = checkMaterialType.MATERIAL_TYPE_NAME;
                            }
                            sdo.SERVICE_UNIT_NAME = item.UNIT_NAME;
                            sdo.Type = 2;
                            ListMedicine.Add(sdo);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceCLS()
        {
            try
            {
                this.Mps000222SDO = new MPS.Processor.Mps000222.PDO.Mps000222SDO();
                this.Mps000222SDO.HisServiceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>();

                MOS.Filter.HisSereServFilter seseFilter = new MOS.Filter.HisSereServFilter();
                seseFilter.TREATMENT_ID = this.TreatmentId;
                seseFilter.TDL_SERVICE_TYPE_IDs = new List<long>()
                { 
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                };
                var sereservs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, seseFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (sereservs != null && sereservs.Count > 0)
                {
                    if (RoomId.HasValue)
                    {
                        this.Mps000222SDO.HisSereServs = sereservs.Where(o => o.TDL_REQUEST_ROOM_ID == this.RoomId).ToList();
                    }
                    else
                    {
                        this.Mps000222SDO.HisSereServs = sereservs;
                    }

                    if (this.Mps000222SDO.HisSereServs != null && this.Mps000222SDO.HisSereServs.Count > 0)
                    {
                        MOS.Filter.HisSereServExtFilter extFilter = new MOS.Filter.HisSereServExtFilter();
                        extFilter.SERE_SERV_IDs = this.Mps000222SDO.HisSereServs.Select(s => s.ID).ToList();
                        this.Mps000222SDO.HisSereServsExt = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, extFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceReq()
        {
            try
            {
                this.VHisServiceReqExams = new List<V_HIS_SERVICE_REQ>();
                MOS.Filter.HisServiceReqViewFilter filter = new MOS.Filter.HisServiceReqViewFilter();
                //filter.EXECUTE_ROOM_ID = this.RoomId;
                filter.HAS_EXECUTE = true;
                filter.TREATMENT_ID = this.TreatmentId;
                filter.SERVICE_REQ_TYPE_IDs = new List<long>() { 
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN,
                };
                var serviceReqs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (RoomId.HasValue)
                {
                    this.VHisServiceReqExams = serviceReqs.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && o.EXECUTE_ROOM_ID == this.RoomId).ToList();
                }
                else
                {
                    this.VHisServiceReqExams = serviceReqs.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH).ToList();
                }

                this.VHisServiceReqTests = serviceReqs.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList();
                if (this.VHisServiceReqExams != null && this.VHisServiceReqExams.Count > 0)
                {
                    var dhst = this.VHisServiceReqExams.Where(o => o.DHST_ID.HasValue).ToList();
                    if (dhst != null && dhst.Count > 0)
                    {
                        MOS.Filter.HisDhstFilter dhstFilter = new MOS.Filter.HisDhstFilter();
                        dhstFilter.IDs = dhst.Select(o => o.DHST_ID.Value).ToList();
                        var lstDhst = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumer.ApiConsumers.MosConsumer, dhstFilter, null);
                        if (lstDhst != null && lstDhst.Count > 0)
                        {
                            var data = CheckDhst(lstDhst);
                            if (data != null)
                            {
                                this.hisDhst = data;
                            }
                            else
                            {
                                this.hisDhst = lstDhst.FirstOrDefault();
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

        private HIS_DHST CheckDhst(List<HIS_DHST> lstDhst)
        {
            HIS_DHST result = null;
            try
            {
                result = lstDhst.FirstOrDefault(o => o.BELLY.HasValue || o.BLOOD_PRESSURE_MAX.HasValue || o.BLOOD_PRESSURE_MIN.HasValue || o.BREATH_RATE.HasValue || o.CHEST.HasValue || o.HEIGHT.HasValue || o.PULSE.HasValue || o.TEMPERATURE.HasValue || o.WEIGHT.HasValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void GetService()
        {
            try
            {

                MOS.Filter.HisPatientTypeAlterViewAppliedFilter patyFilter = new MOS.Filter.HisPatientTypeAlterViewAppliedFilter();
                patyFilter.TreatmentId = TreatmentId;
                patyFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 00000000000000;
                this.VHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetApplied", ApiConsumer.ApiConsumers.MosConsumer, patyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                this.HisServices = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => new List<long>{ IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN }.Exists(p=> p == o.SERVICE_TYPE_ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void GetDiimType()
        {
            try
            {               
                this.HisDiimType = BackendDataWorker.Get<HIS_DIIM_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetFuexType()
        {
            try
            {
                this.HisFuexType = BackendDataWorker.Get<HIS_FUEX_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServTein(object obj)
        {
            try
            {
                this.HisSereServs = new List<HIS_SERE_SERV>();
                this.VHisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();

                MOS.Filter.HisSereServFilter seseFilter = new MOS.Filter.HisSereServFilter();
                seseFilter.TREATMENT_ID = this.TreatmentId;
                seseFilter.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                var sereservs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, seseFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (sereservs != null && sereservs.Count > 0)
                {
                    if (RoomId.HasValue)
                    {
                        this.HisSereServs = sereservs.Where(o => o.TDL_REQUEST_ROOM_ID == this.RoomId).ToList();
                    }
                    else
                    {
                        this.HisSereServs = sereservs;
                    }

                    if (this.HisSereServs != null && this.HisSereServs.Count > 0)
                    {
                        MOS.Filter.HisSereServTeinViewFilter teinFilter = new MOS.Filter.HisSereServTeinViewFilter();
                        teinFilter.SERE_SERV_IDs = this.HisSereServs.Select(s => s.ID).ToList();
                        this.VHisSereServTeins = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumer.ApiConsumers.MosConsumer, teinFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                        if (VHisSereServTeins != null && VHisSereServTeins.Count > 0)
                        {
                            foreach (var item in VHisSereServTeins)
                            {
                                if (String.IsNullOrWhiteSpace(item.DESCRIPTION))
                                {
                                    var testIndex = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>().OrderByDescending(o => o.MODIFY_TIME).FirstOrDefault(o => o.TEST_INDEX_ID == item.TEST_INDEX_ID);
                                    if (testIndex != null)
                                    {
                                        item.DESCRIPTION = ProcessRange(testIndex);
                                    }
                                }
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

        private string ProcessRange(V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
                {
                    result = testIndexRange.NORMAL_VALUE;
                }
                else
                {
                    result = "";

                    if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                    {
                        if (testIndexRange.MIN_VALUE != null)
                        {
                            result += testIndexRange.MIN_VALUE + "<= ";
                        }

                        result += "X";

                        if (testIndexRange.MAX_VALUE != null)
                        {
                            result += " < " + testIndexRange.MAX_VALUE;
                        }
                    }
                    else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                    {
                        if (testIndexRange.MIN_VALUE != null)
                        {
                            result += testIndexRange.MIN_VALUE + "<= ";
                        }

                        result += "X";

                        if (testIndexRange.MAX_VALUE != null)
                        {
                            result += " <= " + testIndexRange.MAX_VALUE;
                        }
                    }
                    else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                    {
                        if (testIndexRange.MIN_VALUE != null)
                        {
                            result += testIndexRange.MIN_VALUE + "< ";
                        }

                        result += "X";

                        if (testIndexRange.MAX_VALUE != null)
                        {
                            result += " <= " + testIndexRange.MAX_VALUE;
                        }

                    }
                    else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                    {
                        if (testIndexRange.MIN_VALUE != null)
                        {
                            result += testIndexRange.MIN_VALUE + "< ";
                        }

                        result += "X";

                        if (testIndexRange.MAX_VALUE != null)
                        {
                            result += " < " + testIndexRange.MAX_VALUE;
                        }

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
    }
}
