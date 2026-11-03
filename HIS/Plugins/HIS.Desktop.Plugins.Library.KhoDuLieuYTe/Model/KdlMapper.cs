/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using HIS.Bhyt.Hssk;
using MOS.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.Library.KhoDuLieuYTe.Model
{
    /// <summary>
    /// Chuyển dữ liệu đã dựng sẵn cho cổng BHXH (HIS.Bhyt.Hssk.GenXmlGCSData / GenXmlGBTData) sang
    /// bản ghi JSON của Kho dữ liệu SYT. Không dùng lại nguyên các trường lệch nghĩa giữa 2 đặc tả:
    /// CAP_LAN_DAU (BHXH) ngược nghĩa với LAN_CAP, NOI_SINH_CON có thêm phần huyện, TINH_TRANG_TV thang 0/1.
    /// </summary>
    internal static class KdlMapper
    {
        internal static KdlGcs01Data ToGcs01(GiayChungSinh gcs, V_HIS_BABY baby)
        {
            KdlGcs01Data data = new KdlGcs01Data();
            data.TEN_CSKBCB = N(gcs.TEN_CSKBCB);
            data.MA_CSKBCB = N(gcs.MA_CSKBCB);
            data.MA_GCS = N(gcs.MA_GCS);
            // Kho: "0" cấp lần đầu, "1" cấp lại
            data.LAN_CAP = baby.IS_REISSUED == 1 ? "1" : "0";
            data.HOTEN_ME = N(gcs.HOTEN_NND);
            data.NGAYSINH_ME = N(gcs.NGAYSINH_NND);
            data.MA_DANTOC_ME = N(gcs.MA_DANTOC_NND);
            data.LOAI_GIAYTO_ME = ToLong(gcs.LOAI_GIAYTO_NND);
            data.SO_DDCN_ME = N(gcs.SO_CCCD_NND);
            data.NOI_CU_TRU_ME = N(gcs.NOI_CU_TRU_NND);
            data.MATINH_CU_TRU = N(gcs.MATINH_CU_TRU);
            data.MAXA_CU_TRU = N(gcs.MAXA_CU_TRU);
            data.MA_BHXH_ME = N(gcs.MA_BHXH_NND);
            data.MA_THE_ME = N(gcs.MA_THE_NND);
            data.NGAY_SINH_CON = N(gcs.NGAY_SINH_CON);
            data.NOI_SINH_CON = N(RemoveDistrictPart(gcs.NOI_SINH_CON));
            data.SOCON_LANNAY = ToLong(gcs.SO_CON);
            data.GIOI_TINH_CON = gcs.GIOI_TINH_CON;
            data.CAN_NANG_CON = N(gcs.CAN_NANG_CON);
            data.MA_THE_TAM = N(gcs.MA_THE_TAM);
            data.TEN_CON = N(gcs.TEN_CON);
            data.GHI_CHU = N(gcs.GHI_CHU);
            data.SINHCON_PHAUTHUAT = gcs.SINHCON_PHAUTHUAT;
            data.SINHCON_DUOI32TUAN = gcs.SINHCON_DUOI32TUAN;
            data.SINHCON_PHAUTHUAT_DUOI32TUAN = gcs.SINHCON_PHAUTHUAT_DUOI32TUAN;
            data.TINH_TRANG_CON = N(gcs.TINH_TRANG_CON);
            data.NGAY_CS = N(gcs.NGAY_CT);
            // Model BHXH không có thân nhân; theo ví dụ của đặc tả lấy họ tên mẹ
            data.THAN_NHAN = N(gcs.HOTEN_NND);
            data.NGUOI_DO_DE = N(gcs.NGUOI_DO_DE);
            data.THU_TRUONG_DVI = N(gcs.THU_TRUONG_DVI);
            return data;
        }

        internal static KdlGbtData ToGbt(GiayBaoTu gbt, V_HIS_TREATMENT treatment)
        {
            KdlGbtData data = new KdlGbtData();
            data.MA_GBT = N(gbt.MA_GBT);
            data.MA_BN = N(gbt.MA_BN);
            data.MA_HSBA = N(gbt.MA_HSBA);
            data.HO_TEN = N(gbt.HO_TEN);
            data.NGAY_SINH = N(gbt.NGAY_SINH);
            data.GIOI_TINH = gbt.GIOI_TINH;
            data.MA_THE = N(gbt.MA_THE);
            data.MA_DANTOC = N(gbt.MA_DANTOC);
            data.MA_QUOCTICH = N(gbt.MA_QUOCTICH);
            data.DCHI_THUONGTRU = N(gbt.DCHI_THUONGTRU);
            data.MATINH_THUONGTRU = N(gbt.MATINH_THUONGTRU);
            data.MAHUYEN_THUONGTRU = N(gbt.MAHUYEN_THUONGTRU);
            data.MAXA_THUONGTRU = N(gbt.MAXA_THUONGTRU);
            data.DCHI_HIENTAI = N(gbt.DCHI_HIENTAI);
            data.MATINH_HIENTAI = N(gbt.MATINH_HIENTAI);
            data.MAHUYEN_HIENTAI = N(gbt.MAHUYEN_HIENTAI);
            data.MAXA_HIENTAI = N(gbt.MAXA_HIENTAI);
            data.LOAI_GIAYTO = gbt.LOAI_GIAYTO;
            data.SO_GIAYTO = N(gbt.SO_GIAYTO);
            data.NGAY_CAP = N(gbt.NGAY_CAP);
            data.NOI_CAP = N(gbt.NOI_CAP);
            data.NGAYGIO_VV = N(gbt.NGAYGIO_VV);
            data.NGAY_TV = N(gbt.NGAY_TV);
            // Kho: 1 = tại cơ sở KBCB, 2 = trên đường đến cơ sở. Cổng BHXH dịch DEATH_STATUS 1/2 -> 0/1
            // nên DEATH_STATUS 1/2 khớp thẳng thang của kho; chưa nhập thì mặc định tại cơ sở.
            data.TINH_TRANG_TV = treatment.DEATH_STATUS == 2 ? 2 : 1;
            data.NGUYENNHAN_TV = N(gbt.NGUYENNHAN_TV);
            data.NGUOI_GHIGIAY = N(gbt.NGUOI_GHIGIAY);
            data.NGUOI_THANTHICH = N(gbt.NGUOI_THANTHICH);
            data.TTRUONG_DVI = N(gbt.TTRUONG_DVI);
            data.SO_BAOTU = N(gbt.SO_BAOTU);
            data.QUYEN_SO = N(gbt.QUYEN_SO);
            data.NGAY_CAPGIAYBT = N(gbt.NGAY_CAPGIAYBT);
            data.SO_BAOTU_BD = N(gbt.SO_BAOTU_BD);
            data.QUYEN_SO_BD = N(gbt.QUYEN_SO_BD);
            data.MACSKCB = N(gbt.MACSKCB);
            data.DIACHI_CSKCB = N(gbt.DIACHI_CSKCB);
            return data;
        }

        /// <summary>"tỉnh:huyện:xã:địa chỉ" (BHXH) -> "tỉnh:xã:địa chỉ" (kho).</summary>
        private static string RemoveDistrictPart(string noiSinh)
        {
            if (string.IsNullOrEmpty(noiSinh)) return noiSinh;
            string[] parts = noiSinh.Split(new char[] { ':' }, 4);
            if (parts.Length < 4) return noiSinh;
            return parts[0] + ":" + parts[2] + ":" + parts[3];
        }

        /// <summary>Kho coi "" là không có dữ liệu -> gửi null cho rõ.</summary>
        private static string N(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static long? ToLong(string value)
        {
            long result;
            if (!string.IsNullOrWhiteSpace(value) && long.TryParse(value.Trim(), out result))
                return result;
            return null;
        }
    }
}
