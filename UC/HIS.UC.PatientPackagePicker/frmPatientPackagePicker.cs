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

        private readonly List<V_HIS_PATIENT_PACKAGE> packageSource;
        private readonly LoadDetailDelegate loadDetailFunc;
        private readonly DetailFilterDelegate detailFilterFunc;

        private List<V_HIS_PATIENT_PACKAGE> filteredPackages;

        // Chi tiet cua goi dang focus (sau khi load + ap predicate).
        private List<PackageDetailRowADO> currentDetailRows;
        private List<PackageDetailRowADO> filteredDetailRows;

        public List<SelectedPatientPackageServiceADO> SelectedItems { get; private set; }

        public frmPatientPackagePicker(
            List<V_HIS_PATIENT_PACKAGE> activePackages,
            LoadDetailDelegate loadDetailFunc,
            DetailFilterDelegate detailFilterFunc)
        {
            InitializeComponent();
            this.packageSource = activePackages ?? new List<V_HIS_PATIENT_PACKAGE>();
            this.loadDetailFunc = loadDetailFunc;
            this.detailFilterFunc = detailFilterFunc;
            this.SelectedItems = new List<SelectedPatientPackageServiceADO>();
            this.currentDetailRows = new List<PackageDetailRowADO>();
            this.filteredDetailRows = new List<PackageDetailRowADO>();
        }

        private void frmPatientPackagePicker_Load(object sender, EventArgs e)
        {
            try
            {
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
        /// CREATE_TIME / MODIFY_TIME / REGISTER_DATE trong EFMODEL HIS thuong la
        /// long YYYYMMDDHHMMSS. Format ve dd/MM/yyyy de hien thi cho gon.
        /// </summary>
        private void gridViewPackage_CustomColumnDisplayText(object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column != colRegisterDate
                    && e.Column != colCreateTime
                    && e.Column != colModifyTime) return;
                if (e.Value == null) { e.DisplayText = string.Empty; return; }

                long raw;
                if (!long.TryParse(e.Value.ToString(), out raw) || raw <= 0)
                {
                    e.DisplayText = string.Empty;
                    return;
                }
                e.DisplayText = FormatYyyymmddhhmmss(raw);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string FormatYyyymmddhhmmss(long raw)
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

        private V_HIS_PATIENT_PACKAGE GetFocusedPackage()
        {
            int handle = gridViewPackage.FocusedRowHandle;
            if (handle < 0) return null;
            return gridViewPackage.GetRow(handle) as V_HIS_PATIENT_PACKAGE;
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
        /// Tap hop cac dong da tich, kem theo V_HIS_PATIENT_PACKAGE cha de caller
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
