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

namespace Inventec.UC.CreateReport.Config
{
    internal class SarReportSttCFG
    {
        private const string REPORT_STT_CODE__WAIT = "DBCODE.SAR_RS.SAR_REPORT_STT.REPORT_STT_CODE.WAIT";
        private const string REPORT_STT_CODE__PROCESSING = "DBCODE.SAR_RS.SAR_REPORT_STT.REPORT_STT_CODE.PROCESSING";
        private const string REPORT_STT_CODE__DONE = "DBCODE.SAR_RS.SAR_REPORT_STT.REPORT_STT_CODE.DONE";
        private const string REPORT_STT_CODE__CANCEL = "DBCODE.SAR_RS.SAR_REPORT_STT.REPORT_STT_CODE.CANCEL";
        private const string REPORT_STT_CODE__ERROR = "DBCODE.SAR_RS.SAR_REPORT_STT.REPORT_STT_CODE.ERROR";

        private static long ReportSttIdWait;
        internal static long REPORT_STT_ID__WAIT
        {
            get
            {
                if (ReportSttIdWait == 0)
                {
                    ReportSttIdWait = GetId(REPORT_STT_CODE__WAIT);
                }
                return ReportSttIdWait;
            }
            set
            {
            }
        }

        private static long ReportSttIdProcessing;
        internal static long REPORT_STT_ID__PROCESSING
        {
            get
            {
                if (ReportSttIdProcessing == 0)
                {
                    ReportSttIdProcessing = GetId(REPORT_STT_CODE__PROCESSING);
                }
                return ReportSttIdProcessing;
            }
            set
            {
            }
        }

        private static long ReportSttIdDone;
        internal static long REPORT_STT_ID__DONE
        {
            get
            {
                if (ReportSttIdDone == 0)
                {
                    ReportSttIdDone = GetId(REPORT_STT_CODE__DONE);
                }
                return ReportSttIdDone;
            }
            set
            {
            }
        }

        private static long ReportSttIdCancel;
        internal static long REPORT_STT_ID__CANCEL
        {
            get
            {
                if (ReportSttIdCancel == 0)
                {
                    ReportSttIdCancel = GetId(REPORT_STT_CODE__CANCEL);
                }
                return ReportSttIdCancel;
            }
            set
            {
            }
        }

        private static long ReportSttIdError;
        internal static long REPORT_STT_ID__ERROR
        {
            get
            {
                if (ReportSttIdError == 0)
                {
                    ReportSttIdError = GetId(REPORT_STT_CODE__ERROR);
                }
                return ReportSttIdError;
            }
            set
            {
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                SDA.EFMODEL.DataModels.SDA_CONFIG config = Loader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                string value = string.IsNullOrEmpty(config.VALUE) ? (string.IsNullOrEmpty(config.DEFAULT_VALUE) ? "" : config.DEFAULT_VALUE) : config.VALUE;
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(code);
                SAR.Filter.SarReportSttFilter filter = new SAR.Filter.SarReportSttFilter();
                var data = new Sar.SarReportStt.SarReportSttGet().Get(filter).FirstOrDefault(o => o.REPORT_STT_CODE == value);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => config), config));
                result = data.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
