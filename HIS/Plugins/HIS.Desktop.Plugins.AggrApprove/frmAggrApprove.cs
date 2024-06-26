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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
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
        long expMestId = 0;
        long requestRoomId = 0;
        long mediStockId = 0;
        long expMestTypeID = 0;
        List<long> materialTypeIds;
        List<long> medicineTypeIds;
        List<HIS_EXP_MEST> curentExpMest { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines { get; set; }
        ExpMestApproveADO dataTreePrints { get; set; }


        List<HisMedicineTypeInStockSDO> listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
        List<HisMaterialTypeInStockSDO> listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();

        List<V_HIS_EXP_MEST> listExpmestApprove = new List<V_HIS_EXP_MEST>();
        List<V_HIS_EXP_MEST> ListExpMestRequest = new List<V_HIS_EXP_MEST>();
        List<ExpMestApproveADO> expMestApproveADO = new List<ExpMestApproveADO>();
        List<V_HIS_EXP_MEST> listExpMest = new List<V_HIS_EXP_MEST>();

        List<MediMateTypeADO> mediMateTypeADO { get; set; }

        Inventec.Desktop.Common.Modules.Module currentModule;

        MediMateTypeADO currentMediMate = null;

        public frmAggrApprove()
        {
            InitializeComponent();
        }
        public frmAggrApprove(Inventec.Desktop.Common.Modules.Module module, long ExpMestId)
		:base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.expMestId = ExpMestId;
                this.requestRoomId = module.RoomId;
                this.SetIcon();
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmAggrApprove_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataToTreeListAggrApprove();
                //LoadListMatyMetyId();
                LoadDataToMediMateInStock();
                LoadDataMetyMatyReq();
                GenerateMenuPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToTreeListAggrApprove()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.AGGR_EXP_MEST_ID = this.expMestId;
                listExpMest = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);
                if (listExpMest != null)
                {
                    lbDeparmentRequest.Text = listExpMest.First().REQ_DEPARTMENT_NAME;
                    lbUserRequest.Text = listExpMest.First().REQ_USERNAME;
                    lbDateTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listExpMest.First().TDL_INTRUCTION_TIME ?? 0);
                    lbAggrApproveCode.Text = listExpMest.First().TDL_AGGR_EXP_MEST_CODE;

                    listExpmestApprove = listExpMest.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE).ToList();

                    ListExpMestRequest = listExpMest.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST).ToList();

                    expMestApproveADO = new List<ExpMestApproveADO>();
                    var listExpMestGroup = listExpMest.GroupBy(g => g.EXP_MEST_STT_NAME).ToList();
                    foreach (var expMestgroup in listExpMestGroup)
                    {
                        ExpMestApproveADO ado = new ExpMestApproveADO();
                        ado.CONCRETE_ID__IN_SETY = expMestgroup.First().EXP_MEST_STT_NAME;
                        ado.EXP_MEST_STT_ID = expMestgroup.First().EXP_MEST_STT_ID;
                        ado.EXP_MEST_PARENT = expMestgroup.First().EXP_MEST_STT_NAME;
                        if (expMestgroup.First().EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {
                            ado.EXP_MEST_PARENT = "Đã duyệt";
                        }
                        ado.CHECK = true;
                        expMestApproveADO.Add(ado);

                        if (expMestgroup.First().EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {

                            //thuốc đã duyệt
                            HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                            expMestMedicineFilter.EXP_MEST_IDs = listExpmestApprove.Select(o => o.AGGR_EXP_MEST_ID ?? 0).ToList();
                             expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                            //vật tư đã duyệt
                            HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                            expMestMaterialFilter.EXP_MEST_IDs = listExpmestApprove.Select(o => o.AGGR_EXP_MEST_ID ?? 0).ToList();
                             expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                            if (expMestMedicines != null && expMestMedicines.Count > 0)
                            {

                                var expMestMedicinesGroup = expMestMedicines.GroupBy(g => new { g.APPROVAL_TIME, g.APPROVAL_USERNAME }).ToList();

                                foreach (var expMestMedigroup in expMestMedicinesGroup)
                                {
                                    ExpMestApproveADO Mediado = new ExpMestApproveADO();
                                    Mediado.PARENT_ID__IN_SETY = ado.CONCRETE_ID__IN_SETY;
                                    Mediado.CONCRETE_ID__IN_SETY = expMestMedigroup.Key.APPROVAL_TIME + expMestMedigroup.Key.APPROVAL_USERNAME;
                                    Mediado.EXP_MEST_PARENT = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(expMestMedigroup.Key.APPROVAL_TIME) + " - " + expMestMedigroup.Key.APPROVAL_USERNAME;

                                    Mediado.EXP_MEST_IDs = expMestMedigroup.Select(o => o.PRESCRIPTION_ID ?? 0).Distinct().ToList();
                                    Mediado.EXP_MEST_STT_ID = ado.EXP_MEST_STT_ID;
                                    expMestApproveADO.Add(Mediado);
                                    int d = 0;
                                    //var groupByExpMest = expMestMedigroup.GroupBy(g => g.EXP_MEST_ID).ToList();
                                    var listExpMestMediByGroup = listExpMest.Where(o => Mediado.EXP_MEST_IDs.Contains(o.ID)).ToList();

                                    foreach (var itemMedi in listExpMestMediByGroup.GroupBy(g => g.TDL_PATIENT_ID).ToList())
                                    {
                                        d++;
                                        var expMest = itemMedi.First();
                                        ExpMestApproveADO adoC = new ExpMestApproveADO();
                                        if (expMest != null)
                                        {
                                            adoC.CONCRETE_ID__IN_SETY = Mediado.CONCRETE_ID__IN_SETY + d;
                                            adoC.PARENT_ID__IN_SETY = Mediado.CONCRETE_ID__IN_SETY;
                                            adoC.ID = expMest.ID;
                                            adoC.EXP_MEST_IDs = itemMedi.Select(o => o.ID).Distinct().ToList();
                                            adoC.TDL_TREATMENT_CODE = expMest.TDL_TREATMENT_CODE;
                                            adoC.TDL_PATIENT_CODE = expMest.TDL_PATIENT_CODE;
                                            adoC.EXP_MEST_CODE = expMest.EXP_MEST_CODE;
                                            adoC.EXP_MEST_STT_ID = expMest.EXP_MEST_STT_ID;
                                            adoC.EXP_MEST_PARENT = expMest.TDL_PATIENT_NAME;
                                            expMestApproveADO.Add(adoC);
                                        }
                                        //var expMestMedi = expMestApproveADO.GroupBy(g => g.TDL_PATIENT_ID).ToList();
                                        //foreach (var item in expMestMedi)
                                        //{
                                        //     ExpMestApproveADO adoMediC = new ExpMestApproveADO();
                                        //     adoMediC.PARENT_ID__IN_SETY = adoC.CONCRETE_ID__IN_SETY;
                                        //    adoMediC.me
                                        //}
                                    }
                                }

                            }

                            if (expMestMaterials != null && expMestMaterials.Count > 0)
                            {
                                var expMestMaterialsGroup = expMestMaterials.GroupBy(g => new { g.APPROVAL_TIME, g.APPROVAL_USERNAME });

                                foreach (var expMestMategroup in expMestMaterialsGroup)
                                {
                                    ExpMestApproveADO Mateado = new ExpMestApproveADO();
                                    string mateKey = expMestMategroup.Key.APPROVAL_TIME + expMestMategroup.Key.APPROVAL_USERNAME;
                                    var dataMate = expMestApproveADO.FirstOrDefault(p => p.CONCRETE_ID__IN_SETY == mateKey);
                                    string keyCheck = "";
                                    if (dataMate == null)
                                    {
                                        Mateado.PARENT_ID__IN_SETY = ado.CONCRETE_ID__IN_SETY;
                                        Mateado.CONCRETE_ID__IN_SETY = expMestMategroup.Key.APPROVAL_TIME + expMestMategroup.Key.APPROVAL_USERNAME;
                                        Mateado.EXP_MEST_STT_ID = ado.EXP_MEST_STT_ID;

                                        Mateado.EXP_MEST_IDs = expMestMategroup.Select(o => o.PRESCRIPTION_ID ?? 0).Distinct().ToList();

                                        Mateado.EXP_MEST_PARENT = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(expMestMategroup.Key.APPROVAL_TIME) + " - " + expMestMategroup.Key.APPROVAL_USERNAME;
                                        expMestApproveADO.Add(Mateado);
                                        keyCheck = Mateado.CONCRETE_ID__IN_SETY;

                                        var listExpMestMateByGroup = listExpMest.Where(o => Mateado.EXP_MEST_IDs.Contains(o.ID)).ToList();

                                        int d = 0;
                                        foreach (var item in listExpMestMateByGroup.GroupBy(g => g.TDL_PATIENT_ID).ToList())
                                        {
                                            d++;
                                            var expMest = item.First();
                                            if (expMest != null)
                                            {
                                                var dataChild = expMestApproveADO.FirstOrDefault(p => p.CONCRETE_ID__IN_SETY == (keyCheck + d));
                                                if (dataChild == null)
                                                {
                                                    ExpMestApproveADO adoC = new ExpMestApproveADO();
                                                    adoC.CONCRETE_ID__IN_SETY = Mateado.CONCRETE_ID__IN_SETY + d;
                                                    adoC.PARENT_ID__IN_SETY = Mateado.CONCRETE_ID__IN_SETY;
                                                    adoC.EXP_MEST_STT_ID = expMest.EXP_MEST_STT_ID;
                                                    adoC.TDL_TREATMENT_CODE = expMest.TDL_TREATMENT_CODE;
                                                    adoC.TDL_PATIENT_CODE = expMest.TDL_PATIENT_CODE;
                                                    adoC.EXP_MEST_CODE = expMest.EXP_MEST_CODE;
                                                    adoC.EXP_MEST_IDs = item.Select(o => o.ID).Distinct().ToList();
                                                    adoC.ID = expMest.ID;
                                                    adoC.EXP_MEST_PARENT = expMest.TDL_PATIENT_NAME;
                                                    expMestApproveADO.Add(adoC);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        keyCheck = dataMate.CONCRETE_ID__IN_SETY;
                                        var listExpMestMateByGroup = listExpMest.Where(o => dataMate.EXP_MEST_IDs.Contains(o.ID)).ToList();

                                        int d = 0;
                                        foreach (var item in listExpMestMateByGroup.GroupBy(g => g.TDL_PATIENT_ID).ToList())
                                        {
                                            d++;
                                            var expMest = item.First();
                                            if (expMest != null)
                                            {
                                                var dataChild = expMestApproveADO.FirstOrDefault(p => p.CONCRETE_ID__IN_SETY == (keyCheck + d));
                                                if (dataChild == null)
                                                {
                                                    ExpMestApproveADO adoC = new ExpMestApproveADO();
                                                    adoC.CONCRETE_ID__IN_SETY = Mateado.CONCRETE_ID__IN_SETY + d;
                                                    adoC.PARENT_ID__IN_SETY = Mateado.CONCRETE_ID__IN_SETY;
                                                    adoC.EXP_MEST_STT_ID = expMest.EXP_MEST_STT_ID;
                                                    adoC.TDL_TREATMENT_CODE = expMest.TDL_TREATMENT_CODE;
                                                    adoC.TDL_PATIENT_CODE = expMest.TDL_PATIENT_CODE;
                                                    adoC.EXP_MEST_CODE = expMest.EXP_MEST_CODE;
                                                    adoC.ID = expMest.ID;
                                                    adoC.EXP_MEST_IDs = item.Select(o => o.ID).Distinct().ToList();
                                                    adoC.EXP_MEST_PARENT = expMest.TDL_PATIENT_NAME;
                                                    expMestApproveADO.Add(adoC);
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        else if (expMestgroup.First().EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            foreach (var item in expMestgroup)
                            {
                                ExpMestApproveADO adoCon = new ExpMestApproveADO();
                                adoCon.PARENT_ID__IN_SETY = ado.CONCRETE_ID__IN_SETY;
                                adoCon.CONCRETE_ID__IN_SETY = ado.CONCRETE_ID__IN_SETY + item.ID;
                                adoCon.EXP_MEST_PARENT = item.TDL_PATIENT_NAME;
                                adoCon.TDL_TREATMENT_CODE = item.TDL_TREATMENT_CODE;
                                adoCon.TDL_PATIENT_CODE = item.TDL_PATIENT_CODE;
                                adoCon.EXP_MEST_CODE = item.EXP_MEST_CODE;
                                adoCon.EXP_MEST_STT_ID = item.EXP_MEST_STT_ID;
                                adoCon.ID = item.ID;
                                adoCon.CHECK = true;
                                expMestApproveADO.Add(adoCon);
                            }
                        }
                    }
                    // Inventec.Common.Logging.LogSystem.Error("Kết thúc");
                    expMestApproveADO = expMestApproveADO.OrderBy(o => o.PARENT_ID__IN_SETY).ThenByDescending(o => o.TDL_PATIENT_NAME).ToList();
                    BindingList<ExpMestApproveADO> approve = new BindingList<ExpMestApproveADO>(expMestApproveADO);
                    treeListAggrApprove.DataSource = approve;
                    treeListAggrApprove.ExpandAll();
                    treeSereServ_CheckAllNode(treeListAggrApprove.Nodes);
                    //Inventec.Common.Logging.LogSystem.Error("bat đâu");
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HisExpMestAggrSDO sdo = new HisExpMestAggrSDO();
                List<ExpMestApproveADO> dataCheck = GetListCheck();
                dataCheck = dataCheck.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST).ToList();
                dataCheck = dataCheck.Where(o => o.ID != 0).ToList();
                if (dataCheck != null && dataCheck.Count > 0)
                {
                    sdo.ExpMestIds = dataCheck.Select(o => o.ID).ToList();
                    sdo.ReqRoomId = this.requestRoomId;
                }

                curentExpMest = new List<HIS_EXP_MEST>();
                if (sdo.ExpMestIds == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không chọn phiếu duyệt", "Thông báo");
                    return;
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_EXP_MEST>>(
                    "api/HisExpMest/AggrApprove", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    LoadDataToTreeListAggrApprove();
                    curentExpMest = rs;
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Phím tắt
        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void treeListAggrApprove_Click(object sender, EventArgs e)
        {
            try
            {
                var data = treeListAggrApprove.GetDataRecordByNode(treeListAggrApprove.FocusedNode);
                CommonParam param = new CommonParam();

                ExpMestApproveADO rowData = data as ExpMestApproveADO;
                if (rowData != null && rowData.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && rowData.PARENT_ID__IN_SETY != null && rowData.EXP_MEST_IDs != null)
                {
                    mediMateTypeADO = new List<MediMateTypeADO>();
                    //thuốc đã duyệt
                    HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
                    expMestMedicineFilter.PRESCRIPTION_IDs = rowData.EXP_MEST_IDs;
                    var expMestMedicines = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                    foreach (var item in expMestMedicines)
                    {
                        MediMateTypeADO medicineADO = new MediMateTypeADO();
                        var rs = listMediTypeInStock.FirstOrDefault(p => p.Id == item.TDL_MEDICINE_TYPE_ID);
                        if (rs != null)
                        {
                            medicineADO.MEDI_MATE_TYPE_ID = rs.Id;
                            medicineADO.MEDI_MATE_TYPE_CODE = rs.MedicineTypeCode;
                            medicineADO.MEDI_MATE_TYPE_NAME = rs.MedicineTypeName;
                            medicineADO.SERVICE_UNIT_NAME = rs.ServiceUnitName;
                            medicineADO.YCD_AMOUNT = item.AMOUNT;
                        }
                        mediMateTypeADO.Add(medicineADO);
                    }

                    //vật tư đã duyệt
                    HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                    expMestMaterialFilter.PRESCRIPTION_IDs = rowData.EXP_MEST_IDs;
                    var expMestMaterials = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                    foreach (var item in expMestMaterials)
                    {
                        MediMateTypeADO materialADO = new MediMateTypeADO();
                        var rs = listMateTypeInStock.FirstOrDefault(p => p.Id == item.TDL_MATERIAL_TYPE_ID);
                        if (rs != null)
                        {
                            materialADO.MEDI_MATE_TYPE_ID = rs.Id;
                            materialADO.MEDI_MATE_TYPE_CODE = rs.MaterialTypeCode;
                            materialADO.MEDI_MATE_TYPE_NAME = rs.MaterialTypeName;
                            materialADO.SERVICE_UNIT_NAME = rs.ServiceUnitName;
                            materialADO.YCD_AMOUNT = item.AMOUNT;
                        }
                        mediMateTypeADO.Add(materialADO);
                    }

                    gridControlApprove.DataSource = mediMateTypeADO;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
