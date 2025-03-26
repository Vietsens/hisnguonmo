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
    public class TFlexCelUFNumberToString : TFlexCelUserFunction
    {
        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

            string result = null;
            decimal amountInput = 0;
            int numberDigit = 2;
            try
            {
                string vl1 = parameters[0].ToString();

                if (vl1.Contains("/"))
                {
                    var arrNumber = vl1.Split('/');
                    if (arrNumber != null && arrNumber.Count() > 1)
                    {
                        amountInput = Convert.ToDecimal(arrNumber[0]) / Convert.ToDecimal(arrNumber[1]);
                    }

                    if (parameters.Length > 1)
                    {
                        try
                        {
                            numberDigit = Convert.ToInt32(parameters[1]);
                        }
                        catch (Exception exx)
                        {
                            numberDigit = 2;
                            Inventec.Common.Logging.LogSystem.Error(exx);
                        }
                    }

                    result = Inventec.Common.Number.Convert.NumberToStringRoundAuto(amountInput, numberDigit);
                }
                else
                {
                    //vl1 = vl1.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    //vl1 = vl1.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    result = vl1;
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
