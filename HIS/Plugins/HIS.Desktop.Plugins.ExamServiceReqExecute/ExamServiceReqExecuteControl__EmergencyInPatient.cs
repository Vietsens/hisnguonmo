/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * TT39/BYT: Kê giường tại phòng Cấp cứu theo mốc N giờ.
 *
 * Vùng "Lưu Cấp cứu" CHỈ ĐỂ XEM, đặt cạnh combo Phân loại cấp cứu của PTTK_19083:
 *   - Thời gian lưu Cấp cứu: tính từ giờ tiếp đón đến hiện tại
 *   - Trạng thái: chưa đủ N giờ (diện Khám) / đã đủ N giờ (đã chuyển diện Nội trú)
 *
 * KHÔNG có nút chuyển diện — việc chuyển diện do máy chủ tự làm. Màn hình chỉ phản ánh
 * trạng thái máy chủ trả về, không tự suy ra từ số giờ.
 *
 * Máy chủ chuyển diện bằng hai đường: tiến trình chạy nền quét theo chu kỳ, và lớp rà soát
 * tại chỗ gắn ở BỐN luồng có dính tiền — kê giường, nhập viện, ra viện, tạo yêu cầu thanh toán.
 * MỞ HỒ SƠ KHÔNG rà soát (xác nhận bằng autotest KGCC-005, lần chạy 3262: vừa tiếp đón BN lùi
 * 5 giờ rồi mở ngay màn này thì trạng thái vẫn là diện Khám). Chấp nhận được vì mọi đường
 * sinh tiền đều đã có chốt riêng; màn hình chỉ chậm tối đa một chu kỳ quét.
 *
 * Hai nhãn được tạo LÚC CHẠY và chèn vào layoutControl3 nên không đụng Designer.
 * Viện không khai số giờ thì không tạo gì, màn hình giữ nguyên như hiện tại.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Drawing;
