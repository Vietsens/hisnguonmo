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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.UCD
{
    public class InitUCD
    {
        public delegate void ProcessFormOwner(string lang);
        public delegate void ReloadLoginnameAfterLeave(string loginname);

        internal Inventec.Common.WebApiClient.ApiConsumer sdaCosumer;
        internal string APPLICATION_CODE;
        internal string SYSTEM_FOLDER;
        internal string APP_FOLDER;
        internal ProcessFormOwner processFormOwner;
        public ReloadLoginnameAfterLeave reloadLoginnameAfterLeave;
        public object Branchs;
        public long? FirstBranchId;
        public LabelString LabelString;
        public string LoginnameDefault { get; set; }

        public InitUCD(Inventec.Common.WebApiClient.ApiConsumer sdaconsumer, string AppCode, string SysFolder, string AppFolder)
            : this(sdaconsumer, AppCode, SysFolder, AppFolder, null)
        {
        }

        public InitUCD(Inventec.Common.WebApiClient.ApiConsumer sdaconsumer, string AppCode, string SysFolder, string AppFolder, ProcessFormOwner _processFormOwner)
        {
            this.sdaCosumer = sdaconsumer;
            this.APPLICATION_CODE = AppCode;
            this.SYSTEM_FOLDER = SysFolder;
            this.APP_FOLDER = AppFolder;
            this.processFormOwner = _processFormOwner;
        }
    }
}
