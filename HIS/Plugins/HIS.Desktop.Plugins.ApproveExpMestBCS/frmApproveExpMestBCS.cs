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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ApproveExpMestBCS.ADO;
using HIS.Desktop.Plugins.ApproveExpMestBCS.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApproveExpMestBCS
{
    public partial class frmApproveExpMestBCS : FormBase
    {
        private long expMestId;
        DelegateSelectData delegateSelectData = null;

        HIS_EXP_MEST currentExpMest = null;
        V_HIS_MEDI_STOCK mediStock = null;

        List<HisMedicineTypeInStockSDO> listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
        List<HisMaterialTypeInStockSDO> listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();

        List<MedicineTypeADO> medicineAdos = new List<MedicineTypeADO>();
        List<MaterialTypeADO> materialAdos = new List<MaterialTypeADO>();
        List<HIS_EXP_MEST_METY_REQ> expMestMetyReqs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_MATY_REQ> expMestMatyReqs = new List<HIS_EXP_MEST_MATY_REQ>();

        private HisExpMestResultSDO resultSDO = null;


        public frmApproveExpMestBCS(Inventec.Desktop.Common.Modules.Module currentModule, long expmestid, DelegateSelectData _delegateSelectData)
            : base(currentModule)
        {
            InitializeComponent();
            this.expMestId = expmestid;
            this.delegateSelectData = _delegateSelectData;
            HisConfig.LoadConfig();
        }

        private void frmApproveExpMestBCS_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.LoadExpMest();
                this.VisibleColumnBCS();
                this.LoadDataInStock();
                this.LoadDataMedicine();
                this.LoadDataMaterial();
                this.LoadDataAutoReplace();
                if (medicineAdos != null && medicineAdos.Count > 0)
                {
                    this.xtraTabControl.SelectedTabPageIndex = 0;
                }
                else if (materialAdos != null && materialAdos.Count > 0)
                {
                    this.xtraTabControl.SelectedTabPageIndex = 1;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleColumnBCS()
        {
            try
            {
                if (HisConfig.IS_ALLOW_REPLACE == "1")
                {
                    gridColumn_Medicine_ReplaceName.Visible = true;
                    gridColumn_Medicine_Replace.Visible = true;
                    gridColumn_Medicine_ReplaceAmount.Visible = true;
                    gridColumn_Material_ReplaceName.Visible = true;
                    gridColumn_Material_Replace.Visible = true;
                    gridColumn_Material_ReplaceAmount.Visible = true;
                }
                else
                {
                    gridColumn_Medicine_ReplaceName.Visible = false;
                    gridColumn_Medicine_Replace.Visible = false;
                    gridColumn_Medicine_ReplaceAmount.Visible = false;
                    gridColumn_Material_ReplaceName.Visible = false;
                    gridColumn_Material_Replace.Visible = false;
                    gridColumn_Material_ReplaceAmount.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMest()
        {
            try
            {
                HisExpMestFilter chmsFilter = new HisExpMestFilter();
                chmsFilter.ID = expMestId;
                chmsFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS;
                List<HIS_EXP_MEST> expMests = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, chmsFilter, null);
                currentExpMest = expMests != null ? expMests.FirstOrDefault() : null;

                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataInStock()
        {
            CommonParam param = new CommonParam();
            try
            {
                HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                mediFilter.MEDI_STOCK_ID = this.mediStock.ID;
                mediFilter.IS_LEAF = true;
                if (HisConfig.IsDontPresExpiredItem)
                {
                    mediFilter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                }

                listMediTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK, ApiConsumers.MosConsumer, mediFilter, null);

                HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                mateFilter.MEDI_STOCK_ID = this.mediStock.ID;
                mateFilter.IS_LEAF = true;
                if (HisConfig.IsDontPresExpiredItem)
                {
                    mateFilter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                }

                listMateTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, mateFilter, null);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listMateTypeInStock ", listMateTypeInStock));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataMedicine()
        {
            try
            {
                HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                metyReqFilter.EXP_MEST_ID = this.currentExpMest.ID;
                List<HIS_EXP_MEST_METY_REQ> metyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyReqFilter, null);

                this.expMestMetyReqs = metyReqs;

                List<HIS_EXP_MEST_MEDICINE> medicines = null;
                if (HisConfig.IS_ALLOW_REPLACE == "1" && metyReqs != null && metyReqs.Count > 0)
                {
                    HisExpMestMedicineFilter medicineFilter = new HisExpMestMedicineFilter();
                    medicineFilter.EXP_MEST_ID = this.currentExpMest.ID;
                    medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, null);
                }

                if (metyReqs != null && metyReqs.Count > 0)
                {
                    var Groups = metyReqs.GroupBy(g => g.MEDICINE_TYPE_ID).ToList();
                    foreach (var group in Groups)
                    {
                        List<HIS_EXP_MEST_METY_REQ> listGroup = group.ToList();
                        MedicineTypeADO ado = new MedicineTypeADO(listGroup);

                        if (listMediTypeInStock == null)
                        {
                            listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                        }
                        var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                        var inStock = listMediTypeInStock.FirstOrDefault(p => p.Id == group.Key);

                        if (inStock != null)
                        {
                            ado.AVAIL_AMOUNT = (inStock.AvailableAmount ?? 0);
                            ado.TON_KHO = (inStock.TotalAmount ?? 0);
                            ado.MEDICINE_TYPE_CODE = inStock.MedicineTypeCode;
                            ado.IsReplace = false;
                            ado.MEDICINE_TYPE_NAME = inStock.MedicineTypeName;
                            ado.MEDICINE_TYPE_ID = inStock.Id;
                            ado.ACTIVE_INGR_BHYT_NAME = inStock.ActiveIngrBhytName;
                            ado.ACTIVE_INGR_BHYT_CODE = inStock.ActiveIngrBhytCode;
                            ado.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                            ado.CONCENTRA = inStock.Concentra;

                            if ((ado.AMOUNT - ado.DD_AMOUNT) > (inStock.AvailableAmount ?? 0))
                            {
                                ado.YCD_AMOUNT = inStock.AvailableAmount ?? 0;
                            }
                            else
                            {
                                ado.YCD_AMOUNT = ado.AMOUNT - ado.DD_AMOUNT;
                            }
                            ado.IsCheck = true;
                            if (ado.DD_AMOUNT == ado.AMOUNT)
                            {
                                ado.IsCheck = false;
                            }

                            if (medicineType != null && medicineType.IS_ALLOW_EXPORT_ODD == 1)
                            {
                                ado.IS_ALLOW_EXPORT_ODD = true;
                            }
                            else
                                ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;

                        }
                        else if (medicineType != null)
                        {
                            ado.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                            ado.MEDICINE_TYPE_NAME = medicineType.MEDICINE_TYPE_NAME;
                            ado.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
                            ado.ACTIVE_INGR_BHYT_CODE = medicineType.ACTIVE_INGR_BHYT_CODE;
                            ado.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                            ado.CONCENTRA = medicineType.CONCENTRA;
                        }
                        medicineAdos.Add(ado);
                    }
                }

                if (medicines != null && medicines.Count > 0)
                {
                    var Groups = medicines.GroupBy(g => g.EXP_MEST_METY_REQ_ID).ToList();
                    foreach (var group in Groups)
                    {
                        List<HIS_EXP_MEST_MEDICINE> listGroup = group.ToList();
                        MedicineTypeADO replaceAdo = medicineAdos.FirstOrDefault(o => o.Requests.Any(a => a.ID == group.Key));
                        if (replaceAdo == null) continue;
                        listGroup = listGroup.Where(o => o.TDL_MEDICINE_TYPE_ID != replaceAdo.MEDICINE_TYPE_ID).ToList();
                        var GroupByType = listGroup.GroupBy(g => g.TDL_MEDICINE_TYPE_ID).ToList();
                        foreach (var item in GroupByType)
                        {
                            List<HIS_EXP_MEST_MEDICINE> list = item.ToList();
                            MedicineTypeADO ado = medicineAdos.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.Key && o.REPLACE_MEDICINE_TYPE_ID == replaceAdo.MEDICINE_TYPE_ID);
                            if (ado == null)
                            {
                                ado = new MedicineTypeADO(list);
                                ado.IsApproved = true;
                                replaceAdo.DD_AMOUNT = replaceAdo.DD_AMOUNT - ado.DD_AMOUNT;
                                replaceAdo.TT_AMOUNT = replaceAdo.TT_AMOUNT + ado.DD_AMOUNT;

                                ado.REPLACE_MEDICINE_TYPE_ID = replaceAdo.MEDICINE_TYPE_ID;
                                ado.REPLACE_MEDICINE_TYPE_NAME = replaceAdo.MEDICINE_TYPE_NAME;

                                if (listMediTypeInStock == null)
                                {
                                    listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                                }
                                var medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == ado.MEDICINE_TYPE_ID);

                                var inStock = listMediTypeInStock.FirstOrDefault(p => p.Id == ado.MEDICINE_TYPE_ID);
                                if (inStock != null)
                                {
                                    ado.AVAIL_AMOUNT = inStock.AvailableAmount ?? 0;
                                    ado.MEDICINE_TYPE_CODE = inStock.MedicineTypeCode;
                                    ado.IsReplace = true;
                                    ado.MEDICINE_TYPE_NAME = inStock.MedicineTypeName;
                                    ado.MEDICINE_TYPE_ID = inStock.Id;
                                    ado.ACTIVE_INGR_BHYT_NAME = inStock.ActiveIngrBhytName;
                                    ado.ACTIVE_INGR_BHYT_CODE = inStock.ActiveIngrBhytCode;
                                    ado.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                                    ado.YCD_AMOUNT = 0;
                                    ado.IsCheck = false;
                                    ado.CONCENTRA = inStock.Concentra;

                                    if (medicineType != null && medicineType.IS_ALLOW_EXPORT_ODD == 1)
                                    {
                                        ado.IS_ALLOW_EXPORT_ODD = true;
                                    }
                                    else
                                        ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;

                                }
                                else if (medicineType != null)
                                {
                                    ado.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                                    ado.MEDICINE_TYPE_NAME = medicineType.MEDICINE_TYPE_NAME;
                                    ado.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
                                    ado.ACTIVE_INGR_BHYT_CODE = medicineType.ACTIVE_INGR_BHYT_CODE;
                                    ado.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                                    ado.CONCENTRA = medicineType.CONCENTRA;
                                }
                                medicineAdos.Add(ado);
                            }
                            else
                            {
                                decimal ddAmount = list.Sum(s => s.AMOUNT);
                                ado.DD_AMOUNT = ado.DD_AMOUNT + ddAmount;
                                replaceAdo.DD_AMOUNT = replaceAdo.DD_AMOUNT - ddAmount;
                                replaceAdo.TT_AMOUNT = replaceAdo.TT_AMOUNT + ddAmount;
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

        private void LoadDataMaterial()
        {
            try
            {
                HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                matyReqFilter.EXP_MEST_ID = this.currentExpMest.ID;
                List<HIS_EXP_MEST_MATY_REQ> matyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyReqFilter, null);

                this.expMestMatyReqs = matyReqs;

                List<HIS_EXP_MEST_MATERIAL> materials = null;
                if (HisConfig.IS_ALLOW_REPLACE == "1" && matyReqs != null && matyReqs.Count > 0)
                {
                    HisExpMestMaterialFilter materialFilter = new HisExpMestMaterialFilter();
                    materialFilter.EXP_MEST_ID = this.currentExpMest.ID;
                    materials = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, null);
                }

                if (matyReqs != null && matyReqs.Count > 0)
                {
                    var Groups = matyReqs.GroupBy(g => g.MATERIAL_TYPE_ID).ToList();
                    foreach (var group in Groups)
                    {
                        List<HIS_EXP_MEST_MATY_REQ> listGroup = group.ToList();
                        MaterialTypeADO ado = new MaterialTypeADO(listGroup);

                        if (listMateTypeInStock == null)
                        {
                            listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                        }
                        var materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                        var inStock = listMateTypeInStock.FirstOrDefault(p => p.Id == group.Key);

                        if (inStock != null)
                        {
                            ado.AVAIL_AMOUNT = (inStock.AvailableAmount ?? 0);
                            ado.TON_KHO = (inStock.TotalAmount ?? 0);
                            ado.MATERIAL_TYPE_CODE = inStock.MaterialTypeCode;
                            ado.IsReplace = false;
                            ado.MATERIAL_TYPE_NAME = inStock.MaterialTypeName;
                            ado.MATERIAL_TYPE_ID = inStock.Id;
                            ado.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                            ado.CONCENTRA = inStock.Concentra;

                            if ((ado.AMOUNT - ado.DD_AMOUNT) > (inStock.AvailableAmount ?? 0))
                            {
                                ado.YCD_AMOUNT = inStock.AvailableAmount ?? 0;
                            }
                            else
                            {
                                ado.YCD_AMOUNT = ado.AMOUNT - ado.DD_AMOUNT;
                            }
                            ado.IsCheck = true;
                            if (ado.DD_AMOUNT == ado.AMOUNT)
                            {
                                ado.IsCheck = false;
                            }

                            if (materialType != null && materialType.IS_ALLOW_EXPORT_ODD == 1)
                            {
                                ado.IS_ALLOW_EXPORT_ODD = true;
                            }
                            else
                                ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;

                        }
                        else if (materialType != null)
                        {
                            ado.MATERIAL_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                            ado.MATERIAL_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                            ado.SERVICE_UNIT_NAME = materialType.SERVICE_UNIT_NAME;
                            ado.CONCENTRA = materialType.CONCENTRA;
                        }
                        materialAdos.Add(ado);
                    }
                }

                if (materials != null && materials.Count > 0)
                {
                    var Groups = materials.GroupBy(g => g.EXP_MEST_MATY_REQ_ID).ToList();
                    foreach (var group in Groups)
                    {
                        List<HIS_EXP_MEST_MATERIAL> listGroup = group.ToList();
                        MaterialTypeADO replaceAdo = materialAdos.FirstOrDefault(o => o.Requests.Any(a => a.ID == group.Key));
                        if (replaceAdo == null) continue;
                        listGroup = listGroup.Where(o => o.TDL_MATERIAL_TYPE_ID != replaceAdo.MATERIAL_TYPE_ID).ToList();

                        var GroupByType = listGroup.GroupBy(g => g.TDL_MATERIAL_TYPE_ID).ToList();
                        foreach (var item in GroupByType)
                        {
                            List<HIS_EXP_MEST_MATERIAL> list = item.ToList();
                            MaterialTypeADO ado = materialAdos.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.Key && o.REPLACE_MATERIAL_TYPE_ID == replaceAdo.MATERIAL_TYPE_ID);
                            if (ado == null)
                            {
                                ado = new MaterialTypeADO(listGroup);
                                ado.IsApproved = true;
                                replaceAdo.DD_AMOUNT = replaceAdo.DD_AMOUNT - ado.DD_AMOUNT;
                                replaceAdo.TT_AMOUNT = replaceAdo.TT_AMOUNT + ado.DD_AMOUNT;

                                ado.REPLACE_MATERIAL_TYPE_ID = replaceAdo.MATERIAL_TYPE_ID;
                                ado.REPLACE_MATERIAL_TYPE_NAME = replaceAdo.MATERIAL_TYPE_NAME;

                                if (listMateTypeInStock == null)
                                {
                                    listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                                }
                                var materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == ado.MATERIAL_TYPE_ID);

                                var inStock = listMateTypeInStock.FirstOrDefault(p => p.Id == ado.MATERIAL_TYPE_ID);
                                if (inStock != null)
                                {
                                    ado.AVAIL_AMOUNT = inStock.AvailableAmount ?? 0;
                                    ado.MATERIAL_TYPE_CODE = inStock.MaterialTypeCode;
                                    ado.IsReplace = true;
                                    ado.MATERIAL_TYPE_NAME = inStock.MaterialTypeName;
                                    ado.MATERIAL_TYPE_ID = inStock.Id;
                                    ado.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                                    ado.YCD_AMOUNT = 0;
                                    ado.IsCheck = false;
                                    ado.CONCENTRA = inStock.Concentra;

                                    if (materialType != null && materialType.IS_ALLOW_EXPORT_ODD == 1)
                                    {
                                        ado.IS_ALLOW_EXPORT_ODD = true;
                                    }
                                    else
                                        ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;

                                }
                                else if (materialType != null)
                                {
                                    ado.MATERIAL_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                                    ado.MATERIAL_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                                    ado.SERVICE_UNIT_NAME = materialType.SERVICE_UNIT_NAME;
                                    ado.CONCENTRA = materialType.CONCENTRA;
                                }
                                materialAdos.Add(ado);
                            }
                            else
                            {
                                decimal ddAmount = list.Sum(s => s.AMOUNT);
                                ado.DD_AMOUNT = ado.DD_AMOUNT + ddAmount;
                                replaceAdo.DD_AMOUNT = replaceAdo.DD_AMOUNT - ddAmount;
                                replaceAdo.TT_AMOUNT = replaceAdo.TT_AMOUNT + ddAmount;
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

        private void LoadDataAutoReplace()
        {
            try
            {
                if (HisConfig.IS_ALLOW_REPLACE == "1" && HisConfig.IS_AUTO_REPLACE == "1")
                {
                    HisMediStockReplaceSDOFilter filter = new HisMediStockReplaceSDOFilter();
                    filter.MediStockId = this.mediStock.ID;
                    filter.MaterialTypeIds = materialAdos != null ? materialAdos.Where(o => !o.IsReplace && !o.IsApproved).Select(s => s.MATERIAL_TYPE_ID).ToList() : null;
                    filter.MedicineTypeIds = medicineAdos != null ? medicineAdos.Where(o => !o.IsReplace && !o.IsApproved).Select(s => s.MEDICINE_TYPE_ID).ToList() : null;

                    HisMediStockReplaceSDO replaceSDO = new BackendAdapter(new CommonParam()).Get<HisMediStockReplaceSDO>("api/HisMediStock/GetReplaceSDO", ApiConsumers.MosConsumer, filter, null);

                    if (replaceSDO != null)
                    {
                        var lisMediReqs = medicineAdos != null ? medicineAdos.Where(o => !o.IsReplace && !o.IsApproved).ToList() : null;
                        var lisMateReqs = materialAdos != null ? materialAdos.Where(o => !o.IsReplace && !o.IsApproved).ToList() : null;

                        if (lisMediReqs != null && lisMediReqs.Count > 0 && replaceSDO.MedicineReplaces != null && replaceSDO.MedicineReplaces.Count > 0)
                        {
                            foreach (var item in lisMediReqs)
                            {
                                decimal needApprove = item.AMOUNT - item.CURRENT_DD_AMOUNT;
                                if (item.IsReplace || item.IsApproved || item.TON_KHO > 0 || needApprove <= 0) continue;
                                L_HIS_EXP_MEST_MEDICINE replace = replaceSDO.MedicineReplaces != null ? replaceSDO.MedicineReplaces.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID) : null;
                                if (replace == null) continue;
                                var inStock = listMediTypeInStock.FirstOrDefault(p => p.Id == replace.REPLACE_MEDICINE_TYPE_ID);
                                if (inStock == null || (inStock.AvailableAmount ?? 0) <= 0) continue;

                                MedicineTypeADO replaceMedicine = new MedicineTypeADO();
                                replaceMedicine.REPLACE_MEDICINE_TYPE_ID = item.MEDICINE_TYPE_ID;
                                replaceMedicine.REPLACE_MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                replaceMedicine.MEDICINE_TYPE_CODE = inStock.MedicineTypeCode;
                                replaceMedicine.MEDICINE_TYPE_NAME = inStock.MedicineTypeName;
                                replaceMedicine.MEDICINE_TYPE_ID = inStock.Id;
                                replaceMedicine.ACTIVE_INGR_BHYT_NAME = inStock.ActiveIngrBhytName;
                                replaceMedicine.ACTIVE_INGR_BHYT_CODE = inStock.ActiveIngrBhytCode;
                                replaceMedicine.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                                replaceMedicine.AVAIL_AMOUNT = inStock.AvailableAmount ?? 0;
                                replaceMedicine.IsCheck = true;
                                replaceMedicine.IsReplace = true;
                                replaceMedicine.AMOUNT = 0;
                                replaceMedicine.CONCENTRA = inStock.Concentra;

                                decimal vailable = (inStock.AvailableAmount ?? 0);
                                if (vailable >= needApprove)
                                {
                                    replaceMedicine.YCD_AMOUNT = needApprove;
                                }
                                else
                                {
                                    replaceMedicine.YCD_AMOUNT = vailable;
                                }
                                // nếu đã tồn tại thuốc thay thế thì remove 
                                medicineAdos.RemoveAll(o => o.REPLACE_MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID && !o.IsApproved);
                                medicineAdos.Add(replaceMedicine);

                                item.YCD_AMOUNT = ((item.AMOUNT - replaceMedicine.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMedicine.YCD_AMOUNT));
                                if (item.YCD_AMOUNT <= 0)
                                {
                                    item.IsCheck = false;
                                }
                            }
                        }

                        if (lisMateReqs != null && lisMateReqs.Count > 0 && replaceSDO.MaterialReplaces != null && replaceSDO.MaterialReplaces.Count > 0)
                        {
                            foreach (var item in lisMateReqs)
                            {
                                decimal needApprove = item.AMOUNT - item.CURRENT_DD_AMOUNT;
                                if (item.IsReplace || item.IsApproved || item.TON_KHO > 0 || needApprove <= 0) continue;
                                L_HIS_EXP_MEST_MATERIAL replace = replaceSDO.MaterialReplaces != null ? replaceSDO.MaterialReplaces.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID) : null;
                                if (replace == null) continue;
                                var inStock = listMateTypeInStock.FirstOrDefault(p => p.Id == replace.REPLACE_MATERIAL_TYPE_ID);
                                if (inStock == null || (inStock.AvailableAmount ?? 0) <= 0) continue;
                                MaterialTypeADO replaceMaterial = new MaterialTypeADO();
                                replaceMaterial.REPLACE_MATERIAL_TYPE_ID = item.MATERIAL_TYPE_ID;
                                replaceMaterial.REPLACE_MATERIAL_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                                replaceMaterial.MATERIAL_TYPE_CODE = inStock.MaterialTypeCode;
                                replaceMaterial.MATERIAL_TYPE_NAME = inStock.MaterialTypeName;
                                replaceMaterial.MATERIAL_TYPE_ID = inStock.Id;
                                replaceMaterial.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                                replaceMaterial.AVAIL_AMOUNT = inStock.AvailableAmount ?? 0;
                                replaceMaterial.IsCheck = true;
                                replaceMaterial.IsReplace = true;
                                replaceMaterial.AMOUNT = 0;
                                replaceMaterial.CONCENTRA = inStock.Concentra;
                                decimal vailable = (inStock.AvailableAmount ?? 0);
                                if (vailable >= needApprove)
                                {
                                    replaceMaterial.YCD_AMOUNT = needApprove;
                                }
                                else
                                {
                                    replaceMaterial.YCD_AMOUNT = vailable;
                                }
                                // nếu đã tồn tại thuốc thay thế thì remove 
                                materialAdos.RemoveAll(o => o.REPLACE_MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID && !o.IsApproved);
                                materialAdos.Add(replaceMaterial);

                                item.YCD_AMOUNT = ((item.AMOUNT - replaceMaterial.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMaterial.YCD_AMOUNT));
                                if (item.YCD_AMOUNT <= 0)
                                {
                                    item.IsCheck = false;
                                }
                            }
                        }
                    }
                }
                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = medicineAdos;
                gridControlMedicine.EndUpdate();

                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = materialAdos;
                gridControlMaterial.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column.FieldName != "YCD_AMOUNT")
                    return;
                var data = (MedicineTypeADO)gridViewMedicine.GetRow(e.RowHandle);
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (!data.IsReplace)
                    {
                        if (data.YCD_AMOUNT <= 0)
                        {
                            valid = false;
                            message = "Số lượng duyệt phải lớn hơn 0";
                        }
                        else if (data.YCD_AMOUNT > (data.AMOUNT - (data.CURRENT_DD_AMOUNT + data.CURRENT_YC_AMOUNT)))
                        {
                            valid = false;
                            message = String.Format("Số lượng duyệt {0} lớn hơn số lượng yêu cầu {1}", data.YCD_AMOUNT, (data.AMOUNT - (data.CURRENT_DD_AMOUNT + data.CURRENT_YC_AMOUNT)));
                        }
                        else if (!data.IS_ALLOW_EXPORT_ODD)
                        {
                            decimal x = Math.Abs(Math.Round(data.YCD_AMOUNT, 3) - Math.Floor(data.YCD_AMOUNT));
                            if (x > 0)
                            {
                                valid = false;
                                message = "Không cho phép duyệt lẻ";
                            }
                        }
                    }
                    else
                    {
                        var req = medicineAdos.FirstOrDefault(o => !o.IsReplace && o.MEDICINE_TYPE_ID == data.REPLACE_MEDICINE_TYPE_ID);
                        if (data.YCD_AMOUNT <= 0)
                        {
                            valid = false;
                            message = "Số lượng duyệt phải lớn hơn 0";
                        }
                        else if (data.YCD_AMOUNT > (req.AMOUNT - req.CURRENT_DD_AMOUNT))
                        {
                            valid = false;
                            message = String.Format("Số lượng duyệt {0} lớn hơn số lượng yêu cầu {1}", data.YCD_AMOUNT, (req.AMOUNT - req.CURRENT_DD_AMOUNT));
                        }
                        else if (!data.IS_ALLOW_EXPORT_ODD)
                        {
                            decimal x = Math.Abs(Math.Round(data.YCD_AMOUNT, 3) - Math.Floor(data.YCD_AMOUNT));
                            if (x > 0)
                            {
                                valid = false;
                                message = "Không cho phép duyệt lẻ";
                            }
                        }
                        req.YCD_AMOUNT = ((req.AMOUNT - data.YCD_AMOUNT) >= req.YCD_AMOUNT ? req.YCD_AMOUNT : (req.AMOUNT - data.YCD_AMOUNT));
                        req.CURRENT_YC_AMOUNT = data.YCD_AMOUNT;
                    }
                    if (!valid)
                        gridViewMedicine.SetColumnError(gridViewMedicine.FocusedColumn, message);
                    else
                        gridViewMedicine.ClearColumnErrors();
                }
                gridControlMedicine.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MedicineTypeADO)gridViewMedicine.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "Replace")
                        {
                            if (data.IsCheck && !data.IsReplace)
                                e.RepositoryItem = repositoryItemButtonReplaceMedicine_Enable;
                            else
                                e.RepositoryItem = repositoryItemButtonReplaceMedicine_Disable;
                        }
                        else if (e.Column.FieldName == "IsCheck")
                        {
                            if (data.IsApproved)
                                e.RepositoryItem = repositoryItemCheckMedicine_Disable;
                            else
                                e.RepositoryItem = repositoryItemCheckMedicine_Enable;
                        }
                        else if (e.Column.FieldName == "YCD_AMOUNT")
                        {
                            if (data.IsCheck && (!data.IsApproved))
                                e.RepositoryItem = repositoryItemSpinMedicineYcdAmount;
                            else
                                e.RepositoryItem = repositoryItemSpinMedicineYcdAmount_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MedicineTypeADO)gridViewMedicine.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if ((data.AMOUNT - data.DD_AMOUNT) > data.AVAIL_AMOUNT)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (data.IsReplace)// thuốc thay thế
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column.FieldName != "YCD_AMOUNT")
                    return;
                var data = (MaterialTypeADO)gridViewMaterial.GetRow(e.RowHandle);
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (!data.IsReplace)
                    {
                        if (data.YCD_AMOUNT <= 0)
                        {
                            valid = false;
                            message = "Số lượng duyệt phải lớn hơn 0";
                        }
                        else if (data.YCD_AMOUNT > (data.AMOUNT - (data.CURRENT_DD_AMOUNT + data.CURRENT_YC_AMOUNT)))
                        {
                            valid = false;
                            message = String.Format("Số lượng duyệt {0} lớn hơn số lượng yêu cầu {1}", data.YCD_AMOUNT, (data.AMOUNT - (data.CURRENT_DD_AMOUNT + data.CURRENT_YC_AMOUNT)));
                        }
                        else if (!data.IS_ALLOW_EXPORT_ODD)
                        {
                            decimal x = Math.Abs(Math.Round(data.YCD_AMOUNT, 3) - Math.Floor(data.YCD_AMOUNT));
                            if (x > 0)
                            {
                                valid = false;
                                message = "Không cho phép duyệt lẻ";
                            }
                        }
                    }
                    else
                    {
                        var req = materialAdos.FirstOrDefault(o => !o.IsReplace && o.MATERIAL_TYPE_ID == data.MATERIAL_TYPE_ID);
                        if (data.YCD_AMOUNT <= 0)
                        {
                            valid = false;
                            message = "Số lượng duyệt phải lớn hơn 0";
                        }
                        else if (data.YCD_AMOUNT > (req.AMOUNT - req.CURRENT_DD_AMOUNT))
                        {
                            valid = false;
                            message = String.Format("Số lượng duyệt {0} lớn hơn số lượng yêu cầu {1}", data.YCD_AMOUNT, (req.AMOUNT - req.CURRENT_DD_AMOUNT));
                        }
                        else if (!data.IS_ALLOW_EXPORT_ODD)
                        {
                            decimal x = Math.Abs(Math.Round(data.YCD_AMOUNT, 3) - Math.Floor(data.YCD_AMOUNT));
                            if (x > 0)
                            {
                                valid = false;
                                message = "Không cho phép duyệt lẻ";
                            }
                        }
                        if (valid)
                        {
                            req.YCD_AMOUNT = ((req.AMOUNT - data.YCD_AMOUNT) >= req.YCD_AMOUNT ? req.YCD_AMOUNT : (req.AMOUNT - data.YCD_AMOUNT));
                            req.CURRENT_YC_AMOUNT = data.YCD_AMOUNT;
                        }
                    }
                    if (!valid)
                        gridViewMaterial.SetColumnError(gridViewMaterial.FocusedColumn, message);
                    else
                        gridViewMaterial.ClearColumnErrors();
                }
                gridControlMaterial.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MaterialTypeADO)gridViewMaterial.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "Replace")
                        {
                            if (data.IsCheck && !data.IsReplace)
                                e.RepositoryItem = repositoryItemButtonReplaceMaterial_Enable;
                            else
                                e.RepositoryItem = repositoryItemButtonReplaceMaterial_Disable;
                        }
                        else if (e.Column.FieldName == "IsCheck")
                        {
                            if (data.IsApproved)
                                e.RepositoryItem = repositoryItemCheckMaterial_Disable;
                            else
                                e.RepositoryItem = repositoryItemCheckMaterial_Enable;
                        }
                        else if (e.Column.FieldName == "YCD_AMOUNT")
                        {
                            if (data.IsCheck && (!data.IsApproved))
                                e.RepositoryItem = repositoryItemSpinMaterialYcdAmount;
                            else
                                e.RepositoryItem = repositoryItemSpinMaterialYcdAmount_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MaterialTypeADO)gridViewMaterial.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if ((data.AMOUNT - data.DD_AMOUNT) > data.AVAIL_AMOUNT)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (data.IsReplace)// thuốc thay thế
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;
                //get data 2 grid bỏ bản ghi nào mà có medicine_type_id null or = 0,, material_type_id cũng thế
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HisExpMestApproveSDO data = new HisExpMestApproveSDO();
                data.ExpMestId = this.currentExpMest.ID;
                data.ReqRoomId = this.currentModuleBase.RoomId;
                data.IsFinish = true;
                data.Description = txtDescription.Text;
                data.Materials = new List<ExpMaterialTypeSDO>();
                data.Medicines = new List<ExpMedicineTypeSDO>();

                if (!this.MakeMaterial(ref data))
                {
                    return;
                }

                if (!this.MakeMedicine(ref data))
                {
                    return;
                }

                if (data.Medicines.Count == 0 && data.Materials.Count == 0)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn thuốc, vật tư", "Thông báo");
                    return;
                }

                LogSystem.Info("Input api/HisExpMest/Approve: " + LogUtil.TraceData("data", data));
                var rs = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/Approve", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    resultSDO = rs;
                    this.delegateSelectData(resultSDO);
                    this.LoadDataInStock();
                    this.LoadDataMedicine();
                    this.LoadDataMaterial();

                }

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool MakeMedicine(ref HisExpMestApproveSDO data)
        {
            if (medicineAdos != null && medicineAdos.Count > 0)
            {
                Mapper.CreateMap<HIS_EXP_MEST_METY_REQ, HIS_EXP_MEST_METY_REQ>();
                List<HIS_EXP_MEST_METY_REQ> metyReqAlls = Mapper.Map<List<HIS_EXP_MEST_METY_REQ>>(this.expMestMetyReqs);
                var checkMedicines = medicineAdos.Where(o => o.MEDICINE_TYPE_ID > 0 && o.IsCheck == true).ToList();
                var requests = checkMedicines.Where(o => !o.IsReplace).ToList();
                foreach (var medicine in requests)
                {
                    decimal ycdAmount = medicine.YCD_AMOUNT;

                    if (ycdAmount <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng thuốc phải lớn hơn 0", "Thông báo");
                        return false;
                    }

                    if (ycdAmount > medicine.AVAIL_AMOUNT)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng thuốc lớn hơn tồn kho: {0}", medicine.MEDICINE_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    if (ycdAmount > (medicine.AMOUNT - (medicine.CURRENT_DD_AMOUNT + medicine.CURRENT_YC_AMOUNT)))
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng thuốc lớn hơn yêu cầu: {0}", medicine.MEDICINE_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    List<HIS_EXP_MEST_METY_REQ> metyReqs = metyReqAlls.Where(o => medicine.Requests.Any(a => a.ID == o.ID)).ToList();

                    metyReqs = metyReqs.OrderBy(o => o.AMOUNT).ToList();
                    foreach (var req in metyReqs)
                    {
                        decimal availAmount = req.AMOUNT - (req.DD_AMOUNT ?? 0);
                        if (availAmount <= 0) continue;
                        if (ycdAmount <= 0) break;
                        if (availAmount >= ycdAmount)
                        {
                            ExpMedicineTypeSDO sdo = new ExpMedicineTypeSDO();
                            sdo.Amount = ycdAmount;
                            sdo.ExpMestMetyReqId = req.ID;
                            sdo.MedicineTypeId = medicine.MEDICINE_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + ycdAmount;
                            ycdAmount = 0;
                            data.Medicines.Add(sdo);
                        }
                        else
                        {
                            ExpMedicineTypeSDO sdo = new ExpMedicineTypeSDO();
                            sdo.Amount = availAmount;
                            sdo.ExpMestMetyReqId = req.ID;
                            sdo.MedicineTypeId = medicine.MEDICINE_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + availAmount;
                            ycdAmount = ycdAmount - availAmount;
                            data.Medicines.Add(sdo);
                        }
                    }
                }

                var replaces = checkMedicines.Where(o => o.IsReplace).ToList();
                foreach (var repl in replaces)
                {
                    var request = medicineAdos.FirstOrDefault(o => !o.IsReplace && o.MEDICINE_TYPE_ID == repl.REPLACE_MEDICINE_TYPE_ID);
                    decimal ycdAmount = repl.YCD_AMOUNT;

                    if (ycdAmount <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng thuốc phải lớn hơn 0", "Thông báo");
                        return false;
                    }

                    if (ycdAmount > repl.AVAIL_AMOUNT)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng thuốc lớn hơn tồn kho: {0}", repl.MEDICINE_TYPE_NAME), "Thông báo");
                        return false;
                    }
                    if (ycdAmount > (request.AMOUNT - request.CURRENT_DD_AMOUNT))
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng thuốc lớn hơn yêu cầu: {0} (Thay thế cho {1})", repl.MEDICINE_TYPE_NAME, request.MEDICINE_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    List<HIS_EXP_MEST_METY_REQ> metyReqs = metyReqAlls.Where(o => request.Requests.Any(a => a.ID == o.ID)).ToList();

                    metyReqs = metyReqs.OrderBy(o => o.AMOUNT).ToList();
                    foreach (var req in metyReqs)
                    {
                        decimal availAmount = req.AMOUNT - (req.DD_AMOUNT ?? 0);
                        if (availAmount <= 0) continue;
                        if (ycdAmount <= 0) break;
                        if (availAmount >= ycdAmount)
                        {
                            ExpMedicineTypeSDO sdo = new ExpMedicineTypeSDO();
                            sdo.Amount = ycdAmount;
                            sdo.ExpMestMetyReqId = req.ID;
                            sdo.MedicineTypeId = repl.MEDICINE_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + ycdAmount;
                            ycdAmount = 0;
                            data.Medicines.Add(sdo);
                        }
                        else
                        {
                            ExpMedicineTypeSDO sdo = new ExpMedicineTypeSDO();
                            sdo.Amount = availAmount;
                            sdo.ExpMestMetyReqId = req.ID;
                            sdo.MedicineTypeId = repl.MEDICINE_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + availAmount;
                            ycdAmount = ycdAmount - availAmount;
                            data.Medicines.Add(sdo);
                        }
                    }
                }
            }
            return true;
        }

        private bool MakeMaterial(ref HisExpMestApproveSDO data)
        {
            if (materialAdos != null && materialAdos.Count > 0)
            {
                Mapper.CreateMap<HIS_EXP_MEST_MATY_REQ, HIS_EXP_MEST_MATY_REQ>();
                List<HIS_EXP_MEST_MATY_REQ> matyReqAlls = Mapper.Map<List<HIS_EXP_MEST_MATY_REQ>>(this.expMestMatyReqs);

                var checkMaterials = materialAdos.Where(o => o.MATERIAL_TYPE_ID > 0 && o.IsCheck == true).ToList();
                var requests = checkMaterials.Where(o => !o.IsReplace).ToList();
                foreach (var material in requests)
                {
                    decimal ycdAmount = material.YCD_AMOUNT;

                    if (ycdAmount <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng vật tư phải lớn hơn 0", "Thông báo");
                        return false;
                    }

                    if (ycdAmount > material.AVAIL_AMOUNT)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng vật tư lớn hơn tồn kho: {0}", material.MATERIAL_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    if (ycdAmount > (material.AMOUNT - (material.CURRENT_DD_AMOUNT + material.CURRENT_YC_AMOUNT)))
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng vật tư lớn hơn yêu cầu: {0}", material.MATERIAL_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    List<HIS_EXP_MEST_MATY_REQ> matyReqs = matyReqAlls.Where(o => material.Requests.Any(a => a.ID == o.ID)).ToList(); ;

                    matyReqs = matyReqs.OrderBy(o => o.AMOUNT).ToList();
                    foreach (var req in matyReqs)
                    {
                        decimal availAmount = req.AMOUNT - (req.DD_AMOUNT ?? 0);
                        if (availAmount <= 0) continue;
                        if (ycdAmount <= 0) break;
                        if (availAmount >= ycdAmount)
                        {
                            ExpMaterialTypeSDO sdo = new ExpMaterialTypeSDO();
                            sdo.Amount = ycdAmount;
                            sdo.ExpMestMatyReqId = req.ID;
                            sdo.MaterialTypeId = material.MATERIAL_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + ycdAmount;
                            ycdAmount = 0;
                            data.Materials.Add(sdo);
                        }
                        else
                        {
                            ExpMaterialTypeSDO sdo = new ExpMaterialTypeSDO();
                            sdo.Amount = availAmount;
                            sdo.ExpMestMatyReqId = req.ID;
                            sdo.MaterialTypeId = material.MATERIAL_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + availAmount;
                            ycdAmount = ycdAmount - availAmount;
                            data.Materials.Add(sdo);
                        }
                    }
                }

                var replaces = checkMaterials.Where(o => o.IsReplace).ToList();
                foreach (var repl in replaces)
                {
                    var request = materialAdos.FirstOrDefault(o => !o.IsReplace && o.MATERIAL_TYPE_ID == repl.REPLACE_MATERIAL_TYPE_ID);
                    decimal ycdAmount = repl.YCD_AMOUNT;

                    if (ycdAmount <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng vật tư phải lớn hơn 0", "Thông báo");
                        return false;
                    }

                    if (ycdAmount > repl.AVAIL_AMOUNT)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng vật tư lớn hơn tồn kho: {0}", repl.MATERIAL_TYPE_NAME), "Thông báo");
                        return false;
                    }
                    if (ycdAmount > (request.AMOUNT - request.CURRENT_DD_AMOUNT))
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng vật tư lớn hơn yêu cầu: {0} (Thay thế cho {1})", repl.MATERIAL_TYPE_NAME, request.MATERIAL_TYPE_NAME), "Thông báo");
                        return false;
                    }

                    List<HIS_EXP_MEST_MATY_REQ> matyReqs = matyReqAlls.Where(o => request.Requests.Any(a => a.ID == o.ID)).ToList(); ;

                    matyReqs = matyReqs.OrderBy(o => o.AMOUNT).ToList();
                    foreach (var req in matyReqs)
                    {
                        decimal availAmount = req.AMOUNT - (req.DD_AMOUNT ?? 0);
                        if (availAmount <= 0) continue;
                        if (ycdAmount <= 0) break;
                        if (availAmount >= ycdAmount)
                        {
                            ExpMaterialTypeSDO sdo = new ExpMaterialTypeSDO();
                            sdo.Amount = ycdAmount;
                            sdo.ExpMestMatyReqId = req.ID;
                            sdo.MaterialTypeId = repl.MATERIAL_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + ycdAmount;
                            ycdAmount = 0;
                            data.Materials.Add(sdo);
                        }
                        else
                        {
                            ExpMaterialTypeSDO sdo = new ExpMaterialTypeSDO();
                            sdo.Amount = availAmount;
                            sdo.ExpMestMatyReqId = req.ID;
                            sdo.MaterialTypeId = repl.MATERIAL_TYPE_ID;
                            sdo.TreatmentId = req.TREATMENT_ID;
                            req.DD_AMOUNT = (req.DD_AMOUNT ?? 0) + availAmount;
                            ycdAmount = ycdAmount - availAmount;
                            data.Materials.Add(sdo);
                        }
                    }
                }
            }
            return true;
        }

        private void repositoryItemButtonReplaceMedicine_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                focusMedicine = (MedicineTypeADO)gridViewMedicine.GetFocusedRow();
                if (focusMedicine != null)
                {
                    this.ReplaceForm(focusMedicine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonReplaceMaterial_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                focusMaterial = (MaterialTypeADO)gridViewMaterial.GetFocusedRow();
                if (focusMaterial != null)
                {
                    this.ReplaceForm(focusMaterial);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        MedicineTypeADO focusMedicine = null;

        private void ReplaceForm(MedicineTypeADO expMestFocus)
        {
            try
            {
                frmReplace frm = new frmReplace(expMestFocus, this.currentModuleBase.RoomId, ReplaceMedicineSave);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReplaceMedicineSave(object prescription)
        {
            try
            {
                if (prescription != null && prescription is MetyMatyTypeADO)
                {
                    var expMestMedicineSelect = (MetyMatyTypeADO)prescription;
                    // thuốc thay thế
                    MedicineTypeADO replaceMedicine = new MedicineTypeADO();
                    replaceMedicine.REPLACE_MEDICINE_TYPE_ID = focusMedicine.MEDICINE_TYPE_ID;
                    replaceMedicine.REPLACE_MEDICINE_TYPE_NAME = focusMedicine.MEDICINE_TYPE_NAME;
                    replaceMedicine.MEDICINE_TYPE_CODE = expMestMedicineSelect.MedicineTypeCode;
                    replaceMedicine.MEDICINE_TYPE_NAME = expMestMedicineSelect.MedicineTypeName;
                    replaceMedicine.MEDICINE_TYPE_ID = expMestMedicineSelect.Id;
                    replaceMedicine.ACTIVE_INGR_BHYT_NAME = expMestMedicineSelect.ActiveIngrBhytName;
                    replaceMedicine.ACTIVE_INGR_BHYT_CODE = expMestMedicineSelect.ActiveIngrBhytCode;
                    replaceMedicine.SERVICE_UNIT_NAME = expMestMedicineSelect.ServiceUnitName;
                    replaceMedicine.AVAIL_AMOUNT = expMestMedicineSelect.AvailableAmount ?? 0;
                    replaceMedicine.IsCheck = true;
                    replaceMedicine.IsReplace = true;
                    replaceMedicine.AMOUNT = 0;
                    replaceMedicine.YCD_AMOUNT = expMestMedicineSelect.YCD_AMOUNT;
                    // nếu đã tồn tại thuốc thay thế thì remove 
                    medicineAdos.RemoveAll(o => o.REPLACE_MEDICINE_TYPE_ID == focusMedicine.MEDICINE_TYPE_ID && !o.IsApproved);
                    medicineAdos.Add(replaceMedicine);

                    foreach (var item in medicineAdos)
                    {
                        if (item == focusMedicine)
                        {
                            item.YCD_AMOUNT = ((item.AMOUNT - replaceMedicine.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMedicine.YCD_AMOUNT));
                            item.CURRENT_YC_AMOUNT = replaceMedicine.YCD_AMOUNT;
                        }
                        if (item.YCD_AMOUNT <= 0)
                        {
                            item.IsCheck = false;
                        }
                    }
                    gridControlMedicine.BeginUpdate();
                    gridControlMedicine.DataSource = medicineAdos;
                    gridControlMedicine.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        MaterialTypeADO focusMaterial = null;

        private void ReplaceForm(MaterialTypeADO expMestFocus)
        {
            try
            {
                frmReplace frm = new frmReplace(expMestFocus, this.currentModuleBase.RoomId, ReplaceMaterialSave);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReplaceMaterialSave(object prescription)
        {
            try
            {
                if (prescription != null && prescription is MetyMatyTypeADO)
                {
                    var materialSelect = (MetyMatyTypeADO)prescription;
                    // thuốc thay thế
                    MaterialTypeADO replaceMaterial = new MaterialTypeADO();
                    replaceMaterial.REPLACE_MATERIAL_TYPE_ID = focusMaterial.MATERIAL_TYPE_ID;
                    replaceMaterial.REPLACE_MATERIAL_TYPE_NAME = focusMaterial.MATERIAL_TYPE_NAME;
                    replaceMaterial.MATERIAL_TYPE_CODE = materialSelect.MedicineTypeCode;
                    replaceMaterial.MATERIAL_TYPE_NAME = materialSelect.MedicineTypeName;
                    replaceMaterial.MATERIAL_TYPE_ID = materialSelect.Id;
                    replaceMaterial.ACTIVE_INGR_BHYT_NAME = materialSelect.ActiveIngrBhytName;
                    replaceMaterial.SERVICE_UNIT_NAME = materialSelect.ServiceUnitName;
                    replaceMaterial.AVAIL_AMOUNT = materialSelect.AvailableAmount ?? 0;
                    replaceMaterial.IsCheck = true;
                    replaceMaterial.IsReplace = true;
                    replaceMaterial.AMOUNT = 0;
                    replaceMaterial.YCD_AMOUNT = materialSelect.YCD_AMOUNT;
                    // nếu đã tồn tại thuốc thay thế thì remove 
                    materialAdos.RemoveAll(o => o.REPLACE_MATERIAL_TYPE_ID == focusMaterial.MATERIAL_TYPE_ID && !o.IsApproved);
                    materialAdos.Add(replaceMaterial);

                    foreach (var item in materialAdos)
                    {
                        if (item == focusMaterial)
                        {
                            item.YCD_AMOUNT = ((item.AMOUNT - replaceMaterial.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMaterial.YCD_AMOUNT));
                            item.CURRENT_YC_AMOUNT = replaceMaterial.YCD_AMOUNT;
                        }
                        if (item.YCD_AMOUNT <= 0)
                        {
                            item.IsCheck = false;
                        }
                    }
                    gridControlMaterial.BeginUpdate();
                    gridControlMaterial.DataSource = materialAdos;
                    gridControlMaterial.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckMedicine_Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewMedicine.PostEditor();
                gridControlMedicine.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckMaterial_Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewMaterial.PostEditor();
                gridControlMaterial.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
