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

namespace HIS.Desktop.Plugins.ServiceExecute.ADO
{
    /// <summary>
    /// Cau hinh sinh khoa (key) anh chu ky tu khoa loginname trong bieu mau.
    /// Tuong ung 1 phan tu trong JSON luu o HIS_SERE_SERV_TEMP.GEN_SIGNATURE_BY_KEY_CFG.
    /// VD: { "LoginnameKey":"REQ_LOGINNAME", "SignatureKey":"REQ_LOGINNAME_SIGNATURE" }
    /// </summary>
    internal class GenSignatureByKeyCfgADO
    {
        internal GenSignatureByKeyCfgADO() { }

        /// <summary>Ten key chua loginname trong dicParam (vd: REQ_LOGINNAME).</summary>
        public string LoginnameKey { get; set; }

        /// <summary>Ten key se sinh ra anh chu ky trong dicImage (vd: REQ_LOGINNAME_SIGNATURE).</summary>
        public string SignatureKey { get; set; }
    }
}
