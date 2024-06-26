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
using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000019.PDO
{
    public partial class Mps000019PDO : RDOBase
    {
        public Mps000019PDO() { }

        public Mps000019PDO(
            V_HIS_PATIENT patient,
            V_HIS_DEBATE currentHisDebate,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            Mps000019SingleKey _SingleKey,
            List<HIS_DEBATE_USER> _lstHisDebateUser
            )
        {
            try
            {
                this.Patient = patient;
                this.currentHisDebate = currentHisDebate;
                this.departmentTran = departmentTran;
                this.SingleKey = _SingleKey;
                this.lstHisDebateUser = _lstHisDebateUser;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000019PDO(
            V_HIS_PATIENT patient,
            V_HIS_DEBATE currentHisDebate,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            Mps000019SingleKey _SingleKey,
            V_HIS_TREATMENT _treatment,
            List<HIS_DEBATE_USER> _lstHisDebateUser
            )
        {
            try
            {
                this.Patient = patient;
                this.currentHisDebate = currentHisDebate;
                this.departmentTran = departmentTran;
                this.SingleKey = _SingleKey;
                this.treatment = _treatment;
                this.lstHisDebateUser = _lstHisDebateUser;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
