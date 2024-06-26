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
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using SDA.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using SDA.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using System.Resources;
using HIS.Desktop.Plugins.ModuleButtonCustomize.Resources;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.ModuleButtonCustomize.ADO;
using ACS.EFMODEL.DataModels;
using ACS.Filter;
namespace HIS.Desktop.Plugins.ModuleButtonCustomize
{
    public partial class frmModuleButtonCustomize : HIS.Desktop.Utility.FormBase
    {
        RefeshReference refeshReference;
        List<ModuleControlADO> currentModuleControlADOs = null;
        List<ModuleControlADO> currentModuleControlShows = null;
        List<SDA_CUSTOMIZE_BUTTON> currentCustomizeButton;
        List<ACS_MODULE> lstAcsModules = null;
        List<SDA_HIDE_CONTROL> lstSdaHideControl = null;
        SDA_CUSTOMIZE_BUTTON CustomizeButton;
        BindingList<ModuleControlADO> records;
        List<CustomizeButtonShortcut> ShortCut;
        List<CustomizeADOs> DataGridControl;
        bool isFromMenu;


        public frmModuleButtonCustomize(Inventec.Desktop.Common.Modules.Module module, List<ModuleControlADO> moduleControlADOs, SDA_CUSTOMIZE_BUTTON hControl)
            : this(null, null, moduleControlADOs, hControl)
        {
        }

        public frmModuleButtonCustomize(Inventec.Desktop.Common.Modules.Module module, RefeshReference _refeshReference, List<ModuleControlADO> moduleControlADOs, SDA_CUSTOMIZE_BUTTON hControl)
            : base(module)
        {
            InitializeComponent();
            this.refeshReference = _refeshReference;
            this.currentModuleControlADOs = moduleControlADOs;
            if (moduleControlADOs == null)
            {
                isFromMenu = true;
            }
            this.CustomizeButton = hControl;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmModuleButtonCustomize_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetCaptionByLanguageKey();
                LoadComboFunction();
                loadData();
                if (this.CustomizeButton != null)
                {
                    //loadData();
                    var controlCheckeds = currentModuleControlShows != null ? currentModuleControlShows.Count() : 0;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("controlCheckeds.count", controlCheckeds));
                }

                CreteShortCut();
                LoadDataToCboShortCut();


                FillDataToTileControl();

