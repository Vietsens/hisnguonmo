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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.RegisterVaccination.Run3
{
    public partial class UCRegister : UserControlBase
    {
        private List<HIS_CASHIER_ROOM> GetCashierRoomByUser()
        {
            List<HIS_CASHIER_ROOM> result = new List<HIS_CASHIER_ROOM>();
            try
            {
                //Ci hien thi phong thu ngan ma ng dung chon lam viec
                var roomIds = WorkPlace.GetRoomIds();
                if (roomIds == null || roomIds.Count == 0)
                    throw new ArgumentNullException("Nguoi dung khong chon phong thu ngan nao");
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>().Where(o => roomIds.Contains(o.ROOM_ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void GetPatientInfoFromResultData(ref AssignServiceADO assignServiceADO)
        {
            try
            {
                if (this.resultHisPatientProfileSDO != null)
                {
                    assignServiceADO.PatientName = this.resultHisPatientProfileSDO.HisPatient.VIR_PATIENT_NAME;
                    assignServiceADO.PatientDob = this.resultHisPatientProfileSDO.HisPatient.DOB;
                    assignServiceADO.GenderName = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == this.resultHisPatientProfileSDO.HisPatient.GENDER_ID).GENDER_NAME;
                }
                else if (this.currentHisExamServiceReqResultSDO != null)
                {
                    assignServiceADO.PatientName = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.VIR_PATIENT_NAME;
                    assignServiceADO.PatientDob = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.DOB;
                    assignServiceADO.GenderName = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.GENDER_ID).GENDER_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
