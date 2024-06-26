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
using MPS.Processor.Mps000483.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000483
{
     class Mps000483Processor : AbstractProcessor
    {
        Mps000483PDO rdo;
        public Mps000483Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000483PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                //SetBarcodeKey();
                ProcessSingleKey();

                singleTag.ProcessData(store, singleValueDictionary);
                singleTag.ProcessData(store, rdo._SingKey483.DIC_OTHER_KEY);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "Report", rdo._Mps000483RDOs);
                objectTag.AddObjectData(store, "ListBlood", rdo._ListBloods);
                objectTag.AddObjectData(store, "ReportByPackage", rdo._BloodByPakages);
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void ProcessSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<SingKey483>(rdo._SingKey483, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
