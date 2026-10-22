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
using HIS.Desktop.Common;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient.PublicMedicineMultiPatient
{
    /// <summary>
    /// Args from the bed room screen (HIS.Desktop.Plugins.BedRoomPartial):
    /// the module, the focused row (L_HIS_TREATMENT_BED_ROOM, pre-checked on open)
    /// and the filter currently used to load the bed room grid (HisTreatmentBedRoomLViewFilter, without paging).
    /// </summary>
    public sealed class PublicMedicineMultiPatientBehavior : Tool<IDesktopToolContext>, IPublicMedicineMultiPatient
    {
        object[] entity;

        public PublicMedicineMultiPatientBehavior()
            : base()
        {
        }

        public PublicMedicineMultiPatientBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IPublicMedicineMultiPatient.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module currentModule = null;
                L_HIS_TREATMENT_BED_ROOM focusedTreatmentBedRoom = null;
                HisTreatmentBedRoomLViewFilter treatmentBedRoomFilter = null;
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is L_HIS_TREATMENT_BED_ROOM)
                        {
                            focusedTreatmentBedRoom = (L_HIS_TREATMENT_BED_ROOM)item;
                        }
                        else if (item is HisTreatmentBedRoomLViewFilter)
                        {
                            treatmentBedRoomFilter = (HisTreatmentBedRoomLViewFilter)item;
                        }
                    }
                }
                if (currentModule != null)
                {
                    result = new frmPublicMedicineMultiPatient(currentModule, focusedTreatmentBedRoom, treatmentBedRoomFilter);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("PublicMedicineMultiPatient: thieu module trong tham so dau vao");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
