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
using System;

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient.ADO
{
    /// <summary>
    /// One patient row of the patient list (L_HIS_TREATMENT_BED_ROOM) with its check state
    /// </summary>
    public class TreatmentBedRoomADO : L_HIS_TREATMENT_BED_ROOM
    {
        /// <summary>Checked by the user, kept when the find panel hides the row</summary>
        public bool IsChecked { get; set; }
        public string DOB_STR { get; set; }

        public TreatmentBedRoomADO() { }

        public TreatmentBedRoomADO(L_HIS_TREATMENT_BED_ROOM data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<TreatmentBedRoomADO>(this, data);
                    this.DOB_STR = data.TDL_PATIENT_DOB > 0 ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB) : "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
