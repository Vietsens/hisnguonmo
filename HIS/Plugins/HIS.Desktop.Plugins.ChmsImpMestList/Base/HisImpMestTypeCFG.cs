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

namespace HIS.Desktop.Plugins.ChmsImpMestList.Base
{
    class HisImpMestTypeCFG
    {
        private static long impMestTypeIdAGGR;
        public static long IMP_MEST_TYPE_ID__AGGR
        {
            get
            {
                if (impMestTypeIdAGGR == 0)
                {
                    impMestTypeIdAGGR = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_IMP_MEST_TYPE__IMP_MEST_TYPE_CODE__AGGR);
                }
                return impMestTypeIdAGGR;
            }
            set
            {
                impMestTypeIdAGGR = value;
            }
        }

        private static long impMestTypeIdCHMS;
        public static long IMP_MEST_TYPE_ID__CHMS
        {
            get
            {
                if (impMestTypeIdCHMS == 0)
                {
                    impMestTypeIdCHMS = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_IMP_MEST_TYPE__IMP_MEST_TYPE_CODE__CHMS);
                }
                return impMestTypeIdCHMS;
            }
            set
            {
                impMestTypeIdCHMS = value;
            }
        }

        private static long impMestTypeIdGIBA;
        public static long IMP_MEST_TYPE_ID__GIBA
        {
            get
            {
                if (impMestTypeIdGIBA == 0)
                {
                    impMestTypeIdGIBA = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_IMP_MEST_TYPE__IMP_MEST_TYPE_CODE__GIBA);
                }
                return impMestTypeIdGIBA;
            }
            set
            {
                impMestTypeIdGIBA = value;
            }
        }

        private static long impMestTypeIdINIT;
        public static long IMP_MEST_TYPE_ID__INIT
        {
            get
            {
                if (impMestTypeIdINIT == 0)
                {
                    impMestTypeIdINIT = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_IMP_MEST_TYPE__IMP_MEST_TYPE_CODE__INIT);
                }
                return impMestTypeIdINIT;
            }
            set
            {
                impMestTypeIdINIT = value;
            }
        }

        private static long impMestTypeIdINVE;
        public static long IMP_MEST_TYPE_ID__INVE
        {
            get
            {
                if (impMestTypeIdINVE == 0)
                {
                    impMestTypeIdINVE = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_IMP_MEST_TYPE__IMP_MEST_TYPE_CODE__INVE);
                }
                return impMestTypeIdINVE;
            }
            set
            {
                impMestTypeIdINVE = value;
            }
        }

        private static long impMestTypeIdMOBA;
        public static long IMP_MEST_TYPE_ID__MOBA
        {
            get
            {
                if (impMestTypeIdMOBA == 0)
                {
                    impMestTypeIdMOBA = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_IMP_MEST_TYPE__IMP_MEST_TYPE_CODE__MOBA);
                }
                return impMestTypeIdMOBA;
            }
            set
            {
                impMestTypeIdMOBA = value;
            }
        }

        private static long impMestTypeIdMANU;
        public static long IMP_MEST_TYPE_ID__MANU
        {
            get
            {
                if (impMestTypeIdMANU == 0)
                {
                    impMestTypeIdMANU = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_IMP_MEST_TYPE__IMP_MEST_TYPE_CODE__MANU);
                }
                return impMestTypeIdMANU;
            }
            set
            {
                impMestTypeIdMANU = value;
            }
        }

        private static long impMestTypeIdOTHER;
        public static long IMP_MEST_TYPE_ID__OTHER
        {
            get
            {
                if (impMestTypeIdOTHER == 0)
                {
                    impMestTypeIdOTHER = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_IMP_MEST_TYPE__IMP_MEST_TYPE_CODE__OTHER);
                }
                return impMestTypeIdOTHER;
            }
            set
            {
                impMestTypeIdOTHER = value;
            }
        }

        //private static long impMestTypeIdPRES;
        //public static long IMP_MEST_TYPE_ID__PRES
        //{
        //    get
        //    {
        //        if (impMestTypeIdPRES == 0)
        //        {
        //            impMestTypeIdPRES = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_IMP_MEST_TYPE__IMP_MEST_TYPE_CODE__PRES);
        //        }
        //        return impMestTypeIdPRES;
        //    }
        //    set
        //    {
        //        impMestTypeIdPRES = value;
        //    }
        //}

        //private static long impMestTypeIdSALE;
        //public static long IMP_MEST_TYPE_ID__SALE
        //{
        //    get
        //    {
        //        if (impMestTypeIdSALE == 0)
        //        {
        //            impMestTypeIdSALE = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_IMP_MEST_TYPE__IMP_MEST_TYPE_CODE__SALE);
        //        }
        //        return impMestTypeIdSALE;
        //    }
        //    set
        //    {
        //        impMestTypeIdSALE = value;
        //    }
        //}

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

                var data = HIS.Desktop.Plugins.ChmsImpMestList.Base.GlobalStore.HisImpMestTypes.FirstOrDefault(o => o.IMP_MEST_TYPE_CODE == value);
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
