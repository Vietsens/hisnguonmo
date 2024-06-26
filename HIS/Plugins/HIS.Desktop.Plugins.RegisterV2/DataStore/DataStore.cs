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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.RegisterV2.DataStore
{
    public class DataStore
    {
        internal static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> TranPatiForms { get; set; }
        internal static List<MOS.EFMODEL.DataModels.HIS_ICD> Icds { get; set; }

        public static void LoadDataStore()
        {
            try
            {
                TranPatiForms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>();
                Icds = BackendDataWorker.Get<HIS_ICD>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
