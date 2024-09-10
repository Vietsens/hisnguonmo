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
