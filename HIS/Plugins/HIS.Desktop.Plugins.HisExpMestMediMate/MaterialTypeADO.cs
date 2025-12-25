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

namespace HIS.Desktop.Plugins.HisExpMestMediMate
{
    class MaterialTypeADO : MediMateBaseADO
    {
        public string MATERIAL_TYPE_CODE { get; set; }
        public long EXP_MEST_ID { get; set; }

        public MaterialTypeADO() { }

        public MaterialTypeADO(V_HIS_EXP_MEST_MATERIAL_4 data)
        {
            try
            {
                this.MATERIAL_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                this.TIME = data.EXP_TIME;
                this.MEST_ID = data.EXP_MEST_ID ?? 0;
                this.EXP_MEST_TYPE_ID = data.EXP_MEST_TYPE_ID;
                this.MEST_CODE = data.EXP_MEST_CODE;
                this.MEST_TYPE = data.EXP_MEST_TYPE_NAME;
                this.AMOUNT = data.AMOUNT;
                this.PRICE = data.PRICE;
                this.MEDI_STOCK_PERIOD_NAME = data.MEDI_STOCK_PERIOD_NAME;
                this.MEDI_STOCK_NAME = data.MEDI_STOCK_NAME;
                this.IsExp = true;

                BuildKeyWord(
                    this.EXP_MEDI_STOCK_NAME,
                    this.IMP_MEDI_STOCK_NAME,
                    this.MEDI_STOCK_NAME,
                    this.MEDI_STOCK_PERIOD_NAME,
                    this.REQ_DEPARTMENT_NAME,
                    this.STT_NAME,
                    this.MATERIAL_TYPE_CODE,
                    this.MEST_CODE,
                    this.MEST_TYPE
                );
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MaterialTypeADO(V_HIS_IMP_MEST_MATERIAL_3 data)
        {
            try
            {
                this.MATERIAL_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                this.TIME = data.IMP_TIME;
                this.MEST_ID = data.IMP_MEST_ID;
                this.IMP_MEST_TYPE_ID = data.IMP_MEST_TYPE_ID;
                this.MEST_CODE = data.IMP_MEST_CODE;
                this.MEST_TYPE = data.IMP_MEST_TYPE_NAME;
                this.AMOUNT = data.AMOUNT;
                this.PRICE = data.PRICE;
                this.MEDI_STOCK_PERIOD_NAME = data.MEDI_STOCK_PERIOD_NAME;
                this.MEDI_STOCK_NAME = data.MEDI_STOCK_NAME;
                this.IsExp = false;

                BuildKeyWord(
                    this.EXP_MEDI_STOCK_NAME,
                    this.IMP_MEDI_STOCK_NAME,
                    this.MEDI_STOCK_NAME,
                    this.MEDI_STOCK_PERIOD_NAME,
                    this.REQ_DEPARTMENT_NAME,
                    this.STT_NAME,
                    this.MATERIAL_TYPE_CODE,
                    this.MEST_CODE,
                    this.MEST_TYPE
                );
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
