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
using HIS.Desktop.Plugins.AggrExpMestPrintFilter;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.AggrExpMestPrintFilter.Run;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.AggrExpMestPrintFilter.AggrExpMestPrintFilter
{
    public sealed class AggrExpMestPrintFilterBehavior : Tool<IDesktopToolContext>, IAggrExpMestPrintFilter
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_EXP_MEST aggrExpMest;
        List<V_HIS_EXP_MEST> ListExpMestTraDoi;
        public long printKey;

        public AggrExpMestPrintFilterBehavior()
            : base()
        {
        }

        public AggrExpMestPrintFilterBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IAggrExpMestPrintFilter.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    AggrExpMestPrintSDO printSdo = null;
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is V_HIS_EXP_MEST)
                        {
                            aggrExpMest = (V_HIS_EXP_MEST)item;
                        }
                        else if (item is long)
                        {
                            printKey = (long)item;
                        }
                        else if (item is List<V_HIS_EXP_MEST>)
                        {
                            ListExpMestTraDoi = (List<V_HIS_EXP_MEST>)item;
                        }
                        else if (item is AggrExpMestPrintSDO)
                        {
                            printSdo = (AggrExpMestPrintSDO)item;
                        }
                    }

                    if (currentModule != null && (aggrExpMest != null || ListExpMestTraDoi != null) && printKey > 0)
                    {
                        if (printKey == 3 || printKey == 4)//|| printKey == 5)
                        {
                            PrintNow _PrintNow = new PrintNow(currentModule);
                            if (ListExpMestTraDoi == null)
                            {
                                ListExpMestTraDoi = new List<V_HIS_EXP_MEST>();
                            }

                            if (aggrExpMest != null)
                            {
                                ListExpMestTraDoi.Add(aggrExpMest);
                            }

                            _PrintNow.RunPrintNow(currentModule.RoomId, ListExpMestTraDoi, printKey, printSdo);
                            _PrintNow = null;
                            result = true;
                        }
                        else if (printKey == 5 || printKey == 7)
                        {
                            if (ListExpMestTraDoi == null)
                            {
                                ListExpMestTraDoi = new List<V_HIS_EXP_MEST>();
                            }

                            if (aggrExpMest != null)
                            {
                                ListExpMestTraDoi.Add(aggrExpMest);
                            }

                            result = new frmAggregateExpMestPrintFilter(currentModule, ListExpMestTraDoi, printKey, printSdo);
                        }

                        else
                        {
                            result = new frmAggregateExpMestPrintFilter(currentModule, aggrExpMest, printKey, printSdo);
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
