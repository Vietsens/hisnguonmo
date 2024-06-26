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

namespace HIS.Desktop.Plugins.Library.PrintBordereau
{
    internal class PrintTypeCodeWorker
    {
        internal const string PRINT_TYPE_CODE___NGOAI_TRU_BHYT = "Mps000120";
        internal const string PRINT_TYPE_CODE___NOI_TRU_BHYT = "Mps000121";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI = "Mps000122";
        internal const string PRINT_TYPE_CODE___NOI_TRU_VIENPHI = "Mps000123";
        internal const string PRINT_TYPE_CODE___TONG_HOP = "Mps000124";
        internal const string PRINT_TYPE_CODE___TONG_HOP__NGOAI_TRU__BHYT = "Mps000125";
        internal const string PRINT_TYPE_CODE___TONG_HOP__NOI_TRU__BHYT = "Mps000126";
        internal const string PRINT_TYPE_CODE___THEO_KHOA = "Mps000127";
        internal const string PRINT_TYPE_CODE___THEO_KHOA___BHYT = "Mps000193";
        internal const string PRINT_TYPE_CODE___TRONG_GOI_KY_THUAT_CAO = "Mps000128";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU__BHYT__TPTB = "Mps000194";
        internal const string PRINT_TYPE_CODE___NOI_TRU__BHYT__TPTB = "Mps000195";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU__VIEN_PHI__TPTB = "Mps000196";
        internal const string PRINT_TYPE_CODE___NOI_TRU__VIEN_PHI__TPTB = "Mps000197";
        internal const string PRINT_TYPE_CODE___IN_GIAY_PHU_THU = "Mps000224";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU_BHYT__CHUA_THANH_TOAN = "Mps000249";
        internal const string PRINT_TYPE_CODE___NOI_TRU_BHYT__CHUA_THANH_TOAN = "Mps000250";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__CHUA_THANH_TOAN = "Mps000251";
        internal const string PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__CHUA_THANH_TOAN = "Mps000252";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU_BHYT__HAO_PHI = "Mps000158";
        internal const string PRINT_TYPE_CODE___NOI_TRU_BHYT__HAO_PHI = "Mps000159";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU_VIENPHI__HAO_PHI = "Mps000160";
        internal const string PRINT_TYPE_CODE___NOI_TRU_VIENPHI__HAO_PHI = "Mps000161";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU__HAO_PHI = "Mps000162";
        internal const string PRINT_TYPE_CODE___NOI_TRU__HAO_PHI = "Mps000163";
        internal const string PRINT_TYPE_CODE___TONG_HOP_NGOAI_TRU__HAO_PHI = "Mps000260";
        internal const string PRINT_TYPE_CODE___TONG_HOP_NOI_TRU__HAO_PHI = "Mps000261";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI_100__CHUA_THANH_TOAN = "Mps000265";
        internal const string PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI_100__CHUA_THANH_TOAN = "Mps000266";

        internal const string PRINT_TYPE_CODE___NGOAI_TRU_BHYT__6556_QĐ_BYT = "Mps000279";
        internal const string PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT = "Mps000280";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__6556_QĐ_BYT = "Mps000281";
        internal const string PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__6556_QĐ_BYT = "Mps000282";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU_THUOC_VATTU_CHUONG_TRINH__6556_QĐ_BYT = "Mps000285";
        internal const string PRINT_TYPE_CODE___NOI_TRU_THUOC_VATTU_CHUONG_TRINH__6556_QĐ_BYT = "Mps000286";
        internal const string PRINT_TYPE_CODE___BANG_KE_VIEN_PHI_TONG_HOP = "Mps000295";
        internal const string PRINT_TYPE_CODE___BANG_KE_6556_TONG_HOP = "Mps000302";

        internal const string PRINT_TYPE_CODE___NGOAI_TRU_BHYT__6556_QĐ_BYT__THEO_KHOA = "Mps000304";
        internal const string PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT__THEO_KHOA = "Mps000305";
        internal const string PRINT_TYPE_CODE___NGOAI_TRU_VIEN_PHI__6556_QĐ_BYT__THEO_KHOA = "Mps000306";
        internal const string PRINT_TYPE_CODE___NOI_TRU_VIEN_PHI__6556_QĐ_BYT__THEO_KHOA = "Mps000307";

        internal const string PRINT_TYPE_CODE___TONG_HOP_GOI = "Mps000312";
        internal const string PRINT_TYPE_CODE___BANG_KE_6556_TONG_HOP_GOI = "Mps000313";
        internal const string PRINT_TYPE_CODE___TONG_HOP_CCT = "Mps000314";

        internal const string PRINT_TYPE_CODE___TONG_HOP_6556__THEO_KHOA = "Mps000321";
        internal const string PRINT_TYPE_CODE___NOI_TRU_BHYT__6556_QĐ_BYT_STENT_2 = "Mps000348";

        internal const string PRINT_TYPE_CODE___NGOAI_TRU__HAO_PHI_THEO_KHOA__6556_QĐ_BYT = "Mps000356";
        internal const string PRINT_TYPE_CODE___NOI_TRU__HAO_PHI_THEO_KHOA__6556_QĐ_BYT = "Mps000357";

        internal const string PRINT_TYPE_CODE___BANG_KE_DOI_TUONG_KHAC = "Mps000359";

        internal const string PRINT_TYPE_CODE___TONG_HOP_6556__THEO_KHOA_PHONG_THANH_TOAN = "Mps000441";

        internal const string PRINT_TYPE_CODE___YEU_CAU_THANH_TOAN = "Mps000446";

        internal const string PRINT_TYPE_CODE___BANG_KE_6556_THEO_LOAI_DICH_VU = "Mps000463";
    }
}
