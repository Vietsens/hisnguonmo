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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentFinish.Save
{
    class SaveFactory
    {
        internal static ISave MakeISave(long RoomId,
            long? ServiceReqId,
            bool isSave,
            HIS_TREATMENT currentVHisTreatment,
            HisTreatmentFinishSDO hisTreatmentFinishSDO_process,
            FormTreatmentFinish Form)
        {
            ISave result = null;
            try
            {
                if (hisTreatmentFinishSDO_process != null && hisTreatmentFinishSDO_process.TreatmentEndTypeId > 0)
                {
                    if (hisTreatmentFinishSDO_process.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                    {
                        result = new Transfer.SaveTransfreBehavior(RoomId, ServiceReqId, isSave, currentVHisTreatment, hisTreatmentFinishSDO_process, Form);
                    }
                    else if (hisTreatmentFinishSDO_process.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                    {
                        result = new Appointment.SaveAppointmentBehavior(RoomId, ServiceReqId, isSave, currentVHisTreatment, hisTreatmentFinishSDO_process, Form);
                    }
                    else if (hisTreatmentFinishSDO_process.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                    {
                        result = new Death.SaveDeathBehavior(RoomId, ServiceReqId, isSave, currentVHisTreatment, hisTreatmentFinishSDO_process, Form);
                    }
                    else if (hisTreatmentFinishSDO_process.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM || hisTreatmentFinishSDO_process.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                    {
                        result = new Sick.SaveSickBehavior(RoomId, ServiceReqId, isSave, currentVHisTreatment, hisTreatmentFinishSDO_process, Form);
                    }
                    else
                    {
                        result = new Other.SaveOtherBehavior(RoomId, ServiceReqId, isSave, currentVHisTreatment, hisTreatmentFinishSDO_process, Form);
                    }
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + hisTreatmentFinishSDO_process.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisTreatmentFinishSDO_process), hisTreatmentFinishSDO_process), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
