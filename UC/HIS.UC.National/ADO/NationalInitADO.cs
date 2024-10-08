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
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.National.ADO
{
    public class NationalInitADO
    {
        public EventKey eventKey { get; set; }
        public NationalInputADO NationalInput { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float SizeText { get; set; }
        public bool IsColor { get; set; }
        public string LblNationalMain { get; set; }
        public List<SDA_NATIONAL> DataNationals { get; set; }

        public bool AutoCheckNational { get; set; }

        public DelegatNextFocus DelegateNextFocus { get; set; }
        public DelegateRefeshNational DelegateRefeshNational { get; set; }
    }

    public enum EventKey
    {
        KeyUp, KeyDown, PreviewKeyDown
    }
}
