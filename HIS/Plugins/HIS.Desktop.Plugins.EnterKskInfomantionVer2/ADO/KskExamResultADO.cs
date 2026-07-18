namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO
{
    /// <summary>
    /// 1 dong trong form chon ket qua kham lam sang (>=18 tuoi) de dien vao o "Bệnh tật".
    /// Ten = ten vung kham; KetQua = noi dung ket qua / benh khac (co the nhieu dong); Chon = tich chon.
    /// </summary>
    public class KskExamResultADO
    {
        public string Ten { get; set; }
        public string KetQua { get; set; }
        public bool Chon { get; set; }
    }
}
