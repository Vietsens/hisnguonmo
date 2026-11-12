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
using System;
using System.Collections.Generic;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000524.PDO
{
    /// <summary>
    /// Giấy khám sức khỏe của người lái xe, người điều khiển xe máy chuyên dùng — Mẫu 5, Thông tư 36/2024/TT-BYT.
    /// Nguồn: bản ghi KSK lái xe (HIS_KSK_PRO_DRIVER) + HIS_KSK_GENERAL + HIS_DHST (DHST_ID của bản ghi lái xe).
    /// Processor không gọi API: plugin chuẩn bị sẵn 22 dòng tiền sử, họ tên bác sĩ, tên hạng GPLX.
    /// </summary>
    public partial class Mps000524PDO : RDOBase
    {
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }

        /// <summary>
        /// Bản ghi HIS_KSK_PRO_DRIVER. Khai báo object và processor đọc cột THEO TÊN (Mps000524Util.Col)
        /// để không phụ thuộc phiên bản MOS.EFMODEL: thiếu cột thì ô đó để trống, không hỏng cả phiếu.
        /// </summary>
        public object HisKskProDriver { get; set; }

        public HIS_KSK_GENERAL HisKskGeneral { get; set; }
        public HIS_DHST HisDhst { get; set; }
        public V_HIS_TREATMENT_4 Treatment { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> ExamRanks { get; set; }

        /// <summary>Y lệnh KSK (tùy chọn) — key SREQ_*.</summary>
        public HIS_SERVICE_REQ KskServiceReq { get; set; }
        /// <summary>Bệnh nhân (tùy chọn) — key PATIENT_*, CCCD/ngày cấp/nơi cấp, nơi ở hiện tại (HT_*).</summary>
        public HIS_PATIENT KskPatient { get; set; }

        /// <summary>Tiền sử bản thân 22 dòng, theo thứ tự mẫu (mã HIS_DISEASE_TYPE).</summary>
        public List<Mps000524HistoryADO> PersonalHistories { get; set; }

        /// <summary>LOGINNAME -> họ tên các bác sĩ có trong hồ sơ.</summary>
        public Dictionary<string, string> EmployeeNames { get; set; }

        /// <summary>Tên hạng GPLX (HIS_LICENSE_CLASS theo LICENSE_CLASS_ID).</summary>
        public string LicenseClassName { get; set; }

        public Mps000524PDO(
            V_HIS_SERVICE_REQ hisServiceReq,
            object hisKskProDriver,
            HIS_KSK_GENERAL hisKskGeneral,
            HIS_DHST hisDhst,
            V_HIS_TREATMENT_4 treatment,
            List<HIS_HEALTH_EXAM_RANK> examRanks)
        {
            try
            {
                this.HisServiceReq = hisServiceReq;
                this.HisKskProDriver = hisKskProDriver;
                this.HisKskGeneral = hisKskGeneral;
                this.HisDhst = hisDhst;
                this.Treatment = treatment;
                this.ExamRanks = examRanks;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    /// <summary>Một dòng tiền sử bản thân (HIS_PERIOD_DRIVER_DITY nối HIS_DISEASE_TYPE).</summary>
    public class Mps000524HistoryADO
    {
        public string DISEASE_TYPE_CODE { get; set; }
        public string DISEASE_TYPE_NAME { get; set; }
        /// <summary>"1" = Có, "0" = Không, null = chưa trả lời.</summary>
        public string IS_YES_NO { get; set; }
    }
}
