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
