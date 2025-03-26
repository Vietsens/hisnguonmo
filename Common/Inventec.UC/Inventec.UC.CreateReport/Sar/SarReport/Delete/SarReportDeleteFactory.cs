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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Sar.SarReport.Delete
{
    class SarReportDeleteFactory : Inventec.UC.CreateReport.Base.GetBase
    {
        internal SarReportDeleteFactory()
            : base()
        {

        }

        internal SarReportDeleteFactory(CommonParam paramFactory)
            : base(paramFactory)
        {

        }

        internal SarReportDelete createFactory(SAR.EFMODEL.DataModels.SAR_REPORT Data)
        {
            try
            {
                SarReportDelete deleteConcrete = new SarReportDelete(param);
                deleteConcrete.Behavior = new SarReportDeleteBehaviorDefault(param, Data);
                return deleteConcrete;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
