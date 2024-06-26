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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.XMLViewer.XMLViewer
{
    class XMLViewerBehaviorFactory
    {
        internal static IXMLViewer MakeITransactionDepositCancel(object[] data)
        {
            IXMLViewer result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            string FilePathDefault = null;
            MemoryStream mmStream = null;
            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is string)
                            {
                                FilePathDefault = (string)data[i];
                            }
                            else if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                            else if (data[i] is MemoryStream)
                            {
                                mmStream = (MemoryStream)data[i];
                            }
                        }

                        if (moduleData != null && FilePathDefault != null)
                        {
                            result = new XMLViewerBehavior(moduleData, FilePathDefault);
                        }
                        else if (mmStream != null && moduleData != null)
                        {
                            result = new XMLViewerBehavior(moduleData, mmStream);
                        }
                        else if (moduleData != null)
                        {
                            result = new XMLViewerBehavior(moduleData);
                        }
                        else
                        {
                            result = new XMLViewerBehavior();
                        }
                    }
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
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
