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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
using HIS.Desktop.Utility;
using HIS.UC.WorkPlace;
using IMSys.DbConfig.HIS_RS;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.SickLeave
{
    public partial class frmSickLeave : FormBase
    {
        public void SetValue(object data)
        {
            try
            {
                if (data != null && data is TreatmentEndTypeExtData)
                {
                    TreatmentEndTypeExtData sickLeaveData = data as TreatmentEndTypeExtData;
                    if (sickLeaveData.SickLeaveDay.HasValue)
                        spinSickLeaveDay.Value = sickLeaveData.SickLeaveDay.Value;
                    if (sickLeaveData.SickLeaveFrom.HasValue)
                        dtSickLeaveFromTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sickLeaveData.SickLeaveFrom.Value) ?? DateTime.Now;
                    if (sickLeaveData.SickLeaveTo.HasValue)
                        dtSickLeaveToTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sickLeaveData.SickLeaveTo.Value) ?? DateTime.Now;
                    txtPatientMotherName.Text = sickLeaveData.MotherName;
                    txtPatientFatherName.Text = sickLeaveData.FatherName;
                    txtRelativeName.Text = sickLeaveData.PatientRelativeName;
                    cboRelativeType.EditValue = sickLeaveData.PatientRelativeType;
                    txtPatientWorkPlace.Text = sickLeaveData.PatientWorkPlace;
                    cboDocumentBook.EditValue = sickLeaveData.DocumentBookId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
