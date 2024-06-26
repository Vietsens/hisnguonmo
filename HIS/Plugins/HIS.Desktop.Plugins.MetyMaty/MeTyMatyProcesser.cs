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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.MetyMaty;
using HIS.Desktop.Plugins.MetyMaty.ADO;
using MOS.EFMODEL.DataModels;


namespace HIS.Desktop.Plugins.MetyMaty
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
     "HIS.Desktop.Plugins.MetyMaty",
     "Danh mục",
     "Bussiness",
     4,
     "showproduct_32x32.png",
     "A",
     Module.MODULE_TYPE_ID__FORM,
     true,
     true)
  ]
    public class MetyMatyProcessor : ModuleBase, IDesktopRoot
    {

        CommonParam param;
        public MetyMatyProcessor()
        {
            param = new CommonParam();
        }
        public MetyMatyProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                List<HIS_METY_MATY> LisMetyMaty = null;
                List<HIS_METY_METY> LisMetyMety = null;
                V_HIS_METY_PRODUCT VMetProduct = null;
                DelegateReturnMutilObject _delegate1 = null;
                if (args.GetType() == typeof(object[]))
                {
                    if (args != null && args.Count() > 0)
                    {
                        for (int i = 0; i < args.Count(); i++)
                        {
                            if (args[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)args[i];
                            }
                            if (args[i] is List<HIS_METY_MATY>)
                            {
                                LisMetyMaty = (List<HIS_METY_MATY>)args[i];
                            }
                            if (args[i] is List<HIS_METY_METY>)
                            {
                                LisMetyMety = (List<HIS_METY_METY>)args[i];
                            }
                            if (args[i] is V_HIS_METY_PRODUCT)
                            {
                                VMetProduct = (V_HIS_METY_PRODUCT)args[i];
                            }
                            if (args[i] is DelegateReturnMutilObject)
                            {
                                _delegate1 = (DelegateReturnMutilObject)args[i];
                               
                            }
                        }
                    }
                }

                if (LisMetyMaty == null || LisMetyMety == null)
                    result = new HIS.Desktop.Plugins.MetyMaty.MetyMatyForm(moduleData);
                else
                    result = new HIS.Desktop.Plugins.MetyMaty.MetyMatyForm(moduleData, LisMetyMaty,LisMetyMety,VMetProduct.ID, _delegate1);
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
