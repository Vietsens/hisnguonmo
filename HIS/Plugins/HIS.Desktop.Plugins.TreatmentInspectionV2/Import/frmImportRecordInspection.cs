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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TreatmentInspectionV2.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentInspectionV2
{
    /// <summary>
    /// Imports the list of treatments requested for appraisal by the health insurance body.
    /// The Excel file carries a single column: the treatment code. Every other value shown in
    /// the grid is resolved from the treatment record so the operator can verify the batch.
    /// Rows are never edited here — the user fixes the Excel file and loads it again.
    /// </summary>
    public partial class frmImportRecordInspection : FormBase
    {
        #region Declare

        /// <summary>Maximum number of rows accepted from one file.</summary>
        const int MAX_ROW_PER_FILE = 5000;

        /// <summary>Excel template used to prepare the inspection list.</summary>
        const string IMPORT_TEMPLATE_FILE_NAME = "IMPORT_RECORD_INSPECTION.xlsx";

        /// <summary>Length a treatment code is normalised to.</summary>
        const int TREATMENT_CODE_LENGTH = 12;

        /// <summary>
        /// Height written on every used row before the file is handed to the reader.
        /// Any value above zero satisfies the reader; 20 keeps the sheet readable if a user
        /// happens to open the normalised copy.
        /// </summary>
        const int NORMALIZED_ROW_HEIGHT = 20;

        Inventec.Desktop.Common.Modules.Module currentModule;

        /// <summary>Every row read from the file, including the invalid ones.</summary>
        List<RecordInspectionImportADO> listImport = new List<RecordInspectionImportADO>();

        string currentFileName = "";
        bool isShowingErrorOnly = false;

        /// <summary>Embedded error icon — the same one the other import screens use.</summary>
        const string ROW_ERROR_ICON_RESOURCE =
            "HIS.Desktop.Plugins.TreatmentInspectionV2.Resources.row_error.png";

        #region Tooltip cache

        // The tooltip controller fires on every mouse move. Returning a freshly built
        // ToolTipControlInfo each time makes the tooltip close and reopen, which reads as
        // flickering, so the last one is reused while the pointer stays on the same cell.
        int lastRowHandle = -1;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;

        #endregion

        /// <summary>True when at least one batch was written, so the caller refreshes its list.</summary>
        public bool HasSaved { get; private set; }

        #endregion

        #region Constructor

        public frmImportRecordInspection()
        {
            InitializeComponent();
            SetIcon();
        }

        public frmImportRecordInspection(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                SetIcon();
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
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Load

        private void frmImportRecordInspection_Load(object sender, EventArgs e)
        {
            try
            {
                // Caption first: LoadRowErrorImage reads the button tooltip from the resources.
                SetCaptionByLanguageKey();
                LoadRowErrorImage();
                if (this.currentModule != null && !string.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
                RefreshButtonState();
                UpdateSummary();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.TreatmentInspectionV2.Resources.Lang",
                    typeof(frmImportRecordInspection).Assembly);

                this.btnDownloadTemplate.Text = GetLang("frmImportRecordInspection.btnDownloadTemplate.Text");
                this.btnChooseFile.Text = GetLang("frmImportRecordInspection.btnChooseFile.Text");
                this.btnShowLineError.Text = GetLang("frmImportRecordInspection.btnShowLineError.Text");
                this.btnSave.Text = GetLang("frmImportRecordInspection.btnSave.Text");

                this.gcStt.Caption = GetLang("frmImportRecordInspection.gcStt.Caption");
                this.gcTreatmentCode.Caption = GetLang("frmImportRecordInspection.gcTreatmentCode.Caption");
                this.gcPatientCode.Caption = GetLang("frmImportRecordInspection.gcPatientCode.Caption");
                this.gcPatientName.Caption = GetLang("frmImportRecordInspection.gcPatientName.Caption");
                this.gcInTime.Caption = GetLang("frmImportRecordInspection.gcInTime.Caption");
                this.gcOutTime.Caption = GetLang("frmImportRecordInspection.gcOutTime.Caption");
                this.gcEndDepartment.Caption = GetLang("frmImportRecordInspection.gcEndDepartment.Caption");
                this.gcIcd.Caption = GetLang("frmImportRecordInspection.gcIcd.Caption");
                this.gcNote.Caption = GetLang("frmImportRecordInspection.gcNote.Caption");
                this.gcStatus.Caption = GetLang("frmImportRecordInspection.gcStatus.Caption");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLang(string key)
        {
            return Inventec.Common.Resource.Get.Value(
                key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        }

        /// <summary>
        /// Puts the embedded error icon on the glyph button and gives the button its tooltip,
        /// mirroring the error button of the other import screens.
        /// </summary>
        private void LoadRowErrorImage()
        {
            try
            {
                if (btnGError.Buttons.Count == 0) return;

                using (System.IO.Stream stream = typeof(frmImportRecordInspection).Assembly
                    .GetManifestResourceStream(ROW_ERROR_ICON_RESOURCE))
                {
                    if (stream != null)
                    {
                        btnGError.Buttons[0].Image = System.Drawing.Image.FromStream(stream);
                        btnGError.Buttons[0].ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn(
                            "frmImportRecordInspection: khong doc duoc resource " + ROW_ERROR_ICON_RESOURCE);
                    }
                }

                btnGError.Buttons[0].ToolTip = GetLang("frmImportRecordInspection.btnGError.ToolTip");

                if (btnGWarning.Buttons.Count > 0)
                {
                    // No warning glyph ships with DevExpress 15.2, so the notice uses the info icon
                    // of the gallery — visibly different from the blocking error icon.
                    btnGWarning.Buttons[0].Image = DevExpress.Images.ImageResourceCache.Default.GetImage(
                        "images/support/info_16x16.png");
                    btnGWarning.Buttons[0].ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
                    btnGWarning.Buttons[0].ToolTip = GetLang("frmImportRecordInspection.btnGWarning.ToolTip");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Download template

        private void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                string sourceFile = System.IO.Path.Combine(
                    Application.StartupPath + "\\Tmp\\Imp\\", IMPORT_TEMPLATE_FILE_NAME);

                if (!System.IO.File.Exists(sourceFile))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        string.Format(Resources.ResourceMessage.KhongTimThayFileMau, IMPORT_TEMPLATE_FILE_NAME),
                        Resources.ResourceMessage.Thongbao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.FileName = System.IO.Path.GetFileNameWithoutExtension(IMPORT_TEMPLATE_FILE_NAME);
                    sfd.DefaultExt = "xlsx";
                    sfd.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    sfd.FilterIndex = 1;
                    sfd.RestoreDirectory = true;

                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    System.IO.File.Copy(sourceFile, sfd.FileName, true);

                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.BanCoMuonMoFileNgay,
                        Resources.ResourceMessage.Thongbao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Choose file and validate

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Multiselect = false;
                    ofd.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    if (ofd.ShowDialog() != DialogResult.OK) return;

                    WaitingManager.Show();

                    // A newly loaded file replaces the previous content entirely.
                    this.listImport = new List<RecordInspectionImportADO>();
                    this.currentFileName = System.IO.Path.GetFileName(ofd.FileName);
                    this.isShowingErrorOnly = false;

                    string fileToRead = NormalizeRowHeight(ofd.FileName);
                    try
                    {
                        var excel = new Inventec.Common.ExcelImport.Import();
                        if (!excel.ReadFileExcel(fileToRead))
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(
                                Resources.ResourceMessage.KhongDocDuocFile,
                                Resources.ResourceMessage.Thongbao,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        ReadAndBind(excel);
                    }
                    finally
                    {
                        DeleteTempFile(fileToRead, ofd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Rewrites the workbook with an explicit height on every used row and returns the path to
        /// use for reading.
        ///
        /// Inventec.Common.ExcelImport.Import treats Cells[0,0].RowHeight == 0 as "no usable
        /// worksheet" and then returns an empty list without throwing. Spreadsheet applications
        /// other than Microsoft Excel — WPS Office in particular — save the file without row
        /// height information, so a file that visibly holds data reads as empty. Normalising the
        /// heights into a temporary copy keeps the shared reader while accepting those files.
        ///
        /// Returns the original path unchanged when normalisation is not possible, so a failure
        /// here never blocks an import that would otherwise work.
        /// </summary>
        private string NormalizeRowHeight(string sourcePath)
        {
            try
            {
                string tempPath = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    "HIS_IMPORT_RECORD_INSPECTION_" + Guid.NewGuid().ToString("N") + ".xlsx");

                using (DevExpress.XtraSpreadsheet.SpreadsheetControl spreadsheet =
                    new DevExpress.XtraSpreadsheet.SpreadsheetControl())
                {
                    spreadsheet.LoadDocument(sourcePath, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                    DevExpress.Spreadsheet.IWorkbook workbook = spreadsheet.Document;

                    workbook.BeginUpdate();
                    try
                    {
                        foreach (DevExpress.Spreadsheet.Worksheet sheet in workbook.Worksheets)
                        {
                            int lastRow = sheet.Rows.LastUsedIndex;
                            for (int r = 0; r <= lastRow; r++)
                            {
                                sheet.Rows[r].Height = NORMALIZED_ROW_HEIGHT;
                            }
                        }
                    }
                    finally
                    {
                        workbook.EndUpdate();
                    }

                    workbook.SaveDocument(tempPath, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                }

                Inventec.Common.Logging.LogSystem.Debug(
                    "frmImportRecordInspection.NormalizeRowHeight: normalised copy created for " + sourcePath);
                return tempPath;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return sourcePath;
            }
        }

        /// <summary>Removes the normalised copy; never touches the file chosen by the user.</summary>
        private void DeleteTempFile(string tempPath, string originalPath)
        {
            try
            {
                if (string.IsNullOrEmpty(tempPath)) return;
                if (string.Equals(tempPath, originalPath, StringComparison.OrdinalIgnoreCase)) return;
                if (System.IO.File.Exists(tempPath)) System.IO.File.Delete(tempPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Reads the rows, validates them and binds the grid.</summary>
        private void ReadAndBind(Inventec.Common.ExcelImport.Import excel)
        {
            try
            {
                var rawRows = excel.GetWithCheck<RecordInspectionImportADO>(0);
                int rawCount = rawRows == null ? -1 : rawRows.Count;
                var rows = (rawRows ?? new List<RecordInspectionImportADO>())
                    .Where(o => o != null && !string.IsNullOrWhiteSpace(o.TREATMENT_CODE))
                    .ToList();

                Inventec.Common.Logging.LogSystem.Debug(
                    "frmImportRecordInspection.ReadAndBind: file=" + this.currentFileName
                    + " rawCount=" + rawCount + " usableCount=" + rows.Count);

                if (rows.Count == 0)
                {
                    WaitingManager.Hide();

                    // The reader returns an empty list without throwing whenever it cannot locate
                    // the tag row, so the causes are told apart here — otherwise the user gets one
                    // message that hides which part of the file is wrong.
                    string message = rawCount <= 0
                        ? Resources.ResourceMessage.KhongQuetDuocCotTag
                        : string.Format(Resources.ResourceMessage.DocDuocNhungThieuMaDieuTri, rawCount);

                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        message,
                        Resources.ResourceMessage.Thongbao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetDataSource(this.listImport);
                    return;
                }

                if (rows.Count > MAX_ROW_PER_FILE)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        string.Format(Resources.ResourceMessage.VuotGioiHanSoDong, MAX_ROW_PER_FILE),
                        Resources.ResourceMessage.Thongbao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetDataSource(this.listImport);
                    return;
                }

                this.listImport = BuildProcessList(rows);
                SetDataSource(this.listImport);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Normalises every row, resolves the treatments in one batch call and collects all
        /// validation messages of each row.
        /// </summary>
        private List<RecordInspectionImportADO> BuildProcessList(List<RecordInspectionImportADO> rows)
        {
            List<RecordInspectionImportADO> result = new List<RecordInspectionImportADO>();
            try
            {
                long stt = 0;
                foreach (var row in rows)
                {
                    stt++;
                    var ado = new RecordInspectionImportADO();
                    ado.STT = stt;
                    ado.TREATMENT_CODE = NormalizeTreatmentCode(row.TREATMENT_CODE);
                    ado.NOTE = row.NOTE;
                    result.Add(ado);
                }

                // Batch lookup: one call for the whole file instead of one call per row.
                var treatmentByCode = GetTreatmentByCode(
                    result.Select(o => o.TREATMENT_CODE)
                          .Where(o => !string.IsNullOrWhiteSpace(o))
                          .Distinct()
                          .ToList());

                var departmentById = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .GroupBy(o => o.ID)
                    .ToDictionary(g => g.Key, g => g.First());

                // First row number each code was seen at, used to report duplicates inside the file.
                Dictionary<string, long> firstRowOfCode = new Dictionary<string, long>();

                foreach (var ado in result)
                {
                    List<string> errors = new List<string>();

                    if (string.IsNullOrWhiteSpace(ado.TREATMENT_CODE))
                    {
                        errors.Add(Resources.ResourceMessage.ChuaNhapMaDieuTri);
                    }
                    else
                    {
                        if (!IsAllDigit(ado.TREATMENT_CODE) || ado.TREATMENT_CODE.Length > TREATMENT_CODE_LENGTH)
                        {
                            errors.Add(Resources.ResourceMessage.MaDieuTriKhongDungDinhDang);
                        }

                        V_HIS_TREATMENT_11 treatment;
                        if (treatmentByCode.TryGetValue(ado.TREATMENT_CODE, out treatment) && treatment != null)
                        {
                            ado.TREATMENT_ID = treatment.ID;
                            ado.PATIENT_CODE = treatment.TDL_PATIENT_CODE;
                            ado.PATIENT_NAME = treatment.TDL_PATIENT_NAME;
                            ado.IN_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.IN_TIME);
                            ado.OUT_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.OUT_TIME ?? 0);
                            ado.ICD = treatment.ICD_CODE + " - " + treatment.ICD_NAME;

                            HIS_DEPARTMENT department;
                            if (treatment.END_DEPARTMENT_ID.HasValue
                                && departmentById.TryGetValue(treatment.END_DEPARTMENT_ID.Value, out department))
                            {
                                ado.END_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                        }
                        else
                        {
                            errors.Add(string.Format(
                                Resources.ResourceMessage.KhongTimThayHoSoDieuTri, ado.TREATMENT_CODE));
                        }

                        long firstRow;
                        if (firstRowOfCode.TryGetValue(ado.TREATMENT_CODE, out firstRow))
                        {
                            errors.Add(string.Format(Resources.ResourceMessage.TrungVoiDongTrongFile, firstRow));
                        }
                        else
                        {
                            firstRowOfCode.Add(ado.TREATMENT_CODE, ado.STT);
                        }
                    }

                    ado.ERROR = errors.Count > 0 ? string.Join(Environment.NewLine, errors.ToArray()) : "";
                }

                SetAlreadyReceivedWarning(result);

                foreach (var ado in result)
                {
                    ado.STATUS = ado.HasError
                        ? Resources.ResourceMessage.TrangThaiLoi
                        : (ado.HasWarning
                            ? Resources.ResourceMessage.TrangThaiCanhBao
                            : Resources.ResourceMessage.TrangThaiHopLe);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Flags the rows whose treatment is already in the inspection list from an earlier batch.
        ///
        /// This is a notice, not an error: the appraiser sending a treatment again for another
        /// round is normal business and each batch keeps its own row (R7). The operator sees when a
        /// treatment was received before and decides whether to keep or drop the row.
        ///
        /// One request for the whole file. If the endpoint is unavailable no notice is produced —
        /// the import still works.
        /// </summary>
        private void SetAlreadyReceivedWarning(List<RecordInspectionImportADO> rows)
        {
            try
            {
                if (rows == null || rows.Count == 0) return;

                List<long> treatmentIds = rows
                    .Where(o => o.TREATMENT_ID > 0)
                    .Select(o => o.TREATMENT_ID)
                    .Distinct()
                    .ToList();
                if (treatmentIds.Count == 0) return;

                HIS.Desktop.Plugins.TreatmentInspectionV2.ADO.RecordInspectionImpFilter filter =
                    new HIS.Desktop.Plugins.TreatmentInspectionV2.ADO.RecordInspectionImpFilter();
                filter.TREATMENT_IDs = treatmentIds;

                CommonParam param = new CommonParam();
                var existing = new BackendAdapter(param).Get<List<RecordInspectionImpADO>>(
                    HisRequestUriStore.HIS_RECORD_INSPECTION_IMP_GET, ApiConsumers.MosConsumer, filter, param);

                if (existing == null || existing.Count == 0) return;

                // Latest receipt per treatment, so the notice names the most recent batch.
                Dictionary<long, long> latestImportTime = new Dictionary<long, long>();
                foreach (var item in existing)
                {
                    long current;
                    if (latestImportTime.TryGetValue(item.TREATMENT_ID, out current))
                    {
                        if (item.IMPORT_TIME > current) latestImportTime[item.TREATMENT_ID] = item.IMPORT_TIME;
                    }
                    else
                    {
                        latestImportTime.Add(item.TREATMENT_ID, item.IMPORT_TIME);
                    }
                }

                foreach (var ado in rows)
                {
                    long importTime;
                    if (ado.TREATMENT_ID <= 0) continue;
                    if (!latestImportTime.TryGetValue(ado.TREATMENT_ID, out importTime)) continue;

                    ado.WARNING = string.Format(
                        Resources.ResourceMessage.HoSoDaTiepNhanTruocDo,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(importTime));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Resolves the treatments of the whole file with a single request.</summary>
        private Dictionary<string, V_HIS_TREATMENT_11> GetTreatmentByCode(List<string> codes)
        {
            Dictionary<string, V_HIS_TREATMENT_11> result = new Dictionary<string, V_HIS_TREATMENT_11>();
            try
            {
                if (codes == null || codes.Count == 0) return result;

                HisTreatmentView11FilterV2 filter = new HisTreatmentView11FilterV2();
                filter.TREATMENT_CODEs = codes;

                CommonParam param = new CommonParam(0, codes.Count);
                var apiResult = new BackendAdapter(param).GetRO<List<V_HIS_TREATMENT_11>>(
                    HisRequestUriStore.HIS_TREATMENT_GETVIEW11, ApiConsumers.MosConsumer, filter, param);

                if (apiResult == null || apiResult.Data == null) return result;

                foreach (var item in (List<V_HIS_TREATMENT_11>)apiResult.Data)
                {
                    if (item == null || string.IsNullOrWhiteSpace(item.TREATMENT_CODE)) continue;
                    if (!result.ContainsKey(item.TREATMENT_CODE)) result.Add(item.TREATMENT_CODE, item);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Pads a numeric treatment code with leading zeros, as the search box does.</summary>
        private string NormalizeTreatmentCode(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code)) return "";
                code = code.Trim();
                if (code.Length < TREATMENT_CODE_LENGTH && IsAllDigit(code))
                {
                    code = code.PadLeft(TREATMENT_CODE_LENGTH, '0');
                }
                return code;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return code;
            }
        }

        private bool IsAllDigit(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsDigit(value[i])) return false;
            }
            return true;
        }

        #endregion

        #region Grid

        private void SetDataSource(List<RecordInspectionImportADO> dataSource)
        {
            try
            {
                gridViewData.BeginUpdate();
                try
                {
                    gridControlData.DataSource = null;
                    gridControlData.DataSource = dataSource;
                }
                finally
                {
                    gridViewData.EndUpdate();
                }
                RefreshButtonState();
                UpdateSummary();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefreshButtonState()
        {
            try
            {
                bool hasData = this.listImport != null && this.listImport.Count > 0;
                bool hasError = hasData && this.listImport.Any(o => o.HasError);

                btnShowLineError.Enabled = hasError;
                btnSave.Enabled = hasData && !hasError;

                btnShowLineError.Text = this.isShowingErrorOnly
                    ? GetLang("frmImportRecordInspection.btnShowLineError.TextAll")
                    : GetLang("frmImportRecordInspection.btnShowLineError.Text");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateSummary()
        {
            try
            {
                int total = this.listImport == null ? 0 : this.listImport.Count;
                int error = this.listImport == null ? 0 : this.listImport.Count(o => o.HasError);
                int warning = this.listImport == null ? 0 : this.listImport.Count(o => !o.HasError && o.HasWarning);
                lblSummary.Text = string.Format(
                    Resources.ResourceMessage.ThongKeNhapKhau,
                    total, total - error, error, warning,
                    string.IsNullOrEmpty(this.currentFileName) ? "-" : this.currentFileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                this.isShowingErrorOnly = !this.isShowingErrorOnly;
                var view = this.isShowingErrorOnly
                    ? this.listImport.Where(o => o.HasError).ToList()
                    : this.listImport;

                gridViewData.BeginUpdate();
                try
                {
                    gridControlData.DataSource = null;
                    gridControlData.DataSource = view;
                }
                finally
                {
                    gridViewData.EndUpdate();
                }
                RefreshButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound) return;
                var source = ((IList)((BaseView)sender).DataSource);
                if (source == null || e.ListSourceRowIndex < 0 || e.ListSourceRowIndex >= source.Count) return;

                // Both unbound columns only host a button, so no value is produced here.
                e.Value = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewData_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                var data = gridViewData.GetRow(e.RowHandle) as RecordInspectionImportADO;
                if (data == null) return;

                if (e.Column.FieldName == "ERROR_VIEW")
                {
                    // An icon only shows up on rows that carry something; clean rows keep an empty
                    // cell. An error hides the warning — the blocking problem is what matters.
                    if (data.HasError) e.RepositoryItem = btnGError;
                    else if (data.HasWarning) e.RepositoryItem = btnGWarning;
                }
                else if (e.Column.FieldName == "DELETE_VIEW")
                {
                    e.RepositoryItem = btnGDelete;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewData_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                var data = gridViewData.GetRow(e.RowHandle) as RecordInspectionImportADO;
                if (data == null) return;

                if (data.HasError)
                {
                    e.Appearance.ForeColor = System.Drawing.Color.Red;
                }
                else if (data.HasWarning)
                {
                    // Orange, not red: the row still saves.
                    e.Appearance.ForeColor = System.Drawing.Color.DarkOrange;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Shows every validation message of the hovered row as a tooltip on the error icon, so the
        /// reason is readable without clicking — same behaviour as the other import screens.
        ///
        /// The controller raises this on every mouse move. Building a new ToolTipControlInfo each
        /// time makes the tooltip close and reopen, which the user sees as flickering, so the last
        /// one is cached and reused while the pointer stays on the same cell.
        /// </summary>
        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info != null || e.SelectedControl != gridControlData) return;

                DevExpress.XtraGrid.Views.Grid.GridView view =
                    gridControlData.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null) return;

                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo =
                    view.CalcHitInfo(e.ControlMousePosition);

                if (!hitInfo.InRowCell || hitInfo.Column == null
                    || hitInfo.Column.FieldName != "ERROR_VIEW")
                {
                    ResetToolTipCache();
                    return;
                }

                var data = view.GetRow(hitInfo.RowHandle) as RecordInspectionImportADO;
                if (data == null || (!data.HasError && !data.HasWarning))
                {
                    ResetToolTipCache();
                    return;
                }

                if (this.lastRowHandle != hitInfo.RowHandle || this.lastColumn != hitInfo.Column
                    || this.lastInfo == null)
                {
                    this.lastRowHandle = hitInfo.RowHandle;
                    this.lastColumn = hitInfo.Column;
                    this.lastInfo = new DevExpress.Utils.ToolTipControlInfo(
                        new DevExpress.XtraGrid.GridToolTipInfo(
                            view, new CellToolTipInfo(hitInfo.RowHandle, hitInfo.Column, "Text")),
                        data.HasError ? data.ERROR : data.WARNING);
                }

                e.Info = this.lastInfo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetToolTipCache()
        {
            this.lastRowHandle = -1;
            this.lastColumn = null;
            this.lastInfo = null;
        }

        /// <summary>Clicking the error icon opens the full list — useful when a row has many.</summary>
        private void btnGError_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = gridViewData.GetFocusedRow() as RecordInspectionImportADO;
                if (data == null || !data.HasError) return;

                DevExpress.XtraEditors.XtraMessageBox.Show(
                    data.ERROR, Resources.ResourceMessage.Thongbao,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Clicking the notice icon shows the non blocking messages of the row.</summary>
        private void btnGWarning_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = gridViewData.GetFocusedRow() as RecordInspectionImportADO;
                if (data == null || !data.HasWarning) return;

                DevExpress.XtraEditors.XtraMessageBox.Show(
                    data.WARNING, Resources.ResourceMessage.Thongbao,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = gridViewData.GetFocusedRow() as RecordInspectionImportADO;
                if (data == null) return;

                this.listImport.Remove(data);

                // Removing a row can clear a duplicate flag on the row that kept the code, so the
                // whole batch is validated again from the remaining rows.
                this.listImport = BuildProcessList(this.listImport);
                this.isShowingErrorOnly = false;
                SetDataSource(this.listImport);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Save

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;
                if (this.listImport == null || this.listImport.Count == 0) return;
                if (this.listImport.Any(o => o.HasError)) return;

                long importTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                List<RecordInspectionImpADO> data = this.listImport
                    .Select(o => new RecordInspectionImpADO
                    {
                        TREATMENT_ID = o.TREATMENT_ID,
                        IMPORT_TIME = importTime,
                        FILE_NAME = this.currentFileName
                    })
                    .ToList();

                bool success = false;
                CommonParam param = new CommonParam();
                try
                {
                    WaitingManager.Show();
                    btnSave.Enabled = false;

                    Inventec.Common.Logging.LogSystem.Debug(
                        Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));

                    var result = new BackendAdapter(param).Post<List<RecordInspectionImpADO>>(
                        HisRequestUriStore.HIS_RECORD_INSPECTION_IMP_CREATE_LIST,
                        ApiConsumers.MosConsumer, data, param);

                    success = result != null && result.Count > 0;
                }
                finally
                {
                    WaitingManager.Hide();
                }

                // Session handling stays with MessageManager here, matching the rest of this
                // plugin: it does not reference HIS.Desktop.Controls.Session.
                MessageManager.Show(this, param, success);

                if (!success)
                {
                    btnSave.Enabled = true;
                    return;
                }

                this.HasSaved = true;
                foreach (var item in this.listImport)
                {
                    item.STATUS = Resources.ResourceMessage.TrangThaiDaLuu;
                }
                gridViewData.RefreshData();
                UpdateSummary();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.S))
                {
                    btnSave.PerformClick();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion
    }
}
