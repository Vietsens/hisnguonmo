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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using System;
using System.Linq;
using Inventec.Common.Adapter;
using System.Collections.Generic;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.LocalStorage.SdaConfig;

namespace HIS.Desktop.Plugins.HisAccountBookList.Base
{
    public class HisTransactionCFG
    {
        private static long transactionTypeCodeBill;
        public static long TRANSACTION_TYPE_ID__BILL
        {
            get
            {
                if (transactionTypeCodeBill == 0)
                {
                    transactionTypeCodeBill = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_TRANSACTION_TYPE__TRANSACTION_TYPE_CODE__BILL);
                }
                return transactionTypeCodeBill;
            }
            set
            {
                transactionTypeCodeBill = value;
            }
        }

        private static long transactionTypeIdDeposit;
        public static long TRANSACTION_TYPE_ID__DEPOSIT
        {
            get
            {
                if (transactionTypeIdDeposit == 0)
                {
                    transactionTypeIdDeposit = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_TRANSACTION_TYPE__TRANSACTION_TYPE_CODE__DEPOSIT);
                }
                return transactionTypeIdDeposit;
            }
            set
            {
                transactionTypeIdDeposit = value;
            }
        }

        private static long transactionTypeIdRepay;
        public static long TRANSACTION_TYPE_ID__REPAY
        {
            get
            {
                if (transactionTypeIdRepay == 0)
                {
                    transactionTypeIdRepay = GetId(ConfigKeys.DBCODE__HIS_RS__HIS_TRANSACTION_TYPE__TRANSACTION_TYPE_CODE__REPAY);
                }
                return transactionTypeIdRepay;
            }
            set
            {
                transactionTypeIdRepay = value;
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                SDA.EFMODEL.DataModels.SDA_CONFIG config = ConfigLoader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                string value = String.IsNullOrEmpty(config.VALUE) ? (String.IsNullOrEmpty(config.DEFAULT_VALUE) ? "" : config.DEFAULT_VALUE) : config.VALUE;
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);
                HisTransactionTypeFilter filter = new HisTransactionTypeFilter();
                //sua lai
                filter.TRANSACTION_TYPE_CODE = value;
                var apiresult = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE>>(HisRequestUriStore.HIS_TRANSACTION_TYPE_GET, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, new CommonParam());
                var data = apiresult.FirstOrDefault();
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code);
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
