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
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;

namespace HIS.Desktop.Plugins.TextLibrary.TextLibrary
{
    class TextLibraryBehavior : Tool<IDesktopToolContext>, ITextLibrary
    {
        object[] entity;
        internal TextLibraryBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ITextLibrary.Run()
        {
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            string text = "";
            HIS.Desktop.ADO.TextLibraryInfoADO textLibraryInfoADO = null;
            HIS.Desktop.Common.DelegateDataTextLib hashTag = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        if (item is string)
                            text = (string)item;
                        if (item is HIS.Desktop.Common.DelegateDataTextLib)
                            hashTag = (HIS.Desktop.Common.DelegateDataTextLib)item;
                        if (item is HIS.Desktop.ADO.TextLibraryInfoADO)
                            textLibraryInfoADO = (HIS.Desktop.ADO.TextLibraryInfoADO)item;
                    }
                }
                if (moduleData != null)
                {
                    return new FormTextLibrary(text, hashTag, moduleData, textLibraryInfoADO);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
