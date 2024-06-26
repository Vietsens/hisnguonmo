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

namespace MPS.Processor.Mps000117.PDO
{
    public class HisAnticipateMetyADO : MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_METY
    {
        public int Type { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal IN_STOCK_AMOUNT { get; set; }

        public HisAnticipateMetyADO() { }

        public HisAnticipateMetyADO(MOS.EFMODEL.DataModels.HIS_ANTICIPATE_METY data, MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE mety)
        {
            try
            {
                if (data != null)
                {
                    this.AMOUNT = data.AMOUNT;
                    this.ANTICIPATE_ID = data.ANTICIPATE_ID;
                    this.ID = data.ID;
                    this.IMP_PRICE = data.IMP_PRICE;
                    this.IS_ACTIVE = data.IS_ACTIVE;
                    this.IS_DELETE = data.IS_DELETE;
                    this.MEDICINE_TYPE_ID = data.MEDICINE_TYPE_ID;
                    this.SUPPLIER_ID = data.SUPPLIER_ID;
                }
                if (mety != null)
                {
                    this.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
                    this.BYT_NUM_ORDER = mety.BYT_NUM_ORDER;
                    this.CONCENTRA = mety.CONCENTRA;
                    this.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;
                    this.IS_STAR_MARK = mety.IS_STAR_MARK;
                    this.MANUFACTURER_CODE = mety.MANUFACTURER_CODE;
                    this.MANUFACTURER_ID = mety.MANUFACTURER_ID;
                    this.MANUFACTURER_NAME = mety.MANUFACTURER_NAME;
                    this.MEDICINE_LINE_ID = mety.MEDICINE_LINE_ID;
                    this.MEDICINE_TYPE_CODE = mety.MEDICINE_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = mety.MEDICINE_TYPE_NAME;
                    this.MEDICINE_TYPE_PROPRIETARY_NAME = mety.MEDICINE_TYPE_PROPRIETARY_NAME;
                    this.MEDICINE_USE_FORM_CODE = mety.MEDICINE_USE_FORM_CODE;
                    this.MEDICINE_USE_FORM_ID = mety.MEDICINE_USE_FORM_ID;
                    this.MEDICINE_USE_FORM_NAME = mety.MEDICINE_USE_FORM_NAME;
                    this.NATIONAL_NAME = mety.NATIONAL_NAME;
                    this.NUM_ORDER = mety.NUM_ORDER;
                    this.PACKING_TYPE_NAME = mety.PACKING_TYPE_NAME;
                    //this.PACKING_TYPE_ID__DELETE = mety.PACKING_TYPE_ID__DELETE;
                    this.PACKING_TYPE_NAME = mety.PACKING_TYPE_NAME;
                    this.REGISTER_NUMBER = mety.REGISTER_NUMBER;
                    this.SERVICE_ID = mety.SERVICE_ID;
                    this.SERVICE_UNIT_CODE = mety.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_ID = mety.SERVICE_UNIT_ID;
                    this.SERVICE_UNIT_NAME = mety.SERVICE_UNIT_NAME;
                    this.TCY_NUM_ORDER = mety.TCY_NUM_ORDER;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public HisAnticipateMetyADO(MOS.EFMODEL.DataModels.HIS_ANTICIPATE_MATY data, MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE maty)
        {
            try
            {
                if (data != null)
                {
                    this.AMOUNT = data.AMOUNT;
                    this.ANTICIPATE_ID = data.ANTICIPATE_ID;
                    this.ID = data.ID;
                    this.IMP_PRICE = data.IMP_PRICE;
                    this.IS_ACTIVE = data.IS_ACTIVE;
                    this.IS_DELETE = data.IS_DELETE;
                    this.MEDICINE_TYPE_ID = data.ID;
                    this.SUPPLIER_ID = data.SUPPLIER_ID;
                }
                if (maty != null)
                {
                    this.CONCENTRA = maty.CONCENTRA;
                    this.MANUFACTURER_CODE = maty.MANUFACTURER_CODE;
                    this.MANUFACTURER_ID = maty.MANUFACTURER_ID;
                    this.MANUFACTURER_NAME = maty.MANUFACTURER_NAME;
                    this.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                    this.NATIONAL_NAME = maty.NATIONAL_NAME;
                    this.NUM_ORDER = maty.NUM_ORDER;
                    this.PACKING_TYPE_NAME = maty.PACKING_TYPE_NAME;
                    this.SERVICE_ID = maty.SERVICE_ID;
                    this.SERVICE_UNIT_CODE = maty.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_ID = maty.SERVICE_UNIT_ID;
                    this.SERVICE_UNIT_NAME = maty.SERVICE_UNIT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public HisAnticipateMetyADO(MOS.EFMODEL.DataModels.HIS_ANTICIPATE_BLTY data, MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE blty)
        {
            try
            {
                if (data != null)
                {
                    this.AMOUNT = data.AMOUNT;
                    this.ANTICIPATE_ID = data.ANTICIPATE_ID;
                    this.ID = data.ID;
                    this.IMP_PRICE = data.IMP_PRICE;
                    this.IS_ACTIVE = data.IS_ACTIVE;
                    this.IS_DELETE = data.IS_DELETE;
                    this.MEDICINE_TYPE_ID = data.ID;
                    this.SUPPLIER_ID = data.SUPPLIER_ID;
                }
                if (blty != null)
                {
                    this.CONCENTRA = blty.VOLUME.ToString();
                    this.MEDICINE_TYPE_CODE = blty.BLOOD_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = blty.BLOOD_TYPE_NAME;
                    this.NUM_ORDER = blty.NUM_ORDER;
                    this.PACKING_TYPE_NAME = blty.PACKING_TYPE_NAME;
                    this.SERVICE_ID = blty.SERVICE_ID;
                    this.SERVICE_UNIT_CODE = blty.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_ID = blty.SERVICE_UNIT_ID;
                    this.SERVICE_UNIT_NAME = blty.SERVICE_UNIT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
