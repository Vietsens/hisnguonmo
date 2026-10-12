using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.ImpMestViewDetail.Base;
using System;

namespace HIS.Desktop.Plugins.ImpMestViewDetail.ImpMestViewDetail
{
    // vCong 58605 - Tich hop Cong mua sam: phieu nhap NCC da thuc nhap, Cong mua sam khong cho sua thi khoa nut Luu
    // (sua gia, so lo, han dung, thong tin chung tu).
    public partial class frmImpMestViewDetail : HIS.Desktop.Utility.FormBase
    {
        private void CheckCongMuaSam()
        {
            try
            {
                if (this.impMest == null || !btnSave.Enabled
                    || this.impMest.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC
                    || this.impMest.IMP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT)
                {
                    return;
                }
                CongMuaSamCheckADO result = CongMuaSamChecker.CheckImpMest(this.impMest.ID);
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
