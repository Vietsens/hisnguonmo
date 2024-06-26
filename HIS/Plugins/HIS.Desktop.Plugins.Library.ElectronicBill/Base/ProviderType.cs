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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    public class ProviderType
    {
        public const string VNPT = "VNPT";
        public const string VIETSENS = "VIETSENS";
        public const string BKAV = "BKAV";
        public const string VIETTEL = "VIETEL";
        public const string CongThuong = "MOIT";
        public const string SoftDream = "SODR";
        public const string MISA = "MISA";
        public const string safecert = "SAFECERT";
        public const string CTO = "CTO_PROXY";
        public const string BACH_MAI = "BACH_MAI";
        public const string MOBIFONE = "MOBIFONE";
        public const string CYBERBILL = "CYBERBILL";
        //thêm đối tác cần add thêm vào type
        public static List<string> TYPE
        {
            get
            {
                return new List<string>() { VNPT, VIETSENS, BKAV, VIETTEL, CongThuong, SoftDream, MISA, safecert, CTO, BACH_MAI, MOBIFONE, CYBERBILL };
            }
        }

        //1 - 0%, 2 - 5%, 3 - 10%, 4 - khong chiu thue, 5 - khong ke khai thue, 6 - khac
        internal const int tax_0 = 1;
        internal const int tax_5 = 2;
        internal const int tax_10 = 3;
        internal const int tax_KCT = 4;
        internal const int tax_KKKT = 5;
        internal const int tax_K = 6;
    }
}
