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

namespace MPS.Processor.Mps000181.PDO
{
    public class Mps000181PDO : RDOBase
    {
        public const string PrintTypeCode = "Mps000181";

        public V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        public HIS_SERVICE_REQ vHisPrescription5 { get; set; }
        public List<ExpMestMedicineSDO> expMestMedicines { get; set; }
        public HIS_DHST hisDhst { get; set; }
        public HIS_SERVICE_REQ hisServiceReq_Exam { get; set; }
        public Mps000181ADO Mps000181ADO { get; set; }
        public HIS_TREATMENT HisTreatment { get; set; }
        public List<HIS_SERE_SERV> ListSereServCls { get; set; }
        public long? KeyUseForm;
        public HIS_EXP_MEST HisExpMest { get; set; }
        public HIS_TRANS_REQ TransReq { get; set; }
        public List<HIS_CONFIG> ListHisConfigPaymentQrCode { get; set; }

        public Mps000181PDO(
          V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
          HIS_DHST hisDhst,
          HIS_SERVICE_REQ HisPrescription,
          List<ExpMestMedicineSDO> expMestMedicines,
          HIS_SERVICE_REQ hisServiceReq_Exam,
          HIS_TREATMENT treatment,
          Mps000181ADO mps000181ADO,
          List<HIS_SERE_SERV> listSereServCls,
             long? _KeyUseForm)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.ToList();
                this.hisDhst = hisDhst;
                this.vHisPrescription5 = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.PatyAlterBhyt = vHisPatientTypeAlter;
                this.Mps000181ADO = mps000181ADO;
                this.HisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
                this.KeyUseForm = _KeyUseForm;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000181PDO(
          V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
          HIS_DHST hisDhst,
          HIS_SERVICE_REQ HisPrescription,
          List<ExpMestMedicineSDO> expMestMedicines,
          HIS_SERVICE_REQ hisServiceReq_Exam,
          HIS_TREATMENT treatment,
          Mps000181ADO mps000181ADO,
          List<HIS_SERE_SERV> listSereServCls,
             long? _KeyUseForm,
            HIS_EXP_MEST _hisExpMest)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.ToList();
                this.hisDhst = hisDhst;
                this.vHisPrescription5 = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.PatyAlterBhyt = vHisPatientTypeAlter;
                this.Mps000181ADO = mps000181ADO;
                this.HisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
                this.KeyUseForm = _KeyUseForm;
                this.HisExpMest = _hisExpMest;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000181PDO(
          V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
          HIS_DHST hisDhst,
          HIS_SERVICE_REQ HisPrescription,
          List<ExpMestMedicineSDO> expMestMedicines,
          HIS_SERVICE_REQ hisServiceReq_Exam,
          HIS_TREATMENT treatment,
          Mps000181ADO mps000181ADO,
          List<HIS_SERE_SERV> listSereServCls,
          long? _KeyUseForm,
          HIS_TRANS_REQ transReq,
          List<HIS_CONFIG> listHisConfigPaymentQrCode)
        {
            try
            {
                this.expMestMedicines = expMestMedicines.ToList();
                this.hisDhst = hisDhst;
                this.vHisPrescription5 = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.PatyAlterBhyt = vHisPatientTypeAlter;
                this.Mps000181ADO = mps000181ADO;
                this.HisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
                this.KeyUseForm = _KeyUseForm;
                this.TransReq = transReq;
                this.ListHisConfigPaymentQrCode = listHisConfigPaymentQrCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000181PDO(
          V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
          HIS_DHST hisDhst,
          HIS_SERVICE_REQ HisPrescription,
          List<ExpMestMedicineSDO> expMestMedicines,
          HIS_SERVICE_REQ hisServiceReq_Exam,
          HIS_TREATMENT treatment,
          Mps000181ADO mps000181ADO,
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
                this.vHisPrescription5 = HisPrescription;
                this.hisServiceReq_Exam = hisServiceReq_Exam;
                this.PatyAlterBhyt = vHisPatientTypeAlter;
                this.Mps000181ADO = mps000181ADO;
                this.HisTreatment = treatment;
                this.ListSereServCls = listSereServCls;
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
    }

    public class Mps000181ADO : V_HIS_PATIENT
    {
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string TITLE { get; set; }
        public string REQUEST_PHONE { get; set; }
        public string EXECUTE_PHONE { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }

    public class ExpMestMedicineSDO : V_HIS_EXP_MEST_MEDICINE
    {
        public short? IS_ADDICTIVE { get; set; }
        public short? IS_NEUROLOGICAL { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public int Type { get; set; }//1: thuoc // 2: vat tu, 3: thuoc trong kho, 4: thuoc ngoai kho, 5: tu tuc
        public decimal? PRES_AMOUNT { get; set; }
        public decimal? USING_COUNT_NUMBER { get; set; }

    }
}
