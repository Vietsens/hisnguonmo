using System.Collections.Generic;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials.ADO
{
    /// <summary>
    /// 1 muc co nut "Mau" tren form KSK can bo.
    /// - Muc checkbox/luoi (tien su, trieu chung): DiseaseParentType = V_HIS_DISEASE_DETAIL.PARENT_TYPE (1..5).
    /// - Muc o chu (toan than, noi khoa, 9..14): danh sach Fields.
    /// Hashtag dung de loc mau cua dung muc trong Thu vien van ban (HIS.Desktop.Plugins.TextLibrary).
    /// </summary>
    public class TemplateSectionADO
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Hashtag { get; set; }
        public long? DiseaseParentType { get; set; }
        public List<TemplateFieldADO> Fields { get; set; }

        public TemplateSectionADO(string code, string title, string hashtag)
        {
            this.Code = code;
            this.Title = title;
            this.Hashtag = hashtag;
            this.Fields = new List<TemplateFieldADO>();
        }
    }
}
