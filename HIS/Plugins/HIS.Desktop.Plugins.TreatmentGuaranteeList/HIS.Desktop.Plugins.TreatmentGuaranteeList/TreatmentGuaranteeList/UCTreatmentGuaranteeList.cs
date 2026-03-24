using DevExpress.Office.Crypto.Agile;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee;
using HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee.ADO;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.TreatmentGuaranteeList.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.TreatmentGuaranteeList.TreatmentGuaranteeList
{
    public partial class UCTreatmentGuaranteeList : UserControlBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int limit = 0;
        int start = 0;
        PagingGrid pagingGrid;
        int positionHandle = -1;
        GridColumn lastColumn = null;
        V_HIS_TREATMENT_FEE_1 transFee;

        ToolTipControlInfo lastInfo = null;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<HIS_TREATMENT_TYPE> _DienDieuTriSelecteds;
        List<V_HIS_ROOM> _EndRoomSelecteds;
        List<HIS_DEPARTMENT> _EndDepartmentSelecteds;
        List<HIS_DEPARTMENT> DepartmentSelecteds;
        List<TrangThaiADO> _TrangThaiSelecteds;

        List<HIS_TREATMENT_TYPE> listTreatmentType;
        List<V_HIS_ROOM> listRoom;
        List<HIS_DEPARTMENT> listDepartment;
        List<TrangThaiADO> listTrangThai;
        List<V_HIS_KSK_CONTRACT> listKskContract;
        List<HIS_PATIENT_TYPE> patientTypeSelecteds;
        List<HIS_PATIENT_TYPE> listPatientType;

        internal string typeCodeFind__KeyWork_InDate = "Trong ngày";
        internal string typeCodeFind_InDate = "Trong ngày";
        internal string typeCodeFind__InMonth = "Trong tháng";
        internal string typeCodeFind__InYear = "Trong năm";
        internal string typeCodeFind__InTime = "Khoảng ngày";

        internal string typeCodeFind__KeyWork_OutDate = "Trong ngày";
        internal string typeCodeFind_OutDate = "Trong ngày";
        internal string typeCodeFind__OutMonth = "Trong tháng";
        internal string typeCodeFind__OutYear = "Trong năm";
        internal string typeCodeFind__OutTime = "Khoảng ngày";
        bool isLoadForm = false;
        public UCTreatmentGuaranteeList(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            pagingGrid = new PagingGrid();
        }

        private void UCTreatmentGuaranteeList_Load(object sender, EventArgs e)
        {
            isLoadForm = true;
            HisConfigCFG.LoadConfig();
            InitComboInHopital();
            InitComboOutHospital();
            GetDataCombo();
            InitCombo(cboDienDieuTri, listTreatmentType, "TREATMENT_TYPE_NAME", "ID");
            InitCheck(cboDienDieuTri, SelectionGrid__DienDieuTri);
            InitCombo(cboPhongKetThuc, listRoom, "ROOM_NAME", "ID");
            InitCheck(cboPhongKetThuc, SelectionGrid__PhongKetThuc);
            InitCombo(cboKhoaKetThuc, listDepartment, "DEPARTMENT_NAME", "ID");
            InitCheck(cboKhoaKetThuc, SelectionGrid__EndDepartment);
            InitCombo(cboTrangThai, listTrangThai, "TrangThai", "ID");
            InitCheck(cboTrangThai, SelectionGrid__TrangThai);
            InitCboKhoaVaoVien();
            SetDefaultValue();
            FillDataToControl();
            if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
            {
                this.Text = this.moduleData.text;
            }

            isLoadForm = false;
        }

        private void InitCboKhoaVaoVien()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = 1;

                DepartmentSelecteds = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "Mã khoa", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Tên khoa", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboKhoaVaoVien, DepartmentSelecteds, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__TrangThai(object sender, EventArgs e)
        {
            try
            {
                _TrangThaiSelecteds = new List<TrangThaiADO>();
                foreach (TrangThaiADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _TrangThaiSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__EndDepartment(object sender, EventArgs e)
        {
            try
            {
                _EndDepartmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _EndDepartmentSelecteds.Add(rv);
                }
                if (!isLoadForm)
                {
                    if (_EndDepartmentSelecteds != null && _EndDepartmentSelecteds.Count > 0)
                    {
                        cboPhongKetThuc.Properties.DataSource = listRoom.Where(o => _EndDepartmentSelecteds.Select(p => p.DEPARTMENT_CODE).ToList().Contains(o.DEPARTMENT_CODE)).ToList();

                        GridCheckMarksSelection gridCheckMark = cboPhongKetThuc.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.ClearSelection(cboPhongKetThuc.Properties.View);
                        }
                        _EndRoomSelecteds = new List<V_HIS_ROOM>();
                    }
                    else
                    {
                        cboPhongKetThuc.Properties.DataSource = listRoom;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__PhongKetThuc(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<V_HIS_ROOM> sgSelectedNews = new List<V_HIS_ROOM>();
                    foreach (V_HIS_ROOM rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.ROOM_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this._EndRoomSelecteds = new List<V_HIS_ROOM>();
                    this._EndRoomSelecteds.AddRange(sgSelectedNews);
                }
                this.cboPhongKetThuc.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__DienDieuTri(object sender, EventArgs e)
        {
            try
            {
                _DienDieuTriSelecteds = new List<HIS_TREATMENT_TYPE>();
                foreach (HIS_TREATMENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _DienDieuTriSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDataCombo()
        {
            try
            {
                listTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                listRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
                listTrangThai = new List<TrangThaiADO>();
                listTrangThai.Add(new TrangThaiADO(1, "Đang điều trị"));
                listTrangThai.Add(new TrangThaiADO(2, "Đã kết thúc điều trị"));
                listTrangThai.Add(new TrangThaiADO(3, "Đã duyệt khóa tài chính"));
                listTrangThai.Add(new TrangThaiADO(4, "Đã duyệt khóa bảo hiểm"));
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
        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboOutHospital()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemOutDateCode = new DXMenuItem(typeCodeFind__KeyWork_OutDate, new EventHandler(cboInOfHospital_Click));
                itemOutDateCode.Tag = "OutDate";
                menu.Items.Add(itemOutDateCode);

                DXMenuItem itemOutMonth = new DXMenuItem(typeCodeFind__OutMonth, new EventHandler(cboInOfHospital_Click));
                itemOutMonth.Tag = "OutMonth";
                menu.Items.Add(itemOutMonth);
                DXMenuItem itemOutYear = new DXMenuItem(typeCodeFind__OutYear, new EventHandler(cboInOfHospital_Click));
                itemOutYear.Tag = "OutYear";
                menu.Items.Add(itemOutYear);

                DXMenuItem itemOutTime = new DXMenuItem(typeCodeFind__OutTime, new EventHandler(cboInOfHospital_Click));
                itemOutTime.Tag = "OutTime";
                menu.Items.Add(itemOutTime);

                cboRaVien.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInOfHospital_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                cboRaVien.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_OutDate = btnMenuCodeFind.Caption;

                FormatDtIntructionInDate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //Cái này ra viện
        private void FormatDtIntructionInDate()
        {
            try
            {
                layoutControlItem23.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem24.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                {
                    dteRaVienTu.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dteRaVienTu.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteRaVienTu.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dteRaVienTu.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteRaVienTu.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dteRaVienTu.Properties.EditMask = "dd/MM/yyyy";
                    dteRaVienTu.Properties.Mask.EditMask = "dd/MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                {
                    dteRaVienTu.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dteRaVienTu.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteRaVienTu.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dteRaVienTu.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteRaVienTu.Properties.EditFormat.FormatString = "MM/yyyy";
                    dteRaVienTu.Properties.EditMask = "MM/yyyy";
                    dteRaVienTu.Properties.Mask.EditMask = "MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__InYear)
                {
                    dteRaVienTu.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearsGroupView;
                    dteRaVienTu.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteRaVienTu.Properties.DisplayFormat.FormatString = "yyyy";
                    dteRaVienTu.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteRaVienTu.Properties.EditFormat.FormatString = "yyyy";
                    dteRaVienTu.Properties.EditMask = "yyyy";
                    dteRaVienTu.Properties.Mask.EditMask = "yyyy";
                }
                else
                {
                    layoutControlItem23.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem24.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dteRaVienTu.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dteRaVienTu.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteRaVienTu.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dteRaVienTu.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteRaVienTu.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dteRaVienTu.Properties.EditMask = "dd/MM/yyyy HH:mm";
                    dteRaVienTu.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    dteRaVienDen.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dteRaVienDen.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteRaVienDen.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dteRaVienDen.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteRaVienDen.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dteRaVienDen.Properties.EditMask = "dd/MM/yyyy HH:mm";
                    dteRaVienDen.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    dteRaVienTu.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                    dteRaVienDen.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboInHopital()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemOutDateCode = new DXMenuItem(typeCodeFind__KeyWork_OutDate, new EventHandler(cboOutOfHospital_Click));
                itemOutDateCode.Tag = "OutDate";
                menu.Items.Add(itemOutDateCode);

                DXMenuItem itemOutMonth = new DXMenuItem(typeCodeFind__OutMonth, new EventHandler(cboOutOfHospital_Click));
                itemOutMonth.Tag = "OutMonth";
                menu.Items.Add(itemOutMonth);
                DXMenuItem itemOutYear = new DXMenuItem(typeCodeFind__OutYear, new EventHandler(cboOutOfHospital_Click));
                itemOutYear.Tag = "OutYear";
                menu.Items.Add(itemOutYear);

                DXMenuItem itemOutTime = new DXMenuItem(typeCodeFind__OutTime, new EventHandler(cboOutOfHospital_Click));
                itemOutTime.Tag = "OutTime";
                menu.Items.Add(itemOutTime);

                cboVaoVien.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboOutOfHospital_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                cboVaoVien.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_OutDate = btnMenuCodeFind.Caption;

                FormatDtIntructionOutDate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //Cái này vào viện
        private void FormatDtIntructionOutDate()
        {
            try
            {
                layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                {
                    dteVaoVienTu.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dteVaoVienTu.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteVaoVienTu.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dteVaoVienTu.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteVaoVienTu.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dteVaoVienTu.Properties.EditMask = "dd/MM/yyyy";
                    dteVaoVienTu.Properties.Mask.EditMask = "dd/MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                {
                    dteVaoVienTu.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dteVaoVienTu.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteVaoVienTu.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dteVaoVienTu.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteVaoVienTu.Properties.EditFormat.FormatString = "MM/yyyy";
                    dteVaoVienTu.Properties.EditMask = "MM/yyyy";
                    dteVaoVienTu.Properties.Mask.EditMask = "MM/yyyy";
                }
                else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__InYear)
                {
                    dteVaoVienTu.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearsGroupView;
                    dteVaoVienTu.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteVaoVienTu.Properties.DisplayFormat.FormatString = "yyyy";
                    dteVaoVienTu.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteVaoVienTu.Properties.EditFormat.FormatString = "yyyy";
                    dteVaoVienTu.Properties.EditMask = "yyyy";
                    dteVaoVienTu.Properties.Mask.EditMask = "yyyy";
                }
                else
                {
                    layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dteVaoVienTu.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dteVaoVienTu.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteVaoVienTu.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dteVaoVienTu.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteVaoVienTu.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dteVaoVienTu.Properties.EditMask = "dd/MM/yyyy HH:mm";
                    dteVaoVienTu.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    dteVaoVienDen.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dteVaoVienDen.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteVaoVienDen.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dteVaoVienDen.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dteVaoVienDen.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dteVaoVienDen.Properties.EditMask = "dd/MM/yyyy HH:mm";
                    dteVaoVienDen.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    dteVaoVienTu.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                    dteVaoVienDen.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(Object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE_1>> apiResult = null;
                HisTreatmentFeeView1Filter filter = new HisTreatmentFeeView1Filter();
                filter.IS_GUARANTEE = true;
                if (!string.IsNullOrEmpty(txtMaDT.Text))
                {
                    string code = txtMaDT.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtMaDT.Text = code;
                    }
                    filter.TREATMENT_CODE = code;
                    SetDefaultValue();
                }
                else if (!string.IsNullOrEmpty(txtMaBN.Text))
                {
                    string code = txtMaBN.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtMaBN.Text = code;
                    }
                    filter.PATIENT_CODE = code;
                    SetDefaultValue();
                }
                else
                {
                    if (this._DienDieuTriSelecteds != null && this._DienDieuTriSelecteds.Count > 0)
                    {
                        filter.TDL_TREATMENT_TYPE_IDs = this._DienDieuTriSelecteds.Select(o => o.ID).ToList();
                    }
                    if (this._EndRoomSelecteds != null && this._EndRoomSelecteds.Count > 0)
                    {
                        filter.END_ROOM_IDs = this._EndRoomSelecteds.Select(o => o.ID).ToList();
                        filter.IS_PAUSE = true;
                    }
                    if (this._TrangThaiSelecteds != null && this._TrangThaiSelecteds.Count > 0)
                    {
                        if (this._TrangThaiSelecteds.Exists(o => o.ID == 3))
                        {
                            filter.FEE_LOCK_TIME_FROM = 1;
                            filter.FEE_LOCK_TIME_TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "235959");
                        }
                        if (this._TrangThaiSelecteds.Exists(o => o.ID == 4))
                        {
                            filter.IS_LOCK_HEIN = 1;
                        }

                        if (this._TrangThaiSelecteds.Exists(o => o.ID == 1) && !this._TrangThaiSelecteds.Exists(o => o.ID == 2)) filter.IS_PAUSE = false;
                        if (!this._TrangThaiSelecteds.Exists(o => o.ID == 1) && this._TrangThaiSelecteds.Exists(o => o.ID == 2)) filter.IS_PAUSE = true;
                    }
                    if (_EndDepartmentSelecteds != null && _EndDepartmentSelecteds.Count > 0)
                    {
                        var vHisRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
                        if (vHisRooms != null)
                        {

                            filter.END_ROOM_IDs = vHisRooms.Where(o => _EndDepartmentSelecteds.Exists(p => p.ID == o.DEPARTMENT_ID)).Select(p => p.ID).ToList();
                        }
                    }
                    if (cboKhoaVaoVien.EditValue != null)
                    {
                        filter.HOSPITALIZE_DEPARTMENT_ID = (long)cboKhoaVaoVien.EditValue;
                    }
                    if ((dteVaoVienTu.EditValue == null || dteVaoVienTu.EditValue.ToString() == "") && (dteRaVienTu.EditValue == null || dteRaVienTu.EditValue.ToString() == ""))
                    {
                        //WaitingManager.Hide();
                        //if (DevExpress.XtraEditors.XtraMessageBox.Show("Để tránh cao tải hệ thống, đề nghị bạn nhập thông tin \"Thời gian ra viện\" hoặc \"Thời gian vào viện\" trước khi thực hiện tìm kiếm", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                        //    return;
                    }
                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_InDate
                        && dteVaoVienTu.EditValue != null && dteVaoVienTu.DateTime != DateTime.MinValue)
                    {
                        filter.IN_DATE_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteVaoVienTu.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    else if (this.typeCodeFind__KeyWork_OutDate == typeCodeFind__InMonth
                        && dteVaoVienTu.EditValue != null && dteVaoVienTu.DateTime != DateTime.MinValue)
                    {
                        filter.IN_MONTH_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteVaoVienTu.EditValue).ToString("yyyyMM") + "00000000");
                    }
                    else if (this.typeCodeFind__KeyWork_OutDate == typeCodeFind__InYear
                        && dteVaoVienTu.EditValue != null && dteVaoVienTu.DateTime != DateTime.MinValue)
                    {
                        filter.IN_YEAR_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteVaoVienTu.EditValue).ToString("yyyy") + "0000000000");
                    }
                    else if (this.typeCodeFind__KeyWork_OutDate == typeCodeFind__InTime
                        && dteVaoVienTu.EditValue != null && dteVaoVienTu.DateTime != DateTime.MinValue
                        && dteVaoVienDen.EditValue != null && dteVaoVienDen.DateTime != DateTime.MinValue)
                    {
                        filter.IN_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteVaoVienTu.EditValue).ToString("yyyyMMddHHmm") + "00");
                        filter.IN_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteVaoVienDen.EditValue).ToString("yyyyMMddHHmm") + "59");
                    }


                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_OutDate
                        && dteRaVienTu.EditValue != null && dteRaVienTu.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_DATE_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteRaVienTu.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__OutMonth
                        && dteRaVienTu.EditValue != null && dteRaVienTu.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_MONTH_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteRaVienTu.EditValue).ToString("yyyyMM") + "00000000");
                    }
                    else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__OutYear
                        && dteRaVienTu.EditValue != null && dteRaVienTu.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_YEAR_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteRaVienTu.EditValue).ToString("yyyy") + "0000000000");
                    }
                    else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__OutTime
                        && dteRaVienTu.EditValue != null && dteRaVienTu.DateTime != DateTime.MinValue
                        && dteRaVienDen.EditValue != null && dteRaVienDen.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteRaVienTu.EditValue).ToString("yyyyMMddHHmm") + "00");
                        filter.OUT_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dteRaVienDen.EditValue).ToString("yyyyMMddHHmm") + "59");
                    }
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "CREATE_TIME";
                }

                gridView1.BeginDataUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE_1>>(RequestUriStore.GET_TREATMENT_FEE,
                   ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<V_HIS_TREATMENT_FEE_1>)apiResult.Data;
                    if (data != null)
                    {
                        gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_TREATMENT_FEE_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "ST_DISPLAY")
                        {
                            DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
                            short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_PAUSE ?? -1).ToString());
                            decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((data.IS_ACTIVE ?? -1).ToString());
                            short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_LOCK_HEIN ?? -1).ToString());
                            //Status
                            //1- dang dieu tri
                            //2- da ket thuc
                            //3- khóa hồ sơ
                            //4- duyệt bhyt
                            if (status_islockhein != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                if (status_islock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    if (status_ispause != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                    {
                                        e.Value = imageList2.Images[0];
                                    }
                                    else
                                    {
                                        e.Value = imageList2.Images[1];
                                    }
                                }
                                else
                                {
                                    e.Value = imageList2.Images[2];
                                }
                            }
                            else
                            {
                                e.Value = imageList2.Images[3];
                            }
                        }
                        else if (e.Column.FieldName == "DOB_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                        }
                        else if (e.Column.FieldName == "GUARANTEE_PRICE_STR")
                        {
                            e.Value = data.TOTAL_GUARANTEE_SERVICE_AMOUNT - data.TOTAL_BILL_GUARANTEE_AMOUNT;
                        }
                        else if (e.Column.FieldName == "TREATMENT_TYPE_NAME_STR")
                        {
                            if (data.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                            {
                                e.Value = "Điều trị nội trú";
                            }
                            else if (data.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                            {
                                e.Value = "Điều trị ngoại trú";
                            }
                            else if (data.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__TYTXA)
                            {
                                e.Value = "Điều trị lưu tại TYT xã, PKĐKKV";
                            }
                            else if (data.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                            {
                                e.Value = "Điều trị ban ngày";
                            }
                            else if (data.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__NHANTHUOC)
                            {
                                e.Value = "Nhận thuốc theo hẹn";
                            }
                            else
                            {
                                e.Value = "Khám";
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (V_HIS_TREATMENT_FEE_1)gridView1.GetFocusedRow();
                CommonParam param = new CommonParam();
                if (MessageBox.Show("Bạn có chắc chắn muốn hủy bảo lãnh không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!string.IsNullOrEmpty(Library.RegisterConfig.HisConfigCFG.GuaranteeConnection) || Library.RegisterConfig.HisConfigCFG.GuaranteeConnection != "")
                    {
                        // Parse cấu trúc: <Địa chỉ>|<Mã ứng dụng>:<Tài khoản>:<mật khẩu>|<hạn mức đăng ký mặc định>
                        string[] parts = Library.RegisterConfig.HisConfigCFG.GuaranteeConnection.Split('|');

                        if (parts.Length >= 3)
                        {
                            // Phần 1: Địa chỉ
                            string[] guaranteeAddress = parts[0].Split(';');
                            string uriHast = guaranteeAddress.Length > 0 ? guaranteeAddress[0].Trim() : "";
                            string acsPort = guaranteeAddress.Length > 1 ? guaranteeAddress[1].Trim() : "";

                            // Phần 2: Mã ứng dụng:Tài khoản:Mật khẩu
                            string[] credentials = parts[1].Split(':');
                            string guaranteeAppCode = credentials.Length > 0 ? credentials[0].Trim() : "";
                            string guaranteeUsername = credentials.Length > 1 ? credentials[1].Trim() : "";
                            string guaranteePassword = credentials.Length > 2 ? credentials[2].Trim() : "";

                            // Phần 3: Hạn mức đăng ký mặc định
                            string guaranteeDefaultLimit = parts[2].Trim();

                            // Log để kiểm tra
                            Inventec.Common.Logging.LogSystem.Debug(
                                string.Format("Guarantee Connection - Address: {0}, " +
                                "AppCode: {1}, " +
                                "Username: {2}, " +
                                "DefaultLimit: {3}", guaranteeAddress, guaranteeAppCode, guaranteeUsername, guaranteeDefaultLimit)
                            );

                            string branchHeinMediOrgCode = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;
                            MedicalExpenseGuaranteeProcessor meicalExpenseGuarantee = new MedicalExpenseGuaranteeProcessor();
                            DataInput data = new DataInput();
                            data.hasUri = uriHast;
                            data.acsUri = acsPort;
                            data.username = guaranteeUsername;
                            data.password = guaranteePassword;
                            data.applicationCode = guaranteeAppCode;
                            data.limet = guaranteeDefaultLimit;
                            data.cskcbbd = branchHeinMediOrgCode;
                            data.cancelRegisterUseRequest = new CancelRegisterUseRequest()
                            {
                                RequestId = rowData.GUARANTEE_REQUEST_CODE,
                                ContractNumber = rowData.GUARANTEE_CODE,
                                PatientFullName = rowData.TDL_PATIENT_NAME,
                                PatientDateOfBirth = rowData.TDL_PATIENT_DOB.ToString(),
                                PatientCccd = rowData.TDL_PATIENT_CCCD_NUMBER,
                                Amount = guaranteeDefaultLimit,
                                Remark = "Hủy đăng ký sử dụng bảo lãnh",
                                Signature = "",
                                Token = ""
                            };
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                            if (!string.IsNullOrEmpty(rowData.GUARANTEE_REQUEST_CODE))
                            {
                                CancelRegisterUseResponse rs = meicalExpenseGuarantee.GuaranteeCancelRegisterUse(data);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                                if (rs != null && rs.Success)
                                {
                                    LogSystem.Info("Gọi api hủy bảo lãnh thành công, RequestId: " + rowData.GUARANTEE_REQUEST_CODE);
                                    UpdateGuaranteeInfoSDO sdo = new UpdateGuaranteeInfoSDO();
                                    sdo.TreatmentId = rowData.ID;
                                    sdo.GuaranteeCode = "";
                                    sdo.GuaranteeRequestCode = "";
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                                    var rsUpdate = new BackendAdapter(param).Post<UpdateGuaranteeInfoSDO>(RequestUriStore.UPDATE_GUARANTEE_INFO, ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsUpdate), rsUpdate));
                                    if (rsUpdate != null)
                                    {
                                        XtraMessageBox.Show("Hủy bảo lãnh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        FillDataToControl();
                                    }
                                }
                                else
                                {
                                    XtraMessageBox.Show("Hủy bảo lãnh thất bại. " + rs.Data.ResponseStatus.ErrorDesc, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    LogSystem.Info("Gọi api hủy bảo lãnh thất bại, RequestId: " + rowData.GUARANTEE_REQUEST_CODE);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                txtMaDT.Text = "";
                txtMaBN.Text = "";
                FillDataToControl();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void cboPreVaoVien_Click(object sender, EventArgs e)
        {
            try
            {
                if (dteVaoVienTu.EditValue != null && dteVaoVienTu.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(cboVaoVien.Text))
                {
                    var currentdate = dteVaoVienTu.DateTime;
                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                        dteVaoVienTu.EditValue = currentdate.AddDays(-1);
                    else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                        dteVaoVienTu.EditValue = currentdate.AddMonths(-1);
                    else
                        dteVaoVienTu.EditValue = currentdate.AddYears(-1);

                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAfterVaoVien_Click(object sender, EventArgs e)
        {
            try
            {
                if (dteVaoVienTu.EditValue != null && dteVaoVienTu.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(cboVaoVien.Text))
                {
                    var currentdate = dteVaoVienTu.DateTime;
                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                        dteVaoVienTu.EditValue = currentdate.AddDays(1);
                    else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                        dteVaoVienTu.EditValue = currentdate.AddMonths(1);
                    else
                        dteVaoVienTu.EditValue = currentdate.AddYears(1);

                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPreRaVien_Click(object sender, EventArgs e)
        {
            try
            {
                if (dteRaVienTu.EditValue != null && dteRaVienTu.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(cboRaVien.Text))
                {
                    var currentdate = dteRaVienTu.DateTime;
                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                        dteRaVienTu.EditValue = currentdate.AddDays(-1);
                    else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                        dteRaVienTu.EditValue = currentdate.AddMonths(-1);
                    else
                        dteRaVienTu.EditValue = currentdate.AddYears(-1);

                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAfterRaVien_Click(object sender, EventArgs e)
        {
            try
            {
                if (dteRaVienTu.EditValue != null && dteRaVienTu.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(cboRaVien.Text))
                {
                    var currentdate = dteRaVienTu.DateTime;
                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                        dteRaVienTu.EditValue = currentdate.AddDays(1);
                    else if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind__OutMonth)
                        dteRaVienTu.EditValue = currentdate.AddMonths(1);
                    else
                        dteRaVienTu.EditValue = currentdate.AddYears(1);

                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                ResetCombo(cboDienDieuTri);
                ResetCombo(cboKhoaKetThuc);
                ResetCombo(cboPhongKetThuc);
                ResetCombo(cboTrangThai);
                cboDienDieuTri.Enabled = false;
                cboDienDieuTri.Enabled = true;
                cboKhoaKetThuc.Enabled = false;
                cboKhoaKetThuc.Enabled = true;
                cboTrangThai.Enabled = false;
                cboTrangThai.Enabled = true;
                cboPhongKetThuc.Enabled = false;
                cboPhongKetThuc.Enabled = true;
                cboKhoaVaoVien.EditValue = null;
                txtKeyWord.Text = "";
                cboVaoVien.Text = typeCodeFind_InDate;
                cboRaVien.Text = typeCodeFind_OutDate;
                this.typeCodeFind_InDate = "Trong ngày";
                this.typeCodeFind__KeyWork_InDate = this.typeCodeFind_InDate;
                FormatDtIntructionInDate();
                FormatDtIntructionOutDate();
                dteVaoVienTu.EditValue = null;
                dteRaVienTu.EditValue = null;

                layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem23.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDienDieuTri_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dienDieuTri = "";
                if (_DienDieuTriSelecteds != null && _DienDieuTriSelecteds.Count > 0)
                {
                    foreach (var item in _DienDieuTriSelecteds)
                    {
                        dienDieuTri += item.TREATMENT_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = dienDieuTri;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTrangThai_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string trangThai = "";
                if (_TrangThaiSelecteds != null && _TrangThaiSelecteds.Count > 0)
                {
                    foreach (var item in _TrangThaiSelecteds)
                    {
                        trangThai += item.TrangThai + ", ";
                    }
                }

                e.DisplayText = trangThai;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKhoaKetThuc_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string endDepartment = "";
                if (_EndDepartmentSelecteds != null && _EndDepartmentSelecteds.Count > 0)
                {
                    foreach (var item in _EndDepartmentSelecteds)
                    {
                        endDepartment += item.DEPARTMENT_NAME + ", ";
                    }
                }

                e.DisplayText = endDepartment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhongKetThuc_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (V_HIS_ROOM rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.ROOM_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetCombo(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(/*cbo.Properties.DataSource*/null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboKhoaVaoVien_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboKhoaVaoVien.EditValue = null;
                    cboKhoaVaoVien.Text = null;
                    cboKhoaVaoVien.Focus();
                }
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
                if (e.Info == null && e.SelectedControl == gridControl1)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl1.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (positionHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            positionHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "ST_DISPLAY")
                            {
                                short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(positionHandle, "IS_PAUSE") ?? "-1").ToString());
                                decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(positionHandle, "IS_ACTIVE") ?? "-1").ToString());
                                short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(positionHandle, "IS_LOCK_HEIN") ?? "-1").ToString());
                                //Status
                                //1- dang dieu tri
                                //2- da ket thuc
                                //3- khóa hồ sơ
                                //4- duyệt bhyt
                                if (status_islockhein != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    if (status_islock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                    {
                                        if (status_ispause != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                        {
                                            text = "Đang điều trị";
                                        }
                                        else
                                        {
                                            text = "Kết thúc điều trị";
                                        }
                                    }
                                    else
                                    {
                                        text = "Khóa hồ sơ";
                                    }
                                }
                                else
                                {
                                    text = "Duyệt bảo hiểm y tế";
                                }
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

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "UPDATE_GUARANTEE")
                    {
                        e.RepositoryItem = repositoryItemButtonEdit1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "UPDATE_GUARANTEE")
                    {
                        repositoryItemButtonEdit1_ButtonClick(null, null);
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
