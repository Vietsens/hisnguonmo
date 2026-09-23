/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Hai ô chọn "Nơi công tác" và "Nơi công tác xã/phường" — chỉ tiêu của cổng Sở Y tế TP.HCM.
 *
 * VÌ SAO PHẢI CÓ: cổng BẮT BUỘC nơi công tác với nhóm đối tượng khám là sinh viên / người lao
 * động (mẫu M3 trả 400 "Vui lòng nhập nơi công tác!"). Cổng chỉ nhận Id của danh mục của nó, mà
 * danh mục đó có hơn 5.000 mục tên cơ sở cụ thể — HIS lưu nơi làm việc dạng chữ tự do nên tra
 * theo tên gần như không bao giờ khớp. Phải cho người nhập CHỌN THẲNG trong danh mục của cổng.
 *
 * CHỖ ĐẶT: một dòng ngay TRÊN ô "Lý do khám" ở phần thông tin bệnh nhân (layoutControl2),
 * hai ô chia đôi bề ngang — cùng chỗ với các thông tin hành chính khác, nhìn là thấy, không
 * phải mở tab riêng.
 *
 * CHỖ LƯU: hai khoá `noi_cong_tac` và `noi_cong_tac_xa_phuong` trong cột INTERVIEW_JSON, đi cùng
 * dữ liệu tab Hỏi bệnh lâm sàng HCM. Không thêm cột mới.
 *
 * AN TOÀN ĐA VIỆN: chỉ dựng cho viện đã khai báo cấu hình cổng; viện khác bố cục giữ nguyên.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Inventec.Common.Logging;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Khai báo =====

        /// <summary>Khoá lưu trong INTERVIEW_JSON — trùng tên trường của cổng cho dễ đối chiếu.</summary>
        private const string WPHCM_FIELD__WORK_PLACE = "noi_cong_tac";
        private const string WPHCM_FIELD__WORK_PLACE_WARD = "noi_cong_tac_xa_phuong";

        /// <summary>Mã danh mục của cổng.</summary>
        private const string WPHCM_CAT__WORK_PLACE = "NoiCongTacHocTap";

        /// <summary>
        /// Xã/phường của nơi công tác dùng CHUNG danh mục xã/phường với địa chỉ bệnh nhân.
        /// Bộ Postman của Sở liệt kê 26 danh mục và KHÔNG có danh mục riêng cho xã/phường nơi
        /// công tác, nên đây là danh mục duy nhất khớp.
        /// </summary>
        private const string WPHCM_CAT__WARD = "DiaChiHienTai_XaPhuong";

        private GridLookUpEdit cboKskWorkPlace;
        private GridLookUpEdit cboKskWorkPlaceWard;

        private bool workPlaceHcmInited = false;

        /// <summary>Bề rộng nhãn của hai ô — đủ cho "Nơi công tác xã/phường:".</summary>
        private const int WPHCM_LABEL_W = 145;

        private const int WPHCM_ROW_H = 26;

        #endregion

        #region ===== Dựng ô nhập =====

        private void InitWorkPlaceHcmControls()
        {
            try
            {
                if (workPlaceHcmInited) return;
                if (this.layoutControl2 == null || this.layoutControlItem660 == null) return;
                if (!IsSytHcmDeclared())
                {
                    LogSystem.Debug("SytHcm: chua khai bao cau hinh cong -> KHONG dung o Noi cong tac");
                    return;
                }
                workPlaceHcmInited = true;

                cboKskWorkPlace = NewWorkPlaceCombo("cboKskWorkPlace");
                cboKskWorkPlaceWard = NewWorkPlaceCombo("cboKskWorkPlaceWard");

                this.layoutControl2.BeginUpdate();
                try
                {
                    // Cả hai nằm TRÊN ô Lý do khám, CÙNG MỘT DÒNG, mỗi ô một nửa bề ngang.
                    LayoutControlItem lciWp = (LayoutControlItem)this.layoutControl2.AddItem(
                        "Nơi công tác:", cboKskWorkPlace);
                    SetupWorkPlaceItem(lciWp, "Nơi công tác:");
                    // Nhãn rộng ĐÚNG BẰNG nhãn "Lý do khám" -> ô nhập hai dòng thẳng một cột.
                    // Chỉ chỉnh bên Nơi công tác, KHÔNG đụng vào ô Lý do khám.
                    lciWp.TextSize = this.layoutControlItem660.TextSize;
                    lciWp.Move(this.layoutControlItem660, InsertType.Top);

                    LayoutControlItem lciWard = (LayoutControlItem)this.layoutControl2.AddItem(
                        "Nơi công tác xã/phường:", cboKskWorkPlaceWard);
                    SetupWorkPlaceItem(lciWard, "Nơi công tác xã/phường:");
                    lciWard.Move(lciWp, InsertType.Right);

                    // Chia đôi dòng: hai ô cùng bề rộng thì DevExpress giãn đều hai bên.
                    // KHÔNG ghim MinSize/MaxSize — ghim rồi màn hình hẹp lại là hai ô đè lên nhau.
                    int nuaDong = Math.Max(200, this.layoutControlItem660.Size.Width / 2);
                    lciWp.Size = new Size(nuaDong, WPHCM_ROW_H);
                    lciWard.Size = new Size(nuaDong, WPHCM_ROW_H);
                }
                finally { this.layoutControl2.EndUpdate(); }

                // Đăng ký vào bảng ô của tab hỏi bệnh -> phần lưu và nạp JSON tự lo hai ô này,
                // không phải viết riêng đường đọc/ghi.
                if (dicInterviewHcm != null)
                {
                    dicInterviewHcm[WPHCM_FIELD__WORK_PLACE] = cboKskWorkPlace;
                    dicInterviewHcm[WPHCM_FIELD__WORK_PLACE_WARD] = cboKskWorkPlaceWard;
                }

                ApplySytToWorkPlaceCombos();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private GridLookUpEdit NewWorkPlaceCombo(string name)
        {
            GridLookUpEdit cbo = new GridLookUpEdit();
            cbo.Name = name;
            cbo.MenuManager = this.barManager1;
            cbo.Properties.NullText = "";
            cbo.Properties.DisplayMember = "NAME";
            cbo.Properties.ValueMember = "ID";

            DevExpress.XtraGrid.Columns.GridColumn colId = cbo.Properties.View.Columns.AddField("ID");
            colId.VisibleIndex = 1; colId.Width = 60; colId.Caption = "Mã";
            DevExpress.XtraGrid.Columns.GridColumn colName = cbo.Properties.View.Columns.AddField("NAME");
            colName.VisibleIndex = 2; colName.Width = 520; colName.Caption = "Tên";
            cbo.Properties.PopupFormWidth = 600;
            cbo.Properties.View.OptionsView.ShowColumnHeaders = true;

            // TÌM KIẾM: danh mục hơn 5.000 mục, không ai cuộn tay tìm được nên phải gõ để lọc.
            //   - ImmediatePopup: gõ ký tự đầu là bung danh sách luôn.
            //   - TextEditStyle Standard: cho phép gõ thẳng vào ô (mặc định của ô chọn là chỉ chọn).
            //   - PopupFilterMode Contains: gõ tới đâu LỌC danh sách tới đó, khớp ở GIỮA chuỗi
            //     chứ không bắt phải gõ từ đầu tên.
            //   - OptionsFind.AlwaysVisible: thêm ô tìm ngay trong bảng bung ra, tìm cả theo mã.
            cbo.Properties.ImmediatePopup = true;
            cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            cbo.Properties.View.OptionsFind.AlwaysVisible = true;
            cbo.Properties.View.OptionsFind.FindMode = DevExpress.XtraEditors.FindMode.Always;
            return cbo;
        }

        /// <summary>Canh nhãn phải, cùng kiểu với các ô phía trên để dòng mới không lạc lõng.</summary>
        private void SetupWorkPlaceItem(LayoutControlItem lci, string caption)
        {
            lci.Name = "lci" + caption;
            lci.AppearanceItemCaption.Options.UseTextOptions = true;
            lci.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            lci.Text = caption;
            lci.TextAlignMode = TextAlignModeItem.CustomSize;
            // Nhãn hẹp hơn ô Lý do khám vì mỗi ô chỉ còn một nửa bề ngang; vẫn đủ chỗ cho
            // "Nơi công tác xã/phường:".
            lci.TextSize = new Size(WPHCM_LABEL_W, 20);
            lci.TextToControlDistance = this.layoutControlItem660.TextToControlDistance;
        }

        /// <summary>Đổ hai danh mục của cổng vào ô chọn. Gọi lại khi danh mục tải về muộn.</summary>
        private bool ApplySytToWorkPlaceCombos()
        {
            try
            {
                if (cboKskWorkPlace == null || cboKskWorkPlaceWard == null) return false;

                bool okWp = SetCodeNameSource(cboKskWorkPlace, ToCodeNameList(WPHCM_CAT__WORK_PLACE));
                bool okWard = SetCodeNameSource(cboKskWorkPlaceWard, ToCodeNameList(WPHCM_CAT__WARD));
                return okWp && okWard;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        #endregion

        #region ===== Nạp / lưu khi tab hỏi bệnh CHƯA dựng =====

        /// <summary>
        /// Đổ hai ô nơi công tác từ chuỗi JSON của hồ sơ.
        ///
        /// VÌ SAO TÁCH RIÊNG: đường nạp chung của tab hỏi bệnh chỉ chạy khi tab đó đã dựng, mà tab
        /// dựng muộn (lúc người dùng mở tới). Hai ô này nằm ở phần thông tin bệnh nhân, thấy ngay
        /// khi mở hồ sơ — phải đổ độc lập, không chờ tab.
        /// </summary>
        private void ApplyWorkPlaceHcmFromJson(string json)
        {
            try
            {
                if (cboKskWorkPlace == null || cboKskWorkPlaceWard == null) return;

                cboKskWorkPlace.EditValue = null;
                cboKskWorkPlaceWard.EditValue = null;
                if (string.IsNullOrWhiteSpace(json)) return;

                JObject o = JObject.Parse(json);
                SetWorkPlaceComboValue(cboKskWorkPlace, o[WPHCM_FIELD__WORK_PLACE]);
                SetWorkPlaceComboValue(cboKskWorkPlaceWard, o[WPHCM_FIELD__WORK_PLACE_WARD]);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static void SetWorkPlaceComboValue(GridLookUpEdit cbo, JToken token)
        {
            try
            {
                if (cbo == null || token == null || token.Type == JTokenType.Null) return;
                int v;
                // Mã trong danh mục là kiểu int — gán long thì ô chọn không khớp được dòng nào.
                if (int.TryParse(token.ToString(), out v) && v > 0) cbo.EditValue = v;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Ghép giá trị hai ô nơi công tác vào chuỗi JSON ĐÃ CÓ của hồ sơ, giữ nguyên phần còn lại.
        ///
        /// Dùng khi người dùng lưu hồ sơ mà CHƯA từng mở tab hỏi bệnh: lúc đó đường lưu chung không
        /// chạy (cố ý, để khỏi ghi đè dữ liệu cũ bằng bản rỗng), nhưng hai ô này vẫn có thể vừa
        /// được nhập nên phải ghi lại.
        ///
        /// Trả null khi không có gì để ghi — chỗ gọi hiểu là giữ nguyên cột cũ.
        /// </summary>
        private string MergeWorkPlaceHcmIntoJson(string jsonCu)
        {
            try
            {
                if (cboKskWorkPlace == null || cboKskWorkPlaceWard == null) return null;

                JObject o;
                if (string.IsNullOrWhiteSpace(jsonCu))
                {
                    o = new JObject();
                    o["_v"] = IHCM_JSON_VERSION;
                }
                else
                {
                    o = JObject.Parse(jsonCu);
                }

                PutWorkPlaceValue(o, WPHCM_FIELD__WORK_PLACE, cboKskWorkPlace);
                PutWorkPlaceValue(o, WPHCM_FIELD__WORK_PLACE_WARD, cboKskWorkPlaceWard);
                return o.ToString(Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private static void PutWorkPlaceValue(JObject o, string field, GridLookUpEdit cbo)
        {
            try
            {
                long v;
                if (cbo.EditValue != null && long.TryParse(cbo.EditValue.ToString(), out v) && v > 0)
                    o[field] = v;
                else
                    o.Remove(field);           // bỏ chọn thì xoá hẳn khoá, không gửi 0
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion
    }
}
