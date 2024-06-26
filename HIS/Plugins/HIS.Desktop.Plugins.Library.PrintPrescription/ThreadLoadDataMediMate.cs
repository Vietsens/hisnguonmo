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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.PrintPrescription.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintPrescription
{
    class ThreadLoadDataMediMate
    {
        private const int Thuoc = 1;
        private const int VatTu = 2;
        private const int ThuocNgoaiKho = 3;
        private const int VatTuNgoaiKho = 4;
        private const int TuTuc = 5;
        HIS_TREATMENT treatment;

        CommonParam paramCommon = new CommonParam();

        public ThreadLoadDataMediMate(ThreadMedicineADO lstMediMateInADO, ThreadMedicineADO lstMediMateTuTucADO, HIS_TREATMENT _treatment)
        {
            treatment = _treatment;
            if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.KEY_IsPrintPrescriptionNoThread) == "1")
            {
                if (lstMediMateInADO != null)
                {
                    lstMediMateInADO.DicLstMediMateExpMestTypeADO = new Dictionary<long, List<ExpMestMedicineSDO>>();
                    LoadDataMedicineInStock(lstMediMateInADO);
                    LoadDataMaterialInStock(lstMediMateInADO);
                }

                if (lstMediMateTuTucADO != null)
                {
                    lstMediMateTuTucADO.DicLstMediMateExpMestTypeADO = new Dictionary<long, List<ExpMestMedicineSDO>>();
                    LoadDataMedicineOutStock(lstMediMateTuTucADO);
                    LoadDataMaterialOutStock(lstMediMateTuTucADO);
                }
            }
            else
            {
                List<Task> taskall = new List<Task>();
                if (lstMediMateInADO != null)
                {
                    lstMediMateInADO.DicLstMediMateExpMestTypeADO = new Dictionary<long, List<ExpMestMedicineSDO>>();

                    Task tsMedicineIn = Task.Factory.StartNew(() =>
                    {
                        LoadDataMedicineInStock(lstMediMateInADO);
                    });
                    taskall.Add(tsMedicineIn);

                    Task tsMaterialIn = Task.Factory.StartNew(() =>
                    {
                        LoadDataMaterialInStock(lstMediMateInADO);
                    });
                    taskall.Add(tsMaterialIn);
                }

                if (lstMediMateTuTucADO != null)
                {
                    lstMediMateTuTucADO.DicLstMediMateExpMestTypeADO = new Dictionary<long, List<ExpMestMedicineSDO>>();

                    Task tsMedicineOut = Task.Factory.StartNew(() =>
                    {
                        LoadDataMedicineOutStock(lstMediMateTuTucADO);
                    });
                    taskall.Add(tsMedicineOut);

                    Task tsMaterialOut = Task.Factory.StartNew(() =>
                    {
                        LoadDataMaterialOutStock(lstMediMateTuTucADO);
                    });
                    taskall.Add(tsMaterialOut);
                }

                if (taskall.Count > 0)
                    Task.WaitAll(taskall.ToArray());
            }
        }

        private void LoadDataMedicineInStock(ThreadMedicineADO threadMedicineADO)
        {
            try
            {
                if (threadMedicineADO != null)
                {
                    if (threadMedicineADO.DicLstMediMateExpMestTypeADO == null)
                    {
                        lock (threadMedicineADO)
                        {
                            threadMedicineADO.DicLstMediMateExpMestTypeADO = new Dictionary<long, List<ExpMestMedicineSDO>>();
                        }
                    }

                    List<HIS_EXP_MEST_MEDICINE> lstmedicine = null;

                    //Thuoc va vat tu trong danh muc
                    if (threadMedicineADO.HasMediMate)
                    {
                        lstmedicine = threadMedicineADO.Medicines;
                    }
                    else if (threadMedicineADO.Medicines != null && threadMedicineADO.Medicines.Count > 0)
                    {
                        lstmedicine = threadMedicineADO.Medicines;
                    }
                    else if (threadMedicineADO.ExpMests != null && threadMedicineADO.ExpMests.Count > 0)
                    {
                        HisExpMestMedicineFilter medicineFilter = new HisExpMestMedicineFilter();
                        medicineFilter.EXP_MEST_IDs = threadMedicineADO.ExpMests.Select(s => s.ID).ToList();
                        lstmedicine = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_EXP_MEST_MEDICINE>>(RequestUriStore.HIS_EXP_MEST_MEDICINE_GET, ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    }

                    //#40883
                    //lấy thuốc/vật tư được đánh dấu "ko phải thuốc bác sỹ kê"
                    //bổ sung key số lượng bác sĩ kê.
                    ////#19931
                    ////không lấy thuốc/vật tư được đánh dấu "ko phải thuốc bác sỹ kê"
                    //if (lstmedicine != null && lstmedicine.Count > 0)
                    //{
                    //    lstmedicine = lstmedicine.Where(o => o.IS_NOT_PRES != 1).ToList();
                    //}
                    if (lstmedicine != null && lstmedicine.Count > 0)
                    {
                        List<HIS_SERVICE_REQ> lstServiceReq = new List<HIS_SERVICE_REQ>();
                        List<V_HIS_EXP_MEST_MEDICINE> lstExpMestMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                        if (treatment != null)
                        {
                            List<long> serviceReqTypes = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT };
                            HisServiceReqFilter serviceReqfilter = new HisServiceReqFilter();
                            serviceReqfilter.TREATMENT_ID = treatment.ID;
                            serviceReqfilter.SERVICE_REQ_TYPE_IDs = serviceReqTypes;
                            lstServiceReq = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqfilter, null);
                            HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                            expMestMedicineFilter.TDL_TREATMENT_ID = treatment.ID;
                            lstExpMestMedicine = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, expMestMedicineFilter, null);
                        }
                        var expMestMedicineGroups = lstmedicine.GroupBy(o => new { o.TDL_MEDICINE_TYPE_ID, o.PRICE, o.IS_EXPEND, o.EXP_MEST_ID });

                        foreach (var item in lstExpMestMedicine)
                        {
                            var ServiceReq = lstServiceReq.FirstOrDefault(p => p.ID == item.TDL_SERVICE_REQ_ID);
                            if (ServiceReq != null)
                                item.TDL_INTRUCTION_TIME = ServiceReq.INTRUCTION_TIME;
                        }

                        foreach (var expMestMedicineGroup in expMestMedicineGroups)
                        {
                            var ServiceReq = lstServiceReq.FirstOrDefault(p => p.ID == expMestMedicineGroup.FirstOrDefault().TDL_SERVICE_REQ_ID);

                            ExpMestMedicineSDO mediExpmestADO = new ExpMestMedicineSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineSDO>(mediExpmestADO, expMestMedicineGroup.First());
                            var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == expMestMedicineGroup.First().TDL_MEDICINE_TYPE_ID);

                            long count = 0;
                            long? PreviousUseDay = null;
                            List<V_HIS_EXP_MEST_MEDICINE> lstCheck = new List<V_HIS_EXP_MEST_MEDICINE>();

                            if (lstServiceReq.Count() > 0 && ServiceReq != null)
                            {
                                var CountServideReq = lstServiceReq.Where(o => o.INTRUCTION_TIME <= ServiceReq.INTRUCTION_TIME).Select(o => o.ID).ToList();
                                if (CountServideReq != null && CountServideReq.Count > 0)
                                {
                                    foreach (var itemDem in CountServideReq)
                                    {
                                        var check = lstExpMestMedicine.Where(o => o.TDL_MEDICINE_TYPE_ID == expMestMedicineGroup.FirstOrDefault().TDL_MEDICINE_TYPE_ID && o.TDL_TREATMENT_ID == treatment.ID && o.TDL_SERVICE_REQ_ID == itemDem).ToList();

                                        count += check.Count();

                                        if (PreviousUseDay == null)
                                        {
                                            if (check != null && check.Count() > 0)
                                            {
                                                lstCheck.AddRange(check);
                                            }
                                        }
                                    }
                                }
                            }
                            if (PreviousUseDay == null && lstCheck != null && lstCheck.Count > 0)
                            {
                                var Check1 = lstCheck.Where(o => o.PREVIOUS_USING_COUNT != null).OrderBy(o => o.ID).ToList();
                                if (Check1 != null && Check1.Count > 0)
                                {
                                    PreviousUseDay = Check1.FirstOrDefault().PREVIOUS_USING_COUNT != null ? Check1.FirstOrDefault().PREVIOUS_USING_COUNT : 0;
                                }
                            }

                            PreviousUseDay = PreviousUseDay != null ? PreviousUseDay : 0;

                            if (mety != null)
                            {
                                mediExpmestADO.MEDICINE_TYPE_ID = mety.ID;
                                mediExpmestADO.MEDICINE_TYPE_CODE = mety.MEDICINE_TYPE_CODE;
                                mediExpmestADO.MEDICINE_TYPE_NAME = mety.MEDICINE_TYPE_NAME;
                                mediExpmestADO.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
                                mediExpmestADO.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                                mediExpmestADO.SERVICE_UNIT_NAME = mety.SERVICE_UNIT_NAME;
                                mediExpmestADO.MEDICINE_USE_FORM_NAME = mety.MEDICINE_USE_FORM_NAME;
                                mediExpmestADO.IS_FUNCTIONAL_FOOD = mety.IS_FUNCTIONAL_FOOD;
                                mediExpmestADO.IS_OUT_HOSPITAL = mety.IS_OUT_HOSPITAL;
                                mediExpmestADO.MEDICINE_TYPE_DESCRIPTION = mety.DESCRIPTION;
                                mediExpmestADO.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;
                                mediExpmestADO.NATIONAL_NAME = mety.NATIONAL_NAME;
                                mediExpmestADO.MANUFACTURER_ID = mety.MANUFACTURER_ID;
                                mediExpmestADO.MANUFACTURER_CODE = mety.MANUFACTURER_CODE;
                                mediExpmestADO.MANUFACTURER_NAME = mety.MANUFACTURER_NAME;

                                if (mety.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN)
                                {
                                    mediExpmestADO.IS_ADDICTIVE = (short)1;
                                }
                                else if (mety.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                                {
                                    mediExpmestADO.IS_NEUROLOGICAL = (short)1;
                                }
                                else if (mety.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX)
                                {
                                    mediExpmestADO.IS_RADIOACTIVE = (short)1;
                                }
                                else if (mety.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC)
                                {
                                    mediExpmestADO.IS_POISON = (short)1;
                                }
                                else if (mety.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO)
                                {
                                    mediExpmestADO.IS_TUBERCULOSIS = (short)1;
                                }

                                mediExpmestADO.CONCENTRA = mety.CONCENTRA;
                                mediExpmestADO.CONVERT_RATIO = mety.CONVERT_RATIO;
                                mediExpmestADO.CONVERT_UNIT_NAME = mety.CONVERT_UNIT_NAME;

                                var medicineGroup = mety.MEDICINE_GROUP_ID.HasValue
                                    ? BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == mety.MEDICINE_GROUP_ID.Value)
                                    : null;

                                mediExpmestADO.MEDICINE_GROUP_NUM_ORDER = medicineGroup != null ? medicineGroup.NUM_ORDER : null;

                                var medicineUseform = mety.MEDICINE_USE_FORM_ID.HasValue
                                    ? BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.ID == mety.MEDICINE_USE_FORM_ID.Value)
                                    : null;

                                mediExpmestADO.MEDICINE_USE_FORM_NUM_ORDER = medicineUseform != null ? medicineUseform.NUM_ORDER : null;
                            }

                            mediExpmestADO.Type = Thuoc;
                            mediExpmestADO.AMOUNT = expMestMedicineGroup.Sum(o => o.AMOUNT);
                            mediExpmestADO.NUM_ORDER = expMestMedicineGroup.First().NUM_ORDER ?? 999999;
                            mediExpmestADO.PRES_AMOUNT = 0;
                            foreach (var item in expMestMedicineGroup)
                            {
                                if (item.PRES_AMOUNT != null)
                                {
                                    mediExpmestADO.PRES_AMOUNT += item.PRES_AMOUNT;
                                }
                                else if (!item.IS_NOT_PRES.HasValue)
                                {
                                    mediExpmestADO.PRES_AMOUNT += item.AMOUNT;
                                }
                            }

                            mediExpmestADO.USING_COUNT_NUMBER = count + PreviousUseDay;
                            mediExpmestADO.EXP_MEST_ID = expMestMedicineGroup.First().EXP_MEST_ID;//key

                            if (expMestMedicineGroup.First().HTU_ID.HasValue)
                            {
                                var htu = BackendDataWorker.Get<HIS_HTU>().FirstOrDefault(o => o.ID == expMestMedicineGroup.First().HTU_ID.Value);
                                if (htu != null)
                                {
                                    mediExpmestADO.HTU_NAME = htu.HTU_NAME;
                                }
                            }

                            var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == expMestMedicineGroup.First().PATIENT_TYPE_ID);
                            if (patientType != null)
                            {
                                mediExpmestADO.PATIENT_TYPE_ID = patientType.ID;
                                mediExpmestADO.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                            }

                            if (!threadMedicineADO.DicLstMediMateExpMestTypeADO.ContainsKey(mediExpmestADO.EXP_MEST_ID ?? 0))
                            {
                                lock (threadMedicineADO)
                                {
                                    threadMedicineADO.DicLstMediMateExpMestTypeADO[mediExpmestADO.EXP_MEST_ID ?? 0] = new List<ExpMestMedicineSDO>();
                                }
                            }

                            if (mediExpmestADO.CONVERT_RATIO.HasValue && expMestMedicineGroup.First().USE_ORIGINAL_UNIT_FOR_PRES != 1)
                            {
                                mediExpmestADO.AMOUNT = mediExpmestADO.AMOUNT * mediExpmestADO.CONVERT_RATIO.Value;
                                mediExpmestADO.PRES_AMOUNT = mediExpmestADO.PRES_AMOUNT * mediExpmestADO.CONVERT_RATIO.Value;
                                mediExpmestADO.PRICE = mediExpmestADO.PRICE / mediExpmestADO.CONVERT_RATIO.Value;
                                mediExpmestADO.SERVICE_UNIT_NAME = mediExpmestADO.CONVERT_UNIT_NAME;
                            }

                            threadMedicineADO.DicLstMediMateExpMestTypeADO[mediExpmestADO.EXP_MEST_ID ?? 0].Add(mediExpmestADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterialInStock(ThreadMedicineADO threadMedicineADO)
        {
            try
            {
                if (threadMedicineADO != null)
                {
                    if (threadMedicineADO.DicLstMediMateExpMestTypeADO == null)
                    {
                        lock (threadMedicineADO)
                        {
                            threadMedicineADO.DicLstMediMateExpMestTypeADO = new Dictionary<long, List<ExpMestMedicineSDO>>();
                        }
                    }

                    //vật tư
                    List<HIS_EXP_MEST_MATERIAL> lstmaterial = null;
                    if (threadMedicineADO.HasMediMate)
                    {
                        lstmaterial = threadMedicineADO.Materials;
                    }
                    else if (threadMedicineADO.Materials != null && threadMedicineADO.Materials.Count > 0)
                    {
                        lstmaterial = threadMedicineADO.Materials;
                    }
                    else if (threadMedicineADO.ExpMests != null && threadMedicineADO.ExpMests.Count > 0)
                    {
                        HisExpMestMaterialFilter materialFilter = new HisExpMestMaterialFilter();
                        materialFilter.EXP_MEST_IDs = threadMedicineADO.ExpMests.Select(s => s.ID).ToList();
                        lstmaterial = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_EXP_MEST_MATERIAL>>(RequestUriStore.HIS_EXP_MEST_MATERIAL_GET, ApiConsumer.ApiConsumers.MosConsumer, materialFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    }

                    //#40883
                    //lấy thuốc/vật tư được đánh dấu "ko phải thuốc bác sỹ kê"
                    //bổ sung key số lượng bác sĩ kê.
                    ////#19931
                    ////không lấy thuốc/vật tư được đánh dấu "ko phải thuốc bác sỹ kê"
                    //if (lstmaterial != null && lstmaterial.Count > 0)
                    //{
                    //    lstmaterial = lstmaterial.Where(o => o.IS_NOT_PRES != 1).ToList();
                    //}

                    if (lstmaterial != null && lstmaterial.Count > 0)
                    {
                        var expMestMaterialGroups = lstmaterial.GroupBy(o => new { o.TDL_MATERIAL_TYPE_ID, o.PRICE, o.IS_EXPEND, o.EXP_MEST_ID });
                        foreach (var expMestMaterialGroup in expMestMaterialGroups)
                        {
                            ExpMestMedicineSDO mateExpmestADO = new ExpMestMedicineSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineSDO>(mateExpmestADO, expMestMaterialGroup.First());

                            var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == expMestMaterialGroup.First().TDL_MATERIAL_TYPE_ID);
                            if (maty != null)
                            {
                                mateExpmestADO.MEDICINE_TYPE_ID = maty.ID;
                                mateExpmestADO.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                                mateExpmestADO.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                                mateExpmestADO.SERVICE_UNIT_NAME = maty.SERVICE_UNIT_NAME;
                                mateExpmestADO.CONCENTRA = maty.CONCENTRA;

                                mateExpmestADO.CONVERT_RATIO = maty.CONVERT_RATIO;
                                mateExpmestADO.CONVERT_UNIT_NAME = maty.CONVERT_UNIT_NAME;
                                mateExpmestADO.IS_OUT_HOSPITAL = maty.IS_OUT_HOSPITAL;
                                mateExpmestADO.NATIONAL_NAME = maty.NATIONAL_NAME;
                                mateExpmestADO.MANUFACTURER_ID = maty.MANUFACTURER_ID;
                                mateExpmestADO.MANUFACTURER_CODE = maty.MANUFACTURER_CODE;
                                mateExpmestADO.MANUFACTURER_NAME = maty.MANUFACTURER_NAME;
                            }

                            mateExpmestADO.Type = VatTu;
                            mateExpmestADO.AMOUNT = expMestMaterialGroup.Sum(o => o.AMOUNT);
                            mateExpmestADO.NUM_ORDER = expMestMaterialGroup.First().NUM_ORDER ?? 999999;
                            mateExpmestADO.PRES_AMOUNT = 0;
                            foreach (var item in expMestMaterialGroup)
                            {
                                if (item.PRES_AMOUNT != null)
                                {
                                    mateExpmestADO.PRES_AMOUNT += item.PRES_AMOUNT;
                                }
                                else if (!item.IS_NOT_PRES.HasValue)
                                {
                                    mateExpmestADO.PRES_AMOUNT += item.AMOUNT;
                                }
                            }

                            var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == expMestMaterialGroup.First().PATIENT_TYPE_ID);
                            if (patientType != null)
                            {
                                mateExpmestADO.PATIENT_TYPE_ID = patientType.ID;
                                mateExpmestADO.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                            }

                            if (!threadMedicineADO.DicLstMediMateExpMestTypeADO.ContainsKey(mateExpmestADO.EXP_MEST_ID ?? 0))
                            {
                                lock (threadMedicineADO)
                                {
                                    threadMedicineADO.DicLstMediMateExpMestTypeADO[mateExpmestADO.EXP_MEST_ID ?? 0] = new List<ExpMestMedicineSDO>();
                                }
                            }

                            if (mateExpmestADO.CONVERT_RATIO.HasValue && expMestMaterialGroup.First().USE_ORIGINAL_UNIT_FOR_PRES != 1)
                            {
                                mateExpmestADO.AMOUNT = mateExpmestADO.AMOUNT * mateExpmestADO.CONVERT_RATIO.Value;
                                mateExpmestADO.PRES_AMOUNT = mateExpmestADO.PRES_AMOUNT * mateExpmestADO.CONVERT_RATIO.Value;
                                mateExpmestADO.PRICE = mateExpmestADO.PRICE / mateExpmestADO.CONVERT_RATIO.Value;
                                mateExpmestADO.SERVICE_UNIT_NAME = mateExpmestADO.CONVERT_UNIT_NAME;
                            }

                            threadMedicineADO.DicLstMediMateExpMestTypeADO[mateExpmestADO.EXP_MEST_ID ?? 0].Add(mateExpmestADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicineOutStock(ThreadMedicineADO threadMedicineADO)
        {
            try
            {
                if (threadMedicineADO != null)
                {
                    if (threadMedicineADO.DicLstMediMateExpMestTypeADO == null)
                    {
                        lock (threadMedicineADO)
                        {
                            threadMedicineADO.DicLstMediMateExpMestTypeADO = new Dictionary<long, List<ExpMestMedicineSDO>>();
                        }
                    }

                    //Thuoc
                    List<HIS_SERVICE_REQ_METY> metys = null;
                    if (threadMedicineADO.HasMediMate)
                    {
                        metys = threadMedicineADO.ServiceReqMeties;
                    }
                    else if (threadMedicineADO.ServiceReqMeties != null && threadMedicineADO.ServiceReqMeties.Count > 0)
                    {
                        metys = threadMedicineADO.ServiceReqMeties;
                    }
                    else if (threadMedicineADO.ExpMests != null && threadMedicineADO.ExpMests.Count > 0)
                    {
                        HisServiceReqMetyFilter metyFilter = new HisServiceReqMetyFilter();
                        metyFilter.SERVICE_REQ_IDs = threadMedicineADO.ServiceReqs.Select(o => o.ID).ToList();
                        metys = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ_METY>>(RequestUriStore.HIS_SERVICE_REQ_METY_GET, ApiConsumer.ApiConsumers.MosConsumer, metyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    }

                    if (metys != null && metys.Count > 0)
                    {
                        List<HIS_SERVICE_REQ> lstServiceReq = new List<HIS_SERVICE_REQ>();
                        List<HIS_SERVICE_REQ_METY> lstServiceReqMety = new List<HIS_SERVICE_REQ_METY>();
                        if (treatment != null)
                        {
                            List<long> serviceReqTypes = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT };
                            HisServiceReqFilter serviceReqfilter = new HisServiceReqFilter();
                            serviceReqfilter.TREATMENT_ID = treatment.ID;
                            serviceReqfilter.SERVICE_REQ_TYPE_IDs = serviceReqTypes;
                            lstServiceReq = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqfilter, null);

                            HisServiceReqMetyFilter ServiceReqMetyFilter = new HisServiceReqMetyFilter();
                            ServiceReqMetyFilter.TDL_TREATMENT_ID = treatment.ID;
                            lstServiceReqMety = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, ServiceReqMetyFilter, null);
                        }
                        var expMestMetyGroups = metys.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_NAME, o.MEDICINE_USE_FORM_ID, o.SERVICE_REQ_ID, o.PRICE, o.IS_SUB_PRES });

                        foreach (var expMestMetyGroup in expMestMetyGroups)
                        {
                            var ServiceReq = lstServiceReq.FirstOrDefault(p => p.ID == expMestMetyGroup.FirstOrDefault().SERVICE_REQ_ID);

                            ExpMestMedicineSDO metyExpmestADO = new ExpMestMedicineSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineSDO>(metyExpmestADO, expMestMetyGroup.First());
                            var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == expMestMetyGroup.First().MEDICINE_TYPE_ID);

                            long count = 0;
                            long? PreviousUseDay = null;
                            List<HIS_SERVICE_REQ_METY> lstCheck = new List<HIS_SERVICE_REQ_METY>();

                            if (lstServiceReq.Count() > 0 && ServiceReq != null)
                            {
                                var CountServideReq = lstServiceReq.Where(o => o.INTRUCTION_TIME <= ServiceReq.INTRUCTION_TIME).Select(o => o.ID).ToList();
                                if (CountServideReq != null && CountServideReq.Count > 0)
                                {
                                    foreach (var itemDem in CountServideReq)
                                    {
                                        var check = lstServiceReqMety.Where(o => o.MEDICINE_TYPE_ID == expMestMetyGroup.FirstOrDefault().MEDICINE_TYPE_ID && o.MEDICINE_TYPE_NAME == expMestMetyGroup.FirstOrDefault().MEDICINE_TYPE_NAME && o.TDL_TREATMENT_ID == treatment.ID && o.SERVICE_REQ_ID == itemDem && o.PREVIOUS_USING_COUNT != null).ToList();

                                        count += check.Count();

                                        if (PreviousUseDay == null)
                                        {
                                            if (check != null && check.Count() > 0)
                                            {
                                                lstCheck.AddRange(check);
                                            }
                                        }
                                    }
                                }
                            }
                            if (PreviousUseDay == null && lstCheck != null && lstCheck.Count > 0)
                            {
                                var Check1 = lstCheck.Where(o => o.PREVIOUS_USING_COUNT != null).OrderBy(o => o.ID).ToList();
                                if (Check1 != null && Check1.Count > 0)
                                {
                                    PreviousUseDay = Check1.FirstOrDefault().PREVIOUS_USING_COUNT != null ? Check1.FirstOrDefault().PREVIOUS_USING_COUNT : 0;
                                }
                            }

                            PreviousUseDay = PreviousUseDay != null ? PreviousUseDay : 0;
                            if (mety != null)
                            {
                                metyExpmestADO.MEDICINE_TYPE_ID = mety.ID;
                                metyExpmestADO.MEDICINE_TYPE_CODE = mety.MEDICINE_TYPE_CODE;
                                metyExpmestADO.MEDICINE_TYPE_NAME = mety.MEDICINE_TYPE_NAME;
                                metyExpmestADO.SERVICE_UNIT_NAME = mety.SERVICE_UNIT_NAME;
                                var useForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.ID == (expMestMetyGroup.First().MEDICINE_USE_FORM_ID ?? 0));
                                metyExpmestADO.MEDICINE_USE_FORM_NAME = useForm != null ? useForm.MEDICINE_USE_FORM_NAME : mety.MEDICINE_USE_FORM_NAME;
                                metyExpmestADO.MEDICINE_USE_FORM_NUM_ORDER = useForm != null ? useForm.NUM_ORDER : null;
                                metyExpmestADO.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                                metyExpmestADO.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
                                metyExpmestADO.IS_FUNCTIONAL_FOOD = mety.IS_FUNCTIONAL_FOOD;
                                metyExpmestADO.IS_OUT_HOSPITAL = mety.IS_OUT_HOSPITAL;
                                metyExpmestADO.MEDICINE_TYPE_DESCRIPTION = mety.DESCRIPTION;
                                metyExpmestADO.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;
                                metyExpmestADO.NATIONAL_NAME = mety.NATIONAL_NAME;
                                metyExpmestADO.MANUFACTURER_ID = mety.MANUFACTURER_ID;
                                metyExpmestADO.MANUFACTURER_CODE = mety.MANUFACTURER_CODE;
                                metyExpmestADO.MANUFACTURER_NAME = mety.MANUFACTURER_NAME;

                                if (mety.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN)
                                {
                                    metyExpmestADO.IS_ADDICTIVE = (short)1;
                                }
                                else if (mety.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                                {
                                    metyExpmestADO.IS_NEUROLOGICAL = (short)1;
                                }
                                else if (mety.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX)
                                {
                                    metyExpmestADO.IS_RADIOACTIVE = (short)1;
                                }
                                else if (mety.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC)
                                {
                                    metyExpmestADO.IS_POISON = (short)1;
                                }
                                else if (mety.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO)
                                {
                                    metyExpmestADO.IS_TUBERCULOSIS = (short)1;
                                }
                                metyExpmestADO.CONCENTRA = mety.CONCENTRA;

                                metyExpmestADO.CONVERT_RATIO = mety.CONVERT_RATIO;
                                metyExpmestADO.CONVERT_UNIT_NAME = mety.CONVERT_UNIT_NAME;
                                metyExpmestADO.Type = ThuocNgoaiKho;
                                if (expMestMetyGroup.First().IS_SUB_PRES == 1)
                                {
                                    metyExpmestADO.Type = Thuoc;
                                }
                            }
                            else
                            {
                                metyExpmestADO.MEDICINE_TYPE_NAME = expMestMetyGroup.First().MEDICINE_TYPE_NAME;
                                var useForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.ID == (expMestMetyGroup.First().MEDICINE_USE_FORM_ID ?? 0));
                                metyExpmestADO.MEDICINE_USE_FORM_NAME = useForm != null ? useForm.MEDICINE_USE_FORM_NAME : "";
                                metyExpmestADO.MEDICINE_USE_FORM_NUM_ORDER = useForm != null ? useForm.NUM_ORDER : null;
                                metyExpmestADO.SERVICE_UNIT_NAME = expMestMetyGroup.First().UNIT_NAME;
                                metyExpmestADO.Type = TuTuc;
                            }

                            metyExpmestADO.NUM_ORDER = expMestMetyGroup.First().NUM_ORDER ?? 999999;
                            metyExpmestADO.AMOUNT = expMestMetyGroup.Sum(o => o.AMOUNT);

                            metyExpmestADO.PRES_AMOUNT = expMestMetyGroup.Sum(o => o.PRES_AMOUNT);

                            if (expMestMetyGroup.First().HTU_ID.HasValue)
                            {
                                var htu = BackendDataWorker.Get<HIS_HTU>().FirstOrDefault(o => o.ID == expMestMetyGroup.First().HTU_ID.Value);
                                if (htu != null)
                                {
                                    metyExpmestADO.HTU_NAME = htu.HTU_NAME;
                                }
                            }

                            metyExpmestADO.USING_COUNT_NUMBER = count + PreviousUseDay;
                            metyExpmestADO.EXP_MEST_ID = expMestMetyGroup.First().SERVICE_REQ_ID;

                            if (!threadMedicineADO.DicLstMediMateExpMestTypeADO.ContainsKey(metyExpmestADO.EXP_MEST_ID ?? 0))
                                lock (threadMedicineADO)
                                    threadMedicineADO.DicLstMediMateExpMestTypeADO[metyExpmestADO.EXP_MEST_ID ?? 0] = new List<ExpMestMedicineSDO>();

                            threadMedicineADO.DicLstMediMateExpMestTypeADO[metyExpmestADO.EXP_MEST_ID ?? 0].Add(metyExpmestADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterialOutStock(ThreadMedicineADO threadMedicineADO)
        {
            try
            {
                if (threadMedicineADO != null)
                {
                    if (threadMedicineADO.DicLstMediMateExpMestTypeADO == null)
                    {
                        lock (threadMedicineADO)
                        {
                            threadMedicineADO.DicLstMediMateExpMestTypeADO = new Dictionary<long, List<ExpMestMedicineSDO>>();
                        }
                    }

                    //vat tu
                    List<HIS_SERVICE_REQ_MATY> matys = null;
                    if (threadMedicineADO.HasMediMate)
                    {
                        matys = threadMedicineADO.ServiceReqMaties;
                    }
                    else if (threadMedicineADO.ServiceReqMaties != null && threadMedicineADO.ServiceReqMaties.Count > 0)
                    {
                        matys = threadMedicineADO.ServiceReqMaties;
                    }
                    else if (threadMedicineADO.ExpMests != null && threadMedicineADO.ExpMests.Count > 0)
                    {
                        HisServiceReqMatyFilter matyFilter = new HisServiceReqMatyFilter();
                        matyFilter.SERVICE_REQ_IDs = threadMedicineADO.ServiceReqs.Select(o => o.ID).ToList();
                        matys = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ_MATY>>(RequestUriStore.HIS_SERVICE_REQ_MATY_GET, ApiConsumer.ApiConsumers.MosConsumer, matyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    }

                    if (matys != null && matys.Count > 0)
                    {
                        var expMestMatyGroups = matys.GroupBy(o => new { o.MATERIAL_TYPE_ID, o.MATERIAL_TYPE_NAME, o.SERVICE_REQ_ID, o.PRICE, o.IS_SUB_PRES });
                        foreach (var expMestMatyGroup in expMestMatyGroups)
                        {
                            ExpMestMedicineSDO matyExpmestADO = new ExpMestMedicineSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineSDO>(matyExpmestADO, expMestMatyGroup.First());
                            var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == expMestMatyGroup.First().MATERIAL_TYPE_ID);
                            if (maty != null)
                            {
                                matyExpmestADO.MEDICINE_TYPE_ID = maty.ID;
                                matyExpmestADO.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                                matyExpmestADO.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                                matyExpmestADO.SERVICE_UNIT_NAME = maty.SERVICE_UNIT_NAME;
                                matyExpmestADO.CONCENTRA = maty.CONCENTRA;

                                matyExpmestADO.CONVERT_RATIO = maty.CONVERT_RATIO;
                                matyExpmestADO.CONVERT_UNIT_NAME = maty.CONVERT_UNIT_NAME;
                                matyExpmestADO.IS_OUT_HOSPITAL = maty.IS_OUT_HOSPITAL;
                                matyExpmestADO.NATIONAL_NAME = maty.NATIONAL_NAME;
                                matyExpmestADO.MANUFACTURER_ID = maty.MANUFACTURER_ID;
                                matyExpmestADO.MANUFACTURER_CODE = maty.MANUFACTURER_CODE;
                                matyExpmestADO.MANUFACTURER_NAME = maty.MANUFACTURER_NAME;
                                matyExpmestADO.Type = VatTuNgoaiKho;
                                if (expMestMatyGroup.First().IS_SUB_PRES == 1)
                                {
                                    matyExpmestADO.Type = VatTu;
                                }
                            }
                            else
                            {
                                matyExpmestADO.MEDICINE_TYPE_ID = expMestMatyGroup.First().MATERIAL_TYPE_ID ?? 0;
                                matyExpmestADO.MEDICINE_TYPE_NAME = expMestMatyGroup.First().MATERIAL_TYPE_NAME;
                                matyExpmestADO.SERVICE_UNIT_NAME = expMestMatyGroup.First().UNIT_NAME;
                                matyExpmestADO.Type = TuTuc;
                            }

                            matyExpmestADO.NUM_ORDER = expMestMatyGroup.First().NUM_ORDER ?? 999999;
                            matyExpmestADO.AMOUNT = expMestMatyGroup.Sum(o => o.AMOUNT);
                            matyExpmestADO.PRES_AMOUNT = expMestMatyGroup.Sum(o => o.PRES_AMOUNT);

                            matyExpmestADO.EXP_MEST_ID = expMestMatyGroup.First().SERVICE_REQ_ID;

                            if (!threadMedicineADO.DicLstMediMateExpMestTypeADO.ContainsKey(matyExpmestADO.EXP_MEST_ID ?? 0))
                                lock (threadMedicineADO)
                                    threadMedicineADO.DicLstMediMateExpMestTypeADO[matyExpmestADO.EXP_MEST_ID ?? 0] = new List<ExpMestMedicineSDO>();

                            threadMedicineADO.DicLstMediMateExpMestTypeADO[matyExpmestADO.EXP_MEST_ID ?? 0].Add(matyExpmestADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
