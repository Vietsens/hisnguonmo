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
using HIS.Desktop.Utility;
using MOS.SDO;
using HIS.UC.UCRelativeInfo.ADO;

namespace HIS.Desktop.Plugins.RegisterV3.Run3
{
    public partial class UCRegister : UserControlBase
    {
        private void FillDataIntoUCRelativeInfo(object data)
        {
            try
            {
                if (data is HisPatientSDO)
                {
                    HisPatientSDO dataOldPatient = (HisPatientSDO)data;
                    UCRelativeADO dataRelative = new UCRelativeADO();
                    dataRelative.Correlated = dataOldPatient.RELATIVE_TYPE;
                    dataRelative.RelativeCMND = dataOldPatient.RELATIVE_CMND_NUMBER;
                    dataRelative.RelativeAddress = dataOldPatient.RELATIVE_ADDRESS;
                    dataRelative.RelativeName = dataOldPatient.RELATIVE_NAME;
                    this.ucRelativeInfo1.SetValue(dataRelative);
                }
                else if (data is UCRelativeADO)
                {
                    UCRelativeADO dataRelative = (UCRelativeADO)data;
                    this.ucRelativeInfo1.SetValue(dataRelative);
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
