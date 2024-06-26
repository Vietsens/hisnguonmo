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
using AutoMapper;
using HIS.Desktop.Plugins.RegisterV3.Run3;
using HIS.Desktop.Utility;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterV3.Register
{
    class ServiceRequestRegisterPatientProfileBehavior : ServiceRequestRegisterBehaviorBase, IServiceRequestRegisterPatientProfile
    {
        HisPatientProfileSDO result = null;
        UCRegister _ucServiceRequestRegister;
        internal ServiceRequestRegisterPatientProfileBehavior(CommonParam param, UCRegister ucServiceRequestRegiter, HisPatientSDO patientData)
            : base(param, ucServiceRequestRegiter)
        {
            this._ucServiceRequestRegister = ucServiceRequestRegiter;
        }

        HisPatientProfileSDO IServiceRequestRegisterPatientProfile.Run()
        {
            this.patientProfile = new HisPatientProfileSDO();
            this.patientProfile.HisPatient = new MOS.EFMODEL.DataModels.HIS_PATIENT();
            this.patientProfile.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();
            this.patientProfile.HisTreatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();

            //Process common data
            base.InitBase();

            //Execute call api
            result = (HisPatientProfileSDO)base.RunBase(this.patientProfile, this.ucRequestService);
            if (result == null)
            {
                Inventec.Common.Logging.LogSystem.Warn("Goi api dang ky tiep don that bai, Dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientProfile), patientProfile) + ", Dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
            }
            return result;
        }
    }
}
