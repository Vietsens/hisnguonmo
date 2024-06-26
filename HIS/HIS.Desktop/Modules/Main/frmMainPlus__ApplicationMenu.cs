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
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraTab;
using HIS.Desktop.Base;
using Inventec.Common.Logging;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Common.Modules;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.LocalData;
using System.Linq;
using HIS.Desktop.ModuleExt;

namespace HIS.Desktop.Modules.Main
{
    public partial class frmMain : RibbonForm
    {
        List<Module> applicationModules = new List<Module>();
        private void CreateApplicationMenu()
        {
            try
            {
                applicationMenu1.ClearLinks();
                List<MOS.SDO.WorkPlaceSDO> workPlaces = new List<MOS.SDO.WorkPlaceSDO>();

                MOS.SDO.WorkPlaceSDO wp1 = new MOS.SDO.WorkPlaceSDO();
                wp1.RoomName = Inventec.Common.Resource.Get.Value("frmMain.bbtnPageOther.Caption", HIS.Desktop.Resources.ResourceLanguageManager.LanguageFrmMain, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                wp1.RoomCode = "ModuleTypeCode__NotInRole";
                workPlaces.Add(wp1);

                workPlaces.AddRange(WorkPlace.WorkPlaceSDO);
                workPlaces = workPlaces.OrderBy(o => o.RoomName).ToList();

                foreach (var wp in workPlaces)
                {
                    BarItem bitem = new BarButtonItem();
                    bitem.PaintStyle = BarItemPaintStyle.Caption;
                    bitem.RibbonStyle = RibbonItemStyles.Default;
                    bitem.Caption = wp.RoomName;
                    bitem.Tag = wp;
                    bitem.ItemClick += ApplicationMenuItemClick;
                    applicationMenu1.AddItem(bitem);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ApplicationMenuItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item != null && e.Item.Tag != null)
                {
                    MOS.SDO.WorkPlaceSDO workPlace = e.Item.Tag as MOS.SDO.WorkPlaceSDO;
                    if (workPlace != null)
                    {
                        foreach (RibbonPage page in ribbonMain.Pages)
                        {
                            if (workPlace.RoomId > 0 && workPlace != null && (page.Tag ?? "").ToString() == "ModuleTypeCode__InRole__" + workPlace.RoomCode + "__" + workPlace.RoomTypeId + "__")
                            {
                                ribbonMain.SelectedPage = page;
                                break;
                            }
                            else if ((page.Tag ?? "").ToString() == workPlace.RoomCode)
                            {
                                ribbonMain.SelectedPage = page;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.MessageManager.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay));
                LogSystem.Error(ex);
            }
        }
    }
}
