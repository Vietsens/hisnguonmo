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
using Inventec.Desktop.Common;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.Modules;
using QCS.Desktop.Plugins.QcsQuery.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Common;

namespace QCS.Desktop.Plugins.QcsQuery
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "QCS.Desktop.Plugins.QcsQuery",
       "Danh sách ...",
       "Common",
       31,
       "newitem_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__UC,
       true,
       true)
    ]
    public class SqlProcessor : BusinessBase, IDesktopRoot
    {
        public SqlProcessor()
            : base()
        {

        }
        public SqlProcessor(CommonParam param)
            : base(param)
        {

        }

        public object Run(object[] args)
        {
            CommonParam param = new CommonParam();
            object result = null;
            try
            {
                ISql behavior = SqlFactory.MakeICrateType(param, args);
                result = behavior != null ? (UserControl)(behavior.Run()) : null;
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
