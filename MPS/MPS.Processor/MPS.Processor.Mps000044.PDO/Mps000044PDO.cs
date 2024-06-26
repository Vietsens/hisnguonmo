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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MPS.ProcessorBase;
using MPS.Processor.Mps000044.PDO;

namespace MPS.Processor.Mps000044.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000044PDO : RDOBase
    {
        public const string PrintTypeCode = "Mps000044";
        public HIS_EXP_MEST HisExpMest { get; set; }
        public HIS_TRANS_REQ TransReq { get; set; }
        public List<HIS_CONFIG> ListHisConfigPaymentQrCode { get; set; }

        public Mps000044PDO() { }

        public Mps000044PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST dhsts,
            HIS_SERVICE_REQ vHisPrescription5,
            List<ExpMestMedicineSDO> expMestMedicines,
            Mps000044ADO mps000044ADO,
            HIS_TREATMENT _hisTreatment,
            long? _KeyUseForm
            )
        {
            try
            {
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.dhsts = dhsts;
                this.vHisPrescription5 = vHisPrescription5;
                this.expMestMedicines = expMestMedicines.ToList();
                this.Mps000044ADO = mps000044ADO;
                this.hisTreatment = _hisTreatment;
                this.KeyUseForm = _KeyUseForm;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000044PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST dhsts,
            HIS_SERVICE_REQ vHisPrescription5,
            List<ExpMestMedicineSDO> expMestMedicines,
            Mps000044ADO mps000044ADO,
            HIS_TREATMENT _hisTreatment,
            long? _KeyUseForm,
             HIS_SERVICE_REQ _hisServiceReq_CurentExam
            )
        {
            try
            {
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.dhsts = dhsts;
                this.vHisPrescription5 = vHisPrescription5;
                this.expMestMedicines = expMestMedicines.ToList();
                this.Mps000044ADO = mps000044ADO;
                this.hisTreatment = _hisTreatment;
                this.KeyUseForm = _KeyUseForm;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000044PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST dhsts,
            HIS_SERVICE_REQ vHisPrescription5,
            List<ExpMestMedicineSDO> expMestMedicines,
            Mps000044ADO mps000044ADO,
            HIS_TREATMENT _hisTreatment,
            long? _KeyUseForm,
            HIS_EXP_MEST _hisExpMest
            )
        {
            try
            {
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.dhsts = dhsts;
                this.vHisPrescription5 = vHisPrescription5;
                this.expMestMedicines = expMestMedicines.ToList();
                this.Mps000044ADO = mps000044ADO;
                this.hisTreatment = _hisTreatment;
                this.KeyUseForm = _KeyUseForm;
                this.HisExpMest = _hisExpMest;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000044PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST dhsts,
            HIS_SERVICE_REQ vHisPrescription5,
            List<ExpMestMedicineSDO> expMestMedicines,
            Mps000044ADO mps000044ADO,
            HIS_TREATMENT _hisTreatment,
            long? _KeyUseForm,
            HIS_EXP_MEST _hisExpMest,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam
            )
        {
            try
            {
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.dhsts = dhsts;
                this.vHisPrescription5 = vHisPrescription5;
                this.expMestMedicines = expMestMedicines.ToList();
                this.Mps000044ADO = mps000044ADO;
                this.hisTreatment = _hisTreatment;
                this.KeyUseForm = _KeyUseForm;
                this.HisExpMest = _hisExpMest;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000044PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST dhsts,
            HIS_SERVICE_REQ vHisPrescription5,
            List<ExpMestMedicineSDO> expMestMedicines,
            Mps000044ADO mps000044ADO,
            HIS_TREATMENT _hisTreatment,
            long? _KeyUseForm,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listHisConfigPaymentQrCode
            )
        {
            try
            {
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.dhsts = dhsts;
                this.vHisPrescription5 = vHisPrescription5;
                this.expMestMedicines = expMestMedicines.ToList();
                this.Mps000044ADO = mps000044ADO;
                this.hisTreatment = _hisTreatment;
                this.KeyUseForm = _KeyUseForm;
                this.TransReq = transReq;
                this.ListHisConfigPaymentQrCode = listHisConfigPaymentQrCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000044PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST dhsts,
            HIS_SERVICE_REQ vHisPrescription5,
            List<ExpMestMedicineSDO> expMestMedicines,
            Mps000044ADO mps000044ADO,
            HIS_TREATMENT _hisTreatment,
            long? _KeyUseForm,
             HIS_SERVICE_REQ _hisServiceReq_CurentExam,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listHisConfigPaymentQrCode
            )
        {
            try
            {
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.dhsts = dhsts;
                this.vHisPrescription5 = vHisPrescription5;
                this.expMestMedicines = expMestMedicines.ToList();
                this.Mps000044ADO = mps000044ADO;
                this.hisTreatment = _hisTreatment;
                this.KeyUseForm = _KeyUseForm;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
                this.TransReq = transReq;
                this.ListHisConfigPaymentQrCode = listHisConfigPaymentQrCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000044PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST dhsts,
            HIS_SERVICE_REQ vHisPrescription5,
            List<ExpMestMedicineSDO> expMestMedicines,
            Mps000044ADO mps000044ADO,
            HIS_TREATMENT _hisTreatment,
            long? _KeyUseForm,
            HIS_EXP_MEST _hisExpMest,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listHisConfigPaymentQrCode
            )
        {
            try
            {
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.dhsts = dhsts;
                this.vHisPrescription5 = vHisPrescription5;
                this.expMestMedicines = expMestMedicines.ToList();
                this.Mps000044ADO = mps000044ADO;
                this.hisTreatment = _hisTreatment;
                this.KeyUseForm = _KeyUseForm;
                this.HisExpMest = _hisExpMest;
                this.TransReq = transReq;
                this.ListHisConfigPaymentQrCode = listHisConfigPaymentQrCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000044PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_DHST dhsts,
            HIS_SERVICE_REQ vHisPrescription5,
            List<ExpMestMedicineSDO> expMestMedicines,
            Mps000044ADO mps000044ADO,
            HIS_TREATMENT _hisTreatment,
            long? _KeyUseForm,
            HIS_EXP_MEST _hisExpMest,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listHisConfigPaymentQrCode
            )
        {
            try
            {
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.dhsts = dhsts;
                this.vHisPrescription5 = vHisPrescription5;
                this.expMestMedicines = expMestMedicines.ToList();
                this.Mps000044ADO = mps000044ADO;
                this.hisTreatment = _hisTreatment;
                this.KeyUseForm = _KeyUseForm;
                this.HisExpMest = _hisExpMest;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
                this.TransReq = transReq;
                this.ListHisConfigPaymentQrCode = listHisConfigPaymentQrCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
