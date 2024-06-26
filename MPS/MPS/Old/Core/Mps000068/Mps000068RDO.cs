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
using MPS.ADO;

namespace MPS.Core.Mps000068
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000068RDO : RDOBase
    {
        internal PatientADO currentPatient { get; set; }
        internal V_HIS_EXAM_SERVICE_REQ examServiceReq { get; set; }
        internal V_HIS_SERE_SERV sereServ { get; set; }
        internal HIS_DHST dhst { get; set; }
        internal V_HIS_TRAN_PATI tranPati { get; set; }
        internal V_HIS_TRAN_PATI tranPatiRaVien { get; set; }
        internal V_HIS_TRAN_PATI tranPatiOut { get; set; }
        internal HIS_TRAN_PATI_REASON tranPatiReason { get; set; }
        internal HIS_TRAN_PATI_FORM tranPatiForm { get; set; }
        internal TreatmentADO treatmentADO { get; set; }

        public Mps000068RDO(
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

        internal override void SetSingleKey()
        {
            try
            {
                //keyValues.Add((new KeyValue(Mps000068ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ExamServiceReqs.INTRUCTION_TIME))));
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ>(examServiceReq, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(sereServ, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<HIS_DHST>(dhst, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(tranPati, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(tranPatiRaVien, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(tranPatiOut, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<HIS_TRAN_PATI_REASON>(tranPatiReason, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<HIS_TRAN_PATI_FORM>(tranPatiForm, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(treatmentADO, keyValues,false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(currentPatient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
