using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KskSyncList.ADO
{
    /// <summary>
    /// Loai KSK dung cho combobox loc (muc 4.2.1 PTTK_44350).
    /// KSK_TYPE_ID la ma phan loai KSK trong HIS (tong quat / tren 18 / duoi 18 /
    /// lai xe / lai xe dinh ky / nghe nghiep / khac / tre duoi 6 tuoi).
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
        /// Danh sach loai KSK dang co nguon du lieu trong HIS (mapping muc 3.5).
        /// </summary>
        public static List<KskTypeADO> GetHisKskTypes()
        {
            return new List<KskTypeADO>()
            {
                new KskTypeADO(1, "Dưới 18 tuổi"),
                new KskTypeADO(2, "Trên 18 tuổi"),
                new KskTypeADO(3, "Lái xe"),
                new KskTypeADO(4, "Lái xe định kỳ"),
                new KskTypeADO(5, "Nghề nghiệp"),
                new KskTypeADO(6, "Tổng quát"),
                new KskTypeADO(7, "Trẻ <6 tuổi"),
                new KskTypeADO(8, "Khác"),
            };
        }
    }
}
