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

namespace HIS.Desktop.Plugins.ImportBlood
{
   internal class HisRequestUri
    {
       public const string HIS_MANU_IMP_MEST_CREATE = "api/HisImpMest/ManuCreate";
       public const string HIS_MANU_IMP_MEST_UPDATE = "api/HisImpMest/ManuUpdate";
       public const string HIS_INIT_IMP_MEST_CREATE = "api/HisImpMest/InitCreate";
       public const string HIS_INIT_IMP_MEST_UPDATE = "api/HisImpMest/InitUpdate";
       public const string HIS_INVE_IMP_MEST_CREATE = "api/HisImpMest/InveCreate";
       public const string HIS_INVE_IMP_MEST_UPDATE = "api/HisImpMest/InveUpdate";
       public const string HIS_OTHER_IMP_MEST_CREATE = "api/HisImpMest/OtherCreate";
       public const string HIS_OTHER_IMP_MEST_UPDATE = "api/HisImpMest/OtherUpdate";
       public const string HIS_DONATION_IMP_MEST_CREATE = "api/HisImpMest/DonationCreate";
       public const string HIS_DONATION_IMP_MEST_UPDATE = "api/HisImpMest/DonationUpdate";

       public const string HIS_HIS_BLTY_VOLUME_GET = "api/HisBltyVolume/Get";
    }
}
