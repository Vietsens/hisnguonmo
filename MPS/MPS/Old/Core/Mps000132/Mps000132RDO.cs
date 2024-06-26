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
using MOS.SDO;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000132
{
    /// <summary>
    /// </summary>
    public class Mps000132RDO : RDOBase
    {
        internal V_HIS_MEDI_STOCK_PERIOD mediStock { get; set; }
        internal List<V_HIS_MEST_PERIOD_METY> lstMestPeriodMety { get; set; }
        internal List<V_HIS_MEST_PERIOD_MATY> lstMestPeriodMaty { get; set; }
        internal List<Mps000132ADO> listMrs000132ADO;
        internal bool isCheckMedicine;
        internal bool isCheckMaterial;

        public Mps000132RDO(
            V_HIS_MEDI_STOCK_PERIOD mediStock,
            List<V_HIS_MEST_PERIOD_METY> lstMestPeriodMety,
            List<V_HIS_MEST_PERIOD_MATY> lstMestPeriodMaty,
            bool isCheckMedicine,
            bool isCheckMaterial
            )
        {
            try
            {
                this.mediStock = mediStock;
                this.lstMestPeriodMety = lstMestPeriodMety;
                this.lstMestPeriodMaty = lstMestPeriodMaty;
                this.isCheckMedicine = isCheckMedicine;
                this.isCheckMaterial = isCheckMaterial;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_MEDI_STOCK_PERIOD>(mediStock, keyValues, false);

                listMrs000132ADO = new List<Mps000132ADO>();
                if (lstMestPeriodMety != null && lstMestPeriodMety.Count > 0)
                {
                    listMrs000132ADO.AddRange((from r in lstMestPeriodMety select new Mps000132ADO(r)).ToList());
                }
                if (lstMestPeriodMaty != null && lstMestPeriodMaty.Count > 0)
                {
                    listMrs000132ADO.AddRange((from r in lstMestPeriodMaty select new Mps000132ADO(r)).ToList());
                }

                if (isCheckMedicine)
                {
                    keyValues.Add(new KeyValue(Mps000132ExtendSingleKey.TYPE_PRINT, "THUỐC"));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000132ExtendSingleKey.TYPE_PRINT, "VẬT TƯ Y TẾ TIÊU HAO"));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
