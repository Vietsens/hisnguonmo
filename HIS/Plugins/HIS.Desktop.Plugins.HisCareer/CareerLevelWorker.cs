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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisCareer
{
    /// <summary>
    /// Xu ly thong tin cap 2/3/4 cua danh muc nghe nghiep (QD 34/2020/QD-TTg).
    /// Doc/ghi property LEVELx_CODE/LEVELx_NAME cua HIS_CAREER bang reflection
    /// de khong phu thuoc phien ban MOS.EFMODEL (DLL cu chua co field thi tra ve rong).
    /// </summary>
    internal class CareerLevelWorker
    {
        /// <summary>
        /// Do dai chuan cua ma nghe cap 5 theo QD 34/2020/QD-TTg
        /// </summary>
        internal const int CAREER_CODE_LENGTH = 5;

        /// <summary>
        /// Lay gia tri string cua 1 property theo ten (null-safe voi EFMODEL cu chua co field)
        /// </summary>
        internal static string GetProp(HIS_CAREER data, string propName)
        {
            string result = "";
            try
            {
                if (data != null)
                {
                    var pi = typeof(HIS_CAREER).GetProperty(propName);
                    if (pi != null)
                    {
                        var value = pi.GetValue(data, null);
                        result = value != null ? value.ToString() : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Suy ma cap tu ma nghe cap 5: level = 2/3/4 -> lay 2/3/4 ky tu dau.
        /// Ma khong du 5 ky tu (bo ma cu) -> tra ve rong
        /// </summary>
        internal static string GetLevelCode(string careerCode, int level)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(careerCode) && careerCode.Trim().Length == CAREER_CODE_LENGTH)
                {
                    result = careerCode.Trim().Substring(0, level);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Tim ten cap theo ma cap tu danh muc nghe nghiep trong cache
        /// (lay tu ban ghi khac cung ma cap co ten cap khac rong). Khong co -> rong
        /// </summary>
        internal static string FindLevelName(string levelCode, int level)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(levelCode))
                {
                    string codeProp = "LEVEL" + level + "_CODE";
                    string nameProp = "LEVEL" + level + "_NAME";
                    var careers = BackendDataWorker.Get<HIS_CAREER>();
                    if (careers != null)
                    {
                        var matched = careers.FirstOrDefault(o =>
                            GetProp(o, codeProp) == levelCode && !String.IsNullOrEmpty(GetProp(o, nameProp)));
                        if (matched != null)
                        {
                            result = GetProp(matched, nameProp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
