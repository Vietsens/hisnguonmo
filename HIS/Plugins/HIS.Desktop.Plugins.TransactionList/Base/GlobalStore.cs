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

namespace HIS.Desktop.Plugins.TransactionList.Base
{
    class GlobalStore
    {
        internal static List<string> TypeFilters = new List<string>();

        internal void LoadTypeFilter()
        {
            try
            {
                TypeFilters = new List<string>();
                TypeFilters.Add(TOI_TAO);
                TypeFilters.Add(PHONG);
                TypeFilters.Add(TAT_CA);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal string TOI_TAO
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("frmTransactionList.ToiTao", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
        }
        internal string PHONG
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("frmTransactionList.Phong", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
        }
        
        internal string TAT_CA
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("frmTransactionList.TatCa", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
        }
       

        internal const short IS_TRUE = 1;
    }
}
