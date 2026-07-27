/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Cặp (giá trị, nhãn) để bind LookUpEdit cho các trường enum ECDS.
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO
{
    public class KeyValueADO
    {
        public long Value { get; set; }
        public string Text { get; set; }

        public KeyValueADO() { }
        public KeyValueADO(long value, string text)
        {
            this.Value = value;
            this.Text = text;
        }
    }
}
