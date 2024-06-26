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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ApplicationFont
{
    public class ApplicationFontWorker
    {
        private static RegistryKey appFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);

        public static void ChangeFontSize(float fontSize)
        {
            try
            {
                if (fontSize < ApplicationFontConfig.FontSize825)
                    fontSize = ApplicationFontConfig.FontSize825;

                if (fontSize > ApplicationFontConfig.FontSize1300)
                    fontSize = ApplicationFontConfig.FontSize1300;

                DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = new System.Drawing.Font(DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont.Name, fontSize / 100);
                DevExpress.XtraEditors.WindowsFormsSettings.DefaultMenuFont = new System.Drawing.Font(DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont.Name, fontSize / 100);
                //DevExpress.XtraEditors.WindowsFormsSettings.DefaultPrintFont = new System.Drawing.Font(DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont.Name, fontSize / 100);
                appFolder.SetValue(RegistryConstant.FONT_KEY, fontSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static float GetFontSize()
        {
            float fontSize = ApplicationFontConfig.FontSize825;
            try
            {
                var f = appFolder.GetValue(RegistryConstant.FONT_KEY, ApplicationFontConfig.FontSize825);
                fontSize = Inventec.Common.TypeConvert.Parse.ToFloat(f.ToString());
            }
            catch (Exception ex)
            {
                fontSize = ApplicationFontConfig.FontSize825;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return fontSize;
        }

        public static List<float> GetAllFontSize()
        {
            List<float> rsFonts = new List<float>();
            try
            {
                rsFonts.Add(ApplicationFontConfig.FontSize825);
                rsFonts.Add(ApplicationFontConfig.FontSize875);
                rsFonts.Add(ApplicationFontConfig.FontSize925);
                rsFonts.Add(ApplicationFontConfig.FontSize975);
                rsFonts.Add(ApplicationFontConfig.FontSize1025);
                rsFonts.Add(ApplicationFontConfig.FontSize1300);
            }
            catch (Exception ex)
            {
                rsFonts = new List<float>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rsFonts;
        }
    }
}
