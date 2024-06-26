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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAdrList.Run
{
    public partial class frmHisAdrList : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;

        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        int pageSize = 0;

        internal List<V_HIS_ADR> vHisAdrList { get; set; }

        V_HIS_ADR _RowDataPrint { get; set; }

        public frmHisAdrList()
        {
            InitializeComponent();
        }

        public frmHisAdrList(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisAdrList_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                SetCaptionByLanguageKey();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                dtFromTime.EditValue = DateTime.Now;
                dtToTime.EditValue = DateTime.Now;
                WaitingManager.Show();
                LoadDataADRList();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisAdrList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisAdrList.Run.frmHisAdrList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisAdrList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmHisAdrList.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisAdrList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisAdrList.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmHisAdrList.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisAdrList.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__Search.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.barButton__Search.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__New.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.barButton__New.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__Print.Caption = Inventec.Common.Resource.Get.Value("frmHisAdrList.barButton__Print.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataADRList()
        {
            try
            {
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                ucPagingData(new CommonParam(0, (int)pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(ucPagingData, param, (int)pageSize, gridControlADRs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ucPagingData(object param)
        {
            try
            {
                gridControlADRs.DataSource = null;
                vHisAdrList = new List<V_HIS_ADR>();

                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                MOS.Filter.HisAdrViewFilter _adrViewFilter = new MOS.Filter.HisAdrViewFilter();
                _adrViewFilter.TREATMENT_ID = this.treatmentId;
                _adrViewFilter.ORDER_FIELD = "ADR_TIME";
                _adrViewFilter.ORDER_DIRECTION = "DESC";
                if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                {
                    _adrViewFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                {
                    _adrViewFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "235959");
                }

                var result = new BackendAdapter(paramCommon).GetRO<List<V_HIS_ADR>>("api/HisAdr/GetView", ApiConsumers.MosConsumer, _adrViewFilter, paramCommon);
                if (result != null)
                {
                    vHisAdrList = (List<V_HIS_ADR>)result.Data;
                    rowCount = (vHisAdrList == null ? 0 : vHisAdrList.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }

                gridControlADRs.BeginUpdate();
                gridControlADRs.DataSource = vHisAdrList;
                gridControlADRs.EndUpdate();
                gridViewADRs.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewADRs.OptionsSelection.EnableAppearanceFocusedRow = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewADRs_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_ADR data = (V_HIS_ADR)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data == null)
                        return;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start;
                    }
                    else if (e.Column.FieldName == "ADR_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.ADR_TIME);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME.Value);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewADRs_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string creator = (gridViewADRs.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    // long DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewADRs.GetRowCellValue(e.RowHandle, "DEPARTMENT_ID") ?? "0").ToString());
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    long departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (loginName == creator || CheckLoginAdmin.IsAdmin(loginName))
                        {
                            e.RepositoryItem = repositoryItemButton__Delete_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Delete_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "EDIT")
                    {
                        if (loginName == creator || CheckLoginAdmin.IsAdmin(loginName))
                        {
                            e.RepositoryItem = repositoryItemButton__Edit;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Edit_D;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataADRList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewADRs_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                //if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                //{
                //    GridView view = sender as GridView;
                //    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                //    GridHitInfo hi = view.CalcHitInfo(e.Location);
                //    if (hi.HitTest == GridHitTest.RowCell)
                //    {
                //        long departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                //        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //        if (hi.Column.FieldName == "EDIT")
                //        {
                //            #region EDIT
                //            var row = (V_HIS_TRACKING)gridViewADRs.GetRow(hi.RowHandle);
                //            if (row != null)
                //            {
                //                if (row.DEPARTMENT_ID == departmentId || CheckLoginAdmin.IsAdmin(loginName))
                //                {
                //                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ADRCreate").FirstOrDefault();
                //                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ADRCreate");
                //                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                //                    {
                //                        HIS_TRACKING ado = new HIS_TRACKING();
                //                        Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_TRACKING, MOS.EFMODEL.DataModels.HIS_TRACKING>();
                //                        ado = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_TRACKING, MOS.EFMODEL.DataModels.HIS_TRACKING>(row);
                //                        List<object> listArgs = new List<object>();
                //                        listArgs.Add(ado); ;
                //                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                //                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                //                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                //                        ((Form)extenceInstance).ShowDialog();

                //                        //Load lại tracking
                //                        LoadDataADRList();
                //                    }
                //                }
                //            }
                //            #endregion
                //        }
                //        else if (hi.Column.FieldName == "DELETE")
                //        {
                //            #region DELETE
                //            var row = (V_HIS_TRACKING)gridViewADRs.GetRow(hi.RowHandle);
                //            if (row != null)
                //            {
                //                var creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //                if (creator.Trim() == row.CREATOR.Trim() || CheckLoginAdmin.IsAdmin(loginName))
                //                {
                //                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                //                    MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong),
                //                    MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                //                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                //                    {
                //                        bool success = false;
                //                        CommonParam param = new CommonParam();
                //                        var rs = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TRACKING_DELETE, ApiConsumers.MosConsumer, row.ID, param);
                //                        if (rs)
                //                        {
                //                            success = true;
                //                            //Load lại tracking
                //                            LoadDataADRList();
                //                        }
                //                        MessageManager.Show(this.ParentForm, param, success);
                //                    }
                //                }
                //            }
                //            #endregion
                //        }
                //        else if (hi.Column.FieldName == "SERVICE_REQ")
                //        {
                //            #region Add/Remove ServiceReq
                //            var row = (V_HIS_TRACKING)gridViewADRs.GetRow(hi.RowHandle);
                //            if (row != null)
                //            {
                //                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisServiceReqByADR").FirstOrDefault();
                //                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisServiceReqByADR");
                //                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                //                {
                //                    List<object> listArgs = new List<object>();
                //                    listArgs.Add(row.ID); ;
                //                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                //                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                //                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                //                    ((Form)extenceInstance).ShowDialog();

                //                    //Load lại tracking
                //                    LoadDataADRList();
                //                }
                //            }
                //            #endregion
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAdr").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisAdr");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.treatmentId);
                    listArgs.Add((RefeshReference)LoadDataADRList);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__Search_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__New_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_HIS_ADR)gridViewADRs.GetFocusedRow();
                if (data != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAdr").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisAdr");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data);
                        listArgs.Add((RefeshReference)LoadDataADRList);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Delete_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_HIS_ADR)gridViewADRs.GetFocusedRow();
                if (data != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                                    MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong),
                                    MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        var rs = new BackendAdapter(param).Post<bool>("api/HisAdr/Delete", ApiConsumers.MosConsumer, data.ID, param);
                        if (rs)
                        {
                            success = true;
                            LoadDataADRList();
                        }
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Print_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_HIS_ADR)gridViewADRs.GetFocusedRow();
                if (data != null)
                {
                    this._RowDataPrint = data;
                    PrintProcess();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
