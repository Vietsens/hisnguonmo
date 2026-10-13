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
using EMR.EFMODEL.DataModels;
using EMR.SDO;
using HIS.Desktop.Plugins.HisTrackingList.ADO;
using HIS.Desktop.Plugins.HisTrackingList.UcEmrDocument;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTrackingList.TrackingView
{
    /// <summary>
    /// Read only view of all treatment sheets of a treatment, newest treatment time first,
    /// embedded by other modules (e.g. tab "DS tờ điều trị" of HIS.Desktop.Plugins.TrackingCreate).
    /// </summary>
    public partial class UcTrackingListView : HIS.Desktop.Utility.UserControlBase
    {
        const string KEY__LOADING = "UcTrackingListView.lblStatus.Loading";
        const string KEY__EMPTY = "UcTrackingListView.lblStatus.Empty";
        const string KEY__LOAD_FAILED = "UcTrackingListView.lblStatus.LoadFailed";
        const string KEY__BTN_RETRY = "UcTrackingListView.btnRetry.Text";

        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;
        UcViewEmrDocument ucViewEmrDocument;
        int loadVersion;

        class LoadResult
        {
            internal bool IsEmpty { get; set; }
            internal bool IsError { get; set; }
            internal string ViewFile { get; set; }
        }

        public UcTrackingListView(Inventec.Desktop.Common.Modules.Module module, long treatmentId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.treatmentId = treatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UcTrackingListView_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                InitViewer();
                LoadTrackingList();
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
                this.btnRetry.Text = TrackingSheetRenderer.GetText(KEY__BTN_RETRY);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitViewer()
        {
            try
            {
                this.ucViewEmrDocument = new UcViewEmrDocument(false);
                this.ucViewEmrDocument.Dock = DockStyle.Fill;
                this.pnlViewer.Controls.Add(this.ucViewEmrDocument);
                this.ucViewEmrDocument.SetViewOnlyMode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Loads (again) all treatment sheets of the treatment. Data is loaded and rendered on a background thread,
        /// only this view shows the loading state.
        /// </summary>
        public void LoadTrackingList()
        {
            try
            {
                int version = Interlocked.Increment(ref this.loadVersion);
                ShowStatus(TrackingSheetRenderer.GetText(KEY__LOADING), false);

                string templateFile = TrackingSheetRenderer.GetTemplateFile();
                string noticeFormat = TrackingSheetRenderer.GetText(TrackingSheetRenderer.KEY__SHEET_NOT_SHOWN);
                Thread thread = new Thread(() => LoadInBackground(version, templateFile, noticeFormat));
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                ShowStatus(TrackingSheetRenderer.GetText(KEY__LOAD_FAILED), true);
            }
        }

        private void CancelLoading()
        {
            Interlocked.Increment(ref this.loadVersion);
        }

        private bool IsCurrentLoad(int version)
        {
            return version == Volatile.Read(ref this.loadVersion);
        }

        private void LoadInBackground(int version, string templateFile, string noticeFormat)
        {
            LoadResult result = new LoadResult();
            List<DocumentTrackingADO> documents = new List<DocumentTrackingADO>();
            try
            {
                long roomId = this.currentModule != null ? this.currentModule.RoomId : 0;
                List<V_HIS_TRACKING> trackings = TrackingSheetRenderer.GetTrackings(this.treatmentId);
                if (trackings == null)
                {
                    result.IsError = true;
                }
                else if (trackings.Count == 0)
                {
                    result.IsEmpty = true;
                }
                else
                {
                    bool isEmrError;
                    List<V_EMR_DOCUMENT> emrDocuments = TrackingSheetRenderer.GetSignedDocuments(trackings.First().TREATMENT_CODE, out isEmrError);
                    List<TrackingDisplayUnit> units = TrackingDocumentPlanner.Plan(trackings, emrDocuments);

                    TrackingPrintData printData = null;
                    if (!isEmrError && units.Any(o => o.Document == null))
                    {
                        printData = TrackingSheetRenderer.LoadData(this.treatmentId);
                        if (printData != null)
                            TrackingSheetRenderer.PrepareForManyRenders(printData, this.treatmentId);
                    }

                    if (isEmrError || (printData == null && units.Any(o => o.Document == null)))
                    {
                        result.IsError = true;
                    }
                    else
                    {
                        Dictionary<long, EmrDocumentFileSDO> signedFiles = new Dictionary<long, EmrDocumentFileSDO>();
                        List<long> documentIds = units.Where(o => o.Document != null).Select(o => o.Document.ID).Distinct().ToList();
                        if (documentIds.Count > 0)
                        {
                            var files = TrackingSheetRenderer.DownloadEmrDocumentFiles(documentIds, roomId);
                            if (files != null)
                            {
                                foreach (var file in files)
                                {
                                    if (file != null && file.DocumentId.HasValue && !signedFiles.ContainsKey(file.DocumentId.Value))
                                        signedFiles[file.DocumentId.Value] = file;
                                }
                            }
                        }

                        foreach (var unit in units)
                        {
                            if (!IsCurrentLoad(version))
                                break;
                            DocumentTrackingADO document = unit.Document != null
                                ? TrackingSheetRenderer.CreateSignedDocument(unit, signedFiles)
                                : TrackingSheetRenderer.RenderTracking(printData, unit.Tracking, templateFile, this.currentModule, this.treatmentId);
                            if (document == null)
                                document = TrackingSheetRenderer.BuildNoticeDocument(TrackingSheetRenderer.FormatSheetNotShown(noticeFormat, unit.Tracking), unit.Tracking.TRACKING_TIME);
                            if (document != null)
                                documents.Add(document);
                        }

                        if (IsCurrentLoad(version))
                        {
                            result.ViewFile = this.ucViewEmrDocument.BuildViewFile(documents);
                            result.IsError = String.IsNullOrEmpty(result.ViewFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                DisposeDocuments(documents);
            }

            try
            {
                if (IsCurrentLoad(version) && !this.IsDisposed && this.IsHandleCreated)
                {
                    this.BeginInvoke(new MethodInvoker(delegate { ApplyLoadResult(version, result); }));
                }
                else
                {
                    DeleteFile(result.ViewFile);
                }
            }
            catch (Exception ex)
            {
                DeleteFile(result.ViewFile);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ApplyLoadResult(int version, LoadResult result)
        {
            try
            {
                if (!IsCurrentLoad(version) || this.IsDisposed)
                {
                    DeleteFile(result.ViewFile);
                    return;
                }

                if (result.IsEmpty)
                {
                    ShowStatus(TrackingSheetRenderer.GetText(KEY__EMPTY), false);
                }
                else if (result.IsError)
                {
                    DeleteFile(result.ViewFile);
                    ShowStatus(TrackingSheetRenderer.GetText(KEY__LOAD_FAILED), true);
                }
                else
                {
                    this.pnlStatus.Visible = false;
                    this.pnlViewer.Visible = true;
                    this.ucViewEmrDocument.ShowViewFile(result.ViewFile);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowStatus(string message, bool isShowRetry)
        {
            try
            {
                this.pnlViewer.Visible = false;
                this.pnlStatus.Visible = true;
                this.lblStatus.Text = message;
                this.btnRetry.Visible = isShowRetry;
                ArrangeRetryButton();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ArrangeRetryButton()
        {
            try
            {
                int x = Math.Max(0, (this.pnlStatus.Width - this.btnRetry.Width) / 2);
                this.btnRetry.Location = new System.Drawing.Point(x, this.lblStatus.Bottom + 16);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void pnlStatus_Resize(object sender, EventArgs e)
        {
            ArrangeRetryButton();
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            try
            {
                LoadTrackingList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static void DisposeDocuments(List<DocumentTrackingADO> documents)
        {
            try
            {
                if (documents == null)
                    return;
                foreach (var item in documents)
                {
                    if (item != null && item.DocumentFile != null)
                    {
                        item.DocumentFile.Dispose();
                        item.DocumentFile = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void DeleteFile(string file)
        {
            try
            {
                if (!String.IsNullOrEmpty(file) && File.Exists(file))
                    File.Delete(file);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
