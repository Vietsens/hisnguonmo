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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MobaExamPresCreate.ADO;
using HIS.Desktop.Plugins.MobaExamPresCreate.Config;
using HIS.Desktop.Plugins.MobaExamPresCreate.Resource;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MobaExamPresCreate
{
    public partial class frmImpMobaCreate : HIS.Desktop.Utility.FormBase
    {
        private long serviceReqId;
        private V_HIS_EXP_MEST hisExpMest = null;
        private Inventec.Desktop.Common.Modules.Module currentModule = null;

        private List<SereServADO> ListMedicineADO = new List<SereServADO>();
        private List<SereServADO> ListMaterialADO = new List<SereServADO>();

        private HisImpMestResultSDO resultSDO = null;
        int positionHandle = -1;
        public frmImpMobaCreate(Inventec.Desktop.Common.Modules.Module module, long data)
        {
            InitializeComponent();
            this.SetIcon();
            this.currentModule = module;
            this.serviceReqId = data;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }
            HisConfigCFG.LoadConfig();
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

        private void frmImpMobaCreate_Load(object sender, EventArgs e)
        {
            try
            {
                if (!HisConfigCFG.ALLOW_MOBA_EXAM_PRES)
                {
                    this.DisableControl();
                }
                else
                {
                    WaitingManager.Show();
                    btnSave.Enabled = true;
                    btnPrint.Enabled = false;
                    this.LoadExpMest();
                    this.LoadDataToGrid();
                    LoadComboboxTracking();
                    if (this.ListMedicineADO != null && this.ListMedicineADO.Count > 0)
                    {
                        xtraTabControlMain.SelectedTabPage = xtraTabPageMedicine;
                    }
                    else if (this.ListMaterialADO != null && this.ListMaterialADO.Count > 0)
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
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TrackingTimeStr", "Thời gian", 150, 1));
                columnInfos.Add(new ColumnInfo("CREATOR", "Người tạo", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TrackingTimeStr", "ID", columnInfos, true, 300);
                ControlEditorLoader.Load(this.cboTracking, data, controlEditorADO);
                if (lstTracking != null && lstTracking.Count() > 0)
                {
                    if (HisConfigs.Get<string>("HIS.Desktop.Plugins.MobaCreate.IsTrackingRequired") == "1"
                        && data.Exists(o => o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                        && CheckTrackingTime(o.TRACKING_TIME)))
                    {
                        cboTracking.EditValue = data.Where(o => o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                        && CheckTrackingTime(o.TRACKING_TIME)).OrderByDescending(o => o.TRACKING_TIME).First().ID;
                    }
                }
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

        private void LoadExpMest()
        {
            try
            {
                if (this.serviceReqId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisExpMestFilter expMestFilter = new HisExpMestFilter();
                    expMestFilter.SERVICE_REQ_ID = this.serviceReqId;
                    List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, param);
                    if (expMests == null || expMests.Count <= 0)
                    {
                        this.DisableControl();
                    }
                    else
                    {
                        this.hisExpMest = expMests.FirstOrDefault();
                        if (this.hisExpMest.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            this.DisableControl();
                        }
                    }
                }
                else
                {
                    this.DisableControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGrid()
        {
            try
            {
                this.ListMedicineADO = new List<SereServADO>();
                this.ListMaterialADO = new List<SereServADO>();
                if (this.hisExpMest != null)
                {
                    CommonParam param = new CommonParam();
                    HisSereServView3Filter ssFilter = new HisSereServView3Filter();
                    ssFilter.SERVICE_REQ_ID = this.serviceReqId;
                    List<V_HIS_SERE_SERV_3> sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_3>>("api/HisSereServ/GetView3", ApiConsumer.ApiConsumers.MosConsumer, ssFilter, param);
                    sereServs = sereServs != null ? sereServs.Where(o => o.AMOUNT > 0 && (o.MEDICINE_ID.HasValue || o.MATERIAL_ID.HasValue) && o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT).ToList() : null;
                    if (sereServs != null && sereServs.Count > 0)
                    {
                        if (sereServs.Exists(e => e.MEDICINE_ID.HasValue))
                        {
                            HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                            mediFilter.EXP_MEST_ID = this.hisExpMest.ID;
                            mediFilter.IS_EXPORT = true;
                            List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, mediFilter, null);

                            if (expMestMedicines == null)
                            {
                                throw new Exception("expMestMedicines is null");
                            }

                            var GroupMedicines = sereServs.Where(o => o.MEDICINE_ID.HasValue && o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT).GroupBy(g => g.MEDICINE_ID).ToList();
                            foreach (var group in GroupMedicines)
                            {
                                SereServADO ado = new SereServADO(group.ToList(), expMestMedicines.Where(o => o.MEDICINE_ID == group.Key).ToList());
                                this.ListMedicineADO.Add(ado);
                            }
                        }

                        if (sereServs.Exists(e => e.MATERIAL_ID.HasValue))
                        {
                            HisExpMestMaterialViewFilter mateFilter = new HisExpMestMaterialViewFilter();
                            mateFilter.EXP_MEST_ID = this.hisExpMest.ID;
                            mateFilter.IS_EXPORT = true;
                            List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, mateFilter, null);

                            if (expMestMaterials == null)
                            {
                                throw new Exception("expMestMaterials is null");
                            }

                            var GroupMaterials = sereServs.Where(o => o.MATERIAL_ID.HasValue && o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT).GroupBy(g => g.MATERIAL_ID).ToList();
                            foreach (var group in GroupMaterials)
                            {
                                SereServADO ado = new SereServADO(group.ToList(), expMestMaterials.Where(o => o.MATERIAL_ID == group.Key).ToList());
                                this.ListMaterialADO.Add(ado);
                            }
                        }
                    }
                    else
                    {
                        this.DisableControl();
                    }
                }

                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = this.ListMedicineADO;
                gridControlMedicine.EndUpdate();

                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = this.ListMaterialADO;
                gridControlMaterial.EndUpdate();
            }
            catch (Exception ex)
            {
                this.DisableControl();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DisableControl()
        {
            try
            {
                btnSave.Enabled = false;
                btnPrint.Enabled = false;
                txtReason.Enabled = false;
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
                if (this.ListMaterialADO != null && this.ListMaterialADO.Count > 0)
                {
                    var listSelect = this.ListMaterialADO.Where(o => o.IsCheck).ToList();
                    if (listSelect != null && listSelect.Count > 0)
                    {
                        totalFeePrice += listSelect.Sum(s => ((s.PRICE) * s.TH_AMOUNT));
                        totalVatPrice += listSelect.Sum(s => ((s.PRICE) * s.TH_AMOUNT * (s.VAT_RATIO / (decimal)100)));
                    }
                }
                if (this.ListMedicineADO != null && this.ListMedicineADO.Count > 0)
                {
                    var listSelect = this.ListMedicineADO.Where(o => o.IsCheck).ToList();
                    if (listSelect != null && listSelect.Count > 0)
                    {
                        totalFeePrice += listSelect.Sum(s => ((s.PRICE) * s.TH_AMOUNT));
                        totalVatPrice += listSelect.Sum(s => ((s.PRICE) * s.TH_AMOUNT * (s.VAT_RATIO / (decimal)100)));
                    }
                }
                totalVatPrice = Math.Round(totalVatPrice, 4);
                totalPrice = totalFeePrice + totalVatPrice;
                lblTotalImpPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalFeePrice, 4);
                lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 4);
                lblTotalVatPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalVatPrice, 4);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound && e.Row != null)
                {
                    var data = (SereServADO)e.Row;
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "TH_AMOUNT")
                {
                    var data = (SereServADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsCheck)
                        {
                            e.RepositoryItem = repositoryItemSpinMedicineThAmount;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemSpinMedicineThAmountDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                for (int i = 0; i < gridViewMedicine.DataRowCount; i++)
                {
                    var data = (SereServADO)gridViewMedicine.GetRow(i);
                    if (data != null)
                    {
                        if (gridViewMedicine.IsRowSelected(i))
                        {
                            data.IsCheck = true;
                            data.TH_AMOUNT = data.AVAI_AMOUNT;
                        }
                        else
                        {
                            data.IsCheck = false;
                            data.TH_AMOUNT = 0;
                        }
                    }
                    CalculTotalPrice();
                }
                gridControlMedicine.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "TH_AMOUNT")
                {
                    this.CalculTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound && e.Row != null)
                {
                    var data = (SereServADO)e.Row;
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "TH_AMOUNT")
                {
                    var data = (SereServADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsCheck)
                        {
                            e.RepositoryItem = repositoryItemSpinMaterialThAmount;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemSpinMaterialThAmountDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                for (int i = 0; i < gridViewMaterial.DataRowCount; i++)
                {
                    var data = (SereServADO)gridViewMaterial.GetRow(i);
                    if (data != null)
                    {
                        if (gridViewMaterial.IsRowSelected(i))
                        {
                            data.IsCheck = true;
                            data.TH_AMOUNT = data.AVAI_AMOUNT;
                        }
                        else
                        {
                            data.IsCheck = false;
                            data.TH_AMOUNT = 0;
                        }
                    }
                    CalculTotalPrice();
                }
                gridControlMaterial.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "TH_AMOUNT")
                {
                    this.CalculTotalPrice();
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
                if (this.serviceReqId <= 0 || this.hisExpMest == null || !this.btnSave.Enabled)
                    return;
                if (!CheckExpDate())
                    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool sucsess = false;
                CreateImpMest(param, ref sucsess);
                if (sucsess)
                {
                    //this.ListMaterialADO = new List<SereServADO>();
                    //this.ListMedicineADO = new List<SereServADO>();
                    //this.LoadDataToGrid();
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckExpDate()
        {
            bool success = true;
            try
            {
                var listMedicine = ListMedicineADO.Where(o => o.IsCheck).ToList();
                var listMaterial = ListMaterialADO.Where(o => o.IsCheck).ToList();

                List<MessPackageNumber> _MessPackageNumbers = new List<MessPackageNumber>();
                if (listMedicine != null && listMedicine.Count > 0 && listMedicine.Count != ListMedicineADO.Count)
                {
                    foreach (var item in listMedicine)
                    {
                        if (item.EXPIRED_DATE != null && item.EXPIRED_DATE > 0)
                        {
                            var dataChecks = ListMedicineADO.Where(p => p.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID).ToList();
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
                                        ado.mess = dataExpDates[0].PACKAGE_NUMBER + " - " + dataExpDates[0].SERVICE_NAME;
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
                                            ado.mess = itemMax.PACKAGE_NUMBER + " - " + itemMax.SERVICE_NAME;
                                            _MessPackageNumbers.Add(ado);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (listMaterial != null && listMaterial.Count > 0 && listMaterial.Count != ListMaterialADO.Count)
                {
                    foreach (var item in listMaterial)
                    {
                        if (item.EXPIRED_DATE != null && item.EXPIRED_DATE > 0)
                        {
                            var dataChecks = ListMaterialADO.Where(p => p.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID).ToList();
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
                                        ado.mess = dataExpDates[0].PACKAGE_NUMBER + " - " + dataExpDates[0].SERVICE_NAME;
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
                                            ado.mess = itemMax.PACKAGE_NUMBER + " - " + itemMax.SERVICE_NAME;
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

        private void CreateImpMest(CommonParam param, ref bool success)
        {
            try
            {
                var listMedicine = this.ListMedicineADO.Where(o => o.IsCheck).ToList();
                var listMaterial = this.ListMaterialADO.Where(o => o.IsCheck).ToList();
                if ((listMaterial == null || listMaterial.Count == 0) && (listMedicine == null || listMedicine.Count == 0))
                {
                    param.Messages.Add(ResourceMessageLang.NguoiDungChuaChonThuocVatTu);
                    return;
                }

                HisImpMestMobaOutPresSDO mobaSDO = new HisImpMestMobaOutPresSDO();
                mobaSDO.Description = txtReason.Text;
                mobaSDO.RequestRoomId = this.currentModule.RoomId;
                mobaSDO.ServiceReqId = this.serviceReqId;
                mobaSDO.TrackingId = cboTracking.EditValue != null ? (long?)Convert.ToInt64(cboTracking.EditValue.ToString()) : null;
                if (listMaterial != null && listMaterial.Count > 0)
                {
                    mobaSDO.MobaPresMaterials = new List<HisMobaPresSereServSDO>();
                    foreach (var ado in listMaterial)
                    {
                        if (ado.TH_AMOUNT <= 0)
                        {
                            param.Messages.Add(ResourceMessageLang.SoLuongThuHoiPhaiLonHonKhong);
                            return;
                        }
                        if (ado.TH_AMOUNT > ado.AVAI_AMOUNT)
                        {
                            param.Messages.Add(ResourceMessageLang.SoLuongThuHoiKhongDuocLonHonSoLuongKhaDung);
                            return;
                        }
                        ado.SereServs = ado.SereServs.OrderByDescending(o => o.AMOUNT).ToList();
                        decimal thAmount = ado.TH_AMOUNT;
                        foreach (var ss in ado.SereServs)
                        {
                            if (thAmount <= 0) break;
                            if (ss.AMOUNT <= 0) break;
                            if (ss.AMOUNT >= thAmount)
                            {
                                HisMobaPresSereServSDO ssSDO = new HisMobaPresSereServSDO();
                                ssSDO.SereServId = ss.ID;
                                ssSDO.Amount = thAmount;
                                ss.AMOUNT = ss.AMOUNT - thAmount;
                                mobaSDO.MobaPresMaterials.Add(ssSDO);
                                break;
                            }
                            else
                            {
                                HisMobaPresSereServSDO ssSDO = new HisMobaPresSereServSDO();
                                ssSDO.SereServId = ss.ID;
                                ssSDO.Amount = ss.AMOUNT;
                                ss.AMOUNT = 0;
                                thAmount = thAmount - ss.AMOUNT;
                                mobaSDO.MobaPresMaterials.Add(ssSDO);
                            }
                        }
                    }
                }

                if (listMedicine != null && listMedicine.Count > 0)
                {
                    mobaSDO.MobaPresMedicines = new List<HisMobaPresSereServSDO>();
                    foreach (var ado in listMedicine)
                    {
                        if (ado.TH_AMOUNT <= 0)
                        {
                            param.Messages.Add(ResourceMessageLang.SoLuongThuHoiPhaiLonHonKhong);
                            return;
                        }
                        if (ado.TH_AMOUNT > ado.AVAI_AMOUNT)
                        {
                            param.Messages.Add(ResourceMessageLang.SoLuongThuHoiKhongDuocLonHonSoLuongKhaDung);
                            return;
                        }
                        ado.SereServs = ado.SereServs.OrderByDescending(o => o.AMOUNT).ToList();
                        decimal thAmount = ado.TH_AMOUNT;
                        foreach (var ss in ado.SereServs)
                        {
                            if (thAmount <= 0) break;
                            if (ss.AMOUNT <= 0) break;
                            if (ss.AMOUNT >= thAmount)
                            {
                                HisMobaPresSereServSDO ssSDO = new HisMobaPresSereServSDO();
                                ssSDO.SereServId = ss.ID;
                                ssSDO.Amount = thAmount;
                                ss.AMOUNT = ss.AMOUNT - thAmount;
                                mobaSDO.MobaPresMedicines.Add(ssSDO);
                                break;
                            }
                            else
                            {
                                HisMobaPresSereServSDO ssSDO = new HisMobaPresSereServSDO();
                                ssSDO.SereServId = ss.ID;
                                ssSDO.Amount = ss.AMOUNT;
                                ss.AMOUNT = 0;
                                thAmount = thAmount - ss.AMOUNT;
                                mobaSDO.MobaPresMedicines.Add(ssSDO);
                            }
                        }
                    }
                }

                var rs = new BackendAdapter(param).Post<HisImpMestResultSDO>("api/HisImpMest/MobaOutPresCreate", ApiConsumers.MosConsumer, mobaSDO, param);
                if (rs != null)
                {
                    success = true;
                    this.resultSDO = rs;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultSDO == null) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuYeuCauNhapThuHoi_MPS000084, delegateRunPrint);
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
                if (this.resultSDO == null)
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


                MPS.Processor.Mps000084.PDO.Mps000084PDO rdo = new MPS.Processor.Mps000084.PDO.Mps000084PDO(this.resultSDO.ImpMest, this.hisExpMest, singleKey, this.resultSDO.ImpMedicines, this.resultSDO.ImpMaterials, expMestMedicines, expMestMaterials);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void barBtnItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridViewMaterial.PostEditor();
                gridViewMedicine.PostEditor();
                this.btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnItemPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.btnPrint_Click(null, null);
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

        private void ProcessAfterChangeTrackingTime(HIS_TRACKING tracking)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

    }
}
