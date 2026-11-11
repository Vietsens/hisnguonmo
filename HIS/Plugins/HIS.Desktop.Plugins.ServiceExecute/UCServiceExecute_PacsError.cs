using Inventec.Common.IntegrationError;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    /// <summary>
    /// vCong 62522: gọi dịch vụ lấy ảnh PACS có phân loại nguồn lỗi và báo cho người dùng
    /// (trước đây BackendAdapter nuốt lỗi, màn hình chỉ không hiện ảnh).
    /// </summary>
    public partial class UCServiceExecute : HIS.Desktop.Utility.UserControlBase
    {
        /// <summary>Thông báo lỗi PACS gần nhất (null nếu lần gọi gần nhất thành công).</summary>
        private string pacsLastErrorMessage;
        /// <summary>Thông báo đã hiện ở lần tự động gần nhất, tránh popup lặp lại khi bấm từng dòng.</summary>
        private string pacsLastShownErrorMessage;

        private static IntegrationPartner PacsPartner
        {
            get { return IntegrationPartner.Pacs("hệ thống PACS"); }
        }

        /// <summary>Gọi PACS lấy thông tin ảnh (cùng lời gọi như BackendAdapter.PostWithouApiParam), ném IntegrationException khi lỗi.</summary>
        private PACS.ImageResponseADO CallPacsImageService(PACS.ImageRequestADO input)
        {
            string uri = (HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_PACS ?? "").TrimEnd('/') + "/" + (RequestUriStore.PACS_SERIVCE__LAY_THONG_TIN_ANH ?? "").TrimStart('/');
            try
            {
                return PACS.PacsApiConsumer.PacsConsumer.PostWithouApiParam<PACS.ImageResponseADO>(RequestUriStore.PACS_SERIVCE__LAY_THONG_TIN_ANH, input, 0);
            }
            catch (Inventec.Common.WebApiClient.ApiException apiEx)
            {
                throw IntegrationHttpError.FromStatus(PacsPartner, uri, (int)apiEx.StatusCode, apiEx.StatusCode.ToString(), null);
            }
            catch (Exception ex)
            {
                throw IntegrationHttpError.WrapCall(ex, PacsPartner, uri);
            }
        }

        /// <summary>Dựng thông báo khi PACS trả TrangThai = false / không trả dữ liệu.</summary>
        private string BuildPacsResultError(PACS.ImageResponseADO resultData)
        {
            if (resultData == null)
                return IntegrationErrorMessage.NoResponse(PacsPartner, "lấy thông tin ảnh");
            if (!resultData.TrangThai)
                return IntegrationErrorMessage.PartnerError(PacsPartner, resultData.ThongBao, "PACS không trả về thông tin ảnh của phiếu");
            return null; // TrangThai = true nhưng không có series: phiếu chưa có ảnh, không phải lỗi
        }

        /// <summary>
        /// Hiện lỗi PACS gần nhất. userAction = true (người dùng chủ động mở ảnh): luôn hiện;
        /// false (tự tải khi chọn dòng): chỉ hiện khi khác lỗi đã hiện lần trước.
        /// </summary>
        private void ShowPacsErrorIfAny(bool userAction)
        {
            try
            {
                string msg = this.pacsLastErrorMessage;
                if (String.IsNullOrWhiteSpace(msg)) return;
                if (!userAction && msg == this.pacsLastShownErrorMessage) return;
                this.pacsLastShownErrorMessage = msg;
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() => DevExpress.XtraEditors.XtraMessageBox.Show(msg, "Thông báo")));
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(msg, "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
