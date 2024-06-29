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

namespace HIS.UC.DHST.Run
{
    public partial class UCDHST : UserControl
    {
        public object GetValue()
        {
            object result = null;
            try
            {
                DHSTADO outPut = new DHSTADO();
                if (dtExecuteTime.EditValue != null)
                    outPut.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExecuteTime.DateTime);
                if (spinBloodPressureMax.EditValue != null)
                    outPut.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMax.Value.ToString());
                if (spinBloodPressureMin.EditValue != null)
                    outPut.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMin.Value.ToString());
                if (spinBreathRate.EditValue != null)
                    outPut.BREATH_RATE = Inventec.Common.Number.Get.RoundCurrency(spinBreathRate.Value, 2);
                if (spinHeight.EditValue != null)
                    outPut.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinHeight.Value, 2);
                if (spinChest.EditValue != null)
                    outPut.CHEST = Inventec.Common.Number.Get.RoundCurrency(spinChest.Value, 2);
                if (spinBelly.EditValue != null)
                    outPut.BELLY = Inventec.Common.Number.Get.RoundCurrency(spinBelly.Value, 2);
                if (spinPulse.EditValue != null)
                    outPut.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spinPulse.Value.ToString());
                if (spinTemperature.EditValue != null)
                    outPut.TEMPERATURE = Inventec.Common.Number.Get.RoundCurrency(spinTemperature.Value, 2);
                if (spinWeight.EditValue != null)
                    outPut.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinWeight.Value, 2);
                if (spinSPO2.EditValue != null)
                    outPut.SPO2 = Inventec.Common.Number.Get.RoundCurrency(spinSPO2.Value, 2) / 100;
                if (spinUrine.EditValue != null)
                    outPut.URINE = Inventec.Common.Number.Get.RoundCurrency(spinUrine.Value, 2);
                if (spinCapillaryBloodGlucose.EditValue != null)
                    outPut.CAPILLARY_BLOOD_GLUCOSE = Inventec.Common.Number.Get.RoundCurrency(spinCapillaryBloodGlucose.Value, 2);
                outPut.NOTE = txtNote.Text;

                outPut.IsVali = true;
                //if (dhstInit.IsRequired || dhstInit.IsRequiredWeight)
                //{
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                {
                    outPut.IsVali = false;
                }
                // }

                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
