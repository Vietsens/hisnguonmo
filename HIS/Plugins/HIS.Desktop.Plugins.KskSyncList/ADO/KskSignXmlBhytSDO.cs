using System;

namespace HIS.Desktop.Plugins.KskSyncList.ADO
{
    /// <summary>
    /// Dữ liệu gửi api/EmrSign/SignXmlBhyt khi máy trạm KÝ HỘ thẻ CKS_NGUOI_KET_LUAN bằng phiên đã lưu
    /// của người kết luận. Giống EMR.SDO.SignXmlBhytSDO, thêm SignerLoginname (người ký thật).
    /// Dùng lớp riêng thay vì sửa EMR.SDO.dll ở máy trạm: server EMR bản mới đọc SignerLoginname,
    /// server bản cũ bỏ qua trường lạ và ký như trước.
    /// </summary>
    internal class KskSignXmlBhytSDO
    {
        public string XmlBase64 { get; set; }
        public string TagStoreSignatureValue { get; set; }
        public EMR.SDO.XmlConfigDataSDO ConfigData { get; set; }
        public string SignerLoginname { get; set; }
    }
}
