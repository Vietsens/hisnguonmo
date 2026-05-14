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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PrepareAndExportByArea.Popup;
using HIS.Desktop.Plugins.PrepareAndExportByArea.Validate;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PrepareAndExportByArea.Run
{
    public partial class frmPrepareAndExportByArea : UserControlBase
    {
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private long medistockId = 0;
        private List<long> medistockIds = new List<long>();
        const string timerLoadCPA = "timerLoadCPA";
        private List<HIS_EXP_MEST> lstAll { get; set; }
        private List<HIS_EXP_MEST> lstSendCPA { get; set; }
        private List<HIS_EXP_MEST> lstTab1 { get; set; }
        private List<HIS_EXP_MEST> lstTab2 { get; set; }
        private List<HIS_EXP_MEST> lstTab3 { get; set; }
        private List<HIS_EXP_MEST> lstTab4 { get; set; }
        private List<HIS_EXP_MEST> lstTab5 { get; set; }
        private HIS_EXP_MEST dataPrintMps480 { get; set; }
        private List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE> lstExpMestMedicine { get; set; }
        private List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL> lstExpMestMaterial { get; set; }
        private List<V_HIS_EXP_MEST> lstVExpMest { get; set; }
        private HIS_TREATMENT treatment { get; set; }
        private HIS_EXP_MEST currentCall { get; set; }

        public static HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        public static List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private string moduleLink = "HIS.Desktop.Plugins.PrepareAndExportByArea";
        public static string txtGateCodeString { get; set; }
        public static string txtIpCPA { get; set; }
        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;
        private int positionHandle;
        private bool IsPrintNow = false;
        public frmPrepareAndExportByArea(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmPrepareAndExportByArea_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                spnSecondLoadTab.EditValue = null;
                var currentRoom = BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                if (currentRoom == null) return;

                if (!currentRoom.AREA_ID.HasValue)
                {
                    medistockId = BackendDataWorker
                        .Get<HIS_MEDI_STOCK>()
                        .Where(o => o.ROOM_ID == currentRoom.ID)
                        .Select(o => o.ID)
                        .FirstOrDefault();
                }
                else
                {
                    medistockIds = BackendDataWorker
                        .Get<HIS_MEDI_STOCK>()
                        .Where(ms => BackendDataWorker
                            .Get<HIS_ROOM>()
                            .Any(r => r.AREA_ID == currentRoom.AREA_ID && r.ID == ms.ROOM_ID))
                        .Select(ms => ms.ID)
                        .ToList();
                }
                dteStt.DateTime = DateTime.Now;
                SetValidate();
                LoadListDataSource();
                LoadAllTab();
                InitControlState();
                RunTimerLoadCPA();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void SetValidate()
        {
            try
            {
                ValidDate valid = new ValidDate();
                valid.dte = dteStt;
                dxValidationProvider1.SetValidationRule(dteStt, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadCallPatientRefresh()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(Refesh_));
            //thread.Priority = ThreadPriority.Highest;
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void Refesh_()
        {
            try
            {
                if (this.clienttManager == null)
                    this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager(txtIpCPA);

                List<HIS_EXP_MEST> snapshot = null;
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        snapshot = lstAll == null ? null : new List<HIS_EXP_MEST>(lstAll);
                    }));
                }
                if (snapshot == null) return;

                List<CPA.WCFClient.CallPatientClient.ADO.OrderDataADO> listData = new List<CPA.WCFClient.CallPatientClient.ADO.OrderDataADO>();
                lstSendCPA = snapshot.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE).ToList();
                if (lstSendCPA != null && lstSendCPA.Count() > 0)
                {
                    foreach (var item in lstSendCPA)
                    {
                        CPA.WCFClient.CallPatientClient.ADO.OrderDataADO CallPatientInfoADO_ = new CPA.WCFClient.CallPatientClient.ADO.OrderDataADO();
                        CallPatientInfoADO_.ExpMestId = item.ID;
                        CallPatientInfoADO_.OrderNumber = item.NUM_ORDER;
                        CallPatientInfoADO_.GateCode = item.GATE_CODE;
                        CallPatientInfoADO_.IsPriority = item.PRIORITY == 1;
                        CallPatientInfoADO_.OrderTime = item.LAST_APPROVAL_TIME;
                        CallPatientInfoADO_.IsCalling = false;
                        CallPatientInfoADO_.CallTime = item.CALL_TIME;
                        CallPatientInfoADO_.PatientName = item.TDL_PATIENT_NAME;
                        listData.Add(CallPatientInfoADO_);
                    }
                    listData = listData.OrderByDescending(o => o.CallTime).ToList();
                }
                this.clienttManager.UpdateListOrderDataCalling(txtGateCodeString, listData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadListDataSource()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestFilter filter = new HisExpMestFilter();
                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK;
                if (medistockId > 0)
                    filter.MEDI_STOCK_ID = medistockId;
                if (medistockIds != null && medistockIds.Count > 0)
                {
                    filter.MEDI_STOCK_IDs = medistockIds;
                }
                if (dteStt.EditValue != null && dteStt.DateTime != DateTime.MinValue)
                    filter.CREATE_DATE__EQUAL = Int64.Parse(dteStt.DateTime.ToString("yyyyMMdd000000"));
                lstAll = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadAllTab()
        {
            try
            {
                if (lstAll != null && lstAll.Count > 0)
                {
                    LoadTab1();
                    LoadTab2();
                    LoadTab3();
                    LoadTab4();
                    LoadTab5();
                }
                else
                {
                    gcWaiting.DataSource = null;
                    gcAbssentN.DataSource = null;
                    gcPassMedicine.DataSource = null;
                    gcPrepareMedicine.DataSource = null;
                    gcPrinted.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAutoLoadTab_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                spnSecondLoadTab.Enabled = chkAutoLoadTab.Checked;

                SaveState();
                RunTimerLoadCPA();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RunTimerLoadCPA()
        {
            try
            {
                if (chkAutoLoadTab.Checked && spnSecondLoadTab.EditValue != null)
                {
                    StopTimer(this.currentModule.ModuleLink, timerLoadCPA);
                    var timerLoadCPA_Interval = (int)(spnSecondLoadTab.Value * 1000);
                    DisposeTimer(this.currentModule.ModuleLink, timerLoadCPA);
                    RegisterTimer(this.currentModule.ModuleLink, timerLoadCPA, timerLoadCPA_Interval, timerLoadCPA_Tick);
                    StartTimer(this.currentModule.ModuleLink, timerLoadCPA);
                }
                else
                {
                    StopTimer(this.currentModule.ModuleLink, timerLoadCPA);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SaveState()
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csNotPrint =
                (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                ? currentControlStateRDO.Where(o => o.KEY == chkNotPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault()
                : null;
                if (csNotPrint != null)
                {
                    csNotPrint.VALUE = chkNotPrint.Checked ? "1" : "0";
                }
                else
                {
                    csNotPrint = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csNotPrint.KEY = chkNotPrint.Name;
                    csNotPrint.VALUE = chkNotPrint.Checked ? "1" : "0";
                    csNotPrint.MODULE_LINK = moduleLink;
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(csNotPrint);
                }
                controlStateWorker.SetData(currentControlStateRDO);
                WaitingManager.Hide();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == chkAutoLoadTab.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.PrepareAndExportByArea").FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chkAutoLoadTab.Checked ? "1" : "0";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoLoadTab.Name;
                    csAddOrUpdate.VALUE = chkAutoLoadTab.Checked ? "1" : "0";
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.PrepareAndExportByArea";
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(csAddOrUpdate);
                }
                controlStateWorker.SetData(currentControlStateRDO);
                WaitingManager.Hide();

                if (chkAutoLoadTab.Checked && spnSecondLoadTab.EditValue != null)
                {
                    WaitingManager.Show();
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateSpn = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == spnSecondLoadTab.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.PrepareAndExportByArea").FirstOrDefault() : null;
                    if (csAddOrUpdateSpn != null)
                    {
                        csAddOrUpdateSpn.VALUE = spnSecondLoadTab.Value.ToString();
                    }
                    else
                    {
                        csAddOrUpdateSpn = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdateSpn.KEY = spnSecondLoadTab.Name;
                        csAddOrUpdateSpn.VALUE = spnSecondLoadTab.Value.ToString();
                        csAddOrUpdateSpn.MODULE_LINK = "HIS.Desktop.Plugins.PrepareAndExportByArea";
                        if (currentControlStateRDO == null)
                            currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        currentControlStateRDO.Add(csAddOrUpdateSpn);
                    }
                    controlStateWorker.SetData(currentControlStateRDO);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerLoadCPA_Tick()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("TIMER TẢI LẠI ___");
                LoadListDataSource();
                if (!string.IsNullOrEmpty(txtGateCodeString) && dteStt.DateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                {
                    CreateThreadCallPatientRefresh();
                }
                switch (xtraTabControl1.SelectedTabPageIndex)
                {
                    case 0:
                        LoadTab1();
                        break;
                    case 1:
                        LoadTab2();
                        break;
                    case 2:
                        LoadTab3();
                        break;
                    case 3:
                        LoadTab4();
                        break;
                    case 4:
                        LoadTab5();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    gvWaiting.FocusedRowHandle = 0;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1)
                {
                    if (lstTab2 != null && lstTab2.Count > 0)
                    {
                        gvPrinted.Focus();
                        gvPrinted.FocusedColumn = gridColumn17;
                        gvPrinted.FocusedRowHandle = DevExpress.XtraGrid.GridControl.AutoFilterRowHandle;
                    }
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2)
                {
                    gcPrepareMedicine.BeginInvoke(new Action(() =>
                    {
                        gvPrepareMedicine.Focus();
                        gvPrepareMedicine.FocusedRowHandle = DevExpress.XtraGrid.GridControl.AutoFilterRowHandle;
                        gvPrepareMedicine.FocusedColumn = gridColumn26; // cột Mã điều trị
                        gvPrepareMedicine.ShowEditor();
                        (gvPrepareMedicine.ActiveEditor as DevExpress.XtraEditors.BaseEdit)?.SelectAll();
                    }));
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 3)
                {
                    gcAbssentN.BeginInvoke(new Action(() =>
                    {
                        gvAbssentN.Focus();
                        gvAbssentN.FocusedRowHandle = DevExpress.XtraGrid.GridControl.AutoFilterRowHandle;
                        gvAbssentN.FocusedColumn = gridColumn35; // cột Mã điều trị
                        gvAbssentN.ShowEditor();
                        (gvAbssentN.ActiveEditor as DevExpress.XtraEditors.BaseEdit)?.SelectAll();
                    }));
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 4)
                {
                    gcPassMedicine.BeginInvoke(new Action(() =>
                    {
                        gvPassMedicine.Focus();
                        gvPassMedicine.FocusedRowHandle = DevExpress.XtraGrid.GridControl.AutoFilterRowHandle;
                        gvPassMedicine.FocusedColumn = gridColumn45; // cột Mã điều trị
                        gvPassMedicine.ShowEditor();
                        (gvPassMedicine.ActiveEditor as DevExpress.XtraEditors.BaseEdit)?.SelectAll();
                    }));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {

                controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                {
                    foreach (var item in currentControlStateRDO)
                    {
                        if (item.KEY == "txtGateCodeString")
                        {
                            txtGateCodeString = item.VALUE;
                        }
                        else if (item.KEY == chkCallAll.Name)
                        {
                            chkCallAll.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == "AddressIPCPA")
                        {
                            txtIpCPA = item.VALUE;
                        }
                        else if (item.KEY == chkAutoLoadTab.Name)
                        {
                            chkAutoLoadTab.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == spnSecondLoadTab.Name)
                        {
                            spnSecondLoadTab.Value = Decimal.Parse(item.VALUE);
                        }
                        else if (item.KEY == chkNotPrint.Name)
                        {
                            chkNotPrint.Checked = item.VALUE == "1";
                        }
                    }
                    if (!chkAutoLoadTab.Checked)
                        spnSecondLoadTab.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnSecondLoadTab_Leave(object sender, EventArgs e)
        {
            try
            {
                SaveState();
                if (spnSecondLoadTab.Enabled && spnSecondLoadTab.EditValue != null)
                {
                    RunTimerLoadCPA();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnLoadTab_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                LoadListDataSource();
                LoadAllTab();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #region ShortCut
        public void TaiLai()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("TẢI LẠI");
                btnLoadTab_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void InDon()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("IN ĐƠN");
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void DaPhatThuoc()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("ĐÃ PHÁT THUỐC");
                if (!btnGaveMedicine.Enabled)
                    return;
                btnGaveMedicine_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void VangMat()
        {
            try
            {
                if (xtraTabControl1.SelectedTabPageIndex == 2)
                {
                    Inventec.Common.Logging.LogSystem.Warn("VẮNG MẶT");
                    if (!btnAbsent.Enabled)
                        return;
                    btnAbsent_Click(null, null);
                }

                if (xtraTabControl1.SelectedTabPageIndex == 3)
                {
                    if (!btnUnAbsent.Enabled)
                        return;

                    btnUnAbsent_Click(null, null);
                }



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void Goi()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("GỌI");
                if (!btnCall.Enabled)
                    return;
                btnCall_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteStt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                btnCall.Enabled = false;
                if (dteStt.EditValue != null && dteStt.DateTime != DateTime.MinValue)
                {
                    if (dteStt.DateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                    {
                        btnCall.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repViewWaiting_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_EXP_MEST rowData = (HIS_EXP_MEST)gvWaiting.GetFocusedRow();
                if (rowData == null) return;

                var expMestCodes = rowData.EXP_MEST_CODE?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ToList();

                if (expMestCodes == null || expMestCodes.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có mã phiếu xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (expMestCodes.Count == 1)
                {
                    var expMest = lstAll.FirstOrDefault(x => x.EXP_MEST_CODE == expMestCodes[0]);
                    if (expMest != null)
                    {
                        V_HIS_EXP_MEST viewData = new V_HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(viewData, expMest);
                        OpenModuleAggrExpMestDetail(viewData);
                    }
                    return;
                }

                System.Windows.Forms.ContextMenuStrip menu = new System.Windows.Forms.ContextMenuStrip();

                foreach (var code in expMestCodes)
                {
                    var expMest = lstAll.FirstOrDefault(x => x.EXP_MEST_CODE == code);
                    if (expMest != null)
                    {
                        ToolStripMenuItem menuItem = new ToolStripMenuItem(code);
                        menuItem.Tag = expMest;
                        menuItem.Click += MenuItemExpMest_Click;
                        menu.Items.Add(menuItem);
                    }
                }

                if (menu.Items.Count > 0)
                {
                    menu.Show(Cursor.Position);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất trong danh sách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repViewPrinted_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_EXP_MEST rowData = (HIS_EXP_MEST)gvPrinted.GetFocusedRow();
                if (rowData == null) return;

                // Split EXP_MEST_CODE để lấy danh sách mã
                var expMestCodes = rowData.EXP_MEST_CODE?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ToList();

                if (expMestCodes == null || expMestCodes.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có mã phiếu xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Nếu chỉ có 1 phiếu xuất → Mở trực tiếp
                if (expMestCodes.Count == 1)
                {
                    var expMest = lstAll.FirstOrDefault(x => x.EXP_MEST_CODE == expMestCodes[0]);
                    if (expMest != null)
                    {
                        V_HIS_EXP_MEST viewData = new V_HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(viewData, expMest);
                        OpenModuleAggrExpMestDetail(viewData);
                    }
                    return;
                }

                // Nếu có nhiều phiếu xuất (>= 2) → Hiển thị menu
                System.Windows.Forms.ContextMenuStrip menu = new System.Windows.Forms.ContextMenuStrip();

                // THÊM CÁC MENU ITEM VÀO MENU
                foreach (var code in expMestCodes)
                {
                    var expMest = lstAll.FirstOrDefault(x => x.EXP_MEST_CODE == code);
                    if (expMest != null)
                    {
                        // Tạo menu item với text là mã phiếu xuất
                        ToolStripMenuItem menuItem = new ToolStripMenuItem(code);
                        // Lưu object vào Tag để dùng khi click
                        menuItem.Tag = expMest;
                        // Gắn sự kiện click
                        menuItem.Click += MenuItemExpMest_Click;
                        // THÊM VÀO MENU
                        menu.Items.Add(menuItem);
                    }
                }

                if (menu.Items.Count > 0)
                {
                    // Hiển thị menu tại vị trí chuột
                    menu.Show(Cursor.Position);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất trong danh sách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MenuItemExpMest_Click(object sender, EventArgs e)
        {
            try
            {
                var menuItem = sender as ToolStripMenuItem;
                if (menuItem == null) return;

                var selectedExpMest = menuItem.Tag as HIS_EXP_MEST;
                if (selectedExpMest != null)
                {
                    V_HIS_EXP_MEST viewData = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(viewData, selectedExpMest);
                    OpenModuleAggrExpMestDetail(viewData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repViewCall_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_EXP_MEST rowData = (HIS_EXP_MEST)gvPrepareMedicine.GetFocusedRow();
                if (rowData == null) return;

                var expMestCodes = rowData.EXP_MEST_CODE?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ToList();

                if (expMestCodes == null || expMestCodes.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có mã phiếu xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (expMestCodes.Count == 1)
                {
                    var expMest = lstAll.FirstOrDefault(x => x.EXP_MEST_CODE == expMestCodes[0]);
                    if (expMest != null)
                    {
                        V_HIS_EXP_MEST viewData = new V_HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(viewData, expMest);
                        OpenModuleAggrExpMestDetail(viewData);
                    }
                    return;
                }

                System.Windows.Forms.ContextMenuStrip menu = new System.Windows.Forms.ContextMenuStrip();

                foreach (var code in expMestCodes)
                {
                    var expMest = lstAll.FirstOrDefault(x => x.EXP_MEST_CODE == code);
                    if (expMest != null)
                    {
                        ToolStripMenuItem menuItem = new ToolStripMenuItem(code);
                        menuItem.Tag = expMest;
                        menuItem.Click += MenuItemExpMest_Click;
                        menu.Items.Add(menuItem);
                    }
                }

                if (menu.Items.Count > 0)
                {
                    menu.Show(Cursor.Position);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất trong danh sách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repViewN_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_EXP_MEST rowData = (HIS_EXP_MEST)gvAbssentN.GetFocusedRow();
                if (rowData == null) return;

                var expMestCodes = rowData.EXP_MEST_CODE?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ToList();

                if (expMestCodes == null || expMestCodes.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có mã phiếu xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (expMestCodes.Count == 1)
                {
                    var expMest = lstAll.FirstOrDefault(x => x.EXP_MEST_CODE == expMestCodes[0]);
                    if (expMest != null)
                    {
                        V_HIS_EXP_MEST viewData = new V_HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(viewData, expMest);
                        OpenModuleAggrExpMestDetail(viewData);
                    }
                    return;
                }

                System.Windows.Forms.ContextMenuStrip menu = new System.Windows.Forms.ContextMenuStrip();

                foreach (var code in expMestCodes)
                {
                    var expMest = lstAll.FirstOrDefault(x => x.EXP_MEST_CODE == code);
                    if (expMest != null)
                    {
                        ToolStripMenuItem menuItem = new ToolStripMenuItem(code);
                        menuItem.Tag = expMest;
                        menuItem.Click += MenuItemExpMest_Click;
                        menu.Items.Add(menuItem);
                    }
                }

                if (menu.Items.Count > 0)
                {
                    menu.Show(Cursor.Position);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất trong danh sách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repViewNq_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_EXP_MEST rowData = (HIS_EXP_MEST)gvPassMedicine.GetFocusedRow();
                if (rowData == null) return;

                var expMestCodes = rowData.EXP_MEST_CODE?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ToList();

                if (expMestCodes == null || expMestCodes.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có mã phiếu xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (expMestCodes.Count == 1)
                {
                    var expMest = lstAll.FirstOrDefault(x => x.EXP_MEST_CODE == expMestCodes[0]);
                    if (expMest != null)
                    {
                        V_HIS_EXP_MEST viewData = new V_HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(viewData, expMest);
                        OpenModuleAggrExpMestDetail(viewData);
                    }
                    return;
                }

                System.Windows.Forms.ContextMenuStrip menu = new System.Windows.Forms.ContextMenuStrip();

                foreach (var code in expMestCodes)
                {
                    var expMest = lstAll.FirstOrDefault(x => x.EXP_MEST_CODE == code);
                    if (expMest != null)
                    {
                        ToolStripMenuItem menuItem = new ToolStripMenuItem(code);
                        menuItem.Tag = expMest;
                        menuItem.Click += MenuItemExpMest_Click;
                        menu.Items.Add(menuItem);
                    }
                }

                if (menu.Items.Count > 0)
                {
                    menu.Show(Cursor.Position);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất trong danh sách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenModuleAggrExpMestDetail(V_HIS_EXP_MEST expMest)
        {
            try
            {

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestDetail").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestDetail");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentModule);
                    listArgs.Add(expMest);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                Requirements popup = new Requirements(lstAll);
                popup.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNotPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SaveState();
                LoadTab2();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gcPrepareMedicine_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter) return;

                if (gvPrepareMedicine.FocusedRowHandle != DevExpress.XtraGrid.GridControl.AutoFilterRowHandle) return;

                if (gvPrepareMedicine.FocusedColumn != gridColumn26) return;

                if (gvPrepareMedicine.RowCount != 1) return;

                int rowHandle = gvPrepareMedicine.GetVisibleRowHandle(0);
                var one = gvPrepareMedicine.GetRow(rowHandle) as HIS_EXP_MEST;
                if (one == null) return;
                if (currentCall != null && currentCall.ID != one.ID)
                {
                    return;
                }

                CallSpecific(one);
                //btnCall_Click(null, null);

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CallSpecific(HIS_EXP_MEST one)
        {
            bool rs;
            try
            {
                if (string.IsNullOrEmpty(txtGateCodeString))
                {
                    frmConfig frm = new frmConfig(IsOpen, GateConfig, IpConfig);
                    frm.ShowDialog();
                    return;
                }
                if (this.clienttManager == null)
                    this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager(txtIpCPA);
                var myGate = (txtGateCodeString ?? "").Trim();
                var hisGate = (one?.GATE_CODE ?? "").Trim();

                if (!string.IsNullOrEmpty(hisGate) &&
                    !string.Equals(hisGate, myGate, StringComparison.OrdinalIgnoreCase))
                {
                    rs = this.clienttManager.RecallOrderDataClientBool(one.NUM_ORDER.ToString(), one?.GATE_CODE);
                    Inventec.Common.Logging.LogSystem.Error("GỌI ___" + rs);
                    return;
                }

                currentCall = one;
                txtCurrentCall.Text = currentCall.NUM_ORDER + " - " + currentCall.TDL_PATIENT_NAME + " - " + currentCall.TDL_TREATMENT_CODE;

                rs = this.clienttManager.RecallOrderDataClientBool(currentCall.NUM_ORDER.ToString(), txtGateCodeString);
                Inventec.Common.Logging.LogSystem.Error("GỌI ___" + rs);

                if (txtGateCodeString != currentCall.GATE_CODE)
                {
                    CommonParam param = new CommonParam();
                    ExpMestCallSDO sdo = new ExpMestCallSDO();
                    sdo.ExpMestId = currentCall.ID;
                    sdo.GateCode = txtGateCodeString;

                    WaitingManager.Show();
                    bool success = new Inventec.Common.Adapter.BackendAdapter(param)
                        .Post<bool>("api/HisExpMest/Call", ApiConsumers.MosConsumer, sdo, param);
                    WaitingManager.Hide();

                    if (success)
                    {
                        var item = lstAll.FirstOrDefault(x => x.ID == currentCall.ID);
                        if (item != null) item.GATE_CODE = txtGateCodeString;

                        LoadTab3();
                    }
                    //else
                    //{
                    //    LoadTab3();
                        
                    //    var newTarget = GetTargetFromPrepareGrid();
                    //    if (newTarget != null)
                    //    {
                    //        // Gọi lại với target mới
                    //        CallSpecific(newTarget);
                    //    }
                    //}    
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private HIS_EXP_MEST GetMinOrderForMyGate()
        {
            try
            {
                var myGate = (txtGateCodeString ?? "").Trim();

                var source = lstTab3 ?? lstAll;

                if (source == null || source.Count == 0) return null;

                var pick = source
                    .Where(x => x != null
                                && x.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                                && x.IS_ABSENT != 1
                                && (string.IsNullOrEmpty(x.GATE_CODE) ||
                                    string.Equals((x.GATE_CODE ?? "").Trim(), myGate, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(x => x.NUM_ORDER)
                    .ThenBy(x => x.LAST_APPROVAL_TIME)
                    .FirstOrDefault();

                return pick;
            }
            catch
            {
                return null;
            }
        }

        public List<long> expCodeToId(string expCode)
        {
            try
            {
                var expMestCodes = expCode.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ToList();

                if (expMestCodes == null || expMestCodes.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có mã phiếu xuất để xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
                // Lấy danh sách ID từ lstAll
                List<long> expMestIds = new List<long>();
                foreach (var code in expMestCodes)
                {
                    var expMest = lstAll.FirstOrDefault(o => o.EXP_MEST_CODE == code);
                    if (expMest != null)
                    {
                        expMestIds.Add(expMest.ID);
                    }
                }

                if (expMestIds.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất trong danh sách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                return expMestIds;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private void bbtnF5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CommonParam param = new CommonParam();
            ExpMestDetailResultSDO sdo;
            bool success = false;
            try
            {
                // Chỉ xử lý khi đang ở Tab 1 (Chờ in)
                if (xtraTabControl1.SelectedTabPageIndex != 0)
                {
                    return;
                }

                // Kiểm tra có dữ liệu không
                if (lstTab1 == null || lstTab1.Count == 0)
                {
                    LogSystem.Debug("Không có dữ liệu trong danh sách!");
                    return;
                }

                // Lấy dòng đầu tiên
                HIS_EXP_MEST firstExpMest = lstTab1.First();
                
                // Lấy tất cả phiếu xuất cùng nhóm (cùng TDL_TREATMENT_CODE, IS_CONFIRM, EXP_MEST_STT_ID)
                List<long> groupedExpMestIds = expCodeToId(firstExpMest.EXP_MEST_CODE);

                if (groupedExpMestIds.Count == 0)
                {
                    LogSystem.Debug("Không tìm thấy phiếu xuất nào");
                    return;
                }

                dataPrintMps480 = firstExpMest;
                
                WaitingManager.Show();                
                
                sdo = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<ExpMestDetailResultSDO>("api/HisExpMest/ConfirmAndGetDetails", 
                                                   ApiConsumers.MosConsumer,
                                                   groupedExpMestIds, 
                                                   param);
                
                WaitingManager.Hide();
                
                if (sdo != null)
                {
                    dataPrintMps480 = sdo.ExpMest;
                    lstExpMestMedicine = sdo.ExpMestMedicines;
                    lstExpMestMaterial = sdo.ExpMestMaterials;
                    lstVExpMest = sdo.ViewExpMests;

                    // Cập nhật tất cả các phiếu trong nhóm
                    foreach (var expMestId in groupedExpMestIds)
                    {
                        var item = lstAll.FirstOrDefault(x => x.ID == expMestId);
                        if (item != null)
                        {
                            item.IS_CONFIRM = 1;
                            
                            // Thêm vào Tab 2 (Đã in) nếu chưa có
                            if (lstTab2 == null)
                                lstTab2 = new List<HIS_EXP_MEST>();
                                
                            if (!lstTab2.Any(x => x.ID == item.ID))
                            {
                                lstTab2.Add(item);
                            }
                        }
                    }
                    
                    // Reload tab 2
                    gcPrinted.DataSource = null;
                    gcPrinted.DataSource = lstTab2;
                    
                    // Lấy thông tin treatment
                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    if (dataPrintMps480 != null && dataPrintMps480.TDL_TREATMENT_ID != null)
                    {
                        treatmentFilter.ID = dataPrintMps480.TDL_TREATMENT_ID;
                    }
                    else if (lstExpMestMedicine != null && lstExpMestMedicine.Count > 0)
                    {
                        treatmentFilter.ID = lstExpMestMedicine.FirstOrDefault(o => o.TDL_TREATMENT_ID != null)?.TDL_TREATMENT_ID;
                    }
                    else if (lstExpMestMaterial != null && lstExpMestMaterial.Count > 0)
                    {
                        treatmentFilter.ID = lstExpMestMaterial.FirstOrDefault(o => o.TDL_TREATMENT_ID != null)?.TDL_TREATMENT_ID;
                    }

                    if (treatmentFilter.ID != null)
                    {
                        List<HIS_TREATMENT> lstTreatment = new Inventec.Common.Adapter.BackendAdapter(param)
                            .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", 
                                                       ApiConsumer.ApiConsumers.MosConsumer, 
                                                       treatmentFilter, 
                                                       param);
                        if (lstTreatment != null && lstTreatment.Count > 0)
                        {
                            treatment = lstTreatment.FirstOrDefault();
                        }
                    }
                    
                    success = true;
                    
                    // Reload Tab 1 để remove các item đã xử lý
                    LoadTab1();
                    LoadTab2();
                    IsPrintNow = true;
                    PrintMps480();
                }
                
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnAbsent_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_EXP_MEST data = lstTab4.First();
                if (data == null) return;

                // Lấy danh sách ID từ lstAll
                List<long> groupedExpMestIds = expCodeToId(data.EXP_MEST_CODE);

                if (groupedExpMestIds.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy phiếu xuất trong danh sách", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                HisExpMestSDO sdo = new HisExpMestSDO();
                sdo.ExpMestIds = groupedExpMestIds;
                sdo.ReqRoomId = this.currentModule.RoomId;

                WaitingManager.Show();
                List<V_HIS_EXP_MEST> rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<V_HIS_EXP_MEST>>("api/HisExpMest/UnAbsent", ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();

                if (rs != null && rs.Count > 0)
                {
                    success = true;

                    // Cập nhật trạng thái cho tất cả phiếu trong kết quả
                    foreach (var rsItem in rs)
                    {
                        var item = lstAll.FirstOrDefault(x => x.ID == rsItem.ID);
                        if (item != null)
                        {
                            item.EXP_MEST_STT_ID = rsItem.EXP_MEST_STT_ID;
                            item.IS_ABSENT = null;

                            // Thêm vào Tab 3 (Đã soạn thuốc) nếu chưa có
                            if (lstTab3 == null)
                                lstTab3 = new List<HIS_EXP_MEST>();

                            if (!lstTab3.Any(x => x.ID == item.ID))
                            {
                                lstTab3.Add(item);
                            }
                        }
                    }

                    // Reload tab 3
                    gcPrepareMedicine.DataSource = null;
                    gcPrepareMedicine.DataSource = lstTab3;

                    // Refresh CPA nếu cần
                    if (!string.IsNullOrEmpty(txtGateCodeString) && dteStt.DateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                    {
                        CreateThreadCallPatientRefresh();
                    }

                    LoadTab4();
                    LoadTab3();
                }

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
