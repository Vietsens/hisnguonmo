using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DepositService.DepositService.Validtion
{
    class BankRequiredValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cboBank;
        public bool IsRequired { get; set; }
        public long? PresetBankId { get; set; }
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            try
            {
                if (!IsRequired) return true;

                if (PresetBankId.HasValue) return true;

                if (cboBank == null || cboBank.EditValue == null || string.IsNullOrWhiteSpace(cboBank.EditValue.ToString()))
                {
                    ErrorText = Base.ResourceMessageLang.TruongDuLieuBatBuoc;
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return true;
            }
        }
    }
    }
