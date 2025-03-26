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

namespace Inventec.UC.ListReports.Config
{
    class SarPaperSizeCFG
    {
        private const string PAPER_SIZE_CODE__A2 = "DBCODE.SAR_RS.SAR_PAPER_SIZE.PAPER_SIZE_CODE.A2";
        private const string PAPER_SIZE_CODE__A3 = "DBCODE.SAR_RS.SAR_PAPER_SIZE.PAPER_SIZE_CODE.A3";
        private const string PAPER_SIZE_CODE__A4 = "DBCODE.SAR_RS.SAR_PAPER_SIZE.PAPER_SIZE_CODE.A4";
        private const string PAPER_SIZE_CODE__A5 = "DBCODE.SAR_RS.SAR_PAPER_SIZE.PAPER_SIZE_CODE.A5";
        private const string PAPER_SIZE_CODE__A6 = "DBCODE.SAR_RS.SAR_PAPER_SIZE.PAPER_SIZE_CODE.A6";

        private static long PaperSizeIdA2;
        internal static long PAPER_SIZE_ID__A2
        {
            get
            {
                if (PaperSizeIdA2 == 0)
                {
                    PaperSizeIdA2 = GetId(PAPER_SIZE_CODE__A2);
                }
                return PaperSizeIdA2;
            }
            set
            {
                PaperSizeIdA2 = value;
            }
        }


        private static long PaperSizeIdA3;
        internal static long PAPER_SIZE_ID__A3
        {
            get
            {
                if (PaperSizeIdA3 == 0)
                {
                    PaperSizeIdA3 = GetId(PAPER_SIZE_CODE__A3);
                }
                return PaperSizeIdA3;
            }
            set
            {
                PaperSizeIdA3 = value;
            }

        }

        private static long PaperSizeIdA4;
        internal static long PAPER_SIZE_ID__A4
        {
            get
            {
                if (PaperSizeIdA4 == 0)
                {
                    PaperSizeIdA4 = GetId(PAPER_SIZE_CODE__A4);
                }
                return PaperSizeIdA4;
            }
            set
            {
                PaperSizeIdA4 = value;
            }
        }

        private static long PaperSizeIdA5;
        internal static long PAPER_SIZE_ID__A5
        {
            get
            {
                if (PaperSizeIdA5 == 0)
                {
                    PaperSizeIdA5 = GetId(PAPER_SIZE_CODE__A5);
                }
                return PaperSizeIdA5;
            }
            set
            {
                PaperSizeIdA5 = value;
            }
        }

        private static long PaperSizeIdA6;
        internal static long PAPER_SIZE_ID__A6
        {
            get
            {
                if (PaperSizeIdA6 == 0)
                {
                    PaperSizeIdA6 = GetId(PAPER_SIZE_CODE__A6);
                }
                return PaperSizeIdA6;
            }
            set
            {
                PaperSizeIdA6 = value;
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
                SAR.Filter.SarPaperSizeFilter filter = new SAR.Filter.SarPaperSizeFilter();
                var data = new Sar.SarPaperSize.Get.SarPaperSizeGet().Get(filter).FirstOrDefault(o => o.PAPER_SIZE_CODE == value);
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
