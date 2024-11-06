using System;
using System.Windows.Forms;
using EO.WebBrowser;
using EO.WinForm;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup
{
    public partial class WebViewer : Form
    {

        public WebViewer()
        {
            InitializeComponent();
            InitializeEOWebBrowser();
        }

        private string url = "";
        const string license_code = "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";

        public WebViewer(string url) : this()
        {
            try
            {
                this.url = url;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void WebViewer_Load(object sender, EventArgs e)
        {
            try
            {
                EO.Base.Runtime.EnableEOWP = true;
                EO.WebBrowser.Runtime.AddLicense(license_code);
                if (!string.IsNullOrEmpty(url))
                {
                    webView.Url = url; 
                }
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
    }
}
