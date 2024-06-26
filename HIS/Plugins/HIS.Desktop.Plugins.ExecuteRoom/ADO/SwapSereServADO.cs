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

namespace HIS.Desktop.Plugins.ExecuteRoom.ADO
{
    class SwapSereServADO : MOS.EFMODEL.DataModels.V_HIS_SERE_SERV
    {
        public bool Selected { get; set; }

        public bool? IsExpend { get; set; }
        public bool? IsKHBHYT { get; set; }
        public bool? IsOutKtcFee { get; set; }

        public long? PackagePriceId { get; set; }
        public long? IsAllowExpend { get; set; }

        public short? IS_MULTI_REQUEST { get; set; }
        public decimal? HEIN_LIMIT_PRICE_OLD { get; set; }
        public decimal? HEIN_LIMIT_RATIO_OLD { get; set; }
        public long? HEIN_LIMIT_PRICE_IN_TIME { get; set; }
        public long? HEIN_LIMIT_PRICE_INTR_TIME { get; set; }
        public long? BILL_PATIENT_TYPE_ID { get; set; }
        public long SERVICE_TYPE_ID { get; set; }
        public long? SERVICE_NUM_ORDER { get; set; }
        public string SERVICE_CODE_HIDDEN { get; set; }
        public string SERVICE_NAME_HIDDEN { get; set; }

        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeAmount { get; set; }
        public string ErrorMessageAmount { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypePatientTypeId { get; set; }
        public string ErrorMessagePatientTypeId { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeIsAssignDay { get; set; }
        public string ErrorMessageIsAssignDay { get; set; }

    }
}
