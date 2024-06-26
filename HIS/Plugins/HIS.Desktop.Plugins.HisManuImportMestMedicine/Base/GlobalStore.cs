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
using Inventec.Core;
using MOS.Filter;

namespace HIS.Desktop.Plugins.HisManuImportMestMedicine.Base
{
    class GlobalStore
    {
        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> ListMediStock;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> HisListMediStocks
        {
            get
            {
                if (ListMediStock == null || ListMediStock.Count == 0)
                {
                    ListMediStock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                return ListMediStock;
            }
            set
            {
                ListMediStock = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY> ListMedicinePaty; 
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY> HisListMedicinePatys
        {
            get
            {
                if (ListMedicinePaty == null || ListMedicinePaty.Count == 0)
                {
                    ListMedicinePaty = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY>().OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                return ListMedicinePaty;
            }
            set
            {
                ListMedicinePaty = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT> ImpMestStt;
        public static List<MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT> HisImpMestStts
        {
            get
            {
                if (ImpMestStt == null || ImpMestStt.Count == 0)
                {
                    ImpMestStt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT>().OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                return ImpMestStt;
            }
            set
            {
                ImpMestStt = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE> ImpMestType;
        public static List<MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE> HisImpMestTypes
        {
            get
            {
                if (ImpMestType == null || ImpMestType.Count == 0)
                {
                    ImpMestType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_IMP_MEST_TYPE>().OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                return ImpMestType;
            }
            set
            {
                ImpMestType = value;
            }
        }
    }
}
