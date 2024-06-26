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
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProxyBehavior.BACHMAI.Model
{
    class E_BM_Invoice
    {
        [JsonProperty(Order = 1)]
        public string id_master { get; set; }

        [JsonProperty(Order = 2)]
        public string sodonhang { get; set; }

        [JsonProperty(Order = 3)]
        public string ngaydonhang { get; set; }

        [JsonProperty(Order = 4)]
        public string maKhachHang { get; set; }

        [JsonProperty(Order = 5)]
        public string tenkhachhang { get; set; }

        [JsonProperty(Order = 6)]
        public string tendonvi { get; set; }

        [JsonProperty(Order = 7)]
        public string masothue { get; set; }//mã số thuế người mua

        [JsonProperty(Order = 8)]
        public string masothuenn { get; set; }//mã số thuế đơn vị

        [JsonProperty(Order = 9)]
        public string diachi { get; set; }

        [JsonProperty(Order = 10)]
        public string dienthoainguoimua { get; set; }

        [JsonProperty(Order = 11)]
        public string faxnguoimua { get; set; }

        [JsonProperty(Order = 12)]
        public string emailnguoimua { get; set; }

        [JsonProperty(Order = 13)]
        public string sotaikhoan { get; set; }

        [JsonProperty(Order = 14)]
        public string noimotaikhoan { get; set; }

        [JsonProperty(Order = 15)]
        public string hinhthuctt { get; set; }

        [JsonProperty(Order = 16)]
        public long tongtienkct { get; set; }

        [JsonProperty(Order = 17)]
        public long tongtien0 { get; set; }

        [JsonProperty(Order = 18)]
        public long tongtienchuavat5 { get; set; }

        [JsonProperty(Order = 19)]
        public long tongtienvat5 { get; set; }

        [JsonProperty(Order = 20)]
        public long tongtienchuavat10 { get; set; }

        [JsonProperty(Order = 21)]
        public long tongtienvat10 { get; set; }

        [JsonProperty(Order = 22)]
        public long tongtienhang { get; set; }

        [JsonProperty(Order = 23)]
        public long tongtienthue { get; set; }

        [JsonProperty(Order = 24)]
        public long tongtienckgg { get; set; }

        [JsonProperty(Order = 25)]
        public long tienchiphikhac { get; set; }

        [JsonProperty(Order = 26)]
        public long tongtientt { get; set; }

        [JsonProperty(Order = 27)]
        public string sotienbangchu { get; set; }

        [JsonProperty(Order = 28)]
        public string macn_ch { get; set; }

        [JsonProperty(Order = 29)]
        public string tencn_ch { get; set; }

        [JsonProperty(Order = 30)]
        public string diachicn_ch { get; set; }

        [JsonProperty(Order = 31)]
        public string id_master_ref { get; set; }

        [JsonProperty(Order = 32)]
        public string loaitiente { get; set; }

        [JsonProperty(Order = 33)]
        public int tygia { get; set; }

        [JsonProperty(Order = 34)]
        public string ghichu { get; set; }

        [JsonProperty(Order = 35)]
        public int hienthingoaite { get; set; }

        [JsonProperty(Order = 36)]
        public string id_company { get; set; }

        /// <summary>
        /// tiền khách hàng tạm ứng (Number)
        /// </summary>
        [JsonProperty(Order = 37)]
        public long tamung { get; set; }

        /// <summary>
        /// tiền khách hàng còn phải trả (Number)
        /// </summary>
        [JsonProperty(Order = 38)]
        public long khachhangconphaitra { get; set; }

        /// <summary>
        /// tiền khách hàng nhận lại (Number)
        /// </summary>
        [JsonProperty(Order = 39)]
        public long khachhangnhanlai { get; set; }
    }
}
