using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.MIMS.Integration.Core;

namespace HIS.Desktop.MIMS.Integration.View
{
    public partial class WebBrowser : Form
    {
        public enum WebBrowserResult
        {
            None,
            Confirmed,
            Skipped,
            Closed
        }

        const string license_code = "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";

        public WebBrowserResult Result { get; private set; }

        private string _tempHtmlPath;

        public WebBrowser()
        {
            InitializeComponent();

            EO.Base.Runtime.EnableEOWP = true;
            EO.WebBrowser.Runtime.AddLicense(license_code);
            //webView1.NewWindow += NewWindowBrowser;
            this.FormClosed += WebBrowser_FormClosed;
        }
        private void WebBrowser_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Result == WebBrowserResult.None)
            {
                Result = WebBrowserResult.Closed;
            }
        }

        public WebBrowser(string html, string title = "MIMS", int width = 900, int height = 700, bool? xacNhan = false)
        {
            InitializeComponent();
            this.Text = title;
            if (xacNhan == true)
            {
                layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            else
            {
                layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }

            EO.Base.Runtime.EnableEOWP = true;
            EO.WebBrowser.Runtime.AddLicense(license_code);
            //webView1.NewWindow += NewWindowBrowser;
            this.FormClosed += WebBrowser_FormClosed;

            LoadHtml(html);
        }

        /// <summary>
        /// Nạp lại HTML vào WebView (ghi file tạm trong thư mục MIMSStyleSheet rồi LoadUrl).
        /// </summary>
        public void LoadHtml(string html)
        {
            // Xóa file tạm cũ nếu có
            if (!string.IsNullOrEmpty(_tempHtmlPath))
            {
                try
                {
                    if (File.Exists(_tempHtmlPath))
                        File.Delete(_tempHtmlPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                _tempHtmlPath = null;
            }

            // Write HTML into the same folder as CSS/JS (MIMSStyleSheet) so relative paths work
            var baseDir = MimsConfig.ResourceBasePath; // e.g. ...\\bin\\Resources\\MIMSStyleSheet
            if (!Directory.Exists(baseDir))
            {
                MessageBox.Show("MIMS resources folder not found: " + baseDir,
                                "MIMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var fileName = "mims_result_" + Guid.NewGuid().ToString("N") + ".html";
            var htmlPath = Path.Combine(baseDir, fileName);
            File.WriteAllText(htmlPath, html, Encoding.UTF8);

            _tempHtmlPath = htmlPath;

            var fileUri = new Uri(htmlPath).AbsoluteUri;
            webView1.LoadUrl(fileUri);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            if (!string.IsNullOrEmpty(_tempHtmlPath))
            {
                try
                {
                    if (File.Exists(_tempHtmlPath))
                        File.Delete(_tempHtmlPath);
                }
                catch (Exception ex)
                {
                    // Không chặn đóng form nếu xóa file temp lỗi
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            Result = WebBrowserResult.Confirmed;
            this.Close();
        }
    }
}
