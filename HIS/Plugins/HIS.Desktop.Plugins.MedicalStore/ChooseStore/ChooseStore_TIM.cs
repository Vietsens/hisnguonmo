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
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MedicalStore.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicalStore.ChooseStore
{
    public partial class ChooseStore_TIM : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<TreatmentADO> currentTreatmentAdo;
        RefeshReference refreshReference;
        HisTreatmentCheckDataStoreSDO hisTreatmentCheckDataStoreSDO;
        List<TreatmentADO> treatmentCoPhauThuat;
        List<TreatmentADO> treatmentKhongCoPhauThuat;
        List<V_HIS_DATA_STORE> currentDataStore;

        public ChooseStore_TIM(Inventec.Desktop.Common.Modules.Module currentModule, List<TreatmentADO> currentMediRecord, RefeshReference refeshData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.currentTreatmentAdo = currentMediRecord;
                this.refreshReference = refeshData;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChooseStore_TIM_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                GetDataStore();
                InitComboDataStore(cboKhoCoPhauThuat, currentDataStore);
                InitComboDataStore(cboKhoKhongCoPhauThuat, currentDataStore);
                if (currentDataStore != null && currentDataStore.Count > 0)
                {
                    var f1 = currentDataStore.FirstOrDefault(o => o.DATA_STORE_CODE == "M");
                    if (f1 != null)
                    {
                        cboKhoCoPhauThuat.EditValue = f1.ID;
                    }

                    var f2 = currentDataStore.FirstOrDefault(o => o.DATA_STORE_CODE == "N");
                    if (f2 != null)
                    {
                        cboKhoKhongCoPhauThuat.EditValue = f2.ID;
                    }
                }
                SetDataToGrid(this.currentTreatmentAdo);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToGrid(List<TreatmentADO> data)
        {
            try
            {
                if (data != null && data.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    hisTreatmentCheckDataStoreSDO = new BackendAdapter(param).Post<HisTreatmentCheckDataStoreSDO>("api/HisTreatment/CheckDataStore", ApiConsumers.MosConsumer, data.Select(o => o.ID).ToList(), param);
                    if (hisTreatmentCheckDataStoreSDO != null)
                    {
                        if (hisTreatmentCheckDataStoreSDO.HasSurgIds != null && hisTreatmentCheckDataStoreSDO.HasSurgIds.Count > 0)
                        {
                            gridControlCoPhauThuat.BeginUpdate();
                            treatmentCoPhauThuat = data.Where(o => hisTreatmentCheckDataStoreSDO.HasSurgIds.Contains(o.ID)).ToList();
                            gridControlCoPhauThuat.DataSource = treatmentCoPhauThuat;
                            gridControlCoPhauThuat.EndUpdate();
                        }

                        if (hisTreatmentCheckDataStoreSDO.HasNotSurgIds != null && hisTreatmentCheckDataStoreSDO.HasNotSurgIds.Count > 0)
                        {
                            gridControlKhongCoPhauThuat.BeginUpdate();
                            treatmentKhongCoPhauThuat = data.Where(o => hisTreatmentCheckDataStoreSDO.HasNotSurgIds.Contains(o.ID)).ToList();
                            gridControlKhongCoPhauThuat.DataSource = treatmentKhongCoPhauThuat;
                            gridControlKhongCoPhauThuat.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDataStore()
        {

            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDataStoreViewFilter dataStoreFilter = new MOS.Filter.HisDataStoreViewFilter();
                dataStoreFilter.IS_ACTIVE = 1;
                currentDataStore = new BackendAdapter(param).Get<List<V_HIS_DATA_STORE>>(HisRequestUriStore.HIS_DATA_STORE_GETVIEW, ApiConsumers.MosConsumer, dataStoreFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboDataStore(GridLookUpEdit cbo, List<V_HIS_DATA_STORE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DATA_STORE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DATA_STORE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DATA_STORE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                if (btnSave.Enabled)
                    btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if ((cboKhoCoPhauThuat.EditValue == null && treatmentCoPhauThuat != null && treatmentCoPhauThuat.Count > 0) || (cboKhoKhongCoPhauThuat.EditValue == null && treatmentKhongCoPhauThuat != null && treatmentKhongCoPhauThuat.Count > 0))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn kho dữ liệu", "Thông báo");
                    return;
                }

                if (treatmentCoPhauThuat == null && treatmentKhongCoPhauThuat == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có dữ liệu bệnh nhân", "Thông báo");
                    return;
                }

                bool result = false;
                CommonParam param = new CommonParam();
                string MessageSuccess = "";
                List<HIS_TREATMENT> treatmentResults = null;

                List<HIS_TREATMENT> lstMediRecordUpdate = new List<HIS_TREATMENT>();

                if (treatmentCoPhauThuat != null && treatmentCoPhauThuat.Count > 0)
                {
                    foreach (var item in treatmentCoPhauThuat)
                    {
                        HIS_TREATMENT hisMediRecord = new HIS_TREATMENT();
                        Mapper.CreateMap<V_HIS_TREATMENT_9, HIS_TREATMENT>();
                        hisMediRecord = Mapper.Map<V_HIS_TREATMENT_9, HIS_TREATMENT>(item);
                        hisMediRecord.DATA_STORE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboKhoCoPhauThuat.EditValue.ToString());
                        lstMediRecordUpdate.Add(hisMediRecord);
                    }
                }

                if (treatmentKhongCoPhauThuat != null && treatmentKhongCoPhauThuat.Count > 0)
                {
                    foreach (var item in treatmentKhongCoPhauThuat)
                    {
                        HIS_TREATMENT hisMediRecord = new HIS_TREATMENT();
                        Mapper.CreateMap<V_HIS_TREATMENT_9, HIS_TREATMENT>();
                        hisMediRecord = Mapper.Map<V_HIS_TREATMENT_9, HIS_TREATMENT>(item);
                        hisMediRecord.DATA_STORE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboKhoKhongCoPhauThuat.EditValue.ToString());
                        lstMediRecordUpdate.Add(hisMediRecord);
                    }
                }

                WaitingManager.Show();
                treatmentResults = new BackendAdapter(param).Post<List<HIS_TREATMENT>>("api/HisTreatment/UpdateListDataStoreId", ApiConsumers.MosConsumer, lstMediRecordUpdate, param);
                WaitingManager.Hide();
                if (treatmentResults != null && treatmentResults.Count > 0)
                {
                    result = true;
                    foreach (var item in treatmentResults)
                    {
                        MessageSuccess += item.STORE_CODE + ", ";
                    }
                    this.refreshReference();
                    btnSave.Enabled = false;
                }

                #region Show message
                if (treatmentResults != null && treatmentResults.Count > 0)
                {
                    MessageManager.Show("Xử lý thành công. Số lưu trữ của hồ sơ là: " + MessageSuccess);
                }
                else
                {
                    MessageManager.Show(this.ParentForm, param, result);
                }
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCoPhauThuat_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TreatmentADO pData = (TreatmentADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewKhongCoPhauThuat_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TreatmentADO pData = (TreatmentADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
