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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.CareCreate.Resources;
using HIS.Desktop.Plugins.CareCreate.Validate.ValidationRule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CareCreate
{
    public partial class CareCreate : HIS.Desktop.Utility.FormBase
    {
        private void ValidControl()
        {
            try
            {
                ValidExecuteTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidExecuteTime()
        {
            CareExecuteTime__ValidationRule oDobDateRule = new CareExecuteTime__ValidationRule();
            oDobDateRule.dtExecuteTime = dtExcuteTime;
            oDobDateRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.NguoiDungNhapNgayKhongHopLe);
            oDobDateRule.ErrorType = ErrorType.Warning;
            this.dxValidationProvider.SetValidationRule(dtExcuteTime, oDobDateRule);
        }

        private void ValidSpinPulse(SpinEdit spinEdit)
        {
            SpinEditValidationRule spin = new SpinEditValidationRule();
            spin.spinEdit = spinEdit;
            spin.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spin.ErrorType = ErrorType.Warning;
            this.dxValidationProvider.SetValidationRule(spinEdit, spin);
        }
    }
}
