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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCellExport
{
    class TFlexCelUFRowNumber : TFlexCelUserFunction
    {
        public TFlexCelUFRowNumber()
        {
        }
        public override object Evaluate(object[] parameters)
        {
            long result = 0;
            try
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                long rownumber = Convert.ToInt64(parameters[0]);
                result = (rownumber + 1);
            }
            catch (Exception ex)
            {
                result = 0;
                LogSystem.Debug(ex);
            }

            return result;
        }
    }
}
