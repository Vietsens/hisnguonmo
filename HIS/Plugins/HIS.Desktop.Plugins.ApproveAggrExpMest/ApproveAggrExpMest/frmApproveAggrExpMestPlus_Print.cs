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
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ApproveAggrExpMest.ApproveAggrExpMest.AggregateExpMestPrintFilter;
using HIS.Desktop.Plugins.ApproveAggrExpMest.Base;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApproveAggrExpMest.ApproveAggrExpMest
{
    public partial class frmApproveAggrExpMest : HIS.Desktop.Utility.FormBase
    {
        internal void PrintAggregateExpMest()
        {
            try
            {
                if (barManager1 == null)
                {
                    barManager1 = new DevExpress.XtraBars.BarManager();
                    barManager1.Form = this;
                }

                ExpMestAggregateListPopupMenuProcessor processor = new ExpMestAggregateListPopupMenuProcessor(this.AggrExpMest, ExpMestAggregateMouseRightClick, barManager1);
                processor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void PrintAggregateExpMest(V_HIS_AGGR_EXP_MEST currentAggExpMest)
        {
            try
            {
                this.AggrExpMest = currentAggExpMest;
                if (barManager1 == null)
                {
                    barManager1 = new DevExpress.XtraBars.BarManager();
                    barManager1.Form = this;
                }

                ExpMestAggregateListPopupMenuProcessor processor = new ExpMestAggregateListPopupMenuProcessor(this.AggrExpMest, ExpMestAggregateMouseRightClick, barManager1);
                processor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ExpMestAggregateMouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (e.Item.Tag is ExpMestAggregateListPopupMenuProcessor.PrintType)
                {
                    //frmAggregateExpMestPrintFilter formPrintFilter;
                    var moduleType = (ExpMestAggregateListPopupMenuProcessor.PrintType)e.Item.Tag;
                    switch (moduleType)
                    {
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InTraDoiThuoc:
                            ShowFormFilter(1);
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuTongHop:
                           
                            ShowFormFilter(2);
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhThuoc:
                            
                            ShowFormFilter(3);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowFormFilter(long printType)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.AggrExpMest);
                    listArgs.Add(printType);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
