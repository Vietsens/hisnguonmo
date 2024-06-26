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
using LIS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using LIS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.SampleRoomPartialKXN
{
    public class LisSampleSttCFG
    {
        private const string SAMPLE_STT_CODE__UNSPECIMEN = "LIS.LIS_SAMPLE_STT.SAMPLE_STT_CODE.UNSPECIMEN";
        private const string SAMPLE_STT_CODE__SPECIMEN = "LIS.LIS_SAMPLE_STT.SAMPLE_STT_CODE.SPECIMEN";
        private const string SAMPLE_STT_CODE__RESULT = "LIS.LIS_SAMPLE_STT.SAMPLE_STT_CODE.RESULT";
        private const string SAMPLE_STT_CODE__RETURN_RESULT = "LIS.LIS_SAMPLE_STT.SAMPLE_STT_CODE.RETURN_RESULT";

        private static long sampleSttIdUnSpecimen;
        public static long SAMPLE_STT_ID__UNSPECIMEN
        {
            get
            {
                if (sampleSttIdUnSpecimen == 0)
                {
                    sampleSttIdUnSpecimen = GetId(SAMPLE_STT_CODE__UNSPECIMEN);
                }
                return sampleSttIdUnSpecimen;
            }
            set
            {
                sampleSttIdUnSpecimen = value;
            }
        }
        private static long sampleSttIdSpecimen;
        public static long SAMPLE_STT_ID__SPECIMEN
        {
            get
            {
                if (sampleSttIdSpecimen == 0)
                {
                    sampleSttIdSpecimen = GetId(SAMPLE_STT_CODE__SPECIMEN);
                }
                return sampleSttIdSpecimen;
            }
            set
            {
                sampleSttIdSpecimen = value;
            }
        }

        private static long sampleSttIdResult;
        public static long SAMPLE_STT_ID__RESULT
        {
            get
            {
                if (sampleSttIdResult == 0)
                {
                    sampleSttIdResult = GetId(SAMPLE_STT_CODE__RESULT);
                }
                return sampleSttIdResult;
            }
            set
            {
                sampleSttIdResult = value;
            }
        }

        private static long sampleSttIdReturnResult;
        public static long SAMPLE_STT_ID__RETURN_RESULT
        {
            get
            {
                if (sampleSttIdReturnResult == 0)
                {
                    sampleSttIdReturnResult = GetId(SAMPLE_STT_CODE__RETURN_RESULT);
                }
                return sampleSttIdReturnResult;
            }
            set
            {
                sampleSttIdReturnResult = value;
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                SDA.EFMODEL.DataModels.SDA_CONFIG config = Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                string value = String.IsNullOrEmpty(config.VALUE) ? (String.IsNullOrEmpty(config.DEFAULT_VALUE) ? "" : config.DEFAULT_VALUE) : config.VALUE;
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);
                LisSampleSttFilter filter = new LisSampleSttFilter();
                //filter.SERVICE_REQ_STT_CODE = value;//TODO
                var data = BackendDataWorker.Get<LIS_SAMPLE_STT>().FirstOrDefault(o => o.SAMPLE_STT_CODE == value);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => config), config));
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
