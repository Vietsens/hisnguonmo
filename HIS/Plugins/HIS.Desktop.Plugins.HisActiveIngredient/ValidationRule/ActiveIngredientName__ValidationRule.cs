using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisActiveIngredient.ValidationRule
{
    class ActiveIngredientName__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtHisActiveIngredientName;

        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (txtHisActiveIngredientName != null);
                if (valid)
                {
                    string strError = "";
                    string hisActiveIngredientName = txtHisActiveIngredientName.Text.Trim();
                    int? countLength = Inventec.Common.String.CountVi.Count(hisActiveIngredientName);

                    if (String.IsNullOrEmpty(hisActiveIngredientName))
                    {
                        valid = false;
                        strError = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(
                            Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    }

                    this.ErrorText = strError;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }
    }
}
