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
using DevExpress.XtraEditors;
using HTC.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HTC.Filter;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Controls.Session;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using DevExpress.Utils;
using Inventec.Common.TypeConvert;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Plugins.TreatmentList.ADO;
using HIS.Desktop.Plugins.TreatmentList.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using MOS.SDO;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LocalStorage.Location;
using System.IO;


namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class UCTreatmentList : UserControlBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int lastRowHandle = -1;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        PopupMenuProcessor popupMenuProcessor = null;
        List<V_HIS_TREATMENT_4> ListHisTreatment = new List<V_HIS_TREATMENT_4>();
        List<HIS_ICD> ListICD = new List<HIS_ICD>();
        V_HIS_TREATMENT_4 currentTreatment = null;

        List<HIS_TREATMENT_TYPE> _DienDieuTriSelecteds;
        List<HIS_DEPARTMENT> _EndDepartmentSelecteds;
        List<KieuBenhNhanADO> _KieuBenhNhanSelecteds;
        List<TrangThaiADO> _TrangThaiSelecteds;

        List<HIS_TREATMENT_TYPE> listTreatmentType;
        List<HIS_DEPARTMENT> listDepartment;
        List<KieuBenhNhanADO> listKieuBenhNhan;
        List<TrangThaiADO> listTrangThai;
        List<V_HIS_KSK_CONTRACT> listKskContract;
        string fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", "HIS.Desktop.Plugins.TreatmentList.gridViewtreatmentList.xml"));

        public UCTreatmentList(Inventec.Desktop.Common.Modules.Module module, string treatmentCode)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                txtTreatment.Text = treatmentCode;
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCTreatmentList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            currentModule = module;
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCTreatmentList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                GetDataCombo();
                //InitComboEndDepartment();
                InitComboContract();
                InitCheck(cboDienDieuTri, SelectionGrid__DienDieuTri);
                InitCombo(cboDienDieuTri, listTreatmentType, "TREATMENT_TYPE_NAME", "ID");
                InitCheck(cboEndDepartment, SelectionGrid__EndDepartment);
                InitCombo(cboEndDepartment, listDepartment, "DEPARTMENT_NAME", "ID");
                InitCheck(cboKieuBenhNhan, SelectionGrid__KieuBenhNhan);
                InitCombo(cboKieuBenhNhan, listKieuBenhNhan, "KieuBenhNhan", "ID");
                InitCheck(cboTrangThai, SelectionGrid__TrangThai);
                InitCombo(cboTrangThai, listTrangThai, "TrangThai", "ID");
                SetDefaultControl();
                FillDataToGrid();
                HisConfigCFG.LoadConfig();
                
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataCombo()
        {
            try
            {
                listTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
                listTrangThai = new List<TrangThaiADO>();
                listKieuBenhNhan = new List<KieuBenhNhanADO>();

                listKieuBenhNhan.Add(new KieuBenhNhanADO(1, "Bệnh nhân mãn tính"));
                listKieuBenhNhan.Add(new KieuBenhNhanADO(2, "Bệnh nhân thường"));

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

        private void SelectionGrid__KieuBenhNhan(object sender, EventArgs e)
        {
            try
            {
                _KieuBenhNhanSelecteds = new List<KieuBenhNhanADO>();
                foreach (KieuBenhNhanADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _KieuBenhNhanSelecteds.Add(rv);
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

        private void InitComboContract()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisKskContractViewFilter filter = new HisKskContractViewFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "KSK_CONTRACT_CODE";
                listKskContract = new BackendAdapter(param).Get<List<V_HIS_KSK_CONTRACT>>("api/HisKskContract/GetView", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", "Mã hợp đồng", 100, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "Tên công ty", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboContract, listKskContract, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void InitComboEndDepartment()
        //{
        //    try
        //    {
        //        CommonParam param = new CommonParam();

        //        List<HIS_DEPARTMENT> listEndDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "Mã", 100, 1));
        //        columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Tên", 250, 2));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, true, 350);
        //        ControlEditorLoader.Load(cboEndDepartment, listEndDepartment, controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

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

        private void SetDefaultControl()
        {
            try
            {
                ResetCombo(cboDienDieuTri);
                ResetCombo(cboEndDepartment);
                ResetCombo(cboKieuBenhNhan);
                ResetCombo(cboTrangThai);
                cboDienDieuTri.Enabled = false;
                cboDienDieuTri.Enabled = true;
                cboEndDepartment.Enabled = false;
                cboEndDepartment.Enabled = true;
                cboKieuBenhNhan.Enabled = false;
                cboKieuBenhNhan.Enabled = true;
                cboTrangThai.Enabled = false;
                cboTrangThai.Enabled = true;
                cboContract.Enabled = false;
                cboContract.Enabled = true;

                InDepartment.Checked = true;
                dtCreateTimeFrom.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                dtCreateTimeTo.DateTime = DateTime.Now;
                dtOutTimeFrom.Text = "";
                dtOutTimeTo.Text = "";
                txtKeyword.Text = "";
                cboContract.EditValue = null;
                cboEndDepartment.EditValue = null;
                btnPrintServiceReq.Enabled = false;
                btnPrintfKSK.Enabled = false;
                txtInCode.Text = "";
                txtOutCode.Text = "";
                txtStoreCode.Text = "";
                txtPatientName.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                int pageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                //gridControlTreatmentList.DataSource = null;
                FillDataToGridTreatment(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTreatment, param, pageSize, gridControlTreatmentList);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToGridTreatment(object param)
        {
            try
            {
                start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisTreatmentView4Filter filter = new HisTreatmentView4Filter();

                if (!string.IsNullOrEmpty(txtTreatment.Text))
                {
                    string code = txtTreatment.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatment.Text = code;
                    }
                    filter = new HisTreatmentView4Filter();
                    filter.TREATMENT_CODE__EXACT = code;
                }
                else if (!string.IsNullOrEmpty(txtPatient.Text))
                {
                    string code = txtPatient.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatient.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }
                else if (!string.IsNullOrEmpty(txtStoreCode.Text))
                {
                    filter.STORE_CODE__EXACT = txtStoreCode.Text.Trim();
                }
                else if (!string.IsNullOrEmpty(txtInCode.Text))
                {
                    filter.IN_CODE__EXACT = txtInCode.Text.Trim();
                }
                else if (!string.IsNullOrEmpty(txtOutCode.Text))
                {
                    filter.END_CODE__EXACT = txtOutCode.Text.Trim();
                }
                else
                {
                    if (this.InDepartment.Checked)
                    {

                        filter.WAS_BEEN_DEPARTMENT_ID = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.currentModule.RoomId).First().DEPARTMENT_ID;
                    }

                    if (this._DienDieuTriSelecteds != null && this._DienDieuTriSelecteds.Count > 0)
                    {
                        filter.TDL_TREATMENT_TYPE_IDs = this._DienDieuTriSelecteds.Select(o => o.ID).ToList();
                    }

                    if (this._KieuBenhNhanSelecteds != null && this._KieuBenhNhanSelecteds.Count > 0)
                    {
                        if (this._KieuBenhNhanSelecteds.Exists(o => o.ID == 1))
                        {
                            filter.IS_CHRONIC = true;
                        }
                        if (this._KieuBenhNhanSelecteds.Exists(o => o.ID == 2))
                        {
                            filter.IS_CHRONIC = false;
                        }
                        if (this._KieuBenhNhanSelecteds.Exists(o => o.ID == 1) && this._KieuBenhNhanSelecteds.Exists(o => o.ID == 2))
                        {
                            filter.IS_CHRONIC = null;
                        }
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
                            filter.IS_LOCK_HEIN = true;
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
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Info("danh sach phong");
                        }
                    }

                    if (cboContract.EditValue != null)
                    {
                        filter.TDL_KSK_CONTRACT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString());
                    }

                    filter.PATIENT_NAME = txtPatientName.Text.Trim();

                    filter.KEY_WORD = txtKeyword.Text != "" ? txtKeyword.Text : null;

                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter.IN_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    }

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.IN_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                    }

                    if (dtOutTimeFrom.EditValue != null && dtOutTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtOutTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    }

                    if (dtOutTimeTo.EditValue != null && dtOutTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.OUT_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtOutTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                    }

                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                }

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_4>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW4, ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    ListHisTreatment = (List<V_HIS_TREATMENT_4>)result.Data;

                    gridControlTreatmentList.BeginUpdate();
                    gridControlTreatmentList.DataSource = null;
                    gridControlTreatmentList.DataSource = ListHisTreatment;
                    rowCount = (ListHisTreatment == null ? 0 : ListHisTreatment.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    gridControlTreatmentList.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_HIS_TREATMENT_4)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + start;
                        }
                        else if (e.Column.FieldName == "ST_DEPLAY")
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
                                        e.Value = imageList1.Images[0];
                                    }
                                    else
                                    {
                                        e.Value = imageList1.Images[1];
                                    }
                                }
                                else
                                {
                                    e.Value = imageList1.Images[2];
                                }
                            }
                            else
                            {
                                e.Value = imageList1.Images[3];
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
                        else if (e.Column.FieldName == "IS_CHRONIC_STR")
                        {
                            e.Value = "";
                            if (data.IS_CHRONIC == 1)
                            {
                                e.Value = "Mãn tính";
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "CLINICAL_IN_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CLINICAL_IN_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "ICD_MAIN_TEXT_ST")
                        {
                            e.Value = data.ICD_NAME;
                        }
                        else if (e.Column.FieldName == "AUTO_DISCOUNT_RATIO_STR")
                        {
                            e.Value = data.AUTO_DISCOUNT_RATIO.HasValue ? Inventec.Common.Number.Convert.NumberToString(((data.AUTO_DISCOUNT_RATIO ?? 0) * 100), ConfigApplications.NumberSeperator) : "";
                        }
                        else if (e.Column.FieldName == "SENT_INTEGRATE_HIS")
                        {
                            if (data.IS_INTEGRATE_HIS_SENT == (short)1)
                            {
                                e.Value = Inventec.Common.Resource.Get.Value("UCTreatmentList.DaGui", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                            }
                            else
                            {
                                e.Value = Inventec.Common.Resource.Get.Value("UCTreatmentList.ChuaGui", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
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

        private void gridViewtreatmentList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    short isPause = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewtreatmentList.GetRowCellValue(e.RowHandle, "IS_PAUSE") ?? "").ToString());
                    short isAutoDiscount = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewtreatmentList.GetRowCellValue(e.RowHandle, "IS_AUTO_DISCOUNT") ?? "").ToString());

                    string departmentIds = (gridViewtreatmentList.GetRowCellValue(e.RowHandle, "DEPARTMENT_IDS") ?? "").ToString();
                    bool AssignService = false;
                    bool isfinishButton = false;
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (e.Column.FieldName == "ServiceReq")
                    {
                        AssignService = (isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds));
                        e.RepositoryItem = (AssignService == true ? repositoryItembtnServiceReq : repositoryItembtnServiceReqU);
                    }
                    else if (e.Column.FieldName == "BedRoomIn")
                    {
                        AssignService = (isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds));
                        e.RepositoryItem = (AssignService == true ? repositoryItembtnBedRoomIn : repositoryItembtnBedRoomInU);
                    }
                    else if (e.Column.FieldName == "Finish")
                    {
                        isfinishButton = (isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds));
                        e.RepositoryItem = (isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds)) ? repositoryItembtnFinish :
