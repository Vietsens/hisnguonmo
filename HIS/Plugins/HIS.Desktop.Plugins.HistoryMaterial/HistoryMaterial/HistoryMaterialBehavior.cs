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
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.HistoryMaterial.HistoryMaterial;

namespace HIS.Desktop.Plugins.HistoryMaterial
{
    class HistoryMaterialBehavior : BusinessBase, IHistoryMaterial
    {
        object[] entity;
        long materialTypeId;
        Inventec.Desktop.Common.Modules.Module _Module;
        internal HistoryMaterialBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHistoryMaterial.Run()
        {
            object rs = null;
            try
            {
                List<long> mediStockIds = null;
                string PACKAGE_NUMBER = "";

                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is long)
                            {
                                materialTypeId = (long)entity[i];
                            }
                            else if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                _Module = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            else if (entity[i] is string)
                            {
                                PACKAGE_NUMBER = (string)entity[i];
                            }
                        }
                    }
                }
                if (materialTypeId > 0 && _Module != null)
                {
                    rs = new frmHistory(_Module, materialTypeId, PACKAGE_NUMBER);
                }
                else if (materialTypeId > 0 && _Module != null)
                {
                    rs = new frmHistory(_Module, materialTypeId);
                }
                else if (materialTypeId == 0 && _Module != null)
                {
                    rs = new UC_HistoryMaterial(_Module);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
            return rs;
        }
    }
}