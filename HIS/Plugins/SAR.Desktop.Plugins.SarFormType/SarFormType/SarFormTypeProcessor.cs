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
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAR.Desktop.Plugins.SarFormType.SarFormType
{
	[ExtensionOf(typeof(DesktopRootExtensionPoint),
			 "SAR.Desktop.Plugins.SarFormType",
			 "Danh mục",
			 "Bussiness",
			 4,
             "nhan-vien.png",
			 "A",
			 Module.MODULE_TYPE_ID__FORM,
			 true,
			 true)
	]
	class SarFormTypeProcessor : ModuleBase, IDesktopRoot
	{
		CommonParam param;
		public SarFormTypeProcessor()
		{
			param = new CommonParam();
		}
		public SarFormTypeProcessor(CommonParam paramBusiness)
		{
			param = (paramBusiness != null ? paramBusiness : new CommonParam());
		}

		public object Run(object[] args)
		{
			object result = null;
			try
			{
                ISarFormType behavior = SarFormTypeFactory.makeIControl(param, args);
				result = behavior != null ? (object)(behavior.Run()) : null;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				result = null;
			}
			return result;
		}

		/// <summary>
		/// Ham tra ve trang thai cua module la enable hay disable
		/// Ghi de gia tri khac theo nghiep vu tung module
		/// </summary>
		/// <returns>true/false</returns>
		public override bool IsEnable()
		{
			bool result = false;
			try
			{
				result = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				result = false;
			}

			return result;
		}
	}
}
