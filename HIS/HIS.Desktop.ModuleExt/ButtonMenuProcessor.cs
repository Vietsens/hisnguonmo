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
using DevExpress.XtraBars;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Token;
using System;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Utility;
using DevExpress.XtraTab;
using System.Collections.Generic;
using Inventec.Desktop.Common.Message;
using System.Linq;
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;

namespace HIS.Desktop.ModuleExt
{
    public class ButtonMenuProcessor
    {
        private const string CONFIG_KEY__HIS_DESKTOP__AUTO_RUN_MODULE_LINKS_CFG = "CONFIG_KEY__HIS_DESKTOP__AUTO_RUN_MODULE_LINKS";

        public static void MenuItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item != null && e.Item.Tag != null)
                {
                    ItemClickProcess(e.Item.Tag, e.Item.Name);
                }
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay));
                LogSystem.Error(ex);
            }
        }

        public static void MenuItemClick(object sender, TileItemEventArgs e, string name)
        {
            try
            {
                if (e.Item != null && e.Item.Tag != null)
                {
                    ItemClickProcess(e.Item.Tag, name);

                }
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay));
                LogSystem.Error(ex);
            }
        }

        static void ItemClickProcess(object data, string menuName)
        {
            string mdName = "";
            try
            {
                if (data != null)
                {
                    if (data is Inventec.Desktop.Common.Modules.Module)
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = data as Inventec.Desktop.Common.Modules.Module;


                        if (moduleData == null) throw new NullReferenceException("e.Item.Tag (Inventec.Desktop.Common.Modules.Module) is null");
                        Inventec.Common.Logging.LogSystem.Debug("ItemClickProcess.moduleData:ModuleLink=" + moduleData.ModuleLink + ",IsNotShowDialog=" + moduleData.IsNotShowDialog);
                        mdName = (moduleData.text + " - " + moduleData.ModuleLink);
                        //barItem__66__1
                        Inventec.Desktop.Common.Modules.Module module = new Inventec.Desktop.Common.Modules.Module();
                        V_HIS_ROOM room = null;
                        if (!String.IsNullOrEmpty(menuName))
                        {
                            string butttonName = menuName;
                            string[] seperator = { "__" };

                            var arrButtonSplitNames = butttonName.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                            if (arrButtonSplitNames != null && arrButtonSplitNames.Length > 2)
                            {
                                long roomId = Inventec.Common.TypeConvert.Parse.ToInt64(arrButtonSplitNames[1]);
                                long roomTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(arrButtonSplitNames[2]);
                                module = CreateModuleData(moduleData, roomId, roomTypeId);
                                room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == roomId).FirstOrDefault();
                            }
                        }
                        else
                        {
                            module = CreateModuleData(moduleData, moduleData.RoomId, moduleData.RoomTypeId);
                            room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == moduleData.RoomId).FirstOrDefault();
                        }

                        if (room != null)
                        {
                            Inventec.Common.RichEditor.RichEditorConfig.WorkingRoom = room;
                        }

                        PluginInstanceBehavior.ShowModule(module, new PluginInstanceBehavior().AddItemIntoListArg(moduleData.ModuleLink));

                        if (moduleData.ModuleLink == ModuleCodeConstant.MODULE_CHOOSE_ROOM)
                        {
                            ProcessAutoOpenModuleLink();
                        }
                    }
                    //Trường hợp click vào menu khác để mở ra module hiển thị tất cả các menu có trong tabpage đó
                    //Gọi đến module show menu in tabpage và truyền vào danh sách các module
                    else if (data is HIS.Desktop.ModuleExt.MenuAllADO)
                    {
                        var menuAllADO = data as HIS.Desktop.ModuleExt.MenuAllADO;
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MenuAll").FirstOrDefault();
                        mdName = ((moduleData != null ? moduleData.text : "") + " - " + "HIS.Desktop.Plugins.MenuAll");
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.MenuAll'");

                        List<object> listArgs = new List<object>();
                        moduleData.ModuleTypeId = Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM;
                        PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                LogSystem.Error(ex);
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                Inventec.Desktop.Common.Message.MessageManager.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa), mdName));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public static Inventec.Desktop.Common.Modules.Module CreateModuleData(Inventec.Desktop.Common.Modules.Module data, long roomId, long roomTypeId)
        {
            Inventec.Desktop.Common.Modules.Module result = new Inventec.Desktop.Common.Modules.Module();
            try
            {
                if (data == null) throw new ArgumentNullException("data");

                result.Children = data.Children;
                result.Description = data.Description;
                result.ExtensionInfo = data.ExtensionInfo;
                result.Icon = data.Icon;
                result.ImageIndex = data.ImageIndex;
                result.IsPlugin = data.IsPlugin;
                result.leaf = data.leaf;
                result.ModuleCode = data.ModuleCode;
                result.ModuleLink = data.ModuleLink;
                result.ModuleTypeId = data.ModuleTypeId;
                result.NumberOrder = data.NumberOrder;
                result.PageGroupCaption = data.PageGroupCaption;
                result.PageGroupName = data.PageGroupName;
                result.ParentCode = data.ParentCode;
                result.PluginInfo = data.PluginInfo;
                result.RoomId = roomId;
                result.RoomTypeId = roomTypeId;
                result.text = data.text;
                result.Visible = data.Visible;
                result.IsNotShowDialog = data.IsNotShowDialog;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return result;
        }

        private static void ProcessAutoOpenModuleLink()
        {
            try
            {
                if (WorkPlace.WorkPlaceSDO != null && WorkPlace.WorkPlaceSDO.Count == 1)
                {
                    string moduleLinks = ConfigApplicationWorker.Get<string>(CONFIG_KEY__HIS_DESKTOP__AUTO_RUN_MODULE_LINKS_CFG);
                    if (!String.IsNullOrWhiteSpace(moduleLinks))
                    {
                        List<HIS_ROOM_TYPE_MODULE> roomTypeModules = BackendDataWorker.Get<HIS_ROOM_TYPE_MODULE>().Where(o => o.ROOM_TYPE_ID == WorkPlace.WorkPlaceSDO.First().RoomTypeId).ToList();
                        if (roomTypeModules == null || roomTypeModules.Count <= 0)
                        {
                            return;
                        }

                        string[] moduleLink = moduleLinks.Split(';');
                        foreach (var item in moduleLink)
                        {
                            System.Threading.Thread.Sleep(500);
                            //kiểm tra thiết lập loại phòng chức năng
                            if (!roomTypeModules.Exists(o => o.MODULE_LINK == item))
                            {
                                continue;
                            }

                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.FirstOrDefault(o => o.ModuleLink == item);
                            if (moduleData != null)
                            {
                                Inventec.Desktop.Common.Modules.Module addmodule = PluginInstance.GetModuleWithWorkingRoom(moduleData, WorkPlace.WorkPlaceSDO.First().RoomId, WorkPlace.WorkPlaceSDO.First().RoomTypeId);

                                List<object> listArgs = new List<object>();
                                listArgs.Add(addmodule);

                                var extenceInstance = PluginInstance.GetPluginInstance(addmodule, listArgs);
                                if (extenceInstance == null)
                                {
                                    Inventec.Common.Logging.LogSystem.Error("extenceInstance is null");
                                    continue;
                                }

                                // Luôn dùng Show với form vì có thể mở nhiều popup
                                switch (moduleData.ModuleTypeId)
                                {
                                    case Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM:
                                        ((System.Windows.Forms.Form)extenceInstance).Show();
                                        break;
                                    case Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__UC:
                                        TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), moduleData.ExtensionInfo.Code, moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, addmodule);
                                        break;
                                    case Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__COMBO:
                                        if (extenceInstance is System.Windows.Forms.UserControl)
                                        {
                                            TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), moduleData.ExtensionInfo.Code, moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, addmodule);
                                        }
                                        else if (extenceInstance is System.Windows.Forms.Form)
                                        {
                                            ((System.Windows.Forms.Form)extenceInstance).Show();
                                        }
                                        break;
                                    default:
                                        if (extenceInstance is System.Windows.Forms.UserControl)
                                        {
                                            TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), moduleData.ExtensionInfo.Code, moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, addmodule);
                                        }
                                        else if (extenceInstance is System.Windows.Forms.Form)
                                        {
                                            ((System.Windows.Forms.Form)extenceInstance).Show();
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("Không tìm thấy module ứng với module link: " + item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
