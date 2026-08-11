using System;

namespace HIS.Desktop.Plugins.HisImportXmlAdjust.ADO
{
    /// <summary>
    /// Body gửi lên api/EmrSign/SignXml09Bh (ký HSM file XML 09/BH ra đúng profile cổng giám định BHXH).
    /// Khớp đúng thuộc tính của EMR.SDO.SignXml09BhSDO bên backend.
    ///
    /// Vì sao khai báo lại ở đây thay vì dùng EMR.SDO.SignXml09BhSDO:
    /// EMR.SDO.dll dùng chung (..\..\..\..\lib\EMR\EMR.SDO.dll) chưa có kiểu mới này, và thay DLL
    /// dùng chung sẽ ảnh hưởng mọi plugin khác. BackendAdapter.Post nhận tham số Object rồi
    /// serialize sang JSON nên chỉ cần trùng TÊN thuộc tính là được.
    /// ConfigData vẫn dùng EMR.SDO.XmlConfigDataSDO vì kiểu đó đã có sẵn trong DLL hiện tại.
    /// </summary>
    public class SignXml09BhADO
    {
        /// <summary>File XML 09/BH chưa ký, base64 của nội dung UTF-8.</summary>
        public string XmlBase64 { get; set; }

        /// <summary>Thông tin chứng thư/HSM (serial, user code, password, secret key...).</summary>
        public EMR.SDO.XmlConfigDataSDO ConfigData { get; set; }

        /// <summary>Thẻ chứa chữ ký. Bỏ trống thì backend dùng CHUKYDONVI.</summary>
        public string SignatureTagName { get; set; }

        /// <summary>Thẻ mà Reference thứ hai trỏ tới. Bỏ trống thì backend dùng TT_HOSO.</summary>
        public string ReferenceTagName { get; set; }
    }
}
