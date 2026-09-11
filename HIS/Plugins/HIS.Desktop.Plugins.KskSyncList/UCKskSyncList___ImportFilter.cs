/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Nút "NK điều kiện lọc" — nhập khẩu danh sách MÃ ĐIỀU TRỊ từ tệp Excel rồi lọc lưới theo danh
 * sách đó. Làm theo nút cùng tên ở màn Xuất XML QĐ 130, nhưng CHỈ lọc theo mã điều trị.
 *
 * TỆP EXCEL: một cột duy nhất — cột A là mã điều trị, mỗi dòng một mã. Dòng tiêu đề để cũng được:
 * mã điều trị toàn chữ số nên dòng chữ bị loại ngay khi lọc, có ghi số dòng bỏ qua vào nhật ký.
 *
 * VÌ SAO GỌI NHIỀU LƯỢT THAY VÌ MỘT LƯỢT: bộ lọc của API `api/HisKskSync/GetView` chỉ có
 * TREATMENT_CODE__EXACT — một mã cho mỗi lượt gọi, KHÔNG có trường nhận danh sách. Muốn gọi một
 * lượt thì phải bổ sung trường vào MOS.Filter rồi sửa cả tầng dữ liệu và phát hành lại máy chủ —
 * việc đó nặng hơn nhiều so với thứ đang cần, nên ở đây gọi tuần tự từng mã rồi gộp kết quả.
 * Danh sách vài trăm mã vẫn chạy được, nhưng lâu; xem NGƯỠNG_HOI_LAI bên dưới.
 *
 * KẾT QUẢ đổ thẳng vào lưới thành MỘT TRANG, giống hệt cách màn Xuất XML QĐ 130 làm — người dùng
 * đang muốn xem đúng danh sách vừa nhập, không phải phân trang lại theo bộ lọc cũ.
 */
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.KskSyncList.ADO;
using HIS.Desktop.Utility;
using HIS.UC.SettingSignInfo;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Windows.Forms;
using System;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Bộ lọc gửi lên máy chủ cho riêng việc lọc theo DANH SÁCH mã điều trị.
    ///
    /// VÌ SAO KHÔNG DÙNG THẲNG HisKskSyncViewFilter: plugin biên dịch với tệp MOS.Filter.dll DÙNG
    /// CHUNG nằm ở FRONTEND\lib, mà tệp đó chưa có trường TREATMENT_CODES. Muốn dùng thẳng thì phải
    /// dựng lại và ghi đè tệp dùng chung ấy — nó là tệp của toàn bộ 992 plugin, đổi một tệp để thêm
    /// một trường cho một màn hình là đánh đổi quá lớn.
    ///
    /// Bộ lọc đi lên máy chủ dưới dạng JSON nên chỉ cần TÊN TRƯỜNG khớp; máy chủ đọc vào
    /// HisKskSyncViewFilterQuery của nó. Các trường không khai ở đây thì máy chủ hiểu là bỏ trống.
    ///
    /// KHI NÀO BỎ LỚP NÀY: sau khi bản phát hành làm mới MOS.Filter.dll ở FRONTEND\lib, đổi lại
    /// dùng HisKskSyncViewFilter cho thống nhất rồi xoá lớp này đi.
    /// </summary>
    internal class KskSyncFilterByCodes
    {
        /// <summary>Danh sách mã điều trị — máy chủ dịch thành mệnh đề IN.</summary>
        public List<string> TREATMENT_CODES { get; set; }

        public string ORDER_FIELD { get; set; }
        public string ORDER_DIRECTION { get; set; }
    }

    public partial class UCKskSyncList
    {
        /// <summary>Mã điều trị của HIS dài 12 chữ số; tệp thường ghi thiếu số 0 ở đầu.</summary>
        private const int IMPORT_FILTER__CODE_LEN = 12;

        /// <summary>
        /// Số mã gửi trong MỘT lượt gọi.
        ///
        /// Mệnh đề IN của Oracle chỉ nhận tối đa 1000 phần tử; lấy 500 cho còn biên an toàn và để
        /// câu truy vấn khỏi quá dài. Danh sách lớn hơn thì chia thành nhiều lượt.
        /// </summary>
        private const int IMPORT_FILTER__BATCH = 500;

        private void btnImportFilter_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> codes = ReadTreatmentCodesFromExcel();
                if (codes == null) return;            // người dùng bỏ, hoặc tệp không đọc được

                if (codes.Count == 0)
                {
                    XtraMessageBoxShow("Tệp không có mã điều trị nào hợp lệ."
                        + "\r\n\r\nTệp cần MỘT cột: cột A ghi mã điều trị, mỗi dòng một mã.");
                    return;
                }

                List<V_HIS_KSK_SYNC> rows = LoadKskByTreatmentCodes(codes);
                if (rows == null) return;          // máy chủ chưa hỗ trợ — đã báo người dùng rồi

                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "KskSync/NK: nhap {0} ma dieu tri -> tim thay {1} ho so", codes.Count, rows.Count));

                BindImportedRows(rows);

                if (rows.Count < codes.Count)
                    XtraMessageBoxShow(string.Format(
                        "Đã nhập {0} mã điều trị, tìm thấy {1} hồ sơ khám sức khỏe."
                        + "\r\n\r\n{2} mã không có hồ sơ khám sức khỏe tương ứng.",
                        codes.Count, rows.Count, codes.Count - rows.Count));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Đọc cột A của tệp Excel thành danh sách mã điều trị đã chuẩn hoá.
        /// Trả null khi người dùng bỏ hoặc tệp không đọc được.
        /// </summary>
        private List<string> ReadTreatmentCodesFromExcel()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Excel (*.xlsx;*.xls)|*.xlsx;*.xls|Tất cả (*.*)|*.*";
            if (ofd.ShowDialog() != DialogResult.OK) return null;

            try
            {
                WaitingManager.Show();

                var import = new Inventec.Common.ExcelImport.Import();
                if (!import.ReadFileExcel(ofd.FileName))
                {
                    WaitingManager.Hide();
                    XtraMessageBoxShow("Không đọc được tệp Excel này.");
                    return null;
                }

                List<KskTreatmentCodeImportADO> raw = import.GetWithCheck<KskTreatmentCodeImportADO>(0);
                WaitingManager.Hide();
                if (raw == null) return new List<string>();

                int boQua = 0;
                List<string> codes = new List<string>();
                foreach (KskTreatmentCodeImportADO item in raw)
                {
                    string code = NormalizeTreatmentCode(item != null ? item.TREATMENT_CODE : null);
                    if (code == null) { boQua++; continue; }
                    if (!codes.Contains(code)) codes.Add(code);      // trùng mã thì chỉ hỏi một lần
                }

                if (boQua > 0)
                    Inventec.Common.Logging.LogSystem.Info(
                        "KskSync/NK: bo qua " + boQua + " dong khong phai ma dieu tri (dong tieu de, o trong...)");

                return codes;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBoxShow("Không đọc được tệp Excel này.");
                return null;
            }
        }

        /// <summary>
        /// Mã điều trị hợp lệ -> chuỗi 12 chữ số. Không phải chữ số (dòng tiêu đề, ô trống, ghi chú)
        /// -> null để bên gọi bỏ qua.
        ///
        /// Đệm số 0 ở đầu giống hệt ô nhập mã điều trị trên màn hình: người dùng hay chép mã từ Excel
        /// nên số 0 đứng đầu bị Excel cắt mất.
        /// </summary>
        private static string NormalizeTreatmentCode(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            string code = raw.Trim();
            foreach (char c in code)
                if (!char.IsDigit(c)) return null;

            if (code.Length < IMPORT_FILTER__CODE_LEN)
            {
                long v;
                if (!long.TryParse(code, out v)) return null;
                code = v.ToString(new string('0', IMPORT_FILTER__CODE_LEN));
            }
            return code;
        }

        /// <summary>
        /// Hỏi máy chủ theo LÔ rồi gộp lại, bỏ hồ sơ trùng.
        ///
        /// Máy chủ nhận trường TREATMENT_CODES — cả lô mã trong một lượt gọi. Máy chủ CHƯA CẬP NHẬT
        /// thì nó bỏ qua trường lạ này và trả về cả bảng; xem CheckServerFiltered.
        /// </summary>
        private List<V_HIS_KSK_SYNC> LoadKskByTreatmentCodes(List<string> codes)
        {
            List<V_HIS_KSK_SYNC> rows = new List<V_HIS_KSK_SYNC>();
            try
            {
                WaitingManager.Show();

                for (int i = 0; i < codes.Count; i += IMPORT_FILTER__BATCH)
                {
                    List<string> lo = codes.Skip(i).Take(IMPORT_FILTER__BATCH).ToList();
                    try
                    {
                        CommonParam param = new CommonParam();
                        KskSyncFilterByCodes filter = new KskSyncFilterByCodes();
                        filter.TREATMENT_CODES = lo;
                        filter.ORDER_FIELD = "CONCLUSION_TIME";
                        filter.ORDER_DIRECTION = "DESC";

                        List<V_HIS_KSK_SYNC> rs = new BackendAdapter(param)
                            .GetRO<List<V_HIS_KSK_SYNC>>("api/HisKskSync/GetView",
                                ApiConsumers.MosConsumer, filter, param).Data;
                        if (rs == null || rs.Count == 0) continue;

                        if (!CheckServerFiltered(rs, lo)) return null;      // máy chủ chưa cập nhật
                        rows.AddRange(rs);
                    }
                    catch (Exception exLo)
                    {
                        // Một lô lỗi thì bỏ lô đó, KHÔNG dừng cả mẻ — người dùng vẫn xem được phần còn lại.
                        Inventec.Common.Logging.LogSystem.Warn(
                            "KskSync/NK: khong lay duoc ho so cua lo bat dau tu ma " + lo[0]);
                        Inventec.Common.Logging.LogSystem.Warn(exLo);
                    }
                }
            }
            finally { WaitingManager.Hide(); }

            return rows.GroupBy(o => o.ID).Select(g => g.First()).ToList();
        }

        /// <summary>
        /// Máy chủ CÓ thật sự lọc theo danh sách mã hay không.
        ///
        /// VÌ SAO PHẢI KIỂM: bộ lọc đi qua đường JSON. Máy chủ chưa cập nhật thư viện bộ lọc thì nó
        /// LẶNG LẼ BỎ QUA trường TREATMENT_CODES, không báo lỗi gì, và trả về cả bảng đã phân trang.
        /// Người dùng sẽ thấy một danh sách trông rất bình thường nhưng KHÔNG PHẢI thứ họ vừa nhập —
        /// tệ hơn nhiều so với một câu báo lỗi.
        ///
        /// Cách kiểm rẻ nhất: hồ sơ trả về mà có mã điều trị NGOÀI lô vừa hỏi thì chắc chắn máy chủ
        /// đã bỏ qua điều kiện lọc.
        /// </summary>
        private bool CheckServerFiltered(List<V_HIS_KSK_SYNC> rows, List<string> lo)
        {
            foreach (V_HIS_KSK_SYNC r in rows)
            {
                if (r == null) continue;
                if (lo.Contains(r.TDL_TREATMENT_CODE)) continue;

                Inventec.Common.Logging.LogSystem.Warn(
                    "KskSync/NK: may chu tra ve ho so NGOAI danh sach ma vua hoi -> may chu chua co"
                    + " truong loc TREATMENT_CODES. Can cap nhat MOS.Filter va MOS.MANAGER cho may chu.");

                XtraMessageBoxShow(
                    "Máy chủ chưa hỗ trợ lọc theo danh sách mã điều trị nên đã bỏ qua điều kiện lọc."
                    + "\r\n\r\nVui lòng cập nhật máy chủ (MOS) lên bản có tính năng này rồi thử lại."
                    + "\r\n\r\nDanh sách trên màn hình giữ nguyên, KHÔNG hiển thị kết quả sai.");
                return false;
            }
            return true;
        }

        /// <summary>Đổ kết quả nhập khẩu vào lưới thành một trang.</summary>
        private void BindImportedRows(List<V_HIS_KSK_SYNC> rows)
        {
            try
            {
                gridView1.BeginUpdate();

                if (ucPaging != null && ucPaging.pagingGrid != null)
                {
                    ucPaging.pagingGrid.CurrentPage = 1;
                    ucPaging.pagingGrid.PageCount = 1;
                    ucPaging.pagingGrid.MaxRec = rows.Count;
                    ucPaging.pagingGrid.DataCount = rows.Count;
                }

                gridControl1.DataSource = (rows.Count > 0) ? rows : null;
                rowCount = rows.Count;
                dataTotal = rows.Count;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
            finally
            {
                try { gridView1.EndUpdate(); }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            }
        }

        private static void XtraMessageBoxShow(string message)
        {
            XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
