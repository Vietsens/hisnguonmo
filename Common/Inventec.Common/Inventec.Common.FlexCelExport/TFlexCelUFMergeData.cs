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
    class TFlexCelUFMergeData : TFlexCelUserFunction
    {
        private string previousValues = string.Empty;

        public TFlexCelUFMergeData()
        {
        }

        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

            try
            {
                string currentValues = string.Empty;

                if (parameters.Length == 1 && parameters[0] != null)
                {
                    currentValues = parameters[0].ToString().Trim();
                }
                else if (parameters.Length > 1)
                {
                    List<string> valueList = new List<string>();
                    foreach (var param in parameters)
                    {
                        if (param != null)
                        {
                            valueList.Add(param.ToString().Trim());
                        }
                    }
                    currentValues = string.Join(";", valueList);
                }
                if (!string.IsNullOrEmpty(currentValues))
                {
                    if (this.previousValues == currentValues)
                    {
                        return true;
                    }
                    else
                    {
                        this.previousValues = currentValues;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return false;
        }
    }
}