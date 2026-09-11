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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PaanExecuteList.ADO
{
    /// <summary>
    /// Doi tuong hien thi tren luoi danh sach Giai phau benh.
    /// Ke thua view V_HIS_SERE_SERV_GPBL, bo sung cac cot chi dung de HIEN THI
    /// (so thu tu, nam sinh, tuoi, chuoi ngay gio da dinh dang).
    /// </summary>
    public class PaanSereServADO : V_HIS_SERE_SERV_GPBL
    {
        /// <summary>Cot 1: So thu tu tren luoi, gan sau khi sap xep.</summary>
        public int STT { get; set; }

        /// <summary>Cot 10: Tuoi cua BN den thoi diem hien tai.</summary>
        public string AGE_DISPLAY { get; set; }

        /// <summary>Cot 11: Nam sinh, cat 4 ky tu dau cua TDL_PATIENT_DOB.</summary>
        public string DOB_DISPLAY { get; set; }

        /// <summary>Cot 2: Thoi gian y lenh da dinh dang dd/MM/yyyy HH:mm:ss.</summary>
        public string INTRUCTION_TIME_DISPLAY { get; set; }

        /// <summary>Cot 3: Ngay bat dau da dinh dang.</summary>
        public string BEGIN_TIME_DISPLAY { get; set; }

        /// <summary>Cot 4: Ngay ket thuc da dinh dang.</summary>
        public string END_TIME_DISPLAY { get; set; }

        /// <summary>
        /// Ngay ket qua da dinh dang (viec 52795).
        /// Nguon: HIS_SERE_SERV_EXT.RESULT_READ_TIME - chinh la o "Ngay KQ"
        /// tren man Tra ket qua.
        /// </summary>
        public string RESULT_READ_TIME_DISPLAY { get; set; }

        public PaanSereServADO() { }

        public PaanSereServADO(V_HIS_SERE_SERV_GPBL data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<PaanSereServADO>(this, data);

                    // Nam sinh: cat 4 ky tu dau cua so ngay sinh (YYYYMMDD...).
                    // Lam giong man hinh cu (ExecuteRoom\ADO\ServiceReqADO.cs) cho dong bo.
                    this.DOB_DISPLAY = data.TDL_PATIENT_DOB > 0
                        ? data.TDL_PATIENT_DOB.ToString().Substring(0, 4)
                        : null;

                    this.AGE_DISPLAY = CalculateAge(data.TDL_PATIENT_DOB);

                    this.INTRUCTION_TIME_DISPLAY = FormatTime(data.TDL_INTRUCTION_TIME);

                    // BEGIN_TIME co kieu decimal? chu KHONG phai long?, vi trong cau
                    // tao view no di qua ham NVL() nen Oracle suy ra kieu NUMBER
                    // khong ro do dai. Phai ep kieu truoc khi dinh dang.
                    this.BEGIN_TIME_DISPLAY = FormatTime(
                        data.BEGIN_TIME.HasValue ? (long?)data.BEGIN_TIME.Value : null);

                    this.END_TIME_DISPLAY = FormatTime(data.END_TIME);
                    this.RESULT_READ_TIME_DISPLAY = FormatTime(data.RESULT_READ_TIME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tinh tuoi tu so ngay sinh dang YYYYMMDD... den ngay hien tai.
        /// Tra ve null neu ngay sinh khong hop le.
        /// </summary>
        private static string CalculateAge(long dob)
        {
            try
            {
                if (dob <= 0) return null;

                string raw = dob.ToString();
                if (raw.Length < 4) return null;

                int year;
                if (!int.TryParse(raw.Substring(0, 4), out year)) return null;
                if (year <= 0 || year > DateTime.Now.Year) return null;

                int age = DateTime.Now.Year - year;

                // Neu co du thang va ngay thi tru bot 1 tuoi khi chua den sinh nhat.
                if (raw.Length >= 8)
                {
                    int month, day;
                    if (int.TryParse(raw.Substring(4, 2), out month)
                        && int.TryParse(raw.Substring(6, 2), out day)
                        && month >= 1 && month <= 12 && day >= 1 && day <= 31)
                    {
                        if (DateTime.Now.Month < month
                            || (DateTime.Now.Month == month && DateTime.Now.Day < day))
                        {
                            age = age - 1;
                        }
                    }
                }

                return age >= 0 ? age.ToString() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Doi so 14 chu so YYYYMMDDHHMMSS sang chuoi hien thi.
        /// CANH BAO: TimeNumberToSystemDateTime tra ve null khi gio bi rac
        /// (vi du phut = 60). Khi do tra ve null de o luoi de trong,
        /// khong duoc de ngoai le lam vo ca dong.
        /// </summary>
        private static string FormatTime(long? time)
        {
            try
            {
                if (!time.HasValue || time.Value <= 0) return null;

                DateTime? value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(time.Value);
                return value.HasValue ? value.Value.ToString("dd/MM/yyyy HH:mm:ss") : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
