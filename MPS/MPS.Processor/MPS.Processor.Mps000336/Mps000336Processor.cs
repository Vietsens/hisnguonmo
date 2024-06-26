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
using MOS.SDO;
using MPS.Processor.Mps000336.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000336
{
    public class Mps000336Processor : AbstractProcessor
    {
        Mps000336PDO rdo;
        List<MaterialTypeViewADO> MaterialTypeViews = new List<MaterialTypeViewADO>();
        //gán ID vào PARENT_ID để list con có PARENT_ID null vẫn hiển thị.
        List<MaterialTypeViewADO> MaterialTypeParents = new List<MaterialTypeViewADO>();
        public Mps000336Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000336PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                ProcessMaterialSDO();
                objectTag.AddObjectData(store, "ListPrice", MaterialTypeViews);
                objectTag.AddObjectData(store, "ListParent", MaterialTypeParents);
                objectTag.AddRelationship(store, "ListParent", "ListPrice", "PARENT_ID", "PARENT_ID");
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                string title = "BẢNG GIÁ {0}";
                if (rdo.ParentMaterialType != null)
                {
                    title = String.Format(title, (rdo.ParentMaterialType.MATERIAL_TYPE_NAME ?? "").ToUpper());
                    singleValueDictionary.Add(Mps000336ExtendSingleKey.PARENT_CODE, rdo.ParentMaterialType.MATERIAL_TYPE_CODE);
                    singleValueDictionary.Add(Mps000336ExtendSingleKey.PARENT_NAME, rdo.ParentMaterialType.MATERIAL_TYPE_NAME);
                }
                else
                {
                    title = String.Format(title, "VẬT TƯ Y TẾ");
                    singleValueDictionary.Add(Mps000336ExtendSingleKey.PARENT_CODE, "");
                    singleValueDictionary.Add(Mps000336ExtendSingleKey.PARENT_NAME, "");
                }

                singleValueDictionary.Add(Mps000336ExtendSingleKey.TITLE, title);

                if (rdo.MaterialTypeSdos != null && rdo.MaterialTypeSdos.Count > 0)
                {
                    rdo.MaterialTypeSdos = rdo.MaterialTypeSdos.Where(o => o.MATERIAL_ID.HasValue).OrderBy(s => s.MATERIAL_TYPE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void MedicineTypeProcess(ref List<MaterialTypeViewADO> result, List<HisMaterialTypeView1SDO> inputData, bool isLeaf)
        {
            try
            {
                List<HIS_SALE_PROFIT_CFG> saleProfitMedicines = new List<HIS_SALE_PROFIT_CFG>();
                if (rdo.saleProfitCfgs != null && rdo.saleProfitCfgs.Count() > 0)
                {
                    saleProfitMedicines = rdo.saleProfitCfgs.Where(o => o.IS_MATERIAL == 1).ToList();
                }
                foreach (var item in inputData)
                {
                    MaterialTypeViewADO materialType = new MaterialTypeViewADO();
                    AutoMapper.Mapper.CreateMap<MOS.SDO.HisMaterialTypeView1SDO, MaterialTypeViewADO>();
                    materialType = AutoMapper.Mapper.Map<MaterialTypeViewADO>(item);
                    var materials = rdo.Materials.Where(o => o.MATERIAL_TYPE_ID == item.ID).OrderByDescending(p => p.CREATE_TIME).ToList();
                    materialType.MANUFACTURER_NAME = item.MANUFACTURER_ID.HasValue ? rdo.ListManufacturer.FirstOrDefault(o => o.ID == item.MANUFACTURER_ID.Value).MANUFACTURER_NAME : "";

                    if (!isLeaf)
                    {
                        List<V_HIS_MATERIAL_TYPE> parent = rdo.TotalMaterialType != null ? rdo.TotalMaterialType.Where(o => o.ID == item.PARENT_ID).ToList() : null;
                        materialType.MATERIAL_PARENT_CODE = parent != null && parent.Count() > 0 ? parent[0].MATERIAL_TYPE_CODE : "";
                        materialType.MATERIAL_PARENT_NAME = parent != null && parent.Count() > 0 ? parent[0].MATERIAL_TYPE_NAME : "";
                    }

                    // ti le loi nhuan(thuoc kinh doanh moi co ti le loi nhuan)
                    if (saleProfitMedicines != null && saleProfitMedicines.Count() > 0 && item.IS_BUSINESS == 1)
                    {
                        var checks = saleProfitMedicines.Where(o => o.IMP_PRICE_FROM <= item.IMP_PRICE && item.IMP_PRICE <= o.IMP_PRICE_TO).ToList();
                        if (checks != null && checks.Count() > 0)
                            materialType.TLLN = checks.FirstOrDefault().RATIO;
                    }

                    materialType.IMP_PRICE_AFTER_VAT = materialType.IMP_PRICE ?? 0 + (materialType.IMP_PRICE ?? 0) * (materialType.IMP_VAT_RATIO ?? 0);
                    materialType.LOINHUAN = (materialType.TLLN ?? 0) * (materialType.IMP_PRICE_AFTER_VAT ?? 0);
                    materialType.GIADUKIEN = materialType.IMP_PRICE_AFTER_VAT + materialType.LOINHUAN;

                    if (materials != null && materials.Count == 1)
                    {
                        materialType.IMP_PRICE_1 = materials[0].IMP_PRICE;
                        materialType.IMP_VAT_RATIO_1 = materials[0].IMP_VAT_RATIO;
                        materialType.SUPPLIER_NAME_1 = materials[0].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == materials[0].SUPPLIER_ID).SUPPLIER_NAME : "";
                        SetLoiNhuan(saleProfitMedicines, ref materialType, materialType.IMP_PRICE_1, materialType.IMP_VAT_RATIO_1);
                    }
                    else if (materials != null && materials.Count == 2)
                    {
                        materialType.IMP_PRICE_1 = materials[0].IMP_PRICE;
                        materialType.IMP_PRICE_2 = materials[1].IMP_PRICE;
                        materialType.IMP_VAT_RATIO_1 = materials[0].IMP_VAT_RATIO;
                        materialType.IMP_VAT_RATIO_2 = materials[1].IMP_VAT_RATIO;
                        materialType.SUPPLIER_NAME_1 = materials[0].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == materials[0].SUPPLIER_ID).SUPPLIER_NAME : "";
                        materialType.SUPPLIER_NAME_2 = materials[1].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == materials[1].SUPPLIER_ID).SUPPLIER_NAME : "";
                        SetLoiNhuan(saleProfitMedicines, ref materialType, materialType.IMP_PRICE_2, materialType.IMP_VAT_RATIO_2);
                    }
                    else if (materials != null && materials.Count == 3)
                    {
                        materialType.IMP_PRICE_1 = materials[0].IMP_PRICE;
                        materialType.IMP_PRICE_2 = materials[1].IMP_PRICE;
                        materialType.IMP_PRICE_3 = materials[2].IMP_PRICE;
                        materialType.IMP_VAT_RATIO_1 = materials[0].IMP_VAT_RATIO;
                        materialType.IMP_VAT_RATIO_2 = materials[1].IMP_VAT_RATIO;
                        materialType.IMP_VAT_RATIO_3 = materials[2].IMP_VAT_RATIO;
                        materialType.SUPPLIER_NAME_1 = materials[0].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == materials[0].SUPPLIER_ID).SUPPLIER_NAME : "";
                        materialType.SUPPLIER_NAME_2 = materials[1].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == materials[1].SUPPLIER_ID).SUPPLIER_NAME : "";
                        materialType.SUPPLIER_NAME_3 = materials[2].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == materials[2].SUPPLIER_ID).SUPPLIER_NAME : "";
                        SetLoiNhuan(saleProfitMedicines, ref materialType, materialType.IMP_PRICE_3, materialType.IMP_VAT_RATIO_3);
                    }
                    else if (materials != null && materials.Count >= 4)
                    {
                        materialType.IMP_PRICE_1 = materials[0].IMP_PRICE;
                        materialType.IMP_PRICE_2 = materials[1].IMP_PRICE;
                        materialType.IMP_PRICE_3 = materials[2].IMP_PRICE;
                        materialType.IMP_PRICE_4 = materials[3].IMP_PRICE;
                        materialType.IMP_VAT_RATIO_1 = materials[0].IMP_VAT_RATIO;
                        materialType.IMP_VAT_RATIO_2 = materials[1].IMP_VAT_RATIO;
                        materialType.IMP_VAT_RATIO_3 = materials[2].IMP_VAT_RATIO;
                        materialType.IMP_VAT_RATIO_4 = materials[3].IMP_VAT_RATIO;
                        materialType.SUPPLIER_NAME_1 = materials[0].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == materials[0].SUPPLIER_ID).SUPPLIER_NAME : "";
                        materialType.SUPPLIER_NAME_2 = materials[1].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == materials[1].SUPPLIER_ID).SUPPLIER_NAME : "";
                        materialType.SUPPLIER_NAME_3 = materials[2].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == materials[2].SUPPLIER_ID).SUPPLIER_NAME : "";
                        materialType.SUPPLIER_NAME_4 = materials[3].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == materials[3].SUPPLIER_ID).SUPPLIER_NAME : "";
                        SetLoiNhuan(saleProfitMedicines, ref materialType, materialType.IMP_PRICE_4, materialType.IMP_VAT_RATIO_4);
                    }

                    if (item.IS_SALE_EQUAL_IMP_PRICE.HasValue && item.IS_SALE_EQUAL_IMP_PRICE == 1)
                    {
                        if (materials != null && materials.Count == 1)
                        {
                            materialType.EXP_PRICE_1 = materials[0].IMP_PRICE;
                            materialType.EXP_VAT_RATIO_1 = materials[0].IMP_VAT_RATIO;
                        }
                        else if (materials != null && materials.Count == 2)
                        {
                            materialType.EXP_PRICE_1 = materials[0].IMP_PRICE;
                            materialType.EXP_PRICE_2 = materials[1].IMP_PRICE;
                            materialType.EXP_VAT_RATIO_1 = materials[0].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_2 = materials[1].IMP_VAT_RATIO;
                        }
                        else if (materials != null && materials.Count == 3)
                        {
                            materialType.EXP_PRICE_1 = materials[0].IMP_PRICE;
                            materialType.EXP_PRICE_2 = materials[1].IMP_PRICE;
                            materialType.EXP_PRICE_3 = materials[2].IMP_PRICE;
                            materialType.EXP_VAT_RATIO_1 = materials[0].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_2 = materials[1].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_3 = materials[2].IMP_VAT_RATIO;
                        }
                        else if (materials != null && materials.Count >= 4)
                        {
                            materialType.EXP_PRICE_1 = materials[0].IMP_PRICE;
                            materialType.EXP_PRICE_2 = materials[1].IMP_PRICE;
                            materialType.EXP_PRICE_3 = materials[2].IMP_PRICE;
                            materialType.EXP_PRICE_4 = materials[3].IMP_PRICE;
                            materialType.EXP_VAT_RATIO_1 = materials[0].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_2 = materials[1].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_3 = materials[2].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_4 = materials[3].IMP_VAT_RATIO;
                        }
                    }
                    else
                    {
                        var materialPaties = rdo.MaterialPaties.Where(o => o.MATERIAL_TYPE_ID == item.ID).OrderByDescending(p => p.CREATE_TIME).ToList();

                        if (materialPaties != null && materialPaties.Count == 1)
                        {
                            materialType.EXP_PRICE_1 = materialPaties[0].IMP_PRICE;
                            materialType.EXP_VAT_RATIO_1 = materialPaties[0].IMP_VAT_RATIO;
                        }
                        else if (materialPaties != null && materialPaties.Count == 2)
                        {
                            materialType.EXP_PRICE_1 = materialPaties[0].IMP_PRICE;
                            materialType.EXP_PRICE_2 = materialPaties[1].IMP_PRICE;
                            materialType.EXP_VAT_RATIO_1 = materialPaties[0].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_2 = materialPaties[1].IMP_VAT_RATIO;
                        }
                        else if (materialPaties != null && materialPaties.Count == 3)
                        {
                            materialType.EXP_PRICE_1 = materialPaties[0].IMP_PRICE;
                            materialType.EXP_PRICE_2 = materialPaties[1].IMP_PRICE;
                            materialType.EXP_PRICE_3 = materialPaties[2].IMP_PRICE;
                            materialType.EXP_VAT_RATIO_1 = materialPaties[0].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_2 = materialPaties[1].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_3 = materialPaties[2].IMP_VAT_RATIO;
                        }
                        else if (materialPaties != null && materialPaties.Count >= 4)
                        {
                            materialType.EXP_PRICE_1 = materialPaties[0].IMP_PRICE;
                            materialType.EXP_PRICE_2 = materialPaties[1].IMP_PRICE;
                            materialType.EXP_PRICE_3 = materialPaties[2].IMP_PRICE;
                            materialType.EXP_PRICE_4 = materialPaties[3].IMP_PRICE;
                            materialType.EXP_VAT_RATIO_1 = materialPaties[0].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_2 = materialPaties[1].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_3 = materialPaties[2].IMP_VAT_RATIO;
                            materialType.EXP_VAT_RATIO_4 = materialPaties[3].IMP_VAT_RATIO;
                        }
                    }
                    MaterialTypeViews.Add(materialType);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetLoiNhuan(List<HIS_SALE_PROFIT_CFG> saleProfitMedicines, ref MaterialTypeViewADO medicineType, decimal? impPrice, decimal? impVAT)
        {
            try
            {
                // ti le loi nhuan(thuoc kinh doanh moi co ti le loi nhuan)
                if (saleProfitMedicines != null && saleProfitMedicines.Count() > 0 && medicineType.IS_BUSINESS == 1)
                {
                    var checks = saleProfitMedicines.Where(o => (o.IMP_PRICE_FROM ?? 0) <= (impPrice ?? 0) && (impPrice ?? 0) <= (o.IMP_PRICE_TO ?? 0)).ToList();
                    if (checks != null && checks.Count() > 0)
                        medicineType.TLLN = checks.FirstOrDefault().RATIO;
                }
                medicineType.IMP_PRICE_AFTER_VAT = impPrice ?? 0 + (impPrice ?? 0) * (impVAT ?? 0);
                medicineType.LOINHUAN = (medicineType.TLLN ?? 0) * (medicineType.IMP_PRICE_AFTER_VAT ?? 0) / 100;
                medicineType.GIADUKIEN = medicineType.IMP_PRICE_AFTER_VAT + medicineType.LOINHUAN;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMaterialSDO()
        {
            try
            {
                if (rdo.medicineTypeIsLeafs != null && rdo.medicineTypeIsLeafs.Count() > 0)
                    MedicineTypeProcess(ref this.MaterialTypeViews, rdo.medicineTypeIsLeafs, true);
                if (rdo.MaterialTypeSdos != null && rdo.MaterialTypeSdos.Count() > 0)
                    MedicineTypeProcess(ref this.MaterialTypeViews, rdo.MaterialTypeSdos, false);

                this.MaterialTypeViews = this.MaterialTypeViews.OrderBy(o => o.MATERIAL_PARENT_NAME).ThenBy(o => o.MATERIAL_TYPE_NAME).ToList();

                var groupParent = this.MaterialTypeViews.GroupBy(o => o.PARENT_ID).ToList();
                foreach (var item in groupParent)
                {
                    MaterialTypeViewADO data = new MaterialTypeViewADO();

                    var mety = rdo.TotalMaterialType != null ? rdo.TotalMaterialType.FirstOrDefault(o => o.ID == item.Key) : null;
                    if (mety != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<MaterialTypeViewADO>(data, mety);
                    }

                    data.PARENT_ID = item.Key;

                    MaterialTypeParents.Add(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