                SetDefaultValue();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void LoadComboFunction()
        {
            try
            {

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                CommonParam param = new CommonParam();
                AcsModuleFilter acsfilter = new AcsModuleFilter();
                this.lstAcsModules = new BackendAdapter(param).Get<List<ACS_MODULE>>(SdaRequestUriStore.ACS_CUSTOMIZE_MODULE, ApiConsumers.AcsConsumer, acsfilter, param);

                CommonParam paramCommon = new CommonParam();
                SdaCustomizeButtonFilter filter = new SdaCustomizeButtonFilter();
                this.currentCustomizeButton = new BackendAdapter(paramCommon).Get<List<SDA_CUSTOMIZE_BUTTON>>(SdaRequestUriStore.SDA_CUSTOMIZE_BUTTON_GET, ApiConsumers.SdaConsumer, filter, paramCommon);
                List<string> lstNames = new List<string>();
                lstNames = currentCustomizeButton.Select(o => o.MODULE_LINK).Distinct().ToList();
                List<FunctionADO> data = new List<FunctionADO>();

                foreach (var item in lstNames)
                {
                    FunctionADO func = new FunctionADO();
                    var name = lstAcsModules.Where(o => o.MODULE_LINK == item).FirstOrDefault();
                    if (name != null)
                    {
                        func.FUNCTION = lstAcsModules.Where(o => o.MODULE_LINK == item).FirstOrDefault().MODULE_NAME;
                    }
                    func.MODULE_LINK = item;
                    data.Add(func);
                }

                columnInfos.Add(new ColumnInfo("FUNCTION", "", 250, 1));
                columnInfos.Add(new ColumnInfo("MODULE_LINK", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("FUNCTION", "MODULE_LINK", columnInfos, false, 500);
                ControlEditorLoader.Load(cboFindFunction, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void loadData()
        {
            try
            {

                var branch = BranchDataWorker.Branch;
                CommonParam paramCommon = new CommonParam();
                SdaCustomizeButtonFilter filter = new SdaCustomizeButtonFilter();
                ////filter.MODULE_LINK__EXACT = this.CustomizeButton.MODULE_LINK;
                ////filter.APP_CODE__EXACT = this.CustomizeButton.APP_CODE;
                ////filter.BRANCH_CODE__EXACT = branch.BRANCH_CODE;
                this.currentCustomizeButton = new BackendAdapter(paramCommon).Get<List<SDA_CUSTOMIZE_BUTTON>>(SdaRequestUriStore.SDA_CUSTOMIZE_BUTTON_GET, ApiConsumers.SdaConsumer, filter, paramCommon);

                CommonParam param = new CommonParam();
                AcsModuleFilter acsfilter = new AcsModuleFilter();
                this.lstAcsModules = new BackendAdapter(param).Get<List<ACS_MODULE>>(SdaRequestUriStore.ACS_CUSTOMIZE_MODULE, ApiConsumers.AcsConsumer, acsfilter, param);

                CommonParam paramHide = new CommonParam();
                SdaHideControlFilter sdaHidefilter = new SdaHideControlFilter();
                this.lstSdaHideControl = new BackendAdapter(paramHide).Get<List<SDA_HIDE_CONTROL>>(SdaRequestUriStore.SDA_HIDE_CONTROL_GET, ApiConsumers.SdaConsumer, sdaHidefilter, paramHide);
                if (isFromMenu)
                {
                    var currentModuleControlVisible = currentCustomizeButton;
                    currentModuleControlShows = new List<ModuleControlADO>();
                    foreach (var item in currentModuleControlVisible)
                    {
                        var existHideControl = lstSdaHideControl.Where(o => o.CONTROL_PATH == item.BUTTON_PATH).FirstOrDefault();
                        ModuleControlADO ad = new ModuleControlADO();
                        ad.CAPTION = item.CAPTION;
                        ad.CURRENT_SHORTCUT = item.CURRENT_SHORTCUT;
                        ad.SHORTCUT = item.SHORTCUT;
                        ad.MODULE_LINK = item.MODULE_LINK;
                        ad.ControlPath = item.BUTTON_PATH;
                        if (existHideControl != null)
                        {
                            ad.IsVisible = false;
                        }
                        else
                        {
                            ad.IsVisible = true;
                        }
                        ad.Title = item.DEFAULT_CAPTION;
                        ad.ControlName = item.BUTTON_CODE;
                        ad.FUNCTION = lstAcsModules.Where(o => o.MODULE_LINK == item.MODULE_LINK).FirstOrDefault().MODULE_NAME;
                        currentModuleControlShows.Add(ad);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModuleControlADOs), currentModuleControlADOs));
                    var currentModuleControlVisible = currentModuleControlADOs.Where(o => (o.ControlType == "DevExpress.XtraEditors.SimpleButton" || o.ControlType == "System.Windows.Forms.Button" || (o.ControlType == "DevExpress.XtraEditors.CheckEdit" || o.ControlType == "System.Windows.Forms.CheckBox" || o.ControlType == "System.Windows.Forms.Label" || o.ControlType == "DevExpress.XtraEditors.LabelControl" || (o.ControlType == "DevExpress.XtraLayout.LayoutControlItem" && o.IsTextVisible))) && (!String.IsNullOrEmpty(o.Title) || !String.IsNullOrEmpty((o.Value ?? "").ToString()) || !String.IsNullOrEmpty(o.TitleLayout))).ToList();
                    currentModuleControlShows = new List<ModuleControlADO>();
                    if (currentModuleControlVisible != null && currentModuleControlVisible.Count > 0)
                    {
                        foreach (var item in currentModuleControlVisible)
                        {
                            var dataDB = currentCustomizeButton != null ? currentCustomizeButton.Where(o => o.BUTTON_PATH == item.ControlPath).FirstOrDefault() : null;

                            ModuleControlADO ad = new ModuleControlADO();
                            ad.ControlKey = item.ControlKey;
                            ad.ControlName = item.ControlName;
                            ad.ControlPath = item.ControlPath;
                            ad.ControlType = item.ControlType;
                            ad.IsChecked = item.IsChecked;
                            ad.IsVisible = item.IsVisible;
                            ad.mControl = item.mControl;
                            ad.ParentControlKey = item.ParentControlKey;
                            ad.ParentControlName = item.ParentControlName;
                            //ad.Title = item.Title;
                            ad.Value = item.Value;

                            if (dataDB != null)
                            {
                                ad.CURRENT_SHORTCUT = dataDB.CURRENT_SHORTCUT;
                                ad.SHORTCUT = dataDB.SHORTCUT;
                                ad.CAPTION = dataDB.CAPTION;
                                ad.Title = !String.IsNullOrEmpty(dataDB.DEFAULT_CAPTION) ? dataDB.DEFAULT_CAPTION : (!String.IsNullOrEmpty(item.Title) ? item.Title : (!String.IsNullOrEmpty((item.Value ?? "").ToString()) ? (item.Value ?? "").ToString() : (item.TitleLayout)));
                                ad.MODULE_LINK = dataDB.MODULE_LINK;
                                ad.FUNCTION = lstAcsModules.Where(o => o.MODULE_LINK == dataDB.MODULE_LINK).FirstOrDefault().MODULE_NAME;
                            }
                            else
                            {
                                ad.Title = (!String.IsNullOrEmpty(item.Title) ? item.Title : (!String.IsNullOrEmpty((item.Value ?? "").ToString()) ? (item.Value ?? "").ToString() : (item.TitleLayout)));
                                //ad.Title = item.Title + "(" + item.ControlName + "," + item.Value + ")";
                            }
                            currentModuleControlShows.Add(ad);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDataToTileControl()
        {
            try
            {
                if (isFromMenu)
                {
                    string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());
                    string func = "";
                    var query = this.currentModuleControlShows.AsQueryable();
                    query = query.Where(o => (o.ControlName ?? "").ToLower().Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.ControlPath ?? "").ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.ControlType ?? "").ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.Title ?? "").ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.TitleLayout ?? "").ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.Value ?? "").ToString().ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.SHORTCUT ?? "").ToString().ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.CURRENT_SHORTCUT ?? "").ToString().ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.CAPTION ?? "").ToString().ToLower()).Contains(keyword)
                            || (o.ControlType ?? "").ToLower().Contains(keyword));
                    if (cboFindFunction.EditValue != null && cboFindFunction.EditValue.ToString() != "")
                    {
                        func = cboFindFunction.EditValue.ToString();
                        query = query.Where(o => o.MODULE_LINK == func);
                    }
                    if (query == null || query.Count() == 0)
                    {
                        this.myGridControl1.BeginUpdate();
                        this.myGridControl1.DataSource = null;
                        this.myGridControl1.EndUpdate();
                        this.btnSave.Enabled = false;
                        return;
                    }

                    records = new BindingList<ModuleControlADO>(query.ToList());
                    DataGridControl = new List<CustomizeADOs>();
                    foreach (var item in records)
                    {
                        CustomizeADOs itemdata = new CustomizeADOs();
                        Inventec.Common.Mapper.DataObjectMapper.Map<CustomizeADOs>(itemdata, item);
                        DataGridControl.Add(itemdata);
                    }
                    this.myGridControl1.DataSource = DataGridControl;

                    this.btnSave.Enabled = (DataGridControl != null && DataGridControl.Count() > 0);
                }
                else
                {
                    string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());
                    var query = this.currentModuleControlShows.AsQueryable();
                    query = query.Where(o => (o.ControlName ?? "").ToLower().Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.ControlPath ?? "").ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.ControlType ?? "").ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.Title ?? "").ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.TitleLayout ?? "").ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.Value ?? "").ToString().ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.SHORTCUT ?? "").ToString().ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.CURRENT_SHORTCUT ?? "").ToString().ToLower()).Contains(keyword)
                            || Inventec.Common.String.Convert.UnSignVNese((o.CAPTION ?? "").ToString().ToLower()).Contains(keyword)
                            || (o.ControlType ?? "").ToLower().Contains(keyword));

                    if (query == null || query.Count() == 0)
                    {
                        this.myGridControl1.BeginUpdate();
                        this.myGridControl1.DataSource = null;
                        this.myGridControl1.EndUpdate();
                        this.btnSave.Enabled = false;
                        return;
                    }

                    records = new BindingList<ModuleControlADO>(query.ToList());
                    DataGridControl = new List<CustomizeADOs>();
                    foreach (var item in records)
                    {
                        CustomizeADOs itemdata = new CustomizeADOs();
                        Inventec.Common.Mapper.DataObjectMapper.Map<CustomizeADOs>(itemdata, item);
                        DataGridControl.Add(itemdata);
                    }
                    this.myGridControl1.DataSource = DataGridControl;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => DataGridControl), DataGridControl));
                    this.btnSave.Enabled = (DataGridControl != null && DataGridControl.Count() > 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ModuleButtonCustomize.Resources.Lang", typeof(HIS.Desktop.Plugins.ModuleButtonCustomize.frmModuleButtonCustomize).Assembly);

                if (this.currentModuleBase != null && !String.IsNullOrEmpty(this.currentModuleBase.text))
                {
                    this.Text = this.currentModuleBase.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridView2.Focus();
                    gridView2.FocusedRowHandle = 0;
                    var rowData = (CustomizeADOs)gridView2.GetFocusedRow();
                    if (rowData != null)
                    {
                        //ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.FillDataToTileControl();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                myGridControl1.BeginUpdate();
                btnSave.Focus();
                //myGridControl1.PostEditor();
                myGridControl1.EndUpdate();
                string errr = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                var check = DataGridControl.Where(o => o.ErrorMessageCaption == "Độ dài ký tự không vượt quá 200" || o.ErrorMessageCurrentShortCut == errr || o.ErrorMessageShortCut == errr).ToList();

                if (check != null && check.Count > 0)
                    return;
                CommonParam param = new CommonParam();
                List<string> roomIdCheckeds = new List<string>();
                var branch = BranchDataWorker.Branch;

                //string error = "";
                if (currentCustomizeButton != null && currentCustomizeButton.Count > 0)
                {
                    foreach (var item in currentCustomizeButton)
                    {
                        foreach (var item1 in DataGridControl)
                        {
                            if (item.BUTTON_PATH == item1.ControlPath)
                            {
                                item.CAPTION = item1.CAPTION;
                                item.CURRENT_SHORTCUT = item1.CURRENT_SHORTCUT;
                                item.SHORTCUT = item1.SHORTCUT;
                            }
                        }
                    }
                }
                if (this.currentModuleControlShows != null && this.currentModuleControlShows.Count > 0)
                {
                    foreach (var item in currentModuleControlShows)
                    {
                        foreach (var item1 in DataGridControl)
                        {
                            if (item.ControlPath == item1.ControlPath)
                            {
                                item.CAPTION = item1.CAPTION;
                                item.CURRENT_SHORTCUT = item1.CURRENT_SHORTCUT;
                                item.SHORTCUT = item1.SHORTCUT;
                            }
                        }
                    }
                }
                if (this.currentModuleControlShows != null)
                {
                    if (isFromMenu)
                    {
                        this.ChangeLockButtonWhileProcess(false);
                        WaitingManager.Show();
                        List<string> buttonExists = currentModuleControlShows != null ? currentModuleControlShows.Select(o => o.ControlPath).ToList() : null;

                        List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON> moduleControlUpdFromMenu = currentCustomizeButton != null ? currentCustomizeButton.Where(o => buttonExists.Contains(o.BUTTON_PATH)).ToList() : null;


                        bool result = false;
                        if (moduleControlUpdFromMenu != null && moduleControlUpdFromMenu.Count > 0)
                        {
                            var listUpdate = DataGridControl.Where(o => moduleControlUpdFromMenu.Select(p => p.BUTTON_PATH).Contains(o.ControlPath)).ToList();
                            var rsUpd = new BackendAdapter(param).Post<List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON>>("api/SdaCustomizeButton/UpdateList", ApiConsumers.SdaConsumer, moduleControlUpdFromMenu, param);
                            if (rsUpd == null || rsUpd.Count == 0)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Goi api api/SdaCustomizeButton/UpdateList that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleControlUpdFromMenu), moduleControlUpdFromMenu) + "____Ket qua tra ve" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsUpd), rsUpd));
                            }
                            else
                                result = true;
                            if (result)
                            {
                                HIS.Desktop.LocalStorage.ConfigCustomizaButton.ConfigCustomizaButtonWorker.Init();
                            }
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, result);
                    }
                    else if (this.Check(param, ref roomIdCheckeds))
                    {
                        this.ChangeLockButtonWhileProcess(false);
                        WaitingManager.Show();

                        List<string> buttonInDBExists = currentCustomizeButton != null ? currentCustomizeButton.Select(o => o.BUTTON_PATH).ToList() : null;
                        List<string> buttonRawExists = currentModuleControlShows != null ? currentModuleControlShows.Select(o => o.ControlPath).ToList() : null;
                        List<ModuleControlADO> moduleControlCreates = this.currentModuleControlShows.Where(o => !buttonInDBExists.Contains(o.ControlPath)).ToList();

                        List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON> moduleControlDels = currentCustomizeButton != null ? currentCustomizeButton.Where(o => buttonRawExists.Contains(o.BUTTON_PATH)).ToList() : null;

                        List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON> moduleControlUpds = currentCustomizeButton != null ? currentCustomizeButton.Where(o => buttonRawExists.Contains(o.BUTTON_PATH)).ToList() : null;

                        if ((moduleControlCreates != null && moduleControlCreates.Count > 0) || (moduleControlDels != null && moduleControlDels.Count > 0) || (moduleControlUpds != null && moduleControlUpds.Count > 0))
                        {
                            //foreach (var item in moduleControlCreates)
                            //{
                            //    if (!String.IsNullOrEmpty(item.CURRENT_SHORTCUT) && String.IsNullOrEmpty(item.SHORTCUT)) error += "Chưa nhập phím tắt mới " + "\r\n";
                            //    if (String.IsNullOrEmpty(item.CURRENT_SHORTCUT) && !String.IsNullOrEmpty(item.SHORTCUT)) error += "Chưa nhập phím tắt hiện tại " + "\r\n";
                            //}
                            //if (!String.IsNullOrEmpty(error))
                            //{
                            //    WaitingManager.Hide();
                            //    MessageBox.Show(error);
                            //    return;
                            //}

                            bool success = false;
                            if (moduleControlCreates != null && moduleControlCreates.Count > 0)
                            {
                                List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON> CustomizeButtonCreate = new List<SDA_CUSTOMIZE_BUTTON>();
                                foreach (var item in moduleControlCreates)
                                {
                                    if (item.ControlType == "DevExpress.XtraEditors.SimpleButton"
                                        || item.ControlType == "DevExpress.XtraEditors.DropDownButton"
                                        || item.ControlType == "DevExpress.XtraTab.XtraTabPage"
                                        || item.ControlType == "System.Windows.Forms.TabPage"
                                        || item.ControlType == "DevExpress.XtraEditors.CheckEdit"
                                        || item.ControlType == "System.Windows.Forms.CheckBox"
                                        || item.ControlType == "DevExpress.XtraLayout.LayoutControlItem"
                                        || item.ControlType == "System.Windows.Forms.Label"
                                        || item.ControlType == "DevExpress.XtraEditors.LabelControl"
                                        )
                                    {

                                        SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON customizeButtonAdd = new SDA_CUSTOMIZE_BUTTON();
                                        customizeButtonAdd.APP_CODE = this.CustomizeButton.APP_CODE;
                                        customizeButtonAdd.MODULE_LINK = this.CustomizeButton.MODULE_LINK;
                                        customizeButtonAdd.BUTTON_CODE = item.ControlName;
                                        customizeButtonAdd.BUTTON_PATH = item.ControlPath;
                                        customizeButtonAdd.BRANCH_CODE = branch.BRANCH_CODE;
                                        customizeButtonAdd.CAPTION = item.CAPTION;
                                        customizeButtonAdd.DEFAULT_CAPTION = item.Title;
                                        customizeButtonAdd.SHORTCUT = item.SHORTCUT;
                                        customizeButtonAdd.CURRENT_SHORTCUT = item.CURRENT_SHORTCUT;
                                        CustomizeButtonCreate.Add(customizeButtonAdd);
                                    }
                                }
                                var rsCreate = new BackendAdapter(param).Post<List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON>>("api/SdaCustomizeButton/CreateList", ApiConsumers.SdaConsumer, CustomizeButtonCreate, param);
                                if (rsCreate == null)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Goi api api/SdaCustomizeButton/CreateList that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => CustomizeButton), CustomizeButton) + "____Ket qua tra ve" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsCreate), rsCreate));
                                }
                                else
                                    success = true;
                            }

                            if (moduleControlUpds != null && moduleControlUpds.Count > 0)
                            {
                                var listUpdate = DataGridControl.Where(o => moduleControlUpds.Select(p => p.BUTTON_PATH).Contains(o.ControlPath)).ToList();
                                //foreach (var item in listUpdate)
                                //{
                                //    if (!String.IsNullOrEmpty(item.CURRENT_SHORTCUT) && String.IsNullOrEmpty(item.SHORTCUT)) error += "Chưa nhập phím tắt mới" + "\r\n";
                                //    if (String.IsNullOrEmpty(item.CURRENT_SHORTCUT) && !String.IsNullOrEmpty(item.SHORTCUT)) error += "Chưa nhập phím tắt hiện tại" + "\r\n";
                                //}
                                //if (!String.IsNullOrEmpty(error))
                                //{
                                //    WaitingManager.Hide();
                                //    MessageBox.Show(error);
                                //    return;
                                //}
                                var rsUpd = new BackendAdapter(param).Post<List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON>>("api/SdaCustomizeButton/UpdateList", ApiConsumers.SdaConsumer, moduleControlUpds, param);
                                if (rsUpd == null || rsUpd.Count == 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Goi api api/SdaCustomizeButton/UpdateList that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleControlUpds), moduleControlUpds) + "____Ket qua tra ve" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsUpd), rsUpd));
                                }
                                else
                                    success = true;
                            }

                            if (success)
                            {
                                HIS.Desktop.LocalStorage.ConfigCustomizaButton.ConfigCustomizaButtonWorker.Init();
                            }
                            WaitingManager.Hide();
                            MessageManager.Show(this, param, success);
                        }

                    }
                    else
                    {
                        HIS.Desktop.LocalStorage.ConfigCustomizaButton.ConfigCustomizaButtonWorker.Init();
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Debug("ModuleButtonCustomize. Click ham luu khong thay du lieu thay doi hoac them moi => reload lai du lieu trong truong hop nguoi dung khac thay doi du lieu");
                    }

                    this.ChangeLockButtonWhileProcess(true);
                }
                loadData();
            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        private bool Check(CommonParam param, ref List<string> roomIdCheckeds)
        {
            bool valid = true;
            try
            {
                myGridControl1.RefreshDataSource();
                roomIdCheckeds = this.currentModuleControlShows.Select(o => o.ControlName).ToList();
                if (roomIdCheckeds == null || roomIdCheckeds.Count == 0)
                {
                    param.Messages.Add(ResourceMessage.KhongChonControlCanAnHien);
                    valid = false;
                }

                if (!valid)
                {
                    MessageManager.Show(param, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return valid;
        }

        private void ChangeLockButtonWhileProcess(bool isLock)
        {
            try
            {
                this.btnSave.Enabled = isLock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmModuleButtonCustomize_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void bbtnCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnSave.Enabled)
                    this.btnSave_Click(null, null);

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        //Key ShortCut
        private void CreteShortCut()
        {
            try
            {

                this.ShortCut = new List<CustomizeButtonShortcut>();
                CustomizeButtonShortcut v4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut a = new CustomizeButtonShortcut();
                CustomizeButtonShortcut b = new CustomizeButtonShortcut();
                CustomizeButtonShortcut c = new CustomizeButtonShortcut();
                CustomizeButtonShortcut d = new CustomizeButtonShortcut();
                CustomizeButtonShortcut e = new CustomizeButtonShortcut();
                CustomizeButtonShortcut f = new CustomizeButtonShortcut();
                CustomizeButtonShortcut g = new CustomizeButtonShortcut();
                CustomizeButtonShortcut h = new CustomizeButtonShortcut();
                CustomizeButtonShortcut i = new CustomizeButtonShortcut();
                CustomizeButtonShortcut k = new CustomizeButtonShortcut();
                CustomizeButtonShortcut n = new CustomizeButtonShortcut();
                CustomizeButtonShortcut m = new CustomizeButtonShortcut();
                CustomizeButtonShortcut u = new CustomizeButtonShortcut();
                CustomizeButtonShortcut y = new CustomizeButtonShortcut();
                CustomizeButtonShortcut q = new CustomizeButtonShortcut();
                CustomizeButtonShortcut l = new CustomizeButtonShortcut();
                CustomizeButtonShortcut a1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut a2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut a3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut a4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut a5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut a6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut a7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut a8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut a9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut b1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut b2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut b3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut b4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut b5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut b6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut b7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut b8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut b9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut c1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut c2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut c3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut c4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut c5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut c6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut c7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut c8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut c9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut d3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut d4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut d5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut d6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut d7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut d8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut d9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut e1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut e2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut e3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut e4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut e5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut e6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut e7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut e8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut e9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut f1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut f2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut f3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut f4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut f5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut f6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut f7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut f8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut f9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut g1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut g2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut g3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut g4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut g5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut g6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut g7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut g8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut g9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut h1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut h2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut h3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut h4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut h5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut h6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut h7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut h8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut h9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut i1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut i2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut i3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut i4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut i5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut i6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut i7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut i8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut i9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut k1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut k2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut k3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut k4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut k5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut k6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut k7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut k8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut k9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut m1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut m2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut m3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut m4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut m5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut m6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut m7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut m8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut m9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut n1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut n2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut n3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut n4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut n5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut n6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut n7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut n8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut n9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut q1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut q2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut q3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut q4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut q5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut q6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut q7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut q8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut q9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut z1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut z2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut z3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut z4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut z5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut z6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut z7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut z8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut z9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut t1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut t2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut t3 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut t4 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut t5 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut t6 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut t7 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut t8 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut t9 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut v1 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut v2 = new CustomizeButtonShortcut();
                CustomizeButtonShortcut v3 = new CustomizeButtonShortcut();


                v4.Value = 0;
                v4.SHORTCUT = "";
                a.Value = 45;
                a.SHORTCUT = "Insert";
                b.Value = 46;
                b.SHORTCUT = "Delete";
                c.Value = 112;
                c.SHORTCUT = "F1";
                d.Value = 113;
                d.SHORTCUT = "F2";
                e.Value = 114;
                e.SHORTCUT = "F3";
                f.Value = 115;
                f.SHORTCUT = "F4";
                g.Value = 116;
                g.SHORTCUT = "F5";
                h.Value = 117;
                h.SHORTCUT = "F6";
                i.Value = 118;
                i.SHORTCUT = "F7";
                k.Value = 119;
                k.SHORTCUT = "F8";
                n.Value = 120;
                n.SHORTCUT = "F9";
                m.Value = 121;
                m.SHORTCUT = "F10";
                u.Value = 122;
                u.SHORTCUT = "F11";
                y.Value = 123;
                y.SHORTCUT = "F12";
                q.Value = 65581;
                q.SHORTCUT = "Shift+Insert";
                l.Value = 65582;
                l.SHORTCUT = "Shift+Delete";
                a1.Value = 65648;
                a1.SHORTCUT = "Shift+F1";
                a2.Value = 65649;
                a2.SHORTCUT = "Shift+F2";
                a3.Value = 65650;
                a3.SHORTCUT = "Shift+F3";
                a4.Value = 65651;
                a4.SHORTCUT = "Shift+F4";
                a5.Value = 65652;
                a5.SHORTCUT = "Shift+F5";
                a6.Value = 65653;
                a6.SHORTCUT = "Shift+F6";
                a7.Value = 65654;
                a7.SHORTCUT = "Shift+F7";
                a8.Value = 65655;
                a8.SHORTCUT = "Shift+F8";
                a9.Value = 65656;
                a9.SHORTCUT = "Shift+F9";
                b1.Value = 65657;
                b1.SHORTCUT = "Shift+F10";
                b2.Value = 65658;
                b2.SHORTCUT = "Shift+F11";
                b3.Value = 65659;
                b3.SHORTCUT = "Shift+F12";
                b4.Value = 131117;
                b4.SHORTCUT = "Ctrl+Insert";
                b5.Value = 131118;
                b5.SHORTCUT = "Ctrl+Delete";
                b6.Value = 131120;
                b6.SHORTCUT = "Ctrl+0";
                b7.Value = 131121;
                b7.SHORTCUT = "Ctrl+1";
                b8.Value = 131122;
                b8.SHORTCUT = "Ctrl+2";
                b9.Value = 131123;
                b9.SHORTCUT = "Ctrl+3";
                c1.Value = 131124;
                c1.SHORTCUT = "Ctrl+4";
                c2.Value = 131125;
                c2.SHORTCUT = "Ctrl+5";
                c3.Value = 131126;
                c3.SHORTCUT = "Ctrl+6";
                c4.Value = 131127;
                c4.SHORTCUT = "Ctrl+7";
                c5.Value = 131128;
                c5.SHORTCUT = "Ctrl+8";
                c6.Value = 131129;
                c6.SHORTCUT = "Ctrl+9";
                c7.Value = 131137;
                c7.SHORTCUT = "Ctrl+A";
                c8.Value = 131138;
                c8.SHORTCUT = "Ctrl+B";
                c9.Value = 131139;
                c9.SHORTCUT = "Ctrl+C";
                d3.Value = 131140;
                d3.SHORTCUT = "Ctrl+D";
                d4.Value = 131141;
                d4.SHORTCUT = "Ctrl+E";
                d5.Value = 131142;
                d5.SHORTCUT = "Ctrl+F";
                d6.Value = 131143;
                d6.SHORTCUT = "Ctrl+G";
                d7.Value = 131144;
                d7.SHORTCUT = "Ctrl+H";
                d8.Value = 131145;
                d8.SHORTCUT = "Ctrl+I";
                d9.Value = 131146;
                d9.SHORTCUT = "Ctrl+J";
                e1.Value = 131147;
                e1.SHORTCUT = "Ctrl+K";
                e2.Value = 131148;
                e2.SHORTCUT = "Ctrl+L";
                e3.Value = 131149;
                e3.SHORTCUT = "Ctrl+M";
                e4.Value = 131150;
                e4.SHORTCUT = "Ctrl+N";
                e5.Value = 131151;
                e5.SHORTCUT = "Ctrl+O";
                e6.Value = 131152;
                e6.SHORTCUT = "Ctrl+P";
                e7.Value = 131153;
                e7.SHORTCUT = "Ctrl+Q";
                e8.Value = 131154;
                e8.SHORTCUT = "Ctrl+R";
                e9.Value = 131155;
                e9.SHORTCUT = "Ctrl+S";
                f1.Value = 131156;
                f1.SHORTCUT = "Ctrl+T";
                f2.Value = 131157;
                f2.SHORTCUT = "Ctrl+U";
                f3.Value = 131158;
                f3.SHORTCUT = "Ctrl+V";
                f4.Value = 131159;
                f4.SHORTCUT = "Ctrl+W";
                f5.Value = 131160;
                f5.SHORTCUT = "Ctrl+X";
                f6.Value = 131161;
                f6.SHORTCUT = "Ctrl+Y";
                f7.Value = 131162;
                f7.SHORTCUT = "Ctrl+Z";
                f8.Value = 131184;
                f8.SHORTCUT = "Ctrl+F1";
                f9.Value = 131185;
                f9.SHORTCUT = "Ctrl+F2";
                g1.Value = 131186;
                g1.SHORTCUT = "Ctrl+F3";
                g2.Value = 131187;
                g2.SHORTCUT = "Ctrl+F4";
                g3.Value = 131188;
                g3.SHORTCUT = "Ctrl+F5";
                g4.Value = 131189;
                g4.SHORTCUT = "Ctrl+F6";
                g5.Value = 131190;
                g5.SHORTCUT = "Ctrl+F7";
                g6.Value = 131191;
                g6.SHORTCUT = "Ctrl+F8";
                g7.Value = 131192;
                g7.SHORTCUT = "Ctrl+F9";
                g8.Value = 131193;
                g8.SHORTCUT = "Ctrl+F10";
                g9.Value = 131194;
                g9.SHORTCUT = "Ctrl+F11";
                h1.Value = 131195;
                h1.SHORTCUT = "Ctrl+F12";
                h2.Value = 196656;
                h2.SHORTCUT = "Ctrl+Shift+0";
                h3.Value = 196657;
                h3.SHORTCUT = "Ctrl+Shift+1";
                h4.Value = 196658;
                h4.SHORTCUT = "Ctrl+Shift+2";
                h5.Value = 196659;
                h5.SHORTCUT = "Ctrl+Shift+3";
                h6.Value = 196660;
                h6.SHORTCUT = "Ctrl+Shift+4";
                h7.Value = 196661;
                h7.SHORTCUT = "Ctrl+Shift+5";
                h8.Value = 196662;
                h8.SHORTCUT = "Ctrl+Shift+6";
                h9.Value = 196663;
                h9.SHORTCUT = "Ctrl+Shift+7";
                i1.Value = 196664;
                i1.SHORTCUT = "Ctrl+Shift+8";
                i2.Value = 196665;
                i2.SHORTCUT = "Ctrl+Shift+9";
                i3.Value = 196674;
                i3.SHORTCUT = "Ctrl+Shift+A";
                i4.Value = 196675;
                i4.SHORTCUT = "Ctrl+Shift+B";
                i5.Value = 196676;
                i5.SHORTCUT = "Ctrl+Shift+C";
                i6.Value = 196677;
                i6.SHORTCUT = "Ctrl+Shift+D";
                i7.Value = 196678;
                i7.SHORTCUT = "Ctrl+Shift+E";
                i8.Value = 196679;
                i8.SHORTCUT = "Ctrl+Shift+F";
                i9.Value = 196680;
                i9.SHORTCUT = "Ctrl+Shift+G";
                k1.Value = 196681;
                k1.SHORTCUT = "Ctrl+Shift+H";
                k2.Value = 196682;
                k2.SHORTCUT = "Ctrl+Shift+I";
                k3.Value = 196683;
                k3.SHORTCUT = "Ctrl+Shift+K";
                k4.Value = 196684;
                k4.SHORTCUT = "Ctrl+Shift+L";
                k5.Value = 196685;
                k5.SHORTCUT = "Ctrl+Shift+M";
                k6.Value = 196686;
                k6.SHORTCUT = "Ctrl+Shift+N";
                k7.Value = 196687;
                k7.SHORTCUT = "Ctrl+Shift+O";
                k8.Value = 196688;
                k8.SHORTCUT = "Ctrl+Shift+P";
                k9.Value = 196689;
                k9.SHORTCUT = "Ctrl+Shift+Q";
                m1.Value = 196690;
                m1.SHORTCUT = "Ctrl+Shift+R";
                m2.Value = 196691;
                m2.SHORTCUT = "Ctrl+Shift+S";
                m3.Value = 196692;
                m3.SHORTCUT = "Ctrl+Shift+T";
                m4.Value = 196693;
                m4.SHORTCUT = "Ctrl+Shift+U";
                m5.Value = 196694;
                m5.SHORTCUT = "Ctrl+Shift+V";
                m6.Value = 196695;
                m6.SHORTCUT = "Ctrl+Shift+W";
                m7.Value = 196696;
                m7.SHORTCUT = "Ctrl+Shift+X";
                m8.Value = 196697;
                m8.SHORTCUT = "Ctrl+Shift+Y";
                m9.Value = 196698;
                m9.SHORTCUT = "Ctrl+Shift+Z";
                n1.Value = 196720;
                n1.SHORTCUT = "Ctrl+Shift+F1";
                n2.Value = 196721;
                n2.SHORTCUT = "Ctrl+Shift+F2";
                n3.Value = 196722;
                n3.SHORTCUT = "Ctrl+Shift+F3";
                n4.Value = 196723;
                n4.SHORTCUT = "Ctrl+Shift+F4";
                n5.Value = 196724;
                n5.SHORTCUT = "Ctrl+Shift+F5";
                n6.Value = 196725;
                n6.SHORTCUT = "Ctrl+Shift+F6";
                n7.Value = 196726;
                n7.SHORTCUT = "Ctrl+Shift+F7";
                n8.Value = 196727;
                n8.SHORTCUT = "Ctrl+Shift+F8";
                n9.Value = 196728;
                n9.SHORTCUT = "Ctrl+Shift+F9";
                q1.Value = 196729;
                q1.SHORTCUT = "Ctrl+Shift+F10";
                q2.Value = 196730;
                q2.SHORTCUT = "Ctrl+Shift+F11";
                q3.Value = 196731;
                q3.SHORTCUT = "Ctrl+Shift+F12";
                q4.Value = 262152;
                q4.SHORTCUT = "Alt+BackSpace";
                q5.Value = 262181;
                q5.SHORTCUT = "Alt+LeftArrow";
                q6.Value = 262182;
                q6.SHORTCUT = "Alt+UpArrow";
                q7.Value = 262183;
                q7.SHORTCUT = "Alt+RightArrow";
                q8.Value = 262184;
                q8.SHORTCUT = "Alt+DownArrow";
                q9.Value = 262192;
                q9.SHORTCUT = "Alt+0";
                z1.Value = 262193;
                z1.SHORTCUT = "Alt+1";
                z2.Value = 262194;
                z2.SHORTCUT = "Alt+2";
                z3.Value = 262195;
                z3.SHORTCUT = "Alt+3";
                z4.Value = 262196;
                z4.SHORTCUT = "Alt+4";
                z5.Value = 262197;
                z5.SHORTCUT = "Alt+5";
                z6.Value = 262198;
                z6.SHORTCUT = "Alt+6";
                z7.Value = 262199;
                z7.SHORTCUT = "Alt+7";
                z8.Value = 2621200;
                z8.SHORTCUT = "Alt+8";
                z9.Value = 262201;
                z9.SHORTCUT = "Alt+9";
                t1.Value = 262256;
                t1.SHORTCUT = "Alt+F1";
                t2.Value = 262257;
                t2.SHORTCUT = "Alt+F2";
                t3.Value = 262258;
                t3.SHORTCUT = "Alt+F3";
                t4.Value = 262259;
                t4.SHORTCUT = "Alt+F4";
                t5.Value = 262260;
                t5.SHORTCUT = "Alt+F5";
                t6.Value = 262261;
                t6.SHORTCUT = "Alt+F6";
                t7.Value = 262262;
                t7.SHORTCUT = "Alt+F7";
                t8.Value = 262263;
                t8.SHORTCUT = "Alt+F8";
                t9.Value = 262264;
                t9.SHORTCUT = "Alt+F9";
                v1.Value = 262265;
                v1.SHORTCUT = "Alt+F10";
                v2.Value = 262266;
                v2.SHORTCUT = "Alt+F11";
                v3.Value = 262267;
                v3.SHORTCUT = "Alt+F12";

                this.ShortCut.Add(v4);
                this.ShortCut.Add(a);
                this.ShortCut.Add(b);
                this.ShortCut.Add(c);
                this.ShortCut.Add(d);
                this.ShortCut.Add(e);
                this.ShortCut.Add(f);
                this.ShortCut.Add(g);
                this.ShortCut.Add(h);
                this.ShortCut.Add(i);
                this.ShortCut.Add(k);
                this.ShortCut.Add(n);
                this.ShortCut.Add(m);
                this.ShortCut.Add(u);
                this.ShortCut.Add(y);
                this.ShortCut.Add(q);
                this.ShortCut.Add(l);
                this.ShortCut.Add(a1);
                this.ShortCut.Add(a2);
                this.ShortCut.Add(a3);
                this.ShortCut.Add(a4);
                this.ShortCut.Add(a5);
                this.ShortCut.Add(a6);
                this.ShortCut.Add(a7);
                this.ShortCut.Add(a8);
                this.ShortCut.Add(a9);
                this.ShortCut.Add(b1);
                this.ShortCut.Add(b2);
                this.ShortCut.Add(b3);
                this.ShortCut.Add(b4);
                this.ShortCut.Add(b5);
                this.ShortCut.Add(b6);
                this.ShortCut.Add(b7);
                this.ShortCut.Add(b8);
                this.ShortCut.Add(b9);
                this.ShortCut.Add(c1);
                this.ShortCut.Add(c2);
                this.ShortCut.Add(c3);
                this.ShortCut.Add(c4);
                this.ShortCut.Add(c5);
                this.ShortCut.Add(c6);
                this.ShortCut.Add(c7);
                this.ShortCut.Add(c8);
                this.ShortCut.Add(c9);
                this.ShortCut.Add(d3);
                this.ShortCut.Add(d4);
                this.ShortCut.Add(d5);
                this.ShortCut.Add(d6);
                this.ShortCut.Add(d7);
                this.ShortCut.Add(d8);
                this.ShortCut.Add(d9);
                this.ShortCut.Add(e1);
                this.ShortCut.Add(e2);
                this.ShortCut.Add(e3);
                this.ShortCut.Add(e4);
                this.ShortCut.Add(e5);
                this.ShortCut.Add(e6);
                this.ShortCut.Add(e7);
                this.ShortCut.Add(e8);
                this.ShortCut.Add(e9);
                this.ShortCut.Add(f1);
                this.ShortCut.Add(f2);
                this.ShortCut.Add(f3);
                this.ShortCut.Add(f4);
                this.ShortCut.Add(f5);
                this.ShortCut.Add(f6);
                this.ShortCut.Add(f7);
                this.ShortCut.Add(f8);
                this.ShortCut.Add(f9);
                this.ShortCut.Add(g1);
                this.ShortCut.Add(g2);
                this.ShortCut.Add(g3);
                this.ShortCut.Add(g4);
                this.ShortCut.Add(g5);
                this.ShortCut.Add(g6);
                this.ShortCut.Add(g7);
                this.ShortCut.Add(g8);
                this.ShortCut.Add(g9);
                this.ShortCut.Add(h1);
                this.ShortCut.Add(h2);
                this.ShortCut.Add(h3);
                this.ShortCut.Add(h4);
                this.ShortCut.Add(h5);
                this.ShortCut.Add(h6);
                this.ShortCut.Add(h7);
                this.ShortCut.Add(h8);
                this.ShortCut.Add(h9);
                this.ShortCut.Add(i1);
                this.ShortCut.Add(i2);
                this.ShortCut.Add(i3);
                this.ShortCut.Add(i4);
                this.ShortCut.Add(i5);
                this.ShortCut.Add(i6);
                this.ShortCut.Add(i7);
                this.ShortCut.Add(i8);
                this.ShortCut.Add(i9);
                this.ShortCut.Add(k1);
                this.ShortCut.Add(k2);
                this.ShortCut.Add(k3);
                this.ShortCut.Add(k4);
                this.ShortCut.Add(k5);
                this.ShortCut.Add(k6);
                this.ShortCut.Add(k7);
                this.ShortCut.Add(k8);
                this.ShortCut.Add(k9);
                this.ShortCut.Add(m1);
                this.ShortCut.Add(m2);
                this.ShortCut.Add(m3);
                this.ShortCut.Add(m4);
                this.ShortCut.Add(m5);
                this.ShortCut.Add(m6);
                this.ShortCut.Add(m7);
                this.ShortCut.Add(m8);
                this.ShortCut.Add(m9);
                this.ShortCut.Add(n1);
                this.ShortCut.Add(n2);
                this.ShortCut.Add(n3);
                this.ShortCut.Add(n4);
                this.ShortCut.Add(n5);
                this.ShortCut.Add(n6);
                this.ShortCut.Add(n7);
                this.ShortCut.Add(n8);
                this.ShortCut.Add(n9);
                this.ShortCut.Add(q1);
                this.ShortCut.Add(q2);
                this.ShortCut.Add(q3);
                this.ShortCut.Add(q4);
                this.ShortCut.Add(q5);
                this.ShortCut.Add(q6);
                this.ShortCut.Add(q7);
                this.ShortCut.Add(q8);
                this.ShortCut.Add(q9);
                this.ShortCut.Add(z1);
                this.ShortCut.Add(z2);
                this.ShortCut.Add(z3);
                this.ShortCut.Add(z4);
                this.ShortCut.Add(z5);
                this.ShortCut.Add(z6);
                this.ShortCut.Add(z7);
                this.ShortCut.Add(z8);
                this.ShortCut.Add(z9);
                this.ShortCut.Add(t1);
                this.ShortCut.Add(t2);
                this.ShortCut.Add(t3);
                this.ShortCut.Add(t4);
                this.ShortCut.Add(t5);
                this.ShortCut.Add(t6);
                this.ShortCut.Add(t7);
                this.ShortCut.Add(t8);
                this.ShortCut.Add(t9);
                this.ShortCut.Add(v1);
                this.ShortCut.Add(v2);
                this.ShortCut.Add(v3);



            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboShortCut()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SHORTCUT", "SHORTCUT", 50, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SHORTCUT", "SHORTCUT", columnInfos, false, 50);
                ControlEditorLoader.Load(repositoryItemLookUpEdit2, ShortCut, controlEditorADO);
                ControlEditorLoader.Load(repositoryItemLookUpEdit1, ShortCut, controlEditorADO);

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        //validation
        //private void treeListControl_ValidateNode(object sender, DevExpress.XtraTreeList.ValidateNodeEventArgs e)
        //{
        //    //var CAPTION = e.Node[treeListColumn9];
        //    //var SHORTCUT = e.Node[treeListColumn3];
        //    //int? a = 0;
        //    //if (CAPTION != null && CAPTION.ToString().Length > 0)
        //    //{
        //    //    a = Inventec.Common.String.CountVi.Count(CAPTION.ToString());
        //    //    Inventec.Common.Logging.LogSystem.Info("1 nnn" + CAPTION.ToString());
        //    //}
        //    //var CURRENT_SHORTCUT = e.Node[treeListColumn8];
        //    //if (SHORTCUT == null && CURRENT_SHORTCUT != null)
        //    //{
        //    //    e.Valid = false;
        //    //    //Set errors with specific descriptions for the columns 
        //    //    treeListControl.SetColumnError(treeListColumn3, "Trường dữ liệu bắt buộc");
        //    //    Inventec.Common.Logging.LogSystem.Info("2 nnn" + CURRENT_SHORTCUT.ToString());
        //    //}
        //    //else if (SHORTCUT != null && CURRENT_SHORTCUT == null)
        //    //{
        //    //    e.Valid = false;
        //    //    treeListControl.SetColumnError(treeListColumn8, "Trường dữ liệu bắt buộc");
        //    //    Inventec.Common.Logging.LogSystem.Info("3 nnn" + CURRENT_SHORTCUT.ToString());
        //    //}
        //    //else if (a > 200)
        //    //{
        //    //    e.Valid = false;
        //    //    treeListControl.SetColumnError(treeListColumn9, "Độ dài không vượt quá 200");
        //    //    Inventec.Common.Logging.LogSystem.Info("4 nnn" + CURRENT_SHORTCUT.ToString());
        //    //}
        //}

        //private void treeListControl_InvalidNodeException(object sender, DevExpress.XtraTreeList.InvalidNodeExceptionEventArgs e)
        //{
        //    //e.ExceptionMode = ExceptionMode.NoAction;

        //}

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToTileControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridView2.Focus();
                    gridView2.FocusedRowHandle = 0;
                    var rowData = (CustomizeADOs)gridView2.GetFocusedRow();
                    if (rowData != null)
                    {
                        //ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            ModuleControlADO moduleControlADO = new ModuleControlADO();
            var data = this.gridView2.GetRow(e.RowHandle);
            if (data != null && data is CustomizeADOs)
            {
                moduleControlADO = (CustomizeADOs)data;
            }
            if (e.Column.FieldName == "IsChecked")
            {
                if (String.IsNullOrEmpty(((CustomizeADOs)data).ParentControlKey))
                {
                    e.RepositoryItem = null;
                }
            }
        }

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (CustomizeADOs)gridView2.GetFocusedRow();
                    if (rowData != null)
                    {
                        //ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var data = this.gridView2.GetRow(e.RowHandle);
                if (data != null && data is CustomizeADOs)
                {
                    var noteData = (CustomizeADOs)data;
                    if (noteData.IsVisible)
                    {
                        e.Appearance.ForeColor = Color.Black;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Gray;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridView2_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ModuleControlADO data = view.GetFocusedRow() as CustomizeADOs;
                if (view.FocusedColumn.FieldName == "CURRENT_SHORTCUT" && view.ActiveEditor is LookUpEdit)
                {
                    LookUpEdit editor = view.ActiveEditor as LookUpEdit;
                    if (data != null)
                    {
                        // this.LoadDataToCboShortCut();
                        editor.EditValue = data.CURRENT_SHORTCUT;
                    }
                }
                else if (view.FocusedColumn.FieldName == "SHORTCUT" && view.ActiveEditor is LookUpEdit)
                {
                    LookUpEdit editor = view.ActiveEditor as LookUpEdit;
                    if (data != null)
                    {
                        //this.LoadDataToCboShortCut();
                        editor.EditValue = data.SHORTCUT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "CURRENT_SHORTCUT" || e.ColumnName == "SHORTCUT")
                {
                    this.gridView2_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridView2.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = null;
                    return;
                }
                var listDatas = this.myGridControl1.DataSource as List<CustomizeADOs>;
                var row = listDatas[index];
                if (e.ColumnName == "CAPTION")
                {
                    if (row.ErrorCaption == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorCaption);
                        e.Info.ErrorText = (string)(row.ErrorMessageCaption);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = null;
                    }
                }
                else if (e.ColumnName == "CURRENT_SHORTCUT")
                {
                    if (row.ErrorCurrent_shortCut == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorCurrent_shortCut);
                        e.Info.ErrorText = (string)(row.ErrorMessageCurrentShortCut);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = null;
                    }
                }
                else if (e.ColumnName == "SHORTCUT")
                {
                    if (row.ErrorShort_Cut == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorShort_Cut);
                        e.Info.ErrorText = (string)(row.ErrorMessageShortCut);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = null;
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var DataRow = (CustomizeADOs)this.gridView2.GetFocusedRow();
                this.ValidServiceDetailProcessing(DataRow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidServiceDetailProcessing(CustomizeADOs data)
        {
            try
            {
                if (data.SHORTCUT == null && data.CURRENT_SHORTCUT != null)
                {

                    data.ErrorMessageShortCut = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                    data.ErrorShort_Cut = ErrorType.Warning;
                }
                else if (data.SHORTCUT != null && data.CURRENT_SHORTCUT == null)
                {
                    data.ErrorMessageCurrentShortCut = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                    data.ErrorCurrent_shortCut = ErrorType.Warning;
                }
                else
                {
                    data.ErrorMessageCurrentShortCut = "";
                    data.ErrorCurrent_shortCut = ErrorType.None;
                    data.ErrorMessageShortCut = "";
                    data.ErrorShort_Cut = ErrorType.None;
                }
                if (Inventec.Common.String.CountVi.Count(data.CAPTION) > 200)
                {
                    data.ErrorMessageCaption = "Độ dài ký tự không vượt quá 200";
                    data.ErrorCaption = ErrorType.Warning;
                }
                else
                {
                    data.ErrorMessageCaption = "";
                    data.ErrorCaption = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboFindFunction_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboFindFunction.Properties.Buttons[1].Visible = false;
                    cboFindFunction.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
