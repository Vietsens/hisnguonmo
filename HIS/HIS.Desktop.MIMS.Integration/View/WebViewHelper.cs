using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.Core;

namespace HIS.Desktop.MIMS.Integration.View
{
    public static class WebViewHelper
    {
        /// <summary>
        /// Hiển thị HTML tĩnh (đã có kết quả).
        /// </summary>
        public static void ShowHtml(string html, string title = "MIMS")
        {
            var form = new WebBrowser(html, title);
            form.Show();
        }

        /// <summary>
        /// Thực hiện gọi MIMS ở background và hiển thị form WebBrowser:
        /// - Ban đầu hiển thị "Đang kiểm tra".
        /// - Khi có kết quả, nạp HTML hoặc thông báo lỗi.
        /// </summary>
        public static void ShowResultAsync(Func<MimsResult> action, string title = "MIMS", int width = 900, int height = 700)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            string loadingHtml = "<html><head><meta charset=\"utf-8\"/></head><body><h3>Đang kiểm tra...</h3></body></html>";
            var form = new WebBrowser(loadingHtml, title, width, height);
            form.Show();

            string actionErrorMessage = null; // vCong 62522
            Task.Run(() =>
            {
                try
                {
                    //System.Threading.Thread.Sleep(1000);
                    return action();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    actionErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, MimsClient.Partner, "tra cứu cảnh báo thuốc");
                    return null;
                }
            })
            .ContinueWith(t =>
            {
                if (form.IsDisposed)
                    return;

                var result = t.Result;
                string htmlToShow;

                if (result == null || result.IsTimeout)
                {
                    // vCong 62522: hien loi da phan loai (ket noi / tai khoan MIMS / phia MIMS) thay cho "Kiem tra ket noi MIMS"
                    string msg = result != null && Inventec.Common.IntegrationError.IntegrationErrorMessage.IsBuilt(result.Message) ? result.Message
                        : (actionErrorMessage ?? Inventec.Common.IntegrationError.IntegrationErrorMessage.NoResponse(MimsClient.Partner, "tra cứu cảnh báo thuốc"));
                    htmlToShow = "<html><head><meta charset=\"utf-8\"/></head><body><h3>" + System.Security.SecurityElement.Escape(msg) + "</h3></body></html>";
                }
                else if (result.IsErrorResponse)
                {
                    var msg = Inventec.Common.IntegrationError.IntegrationErrorMessage.PartnerError(MimsClient.Partner, result.ErrorMessage, "MIMS trả về lỗi");
                    string safe = System.Security.SecurityElement.Escape(msg);
                    htmlToShow = "<html><head><meta charset=\"utf-8\"/></head><body><h3>" + safe + "</h3></body></html>";
                }
                else if (!string.IsNullOrEmpty(result.Html))
                {
                    htmlToShow = result.Html;
                }
                else
                {
                    var msg = string.IsNullOrEmpty(result.Message) ? "Không có dữ liệu từ MIMS" : result.Message;
                    string safe = System.Security.SecurityElement.Escape(msg);
                    htmlToShow = "<html><head><meta charset=\"utf-8\"/></head><body><h3>" + safe + "</h3></body></html>";
                }

                Action update = () => form.LoadHtml(htmlToShow);

                if (form.InvokeRequired)
                    form.BeginInvoke(update);
                else
                    update();
            });
        }

        /// <summary>
        /// Hiển thị form WebBrowser dạng dialog, trả về kết quả người dùng chọn (Xác nhận, Bỏ qua, Đóng form).
        /// </summary>
        public static bool ShowDialog(string html, string title = "MIMS", int width = 900, int height = 700)
        {
            using (var form = new WebBrowser(html, title, width, height, true))
            {
                form.ShowDialog();
                return form.Result == WebBrowser.WebBrowserResult.Confirmed;
            }
        }
    }
}
