using EMR.SDO;

namespace EMR.Desktop.Plugins.EmrPatientCertificateRegister.ADO
{
    /// <summary>
    /// Du lieu gui api/EmrPatientCertificate/Register khi dung nen tang CMC (EMR.HSM.CMC.INTEGRATE_OPTION khac 0).
    /// Bo sung 3 truong ma EMR.SDO.dll trong lib chua co. Khi EMR.SDO.dll cap nhat co san cac truong nay thi bo lop nay,
    /// dung truc tiep EmrPatientCertificateRegisterSDO.
    /// </summary>
    public class CmcCertificateRegisterSDO : EmrPatientCertificateRegisterSDO
    {
        /// <summary>Base64 anh CCCD mat truoc - server ghep PDF va ky bang CKS Vien</summary>
        public string cccdFrontImage { get; set; }
        /// <summary>Base64 anh CCCD mat sau</summary>
        public string cccdBackImage { get; set; }
        /// <summary>De trong: server tu tao PDF anh 2 mat CCCD tu cccdFrontImage/cccdBackImage</summary>
        public string citizenIdentifyFile { get; set; }
    }
}
