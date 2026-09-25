/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Ghi và đọc file Excel mẫu khám sức khỏe (việc 57621).
 *
 * Luồng: "Xuất mẫu" -> file Excel đã điền sẵn "Bình thường" / "Loại I" -> người dùng sửa ->
 * "Nhập mẫu" -> điền trở lại lên form.
 *
 * Cấu trúc file: 1 dòng tiêu đề + mỗi mục khám 1 dòng, 5 cột
 *      A Mục khám   B Kết quả   C Phân loại   D Mã ô   E Mã ô phân loại
 * Hai cột D và E là KHÓA ĐỐI CHIẾU khi nhập lại, được ẩn đi vì người dùng không cần nhìn.
 * KHÔNG khớp theo cột A: nhãn mục có thể đổi giữa các bản phát hành, tên control thì không.
 *
 * Dùng thẳng DevExpress.Spreadsheet (đã có sẵn trong bộ cài) thay vì thư viện
 * Inventec.Common.ExcelImport: thư viện đó đọc theo thẻ {%IMPORT%} và ánh xạ vào thuộc tính lớp
 * theo tên cột, hợp với file danh mục cố định; ở đây mỗi mẫu khám một bộ dòng khác nhau nên
 * đọc trực tiếp theo vị trí cột gọn và rõ hơn.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Spreadsheet;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>Ghi/đọc file Excel mẫu khám sức khỏe.</summary>
    public static class KskExcelFile
    {
        /// <summary>Ô A1 — nhãn nhận dạng file, chặn nhập nhầm file của chức năng khác.</summary>
        private const string FILE_TAG = "MAU_KSK";

        private const int COL_MUC = 0;
        private const int COL_KET_QUA = 1;
        private const int COL_PHAN_LOAI = 2;
        private const int COL_MA_O = 3;
        private const int COL_MA_O_PL = 4;

        /// <summary>Dòng 0 là nhãn nhận dạng, dòng 1 là tiêu đề, dữ liệu bắt đầu từ dòng 2.</summary>
        private const int ROW_TAG = 0;
        private const int ROW_HEADER = 1;
        private const int ROW_FIRST_DATA = 2;

        #region ===== Ghi file =====

        /// <summary>
        /// Ghi danh sách mục khám ra file Excel. Trả false nếu lỗi (đã ghi log).
        /// <paramref name="tabName"/> chỉ để hiện trên dòng nhãn cho người dùng biết file của mẫu khám nào.
        /// </summary>
        public static bool Write(string path, string tabName, List<KskExcelRowADO> rows)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || rows == null) return false;

                using (var wb = new Workbook())
                {
                    wb.BeginUpdate();
                    try
                    {
                        Worksheet ws = wb.Worksheets[0];
                        ws.Name = "MauKSK";

                        // Dòng nhãn: cột A giữ mã nhận dạng (đọc lại để kiểm), cột B ghi tên mẫu khám
                        // cho người dùng biết file thuộc mẫu nào.
                        ws.Cells[ROW_TAG, COL_MUC].SetValue(FILE_TAG);
                        ws.Cells[ROW_TAG, COL_KET_QUA].SetValue(tabName ?? "");

                        ws.Cells[ROW_HEADER, COL_MUC].SetValue("Mục khám");
                        ws.Cells[ROW_HEADER, COL_KET_QUA].SetValue("Kết quả");
                        ws.Cells[ROW_HEADER, COL_PHAN_LOAI].SetValue("Phân loại");
                        ws.Cells[ROW_HEADER, COL_MA_O].SetValue("Mã ô");
                        ws.Cells[ROW_HEADER, COL_MA_O_PL].SetValue("Mã ô phân loại");
                        ws.Range.FromLTRB(COL_MUC, ROW_HEADER, COL_MA_O_PL, ROW_HEADER).Font.Bold = true;

                        int r = ROW_FIRST_DATA;
                        foreach (var row in rows)
                        {
                            if (row == null) continue;
                            ws.Cells[r, COL_MUC].SetValue(row.MUC_KHAM ?? "");
                            ws.Cells[r, COL_KET_QUA].SetValue(row.KET_QUA ?? "");
                            ws.Cells[r, COL_PHAN_LOAI].SetValue(row.PHAN_LOAI ?? "");
                            ws.Cells[r, COL_MA_O].SetValue(row.MA_O ?? "");
                            ws.Cells[r, COL_MA_O_PL].SetValue(row.MA_O_PHAN_LOAI ?? "");
                            r++;
                        }

                        ws.Columns[COL_MUC].WidthInCharacters = 34;
                        ws.Columns[COL_KET_QUA].WidthInCharacters = 34;
                        ws.Columns[COL_PHAN_LOAI].WidthInCharacters = 16;
                        // Ẩn 2 cột khóa: người dùng chỉ sửa cột Kết quả và Phân loại. Ẩn chứ KHÔNG bỏ,
                        // vì thiếu 2 cột này thì không nhập lại được.
                        ws.Columns[COL_MA_O].Visible = false;
                        ws.Columns[COL_MA_O_PL].Visible = false;

                        ws.FreezeRows(ROW_HEADER);
                    }
                    finally { wb.EndUpdate(); }

                    wb.SaveDocument(path, DocumentFormat.Xlsx);
                }
                return true;
            }
            catch (Exception ex) { LogSystem.Error(ex); return false; }
        }

        #endregion

        #region ===== Đọc file =====

        /// <summary>
        /// Đọc file Excel đã sửa. Trả false kèm <paramref name="error"/> khi file sai cấu trúc.
        /// <paramref name="rows"/> luôn khác null.
        /// </summary>
        public static bool Read(string path, out List<KskExcelRowADO> rows, out string error)
        {
            rows = new List<KskExcelRowADO>();
            error = null;
            try
            {
                if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path))
                {
                    error = "Không tìm thấy file.";
                    return false;
                }

                using (var wb = new Workbook())
                {
                    try { wb.LoadDocument(path, DocumentFormat.Xlsx); }
                    catch (Exception exLoad)
                    {
                        LogSystem.Warn(exLoad);
                        error = "Không đọc được file. Vui lòng chọn file Excel đã xuất từ chức năng này.";
                        return false;
                    }

                    Worksheet ws = wb.Worksheets[0];
                    string tag = GetText(ws, ROW_TAG, COL_MUC);
                    if (tag != FILE_TAG)
                    {
                        error = "File không đúng mẫu của chức năng Thông tin khám sức khỏe.";
                        return false;
                    }

                    int last = ws.Rows.LastUsedIndex;
                    for (int r = ROW_FIRST_DATA; r <= last; r++)
                    {
                        string maO = GetText(ws, r, COL_MA_O);
                        string maOPl = GetText(ws, r, COL_MA_O_PL);
                        // Dòng không có khóa nào thì không đối chiếu được -> bỏ (VD người dùng tự thêm dòng).
                        if (string.IsNullOrEmpty(maO) && string.IsNullOrEmpty(maOPl)) continue;

                        rows.Add(new KskExcelRowADO()
                        {
                            MUC_KHAM = GetText(ws, r, COL_MUC),
                            KET_QUA = GetText(ws, r, COL_KET_QUA),
                            PHAN_LOAI = GetText(ws, r, COL_PHAN_LOAI),
                            MA_O = maO,
                            MA_O_PHAN_LOAI = maOPl
                        });
                    }
                }

                if (rows.Count == 0)
                {
                    error = "File không có dòng nào đọc được.";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                error = "Không đọc được file: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Lấy nội dung một ô dưới dạng chuỗi đã bỏ khoảng trắng thừa.
        /// Bản DevExpress 15.2 KHÔNG có CellValue.ToText() — phải đọc theo IsText/IsNumeric,
        /// giống cách thư viện Inventec.Common.ExcelImport đang làm.
        /// </summary>
        private static string GetText(Worksheet ws, int row, int col)
        {
            try
            {
                CellValue v = ws.Cells[row, col].Value;
                if (v == null || v.IsEmpty) return "";
                if (v.IsText) return (v.TextValue ?? "").Trim();
                if (v.IsNumeric) return v.NumericValue.ToString(System.Globalization.CultureInfo.InvariantCulture).Trim();
                return (v.ToString() ?? "").Trim();
            }
            catch (Exception ex) { LogSystem.Warn(ex); return ""; }
        }

        #endregion
    }
}
