/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;

namespace HIS.Desktop.Plugins.VlgPortalLookup.ADO
{
    /// <summary>
    /// 1 dong luoi tra cuu ho so tren Cong tiep nhan Vinh Long — dung chung cho 3 nhom:
    /// KSK QD2062 (FromPortalItem), KCB (FromKcbItem), HSSK 831 (FromHssk831Item),
    /// va dong tong hop khi Doi soat voi HIS (chi che do KSK).
    /// Cac truong His*/DoiSoat* chi co gia tri sau khi chay Doi soat.
    /// </summary>
    public class VlgHoSoADO
    {
        public string MaLk { get; set; }             // KSK/KCB: ma lien ket = ma dieu tri; HSSK: ma dinh danh
        public string HoTen { get; set; }
        public string SoCccd { get; set; }
        public string MaBn { get; set; }             // KCB: ma benh nhan
        public string NgayKhamText { get; set; }     // KSK: ngay kham; KCB: ngay vao; HSSK: ngay nhan
        public string FormName { get; set; }         // KSK: ten mau phieu cong phan loai
        public string ValidationStatus { get; set; } // KSK: VALID/INVALID; KCB: status ho so; HSSK: status request
        public string LatestStatus { get; set; }     // KSK: trang thai lan gui cuoi; KCB: "DA HUY" khi is_cancelled
        public string LatestReceivedText { get; set; }
        public string TrackingId { get; set; }

        // Doi soat voi HIS (null khi chi tra cuu thuong)
        public string HisStatusText { get; set; }    // trang thai tren HIS: Da dong bo / That bai / Chua dong bo
        public string DoiSoatText { get; set; }      // ket qua doi soat (co "⚠" = lech)
        public bool IsMismatch { get; set; }

        // Chi tiet render san cho memo (chi co khi goi trang-thai)
        public string DetailText { get; set; }

        /// <summary>Dung tu item JSON cua API danh sach ho so.</summary>
        internal static VlgHoSoADO FromPortalItem(Newtonsoft.Json.Linq.JToken it)
        {
            var ado = new VlgHoSoADO();
            try
            {
                ado.MaLk = (string)it["ma_lk"];
                ado.HoTen = (string)it["ho_ten"];
                ado.SoCccd = (string)it["so_cccd"];
                ado.NgayKhamText = IsoToDateText((string)it["ngay_kham"]);
                ado.FormName = (string)it["form_name"];
                ado.ValidationStatus = (string)it["validation_status"];
                var latest = it["latest_request"];
                if (latest != null && latest.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                {
                    ado.TrackingId = (string)latest["tracking_id"];
                    ado.LatestStatus = (string)latest["status"];
                    ado.LatestReceivedText = IsoToTimeText((string)latest["received_at"]);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return ado;
        }

        /// <summary>Dung tu item JSON cua GET /api/kham-chua-benh/ho-so (nhom KCB).</summary>
        internal static VlgHoSoADO FromKcbItem(Newtonsoft.Json.Linq.JToken it)
        {
            var ado = new VlgHoSoADO();
            try
            {
                ado.MaLk = (string)it["ma_lk"];
                ado.HoTen = (string)it["ho_ten"];
                ado.MaBn = (string)it["ma_bn"];
                ado.NgayKhamText = IsoToTimeText((string)it["ngay_vao"]);
                ado.ValidationStatus = (string)it["status"];
                bool cancelled = false;
                try { cancelled = it["is_cancelled"] != null && (bool?)it["is_cancelled"] == true; }
                catch { }
                ado.LatestStatus = cancelled ? "ĐÃ HỦY" : "";
                ado.TrackingId = (string)it["latest_tracking_id"];
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return ado;
        }

        /// <summary>Dung tu item JSON cua GET /api/ho-so-suc-khoe/qd-831-2017/ho-so (nhom HSSK 831).</summary>
        internal static VlgHoSoADO FromHssk831Item(Newtonsoft.Json.Linq.JToken it)
        {
            var ado = new VlgHoSoADO();
            try
            {
                ado.MaLk = (string)it["ma_dinh_danh"];
                ado.HoTen = (string)it["ho_ten"];
                ado.ValidationStatus = (string)it["status"];
                ado.LatestReceivedText = IsoToTimeText((string)it["received_at"]);
                ado.TrackingId = (string)it["tracking_id"];
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return ado;
        }

        /// <summary>"2026-08-12T08:32:00.895818" -> "12/08/2026". Khong parse duoc -> giu nguyen.</summary>
        internal static string IsoToDateText(string iso)
        {
            try
            {
                if (string.IsNullOrEmpty(iso)) return "";
                DateTime dt;
                if (DateTime.TryParse(iso, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out dt))
                    return dt.ToString("dd/MM/yyyy");
                return iso;
            }
            catch { return iso; }
        }

        /// <summary>ISO -> "12/08/2026 08:32". Khong parse duoc -> giu nguyen.</summary>
        internal static string IsoToTimeText(string iso)
        {
            try
            {
                if (string.IsNullOrEmpty(iso)) return "";
                DateTime dt;
                if (DateTime.TryParse(iso, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out dt))
                    return dt.ToString("dd/MM/yyyy HH:mm");
                return iso;
            }
            catch { return iso; }
        }
    }
}