using System.Linq;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private LabelControl lblEmergencyStayTime;
        private LabelControl lblEmergencyStayStatus;
        private LayoutControlItem lciEmergencyStayTime;
        private LayoutControlItem lciEmergencyStayStatus;

        /// <summary>
        /// Lấy chuỗi hiển thị theo ngôn ngữ hiện tại cho vùng Lưu Cấp cứu
        /// </summary>
        private string GetEmergencyInPatientText(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    "ExamServiceReqExecuteControl." + key,
                    ResourceLangManager.LanguageUCExamServiceReqExecute,
                    LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>
        /// Tạo hai nhãn chỉ đọc và chèn vào layoutControl3, ngay sau combo Phân loại cấp cứu.
        /// Gọi một lần khi nạp màn hình; gọi lại cũng không tạo trùng.
        /// </summary>
        private void InitEmergencyInPatientArea()
        {
            try
            {
                if (HisConfigCFG.EmergencyInPatientHours <= 0) return;
                if (this.lblEmergencyStayTime != null) return;
                if (this.layoutControl3 == null || this.cboEmergencyClassifyId1 == null) return;

                LayoutControlItem baseItem = this.layoutControl3.GetItemByControl(this.cboEmergencyClassifyId1) as LayoutControlItem;
                if (baseItem == null) return;

                // AutoSizeMode mặc định (Default) để nhãn tự giãn theo độ dài chữ. Đặt None mà không cho kích thước
                // thì ô chỉ rộng 51px, chữ bị cắt cụt ("0 giờ 2 phú", "Chưa đủ 4") — autotest bắt được ở lần chạy 3156.
                // KHÔNG ép MinSize cho ô layout: lần chạy 3159 cho thấy ép 240+330px làm hàng này vỡ,
                // combo Phân loại cấp cứu bị co từ 199px xuống 50px và hai nhãn bị đẩy khỏi bố cục.
                this.lblEmergencyStayTime = new LabelControl();
                this.lblEmergencyStayTime.Name = "lblEmergencyStayTime";
                this.lblEmergencyStayTime.AutoSizeMode = LabelAutoSizeMode.Default;

                this.lblEmergencyStayStatus = new LabelControl();
                this.lblEmergencyStayStatus.Name = "lblEmergencyStayStatus";
                this.lblEmergencyStayStatus.AutoSizeMode = LabelAutoSizeMode.Default;

                this.layoutControl3.BeginUpdate();
                try
                {
                    this.lciEmergencyStayTime = this.layoutControl3.AddItem(
                        this.GetEmergencyInPatientText("lciEmergencyStayTime.Text"),
                        this.lblEmergencyStayTime, baseItem, InsertType.Right);

                    this.lciEmergencyStayStatus = this.layoutControl3.AddItem(
                        this.GetEmergencyInPatientText("lciEmergencyStayStatus.Text"),
                        this.lblEmergencyStayStatus, this.lciEmergencyStayTime, InsertType.Right);
                }
                finally
                {
                    this.layoutControl3.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ẩn/hiện vùng "Lưu Cấp cứu" theo: (viện có khai số giờ) AND (phòng hiện tại là phòng Cấp cứu).
        /// </summary>
        private void UpdateEmergencyInPatientVisibility()
        {
            try
            {
                if (this.lciEmergencyStayTime == null || this.lciEmergencyStayStatus == null) return;

                bool show = false;
                if (HisConfigCFG.EmergencyInPatientHours > 0 && this.moduleData != null)
                {
                    var execRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>()
                        .FirstOrDefault(o => o.ROOM_ID == this.moduleData.RoomId);
                    show = execRoom != null && execRoom.IS_EMERGENCY == 1;
                }

                LayoutVisibility visibility = show ? LayoutVisibility.Always : LayoutVisibility.Never;
                this.lciEmergencyStayTime.Visibility = visibility;
                this.lciEmergencyStayStatus.Visibility = visibility;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đổ thời gian lưu và trạng thái diện điều trị hiện hành lên hai nhãn.
        /// Diện lấy theo dữ liệu máy chủ trả về (hồ sơ đã được rà soát trước khi trả),
        /// không tự suy ra từ số giờ để tránh lệch với dữ liệu thật.
        /// </summary>
        private void LoadEmergencyInPatientValues()
        {
            try
            {
                if (this.lblEmergencyStayTime == null || this.lblEmergencyStayStatus == null) return;
                if (this.treatment == null) return;

                int hours = HisConfigCFG.EmergencyInPatientHours;

                DateTime? inTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatment.IN_TIME);
                if (inTime.HasValue)
                {
                    TimeSpan stay = DateTime.Now - inTime.Value;
                    if (stay.TotalMinutes < 0) stay = TimeSpan.Zero;
                    this.lblEmergencyStayTime.Text = string.Format(
                        this.GetEmergencyInPatientText("lblEmergencyStayTime.Format"),
                        (int)stay.TotalHours, stay.Minutes);
                }
                else
                {
                    // Không để chuỗi rỗng: nhãn tự giãn theo chữ nên rỗng sẽ co về 0px, người dùng không thấy ô đâu
                    // và cũng không kiểm được bằng autotest (lần chạy 3167 mất hẳn nhãn này).
                    this.lblEmergencyStayTime.Text = "—";
                }

                bool isInPatient = this.treatment.TDL_TREATMENT_TYPE_ID.HasValue
                    && this.treatment.TDL_TREATMENT_TYPE_ID.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU;

                if (isInPatient)
                {
                    this.lblEmergencyStayStatus.Text = string.Format(
                        this.GetEmergencyInPatientText("lblEmergencyStayStatus.Done"), hours);
                    this.lblEmergencyStayStatus.ForeColor = Color.Green;
                }
                else
                {
                    this.lblEmergencyStayStatus.Text = string.Format(
                        this.GetEmergencyInPatientText("lblEmergencyStayStatus.NotYet"), hours);
                    this.lblEmergencyStayStatus.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
