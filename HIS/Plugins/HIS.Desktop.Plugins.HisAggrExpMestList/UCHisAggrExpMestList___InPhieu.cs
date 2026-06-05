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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisAggrExpMestList
{
    public partial class UCHisAggrExpMestList : HIS.Desktop.Utility.UserControlBase
    {
        #region ControlState - In Phiếu
        /// <summary>Worker đọc/ghi trạng thái control xuống local (SQLite).</summary>
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;

        /// <summary>Danh sách trạng thái control hiện tại của module.</summary>
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        /// <summary>
        /// Chặn lưu trạng thái khi đang load lần đầu (set giá trị từ cache).
        /// = true khi đang InitInPhieuControl; = false sau khi load xong.
        /// </summary>
        bool isNotLoadWhileChangeControlStateInFirst = false;

        /// <summary>Chặn đệ quy khi đồng bộ 2 chiều giữa ô 'In phiếu' và danh sách loại phiếu.</summary>
        bool isSyncingInPhieu = false;

        /// <summary>Module ID — key phân biệt plugin khi lưu ControlState.</summary>
        readonly string moduleLink = "HIS.Desktop.Plugins.HisAggrExpMestList";

        const string CONTROL_STATE_KEY__CHK_IN_PHIEU = "chkInPhieu";
        const string CONTROL_STATE_KEY__IN_PHIEU_TYPES = "InPhieuPrintTypes";

        const string MODULE_LINK__AGGR_EXP_MEST_PRINT_FILTER = "HIS.Desktop.Plugins.AggrExpMestPrintFilter";

        /// <summary>Dropdown chọn loại phiếu in — CheckedListBoxControl (ô tích vuông) hiển thị dạng popup.</summary>
        PopupControlContainer popupContainerInPhieu;
        CheckedListBoxControl chkLstInPhieu;

        /// <summary>Thứ tự loại phiếu trong dropdown — index của CheckedListBoxControl ánh xạ sang loại phiếu.</summary>
        readonly EnumInPhieuPrintType[] inPhieuItemOrder = new EnumInPhieuPrintType[]
        {
            EnumInPhieuPrintType.PhieuTraDoiThuoc,
            EnumInPhieuPrintType.PhieuTongHop,
            EnumInPhieuPrintType.PhieuLinhThuocVatTu,
            EnumInPhieuPrintType.PhieuLinhTheoBenhNhan,
            EnumInPhieuPrintType.PhieuCongKhaiTheoBenhNhan
        };

        /// <summary>
        /// Nguồn dữ liệu CHUẨN cho các loại phiếu đang chọn — độc lập với việc dropdown đã được build hay chưa.
        /// </summary>
        HashSet<int> selectedPrintTypeValues = new HashSet<int>();
        #endregion

        #region Init + ControlState
        /// <summary>
        /// Khởi tạo checkbox 'In Phiếu' + dropdown loại phiếu, khôi phục trạng thái đã lưu.
        /// Gọi trong Load event, SAU SetDefaultValueControl.
        /// </summary>
        private void InitInPhieuControl()
        {
            try
            {
                // BẬT flag — chặn CheckedChanged lưu khi set giá trị từ cache
                isNotLoadWhileChangeControlStateInFirst = true;

                // Caption checkbox theo ngôn ngữ
                chkInPhieu.Properties.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__CHK_IN_PHIEU",
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                // Đọc trạng thái đã lưu (dropdown được build lazy ở lần mở đầu tiên — SAU Load, tránh lỗi BarManager.Form khi UC chưa gắn form)
                controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                currentControlStateRDO = controlStateWorker.GetData(moduleLink)
                    ?? new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                bool chkChecked = false;
                HashSet<int> savedTypes = null;
                foreach (var item in currentControlStateRDO)
                {
                    if (item.KEY == CONTROL_STATE_KEY__CHK_IN_PHIEU)
                    {
                        chkChecked = item.VALUE == "1";
                    }
                    else if (item.KEY == CONTROL_STATE_KEY__IN_PHIEU_TYPES)
                    {
                        savedTypes = ParsePrintTypes(item.VALUE);
                    }
                }

                // Đồng bộ 2 chiều: ô 'In phiếu' tick KHI VÀ CHỈ KHI có >=1 loại phiếu được chọn.
                // Mặc định (chưa lưu gì): không chọn loại nào -> ô 'In phiếu' bỏ tick.
                if (savedTypes == null)
                {
                    savedTypes = new HashSet<int>();
                }
                selectedPrintTypeValues = savedTypes;

                bool derivedCheck = selectedPrintTypeValues.Count >= 1;
                chkInPhieu.Checked = derivedCheck;

                // TẮT flag — từ giờ thay đổi do user sẽ được lưu
                isNotLoadWhileChangeControlStateInFirst = false;

                // Tự sửa trạng thái lưu bị lệch cho khớp đồng bộ (đã lưu tick nhưng không có loại nào, hoặc ngược lại)
                if (chkChecked != derivedCheck)
                {
                    SaveControlStateValue(CONTROL_STATE_KEY__CHK_IN_PHIEU, derivedCheck ? "1" : "");
                }
            }
            catch (Exception ex)
            {
                isNotLoadWhileChangeControlStateInFirst = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Tạo dropdown loại phiếu dạng CheckedListBoxControl (ô tích vuông) trong PopupControlContainer.</summary>
        private void BuildInPhieuDropdown()
        {
            try
            {
                if (popupContainerInPhieu != null)
                    return;

                if (this.baManager == null)
                {
                    this.baManager = new BarManager();
                    this.baManager.Form = this;
                }

                chkLstInPhieu = new CheckedListBoxControl();
                chkLstInPhieu.Dock = DockStyle.Fill;
                chkLstInPhieu.CheckOnClick = true; // tích/bỏ tích chỉ với 1 click (không cần chọn dòng trước)

                // Thêm 5 loại phiếu theo thứ tự
                for (int i = 0; i < inPhieuItemOrder.Length; i++)
                {
                    chkLstInPhieu.Items.Add(GetInPhieuCaption(inPhieuItemOrder[i]));
                }
                // Áp trạng thái tích từ nguồn chuẩn TRƯỚC khi gắn handler (tránh ItemCheck fire khi khởi tạo)
                for (int i = 0; i < inPhieuItemOrder.Length; i++)
                {
                    chkLstInPhieu.SetItemChecked(i, selectedPrintTypeValues != null
                        && selectedPrintTypeValues.Contains((int)inPhieuItemOrder[i]));
                }
                chkLstInPhieu.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(InPhieuList_ItemCheck);

                popupContainerInPhieu = new PopupControlContainer();
                popupContainerInPhieu.Controls.Add(chkLstInPhieu);
                popupContainerInPhieu.Size = new Size(230, inPhieuItemOrder.Length * 22 + 8);
                popupContainerInPhieu.Visible = false;
                this.Controls.Add(popupContainerInPhieu);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Lấy tên hiển thị (đa ngôn ngữ) của 1 loại phiếu.</summary>
        private string GetInPhieuCaption(EnumInPhieuPrintType type)
        {
            string languageKey;
            switch (type)
            {
                case EnumInPhieuPrintType.PhieuTraDoiThuoc:
                    languageKey = "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__IN_PHIEU__TRA_DOI_THUOC";
                    break;
                case EnumInPhieuPrintType.PhieuTongHop:
                    languageKey = "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__IN_PHIEU__TONG_HOP";
                    break;
                case EnumInPhieuPrintType.PhieuLinhThuocVatTu:
                    languageKey = "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__IN_PHIEU__LINH_THUOC_VAT_TU";
                    break;
                case EnumInPhieuPrintType.PhieuLinhTheoBenhNhan:
                    languageKey = "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__IN_PHIEU__LINH_THEO_BENH_NHAN";
                    break;
                case EnumInPhieuPrintType.PhieuCongKhaiTheoBenhNhan:
                    languageKey = "IVT_LANGUAGE_KEY__UC_AGGREXMEST__POPUP_MENU__ITEM_PHIEU_CONG_KHAI_THEO_BN";
                    break;
                default:
                    languageKey = "";
                    break;
            }
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    languageKey,
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return type.ToString();
        }

        private HashSet<int> ParsePrintTypes(string value)
        {
            HashSet<int> result = new HashSet<int>();
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    foreach (var part in value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        int v;
                        if (int.TryParse(part.Trim(), out v))
                            result.Add(v);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<EnumInPhieuPrintType> GetSelectedPrintTypes()
        {
            List<EnumInPhieuPrintType> result = new List<EnumInPhieuPrintType>();
            try
            {
                if (selectedPrintTypeValues != null)
                {
                    foreach (int v in selectedPrintTypeValues)
                    {
                        if (Enum.IsDefined(typeof(EnumInPhieuPrintType), v))
                            result.Add((EnumInPhieuPrintType)v);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Thêm/cập nhật 1 KEY trong ControlState và ghi xuống local.</summary>
        private void SaveControlStateValue(string key, string value)
        {
            try
            {
                if (controlStateWorker == null)
                    controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                if (currentControlStateRDO == null)
                    currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                var item = currentControlStateRDO.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == moduleLink);
                if (item != null)
                {
                    item.VALUE = value;
                }
                else
                {
                    currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = key,
                        MODULE_LINK = moduleLink,
                        VALUE = value
                    });
                }

                controlStateWorker.SetData(currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region UI events
        /// <summary>Hiển thị dropdown chọn loại phiếu ngay dưới checkbox.</summary>
        private void ShowInPhieuDropdown()
        {
            try
            {
                if (popupContainerInPhieu == null)
                    BuildInPhieuDropdown();
                if (popupContainerInPhieu == null)
                    return;

                Point location = chkInPhieu.PointToScreen(new Point(0, chkInPhieu.Height));
                popupContainerInPhieu.ShowPopup(this.baManager, location);
                if (chkLstInPhieu != null)
                    chkLstInPhieu.Focus(); // để click đầu tiên vào dòng tích/bỏ tích ngay (không bị "mất" do kích hoạt popup)
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkInPhieu_CheckedChanged(object sender, EventArgs e)
        {
            if (isNotLoadWhileChangeControlStateInFirst) return;
            if (isSyncingInPhieu) return;
            try
            {
                if (selectedPrintTypeValues == null)
                    selectedPrintTypeValues = new HashSet<int>();

                // Ô 'In phiếu' là CHỈ BÁO: tick <=> có >=1 loại được chọn.
                // Click vào ô KHÔNG bật/tắt trực tiếp mà chỉ MỞ dropdown để người dùng tự chọn loại
                // (tránh trạng thái "tick mà rỗng" và không ép chọn mặc định).
                bool derived = selectedPrintTypeValues.Count >= 1;
                if (chkInPhieu.Checked != derived)
                {
                    isSyncingInPhieu = true;
                    try { chkInPhieu.Checked = derived; }
                    finally { isSyncingInPhieu = false; }
                }

                if (this.IsHandleCreated)
                    this.BeginInvoke(new MethodInvoker(ShowInPhieuDropdown));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkInPhieu_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                // Chuột phải vào ô 'In Phiếu' -> hiển thị dropdown
                if (e.Button == MouseButtons.Right)
                {
                    ShowInPhieuDropdown();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuList_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            if (isNotLoadWhileChangeControlStateInFirst) return;
            if (isSyncingInPhieu) return;
            try
            {
                if (e.Index < 0 || e.Index >= inPhieuItemOrder.Length)
                    return;

                int typeVal = (int)inPhieuItemOrder[e.Index];
                bool isChecked = (e.State == System.Windows.Forms.CheckState.Checked);

                if (selectedPrintTypeValues == null)
                    selectedPrintTypeValues = new HashSet<int>();
                if (isChecked)
                    selectedPrintTypeValues.Add(typeVal);
                else
                    selectedPrintTypeValues.Remove(typeVal);

                Inventec.Common.Logging.LogSystem.Debug("INPHIEU_AUTOPRINT: toggle "
                    + inPhieuItemOrder[e.Index] + " checked=" + isChecked
                    + ", remaining=" + selectedPrintTypeValues.Count);

                // Ghi nhớ trạng thái từng loại phiếu
                SaveControlStateValue(
                    CONTROL_STATE_KEY__IN_PHIEU_TYPES,
                    String.Join(",", selectedPrintTypeValues.Select(o => o.ToString()).ToArray()));

                // Đồng bộ 2 chiều: ô 'In phiếu' tick <=> có >=1 loại được chọn
                bool shouldCheck = selectedPrintTypeValues.Count >= 1;
                if (chkInPhieu.Checked != shouldCheck)
                {
                    isSyncingInPhieu = true;
                    try { chkInPhieu.Checked = shouldCheck; }
                    finally { isSyncingInPhieu = false; }
                    SaveControlStateValue(CONTROL_STATE_KEY__CHK_IN_PHIEU, shouldCheck ? "1" : "");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Auto print after export
        /// <summary>
        /// Sau khi THỰC XUẤT 1 phiếu thành công + ô 'In Phiếu' đang tick:
        /// mở màn hình xem trước cho TỪNG loại phiếu đã chọn, ứng với phiếu vừa thực xuất.
        /// </summary>
        internal void ProcessAutoPrintAfterExport(V_HIS_EXP_MEST expMest)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("INPHIEU_AUTOPRINT: ProcessAutoPrintAfterExport chkChecked="
                    + (chkInPhieu != null ? chkInPhieu.Checked.ToString() : "null")
                    + ", expMest=" + (expMest != null ? expMest.ID.ToString() : "null"));

                if (expMest == null)
                    return;
                if (chkInPhieu == null || !chkInPhieu.Checked)
                    return;

                List<EnumInPhieuPrintType> selectedTypes = GetSelectedPrintTypes();
                Inventec.Common.Logging.LogSystem.Debug("INPHIEU_AUTOPRINT: selectedTypes=["
                    + String.Join(",", selectedTypes.Select(o => ((int)o).ToString()).ToArray()) + "]");
                if (selectedTypes == null || selectedTypes.Count == 0)
                    return;

                foreach (var type in selectedTypes)
                {
                    try
                    {
                        switch (type)
                        {
                            case EnumInPhieuPrintType.PhieuTraDoiThuoc:
                                ShowAggrExpMestPrintFilter(expMest, 1);
                                break;
                            case EnumInPhieuPrintType.PhieuTongHop:
                                ShowAggrExpMestPrintFilter(expMest, 2);
                                break;
                            case EnumInPhieuPrintType.PhieuLinhThuocVatTu:
                                ShowAggrExpMestPrintFilter(expMest, 3);
                                break;
                            case EnumInPhieuPrintType.PhieuLinhTheoBenhNhan:
                                ShowAggrExpMestPrintFilter(expMest, 4);
                                break;
                            case EnumInPhieuPrintType.PhieuCongKhaiTheoBenhNhan:
                                InPhieuCongKhaiTheoBN(expMest.ID);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception exInner)
                    {
                        Inventec.Common.Logging.LogSystem.Error(exInner);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Mở plugin in AggrExpMestPrintFilter (xem trước) cho 1 phiếu lĩnh với printType tương ứng.
        /// KHÔNG truyền AggrExpMestPrintSDO -> không in thẳng -> hiển thị xem trước.
        /// </summary>
        private void ShowAggrExpMestPrintFilter(V_HIS_EXP_MEST expMest, long printType)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == MODULE_LINK__AGGR_EXP_MEST_PRINT_FILTER)
                    .FirstOrDefault();
                Inventec.Common.Logging.LogSystem.Debug("INPHIEU_AUTOPRINT: ShowAggrExpMestPrintFilter printType=" + printType
                    + ", moduleFound=" + (moduleData != null) + ", isPlugin=" + (moduleData != null && moduleData.IsPlugin));

                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + MODULE_LINK__AGGR_EXP_MEST_PRINT_FILTER);
                    return;
                }

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    var moduleWithRoom = PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(expMest);
                    listArgs.Add(printType);
                    listArgs.Add(moduleWithRoom);

                    var extenceInstance = PluginInstance.GetPluginInstance(moduleWithRoom, listArgs);
                    Inventec.Common.Logging.LogSystem.Debug("INPHIEU_AUTOPRINT: instance="
                        + (extenceInstance == null ? "null" : extenceInstance.GetType().Name));
                    if (extenceInstance == null)
                        return;

                    // printKey 3/4 -> Behavior in trực tiếp và trả về bool, không có form để show
                    if (extenceInstance is bool)
                        return;

                    if (extenceInstance is Form)
                        ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
