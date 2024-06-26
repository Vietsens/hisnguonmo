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
using HIS.Desktop.Plugins.ImpMestViewDetail.ImpMestViewDetail;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.ImpMestViewDetail.ImpMestViewDetail
{
    class ImpMestViewDetailBehavior : BusinessBase, IImpMestViewDetail
    {
        object[] entity;

        internal ImpMestViewDetailBehavior(CommonParam param, object[] filter)
            : base()
        {
            entity = filter;
        }

        object IImpMestViewDetail.Run()
        {
            try
            {
                ImpMestViewDetailADO impMestViewDetailADO = null;
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                DelegateSelectData delegateSelectData = null;


                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            else if (entity[i] is ImpMestViewDetailADO)
                            {
                                impMestViewDetailADO = ((ImpMestViewDetailADO)entity[i]);
                            }
                            else if (entity[i] is DelegateSelectData)
                            {
                                delegateSelectData = ((DelegateSelectData)entity[i]);
                            }
                        }
                    }
                }

                return new frmImpMestViewDetail(impMestViewDetailADO.ImpMestId, impMestViewDetailADO.IMP_MEST_TYPE_ID, impMestViewDetailADO.ImpMestSttId, moduleData, delegateSelectData);
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
