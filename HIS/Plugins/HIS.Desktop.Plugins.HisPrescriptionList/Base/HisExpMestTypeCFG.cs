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
using Inventec.Common.Adapter;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using SDA.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisPrescriptionList.Base
{
    class HisExpMestTypeCFG
    {
        private static long expMestTypeIdAGGR;
        public static long EXP_MEST_TYPE_ID__AGGR
        {
            get
            {
                if (expMestTypeIdAGGR == 0)
                {
                    expMestTypeIdAGGR = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_TYPE__EXP_MEST_TYPE_CODE__AGGR);
                }
                return expMestTypeIdAGGR;
            }
            set
            {
                expMestTypeIdAGGR = value;
            }
        }

        private static long expMestTypeIdCHMS;
        public static long EXP_MEST_TYPE_ID__CHMS
        {
            get
            {
                if (expMestTypeIdCHMS == 0)
                {
                    expMestTypeIdCHMS = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_TYPE__EXP_MEST_TYPE_CODE__CHMS);
                }
                return expMestTypeIdCHMS;
            }
            set
            {
                expMestTypeIdCHMS = value;
            }
        }

        private static long expMestTypeIdDEPA;
        public static long EXP_MEST_TYPE_ID__DEPA
        {
            get
            {
                if (expMestTypeIdDEPA == 0)
                {
                    expMestTypeIdDEPA = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_TYPE__EXP_MEST_TYPE_CODE__DEPA);
                }
                return expMestTypeIdDEPA;
            }
            set
            {
                expMestTypeIdDEPA = value;
            }
        }

        private static long expMestTypeIdEXPE;
        public static long EXP_MEST_TYPE_ID__EXPE
        {
            get
            {
                if (expMestTypeIdEXPE == 0)
                {
                    expMestTypeIdEXPE = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_TYPE__EXP_MEST_TYPE_CODE__EXPE);
                }
                return expMestTypeIdEXPE;
            }
            set
            {
                expMestTypeIdEXPE = value;
            }
        }

        private static long expMestTypeIdLIQU;
        public static long EXP_MEST_TYPE_ID__LIQU
        {
            get
            {
                if (expMestTypeIdLIQU == 0)
                {
                    expMestTypeIdLIQU = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_TYPE__EXP_MEST_TYPE_CODE__LIQU);
                }
                return expMestTypeIdLIQU;
            }
            set
            {
                expMestTypeIdLIQU = value;
            }
        }

        private static long expMestTypeIdLOST;
        public static long EXP_MEST_TYPE_ID__LOST
        {
            get
            {
                if (expMestTypeIdLOST == 0)
                {
                    expMestTypeIdLOST = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_TYPE__EXP_MEST_TYPE_CODE__LOST);
                }
                return expMestTypeIdLOST;
            }
            set
            {
                expMestTypeIdLOST = value;
            }
        }

        private static long expMestTypeIdMANU;
        public static long EXP_MEST_TYPE_ID__MANU
        {
            get
            {
                if (expMestTypeIdMANU == 0)
                {
                    expMestTypeIdMANU = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_TYPE__EXP_MEST_TYPE_CODE__MANU);
                }
                return expMestTypeIdMANU;
            }
            set
            {
                expMestTypeIdMANU = value;
            }
        }

        private static long expMestTypeIdOTHER;
        public static long EXP_MEST_TYPE_ID__OTHER
        {
            get
            {
                if (expMestTypeIdOTHER == 0)
                {
                    expMestTypeIdOTHER = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_TYPE__EXP_MEST_TYPE_CODE__OTHER);
                }
                return expMestTypeIdOTHER;
            }
            set
            {
                expMestTypeIdOTHER = value;
            }
        }

        private static long expMestTypeIdPRES;
        public static long EXP_MEST_TYPE_ID__PRES
        {
            get
            {
                if (expMestTypeIdPRES == 0)
                {
                    expMestTypeIdPRES = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_TYPE__EXP_MEST_TYPE_CODE__PRES);
                }
                return expMestTypeIdPRES;
            }
            set
            {
                expMestTypeIdPRES = value;
            }
        }

        private static long expMestTypeIdSALE;
        public static long EXP_MEST_TYPE_ID__SALE
        {
            get
            {
                if (expMestTypeIdSALE == 0)
                {
                    expMestTypeIdSALE = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_TYPE__EXP_MEST_TYPE_CODE__SALE);
                }
                return expMestTypeIdSALE;
            }
            set
            {
                expMestTypeIdSALE = value;
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                CommonParam param = new CommonParam();
                SDA_CONFIG config = ConfigLoader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                string value = String.IsNullOrEmpty(config.VALUE) ? (String.IsNullOrEmpty(config.DEFAULT_VALUE) ? "" : config.DEFAULT_VALUE) : config.VALUE;
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);

                var data = HIS.Desktop.Plugins.HisPrescriptionList.Base.GlobalStore.HisExpMestTypes.FirstOrDefault(o => o.EXP_MEST_TYPE_CODE == value);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => config), config));
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
