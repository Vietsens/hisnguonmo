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
using HIS.Desktop.Plugins.PrepareAndExport.Popup;
using HIS.Desktop.Plugins.PrepareAndExport.Validate;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
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

        //So id dot dieu tri toi da cho moi lan goi api tra cuu ma benh nhan
        private const int MAX_TREATMENT_ID_PER_CALL = 500;

        //Lan quet gan nhat khop theo ma benh nhan hay ma dieu tri, de LoadTab3 focus lai dung o
        private bool isScanByPatientCode = false;

        #region OddEvenFilter
        internal const string ODD_EVEN_FILTER__ALL = "ALL";
        internal const string ODD_EVEN_FILTER__EVEN = "EVEN";
        internal const string ODD_EVEN_FILTER__ODD = "ODD";

        private string currentOddEvenFilter = ODD_EVEN_FILTER__ALL;

        internal class OddEvenFilterADO
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }
        #endregion
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
                InitComboOddEven();
                LoadListDataSource();
                InitControlState();
                LoadAllTab();
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
                FillMissingPatientCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Bo sung ma benh nhan cho cac phieu khong co TDL_PATIENT_CODE, lay theo dot dieu tri. 
        /// Chi goi api khi thuc su co phieu thieu ma.
        /// </summary>
        private void FillMissingPatientCode()
        {
            try
            {
                if (lstAll == null || lstAll.Count == 0) return;

                List<long> treatmentIds = lstAll
                    .Where(o => string.IsNullOrEmpty(o.TDL_PATIENT_CODE) && o.TDL_TREATMENT_ID.HasValue)
                    .Select(o => o.TDL_TREATMENT_ID.Value)
                    .Distinct()
                    .ToList();
                if (treatmentIds.Count == 0) return;

                Dictionary<long, string> dicPatientCode = new Dictionary<long, string>();
                for (int i = 0; i < treatmentIds.Count; i += MAX_TREATMENT_ID_PER_CALL)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.IDs = treatmentIds.Skip(i).Take(MAX_TREATMENT_ID_PER_CALL).ToList();
                    var treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                    if (treatments == null) continue;
                    foreach (var treatment in treatments)
                    {
                        if (!dicPatientCode.ContainsKey(treatment.ID))
                            dicPatientCode.Add(treatment.ID, treatment.TDL_PATIENT_CODE);
                    }
                }

                foreach (var expMest in lstAll)
                {
                    if (!string.IsNullOrEmpty(expMest.TDL_PATIENT_CODE) || !expMest.TDL_TREATMENT_ID.HasValue) continue;
                    string patientCode;
                    if (dicPatientCode.TryGetValue(expMest.TDL_TREATMENT_ID.Value, out patientCode))
                        expMest.TDL_PATIENT_CODE = patientCode;
                }
                Inventec.Common.Logging.LogSystem.Debug("BO SUNG MA BENH NHAN ___ so dot dieu tri phai tra cuu: " + treatmentIds.Count);
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
                        else if (item.KEY == cboOddEven.Name)
                        {
                            currentOddEvenFilter = NormalizeOddEvenFilter(item.VALUE);
                            cboOddEven.EditValueChanged -= cboOddEven_EditValueChanged;
                            cboOddEven.EditValue = currentOddEvenFilter;
                            cboOddEven.EditValueChanged += cboOddEven_EditValueChanged;
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

        private void InitComboOddEven()
        {
            try
            {
                var source = new List<OddEvenFilterADO>
                {
                    new OddEvenFilterADO { Code = ODD_EVEN_FILTER__ALL, Name = "Tất cả" },
                    new OddEvenFilterADO { Code = ODD_EVEN_FILTER__EVEN, Name = "STT chẵn" },
                    new OddEvenFilterADO { Code = ODD_EVEN_FILTER__ODD, Name = "STT lẻ" }
                };

                cboOddEven.EditValueChanged -= cboOddEven_EditValueChanged;
                cboOddEven.Properties.DataSource = source;
                cboOddEven.Properties.DisplayMember = "Name";
                cboOddEven.Properties.ValueMember = "Code";
                cboOddEven.EditValue = ODD_EVEN_FILTER__ALL;
                cboOddEven.EditValueChanged += cboOddEven_EditValueChanged;

                currentOddEvenFilter = ODD_EVEN_FILTER__ALL;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboOddEven_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                currentOddEvenFilter = NormalizeOddEvenFilter(cboOddEven.EditValue as string);
                SaveOddEvenControlState();
                LoadAllTab();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveOddEvenControlState()
        {
            try
            {
                if (controlStateWorker == null)
                    controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                if (currentControlStateRDO == null)
                    currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                var existing = currentControlStateRDO
                    .FirstOrDefault(o => o.KEY == cboOddEven.Name && o.MODULE_LINK == moduleLink);
                if (existing != null)
                {
                    existing.VALUE = currentOddEvenFilter;
                }
                else
                {
                    currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = cboOddEven.Name,
                        VALUE = currentOddEvenFilter,
                        MODULE_LINK = moduleLink
                    });
                }
                controlStateWorker.SetData(currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static string NormalizeOddEvenFilter(string value)
        {
            if (value == ODD_EVEN_FILTER__EVEN) return ODD_EVEN_FILTER__EVEN;
            if (value == ODD_EVEN_FILTER__ODD) return ODD_EVEN_FILTER__ODD;
            return ODD_EVEN_FILTER__ALL;
        }

        internal List<HIS_EXP_MEST> ApplyOddEvenFilter(List<HIS_EXP_MEST> list)
        {
            try
            {
                if (list == null || list.Count == 0) return list;
                if (currentOddEvenFilter == ODD_EVEN_FILTER__ALL) return list;

                long mod = (currentOddEvenFilter == ODD_EVEN_FILTER__EVEN) ? 0 : 1;
                return list.Where(o => o != null
                                       && o.NUM_ORDER.HasValue
                                       && (o.NUM_ORDER.Value % 2) == mod).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return list;
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

                if (gvPrepareMedicine.FocusedColumn != gridColumn26 && gvPrepareMedicine.FocusedColumn != colPatientCode) return;

                //Quet vao o nay khong ra dong nao thi thu lai o ma con lai 
                if (gvPrepareMedicine.RowCount == 0)
                {
                    SwapScanCodeToOtherColumn();
                }

                if (gvPrepareMedicine.RowCount != 1) return;

                int rowHandle = gvPrepareMedicine.GetVisibleRowHandle(0);
                var one = gvPrepareMedicine.GetRow(rowHandle) as HIS_EXP_MEST;
                if (one == null) return;

                //Nho o vua quet khop de sau khi phat thuoc LoadTab3 focus lai dung o do
                isScanByPatientCode = gvPrepareMedicine.FocusedColumn == colPatientCode;
                if (currentCall != null && currentCall.ID != one.ID)
                {
                    return;
                }

                CallSpecific(one);
                //btnCall_Click(null, null);

                //Quet xong phat thuoc luon, khong phai bam nut.
                //CallSpecific khong set currentCall khi chua cau hinh quay hoac phieu thuoc quay khac, khi do khong phat.
                if (currentCall != null && currentCall.ID == one.ID)
                {
                    btnGaveMedicine_Click(null, null);
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        /// Chuyen chuoi vua quet sang o loc ma con lai (ma dieu tri &lt;-&gt; ma benh nhan).
        /// Chay 2 chieu vi con tro co the dang o o nao trong hai o.
        /// </summary>
        private void SwapScanCodeToOtherColumn()
        {
            try
            {
                DevExpress.XtraGrid.Columns.GridColumn fromColumn = gvPrepareMedicine.FocusedColumn;
                DevExpress.XtraGrid.Columns.GridColumn toColumn = (fromColumn == gridColumn26) ? colPatientCode : gridColumn26;

                string scanCode = (gvPrepareMedicine.GetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, fromColumn) ?? "").ToString().Trim();
                Inventec.Common.Logging.LogSystem.Debug("QUET MA ___ khong khop o " + fromColumn.Caption + ", thu sang o " + toColumn.Caption + ": " + scanCode);
                if (string.IsNullOrEmpty(scanCode)) return;

                gvPrepareMedicine.HideEditor();
                gvPrepareMedicine.SetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, fromColumn, null);
                gvPrepareMedicine.SetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, toColumn, scanCode);
                gvPrepareMedicine.FocusedColumn = toColumn;
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
    }
}
