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

namespace MPS.ADO
{
    public class Mps000089ADO : V_HIS_EXP_MEST_MEDICINE
    {
        public string EXPIRED_DATE_STR { get; set; }

        public Mps000089ADO()
        { }

        public Mps000089ADO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine)
        {
            try
            {
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    this.MEDICINE_TYPE_ID = listMedicine.First().MEDICINE_TYPE_ID;
                    this.MEDICINE_TYPE_CODE = listMedicine.First().MEDICINE_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = listMedicine.First().MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = listMedicine.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMedicine.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMedicine.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMedicine.First().SUPPLIER_NAME;
                    this.NUM_ORDER = listMedicine.First().NUM_ORDER;
                    this.PACKAGE_NUMBER = listMedicine.First().PACKAGE_NUMBER;
                    this.REGISTER_NUMBER = listMedicine.First().REGISTER_NUMBER;
                    this.BID_NAME = listMedicine.First().BID_NAME;
                    this.BID_NUMBER = listMedicine.First().BID_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicine.First().EXPIRED_DATE ?? 0);
                    this.AMOUNT = listMedicine.Sum(s => s.AMOUNT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
