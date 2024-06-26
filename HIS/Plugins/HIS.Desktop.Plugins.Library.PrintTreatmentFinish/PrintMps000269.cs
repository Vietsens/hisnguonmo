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
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTreatmentFinish
{
    class PrintMps000269
    {
        MPS.Processor.Mps000269.PDO.Mps000269PDO mps000269RDO { get; set; }
        bool printNow { get; set; }

        public PrintMps000269(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, HIS_SERE_SERV _sereServExamFirst,MPS.Processor.Mps000269.PDO.Mps000269ADO ado, bool _printNow, long? roomId)
        {
            try
            {
                if (HisTreatment == null || HisTreatment.ID <= 0)
                {
                    result = false;
                    return;
                }
                this.printNow = _printNow;

                mps000269RDO = new MPS.Processor.Mps000269.PDO.Mps000269PDO(
                   HisTreatment, 
                   patientTypeAlter,
                   _sereServExamFirst,
                   ado
                   );

                result = Print.RunPrint(printTypeCode, fileName, mps000269RDO, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, _printNow, roomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool RunPrint(string printTypeCode, string fileName, bool result, bool _printNow)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (_printNow)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000269RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000269RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000269RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return result;
        }

        private void EventLogPrint()
        {
            try
            {
                string message = "In giấy nghỉ ốm. Mã in : Mps000269" + "  TREATMENT_CODE: " + this.mps000269RDO.TreatmentView.TREATMENT_CODE + "  Thời gian in: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người in: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
