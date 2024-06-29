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
using HIS.UC.UCHeniInfo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.UCHeniInfo.CustomValidateRule
{
    class TemplateHeinBHYT1__MediOrg__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtMaDKKCBBD;
        internal DevExpress.XtraEditors.GridLookUpEdit cboDKKCBBD;
        internal long PatientTypeId;
        internal DevExpress.XtraEditors.CheckEdit chkHasDobCertificate;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtMaDKKCBBD == null || cboDKKCBBD == null || chkHasDobCertificate == null || PatientTypeId <= 0) return valid;
                var patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == PatientTypeId && o.ID == DataStore.PatientTypeId);
                if (chkHasDobCertificate.Checked == false)
                    if ((patientType != null) && (String.IsNullOrEmpty(txtMaDKKCBBD.Text) || cboDKKCBBD.EditValue == null))
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
