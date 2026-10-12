using DevExpress.XtraEditors;
using System;

namespace HIS.Desktop.Plugins.ManuImpMestEdit
{
    // vCong 58605 - Tich hop Cong mua sam: phieu nhap NCC da thuc nhap, Cong mua sam khong cho sua thi khoa nut Luu.
    public partial class FormManuImpMestEdit : HIS.Desktop.Utility.FormBase
    {
        // Plugin khong tham chieu IMSys.DbConfig: HIS_IMP_MEST_TYPE.ID__NCC = 2, HIS_IMP_MEST_STT.ID__IMPORT = 5
        private const long IMP_MEST_TYPE_ID__NCC = 2;
        private const long IMP_MEST_STT_ID__IMPORT = 5;

        private void CheckCongMuaSam()
        {
            try
            {
                if (manuImpMest == null || manuImpMest.ID <= 0
                    || manuImpMest.IMP_MEST_TYPE_ID != IMP_MEST_TYPE_ID__NCC
                    || manuImpMest.IMP_MEST_STT_ID != IMP_MEST_STT_ID__IMPORT)
                {
                    return;
                }
                CongMuaSamCheckADO result = CongMuaSamChecker.CheckImpMest(manuImpMest.ID);
                if (result == null) return;
                if (!result.IsConnected)
                {
                    // Dac ta 6.3: khong ket noi duoc thi cho sua sau khi canh bao
                    XtraMessageBox.Show("Không kết nối được Cổng mua sắm để kiểm tra phiếu. Phiếu vẫn sửa được nhưng có thể lệch với chứng từ trên Cổng mua sắm.", "Cảnh báo");
                }
                else if (!result.CanEdit)
                {
                    btnSave.Enabled = false;
                    XtraMessageBox.Show("Cổng mua sắm: " + (String.IsNullOrWhiteSpace(result.Reason) ? "không cho phép sửa phiếu này" : result.Reason), "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
