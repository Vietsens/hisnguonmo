/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Ô "Phân loại" ở mục V. KẾT LUẬN của tab Khám sức khỏe dưới 18 tuổi.
 *
 * Lưu vào HIS_KSK_UNDER_EIGHTEEN.HEALTH_EXAM_RANK_ID — cột RIÊNG của tab này, không dùng chung
 * với ô Phân loại của tab Thông tin chung (cột đó thuộc HIS_KSK_GENERAL). Nên việc lưu và nạp làm
 * trực tiếp như mọi ô khác của tab, xem GetValueUnderEighteen / FillDataUnderEighteen.
 */
using System;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Ô Phân loại của tab dưới 18 tuổi — hậu tố 3 theo quy ước của màn hình.</summary>
        private GridLookUpEdit cboHealthExamRank3;

        private bool rank3Inited;

        /// <summary>
        /// Dựng ô Phân loại và chèn ngay dưới dòng "Sức khỏe:" của mục V. KẾT LUẬN.
        /// Gọi được nhiều lần, chỉ dựng một lượt.
        /// </summary>
        private void BuildKskRank3()
        {
            try
            {
                if (rank3Inited) return;
                // layoutControlItem251 = dòng "Sức khỏe:", ngay dưới tiêu đề "V. KẾT LUẬN".
                if (this.layoutControlGroup8 == null || this.layoutControlItem251 == null) return;
                if (this.cboHealthExamRank == null) return;   // chưa có ô mẫu thì chưa dựng
                rank3Inited = true;

                cboHealthExamRank3 = new GridLookUpEdit();
                cboHealthExamRank3.Name = "cboHealthExamRank3";
                cboHealthExamRank3.MenuManager = this.barManager1;

                // Sao y cách hiển thị của ô Phân loại ở tab Thông tin chung: cột nào hiện, cột nào
                // là giá trị, cột nào là chữ. Tự dựng lại thì phải đoán tên cột của danh mục.
                cboHealthExamRank3.Properties.Assign(this.cboHealthExamRank.Properties);
                cboHealthExamRank3.Properties.DataSource =
                    HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>()
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        .ToList();

                LayoutControl lc = this.layoutControlItem251.Owner as LayoutControl;
                if (lc != null) lc.BeginUpdate();
                try
                {
                    LayoutControlItem lci =
                        (LayoutControlItem)this.layoutControlGroup8.AddItem("Phân loại:", cboHealthExamRank3);
                    lci.Name = "lciHealthExamRank3";
                    lci.Text = "Phân loại:";
                    lci.TextAlignMode = TextAlignModeItem.CustomSize;
                    lci.TextSize = this.layoutControlItem251.TextSize;
                    lci.TextToControlDistance = this.layoutControlItem251.TextToControlDistance;
                    // Xếp lại mục V thành HAI hàng:
                    //   hàng 1  Sức khỏe | Các vấn đề khác
                    //   hàng 2  Phân loại | Ngày kết luận | Người khám
                    // Trước đây cả bốn dòng nằm trên một hàng ngang nên hàng rất dài.
                    lci.Move(this.layoutControlItem252, InsertType.Bottom);
                    if (this.lciConclusionTime3 != null)
                        this.lciConclusionTime3.Move(lci, InsertType.Right);
                    if (this.lciKskConcluder2 != null)
                        this.lciKskConcluder2.Move(
                            (this.lciConclusionTime3 != null) ? (BaseLayoutItem)this.lciConclusionTime3 : lci,
                            InsertType.Right);

                    BalanceKskConclusionRows(lci);
                }
                finally { if (lc != null) lc.EndUpdate(); }

                // Hồ sơ có thể đã nạp TRƯỚC khi ô này được dựng (người dùng chưa mở tab) -> đổ lại.
                FillKskRank3();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }


        /// <summary>
        /// Cho hai hàng của mục V. KẾT LUẬN thẳng cột nhau.
        ///
        /// Hàng 1 có 2 dòng nên mỗi dòng chiếm nửa bề ngang, hàng 2 có 3 dòng nên mỗi dòng một phần
        /// ba — hai hàng không thể trùng mốc cột, nhìn vào thấy lệch. Thêm MỘT Ô TRỐNG vào cuối hàng
        /// 1 để cả hai hàng đều 3 cột, khi đó bộ xếp bố cục chia đều bằng nhau.
        ///
        /// Kèm theo: cho bề rộng NHÃN của cả 5 dòng bằng nhau, để ô nhập ở mọi hàng bắt đầu cùng một
        /// vạch. Nhãn dài nhất là "Các vấn đề khác:" nên lấy theo nó.
        /// </summary>
        private void BalanceKskConclusionRows(LayoutControlItem lciRank)
        {
            try
            {
                // Ô trống bù cột thứ ba của hàng 1.
                if (this.layoutControlItem252 != null && this.layoutControlItem252.Parent != null)
                {
                    EmptySpaceItem filler = new EmptySpaceItem();
                    filler.Name = "esiKskConclusionFiller3";
                    filler.AllowHotTrack = false;
                    this.layoutControlItem252.Parent.AddItem(filler, this.layoutControlItem252, InsertType.Right);
                }

                // Bề rộng nhãn bằng nhau -> ô nhập thẳng vạch giữa các hàng.
                int labelW = 110;   // theo nhãn dài nhất "Các vấn đề khác:"
                BaseLayoutItem[] rows = new BaseLayoutItem[]
                {
                    this.layoutControlItem251, this.layoutControlItem252,
                    lciRank, this.lciConclusionTime3, this.lciKskConcluder2
                };
                foreach (BaseLayoutItem it in rows)
                {
                    LayoutControlItem one = it as LayoutControlItem;
                    if (one == null) continue;
                    one.TextAlignMode = TextAlignModeItem.CustomSize;
                    one.TextSize = new System.Drawing.Size(labelW, 20);
                    one.TextToControlDistance = 5;
                }

                // Ô "Người khám" đang ghim bề rộng tối thiểu 250 nên kéo lệch phần chia đều của hàng 2.
                if (this.lciKskConcluder2 != null)
                    this.lciKskConcluder2.SizeConstraintsType = SizeConstraintsType.Default;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổ giá trị đã lưu vào ô Phân loại. Gọi sau khi nạp hồ sơ của tab.</summary>
        private void FillKskRank3()
        {
            try
            {
                if (cboHealthExamRank3 == null) return;
                cboHealthExamRank3.EditValue = (currentKskUnderEight != null)
                    ? currentKskUnderEight.HEALTH_EXAM_RANK_ID : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Mã phân loại đang chọn — dùng khi lưu. Chưa dựng ô thì giữ nguyên giá trị cũ.</summary>
        private long? GetKskRank3Value()
        {
            try
            {
                if (cboHealthExamRank3 == null)
                {
                    // Người dùng chưa mở tab nên ô chưa được dựng: KHÔNG trả null, vì lượt lưu sẽ
                    // ghi đè mất giá trị đang có trong cơ sở dữ liệu.
                    return (currentKskUnderEight != null) ? currentKskUnderEight.HEALTH_EXAM_RANK_ID : null;
                }
                if (cboHealthExamRank3.EditValue == null) return null;
                long v;
                return long.TryParse(cboHealthExamRank3.EditValue.ToString(), out v) ? (long?)v : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }
    }
}
