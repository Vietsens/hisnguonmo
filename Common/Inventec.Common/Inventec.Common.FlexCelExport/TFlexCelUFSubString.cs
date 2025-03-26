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
    class TFlexCelUFSubString : TFlexCelUserFunction
    {
        public TFlexCelUFSubString()
        {
        }
        public override object Evaluate(object[] parameters)
        {
            string result = "";
            try
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to TFlexCelUFSubString() user-defined function");

                int length = parameters.Length;
                string value = parameters[0].ToString();
                int lengthRaw = value.Length;
                int startPosition = 0;
                int lenghtTo = 0;

                switch (length)
                {
                    case 1:
                        result = value;
                        break;
                    case 2:
                        startPosition = Convert.ToInt32(parameters[1]);
                        result = value.Substring(startPosition);
                        break;
                    case 3:
                        startPosition = Convert.ToInt32(parameters[1]);
                        lenghtTo = Convert.ToInt32(parameters[2]);

                        if (lenghtTo < lengthRaw)
                        {
                            result = value.Substring(startPosition, lenghtTo);
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                result = "";
                LogSystem.Debug(ex);
            }

            return result;
        }
    }
}
