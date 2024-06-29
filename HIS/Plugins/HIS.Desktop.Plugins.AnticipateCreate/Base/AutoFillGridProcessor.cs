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
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AnticipateCreate.Base
{
    class AutoFillGridProcessor
    {
        private long TimeFrom;
        private long TimeTo;
        private List<long> MediStockIds;
        private long SpinCoefficient;

        internal List<HIS_BID> ListBid;
        internal List<V_HIS_BID_MATERIAL_TYPE> ListBidMaterialType;
        internal List<V_HIS_BID_MEDICINE_TYPE> ListBidMedicineType;

        internal List<HIS_MEDICINE> ListMedicine;
        internal List<HIS_MATERIAL> ListMaterial;

        //private List<V_HIS_IMP_MEST_MATERIAL> ListImpMestMaterial;
        //private List<V_HIS_IMP_MEST_MEDICINE> ListImpMestMedicine;

        private List<HisMedicineInStockSDO> ListMedicineInStockSDO;
        private List<HisMaterialInStockSDO> ListMaterialInStockSDO;

        private List<V_HIS_EXP_MEST_MATERIAL> ListExpMestMaterial;
        private List<V_HIS_EXP_MEST_MEDICINE> ListExpMestMedicine;

        private List<long> ExpMestTypeNotIn = new List<long>()
        {
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP
        };

        public AutoFillGridProcessor(long timeFrom, long timeTo, List<long> mediStockIds, long spinCoefficient)
        {
            this.TimeFrom = timeFrom;
            this.TimeTo = timeTo;
            this.MediStockIds = mediStockIds;
            this.SpinCoefficient = spinCoefficient;
        }

        //lấy lượng thuốc, vật tư xuất ra khỏi kho trong khoảng thời gian.
        // số lượng  = tổng số xuất ra - số tồn(nhập mà chưa xuất) - số lượng thầu chưa nhập.
        // không dùng cái này
        internal List<ADO.MedicineTypeADO> GetListMedicineTypeAdo1()
        {
            List<ADO.MedicineTypeADO> result = null;
            try
            {
                //Thread nhập xuất
                CreateThreadGetData();

                List<long> ListMaterialTypeId = new List<long>();
                if (ListExpMestMaterial != null && ListExpMestMaterial.Count > 0)
                {
                    ListMaterialTypeId = ListExpMestMaterial.GroupBy(o => o.TDL_MATERIAL_TYPE_ID).Select(s => s.Key ?? 0).ToList();
                }

                List<long> ListMedicineTypeId = new List<long>();
                if (ListExpMestMedicine != null && ListExpMestMedicine.Count > 0)
                {
                    ListMedicineTypeId = ListExpMestMedicine.GroupBy(o => o.TDL_MEDICINE_TYPE_ID).Select(s => s.Key ?? 0).ToList();
                }

                if ((ListMedicineTypeId != null && ListMedicineTypeId.Count > 0) || (ListMaterialTypeId != null && ListMaterialTypeId.Count > 0))
                {
                    result = new List<ADO.MedicineTypeADO>();

                    if (ListMedicineTypeId != null && ListMedicineTypeId.Count > 0)
                    {
                        ListMedicineTypeId = ListMedicineTypeId.Distinct().ToList();
                        foreach (var medicineTypeId in ListMedicineTypeId)
                        {
                            decimal amount = 0;
                            var expMestMedicine = ListExpMestMedicine.Where(o => o.TDL_MEDICINE_TYPE_ID == medicineTypeId && o.MEDICINE_ID.HasValue).ToList();
                            if (expMestMedicine != null && expMestMedicine.Count > 0)
                            {
                                amount = expMestMedicine.Sum(s => s.AMOUNT);
                            }

                            var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeId);
                            if (medicineType != null)
                            {
                                var bidMedicineTypes = ListBidMedicineType.Where(o => o.MEDICINE_TYPE_ID == medicineType.ID).ToList();
                                if (bidMedicineTypes != null && bidMedicineTypes.Count > 0)
                                {
                                    foreach (var item in bidMedicineTypes)
                                    {
                                        ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO();
                                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(ado, medicineType);

                                        ado.IdRow = setIdRow(result);
                                        ado.AMOUNT = 0;
                                        ado.Type = Base.GlobalConfig.THUOC;
                                        var bid = this.ListBid.FirstOrDefault(o => o.ID == item.BID_ID);
                                        if (bid != null)
                                        {
                                            ado.BID_ID = bid.ID;
                                            ado.BID_NUMBER = bid.BID_NUMBER;
                                            ado.BID_YEAR = bid.BID_YEAR;
                                        }

                                        var medicines = ListMedicine.Where(o => o.MEDICINE_TYPE_ID == medicineType.ID && o.BID_ID == item.BID_ID).ToList();
                                        if (medicines != null && medicines.Count > 0)
                                        {
                                            var meExpMestMedicine = expMestMedicine.Where(o => medicines.Select(s => s.ID).Contains(o.MEDICINE_ID ?? 0)).ToList();

                                            if (meExpMestMedicine != null && meExpMestMedicine.Count > 0)
                                            {
                                                ado.AMOUNT_EXPORT = meExpMestMedicine.Sum(s => s.AMOUNT);
                                                amount -= ado.AMOUNT_EXPORT ?? 0;
                                            }
                                            else
                                                continue;

                                            var medicineInstocks = ListMedicineInStockSDO.Where(o => o.MEDICINE_TYPE_ID == medicineType.ID && medicines.Select(s => s.ID).Contains(o.ID)).ToList();
                                            if (medicineInstocks != null && medicineInstocks.Count > 0)
                                            {
                                                ado.AMOUNT_MEDI_STOCK = medicineInstocks.Sum(o => o.TotalAmount);
                                            }

                                            ado.AMOUNT = (ado.AMOUNT_EXPORT ?? 0) - (ado.AMOUNT_MEDI_STOCK ?? 0) - (item.AMOUNT - medicines.Sum(s => s.AMOUNT));

                                            ado.IMP_PRICE = medicines.First().IMP_PRICE * (1 + medicines.First().IMP_VAT_RATIO);

                                            var supMedi = medicines.Where(o => result.Exists(e => e.ID == o.MEDICINE_TYPE_ID) && !result.Select(s => s.SUPPLIER_ID).Contains(o.SUPPLIER_ID)).ToList();
                                            var supplier = new HIS_SUPPLIER();
                                            if (supMedi != null && supMedi.Count > 0)
                                                supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == supMedi.First().SUPPLIER_ID);
                                            else
                                                supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == medicines.First().SUPPLIER_ID);

                                            if (supplier != null)
                                            {
                                                ado.SUPPLIER_ID = supplier.ID;
                                                ado.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                                                ado.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                                            }

                                            result.Add(ado);
                                        }
                                    }
                                }

                                if (amount > 0)
                                {
                                    ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(ado, medicineType);

                                    ado.IdRow = setIdRow(result);
                                    ado.Type = Base.GlobalConfig.THUOC;
                                    ado.AMOUNT_EXPORT = amount;

                                    var medicine = ListMedicine.Where(o => o.MEDICINE_TYPE_ID == medicineType.ID && !o.BID_ID.HasValue).ToList();
                                    if (medicine != null && medicine.Count > 0)
                                    {
                                        var medicineInstocks = ListMedicineInStockSDO.Where(o => o.MEDICINE_TYPE_ID == medicineType.ID && medicine.Select(s => s.ID).Contains(o.ID)).ToList();
                                        if (medicineInstocks != null && medicineInstocks.Count > 0)
                                        {
                                            ado.AMOUNT_MEDI_STOCK = medicineInstocks.Sum(o => o.TotalAmount);
                                        }

                                        ado.IMP_PRICE = medicine.First().IMP_PRICE * (1 + medicine.First().IMP_VAT_RATIO);

                                        var supMedi = medicine.Where(o => result.Exists(e => e.ID == o.MEDICINE_TYPE_ID) && !result.Select(s => s.SUPPLIER_ID).Contains(o.SUPPLIER_ID)).ToList();
                                        var supplier = new HIS_SUPPLIER();
                                        if (supMedi != null && supMedi.Count > 0)
                                            supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == supMedi.First().SUPPLIER_ID);
                                        else
                                            supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == medicine.First().SUPPLIER_ID);

                                        if (supplier != null)
                                        {
                                            ado.SUPPLIER_ID = supplier.ID;
                                            ado.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                                            ado.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                                        }
                                    }
                                    ado.AMOUNT = (ado.AMOUNT_EXPORT ?? 0) - (ado.AMOUNT_MEDI_STOCK ?? 0);

                                    result.Add(ado);
                                }
                            }
                        }
                    }

                    if (ListMaterialTypeId != null && ListMaterialTypeId.Count > 0)
                    {
                        ListMaterialTypeId = ListMaterialTypeId.Distinct().ToList();
                        foreach (var materialTypeId in ListMaterialTypeId)
                        {
                            decimal amount = 0;
                            var expMestMaterial = ListExpMestMaterial.Where(o => o.TDL_MATERIAL_TYPE_ID == materialTypeId && o.MATERIAL_ID.HasValue).ToList();
                            if (expMestMaterial != null && expMestMaterial.Count > 0)
                            {
                                amount = expMestMaterial.Sum(s => s.AMOUNT);
                            }

                            var materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == materialTypeId);
                            if (materialType != null)
                            {
                                var bidMaterialTypes = ListBidMaterialType.Where(o => o.MATERIAL_TYPE_ID == materialType.ID).ToList();
                                if (bidMaterialTypes != null && bidMaterialTypes.Count > 0)
                                {
                                    foreach (var item in bidMaterialTypes)
                                    {
                                        ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO();
                                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(ado, materialType);

                                        ado.IdRow = setIdRow(result);
                                        ado.AMOUNT = 0;
                                        ado.MEDICINE_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                                        ado.MEDICINE_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                                        ado.Type = Base.GlobalConfig.VATTU;
                                        var bid = this.ListBid.FirstOrDefault(o => o.ID == item.BID_ID);
                                        if (bid != null)
                                        {
                                            ado.BID_ID = bid.ID;
                                            ado.BID_NUMBER = bid.BID_NUMBER;
                                            ado.BID_YEAR = bid.BID_YEAR;
                                        }

                                        var materials = ListMaterial.Where(o => o.MATERIAL_TYPE_ID == materialType.ID && o.BID_ID == item.BID_ID).ToList();
                                        if (materials != null && materials.Count > 0)
                                        {
                                            var materialInstocks = ListMaterialInStockSDO.Where(o => o.MATERIAL_TYPE_ID == materialType.ID && materials.Select(s => s.ID).Contains(o.ID)).ToList();
                                            if (materialInstocks != null && materialInstocks.Count > 0)
                                            {
                                                ado.AMOUNT_MEDI_STOCK = materialInstocks.Sum(o => o.TotalAmount);
                                            }

                                            var meExpMestMaterial = expMestMaterial.Where(o => materials.Select(s => s.ID).Contains(o.MATERIAL_ID ?? 0)).ToList();

                                            if (meExpMestMaterial != null && meExpMestMaterial.Count > 0)
                                            {
                                                ado.AMOUNT_EXPORT = meExpMestMaterial.Sum(s => s.AMOUNT);
                                                amount -= ado.AMOUNT_EXPORT ?? 0;
                                            }
                                            else
                                                continue;

                                            ado.AMOUNT = (ado.AMOUNT_EXPORT ?? 0) - (ado.AMOUNT_MEDI_STOCK ?? 0) - (item.AMOUNT - materials.Sum(s => s.AMOUNT));
                                            amount -= ado.AMOUNT ?? 0;

                                            ado.IMP_PRICE = materials.First().IMP_PRICE * (1 + materials.First().IMP_VAT_RATIO);

                                            var supMedi = materials.Where(o => result.Exists(e => e.ID == o.MATERIAL_TYPE_ID) && !result.Select(s => s.SUPPLIER_ID).Contains(o.SUPPLIER_ID)).ToList();
                                            var supplier = new HIS_SUPPLIER();
                                            if (supMedi != null && supMedi.Count > 0)
                                                supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == supMedi.First().SUPPLIER_ID);
                                            else
                                                supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == materials.First().SUPPLIER_ID);

                                            if (supplier != null)
                                            {
                                                ado.SUPPLIER_ID = supplier.ID;
                                                ado.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                                                ado.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                                            }

                                            result.Add(ado);
                                        }
                                    }

                                    if (amount > 0)
                                    {
                                        ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO();
                                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(ado, materialType);

                                        ado.IdRow = setIdRow(result);
                                        ado.MEDICINE_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                                        ado.MEDICINE_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                                        ado.Type = Base.GlobalConfig.VATTU;

                                        ado.AMOUNT_EXPORT = amount;
                                        var materials = ListMaterial.Where(o => o.MATERIAL_TYPE_ID == materialType.ID && !o.BID_ID.HasValue).ToList();
                                        if (materials != null && materials.Count > 0)
                                        {
                                            var materialInstocks = ListMaterialInStockSDO.Where(o => o.MATERIAL_TYPE_ID == materialType.ID && materials.Select(s => s.ID).Contains(o.ID)).ToList();
                                            if (materialInstocks != null && materialInstocks.Count > 0)
                                            {
                                                ado.AMOUNT_MEDI_STOCK = materialInstocks.Sum(o => o.TotalAmount);
                                            }

                                            ado.IMP_PRICE = materials.First().IMP_PRICE * (1 + materials.First().IMP_VAT_RATIO);

                                            var supMedi = materials.Where(o => result.Exists(e => e.ID == o.MATERIAL_TYPE_ID) && !result.Select(s => s.SUPPLIER_ID).Contains(o.SUPPLIER_ID)).ToList();
                                            var supplier = new HIS_SUPPLIER();
                                            if (supMedi != null && supMedi.Count > 0)
                                                supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == supMedi.First().SUPPLIER_ID);
                                            else
                                                supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == materials.First().SUPPLIER_ID);

                                            if (supplier != null)
                                            {
                                                ado.SUPPLIER_ID = supplier.ID;
                                                ado.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                                                ado.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                                            }
                                        }
                                        ado.AMOUNT = (ado.AMOUNT_EXPORT ?? 0) - (ado.AMOUNT_MEDI_STOCK ?? 0);

                                        result.Add(ado);
                                    }
                                }
                            }
                        }
                    }

                    if (result != null && result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            if (item.AMOUNT <= 0)
                            {
                                item.AMOUNT = 0;
                                item.IsNotSave = true;
                            }
                        }
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


        internal List<ADO.MedicineTypeADO> GetListMedicineTypeAdo()
        {
            List<ADO.MedicineTypeADO> result = null;
            try
            {
                //Thread nhập xuất
                CreateThreadGetData();

                List<long> ListMaterialTypeId = new List<long>();
                if (ListExpMestMaterial != null && ListExpMestMaterial.Count > 0)
                {
                    ListMaterialTypeId = ListExpMestMaterial.GroupBy(o => o.TDL_MATERIAL_TYPE_ID).Select(s => s.Key ?? 0).ToList();
                }

                List<long> ListMedicineTypeId = new List<long>();
                if (ListExpMestMedicine != null && ListExpMestMedicine.Count > 0)
                {
                    ListMedicineTypeId = ListExpMestMedicine.GroupBy(o => o.TDL_MEDICINE_TYPE_ID).Select(s => s.Key ?? 0).ToList();
                }

                if ((ListMedicineTypeId != null && ListMedicineTypeId.Count > 0) || (ListMaterialTypeId != null && ListMaterialTypeId.Count > 0))
                {
                    result = new List<ADO.MedicineTypeADO>();

                    if (ListMedicineTypeId != null && ListMedicineTypeId.Count > 0)
                    {
                        ListMedicineTypeId = ListMedicineTypeId.Distinct().ToList();
                        foreach (var medicineTypeId in ListMedicineTypeId)
                        {
                            decimal? AMOUNT_EXPORT = 0;
                            decimal? AMOUNT_MEDI_STOCK = 0, AllowAmount = 0;
                            decimal amountEstimated = 0;

                            var expMestMedicine = ListExpMestMedicine.Where(o => o.TDL_MEDICINE_TYPE_ID == medicineTypeId && o.MEDICINE_ID.HasValue).ToList();

                            var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeId);
                            if (medicineType != null)
                            {
                                var bidMedicineTypes = ListBidMedicineType.Where(o => o.MEDICINE_TYPE_ID == medicineType.ID).ToList();
                                var medicines = ListMedicine.Where(o => o.MEDICINE_TYPE_ID == medicineType.ID).ToList();
                                if (medicines != null && medicines.Count > 0)
                                {
                                    var meExpMestMedicine = expMestMedicine.Where(o => medicines.Select(s => s.ID).Contains(o.MEDICINE_ID ?? 0)).ToList();

                                    if (meExpMestMedicine != null && meExpMestMedicine.Count > 0)
                                    {
                                        //số lượng xuất
                                        AMOUNT_EXPORT = meExpMestMedicine.Sum(s => s.AMOUNT);
                                    }
                                    else
                                        continue;

                                    var medicineInstocks = ListMedicineInStockSDO.Where(o => o.MEDICINE_TYPE_ID == medicineType.ID && medicines.Select(s => s.ID).Contains(o.ID)).ToList();
                                    if (medicineInstocks != null && medicineInstocks.Count > 0)
                                    {
                                        //số lượng tồn kho
                                        AMOUNT_MEDI_STOCK = medicineInstocks.Sum(o => o.TotalAmount);
                                    }

                                    amountEstimated = (AMOUNT_EXPORT ?? 0) * this.SpinCoefficient / (decimal)100 - (AMOUNT_MEDI_STOCK ?? 0);

                                    if (bidMedicineTypes != null && bidMedicineTypes.Count > 0)
                                    {
                                        AllowAmount = bidMedicineTypes.Sum(o => o.AMOUNT) - bidMedicineTypes.Sum(o => o.IN_AMOUNT) + bidMedicineTypes.Sum(o => o.ADJUST_AMOUNT ?? 0) + bidMedicineTypes.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO ?? 0);
                                    }

                                    if (amountEstimated <= 0)
                                    {
                                        continue;
                                    }

                                }

                                if (UCAnticipateCreate.isBusiness == true && (bidMedicineTypes == null || bidMedicineTypes.Count() == 0))
                                {
                                    ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(ado, medicineType);
                                    ado.IdRow = setIdRow(result);
                                    ado.Type = Base.GlobalConfig.THUOC;
                                    ado.AMOUNT = amountEstimated;
                                    ado.IMP_PRICE = medicines.First().IMP_PRICE * (1 + medicines.First().IMP_VAT_RATIO);

                                    ado.isAmount = amountEstimated > AllowAmount;
                                    ado.AMOUNT_EXPORT = AMOUNT_EXPORT;
                                    ado.AMOUNT_MEDI_STOCK = AMOUNT_MEDI_STOCK;
                                    if (ado.AMOUNT > 0)
                                    {
                                        result.Add(ado);
                                    }
                                }
                                else if (bidMedicineTypes != null && bidMedicineTypes.Count > 0)
                                {
                                    if (UCAnticipateCreate.isBusiness == true && medicineType.IS_BUSINESS != 1)
                                        continue;

                                    var bidMedicineTypesGroup = bidMedicineTypes.GroupBy(o => new { o.BID_ID, o.SUPPLIER_ID });
                                    foreach (var itemGroup in bidMedicineTypesGroup)
                                    {
                                        foreach (var item in itemGroup)
                                        {

                                            ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO();
                                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(ado, medicineType);
                                            ado.IdRow = setIdRow(result);
                                            ado.Type = Base.GlobalConfig.THUOC;
                                            ado.AMOUNT = 0;

                                            if (amountEstimated > itemGroup.Sum(o => o.IN_AMOUNT))
                                            {
                                                ado.AMOUNT = itemGroup.Sum(o => o.IN_AMOUNT);
                                            }
                                            else
                                            {
                                                ado.AMOUNT = amountEstimated;
                                            }

                                            var bid = this.ListBid.FirstOrDefault(o => o.ID == item.BID_ID);
                                            if (bid != null)
                                            {
                                                ado.BID_ID = bid.ID;
                                                ado.BID_NUMBER = bid.BID_NUMBER;
                                                ado.BID_YEAR = bid.BID_YEAR;
                                            }

                                            ado.IMP_PRICE = medicines.First().IMP_PRICE * (1 + medicines.First().IMP_VAT_RATIO);

                                            var supMedi = medicines.Where(o => result.Exists(e => e.ID == o.MEDICINE_TYPE_ID) && !result.Select(s => s.SUPPLIER_ID).Contains(o.SUPPLIER_ID)).ToList();
                                            var supplier = new HIS_SUPPLIER();
                                            if (supMedi != null && supMedi.Count > 0)
                                                supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == supMedi.First().SUPPLIER_ID);
                                            else
                                                supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == medicines.First().SUPPLIER_ID);

                                            if (supplier != null)
                                            {
                                                ado.SUPPLIER_ID = supplier.ID;
                                                ado.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                                                ado.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                                            }

                                            ado.isAmount = amountEstimated > AllowAmount;
                                            ado.AMOUNT_EXPORT = AMOUNT_EXPORT;
                                            ado.AMOUNT_MEDI_STOCK = AMOUNT_MEDI_STOCK;
                                            if (ado.AMOUNT > 0)
                                            {
                                                result.Add(ado);
                                            }
                                            if (UCAnticipateCreate.isBusiness == false)
                                            {
                                                amountEstimated = amountEstimated - ado.AMOUNT ?? 0;
                                                if (amountEstimated <= 0)
                                                {
                                                    continue;
                                                }
                                            }

                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                if (ListMaterialTypeId != null && ListMaterialTypeId.Count > 0)
                {
                    ListMaterialTypeId = ListMaterialTypeId.Distinct().ToList();
                    foreach (var materialTypeId in ListMaterialTypeId)
                    {
                        decimal? AMOUNT_EXPORT = 0;
                        decimal? AMOUNT_MEDI_STOCK = 0, AllowAmount = 0;
                        decimal amountEstimated = 0;

                        var expMestMaterial = ListExpMestMaterial.Where(o => o.TDL_MATERIAL_TYPE_ID == materialTypeId && o.MATERIAL_ID.HasValue).ToList();

                        var materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == materialTypeId);
                        if (materialType != null)
                        {
                            var bidMaterialTypes = ListBidMaterialType.Where(o => o.MATERIAL_TYPE_ID == materialType.ID).ToList();
                            var materials = ListMaterial.Where(o => o.MATERIAL_TYPE_ID == materialType.ID).ToList();
                            if (materials != null && materials.Count > 0)
                            {
                                var meExpMestMaterial = expMestMaterial.Where(o => materials.Select(s => s.ID).Contains(o.MATERIAL_ID ?? 0)).ToList();

                                if (meExpMestMaterial != null && meExpMestMaterial.Count > 0)
                                {
                                    //số lượng xuất
                                    AMOUNT_EXPORT = meExpMestMaterial.Sum(s => s.AMOUNT);
                                }
                                else
                                    continue;

                                var materialInstocks = ListMaterialInStockSDO.Where(o => o.MATERIAL_TYPE_ID == materialType.ID && materials.Select(s => s.ID).Contains(o.ID)).ToList();
                                if (materialInstocks != null && materialInstocks.Count > 0)
                                {
                                    //số lượng tồn kho
                                    AMOUNT_MEDI_STOCK = materialInstocks.Sum(o => o.TotalAmount);
                                }

                                amountEstimated = (AMOUNT_EXPORT ?? 0) * this.SpinCoefficient / (decimal)100 - (AMOUNT_MEDI_STOCK ?? 0);

                                if (bidMaterialTypes != null && bidMaterialTypes.Count > 0)
                                {
                                    AllowAmount = bidMaterialTypes.Sum(o => o.AMOUNT) - bidMaterialTypes.Sum(o => o.IN_AMOUNT) + bidMaterialTypes.Sum(o => o.ADJUST_AMOUNT ?? 0) + bidMaterialTypes.Sum(o => o.AMOUNT * o.IMP_MORE_RATIO ?? 0);
                                }

                                if (amountEstimated <= 0 || amountEstimated > AllowAmount)
                                {
                                    continue;
                                }
                            }

                            if (UCAnticipateCreate.isBusiness == true && (bidMaterialTypes == null || bidMaterialTypes.Count() == 0))
                            {
                                ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(ado, materialType);
                                ado.IdRow = setIdRow(result);
                                ado.Type = Base.GlobalConfig.THUOC;
                                ado.AMOUNT = amountEstimated;
                                ado.IMP_PRICE = materials.First().IMP_PRICE * (1 + materials.First().IMP_VAT_RATIO);

                                ado.isAmount = amountEstimated > AllowAmount;
                                ado.AMOUNT_EXPORT = AMOUNT_EXPORT;
                                ado.AMOUNT_MEDI_STOCK = AMOUNT_MEDI_STOCK;
                                if (ado.AMOUNT > 0)
                                {
                                    result.Add(ado);
                                }
                            }
                            else if (bidMaterialTypes != null && bidMaterialTypes.Count > 0)
                            {
                                if (UCAnticipateCreate.isBusiness == true && materialType.IS_BUSINESS != 1)
                                    continue;
                                var bidMaterialTypesGroup = bidMaterialTypes.GroupBy(o => new { o.BID_ID, o.SUPPLIER_ID });
                                foreach (var itemGroup in bidMaterialTypesGroup)
                                {
                                    foreach (var item in itemGroup)
                                    {

                                        ADO.MedicineTypeADO ado = new ADO.MedicineTypeADO();
                                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MedicineTypeADO>(ado, materialType);
                                        ado.IdRow = setIdRow(result);
                                        ado.Type = Base.GlobalConfig.THUOC;
                                        ado.AMOUNT = 0;

                                        if (amountEstimated > itemGroup.Sum(o => o.IN_AMOUNT))
                                        {
                                            ado.AMOUNT = itemGroup.Sum(o => o.IN_AMOUNT);
                                        }
                                        else
                                        {
                                            ado.AMOUNT = amountEstimated;
                                        }

                                        var bid = this.ListBid.FirstOrDefault(o => o.ID == item.BID_ID);
                                        if (bid != null)
                                        {
                                            ado.BID_ID = bid.ID;
                                            ado.BID_NUMBER = bid.BID_NUMBER;
                                            ado.BID_YEAR = bid.BID_YEAR;
                                        }

                                        ado.IMP_PRICE = materials.First().IMP_PRICE * (1 + materials.First().IMP_VAT_RATIO);

                                        var supMedi = materials.Where(o => result.Exists(e => e.ID == o.MATERIAL_TYPE_ID) && !result.Select(s => s.SUPPLIER_ID).Contains(o.SUPPLIER_ID)).ToList();
                                        var supplier = new HIS_SUPPLIER();
                                        if (supMedi != null && supMedi.Count > 0)
                                            supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == supMedi.First().SUPPLIER_ID);
                                        else
                                            supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == materials.First().SUPPLIER_ID);

                                        if (supplier != null)
                                        {
                                            ado.SUPPLIER_ID = supplier.ID;
                                            ado.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                                            ado.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                                        }
                                        ado.AMOUNT_EXPORT = AMOUNT_EXPORT;
                                        ado.AMOUNT_MEDI_STOCK = AMOUNT_MEDI_STOCK;
                                        if (ado.AMOUNT > 0)
                                        {
                                            result.Add(ado);
                                        }
                                        if (UCAnticipateCreate.isBusiness == false)
                                        {
                                            amountEstimated = amountEstimated - ado.AMOUNT ?? 0;
                                            if (amountEstimated <= 0)
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }

                            }
                        }
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


        private double setIdRow(List<ADO.MedicineTypeADO> medicineTypes)
        {
            double currentIdRow = 0;
            try
            {
                if (medicineTypes != null && medicineTypes.Count > 0)
                {
                    var maxIdRow = medicineTypes.Max(o => o.IdRow);
                    currentIdRow = ++maxIdRow;
                }
                else
                {
                    currentIdRow = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentIdRow;
        }

        private void CreateThreadGetData()
        {
            Thread medicineInStock = new Thread(GetMedicineInStock);
            Thread materialInStock = new Thread(GetMaterialInStock);
            Thread expMestMaterial = new Thread(GetExpMestMaterial);
            Thread expMestMedicine = new Thread(GetExpMestMedicine);
            try
            {
                medicineInStock.Start();
                materialInStock.Start();
                expMestMaterial.Start();
                expMestMedicine.Start();

                medicineInStock.Join();
                materialInStock.Join();
                expMestMaterial.Join();
                expMestMedicine.Join();
            }
            catch (Exception ex)
            {
                medicineInStock.Abort();
                materialInStock.Abort();
                expMestMaterial.Abort();
                expMestMedicine.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetExpMestMedicine()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_TIME_FROM = TimeFrom;
                expMestMedicineFilter.EXP_TIME_TO = TimeTo;
                expMestMedicineFilter.TDL_MEDI_STOCK_IDs = MediStockIds;
                expMestMedicineFilter.IS_EXPORT = true;
                ListExpMestMedicine = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestMedicineFilter, param);
                if (ListExpMestMedicine != null && ListExpMestMedicine.Count > 0)
                {
                    ListExpMestMedicine = ListExpMestMedicine.Where(o => !ExpMestTypeNotIn.Contains(o.EXP_MEST_TYPE_ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetExpMestMaterial()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                expMestMaterialFilter.EXP_TIME_FROM = TimeFrom;
                expMestMaterialFilter.EXP_TIME_TO = TimeTo;
                expMestMaterialFilter.TDL_MEDI_STOCK_IDs = MediStockIds;
                expMestMaterialFilter.IS_EXPORT = true;
                ListExpMestMaterial = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialFilter, param);
                if (ListExpMestMaterial != null && ListExpMestMaterial.Count > 0)
                {
                    ListExpMestMaterial = ListExpMestMaterial.Where(o => !ExpMestTypeNotIn.Contains(o.EXP_MEST_TYPE_ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetMedicineInStock()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineStockViewFilter mediFilter = new HisMedicineStockViewFilter();
                mediFilter.MEDI_STOCK_IDs = MediStockIds;
                mediFilter.INCLUDE_EMPTY = false;
                ListMedicineInStockSDO = new BackendAdapter(param).Get<List<HisMedicineInStockSDO>>("/api/HisMedicine/GetInStockMedicineWithTypeTree", ApiConsumer.ApiConsumers.MosConsumer, mediFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetMaterialInStock()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMaterialStockViewFilter mateFilter = new MOS.Filter.HisMaterialStockViewFilter();
                mateFilter.MEDI_STOCK_IDs = MediStockIds;
                mateFilter.INCLUDE_EMPTY = false;
                ListMaterialInStockSDO = new BackendAdapter(param).Get<List<HisMaterialInStockSDO>>("api/HisMaterial/GetInStockMaterialWithTypeTree", ApiConsumer.ApiConsumers.MosConsumer, mateFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
