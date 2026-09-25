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
namespace HIS.Desktop.Plugins.CallPatientSample.ADO
{
    /// <summary>
    /// Mot luot benh nhan cho lay mau tren man hinh cho _NA.
    /// Thay cho bang HIS_TREATMENT_SAMPLE_DESK / view V_HIS_TREATMENT_SAMPLE_DESK cua ban goc:
    /// du lieu dung lai tu api/LisSample/GetView, con co "dang goi" va thoi diem goi
    /// lay tu CallPatientDataWorker (cache tinh cua phong lay mau) nen khong can them bang.
    /// </summary> 
    public class SampleWaitingNaADO
    {
        /// <summary>Khoa dinh danh luot cho: ma benh nhan + so goi lay mau.</summary>
        public string KEY { get; set; }

        public string TDL_PATIENT_CODE { get; set; }

        public string TDL_PATIENT_NAME { get; set; }

        public string FIRST_NAME { get; set; }

        public long TDL_PATIENT_DOB { get; set; }

        public string TDL_PATIENT_ADDRESS { get; set; }

        /// <summary>So thu tu goi lay mau (V_LIS_SAMPLE.CALL_SAMPLE_ORDER).</summary>
        public long? NUM_ORDER { get; set; }

        public long? SERVICE_REQ_STT_ID { get; set; }

        public string SERVICE_REQ_STT_NAME { get; set; }

        public string INSTRUCTION_TIME_STR { get; set; }

        /// <summary>Thoi diem duoc goi, dang yyyyMMddHHmmss. 0 = chua goi lan nao.</summary>
        public long CALL_TIME { get; set; }

        /// <summary>Dang duoc goi - to mau noi bat va doc len label.</summary>
        public bool IS_CALLING { get; set; }
    }
}
