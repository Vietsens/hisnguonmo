using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboDistrict.Desgin.Template1
{
    public partial class Template1
    {
        internal string GetDistrictName()
        {
            string result = null;
            try
            {
                result = cboHuyen.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        internal string GetDistrictCode()
        {
            string result = null;
            try
            {
                result = txtMaHuyen.Text;
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
                result = cboHuyen.EditValue;
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
