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

namespace HIS.Desktop.Plugins.Library.FormOtherServiceReq
{
    public class FormOtherProcessor
    {
        HIS_SERVICE_REQ CurrentServiceReq;
        List<V_HIS_FORM_TYPE_CFG_DATA> ListFormTypeConfig;
        List<SAR_FORM> ListForm;
        List<SAR_FORM_TYPE> ListFormType;
        RefeshReference RefreshList;
        Action RefreshReference;

        public FormOtherProcessor(HIS_SERVICE_REQ _serviceReq, RefeshReference _refresh)
        {
            this.CurrentServiceReq = _serviceReq;
            this.RefreshList = _refresh;
        }

        public FormOtherProcessor(HIS_SERVICE_REQ _serviceReq, RefeshReference _refresh, Action _actrefresh)
        {
            this.CurrentServiceReq = _serviceReq;
            this.RefreshList = _refresh;
            this.RefreshReference = _actrefresh;
        }

        public List<BarButtonItem> GetBarButtonItem(BarManager barManager)
        {
            List<BarButtonItem> result = null;
            try
            {
                if (CurrentServiceReq != null && barManager != null)
                {
                    ProcessGetData();

                    List<V_HIS_FORM_TYPE_CFG_DATA> currentFormTypeConfig = ProcessFormTypeCfgData();

                    if (currentFormTypeConfig == null || currentFormTypeConfig.Count <= 0) return null;

                    result = new List<BarButtonItem>();
                    List<string> lstFormTypeCode = currentFormTypeConfig.Select(s => s.FORM_TYPE_CODE).Distinct().ToList();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstFormTypeCode), lstFormTypeCode));
                    foreach (var item in lstFormTypeCode)
                    {
                        var formType = ListFormType.FirstOrDefault(o => o.FORM_TYPE_CODE == item);
                        if (formType != null)
                        {
                            BarButtonItem barItem = new BarButtonItem(barManager, formType.FORM_TYPE_NAME, 4);

                            FormOtherADO ado = new FormOtherADO();
                            ado.HisServiceReq = this.CurrentServiceReq;
                            ado.SarForm = ListForm != null ? ListForm.FirstOrDefault(o => o.FORM_TYPE_ID == formType.ID) : null;
                            ado.SarFormType = formType;

                            barItem.Tag = ado;//
                            barItem.ItemClick += new ItemClickEventHandler(this.ServiceReq__MouseRightClick);
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

        public List<DXMenuItem> GetDXMenuItem()
        {
            List<DXMenuItem> result = null;
            try
            {
                if (CurrentServiceReq != null)
                {
                    ProcessGetData();

                    List<V_HIS_FORM_TYPE_CFG_DATA> currentFormTypeConfig = ProcessFormTypeCfgData();

                    if (currentFormTypeConfig == null || currentFormTypeConfig.Count <= 0) return null;

                    result = new List<DXMenuItem>();
                    List<string> lstFormTypeCode = currentFormTypeConfig.Select(s => s.FORM_TYPE_CODE).Distinct().ToList();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstFormTypeCode), lstFormTypeCode));
                    foreach (var item in lstFormTypeCode)
                    {
                        var formType = ListFormType.FirstOrDefault(o => o.FORM_TYPE_CODE == item);
                        if (formType != null)
                        {
                            DXMenuItem menuItem = new DXMenuItem(formType.FORM_TYPE_NAME);

                            FormOtherADO ado = new FormOtherADO();
                            ado.HisServiceReq = this.CurrentServiceReq;
                            ado.SarForm = ListForm != null ? ListForm.FirstOrDefault(o => o.FORM_TYPE_ID == formType.ID) : null;
                            ado.SarFormType = formType;

                            menuItem.Tag = ado;//
                            menuItem.Click += new EventHandler(this.ServiceReq__MenuClick);
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

        private void ProcessGetData()
        {
            Thread HisFormTypeConfig = new Thread(GetHisFormTypeConfig);
            Thread SarFormType = new Thread(GetSarFormType);
            Thread SarForm = new Thread(GetSarForm);
            try
            {
                HisFormTypeConfig.Start();
                SarFormType.Start();
                SarForm.Start();

                HisFormTypeConfig.Join();
                SarFormType.Join();
                SarForm.Join();
            }
            catch (Exception ex)
            {
                HisFormTypeConfig.Abort();
                SarFormType.Abort();
                SarForm.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSarForm()
        {
            try
            {
                // List<HIS_FORM_TYPE_CONFIG> currentFormTypeConfig = new List<HIS_FORM_TYPE_CONFIG>();
                if (!String.IsNullOrWhiteSpace(CurrentServiceReq.JSON_FORM_ID))
                {
                    SarFormFilter formFilter = new SarFormFilter();
                    formFilter.ORDER_FIELD = "CREATE_TIME";
                    formFilter.ORDER_DIRECTION = "DESC";
                    formFilter.IDs = GetListIdFormJson(CurrentServiceReq.JSON_FORM_ID);
                    formFilter.IS_ACTIVE = 1;
                    ListForm = new BackendAdapter(new CommonParam()).Get<List<SAR_FORM>>("api/SarForm/Get", ApiConsumer.ApiConsumers.SarConsumer, formFilter, null);
                }
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

        private void ServiceReq__MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //gọi base dll
                if (sender is BarButtonItem)
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
                        ado.Data = tag.HisServiceReq;
                        if (tag.SarForm != null)
                        {
                            ado.FormId = tag.SarForm.ID;//Có thiof update
                            ado.IsEdit = true;//Sửa
                        }
                        ado.SarForms = this.ListForm;
                        ado.IsView = false;//xem
                        ado.delegateSelectFormId = UpdateJsonFormId_ServiceReq;
                        ado.dlgRefeshControl = RefreshReference;
                        FRD.FrdProcessor.Run(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ServiceReq__MenuClick(object sender, EventArgs e)
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
                        ado.Data = tag.HisServiceReq;
                        if (tag.SarForm != null)
                        {
                            ado.FormId = tag.SarForm.ID;//Có thiof update
                            ado.IsEdit = true;//Sửa
                        }
                        ado.SarForms = this.ListForm;
                        ado.IsView = false;//xem
                        ado.delegateSelectFormId = UpdateJsonFormId_ServiceReq;
                        ado.dlgRefeshControl = RefreshReference;
                        FRD.FrdProcessor.Run(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateJsonFormId_ServiceReq(long formId)
        {
            try
            {
                if (this.CurrentServiceReq != null)
                {
                    List<long> lstFormId = new List<long>();
                    lstFormId = GetListIdFormJson(CurrentServiceReq.JSON_FORM_ID);
                    if (lstFormId == null) lstFormId = new List<long>();
                    lstFormId.Add(formId);
                    lstFormId = lstFormId.Distinct().ToList();

                    string printIds = string.Join(",", lstFormId);
                    HIS_SERVICE_REQ updateData = new HIS_SERVICE_REQ();

                    updateData.JSON_FORM_ID = printIds;
                    updateData.ID = CurrentServiceReq.ID;

                    CommonParam param = new CommonParam();
                    //bool success = false;
                    var apiResult = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisServiceReq/UpdateJsonForm", ApiConsumer.ApiConsumers.MosConsumer, updateData, SessionManager.ActionLostToken, param);
                    if (apiResult == null || param.BugCodes.Count > 0 || param.Messages.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    }
                    else
                    {
                        this.CurrentServiceReq = apiResult;
                        if (RefreshList != null)
                        {
                            RefreshList();
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
                        //if (!grFormType.ToList().Exists(o => o.FORM_TYPE_CFG_CODE == "IS_FOR_SERVICE_REQ")) continue;

                        bool isAdd = true;
                        foreach (var item in grFormType)
                        {
                            if (!isAdd) break;

                            //Bo qua khi khong co loai
                            if (item.FORM_TYPE_CFG_CODE == "SERVICE_REQ_TYPE_IDS" &&
                                 !String.IsNullOrWhiteSpace(item.VALUE) &&
                                (!GetListIdFormJson(item.VALUE).Contains(CurrentServiceReq.SERVICE_REQ_TYPE_ID)))
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

        private bool CheckFormTypeCfgCode(string code)
        {
            bool result = true;
            try
            {
                if (!String.IsNullOrWhiteSpace(code))
                {
                    //    if (code.ToUpper().Contains("IS_HAS_"))
                    //    {
                    //        var keyName = code.Replace("IS_HAS_", " ").Trim();
                    //        System.Reflection.PropertyInfo[] pis = typeof(HIS_TREATMENT).GetProperties();
                    //        if (pis != null && pis.Length > 0)
                    //        {
                    //            foreach (var pi in pis)
                    //            {
                    //                if (pi.Name == keyName)
                    //                {
                    //                    var piValue = pi.GetValue(CurrentTreatment);
                    //                    if (piValue != null)
                    //                    {
                    //                        result = false;
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //    else 
                    if (code.ToUpper() == "IS_FOR_SERVICE_REQ")
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
