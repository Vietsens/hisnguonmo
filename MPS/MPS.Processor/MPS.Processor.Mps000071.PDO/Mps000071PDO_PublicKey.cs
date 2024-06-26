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

namespace MPS.Processor.Mps000071.PDO
{
    public partial class Mps000071PDO : RDOBase
    {
        public PatientADO patientADO { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER { get; set; }
        public List<ExeSereServSdo> sereServExamServiceReqs { get; set; }
        public V_HIS_SERE_SERV sereServExamSerivceReq { get; set; }
        public V_HIS_SERVICE_REQ vHisServiceReq { get; set; }
    }

    public class PatientADO : MOS.EFMODEL.DataModels.V_HIS_PATIENT
    {
        public string AGE { get; set; }
        public string DOB_STR { get; set; }
        public string CMND_DATE_STR { get; set; }
        public string DOB_YEAR { get; set; }
        public string GENDER_MALE { get; set; }
        public string GENDER_FEMALE { get; set; }

        public PatientADO() { }

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
                    //this.CMND_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.CMND_DATE ?? 0);

                    if (this.GENDER_CODE == Inventec.Common.LocalStorage.SdaConfig.SdaConfigs.Get<string>(Inventec.Common.LocalStorage.SdaConfig.ConfigKeys.DBCODE__HIS_RS__HIS_GENDER__GENDER_CODE__FEMALE))
                    {
                        this.GENDER_MALE = "";
                        this.GENDER_FEMALE = "X";
                    }
                    else
                    {
                        this.GENDER_MALE = "X";
                        this.GENDER_FEMALE = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ExeSereServSdo : V_HIS_SERE_SERV
    {
        public string INSTRUCTION_TIME_STR { get; set; }
        public long? NUM_ORDER_EXAM { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public long? SERVICE_NUM_ORDER { get; set; }

        public ExeSereServSdo() { }

        public ExeSereServSdo(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_11 sereserv, MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ serviceReqExam)
        {
            if (sereserv != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(this, sereserv);

                this.SERVICE_CODE = sereserv.TDL_SERVICE_CODE;
                this.SERVICE_NAME = sereserv.TDL_SERVICE_NAME;
                this.REQUEST_LOGINNAME = sereserv.TDL_REQUEST_LOGINNAME;
                this.REQUEST_USERNAME = sereserv.TDL_REQUEST_USERNAME;
                this.NUM_ORDER_EXAM = serviceReqExam.NUM_ORDER;
                this.REQUEST_DEPARTMENT_CODE = serviceReqExam.REQUEST_DEPARTMENT_CODE;
                this.REQUEST_DEPARTMENT_NAME = serviceReqExam.REQUEST_DEPARTMENT_NAME;
                this.REQUEST_LOGINNAME = serviceReqExam.REQUEST_LOGINNAME;
                this.REQUEST_ROOM_CODE = serviceReqExam.REQUEST_ROOM_CODE;
                this.REQUEST_ROOM_NAME = serviceReqExam.REQUEST_ROOM_NAME;
                this.REQUEST_USERNAME = serviceReqExam.REQUEST_USERNAME;
            }
        }

        public ExeSereServSdo(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV sereserv, MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ serviceReqExam)
        {
            if (sereserv != null)
            {

                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, item.GetValue(sereserv));
                }

                this.NUM_ORDER_EXAM = serviceReqExam.NUM_ORDER;
                this.SERVICE_CODE = sereserv.TDL_SERVICE_CODE;
                this.SERVICE_NAME = sereserv.TDL_SERVICE_NAME;
                this.REQUEST_LOGINNAME = sereserv.TDL_REQUEST_LOGINNAME;
                this.REQUEST_USERNAME = sereserv.TDL_REQUEST_USERNAME;
            }
        }
    }
}
