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
    /// Chuyển đổi số thập phân về dạng chuỗi phân số
    /// vd: 0.125 -> "1/8"
    /// </summary>
    class TFlexCelUFDecToFrac : TFlexCelUserFunction
    {
        public TFlexCelUFDecToFrac()
        {
        }
        public override object Evaluate(object[] parameters)
        {
            string result = System.String.Empty;
            try
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to FuncCalculatePatientAge user-defined function");

                result = Inventec.Common.Number.Convert.Dec2frac((decimal)(parameters[0]));               
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }

            return result;
        }
    }
}
