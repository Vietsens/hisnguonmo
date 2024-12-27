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
using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisExportMestMedicine.Base
{
    class HisConfigCFG
    {
        /// <summary>
        /// Phải thanh toán mới cho phép thực xuất phiếu xuất bán
        /// </summary>
        private const string CONFIG_KEY__EXP_MEST_SALE__MUST_BILL = "MOS.EXP_MEST.EXPORT_SALE.MUST_BILL";
        private const string CONFIG_KEY__ALLOW_THER_LOGINNAME = "HIS.HIS_TRANSACTION.TRANSACTION_CANCEL.ALLOW_OTHER_LOGINNAME";
        private const string EXP_MEST_SALE__MODULE_UPDATE_OPTION = "HIS.Desktop.Plugins.ExpMestSale.ModuleUpdateOption";
        private const string CONFIG_KEY__MUST_CONFIRM_BEFORE_APPROVE = "MOS.HIS_EXP_MEST.MUST_CONFIRM_BEFORE_APPROVE.OPTION";
        private const string CONFIG_KEY__WARM_MODIFIEDPRESCRIPTIONOPTION = "HIS.Desktop.Plugins.WarningModifiedPrescriptionOption";
        private const string CONFIG_KEY__IS_REASON_REQUIRED =
        "MOS.EXP_MEST.IS_REASON_REQUIRED";
        private const string CONFIG_KEY__DO_NOT_ALLOW_MOBA_HAS_TRANSACTION_SALE_EXP_MEST =
        "MOS.HIS_EXP_MEST.DO_NOT_ALLOW_MOBA_HAS_TRANSACTION_SALE_EXP_MEST";
        private const string CONFIG_KEY__CALL_PATIENT_FORMAT = "HIS.EXP_MEST.CALL_PATIENT_FORMAT";
        private const string CONFIG_KEY__APPROVAL_OR_EXP_OR_IMP_LOGINNAME_OPTION = "HIS.Desktop.Plugins.HisExportMestMedicine.ApprovalOrExpOrImpLoginNameOption";
        private const string ELECTRONIC_BILL__PRINT_NUM_COPY = "CONFIG_KEY__HIS_DESKTOP__ELECTRONIC_BILL__PRINT_NUM_COPY";
        private const string PlatformOptionCFG = "Inventec.Common.DocumentViewer.PlatformOption";

        private const string ALLOW_EDIT_EXP_TIME = "HIS.Desktop.Plugins.HisExportMestMedicine.AllowToEditFinishTimeOutPres";

        private const string AUTO_PRINT_TYPE = "HIS.Desktop.Plugins.TransactionBill.ElectronicBill.AutoPrintType";
        private const string ENABLE_BUTTON_DELETE = "HIS.Desktop.Plugins.HisExportMestMedicine.EnableButtonDelete";
        internal static bool EXPORT_SALE__MUST_BILL;
        internal static bool CANCEL_ALLOW_OTHER_LOGINNAME;
        internal static string EXP_MEST_SALE__MODULE_UPDATE_OPTION_SELECT;
        internal static string MUST_CONFIRM_BEFORE_APPROVE;
        internal static string WARM_MODIFIEDPRESCRIPTIONOPTION;
        internal static string DO_NOT_ALLOW_MOBA_HAS_TRANSACTION_SALE_EXP_MEST;
        public static string IS_REASON_REQUIRED;
        public static string CALL_PATIENT_FORMAT;
        public static string APPROVAL_OR_EXP_OR_IMP_LOGINNAME_OPTION;

        internal static int PlatformOption;
        internal static int E_BILL__PRINT_NUM_COPY;
        internal static string EnableButtonDelete;
        internal static string autoPrintType;

        internal static string AllowEditExpTime;

        internal static void LoadConfig()
        {
            try
            {
                EnableButtonDelete = HisConfigs.Get<string>(ENABLE_BUTTON_DELETE);
                IS_REASON_REQUIRED = HisConfigs.Get<string>(CONFIG_KEY__IS_REASON_REQUIRED);
                EXPORT_SALE__MUST_BILL = HisConfigs.Get<string>(CONFIG_KEY__EXP_MEST_SALE__MUST_BILL) == "1";
                CANCEL_ALLOW_OTHER_LOGINNAME = HisConfigs.Get<string>(CONFIG_KEY__ALLOW_THER_LOGINNAME) == "1";
                EXP_MEST_SALE__MODULE_UPDATE_OPTION_SELECT = HisConfigs.Get<string>(EXP_MEST_SALE__MODULE_UPDATE_OPTION);
                MUST_CONFIRM_BEFORE_APPROVE = HisConfigs.Get<string>(CONFIG_KEY__MUST_CONFIRM_BEFORE_APPROVE);
                WARM_MODIFIEDPRESCRIPTIONOPTION = HisConfigs.Get<string>(CONFIG_KEY__WARM_MODIFIEDPRESCRIPTIONOPTION);
                DO_NOT_ALLOW_MOBA_HAS_TRANSACTION_SALE_EXP_MEST = HisConfigs.Get<string>(CONFIG_KEY__DO_NOT_ALLOW_MOBA_HAS_TRANSACTION_SALE_EXP_MEST);
                CALL_PATIENT_FORMAT = HisConfigs.Get<string>(CONFIG_KEY__CALL_PATIENT_FORMAT);
                APPROVAL_OR_EXP_OR_IMP_LOGINNAME_OPTION = HisConfigs.Get<string>(CONFIG_KEY__APPROVAL_OR_EXP_OR_IMP_LOGINNAME_OPTION);
                E_BILL__PRINT_NUM_COPY = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>(ELECTRONIC_BILL__PRINT_NUM_COPY);
                autoPrintType = HisConfigs.Get<string>(AUTO_PRINT_TYPE);
                PlatformOption = HisConfigs.Get<int>(PlatformOptionCFG);
                AllowEditExpTime = HisConfigs.Get<string>(ALLOW_EDIT_EXP_TIME);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
