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
using Inventec.Core;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Linq;
using MOS.LibraryHein.Bhyt.HeinLevel;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.LocalStorage.HisConfig
{
    public class HisHeinLevelCFG
    {
        private static string heinLevelCodeCurrent;
        public static string HEIN_LEVEL_CODE__CURRENT
        {
            get
            {
                //if (String.IsNullOrEmpty(heinLevelCodeCurrent))
                //{
                try
                {
                    heinLevelCodeCurrent = BranchCFG.Branch.HEIN_LEVEL_CODE;
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                }
                //}
                return heinLevelCodeCurrent;
            }
            set
            {
                heinLevelCodeCurrent = value;
            }
        }
    }
}
