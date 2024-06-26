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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Plugins.Vaccination.ADO;
using ACS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.UC.Paging;
using DevExpress.XtraBars;
using DevExpress.Utils.Menu;
using HIS.Desktop.Plugins.Library.ResourceMessage;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Plugins.Vaccination.UC;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Desktop.CustomControl;

namespace HIS.Desktop.Plugins.Vaccination
{
    public partial class UCVaccination : UserControl
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_VACCINATION currentVaccination;
        HIS_EXP_MEST currentExpMest;
        List<HIS_VACCINATION_REACT> listVaccinationReact;
        List<HIS_VACC_HEALTH_STT> listVaccHealthStt;
        List<HIS_VACC_REACT_PLACE> listVaccReactPlace;
        List<HIS_VACC_REACT_TYPE> listVaccReactType;
        List<ACS_USER> listAcsUser;

        List<HIS_VACC_REACT_TYPE> _vaccReactTypeSelected;

        ToolTipControlInfo lastInfo;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        V_HIS_MEDI_STOCK currentMediStock;
        UCExecute ucExecute;

        #region Construct

        public UCVaccination(Inventec.Desktop.Common.Modules.Module currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCVaccination(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_VACCINATION vaccination)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.currentVaccination = vaccination;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCVaccination_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                gridControlVaccination.ToolTipController = this.toolTipController1;
                this.currentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId && o.ROOM_TYPE_ID == this.currentModule.RoomTypeId);
                ValidateForm();

                LoadCombo();
                SetDefaultValueForm();
                LoadDataToGridVaccination();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Method

