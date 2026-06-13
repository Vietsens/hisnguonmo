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
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.Icd.ADO
{
    public class IcdInitADO
    {
        public IcdInputADO IcdInput { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float SizeText { get; set; }
        public int LabelTextSize { get; set; }
        public int MinSize { get; set; }
        public bool IsColor { get; set; }
        public string ToolTipsIcdMain { get; set; }
        public string LblIcdMain { get; set; }
        public List<HIS_ICD> DataIcds { get; set; }
        public HIS_TREATMENT hisTreatment { get; set; }
        public bool IsObligatoryTranferMediOrg { get; set; }
        public bool IsAcceptWordNotInData { get; set; }
        public bool IsUCCause { get; set; }
        public bool AutoCheckIcd { get; set; }
        public bool? IsYHCT { get; set; }
        /// <summary>
        /// Hiển thị các chẩn đoán được đánh dấu là nguyên nhân tử vong (IS_DEATH_CAUSE_ONLY = 1).
        /// Mặc định false: ẩn các chẩn đoán nguyên nhân tử vong khỏi danh sách (không liên quan chẩn đoán YHCT).
        /// </summary>
        public bool IsShowDeathCause { get; set; }
        /// <summary>
        /// Không cảnh báo khi chọn/sửa chẩn đoán "không khuyến khích dùng là bệnh chính" (IS_NOT_RECOMMEND_MAIN = 1).
        /// Mặc định false: vẫn hiển thị cảnh báo khi sửa thông tin chẩn đoán chính.
        /// </summary>
        public bool IsNotWarningNotRecommendMain { get; set; }
        public long DepamentId { get; set; }
        public Template Template { get; set; }
        public DelegatNextFocus DelegateNextFocus { get; set; }
        public DelegateRefeshIcd DelegateRefeshIcd { get; set; }
        public DelegateCheckICD delegateCheckICD { get; set; }
        public DelegateRefeshIcdMainText DelegateRefeshIcdMainText { get; set; }
        public DelegateRequiredCause DelegateRequiredCause { get; set; }
        public DelegateRefreshSubIcd DelegateRefreshSubIcd { get; set; }


    }

    public enum Template
    {
        Default,
        NoFocus
    }
}
