using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;


namespace HIS.Desktop.Plugins.TreatmentList.Popup
{
    public partial class frmAIViewChatUrlFormat: Form
    {
        private string _url;
        private bool _isLoaded = false;
        public frmAIViewChatUrlFormat(string url)
        {
            InitializeComponent();
            _url = url;
        }

        private void frmAIViewChatUrlFormat_Load(object sender, EventArgs e)
        {               
        }
        private void frmAIViewChatUrlFormat_Shown(object sender, EventArgs e)
        {
            if (_isLoaded) return;
            _isLoaded = true;
            try
            {
                if (!string.IsNullOrWhiteSpace(_url))
                {
                    txtAIViewChatUrlFormat.Text = _url;
                    webView21.CoreWebView2InitializationCompleted += (s, args) =>
                    {
                        try
                        {
                            if (args.IsSuccess)
                            {
                                webView21.CoreWebView2.Navigate(_url);
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error(args.InitializationException);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    };
                    webView21.EnsureCoreWebView2Async();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void webView21_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            try
            {
                if (e.IsSuccess)
                {
                    webView21.CoreWebView2.Navigate(_url);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error(e.InitializationException);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmAIViewChatUrlFormat_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                webView21?.Dispose();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
