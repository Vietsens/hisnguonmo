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
using MPS.Processor.Mps000097.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000097
{
    class Mps000097Processor : AbstractProcessor
    {
         Mps000097PDO rdo;
        public Mps000097Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000097PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "USER", rdo.ekipUsers);


                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private void SetSingleKey()
        {
            try
            {
                System.DateTime now = System.DateTime.Now;
                SetSingleKey(new KeyValue(Mps000097ExtendSingleKey.NOW_TIME, Inventec.Common.DateTime.Convert.SystemDateTimeToDateSeparateString(now)));
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.vHisTreatment, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.patient);

                if (rdo.vHisSereServPttt != null)
                {
                    AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT>(rdo.vHisSereServPttt);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
