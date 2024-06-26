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
using Inventec.Core;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS
{
    public class MpsPrinter
    {
        private static ProcessorFactory factory = null;
        public static void LoadFactory()
        {
            try
            {
                factory = new ProcessorFactory();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public static void LoadFactory(Action<int> GetCurrentMpsDll, Action<int> TotalMps)
        {
            try
            {
                factory = new ProcessorFactory(GetCurrentMpsDll, TotalMps);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static bool Valid(PrintData printData)
        {
            bool valid = true;
            try
            {
                if (printData == null) throw new ArgumentNullException("printData");
                if (printData.data == null) throw new ArgumentNullException("printData.data");
                if (String.IsNullOrEmpty(printData.fileName)) throw new ArgumentNullException("printData.fileName");
                if (String.IsNullOrEmpty(printData.printTypeCode)) throw new ArgumentNullException("printData.printTypeCode");
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        public static bool Run(PrintData printData)
        {
            bool result = false;
            CommonParam param = new CommonParam();
            try
            {
                if (Valid(printData))
                {
                    var processor = printData != null ? factory.GetProcessor(printData, param) : null;

                    if (processor != null && processor is AbstractProcessor)
                    {
                        result = ((AbstractProcessor)processor).Run();
                    }
                    //try
                    //{
                    //    ((AbstractProcessor)processor).Dispose();
                    //    processor = null;
                    //}
                    //catch (Exception exx)
                    //{
                    //  Inventec.Common.Logging.LogSystem.Warn(exx);
                    //}
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
