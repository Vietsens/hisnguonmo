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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using AutoMapper;
using HIS.Desktop.Common;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.ImpMestAggregate
{
    public partial class UCImpMestAggregate : HIS.Desktop.Utility.UserControlBase
    {
        internal PopupMenu menu;
        internal V_HIS_IMP_MEST_2 row_aggImpMest { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2 currentAggImppMest;

        internal void PrintAggregateImpMest(V_HIS_IMP_MEST_2 currentAggImpMest)
        {
            try
            {
                this.currentAggImppMest = currentAggImpMest;
                if (barManager1 == null)
                {
                    barManager1 = new DevExpress.XtraBars.BarManager();
                    barManager1.Form = this;
                }

                ImpMestAggregateListPopupMenuProcessor processor = new ImpMestAggregateListPopupMenuProcessor(this.currentAggImppMest, null, ImpMestAggregateMouseRightClick, barManager1);
                processor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ImpMestAggregateMouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (e.Item.Tag is ImpMestAggregateListPopupMenuProcessor.PrintType)
                {
                    var moduleType = (ImpMestAggregateListPopupMenuProcessor.PrintType)e.Item.Tag;
                    switch (moduleType)
                    {
                        case ImpMestAggregateListPopupMenuProcessor.PrintType.InTraDoiThuoc:
                            ShowFormFilter(ImpMestAggregateListPopupMenuProcessor.PrintType.InTraDoiThuoc);
                            break;
                        case ImpMestAggregateListPopupMenuProcessor.PrintType.InPhieuTraTongHop:
                            ShowFormFilter(ImpMestAggregateListPopupMenuProcessor.PrintType.InPhieuTraTongHop);
                            break;
                        case ImpMestAggregateListPopupMenuProcessor.PrintType.InPhieuTraThuocGayNghienHuongTT:
                            ShowFormFilter(ImpMestAggregateListPopupMenuProcessor.PrintType.InPhieuTraThuocGayNghienHuongTT);
                            break;
                        case ImpMestAggregateListPopupMenuProcessor.PrintType.InPhieuTraThuoc:
                            ShowFormFilter(ImpMestAggregateListPopupMenuProcessor.PrintType.InPhieuTraThuoc);
                            break;
                        case ImpMestAggregateListPopupMenuProcessor.PrintType.TheoBenhNhan:
                            ShowFormFilter(ImpMestAggregateListPopupMenuProcessor.PrintType.TheoBenhNhan);
                            break;
                        case ImpMestAggregateListPopupMenuProcessor.PrintType.InTraDoiThuocTongHop:
                            ShowFormFilter(ImpMestAggregateListPopupMenuProcessor.PrintType.InTraDoiThuocTongHop, true);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowFormFilter(ImpMestAggregateListPopupMenuProcessor.PrintType PrintType, bool selectMulti = false)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrImpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) 
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrImpMestPrintFilter"); 
                    return; 
                }
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    V_HIS_IMP_MEST impMest = new V_HIS_IMP_MEST();
                    AutoMapper.Mapper.CreateMap<V_HIS_IMP_MEST_2, V_HIS_IMP_MEST>();
                    impMest = AutoMapper.Mapper.Map<V_HIS_IMP_MEST>(this.currentAggImppMest);
                    List<V_HIS_IMP_MEST_2> _ImpMestTraDoiChecks = new List<V_HIS_IMP_MEST_2>();
                    List<object> listArgs = new List<object>();
                    if (selectMulti)
                    {                       
                        if (gridViewAggrImpMest.RowCount > 0)
                        {
                            for (int i = 0; i < gridViewAggrImpMest.SelectedRowsCount; i++)
                            {
                                if (gridViewAggrImpMest.GetSelectedRows()[i] >= 0)
                                {
                                    _ImpMestTraDoiChecks.Add((V_HIS_IMP_MEST_2)gridViewAggrImpMest.GetRow(gridViewAggrImpMest.GetSelectedRows()[i]));
                                }
                            }
                        }
                        if (_ImpMestTraDoiChecks != null && _ImpMestTraDoiChecks.Count > 0)
                        {
                            listArgs.Add(_ImpMestTraDoiChecks);
                        }
                    }
                    listArgs.Add(impMest);
                    listArgs.Add((long)PrintType);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    if (extenceInstance.GetType() == typeof(bool))
                    {
                        return;
                    }
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
