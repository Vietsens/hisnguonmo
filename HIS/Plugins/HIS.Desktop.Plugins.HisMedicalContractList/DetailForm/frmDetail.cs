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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using HIS.UC.MaterialType;
using HIS.UC.MedicineType;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisMedicalContractList.DetailForm
{
    public partial class frmDetail : FormBase
    {
        MedicineTypeProcessor medicineProcessor = null;
        MaterialTypeTreeProcessor materialProcessor = null;
        private MOS.EFMODEL.DataModels.V_HIS_MEDICAL_CONTRACT _MedicalContract { get; set; }
        Inventec.Desktop.Common.Modules.Module moduleData;

        List<V_HIS_MEDI_CONTRACT_METY> ListHisMedicalContract = new List<V_HIS_MEDI_CONTRACT_METY>();
        List<V_HIS_MEDI_CONTRACT_MATY> ListHisMaterialContract = new List<V_HIS_MEDI_CONTRACT_MATY>();

        string loginName = null;
        public frmDetail()
        {
            InitializeComponent();
        }

        public frmDetail(Inventec.Desktop.Common.Modules.Module moduleData, MOS.EFMODEL.DataModels.V_HIS_MEDICAL_CONTRACT medicalContract)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    this._MedicalContract = medicalContract;
                    this.moduleData = moduleData;
                    this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
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
        private void frmDetail_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridDetail()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisMediContractMetyViewFilter filter = new HisMediContractMetyViewFilter();
                filter.MEDICAL_CONTRACT_ID = this._MedicalContract.ID;
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_MEDI_CONTRACT_METY>>("api/HisMediContractMety/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                CommonParam param = new CommonParam();
                HisMediContractMatyViewFilter filterMaterial = new HisMediContractMatyViewFilter();
                filterMaterial.MEDICAL_CONTRACT_ID = this._MedicalContract.ID;
                var resultMaterial = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<V_HIS_MEDI_CONTRACT_MATY>>("api/HisMediContractMaty/GetView", ApiConsumers.MosConsumer, filterMaterial, param);
                Inventec.Common.Logging.LogSystem.Info("this._MedicalContract.ID " + this._MedicalContract.ID);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultMaterial), resultMaterial));
                if (result != null)
                {
                    ListHisMedicalContract = (List<V_HIS_MEDI_CONTRACT_METY>)result.Data;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListHisMedicalContract), ListHisMedicalContract));

                    gridControlMedicine.BeginUpdate();
                    gridControlMedicine.DataSource = null;
                    gridControlMedicine.DataSource = ListHisMedicalContract;
                    
                    gridControlMedicine.EndUpdate();
                }
                if (resultMaterial != null)
                {
                    ListHisMaterialContract = (List<V_HIS_MEDI_CONTRACT_MATY>)resultMaterial.Data;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListHisMaterialContract), ListHisMaterialContract));
                    gridControlMaterial.BeginUpdate();
                    gridControlMaterial.DataSource = null;
                    gridControlMaterial.DataSource = ListHisMaterialContract;
                    gridControlMaterial.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMedicine_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_HIS_MEDI_CONTRACT_METY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TDL_SERVICE_UNIT_STR")
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                            if (data.TDL_SERVICE_UNIT_ID != 0)
                            {
                                var serviceUnits = BackendDataWorker.Get<HIS_SERVICE_UNIT>();
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceUnits), serviceUnits));
                                if (serviceUnits != null && serviceUnits.Count > 0)
                                {
                                    var serviceUnit = serviceUnits.FirstOrDefault(o => o.ID == data.TDL_SERVICE_UNIT_ID);
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceUnit), serviceUnit));
                                    e.Value = serviceUnit.SERVICE_UNIT_NAME;
                                }
                            }
                        }

                        if (e.Column.FieldName == "IMP_EXPIRED_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_EXPIRED_DATE ?? 0);
                        }
                        if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXPIRED_DATE ?? 0);
                        
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
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_HIS_MEDI_CONTRACT_MATY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TDL_SERVICE_UNIT_ID_STR")
                        {
                            if (data.TDL_SERVICE_UNIT_ID != 0)
                            {
                                var serviceUnits = BackendDataWorker.Get<HIS_SERVICE_UNIT>();
                                if (serviceUnits != null && serviceUnits.Count > 0)
                                {
                                    var serviceUnit = serviceUnits.FirstOrDefault(o => o.ID == data.TDL_SERVICE_UNIT_ID);
                                    e.Value = serviceUnit.SERVICE_UNIT_NAME;
                                }
                            }
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_STR")
                        {
                            e.Value = data.IMP_VAT_RATIO * 100 + "%";
                        }
                        if (e.Column.FieldName == "IMP_EXPIRED_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_EXPIRED_DATE ?? 0); 
                        }
                        if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXPIRED_DATE ?? 0);

                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_MEDI_CONTRACT_MATY data = (V_HIS_MEDI_CONTRACT_MATY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        if (data != null)
                        {
                            if (this.loginName == data.CREATOR)
                            {
                                e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryItemUnLock : repositoryItemLock);
                            }
                            else
                            {
                                e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryItemUnLock_Disable : repositoryItemLock_Disable);   
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

        private void gridViewMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_MEDI_CONTRACT_METY data = (V_HIS_MEDI_CONTRACT_METY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        if (data != null)
                        {
                            if (this.loginName == data.CREATOR)
                            {
                                e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryItemButtonMediUnlock : repositoryItemButtonMediLock);
                            }
                            else
                            {
                                e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryItemButtonMediUnlock_Disable : repositoryItemButtonMediLock_Disable);
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

        private void repositoryItemButtonMediLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDI_CONTRACT_METY result = new HIS_MEDI_CONTRACT_METY();
            bool success = false;
            try
            {

                V_HIS_MEDI_CONTRACT_METY data = (V_HIS_MEDI_CONTRACT_METY)gridViewMedicine.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MEDI_CONTRACT_METY>("api/HisMediContractMety/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridDetail();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonMediUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDI_CONTRACT_METY result = new HIS_MEDI_CONTRACT_METY();
            bool success = false;
            try
            {

                V_HIS_MEDI_CONTRACT_METY data = (V_HIS_MEDI_CONTRACT_METY)gridViewMedicine.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MEDI_CONTRACT_METY>("api/HisMediContractMety/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridDetail();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDI_CONTRACT_MATY result = new HIS_MEDI_CONTRACT_MATY();
            bool success = false;
            try
            {

                V_HIS_MEDI_CONTRACT_MATY data = (V_HIS_MEDI_CONTRACT_MATY)gridViewMedicine.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MEDI_CONTRACT_MATY>("api/HisMediContractMaty/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridDetail();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDI_CONTRACT_MATY result = new HIS_MEDI_CONTRACT_MATY();
            bool success = false;
            try
            {

                V_HIS_MEDI_CONTRACT_MATY data = (V_HIS_MEDI_CONTRACT_MATY)gridViewMedicine.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MEDI_CONTRACT_MATY>("api/HisMediContractMaty/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridDetail();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        
    }
}
