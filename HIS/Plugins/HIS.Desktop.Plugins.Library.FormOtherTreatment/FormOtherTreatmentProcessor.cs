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
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.FormOtherTreatment
{
    public class FormOtherTreatmentProcessor
    {
        HIS_TREATMENT CurrentTreatment;
        HIS_TREATMENT LastTreatment;
        List<V_HIS_FORM_TYPE_CFG_DATA> ListFormTypeConfig;
        List<SAR_FORM> ListForm;
        List<SAR_FORM_TYPE> ListFormType;
        RefeshReference RefreshList;

        DelegateSelectData delegateSelectData;

        public FormOtherTreatmentProcessor(HIS_TREATMENT _treatment, RefeshReference _refresh)
        {
            this.CurrentTreatment = _treatment;
            this.RefreshList = _refresh;
        }

        public FormOtherTreatmentProcessor(HIS_TREATMENT _treatment, DelegateSelectData _delegateSelectData)
        {
            this.CurrentTreatment = _treatment;
            this.delegateSelectData = _delegateSelectData;
        }

        public List<BarButtonItem> GetBarButtonItem(BarManager barManager)
        {
            return GetBarButtonItem(barManager, null);
        }

        public List<BarButtonItem> GetBarButtonItem(BarManager barManager, List<string> listFormTypeCodeDel)
        {
            List<BarButtonItem> result = null;
            try
            {
                if (CurrentTreatment != null && barManager != null)
                {
                    ProcessGetData();

                    List<V_HIS_FORM_TYPE_CFG_DATA> currentFormTypeConfig = ProcessFormTypeCfgData();

                    if (currentFormTypeConfig == null || currentFormTypeConfig.Count <= 0) return null;

                    result = new List<BarButtonItem>();

                    List<string> lstFormTypeCode = currentFormTypeConfig.Where(o => listFormTypeCodeDel == null || listFormTypeCodeDel.Count == 0 || (listFormTypeCodeDel != null && listFormTypeCodeDel.Count > 0 && !listFormTypeCodeDel.Contains(o.FORM_TYPE_CODE))).Select(s => s.FORM_TYPE_CODE).Distinct().ToList();
                    if (!String.IsNullOrWhiteSpace(CurrentTreatment.JSON_FORM_ID))
                    {
                        SarFormFilter formFilter = new SarFormFilter();
                        formFilter.ORDER_FIELD = "CREATE_TIME";
                        formFilter.ORDER_DIRECTION = "DESC";
                        formFilter.IDs = GetListIdFormJson(CurrentTreatment.JSON_FORM_ID);
                        formFilter.IS_ACTIVE = 1;
                        ListForm = new BackendAdapter(new CommonParam()).Get<List<SAR_FORM>>("api/SarForm/Get", ApiConsumer.ApiConsumers.SarConsumer, formFilter, null);
                    }
                    foreach (var item in lstFormTypeCode)
                    {
                        var formType = ListFormType.FirstOrDefault(o => o.FORM_TYPE_CODE == item);


                        if (formType != null)
                        {
                            BarButtonItem barItem = new BarButtonItem(barManager, formType.FORM_TYPE_NAME);

                            FormOtherADO ado = new FormOtherADO();
                            ado.HisTreatment = this.CurrentTreatment;
                            ado.SarForm = ListForm != null ? ListForm.FirstOrDefault(o => o.FORM_TYPE_ID == formType.ID) : null;
                            ado.SarFormType = formType;

                            barItem.Tag = ado;//
                            barItem.ItemClick += new ItemClickEventHandler(this.Treatment__MouseRightClick);
                            result.Add(barItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<List<BarButtonItem>> GetAsyncBarButtonItem(BarManager barManager, List<string> listFormTypeCodeDel = null)
        {
            List<BarButtonItem> result = null;
            try
            {
                if (CurrentTreatment != null && barManager != null)
                {
                    ListFormTypeConfig = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_FORM_TYPE_CFG_DATA>().Where(o => o.IS_ACTIVE == 1).ToList();
                    ListFormType = LocalStorage.BackendData.BackendDataWorker.Get<SAR_FORM_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();

                    if (!String.IsNullOrWhiteSpace(CurrentTreatment.JSON_FORM_ID))
                    {
                        SarFormFilter formFilter = new SarFormFilter();
                        formFilter.ORDER_FIELD = "CREATE_TIME";
                        formFilter.ORDER_DIRECTION = "DESC";
                        formFilter.IDs = GetListIdFormJson(CurrentTreatment.JSON_FORM_ID);
                        formFilter.IS_ACTIVE = 1;
                        ListForm = await new BackendAdapter(new CommonParam()).GetAsync<List<SAR_FORM>>("api/SarForm/Get", ApiConsumer.ApiConsumers.SarConsumer, formFilter, null);
                    }

                    List<V_HIS_FORM_TYPE_CFG_DATA> currentFormTypeConfig = ProcessFormTypeCfgData();

                    if (currentFormTypeConfig == null || currentFormTypeConfig.Count <= 0) return null;

                    result = new List<BarButtonItem>();

                    List<string> lstFormTypeCode = currentFormTypeConfig.Where(o => listFormTypeCodeDel == null || listFormTypeCodeDel.Count == 0 || (listFormTypeCodeDel != null && listFormTypeCodeDel.Count > 0 && !listFormTypeCodeDel.Contains(o.FORM_TYPE_CODE))).Select(s => s.FORM_TYPE_CODE).Distinct().ToList();
                    foreach (var item in lstFormTypeCode)
                    {
                        var formType = ListFormType.FirstOrDefault(o => o.FORM_TYPE_CODE == item);
                        if (formType != null)
                        {
                            BarButtonItem barItem = new BarButtonItem(barManager, formType.FORM_TYPE_NAME);

                            FormOtherADO ado = new FormOtherADO();
                            ado.HisTreatment = this.CurrentTreatment;
                            ado.SarForm = ListForm != null ? ListForm.FirstOrDefault(o => o.FORM_TYPE_ID == formType.ID) : null;
                            ado.SarFormType = formType;

                            barItem.Tag = ado;//
                            barItem.ItemClick += new ItemClickEventHandler(this.Treatment__MouseRightClick);
                            result.Add(barItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public async Task<List<DXMenuItem>> GetAsyncDXMenuItem(List<string> listFormTypeCodeDel = null)
        {
            List<DXMenuItem> result = null;
            try
            {
                if (CurrentTreatment != null)
                {
                    ListFormType = LocalStorage.BackendData.BackendDataWorker.Get<SAR_FORM_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                    ListFormTypeConfig = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_FORM_TYPE_CFG_DATA>().Where(o => o.IS_ACTIVE == 1).ToList();

                    if (!String.IsNullOrWhiteSpace(CurrentTreatment.JSON_FORM_ID))
                    {
                        SarFormFilter formFilter = new SarFormFilter();
                        formFilter.ORDER_FIELD = "CREATE_TIME";
                        formFilter.ORDER_DIRECTION = "DESC";
                        formFilter.IDs = GetListIdFormJson(CurrentTreatment.JSON_FORM_ID);
                        formFilter.IS_ACTIVE = 1;
                        ListForm = await new BackendAdapter(new CommonParam()).GetAsync<List<SAR_FORM>>("api/SarForm/Get", ApiConsumer.ApiConsumers.SarConsumer, formFilter, null);
                    }

                    List<V_HIS_FORM_TYPE_CFG_DATA> currentFormTypeConfig = ProcessFormTypeCfgData();

                    if (currentFormTypeConfig == null || currentFormTypeConfig.Count <= 0) return null;

                    result = new List<DXMenuItem>();

                    List<string> lstFormTypeCode = currentFormTypeConfig.Where(o => listFormTypeCodeDel == null || listFormTypeCodeDel.Count == 0 || (listFormTypeCodeDel != null && listFormTypeCodeDel.Count > 0 && !listFormTypeCodeDel.Contains(o.FORM_TYPE_CODE))).Select(s => s.FORM_TYPE_CODE).Distinct().ToList();
                    foreach (var item in lstFormTypeCode)
                    {
                        var formType = ListFormType.FirstOrDefault(o => o.FORM_TYPE_CODE == item);
                        if (formType != null)
                        {
                            DXMenuItem menuItem = new DXMenuItem(formType.FORM_TYPE_NAME);

                            FormOtherADO ado = new FormOtherADO();
                            ado.HisTreatment = this.CurrentTreatment;
                            ado.SarForm = ListForm != null ? ListForm.FirstOrDefault(o => o.FORM_TYPE_ID == formType.ID) : null;
                            ado.SarFormType = formType;

                            menuItem.Tag = ado;//
                            menuItem.Click += new EventHandler(this.Treatment__MenuClick);
                            result.Add(menuItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string ProcessJsonFormId(MOS.EFMODEL.DataModels.HIS_TREATMENT treatment)
        {
            string result = "";
            try
            {
                char[] separator = { ',' };
                List<string> jsonFormIdLastList = null;
                List<string> jsonFormIdCurrentList = null;
                if (this.LastTreatment != null && !String.IsNullOrEmpty(this.LastTreatment.JSON_FORM_ID))
                {
                    jsonFormIdLastList = this.LastTreatment.JSON_FORM_ID.Split(separator).ToList();
                }

                if (treatment != null && !String.IsNullOrEmpty(treatment.JSON_FORM_ID))
                {
                    jsonFormIdCurrentList = treatment.JSON_FORM_ID.Split(separator).ToList();
                }

                List<string> jsonIdJoinList = new List<string>();

                if (jsonFormIdLastList != null && jsonFormIdLastList.Count > 0)
                    jsonIdJoinList.Add(String.Join(",", jsonFormIdLastList));

                if (jsonFormIdCurrentList != null && jsonFormIdCurrentList.Count > 0)
                    jsonIdJoinList.Add(String.Join(",", jsonFormIdCurrentList));

                if (jsonIdJoinList != null && jsonIdJoinList.Count > 0)
                    jsonIdJoinList = jsonIdJoinList.Distinct().ToList();

                var currentTreatment = treatment;
                result = String.Join(",", jsonIdJoinList);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public List<DXMenuItem> GetDXMenuItem()
        {
            return GetDXMenuItem(null);
        }

        public List<DXMenuItem> GetDXMenuItem(List<string> listFormTypeCodeDel)
        {
            List<DXMenuItem> result = null;
            try
            {
                if (CurrentTreatment != null)
                {

                    ProcessGetData();

                    //CurrentTreatment.JSON_FORM_ID = ProcessJsonFormId(CurrentTreatment);

                    List<V_HIS_FORM_TYPE_CFG_DATA> currentFormTypeConfig = ProcessFormTypeCfgData();

                    if (currentFormTypeConfig == null || currentFormTypeConfig.Count <= 0) return null;

                    result = new List<DXMenuItem>();

                    List<string> lstFormTypeCode = currentFormTypeConfig.Where(o => listFormTypeCodeDel == null || listFormTypeCodeDel.Count == 0 || (listFormTypeCodeDel != null && listFormTypeCodeDel.Count > 0 && !listFormTypeCodeDel.Contains(o.FORM_TYPE_CODE))).Select(s => s.FORM_TYPE_CODE).Distinct().ToList();                 
                    foreach (var item in lstFormTypeCode)
                    {
                        var formType = ListFormType.FirstOrDefault(o => o.FORM_TYPE_CODE == item);
                        if (formType != null)
                        {
                            DXMenuItem menuItem = new DXMenuItem(formType.FORM_TYPE_NAME);

                            FormOtherADO ado = new FormOtherADO();

                            ado.HisTreatment = this.CurrentTreatment;

                            ado.SarForm = ListForm != null ? ListForm.FirstOrDefault(o => o.FORM_TYPE_ID == formType.ID) : null;
                            ado.SarFormType = formType;

                            menuItem.Tag = ado;//
                            menuItem.Click += new EventHandler(this.Treatment__MenuClick);
                            result.Add(menuItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_FORM_TYPE_CFG_DATA> ProcessFormTypeCfgData()
        {
            List<V_HIS_FORM_TYPE_CFG_DATA> result = null;
            try
            {
                if (ListFormTypeConfig != null && ListFormTypeConfig.Count > 0)
                {
                    result = new List<V_HIS_FORM_TYPE_CFG_DATA>();
                    var groupFormTypeCode = ListFormTypeConfig.Where(o => !CheckFormTypeCfgCode(o.FORM_TYPE_CFG_CODE)).GroupBy(o => o.FORM_TYPE_CODE).ToList();
                    foreach (var grFormType in groupFormTypeCode)
                    {
                        //if (!grFormType.ToList().Exists(o => o.FORM_TYPE_CFG_CODE == "IS_FOR_TREATMENT"))
                        //    continue;//xuandv bo qua de lay ho so dieu tri có thoi gian tu vong , thoi gian vao vien

                        bool isAdd = true;
                        foreach (var item in grFormType)
                        {
                            if (!isAdd) break;

                            //if (CheckFormTypeCfgCode(item.FORM_TYPE_CFG_CODE))
                            //{
                            //    isAdd = false;
                            //}

                            //Bo qua khi khong co thoi gian tu vong
                            if (item.FORM_TYPE_CFG_CODE == "IS_HAS_DEATH_TIME" &&
                                (CurrentTreatment.DEATH_TIME == null || CurrentTreatment.DEATH_TIME <= 0))
                                isAdd = false;

                            //Bo qua khi khong co thoi gian vao vien
                            if (item.FORM_TYPE_CFG_CODE == "IS_HAS_CLINICAL_IN_TIME" &&
                                (!CurrentTreatment.CLINICAL_IN_TIME.HasValue || CurrentTreatment.CLINICAL_IN_TIME <= 0))
                                isAdd = false;
                        }

                        if (isAdd)
                        {
                            result.AddRange(grFormType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessGetData()
        {
            Thread HisFormTypeConfig = new Thread(GetHisFormTypeConfig);
            Thread SarFormType = new Thread(GetSarFormType);
            Thread SarForm = new Thread(GetSarForm);
            //Thread LastTreatment = new Thread(GetLastTreament);
            try
            {
                HisFormTypeConfig.Start();
                SarFormType.Start();
                SarForm.Start();
                //LastTreatment.Start();

                HisFormTypeConfig.Join();
                SarFormType.Join();
                SarForm.Join();
                //LastTreatment.Join();
            }
            catch (Exception ex)
            {
                HisFormTypeConfig.Abort();
                SarFormType.Abort();
                SarForm.Abort();
                //LastTreatment.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSarForm()
        {
            try
            {
                GetLastTreament();
                CurrentTreatment.JSON_FORM_ID = ProcessJsonFormId(CurrentTreatment);
                //   List<HIS_FORM_TYPE_CONFIG> currentFormTypeConfig = new List<HIS_FORM_TYPE_CONFIG>();
                if (!String.IsNullOrWhiteSpace(CurrentTreatment.JSON_FORM_ID))
                {
                    SarFormFilter formFilter = new SarFormFilter();
                    formFilter.ORDER_FIELD = "CREATE_TIME";
                    formFilter.ORDER_DIRECTION = "DESC";
                    formFilter.IDs = GetListIdFormJson(CurrentTreatment.JSON_FORM_ID);
                    formFilter.IS_ACTIVE = 1;
                    ListForm = new BackendAdapter(new CommonParam()).Get<List<SAR_FORM>>("api/SarForm/Get", ApiConsumer.ApiConsumers.SarConsumer, formFilter, null);
                }

                Inventec.Common.Logging.LogSystem.Debug("GetSarForm() CurrentTreatment JSON_FORM_ID " + CurrentTreatment.JSON_FORM_ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSarFormType()
        {
            try
            {
                ListFormType = LocalStorage.BackendData.BackendDataWorker.Get<SAR_FORM_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetHisFormTypeConfig()
        {
            try
            {
                ListFormTypeConfig = LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_FORM_TYPE_CFG_DATA>().Where(o => o.IS_ACTIVE == 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetLastTreament()
        {
            try
            {
                if (this.CurrentTreatment != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("GetLastTreament() this.CurrentTreatment PATIENT_ID " + this.CurrentTreatment.PATIENT_ID);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("GetLastTreament() this.CurrentTreatment IS NULL");
                }

                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                //treatmentFilter.IS_ACTIVE = 1;
                treatmentFilter.PATIENT_ID = this.CurrentTreatment.PATIENT_ID;
                var LastTreatments = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, null);

                if (LastTreatments != null && LastTreatments.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("LastTreatments IDs: " + String.Join("; ", LastTreatments.Select(o => o.ID).ToList()));
                    var checkTreament = LastTreatments.OrderByDescending(o => o.IN_TIME).Where(o => o.IN_TIME < this.CurrentTreatment.IN_TIME);
                    this.LastTreatment = checkTreament != null && checkTreament.Count() > 0 ? checkTreament.FirstOrDefault() : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<long> GetListIdFormJson(string JSON_FORM_ID)
        {
            List<long> result = null;
            try
            {
                if (!String.IsNullOrWhiteSpace(JSON_FORM_ID))
                {
                    result = new List<long>();
                    var arrIds = JSON_FORM_ID.Split(',', ';');
                    if (arrIds != null && arrIds.Length > 0)
                    {
                        foreach (var id in arrIds)
                        {
                            long formId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                            if (formId > 0)
                            {
                                result.Add(formId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Treatment__MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //gọi base dll
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    FormOtherADO tag = (FormOtherADO)(e.Item.Tag);
                    if (tag != null)
                    {
                        FRD.ProcessorBase.FDO.FormInitFDO ado = new FRD.ProcessorBase.FDO.FormInitFDO();
                        if (tag.SarFormType != null)
                        {
                            ado.FormTypeCode = tag.SarFormType.FORM_TYPE_CODE;
                            ado.FormTypeName = tag.SarFormType.FORM_TYPE_NAME;
                            ado.IsOne = tag.SarFormType.IS_ONE;
                        }
                        var currentTreatment = tag.HisTreatment;
                        currentTreatment.JSON_FORM_ID = ProcessJsonFormId(tag.HisTreatment);
                        ado.Data = tag.HisTreatment;
                        if (tag.SarForm != null)
                        {
                            ado.FormId = tag.SarForm.ID;//Có thiof update
                            ado.IsEdit = true;//Sửa
                        }
                        ado.IsView = false;//xem
                        ado.SarForms = (this.ListForm != null && this.ListForm.Count > 0 && tag.SarFormType != null) ? this.ListForm.Where(o => o.FORM_TYPE_ID == tag.SarFormType.ID).ToList() : null;
                        ado.delegateSelectFormId = UpdateJsonFormId_Treatment;
                        ado.delegateDeleteFormId = DeleteJsonFormId_Treatment;
                        if (ado.FormTypeCode == "Frd000008")
                        {
                            FRD.FrdProcessorV2.Run(ado);
                        }
                        else
                        {
                            FRD.FrdProcessor.Run(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Treatment__MenuClick(object sender, EventArgs e)
        {
            try
            {
                if (sender is DXMenuItem)
                {
                    var bbtnItem = sender as DXMenuItem;
                    FormOtherADO tag = (FormOtherADO)(bbtnItem.Tag);
                    if (tag != null)
                    {
                        FRD.ProcessorBase.FDO.FormInitFDO ado = new FRD.ProcessorBase.FDO.FormInitFDO();
                        if (tag.SarFormType != null)
                        {
                            ado.FormTypeCode = tag.SarFormType.FORM_TYPE_CODE;
                            ado.FormTypeName = tag.SarFormType.FORM_TYPE_NAME;
                            ado.IsOne = tag.SarFormType.IS_ONE;
                        }
                        var currentTreatment = tag.HisTreatment;
                        currentTreatment.JSON_FORM_ID = ProcessJsonFormId(tag.HisTreatment);
                        
                        ado.Data = currentTreatment;

                        if (tag.SarForm != null)
                        {
                            ado.FormId = tag.SarForm.ID;//Có thì update
                            ado.IsEdit = true;//Sửa
                        }
                        ado.SarForms = (this.ListForm != null && this.ListForm.Count > 0 && tag.SarFormType != null) ? this.ListForm.Where(o => o.FORM_TYPE_ID == tag.SarFormType.ID).ToList() : null;
                        ado.IsView = false;//xem
                        ado.delegateSelectFormId = UpdateJsonFormId_Treatment;
                        ado.delegateDeleteFormId = DeleteJsonFormId_Treatment;
                        if (ado.FormTypeCode == "Frd000008")
                        {
                            FRD.FrdProcessorV2.Run(ado);
                        }
                        else
                        {
                            FRD.FrdProcessor.Run(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateJsonFormId_Treatment(long formId)
        {
            try
            {
                if (this.CurrentTreatment != null)
                {
                    List<long> lstFormId = new List<long>();
                    lstFormId = GetListIdFormJson(CurrentTreatment.JSON_FORM_ID);
                    if (lstFormId == null) lstFormId = new List<long>();
                    lstFormId.Add(formId);
                    lstFormId = lstFormId.Distinct().ToList();

                    string printIds = string.Join(",", lstFormId);
                    HIS_TREATMENT updateData = new HIS_TREATMENT();

                    updateData.JSON_FORM_ID = printIds;
                    updateData.ID = CurrentTreatment.ID;

                    CommonParam param = new CommonParam();
                    //bool success = false;
                    var apiResult = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateJsonForm", ApiConsumer.ApiConsumers.MosConsumer, updateData, SessionManager.ActionLostToken, param);
                    if (apiResult == null || param.BugCodes.Count > 0 || param.Messages.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    }
                    else
                    {
                        this.CurrentTreatment = apiResult;
                        if (RefreshList != null)
                        {
                            RefreshList();
                        }
                        if (delegateSelectData != null)
                        {
                            delegateSelectData(this.CurrentTreatment);
                        }
                    }

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DeleteJsonFormId_Treatment(long formId)
        {
            try
            {
                if (this.CurrentTreatment != null)
                {
                    List<long> lstFormId = new List<long>();
                    lstFormId = GetListIdFormJson(CurrentTreatment.JSON_FORM_ID);
                    if (lstFormId == null) lstFormId = new List<long>();
                    if (lstFormId != null && lstFormId.Contains(formId))
                    {
                        lstFormId.Remove(formId);
                    }
                    lstFormId = lstFormId.Distinct().ToList();

                    string printIds = lstFormId.Count > 0 ? string.Join(",", lstFormId) : "";
                    HIS_TREATMENT updateData = new HIS_TREATMENT();

                    updateData.JSON_FORM_ID = printIds;
                    updateData.ID = CurrentTreatment.ID;

                    CommonParam param = new CommonParam();
                    //bool success = false;
                    var apiResult = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateJsonForm", ApiConsumer.ApiConsumers.MosConsumer, updateData, SessionManager.ActionLostToken, param);                 
                    if (apiResult == null || param.BugCodes.Count > 0 || param.Messages.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    }
                    else
                    {
                        this.CurrentTreatment = apiResult;
                        if (RefreshList != null)
                        {
                            RefreshList();
                        }
                        if (delegateSelectData != null)
                        {
                            delegateSelectData(this.CurrentTreatment);
                        }
                    }

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckFormTypeCfgCode(string code)
        {
            bool result = true;
            try
            {
                if (!String.IsNullOrWhiteSpace(code))
                {
                    if (code.ToUpper().Contains("IS_HAS_"))
                    {
                        var keyName = code.Replace("IS_HAS_", " ").Trim();
                        System.Reflection.PropertyInfo[] pis = typeof(HIS_TREATMENT).GetProperties();
                        if (pis != null && pis.Length > 0)
                        {
                            foreach (var pi in pis)
                            {
                                if (pi.Name == keyName)
                                {
                                    var piValue = pi.GetValue(CurrentTreatment);
                                    if (piValue != null)
                                    {
                                        result = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (code.ToUpper() == "IS_FOR_TREATMENT")
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
