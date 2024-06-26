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
using MPS.Processor.Mps000200.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000200
{
  public class Mps000200Processor : AbstractProcessor
  {
    Mps000200PDO rdo;

    public Mps000200Processor(CommonParam param, PrintData printData)
      : base(param, printData)
    {
      rdo = (Mps000200PDO)rdoBase;
    }

    /// <summary>
    /// Ham xu ly du lieu da qua xu ly
    /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
    /// </summary>
    /// <returns></returns>
    public override bool ProcessData()
    {
      bool result = false;
      try
      {
        Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
        Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
        Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

        store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
        singleTag.ProcessData(store, singleValueDictionary);
        if (rdo.lstMedicine != null && rdo.lstMedicine.Count > 0)
        {
            List<ADO.MedicineTypeAdo> listMedicineADO = new List<ADO.MedicineTypeAdo>();
            foreach (var medicine in rdo.lstMedicine)
            {
                ADO.MedicineTypeAdo ado = new ADO.MedicineTypeAdo(medicine);
                listMedicineADO.Add(ado);
            }
            objectTag.AddObjectData(store, "ListMedicine", listMedicineADO);
            result = true;
        }
      }
      catch (Exception ex)
      {
        result = false;
        Inventec.Common.Logging.LogSystem.Error(ex);
      }

      return result;
    }
  }
}
