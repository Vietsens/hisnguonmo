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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.AggrImpMestPrintFilter;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.AggrImpMestPrintFilter.Run;

namespace HIS.Desktop.Plugins.AggrImpMestPrintFilter.AggrImpMestPrintFilter
{
    public sealed class AggrImpMestPrintFilterBehavior : Tool<IDesktopToolContext>, IAggrImpMestPrintFilter
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_IMP_MEST aggrExpMest;//= new V_HIS_AGGR_EXP_MEST();
        V_HIS_IMP_MEST_2 aggrExpMest2;
        List<V_HIS_IMP_MEST_2> ListExpMestTraDoi;
        List<V_HIS_IMP_MEST> ListExpMestTraDoi1;

        long printKey;
        public AggrImpMestPrintFilterBehavior()
            : base()
        {
        }

        public AggrImpMestPrintFilterBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IAggrImpMestPrintFilter.Run()
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
                        else if (item is V_HIS_IMP_MEST)
                        {
                            aggrExpMest = (V_HIS_IMP_MEST)item;
                        }
                        else if (item is long)
                        {
                            printKey = (long)item;
                        }
                        else if (item is List<V_HIS_IMP_MEST_2>)
                        {
                            ListExpMestTraDoi = (List<V_HIS_IMP_MEST_2>)item;
                        }
                        else if (item is List<V_HIS_IMP_MEST>)
                        {
                            ListExpMestTraDoi1 = (List<V_HIS_IMP_MEST>)item;
                        }
                    }
                    if (currentModule != null && (aggrExpMest != null || ListExpMestTraDoi != null || ListExpMestTraDoi1 != null) && printKey > 0)
                    {
                        if (printKey == 5 || printKey == 6 || printKey == 7)
                        {
                            PrintNow _PrintNow = new PrintNow(currentModule);
                            if (ListExpMestTraDoi == null)
                            {
                                ListExpMestTraDoi = new List<V_HIS_IMP_MEST_2>();
                            }
                            if (ListExpMestTraDoi1 == null)
                            {
                                ListExpMestTraDoi1 = new List<V_HIS_IMP_MEST>();
                            }
                            if (aggrExpMest != null)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_IMP_MEST, V_HIS_IMP_MEST_2>();
                                aggrExpMest2 = AutoMapper.Mapper.Map<V_HIS_IMP_MEST_2>(aggrExpMest);
                                ListExpMestTraDoi.Add(aggrExpMest2);
                                ListExpMestTraDoi1.Add(aggrExpMest);
                            }
                            if (printKey == 5)
                            {
                                _PrintNow.RunPrintNow(currentModule.RoomId, aggrExpMest, printKey);
                            }
                            if (printKey == 6) 
                            {
                                _PrintNow.RunPrintNow(currentModule.RoomId, ListExpMestTraDoi, printKey);
                            }
                            if (printKey == 7)
                            {
                                _PrintNow.RunPrintNow(currentModule.RoomId, ListExpMestTraDoi1, printKey);
                            }
                            result = true;
                        }
                        else
                        {
                            result = new frmAggregateImpMestPrintFilter(currentModule, aggrExpMest, printKey);
                            
                        }
                    }
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
