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
                medicineInStockSdoPrints = new List<HisMediMateBloodInStock_Print>();

                if (rdo.isCheckMedicine)
                {
                    ProcessHierarchicalData<HisMedicineInStockSDO>(rdo.lstMedicineInStockSDO, true); 
                }
                else if (rdo.isCheckMaterial)
                {
                    ProcessHierarchicalData<HisMaterialInStockSDO>(rdo.lstMaterialInStockSDO, false);
                }
                else // Blood
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

        private void ProcessHierarchicalData<T>(List<T> data, bool isMedicine) where T : class
        {
            // Lọc các node gốc (ParentNodeId = null và isTypeNode = true mà không có cha)
            var rootNodes = data.Where(o => GetParentNodeId(o) == null && GetIsTypeNode(o)).ToList(); //danh sách các node gốc 
            int rootIndex = 1;

            foreach (var root in rootNodes)// duyệt qua từng node gốc trong danh sách
            {
                ProcessNode(root, data, "", rootIndex, isMedicine, 0); // xử lý từng node trong root gốc
                rootIndex++;
            }
        }

        private void ProcessNode<T>(T node, List<T> data, string prefix, int rootIndex, bool isMedicine, int level) where T : class
        {
            //node là từng node gốc hiện tại đang xử lý, data là danh sách dữ liệu ban đầu 
            HisMediMateBloodInStock_Print rootAdo = isMedicine //khai báo 
                ? new HisMediMateBloodInStock_Print(node as HisMedicineInStockSDO, true)
                : new HisMediMateBloodInStock_Print(node as HisMaterialInStockSDO, true);

            // Định dạng tên node gốc
            rootAdo.MEDICINE_TYPE_NAME = string.Format("{0}{1}{2}.{3}", new string(' ', level * 5), prefix, rootIndex, rootAdo.MEDICINE_TYPE_NAME.ToUpper());
            // Lấy các node con hoặc leaf thuộc node hiện tại
            var children = data.Where(o => GetParentNodeId(o) == GetNodeId(node)).ToList();
            var leafChildren = children.Where(o => !GetIsTypeNode(o)).ToList(); //những leaf con là thuốc cụ thể
            var nodeChildren = children.Where(o => GetIsTypeNode(o)).ToList();//những leaf con mà còn là node

            // Tính tổng số lượng từ các leaf con
            rootAdo.AvailableAmount = leafChildren.Sum(o => GetAvailableAmount(o) ?? 0);
            rootAdo.TotalAmount = leafChildren.Sum(o => GetTotalAmount(o) ?? 0);

            // Thêm node gốc vào danh sách
            medicineInStockSdoPrints.Add(rootAdo);

            // Xử lý các leaf con trực tiếp
            var groupedLeaves = leafChildren
                .GroupBy(o => new { RegisterNumber = GetRegisterNumber(o), PackageNumber = GetPackageNumber(o), ExpiredDate = GetExpiredDate(o), TypeName = GetTypeName(o, isMedicine), TypeCode = GetTypeCode(o, isMedicine) })
                .ToList();

            foreach (var leafGroup in groupedLeaves)
            {
                var firstLeaf = leafGroup.First();
                HisMediMateBloodInStock_Print leafAdo = isMedicine
                    ? new HisMediMateBloodInStock_Print(firstLeaf as HisMedicineInStockSDO, false)
                    : new HisMediMateBloodInStock_Print(firstLeaf as HisMaterialInStockSDO, false);
                leafAdo.MEDICINE_TYPE_NAME = string.Format("{1}{0}", leafAdo.MEDICINE_TYPE_NAME, new string(' ', (level+1)*5));
                leafAdo.AvailableAmount = leafGroup.Sum(p => GetAvailableAmount(p) ?? 0);
                leafAdo.TotalAmount = leafGroup.Sum(p => GetTotalAmount(p) ?? 0);
                leafAdo.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(leafAdo.EXPIRED_DATE ?? 0));
                medicineInStockSdoPrints.Add(leafAdo);
            }

            // Xử lý các node con
            int childIndex = 1;
            foreach (var childNode in nodeChildren)
            {
                ProcessNode(childNode, data, string.Format("{0}{1}.", prefix, rootIndex), childIndex, isMedicine, level + 1);
                childIndex++;
            }
        }

        private string GetNodeId<T>(T item)
        {
            return item.GetType().GetProperty("NodeId")?.GetValue(item)?.ToString();
        }

        private string GetParentNodeId<T>(T item)
        {
            return item.GetType().GetProperty("ParentNodeId")?.GetValue(item)?.ToString();
        }

        private bool GetIsTypeNode<T>(T item)
        {
            return (bool)(item.GetType().GetProperty("isTypeNode")?.GetValue(item) ?? false);
        }

        private decimal? GetAvailableAmount<T>(T item)
        {
            return (decimal?)item.GetType().GetProperty("AvailableAmount")?.GetValue(item);
        }

        private decimal? GetTotalAmount<T>(T item)
        {
            return (decimal?)item.GetType().GetProperty("TotalAmount")?.GetValue(item);
        }

        private string GetRegisterNumber<T>(T item)
        {
            return item.GetType().GetProperty("REGISTER_NUMBER")?.GetValue(item)?.ToString();
        }

        private string GetPackageNumber<T>(T item)
        {
            return item.GetType().GetProperty("PACKAGE_NUMBER")?.GetValue(item)?.ToString();
        }

        private decimal? GetExpiredDate<T>(T item)
        {
            return (decimal?)item.GetType().GetProperty("EXPIRED_DATE")?.GetValue(item);
        }

        private string GetTypeName<T>(T item, bool isMedicine)
        {
            return item.GetType().GetProperty(isMedicine ? "MEDICINE_TYPE_NAME" : "MATERIAL_TYPE_NAME")?.GetValue(item)?.ToString();
        }

        private string GetTypeCode<T>(T item, bool isMedicine)
        {
            return item.GetType().GetProperty(isMedicine ? "MEDICINE_TYPE_CODE" : "MATERIAL_TYPE_CODE")?.GetValue(item)?.ToString();
        }

    }
}
