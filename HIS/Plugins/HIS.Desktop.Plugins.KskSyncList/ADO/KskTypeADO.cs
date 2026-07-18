using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KskSyncList.ADO
{
    /// <summary>
    /// Loai KSK dung cho combobox loc (muc 4.2.1 PTTK_44350).
    /// KSK_TYPE_ID = so thu tu mau phieu QD 1551 (Phu luc 01), trung khit gia tri
    /// enum His.Ksk.QD2062.Base.FormType (1..17): 1=6-<18, 2=>=18, 3=Lai xe,
    /// 4=Duong sat, 5=Thuyen vien, 6..13=Tre <6 tuoi theo moc thang, 14..17=Hoc sinh.
    /// </summary>
    public class KskTypeADO
    {
        public long KSK_TYPE_ID { get; set; }
        public string KSK_TYPE_NAME { get; set; }

        public KskTypeADO() { }
        public KskTypeADO(long id, string name)
        {
            this.KSK_TYPE_ID = id;
            this.KSK_TYPE_NAME = name;
        }

        // ComboBoxEdit hien thi theo ToString()
        public override string ToString()
        {
            return KSK_TYPE_NAME;
        }

        /// <summary>
        /// Danh sach 17 loai KSK theo Phu luc 01 QD 1551 (KSK_TYPE_ID = so thu tu mau).
        /// Dung cho combobox loc. Cac loai chua co bieu mau HIS (4,5,14-17) van liet ke
        /// de loc du lieu neu view V_HIS_KSK_SYNC co ban ghi phan loai tuong ung.
        /// </summary>
        public static List<KskTypeADO> GetHisKskTypes()
        {
            return new List<KskTypeADO>()
            {
                new KskTypeADO(1, "Người 6 - <18 tuổi"),
                new KskTypeADO(2, "Người ≥18 tuổi"),
                new KskTypeADO(3, "Lái xe"),
                new KskTypeADO(4, "Nhân viên đường sắt"),
                new KskTypeADO(5, "Thuyền viên"),
                new KskTypeADO(6, "Trẻ 0 - <2 tháng"),
                new KskTypeADO(7, "Trẻ 2 - 3 tháng"),
                new KskTypeADO(8, "Trẻ 4 - 6 tháng"),
                new KskTypeADO(9, "Trẻ 7 - 9 tháng"),
                new KskTypeADO(10, "Trẻ 10 - 12 tháng"),
                new KskTypeADO(11, "Trẻ 13 - 18 tháng"),
                new KskTypeADO(12, "Trẻ 19 - <24 tháng"),
                new KskTypeADO(13, "Trẻ 2 - <6 tuổi"),
                new KskTypeADO(14, "Học sinh mầm non (3 tháng - <6 tuổi)"),
                new KskTypeADO(15, "Học sinh lớp 1 - 5"),
                new KskTypeADO(16, "Học sinh lớp 6 - 9"),
                new KskTypeADO(17, "Học sinh lớp 10 - 12"),
            };
        }
    }
}
