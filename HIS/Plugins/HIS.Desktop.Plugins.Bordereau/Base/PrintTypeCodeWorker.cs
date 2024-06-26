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

namespace HIS.Desktop.Plugins.Bordereau.Base
{
    public class PrintTypeCodeWorker
    {
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU__BHYT__MPS000120 = "Mps000120";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU__BHYT__MPS000121 = "Mps000121";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU__VIENPHI__MPS000122 = "Mps000122";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU__VIENPHI__MPS000123 = "Mps000123";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_TONG_HOP__MPS000124 = "Mps000124";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU__TONG_HOP_BHYT__MPS000125 = "Mps000125";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU__TONG_HOP_BHYT__MPS000080 = "Mps000080";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU__TONG_HOP_BHYT__MPS000126 = "Mps000126";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU__TONG_HOP_BHYT__MPS000081 = "Mps000081";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_THEO_KHOA__MPS000079 = "Mps000079";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_THEO_KHOA__MPS000127 = "Mps000127";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_THEO_KHOA__MPS000202 = "Mps000202";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_CHI_PHI_DICH_VU_KTC__MPS000128 = "Mps000128";

        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU__BHYT__HAOPHI__MPS000158 = "Mps000158";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU__BHYT__HAOPHI__MPS000159 = "Mps000159";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU__VIENPHI__HAOPHI__MPS000160 = "Mps000160";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU__VIENPHI__HAOPHI__MPS000161 = "Mps000161";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU__HAOPHI__MPS000162 = "Mps000162";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU__HAOPHI__MPS000163 = "Mps000163";

        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_VIENPHI__MPS000164 = "Mps000164";
        //internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_CHAY_THAN_NHAN_TAO__MPS000176 = "Mps000176";
        //internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_DICH_VU__MPS000180 = "Mps000180";
        //internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU_BHYT_02BV__MPS000182 = "Mps000182";
        //internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_BHYT_01BV__MPS000183 = "Mps000183";
        //internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU_THUPHI_02BV__MPS000184 = "Mps000184";
        //internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_THUPHI_01BV__MPS000185 = "Mps000185";
        //internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU_DICHVU_02BV__MPS000186 = "Mps000186";
        //internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_DICHVU_01BV__MPS000187 = "Mps000187";
        //internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU_HAOPHI_02BV__MPS000188 = "Mps000188";
        //internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_HAOPHI_01BV__MPS000189 = "Mps000189";
        //internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_IN_NHIEU_01BV__MPS000190 = "Mps000190";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_BHYT_THEO_KHOA__MPS000193 = "Mps000193";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_BHYT__MPS000194 = "Mps000194";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU_BHYT__MPS000195 = "Mps000195";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_VIEN_PHI__MPS000196 = "Mps000196";
        internal const string PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NOI_TRU_VIEN_PHI__MPS000197 = "Mps000197";


    }
}
