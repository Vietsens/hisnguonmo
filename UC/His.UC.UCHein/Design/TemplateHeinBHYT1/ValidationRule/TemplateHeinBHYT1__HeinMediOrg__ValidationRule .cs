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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His.UC.UCHein.ValidationRule
{
    class TemplateHeinBHYT1__HeinMediOrg__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cbo.EditValue == null)
                    return false;
                var mediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == cbo.EditValue.ToString());
                if (!string.IsNullOrEmpty(BranchDataWorker.Branch.DO_NOT_ALLOW_HEIN_LEVEL_CODE) && mediOrg != null && (";" + BranchDataWorker.Branch.DO_NOT_ALLOW_HEIN_LEVEL_CODE + ";").Contains(";" + mediOrg.LEVEL_CODE + ";"))
                {
                    this.ErrorText = String.Format("Nơi đăng ký khám chữa bệnh ban đầu thuộc tuyến {0}, không được hưởng BHYT", mediOrg.LEVEL_CODE == "1" ? "trung ương" : (mediOrg.LEVEL_CODE == "2" ? "Tỉnh" : (mediOrg.LEVEL_CODE == "3" ? "Huyện" : "Xã")));
                    this.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    valid = false;
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