(isPause == 1 && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds)) ? repositoryItembtnUnFinish :
(isPause == 1 ? repositoryItembtnUnFinish_Disable : repositoryItembtnFinish_Disable));
                    }
                    else if (e.Column.FieldName == "PRINT_DISPLAY")
                    {

                        e.RepositoryItem = repositoryItembtnEdit_Print;
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        //if (CheckLoginAdmin.IsAdmin(loginName))
                        {
                            e.RepositoryItem = BtnDelete_Enable;
                        }
                        //else
                        //{
                        //    e.RepositoryItem = BtnDelete_Disable;
                        //}
                    }
                    else if (e.Column.FieldName == "IS_AUTO_DISCOUNT_DISPLAY")
                    {
                        if (isAutoDiscount == 1)
                        {
                            e.RepositoryItem = ButtonEditIsAutoDiscount;
                        }
                    }
                    //view.EndDataUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnFind(object sender, EventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnRefreshs()
        {
            try
            {
                if (btnRefresh.Enabled)
                {
                    BtnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultControl();
                txtTreatment.Text = "";
                txtPatient.Text = "";
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnSearch()
        {
            try
            {
                if (btnFind.Enabled)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

        private void txtPatient_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlTreatmentList)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlTreatmentList.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "TDL_PATIENT_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "TDL_PATIENT_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "ST_DEPLAY")
                            {
                                short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_PAUSE") ?? "-1").ToString());
                                decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(lastRowHandle, "IS_ACTIVE") ?? "-1").ToString());
                                short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_LOCK_HEIN") ?? "-1").ToString());
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

        private void UCTreatmentList_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Control)
            //{
            // if (e.KeyCode == Keys.F) btnFind_Click(null, null);

            //  if (e.KeyCode == Keys.R)  btnRefresh_Click(null, null);

            //}

        }

        bool IsStayingDepartment(string departmentIds)
        {
            bool result = false;
            try
            {
                result = (this.currentModule != null && NowDepartmentOfTreatment(departmentIds) == HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId).DepartmentId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        long NowDepartmentOfTreatment(string departmentIds)
        {
            long result = 0;
            string strResult = "";
            List<string> DepartmentIds = departmentIds.Split(',').ToList();
            strResult = DepartmentIds[DepartmentIds.Count - 1];
            DepartmentIds = strResult.Split('_').ToList();
            strResult = DepartmentIds[0];
            result = Convert.ToInt64(strResult);
            return result;

        }

        private void cboDienDieuTri_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
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

        private void cboEndDepartment_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
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

        private void cboTrangThai_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
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

        private void cboKieuBenhNhan_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string kieuBenhNhan = "";
                if (_KieuBenhNhanSelecteds != null && _KieuBenhNhanSelecteds.Count > 0)
                {
                    foreach (var item in _KieuBenhNhanSelecteds)
                    {
                        kieuBenhNhan += item.KieuBenhNhan + ", ";
                    }
                }
                e.DisplayText = kieuBenhNhan;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboContract_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string text = "";
                if (cboContract.EditValue != null && listKskContract != null && listKskContract.Count > 0)
                {
                    var ksk = listKskContract.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString()));
                    if (ksk != null)
                    {
                        text = ksk.KSK_CONTRACT_CODE + " - " + ksk.WORK_PLACE_NAME;
                    }
                }
                e.DisplayText = text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboContract_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboContract.Enabled = false;
                cboContract.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboEndDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboEndDepartment.Enabled = false;
                cboEndDepartment.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewtreatmentList_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (gridViewtreatmentList.GetSelectedRows().Count() > 0)
                {
                    btnPrintServiceReq.Enabled = true;
                    btnPrintfKSK.Enabled = true;
                }
                else
                {
                    btnPrintServiceReq.Enabled = false;
                    btnPrintfKSK.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnImportKsk_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listObj = new List<object>();
                listObj.Add((HIS.Desktop.Common.DelegateSelectData)DelegateKSK);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisImportKsk", this.currentModule.RoomId, this.currentModule.RoomTypeId, listObj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnPrintServiceReq_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_TREATMENT_4> listTreatment = new List<V_HIS_TREATMENT_4>();
                var rowHandles = gridViewtreatmentList.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(i);
                        if (row != null)
                        {
                            listTreatment.Add(row);
                        }
                    }
                }

                if (listTreatment != null && listTreatment.Count > 0)
                {
                    foreach (var item in listTreatment)
                    {
                        HisServiceReqListResultSDO serviceReq = new HisServiceReqListResultSDO();
                        HisTreatmentWithPatientTypeInfoSDO treatmentWithPatientType = new HisTreatmentWithPatientTypeInfoSDO();
                        List<V_HIS_BED_LOG> listBedLogs = new List<V_HIS_BED_LOG>();

                        ProcessDataPrint(item, ref serviceReq, ref treatmentWithPatientType, ref listBedLogs);

                        if (serviceReq != null && treatmentWithPatientType != null && serviceReq.ServiceReqs != null && serviceReq.ServiceReqs.Count > 0)
                        {
                            HIS.Desktop.Plugins.Library.PrintServiceReq.PrintServiceReqProcessor printProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReq, treatmentWithPatientType, listBedLogs, currentModule != null ? currentModule.RoomId : 0);
                            printProcessor.SaveNPrint();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewtreatmentList_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        var treatmentData = (V_HIS_TREATMENT_4)view.GetRow(hi.RowHandle);
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        short isPause = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(hi.RowHandle, "IS_PAUSE") ?? "").ToString());
                        string departmentIds = (view.GetRowCellValue(hi.RowHandle, "DEPARTMENT_IDS") ?? "").ToString();
                        bool AssignService = false;
                        bool isfinishButton = false;

                        if (treatmentData != null)
                        {
                            if (hi.Column.FieldName == "Finish")
                            {
                                #region ----- Kết thúc điều trị -----
                                if (treatmentData != null)
                                {
                                    isfinishButton = (isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds));
                                    if ((isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds)))
                                    {
                                        repositoryItembtnFinish_Click(treatmentData);
                                    }
                                    else if ((isPause == 1 && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds))))
                                    {
                                        repositoryItembtnUnifinish_Click(treatmentData);
                                    }
                                }
                                #endregion
                            }
                            if (hi.Column.FieldName == "Delete")
                            {
                                #region ----- Xóa -----
                                //if (treatmentData != null && CheckLoginAdmin.IsAdmin(loginName))
                                {
                                    BtnDelete_Enable_ButtonClick(treatmentData);
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "PRINT_DISPLAY")
                            {
                                #region ----- In -----
                                if (treatmentData != null)
                                {
                                    repositoryItemButtonEdit_Print_ButtonClick(treatmentData);
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "ServiceReqList")
                            {
                                #region ----- Danh sách yêu cầu -----
                                if (treatmentData != null)
                                {
                                    repositoryItembtnServiceReqList_Click(treatmentData);
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "Edit")
                            {
                                #region ----- Sửa hsdt -----
                                if (treatmentData != null)
                                {
                                    repositoryItembtnEditTreatment_btnClick(treatmentData);
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "PaySereServ")
                            {
                                #region ----- Bảng kê thanh toán -----
                                if (treatmentData != null)
                                {
                                    repositoryItembtnPaySereServ_Click(treatmentData);
                                }
                                #endregion
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

        private void ProcessDataPrint(V_HIS_TREATMENT_4 treatment, ref HisServiceReqListResultSDO serviceReq, ref HisTreatmentWithPatientTypeInfoSDO treatmentWithPatientType, ref List<V_HIS_BED_LOG> bedLog)
        {
            try
            {
                var serviceReqGet = GetServiceReq(treatment.ID);
                var sereServGet = GetSereServ(treatment.ID);
                var sereServExtGet = GetSereServExt(treatment.ID);
                var bedLogGet = GetBedLog(treatment.ID);

                serviceReq.ServiceReqs = serviceReqGet != null ? serviceReqGet : new List<V_HIS_SERVICE_REQ>();
                serviceReq.SereServs = sereServGet != null ? sereServGet : new List<V_HIS_SERE_SERV>();
                serviceReq.SereServExts = sereServExtGet != null ? sereServExtGet : new List<HIS_SERE_SERV_EXT>();

                bedLog = bedLogGet != null ? bedLogGet : new List<V_HIS_BED_LOG>();

                Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(treatmentWithPatientType, treatment);
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
                LoadCurrentPatientTypeAlter(treatment.ID, ref patientTypeAlter);
                if (patientTypeAlter != null)
                {
                    treatmentWithPatientType.PATIENT_TYPE_CODE = patientTypeAlter.PATIENT_TYPE_CODE;
                    treatmentWithPatientType.HEIN_CARD_FROM_TIME = patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                    treatmentWithPatientType.HEIN_CARD_NUMBER = patientTypeAlter.HEIN_CARD_NUMBER;
                    treatmentWithPatientType.HEIN_CARD_TO_TIME = patientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                    treatmentWithPatientType.HEIN_MEDI_ORG_CODE = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                    treatmentWithPatientType.LEVEL_CODE = patientTypeAlter.LEVEL_CODE;
                    treatmentWithPatientType.RIGHT_ROUTE_CODE = patientTypeAlter.RIGHT_ROUTE_CODE;
                    treatmentWithPatientType.RIGHT_ROUTE_TYPE_CODE = patientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                    treatmentWithPatientType.TREATMENT_TYPE_CODE = patientTypeAlter.TREATMENT_TYPE_CODE;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void LoadCurrentPatientTypeAlter(long treatmentId, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();

                hisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, treatmentId, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_SERVICE_REQ> GetServiceReq(long treatmentId)
        {
            List<V_HIS_SERVICE_REQ> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.HAS_EXECUTE = true;
                filter.TREATMENT_ID = treatmentId;

                rs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private List<V_HIS_SERE_SERV> GetSereServ(long treatmentId)
        {
            List<V_HIS_SERE_SERV> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.HAS_EXECUTE = true;

                rs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private List<HIS_SERE_SERV_EXT> GetSereServExt(long treatmentId)
        {
            List<HIS_SERE_SERV_EXT> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServExtFilter filter = new HisSereServExtFilter();
                filter.TDL_TREATMENT_ID = treatmentId;

                rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private List<V_HIS_BED_LOG> GetBedLog(long treatmentId)
        {
            List<V_HIS_BED_LOG> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                bedLogFilter.TREATMENT_ID = treatmentId;

                rs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private void cboContract_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboContract.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndDepartment_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEndDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void DelegateKSK(object data)
        {
            try
            {
                if (data != null)
                {
                    cboContract.EditValue = (long)data;
                    dtCreateTimeFrom.EditValue = null;
                    dtCreateTimeTo.EditValue = null;
                    dtOutTimeFrom.EditValue = null;
                    dtOutTimeTo.EditValue = null;
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtStoreCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtInCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtOutCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnDelete_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

        }

        private void BtnDelete_Enable_ButtonClick(V_HIS_TREATMENT_4 data)
        {
            try
            {
                if (data == null) return;
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa hồ sơ điều trị?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, data);

                    WaitingManager.Show();

                    var rs = new BackendAdapter(param).Post<bool>("api/HisTreatment/DeleteTestData", ApiConsumers.MosConsumer, treatment, param);

                    if (rs)
                    {
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();

                    MessageManager.Show(this.ParentForm, param, rs);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnPrintHuongDan_Click(object sender, EventArgs e)
        {
            try
            {
                List<HIS_TREATMENT> listTreatment = new List<HIS_TREATMENT>();
                var rowHandles = gridViewtreatmentList.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(i);
                        if (row != null)
                        {
                            HIS_TREATMENT treat = new HIS_TREATMENT();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treat, row);
                            listTreatment.Add(treat);
                        }
                    }
                }

                if (listTreatment != null && listTreatment.Count > 0)
                {
                    var PrintServiceReqProcessor = new HIS.Desktop.Plugins.Library.PrintServiceReqTreatment.PrintServiceReqTreatmentProcessor(listTreatment, this.currentModule != null ? this.currentModule.RoomId : 0);
                    PrintServiceReqProcessor.Print("Mps000276", false);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn hồ sơ");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintfKSK_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_TREATMENT_4> listTreatment = new List<V_HIS_TREATMENT_4>();
                var rowHandles = gridViewtreatmentList.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_TREATMENT_4)gridViewtreatmentList.GetRow(i);
                        if (row != null)
                        {
                            listTreatment.Add(row);
                        }
                    }
                }

                if (listTreatment != null && listTreatment.Count > 0)
                {
                    ProcessPrintf(listTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
