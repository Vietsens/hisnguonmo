using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Design.Template1
{
    public partial class Template1
    {
        internal string GetNationalCode()
        {
            string result = null;
            try
            {
                result = txtMaQuocTich.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        internal string GetNationalName()
        {
            string result = null;
            try
            {
                result = cboQuocTich.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
