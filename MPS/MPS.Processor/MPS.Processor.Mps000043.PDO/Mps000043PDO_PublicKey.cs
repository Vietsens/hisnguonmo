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
using MOS.SDO;

namespace MPS.Processor.Mps000043.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000043PDO : RDOBase
    {
        //public MPS.ADO.HisServiceReqCombo hisServiceReqCombo { get; set; }
        //public List<MPS.ADO.SereServGroupPlusSDO> sereServs_All { get; set; }
        //public List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> lstSereServResult { get; set; }
        //public string departmentName;
        public PatientADO patientADO { get; set; }
        public PatyAlterBhytADO patyAlterBhytADO { get; set; }
        public string bebRoomName;
        public string resultCreateMedicine;
        public List<MedicineExpmestTypeADO> lstMedicineExpmestTypeADO { get; set; }
        public HIS_DEPARTMENT department { get; set; }
        public HisPrescriptionSDO HisPrescriptionSDO { get; set; }
        public HIS_ICD dataIcd { get; set; }
        public HIS_DHST dhst { get; set; }
        public string mediStockExportName { get; set; }
        public V_HIS_TREATMENT treatment { get; set; }
    }
    public class PatientADO : MOS.EFMODEL.DataModels.V_HIS_PATIENT
    {
        public string AGE { get; set; }
        public string DOB_STR { get; set; }
        public string CMND_DATE_STR { get; set; }
        public string DOB_YEAR { get; set; }
        public string GENDER_MALE { get; set; }
        public string GENDER_FEMALE { get; set; }

        public PatientADO()
        {

        }

        public PatientADO(V_HIS_PATIENT data)
        {
            try
            {
                if (data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_PATIENT>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, item.GetValue(data));
                    }

                    this.AGE = AgeUtil.CalculateFullAge(this.DOB);
                    this.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.DOB);
                    string temp = this.DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        this.DOB_YEAR = temp.Substring(0, 4);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class PatyAlterBhytADO : V_HIS_PATY_ALTER_BHYT
    {
        public string PATIENT_TYPE_NAME { get; set; }
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
    public class MedicineExpmestTypeADO
    {
        public string MEDICINE_TYPE_NAME { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string MEDICINE_ACTIVE { get; set; }//hoat chat
        public string MEDICINE_CONCENTRATION { get; set; }//ham luong
        public string AMOUNT { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string TUTORIAL { get; set; }
        public long NUM_ORDER { get; set; }
    }
    public class HisPrescriptionSDO
    {
        //public HisPrescriptionSDO();
        public string Advise { get; set; }
        public long? ExecuteGroupId { get; set; }
        public HIS_EXP_MEST ExpMest { get; set; }
        public List<HisExpMestBltySDO> ExpMestBlties { get; set; }
        public List<HIS_EXP_MEST_MATY> ExpMestMaties { get; set; }
        public List<HIS_EXP_MEST_METY> ExpMestMeties { get; set; }
        public List<HIS_EXP_MEST_OTHER> ExpMestOthers { get; set; }
        public long? IcdId { get; set; }
        public string IcdMainText { get; set; }
        public string IcdSubCode { get; set; }
        public string IcdText { get; set; }
        public long InstructionTime { get; set; }
        public List<long> InstructionTimes { get; set; }
        public long? MediStockId { get; set; }
        public long? ParentServiceReqId { get; set; }
        public long PatientId { get; set; }
        public List<HisPrescriptionMaterialSDO> PrescriptionMaterials { get; set; }
        public List<HisPrescriptionMedicineSDO> PrescriptionMedicines { get; set; }
        public long? RemedyCount { get; set; }
        public string RequestLoginName { get; set; }
        public long RequestRoomId { get; set; }
        public string RequestUserName { get; set; }
        public long TreatmentId { get; set; }
        public long? UseTime { get; set; }
    }
}
