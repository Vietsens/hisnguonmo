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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Cua so ket qua soat loi MDInsight, gom BA phan tu tren xuong:
    ///
    ///  1. Danh sach ho so   - moi ho so mot dong, kem cot Ly do (chi co noi dung khi trang thai 4 hoac 5)
    ///  2. Chi tiet loi      - hien khi bam vao mot ho so; moi loi mot dong, xep loi nghiem trong len truoc
    ///  3. Dien giai day du  - hien khi bam vao mot dong loi. CHI CO trong phien vua tra, khong luu CSDL
    ///
    /// ⚠️ Phan dien giai day du chua HO TEN, MA BENH NHAN va CHAN DOAN.
    /// Duoc hien len man hinh, nhung TUYET DOI khong ghi ra tep nhat ky (quy tac QT-28).
    /// Nut xuat tep phai canh bao truoc khi xuat (quy tac QT-29).
    ///
    /// Giao dien dung luc chay thay vi Designer de khong phat sinh them tep .resx phai dich
    /// cho mot cua so chi hien khi vien da dau noi.
    ///
    /// Tham chieu: PTTK muc B.4.1.1 - "Bo sung cua so ket qua".
    /// </summary>
    public class frmMdInsightResult : XtraForm
    {
        private readonly List<MdInsightResultADO> results;
        private readonly UCExportXml owner;

        private GridControl grdTreatment;
        private GridView gvTreatment;
        private GridControl grdError;
        private GridView gvError;
        private MemoEdit txtFullDescription;
        private SimpleButton btnExport;
        private SimpleButton btnClose;

        public frmMdInsightResult(List<MdInsightResultADO> results, UCExportXml owner)
        {
            this.results = results ?? new List<MdInsightResultADO>();
            this.owner = owner;

            BuildLayout();
            SetIcon();
            BindTreatments();
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #region Dung giao dien

        private void BuildLayout()
        {
            this.SuspendLayout();
            try
            {
                this.Text = Resources.ResourceMessageLang.MdInsightTieuDeKetQua;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimizeBox = false;
                this.MaximizeBox = true;
                this.ClientSize = new Size(1000, 640);
                this.MinimumSize = new Size(760, 480);

                PanelControl footer = new PanelControl();
                footer.Dock = DockStyle.Bottom;
                footer.Height = 40;
                footer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

                this.btnExport = new SimpleButton();
                this.btnExport.Text = Resources.ResourceMessageLang.MdInsightNutXuatDanhSach;
                this.btnExport.Size = new Size(160, 26);
                this.btnExport.Location = new Point(8, 7);
                this.btnExport.Click += btnExport_Click;

                this.btnClose = new SimpleButton();
                this.btnClose.Text = Resources.ResourceMessageLang.MdInsightNutDong;
                this.btnClose.Size = new Size(100, 26);
                this.btnClose.Location = new Point(176, 7);
                this.btnClose.Click += delegate { this.Close(); };

                footer.Controls.Add(this.btnExport);
                footer.Controls.Add(this.btnClose);

                this.grdTreatment = new GridControl();
                this.gvTreatment = new GridView(this.grdTreatment);
                this.grdTreatment.MainView = this.gvTreatment;
                this.grdTreatment.Dock = DockStyle.Fill;
                BuildTreatmentColumns();

                this.grdError = new GridControl();
                this.gvError = new GridView(this.grdError);
                this.grdError.MainView = this.gvError;
                this.grdError.Dock = DockStyle.Fill;
                BuildErrorColumns();

                this.txtFullDescription = new MemoEdit();
                this.txtFullDescription.Dock = DockStyle.Fill;
                this.txtFullDescription.Properties.ReadOnly = true;
                this.txtFullDescription.Properties.ScrollBars = ScrollBars.Vertical;

                GroupControl grpTreatment = WrapInGroup(
                    this.grdTreatment, Resources.ResourceMessageLang.MdInsightNhomDanhSachHoSo);
                GroupControl grpError = WrapInGroup(
                    this.grdError, Resources.ResourceMessageLang.MdInsightNhomChiTietLoi);
                GroupControl grpDescription = WrapInGroup(
                    this.txtFullDescription, Resources.ResourceMessageLang.MdInsightNhomDienGiai);

                SplitContainerControl splitBottom = new SplitContainerControl();
                splitBottom.Dock = DockStyle.Fill;
                splitBottom.Horizontal = false;
                splitBottom.Panel1.Controls.Add(grpError);
                splitBottom.Panel2.Controls.Add(grpDescription);
                splitBottom.SplitterPosition = 220;

                SplitContainerControl splitMain = new SplitContainerControl();
                splitMain.Dock = DockStyle.Fill;
                splitMain.Horizontal = false;
                splitMain.Panel1.Controls.Add(grpTreatment);
                splitMain.Panel2.Controls.Add(splitBottom);
                splitMain.SplitterPosition = 200;

                this.Controls.Add(splitMain);
                this.Controls.Add(footer);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            finally
            {
                this.ResumeLayout(false);
            }
        }

        private static GroupControl WrapInGroup(Control inner, string caption)
        {
            GroupControl group = new GroupControl();
            group.Text = caption;
            group.Dock = DockStyle.Fill;
            group.Controls.Add(inner);
            return group;
        }

        private void BuildTreatmentColumns()
        {
            this.gvTreatment.OptionsBehavior.Editable = false;
            this.gvTreatment.OptionsView.ShowGroupPanel = false;
            this.gvTreatment.OptionsView.ColumnAutoWidth = false;
            this.gvTreatment.OptionsView.AnimationType = DevExpress.XtraGrid.Views.Base.GridAnimationType.NeverAnimate;

            AddColumn(this.gvTreatment, "TreatmentCode", Resources.ResourceMessageLang.MdInsightCotMaDieuTri, 130);
            AddColumn(this.gvTreatment, "PatientName", Resources.ResourceMessageLang.MdInsightCotTenBenhNhan, 200);
            AddColumn(this.gvTreatment, "Status", Resources.ResourceMessageLang.MdInsightCotKetQua, 150);
            AddColumn(this.gvTreatment, "ErrorNum", Resources.ResourceMessageLang.MdInsightCotSoLoi, 80);
            AddColumn(this.gvTreatment, "CriticalNum", Resources.ResourceMessageLang.MdInsightCotSoLoiNghiemTrong, 120);
            AddColumn(this.gvTreatment, "Reason", Resources.ResourceMessageLang.MdInsightCotLyDo, 300);

            this.gvTreatment.CustomColumnDisplayText += gvTreatment_CustomColumnDisplayText;
            this.gvTreatment.FocusedRowChanged += gvTreatment_FocusedRowChanged;
        }

        private void BuildErrorColumns()
        {
            this.gvError.OptionsBehavior.Editable = false;
            this.gvError.OptionsView.ShowGroupPanel = false;
            this.gvError.OptionsView.ColumnAutoWidth = false;
            this.gvError.OptionsView.AnimationType = DevExpress.XtraGrid.Views.Base.GridAnimationType.NeverAnimate;

            AddColumn(this.gvError, "IsCritical", Resources.ResourceMessageLang.MdInsightCotMucDo, 110);
            AddColumn(this.gvError, "FileName", Resources.ResourceMessageLang.MdInsightCotTepThanhPhan, 110);
            AddColumn(this.gvError, "TagName", Resources.ResourceMessageLang.MdInsightCotTheDuLieu, 150);
            AddColumn(this.gvError, "RowIndex", Resources.ResourceMessageLang.MdInsightCotDongThuMay, 80);
            AddColumn(this.gvError, "CurrentValue", Resources.ResourceMessageLang.MdInsightCotGiaTriHienTai, 160);
            AddColumn(this.gvError, "ErrorContent", Resources.ResourceMessageLang.MdInsightCotNoiDungLoi, 320);

            this.gvError.CustomColumnDisplayText += gvError_CustomColumnDisplayText;
            this.gvError.RowCellStyle += gvError_RowCellStyle;
            this.gvError.FocusedRowChanged += gvError_FocusedRowChanged;
        }

        private static GridColumn AddColumn(GridView view, string fieldName, string caption, int width)
        {
            GridColumn column = view.Columns.AddField(fieldName);
            column.Caption = caption;
            column.Width = width;
            column.Visible = true;
            column.VisibleIndex = view.Columns.Count;
            column.OptionsColumn.AllowEdit = false;
            return column;
        }

        #endregion

        #region Nap du lieu

        private void BindTreatments()
        {
            try
            {
                this.gvTreatment.BeginUpdate();
                try
                {
                    this.grdTreatment.DataSource = this.results;
                }
                finally
                {
                    this.gvTreatment.EndUpdate();
                }

                if (this.results.Count > 0)
                {
                    this.gvTreatment.FocusedRowHandle = 0;
                    BindErrorsOfFocusedTreatment();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gvTreatment_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                BindErrorsOfFocusedTreatment();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Cac dong loi duoc NHOM VA SAP XEP theo nhom quy dinh lay tu tien to cua noi dung loi,
        /// trong moi nhom xep loi nghiem trong len truoc - PTTK muc B.4.1.1.
        /// </summary>
        private void BindErrorsOfFocusedTreatment()
        {
            MdInsightResultADO current = this.gvTreatment.GetFocusedRow() as MdInsightResultADO;

            List<MdInsightErrorADO> errors = current == null || current.Errors == null
                ? new List<MdInsightErrorADO>()
                : current.Errors
                    .OrderBy(o => GetErrorGroupKey(o))
                    .ThenByDescending(o => o.IsCritical)
                    .ToList();

            this.gvError.BeginUpdate();
            try
            {
                this.grdError.DataSource = errors;
            }
            finally
            {
                this.gvError.EndUpdate();
            }

            this.txtFullDescription.Text = "";
            ShowTruncatedNoteIfAny(current);

            if (errors.Count > 0)
            {
                this.gvError.FocusedRowHandle = 0;
                ShowFullDescriptionOfFocusedError();
            }
        }

        /// <summary>Tien to cua noi dung loi dong vai tro nhom quy dinh - xem PTTK muc A.2.5 diem 9</summary>
        private static string GetErrorGroupKey(MdInsightErrorADO error)
        {
            if (error == null || string.IsNullOrWhiteSpace(error.ErrorContent))
            {
                return "";
            }

            int separator = error.ErrorContent.IndexOf(':');
            return separator > 0 ? error.ErrorContent.Substring(0, separator).Trim() : "";
        }

        private void ShowTruncatedNoteIfAny(MdInsightResultADO current)
        {
            if (current == null || !current.IsTruncated)
            {
                return;
            }

            this.txtFullDescription.Text = string.Format(
                Resources.ResourceMessageLang.MdInsightConLoiChuaLuu, current.TruncatedCount);
        }

        private void gvError_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                ShowFullDescriptionOfFocusedError();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Phan dien giai day du nam NGOAI dac ta chinh thuc cua nha cung cap - ho co the bo bat cu luc nao.
        /// Khong co thi AN HAN phan nay, cua so van hoat dong du voi cac truong trong dac ta.
        /// Khong duoc de o trong tron, va khong duoc coi thieu truong la loi.
        /// </summary>
        private void ShowFullDescriptionOfFocusedError()
        {
            MdInsightErrorADO error = this.gvError.GetFocusedRow() as MdInsightErrorADO;

            if (error == null)
            {
                this.txtFullDescription.Text = "";
                return;
            }

            if (!string.IsNullOrWhiteSpace(error.FullDescription))
            {
                this.txtFullDescription.Text = error.FullDescription;
                return;
            }

            //Mo lai cua so sau khi dong man hinh thi phan nay trong - goi y tra lai
            this.txtFullDescription.Text = Resources.ResourceMessageLang.MdInsightKhongCoDienGiai;
        }

        private void gvTreatment_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Status")
                {
                    EnumXmlPrecheckStatus status = (EnumXmlPrecheckStatus)e.Value;
                    e.DisplayText = this.owner == null ? "" : this.owner.GetMdInsightStatusName((short)status);
                    return;
                }

                if (e.Column.FieldName == "ErrorNum" || e.Column.FieldName == "CriticalNum")
                {
                    long? value = e.Value as long?;
                    e.DisplayText = value.HasValue && value.Value > 0 ? value.Value.ToString() : "";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gvError_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column.FieldName != "IsCritical")
                {
                    return;
                }

                bool isCritical = e.Value is bool && (bool)e.Value;
                e.DisplayText = isCritical
                    ? Resources.ResourceMessageLang.MdInsightMucDoNghiemTrong
                    : Resources.ResourceMessageLang.MdInsightMucDoCanhBao;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gvError_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                MdInsightErrorADO error = this.gvError.GetRow(e.RowHandle) as MdInsightErrorADO;
                if (error != null && error.IsCritical)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Xuat danh sach loi ra tep

        /// <summary>
        /// Xuat danh sach loi de gui cho nguoi sua ho so.
        /// BAT BUOC canh bao truoc: tep nay chua thong tin benh nhan - quy tac QT-29.
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.results.All(o => o.Errors == null || o.Errors.Count == 0))
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessageLang.MdInsightXuatKhongCoDuLieu,
                        Resources.ResourceMessageLang.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (XtraMessageBox.Show(
                        Resources.ResourceMessageLang.MdInsightCanhBaoDuLieuBenhNhan,
                        Resources.ResourceMessageLang.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }

                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "CSV (*.csv)|*.csv";
                    dialog.FileName = "MDInsight_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";

                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    //UTF-8 co dau thu tu byte de Excel mo dung tieng Viet
                    File.WriteAllText(dialog.FileName, BuildCsv(), new UTF8Encoding(true));

                    XtraMessageBox.Show(
                        string.Format(Resources.ResourceMessageLang.MdInsightXuatThanhCong, dialog.FileName),
                        Resources.ResourceMessageLang.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                XtraMessageBox.Show(
                    Resources.ResourceMessageLang.MdInsightXuatThatBai,
                    Resources.ResourceMessageLang.ThongBao,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildCsv()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(string.Join(";", new string[]
            {
                Resources.ResourceMessageLang.MdInsightCotMaDieuTri,
                Resources.ResourceMessageLang.MdInsightCotTenBenhNhan,
                Resources.ResourceMessageLang.MdInsightCotMucDo,
                Resources.ResourceMessageLang.MdInsightCotTepThanhPhan,
                Resources.ResourceMessageLang.MdInsightCotTheDuLieu,
                Resources.ResourceMessageLang.MdInsightCotDongThuMay,
                Resources.ResourceMessageLang.MdInsightCotGiaTriHienTai,
                Resources.ResourceMessageLang.MdInsightCotNoiDungLoi
            }));

            foreach (MdInsightResultADO result in this.results)
            {
                if (result.Errors == null || result.Errors.Count == 0)
                {
                    continue;
                }

                foreach (MdInsightErrorADO error in result.Errors)
                {
                    builder.AppendLine(string.Join(";", new string[]
                    {
                        Csv(result.TreatmentCode),
                        Csv(result.PatientName),
                        Csv(error.IsCritical
                            ? Resources.ResourceMessageLang.MdInsightMucDoNghiemTrong
                            : Resources.ResourceMessageLang.MdInsightMucDoCanhBao),
                        Csv(error.FileName),
                        Csv(error.TagName),
                        Csv(error.RowIndex),
                        Csv(error.CurrentValue),
                        Csv(error.ErrorContent)
                    }));
                }

                if (result.IsTruncated)
                {
                    builder.AppendLine(Csv(result.TreatmentCode) + ";"
                        + Csv(string.Format(Resources.ResourceMessageLang.MdInsightConLoiChuaLuu, result.TruncatedCount)));
                }
            }

            return builder.ToString();
        }

        /// <summary>Bo ky tu ngan cach va ky tu xuong dong de tep khong bi vo cot</summary>
        private static string Csv(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            return value.Replace(';', ',').Replace('\r', ' ').Replace('\n', ' ').Trim();
        }

        #endregion
    }
}
