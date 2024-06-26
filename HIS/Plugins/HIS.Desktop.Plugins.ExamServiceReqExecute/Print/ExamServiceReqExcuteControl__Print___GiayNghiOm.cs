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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt;
using HIS.Desktop.Plugins.Library.PrintTreatmentFinish;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private void LoadBieuMauGiayNghiOm(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                PrintTreatmentEndTypeExtProcessor printTreatmentEndTypeExtProcessor = new PrintTreatmentEndTypeExtProcessor(this.treatmentId, ReloadMenuTreatmentEndTypeExt, CreateMenu.TYPE.DYNAMIC, currentModuleBase != null ? currentModuleBase.RoomId : 0);

                printTreatmentEndTypeExtProcessor.Print(HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.Base.PrintTreatmentEndTypeExPrintType.TYPE.NGHI_OM, PrintTreatmentEndTypeExtProcessor.OPTION.PRINT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBieuMauGiayHenMo(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                PrintTreatmentEndTypeExtProcessor printTreatmentEndTypeExtProcessor = new PrintTreatmentEndTypeExtProcessor(this.treatmentId, ReloadMenuTreatmentEndTypeExt, CreateMenu.TYPE.DYNAMIC, currentModuleBase != null ? currentModuleBase.RoomId : 0);

                printTreatmentEndTypeExtProcessor.Print(HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.Base.PrintTreatmentEndTypeExPrintType.TYPE.HEN_MO, PrintTreatmentEndTypeExtProcessor.OPTION.PRINT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
