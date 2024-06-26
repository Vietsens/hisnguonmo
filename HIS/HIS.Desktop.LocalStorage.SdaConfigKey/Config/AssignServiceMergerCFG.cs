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
    public class AssignServiceMergerCFG
    {
        private const string LOAD_PATIENT_TYPE_DEFAULT_KEY = "EXE.ASSING_SERVICE_MERGER.LOAD_PATIENT_TYPE_DEFAULT";
        private const string IS_LOAD_INGREDIENT_NAME_KEY = "EXE.ASSING_SERVICE_MERGER.IS_LOAD_INGREDIENT_NAME";
        private const string VIEW_MEDICINE_MATERIAL_OUT_OF_STOCK_KEY = "EXE.ASSING_SERVICE_MERGER.VIEW_MEDICINE_MATERIAL_OUT_OF_STOCK";
        private const string IS_VISILBE_PRIVIOUS_EXP_MEST_KEY = "EXE.ASSING_SERVICE_MERGER.IS_VISILBE_PRIVIOUS_EXP_MEST";
        private const string IS_VISILBE_TEMPLATE_MEDICINE_KEY = "EXE.ASSING_SERVICE_MERGER.IS_VISILBE_TEMPLATE_MEDICINE";
        private const string IS_VISILBE_EXECUTE_GROUP_KEY = "HIS.Desktop.Plugins.Assign.IsExecuteGroup";
        private static string loadPatientTypeDefault;

        private static string isVisilbleExecuteGroup;

        public static string IS_VISILBE_EXECUTE_GROUP
        {
            get
            {
                if (isVisilbleExecuteGroup == null)
                {
                    isVisilbleExecuteGroup = GetValue(IS_VISILBE_EXECUTE_GROUP_KEY);
                }
                return isVisilbleExecuteGroup;
            }
            set
            {
                isVisilbleExecuteGroup = value;
            }
        }

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

        private static string isVisilbePriviousExpMest;

        public static string IS_VISILBE_PRIVIOUS_EXP_MEST
        {
            get
            {
                if (isVisilbePriviousExpMest == null)
                {
                    isVisilbePriviousExpMest = GetValue(IS_VISILBE_PRIVIOUS_EXP_MEST_KEY);
                }
                return isVisilbePriviousExpMest;
            }
            set
            {
                isVisilbePriviousExpMest = value;
            }
        }

        private static string isVisilbleTemplateMedicine;

        public static string IS_VISILBE_TEMPLATE_MEDICINE
        {
            get
            {
                if (isVisilbleTemplateMedicine == null)
                {
                    isVisilbleTemplateMedicine = GetValue(IS_VISILBE_TEMPLATE_MEDICINE_KEY);
                }
                return isVisilbleTemplateMedicine;
            }
            set
            {
                isVisilbleTemplateMedicine = value;
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
                result = SdaConfigs.Get<string>(code);
                if (String.IsNullOrEmpty(result)) throw new ArgumentNullException(code);
                return result;
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
