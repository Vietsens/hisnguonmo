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
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.XMLViewer130.XMLViewer130
{
    class XMLViewer130Behavior : Tool<IDesktopToolContext>, IXMLViewer130
    {
        string url;
        MemoryStream mmStream;
        long? dataType;
        Inventec.Desktop.Common.Modules.Module Module;
        internal XMLViewer130Behavior()
            : base()
        {

        }

        internal XMLViewer130Behavior(Inventec.Desktop.Common.Modules.Module module, string data)
            : base()
        {
            this.Module = module;
            this.url = data;
        }
        internal XMLViewer130Behavior(Inventec.Desktop.Common.Modules.Module module, MemoryStream _mmStream)
            : base()
        {
            this.Module = module;
            this.mmStream = _mmStream;
        }
        internal XMLViewer130Behavior(Inventec.Desktop.Common.Modules.Module module, string data, long? dataType)
            : base()
        {
            this.Module = module;
            this.url = data;
            this.dataType = dataType;
        }
        internal XMLViewer130Behavior(Inventec.Desktop.Common.Modules.Module module, MemoryStream _mmStream, long? dataType)
            : base()
        {
            this.Module = module;
            this.mmStream = _mmStream;
            this.dataType = dataType;
        }

        internal XMLViewer130Behavior(Inventec.Desktop.Common.Modules.Module module)
            : base()
        {
            this.Module = module;
        }

        object IXMLViewer130.Run()
        {
            object result = null;
            try
            {
                if (this.Module != null && !string.IsNullOrEmpty(url) && dataType != null)
                {
                    result = new frmXMLViewer130(Module, url, dataType);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Module), Module));
                }
                else if (this.Module != null && mmStream != null && dataType != null)
                {
                    result = new frmXMLViewer130(Module, mmStream, dataType);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Module), Module));
                }
                else if (this.Module != null && !string.IsNullOrEmpty(url))
                {
                    result = new frmXMLViewer130(Module, url);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Module), Module));
                }
                else if (this.Module != null && mmStream != null)
                {
                    result = new frmXMLViewer130(Module, mmStream);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Module), Module));
                }
                else
                {
                    result = new frmXMLViewer130();
                    Inventec.Common.Logging.LogSystem.Error("Module is null****");
                }
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
