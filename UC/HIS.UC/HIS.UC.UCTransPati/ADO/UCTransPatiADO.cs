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

namespace HIS.UC.UCTransPati.ADO
{
    public class UCTransPatiADO
    {
        public UCTransPatiADO() { }

        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
        public string ICD_TEXT { get; set; }
        public string ICD_SUB_CODE { get; set; }
        public string ICD_SUB_NAME { get; set; }
        public string NOICHUYENDEN_CODE { get; set; }
        public string NOICHUYENDEN_NAME { get; set; }
        public string SOCHUYENVIEN { get; set; }
        public string RIGHT_ROUTER_TYPE { get; set; }
        public long? HINHTHUCHUYEN_ID { get; set; }
        public long? LYDOCHUYEN_ID { get; set; }
        public long? TRANSFER_IN_CMKT { get; set; }
        public bool IsHasDialogText { get; set; }
        public bool IsDisablelblEditICD { get; set; }
        public long? TRANSFER_IN_TIME_FROM { get; set; }
        public long? TRANSFER_IN_TIME_TO { get; set; }
        public short? TRANSFER_IN_REVIEWS { get; set; }
        public byte[] ImgTransferInData { get; set; }
    }
}
