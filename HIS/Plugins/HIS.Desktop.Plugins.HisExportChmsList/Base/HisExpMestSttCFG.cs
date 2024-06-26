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
using Inventec.Common.LocalStorage.SdaConfig;
using SDA.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisExportMestMedicine.Base
{
    class HisExpMestSttCFG
    {
        private static long expMestSttIdAggregate;
        public static long EXP_MEST_STT_ID__AGGREGATE
        {
            get
            {
                if (expMestSttIdAggregate == 0)
                {
                    expMestSttIdAggregate = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_STT__EXP_MEST_STT_CODE__AGGREGATE);
                }
                return expMestSttIdAggregate;
            }
            set
            {
                expMestSttIdAggregate = value;
            }
        }

        private static long expMestSttIdApproved;
        public static long EXP_MEST_STT_ID__APPROVED
        {
            get
            {
                if (expMestSttIdApproved == 0)
                {
                    expMestSttIdApproved = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_STT__EXP_MEST_STT_CODE__APPROVED);
                }
                return expMestSttIdApproved;
            }
            set
            {
                expMestSttIdApproved = value;
            }
        }

        private static long expMestSttIdDraft;
        public static long EXP_MEST_STT_ID__DRAFT
        {
            get
            {
                if (expMestSttIdDraft == 0)
                {
                    expMestSttIdDraft = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_STT__EXP_MEST_STT_CODE__DRAFT);
                }
                return expMestSttIdDraft;
            }
            set
            {
                expMestSttIdDraft = value;
            }
        }

        private static long expMestSttIdExported;
        public static long EXP_MEST_STT_ID__EXPORTED
        {
            get
            {
                if (expMestSttIdExported == 0)
                {
                    expMestSttIdExported = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_STT__EXP_MEST_STT_CODE__EXPORTED);
                }
                return expMestSttIdExported;
            }
            set
            {
                expMestSttIdExported = value;
            }
        }

        private static long expMestSttIdRejected;
        public static long EXP_MEST_STT_ID__REJECTED
        {
            get
            {
                if (expMestSttIdRejected == 0)
                {
                    expMestSttIdRejected = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_STT__EXP_MEST_STT_CODE__REJECTED);
                }
                return expMestSttIdRejected;
            }
            set
            {
                expMestSttIdRejected = value;
            }
        }

        private static long expMestSttIdRequest;
        public static long EXP_MEST_STT_ID__REQUEST
        {
            get
            {
                if (expMestSttIdRequest == 0)
                {
                    expMestSttIdRequest = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_EXP_MEST_STT__EXP_MEST_STT_CODE__REQUEST);
                }
                return expMestSttIdRequest;
            }
            set
            {
                expMestSttIdRequest = value;
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                SDA_CONFIG config = ConfigLoader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                string value = String.IsNullOrEmpty(config.VALUE) ? (String.IsNullOrEmpty(config.DEFAULT_VALUE) ? "" : config.DEFAULT_VALUE) : config.VALUE;
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);

                var data = GlobalStore.HisExpMestStts.FirstOrDefault(o => o.EXP_MEST_STT_CODE == value);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => config), config));
                result = data.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
