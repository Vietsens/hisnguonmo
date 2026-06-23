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

namespace HIS.UC.SecondaryIcd.ADO
{
    public class SecondaryIcdInitADO
    {
        public int Width { get; set; }
        public int TextSize { get; set; }
        public int Height { get; set; }

        public int limitDataSource { get; set; }

        public string TextLblIcd { get; set; }
        public string TextNullValue { get; set; }
        public string TootiplciIcdSubCode { get; set; }
        public DelegateNextFocus DelegateNextFocus { get; set; }
        public DelegateGetIcdMain DelegateGetIcdMain { get; set; }
        public DelegateGetIcdSub DelegateGetIcdSub { get; set; }
        public DelegateCheckICD delegateCheckICD { get; set; }
        public DelegateSetError delegateSetError { get; set; }
        public List<HIS_ICD> HisIcds { get; set; }
        public List<V_HIS_ICD> ViewHisIcds { get; set; }
        public HIS_TREATMENT hisTreatment { get; set; }
        /// <summary>
        /// Hiển thị các chẩn đoán được đánh dấu là nguyên nhân tử vong (IS_DEATH_CAUSE_ONLY = 1).
        /// Mặc định false: ẩn các chẩn đoán nguyên nhân tử vong khỏi danh sách (không liên quan chẩn đoán YHCT).
        /// </summary>
        public bool IsShowDeathCause { get; set; }
    }
}
