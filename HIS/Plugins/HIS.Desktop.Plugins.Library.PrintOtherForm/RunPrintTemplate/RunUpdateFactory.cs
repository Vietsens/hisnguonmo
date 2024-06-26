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
using HIS.Desktop.Plugins.Library.PrintOtherForm.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.RunPrintTemplate
{
    public class RunUpdateFactory
    {
        internal static IRunTemp RunTemplateByUpdateType(object data, UpdateType.TYPE updateType)
        {
            IRunTemp result = null;
            try
            {
                switch (updateType)
                {
                    case UpdateType.TYPE.TREATMENT:
                        result = new RunUpdateTreatmentBehavior(data);
                        break;
                    case UpdateType.TYPE.SERVICE_REQ:
                        result = new RunUpdateServiceReqBehavior(data);
                        break;
                    case UpdateType.TYPE.SERE_SERV:
                        //result = new RunUpdateSereServBehavior();
                        break;
                    default:
                        break;
                }
                if (result == null) throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
