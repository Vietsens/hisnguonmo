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

namespace MPS.Processor.Mps000517
{
    /// <summary>
    /// Tầng gom nhóm "Loại xét nghiệm" (Huyết học / Sinh hóa / Miễn dịch / Vi sinh...).
    /// Nằm giữa ServiceParent (nhóm DV cha) và ListTestParent (dịch vụ) trong template.
    /// </summary>
    class TestTypeADO
    {
        /// <summary>Khóa duy nhất theo cặp (nhóm DV cha, loại XN). Nối với ListTestParent.GROUP_ID.</summary>
        public string GROUP_ID { get; set; }
        /// <summary>ID nhóm DV cha. Nối với ServiceParent.ID.</summary>
        public long SERVICE_PARENT_ID { get; set; }
        public long? TEST_TYPE_ID { get; set; }
        public string TEST_TYPE_CODE { get; set; }
        public string TEST_TYPE_NAME { get; set; }
        public long? NUM_ORDER { get; set; }
    }
}
