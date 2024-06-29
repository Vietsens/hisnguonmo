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
        public bool IsObligatoryTranferMediOrg { get; set; }
        public bool IsAcceptWordNotInData { get; set; }
        public bool IsUCCause { get; set; }
        public bool AutoCheckIcd { get; set; }

        public DelegatNextFocus DelegateNextFocus { get; set; }
        public DelegateRefeshIcd DelegateRefeshIcd { get; set; }
        public DelegateRefeshIcdMainText DelegateRefeshIcdMainText { get; set; }
        public DelegateRequiredCause DelegateRequiredCause { get; set; }
    }
}
