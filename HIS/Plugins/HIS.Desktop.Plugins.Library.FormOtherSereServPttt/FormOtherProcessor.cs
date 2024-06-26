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
using MPS.Processor.Mps000332.PDO;

namespace HIS.Desktop.Plugins.Library.FormOtherSereServPttt
{
    public class FormOtherProcessor
    {
        V_HIS_SERE_SERV_PTTT CurrentSereServPttt;
        HIS_SERE_SERV_EXT CurrentSereServExt;
        HIS_SERVICE_REQ CurrentServiceReq;
        V_HIS_SERE_SERV CurrentSereServ;

        List<V_HIS_FORM_TYPE_CFG_DATA> ListFormTypeConfig;
        List<SAR_FORM> ListForm;
        List<SAR_FORM_TYPE> ListFormType;
        RefeshReference RefreshList;

        public FormOtherProcessor(HIS_SERVICE_REQ _ServiceReq, V_HIS_SERE_SERV _SereServ, V_HIS_SERE_SERV_PTTT _SereServPttt, HIS_SERE_SERV_EXT _sereServExt, RefeshReference _refresh)
        {
            this.CurrentSereServPttt = _SereServPttt;
            this.CurrentSereServExt = _sereServExt;
            this.CurrentSereServ = _SereServ;
            this.CurrentServiceReq = _ServiceReq;
            this.RefreshList = _refresh;
        }

