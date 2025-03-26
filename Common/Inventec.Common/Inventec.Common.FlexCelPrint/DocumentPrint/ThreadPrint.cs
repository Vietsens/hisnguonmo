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
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCelPrint.DocumentPrint
{
    class ThreadPrint
    {
        FlexCel.Render.FlexCelPrintDocument Document;

        public ThreadPrint(FlexCel.Render.FlexCelPrintDocument document)
        {
            this.Document = document;
        }

        public void Print()
        {
            Thread thread = new System.Threading.Thread(ProcessPrintNewThread);
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void ProcessPrintNewThread()
        {
            try
            {
                if (Document != null)
                {
                    using (Document)
                    {
                        if (FlexCelPrintUtil.GetConfigValue())
                        {
                            Document.PrintController = new StandardPrintController();
                        }
                        Document.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
