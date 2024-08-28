using FlexCel.Report;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCellExport
{
    /// <summary>
    /// Chuyển đổi ngày kiểu sô về dạng ngày tháng chuỗi
    /// vd: 20170416010203 -> "16/04/2017"
    /// </summary>
    class TFlexCelUFTimeNumberToDateString : TFlexCelUserFunction
    {
        public TFlexCelUFTimeNumberToDateString()
        {
        }
        public override object Evaluate(object[] parameters)
        {
            string result = System.String.Empty;
            try
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to FlFuncTimeNumberToDateString user-defined function");

                result = Inventec.Common.DateTime.Convert.TimeNumberToDateString(long.Parse(parameters[0].ToString()));
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }

            return result;
        }
    }
}
