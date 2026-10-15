using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials.ADO
{
    /// <summary>
    /// 1 cap "ten": gia tri trong JSON mau sau khi trai phang:
    ///   { "Tang HA": true }                 → Group = null, Key = "Tang HA"
    ///   { "1) Tim mach": { "Tang HA": true } } → Group = "1) Tim mach", Key = "Tang HA"
    /// </summary>
    public class TemplateEntryADO
    {
        public string Group { get; set; }
        public string Key { get; set; }
        public JToken Value { get; set; }

        /// <summary>Ten hien thi khi bao dong khong khop.</summary>
        public string DisplayName
        {
            get { return string.IsNullOrEmpty(Group) ? Key : Group + " > " + Key; }
        }
    }
}
