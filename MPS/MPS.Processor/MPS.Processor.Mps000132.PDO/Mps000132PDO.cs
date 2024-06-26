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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000132.PDO
{
    /// <summary>
    /// </summary>
    public partial class Mps000132PDO : RDOBase
    {
        public List<RoleADO> roleAdo = new List<RoleADO>();
        public bool IsThuoc { get; set; }
        public bool IsVatTu { get; set; }
        public bool IsMau { get; set; }

        public Mps000132PDO() { }
        public Mps000132PDO(
            V_HIS_MEDI_STOCK_PERIOD mediStock,
            List<V_HIS_MEST_PERIOD_METY> _mestPeriodMety,
            List<V_HIS_MEST_PERIOD_MATY> _mestPeriodMaty,
            List<V_HIS_MEST_PERIOD_BLTY> _mestPeriodBlty,
            List<V_HIS_MEST_INVE_USER> listMestInveUser,
            bool isThuoc,
            bool isVatTu,
            bool isMau
            )
        {
            try
            {
                this.mediStock = mediStock;
                this.lstMestPeriodMety = _mestPeriodMety;
                this.lstMestPeriodMaty = _mestPeriodMaty;
                this.lstMestPeriodBlty = _mestPeriodBlty;
                this.listMestInveUser = listMestInveUser;
                this.IsThuoc = isThuoc;
                this.IsVatTu = isVatTu;
                this.IsMau = isMau;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000132PDO(
           V_HIS_MEDI_STOCK_PERIOD mediStock,
           List<V_HIS_MEST_INVE_USER> listMestInveUser,
            List<V_HIS_MEST_PERIOD_MEDI> _mestPeriodMediList,
            List<V_HIS_MEST_PERIOD_MATE> _mestPeriodMateList,
            List<HIS_MEDICINE_GROUP> _medicineGroupList,
            List<long> _medistockIdList,
            List<HIS_MEDI_STOCK_METY> _medistockMety,
            List<HIS_MEDI_STOCK_MATY> _medistockMaty
           )
        {
            try
            {
                this.mediStock = mediStock;
                this.listMestInveUser = listMestInveUser;
                this.lstMestPeriodMedi = _mestPeriodMediList;
                this.lstMestPeriodMate = _mestPeriodMateList;
                this.lstMedicinGroup = _medicineGroupList;
                this.medistockIdList = _medistockIdList;
                this.ListMediStockMety = _medistockMety;
                this.ListMediStockMaty = _medistockMaty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
