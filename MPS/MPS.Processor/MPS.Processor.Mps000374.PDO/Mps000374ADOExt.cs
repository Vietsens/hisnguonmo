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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000374.PDO
{
    public class Mps000374ADOExts
    {
        public Mps000374ADOExts() { }

        public List<Mps000374ExtADO> Mps000374ADOs { get; set; }
        public Mps000374SingleKey WorkPlaceSDO { get; set; }
        public Dictionary<string, object> SingleValueDictionary { get; set; }
    }

    public class NumberDate
    {
        public long INTRUCTION_DATE { get; set; }
        public long MEDICINE_TYPE_ID { get; set; }
        public long MEDICINE_GROUP_ID { get; set; }
        public int num { get; set; }
    }

    public class SingleKeyTracking
    {
        public string TRACKING_DATE_STR { get; set; }
        public string Y_LENH { get; set; }
    }

    public class Mps000374ADO : HIS_TRACKING
    {
        public string TRACKING_TIME_STR { get; set; }

        public string TRACKING_DATE_STR { get; set; }

        public string TRACKING_DATE_SEPARATE_STR { get; set; }

        public long? REMEDY_COUNT { get; set; }

        public string ICD_NAME_TRACKING { get; set; }

        public string IS_SHOW_MEDICINE { get; set; }

        public long? SUM_MEDI_MATE { get; set; }

        public decimal? BELLY { get; set; }
        public long? BLOOD_PRESSURE_MAX { get; set; }
        public long? BLOOD_PRESSURE_MIN { get; set; }
        public decimal? BREATH_RATE { get; set; }
        public decimal? CHEST { get; set; }
        public decimal? HEIGHT { get; set; }
        public long? PULSE { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public decimal? VIR_BMI { get; set; }
        public decimal? VIR_BODY_SURFACE_AREA { get; set; }
        public decimal? WEIGHT { get; set; }
        public decimal? SPO2 { get; set; }
        public decimal? CAPILLARY_BLOOD_GLUCOSE { get; set; }
        public string TRACKING_USERNAME { get; set; }
        public string NOTE { get; set; }
    }

    public class Mps000374ExtADO : HIS_TRACKING
    {
        public Mps000374ExtADO() { }
        public Mps000374ExtADO(Mps000374ADO Mps000374ADO)
        {
            this.TRACKING_TIME_STR = Mps000374ADO.TRACKING_TIME_STR;
            this.TRACKING_DATE_STR = Mps000374ADO.TRACKING_DATE_STR;
            this.TRACKING_DATE_SEPARATE_STR = Mps000374ADO.TRACKING_DATE_SEPARATE_STR;
            this.REMEDY_COUNT = Mps000374ADO.REMEDY_COUNT;
            this.ICD_NAME_TRACKING = Mps000374ADO.ICD_NAME_TRACKING;
            this.IS_SHOW_MEDICINE = Mps000374ADO.IS_SHOW_MEDICINE;
            this.SUM_MEDI_MATE = Mps000374ADO.SUM_MEDI_MATE;
            this.BELLY = Mps000374ADO.BELLY;
            this.BLOOD_PRESSURE_MAX = Mps000374ADO.BLOOD_PRESSURE_MAX;
            this.BLOOD_PRESSURE_MIN = Mps000374ADO.BLOOD_PRESSURE_MIN;
            this.BREATH_RATE = Mps000374ADO.BREATH_RATE;
            this.CHEST = Mps000374ADO.CHEST;
            this.HEIGHT = Mps000374ADO.HEIGHT;
            this.PULSE = Mps000374ADO.PULSE;
            this.TEMPERATURE = Mps000374ADO.TEMPERATURE;
            this.NOTE = Mps000374ADO.NOTE;

            this.VIR_BMI = Mps000374ADO.VIR_BMI;
            this.VIR_BODY_SURFACE_AREA = Mps000374ADO.VIR_BODY_SURFACE_AREA;
            this.WEIGHT = Mps000374ADO.WEIGHT;
            this.SPO2 = Mps000374ADO.SPO2;
            this.CAPILLARY_BLOOD_GLUCOSE = Mps000374ADO.CAPILLARY_BLOOD_GLUCOSE;

            this.AWARENESS_BEHAVIOR = Mps000374ADO.AWARENESS_BEHAVIOR;
            this.CARDIOVASCULAR = Mps000374ADO.CARDIOVASCULAR;
            this.CARE_INSTRUCTION = Mps000374ADO.CARE_INSTRUCTION;
            this.CONCENTRATION = Mps000374ADO.CONCENTRATION;
            this.CONTENT = Mps000374ADO.CONTENT;
            this.CONTENT_OF_THINKING = Mps000374ADO.CONTENT_OF_THINKING;
            this.CREATE_TIME = Mps000374ADO.CREATE_TIME;
            this.CREATOR = Mps000374ADO.CREATOR;
            this.DEPARTMENT_ID = Mps000374ADO.DEPARTMENT_ID;
            this.EMOTION = Mps000374ADO.EMOTION;
            this.FORM_OF_THINKING = Mps000374ADO.FORM_OF_THINKING;
            this.GENERAL_EXPRESSION = Mps000374ADO.GENERAL_EXPRESSION;
            this.ICD_CODE = Mps000374ADO.ICD_CODE;
            this.ICD_NAME = Mps000374ADO.ICD_NAME;
            this.ICD_SUB_CODE = Mps000374ADO.ICD_SUB_CODE;
            this.ICD_TEXT = Mps000374ADO.ICD_TEXT;
            this.ID = Mps000374ADO.ID;
            this.INSTINCTIVELY_BEHAVIOR = Mps000374ADO.INSTINCTIVELY_BEHAVIOR;
            this.INTELLECTUAL = Mps000374ADO.INTELLECTUAL;
            this.IS_ACTIVE = Mps000374ADO.IS_ACTIVE;
            this.IS_DELETE = Mps000374ADO.IS_DELETE;
            this.MEDICAL_INSTRUCTION = Mps000374ADO.MEDICAL_INSTRUCTION;
            this.MEMORY = Mps000374ADO.MEMORY;
            this.MODIFIER = Mps000374ADO.MODIFIER;
            this.MODIFY_TIME = Mps000374ADO.MODIFY_TIME;
            this.ORIENTATION_CAPACITY = Mps000374ADO.ORIENTATION_CAPACITY;
            this.PERCEPTION = Mps000374ADO.PERCEPTION;
            this.RESPIRATORY = Mps000374ADO.RESPIRATORY;
            this.SUBCLINICAL_PROCESSES = Mps000374ADO.SUBCLINICAL_PROCESSES;
            this.TRACKING_TIME = Mps000374ADO.TRACKING_TIME;
            this.TREATMENT_ID = Mps000374ADO.TREATMENT_ID;
        }

        public string TRACKING_TIME_STR { get; set; }
        public string TRACKING_DATE_STR { get; set; }
        public string TRACKING_DATE_SEPARATE_STR { get; set; }
        public long? REMEDY_COUNT { get; set; }
        public string ICD_NAME_TRACKING { get; set; }
        public string IS_SHOW_MEDICINE { get; set; }
        public long? SUM_MEDI_MATE { get; set; }
        public decimal? BELLY { get; set; }
        public long? BLOOD_PRESSURE_MAX { get; set; }
        public long? BLOOD_PRESSURE_MIN { get; set; }
        public decimal? BREATH_RATE { get; set; }
        public decimal? CHEST { get; set; }
        public decimal? HEIGHT { get; set; }
        public long? PULSE { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public decimal? VIR_BMI { get; set; }
        public decimal? VIR_BODY_SURFACE_AREA { get; set; }
        public decimal? WEIGHT { get; set; }
        public decimal? SPO2 { get; set; }
        public decimal? CAPILLARY_BLOOD_GLUCOSE { get; set; }
        public string NOTE { get; set; }

        public List<ExpMestMetyReqADO> ExpMestMetyReqADOs { get; set; }
        public List<ServiceCLS> ServiceCLSs { get; set; }
        public List<ServiceCLS> Bloods { get; set; }
        public List<ServiceCLS> ExamServices { get; set; }
        public List<ServiceCLS> TTServices { get; set; }
        public List<RemedyCountADO> RemedyCountADOs { get; set; }
        public List<ExpMestMatyReqADO> ExpMestMatyReqADOs { get; set; }
        public List<ServiceReqMetyADO> ServiceReqMetyADOs { get; set; }
        public List<ServiceReqMatyADO> ServiceReqMatyADOs { get; set; }
        public List<ImpMestMedicineADO> ImpMestMedicineADOs { get; set; }
        public List<ImpMestMaterialADO> ImpMestMaterialADOs { get; set; }
        public List<MedicalInstruction> MedicalInstructions { get; set; }
        public List<HIS_CARE> Cares { get; set; }
        public List<V_HIS_CARE_DETAIL> CareDetails { get; set; }
        public List<MedicineLineADO> MedicineLineADOs { get; set; }
    }

    public class MedicineLineADO
    {
        public long? ID { get; set; }
        public String MEDICINE_LINE_NAME { get; set; }
        public long? EXP_MEST_ID { get; set; }
        public long? TRACKING_ID { get; set; }
    }

    public class RemedyCountADO
    {
        public long TRACKING_ID { get; set; }
        public long EXP_MEST_ID { get; set; } //exp_mest_id
        public long? REMEDY_COUNT { get; set; }
        public short PRESCRIPTION_TYPE_ID { get; set; }
        public string TUTORIAL_REMEDY { get; set; }
        public long? MEDICINE_LINE_ID { get; set; }
        public long DAY_COUNT { get; set; }
    }

    public class ExpMestMetyReqADO : HIS_EXP_MEST_MEDICINE
    {
        public long TRACKING_ID { get; set; }
        public long? REMEDY_COUNT { get; set; }
        public decimal? Amount_By_Remedy_Count { get; set; }
        public long? NUMBER_H_N { get; set; }
        public long? NUMBER_BY_TYPE { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public long? MEDICINE_USE_FORM_ID { get; set; }
        public long NUM_ORDER_BY_USE_FORM { get; set; }
        public string TUTORIAL_REMEDY { get; set; }
        public string CONCENTRA { get; set; }
        public string GROUP_BHYT { get; set; }
        public string AMOUNT_STR { get; set; }
        public short PRESCRIPTION_TYPE_ID { get; set; }
        public long? MEDICINE_LINE_ID { get; set; }
        public string MEDICINE_LINE_NAME { get; set; }
        public string MEDICINE_LINE_CODE { get; set; }
        public long? MEDICINE_LINE_NUM_ORDER { get; set; }

        public string DESCRIPTION_INTRUCTION_DAYS { get; set; }
        public long? INTRUCTION_DAYS { get; set; }
        public long INTRUCTION_DATE { get; set; }
        public long INTRUCTION_TIME { get; set; }

        public long TYPE_ID { get; set; }
        public string NUMBER_INTRUCTION_DATE { get; set; }

        public ExpMestMetyReqADO() { }
        public ExpMestMetyReqADO(HIS_EXP_MEST_MEDICINE data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMetyReqADO>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ExpMestMatyReqADO : HIS_EXP_MEST_MATERIAL
    {
        public long TRACKING_ID { get; set; }
        public string MATERIAL_TYPE_CODE { get; set; }
        public string MATERIAL_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public string AMOUNT_STR { get; set; }

        public ExpMestMatyReqADO() { }
        public ExpMestMatyReqADO(HIS_EXP_MEST_MATERIAL data)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMatyReqADO>(this, data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ServiceReqMetyADO : HIS_SERVICE_REQ_METY
    {
        public long TRACKING_ID { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }

        public ServiceReqMetyADO() { }
        public ServiceReqMetyADO(HIS_SERVICE_REQ_METY data, long trackingId)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqMetyADO>(this, data);
                    this.SERVICE_UNIT_NAME = data.UNIT_NAME;
                    this.TRACKING_ID = trackingId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ServiceReqMatyADO : HIS_SERVICE_REQ_MATY
    {
        public long TRACKING_ID { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }

        public ServiceReqMatyADO() { }
        public ServiceReqMatyADO(HIS_SERVICE_REQ_MATY data, long trackingId)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqMatyADO>(this, data);
                    this.SERVICE_UNIT_NAME = data.UNIT_NAME;
                    this.TRACKING_ID = trackingId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class MedicalInstruction
    {
        public long TRACKING_ID { get; set; }
        public string MEDICAL_INSTRUCTION { get; set; }
    }

    public class ServiceCLS : HIS_SERE_SERV
    {
        public long TRACKING_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public string INSTRUCTION_NOTE { get; set; }

        public ServiceCLS() { }

        public ServiceCLS(HIS_SERE_SERV data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceCLS>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ImpMestMedicineADO : V_HIS_IMP_MEST_MEDICINE
    {
        public long TRACKING_ID { get; set; }

        public ImpMestMedicineADO() { }

        public ImpMestMedicineADO(V_HIS_IMP_MEST_MEDICINE data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ImpMestMedicineADO>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ImpMestMaterialADO : V_HIS_IMP_MEST_MATERIAL
    {
        public long TRACKING_ID { get; set; }

        public ImpMestMaterialADO() { }

        public ImpMestMaterialADO(V_HIS_IMP_MEST_MATERIAL data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ImpMestMaterialADO>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }


    // 007
    public class PatientADO : V_HIS_PATIENT
    {
        public string VIR_ADDRESS { get; set; }
        public string VIR_PATIENT_NAME { get; set; }
        public long DOB { get; set; }
        public string AGE { get; set; }
        public string DOB_STR { get; set; }
        public string CMND_DATE_STR { get; set; }
        public string DOB_YEAR { get; set; }
        public string GENDER_MALE { get; set; }
        public string GENDER_FEMALE { get; set; }
    }

    public class PatyAlterBhytADO : V_HIS_PATIENT_TYPE_ALTER
    {
        public string HEIN_CARD_NUMBER_SEPARATE { get; set; }
        public string IS_HEIN { get; set; }
        public string IS_VIENPHI { get; set; }
        public string STR_HEIN_CARD_FROM_TIME { get; set; }
        public string STR_HEIN_CARD_TO_TIME { get; set; }
        public string RATIO { get; set; }
        public string HEIN_CARD_NUMBER_1 { get; set; }
        public string HEIN_CARD_NUMBER_2 { get; set; }
        public string HEIN_CARD_NUMBER_3 { get; set; }
        public string HEIN_CARD_NUMBER_4 { get; set; }
        public string HEIN_CARD_NUMBER_5 { get; set; }
        public string HEIN_CARD_NUMBER_6 { get; set; }
        public long TIME_IN_TREATMENT { get; set; }
    }

    public class ExpMestBloods
    {
        public string BLOOD_TYPE_NAME { get; set; }
        public decimal VOLUME { get; set; }
        public decimal AMOUNT { get; set; }
        public string DESCRIPTION { get; set; }
    }
}
