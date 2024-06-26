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
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.TransactionCancel.Config
{
    internal class HisConfigCFG
    {
        private const string CONFIG_KEY__TRAN_BILL_SELECT = "HIS.Desktop.TransactionBillSelect";
        private const string CONFIG_KEY__TRAN_BILL_OTHER_AUTOCANCEL = "HIS.Desktop.Plugins.Transaction.BillOther.AutoCancel";
        private const string CONFIG_KEY__CASHIER_ROOM_PAYMENT_OPTION = "MOS.EPAYMENT.CASHIER_ROOM_PAYMENT_OPTION";
        private const string CONFIG_KEY__ALLOW_WHEN_REQUEST = "HIS.HIS_TRANSACTION.TRANSACTION_CANCEL.ALLOW_WHEN_REQUEST";

        internal static string TransactionBill__Select;
        internal static string TransactionBill__AutoCancel;
        internal static string TransactionBill__CashierRoomPaymentOption;
        internal static string TransactionBill__AllowWhenRequest;
        internal static void LoadConfig()
        {
            try
            {
                TransactionBill__Select = GetValue(CONFIG_KEY__TRAN_BILL_SELECT);
                TransactionBill__AutoCancel = GetValue(CONFIG_KEY__TRAN_BILL_OTHER_AUTOCANCEL);
                TransactionBill__CashierRoomPaymentOption = GetValue(CONFIG_KEY__CASHIER_ROOM_PAYMENT_OPTION);
                TransactionBill__AllowWhenRequest = GetValue(CONFIG_KEY__ALLOW_WHEN_REQUEST);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private static string GetValue(string key)
        {
            try
            {
                return HisConfigs.Get<string>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return "";
        }
    }
   
}
