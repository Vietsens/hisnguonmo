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
using His.UC.UCHein.Core.UpdateDataFormIntoPatientProfile;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His.UC.UCHein.Core.UpdateDataFormIntoPatientTypeAlter
{
    class UpdateDataFormIntoPatientTypeAlterFactory
    {
        internal static IUpdateDataFormIntoPatientTypeAlter MakeIUpdateDataFormIntoPatientProfile(CommonParam param, UserControl UC, HisPatientProfileSDO patientProfileSDO)
        {
            IUpdateDataFormIntoPatientTypeAlter result = null;
            try
            {
                if (UC is Design.TemplateHeinBHYT1.Template__HeinBHYT1)
                {
                    result = new UpdateDataFormIntoPatientTypeAlterBehavior(param, ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)UC), patientProfileSDO);
                }
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + (UC != null ? UC.GetType().ToString() : patientProfileSDO.GetType().ToString()) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientProfileSDO), patientProfileSDO), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
