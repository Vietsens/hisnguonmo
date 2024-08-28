using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.DicomViewer.ADO
{    
    public class StudyADO
    {
        /// <summary>
        /// Gets the Study Instance UID of the identified study.
        /// </summary>
        string StudyInstanceUid { get; }

        /// <summary>
        /// Gets the sop classes in the identified study.
        /// </summary>
        string[] SopClassesInStudy { get; }

        /// <summary>
        /// Gets the modalities in the identified study.
        /// </summary>
        string[] ModalitiesInStudy { get; }

        /// <summary>
        /// Gets the study description of the identified study.
        /// </summary>
        string StudyDescription { get; }

        /// <summary>
        /// Gets the study ID of the identified study.
        /// </summary>
        string StudyId { get; }

        /// <summary>
        /// Gets the study date of the identified study.
        /// </summary>
        string StudyDate { get; }

        /// <summary>
        /// Gets the study time of the identified study.
        /// </summary>
        string StudyTime { get; }

        /// <summary>
        /// Gets the accession number of the identified study.
        /// </summary>
        string AccessionNumber { get; }

        /// <summary>
        /// Gets the referring physician's name for the study.
        /// </summary>
        string ReferringPhysiciansName { get; }

        /// <summary>
        /// Gets the Patient's Age at the time of the study.
        /// </summary>
        string PatientsAge { get; }

        /// <summary>
        /// Gets the number of series belonging to the identified study.
        /// </summary>
        int? NumberOfStudyRelatedSeries { get; }

        /// <summary>
        /// Gets the number of composite object instances belonging to the identified study.
        /// </summary>
        int? NumberOfStudyRelatedInstances { get; }
    }
}
