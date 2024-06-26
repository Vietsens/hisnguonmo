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
using HIS.Desktop.Library.CacheClient.Redis;
using System.Configuration;

namespace HIS.Desktop.Library.CacheClient
{
    public class SerivceConfig
    {
        public static string MosBaseUri { get; set; }
        public static string SdaBaseUri { get; set; }
        public static string SarBaseUri { get; set; }
        public static string AcsBaseUri { get; set; }
        public static string TokenCode { get; set; }
        public static string ApplicationCode { get; set; }
        public static long CacheType { get; set; }
        public static RedisSaveType RedisSaveType { get; set; }
        public static string PreNamespaceFolder { get; set; }

        //public static readonly int timeSync = int.Parse(ConfigurationManager.AppSettings["HIS.Service.LocalStorage.DataConfig.TimeSync"] ?? "600000");

        public const string Seperate = ",";
        public const string TableName__SHC_SYNC = "SHC_SYNC";

        public const string ID = "ID";
        public const string KEY = "KEY";
        public const string LAST_DB_MODIFY_TIME = "LAST_DB_MODIFY_TIME";
        public const string LAST_SYNC_MODIFY_TIME = "LAST_SYNC_MODIFY_TIME";
        public const string VALUE = "VALUE";
        public const string IS_MODIFIED = "IS_MODIFIED";
        public const string MODIFY_TIME = "MODIFY_TIME";

        public const string Pattern = "urn:{0}:*";
    }
}
