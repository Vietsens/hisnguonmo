using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials
{
    /// <summary>
    /// Tai truoc song song cac danh muc form dung khi mo.
    /// Truoc day goi tuan tu tren UI thread trong MeShow: ~4.5s khi cache RAM lanh, 15s+ khi server cham
    /// (log 12/10/2026: ACS_USER 16k dong 8.4s, V_HIS_EXECUTE_ROOM 4.8s, cong 6 danh muc nho).
    /// Bat dau ngay trong constructor (chong len InitializeComponent), MeShow cho xong roi moi dung.
    /// BackendDataWorker dung ConcurrentDictionary; man Dang nhap cung tai song song kieu nay.
    /// </summary>
    partial class frmKskInfomantionOfficials
    {
        private List<Task> prefetchTasks;
        // ACS_USER (~16k tai khoan) API rat thang thot (1.8s → 19s) → khong cho khi mo form,
        // cac o nguoi kham nap danh sach khi tai xong (BindAcsUserCombosWhenReady)
        private Task acsUserTask;
        private Stopwatch openStopwatch;

        private void StartPrefetchCatalogs()
        {
            try
            {
                this.openStopwatch = Stopwatch.StartNew();
                // .NET Framework mac dinh chi 2 ket noi/host → cac task se xep hang tung doi
                if (ServicePointManager.DefaultConnectionLimit < 20)
                    ServicePointManager.DefaultConnectionLimit = 20;

                this.acsUserTask = StartPrefetch(() => BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>());
                this.prefetchTasks = new List<Task>
                {
                    StartPrefetch(() => BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>()),
                    StartPrefetch(() => BackendDataWorker.Get<HIS_SERVICE_REQ_STT>()),
                    StartPrefetch(() => BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>()),
                    StartPrefetch(() => BackendDataWorker.Get<HIS_KSK_CONTRACT>()),
                    StartPrefetch(() => BackendDataWorker.Get<HIS_WORK_PLACE>()),
                    StartPrefetch(() => BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>()),
                    StartPrefetch(() => BackendDataWorker.Get<HIS_DEPARTMENT>()),
                    StartPrefetch(EnsureDiseaseDetailCache)
                };
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static Task StartPrefetch(Action action)
        {
            // LongRunning: thread rieng, khong cho ThreadPool tang dan (~0.5s/thread)
            return Task.Factory.StartNew(() =>
            {
                try { action(); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>Cho tai truoc xong (loi/het gio thi cac cho dung tu tai lai nhu cu).</summary>
        private void WaitPrefetchCatalogs()
        {
            if (this.prefetchTasks == null) return;
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                Task.WaitAll(this.prefetchTasks.ToArray(), TimeSpan.FromSeconds(90));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            this.prefetchTasks = null;
            LogOpenStep("cho tai danh muc " + sw.ElapsedMilliseconds + " ms");
        }

        /// <summary>Nap danh sach tai khoan vao cac o nguoi kham / bac si ket luan: ngay neu da co, khong thi khi tai xong.</summary>
        private void BindAcsUserCombosWhenReady()
        {
            try
            {
                if (this.acsUserTask == null || this.acsUserTask.IsCompleted)
                {
                    BindAcsUserCombos();
                    return;
                }
                LogOpenStep("o nguoi kham cho danh sach tai khoan (nap sau)");
                this.acsUserTask.ContinueWith(t =>
                {
                    try
                    {
                        if (this.IsDisposed || !this.IsHandleCreated) return;
                        this.BeginInvoke(new Action(() =>
                        {
                            if (this.IsDisposed) return;
                            BindAcsUserCombos();
                            LogOpenStep("da nap danh sach tai khoan vao o nguoi kham");
                        }));
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BindAcsUserCombos()
        {
            List<ACS.EFMODEL.DataModels.ACS_USER> acsUsers = GetAcsUsers().Where(o => o.IS_ACTIVE == 1).ToList();
            loadcomboBSketluan(acsUsers);
            BindDoctorCombos(acsUsers);
        }

        /// <summary>Lay ACS_USER; dang tai truoc thi cho dung task do (khong goi API lan 2).</summary>
        private List<ACS.EFMODEL.DataModels.ACS_USER> GetAcsUsers()
        {
            try
            {
                if (this.acsUserTask != null && !this.acsUserTask.IsCompleted)
                    this.acsUserTask.Wait(TimeSpan.FromSeconds(90));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>() ?? new List<ACS.EFMODEL.DataModels.ACS_USER>();
        }

        private void LogOpenStep(string step)
        {
            Inventec.Common.Logging.LogSystem.Debug("KskOfficials.Open: " + step
                + (this.openStopwatch != null ? " | tu luc mo " + this.openStopwatch.ElapsedMilliseconds + " ms" : ""));
        }
    }
}
