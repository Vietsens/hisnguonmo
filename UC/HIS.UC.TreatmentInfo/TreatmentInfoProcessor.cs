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
using HIS.UC.TreatmentInfo.ADO;
using HIS.UC.TreatmentInfo.Run;
using HIS.UC.TreatmentInfo.SetValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TreatmentInfo
{
    public class TreatmentInfoProcessor
    {
        public object Run(InitADO arg)
        {
            object result = null;
            try
            {
                IRun behavior = RunFactory.MakeIRun(arg);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void SetValue(UserControl Uc, TreatmentInfoADO data)
        {
            try
            {
                ISetValue behavior = SetValueFactory.MakeISetValue(data);
                if (behavior != null) behavior.Run(Uc);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
