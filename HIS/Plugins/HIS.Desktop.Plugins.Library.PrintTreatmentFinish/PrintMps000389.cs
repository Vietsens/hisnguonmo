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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTreatmentFinish
{
    class PrintMps000389
    {
         MPS.Processor.Mps000389.PDO.Mps000389PDO mps000389RDO { get; set; }

        public PrintMps000389(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.V_HIS_PATIENT HisPatient, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, bool _printNow, long? roomId)
        {
            try
            {
                if (HisTreatment == null || HisTreatment.ID <= 0)
                {
                    result = false;
                    return;
                }

                mps000389RDO = new MPS.Processor.Mps000389.PDO.Mps000389PDO(
                   HisTreatment
                   );

                result = Print.RunPrint(printTypeCode, fileName, mps000389RDO, null
                    , result, _printNow, roomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //public PrintMps000389(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.V_HIS_PATIENT HisPatient, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, MPS.ProcessorBase.PrintConfig.PreviewType? _previewType, long? roomId)
        //{
        //    try
        //    {
        //        if (HisTreatment == null || HisTreatment.ID <= 0)
        //        {
        //            result = false;
        //            return;
        //        }

        //        mps000389RDO = new MPS.Processor.Mps000389.PDO.Mps000389PDO(
        //           HisTreatment
        //           );

        //        result = Print.RunPrint(printTypeCode, fileName, mps000389RDO, null
        //            , result, _previewType, roomId);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
    }
}
