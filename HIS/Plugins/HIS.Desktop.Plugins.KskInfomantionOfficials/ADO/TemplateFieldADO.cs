using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials.ADO
{
    /// <summary>
    /// 1 o chu thuoc muc mau (VD: Da niem mac, Ham tren...).
    /// Trong JSON mau: Group = ten nhom long ("12. Rang ham mat": {...}), Label = ten o (dung chu nhu tren form).
    /// Key = ten cot HIS_KSK_GENERAL/HIS_DHST tuong ung — chap nhan thay cho Label khi go tay.
    /// </summary>
    public class TemplateFieldADO
    {
        public string Group { get; set; }
        public string Key { get; set; }
        public string Label { get; set; }
        public Control Editor { get; set; }

        public TemplateFieldADO(string group, string key, string label, Control editor)
        {
            this.Group = group;
            this.Key = key;
            this.Label = label;
            this.Editor = editor;
        }
    }
}
