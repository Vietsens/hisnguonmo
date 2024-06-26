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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionList.Config
{
    internal class HisConfigCFG
    {
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong 
        private const string CONFIG_KEY__TRANSACTION_BILL_SELECT = "HIS.Desktop.TransactionBillSelect";//thanh toan 2 hay 1 so
        private const string CFG_TRANSACTION_BILL_TWO_BOOK__OPTION = "MOS.HIS_TRANSACTION.BILL_TWO_BOOK.OPTION";
        private const string CFG__TRANSACTION__ALLOW_EDIT_NUM_ORDER = "MOS.HIS_TRANSACTION.NUM_ORDER.ALLOW_EDIT";
        private const string CFG__TRANSACTION_QR_PAYMENT_STATUS_OPTION = "MOS.HIS_TRANSACTION.QR_PAYMENT.STATUS_OPTION";

        private const string HIS_DESKTOP_TRANSACTION_LIST_SHOW_TRANS_OF_OTHER_OPTION = "HIS.DESKTOP.TRANSACTION_LIST.SHOW_TRANS_OF_OTHER_OPTION";
        private const string Key__InvoiceTypeCreate = "HIS.Desktop.ElectronicBill.Type";

        private const string TRANSACTION_CANCEL__ALLOW_OTHER_LOGINNAME = "HIS.HIS_TRANSACTION.TRANSACTION_CANCEL.ALLOW_OTHER_LOGINNAME";

        private const string ELECTRONIC_BILL_CANCEL_OPTION = "HIS.Desktop.Plugins.Transaction.ElectronicBill.Cancel_Option";

        private const string TRANSACTION_CANCEL__UNCANCEL_OPTION = "HIS.DESKTOP.TRANSACTION_LIST.UNCANCEL_OPTION";

        private const string ELECTRONIC_BILL__PRINT_NUM_COPY = "CONFIG_KEY__HIS_DESKTOP__ELECTRONIC_BILL__PRINT_NUM_COPY";

        private const string PlatformOptionCFG = "Inventec.Common.DocumentViewer.PlatformOption";

        private const string AUTO_PRINT_TYPE = "HIS.Desktop.Plugins.TransactionBill.ElectronicBill.AutoPrintType";

        private const string ALLOW_WHEN_REQUEST = "HIS.HIS_TRANSACTION.TRANSACTION_CANCEL.ALLOW_WHEN_REQUEST";

        private const string ELECTRONIC_BILL__EXPORT_OPTION = "HIS.Desktop.Plugins.TransactionList.ElectronicBill.ExportOption";

        /// <summary>
        /// Cấu hình chế độ tạo hóa đơn điện tử, chữ ký điện tử
        //- Đặt 1: Chỉ tạo hóa đơn điện tử trên hệ thống của vnpt, không tạo trên hệ thống HIS
        //- Đặt 2: Tạo giao dịch trên hệ thống HIS, tự tạo hóa đơn + ký điện tử trên hóa đơn lưu trên hệ thống HIS
        //- Mặc định là ẩn chức năng đi
        /// </summary>

        internal enum BILL_OPTION
        {
            CTO_TW = 1,
            HCM_115 = 2,
            QBH_CUBA = 3,
        }

        internal static string IsEditTransactionTimeCFG;

        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;
        internal static string TransactionBillSelect;
        internal static int BILL_TWO_BOOK__OPTION;
        internal static string ALLOW_EDIT_NUM_ORDER;
        public static string InvoiceTypeCreate;
        public static string TRANSACTION_LIST_SHOW_TRANS_OF_OTHER_OPTION;
        public static string ALLOW_OTHER_LOGINNAME;
        public static string UNCANCEL_OPTION;
        public static string Cancel_Option;

        internal static int E_BILL__PRINT_NUM_COPY;
        internal static int PlatformOption;
        internal static string autoPrintType;
        internal static string AllowWhenRequest;
        internal static string TransactionQrPaymentStatusOption;
        internal static string ElectronicBillExportOption;

        internal static void LoadConfig()
        {
            try
            {
                LogSystem.Debug("LoadConfig => 1");

                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                TransactionBillSelect = GetValue(CONFIG_KEY__TRANSACTION_BILL_SELECT);
                BILL_TWO_BOOK__OPTION = HisConfigs.Get<int>(CFG_TRANSACTION_BILL_TWO_BOOK__OPTION);
                ALLOW_EDIT_NUM_ORDER = GetValue(CFG__TRANSACTION__ALLOW_EDIT_NUM_ORDER);
                TRANSACTION_LIST_SHOW_TRANS_OF_OTHER_OPTION = GetValue(HIS_DESKTOP_TRANSACTION_LIST_SHOW_TRANS_OF_OTHER_OPTION);
                InvoiceTypeCreate = GetValue(Key__InvoiceTypeCreate);
                ALLOW_OTHER_LOGINNAME = GetValue(TRANSACTION_CANCEL__ALLOW_OTHER_LOGINNAME);
                UNCANCEL_OPTION = GetValue(TRANSACTION_CANCEL__UNCANCEL_OPTION);
                E_BILL__PRINT_NUM_COPY = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>(ELECTRONIC_BILL__PRINT_NUM_COPY);
                PlatformOption = HisConfigs.Get<int>(PlatformOptionCFG);
                autoPrintType = HisConfigs.Get<string>(AUTO_PRINT_TYPE);
                Cancel_Option = HisConfigs.Get<string>(ELECTRONIC_BILL_CANCEL_OPTION);
                AllowWhenRequest = HisConfigs.Get<string>(ALLOW_WHEN_REQUEST);
                TransactionQrPaymentStatusOption = GetValue(CFG__TRANSACTION_QR_PAYMENT_STATUS_OPTION);
                ElectronicBillExportOption = GetValue(ELECTRONIC_BILL__EXPORT_OPTION);
                LogSystem.Debug("LoadConfig => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        static HIS_PATIENT_TYPE GetPatientTypeByCode(string code)
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
