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
using DevExpress.Data;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;

namespace HIS.Desktop.Plugins.ApproveAggrExpMest.ApproveAggrExpMest
{
    public partial class frmApproveAggrExpMest : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        long ExpMestId;
        long ExpMestStt;
        MOS.EFMODEL.DataModels.V_HIS_AGGR_EXP_MEST AggrExpMest;
        List<MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_3> prescriptions;
        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines;
        List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials;
        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineInPrescriptions;
        List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialInPrescriptions;
        List<MOS.EFMODEL.DataModels.V_HIS_ROOM> RoomDTO2s = new List<MOS.EFMODEL.DataModels.V_HIS_ROOM>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        DelegateSelectData delegateSelectData = null;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        #endregion

        #region Construct
        public frmApproveAggrExpMest(long _expMestId, Inventec.Desktop.Common.Modules.Module moduleData, DelegateSelectData _delegateSelectData, long _ExpMestStt)
		:base(moduleData)
        {
            try
            {
                InitializeComponent();
                ExpMestId = _expMestId;
                delegateSelectData = _delegateSelectData;
                this.ExpMestStt = _ExpMestStt;
                this.moduleData = moduleData;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmBill_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadDataToControl();
                SetCaptionByLanguageKey();
                SetIcon();
                //xtraTabControlExpMest.SelectedTabPageIndex = 0;
                //xtraTabControlDetailPrescription.SelectedTabPageIndex = 0;
                medistock = Base.GlobalStore.ListMediStock.FirstOrDefault(o => o.ROOM_ID == this.moduleData.RoomId && o.ROOM_TYPE_ID == this.moduleData.RoomTypeId);
                if (medistock == null)
                {
                    btnApproval.Enabled = false;
                    btnExport.Enabled = false;
                }
                else
                {
                    if (AggrExpMest.MEDI_STOCK_ID != medistock.ID)
                    {
                        btnApproval.Enabled = false;
                        btnExport.Enabled = false;
                    }
                    else
                    {
                        if (this.ExpMestStt == HisExpMestSttCFG.HisExpMestSttId__Request)
                        {
                            btnApproval.Enabled = true;
                            btnExport.Enabled = false;
                        }
                        else if (this.ExpMestStt == HisExpMestSttCFG.HisExpMestSttId__Approved)
                        {
                            btnApproval.Enabled = false;
                            btnExport.Enabled = true;
                        }
                        else
                        {
                            btnApproval.Enabled = false;
                            btnExport.Enabled = false;
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region private function

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToControl()
        {
            CommonParam param = new CommonParam();
            try
            {
                // lấy aggr_exp_mest từ exp_mest
                MOS.Filter.HisAggrExpMestViewFilter aggrExpMestFilter = new HisAggrExpMestViewFilter();
                aggrExpMestFilter.EXP_MEST_ID = ExpMestId;
                AggrExpMest = new BackendAdapter(param).Get<List<V_HIS_AGGR_EXP_MEST>>(HisRequestUriStore.HIS_AGGR_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, aggrExpMestFilter, param).FirstOrDefault();
                // lấy exp_mest từ aggr_exp_mest
                if (AggrExpMest != null)
                {
                    MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                    expMestFilter.AGGR_EXP_MEST_ID = AggrExpMest.ID;
                    var expMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                    // lấy các prescription theo expMests
                    if (expMests != null && expMests.Count > 0)
                    {
                        // get prescriptions
                        MOS.Filter.HisPrescriptionView3Filter prescriptionFilter = new HisPrescriptionView3Filter();
                        prescriptionFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                        prescriptions = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_3>>(HisRequestUriStore.HIS_PRESCRIPTION_GETVIEW, ApiConsumers.MosConsumer, prescriptionFilter, param);

                        // get chmsExpMest
                        MOS.Filter.HisChmsExpMestView1Filter chmsExpMestViewFilter = new HisChmsExpMestView1Filter();
                        chmsExpMestViewFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                        var chmsExpMests = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_CHMS_EXP_MEST_1>>(HisRequestUriStore.HIS_CHMS_EXP_MEST_1_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, chmsExpMestViewFilter, param);
                        foreach (var item in chmsExpMests)
                        {
                            MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_3 prescription1 = new V_HIS_PRESCRIPTION_3();
                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_CHMS_EXP_MEST_1, V_HIS_PRESCRIPTION_3>();
                            prescription1 = AutoMapper.Mapper.Map<V_HIS_PRESCRIPTION_3>(item);
                            prescriptions.Add(prescription1);
                        }

                        gridControlPrescription.DataSource = prescriptions;

                        var prescriptionFocus = (V_HIS_PRESCRIPTION_3)gridViewPrescription.GetFocusedRow();
                        if (prescriptionFocus != null)
                        {
                            LoadDataToPrescriptionCommonInfo(prescriptionFocus);
                            LoadDataToGridMedicineMaterialInPrescription(prescriptionFocus);

                        }
                    }
                    LoadDataToCommonInfo(this.ExpMestId);
                    LoadDataToGridMedicine(AggrExpMest);
                    LoadDataToGridMaterial(AggrExpMest);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCommonInfo(long expMestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = expMestId;
                var expMests = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, expMestFilter, param);
                if (expMests != null && expMests.Count > 0)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST expMest = expMests.FirstOrDefault();
                    lblUserTimeCommon.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(expMest.USE_TIME ?? 0);
                    lblCreator.Text = expMest.CREATOR;
                    lblCreateTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(expMest.CREATE_TIME ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToPrescriptionCommonInfo(V_HIS_PRESCRIPTION_3 prescription)
        {
            try
            {
                if (prescription != null)
                {
                    lblInstructionTimePrescription.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(prescription.INTRUCTION_TIME);
                    // get icd
                    MOS.Filter.HisIcdFilter icdFilter = new HisIcdFilter();
                    icdFilter.ID = prescription.ICD_ID;
                    var icds = new BackendAdapter(new CommonParam()).Get<List<HIS_ICD>>("api/HisIcd/Get", ApiConsumer.ApiConsumers.MosConsumer, icdFilter, new CommonParam());
                    MOS.EFMODEL.DataModels.HIS_ICD icd = new HIS_ICD();
                    if (icds != null && icds.Count > 0)
                    {
                        icd = icds.FirstOrDefault();
                        if (!String.IsNullOrEmpty(prescription.ICD_MAIN_TEXT))
                        {
                            lblIcd.Text = icd.ICD_CODE + " - " + prescription.ICD_MAIN_TEXT;
                        }
                        else
                        {
                            lblIcd.Text = icd.ICD_CODE + " - " + icd.ICD_NAME;
                        }
                    }
                   
                    lblReqUserName.Text = prescription.REQ_LOGINNAME + " - " + prescription.REQ_USERNAME;
                    lblUserTimePrescription.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(prescription.USE_TIME ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(prescription.USE_TIME_TO ?? 0);
                    lblAdvise.Text = prescription.ADVISE;
                }
                else
                {
                    ResetPrecriptionCommonInfo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetPrecriptionCommonInfo()
        {
            try
            {
                lblInstructionTimePrescription.Text = "";
                lblIcd.Text = "";
                lblReqUserName.Text = "";
                lblUserTimePrescription.Text = "";
                lblAdvise.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisiblePriorityTab()
        {
            try
            {
                if (gridControlApprovalMedcineInPrescription.DataSource != null && (gridControlApprovalMedcineInPrescription.DataSource as List<V_HIS_EXP_MEST_MEDICINE>).Count > 0)
                {
                    xtraTabControlDetailPrescription.SelectedTabPageIndex = 1;
                }
                else
                {
                    xtraTabControlDetailPrescription.SelectedTabPageIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE> GroupExpMestMedicineByIdAndPrice(List<V_HIS_EXP_MEST_MEDICINE> expMestmedicines)
        {
            try
            {
                //Duyet danh sach gom nhom de binding datasource
                List<V_HIS_EXP_MEST_MEDICINE> dataSourceExpMestMedicine1InPrescriptionsGroup = new List<V_HIS_EXP_MEST_MEDICINE>();

                if (expMestmedicines != null && expMestmedicines.Count > 0)
                {
                    //gom nhom theo loai thuoc, gia
                    var expMestMedicineInPrescriptionsGroup = expMestmedicines.GroupBy(o => new
                    {
                        o.MEDICINE_TYPE_ID,
                        o.IMP_PRICE,
                        o.IMP_VAT_RATIO,
                        o.INTERNAL_PRICE,
                        o.PRICE,
                        o.VAT_RATIO,
                        o.SERVICE_UNIT_ID,
                        o.IN_EXECUTE,
                        o.IN_REQUEST
                    });

                    Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, V_HIS_EXP_MEST_MEDICINE>();


                    foreach (var t in expMestMedicineInPrescriptionsGroup)
                    {
                        //clone de ko thay doi so luong cua du lieu ban dau
                        V_HIS_EXP_MEST_MEDICINE x = Mapper.Map<V_HIS_EXP_MEST_MEDICINE>(t.First());
                        x.AMOUNT = t.Sum(o => o.AMOUNT);
                        dataSourceExpMestMedicine1InPrescriptionsGroup.Add(x);
                    }
                }
                return dataSourceExpMestMedicine1InPrescriptionsGroup;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL> GroupExpMestMaterialByIdAndPrice(List<V_HIS_EXP_MEST_MATERIAL> expMestMATERIALs)
        {
            try
            {
                //Duyet danh sach gom nhom de binding datasource
                List<V_HIS_EXP_MEST_MATERIAL> dataSourceExpMestMATERIAL1InPrescriptionsGroup = new List<V_HIS_EXP_MEST_MATERIAL>();
                if (expMestMATERIALs != null && expMestMATERIALs.Count > 0)
                {
                    //gom nhom theo loai thuoc, gia
                    var expMestMATERIALInPrescriptionsGroup = expMestMATERIALs.GroupBy(o => new
                    {
                        o.MATERIAL_TYPE_ID,
                        o.IMP_PRICE,
                        o.IMP_VAT_RATIO,
                        o.INTERNAL_PRICE,
                        o.PRICE,
                        o.VAT_RATIO,
                        o.SERVICE_UNIT_ID,
                        o.IN_EXECUTE,
                        o.IN_REQUEST
                    });

                    Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, V_HIS_EXP_MEST_MATERIAL>();


                    foreach (var t in expMestMATERIALInPrescriptionsGroup)
                    {
                        //clone de ko thay doi so luong cua du lieu ban dau
                        V_HIS_EXP_MEST_MATERIAL x = Mapper.Map<V_HIS_EXP_MEST_MATERIAL>(t.First());
                        x.AMOUNT = t.Sum(o => o.AMOUNT);
                        dataSourceExpMestMATERIAL1InPrescriptionsGroup.Add(x);
                    }
                }
                return dataSourceExpMestMATERIAL1InPrescriptionsGroup;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private void LoadDataToGridMedicineMaterialInPrescription(V_HIS_PRESCRIPTION_3 prescription)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (prescription != null)
                {
                    // thuoc
                    MOS.Filter.HisExpMestMedicineViewFilter expMestmedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestmedicineFilter.EXP_MEST_ID = prescription.EXP_MEST_ID;
                    expMestMedicineInPrescriptions = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, expMestmedicineFilter, param);

                    // vat tu
                    MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.EXP_MEST_ID = prescription.EXP_MEST_ID;
                    expMestMaterialInPrescriptions = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                    gridControlRequestMedcineInPrescription.DataSource = null;
                    gridControlApprovalMedcineInPrescription.DataSource = null;
                    List<V_HIS_EXP_MEST_MEDICINE> dataSourceExpMestMedicine1InPrescriptionsGroup = new List<V_HIS_EXP_MEST_MEDICINE>();
                    List<V_HIS_EXP_MEST_MATERIAL> dataSourceExpMestMaterialInPrescriptionsGroup = new List<V_HIS_EXP_MEST_MATERIAL>();
                    dataSourceExpMestMedicine1InPrescriptionsGroup = GroupExpMestMedicineByIdAndPrice(expMestMedicineInPrescriptions);
                    dataSourceExpMestMaterialInPrescriptionsGroup = GroupExpMestMaterialByIdAndPrice(expMestMaterialInPrescriptions);

                    foreach (var item in dataSourceExpMestMaterialInPrescriptionsGroup)
                    {
                        V_HIS_EXP_MEST_MEDICINE expMestmedicine = new V_HIS_EXP_MEST_MEDICINE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(expMestmedicine, item);
                        expMestmedicine.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                        expMestmedicine.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                        dataSourceExpMestMedicine1InPrescriptionsGroup.Add(expMestmedicine);
                    }

                    if (dataSourceExpMestMedicine1InPrescriptionsGroup != null && dataSourceExpMestMedicine1InPrescriptionsGroup.Count > 0)
                    {
                        // yêu cầu 
                        var requestExpMestMedicineInPrescriptions = dataSourceExpMestMedicine1InPrescriptionsGroup.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                        gridControlRequestMedcineInPrescription.DataSource = requestExpMestMedicineInPrescriptions;

                        // duyệt
                        var executeExpMestMedicineInPrescriptions = dataSourceExpMestMedicine1InPrescriptionsGroup.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                        gridControlApprovalMedcineInPrescription.DataSource = executeExpMestMedicineInPrescriptions;

                        //show tab
                        if (executeExpMestMedicineInPrescriptions != null && executeExpMestMedicineInPrescriptions.Count > 0)
                        {
                            xtraTabControlDetailPrescription.SelectedTabPageIndex = 1;
                        }
                        else if (requestExpMestMedicineInPrescriptions != null && requestExpMestMedicineInPrescriptions.Count > 0)
                        {
                            xtraTabControlDetailPrescription.SelectedTabPageIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridMedicine(V_HIS_AGGR_EXP_MEST aggrExpMest)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (aggrExpMest != null)
                {
                    //MOS.Filter.HisMedicineTypeStockViewFilter filter = new MOS.Filter.HisMedicineTypeStockViewFilter();
                    //filter.MEDI_STOCK_ID = ExpMest.MEDI_STOCK_ID;
                    //var medicineTYpeInStocks = new BackendAdapter(param).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_GETINSTOCKMEDICINETYPE, ApiConsumers.MosConsumer, filter, param);

                    //MOS.Filter.HisExpMestMedicineViewFilter expMestmedicineFilter = new HisExpMestMedicineViewFilter();
                    //expMestmedicineFilter.AGGR_EXP_MEST_ID = aggrExpMest.ID;
                    //List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE> dataMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, expMestmedicineFilter, param);

                    //List<ExpMestMedicineSDODetail> expMestMedicineSdoDetails = new List<ExpMestMedicineSDODetail>();
                    //foreach (var dataMedicine in dataMedicines)
                    //{
                    //    ExpMestMedicineSDODetail medicineSdo = new ExpMestMedicineSDODetail(dataMedicine);
                    //    var medicineTypeInStockExist = medicineTYpeInStocks.FirstOrDefault(o => o.MedicineTypeCode == dataMedicine.MEDICINE_TYPE_CODE);
                    //    if (medicineTypeInStockExist != null)
                    //    {
                    //        medicineSdo.TotalAmount = medicineTypeInStockExist.TotalAmount;
                    //    }
                    //    expMestMedicineSdoDetails.Add(medicineSdo);
                    //}
                    //if (expMestMedicineSdoDetails != null && expMestMedicineSdoDetails.Count > 0)
                    //{
                    //    expMestMedicineSdoDetails = expMestMedicineSdoDetails.OrderByDescending(o => o.NUM_ORDER).ToList();
                    //}

                    MOS.Filter.HisExpMestMedicineViewFilter expMestmedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestmedicineFilter.AGGR_EXP_MEST_ID = aggrExpMest.ID;
                    expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, expMestmedicineFilter, param);

                    List<V_HIS_EXP_MEST_MEDICINE> dataSourceExpMestMedicine1InPrescriptionsGroup = new List<V_HIS_EXP_MEST_MEDICINE>();
                    dataSourceExpMestMedicine1InPrescriptionsGroup = GroupExpMestMedicineByIdAndPrice(expMestMedicines);

                    if (dataSourceExpMestMedicine1InPrescriptionsGroup != null && dataSourceExpMestMedicine1InPrescriptionsGroup.Count > 0)
                    {
                        var requestExpMestMedicines = dataSourceExpMestMedicine1InPrescriptionsGroup.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                        gridControlRequestMedicine.DataSource = requestExpMestMedicines;

                        var executeExpMestMedicines = dataSourceExpMestMedicine1InPrescriptionsGroup.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                        gridControlApprovalMedicine.DataSource = executeExpMestMedicines;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridMaterial(V_HIS_AGGR_EXP_MEST aggrExpMest)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (aggrExpMest != null)
                {
                    //MOS.Filter.HisMaterialTypeStockViewFilter filter = new MOS.Filter.HisMaterialTypeStockViewFilter();
                    //filter.MEDI_STOCK_ID = ExpMest.MEDI_STOCK_ID;
                    //var materialTypeInStocks = new BackendAdapter(param).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GETINSTOCKMATERIALTYPE, ApiConsumers.MosConsumer, filter, param);

                    //List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL> dataMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEWANDINCLUDECHIILDRENBYEXPMESTID, ApiConsumers.MosConsumer, this.ExpMest.ID, param);

                    //List<ExpMestMaterialSDODetail> expMestMaterialSdoDetails = new List<ExpMestMaterialSDODetail>();
                    //foreach (var dataMaterial in dataMaterials)
                    //{
                    //    ExpMestMaterialSDODetail materialSdo = new ExpMestMaterialSDODetail(dataMaterial);
                    //    var materialTypeInStockExist = materialTypeInStocks.FirstOrDefault(o => o.MaterialTypeCode == dataMaterial.MATERIAL_TYPE_CODE);
                    //    if (materialTypeInStockExist != null)
                    //    {
                    //        materialSdo.TotalAmount = materialTypeInStockExist.TotalAmount;
                    //    }
                    //    expMestMaterialSdoDetails.Add(materialSdo);
                    //}
                    //if (expMestMaterialSdoDetails != null && expMestMaterialSdoDetails.Count > 0)
                    //{
                    //    expMestMaterialSdoDetails = expMestMaterialSdoDetails.OrderByDescending(o => o.NUM_ORDER).ToList();
                    //}
                    MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.AGGR_EXP_MEST_ID = aggrExpMest.ID;
                    expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, expMestMaterialFilter, param);
                    List<V_HIS_EXP_MEST_MATERIAL> dataSourceExpMestMaterialInPrescriptionsGroup = new List<V_HIS_EXP_MEST_MATERIAL>();
                    dataSourceExpMestMaterialInPrescriptionsGroup = GroupExpMestMaterialByIdAndPrice(expMestMaterials);
                    if (dataSourceExpMestMaterialInPrescriptionsGroup != null && dataSourceExpMestMaterialInPrescriptionsGroup.Count > 0)
                    {
                        var requestExpMestMaterials = dataSourceExpMestMaterialInPrescriptionsGroup.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MATERIAL.IN_REQUEST__TRUE).ToList();
                        gridControRequestlMaterial.DataSource = requestExpMestMaterials;

                        var executeExpMestMaterials = dataSourceExpMestMaterialInPrescriptionsGroup.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MATERIAL.IN_EXECUTE__TRUE).ToList();
                        gridControlApprovalMaterial.DataSource = executeExpMestMaterials;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region public function
        #endregion

        #region Event handler
        private void gridViewMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_EXP_MEST_MEDICINE dataRow = (V_HIS_EXP_MEST_MEDICINE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "IS_EXPEND_DISPLAY")
                        {
                            if (dataRow.IS_EXPEND == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IS_EXPEND__TRUE)
                            {
                                e.Value = imageCollection1.Images[0];
                            }
                        }

                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {

                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_EXP_MEST_MATERIAL dataRow = (V_HIS_EXP_MEST_MATERIAL)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "IS_EXPEND_DISPLAY")
                        {
                            if (dataRow.IS_EXPEND == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MATERIAL.IS_EXPEND__TRUE)
                            {
                                e.Value = imageCollection1.Images[0];
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.EXPIRED_DATE ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPrescription_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var prescription = (V_HIS_PRESCRIPTION_3)gridViewPrescription.GetFocusedRow();
                LoadDataToGridMedicineMaterialInPrescription(prescription);
                LoadDataToPrescriptionCommonInfo(prescription);
                VisiblePriorityTab();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEdit_Remove_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                WaitingManager.Show();
                var vPrescription = (MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_3)gridViewPrescription.GetFocusedRow();
                success = new Inventec.Common.Adapter.BackendAdapter(param).Post<Boolean>(HisRequestUriStore.HIS_EXPMEST_REMOVEAGGR, ApiConsumers.MosConsumer, vPrescription.EXP_MEST_ID, param);
                if (success)
                {
                    prescriptions.Remove(vPrescription);
                    gridControlPrescription.DataSource = null;
                    gridControlPrescription.DataSource = prescriptions;

                    var precriptionFocus = gridViewPrescription.GetFocusedRow();
                    if (precriptionFocus != null && precriptionFocus is MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_3)
                    {
                        LoadDataToPrescriptionCommonInfo((MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_3)precriptionFocus);
                        LoadDataToGridMedicineMaterialInPrescription((MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_3)precriptionFocus);
                        // remove trong gridControl medicine
                        for (int i = this.expMestMedicines.Count - 1; i > 0; i--)
                        {
                            if (this.expMestMedicines[i].EXP_MEST_ID == vPrescription.EXP_MEST_ID)
                            {
                                this.expMestMedicines.RemoveAt(i);
                            }
                        }

                        // remove trong gridControl material
                        for (int i = this.expMestMaterials.Count - 1; i > 0; i--)
                        {
                            if (this.expMestMaterials[i].EXP_MEST_ID == vPrescription.EXP_MEST_ID)
                            {
                                this.expMestMaterials.RemoveAt(i);
                            }
                        }

                        gridControlRequestMedicine.DataSource = null;
                        gridControlRequestMedicine.DataSource = this.expMestMedicines;
                        gridControRequestlMaterial.DataSource = this.expMestMaterials;
                    }
                    else
                    {
                        ResetPrecriptionCommonInfo();
                        this.expMestMaterials = null;
                        this.expMestMedicines = null;
                        this.expMestMedicineInPrescriptions = null;
                        gridControRequestlMaterial.DataSource = null;
                        gridControlRequestMedicine.DataSource = null;
                        gridControlRequestMedcineInPrescription.DataSource = null;
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPrescription_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_3 data = (MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_3)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMedcineInPrescription_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE")
                        {
                            try
                            {
                                decimal price = data.PRICE ?? 0;
                                decimal amount = data.AMOUNT;
                                decimal vatRatio = (data.VAT_RATIO ?? 0);
                                decimal disCount = data.DISCOUNT ?? 0;
                                decimal valueTotal = (price * amount * (1 + vatRatio)) - disCount;
                                e.Value = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(valueTotal);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TOTAL_PRICE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong IMP_VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((data.VAT_RATIO ?? 0) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintAggregateExpMest(this.AggrExpMest);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewApprovalMedcineInPrescription_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE")
                        {
                            try
                            {
                                decimal price = data.PRICE ?? 0;
                                decimal amount = data.AMOUNT;
                                decimal vatRatio = (data.VAT_RATIO ?? 0);
                                decimal disCount = data.DISCOUNT ?? 0;
                                decimal valueTotal = (price * amount * (1 + vatRatio)) - disCount;
                                e.Value = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(valueTotal);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TOTAL_PRICE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong IMP_VAT_RATIO", ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXP_VAT_RATIO_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Get.RoundCurrency((data.VAT_RATIO ?? 0) * 100, 2);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewApprvovalMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_EXP_MEST_MEDICINE dataRow = (V_HIS_EXP_MEST_MEDICINE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "IS_EXPEND_DISPLAY")
                        {
                            if (dataRow.IS_EXPEND == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IS_EXPEND__TRUE)
                            {
                                e.Value = imageCollection1.Images[0];
                            }
                        }

                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_RATIO", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewApprovalMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {

                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_EXP_MEST_MATERIAL dataRow = (V_HIS_EXP_MEST_MATERIAL)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "IS_EXPEND_DISPLAY")
                        {
                            if (dataRow.IS_EXPEND == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MATERIAL.IS_EXPEND__TRUE)
                            {
                                e.Value = imageCollection1.Images[0];
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.EXPIRED_DATE ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnApproval_Click(object sender, EventArgs e)
        {
            try
            {
                //if (this.expMest == null)
                //{
                //    return;
                //}
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.ID = this.ExpMestId;
                var ExpmestResult = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>(RequestUriStore.HIS_EXP_MEST_Get, ApiConsumers.MosConsumer, expMestFilter, param);
                if (ExpmestResult != null && ExpmestResult.Count > 0)
                {
                    data = ExpmestResult.FirstOrDefault();
                    //AggrExpMest = expMests.FirstOrDefault();
                    //MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();

                    //Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, this.AggrExpMest);
                    data.EXP_MEST_STT_ID = HIS.Desktop.LocalStorage.SdaConfigKey.Config.HisExpMestSttCFG.HisExpMestSttId__Approved;
                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_UPDATE_STATUS, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    if (apiresul != null && apiresul.EXP_MEST_STT_ID == HIS.Desktop.LocalStorage.SdaConfigKey.Config.HisExpMestSttCFG.HisExpMestSttId__Approved)
                    //if(apiresul !=null)
                    {
                        success = true;
                        //FillDataToGrid();TODO
                        //ExpMestStt = apiresul.EXP_MEST_STT_ID;
                        this.AggrExpMest.MEDI_STOCK_ID = apiresul.MEDI_STOCK_ID;
                        //EnableBottomButton(ExpMestStt, this.expMest.MEDI_STOCK_ID, this.currentMedistockId);

                        LoadDataToGridMedicine(AggrExpMest);
                        LoadDataToGridMaterial(AggrExpMest);
                        LoadDataToControl();
                        delegateSelectData(apiresul);
                        //if (this.refeshReference != null)
                        //{
                        //    this.refeshReference();
                        //}
                    }
                }

                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void refreshData(object data)
        {
            try
            {
                if (data is HisAggrExpMestSDO)
                {
                    var expMestApprove = (MOS.SDO.HisAggrExpMestSDO)data;
                    //EnableBottomButton(expMestApprove.ExpMestIds, expMestApprove.RequestRoomId);
                }
                delegateSelectData(new HIS_EXP_MEST());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.ID = this.ExpMestId;
                var expMests = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>(RequestUriStore.HIS_EXP_MEST_Get, ApiConsumers.MosConsumer, expMestFilter, param);
                if (expMests != null && expMests.Count > 0)
                {
                    data = expMests.FirstOrDefault();
                }

                if (data != null)
                {
                    WaitingManager.Show();
                    //MOS.EFMODEL.DataModels.HIS_EXP_MEST data = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                    //Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(data, this.expMest);
                    HIS_EXP_MEST apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                        (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, data, param);
                    if (apiresult != null)
                    {
                        success = true;
                        //FillDataToGrid();TODO
                        //EnableBottomButton(apiresult.EXP_MEST_STT_ID, apiresult.MEDI_STOCK_ID, this.currentMedistockId);
                        delegateSelectData(apiresult);
                        btnExport.Enabled = false;
                        this.Close();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }

                //else if (this.expMest.BLOOD_TYPE_COUNT > 0)
                //{
                //    WaitingManager.Show();

                //    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExportBlood").FirstOrDefault();
                //    if (moduleData == null)
                //    {
                //        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ExportBlood");
                //        MessageManager.Show(ResourceLanguageManager.TaiKhoanKhongCoQuyenThucHienChucNang);
                //    }
                //    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                //    {

                //        List<object> listArgs = new List<object>();
                //        listArgs.Add(this.expMest.ID);
                //        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                //        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                //        WaitingManager.Hide();
                //        ((Form)extenceInstance).ShowDialog();
                //    }
                //    else
                //    {
                //        MessageManager.Show(ResourceLanguageManager.TaiKhoanKhongCoQuyenThucHienChucNang);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void xtraTabControlExpMest_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (xtraTabControlExpMest.SelectedTabPageIndex == 0)
                {
                    btnApproval.Enabled = true;
                    btnExport.Enabled = false;
                }
                else if (xtraTabControlExpMest.SelectedTabPageIndex == 1)
                {
                    btnApproval.Enabled = true;
                    btnExport.Enabled = false;
                }
                else if (xtraTabControlExpMest.SelectedTabPageIndex == 2)
                {
                    btnApproval.Enabled = false;
                    btnExport.Enabled = true;
                }
                else if (xtraTabControlExpMest.SelectedTabPageIndex == 3)
                {
                    btnApproval.Enabled = true;
                    btnExport.Enabled = false;
                }
                else if (xtraTabControlExpMest.SelectedTabPageIndex == 4)
                {
                    btnApproval.Enabled = false;
                    btnExport.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void xtraTabControlDetailPrescription_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (xtraTabControlDetailPrescription.SelectedTabPageIndex == 0)
                {
                    btnApproval.Enabled = true;
                    btnExport.Enabled = false;
                }
                else if (xtraTabControlDetailPrescription.SelectedTabPageIndex == 1)
                {
                    btnApproval.Enabled = false;
                    btnExport.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
