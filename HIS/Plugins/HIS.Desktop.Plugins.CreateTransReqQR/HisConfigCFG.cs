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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CreateTransReqQR
{
    internal class HisConfigCFG
    {
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
       

        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;


        private const string CONFIG__ShowServiceByRoom = "HIS.Desktop.Plugins.CreateTransReqQR.ShowServiceByRoom";
        private const string CONFIG_KEY__ShowServiceBhyt = "HIS.Desktop.Plugins.CreateTransReqQR.ShowServiceBhyt";
        private const string CONFIG_KEY__TransactionBillSelect = "HIS.Desktop.TransactionBillSelect";
        private const string CONFIG_KEY__BILL_TWO_BOOK = "MOS.HIS_TRANSACTION.BILL_TWO_BOOK.OPTION";
        private const string AUTO_PRINT_TYPE = "HIS.Desktop.Plugins.TransactionBill.ElectronicBill.AutoPrintType";
        private const string ELECTRONIC_BILL__PRINT_NUM_COPY = "CONFIG_KEY__HIS_DESKTOP__ELECTRONIC_BILL__PRINT_NUM_COPY";
        private const string PlatformOptionCFG = "Inventec.Common.DocumentViewer.PlatformOption";
        private const string ElectronicInvoicePublishingDelayTimeCFG = "HIS.Desktop.Plugins.TransactionBill.ElectronicInvoicePublishingDelayTime";

        internal static string TransactionBillSelect;
        internal static string BillTwoOption;
        internal static bool ShowServiceBhyt;
        internal static string ShowServiceByRoomOption;
        internal static string autoPrintType;
        internal static int E_BILL__PRINT_NUM_COPY;
        internal static int PlatformOption;
        internal static decimal ElectronicInvoicePublishingDelayTime;
        internal static void LoadConfig()
        {
            try
            {
                PlatformOption = HisConfigs.Get<int>(PlatformOptionCFG);
                E_BILL__PRINT_NUM_COPY = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>(ELECTRONIC_BILL__PRINT_NUM_COPY);
                string delayTime = HisConfigs.Get<string>(ElectronicInvoicePublishingDelayTimeCFG);
                ElectronicInvoicePublishingDelayTime = Decimal.Parse(delayTime, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                autoPrintType = HisConfigs.Get<string>(AUTO_PRINT_TYPE);
                TransactionBillSelect = GetValue(CONFIG_KEY__TransactionBillSelect);
                BillTwoOption = GetValue(CONFIG_KEY__BILL_TWO_BOOK);
                ShowServiceBhyt = GetValue(CONFIG_KEY__ShowServiceBhyt) == "1";
                ShowServiceByRoomOption = GetValue(CONFIG__ShowServiceByRoom);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        static MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetPatientTypeByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
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
