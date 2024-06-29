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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.BrowseExportTicket.ADO;
using HIS.Desktop.Plugins.BrowseExportTicket.Resources;
using HIS.Desktop.Plugins.BrowseExportTicket.Validation;
using HIS.Desktop.Plugins.TestServiceReqExcute.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BrowseExportTicket
{
    public partial class frmBrowseExportTicket : HIS.Desktop.Utility.FormBase
    {
        private void FillDataToGridExpMestBlty()
        {
            try
            {
                listExpMestBlty = new List<V_HIS_EXP_MEST_BLTY_REQ_1>();
                HisExpMestBltyReqView1Filter expMestBltyFilter = new HisExpMestBltyReqView1Filter();
                expMestBltyFilter.EXP_MEST_ID = this.expMestId;
                listExpMestBlty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ_1>>("api/HisExpMestBltyReq/GetView1", ApiConsumers.MosConsumer, expMestBltyFilter, null);
                gridControlExpMestBlty.BeginUpdate();
                gridControlExpMestBlty.DataSource = listExpMestBlty;
                gridControlExpMestBlty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBloodAndPatyMediStockId()
        {
            try
            {
                List<HIS_SERVICE_PATY> servicePatys = new List<HIS_SERVICE_PATY>();
                listBlood = new List<V_HIS_BLOOD>();
                HisBloodViewFilter bloodFilter = new HisBloodViewFilter();
                bloodFilter.MEDI_STOCK_ID = this.mediStockId;
                bloodFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                listBlood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BLOOD>>("api/HisBlood/GetView", ApiConsumers.MosConsumer, bloodFilter, null);
                if (listBlood != null && listBlood.Count > 0)
                {
                    var listServiceId = listBlood.Select(p => p.SERVICE_ID).Distinct().ToList();

                    var skip = 0;
                    while (listServiceId.Count - skip > 0)
                    {
                        var limit = listServiceId.Skip(skip).Take(100).ToList();
                        skip += 100;
                        MOS.Filter.HisServicePatyFilter filter = new HisServicePatyFilter();
                        filter.SERVICE_IDs = limit;
                        var servicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_PATY>>("api/HisServicePaty/Get", ApiConsumers.MosConsumer, filter, null);

                        servicePatys.AddRange(servicePaty);
                    }

                    if (servicePatys != null && servicePatys.Count > 0)
                    {
                        List<long> serviceIds = servicePatys.Select(p => p.SERVICE_ID).ToList();
                        listBlood = listBlood.Where(p => serviceIds.Contains(p.SERVICE_ID)).ToList();
                    }
                    else
                    {
                        listBlood = new List<V_HIS_BLOOD>();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataBlood()
        {
            try
            {
                dicCurrentBlood = new Dictionary<long, V_HIS_BLOOD>();
                dicShowBlood = new Dictionary<long, V_HIS_BLOOD>();
                dicBloodCode = new Dictionary<string, V_HIS_BLOOD>();
                if (listBlood != null && listBlood.Count > 0)
                {
                    foreach (var item in listBlood)
                    {
                        dicBloodCode[item.BLOOD_CODE] = item;
                        dicCurrentBlood[item.ID] = item;
                        if (!dicBloodAdo.ContainsKey(item.ID))
                            dicShowBlood[item.ID] = item;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridBlood()
        {
            try
            {
                lciForbtnAssignPresDTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciForbtnAssignService.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                gridControlBlood.BeginUpdate();
                if (listExpMestBlty == null || listExpMestBlty.Count == 0)
                {
                    return;
                }
                var dataGrid = dicShowBlood.Select(s => s.Value).OrderBy(o =>
                  o.BLOOD_TYPE_NAME).ThenBy(o => o.VOLUME).ThenBy(o => o.EXPIRED_DATE).ThenBy(o => o.BLOOD_CODE).ToList();

                if (chkExpiryDate.Checked)
                {
                    dataGrid = dataGrid.Where(o => o.EXPIRED_DATE > 0 && Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(o.EXPIRED_DATE ?? 0) > DateTime.Now).ToList();
                }

                if (chkShowBloodType.Checked && dataGrid.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    if (cboBloodType.EditValue != null)
                    {
                        HisBloodTypeFilter BloodFilter = new HisBloodTypeFilter();
                        BloodFilter.ID = (long?)cboBloodType.EditValue;
                        var objBloodType = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>>
                      ("api/HisBloodType/Get", ApiConsumer.ApiConsumers.MosConsumer, BloodFilter, param);
                        if (objBloodType != null && objBloodType.Count > 0)
                        {
                            var bt = objBloodType[0];
                            if (bt.PREPARATIONS_BLOOD_ID != null)
                            {
                                HisBltyPreparationsFilter filter = new HisBltyPreparationsFilter();
                                filter.PREPARATIONS_BLOOD_ID = bt.PREPARATIONS_BLOOD_ID;
                                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLTY_PREPARATIONS>>
                              ("api/HisBltyPreparations/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                                if (obj != null && obj.Count > 0)
                                {
                                    dataGrid = dataGrid.Where(o => obj.Exists(p => p.RECEIVE_BLOOD_ABO_CODE == o.BLOOD_ABO_CODE && p.RECEIVE_BLOOD_RH_CODE == o.BLOOD_RH_CODE)).ToList();
                                }
                            }
                        }
                    }
                }

                gridControlBlood.DataSource = dataGrid;
                gridControlBlood.EndUpdate();

                lciForbtnAssignPresDTT.Visibility = (dataGrid != null && dataGrid.Count > 0 && ChmsExpMest != null && ChmsExpMest.TDL_TREATMENT_ID.HasValue && ChmsExpMest.TDL_TREATMENT_ID.Value > 0) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciForbtnAssignService.Visibility = (dataGrid != null && dataGrid.Count > 0 && ChmsExpMest != null && ChmsExpMest.TDL_TREATMENT_ID.HasValue && ChmsExpMest.TDL_TREATMENT_ID.Value > 0) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridExpMestBlood()
        {
            try
            {
                gridControlExpMestBlood.BeginUpdate();
                gridControlExpMestBlood.DataSource = dicBloodAdo.Select(s => s.Value).ToList();
                gridControlExpMestBlood.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlByExpMestBlty()
        {
            try
            {
                if (this.currentBlty != null)
                {
                    var bloodType = BackendDataWorker.Get<HIS_BLOOD_TYPE>().Where(o => o.ID == this.currentBlty.BLOOD_TYPE_ID);
                    lblBloodTypeInfo.Text = bloodType.FirstOrDefault().BLOOD_TYPE_CODE + " - " + bloodType.FirstOrDefault().BLOOD_TYPE_NAME;
                }
                else
                {
                    lblBloodTypeInfo.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void setGridLookUpClick()
        {
            try
            {
                gridLookUpVolume.EditValue = this.currentBlty.BLOOD_VOLUME_ID;
                gridLookUpBloodAboCode.EditValue = this.currentBlty.BLOOD_ABO_ID;
                gridLookUpBloodRhCode.EditValue = this.currentBlty.BLOOD_RH_ID;
                cboBloodType.EditValue = this.currentBlty.BLOOD_TYPE_ID;
                checkBtnRefresh = false;
                fillDataGridViewBlood();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void fillDataGridViewBlood()
        {
            try
            {
                //Review 
                List<V_HIS_BLOOD> dataNew = new List<V_HIS_BLOOD>();
                if (this.listBlood != null && this.listBlood.Count > 0)
                {
                    dataNew.AddRange(this.listBlood);
                    long maxId = 0;
                    if (bloodVolume != null && bloodVolume.Count > 0)
                    {
                        var listID = bloodVolume.Select(o => o.ID).ToList();
                        maxId = listID.Max();
                        if (gridLookUpVolume.EditValue != null)
                        {
                            long bloodVolumeId = Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUpVolume.EditValue.ToString());
                            if (bloodVolumeId != maxId)
                                dataNew = dataNew.Where(p => p.BLOOD_VOLUME_ID == bloodVolumeId).ToList();
                        }

                    }
                }

                if (cboBloodType.EditValue != null)
                {
                    long bloodTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodType.EditValue.ToString());
                    dataNew = dataNew.Where(p => p.BLOOD_TYPE_ID == bloodTypeId).ToList();
                }
                List<V_HIS_BLOOD> dataGrid = new List<V_HIS_BLOOD>();
                if (dataNew != null && dataNew.Count > 0 && this.dicShowBlood != null && this.dicShowBlood.Count > 0)
                {
                    List<long> bloodIds = dataNew.Select(p => p.ID).ToList();
                    var aboCode = gridLookUpBloodAboCode.EditValue == null ? 0 : Convert.ToInt64(gridLookUpBloodAboCode.EditValue);
                    if (aboCode != 0)
                    {
                        dataGrid = this.dicShowBlood.Where(p => bloodIds.Contains(p.Key))
                                                    .Select(p => p.Value)
                                                    .Where(p => p.BLOOD_ABO_ID == Convert.ToInt64(gridLookUpBloodAboCode.EditValue))
                                                    .OrderBy(o => o.BLOOD_TYPE_NAME).ThenBy(o=>o.BLOOD_ABO_CODE).ThenBy(o => o.VOLUME).ThenBy(o => o.EXPIRED_DATE).ThenBy(o => o.BLOOD_CODE)
                                                    .ToList();
                    }
                    else
                    {
                        dataGrid = this.dicShowBlood.Where(p => bloodIds.Contains(p.Key))
                                                    .Select(p => p.Value)
                                                    .OrderBy(o => o.BLOOD_TYPE_NAME).ThenBy(o => o.BLOOD_ABO_CODE).ThenBy(o => o.VOLUME).ThenBy(o => o.EXPIRED_DATE).ThenBy(o => o.BLOOD_CODE)
                                                    .ToList();
                    }
                    
                }
                if (chkExpiryDate.Checked)
                {
                    dataGrid = dataGrid.Where(o => o.EXPIRED_DATE > 0 && Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(o.EXPIRED_DATE ?? 0) > DateTime.Now).ToList();
                }
                bool IsLoadShowBloodType = false;
                if (chkShowBloodType.Checked && dataGrid.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    if (cboBloodType.EditValue != null)
                    {
                        HisBloodTypeFilter BloodFilter = new HisBloodTypeFilter();
                        BloodFilter.ID = (long?)cboBloodType.EditValue;
                        var objBloodType = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>>
                      ("api/HisBloodType/Get", ApiConsumer.ApiConsumers.MosConsumer, BloodFilter, param);
                        if(objBloodType != null && objBloodType.Count > 0)
                        {
                            var bt = objBloodType[0];
                            if (bt.PREPARATIONS_BLOOD_ID != null)
                            {
                                HisBltyPreparationsFilter filter = new HisBltyPreparationsFilter();
                                filter.PREPARATIONS_BLOOD_ID = bt.PREPARATIONS_BLOOD_ID;
                                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLTY_PREPARATIONS>>
                              ("api/HisBltyPreparations/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                                if(obj != null && obj.Count > 0)
                                {
                                    if (gridLookUpBloodAboCode.EditValue != null)
                                    {
                                        long bloodAboId = Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUpBloodAboCode.EditValue.ToString());
                                        var data = gridLookUpBloodAboCode.Properties.DataSource as List<HIS_BLOOD_ABO>;
                                        obj = obj.Where(o => o.PROVIDE_BLOOD_ABO_CODE == data.FirstOrDefault(p=>p.ID == bloodAboId).BLOOD_ABO_CODE).ToList();
                                    }
                                    if (gridLookUpBloodRhCode.EditValue != null)
                                    {
                                        long bloodRhId = Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUpBloodRhCode.EditValue.ToString());
                                        var data = gridLookUpBloodRhCode.Properties.DataSource as List<HIS_BLOOD_RH>;
                                        obj = obj.Where(o => o.PROVIDE_BLOOD_RH_CODE == data.FirstOrDefault(p => p.ID == bloodRhId).BLOOD_RH_CODE).ToList();
                                    }
                                    if (obj != null && obj.Count > 0)
                                        dataGrid = dataGrid.Where(o => obj.Exists(p => p.RECEIVE_BLOOD_ABO_CODE == o.BLOOD_ABO_CODE && p.RECEIVE_BLOOD_RH_CODE == o.BLOOD_RH_CODE)).ToList();
                                    else
                                        dataGrid = new List<V_HIS_BLOOD>();
                                    IsLoadShowBloodType = true;
                                }
                            }
                        }    
                    }
                }
                if (!IsLoadShowBloodType)
                {
                    if (gridLookUpBloodAboCode.EditValue != null)
                    {
                        long bloodAboId = Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUpBloodAboCode.EditValue.ToString());
                        dataNew = dataNew.Where(p => p.BLOOD_ABO_ID == bloodAboId).ToList();
                    }
                    if (gridLookUpBloodRhCode.EditValue != null)
                    {
                        long bloodRhId = Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUpBloodRhCode.EditValue.ToString());
                        dataNew = dataNew.Where(p => p.BLOOD_RH_ID == bloodRhId).ToList();
                    }
                }
                gridViewBlood.GridControl.DataSource = dataGrid;

                lciForbtnAssignPresDTT.Visibility = (dataGrid != null && dataGrid.Count > 0) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciForbtnAssignService.Visibility = (dataGrid != null && dataGrid.Count > 0) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region LoadCombo
        private void frmExpMestBlood_Plus_GridLookup()
        {
            try
            {
                initGridLookupVolume();
                initGridLookupAbo();
                initGridLookupRh();
                initGridLookupBloodType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupVolume()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodVolumeFilter filter = new MOS.Filter.HisBloodVolumeFilter();
                filter.KEY_WORD = gridLookUpVolume.Text;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "VOLUME";
                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME>>
                  ("api/HisBloodVolume/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                bloodVolume = new List<BloodVolumeADO>();
                BloodVolumeADO addAll = new BloodVolumeADO();
                addAll.ID = 0;// obj.FirstOrDefault().ID + 1;
                addAll.Blood_Volume_Str = "Tất cả";
                bloodVolume.Add(addAll);
                if (obj != null && obj.Count > 0)
                {
                    foreach (var item in obj)
                    {
                        BloodVolumeADO blood = new BloodVolumeADO(item);
                        bloodVolume.Add(blood);
                    }
                }

                // bloodVolume = bloodVolume.OrderBy(o => o.ID).ThenBy(p => p.VOLUME).ToList();

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("Blood_Volume_Str", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("Blood_Volume_Str", "ID", columnInfo, false);
                ControlEditorLoader.Load(gridLookUpVolume, bloodVolume, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupAbo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodAboFilter filter = new MOS.Filter.HisBloodAboFilter();

                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>>
                  ("api/HisBloodAbo/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfo, false);
                ControlEditorLoader.Load(gridLookUpBloodAboCode, obj, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupRh()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodRhFilter filter = new MOS.Filter.HisBloodRhFilter();

                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>>
                  ("api/HisBloodRh/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("BLOOD_RH_CODE", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfo, false);
                ControlEditorLoader.Load(gridLookUpBloodRhCode, obj, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupBloodType()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodTypeFilter filter = new MOS.Filter.HisBloodTypeFilter();

                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>>
                  ("api/HisBloodType/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("BLOOD_TYPE_CODE", "", 0, 1));
                columnInfo.Add(new ColumnInfo("BLOOD_TYPE_NAME", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("BLOOD_TYPE_NAME", "ID", columnInfo, false);
                ControlEditorLoader.Load(cboBloodType, obj, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region GridBlood_Control
        private void gridViewExpMestBlty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_EXP_MEST_BLTY_REQ_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void gridViewBlood_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_BLOOD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            if (data.EXPIRED_DATE > 0)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        else if (e.Column.FieldName == "IMP_TIME_STR")
                        {
                            if (data.IMP_TIME > 0)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_TIME ?? 0);
                            }
                            else
                            {
                                e.Value = "";
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

        private void gridViewExMestBlood_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisBloodADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "SALT_ENVI_STR")
                        {
                            try
                            {
                                e.Value = data.SALT_ENVI;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "ANTI_GLOBULIN_ENVI_STR")
                        {
                            try
                            {
                                e.Value = data.ANTI_GLOBULIN_ENVI;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "AC_SELF_ENVIDENCE_STR")
                        {
                            try
                            {
                                e.Value = data.AC_SELF_ENVIDENCE;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void gridViewExMestBlood_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                if (e.Column.FieldName == "EXPIRED_DATE_STR")
                {
                    gridViewExMestBlood.PostEditor();
                    DateTime? dt = null;
                    if (gridViewExMestBlood.EditingValue != null)
                    {
                        dt = (DateTime)gridViewExMestBlood.EditingValue;
                    }
                    if (!dt.HasValue || dt.Value == DateTime.MinValue)
                    {
                        MessageManager.Show(ResourceMessage.TruongDuLieuBatBuoc);
                    }
                    else if (dt.Value < DateTime.Now)
                    {
                        MessageManager.Show(ResourceMessage.HanDungKhongDuocBeHonHienTai);
                    }
                    else
                    {
                        var data = (VHisBloodADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                        if (data != null)
                        {
                            data.EXPIRED_DATE = Convert.ToInt64(dt.Value.ToString("yyyyMMdd") + "235959");
                        }
                    }

                }
                if (e.Column.FieldName == "SALT_ENVI_STR")
                {
                    gridViewExMestBlood.PostEditor();
                    long value = 0;
                    if (gridViewExMestBlood.EditingValue != null)
                    {
                        value = (long)gridViewExMestBlood.EditingValue;
                        var data = (VHisBloodADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                        if (data != null)
                        {
                            data.SALT_ENVI = value;
                        }
                    }
                    else
                    {

                    }
                }
                if (e.Column.FieldName == "ANTI_GLOBULIN_ENVI_STR")
                {
                    gridViewExMestBlood.PostEditor();
                    long value = 0;
                    if (gridViewExMestBlood.EditingValue != null)
                    {
                        value = (long)gridViewExMestBlood.EditingValue;
                        var data = (VHisBloodADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                        if (data != null)
                        {
                            data.ANTI_GLOBULIN_ENVI = value;
                        }
                    }
                    else
                    {

                    }
                }
                if (e.Column.FieldName == "AC_SELF_ENVIDENCE_STR")
                {
                    gridViewExMestBlood.PostEditor();
                    decimal value = 0;
                    if (gridViewExMestBlood.EditingValue != null)
                    {
                        value = decimal.Parse(gridViewExMestBlood.EditingValue.ToString());
                        var data = (VHisBloodADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                        if (data != null)
                        {
                            data.AC_SELF_ENVIDENCE = value;
                        }
                    }
                    else
                    {

                    }
                }
                gridControlExpMestBlood.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlty_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentBlty = null;
                this.currentBlty = (V_HIS_EXP_MEST_BLTY_REQ_1)gridViewExpMestBlty.GetFocusedRow();

                if (ShowTestResult)
                    LoadDataToGridResult(currentBlty);


                this.SetControlByExpMestBlty();
                setGridLookUpClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void LoadDataToGridResult(V_HIS_EXP_MEST_BLTY_REQ_1 currentBlty)
        {

            try
            {
                //    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("currentBlty__", currentBlty));

                HisServiceReqViewFilter filterServiceReq = new HisServiceReqViewFilter();
                filterServiceReq.PARENT_ID = currentBlty.SERVICE_REQ_ID;
                _ServiceReqs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filterServiceReq, null);
                if (_ServiceReqs != null && _ServiceReqs.Count() > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisSereServViewFilter filter = new MOS.Filter.HisSereServViewFilter();
                    filter.SERVICE_REQ_IDs = _ServiceReqs.Select(o => o.ID).ToList();
                    filter.HAS_EXECUTE = true;
                    lstSereServ = new List<HIS_SERE_SERV>();
                    lstSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filter, param);
                    List<long> lstSereServIds = new List<long>();
                    List<long> _ServiceIds = new List<long>();
                    if (lstSereServ != null && lstSereServ.Count > 0)
                    {
                        lstSereServ = lstSereServ.OrderBy(o => o.ID).ThenBy(p => p.TDL_SERVICE_NAME).ToList();
                        lstSereServIds = lstSereServ.Select(p => p.ID).ToList();
                        _ServiceIds = lstSereServ.Select(p => p.SERVICE_ID).ToList();
                        //Lấy cấu hình dịch vụ máy
                        _ServiceMachines = new List<HIS_SERVICE_MACHINE>();
                        MOS.Filter.HisServiceMachineFilter _machineFilter = new HisServiceMachineFilter();
                        _machineFilter.SERVICE_IDs = _ServiceIds;
                        _machineFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        _ServiceMachines = new BackendAdapter(param).Get<List<HIS_SERVICE_MACHINE>>("api/HisServiceMachine/Get", ApiConsumers.MosConsumer, _machineFilter, param);
                        //Lay ss dc luu trong ss ext
                        MOS.Filter.HisSereServExtFilter ssFilter = new HisSereServExtFilter();
                        ssFilter.SERE_SERV_IDs = lstSereServIds;
                        ssFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                        var _SereServExts = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("/api/HisSereServExt/Get", ApiConsumers.MosConsumer, ssFilter, param);

                        //Lay HIS_TEST_INDEX  #2170
                        MOS.Filter.HisTestIndexViewFilter _TestIndexFilter = new HisTestIndexViewFilter();
                        _TestIndexFilter.SERVICE_IDs = _ServiceIds;
                        _TestIndexFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        var _TestIndexs = new BackendAdapter(param).Get<List<V_HIS_TEST_INDEX>>("/api/HisTestIndex/GetView", ApiConsumers.MosConsumer, _TestIndexFilter, param);


                        HisSereServTeinViewFilter sereSerTeinFilter = new HisSereServTeinViewFilter();
                        sereSerTeinFilter.SERE_SERV_IDs = lstSereServIds;
                        sereSerTeinFilter.ORDER_FIELD = "NUM_ORDER";
                        sereSerTeinFilter.ORDER_DIRECTION = "DESC";
                        sereSerTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        var lstSereServTeinItem = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>(HisRequestUriStore.HIS_SERE_SERV_TEIN_GET, ApiConsumers.MosConsumer, sereSerTeinFilter, param);


                        lstHisSereServTeinSDO = new List<HisSereServTeinSDO>();

                        var sereServGroup = lstSereServ.GroupBy(o => o.TDL_SERVICE_CODE).ToList();
                        foreach (var group in sereServGroup)
                        {
                            var item = group.First();
                            HisSereServTeinSDO hisSereServTeinSDO = new HisSereServTeinSDO();
                            hisSereServTeinSDO.IS_PARENT = 1;
                            hisSereServTeinSDO.TEST_INDEX_CODE = item.TDL_SERVICE_CODE;
                            hisSereServTeinSDO.TEST_INDEX_NAME = item.TDL_SERVICE_NAME;
                            hisSereServTeinSDO.TDL_SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                            hisSereServTeinSDO.SERE_SERV_ID = item.ID;
                            hisSereServTeinSDO.SERVICE_ID = item.SERVICE_ID;

                            if (_ServiceMachines != null && _ServiceMachines.Count > 0)
                            {
                                var ssMachine = _ServiceMachines.Where(p => p.SERVICE_ID == item.SERVICE_ID).ToList();
                                if (ssMachine != null && ssMachine.Count == 1)
                                {
                                    hisSereServTeinSDO.MACHINE_ID = ssMachine[0].MACHINE_ID;
                                }
                            }

                            if (_SereServExts != null && _SereServExts.Count > 0)
                            {
                                var data = _SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                                if (data != null)
                                {
                                    hisSereServTeinSDO.MACHINE_ID = data.MACHINE_ID;
                                }
                            }

                            var listSereServTeinByServServ = lstSereServTeinItem.Where(o => o.SERE_SERV_ID == item.ID).ToList();

                            if (listSereServTeinByServServ != null
                                && listSereServTeinByServServ.Count == 1
                                && listSereServTeinByServServ[0].IS_NOT_SHOW_SERVICE == 1)
                            {
                                hisSereServTeinSDO.HAS_ONE_CHILD = 1;
                                if (!String.IsNullOrEmpty(listSereServTeinByServServ[0].VALUE) && listSereServTeinByServServ[0].CREATE_TIME != listSereServTeinByServServ[0].MODIFY_TIME)
                                {
                                    hisSereServTeinSDO.VALUE = listSereServTeinByServServ[0].VALUE;
                                }
                                else if (listSereServTeinByServServ[0].CREATE_TIME == listSereServTeinByServServ[0].MODIFY_TIME)
                                {
                                    var testIndex = _TestIndexs.Where(o => o.SERVICE_CODE == item.TDL_SERVICE_CODE) != null ? _TestIndexs.Where(o => o.SERVICE_CODE == item.TDL_SERVICE_CODE).FirstOrDefault() : null;
                                    if (testIndex != null && testIndex.IS_TEST_HARMONY_BLOOD == 1)
                                    {
                                        hisSereServTeinSDO.VALUE = expMest.BLOOD_LEVEL != 1 ? "Âm tính" : "";
                                    }
                                    else if (testIndex != null && testIndex.IS_BLOOD_ABO == 1)
                                        hisSereServTeinSDO.VALUE = currentBlty.BLOOD_ABO_CODE;
                                    else if (testIndex != null && testIndex.IS_BLOOD_RH == 1)
                                        hisSereServTeinSDO.VALUE = currentBlty.BLOOD_RH_CODE;
                                }
                                // hisSereServTeinSDO.DESCRIPTION = listSereServTeinByServServ[0].DESCRIPTION;
                                hisSereServTeinSDO.SERE_SERV_ID = listSereServTeinByServServ[0].SERE_SERV_ID;
                                hisSereServTeinSDO.TDL_SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                                hisSereServTeinSDO.TEST_INDEX_ID = listSereServTeinByServServ[0].TEST_INDEX_ID;
                                hisSereServTeinSDO.TEST_INDEX_CODE = listSereServTeinByServServ[0].TEST_INDEX_CODE;
                                hisSereServTeinSDO.TEST_INDEX_NAME = listSereServTeinByServServ[0].TEST_INDEX_NAME;
                                hisSereServTeinSDO.TEST_INDEX_UNIT_NAME = listSereServTeinByServServ[0].TEST_INDEX_UNIT_NAME;
                                hisSereServTeinSDO.RESULT_CODE = listSereServTeinByServServ[0].RESULT_CODE;
                                hisSereServTeinSDO.IS_IMPORTANT = listSereServTeinByServServ[0].IS_IMPORTANT;
                                if (!String.IsNullOrEmpty(listSereServTeinByServServ[0].NOTE))
                                {
                                    hisSereServTeinSDO.NOTE = listSereServTeinByServServ[0].NOTE;
                                }
                                else
                                {
                                    if (_SereServExts != null && _SereServExts.Count > 0)
                                    {
                                        var lstSereServExt = _SereServExts.Where(o => o.SERE_SERV_ID == listSereServTeinByServServ[0].SERE_SERV_ID).ToList();
                                        if (true)
                                        {
                                            hisSereServTeinSDO.NOTE = lstSereServExt.FirstOrDefault().INSTRUCTION_NOTE;
                                        }
                                    }
                                }
                                hisSereServTeinSDO.LEAVEN = listSereServTeinByServServ[0].LEAVEN;
                                lstHisSereServTeinSDO.Add(hisSereServTeinSDO);
                            }
                            else if (listSereServTeinByServServ != null && (listSereServTeinByServServ.Count > 1 || (listSereServTeinByServServ.Count == 1 && listSereServTeinByServServ[0].IS_NOT_SHOW_SERVICE != 1)))
                            {
                                lstHisSereServTeinSDO.Add(hisSereServTeinSDO);
                                //var dataTeins = listSereServTeinByServServ.Where(p => p.SERE_SERV_ID == item.ID).ToList();
                                List<V_HIS_TEST_INDEX> dataTestIndexs = new List<V_HIS_TEST_INDEX>();
                                if (_TestIndexs != null && _TestIndexs.Count > 0)
                                {
                                    dataTestIndexs = _TestIndexs.Where(p => p.TEST_SERVICE_TYPE_ID == item.SERVICE_ID).ToList();
                                    if (listSereServTeinByServServ != null && listSereServTeinByServServ.Count > 0 && dataTestIndexs != null && dataTestIndexs.Count > 0)
                                    {
                                        dataTestIndexs = dataTestIndexs.Where(p => !listSereServTeinByServServ.Select(o => o.TEST_INDEX_ID).Contains(p.ID)).ToList();
                                    }
                                }

                                foreach (var ssTein in listSereServTeinByServServ)
                                {
                                    HisSereServTeinSDO hisSereServTein = new HisSereServTeinSDO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HisSereServTeinSDO>(hisSereServTein, item);
                                    hisSereServTein.IS_PARENT = 0;
                                    hisSereServTein.HAS_ONE_CHILD = 0;
                                    if (!String.IsNullOrEmpty(ssTein.VALUE) && ssTein.CREATE_TIME != ssTein.MODIFY_TIME)
                                    {
                                        hisSereServTein.VALUE = ssTein.VALUE;
                                    }
                                    else if (ssTein.CREATE_TIME == ssTein.MODIFY_TIME)
                                    {
                                        var testIndex = _TestIndexs.Where(o => o.SERVICE_CODE == item.TDL_SERVICE_CODE) != null ? _TestIndexs.Where(o => o.SERVICE_CODE == item.TDL_SERVICE_CODE).FirstOrDefault() : null;
                                        if (testIndex != null && testIndex.IS_TEST_HARMONY_BLOOD == 1)
                                        {
                                            hisSereServTein.VALUE = expMest.BLOOD_LEVEL != 1 ? "Âm tính" : "";
                                        }
                                        else if (testIndex != null && testIndex.IS_BLOOD_ABO == 1)
                                            hisSereServTein.VALUE = currentBlty.BLOOD_ABO_CODE;
                                        else if (testIndex != null && testIndex.IS_BLOOD_RH == 1)
                                            hisSereServTein.VALUE = currentBlty.BLOOD_RH_CODE;
                                    }
                                    //hisSereServTein.DESCRIPTION = ssTein.DESCRIPTION;
                                    hisSereServTein.SERE_SERV_ID = ssTein.SERE_SERV_ID;
                                    hisSereServTein.TDL_SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                                    hisSereServTein.TEST_INDEX_ID = ssTein.TEST_INDEX_ID;
                                    hisSereServTein.TEST_INDEX_CODE = "        " + ssTein.TEST_INDEX_CODE;
                                    hisSereServTein.TEST_INDEX_NAME = ssTein.TEST_INDEX_NAME;
                                    hisSereServTein.TEST_INDEX_UNIT_NAME = ssTein.TEST_INDEX_UNIT_NAME;
                                    hisSereServTein.RESULT_CODE = ssTein.RESULT_CODE;
                                    if (!String.IsNullOrEmpty(ssTein.NOTE))
                                    {
                                        hisSereServTein.NOTE = ssTein.NOTE;
                                    }
                                    else
                                    {
                                        if (_SereServExts != null && _SereServExts.Count > 0)
                                        {
                                            var sereServExt = _SereServExts.Where(o => o.SERE_SERV_ID == ssTein.SERE_SERV_ID).ToList();
                                            if (sereServExt != null && sereServExt.Count() > 0)
                                            {
                                                hisSereServTein.NOTE = sereServExt.FirstOrDefault().INSTRUCTION_NOTE;
                                            }

                                        }
                                    }
                                    hisSereServTein.LEAVEN = ssTein.LEAVEN;
                                    hisSereServTein.IS_IMPORTANT = ssTein.IS_IMPORTANT;
                                    lstHisSereServTeinSDO.Add(hisSereServTein);
                                }
                                foreach (var itemTestIndex in dataTestIndexs)
                                {
                                    HisSereServTeinSDO hisSereServTein = new HisSereServTeinSDO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HisSereServTeinSDO>(hisSereServTein, item);
                                    hisSereServTein.IS_PARENT = 0;
                                    hisSereServTein.HAS_ONE_CHILD = 0;
                                    //hisSereServTein.DESCRIPTION = itemTestIndex.DEFAULT_VALUE;
                                    // hisSereServTein.SERE_SERV_ID = itemTestIndex.SERE_SERV_ID;

                                    if (itemTestIndex.IS_TEST_HARMONY_BLOOD == 1)
                                    {
                                        hisSereServTeinSDO.VALUE = expMest.BLOOD_LEVEL != 1 ? "Âm tính" : "";
                                    }
                                    else if (itemTestIndex.IS_BLOOD_ABO == 1)
                                        hisSereServTein.VALUE = currentBlty.BLOOD_ABO_CODE;
                                    else if (itemTestIndex.IS_BLOOD_RH == 1)
                                        hisSereServTein.VALUE = currentBlty.BLOOD_RH_CODE;
                                    hisSereServTein.TEST_INDEX_ID = itemTestIndex.ID;
                                    hisSereServTein.TEST_INDEX_CODE = "        " + itemTestIndex.TEST_INDEX_CODE;
                                    hisSereServTein.TEST_INDEX_NAME = itemTestIndex.TEST_INDEX_NAME;
                                    hisSereServTein.TEST_INDEX_UNIT_NAME = itemTestIndex.TEST_INDEX_UNIT_NAME;
                                    hisSereServTein.IS_IMPORTANT = itemTestIndex.IS_IMPORTANT;
                                    lstHisSereServTeinSDO.Add(hisSereServTein);
                                }
                            }
                            else if (_TestIndexs != null && _TestIndexs.Count > 0)
                            {
                                var dataTestIndexs = _TestIndexs.Where(p => p.TEST_SERVICE_TYPE_ID == item.SERVICE_ID).ToList();
                                if (dataTestIndexs != null
                                    && dataTestIndexs.Count == 1
                                    && dataTestIndexs[0].IS_NOT_SHOW_SERVICE == 1)
                                {
                                    HisSereServTeinSDO hisSereServTein = new HisSereServTeinSDO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HisSereServTeinSDO>(hisSereServTein, item);
                                    hisSereServTein.IS_PARENT = 0;
                                    hisSereServTein.HAS_ONE_CHILD = 0;
                                    //hisSereServTein.DESCRIPTION = dataTestIndexs[0].DEFAULT_VALUE;s
                                    if (dataTestIndexs[0].IS_TEST_HARMONY_BLOOD == 1)
                                    {
                                        hisSereServTein.VALUE = expMest.BLOOD_LEVEL != 1 ? "Âm tính" : "";
                                    }
                                    else if (dataTestIndexs[0].IS_BLOOD_ABO == 1)
                                        hisSereServTein.VALUE = currentBlty.BLOOD_ABO_CODE;
                                    else if (dataTestIndexs[0].IS_BLOOD_RH == 1)
                                        hisSereServTein.VALUE = currentBlty.BLOOD_RH_CODE;
                                    hisSereServTein.TEST_INDEX_ID = dataTestIndexs[0].ID;
                                    hisSereServTein.TEST_INDEX_CODE = "        " + dataTestIndexs[0].TEST_INDEX_CODE;
                                    hisSereServTein.TEST_INDEX_NAME = dataTestIndexs[0].TEST_INDEX_NAME;
                                    hisSereServTein.TEST_INDEX_UNIT_NAME = dataTestIndexs[0].TEST_INDEX_UNIT_NAME;
                                    hisSereServTein.IS_IMPORTANT = dataTestIndexs[0].IS_IMPORTANT;
                                    lstHisSereServTeinSDO.Add(hisSereServTein);
                                }
                                else if (dataTestIndexs != null && dataTestIndexs.Count > 0)
                                {
                                    lstHisSereServTeinSDO.Add(hisSereServTeinSDO);
                                    foreach (var itemTestIndex in dataTestIndexs)
                                    {
                                        HisSereServTeinSDO hisSereServTein = new HisSereServTeinSDO();
                                        Inventec.Common.Mapper.DataObjectMapper.Map<HisSereServTeinSDO>(hisSereServTein, item);
                                        hisSereServTein.IS_PARENT = 0;
                                        hisSereServTein.HAS_ONE_CHILD = 0;
                                        hisSereServTein.TDL_SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                                        if (itemTestIndex.IS_TEST_HARMONY_BLOOD == 1)
                                        {
                                            hisSereServTein.VALUE = expMest.BLOOD_LEVEL != 1 ? "Âm tính" : "";
                                        }
                                        else if (itemTestIndex.IS_BLOOD_ABO == 1)
                                            hisSereServTein.VALUE = currentBlty.BLOOD_ABO_CODE;
                                        else if (itemTestIndex.IS_BLOOD_RH == 1)
                                            hisSereServTein.VALUE = currentBlty.BLOOD_RH_CODE;
                                        //hisSereServTein.DESCRIPTION = itemTestIndex.DEFAULT_VALUE;
                                        hisSereServTein.TEST_INDEX_ID = itemTestIndex.ID;
                                        hisSereServTein.TEST_INDEX_CODE = "        " + itemTestIndex.TEST_INDEX_CODE;
                                        hisSereServTein.TEST_INDEX_NAME = itemTestIndex.TEST_INDEX_NAME;
                                        hisSereServTein.TEST_INDEX_UNIT_NAME = itemTestIndex.TEST_INDEX_UNIT_NAME;
                                        hisSereServTein.IS_IMPORTANT = itemTestIndex.IS_IMPORTANT;
                                        lstHisSereServTeinSDO.Add(hisSereServTein);
                                    }
                                }
                            }
                            else
                            {
                                lstHisSereServTeinSDO.Add(hisSereServTeinSDO);
                            }
                        }
                    }
                }

                // gán test index range
                if (lstHisSereServTeinSDO != null && lstHisSereServTeinSDO.Count > 0)
                {

                    var testIndexRangeAll = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>();
                    foreach (var hisSereServTeinSDO in lstHisSereServTeinSDO)
                    {
                        var serviceReqs = _ServiceReqs.Where(o => o.ID == hisSereServTeinSDO.TDL_SERVICE_REQ_ID);
                        if (serviceReqs != null && serviceReqs.Count() > 0)
                        {
                            V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                            testIndexRange = GetTestIndexRange(serviceReqs.First().TDL_PATIENT_DOB, GetGenderId(serviceReqs.First()), hisSereServTeinSDO.TEST_INDEX_ID ?? 0, ref this.testIndexRangeAll);

                            ProcessMaxMixValue(hisSereServTeinSDO, testIndexRange);
                        }

                    }
                }

                gridControlSereServTein.DataSource = lstHisSereServTeinSDO;

                gridViewSereServTein.FocusedRowHandle = 1;

                gridViewSereServTein.FocusedColumn = gridViewSereServTein.VisibleColumns[3];

                // gridViewSereServTein.ShowEditor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_TEST_INDEX_RANGE GetTestIndexRange(long dob, long genderId, long testIndexId, ref List<V_HIS_TEST_INDEX_RANGE> testIndexRanges)
        {
            MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
            try
            {
                if (testIndexRanges != null && testIndexRanges.Count > 0)
                {
                    double age = 0;
                    List<V_HIS_TEST_INDEX_RANGE> query = new List<V_HIS_TEST_INDEX_RANGE>();
                    var ageDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob);
                    foreach (var item in testIndexRanges)
                    {
                        if (item.TEST_INDEX_ID == testIndexId)
                        {
                            if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__YEAR) // Năm
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays / 365;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__MONTH) // Tháng
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays / 30;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__DAY) // Ngày
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__HOUR) // Giờ
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalHours;
                            }
                            Inventec.Common.Logging.LogSystem.Debug(age + "______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dob), dob));

                            if (((item.AGE_FROM.HasValue && item.AGE_FROM.Value <= age) || !item.AGE_FROM.HasValue)
                                 && ((item.AGE_TO.HasValue && item.AGE_TO.Value >= age) || !item.AGE_TO.HasValue))
                            {
                                query.Add(item);
                            }
                        }
                    }
                    if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1).ToList();
                    }
                    else if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        query = query.Where(o => o.IS_FEMALE == 1).ToList();
                    }
                    testIndexRange = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return testIndexRange;
        }

        long GetGenderId(V_HIS_SERVICE_REQ serviceReq)
        {

            long genderId = 0;
            try
            {
                CommonParam param = new CommonParam();
                if (serviceReq != null && serviceReq.TDL_PATIENT_GENDER_ID != null)
                {
                    genderId = serviceReq.TDL_PATIENT_GENDER_ID ?? 0;
                }
                else if (serviceReq != null && !String.IsNullOrWhiteSpace(serviceReq.TDL_PATIENT_CODE))
                {
                    HisPatientFilter patientFilter = new HisPatientFilter();
                    patientFilter.PATIENT_CODE = serviceReq.TDL_PATIENT_CODE;
                    var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                    if (patients != null && patients.Count > 0)
                    {
                        genderId = patients.FirstOrDefault().GENDER_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                genderId = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return genderId;
        }

        private void ProcessMaxMixValue(HisSereServTeinSDO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                Decimal minValue = 0, maxValue = 0;
                if (ti != null && testIndexRange != null)
                {
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out minValue))
                        {
                            ti.MIN_VALUE = minValue;
                        }
                        else
                        {
                            ti.MIN_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out maxValue))
                        {
                            ti.MAX_VALUE = maxValue;
                        }
                        else
                        {
                            ti.MAX_VALUE = null;
                        }
                    }

                    ti.IS_NORMAL = null;
                    ti.IS_HIGHER = null;
                    ti.IS_LOWER = null;
                    ti.DESCRIPTION = "";

                    if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
                    {
                        ti.DESCRIPTION = testIndexRange.NORMAL_VALUE;
                        if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE) && ti.VALUE.ToUpper() == ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_NORMAL = true;
                        }
                        else if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE) && ti.VALUE.ToUpper() != ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_LOWER = true;
                            ti.IS_HIGHER = true;
                        }
                    }
                    else
                    {
                        if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            ti.DESCRIPTION = "";
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }

                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE <= Convert.ToInt64(ti.VALUE.Trim()) && ti.MAX_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE <= Convert.ToInt64(ti.VALUE.Trim()))
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            ti.DESCRIPTION = "";
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE <= Convert.ToInt64(ti.VALUE.Trim()) && ti.MAX_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE < Convert.ToInt64(ti.VALUE.Trim()))
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            ti.DESCRIPTION = "";
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE < Convert.ToInt64(ti.VALUE.Trim()) && ti.MAX_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE < Convert.ToInt64(ti.VALUE.Trim()))
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            ti.DESCRIPTION = "";
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }
                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE < Convert.ToInt64(ti.VALUE.Trim()) && ti.MAX_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) <= ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE <= Convert.ToInt64(ti.VALUE.Trim()))
                            {
                                ti.IS_HIGHER = true;
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

        private void repositoryItemButtonEditAddBlood_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (this.currentBlty == null || this.resultExpMest != null || !dxValidationProvider1.Validate())
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanCanChonYeuCauXuatTruocKhiChonTuiMau, "Thông báo");
                    return;
                }
                var blood = (V_HIS_BLOOD)gridViewBlood.GetFocusedRow();
                if (blood != null)
                {
                    btnSave.Enabled = true;

                    var bloodTypeHasWarning = BackendDataWorker.Get<HIS_BLOOD_TYPE>().FirstOrDefault(o => o.ID == blood.BLOOD_TYPE_ID && o.WARNING_DAY > 0);
                    TimeSpan time = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Convert.ToInt64(blood.EXPIRED_DATE.ToString())) - DateTime.Today;
                    double timeCheck = time.TotalDays;

                    if (bloodTypeHasWarning != null)
                    {

                        if (timeCheck > 0 && timeCheck <= bloodTypeHasWarning.WARNING_DAY)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Túi máu sắp hết hạn, còn {0} ngày sử dụng. Bạn có muốn thêm?", timeCheck),
                   MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                return;
                            }
                        }
                    }
                    if (timeCheck <= 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Túi máu đã hết hạn", "Thông báo");
                        return;
                    }


                    if (blood.BLOOD_TYPE_ID != this.currentBlty.BLOOD_TYPE_ID && this.AllowExportBloodOverRequestCFG == "0")
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.TuiMauKhongThuocLoaiMauDangChon,
                   MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    var count = dicBloodAdo.Select(s => s.Value).ToList().Where(o => o.ExpMestBltyId == this.currentBlty.ID).ToList().Count();
                    if (count >= this.currentBlty.AMOUNT && this.AllowExportBloodOverRequestCFG == "0")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DaDuSoLuongMauYeuCau, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao);
                        return;
                    }

                    WaitingManager.Show();
                    VHisBloodADO ado = new VHisBloodADO(blood);
                    ado.PATIENT_TYPE_ID = this.currentBlty.PATIENT_TYPE_ID;
                    ado.PATIENT_TYPE_CODE = this.currentBlty.PATIENT_TYPE_CODE;
                    ado.PATIENT_TYPE_NAME = this.currentBlty.PATIENT_TYPE_NAME;
                    ado.ExpMestBltyId = currentBlty.ID;
                    ado.ANTI_GLOBULIN_ENVI = 5;
                    ado.SALT_ENVI = 5;
                    ado.AC_SELF_ENVIDENCE = 0;
                    dicBloodAdo[ado.ID] = ado;
                    if (dicShowBlood.ContainsKey(ado.ID))
                    {
                        dicShowBlood.Remove(ado.ID);
                    }
                    fillDataGridViewBlood();
                    FillDataToGridExpMestBlood();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDeleteBlood_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewExMestBlood.FocusedRowHandle >= 0)
                {
                    var data = (VHisBloodADO)gridViewExMestBlood.GetFocusedRow();
                    if (data != null)
                    {
                        if (dicBloodAdo.ContainsKey(data.ID))
                        {
                            dicBloodAdo.Remove(data.ID);
                            dicShowBlood[data.ID] = dicCurrentBlood[data.ID];
                        }
                        fillDataGridViewBlood();
                        FillDataToGridExpMestBlood();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region focus

        private void txtBloodCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ProcessAddBloodIntoExpMest();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessAddBloodIntoExpMest()
        {
            try
            {
                positionHandle = -1;
                if (this.currentBlty == null || this.resultExpMest != null || !dxValidationProvider2.Validate())
                    return;
                string bloodCode = txtBloodCode.Text.Trim();
                if (!dicBloodCode.ContainsKey(bloodCode))
                {
                    MessageManager.Show(String.Format(ResourceMessage.MaVachKhongChinhXac, bloodCode));
                    return;
                }

                if (gridControlBlood.DataSource == null) return;
                var listBlood = (List<V_HIS_BLOOD>)gridControlBlood.DataSource;
                if (!listBlood.Exists(o => o.BLOOD_CODE == bloodCode))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Túi máu không thuộc loại máu đang chọn.", MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao));
                    WaitingManager.Hide();
                    return;
                }

                var blood = dicBloodCode[bloodCode];
                if (blood != null)
                {
                    btnSave.Enabled = true;
                    VisibleColumnMTMuoiAndMTAntiGlobulin();

                    if (blood.BLOOD_TYPE_ID != this.currentBlty.BLOOD_TYPE_ID)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.TuiMauKhongThuocLoaiMauDangChon,
                   MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            WaitingManager.Hide();
                            return;
                        }
                    }
                    var count = dicBloodAdo.Select(s => s.Value).ToList().Where(o => o.ExpMestBltyId == this.currentBlty.ID).ToList().Count();
                    if (count >= this.currentBlty.AMOUNT)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DaDuSoLuongMauYeuCau, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao);
                        return;
                        //if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.DaCoTrongDanhSachDuyet_BanCoMuonThayThe, this.currentBlty.BLOOD_TYPE_NAME), ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        //{
                        //    return;
                        //}
                    }

                    WaitingManager.Show();
                    VHisBloodADO ado = new VHisBloodADO(blood);
                    ado.PATIENT_TYPE_ID = this.currentBlty.PATIENT_TYPE_ID;
                    ado.PATIENT_TYPE_CODE = this.currentBlty.PATIENT_TYPE_CODE;
                    ado.PATIENT_TYPE_NAME = this.currentBlty.PATIENT_TYPE_NAME;
                    ado.ExpMestBltyId = this.currentBlty.ID;
                    ado.SALT_ENVI = 5;
                    ado.ANTI_GLOBULIN_ENVI = 5;
                    ado.AC_SELF_ENVIDENCE = 0;
                    // ado.EXPIRED_DATE = Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "235959");
                    dicBloodAdo[ado.ID] = ado;
                    if (dicShowBlood.ContainsKey(ado.ID))
                    {
                        dicShowBlood.Remove(ado.ID);
                    }
                    //  FillDataToGridBlood();
                    FillDataToGridExpMestBlood();
                    txtBloodCode.Text = "";
                    txtBloodCode.Focus();
                    txtBloodCode.SelectAll();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBloodType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBloodType.EditValue != null)
                    {
                        fillDataGridViewBlood();
                        gridLookUpVolume.Focus();
                        gridLookUpVolume.SelectAll();
                    }
                    else
                    {
                        cboBloodType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboBloodType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                fillDataGridViewBlood();
                cboBloodType.Properties.Buttons[1].Visible = true;
                gridLookUpVolume.Focus();
                gridLookUpVolume.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridLookUpVolume_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (gridLookUpVolume.EditValue != null)
                    {
                        fillDataGridViewBlood();
                        gridLookUpBloodAboCode.Focus();
                        gridLookUpBloodAboCode.SelectAll();
                    }
                    else
                    {
                        gridLookUpVolume.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodAboCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (gridLookUpBloodAboCode.EditValue != null)
                    {
                        fillDataGridViewBlood();
                        gridLookUpBloodRhCode.Focus();
                        gridLookUpBloodRhCode.SelectAll();
                    }
                    else
                    {
                        gridLookUpBloodAboCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodRhCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (gridLookUpBloodRhCode.EditValue != null)
                    {
                        fillDataGridViewBlood();

                    }
                    else
                    {
                        gridLookUpBloodRhCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpVolume_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                fillDataGridViewBlood();
                gridLookUpVolume.Properties.Buttons[1].Visible = true;
                gridLookUpBloodAboCode.Focus();
                gridLookUpBloodAboCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodAboCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                fillDataGridViewBlood();
                gridLookUpBloodAboCode.Properties.Buttons[1].Visible = true;
                gridLookUpBloodRhCode.Focus();
                gridLookUpBloodRhCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodRhCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                gridLookUpBloodRhCode.Properties.Buttons[1].Visible = true;
                fillDataGridViewBlood();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region validate
        private void ValidControl()
        {
            try
            {
                ValidControlBloodCode();
                //ValidControlExpiredDate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlBloodCode()
        {
            try
            {
                BarCodeValidationRule bloodCodeRule = new BarCodeValidationRule();
                bloodCodeRule.txtBarCode = txtBloodCode;
                dxValidationProvider2.SetValidationRule(txtBloodCode, bloodCodeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void ValidControlExpiredDate()
        //{
        //    try
        //    {
        //        ExpiredDateValidationRule expiredDateRule = new ExpiredDateValidationRule();
        //        expiredDateRule.dtExpiredDate = dtExpiredDate;
        //        dxValidationProvider1.SetValidationRule(txtExpiredDate, expiredDateRule);
        //        dxValidationProvider2.SetValidationRule(txtExpiredDate, expiredDateRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        #endregion

    }
}
