using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials.ADO
{
    /// <summary>
    /// 1 muc danh muc benh (V_HIS_DISEASE_DETAIL) trong muc mau checkbox/luoi, kem control dang hien thi no:
    /// - PARENT_TYPE 1, 2: CheckEdit + o chu gen dong (Check / TextEditor)
    /// - PARENT_TYPE 3, 4, 5: dong tren luoi (GridRow)
    /// </summary>
    public class TemplateItemADO
    {
        public long Id { get; set; }
        public string GroupName { get; set; }
        public string Name { get; set; }
        public CheckEdit Check { get; set; }
        public Control TextEditor { get; set; }
        public DiseaseDetailGridADO GridRow { get; set; }

        /// <summary>"Nhom > Muc" — hien trong noi dung mau, dung de khop khi ma muc khong con.</summary>
        public string Path
        {
            get { return string.IsNullOrEmpty(GroupName) ? Name : GroupName + " > " + Name; }
        }

        public TemplateItemADO(long id, string groupName, string name)
        {
            this.Id = id;
            this.GroupName = (groupName ?? "").Trim();
            this.Name = (name ?? "").Trim();
        }
    }
}
