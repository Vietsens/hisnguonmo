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
using System.Windows.Forms;

namespace Inventec.UC.Development.Init
{
    class Init : IInit
    {
        public System.Windows.Forms.UserControl InitUC(MainDevelopment.EmumTemp enumT, string contentDonVi, string contentPhatTrien)
        {
            UserControl result = null;
            try
            {
                if (enumT == MainDevelopment.EmumTemp.TEMPLATE1)
                {
                    result = new Design.Template1.Template1(contentDonVi, contentPhatTrien);
                }

                if (result == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Content truyen vao luc Khoi tao UserControl Phat trien san pham: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => contentDonVi), contentDonVi));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => contentPhatTrien), contentPhatTrien));
                }
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
