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

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne.ADO
{
    public class VHisSereServADO : V_HIS_SERE_SERV_5
    {
        public decimal? InvoicePrice { get; set; }
        public decimal? RecieptPrice { get; set; }
        public bool IsInvoiced { get; set; }
        public bool IsReciepted { get; set; }

        /// <summary>
        /// Phần bệnh nhân tự trả = tổng tiền BN phải trả - phần đồng chi trả BHYT.
        /// Gom cả phụ thu (vượt giá trần BHYT) lẫn dịch vụ thu phí 100%.
        /// Cột thông tin để đối chiếu cơ cấu chi phí, không phải số tiền cần thu.
        /// </summary>
        public decimal? DifferentPrice { get; set; }

        /// <summary>
        /// Phần đồng chi trả BHYT của bệnh nhân, lấy từ VIR_TOTAL_PATIENT_PRICE_BHYT.
        /// Chỉ tính khi BHYT thực sự có chi trả cho dịch vụ (VIR_TOTAL_HEIN_PRICE > 0);
        /// dịch vụ thu phí 100% hoặc BN không thuộc đối tượng BHYT thì bằng 0.
        /// Cột thông tin để đối chiếu cơ cấu chi phí, không phải số tiền cần thu.
        /// </summary>
        public decimal? PatientPriceBhyt { get; set; }

        public string CONCRETE_ID__IN_SETY { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public decimal? AMOUNT_PLUS { get; set; }
        public string VIR_PRICE_PLUS { get; set; }
        public decimal VAT { get; set; }
        public bool? IsExpend { get; set; }
        public bool? IsLeaf { get; set; }
        public bool? IsGuaranteed { get; set; }

        public VHisSereServADO()
        {
        }

        public VHisSereServADO(V_HIS_SERE_SERV_5 service)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<VHisSereServADO>(this, service);
            IsExpend = (service.IS_EXPEND == 1);
            this.AMOUNT_PLUS = service.AMOUNT;
            this.VAT = service.VAT_RATIO * 100;
            this.VIR_PRICE_PLUS = service.VIR_PRICE.HasValue ? Inventec.Common.Number.Convert.NumberToString(service.VIR_PRICE.Value, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator) : "";
            //this.CONCRETE_ID__IN_SETY = (service.SERVICE_TYPE_ID + "." + service.CONCRETE_ID);
            //this.PARENT_ID__IN_SETY = (service.SERVICE_TYPE_ID + "." + service.PARENT_ID);
            IsGuaranteed = (service.IS_GUARANTEED == 1);
        }

        public VHisSereServADO(V_HIS_SERE_SERV service, int patyId)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<VHisSereServADO>(this, service);
            IsExpend = (service.IS_EXPEND == 1);
            this.PARENT_ID__IN_SETY = patyId + "." + service.TDL_SERVICE_TYPE_ID;
            this.CONCRETE_ID__IN_SETY = patyId + "." + service.TDL_SERVICE_TYPE_ID + "." + service.SERVICE_ID;
            IsGuaranteed = (service.IS_GUARANTEED == 1);
        }
    }
}
