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
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using Inventec.Common.Repository;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData
{
    public partial class BackendDataWorkerSet
    {
        public BackendDataWorkerSet() { }

        public List<T> Set<T>() where T : class
        {
            return Set<T>(false, false);
        }

        public List<T> Set<T>(bool isTranslate, bool isRamOnly) where T : class
        {
            return Set<T>(isTranslate, isRamOnly, false);
        }

        public List<T> Set<T>(bool isTranslate, bool isRamOnly, bool islock) where T : class
        {
            return BackendDataWorker.Get<T>(isTranslate, isRamOnly, islock, true);
        }
    }
}
