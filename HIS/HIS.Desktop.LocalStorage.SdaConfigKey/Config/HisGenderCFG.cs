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
using Inventec.Core;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Linq;
using SDA.Filter;
using System.Collections.Generic;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class HisGenderCFG
    {
        private const string GENDER_CODE_BASE = "DBCODE.HIS_RS.HIS_GENDER.GENDER_CODE.FEMALE";
        private const string SDA_CONFIG_GENDER_CODE_MALE = "DBCODE.HIS_RS.HIS_GENDER.GENDER_CODE.MALE";
        private const string SDA_CONFIG_GENDER_CODE_FEMALE = "DBCODE.HIS_RS.HIS_GENDER.GENDER_CODE.FEMALE";

        private static MOS.EFMODEL.DataModels.HIS_GENDER genderBase;
        public static MOS.EFMODEL.DataModels.HIS_GENDER GENDER_BASE
        {
            get
            {
                if (genderBase == null)
                {
                    genderBase = GetData(GENDER_CODE_BASE);
                }
                return genderBase;
            }
            set
            {
                genderBase = value;
            }
        }

        private static string genderCodeBase;
        public static string GENDER_CODE_BASE_RETURN
        {
            get
            {
                if (genderCodeBase == "")
                {
                    genderCodeBase = GetCode(GENDER_CODE_BASE);
                }
                return genderCodeBase;
            }
            set
            {
                genderCodeBase = value;
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                result = GetData(code).ID;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }

        private static string GetCode(string code)
        {
            string result = "";
            try
            {
                result = GetData(code).GENDER_CODE;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }


        private static long genderId_Male;
        public static long GENDER_ID_MALE
        {
            get
            {
                if (genderId_Male <= 0)
                {
                    genderId_Male = GetId(GENDER_CODE_MALE);
                }
                return genderId_Male;
            }
            set
            {
                genderId_Male = value;
            }
        }

        private static long genderId_Female;
        public static long GENDER_ID_FEMALE
        {
            get
            {
                if (genderId_Female <= 0)
                {
                    genderId_Female = GetId(GENDER_CODE_FEMALE);
                }
                return genderId_Female;
            }
            set
            {
                genderId_Female = value;
            }
        }

        private static string genderCodeMale;
        public static string GENDER_CODE_MALE
        {
            get
            {
                if (String.IsNullOrEmpty(genderCodeMale))
                {
                    genderCodeMale = GetCode(SDA_CONFIG_GENDER_CODE_MALE);
                }
                return genderCodeMale;
            }
            set
            {
                genderCodeMale = value;
            }
        }

        private static long genderIdBase;
        public static long GENDER_ID_BASE
        {
            get
            {
                if (genderIdBase == 0)
                {
                    genderIdBase = GetId(GENDER_CODE_BASE);
                }
                return genderIdBase;
            }
            set
            {
                genderIdBase = value;
            }
        }

        private static string genderCodeFeMale;
        public static string GENDER_CODE_FEMALE
        {
            get
            {
                if (String.IsNullOrEmpty(genderCodeFeMale))
                {
                    genderCodeFeMale = GetCode(SDA_CONFIG_GENDER_CODE_FEMALE);
                }
                return genderCodeFeMale;
            }
            set
            {
                genderCodeFeMale = value;
            }
        }

        private static MOS.EFMODEL.DataModels.HIS_GENDER GetData(string code)
        {
            MOS.EFMODEL.DataModels.HIS_GENDER result = null;
            try
            {
                SDA.EFMODEL.DataModels.SDA_CONFIG config = Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                string value = String.IsNullOrEmpty(config.VALUE) ? (String.IsNullOrEmpty(config.DEFAULT_VALUE) ? "" : config.DEFAULT_VALUE) : config.VALUE;
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);
                HisGenderFilter filter = new HisGenderFilter();
                //filter.GENDER_CODE = value;
                var data = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.GENDER_CODE == value);

                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => config), config));
                result = data;
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
