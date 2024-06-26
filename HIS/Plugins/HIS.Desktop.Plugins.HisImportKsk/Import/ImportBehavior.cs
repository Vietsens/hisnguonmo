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
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.HisImportKsk.FormLoad;

namespace HIS.Desktop.Plugins.HisImportKsk
{
    class ImportBehavior : BusinessBase, IImport
    {
        object[] entity;
        //DelegateRefreshData delegateRefresh = null;

        internal ImportBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IImport.Run()
        {
            object frm = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                RefeshReference refeshReference = null;
                DelegateSelectData selectData = null;
                //List<ServiceImportADO> service = null;
                //List<ServiceImportADO> service = null;


                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is RefeshReference)
                            {
                                refeshReference = (RefeshReference)entity[i];
                            }
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            if (entity[i] is DelegateSelectData)
                            {
                                selectData = (DelegateSelectData)entity[i];
                            }
                        }
                    }
                }

                if (moduleData != null && refeshReference != null)
                {
                    frm = new frmKsk(moduleData, refeshReference);
                }
                else if (moduleData != null && refeshReference == null && selectData == null)
                {
                    frm = new frmKsk(moduleData);
                }
                else if (moduleData != null && selectData != null)
                {
                    frm = new frmKsk(moduleData, selectData);
                }
                return frm;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
