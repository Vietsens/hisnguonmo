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

        // Reconciliation counters — verify Individual files vs Group XML1 rows
        private int _hosoTotal;            // Tổng HOSO đã đọc từ XML chunks
        private int _hosoNoFilehoso;       // HOSO có FILEHOSO null/empty → skip individual + group
        private int _hosoNoXml1Entry;      // Có FILEHOSO nhưng KHÔNG có entry LOAIHOSO=XML1
        private int _hosoXml1ParseEmpty;   // Có XML1 entry nhưng ExtractItems rỗng → placeholder appended
        private int _hosoXml1Ok;           // XML1 valid, items.Count > 0 → group XML1 +1 (real)
        private int _individualSaved;      // Saver Save() success
        private int _individualSaveFailed; // Saver Save() throw

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
                        "Bạn đang xuất {0:N0} hồ sơ (chia thành {1} thư mục con Batch_NNN, mỗi thư mục {2:N0} file).\nƯớc lượng thời gian: {3}-{4} phút, dung lượng ~{5:N0} MB.\n\nTiếp tục?",
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

                // Reset reconciliation counters
                _hosoTotal = 0;
                _hosoNoFilehoso = 0;
                _hosoNoXml1Entry = 0;
                _hosoXml1ParseEmpty = 0;
                _hosoXml1Ok = 0;
                _individualSaved = 0;
                _individualSaveFailed = 0;
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
            // Callback đếm save success/fail cho individual files (4 threads → cần Interlocked)
            Action<bool> indvOnComplete = ok =>
            {
                if (ok) Interlocked.Increment(ref _individualSaved);
                else Interlocked.Increment(ref _individualSaveFailed);
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
                        globalIdx += take;
                        continue;
                    }

                    if (string.IsNullOrEmpty(chunkXmlPath) || !File.Exists(chunkXmlPath))
                    {
                        Inventec.Common.Logging.LogSystem.Warn(
                            "ExportExcel chunk " + chunkIdx + " XML file missing → skip chunk");
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
                    }
                    finally
                    {
                        // Xoa file DATA_XML_*.xml ngay sau khi parse xong chunk — KHONG luu rac trong XmlExcel\Excel130\yyyyMMdd\
                        globalIdx = beforeIdx + take;
                        try
                        {
                            if (!string.IsNullOrEmpty(chunkXmlPath) && File.Exists(chunkXmlPath))
                                File.Delete(chunkXmlPath);
                            if (!string.IsNullOrEmpty(saveFileExcel12) && File.Exists(saveFileExcel12))
                                File.Delete(saveFileExcel12);
                        }
                        catch (Exception exDel)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(
                                "ExportExcel chunk " + chunkIdx + " delete DATA_XML failed: " + exDel.Message);
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

                stopwatch.Stop();
                Inventec.Common.Logging.LogSystem.Info(
                    "ExportExcel: DoWork finished. Processed " + globalIdx + " HOSO in " +
                    stopwatch.Elapsed.TotalSeconds.ToString("F1") + "s.");

                // Reconciliation log — verify Individual files vs Group XML1 rows
                int expectedXml1Rows = _hosoXml1Ok + _hosoXml1ParseEmpty;
                int expectedIndividualFiles = _hosoTotal - _hosoNoFilehoso;
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "ExportExcel reconciliation: total={0}, no_filehoso={1}, no_xml1_entry={2}, xml1_parse_empty={3}, xml1_ok={4}, individual_saved={5}, individual_failed={6} → expected_individual={7}, expected_xml1_rows={8}",
                    _hosoTotal, _hosoNoFilehoso, _hosoNoXml1Entry, _hosoXml1ParseEmpty,
                    _hosoXml1Ok, _individualSaved, _individualSaveFailed,
                    expectedIndividualFiles, expectedXml1Rows));

                if (_hosoNoXml1Entry > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "ExportExcel: " + _hosoNoXml1Entry + " HOSO không có entry LOAIHOSO=XML1 → individual file vẫn xuất nhưng group XML1 không +row. Đây là data thiếu XML1, không phải bug code.");
                }
                if (_individualSaveFailed > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "ExportExcel: " + _individualSaveFailed + " individual file Save() throw exception (xem log trước đó).");
                }
            }
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
                        globalIdx++;
                        continue;
                    }

                    if (hoSo != null)
                    {
                        _hosoTotal++;

                        if (hoSo.FILEHOSO == null || hoSo.FILEHOSO.Count == 0)
                        {
                            _hosoNoFilehoso++;
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

                            ProcessHoSo(hoSo, globalIdx, usedNames, groupBuckets, saveQueue);
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
                    // HOSO thật sự không có XML1 entry — data thiếu, không phải bug code
                    _hosoNoXml1Entry++;
                    Inventec.Common.Logging.LogSystem.Warn(
                        "ExportExcel: HOSO idx " + idx + " no XML1 entry, MA_LK=" + treatmentCode + " → individual file vẫn xuất, group XML1 không +row");
                }

                string fileName = SanitizeFileName(treatmentCode, idx, usedNames);
                // (I) Subfolder bucketing — Batch_NNN/MA_LK.xlsx, mỗi folder tối đa FilesPerBucket file
                string bucketDir = GetBucketDir(idx);
                string filePath = Path.Combine(bucketDir, fileName + ".xlsx");
                saveQueue.Add(new IndividualSaveJob { Workbook = wbInd, FilePath = filePath });
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
        // onComplete(true) khi Save thành công, onComplete(false) khi throw — null nếu không cần đếm
        private static void RunSaver(BlockingCollection<IndividualSaveJob> queue, Action<bool> onComplete)
        {
            try
            {
                try { Thread.CurrentThread.Priority = ThreadPriority.BelowNormal; } catch { }

                foreach (var job in queue.GetConsumingEnumerable())
                {
                    Workbook wb = job.Workbook;
                    bool ok = false;
                    try
                    {
                        wb.Save(job.FilePath, SaveFormat.Xlsx);
                        ok = true;
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(
                            "Save failed: " + job.FilePath + " - " + ex);
                    }
                    finally
                    {
                        var disp = wb as IDisposable;
                        if (disp != null) try { disp.Dispose(); } catch { }
                        if (onComplete != null)
                        {
                            try { onComplete(ok); }
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

        private List<object> ExtractItems(His.Bhyt.ExportXml.XML130.XML.FileHoSo fileHoSo, out string maLk)
        {
            maLk = null;
            var listObj = new List<object>();
            try
            {
                switch (fileHoSo.LOAIHOSO)
                {
                    case "XML1":
                        var xml1Main = new His.Bhyt.ExportXml.XML130.XML1.CreateXmlMain();
                        var xml1Data = xml1Main.RunXml1Data(fileHoSo.NOIDUNGFILE);
                        if (xml1Data != null)
                        {
                            listObj.Add(xml1Data);
                            var prop = xml1Data.GetType().GetProperty("MA_LK");
                            if (prop != null)
                            {
                                var v = prop.GetValue(xml1Data, null);
                                if (v != null) maLk = v.ToString();
                            }
                        }
                        break;
                    case "XML2":
                        var xml2Main = new His.Bhyt.ExportXml.XML130.XML2.CreateXmlMain();
                        var xml2Data = xml2Main.RunXml2Data(fileHoSo.NOIDUNGFILE);
                        if (xml2Data != null && xml2Data.DSACH_CHI_TIET_THUOC != null
                            && xml2Data.DSACH_CHI_TIET_THUOC.CHI_TIET_THUOC != null)
                            listObj.AddRange(xml2Data.DSACH_CHI_TIET_THUOC.CHI_TIET_THUOC);
                        break;
                    case "XML3":
                        var xml3Data = His.Bhyt.ExportXml.XML130.XML3.XML3Data.LoadFromXMLString(fileHoSo.NOIDUNGFILE);
                        if (xml3Data != null && xml3Data.DSACH_CHI_TIET_DVKT != null
                            && xml3Data.DSACH_CHI_TIET_DVKT.CHI_TIET_DVKT != null)
                            listObj.AddRange(xml3Data.DSACH_CHI_TIET_DVKT.CHI_TIET_DVKT);
                        break;
                    case "XML4":
                        var xml4Main = new His.Bhyt.ExportXml.XML130.XML4.CreateXmlMain();
                        var xml4Items = xml4Main.RunXml4DetailData(fileHoSo.NOIDUNGFILE);
                        if (xml4Items != null) listObj.AddRange(xml4Items);
                        break;
                    case "XML5":
                        var xml5Main = new His.Bhyt.ExportXml.XML130.XML5.CreateXmlMain();
                        var xml5Items = xml5Main.RunXml5DetailData(fileHoSo.NOIDUNGFILE);
                        if (xml5Items != null) listObj.AddRange(xml5Items);
                        break;
                    case "XML6":
                        var xml6Main = new His.Bhyt.ExportXml.XML130.XML6.CreateXmlMain();
                        var xml6Data = xml6Main.RunXml6Data(fileHoSo.NOIDUNGFILE);
                        if (xml6Data != null
                            && xml6Data.DSACH_HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS != null)
                            listObj.AddRange(xml6Data.DSACH_HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS.HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS);
                        break;
                    case "XML7":
                        var xml7Data = His.Bhyt.ExportXml.XML130.XML7.XML7Data.LoadFromXMLString(fileHoSo.NOIDUNGFILE);
                        if (xml7Data != null) listObj.Add(xml7Data);
                        break;
                    case "XML8":
                        var xml8Main = new His.Bhyt.ExportXml.XML130.XML8.CreateXmlMain();
                        var xml8Data = xml8Main.RunXml8Data(fileHoSo.NOIDUNGFILE);
                        if (xml8Data != null) listObj.Add(xml8Data);
                        break;
                    case "XML9":
                        var xml9Main = new His.Bhyt.ExportXml.XML130.XML9.CreateXmlMain();
                        var xml9Items = xml9Main.RunXml9DetailData(fileHoSo.NOIDUNGFILE);
                        if (xml9Items != null) listObj.AddRange(xml9Items);
                        break;
                    case "XML10":
                        var xml10Main = new His.Bhyt.ExportXml.XML130.XML10.CreateXmlMain();
                        var xml10Data = xml10Main.RunXml10DetailData(fileHoSo.NOIDUNGFILE);
                        if (xml10Data != null) listObj.Add(xml10Data);
                        break;
                    case "XML11":
                        var xml11Main = new His.Bhyt.ExportXml.XML130.XML11.CreateXmlMain();
                        var xml11Data = xml11Main.RunXml11Data(fileHoSo.NOIDUNGFILE);
                        if (xml11Data != null) listObj.Add(xml11Data);
                        break;
                    case "XML12":
                        var xml12Main = new His.Bhyt.ExportXml.XML130.XML12.CreateXmlMain();
                        var xml12Items = xml12Main.RunXml12DetailsData(fileHoSo.NOIDUNGFILE);
                        if (xml12Items != null) listObj.AddRange(xml12Items);
                        break;
                    case "XML13":
                        var xml13Main = new His.Bhyt.ExportXml.XML130.XML13.CreateXmlMain();
                        var xml13Data = xml13Main.RunXML13DetailsData(fileHoSo.NOIDUNGFILE);
                        if (xml13Data != null) listObj.Add(xml13Data);
                        break;
                    case "XML14":
                        var xml14Main = new His.Bhyt.ExportXml.XML130.XML14.CreateXmlMain();
                        var xml14Data = xml14Main.RunXML14DetailsData(fileHoSo.NOIDUNGFILE);
                        if (xml14Data != null) listObj.Add(xml14Data);
                        break;
                    case "XML15":
                        var xml15Main = new His.Bhyt.ExportXml.XML130.XML15.CreateXmlMain();
                        var xml15Items = xml15Main.RunXML15DetailsData(fileHoSo.NOIDUNGFILE);
                        if (xml15Items != null) listObj.AddRange(xml15Items);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("ExtractItems failed for " + fileHoSo.LOAIHOSO + ": " + ex);
            }
            return listObj;
        }

        // Bulk import + Compiled accessors + Combined cache (for individual files only)
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

                    // Apply column-specific value transform (e.g. T_* amount string → double)
                    var tx = meta.ColumnSpecs[col].ValueTransform;
                    if (tx != null) value = tx(value);

                    data[row + 1, col] = value;
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

                    var tx = ctx.Meta.ColumnSpecs[col].ValueTransform;
                    if (tx != null) value = tx(value);

                    ctx.BufferArray[ctx.BufferRowCount, col] = value;
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

        // Heuristic: PropertyType + Name → Excel NumberFormat + optional ValueTransform
        private static ExcelColumnSpec BuildExcelColumnSpec(PropertyInfo p)
        {
            Type t = p.PropertyType;
            string name = p.Name ?? string.Empty;

            // Numeric ints → "0"
            if (t == typeof(int) || t == typeof(short) || t == typeof(byte)
                || t == typeof(uint) || t == typeof(ushort)
                || t == typeof(long) || t == typeof(ulong))
                return new ExcelColumnSpec { NumberFormat = "0", ValueTransform = null };

            // Floating → "#,##0.##"
            if (t == typeof(double) || t == typeof(float) || t == typeof(decimal))
                return new ExcelColumnSpec { NumberFormat = "#,##0.##", ValueTransform = null };

            // XmlCDataSection → text (cdata.Value đã unwrap khi ghi cell)
            if (t == typeof(XmlCDataSection))
                return new ExcelColumnSpec { NumberFormat = "@", ValueTransform = null };

            // DateTime → date format
            if (t == typeof(DateTime) || t == typeof(DateTime?))
                return new ExcelColumnSpec { NumberFormat = "yyyy-mm-dd hh:mm:ss", ValueTransform = null };

            // String — heuristic theo prefix tên property
            if (t == typeof(string))
            {
                // Toàn bộ trường tiền tệ trong XML130 (XML1..XML15):
                //  - T_*  (tổng tiền): T_THUOC, T_VTYT, T_TONGCHI_BV, T_TONGCHI_BH, T_BNTT, T_BNCCT,
                //                       T_BHTT, T_NGUONKHAC, T_BHTT_GDV, T_NGUONKHAC_NSNN/VTNN/VTTN/CL
                //  - THANH_TIEN_BV, THANH_TIEN_BH
                //  - DON_GIA, DON_GIA_BV, DON_GIA_BH
                // → convert string → double + format "#,##0" (thousands separator, Excel SUM được)
                // Dùng prefix exact, KHÔNG dùng substring contains "TIEN"/"GIA" để tránh false positives
                // (NGAY_MIEN_CCT, GIAY_CHUYEN_TUYEN, TONG_TYLE_TTCT, KHAM_GIAM_DINH, …)
                if (name.StartsWith("T_", StringComparison.Ordinal)
                    || name.StartsWith("THANH_TIEN", StringComparison.Ordinal)
                    || name.StartsWith("DON_GIA", StringComparison.Ordinal)
                    || name.StartsWith("SO_LUONG", StringComparison.Ordinal)
                    || name.StartsWith("TYLE_", StringComparison.Ordinal)
                    || name.StartsWith("MUC_HUONG", StringComparison.Ordinal))
                {
                    return new ExcelColumnSpec
                    {
                        //NumberFormat = "0.####",
                        NumberFormat = null,
                        ValueTransform = TransformStringToDouble
                    };
                }

                // Mặc định string → text "@" để chống Excel auto-convert dãy số dài/ngày dạng yyyymmdd
                return new ExcelColumnSpec { NumberFormat = "@", ValueTransform = null };
            }

            // Fallback: không set format
            return new ExcelColumnSpec { NumberFormat = null, ValueTransform = null };
        }

        // String số tiền → double (parse fail → giữ string gốc để user thấy data raw)
        // Data XML130 dạng VN: "1.200,125" (. = thousand, , = decimal) → normalize sang invariant "1200.125"
        private static object TransformStringToDouble(object value)
        {
            if (value == null) return null;
            string s = value as string;
            if (string.IsNullOrWhiteSpace(s)) return null;

            // VN format → invariant: bỏ hết dấu '.', đổi ',' thành '.'
            //string normalized = s.Replace(".", "").Replace(",", ".");
            //string normalized = s.IndexOf(',') >= 0
            //    ? s.Replace(".", string.Empty).Replace(",", ".")
            //    : s;
            string normalized;
            if (s.IndexOf(',') >= 0)
            {
                normalized = s.Replace(".", string.Empty).Replace(",", ".");
            }
            else
            {
                int dotCount = 0;
                for (int i = 0; i < s.Length; i++) if (s[i] == '.') dotCount++;
                normalized = dotCount >= 2 ? s.Replace(".", string.Empty) : s;
            }


            double d;
            if (double.TryParse(normalized, NumberStyles.Float,
                                CultureInfo.InvariantCulture, out d))
                return d;
            return s;
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

        // Excel column format + optional value transform (e.g. amount string → double)
        private class ExcelColumnSpec
        {
            public string NumberFormat;                  // "@", "0", "#,##0", … set vào Style.Custom
            public Func<object, object> ValueTransform;  // null = giữ nguyên
        }

        private class IndividualSaveJob
        {
            public Workbook Workbook;
            public string FilePath;
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
