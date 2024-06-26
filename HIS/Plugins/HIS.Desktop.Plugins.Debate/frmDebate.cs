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
using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Debate.Config;
using HIS.Desktop.Plugins.Debate.Processors;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.Base;
using Inventec.Common.SignLibrary;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using iTextSharp.text.pdf;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageUtil = HIS.Desktop.LibraryMessage.MessageUtil;

namespace HIS.Desktop.Plugins.Debate
{
    public partial class frmDebate : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module Module;
        internal long treatmentId;
        internal MOS.EFMODEL.DataModels.HIS_DEBATE currentHisDebate { get; set; }
        private List<V_HIS_DEBATE> hisDebates { get; set; }
        private V_HIS_TREATMENT treatment { get; set; }

        private List<V_HIS_TREATMENT> lstTreatment;
        internal V_HIS_TREATMENT treatmentToPrint = new V_HIS_TREATMENT();
        private string roomName;
        private string departmentName;
        private PrintDebateMenuProcessor PrintDebateMenuProcessor;
        private bool IsTreatmentList;
        private V_HIS_ROOM CurrentRoom;

        PopupMenuProcessor popupMenuProcessor = null;
        MOS.SDO.WorkPlaceSDO WorkPlaceSDO;
        HIS_BRANCH HisBranch;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO__Create;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO__Update;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        List<HIS_DEPARTMENT> executeDepartmentSelecteds { get; set; }

        public bool isGroupMps020;
        List<V_HIS_DEBATE> lstDebateToPrint = new List<V_HIS_DEBATE>();
        V_HIS_DEBATE debateToPrint = new V_HIS_DEBATE();
        private string currentTypeCode020 = "";
        private string currentFileName020 = "";
        List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> GroupStreamPrint;
        Dictionary<long, string> DicoutPdfFile;
        string outPdfFile;
        public frmDebate()
        {
            InitializeComponent();
        }

        public frmDebate(long treatmentId)
        {
            InitializeComponent();
            this.treatmentId = treatmentId;
        }

        public frmDebate(Inventec.Desktop.Common.Modules.Module moduleData, long treatmentId)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.Module = moduleData;
                this.treatmentId = treatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmDebate(Inventec.Desktop.Common.Modules.Module moduleData, long treatmentId, bool IsTreatmentList)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.Module = moduleData;
                this.treatmentId = treatmentId;
                this.IsTreatmentList = IsTreatmentList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDebate_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"])); ///icon chuan                                                                                                                 ///
                InitControlState();
                SetDeFaultConTrol();
                lciChkAutoSign.Visibility = HisConfigCFG.IsUseSignEmr ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                LoadCurrentBranch();
                GetRoom();

                InitCheck(cboExecuteDepartment, SelectionGrid__ExecuteDepartment);
                InitCombo(cboExecuteDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList(), "DEPARTMENT_NAME", "ID");


                LoadTreatmentById(treatmentId);


                LoadGridDebate();

                SetCaptionByLanguageKey();

