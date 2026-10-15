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
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.HisTrackingList.TrackingView
{
    /// <summary>
    /// Data of one treatment used to build the treatment sheet print (Mps000062).
    /// Shared by the tracking list form and the read-only tracking list view.
    /// </summary>
    internal class TrackingPrintData
    {
        internal HIS_TREATMENT Treatment { get; set; }
        internal List<V_HIS_TREATMENT_BED_ROOM> TreatmentBedRooms { get; set; }
        internal Dictionary<long, HIS_SERVICE_REQ> DicServiceReqs { get; set; }
        internal List<HIS_SERE_SERV> SereServs { get; set; }
        internal Dictionary<long, List<HIS_SERE_SERV>> DicSereServs { get; set; }
        internal Dictionary<long, HIS_EXP_MEST> DicExpMests { get; set; }
        internal List<HIS_EXP_MEST_MEDICINE> ExpMestMedicines { get; set; }
        internal Dictionary<long, List<HIS_EXP_MEST_MEDICINE>> DicExpMestMedicines { get; set; }
        internal List<HIS_EXP_MEST_MATERIAL> ExpMestMaterials { get; set; }
        internal Dictionary<long, List<HIS_EXP_MEST_MATERIAL>> DicExpMestMaterials { get; set; }
        internal Dictionary<long, List<HIS_SERVICE_REQ_METY>> DicServiceReqMetys { get; set; }
        internal Dictionary<long, List<HIS_SERVICE_REQ_MATY>> DicServiceReqMatys { get; set; }
        internal List<HIS_SERE_SERV_EXT> SereServExts { get; set; }
        internal List<V_HIS_SERE_SERV_RATION> SereServRations { get; set; }
        internal List<V_HIS_IMP_MEST_2> MobaImpMests { get; set; }
        internal List<V_HIS_IMP_MEST_MEDICINE> ImpMestMedicinesTL { get; set; }
        internal List<V_HIS_IMP_MEST_MATERIAL> ImpMestMaterialsTL { get; set; }
        internal List<V_HIS_IMP_MEST_BLOOD> ImpMestBloodsTL { get; set; }
        internal List<HIS_CARE> Cares { get; set; }
        internal List<V_HIS_CARE_DETAIL> CareDetails { get; set; }
        internal List<HIS_DHST> Dhsts { get; set; }
        internal List<V_HIS_EXP_MEST_BLTY_REQ_2> ExpMestBltyReq2s { get; set; }

        /// <summary>
        /// Original service requests, used to restore DicServiceReqs before each render
        /// because the Mps000062 processor modifies the service request objects.
        /// Null means the caller keeps DicServiceReqs itself.
        /// </summary>
        internal List<HIS_SERVICE_REQ> ServiceReqSnapshot { get; set; }

        /// <summary>
        /// Data that does not depend on the rendered trackings. When IsSharedDataLoaded is true they are
        /// fetched once and reused for every rendered sheet, otherwise they are fetched on each render.
        /// </summary>
        internal bool IsSharedDataLoaded { get; set; }
        internal List<V_HIS_BED_LOG> BedLogs { get; set; }
        internal List<V_HIS_SPECIALIST_EXAM> SpecialistExams { get; set; }
        internal HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
    }
}
