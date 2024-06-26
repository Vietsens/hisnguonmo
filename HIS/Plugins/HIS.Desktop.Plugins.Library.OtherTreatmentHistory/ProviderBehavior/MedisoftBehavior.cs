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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Library.OtherTreatmentHistory.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.OtherTreatmentHistory.ProviderBehavior
{
    class MedisoftBehavior : IRun
    {
        const string IntegrateFolder = "Integrate";
        const string MedisoftFlder = "MEDISOFT";
        const string dltw = "DLTW.dll";

        private Base.InitDataADO InitDataADO;

        public MedisoftBehavior(Base.InitDataADO initDataADO)
        {
            // TODO: Complete member initialization
            this.InitDataADO = initDataADO;
        }

        void IRun.Run(Enum type)
        {
            if (this.InitDataADO.PatientId > 0 && this.InitDataADO.Patient == null)
            {
                GetPatient();
            }

            switch (type)
            {
                case Enum.Xemthuocpk:
                    Xemthuocpk(this.InitDataADO.Patient);
                    break;
                case Enum.XemCanLamSan:
                    XemCanLamSan(this.InitDataADO.Patient);
                    break;
                default:
                    break;
            }
        }

        public void Xemthuocpk(HIS_PATIENT patient)
        {
            try
            {
                //DLTW.frmXemthuocpk f = new DLTW.frmXemthuocpk(MedisoftCode(patientCode), patientName);
                DLTW.frmXemthuocpk f = new DLTW.frmXemthuocpk(patient.PATIENT_CODE, patient.VIR_PATIENT_NAME);
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void XemCanLamSan(HIS_PATIENT patient)
        {
            try
            {
                //DLTW.frmXemCanLamSan f = new DLTW.frmXemCanLamSan(MedisoftCode(patientCode));
                DLTW.frmXemCanLamSan f = new DLTW.frmXemCanLamSan(patient.PATIENT_CODE);
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string MedisoftCode(string patientCode)
        {
            return (!String.IsNullOrWhiteSpace(patientCode) && patientCode.Length == 10) ? patientCode.Substring(2, 8) : patientCode;
        }

        private void GetPatient()
        {
            try
            {
                if (this.InitDataADO.PatientId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.ID = this.InitDataADO.PatientId;
                    var lstpatient = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param);
                    if (lstpatient != null && lstpatient.Count > 0)
                    {
                        this.InitDataADO.Patient = lstpatient.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
