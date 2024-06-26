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
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.AggrApprove.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrApprove
{
    public partial class frmAggrApprove : HIS.Desktop.Utility.FormBase
    {
        //private void LoadListMatyMetyId()
        //{
        //    try
        //    {
        //        V_HIS_EXP_MEST ChmsExpMest = new V_HIS_EXP_MEST();
        //        ChmsExpMest = listExpMest.First();
        //        this.mediStockId = ChmsExpMest.MEDI_STOCK_ID;
        //        this.expMestTypeID = ChmsExpMest.EXP_MEST_TYPE_ID;
        //        var stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == ChmsExpMest.MEDI_STOCK_ID);
        //        //if (stock.IS_GOODS_RESTRICT == 1)
        //        //{
        //        List<V_HIS_MEDI_STOCK_MATY> material = new List<V_HIS_MEDI_STOCK_MATY>();
        //        List<V_HIS_MEDI_STOCK_METY> medicine = new List<V_HIS_MEDI_STOCK_METY>();
        //        HisMediStockMatyViewFilter matyFilter = new HisMediStockMatyViewFilter();
        //        matyFilter.MEDI_STOCK_ID = stock.ID;
        //        material = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK_MATY>>(
        //            "api/HisMediStockMaty/GetView", ApiConsumers.MosConsumer, matyFilter, null);
        //        //material = material.Where(o => o.IS_GOODS_RESTRICT == 1).ToList();
        //        if (material != null && material.Count > 0)
        //        {
        //            materialTypeIds = material.Select(o => o.MATERIAL_TYPE_ID).ToList();
        //        }

        //        HisMediStockMetyViewFilter metyFilter = new HisMediStockMetyViewFilter();
        //        metyFilter.MEDI_STOCK_ID = stock.ID;
        //        medicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK_METY>>(
        //           "api/HisMediStockMety/GetView", ApiConsumers.MosConsumer, metyFilter, null);
        //        //medicine = medicine.Where(o => o.IS_GOODS_RESTRICT == 1).ToList();
        //        if (medicine != null && medicine.Count > 0)
        //        {
        //            medicineTypeIds = medicine.Select(o => o.MEDICINE_TYPE_ID).ToList();
        //        }
        //        // }
        //    }
        //    catch (Exception ex)
        //    {

        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void LoadDataToMediMateInStock()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                mediFilter.MEDI_STOCK_ID = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this.currentModule.RoomId).ID;
                mediFilter.IS_LEAF = true;

                // mediFilter.MEDICINE_TYPE_IDs = medicineTypeIds;
                //.Where(p=> !medicineAdoIds.Contains(p)).ToList()

                listMediTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK, ApiConsumers.MosConsumer, mediFilter, null);

                HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                mateFilter.MEDI_STOCK_ID = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this.currentModule.RoomId).ID;
                mateFilter.IS_LEAF = true;
                //if (materialTypeIds != null && materialTypeIds.Count > 0)
                //{
                //    mateFilter.MATERIAL_TYPE_IDs = materialTypeIds;
                //}
                listMateTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, mateFilter, null);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataMetyMatyReq()
        {
            try
            {
                WaitingManager.Show();
                if (expMestApproveADO == null || expMestApproveADO.Count == 0)
                {
                    return;
                }
                CommonParam param = new CommonParam();
                HisExpMestMetyReqViewFilter medicineFilter = new HisExpMestMetyReqViewFilter();
                List<ExpMestApproveADO> dataCheck = GetListCheck();
                dataCheck = dataCheck.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST).ToList();
                medicineFilter.EXP_MEST_IDs = dataCheck.Select(o => o.ID).ToList();

                var medicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/GetView", ApiConsumers.MosConsumer, medicineFilter, param);
                mediMateTypeADO = new List<MediMateTypeADO>();
                foreach (var item in medicines)
                {
                    MediMateTypeADO mediAdo = new MediMateTypeADO();
                    var rs = listMediTypeInStock.FirstOrDefault(p => p.Id == item.MEDICINE_TYPE_ID);
                    if (rs != null)
                    {
                        mediAdo.MEDI_MATE_TYPE_ID = rs.Id;
                        mediAdo.MEDI_MATE_TYPE_CODE = rs.MedicineTypeCode;
                        mediAdo.MEDI_MATE_TYPE_NAME = rs.MedicineTypeName;
                        mediAdo.SERVICE_UNIT_NAME = rs.ServiceUnitName;
                        mediAdo.SUM_IN_STOCK = rs.AvailableAmount;
                        mediAdo.YCD_AMOUNT = item.AMOUNT;
                    }
                    mediMateTypeADO.Add(mediAdo);
                }

                HisExpMestMatyReqViewFilter materialFilter = new HisExpMestMatyReqViewFilter();
                materialFilter.EXP_MEST_IDs = dataCheck.Select(o => o.ID).ToList();
                var materials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/GetView", ApiConsumers.MosConsumer, materialFilter, param);
                foreach (var item in materials)
                {
                    MediMateTypeADO mateAdo = new MediMateTypeADO();
                    var rs = listMateTypeInStock.FirstOrDefault(p => p.Id == item.MATERIAL_TYPE_ID);
                    if (rs != null)
                    {
                        mateAdo.MEDI_MATE_TYPE_ID = rs.Id;
                        mateAdo.MEDI_MATE_TYPE_CODE = rs.MaterialTypeCode;
                        mateAdo.MEDI_MATE_TYPE_NAME = rs.MaterialTypeName;
                        mateAdo.SERVICE_UNIT_NAME = rs.ServiceUnitName;
                        mateAdo.YCD_AMOUNT = item.AMOUNT;
                        mateAdo.SUM_IN_STOCK = rs.AvailableAmount;
                    }
                    mediMateTypeADO.Add(mateAdo);
                }
                var mediMateTypeADOGroup = mediMateTypeADO.GroupBy(g => g.MEDI_MATE_TYPE_ID).ToList();
                List<MediMateTypeADO> meidMateGroups = new List<MediMateTypeADO>();
                foreach (var item in mediMateTypeADOGroup)
                {
                     MediMateTypeADO mateAdoGroup = new MediMateTypeADO();
                     mateAdoGroup.MEDI_MATE_TYPE_NAME = item.First().MEDI_MATE_TYPE_NAME;
                     mateAdoGroup.MEDI_MATE_TYPE_CODE = item.First().MEDI_MATE_TYPE_CODE;
                     mateAdoGroup.SERVICE_UNIT_NAME = item.First().SERVICE_UNIT_NAME;
                     mateAdoGroup.SUM_IN_STOCK = item.First().SUM_IN_STOCK;
                     mateAdoGroup.YCD_AMOUNT = item.Sum(o => o.YCD_AMOUNT);
                     meidMateGroups.Add(mateAdoGroup);
                }

                gridControlMediMate.DataSource = meidMateGroups;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
