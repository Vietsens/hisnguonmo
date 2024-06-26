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
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Debate.Base
{
    class GlobalStore
    {
        internal static List<string> TypeFilters = new List<string>() { TOI_TAO, PHONG_CHI_DINH, HO_SO_DIEU_TRI, BENH_NHAN, TAT_CA };

        static string TOI_TAO
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("frmDebate.ToiTao", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
            }
        }
        static string PHONG_CHI_DINH
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("frmServiceReqList.PhongChiDinh", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
            }
        }
        static string HO_SO_DIEU_TRI
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("frmServiceReqList.HoSoDieuTri", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
            }
        }
        static string BENH_NHAN
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("frmServiceReqList.BenhNhan", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
            }
        }
        static string TAT_CA
        {
            get
            {
                return Inventec.Common.Resource.Get.Value("frmServiceReqList.TatCa", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
            }
        }
    }
}
