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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT
{
    class ExpMestWorker
    {
        internal static V_HIS_EXP_MEST CreateView(V_HIS_SERVICE_REQ_7 input)
        {
            V_HIS_EXP_MEST result = new V_HIS_EXP_MEST();
            try
            {
                result.ID = (input.EXP_MEST_ID ?? 0);
                result.SERVICE_REQ_ID = input.ID;
                result.EXP_MEST_TYPE_ID = (input.EXP_MEST_TYPE_ID ?? 0);
                result.TDL_PATIENT_ID = input.TDL_PATIENT_ID;
                result.TDL_PATIENT_NAME = input.TDL_PATIENT_NAME;
                result.TDL_PATIENT_DOB = input.TDL_PATIENT_DOB;
                result.TDL_INTRUCTION_TIME = input.INTRUCTION_TIME;
                result.TDL_INTRUCTION_DATE = input.INTRUCTION_DATE;
                result.TDL_TREATMENT_ID = input.TREATMENT_ID;
                result.TDL_TREATMENT_CODE = input.TDL_TREATMENT_CODE;
                result.TDL_PATIENT_GENDER_ID = input.TDL_PATIENT_GENDER_ID;
                result.TDL_PATIENT_ADDRESS = input.TDL_PATIENT_ADDRESS;
                var mediStock = (BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == input.MEDI_STOCK_ID) ?? new V_HIS_MEDI_STOCK());
                result.MEDI_STOCK_ID = input.MEDI_STOCK_ID ?? 0;
                result.MEDI_STOCK_CODE = mediStock.MEDI_STOCK_CODE;
                result.MEDI_STOCK_NAME = mediStock.MEDI_STOCK_NAME;
                var expstt = (BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == input.EXP_MEST_STT_ID) ?? new HIS_EXP_MEST_STT());
                result.EXP_MEST_STT_ID = (input.EXP_MEST_STT_ID ?? 0);
                result.EXP_MEST_STT_CODE = expstt.EXP_MEST_STT_CODE;
                result.EXP_MEST_STT_NAME = expstt.EXP_MEST_STT_NAME;
                var exptype = (BackendDataWorker.Get<HIS_EXP_MEST_TYPE>().FirstOrDefault(o => o.ID == input.EXP_MEST_TYPE_ID) ?? new HIS_EXP_MEST_TYPE());
                result.EXP_MEST_TYPE_CODE = exptype.EXP_MEST_TYPE_CODE;
                result.EXP_MEST_TYPE_NAME = exptype.EXP_MEST_TYPE_NAME;
                result.EXP_MEST_CODE = input.EXP_MEST_CODE;
                result.REQ_USERNAME = input.REQUEST_USERNAME;
                result.REQ_LOGINNAME = input.REQUEST_LOGINNAME;
                var depa = (BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == input.REQUEST_DEPARTMENT_ID) ?? new HIS_DEPARTMENT());
                result.REQ_DEPARTMENT_ID = input.REQUEST_DEPARTMENT_ID;
                result.REQ_DEPARTMENT_CODE = depa.DEPARTMENT_CODE;
                result.REQ_DEPARTMENT_NAME = depa.DEPARTMENT_NAME;
                var room = (BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == input.REQUEST_ROOM_ID) ?? new V_HIS_ROOM());
                result.REQ_ROOM_ID = input.REQUEST_ROOM_ID;
                result.REQ_ROOM_CODE = room.ROOM_CODE;
                result.REQ_ROOM_NAME = room.ROOM_NAME;
                result.CREATE_TIME = input.CREATE_TIME;
                result.CREATOR = input.CREATOR;
                result.FINISH_TIME = input.FINISH_TIME;
                result.DESCRIPTION = input.DESCRIPTION;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
