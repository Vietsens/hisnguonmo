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
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentFinish.Save.Death
{
    class SaveDeathBehavior : SaveAbstract, ISave
    {
        internal SaveDeathBehavior(long RoomId,
            long? ServiceReqId,
            bool isSave,
            HIS_TREATMENT currentVHisTreatment,
            HisTreatmentFinishSDO hisTreatmentFinishSDO_process,
            FormTreatmentFinish Form)
            : base(RoomId, ServiceReqId, isSave, currentVHisTreatment, hisTreatmentFinishSDO_process, Form)
        { }

        object ISave.Run()
        {
            HisTreatmentFinishSDO result = null;
            try
            {
                if (this.CheckDeath())
                {
                    result = new HisTreatmentFinishSDO();

                    result.TreatmentFinishTime = this.TreatmentFinishTime;
                    result.TreatmentId = this.TreatmentId;
                    result.EndRoomId = this.RoomId;
                    result.ServiceReqId = this.ServiceReqId;
                    result.TreatmentEndTypeId = this.TreatmentEndTypeId;
                    result.TreatmentResultId = this.TreatmentResultId;
                    result.IcdCode = this.IcdCode;
                    result.IcdName = this.IcdName;
                    result.IcdSubCode = this.IcdSubCode;
                    result.IcdText = this.IcdText;
                    result.IsChronic = this.IsChronic;
                    result.IsTemporary = this.IsTemporary;
                    result.DoctorLoginname = this.DoctorLoginname;
                    result.DoctorUsernname = this.DoctorUsernname;

                    //treatment out
                    result.Advise = this.Advised;
                    result.TreatmentMethod = this.Treatment_Method;

                    result.ClinicalNote = this.ClinicalNote;
                    result.SubclinicalResult = this.Subclinical;

                    //treatment death
                    result.DeathCauseId = hisTreatmentFinishSDO_process.DeathCauseId;
                    result.DeathTime = hisTreatmentFinishSDO_process.DeathTime;
                    result.DeathWithinId = hisTreatmentFinishSDO_process.DeathWithinId;
                    result.IsHasAupopsy = hisTreatmentFinishSDO_process.IsHasAupopsy;
                    result.MainCause = hisTreatmentFinishSDO_process.MainCause;
                    result.Surgery = hisTreatmentFinishSDO_process.Surgery;
                    result.DeathCertBookId = hisTreatmentFinishSDO_process.DeathCertBookId;

                    result.DeathDocumentDate = hisTreatmentFinishSDO_process.DeathDocumentDate;
                    result.DeathDocumentNumber = hisTreatmentFinishSDO_process.DeathDocumentNumber;
                    result.DeathDocumentPlace = hisTreatmentFinishSDO_process.DeathDocumentPlace;
                    result.DeathDocumentType = hisTreatmentFinishSDO_process.DeathDocumentType;
                    result.DeathPlace = hisTreatmentFinishSDO_process.DeathPlace;
                    //
                    result.DeathCertBookFirstId = hisTreatmentFinishSDO_process.DeathCertBookFirstId;
                    result.DeathCertNumFirst = hisTreatmentFinishSDO_process.DeathCertNumFirst;
                    result.DeathCertIssuerLoginname = hisTreatmentFinishSDO_process.DeathCertIssuerLoginname;
                    result.DeathCertIssuerUsername = hisTreatmentFinishSDO_process.DeathCertIssuerUsername;
                    result.DeathDocumentTypeCode = hisTreatmentFinishSDO_process.DeathDocumentTypeCode;
                    result.DeathDocumentType = hisTreatmentFinishSDO_process.DeathDocumentType;
                    result.DeathStatus = hisTreatmentFinishSDO_process.DeathStatus;
                    result.PatientRelativeName = hisTreatmentFinishSDO_process.PatientRelativeName;
                    result.DeathIssuedDate = hisTreatmentFinishSDO_process.DeathIssuedDate;
                }
                else
                {
                    result = new HisTreatmentFinishSDO();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
