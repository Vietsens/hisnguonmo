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
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.UCTransPati.Base
{
    public class LanguageManager
    {
        public CultureInfo Language { get; set; }

        public LanguageManager(CultureInfo Language)
        {
            this.Language = Language;
            Init(this.Language);
        }

        private static CultureInfo cultureInfo { get; set; }

        public static bool Init(CultureInfo current)
        {
            bool result = false;
            try
            {
                cultureInfo = current;
            }
            catch (Exception ex)
            {
                result = false;
                LogSystem.Error(ex);
            }
            return result;
        }

        public static CultureInfo GetCulture()
        {
            CultureInfo result = null;
            try
            {
                if (cultureInfo == null)
                {
                    cultureInfo = new CultureInfo("vi");
                }
                result = cultureInfo;
            }
            catch (Exception ex)
            {
                result = null;
                LogSystem.Error(ex);
            }
            return result;
        }
    }
}
