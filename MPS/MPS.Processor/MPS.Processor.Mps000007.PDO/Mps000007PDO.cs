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

namespace MPS.Processor.Mps000007.PDO
{
    public partial class Mps000007PDO : RDOBase
    {
        public SingleKeyValue SingleKeyValue { get; set; }


        public Mps000007PDO(
           V_HIS_PATIENT _currentPatient,
           V_HIS_PATIENT_TYPE_ALTER _patyAlter,
           List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
           V_HIS_SERVICE_REQ _examServiceReq,
           HIS_DHST _dhst,
           V_HIS_TREATMENT _treatment,
           List<HIS_SERE_SERV> _sereServs,
           SingleKeyValue _singleKeyValue,
            List<V_HIS_EXP_MEST_BLOOD> _expMestBloodList,
            List<V_HIS_EXP_MEST_BLTY_REQ> _expMestBltyReqlist,
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicineList,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterialList
           )
        {
            try
            {
                this.PatyAlter = _patyAlter;
                this.Treatment = _treatment;
                this.SereServs = _sereServs;
                this.DepartmentTrans = _departmentTrans;
                this.SingleKeyValue = _singleKeyValue;
                this.ExamServiceReq = _examServiceReq;
                this._currentPatient = _currentPatient;
                this.DHST = _dhst;
                this.ExpMestBloodList = _expMestBloodList;
                this.ExpMestBltyReqList = _expMestBltyReqlist;
                this.ExpMestMaterialList = _expMestMaterialList;
                this.ExpMestMedicineList = _expMestMedicineList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000007PDO(
            V_HIS_PATIENT _currentPatient,
            V_HIS_PATIENT_TYPE_ALTER _patyAlter,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            V_HIS_SERVICE_REQ _examServiceReq,
            HIS_DHST _dhst,
            V_HIS_TREATMENT _treatment,
            List<HIS_SERE_SERV> _sereServs,
            SingleKeyValue _singleKeyValue
            )
        {
            try
            {
                this.PatyAlter = _patyAlter;
                this.Treatment = _treatment;
                this.SereServs = _sereServs;
                this.DepartmentTrans = _departmentTrans;
                this.SingleKeyValue = _singleKeyValue;
                this.ExamServiceReq = _examServiceReq;
                this._currentPatient = _currentPatient;
                this.DHST = _dhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000007PDO(
            V_HIS_PATIENT _currentPatient,
            V_HIS_PATIENT_TYPE_ALTER _patyAlter,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            V_HIS_SERVICE_REQ _examServiceReq,
            HIS_DHST _dhst,
            V_HIS_TREATMENT _treatment,
            SingleKeyValue _singleKeyValue
            )
        {
            try
            {
                this.PatyAlter = _patyAlter;
                this.Treatment = _treatment;
                this.DepartmentTrans = _departmentTrans;
                this.SingleKeyValue = _singleKeyValue;
                this.ExamServiceReq = _examServiceReq;
                this._currentPatient = _currentPatient;
                this.DHST = _dhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
