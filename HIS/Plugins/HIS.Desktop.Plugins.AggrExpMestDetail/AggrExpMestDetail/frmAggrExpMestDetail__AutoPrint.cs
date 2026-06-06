/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.AggrExpMestDetail.AggrExpMestDetail.AggregateExpMestPrintFilter;
using HIS.Desktop.Plugins.AggrExpMestDetail.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrExpMestDetail.AggrExpMestDetail
{
    /// <summary>
    /// PTTK_42983 - Tự động in phiếu khi Thực xuất phiếu lĩnh (thuần Frontend, mặc định TẮT).
    /// Ô tích "In Phiếu" cạnh nút "In ấn" + dropdown chọn loại phiếu (chọn nhiều).
    /// Trạng thái được ghi nhớ qua ControlStateWorker giống checkbox "In:" (chkPrint).
    /// </summary>
    public partial class frmAggrExpMestDetail : HIS.Desktop.Utility.FormBase
    {
        #region AutoPrint declaration

        /// <summary>
        /// KEY lưu trạng thái ô "In Phiếu" - GẮN THEO TỪNG PHIẾU LĨNH (AggExpMest.ID)
        /// để mỗi phiếu ghi nhớ riêng; phiếu chưa cấu hình thì mặc định TẮT.
        /// </summary>
        private string AutoPrintCheckedKey
        {
            get { return "chkInPhieu_" + (this.AggExpMest != null ? this.AggExpMest.ID : 0); }
        }

        /// <summary>KEY lưu danh sách loại phiếu đã chọn - gắn theo từng phiếu lĩnh.</summary>
        private string AutoPrintTypesKey
        {
            get { return "chkInPhieu_PrintTypes_" + (this.AggExpMest != null ? this.AggExpMest.ID : 0); }
        }

        /// <summary>Danh sách loại phiếu được tích chọn để tự động in.</summary>
        private HashSet<ExpMestAggregateListPopupMenuProcessor.PrintType> autoPrintSelectedTypes
            = new HashSet<ExpMestAggregateListPopupMenuProcessor.PrintType>();

        /// <summary>Form dropdown (borderless) chứa danh sách loại phiếu, mỗi dòng là 1 CheckEdit.</summary>
        private Form autoPrintPopupForm;

        /// <summary>
        /// Cờ chặn vòng lặp: khi tích loại phiếu trong dropdown làm ô "In Phiếu" tự tích/bỏ tích,
        /// không cho CheckedChanged của ô "In Phiếu" lưu lại hay mở lại dropdown.
        /// </summary>
        private bool autoPrintUpdatingFromPopup = false;

        #endregion

        #region Event handler

        /// <summary>
        /// Tick/bỏ tick ô "In Phiếu": lưu trạng thái; nếu bật thì mở dropdown chọn loại phiếu.
        /// </summary>
        private void chkInPhieu_CheckedChanged(object sender, EventArgs e)
        {
            // Chặn lưu/hiện dropdown khi đang khôi phục trạng thái lúc Load.
            if (isNotLoadWhileChangeControlStateInFirst)
            {
                return;
            }
            // Đang được cập nhật từ dropdown (tích loại phiếu) -> dropdown tự lưu, không mở lại.
            if (autoPrintUpdatingFromPopup)
            {
                return;
            }
            try
            {
                SaveAutoPrintControlState();
                if (chkInPhieu.Checked)
                {
                    ShowAutoPrintTypeMenu();
                }
                else
                {
                    CloseAutoPrintPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Chuột phải vào ô "In Phiếu" cũng mở dropdown chọn loại phiếu.</summary>
        private void chkInPhieu_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    ShowAutoPrintTypeMenu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Private function

        /// <summary>
        /// Danh sách loại phiếu khả dụng - đồng bộ với dropdown nút "In ấn"
        /// (xem ExpMestAggregateListPopupMenuProcessor.InitMenu).
        /// </summary>
        private List<ExpMestAggregateListPopupMenuProcessor.PrintType> GetAvailableAutoPrintTypes()
        {
            var list = new List<ExpMestAggregateListPopupMenuProcessor.PrintType>
            {
                ExpMestAggregateListPopupMenuProcessor.PrintType.InTraDoiThuoc,
                ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuTongHop,
                ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhThuoc,
                ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhThuocTheoBenhNhan,
                ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuCongKhaiThuocBenhNhan
            };
            if (this.AggExpMest != null && this.AggExpMest.HAS_NOT_PRES == 1)
            {
                list.Add(ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuHuyThuocVatTu_434);
            }
            return list;
        }

        /// <summary>Lấy tên hiển thị loại phiếu (tái sử dụng key đa ngôn ngữ của menu "In ấn").</summary>
        private string GetPrintTypeCaption(ExpMestAggregateListPopupMenuProcessor.PrintType pt)
        {
            string key;
            switch (pt)
            {
                case ExpMestAggregateListPopupMenuProcessor.PrintType.InTraDoiThuoc:
                    key = "IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_TRA_DOI_THUOC";
                    break;
                case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuTongHop:
                    key = "IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_TONG_HOP";
                    break;
                case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhThuoc:
                    key = "IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_LINH_THUOC";
                    break;
                case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhThuocTheoBenhNhan:
                    key = "IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_LINH_THUOC_THEO_BENH_NHAN";
                    break;
                case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuCongKhaiThuocBenhNhan:
                    key = "IVT_LANGUAGE_KEY_EXP_MEST__IN_PHIEU_CONG_KHAI_THEO_BENH_NHAN";
                    break;
                case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuHuyThuocVatTu_434:
                    key = "IVT_LANGUAGE_KEY_EXP_MEST__IN_PHIEU_HUY_THUOC_VAT_TU";
                    break;
                default:
                    return "";
            }
            return Inventec.Common.Resource.Get.Value(key, ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        }

        /// <summary>
        /// Hiển thị dropdown các loại phiếu: mỗi dòng là 1 CheckEdit (ô tích ở đầu, click 1 phát là tích).
        /// Cho tích chọn NHIỀU, dropdown ở lại đến khi click ra ngoài. Tích loại nào -> ô "In Phiếu" tự tích ngay.
        /// </summary>
        private void ShowAutoPrintTypeMenu()
        {
            try
            {
                // Nếu đang mở thì không mở chồng.
                if (autoPrintPopupForm != null && !autoPrintPopupForm.IsDisposed)
                {
                    autoPrintPopupForm.Activate();
                    return;
                }

                var types = GetAvailableAutoPrintTypes();

                System.Windows.Forms.FlowLayoutPanel panel = new System.Windows.Forms.FlowLayoutPanel();
                panel.Dock = DockStyle.Fill;
                panel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
                panel.WrapContents = false;
                panel.AutoScroll = false;
                panel.BackColor = Color.White;
                panel.Padding = new Padding(2);

                foreach (var pt in types)
                {
                    CheckEdit chk = new CheckEdit();
                    chk.Text = GetPrintTypeCaption(pt);
                    chk.Tag = pt;
                    chk.Checked = autoPrintSelectedTypes.Contains(pt);
                    chk.Width = 224;
                    chk.Margin = new Padding(2, 1, 2, 1);
                    chk.CheckedChanged += AutoPrintItem_CheckedChanged;
                    panel.Controls.Add(chk);
                }

                this.autoPrintPopupForm = new AutoPrintDropDownForm();
                this.autoPrintPopupForm.FormBorderStyle = FormBorderStyle.None;
                this.autoPrintPopupForm.ShowInTaskbar = false;
                this.autoPrintPopupForm.MinimizeBox = false;
                this.autoPrintPopupForm.MaximizeBox = false;
                this.autoPrintPopupForm.ControlBox = false;
                this.autoPrintPopupForm.StartPosition = FormStartPosition.Manual;
                this.autoPrintPopupForm.Padding = new Padding(1);
                this.autoPrintPopupForm.BackColor = Color.FromArgb(118, 118, 118); // viền mảnh
                this.autoPrintPopupForm.Size = new Size(240, types.Count * 24 + 8);
                this.autoPrintPopupForm.Controls.Add(panel);
                this.autoPrintPopupForm.Deactivate += AutoPrintPopupForm_Deactivate;

                // Vị trí: canh phải ô "In Phiếu", mở LÊN TRÊN (ô ở sát đáy form).
                Point anchor = chkInPhieu.PointToScreen(new Point(chkInPhieu.Width, 0));
                int x = anchor.X - this.autoPrintPopupForm.Width;
                int y = anchor.Y - this.autoPrintPopupForm.Height;
                Rectangle wa = Screen.FromControl(chkInPhieu).WorkingArea;
                if (x < wa.Left) x = wa.Left;
                if (x + this.autoPrintPopupForm.Width > wa.Right) x = wa.Right - this.autoPrintPopupForm.Width;
                if (y < wa.Top) y = anchor.Y + chkInPhieu.Height; // không đủ chỗ phía trên -> mở xuống dưới
                this.autoPrintPopupForm.Location = new Point(x, y);

                this.autoPrintPopupForm.Show(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tích/bỏ tích 1 loại phiếu trong dropdown (click 1 lần).
        /// Cập nhật ngay danh sách + ô "In Phiếu" tự tích (có loại) / tự bỏ tích (hết loại) + lưu trạng thái.
        /// </summary>
        private void AutoPrintItem_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckEdit chk = sender as CheckEdit;
                if (chk == null || !(chk.Tag is ExpMestAggregateListPopupMenuProcessor.PrintType))
                {
                    return;
                }
                var pt = (ExpMestAggregateListPopupMenuProcessor.PrintType)chk.Tag;
                if (chk.Checked)
                {
                    autoPrintSelectedTypes.Add(pt);
                }
                else
                {
                    autoPrintSelectedTypes.Remove(pt);
                }

                // Đồng bộ ô "In Phiếu": có loại -> tự tích; hết loại -> tự bỏ tích.
                autoPrintUpdatingFromPopup = true;
                chkInPhieu.Checked = autoPrintSelectedTypes.Count > 0;
                autoPrintUpdatingFromPopup = false;

                SaveAutoPrintControlState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Click ra ngoài (mất focus) -> đóng dropdown (trạng thái đã lưu ngay khi tích).</summary>
        private void AutoPrintPopupForm_Deactivate(object sender, EventArgs e)
        {
            try
            {
                // Đóng dropdown mà không chọn loại nào -> bỏ tích ô "In Phiếu" cho nhất quán.
                if (chkInPhieu.Checked && (autoPrintSelectedTypes == null || autoPrintSelectedTypes.Count == 0))
                {
                    chkInPhieu.Checked = false; // CheckedChanged sẽ tự lưu + đóng dropdown
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            CloseAutoPrintPopup();
        }

        /// <summary>Đóng và giải phóng form dropdown.</summary>
        private void CloseAutoPrintPopup()
        {
            try
            {
                Form f = this.autoPrintPopupForm;
                this.autoPrintPopupForm = null;
                if (f != null)
                {
                    f.Deactivate -= AutoPrintPopupForm_Deactivate;
                    if (!f.IsDisposed)
                    {
                        f.Close();
                        f.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Lưu trạng thái ô "In Phiếu" + danh sách loại phiếu đã chọn xuống ControlState.</summary>
        private void SaveAutoPrintControlState()
        {
            try
            {
                if (this.controlStateWorker == null)
                {
                    this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                }
                if (this.currentControlStateRDO == null)
                {
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                }

                if (chkInPhieu.Checked)
                {
                    SetControlStateValueInMemory(AutoPrintCheckedKey, "1");
                    SetControlStateValueInMemory(AutoPrintTypesKey,
                        string.Join(",", autoPrintSelectedTypes.Select(o => o.ToString())));
                }
                else
                {
                    // Bỏ tick -> xóa hẳn ghi nhớ của phiếu này (giữ ControlState gọn, mặc định TẮT).
                    RemoveControlStateValueInMemory(AutoPrintCheckedKey);
                    RemoveControlStateValueInMemory(AutoPrintTypesKey);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Thêm/cập nhật một KEY trong danh sách ControlState (chưa ghi xuống đĩa).</summary>
        private void SetControlStateValueInMemory(string key, string value)
        {
            var cs = this.currentControlStateRDO
                .FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == moduleLink);
            if (cs != null)
            {
                cs.VALUE = value;
            }
            else
            {
                this.currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                {
                    KEY = key,
                    VALUE = value,
                    MODULE_LINK = moduleLink
                });
            }
        }

        /// <summary>Xóa một KEY khỏi danh sách ControlState (chưa ghi xuống đĩa).</summary>
        private void RemoveControlStateValueInMemory(string key)
        {
            if (this.currentControlStateRDO == null)
            {
                return;
            }
            this.currentControlStateRDO.RemoveAll(o => o.KEY == key && o.MODULE_LINK == moduleLink);
        }

        /// <summary>
        /// Khôi phục trạng thái ô "In Phiếu" + loại phiếu đã chọn. Gọi trong InitControlState
        /// (lúc isNotLoadWhileChangeControlStateInFirst = true để không kích hoạt lưu/hiện dropdown).
        /// </summary>
        private void RestoreAutoPrintControlState()
        {
            try
            {
                if (this.autoPrintSelectedTypes == null)
                {
                    this.autoPrintSelectedTypes = new HashSet<ExpMestAggregateListPopupMenuProcessor.PrintType>();
                }
                this.autoPrintSelectedTypes.Clear();

                if (this.currentControlStateRDO == null || this.currentControlStateRDO.Count == 0)
                {
                    return;
                }

                foreach (var item in this.currentControlStateRDO)
                {
                    if (item.KEY == AutoPrintCheckedKey)
                    {
                        chkInPhieu.Checked = item.VALUE == "1";
                    }
                    else if (item.KEY == AutoPrintTypesKey && !string.IsNullOrEmpty(item.VALUE))
                    {
                        foreach (var s in item.VALUE.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            ExpMestAggregateListPopupMenuProcessor.PrintType pt;
                            if (Enum.TryParse(s, out pt))
                            {
                                this.autoPrintSelectedTypes.Add(pt);
                            }
                        }
                    }
                }

                // Nếu đã tick nhưng không còn loại phiếu nào -> bỏ tick cho nhất quán.
                if (chkInPhieu.Checked && this.autoPrintSelectedTypes.Count == 0)
                {
                    chkInPhieu.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tự động mở Xem trước cho từng loại phiếu đã chọn sau khi Thực xuất thành công.
        /// Tái sử dụng plugin in hiện có (ExecutePrintByType / clickItemInGopDonThuoc).
        /// </summary>
        private void AutoPrintAfterExport()
        {
            try
            {
                if (chkInPhieu == null || !chkInPhieu.Checked)
                {
                    return;
                }

                // Phiếu lĩnh tổng hợp phòng khám -> in gộp đơn thuốc (giống nút "In ấn").
                if (this.AggExpMest != null
                    && this.AggExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                {
                    clickItemInGopDonThuoc(this.AggExpMest);
                    return;
                }

                if (this.autoPrintSelectedTypes == null || this.autoPrintSelectedTypes.Count == 0)
                {
                    return;
                }

                // Lần lượt theo thứ tự hiển thị trong dropdown.
                foreach (var pt in GetAvailableAutoPrintTypes())
                {
                    if (this.autoPrintSelectedTypes.Contains(pt))
                    {
                        ExecutePrintByType(pt);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Nested type

        /// <summary>
        /// Form dropdown không viền cho danh sách chọn loại phiếu.
        /// Xử lý WM_MOUSEACTIVATE để cú click ĐẦU TIÊN vừa kích hoạt vừa tác động tới control
        /// (tránh phải click 2 lần mới tích được checkbox).
        /// </summary>
        private class AutoPrintDropDownForm : Form
        {
            private const int WM_MOUSEACTIVATE = 0x0021;
            private const int MA_ACTIVATE = 1;
            private const int MA_ACTIVATEANDEAT = 2;

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_MOUSEACTIVATE)
                {
                    base.WndProc(ref m);
                    // Đổi "kích hoạt và nuốt click" -> "kích hoạt" để click không bị mất.
                    if (m.Result.ToInt32() == MA_ACTIVATEANDEAT)
                    {
                        m.Result = (IntPtr)MA_ACTIVATE;
                    }
                    return;
                }
                base.WndProc(ref m);
            }
        }

        #endregion
    }
}
