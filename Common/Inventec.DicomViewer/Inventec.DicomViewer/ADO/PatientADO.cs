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

namespace Inventec.DicomViewer.ADO
{
    public class PatientADO
    {
        string PatientId { get; }
        string PatientsName { get; }
        string PatientsBirthDate { get; }
        string PatientsBirthTime { get; }
        string PatientsSex { get; }

        #region Species

        string PatientSpeciesDescription { get; }
        string PatientSpeciesCodeSequenceCodingSchemeDesignator { get; }
        string PatientSpeciesCodeSequenceCodeValue { get; }
        string PatientSpeciesCodeSequenceCodeMeaning { get; }

        #endregion

        #region Breed

        string PatientBreedDescription { get; }
        string PatientBreedCodeSequenceCodingSchemeDesignator { get; }
        string PatientBreedCodeSequenceCodeValue { get; }
        string PatientBreedCodeSequenceCodeMeaning { get; }

        #endregion

        #region Responsible Person/Organization

        string ResponsiblePerson { get; }
        string ResponsiblePersonRole { get; }
        string ResponsibleOrganization { get; }

        #endregion
    }
}
