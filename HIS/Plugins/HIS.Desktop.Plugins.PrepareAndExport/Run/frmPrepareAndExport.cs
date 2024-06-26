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
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using System.Threading;
using HIS.Desktop.Plugins.PrepareAndExport.Validate;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.PrepareAndExport.Run
{
    public partial class frmPrepareAndExport : UserControlBase
    {
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private long medistockId = 0;
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
        private HIS_TREATMENT treatment { get; set; }
        private HIS_EXP_MEST currentCall { get; set; }

        public static HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        public static List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private string moduleLink = "HIS.Desktop.Plugins.PrepareAndExport";
        public static string txtGateCodeString { get; set; }
        public static string txtIpCPA { get; set; }
        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;
        private int positionHandle;
        private bool IsPrintNow = false;
        public frmPrepareAndExport(Inventec.Desktop.Common.Modules.Module currentModule)
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

        private void frmPrepareAndExport_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                spnSecondLoadTab.EditValue = null;
                medistockId = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId).ID;
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

            if (this.clienttManager == null)
                this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager(txtIpCPA);
            List<CPA.WCFClient.CallPatientClient.ADO.OrderDataADO> listData = new List<CPA.WCFClient.CallPatientClient.ADO.OrderDataADO>();
            lstSendCPA = lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE).ToList();
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

        private void LoadListDataSource()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestFilter filter = new HisExpMestFilter();
                filter.EXP_MEST_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK, IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL};
                filter.MEDI_STOCK_ID = medistockId;
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
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == chkAutoLoadTab.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.PrepareAndExport").FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chkAutoLoadTab.Checked ? "1" : "0";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoLoadTab.Name;
                    csAddOrUpdate.VALUE = chkAutoLoadTab.Checked ? "1" : "0";
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.PrepareAndExport";
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(csAddOrUpdate);
                }
                controlStateWorker.SetData(currentControlStateRDO);
                WaitingManager.Hide();

                if (chkAutoLoadTab.Checked && spnSecondLoadTab.EditValue != null)
                {
                    WaitingManager.Show();
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateSpn = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == spnSecondLoadTab.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.PrepareAndExport").FirstOrDefault() : null;
                    if (csAddOrUpdateSpn != null)
                    {
                        csAddOrUpdateSpn.VALUE = spnSecondLoadTab.Value.ToString();
                    }
                    else
                    {
                        csAddOrUpdateSpn = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdateSpn.KEY = spnSecondLoadTab.Name;
                        csAddOrUpdateSpn.VALUE = spnSecondLoadTab.Value.ToString();
                        csAddOrUpdateSpn.MODULE_LINK = "HIS.Desktop.Plugins.PrepareAndExport";
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
                Inventec.Common.Logging.LogSystem.Warn("VẮNG MẶT");
                if (!btnAbsent.Enabled)
                    return;
                btnAbsent_Click(null, null);
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
            V_HIS_EXP_MEST data = new V_HIS_EXP_MEST();
            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(data, (HIS_EXP_MEST)gvWaiting.GetFocusedRow());
            OpenModuleAggrExpMestDetail(data);
        }

        private void repViewPrinted_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            V_HIS_EXP_MEST data = new V_HIS_EXP_MEST();
            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(data, (HIS_EXP_MEST)gvPrinted.GetFocusedRow());
            OpenModuleAggrExpMestDetail(data);
        }

        private void repViewCall_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            V_HIS_EXP_MEST data = new V_HIS_EXP_MEST();
            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(data, (HIS_EXP_MEST)gvPrepareMedicine.GetFocusedRow());
            OpenModuleAggrExpMestDetail(data);
        }

        private void repViewN_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            V_HIS_EXP_MEST data = new V_HIS_EXP_MEST();
            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(data, (HIS_EXP_MEST)gvAbssentN.GetFocusedRow());
            OpenModuleAggrExpMestDetail(data);
        }

        private void repViewNq_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            V_HIS_EXP_MEST data = new V_HIS_EXP_MEST();
            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(data, (HIS_EXP_MEST)gvPassMedicine.GetFocusedRow());
            OpenModuleAggrExpMestDetail(data);
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

    }
}