        private void SetDefaultValueForm()
        {
            try
            {
                dtNgayTiemTu.DateTime = DateTime.Now;
                dtNgayTiemDen.DateTime = DateTime.Now;
                dtThoiGianTiem.DateTime = DateTime.Now;
                cboNguoiTiem.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                cboStatus.EditValue = "5";
                txtKeyWord.Text = "";
                btnCancelReact.Enabled = false;
                btnSaveExtend.Enabled = false;
                chkNo.Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(Control control, string displayMember, string valueMember, string column1, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();

                if (!string.IsNullOrEmpty(column1))
                {
                    columnInfos.Add(new ColumnInfo(column1, "", 100, 1));
                }

                columnInfos.Add(new ColumnInfo(displayMember, "", 250, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 250);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(RepositoryItem control, string displayMember, string valueMember, string column1, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();

                if (!string.IsNullOrEmpty(column1))
                {
                    columnInfos.Add(new ColumnInfo(column1, "", 100, 1));
                }
                columnInfos.Add(new ColumnInfo(displayMember, "", 250, 2));

                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo1(RepositoryItem control, string displayMember, string valueMember, string column1, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();

                if (!string.IsNullOrEmpty(column1))
                {
                    columnInfos.Add(new ColumnInfo(column1, "", 100, 2));
                }
                columnInfos.Add(new ColumnInfo(displayMember, "", 250, 1));

                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCombo()
        {
            try
            {
                List<StatusADO> listStatus = new List<StatusADO>();
                listStatus.Add(new StatusADO("1", GetResource.Get(KeyMessage.TatCa)));
                listStatus.Add(new StatusADO("2", GetResource.Get(KeyMessage.ChuaXuLy)));
                listStatus.Add(new StatusADO("3", GetResource.Get(KeyMessage.DangXuLy)));
                listStatus.Add(new StatusADO("4", GetResource.Get(KeyMessage.HoanThanh)));
                listStatus.Add(new StatusADO("5", GetResource.Get(KeyMessage.ChuaHoanThanh)));

                CommonParam param = new CommonParam();
                HisVaccinationReactFilter hisVaccinationReactFilter = new HisVaccinationReactFilter();
                hisVaccinationReactFilter.IS_ACTIVE = 1;
                this.listVaccinationReact = new BackendAdapter(param).Get<List<HIS_VACCINATION_REACT>>("api/HisVaccinationReact/Get", ApiConsumer.ApiConsumers.MosConsumer, hisVaccinationReactFilter, param);

                HisVaccinationResultFilter hisVaccinationResultFilter = new HisVaccinationResultFilter();
                hisVaccinationResultFilter.IS_ACTIVE = 1;
                var vaccinationResult = new BackendAdapter(param).Get<List<HIS_VACCINATION_RESULT>>("api/HisVaccinationResult/Get", ApiConsumer.ApiConsumers.MosConsumer, hisVaccinationResultFilter, param);

                HisVaccHealthSttFilter hisVaccHealthSttFilter = new HisVaccHealthSttFilter();
                hisVaccHealthSttFilter.IS_ACTIVE = 1;
                this.listVaccHealthStt = new BackendAdapter(param).Get<List<HIS_VACC_HEALTH_STT>>("api/HisVaccHealthStt/Get", ApiConsumer.ApiConsumers.MosConsumer, hisVaccHealthSttFilter, param);

                HisVaccReactPlaceFilter hisVaccReactPlaceFilter = new HisVaccReactPlaceFilter();
                hisVaccReactPlaceFilter.IS_ACTIVE = 1;
                this.listVaccReactPlace = new BackendAdapter(param).Get<List<HIS_VACC_REACT_PLACE>>("api/HisVaccReactPlace/Get", ApiConsumer.ApiConsumers.MosConsumer, hisVaccReactPlaceFilter, param);

                HisVaccReactTypeFilter hisVaccReactTypeFilter = new HisVaccReactTypeFilter();
                hisVaccReactTypeFilter.IS_ACTIVE = 1;
                this.listVaccReactType = new BackendAdapter(param).Get<List<HIS_VACC_REACT_TYPE>>("api/HisVaccReactType/Get", ApiConsumer.ApiConsumers.MosConsumer, hisVaccReactTypeFilter, param);

                this.listAcsUser = BackendDataWorker.Get<ACS_USER>();

                InitCombo(cboStatus, "STATUS", "ID", "", listStatus);
                InitComboAcsUserSearchCode(cboNguoiTiem);
                InitCombo(cboPhanUng, "VACCINATION_REACT_NAME", "ID", "VACCINATION_REACT_CODE", this.listVaccinationReact);
                InitCombo(cboKetQuaTiem_Enable, "VACCINATION_RESULT_NAME", "ID", "VACCINATION_RESULT_CODE", vaccinationResult);

                InitCheck(cboKetQua, SelectionGrid__VaccReactType);
                InitCombo(cboKetQua, this.listVaccReactType, "VACC_REACT_TYPE_NAME", "ID");

                //InitCombo(cboKetQuaTiem_Disable, "VACCINATION_RESULT_NAME", "ID", "VACCINATION_RESULT_CODE", vaccinationResult);
                LoadComboLoThuoc(null, cboLoVaccine_Enable);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridVaccination()
        {
            try
            {
                WaitingManager.Show();


                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPagingVaccination(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPagingVaccination, param, numPageSize, this.gridControlExpMestMedicine);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPagingVaccination(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(startPage, limit);
                HisVaccinationViewFilter hisVaccinationViewFilter = new HisVaccinationViewFilter();
                hisVaccinationViewFilter.BRANCH_ID = BranchDataWorker.GetCurrentBranchId();
                hisVaccinationViewFilter.ORDER_FIELD = "REQUEST_TIME";
                hisVaccinationViewFilter.ORDER_DIRECTION = "DESC";

                if (cboStatus.EditValue.ToString() == "2")
                {
                    hisVaccinationViewFilter.VACCINATION_STT_ID = IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__NEW;
                }
                else if (cboStatus.EditValue.ToString() == "3")
                {
                    hisVaccinationViewFilter.VACCINATION_STT_ID = IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__PROCESSING;
                }
                else if (cboStatus.EditValue.ToString() == "4")
                {
                    hisVaccinationViewFilter.VACCINATION_STT_ID = IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH; ;
                }
                else if (cboStatus.EditValue.ToString() == "5")
                {
                    var ids = new List<long>();
                    ids.Add(IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__PROCESSING);
                    ids.Add(IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__NEW);
                    hisVaccinationViewFilter.VACCINATION_STT_IDs = ids;
                }

                hisVaccinationViewFilter.KEY_WORD = txtKeyWord.Text.Trim();

                if (dtNgayTiemTu.EditValue != null && dtNgayTiemTu.DateTime != DateTime.MinValue)
                    hisVaccinationViewFilter.REQUEST_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtNgayTiemTu.EditValue).ToString("yyyyMMdd") + "000000");
                if (dtNgayTiemDen.EditValue != null && dtNgayTiemDen.DateTime != DateTime.MinValue)
                    hisVaccinationViewFilter.REQUEST_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtNgayTiemDen.EditValue).ToString("yyyyMMdd") + "000000");

                var rsApi = new BackendAdapter(paramCommon).GetRO<List<V_HIS_VACCINATION>>("api/HisVaccination/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisVaccinationViewFilter, paramCommon);

                gridControlVaccination.BeginUpdate();

                if (rsApi != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_VACCINATION>)rsApi.Data;
                    if (data != null)
                    {
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (rsApi.Param == null ? 0 : rsApi.Param.Count ?? 0);
                    }
                    gridControlVaccination.DataSource = data;
                }

                gridControlVaccination.DataSource = rsApi;
                gridControlVaccination.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridExpMestMedicine(long vaccinationId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter hisExpMestMedicineViewFilter = new HisExpMestMedicineViewFilter();
                hisExpMestMedicineViewFilter.TDL_VACCINATION_ID = vaccinationId;

                var rsApi = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisExpMestMedicineViewFilter, param);

                gridControlExpMestMedicine.BeginUpdate();
                gridControlExpMestMedicine.DataSource = rsApi;
                gridControlExpMestMedicine.EndUpdate();

                if (rsApi != null && rsApi.Count > 0)
                {
                    btnSaveExtend.Enabled = !rsApi.Exists(o => o.VACCINATION_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_VACCINATION_RESULT.ID__UNINJECT);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboLoThuoc(long? medicineTypeId, RepositoryItemGridLookUpEdit control)
        {
            try
            {
                if (this.currentMediStock != null)
                {
                    CommonParam param = new CommonParam();
                    HisMedicineStockViewFilter medicineFilter = new HisMedicineStockViewFilter();
                    medicineFilter.MEDICINE_TYPE_ID = medicineTypeId;
                    medicineFilter.IS_LEAF = 1;
                    medicineFilter.MEDI_STOCK_ID = this.currentMediStock.ID;
                    medicineFilter.MEDICINE_TYPE_IS_ACTIVE = true;
                    medicineFilter.ORDER_DIRECTION = "ASC";
                    medicineFilter.ORDER_FIELD = "MEDICINE_TYPE_NAME";

                    var medicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisMedicineInStockSDO>>("/api/HisMedicine/GetInStockMedicine", ApiConsumers.MosConsumer, medicineFilter, param);

                    if (medicine != null && medicine.Count > 0)
                    {
                        var availableMedicine = medicine.Where(o => (o.AvailableAmount != null && o.AvailableAmount > 0)).ToList();
                        InitCombo1(control, "PACKAGE_NUMBER", "ID", "AvailableAmount", availableMedicine);
                        //InitCombo(control, "PACKAGE_NUMBER", "ID", "AvailableAmount", availableMedicine);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridHistoryVaccination(long patientId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisVaccinationViewFilter hisVaccinationViewFilter = new HisVaccinationViewFilter();
                hisVaccinationViewFilter.PATIENT_ID = patientId;
                hisVaccinationViewFilter.VACCINATION_STT_ID = IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH;

                var rsApi = new BackendAdapter(param).Get<List<V_HIS_VACCINATION>>("api/HisVaccination/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisVaccinationViewFilter, param);

                gridControlHistoryVaccination.BeginUpdate();
                gridControlHistoryVaccination.DataSource = rsApi;
                gridControlHistoryVaccination.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DefaultButton()
        {
            try
            {
                btnDuyet.Enabled = false;
                btnSave.Enabled = false;
                btnThucXuat.Enabled = false;
                ddbKhac.Enabled = false;
                btnSaveExtend.Enabled = false;
                btnCancelReact.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DefaultGrid()
        {
            try
            {
                gridControlExpMestMedicine.DataSource = null;
                gridControlHistoryVaccination.DataSource = null;
                LoadDataToGridVaccination();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DefaultValue()
        {
            try
            {
                this.currentVaccination = null;
                this.currentExpMest = null;
                txtPhanUng.Text = "";
                cboPhanUng.EditValue = null;
                dtReactTime.EditValue = null;
                ResetCheckComboVaccReactType(cboKetQua, null);
                txtTienSuBenhTat.Text = "";
                cboKetQua.Enabled = false;
                cboKetQua.Enabled = true;
                chkNo.Checked = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetButtonByExpMest(HIS_EXP_MEST data)
        {
            try
            {
                gridControlExpMestMedicine.RefreshDataSource();
                if (data != null)
                {
                    if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                    {
                        btnDuyet.Enabled = true;
                        btnThucXuat.Enabled = false;
                        ddbKhac.Enabled = false;
                    }
                    else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                    {
                        btnDuyet.Enabled = false;
                        btnThucXuat.Enabled = true;
                        ddbKhac.Enabled = true;
                    }
                    else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        btnDuyet.Enabled = false;
                        btnThucXuat.Enabled = false;
                        ddbKhac.Enabled = true;
                    }
                }
                else
                {
                    btnDuyet.Enabled = false;
                    btnThucXuat.Enabled = false;
                    ddbKhac.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GenerateMenuKhac()
        {
            try
            {
                if (this.currentExpMest != null)
                {
                    DXPopupMenu menu = new DXPopupMenu();
                    if (this.currentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                    {
                        menu.Items.Add(new DXMenuItem("Hủy duyệt", new EventHandler(ClickHuyDuyet)));
                    }
                    if (this.currentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        menu.Items.Add(new DXMenuItem("Hủy thực xuất", new EventHandler(ClickHuyThucXuat)));
                    }

                    ddbKhac.DropDownControl = menu;
                }
                else
                {
                    ddbKhac.DropDownControl = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataReactToControl(V_HIS_VACCINATION data)
        {
            try
            {
                if (ucExecute != null)
                {
                    dxValidationProvider2.SetValidationRule(ucExecute.txtReactReporter, null);
                    dxValidationProvider2.SetValidationRule(ucExecute.txtReactResponser, null);
                    ValidationSingleControlMaxLength(dxValidationProvider2, ucExecute.txtReactReporter, 100, false);
                    ValidationSingleControlMaxLength(dxValidationProvider2, ucExecute.txtReactResponser, 100, false);

                    InitComboAcsUserSearchCode(ucExecute.cboFollow);
                    InitCombo(ucExecute.cboTinhTrangHienTai, "VACC_HEALTH_STT_NAME", "ID", "VACC_HEALTH_STT_CODE", this.listVaccHealthStt);

                    InitCheck(ucExecute.cboReactPlace, SelectionGrid__VaccReactPlace);
                    InitCombo(ucExecute.cboReactPlace, this.listVaccReactPlace, "VACC_REACT_PLACE_NAME", "ID");

                    ucExecute.txtReactResponser.Text = data.REACT_RESPONSER;
                    ucExecute.txtReactReporter.Text = data.REACT_REPORTER;
                    if (data.DEATH_TIME != null)
                    {
                        ucExecute.dtDeathTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.DEATH_TIME ?? 0) ?? new DateTime();
                    }
                    else
                        ucExecute.dtDeathTime.EditValue = null;
                    ucExecute.cboTinhTrangHienTai.EditValue = data.VACC_HEALTH_STT_ID;
                    if (!string.IsNullOrEmpty(data.FOLLOW_LOGINNAME))
                        ucExecute.cboFollow.EditValue = data.FOLLOW_LOGINNAME;
                    else
                        ucExecute.cboFollow.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    ucExecute.cboReactPlace.Enabled = false;
                    ucExecute.cboReactPlace.Enabled = true;

                    CommonParam param = new CommonParam();
                    HisVaccinationVrplFilter hisVaccinationVrplFilter = new HisVaccinationVrplFilter();
                    hisVaccinationVrplFilter.IS_ACTIVE = 1;
                    hisVaccinationVrplFilter.VACCINATION_ID = data.ID;

                    var vrpl = new BackendAdapter(param).Get<List<HIS_VACCINATION_VRPL>>("api/HisVaccinationVrpl/Get", ApiConsumers.MosConsumer, hisVaccinationVrplFilter, param);
                    if (vrpl != null && vrpl.Count > 0)
                    {
                        ResetCheckComboVaccReactPlace(ucExecute.cboReactPlace, listVaccReactPlace.Where(o => vrpl.Select(p => p.VACC_REACT_PLACE_ID).Contains(o.ID)).ToList());
                    }
                    else
                        ResetCheckComboVaccReactPlace(ucExecute.cboReactPlace, null);

                    ucExecute.cboReactPlace.Enabled = false;
                    ucExecute.cboReactPlace.Enabled = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 400;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 400;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboAcsUserSearchCode(CustomGridLookUpEditWithFilterMultiColumn cbo)
        {
            try
            {
                var listAcsUser = BackendDataWorker.Get<ACS_USER>();


                cbo.Properties.DataSource = listAcsUser;
                cbo.Properties.DisplayMember = "USERNAME";
                cbo.Properties.ValueMember = "LOGINNAME";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(400, 250);

                var aColumnCode = cbo.Properties.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Tài khoản";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                var aColumnName = cbo.Properties.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Người dùng";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetCheckComboVaccReactPlace(GridLookUpEdit cbo, List<HIS_VACC_REACT_PLACE> place)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(place);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetCheckComboVaccReactType(GridLookUpEdit cbo, List<HIS_VACC_REACT_TYPE> type)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(type);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__VaccReactPlace(object sender, EventArgs e)
        {
            try
            {
                if (ucExecute != null)
                {
                    ucExecute._vaccReactPlaceSelected = new List<HIS_VACC_REACT_PLACE>();
                    foreach (HIS_VACC_REACT_PLACE rv in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (rv != null)
                            ucExecute._vaccReactPlaceSelected.Add(rv);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__VaccReactType(object sender, EventArgs e)
        {
            try
            {
                _vaccReactTypeSelected = new List<HIS_VACC_REACT_TYPE>();
                foreach (HIS_VACC_REACT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _vaccReactTypeSelected.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Phim tat
        public void Search()
        {
            if (btnSearch.Enabled)
                btnSearch_Click(null, null);
        }

        public void Save()
        {
            if (btnSave.Enabled)
                btnSave_Click(null, null);
        }

        public void Focus()
        {
            txtKeyWord.Focus();
        }

        public void SaveExtend()
        {
            if (btnSaveExtend.Enabled)
                btnSaveExtend_Click(null, null);
        }
        #endregion

        #endregion

        #region Event form

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGridVaccination();
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
                bool success = false;

                var dataSource = (List<V_HIS_EXP_MEST_MEDICINE>)gridControlExpMestMedicine.DataSource;
                if (dataSource == null || dataSource.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(GetResource.Get(KeyMessage.KhongCoDuLieu), GetResource.Get(KeyMessage.ThongBao));
                    return;
                }

                if (dataSource.Exists(o => o.MEDICINE_ID == null))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(GetResource.Get(KeyMessage.TonTaiDuLieuKhongCoThongTinLoThuoc), GetResource.Get(KeyMessage.ThongBao));
                    return;
                }

                CommonParam param = new CommonParam();

                HisVaccinationProcessSDO hisVaccinationProcessSDO = new HisVaccinationProcessSDO();
                if (cboNguoiTiem.EditValue != null)
                {
                    var acsUser = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboNguoiTiem.EditValue.ToString());
                    if (acsUser != null)
                    {
                        hisVaccinationProcessSDO.ExecuteLoginname = acsUser.LOGINNAME;
                        hisVaccinationProcessSDO.ExecuteUsername = acsUser.USERNAME;
                    }
                }
                if (dtThoiGianTiem.EditValue != null)
                {
                    hisVaccinationProcessSDO.ExecuteTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtThoiGianTiem.DateTime) ?? 0;
                }

                hisVaccinationProcessSDO.WorkingRoomId = this.currentModule.RoomId;
                hisVaccinationProcessSDO.VaccinationId = dataSource.FirstOrDefault().TDL_VACCINATION_ID ?? 0;
                List<HisVaccinationInjectionSDO> hisVaccinationInjectionSDO = new List<HisVaccinationInjectionSDO>();
                foreach (var item in dataSource)
                {
                    HisVaccinationInjectionSDO sdo = new HisVaccinationInjectionSDO();
                    sdo.ExpMestMedicineId = item.ID;
                    sdo.VaccinationResultId = item.VACCINATION_RESULT_ID ?? 0;
                    hisVaccinationInjectionSDO.Add(sdo);
                }
                hisVaccinationProcessSDO.VaccinationInjections = hisVaccinationInjectionSDO;

                var rsApi = new BackendAdapter(param).Post<HIS_VACCINATION>("api/HisVaccination/Process", ApiConsumers.MosConsumer, hisVaccinationProcessSDO, param);
                if (rsApi != null)
                {
                    success = true;
                    LoadDataToGridVaccination();
                    btnSave.Enabled = false;
                }

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveExtend_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;

                if (!dxValidationProvider2.Validate())
                    return;

                if (this.currentVaccination == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(GetResource.Get(KeyMessage.KhongCoThongTinVaccination), GetResource.Get(KeyMessage.ThongBao));
                    return;
                }
                CommonParam param = new CommonParam();
                HisVaccReactInfoSDO sdo = new HisVaccReactInfoSDO();
                sdo.VaccinationId = this.currentVaccination.ID;
                sdo.VaccinationReactId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPhanUng.EditValue.ToString());
                sdo.ReactTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtReactTime.DateTime) ?? 0;
                sdo.PathologicalHistory = txtTienSuBenhTat.Text;
                sdo.IsReactResponse = chkYes.Checked;

                if (_vaccReactTypeSelected != null && _vaccReactTypeSelected.Count > 0)
                {
                    List<HIS_VACCINATION_VRTY> vrty = new List<HIS_VACCINATION_VRTY>();
                    foreach (var item in _vaccReactTypeSelected)
                    {
                        HIS_VACCINATION_VRTY vr = new HIS_VACCINATION_VRTY();
                        vr.VACCINATION_ID = this.currentVaccination.ID;
                        vr.VACC_REACT_TYPE_ID = item.ID;
                        vrty.Add(vr);
                    }
                    sdo.HisVaccinationVrtys = vrty;
                }

                if (ucExecute != null)
                {
                    sdo.ReactReporter = ucExecute.txtReactReporter.Text;
                    sdo.ReactResponser = ucExecute.txtReactResponser.Text;
                    if (ucExecute.dtDeathTime.EditValue != null)
                    {
                        sdo.DeathTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ucExecute.dtDeathTime.DateTime);
                    }
                    else
                        sdo.DeathTime = null;
                    if (ucExecute.cboFollow.EditValue != null)
                    {
                        var user = this.listAcsUser.FirstOrDefault(o => o.LOGINNAME == ucExecute.cboFollow.EditValue.ToString());
                        if (user != null)
                        {
                            sdo.FollowLoginname = user.LOGINNAME;
                            sdo.FollowUsername = user.USERNAME;
                        }
                    }
                    else
                    {
                        sdo.FollowLoginname = "";
                        sdo.FollowUsername = "";
                    }

                    if (ucExecute.cboTinhTrangHienTai.EditValue != null)
                        sdo.VaccHealthSttId = Inventec.Common.TypeConvert.Parse.ToInt64(ucExecute.cboTinhTrangHienTai.EditValue.ToString());
                    else
                        sdo.VaccHealthSttId = null;

                    if (ucExecute._vaccReactPlaceSelected != null && ucExecute._vaccReactPlaceSelected.Count > 0)
                    {
                        List<HIS_VACCINATION_VRPL> vrpl = new List<HIS_VACCINATION_VRPL>();
                        foreach (var item in ucExecute._vaccReactPlaceSelected)
                        {
                            HIS_VACCINATION_VRPL vr = new HIS_VACCINATION_VRPL();
                            vr.VACCINATION_ID = this.currentVaccination.ID;
                            vr.VACC_REACT_PLACE_ID = item.ID;
                            vrpl.Add(vr);
                        }
                        sdo.HisVaccinationVrpls = vrpl;
                    }
                }

                WaitingManager.Show();

                var rsApi = new BackendAdapter(param).Post<HIS_VACCINATION>("api/HisVaccination/UpdateReactInfo", ApiConsumers.MosConsumer, sdo, param);

                if (rsApi != null)
                {
                    success = true;
                    DefaultGrid();
                    LoadDataToGridVaccination();
                    DefaultButton();
                    DefaultValue();
                }

                WaitingManager.Hide();

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhanUng_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhanUng.EditValue != null)
                    {
                        var data = this.listVaccinationReact.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhanUng.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtPhanUng.Text = data.VACCINATION_REACT_CODE;
                            txtPhanUng.Focus();
                            txtPhanUng.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhanUng_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtPhanUng.Text))
                    {
                        cboPhanUng.EditValue = null;
                        cboPhanUng.Focus();
                        cboPhanUng.ShowPopup();
                    }
                    else
                    {
                        List<HIS_VACCINATION_REACT> searchs = null;
                        var listData1 = this.listVaccinationReact.Where(o => o.VACCINATION_REACT_CODE.ToUpper().Contains(txtPhanUng.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.VACCINATION_REACT_CODE.ToUpper() == txtPhanUng.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtPhanUng.Text = searchs[0].VACCINATION_REACT_CODE;
                            cboPhanUng.EditValue = searchs[0].ID;
                            if (dtReactTime.Enabled)
                            {
                                dtReactTime.Focus();
                                dtReactTime.ShowPopup();
                            }
                            else
                            {
                                cboKetQua.Focus();
                                cboKetQua.ShowPopup();
                            }
                        }
                        else
                        {
                            cboPhanUng.Focus();
                            cboPhanUng.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ddbKhac_Click(object sender, EventArgs e)
        {
            try
            {
                ddbKhac.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClickHuyDuyet(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();

                if (this.currentExpMest != null)
                {
                    HisExpMestSDO sdo = new HisExpMestSDO();
                    sdo.ExpMestId = this.currentExpMest.ID;
                    sdo.ReqRoomId = this.currentModule.RoomId;

                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unapprove", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                    if (rs != null)
                    {
                        success = true;
                        this.currentExpMest = rs;
                        SetButtonByExpMest(this.currentExpMest);
                        GenerateMenuKhac();
                    }
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClickHuyThucXuat(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();


                if (this.currentExpMest != null)
                {
                    HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                    hisExpMestApproveSDO.ExpMestId = this.currentExpMest.ID;
                    //hisExpMestApproveSDO.IsFinish = true;
                    hisExpMestApproveSDO.ReqRoomId = this.currentModule.RoomId;

                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
               "api/HisExpMest/Unexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                    if (rs != null)
                    {
                        success = true;
                        this.currentExpMest = rs;
                        this.currentExpMest.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                        SetButtonByExpMest(this.currentExpMest);
                        GenerateMenuKhac();
                    }
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDuyet_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();

                if (this.currentExpMest != null)
                {

                    HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                    hisExpMestApproveSDO.ExpMestId = this.currentExpMest.ID;
                    hisExpMestApproveSDO.IsFinish = true;
                    hisExpMestApproveSDO.ReqRoomId = this.currentModule.RoomId;

                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(
               "api/HisExpMest/Approve", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                    if (rs != null)
                    {
                        success = true;
                        this.currentExpMest = rs.ExpMest;
                        SetButtonByExpMest(this.currentExpMest);
                        GenerateMenuKhac();
                    }
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnThucXuat_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();

                if (this.currentExpMest != null)
                {
                    HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                    sdo.ExpMestId = this.currentExpMest.ID;
                    sdo.ReqRoomId = this.currentModule.RoomId;
                    sdo.IsFinish = true;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                        (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                    if (apiresult != null)
                    {
                        success = true;
                        this.currentExpMest = apiresult;
                        SetButtonByExpMest(this.currentExpMest);
                        GenerateMenuKhac();
                    }
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancelReact_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;

                if (this.currentVaccination == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(GetResource.Get(KeyMessage.KhongCoThongTinVaccination), GetResource.Get(KeyMessage.ThongBao));
                    return;
                }
                HIS_VACCINATION data = new HIS_VACCINATION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_VACCINATION>(data, this.currentVaccination);

                CommonParam param = new CommonParam();
                WaitingManager.Show();
                var rsApi = new BackendAdapter(param).Post<HIS_VACCINATION>("api/HisVaccination/UnReact", ApiConsumers.MosConsumer, data, param);
                if (rsApi != null)
                {
                    success = true;
                    DefaultGrid();
                    LoadDataToGridVaccination();
                    DefaultButton();
                    DefaultValue();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkYes_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkYes.Checked)
                {
                    ucExecute = new UCExecute();
                    ucExecute.Dock = DockStyle.Fill;
                    panelControlExecute.Controls.Add(ucExecute);
                    if (this.currentVaccination != null)
                    {
                        FillDataReactToControl(this.currentVaccination);
                        
                    }
                    chkNo.Checked = false;
                }
                else
                {
                    panelControlExecute.Controls.Remove(ucExecute);
                    ucExecute = null;
                    chkNo.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkNo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkNo.Checked)
                {
                    chkYes.Checked = false;
                }
                else
                {
                    chkYes.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKetQua_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string ReactTypeName = "";
                if (_vaccReactTypeSelected != null && _vaccReactTypeSelected.Count > 0)
                {
                    foreach (var item in _vaccReactTypeSelected)
                    {
                        ReactTypeName += item.VACC_REACT_TYPE_NAME + ", ";
                    }
                }
                e.DisplayText = ReactTypeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlVaccination)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlVaccination.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "ICON_STT")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "VACCINATION_STT_NAME") ?? "").ToString();
                            }

                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKetQua_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboKetQua.Enabled = false;
                cboKetQua.Enabled = true;
                txtTienSuBenhTat.Focus();
                txtTienSuBenhTat.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhanUng_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPhanUng.EditValue != null)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64(cboPhanUng.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_VACCINATION_REACT.ID__NO_REACT)
                    {
                        dxValidationProvider2.SetValidationRule(dtReactTime, null);
                        layoutControlItem6.AppearanceItemCaption.ForeColor = Color.Transparent;
                        dtReactTime.Enabled = false;
                    }
                    else
                    {
                        ValidationSingleControl(dxValidationProvider2, dtReactTime);
                        layoutControlItem6.AppearanceItemCaption.ForeColor = Color.Maroon;
                        dtReactTime.Enabled = true;
                    }
                }
                else
                {
                    ValidationSingleControl(dxValidationProvider2, dtReactTime);
                    layoutControlItem6.AppearanceItemCaption.ForeColor = Color.Maroon;
                    dtReactTime.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtReactTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (dtReactTime.EditValue != null)
                {
                    cboKetQua.Focus();
                    cboKetQua.ShowPopup();
                }
                else
                {
                    dtReactTime.Focus();
                    dtReactTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkYes_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (ucExecute != null)
                    {
                        ucExecute.cboReactPlace.Focus();
                        ucExecute.cboReactPlace.ShowPopup();
                    }
                    else
                        chkNo.Focus();
                    //chkNo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (ucExecute != null)
                    {
                        ucExecute.cboReactPlace.Focus();
                        ucExecute.cboReactPlace.ShowPopup();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event Grid Vaccination

        private void gridViewVaccination_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                var row = (V_HIS_VACCINATION)gridViewVaccination.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = row.VACCINATION_STT_ID == IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH ? Btn_Delete_Vaccination_Disable : Btn_Delete_Vaccination_Enable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVaccination_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_VACCINATION data = (MOS.EFMODEL.DataModels.V_HIS_VACCINATION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "REQUEST_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.REQUEST_TIME);
                        }
                        else if (e.Column.FieldName == "REQUEST_NAME")
                        {
                            e.Value = data.REQUEST_LOGINNAME + " - " + data.REQUEST_USERNAME;
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }

                        else if (e.Column.FieldName == "ICON_STT")
                        {
                            if (data.VACCINATION_STT_ID == IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__NEW)
                            {
                                e.Value = imageList1.Images[0];
                            }
                            else if (data.VACCINATION_STT_ID == IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__PROCESSING)
                            {
                                e.Value = imageList1.Images[1];
                            }
                            else if (data.VACCINATION_STT_ID == IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH)
                            {
                                e.Value = imageList1.Images[2];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVaccination_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {

        }

        private void gridControlVaccination_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var row = (V_HIS_VACCINATION)gridViewVaccination.GetFocusedRow();
                if (row != null)
                {
                    this.currentVaccination = row;

                    CommonParam param = new CommonParam();
                    HisExpMestFilter filter = new HisExpMestFilter();
                    filter.VACCINATION_ID = this.currentVaccination.ID;

                    var rsVaccination = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, param);

                    if (rsVaccination != null && rsVaccination.Count > 0)
                    {
                        this.currentExpMest = rsVaccination.FirstOrDefault();
                    }
                    else
                    {
                        this.currentExpMest = null;
                    }

                    GenerateMenuKhac();
                    SetButtonByExpMest(this.currentExpMest);
                    if (row.EXECUTE_TIME != null)
                    {
                        dtThoiGianTiem.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.EXECUTE_TIME ?? 0) ?? new DateTime();
                    }
                    else
                    {
                        dtThoiGianTiem.DateTime = DateTime.Now;
                    }

                    if (!string.IsNullOrEmpty(row.EXECUTE_LOGINNAME))
                    {
                        cboNguoiTiem.EditValue = row.EXECUTE_LOGINNAME;
                    }
                    else
                    {
                        cboNguoiTiem.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    }

                    btnSave.Enabled = (row.VACCINATION_STT_ID != IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH);

                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider2, dxErrorProvider1);

                    chkNo.Checked = (row.IS_REACT_RESPONSE != 1);
                    chkYes.Checked = (row.IS_REACT_RESPONSE == 1);

                    btnCancelReact.Enabled = (row.FOLLOW_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()) && row.VACCINATION_STT_ID == IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH;

                    HisVaccinationVrtyFilter hisVaccinationVrtyFilter = new HisVaccinationVrtyFilter();
                    hisVaccinationVrtyFilter.IS_ACTIVE = 1;
                    hisVaccinationVrtyFilter.VACCINATION_ID = row.ID;

                    var vrty = new BackendAdapter(param).Get<List<HIS_VACCINATION_VRTY>>("api/HisVaccinationVrty/Get", ApiConsumers.MosConsumer, hisVaccinationVrtyFilter, param);
                    if (vrty != null && vrty.Count > 0)
                    {
                        ResetCheckComboVaccReactType(cboKetQua, listVaccReactType.Where(o => vrty.Select(p => p.VACC_REACT_TYPE_ID).Contains(o.ID)).ToList());
                    }
                    else
                        ResetCheckComboVaccReactType(cboKetQua, null);

                    cboKetQua.Enabled = false;
                    cboKetQua.Enabled = true;

                    LoadDataToGridExpMestMedicine(row.ID);
                    LoadDataToGridHistoryVaccination(row.PATIENT_ID);

                    txtPhanUng.Text = row.VACCINATION_REACT_CODE;
                    cboPhanUng.EditValue = row.VACCINATION_REACT_ID;
                    if (row.REACT_TIME != null)
                    {
                        dtReactTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.REACT_TIME ?? 0) ?? new DateTime();
                    }
                    else
                    {
                        dtReactTime.EditValue = null;
                    }

                    txtTienSuBenhTat.Text = row.PATHOLOGICAL_HISTORY;
                    FillDataReactToControl(row);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_Delete_Vaccination_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (DevExpress.XtraEditors.XtraMessageBox.Show(HIS.Desktop.Plugins.Library.ResourceMessage.GetResource.Get(HIS.Desktop.Plugins.Library.ResourceMessage.KeyMessage.BanCoMuonXoaDuLieu), HIS.Desktop.Plugins.Library.ResourceMessage.GetResource.Get(HIS.Desktop.Plugins.Library.ResourceMessage.KeyMessage.ThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var row = (V_HIS_VACCINATION)gridViewVaccination.GetFocusedRow();
                    if (row != null)
                    {
                        CommonParam param = new CommonParam();

                        HisVaccinationSDO sdo = new HisVaccinationSDO();
                        sdo.Id = row.ID;
                        sdo.RequestRoomId = this.currentModule.RoomId;

                        var rsApi = new BackendAdapter(param).Post<bool>("api/HisVaccination/Delete", ApiConsumers.MosConsumer, sdo, param);
                        if (rsApi)
                        {
                            DefaultGrid();
                            LoadDataToGridVaccination();
                            DefaultButton();
                            DefaultValue();
                        }
                        MessageManager.Show(this.ParentForm, param, rsApi);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Grid Exp Mest Medicine

        private void gridViewExpMestMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_MEDICINE)gridViewExpMestMedicine.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.Column.FieldName == "VACCINATION_RESULT_ID")
                    {
                        e.RepositoryItem = cboKetQuaTiem_Enable;
                    }
                    else if (e.Column.FieldName == "CHANGE_MEDICINE")
                    {
                        if (this.currentVaccination != null && this.currentExpMest != null)
                        {
                            if (this.currentVaccination.VACCINATION_STT_ID == IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__FINISH || this.currentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || this.currentExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                            {
                                e.RepositoryItem = Res_DoiLo_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = Res_DoiLo_Enable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        //if (e.Column.FieldName == "STT")
                        //{
                        //    e.Value = e.ListSourceRowIndex + 1;
                        //}
                        //else if (e.Column.FieldName == "REQUEST_TIME_STR")
                        //{
                        //    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.REQUEST_TIME);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestMedicine_ShownEditor(object sender, EventArgs e)
        {
            //try
            //{
            //    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //    V_HIS_VACCINATION_METY data = view.GetFocusedRow() as V_HIS_VACCINATION_METY;
            //    if (view.FocusedColumn.FieldName == "MEDICINE_ID" && view.ActiveEditor is GridLookUpEdit)
            //    {
            //        GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
            //        LoadComboLoThuoc(data.MEDICINE_TYPE_ID, editor);
            //        editor.EditValue = data != null ? data.PATIENT_TYPE_ID : 0;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void gridViewExpMestMedicine_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            //try
            //{
            //    GridView view = sender as GridView;
            //    if (view.FocusedColumn.FieldName == "VACCINATION_RESULT_ID")
            //    {
            //        if (e.Value == null)
            //        {
            //            e.Valid = false;
            //            e.ErrorText = "Trường dữ liệu bắt buộc nhập";
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void gridViewExpMestMedicine_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                //Loại xử lý khi xảy ra exception Hiển thị. k cho nhập
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.DisplayError;
                //Show thông báo lỗi ở cột
                gridViewExpMestMedicine.SetColumnError(gridViewExpMestMedicine.Columns.ColumnByFieldName("VACCINATION_RESULT_ID"), e.ErrorText, ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestMedicine_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                //Loại xử lý khi xảy ra exception Hiển thị. k cho nhập
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.DisplayError;
                //Show thông báo lỗi ở cột
                gridViewExpMestMedicine.SetColumnError(gridViewExpMestMedicine.Columns.ColumnByFieldName("VACCINATION_RESULT_ID"), e.ErrorText, ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestMedicine_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_MEDICINE)e.Row;
                if (row != null)
                {
                    if (row.VACCINATION_RESULT_ID == null)
                    {
                        e.Valid = false;
                        e.ErrorText = e.ErrorText = "Trường dữ liệu bắt buộc nhập";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Res_DoiLo_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_MEDICINE)gridViewExpMestMedicine.GetFocusedRow();
                if (row != null)
                {
                    frmChangeMedicine frm = new frmChangeMedicine(row, delegateMedicine, this.currentModule.RoomId, this.currentMediStock);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void delegateMedicine(object data)
        {
            try
            {
                var dataSource = (List<V_HIS_EXP_MEST_MEDICINE>)gridControlExpMestMedicine.DataSource;
                if (dataSource != null && dataSource.Count > 0)
                {
                    var exp = (V_HIS_EXP_MEST_MEDICINE)data;
                    dataSource.FirstOrDefault(o => o.ID == exp.ID).MEDICINE_ID = exp.MEDICINE_ID;
                    gridControlExpMestMedicine.BeginUpdate();
                    gridControlExpMestMedicine.DataSource = dataSource;
                    gridControlExpMestMedicine.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Grid History Vaccination

        private void gridViewHistoryVaccination_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

        }

        private void gridViewHistoryVaccination_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_VACCINATION data = (MOS.EFMODEL.DataModels.V_HIS_VACCINATION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "EXECUTE_NAME")
                        {
                            e.Value = data.EXECUTE_LOGINNAME + " - " + data.EXECUTE_USERNAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(dxValidationProvider1, cboNguoiTiem);
                ValidationSingleControl(dxValidationProvider1, dtThoiGianTiem);
                ValidateGridLookupWithTextEdit(dxValidationProvider2, cboPhanUng, txtPhanUng);
                ValidationSingleControlMaxLength(dxValidationProvider2, txtTienSuBenhTat, 3000, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(DXValidationProvider dx, LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dx.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(DXValidationProvider dx, GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dx.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(DXValidationProvider dx, BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dx.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControlMaxLength(DXValidationProvider dx, BaseEdit control, int maxLength, bool isRequied)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validRule.editor = control;
                validRule.maxLength = maxLength;
                validRule.IsRequired = isRequied;
                validRule.ErrorType = ErrorType.Warning;
                dx.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

    }
}
