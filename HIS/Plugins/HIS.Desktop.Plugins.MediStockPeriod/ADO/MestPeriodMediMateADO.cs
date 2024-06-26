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

namespace HIS.Desktop.Plugins.MediStockPeriod.ADO
{
    public class MestPeriodMediMateADO : MOS.EFMODEL.DataModels.V_HIS_MEST_PERIOD_MEDI
    {
        public string METY_MATY_CODE { get; set; }
        public string METY_MATY_NAME { get; set; }
        public string IS_MEDICINE { get; set; }
        public string AMOUNT_STR { get; set; }
        //public decimal AMOUNT { get; set; }
        public string EXPIRED_DATE_STR { get; set; }

        public MestPeriodMediMateADO()
        {
        }

        public MestPeriodMediMateADO(MOS.EFMODEL.DataModels.V_HIS_MEST_PERIOD_MEDI item)
        {
            try
            {
                if (item != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MestPeriodMediMateADO>(this, item);
                    this.METY_MATY_CODE = item.MEDICINE_TYPE_CODE;
                    this.METY_MATY_NAME = item.MEDICINE_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MestPeriodMediMateADO(MOS.EFMODEL.DataModels.V_HIS_MEST_PERIOD_MATE item)
        {
            try
            {
                if (item != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MestPeriodMediMateADO>(this, item);
                    this.METY_MATY_CODE = item.MATERIAL_TYPE_CODE;
                    this.METY_MATY_NAME = item.MATERIAL_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
