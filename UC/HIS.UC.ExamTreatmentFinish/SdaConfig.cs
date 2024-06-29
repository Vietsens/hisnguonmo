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

namespace HIS.UC.ExamTreatmentFinish
{
    public class SdaConfig
    {
        /// <summary>
        /// Thời gian hẹn khám : 1, ưu tiên thời gian hẹn khám theo hạn dùng của đơn thuốc
        /// </summary>
        public const string PRESCRIPTION_TIME_AND_APPOINTMENT_TIME_KEY = "HIS.Desktop.Plugins.TreatmentFinish.APPOINTMENT_TIME";

        /// <summary>
        /// Mặc định thời gian hẹn khám, tính theo ngày
        /// </summary>
        public const string TREATMENT_END___APPOINTMENT_TIME_DEFAULT_KEY = "EXE.HIS_TREATMENT_END.APPOINTMENT_TIME_DEFAULT";
        /// <summary>
        /// 0: Mặc định để trống
        /// 1: Hẹn khám
        /// 2: Cấp toa cho về
        /// </summary>
        public const string TREATMENT_END___TREATMENT_END_TYPE_DEFAULT = "HIS.Desktop.Plugins.TreatmentFinish.TreatmentEndTypeDefault";

        public const string TREATMENT_RESULT__TREATMENT_RESULT_CODE_DEFAULT = "MOS.HIS_TREATMENT_RESULT.TREATMENT_RESULT_CODE.DEFAULT_OF_EXAM";

        public const string AUTO_CHECK_PRINT_BORDEREAU_BY_PATIENT_TYPE = "HIS.DESKTOP.AUTO_CHECK_PRINT_BORDEREAU_BY_PATIENT_TYPE";
    }
}
