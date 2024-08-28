using FlexCel.Report;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCellExport
{
    class TFlexCelUFSpeechNumberToString : TFlexCelUserFunction
    {
        public TFlexCelUFSpeechNumberToString()
        {
        }
        public override object Evaluate(object[] parameters)
        {
            string result = System.String.Empty;
            try
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                string uiGSep = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator;
                string uiDSep = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string vString = parameters[0].ToString();
                vString = vString.Replace(uiGSep, "");
                string temp = vString.Split(new System.String[] { uiDSep }, StringSplitOptions.None)[0];
                result = Inventec.Common.String.Convert.CurrencyToVneseString(temp);
                //decimal value = 0;
                //if (parameters[0] is string)
                //{
                //    temp = temp.Replace(uiGSep, "");
                //    value = Convert.ToDecimal(temp);
                //}
                //else
                //{
                //    value = Convert.ToDecimal(parameters[0]);
                //}
                //result = Inventec.Common.String.Convert.CurrencyToVneseString(Inventec.Common.Number.Convert.NumberToString(value).Replace(uiGSep, ""));
            }
            catch (Exception ex)
            {
                result = System.String.Empty;
                LogSystem.Debug(ex);
            }

            return result;
        }
    }
}
