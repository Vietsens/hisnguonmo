using System;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials.ADO
{
    /// <summary>
    /// ADO row cho grid hien thi danh sach benh dang doc (PARENT_TYPE = 3, 4, 5).
    /// Moi row = 1 V_HIS_DISEASE_DETAIL, hien thi 3 cot: STT, Ten benh (DISPLAY_NAME), Co.
    /// Dong IS_OTHER=1: cot Ten benh cho phep nhap text (gop chuc nang "Khac").
    /// </summary>
    public class DiseaseDetailGridADO
    {
        /// <summary>V_HIS_DISEASE_DETAIL.ID — dung lam DISEASE_DETAIL_ID khi luu</summary>
        public long DISEASE_DETAIL_ID { get; set; }

        /// <summary>So thu tu hien thi</summary>
        public int STT { get; set; }

        /// <summary>Ten goc tu DB (readonly, dung lam placeholder khi IS_OTHER=1)</summary>
        public string DISEASE_NAME { get; set; }

        /// <summary>
        /// Field bind len grid cot "Ten benh".
        /// Binh thuong = DISEASE_NAME. Khi IS_OTHER=1: user nhap de len = noi dung "Khac".
        /// </summary>
        public string DISPLAY_NAME { get; set; }

        /// <summary>Co check hay khong (IS_CHECKBOX=1 moi hien thi checkbox)</summary>
        public bool IS_CHECKED { get; set; }

        /// <summary>Noi dung nhap khac (IS_OTHER=1 moi hien thi textbox)</summary>
        public string OTHER_TEXT { get; set; }

        /// <summary>Flag: row nay co hien thi checkbox khong</summary>
        public bool HAS_CHECKBOX { get; set; }

        /// <summary>Flag: row nay co hien thi textbox khong</summary>
        public bool HAS_OTHER { get; set; }

        /// <summary>Ten nhom cha (DISEASE_TYPE_NAME) — de group</summary>
        public string GROUP_NAME { get; set; }

        /// <summary>Thu tu nhom cha</summary>
        public long NUM_ORDER_TYPE { get; set; }

        /// <summary>Thu tu chi tiet</summary>
        public long NUM_ORDER_DETAIL { get; set; }
    }
}
