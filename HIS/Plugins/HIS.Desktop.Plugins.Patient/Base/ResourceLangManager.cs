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
using System.Resources;

namespace HIS.Desktop.Plugins.Patient.Base
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageUCListPatient { get; set; }
        internal static ResourceManager LanguageFrmHeinCard { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageUCListPatient = new ResourceManager("HIS.Desktop.Plugins.Patient.Resources.Lang", typeof(HIS.Desktop.Plugins.Patient.UCListPatient).Assembly);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            try
            {
                LanguageFrmHeinCard = new ResourceManager("HIS.Desktop.Plugins.Patient.Resources.Lang", typeof(HIS.Desktop.Plugins.Patient.frmHeinCard).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
