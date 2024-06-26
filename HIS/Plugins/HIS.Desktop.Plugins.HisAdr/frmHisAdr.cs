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
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAdr
{
    public partial class frmHisAdr : HIS.Desktop.Utility.FormBase
    {
        long _TreatmentId = 0;
        Inventec.Desktop.Common.Modules.Module currentModule;

        bool isShowContainerMediMaty = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;
        int theRequiredWidth = 900, theRequiredHeight = 220;
        internal MedicineTypeADO currentMedicineTypeADOForEdit;

        List<V_HIS_ADR_MEDICINE_TYPE> _AdrMedicineTypeNNs { get; set; }
        List<V_HIS_ADR_MEDICINE_TYPE> _AdrMedicineTypes { get; set; }

        V_HIS_ADR _ADROld { get; set; }

        int positionHandleControl = -1;
        int action = 0;

        HIS_TREATMENT _TreatmentPrint { get; set; }

        RefeshReference _RefeshReference { get; set; }

        public frmHisAdr()
        {
            InitializeComponent();
        }

        public frmHisAdr(Inventec.Desktop.Common.Modules.Module currentModule, long _treatmentId, RefeshReference _refeshReference)
            : base(currentModule)
        {
            InitializeComponent();
            this.SetIcon();
            this.currentModule = currentModule;
            this._TreatmentId = _treatmentId;
            this.action = 1;
            if (this.currentModule != null)
            {
                this.Text = currentModule.text;
            }
            this._RefeshReference = _refeshReference;
        }

        public frmHisAdr(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_ADR _aDROld, RefeshReference _refeshReference)
            : base(currentModule)
        {
            InitializeComponent();
            this.SetIcon();
            this.currentModule = currentModule;
            this._ADROld = _aDROld;
            this._TreatmentId = _aDROld.TREATMENT_ID;
            if (this.currentModule != null)
            {
                this.Text = currentModule.text;
            }
            this._RefeshReference = _refeshReference;
            this.GetDataAdrOld();
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

        private void frmHisAdr_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.LoadDataDefault();
                this.LoadExpMestMedicines();
                this._AdrMedicineTypes = new List<V_HIS_ADR_MEDICINE_TYPE>();
                this._AdrMedicineTypeNNs = new List<V_HIS_ADR_MEDICINE_TYPE>();
                this.RefeshControl();
                if (this.action == 1)
                {
                    this.spinWeight.EditValue = null;
                    this.dtNgayXuatHien.DateTime = DateTime.Now;
                }
                this.ValidationSingleControl(dtNgayXuatHien);
                this.GetDataAdrMedicineTypes();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataAdrOld()
        {
            try
            {

                this.action = 2;
                this.btnPrint.Enabled = true;
                if (this._ADROld.WEIGHT.HasValue)
                    spinWeight.EditValue = this._ADROld.WEIGHT;
                else
                    spinWeight.EditValue = null;

                dtNgayXuatHien.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this._ADROld.ADR_TIME) ?? DateTime.Now;
                txtPhanUngXuatHien.Text = this._ADROld.APPEAR_AFTER_TIME;
                txtXetNghiemLienQuan.Text = this._ADROld.RELATED_TEST;
                txtTienSu.Text = this._ADROld.PATHOLOGICAL_HISTORY;
                txtCachXuTriPhanUng.Text = this._ADROld.REACTION_METHOD;
                txtMotaBieuHien.Text = this._ADROld.DESCRIPTION;

                if (this._ADROld.SERIOUS_LEVEL.HasValue)
                {
                    if (this._ADROld.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__DiTatThai)
                        chkDiTat11.Checked = true;
                    else if (this._ADROld.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__KhongNghiemTrong)
                        chkKhongNghiemTrong11.Checked = true;
                    else if (this._ADROld.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__NGuyCoTuVong)
                        chkDeDoaTinhMang11.Checked = true;
                    else if (this._ADROld.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__NhapVienKeoDai)
                        chkNhapVien11.Checked = true;
                    else if (this._ADROld.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__TanTatVinhVien)
                        chkTanTat11.Checked = true;
                    else if (this._ADROld.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__TuVong)
                        chkTuVong11.Checked = true;
                }
                if (this._ADROld.ADR_RESULT_ID.HasValue)
                {
                    if (this._ADROld.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__ChuaHoiPhuc)
                        chkChuaHoiPhuc12.Checked = true;
                    else if (this._ADROld.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__DangHoiPhuc)
                        chkDangHoiPhuc12.Checked = true;
                    else if (this._ADROld.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__HoiPhucCoDiChung)
                        chkHoiPhucCoDiChung12.Checked = true;
                    else if (this._ADROld.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__HoiPhucKhongCoDiChung)
                        chkHoiPhucKhongDiChung12.Checked = true;
                    else if (this._ADROld.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__KhongRo)
                        chkKhongRo12.Checked = true;
                    else if (this._ADROld.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__TuVongDoADR)
                        chkTuVongDoADR12.Checked = true;
                    else if (this._ADROld.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__TuVongKhongDoADR)
                        chkTuVongKhongLienQuanDT12.Checked = true;
                }
                if (this._ADROld.RELATIONSHIP_ID.HasValue)
                {
                    if (this._ADROld.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__CoKhaNang)
                        chkCoKhaNang17.Checked = true;
                    else if (this._ADROld.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__CoThe)
                        chkCoThe17.Checked = true;
                    else if (this._ADROld.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__ChacChan)
                        chkChacChan17.Checked = true;
                    else if (this._ADROld.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__ChuaPhan)
                        chkChuaPhanLoai17.Checked = true;
                    else if (this._ADROld.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__Khac)
                    {
                        chkKhac17.Checked = true;
                        txtKhac17.Text = this._ADROld.REACTION_METHOD;
                    }
                    else if (this._ADROld.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__KhongChacChan)
                        chkKhongChacChan17.Checked = true;
                    else if (this._ADROld.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__KhongThePhanLoai)
                        chkKhongThePhanLoai17.Checked = true;
                }
                if (this._ADROld.EXPERTISE_STANDER_ID.HasValue)
                {
                    if (this._ADROld.EXPERTISE_STANDER_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.EXPERTISE_STANDER_ID__NARANJO)
                        chkThangNaranjo.Checked = true;
                    else if (this._ADROld.EXPERTISE_STANDER_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.EXPERTISE_STANDER_ID__WHO)
                        chkThangWHO.Checked = true;
                    else if (this._ADROld.EXPERTISE_STANDER_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.EXPERTISE_STANDER_ID__OTHER)
                    {
                        chkThangKhac.Checked = true;
                        txtThangKhac18.Text = this._ADROld.EXPERTISE_STANDER_OTHER;
                    }
                }
                this.txtBinhLuanCuaBoYTe.Text = this._ADROld.BYT_COMMENT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataAdrMedicineTypes()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisAdrMedicineTypeViewFilter filter = new MOS.Filter.HisAdrMedicineTypeViewFilter();
                if (this._ADROld != null && this._ADROld.ID > 0)
                    filter.ADR_ID = this._ADROld.ID;
                else
                    return;
                var dataAdrMedicineTypes = new BackendAdapter
                    (param).Get<List<V_HIS_ADR_MEDICINE_TYPE>>
                    ("api/HisAdrMedicineType/GetView", ApiConsumers.MosConsumer, filter, param);
                if (dataAdrMedicineTypes != null && dataAdrMedicineTypes.Count > 0)
                {
                    this._AdrMedicineTypeNNs.AddRange(dataAdrMedicineTypes.Where(p => p.IS_ADR.HasValue).ToList());
                    if (this._AdrMedicineTypeNNs != null && this._AdrMedicineTypeNNs.Count > 0)
                    {
                        this._AdrMedicineTypeNNs = this._AdrMedicineTypeNNs.OrderBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        gridControlNghiNgo.DataSource = null;
                        gridControlNghiNgo.DataSource = this._AdrMedicineTypeNNs;
                    }
                    this._AdrMedicineTypes.AddRange(dataAdrMedicineTypes.Where(p => p.IS_ADR == null || p.IS_ADR <= 0).ToList());
                    if (this._AdrMedicineTypes != null && this._AdrMedicineTypes.Count > 0)
                    {
                        this._AdrMedicineTypes = this._AdrMedicineTypes.OrderBy(p => p.MEDICINE_TYPE_NAME).ToList();
                        gridControlKhongNghiNgo.DataSource = null;
                        gridControlKhongNghiNgo.DataSource = this._AdrMedicineTypes;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDefault()
        {
            try
            {
                CommonParam param = new CommonParam();
                this._TreatmentPrint = new HIS_TREATMENT();

                MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                treatmentFilter.ID = this._TreatmentId;

                var dataTreaments = new BackendAdapter
                    (param).Get<List<HIS_TREATMENT>>
                    (HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param);
                if (dataTreaments != null && dataTreaments.Count > 0)
                {
                    this._TreatmentPrint = dataTreaments[0];

                    this.lblPatientName.Text = dataTreaments[0].TDL_PATIENT_NAME;
                    this.lblGenderName.Text = dataTreaments[0].TDL_PATIENT_GENDER_NAME;

                    if (dataTreaments[0].TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        this.lblDob.Text = dataTreaments[0].TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    else
                        this.lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataTreaments[0].TDL_PATIENT_DOB);
                }
                if (this.action == 1)
                {
                    MOS.Filter.HisDhstFilter dhstFilter = new MOS.Filter.HisDhstFilter();
                    dhstFilter.TREATMENT_ID = this._TreatmentId;
                    dhstFilter.ORDER_DIRECTION = "DESC";
                    dhstFilter.ORDER_FIELD = "MODIFY_TIME";
                    var dataDHSTs = new BackendAdapter
                        (param).Get<List<HIS_DHST>>
                        (HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);
                    if (dataDHSTs != null && dataDHSTs.Count > 0)
                    {
                        this.spinWeight.EditValue = dataDHSTs[0].WEIGHT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMedicines()
        {
            try
            {
                CommonParam param = new CommonParam();

                MOS.Filter.HisExpMestMedicineFilter _medicineFilter = new MOS.Filter.HisExpMestMedicineFilter();
                _medicineFilter.TDL_TREATMENT_ID = this._TreatmentId;
                var dataExpMestMedicines = new BackendAdapter
               (param).Get<List<HIS_EXP_MEST_MEDICINE>>
               (HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GET, ApiConsumers.MosConsumer, _medicineFilter, param);
                if (dataExpMestMedicines != null && dataExpMestMedicines.Count > 0)
                {
                    List<long> _medicineTypeIds = dataExpMestMedicines.Select(p => p.TDL_MEDICINE_TYPE_ID ?? 0).Distinct().ToList();
                    List<long> _medicineIds = dataExpMestMedicines.Select(p => p.MEDICINE_ID ?? 0).Distinct().ToList();
                    var dataGroups = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(p => _medicineTypeIds.Contains(p.ID)).OrderBy(p => p.MEDICINE_TYPE_NAME).ToList();


                    MOS.Filter.HisMedicineFilter _filter = new MOS.Filter.HisMedicineFilter();
                    _filter.IDs = _medicineIds;
                    var dataMedicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, _filter, param);
                    List<MedicineTypeADO> _MedicineTypeADOs = new List<MedicineTypeADO>();

                    _MedicineTypeADOs.AddRange((from r in dataGroups select new MedicineTypeADO(r, dataMedicines)).ToList());

                    RebuildMediMatyWithInControlContainer(_MedicineTypeADOs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToCboMedicine(List<V_HIS_EXP_MEST_MEDICINE> datas)
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "Mã loại thuốc", 50, 1));
                //columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "Tên loại thuốc", 200, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "MEDICINE_TYPE_ID", columnInfos, true, 250);
                //ControlEditorLoader.Load(this.cboMedicineType, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region ----- Event Grid MEDICINE -----
        private void txtMedicine_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainerMediMaty = !isShowContainerMediMaty;
                    if (isShowContainerMediMaty)
                    {
                        Rectangle buttonBounds = new Rectangle(txtMedicine.Bounds.X, txtMedicine.Bounds.Y, txtMedicine.Bounds.Width, txtMedicine.Bounds.Height);
                        popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 418));

                        if (this.currentMedicineTypeADOForEdit != null)
                        {
                            int rowIndex = 0;
                            var listDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<V_HIS_MEDICINE_TYPE>>(Newtonsoft.Json.JsonConvert.SerializeObject(this.gridControlMediMaty.DataSource));
                            for (int i = 0; i < listDatas.Count; i++)
                            {
                                if (listDatas[i].ID == this.currentMedicineTypeADOForEdit.ID)
                                {
                                    rowIndex = i;
                                    break;
                                }
                            }
                            gridViewMediMaty.FocusedRowHandle = rowIndex;
                        }
                    }
                    else
                    {
                        //popupControlContainerMediMaty.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMedicine_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (gridViewMediMaty.RowCount == 1)
                    //{
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();

                        MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                    }
                    //}
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewMediMaty.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtMedicine.Bounds.X, txtMedicine.Bounds.Y, txtMedicine.Bounds.Width, txtMedicine.Bounds.Height);
                    popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 418));
                    gridViewMediMaty.Focus();
                    gridViewMediMaty.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtMedicine.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MetyMatyTypeInStock_RowClick(object data)
        {
            try
            {
                this.currentMedicineTypeADOForEdit = new MedicineTypeADO();
                this.currentMedicineTypeADOForEdit = data as MedicineTypeADO;
                if (this.currentMedicineTypeADOForEdit != null)
                {
                    this.txtMedicine.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    this.lblDuongDung.Text = this.currentMedicineTypeADOForEdit.MEDICINE_USE_FORM_NAME;
                    this.txtSoLo.Text = this.currentMedicineTypeADOForEdit.PACKAGE_NUMBER;

                    this.btnAdd.Enabled = true;
                    this.chkNghiNgo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMedicine_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtMedicine.Text))
                {
                    txtMedicine.Refresh();
                    if (isShowContainerMediMatyForChoose)
                    {
                        gridViewMediMaty.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        gridViewMediMaty.ActiveFilterString = "[MEDICINE_TYPE_NAME] Like '%" + txtMedicine.Text
                            + "%' OR [MEDICINE_TYPE_CODE] Like '%" + txtMedicine.Text + "%'"
                        + " OR [ACTIVE_INGR_BHYT_NAME] Like '%" + txtMedicine.Text + "%'";
                        //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                        //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                        gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewMediMaty.FocusedRowHandle = 0;
                        gridViewMediMaty.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewMediMaty.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtMedicine.Bounds.X, txtMedicine.Bounds.Y, txtMedicine.Bounds.Width, txtMedicine.Bounds.Height);
                        if (isShow)
                        {
                            popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 418));
                            isShow = false;
                        }

                        txtMedicine.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    gridViewMediMaty.ActiveFilter.Clear();
                    // this.currentMedicineTypeADOForEdit = null;
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerMediMaty.HidePopup();
                    }
                }
                // this.ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RebuildMediMatyWithInControlContainer(List<MedicineTypeADO> datas)
        {
            try
            {
                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();
                popupControlContainerMediMaty.Width = theRequiredWidth;
                popupControlContainerMediMaty.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_CODE";
                col2.Caption = "Mã";
                col2.Width = 60;
                col2.VisibleIndex = 1;
                gridViewMediMaty.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_NAME";
                col1.Caption = "Tên";
                col1.Width = 250;
                col1.VisibleIndex = 2;
                gridViewMediMaty.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "SERVICE_UNIT_NAME";
                col7.Caption = "Đơn vị tính";
                col7.Width = 60;
                col7.VisibleIndex = 3;
                gridViewMediMaty.Columns.Add(col7);

                //DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                //col3.FieldName = "AMOUNT";
                //col3.Caption = "Số lượng";
                //col3.Width = 70;
                //col3.VisibleIndex = 4;
                //col3.DisplayFormat.FormatString = "#,##0.00";
                //col3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                //gridViewMediMaty.Columns.Add(col3);



                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "CONCENTRA";
                col5.Caption = "Hàm lượng";
                col5.Width = 100;
                col5.VisibleIndex = 4;
                gridViewMediMaty.Columns.Add(col5);



                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "MANUFACTURER_NAME";
                col8.Caption = "Nhà SX";
                col8.Width = 150;
                col8.VisibleIndex = 5;
                gridViewMediMaty.Columns.Add(col8);

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "NATIONAL_NAME";
                col9.Caption = "Quốc gia";
                col9.Width = 80;
                col9.VisibleIndex = 6;
                gridViewMediMaty.Columns.Add(col9);

                gridViewMediMaty.GridControl.DataSource = datas;
                gridViewMediMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerMediMaty_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view.OptionsFind.HighlightFindResults && !txtMedicine.Text.Equals(string.Empty))
                {
                    CriteriaOperator op = ConvertFindPanelTextToCriteriaOperator(txtMedicine.Text, view, false);
                    if (op is GroupOperator)
                    {
                        string findText = txtMedicine.Text;
                        if (HiglightSubString(e, findText))
                            e.Handled = true;
                    }
                    else if (op is FunctionOperator)
                    {
                        FunctionOperator func = op as FunctionOperator;
                        CriteriaOperator colNameOperator = func.Operands[0];
                        string colName = colNameOperator.LegacyToString().Replace("[", string.Empty).Replace("]", string.Empty);
                        if (!e.Column.FieldName.StartsWith(colName)) return;

                        CriteriaOperator valueOperator = func.Operands[1];
                        string findText = valueOperator.LegacyToString().ToLower().Replace("'", "");
                        if (HiglightSubString(e, findText))
                            e.Handled = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static CriteriaOperator ConvertFindPanelTextToCriteriaOperator(string findPanelText, GridView view, bool applyPrefixes)
        {
            if (!string.IsNullOrEmpty(findPanelText))
            {
                FindSearchParserResults parseResult = new FindSearchParser().Parse(findPanelText, GetFindToColumnsCollection(view));
                if (applyPrefixes)
                    parseResult.AppendColumnFieldPrefixes();

                return DxFtsContainsHelperAlt.Create(parseResult, FilterCondition.Contains, false);
            }
            return null;
        }

        private static ICollection GetFindToColumnsCollection(GridView view)
        {
            System.Reflection.MethodInfo mi = typeof(ColumnView).GetMethod("GetFindToColumnsCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return mi.Invoke(view, null) as ICollection;
        }

        private bool HiglightSubString(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e, string findText)
        {
            int index = FindSubStringStartPosition(e.DisplayText, findText);
            if (index == -1)
            {
                return false;
            }

            e.Appearance.FillRectangle(e.Cache, e.Bounds);
            e.Cache.Paint.DrawMultiColorString(e.Cache, e.Bounds, e.DisplayText, GetStringWithoutQuotes(findText),
                e.Appearance, Color.Indigo, Color.Gold, true, index);
            return true;
        }

        private string GetStringWithoutQuotes(string findText)
        {
            string stringWithoutQuotes = findText.ToLower().Replace("\"", string.Empty);
            return stringWithoutQuotes;
        }

        private int FindSubStringStartPosition(string dispalyText, string findText)
        {
            string stringWithoutQuotes = GetStringWithoutQuotes(findText);
            int index = dispalyText.ToLower().IndexOf(stringWithoutQuotes);
            return index;
        }

        private void gridViewMediMaty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewMediMaty.Focus();
                    this.gridViewMediMaty.FocusedRowHandle = this.gridViewMediMaty.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                //                D_HIS_MEDI_STOCK_1 dMediStock = gridViewMediMaty.GetRow(e.RowHandle) as D_HIS_MEDI_STOCK_1;
                //                if (dMediStock != null && (dMediStock.IS_STAR_MARK ?? 0) == 1)
                //                {
                //                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                //                }

                //                MedicineMaterialTypeComboADO medicineMaterialTypeComboADO = gridViewMediMaty.GetRow
                //(e.RowHandle) as MedicineMaterialTypeComboADO;
                //                if (medicineMaterialTypeComboADO != null && (medicineMaterialTypeComboADO.IS_STAR_MARK ?? 0) == 1)
                //                {
                //                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                //                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediMaty_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                if (medicineTypeADOForEdit != null)
                {
                    popupControlContainerMediMaty.HidePopup();
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;

                    MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void chkNghiNgo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.SetEnableControlByChkNghiNgo(chkNghiNgo.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableControlByChkNghiNgo(bool key)
        {
            try
            {
                txtSoLo.Enabled = key;
                spinLieuDung.Enabled = key;
                spinSoLanDung.Enabled = key;
                dtBatDau.Enabled = key;
                dtKetThuc.Enabled = key;
                txtLyDoDung.Enabled = key;
                chkCo14.Enabled = key;
                chkKhong14.Enabled = key;
                chkKhongNgungGiamLieu14.Enabled = key;
                chkKhongCothongTin14.Enabled = key;
                chkCo15.Enabled = key;
                chkKhong15.Enabled = key;
                chkKhongCoThongTin15.Enabled = key;
                chkKhongTaiSuDung15.Enabled = key;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAdd.Enabled)
                    return;
                if (this.currentMedicineTypeADOForEdit != null)
                {
                    if (chkNghiNgo.Checked)
                    {
                        var dataCheck = this._AdrMedicineTypeNNs.FirstOrDefault(p => p.MEDICINE_TYPE_ID == this.currentMedicineTypeADOForEdit.ID);
                        if (dataCheck != null)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(
                    ResourceMessage.ThuocDaCoTrongDanhSach_BanCoMuonThayThe, dataCheck.MEDICINE_TYPE_NAME),
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                return;
                            }
                            else
                                this._AdrMedicineTypeNNs.Remove(dataCheck);
                        }

                        V_HIS_ADR_MEDICINE_TYPE type = new V_HIS_ADR_MEDICINE_TYPE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_ADR_MEDICINE_TYPE>(type, this.currentMedicineTypeADOForEdit);
                        type.MEDICINE_TYPE_ID = this.currentMedicineTypeADOForEdit.ID;
                        type.IS_ADR = 1;
                        type.PACKAGE_NUMBER = txtSoLo.Text.Trim();

                        if (dtBatDau.EditValue != null)
                        {
                            type.START_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBatDau.DateTime) ?? 0;
                        }
                        if (dtKetThuc.EditValue != null)
                        {
                            type.FINISH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtKetThuc.DateTime) ?? 0;
                        }
                        if (dtBatDau.EditValue != null && dtKetThuc.EditValue != null)
                        {
                            if (type.START_TIME.HasValue && type.FINISH_TIME.HasValue && type.START_TIME > type.FINISH_TIME)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThoiGianBatDauKhongDuocThoiGianKetThuc,
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao));
                                dtBatDau.Focus();
                                return;
                            }
                        }

                        if (spinLieuDung.EditValue != null)
                        {
                            type.ONCE_TUTORIAL = spinLieuDung.Value;
                        }
                        if (spinSoLanDung.EditValue != null)
                        {
                            type.NUMBER_USE = spinSoLanDung.Value.ToString();
                        }
                        if (chkCo14.Checked)
                            type.IMPROVE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ADR_MEDICINE_TYPE.IMPROVE_TYPE_ID__Co;
                        else if (chkKhong14.Checked)
                            type.IMPROVE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ADR_MEDICINE_TYPE.IMPROVE_TYPE_ID__Khong;
                        else if (chkKhongNgungGiamLieu14.Checked)
                            type.IMPROVE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ADR_MEDICINE_TYPE.IMPROVE_TYPE_ID__KhongGiamLieu;
                        else if (chkKhongCothongTin14.Checked)
                            type.IMPROVE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ADR_MEDICINE_TYPE.IMPROVE_TYPE_ID__KhongCoTT;

                        if (chkCo15.Checked)
                            type.REAPPEAR_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ADR_MEDICINE_TYPE.REAPPEAR_TYPE_ID__Co;
                        else if (chkKhong15.Checked)
                            type.REAPPEAR_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ADR_MEDICINE_TYPE.REAPPEAR_TYPE_ID__Khong;
                        else if (chkKhongTaiSuDung15.Checked)
                            type.REAPPEAR_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ADR_MEDICINE_TYPE.REAPPEAR_TYPE_ID__KhongTaiSD;
                        else if (chkKhongCoThongTin15.Checked)
                            type.REAPPEAR_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ADR_MEDICINE_TYPE.REAPPEAR_TYPE_ID__KhongCoTT;
                        type.REASON = this.txtLyDoDung.Text.Trim();
                        type.ID = 0;
                        this._AdrMedicineTypeNNs.Add(type);

                        gridControlNghiNgo.DataSource = null;
                        gridControlNghiNgo.DataSource = this._AdrMedicineTypeNNs;
                    }
                    else
                    {
                        var dataCheck = this._AdrMedicineTypes.FirstOrDefault(p => p.MEDICINE_TYPE_ID == this.currentMedicineTypeADOForEdit.ID);
                        if (dataCheck != null)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(
                    ResourceMessage.ThuocDaCoTrongDanhSach_BanCoMuonThayThe, dataCheck.MEDICINE_TYPE_NAME),
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                return;
                            }
                            else
                                this._AdrMedicineTypes.Remove(dataCheck);
                        }
                        V_HIS_ADR_MEDICINE_TYPE type = new V_HIS_ADR_MEDICINE_TYPE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_ADR_MEDICINE_TYPE>(type, this.currentMedicineTypeADOForEdit);
                        type.MEDICINE_TYPE_ID = this.currentMedicineTypeADOForEdit.ID;
                        type.ID = 0;
                        this._AdrMedicineTypes.Add(type);
                        gridControlKhongNghiNgo.DataSource = null;
                        gridControlKhongNghiNgo.DataSource = this._AdrMedicineTypes;
                    }
                    this.RefeshControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefeshControl()
        {
            try
            {
                this.currentMedicineTypeADOForEdit = null;
                this.txtMedicine.Text = "";
                chkNghiNgo.Checked = false;
                txtSoLo.Text = "";
                spinLieuDung.EditValue = null;
                spinSoLanDung.EditValue = null;
                lblDuongDung.Text = "";
                dtBatDau.EditValue = null;
                dtKetThuc.EditValue = null;
                txtLyDoDung.Text = "";
                chkCo14.Checked = false;
                chkCo15.Checked = false;
                chkKhong14.Checked = false;
                chkKhong15.Checked = false;
                chkKhongCothongTin14.Checked = false;
                chkKhongCoThongTin15.Checked = false;
                chkKhongNgungGiamLieu14.Checked = false;
                chkKhongTaiSuDung15.Checked = false;
                btnAdd.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewNghiNgo_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_ADR_MEDICINE_TYPE data = (V_HIS_ADR_MEDICINE_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "START_TIME_DISPLAY" && data.START_TIME.HasValue)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.START_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "FINISH_TIME_DISPLAY" && data.FINISH_TIME.HasValue)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FINISH_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_DELETE_NN_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_HIS_ADR_MEDICINE_TYPE)gridViewNghiNgo.GetFocusedRow();
                if (data != null)
                {
                    this._AdrMedicineTypeNNs.Remove(data);
                    gridControlNghiNgo.DataSource = null;
                    gridControlNghiNgo.DataSource = this._AdrMedicineTypeNNs;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_HIS_ADR_MEDICINE_TYPE)gridViewKhongNghiNgo.GetFocusedRow();
                if (data != null)
                {
                    this._AdrMedicineTypes.Remove(data);
                    gridControlKhongNghiNgo.DataSource = null;
                    gridControlKhongNghiNgo.DataSource = this._AdrMedicineTypes;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewKhongNghiNgo_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_ADR_MEDICINE_TYPE data = (V_HIS_ADR_MEDICINE_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
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
                if (chkKhac17.Checked)
                    ValidationSingleControl(txtKhac17);
                else
                    this.dxValidationProvider1.SetValidationRule(txtKhac17, null);
                if (chkThangKhac.Checked)
                    ValidationSingleControl(txtThangKhac18);
                else
                    this.dxValidationProvider1.SetValidationRule(txtThangKhac18, null);

                this.positionHandleControl = -1;
                if (!this.dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                #region ----- GET DATA -----
                HisAdrSDO sdo = new HisAdrSDO();

                sdo.AdrMedicineTypes = new List<HIS_ADR_MEDICINE_TYPE>();
                if (this._AdrMedicineTypeNNs != null && this._AdrMedicineTypeNNs.Count > 0)
                {
                    foreach (var item in this._AdrMedicineTypeNNs)
                    {
                        HIS_ADR_MEDICINE_TYPE ado = new HIS_ADR_MEDICINE_TYPE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ADR_MEDICINE_TYPE>(ado, item);
                        sdo.AdrMedicineTypes.Add(ado);
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BatBuocPhaiChonThuocPhanUngCoHai,
                   HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao));
                    txtMedicine.Focus();
                    return;
                }
                if (this._AdrMedicineTypes != null && this._AdrMedicineTypes.Count > 0)
                {
                    foreach (var item in this._AdrMedicineTypes)
                    {
                        HIS_ADR_MEDICINE_TYPE ado = new HIS_ADR_MEDICINE_TYPE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ADR_MEDICINE_TYPE>(ado, item);
                        sdo.AdrMedicineTypes.Add(ado);
                    }
                }

                HIS_ADR _adr = new HIS_ADR();
                _adr.REPORT_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                _adr.REPORT_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

                _adr.TREATMENT_ID = this._TreatmentId;
                if (spinWeight.EditValue != null)
                {
                    _adr.WEIGHT = spinWeight.Value;
                }
                if (dtNgayXuatHien.EditValue != null)
                {
                    _adr.ADR_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtNgayXuatHien.DateTime) ?? 0;
                }
                if (chkTuVongDoADR12.Checked)
                    _adr.ADR_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__TuVongDoADR;
                else if (chkTuVongKhongLienQuanDT12.Checked)
                    _adr.ADR_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__TuVongKhongDoADR;
                else if (chkChuaHoiPhuc12.Checked)
                    _adr.ADR_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__ChuaHoiPhuc;
                else if (chkDangHoiPhuc12.Checked)
                    _adr.ADR_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__DangHoiPhuc;
                else if (chkHoiPhucCoDiChung12.Checked)
                    _adr.ADR_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__HoiPhucCoDiChung;
                else if (chkHoiPhucKhongDiChung12.Checked)
                    _adr.ADR_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__HoiPhucKhongCoDiChung;
                else if (chkKhongRo12.Checked)
                    _adr.ADR_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__KhongRo;

                _adr.RELATED_TEST = txtXetNghiemLienQuan.Text.Trim();
                _adr.PATHOLOGICAL_HISTORY = txtTienSu.Text.Trim();
                _adr.REACTION_METHOD = txtCachXuTriPhanUng.Text.Trim();
                _adr.DESCRIPTION = txtMotaBieuHien.Text.Trim();
                _adr.APPEAR_AFTER_TIME = txtPhanUngXuatHien.Text.Trim();

                if (chkTuVong11.Checked)
                    _adr.SERIOUS_LEVEL = IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__TuVong;
                else if (chkDeDoaTinhMang11.Checked)
                    _adr.SERIOUS_LEVEL = IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__NGuyCoTuVong;
                else if (chkTanTat11.Checked)
                    _adr.SERIOUS_LEVEL = IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__TanTatVinhVien;
                else if (chkDiTat11.Checked)
                    _adr.SERIOUS_LEVEL = IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__DiTatThai;
                else if (chkKhongNghiemTrong11.Checked)
                    _adr.SERIOUS_LEVEL = IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__KhongNghiemTrong;
                else if (chkNhapVien11.Checked)
                    _adr.SERIOUS_LEVEL = IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__NhapVienKeoDai;

                if (chkChacChan17.Checked)
                    _adr.RELATIONSHIP_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__ChacChan;
                else if (chkCoKhaNang17.Checked)
                    _adr.RELATIONSHIP_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__CoKhaNang;
                else if (chkCoThe17.Checked)
                    _adr.RELATIONSHIP_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__CoThe;
                else if (chkKhongChacChan17.Checked)
                    _adr.RELATIONSHIP_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__KhongChacChan;
                else if (chkChuaPhanLoai17.Checked)
                    _adr.RELATIONSHIP_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__ChuaPhan;
                else if (chkKhongThePhanLoai17.Checked)
                    _adr.RELATIONSHIP_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__KhongThePhanLoai;
                else if (chkKhac17.Checked)
                {
                    _adr.RELATIONSHIP_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__Khac;
                    if (string.IsNullOrEmpty(txtKhac17.Text))
                    {
                        txtKhac17.Focus();
                        return;
                    }
                    _adr.RELATIONSHIP_ORTHER = txtKhac17.Text.Trim();
                }

                if (chkThangWHO.Checked)
                    _adr.EXPERTISE_STANDER_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.EXPERTISE_STANDER_ID__WHO;
                else if (chkThangNaranjo.Checked)
                    _adr.EXPERTISE_STANDER_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.EXPERTISE_STANDER_ID__NARANJO;
                else if (chkThangKhac.Checked)
                {
                    _adr.EXPERTISE_STANDER_ID = IMSys.DbConfig.HIS_RS.HIS_ADR.EXPERTISE_STANDER_ID__OTHER;
                    if (string.IsNullOrEmpty(txtThangKhac18.Text))
                    {
                        txtThangKhac18.Focus();
                        return;
                    }
                    _adr.EXPERTISE_STANDER_OTHER = txtThangKhac18.Text.Trim();
                }

                _adr.BYT_COMMENT = txtBinhLuanCuaBoYTe.Text.Trim();
                sdo.Adr = _adr;

                #endregion

                CommonParam param = new CommonParam();
                bool succes = false;
                HisAdrResultSDO result = null;
                if (this.action == 1)
                {
                    result = new BackendAdapter(param).Post<HisAdrResultSDO>("api/HisAdr/Create", ApiConsumers.MosConsumer, sdo, param);
                }
                else if (this.action == 2 && this._ADROld != null)
                {
                    sdo.Adr.ID = this._ADROld.ID;
                    result = new BackendAdapter(param).Post<HisAdrResultSDO>("api/HisAdr/Update", ApiConsumers.MosConsumer, sdo, param);
                }
                if (result != null)
                {
                    this.action = 2;
                    this._ADROld = result.HisAdr;
                    succes = true;
                    btnPrint.Enabled = true;
                    if (this._RefeshReference != null)
                    {
                        this._RefeshReference();
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, succes);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #region ---- Event ----
        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(DateEdit control)
        {
            try
            {
                DateTimeEdit__ValidationRule validRule = new DateTimeEdit__ValidationRule();
                validRule.dateEdit = control;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(TextEdit control)
        {
            try
            {
                TextEdit__ValidationRule validRule = new TextEdit__ValidationRule();
                validRule.textEdit = control;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinWeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtNgayXuatHien.Focus();
                    dtNgayXuatHien.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtNgayXuatHien_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtPhanUngXuatHien.Focus();
                    txtPhanUngXuatHien.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtNgayXuatHien_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPhanUngXuatHien.Focus();
                    txtPhanUngXuatHien.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPhanUngXuatHien_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMotaBieuHien.Focus();
                    txtMotaBieuHien.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMotaBieuHien_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Tab)
                //{
                //    txtXetNghiemLienQuan.Focus();
                //    txtXetNghiemLienQuan.SelectAll();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtXetNghiemLienQuan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTienSu.Focus();
                    txtTienSu.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTienSu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCachXuTriPhanUng.Focus();
                    txtCachXuTriPhanUng.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCachXuTriPhanUng_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkTuVong11.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTuVong11_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkDeDoaTinhMang11.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDeDoaTinhMang11_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkTanTat11.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTanTat11_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkDiTat11.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDiTat11_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhongNghiemTrong11.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongNghiemTrong11_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkNhapVien11.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNhapVien11_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkTuVongDoADR12.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTuVongDoADR12_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkTuVongKhongLienQuanDT12.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTuVongKhongLienQuanDT12_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkChuaHoiPhuc12.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkChuaHoiPhuc12_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkDangHoiPhuc12.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDangHoiPhuc12_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHoiPhucCoDiChung12.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHoiPhucCoDiChung12_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHoiPhucKhongDiChung12.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHoiPhucKhongDiChung12_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhongRo12.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongRo12_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicine.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNghiNgo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkNghiNgo.Checked)
                    {
                        txtSoLo.Focus();
                        txtSoLo.SelectAll();
                    }
                    else
                    {
                        btnAdd.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSoLo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinLieuDung.Focus();
                    spinLieuDung.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinLieuDung_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSoLanDung.Focus();
                    spinSoLanDung.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSoLanDung_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtBatDau.Focus();
                    dtBatDau.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtBatDau_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtKetThuc.Focus();
                    dtKetThuc.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtBatDau_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtKetThuc.Focus();
                    dtKetThuc.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtKetThuc_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtLyDoDung.Focus();
                    txtLyDoDung.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtKetThuc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLyDoDung.Focus();
                    txtLyDoDung.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLyDoDung_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCo14.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCo14_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhong14.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhong14_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhongNgungGiamLieu14.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongNgungGiamLieu14_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhongCothongTin14.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongCothongTin14_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCo15.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCo15_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhong15.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhong15_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhongTaiSuDung15.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongTaiSuDung15_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhongCoThongTin15.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongCoThongTin15_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkChacChan17_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCoKhaNang17.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCoKhaNang17_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCoThe17.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCoThe17_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhongChacChan17.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongChacChan17_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkChuaPhanLoai17.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkChuaPhanLoai17_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhongThePhanLoai17.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongThePhanLoai17_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhac17.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKhac17_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkThangWHO.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkThangWHO_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkThangNaranjo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkThangNaranjo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkThangKhac.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtThangKhac18_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBinhLuanCuaBoYTe.Focus();
                    txtBinhLuanCuaBoYTe.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkThangKhac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtThangKhac18.Focus();
                    txtThangKhac18.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBinhLuanCuaBoYTe_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhac17_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkKhac17.Checked)
                {
                    txtKhac17.Enabled = true;
                }
                else
                {
                    txtKhac17.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkThangKhac_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkThangKhac.Checked)
                {
                    txtThangKhac18.Enabled = true;
                }
                else
                {
                    txtThangKhac18.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void barButtonI__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled)
                    return;
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__ADD_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnAdd.Enabled)
                    return;
                btnAdd_Click(null, null);
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
                ProcessPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrint()
        {
            try
            {

                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate("Mps000248", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000248":
                        Mps000248(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void Mps000248(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_TreatmentPrint != null ? _TreatmentPrint.TREATMENT_CODE : ""), printTypeCode, currentModule.RoomId);
                MPS.Processor.Mps000248.PDO.Mps000248PDO mps000248PDO = new MPS.Processor.Mps000248.PDO.Mps000248PDO
                   (
                   this._TreatmentPrint,
                   this._ADROld,
                   this._AdrMedicineTypeNNs,
                   this._AdrMedicineTypes
                     );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000248PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000248PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
