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
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Print;

namespace HIS.Desktop.Plugins.RequestDeposit
{
    public partial class UC_RequestDeposit : UserControl
    {
        bool delegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                InYeuCauTamUng(printTypeCode, fileName, depositReqPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InYeuCauTamUng(string printTypeCode, string fileName, V_HIS_DEPOSIT_REQ depositReq)
        {
            try
            {
                bool result = false;

                WaitingManager.Show();

                //Thông tin bệnh nhân
                var patient = PrintGlobalStore.getPatient(treatmentID);
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;

                //BHYT
                var patyAlterBhyt = PrintGlobalStore.getPatyAlterBhyt(treatmentID, instructionTime);
                WaitingManager.Hide();
                MPS.Core.Mps000091.Mps000091RDO mps000091RDO = new MPS.Core.Mps000091.Mps000091RDO(
                                patient,
                                depositReq,
                                patyAlterBhyt
                                );

                result = MPS.Printer.Run(printTypeCode, fileName, mps000091RDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
