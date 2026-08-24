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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisImpMestMediMate.Base
{
    class GlobalStore
    {
        private static List<V_HIS_MEDI_STOCK> mediStocks;
        internal static List<V_HIS_MEDI_STOCK> MediStocks
        {
            get
            {
                if (mediStocks == null || mediStocks.Count == 0)
                {
                    mediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                }
                return mediStocks ?? new List<V_HIS_MEDI_STOCK>();
            }
        }

        private static List<V_HIS_MEDICINE_TYPE> medicineTypes;
        internal static List<V_HIS_MEDICINE_TYPE> MedicineTypes
        {
            get
            {
                if (medicineTypes == null || medicineTypes.Count == 0)
                {
                    medicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                }
                return medicineTypes ?? new List<V_HIS_MEDICINE_TYPE>();
            }
        }

        private static List<V_HIS_MATERIAL_TYPE> materialTypes;
        internal static List<V_HIS_MATERIAL_TYPE> MaterialTypes
        {
            get
            {
                if (materialTypes == null || materialTypes.Count == 0)
                {
                    materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                }
                return materialTypes ?? new List<V_HIS_MATERIAL_TYPE>();
            }
        }

        private static List<HIS_IMP_SOURCE> impSources;
        /// <summary>
        /// Danh muc Nguon nhap. Khong nam trong bo du lieu cache san tren may tram
        /// nen lay 1 lan qua API roi giu lai trong phien lam viec.
        /// </summary>
        internal static List<HIS_IMP_SOURCE> ImpSources
        {
            get
            {
                if (impSources == null)
                {
                    impSources = LoadImpSources();
                }
                return impSources;
            }
        }

        private static List<HIS_IMP_SOURCE> LoadImpSources()
        {
            var result = new List<HIS_IMP_SOURCE>();
            try
            {
                CommonParam param = new CommonParam();
                HisImpSourceFilter filter = new HisImpSourceFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_IMP_SOURCE>>(
                    MediMateRequestUriStore.HIS_IMP_SOURCE_GET, ApiConsumers.MosConsumer, filter, param);
                if (data != null && data.Count > 0)
                {
                    result = data.ToList();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
