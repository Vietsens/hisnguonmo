﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockPeriod
{
    class LookupEditWithTextEditValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        //internal DevExpress.XtraEditors.TextEdit txtTextEdit;
        internal DevExpress.XtraEditors.GridLookUpEdit cbo;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                //if (txtTextEdit == null || cbo == null) return valid;
                //if (String.IsNullOrEmpty(txtTextEdit.Text) || cbo.EditValue == null)
                if (cbo.Text == null || string.IsNullOrWhiteSpace(cbo.Text))
                    return valid;
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
