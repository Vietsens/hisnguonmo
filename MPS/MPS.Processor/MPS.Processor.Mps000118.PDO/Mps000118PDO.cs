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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000118.PDO
{
    public partial class Mps000118PDO : RDOBase
    {
        public const string PrintTypeCode = "Mps000118";
        public List<HIS_SERE_SERV> ListSereServCls { get; set; }
        public HIS_EXP_MEST HisExpMest { get; set; }
        public HIS_TRANS_REQ TransReq { get; set; }
        public List<HIS_CONFIG> ListHisConfigPaymentQrCode { get; set; }

        public Mps000118PDO() { }

        public Mps000118PDO(
          V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
          HIS_DHST hisDhst,
          HIS_SERVICE_REQ HisPrescription,
          List<ExpMestMedicineSDO> expMestMedicines,
          HIS_SERVICE_REQ hisServiceReq_Exam,
          HIS_TREATMENT treatment,
          Mps000118ADO mps000118ADO,
          List<HIS_SERE_SERV> listSereServCls,
              long? _KeyUseForm)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000118ADO = mps000118ADO;
                this.hisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
                this._KeyUseForm = _KeyUseForm;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000118PDO(
          V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
          HIS_DHST hisDhst,
          HIS_SERVICE_REQ HisPrescription,
          List<ExpMestMedicineSDO> expMestMedicines,
          HIS_SERVICE_REQ hisServiceReq_Exam,
          HIS_TREATMENT treatment,
          Mps000118ADO mps000118ADO,
          List<HIS_SERE_SERV> listSereServCls,
              long? _KeyUseForm,
              HIS_SERVICE_REQ _hisServiceReq_CurentExam)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000118ADO = mps000118ADO;
                this.hisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
                this._KeyUseForm = _KeyUseForm;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000118PDO(
          V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
          HIS_DHST hisDhst,
          HIS_SERVICE_REQ HisPrescription,
          List<ExpMestMedicineSDO> expMestMedicines,
          HIS_SERVICE_REQ hisServiceReq_Exam,
          HIS_TREATMENT treatment,
          Mps000118ADO mps000118ADO,
          List<HIS_SERE_SERV> listSereServCls,
              long? _KeyUseForm,
            HIS_EXP_MEST _hisExpMest)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000118ADO = mps000118ADO;
                this.hisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
                this._KeyUseForm = _KeyUseForm;
                this.HisExpMest = _hisExpMest;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000118PDO(
          V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
          HIS_DHST hisDhst,
          HIS_SERVICE_REQ HisPrescription,
          List<ExpMestMedicineSDO> expMestMedicines,
          HIS_SERVICE_REQ hisServiceReq_Exam,
          HIS_TREATMENT treatment,
          Mps000118ADO mps000118ADO,
          List<HIS_SERE_SERV> listSereServCls,
              long? _KeyUseForm,
            HIS_EXP_MEST _hisExpMest,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000118ADO = mps000118ADO;
                this.hisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
                this._KeyUseForm = _KeyUseForm;
                this.HisExpMest = _hisExpMest;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000118PDO(
        V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
        HIS_DHST hisDhst,
        HIS_SERVICE_REQ HisPrescription,
        List<ExpMestMedicineSDO> expMestMedicines,
        HIS_SERVICE_REQ hisServiceReq_Exam,
        HIS_TREATMENT treatment,
        Mps000118ADO mps000118ADO,
        List<HIS_SERE_SERV> listSereServCls,
        long? _KeyUseForm,
        HIS_TRANS_REQ transReq,
        List<HIS_CONFIG> listHisConfigPaymentQrCode)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000118ADO = mps000118ADO;
                this.hisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
                this._KeyUseForm = _KeyUseForm;
                this.TransReq = transReq;
                this.ListHisConfigPaymentQrCode = listHisConfigPaymentQrCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000118PDO(
          V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
          HIS_DHST hisDhst,
          HIS_SERVICE_REQ HisPrescription,
          List<ExpMestMedicineSDO> expMestMedicines,
          HIS_SERVICE_REQ hisServiceReq_Exam,
          HIS_TREATMENT treatment,
          Mps000118ADO mps000118ADO,
          List<HIS_SERE_SERV> listSereServCls,
              long? _KeyUseForm,
              HIS_SERVICE_REQ _hisServiceReq_CurentExam,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listHisConfigPaymentQrCode)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000118ADO = mps000118ADO;
                this.hisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
                this._KeyUseForm = _KeyUseForm;
                this.hisServiceReq_CurentExam = _hisServiceReq_CurentExam;
                this.TransReq = transReq;
                this.ListHisConfigPaymentQrCode = listHisConfigPaymentQrCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000118PDO(
          V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
          HIS_DHST hisDhst,
          HIS_SERVICE_REQ HisPrescription,
          List<ExpMestMedicineSDO> expMestMedicines,
          HIS_SERVICE_REQ hisServiceReq_Exam,
          HIS_TREATMENT treatment,
          Mps000118ADO mps000118ADO,
          List<HIS_SERE_SERV> listSereServCls,
              long? _KeyUseForm,
            HIS_EXP_MEST _hisExpMest,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listHisConfigPaymentQrCode)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000118ADO = mps000118ADO;
                this.hisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
                this._KeyUseForm = _KeyUseForm;
                this.HisExpMest = _hisExpMest;
                this.TransReq = transReq;
                this.ListHisConfigPaymentQrCode = listHisConfigPaymentQrCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000118PDO(
          V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
          HIS_DHST hisDhst,
          HIS_SERVICE_REQ HisPrescription,
          List<ExpMestMedicineSDO> expMestMedicines,
          HIS_SERVICE_REQ hisServiceReq_Exam,
          HIS_TREATMENT treatment,
          Mps000118ADO mps000118ADO,
          List<HIS_SERE_SERV> listSereServCls,
              long? _KeyUseForm,
            HIS_EXP_MEST _hisExpMest,
            HIS_SERVICE_REQ _hisServiceReq_CurentExam,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listHisConfigPaymentQrCode)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.ToList();
                this.hisDhst = hisDhst;
                this.HisPrescription = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.Mps000118ADO = mps000118ADO;
                this.hisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
                this._KeyUseForm = _KeyUseForm;
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
