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
using System.Text;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// Ma hoa / giai ma cot HIS_TREATMENT.XML_PRECHECK_DESC.
    ///
    /// DAY LA HOP DONG DU LIEU. Cot nay la NOI DUY NHAT dung lai duoc cua so ket qua sau khi
    /// nguoi dung dong man hinh, nen doi quy cach la hong toan bo du lieu cu.
    /// Dong dau tien mang NHAN PHIEN BAN de sau nay con doi duoc mot cach an toan.
    ///
    /// Quy cach khi trang thai la 1, 2, 3 (co danh sach loi):
    ///   Dong 1        : #MDI1
    ///   Dong 2 tro di : muc_do|tep_thanh_phan|the_du_lieu|so_thu_tu_dong|gia_tri_hien_tai|noi_dung_loi
    ///                   muc_do = C (nghiem trong) hoac W (canh bao)
    ///   Dong cuoi     : #MORE|N   (chi co khi bi cat bot, N = so dong loi con lai)
    ///
    /// Quy cach khi trang thai la 4 hoac 5:
    ///   Van ban nghiep vu thuan, KHONG co nhan phien ban. Chuoi khong bat dau bang #MDI1
    ///   duoc hieu la ly do dang van ban.
    ///
    /// KHONG LUU dien giai day du (truong errorDesc cua he ngoai): doan do chiem ~80% dung luong
    /// va chua ho ten benh nhan, ma benh nhan, chan doan. Tham chieu: PTTK muc B.2.2, quy tac QT-28.
    /// </summary>
    public static class MdInsightDescCodec
    {
        /// <summary>Nhan phien ban quy cach - doi nhan moi duoc doi cau truc dong loi</summary>
        private const string VERSION_TAG = "#MDI1";

        /// <summary>Nhan dong bao con loi chua luu het</summary>
        private const string MORE_TAG = "#MORE";

        private const char FIELD_SEPARATOR = '|';
        private const char RECORD_SEPARATOR = '\n';

        private const string SEVERITY_CRITICAL = "C";
        private const string SEVERITY_WARNING = "W";

        /// <summary>Suc chua cot VARCHAR2(4000 BYTE) - dem theo BYTE chu khong phai ky tu</summary>
        private const int MAX_BYTE = 4000;

        /// <summary>Chua cho dong #MORE|NNNNNN o cuoi</summary>
        private const int MORE_LINE_RESERVE_BYTE = 20;

        /// <summary>
        /// Ma hoa danh sach dong loi thanh chuoi luu xuong cot.
        /// Vuot suc chua thi luu duoc bao nhieu dong thi luu va them dong bao so loi con lai -
        /// KHONG duoc cat cut giua chung ma khong bao (PTTK muc B.2.2).
        /// </summary>
        /// <param name="errors">Danh sach dong loi</param>
        /// <param name="truncatedCount">Tra ve so dong loi khong luu duoc</param>
        public static string Encode(List<MdInsightErrorADO> errors, out int truncatedCount)
        {
            truncatedCount = 0;
            try
            {
                if (errors == null || errors.Count == 0)
                {
                    return null;
                }

                StringBuilder builder = new StringBuilder();
                builder.Append(VERSION_TAG);

                int usedByte = ByteCount(VERSION_TAG);
                int written = 0;

                for (int i = 0; i < errors.Count; i++)
                {
                    string line = BuildLine(errors[i]);
                    //Cong 1 byte cho ky tu xuong dong dat truoc dong nay
                    int lineByte = ByteCount(line) + 1;

                    if (usedByte + lineByte > MAX_BYTE - MORE_LINE_RESERVE_BYTE)
                    {
                        truncatedCount = errors.Count - written;
                        break;
                    }

                    builder.Append(RECORD_SEPARATOR).Append(line);
                    usedByte += lineByte;
                    written++;
                }

                if (truncatedCount > 0)
                {
                    builder.Append(RECORD_SEPARATOR)
                           .Append(MORE_TAG).Append(FIELD_SEPARATOR).Append(truncatedCount);

                    //Khong ghi noi dung loi ra nhat ky - chi ghi so luong
                    LogSystem.Info("MdInsightDescCodec - Danh sach loi vuot suc chua cot, da luu "
                        + written + " dong, con " + truncatedCount + " dong khong luu duoc.");
                }

                return builder.ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Giai ma chuoi da luu thanh danh sach dong loi.
        /// Chuoi khong bat dau bang nhan phien ban duoc hieu la LY DO dang van ban (trang thai 4/5),
        /// khi do tra ve danh sach rong va <paramref name="reason"/> mang noi dung do.
        /// </summary>
        public static List<MdInsightErrorADO> Decode(string stored, out string reason, out int truncatedCount)
        {
            reason = null;
            truncatedCount = 0;
            List<MdInsightErrorADO> result = new List<MdInsightErrorADO>();

            try
            {
                if (String.IsNullOrWhiteSpace(stored))
                {
                    return result;
                }

                string[] lines = stored.Split(RECORD_SEPARATOR);

                if ((lines[0] ?? "").Trim() != VERSION_TAG)
                {
                    //Khong phai danh sach loi - day la ly do dang van ban nghiep vu
                    reason = stored.Trim();
                    return result;
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = (lines[i] ?? "").Trim();
                    if (String.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (line.StartsWith(MORE_TAG, StringComparison.Ordinal))
                    {
                        string[] moreParts = line.Split(FIELD_SEPARATOR);
                        int count;
                        if (moreParts.Length > 1 && Int32.TryParse(moreParts[1], out count))
                        {
                            truncatedCount = count;
                        }
                        continue;
                    }

                    string[] fields = line.Split(FIELD_SEPARATOR);
                    if (fields.Length < 6)
                    {
                        //Dong hong - bo qua rieng dong do, khong lam hong ca cua so ket qua
                        continue;
                    }

                    result.Add(new MdInsightErrorADO
                    {
                        IsCritical = String.Equals(fields[0], SEVERITY_CRITICAL, StringComparison.OrdinalIgnoreCase),
                        FileName = fields[1],
                        TagName = fields[2],
                        RowIndex = fields[3],
                        CurrentValue = fields[4],
                        //Noi dung loi la thanh phan cuoi - noi lai phong khi chinh no tung chua dau gach dung
                        ErrorContent = String.Join(FIELD_SEPARATOR.ToString(), fields, 5, fields.Length - 5)
                    });
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return result;
        }

        /// <summary>
        /// Lam sach mot cau ly do truoc khi luu (trang thai 4 hoac 5).
        /// Ly do PHAI la van ban nghiep vu, khong duoc chua du lieu benh nhan - quy tac QT-28.
        /// Ham nay chi bao dam ve mat ky thuat (do dai, ky tu dieu khien);
        /// trach nhiem khong dua du lieu benh nhan vao day thuoc ve noi sinh ra cau ly do.
        /// </summary>
        public static string EncodeReason(string reason)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(reason))
                {
                    return null;
                }

                string cleaned = Sanitize(reason, keepSeparator: true);

                //Cat theo BYTE de khong vuot suc chua cot
                return TruncateByByte(cleaned, MAX_BYTE);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>Dung mot dong loi tu sau thanh phan - KHONG bao gom dien giai day du</summary>
        private static string BuildLine(MdInsightErrorADO error)
        {
            StringBuilder line = new StringBuilder();
            line.Append(error.IsCritical ? SEVERITY_CRITICAL : SEVERITY_WARNING);
            line.Append(FIELD_SEPARATOR).Append(Sanitize(error.FileName, keepSeparator: false));
            line.Append(FIELD_SEPARATOR).Append(Sanitize(error.TagName, keepSeparator: false));
            line.Append(FIELD_SEPARATOR).Append(Sanitize(error.RowIndex, keepSeparator: false));
            line.Append(FIELD_SEPARATOR).Append(Sanitize(error.CurrentValue, keepSeparator: false));
            //Thanh phan cuoi cung duoc giu dau gach dung: khi giai ma se noi lai phan con lai
            line.Append(FIELD_SEPARATOR).Append(Sanitize(error.ErrorContent, keepSeparator: true));
            return line.ToString();
        }

        /// <summary>
        /// Bo ky tu dieu khien va ky tu xuong dong (chung la ky tu ngan cach ban ghi).
        /// <paramref name="keepSeparator"/> = false thi doi luon dau gach dung thanh dau gach cheo,
        /// de viec tach thanh phan luc giai ma khong bao gio sai.
        /// </summary>
        private static string Sanitize(string value, bool keepSeparator)
        {
            if (String.IsNullOrEmpty(value))
            {
                return "";
            }

            StringBuilder builder = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                if (Char.IsControl(c))
                {
                    //Gom ca xuong dong, xuong dong ve dau va tab
                    builder.Append(' ');
                }
                else if (c == FIELD_SEPARATOR && !keepSeparator)
                {
                    builder.Append('/');
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString().Trim();
        }

        private static int ByteCount(string value)
        {
            return String.IsNullOrEmpty(value) ? 0 : Encoding.UTF8.GetByteCount(value);
        }

        /// <summary>
        /// Cat chuoi theo gioi han BYTE, khong cat giua mot ky tu nhieu byte.
        /// Chu tieng Viet co dau chiem toi 3 byte nen khong duoc dem theo ky tu.
        /// </summary>
        private static string TruncateByByte(string value, int maxByte)
        {
            if (String.IsNullOrEmpty(value) || ByteCount(value) <= maxByte)
            {
                return value;
            }

            int length = value.Length;
            while (length > 0 && Encoding.UTF8.GetByteCount(value.Substring(0, length)) > maxByte)
            {
                length--;
            }

            return value.Substring(0, length);
        }
    }
}
