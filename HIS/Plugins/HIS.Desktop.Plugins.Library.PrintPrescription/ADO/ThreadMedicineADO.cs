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

namespace HIS.Desktop.Plugins.Library.PrintPrescription.ADO
{
    class ThreadMedicineADO
    {
        #region du lieu vao
        public List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> ExpMests { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL> Materials { get; set; }
        //public List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATY_REQ> MatyReqs { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE> Medicines { get; set; }
        //public List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_METY_REQ> MetyReqs { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY> ServiceReqMaties { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY> ServiceReqMeties { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ> ServiceReqs { get; set; }
        
        #endregion
        public bool? HasOutHospital { get; set; }
        public bool HasMediMate { get; set; }

        /// <summary>
        /// key exp_Mest_id thuốc, vật tư trong kho
        /// key service_req_id thuốc, vật tư ngoài kho
        /// </summary>
        public Dictionary<long, List<ExpMestMedicineSDO>> DicLstMediMateExpMestTypeADO { get; set; }//ra
        //public List<ExpMestMedicineSDO> lstMedicineExpmestTypeADO { get; set; }
        
        public ThreadMedicineADO(MOS.SDO.OutPatientPresResultSDO data, bool hasMediMate, bool? hasOutHospital = null)
        {
            try
            {
                if (data != null)
                {
                    this.ExpMests = data.ExpMests;
                    this.Materials = data.Materials;
                    this.Medicines = data.Medicines;
                    this.ServiceReqMaties = data.ServiceReqMaties;
                    this.ServiceReqMeties = data.ServiceReqMeties;
                    this.ServiceReqs = data.ServiceReqs;
                    this.HasMediMate = hasMediMate;
                    this.HasOutHospital = hasOutHospital;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public ThreadMedicineADO(MOS.SDO.InPatientPresResultSDO data, bool hasMediMate, bool? hasOutHospital = null)
        {
            try
            {
                if (data != null)
                {
                    this.ExpMests = data.ExpMests;
                    this.Materials = data.Materials;
                    this.Medicines = data.Medicines;
                    this.ServiceReqMaties = data.ServiceReqMaties;
                    this.ServiceReqMeties = data.ServiceReqMeties;
                    this.ServiceReqs = data.ServiceReqs;
                    this.HasMediMate = hasMediMate;
                    this.HasOutHospital = hasOutHospital;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
