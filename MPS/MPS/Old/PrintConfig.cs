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
using SAR.EFMODEL.DataModels;
using System.Collections.Generic;

namespace MPS
{
    public class PrintConfig
    {

        /// <summary>
        /// Ung dung co trach nhiem load data tu sda & set vao day (thuc hien 1 lan khi dang nhap)
        /// </summary>
        private static List<SAR_PRINT_TYPE> printTypes = new List<SAR_PRINT_TYPE>();
        public static List<SAR_PRINT_TYPE> PrintTypes
        {

            get
            {
                return printTypes;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    printTypes = value;               
                }
                else
                {
                    printTypes = new List<SAR_PRINT_TYPE>();
                }
            }
        }


        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> medicineTypes;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> VHisMedicineTypes
        {
            get
            {
                return medicineTypes;
            }
            set
            {
                medicineTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> materialTypes;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> VHisMaterialTypes
        {
            get
            {
                return materialTypes;
            }
            set
            {
                materialTypes = value;
            }
        }
        
        private static List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM> treatmentBedRooms;
        public static List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM> VHisTreatmentBedRooms
        {
            get
            {
                return treatmentBedRooms;
            }
            set
            {
                treatmentBedRooms = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_ROOM> vRooms;
        public static List<MOS.EFMODEL.DataModels.V_HIS_ROOM> HisVRooms
        {
            get
            {
                return vRooms;
            }
            set
            {
                vRooms = value;
            }
        }
    }
}
