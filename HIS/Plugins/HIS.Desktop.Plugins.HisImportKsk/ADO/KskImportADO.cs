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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisImportKsk.ADO
{
    public class KskImportADO : V_HIS_PATIENT
    {
        public string KSK_CODE { get; set; }
        public string KSK_NAME { get; set; }
        public long KSK_ID { get; set; }

        public string INTRUCTION_TIME_STR { get; set; }
        public long INTRUCTION_TIME { get; set; }
        public string YEAR_DOB { get; set; }
        public string DOB_STR { get; set; }
        public string CMND_CCCD { get; set; }

        public string KSK_ORDER_STR { get; set; }
        public long? KSK_ORDER { get; set; }

        public string ERROR { get; set; }
        public string BARCODE { get; set; }

        public string AdditionKskId_STR { get; set; }
        public long? AdditionKskId { get; set; }

        public string HRM_EMPLOYEE_CODE_STR { get; set; }
        public string HRM_KSK_CODE_STR { get; set; }

        public string AdditionServiceIds_STR { get; set; }
        public List<long> AdditionServiceIds { get; set; }

        public long? militaryRankId { get; set; }
        public string MILITARY_RANK_CODE_STR { get; set; }
        public string MILITARY_RANK_NAME_STR { get; set; }
        public long? positionId { get; set; }
        public string POSITION_CODE_STR { get; set; }
        public string POSITION_NAME_STR { get; set; }
        public long? patientClassifyId { get; set; }
        public string PATIENT_CLASSIFY_CODE_STR { get; set; }
        public string PATIENT_CLASSIFY_NAME_STR { get; set; }
        public long? careerId { get; set; }
        public string CAREER_CODE_STR { get; set; }
        public string CAREER_NAME_STR { get; set; }
        public string HT_ADDRESS_STR { get; set; }
        public string ICD_CODE { get; set; }
        public string ICD_CODE_STR { get; set; }
        public string ICD_NAME { get; set; }
        public string ICD_SUB_CODE_STR { get; set; }
        public string ICD_SUB_CODE { get; set; }
        public string ICD_TEXT { get; set; }
        public string PROVINCE_CODE_STR { get; set; }
        public string DISTRICT_CODE_STR { get; set; }
        public string COMMUNE_CODE_STR { get; set; }
        public string NATIONAL_CODE_STR { get; set; }
        public long? provinceId { get; set; }
        public long? districtId { get; set; }
        public long? nationalId { get; set; }

        public KskImportADO()
        {
        }

        public KskImportADO(V_HIS_PATIENT data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<KskImportADO>(this, data);
        }

        public KskImportADO(HisKskPatientSDO data, List<HIS_KSK> hisKsk)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<KskImportADO>(this, data.Patient);
                    if (data.Descriptions != null && data.Descriptions.Count > 0)
                    {
                        this.ERROR = string.Join(";", data.Descriptions);
                    }
                    if (hisKsk != null && hisKsk.Count > 0)
                    {
                        var ksk = hisKsk.FirstOrDefault(o => o.ID == data.KskId);
                        if (ksk != null)
                        {
                            this.KSK_ID = ksk.ID;
                            this.KSK_CODE = ksk.KSK_CODE;
                            this.KSK_NAME = ksk.KSK_NAME;
                        }
                    }

                    if (data.IntructionTime > 0)
                    {
                        this.INTRUCTION_TIME = data.IntructionTime;
                        this.INTRUCTION_TIME_STR = data.IntructionTime.ToString();
                    }

                    if (data.Patient.IS_HAS_NOT_DAY_DOB == 1)
                    {
                        this.YEAR_DOB = data.Patient.DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        this.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.Patient.DOB);
                    }

                    this.CMND_CCCD = !string.IsNullOrEmpty(data.Patient.CCCD_NUMBER) ? data.Patient.CCCD_NUMBER : data.Patient.CMND_NUMBER;

                    var gender = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == data.Patient.GENDER_ID);
                    if (gender != null)
                    {
                        this.GENDER_CODE = gender.GENDER_CODE;
                        this.GENDER_NAME = gender.GENDER_NAME;
                        this.GENDER_ID = gender.ID;
                    }

                    //this.AdditionKskId = data.AdditionKskId;
                    //if (data.AdditionServiceIds != null && data.AdditionServiceIds.Count > 0)
                    //{
                    //    this.AdditionServiceIds = data.AdditionServiceIds;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
