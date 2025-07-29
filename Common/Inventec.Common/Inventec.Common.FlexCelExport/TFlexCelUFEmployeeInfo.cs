using FlexCel.Report;
using Inventec.Common.FlexCellExport;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Inventec.Common.FlexCelExport
{
    class TFlexCelUFEmployeeInfo : TFlexCelUserFunction
    {
        public TFlexCelUFEmployeeInfo()
        {
        }
        public override object Evaluate(object[] parameters)
        {
            if (Store.EmployeeInfoHandler == null)
                return null;
            string loginName = (parameters != null && parameters.Length > 0) ? Convert.ToString(parameters[0]) : null;
            string infoKey = (parameters != null && parameters.Length > 1 && !string.IsNullOrWhiteSpace(Convert.ToString(parameters[1])))
                ? Convert.ToString(parameters[1])
                : "TITLE";
            return Store.EmployeeInfoHandler(loginName, infoKey);
        }
    }
}