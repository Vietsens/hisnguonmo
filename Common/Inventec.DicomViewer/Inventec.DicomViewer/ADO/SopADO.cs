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
    public class SopADO
    {
        public SopADO() { }

        public string AccessionNumber { get; set; }
        public string AdditionalPatientsHistory { get; set; }
        public string[] AdmittingDiagnosesDescription { get; set; }
        public string BodyPartExamined { get; set; }
        public string ContentDate { get; set; }
        public string ContentTime { get; set; }
        public int InstanceNumber { get; set; }
        public string InstitutionalDepartmentName { get; set; }
        public string InstitutionName { get; set; }
        public bool IsImage { get; set; }
        public bool IsStored { get; set; }
        public string Laterality { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturersModelName { get; set; }
        public string Modality { get; set; }
        public object NameOfPhysiciansReadingStudy { get; set; }
        public object OperatorsName { get; set; }
        public object PatientBreedCodeSequence { get; set; }
        public string PatientBreedCodeSequenceCodeMeaning { get; set; }
        public string PatientBreedCodeSequenceCodeValue { get; set; }
        public string PatientBreedCodeSequenceCodingSchemeDesignator { get; set; }
        public string PatientBreedDescription { get; set; }
        public string PatientId { get; set; }
        public string PatientPosition { get; set; }
        public string PatientsAge { get; set; }
        public string PatientsBirthDate { get; set; }
        public string PatientsBirthTime { get; set; }
        public object PatientsName { get; set; }
        public object PatientSpeciesCodeSequence { get; set; }
        public string PatientSpeciesCodeSequenceCodeMeaning { get; set; }
        public string PatientSpeciesCodeSequenceCodeValue { get; set; }
        public string PatientSpeciesCodeSequenceCodingSchemeDesignator { get; set; }
        public string PatientSpeciesDescription { get; set; }
        public string PatientsSex { get; set; }
        public object PerformingPhysiciansName { get; set; }
        public string ProtocolName { get; set; }
        public object ReferringPhysiciansName { get; set; }
        public string ResponsibleOrganization { get; set; }
        public object ResponsiblePerson { get; set; }
        public string ResponsiblePersonRole { get; set; }
        public string SeriesDate { get; set; }
        public string SeriesDescription { get; set; }
        public string SeriesInstanceUid { get; set; }
        public int SeriesNumber { get; set; }
        public string SeriesTime { get; set; }
        public string SopClassUid { get; set; }
        public string SopInstanceUid { get; set; }
        public string[] SpecificCharacterSet { get; set; }
        public string StationName { get; set; }
        public string StudyDate { get; set; }
        public string StudyDescription { get; set; }
        public string StudyId { get; set; }
        public string StudyInstanceUid { get; set; }
        public string StudyTime { get; set; }
        public string TransferSyntaxUid { get; set; }
    }
}
