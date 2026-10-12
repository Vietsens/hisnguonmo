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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate.ADO
{
    /// <summary>
    /// Viec 58006: 1 dong = 1 benh nhan co don chua tao phieu xuat trong ngay (gop cac don cua benh nhan)
    /// </summary>
    class PendingPatientADO
    {
        public long PATIENT_ID { get; set; }
        public string PATIENT_CODE { get; set; }
        public string PATIENT_NAME { get; set; }
        public string DOB_STR { get; set; }
        public string GENDER_NAME { get; set; }
        public int PRES_COUNT { get; set; }
        public long LAST_INTRUCTION_TIME { get; set; }

        /// <summary>
        /// Cot hien thi kiem cot loc: GridLookUpEdit 15.2 chi loc Contains tren DisplayMember
        /// -> ghep ten + ma de go duoc ca hai
        /// </summary>
        public string DISPLAY { get; set; }

        public List<V_HIS_SERVICE_REQ_11> ServiceReqs { get; set; }

        public PendingPatientADO(List<V_HIS_SERVICE_REQ_11> serviceReqs)
        {
            this.ServiceReqs = serviceReqs;
            V_HIS_SERVICE_REQ_11 last = serviceReqs.OrderByDescending(o => o.INTRUCTION_TIME).First();
            this.PATIENT_ID = last.TDL_PATIENT_ID;
            this.PATIENT_CODE = last.TDL_PATIENT_CODE;
            this.PATIENT_NAME = last.TDL_PATIENT_NAME;
            this.GENDER_NAME = last.TDL_PATIENT_GENDER_NAME;
            this.PRES_COUNT = serviceReqs.Count;
            this.LAST_INTRUCTION_TIME = last.INTRUCTION_TIME;
            this.DISPLAY = String.Format("{0} ({1})", last.TDL_PATIENT_NAME, last.TDL_PATIENT_CODE);
            try
            {
                if (last.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == (short)1)
                    this.DOB_STR = last.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                else
                    this.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(last.TDL_PATIENT_DOB);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
