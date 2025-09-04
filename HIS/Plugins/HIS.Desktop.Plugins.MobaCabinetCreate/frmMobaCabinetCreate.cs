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
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MobaCabinetCreate.ADO;
using HIS.Desktop.Plugins.MobaCabinetCreate;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Common.RichEditor.Base;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.MobaCabinetCreate
{
    public partial class frmMobaCabinetCreate : HIS.Desktop.Utility.FormBase
    {
        long expMestId;
        V_HIS_EXP_MEST hisExpMest = null;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        int positionHandle = -1;

        List<VHisExpMestMedicineADO> listExpMestMedicineADO = new List<VHisExpMestMedicineADO>();
        List<VHisExpMestMaterialADO> listExpMestMaterialADO = new List<VHisExpMestMaterialADO>();
        List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine;
        List<V_HIS_EXP_MEST_MATERIAL> listExpMestMaterial;

        HisImpMestResultSDO resultMobaSdo = null;
        List<HisImpMestResultSDO> impMestResultSdoList = null;

        public frmMobaCabinetCreate(Inventec.Desktop.Common.Modules.Module module, long data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                HisConfigCFG.LoadConfig();
                if (HisConfigCFG.IsAutoSelectImpMediStock)
                {
                    gridColumn_ExpMestMedicine_ImpMediStockName.Visible = true;
                    gridColumn_ExpMestMaterial_ImpMediStockName.Visible = true;
                }
                else
                {
                    gridColumn_ExpMestMedicine_ImpMediStockName.Visible = false;
                    gridColumn_ExpMestMaterial_ImpMediStockName.Visible = false;
                }
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

        private void frmMobaCabinetCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyFrmLanguage();
                LoadExpMest();
                LoadExpMestMedicine();
                LoadExpMestMaterial();
                btnSave.Enabled = true;
                btnPrint.Enabled = false;
                LoadComboImpMediStock();
                SetMediStock();
                if (HisConfigCFG.IsTrackingRequired)
                {
                    this.lciTracking.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    ValidationSingleControl(cboTracking);
                }
                SetComboTracking();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetComboTracking()
        {
            try
            {

                CommonParam param = new CommonParam();
                long departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                HisTrackingFilter filter = new HisTrackingFilter();
                filter.TREATMENT_ID = hisExpMest.TDL_TREATMENT_ID;
                filter.DEPARTMENT_ID = departmentId;
                List<HIS_TRACKING> trackings = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, filter, param);

                trackings = trackings.OrderByDescending(o => o.TRACKING_TIME).ToList();

                List<TrackingADO> trackingADOs = new List<TrackingADO>();
                foreach (var item in trackings)
                {
                    TrackingADO tracking = new TrackingADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TrackingADO>(tracking, item);
                    tracking.TRACKING_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(tracking.TRACKING_TIME);
                    trackingADOs.Add(tracking);
                }
                trackingADOs = trackingADOs.OrderByDescending(o => o.TRACKING_TIME).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TRACKING_TIME_STR", "Thời gian", 250, 1));
                columnInfos.Add(new ColumnInfo("CREATOR", "Người tạo", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TRACKING_TIME_STR", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboTracking, trackingADOs, controlEditorADO);
                cboTracking.Properties.ImmediatePopup = true;
                cboTracking.EditValue = null;
                if (HisConfigCFG.IsTrackingRequired)
                {
                    var startDay = Inventec.Common.DateTime.Get.StartDay() ?? 0;
                    var endDay = Inventec.Common.DateTime.Get.EndDay() ?? 0;
                    if (trackingADOs != null && trackingADOs.Count > 0)
                    {
                        foreach (var item in trackingADOs)
                        {
                            if (item.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                                && item.TRACKING_TIME <= endDay && item.TRACKING_TIME >= startDay)
                            {
                                cboTracking.EditValue = item.ID;
                                break;
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

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetMediStock()
        {
            try
            {
                if (HisConfigCFG.IsMobaIntoMediStockExport)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                    ControlEditorLoader.Load(cboImpMediStock, BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => p.IS_ACTIVE == 1).ToList(), controlEditorADO);

                    cboImpMediStock.EditValue = this.hisExpMest.MEDI_STOCK_ID;
                    cboImpMediStock.ReadOnly = true;
                }
                else
                {
                    cboImpMediStock.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboImpMediStock()
        {
            try
            {
                List<V_HIS_MEDI_STOCK> data = new List<V_HIS_MEDI_STOCK>();
                var currentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.hisExpMest.MEDI_STOCK_ID);
                if (currentMediStock != null)
                {
                    var _RoomIds = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.MEDI_STOCK_ID == currentMediStock.ID).Select(p => p.ROOM_ID).Distinct().ToList();
                    //var expMestMedistock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_BUSINESS != 1 && o.IS_CABINET != 1 && o.DEPARTMENT_ID == currentMediStock.DEPARTMENT_ID).ToList();
                    // if (expMestMedistock != null && expMestMedistock.Count > 0)
                    //{
                    if (_RoomIds != null && _RoomIds.Count > 0)
                    {
                        //data = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => 
                        //    _RoomIds.Contains(o.ROOM_ID) 
                        //    && (o.IS_BUSINESS != 1 && o.IS_CABINET != 1)
                        //    ).ToList();//Code Cu

                        //#15627
                        if (currentMediStock.IS_BUSINESS == 1)
                        {
                            data = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o =>
                           _RoomIds.Contains(o.ROOM_ID)
                           && o.IS_BUSINESS == 1
                           ).ToList();
                        }
                        else
                        {
                            data = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o =>
                           _RoomIds.Contains(o.ROOM_ID)
                           && o.IS_BUSINESS != 1
                           ).ToList();
                        }
                    }

                    //}
                }
                if (data != null && data.Count > 0 && !HisConfigCFG.IsMobaIntoMediStockExport)
                {
                    data = data.Where(o => o.IS_CABINET != 1).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboImpMediStock, data, controlEditorADO);
                if (data != null && data.Count == 1)
                {
                    cboImpMediStock.EditValue = data[0].ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        var AllMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == 1).ToList();

                        List<HIS_MEDI_STOCK_METY> mediStockMetyList = null;
                        if (HisConfigCFG.IsAutoSelectImpMediStock)
                        {
                            List<long> MedicineTypeIdList = listExpMestMedicine.Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                            MOS.Filter.HisMediStockMetyFilter mediStockMetyFilter = new HisMediStockMetyFilter();
                            mediStockMetyFilter.MEDICINE_TYPE_IDs = MedicineTypeIdList;
                            if (this.hisExpMest != null)
                                mediStockMetyFilter.MEDI_STOCK_ID = this.hisExpMest.MEDI_STOCK_ID;
                            mediStockMetyList = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_STOCK_METY>>("api/HisMediStockMety/Get", ApiConsumer.ApiConsumers.MosConsumer, mediStockMetyFilter, null);
                        }

                        var group = listExpMestMedicine.GroupBy(o => new { o.MEDICINE_ID, o.PATIENT_TYPE_ID, o.IS_EXPEND, o.IS_OUT_PARENT_FEE }).ToList();
                        foreach (var item in group)
                        {
                            VHisExpMestMedicineADO medi = new VHisExpMestMedicineADO(item.First());
                            medi.AMOUNT = item.Sum(s => s.AMOUNT);
                            medi.CAN_MOBA_AMOUNT = item.Sum(s => s.AMOUNT) - item.Sum(p => p.TH_AMOUNT ?? 0);
                            var mediStockMetyCheck = mediStockMetyList != null && mediStockMetyList.Count > 0
                                ? mediStockMetyList.FirstOrDefault(o => o.MEDICINE_TYPE_ID == medi.MEDICINE_TYPE_ID)
                                : null;
                            if (mediStockMetyCheck != null)
                            {
                                medi.IMP_MEDI_STOCK_ID = mediStockMetyCheck.EXP_MEDI_STOCK_ID;
                                var mediStock = AllMediStock.FirstOrDefault(o => o.ID == medi.IMP_MEDI_STOCK_ID);
                                medi.IMP_MEDI_STOCK_NAME = mediStock != null ? mediStock.MEDI_STOCK_NAME : "";
                            }

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
                        var AllMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == 1).ToList();

                        List<HIS_MEDI_STOCK_MATY> mediStockMatyList = null;
                        if (HisConfigCFG.IsAutoSelectImpMediStock)
                        {
                            List<long> MaterialTypeIdList = listExpMestMaterial.Select(o => o.MATERIAL_TYPE_ID).Distinct().ToList();
                            MOS.Filter.HisMediStockMatyFilter mediStockMatyFilter = new HisMediStockMatyFilter();
                            mediStockMatyFilter.MATERIAL_TYPE_IDs = MaterialTypeIdList;
                            if (this.hisExpMest != null)
                                mediStockMatyFilter.MEDI_STOCK_ID = this.hisExpMest.MEDI_STOCK_ID;
                            mediStockMatyList = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_STOCK_MATY>>("api/HisMediStockMaty/Get", ApiConsumer.ApiConsumers.MosConsumer, mediStockMatyFilter, null);
                        }
                        var group = listExpMestMaterial.GroupBy(o => new { o.MATERIAL_ID, o.PATIENT_TYPE_ID, o.IS_EXPEND, o.IS_OUT_PARENT_FEE }).ToList();
                        foreach (var item in group)
                        {
                            VHisExpMestMaterialADO mate = new VHisExpMestMaterialADO(item.First());
                            mate.AMOUNT = item.Sum(s => s.AMOUNT);
                            mate.CAN_MOBA_AMOUNT = item.Sum(s => s.AMOUNT) - item.Sum(p => p.TH_AMOUNT ?? 0);
                            var mediStockMetyCheck = mediStockMatyList != null && mediStockMatyList.Count > 0
                               ? mediStockMatyList.FirstOrDefault(o => o.MATERIAL_TYPE_ID == mate.MATERIAL_TYPE_ID)
                               : null;
                            if (mediStockMetyCheck != null)
                            {
                                mate.IMP_MEDI_STOCK_ID = mediStockMetyCheck.EXP_MEDI_STOCK_ID;
                                var mediStock = AllMediStock.FirstOrDefault(o => o.ID == mate.IMP_MEDI_STOCK_ID);
                                mate.IMP_MEDI_STOCK_NAME = mediStock != null ? mediStock.MEDI_STOCK_NAME : "";
                            }
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
                        else if (data.IsMoba)
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

        private void gridViewExpMestMedicine_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {

        }

        private void gridViewExpMestMedicine_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {

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
                        else if (data.IsMoba && data.IS_EXPORT == 1)
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
                gridViewExpMestMaterial.PostEditor();
                gridViewExpMestMedicine.PostEditor();
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                if (!btnSave.Enabled || this.expMestId <= 0 || this.hisExpMest == null)
                    return;

                if (cboImpMediStock.EditValue == null && !HisConfigCFG.IsAutoSelectImpMediStock)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn kho nhập", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cboImpMediStock.EditValue != null)
                {
                    if ((long)cboImpMediStock.EditValue != this.hisExpMest.MEDI_STOCK_ID)
                    {
                        if (MessageBox.Show(String.Format("Kho nhập khác với Kho xuất {0}. Bạn có muốn tiếp tục?", this.hisExpMest.MEDI_STOCK_NAME), "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }
                }

                if (!CheckExpDate())
                    return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool sucsess = false;
                CreateMobaImpMest(param, ref sucsess);
                if (sucsess)
                {
                    listExpMestMaterialADO = new List<VHisExpMestMaterialADO>();
                    listExpMestMedicineADO = new List<VHisExpMestMedicineADO>();
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

        private bool CheckExpDate()
        {
            bool success = true;
            try
            {
                var listMedicine = listExpMestMedicineADO != null && listExpMestMedicineADO.Count > 0 ? listExpMestMedicineADO.Where(o => o.IsMoba).ToList() : null;
                var listMaterial = listExpMestMaterialADO != null && listExpMestMaterialADO.Count > 0 ? listExpMestMaterialADO.Where(o => o.IsMoba).ToList() : null;

                List<VHisExpMestMedicineADO> checkHasImpMediStockMedicine = null;
                List<VHisExpMestMaterialADO> checkHasImpMediStockMaterial = null;

                if (HisConfigCFG.IsAutoSelectImpMediStock && cboImpMediStock.EditValue == null)
                {
                    if (listMedicine != null && listMedicine.Count > 0)
                        checkHasImpMediStockMedicine = listMedicine.Where(o => !o.IMP_MEDI_STOCK_ID.HasValue || o.IMP_MEDI_STOCK_ID == 0).ToList();
                    if (listMaterial != null && listMaterial.Count > 0)
                        checkHasImpMediStockMaterial = listMaterial.Where(o => !o.IMP_MEDI_STOCK_ID.HasValue || o.IMP_MEDI_STOCK_ID == 0).ToList();

                }

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

                if ((checkHasImpMediStockMedicine != null && checkHasImpMediStockMedicine.Count > 0)
                       || (checkHasImpMediStockMaterial != null && checkHasImpMediStockMaterial.Count > 0))
                {
                    string mess = "";
                    List<string> arrMess = new List<string>();

                    if (checkHasImpMediStockMedicine != null && checkHasImpMediStockMedicine.Count > 0
)
                        arrMess.AddRange(checkHasImpMediStockMedicine.Select(o => o.MEDICINE_TYPE_NAME).Distinct());
                    if (checkHasImpMediStockMaterial != null && checkHasImpMediStockMaterial.Count > 0)
                        arrMess.AddRange(checkHasImpMediStockMaterial.Select(o => o.MATERIAL_TYPE_NAME).Distinct());
                    mess = String.Join(", ", arrMess);

                    success = false;
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Các thuốc/vật tư {0} chưa có thông tin kho nhập. Vui lòng chọn kho nhập", mess));
                }

            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private void CreateMobaImpMest(CommonParam param, ref bool sucsess)
        {
            try
            {
                List<HisMobaPresMedicineSDO> MobaPresMedicines = new List<HisMobaPresMedicineSDO>();
                List<HisMobaPresMaterialSDO> MobaPresMaterials = new List<HisMobaPresMaterialSDO>();

                var listMedicine = listExpMestMedicineADO != null && listExpMestMedicineADO.Count > 0 ? listExpMestMedicineADO.Where(o => o.IsMoba).ToList() : null;
                var listMaterial = listExpMestMaterialADO != null && listExpMestMaterialADO.Count > 0 ? listExpMestMaterialADO.Where(o => o.IsMoba).ToList() : null;

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
                            if (groupFirst.MEDICINE_ID == item.MEDICINE_ID && groupFirst.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID && groupFirst.IS_EXPEND == item.IS_EXPEND && groupFirst.IS_OUT_PARENT_FEE == item.IS_OUT_PARENT_FEE)
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
                                        MobaPresMedicines.Add(medi);
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

                        var groupMaterial = listExpMestMaterial.GroupBy(o => new { o.MATERIAL_ID, o.PATIENT_TYPE_ID, o.IS_EXPEND, o.IS_OUT_PARENT_FEE }).ToList();

                        foreach (var group in groupMaterial)
                        {
                            var groupFirst = group.FirstOrDefault();
                            if (groupFirst.MATERIAL_ID == item.MATERIAL_ID && groupFirst.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID && groupFirst.IS_EXPEND == item.IS_EXPEND && groupFirst.IS_OUT_PARENT_FEE == item.IS_OUT_PARENT_FEE)
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
                                        MobaPresMaterials.Add(medi);
                                    }
                                }
                            }
                        }
                    }
                }

                if (HisConfigCFG.IsAutoSelectImpMediStock)
                {
                    HisImpMestMobaPresCabinetSDO data = new HisImpMestMobaPresCabinetSDO();
                    data.Description = this.txtDescription.Text;
                    data.ExpMestId = this.expMestId;
                    if (cboImpMediStock.EditValue != null)
                    {
                        data.ImpMediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboImpMediStock.EditValue.ToString());
                    }
                    if (cboTracking.EditValue != null)
                    {
                        data.TrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTracking.EditValue.ToString());
                    }
                    data.MobaPresMedicines = new List<HisMobaPresMedicineSDO>();
                    data.MobaPresMaterials = new List<HisMobaPresMaterialSDO>();
                    data.MobaPresMaterials = MobaPresMaterials;
                    data.MobaPresMedicines = MobaPresMedicines;
                    data.WorkingRoomId = this.currentModule.RoomId;

                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HisImpMestResultSDO>>("api/HisImpMest/MobaPresCabinetCreate", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        sucsess = true;
                        impMestResultSdoList = rs;
                    }
                }
                else
                {
                    HisImpMestMobaPresSDO data = new HisImpMestMobaPresSDO();
                    data.MobaPresMedicines = new List<HisMobaPresMedicineSDO>();
                    data.MobaPresMaterials = new List<HisMobaPresMaterialSDO>();
                    data.MobaPresMedicines = MobaPresMedicines;
                    data.MobaPresMaterials = MobaPresMaterials;
                    data.ExpMestId = this.expMestId;
                    data.Description = this.txtDescription.Text;
                    data.RequestRoomId = this.currentModule.RoomId;
                    data.ImpMediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboImpMediStock.EditValue.ToString());
                    if (cboTracking.EditValue != null)
                    {
                        data.TrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTracking.EditValue.ToString());
                    }
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestResultSDO>("api/HisImpMest/MobaPresCreate", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        sucsess = true;
                        resultMobaSdo = rs;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                sucsess = false;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled)
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
                if (this.resultMobaSdo == null && this.impMestResultSdoList == null)
                    return result;

                if (this.resultMobaSdo != null)
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    MOS.Filter.HisExpMestMedicineViewFilter hisExpMestMedicineViewFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                    hisExpMestMedicineViewFilter.EXP_MEST_ID = this.hisExpMest.ID;
                    var expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, hisExpMestMedicineViewFilter, null);

                    MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialViewFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                    expMestMaterialViewFilter.EXP_MEST_ID = this.hisExpMest.ID;
                    var expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialViewFilter, null);


                    SingleKey singleKey = new SingleKey();
                    singleKey.AGGR_EXP_MEST_CODE = hisExpMest.TDL_AGGR_EXP_MEST_CODE;

                    WaitingManager.Hide();
                    MPS.Processor.Mps000084.PDO.Mps000084PDO rdo = new MPS.Processor.Mps000084.PDO.Mps000084PDO(this.resultMobaSdo.ImpMest, this.hisExpMest, singleKey, this.resultMobaSdo.ImpMedicines, this.resultMobaSdo.ImpMaterials, expMestMedicines, expMestMaterials);

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest != null ? this.hisExpMest.TDL_TREATMENT_CODE : "", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO });
                }
                else
                {
                    foreach (var impMestResult in this.impMestResultSdoList)
                    {
                        CommonParam param = new CommonParam();
                        WaitingManager.Show();
                        MOS.Filter.HisExpMestMedicineViewFilter hisExpMestMedicineViewFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                        hisExpMestMedicineViewFilter.EXP_MEST_ID = this.hisExpMest.ID;
                        var expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, hisExpMestMedicineViewFilter, null);

                        MOS.Filter.HisExpMestMaterialViewFilter expMestMaterialViewFilter = new MOS.Filter.HisExpMestMaterialViewFilter();
                        expMestMaterialViewFilter.EXP_MEST_ID = this.hisExpMest.ID;
                        var expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expMestMaterialViewFilter, null);


                        SingleKey singleKey = new SingleKey();
                        singleKey.AGGR_EXP_MEST_CODE = hisExpMest.TDL_AGGR_EXP_MEST_CODE;

                        WaitingManager.Hide();
                        MPS.Processor.Mps000084.PDO.Mps000084PDO rdo = new MPS.Processor.Mps000084.PDO.Mps000084PDO(impMestResult.ImpMest, this.hisExpMest, singleKey, impMestResult.ImpMedicines, impMestResult.ImpMaterials, expMestMedicines, expMestMaterials);

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest != null ? this.hisExpMest.TDL_TREATMENT_CODE : "", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MobaCabinetCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.MobaCabinetCreate.frmMobaCabinetCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboImpMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.cboImpMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCSave.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.bbtnRCSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCPrint.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.bbtnRCPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.xtraTabPageMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_Stt.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_MedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypName.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_MedicineTypName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_MobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_Amount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_CanMobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_CanMobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_ImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VatRatio.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_VatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VirTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_VirTotalImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_BidNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_BidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_PackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_RegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_RegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_ExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.xtraTabPageMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_Stt.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_MaterialTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_MaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_MobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_Amount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_CanMobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_CanMobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_ImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_VatRatio.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_VatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_VirTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_VirTotalImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_BidNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_BidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_PackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_RegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_RegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_ExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTotalPrice.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutTotalPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTotalFeePrice.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutTotalFeePrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTotalVatPrice.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutTotalVatPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void cboTracking_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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
                        listArgs.Add(this.hisExpMest.TDL_TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();

                        this.SetComboTracking();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTracking_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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
    }
}
