using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using HIS.Desktop.Plugins.Transaction.Config;
using MOS.Filter;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Transaction
{
    /// <summary>
    /// Thanh toan vien phi truoc khi ket thuc dieu tri.
    /// Bo sung o loc trang thai duyet tren danh sach Vien phi.
    /// Tham chieu: _Software-Specs/02_Analysis_Design/PTTK_TBD_Thanh_Toan_Vien_Phi_Truoc_Ket_Thuc_Dieu_Tri.md
    ///
    /// Toan bo phan nay chi dung khi khoa MOS.HIS_TREATMENT.PAY_BEFORE_FINISH_OPTION = 1.
    /// Vien khong khai bao thi khong tao control nao, bo cuc luoi giu nguyen nhu hien tai.
    ///
    /// Control duoc them bang API cua LayoutControl chu khong sua file Designer,
    /// vi gan Location truc tiep tren LayoutControl khong an.
    /// </summary>
    public partial class UCTransaction : HIS.Desktop.Utility.UserControlBase
    {
        private CheckEdit chkApproveFinish;

        /// <summary>
        /// Goi sau khi da nap cau hinh trong su kien Load cua man hinh.
        /// </summary>
        private void InitPayBeforeFinish()
        {
            try
            {
                if (!HisConfigCFG.IsPayBeforeFinish)
                {
                    return;
                }

                this.InitApproveFinishFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// O loc hai trang thai, dat ngay ben phai o loc "No VP" cho dong bo voi cac dieu kien loc san co.
        /// Khong tich = Tat ca, tich = Chi ho so da duyet.
        /// </summary>
        private void InitApproveFinishFilter()
        {
            try
            {
                if (this.layoutControl1 == null || this.lciChkIsInDebt == null)
                {
                    return;
                }

                this.chkApproveFinish = new CheckEdit();
                this.chkApproveFinish.Name = "chkApproveFinish";
                this.chkApproveFinish.Properties.Caption = ":Đủ ĐK RV";
                this.chkApproveFinish.Properties.AllowGrayed = false;
                this.chkApproveFinish.RightToLeft = RightToLeft.Yes;
                this.chkApproveFinish.Checked = false;
                this.chkApproveFinish.ToolTip = "Không tích: tất cả hồ sơ"
                    + Environment.NewLine + "Tích: chỉ hồ sơ đã duyệt đủ điều kiện ra viện"
                    + Environment.NewLine + "Chọn xong bấm Tìm kiếm để nạp lại danh sách";

                LayoutControlItem lci = this.layoutControl1.AddItem(string.Empty, this.chkApproveFinish, this.lciChkIsInDebt, DevExpress.XtraLayout.Utils.InsertType.Right);
                lci.Name = "lciChkApproveFinish";
                lci.TextVisible = false;
                lci.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
                lci.MinSize = new System.Drawing.Size(80, 24);
                lci.MaxSize = new System.Drawing.Size(80, 24);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Gan dieu kien loc trang thai duyet vao bo loc danh sach ho so.
        /// Khong bat cau hinh hoac chon "Tat ca" thi khong gan gi, truy van giu nguyen nhu hien tai.
        /// </summary>
        private void SetApproveFinishFilter(ref HisTreatmentFeeViewFilter treatFilter)
        {
            try
            {
                if (!HisConfigCFG.IsPayBeforeFinish || this.chkApproveFinish == null || treatFilter == null)
                {
                    return;
                }

                if (this.chkApproveFinish.Checked)
                {
                    treatFilter.IS_APPROVE_FINISH = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
