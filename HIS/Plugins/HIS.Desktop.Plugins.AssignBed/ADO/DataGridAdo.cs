using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignBed.ADO
{
    public class DataGridAdo : SereServADO
    {
        public DateTime? TIME_FROM { get; set; }
        public DateTime? TIME_TO { get; set; }
        public decimal? QUANTITY { get; set; }
        public long? TDL_EXECUTE_ROOM_ID_STR { get; set; } = null;
        public long? PATIENT_TYPE_ID_STR { get; set; } = null;
        public string BED_CODE { get; set; }

        public string TIME_FROM_STR => TIME_FROM?.ToString("dd/MM/yyyy HH:mm");
        public string TIME_TO_STR => TIME_TO?.ToString("dd/MM/yyyy HH:mm");
        public string QUANTITY_STR => QUANTITY?.ToString();
        public string SHARE_COUNT_STR => ShareCount?.ToString();

        // Field tìm kiếm cho Auto Filter Row: bỏ dấu + viết HOA (ASCII) để lọc không phân biệt
        // hoa/thường VÀ không phân biệt dấu. Dữ liệu lẫn từ khóa nhập đều quy về ASCII nên DevExpress
        // so khớp ổn định (Contains/Upper của DevExpress xử lý ký tự tiếng Việt/Unicode không ổn định).
        public string TDL_SERVICE_CODE_SEARCH => NormalizeSearch(TDL_SERVICE_CODE);
        public string TDL_HEIN_SERVICE_BHYT_CODE_SEARCH => NormalizeSearch(TDL_HEIN_SERVICE_BHYT_CODE);
        public string TDL_SERVICE_NAME_SEARCH => NormalizeSearch(TDL_SERVICE_NAME);
        public string BED_CODE_SEARCH => NormalizeSearch(BED_CODE);
        public string OTHER_PAY_SOURCE_NAME_SEARCH => NormalizeSearch(OTHER_PAY_SOURCE_NAME);
        public string PATIENT_TYPE_NAME_SEARCH => NormalizeSearch(PATIENT_TYPE_NAME);

        // Bỏ dấu tiếng Việt + viết HOA. Dùng chung cho field dữ liệu và từ khóa người dùng nhập.
        public static string NormalizeSearch(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            try
            {
                string formD = input.Normalize(NormalizationForm.FormD);
                var sb = new StringBuilder(formD.Length);
                foreach (char ch in formD)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                        sb.Append(ch);
                }
                return sb.ToString()
                    .Replace('đ', 'd').Replace('Đ', 'D')
                    .ToUpperInvariant();
            }
            catch
            {
                return input.ToUpperInvariant();
            }
        }

        public new DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeTimeTo { get; set; }
        public new string ErrorMessageTimeTo { get; set; }
    }
}
