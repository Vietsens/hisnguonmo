using Inventec.Core;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK
{
    /// <summary>
    /// Hien cau canh bao cong no do backend tra kem khi luu THANH CONG don thuoc
    /// cho ho so thuoc dien thu sau. Chi bao cho biet, khong chan luu.
    /// </summary>
    public static class PaylaterDebtWarningUtil
    {
        public static void ShowAfterSave(CommonParam param)
        {
            try
            {
                if (param == null)
                {
                    return;
                }

                string message = param.GetMessage();
                if (string.IsNullOrWhiteSpace(message))
                {
                    return;
                }

                MessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                //Chi la thong bao cho biet, loi o day tuyet doi khong duoc anh huong ket qua luu
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
