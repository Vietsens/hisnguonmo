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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000068.PDO
{
 public partial	class Mps000068PDO : RDOBase
	{
		public Mps000068PDO(
            PatientADO currentPatient,
            V_HIS_EXAM_SERVICE_REQ examServiceReq,
            V_HIS_SERE_SERV sereServ,
            HIS_DHST dhst,
            V_HIS_TRAN_PATI tranPati,
            V_HIS_TRAN_PATI tranPatiRaVien,
            V_HIS_TRAN_PATI tranPatiOut,
            HIS_TRAN_PATI_REASON tranPatiReason,
            HIS_TRAN_PATI_FORM tranPatiForm,
            TreatmentADO treatmentADO
            )
        {
            try
            {
                this.currentPatient = currentPatient;
                this.examServiceReq = examServiceReq;
                this.sereServ = sereServ;
                this.dhst = dhst;
                this.tranPati = tranPati;
                this.tranPatiRaVien = tranPatiRaVien;
                this.tranPatiOut = tranPatiOut;
                this.tranPatiReason = tranPatiReason;
                this.tranPatiForm = tranPatiForm;
                this.treatmentADO = treatmentADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
	}
}
