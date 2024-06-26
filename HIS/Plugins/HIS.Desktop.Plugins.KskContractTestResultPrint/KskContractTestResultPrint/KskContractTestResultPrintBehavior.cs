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
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KskContractTestResultPrint.KskContractTestResultPrint
{
    class KskContractTestResultPrintBehavior : Tool<IDesktopToolContext>, IKskContractTestResultPrint
    {
        Inventec.Desktop.Common.Modules.Module Module;
        internal KskContractTestResultPrintBehavior()
            : base()
        {

        }

        internal KskContractTestResultPrintBehavior(Inventec.Desktop.Common.Modules.Module moduleData, CommonParam param)
            : base()
        {
            Module = moduleData;
        }

        object IKskContractTestResultPrint.Run()
        {
            object result = null;
            try
            {
                result = new frmKskContractTestResultPrint(Module);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
