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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PublicServices_NT.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PublicServices_NT
{
    public partial class frmPublicServices_NT : HIS.Desktop.Utility.FormBase
    {
        internal List<Service_NT_ADO> _Datas { get; set; }
        /// <summary>
        /// Trong Kho
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadLoadData(object param)
        {
            Thread threadMedicine = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMedicineNewThread));
            Thread threadMaterial = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMaterialNewThread));
            Thread threadBlood = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataBloodNewThread));

            try
            {
                threadMedicine.Start(param);
                threadMaterial.Start(param);
                threadBlood.Start(param);

                threadMedicine.Join();
                threadMaterial.Join();
                threadBlood.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMedicine.Abort();
                threadMaterial.Abort();
                threadBlood.Abort();
            }
        }

        private void LoadDataMedicineNewThread(object param)
        {
            try
            {
                LoadDataMedicine((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicine(List<long> _expMestIds)
        {
            try
            {
                if (_expMestIds != null && _expMestIds.Count > 0)
                {
                    List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                    int skip = 0;
                    while (_expMestIds.Count - skip > 0)
                    {
                        var listIds = _expMestIds.Skip(skip).Take(MaxReqFilter).ToList();
                        skip += MaxReqFilter;

                        CommonParam param = new CommonParam();
                        MOS.Filter.HisExpMestMedicineViewFilter expMestMediFilter = new HisExpMestMedicineViewFilter();
                        expMestMediFilter.EXP_MEST_IDs = listIds;
                        if (!chkHaoPhi.Checked)
                        {
                            expMestMediFilter.IS_EXPEND = false;
                        }

                        var _ExpMestMedis = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMediFilter, param);
                        if (_ExpMestMedis != null && _ExpMestMedis.Count > 0)
                        {
                            _ExpMestMedicines.AddRange(_ExpMestMedis);
                        }
                    }

                    _ExpMestMedicines = _ExpMestMedicines != null ? _ExpMestMedicines.Where(o => CheckPatientTypeInListAllow(o.PATIENT_TYPE_ID ?? 0)).ToList() : null;
                    if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0 && this.dicExpMest != null && this.dicExpMest.Count > 0)
                    {
                        var expMestMedicineGroups = _ExpMestMedicines.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.MEDICINE_ID, p.EXP_MEST_ID, p.PRICE, p.CONCENTRA, p.PATIENT_TYPE_ID, p.IS_EXPEND }).Select(p => p.ToList()).ToList();
                        foreach (var itemGroups in expMestMedicineGroups)
                        {
                            if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID ?? 0))
                            {
                                var _expMest = this.dicExpMest[itemGroups[0].EXP_MEST_ID ?? 0];
                                if (_expMest != null)
                                {
                                    decimal _AMOUNT = itemGroups.Sum(p => p.AMOUNT - (p.TH_AMOUNT ?? 0));
                                    Service_NT_ADO expMedi = new Service_NT_ADO();
                                    expMedi.DESCRIPTION = itemGroups[0].DESCRIPTION;
                                    expMedi.PRICE = itemGroups[0].PRICE;
                                    expMedi.SERVICE_CODE = itemGroups[0].MEDICINE_TYPE_CODE;
                                    expMedi.SERVICE_ID = itemGroups[0].MEDICINE_TYPE_ID;
                                    expMedi.SERVICE_NAME = itemGroups[0].MEDICINE_TYPE_NAME;
                                    expMedi.SERVICE_UNIT_CODE = itemGroups[0].SERVICE_UNIT_CODE;
                                    expMedi.SERVICE_UNIT_ID = itemGroups[0].SERVICE_UNIT_ID;
                                    expMedi.SERVICE_UNIT_NAME = itemGroups[0].SERVICE_UNIT_NAME;
                                    expMedi.INTRUCTION_DATE = ConvertToOutputFormat(dicServiceReq[_expMest.SERVICE_REQ_ID ?? 0].USE_TIME) ?? ConvertToOutputFormat(_expMest.TDL_INTRUCTION_DATE) ?? 0;
                                    expMedi.IS_EXPEND = itemGroups[0].IS_EXPEND;
                                    expMedi.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                                    expMedi.AMOUNT = _AMOUNT;
                                    expMedi.CONCENTRA = itemGroups[0].CONCENTRA;
                                    expMedi.MORNING = itemGroups.Sum(s => FormatSessionOfDay(s.MORNING)).ToString();
                                    expMedi.EVENING = itemGroups.Sum(s => FormatSessionOfDay(s.EVENING)).ToString();
                                    expMedi.AFTERNOON = itemGroups.Sum(s => FormatSessionOfDay(s.AFTERNOON)).ToString();
                                    expMedi.NOON = itemGroups.Sum(s => FormatSessionOfDay(s.NOON)).ToString();
                                    expMedi.PATIENT_TYPE_ID = itemGroups[0].PATIENT_TYPE_ID ?? 0;
                                    //Cong khai thuc hien cua thuoc: uu tien USED_TIME (thoi gian dung).
                                    //USED_TIME chi duoc ghi khi dieu duong danh dau da dung tren man hinh rieng,
                                    //nen phai lui ve EXP_TIME (thoi diem xuat kho) de cot khong bi trong.
                                    long? usedTimeMedi = itemGroups
                                        .Where(o => o.USED_TIME.HasValue && o.USED_TIME.Value > 0)
                                        .Max(o => o.USED_TIME);
                                    if (!usedTimeMedi.HasValue)
                                    {
                                        usedTimeMedi = itemGroups
                                            .Where(o => o.EXP_TIME.HasValue && o.EXP_TIME.Value > 0)
                                            .Max(o => o.EXP_TIME);
                                    }
                                    expMedi.EXECUTE_PUBLIC_TIME = usedTimeMedi;
                                    //Thanh tien BHYT tra / BN tra: doi chieu sang sere_serv theo id dong xuat kho
                                    SetHeinPatientPriceByExpMedicine(expMedi, itemGroups
                                        .Select(o => new ExpLineForPrice { Id = o.ID, Amount = o.AMOUNT, ThAmount = o.TH_AMOUNT ?? 0 })
                                        .ToList());
                                    _Datas.Add(expMedi);
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

        private bool CheckPatientTypeInListAllow(long patientTypeId)
        {
            return this.patientTypeHasSelecteds != null ? this.patientTypeHasSelecteds.Any(o => o.ID == patientTypeId) : false;
        }

        private void LoadDataMaterialNewThread(object param)
        {
            try
            {
                LoadDataMaterial((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterial(List<long> _expMestIds)
        {
            try
            {
                if (_expMestIds != null && _expMestIds.Count > 0)
                {
                    List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                    int skip = 0;
                    while (_expMestIds.Count - skip > 0)
                    {
                        var listIds = _expMestIds.Skip(skip).Take(MaxReqFilter).ToList();
                        skip += MaxReqFilter;

                        CommonParam param = new CommonParam();
                        MOS.Filter.HisExpMestMaterialViewFilter expMestMateFilter = new HisExpMestMaterialViewFilter();
                        expMestMateFilter.EXP_MEST_IDs = listIds;
                        if (!chkHaoPhi.Checked)
                        {
                            expMestMateFilter.IS_EXPEND = false;
                        }
                        var _ExpMestMate = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMateFilter, param);

                        if (_ExpMestMate != null && _ExpMestMate.Count > 0)
                        {
                            _ExpMestMaterials.AddRange(_ExpMestMate);
                        }
                    }

                    _ExpMestMaterials = _ExpMestMaterials != null ? _ExpMestMaterials.Where(o => CheckPatientTypeInListAllow(o.PATIENT_TYPE_ID ?? 0)).ToList() : null;
                    if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0 && this.dicExpMest != null && this.dicExpMest.Count > 0)
                    {
                        var expMestMaterialGroups = _ExpMestMaterials.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.MATERIAL_ID, p.EXP_MEST_ID, p.PRICE, p.PATIENT_TYPE_ID, p.IS_EXPEND }).Select(p => p.ToList()).ToList();
                        foreach (var itemGroups in expMestMaterialGroups)
                        {
                            if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID ?? 0))
                            {
                                var _expMest = this.dicExpMest[itemGroups[0].EXP_MEST_ID ?? 0];
                                if (_expMest != null)
                                {
                                    decimal _AMOUNT = itemGroups.Sum(p => p.AMOUNT - (p.TH_AMOUNT ?? 0));
                                    Service_NT_ADO expMate = new Service_NT_ADO();
                                    expMate.DESCRIPTION = itemGroups[0].DESCRIPTION;
                                    expMate.PRICE = itemGroups[0].PRICE;
                                    expMate.SERVICE_CODE = itemGroups[0].MATERIAL_TYPE_CODE;
                                    expMate.SERVICE_ID = itemGroups[0].MATERIAL_TYPE_ID;
                                    expMate.SERVICE_NAME = itemGroups[0].MATERIAL_TYPE_NAME;
                                    expMate.SERVICE_UNIT_CODE = itemGroups[0].SERVICE_UNIT_CODE;
                                    expMate.SERVICE_UNIT_ID = itemGroups[0].SERVICE_UNIT_ID;
                                    expMate.SERVICE_UNIT_NAME = itemGroups[0].SERVICE_UNIT_NAME;
                                    expMate.INTRUCTION_DATE = ConvertToOutputFormat(dicServiceReq[_expMest.SERVICE_REQ_ID ?? 0].USE_TIME) ?? _expMest.TDL_INTRUCTION_DATE ?? 0;
                                    expMate.IS_EXPEND = itemGroups[0].IS_EXPEND;
                                    expMate.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                                    expMate.AMOUNT = _AMOUNT;
                                    expMate.PATIENT_TYPE_ID = itemGroups[0].PATIENT_TYPE_ID ?? 0;
                                    var concentra = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == itemGroups[0].MATERIAL_TYPE_ID);
                                    if (concentra != null)
                                    {
                                        expMate.CONCENTRA = concentra.CONCENTRA;
                                    }
                                    //Cong khai thuc hien cua vat tu: uu tien USED_TIME, lui ve EXP_TIME
                                    long? usedTimeMate = itemGroups
                                        .Where(o => o.USED_TIME.HasValue && o.USED_TIME.Value > 0)
                                        .Max(o => o.USED_TIME);
                                    if (!usedTimeMate.HasValue)
                                    {
                                        usedTimeMate = itemGroups
                                            .Where(o => o.EXP_TIME.HasValue && o.EXP_TIME.Value > 0)
                                            .Max(o => o.EXP_TIME);
                                    }
                                    expMate.EXECUTE_PUBLIC_TIME = usedTimeMate;
                                    //Thanh tien BHYT tra / BN tra: doi chieu sang sere_serv theo id dong xuat kho
                                    SetHeinPatientPriceByExpMaterial(expMate, itemGroups
                                        .Select(o => new ExpLineForPrice { Id = o.ID, Amount = o.AMOUNT, ThAmount = o.TH_AMOUNT ?? 0 })
                                        .ToList());
                                    _Datas.Add(expMate);
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

        private void LoadDataBloodNewThread(object param)
        {
            try
            {
                LoadDataBlood((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBlood(List<long> _expMestIds)
        {
            try
            {
                if (_expMestIds != null && _expMestIds.Count > 0)
                {
                    List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloods = new List<V_HIS_EXP_MEST_BLOOD>();
                    int skip = 0;
                    while (_expMestIds.Count - skip > 0)
                    {
                        var listIds = _expMestIds.Skip(skip).Take(MaxReqFilter).ToList();
                        skip += MaxReqFilter;
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                        bloodFilter.EXP_MEST_IDs = listIds;

                        var _ExpMestBls = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);
                        if (_ExpMestBls != null && _ExpMestBls.Count > 0)
                        {
                            _ExpMestBloods.AddRange(_ExpMestBls);
                        }
                    }

                    _ExpMestBloods = _ExpMestBloods != null ? _ExpMestBloods.Where(o => CheckPatientTypeInListAllow(o.PATIENT_TYPE_ID ?? 0)).ToList() : null;
                    if (_ExpMestBloods != null && _ExpMestBloods.Count > 0 && this.dicExpMest != null && this.dicExpMest.Count > 0)
                    {
                        var expMestBllodGroups = _ExpMestBloods.GroupBy(p => new { p.BLOOD_ID, p.EXP_MEST_ID, p.PATIENT_TYPE_ID }).Select(p => p.ToList()).ToList();
                        foreach (var itemGroups in expMestBllodGroups)
                        {
                            if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID))
                            {
                                var _expMest = this.dicExpMest[itemGroups[0].EXP_MEST_ID];
                                if (_expMest != null)
                                {
                                    decimal _AMOUNT = itemGroups.Count;
                                    Service_NT_ADO expMate = new Service_NT_ADO();
                                    expMate.DESCRIPTION = itemGroups[0].DESCRIPTION;
                                    expMate.PRICE = itemGroups[0].PRICE;
                                    expMate.SERVICE_CODE = itemGroups[0].BLOOD_TYPE_CODE;
                                    expMate.SERVICE_ID = itemGroups[0].BLOOD_TYPE_ID;
                                    expMate.SERVICE_NAME = itemGroups[0].BLOOD_TYPE_NAME;
                                    expMate.SERVICE_UNIT_CODE = itemGroups[0].SERVICE_UNIT_CODE;
                                    expMate.SERVICE_UNIT_ID = itemGroups[0].SERVICE_UNIT_ID;
                                    expMate.SERVICE_UNIT_NAME = itemGroups[0].SERVICE_UNIT_NAME;
                                    expMate.INTRUCTION_DATE = _expMest.TDL_INTRUCTION_DATE ?? 0;
                                    expMate.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU;
                                    expMate.AMOUNT = _AMOUNT;
                                    expMate.PATIENT_TYPE_ID = itemGroups[0].PATIENT_TYPE_ID ?? 0;
                                    //Cong khai thuc hien cua mau: view mau chi co EXP_TIME, khong co USED_TIME
                                    expMate.EXECUTE_PUBLIC_TIME = itemGroups
                                        .Where(o => o.EXP_TIME.HasValue && o.EXP_TIME.Value > 0)
                                        .Max(o => o.EXP_TIME);
                                    //Thanh tien BHYT tra / BN tra: doi chieu sang sere_serv theo BLOOD_ID.
                                    //Mau khong co TH_AMOUNT nen moi tui tinh tron so tien (ty le = 1).
                                    SetHeinPatientPriceByBlood(expMate, itemGroups
                                        .Select(o => new ExpLineForPrice { Id = o.BLOOD_ID, Amount = 1, ThAmount = 0 })
                                        .ToList());
                                    _Datas.Add(expMate);
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
        /// <summary>
        /// Mot dong xuat kho dung cho viec doi chieu tien: id dong, so da xuat, so da tra lai.
        /// </summary>
        private class ExpLineForPrice
        {
            public long Id { get; set; }
            public decimal Amount { get; set; }
            public decimal ThAmount { get; set; }
        }

        /// <summary>
        /// Cong tien BHYT tra / BN tra cua cac dong xuat kho thuoc trong nhom.
        /// </summary>
        private void SetHeinPatientPriceByExpMedicine(Service_NT_ADO ado, List<ExpLineForPrice> expLines)
        {
            SetHeinPatientPrice(ado, expLines, this.dicSereServByExpMedicine);
        }

        /// <summary>
        /// Cong tien BHYT tra / BN tra cua cac dong xuat kho vat tu trong nhom.
        /// </summary>
        private void SetHeinPatientPriceByExpMaterial(Service_NT_ADO ado, List<ExpLineForPrice> expLines)
        {
            SetHeinPatientPrice(ado, expLines, this.dicSereServByExpMaterial);
        }

        /// <summary>
        /// Cong tien BHYT tra / BN tra cua dong mau trong nhom.
        /// </summary>
        private void SetHeinPatientPriceByBlood(Service_NT_ADO ado, List<ExpLineForPrice> expLines)
        {
            SetHeinPatientPrice(ado, expLines, this.dicSereServByBlood);
        }

        /// <summary>
        /// VIR_TOTAL_* la tien cua ca dong (da nhan so luong) nen cong don, khong nhan lai voi AMOUNT.
        /// Rieng truong hop tra lai hang: so luong in ra da tru TH_AMOUNT nen tien cung phai
        /// tinh theo ty le phan con lai, neu khong se in so luong it ma tien nhieu.
        /// </summary>
        private void SetHeinPatientPrice(Service_NT_ADO ado, List<ExpLineForPrice> expLines, Dictionary<long, V_HIS_SERE_SERV> dicSereServ)
        {
            try
            {
                if (ado == null || expLines == null || expLines.Count <= 0 || dicSereServ == null || dicSereServ.Count <= 0)
                    return;

                decimal? totalHein = null;
                decimal? totalPatient = null;
                foreach (var line in expLines)
                {
                    if (line == null)
                        continue;

                    V_HIS_SERE_SERV sereServ = null;
                    if (!dicSereServ.TryGetValue(line.Id, out sereServ) || sereServ == null)
                        continue;

                    //Ty le phan con lai sau khi tra hang. Xuat 10 tra 4 thi chi tinh 6/10 so tien.
                    decimal gross = line.Amount;
                    decimal net = line.Amount - line.ThAmount;
                    decimal factor = gross > 0 ? (net / gross) : 0;

                    if (sereServ.VIR_TOTAL_HEIN_PRICE.HasValue)
                    {
                        totalHein = (totalHein ?? 0) + sereServ.VIR_TOTAL_HEIN_PRICE.Value * factor;
                    }
                    if (sereServ.VIR_TOTAL_PATIENT_PRICE.HasValue)
                    {
                        totalPatient = (totalPatient ?? 0) + sereServ.VIR_TOTAL_PATIENT_PRICE.Value * factor;
                    }
                }

                ado.TOTAL_HEIN_PRICE = totalHein;
                ado.TOTAL_PATIENT_PRICE = totalPatient;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public long? ConvertToOutputFormat(long? input)
        {
            long? outputLong = null;
            try
            {
                if (input == null)
                    return outputLong;
                long? yearMonthDay = input / 1000000; // Lấy năm tháng ngày
                outputLong = yearMonthDay * 1000000; // Gán giờ phút giây bằng 000000
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return outputLong;
        }
    }
}
