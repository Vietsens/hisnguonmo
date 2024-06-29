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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.MetyMatyTypeInStock.ADO
{
    public class MetyMatyTypeInStockADO : MOS.EFMODEL.DataModels.D_HIS_MEDI_STOCK_1
    {
        public bool? bIsLeaf { get; set; }
        public MetyMatyTypeInStockADO()
        {
        }

        public MetyMatyTypeInStockADO(MOS.EFMODEL.DataModels.D_HIS_MEDI_STOCK_1 MetyMatyTypeInStock)
        {
            this.ACTIVE_INGR_BHYT_CODE = MetyMatyTypeInStock.ACTIVE_INGR_BHYT_CODE;
            this.ACTIVE_INGR_BHYT_NAME = MetyMatyTypeInStock.ACTIVE_INGR_BHYT_NAME;
            this.ALERT_MAX_IN_TREATMENT = MetyMatyTypeInStock.ALERT_MAX_IN_TREATMENT;
            this.ALERT_MIN_IN_STOCK = MetyMatyTypeInStock.ALERT_MIN_IN_STOCK;
            this.AMOUNT = MetyMatyTypeInStock.AMOUNT;
            this.CONCENTRA = MetyMatyTypeInStock.CONCENTRA;
            this.HEIN_SERVICE_TYPE_ID = MetyMatyTypeInStock.HEIN_SERVICE_TYPE_ID;
            this.ID = MetyMatyTypeInStock.ID;
            this.IS_ACTIVE = MetyMatyTypeInStock.IS_ACTIVE;
            this.IS_AUTO_EXPEND = MetyMatyTypeInStock.IS_AUTO_EXPEND;
            this.IS_CHEMICAL_SUBSTANCE = MetyMatyTypeInStock.IS_CHEMICAL_SUBSTANCE;
            this.IS_OUT_PARENT_FEE = MetyMatyTypeInStock.IS_OUT_PARENT_FEE;
            this.IS_STAR_MARK = MetyMatyTypeInStock.IS_STAR_MARK;
            this.MANUFACTURER_CODE = MetyMatyTypeInStock.MANUFACTURER_CODE;
            this.MANUFACTURER_ID = MetyMatyTypeInStock.MANUFACTURER_ID;
            this.MANUFACTURER_NAME = MetyMatyTypeInStock.MANUFACTURER_NAME;
            this.MEDI_STOCK_CODE = MetyMatyTypeInStock.MEDI_STOCK_CODE;
            this.MEDI_STOCK_ID = MetyMatyTypeInStock.MEDI_STOCK_ID;
            this.MEDI_STOCK_NAME = MetyMatyTypeInStock.MEDI_STOCK_NAME;
            this.MEDICINE_TYPE_CODE = MetyMatyTypeInStock.MEDICINE_TYPE_CODE;
            this.MEDICINE_TYPE_NAME = MetyMatyTypeInStock.MEDICINE_TYPE_NAME;
            this.MEDICINE_USE_FORM_ID = MetyMatyTypeInStock.MEDICINE_USE_FORM_ID;
            this.NATIONAL_NAME = MetyMatyTypeInStock.NATIONAL_NAME;
            this.SERVICE_ID = MetyMatyTypeInStock.SERVICE_ID;
            this.SERVICE_TYPE_ID = MetyMatyTypeInStock.SERVICE_TYPE_ID;
            this.SERVICE_UNIT_CODE = MetyMatyTypeInStock.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_ID = MetyMatyTypeInStock.SERVICE_UNIT_ID;
            this.SERVICE_UNIT_NAME = MetyMatyTypeInStock.SERVICE_UNIT_NAME;
            this.TUTORIAL = MetyMatyTypeInStock.TUTORIAL;
            this.USE_ON_DAY = MetyMatyTypeInStock.USE_ON_DAY;

            //Inventec.Common.Mapper.DataObjectMapper.Map<MetyMatyTypeInStockADO>(this, MetyMatyTypeInStock);
            //bIsLeaf = (MetyMatyTypeInStock.IsLeaf == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_LEAF__TRUE);

        }
    }
}
