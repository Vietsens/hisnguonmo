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
    /// Tuong duong MPS.ProcessorBase.Core.GenSignatureImageKeyADO.
    /// 1 phan tu trong JSON luu o HIS_SERE_SERV_TEMP.GEN_SIGNATURE_BY_KEY_CFG.
    /// VD: { "LoginnameKey":"REQUEST_LOGINNAME", "SignatureKey":"REQUEST_LOGINNAME_SIGNATURE" }
    /// </summary>
    internal class GenSignatureImageKeyADO
    {
        internal GenSignatureImageKeyADO() { }
        public string LoginnameKey { get; set; }
        public string SignatureKey { get; set; }
    }
}
