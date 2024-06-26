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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.RepartitionRatioCreate.ADO;
using HTC.EFMODEL.DataModels;
using HTC.Filter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RepartitionRatioCreate
{
    public partial class frmRepartitionRatioCreate : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;

        HTC_PERIOD currentPeriod = null;

        bool isUpdate = false;

        List<HtcRepartitionRatioADO> listRepartitionRatioAdo = new List<HtcRepartitionRatioADO>();
        BindingList<HtcRepartitionRatioADO> records;

        List<V_HTC_REPARTITION_RATIO> htcReparitionRatios;

        public frmRepartitionRatioCreate(Inventec.Desktop.Common.Modules.Module module, HTC_PERIOD period)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.currentModule = module;
                this.currentPeriod = period;
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
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmRepartitionRatioCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetValueControl();
                this.LoadDataToTreeList();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueControl()
        {
            try
            {
                if (this.currentPeriod != null)
                {
                    lblPeriod.Text = this.currentPeriod.PERIOD_CODE + " - " + this.currentPeriod.PERIOD_NAME;
                    lblYearMonth.Text = this.currentPeriod.MONTH + "/" + this.currentPeriod.YEAR;
                }
                else
                {
                    btnRefresh.Enabled = false;
                    btnSave.Enabled = false;
                    btnSelectPreviousPeriod.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToTreeList()
        {
            try
            {
                this.listRepartitionRatioAdo = new List<HtcRepartitionRatioADO>();
                if (this.currentPeriod != null)
                {
                    HtcRepartitionRatioViewFilter filter = new HtcRepartitionRatioViewFilter();
                    filter.PERIOD_ID = this.currentPeriod.ID;
                    filter.ORDER_DIRECTION = "ASC";
                    filter.ORDER_FIELD = "REPARTITION_TYPE_CODE";
                    htcReparitionRatios = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HTC_REPARTITION_RATIO>>("api/HtcRepartitionRatio/GetView", ApiConsumer.ApiConsumers.HtcConsumer, filter, null);
                    if (htcReparitionRatios != null && htcReparitionRatios.Count > 0)
                    {
                        this.isUpdate = true;
                        var Groups = htcReparitionRatios.GroupBy(g => g.REPARTITION_TYPE_ID).ToList();
                        foreach (var group in Groups)
                        {
                            List<V_HTC_REPARTITION_RATIO> listSub = group.ToList();
                            HtcRepartitionRatioADO parent = new HtcRepartitionRatioADO();
                            parent.TypeName = listSub.First().REPARTITION_TYPE_NAME;
                            parent.RepartitionTypeId = listSub.First().REPARTITION_TYPE_ID;
                            parent.AdoId = listSub.First().REPARTITION_TYPE_ID + "";
                            listRepartitionRatioAdo.Add(parent);
                            foreach (var item in listSub)
                            {
                                HtcRepartitionRatioADO child = new HtcRepartitionRatioADO();
                                child.DepartmentCode = item.DEPARTMENT_CODE;
                                child.TypeName = item.DEPARTMENT_NAME;
                                child.Id = item.ID;
                                child.AdoId = parent.AdoId + "_" + item.DEPARTMENT_CODE;
                                child.Ratio = item.RATIO * 100;
                                child.ParentId = parent.RepartitionTypeId;
                                child.AdoParentId = parent.AdoId;
                                child.RepartitionTypeId = -1;
                                child.CreateTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.CREATE_TIME ?? 0);
                                child.Creator = item.CREATOR;
                                child.CreateTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.MODIFY_TIME ?? 0);
                                child.Creator = item.MODIFIER;
                                listRepartitionRatioAdo.Add(child);
                            }
                        }
                    }
                    else
                    {
                        HtcRepartitionTypeFilter typeFilter = new HtcRepartitionTypeFilter();
                        typeFilter.ORDER_DIRECTION = "ASC";
                        typeFilter.ORDER_FIELD = "REPARTITION_TYPE_CODE";
                        List<HTC_REPARTITION_TYPE> htcRepartitionTypes = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HTC_REPARTITION_TYPE>>("api/HtcRepartitionType/Get", ApiConsumer.ApiConsumers.HtcConsumer, typeFilter, null);
                        if (htcRepartitionTypes != null)
                        {
                            foreach (var type in htcRepartitionTypes)
                            {
                                if (type.IS_HAS_NOT_RATIO == 1)
                                    continue;
                                HtcRepartitionRatioADO parent = new HtcRepartitionRatioADO();
                                parent.TypeName = type.REPARTITION_TYPE_NAME;
                                parent.RepartitionTypeId = type.ID;
                                parent.AdoId = type.ID + "";
                                listRepartitionRatioAdo.Add(parent);
                                foreach (var item in BackendDataWorker.Get<HIS_DEPARTMENT>())
                                {
                                    HtcRepartitionRatioADO child = new HtcRepartitionRatioADO();
                                    child.DepartmentCode = item.DEPARTMENT_CODE;
                                    child.TypeName = item.DEPARTMENT_NAME;
                                    child.ParentId = parent.RepartitionTypeId;
                                    child.Ratio = 0;
                                    child.AdoId = parent.AdoId + "_" + item.DEPARTMENT_CODE;
                                    child.AdoParentId = parent.AdoId;
                                    listRepartitionRatioAdo.Add(child);
                                }
                            }
                        }
                    }
                }

                this.records = new BindingList<HtcRepartitionRatioADO>(listRepartitionRatioAdo);
                treeListRepartitionRatio.DataSource = this.records;
                treeListRepartitionRatio.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListRepartitionRatio_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            try
            {
                var data = (HtcRepartitionRatioADO)treeListRepartitionRatio.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    data.IsUpdate = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListRepartitionRatio_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = (HtcRepartitionRatioADO)treeListRepartitionRatio.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (!data.ParentId.HasValue || this.currentPeriod.IS_ACTIVE != IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        e.RepositoryItem = repositoryItemSpinRatioDisable;
                    }
                    else if (this.currentPeriod.IS_ACTIVE == IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        e.RepositoryItem = repositoryItemSpinRatioEnable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListRepartitionRatio_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (HtcRepartitionRatioADO)treeListRepartitionRatio.GetDataRecordByNode(e.Node);
                if (data != null && !data.ParentId.HasValue)
                {
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSelectPreviousPeriod_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSelectPreviousPeriod.Enabled) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HTC_PERIOD previous = new Inventec.Common.Adapter.BackendAdapter(param).Get<HTC_PERIOD>("api/HtcPeriod/GetPrevious", ApiConsumer.ApiConsumers.HtcConsumer, this.currentPeriod.ID, param);
                if (previous != null)
                {
                    HtcRepartitionRatioViewFilter filter = new HtcRepartitionRatioViewFilter();
                    filter.PERIOD_ID = previous.ID;
                    filter.ORDER_DIRECTION = "ASC";
                    filter.ORDER_DIRECTION = "REPARTITION_TYPE_CODE";
                    List<V_HTC_REPARTITION_RATIO> listRatio = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HTC_REPARTITION_RATIO>>("api/HtcRepartitionRatio/GetView", ApiConsumer.ApiConsumers.HtcConsumer, filter, null);
                    if (listRatio != null && listRatio.Count > 0)
                    {
                        success = true;
                        foreach (var item in listRepartitionRatioAdo)
                        {
                            if (!item.ParentId.HasValue) continue;
                            var pre = listRatio.FirstOrDefault(o => o.REPARTITION_TYPE_ID == item.ParentId.Value && o.DEPARTMENT_CODE == item.DepartmentCode);
                            if (pre != null)
                            {
                                item.Ratio = pre.RATIO * 100;
                                item.IsUpdate = true;
                            }
                        }
                        treeListRepartitionRatio.RefreshDataSource();
                    }
                    else
                    {
                        param.Messages.Add("Kỳ tài chính trươc không có tỷ lệ phân phối");
                    }
                }
                else
                {
                    param.Messages.Add("Không lấy được kỳ tài chính trước");
                }
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.ShowAlert(this, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRefresh.Enabled) return;
                WaitingManager.Show();
                this.SetValueControl();
                this.LoadDataToTreeList();
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
                if (!btnSave.Enabled) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                List<HTC_REPARTITION_RATIO> listData = new List<HTC_REPARTITION_RATIO>();
                if (this.isUpdate)
                {
                    var listUpdate = this.listRepartitionRatioAdo.Where(o => o.ParentId.HasValue && o.IsUpdate.HasValue && o.IsUpdate.Value).ToList();
                    if (listUpdate != null && listUpdate.Count > 0)
                    {
                        foreach (var item in listUpdate)
                        {
                            var update = htcReparitionRatios.FirstOrDefault(o => o.ID == item.Id);
                            if (update != null)
                            {
                                Mapper.CreateMap<V_HTC_REPARTITION_RATIO, HTC_REPARTITION_RATIO>();
                                HTC_REPARTITION_RATIO data = Mapper.Map<HTC_REPARTITION_RATIO>(update);
                                data.RATIO = (item.Ratio ?? 0) / 100;
                                listData.Add(data);
                            }
                        }
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HTC_REPARTITION_RATIO>>("api/HtcRepartitionRatio/UpdateList", ApiConsumer.ApiConsumers.HtcConsumer, listData, param);
                        if (rs != null && rs.Count > 0)
                        {
                            success = true;
                        }
                    }
                }
                else
                {
                    var listGroup = this.listRepartitionRatioAdo.Where(o => o.ParentId.HasValue).GroupBy(g => g.DepartmentCode).ToList();
                    bool valid = true;
                    foreach (var group in listGroup)
                    {
                        var listSub = group.ToList();
                        //if (group.Sum(s => s.Ratio) != 100)
                        //{
                        //    valid = false;
                        //    param.Messages.Add("Tổng tỷ lệ của khoa " + group.FirstOrDefault().TypeName + " không = 100%");
                        //    break;
                        //}
                        foreach (var item in listSub)
                        {
                            HTC_REPARTITION_RATIO data = new HTC_REPARTITION_RATIO();
                            data.PERIOD_ID = this.currentPeriod.ID;
                            data.REPARTITION_TYPE_ID = item.ParentId.Value;
                            data.DEPARTMENT_CODE = item.DepartmentCode;
                            data.DEPARTMENT_NAME = item.TypeName;
                            data.RATIO = (item.Ratio ?? 0) / 100;
                            listData.Add(data);
                        }
                    }
                    if (valid)
                    {
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HTC_REPARTITION_RATIO>>("api/HtcRepartitionRatio/CreateList", ApiConsumer.ApiConsumers.HtcConsumer, listData, param);
                        if (rs != null && rs.Count > 0)
                        {
                            success = true;
                        }
                    }
                }
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.ShowAlert(this, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
