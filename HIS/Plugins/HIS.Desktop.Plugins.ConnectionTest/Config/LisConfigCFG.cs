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
using HIS.Desktop.LocalStorage.LisConfig;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ConnectionTest.Config
{
    internal class LisConfigCFG
    {

        private const string CONFIG_KEY__CONNECTION_TEST__ISALOWSAVEWHENNOTFULLVALUES = "LIS.SAMPLE__IS_ALLOW_SAVE_WHEN_NOT_FULL_VALUES";
        private const string CONFIG_KEY__CONNECTION_TEST__IS_AUTO_CREATE_BARCODE = "LIS.LIS_SAMPLE.IS_AUTO_CREATE_BARCODE";
        //private const string CONFIG_KEY__MUST_APPROVE_SAMPLE = "LIS.LIS_SAMPLE.MUST_APPROVE_SAMPLE";
        private const string CONFIG_KEY__MUST_APPROVE_RESULT = "LIS.LIS_SAMPLE.MUST_APPROVE_RESULT_BEFORE_RETURN";
        private const string CONFIG_KEY__PRINT_BARCODE_BY_BARTENDER = "LIS.LIS_SAMPLE.PRINT_BARCODE.BY_BARTENDER";
        private const string CONFIG_KEY__SHOW_BUTTON_APPROVE = "LIS.LIS_SAMPLE.IS_SHOW_BUTTON_APPROVE_SAMPLE";
        private const string CONFIG_KEY__SHOW_FROM_SAMPLE_INFO = "LIS.LIS_SAMPLE.TAKE_SAMPLE.IS_SHOW_FORM_SAMPLE_INFO";
        private const string CONFIG_KEY__IS_PRINT_WHEN_APPROVE_RESULT = "LIS.LIS_SAMPLE.WHEN_APPROVE_RESULT.IS_PRINT";
        private const string CONFIG_KEY__MUST_TICK_RUN_AGAIN_WHEN_RUN_TEST = "LIS.LIS_SAMPLE_SERVICE.MUST_TICK_RUN_AGAIN_WHEN_RERUN_TEST";
        private const string CONFIG_KEY__IS_NOT_RETURN_LIS_SERVICE_WHEN_RUNNING = "LIS.LIS_SAMPLE_SERVICE.WHEN_RUNNING.IS_NOT_RETURN_TO_LIS_SERVICE";
        private const string CONFIG_KEY__AUTO_SAMPLE_AFTER_ENTER_BARCODE = "LIS.DESKTOP.AUTO_SAMPLE_AFTER_ENTER_BARCODE";
        private const string CONFIG_KEY__ALLOW_SAVE_WHEN_EMPTY = "LIS_RESULT.SAVE_RESULT.ALLOW_WHEN_EMPTY_OPTION";
        private const string CONFIG_KEY__ALLOW_TO_EDIT_APPROVE_TIME = "LIS.LIS_SAMPLE.ALLOW_TO_EDIT_APPROVE_TIME";
        private const string CONFIG_KEY__WARNING_APPROVE_TIME = "LIS.LIS_SAMPLE.WARNING_APPROVE_TIME";
        private const string CONFIG_KEY__ALLOW_TO_EDIT_APPROVE_RESULT_TIME = "LIS.LIS_SAMPLE.ALLOW_TO_EDIT_APPROVE_RESULT_TIME";
        private const string CONFIG_KEY__IS_REQUIRED_APPROVE_SAMPLE = "LIS.LIS_SAMPLE.IS_REQUIRED_APPROVE_SAMPLE";

        internal static bool ALLOW_TO_EDIT_APPROVE_RESULT_TIME;
        internal static string IS_AUTO_CREATE_BARCODE;
        internal static string IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT;
        internal static string PRINT_BARCODE_BY_BARTENDER;
        internal static string MUST_APPROVE_RESULT;
        internal static string SHOW_BUTTON_APPROVE;
        internal static string SHOW_FORM_SAMPLE_INFO;
        internal static string IS_PRINT_WHEN_APPROVE_RESULT;
        internal static string MUST_TICK_RUN_AGAIN;
        internal static bool IsAutoSampleAfterEnterBarcode;
        internal static string ALLOW_SAVE_WHEN_EMPTY;
        internal static string ALLOW_TO_EDIT_APPROVE_TIME;
        internal static long WARNING_APPROVE_TIME;
        internal static bool IS_REQUIRED_APPROVE_SAMPLE;

        internal static void LoadConfig()
        {
            try
            {
                ALLOW_TO_EDIT_APPROVE_RESULT_TIME = GetValue(CONFIG_KEY__ALLOW_TO_EDIT_APPROVE_RESULT_TIME) == "1";
                IS_AUTO_CREATE_BARCODE = GetValue(CONFIG_KEY__CONNECTION_TEST__IS_AUTO_CREATE_BARCODE);
                IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT = GetValue(CONFIG_KEY__CONNECTION_TEST__ISALOWSAVEWHENNOTFULLVALUES);
                PRINT_BARCODE_BY_BARTENDER = GetValue(CONFIG_KEY__PRINT_BARCODE_BY_BARTENDER);
                MUST_APPROVE_RESULT = GetValue(CONFIG_KEY__MUST_APPROVE_RESULT);
                SHOW_BUTTON_APPROVE = GetValue(CONFIG_KEY__SHOW_BUTTON_APPROVE);
                SHOW_FORM_SAMPLE_INFO = GetValue(CONFIG_KEY__SHOW_FROM_SAMPLE_INFO);
                IS_PRINT_WHEN_APPROVE_RESULT = GetValue(CONFIG_KEY__IS_PRINT_WHEN_APPROVE_RESULT);
                MUST_TICK_RUN_AGAIN = GetValue(CONFIG_KEY__MUST_TICK_RUN_AGAIN_WHEN_RUN_TEST);
                ALLOW_SAVE_WHEN_EMPTY = GetValue(CONFIG_KEY__ALLOW_SAVE_WHEN_EMPTY);
                IsAutoSampleAfterEnterBarcode = GetValue(CONFIG_KEY__AUTO_SAMPLE_AFTER_ENTER_BARCODE) == "1";
                ALLOW_TO_EDIT_APPROVE_TIME = GetValue(CONFIG_KEY__ALLOW_TO_EDIT_APPROVE_TIME);
                WARNING_APPROVE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(GetValue(CONFIG_KEY__WARNING_APPROVE_TIME) ?? "0");
                IS_REQUIRED_APPROVE_SAMPLE = GetValue(CONFIG_KEY__IS_REQUIRED_APPROVE_SAMPLE) == "1";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return LisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
    }
}
