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

namespace HIS.Desktop.Plugins.Register.Config
{
    class GateAndStepCFG
    {
        private static string value = System.Configuration.ConfigurationSettings.AppSettings["HIS.Desktop.Plugins.Register.GateAndStep"];

        private static string gate_Number = null;
        public static string GateNumber
        {
            get
            {
                if (gate_Number == null)
                {
                    gate_Number = GetValue(0);
                }
                return gate_Number;
            }
            set { gate_Number = value; }
        }

        private static string step_Number = null;
        public static string StepNumber
        {
            get
            {
                if (step_Number == null)
                {
                    step_Number = GetValue(1);
                }
                return step_Number;
            }
            set { step_Number = value; }
        }

        private static string GetValue(int index)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    var arr = value.Split(':');
                    if (arr != null && arr.Length > 1)
                    {
                        result = arr[index];
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
    }
}
