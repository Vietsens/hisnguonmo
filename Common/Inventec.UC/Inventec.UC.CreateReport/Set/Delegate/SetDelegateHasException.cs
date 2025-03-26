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

namespace Inventec.UC.CreateReport.Set.Delegate
{
    class SetDelegateHasException : ISetDelegateHasException
    {
        public bool SetDelegate(System.Windows.Forms.UserControl UC, ProcessHasException hasException)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.TemplateBetweenTimeFilterOnly.TemplateBetweenTimeFilterOnly))
                {
                    Design.TemplateBetweenTimeFilterOnly.TemplateBetweenTimeFilterOnly UCCreateReport = (Design.TemplateBetweenTimeFilterOnly.TemplateBetweenTimeFilterOnly)UC;
                    result = UCCreateReport.SetDelegateHasException(hasException);                    
                }
                else if (UC.GetType() == typeof(Design.TemplateBetweenTimeFilterOnly2.TemplateBetweenTimeFilterOnly2))
                {
                    Design.TemplateBetweenTimeFilterOnly2.TemplateBetweenTimeFilterOnly2 ucCreateReport = (Design.TemplateBetweenTimeFilterOnly2.TemplateBetweenTimeFilterOnly2)UC;
                    result = ucCreateReport.SetDelegateHasException(hasException);
                }
                if (!result)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hasException), hasException));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
