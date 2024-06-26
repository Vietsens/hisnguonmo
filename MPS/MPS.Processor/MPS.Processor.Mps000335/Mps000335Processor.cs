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
using MPS.Processor.Mps000335.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000335
{
    public class Mps000335Processor : AbstractProcessor
    {
        List<MedicineTypeViewADO> MedicineTypeViews = new List<MedicineTypeViewADO>();
        //gán ID vào PARENT_ID để list con có PARENT_ID null vẫn hiển thị.
        List<MedicineTypeViewADO> MedicineTypeParents = new List<MedicineTypeViewADO>();
        Mps000335PDO rdo;
        public Mps000335Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000335PDO)rdoBase;
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
                ProcessMedicineSDO();
                objectTag.AddObjectData(store, "ListPrice", MedicineTypeViews);
                objectTag.AddObjectData(store, "ListParent", MedicineTypeParents);
                objectTag.AddRelationship(store, "ListParent", "ListPrice", "PARENT_ID", "PARENT_ID");
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void MedicineTypeProcess(ref List<MedicineTypeViewADO> result, List<HisMedicineTypeView1SDO> inputData, bool isLeaf)
        {
            try
            {
                List<HIS_SALE_PROFIT_CFG> saleProfitMedicines = new List<HIS_SALE_PROFIT_CFG>();
                if (rdo.saleProfitCfgs != null && rdo.saleProfitCfgs.Count() > 0)
                {
                    saleProfitMedicines = rdo.saleProfitCfgs.Where(o => o.IS_MEDICINE == 1).ToList();
                }
                var parentList = !isLeaf ? result.Where(o => o.PARENT_ID != 1).ToList() : null;
                foreach (var item in inputData)
                {
                    MedicineTypeViewADO medicineType = new MedicineTypeViewADO();
                    AutoMapper.Mapper.CreateMap<MOS.SDO.HisMedicineTypeView1SDO, MedicineTypeViewADO>();
                    medicineType = AutoMapper.Mapper.Map<MedicineTypeViewADO>(item);

                    if (item.MEDICINE_TYPE_CODE == "NTACE006")
                    {
                        Inventec.Common.Logging.LogSystem.Debug("");
                    }
                    //var parent = parentList != null ? parentList.Where(o => item.PARENT_ID.HasValue && o.ID == item.PARENT_ID).ToList() : null;
                    if (!isLeaf)
                    {
                        List<V_HIS_MEDICINE_TYPE> parent = rdo.TotalMedicineType != null ? rdo.TotalMedicineType.Where(o => o.ID == item.PARENT_ID).ToList() : null;
                        medicineType.MEDICINE_PARENT_CODE = parent != null && parent.Count() > 0 ? parent[0].MEDICINE_TYPE_CODE : "";
                        medicineType.MEDICINE_PARENT_NAME = parent != null && parent.Count() > 0 ? parent[0].MEDICINE_TYPE_NAME : "";
                    }


                    Inventec.Common.Logging.LogSystem.Debug("ProcessMedicineSDO 1");
                    var medicines = rdo.Medicines.Where(o => o.MEDICINE_TYPE_ID == item.ID).OrderByDescending(p => p.CREATE_TIME).ToList();
                    medicineType.MANUFACTURER_NAME = item.MANUFACTURER_ID.HasValue ? rdo.ListManufacturer.FirstOrDefault(o => o.ID == item.MANUFACTURER_ID.Value).MANUFACTURER_NAME : "";
                    Inventec.Common.Logging.LogSystem.Debug("ProcessMedicineSDO 2");

                    if (medicines != null && medicines.Count == 1)
                    {
                        medicineType.IMP_PRICE_1 = medicines[0].IMP_PRICE;
                        medicineType.IMP_VAT_RATIO_1 = medicines[0].IMP_VAT_RATIO;
                        medicineType.SUPPLIER_NAME_1 = medicines[0].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == medicines[0].SUPPLIER_ID).SUPPLIER_NAME : "";
                        SetLoiNhuan(saleProfitMedicines, ref medicineType, medicineType.IMP_PRICE_1, medicineType.IMP_VAT_RATIO_1);
                    }
                    else if (medicines != null && medicines.Count == 2)
                    {
                        medicineType.IMP_PRICE_1 = medicines[0].IMP_PRICE;
                        medicineType.IMP_PRICE_2 = medicines[1].IMP_PRICE;
                        medicineType.IMP_VAT_RATIO_1 = medicines[0].IMP_VAT_RATIO;
                        medicineType.IMP_VAT_RATIO_2 = medicines[1].IMP_VAT_RATIO;
                        medicineType.SUPPLIER_NAME_1 = medicines[0].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == medicines[0].SUPPLIER_ID).SUPPLIER_NAME : "";
                        medicineType.SUPPLIER_NAME_2 = medicines[1].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == medicines[1].SUPPLIER_ID).SUPPLIER_NAME : "";
                        SetLoiNhuan(saleProfitMedicines, ref medicineType, medicineType.IMP_PRICE_2, medicineType.IMP_VAT_RATIO_2);
                    }
                    else if (medicines != null && medicines.Count == 3)
                    {
                        medicineType.IMP_PRICE_1 = medicines[0].IMP_PRICE;
                        medicineType.IMP_PRICE_2 = medicines[1].IMP_PRICE;
                        medicineType.IMP_PRICE_3 = medicines[2].IMP_PRICE;
                        medicineType.IMP_VAT_RATIO_1 = medicines[0].IMP_VAT_RATIO;
                        medicineType.IMP_VAT_RATIO_2 = medicines[1].IMP_VAT_RATIO;
                        medicineType.IMP_VAT_RATIO_3 = medicines[2].IMP_VAT_RATIO;
                        medicineType.SUPPLIER_NAME_1 = medicines[0].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == medicines[0].SUPPLIER_ID).SUPPLIER_NAME : "";
                        medicineType.SUPPLIER_NAME_2 = medicines[1].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == medicines[1].SUPPLIER_ID).SUPPLIER_NAME : "";
                        medicineType.SUPPLIER_NAME_3 = medicines[2].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == medicines[2].SUPPLIER_ID).SUPPLIER_NAME : "";
                        SetLoiNhuan(saleProfitMedicines, ref medicineType, medicineType.IMP_PRICE_3, medicineType.IMP_VAT_RATIO_3);
                    }
                    else if (medicines != null && medicines.Count >= 4)
                    {
                        medicineType.IMP_PRICE_1 = medicines[0].IMP_PRICE;
                        medicineType.IMP_PRICE_2 = medicines[1].IMP_PRICE;
                        medicineType.IMP_PRICE_3 = medicines[2].IMP_PRICE;
                        medicineType.IMP_PRICE_4 = medicines[3].IMP_PRICE;
                        medicineType.IMP_VAT_RATIO_1 = medicines[0].IMP_VAT_RATIO;
                        medicineType.IMP_VAT_RATIO_2 = medicines[1].IMP_VAT_RATIO;
                        medicineType.IMP_VAT_RATIO_3 = medicines[2].IMP_VAT_RATIO;
                        medicineType.IMP_VAT_RATIO_4 = medicines[3].IMP_VAT_RATIO;
                        medicineType.SUPPLIER_NAME_1 = medicines[0].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == medicines[0].SUPPLIER_ID).SUPPLIER_NAME : "";
                        medicineType.SUPPLIER_NAME_2 = medicines[1].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == medicines[1].SUPPLIER_ID).SUPPLIER_NAME : "";
                        medicineType.SUPPLIER_NAME_3 = medicines[2].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == medicines[2].SUPPLIER_ID).SUPPLIER_NAME : "";
                        medicineType.SUPPLIER_NAME_4 = medicines[3].SUPPLIER_ID.HasValue ? rdo.ListSupplier.FirstOrDefault(o => o.ID == medicines[3].SUPPLIER_ID).SUPPLIER_NAME : "";
                        SetLoiNhuan(saleProfitMedicines, ref medicineType, medicineType.IMP_PRICE_4, medicineType.IMP_VAT_RATIO_4);
                    }

                    if (item.IS_SALE_EQUAL_IMP_PRICE.HasValue && item.IS_SALE_EQUAL_IMP_PRICE == 1)
                    {
                        if (medicines != null && medicines.Count == 1)
                        {
                            medicineType.EXP_PRICE_1 = medicines[0].IMP_PRICE;
                            medicineType.EXP_VAT_RATIO_1 = medicines[0].IMP_VAT_RATIO;

                            Inventec.Common.Logging.LogSystem.Debug("ProcessMedicineSDO 7");
                        }
                        else if (medicines != null && medicines.Count == 2)
                        {
                            medicineType.EXP_PRICE_1 = medicines[0].IMP_PRICE;
                            medicineType.EXP_PRICE_2 = medicines[1].IMP_PRICE;
                            medicineType.EXP_VAT_RATIO_1 = medicines[0].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_2 = medicines[1].IMP_VAT_RATIO;
                            Inventec.Common.Logging.LogSystem.Debug("ProcessMedicineSDO 8");
                        }
                        else if (medicines != null && medicines.Count == 3)
                        {
                            medicineType.EXP_PRICE_1 = medicines[0].IMP_PRICE;
                            medicineType.EXP_PRICE_2 = medicines[1].IMP_PRICE;
                            medicineType.EXP_PRICE_3 = medicines[2].IMP_PRICE;
                            medicineType.EXP_VAT_RATIO_1 = medicines[0].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_2 = medicines[1].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_3 = medicines[2].IMP_VAT_RATIO;
                            Inventec.Common.Logging.LogSystem.Debug("ProcessMedicineSDO 9");
                        }
                        else if (medicines != null && medicines.Count >= 4)
                        {
                            medicineType.EXP_PRICE_1 = medicines[0].IMP_PRICE;
                            medicineType.EXP_PRICE_2 = medicines[1].IMP_PRICE;
                            medicineType.EXP_PRICE_3 = medicines[2].IMP_PRICE;
                            medicineType.EXP_PRICE_4 = medicines[3].IMP_PRICE;
                            medicineType.EXP_VAT_RATIO_1 = medicines[0].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_2 = medicines[1].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_3 = medicines[2].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_4 = medicines[3].IMP_VAT_RATIO;
                            Inventec.Common.Logging.LogSystem.Debug("ProcessMedicineSDO 10");
                        }
                    }
                    else
                    {
                        var medicinePaties = rdo.MedicinePaties.Where(o => o.MEDICINE_TYPE_ID == item.ID).OrderByDescending(p => p.CREATE_TIME).ToList();
                        Inventec.Common.Logging.LogSystem.Debug("ProcessMedicineSDO 11");
                        if (medicinePaties != null && medicinePaties.Count == 1)
                        {
                            medicineType.EXP_PRICE_1 = medicinePaties[0].IMP_PRICE;
                            medicineType.EXP_VAT_RATIO_1 = medicinePaties[0].IMP_VAT_RATIO;
                            Inventec.Common.Logging.LogSystem.Debug("ProcessMedicineSDO 12");
                        }
                        else if (medicinePaties != null && medicinePaties.Count == 2)
                        {
                            medicineType.EXP_PRICE_1 = medicinePaties[0].IMP_PRICE;
                            medicineType.EXP_PRICE_2 = medicinePaties[1].IMP_PRICE;
                            medicineType.EXP_VAT_RATIO_1 = medicinePaties[0].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_2 = medicinePaties[1].IMP_VAT_RATIO;
                            Inventec.Common.Logging.LogSystem.Debug("ProcessMedicineSDO 13");
                        }
                        else if (medicinePaties != null && medicinePaties.Count == 3)
                        {
                            medicineType.EXP_PRICE_1 = medicinePaties[0].IMP_PRICE;
                            medicineType.EXP_PRICE_2 = medicinePaties[1].IMP_PRICE;
                            medicineType.EXP_PRICE_3 = medicinePaties[2].IMP_PRICE;
                            medicineType.EXP_VAT_RATIO_1 = medicinePaties[0].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_2 = medicinePaties[1].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_3 = medicinePaties[2].IMP_VAT_RATIO;
                            Inventec.Common.Logging.LogSystem.Debug("ProcessMedicineSDO 14");
                        }
                        else if (medicinePaties != null && medicinePaties.Count >= 4)
                        {
                            medicineType.EXP_PRICE_1 = medicinePaties[0].IMP_PRICE;
                            medicineType.EXP_PRICE_2 = medicinePaties[1].IMP_PRICE;
                            medicineType.EXP_PRICE_3 = medicinePaties[2].IMP_PRICE;
                            medicineType.EXP_PRICE_4 = medicinePaties[3].IMP_PRICE;
                            medicineType.EXP_VAT_RATIO_1 = medicinePaties[0].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_2 = medicinePaties[1].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_3 = medicinePaties[2].IMP_VAT_RATIO;
                            medicineType.EXP_VAT_RATIO_4 = medicinePaties[3].IMP_VAT_RATIO;
                            Inventec.Common.Logging.LogSystem.Debug("ProcessMedicineSDO 15");
                        }
                    }

                    result.Add(medicineType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetLoiNhuan(List<HIS_SALE_PROFIT_CFG> saleProfitMedicines, ref MedicineTypeViewADO medicineType, decimal? impPrice, decimal? impVAT)
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

        private void ProcessMedicineSDO()
        {
            try
            {
                if (rdo.medicineTypeIsLeafs != null && rdo.medicineTypeIsLeafs.Count() > 0)
                    MedicineTypeProcess(ref this.MedicineTypeViews, rdo.medicineTypeIsLeafs, true);
                if (rdo.MedicineTypeSdos != null && rdo.MedicineTypeSdos.Count() > 0)
                    MedicineTypeProcess(ref this.MedicineTypeViews, rdo.MedicineTypeSdos, false);

                this.MedicineTypeViews = this.MedicineTypeViews.OrderBy(o => o.MEDICINE_PARENT_NAME).ThenBy(o => o.MEDICINE_TYPE_NAME).ToList();

                var groupParent = this.MedicineTypeViews.GroupBy(o => o.PARENT_ID).ToList();
                foreach (var item in groupParent)
                {
                    MedicineTypeViewADO data = new MedicineTypeViewADO();

                    var mety = rdo.TotalMedicineType != null ? rdo.TotalMedicineType.FirstOrDefault(o => o.ID == item.Key) : null;
                    if (mety != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeViewADO>(data, mety);
                    }

                    data.PARENT_ID = item.Key;

                    MedicineTypeParents.Add(data);
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
                string title = "BẢNG GIÁ {0}";
                if (rdo.ParentMedicineType != null)
                {
                    title = String.Format(title, (rdo.ParentMedicineType.MEDICINE_TYPE_NAME ?? "").ToUpper());
                    singleValueDictionary.Add(Mps000335ExtendSingleKey.PARENT_CODE, rdo.ParentMedicineType.MEDICINE_TYPE_CODE);
                    singleValueDictionary.Add(Mps000335ExtendSingleKey.PARENT_NAME, rdo.ParentMedicineType.MEDICINE_TYPE_NAME);
                }
                else
                {
                    title = String.Format(title, "THUỐC");
                    singleValueDictionary.Add(Mps000335ExtendSingleKey.PARENT_CODE, "");
                    singleValueDictionary.Add(Mps000335ExtendSingleKey.PARENT_NAME, "");
                }

                singleValueDictionary.Add(Mps000335ExtendSingleKey.TITLE, title);

                if (rdo.MedicineTypeSdos != null && rdo.MedicineTypeSdos.Count > 0)
                {
                    rdo.MedicineTypeSdos = rdo.MedicineTypeSdos.Where(o => o.MEDICINE_ID.HasValue).OrderBy(s => s.MEDICINE_TYPE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
