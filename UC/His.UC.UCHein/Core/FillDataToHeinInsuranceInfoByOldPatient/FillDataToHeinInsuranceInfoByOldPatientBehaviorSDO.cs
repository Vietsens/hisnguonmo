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
using System.Windows.Forms;
using His.UC.UCHein.Base;

namespace His.UC.UCHein.FillDataToHeinInsuranceInfoByOldPatient
{
    public sealed class FillDataToHeinInsuranceInfoByOldPatientBehaviorSDO : IRun
    {
        MOS.SDO.HisPatientSDO patientParam;
        System.Windows.Forms.UserControl ucParam;

        public FillDataToHeinInsuranceInfoByOldPatientBehaviorSDO()
            : base()
        {
        }

        public FillDataToHeinInsuranceInfoByOldPatientBehaviorSDO(CommonParam param, MOS.SDO.HisPatientSDO patient, System.Windows.Forms.UserControl uc)
            : base()
        {
            this.patientParam = patient;
            this.ucParam = uc;
        }

        object IRun.Run()
        {
            try
            {
                if (this.ucParam.GetType() == typeof(Design.TemplateHeinBHYT1.Template__HeinBHYT1))
                {
                    ((Design.TemplateHeinBHYT1.Template__HeinBHYT1)this.ucParam).FillDataToHeinInsuranceInfoByOldPatient(this.patientParam);
                }
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + patientParam.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientParam), patientParam), ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
    }
}
