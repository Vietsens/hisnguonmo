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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportMediMatePriceList.ADO
{
    public class MediMateADO
    {
        public long ID { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public bool IsMedicine { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public bool CHECK { get; set; }
        public short? IS_LEAF { get; set; }
        public string SERVICE_NAME_HIDDEN { get; set; }
        public string SERVICE_CODE_HIDDEN { get; set; }
        public long? PARENT_ID { get; set; }
        public long? NUM_ORDER { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public short? IS_BUSINESS { get; set; }

        public V_HIS_MEDICINE_TYPE MedicineType { get; set; }
        public V_HIS_MATERIAL_TYPE MaterialType { get; set; }

        public MediMateADO(V_HIS_MEDICINE_TYPE medicine)
        {
            ID = medicine.ID;
            MEDI_MATE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
            MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
            IsMedicine = true;
            SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
            IS_LEAF = medicine.IS_LEAF;
            this.SERVICE_NAME_HIDDEN = convertToUnSign3(this.MEDI_MATE_TYPE_NAME) + this.MEDI_MATE_TYPE_NAME;
            this.SERVICE_CODE_HIDDEN = convertToUnSign3(this.MEDI_MATE_TYPE_CODE) + this.MEDI_MATE_TYPE_CODE;
            this.PARENT_ID = medicine.PARENT_ID;
            this.NUM_ORDER = medicine.NUM_ORDER;
            this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
            this.NATIONAL_NAME = medicine.NATIONAL_NAME;
            this.IS_BUSINESS = medicine.IS_BUSINESS;
            this.MedicineType = medicine;
        }

        public MediMateADO(V_HIS_MATERIAL_TYPE material)
        {
            ID = material.ID;
            MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
            MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
            IsMedicine = false;
            SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
            IS_LEAF = material.IS_LEAF;
            this.SERVICE_NAME_HIDDEN = convertToUnSign3(this.MEDI_MATE_TYPE_NAME) + this.MEDI_MATE_TYPE_NAME;
            this.SERVICE_CODE_HIDDEN = convertToUnSign3(this.MEDI_MATE_TYPE_CODE) + this.MEDI_MATE_TYPE_CODE;
            this.PARENT_ID = material.PARENT_ID;
            this.NUM_ORDER = material.NUM_ORDER;
            this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
            this.NATIONAL_NAME = material.NATIONAL_NAME;
            this.IS_BUSINESS = material.IS_BUSINESS;
            this.MaterialType = material;
        }

        public string convertToUnSign3(string s)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(s))
                    return "";

                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = s.Normalize(NormalizationForm.FormD);
                return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            }
            catch
            {

            }
            return "";
        }
    }
}
