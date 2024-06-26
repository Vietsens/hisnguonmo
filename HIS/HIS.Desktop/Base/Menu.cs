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
using ACS.Filter;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.Core;
using HIS.Desktop.LocalStorage.ConfigCustomizaButton;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Base
{
    public partial class Menu
    {
        public static List<ACS.EFMODEL.DataModels.ACS_MODULE_GROUP> ModuleGroups { get; set; }

        internal static bool IsInited = true;
        string[] arrSeperator = new string[] { "_" };
        int maxTabPageItemCount = Inventec.Common.TypeConvert.Parse.ToInt32((ConfigurationManager.AppSettings["His.Desktop.MaxTabPageItemCount"] ?? "15").ToString());
        public Menu()
        { }

        /// <summary>
        /// Tao du lieu danh sach chuc nang cho phan mem
        /// </summary>
        internal void Init()
        {
            try
            {
                //Lay ve tat ca cac module duoc phan quyen
                //Lay tat ca du lieu cau hinh roomtypemodule
                //Kiem tra trong danh sach module, 
                //neu module co trong cau hinh roomtypemodule va roomtype duoc chon thi day vao cac page theo phong
                //Cac module khong co trong cau hinh roomtypemodule & phai chọn phong thi visible đi
                //Cac module con lai se day vao page khác
                //Tat ca cac module trong cac page sẽ tổ chức theo cha con: 
                //    + nếu module có cha mà cha lại không được cấu hình sử dụng thì các module đó sẽ hiển thị dạng button icon lớn
                //    + nếu module có cha và cha có trong cấu hình -> hiển thị button menu, click vào cha sổ xuống các button của module con, hiển thị icon nhỏ

                ListModule = new List<Module>();
                CommonParam param = new CommonParam();
                ACS.SDO.AcsTokenLoginSDO tokenLoginSDOForAuthorize = new ACS.SDO.AcsTokenLoginSDO();
                tokenLoginSDOForAuthorize.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                tokenLoginSDOForAuthorize.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;
                if (System.IO.File.Exists((System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, "readme.txt"))))
                    tokenLoginSDOForAuthorize.APP_VERSION = System.IO.File.ReadAllText(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, "readme.txt"));
                GlobalVariables.AcsAuthorizeSDO = new BackendAdapter(param).Get<ACS.SDO.AcsAuthorizeSDO>(HIS.Desktop.ApiConsumer.AcsRequestUriStore.ACS_TOKEN__AUTHORIZE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, tokenLoginSDOForAuthorize, param);
                if (param.BugCodes.Contains("ACS401"))
                {
                    Inventec.Common.Logging.LogSystem.Warn(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai));
                    HIS.Desktop.Controls.Session.SessionManager.ActionLostToken();
                    GlobalVariables.IsLostToken = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                ListModule = null;
                LogSystem.Error(ex);
            }
        }

        private List<Module> ListModule { get;set; }

        public void ProcessDataModule()
        {
            try
            {
                if (GlobalVariables.AcsAuthorizeSDO != null
                    && GlobalVariables.AcsAuthorizeSDO.ModuleInRoles != null
                    && GlobalVariables.AcsAuthorizeSDO.ModuleInRoles.Count > 0
                    )
                {
                    Inventec.Common.Logging.LogSystem.Debug("Init => ModuleInRoles.Count =" + (GlobalVariables.AcsAuthorizeSDO.ModuleInRoles != null ? GlobalVariables.AcsAuthorizeSDO.ModuleInRoles.Count : -1) + " | " + "ControlInRoles.Count =" + (GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null ? GlobalVariables.AcsAuthorizeSDO.ControlInRoles.Count : -1));
                    List<ACS.EFMODEL.DataModels.ACS_MODULE> acsModules = new List<ACS.EFMODEL.DataModels.ACS_MODULE>();
                    foreach (var am in GlobalVariables.AcsAuthorizeSDO.ModuleInRoles)
                    {
                        var checkExists = acsModules.FirstOrDefault(o => o.ID == am.ID);
                        if (checkExists == null)
                        {
                            ACS.EFMODEL.DataModels.ACS_MODULE module = new ACS.EFMODEL.DataModels.ACS_MODULE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ACS.EFMODEL.DataModels.ACS_MODULE>(module, am);
                            acsModules.Add(module);
                        }
                    }

                    if (TranslateDataWorker.Language != null && TranslateDataWorker.Language.IS_BASE != 1)
                    {
                        TranslateWorker.TranslateData<ACS.EFMODEL.DataModels.ACS_MODULE>(acsModules);
                    }

                    CommonParam param = new CommonParam();
                    AcsModuleGroupFilter moduleGroupFilter = new AcsModuleGroupFilter();
                    DATA.ModuleGroups = new BackendAdapter(param).Get<List<ACS.EFMODEL.DataModels.ACS_MODULE_GROUP>>(AcsRequestUriStore.ACS_MODULE_GROUP_GET, ApiConsumers.AcsConsumer, moduleGroupFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (TranslateDataWorker.Language != null && TranslateDataWorker.Language.IS_BASE != 1)
                    {
                        TranslateWorker.TranslateData<ACS.EFMODEL.DataModels.ACS_MODULE_GROUP>(DATA.ModuleGroups);
                    }

                    foreach (var mod in acsModules)
                    {
                        if (mod == null || String.IsNullOrEmpty(mod.MODULE_LINK))
                        {
                            LogSystem.Debug("Khong xac dinh duoc module duoc cau hinh tren danh muc module tren acs. MODULE_LINK = " + (mod != null ? mod.MODULE_LINK : "") + ", APPLICATION_CODE = " + GlobalVariables.APPLICATION_CODE);
                        }
                        else
                        {
                            var parent = GetByCode((mod.PARENT_ID == null ? "" : mod.PARENT_ID.ToString()), GlobalVariables.AcsAuthorizeSDO.ModuleInRoles);
                            var mdg = DATA.ModuleGroups.FirstOrDefault(o => o.ID == mod.MODULE_GROUP_ID) ?? new ACS.EFMODEL.DataModels.ACS_MODULE_GROUP();

                            Module module = new Module(
                                mod.ID.ToString(),
                                mod.MODULE_LINK,
                                mod.MODULE_NAME,
                                (mod.IS_LEAF == 1),
                                Module.MODULE_TYPE_ID__UC,
                                (mod.MODULE_LINK == ModuleCodeConstant.MODULE_LOG_OUT ? 0 : 0),
                                (int)(mod.NUM_ORDER ?? 0),
                                ((parent == null || parent.ID == 0) ? "" : parent.ID.ToString()),
                                mdg.MODULE_GROUP_NAME,
                                mdg.MODULE_GROUP_CODE,
                                0,
                                false,
                                null,
                                null,
                                (mod.IS_VISIBLE == 1)
                            );
                            module.Description = (mdg.NUM_ORDER ?? 0).ToString();
                            module.Icon = mod.ICON_LINK;
                            module.IsNotShowDialog = (mod.IS_NOT_SHOW_DIALOG.HasValue && mod.IS_NOT_SHOW_DIALOG == 1);

                            module.ExtensionInfo = new ExtensionInfo(null, null, null, null, null, null, 0, null, 0, true, null);
                            module.IsPlugin = true;

                            ListModule.Add(module);
                        }
                    }

                    //TODO
                    //if (!Platform.IsLazyLoadPlugins)
                    //{
                    //    string moduleLinkApplys = ";HIS.Desktop.Plugins.AssignService;HIS.Desktop.Plugins.AssignPrescriptionPK;HIS.Desktop.Plugins.ChooseRoom;HIS.Desktop.Plugins.ExecuteRoom;HIS.Desktop.Plugins.ExamServiceReqExecute;HIS.Desktop.Plugins.TreatmentList;HIS.Desktop.Plugins.ServiceExecute;HIS.Desktop.Plugins.RegisterV2;";
                    //    Platform.PluginHasAuthenticates = listModule.Where(o => moduleLinkApplys.Contains(o.ModuleLink)).ToDictionary(o => o.ModuleLink.ToLower(), o => o.ModuleLink);
                    //}

                    LogSystem.Debug("PluginHasAuthenticates.Count =" + (Platform.PluginHasAuthenticates != null ? Platform.PluginHasAuthenticates.Count : 0));
                    var plugins = Platform.PluginManager.Plugins;

                    if (plugins != null && plugins.Count > 0)
                    {
                        foreach (var item in plugins)
                        {
                            if (item.Extensions == null || item.Extensions.Count == 0) continue;

                            foreach (var itemExt in item.Extensions)
                            {
                                var mds = ListModule.Where(o => o.ModuleLink == itemExt.Code).ToList();
                                if (mds == null || mds.Count == 0) continue;

                                foreach (var md in mds)
                                {
                                    md.ExtensionInfo = itemExt;
                                    md.IsPlugin = true;
                                    md.PluginInfo = item;
                                    md.ModuleTypeId = itemExt.ModuleType;

                                    ToolSetActionComponent ivComponent = new ToolSetActionComponent(itemExt);
                                    md.ExtensionInfo.ToolSet = ivComponent.ToolSet;

                                    //Đoạn code bên dưới xử lý việc keiemr tra module dạng usercontrol có khai báo shortcut không
                                    //Nếu có khai báo & có cấu hình customiza nút(trong bảng SDA_CUSTOMIZA_BUTTON) thì thực hiện so sánh
                                    //Nếu có SDA_CUSTOMIZA_BUTTON.CURRENT_SHORTCUT == shortcut khai báo trong module sẽ thay thế = shortcut SDA_CUSTOMIZA_BUTTON.SHORTCUT
                                    if (md.ExtensionInfo.ToolSet == null || md.ExtensionInfo.ToolSet.Actions == null || md.ExtensionInfo.ToolSet.Actions.Count == 0) continue;
                                    var buttonCustomizaControls = ConfigCustomizaButtonWorker.GetByModule(md.ModuleLink);
                                    if (buttonCustomizaControls == null || buttonCustomizaControls.Count == 0) continue;

                                    foreach (KeyboardAction iAct in md.ExtensionInfo.ToolSet.Actions)
                                    {
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public void SetModuleToGlobalVariables(Action<int> GeCurrentNumberPlugin = null, Action<int> TotalModule = null)
        {
            try
            {
                //Neu da chon phong thi se chi hien thi danh sach menu tuong ung voi loai phong da chon
                ListModule = ListModule.OrderBy(o => o.ParentCode).ThenBy(o => o.NumberOrder).ThenBy(o => o.PageGroupName).ToList();
                GlobalVariables.currentModuleRaws = new List<Module>();
                GlobalVariables.currentModuleRaws.AddRange(ListModule);
                List<Module> moduleTemps = new List<Module>();
                if (TotalModule != null)
                    TotalModule(ListModule.Count);
                int count = 1;
                foreach (var item in ListModule)
                {
                    moduleTemps.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(item, item.RoomId, item.RoomTypeId));
                    if (GeCurrentNumberPlugin != null)
                        GeCurrentNumberPlugin(count);
                }
                var listModuleDefault = ModuleManager.GenerateListTree(moduleTemps);
                ModuleManager.SetListTreeToSession(listModuleDefault);
                GlobalVariables.currentModules = new List<Module>();
                GlobalVariables.currentModules.AddRange(listModuleDefault);
                foreach (var item in GlobalVariables.currentModuleRaws)
                {
                    item.Children = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        ACS.EFMODEL.DataModels.V_ACS_MODULE GetByCode(string moduleCode, List<ACS.EFMODEL.DataModels.V_ACS_MODULE> listModule)
        {
            try
            {
                if (!String.IsNullOrEmpty(moduleCode))
                {
                    return listModule.SingleOrDefault(o => o.ID.ToString() == moduleCode);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return new ACS.EFMODEL.DataModels.V_ACS_MODULE();
        }

        bool IsVisible(Module module)
        {
            bool result = false;
            try
            {
                switch (module.ModuleLink)
                {
                    case ModuleCodeConstant.MODULE_LOG_OUT:
                        result = true;
                        break;
                    default:
                        if (module.PluginInfo != null && module.ExtensionInfo != null)
                        {
                            var asm = module.PluginInfo.AssemblyResolve;
                            if (asm != null)
                            {
                                Type type = asm.GetType(module.ExtensionInfo.FormalName);
                                if (type != null
                                    && !String.IsNullOrEmpty(ModuleBase.MethodNameIsEnable))
                                {
                                    object t = Activator.CreateInstance(type);
                                    System.Reflection.MethodInfo methodInfo = type.GetMethod(ModuleBase.MethodNameIsEnable);
                                    result = (bool)(methodInfo.Invoke(t, null));
                                }
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return result;
        }

        bool IsEnable(Module module)
        {
            bool result = false;
            try
            {
                switch (module.ModuleLink)
                {
                    case ModuleCodeConstant.MODULE_LOG_OUT:
                        result = true;
                        break;
                    default:
                        var asm = module.PluginInfo.AssemblyResolve;
                        if (asm != null)
                        {
                            Type type = asm.GetType(module.ExtensionInfo.FormalName);
                            if (type != null
                                && !String.IsNullOrEmpty(ModuleBase.MethodNameIsEnable))
                            {
                                object t = Activator.CreateInstance(type);
                                System.Reflection.MethodInfo methodInfo = type.GetMethod(ModuleBase.MethodNameIsEnable);
                                result = (bool)(methodInfo.Invoke(t, null));
                            }
                        }
                        break;
                }
                if (!result)
                {
                    //currentRoomTypeModules
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return result;
        }

        void CloseButtonHandlerClick(XtraTabPage page, XtraTabControl xtab)
        {
            try
            {
                if (page == null || xtab == null) throw new ArgumentNullException("XtraTabPage or XtraTabControl is null");

                UserControl container = null;
                if (page.Controls.Count > 0)
                {
                    var control = page.Controls[0];

                    if (control != null && control is UserControl)
                    {
                        container = page.Controls[0] as UserControl;
                    }
                    else if (control != null && control is XtraUserControl)
                    {
                        container = page.Controls[0] as XtraUserControl;
                    }
                }

                int i = xtab.SelectedTabPageIndex;
                xtab.TabPages.Remove(page);
                xtab.SelectedTabPageIndex = i - 1;

                RefeshUserControlWorker(container, page);

                page.Controls.Clear();
                if (container != null)
                {
                    container.Dispose();
                    container = null;
                }
                page.Dispose();
                page = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void RefeshUserControlWorker(UserControl container, XtraTabPage page)
        {
            try
            {
                if (TabControlBaseProcess.dicSaveData != null && TabControlBaseProcess.dicSaveData.Count > 0)
                {
                    var saveDt = TabControlBaseProcess.dicSaveData.FirstOrDefault(o => o.Key == page.Name);
                    if (saveDt.Key != null && saveDt.Value != null)
                    {
                        if (container != null)
                        {
                            saveDt.Value(container);
                            TabControlBaseProcess.dicSaveData.Remove(page.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ProcessPageInRoomRemoved(DevExpress.XtraBars.Ribbon.RibbonControl ribbonMain, DevExpress.XtraTab.XtraTabControl TabControlMain)
        {
            try
            {
                //dicSaveData.Add(name + "_" + (workPlace != null ? (workPlace.RoomId + "_" + workPlace.RoomTypeId) : "0"), saveData);
                var roomIdSelecteds = WorkPlace.WorkPlaceSDO.Select(o => (o.RoomId + "_" + o.RoomTypeId)).ToList();
                if (ribbonMain.Pages.Count > 0)
                {
                    for (int i = TabControlMain.TabPages.Count - 1; i >= 0; i--)
                    {
                        string mCode = TabControlMain.TabPages[i].Name;
                        if (!String.IsNullOrEmpty(mCode))
                        {
                            var arrMD = mCode.Split(arrSeperator, StringSplitOptions.RemoveEmptyEntries);
                            if (arrMD != null && arrMD.Count() >= 3)
                            {
                                if (!roomIdSelecteds.Contains(arrMD[1] + "_" + arrMD[2]))
                                {
                                    CloseButtonHandlerClick(TabControlMain.TabPages[i], TabControlMain);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Generate tu dong top menu theo loai phong
        /// </summary>
        /// <param name="ribbonMain"></param>
        internal void GenerateMenu(DevExpress.XtraBars.Ribbon.RibbonControl ribbonMain, ImageCollection imageCollection, ImageCollection imageCollectionS)
        {
            try
            {
                //Lay ve tat ca cac module duoc phan quyen
                //Lay tat ca du lieu cau hinh roomtypemodule
                //Kiem tra trong danh sach module, 
                //neu module co trong cau hinh roomtypemodule va roomtype duoc chon thi day vao cac page theo phong
                //Cac module khong co trong cau hinh roomtypemodule & phai chọn phong thi visible đi
                //Cac module con lai se day vao page khác
                //Tat ca cac module trong cac page sẽ tổ chức theo cha con: 
                //    + nếu module có cha mà cha lại không được cấu hình sử dụng thì các module đó sẽ hiển thị dạng button icon lớn
                //    + nếu module có cha và cha có trong cấu hình -> hiển thị button menu, click vào cha sổ xuống các button của module con, hiển thị icon nhỏ

                if (ribbonMain == null) throw new ArgumentNullException("ribbonMain is null");
                if (GlobalVariables.currentModuleRaws == null || GlobalVariables.currentModuleRaws.Count == 0) throw new ArgumentNullException("currentModuleRaws is null");

                ResetRibbonControlToDefault(ribbonMain);
                var currentModules = GlobalVariables.currentModuleRaws.Where(o => o.Visible && (String.IsNullOrEmpty(o.ParentCode) || o.ParentCode == "0")).ToList();
                var currentModuleAlls = (from m in currentModules select new ModuleADO(m)).ToList();


                //if (!IsInited)
                //    ProcessPageInRoomRemoved(ribbonMain);

                //if (IsInited)
                ProcessMenuByModuleNotInRoty(currentModuleAlls, ribbonMain, imageCollection, imageCollectionS);

                //Nếu có dữ liệu module theo loại phòng & đã chọn phòng làm việc
                ProcessMenuByModuleInRoty(currentModuleAlls, ribbonMain, imageCollection, imageCollectionS);

                ribbonMain.Update();
                IsInited = false;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void ResetSingleRibbonItem(BarItemLink itemLnk)
        {
            try
            {
                if (itemLnk.Item != null)
                {
                    itemLnk.Item.Tag = null;
                    if (itemLnk.Item.SuperTip != null)
                        itemLnk.Item.SuperTip = null;
                    if (itemLnk.Item.Glyph != null)
                        itemLnk.Item.Glyph = null;
                    if (itemLnk.Item.LargeGlyph != null)
                        itemLnk.Item.LargeGlyph = null;
                    try
                    {
                        itemLnk.Item.ItemClick -= ButtonMenuProcessor.MenuItemClick;
                    }
                    catch { }
                    var childItems = itemLnk.Links;
                    //if (childItems != null && childItems.Count > 0)
                    //{
                    //    for (int i = childItems.Count - 1; i >= 0; i--)
                    //    {
                    //        ResetSingleRibbonItem((BarItemLink)childItems[i]);
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        void ResetRibbonControlToDefault(DevExpress.XtraBars.Ribbon.RibbonControl ribbonMain)
        {
            try
            {
                try
                {
                    Inventec.Common.Logging.LogSystem.Info("ResetRibbonControlToDefault.1");
                    Inventec.Common.Logging.LogSystem.Info("ribbonMain.Pages.Count=" + ribbonMain.Pages.Count);
                    for (int i = 0; i < ribbonMain.Pages.Count; i++)
                    {
                        Inventec.Common.Logging.LogSystem.Info("ribbonMain.Pages[i].Groups.Count=" + ribbonMain.Pages[i].Groups.Count);
                        for (int j = 0; j < ribbonMain.Pages[i].Groups.Count; j++)
                        {
                            if (ribbonMain.Pages[i].Groups[j] != null)
                            {
                                if (ribbonMain.Pages[i].Groups[j].ItemLinks != null && ribbonMain.Pages[i].Groups[j].ItemLinks.Count > 0)
                                {
                                    var itemLinks = ribbonMain.Pages[i].Groups[j].ItemLinks.ToList<BarItemLink>();
                                    int itemlinkcount = itemLinks.Count;
                                    for (int kk = itemlinkcount - 1; kk >= 0; kk--)
                                    {
                                        ResetSingleRibbonItem(itemLinks[kk]);

                                        //itemLinks[kk].Item.Dispose();
                                        //itemLinks[kk].Dispose();
                                    }
                                    ribbonMain.Pages[i].Groups[j].ItemLinks.Clear();
                                }

                                if (ribbonMain.Pages[i].Groups[j].Glyph != null)
                                    ribbonMain.Pages[i].Groups[j].Glyph = null;
                                if (ribbonMain.Pages[i].Groups[j].SuperTip != null)
                                    ribbonMain.Pages[i].Groups[j].SuperTip = null;
                                ribbonMain.Pages[i].Groups[j].Tag = null;
                                //ribbonMain.Pages[i].Groups[j].Dispose();
                            }
                        }
                        ribbonMain.Pages[i].Groups.Clear();
                        if (ribbonMain.Pages[i].Image != null)
                            ribbonMain.Pages[i].Image = null;
                        ribbonMain.Pages[i].Tag = null;
                        //ribbonMain.Pages[i].Dispose();
                    }
                    Inventec.Common.Logging.LogSystem.Info("ResetRibbonControlToDefault.2");
                }
                catch (Exception exx)
                {
                    LogSystem.Warn(exx);
                }
                ribbonMain.Pages.Clear();//TODO
                Inventec.Common.Logging.LogSystem.Info("ResetRibbonControlToDefault.3");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        void ProcessMenuByModuleInRoty(List<ModuleADO> currentModuleAlls, DevExpress.XtraBars.Ribbon.RibbonControl ribbonMain, ImageCollection imageCollection, ImageCollection imageCollectionS)
        {
            try
            {
                var roomTypeIdSelecteds = WorkPlace.GetRoomTypeIds();

                var currentModuleInRotys = (
                    from m in currentModuleAlls
                    from n in roomTypeIdSelecteds
                    where m.RoomTypeIds != null
                        && m.RoomTypeIds.Count > 0
                        && m.RoomTypeIds.Contains(n)
                    select m).Distinct().OrderBy(o => o.text).ToList();

                //Nếu có dữ liệu module theo loại phòng & đã chọn phòng làm việc
                if (currentModuleInRotys != null
                    && currentModuleInRotys.Count > 0
                    && WorkPlace.WorkPlaceSDO != null
                    && WorkPlace.WorkPlaceSDO.Count > 0)
                {

                    var groupRoomOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ChooseRoom.GroupRoomOption");

                    //Duyệt lần lượt từng phòng làm việc
                    foreach (var wp in WorkPlace.WorkPlaceSDO)
                    {
                        List<ModuleADO> moduleInRotys = new List<ModuleADO>();
                        ModuleADO parentModulePage = new ModuleADO();


                        if (groupRoomOption == "1" && wp.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                        {
                            string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            var userRoomByUserWithRoomTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>().Where(o =>
                                o.LOGINNAME == loginName
                                && (o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG
                                && wp.DepartmentId == o.DEPARTMENT_ID).ToList();
                            if (userRoomByUserWithRoomTypes != null && userRoomByUserWithRoomTypes.Count > 1)
                            {
                                var minId = userRoomByUserWithRoomTypes.Min(k => k.ROOM_ID);

                                var roomIdMin = userRoomByUserWithRoomTypes.FirstOrDefault(f => f.ROOM_ID == minId);

                                parentModulePage.text = (roomIdMin != null ? String.Format("{0} {1}", roomIdMin.ROOM_TYPE_NAME, roomIdMin.DEPARTMENT_NAME) : wp.RoomName);
                            }
                            else
                            {
                                parentModulePage.text = wp.RoomName;
                            }
                        }
                        else
                        {
                            parentModulePage.text = wp.RoomName;
                        }
                        parentModulePage.ModuleCode = "ModuleTypeCode__InRole__" + wp.RoomCode + "__" + wp.RoomTypeId + "__";

                        //Lọc danh sách moduleADO có roomtypeid bằng roomtypeid của phòng đang duyệt
                        var moduleInRotyTemps = (
                            from m in currentModuleInRotys
                            where m.RoomTypeIds != null
                                && m.RoomTypeIds.Contains(wp.RoomTypeId)
                            select m).ToList();

                        if (moduleInRotyTemps != null && moduleInRotyTemps.Count > 0)
                        {
                            foreach (var roty in moduleInRotyTemps)
                            {
                                roty.RoomTypeId = wp.RoomTypeId;
                                roty.RoomId = wp.RoomId;
                            }

                            var groups = moduleInRotyTemps.GroupBy(o => o.ModuleGroupCode).ToList();
                            if (groups != null && groups.Count > 0)
                            {
                                List<ModuleADO> moduleInGroups = new List<ModuleADO>();
                                foreach (var gr in groups)
                                {
                                    if (!String.IsNullOrEmpty(gr.Key))
                                    {
                                        ModuleADO ModuleADOGroup = new ModuleADO();
                                        ModuleADOGroup.text = gr.First().ModuleGroupName;
                                        ModuleADOGroup.ModuleCode = "ModuleTypeCode__InRole__" + wp.RoomCode + "__" + wp.RoomTypeId + "__" + gr.Key;
                                        ModuleADOGroup.ModuleGroupCode = gr.Key;
                                        ModuleADOGroup.ChildrenADO = gr.ToList();
                                        ModuleADOGroup.NumberOrder = Inventec.Common.TypeConvert.Parse.ToInt32(gr.First().Description);
                                        moduleInGroups.Add(ModuleADOGroup);
                                    }
                                    else
                                    {
                                        moduleInGroups.AddRange(gr.ToList());
                                    }
                                }

                                moduleInGroups = moduleInGroups.OrderByDescending(o => o.NumberOrder).ThenBy(o => o.text).ToList();
                                parentModulePage.ChildrenADO = moduleInGroups;
                                moduleInRotys.Add(parentModulePage);
                                moduleInRotys = moduleInRotys.OrderBy(o => o.text).ToList();

                                this.GenMenuPage(ribbonMain, imageCollection, imageCollectionS, moduleInRotys, moduleInRotyTemps);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void ProcessMenuByModuleNotInRoty(List<ModuleADO> currentModuleAlls, DevExpress.XtraBars.Ribbon.RibbonControl ribbonMain, ImageCollection imageCollection, ImageCollection imageCollectionS)
        {
            try
            {
                var roomTypeModuleNotIns =
                    currentModuleAlls.Where(o => o.RoomTypeIds == null
                                                || o.RoomTypeIds.Count == 0).ToList();
                //if (roomTypeModuleNotIns==null || roomTypeModuleNotIns.Count==0)
                //{
                //    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModuleAlls), currentModuleAlls));
                //}
                if (roomTypeModuleNotIns != null && roomTypeModuleNotIns.Count > 0)
                {
                    List<ModuleADO> moduleInOtherTabPages = new List<ModuleADO>();
                    ModuleADO otherModulePage = new ModuleADO();
                    otherModulePage.ModuleCode = "ModuleTypeCode__NotInRole";
                    otherModulePage.text = Inventec.Common.Resource.Get.Value("frmMain.bbtnPageOther.Caption", HIS.Desktop.Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());

                    List<ModuleADO> moduleInGroups = new List<ModuleADO>();
                    var groups = roomTypeModuleNotIns.GroupBy(o => o.ModuleGroupCode).ToList();
                    foreach (var gr in groups)
                    {
                        if (!String.IsNullOrEmpty(gr.Key))
                        {
                            ModuleADO moduleADOGroup = new ModuleADO();
                            moduleADOGroup.text = gr.First().ModuleGroupName;
                            moduleADOGroup.ModuleCode = "ModuleTypeCode__NotInRole" + "__" + gr.Key;
                            moduleADOGroup.ModuleGroupCode = gr.Key;
                            moduleADOGroup.ChildrenADO = gr.ToList();
                            moduleADOGroup.NumberOrder = Inventec.Common.TypeConvert.Parse.ToInt32(gr.First().Description);
                            moduleInGroups.Add(moduleADOGroup);
                        }
                        else
                        {
                            moduleInGroups.AddRange(gr.ToList());
                        }
                    }
                    moduleInGroups = moduleInGroups.OrderByDescending(o => o.NumberOrder).ThenBy(o => o.text).ToList();
                    otherModulePage.ChildrenADO = moduleInGroups;

                    moduleInOtherTabPages.Add(otherModulePage);
                    moduleInOtherTabPages = moduleInOtherTabPages.OrderBy(o => o.text).ToList();

                    this.GenMenuPage(ribbonMain, imageCollection, imageCollectionS, moduleInOtherTabPages, roomTypeModuleNotIns);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void GenMenuPage(
            DevExpress.XtraBars.Ribbon.RibbonControl ribbonMain,
            ImageCollection imageCollection,
            ImageCollection imageCollectionS,
            List<ModuleADO> modules,
            List<ModuleADO> moduleInOtherPages
            )
        {
            try
            {
                if (modules == null || modules.Count == 0)
                    throw new ArgumentNullException("modules is null");

                foreach (var item in modules)
                {
                    var childs = item.ChildrenADO;
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("childs.count", childs != null ? childs.Count : 0));
                    //Kiểm tra module có con hay không?                        
                    //Trường hợp module có con
                    //thì mới thêm các page -> child
                    //Ngược lại bỏ qua module cha này
                    //Khởi tạo các tab page cha
                    RibbonPage parentPage = new RibbonPage(item.text);
                    parentPage.Tag = item.ModuleCode;
                    RibbonPageGroup groupCommon = new RibbonPageGroup("");
                    groupCommon.AllowMinimize = false;

                    bool isCreateCommonGroup = false;
                    RibbonPageGroup groupOther = new RibbonPageGroup("");
                    groupOther.AllowMinimize = false;

                    List<ModuleADO> moduleInPages = new List<ModuleADO>();
                    List<BarItem> bItemInRibbons = new List<BarItem>();
                    if (childs != null && childs.Count > 0)
                    {
                        moduleInPages.AddRange(childs);
                        if (childs.Count > maxTabPageItemCount)
                        {
                            moduleInPages = moduleInPages.Skip(0).Take(maxTabPageItemCount).OrderByDescending(o => o.NumberOrder).ThenBy(o => o.text).ToList();
                            isCreateCommonGroup = true;
                        }

                        foreach (var chil in moduleInPages)
                        {
                            var childLevel2s = chil.ChildrenADO;
                            if (!String.IsNullOrEmpty(chil.ModuleLink) && (childLevel2s == null || childLevel2s.Count == 0))
                            {
                                BarButtonItem buttonItem = new BarButtonItem(ribbonMain.Manager, chil.text);
                                buttonItem.Id = ribbonMain.Manager.GetNewItemId(); //Ensures correct runtime layout (de)serialization.
                                if (!String.IsNullOrEmpty(chil.Icon))
                                {
                                    SetIconMenu(buttonItem, imageCollection, chil.Icon);
                                }
                                else if (chil.ImageIndex > 0)
                                {
                                    SetIconMenu(buttonItem, imageCollection, chil.ImageIndex);
                                }
                                else
                                {
                                    SetIconMenu(buttonItem, imageCollection, -1);//Set default image
                                }
                                buttonItem.Tag = ButtonMenuProcessor.CreateModuleData(chil, chil.RoomId, chil.RoomTypeId);
                                buttonItem.Name = "barItem__" + chil.RoomId + "__" + chil.RoomTypeId;
                                buttonItem.ItemClick += ButtonMenuProcessor.MenuItemClick;
                                buttonItem.RibbonStyle = RibbonItemStyles.Large;

                                bItemInRibbons.Add(buttonItem);
                            }
                            else
                            {
                                if (childLevel2s != null && childLevel2s.Count > 0)
                                {
                                    childLevel2s = childLevel2s.OrderByDescending(o => o.NumberOrder).ThenBy(o => o.text).ToList();
                                    BarSubItem barSubItem = new BarSubItem(ribbonMain.Manager, chil.text);
                                    barSubItem.Id = ribbonMain.Manager.GetNewItemId(); //Ensures correct runtime layout (de)serialization.
                                    //barSubItem.PaintStyle = BarItemPaintStyle.Caption;

                                    SetMultiColumnMenu(barSubItem, childLevel2s.Count);

                                    string iconLink = "showproduct_32x32.png";
                                    if (!String.IsNullOrEmpty(chil.ModuleGroupCode))
                                    {
                                        var mg = DATA.ModuleGroups.FirstOrDefault(o => o.MODULE_GROUP_CODE == chil.ModuleGroupCode);

                                        if (mg != null && !String.IsNullOrEmpty(mg.ICON_LINK) && imageCollection.Images.Keys.IndexOf(mg.ICON_LINK) > -1)
                                        {
                                            iconLink = mg.ICON_LINK;
                                        }
                                    }
                                    SetIconMenu(barSubItem, imageCollection, iconLink);//FIX image for menu
                                    //if (!String.IsNullOrEmpty(chil.Icon))
                                    //{
                                    //    SetIconMenu(barSubItem, imageCollection, "showproduct_32x32.png");
                                    //}
                                    //else if (chil.ImageIndex > 0)
                                    //{
                                    //    SetIconMenu(barSubItem, imageCollection, chil.ImageIndex);
                                    //}
                                    //else
                                    //{
                                    //    SetIconMenu(barSubItem, imageCollection, -1);//Set default image
                                    //}
                                    barSubItem.Tag = ButtonMenuProcessor.CreateModuleData(chil, chil.RoomId, chil.RoomTypeId);
                                    barSubItem.RibbonStyle = RibbonItemStyles.Large;
                                    foreach (var chil3 in childLevel2s)
                                    {
                                        BarButtonItem barButtonItemChildInSubMenu = new BarButtonItem(ribbonMain.Manager, chil3.text);
                                        barButtonItemChildInSubMenu.Id = ribbonMain.Manager.GetNewItemId(); //Ensures correct runtime layout (de)serialization.
                                        barButtonItemChildInSubMenu.PaintStyle = BarItemPaintStyle.CaptionGlyph;
                                        if (!String.IsNullOrEmpty(chil3.Icon))
                                        {
                                            SetIconMenu(barButtonItemChildInSubMenu, imageCollectionS, chil3.Icon, "");
                                        }
                                        else if (chil3.ImageIndex > 0)
                                        {
                                            SetIconMenu(barButtonItemChildInSubMenu, imageCollectionS, chil3.ImageIndex, -1);
                                        }
                                        else
                                        {
                                            SetIconMenu(barButtonItemChildInSubMenu, imageCollectionS, -1);//Set default image
                                        }
                                        barButtonItemChildInSubMenu.Tag = ButtonMenuProcessor.CreateModuleData(chil3, chil3.RoomId, chil3.RoomTypeId);
                                        barButtonItemChildInSubMenu.Name = "barItem__" + chil3.RoomId + "__" + chil3.RoomTypeId;
                                        barButtonItemChildInSubMenu.ItemClick += ButtonMenuProcessor.MenuItemClick;
                                        barButtonItemChildInSubMenu.RibbonStyle = RibbonItemStyles.Default;
                                        barSubItem.ItemLinks.Add(barButtonItemChildInSubMenu);
                                    }
                                    bItemInRibbons.Add(barSubItem);
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(chil.ModuleLink))
                                    {
                                        BarButtonItem itemButtonNotParent = new BarButtonItem(ribbonMain.Manager, chil.text);
                                        itemButtonNotParent.Id = ribbonMain.Manager.GetNewItemId(); //Ensures correct runtime layout (de)serialization.
                                        //itemButtonNotParent.PaintStyle = BarItemPaintStyle.CaptionGlyph;
                                        if (!String.IsNullOrEmpty(chil.Icon))
                                        {
                                            SetIconMenu(itemButtonNotParent, imageCollection, chil.Icon);
                                        }
                                        else if (chil.ImageIndex > 0)
                                        {
                                            SetIconMenu(itemButtonNotParent, imageCollection, chil.ImageIndex);
                                        }
                                        else
                                        {
                                            SetIconMenu(itemButtonNotParent, imageCollection, -1);//Set default image
                                        }
                                        itemButtonNotParent.Tag = ButtonMenuProcessor.CreateModuleData(chil, chil.RoomId, chil.RoomTypeId);
                                        itemButtonNotParent.Name = "barItem__" + chil.RoomId + "__" + chil.RoomTypeId;
                                        itemButtonNotParent.ItemClick += ButtonMenuProcessor.MenuItemClick;
                                        itemButtonNotParent.RibbonStyle = RibbonItemStyles.Large;
                                        bItemInRibbons.Add(itemButtonNotParent);
                                    }
                                }
                            }
                        }

                        //Insert tabpage khác nếu số menu trong page vượt quá giới hạn
                        if (isCreateCommonGroup)
                        {
                            //Nếu số lượng menu trên 1 tabpage lớn hơn giới hạn (15) thì sẽ chỉ tạo ra số menu = giới hạn + 1 menu khác (mở menu này sẽ hiển thị toàn bộ các menu trong tabpage đó)
                            BarButtonItem itemButtonOther = new BarButtonItem(ribbonMain.Manager, ResourceCommon.CaptionOtherButton);
                            itemButtonOther.Name = "menuAllItem__" + moduleInPages[0].RoomId + "__" + moduleInPages[0].RoomTypeId;
                            itemButtonOther.Id = ribbonMain.Manager.GetNewItemId(); //Ensures correct runtime layout (de)serialization.                    
                            SetIconMenu(itemButtonOther, imageCollection, "contentarrangeinrows_32x32.png");//Fix icon cho nút
                            List<Module> mds = new List<Module>();
                            foreach (var md in moduleInOtherPages)
                            {
                                mds.Add(ButtonMenuProcessor.CreateModuleData(md, md.RoomId, md.RoomTypeId));
                            }
                            HIS.Desktop.ModuleExt.MenuAllADO menuADO = new HIS.Desktop.ModuleExt.MenuAllADO(mds, imageCollection);
                            itemButtonOther.Tag = menuADO;

                            itemButtonOther.ItemClick += ButtonMenuProcessor.MenuItemClick;
                            itemButtonOther.RibbonStyle = RibbonItemStyles.Large;

                            groupOther.ItemLinks.Add(itemButtonOther);
                            parentPage.Groups.Add(groupOther);
                        }

                        //Insert tabpage chứa các menu trong phòng làm việc
                        if (bItemInRibbons != null && bItemInRibbons.Count > 0)
                            groupCommon.ItemLinks.AddRange(bItemInRibbons);
                        if (groupCommon.ItemLinks.Count > 0)
                        {
                            parentPage.Groups.Add(groupCommon);
                        }

                        //Add all page to ribbon control
                        ribbonMain.Pages.Add(parentPage);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetIconMenu(BarButtonItem itemWorkingTable, ImageCollection imageCollection, string icon)
        {
            try
            {
                if (!String.IsNullOrEmpty(icon))
                {
                    itemWorkingTable.Glyph = imageCollection.Images[icon];
                    itemWorkingTable.LargeGlyph = imageCollection.Images[icon];
                }
                else
                {
                    itemWorkingTable.PaintStyle = BarItemPaintStyle.Caption;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetIconMenu(BarButtonItem itemWorkingTable, ImageCollection imageCollection, string icon, string largeIcon)
        {
            try
            {
                if (!String.IsNullOrEmpty(icon))
                {
                    itemWorkingTable.Glyph = imageCollection.Images[icon];
                }
                if (!String.IsNullOrEmpty(largeIcon))
                {
                    itemWorkingTable.LargeGlyph = imageCollection.Images[icon];
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetIconMenu(BarButtonItem itemWorkingTable, ImageCollection imageCollection, int imageIndex)
        {
            try
            {
                if (imageIndex > -1)
                {
                    itemWorkingTable.Glyph = imageCollection.Images[imageIndex];
                    itemWorkingTable.LargeGlyph = imageCollection.Images[imageIndex];
                }
                else
                {
                    itemWorkingTable.PaintStyle = BarItemPaintStyle.Caption;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetIconMenu(BarButtonItem itemWorkingTable, ImageCollection imageCollection, int imageIndex, int largeImageIndex)
        {
            try
            {
                if (imageIndex > -1)
                {
                    itemWorkingTable.Glyph = imageCollection.Images[imageIndex];
                }
                if (largeImageIndex > -1)
                {
                    itemWorkingTable.LargeGlyph = imageCollection.Images[imageIndex];
                }
                //else
                //{
                //    itemWorkingTable.PaintStyle = BarItemPaintStyle.Caption;
                //}
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetIconMenu(BarSubItem itemWorkingTable, ImageCollection imageCollection, string icon)
        {
            try
            {
                if (!String.IsNullOrEmpty(icon))
                {
                    itemWorkingTable.Glyph = imageCollection.Images[icon];
                    itemWorkingTable.LargeGlyph = imageCollection.Images[icon];
                }
                else
                {
                    itemWorkingTable.PaintStyle = BarItemPaintStyle.Caption;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetIconMenu(BarSubItem itemWorkingTable, ImageCollection imageCollection, int imageIndex)
        {
            try
            {
                if (imageIndex > -1)
                {
                    itemWorkingTable.Glyph = imageCollection.Images[imageIndex];
                    itemWorkingTable.LargeGlyph = imageCollection.Images[imageIndex];
                }
                else
                {
                    itemWorkingTable.PaintStyle = BarItemPaintStyle.Caption;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetMultiColumnMenu(BarSubItem itemWorking, int menuCount)
        {
            try
            {
                if (menuCount > 20)
                {
                    //itemWorking.MultiColumn = DefaultBoolean.True;
                    //itemWorking.OptionsMultiColumn.ShowItemText = DefaultBoolean.True;
                    //itemWorking.OptionsMultiColumn.ColumnCount = 2;
                    //if (menuCount > 20)
                    //else if (menuCount > 40)
                    //    itemWorking.OptionsMultiColumn.ColumnCount = 3;
                    //else if (menuCount > 60)
                    //    itemWorking.OptionsMultiColumn.ColumnCount = 4;
                    //else if (menuCount > 80)
                    //    itemWorking.OptionsMultiColumn.ColumnCount = 5;
                    //else if (menuCount > 100)
                    //    itemWorking.OptionsMultiColumn.ColumnCount = 6;
                    //else if (menuCount > 120)
                    //    itemWorking.OptionsMultiColumn.ColumnCount = 7;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        ModuleADO CreateModuleData(ModuleADO data, long roomId, long roomTypeId)
        {
            ModuleADO result = new ModuleADO();
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
    }
}