        public List<BarButtonItem> GetBarButtonItem(BarManager barManager)
        {
            List<BarButtonItem> result = null;
            try
            {
                if (CurrentSereServ != null && barManager != null)
                {
                    ProcessGetData();

                    List<V_HIS_FORM_TYPE_CFG_DATA> currentFormTypeConfig = ProcessFormTypeCfgData();

                    if (currentFormTypeConfig == null || currentFormTypeConfig.Count <= 0) return null;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentFormTypeConfig), currentFormTypeConfig));
                    result = new List<BarButtonItem>();
                    List<string> lstFormTypeCode = currentFormTypeConfig.Select(s => s.FORM_TYPE_CODE).Distinct().ToList();
                    foreach (var item in lstFormTypeCode)
                    {
                        var formType = ListFormType.FirstOrDefault(o => o.FORM_TYPE_CODE == item);
                        if (formType != null)
                        {
                            BarButtonItem barItem = new BarButtonItem(barManager, formType.FORM_TYPE_NAME, 4);

                            FormOtherADO ado = new FormOtherADO();
                            ado.HisSereServPttt = this.CurrentSereServPttt;
                            ado.HisSereServExt = this.CurrentSereServExt;
                            ado.HisSereServ = this.CurrentSereServ;
                            ado.HisServiceReq = this.CurrentServiceReq;
                            ado.SarForm = ListForm != null ? ListForm.FirstOrDefault(o => o.FORM_TYPE_ID == formType.ID) : null;
                            ado.SarFormType = formType;

                            barItem.Tag = ado;//
                            barItem.ItemClick += new ItemClickEventHandler(this.SereServ__MouseRightClick);
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
                Inventec.Common.Logging.LogSystem.Debug("GetDXMenuItem. 1");
                if (CurrentSereServ != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("GetDXMenuItem. 2");
                    ProcessGetData();

                    List<V_HIS_FORM_TYPE_CFG_DATA> currentFormTypeConfig = ProcessFormTypeCfgData();

                    if (currentFormTypeConfig == null || currentFormTypeConfig.Count <= 0) return null;

                    result = new List<DXMenuItem>();
                    List<string> lstFormTypeCode = currentFormTypeConfig.Select(s => s.FORM_TYPE_CODE).Distinct().ToList();
                    foreach (var item in lstFormTypeCode)
                    {
                        var formType = ListFormType.FirstOrDefault(o => o.FORM_TYPE_CODE == item);
                        if (formType != null)
                        {
                            DXMenuItem menuItem = new DXMenuItem(formType.FORM_TYPE_NAME);

                            FormOtherADO ado = new FormOtherADO();
                            ado.HisSereServPttt = this.CurrentSereServPttt;
                            ado.HisSereServExt = this.CurrentSereServExt;
                            ado.HisSereServ = this.CurrentSereServ;
                            ado.HisServiceReq = this.CurrentServiceReq;
                            ado.SarForm = ListForm != null ? ListForm.FirstOrDefault(o => o.FORM_TYPE_ID == formType.ID) : null;
                            ado.SarFormType = formType;

                            menuItem.Tag = ado;//
                            menuItem.Click += new EventHandler(this.SereServ__MenuClick);
                            result.Add(menuItem);
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("GetDXMenuItem. 3");
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
                if (CurrentSereServExt != null && !String.IsNullOrWhiteSpace(CurrentSereServExt.JSON_FORM_ID))
                {
                    SarFormFilter formFilter = new SarFormFilter();
                    formFilter.ORDER_FIELD = "CREATE_TIME";
                    formFilter.ORDER_DIRECTION = "DESC";
                    formFilter.IDs = GetListIdFormJson(CurrentSereServExt.JSON_FORM_ID);
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
                SarFormTypeFilter formTypeFilter = new SarFormTypeFilter();
                formTypeFilter.IS_ACTIVE = 1;
                ListFormType = new BackendAdapter(new CommonParam()).Get<List<SAR_FORM_TYPE>>("api/SarFormType/Get", ApiConsumer.ApiConsumers.SarConsumer, formTypeFilter, null);
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
                HisFormTypeCfgDataViewFilter formTypeConfigFilter = new HisFormTypeCfgDataViewFilter();
                formTypeConfigFilter.IS_ACTIVE = 1;
                ListFormTypeConfig = new BackendAdapter(new CommonParam()).Get<List<V_HIS_FORM_TYPE_CFG_DATA>>("api/HisFormTypeCfgData/GetView", ApiConsumer.ApiConsumers.MosConsumer, formTypeConfigFilter, null);
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

        private void SereServ__MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //gọi base dll
                Inventec.Common.Logging.LogSystem.Debug("SereServ__MouseRightClick.1");
                if (e.Item != null && e.Item.Tag != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("SereServ__MouseRightClick.2");
                    //var bbtnItem = sender as BarButtonItem;
                    FormOtherADO tag = (FormOtherADO)(e.Item.Tag);
                    Inventec.Common.Logging.LogSystem.Debug("SereServ__MouseRightClick.3");
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tag), tag));
                    if (tag != null)
                    {
                        FRD.ProcessorBase.FDO.FormInitFDO ado = new FRD.ProcessorBase.FDO.FormInitFDO();
                        if (tag.SarFormType != null)
                        {
                            ado.FormTypeCode = tag.SarFormType.FORM_TYPE_CODE;
                            ado.FormTypeName = tag.SarFormType.FORM_TYPE_NAME;
                            ado.IsOne = tag.SarFormType.IS_ONE;
                        }
                        Mps000332PDO mps000332PDO = new MPS.Processor.Mps000332.PDO.Mps000332PDO(null, tag.HisServiceReq, tag.HisSereServ, tag.HisSereServExt, tag.HisSereServPttt, null);
                        ado.Data = mps000332PDO;
                        if (tag.SarForm != null)
                        {
                            ado.FormId = tag.SarForm.ID;//Có thiof update
                            ado.IsEdit = true;//Sửa
                        }
                        ado.SarForms = this.ListForm;
                        ado.IsView = false;//xem
                        ado.delegateSelectFormId = UpdateJsonFormId_SereServ;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ado), ado));
                        FRD.FrdProcessor.Run(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SereServ__MenuClick(object sender, EventArgs e)
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
                        Mps000332PDO mps000332PDO = new MPS.Processor.Mps000332.PDO.Mps000332PDO(null, tag.HisServiceReq, tag.HisSereServ, tag.HisSereServExt, tag.HisSereServPttt, null);
                        if (ado.FormTypeCode == "Frd000003" || ado.FormTypeCode == "Frd000002" || ado.FormTypeCode == "Frd000005")//xuandv them de mo ra frm an toan PTTT,sơ kết bệnh án duyệt mổ
                            ado.Data = tag.HisServiceReq;
                        else
                            ado.Data = mps000332PDO;
                        if (tag.SarForm != null)
                        {
                            ado.FormId = tag.SarForm.ID;//Có thiof update
                            ado.IsEdit = true;//Sửa
                        }
                        ado.SarForms = this.ListForm;
                        ado.IsView = false;//xem
                        ado.delegateSelectFormId = UpdateJsonFormId_SereServ;
                        FRD.FrdProcessor.Run(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateJsonFormId_SereServ(long formId)
        {
            try
            {
                //if (this.CurrentSereServPttt != null)//xuandv bo k thay lam gi =))
                //{
                List<long> lstFormId = new List<long>();
                if (CurrentSereServExt != null)
                    lstFormId = GetListIdFormJson(CurrentSereServExt.JSON_FORM_ID);//TODO
                if (lstFormId == null) lstFormId = new List<long>();
                lstFormId.Add(formId);
                lstFormId = lstFormId.Distinct().ToList();

                string printIds = string.Join(",", lstFormId);
                HIS_SERE_SERV_EXT updateData = new HIS_SERE_SERV_EXT();

                updateData.JSON_FORM_ID = printIds;
                updateData.ID = CurrentSereServExt.ID;

                CommonParam param = new CommonParam();
                //bool success = false;
                //TODO
                var apiResult = new BackendAdapter(param).Post<HIS_SERE_SERV_EXT>("api/HisSereServExt/UpdateJsonForm", ApiConsumer.ApiConsumers.MosConsumer, updateData, SessionManager.ActionLostToken, param);
                if (apiResult == null || param.BugCodes.Count > 0 || param.Messages.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                }
                else
                {
                    this.CurrentSereServExt = apiResult;
                    if (RefreshList != null)
                    {
                        RefreshList();
                    }
                }

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
                //}
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
                    var groupFormTypeCode = ListFormTypeConfig.GroupBy(o => o.FORM_TYPE_CODE).ToList();
                    foreach (var grFormType in groupFormTypeCode)
                    {
                        if (!grFormType.ToList().Exists(o => o.FORM_TYPE_CFG_CODE == "IS_FOR_SERE_SERV_PTTT")) continue;

                        bool isAdd = true;
                        //foreach (var item in grFormType)
                        //{
                        //    if (!isAdd) break;

                        //    //Bo qua khi khong co loai
                        //    if (item.FORM_TYPE_CFG_CODE == "SERVICE_TYPE_IDS" &&
                        //         !String.IsNullOrWhiteSpace(item.VALUE) &&
                        //        (!GetListIdFormJson(item.VALUE).Contains(CurrentSereServPttt.TDL_SERVICE_TYPE_ID)))
                        //        isAdd = false;
                        //}

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
    }
}
