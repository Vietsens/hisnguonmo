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
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// Doc khoa cau hinh HIS.MDINSIGHT.CONNECTION_INFO.
    ///
    /// Dinh dang HAI TANG NGAN CACH:
    ///   doan_ket_noi ; doan_tai_khoan_1 ; doan_tai_khoan_2 ; ...
    ///
    ///   Doan 1 (ket noi), ngan boi dau gach dung:
    ///     dia_chi [| han_muc_mot_luot_goi] [| thoi_gian_giam_dinh_toi_thieu]
    ///             [| khoang_cho_sau_khi_gui] [| han_giu_chua_co_ket_qua] [| khoang_nghi_giua_hai_luot_tra]
    ///     Thanh phan 1 BAT BUOC, nam thanh phan con lai tuy chon (don vi giay, so nguyen duong).
    ///
    ///   Doan 2 tro di (tai khoan), LUON DUNG BA thanh phan:
    ///     ma_co_so_kcb | tai_khoan | mat_khau
    ///
    /// Moi co so kham chua benh mot doan. Them co so = them mot doan, khong them khoa.
    ///
    /// QUY TAC TACH: cat theo dau CHAM PHAY truoc de ra cac doan, roi moi cat tung doan theo dau
    /// GACH DUNG. Doan thu nhat LUON la ket noi, tu doan thu hai tro di LUON la tai khoan -
    /// khong suy doan theo noi dung.
    ///
    /// BAO MAT: khong bao gio ghi nhat ky gia tri tho, ten tai khoan hay mat khau.
    /// Nhat ky chi ghi SO LUONG doan/thanh phan doc duoc va ma co so (khong phai du lieu nhay cam).
    ///
    /// Tham chieu: PTTK muc B.2.5.1, B.2.5.2, B.2.5.5, quy tac QT-01, QT-26.
    /// </summary>
    public class MdInsightConfig
    {
        private const char SEGMENT_SEPARATOR = ';';
        private const char FIELD_SEPARATOR = '|';

        /// <summary>So thanh phan toi da cua doan ket noi: dia chi + 5 nguong</summary>
        private const int MAX_CONNECTION_FIELD = 6;

        /// <summary>So thanh phan cua mot doan tai khoan CO khai ma co so</summary>
        private const int ACCOUNT_FIELD_COUNT = 3;

        /// <summary>So thanh phan cua mot doan tai khoan KHONG khai ma co so - tai khoan dung chung</summary>
        private const int ACCOUNT_FIELD_COUNT_NO_ORG = 2;

        /// <summary>
        /// Tai khoan dung chung cho moi ho so, khai bang hai thanh phan (khong co ma co so).
        /// CHI hop le khi ca cau hinh co dung MOT tai khoan.
        /// </summary>
        private MdInsightAccountADO fallbackAccount;

        //Gia tri mac dinh - bang nguong muc B.2.5.2 la nguon duy nhat
        public const int DEFAULT_CALL_TIMEOUT_SECOND = 15;
        public const int DEFAULT_MIN_CHECK_DURATION_SECOND = 300;
        public const int DEFAULT_WAIT_AFTER_UPLOAD_SECOND = 30;
        public const int DEFAULT_PENDING_HOLD_SECOND = 86400;
        public const int DEFAULT_REST_BETWEEN_CALL_SECOND = 0;

        //Tran cua tung nguong. Khai vuot tran thi coi la go nham va dung lai gia tri mac dinh.
        //Rieng "khoang cho sau khi gui" tran chi 10 phut vi nguoi dung phai NGOI DOI truoc
        //cua so tien do trong suot khoang do - dai hon nua la khong dung duoc.
        private const int MAX_CALL_TIMEOUT_SECOND = 300;
        private const int MAX_MIN_CHECK_DURATION_SECOND = 86400;
        private const int MAX_WAIT_AFTER_UPLOAD_SECOND = 600;
        private const int MAX_PENDING_HOLD_SECOND = 604800;
        private const int MAX_REST_BETWEEN_CALL_SECOND = 60;

        private readonly Dictionary<string, MdInsightAccountADO> accountByMediOrg
            = new Dictionary<string, MdInsightAccountADO>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Dia chi may chu MDInsight - thanh phan bat buoc</summary>
        public string BaseUrl { get; private set; }

        /// <summary>Han muc mot luot goi he ngoai (giay) - bang nguong dong 1</summary>
        public int CallTimeoutSecond { get; private set; }

        /// <summary>Thoi gian giam dinh toi thieu (giay) - dieu kien duoc phep ket luan ho so sach, quy tac QT-16</summary>
        public int MinCheckDurationSecond { get; private set; }

        /// <summary>Khoang cho sau khi gui, truoc khi tra (giay) - bang nguong dong 3</summary>
        public int WaitAfterUploadSecond { get; private set; }

        /// <summary>Han giu trang thai Chua co ket qua (giay) - bang nguong dong 7</summary>
        public int PendingHoldSecond { get; private set; }

        /// <summary>Khoang nghi toi thieu giua hai luot tra (giay) - bang nguong dong 8</summary>
        public int RestBetweenCallSecond { get; private set; }

        /// <summary>
        /// True khi co dia chi dung giao thuc ma hoa VA it nhat mot doan tai khoan hop le.
        /// Thieu mot trong hai la TAT ca tinh nang - quy tac QT-01.
        /// </summary>
        public bool IsValidConfig { get; private set; }

        /// <summary>Danh sach ma co so da khai tai khoan - dung de doi chieu nhom ho so</summary>
        public List<string> DeclaredMediOrgCodes
        {
            get { return this.accountByMediOrg.Keys.ToList(); }
        }

        public MdInsightConfig(string connectionInfo)
        {
            //Dat mac dinh truoc, sau do ghi de neu cau hinh co khai
            this.CallTimeoutSecond = DEFAULT_CALL_TIMEOUT_SECOND;
            this.MinCheckDurationSecond = DEFAULT_MIN_CHECK_DURATION_SECOND;
            this.WaitAfterUploadSecond = DEFAULT_WAIT_AFTER_UPLOAD_SECOND;
            this.PendingHoldSecond = DEFAULT_PENDING_HOLD_SECOND;
            this.RestBetweenCallSecond = DEFAULT_REST_BETWEEN_CALL_SECOND;
            this.IsValidConfig = false;

            try
            {
                if (String.IsNullOrWhiteSpace(connectionInfo))
                {
                    //Chua khai = vien chua dau noi. Khong ghi canh bao, day la trang thai binh thuong.
                    return;
                }

                //Tang 1: cat theo dau cham phay, bo qua doan rong (hau qua cua dau cham phay thua)
                List<string> segments = connectionInfo
                    .Split(SEGMENT_SEPARATOR)
                    .Select(o => (o ?? "").Trim())
                    .Where(o => !String.IsNullOrEmpty(o))
                    .ToList();

                if (segments.Count == 0)
                {
                    LogSystem.Warn("MdInsightConfig - Cau hinh HIS.MDINSIGHT.CONNECTION_INFO khong co doan nao doc duoc. Tinh nang TAT.");
                    return;
                }

                if (!ParseConnectionSegment(segments[0]))
                {
                    return;
                }

                //Tang 2: tu doan thu hai tro di la tai khoan
                for (int i = 1; i < segments.Count; i++)
                {
                    ParseAccountSegment(segments[i], i);
                }

                int declaredCount = this.accountByMediOrg.Count + (this.fallbackAccount != null ? 1 : 0);

                if (declaredCount == 0)
                {
                    LogSystem.Warn("MdInsightConfig - Cau hinh co dia chi nhung KHONG co doan tai khoan hop le nao. Tinh nang TAT - quy tac QT-01.");
                    return;
                }

                //⚠️ Tai khoan dung chung (khai hai thanh phan) CHI an toan khi ca cau hinh co dung mot
                //tai khoan. Tu hai tai khoan tro len thi moi tai khoan BAT BUOC phai khai ma co so:
                //khong co ma thi phan mem khong biet ho so nao dung tai khoan nao, ma gui nham tai khoan
                //thi he ngoai VAN NHAN TEP nhung KHONG BAO GIO tra ket qua (tai khoan bi co lap theo
                //co so - xem PTTK muc A.2.8). Khi do ho so se bi ket luan "khong co loi" oan sau khi
                //qua nguong thoi gian giam dinh - dung loai sai lam nguy hiem nhat cua tinh nang nay.
                if (this.fallbackAccount != null && declaredCount > 1)
                {
                    LogSystem.Warn("MdInsightConfig - Cau hinh vua co tai khoan dung chung (khai 2 thanh phan)"
                        + " vua co " + this.accountByMediOrg.Count + " tai khoan theo co so."
                        + " Tu hai tai khoan tro len thi MOI tai khoan deu phai khai ma co so."
                        + " Tinh nang TAT de tranh gui nham tai khoan.");
                    return;
                }

                this.IsValidConfig = true;

                LogSystem.Info(this.fallbackAccount != null
                    ? "MdInsightConfig - Da doc cau hinh: mot tai khoan dung chung cho moi co so."
                    : "MdInsightConfig - Da doc cau hinh: " + this.accountByMediOrg.Count
                      + " co so kham chua benh duoc khai tai khoan.");

                //In dia chi may chu va danh sach ma co so dang khai. Khong co hai thong tin nay
                //thi khi goi nham may chu hoac khai thieu co so deu phai mo bang HIS_CONFIG ra doi chieu.
                //KHONG in tai khoan va mat khau - quy tac QT-28.
                LogSystem.Info("MdInsightConfig - May chu: " + this.BaseUrl
                    + " | ma co so dang khai: "
                    + (this.accountByMediOrg.Count > 0
                        ? String.Join(", ", this.accountByMediOrg.Keys.ToArray())
                        : "(khong khai - dung chung)"));

                //In ra nguong THUC SU dang dung, de khi khai xong ma phan mem van chay theo so cu
                //thi nhin nhat ky la biet ngay do chua nap lai cau hinh, khong phai doan
                LogSystem.Info("MdInsightConfig - Nguong dang dung:"
                    + " han muc mot luot goi=" + this.CallTimeoutSecond + "s"
                    + " | thoi gian giam dinh toi thieu=" + this.MinCheckDurationSecond + "s"
                    + " | khoang cho sau khi gui=" + this.WaitAfterUploadSecond + "s"
                    + " | han giu Chua co ket qua=" + this.PendingHoldSecond + "s"
                    + " | khoang nghi giua hai luot tra=" + this.RestBetweenCallSecond + "s");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                this.IsValidConfig = false;
            }
        }

        /// <summary>
        /// Doc doan ket noi. Tra ve false khi cau hinh khong hop le (da ghi nhat ky ly do).
        /// </summary>
        private bool ParseConnectionSegment(string segment)
        {
            string[] fields = segment.Split(FIELD_SEPARATOR).Select(o => (o ?? "").Trim()).ToArray();

            //Chan doan loi khai bao de mac nhat cua dinh dang hai tang:
            //quan tri quen dau cham phay truoc danh sach tai khoan nen tat ca dinh vao doan dau.
            if (fields.Length > MAX_CONNECTION_FIELD)
            {
                LogSystem.Warn("MdInsightConfig - Doan ket noi co " + fields.Length
                    + " thanh phan, vuot muc toi da " + MAX_CONNECTION_FIELD
                    + ". Gan nhu chac chan la THIEU DAU CHAM PHAY truoc danh sach tai khoan."
                    + " Dinh dang dung: dia_chi [|5 nguong] ; ma_co_so|tai_khoan|mat_khau ; ..."
                    + " Tinh nang TAT.");
                return false;
            }

            this.BaseUrl = fields[0];

            if (String.IsNullOrEmpty(this.BaseUrl))
            {
                LogSystem.Warn("MdInsightConfig - Doan ket noi thieu dia chi may chu. Tinh nang TAT.");
                return false;
            }

            //Bat buoc giao thuc ma hoa: kenh nay cho ca mat khau lan toan bo noi dung
            //tep XML ho so benh nhan. Khong duoc tu cham chuoc, ke ca moi truong thu nghiem.
            //Tham chieu: PTTK muc B.2.5.6 rui ro 2.
            if (!this.BaseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                LogSystem.Warn("MdInsightConfig - Dia chi may chu KHONG dung giao thuc ma hoa (https)."
                    + " Coi nhu cau hinh khong hop le, tinh nang TAT - PTTK muc B.2.5.6 rui ro 2.");
                this.BaseUrl = null;
                return false;
            }

            //Nam thanh phan tuy chon, don vi giay. Khai sai kieu thi bo qua RIENG thanh phan do
            //va dung mac dinh - khong tat ca tinh nang (PTTK muc B.2.5.5).
            this.CallTimeoutSecond = ReadOptionalSecond(fields, 1,
                DEFAULT_CALL_TIMEOUT_SECOND, MAX_CALL_TIMEOUT_SECOND, "han muc mot luot goi");
            this.MinCheckDurationSecond = ReadOptionalSecond(fields, 2,
                DEFAULT_MIN_CHECK_DURATION_SECOND, MAX_MIN_CHECK_DURATION_SECOND, "thoi gian giam dinh toi thieu");
            this.WaitAfterUploadSecond = ReadOptionalSecond(fields, 3,
                DEFAULT_WAIT_AFTER_UPLOAD_SECOND, MAX_WAIT_AFTER_UPLOAD_SECOND, "khoang cho sau khi gui");
            this.PendingHoldSecond = ReadOptionalSecond(fields, 4,
                DEFAULT_PENDING_HOLD_SECOND, MAX_PENDING_HOLD_SECOND, "han giu trang thai Chua co ket qua");
            this.RestBetweenCallSecond = ReadOptionalSecond(fields, 5,
                DEFAULT_REST_BETWEEN_CALL_SECOND, MAX_REST_BETWEEN_CALL_SECOND, "khoang nghi giua hai luot tra");

            return true;
        }

        /// <summary>
        /// Doc mot thanh phan nguong tuy chon. Phai la so nguyen duong (rieng khoang nghi cho phep 0)
        /// va KHONG vuot tran cua rieng thanh phan do.
        ///
        /// ⚠️ Co tran la bat buoc: go thua mot chu so o thanh phan "khoang cho sau khi gui" se bat
        /// nguoi dung ngoi truoc cua so tien do hang ngay, ma phan mem van coi la cau hinh hop le.
        /// </summary>
        private static int ReadOptionalSecond(
            string[] fields, int index, int defaultValue, int maxValue, string fieldName)
        {
            try
            {
                if (fields == null || index >= fields.Length || String.IsNullOrEmpty(fields[index]))
                {
                    return defaultValue;
                }

                int value;
                if (!Int32.TryParse(fields[index], out value) || value < 0)
                {
                    LogSystem.Warn("MdInsightConfig - Thanh phan '" + fieldName + "' khai sai kieu"
                        + " (phai la so nguyen khong am, don vi giay). Dung gia tri mac dinh " + defaultValue + ".");
                    return defaultValue;
                }

                //Rieng gia tri 0 chi hop le voi khoang nghi giua hai luot tra
                if (value == 0 && defaultValue != DEFAULT_REST_BETWEEN_CALL_SECOND)
                {
                    LogSystem.Warn("MdInsightConfig - Thanh phan '" + fieldName + "' khai bang 0,"
                        + " khong hop le. Dung gia tri mac dinh " + defaultValue + ".");
                    return defaultValue;
                }

                if (value > maxValue)
                {
                    LogSystem.Warn("MdInsightConfig - Thanh phan '" + fieldName + "' khai " + value
                        + " giay, vuot tran cho phep " + maxValue + " giay. Nghi go nham so."
                        + " Dung gia tri mac dinh " + defaultValue + ".");
                    return defaultValue;
                }

                return value;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// Doc mot doan tai khoan. Doan hong thi BO QUA RIENG doan do, ghi nhat ky canh bao
        /// cho quan tri, KHONG bao loi cho nguoi dung (loi cau hinh, khong phai loi nghiep vu cua ho).
        /// </summary>
        private void ParseAccountSegment(string segment, int segmentIndex)
        {
            try
            {
                string[] fields = segment.Split(FIELD_SEPARATOR).Select(o => (o ?? "").Trim()).ToArray();

                if ((fields.Length != ACCOUNT_FIELD_COUNT && fields.Length != ACCOUNT_FIELD_COUNT_NO_ORG)
                    || fields.Any(String.IsNullOrEmpty))
                {
                    //Nguyen nhan hay gap nhat: mat khau chua dau gach dung hoac dau cham phay.
                    //Ghi SO thanh phan doc duoc, TUYET DOI khong ghi gia tri - PTTK muc B.2.5.6 rui ro 3.
                    LogSystem.Warn("MdInsightConfig - Doan tai khoan thu " + segmentIndex
                        + " doc duoc " + fields.Length + " thanh phan, can " + ACCOUNT_FIELD_COUNT_NO_ORG
                        + " (tai_khoan|mat_khau) hoac " + ACCOUNT_FIELD_COUNT
                        + " (ma_co_so|tai_khoan|mat_khau). Kha nang mat khau chua ky tu ngan cach."
                        + " Bo qua doan nay.");
                    return;
                }

                //Khai hai thanh phan = KHONG neu ma co so = tai khoan dung chung cho MOI ho so.
                //Chi cho phep khi ca cau hinh chi co DUNG MOT tai khoan - kiem o cuoi ham doc cau hinh.
                if (fields.Length == ACCOUNT_FIELD_COUNT_NO_ORG)
                {
                    this.fallbackAccount = new MdInsightAccountADO
                    {
                        MediOrgCode = "",
                        UserName = fields[0],
                        Password = fields[1]
                    };
                    return;
                }

                string mediOrgCode = fields[0];

                if (this.accountByMediOrg.ContainsKey(mediOrgCode))
                {
                    LogSystem.Warn("MdInsightConfig - Ma co so " + mediOrgCode
                        + " bi khai trung o doan thu " + segmentIndex + ". Bo qua doan nay, giu doan khai truoc.");
                    return;
                }

                this.accountByMediOrg.Add(mediOrgCode, new MdInsightAccountADO
                {
                    MediOrgCode = mediOrgCode,
                    UserName = fields[1],
                    Password = fields[2]
                });
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lay tai khoan cua mot co so kham chua benh. Tra ve null khi co so chua duoc khai -
        /// khi do ho so cua co so do mang trang thai Khong kiem tra duoc, CAC CO SO KHAC VAN CHAY
        /// binh thuong (quy tac QT-09b).
        /// </summary>
        public MdInsightAccountADO GetAccount(string mediOrgCode)
        {
            MdInsightAccountADO account;

            if (!String.IsNullOrWhiteSpace(mediOrgCode)
                && this.accountByMediOrg.TryGetValue(mediOrgCode.Trim(), out account))
            {
                return account;
            }

            //Khong co tai khoan khai rieng cho co so nay - dung tai khoan dung chung neu co.
            //Tra ve null thi ho so mang trang thai Khong kiem tra duoc (quy tac QT-09b).
            return this.fallbackAccount;
        }

        /// <summary>Co so nay da duoc khai tai khoan chua</summary>
        public bool HasAccount(string mediOrgCode)
        {
            return GetAccount(mediOrgCode) != null;
        }
    }
}
