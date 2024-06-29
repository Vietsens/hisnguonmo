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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
//using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using HIS.Desktop.Utility;
using DevExpress.XtraLayout;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.ServiceReqPatient.ADO;
using DevExpress.XtraGrid.Views.Grid;
using System.IO;
namespace HIS.Desktop.Plugins.ServiceReqPatient
{
    public partial class ServiceReqPatientForm : FormBase
    {
        #region Declare
        int STT = 1;
        string DeparmentName;
        List<HIS_SERVICE_REQ> HisServiceReqList = new List<HIS_SERVICE_REQ>();
        List<ServiceReqPatientADO> ServiceReqPatientADOList = new List<ServiceReqPatientADO>();
        List<ServiceReqPatientADO> CurrentDataADOs = new List<ServiceReqPatientADO>();
        List<NgayThucHien> CreateListDate = new List<NgayThucHien>();
        long? TreatmentID;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<long> ListMobaExpMestIds = new List<long>();

        List<HIS_IMP_MEST_MEDICINE> ListMobaImpMestMedicine = new List<HIS_IMP_MEST_MEDICINE>();
        List<HIS_IMP_MEST_MATERIAL> ListMobaImpMestMaterial = new List<HIS_IMP_MEST_MATERIAL>();
        HIS_TREATMENT curentTreatment;
        #endregion

        #region construct
        public ServiceReqPatientForm(Inventec.Desktop.Common.Modules.Module module, long? _TreatmentId, string _DeparmentName)
            : base(module)
        {
            try
            {
                InitializeComponent();
                currentModule = module;
                this.TreatmentID = _TreatmentId;
                this.DeparmentName = _DeparmentName;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public ServiceReqPatientForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {

            try
            {
                InitializeComponent();
                //pagingGrid = new PagingGrid();
                currentModule = module;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method

        private void ServiceReqPatientForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (currentModule != null)
                {
                    this.Text = currentModule.text;
                }
                MeShow();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void SetDefaultValue()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = TreatmentID;
                var lstTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param);
                if (lstTreatment != null && lstTreatment.Count > 0)
                {
                    var _Treatment = lstTreatment.FirstOrDefault();
                    curentTreatment = lstTreatment.FirstOrDefault();
                    txtName.Text = _Treatment.TDL_PATIENT_NAME;
                    txtDBirth.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_Treatment.TDL_PATIENT_DOB) + " (" + CaculatorAge(_Treatment.TDL_PATIENT_DOB) + ")";
                    txtGender.Text = _Treatment.TDL_PATIENT_GENDER_NAME;
                    txtBHYT.Text = _Treatment.TDL_HEIN_CARD_NUMBER;
                    CommonParam param2 = new CommonParam();
                    MOS.Filter.HisPatientTypeAlterFilter PatientTypeAlterFilter = new HisPatientTypeAlterFilter();
                    PatientTypeAlterFilter.TREATMENT_ID = TreatmentID;
                    var PatientTypeAlter = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, PatientTypeAlterFilter, param2).FirstOrDefault();
                    if (PatientTypeAlter.HEIN_CARD_FROM_TIME != null)
                    {
                        txtFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatientTypeAlter.HEIN_CARD_TO_TIME ?? 0); ;
                    }
                    txtDeparment.Text = DeparmentName;
                    txtICDMain.Text = _Treatment.ICD_CODE +" - "+_Treatment.ICD_NAME;
                    txtICD.Text = _Treatment.ICD_TEXT;
                    txtWeight.Text = CaculatorWeight() + "kg";
                    txtNVV.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_Treatment.IN_TIME);

                    HisTreatmentBedRoomViewFilter bedRoomViewFilter = new HisTreatmentBedRoomViewFilter();
                    bedRoomViewFilter.TREATMENT_ID = _Treatment.ID;

                    var bedroom = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, bedRoomViewFilter, param);
                    if (bedroom != null && bedroom.Count > 0)
                    {
                        this.HisTreatmentBedRoom = bedroom.FirstOrDefault();
                    }

