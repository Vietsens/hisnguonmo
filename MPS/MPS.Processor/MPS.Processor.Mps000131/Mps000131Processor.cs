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
using FlexCel.Report;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using MPS.Processor.Mps000131.ADO;
using MPS.Processor.Mps000131.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000131
{
    public class Mps000131Processor : AbstractProcessor
    {
        Mps000131PDO rdo;
        List<HisMediMateBloodInStock_Print> medicineInStockSdoPrints { get; set; }

        public Mps000131Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000131PDO)rdoBase;
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                if (store.ReadTemplate(System.IO.Path.GetFullPath(fileName)))
                {
                    FunctionExcuteData(); //xử lý dữ liệu
                    ProcessSingleKey();
                    singleTag.ProcessData(store, singleValueDictionary);
                    //barCodeTag.ProcessData(store, dicImage);
                    objectTag.AddObjectData(store, "ListSDO", medicineInStockSdoPrints);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<V_HIS_MEDI_STOCK>(rdo.mediStock, false);

                if (rdo.isCheckMedicine)
                {
                    SetSingleKey(new KeyValue(Mps000131ExtendSingleKey.IS_MEDICINE_TYPE, " Thuốc"));
                }
                else if (rdo.isCheckMaterial)
                {
                    SetSingleKey(new KeyValue(Mps000131ExtendSingleKey.IS_MEDICINE_TYPE, " Vật tư"));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000131ExtendSingleKey.IS_MEDICINE_TYPE, " Máu"));
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void FunctionExcuteData()
        {
            try
            {
                List<MOS.SDO.HisMedicineInStockSDO> medicineInStockSdos = new List<MOS.SDO.HisMedicineInStockSDO>();
                //List<MOS.SDO.HisMaterialInStockSDO> materialInStockSdos = new List<MOS.SDO.HisMaterialInStockSDO>();
                List<MOS.SDO.HisMedicineInStockSDO> instockMedicines = new List<MOS.SDO.HisMedicineInStockSDO>();
                List<MOS.SDO.HisMaterialInStockSDO> instockMaterials = new List<MOS.SDO.HisMaterialInStockSDO>();
                List<MOS.SDO.HisMedicineInStockSDO> listRootMedicines = new List<MOS.SDO.HisMedicineInStockSDO>();
                List<MOS.SDO.HisMaterialInStockSDO> listRootMaterials = new List<MOS.SDO.HisMaterialInStockSDO>();
                medicineInStockSdoPrints = new List<HisMediMateBloodInStock_Print>();

                if (rdo.isCheckMedicine)
                {
                    List<MOS.SDO.HisMedicineInStockSDO> instockMedicineSdos = new List<MOS.SDO.HisMedicineInStockSDO>();
                    instockMedicineSdos = rdo.lstMedicineInStockSDO;
                    var selectedMedicines = instockMedicineSdos.Where(o => !o.isTypeNode).ToList();
                    // var datatest = instockMedicineSdos.Where(o => o.MEDICINE_TYPE_NAME == "Thuốc truyền 1").ToList();
                    var selectedRootMedicines = instockMedicineSdos.Where(o => o.isTypeNode).ToList();

                    //var dataGoups = selectedMedicines.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();

                    Mapper.CreateMap<HisMedicineInStockSDO, HisMedicineInStockSDO>();
                    instockMedicines = Mapper.Map<List<HisMedicineInStockSDO>>(selectedMedicines);
                    listRootMedicines = Mapper.Map<List<HisMedicineInStockSDO>>(selectedRootMedicines);
                    var dataGoups = instockMedicines.GroupBy(p => p.ParentNodeId).ToList();

                    int dem = 1;

                    foreach (var gRoot in selectedRootMedicines)
                    {
                        HisMediMateBloodInStock_Print root = new HisMediMateBloodInStock_Print(gRoot, true);
                        var grByNode = dataGoups.FirstOrDefault(o => o.Key == gRoot.NodeId);
                        if (grByNode != null && grByNode.Count() > 0)
                        {

                            var childs = grByNode.ToList();
                            root.AvailableAmount = childs.Sum(s => (s.AvailableAmount ?? 0));
                            root.TotalAmount = childs.Sum(s => (s.TotalAmount ?? 0));
                            root.MEDICINE_TYPE_NAME = dem + "." + root.MEDICINE_TYPE_NAME.ToUpper();
                            var gByPrice = childs.GroupBy(g => new { g.REGISTER_NUMBER, g.PACKAGE_NUMBER, g.EXPIRED_DATE, g.MEDICINE_TYPE_NAME, g.MEDICINE_TYPE_CODE }).ToList();

                            medicineInStockSdoPrints.Add(root);
                            var checkMedicine = gByPrice.FirstOrDefault(o => o.FirstOrDefault().MEDICINE_TYPE_CODE == "PER011");
                            if (checkMedicine != null)
                            {
                                var a = 0;
                            }
                            foreach (var medicineInStock in gByPrice)
                            {
                                HisMediMateBloodInStock_Print ado = new HisMediMateBloodInStock_Print(medicineInStock.FirstOrDefault(), false);

                                ado.MEDICINE_TYPE_NAME = "     " + ado.MEDICINE_TYPE_NAME;
                                ado.AvailableAmount = medicineInStock.Sum(p => (p.AvailableAmount ?? 0));
                                ado.TotalAmount = medicineInStock.Sum(s => (s.TotalAmount ?? 0));
                                medicineInStockSdoPrints.Add(ado);
                            }
                            dem++;
                        }
                    }

                    //foreach (var item in medicineInStockSdos)
                    //{

                    //    HisMediMateBloodInStock_Print medicine = new HisMediMateBloodInStock_Print();
                    //    Inventec.Common.Mapper.DataObjectMapper.Map<HisMediMateBloodInStock_Print>(medicine, item);
                    //    medicine.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64
                    //    (item.EXPIRED_DATE ?? 0));
                    //    medicineInStockSdoPrints.Add(medicine);
                    //}
                }
                else if (rdo.isCheckMaterial)
                {
                    List<MOS.SDO.HisMaterialInStockSDO> materialInstocks = new List<MOS.SDO.HisMaterialInStockSDO>();
                    materialInstocks = rdo.lstMaterialInStockSDO;
                    //var selectedMaterials = materialInstocks.Where(o => !String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                    var selectedMaterials = materialInstocks.Where(o => !o.isTypeNode).ToList();
                    var selectedRootMedicines = materialInstocks.Where(o => o.isTypeNode).ToList();
                    var selectedRootMaterials = materialInstocks.Where(o => String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                    Mapper.CreateMap<HisMaterialInStockSDO, HisMaterialInStockSDO>();
                    instockMaterials = Mapper.Map<List<HisMaterialInStockSDO>>(selectedMaterials);
                    //listRootMaterials = Mapper.Map<List<HisMaterialInStockSDO>>(selectedRootMaterials);
                    //instockMaterials = materialInstocks.Where(o => !String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                    //listRootMaterials = materialInstocks.Where(o => String.IsNullOrEmpty(o.ParentNodeId)).ToList();

                    var dataGoups = instockMaterials.GroupBy(p => p.ParentNodeId).ToList();

                    int dem = 1;

                    foreach (var gRoot in selectedRootMedicines)
                    {
                        HisMediMateBloodInStock_Print root = new HisMediMateBloodInStock_Print(gRoot, true);
                        var grByNode = dataGoups.FirstOrDefault(o => o.Key == gRoot.NodeId);
                        if (grByNode != null && grByNode.Count() > 0)
                        {
                            var childs = grByNode.ToList();
                            root.AvailableAmount = childs.Sum(s => (s.AvailableAmount ?? 0));
                            root.TotalAmount = childs.Sum(s => (s.TotalAmount ?? 0));
                            root.MEDICINE_TYPE_NAME = dem + "." + root.MEDICINE_TYPE_NAME.ToUpper();
                            var gByPrice = childs.GroupBy(g => new { g.REGISTER_NUMBER, g.PACKAGE_NUMBER, g.EXPIRED_DATE, g.MATERIAL_TYPE_NAME, g.MATERIAL_TYPE_CODE }).ToList();

                            medicineInStockSdoPrints.Add(root);
                            foreach (var medicineInStock in gByPrice)
                            {
                                HisMediMateBloodInStock_Print ado = new HisMediMateBloodInStock_Print(medicineInStock.FirstOrDefault(), false);

                                ado.MEDICINE_TYPE_NAME = "     " + ado.MEDICINE_TYPE_NAME;
                                ado.AvailableAmount = medicineInStock.Sum(p => (p.AvailableAmount ?? 0));
                                ado.TotalAmount = medicineInStock.Sum(s => (s.TotalAmount ?? 0));
                                medicineInStockSdoPrints.Add(ado);
                            }
                            dem++;
                        }
                    }
                    //int dem = 1;
                    //foreach (var root in listRootMaterials)
                    //{
                    //    MOS.SDO.HisMedicineInStockSDO rootMaterial = new MOS.SDO.HisMedicineInStockSDO();
                    //    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.SDO.HisMedicineInStockSDO>(rootMaterial, root);
                    //    rootMaterial.MEDICINE_TYPE_NAME = dem + "." + root.MATERIAL_TYPE_NAME.ToUpper();
                    //    rootMaterial.MEDICINE_TYPE_CODE = root.MATERIAL_TYPE_CODE;
                    //    medicineInStockSdos.Add(rootMaterial);
                    //    foreach (var materialInStock in instockMaterials)
                    //    {
                    //        if (materialInStock.ParentNodeId == root.NodeId)
                    //        {
                    //            MOS.SDO.HisMedicineInStockSDO rootMaterialChild = new MOS.SDO.HisMedicineInStockSDO();
                    //            Inventec.Common.Mapper.DataObjectMapper.Map<MOS.SDO.HisMedicineInStockSDO>(rootMaterialChild, materialInStock);
                    //            rootMaterialChild.MEDICINE_TYPE_NAME = "     " + materialInStock.MATERIAL_TYPE_NAME;
                    //            rootMaterialChild.MEDICINE_TYPE_CODE = materialInStock.MATERIAL_TYPE_CODE;
                    //            medicineInStockSdos.Add(rootMaterialChild);
                    //        }
                    //    }
                    //    dem++;
                    //}
                    //foreach (var item in medicineInStockSdos)
                    //{
                    //    HisMediMateBloodInStock_Print medicine = new HisMediMateBloodInStock_Print();
                    //    Inventec.Common.Mapper.DataObjectMapper.Map<HisMediMateBloodInStock_Print>(medicine, item);
                    //    medicine.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64
                    //    (item.EXPIRED_DATE ?? 0));
                    //    medicineInStockSdoPrints.Add(medicine);
                    //}
                }
                else
                {
                    List<HisBloodTypeInStockSDO> lstBlood = new List<HisBloodTypeInStockSDO>();
                    lstBlood = rdo.lstBloodInStockSDO;
                    foreach (var item in lstBlood)
                    {
                        HisMediMateBloodInStock_Print blood = new HisMediMateBloodInStock_Print();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisMediMateBloodInStock_Print>(blood, item);

                        blood.MEDICINE_TYPE_NAME = item.BloodTypeName;
                        blood.MEDICINE_TYPE_CODE = item.BloodTypeCode;
                        // blood.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item. ?? 0);
                        blood.TotalAmount = item.Volume;
                        blood.AvailableAmount = item.Amount;
                        medicineInStockSdoPrints.Add(blood);
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
