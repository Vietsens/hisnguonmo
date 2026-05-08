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

        private List<MediOrgADO> allRows;
        private List<MediOrgADO> filteredRows;
        private readonly string initialValue;
        private int rowCount;
        private int dataTotal;

        public string SelectedValue { get; private set; }

        public frmMediOrgPicker(string initialValue)
        {
            InitializeComponent();
            this.initialValue = initialValue ?? string.Empty;
        }

        private void frmMediOrgPicker_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataSource();
                PrefillKeywordFromInitial();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Neu form goi truyen vao "C.01234" thi tu dong dien "01234" vao o tim kiem
        /// de loc ngay den dong tuong ung, thay vi nhay trang.
        /// </summary>
        private void PrefillKeywordFromInitial()
        {
            string code = ExtractCode(initialValue);
            if (string.IsNullOrWhiteSpace(code)) return;
            txtKeyword.EditValueChanged -= txtKeyword_EditValueChanged;
            txtKeyword.Text = code;
            txtKeyword.EditValueChanged += txtKeyword_EditValueChanged;
            string kwUnsigned = Inventec.Common.String.Convert.UnSignVNese2(code).ToLowerInvariant();
            filteredRows = allRows.Where(o =>
            {
                string c = o.MEDI_ORG_CODE ?? string.Empty;
                string nameUnsigned = o.MEDI_ORG_NAME_UNSIGNED ?? string.Empty;
                return c.IndexOf(code, StringComparison.OrdinalIgnoreCase) >= 0
                    || nameUnsigned.ToLowerInvariant().Contains(kwUnsigned);
            }).ToList();
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
        /// Tach phan code tu chuoi dau vao. Vi du:
        ///   "C.01234" -> "01234"
        ///   "X.01234" -> "01234"
        ///   "01234"   -> "01234"
        /// </summary>
        private static string ExtractCode(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            int dot = raw.IndexOf('.');
            return (dot > 0 && dot < raw.Length - 1) ? raw.Substring(dot + 1) : raw;
        }

        /// <summary>
        /// Tra ve prefix tu chuoi dau vao, default "C" neu khong co.
        /// "X.01234" -> "X"; "01234" -> "C"; ""/null -> "C"
        /// </summary>
        private string GetEffectivePrefix()
        {
            if (string.IsNullOrWhiteSpace(initialValue)) return DefaultPrefix;
            int dot = initialValue.IndexOf('.');
            if (dot > 0)
            {
                string p = initialValue.Substring(0, dot);
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

        private void gridViewMediOrg_DoubleClick(object sender, EventArgs e)
        {
            CommitSelection();
        }

        private void gridViewMediOrg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
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
                var row = GetFocusedRow();
                if (row == null || string.IsNullOrEmpty(row.MEDI_ORG_CODE))
                {
                    return;
                }
                // Khong block do dai o day - de form goi (TextEdit "CSKCB chuyen")
                // hien canh bao realtime + chan luu neu vuot 10 ky tu.
                this.SelectedValue = GetEffectivePrefix() + "." + row.MEDI_ORG_CODE;
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
