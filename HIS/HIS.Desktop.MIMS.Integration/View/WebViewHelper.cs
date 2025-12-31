using System;
using EO.WebBrowser;
using EO.WinForm;
using System.Windows.Forms;

namespace HIS.Desktop.MIMS.Integration.View
{
    public static class WebViewHelper
    {
        /// <summary>
        /// Hiển thị HTML
        /// </summary>
        /// <param name="html">Chuỗi HTML cần hiển thị</param>
        /// <param name="title">Tiêu đề form (tùy chọn)</param>
        public static void ShowHtml(string html, string title = "MIMS")
        {
            var form = new WebBrowser(html, title);
            //var form = new WebBrowserStyle1(html, title, width, height);
            form.Show();
        }
    }
}
