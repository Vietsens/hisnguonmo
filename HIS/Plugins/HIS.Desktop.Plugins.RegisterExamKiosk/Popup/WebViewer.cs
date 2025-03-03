using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using EO.WebBrowser;
using EO.WinForm;
using HIS.Desktop.Plugins.RegisterExamKiosk.Config;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup
{
    public partial class WebViewer : Form
    {

        public WebViewer()
        {
            InitializeComponent();
            InitializeEOWebBrowser();
            //InitializeCountdownTimer();
        }

        private string url = "";
        const string license_code = "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";
        HIS.Desktop.Common.DelegateCloseForm_Uc DelegateClose;
        System.Threading.Thread CloseThread;
        private int countdownDuration = 60;
        private int remainingTime;
        public WebViewer(string url, HIS.Desktop.Common.DelegateCloseForm_Uc closeForm_Uc) : this()
        {
            try
            {
                this.url = url;
                this.DelegateClose = closeForm_Uc;
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitializeCountdownTimer()
        {
            countdownDuration = HisConfigCFG.timeWaitingMilisecond;
            remainingTime = countdownDuration;
            countdownTimer = new Timer();
            countdownTimer.Interval = 100; // Kiểm tra mỗi 100 ms để có độ chính xác cao hơn
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            remainingTime -= 100; // Trừ đi 100 ms mỗi lần
            lblTime.Text = "Thoi gian con lai: " + remainingTime/1000;
            lblTime.Font = new System.Drawing.Font("Arial", 5);
            if (remainingTime <= 0)
            {
                countdownTimer.Stop();
                ClosingForm();
            }
        }

        private void ResetCountdown(object sender, EventArgs e)
        {
            LogResetCountdownCaller();
            // Reset lại thời gian đếm ngược
            remainingTime = countdownDuration;
        }
        private void LogResetCountdownCaller()
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var caller = stackTrace.GetFrame(1)?.GetMethod()?.Name;
            Inventec.Common.Logging.LogSystem.Info($"ResetCountdown called from: {caller}");
        }   
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.IsHandleCreated)
                {
                    // Đảm bảo rằng việc gọi Close diễn ra trong UI thread
                    this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                }
                else
                {
                    // Nếu form chưa được tạo handle, có thể gọi Close trực tiếp
                    this.Close();
                }
                countdownTimer.Stop();
                CloseThread.Abort();
                // Thực thi delegate nếu cần
                //DelegateClose?.Invoke(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void WebViewer_Load(object sender, EventArgs e)
        {
            try
            {
                EO.Base.Runtime.EnableEOWP = true;
                EO.WebBrowser.Runtime.AddLicense(license_code);

                if (!string.IsNullOrEmpty(url))
                {
                    await LoadWebPageAsync();
                    InitializeCountdownTimer();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadWebPageAsync()
        {
            try
            {
               
                webView.Url = url;
                
                await Task.Run(() =>
                {
                    while (!webView.IsCreated)
                    {
                        
                        System.Threading.Thread.Sleep(5);
                    }
                });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                
            }
        }

        private void InitializeEOWebBrowser()
        {
            webControl.WebView = webView;
        }

        private void webView_NewWindow(object sender, NewWindowEventArgs e)
        {
            try
            {
                MessageBox.Show(this, "Tính năng chưa hỗ trợ trên thiết bị hiện tại", "Thông báo", MessageBoxButtons.OK);
                return;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ClosingForm()
        {
            try
            {
                // Kiểm tra nếu timeWaitingMilisecond > 0, cho phép đóng form
                if (HisConfigCFG.timeWaitingMilisecond > 0)
                {
                    // Kiểm tra nếu form đã được tạo handle
                    if (this.IsHandleCreated)
                    {
                        // Đảm bảo rằng việc gọi Close diễn ra trong UI thread
                        this.Invoke(new MethodInvoker(delegate { this.Close(); }));
                    }
                    else
                    {
                        // Nếu form chưa được tạo handle, có thể gọi Close trực tiếp
                        this.Close();
                    }

                    // Thực thi delegate nếu cần
                    DelegateClose?.Invoke(null);
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void webView_JSDialog(object sender, JSDialogEventArgs e)
        {
            try
            {
                //ResetCountdown(sender, e);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void WebViewer_Click(object sender, EventArgs e)
        {
            try
            {
                //ResetCountdown(sender, e);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void webView_MouseDown(object sender, EO.Base.UI.MouseEventArgs e)
        {
            try
            {
                ResetCountdown(sender, e);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void webView_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                //ResetCountdown(sender, e);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void webView_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                //ResetCountdown(sender, e);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private int lastMouseX = -1;
        private int lastMouseY = -1;

        private void webView_MouseMove(object sender, EO.Base.UI.MouseEventArgs e)
        {
            try
            {
                // Chỉ gọi ResetCountdown khi vị trí chuột thay đổi
                if (e.X != lastMouseX || e.Y != lastMouseY)
                {
                    lastMouseX = e.X;
                    lastMouseY = e.Y;
                    Inventec.Common.Logging.LogSystem.Debug("reset countdown");
                    ResetCountdown(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void webView_KeyDown(object sender, EO.Base.UI.WndMsgEventArgs e)
        {
            try
            {
                ResetCountdown(sender, e);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
