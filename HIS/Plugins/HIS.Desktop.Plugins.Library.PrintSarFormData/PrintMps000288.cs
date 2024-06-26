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
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintSarFormData
{
    class PrintMps000288
    {
        MPS.Processor.Mps000288.PDO.Mps000288PDO mps000288RDO { get; set; }

        public PrintMps000288(string printTypeCode, string fileName, ref bool result, List<SAR_FORM_DATA> _sarFormDatas, bool _printNow, long? roomId)
        {
            try
            {
                if (_sarFormDatas != null && _sarFormDatas.Count > 0)
                {
                    mps000288RDO = new MPS.Processor.Mps000288.PDO.Mps000288PDO(
                      _sarFormDatas);

                    result = Print.RunPrint(printTypeCode, fileName, mps000288RDO, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, _printNow, roomId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EventLogPrint()
        {
            try
            {
                string message = "In phiếu kiểm điểm tử vong. Mã in : Mps000288" + "  TREATMENT_CODE: " + EmrDataStore.treatmentCode + "  Thời gian in: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người in: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
