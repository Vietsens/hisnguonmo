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

namespace HIS.Desktop.ApiConsumer
{
    public class SdaRequestUriStore
    {
        public const string SDA_HIDE_CONTROL_CREATE = "api/sdaHideControl/Create";
        public const string SDA_HIDE_CONTROL_DELETE = "api/sdaHideControl/Delete";
        public const string SDA_HIDE_CONTROL_UPDATE = "api/sdaHideControl/Update";
        public const string SDA_HIDE_CONTROL_GET = "api/sdaHideControl/Get";
        public const string SDA_HIDE_CONTROL_CHANGE_LOCK = "api/sdaHideControl/ChangeLock";
        public const string SDA_ETHNIC_GET = "api/SdaEthnic/Get";
        public const string SDA_GROUP_GET = "api/SdaGroup/Get";
        public const string SDA_PROVINCE_GET = "api/SdaProvince/Get";
        public const string SDA_DISTRICT_GET = "api/SdaDistrict/Get";
        public const string SDA_COMMUNE_GET = "api/SdaCommune/Get";
        public const string SDA_PROVINCE_GETVIEW = "api/SdaProvince/GetView";
        public const string SDA_DISTRICT_GETVIEW = "api/SdaDistrict/GetView";
        public const string SDA_COMMUNE_GETVIEW = "api/SdaCommune/GetView";
        public const string SDA_NATIONAL_GET = "api/SdaNational/Get";
        public const string SDA_CONFIG_GET = "api/SdaConfig/Get";
        public const string SDA_LANGUAGE_GET = "api/SdaLanguage/Get";
        public const string SDA_TRANSLATE_GET = "api/SdaTranslate/Get";
        public const string SDA_MODULE_FIELD_GET = "api/SdaModuleField/Get";
    }
}
