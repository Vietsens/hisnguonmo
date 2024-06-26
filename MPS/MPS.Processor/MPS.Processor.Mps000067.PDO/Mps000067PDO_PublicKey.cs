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
using MOS.SDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000067.PDO
{
	public partial class Mps000067PDO : RDOBase
	{
		public PatientADO Patient { get; set; }
		public ExeExpMestMedicineSDO expMestMedicines { get; set; }
		public MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE expMestMedicine { get; set; }
		public HisPrescriptionSDO HisPrescriptionSDO { get; set; }
		public Mps000067PDO(
				PatientADO patient,
				ExeExpMestMedicineSDO expMestMedicines
				)
		{
			try
			{
				this.Patient = patient;
				this.expMestMedicines = expMestMedicines;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
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

	public class ExeExpMestMedicineSDO : MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE
	{
		public int Type { get; set; }//1: thuoc // 2: vat tu, 3: thuoc trong kho, 4: thuoc ngoai kho, 5: tu tuc
		public bool rdoChoose { get; set; }
		public string PatientTypeName { get; set; }
		public long PatientTypeId { get; set; }
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
