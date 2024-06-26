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
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.DHST.Run
{
    public partial class UCDHST : UserControl
    {
        public object SetValue(HIS_DHST dhst)
        {
            try
            {
                if (dhst != null)
                {
                    if (dhst.EXECUTE_TIME != null)
                        dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dhst.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtExecuteTime.EditValue = DateTime.Now;
                    spinBloodPressureMax.EditValue = dhst.BLOOD_PRESSURE_MAX;
                    spinBloodPressureMin.EditValue = dhst.BLOOD_PRESSURE_MIN;
                    spinBreathRate.EditValue = dhst.BREATH_RATE;
                    spinHeight.EditValue = dhst.HEIGHT;
                    spinChest.EditValue = dhst.CHEST;
                    spinBelly.EditValue = dhst.BELLY;
                    spinPulse.EditValue = dhst.PULSE;
                    spinTemperature.EditValue = dhst.TEMPERATURE;
                    spinWeight.EditValue = dhst.WEIGHT;
                    if (dhst.SPO2.HasValue)
                        spinSPO2.Value = (dhst.SPO2.Value * 100);
                    else
                        spinSPO2.EditValue = null;
                    txtNote.Text = dhst.NOTE;
                    spinUrine.EditValue = dhst.URINE;
                    spinCapillaryBloodGlucose.EditValue = dhst.CAPILLARY_BLOOD_GLUCOSE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
    }
}
