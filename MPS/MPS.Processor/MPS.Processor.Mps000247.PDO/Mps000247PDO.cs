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

namespace MPS.Processor.Mps000247.PDO
{
    public class Mps000247PDO : RDOBase
    {
        public HIS_DEPARTMENT Department { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        public List<V_HIS_EXP_MEST> _ExpMests_Print { get; set; }
        public long _ConfigKeyMERGER_DATA { get; set; }
        public long _TimeFilterOption { get; set; }
        public List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms { get; set; }
        public List<V_HIS_BED_LOG> _listBedLog { get; set; }

        public Mps000247PDO() { }

        public Mps000247PDO(
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            List<V_HIS_EXP_MEST> _expMests_Print,
            HIS_DEPARTMENT department,
            long _configKeyMERGER_DATA
            )
        {
            try
            {
                this._ExpMestMedicines = _expMestMedicines;
                this._ExpMestMaterials = _expMestMaterials;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this._ConfigKeyMERGER_DATA = _configKeyMERGER_DATA;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000247PDO(
           List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
           List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
           List<V_HIS_EXP_MEST> _expMests_Print,
           HIS_DEPARTMENT department,
           long _configKeyMERGER_DATA,
           List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms,
           List<V_HIS_BED_LOG> listBedLog
           )
        {
            try
            {
                this._ExpMestMedicines = _expMestMedicines;
                this._ExpMestMaterials = _expMestMaterials;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this._ConfigKeyMERGER_DATA = _configKeyMERGER_DATA;
                this.vHisTreatmentBedRooms = vHisTreatmentBedRooms;
                this._listBedLog = listBedLog;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000247PDO(
           List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
           List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
           List<V_HIS_EXP_MEST> _expMests_Print,
           HIS_DEPARTMENT department,
           long _configKeyMERGER_DATA,
           List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms,
           List<V_HIS_BED_LOG> listBedLog,
           long _timeFilterOption
           )
        {
            try
            {
                this._ExpMestMedicines = _expMestMedicines;
                this._ExpMestMaterials = _expMestMaterials;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this._ConfigKeyMERGER_DATA = _configKeyMERGER_DATA;
                this.vHisTreatmentBedRooms = vHisTreatmentBedRooms;
                this._listBedLog = listBedLog;
                this._TimeFilterOption = _timeFilterOption;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ExpMestMedicineADO : V_HIS_EXP_MEST_MEDICINE
    {

        public ExpMestMedicineADO() { }

        public ExpMestMedicineADO(List<V_HIS_EXP_MEST_MEDICINE> datas, long patientId, long intructionDate)
        {
            try
            {
                if (datas != null && datas.Count > 0 && patientId > 0 && intructionDate > 0)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineADO>(this, datas.FirstOrDefault());
                    this.AMOUNT = datas.Sum(p => p.AMOUNT);
                    this.TDL_PATIENT_ID = patientId;
                    this.TDL_INTRUCTION_DATE = intructionDate;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ExpMestMaterialADO : V_HIS_EXP_MEST_MATERIAL
    {
        public long TDL_PATIENT_ID { get; set; }

        public ExpMestMaterialADO() { }

        public ExpMestMaterialADO(List<V_HIS_EXP_MEST_MATERIAL> datas, long patientId, long intructionDate)
        {
            try
            {
                if (datas != null && datas.Count > 0 && patientId > 0 && intructionDate > 0)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMaterialADO>(this, datas.FirstOrDefault());
                    this.AMOUNT = datas.Sum(p => p.AMOUNT);
                    this.TDL_PATIENT_ID = patientId;
                    this.TDL_INTRUCTION_DATE = intructionDate;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }



    public class ExpMestADO : V_HIS_EXP_MEST 
    {
        public string BED_CODE { get; set; }
        public string BED_NAME { get; set; }
        public string BED_ROOM_NAMEs { get; set; }

        public ExpMestADO() { }

        public ExpMestADO(V_HIS_EXP_MEST data) 
        {
            try
            { 
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestADO>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
