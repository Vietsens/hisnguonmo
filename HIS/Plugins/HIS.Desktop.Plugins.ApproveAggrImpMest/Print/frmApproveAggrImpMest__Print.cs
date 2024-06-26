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
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ApproveAggrImpMest.ADO;
using HIS.Desktop.Plugins.ApproveAggrImpMest.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApproveAggrImpMest
{
    public partial class frmApproveAggrImpMest : FormBase
    {
        enum PrintType
        {
            TRA_DOI_THUOC,
            PHIEU_TRA_TONG_HOP,
            PHIEU_TRA_THUOC_GAY_NGHIEN_HUONG_THAN,
            PHIEU_TRA_THUOC,
            BENH_NHAN
        }

        private void FillDataToButtonPrint()
        {
            try
            {

                DXPopupMenu menu = new DXPopupMenu();

                DXMenuItem itemTraDoiThuoc = new DXMenuItem("In phiếu trả đổi thuốc", new EventHandler(onClickPrint));
                itemTraDoiThuoc.Tag = PrintType.TRA_DOI_THUOC;
                menu.Items.Add(itemTraDoiThuoc);

                DXMenuItem itemTraTongHop = new DXMenuItem("In phiếu trả thuốc tổng hợp", new EventHandler(onClickPrint));
                itemTraTongHop.Tag = PrintType.PHIEU_TRA_TONG_HOP;
                menu.Items.Add(itemTraTongHop);

                DXMenuItem itemTraThuocGayNghienHuongThan = new DXMenuItem("In phiếu trả thuốc gây nghiện, hướng thần", new EventHandler(onClickPrint));
                itemTraThuocGayNghienHuongThan.Tag = PrintType.PHIEU_TRA_THUOC_GAY_NGHIEN_HUONG_THAN;
                menu.Items.Add(itemTraThuocGayNghienHuongThan);

                DXMenuItem itemTraThuocVatTu = new DXMenuItem("In phiếu trả thuốc, vật tư", new EventHandler(onClickPrint));
                itemTraThuocVatTu.Tag = PrintType.PHIEU_TRA_THUOC;
                menu.Items.Add(itemTraThuocVatTu);

                DXMenuItem itemTraBenhNhan = new DXMenuItem("In phiếu trả theo bệnh nhân", new EventHandler(onClickPrint));
                itemTraBenhNhan.Tag = PrintType.BENH_NHAN;
                menu.Items.Add(itemTraBenhNhan);

                btnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void onClickPrint(object sender, EventArgs e)
        {
            try
            {
                PrintType printType;
                if (sender is DXMenuItem)
                {
                    var btn = sender as DXMenuItem;
                    printType = (PrintType)(btn.Tag);
                    switch (printType)
                    {
                        case PrintType.TRA_DOI_THUOC:
                            PrintByPrintType(1);
                            break;
                        case PrintType.PHIEU_TRA_TONG_HOP:
                            PrintByPrintType(2);
                            break;
                        case PrintType.PHIEU_TRA_THUOC_GAY_NGHIEN_HUONG_THAN:
                            PrintByPrintType(3);
                            break;
                        case PrintType.PHIEU_TRA_THUOC:
                            PrintByPrintType(4);
                            break;
                        case PrintType.BENH_NHAN:
                            PrintByPrintType(5);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void PrintByPrintType(long printType)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrImpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrImpMestPrintFilter");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    V_HIS_IMP_MEST impMestMap = new V_HIS_IMP_MEST();
                    AutoMapper.Mapper.CreateMap<V_HIS_IMP_MEST_2, V_HIS_IMP_MEST>();
                    impMestMap = AutoMapper.Mapper.Map<V_HIS_IMP_MEST>(this.impMest);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(impMestMap);
                    listArgs.Add(printType);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
