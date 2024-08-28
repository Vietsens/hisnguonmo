using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboEthnic.Design.Template1
{
    public partial class Template1
    {
        internal string GetEthnicCode()
        {
            string result = null;
            try
            {
                result = txtMaDanToc.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        internal string GetEthnicName()
        {
            string result = null;
            try
            {
                result = cboDanToc.Text;
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
