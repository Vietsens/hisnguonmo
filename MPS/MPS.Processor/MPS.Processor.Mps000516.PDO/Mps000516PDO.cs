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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using MOS.SDO;
namespace MPS.Processor.Mps000516.PDO
{
    /// <summary>
    /// PDO phiếu Khám sức khỏe trẻ em dưới 6 tuổi (bảng HIS_KSK_UNDER_SIX).
    /// Khuôn theo Mps000453 (Under 18) nhưng GỌN hơn: không có bảng con vaccine
    /// (mục tiêm chủng là 3 cờ scalar nằm ngay trên HIS_KSK_UNDER_SIX),
    /// chỉ số sinh tồn tái sử dụng HIS_DHST qua DHST_ID.
    /// </summary>
    public partial class Mps000516PDO : RDOBase
    {
        public HIS_KSK_UNDER_SIX HisKskUnderSix { get; set; }
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        public HIS_DHST HisDhst { get; set; }
        // Kết luận & tư vấn (mục P) lưu ở HIS_KSK_GENERAL cùng SERVICE_REQ_ID (theo thiết kế DB).
        public HIS_KSK_GENERAL HisKskGeneral { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> examRank { get; set; }
        public V_HIS_TREATMENT_4 treatment { get; set; }
        /// <summary>Y lệnh KSK (entity HIS_SERVICE_REQ) — tùy chọn; processor đổ key prefix SREQ_.</summary>
        public HIS_SERVICE_REQ KskServiceReq { get; set; }
        /// <summary>Bệnh nhân (HIS_PATIENT) — tùy chọn; processor đổ key prefix PATIENT_.</summary>
        public HIS_PATIENT KskPatient { get; set; }

        public Mps000516PDO(
            HIS_KSK_UNDER_SIX HisKskUnderSix,
            V_HIS_SERVICE_REQ HisServiceReq,
            HIS_DHST HisDhst,
            HIS_KSK_GENERAL HisKskGeneral,
            List<HIS_HEALTH_EXAM_RANK> examRank,
            V_HIS_TREATMENT_4 treatment
            )
        {
            try
            {
                this.HisKskUnderSix = HisKskUnderSix;
                this.HisServiceReq = HisServiceReq;
                this.HisDhst = HisDhst;
                this.HisKskGeneral = HisKskGeneral;
                this.examRank = examRank;
                this.treatment = treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
