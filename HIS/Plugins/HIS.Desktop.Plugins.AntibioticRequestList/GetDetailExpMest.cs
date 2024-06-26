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
using HIS.Desktop.Plugins.AntibioticRequestList.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AntibioticRequestList
{
    class GetDetailExpMest
    {
        private V_HIS_EXP_MEST_2 RowData;
        private List<ExpDetailADO> Detail = new List<ExpDetailADO>();
        private List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs_Print;
        private List<V_HIS_EXP_MEST_MEDICINE_1> _ExpMestMedicines_Print;
        private List<V_HIS_EXP_MEST_MEDICINE_1> _ExpMestMetyReqs;
        private List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs_Print;
        private List<V_HIS_EXP_MEST_MATERIAL_1> _ExpMestMaterials_Print;
        private List<V_HIS_EXP_MEST_MATERIAL_1> _ExpMestMatyReqs;
        private List<HIS_SERVICE_REQ_MATY> ServiceReqMatys;
        private List<HIS_SERVICE_REQ_METY> ServiceReqMetys;
        private List<ExpMestBloodADO> _ExpMestBltyReqs;
        private List<ExpMestBloodADO> _ExpMestBloods;

        public GetDetailExpMest(V_HIS_EXP_MEST_2 rowData)
        {
            RowData = rowData;
        }

        public List<ExpDetailADO> Get()
        {
            List<ExpDetailADO> result = null;
            try
            {
                List<Task> taskall = new List<Task>();
                Task ts0 = Task.Factory.StartNew(() => { LoadExpMest(); });
                Task ts1 = Task.Factory.StartNew(() => { LoadExpMestMetyReq(); });
                Task ts2 = Task.Factory.StartNew(() => { LoadExpMestMatyReq(); });
                Task ts3 = Task.Factory.StartNew(() => { LoadExpMestBltyReq(); });
                Task ts4 = Task.Factory.StartNew(() => { LoadServiceReqMaty(); });
                Task ts5 = Task.Factory.StartNew(() => { LoadServiceReqMety(); });
                taskall.Add(ts0);
                taskall.Add(ts1);
                taskall.Add(ts2);
                taskall.Add(ts3);
                taskall.Add(ts4);
                taskall.Add(ts5);

                Task.WaitAll(taskall.ToArray());

                result = new List<ExpDetailADO>();

                List<V_HIS_EXP_MEST_MEDICINE_1> expMestMety = new List<V_HIS_EXP_MEST_MEDICINE_1>();
                if (_ExpMestMetyReqs != null && _ExpMestMetyReqs.Count > 0)
                {
                    expMestMety.AddRange(_ExpMestMetyReqs);
                }

                if (_ExpMestMedicines_Print != null && _ExpMestMedicines_Print.Count > 0)
                {
                    expMestMety.AddRange(_ExpMestMedicines_Print);
                }

                if (expMestMety != null && expMestMety.Count > 0)
                {
                    expMestMety = expMestMety.Where(o => o.EXP_MEST_STT_ID == RowData.EXP_MEST_STT_ID).ToList();
                    expMestMety = expMestMety.OrderBy(o => o.NUM_ORDER).ToList();
                    expMestMety = GroupExpMestMedicine(expMestMety);
                    foreach (var item in expMestMety)
                    {
                        ExpDetailADO ado = new ExpDetailADO();
                        ado.AMOUNT = item.AMOUNT;
                        ado.CONCENTRA = item.CONCENTRA;
                        ado.MEDI_MATE_CODE = item.MEDICINE_TYPE_CODE;
                        ado.MEDI_MATE_NAME = item.MEDICINE_TYPE_NAME;
                        ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        result.Add(ado);
                    }
                }

                List<V_HIS_EXP_MEST_MATERIAL_1> expMestMaty = new List<V_HIS_EXP_MEST_MATERIAL_1>();
                if (_ExpMestMatyReqs != null && _ExpMestMatyReqs.Count > 0)
                {
                    expMestMaty.AddRange(_ExpMestMatyReqs);
                }

                if (_ExpMestMaterials_Print != null && _ExpMestMaterials_Print.Count > 0)
                {
                    expMestMaty.AddRange(_ExpMestMaterials_Print);
                }

                if (expMestMaty != null && expMestMaty.Count > 0)
                {
                    expMestMaty = expMestMaty.Where(o => o.EXP_MEST_STT_ID == RowData.EXP_MEST_STT_ID).ToList();
                    expMestMaty = expMestMaty.OrderBy(o => o.NUM_ORDER).ToList();
                    expMestMaty = GroupExpMestMaterial(expMestMaty);
                    foreach (var item in expMestMaty)
                    {
                        ExpDetailADO ado = new ExpDetailADO();
                        ado.AMOUNT = item.AMOUNT;
                        ado.CONCENTRA = "";
                        ado.MEDI_MATE_CODE = item.MATERIAL_TYPE_CODE;
                        ado.MEDI_MATE_NAME = item.MATERIAL_TYPE_NAME;
                        ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        result.Add(ado);
                    }
                }

                List<ExpMestBloodADO> blt = new List<ExpMestBloodADO>();
                if (_ExpMestBltyReqs != null && _ExpMestBltyReqs.Count > 0)
                {
                    blt.AddRange(_ExpMestBltyReqs);
                }

                if (_ExpMestBloods != null && _ExpMestBloods.Count > 0)
                {
                    blt.AddRange(_ExpMestBloods);
                }

                if (blt != null && blt.Count > 0)
                {
                    blt = blt.Where(o => o.EXP_MEST_STT_ID == RowData.EXP_MEST_STT_ID).ToList();
                    blt = blt.OrderBy(o => o.NUM_ORDER).ToList();
                    foreach (var item in blt)
                    {
                        ExpDetailADO ado = new ExpDetailADO();
                        ado.AMOUNT = item.AMOUNT;
                        ado.CONCENTRA = "";
                        ado.MEDI_MATE_CODE = item.BLOOD_TYPE_CODE;
                        ado.MEDI_MATE_NAME = item.BLOOD_TYPE_NAME;
                        ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        result.Add(ado);
                    }
                }

                if (ServiceReqMatys != null && ServiceReqMatys.Count > 0)
                {
                    foreach (var item in ServiceReqMatys)
                    {
                        ExpDetailADO ado = new ExpDetailADO();
                        ado.AMOUNT = item.AMOUNT;
                        ado.MEDI_MATE_NAME = item.MATERIAL_TYPE_NAME;
                        ado.SERVICE_UNIT_NAME = item.UNIT_NAME;
                        if (item.MATERIAL_TYPE_ID.HasValue)
                        {
                            var typeName = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                            if (typeName != null)
                            {
                                ado.CONCENTRA = typeName.CONCENTRA;
                                ado.MEDI_MATE_CODE = typeName.MATERIAL_TYPE_CODE;
                            }
                        }

                        result.Add(ado);
                    }
                }

                if (ServiceReqMetys != null && ServiceReqMetys.Count > 0)
                {
                    foreach (var item in ServiceReqMetys)
                    {
                        ExpDetailADO ado = new ExpDetailADO();
                        ado.AMOUNT = item.AMOUNT;
                        ado.MEDI_MATE_NAME = item.MEDICINE_TYPE_NAME;
                        ado.SERVICE_UNIT_NAME = item.UNIT_NAME;
                        if (item.MEDICINE_TYPE_ID.HasValue)
                        {
                            var typeName = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            if (typeName != null)
                            {
                                ado.CONCENTRA = typeName.CONCENTRA;
                                ado.MEDI_MATE_CODE = typeName.MEDICINE_TYPE_CODE;
                            }
                        }

                        result.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //cập nhật trạng thái mới nhất
        private void LoadExpMest()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestFilter filter = new HisExpMestFilter();
                filter.ID = this.RowData.ID;
                var expMest = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, param);
                if (expMest != null && expMest.Count > 0)
                {
                    RowData.EXP_MEST_STT_ID = expMest.First().EXP_MEST_STT_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu thuốc yêu cầu
        /// </summary>
        private void LoadExpMestMetyReq()
        {
            try
            {
                _ExpMestMetyReqs_Print = new List<HIS_EXP_MEST_METY_REQ>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMetyReqFilter filter = new HisExpMestMetyReqFilter();
                filter.EXP_MEST_ID = this.RowData.ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisExpMestMetyReq/Get");
                _ExpMestMetyReqs_Print = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisExpMestMetyReq/Get");
                _ExpMestMetyReqs = new List<V_HIS_EXP_MEST_MEDICINE_1>();
                if (_ExpMestMetyReqs_Print != null && _ExpMestMetyReqs_Print.Count > 0)
                {
                    var dataGroups = _ExpMestMetyReqs_Print.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        V_HIS_EXP_MEST_MEDICINE_1 ado = new V_HIS_EXP_MEST_MEDICINE_1();
                        AutoMapper.Mapper.CreateMap<HIS_EXP_MEST_METY_REQ, V_HIS_EXP_MEST_MEDICINE_1>();
                        ado = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_MEDICINE_1>(item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST;
                        ado.NUM_ORDER = item.FirstOrDefault().NUM_ORDER;

                        var typeName = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item[0].MEDICINE_TYPE_ID);
                        if (typeName != null)
                        {
                            ado.MEDICINE_TYPE_NAME = typeName.MEDICINE_TYPE_NAME;
                            ado.MEDICINE_TYPE_CODE = typeName.MEDICINE_TYPE_CODE;
                            ado.SERVICE_UNIT_NAME = typeName.SERVICE_UNIT_NAME;
                            ado.MEDICINE_GROUP_ID = typeName.MEDICINE_GROUP_ID;
                            ado.CONVERT_RATIO = typeName.CONVERT_RATIO;
                            ado.CONVERT_UNIT_NAME = typeName.CONVERT_UNIT_NAME;
                            ado.CONCENTRA = typeName.CONCENTRA;
                        }
                        _ExpMestMetyReqs.Add(ado);
                    }
                }

                MOS.Filter.HisExpMestMedicineView1Filter MediFilter = new HisExpMestMedicineView1Filter();
                MediFilter.EXP_MEST_ID = this.RowData.ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisExpMestMedicine/GetView1");
                _ExpMestMedicines_Print = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE_1>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW1, ApiConsumers.MosConsumer, MediFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu Vat Tu yêu cầu
        /// </summary>
        private void LoadExpMestMatyReq()
        {
            try
            {
                _ExpMestMatyReqs_Print = new List<HIS_EXP_MEST_MATY_REQ>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMatyReqFilter filter = new HisExpMestMatyReqFilter();
                filter.EXP_MEST_ID = this.RowData.ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisExpMestMatyReq/Get");
                _ExpMestMatyReqs_Print = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisExpMestMatyReq/Get");
                _ExpMestMatyReqs = new List<V_HIS_EXP_MEST_MATERIAL_1>();
                if (_ExpMestMatyReqs_Print != null && _ExpMestMatyReqs_Print.Count > 0)
                {
                    var dataGroups = _ExpMestMatyReqs_Print.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        V_HIS_EXP_MEST_MATERIAL_1 ado = new V_HIS_EXP_MEST_MATERIAL_1();
                        AutoMapper.Mapper.CreateMap<HIS_EXP_MEST_MATY_REQ, V_HIS_EXP_MEST_MATERIAL_1>();
                        ado = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_MATERIAL_1>(item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST;
                        ado.NUM_ORDER = item.FirstOrDefault().NUM_ORDER;

                        var typeName = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item[0].MATERIAL_TYPE_ID);
                        if (typeName != null)
                        {
                            ado.MATERIAL_TYPE_NAME = typeName.MATERIAL_TYPE_NAME;
                            ado.MATERIAL_TYPE_CODE = typeName.MATERIAL_TYPE_CODE;
                            ado.SERVICE_UNIT_NAME = typeName.SERVICE_UNIT_NAME;
                            ado.IS_CHEMICAL_SUBSTANCE = typeName.IS_CHEMICAL_SUBSTANCE;
                            ado.CONVERT_RATIO = typeName.CONVERT_RATIO;
                            ado.CONVERT_UNIT_NAME = typeName.CONVERT_UNIT_NAME;
                        }
                        _ExpMestMatyReqs.Add(ado);
                    }
                }
                //_ExpMestMaterials_Print
                MOS.Filter.HisExpMestMaterialView1Filter mateFilter = new HisExpMestMaterialView1Filter();
                mateFilter.EXP_MEST_ID = this.RowData.ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisExpMestMaterial/GetView1");
                _ExpMestMaterials_Print = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL_1>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW1, ApiConsumers.MosConsumer, mateFilter, param);
                Inventec.Common.Logging.LogSystem.Info("ket thuc goi api: HisExpMestMaterial/GetView1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu Mau yêu cầu
        /// </summary>
        private void LoadExpMestBltyReq()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestBltyReqView1Filter filter = new HisExpMestBltyReqView1Filter();
                filter.EXP_MEST_ID = this.RowData.ID;
                Inventec.Common.Logging.LogSystem.Warn("Bat dau goi api HisExpMestBltyReq/GetView");
                var _ExpMestBltyReqs_Print = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLTY_REQ_1>>("/api/HisExpMestBltyReq/GetView1", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Warn("Ket thuc goi api HisExpMestBltyReq/GetView");
                _ExpMestBltyReqs = new List<ExpMestBloodADO>();
                if (_ExpMestBltyReqs_Print != null && _ExpMestBltyReqs_Print.Count > 0)
                {
                    var dataGroups = _ExpMestBltyReqs_Print.GroupBy(p => p.BLOOD_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var itemGroup in dataGroups)
                    {
                        var _bloodTypes = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>();
                        ExpMestBloodADO ado = new ExpMestBloodADO();
                        var firstItem = itemGroup.First();
                        ado.AMOUNT = itemGroup.Sum(o => o.AMOUNT);
                        ado.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST;
                        ado.NUM_ORDER = firstItem.NUM_ORDER;
                        var data = _bloodTypes.FirstOrDefault(p => p.ID == itemGroup[0].BLOOD_TYPE_ID);
                        if (data != null)
                        {
                            ado.BLOOD_TYPE_CODE = firstItem.BLOOD_TYPE_CODE;
                            ado.BLOOD_TYPE_ID = data.ID;
                            ado.BLOOD_RH_CODE = firstItem.BLOOD_RH_CODE;
                            ado.BLOOD_ABO_CODE = firstItem.BLOOD_ABO_CODE;
                            ado.BLOOD_TYPE_NAME = firstItem.BLOOD_TYPE_NAME;
                            ado.SERVICE_UNIT_CODE = firstItem.SERVICE_UNIT_CODE;
                            ado.SERVICE_UNIT_NAME = firstItem.SERVICE_UNIT_NAME;
                            ado.VOLUME = data.VOLUME;
                        }
                        _ExpMestBltyReqs.Add(ado);
                    }
                }

                _ExpMestBloods = new List<ExpMestBloodADO>();
                MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                bloodFilter.EXP_MEST_ID = this.RowData.ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisExpMestBlood/GetView");
                var _ExpMestBloods_Print = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>("/api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, bloodFilter, param);
                Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisExpMestBlood/GetView");
                if (_ExpMestBloods_Print != null && _ExpMestBloods_Print.Count > 0)
                {
                    List<V_HIS_EXP_MEST_BLOOD> expMestBloodTemps = new List<V_HIS_EXP_MEST_BLOOD>();
                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_BLOOD, V_HIS_EXP_MEST_BLOOD>();
                    expMestBloodTemps = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_BLOOD>>(_ExpMestBloods_Print);
                    var dataGroups = expMestBloodTemps.GroupBy(p => new { p.BLOOD_TYPE_ID, p.PRICE, p.IMP_PRICE, p.VOLUME }).Select(p => p.ToList()).ToList();
                    foreach (var itemGroup in dataGroups)
                    {
                        ExpMestBloodADO ado = new ExpMestBloodADO(itemGroup[0]);
                        ado.AMOUNT = itemGroup.Count();
                        _ExpMestBloods.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu vật tư ngoài kho
        /// </summary>
        private void LoadServiceReqMaty()
        {
            try
            {
                if (this.RowData == null || this.RowData.SERVICE_REQ_ID == null)
                    return;

                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMatyFilter filter = new HisServiceReqMatyFilter();
                filter.SERVICE_REQ_ID = this.RowData.SERVICE_REQ_ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisServiceReqMaty/Get");
                ServiceReqMatys = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisServiceReqMaty/Get");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu thuốc ngoài kho
        /// </summary>
        private void LoadServiceReqMety()
        {
            try
            {
                if (this.RowData == null || this.RowData.SERVICE_REQ_ID == null)
                    return;

                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMetyFilter filter = new HisServiceReqMetyFilter();
                filter.SERVICE_REQ_ID = this.RowData.SERVICE_REQ_ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisServiceReqMety/Get");
                ServiceReqMetys = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisServiceReqMety/Get");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<V_HIS_EXP_MEST_MEDICINE_1> GroupExpMestMedicine(List<V_HIS_EXP_MEST_MEDICINE_1> expMestMedicine1s)
        {
            if (expMestMedicine1s == null || expMestMedicine1s.Count == 0)
            {
                return new List<V_HIS_EXP_MEST_MEDICINE_1>();
            }

            List<V_HIS_EXP_MEST_MEDICINE_1> expMestmedicineTemps = new List<V_HIS_EXP_MEST_MEDICINE_1>();

            AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE_1, V_HIS_EXP_MEST_MEDICINE_1>();
            expMestmedicineTemps = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_MEDICINE_1>>(expMestMedicine1s);

            List<V_HIS_EXP_MEST_MEDICINE_1> result = new List<V_HIS_EXP_MEST_MEDICINE_1>();
            try
            {
                var dataGroups = expMestmedicineTemps.GroupBy(o => new
                {
                    o.MEDICINE_TYPE_ID,
                    o.PRICE,
                    o.IMP_PRICE,
                    o.IMP_VAT_RATIO,
                    o.IS_NOT_PRES
                }).ToList();

                foreach (var dataGroup in dataGroups)
                {
                    V_HIS_EXP_MEST_MEDICINE_1 expMestmedicine = new V_HIS_EXP_MEST_MEDICINE_1();
                    expMestmedicine = dataGroup.First();
                    expMestmedicine.AMOUNT = dataGroup.Sum(o => o.AMOUNT);
                    expMestmedicine.SUM_BY_MEDICINE_IN_STOCK = dataGroup.Sum(o => o.SUM_BY_MEDICINE_IN_STOCK);
                    result.Add(expMestmedicine);
                }

                result = result.OrderBy(o => o.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_EXP_MEST_MEDICINE_1>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<V_HIS_EXP_MEST_MATERIAL_1> GroupExpMestMaterial(List<V_HIS_EXP_MEST_MATERIAL_1> expMestMedicine1s)
        {
            if (expMestMedicine1s == null || expMestMedicine1s.Count == 0)
            {
                return new List<V_HIS_EXP_MEST_MATERIAL_1>();
            }

            List<V_HIS_EXP_MEST_MATERIAL_1> result = new List<V_HIS_EXP_MEST_MATERIAL_1>();
            try
            {
                List<V_HIS_EXP_MEST_MATERIAL_1> expMestMaterialTemp = new List<V_HIS_EXP_MEST_MATERIAL_1>();
                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL_1, V_HIS_EXP_MEST_MATERIAL_1>();
                expMestMaterialTemp = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_MATERIAL_1>>(expMestMedicine1s);

                var dataGroups = expMestMaterialTemp.GroupBy(o => new
                {
                    o.MATERIAL_TYPE_ID,
                    o.PRICE,
                    o.IMP_PRICE,
                    o.IMP_VAT_RATIO,
                    o.IS_NOT_PRES
                }).ToList();

                foreach (var dataGroup in dataGroups)
                {
                    V_HIS_EXP_MEST_MATERIAL_1 expMestmedicine = new V_HIS_EXP_MEST_MATERIAL_1();
                    expMestmedicine = dataGroup.First();
                    expMestmedicine.AMOUNT = dataGroup.Sum(o => o.AMOUNT);
                    result.Add(expMestmedicine);
                }
                result = result != null ? result.OrderBy(o => o.NUM_ORDER).ToList() : result;
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_EXP_MEST_MATERIAL_1>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
