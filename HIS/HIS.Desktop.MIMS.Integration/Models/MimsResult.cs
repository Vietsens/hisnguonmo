using System.Collections.Generic;

namespace HIS.Desktop.MIMS.Integration.Models
{
	public class MimsResult
	{
		/// <summary>
		/// XML trả về từ MIMS API
		/// </summary>
		public string RawXml { get; set; }

		/// <summary>
		/// HTML đã transform từ XML (nếu có)
		/// </summary>
		public string Html { get; set; }

		/// <summary>
		/// Trạng thái xử lý (thành công/lỗi)
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// Thông báo lỗi hoặc thông tin thêm
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Danh sách cảnh báo text tổng quan (nếu cần lưu thêm thông tin đơn giản)
		/// </summary>
		public List<string> Alerts { get; set; }

		/// <summary>
		/// Chi tiết cảnh báo chống chỉ định VN (VN Contraindication Alert)
		/// Nếu không phải kết quả VN Contraindication thì có thể null hoặc rỗng.
		/// </summary>
		public List<VnContraindicationInteraction> VnContraindicationDetails { get; set; }

		/// <summary>
		/// Chi tiết cảnh báo tương tác thuốc CDS (MIMS DRUG-DRUG Alert)
		/// Nếu không phải kết quả tương tác thuốc thì có thể null hoặc rỗng.
		/// </summary>
		public List<DrugDrugAlertDetail> DrugDrugAlertDetails { get; set; }

		/// <summary>
		/// Thông tin chi tiết Drug Information (GGPI)
		/// </summary>
		public DrugInformationGgpiDetail DrugInformationGgpi { get; set; }

		/// <summary>
		/// Cờ cho biết request tới MIMS bị timeout / lỗi kết nối.
		/// </summary>
		public bool IsTimeout { get; set; }

		/// <summary>
		/// Cờ cho biết MIMS trả về XML lỗi dạng &lt;Error&gt;...&lt;/Error&gt;.
		/// </summary>
		public bool IsErrorResponse { get; set; }

		/// <summary>
		/// Nội dung lỗi chi tiết (nếu MIMS trả về &lt;Error&gt;&lt;Message&gt;...&lt;/Message&gt;&lt;/Error&gt;).
		/// </summary>
		public string ErrorMessage { get; set; }

		public MimsResult()
		{
			Alerts = new List<string>();
			VnContraindicationDetails = new List<VnContraindicationInteraction>();
			DrugDrugAlertDetails = new List<DrugDrugAlertDetail>();
		}
	}
}
