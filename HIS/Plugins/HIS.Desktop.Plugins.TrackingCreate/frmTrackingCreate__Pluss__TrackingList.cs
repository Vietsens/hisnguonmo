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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TrackingCreate
{
    /// <summary>
    /// Tab "DS tờ điều trị": read only view of all treatment sheets of the treatment, newest first,
    /// provided by the module HIS.Desktop.Plugins.HisTrackingList (view only mode).
    /// </summary>
    public partial class frmTrackingCreateNew
    {
        private const string MODULE_LINK__HIS_TRACKING_LIST = "HIS.Desktop.Plugins.HisTrackingList";
        // Việc 57960: khóa bật tính năng; khác 1 thì ẩn tab DS tờ điều trị (như trước 57960)
        private const string CONFIG_KEY__IS_VIEW_TRACKING_NEWEST_FIRST = "HIS.Desktop.Plugins.HisTrackingList.IsViewTrackingNewestFirst";

        private Control trackingListView;
        private bool isTrackingListNeedReload;

        /// <summary>
        /// Shows the tab "DS tờ điều trị" only when the config key IsViewTrackingNewestFirst is 1.
        /// Called once on form load, before the user can select the tab.
        /// </summary>
        private void ApplyTrackingListTabVisibility()
        {
            try
            {
                bool isEnabled = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IS_VIEW_TRACKING_NEWEST_FIRST) == "1";
                this.xtraTabPageTrackingList.PageVisible = isEnabled;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Called when the selected tab changes: builds the view the first time the tab is opened,
        /// or again after the treatment sheet was saved. Otherwise keeps the view (scroll position and zoom).
        /// </summary>
        private void ProcessSelectTrackingListTab()
        {
            try
            {
                if (this.xtraTabControl2.SelectedTabPage != this.xtraTabPageTrackingList)
                    return;
                if (this.trackingListView == null || this.isTrackingListNeedReload)
                    InitTrackingListView();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// The treatment sheets changed (save, save and sign): reload the view now if the tab is open,
        /// otherwise the next time the tab is opened.
        /// </summary>
        private void SetTrackingListNeedReload()
        {
            try
            {
                if (this.trackingListView == null)
                    return;
                this.isTrackingListNeedReload = true;
                if (this.xtraTabControl2.SelectedTabPage == this.xtraTabPageTrackingList)
                    InitTrackingListView();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitTrackingListView()
        {
            try
            {
                this.isTrackingListNeedReload = false;
                DisposeTrackingListView();
                this.lblTrackingListMessage.Visible = false;

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws != null
                    ? GlobalVariables.currentModuleRaws.FirstOrDefault(o => o.ModuleLink == MODULE_LINK__HIS_TRACKING_LIST)
                    : null;
                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Tai khoan khong co chuc nang " + MODULE_LINK__HIS_TRACKING_LIST + ", khong hien thi tab DS to dieu tri");
                    ShowTrackingListMessage(Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lblTrackingListMessage.NoPermission", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()));
                    return;
                }

                Inventec.Desktop.Common.Modules.Module module = PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId);
                List<object> listArgs = new List<object>();
                listArgs.Add(this.treatmentId);
                listArgs.Add(true);//view only
                listArgs.Add(module);
                var extenceInstance = PluginInstance.GetPluginInstance(module, listArgs);
                UserControl view = extenceInstance as UserControl;
                if (view == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Module " + MODULE_LINK__HIS_TRACKING_LIST + " khong tra ve UserControl o che do chi xem");
                    ShowTrackingListMessage(Inventec.Common.Resource.Get.Value("frmTrackingCreateNew.lblTrackingListMessage.CannotOpen", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()));
                    if (extenceInstance is IDisposable)
                        ((IDisposable)extenceInstance).Dispose();
                    return;
                }

                view.Dock = DockStyle.Fill;
                this.xtraTabPageTrackingList.Controls.Add(view);
                view.BringToFront();
                this.trackingListView = view;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowTrackingListMessage(string message)
        {
            try
            {
                this.lblTrackingListMessage.Text = message;
                this.lblTrackingListMessage.Visible = true;
                this.lblTrackingListMessage.BringToFront();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DisposeTrackingListView()
        {
            try
            {
                if (this.trackingListView != null)
                {
                    Control view = this.trackingListView;
                    this.trackingListView = null;
                    this.xtraTabPageTrackingList.Controls.Remove(view);
                    view.Dispose();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
