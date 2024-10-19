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

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ADO
{
    public class DMediStock1ADO : D_HIS_MEDI_STOCK_1
    {
        public string MEDICINE_TYPE_CODE__UNSIGN { get; set; }
        public string MEDICINE_TYPE_NAME__UNSIGN { get; set; }
        public string ACTIVE_INGR_BHYT_NAME__UNSIGN { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public long? TDL_MATERIAL_MAX_REUSE_COUNT { get; set; }//Số lần sử dụng tối đa
        public long? REMAIN_REUSE_COUNT { get; set; }//Số lần sử dụng còn lại
        public long? USE_COUNT { get; set; }//Số lần sử dụng
        public string USE_COUNT_DISPLAY { get; set; }//Số lần sử dụng
        public long MATERIAL_TYPE_ID { get; set; }
        public short? IS_KIDNEY { get; set; }
        public string TDL_PACKAGE_NUMBER { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public bool? IsAssignPackage { get; set; }
        public bool? IsAllowOdd { get; set; }
        public bool? IsAllowOddAndExportOdd { get; set; }
        public long? MAME_ID { get; set; }
        public bool? IsUseOrginalUnitForPres { get; set; }
        public decimal? EXP_PRICE_DISPLAY { get; set; }
        public string HEIN_SERVICE_TYPE_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_NAME { get; set; }
        public string MATERIAL_TYPE_MAP_CODE { get; set; }
        public string MATERIAL_TYPE_MAP_NAME { get; set; }
        public short? IS_BLOCK_MAX_IN_PRESCRIPTION { get; set; }
        public decimal? ALERT_MAX_IN_DAY { get; set; }
        public short? IS_BLOCK_MAX_IN_DAY { get; set; }
        public short? IS_BLOCK_MAX_IN_TREATMENT { get; set; }
        public decimal? BREATH_SPEED { get; set; }
        public decimal? BREATH_TIME { get; set; }
        public short? IS_OXYGEN { get; set; }
        public short? IS_SPLIT_COMPENSATION { get; set; }
        public string ATC_CODES { get; set; }
        public string ATC_GROUP_CODES { get; set; }
        public string CONTRAINDICATION_IDS { get; set; }
        public string DESCRIPTION { get; set; }
        public short? IS_OUT_HOSPITAL { get; set; }
        public long? MEDICINE_GROUP_ID { get; set; }
        public string ODD_WARNING_CONTENT { get; set; }
        public bool IsExistAssignPres { get; set; }
        public long IdRow { get; set; }
    }
}
