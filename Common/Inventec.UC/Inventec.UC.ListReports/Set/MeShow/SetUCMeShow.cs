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

namespace Inventec.UC.ListReports.Set.MeShow
{
  class SetUCMeShow : ISetUCMeShow
  {
    public void MeShow(System.Windows.Forms.UserControl UC)
    {
      try
      {
        if (UC.GetType() == typeof(Design.Template1.Template1))
        {
          Design.Template1.Template1 UCReports = (Design.Template1.Template1)UC;
          UCReports.MeShow();
        }
        else if (UC.GetType() == typeof(Design.Template2.Template2))
        {
          Design.Template2.Template2 UCReports = (Design.Template2.Template2)UC;
          UCReports.MeShow();
        }
        else if (UC.GetType() == typeof(Design.Template3.Template3))
        {
          Design.Template3.Template3 UCReports = (Design.Template3.Template3)UC;
          UCReports.MeShow();
        }
        else
        {
          Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }
}
