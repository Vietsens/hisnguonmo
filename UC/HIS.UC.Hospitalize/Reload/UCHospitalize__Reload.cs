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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using MOS.EFMODEL.DataModels;
using HIS.UC.Hospitalize.ADO;

namespace HIS.UC.Hospitalize.Run
{
    public partial class UCHospitalize : UserControl
    {
        public void Reload(HospitalizeInitADO hospitalize)
        {
            try
            {
                if (hospitalize != null)
                {
                    lblNumHospitalize.Text = hospitalize.InCode;
                    chkPrintHospitalizeExam.CheckState = hospitalize.isAutoCheckChkHospitalizeExam ? CheckState.Checked : CheckState.Unchecked;
                    if (hospitalize.FinishTime.HasValue)
                    {
                        dtFinishTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hospitalize.FinishTime.Value) ?? DateTime.Now;
                    }
                    LoadIcdToControl(hospitalize.IcdCode, hospitalize.IcdName);
                    LoadTraditionalIcdToControl(hospitalize.TraditionalIcdCode, hospitalize.TraditionalIcdName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
