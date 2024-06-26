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

namespace HIS.Desktop.Plugins.ExpMestAggrExam.ADO
{
    public class MssMedicineTypeSDO
    {
        public long TYPE_ID { get; set; }//1.Thuoc, 2.VatTu
        public decimal AMOUNT { get; set; }

        public MssMedicineTypeSDO() { }

        public MssMedicineTypeSDO(List<V_HIS_EXP_MEST_MEDICINE> datas)
        {
            try
            {
                this.TYPE_ID = 1;
                this.AMOUNT = datas.Sum(p => p.AMOUNT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MssMedicineTypeSDO(List<V_HIS_EXP_MEST_MATERIAL> datas)
        {
            try
            {
                this.TYPE_ID = 2;
                this.AMOUNT = datas.Sum(p => p.AMOUNT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
