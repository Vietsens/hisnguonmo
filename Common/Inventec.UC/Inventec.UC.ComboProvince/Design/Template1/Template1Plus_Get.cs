using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Design.Template1
{
    internal partial class Template1
    {
        internal string GetProvinceName()
        {
            string result = null;
            try
            {
                result = cboTinh.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        internal string GetProvinceCode()
        {
            string result = null;
            try
            {
                result = txtMaTinh.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        internal object GetEditValueCombo()
        {
            object result = null;
            try
            {
                result = cboTinh.EditValue;
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
