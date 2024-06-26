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

namespace HIS.Desktop.Plugins.AggrExpMestDetail.Base
{
    class GlobalStore
    {
        private static List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE> heinServiceTypes;
        public static List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE> HisHeinServiceTypes
        {
            get
            {
                if (heinServiceTypes == null || heinServiceTypes.Count == 0)
                {
                    heinServiceTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>().OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                return heinServiceTypes;
            }
            set
            {
                heinServiceTypes = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> MediStock;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> ListMediStock
        {
            get
            {
                if (MediStock == null || MediStock.Count == 0)
                {
                    MediStock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where( o => o.IS_PAUSE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                return MediStock;
            }
            set
            {
                MediStock = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY> MedicinePaty;
        public static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY> ListMedicinePaty
        {
            get
            {
                if (MedicinePaty == null || MedicinePaty.Count == 0)
                {
                    MedicinePaty = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_PATY>().OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                return MedicinePaty;
            }
            set
            {
                MedicinePaty = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT> ExpMestStt;
        public static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT> HisExpMestStts
        {
            get
            {
                if (ExpMestStt == null || ExpMestStt.Count == 0)
                {
                    ExpMestStt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT>().OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                return ExpMestStt;
            }
            set
            {
                ExpMestStt = value;
            }
        }

        private static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE> ExpMestType;
        public static List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE> HisExpMestTypes
        {
            get
            {
                if (ExpMestType == null || ExpMestType.Count == 0)
                {
                    ExpMestType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE>().OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                return ExpMestType;
            }
            set
            {
                ExpMestType = value;
            }
        }
    }
}
