/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */

namespace HIS.Desktop.Plugins.Library.KhoDuLieuYTe.ADO
{
    /// <summary>Kết quả gửi 1 giấy (1 dòng trên popup kết quả).</summary>
    public class KdlResultADO
    {
        public const int STATUS__SUCCESS = 1;
        public const int STATUS__EXISTED = 2;
        public const int STATUS__FAILED = 3;
        public const int STATUS__SKIPPED = 4;

        public string TreatmentCode { get; set; }
        public string PatientName { get; set; }
        public string CertCode { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }

        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case STATUS__SUCCESS:
                        return "Thành công";
                    case STATUS__EXISTED:
                        return "Đã có trên kho";
                    case STATUS__FAILED:
                        return "Lỗi";
                    default:
                        return "Bỏ qua";
                }
            }
        }
    }
}
