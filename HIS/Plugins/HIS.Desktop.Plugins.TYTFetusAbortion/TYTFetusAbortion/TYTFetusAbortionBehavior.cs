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
using Inventec.Core;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TYT.Desktop.Plugins.FetusAbortion;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;

namespace TYT.Desktop.Plugins.FetusAbortion.TYTFetusAbortion
{
    public sealed class TYTFetusAbortionBehavior : Tool<IDesktopToolContext>, ITYTFetusAbortion
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        V_HIS_PATIENT patient { get; set; }
        public TYTFetusAbortionBehavior()
            : base()
        {
        }

        public TYTFetusAbortionBehavior(CommonParam param,V_HIS_PATIENT patient, Inventec.Desktop.Common.Modules.Module moduleData)
            : base()
        {
            this.moduleData = moduleData;
            this.patient = patient;
        }

        object ITYTFetusAbortion.Run()
        {
            try
            {
                return new frmTYTFetusAbortion(this.moduleData, patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true;
                return null;
            }
        }
    }
}
