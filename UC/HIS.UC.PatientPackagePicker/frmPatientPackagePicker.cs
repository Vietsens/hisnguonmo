/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.UC.PatientPackagePicker.ADO;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.PatientPackagePicker
{
    public partial class frmPatientPackagePicker : XtraForm
    {
        /// <summary>
        /// Caller cung cap delegate load TOAN BO chi tiet (V_HIS_PATIENT_PACKAGE_DT)
        /// cua mot goi theo PATIENT_PACKAGE_ID. Loader nay nen DUNG CHUNG giua
        /// cac caller — chi viec goi 1 API HisPatientPackageDt/Get.
        /// </summary>
        public delegate List<V_HIS_PATIENT_PACKAGE_DT> LoadDetailDelegate(long patientPackageId);

        /// <summary>
        /// Predicate loc theo loai dich vu, do TUNG caller cung cap:
        ///  - AssignService: d => !IsDrugMaterialBloodMeal(d.SERVICE_TYPE_CODE)
        ///  - AssignPrescriptionPK: d => IsDrugOrMaterial(d.SERVICE_TYPE_CODE)
        ///  - TransactionBillOther / Bordereau: null (lay tat ca)
        /// </summary>
        public delegate bool DetailFilterDelegate(V_HIS_PATIENT_PACKAGE_DT detail);

        private readonly List<HIS_PATIENT_PACKAGE> packageSource;
        private readonly LoadDetailDelegate loadDetailFunc;
        private readonly DetailFilterDelegate detailFilterFunc;

        // Map loginname -> ten nhan vien (TDL_USERNAME) lay tu V_HIS_EMPLOYEE.
        // Build 1 lan khi load form de cot Nguoi tao / Nguoi sua hien duoc ten.
        private Dictionary<string, string> userNameByLoginname
            = new Dictionary<string, string>();

        private List<HIS_PATIENT_PACKAGE> filteredPackages;

        // Chi tiet cua goi dang focus (sau khi load + ap predicate).
        private List<PackageDetailRowADO> currentDetailRows;
        private List<PackageDetailRowADO> filteredDetailRows;

        // Trang thai checkbox "Chon tat ca" ve o tieu de cot tich (colCheck).
        private bool selectAllChecked = false;

        public List<SelectedPatientPackageServiceADO> SelectedItems { get; private set; }

        public frmPatientPackagePicker(
            List<HIS_PATIENT_PACKAGE> activePackages,
            LoadDetailDelegate loadDetailFunc,
            DetailFilterDelegate detailFilterFunc)
        {
            InitializeComponent();
            SetIcon();
            this.packageSource = activePackages ?? new List<HIS_PATIENT_PACKAGE>();
            this.loadDetailFunc = loadDetailFunc;
            this.detailFilterFunc = detailFilterFunc;
            this.SelectedItems = new List<SelectedPatientPackageServiceADO>();
            this.currentDetailRows = new List<PackageDetailRowADO>();
            this.filteredDetailRows = new List<PackageDetailRowADO>();
        }

        /// <summary>
        /// Gan icon cho form theo icon cua ung dung dang chay (HIS.Desktop.exe).
        /// Dung Application.ExecutablePath de UC nay khong phai phu thuoc vao
        /// HIS.Desktop.LocalStorage.* — giu UC nhe, tai su dung duoc.
        /// </summary>
        private void SetIcon()
        {
            try
            {
                string exePath = System.Windows.Forms.Application.ExecutablePath;
                if (!string.IsNullOrEmpty(exePath) && System.IO.File.Exists(exePath))
                {
                    this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmPatientPackagePicker_Load(object sender, EventArgs e)
        {
            try
            {
                BuildUserNameMap();
                filteredPackages = packageSource.ToList();
                BindPackageGrid();
                // Sau khi bind, neu co goi -> focus dong dau de load chi tiet.
                if (filteredPackages.Count > 0)
                {
                    gridViewPackage.FocusedRowHandle = 0;
                    LoadDetailForFocusedPackage();
                }
                else
                {
                    BindDetailGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Package grid

        private void BindPackageGrid()
        {
            try
            {
                gridControlPackage.BeginUpdate();
                gridControlPackage.DataSource = filteredPackages;
                gridControlPackage.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeywordPackage_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string kw = (txtKeywordPackage.Text ?? string.Empty).Trim();
                if (kw.Length == 0)
                {
                    filteredPackages = packageSource.ToList();
                }
                else
                {
                    string kwUnsigned = Inventec.Common.String.Convert
                        .UnSignVNese2(kw)
                        .ToLowerInvariant();
                    filteredPackages = packageSource
                        .Where(p => p != null
                            && !string.IsNullOrEmpty(p.PACKAGE_NAME)
                            && Inventec.Common.String.Convert
                                .UnSignVNese2(p.PACKAGE_NAME)
                                .ToLowerInvariant()
                                .Contains(kwUnsigned))
                        .ToList();
                }
                BindPackageGrid();
                if (filteredPackages.Count > 0)
                {
                    gridViewPackage.FocusedRowHandle = 0;
                    LoadDetailForFocusedPackage();
                }
                else
                {
                    currentDetailRows.Clear();
                    BindDetailGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPackage_FocusedRowChanged(object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                if (e.FocusedRowHandle < 0) return;
                LoadDetailForFocusedPackage();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Format hien thi cho cac cot tren grid goi:
        ///  - REGISTER_DATE: long YYYYMMDDHHMMSS (gio = 000000) -> dd/MM/yyyy.
        ///  - CREATE_TIME / MODIFY_TIME: long? YYYYMMDDHHMMSS -> dd/MM/yyyy HH:mm:ss.
        ///  - CREATOR / MODIFIER: loginname -> "loginname - ten nhan vien"
        ///    (neu co resolver; neu khong chi hien loginname).
        /// </summary>
        private void gridViewPackage_CustomColumnDisplayText(object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column == colRegisterDate
                    || e.Column == colCreateTime
                    || e.Column == colModifyTime)
                {
                    if (e.Value == null) { e.DisplayText = string.Empty; return; }

                    long raw;
                    if (!long.TryParse(e.Value.ToString(), out raw) || raw <= 0)
                    {
                        e.DisplayText = string.Empty;
                        return;
                    }
                    // Ngay dang ky chi can ngay; ngay tao/sua hien ca gio phut giay.
                    e.DisplayText = e.Column == colRegisterDate
                        ? FormatDate(raw)
                        : FormatDateTime(raw);
                    return;
                }

                if (e.Column == colCreator || e.Column == colModifier)
                {
                    e.DisplayText = ResolveUserDisplay(e.Value as string);
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// long YYYYMMDDHHMMSS -> "dd/MM/yyyy".
        /// </summary>
        private string FormatDate(long raw)
        {
            try
            {
                string s = raw.ToString();
                if (s.Length < 8) return s;
                string yyyy = s.Substring(0, 4);
                string mm = s.Substring(4, 2);
                string dd = s.Substring(6, 2);
                return string.Format("{0}/{1}/{2}", dd, mm, yyyy);
            }
            catch
            {
                return raw.ToString();
            }
        }

        /// <summary>
        /// long YYYYMMDDHHMMSS -> "dd/MM/yyyy HH:mm:ss".
        /// </summary>
        private string FormatDateTime(long raw)
        {
            try
            {
                string s = raw.ToString().PadLeft(14, '0');
                if (s.Length < 14) return FormatDate(raw);
                string yyyy = s.Substring(0, 4);
                string mm = s.Substring(4, 2);
                string dd = s.Substring(6, 2);
                string hh = s.Substring(8, 2);
                string mi = s.Substring(10, 2);
                string ss = s.Substring(12, 2);
                return string.Format("{0}/{1}/{2} {3}:{4}:{5}", dd, mm, yyyy, hh, mi, ss);
            }
            catch
            {
                return FormatDate(raw);
            }
        }

        /// <summary>
        /// Build map loginname -> ten nhan vien (TDL_USERNAME) tu cache
        /// V_HIS_EMPLOYEE cua BackendData. Goi 1 lan khi load form.
        /// </summary>
        private void BuildUserNameMap()
        {
            try
            {
                userNameByLoginname = new Dictionary<string, string>();
                var employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                    .Get<V_HIS_EMPLOYEE>();
                if (employees == null) return;

                foreach (var emp in employees)
                {
                    if (emp == null || string.IsNullOrEmpty(emp.LOGINNAME)) continue;
                    // Giu ban ghi dau theo loginname (V_HIS_EMPLOYEE co the co nhieu dong).
                    if (!userNameByLoginname.ContainsKey(emp.LOGINNAME))
                        userNameByLoginname[emp.LOGINNAME] = emp.TDL_USERNAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Doi loginname sang chuoi hien thi "loginname - ten nhan vien".
        /// Soi loginname sang TDL_USERNAME trong map V_HIS_EMPLOYEE; neu khong
        /// tim thay thi chi hien loginname.
        /// </summary>
        private string ResolveUserDisplay(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) return string.Empty;

            string userName;
            if (userNameByLoginname.TryGetValue(loginName, out userName)
                && !string.IsNullOrEmpty(userName))
            {
                return string.Format("{0} - {1}", loginName, userName);
            }
            return loginName;
        }

        private HIS_PATIENT_PACKAGE GetFocusedPackage()
        {
            int handle = gridViewPackage.FocusedRowHandle;
            if (handle < 0) return null;
            return gridViewPackage.GetRow(handle) as HIS_PATIENT_PACKAGE;
        }

        #endregion

        #region Detail grid

        private void LoadDetailForFocusedPackage()
        {
            try
            {
                currentDetailRows.Clear();
                var pkg = GetFocusedPackage();
                if (pkg != null && pkg.ID > 0 && loadDetailFunc != null)
                {
                    var details = loadDetailFunc(pkg.ID) ?? new List<V_HIS_PATIENT_PACKAGE_DT>();
                    foreach (var d in details)
                    {
                        if (d == null) continue;
                        // Ap predicate cua caller (vd: bo thuoc/VT cho AssignService).
                        // Neu predicate null thi giu tat ca.
                        if (detailFilterFunc != null && !detailFilterFunc(d)) continue;
                        currentDetailRows.Add(new PackageDetailRowADO
                        {
                            Detail = d,
                            IS_CHECKED = false,
                            AMOUNT_THIS_TIME = 1m
                        });
                    }
                }
                // Goi moi -> bo tich "Chon tat ca" o tieu de cot.
                selectAllChecked = false;
                ApplyDetailFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ApplyDetailFilter()
        {
            try
            {
                string kw = (txtKeywordDetail.Text ?? string.Empty).Trim();
                if (kw.Length == 0)
                {
                    filteredDetailRows = currentDetailRows.ToList();
                }
                else
                {
                    string kwUnsigned = Inventec.Common.String.Convert
                        .UnSignVNese2(kw)
                        .ToLowerInvariant();
                    filteredDetailRows = currentDetailRows.Where(r =>
                    {
                        string code = r.SERVICE_CODE ?? string.Empty;
                        string nameUnsigned = Inventec.Common.String.Convert
                            .UnSignVNese2(r.SERVICE_NAME ?? string.Empty)
                            .ToLowerInvariant();
                        return code.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0
                            || nameUnsigned.Contains(kwUnsigned);
                    }).ToList();
                }
                BindDetailGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BindDetailGrid()
        {
            try
            {
                gridControlDetail.BeginUpdate();
                gridControlDetail.DataSource = filteredDetailRows;
                gridControlDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeywordDetail_EditValueChanged(object sender, EventArgs e)
        {
            ApplyDetailFilter();
        }

        /// <summary>
        /// Khi user sua AMOUNT_THIS_TIME: khong cho nho hon 1.
        /// Khi user tich/bo tich tung dong: dong bo lai trang thai checkbox "Chon tat ca".
        /// </summary>
        private void gridViewDetail_CellValueChanged(object sender,
            DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column == colAmountThisTime)
                {
                    var row = gridViewDetail.GetRow(e.RowHandle) as PackageDetailRowADO;
                    if (row != null)
                    {
                        if (row.AMOUNT_THIS_TIME < 1m) row.AMOUNT_THIS_TIME = 1m;
                    }
                }
                else if (e.Column == colCheck)
                {
                    SyncSelectAllStateFromRows();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ve checkbox "Chon tat ca" ngay tren tieu de cot tich (colCheck).
        /// </summary>
        private void gridViewDetail_CustomDrawColumnHeader(object sender,
            DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != colCheck) return;
                // Ve nen header chuan (bo caption) roi ve checkbox len giua.
                e.Info.InnerElements.Clear();
                e.Info.Caption = string.Empty;
                e.Painter.DrawObject(e.Info);
                DrawHeaderCheckBox(e.Cache, GetHeaderCheckBoxBounds(e.Bounds));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Click vao tieu de cot tich -> tich/bo tich toan bo dich vu trong goi.
        /// </summary>
        private void gridViewDetail_Click(object sender, EventArgs e)
        {
            try
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null) return;
                System.Drawing.Point pt = view.GridControl.PointToClient(
                    System.Windows.Forms.Control.MousePosition);
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit = view.CalcHitInfo(pt);
                if (hit.InColumnPanel && hit.Column == colCheck)
                {
                    ToggleSelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tich/bo tich TOAN BO dich vu trong goi (tac dong len currentDetailRows,
        /// ke ca dong dang bi an boi bo loc tim kiem).
        /// </summary>
        private void ToggleSelectAll()
        {
            try
            {
                if (currentDetailRows == null || currentDetailRows.Count == 0) return;

                selectAllChecked = !selectAllChecked;
                foreach (var row in currentDetailRows)
                {
                    if (row != null) row.IS_CHECKED = selectAllChecked;
                }
                gridViewDetail.CloseEditor();
                gridViewDetail.RefreshData();
                InvalidateCheckHeader();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dong bo trang thai checkbox tieu de theo cac dong: chi tick khi
        /// tat ca dich vu trong goi deu da duoc chon.
        /// </summary>
        private void SyncSelectAllStateFromRows()
        {
            bool allChecked = currentDetailRows != null
                && currentDetailRows.Count > 0
                && currentDetailRows.All(r => r != null && r.IS_CHECKED);
            if (allChecked != selectAllChecked)
            {
                selectAllChecked = allChecked;
                InvalidateCheckHeader();
            }
        }

        /// <summary>
        /// Ve glyph checkbox cua repoCheck len vung tieu de cot.
        /// </summary>
        private void DrawHeaderCheckBox(DevExpress.Utils.Drawing.GraphicsCache cache,
            System.Drawing.Rectangle bounds)
        {
            var info = repoCheck.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            var painter = repoCheck.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            if (info == null || painter == null) return;

            info.EditValue = selectAllChecked;
            info.Bounds = bounds;
            info.CalcViewInfo(cache.Graphics);
            var args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, bounds);
            painter.Draw(args);
            args.Cache = null;
        }

        /// <summary>
        /// Hop chua glyph checkbox: vuong 16px, can giua o tieu de cot.
        /// </summary>
        private System.Drawing.Rectangle GetHeaderCheckBoxBounds(System.Drawing.Rectangle headerBounds)
        {
            int size = 16;
            int x = headerBounds.X + (headerBounds.Width - size) / 2;
            int y = headerBounds.Y + (headerBounds.Height - size) / 2;
            return new System.Drawing.Rectangle(x, y, size, size);
        }

        /// <summary>
        /// Ve lai tieu de cot tich de cap nhat trang thai checkbox.
        /// </summary>
        private void InvalidateCheckHeader()
        {
            try
            {
                gridControlDetail.Invalidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Commit

        private void btnChoose_Click(object sender, EventArgs e)
        {
            CommitSelection();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmPatientPackagePicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                CommitSelection();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Tap hop cac dong da tich, kem theo HIS_PATIENT_PACKAGE cha de caller
        /// biet dich vu thuoc goi nao -> ghi PATIENT_PACKAGE_ID vao HIS_SERE_SERV.
        /// </summary>
        private void CommitSelection()
        {
            try
            {
                // Buoc cap nhat editor truoc khi doc gia tri (truong hop user dang
                // sua AMOUNT_THIS_TIME ma chua roi cell -> commit thu cong).
                gridViewDetail.CloseEditor();
                gridViewDetail.UpdateCurrentRow();

                var picked = currentDetailRows.Where(r => r.IS_CHECKED).ToList();
                if (picked.Count == 0)
                {
                    XtraMessageBox.Show(
                        "Vui lòng chọn ít nhất một dịch vụ.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var focusedPkg = GetFocusedPackage();
                this.SelectedItems = picked.Select(r => new SelectedPatientPackageServiceADO
                {
                    PatientPackage = focusedPkg,
                    PatientPackageDetail = r.Detail,
                    AmountThisTime = r.AMOUNT_THIS_TIME < 1m ? 1m : r.AMOUNT_THIS_TIME
                }).ToList();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
