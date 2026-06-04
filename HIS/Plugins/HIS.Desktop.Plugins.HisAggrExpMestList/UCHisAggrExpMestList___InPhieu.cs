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

        /// <summary>Module ID — key phân biệt plugin khi lưu ControlState.</summary>
        readonly string moduleLink = "HIS.Desktop.Plugins.HisAggrExpMestList";

        const string CONTROL_STATE_KEY__CHK_IN_PHIEU = "chkInPhieu";
        const string CONTROL_STATE_KEY__IN_PHIEU_TYPES = "InPhieuPrintTypes";

        const string MODULE_LINK__AGGR_EXP_MEST_PRINT_FILTER = "HIS.Desktop.Plugins.AggrExpMestPrintFilter";

        /// <summary>Dropdown chọn loại phiếu in (mỗi item là 1 ô tích). Dùng chung baManager của UC.</summary>
        PopupMenu popupMenuInPhieu;
        Dictionary<EnumInPhieuPrintType, BarCheckItem> dicInPhieuItems;
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

                // Tạo dropdown loại phiếu (giống dropdown 'In ẩn' ở màn Chi tiết phiếu lĩnh)
                BuildInPhieuPopupMenu();

                // Đọc trạng thái đã lưu
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

                // Mặc định: chọn "Phiếu lĩnh thuốc, vật tư" khi chưa có lựa chọn nào lưu trước đó
                if (savedTypes == null)
                {
                    savedTypes = new HashSet<int> { (int)EnumInPhieuPrintType.PhieuLinhThuocVatTu };
                }

                if (dicInPhieuItems != null)
                {
                    foreach (var kv in dicInPhieuItems)
                    {
                        kv.Value.Checked = savedTypes.Contains((int)kv.Key);
                    }
                }

                chkInPhieu.Checked = chkChecked;

                // TẮT flag — từ giờ thay đổi do user sẽ được lưu
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                isNotLoadWhileChangeControlStateInFirst = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Tạo dropdown các loại phiếu (BarCheckItem cho phép tích chọn nhiều loại).</summary>
        private void BuildInPhieuPopupMenu()
        {
            try
            {
                if (popupMenuInPhieu != null)
                    return;

                if (this.baManager == null)
                {
                    this.baManager = new BarManager();
                    this.baManager.Form = this;
                }

                popupMenuInPhieu = new PopupMenu(this.baManager);
                dicInPhieuItems = new Dictionary<EnumInPhieuPrintType, BarCheckItem>();

                AddInPhieuItem(EnumInPhieuPrintType.PhieuTraDoiThuoc,
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__IN_PHIEU__TRA_DOI_THUOC");
                AddInPhieuItem(EnumInPhieuPrintType.PhieuTongHop,
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__IN_PHIEU__TONG_HOP");
                AddInPhieuItem(EnumInPhieuPrintType.PhieuLinhThuocVatTu,
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__IN_PHIEU__LINH_THUOC_VAT_TU");
                AddInPhieuItem(EnumInPhieuPrintType.PhieuLinhTheoBenhNhan,
                    "IVT_LANGUAGE_KEY__UC_HIS_AGGR_EXP_MEST_LIST__IN_PHIEU__LINH_THEO_BENH_NHAN");
                AddInPhieuItem(EnumInPhieuPrintType.PhieuCongKhaiTheoBenhNhan,
                    "IVT_LANGUAGE_KEY__UC_AGGREXMEST__POPUP_MENU__ITEM_PHIEU_CONG_KHAI_THEO_BN");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddInPhieuItem(EnumInPhieuPrintType type, string languageKey)
        {
            try
            {
                BarCheckItem item = new BarCheckItem(this.baManager);
                item.Caption = Inventec.Common.Resource.Get.Value(
                    languageKey,
                    Resources.ResourceLanguageManager.LanguageUCHisAggrExpMestList,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                item.Tag = type;
                item.CheckedChanged += new ItemClickEventHandler(InPhieuItem_CheckedChanged);
                popupMenuInPhieu.AddItem(item);
                dicInPhieuItems[type] = item;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                if (dicInPhieuItems != null)
                {
                    foreach (var kv in dicInPhieuItems)
                    {
                        if (kv.Value.Checked)
                            result.Add(kv.Key);
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
                if (popupMenuInPhieu == null)
                    BuildInPhieuPopupMenu();
                if (popupMenuInPhieu == null)
                    return;

                Point location = chkInPhieu.PointToScreen(new Point(0, chkInPhieu.Height));
                popupMenuInPhieu.ShowPopup(location);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkInPhieu_CheckedChanged(object sender, EventArgs e)
        {
            if (isNotLoadWhileChangeControlStateInFirst) return;
            try
            {
                SaveControlStateValue(CONTROL_STATE_KEY__CHK_IN_PHIEU, chkInPhieu.Checked ? "1" : "");

                // Người dùng tick ô 'In Phiếu' -> hiển thị dropdown để chọn loại phiếu.
                // Defer bằng BeginInvoke: popup không bị đóng ngay bởi MouseUp của chính cú click này.
                if (chkInPhieu.Checked && this.IsHandleCreated)
                {
                    this.BeginInvoke(new MethodInvoker(ShowInPhieuDropdown));
                }
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

        private void InPhieuItem_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            if (isNotLoadWhileChangeControlStateInFirst) return;
            try
            {
                List<EnumInPhieuPrintType> selectedTypes = GetSelectedPrintTypes();

                // Ghi nhớ trạng thái từng loại phiếu
                SaveControlStateValue(
                    CONTROL_STATE_KEY__IN_PHIEU_TYPES,
                    String.Join(",", selectedTypes.Select(o => ((int)o).ToString()).ToArray()));

                if (selectedTypes.Count == 0)
                {
                    // Dropdown không còn loại nào được chọn -> ô 'In Phiếu' tự bỏ tick
                    if (chkInPhieu.Checked)
                        chkInPhieu.Checked = false;
                }
                else if (this.IsHandleCreated)
                {
                    // PopupMenu đóng sau mỗi lần click item -> mở lại để cho phép tích chọn nhiều loại liên tiếp
                    this.BeginInvoke(new MethodInvoker(ShowInPhieuDropdown));
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
                if (expMest == null)
                    return;
                if (chkInPhieu == null || !chkInPhieu.Checked)
                    return;

                List<EnumInPhieuPrintType> selectedTypes = GetSelectedPrintTypes();
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