                    if (this.HisTreatmentBedRoom != null)
                    {
                        txtBedName.Text = this.HisTreatmentBedRoom.BED_NAME;
                        txtBedRoom.Text = this.HisTreatmentBedRoom.BED_ROOM_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                // btnEdit.Enabled = (action == GlobalVariables.ActionEdit);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FillDatagctFormList()
        {
            try
            {
                WaitingManager.Show();
                LoadServiceReq();
                //LoadExpMestMedicine();
                //LoadExpMestMaterial();

                LoadExpMestMedicineMaterialV2();

                if (ListMobaExpMestIds != null && ListMobaExpMestIds.Count > 0)
                {
                    ListMobaExpMestIds = ListMobaExpMestIds.Distinct().ToList();
                    GetImpMestByMobaExpMestId(ListMobaExpMestIds);
                }

                LoadComboMediStock();

                CreateGridByDataNew();

                #region ---- Code Cu ----
                //for (int i = 0; i < ServiceReqPatientADOList.Count - 1; i++)
                //{
                //    for (int j = i + 1; j < ServiceReqPatientADOList.Count; j++)
                //    {
                //        if (((ServiceReqPatientADOList[i].MATERIAL_TYPE_ID == ServiceReqPatientADOList[j].MATERIAL_TYPE_ID)
                //            && ServiceReqPatientADOList[i].MATERIAL_TYPE_ID != null) || ((ServiceReqPatientADOList[i].MEDICINE_TYPE_ID == ServiceReqPatientADOList[j].MEDICINE_TYPE_ID)
                //            && ServiceReqPatientADOList[i].MEDICINE_TYPE_ID != 0))
                //        {
                //            if (ServiceReqPatientADOList[i].AmountDate.IS_export == ServiceReqPatientADOList[j].AmountDate.IS_export)
                //            {
                //                if (ServiceReqPatientADOList[i].AmountDate.DATE == ServiceReqPatientADOList[j].AmountDate.DATE)
                //                {
                //                    foreach (var item in ServiceReqPatientADOList[i].AmountDateList)
                //                    {
                //                        if (item.DATE == ServiceReqPatientADOList[j].AmountDate.DATE)
                //                        {
                //                            item.Amount = item.Amount + ServiceReqPatientADOList[j].AmountDate.Amount;
                //                        }
                //                    }
                //                    ServiceReqPatientADOList[i].SUM = ServiceReqPatientADOList[i].SUM + ServiceReqPatientADOList[j].SUM;
                //                    ServiceReqPatientADOList.RemoveAt(j);
                //                    j--;
                //                }
                //                else
                //                {
                //                    ServiceReqPatientADOList[i].AmountDateList.Add(ServiceReqPatientADOList[j].AmountDate);
                //                    ServiceReqPatientADOList[i].SUM = ServiceReqPatientADOList[i].SUM + ServiceReqPatientADOList[j].SUM;
                //                    ServiceReqPatientADOList.RemoveAt(j);
                //                    j--;
                //                }
                //            }
                //            else
                //            {
                //                ServiceReqPatientADOList[i].AmountDateList.Add(ServiceReqPatientADOList[j].AmountDate);
                //                ServiceReqPatientADOList[i].SUM = ServiceReqPatientADOList[i].SUM + ServiceReqPatientADOList[j].SUM;
                //                ServiceReqPatientADOList.RemoveAt(j);
                //                j--;
                //            }
                //        }
                //    }
                //}

                //#region
                ////for (int i = 0; i < ServiceReqPatientADOList.Count; i++)
                ////{
                ////    for (int j = i + 1; j < ServiceReqPatientADOList.Count; j++)
                ////    {
                ////        if (((ServiceReqPatientADOList[i].MATERIAL_TYPE_ID == ServiceReqPatientADOList[j].MATERIAL_TYPE_ID) && ServiceReqPatientADOList[i].MATERIAL_TYPE_ID != null) || ((ServiceReqPatientADOList[i].MEDICINE_TYPE_ID == ServiceReqPatientADOList[j].MEDICINE_TYPE_ID) && ServiceReqPatientADOList[i].MEDICINE_TYPE_ID!=null))
                ////        {
                ////            if (ServiceReqPatientADOList[i].AmountDate.DATE == ServiceReqPatientADOList[j].AmountDate.DATE)
                ////            {
                ////                foreach (var item in ServiceReqPatientADOList[i].AmountDateList)
                ////                {
                ////                    if (item.DATE==ServiceReqPatientADOList[j].AmountDate.DATE)
                ////                    {
                ////                        item.Amount = item.Amount + ServiceReqPatientADOList[j].AmountDate.Amount;
                ////                    }
                ////                }
                ////                ServiceReqPatientADOList[i].SUM = ServiceReqPatientADOList[i].SUM + ServiceReqPatientADOList[j].SUM;
                ////                ServiceReqPatientADOList.RemoveAt(j);
                ////                j--;
                ////            }
                ////            else
                ////            {
                ////                ServiceReqPatientADOList[i].AmountDateList.Add(ServiceReqPatientADOList[j].AmountDate);
                ////                ServiceReqPatientADOList[i].SUM = ServiceReqPatientADOList[i].SUM + ServiceReqPatientADOList[j].SUM;
                ////                ServiceReqPatientADOList.RemoveAt(j);
                ////                j--;
                ////            }
                ////        }
                ////    }
                ////}
                //#endregion
                //CreateColumn();
                //ServiceReqPatientADOList = ServiceReqPatientADOList.OrderBy(o => o.Type)
                //   .ThenByDescending(o => o.IS_MADICINE).ToList();
                //gridControl1.DataSource = ServiceReqPatientADOList;
                #endregion

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GetImpMestByMobaExpMestId(List<long> listMobaExpMestIds)
        {
            try
            {
                if (listMobaExpMestIds != null && listMobaExpMestIds.Count > 0)
                {
                    var skip = 0;
                    while (listMobaExpMestIds.Count - skip > 0)
                    {
                        CommonParam param = new CommonParam();
                        var limit = listMobaExpMestIds.Skip(skip).Take(100).ToList();
                        skip += 100;
                        HisImpMestFilter filter = new HisImpMestFilter();
                        filter.MOBA_EXP_MEST_IDs = limit;
                        filter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                        var impMest = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, filter, param);
                        if (impMest != null && impMest.Count > 0)
                        {
                            GetImpMestMedicine(impMest);
                            GetImpMestMaterial(impMest);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetImpMestMaterial(List<HIS_IMP_MEST> impMest)
        {
            try
            {
                if (impMest != null && impMest.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisImpMestMaterialFilter filter = new HisImpMestMaterialFilter();
                    filter.IMP_MEST_IDs = impMest.Select(s => s.ID).ToList();
                    var material = new BackendAdapter(param).Get<List<HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/Get", ApiConsumers.MosConsumer, filter, param);
                    if (material != null && material.Count > 0)
                    {
                        ListMobaImpMestMaterial.AddRange(material);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetImpMestMedicine(List<HIS_IMP_MEST> impMest)
        {
            try
            {
                if (impMest != null && impMest.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisImpMestMedicineFilter filter = new HisImpMestMedicineFilter();
                    filter.IMP_MEST_IDs = impMest.Select(s => s.ID).ToList();
                    var medicines = new BackendAdapter(param).Get<List<HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/Get", ApiConsumers.MosConsumer, filter, param);
                    if (medicines != null && medicines.Count > 0)
                    {
                        ListMobaImpMestMedicine.AddRange(medicines);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void CreateGridByDataNewV2()
        //{
        //    try
        //    {
        //        WaitingManager.Show();
        //        this.ServiceReqPatientADOList = new List<ServiceReqPatientADO>();
        //        List<ServiceReqPatientADO> currentDataADOTemps = (from r in this.CurrentDataADOs select new ServiceReqPatientADO(r)).ToList();
        //        if (currentDataADOTemps != null && currentDataADOTemps.Count > 0 && cboMediStock.EditValue != null)
        //        {
        //            this.ServiceReqPatientADOList = currentDataADOTemps.Where(p => p.MEDI_STOCK_ID == (long)cboMediStock.EditValue).ToList();
        //        }
        //        else
        //            this.ServiceReqPatientADOList = currentDataADOTemps;

        //        for (int i = 0; i < this.ServiceReqPatientADOList.Count - 1; i++)
        //        {
        //            for (int j = i + 1; j < this.ServiceReqPatientADOList.Count; j++)
        //            {
        //                if (((this.ServiceReqPatientADOList[i].MATERIAL_TYPE_ID == this.ServiceReqPatientADOList[j].MATERIAL_TYPE_ID)
        //                    && this.ServiceReqPatientADOList[i].MATERIAL_TYPE_ID != null)
        //                    || ((this.ServiceReqPatientADOList[i].MEDICINE_TYPE_ID == this.ServiceReqPatientADOList[j].MEDICINE_TYPE_ID)
        //                    && this.ServiceReqPatientADOList[i].MEDICINE_TYPE_ID != 0))
        //                {
        //                    if (this.ServiceReqPatientADOList[i].AmountDate.IS_export == this.ServiceReqPatientADOList[j].AmountDate.IS_export)
        //                    {
        //                        if (this.ServiceReqPatientADOList[i].AmountDate.DATE == this.ServiceReqPatientADOList[j].AmountDate.DATE)
        //                        {
        //                            foreach (var item in this.ServiceReqPatientADOList[i].AmountDateList)
        //                            {
        //                                if (item.DATE == this.ServiceReqPatientADOList[j].AmountDate.DATE)
        //                                {
        //                                    item.Amount = item.Amount + this.ServiceReqPatientADOList[j].AmountDate.Amount;
        //                                }
        //                            }
        //                            this.ServiceReqPatientADOList[i].SUM = this.ServiceReqPatientADOList[i].SUM + this.ServiceReqPatientADOList[j].SUM;
        //                            this.ServiceReqPatientADOList.RemoveAt(j);
        //                            j--;
        //                        }
        //                        else
        //                        {
        //                            var dataCheck = this.ServiceReqPatientADOList[i].AmountDateList.FirstOrDefault(p => p.DATE == this.ServiceReqPatientADOList[j].AmountDate.DATE);
        //                            if (dataCheck != null)
        //                            {
        //                                dataCheck.Amount += this.ServiceReqPatientADOList[j].AmountDate.Amount;
        //                            }
        //                            else
        //                                this.ServiceReqPatientADOList[i].AmountDateList.Add(this.ServiceReqPatientADOList[j].AmountDate);
        //                            this.ServiceReqPatientADOList[i].SUM = this.ServiceReqPatientADOList[i].SUM + this.ServiceReqPatientADOList[j].SUM;
        //                            this.ServiceReqPatientADOList.RemoveAt(j);
        //                            j--;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        this.ServiceReqPatientADOList[i].AmountDateList.Add(this.ServiceReqPatientADOList[j].AmountDate);
        //                        this.ServiceReqPatientADOList[i].SUM = this.ServiceReqPatientADOList[i].SUM + this.ServiceReqPatientADOList[j].SUM;
        //                        this.ServiceReqPatientADOList.RemoveAt(j);
        //                        j--;
        //                    }
        //                }
        //            }
        //        }
        //        CreateColumn();
        //        this.ServiceReqPatientADOList = this.ServiceReqPatientADOList.OrderBy(o => o.Type)
        //           .ThenByDescending(o => o.IS_MADICINE).ToList();
        //        gridControl1.DataSource = this.ServiceReqPatientADOList;
        //        WaitingManager.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void CreateGridByDataNew()
        {
            try
            {
                WaitingManager.Show();
                this.ServiceReqPatientADOList = new List<ServiceReqPatientADO>();
                List<ServiceReqPatientADO> currentDataADOTemps = (from r in this.CurrentDataADOs select new ServiceReqPatientADO(r)).ToList();

                if (currentDataADOTemps != null && currentDataADOTemps.Count > 0)
                {
                    if (cboMediStock.EditValue != null)
                    {
                        currentDataADOTemps = currentDataADOTemps.Where(p => p.MEDI_STOCK_ID == (long)cboMediStock.EditValue).ToList();
                    }
                    var dataGroupByMediMateTypeId = currentDataADOTemps.GroupBy(p => new {p.MEDI_MATE_TYPE_ID, p.IS_MADICINE, p.IS_EXPEND }).Select(p => p.ToList()).ToList();
                    foreach (var itemGroups in dataGroupByMediMateTypeId)
                    {
                        ServiceReqPatientADO ado = new ServiceReqPatientADO(itemGroups[0]);
                        ado.AmountDateList = new List<ServiceReqPatientADO.amountdate>();
                        decimal total = 0;
                        foreach (var item in itemGroups)
                        {
                            ServiceReqPatientADO.amountdate adoDate = new ServiceReqPatientADO.amountdate();
                            adoDate.IS_export = item.AmountDate.IS_export;
                            adoDate.DATE = item.AmountDate.DATE;
                            adoDate.Amount = item.AmountDate.Amount;
                            adoDate.SortDate = item.AmountDate.SortDate;

                            if (item.MEDICINE_TYPE_ID > 0)
                            {
                                var mobaMedi = ListMobaImpMestMedicine.FirstOrDefault(o => o.TH_EXP_MEST_MEDICINE_ID == item.MEDI_MATE_EXP_MEST_ID);
                                if (mobaMedi != null)
                                {
                                    adoDate.Amount -= mobaMedi.AMOUNT;
                                }
                            }
                            else if (item.MATERIAL_TYPE_ID > 0)
                            {
                                var mobaMate = ListMobaImpMestMaterial.FirstOrDefault(o => o.TH_EXP_MEST_MATERIAL_ID == item.MEDI_MATE_EXP_MEST_ID);
                                if (mobaMate != null)
                                {
                                    adoDate.Amount -= mobaMate.AMOUNT;
                                }
                            }
                            total += adoDate.Amount;
                            var dataCheck = ado.AmountDateList.FirstOrDefault(p => p.DATE == adoDate.DATE && p.IS_export == adoDate.IS_export);
                            if (dataCheck != null)
                            {
                                dataCheck.Amount += adoDate.Amount;
                            }
                            else
                                ado.AmountDateList.Add(adoDate);
                        }
                        ado.SUM = total;
                        this.ServiceReqPatientADOList.Add(ado);
                    }
                }

                CreateColumn();
                this.ServiceReqPatientADOList = this.ServiceReqPatientADOList.OrderBy(o => o.Type)
                   .OrderBy(o => o.IS_MADICINE).ToList();
                gridControl1.DataSource = this.ServiceReqPatientADOList;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateColumn()
        {
            CreateDate();
            grblDay.Children.Clear();
            if (CreateListDate != null && CreateListDate.Count > 0)
            {
                CreateListDate = CreateListDate.OrderByDescending(p => p.IS_EXP).ThenBy(p => p.date).ToList();
            }
            foreach (var item in CreateListDate)
            {
                if (item.IS_EXP)
                {
                    DevExpress.XtraGrid.Views.BandedGrid.GridBand grblx = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();

                    grblx.Caption = item.Date.Split('e')[0] + "\n" + FactoryIsExp(item.IS_EXP);
                    grblx.Width = 80;
                    grblx.RowCount = 2;
                    DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn y = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                    y.Caption = "";
                    y.OptionsColumn.AllowEdit = false;
                    y.FieldName = item.Date;
                    y.Name = item.Date;
                    y.OptionsColumn.ShowCaption = false;
                    y.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    y.Visible = true;

                    grblDay.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] {
            grblx});

                    grblx.Columns.Add(y);
                }
                else
                {
                    DevExpress.XtraGrid.Views.BandedGrid.GridBand grblx = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();

                    grblx.Caption = item.Date.Split('e')[0] + "\n" + FactoryIsExp(item.IS_EXP);
                    grblx.Width = 80;
                    grblx.RowCount = 2;
                    DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn y = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                    y.Caption = "";
                    y.OptionsColumn.AllowEdit = false;
                    y.FieldName = item.Date;
                    y.Name = item.Date;
                    y.OptionsColumn.ShowCaption = false;
                    y.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    y.Visible = true;

                    grblDay.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] {
            grblx});

                    grblx.Columns.Add(y);
                }
            }
            //foreach (var item in CreateListDate)
            //{

            //    if (!item.IS_EXP)
            //    {
            //        DevExpress.XtraGrid.Views.BandedGrid.GridBand grblx = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();

            //        grblx.Caption = item.Date.Split('e')[0] + "\n" + FactoryIsExp(item.IS_EXP);
            //        grblx.Width = 80;
            //        grblx.RowCount = 2;
            //        DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn y = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            //        y.Caption = "";
            //        y.OptionsColumn.AllowEdit = false;
            //        y.FieldName = item.Date;
            //        y.Name = item.Date;
            //        y.OptionsColumn.ShowCaption = false;
            //        y.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            //        y.Visible = true;

            //        grblDay.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] {
            //grblx});

            //        grblx.Columns.Add(y);
            //    }
            //}
            grblDay.Width = 80 * CreateListDate.Count();
            #region v1
            //NumDate = HisServiceReqList.Count();
            //for (int i = 0; i < HisServiceReqList.Count(); i++)
            //{
            //    for (int j = i + 1; j < HisServiceReqList.Count(); j++)
            //    {
            //        if (HisServiceReqList[i].INTRUCTION_DATE == HisServiceReqList[j].INTRUCTION_DATE)
            //        {
            //            NumDate = NumDate - 1;
            //            HisServiceReqList.RemoveAt(j);
            //            j--;
            //        }
            //    }
            //}
            //for (int i = 0; i < NumDate; i++)
            //{
            //    DevExpress.XtraGrid.Views.BandedGrid.GridBand grblx = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            //    grblx.Caption = FactoryName(Convert.ToString(HisServiceReqList[i].INTRUCTION_DATE));

            //    DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn y = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            //    y.Caption = "";

            //    y.FieldName = FactoryName(Convert.ToString(HisServiceReqList[i].INTRUCTION_DATE));
            //    y.Name = FactoryName(Convert.ToString(HisServiceReqList[i].INTRUCTION_DATE));
            //    y.OptionsColumn.ShowCaption = false;
            //    y.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            //    y.Visible = true;

            //    grblDay.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] {
            //grblx});

            //    grblx.Columns.Add(y);

            //}
            #endregion


        }

        private void CreateDate()
        {
            CreateListDate = new List<NgayThucHien>();
            for (int i = 0; i < ServiceReqPatientADOList.Count; i++)
            {
                foreach (var item in ServiceReqPatientADOList[i].AmountDateList)
                {
                    NgayThucHien x = new NgayThucHien();
                    x.date = item.SortDate;
                    x.Date = item.DATE;
                    x.IS_EXP = item.IS_export;
                    if (CreateListDate.Count == 0)
                    {
                        CreateListDate.Add(x);
                    }
                    else
                    {
                        var dataCheck = CreateListDate.FirstOrDefault(p => p.Date == x.Date && p.IS_EXP == x.IS_EXP);
                        if (dataCheck == null)
                        {
                            CreateListDate.Add(x);
                        }
                        //Boolean check = false;
                        //foreach (var item2 in CreateListDate)
                        //{
                        //    if (x.Date == item2.Date && x.IS_EXP == item2.IS_EXP)
                        //    {
                        //        check = true;
                        //        break;
                        //    }
                        //}
                        //if (!check)
                        //{
                        //    CreateListDate.Add(x);
                        //}
                    }
                }
            }
            CreateListDate = CreateListDate.OrderBy(o => o.date).ToList();
        }

        private string FactoryIsExp(Boolean x)
        {
            string exp = "";
            if (x)
            {
                exp = " (Đã xuất)";
            }
            if (x == null || !x)
            {
                exp = " (Chưa lĩnh)";
            }
            return exp;
        }

        private string FactoryName(string Name)
        {
            string Month = Name.Substring(4, 2);
            string Day = Name.Substring(6, 2);
            string Year = Name.Substring(0, 4);
            return Day + "/" + Month + "/" + Year;
        }

        //private void LoadExpMestMedicine()
        //{
        //    foreach (var item in HisServiceReqList)
        //    {
        //        CommonParam paramCommon = new CommonParam();
        //        HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
        //        filter.TDL_SERVICE_REQ_ID = item.ID;
        //        List<HIS_EXP_MEST_MEDICINE> HisExpMestMedicine = new BackendAdapter(paramCommon).Get<List<HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.MOSHIS_EXP_MEST_MEDICINE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
        //        foreach (var Mety in HisExpMestMedicine)
        //        {
        //            ServiceReqPatientADO service = new ServiceReqPatientADO();
        //            if (Mety.TH_AMOUNT != null)
        //            {
        //                service.AMOUNT = Mety.AMOUNT - (decimal)Mety.TH_AMOUNT;
        //            }
        //            else
        //            {
        //                service.AMOUNT = Mety.AMOUNT;
        //            }
        //            service.MEDICINE_TYPE_ID = Mety.TDL_MEDICINE_TYPE_ID;
        //            if (Mety.IS_EXPEND == 1)
        //            {
        //                service.Type = "Hao phí";
        //            }
        //            else
        //            {
        //                service.Type = "Lĩnh thường";
        //            }
        //            service.SUM = Convert.ToInt32(service.AMOUNT);
        //            service.IS_MADICINE = 0;
        //            service.IS_EXPEND = Mety.IS_EXPEND;
        //            HisMedicineTypeViewFilter filter2 = new HisMedicineTypeViewFilter();
        //            filter2.ID = Mety.TDL_MEDICINE_TYPE_ID;
        //            var HisMedicineType = new BackendAdapter(paramCommon).Get<List<V_HIS_MEDICINE_TYPE>>(HisRequestUriStore.MOSHIS_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, filter2, paramCommon);
        //            service.ServiceReqPatientName = HisMedicineType.FirstOrDefault().MEDICINE_TYPE_NAME;
        //            service.UNIT_NAME = HisMedicineType.FirstOrDefault().SERVICE_UNIT_NAME;
        //            service.IS_STAR_MARK = (HisMedicineType.FirstOrDefault().IS_STAR_MARK == 1) ? true : false;
        //            service.IS_Red = (HisMedicineType.FirstOrDefault().MEDICINE_GROUP_ID == 1 || HisMedicineType.FirstOrDefault().MEDICINE_GROUP_ID == 2 || HisMedicineType.FirstOrDefault().MEDICINE_GROUP_ID == 3 || HisMedicineType.FirstOrDefault().MEDICINE_GROUP_ID == 4) ? true : false;
        //            service.NOTE = item.NOTE;
        //            HIS.Desktop.Plugins.ServiceReqPatient.ADO.ServiceReqPatientADO.amountdate x = new ServiceReqPatientADO.amountdate();
        //            if (Mety.IS_EXPORT == 1)
        //            {
        //                x.amountDate(FactoryName(item.INTRUCTION_DATE.ToString()) + "exp", service.AMOUNT, true);
        //            }
        //            else
        //            {
        //                x.amountDate(FactoryName(item.INTRUCTION_DATE.ToString()), service.AMOUNT, false);
        //            }
        //            x.SortDate = item.INTRUCTION_DATE;
        //            service.AmountDate = x;
        //            List<HIS.Desktop.Plugins.ServiceReqPatient.ADO.ServiceReqPatientADO.amountdate> ad = new List<ServiceReqPatientADO.amountdate>();
        //            ad.Add(x);
        //            service.AmountDateList = ad;
        //            ServiceReqPatientADOList.Add(service);
        //        }
        //    }
        //}

        private void LoadExpMestMedicineMaterialV2()
        {
            try
            {
                this.CurrentDataADOs = new List<ServiceReqPatientADO>();
                if (this.HisServiceReqList != null && this.HisServiceReqList.Count > 0)
                {
                    ListMobaExpMestIds.Clear();
                    int start = 0;
                    int count = this.HisServiceReqList.Count;
                    while (count > 0)
                    {
                        int limit = (count <= 100) ? count : 100;
                        var listSub = this.HisServiceReqList.Skip(start).Take(limit).ToList();
                        List<long> _serviceReqIds = new List<long>();
                        _serviceReqIds = listSub.Select(p => p.ID).Distinct().ToList();

                        GetExpMestMedicines(_serviceReqIds);
                        GetExpMestMaterials(_serviceReqIds);

                        start += 100;
                        count -= 100;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetExpMestMedicines(List<long> _ServiceReqIds)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
                filter.TDL_SERVICE_REQ_IDs = _ServiceReqIds;
                List<HIS_EXP_MEST_MEDICINE> HisExpMestMedicine = new BackendAdapter(paramCommon).Get<List<HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.MOSHIS_EXP_MEST_MEDICINE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                foreach (var Mety in HisExpMestMedicine)
                {
                    ServiceReqPatientADO service = new ServiceReqPatientADO();
                    service.MEDI_MATE_EXP_MEST_ID = Mety.ID;

                    service.AMOUNT = Mety.AMOUNT;
                    if (Mety.TH_AMOUNT.HasValue)
                    {
                        service.TH_AMOUNT = Mety.TH_AMOUNT;
                        ListMobaExpMestIds.Add(Mety.EXP_MEST_ID ?? 0);
                    }

                    service.MEDICINE_TYPE_ID = Mety.TDL_MEDICINE_TYPE_ID ?? 0;
                    service.MEDI_MATE_TYPE_ID = Mety.TDL_MEDICINE_TYPE_ID ?? 0;
                    if (Mety.IS_EXPEND == 1)
                    {
                        service.Type = "Hao phí";
                    }
                    else
                    {
                        service.Type = "Lĩnh thường";
                    }
                    service.SUM = service.AMOUNT;
                    service.IS_MADICINE = 0;
                    service.IS_EXPEND = Mety.IS_EXPEND;
                    service.MEDI_STOCK_ID = Mety.TDL_MEDI_STOCK_ID ?? 0;
                    service.TUTORIAL = Mety.TUTORIAL;

                    var _MedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == Mety.TDL_MEDICINE_TYPE_ID);
                    if (_MedicineType != null)
                    {
                        service.ServiceReqPatientName = _MedicineType.MEDICINE_TYPE_NAME;
                        service.UNIT_NAME = _MedicineType.SERVICE_UNIT_NAME;
                        service.IS_STAR_MARK = (_MedicineType.IS_STAR_MARK == 1) ? true : false;
                        service.IS_Red = (_MedicineType.MEDICINE_GROUP_ID == 1
                            || _MedicineType.MEDICINE_GROUP_ID == 2
                            || _MedicineType.MEDICINE_GROUP_ID == 3
                            || _MedicineType.MEDICINE_GROUP_ID == 4) ? true : false;
                        service.ACTIVE_INGR_BHYT_NAME = _MedicineType.ACTIVE_INGR_BHYT_NAME;
                    }
                    else continue;

                    var _ServiceReq = this.HisServiceReqList.FirstOrDefault(p => p.ID == Mety.TDL_SERVICE_REQ_ID);
                    if (_ServiceReq != null)
                    {
                        service.NOTE = _ServiceReq.NOTE;
                        ServiceReqPatientADO.amountdate x = new ServiceReqPatientADO.amountdate();
                        if (Mety.IS_EXPORT == 1)
                        {
                            x.amountDate(FactoryName(_ServiceReq.INTRUCTION_DATE.ToString()) + "exp", service.AMOUNT, true);
                        }
                        else
                        {
                            x.amountDate(FactoryName(_ServiceReq.INTRUCTION_DATE.ToString()), service.AMOUNT, false);
                        }
                        x.SortDate = _ServiceReq.INTRUCTION_DATE;
                        service.AmountDate = x;
                        //List<ServiceReqPatientADO.amountdate> ad = new List<ServiceReqPatientADO.amountdate>();
                        //ad.Add(x);
                        //service.AmountDateList = ad;
                        if (service.AmountDateList == null) service.AmountDateList = new List<ServiceReqPatientADO.amountdate>();
                        service.AmountDateList.Add(x);
                    }
                    else continue;

                    this.CurrentDataADOs.Add(service);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetExpMestMaterials(List<long> _ServiceReqIds)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisExpMestMaterialFilter filter = new HisExpMestMaterialFilter();
                filter.TDL_SERVICE_REQ_IDs = _ServiceReqIds;
                List<HIS_EXP_MEST_MATERIAL> HisExpMestMaterial = new BackendAdapter(paramCommon).Get<List<HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.MOSHIS_EXP_MEST_MATERIAL_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                foreach (var Mety in HisExpMestMaterial)
                {
                    ServiceReqPatientADO service = new ServiceReqPatientADO();
                    service.MEDI_MATE_EXP_MEST_ID = Mety.ID;

                    service.AMOUNT = Mety.AMOUNT;
                    if (Mety.TH_AMOUNT.HasValue)
                    {
                        service.TH_AMOUNT = Mety.TH_AMOUNT;
                        ListMobaExpMestIds.Add(Mety.EXP_MEST_ID ?? 0);
                    }

                    service.MATERIAL_TYPE_ID = Mety.TDL_MATERIAL_TYPE_ID;
                    service.MEDI_MATE_TYPE_ID = Mety.TDL_MATERIAL_TYPE_ID ?? 0;

                    if (Mety.IS_EXPEND == 1)
                    {
                        service.Type = "Hao phí";
                    }
                    else
                    {
                        service.Type = "Lĩnh thường";
                    }
                    service.SUM = service.AMOUNT;
                    service.IS_MADICINE = 1;
                    service.IS_EXPEND = Mety.IS_EXPEND;
                    service.MEDI_STOCK_ID = Mety.TDL_MEDI_STOCK_ID ?? 0;
                    service.TUTORIAL = Mety.TUTORIAL;

                    var _MaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == Mety.TDL_MATERIAL_TYPE_ID);
                    if (_MaterialType != null)
                    {
                        service.ServiceReqPatientName = _MaterialType.MATERIAL_TYPE_NAME;
                        service.UNIT_NAME = _MaterialType.SERVICE_UNIT_NAME;
                    }
                    else continue;

                    var _ServiceReq = this.HisServiceReqList.FirstOrDefault(p => p.ID == Mety.TDL_SERVICE_REQ_ID);
                    if (_ServiceReq != null)
                    {
                        service.NOTE = _ServiceReq.NOTE;
                        HIS.Desktop.Plugins.ServiceReqPatient.ADO.ServiceReqPatientADO.amountdate x = new ServiceReqPatientADO.amountdate();
                        if (Mety.IS_EXPORT == 1)
                        {
                            x.amountDate(FactoryName(_ServiceReq.INTRUCTION_DATE.ToString()) + "exp", service.AMOUNT, true);
                        }
                        else
                        {
                            x.amountDate(FactoryName(_ServiceReq.INTRUCTION_DATE.ToString()), service.AMOUNT, false);
                        }
                        x.SortDate = _ServiceReq.INTRUCTION_DATE;
                        service.AmountDate = x;
                        //List<HIS.Desktop.Plugins.ServiceReqPatient.ADO.ServiceReqPatientADO.amountdate> ad = new List<ServiceReqPatientADO.amountdate>();
                        //ad.Add(x);
                        //service.AmountDateList = ad;
                        if (service.AmountDateList == null) service.AmountDateList = new List<ServiceReqPatientADO.amountdate>();
                        service.AmountDateList.Add(x);
                    }
                    else continue;

                    this.CurrentDataADOs.Add(service);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServiceReq()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.TREATMENT_ID = TreatmentID;
                HisServiceReqList = new BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.MOSHIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, paramCommon);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {

                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {

                //ValidationSingleControl(txtEkipName);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDefaultFocus()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                #region Process has exception
                CommonParam paramCommon = new CommonParam();
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar()
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_EKIP_TEMP currentDTO)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void SetFocusEditor()
        {
            try
            {
                //txtEkipName.Focus();
                //txtEkipName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_EKIP_TEMP data)
        {
            try
            {
                if (data != null)
                {

                    //txtEkipName.Text = data.EKIP_TEMP_NAME;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Public method

        public void MeShow()
        {
            //cboIsPublic.SelectedIndex = 2;

            SetDefaultValue();

            //EnableControlChanged(this.ActionType);

            FillDatagctFormList();

            SetCaptionByLanguageKey();

            InitTabIndex();

            ValidateForm();

            SetDefaultFocus();
        }

        private void LoadComboMediStock()
        {
            try
            {
                if (this.CurrentDataADOs != null && this.CurrentDataADOs.Count > 0)
                {
                    List<long> _mediStockIds = this.CurrentDataADOs.Select(p => p.MEDI_STOCK_ID).Distinct().ToList();
                    var datas = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => _mediStockIds.Contains(p.ID)).ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "Mã", 50, 1));
                    columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "Tên", 200, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, true, 250);
                    ControlEditorLoader.Load(this.cboMediStock, datas, controlEditorADO);

                    var mediStockByRoomId = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this.currentModule.RoomId);
                    if (mediStockByRoomId != null)
                    {
                        this.cboMediStock.EditValue = mediStockByRoomId.ID;
                        this.cboMediStock.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        this.cboMediStock.EditValue = null;
                        this.cboMediStock.Properties.Buttons[1].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        Stream stream;

        private void bandedGridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ServiceReqPatientADO pData = (ServiceReqPatientADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    decimal amount = -1;
                    foreach (var item in pData.AmountDateList)
                    {
                        if (item.DATE == e.Column.FieldName)
                        {
                            amount = item.Amount;
                        }
                    }
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = ServiceReqPatientADOList.Count - e.ListSourceRowIndex;
                    }
                    else if (amount != -1)
                    {
                        e.Value = amount;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bandedGridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {

            try
            {
                GridView view = sender as GridView;
                if (view == null) return;
                if (e.Column.FieldName == "Type" && e.IsForGroupRow)
                {
                    string rowValue = Convert.ToString(view.GetGroupRowValue(e.GroupRowHandle, e.Column));
                    e.DisplayText = "" + rowValue;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewRooms_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = Convert.ToString(this.bandedGridView1.GetGroupRowValue(e.RowHandle, this.grclType) ?? "");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void bandedGridView1_CustomColumnGroup(object sender, CustomColumnSortEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "TYPE")
                {
                    e.Result = 1;
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void bandedGridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var data = (ServiceReqPatientADO)bandedGridView1.GetRow(e.RowHandle);
            if (data.IS_Red)
            {
                e.Appearance.ForeColor = System.Drawing.Color.Red;
            }
            if (data.IS_STAR_MARK)
            {
                e.Appearance.ForeColor = System.Drawing.Color.Blue;
            }
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            try
            {
                //bandedGridView1.ExportToExcelOld("C:\\Users\\TUNG\\Desktop\\Book1");
                //bandedGridView1.ExportToHtml(stream);
                //ADO.Printf _printf = new ADO.Printf();
                //_printf.TABLE = stream;
                //Printf.XtraForm1 o = new Printf.XtraForm1(_printf);
                //o.Show();

                PrintProcess(PrintTypeVotesMedicines.PHIEU_THEO_DOI_THUOC_VTHP_TIEU_HAO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboMediStock.EditValue = null;
                    cboMediStock.Properties.Buttons[1].Visible = false;
                    CreateGridByDataNew();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMediStock.EditValue != null)
                        cboMediStock.Properties.Buttons[1].Visible = true;
                    else
                        cboMediStock.Properties.Buttons[1].Visible = false;
                    CreateGridByDataNew();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSumaryTestResults_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.TreatmentID != null && this.TreatmentID > 0)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    listArgs.Add((long)(this.TreatmentID));

                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.SumaryTestResults", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #region handle new button to dieu tri, danh sach y lenh
        private void btnToDieuTri_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.TreatmentID != null && this.TreatmentID > 0)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    listArgs.Add((long)(this.TreatmentID));

                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTrackingList", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Debug(ex);
            }
        }

        private void btnDsYlenh_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.TreatmentID != null && this.TreatmentID > 0)
                {
                    HIS_TREATMENT dataTreatment = new HIS_TREATMENT();
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(dataTreatment, this.curentTreatment);
                    listArgs.Add(dataTreatment);
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceReqList", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        private string CaculatorAge(long date)
        {
            try
            {
                
                //DateTime dob = ConvertLongToDateTime(date);
                //DateTime currentDate = DateTime.Now;
                //TimeSpan ageSpan = currentDate - dob;

                //if (ageSpan.TotalHours < 24)
                //{
                //    return Math.Round(ageSpan.TotalHours)+" giờ tuổi";
                //}
                //else if (ageSpan.TotalDays < 30)
                //{
                //    return Math.Round(ageSpan.TotalDays)+" ngày tuổi";
                //}
                //else if (ageSpan.TotalDays < 72 * 30) // 72 tháng
                //{
                //    int months = (int)(ageSpan.TotalDays / 30);
                //    return months+" tháng tuổi";
                //}
                //else
                //{
                   
                //}
                var age = MPS.AgeUtil.CalculateFullAge(date);
                return age;
                
            }
            catch (Exception ex)
            {
                return "";
                LogSystem.Debug(ex);
            }
        }
        private static DateTime ConvertLongToDateTime(long dateTimeLong)
        {
            string dateTimeString = dateTimeLong.ToString();
            int year = int.Parse(dateTimeString.Substring(0, 4));
            int month = int.Parse(dateTimeString.Substring(4, 2));
            int day = int.Parse(dateTimeString.Substring(6, 2));
            int hour = int.Parse(dateTimeString.Substring(8, 2));
            int minute = int.Parse(dateTimeString.Substring(10, 2));
            return new DateTime(year, month, day, hour, minute, 0);
        }
        private decimal CaculatorWeight()
        {
            try
            {
                decimal weight = 0;
                HisDhstFilter  filter = new HisDhstFilter();
                filter.ORDER_FIELD = "EXECUTE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.TREATMENT_ID = TreatmentID;
                var result = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("/api/HisDhst/Get", ApiConsumers.MosConsumer, filter, null);
                if (result != null)
                {
                    result = result.Where(s => s.WEIGHT != null).ToList();
                    var maxExecuteTime = result.Max(x => x.EXECUTE_TIME);

                    // Lấy tất cả các bản ghi có EXECUTE_TIME bằng maxExecuteTime
                    var candidates = result.Where(x => x.EXECUTE_TIME == maxExecuteTime);

                    // Trong số các bản ghi có EXECUTE_TIME bằng nhau, lấy bản ghi có ID lớn nhất
                    var tempRs = candidates.OrderByDescending(x => x.ID).FirstOrDefault();

                    if (tempRs != null)
                    {
                        weight = tempRs.WEIGHT ?? 0;

                    }
                }
                return weight;
            }
            catch (Exception ex)
            {
                return 0;
                LogSystem.Debug(ex);
            }
        }
        #endregion


        
    }
}
