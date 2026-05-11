/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using Inventec.Core;

namespace HIS.UC.MediOrgPicker
{
    public partial class frmMediOrgPicker : XtraForm
    {
        private const string DefaultPrefix = "C";
        private const int DefaultPageSize = 50;
        private const int SelectedValueMaxLength = 10;

        private List<MediOrgADO> allRows;
        private List<MediOrgADO> filteredRows;
        private int rowCount;
        private int dataTotal;

        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider selectedValueErrorProvider;
        private bool isSyncingSelectedValue;
        // Chan auto-fill txtSelectedValue khi grid bind data lan dau (row 0 tu dong focus
        // se fire FocusedRowChanged). Chi cho phep ghep sau khi user thuc su tuong tac.
        private bool isInitialLoad = true;

        public string SelectedValue { get; private set; }

        // initialValue được giữ chữ ký constructor để không phá public API,
        // nhưng cố ý KHÔNG dùng để pre-select hay filter — mỗi lần mở picker
        // user thấy danh sách đầy đủ và chọn lại từ đầu.
        public frmMediOrgPicker(string initialValue)
        {
            InitializeComponent();
        }

        private void frmMediOrgPicker_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataSource();
                EnsureSelectedValueErrorProvider();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                // Sau khi grid da bind xong (FocusedRowChanged cho row 0 da fire va bi bo qua),
                // mo cong cho cac tuong tac that su cua user.
                isInitialLoad = false;
            }
        }

        private void LoadDataSource()
        {
            var source = MediOrgDataWorker.MediOrgADOs ?? new List<MediOrgADO>();
            allRows = source
                .Where(o => o != null && o.IS_ACTIVE == 1 && o.IS_DELETE != 1)
                .OrderBy(o => o.MEDI_ORG_CODE)
                .ToList();
            filteredRows = allRows;
        }

        private void FillDataToGrid()
        {
            try
            {
                int pageSize = ucPaging1.pagingGrid != null
                    ? ucPaging1.pagingGrid.PageSize
                    : DefaultPageSize;

                FillDataToGridPage(new CommonParam(0, pageSize));

                var param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridPage, param, pageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridPage(object param)
        {
            try
            {
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? DefaultPageSize;
                if (limit <= 0) limit = DefaultPageSize;

                if (filteredRows == null) filteredRows = new List<MediOrgADO>();
                dataTotal = filteredRows.Count;

                var page = filteredRows.Skip(start).Take(limit).ToList();
                rowCount = page.Count;

                gridControlMediOrg.BeginUpdate();
                gridControlMediOrg.DataSource = page;
                gridControlMediOrg.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (allRows == null) return;
                string kw = (txtKeyword.Text ?? string.Empty).Trim();
                if (kw.Length == 0)
                {
                    filteredRows = allRows;
                }
                else
                {
                    string kwUnsigned = Inventec.Common.String.Convert.UnSignVNese2(kw).ToLowerInvariant();
                    filteredRows = allRows.Where(o =>
                    {
                        string code = o.MEDI_ORG_CODE ?? string.Empty;
                        string nameUnsigned = o.MEDI_ORG_NAME_UNSIGNED ?? string.Empty;
                        return code.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0
                            || nameUnsigned.ToLowerInvariant().Contains(kwUnsigned);
                    }).ToList();
                }
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter)
            {
                gridViewMediOrg.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Lay prefix hien tai tu txtSelectedValue (phan truoc dau "."), default "C".
        /// User co the go lai prefix trong textEdit (vi du "X") va se duoc giu lai
        /// khi click sang row khac.
        /// </summary>
        private string GetCurrentPrefix()
        {
            string text = (txtSelectedValue.Text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(text)) return DefaultPrefix;
            int dot = text.IndexOf('.');
            if (dot > 0)
            {
                string p = text.Substring(0, dot);
                if (!string.IsNullOrWhiteSpace(p)) return p;
            }
            return DefaultPrefix;
        }

        private MediOrgADO GetFocusedRow()
        {
            int handle = gridViewMediOrg.FocusedRowHandle;
            if (handle < 0) return null;
            return gridViewMediOrg.GetRow(handle) as MediOrgADO;
        }

        /// <summary>
        /// Khi user CLICK chu dong vao 1 row: tu dong ghep prefix + "." + MEDI_ORG_CODE
        /// vao txtSelectedValue. Prefix lay tu chinh txtSelectedValue (giu cai user da go).
        /// Khong dung FocusedRowChanged vi se trigger ca khi grid rebind data (lan dau mo
        /// form, sau moi lan search) — gay hieu ung tu dong dien "C.00000" du user chua chon gi.
        /// </summary>
        private void gridViewMediOrg_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            UpdateSelectedValueFromFocusedRow();
        }

        private void UpdateSelectedValueFromFocusedRow()
        {
            try
            {
                if (isInitialLoad) return;
                var row = GetFocusedRow();
                if (row == null || string.IsNullOrEmpty(row.MEDI_ORG_CODE)) return;

                string newText = GetCurrentPrefix() + "." + row.MEDI_ORG_CODE;

                isSyncingSelectedValue = true;
                txtSelectedValue.Text = newText;
                isSyncingSelectedValue = false;

                UpdateSelectedValueErrorState();
            }
            catch (Exception ex)
            {
                isSyncingSelectedValue = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnsureSelectedValueErrorProvider()
        {
            try
            {
                if (selectedValueErrorProvider != null) return;
                selectedValueErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
                selectedValueErrorProvider.ContainerControl = this;
                selectedValueErrorProvider.SetIconAlignment(
                    txtSelectedValue,
                    System.Windows.Forms.ErrorIconAlignment.MiddleRight);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateSelectedValueErrorState()
        {
            try
            {
                EnsureSelectedValueErrorProvider();
                if (selectedValueErrorProvider == null) return;
                string text = (txtSelectedValue.Text ?? string.Empty).Trim();
                if (text.Length > SelectedValueMaxLength)
                {
                    selectedValueErrorProvider.SetError(
                        txtSelectedValue,
                        string.Format("Mã CSKCB chuyển tối đa {0} ký tự", SelectedValueMaxLength),
                        DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                }
                else
                {
                    selectedValueErrorProvider.SetError(txtSelectedValue, "");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSelectedValue_EditValueChanged(object sender, EventArgs e)
        {
            if (isSyncingSelectedValue) { UpdateSelectedValueErrorState(); return; }
            UpdateSelectedValueErrorState();
        }

        private bool ValidateSelectedValueLength()
        {
            string value = (txtSelectedValue.Text ?? string.Empty).Trim();
            if (value.Length > SelectedValueMaxLength)
            {
                XtraMessageBox.Show(
                    string.Format("Mã CSKCB chuyển tối đa {0} ký tự", SelectedValueMaxLength),
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtSelectedValue.Focus();
                return false;
            }
            return true;
        }

        private void gridViewMediOrg_DoubleClick(object sender, EventArgs e)
        {
            // Double-click row: dam bao txtSelectedValue da duoc ghep theo row do roi commit.
            UpdateSelectedValueFromFocusedRow();
            CommitSelection();
        }

        private void gridViewMediOrg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateSelectedValueFromFocusedRow();
                CommitSelection();
                e.Handled = true;
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            CommitSelection();
        }

        private void frmMediOrgPicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                CommitSelection();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void CommitSelection()
        {
            try
            {
                string value = (txtSelectedValue.Text ?? string.Empty).Trim();

                // Bat buoc user phai chon mot dong (qua RowClick/Enter/DoubleClick)
                // hoac tu nhap vao textEdit. Khong fallback lay row dang focus
                // de tranh tu dong nhan dong dau tien khi user chua chon gi.
                if (string.IsNullOrEmpty(value))
                {
                    XtraMessageBox.Show(
                        "Vui lòng chọn cơ sở khám chữa bệnh",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Validate do dai (giong 3 man cha) — chan commit neu vuot 10 ky tu.
                if (value.Length > SelectedValueMaxLength)
                {
                    XtraMessageBox.Show(
                        string.Format("Mã CSKCB chuyển tối đa {0} ký tự", SelectedValueMaxLength),
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    txtSelectedValue.Focus();
                    return;
                }

                this.SelectedValue = value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
