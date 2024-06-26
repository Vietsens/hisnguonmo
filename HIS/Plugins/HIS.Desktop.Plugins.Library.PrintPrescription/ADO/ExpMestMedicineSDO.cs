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
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintPrescription.ADO
{
    class ExpMestMedicineSDO : MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE
    {
        public short? IS_ADDICTIVE { get; set; }//gay nghien
        //public short? IS_FUNCTIONAL_FOOD { get; set; }//tpcn
        public short? IS_NEUROLOGICAL { get; set; }//huong than
        public short? IS_RADIOACTIVE { get; set; }//phong xa
        public short? IS_POISON { get; set; }//doc
        public short? IS_TUBERCULOSIS { get; set; } //lao
        //public string MEDICINE_USE_FORM_NAME { get; set; }
        public int Type { get; set; }//1: thuoc, 2: vat tu, 3: thuoc ngoai kho, 4: vat tu ngoai kho, 5: tu tuc
        public decimal? PRES_AMOUNT { get; set; }

        public string MEDICINE_TYPE_DESCRIPTION { get; set; }
        public decimal? USING_COUNT_NUMBER { get; set; }
        public long? INTRUCTION_TIME { get; set; }

        public ExpMestMedicineSDO()
        {

        }
    }
}
