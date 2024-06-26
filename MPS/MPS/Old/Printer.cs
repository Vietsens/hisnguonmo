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

namespace MPS
{
    public class Printer
    {
        public enum PreviewType
        {
            Show,
            ShowDialog,
            PrintNow
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysCode"></param>
        /// <param name="printCode"></param>
        /// <param name="data"></param>
        /// <returns>
        /// true = tao duoc file & thuc hien preview printer thanh cong
        /// false = that bai
        /// </returns>
        public static bool Run(string printTypeCode, string fileName, object data)
        {
            bool result = false;
            try
            {
                return Run(printTypeCode, fileName, data, PreviewType.ShowDialog);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public static bool Run(string printTypeCode, string fileName, object data, PreviewType previewType)
        {
            bool result = false;
            try
            {
                return Run(printTypeCode, fileName, data, previewType, "");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public static bool Run(string printCode, string fileName, object data, PreviewType previewType, string printerName)
        {
            bool result = false;
            try
            {
                IProcessorPrint processor = ProcessorFactory.MakeProcessorBase(printCode, fileName, data, previewType, printerName);
                if (processor != null)
                {
                    result = processor.Run();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public static bool Run(string printCode, string fileName, object data, PreviewType previewType, string printerName, int numCopy)
        {
            bool result = false;
            try
            {
                IProcessorPrint processor = ProcessorFactoryPlus.MakeProcessorBase(printCode, fileName, data, previewType, printerName, numCopy, false);
                if (processor != null)
                {
                    result = processor.Run();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public static bool Run(string printCode, string fileName, object data, PreviewType previewType, string printerName, int numCopy, bool isPreview)
        {
            bool result = false;
            try
            {
                IProcessorPrint processor = ProcessorFactoryPlus.MakeProcessorBase(printCode, fileName, data, previewType, printerName, numCopy, isPreview);
                if (processor != null)
                {
                    result = processor.Run();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
