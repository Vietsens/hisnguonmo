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
using FlexCel.Report;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000132.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000132
{
    public class Mps000132Processor : AbstractProcessor
    {
        Mps000132PDO rdo;
        List<MetyMatyParentADO> listParentMedicineType = new List<MetyMatyParentADO>();
        List<MetyMatyParentADO> listParentMaterialType = new List<MetyMatyParentADO>();
        List<Mps000132ADO> listMedicine = new List<Mps000132ADO>();
        List<Mps000132ADO> listMaterial = new List<Mps000132ADO>();
        List<Mps000132ADO> listMedicineNotParent = new List<Mps000132ADO>();
        List<Mps000132ADO> listMaterialNotParent = new List<Mps000132ADO>();
        List<HIS_MEDICINE_GROUP> lstMedicinGroup = new List<HIS_MEDICINE_GROUP>();
        List<V_HIS_MEDI_STOCK> medistockList = new List<V_HIS_MEDI_STOCK>();
        public const long TYPE_THUOC = 1;
        public const long TYPE_VATTU = 2;
        public Mps000132Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000132PDO)rdoBase;
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
                    ProcessSingleKeyV2();
                    singleTag.ProcessData(store, singleValueDictionary);
                    barCodeTag.ProcessData(store, dicImage);
                    objectTag.AddObjectData(store, "listMedicineGroup", rdo.lstMedicinGroup);
                    objectTag.AddObjectData(store, "listParentMedicineType", listParentMedicineType);
                    objectTag.AddObjectData(store, "listParentMaterialType", listParentMaterialType);

                    objectTag.AddObjectData(store, "listMedicineNotParent", listMedicineNotParent);
                    objectTag.AddObjectData(store, "listMaterialNotParent", listMaterialNotParent);

                    objectTag.AddObjectData(store, "ListUser", rdo.listMestInveUser);
                    objectTag.AddObjectData(store, "ListRoleUserEnd", rdo.roleAdo);
                    objectTag.AddObjectData(store, "ListMedistock", this.medistockList);
                    objectTag.AddRelationship(store, "listMedicineGroup", "listParentMedicineType", "MEDICINE_GROUP_CODE", "MEDICINE_GROUP_CODE");
                    objectTag.AddRelationship(store, "listParentMedicineType", "listMedicine", "ID", "PARENT_ID");
                    objectTag.AddRelationship(store, "listParentMaterialType", "listMaterial", "ID", "PARENT_ID");


                    objectTag.AddObjectData(store, "listMaterial", listMaterial);
                    objectTag.AddObjectData(store, "listMedicine", listMedicine);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listMedicine  " + Inventec.Common.Logging.LogUtil.GetMemberName(() => listMedicine.Select(o => o.REGISTER_NUMBER)), listMedicine.Select(o => o.REGISTER_NUMBER)));

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listMaterial  " + Inventec.Common.Logging.LogUtil.GetMemberName(() => listMaterial.Select(o => o.REGISTER_NUMBER)), listMaterial.Select(o => o.REGISTER_NUMBER)));

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(" rdo.lstMedicinGroup  " + Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.lstMedicinGroup), rdo.lstMedicinGroup));

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

        void ProcessSingleKeyV2()
        {
            try
            {
                if (rdo.medistockIdList != null)
                {
                    this.medistockList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => rdo.medistockIdList.Contains(o.ID)).ToList();
                }

                AddObjectKeyIntoListkey<V_HIS_MEDI_STOCK_PERIOD>(rdo.mediStock, false);

                rdo.listMrs000132ADO = new List<MPS.Processor.Mps000132.PDO.Mps000132ADO>();

                if (rdo.lstMestPeriodMedi != null && rdo.lstMestPeriodMedi.Count > 0)
                {
                    this.listMedicine.AddRange((from r in rdo.lstMestPeriodMedi select new MPS.Processor.Mps000132.PDO.Mps000132ADO(r)).ToList());
                    rdo.listMrs000132ADO.AddRange((from r in rdo.lstMestPeriodMedi select new MPS.Processor.Mps000132.PDO.Mps000132ADO(r)).ToList());
                    if (rdo.ListMediStockMety != null && rdo.ListMediStockMety.Count() > 0)
                    {
                        foreach (var item in this.listMedicine)
                        {
                            var checkAlert = rdo.ListMediStockMety.Where(o => o.MEDICINE_TYPE_ID == item.METY_MATY_ID);
                            if (checkAlert != null && checkAlert.Count() > 0)
                            {
                                item.ALERT_MAX_IN_STOCK = checkAlert.OrderByDescending(o => o
                                .CREATE_TIME).FirstOrDefault().ALERT_MAX_IN_STOCK;
                            }
                        }
                    }

                    this.listMedicine = this.listMedicine != null ? this.listMedicine.OrderBy(o => o.MEDI_MATE_TYPE_NAME).ThenBy(p => p.ALERT_MAX_IN_STOCK).ToList() : this.listMedicine;

                    this.listMedicineNotParent = this.listMedicine.Where(o => !o.PARENT_ID.HasValue).ToList();
                }

                if (rdo.lstMestPeriodMate != null && rdo.lstMestPeriodMate.Count > 0)
                {
                    this.listMaterial.AddRange((from r in rdo.lstMestPeriodMate select new MPS.Processor.Mps000132.PDO.Mps000132ADO(r)).ToList());
                    if (rdo.ListMediStockMaty != null && rdo.ListMediStockMaty.Count() > 0)
                    {
                        foreach (var item in this.listMaterial)
                        {
                            var checkAlert = rdo.ListMediStockMaty.Where(o => o.MATERIAL_TYPE_ID == item.METY_MATY_ID);
                            if (checkAlert != null && checkAlert.Count() > 0)
                            {
                                item.ALERT_MAX_IN_STOCK = checkAlert.OrderByDescending(o => o
                                .CREATE_TIME).FirstOrDefault().ALERT_MAX_IN_STOCK;
                            }
                        }
                    }

                    rdo.listMrs000132ADO.AddRange((from r in rdo.lstMestPeriodMate select new MPS.Processor.Mps000132.PDO.Mps000132ADO(r)).ToList());
                    this.listMaterial = this.listMaterial != null ? this.listMaterial.OrderBy(o => o
                        .MEDI_MATE_TYPE_NAME).ThenBy(o => o.ALERT_MAX_IN_STOCK).ToList() : this.listMaterial;
                    this.listMaterialNotParent = this.listMaterial.Where(o => !o.PARENT_ID.HasValue).ToList();
                }

                if (rdo.listMrs000132ADO != null)
                {
                    SetSingleKey(new KeyValue(Mps000132ExtendSingleKey.COUNT_LIST, rdo.listMrs000132ADO.Count.ToString()));
                }

                string typeName = "";

                List<long> parentMedicineTypeList = rdo.listMrs000132ADO.Where(o => o.TYPE == TYPE_THUOC && o.PARENT_ID.HasValue).Select(p => p.PARENT_ID.Value).ToList();
                List<long> parentMaterialTypeList = rdo.listMrs000132ADO.Where(o => o.TYPE == TYPE_VATTU && o.PARENT_ID.HasValue).Select(p => p.PARENT_ID.Value).ToList();


                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => parentMedicineTypeList), parentMedicineTypeList));
                if (parentMedicineTypeList != null && parentMedicineTypeList.Count() > 0)
                {

                    //CommonParam param = new CommonParam();
                    //MOS.Filter.HisMedicineTypeViewFilter Filter = new MOS.Filter.HisMedicineTypeViewFilter();
                    //Filter.IS_ACTIVE = 1;
                    //var medicineTypes = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>>("api/HisMedicineType/GetView", ApiConsumers.MosConsumer, Filter, param);

                    var medicineTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => parentMedicineTypeList.Contains(o.ID)).ToList();
                    //medicineTypes = medicineTypes.Where(o => parentMedicineTypeList.Contains(o.ID)).ToList();

                    Inventec.Common.Logging.LogSystem.Debug("log 1___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypes.Select(o => new { o.ID, o.MEDICINE_GROUP_CODE })), medicineTypes.Select(o => new { o.ID, o.MEDICINE_GROUP_CODE })));

                    foreach (var item in medicineTypes)
                    {
                        if (item.MEDICINE_GROUP_CODE == null || string.IsNullOrEmpty((item.MEDICINE_GROUP_CODE)))
                        {
                            HIS_MEDICINE_GROUP group = new HIS_MEDICINE_GROUP();
                            item.MEDICINE_GROUP_CODE = group.MEDICINE_GROUP_CODE;
                        }
                        MetyMatyParentADO mety = new MetyMatyParentADO(item);
                        listParentMedicineType.Add(mety);
                        
                    }
                    foreach (var item in listParentMedicineType)
                    {
                        if (item.MEDICINE_GROUP_CODE == null || string.IsNullOrEmpty((item.MEDICINE_GROUP_CODE)))
                        {
                            HIS_MEDICINE_GROUP group = new HIS_MEDICINE_GROUP();
                            group.MEDICINE_GROUP_CODE = item.MEDICINE_GROUP_CODE;
                           // group.ID = item.MEDICINE_GROUP_ID ?? 0 ;
                            lstMedicinGroup.Add(group);
                        }
                        
                    }
                   
                    Inventec.Common.Logging.LogSystem.Debug("rdo.lstMedicinGroup____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.lstMedicinGroup.Select(o => new { o.ID, o.MEDICINE_GROUP_CODE })), rdo.lstMedicinGroup.Select(o => new { o.ID, o.MEDICINE_GROUP_CODE })));
                    var data = rdo.lstMedicinGroup.Where(o => listParentMedicineType.Select(p => p.MEDICINE_GROUP_CODE).Contains(o.MEDICINE_GROUP_CODE)).ToList();
                   
                    if (data != null && data.Count() > 0)
                    {
                        rdo.lstMedicinGroup = data;
                    }
                    else
                    {
                        rdo.lstMedicinGroup.AddRange(lstMedicinGroup);
                    }
                    Inventec.Common.Logging.LogSystem.Debug("listParentMedicineType___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listParentMedicineType.Select(o => new { o.ID, o.MEDICINE_GROUP_CODE })), listParentMedicineType.Select(o => new { o.ID, o.MEDICINE_GROUP_CODE })));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypes.Select(o => new { o.ID, o.MEDICINE_GROUP_CODE })), medicineTypes.Select(o => new { o.ID, o.MEDICINE_GROUP_CODE })));
                }

                if (parentMaterialTypeList != null && parentMaterialTypeList.Count() > 0)
                {
                    var materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => parentMaterialTypeList.Contains(o.ID)).ToList();
                    foreach (var item in materialTypes)
                    {
                        MetyMatyParentADO mety = new MetyMatyParentADO(item);
                        listParentMaterialType.Add(mety);
                    }
                }

                SetSingleKey(new KeyValue(Mps000132ExtendSingleKey.TYPE_PRINT, typeName));

                if (rdo.listMestInveUser != null && rdo.listMestInveUser.Count > 0)
                {
                    RoleADO role = new RoleADO();

                    int count = 1;
                    foreach (var item in rdo.listMestInveUser)
                    {
                        if (count > 10)
                            break;
                        System.Reflection.PropertyInfo piRole = typeof(RoleADO).GetProperty("Role" + count);
                        System.Reflection.PropertyInfo piUser = typeof(RoleADO).GetProperty("User" + count);
                        piRole.SetValue(role, item.EXECUTE_ROLE_NAME);
                        piUser.SetValue(role, item.USERNAME);
                        count++;
                    }
                    rdo.roleAdo.Add(role);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //void ProcessSingleKey()
        //{
        //    try
        //    {
        //        AddObjectKeyIntoListkey<V_HIS_MEDI_STOCK_PERIOD>(rdo.mediStock, false);

        //        rdo.listMrs000132ADO = new List<MPS.Processor.Mps000132.PDO.Mps000132ADO>();
        //        if (rdo.lstMestPeriodMety != null && rdo.lstMestPeriodMety.Count > 0)
        //        {
        //            rdo.listMrs000132ADO.AddRange((from r in rdo.lstMestPeriodMety select new MPS.Processor.Mps000132.PDO.Mps000132ADO(r)).ToList());
        //        }
        //        if (rdo.lstMestPeriodMaty != null && rdo.lstMestPeriodMaty.Count > 0)
        //        {
        //            rdo.listMrs000132ADO.AddRange((from r in rdo.lstMestPeriodMaty select new MPS.Processor.Mps000132.PDO.Mps000132ADO(r)).ToList());
        //        }

        //        if (rdo.lstMestPeriodBlty != null && rdo.lstMestPeriodBlty.Count > 0)
        //        {
        //            rdo.listMrs000132ADO.AddRange((from r in rdo.lstMestPeriodBlty select new MPS.Processor.Mps000132.PDO.Mps000132ADO(r)).ToList());
        //        }

        //        if (rdo.listMrs000132ADO != null)
        //        {
        //            SetSingleKey(new KeyValue(Mps000132ExtendSingleKey.COUNT_LIST, rdo.listMrs000132ADO.Count.ToString()));
        //        }
        //        string typeName = "";
        //        if (rdo.IsThuoc)
        //        {
        //            typeName = "THUỐC";
        //            rdo.listMrs000132ADO = rdo.listMrs000132ADO.Where(p => p.TYPE == 1).ToList();
        //        }
        //        else if (rdo.IsVatTu)
        //        {
        //            typeName = "VẬT TƯ Y TẾ";
        //            rdo.listMrs000132ADO = rdo.listMrs000132ADO.Where(p => p.TYPE == 2).ToList();
        //        }
        //        else if (rdo.IsMau)
        //        {
        //            typeName = "MÁU";
        //            rdo.listMrs000132ADO = rdo.listMrs000132ADO.Where(p => p.TYPE == 3).ToList();
        //        }
        //        SetSingleKey(new KeyValue(Mps000132ExtendSingleKey.TYPE_PRINT, typeName));

        //        if (rdo.listMestInveUser != null && rdo.listMestInveUser.Count > 0)
        //        {
        //            RoleADO role = new RoleADO();

        //            int count = 1;
        //            foreach (var item in rdo.listMestInveUser)
        //            {
        //                if (count > 10)
        //                    break;
        //                System.Reflection.PropertyInfo piRole = typeof(RoleADO).GetProperty("Role" + count);
        //                System.Reflection.PropertyInfo piUser = typeof(RoleADO).GetProperty("User" + count);
        //                piRole.SetValue(role, item.EXECUTE_ROLE_NAME);
        //                piUser.SetValue(role, item.USERNAME);
        //                count++;
        //            }
        //            rdo.roleAdo.Add(role);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
    }
}
