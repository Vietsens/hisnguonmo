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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000003.PDO
{
    public class Mps000003PDO : RDOBase
    {
        public V_HIS_PATY_ALTER_BHYT PatyAlterBhyt { get; set; }
        public V_HIS_PATIENT Patient { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        public List<SereSrevPrintADO> HisSereServ_Bordereau_Medicines = null;//list Medicine
        public List<SereSrevPrintADO> HisSereServ_Bordereau_Services = null;//list Service
        public List<SereSrevPrintADO> HisSereServ_Bordereau_Materials = null;//list Material
        public List<SereSrevPrintADO> HisSereServ_Bordereau_ExamServices = null;//list exam Service
        public V_HIS_TREATMENT treatment { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans;
        public List<V_HIS_SERE_SERV> SereServ2s { get; set; }
        public string currentDateSeparateFullTime = "";

        public Mps000003PDO(
            V_HIS_PATIENT patient,
            V_HIS_PATY_ALTER_BHYT patyAlterBhyt,
            V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter,
            List<SereSrevPrintADO> HisSereServ_Bordereau_Medicines,
            List<SereSrevPrintADO> HisSereServ_Bordereau_Services,
            List<SereSrevPrintADO> HisSereServ_Bordereau_Materials,
            List<SereSrevPrintADO> HisSereServ_Bordereau_ExamServices,
            V_HIS_TREATMENT treatment,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans,
            List<V_HIS_SERE_SERV> SereServ2s,
            string currentDateSeparateFullTime)
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.PatientTypeAlter = PatientTypeAlter;
                this.HisSereServ_Bordereau_Medicines = HisSereServ_Bordereau_Medicines;
                this.HisSereServ_Bordereau_Services = HisSereServ_Bordereau_Services;
                this.HisSereServ_Bordereau_Materials = HisSereServ_Bordereau_Materials;
                this.HisSereServ_Bordereau_ExamServices = HisSereServ_Bordereau_ExamServices;
                this.treatment = treatment;
                this.departmentTrans = departmentTrans;
                this.SereServ2s = SereServ2s;
                this.currentDateSeparateFullTime = currentDateSeparateFullTime;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class SereSrevPrintADO : MOS.EFMODEL.DataModels.V_HIS_SERE_SERV
    {
        public string CONCENTRA_PACKING_TYPE_NAME { get; set; }
        public SereSrevPrintADO(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV data)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereSrevPrintADO(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV data, List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE> currentMedicines, List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> currentMedicineTypes)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }
                if (currentMedicines != null && currentMedicines.Count > 0)
                {
                    var medi = currentMedicines.FirstOrDefault(o => o.ID == data.MEDICINE_ID);
                    if (medi != null)
                    {
                        if (currentMedicineTypes != null)
                        {
                            var mediType = currentMedicineTypes.FirstOrDefault(o => o.ID == medi.ID);
                            CONCENTRA_PACKING_TYPE_NAME = mediType.CONCENTRA;
                            if (!String.IsNullOrEmpty(mediType.PACKING_TYPE_NAME))
                            {
                                CONCENTRA_PACKING_TYPE_NAME += ";" + mediType.PACKING_TYPE_NAME;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereSrevPrintADO(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV data, List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL> currentMaterials, List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> currentMaterialTypes)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }
                if (currentMaterials != null && currentMaterials.Count > 0)
                {
                    var medi = currentMaterials.FirstOrDefault(o => o.ID == data.MEDICINE_ID);
                    if (medi != null)
                    {
                        if (currentMaterialTypes != null)
                        {
                            var mediType = currentMaterialTypes.FirstOrDefault(o => o.ID == medi.ID);
                            CONCENTRA_PACKING_TYPE_NAME = mediType.CONCENTRA;
                            if (!String.IsNullOrEmpty(mediType.PACKING_TYPE_NAME))
                            {
                                CONCENTRA_PACKING_TYPE_NAME += ";" + mediType.PACKING_TYPE_NAME;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
