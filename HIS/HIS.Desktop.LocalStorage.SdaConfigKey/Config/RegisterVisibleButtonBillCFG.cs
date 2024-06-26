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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class RegisterVisibleButtonBillCFG
    {
        private const string SERVICE_REQUEST_REGISTER__IS_PRINT_AFTER_SAVE__KEY = "EXE.SERVICE_REQUEST_REGISTER.IS_PRINT_AFTER_SAVE";
        private const string SERVICE_REQUEST_REGISTER__IS_VISIBLE_BILL__KEY = "EXE.SERVICE_REQUEST_REGISTER.IS_VISIBLE_BILL";

        private static string serviceRequestRegisterIsPrintAfterSave;
        public static string SERVICE_REQUEST_REGISTER__IS_PRINT_AFTER_SAVE
        {
            get
            {
                if (serviceRequestRegisterIsPrintAfterSave == null)
                {
                    serviceRequestRegisterIsPrintAfterSave = GetValue(SERVICE_REQUEST_REGISTER__IS_PRINT_AFTER_SAVE__KEY);
                }
                return serviceRequestRegisterIsPrintAfterSave;
            }
            set
            {
                serviceRequestRegisterIsPrintAfterSave = value;
            }
        }

        private static string serviceRequestRegisterIsVisibleBill;
        public static string SERVICE_REQUEST_REGISTER__IS_VISIBLE_BILL
        {
            get
            {
                if (serviceRequestRegisterIsVisibleBill == null)
                {
                    serviceRequestRegisterIsVisibleBill = GetValue(SERVICE_REQUEST_REGISTER__IS_VISIBLE_BILL__KEY);
                }
                return serviceRequestRegisterIsVisibleBill;
            }
            set
            {
                serviceRequestRegisterIsVisibleBill = value;
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                SDA.EFMODEL.DataModels.SDA_CONFIG config = Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                result = String.IsNullOrEmpty(config.VALUE) ? (String.IsNullOrEmpty(config.DEFAULT_VALUE) ? "" : config.DEFAULT_VALUE) : config.VALUE;
                if (String.IsNullOrEmpty(result)) throw new ArgumentNullException(code);
                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

    }
}
