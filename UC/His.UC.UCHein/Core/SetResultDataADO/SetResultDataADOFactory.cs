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
using His.UC.UCHein.Set.ResetValueControl;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His.UC.UCHein.Core.SetResultDataADO
{
    class SetResultDataADOFactory
    {
        internal static ISetResultDataADO MakeISetResultDataADO(CommonParam param, UserControl data, ResultDataADO ResultDataADO)
        {
            ISetResultDataADO result = null;
            try
            {
                if (data is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    result = new SetResultDataADOBehavior(param, (data as Design.TemplateHeinBHYT1.Template__HeinBHYT1), ResultDataADO);
                }
                //else if (data.GetType() == typeof(Design.TemplateKskContract1.Template__KskContract1))
                //{
                //    result = new ResetValueControlKskBehavior(param, ((Design.TemplateKskContract1.Template__KskContract1)data));
                //}
                
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
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
