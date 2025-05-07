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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.PlusInfo.ADO;

namespace HIS.UC.PlusInfo
{
    public class PlusInfoProsessor
    {
        public UCPlusInfo ControlWorker { get; set; }

        public PlusInfoProsessor()
        {
            this.Init();
        }

        void Init()
        {
            try
            {
                //this.ControlWorker = new UCPlusInfo(Inventec.Desktop.Common.Modules.Module module);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public PatientInformationADO GetValue()
        {
            try
            {
                //return this.ControlWorker.GetValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        
        public void SetValue(PatientInformationADO dataToFillUserControl)
        {
            try
            {
                if (dataToFillUserControl != null)
                {
                    //this.ControlWorker.SetValue(dataToFillUserControl);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }    
    }
}
