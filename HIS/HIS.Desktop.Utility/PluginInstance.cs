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
using DevExpress.Utils;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Utility
{
    public class PluginInstance
    {
        public static object GetPluginInstance(Inventec.Desktop.Common.Modules.Module moduleData, List<object> arrParams)
        {
            object extenceInstance = null;
            try
            {
                LogSystem.Debug("GetPluginInstance.Begin");
                string formalName = "";
                if (moduleData.PluginInfo == null || moduleData.PluginInfo.AssemblyResolve == null || moduleData.ExtensionInfo == null)
                {
                    LogSystem.Debug("GetPluginInstance.2");
                    var pinfo = Platform.LoadSingleAssembly(moduleData.ModuleLink, null);
                    if (pinfo == null || pinfo.PluginInfo == null || pinfo.PluginInfo.AssemblyResolve == null || pinfo.PluginInfo.Extensions == null || pinfo.ExtensionInfo == null)
                    {
                        LogSystem.Debug("GetPluginInstance.3");
                        LogSystem.Warn("Goi ham khoi tao module loi do khong tim thay module hoac do có loi khi khoi tao doi tuong module (GetPluginInstance)____Result data LoadSingleAssembly:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pinfo), pinfo)
                                  + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData.ModuleLink), moduleData.ModuleLink)
                                   + "____Output____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => extenceInstance), extenceInstance));

                        return extenceInstance;
                    }
                    else
                    {
                        foreach (var itemExt in pinfo.PluginInfo.Extensions)
                        {
                            //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.FirstOrDefault(o => o.ModuleLink == item);
                            var mds = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == itemExt.Code).ToList();
                            if (mds == null || mds.Count == 0) continue;

                            foreach (var md in mds)
                            {
                                md.ExtensionInfo = itemExt;
                                md.IsPlugin = true;
                                md.PluginInfo = pinfo.PluginInfo;
                                md.ModuleTypeId = itemExt.ModuleType;

                                moduleData.ModuleTypeId = itemExt.ModuleType;

                                ToolSetActionComponent ivComponent = new ToolSetActionComponent(itemExt);
                                md.ExtensionInfo.ToolSet = ivComponent.ToolSet;
                                moduleData.ExtensionInfo.ToolSet = ivComponent.ToolSet;
                                //Đoạn code bên dưới xử lý việc keiemr tra module dạng usercontrol có khai báo shortcut không
                                //Nếu có khai báo & có cấu hình customiza nút(trong bảng SDA_CUSTOMIZA_BUTTON) thì thực hiện so sánh
                                //Nếu có SDA_CUSTOMIZA_BUTTON.CURRENT_SHORTCUT == shortcut khai báo trong module sẽ thay thế = shortcut SDA_CUSTOMIZA_BUTTON.SHORTCUT
                                Inventec.Common.Logging.LogSystem.Debug("md.ExtensionInfo.ToolSet.Actions.Count=" + (md.ExtensionInfo.ToolSet != null && md.ExtensionInfo.ToolSet.Actions != null ? md.ExtensionInfo.ToolSet.Actions.Count : 0));
                                if (md.ExtensionInfo.ToolSet == null || md.ExtensionInfo.ToolSet.Actions == null || md.ExtensionInfo.ToolSet.Actions.Count == 0) continue;

                                var buttonCustomizaControls = HIS.Desktop.LocalStorage.ConfigCustomizaButton.ConfigCustomizaButtonWorker.GetByModule(md.ModuleLink);
                                if (buttonCustomizaControls != null && buttonCustomizaControls.Count > 0)
                                {
                                    foreach (Inventec.Desktop.Core.Actions.KeyboardAction iAct in md.ExtensionInfo.ToolSet.Actions)
                                    {
                                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => iAct.KeyStroke), iAct.KeyStroke));
                                        if (iAct != null && iAct.KeyStroke != XKeys.None)
                                        {
                                            SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON btnCustomizaControl = null;
                                            try
                                            {
                                                var ShortcutKeys = (System.Windows.Forms.Keys)(iAct.KeyStroke);
                                                var scDisplay = Inventec.Desktop.Core.XKeysConverter.Format(iAct.KeyStroke);
                                                btnCustomizaControl = buttonCustomizaControls.Where(o => o.CURRENT_SHORTCUT == scDisplay).FirstOrDefault();
                                                if (btnCustomizaControl == null || String.IsNullOrEmpty(btnCustomizaControl.SHORTCUT)) continue;
                                                XKeys keyNew = XKeysConverter.Parse(btnCustomizaControl.SHORTCUT);
                                                string scDisplayNew = "";
                                                if (keyNew != null && keyNew != XKeys.None)
                                                {
                                                    iAct.KeyStroke = keyNew;
                                                    scDisplayNew = Inventec.Desktop.Core.XKeysConverter.Format(iAct.KeyStroke);
                                                }

                                                Inventec.Common.Logging.LogSystem.Debug("Gan customiza shortcut cho cac nut duoc cau hinh thanh cong____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => scDisplay), scDisplay) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => scDisplayNew), scDisplayNew));
                                            }
                                            catch (Exception exx)
                                            {
                                                LogSystem.Warn("Gan customiza shortcut cho cac nut duoc cau hinh that bai____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => md.ModuleLink), md.ModuleLink) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => btnCustomizaControl), btnCustomizaControl), exx);
                                            }
                                        }
                                    }
                                }
                            }

                            try
                            {
                                var formMain = HIS.Desktop.Controls.Session.SessionManager.GetFormMain();
                                if (formMain != null)
                                {
                                    Type type = formMain.GetType();
                                    if (type != null)
                                    {
                                        System.Reflection.MethodInfo methodInfo__BuildToolStrip = type.GetMethod("BuildToolStrip");
                                        if (methodInfo__BuildToolStrip != null)
                                            methodInfo__BuildToolStrip.Invoke(formMain, new object[] { mds });
                                    }
                                }
                            }
                            catch (Exception exxx)
                            {
                                LogSystem.Warn("Refesh lai danh sach shortcut cua phan mem that bai____", exxx);
                            }
                        }

                        LogSystem.Debug("GetPluginInstance.4");
                        moduleData.PluginInfo = pinfo.PluginInfo;
                        moduleData.ExtensionInfo = pinfo.ExtensionInfo;
                        formalName = pinfo.ExtensionInfo.FormalName;
                    }
                }
                else
                {
                    LogSystem.Debug("GetPluginInstance.5");
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData.ExtensionInfo), moduleData.ExtensionInfo));
                    formalName = moduleData.ExtensionInfo.FormalName;
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => formalName), formalName));
                }

                var asm = moduleData.PluginInfo.AssemblyResolve;
                //LogSystem.Debug("GetPluginInstance.6");
                if (asm != null)
                {
                    //LogSystem.Debug("GetPluginInstance.7");
                    //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("formalName", formalName));
                    Type type = asm.GetType(formalName);
                    if (type != null)
                    {
                        //LogSystem.Debug("GetPluginInstance.8");
                        var obj = Activator.CreateInstance(type) as IDesktopRoot;
                        if (obj != null)
                        {
                            //LogSystem.Debug("GetPluginInstance.9");
                            arrParams = arrParams ?? new List<object>();
                            arrParams.Add(moduleData);
                            LogSystem.Info($"extenceInstance {extenceInstance}");
                            LogSystem.Info($"arrParams {arrParams}");
                            LogSystem.Info($"moduleData {moduleData}");
                            Inventec.Common.Logging.LogSystem.Debug("-----------------------------" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData));
                            Inventec.Common.Logging.LogSystem.Debug("-----------------------------arrParams" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => arrParams), arrParams));
                            extenceInstance = obj.Run(arrParams != null ? arrParams.ToArray() : null);
                            if (extenceInstance == null)
                            {
                                LogSystem.Warn("Goi ham khoi tao module loi do khong tim thay module hoac do có loi khi khoi tao doi tuong module (GetPluginInstance)____Input:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => arrParams), arrParams)
                                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData)
                                     + "____Output____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => extenceInstance), extenceInstance));

                                //DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa), moduleData.text + " - " + moduleData.ModuleLink), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning, DefaultBoolean.True);
                            }
                            //LogSystem.Debug("GetPluginInstance.10");
                        }
                        else
                        {
                            LogSystem.Error("obj is null, type=" + type.ToString() + ",formalName=" + formalName);
                        }
                    }
                    else
                    {
                        LogSystem.Error("type is null, formalName=" + formalName);
                    }
                }
                else
                {
                    LogSystem.Error("asm is null, formalName=" + formalName);
                }
                LogSystem.Debug("GetPluginInstance.End");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return extenceInstance;
        }

        public static void InitLoadPluginWithExistsInstance(object extenceInstance, Inventec.Desktop.Common.Modules.Module moduleData, List<object> arrParams)
        {
            try
            {
                var asm = moduleData.PluginInfo.AssemblyResolve;
                if (asm != null)
                {
                    Type type = asm.GetType(moduleData.ExtensionInfo.FormalName);
                    if (type != null)
                    {
                        var obj = Activator.CreateInstance(type) as IDesktopInit;
                        if (obj != null)
                        {
                            arrParams = arrParams ?? new List<object>();
                            arrParams.Add(moduleData);
                            //obj.RunInit(extenceInstance, arrParams != null ? arrParams.ToArray() : null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public static Inventec.Desktop.Common.Modules.Module GetModuleWithWorkingRoom(Inventec.Desktop.Common.Modules.Module moduleRaw, long roomId, long roomTypeId)
        {
            Inventec.Desktop.Common.Modules.Module moduleResult = new Inventec.Desktop.Common.Modules.Module();
            try
            {
                if (moduleRaw != null)
                {
                    moduleResult.RoomId = roomId;
                    moduleResult.RoomTypeId = roomTypeId;
                    moduleResult.Children = moduleRaw.Children;
                    moduleResult.Description = moduleRaw.Description;
                    moduleResult.ExtensionInfo = moduleRaw.ExtensionInfo;
                    moduleResult.Icon = moduleRaw.Icon;
                    moduleResult.ImageIndex = moduleRaw.ImageIndex;
                    moduleResult.IsPlugin = moduleRaw.IsPlugin;
                    moduleResult.leaf = moduleRaw.leaf;
                    moduleResult.ModuleCode = moduleRaw.ModuleCode;
                    moduleResult.ModuleLink = moduleRaw.ModuleLink;
                    moduleResult.ModuleTypeId = moduleRaw.ModuleTypeId;
                    moduleResult.NumberOrder = moduleRaw.NumberOrder;
                    moduleResult.PageGroupCaption = moduleRaw.PageGroupCaption;
                    moduleResult.PageGroupName = moduleRaw.PageGroupName;
                    moduleResult.ParentCode = moduleRaw.ParentCode;
                    moduleResult.PluginInfo = moduleRaw.PluginInfo;
                    moduleResult.text = moduleRaw.text;
                    moduleResult.Visible = moduleRaw.Visible;
                    moduleResult.IsNotShowDialog = moduleRaw.IsNotShowDialog;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return moduleResult;
        }
    }
}
