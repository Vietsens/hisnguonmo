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
using EMR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.EmrSign.ADO
{
    class SignADO : V_EMR_SIGN
    {
        public int Action { get; set; }
        public string SIGN_TIME_STR { get; set; }
        public string REJECT_TIME_STR { get; set; }
        public long? DEPARTMENT_ID { get; set; }
        public bool IsPatient { get; set; }
        public long IdRow { get; set; }

        public object Signer { get; set; }

        public SignADO()
        {

        }

        public SignADO(V_EMR_SIGN data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SignADO>(this, data);
                this.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                this.SIGN_TIME_STR = data.SIGN_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.SIGN_TIME.Value) : "";
                this.REJECT_TIME_STR = data.REJECT_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.REJECT_TIME.Value) : "";
                if (!String.IsNullOrWhiteSpace(data.DEPARTMENT_CODE))
                {
                    var department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().FirstOrDefault(o => o.DEPARTMENT_CODE.ToLower() == data.DEPARTMENT_CODE.ToLower());
                    if (department != null)
                    {
                        this.DEPARTMENT_ID = department.ID;
                    }
                }
                if (this.FLOW_ID.HasValue)
                {
                    this.Signer = this.FLOW_NAME;
                }
                else if (!String.IsNullOrWhiteSpace(this.LOGINNAME))
                {
                    this.Signer = data.LOGINNAME;
                }
                if (!String.IsNullOrWhiteSpace(data.PATIENT_CODE))
                {
                    IsPatient = true;
                    if (!String.IsNullOrWhiteSpace(data.RELATION_NAME))
                    {
                        this.Signer = string.Format("{0}({1})", data.RELATION_PEOPLE_NAME, data.CARD_CODE);
                        this.TITLE = data.RELATION_NAME;
                    }
                    else
                    {
                        this.Signer = data.VIR_PATIENT_NAME;
                    }
                }

            }
        }
    }
}
