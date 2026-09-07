using Aspose.Cells;
using DevExpress.XtraEditors;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        // Tuning constants
        private const int RowLimitPerPart = 950000;            // Excel xlsx hard limit là 1,048,576 — rotate ở 950k để chừa biên
        private const int ConfirmThreshold = 5000;             // Show confirm dialog khi count >= ngưỡng này
        private const int ProgressEvery = 50;                  // ReportProgress mỗi N hồ sơ
        private const int ParallelSaverCount = 4;              // Số saver thread cho individual files
        private const int SaveQueueCapacity = 8;               // Bounded queue (backpressure cho producer)
        private const int GroupBufferFlushThreshold = 10000;   // Flush group buffer mỗi N rows
        private const int FileReadBufferSize = 65536;          // FileStream buffer 64KB cho XmlReader stream
        private const int FilesPerBucket = 1000;               // Bucketing: 1000 individual file/subfolder Batch_NNN
        private const int ChunkSize = 1000;                    // Chunk size cho GenerateXmlPlus per iteration

        // Process-wide caches (thread-safe)
        private static readonly ConcurrentDictionary<Type, TypeMeta> TypeMetaCache
            = new ConcurrentDictionary<Type, TypeMeta>();

        // Cached XmlSerializer for streaming HOSO parse (expensive to construct, thread-safe to use)
        private static readonly XmlSerializer HoSoSerializer
            = new XmlSerializer(typeof(His.Bhyt.ExportXml.XML130.XML.HoSo));

        // Cached invalid filename chars (HashSet O(1) lookup vs Array.IndexOf O(n))
        private static readonly HashSet<char> InvalidFileNameChars
            = new HashSet<char>(Path.GetInvalidFileNameChars());

        string Aspose_Key =
            "PExpY2Vuc2U+DQogIDxEYXRhPg0KICAgIDxMaWNlbnNlZFRvPkFzcG9zZSBTY290bGFuZCB" +
            "UZWFtPC9MaWNlbnNlZFRvPg0KICAgIDxFbWFpbFRvPmJpbGx5Lmx1bmRpZUBhc3Bvc2UuY2" +
            "9tPC9FbWFpbFRvPg0KICAgIDxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgT0VNPC9MaWNlbnNlV" +
            "HlwZT4NCiAgICA8TGljZW5zZU5vdGU+TGltaXRlZCB0byAxIGRldmVsb3BlciwgdW5saW1p" +
            "dGVkIHBoeXNpY2FsIGxvY2F0aW9uczwvTGljZW5zZU5vdGU+DQogICAgPE9yZGVySUQ+MTQ" +
            "wNDA4MDUyMzI0PC9PcmRlcklEPg0KICAgIDxVc2VySUQ+OTQyMzY8L1VzZXJJRD4NCiAgIC" +
            "A8T0VNPlRoaXMgaXMgYSByZWRpc3RyaWJ1dGFibGUgbGljZW5zZTwvT0VNPg0KICAgIDxQc" +
            "m9kdWN0cz4NCiAgICAgIDxQcm9kdWN0PkFzcG9zZS5Ub3RhbCBmb3IgLk5FVDwvUHJvZHVj" +
            "dD4NCiAgICA8L1Byb2R1Y3RzPg0KICAgIDxFZGl0aW9uVHlwZT5FbnRlcnByaXNlPC9FZGl" +
            "0aW9uVHlwZT4NCiAgICA8U2VyaWFsTnVtYmVyPjlhNTk1NDdjLTQxZjAtNDI4Yi1iYTcyLT" +
            "djNDM2OGYxNTFkNzwvU2VyaWFsTnVtYmVyPg0KICAgIDxTdWJzY3JpcHRpb25FeHBpcnk+M" +
            "jAxNTEyMzE8L1N1YnNjcmlwdGlvbkV4cGlyeT4NCiAgICA8TGljZW5zZVZlcnNpb24+My4w" +
            "PC9MaWNlbnNlVmVyc2lvbj4NCiAgICA8TGljZW5zZUluc3RydWN0aW9ucz5odHRwOi8vd3d" +
            "3LmFzcG9zZS5jb20vY29ycG9yYXRlL3B1cmNoYXNlL2xpY2Vuc2UtaW5zdHJ1Y3Rpb25zLm" +
            "FzcHg8L0xpY2Vuc2VJbnN0cnVjdGlvbnM+DQogIDwvRGF0YT4NCiAgPFNpZ25hdHVyZT5GT" +
            "zNQSHNibGdEdDhGNTlzTVQxbDFhbXlpOXFrMlY2RThkUWtJUDdMZFRKU3hEaWJORUZ1MXpP" +
            "aW5RYnFGZkt2L3J1dHR2Y3hvUk9rYzF0VWUwRHRPNmNQMVpmNkowVmVtZ1NZOGkvTFpFQ1R" +
            "Hc3pScUpWUVJaME1vVm5CaHVQQUprNWVsaTdmaFZjRjhoV2QzRTRYUTNMemZtSkN1YWoyTk" +
            "V0ZVJpNUhyZmc9PC9TaWduYXR1cmU+DQo8L0xpY2Vuc2U+";

        BackgroundWorker backgroundWorkerExel = null;
        string PathTempXml = null;
        string IndividualDir = null;
        string GroupDir = null;   // Folder chua Group_LOAIHOSO_Part*.xlsx (per-run)
        string GroupRunStamp = null;
        bool IsProcessingExcel = false;
        CommonParam paramExcel = new CommonParam();
        string saveFileExcel = "";
        string saveFileExcel12 = "";
        // Cache bucket folder đã tạo — tránh gọi Directory.Exists/CreateDirectory mỗi iteration
        private readonly HashSet<int> _createdBuckets = new HashSet<int>();
        // Danh sách file XML từng lô (__xmlpart_NNNN.xml) để cuối run gộp thành 1 file Group_<stamp>.xml
        private readonly List<string> _xmlPartPaths = new List<string>();

        // Reconciliation counters — verify Individual files vs Group XML1 rows
        private int _hosoTotal;            // Tổng HOSO đã đọc từ XML chunks
        private int _hosoNoFilehoso;       // HOSO có FILEHOSO null/empty → skip individual + group
        private int _hosoNoXml1Entry;      // Có FILEHOSO nhưng KHÔNG có entry LOAIHOSO=XML1
        private int _hosoXml1ParseEmpty;   // Có XML1 entry nhưng ExtractItems rỗng → placeholder appended
        private int _hosoXml1Ok;           // XML1 valid, items.Count > 0 → group XML1 +1 (real)
        private int _individualSaved;      // Saver Save() success
        private int _individualSaveFailed; // Saver Save() throw

        // Status theo từng hồ sơ — index khớp listSelection sau dedup.
        // null = chưa xử lý (= không tìm thấy HOSO trong XML output → unaccounted),
        // "OK" = save thành công, giá trị khác = lý do lỗi (sẽ được log cuối run).
        private string[] _recordStatus;

        // Đối soát Batch vs Group theo mã hồ sơ điều trị (index khớp listSelection sau dedup).
        // _recordTreatmentCode: mã hồ sơ (MA_LK) của idx.
        // _recordInGroupXml1: idx này đã được append 1 row vào group XML1 (real/placeholder) chưa — set bởi producer thread.
        // _recordInBatch: idx này đã có file individual save OK (kể cả fallback) chưa — set bởi saver thread.
        private string[] _recordTreatmentCode;
        private bool[] _recordInGroupXml1;
        private bool[] _recordInBatch;

        public void ProcessDataExcel()
        {
            try
            {
                if (listSelection == null || listSelection.Count == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn hồ sơ để xuất.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                listSelection = listSelection.GroupBy(o => o.TREATMENT_CODE).Select(s => s.First()).ToList();

                if (backgroundWorkerExel != null && backgroundWorkerExel.IsBusy)
                {
                    XtraMessageBox.Show("Đang xử lý xuất Excel, vui lòng đợi.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (listSelection.Count >= ConfirmThreshold)
                {
                    long estMinLow = Math.Max(1, listSelection.Count / 1500);
                    long estMinHigh = Math.Max(2, listSelection.Count / 500);
                    long estMb = (long)listSelection.Count * 120 / 1024;
                    int bucketCount = (listSelection.Count + FilesPerBucket - 1) / FilesPerBucket;
                    string msg = string.Format(
                        "Bạn đang xuất {0:N0} hồ sơ (chia thành {1} thư mục con Batch_NNN, mỗi thư mục {2:N0} file).\nKèm 1 file XML gộp Group_<thời gian>.xml chứa tất cả hồ sơ.\nƯớc lượng thời gian: {3}-{4} phút, dung lượng ~{5:N0} MB.\n\nTiếp tục?",
                        listSelection.Count, bucketCount, FilesPerBucket, estMinLow, estMinHigh, estMb);
                    var res = XtraMessageBox.Show(msg, "Cảnh báo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (res != DialogResult.Yes) return;
                }

                // Setup nặng (license, directory, delete cũ) chuyển vào DoWork — UI thread không block khi click
                paramExcel = new CommonParam();
                IsProcessingExcel = true;

                backgroundWorkerExel = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true,
                };
                backgroundWorkerExel.DoWork += backgroundWorkerExel_DoWork;
                backgroundWorkerExel.ProgressChanged += backgroundWorkerExel_ProgressChanged;
                backgroundWorkerExel.RunWorkerCompleted += backgroundWorkerExel_RunWorkerCompleted;

                Inventec.Common.Logging.LogSystem.Info(
                    "ProcessDataExcel - Begin export: " + listSelection.Count + " ho so (bucketed by " + FilesPerBucket + ")");

                WaitingManager.Show();
                backgroundWorkerExel.RunWorkerAsync(chkXML3176.Checked);
            }
            catch (Exception ex)
            {
                IsProcessingExcel = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorkerExel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                string text = e.UserState as string;
                if (!string.IsNullOrEmpty(text))
                    Inventec.Common.Logging.LogSystem.Info("ExportExcel progress: " + text);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorkerExel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                IsProcessingExcel = false;
                WaitingManager.Hide();

                if (e.Cancelled)
                {
                    XtraMessageBox.Show("Đã hủy xuất Excel.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (e.Error != null)
                {
                    Inventec.Common.Logging.LogSystem.Error(e.Error);
                    XtraMessageBox.Show("Lỗi xuất Excel: " + e.Error.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Báo cho user biết có bao nhiêu hồ sơ lỗi/thiếu (chi tiết MA_DT đã log vào file).
                int failedCount = 0;
                if (_recordStatus != null)
                {
                    for (int i = 0; i < _recordStatus.Length; i++)
                    {
                        if (_recordStatus[i] != "OK") failedCount++;
                    }
                    if (failedCount > 0)
                    {
                        XtraMessageBox.Show(
                            "Xuất Excel hoàn tất.\nCó " + failedCount + "/" + _recordStatus.Length +
                            " hồ sơ KHÔNG xuất được file. Danh sách MA_DT chi tiết đã ghi trong log.",
                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                MessageManager.Show(paramExcel, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorkerExel_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            bool isXML3176_Value = e.Argument is bool ? (bool)e.Argument : false;
            var stopwatch = Stopwatch.StartNew();

            // (D) Lower priority để UI thread luôn được ưu tiên CPU → tránh Not Responding
            try { Thread.CurrentThread.Priority = ThreadPriority.BelowNormal; } catch { }

            // (B) Setup nặng chuyển từ UI thread vào đây — KHÔNG block UI khi user click Export
            try { SetLicenseForAsposeCell(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }

            try
            {
                // Path: XmlExcel/Excel130/yyyyMMdd/Run_HHmmss/
                // - KHONG xoa file cu tu cac run truoc
                // - Moi run tao subfolder Run_HHmmss rieng → tranh ghi de cross-run
                PathTempXml = Path.Combine(Application.StartupPath, "XmlExcel", "Excel130",
                    DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(PathTempXml))
                    Directory.CreateDirectory(PathTempXml);

                string runStamp = DateTime.Now.ToString("HHmmss");
                string runDir = Path.Combine(PathTempXml, "Run_" + runStamp);
                if (!Directory.Exists(runDir))
                    Directory.CreateDirectory(runDir);

                // Group_LOAIHOSO_Part*.xlsx + DATA_XML_*.xml deu nam trong runDir
                GroupDir = runDir;
                GroupRunStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                IndividualDir = Path.Combine(runDir, "Individual");
                if (!Directory.Exists(IndividualDir))
                    Directory.CreateDirectory(IndividualDir);

                // Reset bucket cache 1 lan dau run (KHONG clear giua cac chunk)
                _createdBuckets.Clear();
                _xmlPartPaths.Clear();

                // Reset reconciliation counters
                _hosoTotal = 0;
                _hosoNoFilehoso = 0;
                _hosoNoXml1Entry = 0;
                _hosoXml1ParseEmpty = 0;
                _hosoXml1Ok = 0;
                _individualSaved = 0;
                _individualSaveFailed = 0;

                // Reset per-record status array — size khớp listSelection sau dedup
                _recordStatus = new string[listSelection.Count];
                _recordTreatmentCode = new string[listSelection.Count];
                _recordInGroupXml1 = new bool[listSelection.Count];
                _recordInBatch = new bool[listSelection.Count];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return;
            }

            // (L) Parallel save pipeline — 4 saver threads cho individual files (SSD đủ throughput)
            // Pipeline khoi tao 1 LAN, dung chung cho moi chunk — KHONG complete giua cac chunk
            BlockingCollection<IndividualSaveJob> saveQueue = new BlockingCollection<IndividualSaveJob>(SaveQueueCapacity);
            List<Task> saverTasks = new List<Task>();
            // Callback đếm save success/fail + mark per-record status (4 threads → Interlocked cho counter,
            // array write per-idx thread-safe vì mỗi job có Idx duy nhất)
            Action<IndividualSaveJob, bool, Exception> indvOnComplete = (job, ok, exSave) =>
            {
                if (ok)
                {
                    Interlocked.Increment(ref _individualSaved);
                    MarkRecordStatus(job.Idx, "OK");
                    if (job.Idx >= 0 && _recordInBatch != null && job.Idx < _recordInBatch.Length)
                        _recordInBatch[job.Idx] = true;
                }
                else
                {
                    Interlocked.Increment(ref _individualSaveFailed);
                    MarkRecordStatus(job.Idx, "Save failed: " + (exSave != null ? exSave.Message : "unknown"));
                }
            };
            for (int t = 0; t < ParallelSaverCount; t++)
            {
                saverTasks.Add(Task.Factory.StartNew(() => RunSaver(saveQueue, indvOnComplete),
                    CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default));
            }

            // (G) Group saver dedicated — capacity=1 để không giữ nhiều group workbook (~1GB/cái) trong RAM cùng lúc
            // Group saver KHÔNG đếm vào _individualSaved (chỉ đếm save individual file)
            BlockingCollection<IndividualSaveJob> groupSaveQueue = new BlockingCollection<IndividualSaveJob>(1);
            Task groupSaverTask = Task.Factory.StartNew(() => RunSaver(groupSaveQueue, null),
                CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            // State chia se cross-chunk (KHONG reset giua cac chunk de tranh trung MA_LK / sai group counter)
            var groupBuckets = new Dictionary<string, GroupContext>(StringComparer.OrdinalIgnoreCase);
            var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int totalCount = listSelection.Count;
            int globalIdx = 0;
            int chunkIdx = 0;
            int totalChunks = (totalCount + ChunkSize - 1) / ChunkSize;

            try
            {
                while (globalIdx < totalCount)
                {
                    if (worker.CancellationPending) { e.Cancel = true; break; }

                    int take = Math.Min(ChunkSize, totalCount - globalIdx);
                    var chunk = listSelection.Skip(globalIdx).Take(take).ToList();
                    chunkIdx++;
                    int chunkStartIdx = globalIdx;

                    Inventec.Common.Logging.LogSystem.Info(string.Format(
                        "ExportExcel chunk {0}/{1}: {2} ho so (range {3}-{4})",
                        chunkIdx, totalChunks, take, chunkStartIdx, chunkStartIdx + take - 1));

                    // 1. Generate XML cho chunk hien tai
                    string chunkXmlPath = null;
                    try
                    {
                        using (var ms = new MemoryStream())
                        {
                            MemoryStream tempStream = ms;
                            bool ok = this.GenerateXmlPlus(ref paramExcel, ref tempStream, true, chunk, isXML3176_Value);
                            if (!ok)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(
                                    "ExportExcel chunk " + chunkIdx + " GenerateXmlPlus failed → skip chunk");
                                for (int i = 0; i < take; i++)
                                    MarkRecordStatus(chunkStartIdx + i, "GenerateXmlPlus failed (chunk " + chunkIdx + ")");
                                globalIdx += take;
                                continue;
                            }
                        }
                        chunkXmlPath = saveFileExcel; // GenerateXmlPlus set saveFileExcel = path file XML moi
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(
                            "ExportExcel chunk " + chunkIdx + " generate exception: " + ex);
                        for (int i = 0; i < take; i++)
                            MarkRecordStatus(chunkStartIdx + i, "GenerateXmlPlus exception (chunk " + chunkIdx + "): " + ex.Message);
                        globalIdx += take;
                        continue;
                    }

                    if (string.IsNullOrEmpty(chunkXmlPath) || !File.Exists(chunkXmlPath))
                    {
                        Inventec.Common.Logging.LogSystem.Warn(
                            "ExportExcel chunk " + chunkIdx + " XML file missing → skip chunk");
                        for (int i = 0; i < take; i++)
                            MarkRecordStatus(chunkStartIdx + i, "XML file missing (chunk " + chunkIdx + ")");
                        globalIdx += take;
                        continue;
                    }

                    // 2. Parse + emit individual files cho chunk hien tai
                    int beforeIdx = globalIdx;
                    try
                    {
                        ParseChunkAndEmit(chunkXmlPath, worker, ref globalIdx, totalCount,
                            usedNames, groupBuckets, saveQueue);

                        if (worker.CancellationPending) { e.Cancel = true; break; }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(
                            "ExportExcel chunk " + chunkIdx + " parse exception: " + ex);
                        // Bu globalIdx neu parse fail giua chung — chunk nay co the da emit 1 phan
                        //if (globalIdx < beforeIdx + take) globalIdx = beforeIdx + take;
                        // Mark cac record con lai trong chunk chua xu ly (status van null) la parse exception
                        for (int i = globalIdx; i < beforeIdx + take; i++)
                            MarkRecordStatus(i, "Parse chunk exception: " + ex.Message);
                    }
                    finally
                    {
                        // Giu lai DATA_XML chunk -> MOVE sang part file duy nhat (theo chunkIdx) de cuoi run
                        // gop thanh 1 file Group_<stamp>.xml chua tat ca ho so. Van xoa file XML12 (ngoai pham vi).
                        globalIdx = beforeIdx + take;
                        try
                        {
                            if (!string.IsNullOrEmpty(chunkXmlPath) && File.Exists(chunkXmlPath))
                            {
                                string partPath = Path.Combine(GroupDir ?? PathTempXml,
                                    "__xmlpart_" + chunkIdx.ToString("D4") + ".xml");
                                if (File.Exists(partPath)) File.Delete(partPath);
                                File.Move(chunkXmlPath, partPath);
                                _xmlPartPaths.Add(partPath);
                            }
                            if (!string.IsNullOrEmpty(saveFileExcel12) && File.Exists(saveFileExcel12))
                                File.Delete(saveFileExcel12);
                        }
                        catch (Exception exDel)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(
                                "ExportExcel chunk " + chunkIdx + " move DATA_XML to part failed: " + exDel.Message);
                        }
                    }
                }

                worker.ReportProgress(100, globalIdx + "/" + totalCount);
            }
            finally
            {
                // (G) Hand off group workbooks to dedicated group saver — KHÔNG save trên producer thread nữa
                foreach (var ctx in groupBuckets.Values)
                {
                    try
                    {
                        FlushGroup(ctx);
                        if (ctx.RowCount > 0 && ctx.Workbook != null)
                        {
                            // Transfer ownership to group saver thread
                            try
                            {
                                groupSaveQueue.Add(new IndividualSaveJob { Workbook = ctx.Workbook, FilePath = ctx.FilePath });
                                ctx.Workbook = null;
                                ctx.Sheet = null;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    finally
                    {
                        ctx.Dispose(); // dispose any remaining workbook
                    }
                }

                // Stop accepting new jobs
                saveQueue.CompleteAdding();
                groupSaveQueue.CompleteAdding();

                // Wait for all savers to drain (no timeout — let them finish properly)
                try
                {
                    var allTasks = new List<Task>(saverTasks) { groupSaverTask };
                    Task.WaitAll(allTasks.ToArray());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                saveQueue.Dispose();
                groupSaveQueue.Dispose();

                // Gop cac file Group_LOAIHOSO_PartN.xlsx cung Part vao 1 file Group_PartN.xlsx
                // (moi LOAIHOSO = 1 sheet) — sau khi tat ca savers da drain
                try { MergeGroupFilesByPart(); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error("MergeGroupFilesByPart: " + ex); }

                // Gop cac file XML tung lo (__xmlpart_*.xml) thanh 1 file Group_<stamp>.xml duy nhat chua tat ca ho so
                try { MergeXmlPartsIntoGroup(); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error("MergeXmlPartsIntoGroup: " + ex); }

                stopwatch.Stop();
                Inventec.Common.Logging.LogSystem.Info(
                    "ExportExcel: DoWork finished. Processed " + globalIdx + " HOSO in " +
                    stopwatch.Elapsed.TotalSeconds.ToString("F1") + "s.");

                // Reconciliation log — verify Individual files vs Group XML1 rows
                // Group XML1 giờ có 1 row/HOSO cho MỌI HOSO có FILEHOSO (ok + parse_empty + no_xml1_entry)
                // → khớp với số file individual (Batch). expectedXml1Rows == expectedIndividualFiles.
                int expectedXml1Rows = _hosoXml1Ok + _hosoXml1ParseEmpty + _hosoNoXml1Entry;
                int expectedIndividualFiles = _hosoTotal - _hosoNoFilehoso;
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "ExportExcel reconciliation: total={0}, no_filehoso={1}, no_xml1_entry={2}, xml1_parse_empty={3}, xml1_ok={4}, individual_saved={5}, individual_failed={6} → expected_individual={7}, expected_xml1_rows={8}",
                    _hosoTotal, _hosoNoFilehoso, _hosoNoXml1Entry, _hosoXml1ParseEmpty,
                    _hosoXml1Ok, _individualSaved, _individualSaveFailed,
                    expectedIndividualFiles, expectedXml1Rows));

                if (_hosoNoXml1Entry > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "ExportExcel: " + _hosoNoXml1Entry + " HOSO không có entry LOAIHOSO=XML1 → đã append placeholder _NO_XML1 vào group XML1 để khớp số file Batch. Đây là data thiếu XML1, không phải bug code.");
                }
                if (_individualSaveFailed > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "ExportExcel: " + _individualSaveFailed + " individual file Save() throw exception (xem log trước đó).");
                }

                // Log thô message thư viện trả về (chỉ có khi cả batch build XML fail — thư viện chỉ set
                // messageError ở nhánh giamdinh fail; trường hợp drop lẻ từng hồ sơ thì rỗng).
                try
                {
                    if (paramExcel != null && paramExcel.Messages != null && paramExcel.Messages.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(
                            "ExportExcel drop reasons (raw từ thư viện XML130): " + string.Join(" | ", paramExcel.Messages));
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                // Đối soát chi tiết theo mã hồ sơ điều trị: liệt kê mã CÓ trong Batch mà KHÔNG có trong Group
                // (file individual save OK nhưng thiếu row XML1) và ngược lại — để truy vết lệch chính xác.
                try
                {
                    LogBatchGroupMismatch();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                // Liệt kê chi tiết MA_DT của từng hồ sơ lỗi / không xuất được — để user xác định
                // chính xác hồ sơ nào trong tổng N hồ sơ bị thiếu file.
                try
                {
                    LogFailedRecords();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }

        // Tổng hợp + log danh sách MA_DT của các hồ sơ KHÔNG có file output (status != "OK")
        // chia theo lý do để user filter dễ. Status null = không thấy HOSO trong XML output (unaccounted).
        private void LogFailedRecords()
        {
            if (_recordStatus == null || listSelection == null) return;

            int total = _recordStatus.Length;
            var byReason = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            int failedCount = 0;
            int unaccountedCount = 0;

            for (int i = 0; i < total; i++)
            {
                string st = _recordStatus[i];
                if (st == "OK") continue;

                string code = (i < listSelection.Count && listSelection[i] != null)
                    ? listSelection[i].TREATMENT_CODE
                    : null;

                string reason;
                if (st == null)
                {
                    // Record null = HOSO không có trong XML output → bị thư viện His.Bhyt.ExportXml.XML130 loại
                    // ở khâu build XML (CreateHoSoPlus thất bại). Nguyên nhân thường gặp: thiếu thông tin địa chỉ
                    // bệnh nhân, hoặc không còn DVKT hợp lệ (giá > 0, có tên BHYT...). Lý do chi tiết từng hồ sơ
                    // nằm ở log 'Tao FileHoSo that bai' của thư viện (không kèm MA_DT).
                    reason = "Bị loại khi build XML130 — không có trong XML output (thường do thiếu địa chỉ BN / không có DVKT hợp lệ; xem log 'Tao FileHoSo that bai')";
                    unaccountedCount++;
                }
                else
                {
                    reason = st;
                    failedCount++;
                }

                List<string> bucket;
                if (!byReason.TryGetValue(reason, out bucket))
                {
                    bucket = new List<string>();
                    byReason[reason] = bucket;
                }
                bucket.Add("idx=" + i + " MA_DT=" + (code ?? "(null)"));
            }

            int totalProblem = failedCount + unaccountedCount;
            if (totalProblem == 0)
            {
                Inventec.Common.Logging.LogSystem.Info(
                    "ExportExcel: Tat ca " + total + " ho so da xuat thanh cong, khong co ho so loi.");
                return;
            }

            var sb = new StringBuilder();
            sb.Append("ExportExcel: TONG ").Append(totalProblem)
              .Append(" ho so co van de (").Append(failedCount).Append(" loi + ")
              .Append(unaccountedCount).Append(" khong tim thay HOSO) tren ")
              .Append(total).Append(" ho so. Chi tiet theo nhom:")
              .AppendLine();

            foreach (var kv in byReason)
            {
                sb.Append("--- [").Append(kv.Value.Count).Append("] ").Append(kv.Key).AppendLine(" ---");
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    sb.Append("  ").AppendLine(kv.Value[i]);
                }
            }
            Inventec.Common.Logging.LogSystem.Info("Log hồ sơ lỗi/thiếu hoàn tất.");
            Inventec.Common.Logging.LogSystem.Warn(sb.ToString());
        }

        // Đối soát theo MÃ HỒ SƠ ĐIỀU TRỊ giữa Batch (file individual save OK) và Group (row XML1):
        //   - batchOnly : có file trong Batch_NNN nhưng KHÔNG có row XML1 trong Group
        //   - groupOnly : có row XML1 trong Group nhưng KHÔNG có file trong Batch
        // Khi 2 fix đếm khớp hoạt động đúng → cả 2 danh sách rỗng.
        private void LogBatchGroupMismatch()
        {
            if (_recordInBatch == null || _recordInGroupXml1 == null) return;

            const int MaxList = 500; // cap số mã liệt kê mỗi chiều để log không quá lớn
            int total = _recordInBatch.Length;
            var batchOnly = new List<string>();
            var groupOnly = new List<string>();
            int batchOnlyTotal = 0, groupOnlyTotal = 0;

            for (int i = 0; i < total; i++)
            {
                bool inBatch = _recordInBatch[i];
                bool inGroup = _recordInGroupXml1[i];
                if (inBatch == inGroup) continue; // khớp (cùng có hoặc cùng không) → bỏ qua

                string code = (_recordTreatmentCode != null && i < _recordTreatmentCode.Length)
                    ? _recordTreatmentCode[i] : null;
                if (string.IsNullOrEmpty(code) && i < listSelection.Count && listSelection[i] != null)
                    code = listSelection[i].TREATMENT_CODE;
                string entry = "idx=" + i + " MA_DT=" + (code ?? "(null)");

                if (inBatch) // && !inGroup
                {
                    batchOnlyTotal++;
                    if (batchOnly.Count < MaxList) batchOnly.Add(entry);
                }
                else // inGroup && !inBatch
                {
                    groupOnlyTotal++;
                    if (groupOnly.Count < MaxList) groupOnly.Add(entry);
                }
            }

            if (batchOnlyTotal == 0 && groupOnlyTotal == 0)
            {
                Inventec.Common.Logging.LogSystem.Info(
                    "ExportExcel batch/group reconcile: KHỚP — mọi mã hồ sơ trong Batch đều có trong Group và ngược lại (" + total + " hồ sơ).");
                return;
            }

            var sb = new StringBuilder();
            sb.Append("ExportExcel batch/group reconcile: LECH — batch_only=").Append(batchOnlyTotal)
              .Append(" (co file Batch nhung thieu row Group XML1), group_only=").Append(groupOnlyTotal)
              .Append(" (co row Group XML1 nhung thieu file Batch).").AppendLine();

            if (batchOnlyTotal > 0)
            {
                sb.Append("--- [").Append(batchOnlyTotal).Append("] CHI CO TRONG BATCH (thieu o Group)");
                if (batchOnlyTotal > batchOnly.Count) sb.Append(" — liet ke ").Append(batchOnly.Count).Append(" ma dau");
                sb.AppendLine(" ---");
                for (int i = 0; i < batchOnly.Count; i++) sb.Append("  ").AppendLine(batchOnly[i]);
            }
            if (groupOnlyTotal > 0)
            {
                sb.Append("--- [").Append(groupOnlyTotal).Append("] CHI CO TRONG GROUP (thieu o Batch)");
                if (groupOnlyTotal > groupOnly.Count) sb.Append(" — liet ke ").Append(groupOnly.Count).Append(" ma dau");
                sb.AppendLine(" ---");
                for (int i = 0; i < groupOnly.Count; i++) sb.Append("  ").AppendLine(groupOnly[i]);
            }
            Inventec.Common.Logging.LogSystem.Warn(sb.ToString());
        }

        // Parse 1 file XML chunk → emit individual xlsx files va append vao group buckets.
        // globalIdx la ref de chunks ke tiep biet bat dau bucket nao (Batch_NNN = idx/1000).
        private void ParseChunkAndEmit(
            string xmlPath,
            BackgroundWorker worker,
            ref int globalIdx,
            int totalEstimate,
            HashSet<string> usedNames,
            Dictionary<string, GroupContext> groupBuckets,
            BlockingCollection<IndividualSaveJob> saveQueue)
        {
            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true,
                DtdProcessing = DtdProcessing.Ignore,
                CloseInput = true,
            };

            using (var fs = new FileStream(xmlPath, FileMode.Open, FileAccess.Read,
                FileShare.Read, FileReadBufferSize, FileOptions.SequentialScan))
            using (var reader = XmlReader.Create(fs, settings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    if (reader.Name != "HOSO") continue;

                    if (worker.CancellationPending) return;

                    His.Bhyt.ExportXml.XML130.XML.HoSo hoSo = null;
                    try
                    {
                        using (var subReader = reader.ReadSubtree())
                        {
                            subReader.MoveToContent();
                            hoSo = (His.Bhyt.ExportXml.XML130.XML.HoSo)HoSoSerializer.Deserialize(subReader);
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(
                            "HOSO deserialize failed at idx " + globalIdx + ": " + ex);
                        MarkRecordStatus(globalIdx, "HOSO deserialize failed: " + ex.Message);
                        globalIdx++;
                        continue;
                    }

                    if (hoSo != null)
                    {
                        _hosoTotal++;

                        if (hoSo.FILEHOSO == null || hoSo.FILEHOSO.Count == 0)
                        {
                            _hosoNoFilehoso++;
                            MarkRecordStatus(globalIdx, "FILEHOSO null/empty");
                        }
                        else
                        {
                            // Decode base64 NOIDUNGFILE per file (mirror GetDataFromString behavior)
                            foreach (var f in hoSo.FILEHOSO)
                            {
                                if (f != null && !string.IsNullOrWhiteSpace(f.NOIDUNGFILE))
                                {
                                    try
                                    {
                                        f.NOIDUNGFILE = Encoding.UTF8.GetString(
                                            Convert.FromBase64String(f.NOIDUNGFILE));
                                    }
                                    catch (Exception ex)
                                    {
                                        Inventec.Common.Logging.LogSystem.Warn(
                                            "Base64 decode failed for " + f.LOAIHOSO + ": " + ex.Message);
                                    }
                                }
                            }

                            // Bọc try/catch riêng từng hồ sơ: 1 hồ sơ lỗi (vd Aspose "Invalid formula")
                            // KHÔNG được làm vọt exception ra ngoài giết cả chunk (~1000 hồ sơ).
                            try
                            {
                                ProcessHoSo(hoSo, globalIdx, usedNames, groupBuckets, saveQueue);
                            }
                            catch (Exception exHoso)
                            {
                                Inventec.Common.Logging.LogSystem.Error(
                                    "ProcessHoSo failed at idx " + globalIdx + ": " + exHoso);
                                MarkRecordStatus(globalIdx, "ProcessHoSo failed: " + exHoso.Message);
                            }
                        }
                    }

                    globalIdx++;
                    if (globalIdx % ProgressEvery == 0)
                    {
                        int pct = totalEstimate > 0
                            ? (int)Math.Min(99, 100.0 * globalIdx / totalEstimate)
                            : 0;
                        worker.ReportProgress(pct, globalIdx + "/" + totalEstimate);
                    }
                }
            }
        }

        // (I) Lazy-create + cache bucket dir cho idx tương ứng
        private string GetBucketDir(int idx)
        {
            int bucketIdx = idx / FilesPerBucket;
            string bucketDir = Path.Combine(IndividualDir, "Batch_" + bucketIdx.ToString("D3"));
            if (!_createdBuckets.Contains(bucketIdx))
            {
                if (!Directory.Exists(bucketDir))
                    Directory.CreateDirectory(bucketDir);
                _createdBuckets.Add(bucketIdx);
            }
            return bucketDir;
        }

        // Workbook leak protection - dispose if exception happens before queue handoff
        private void ProcessHoSo(
            His.Bhyt.ExportXml.XML130.XML.HoSo hoSo,
            int idx,
            HashSet<string> usedNames,
            Dictionary<string, GroupContext> groupBuckets,
            BlockingCollection<IndividualSaveJob> saveQueue)
        {
            if (hoSo.FILEHOSO == null || hoSo.FILEHOSO.Count == 0) return;

            string treatmentCode = null;
            // Track XML1 entry/parse để đảm bảo 1 HOSO ↔ 1 row XML1 trong group
            bool hasXml1Entry = false;
            int xml1ItemCount = 0;
            bool xml1RowAppended = false;   // đã append 1 row XML1 (real/placeholder) vào group cho HOSO này
            Workbook wbInd = new Workbook();
            bool handedOff = false;
            try
            {
                bool firstSheet = true;

                foreach (var fileHoSo in hoSo.FILEHOSO)
                {
                    if (fileHoSo == null || string.IsNullOrEmpty(fileHoSo.LOAIHOSO)) continue;

                    bool isXml1 = string.Equals(fileHoSo.LOAIHOSO, "XML1", StringComparison.OrdinalIgnoreCase);
                    if (isXml1) hasXml1Entry = true;

                    string maLk;
                    List<object> items = ExtractItems(fileHoSo, out maLk);
                    if (!string.IsNullOrEmpty(maLk)) treatmentCode = maLk;

                    if (isXml1 && items != null) xml1ItemCount = items.Count;

                    Worksheet sheet;
                    if (firstSheet)
                    {
                        sheet = wbInd.Worksheets[0];
                        firstSheet = false;
                    }
                    else
                    {
                        int newIdx = wbInd.Worksheets.Add();
                        sheet = wbInd.Worksheets[newIdx];
                    }
                    sheet.Name = MakeUniqueSheetName(wbInd, fileHoSo.LOAIHOSO, sheet);
                    WriteSheet(sheet, items);

                    if (items != null && items.Count > 0)
                    {
                        var ctx = GetOrCreateGroup(groupBuckets, fileHoSo.LOAIHOSO);
                        AppendToGroupBuffer(ctx, items);
                        if (ctx.EffectiveRowCount >= RowLimitPerPart)
                            RotateGroup(ctx);
                        if (isXml1) xml1RowAppended = true;
                    }
                }

                // Reconciliation: đảm bảo Group XML1 có đúng 1 row mỗi HOSO có XML1 entry
                if (hasXml1Entry)
                {
                    if (xml1ItemCount > 0)
                    {
                        _hosoXml1Ok++;
                    }
                    else
                    {
                        // XML1 entry tồn tại nhưng parse fail (NOIDUNGFILE lỗi base64 hoặc serializer null)
                        // → append placeholder XML1Data với MA_LK + MA_HSBA="_PARSE_FAILED" để giữ count khớp
                        _hosoXml1ParseEmpty++;
                        try
                        {
                            var placeholder = new His.Bhyt.ExportXml.XML130.XML1.QD130.XML.XML1Data
                            {
                                MA_LK = treatmentCode ?? ("idx_" + idx),
                                MA_HSBA = "_PARSE_FAILED"
                            };
                            var ctx = GetOrCreateGroup(groupBuckets, "XML1");
                            AppendToGroupBuffer(ctx, new List<object> { placeholder });
                            if (ctx.EffectiveRowCount >= RowLimitPerPart)
                                RotateGroup(ctx);
                            xml1RowAppended = true;

                            Inventec.Common.Logging.LogSystem.Warn(
                                "ExportExcel: XML1 parse fail at idx " + idx + ", MA_LK=" + treatmentCode + " → placeholder appended");
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(
                                "ExportExcel: append XML1 placeholder failed at idx " + idx + ": " + ex);
                        }
                    }
                }
                else
                {
                    // HOSO không có entry XML1 (data thiếu XML1) — vẫn append placeholder để group XML1
                    // có đúng 1 row/HOSO, khớp với file individual (Batch) đã xuất. Đánh dấu _NO_XML1 để truy vết.
                    _hosoNoXml1Entry++;
                    try
                    {
                        var placeholder = new His.Bhyt.ExportXml.XML130.XML1.QD130.XML.XML1Data
                        {
                            MA_LK = treatmentCode ?? ("idx_" + idx),
                            MA_HSBA = "_NO_XML1"
                        };
                        var ctx = GetOrCreateGroup(groupBuckets, "XML1");
                        AppendToGroupBuffer(ctx, new List<object> { placeholder });
                        if (ctx.EffectiveRowCount >= RowLimitPerPart)
                            RotateGroup(ctx);
                        xml1RowAppended = true;

                        Inventec.Common.Logging.LogSystem.Warn(
                            "ExportExcel: HOSO idx " + idx + " no XML1 entry, MA_LK=" + treatmentCode + " → placeholder _NO_XML1 appended (giữ count khớp Batch)");
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(
                            "ExportExcel: append _NO_XML1 placeholder failed at idx " + idx + ": " + ex);
                    }
                }

                // Ghi nhận mã hồ sơ + trạng thái group XML1 theo idx để đối soát Batch vs Group cuối run
                if (idx >= 0 && _recordTreatmentCode != null && idx < _recordTreatmentCode.Length)
                {
                    _recordTreatmentCode[idx] = treatmentCode;
                    _recordInGroupXml1[idx] = xml1RowAppended;
                }

                string fileName = SanitizeFileName(treatmentCode, idx, usedNames);
                // (I) Subfolder bucketing — Batch_NNN/MA_LK.xlsx, mỗi folder tối đa FilesPerBucket file
                string bucketDir = GetBucketDir(idx);
                string filePath = Path.Combine(bucketDir, fileName + ".xlsx");
                saveQueue.Add(new IndividualSaveJob { Workbook = wbInd, FilePath = filePath, Idx = idx });
                handedOff = true;
            }
            finally
            {
                if (!handedOff)
                {
                    var disp = wbInd as IDisposable;
                    if (disp != null) try { disp.Dispose(); } catch { }
                }
            }
        }

        // (D) Saver consumer — runs on background thread with lower priority
        // onComplete(job, true, null) khi Save thành công, onComplete(job, false, ex) khi throw — null nếu không cần đếm
        private static void RunSaver(BlockingCollection<IndividualSaveJob> queue, Action<IndividualSaveJob, bool, Exception> onComplete)
        {
            try
            {
                try { Thread.CurrentThread.Priority = ThreadPriority.BelowNormal; } catch { }

                foreach (var job in queue.GetConsumingEnumerable())
                {
                    Workbook wb = job.Workbook;
                    bool ok = false;
                    Exception saveEx = null;
                    try
                    {
                        wb.Save(job.FilePath, SaveFormat.Xlsx);
                        ok = true;
                    }
                    catch (Exception ex)
                    {
                        saveEx = ex;
                        Inventec.Common.Logging.LogSystem.Error(
                            "Save failed: " + job.FilePath + " - " + ex);

                        // Fallback: Save lỗi (thường do tên/đường dẫn file quá dài hoặc ký tự lạ) →
                        // thử lưu lại với tên ngắn an toàn "idx_<Idx>.xlsx" trong CÙNG folder Batch_NNN.
                        // Đảm bảo số file Batch không thiếu so với số dòng group (group đã +row trước đó).
                        // Bỏ qua group save job (Idx = -1) — chỉ áp dụng cho individual file.
                        if (job.Idx >= 0)
                        {
                            try
                            {
                                string dir = Path.GetDirectoryName(job.FilePath);
                                if (!string.IsNullOrEmpty(dir))
                                {
                                    string fallbackPath = Path.Combine(dir, "idx_" + job.Idx + ".xlsx");
                                    wb.Save(fallbackPath, SaveFormat.Xlsx);
                                    ok = true;
                                    saveEx = null;
                                    Inventec.Common.Logging.LogSystem.Warn(
                                        "Save fallback OK: " + job.FilePath + " → " + fallbackPath);
                                }
                            }
                            catch (Exception exFb)
                            {
                                Inventec.Common.Logging.LogSystem.Error(
                                    "Save fallback failed: " + job.FilePath + " - " + exFb);
                            }
                        }
                    }
                    finally
                    {
                        var disp = wb as IDisposable;
                        if (disp != null) try { disp.Dispose(); } catch { }
                        if (onComplete != null)
                        {
                            try { onComplete(job, ok, saveEx); }
                            catch (Exception exCb) { Inventec.Common.Logging.LogSystem.Warn(exCb); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Thread-safe per-record status setter (array write atomic for reference type,
        // mỗi job giữ Idx duy nhất nên không có race per-index).
        private void MarkRecordStatus(int idx, string reason)
        {
            var arr = _recordStatus;
            if (arr == null) return;
            if (idx < 0 || idx >= arr.Length) return;
            arr[idx] = reason;
        }

        // ── vCong46659: hạ tầng đọc XML thô ──────────────────────────────────────────
        // LOAIHOSO → (kiểu dữ liệu của 1 dòng, tên thẻ XML lặp lại theo dòng).
        // Kiểu dữ liệu giữ nguyên như trước để số cột, thứ tự cột, tên cột của mọi sheet không đổi.
        private class RawSchema
        {
            public readonly Type RowType;
            public readonly string RowElement;

            public RawSchema(Type rowType, string rowElement)
            {
                this.RowType = rowType;
                this.RowElement = rowElement;
            }
        }

        private static readonly Dictionary<string, RawSchema> RawSchemas =
            new Dictionary<string, RawSchema>(StringComparer.OrdinalIgnoreCase)
            {
                { "XML1",  new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML1.QD130.XML.XML1Data), "TONG_HOP") },
                { "XML2",  new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML2.QD130.XML.XML2DetailData), "CHI_TIET_THUOC") },
                { "XML3",  new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML3.XML3DetailData), "CHI_TIET_DVKT") },
                { "XML4",  new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML4.QD130.XML.XML4DetailData), "CHI_TIET_CLS") },
                { "XML5",  new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML5.QD130.XML.XML5.XML5DetailData), "CHI_TIET_DIEN_BIEN_BENH") },
                { "XML6",  new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML6.QD130.XML.XML6DetailData), "HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS") },
                { "XML7",  new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML7.XML7Data), "CHI_TIEU_DU_LIEU_GIAY_RA_VIEN") },
                { "XML8",  new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML8.QD130.XML.XML8Data), "CHI_TIEU_DU_LIEU_TOM_TAT_HO_SO_BENH_AN") },
                { "XML9",  new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML9.QD130.XML.XML9.XML9DetailData), "DU_LIEU_GIAY_CHUNG_SINH") },
                { "XML10", new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML10.QD130.XML.XML10Data), "CHI_TIEU_DU_LIEU_GIAY_NGHI_DUONG_THAI") },
                { "XML11", new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML11.QD130.XML.XML11Data), "CHI_TIEU_DU_LIEU_GIAY_CHUNG_NHAN_NGHI_VIEC_HUONG_BAO_HIEM_XA_HOI") },
                { "XML12", new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML12.QD130.XML.XML12DetailData), "GIAM_DINH_Y_KHOA") },
                { "XML13", new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML13.QD130.XML.XML13Data), "CHI_TIEU_GIAYCHUYENTUYEN") },
                { "XML14", new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML14.QD130.XML.XML14Data), "CHI_TIEU_GIAYHEN_KHAMLAI") },
                { "XML15", new RawSchema(typeof(His.Bhyt.ExportXml.XML130.XML15.QD130.XML.XML15DetailData), "CHITIET_DIEUTRI_BENHLAO") },
            };

        // Setter đã compile cho từng thẻ dữ liệu — tránh reflection trong vòng lặp ghi hàng chục nghìn dòng
        private class RawSetter
        {
            public Action<object, object> Set;
            public bool IsCData;
            // Chỉ tiêu khai báo kiểu số (int/long/decimal…) cần đổi chuỗi trong thẻ XML sang đúng kiểu
            // trước khi gán. null = giữ nguyên văn chuỗi (string).
            public Func<string, object> Convert;
        }

        // Tra theo TÊN THẺ để mỗi dòng chỉ duyệt danh sách thẻ con đúng 1 lượt
        private static readonly ConcurrentDictionary<Type, Dictionary<string, RawSetter>> RawSetterCache
            = new ConcurrentDictionary<Type, Dictionary<string, RawSetter>>();

        private static readonly ConcurrentDictionary<Type, Func<object>> RawFactoryCache
            = new ConcurrentDictionary<Type, Func<object>>();

        // XmlDocument chỉ dùng để tạo node CDATA. Không thread-safe nên gắn theo thread.
        [ThreadStatic]
        private static XmlDocument _cdataOwnerDoc;

        private static Dictionary<string, RawSetter> GetRawSetters(Type t)
        {
            return RawSetterCache.GetOrAdd(t, type =>
            {
                var props = type.GetProperties();
                var setters = new Dictionary<string, RawSetter>(props.Length, StringComparer.Ordinal);
                for (int i = 0; i < props.Length; i++)
                {
                    var prop = props[i];
                    var setMethod = prop.GetSetMethod();
                    if (setMethod == null) continue;

                    bool isCData = prop.PropertyType == typeof(XmlCDataSection);

                    // vCong46659 chỉ nhận string + CDATA nên MỌI chỉ tiêu khai báo kiểu số bị bỏ qua,
                    // không bao giờ được gán → ô Excel ra giá trị mặc định của kiểu (0 hoặc rỗng):
                    //   XML1: STT (int), SO_NGAY_DTRI (long)   XML3: STT (int), MA_PTTT (int)
                    //   XML4/XML5: STT (int)                   XML6: 4 chỉ tiêu long?
                    //   XML7: SO_NGAY_NGHI (int)
                    // Bổ sung converter để các chỉ tiêu này đọc đúng giá trị trong thẻ XML.
                    Func<string, object> convert = null;
                    if (!isCData && prop.PropertyType != typeof(string))
                    {
                        convert = BuildRawConverter(prop.PropertyType);
                        if (convert == null) continue;
                    }
                    if (setters.ContainsKey(prop.Name)) continue;

                    var objParam = Expression.Parameter(typeof(object), "obj");
                    var valParam = Expression.Parameter(typeof(object), "val");
                    var body = Expression.Call(
                        Expression.Convert(objParam, type),
                        setMethod,
                        Expression.Convert(valParam, prop.PropertyType));

                    setters.Add(prop.Name, new RawSetter
                    {
                        Set = Expression.Lambda<Action<object, object>>(body, objParam, valParam).Compile(),
                        IsCData = isCData,
                        Convert = convert
                    });
                }
                return setters;
            });
        }

        // Đổi nguyên văn chuỗi trong thẻ XML sang kiểu khai báo của property.
        // Trả null khi thẻ rỗng hoặc không đổi được → giữ giá trị mặc định, KHÔNG ném exception
        // để 1 thẻ sai định dạng không làm hỏng cả dòng.
        private static Func<string, object> BuildRawConverter(Type propType)
        {
            Type target = Nullable.GetUnderlyingType(propType) ?? propType;
            if (!typeof(IConvertible).IsAssignableFrom(target)) return null;

            return text =>
            {
                if (string.IsNullOrWhiteSpace(text)) return null;
                try
                {
                    return System.Convert.ChangeType(text.Trim(), target, CultureInfo.InvariantCulture);
                }
                catch
                {
                    return null;
                }
            };
        }

        private static Func<object> GetRawFactory(Type t)
        {
            return RawFactoryCache.GetOrAdd(t, type =>
                Expression.Lambda<Func<object>>(
                    Expression.Convert(Expression.New(type), typeof(object))).Compile());
        }

        private static XmlCDataSection MakeCData(string text)
        {
            if (_cdataOwnerDoc == null) _cdataOwnerDoc = new XmlDocument();
            return _cdataOwnerDoc.CreateCDataSection(text ?? string.Empty);
        }

        // Cắt BOM bằng so sánh Ordinal.
        // KHÔNG dùng lại RemoveByteOrderMark hiện có: hàm đó so sánh theo culture, mà BOM là ký tự
        // trọng số 0 nên StartsWith luôn trả true → cắt nhầm ký tự '<' đầu tiên khi chuỗi không có BOM.
        private static string StripByteOrderMark(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return xml;
            string bom = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            return xml.StartsWith(bom, StringComparison.Ordinal) ? xml.Remove(0, bom.Length) : xml;
        }

        // Đọc từng dòng dữ liệu từ chuỗi XML, lấy NGUYÊN VĂN nội dung mỗi thẻ con.
        // Thẻ không có trong XML → để null (ô Excel trống), giống kết quả deserialize trước đây.
        private List<object> ReadRowsRaw(string xmlString, RawSchema schema, out string maLk)
        {
            maLk = null;
            var rows = new List<object>();
            if (string.IsNullOrEmpty(xmlString) || schema == null) return rows;

            var setters = GetRawSetters(schema.RowType);
            var factory = GetRawFactory(schema.RowType);

            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true,
                CheckCharacters = false,
                DtdProcessing = DtdProcessing.Prohibit
            };

            // Đọc streaming (không dựng DOM) — nhanh hơn và ít cấp phát khi xuất hàng chục nghìn hồ sơ
            using (var textReader = new StringReader(StripByteOrderMark(xmlString)))
            using (var reader = XmlReader.Create(textReader, settings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    if (!string.Equals(reader.Name, schema.RowElement, StringComparison.Ordinal)) continue;

                    object row = factory();
                    if (!reader.IsEmptyElement)
                        ReadRowFields(reader, setters, row, ref maLk);
                    rows.Add(row);
                }
            }
            return rows;
        }

        // Đọc các thẻ con của 1 dòng. Thẻ lạ (không có trong kiểu dữ liệu) được bỏ qua nguyên cụm.
        private static void ReadRowFields(
            XmlReader reader, Dictionary<string, RawSetter> setters, object row, ref string maLk)
        {
            int rowDepth = reader.Depth;
            bool alreadyAdvanced = false;

            while (true)
            {
                if (!alreadyAdvanced && !reader.Read()) break;
                alreadyAdvanced = false;

                if (reader.NodeType == XmlNodeType.EndElement && reader.Depth == rowDepth) break;
                if (reader.NodeType != XmlNodeType.Element) continue;

                string name = reader.Name;
                RawSetter setter;
                if (!setters.TryGetValue(name, out setter))
                {
                    if (!reader.IsEmptyElement)
                    {
                        reader.Skip();
                        alreadyAdvanced = true;
                    }
                    continue;
                }

                string text;
                if (reader.IsEmptyElement)
                {
                    text = string.Empty;
                }
                else
                {
                    text = reader.ReadElementContentAsString();
                    alreadyAdvanced = true;
                }

                object value;
                if (setter.IsCData) value = MakeCData(text);
                else if (setter.Convert != null) value = setter.Convert(text);
                else value = text;

                // Chỉ tiêu số có thẻ rỗng/sai định dạng → bỏ qua, giữ giá trị mặc định của kiểu
                if (value != null) setter.Set(row, value);

                if (maLk == null && text.Length > 0
                    && string.Equals(name, "MA_LK", StringComparison.Ordinal))
                    maLk = text;
            }
        }

        // vCong46659: đọc XML THÔ — giữ nguyên văn giá trị như trong file XML đã sinh.
        //
        // KHÔNG dùng RunXmlNData/LoadFromXMLString của thư viện XML130 nữa: các hàm đó chạy ConvertValue
        // ngay sau khi deserialize, đổi ngày giờ sang dd/MM/yyyy ở 13/15 nhóm hồ sơ (chỉ XML3, XML7 giữ
        // nguyên) và có trường hợp trả rỗng khi chuỗi ngày ngắn hơn 12 ký tự → Excel lệch XML.
        // Thư viện KHÔNG sửa vì màn Xem XML 130 (XMLViewer130) đang cần hiển thị dd/MM/yyyy trên lưới.
        private List<object> ExtractItems(His.Bhyt.ExportXml.XML130.XML.FileHoSo fileHoSo, out string maLk)
        {
            maLk = null;
            var listObj = new List<object>();
            try
            {
                RawSchema schema;
                if (fileHoSo.LOAIHOSO == null
                    || !RawSchemas.TryGetValue(fileHoSo.LOAIHOSO, out schema))
                    return listObj;

                string maLkRead;
                listObj = ReadRowsRaw(fileHoSo.NOIDUNGFILE, schema, out maLkRead);

                // Giữ đúng hành vi cũ: chỉ XML1 cung cấp MA_LK để đặt tên file và đối soát Batch/Group
                if (string.Equals(fileHoSo.LOAIHOSO, "XML1", StringComparison.OrdinalIgnoreCase))
                    maLk = maLkRead;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("ExtractItems failed for " + fileHoSo.LOAIHOSO + ": " + ex);
            }
            return listObj;
        }

        // Bulk import + Compiled accessors + Combined cache (for individual files only)
        // Aspose ImportTwoDimensionArray coi chuỗi bắt đầu bằng '=' là công thức (formula) → ném
        // "Error in Cell: ... Invalid formula" nếu không parse được. Thêm quote-prefix (') để Aspose
        // ghi đúng nội dung dạng TEXT (dấu nháy bị ẩn, dữ liệu gốc giữ nguyên hiển thị).
        private static object NeutralizeFormula(object value)
        {
            string s = value as string;
            if (!string.IsNullOrEmpty(s) && s[0] == '=')
                return "'" + s;
            return value;
        }

        private void WriteSheet(Worksheet sheet, List<object> items)
        {
            if (items == null || items.Count == 0) return;
            var meta = GetCachedMeta(items[0].GetType());
            int cols = meta.Properties.Length;
            int rows = items.Count + 1;

            object[,] data = new object[rows, cols];
            for (int col = 0; col < cols; col++)
                data[0, col] = meta.Properties[col].Name;

            for (int row = 0; row < items.Count; row++)
            {
                var obj = items[row];
                if (obj == null) continue;
                for (int col = 0; col < cols; col++)
                {
                    object value;
                    try { value = meta.Accessors[col](obj); }
                    catch { value = null; }
                    var cdata = value as XmlCDataSection;
                    if (cdata != null) value = cdata.Value;

                    data[row + 1, col] = NeutralizeFormula(value);
                }
            }

            sheet.Cells.ImportTwoDimensionArray(data, 0, 0);
            ApplyColumnStyles(sheet, meta);
        }

        private GroupContext GetOrCreateGroup(Dictionary<string, GroupContext> buckets, string loaiHoSo)
        {
            GroupContext ctx;
            if (!buckets.TryGetValue(loaiHoSo, out ctx))
            {
                ctx = new GroupContext
                {
                    LoaiHoSo = loaiHoSo,
                    // Dung GroupDir (= runDir) thay PathTempXml — group file co lap per-run
                    OutDir = GroupDir ?? PathTempXml,
                    PartIdx = 1,
                };
                ctx.OpenNewWorkbook();
                buckets[loaiHoSo] = ctx;
            }
            return ctx;
        }

        // (C) Pre-allocated 2D buffer thay List<object[]> — giảm 22M allocations xuống 15 cho 50k hồ sơ
        private void AppendToGroupBuffer(GroupContext ctx, List<object> items)
        {
            if (items == null || items.Count == 0) return;

            if (ctx.Meta == null)
            {
                Type t = items[0].GetType();
                ctx.Meta = GetCachedMeta(t);
                int hcols = ctx.Meta.Properties.Length;

                // Write header (1 row × cols)
                object[,] header = new object[1, hcols];
                for (int col = 0; col < hcols; col++)
                    header[0, col] = ctx.Meta.Properties[col].Name;
                ctx.Sheet.Cells.ImportTwoDimensionArray(header, 0, 0);

                // Apply column-level format ngay sau header — new cells imported sau đó sẽ kế thừa
                ApplyColumnStyles(ctx.Sheet, ctx.Meta);

                // (C) Lazy alloc reusable 2D buffer (FlushThreshold × cols)
                ctx.BufferArray = new object[GroupBufferFlushThreshold, hcols];
                ctx.BufferRowCount = 0;
            }

            int cols = ctx.Meta.Properties.Length;
            for (int i = 0; i < items.Count; i++)
            {
                var obj = items[i];
                if (obj == null) continue;
                for (int col = 0; col < cols; col++)
                {
                    object value;
                    try { value = ctx.Meta.Accessors[col](obj); }
                    catch { value = null; }
                    var cdata = value as XmlCDataSection;
                    if (cdata != null) value = cdata.Value;

                    ctx.BufferArray[ctx.BufferRowCount, col] = NeutralizeFormula(value);
                }
                ctx.BufferRowCount++;

                if (ctx.BufferRowCount >= GroupBufferFlushThreshold)
                    FlushGroup(ctx);
            }
        }

        // (C) Flush buffer to sheet — reuse same array next time, only allocate small slice for partial flush
        private void FlushGroup(GroupContext ctx)
        {
            if (ctx.Meta == null || ctx.BufferRowCount == 0) return;
            int cols = ctx.Meta.Properties.Length;
            int n = ctx.BufferRowCount;

            object[,] toImport;
            if (n == GroupBufferFlushThreshold)
            {
                // Full buffer — reuse same array (Aspose copies values internally)
                toImport = ctx.BufferArray;
            }
            else
            {
                // Partial — allocate small slice
                toImport = new object[n, cols];
                for (int r = 0; r < n; r++)
                    for (int c = 0; c < cols; c++)
                        toImport[r, c] = ctx.BufferArray[r, c];
            }

            ctx.Sheet.Cells.ImportTwoDimensionArray(toImport, ctx.RowCount + 1, 0);
            ctx.RowCount += n;

            // Clear references in buffer for GC (but don't reallocate)
            Array.Clear(ctx.BufferArray, 0, n * cols);
            ctx.BufferRowCount = 0;
        }

        private void RotateGroup(GroupContext ctx)
        {
            FlushGroup(ctx);
            ctx.Save();
            ctx.Dispose();
            ctx.PartIdx++;
            ctx.RowCount = 0;
            ctx.Meta = null;
            ctx.BufferArray = null;
            ctx.BufferRowCount = 0;
            ctx.OpenNewWorkbook();
        }

        // Combined property + accessor cache - single dict lookup
        private static TypeMeta GetCachedMeta(Type t)
        {
            return TypeMetaCache.GetOrAdd(t, type =>
            {
                var props = type.GetProperties();
                var accessors = new Func<object, object>[props.Length];
                var specs = new ExcelColumnSpec[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    var prop = props[i];
                    if (!prop.CanRead || prop.GetGetMethod() == null)
                    {
                        accessors[i] = _ => null;
                    }
                    else
                    {
                        var paramExpr = Expression.Parameter(typeof(object), "obj");
                        Expression body = Expression.Property(
                            Expression.Convert(paramExpr, type), prop);
                        if (body.Type != typeof(object))
                            body = Expression.Convert(body, typeof(object));
                        accessors[i] = Expression.Lambda<Func<object, object>>(body, paramExpr).Compile();
                    }
                    specs[i] = BuildExcelColumnSpec(prop);
                }
                return new TypeMeta { Properties = props, Accessors = accessors, ColumnSpecs = specs };
            });
        }

        // Heuristic: PropertyType → Excel NumberFormat
        //
        // vCong46659: mọi chỉ tiêu XML130/3176 đều là chuỗi → ghi TEXT, giữ nguyên văn giá trị như XML.
        // Trước đây các cột tiền (T_*, THANH_TIEN*, DON_GIA, SO_LUONG, TYLE_*, MUC_HUONG) bị convert
        // sang double và để format General → Excel tự làm tròn theo độ rộng cột, số ≥ 12 chữ số ra dạng
        // khoa học (1.23457E+11). Bỏ hẳn phần convert đó để Excel khớp XML từng ký tự.
        private static ExcelColumnSpec BuildExcelColumnSpec(PropertyInfo p)
        {
            Type t = p.PropertyType;

            // Numeric ints → "0"
            if (t == typeof(int) || t == typeof(short) || t == typeof(byte)
                || t == typeof(uint) || t == typeof(ushort)
                || t == typeof(long) || t == typeof(ulong))
                return new ExcelColumnSpec { NumberFormat = "0" };

            // Floating → "#,##0.##"
            if (t == typeof(double) || t == typeof(float) || t == typeof(decimal))
                return new ExcelColumnSpec { NumberFormat = "#,##0.##" };

            // DateTime → date format
            if (t == typeof(DateTime) || t == typeof(DateTime?))
                return new ExcelColumnSpec { NumberFormat = "yyyy-mm-dd hh:mm:ss" };

            // string + XmlCDataSection (cdata.Value đã unwrap khi ghi cell) → text "@"
            // Chống Excel auto-convert dãy số dài, ngày dạng yyyyMMddHHmm, mã có số 0 ở đầu.
            if (t == typeof(string) || t == typeof(XmlCDataSection))
                return new ExcelColumnSpec { NumberFormat = "@" };

            // Fallback: không set format
            return new ExcelColumnSpec { NumberFormat = null };
        }

        // Apply Style.Custom + StyleFlag.NumberFormat cho từng column. O(cols), không loop cell.
        private static void ApplyColumnStyles(Worksheet sheet, TypeMeta meta)
        {
            if (sheet == null || meta == null || meta.ColumnSpecs == null) return;
            var flag = new StyleFlag { NumberFormat = true };
            int cols = meta.ColumnSpecs.Length;
            for (int c = 0; c < cols; c++)
            {
                string fmt = meta.ColumnSpecs[c].NumberFormat;
                if (string.IsNullOrEmpty(fmt)) continue;
                try
                {
                    var col = sheet.Cells.Columns[c];
                    var style = col.Style;
                    style.Custom = fmt;
                    col.ApplyStyle(style, flag);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "ApplyColumnStyles col=" + c + " fmt=" + fmt + " - " + ex.Message);
                }
            }
        }

        // HashSet<char> for O(1) invalid char lookup
        private static string SanitizeFileName(string raw, int idx, HashSet<string> usedNames)
        {
            string name;
            if (string.IsNullOrWhiteSpace(raw))
            {
                name = "idx_" + idx;
            }
            else
            {
                var sb = new StringBuilder(raw.Length);
                foreach (var c in raw)
                    sb.Append(InvalidFileNameChars.Contains(c) ? '_' : c);
                name = sb.ToString().Trim();
                if (string.IsNullOrEmpty(name)) name = "idx_" + idx;
            }

            if (usedNames.Contains(name))
            {
                int suffix = 2;
                string candidate;
                do
                {
                    candidate = name + "_" + suffix;
                    suffix++;
                } while (usedNames.Contains(candidate));
                name = candidate;
            }
            usedNames.Add(name);
            return name;
        }

        private static string MakeUniqueSheetName(Workbook wb, string desiredName, Worksheet currentSheet)
        {
            string trimmed = (desiredName ?? string.Empty).Trim();
            if (trimmed.Length == 0) trimmed = "Sheet";
            if (trimmed.Length > 31) trimmed = trimmed.Substring(0, 31);

            string candidate = trimmed;
            int suffix = 2;
            while (true)
            {
                bool clash = false;
                for (int i = 0; i < wb.Worksheets.Count; i++)
                {
                    var s = wb.Worksheets[i];
                    if (ReferenceEquals(s, currentSheet)) continue;
                    if (string.Equals(s.Name, candidate, StringComparison.OrdinalIgnoreCase))
                    {
                        clash = true;
                        break;
                    }
                }
                if (!clash) return candidate;
                string suffixStr = "_" + suffix;
                candidate = (trimmed.Length + suffixStr.Length > 31
                    ? trimmed.Substring(0, 31 - suffixStr.Length)
                    : trimmed) + suffixStr;
                suffix++;
            }
        }

        public void DeleteXmlFilesInPathTempXml()
        {
            try
            {
                if (string.IsNullOrEmpty(PathTempXml) || !Directory.Exists(PathTempXml))
                    return;

                // Xóa group files top-level
                var xlsxFiles = Directory.GetFiles(PathTempXml, "*.xlsx", SearchOption.TopDirectoryOnly);
                foreach (var file in xlsxFiles)
                {
                    try { File.Delete(file); }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
                }

                // (I) Xóa toàn bộ Individual/ (recursive — bao gồm các Batch_NNN subfolder)
                string indDir = Path.Combine(PathTempXml, "Individual");
                if (Directory.Exists(indDir))
                {
                    try { Directory.Delete(indDir, true); }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Gop tat ca file Group_LOAIHOSO_PartN.xlsx cung Part vao 1 file Group_PartN.xlsx
        // Moi LOAIHOSO -> 1 sheet trong file dich.
        private void MergeGroupFilesByPart()
        {
            try
            {
                if (string.IsNullOrEmpty(GroupDir) || !Directory.Exists(GroupDir)) return;

                // Pattern strict: Group_LOAIHOSO_PartN.xlsx (KHONG match Group_PartN.xlsx)
                var srcFiles = Directory.GetFiles(GroupDir, "Group_*_Part*.xlsx", SearchOption.TopDirectoryOnly);
                if (srcFiles.Length == 0) return;

                var rgx = new Regex(@"^Group_(.+?)_Part(\d+)\.xlsx$", RegexOptions.IgnoreCase);
                var partMap = new Dictionary<int, List<Tuple<string, string>>>();

                foreach (var fp in srcFiles)
                {
                    string fn = Path.GetFileName(fp);
                    var m = rgx.Match(fn);
                    if (!m.Success) continue;
                    string loai = m.Groups[1].Value;
                    int part;
                    if (!int.TryParse(m.Groups[2].Value, out part)) continue;

                    List<Tuple<string, string>> list;
                    if (!partMap.TryGetValue(part, out list))
                    {
                        list = new List<Tuple<string, string>>();
                        partMap[part] = list;
                    }
                    list.Add(Tuple.Create(loai, fp));
                }

                foreach (var kv in partMap)
                {
                    int partIdx = kv.Key;
                    var entries = kv.Value;
                    // Sort theo LOAIHOSO de sheet order on dinh
                    entries.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.Item1, b.Item1));

                    //string destPath = Path.Combine(GroupDir, "Group_Part" + partIdx + ".xlsx");
                    // Ten file: Group_<yyyyMMdd_HHmmss>.xlsx (1 part) hoac Group_<stamp>_PartN.xlsx (>=2 parts)
                    string stamp = string.IsNullOrEmpty(GroupRunStamp)
                        ? DateTime.Now.ToString("yyyyMMdd_HHmmss")
                        : GroupRunStamp;
                    string destFileName = partMap.Count > 1
                        ? "Group_" + stamp + "_Part" + partIdx + ".xlsx"
                        : "Group_" + stamp + ".xlsx";
                    string destPath = Path.Combine(GroupDir, destFileName);
                    Workbook dest = null;
                    bool saveSuccess = false;
                    try
                    {
                        dest = new Workbook();
                        dest.Worksheets.Clear();

                        foreach (var entry in entries)
                        {
                            string loai = entry.Item1;
                            string srcPath = entry.Item2;
                            Workbook src = null;
                            try
                            {
                                src = new Workbook(srcPath);
                                foreach (Worksheet srcSheet in src.Worksheets)
                                {
                                    string targetName = string.IsNullOrEmpty(loai) ? "Sheet" : loai;
                                    if (targetName.Length > 31) targetName = targetName.Substring(0, 31);

                                    string finalName = targetName;
                                    int suffix = 2;
                                    while (dest.Worksheets[finalName] != null)
                                    {
                                        string baseName = targetName;
                                        string suf = "_" + suffix;
                                        if (baseName.Length + suf.Length > 31)
                                            baseName = baseName.Substring(0, 31 - suf.Length);
                                        finalName = baseName + suf;
                                        suffix++;
                                    }

                                    int newIdx = dest.Worksheets.Add();
                                    Worksheet newSheet = dest.Worksheets[newIdx];
                                    newSheet.Copy(srcSheet);
                                    newSheet.Name = finalName;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(
                                    "MergeGroupFilesByPart: load src failed " + srcPath + " - " + ex);
                            }
                            finally
                            {
                                var disp = src as IDisposable;
                                if (disp != null) try { disp.Dispose(); } catch { }
                            }
                        }

                        // Neu khong co sheet nao copy duoc -> them sheet trong de file hop le
                        if (dest.Worksheets.Count == 0)
                        {
                            int idx0 = dest.Worksheets.Add();
                            dest.Worksheets[idx0].Name = "Empty";
                        }

                        dest.Save(destPath, SaveFormat.Xlsx);
                        saveSuccess = true;
                        Inventec.Common.Logging.LogSystem.Info(
                            "MergeGroupFilesByPart: saved " + destPath + " with " + dest.Worksheets.Count + " sheets");
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(
                            "MergeGroupFilesByPart Part" + partIdx + " failed: " + ex);
                    }
                    finally
                    {
                        var disp = dest as IDisposable;
                        if (disp != null) try { disp.Dispose(); } catch { }
                    }

                    // Chi xoa file goc neu save dich thanh cong
                    if (saveSuccess)
                    {
                        foreach (var entry in entries)
                        {
                            try { File.Delete(entry.Item2); }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(
                                    "Cannot delete src group file " + entry.Item2 + ": " + ex.Message);
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

        // Gop tat ca file XML tung lo (__xmlpart_*.xml, moi file la 1 GIAMDINHHS) thanh 1 file
        // Group_<stamp>.xml DUY NHAT chua TAT CA ho so. Streaming doc + ghi -> RAM thap, chiu volume lon.
        // Ten file trung base name voi file Excel Group (Group_<GroupRunStamp>).
        private void MergeXmlPartsIntoGroup()
        {
            if (_xmlPartPaths == null || _xmlPartPaths.Count == 0) return;

            string stamp = string.IsNullOrEmpty(GroupRunStamp)
                ? DateTime.Now.ToString("yyyyMMdd_HHmmss")
                : GroupRunStamp;
            string destPath = Path.Combine(GroupDir ?? PathTempXml, "Group_" + stamp + ".xml");

            var readerSettings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true,
                DtdProcessing = DtdProcessing.Ignore,
                CloseInput = true,
            };

            // Pass 1: lay MACSKCB + NGAYLAP tu part dau tien doc duoc, dem tong so <HOSO> tren TAT CA part.
            string maCskcb = null;
            string ngayLap = null;
            int totalHoso = 0;
            bool headerCaptured = false;
            foreach (var part in _xmlPartPaths)
            {
                if (string.IsNullOrEmpty(part) || !File.Exists(part)) continue;
                try
                {
                    using (var fs = new FileStream(part, FileMode.Open, FileAccess.Read,
                        FileShare.Read, FileReadBufferSize, FileOptions.SequentialScan))
                    using (var r = XmlReader.Create(fs, readerSettings))
                    {
                        r.MoveToContent();
                        while (!r.EOF)
                        {
                            if (r.NodeType == XmlNodeType.Element)
                            {
                                if (r.Name == "HOSO")
                                {
                                    totalHoso++;
                                    r.Skip(); // bo qua noi dung HOSO (base64 lon), chi dem
                                    continue;
                                }
                                if (!headerCaptured && r.Name == "MACSKCB")
                                {
                                    maCskcb = r.ReadElementContentAsString();
                                    continue;
                                }
                                if (!headerCaptured && r.Name == "NGAYLAP")
                                {
                                    ngayLap = r.ReadElementContentAsString();
                                    continue;
                                }
                            }
                            r.Read();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error("MergeXmlPartsIntoGroup count pass failed for " + part + ": " + ex);
                }
                headerCaptured = true; // chi lay header tu part dau tien doc duoc
            }

            var writerSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = false,
            };

            int writtenHoso = 0;
            try
            {
                using (var w = XmlWriter.Create(destPath, writerSettings))
                {
                    w.WriteStartDocument();
                    w.WriteStartElement("GIAMDINHHS");

                    w.WriteStartElement("THONGTINDONVI");
                    w.WriteElementString("MACSKCB", maCskcb ?? "");
                    w.WriteEndElement(); // THONGTINDONVI

                    w.WriteStartElement("THONGTINHOSO");
                    w.WriteElementString("NGAYLAP",
                        string.IsNullOrEmpty(ngayLap) ? DateTime.Now.ToString("yyyyMMdd") : ngayLap);
                    w.WriteElementString("SOLUONGHOSO", totalHoso.ToString());
                    w.WriteStartElement("DANHSACHHOSO");

                    foreach (var part in _xmlPartPaths)
                    {
                        if (string.IsNullOrEmpty(part) || !File.Exists(part)) continue;
                        try
                        {
                            using (var fs = new FileStream(part, FileMode.Open, FileAccess.Read,
                                FileShare.Read, FileReadBufferSize, FileOptions.SequentialScan))
                            using (var r = XmlReader.Create(fs, readerSettings))
                            {
                                while (r.Read())
                                {
                                    if (r.NodeType != XmlNodeType.Element || r.Name != "HOSO") continue;
                                    using (var sub = r.ReadSubtree())
                                    {
                                        sub.MoveToContent();
                                        w.WriteNode(sub, false); // copy nguyen subtree HOSO, base64 giu nguyen
                                    }
                                    writtenHoso++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error("MergeXmlPartsIntoGroup write pass failed for " + part + ": " + ex);
                        }
                    }

                    w.WriteEndElement(); // DANHSACHHOSO
                    w.WriteEndElement(); // THONGTINHOSO
                    w.WriteEndElement(); // GIAMDINHHS
                    w.WriteEndDocument();
                }

                Inventec.Common.Logging.LogSystem.Info(
                    "MergeXmlPartsIntoGroup: saved " + destPath + " (hoso written=" + writtenHoso + ", counted=" + totalHoso + ")");
                if (writtenHoso != totalHoso)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "MergeXmlPartsIntoGroup: SOLUONGHOSO=" + totalHoso + " khac so HOSO da ghi=" + writtenHoso);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("MergeXmlPartsIntoGroup write failed: " + ex);
            }

            // Xoa cac file part tam sau khi gop xong
            foreach (var part in _xmlPartPaths)
            {
                try { if (!string.IsNullOrEmpty(part) && File.Exists(part)) File.Delete(part); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn("Delete xml part failed " + part + ": " + ex.Message); }
            }
        }

        private void SetLicenseForAsposeCell()
        {
            try
            {
                if (!string.IsNullOrEmpty(Aspose_Key))
                {
                    using (var stream = new MemoryStream(Convert.FromBase64String(Aspose_Key)))
                    {
                        var license = new Aspose.Cells.License();
                        license.SetLicense(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // (H) DEFERRED — Aspose ILightCellsDataProvider streaming write cho group files >100k rows.
        // Cần verify chính xác API của Aspose.Cells 8.3.2.1 (interface methods khác giữa các version).
        // Hiệu quả: giảm RAM group workbook 95% (1GB → 50MB), cho phép xuất 500k+ trên RAM 8GB.
        // Khi enable: thay ctx.Save() bằng SaveWithLightProvider(ctx) trong RunSaver,
        // implement class GroupLightProvider : ILightCellsDataProvider để stream từng row.

        private class TypeMeta
        {
            public PropertyInfo[] Properties;
            public Func<object, object>[] Accessors;
            public ExcelColumnSpec[] ColumnSpecs;
        }

        // Excel column format
        private class ExcelColumnSpec
        {
            public string NumberFormat;                  // "@", "0", "#,##0", … set vào Style.Custom
        }

        private class IndividualSaveJob
        {
            public Workbook Workbook;
            public string FilePath;
            // Idx khớp listSelection để callback mark status đúng record.
            // -1 cho group save job (không cần track per-record).
            public int Idx = -1;
        }

        private class GroupContext : IDisposable
        {
            public string LoaiHoSo;
            public string OutDir;
            public Workbook Workbook;
            public Worksheet Sheet;
            public TypeMeta Meta;
            // (C) Pre-allocated 2D buffer thay List<object[]> — reuse, không alloc per-row
            public object[,] BufferArray;
            public int BufferRowCount;
            public int RowCount;
            public int PartIdx;

            public int EffectiveRowCount { get { return RowCount + BufferRowCount; } }

            public string FilePath
            {
                get { return Path.Combine(OutDir, "Group_" + LoaiHoSo + "_Part" + PartIdx + ".xlsx"); }
            }

            public void OpenNewWorkbook()
            {
                Workbook = new Workbook();
                Sheet = Workbook.Worksheets[0];
                string sheetName = LoaiHoSo;
                if (sheetName != null && sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);
                Sheet.Name = string.IsNullOrEmpty(sheetName) ? "Sheet" : sheetName;
            }

            public void Save()
            {
                if (Workbook == null) return;
                Workbook.Save(FilePath, SaveFormat.Xlsx);
            }

            public void Dispose()
            {
                var disp = Workbook as IDisposable;
                if (disp != null)
                {
                    try { disp.Dispose(); } catch { }
                }
                Workbook = null;
                Sheet = null;
                BufferArray = null;
            }
        }
    }
}
