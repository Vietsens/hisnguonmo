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
using Inventec.Common.Logging;
using System;
using System.Resources;

namespace HIS.Desktop.Base
{
    class ResouceManager
    {
        internal static void InitResourceLanguageManager()
        {
            try
            {
                HIS.Desktop.Resources.ResourceLanguageManager.LanguageFrmLogin = new ResourceManager("HIS.Desktop.Resources.Lang", typeof(HIS.Desktop.Modules.Login.frmLogin).Assembly);
                HIS.Desktop.Resources.ResourceLanguageManager.LanguageFrmMain = new ResourceManager("HIS.Desktop.Resources.Lang", typeof(HIS.Desktop.Modules.Main.frmMain).Assembly);

                //Inventec.UC.ListReports.Base.ResouceManager.ResourceLanguageManager();
                //Inventec.UC.ListReportType.Base.ResouceManager.ResourceLanguageManager();
                //His.UC.UCHein.Base.ResouceManager.ResourceLanguageManager();
                //HIS.UC.FormType.Base.ResouceManager.ResourceLanguageManager();
                //His.UC.CreateReport.Base.ResouceManager.ResourceLanguageManager();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
