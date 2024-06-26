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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.MOBIFONE.Model
{
    public class HoaDon78Result
    {
        public HoaDon78ResultData data { get; set; }
        public string ok { get; set; }
        public string error { get; set; }
    }
    public class HoaDon78ResultData
    {
        public string id { get; set; }
        public string hdon_id { get; set; }
        public string cctbao_id { get; set; }
        public string hdlket_id { get; set; }
        public string tthai { get; set; }
        public long tthdon { get; set; }
        public string khieu { get; set; }
        public string shdon { get; set; }
        public string tdlap { get; set; }
        public string dvtte { get; set; }
        public long tgia { get; set; }
        public string gchu { get; set; }
        public string tnmua { get; set; }
        public string mnmua { get; set; }
        public string ten { get; set; }
        public string mst { get; set; }
        public string dchi { get; set; }
        public string email { get; set; }
        public string sdtnmua { get; set; }
        public string stknmua { get; set; }
        public string htttoan { get; set; }
        public string stknban { get; set; }
        public string sbmat { get; set; }
        public string mdvi { get; set; }
        public string nglap { get; set; }
        public DateTime nlap { get; set; }
        public string ngsua { get; set; }
        public string nsua { get; set; }
        public long tgtcthue { get; set; }
        public long tgtthue { get; set; }
        public long ttcktmai { get; set; }
        public long tgtttbso { get; set; }
        public string tgtttbchu { get; set; }
        public string dlqrcode { get; set; }
        public string sdhang { get; set; }
        public string shdon1 { get; set; }
        public string mccqthue { get; set; }
        public string ngky { get; set; }
        public string nky { get; set; }
        public string signature { get; set; }
        public string hthdbtthe { get; set; }
        public string tdlhdbtthe { get; set; }
        public string khmshdbtthe { get; set; }
        public string khhdbtthe { get; set; }
        public string shdbtthe { get; set; }
        public long tgtphi { get; set; }
        public string tgtcthue0 { get; set; }
        public string tgtthue0 { get; set; }
        public string ttcktmai0 { get; set; }
        public string tgtttbso0 { get; set; }
        public string tgtcthue5 { get; set; }
        public string tgtthue5 { get; set; }
        public string ttcktmai5 { get; set; }
        public string tgtttbso5 { get; set; }
        public string tgtcthue10 { get; set; }
        public string tgtthue10 { get; set; }
        public string ttcktmai10 { get; set; }
        public string tgtttbso10 { get; set; }
        public string tgtcthuekct { get; set; }
        public string tgtthuekct { get; set; }
        public string ttcktmaikct { get; set; }
        public string tgtttbsokct { get; set; }
        public string tgtcthuekkk { get; set; }
        public string tgtthuekkk { get; set; }
        public string ttcktmaikkk { get; set; }
        public string tgtttbsokkk { get; set; }
        public string tgtphi0 { get; set; }
        public string tgtphi5 { get; set; }
        public string tgtphi10 { get; set; }
        public string tgtphikct { get; set; }
        public string tgtphikkk { get; set; }
        public string lhdon { get; set; }
        public string lddnbo { get; set; }
        public string tnvchuyen { get; set; }
        public string ptvchuyen { get; set; }
        public string dckhoxuat { get; set; }
        public string dckhonhap { get; set; }
        public string tennguoinhanhang { get; set; }
        public string mstnguoinhanhang { get; set; }
        public string phongban { get; set; }
        public string veviec { get; set; }
        public string sohopdong { get; set; }
        public string hdon68_id_lk { get; set; }
        public string mtdiep_cqt { get; set; }
        public string mtdiep_gui { get; set; }
        public string tthdon_old { get; set; }
        public string hdon_id_old { get; set; }
        public long is_hdcma { get; set; }
        public string hdon_ghichu { get; set; }
        public long tthdon_original { get; set; }
        public long kygui_cqt { get; set; }
        public string hdktngay { get; set; }
        public string tnhban { get; set; }
        public string tnhmua { get; set; }
        public string hddckptquan { get; set; }
        public string sbke { get; set; }
        public string faxban { get; set; }
        public string webban { get; set; }
        public string sqdbants { get; set; }
        public string nqdbants { get; set; }
        public string cqqdbants { get; set; }
        public string htbants { get; set; }
        public string cmndmua { get; set; }
        public string hdvc { get; set; }
        public string hvtnxhang { get; set; }
        public string hdktso { get; set; }
        public string nbke { get; set; }
        public string ddvchden { get; set; }
        public string tgvchdtu { get; set; }
        public string tgvchdden { get; set; }
        public string sdtban { get; set; }
        public long tkcktmn { get; set; }
        public long tgtttbso_last { get; set; }
        public string mdvqhnsach_mua { get; set; }
        public string mdvqhnsach_ban { get; set; }
        public string stbao { get; set; }
        public string ntbao { get; set; }
        public string kmai { get; set; }
        public string tgtcthuek { get; set; }
        public string tgtthuek { get; set; }
        public string ttcktmaik { get; set; }
        public string tgtttbsok { get; set; }
        public string error_status { get; set; }
        public long issendmail { get; set; }
        public string docngoaitetv { get; set; }
        public string giamthuebanhang20 { get; set; }
        public string tienthuegtgtgiam { get; set; }
        public string lhdclquan { get; set; }
        public string khmshdclquan { get; set; }
        public string khhdclquan { get; set; }
        public string shdclquan { get; set; }
        public string nlhdclquan { get; set; }
        public string tgtkhac { get; set; }
        public string thdon { get; set; }
        public string msctu { get; set; }
        public string khctu { get; set; }
        public string sctu { get; set; }
        public long tcctu { get; set; }
        public string lctclquan { get; set; }
        public string qtich { get; set; }
        public long? cnctru { get; set; }
        public string ngccmnd { get; set; }
        public string nccmnd { get; set; }
        public string ktnhap { get; set; }
        public string thang { get; set; }
        public string nam { get; set; }
        public string ttncthue { get; set; }
        public string ttntthue { get; set; }
        public string sthue { get; set; }
        public string tblai { get; set; }
        public string msblai { get; set; }
        public string khblai { get; set; }
        public string sblai { get; set; }
        public string tlkthu { get; set; }


    }
}
