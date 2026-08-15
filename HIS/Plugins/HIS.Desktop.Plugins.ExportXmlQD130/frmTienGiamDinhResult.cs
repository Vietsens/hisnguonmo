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
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Cua so ket qua kiem tra ho so tren he thong tien giam dinh.
    /// Luoi tren: moi ho so mot dong. Chon mot dong thi luoi duoi hien chi tiet loi
    /// cua ho so do theo ba nhom.
    ///
    /// Tham chieu: PTTK_53286 muc B.4.2
    /// </summary>
    public partial class frmTienGiamDinhResult : HIS.Desktop.Utility.FormBase
    {
        /// <summary>Ten trang tinh cua tep xuat ra</summary>
        private const string EXPORT_SHEET_NAME = "DanhSachLoi";

        /// <summary>Do rong cot mo ta loi (theo so ky tu) - mo ta thuong rat dai nen phai chan lai</summary>
        private const double EXPORT_DESCRIPTION_COLUMN_WIDTH = 80;

        //Thu tu cot trong tep xuat ra
        private const int EXPORT_COL_STT = 0;
        private const int EXPORT_COL_TREATMENT_CODE = 1;
        private const int EXPORT_COL_PATIENT_NAME = 2;
        private const int EXPORT_COL_RESULT_STATUS = 3;
        private const int EXPORT_COL_GROUP = 4;
        private const int EXPORT_COL_SEVERITY = 5;
        private const int EXPORT_COL_ERROR_CODE = 6;
        private const int EXPORT_COL_DESCRIPTION = 7;

        private readonly List<TienGiamDinhResultADO> results;

        public frmTienGiamDinhResult(List<TienGiamDinhResultADO> results)
        {
            InitializeComponent();
            this.results = results ?? new List<TienGiamDinhResultADO>();
            this.SetIcon();
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
                LogSystem.Error(ex);
            }
        }

        private void frmTienGiamDinhResult_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                FillDataToGrid();
                this.gridViewSummary.FocusedRowChanged += gridViewSummary_FocusedRowChanged;
                //Hien chi tiet cua dong dau tien ngay khi mo
                FillDetailOfFocusedRow();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Dat ten hien thi theo ngon ngu dang dung</summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.ExportXmlQD130.Resources.Lang",
                    typeof(frmTienGiamDinhResult).Assembly);

                this.Text = GetLangValue("frmTienGiamDinhResult.Text", this.Text);
                this.btnClose.Text = GetLangValue("frmTienGiamDinhResult.btnClose.Text", this.btnClose.Text);
                this.btnExportError.Text = GetLangValue("frmTienGiamDinhResult.btnExportError.Text", this.btnExportError.Text);

                this.gridColTreatmentCode.Caption = GetLangValue("frmTienGiamDinhResult.gridColTreatmentCode.Caption", this.gridColTreatmentCode.Caption);
                this.gridColPatientName.Caption = GetLangValue("frmTienGiamDinhResult.gridColPatientName.Caption", this.gridColPatientName.Caption);
                this.gridColErrorCount.Caption = GetLangValue("frmTienGiamDinhResult.gridColErrorCount.Caption", this.gridColErrorCount.Caption);
                this.gridColStatusName.Caption = GetLangValue("frmTienGiamDinhResult.gridColStatusName.Caption", this.gridColStatusName.Caption);

                this.gridColGroupName.Caption = GetLangValue("frmTienGiamDinhResult.gridColGroupName.Caption", this.gridColGroupName.Caption);
                this.gridColSeverityName.Caption = GetLangValue("frmTienGiamDinhResult.gridColSeverityName.Caption", this.gridColSeverityName.Caption);
                this.gridColErrorCode.Caption = GetLangValue("frmTienGiamDinhResult.gridColErrorCode.Caption", this.gridColErrorCode.Caption);
                this.gridColDescription.Caption = GetLangValue("frmTienGiamDinhResult.gridColDescription.Caption", this.gridColDescription.Caption);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Doc chuoi hien thi, khong doc duoc thi giu nguyen gia tri dang co</summary>
        private string GetLangValue(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key,
                    Resources.ResourceLanguageManager.LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return defaultValue;
        }

        private void FillDataToGrid()
        {
            try
            {
                //Dua ho so co loi len dau de nguoi dung thay ngay cai can sua
                List<TienGiamDinhResultADO> ordered = this.results
                    .OrderByDescending(o => o.Status == EnumTienGiamDinhStatus.Critical)
                    .ThenByDescending(o => o.Status == EnumTienGiamDinhStatus.Warning)
                    .ThenByDescending(o => o.TotalErrorCount)
                    .ToList();

                this.gridViewSummary.BeginUpdate();
                try
                {
                    this.gridControlSummary.DataSource = ordered;
                }
                finally
                {
                    this.gridViewSummary.EndUpdate();
                }

                this.lblSummary.Text = BuildSummaryText();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Dong tong ket duoi cung: bao nhieu ho so, bao nhieu bi chan</summary>
        private string BuildSummaryText()
        {
            try
            {
                int total = this.results.Count;
                int critical = this.results.Count(o => o.Status == EnumTienGiamDinhStatus.Critical);
                int warning = this.results.Count(o => o.Status == EnumTienGiamDinhStatus.Warning);
                int failed = this.results.Count(o => o.Status == EnumTienGiamDinhStatus.CheckFailed);

                return string.Format(Resources.ResourceMessageLang.TienGiamDinhTongKetKetQua,
                    total, critical, warning, failed);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return "";
        }

        private void gridViewSummary_FocusedRowChanged(object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                FillDetailOfFocusedRow();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Hien chi tiet loi cua ho so dang chon o luoi tren</summary>
        private void FillDetailOfFocusedRow()
        {
            try
            {
                TienGiamDinhResultADO current =
                    this.gridViewSummary.GetFocusedRow() as TienGiamDinhResultADO;

                this.gridViewDetail.BeginUpdate();
                try
                {
                    this.gridControlDetail.DataSource = current == null
                        ? new List<TienGiamDinhErrorADO>()
                        : current.Errors;
                }
                finally
                {
                    this.gridViewDetail.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #region Xuat danh sach loi ra tep Excel

        /// <summary>
        /// Ghi danh sach ho so con loi ra tep Excel.
        /// Chi ghi lai ket qua dang xem tren luoi, KHONG goi lai he tien giam dinh - nguoi dung
        /// da doc ket qua roi moi bam Xuat thi khong duoc phat sinh them luot goi nao.
        /// </summary>
        private void btnExportError_Click(object sender, EventArgs e)
        {
            try
            {
                //Chi lay ho so that su con loi - day la phan nguoi dung phai di sua.
                //Ho so khong kiem tra duoc khong dua vao vi chua biet no co loi hay khong.
                List<TienGiamDinhResultADO> errorResults = this.results
                    .Where(o => o.Status == EnumTienGiamDinhStatus.Critical
                        || o.Status == EnumTienGiamDinhStatus.Warning)
                    .OrderByDescending(o => o.Status == EnumTienGiamDinhStatus.Critical)
                    .ThenByDescending(o => o.TotalErrorCount)
                    .ToList();

                if (errorResults.Count == 0)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessageLang.TienGiamDinhXuatKhongCoDuLieu,
                        Resources.ResourceMessageLang.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string filePath = ChooseExportFilePath();
                if (string.IsNullOrEmpty(filePath))
                {
                    //Nguoi dung bam Huy o hop thoai chon duong dan
                    return;
                }

                int errorLineCount = WriteErrorListToFile(filePath, errorResults);
                if (errorLineCount < 0)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessageLang.TienGiamDinhXuatThatBai,
                        Resources.ResourceMessageLang.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LogSystem.Info("TienGiamDinh - Da xuat danh sach loi: " + errorLineCount
                    + " dong loi cua " + errorResults.Count + " ho so. Tep: " + filePath);

                XtraMessageBox.Show(
                    string.Format(Resources.ResourceMessageLang.TienGiamDinhXuatThanhCong,
                        errorLineCount, errorResults.Count, filePath),
                    Resources.ResourceMessageLang.ThongBao,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Hoi noi luu tep. Tra ve rong khi nguoi dung huy.</summary>
        private string ChooseExportFilePath()
        {
            try
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                    dialog.DefaultExt = "xlsx";
                    dialog.AddExtension = true;
                    dialog.OverwritePrompt = true;
                    dialog.FileName = string.Format(
                        Resources.ResourceMessageLang.TienGiamDinhXuatTenTepMacDinh,
                        DateTime.Now.ToString("yyyyMMddHHmmss"));

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        return dialog.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return "";
        }

        /// <summary>
        /// Ghi tep: moi dong loi mot hang, ho so nhieu loi thi lap lai ma dieu tri o tung hang
        /// de nguoi doc loc va sap xep duoc bang Excel.
        /// Tra ve so dong loi da ghi, hoac -1 khi khong ghi duoc tep.
        /// </summary>
        private int WriteErrorListToFile(string filePath, List<TienGiamDinhResultADO> errorResults)
        {
            try
            {
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();
                Aspose.Cells.Worksheet sheet = workbook.Worksheets[0];
                sheet.Name = EXPORT_SHEET_NAME;

                WriteExportHeaderRow(sheet);

                int row = 1;
                int errorLineCount = 0;

                foreach (TienGiamDinhResultADO result in errorResults)
                {
                    foreach (TienGiamDinhErrorADO error in result.Errors)
                    {
                        errorLineCount++;
                        WriteExportErrorRow(sheet, row, errorLineCount, result, error);
                        row++;
                    }

                    //Ho so bi cat bot danh sach loi thi phai ghi ro ra tep, neu khong nguoi doc
                    //tuong da co du loi va sua thieu (QT-06).
                    if (result.IsTruncated)
                    {
                        sheet.Cells[row, EXPORT_COL_TREATMENT_CODE].PutValue(result.TreatmentCode ?? "");
                        sheet.Cells[row, EXPORT_COL_PATIENT_NAME].PutValue(result.PatientName ?? "");
                        sheet.Cells[row, EXPORT_COL_DESCRIPTION].PutValue(
                            Resources.ResourceMessageLang.TienGiamDinhXuatGhiChuCatBot);
                        row++;
                    }
                }

                AutoFitExportColumns(sheet);
                workbook.Save(filePath, Aspose.Cells.SaveFormat.Xlsx);
                return errorLineCount;
            }
            catch (Exception ex)
            {
                LogSystem.Error("TienGiamDinh - Khong ghi duoc tep danh sach loi: " + filePath, ex);
            }
            return -1;
        }

        /// <summary>
        /// Dong tieu de. Lay lai dung chu dang hien tren luoi de nguoi doc tep khong phai doi chieu;
        /// rieng hai cot muc do phai dat ten rieng vi tren luoi ca hai deu ghi "Muc do".
        /// </summary>
        private void WriteExportHeaderRow(Aspose.Cells.Worksheet sheet)
        {
            sheet.Cells[0, EXPORT_COL_STT].PutValue(
                GetLangValue("frmTienGiamDinhResult.export.colStt", "STT"));
            sheet.Cells[0, EXPORT_COL_TREATMENT_CODE].PutValue(this.gridColTreatmentCode.Caption);
            sheet.Cells[0, EXPORT_COL_PATIENT_NAME].PutValue(this.gridColPatientName.Caption);
            sheet.Cells[0, EXPORT_COL_RESULT_STATUS].PutValue(
                GetLangValue("frmTienGiamDinhResult.export.colResultStatus", "Kết quả hồ sơ"));
            sheet.Cells[0, EXPORT_COL_GROUP].PutValue(this.gridColGroupName.Caption);
            sheet.Cells[0, EXPORT_COL_SEVERITY].PutValue(
                GetLangValue("frmTienGiamDinhResult.export.colSeverity", "Mức độ lỗi"));
            sheet.Cells[0, EXPORT_COL_ERROR_CODE].PutValue(this.gridColErrorCode.Caption);
            sheet.Cells[0, EXPORT_COL_DESCRIPTION].PutValue(this.gridColDescription.Caption);

            try
            {
                Aspose.Cells.Style headerStyle = sheet.Cells[0, EXPORT_COL_STT].GetStyle();
                headerStyle.Font.IsBold = true;

                for (int col = EXPORT_COL_STT; col <= EXPORT_COL_DESCRIPTION; col++)
                {
                    sheet.Cells[0, col].SetStyle(headerStyle);
                }

                //Danh sach loi thuong dai - giu dong tieu de khi cuon
                sheet.FreezePanes(1, 0, 1, 0);
            }
            catch (Exception ex)
            {
                //To dam hong thi tep van dung du lieu, khong can chan viec xuat
                LogSystem.Warn(ex);
            }
        }

        private void WriteExportErrorRow(Aspose.Cells.Worksheet sheet, int row, int stt,
            TienGiamDinhResultADO result, TienGiamDinhErrorADO error)
        {
            sheet.Cells[row, EXPORT_COL_STT].PutValue(stt);
            sheet.Cells[row, EXPORT_COL_TREATMENT_CODE].PutValue(result.TreatmentCode ?? "");
            sheet.Cells[row, EXPORT_COL_PATIENT_NAME].PutValue(result.PatientName ?? "");
            sheet.Cells[row, EXPORT_COL_RESULT_STATUS].PutValue(result.StatusName ?? "");
            sheet.Cells[row, EXPORT_COL_GROUP].PutValue(error.GroupName ?? "");
            sheet.Cells[row, EXPORT_COL_SEVERITY].PutValue(error.SeverityName ?? "");
            sheet.Cells[row, EXPORT_COL_ERROR_CODE].PutValue(error.Code ?? "");
            sheet.Cells[row, EXPORT_COL_DESCRIPTION].PutValue(error.Description ?? "");
        }

        /// <summary>Gian rong cot cho vua chu, rieng cot mo ta thi chan lai keo khong tran qua man hinh</summary>
        private void AutoFitExportColumns(Aspose.Cells.Worksheet sheet)
        {
            try
            {
                sheet.AutoFitColumns();
                sheet.Cells.SetColumnWidth(EXPORT_COL_DESCRIPTION, EXPORT_DESCRIPTION_COLUMN_WIDTH);
            }
            catch (Exception ex)
            {
                //Gian cot hong thi tep van doc duoc, khong can chan viec xuat
                LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
