/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tồn kho nhập xuất tồn - phần bổ sung: bộ lọc Từ ngày/Đến ngày,
 * gọi API GetWithImpExp và dựng dictionary cho 3 cột Tổng nhập/Tổng xuất/Tồn cuối kỳ.
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.MediStockSummaryWithImpExp.ADO;
using HIS.UC.HisBloodTypeInStock;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockSummaryWithImpExp
{
    public partial class UCMediStockSummaryWithImpExp
    {
        #region ---ImpExp fields
        // Ô lọc Từ ngày / Đến ngày được tạo bằng code (thêm vào layoutControl3).
        internal DevExpress.XtraEditors.DateEdit dtFromDate;
        internal DevExpress.XtraEditors.DateEdit dtToDate;

        // Tổng nhập/xuất/tồn cuối kỳ tra theo MEDICINE_TYPE_ID / MATERIAL_TYPE_ID.
        internal Dictionary<long, MediStockImpExpADO> dicMediImpExp = new Dictionary<long, MediStockImpExpADO>();
        internal Dictionary<long, MediStockImpExpADO> dicMateImpExp = new Dictionary<long, MediStockImpExpADO>();

        // True sau khi form Load xong: dùng để chặn tự tìm lúc mới mở, nhưng cho tự nạp khi đổi radio loại.
        internal bool formLoaded = false;
        #endregion

        /// <summary>
        /// Khởi tạo 2 ô Từ ngày/Đến ngày và đặt giá trị mặc định.
        /// Gọi sau InitializeComponent (trong Load).
        /// </summary>
        private void InitImpExpFilter()
        {
            try
            {
                dtFromDate = new DevExpress.XtraEditors.DateEdit();
                dtFromDate.Name = "dtFromDate";
                dtFromDate.Properties.Mask.EditMask = "dd/MM/yyyy";
                dtFromDate.Properties.Mask.UseMaskAsDisplayFormat = true;
                dtFromDate.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Vista;
                dtFromDate.Width = 92;

                dtToDate = new DevExpress.XtraEditors.DateEdit();
                dtToDate.Name = "dtToDate";
                dtToDate.Properties.Mask.EditMask = "dd/MM/yyyy";
                dtToDate.Properties.Mask.UseMaskAsDisplayFormat = true;
                dtToDate.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Vista;
                dtToDate.Width = 92;

                if (this.layoutControl3 != null)
                {
                    this.layoutControl3.AddItem("Từ ngày", dtFromDate);
                    this.layoutControl3.AddItem("Đến ngày", dtToDate);
                }

                ResetImpExpFilterDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Từ ngày = ngày 01 tháng hiện tại; Đến ngày = ngày hiện tại.
        /// </summary>
        private void ResetImpExpFilterDefault()
        {
            try
            {
                var now = DateTime.Now;
                var firstDay = new DateTime(now.Year, now.Month, 1);
                if (dtFromDate != null) dtFromDate.EditValue = firstDay;
                if (dtToDate != null) dtToDate.EditValue = now.Date;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lấy khoảng thời gian (yyyyMMddHHmmss). Trả về false nếu Từ ngày &gt; Đến ngày.
        /// fromTime/toTime = null nếu ô tương ứng để trống.
        /// </summary>
        private bool TryGetImpExpRange(out long? fromTime, out long? toTime)
        {
            fromTime = null;
            toTime = null;
            DateTime? from = null;
            DateTime? to = null;
            if (dtFromDate != null && dtFromDate.EditValue != null) from = dtFromDate.DateTime.Date;
            if (dtToDate != null && dtToDate.EditValue != null) to = dtToDate.DateTime.Date;

            if (from.HasValue && to.HasValue && from.Value > to.Value)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Từ ngày phải nhỏ hơn hoặc bằng đến ngày", "Thông báo");
                return false;
            }

            if (from.HasValue)
                fromTime = Inventec.Common.TypeConvert.Parse.ToInt64(from.Value.ToString("yyyyMMdd") + "000000");
            if (to.HasValue)
                toTime = Inventec.Common.TypeConvert.Parse.ToInt64(to.Value.ToString("yyyyMMdd") + "235959");
            return true;
        }

        /// <summary>
        /// Gọi API GetWithImpExp và dựng dictionary tổng nhập/xuất/tồn cuối kỳ.
        /// Không nhập khoảng thời gian =&gt; bỏ qua (Tổng nhập/xuất = 0, Tồn cuối kỳ = tồn hiện tại, xử lý ở callback).
        /// </summary>
        private void BuildImpExpDictionary(bool isMedicine, long? fromTime, long? toTime)
        {
            try
            {
                if (isMedicine) dicMediImpExp = new Dictionary<long, MediStockImpExpADO>();
                else dicMateImpExp = new Dictionary<long, MediStockImpExpADO>();

                if (!fromTime.HasValue && !toTime.HasValue)
                    return;

                if (this.mediStockIds == null || this.mediStockIds.Count == 0)
                    return;

                CommonParam param = new CommonParam();
                MediStockImpExpFilter filter = new MediStockImpExpFilter();
                filter.MEDI_STOCK_IDs = this.mediStockIds;
                filter.FROM_TIME = fromTime;
                filter.TO_TIME = toTime;

                string uri = isMedicine
                    ? "api/HisMediStockMety/GetWithImpExp"
                    : "api/HisMediStockMaty/GetWithImpExp";

                Inventec.Common.Logging.LogSystem.Info("[ImpExp] CALL uri=" + uri
                    + Inventec.Common.Logging.LogUtil.TraceData("filter", filter));

                var result = new BackendAdapter(param).Post<List<MediStockImpExpADO>>(uri, ApiConsumers.MosConsumer, filter, param);

                Inventec.Common.Logging.LogSystem.Info("[ImpExp] RESULT uri=" + uri
                    + " count=" + (result == null ? "null" : result.Count.ToString())
                    + Inventec.Common.Logging.LogUtil.TraceData("param", param));

                if (result == null) return;

                var dic = isMedicine ? dicMediImpExp : dicMateImpExp;
                foreach (var item in result)
                {
                    if (item == null) continue;
                    long key = isMedicine ? (item.MEDICINE_TYPE_ID ?? 0) : (item.MATERIAL_TYPE_ID ?? 0);
                    if (key == 0) continue;
                    if (dic.ContainsKey(key))
                    {
                        dic[key].TOTAL_IMP_QUANTITY = (dic[key].TOTAL_IMP_QUANTITY ?? 0) + (item.TOTAL_IMP_QUANTITY ?? 0);
                        dic[key].TOTAL_EXP_QUANTITY = (dic[key].TOTAL_EXP_QUANTITY ?? 0) + (item.TOTAL_EXP_QUANTITY ?? 0);
                        dic[key].CLOSE_QUANTITY = (dic[key].CLOSE_QUANTITY ?? 0) + (item.CLOSE_QUANTITY ?? 0);
                    }
                    else
                    {
                        dic[key] = item;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region ---Giữ text tìm trên cây kết quả khi bấm Tìm (Reload làm mất FindFilterText)
        /// <summary>Tìm TreeList lồng bên trong UserControl tồn kho (đệ quy).</summary>
        private DevExpress.XtraTreeList.TreeList FindTreeList(Control c)
        {
            if (c == null) return null;
            DevExpress.XtraTreeList.TreeList tl = c as DevExpress.XtraTreeList.TreeList;
            if (tl != null) return tl;
            foreach (Control child in c.Controls)
            {
                var r = FindTreeList(child);
                if (r != null) return r;
            }
            return null;
        }

        private string GetTreeFindText(Control uc)
        {
            try
            {
                var tl = FindTreeList(uc);
                return tl != null ? tl.FindFilterText : null;
            }
            catch { return null; }
        }

        private void SetTreeFindText(Control uc, string text)
        {
            try
            {
                var tl = FindTreeList(uc);
                if (tl != null) tl.ApplyFindFilter(text);
            }
            catch { }
        }

        /// <summary>Lấy text tìm của cây đang hiển thị (theo loại đang chọn).</summary>
        private string GetActiveTreeFindText()
        {
            try
            {
                if (chkMedicine.Checked) return GetTreeFindText(ucMedicineInfo);
                if (chkMaterial.Checked) return GetTreeFindText(ucMaterialInfo);
            }
            catch { }
            return null;
        }

        /// <summary>Gán lại text tìm cho cây đang hiển thị sau khi Reload.</summary>
        private void RestoreActiveTreeFindText(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) return;
                if (chkMedicine.Checked) SetTreeFindText(ucMedicineInfo, text);
                else if (chkMaterial.Checked) SetTreeFindText(ucMaterialInfo, text);
            }
            catch { }
        }
        #endregion

        /// <summary>
        /// Xóa kết quả trên panel kết quả, chuyển panel theo loại đang chọn và để trống (chờ nhấn Tìm).
        /// </summary>
        private void ClearResultPanel()
        {
            try
            {
                if (dicMediImpExp != null) dicMediImpExp.Clear();
                if (dicMateImpExp != null) dicMateImpExp.Clear();

                this.panelControlMediMate.Controls.Clear();
                if (chkMedicine.Checked && this.ucMedicineInfo != null)
                {
                    this.panelControlMediMate.Controls.Add(this.ucMedicineInfo);
                    this.ucMedicineInfo.Dock = DockStyle.Fill;
                    hisMediInStockProcessor.Reload(ucMedicineInfo, null, null);
                }
                else if (chkMaterial.Checked && this.ucMaterialInfo != null)
                {
                    this.panelControlMediMate.Controls.Add(this.ucMaterialInfo);
                    this.ucMaterialInfo.Dock = DockStyle.Fill;
                    hisMateInStockProcessor.Reload(ucMaterialInfo, null, null);
                }
                else if (chkBlood.Checked && this.ucBloodInfo != null)
                {
                    this.panelControlMediMate.Controls.Add(this.ucBloodInfo);
                    this.ucBloodInfo.Dock = DockStyle.Fill;
                    hisBloodProcessor.Reload(ucBloodInfo, new List<HisBloodInStockSDO>());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
