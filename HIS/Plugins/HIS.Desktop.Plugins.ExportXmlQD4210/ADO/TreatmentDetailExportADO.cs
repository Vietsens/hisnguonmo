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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD4210.ADO
{
    public class TreatmentDetailExportADO : V_HIS_SERE_SERV_2
    {
        public TreatmentDetailExportADO(V_HIS_SERE_SERV_2 sereServ, V_HIS_TREATMENT_1 treatment)
        {
            try
            {
                // map treatment trước để ghi đè thông tin khi map sereServ sau
                if (treatment != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<TreatmentDetailExportADO>(this, treatment);
                    this.TREATMENT_ICD_CODE = treatment.ICD_CODE;
                    this.TREATMENT_ICD_NAME = treatment.ICD_NAME;
                    this.TREATMENT_ICD_SUB_CODE = treatment.ICD_SUB_CODE;
                    this.TREATMENT_ICD_TEXT = treatment.ICD_TEXT;
                }

                if (sereServ != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<TreatmentDetailExportADO>(this, sereServ);

                    var reqdepa = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == sereServ.REQUEST_DEPARTMENT_ID);
                    if (reqdepa != null)
                    {
                        this.REQUEST_DEPARTMENT_CODE = reqdepa.DEPARTMENT_CODE;
                        this.REQUEST_DEPARTMENT_NAME = reqdepa.DEPARTMENT_NAME;
                        this.REQUEST_DEPARTMENT_BHYT_CODE = reqdepa.BHYT_CODE;
                    }

                    var exedepa = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == sereServ.TDL_EXECUTE_DEPARTMENT_ID);
                    if (exedepa != null)
                    {
                        this.EXECUTE_DEPARTMENT_CODE = exedepa.DEPARTMENT_CODE;
                        this.EXECUTE_DEPARTMENT_NAME = exedepa.DEPARTMENT_NAME;
                        this.EXECUTE_DEPARTMENT_BHYT_CODE = exedepa.BHYT_CODE;
                    }

                    var reqroom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == sereServ.TDL_REQUEST_ROOM_ID);
                    if (reqroom != null)
                    {
                        this.REQUEST_ROOM_CODE = reqroom.ROOM_CODE;
                        this.REQUEST_ROOM_NAME = reqroom.ROOM_NAME;
                    }

                    if (!String.IsNullOrWhiteSpace(sereServ.JSON_PATIENT_TYPE_ALTER))
                    {
                        var patiAlter = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(sereServ.JSON_PATIENT_TYPE_ALTER);
                        if (patiAlter != null)
                        {
                            this.HEIN_CARD_NUMBER = patiAlter.HEIN_CARD_NUMBER;
                            this.HEIN_CARD_FROM_TIME = patiAlter.HEIN_CARD_FROM_TIME;
                            this.HEIN_CARD_TO_TIME = patiAlter.HEIN_CARD_TO_TIME;
                            this.HEIN_MEDI_ORG_CODE = patiAlter.HEIN_MEDI_ORG_CODE;
                            this.HEIN_MEDI_ORG_NAME = patiAlter.HEIN_MEDI_ORG_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string END_CODE { get; set; }
        public string END_DEPARTMENT_CODE { get; set; }
        public string END_DEPARTMENT_NAME { get; set; }
        public string END_LOGINNAME { get; set; }
        public string END_USERNAME { get; set; }
        public string EXTRA_END_CODE { get; set; }
        public string ICD_CAUSE_CODE { get; set; }
        public string ICD_CAUSE_NAME { get; set; }
        public string IN_CODE { get; set; }
        public long IN_DATE { get; set; }
        public string IN_ICD_CODE { get; set; }
        public string IN_ICD_NAME { get; set; }
        public string IN_ICD_SUB_CODE { get; set; }
        public string IN_ICD_TEXT { get; set; }
        public long IN_TIME { get; set; }
        public string IN_USERNAME { get; set; }
        public string MEDI_ORG_CODE { get; set; }
        public string MEDI_ORG_NAME { get; set; }
        public string OUT_CODE { get; set; }
        public long? OUT_DATE { get; set; }
        public long? OUT_TIME { get; set; }
        public string STORE_CODE { get; set; }
        public long? STORE_TIME { get; set; }
        public string TDL_HEIN_CARD_NUMBER { get; set; }
        public string TDL_HEIN_MEDI_ORG_CODE { get; set; }
        public string TDL_HEIN_MEDI_ORG_NAME { get; set; }
        public string TDL_PATIENT_ADDRESS { get; set; }
        public string TDL_PATIENT_CAREER_NAME { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public string TDL_PATIENT_COMMUNE_CODE { get; set; }
        public string TDL_PATIENT_DISTRICT_CODE { get; set; }
        public long TDL_PATIENT_DOB { get; set; }
        public string TDL_PATIENT_FIRST_NAME { get; set; }
        public long TDL_PATIENT_GENDER_ID { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public short? TDL_PATIENT_IS_HAS_NOT_DAY_DOB { get; set; }
        public string TDL_PATIENT_LAST_NAME { get; set; }
        public string TDL_PATIENT_MILITARY_RANK_NAME { get; set; }
        public string TDL_PATIENT_MOBILE { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public string TDL_PATIENT_NATIONAL_NAME { get; set; }
        public string TDL_PATIENT_PHONE { get; set; }
        public string TDL_PATIENT_PROVINCE_CODE { get; set; }
        public string TDL_PATIENT_RELATIVE_NAME { get; set; }
        public string TDL_PATIENT_RELATIVE_TYPE { get; set; }
        public string TDL_PATIENT_TAX_CODE { get; set; }
        public string TDL_PATIENT_WORK_PLACE { get; set; }
        public string TDL_PATIENT_WORK_PLACE_NAME { get; set; }
        public string TRANSFER_IN_CODE { get; set; }
        public string TRANSFER_IN_ICD_CODE { get; set; }
        public string TRANSFER_IN_ICD_NAME { get; set; }
        public string TRANSFER_IN_MEDI_ORG_CODE { get; set; }
        public string TRANSFER_IN_MEDI_ORG_NAME { get; set; }
        public long? TRANSFER_IN_TIME_FROM { get; set; }
        public long? TRANSFER_IN_TIME_TO { get; set; }
        public string TRANSPORT_VEHICLE { get; set; }
        public string TRANSPORTER { get; set; }
        public string TREATMENT_CODE { get; set; }
        public decimal? TREATMENT_DAY_COUNT { get; set; }

        public string IN_DEPARTMENT_ICD_CODE { get; set; }
        public string IN_DEPARTMENT_ICD_NAME { get; set; }
        public string IN_DEPARTMENT_ICD_SUB_CODE { get; set; }
        public string IN_DEPARTMENT_ICD_TEXT { get; set; }
        public string OUT_DEPARTMENT_ICD_CODE { get; set; }
        public string OUT_DEPARTMENT_ICD_NAME { get; set; }
        public string OUT_DEPARTMENT_ICD_SUB_CODE { get; set; }
        public string OUT_DEPARTMENT_ICD_TEXT { get; set; }
        public string TREATMENT_ICD_CODE { get; set; }
        public string TREATMENT_ICD_NAME { get; set; }
        public string TREATMENT_ICD_SUB_CODE { get; set; }
        public string TREATMENT_ICD_TEXT { get; set; }

        public string REQUEST_DEPARTMENT_CODE { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string REQUEST_DEPARTMENT_BHYT_CODE { get; set; }
        public string EXECUTE_DEPARTMENT_CODE { get; set; }
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_DEPARTMENT_BHYT_CODE { get; set; }

        public long? HEIN_CARD_FROM_TIME { get; set; }
        public long? HEIN_CARD_TO_TIME { get; set; }
        public string HEIN_MEDI_ORG_CODE { get; set; }
        public string HEIN_MEDI_ORG_NAME { get; set; }
        public string REQUEST_ROOM_CODE { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
    }
}
