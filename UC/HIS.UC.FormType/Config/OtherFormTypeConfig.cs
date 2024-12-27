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

namespace HIS.UC.FormType.Config
{
    public class OtherFormTypeConfig
    {
        private static List<MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData> heinLiveAreas;
        public static List<MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData> HeinLiveAreas
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData));
                if (heinLiveAreas == null || heinLiveAreas.Count == 0)
                {
                    heinLiveAreas = MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaStore.Get();
                }
                return heinLiveAreas;
            }
            set
            {
                heinLiveAreas = value;
            }
        }

        private static List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> rightRouteTypes;
        public static List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> HeinRightRouteTypes
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData));
                if (rightRouteTypes == null || rightRouteTypes.Count == 0)
                {
                    rightRouteTypes = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeStore.Get();
                }
                return rightRouteTypes;
            }
            set
            {
                rightRouteTypes = value;
            }
        }
    }
}
