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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Exemptions.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Exemptions
{
    public partial class frmExemptions : HIS.Desktop.Utility.FormBase
    {
        /// <summary>Chiết khấu (HIS_SERE_SERV_DISCOUNT) đã có, gom theo SERE_SERV_ID.</summary>
        Dictionary<long, List<HIS_SERE_SERV_DISCOUNT>> dicDiscountBySereServ = null;

        /// <summary>Bộ đếm tạo CONCRETE_ID tạm cho dòng chiết khấu mới (chưa có ID).</summary>
        int tempDiscountKeySeq = 0;

        /// <summary>
        /// Lấy danh sách chiết khấu của hồ sơ điều trị hiện tại, gom theo SERE_SERV_ID.
        /// Filter ADO bổ sung TREATMENT_ID vì MOS.Filter chưa khai báo (backend đọc qua JSON).
        /// </summary>
        private Dictionary<long, List<HIS_SERE_SERV_DISCOUNT>> LoadDiscountData()
        {
            var dic = new Dictionary<long, List<HIS_SERE_SERV_DISCOUNT>>();
            try
            {
                if (!this.treatmentId.HasValue || this.treatmentId.Value <= 0)
                {
                    return dic;
                }

                HisSereServDiscountFilterADO filter = new HisSereServDiscountFilterADO();
                filter.TREATMENT_ID = this.treatmentId.Value;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                var listDiscount = new BackendAdapter(new CommonParam())
                    .Get<List<HIS_SERE_SERV_DISCOUNT>>(
                        RequestUriStore.HIS_SERE_SERV_DISCOUNT_GET,
                        ApiConsumers.MosConsumer, filter, null);

                if (listDiscount != null && listDiscount.Count > 0)
                {
                    foreach (var item in listDiscount)
                    {
                        if (item.IS_DELETE == IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE)
                        {
                            continue;
                        }
                        if (!dic.ContainsKey(item.SERE_SERV_ID))
                        {
                            dic[item.SERE_SERV_ID] = new List<HIS_SERE_SERV_DISCOUNT>();
                        }
                        dic[item.SERE_SERV_ID].Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return dic;
        }

        /// <summary>
        /// Gắn các dòng chiết khấu con vào dưới mỗi dòng dịch vụ (leaf).
        /// Đặt ô Chiết khấu của dịch vụ = tổng các dòng con, để trống Lý do miễn giảm ở dòng dịch vụ.
        /// Gọi trong BindTreePlus (sau khi đã dựng SereServADOs) khi bật key đa chiết khấu.
        /// </summary>
        private void AttachDiscountRows()
        {
            try
            {
                if (SereServADOs == null || SereServADOs.Count == 0)
                {
                    return;
                }

                var serviceLeaves = SereServADOs
                    .Where(o => o.IsLeaf == true && o.IsDiscountRow != true)
                    .ToList();

                foreach (var service in serviceLeaves)
                {
                    service.DISCOUNT_REASON = null; // dòng dịch vụ: Lý do miễn giảm để trống

                    decimal sum = 0;
                    List<HIS_SERE_SERV_DISCOUNT> discounts = null;
                    if (dicDiscountBySereServ != null
                        && dicDiscountBySereServ.TryGetValue(service.ID, out discounts)
                        && discounts != null)
                    {
                        foreach (var d in discounts)
                        {
                            SereServADO child = BuildDiscountRow(service, d);
                            SereServADOs.Add(child);
                            sum += d.DISCOUNT ?? 0;
                        }
                    }
                    service.VIR_TOTAL_DISCOUNT = sum;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Dựng 1 dòng chiết khấu (ADO) gắn với dịch vụ. d = null khi tạo mới.</summary>
        private SereServADO BuildDiscountRow(SereServADO service, HIS_SERE_SERV_DISCOUNT d)
        {
            SereServADO row = new SereServADO();
            row.IsDiscountRow = true;
            row.IsLeaf = false;
            row.SERE_SERV_ID_REF = service.ID;
            row.PARENT_PATIENT_PRICE = service.VIR_TOTAL_PATIENT_PRICE_NO_DC;
            row.PARENT_ID__IN_SETY = service.CONCRETE_ID__IN_SETY;
            row.TDL_SERVICE_NAME = "";

            if (d != null)
            {
                row.DISCOUNT_ID = d.ID;
                row.VIR_TOTAL_DISCOUNT = d.DISCOUNT;
                row.DISCOUNT_RATIO_PERCENT = d.DISCOUNT_RATIO;
                row.DISCOUNT_REASON = d.REASON;
                row.CONCRETE_ID__IN_SETY = service.CONCRETE_ID__IN_SETY + "_DC_" + d.ID;
            }
            else
            {
                row.DISCOUNT_ID = null;
                row.VIR_TOTAL_DISCOUNT = 0;
                row.DISCOUNT_RATIO_PERCENT = 0;
                row.DISCOUNT_REASON = "";
                row.CONCRETE_ID__IN_SETY = service.CONCRETE_ID__IN_SETY + "_DCNEW_" + (++tempDiscountKeySeq);
            }
            return row;
        }

        /// <summary>Ô Chiết khấu của dịch vụ = tổng tiền các dòng chiết khấu con.</summary>
        private void RecomputeServiceDiscountSum(TreeListNode serviceNode)
        {
            try
            {
                if (serviceNode == null)
                {
                    return;
                }
                var service = trvService.GetDataRecordByNode(serviceNode) as SereServADO;
                if (service == null)
                {
                    return;
                }
                decimal sum = 0;
                foreach (TreeListNode child in serviceNode.Nodes)
                {
                    var c = trvService.GetDataRecordByNode(child) as SereServADO;
                    if (c != null && c.IsDiscountRow == true)
                    {
                        sum += c.VIR_TOTAL_DISCOUNT ?? 0;
                    }
                }
                service.VIR_TOTAL_DISCOUNT = sum;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Đa chiết khấu: áp 1 tỉ lệ % cho TẤT CẢ dịch vụ đang được tích chọn.
        /// Mỗi dịch vụ được thêm 1 dòng chiết khấu (%) MỚI (cộng dồn với dòng đã có);
        /// số tiền tính trên tiền bệnh nhân phải trả gốc (VIR_TOTAL_PATIENT_PRICE_NO_DC) của chính dịch vụ đó.
        /// Chỉ dùng khi key MOS.HIS_TRANSACTION_ENABLE_MULTI_DISCOUNT bật. Lưu khi nhấn nút Lưu.
        /// </summary>
        private void ApplyRatioToCheckedServices(string discountReason)
        {
            try
            {
                // 1. Validate tỉ lệ % (0; 100]
                decimal ratio = spinEditTyLe.Value;
                if (ratio <= 0 || ratio > 100)
                {
                    XtraMessageBox.Show(
                        ResourceMessage.TiLePhanTramMienGiamKhongHopLe,
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Validate độ dài lý do (UTF-8 <= 250 byte)
                string reason = discountReason ?? "";
                if (System.Text.Encoding.UTF8.GetByteCount(reason) > MAX_DISCOUNT_REASON_BYTES)
                {
                    XtraMessageBox.Show(
                        ResourceMessage.LyDoMienGiamVuotQuaGioiHan,
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. Lấy các dịch vụ (leaf) đang tích chọn — GetListCheck đã loại dòng chiết khấu con
                trvService.PostEditor();
                List<SereServADO> checkedServices = GetListCheck();
                if (checkedServices == null || checkedServices.Count == 0)
                {
                    return;
                }

                // 4. Fan-out: mỗi dịch vụ tích chọn -> thêm 1 dòng chiết khấu (%) mới
                HashSet<long> affectedSereServIds = new HashSet<long>();
                foreach (var service in checkedServices)
                {
                    SereServADO row = BuildDiscountRow(service, null);
                    row.DISCOUNT_RATIO_PERCENT = ratio;
                    decimal basePrice = service.VIR_TOTAL_PATIENT_PRICE_NO_DC ?? 0;
                    row.VIR_TOTAL_DISCOUNT = basePrice / 100 * ratio;
                    row.DISCOUNT_REASON = reason;
                    records.Add(row);
                    affectedSereServIds.Add(service.ID);
                }

                // 5. Cập nhật ô Chiết khấu (tổng) của mỗi dịch vụ = tổng các dòng con
                var sumBySereServ = records
                    .Where(r => r.IsDiscountRow == true && r.SERE_SERV_ID_REF.HasValue)
                    .GroupBy(r => r.SERE_SERV_ID_REF.Value)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.VIR_TOTAL_DISCOUNT ?? 0));

                string exceededServices = "";
                foreach (var service in records.Where(r => r.IsLeaf == true && r.IsDiscountRow != true))
                {
                    decimal sum;
                    if (sumBySereServ.TryGetValue(service.ID, out sum))
                    {
                        service.VIR_TOTAL_DISCOUNT = sum;
                        // BR8: cảnh báo nếu tổng miễn giảm vượt tiền BN phải trả (chỉ với DV vừa áp)
                        if (affectedSereServIds.Contains(service.ID)
                            && sum > (service.VIR_TOTAL_PATIENT_PRICE_NO_DC ?? 0))
                        {
                            if (exceededServices.Length > 0) exceededServices += "; ";
                            exceededServices += service.TDL_SERVICE_NAME;
                        }
                    }
                }

                trvService.RefreshDataSource();
                trvService.ExpandAll();

                // 6. Cảnh báo (KHÔNG chặn) — bước Lưu sẽ chặn nếu vẫn vượt
                if (exceededServices.Length > 0)
                {
                    XtraMessageBox.Show(
                        string.Format(ResourceMessage.TongMienGiamVuotTienBenhNhanPhaiTra, exceededServices),
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Build danh sách HIS_SERE_SERV_DISCOUNT để truyền vào SDO khi lưu.</summary>
        private List<HIS_SERE_SERV_DISCOUNT> BuildDiscountListForSave()
        {
            List<HIS_SERE_SERV_DISCOUNT> result = new List<HIS_SERE_SERV_DISCOUNT>();
            try
            {
                if (records == null)
                {
                    return result;
                }
                foreach (var row in records)
                {
                    if (row == null || row.IsDiscountRow != true)
                    {
                        continue;
                    }
                    HIS_SERE_SERV_DISCOUNT d = new HIS_SERE_SERV_DISCOUNT();
                    d.ID = row.DISCOUNT_ID ?? 0;                 // cũ: ID gốc; mới: 0
                    d.SERE_SERV_ID = row.SERE_SERV_ID_REF ?? 0;
                    d.TREATMENT_ID = this.treatmentId ?? 0;
                    d.DISCOUNT = row.VIR_TOTAL_DISCOUNT;
                    // DISCOUNT_RATIO backend kiểu long -> làm tròn từ giá trị % (decimal)
                    d.DISCOUNT_RATIO = row.DISCOUNT_RATIO_PERCENT.HasValue
                        ? (long?)Math.Round(row.DISCOUNT_RATIO_PERCENT.Value)
                        : null;
                    d.REASON = row.DISCOUNT_REASON;
                    result.Add(d);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Giới hạn byte (UTF-8) của cột REASON — khớp độ dài cột DB.</summary>
        private const int MAX_DISCOUNT_REASON_BYTES = 250;

        /// <summary>
        /// Kiểm tra Lý do miễn giảm trên mỗi dòng chiết khấu không vượt quá 250 byte (UTF-8).
        /// Tiếng Việt có dấu mỗi ký tự chiếm 2-3 byte nên MaxLength theo ký tự không đủ chặn.
        /// </summary>
        private bool ValidateMultiDiscountReason()
        {
            try
            {
                if (records == null)
                {
                    return true;
                }
                foreach (var row in records)
                {
                    if (row == null || row.IsDiscountRow != true)
                    {
                        continue;
                    }
                    string reason = row.DISCOUNT_REASON ?? "";
                    if (System.Text.Encoding.UTF8.GetByteCount(reason) > MAX_DISCOUNT_REASON_BYTES)
                    {
                        XtraMessageBox.Show(
                            ResourceMessage.LyDoMienGiamVuotQuaGioiHan,
                            ResourceMessage.ThongBao,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }

        /// <summary>Nút "+" trên ô Chiết khấu của dịch vụ — thêm 1 dòng chiết khấu mới (lưu khi nhấn Lưu).</summary>
        private void repoBtnAddDiscount_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind != ButtonPredefines.Plus)
                {
                    return;
                }
                TreeListNode node = trvService.FocusedNode;
                if (node == null)
                {
                    return;
                }
                var service = trvService.GetDataRecordByNode(node) as SereServADO;
                if (service == null || service.IsLeaf != true || service.IsDiscountRow == true)
                {
                    return;
                }

                trvService.PostEditor();
                SereServADO child = BuildDiscountRow(service, null);
                records.Add(child);
                trvService.RefreshDataSource();

                // Sau RefreshDataSource, dòng dịch vụ chuyển từ leaf thành node cha
                // -> node cũ bị stale, phải tìm lại theo KeyFieldName rồi mới mở rộng được
                TreeListNode serviceNode = trvService.FindNodeByKeyID(service.CONCRETE_ID__IN_SETY);
                if (serviceNode != null)
                {
                    serviceNode.Expanded = true;
                    trvService.FocusedNode = serviceNode;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Nút "X" (cột riêng) trên dòng chiết khấu — xóa dòng (gọi API Delete nếu đã có ID).</summary>
        private void repoBtnDeleteDiscount_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind != ButtonPredefines.Delete)
                {
                    return;
                }
                DeleteFocusedDiscountRow();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Nút "X" trên ô Chiết khấu (SpinEdit, hiện khi sửa) — xóa dòng.</summary>
        private void repoSpinDiscountRow_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind != ButtonPredefines.Delete)
                {
                    return;
                }
                DeleteFocusedDiscountRow();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Xóa dòng chiết khấu đang focus — gọi API Delete nếu đã có ID, rồi cập nhật tree.</summary>
        private void DeleteFocusedDiscountRow()
        {
            try
            {
                TreeListNode node = trvService.FocusedNode;
                if (node == null)
                {
                    return;
                }
                var row = trvService.GetDataRecordByNode(node) as SereServADO;
                if (row == null || row.IsDiscountRow != true)
                {
                    return;
                }

                if (XtraMessageBox.Show(
                        ResourceMessage.BanCoMuonXoaDongChietKhauKhong,
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                TreeListNode parentNode = node.ParentNode;

                // Đã lưu (có ID) -> gọi API xóa ngay
                if (row.DISCOUNT_ID.HasValue && row.DISCOUNT_ID.Value > 0)
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    bool ok = new BackendAdapter(param).Post<bool>(
                        RequestUriStore.HIS_SERE_SERV_DISCOUNT_DELETE,
                        ApiConsumers.MosConsumer, row.DISCOUNT_ID.Value, param);
                    WaitingManager.Hide();
                    if (!ok)
                    {
                        MessageManager.Show(this, param, false);
                        return;
                    }
                }

                records.Remove(row);
                trvService.RefreshDataSource();

                if (parentNode != null)
                {
                    RecomputeServiceDiscountSum(parentNode);
                    trvService.RefreshNode(parentNode);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
