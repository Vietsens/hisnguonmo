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