                if ((treatment != null && treatment.IS_PAUSE == 1) || IsTreatmentList)
                {
                    btnnew.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, List<HIS_DEPARTMENT> data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                var currentDepartment = CurrentRoom != null ? data.Where(o => o.ID == CurrentRoom.DEPARTMENT_ID).ToList() : null;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && currentDepartment != null)
                {
                    gridCheckMark.SelectAll(currentDepartment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__ExecuteDepartment(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";

                executeDepartmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        executeDepartmentSelecteds.Add(rv);
                        typeName += rv.DEPARTMENT_NAME + ", ";
                    }

                }
                cboExecuteDepartment.Text = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentBranch()
        {
            try
            {
                this.WorkPlaceSDO = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId);
                this.HisBranch = BackendDataWorker.Get<HIS_BRANCH>().Where(o => o.ID == WorkPlaceSDO.BranchId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK_DEBATE);
                this.currentControlStateRDO__Create = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK_DEBATE);
                this.currentControlStateRDO__Update = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK_DEBATE_DIAGNOSTIC);
                if (this.currentControlStateRDO__Create != null && this.currentControlStateRDO__Create.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO__Create)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD)
                        {
                            chkAutoSign.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == "debateStatus")
                        {
                            if (item.VALUE == "1")
                            {
                                chkCreator.Checked = true;
                            }
                            else if (item.VALUE == "2")
                            {
                                chkUserInvite.Checked = true;
                            }
                        }
                    }
                }
                if (currentControlStateRDO__Update != null && currentControlStateRDO__Update.Count > 0 && !chkAutoSign.Checked)
                {
                    foreach (var item in this.currentControlStateRDO__Update)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD)
                        {
                            chkAutoSign.Checked = item.VALUE == "1";
                        }
                    }
                }
                if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "debateStatus")
                        {
                            Inventec.Common.Logging.LogSystem.Debug(item.VALUE + "___checkSTT__Lan 2");

                            if (item.VALUE == "1")
                            {
                                chkCreator.Checked = true;
                            }
                            else if (item.VALUE == "2")
                            {
                                chkUserInvite.Checked = true;
                            }
                            else if (item.VALUE == "3")
                            {
                                chkAll.Checked = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetRoom()
        {
            try
            {
                CurrentRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Module.RoomId);
                if (CurrentRoom != null)
                {
                    roomName = CurrentRoom.ROOM_NAME;
                    departmentName = CurrentRoom.DEPARTMENT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDebateReq_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {

                    MOS.EFMODEL.DataModels.V_HIS_DEBATE data = (MOS.EFMODEL.DataModels.V_HIS_DEBATE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "DATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.DEBATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "ICD_NAME_DISPLAy")
                        {
                            if (!string.IsNullOrEmpty(data.ICD_CODE))
                            {
                                if (String.IsNullOrEmpty(data.ICD_NAME))
                                {
                                    var currentIcd = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().Where(p => p.ICD_CODE == data.ICD_CODE).FirstOrDefault();
                                    if (currentIcd != null)
                                    {
                                        e.Value = currentIcd.ICD_CODE + "- " + currentIcd.ICD_NAME;
                                    }
                                }
                                else
                                {
                                    e.Value = data.ICD_CODE + "- " + data.ICD_NAME;
                                }
                            }
                        }
                    }
                }
            }


            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDebateReq_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                V_HIS_DEBATE data = null;
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    data = (V_HIS_DEBATE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string departmentCode = (gridViewDebateReq.GetRowCellValue(e.RowHandle, "DEPARTMENT_CODE") ?? "").ToString().Trim();
                    string _CREATOR = (gridViewDebateReq.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    string _loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var treatmentRow = lstTreatment.Where(o => o.ID == data.TREATMENT_ID);
                    if (treatmentRow != null && treatmentRow.Count() > 0)
                    {
                        if (e.Column.FieldName == "DEBATE_EDIT")
                        {
                            if (treatmentRow.First().IS_PAUSE != 1
                                && (HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(_loginName)
                                || (CurrentRoom != null && _CREATOR.Trim() == _loginName
                                && departmentCode == CurrentRoom.DEPARTMENT_CODE)))
                            {
                                e.RepositoryItem = repositoryItemButtonEdit_DebateTab;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEdit_DebateTabDisable;
                            }
                        }
                        else if (e.Column.FieldName == "DEBATE_DELETE")
                        {
                            if (treatmentRow.First().IS_PAUSE != 1
                                && (HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(_loginName)
                                || (CurrentRoom != null && _CREATOR.Trim() == _loginName
                                && departmentCode == CurrentRoom.DEPARTMENT_CODE)))
                            {
                                e.RepositoryItem = repositoryItemButtonDelete_Debate;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonDelete_DebateDisable;
                            }
                        }
                        else if (e.Column.FieldName == "Detail")
                        {
                            if (data != null
                                && (HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(_loginName)
                                || (CurrentRoom != null && data != null && data.CREATOR.Trim() == _loginName
                                && data.DEPARTMENT_CODE == CurrentRoom.DEPARTMENT_CODE)))
                            {
                                e.RepositoryItem = repositoryItemButtonView_DebateTab;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonView_DebateTabDisable;
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void repositoryItemButtonDelete_Debate_ButtonClick(V_HIS_DEBATE HisDebateRow)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            bool value = true;
            try
            {
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (HisDebateRow != null)
                    {
                        //value = value && ExaminationControlProcess_DeleteCheck.CheckValidRequestLoginName(HisDebateRow.CREATOR, param);
                        var treatmentRow = lstTreatment.Where(o => o.ID == HisDebateRow.TREATMENT_ID).First();
                        value = value && CheckValidTreatmentIsPause(treatmentRow.IS_PAUSE ?? -1, param);
                        if (value)
                        {
                            WaitingManager.Show();
                            var result = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_DEBATE_DELETE, ApiConsumers.MosConsumer, HisDebateRow, param);

                            if (result == true)
                            {
                                success = true;
                                gridViewDebateReq.DeleteRow(gridViewDebateReq.FocusedRowHandle);
                            }
                            WaitingManager.Hide();
                        }
                        #region Show message
                        MessageManager.Show(this, param, success);
                        //ResultManager.ShowMessage(param, success);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnnew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnnew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnnew.Enabled) return;

                if (treatmentId > 0)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DebateDiagnostic").FirstOrDefault();
                    if (moduleData == null) throw new ArgumentNullException("khong tim thay moduleLink = HIS.Desktop.Plugins.DebateDiagnostic");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        TreatmentLogADO treatmentLogADO = new ADO.TreatmentLogADO();
                        treatmentLogADO.RoomId = this.Module.RoomId;
                        treatmentLogADO.TreatmentId = treatmentId;
                        listArgs.Add(treatmentLogADO);

                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, Module.RoomId, Module.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)(extenceInstance)).ShowDialog();
                        LoadGridDebate();
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa), "HIS.Desktop.Plugins.DebateDiagnostic"), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonPrint_Print_ButtonClick(V_HIS_DEBATE rowData)
        {
            try
            {
                if (rowData != null)
                {
                    HisDebateFilter filter = new HisDebateFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(new CommonParam()).Get<List<HIS_DEBATE>>("api/HisDebate/Get", ApiConsumers.MosConsumer, filter, null);
                    if (data != null && data.Count > 0)
                    {
                        currentHisDebate = new HIS_DEBATE();
                        currentHisDebate = data.FirstOrDefault();
                        PrintDebateMenuProcessor = new PrintDebateMenuProcessor(PrintMedicine_Click, barManager1);
                        PrintDebateMenuProcessor.InitMenu();
                    }

                    HisTreatmentViewFilter filterTreatment = new HisTreatmentViewFilter();
                    filterTreatment.ID = rowData.TREATMENT_ID;
                    treatmentToPrint = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filterTreatment, null).First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAutoSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAdd = (this.currentControlStateRDO__Create != null && this.currentControlStateRDO__Create.Count > 0) ? this.currentControlStateRDO__Create.Where(o => o.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_DEBATE).FirstOrDefault() : null;

                HIS.Desktop.Library.CacheClient.ControlStateRDO csUpdate = (this.currentControlStateRDO__Update != null && this.currentControlStateRDO__Update.Count > 0) ? this.currentControlStateRDO__Update.Where(o => o.KEY == ControlStateConstant.CHECK_CLEAR_TO_ADD && o.MODULE_LINK == ControlStateConstant.MODULE_LINK_DEBATE_DIAGNOSTIC).FirstOrDefault() : null;

                if (csAdd != null)
                {
                    csAdd.VALUE = (chkAutoSign.Checked ? "1" : "");
                }
                else if (csUpdate != null)
                {
                    csUpdate.VALUE = (chkAutoSign.Checked ? "1" : "");
                }
                else
                {
                    csAdd = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAdd.KEY = ControlStateConstant.CHECK_CLEAR_TO_ADD;
                    csAdd.VALUE = (chkAutoSign.Checked ? "1" : "");
                    csAdd.MODULE_LINK = ControlStateConstant.MODULE_LINK_DEBATE;
                    if (this.currentControlStateRDO__Create == null)
                        this.currentControlStateRDO__Create = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO__Create.Add(csAdd);

                    csUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csUpdate.KEY = ControlStateConstant.CHECK_CLEAR_TO_ADD;
                    csUpdate.VALUE = (chkAutoSign.Checked ? "1" : "");
                    csUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK_DEBATE_DIAGNOSTIC;
                    if (this.currentControlStateRDO__Update == null)
                        this.currentControlStateRDO__Update = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO__Update.Add(csUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO__Create);
                this.controlStateWorker.SetData(this.currentControlStateRDO__Update);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                LoadGridDebate();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void repositoryItemBtnView_Click(V_HIS_DEBATE currentVDebate)
        {
            try
            {
                if (currentVDebate != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DebateDiagnostic").FirstOrDefault();
                    if (moduleData == null) throw new ArgumentNullException("khong tim thay moduleLink = HIS.Desktop.Plugins.DebateDiagnostic");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        HIS_DEBATE currentDebate = new HIS_DEBATE();
                        HisDebateFilter filter = new HisDebateFilter();
                        filter.ID = currentVDebate.ID;
                        var data = new BackendAdapter(new CommonParam()).Get<List<HIS_DEBATE>>("api/HisDebate/Get", ApiConsumers.MosConsumer, filter, null);

                        if (data != null)
                        {
                            currentDebate = data.FirstOrDefault();
                        }

                        List<object> listArgs = new List<object>();
                        listArgs.Add(currentDebate);

                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, Module.RoomId, Module.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)(extenceInstance)).ShowDialog();
                        LoadGridDebate();
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa), "HIS.Desktop.Plugins.DebateDiagnostic"), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCreator_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCreator.Checked)
                {
                    chkUserInvite.Checked = !chkCreator.Checked;
                    chkAll.Checked = !chkCreator.Checked;
                }
                else if (!chkCreator.Checked && !chkUserInvite.Checked)
                {
                    chkAll.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkUserInvite_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkUserInvite.Checked)
                {
                    chkCreator.Checked = !chkUserInvite.Checked;
                    chkAll.Checked = !chkUserInvite.Checked;
                }
                else if (!chkCreator.Checked && !chkUserInvite.Checked)
                {
                    chkAll.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDeFaultConTrol();
                LoadGridDebate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDeFaultConTrol()
        {
            try
            {
                btnPrintDebate.Enabled = false;
                btnPrintDebateSigned.Enabled = false;
                txtKeyword.Text = "";
                if (this.treatmentId == null || this.treatmentId == 0)
                {
                    txtTreatmentCode.Text = "";
                    dtTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                    dtTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;
                }
                if (this.treatmentId > 0)
                {
                    txtTreatmentCode.Enabled = false;
                }
                else
                {
                    layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadGridDebate(); // Du lieu Grid yeu cau
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDebate_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                string checkSTT = "";
                if (chkCreator.Checked)
                {
                    checkSTT = "1";
                }
                else if (chkUserInvite.Checked)
                {
                    checkSTT = "2";
                }
                else if (chkAll.Checked)
                {
                    checkSTT = "3";
                }
                Inventec.Common.Logging.LogSystem.Debug(checkSTT + "___checkSTT");
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "debateStatus" && o.MODULE_LINK == "HIS.Desktop.Plugins.Debate").FirstOrDefault() : null;
                if (csAddOrUpdateValue != null)
                {
                    csAddOrUpdateValue.VALUE = checkSTT;
                }
                else
                {
                    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValue.KEY = "debateStatus";
                    csAddOrUpdateValue.VALUE = checkSTT;
                    csAddOrUpdateValue.MODULE_LINK = "HIS.Desktop.Plugins.Debate";
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateValue);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAll.Checked)
                {
                    chkCreator.Checked = !chkAll.Checked;
                    chkUserInvite.Checked = !chkAll.Checked;
                }
                else if (!chkCreator.Checked && !chkUserInvite.Checked && !chkAll.Checked)
                {
                    chkCreator.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadGridDebate(); // Du lieu Grid yeu cau
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDebateReq_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                if (this.isProcessingTMP == true)
                {
                    return;
                }
                GridHitInfo hi = e.HitInfo;
                var position = Cursor.Position;
                if (hi.InRowCell)
                {
                    int rowHandle = gridViewDebateReq.GetVisibleRowHandle(hi.RowHandle);

                    this.debate_ForProcess = (V_HIS_DEBATE)gridViewDebateReq.GetRow(rowHandle);

                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }
                    popupMenuProcessor = new PopupMenuProcessor(this.debate_ForProcess, barManager1, Debate_MouseRightClick, (RefeshReference)BtnSearch);//(RefeshReference)BtnSearch
                    popupMenuProcessor.InitMenu(position);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnSearch()
        {
            try
            {
                if (btnFind.Enabled)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboExecuteDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dienDieuTri = "";
                if (executeDepartmentSelecteds != null && executeDepartmentSelecteds.Count > 0)
                {
                    foreach (var item in executeDepartmentSelecteds)
                    {
                        dienDieuTri += item.DEPARTMENT_NAME + ", ";
                    }
                }

                e.DisplayText = dienDieuTri;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboExecuteDepartment.Enabled = false;
                cboExecuteDepartment.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExecuteDepartment.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = cboExecuteDepartment.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboExecuteDepartment.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrintDebate_Click(object sender, EventArgs e)
        {
            try
            {
                this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                // check xem co phai in gop khong
                isGroupMps020 = lstDebateToPrint != null && lstDebateToPrint.Count() > 0;

                this.lstDebateToPrint = this.lstDebateToPrint.OrderByDescending(o => o.DEBATE_TIME).ToList();
                foreach (var debate in lstDebateToPrint)
                {
                    this.debateToPrint = debate;
                    bool result = false;
                    if (!string.IsNullOrEmpty(currentFileName020))
                    {
                        InSoBienBanHoiChanProcessGroup(this.currentTypeCode020, currentFileName020, ref result, debate);
                    }
                    else
                    {
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__SO_BIEN_BAN_HOI_CHAN__MPS000020, DelegateRunPrinter);
                    }
                }
                PrintMerge();

                // sau khi in xong thi gan lai = false
                isGroupMps020 = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintMerge()
        {
            try
            {
                Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo adodata = this.GroupStreamPrint.First();

                Inventec.Common.Logging.LogSystem.Debug("List MPS Group: " + string.Join("; ", this.GroupStreamPrint.Select(s => s.printTypeCode).Distinct()));
                Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(adodata.saveMemoryStream, adodata.printerName, adodata.fileName, adodata.numCopy, true,
                    adodata.isAllowExport, adodata.TemplateKey, adodata.eventLog, adodata.eventPrint, adodata.EmrInputADO, adodata.PrintLog, adodata.ShowPrintLog, adodata.IsAllowEditTemplateFile, adodata.IsSingleCopy);
                printProcess.SetPartialFile(this.GroupStreamPrint);
                printProcess.PrintPreviewShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDebateReq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                btnPrintDebate.Enabled = btnPrintDebateSigned.Enabled = gridViewDebateReq.GetSelectedRows().Count() > 0;

                if (gridViewDebateReq.GetSelectedRows().Count() > 0)
                {
                    lstDebateToPrint = new List<V_HIS_DEBATE>();
                    var rowHandles = gridViewDebateReq.GetSelectedRows();
                    if (rowHandles != null && rowHandles.Count() > 0)
                    {
                        foreach (var i in rowHandles)
                        {
                            var row = (V_HIS_DEBATE)gridViewDebateReq.GetRow(i);
                            if (row != null)
                            {
                                lstDebateToPrint.Add(row);
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDebateReq_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        var debate = (V_HIS_DEBATE)view.GetRow(hi.RowHandle);
                        var treatmentRow = lstTreatment.FirstOrDefault(o => o.ID == debate.TREATMENT_ID);
                        string _loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                        if (hi.Column.FieldName == "PRINT")
                        {
                            #region ----- In -----
                            if (debate != null)
                            {
                                repositoryItemButtonPrint_Print_ButtonClick(debate);
                            }
                            #endregion
                        }
                        #region ----- Sửa -----
                        else if (hi.Column.FieldName == "DEBATE_EDIT")
                        {

                            if (debate != null && treatmentRow != null && treatmentRow.IS_PAUSE != 1 && (HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(_loginName)
                        || (CurrentRoom != null && debate != null && debate.CREATOR.Trim() == _loginName
                        && debate.DEPARTMENT_CODE == CurrentRoom.DEPARTMENT_CODE)))
                            {
                                repositoryItemButtonEdit_DebateTab_Click(debate);
                            }
                        }
                        #endregion

                        else if (hi.Column.FieldName == "Detail")
                        {
                            #region ----- Xem -----
                            if (debate != null
                                && (HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(_loginName)
                                || (CurrentRoom != null && debate != null && debate.CREATOR.Trim() == _loginName
                                && debate.DEPARTMENT_CODE == CurrentRoom.DEPARTMENT_CODE)))
                            {
                                repositoryItemBtnView_Click(debate);
                            }
                            #endregion
                        }
                        else if (hi.Column.FieldName == "DEBATE_DELETE")
                        {
                            #region ----- Xóa -----
                            if (debate != null)
                            {
                                repositoryItemButtonDelete_Debate_ButtonClick(debate);
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void repositoryItemButtonEdit_DebateTab_Click(V_HIS_DEBATE currentVDebate)
        {
            try
            {
                if (currentVDebate != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DebateDiagnostic").FirstOrDefault();
                    if (moduleData == null)  throw new ArgumentNullException("khong tim thay moduleLink = HIS.Desktop.Plugins.DebateDiagnostic");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_DEBATE, HIS_DEBATE>();
                        HIS_DEBATE currentDebate = AutoMapper.Mapper.Map<V_HIS_DEBATE, HIS_DEBATE>(currentVDebate);
                        HisDebateFilter filter = new HisDebateFilter();
                        filter.ID = currentVDebate.ID;
                        var data = new BackendAdapter(new CommonParam()).Get<List<HIS_DEBATE>>("api/HisDebate/Get", ApiConsumers.MosConsumer, filter, null);

                        if (data != null)
                        {
                            currentDebate.SERVICE_ID = data.FirstOrDefault().SERVICE_ID;
                            currentDebate.DEBATE_REASON_ID = data.FirstOrDefault().DEBATE_REASON_ID;
                        }

                        List<object> listArgs = new List<object>();
                        listArgs.Add(currentDebate);

                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, Module.RoomId, Module.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)(extenceInstance)).ShowDialog();
                        LoadGridDebate();
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(String.Format(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa), "HIS.Desktop.Plugins.DebateDiagnostic"), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private List<EmrDocumentFileSDO> GetEmrDocumentFile(V_EMR_DOCUMENT document, List<long> docIds, bool? IsMerge, bool? IsShowPatientSign, bool? IsShowWatermark)
        {
            CommonParam paramCommon = new CommonParam();
            EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
            var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
            if (document != null)
                emrFilter.ID = document.ID;
            else
                emrFilter.IDs = docIds;
            sdo.EmrDocumentViewFilter = emrFilter;
            sdo.IsMerge = IsMerge;
            sdo.IsShowPatientSign = IsShowPatientSign;
            sdo.IsShowWatermark = IsShowWatermark;
            sdo.RoomCode = CurrentRoom != null ? CurrentRoom.ROOM_CODE : null;
            sdo.DepartmentCode = CurrentRoom != null ? CurrentRoom.DEPARTMENT_CODE : null;
            return new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
        }

        private void btnPrintDebateSigned_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstDebateToPrint == null || lstDebateToPrint.Count == 0)
                    return;
                this.lstDebateToPrint = this.lstDebateToPrint.OrderByDescending(o => o.DEBATE_TIME).ToList();
                EmrDocumentViewFilter ft = new EmrDocumentViewFilter();
                ft.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__DEBATE;
                ft.HAS_REJECTER = false;
                ft.HAS_NEXT_SIGNER = false;
                ft.TREATMENT_CODEs = lstDebateToPrint.Select(o => o.TREATMENT_CODE).ToList();

                var emrSigned = new BackendAdapter(new CommonParam()).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, ft, null);
                if (emrSigned == null || emrSigned.Count() == 0) return;

                var listSignToPrint = emrSigned.Where(o => lstDebateToPrint.Exists(p => !string.IsNullOrEmpty(o.HIS_CODE) && o.HIS_CODE.Contains(p.ID.ToString())) && o.HIS_CODE.Contains("Mps000020")).ToList();
                if (listSignToPrint == null || listSignToPrint.Count() == 0) return;

                var documents = GetEmrDocumentFile(null, listSignToPrint.Select(o => o.ID).ToList(), true, true, false);

                string output = Utils.GenerateTempFileWithin();
                if (documents != null && documents.Count > 0)
                {
                    List<string> joinStreams = new List<string>();

                    List<MemoryStream> documentData = new List<MemoryStream>();
                    foreach (var item in documents)
                    {
                        if (item.Extension.ToLower().Equals("pdf"))
                        {
                            string pdfAddFile = Utils.GenerateTempFileWithin();
                            Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(item.Base64Data))), pdfAddFile);
                            joinStreams.Add(pdfAddFile);
                        }
                    }

                    Stream currentStream = File.Open(output, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                    var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                    var pages = new List<int>();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

                    foreach (var file in joinStreams)
                    {
                        iTextSharp.text.pdf.PdfReader pdfReader = null;
                        pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                        pages = new List<int>();
                        for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                        {
                            pages.Add(i);
                        }
                        pdfReader.SelectPages(pages);
                        pdfConcat.AddPages(pdfReader);
                        pdfReader.Close();
                    }
                    try
                    {
                        pdfConcat.Close();
                    }
                    catch { }

                    foreach (var file in joinStreams)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }
                    }


                    Inventec.Common.Logging.LogSystem.Warn("output: " + output);


                    Inventec.Common.DocumentViewer.Template.frmPdfViewer DocumentView = new Inventec.Common.DocumentViewer.Template.frmPdfViewer(output);

                    DocumentView.Text = "In";
                    DocumentView.ShowDialog();

                }
                else
                {
                    MessageManager.Show("Khong lay duoc file");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadDictionary(List<V_EMR_DOCUMENT> listEmrDocument)
        {
            try
            {
                DicoutPdfFile = new Dictionary<long, string>();
                foreach (var item in listEmrDocument)
                {
                    outPdfFile = "";
                    if (!String.IsNullOrEmpty(item.MERGE_CODE))
                    {
                        if (String.IsNullOrEmpty(outPdfFile))
                        {
                            CommonParam paramCommon = new CommonParam();

                            DocumentMergeSDO documentMergeSDO = new DocumentMergeSDO();
                            documentMergeSDO.MergeCode = item.MERGE_CODE;
                            string base64PdfDocumentMerge = new BackendAdapter(paramCommon).Post<string>("api/EmrDocument/MakeDocumentMergeBySdo", ApiConsumers.EmrConsumer, documentMergeSDO, paramCommon);
                            if (!String.IsNullOrEmpty(base64PdfDocumentMerge))
                            {
                                Utils.ByteToFile(Convert.FromBase64String(base64PdfDocumentMerge), outPdfFile);
                            }
                        }

                        if (DicoutPdfFile.ContainsKey(item.ID) == false)
                        {
                            DicoutPdfFile.Add(item.ID, outPdfFile);
                        }
                    }
                    else
                    {
                        if (DicoutPdfFile.ContainsKey(item.ID) == false)
                        {
                            DicoutPdfFile.Add(item.ID, outPdfFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void InsertPage1(Stream sourceStream, string sourceFile, List<string> fileListJoin, string desFileJoined)
        {
            List<string> joinStreams = new List<string>();
            if (fileListJoin != null && fileListJoin.Count > 0)
            {
                iTextSharp.text.pdf.PdfReader reader1 = null;
                if (!String.IsNullOrEmpty(sourceFile) && File.Exists(sourceFile))
                {
                    reader1 = new iTextSharp.text.pdf.PdfReader(sourceFile);
                }
                else if (sourceStream != null)
                {
                    reader1 = new iTextSharp.text.pdf.PdfReader(sourceStream);
                }
                int pageCount = reader1.NumberOfPages;
                iTextSharp.text.Rectangle pageSize = reader1.GetPageSizeWithRotation(reader1.NumberOfPages);
                iTextSharp.text.Rectangle pageSize1 = new iTextSharp.text.Rectangle(pageSize.Left, pageSize.Bottom, pageSize.Right, (pageSize.Bottom + pageSize.Height), pageSize.Rotation);

                foreach (var item in fileListJoin)
                {
                    int lIndex1 = item.LastIndexOf(".");
                    string EXTENSION = item.Substring(lIndex1 > 0 ? lIndex1 + 1 : lIndex1);
                    if (EXTENSION != "pdf")
                    {
                        var stream = Inventec.Fss.Client.FileDownload.GetFile(item);
                        stream.Position = 0;
                        string convertTpPdf = Utils.GenerateTempFileWithin();
                        Stream streamConvert = new FileStream(convertTpPdf, FileMode.Create, FileAccess.Write);
                        iTextSharp.text.Document iTextdocument = new iTextSharp.text.Document(pageSize1, 0, 0, 0, 0);
                        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(iTextdocument, streamConvert);
                        iTextdocument.Open();
                        writer.Open();

                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(stream);
                        if (img.Height > img.Width)
                        {
                            float percentage = 0.0f;
                            percentage = pageSize.Height / img.Height;
                            img.ScalePercent(percentage * 100);
                        }
                        else
                        {
                            float percentage = 0.0f;
                            percentage = pageSize.Width / img.Width;
                            img.ScalePercent(percentage * 100);
                        }
                        iTextdocument.Add(img);
                        iTextdocument.Close();
                        writer.Close();

                        joinStreams.Add(convertTpPdf);
                    }
                    else
                    {

                        //string joinFileResult = Utils.GenerateTempFileWithin();
                        //var streamSource = FssFileDownload.GetFile(item);
                        //streamSource.Position = 0;
                        //Stream streamConvert = new FileStream(joinFileResult, FileMode.Create, FileAccess.Write);
                        //iTextSharp.text.Document iTextdocument = new iTextSharp.text.Document(pageSize1, 0, 0, 0, 0);
                        //iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(iTextdocument, streamConvert);
                        var stream = Inventec.Fss.Client.FileDownload.GetFile(item);

                        if (stream != null && stream.Length > 0)
                        {
                            stream.Position = 0;
                            string pdfAddFile = Utils.GenerateTempFileWithin();
                            Utils.ByteToFile(Utils.StreamToByte(stream), pdfAddFile);
                            joinStreams.Add(pdfAddFile);
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Loi convert va luu tam file pdf tu server fss ve may tram____item=" + item);
                        }
                    }
                }

                Stream currentStream = File.Open(desFileJoined, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                var pages = new List<int>();
                for (int i = 0; i <= reader1.NumberOfPages; i++)
                {
                    pages.Add(i);
                }
                reader1.SelectPages(pages);
                pdfConcat.AddPages(reader1);


                foreach (var file in joinStreams)
                {
                    iTextSharp.text.pdf.PdfReader pdfReader = null;
                    pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                    pages = new List<int>();
                    for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                    {
                        pages.Add(i);
                    }
                    pdfReader.SelectPages(pages);
                    pdfConcat.AddPages(pdfReader);
                    pdfReader.Close();
                }

                try
                {
                    reader1.Close();
                }
                catch { }

                try
                {
                    if (sourceStream != null)
                        sourceStream.Close();
                }
                catch { }

                try
                {
                    pdfConcat.Close();
                }
                catch { }

                foreach (var file in joinStreams)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
        }

        internal static void InsertPageOne(Stream sourceFile, string streamSourceStr, string desFileJoined)
        {
            List<string> joinStreams = new List<string>();
            iTextSharp.text.pdf.PdfReader reader1 = null;
            if (sourceFile != null)
            {
                reader1 = new PdfReader(sourceFile);
            }
            if (!string.IsNullOrEmpty(streamSourceStr))
            {
                reader1 = new PdfReader(streamSourceStr);
            }

            int pageCount = reader1.NumberOfPages;
            iTextSharp.text.Rectangle pageSize = reader1.GetPageSizeWithRotation(reader1.NumberOfPages);
            iTextSharp.text.Rectangle pageSize1 = new iTextSharp.text.Rectangle(pageSize.Left, pageSize.Bottom, pageSize.Right, (pageSize.Bottom + pageSize.Height), pageSize.Rotation);

            Stream currentStream = File.Open(desFileJoined, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

            var pages = new List<int>();
            for (int i = 0; i <= reader1.NumberOfPages; i++)
            {
                pages.Add(i);
            }
            reader1.SelectPages(pages);
            pdfConcat.AddPages(reader1);

            foreach (var file in joinStreams)
            {
                iTextSharp.text.pdf.PdfReader pdfReader = null;
                pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                pages = new List<int>();
                for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                {
                    pages.Add(i);
                }
                pdfReader.SelectPages(pages);
                pdfConcat.AddPages(pdfReader);
                pdfReader.Close();
            }

            try
            {
                reader1.Close();
            }
            catch { }

            try
            {
                pdfConcat.Close();
            }
            catch { }

            foreach (var file in joinStreams)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }

        private void repositoryItemButtonPrint_Print_Click(object sender, EventArgs e)
        {
            try
            {
                var debate = (V_HIS_DEBATE)gridViewDebateReq.GetFocusedRow();
                if (debate != null)
                {
                    repositoryItemButtonPrint_Print_ButtonClick(debate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

