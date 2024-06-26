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
using MPS.ProcessorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000360
{
    class Mps000360ExtendSingleKey : CommonKey
    {
        internal const string CULTURE_RESULT = "CULTURE_RESULT";
        internal const string BACTERIUM_CODE = "BACTERIUM_CODE";
        internal const string BACTERIUM_NAME = "BACTERIUM_NAME";
        internal const string BACTERIUM_FAMILY_CODE = "BACTERIUM_FAMILY_CODE";
        internal const string BACTERIUM_FAMILY_NAME = "BACTERIUM_FAMILY_NAME";
        internal const string IMPLANTION_RESULT = "IMPLANTION_RESULT";
        internal const string MICROCOPY_RESULT = "MICROCOPY_RESULT";
        internal const string BACTERIUM_AMOUNT = "BACTERIUM_AMOUNT";
        internal const string BACTERIUM_DENSITY = "BACTERIUM_DENSITY";
    }
}
