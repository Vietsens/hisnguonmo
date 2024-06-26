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

namespace MPS.Core.Mps000065
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000065RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal DepartmentTranADO departmentTran { get; set; }
        internal List<HIS_DEBATE_USER> lstHisDebateUser { get; set; }
        internal List<HIS_DEBATE_USER> lstHisDebateUserTGia { get; set; }
        internal HIS_DEBATE HisDebateRow { get; set; }
        string bedRoomName;

        public Mps000065RDO(
            PatientADO patient,
            DepartmentTranADO departmentTran,
            List<HIS_DEBATE_USER> lstHisDebateUser,
            List<HIS_DEBATE_USER> lstHisDebateUserTGia,
            HIS_DEBATE HisDebateRow,
            string bedRoomName
            )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.lstHisDebateUser = lstHisDebateUser;
                this.lstHisDebateUserTGia = lstHisDebateUserTGia;
                this.HisDebateRow = HisDebateRow;
                this.bedRoomName = bedRoomName;
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
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> listPresidentSecretary = lstHisDebateUser.Where(o => o.IS_PRESIDENT == 1 || o.IS_SECRETARY == 1).ToList();
                if (listPresidentSecretary!=null)
                {
                    foreach (var item_User in listPresidentSecretary)
                    {
                        if (item_User.IS_PRESIDENT == 1)
                        {
                            keyValues.Add(new KeyValue(Mps000065ExtendSingleKey.USERNAME_PRESIDENT, item_User.USERNAME));
                        }
                        if (item_User.IS_SECRETARY == 1)
                        {
                            keyValues.Add(new KeyValue(Mps000065ExtendSingleKey.USERNAME_SECRETARY, item_User.USERNAME));
                        }
                    }
                }
                if (HisDebateRow!=null)
                {
                    keyValues.Add(new KeyValue(Mps000065ExtendSingleKey.DEBATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(HisDebateRow.DEBATE_TIME ??0)));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000065ExtendSingleKey.DEBATE_TIME_STR, ""));
                }
                GlobalQuery.AddObjectKeyIntoListkey<DepartmentTranADO>(departmentTran, keyValues, false);
                keyValues.Add(new KeyValue(Mps000065ExtendSingleKey.BED_ROOM_NAME, bedRoomName));
                GlobalQuery.AddObjectKeyIntoListkey<HIS_DEBATE>(HisDebateRow, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
