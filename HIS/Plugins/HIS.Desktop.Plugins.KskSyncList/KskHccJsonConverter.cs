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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Chuyen ban tin JSON do thu vien His.Ksk.QD2062 xuat (che do json/base64) sang DUNG cau truc
    /// tai lieu "HUONG DAN TICH HOP API - Lien thong KSK -> HCC" v1.0, muc 3.3.
    ///
    /// Thu vien xuat ban "JSON hoa" cua XML — ten khoa IN HOA va NOIDUNGFILE khong co lop boc:
    ///   {"KHAMSUCKHOE":{"THONGTINDONVI":{"MACSKCB":".."},"THONGTINHOSO":{"NGAYLAP":"..","SOLUONGHOSO":"1",
    ///     "DANHSACHHOSO":{"HOSO":{"FILEHOSO":[{"LOAIHOSO":"XML1","NOIDUNGFILE":{"HO_TEN":"..",..}}]}}},
    ///     "CHUKYDONVI":{"CKS_NGUOI_KET_LUAN":"","CKS_BENH_VIEN":""}}}
    /// Tai lieu HCC yeu cau:
    ///   {"khamsuckhoe":{"thongtindonvi":{"macskcb":".."},"thongtinhoso":{"ngaylap":"..","soluonghoso":"1",
    ///     "danhsachhoso":{"hoso":{"filehoso":[{"loaihoso":"XML1","noidungfile":{"thong_tin_hanh_chinh":{..}}}]}}},
    ///     "chukydonvi":{"cks_nguoi_ket_luan":"","cks_benh_vien":""}}}
    ///
    /// Ba viec lop nay lam:
    ///   1. Boc NOIDUNGFILE theo TEN KHOI cua tung loai ho so (= XmlRoot cua XMLn, xem BLOCK_BY_LOAIHOSO).
    ///   2. XML11 (can lam sang): lam phang KHAM_CAN_LAM_SANG/DANH_SACH_CLS/CHI_TIET_CLS -> MANG.
    ///   3. Ha CHU THUONG toan bo TEN KHOA (gia tri KHONG doi — "XML1", so, ngay... giu nguyen).
    /// Loi / khong dung dinh dang -> tra lai chuoi GOC (khong lam mat du lieu, cong se bao loi de biet).
    /// </summary>
    internal static class KskHccJsonConverter
    {
        private const string NODE_ROOT = "KHAMSUCKHOE";
        private const string PATH_FILEHOSO = "THONGTINHOSO.DANHSACHHOSO.HOSO.FILEHOSO";
        private const string NODE_LOAIHOSO = "LOAIHOSO";
        private const string NODE_NOIDUNGFILE = "NOIDUNGFILE";
        private const string LOAIHOSO_XML11 = "XML11";
        private const string PATH_CLS_FULL = "KHAM_CAN_LAM_SANG.DANH_SACH_CLS.CHI_TIET_CLS";
        private const string PATH_CLS_SHORT = "DANH_SACH_CLS.CHI_TIET_CLS";
        private const string PATH_CLS_LEAF = "CHI_TIET_CLS";

        /// <summary>
        /// LOAIHOSO -> ten khoi trong noidungfile. Lay tu XmlRootAttribute cua tung ADO trong
        /// His.Ksk.QD2062.XML1..XML12 (khop chinh xac ten khoi chu thuong o tai lieu HCC muc 3.3).
        /// </summary>
        private static readonly Dictionary<string, string> BLOCK_BY_LOAIHOSO =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "XML1",  "THONG_TIN_HANH_CHINH" },
            { "XML2",  "THONG_TIN_CHUNG_VE_LAN_KHAM" },
            { "XML3",  "DANH_GIA_DAU_HIEU_SINH_TON" },
            { "XML4",  "DANH_GIA_DINH_DUONG" },
            { "XML5",  "DANH_GIA_PHAT_TRIEN_TINH_THAN_VAN_DONG" },
            { "XML6",  "DANH_GIA_TIEM_CHUNG" },
            { "XML7",  "KHAM_LAM_SANG" },
            { "XML8",  "KET_LUAN_VA_TU_VAN" },
            { "XML9",  "TIEN_SU_BENH_TAT" },
            { "XML10", "KHAM_THE_LUC" },
            { "XML11", "KHAM_CAN_LAM_SANG" },
            { "XML12", "KET_LUAN" }
        };

        /// <summary>Chuyen chuoi JSON cua thu vien -> JSON dung chuan HCC. Tra chuoi goc neu khong xu ly duoc.</summary>
        internal static string ToHccJson(string libraryJson)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(libraryJson)) return libraryJson;

                JObject root = JObject.Parse(libraryJson);
                JObject ksk = root[NODE_ROOT] as JObject;
                if (ksk == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("KskHccJsonConverter: khong thay node " + NODE_ROOT
                        + " -> giu nguyen JSON cua thu vien.");
                    return libraryJson;
                }

                JArray files = ksk.SelectToken(PATH_FILEHOSO) as JArray;
                if (files != null)
                {
                    foreach (JToken file in files)
                    {
                        JObject fileObj = file as JObject;
                        if (fileObj == null) continue;
                        string loaiHoSo = (fileObj[NODE_LOAIHOSO] != null) ? fileObj[NODE_LOAIHOSO].ToString() : null;
                        JToken content = fileObj[NODE_NOIDUNGFILE];
                        if (content == null) continue;
                        fileObj[NODE_NOIDUNGFILE] = WrapContent(loaiHoSo, content);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("KskHccJsonConverter: khong thay " + PATH_FILEHOSO
                        + " -> chi ha chu thuong ten khoa.");
                }

                return LowercaseKeys(root).ToString(Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return libraryJson;
            }
        }

        /// <summary>
        /// Boc noi dung 1 file ho so vao lop khoi tuong ung. XML11: lam phang thanh MANG chi tiet CLS.
        /// LOAIHOSO khong nam trong bang (thu vien bo sung XML moi) -> giu nguyen, khong boc.
        /// </summary>
        private static JToken WrapContent(string loaiHoSo, JToken content)
        {
            string block;
            if (!BLOCK_BY_LOAIHOSO.TryGetValue(loaiHoSo ?? "", out block))
            {
                Inventec.Common.Logging.LogSystem.Warn("KskHccJsonConverter: LOAIHOSO la '"
                    + (loaiHoSo ?? "(null)") + "' -> giu nguyen noidungfile.");
                return content;
            }

            if (string.Equals(loaiHoSo, LOAIHOSO_XML11, StringComparison.OrdinalIgnoreCase))
            {
                // Thu vien: NOIDUNGFILE.KHAM_CAN_LAM_SANG.DANH_SACH_CLS.CHI_TIET_CLS = [ ... ]
                // Tai lieu HCC: noidungfile.kham_can_lam_sang = [ ... ]
                JToken cls = content.SelectToken(PATH_CLS_FULL)
                          ?? content.SelectToken(PATH_CLS_SHORT)
                          ?? content.SelectToken(PATH_CLS_LEAF);
                JArray list = cls as JArray;
                if (list == null) list = (cls != null) ? new JArray(cls.DeepClone()) : new JArray();
                return new JObject(new JProperty(block, list));
            }

            // Neu thu vien (ban sau) da boc dung lop khoi thi khong boc them.
            JObject obj = content as JObject;
            if (obj != null && obj.Count == 1 && obj.Property(block) != null) return content;

            return new JObject(new JProperty(block, content));
        }

        /// <summary>Ha chu thuong MOI ten khoa (de quy). Gia tri giu nguyen (chuoi/so/bool/null).</summary>
        private static JToken LowercaseKeys(JToken token)
        {
            JObject obj = token as JObject;
            if (obj != null)
            {
                JObject result = new JObject();
                foreach (JProperty prop in obj.Properties())
                    result.Add(prop.Name.ToLowerInvariant(), LowercaseKeys(prop.Value));
                return result;
            }

            JArray arr = token as JArray;
            if (arr != null)
            {
                JArray result = new JArray();
                foreach (JToken item in arr) result.Add(LowercaseKeys(item));
                return result;
            }

            return (token != null) ? token.DeepClone() : null;
        }
    }
}
