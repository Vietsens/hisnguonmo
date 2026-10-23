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
using HIS.Desktop.LocalStorage.LisConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Utils.Drawing.Helpers.NativeMethods;

namespace HIS.Desktop.Plugins.SampleCollectionRoom.Config
{
    class LisConfigCFG
    {
        private const string CONFIG_KEY__IS_AUTO_CREATE_BARCODE = "LIS.LIS_SAMPLE.IS_AUTO_CREATE_BARCODE";
        //private const string CONFIG_KEY__MUST_APPROVE_SAMPLE = "LIS.LIS_SAMPLE.MUST_APPROVE_SAMPLE";
        private const string CONFIG_KEY__PRINT_BARCODE_BY_BARTENDER = "LIS.LIS_SAMPLE.PRINT_BARCODE.BY_BARTENDER";
        //private const string CONFIG_KEY__SHOW_BUTTON_APPROVE = "LIS.LIS_SAMPLE.IS_SHOW_BUTTON_APPROVE_SAMPLE";
        private const string CONFIG_KEY__SHOW_FROM_SAMPLE_INFO = "LIS.LIS_SAMPLE.TAKE_SAMPLE.IS_SHOW_FORM_SAMPLE_INFO";
        private const string CONFIG_KEY__AUTO_SAMPLE_AFTER_ENTER_BARCODE = "LIS.DESKTOP.AUTO_SAMPLE_AFTER_ENTER_BARCODE";
        private const string CONFIG_KEY__ALLOW_AUTO_SAMPLE_AFTER_PRINT_BARCODE = "LIS.LIS_SAMPLE.ALLOW_AUTO_SAMPLE_AFTER_PRINT_BARCODE";
        private const string CONFIG_KEY__DEFAULT_FOCUS_FIELD = "LIS.LIS_SAMPLE.TAKE_SAMPLE.DEFAULT_FOCUS_FIELD"; // vCong59787: 1 = Ma y lenh, 2 = Ma dieu tri, 3 = Barcode, 4 = Tu khoa; trong/khac = Ma dieu tri

        internal static string IS_AUTO_CREATE_BARCODE;
        //internal static string MUST_APPROVE_SAMPLE;
        internal static string PRINT_BARCODE_BY_BARTENDER;
        //internal static string SHOW_BUTTON_APPROVE;
        internal static string SHOW_FORM_SAMPLE_INFO;
        internal static string ALLOW_AUTO_SAMPLE_AFTER_PRINT_BARCODE;
        internal static string DEFAULT_FOCUS_FIELD;

        internal static bool IsAutoSampleAfterEnterBarcode;

        internal static void LoadConfig()
        {
            try
            {
                IS_AUTO_CREATE_BARCODE = GetValue(CONFIG_KEY__IS_AUTO_CREATE_BARCODE);
                //MUST_APPROVE_SAMPLE = GetValue(CONFIG_KEY__MUST_APPROVE_SAMPLE);
                PRINT_BARCODE_BY_BARTENDER = GetValue(CONFIG_KEY__PRINT_BARCODE_BY_BARTENDER);
                //SHOW_BUTTON_APPROVE = GetValue(CONFIG_KEY__SHOW_BUTTON_APPROVE);
                SHOW_FORM_SAMPLE_INFO = GetValue(CONFIG_KEY__SHOW_FROM_SAMPLE_INFO);
                ALLOW_AUTO_SAMPLE_AFTER_PRINT_BARCODE = GetValue(CONFIG_KEY__ALLOW_AUTO_SAMPLE_AFTER_PRINT_BARCODE);
                DEFAULT_FOCUS_FIELD = GetValue(CONFIG_KEY__DEFAULT_FOCUS_FIELD);
                // vCong59787: may tram chi nap LIS_CONFIG khi HIS_CONFIG khai LIS noi bo (MOS.LIS.*) -> khong co trong bo nho thi hoi thang API
                if (DEFAULT_FOCUS_FIELD == null)
                    DEFAULT_FOCUS_FIELD = GetValueFromApi(CONFIG_KEY__DEFAULT_FOCUS_FIELD);
                IsAutoSampleAfterEnterBarcode = GetValue(CONFIG_KEY__AUTO_SAMPLE_AFTER_ENTER_BARCODE) == "1";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValueFromApi(string code)
        {
            string result = null;
            try
            {
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                LIS.Filter.LisConfigFilter filter = new LIS.Filter.LisConfigFilter();
                filter.IS_ACTIVE = 1;
                var configs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<LIS.EFMODEL.DataModels.LIS_CONFIG>>("api/LisConfig/Get", HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer, filter, param);
                var config = configs != null ? configs.FirstOrDefault(o => o.KEY == code) : null;
                if (config != null)
                {
                    result = !String.IsNullOrEmpty(config.VALUE) ? config.VALUE : (config.DEFAULT_VALUE ?? "");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
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
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
