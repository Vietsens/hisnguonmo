/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Dữ liệu trả về khi đăng nhập cổng ECDS (POST /api/fast/v1/auth/login).
 */
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO
{
    public class DangNhapResultDto
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public long expiresIn { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public List<string> roles { get; set; }
    }
}
