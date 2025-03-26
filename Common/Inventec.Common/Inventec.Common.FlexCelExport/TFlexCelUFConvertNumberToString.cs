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
using FlexCel.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRS.MANAGER.Core.MrsReport.RDO
{
    public class TFlexCelUFConvertNumberToString : TFlexCelUserFunction
    {
        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

            string result = null;
            try
            {
                decimal CellGet = Convert.ToDecimal(parameters[0]);
                int numberDigit = 0;
                if (parameters.Count() > 1)
                {
                    numberDigit = Convert.ToInt32(parameters[1]);
                }
                int convert = 0;
                if (parameters.Count() > 2)
                {
                    convert = Convert.ToInt32(parameters[2]);
                }
                result = Inventec.Common.Number.Convert.NumberToString(CellGet, numberDigit);
                if (convert == 1)
                {
                    result = result.Replace(",", "_");
                    result = result.Replace(".", ",");
                    result = result.Replace("_", ".");
                }
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
