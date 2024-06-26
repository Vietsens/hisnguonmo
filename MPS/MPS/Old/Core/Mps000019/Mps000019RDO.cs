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
using MPS.ADO;

namespace MPS.Core.Mps000019
{
    /// <summary>
    /// Trích biên bản hội chẩn
    /// </summary>
    public class Mps000019RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        string bebRoomName;
        string departmentName;
        internal MOS.EFMODEL.DataModels.HIS_DEBATE currentHisDebate { get; set; }
        internal List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = new List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>();
        internal MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        public Mps000019RDO(
            PatientADO patient,
            string bebRoomName,
            string departmentName,
            MOS.EFMODEL.DataModels.HIS_DEBATE currentHisDebate,
            MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran
            )
        {
            try
            {
                this.Patient = patient;
                this.bebRoomName = bebRoomName;
                this.departmentName = departmentName;
                this.currentHisDebate = currentHisDebate;
                this.departmentTran = departmentTran;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);

                if (departmentTran != null)
                {
                    keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTran.LOG_TIME)));
                    keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTran.LOG_TIME)));
                }

                if (this.currentHisDebate != null)
                {
                    lstHisDebateUser = new List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>();
                    if (this.currentHisDebate.HIS_DEBATE_USER != null && this.currentHisDebate.HIS_DEBATE_USER.Count > 0)
                    {
                        //Tìm chủ tọa và thư ký
                        foreach (var item_User in this.currentHisDebate.HIS_DEBATE_USER)
                        {
                            MOS.EFMODEL.DataModels.HIS_DEBATE_USER hisDebateUser = new MOS.EFMODEL.DataModels.HIS_DEBATE_USER();
                            hisDebateUser = item_User;
                            lstHisDebateUser.Add(hisDebateUser);

                            if (item_User.IS_PRESIDENT == 1)
                            {
                                keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.USERNAME_PRESIDENT, item_User.USERNAME));
                                keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.PRESIDENT_DESCRIPTION, item_User.DESCRIPTION));
                            }
                            if (item_User.IS_SECRETARY == 1)
                            {
                                keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.USERNAME_SECRETARY, item_User.USERNAME));
                                keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.SECRETARY_DESCRIPTION, item_User.DESCRIPTION));
                            }
                        }
                        lstHisDebateUser = lstHisDebateUser.Where(o => o.IS_SECRETARY != 1 && o.IS_PRESIDENT != 1).ToList();
                    }

                    keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.BED_ROOM_NAME, bebRoomName));
                    keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.DEPARTMENT_NAME, departmentName));
                    keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.BEFORE_DIAGNOSTIC,
                        currentHisDebate.BEFORE_DIAGNOSTIC));
                    keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.DEBATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentHisDebate.DEBATE_TIME??0)));
                    keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.TREATMENT_TRACKING, currentHisDebate.TREATMENT_TRACKING));
                    keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.CONCLUSION, currentHisDebate.CONCLUSION));
                    keyValues.Add(new KeyValue(Mps000019ExtendSingleKey.TREATMENT_METHOD, currentHisDebate.TREATMENT_METHOD));
                    GlobalQuery.AddObjectKeyIntoListkey<HIS_DEBATE>(currentHisDebate, keyValues);
                }
                    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
