using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Design.Template1
{
    public partial class Template1
    {
        internal string GetCommuneName()
        {
            string result = null;
            try
            {
                result = cboPhuongXa.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        internal string GetCommuneCode()
        {
            string result = null;
            try
            {
                result = txtMaPhuongXa.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        internal object GetEditValue()
        {
            object result = null;
            try
            {
                result = cboPhuongXa.EditValue;
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
