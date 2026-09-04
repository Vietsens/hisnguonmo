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
using MPS.ProcessorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000452
{
    class Mps000452ExtendSingleKey : CommonKey
    {
        internal const string DHST_LOGINNAME = "DHST_LOGINNAME";
        internal const string IMG_AVATAR = "IMG_AVATAR";
        // Số thứ tự KSK (dùng chung 3 phiếu 452/453/516) — template đặt tag {KSK_NUMBER}
        internal const string KSK_NUMBER = "KSK_NUMBER";
        // ----- Kết luận theo bệnh (ICD-10) — lấy từ HIS_KSK_GENERAL (UC dùng chung mọi tab KSK) -----
        // GENERAL.CONCLUSION_ICD_TYPE: 1=Chưa phát hiện bất thường, 2=Chẩn đoán sơ bộ, 3=Chẩn đoán xác định
        internal const string CONCLUSION_ICD_NONE_X = "CONCLUSION_ICD_NONE_X";
        internal const string CONCLUSION_ICD_PRELIM_X = "CONCLUSION_ICD_PRELIM_X";
        internal const string CONCLUSION_ICD_FINAL_X = "CONCLUSION_ICD_FINAL_X";
        internal const string CONCLUSION_ICD_CODE = "CONCLUSION_ICD_CODE";
        internal const string CONCLUSION_ICD_NAME = "CONCLUSION_ICD_NAME";
    }
}
