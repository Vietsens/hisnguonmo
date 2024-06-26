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

namespace HIS.Desktop.Plugins.CallPatientDepartment.ADO
{
    public class CallPatientExamm
    {
     
        public long ID { get; set; }
        public long TYPE_ID { get; set; }
        public long SUM_WAIT_PATIENT { get; set; }
        public long SUM_DOINHG_PATIENT { get; set; }
        public long SUM_DONE_PATIENT { get; set; }
        public string STT_PATIENT { get; set; }
        public long SUM_PATIENT { get; set; }
        public string NameRoom { get; set; }
        public CallPatientExamm() { }

        public CallPatientExamm(HIS_SERVICE_REQ hisserrq)
        {
            try
            {
                if (hisserrq != null)
                {
                    this.TYPE_ID = 1;
                    this.ID = hisserrq.ID;   
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public CallPatientExamm(HIS_VACCINATION_EXAM hisvacation)
        {
            try
            {
                if (hisvacation != null)
                {
                    this.ID = hisvacation.ID;
                    this.TYPE_ID = 2;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
