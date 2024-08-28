using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.DicomViewer.ADO
{
    public class SeriesADO
    {
        /// <summary>
        /// Gets the Study Instance UID of the identified series.
        /// </summary>
        string StudyInstanceUid { get; }

        /// <summary>
        /// Gets the Series Instance UID of the identified series.
        /// </summary>
        string SeriesInstanceUid { get; }

        /// <summary>
        /// Gets the modality of the identified series.
        /// </summary>
        string Modality { get; }

        /// <summary>
        /// Gets the series description of the identified series.
        /// </summary>
        string SeriesDescription { get; }

        /// <summary>
        /// Gets the series number of the identified series.
        /// </summary>
        int SeriesNumber { get; }

        /// <summary>
        /// Gets the number of composite object instances belonging to the identified series.
        /// </summary>
        int? NumberOfSeriesRelatedInstances { get; }
    }
}
