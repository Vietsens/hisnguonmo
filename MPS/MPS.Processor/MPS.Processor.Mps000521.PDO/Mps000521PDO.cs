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
using MPS.ProcessorBase.Core;
using System.Collections.Generic;

namespace MPS.Processor.Mps000521.PDO
{
    /// <summary>
    /// Thẻ barcode tài khoản lấy mẫu (việc 60557). A list so that several cards can be printed at once later
    /// without changing the PDO; the employee screen passes one card.
    /// </summary>
    public class Mps000521PDO : RDOBase
    {
        public List<Mps000521ADO> Employees { get; set; }

        public Mps000521PDO(List<Mps000521ADO> employees)
        {
            this.Employees = employees;
        }
    }
}
