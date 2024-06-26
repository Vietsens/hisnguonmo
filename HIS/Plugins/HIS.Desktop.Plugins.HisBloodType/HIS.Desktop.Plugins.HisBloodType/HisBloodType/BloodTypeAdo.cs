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

namespace HIS.Desktop.Plugins.HisBloodType.HisBloodType
{
    internal class BloodTypeAdo : MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE_1
    {
        internal string AlertExpiredDate { get; set; }
        internal string CreateTime { get; set; }

        internal BloodTypeAdo(MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE_1 bloodType)
        {
            try
            {
                //this.ALERT_EXPIRED_DATE = bloodType.ALERT_EXPIRED_DATE;
                //this.APP_CREATOR = bloodType.APP_CREATOR;
                //this.APP_MODIFIER = bloodType.APP_MODIFIER;
                //this.BILL_OPTION = bloodType.BILL_OPTION;
                //this.BLOOD_GROUP_ID = bloodType.BLOOD_GROUP_ID;
                //this.BLOOD_TYPE_CODE = bloodType.BLOOD_TYPE_CODE;
                //this.BLOOD_TYPE_NAME = bloodType.BLOOD_TYPE_NAME;
                //this.BLOOD_VOLUME_ID = bloodType.BLOOD_VOLUME_ID;
                //this.COGS = bloodType.COGS;
                //this.CREATE_TIME = bloodType.CREATE_TIME;
                //this.CREATOR = bloodType.CREATOR;
                //this.ELEMENT = bloodType.ELEMENT;
                //this.ESTIMATE_DURATION = bloodType.ESTIMATE_DURATION;
                //this.GROUP_CODE = bloodType.GROUP_CODE;
                //this.HEIN_ORDER = bloodType.HEIN_ORDER;
                //this.HEIN_SERVICE_BHYT_CODE = bloodType.HEIN_SERVICE_BHYT_CODE;
                //this.HEIN_SERVICE_BHYT_NAME = bloodType.HEIN_SERVICE_BHYT_NAME;
                //this.HEIN_SERVICE_TYPE_CODE = bloodType.HEIN_SERVICE_TYPE_CODE;
                //this.HEIN_SERVICE_TYPE_ID = bloodType.HEIN_SERVICE_TYPE_ID;
                //this.HEIN_SERVICE_TYPE_NAME = bloodType.HEIN_SERVICE_TYPE_NAME;
                //this.ID = bloodType.ID;
                //this.IMP_PRICE = bloodType.IMP_PRICE;
                //this.IMP_VAT_RATIO = bloodType.IMP_VAT_RATIO;
                //this.INTERNAL_PRICE = bloodType.INTERNAL_PRICE;
                //this.IS_ACTIVE = bloodType.IS_ACTIVE;
                //this.IS_DELETE = bloodType.IS_DELETE;
                //this.IS_LEAF = bloodType.IS_LEAF;
                //this.IS_OUT_PARENT_FEE = bloodType.IS_OUT_PARENT_FEE;
                //this.MODIFIER = bloodType.MODIFIER;
                //this.MODIFY_TIME = bloodType.MODIFY_TIME;
                //this.NUM_ORDER = bloodType.NUM_ORDER;
                //this.PACKING_TYPE_CODE = bloodType.PACKING_TYPE_CODE;
                //this.PACKING_TYPE_ID = bloodType.PACKING_TYPE_ID;
                //this.PACKING_TYPE_NAME = bloodType.PACKING_TYPE_NAME;
                //this.PARENT_ID = bloodType.PARENT_ID;
                //this.SERVICE_ID = bloodType.SERVICE_ID;
                //this.SERVICE_TYPE_CODE = bloodType.SERVICE_TYPE_CODE;
                //this.SERVICE_TYPE_ID = bloodType.SERVICE_TYPE_ID;
                //this.SERVICE_TYPE_NAME = bloodType.SERVICE_TYPE_NAME;
                //this.SERVICE_UNIT_CODE = bloodType.SERVICE_UNIT_CODE;
                //this.SERVICE_UNIT_ID = bloodType.SERVICE_UNIT_ID;
                //this.SERVICE_UNIT_NAME = bloodType.SERVICE_UNIT_NAME;
                //this.SERVICE_UNIT_SYMBOL = bloodType.SERVICE_UNIT_SYMBOL;
                //this.VOLUME = bloodType.VOLUME;
                //this.AlertExpiredDate = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(bloodType.ALERT_EXPIRED_DATE ?? 0);
                //this.CreateTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(bloodType.CREATE_TIME ?? 0);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
