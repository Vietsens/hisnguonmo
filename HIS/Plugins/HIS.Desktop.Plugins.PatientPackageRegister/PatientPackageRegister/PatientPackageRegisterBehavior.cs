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
using HIS.Desktop.Plugins.PatientPackageRegister;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;

namespace Inventec.Desktop.Plugins.PatientPackageRegister.PatientPackageRegister
{
    public sealed class PatientPackageRegisterBehavior : Tool<IDesktopToolContext>, IPatientPackageRegister
    {
        object[] entity;

        public PatientPackageRegisterBehavior()
            : base()
        {
        }

        public PatientPackageRegisterBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IPatientPackageRegister.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                HIS_PATIENT patient = null;
                HIS_PATIENT_PACKAGE patientPackage = null;

                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is HIS_PATIENT)
                        {
                            patient = (HIS_PATIENT)entity[i];
                        }
                        else if (entity[i] is HIS_PATIENT_PACKAGE)
                        {
                            patientPackage = (HIS_PATIENT_PACKAGE)entity[i];
                        }
                    }
                }

                if (moduleData == null) throw new NullReferenceException("moduleData");

                return new frmPatientPackageRegister(moduleData, patient, patientPackage);
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + (entity != null ? entity.GetType().ToString() : "null") + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => entity), entity), ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
    }
}
