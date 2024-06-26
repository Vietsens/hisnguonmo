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
using SAR.EFMODEL.DataModels;
using System;
using System.Linq;

namespace MPS.Core
{
    class ProcessorFactoryPlus
    {
        internal static IProcessorPrint MakeProcessorBase(string printCode, string fileName, object data, MPS.Printer.PreviewType previewType, string printerName, int numCopy, bool isPreview)
        {
            IProcessorPrint result = null;
            try
            {
                SAR_PRINT_TYPE config = null;
                try
                {
                    config = PrintConfig.PrintTypes.Where(o => o.PRINT_TYPE_CODE == printCode).SingleOrDefault();
                    if (config == null) throw new Exception("Khong tim duoc config.");
                }
                catch (Exception ex)
                {
                    throw new Exception("Khong truy van duoc du lieu cau hinh in an theo cac thong tin truyen vao. Kiem tra lai frontend & SAR_PRINT_TYPE. Khong the khoi tao doi tuong xu ly in an." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printCode), printCode) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName), ex);
                }

                try
                {
                    //Tat ca cac mau in an phai khai bao o day
                    //Moi khi them mot chuc nang in an moi thi bo xung them 1 dong
                    switch (config.PRINT_TYPE_CODE)
                    {                        
                        //case "Mps000118":// In đơn thuốc tổng hợp v2
                        //    result = new Mps000118.Mps000118Processor(config, fileName, data, previewType, printerName, numCopy, isPreview);
                            //break;                        
                    }
                }
                catch (Exception)
                {
                    throw new NullReferenceException();
                }

            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printCode), printCode) + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
