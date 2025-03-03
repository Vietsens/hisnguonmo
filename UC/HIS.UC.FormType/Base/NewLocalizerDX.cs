using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Base
{
    public class NewLocalizerDX : Localizer
    {
        public override string GetLocalizedString(StringId id)
        {
            if (id == StringId.XtraMessageBoxYesButtonText)
                return MessageBoxManager.Yes;
            if (id == StringId.XtraMessageBoxNoButtonText)
                return MessageBoxManager.No;
            if (id == StringId.XtraMessageBoxOkButtonText)
                return MessageBoxManager.OK;
            if (id == StringId.XtraMessageBoxCancelButtonText)
                return MessageBoxManager.Cancel;
            return base.GetLocalizedString(id);
        }
    }
}
