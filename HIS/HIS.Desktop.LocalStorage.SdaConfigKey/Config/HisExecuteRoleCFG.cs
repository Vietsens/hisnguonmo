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
using System.Collections.Generic;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.LocalStorage.SdaConfig;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class HisExecuteRoleCFG
    {
        private const string EXECUTE_ROLE_CODE_MAIN = "DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.MAIN";//Bac si phau thuat
        private const string EXECUTE_ROLE_CODE_TT = "DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.TT";//Bac si thu thuat
        private const string EXECUTE_ROLE_CODE_PM1 = "DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.PM1";//Phụ mổ 1
        private const string EXECUTE_ROLE_CODE_PM2 = "DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.PM2";//Phụ mỏ 2
        private const string EXECUTE_ROLE_CODE_PME1 = "DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.PMe1";//Phụ mê 1
        private const string EXECUTE_ROLE_CODE_PME2 = "DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.PMe2";//Phụ mê 2
        private const string EXECUTE_ROLE_CODE_GMHS = "DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.GMHS";//Gây mê hồi sức
        private const string EXECUTE_ROLE_CODE_GV = "DBCODE.HIS_RS.HIS_EXECUTE_ROLE.EXECUTE_ROLE_CODE.GV";//Giúp việc

        private static long executeRoleIdMain;
        public static long EXECUTE_ROLE_ID__MAIN
        {
            get
            {
                if (executeRoleIdMain == 0)
                {
                    executeRoleIdMain = GetId(SdaConfigs.Get<string>(EXECUTE_ROLE_CODE_MAIN));
                }
                return executeRoleIdMain;
            }
            set
            {
                executeRoleIdMain = value;
            }
        }

        private static long executeRoleIdTT;
        public static long EXECUTE_ROLE_ID__TT
        {
            get
            {
                if (executeRoleIdTT == 0)
                {
                    executeRoleIdTT = GetId(SdaConfigs.Get<string>(EXECUTE_ROLE_CODE_TT));
                }
                return executeRoleIdTT;
            }
            set
            {
                executeRoleIdMain = value;
            }
        }

        private static long executeRoleIdPM1;
        public static long EXECUTE_ROLE_ID__PM1
        {
            get
            {
                if (executeRoleIdPM1 == 0)
                {
                    executeRoleIdPM1 = GetId(SdaConfigs.Get<string>(EXECUTE_ROLE_CODE_PM1));
                }
                return executeRoleIdPM1;
            }
            set
            {
                executeRoleIdPM1 = value;
            }
        }

        private static long executeRoleIdPM2;
        public static long EXECUTE_ROLE_ID__PM2
        {
            get
            {
                if (executeRoleIdPM2 == 0)
                {
                    executeRoleIdPM2 = GetId(SdaConfigs.Get<string>(EXECUTE_ROLE_CODE_PM2));
                }
                return executeRoleIdPM2;
            }
            set
            {
                executeRoleIdPM2 = value;
            }
        }

        private static long executeRoleIdPME2;
        public static long EXECUTE_ROLE_ID__PME2
        {
            get
            {
                if (executeRoleIdPME2 == 0)
                {
                    executeRoleIdPME2 = GetId(SdaConfigs.Get<string>(EXECUTE_ROLE_CODE_PME2));
                }
                return executeRoleIdPME2;
            }
            set
            {
                executeRoleIdPME2 = value;
            }
        }

        private static long executeRoleIdPMe1;
        public static long EXECUTE_ROLE_ID__PME1
        {
            get
            {
                if (executeRoleIdPMe1 == 0)
                {
                    executeRoleIdPMe1 = GetId(SdaConfigs.Get<string>(EXECUTE_ROLE_CODE_PME1));
                }
                return executeRoleIdPMe1;
            }
            set
            {
                executeRoleIdPMe1 = value;
            }
        }

        private static long executeRoleIdGMHS;
        public static long EXECUTE_ROLE_ID__GMHS
        {
            get
            {
                if (executeRoleIdGMHS == 0)
                {
                    executeRoleIdGMHS = GetId(SdaConfigs.Get<string>(EXECUTE_ROLE_CODE_GMHS));
                }
                return executeRoleIdGMHS;
            }
            set
            {
                executeRoleIdGMHS = value;
            }
        }

        private static long executeRoleIdGV;
        public static long EXECUTE_ROLE_ID__GV
        {
            get
            {
                if (executeRoleIdGV == 0)
                {
                    executeRoleIdGV = GetId(SdaConfigs.Get<string>(EXECUTE_ROLE_CODE_GV));
                }
                return executeRoleIdGV;
            }
            set
            {
                executeRoleIdGV = value;
            }
        }



        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(o => o.EXECUTE_ROLE_CODE == code);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }
    }
}
