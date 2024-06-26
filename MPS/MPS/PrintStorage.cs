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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS
{
    public class PrintStorage
    {
        public static List<PrintData> PrintDatas { get; set; }

        public static void Add(PrintData printData)
        {
            try
            {
                if (PrintDatas == null)
                {
                    PrintDatas = new List<PrintData>();
                }
                PrintDatas.Add(printData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static void Finish(string printTypeCode)
        {
            try
            {
                var pdUpdate = PrintDatas != null ? PrintDatas.FirstOrDefault(o => o.printTypeCode == printTypeCode && o.PrintMerge == PrintStatus.Running) : null;
                if (pdUpdate != null)
                {
                    pdUpdate.PrintMerge = PrintStatus.Finish;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static void Remove(PrintData printData)
        {
            try
            {
                PrintDatas.Remove(printData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static void Clear()
        {
            try
            {
                PrintDatas = new List<PrintData>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
