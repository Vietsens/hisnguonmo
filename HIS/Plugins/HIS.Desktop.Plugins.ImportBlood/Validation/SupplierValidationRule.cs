/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImportBlood.Validation
{
    class SupplierValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cboSupplier;
        internal DevExpress.XtraEditors.LookUpEdit cboImpMestType;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboSupplier == null || cboImpMestType == null) return valid;
                if (cboImpMestType.EditValue != null)
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMestType.EditValue));
                    if (data != null && data.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC  && (cboSupplier.EditValue == null))
                    {
                        ErrorText = Base.ResourceMessageLang.TruongDuLieuBatBuoc;
                        ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                        return valid;
                    }
                }
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
