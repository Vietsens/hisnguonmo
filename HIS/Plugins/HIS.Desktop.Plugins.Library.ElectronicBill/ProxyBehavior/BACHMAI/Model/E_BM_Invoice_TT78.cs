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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProxyBehavior.BACHMAI.Model
{
    class E_BM_Invoice_TT78
    {
        public Tonghop_TT78 tonghop { get; set; }
        public List<Chitiet_TT78> chitiet { get; set; }
    }

    class Chitiet_TT78
    {
        public Chitiet_TT78()
        {
            this.TINHCHAT = 1;
            this.THUESUAT = "KCT";
        }

        public string ID_MASTER { get; set; }
        public int IDSTT { get; set; }
        public string SOTHUTU { get; set; }
        public string MAHANG { get; set; }
        public string TENHANG { get; set; }
        public decimal DONGIA { get; set; }
        public string THUESUAT { get; set; }
        public string DONVITINH { get; set; }
        public decimal SOLUONG { get; set; }
        public decimal THANHTIEN { get; set; }
        public decimal TIENTHUE { get; set; }
        public decimal TONGTIEN { get; set; }
        public decimal TILECKGG { get; set; }
        public decimal THANHTIENCKGG { get; set; }
        public long TINHCHAT { get; set; }
        public string THUETTDB { get; set; }
        public string EXTEND2 { get; set; }
        public string EXTEND1 { get; set; }
        public string CHITIEU1 { get; set; }
        public string CHITIEU2 { get; set; }
        public string CHITIEU3 { get; set; }
        public string GHICHUCT { get; set; }
        public int MUCBHYTTRA { get; set; }
    }

    class Tonghop_TT78
    {
        public string ID_MASTER { get; set; }
        public string SODONHANG { get; set; }
        public string NGAYDONHANG { get; set; }
        public string MAKHACHHANG { get; set; }
        public string TENKHACHHANG { get; set; }
        public string TENDONVI { get; set; }
        public string MASOTHUE { get; set; }
        public string MASOTHUENN { get; set; }
        public string DIACHI { get; set; }
        public string DIENTHOAINGUOIMUA { get; set; }
        public string FAXNGUOIMUA { get; set; }
        public string EMAILNGUOIMUA { get; set; }
        public string SOTAIKHOAN { get; set; }
        public string NOIMOTAIKHOAN { get; set; }
        public string HINHTHUCTT { get; set; }
        public long TONGTIENKCT { get; set; }
        public long TONGTIEN0 { get; set; }
        public long TONGTIENTHUE { get; set; }
        public long TONGTIENCHUAVAT5 { get; set; }
        public long TONGTIENVAT5 { get; set; }
        public long TONGTIENCHUAVAT10 { get; set; }
        public long TONGTIENVAT10 { get; set; }
        public long TONGTIENCHUAVATKHAC { get; set; }
        public long TONGTIENVATKHAC { get; set; }
        public long TONGTIENHANG { get; set; }
        public long TONGTIENCKGG { get; set; }
        public long TONGTIENTT { get; set; }
        public long TIENCHIPHIKHAC { get; set; }
        public string SOTIENBANGCHU { get; set; }
        public string ID_MASTER_REF { get; set; }
        public string LOAITIENTE { get; set; }
        public string TINHTRANGHOADON { get; set; }
        public int TINHTRANGKYNGUOIBAN { get; set; }
        public string ID_COMPANY { get; set; }
        public int TYGIA { get; set; }
        public int HIENTHINGOAITE { get; set; }
        public string TENCN_CH { get; set; }
        public string MADICHVU { get; set; }
        public string MACN_CH { get; set; }
        public long TONGTIENBHTRA { get; set; }
        public string NVTHU { get; set; }
        public string MAKHOA { get; set; }
        public string TENKHOA { get; set; }
        public string DIEMTHU { get; set; }
        public int GOPHD { get; set; }
    }
}
