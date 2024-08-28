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
