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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisKskDriverCreate.Validation
{
    class ValidateConcentration : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.SpinEdit spConcentration;
        internal DevExpress.XtraEditors.CheckEdit chkMgKhi;
        internal DevExpress.XtraEditors.CheckEdit chkMgMau;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (spConcentration.EditValue == null && spConcentration.Value <= 0)
                {
                    this.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                    return valid;
                }

                if (spConcentration == null || chkMgKhi == null || chkMgMau == null) return valid;

                if ((spConcentration.EditValue != null && spConcentration.Value >= 0 && (chkMgKhi.Checked || chkMgMau.Checked))
                    || ((spConcentration.EditValue == null && spConcentration.Value <= 0) && !chkMgKhi.Checked && !chkMgKhi.Checked))
                    return true;

                if (spConcentration.EditValue != null && spConcentration.Value >= 0 && (!chkMgKhi.Checked && !chkMgMau.Checked))
                    this.ErrorText = Resources.ResourceMessage.ChuaChonThongTinDonViTinh;

                if ((spConcentration.EditValue == null && spConcentration.Value < 0) && (chkMgKhi.Checked || chkMgMau.Checked))
                    this.ErrorText = Resources.ResourceMessage.ChuaNhapThongTinNongDo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
