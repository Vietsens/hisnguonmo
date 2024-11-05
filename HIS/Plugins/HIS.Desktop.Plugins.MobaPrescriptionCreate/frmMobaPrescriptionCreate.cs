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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MobaPrescriptionCreate.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.Processor.Mps000084.PDO;
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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.ViewInfo;

namespace HIS.Desktop.Plugins.MobaPrescriptionCreate
{
    public partial class frmMobaPrescriptionCreate : HIS.Desktop.Utility.FormBase
    {
        long expMestId;
        V_HIS_EXP_MEST hisExpMest = null;
        HIS_SERVICE_REQ hisServiceReq = null;
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        List<VHisExpMestMedicineADO> listExpMestMedicineADO = new List<VHisExpMestMedicineADO>();
        List<VHisExpMestMaterialADO> listExpMestMaterialADO = new List<VHisExpMestMaterialADO>();
        List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine;
        List<V_HIS_EXP_MEST_MATERIAL> listExpMestMaterial;

        HisImpMestResultSDO resultMobaSdo = null;
        int positionHandle = -1;

        public frmMobaPrescriptionCreate(Inventec.Desktop.Common.Modules.Module module, long data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.expMestId = data;
                this.currentModule = module;
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmMobaDepaCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyFrmLanguage();
                LoadExpMest();
                LoadServiceReq();
                LoadRemedyCount();
                LoadComboboxMediStock();
                LoadComboboxTracking();
                LoadExpMestMedicine();
                LoadExpMestMaterial();
                btnSave.Enabled = true;
                btnPrint.Enabled = false;
                if (listExpMestMedicineADO != null && listExpMestMedicineADO.Count > 0)
                {
                    xtraTabControlMain.SelectedTabPage = xtraTabPageMedicine;
                }
                else if (listExpMestMaterialADO != null && listExpMestMaterialADO.Count > 0)
                {
                    xtraTabControlMain.SelectedTabPage = xtraTabPageMaterial;
                }
                else
                {
                    xtraTabControlMain.SelectedTabPage = xtraTabPageMedicine;
                }
                ValidControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadRemedyCount()
        {
            try
            {
                //ToolTip toolTip1 = new ToolTip();
                //toolTip1.SetToolTip(txtRemedyCountHT, "Số thang hiện có");
                //toolTip1.AutoPopDelay = 5000; // Tooltip ẩn sau 5 giây
                //toolTip1.InitialDelay = 500;  // Tooltip hiện sau 0.5 giây

                if (hisServiceReq != null && hisServiceReq.PRESCRIPTION_TYPE_ID == 2)
                {
                    layoutRemedyCount.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutRemedyCountHT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    txtRemedyCountHT.Text = hisServiceReq.REMEDY_COUNT.ToString()??""; 
                    if (hisServiceReq.REMEDY_COUNT > 0)
                    {
                        txtRemedyCount.Enabled = true;
                        txtRemedyCount.EditValue = null;
                        txtRemedyCount.Properties.MaxValue = (decimal)hisServiceReq.REMEDY_COUNT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckTrackingTime(long trackingTime)
        {
            bool rs = false;
            try
            {
                var startDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
                var endDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                var _startDayTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(startDay);
                var _endDayTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(endDay);
                rs = _startDayTime <= trackingTime && trackingTime <= _endDayTime;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private void LoadComboboxTracking()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTrackingFilter trackingFilter = new HisTrackingFilter();
                trackingFilter.TREATMENT_ID = hisExpMest.TDL_TREATMENT_ID;
                trackingFilter.DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId).DepartmentId;
                var lstTracking = new BackendAdapter(param).Get<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumer.ApiConsumers.MosConsumer, trackingFilter, param);
                List<TrackingADO> data = new List<TrackingADO>();
                if (lstTracking != null && lstTracking.Count() > 0)
                {
                    data = (from m in lstTracking select new TrackingADO(m)).OrderByDescending(o => o.TRACKING_TIME).ToList();
                    if (HisConfigs.Get<string>("HIS.Desktop.Plugins.MobaCreate.IsTrackingRequired") == "1"
                        && data.Exists(o => o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                        && CheckTrackingTime(o.TRACKING_TIME)))
                    {
                        cboTracking.EditValue = data.Where(o => o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                       && CheckTrackingTime(o.TRACKING_TIME)).OrderByDescending(o => o.TRACKING_TIME).First().ID;
                    }
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TrackingTimeStr", "Thời gian", 150, 1));
                columnInfos.Add(new ColumnInfo("CREATOR", "Người tạo", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TrackingTimeStr", "ID", columnInfos, true, 300);
                ControlEditorLoader.Load(this.cboTracking, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                if (HisConfigs.Get<string>("HIS.Desktop.Plugins.MobaCreate.IsTrackingRequired") == "1")
                {
                    lciTracking.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationSingleControl(cboTracking);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboboxMediStock()
        {
            try
            {

                List<V_HIS_MEDI_STOCK> lstMediStock = new List<V_HIS_MEDI_STOCK>();
                if (this.hisExpMest != null)
                {
                    HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                    serviceReqFilter.ID = this.hisExpMest.SERVICE_REQ_ID;
                    serviceReqFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var serviceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    if (serviceReq != null && serviceReq.Count > 0)
                    {

                        HisMediStockViewFilter filter = new HisMediStockViewFilter();
                        filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        filter.DEPARTMENT_ID = serviceReq.FirstOrDefault().REQUEST_DEPARTMENT_ID;
                        var datas = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK>>(HisRequestUriStore.HIS_MEDI_STOCK_GETVIEW, ApiConsumers.MosConsumer, filter, null);
                        if (datas != null && datas.Count() > 0)
                        {
                            lstMediStock = datas.Where(o => o.IS_CABINET != 1 && o.IS_BUSINESS != 1).OrderBy(o => o.ID).ToList();
                        }


                        HisMestRoomViewFilter filterMestRoom = new HisMestRoomViewFilter();
                        filterMestRoom.ROOM_ID = serviceReq.FirstOrDefault().REQUEST_ROOM_ID;

                        var dataMestRoom = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEST_ROOM>>("api/HisMestRoom/GetView", ApiConsumers.MosConsumer, filterMestRoom, null);

                        if (dataMestRoom != null && dataMestRoom.Count() > 0)
                        {
                            HisMediStockViewFilter filterMediStock = new HisMediStockViewFilter();
                            filterMediStock.IDs = dataMestRoom.Select(o => o.MEDI_STOCK_ID).ToList();
                            var dataMediStock = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK>>(HisRequestUriStore.HIS_MEDI_STOCK_GETVIEW, ApiConsumers.MosConsumer, filterMediStock, null);
                            if (dataMediStock != null && dataMediStock.Count() > 0)
                            {
                                lstMediStock.AddRange(dataMediStock.Where(o => !lstMediStock.Exists(p => p.ID == o.ID)).ToList());
                            }
                        }
                        lstMediStock = lstMediStock.Where(o => o.IS_CABINET != 1 && o.IS_BUSINESS != 1).Distinct().ToList();
                        HisDepartmentFilter ft = new HisDepartmentFilter();
                        ft.ID = serviceReq.FirstOrDefault().REQUEST_DEPARTMENT_ID;
                        var checkDepartment = new BackendAdapter(new CommonParam()).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, ft, null);
                        if (checkDepartment != null && checkDepartment.Count > 0)
                        {
                            if (checkDepartment.FirstOrDefault().IS_IN_DEP_STOCK_MOBA == 1)
                            {
                                if (lstMediStock != null && lstMediStock.Count > 0)
                                {
                                    cboMediStock.EditValue = lstMediStock.FirstOrDefault().ID;
                                }
                            }
                        }

                    }
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboMediStock, lstMediStock, controlEditorADO);



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMest()
        {
            try
            {
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this.expMestId;
                var hisExpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, expMestFilter, null);
                if (hisExpMests != null && hisExpMests.Count == 1)
                {
                    hisExpMest = hisExpMests.First();
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
                HisServiceReqFilter ServiceReqFilter = new HisServiceReqFilter();
                if(hisExpMest != null)
                {
                    ServiceReqFilter.ID = hisExpMest.SERVICE_REQ_ID;
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, ServiceReqFilter, null);
                if (rs != null && rs.Count == 1)
                {
                    hisServiceReq = rs.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMedicine()
        {
            try
            {
                if (this.hisExpMest != null)
                {
                    CommonParam param = new CommonParam();
                    HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter()
                    {
                        EXP_MEST_ID = this.hisExpMest.ID,
                    };
                    listExpMestMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, param);
                    if (listExpMestMedicine != null && listExpMestMedicine.Count > 0)
                    {
                        var group = listExpMestMedicine.GroupBy(o => new { o.MEDICINE_ID, o.PATIENT_TYPE_ID, o.IS_EXPEND, o.IS_OUT_PARENT_FEE }).ToList();
                        foreach (var item in group)
                        {
                            VHisExpMestMedicineADO medi = new VHisExpMestMedicineADO(item.First());
                            medi.AMOUNT = item.Sum(s => s.AMOUNT);
                            medi.CAN_MOBA_AMOUNT = item.Sum(s => s.AMOUNT) - item.Sum(p => p.TH_AMOUNT ?? 0);
                            listExpMestMedicineADO.Add(medi);
                        }
                    }
                    else
                    {
                        listExpMestMedicineADO = new List<VHisExpMestMedicineADO>();
                    }
                    gridControlExpMestMedicine.BeginUpdate();
                    gridControlExpMestMedicine.DataSource = listExpMestMedicineADO;
                    gridControlExpMestMedicine.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMaterial()
        {
            try
            {
                if (this.hisExpMest != null)
                {
                    HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter()
                    {
                        EXP_MEST_ID = this.hisExpMest.ID,
                    };
                    listExpMestMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, materialFilter, null);
                    if (listExpMestMaterial != null && listExpMestMaterial.Count > 0)
                    {
                        var group = listExpMestMaterial.GroupBy(o => new { o.MATERIAL_ID, o.PATIENT_TYPE_ID, o.IS_EXPEND, o.IS_OUT_PARENT_FEE, o.SERIAL_NUMBER }).ToList();
                        foreach (var item in group)
                        {
                            VHisExpMestMaterialADO mate = new VHisExpMestMaterialADO(item.First());
                            mate.AMOUNT = item.Sum(s => s.AMOUNT);
                            mate.CAN_MOBA_AMOUNT = item.Sum(s => s.AMOUNT) - item.Sum(p => p.TH_AMOUNT ?? 0);
                            listExpMestMaterialADO.Add(mate);
                        }
                    }
                    else
                    {
                        listExpMestMaterialADO = new List<VHisExpMestMaterialADO>();
                    }
                    gridControlExpMestMaterial.BeginUpdate();
                    gridControlExpMestMaterial.DataSource = listExpMestMaterialADO;
                    gridControlExpMestMaterial.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void CalculTotalPrice()
        {
            try
            {
                decimal totalPrice = 0;
                decimal totalFeePrice = 0;
                decimal totalVatPrice = 0;
                if (listExpMestMaterialADO != null && listExpMestMaterialADO.Count > 0)
                {
                    var listSelect = listExpMestMaterialADO.Where(o => o.IsMoba).ToList();
                    if (listSelect != null && listSelect.Count > 0)
                    {
                        totalFeePrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT));
                        totalVatPrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT * (s.VAT_RATIO ?? 0)));
                    }
                }
                if (listExpMestMedicineADO != null && listExpMestMedicineADO.Count > 0)
                {
                    var listSelect = listExpMestMedicineADO.Where(o => o.IsMoba).ToList();
                    if (listSelect != null && listSelect.Count > 0)
                    {
                        totalFeePrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT));
                        totalVatPrice += listSelect.Sum(s => ((s.PRICE ?? 0) * s.MOBA_AMOUNT * (s.VAT_RATIO ?? 0)));
                    }
                }
                totalVatPrice = Math.Round(totalVatPrice, 4);
                totalPrice = totalFeePrice + totalVatPrice;
                lblTotalFeePrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalFeePrice, 4);
                lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 4);
                lblTotalVatPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalVatPrice, 4);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisExpMestMedicineADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "VAT_RATIO_PLUS")
                        {
                            try
                            {
                                e.Value = (data.VAT_RATIO ?? 0) * 100;
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                {
                    var data = (VHisExpMestMedicineADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                        {
                            e.RepositoryItem = repositoryItemSpinMediMobaAmountDisable;
                        }
                        else if (data.IsMoba && data.IS_EXPORT == 1)
                        {
                            e.RepositoryItem = repositoryItemMediSpinMobaAmount;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemSpinMediMobaAmountDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        int selectRow;
        private void gridViewExpMestMedicine_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                selectRow = gridViewExpMestMedicine.DataRowCount;
                for (int i = 0; i < gridViewExpMestMedicine.DataRowCount; i++)
                {
                    var data = (VHisExpMestMedicineADO)gridViewExpMestMedicine.GetRow(i);
                    if (data != null)
                    {
                        if (gridViewExpMestMedicine.IsRowSelected(i))
                        {
                            data.IsMoba = true;
                            data.MOBA_AMOUNT = (data.MOBA_AMOUNT > 0) ? data.MOBA_AMOUNT : data.CAN_MOBA_AMOUNT;
                        }
                        else
                        {
                            data.IsMoba = false;
                            data.MOBA_AMOUNT = 0;
                        }
                    }
                    CalculTotalPrice();
                }
                gridControlExpMestMedicine.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMedicine_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                {
                    CalculTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMaterial_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisExpMestMaterialADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "VAT_RATIO_PLUS")
                        {
                            try
                            {
                                e.Value = (data.VAT_RATIO ?? 0) * 100;
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMaterial_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                {
                    var data = (VHisExpMestMaterialADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                        {
                            e.RepositoryItem = repositoryItemSpinMateMobaAmountDisable;
                        }
                        else if (data.IsMoba)
                        {
                            e.RepositoryItem = repositoryItemSpinMateMobaAmount;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemSpinMateMobaAmountDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMaterial_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                for (int i = 0; i < gridViewExpMestMaterial.DataRowCount; i++)
                {
                    var data = (VHisExpMestMaterialADO)gridViewExpMestMaterial.GetRow(i);
                    if (data != null)
                    {
                        if (gridViewExpMestMaterial.IsRowSelected(i))
                        {
                            data.IsMoba = true;
                            data.MOBA_AMOUNT = (data.MOBA_AMOUNT > 0) ? data.MOBA_AMOUNT : data.CAN_MOBA_AMOUNT;
                        }
                        else
                        {
                            data.IsMoba = false;
                            data.MOBA_AMOUNT = 0;
                        }
                    }
                    CalculTotalPrice();
                }
                gridControlExpMestMaterial.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestMaterial_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "MOBA_AMOUNT")
                {
                    CalculTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled || this.expMestId <= 0 || this.hisExpMest == null)
                    return;
                if (!CheckExpDate())
                    return;
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                if (cboMediStock.EditValue != null)
                {
                    if ((long)cboMediStock.EditValue != this.hisExpMest.MEDI_STOCK_ID)
                    {
                        if (MessageBox.Show(String.Format("Kho nhập khác với Kho xuất {0}. Bạn có muốn tiếp tục?", hisExpMest.MEDI_STOCK_NAME), "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }
                }
                int remedyCount;
                int remedyCountHT;

                //List<int> selectRows = new List<int>();
                VHisExpMestMedicineADO dataExpMestMedicine = null;
                for (int i = 0; i < gridViewExpMestMaterial.RowCount; i++)
                {
                    dataExpMestMedicine = (VHisExpMestMedicineADO)gridViewExpMestMedicine.GetRow(i);
                    //if (dataExpMestMedicine != null)
                    //{
                    //    selectRows.Add(i);
                    //}
                }

                if (hisServiceReq != null && hisServiceReq.PRESCRIPTION_TYPE_ID == 2)
                {
                    if (!string.IsNullOrEmpty(txtRemedyCount.Text) && (int.TryParse(txtRemedyCount.Text, out remedyCount)) && (int.TryParse(txtRemedyCountHT.Text, out remedyCountHT)))
                    {
                        if (dataExpMestMedicine != null && remedyCount != 0 && remedyCount == hisServiceReq.REMEDY_COUNT && dataExpMestMedicine.CAN_MOBA_AMOUNT != dataExpMestMedicine.MOBA_AMOUNT)
                        {
                            MessageBox.Show("Số lượng thang khả dụng bằng 0, cần thu hồi toàn bộ thuốc");
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(txtRemedyCount.Text) && (int.TryParse(txtRemedyCount.Text, out remedyCount)))
                    {
                        if (remedyCount == 0 && selectRow > 0)
                        {
                            if (MessageBox.Show("Số lượng thuốc đã thay đổi, có muốn thu hồi số thang không?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                txtRemedyCount.Focus();
                                return;
                            }
                        }
                    }
                    else if (string.IsNullOrEmpty(txtRemedyCount.Text) && selectRow > 0)
                    {
                        if (MessageBox.Show("Số lượng thuốc đã thay đổi, có muốn thu hồi số thang không?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            txtRemedyCount.Focus();
                            return;
                        }
                    }

                    //if(hisServiceReq != null && hisServiceReq.)
                    bool showMess = false;
                    if (listExpMestMedicineADO != null && selectRow == listExpMestMedicineADO.Count())
                    {
                        foreach (var i in listExpMestMedicineADO)
                        {
                            if (i.MOBA_AMOUNT == i.CAN_MOBA_AMOUNT)
                            {
                                showMess = true;
                            }
                            else
                            {
                                showMess = false;
                                break;
                            }
                        }
                        if (showMess == true && hisServiceReq != null && hisServiceReq.REMEDY_COUNT != null && !string.IsNullOrEmpty(txtRemedyCount.Text) && long.Parse(txtRemedyCount.Text) < hisServiceReq.REMEDY_COUNT)
                        {
                            if (MessageBox.Show("Thuốc đã thu hồi toàn bộ, cần thu hồi toàn bộ số thang", "Cảnh báo", MessageBoxButtons.OK) == DialogResult.OK)
                            {
                                return;
                            }
                            
                        }
                    }
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool sucsess = false;
                CreateMobaImpMest(param, ref sucsess);
                if (sucsess)
                {
                    listExpMestMaterialADO = null;
                    listExpMestMedicineADO = null;
                    this.LoadExpMestMaterial();
                    this.LoadExpMestMedicine();
                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                }
                WaitingManager.Hide();
                if (sucsess)
                {
                    MessageManager.Show(this, param, sucsess);
                }
                else
                {
                    MessageManager.Show(param, sucsess);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateMobaImpMest(CommonParam param, ref bool sucsess)
        {
            try
            {
                HisImpMestMobaPresSDO data = new HisImpMestMobaPresSDO();
                data.MobaPresMedicines = new List<HisMobaPresMedicineSDO>();
                data.MobaPresMaterials = new List<HisMobaPresMaterialSDO>();
                data.ExpMestId = this.expMestId;
                data.Description = this.txtDescription.Text;
                if (!string.IsNullOrEmpty(txtRemedyCount.Text))
                {
                    data.RemedyCount = long.Parse(txtRemedyCount.Text);
                }
                else
                {
                    data.RemedyCount = null;
                }
                Inventec.Common.Logging.LogSystem.Error(cboMediStock.EditValue + "__________" + this.hisExpMest.MEDI_STOCK_ID);
                if (cboMediStock.EditValue != null && cboMediStock.EditValue != "")
                {
                    //                    if ((long)cboMediStock.EditValue != this.hisExpMest.MEDI_STOCK_ID)
                    //                    {

                    data.ImpMediStockId = (long)cboMediStock.EditValue;
                    //                    }

                }
                else
                {
                    data.ImpMediStockId = this.hisExpMest.MEDI_STOCK_ID;
                }
                Inventec.Common.Logging.LogSystem.Error(cboTracking.EditValue + "__________");
                data.TrackingId = cboTracking.EditValue != null ? (long?)Convert.ToInt64(cboTracking.EditValue.ToString()) : null;
                data.RequestRoomId = this.currentModule.RoomId;
                var listMedicine = listExpMestMedicineADO.Where(o => o.IsMoba).ToList();
                var listMaterial = listExpMestMaterialADO.Where(o => o.IsMoba).ToList();
                if ((listMaterial == null || listMaterial.Count == 0) && (listMedicine == null || listMedicine.Count == 0))
                {
                    param.Messages.Add(Base.ResourceMessageLang.NguoiDungChuaChonThuocVatTu);
                    return;
                }
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    foreach (var item in listMedicine)
                    {
                        if (item.MOBA_AMOUNT <= 0)
                        {
                            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiPhaiLonHonKhong);
                            return;
                        }
                        if (item.MOBA_AMOUNT > item.CAN_MOBA_AMOUNT)
                        {
                            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiKhongDuocLonHonSoLuongKhaDung);
                            return;
                        }


                        decimal mobaAmount = item.MOBA_AMOUNT;

                        var groupMedicine = listExpMestMedicine.GroupBy(o => new { o.MEDICINE_ID, o.PATIENT_TYPE_ID, o.IS_EXPEND, o.IS_OUT_PARENT_FEE }).ToList();

                        foreach (var group in groupMedicine)
                        {
                            var groupFirst = group.FirstOrDefault();
                            if (groupFirst.MEDICINE_ID == item.MEDICINE_ID
                                && groupFirst.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                && groupFirst.IS_EXPEND == item.IS_EXPEND
                                && groupFirst.IS_OUT_PARENT_FEE == item.IS_OUT_PARENT_FEE)
                            {
                                foreach (var medic in group)
                                {
                                    if (mobaAmount > 0 && (medic.AMOUNT != medic.TH_AMOUNT || medic.TH_AMOUNT == null))
                                    {
                                        HisMobaPresMedicineSDO medi = new HisMobaPresMedicineSDO();
                                        medi.ExpMestMedicineId = medic.ID;
                                        if (mobaAmount > medic.AMOUNT)
                                        {
                                            medi.Amount = medic.AMOUNT;
                                            mobaAmount = mobaAmount - medic.AMOUNT;
                                        }
                                        else
                                        {
                                            medi.Amount = mobaAmount;
                                            mobaAmount = 0;
                                        }
                                        data.MobaPresMedicines.Add(medi);
                                    }
                                }
                            }
                        }
                    }
                }

                if (listMaterial != null && listMaterial.Count > 0)
                {
                    foreach (var item in listMaterial)
                    {
                        if (item.MOBA_AMOUNT <= 0)
                        {
                            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiPhaiLonHonKhong);
                            return;
                        }
                        if (item.MOBA_AMOUNT > item.CAN_MOBA_AMOUNT)
                        {
                            param.Messages.Add(Base.ResourceMessageLang.SoLuongThuHoiKhongDuocLonHonSoLuongKhaDung);
                            return;
                        }

                        decimal mobaAmount = item.MOBA_AMOUNT;

                        var groupMaterial = listExpMestMaterial.GroupBy(o => new { o.MATERIAL_ID, o.PATIENT_TYPE_ID, o.IS_EXPEND, o.IS_OUT_PARENT_FEE, o.SERIAL_NUMBER }).ToList();

                        foreach (var group in groupMaterial)
                        {
                            var groupFirst = group.FirstOrDefault();
                            if (groupFirst.MATERIAL_ID == item.MATERIAL_ID && groupFirst.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID && groupFirst.IS_EXPEND == item.IS_EXPEND && groupFirst.IS_OUT_PARENT_FEE == item.IS_OUT_PARENT_FEE && groupFirst.SERIAL_NUMBER == item.SERIAL_NUMBER)
                            {
                                foreach (var mater in group)
                                {
                                    if (mobaAmount > 0 && (mater.AMOUNT != mater.TH_AMOUNT || mater.TH_AMOUNT == null))
                                    {
                                        HisMobaPresMaterialSDO medi = new HisMobaPresMaterialSDO();
                                        medi.ExpMestMaterialId = mater.ID;
                                        if (mobaAmount > mater.AMOUNT)
                                        {
                                            medi.Amount = mater.AMOUNT;
                                            mobaAmount = mobaAmount - mater.AMOUNT;
                                        }
                                        else
                                        {
                                            medi.Amount = mobaAmount;
                                            mobaAmount = 0;
                                        }
                                        data.MobaPresMaterials.Add(medi);
                                    }
                                }
                            }
                        }
                    }
                }
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestResultSDO>("api/HisImpMest/MobaPresCreate", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    sucsess = true;
                    resultMobaSdo = rs;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                sucsess = false;
            }
        }

        private bool CheckExpDate()
        {
            bool success = true;
            try
            {
                var listMedicine = listExpMestMedicineADO.Where(o => o.IsMoba).ToList();
                var listMaterial = listExpMestMaterialADO.Where(o => o.IsMoba).ToList();

                List<MessPackageNumber> _MessPackageNumbers = new List<MessPackageNumber>();
                if (listMedicine != null && listMedicine.Count > 0 && listMedicine.Count != listExpMestMedicineADO.Count)
                {
                    foreach (var item in listMedicine)
                    {
                        if (item.EXPIRED_DATE != null && item.EXPIRED_DATE > 0)
                        {
                            var dataChecks = listExpMestMedicineADO.Where(p => p.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID).ToList();
                            if (dataChecks != null && dataChecks.Count > 0)
                            {
                                var dataExpDates = dataChecks.Where(p => p.EXPIRED_DATE == null || p.EXPIRED_DATE > item.EXPIRED_DATE).ToList();
                                if (dataExpDates != null && dataExpDates.Count > 0)
                                {
                                    if (dataExpDates.Count == 1)
                                    {
                                        MessPackageNumber ado = new MessPackageNumber();
                                        ado.IsMedicine = true;
                                        ado.ID = dataExpDates[0].MEDICINE_ID ?? 0;
                                        ado.mess = dataExpDates[0].PACKAGE_NUMBER + " - " + dataExpDates[0].MEDICINE_TYPE_NAME;
                                        _MessPackageNumbers.Add(ado);
                                    }
                                    else
                                    {
                                        var dataMax = dataExpDates.Max(p => p.EXPIRED_DATE);
                                        var list = dataExpDates.Where(p => p.EXPIRED_DATE == dataMax).ToList();
                                        foreach (var itemMax in list)
                                        {
                                            MessPackageNumber ado = new MessPackageNumber();
                                            ado.IsMedicine = true;
                                            ado.ID = itemMax.MEDICINE_ID ?? 0;
                                            ado.mess = itemMax.PACKAGE_NUMBER + " - " + itemMax.MEDICINE_TYPE_NAME;
                                            _MessPackageNumbers.Add(ado);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (listMaterial != null && listMaterial.Count > 0 && listMaterial.Count != listExpMestMaterialADO.Count)
                {
                    foreach (var item in listMaterial)
                    {
                        if (item.EXPIRED_DATE != null && item.EXPIRED_DATE > 0)
                        {
                            var dataChecks = listExpMestMaterialADO.Where(p => p.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID).ToList();
                            if (dataChecks != null && dataChecks.Count > 0)
                            {
                                var dataExpDates = dataChecks.Where(p => p.EXPIRED_DATE == null || p.EXPIRED_DATE > item.EXPIRED_DATE).ToList();
                                if (dataExpDates != null && dataExpDates.Count > 0)
                                {
                                    if (dataExpDates.Count == 1)
                                    {
                                        MessPackageNumber ado = new MessPackageNumber();
                                        ado.IsMedicine = true;
                                        ado.ID = dataExpDates[0].MATERIAL_ID ?? 0;
                                        ado.mess = dataExpDates[0].PACKAGE_NUMBER + " - " + dataExpDates[0].MATERIAL_TYPE_NAME;
                                        _MessPackageNumbers.Add(ado);
                                    }
                                    else
                                    {
                                        var dataMax = dataExpDates.Max(p => p.EXPIRED_DATE);
                                        var list = dataExpDates.Where(p => p.EXPIRED_DATE == dataMax).ToList();
                                        foreach (var itemMax in list)
                                        {
                                            MessPackageNumber ado = new MessPackageNumber();
                                            ado.IsMedicine = true;
                                            ado.ID = itemMax.MATERIAL_ID ?? 0;
                                            ado.mess = itemMax.PACKAGE_NUMBER + " - " + itemMax.MATERIAL_TYPE_NAME;
                                            _MessPackageNumbers.Add(ado);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (_MessPackageNumbers != null && _MessPackageNumbers.Count > 0)
                {
                    var dataMessGroups = _MessPackageNumbers.GroupBy(p => new { p.IsMedicine, p.ID }).Select(p => p.ToList()).ToList();
                    List<string> _str = new List<string>();
                    _str = dataMessGroups.Select(p => p.FirstOrDefault().mess).ToList();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Lô {0} có hạn sử dụng lớn hơn. Bạn có muốn thực hiện ko?", string.Join(",", _str)), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        success = false;
                    }
                }

            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultMobaSdo == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuYeuCauNhapThuHoi_MPS000084, delegateRunPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    gridViewExpMestMaterial.PostEditor();
                    gridViewExpMestMedicine.PostEditor();
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnPrint.Enabled)
                    btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateRunPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultMobaSdo == null)
                    return result;
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineViewFilter hisExpMestMedicineViewFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                hisExpMestMedicineViewFilter.EXP_MEST_ID = this.hisExpMest.ID;
                var expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, hisExpMestMedicineViewFilter, null);

                MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialViewFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                expMestMaterialViewFilter.EXP_MEST_ID = this.hisExpMest.ID;
                var expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialViewFilter, null);

                SingleKey singleKey = new SingleKey();
                singleKey.AGGR_EXP_MEST_CODE = hisExpMest.TDL_AGGR_EXP_MEST_CODE;


                MPS.Processor.Mps000084.PDO.Mps000084PDO rdo = new MPS.Processor.Mps000084.PDO.Mps000084PDO(this.resultMobaSdo.ImpMest, this.hisExpMest, singleKey, this.resultMobaSdo.ImpMedicines, this.resultMobaSdo.ImpMaterials, expMestMedicines, expMestMaterials);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest != null ? this.hisExpMest.TDL_TREATMENT_CODE : "", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MobaPrescriptionCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.MobaPrescriptionCreate.frmMobaPrescriptionCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.xtraTabPageMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_Stt.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_MedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypName.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_MedicineTypName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_MobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_Amount.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_CanMobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_CanMobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_ImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VatRatio.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_VatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VirTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_VirTotalImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_BidNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_BidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_PackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_RegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_RegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMedicine_ExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.xtraTabPageMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_Stt.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_MaterialTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_MaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_MobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_Amount.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_CanMobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_CanMobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_ImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_VatRatio.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_VatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_VirTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_VirTotalImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_BidNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_BidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_PackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_RegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_RegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.gridColumn_ExpMestMaterial_ExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTotalPrice.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.layoutTotalPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTotalFeePrice.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.layoutTotalFeePrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTotalVatPrice.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.layoutTotalVatPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCSave.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.bbtnRCSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCPrint.Caption = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.bbtnRCPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMobaPrescriptionCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModuleBase != null)
                {
                    this.Text = this.currentModuleBase.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMediStock.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTracking_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTracking.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(hisExpMest.TDL_TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));

                        listArgs.Add((Action<HIS_TRACKING>)ProcessAfterChangeTrackingTime);

                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();

                        //Load lại tracking
                        LoadComboboxTracking();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessAfterChangeTrackingTime(HIS_TRACKING obj)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Co loi khi delegate ProcessAfterChangeTrackingTime duoc goi tu chuc nang tao/sua to dieu tri", ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtKeyword.Text.Trim()))
                {
                    List<VHisExpMestMaterialADO> lstExpMestMaterialADO = listExpMestMaterialADO.Where(o => o.SERIAL_NUMBER == txtKeyword.Text.Trim()).ToList();
                    gridControlExpMestMaterial.BeginUpdate();
                    if (lstExpMestMaterialADO != null && lstExpMestMaterialADO.Count > 0)
                    {
                        gridControlExpMestMaterial.DataSource = lstExpMestMaterialADO;
                        gridViewExpMestMaterial.SelectAll();
                    }
                    else
                    {
                        gridControlExpMestMaterial.DataSource = null;
                        XtraMessageBox.Show("Không tồn tại vật tư với số seri cần tìm");
                    }
                    gridControlExpMestMaterial.EndUpdate();
                }
                else
                {
                    gridControlExpMestMaterial.BeginUpdate();
                    gridControlExpMestMaterial.DataSource = listExpMestMaterialADO;
                    List<int> selectRows = new List<int>();
                    for (int i = 0; i < gridViewExpMestMaterial.RowCount; i++)
                    {
                        var data = (VHisExpMestMaterialADO)gridViewExpMestMaterial.GetRow(i);
                        if (data != null && data.IsMoba)
                        {
                            selectRows.Add(i);
                        }
                    }
                    if (selectRows != null && selectRows.Count > 0)
                    {
                        foreach (var item in selectRows)
                        {
                            gridViewExpMestMaterial.SelectRow(item);
                        }
                    }
                        gridControlExpMestMaterial.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void xtraTabControlMain_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)//thuốc
                {
                        this.lciKeyword.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.lciSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else if(xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                {
                    if (listExpMestMaterialADO != null && listExpMestMaterialADO.Count > 0 && listExpMestMaterialADO.FirstOrDefault(o => !String.IsNullOrEmpty(o.SERIAL_NUMBER)) != null)
                    {
                        this.lciKeyword.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.lciSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                    else
                    {
                        this.lciKeyword.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.lciSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
}
    }
}
