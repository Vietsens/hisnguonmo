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
namespace MPS.Processor.Mps000455.PDO
{
    public class Mps000455PDO : RDOBase
    {
         public HIS_KSK_DRIVER_CAR HisKskDriverCar { get; set; }
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> examRank { get; set; }
        /// <summary>Y lệnh KSK (entity HIS_SERVICE_REQ) — tùy chọn; processor đổ key prefix SREQ_.</summary>
        public HIS_SERVICE_REQ KskServiceReq { get; set; }
        /// <summary>Bệnh nhân (HIS_PATIENT) — tùy chọn; processor đổ key prefix PATIENT_.</summary>
        public HIS_PATIENT KskPatient { get; set; }
        /// <summary>
        /// Kết luận theo bệnh (ICD-10) của lượt khám — lưu ở HIS_KSK_GENERAL cùng SERVICE_REQ_ID
        /// (UC "Kết luận theo bệnh ICD-10" dùng chung cho mọi tab KSK).
        /// Tùy chọn; processor đổ key CONCLUSION_ICD_* + object tag {KskGeneral.x}.
        /// </summary>
        public HIS_KSK_GENERAL HisKskGeneral { get; set; }
        public Mps000455PDO(
            HIS_KSK_DRIVER_CAR HisKskDriverCar,
           V_HIS_SERVICE_REQ HisServiceReq,
            List<HIS_HEALTH_EXAM_RANK> examRank
            )
        {
            try
            {
                this.HisKskDriverCar = HisKskDriverCar;
                this.HisServiceReq = HisServiceReq;
                this.examRank = examRank;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
