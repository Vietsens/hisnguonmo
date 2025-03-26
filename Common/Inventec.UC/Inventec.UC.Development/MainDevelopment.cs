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
using Inventec.UC.Development.Init;
using Inventec.UC.Development.Set.SetImageForPicture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Development
{
    public partial class MainDevelopment
    {
        public enum EmumTemp
        {
            TEMPLATE1
        }

        public UserControl Init(EmumTemp enumT, string contentDonVi, string contentPhatTrien)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitUC(enumT, contentDonVi, contentPhatTrien);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public void SetImage(UserControl UC, string pathfile)
        {
            try
            {
                SetImageForPictureFactory.MakeISetImageForPicture().SetImage(UC, pathfile);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
