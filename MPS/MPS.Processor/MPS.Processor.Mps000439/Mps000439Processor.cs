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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000439.PDO;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000439
{
    class Mps000439Processor : AbstractProcessor
    {
         Mps000439PDO rdo;
         public Mps000439Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000439PDO)rdoBase;
        }
         public override bool ProcessData()
         {
             bool result = false;
             try
             {
                 Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                  SetSingleKey();
                 
                 store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                 singleTag.ProcessData(store, singleValueDictionary);
                
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
                 if (rdo.Transaction != null)
                 {
                     AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo.Transaction, false);
                 }

             }
             catch (Exception ex)
             {
                 Inventec.Common.Logging.LogSystem.Error(ex);
             }
         }
    }
}
