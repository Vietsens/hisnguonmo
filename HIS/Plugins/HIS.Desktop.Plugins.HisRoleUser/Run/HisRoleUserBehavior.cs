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
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.HisRoleUser;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.Plugins.HisRoleUser.Run;
using MOS.EFMODEL.DataModels;

namespace Inventec.Desktop.Plugins.HisRoleUser.Run
{
    public sealed class HisRoleUserBehavior : Tool<IDesktopToolContext>, IHisRoleUser
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        HIS_IMP_MEST impMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
        List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials = null;
        List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines = null;
        List<V_HIS_IMP_MEST_BLOOD> impMestBloods = null;
        public HisRoleUserBehavior()
            : base()
        {
        }

        public HisRoleUserBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisRoleUser.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is HIS_IMP_MEST)
                        {
                            impMest = (HIS_IMP_MEST)item;
                        }
                        else if (item is List<V_HIS_IMP_MEST_MATERIAL>)
                        {
                            impMestMaterials = (List<V_HIS_IMP_MEST_MATERIAL>)item;
                        }
                        else if (item is List<V_HIS_IMP_MEST_MEDICINE>)
                        {
                            impMestMedicines = (List<V_HIS_IMP_MEST_MEDICINE>)item;
                        }
                        else if (item is List<V_HIS_IMP_MEST_BLOOD>)
                        {
                            impMestBloods = (List<V_HIS_IMP_MEST_BLOOD>)item;
                        }
                    }

                    //if (currentModule != null && impMest != null && manuImpMest != null && impMestMedicines != null && impMestMaterials != null)
                    //{
                    result = new frmHisRoleUser(currentModule, impMest.ID, impMest.IMP_MEST_TYPE_ID, impMest.IMP_MEST_STT_ID, impMestMedicines, impMestMaterials, impMestBloods);
                    //}
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
