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
        private void CalculateDayNum()
        {
            try
            {
                if (dtSickLeaveFromTime.EditValue != null && dtSickLeaveFromTime.DateTime != DateTime.MinValue
                    && dtSickLeaveToTime.EditValue != null && dtSickLeaveToTime.DateTime != DateTime.MinValue
                    && dtSickLeaveFromTime.DateTime.Date <= dtSickLeaveToTime.DateTime.Date)
                {
                    TimeSpan ts = (TimeSpan)(dtSickLeaveToTime.DateTime.Date - dtSickLeaveFromTime.DateTime.Date);
                    spinSickLeaveDay.Value = ts.Days + 1;
                }
                //else
                //{
                //    spinSickLeaveDay.EditValue = null;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateDateTo()
        {
            try
            {
                if (dtSickLeaveFromTime.EditValue != null && dtSickLeaveFromTime.DateTime != DateTime.MinValue && spinSickLeaveDay.EditValue != null)
                {
                    dtSickLeaveToTime.DateTime = dtSickLeaveFromTime.DateTime.AddDays((double)(spinSickLeaveDay.Value - 1));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<BabyADO> GetBabys()
        {
            List<BabyADO> babyADOs = null;
            try
            {
                List<BabyADO> temps = gridControlMaternityLeave.DataSource as List<BabyADO>;
                if (temps != null && temps.Count > 0)
                {
                    babyADOs = temps.Where(o =>
                        o.BornTimeDt.HasValue
                        || !String.IsNullOrEmpty(o.FatherName)
                        || o.GenderId.HasValue
                        || o.Weight.HasValue).ToList();
                    if (babyADOs == null || babyADOs.Count == 0)
                    {
                        babyADOs = null;
                    }
                }
            }
            catch (Exception ex)
            {
                babyADOs = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return babyADOs;
        }
    }
}
