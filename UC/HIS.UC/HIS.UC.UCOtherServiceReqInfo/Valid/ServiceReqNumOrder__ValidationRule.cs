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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utility;
using HIS.UC.UCOtherServiceReqInfo.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.UCOtherServiceReqInfo.Valid
{
    class ServiceReqNumOrder__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.SpinEdit spinNumOrderPriority;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (spinNumOrderPriority != null);
                if (valid)
                {
                    if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.ReservedNumOders != null
                        && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.ReservedNumOders.Count > 0
                        && spinNumOrderPriority.Value > 0
                        && !HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.ReservedNumOders.Contains(((long)spinNumOrderPriority.Value).ToString())
                        )
                    {
                        valid = false;
                        this.ErrorText = String.Format(ResourceMessage.SoThuTuUuTienPhaiNamTrongDanhSachCauHinhCacSoUuTien, ""); //String.Join("", HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.ReservedNumOders);
                    }
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
