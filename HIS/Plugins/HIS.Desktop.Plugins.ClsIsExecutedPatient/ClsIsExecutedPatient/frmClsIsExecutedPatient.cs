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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ClsIsExecutedPatient.ADO;
using HIS.Desktop.Plugins.ClsIsExecutedPatient.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ClsIsExecutedPatient.ClsIsExecutedPatient
{
    /// <summary>
    /// PTTK_XXXXX_Thuc_Hien_Y_Lenh_CLS_Man_Hinh_Buong_Benh
    /// Man "Thuc hien y lenh CLS": dieu duong buong benh tick "Da thuc hien y lenh" + nhap "Thoi gian thuc hien"
    /// cho tung dich vu can lam sang cua benh nhan. Bo cuc va cach thao tac sao chep man "Thuoc/vt benh nhan da dung":
    /// tick -> ghi ngay (gio = hien tai), o gio chi mo khi da tick, sua gio -> ghi lai, bo tick -> go dau.
    /// Du lieu: cung nguon du lieu cay dich vu cua man Buong benh (api/HisSereServ/GetDHisSereServ2),
    /// ghi qua api/HisSereServ/UpdateNurseExecute.
    /// </summary>
    public partial class frmClsIsExecutedPatient : FormBase
    {
        #region Declare
        private Inventec.Desktop.Common.Modules.Module ModuleData;
        private HIS_TREATMENT currentTreatment;
        /// <summary>Toan bo dich vu CLS cua ho so trong khoang thoi gian y lenh (chua loc theo Trang thai)</summary>
        private List<ClsSereServADO> allAdos = new List<ClsSereServADO>();
        private bool isFirstLoad = true;

        /// <summary>6 loai dich vu cua tab "CLS" tren man Buong benh (R1)</summary>
        private static readonly List<long> CLS_SERVICE_TYPE_IDS = new List<long>
        {
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
        };

        private const int STATUS_ALL = 0;
        private const int STATUS_EXECUTED = 1;
        private const int STATUS_NOT_EXECUTED = 2;
        #endregion

        public frmClsIsExecutedPatient(Inventec.Desktop.Common.Modules.Module moduleData, string _treatmentCode)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.ModuleData = moduleData;
                if (moduleData != null)
                {
                    this.Text = moduleData.text;
                }
                this.txtTreatmentCode.Text = _treatmentCode;
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Load / khoi tao giao dien
        private void frmClsIsExecutedPatient_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                InitComboStatus();
                InitColumns();
                InitTimeEditor();
                HideUnusedControls();

                DateTime now = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0) ?? DateTime.Now;
                dtIntructionTimeFrom.DateTime = now;
                dtIntructionTimeTo.DateTime = now;

                cboIsUse.EditValueChanged += new EventHandler(cboIsUse_EditValueChanged);
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboStatus()
        {
            try
            {
                List<FilterTypeADO> listStatusAll = new List<FilterTypeADO>();
                listStatusAll.Add(new FilterTypeADO(STATUS_ALL, "Tất cả"));
                listStatusAll.Add(new FilterTypeADO(STATUS_EXECUTED, "Đã thực hiện"));
                listStatusAll.Add(new FilterTypeADO(STATUS_NOT_EXECUTED, "Chưa thực hiện"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "id", columnInfos, false, 250);
                ControlEditorLoader.Load(cboIsUse, listStatusAll, controlEditorADO);
                cboIsUse.EditValue = STATUS_ALL;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dung lai cay cua man mau, doi nhan / truong du lieu theo 6 cot cua yeu cau:
        /// Ma dich vu, Ten dich vu, So lan, Don vi tinh, Da thuc hien y lenh, Thoi gian thuc hien.
        /// An cac cot dac thu thuoc (Sang/Trua/Chieu/Toi, nut xem chi tiet buoi).
        /// </summary>
        private void InitColumns()
        {
            try
            {
                treeListColumn1.Caption = "Mã dịch vụ";
                treeListColumn1.FieldName = "SERVICE_CODE";
                treeListColumn1.OptionsColumn.AllowEdit = false;
                treeListColumn1.VisibleIndex = 0;

                treeListColumn2.Caption = "Tên dịch vụ";
                treeListColumn2.FieldName = "SERVICE_NAME";
                treeListColumn2.OptionsColumn.AllowEdit = false;
                treeListColumn2.VisibleIndex = 1;

                treeListColumn4.Caption = "Số lần";
                treeListColumn4.FieldName = "AMOUNT";
                treeListColumn4.OptionsColumn.AllowEdit = false;
                treeListColumn4.VisibleIndex = 2;

                treeListColumn3.Caption = "Đơn vị tính";
                treeListColumn3.FieldName = "SERVICE_UNIT_NAME";
                treeListColumn3.OptionsColumn.AllowEdit = false;
                treeListColumn3.VisibleIndex = 3;

                treeListColumn_IsUsed.Caption = "Đã thực hiện y lệnh";
                treeListColumn_IsUsed.FieldName = "IS_EXECUTED";
                treeListColumn_IsUsed.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Bound;
                treeListColumn_IsUsed.OptionsColumn.AllowEdit = true;
                treeListColumn_IsUsed.VisibleIndex = 4;

                treeListColumn10.Caption = "Thời gian thực hiện";
                treeListColumn10.FieldName = "EXECUTE_TIME_DT";
                treeListColumn10.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Bound;
                treeListColumn10.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
                treeListColumn10.Format.FormatString = "dd/MM/yyyy HH:mm";
                treeListColumn10.OptionsColumn.AllowEdit = true;
                treeListColumn10.VisibleIndex = 5;
                treeListColumn10.Width = 130;

                // Cot dac thu thuoc — khong dung cho CLS
                treeListColumn5.Visible = false;
                treeListColumn6.Visible = false;
                treeListColumn8.Visible = false;
                treeListColumn9.Visible = false;
                treeListColumn7.Visible = false;
                treeListColumn11.Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>O "Thoi gian thuc hien": ngay + gio phut, dinh dang dd/MM/yyyy HH:mm</summary>
        private void InitTimeEditor()
        {
            try
            {
                if (repositoryUsedTime != null)
                {
                    repositoryUsedTime.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
                    repositoryUsedTime.CalendarTimeProperties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    repositoryUsedTime.CalendarTimeProperties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    repositoryUsedTime.CalendarTimeProperties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    repositoryUsedTime.CalendarTimeProperties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    repositoryUsedTime.CalendarTimeProperties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    repositoryUsedTime.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    repositoryUsedTime.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    repositoryUsedTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    repositoryUsedTime.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    repositoryUsedTime.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    repositoryUsedTime.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Vista;
                    repositoryUsedTime.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
                    repositoryUsedTime.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Man nay ghi ngay tung dong nen khong can nut Luu; popup chi tiet buoi dung cua man mau cung khong dung</summary>
        private void HideUnusedControls()
        {
            try
            {
                btnSave.Visible = false;
                barButtonItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                foreach (BaseLayoutItem item in layoutControl1.Items)
                {
                    LayoutControlItem lci = item as LayoutControlItem;
                    if (lci != null && lci.Control == btnSave)
                    {
                        lci.Visibility = LayoutVisibility.Never;
                    }
                }
                popupControlContainer1.Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Tim kiem / nap du lieu
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultValueControl();
                allAdos = new List<ClsSereServADO>();
                currentTreatment = LoadSearch();
                if (currentTreatment == null || currentTreatment.ID <= 0)
                {
                    currentTreatment = null;
                    WaitingManager.Hide();
                    MessageBox.Show(Resources.ResourceLanguageManager.KhongTimThayMaDieuTri);
                    return;
                }
                // R12: lan mo dau tien -> khoang thoi gian y lenh mac dinh tu ngay vao vien den hom nay
                if (isFirstLoad && currentTreatment.IN_TIME > 0)
                {
                    DateTime? inTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatment.IN_TIME);
                    if (inTime.HasValue && inTime.Value < dtIntructionTimeFrom.DateTime)
                    {
                        dtIntructionTimeFrom.DateTime = inTime.Value;
                    }
                }
                isFirstLoad = false;
                FillInfoPatient(currentTreatment);
                allAdos = LoadClsSereServs(currentTreatment);
                BindTree();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_TREATMENT LoadSearch()
        {
            HIS_TREATMENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                    var listTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        result = listTreatment.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Lay dich vu CLS cua ho so tu cung nguon du lieu cay dich vu man Buong benh (view D_HIS_SERE_SERV_2):
        /// loai tru dich vu "khong thuc hien" (R3), chi 6 loai CLS (R1), loc theo khoang thoi gian y lenh; sap theo thoi gian y lenh.
        /// </summary>
        private List<ClsSereServADO> LoadClsSereServs(HIS_TREATMENT treatment)
        {
            List<ClsSereServADO> result = new List<ClsSereServADO>();
            try
            {
                if (treatment == null || treatment.ID <= 0) return result;
                CommonParam param = new CommonParam();
                DHisSereServ2Filter filter = new DHisSereServ2Filter();
                filter.TREATMENT_ID = treatment.ID;
                filter.IS_NO_EXECUTE = false;
                List<DHisSereServ2> data = new BackendAdapter(param).Get<List<DHisSereServ2>>("api/HisSereServ/GetDHisSereServ2", ApiConsumers.MosConsumer, filter, param);
                if (data == null || data.Count == 0) return result;

                long timeFrom = 0;
                long timeTo = long.MaxValue;
                if (dtIntructionTimeFrom.EditValue != null && dtIntructionTimeFrom.DateTime != DateTime.MinValue)
                {
                    timeFrom = Convert.ToInt64(dtIntructionTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtIntructionTimeTo.EditValue != null && dtIntructionTimeTo.DateTime != DateTime.MinValue)
                {
                    timeTo = Convert.ToInt64(dtIntructionTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                var clsRows = data.Where(o => o.SERE_SERV_ID.HasValue && o.SERE_SERV_ID.Value > 0
                        && o.TDL_SERVICE_TYPE_ID.HasValue && CLS_SERVICE_TYPE_IDS.Contains(o.TDL_SERVICE_TYPE_ID.Value)
                        && (o.TDL_INTRUCTION_TIME ?? 0) >= timeFrom && (o.TDL_INTRUCTION_TIME ?? 0) <= timeTo)
                    .OrderBy(o => o.TDL_INTRUCTION_TIME ?? 0).ThenBy(o => o.SERVICE_REQ_ID ?? 0).ThenBy(o => o.SERVICE_CODE)
                    .ToList();
                foreach (var item in clsRows)
                {
                    ClsSereServADO ado = new ClsSereServADO();
                    ado.IS_PARENT = false;
                    ado.SERE_SERV_ID = item.SERE_SERV_ID.Value;
                    ado.SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                    ado.SERVICE_REQ_CODE = item.SERVICE_REQ_CODE;
                    ado.REQUEST_LOGINNAME = item.REQUEST_LOGINNAME;
                    ado.REQUEST_USERNAME = item.REQUEST_USERNAME;
                    ado.INTRUCTION_TIME = item.TDL_INTRUCTION_TIME ?? 0;
                    ado.TDL_SERVICE_TYPE_ID = item.TDL_SERVICE_TYPE_ID;
                    ado.SERVICE_CODE = item.SERVICE_CODE;
                    ado.SERVICE_NAME = item.SERVICE_NAME;
                    ado.AMOUNT = item.AMOUNT;
                    ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                    ado.IS_EXECUTED = item.IS_NURSE_EXECUTED == 1;
                    ado.EXECUTE_TIME = item.IS_NURSE_EXECUTED == 1 ? item.NURSE_EXECUTE_TIME : null;
                    ado.EXECUTE_TIME_DT = ToDateTime(ado.EXECUTE_TIME);
                    ado.EXECUTE_USERNAME = item.NURSE_EXECUTE_USERNAME;
                    ado.CONCRETE_ID__IN_SETY = (item.SERVICE_REQ_ID ?? 0) + "_" + ado.SERE_SERV_ID;
                    ado.PARENT_ID__IN_SETY = (item.SERVICE_REQ_ID ?? 0).ToString();
                    result.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Loc theo Trang thai (Tat ca / Da thuc hien / Chua thuc hien) tren du lieu da nap, dung cay theo y lenh</summary>
        private void BindTree()
        {
            try
            {
                int status = STATUS_ALL;
                if (cboIsUse.EditValue != null)
                {
                    int.TryParse(cboIsUse.EditValue.ToString(), out status);
                }
                List<ClsSereServADO> children = allAdos ?? new List<ClsSereServADO>();
                if (status == STATUS_EXECUTED)
                {
                    children = children.Where(o => o.IS_EXECUTED == true).ToList();
                }
                else if (status == STATUS_NOT_EXECUTED)
                {
                    children = children.Where(o => o.IS_EXECUTED != true).ToList();
                }

                BindingList<ClsSereServADO> rows = new BindingList<ClsSereServADO>();
                foreach (var group in children.GroupBy(o => o.SERVICE_REQ_ID ?? 0).OrderBy(g => g.Min(o => o.INTRUCTION_TIME)))
                {
                    ClsSereServADO first = group.First();
                    ClsSereServADO parent = new ClsSereServADO();
                    parent.IS_PARENT = true;
                    parent.CONCRETE_ID__IN_SETY = group.Key.ToString();
                    parent.PARENT_ID__IN_SETY = null;
                    parent.SERVICE_REQ_ID = first.SERVICE_REQ_ID;
                    parent.SERVICE_REQ_CODE = first.SERVICE_REQ_CODE;
                    parent.INTRUCTION_TIME = first.INTRUCTION_TIME;
                    parent.SERVICE_CODE = first.SERVICE_REQ_CODE;
                    parent.SERVICE_NAME = string.Format("{0} - {1} (TG y lệnh: {2})", first.REQUEST_LOGINNAME, first.REQUEST_USERNAME,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(first.INTRUCTION_TIME));
                    rows.Add(parent);
                    foreach (var child in group.OrderBy(o => o.SERVICE_CODE))
                    {
                        rows.Add(child);
                    }
                }
                treeMedicineIsUsePt.BeginUnboundLoad();
                treeMedicineIsUsePt.DataSource = rows;
                treeMedicineIsUsePt.EndUnboundLoad();
                treeMedicineIsUsePt.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIsUse_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                BindTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillInfoPatient(HIS_TREATMENT data)
        {
            try
            {
                if (data != null)
                {
                    lbPatientCode.Text = data.TDL_PATIENT_CODE;
                    lbPatientName.Text = data.TDL_PATIENT_NAME;
                    lbDateOfBirth.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    lbGender.Text = data.TDL_PATIENT_GENDER_NAME;
                    lbAddress.Text = data.TDL_PATIENT_ADDRESS;

                    var lastPatientType = new BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, data.ID, null);
                    if (lastPatientType != null)
                    {
                        lbHeinCard.Text = HeinCardHelper.TrimHeinCardNumber(lastPatientType.HEIN_CARD_NUMBER);
                        lbDateFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(lastPatientType.HEIN_CARD_FROM_TIME ?? 0);
                        lbDateTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(lastPatientType.HEIN_CARD_TO_TIME ?? 0);
                        lbPlaceToTreat.Text = lastPatientType.HEIN_MEDI_ORG_NAME;
                        lbPatientType.Text = lastPatientType.PATIENT_TYPE_NAME ?? "";
                        lblRightRoute.Text = lastPatientType.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE ? "Đúng tuyến" : "Trái tuyến";
                        string ratio = "";
                        if (lastPatientType.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            decimal? heinRatio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(lastPatientType.HEIN_TREATMENT_TYPE_CODE, lastPatientType.HEIN_CARD_NUMBER, lastPatientType.LEVEL_CODE, lastPatientType.RIGHT_ROUTE_CODE, lastPatientType.FACILITY_CLASS, lastPatientType.FORMER_LEVEL_CODE, (long)(lastPatientType.CLASSIFY_POINT ?? 0), data.CLINICAL_IN_TIME ?? 0);
                            if (heinRatio.HasValue)
                            {
                                ratio = ((long)(heinRatio.Value * 100)).ToString() + "%";
                            }
                        }
                        lblHeinRatio.Text = ratio;
                    }
                }
                else
                {
                    SetDefaultValueControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                lbPatientCode.Text = "";
                lbPatientName.Text = "";
                lbHeinCard.Text = "";
                lblHeinRatio.Text = "";
                lbGender.Text = "";
                lbDateFrom.Text = "";
                lbDateOfBirth.Text = "";
                lbDateTo.Text = "";
                lbAddress.Text = "";
                lblRightRoute.Text = "";
                lbPlaceToTreat.Text = "";
                lbPatientType.Text = "";
                treeMedicineIsUsePt.ClearNodes();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Moi (Ctrl N): dat lai bo loc mac dinh (tu ngay vao vien -> hom nay, Tat ca) roi tim lai</summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime now = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0) ?? DateTime.Now;
                dtIntructionTimeTo.DateTime = now;
                DateTime? inTime = currentTreatment != null && currentTreatment.IN_TIME > 0
                    ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatment.IN_TIME) : null;
                dtIntructionTimeFrom.DateTime = inTime ?? now;
                cboIsUse.EditValue = STATUS_ALL;
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWords_KeyUp(object sender, KeyEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWords_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Cay: editor / mau sac
        private void treeMedicineIsUsePt_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                ClsSereServADO rowData = treeMedicineIsUsePt.GetDataRecordByNode(e.Node) as ClsSereServADO;
                if (rowData == null) return;
                if (!rowData.IS_PARENT && e.Column.FieldName == "IS_EXECUTED")
                {
                    e.RepositoryItem = repositoryItemCheckEditUsed;
                }
                else if (rowData.IS_PARENT && (e.Column.FieldName == "IS_EXECUTED" || e.Column.FieldName == "EXECUTE_TIME_DT"))
                {
                    // Dong y lenh: khong co checkbox / o gio
                    e.RepositoryItem = new RepositoryItem();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>R5: o "Thoi gian thuc hien" chi mo nhap khi dong da tick "Da thuc hien y lenh"</summary>
        private void treeMedicineIsUsePt_CustomNodeCellEditForEditing(object sender, GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                ClsSereServADO rowData = treeMedicineIsUsePt.GetDataRecordByNode(e.Node) as ClsSereServADO;
                if (rowData == null || rowData.IS_PARENT) return;
                if (e.Column.FieldName == "EXECUTE_TIME_DT")
                {
                    if (rowData.IS_EXECUTED == true)
                    {
                        e.RepositoryItem = repositoryUsedTime;
                    }
                    else
                    {
                        e.RepositoryItem = new RepositoryItem();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeMedicineIsUsePt_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                ClsSereServADO rowData = treeMedicineIsUsePt.GetDataRecordByNode(e.Node) as ClsSereServADO;
                if (rowData == null) return;
                if (rowData.IS_PARENT)
                {
                    e.Appearance.ForeColor = Color.Black;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    e.Appearance.BackColor = Color.Yellow;
                    e.Appearance.BackColor2 = Color.Yellow;
                }
                else
                {
                    e.Appearance.ForeColor = Color.Black;
                    if (e.Column.FieldName == "EXECUTE_TIME_DT" && rowData.IS_IN_VALID)
                    {
                        e.Appearance.BackColor = Color.Maroon;
                        e.Appearance.BackColor2 = Color.Maroon;
                        e.Appearance.ForeColor = Color.White;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Tick / bo tick / sua gio -> ghi ngay
        /// <summary>
        /// R4: tick -> ghi ngay voi gio = hien tai, o gio mo cho sua; bo tick -> go dau, o gio dong.
        /// That bai (backend tu choi) -> huy thao tac tick, hien thong bao backend.
        /// </summary>
        private void repositoryItemCheckEditUsed_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                CheckEdit checkEdit = sender as CheckEdit;
                TreeList treeList = checkEdit != null ? checkEdit.Parent as TreeList : null;
                if (treeList == null || treeList.FocusedNode == null) return;
                ClsSereServADO rowData = treeList.GetDataRecordByNode(treeList.FocusedNode) as ClsSereServADO;
                if (rowData == null || rowData.IS_PARENT)
                {
                    e.Cancel = true;
                    return;
                }
                bool isChecked = e.NewValue is bool && (bool)e.NewValue;
                long? executeTime = isChecked ? Inventec.Common.DateTime.Get.Now() : null;
                if (isChecked && executeTime.HasValue && executeTime.Value < rowData.INTRUCTION_TIME)
                {
                    XtraMessageBox.Show(string.Format("Thời gian thực hiện không được trước thời gian y lệnh ({0})", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowData.INTRUCTION_TIME)), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }
                bool success = CallUpdateNurseExecute(rowData, isChecked, executeTime);
                if (success)
                {
                    rowData.IS_EXECUTED = isChecked;
                    rowData.EXECUTE_TIME = isChecked ? executeTime : null;
                    rowData.EXECUTE_TIME_DT = ToDateTime(rowData.EXECUTE_TIME);
                    rowData.IS_IN_VALID = false;
                }
                else
                {
                    e.Cancel = true;
                }
                treeList.RefreshNode(treeList.FocusedNode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// R6: sua gio o dong da tick -> kiem tra som (khong truoc gio y lenh, khong sau hien tai); hop le -> ghi ngay;
        /// vi pham -> to canh bao, giu gia tri cu, khong goi API.
        /// </summary>
        private void repositoryUsedTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DateEdit dateEdit = sender as DateEdit;
                TreeList treeList = dateEdit != null ? dateEdit.Parent as TreeList : null;
                if (treeList == null || treeList.FocusedNode == null) return;
                ClsSereServADO rowData = treeList.GetDataRecordByNode(treeList.FocusedNode) as ClsSereServADO;
                if (rowData == null || rowData.IS_PARENT || rowData.IS_EXECUTED != true) return;
                if (dateEdit.EditValue == null || dateEdit.DateTime == DateTime.MinValue)
                {
                    // Xoa trang gio -> khong hop le, giu gia tri da luu
                    rowData.IS_IN_VALID = true;
                    treeList.RefreshNode(treeList.FocusedNode);
                    return;
                }
                DateTime picked = dateEdit.DateTime;
                picked = new DateTime(picked.Year, picked.Month, picked.Day, picked.Hour, picked.Minute, 0);
                long newTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(picked) ?? 0;
                if (newTime <= 0) return;
                if (rowData.EXECUTE_TIME.HasValue && newTime == rowData.EXECUTE_TIME.Value)
                {
                    rowData.IS_IN_VALID = false;
                    return;
                }
                long now = Inventec.Common.DateTime.Get.Now() ?? 0;
                if (newTime < rowData.INTRUCTION_TIME)
                {
                    rowData.IS_IN_VALID = true;
                    XtraMessageBox.Show(string.Format("Thời gian thực hiện không được trước thời gian y lệnh ({0})", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowData.INTRUCTION_TIME)), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    treeList.RefreshNode(treeList.FocusedNode);
                    return;
                }
                if (now > 0 && newTime > now)
                {
                    rowData.IS_IN_VALID = true;
                    XtraMessageBox.Show("Thời gian thực hiện không được sau thời điểm hiện tại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    treeList.RefreshNode(treeList.FocusedNode);
                    return;
                }
                bool success = CallUpdateNurseExecute(rowData, true, newTime);
                if (success)
                {
                    rowData.EXECUTE_TIME = newTime;
                    rowData.EXECUTE_TIME_DT = picked;
                    rowData.IS_IN_VALID = false;
                }
                else
                {
                    rowData.EXECUTE_TIME_DT = ToDateTime(rowData.EXECUTE_TIME);
                    rowData.IS_IN_VALID = false;
                }
                treeList.RefreshNode(treeList.FocusedNode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Goi api/HisSereServ/UpdateNurseExecute cho 1 dong; hien thong bao backend khi that bai</summary>
        private bool CallUpdateNurseExecute(ClsSereServADO rowData, bool isExecuted, long? executeTime)
        {
            bool success = false;
            try
            {
                List<HisSereServNurseExecuteSDO> sdos = new List<HisSereServNurseExecuteSDO>();
                HisSereServNurseExecuteSDO sdo = new HisSereServNurseExecuteSDO();
                sdo.SereServId = rowData.SERE_SERV_ID;
                sdo.IsExecuted = isExecuted;
                sdo.ExecuteTime = isExecuted ? executeTime : null;
                sdos.Add(sdo);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdos), sdos));
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param).Post<List<HIS_SERE_SERV>>("api/HisSereServ/UpdateNurseExecute", ApiConsumers.MosConsumer, sdos, param);
                success = result != null && result.Count > 0;
                if (!success)
                {
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private static DateTime? ToDateTime(long? timeNumber)
        {
            try
            {
                if (!timeNumber.HasValue || timeNumber.Value <= 0) return null;
                return Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeNumber.Value);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
        #endregion
    }
}
