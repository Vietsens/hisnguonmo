using System.Collections.Generic;

namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Chi tiết một cảnh báo dị ứng thuốc (Drug-Allergy Alert)
    /// được parse từ MIMS DRUG-ALLERGY ALERT Result XML.
    /// </summary>
    public class DrugAllergyAlertDetail
    {
        /// <summary>
        /// Tên thuốc kê đơn trong Interaction (GGPI/Product/GenericItem/@name).
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// Mã tham chiếu thuốc (GGPI/Product/GenericItem/@reference).
        /// </summary>
        public string DrugReference { get; set; }

        /// <summary>
        /// Loại đối tượng dị ứng ("Molecule" hoặc "SubstanceClass").
        /// Giá trị lấy trực tiếp theo tên node XML để FE có thể hiển thị/nhóm.
        /// </summary>
        public string AllergenNodeType { get; set; }

        /// <summary>
        /// Tên chất gây dị ứng (Molecule/@name hoặc SubstanceClass/@name).
        /// </summary>
        public string AllergenName { get; set; }

        /// <summary>
        /// Mã tham chiếu chất gây dị ứng (Molecule/@reference hoặc SubstanceClass/@reference).
        /// </summary>
        public string AllergenReference { get; set; }

        /// <summary>
        /// Tên phân lớp dị ứng nếu tồn tại (SubstanceClass bên trong Molecule hoặc trực tiếp dưới Allergy).
        /// VD: "Trimethoprim and related agents".
        /// </summary>
        public string AllergyClassName { get; set; }

        /// <summary>
        /// Mã tham chiếu phân lớp dị ứng nếu tồn tại.
        /// </summary>
        public string AllergyClassReference { get; set; }

        /// <summary>
        /// Danh sách mô tả/ghi chú thêm (nếu sau này MIMS bổ sung thêm node Text).
        /// Hiện tại để mở rộng trong tương lai, FE có thể hiển thị từng dòng.
        /// </summary>
        public List<string> Notes { get; set; }

        public DrugAllergyAlertDetail()
        {
            Notes = new List<string>();
        }
    }
}
