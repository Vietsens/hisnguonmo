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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class HisCareerCFG
    {
        private const string CAREER_CODE_BASE = "EXE.HIS_CAREER_CODE__BASE";
        private const string CAREER_CODE__UNDER_6_AGE = "EXE.HIS_CAREER_CODE__UNDER_6_AGE";
        private const string CAREER_CODE__HOC_SINH = "HIS.DESKTOP.REGISTER.HIS_CAREER.CARRER_CODE_HS";

        private static HIS_CAREER careerBase;
        public static HIS_CAREER CAREER_BASE
        {
            get
            {
                if (careerBase == null)
                {
                    careerBase = GetData(CAREER_CODE_BASE);
                }
                return careerBase;
            }
            set
            {
                careerBase = value;
            }
        }

        private static HIS_CAREER careerUnder6Age;
        public static HIS_CAREER CAREER__UNDER_6_AGE
        {
            get
            {
                if (careerUnder6Age == null)
                {
                    careerUnder6Age = GetData(CAREER_CODE__UNDER_6_AGE);
                }
                return careerUnder6Age;
            }
            set
            {
                careerUnder6Age = value;
            }
        }

        private static HIS_CAREER careerHocSinh;
        public static HIS_CAREER CAREER__HOCSINH
        {
            get
            {
                if (careerHocSinh == null)
                {
                    careerHocSinh = GetData(CAREER_CODE__HOC_SINH);
                }
                return careerHocSinh;
            }
            set
            {
                careerHocSinh = value;
            }
        }

        private static HIS_CAREER GetData(string code)
        {
            HIS_CAREER result = null;
            try
            {
                SDA.EFMODEL.DataModels.SDA_CONFIG config = Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                string value = String.IsNullOrEmpty(config.VALUE) ? (String.IsNullOrEmpty(config.DEFAULT_VALUE) ? "" : config.DEFAULT_VALUE) : config.VALUE;
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);
                HisCareerFilter filter = new HisCareerFilter();
                //filter.KEY_WORD = value;
                var data = BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.CAREER_CODE == value);
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
