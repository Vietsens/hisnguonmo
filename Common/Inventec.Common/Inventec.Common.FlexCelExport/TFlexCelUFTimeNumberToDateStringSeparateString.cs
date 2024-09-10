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
    /// vd: 20170416010203 -> "Ngày 16 tháng 04 năm 2017"
    /// </summary>
    class TFlexCelUFTimeNumberToDateStringSeparateString : TFlexCelUserFunction
    {
        public TFlexCelUFTimeNumberToDateStringSeparateString()
        {
        }
        public override object Evaluate(object[] parameters)
        {
            string result = System.String.Empty;
            try
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to FlFuncTimeNumberToDateStringSeparateString user-defined function");

                if (!System.String.IsNullOrEmpty(parameters[0].ToString()))
                {
                    result = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(long.Parse(parameters[0].ToString()));
                }
                if (parameters.Length > 1 && parameters[1].ToString() == "1")
                {
                    result = result.ToLower();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }

            return result;
        }
    }
}
