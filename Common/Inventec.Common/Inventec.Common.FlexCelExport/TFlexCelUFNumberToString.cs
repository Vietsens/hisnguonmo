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
