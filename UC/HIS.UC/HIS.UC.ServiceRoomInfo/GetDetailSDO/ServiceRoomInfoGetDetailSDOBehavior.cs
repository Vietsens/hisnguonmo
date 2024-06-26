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
using AutoMapper;
using DevExpress.XtraEditors;
using HIS.UC.ServiceRoomInfo.Run;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.UC.ServiceRoomInfo.GetDetailSDO
{
    class ServiceRoomInfoGetDetailSDOBehavior : IServiceRoomInfoGetDetailSDO
    {
        object uc;
        long patientTypeId;

        internal ServiceRoomInfoGetDetailSDOBehavior(CommonParam param, object uc)
        {
            try
            {
                this.uc = uc;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<ServiceReqDetailSDO> IServiceRoomInfoGetDetailSDO.Run()
        {
            List<ServiceReqDetailSDO> datas = new List<ServiceReqDetailSDO>();
            try
            {
                if (this.uc is UCServiceRoomInfo)
                {
                    var ucControl = this.uc as UCServiceRoomInfo;
                    if (ucControl != null)
                    {
                        long serviceId = Inventec.Common.TypeConvert.Parse.ToInt64((ucControl.cboExamServiceType.EditValue ?? 0).ToString());
                        long roomId = Inventec.Common.TypeConvert.Parse.ToInt64((ucControl.cboRoom.EditValue ?? 0).ToString());
                        if (serviceId > 0)
                        {
                            ServiceReqDetailSDO reqDetail = new ServiceReqDetailSDO();
                            reqDetail.ServiceId = serviceId;
                            reqDetail.PatientTypeId = ucControl.currentHisPatientTypeAlter.PATIENT_TYPE_ID;
                            if (roomId > 0)
                                reqDetail.RoomId = roomId;
                            reqDetail.Amount = 1;

                            datas.Add(reqDetail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return datas;
        }
    }
}
