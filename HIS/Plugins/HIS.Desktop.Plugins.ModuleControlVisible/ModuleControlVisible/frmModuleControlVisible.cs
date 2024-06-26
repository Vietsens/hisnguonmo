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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ModuleControlVisible.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using SDA.EFMODEL.DataModels;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ModuleControlVisible.ChooseRoom
{
    public partial class frmModuleControlVisible : HIS.Desktop.Utility.FormBase
    {
        DelegateSelectData refeshReference;
        List<ModuleControlADO> currentModuleControlADOs = null;
        List<SDA_HIDE_CONTROL> currentHideControls;
        SDA_HIDE_CONTROL hideControl;
        BindingList<ModuleControlADO> records;
        ModuleControlADO selectModuleControl;
        MOS.EFMODEL.DataModels.HIS_BRANCH branch;
        public frmModuleControlVisible(Inventec.Desktop.Common.Modules.Module module, List<ModuleControlADO> moduleControlADOs, SDA_HIDE_CONTROL hControl)
            : this(null, null, moduleControlADOs, hControl)
        {
        }

        public frmModuleControlVisible(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData _refeshReference, List<ModuleControlADO> moduleControlADOs, SDA_HIDE_CONTROL hControl)
            : base(module)
        {
            InitializeComponent();
            this.refeshReference = _refeshReference;
            this.currentModuleControlADOs = moduleControlADOs;
            this.hideControl = hControl;
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

        private void frmChooseRoom_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetCaptionByLanguageKey();
                InitDataForm();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        void InitDataForm()
        {
            try
            {
                var branchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>();
                this.branch = (branchs != null && branchs.Count > 0) ? branchs.FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId()) : null;
                if (this.hideControl != null)
                {
                    CommonParam paramCommon = new CommonParam();
                    SdaHideControlFilter filter = new SdaHideControlFilter();
                    filter.MODULE_LINK__EXACT = this.hideControl.MODULE_LINK;
                    filter.APP_CODE__EXACT = this.hideControl.APP_CODE;
                    this.currentHideControls = new BackendAdapter(paramCommon).Get<List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL>>(SdaRequestUriStore.SDA_HIDE_CONTROL_GET, ApiConsumers.SdaConsumer, filter, paramCommon);

                    this.currentHideControls = (this.currentHideControls != null && this.currentHideControls.Count > 0) ? this.currentHideControls.Where(o => o.BRANCH_CODE == null || (branch != null && o.BRANCH_CODE == branch.BRANCH_CODE)).ToList() : null;

                    if (this.currentHideControls != null && this.currentHideControls.Count > 0)
                    {
                        var ctNames = this.currentHideControls.Select(o => o.CONTROL_PATH).ToList();
                        currentModuleControlADOs.ForEach(o => o.IsChecked = !ctNames.Contains(o.ControlPath));
                    }
                    else
                    {
                        currentModuleControlADOs.ForEach(o => o.IsChecked = true);
                    }

                    var controlCheckeds = currentModuleControlADOs.Where(o => o.IsChecked).Count();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("All controls.count", currentModuleControlADOs != null && currentModuleControlADOs.Count > 0 ? currentModuleControlADOs.Count : 0) + Inventec.Common.Logging.LogUtil.TraceData("controlCheckeds.count", controlCheckeds));
                }

                FillDataToTileControl();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void frmChooseRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ModuleControlVisible.Resources.Lang", typeof(HIS.Desktop.Plugins.ModuleControlVisible.ChooseRoom.frmModuleControlVisible).Assembly);

                //////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                //this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseRoom.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnChoice.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.btnChoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboBranch.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChooseRoom.cboBranch.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmChooseRoom.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumnCheck.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnCheck.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumnRoomCode.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumnRoomName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumnRoomTypeName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnRoomTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gridColumnDepartmentName.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.gridColumnDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciBranch.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.lciBranch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bar2.Text = Inventec.Common.Resource.Get.Value("frmChooseRoom.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bbtnCtrlS.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoom.bbtnCtrlS.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.currentModuleBase != null)
                {
                    this.Text = this.currentModuleBase.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToTileControl()
        {
            try
            {
                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());
                var query = this.currentModuleControlADOs.AsQueryable();
                //query = query.Where(o => o.IsVisible);
                query = query.Where(o => o.ControlName.ToLower().Contains(keyword)
                        || Inventec.Common.String.Convert.UnSignVNese(o.ControlType.ToLower()).Contains(keyword)
                        || Inventec.Common.String.Convert.UnSignVNese(o.Title.ToLower()).Contains(keyword)
                        || Inventec.Common.String.Convert.UnSignVNese((o.Value ?? "").ToString().ToLower()).Contains(keyword)
                        || o.ControlType.ToLower().Contains(keyword));

                if (query == null || query.Count() == 0)
                {
                    this.treeListControl.BeginUpdate();
                    this.treeListControl.DataSource = null;
                    this.treeListControl.EndUpdate();
                    this.btnChoice.Enabled = false;
                    return;
                }

                var dataDistints = from p in query
                                   group p by p.ControlKey into groups
                                   select groups.First();

                var datas = dataDistints
                    .OrderBy(o => o.ControlType)
                    .ThenBy(o => o.IsChecked)
                    .ThenBy(o => o.ControlName).Distinct().ToList();

                if (dataDistints == null || dataDistints.Count() == 0)
                {
                    this.treeListControl.BeginUpdate();
                    this.treeListControl.DataSource = null;
                    this.treeListControl.EndUpdate();
                    this.btnChoice.Enabled = false;
                    return;
                }

                records = new BindingList<ModuleControlADO>(dataDistints.ToList());
                this.treeListControl.DataSource = records;
                this.treeListControl.KeyFieldName = "ControlKey";
                this.treeListControl.ParentFieldName = "ParentControlKey";
                this.treeListControl.ExpandAll();
                this.btnChoice.Enabled = (dataDistints != null && dataDistints.Count() > 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridCheckChange(bool checkedAll)
        {
            try
            {
                foreach (var item in this.currentModuleControlADOs)
                {
                    item.IsChecked = checkedAll;
                }
                this.FillDataToTileControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool Check(CommonParam param, ref List<string> roomIdCheckeds)
        {
            bool valid = true;
            try
            {
                treeListControl.RefreshDataSource();
                roomIdCheckeds = this.currentModuleControlADOs.Where(o => o.IsVisible).Select(o => o.ControlName).ToList();
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
                this.btnChoice.Enabled = isLock;
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
                //if (e.KeyCode == Keys.Enter)
                //{
                //    this.treeListControl. = 0;
                //    this.gridViewRooms.Focus();
                //}
                //else if (e.KeyCode == Keys.Down)
                //{
                //    this.gridViewRooms.FocusedRowHandle = 0;
                //    this.gridViewRooms.Focus();
                //}
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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

        private void treeListControl_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = this.treeListControl.GetDataRecordByNode(e.Node);
                if (data != null && data is ModuleControlADO)
                {
                    var noteData = (ModuleControlADO)data;
                    if (noteData.IsChecked)
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

        private void treeListControl_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName == "ControlTypeDisplay")
                {
                    ModuleControlADO currentRow = e.Row as ModuleControlADO;
                    if (currentRow == null) return;
                    int indexStart = currentRow.ControlType.LastIndexOf(".");
                    indexStart = indexStart == -1 ? 0 : indexStart + 1;
                    e.Value = currentRow.ControlType.Substring(indexStart);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListControl_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var node = treeListControl.FocusedNode;
                    var data = treeListControl.GetDataRecordByNode(node) as ModuleControlADO;
                    if (data != null)
                    {
                        data.IsChecked = !data.IsChecked;
                    }
                    else
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    var node = treeListControl.FocusedNode;
                    var data = treeListControl.GetDataRecordByNode(node) as ModuleControlADO;
                    if (data != null)
                    {
                        data.IsChecked = !data.IsChecked;
                    }
                }
                treeListControl.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListControl_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                //var data = trvService.GetDataRecordByNode(e.Node);
                //if (data != null && data is MedicineTypeADO)
                //{
                //    var rowData = data as MedicineTypeADO;
                //    if (rowData != null && this.MedicineType_CustomDrawNodeCell != null)
                //    {
                //        this.MedicineType_CustomDrawNodeCell(rowData, e);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListControl_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            ModuleControlADO moduleControlADO = new ModuleControlADO();
            var data = this.treeListControl.GetDataRecordByNode(e.Node);
            if (data != null && data is ModuleControlADO)
            {
                moduleControlADO = (ModuleControlADO)data;
            }
            if (e.Column.FieldName == "IsChecked")
            {
                if (String.IsNullOrEmpty(((ModuleControlADO)data).ParentControlKey))
                {
                    e.RepositoryItem = null;
                }
            }
        }

        private void treeListControl_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(treeListControl.FocusedNode[-1].ToString())) //Your condition  
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChoice_Click(object sender, EventArgs e)
        {
            try
            {
                treeListControl.PostEditor();
                btnChoice.Focus();
                CommonParam param = new CommonParam();
                List<string> roomIdCheckeds = new List<string>();
                var branch = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId());
                if (this.currentModuleControlADOs != null)
                {
                    if (this.Check(param, ref roomIdCheckeds))
                    {
                        this.ChangeLockButtonWhileProcess(false);
                        WaitingManager.Show();

                        List<string> controlNameExists = (currentHideControls != null && currentHideControls.Count > 0) ? currentHideControls.Select(o => o.CONTROL_PATH).ToList() : null;
                        List<string> controlNameAllVisibles = this.currentModuleControlADOs.Where(o => o.IsChecked).Select(o => o.ControlPath).ToList();
                        List<ModuleControlADO> moduleControlCreates = this.currentModuleControlADOs.Where(o => !o.IsChecked && (controlNameExists == null || (controlNameExists != null && !controlNameExists.Contains(o.ControlPath)))).ToList();
                        List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL> moduleControlDels = (currentHideControls != null && currentHideControls.Count > 0) ? currentHideControls.Where(o => controlNameAllVisibles != null && controlNameAllVisibles.Contains(o.CONTROL_PATH)).ToList() : null;

                        bool success = false;
                        if ((moduleControlCreates != null && moduleControlCreates.Count > 0) || (moduleControlDels != null && moduleControlDels.Count > 0))
                        {
                            if (moduleControlCreates != null && moduleControlCreates.Count > 0)
                            {
                                List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL> hideControlCreates = new List<SDA_HIDE_CONTROL>();
                                foreach (var item in moduleControlCreates)
                                {
                                    SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL hideControlCreate = new SDA_HIDE_CONTROL();
                                    hideControlCreate.APP_CODE = this.hideControl.APP_CODE;
                                    hideControlCreate.MODULE_LINK = this.hideControl.MODULE_LINK;
                                    hideControlCreate.CONTROL_CODE = item.ControlName;
                                    hideControlCreate.CONTROL_PATH = item.ControlPath;
                                    hideControlCreate.BRANCH_CODE = branch != null ? branch.BRANCH_CODE : "";
                                    hideControlCreates.Add(hideControlCreate);
                                }
                                var rsCreate = new BackendAdapter(param).Post<List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL>>("api/SdaHideControl/CreateList", ApiConsumers.SdaConsumer, hideControlCreates, param);
                                if (rsCreate == null)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Goi api api/SdaHideControl/CreateList that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hideControlCreates), hideControlCreates) + "____Ket qua tra ve" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsCreate), rsCreate));
                                }
                                else
                                    success = true;
                            }

                            if (moduleControlDels != null && moduleControlDels.Count > 0)
                            {
                                var idDels = moduleControlDels.Select(o => o.ID).ToList();
                                var rsDel = new BackendAdapter(param).Post<bool>("api/SdaHideControl/DeleteList", ApiConsumers.SdaConsumer, idDels, param);
                                if (!rsDel)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Goi api api/SdaHideControl/DeleteList that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleControlDels), moduleControlDels) + "____Ket qua tra ve" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsDel), rsDel));
                                }
                                else
                                    success = true;
                            }

                            WaitingManager.Hide();
                            MessageManager.Show(this, param, success);
                        }
                        else
                        {
                            WaitingManager.Hide();
                            MessageManager.Show("Không có dữ liệu thay đổi");
                            Inventec.Common.Logging.LogSystem.Warn("ModuleControlVisible. Click ham luu khong thay du lieu thay doi hoac them moi => reload lai du lieu trong truong hop nguoi dung khac thay doi du lieu");
                        }
                        if (success)
                        {
                            HIS.Desktop.LocalStorage.ConfigHideControl.ConfigHideControlWorker.Init();
                            this.Close();
                        }
                        this.ChangeLockButtonWhileProcess(true);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                GridCheckChange(true);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnUnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                GridCheckChange(false);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #region Shortcut
        private void bbtnCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.btnChoice_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        private void treeListControl_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.treeListControl.FocusedNode != null)
                {
                    ResetSelectModuleControl();
                    this.selectModuleControl = this.treeListControl.GetDataRecordByNode(this.treeListControl.FocusedNode) as ModuleControlADO;

                    SelectModuleControl(this.selectModuleControl);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void frmModuleControlVisible_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ResetSelectModuleControl();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void ResetSelectModuleControl()
        {
            try
            {
                if (this.selectModuleControl == null) return;

                if (this.selectModuleControl.mControl != null)
                {
                    ResetBackColorStateForOneControl(this.selectModuleControl.mControl);
                }
                else if (this.selectModuleControl.lControl != null)
                {
                    if (this.selectModuleControl.lControl is DevExpress.XtraLayout.LayoutControlItem)
                    {
                        ResetBackColorStateForOneControl(((DevExpress.XtraLayout.LayoutControlItem)this.selectModuleControl.lControl).Control);
                    }
                    else if (this.selectModuleControl.lControl is DevExpress.XtraLayout.LayoutControlGroup)
                    {
                        foreach (DevExpress.XtraLayout.LayoutControlItem item in ((DevExpress.XtraLayout.LayoutControlGroup)this.selectModuleControl.lControl).Items)
                        {
                            ResetBackColorStateForOneControl(item.Control);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void ResetBackColorStateForOneControl(Control control)
        {
            try
            {
                if (control == null)
                    return;

                if (control is TreeList)
                {
                    //((TreeList)control).Appearance.OddRow.BackColor = Color.YellowGreen;
                    ((TreeList)control).OptionsView.EnableAppearanceOddRow = false;
                }
                else if (control is GridControl)
                {
                    GridView gridView = (GridView)((GridControl)control).MainView;
                    if (gridView.Columns.Count > 0)
                    {
                        for (int i = 0; i < gridView.Columns.Count; i++)
                        {
                            var col = gridView.Columns[i];
                            col.AppearanceCell.Options.UseBackColor = false;
                        }
                    }
                }
                else if (control is SimpleButton)
                {
                    ((SimpleButton)control).Appearance.Options.UseBackColor = false;
                }
                else if (control is TextEdit || control is MemoEdit)
                {
                    ((TextEdit)control).BackColor = Color.White;
                }
                else if (control is LabelControl)
                {
                    ((LabelControl)control).BackColor = Color.Transparent;
                }
                else
                    control.BackColor = Color.White;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SetBackColorStateForOneControl(Control control)
        {
            try
            {
                if (control == null)
                    return;

                if (control is TreeList)
                {
                    ((TreeList)control).Appearance.OddRow.BackColor = Color.YellowGreen;
                    ((TreeList)control).OptionsView.EnableAppearanceOddRow = true;
                }
                else if (control is GridControl)
                {
                    GridView gridView = (GridView)((GridControl)control).MainView;
                    if (gridView.Columns.Count > 0)
                    {
                        for (int i = 0; i < gridView.Columns.Count; i++)
                        {
                            var col = gridView.Columns[i];
                            col.AppearanceCell.BackColor = Color.YellowGreen;
                            col.AppearanceCell.Options.UseBackColor = true;
                        }
                    }
                }
                else if (control is SimpleButton)
                {
                    ((SimpleButton)control).Appearance.BackColor = Color.YellowGreen;
                }
                else
                    control.BackColor = Color.YellowGreen;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void SelectModuleControl(ModuleControlADO moduleControl)
        {
            try
            {
                if (moduleControl == null)
                    return;

                if (moduleControl.mControl != null)
                {
                    SetBackColorStateForOneControl(moduleControl.mControl);
                }
                else if (moduleControl.lControl != null)
                {
                    if (moduleControl.lControl is DevExpress.XtraLayout.LayoutControlItem)
                    {
                        SetBackColorStateForOneControl(((DevExpress.XtraLayout.LayoutControlItem)moduleControl.lControl).Control);
                    }
                    else if (moduleControl.lControl is DevExpress.XtraLayout.LayoutControlGroup)
                    {
                        foreach (DevExpress.XtraLayout.LayoutControlItem item in ((DevExpress.XtraLayout.LayoutControlGroup)moduleControl.lControl).Items)
                        {
                            SetBackColorStateForOneControl(item.Control);
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleControl.ControlPath), moduleControl.ControlPath));
                Inventec.Common.Logging.LogSystem.Debug((moduleControl.mControl != null ? Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleControl.mControl.Name), moduleControl.mControl.Name) : "moduleControl.mControl is null"));
                Inventec.Common.Logging.LogSystem.Debug((moduleControl.lControl != null ? Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleControl.ControlName), moduleControl.ControlName) : "moduleControl.lControl is null"));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
