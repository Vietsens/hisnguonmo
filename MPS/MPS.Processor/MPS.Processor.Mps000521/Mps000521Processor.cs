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
using MPS.Processor.Mps000521.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;

// The namespace must stay exactly "MPS.Processor.Mps000521": ProcessorFactory derives the print type code from it
namespace MPS.Processor.Mps000521
{
    /// <summary>
    /// Thẻ barcode tài khoản lấy mẫu (việc 60557). The barcode is drawn by the template itself: an image named
    /// &lt;#FlFuncCreateBarcode(&lt;#BARCODE_TEXT;&gt;;500;100)&gt; (CODE128; width 500 fits up to 41 characters).
    /// Single card: plain keys of the first employee. Several cards: band __Employees__.
    /// </summary>
    public class Mps000521Processor : AbstractProcessor
    {
        Mps000521PDO rdo;

        public Mps000521Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000521PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                List<Mps000521ADO> employees = (rdo != null && rdo.Employees != null) ? rdo.Employees : new List<Mps000521ADO>();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                new Inventec.Common.FlexCellExport.ProcessObjectTag().AddObjectData(store, "Employees", employees);

                Mps000521ADO first = employees.FirstOrDefault();
                if (first != null)
                    AddObjectKeyIntoListkey<Mps000521ADO>(first);
                new Inventec.Common.FlexCellExport.ProcessSingleTag().ProcessData(store, singleValueDictionary);
                result = true;
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
