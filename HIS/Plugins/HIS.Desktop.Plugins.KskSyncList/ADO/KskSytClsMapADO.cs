/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.KskSyncList.ADO
{
    /// <summary>
    /// Mot dong o DANH SACH BEN TRAI cua man hinh noi chi so — co the la CHI SO XET NGHIEM hoac
    /// mot DICH VU (sieu am, phau thuat - thu thuat).
    ///
    /// VI SAO CAN: hai chi tieu "X-quang nhu" va "Sieu am 02 tuyen vu" cua cong KHONG phai chi so
    /// xet nghiem ma la KET QUA CUA MOT DICH VU, nen khong co trong danh muc chi so. Khong cho chon
    /// dich vu thi hai chi tieu do vinh vien khong noi duoc.
    ///
    /// TEN THUOC TINH DAT TRUNG voi V_HIS_TEST_INDEX de luoi ben trai giu nguyen cot da thiet ke,
    /// khong phai sua tep bo cuc.
    /// </summary>
    public class KskClsSourceADO
    {
        /// <summary>Ma: chi so xet nghiem thi la ma chi so, dich vu thi la ma dich vu.</summary>
        public string TEST_INDEX_CODE { get; set; }
        public string TEST_INDEX_NAME { get; set; }
        public string TEST_INDEX_UNIT_NAME { get; set; }
        /// <summary>Nhom hien thi. Voi dich vu thi ghi "Dich vu - {ten loai dich vu}".</summary>
        public string TEST_INDEX_GROUP_NAME { get; set; }

        /// <summary>true = dich vu, false = chi so xet nghiem.</summary>
        public bool IS_SERVICE { get; set; }

        public string SourceKind
        {
            get { return this.IS_SERVICE ? KskSytClsFieldStore.SOURCE_KIND__SERVICE
                                         : KskSytClsFieldStore.SOURCE_KIND__TEST_INDEX; }
        }

        /// <summary>Khoa tra cuu — ghep loai voi ma, vi ma dich vu co the trung ma chi so.</summary>
        public string Key { get { return SourceKind + "|" + (this.TEST_INDEX_CODE ?? ""); } }
    }

    /// <summary>
    /// Mot dong tren luoi "Noi chi so can lam sang": 1 chi tieu can lam sang cua mau M3
    /// (co dinh theo dac ta cong SYT TP.HCM) va chi so xet nghiem cua HIS da noi vao chi tieu do.
    /// </summary>
    public class KskSytClsFieldADO
    {
        /// <summary>Nhom chi tieu (Cong thuc mau / Sinh hoa mau / Nuoc tieu / Tam soat nu) — chi de hien thi.</summary>
        public string GroupName { get; set; }
        /// <summary>Ma chi tieu trong goi du lieu M3, vd xnm_slhc. La KHOA khi luu.</summary>
        public string FieldCode { get; set; }
        /// <summary>Ten chi tieu theo dac ta cong.</summary>
        public string FieldName { get; set; }
        /// <summary>Ma chi so xet nghiem cua HIS da noi. Rong = chua noi -> khong day chi tieu nay.</summary>
        public string TestIndexCode { get; set; }
        /// <summary>Ten chi so HIS, tra tu danh muc theo TestIndexCode — khong luu.</summary>
        public string TestIndexName { get; set; }
        /// <summary>Don vi do cua chi so HIS, hien ra de nguoi van hanh tu doi chieu — khong luu.</summary>
        public string TestIndexUnitName { get; set; }
        /// <summary>
        /// Loai nguon da noi: "XN" = chi so xet nghiem, "DV" = dich vu.
        /// Ban khai bao cu KHONG co truong nay -> doc len la rong, hieu la "XN" nhu truoc.
        /// </summary>
        public string SourceKind { get; set; }

        /// <summary>Ghi chu doi soat cua nguoi van hanh.</summary>
        public string Note { get; set; }

        public bool IsMapped { get { return !string.IsNullOrWhiteSpace(this.TestIndexCode); } }

        public bool IsService
        {
            get { return this.SourceKind == KskSytClsFieldStore.SOURCE_KIND__SERVICE; }
        }
    }

    /// <summary>Mot cap noi khi luu / xuat tep. CHI luu ma, ten tra lai tu danh muc khi mo.</summary>
    public class KskSytClsMapItemADO
    {
        public string FieldCode { get; set; }
        public string TestIndexCode { get; set; }
        /// <summary>"XN" hoac "DV". Rong = ban khai bao cu, hieu la "XN".</summary>
        public string SourceKind { get; set; }
        public string Note { get; set; }
    }

    /// <summary>Boc ngoai khi luu / xuat tep — co Version de sau nay doi dinh dang van doc duoc ban cu.</summary>
    public class KskSytClsMapFileADO
    {
        public string Version { get; set; }
        public string FormCode { get; set; }
        public List<KskSytClsMapItemADO> Items { get; set; }

        public KskSytClsMapFileADO()
        {
            this.Version = KskSytClsFieldStore.MAP_FILE_VERSION;
            this.FormCode = KskSytClsFieldStore.FORM_CODE__M3;
            this.Items = new List<KskSytClsMapItemADO>();
        }
    }

    /// <summary>
    /// Danh sach 34 chi tieu can lam sang cua mau M3 — CO DINH theo dac ta cong SYT TP.HCM:
    /// 14 cong thuc mau + 5 sinh hoa mau + 11 nuoc tieu + 4 tam soat rieng cho nu.
    /// Nguoi dung khong them/bot dong.
    /// Chi tieu chuan_doan_hinh_anh (X-quang tim phoi thang) KHONG nam o day vi no la ket qua
    /// cua 1 DICH VU, khong phai chi so xet nghiem.
    /// </summary>
    public static class KskSytClsFieldStore
    {
        public const string MAP_FILE_VERSION = "1";
        public const string FORM_CODE__M3 = "M3";

        /// <summary>Nguon gia tri la chi so xet nghiem (mac dinh, va la cach duy nhat cua ban cu).</summary>
        public const string SOURCE_KIND__TEST_INDEX = "XN";
        /// <summary>Nguon gia tri la ket qua cua mot dich vu (sieu am, phau thuat - thu thuat).</summary>
        public const string SOURCE_KIND__SERVICE = "DV";

        /// <summary>
        /// Ten cu, GIU LAI de doc duoc tep khai bao da xuat truoc day. Ban khai bao luu tren may
        /// khong ghi lai FormCode nen doi hang so nay khong lam mat khai bao cu.
        /// </summary>
        public const string FORM_CODE__M4 = "M4";

        public const string GROUP__BLOOD = "Công thức máu";
        public const string GROUP__BIOCHEM = "Sinh hóa máu";
        public const string GROUP__URINE = "Nước tiểu";
        public const string GROUP__SCREENING = "Tầm soát (nữ)";


        /// <summary>Dung danh sach 34 dong trong (chua noi chi so nao).</summary>
        public static List<KskSytClsFieldADO> BuildFields()
        {
            List<KskSytClsFieldADO> rs = new List<KskSytClsFieldADO>();

            // --- Xet nghiem mau (14) ---
            Add(rs, GROUP__BLOOD, "xnm_slhc", "Số lượng hồng cầu");
            Add(rs, GROUP__BLOOD, "xnm_huyetsacto", "Huyết sắc tố");
            Add(rs, GROUP__BLOOD, "xnm_hematocrit", "Hematocrit");
            Add(rs, GROUP__BLOOD, "xnm_mcv", "Thể tích trung bình hồng cầu (MCV)");
            Add(rs, GROUP__BLOOD, "xnm_mch", "Lượng Hb trung bình hồng cầu (MCH)");
            Add(rs, GROUP__BLOOD, "xnm_mchc", "Nồng độ Hb trung bình hồng cầu (MCHC)");
            Add(rs, GROUP__BLOOD, "xnm_rdw", "Độ phân bố hồng cầu (RDW)");
            Add(rs, GROUP__BLOOD, "xnm_slbc", "Số lượng bạch cầu");
            Add(rs, GROUP__BLOOD, "xnm_slbc_trungtinh", "Bạch cầu trung tính");
            Add(rs, GROUP__BLOOD, "xnm_slbc_lympho", "Bạch cầu lympho");
            Add(rs, GROUP__BLOOD, "xnm_slbc_donnhan", "Bạch cầu đơn nhân");
            Add(rs, GROUP__BLOOD, "xnm_slbc_aitoan", "Bạch cầu ái toan");
            Add(rs, GROUP__BLOOD, "xnm_slbc_aikiem", "Bạch cầu ái kiềm");
            Add(rs, GROUP__BLOOD, "xnm_sltc", "Số lượng tiểu cầu");

            // --- Sinh hoa mau (5) ---
            Add(rs, GROUP__BIOCHEM, "shm_duongmau", "Đường máu");
            Add(rs, GROUP__BIOCHEM, "shm_ure", "Ure");
            Add(rs, GROUP__BIOCHEM, "shm_creatinin", "Creatinin");
            Add(rs, GROUP__BIOCHEM, "shm_asat_got", "ASAT (GOT)");
            Add(rs, GROUP__BIOCHEM, "shm_alat_gpt", "ALAT (GPT)");

            // --- Xet nghiem nuoc tieu (11) ---
            Add(rs, GROUP__URINE, "xnnt_glucose", "Glucose");
            Add(rs, GROUP__URINE, "xnnt_protein", "Protein");
            Add(rs, GROUP__URINE, "xnnt_titrong", "Tỉ trọng");
            Add(rs, GROUP__URINE, "xnnt_ph", "pH");
            Add(rs, GROUP__URINE, "xnnt_bachcau", "Bạch cầu niệu");
            Add(rs, GROUP__URINE, "xnnt_hongcau", "Hồng cầu niệu");
            Add(rs, GROUP__URINE, "xnnt_nitrit", "Nitrit");
            Add(rs, GROUP__URINE, "xnnt_cetonic", "Cetonic");
            Add(rs, GROUP__URINE, "xnnt_bilirubin", "Bilirubin");
            Add(rs, GROUP__URINE, "xnnt_urobilinogen", "Urobilinogen");
            Add(rs, GROUP__URINE, "xnnt_khac", "Chỉ số khác");

            // --- Tam soat rieng cho nu (4) ---
            // Co trong goi du lieu mau M3 (ban moi hon file POSTMAN trong repo — file do chi co xquang_nhu).
            // KIEU DU LIEU: van ban tu do. Trong goi mau, 2 truong mang chuoi go bua ("adasdsa", "qudad")
            // con 2 truong mang ma danh muc ("CDHA_XQuangNhu", "CDHA_SieuAm02TuyenVu") -> khong phai
            // truong tham chieu danh muc, ma la truong KET QUA dang chu.
            // Luu y khi day: nguon hop ly la KET QUA CUA DICH VU (mo ta/ket luan), khong phai gia tri chi so.
            Add(rs, GROUP__SCREENING, "xet_nghiem_te_bao_co_tu_cung", "Xét nghiệm tế bào cổ tử cung");
            Add(rs, GROUP__SCREENING, "xet_nghiem_hpv", "Xét nghiệm HPV");
            Add(rs, GROUP__SCREENING, "xquang_nhu", "X-quang nhũ");
            Add(rs, GROUP__SCREENING, "sieu_am_2_tuyen_vu", "Siêu âm 02 tuyến vú");

            return rs;
        }

        private static void Add(List<KskSytClsFieldADO> rs, string group, string fieldCode, string fieldName)
        {
            rs.Add(new KskSytClsFieldADO { GroupName = group, FieldCode = fieldCode, FieldName = fieldName });
        }
    }
}
