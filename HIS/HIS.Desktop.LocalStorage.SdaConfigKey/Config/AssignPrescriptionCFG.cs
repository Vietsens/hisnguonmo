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
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.SdaConfigKey
{
    public class AssignPrescriptionCFG
    {
        private const string Key__IsCabinet = "HIS.Desktop.Plugins.AssignPrescription.IsCabinet";
        private const string Key__AcinInteractive__Grade = "HIS.Desktop.Plugins.AssignPrescription.AcinInteractive__Grade";
        private const string LOAD_PATIENT_TYPE_DEFAULT_KEY = "EXE.ASSING_SERVICE_MERGER.LOAD_PATIENT_TYPE_DEFAULT";
        private const string IS_LOAD_INGREDIENT_NAME_KEY = "EXE.ASSING_SERVICE_MERGER.IS_LOAD_INGREDIENT_NAME";
        private const string VIEW_MEDICINE_MATERIAL_OUT_OF_STOCK_KEY = "EXE.ASSING_SERVICE_MERGER.VIEW_MEDICINE_MATERIAL_OUT_OF_STOCK";

        /// <summary>
        /// cấu hình hệ thống để hiển thị tủ trực hay không
        ///Đặt 1 là chỉ hiển thị các kho là tủ trực, giá trị khác là hiển thị tất cả các kho
        /// </summary>
        private static string isCabinet;
        public static string IsCabinet
        {
            get
            {
                if (isCabinet == null)
                {
                    isCabinet = GetValue(Key__IsCabinet);
                }
                return isCabinet;
            }
            set
            {
                isCabinet = value;
            }
        }

        /// <summary>
        /// Thẻ cấu hình mức cảnh báo căn cứ theo hoạt chất có trong các loại thuốc, 
        /// thực hiện cảnh báo cho các loại thuốc có trong, kiểu dữ liệu số nguyên.
        /// Nếu cấu hình này khác null thì nếu mức cảnh báo > cấu hình thì client sẽ thực hiện chặn không cho phép kê.      
        /// </summary>
        private static long? acinInteractive__Grade;
        public static long? AcinInteractive__Grade
        {
            get
            {
                if (!acinInteractive__Grade.HasValue)
                {
                    string vl = GetValue(Key__AcinInteractive__Grade);
                    if (!String.IsNullOrEmpty(vl))
                    {
                        acinInteractive__Grade = Inventec.Common.TypeConvert.Parse.ToInt64(vl);
                    }                    
                }
                return acinInteractive__Grade;
            }
            set
            {
                acinInteractive__Grade = value;
            }
        }

        private static string loadPatientTypeDefault;
        public static string LOAD_PATIENT_TYPE_DEFAULT
        {
            get
            {
                if (loadPatientTypeDefault == null)
                {
                    loadPatientTypeDefault = GetValue(LOAD_PATIENT_TYPE_DEFAULT_KEY);
                }
                return loadPatientTypeDefault;
            }
            set
            {
                loadPatientTypeDefault = value;
            }
        }

        private static string viewMedicineMaterialOutOfStock;

        public static string VIEW_MEDICINE_MATERIAL_OUT_OF_STOCK
        {
            get
            {
                if (viewMedicineMaterialOutOfStock == null)
                {
                    viewMedicineMaterialOutOfStock = GetValue(VIEW_MEDICINE_MATERIAL_OUT_OF_STOCK_KEY);
                }
                return viewMedicineMaterialOutOfStock;
            }
            set
            {
                viewMedicineMaterialOutOfStock = value;
            }
        }


        private static string isLoadIngredientName;

        public static string IS_LOAD_INGREDIENT_NAME
        {
            get
            {
                if (isLoadIngredientName == null)
                {
                    isLoadIngredientName = GetValue(IS_LOAD_INGREDIENT_NAME_KEY);
                }
                return isLoadIngredientName;
            }
            set
            {
                isLoadIngredientName = value;
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return SdaConfigs.Get<string>(code);
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
