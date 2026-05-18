using Aspose.Cells;
using DevExpress.XtraEditors;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Xuat HSTH01BH (C79 / TT12) ra Excel — pattern chunked giong Excel 130
    /// nhung don gian hon (1 file 1 sheet flat table 21 cot).
    /// Path: XmlExcel/ExcelTT12/yyyyMMdd/Run_HHmmss/HSTH01BH.xlsx
    /// </summary>
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        BackgroundWorker backgroundWorkerExelTT12 = null;

        public void ProcessDataExcelTT12()
        {
            try
            {
                if (listSelection == null || listSelection.Count == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn hồ sơ để xuất.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (backgroundWorkerExelTT12 != null && backgroundWorkerExelTT12.IsBusy)
                {
                    XtraMessageBox.Show("Đang xử lý xuất Excel TT12, vui lòng đợi.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (backgroundWorkerExel != null && backgroundWorkerExel.IsBusy)
                {
                    XtraMessageBox.Show("Đang xuất Excel khác, vui lòng đợi xong rồi xuất TT12.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (listSelection.Count >= ConfirmThreshold)
                {
                    long estMinLow = Math.Max(1, listSelection.Count / 5000);
                    long estMinHigh = Math.Max(2, listSelection.Count / 2000);
                    int totalChunks = (listSelection.Count + ChunkSize - 1) / ChunkSize;
                    string msg = string.Format(
                        "Bạn đang xuất {0:N0} hồ sơ ra Excel TT12 (chia {1} chunk × {2} hồ sơ).\nƯớc lượng thời gian: {3}-{4} phút.\n\nTiếp tục?",
                        listSelection.Count, totalChunks, ChunkSize, estMinLow, estMinHigh);
                    var res = XtraMessageBox.Show(msg, "Cảnh báo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (res != DialogResult.Yes) return;
                }

                paramExcel = new CommonParam();
                IsProcessingExcel = true;

                backgroundWorkerExelTT12 = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true,
                };
                backgroundWorkerExelTT12.DoWork += backgroundWorkerExelTT12_DoWork;
                backgroundWorkerExelTT12.ProgressChanged += backgroundWorkerExel_ProgressChanged;       // reuse cross-file (cung partial class)
                backgroundWorkerExelTT12.RunWorkerCompleted += backgroundWorkerExel_RunWorkerCompleted; // reuse cross-file

                Inventec.Common.Logging.LogSystem.Info(
                    "ProcessDataExcelTT12 - Begin export: " + listSelection.Count + " ho so (chunked by " + ChunkSize + ")");

                WaitingManager.Show();
                backgroundWorkerExelTT12.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                IsProcessingExcel = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorkerExelTT12_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            var stopwatch = Stopwatch.StartNew();

            // Lower priority de UI thread khong bi block
            try { Thread.CurrentThread.Priority = ThreadPriority.BelowNormal; } catch { }
            try { SetLicenseForAsposeCell(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }

            // Path: XmlExcel/ExcelTT12/yyyyMMdd/Run_HHmmss/
            string baseDir = Path.Combine(Application.StartupPath, "XmlExcel", "ExcelTT12",
                DateTime.Now.ToString("yyyyMMdd"));
            string runDir = Path.Combine(baseDir, "Run_" + DateTime.Now.ToString("HHmmss"));
            try
            {
                if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);
                if (!Directory.Exists(runDir)) Directory.CreateDirectory(runDir);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return;
            }

            // Reflection cache (compiled accessor) — su dung GetCachedMeta tu UCExportXml_Excel.cs (cung partial class)
            var meta = GetCachedMeta(typeof(HSTH01BH_CHITIET));
            int cols = meta.Properties.Length;

            // Pre-allocated 2D buffer reuse cross-chunk (KHONG alloc per-chunk)
            object[,] rowBuffer = new object[ChunkSize, cols];

            // 1 Workbook duy nhat, 1 sheet "C79"
            Workbook wb = null;
            Worksheet sheet = null;
            int writtenRows = 0;
            int total = listSelection.Count;
            int totalChunks = (total + ChunkSize - 1) / ChunkSize;

            try
            {
                wb = new Workbook();
                wb.Worksheets.Clear();
                int sheetIdx = wb.Worksheets.Add();
                sheet = wb.Worksheets[sheetIdx];
                sheet.Name = "C79";

                // Header (21 cot)
                object[,] header = new object[1, cols];
                for (int c = 0; c < cols; c++) header[0, c] = meta.Properties[c].Name;
                sheet.Cells.ImportTwoDimensionArray(header, 0, 0);

                int chunkIdx = 0;
                for (int start = 0; start < total; start += ChunkSize)
                {
                    if (worker.CancellationPending) { e.Cancel = true; break; }

                    int take = Math.Min(ChunkSize, total - start);
                    chunkIdx++;
                    var chunk = listSelection.GetRange(start, take); // O(take), khong Skip O(n)

                    Inventec.Common.Logging.LogSystem.Info(string.Format(
                        "ExportTT12Excel chunk {0}/{1}: {2} ho so", chunkIdx, totalChunks, take));

                    try
                    {
                        // Reset state list cho chunk (giong GenerateXmlTT12)
                        ListPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
                        ListSereServ = new List<V_HIS_SERE_SERV_2>();
                        HisTreatments = new List<V_HIS_TREATMENT_12>();
                        HisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();
                        this.NewConfig = GetNewConfig();

                        isExportXml = true;
                        CreateThreadGetData(chunk);
                        isExportXml = false;

                        var dicErr = new Dictionary<string, List<string>>();
                        var chiTiet = BuildHsth01bhChiTietList(
                            HisTreatments, ListPatientTypeAlter, ListSereServ, HisSereServPttts,
                            writtenRows + 1, dicErr);

                        int rows = chiTiet != null ? chiTiet.Count : 0;
                        if (rows > 0)
                        {
                            // Fill rowBuffer bang COMPILED accessor (no reflection cost)
                            for (int r = 0; r < rows; r++)
                            {
                                var item = chiTiet[r];
                                for (int c = 0; c < cols; c++)
                                    rowBuffer[r, c] = meta.Accessors[c](item);
                            }

                            // Partial slice neu rows < ChunkSize (chunk cuoi); else reuse rowBuffer
                            object[,] toImport;
                            if (rows == ChunkSize)
                            {
                                toImport = rowBuffer;
                            }
                            else
                            {
                                toImport = new object[rows, cols];
                                for (int r = 0; r < rows; r++)
                                    for (int c = 0; c < cols; c++)
                                        toImport[r, c] = rowBuffer[r, c];
                            }

                            sheet.Cells.ImportTwoDimensionArray(toImport, writtenRows + 1, 0); // +1 vi co header row
                            writtenRows += rows;

                            // Clear references trong rowBuffer (tranh hold value cu cho GC) nhung KHONG realloc
                            Array.Clear(rowBuffer, 0, rows * cols);
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(
                            "ExportTT12Excel chunk " + chunkIdx + " error: " + ex);
                        // skip chunk, tiep tuc chunk ke
                    }

                    int pct = total > 0 ? (int)Math.Min(99, 100.0 * (start + take) / total) : 0;
                    worker.ReportProgress(pct, (start + take) + "/" + total);
                }

                // Save 1 LAN cuoi cung
                if (writtenRows > 0)
                {
                    string filePath = Path.Combine(runDir, "HSTH01BH.xlsx");
                    try
                    {
                        wb.Save(filePath, SaveFormat.Xlsx);
                        Inventec.Common.Logging.LogSystem.Info(
                            "ExportTT12Excel: saved " + writtenRows + " rows -> " + filePath);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }

                worker.ReportProgress(100, writtenRows + " rows written");
            }
            finally
            {
                var disp = wb as IDisposable;
                if (disp != null) try { disp.Dispose(); } catch { }
                stopwatch.Stop();
                Inventec.Common.Logging.LogSystem.Info(
                    "ExportTT12Excel: DoWork finished in " +
                    stopwatch.Elapsed.TotalSeconds.ToString("F1") + "s, rows=" + writtenRows);
            }
        }
    }
}
