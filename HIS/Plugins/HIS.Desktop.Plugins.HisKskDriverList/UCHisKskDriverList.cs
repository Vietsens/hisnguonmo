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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors;
using MOS.SDO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using MOS.KskSignData;
using DevExpress.Data;
using System.Diagnostics;
using System.Threading;
using EMR.WCF.DCO;
using Inventec.Common.SignLibrary.ServiceSign;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace HIS.Desktop.Plugins.HisKskDriverList
{
    public partial class UCHisKskDriverList : UserControlBase
    {

        #region Derlare
        string datetime;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        List<HIS_EXECUTE_ROOM> executeRoomSelecteds;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        RefeshReference refeshReference { get; set; }
        int lastRowHandle = -1;
        private bool isNotLoadWhileChangeControlStateInFirst;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private X509Certificate2 certificate;
        private string SerialNumber;
        #endregion
        #region ConstructorLoad
        public UCHisKskDriverList(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.currentModule = module;
        }
        private void UCHisKskDriverList_Load(object sender, EventArgs e)
        {
            try
            {
                timer.Interval = 100;
                timer.Tick += Timer_Tick;
                SetCaptionByLanguageKey();
                SetDefaultControl();
                InitComboExecuteRoomCheck();
                InitComboExecuteRoom();
                InitControlState();

                FillDataToGrid();
                txtKeyWord.Focus();
                LoadDicRefresh();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public string AppFilePathSignService()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "EMR.SignProcessor"), "EMR.SignProcessor.exe");
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }
        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }
        internal bool VerifyServiceSignProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.1");
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("EMR.SignProcessor"))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.2");
                        valid = true;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.3");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exeSignPath), exeSignPath));
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {
                            Process.Start(startInfo);
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.4");
                            Thread.Sleep(500);
                            valid = true;
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.5");
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {

            try
            {
                timer.Stop();
                SerialNumber = null;
                if (chkSignFileCertUtil.Checked)
                {
                    if (VerifyServiceSignProcessorIsRunning())
                    {
                        WcfSignDCO wcfSignDCO = new WcfSignDCO();
                        wcfSignDCO.HwndParent = this.ParentForm.Handle;
                        string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                        SignProcessorClient signProcessorClient = new SignProcessorClient();
                        var wcfSignResultDCO = signProcessorClient.GetSerialNumber(jsonData);  //EDIT
                        if (wcfSignResultDCO != null)
                        {
                            SerialNumber = wcfSignResultDCO.OutputFile;
                        }
                    }
                    if (string.IsNullOrEmpty(SerialNumber))
                    {
                        chkSignFileCertUtil.Checked = false;
                        XtraMessageBox.Show("Không lấy được thông tin chứng thư hoặc chứng thư không hợp lệ", "Thông báo");
                    }
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignFileCertUtil.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = SerialNumber;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignFileCertUtil.Name;
                    csAddOrUpdate.VALUE = SerialNumber;
                    csAddOrUpdate.MODULE_LINK = this.currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        #endregion
        #region Private Method
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisKskDriverList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisKskDriverList.UCHisKskDriverList).Assembly);
                this.navBarGroup1.Caption = Inventec.Common.Resource.Get.Value("navBarGroup1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup2.Caption = Inventec.Common.Resource.Get.Value("navBarGroup2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup3.Caption = Inventec.Common.Resource.Get.Value("navBarGroup3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKskDriverCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("txtKskDriverCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtServiceReqCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("txtServiceReqCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("txtPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadGridData(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadGridData, param, pageSize, gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void LoadGridData(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<V_HIS_KSK_DRIVER>> apiResult = null;
                HisKskDriverViewFilter filter = new HisKskDriverViewFilter();
                SetFilter(ref filter);
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_KSK_DRIVER>>("api/HisKskDriver/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControl1.DataSource = data;
                    }
                    else
                    {
                        gridControl1.DataSource = null;

                    }
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                }
                gridView1.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }
        private void SetFilter(ref HisKskDriverViewFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtServiceReqCode.Text))
                {
                    string code = txtServiceReqCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtServiceReqCode.Text = code;
                    }
                    filter.SERVICE_REQ_CODE__EXACT = txtServiceReqCode.Text;
                }
                else if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = txtTreatmentCode.Text;

                }
                else if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = txtPatientCode.Text;
                }
                else if (!string.IsNullOrEmpty(txtKskDriverCode.Text))
                {
                    filter.KSK_DRIVER_CODE__EXACT = txtKskDriverCode.Text;
                }
                else
                {
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.CONCLUSION_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.CONCLUSION_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                    if (cboSYNC_RESULT_TYPE.EditValue != null)
                    {
                        switch (cboSYNC_RESULT_TYPE.SelectedIndex)
                        {
                            case 0:
                                filter.SYNC_RESULT_TYPE = null;
                                break;
                            case 1:
                                filter.SYNC_RESULT_TYPE = 1;
                                break;
                            case 2:
                                filter.SYNC_RESULT_TYPE = 2;
                                break;
                            case 3:
                                filter.SYNC_RESULT_TYPE = 3;
                                break;
                            default:
                                filter.SYNC_RESULT_TYPE = null;
                                break;
                        }
                    }
                    if (this.executeRoomSelecteds != null && this.executeRoomSelecteds.Count() > 0)
                    {
                        filter.EXECUTE_ROOM_IDs = this.executeRoomSelecteds.Select(o => o.ROOM_ID).Distinct().ToList();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultControl()
        {
            try
            {

                dtCreateTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartMonth() ?? 0));
                dtCreateTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                txtKeyWord.Text = "";
                txtKskDriverCode.Text = "";
                txtPatientCode.Text = "";
                txtTreatmentCode.Text = "";
                txtServiceReqCode.Text = "";
                cboSYNC_RESULT_TYPE.SelectedIndex = 0;
                btnDongBo.Enabled = false;
                GridCheckMarksSelection gridCheckMark = cboEXECUTE_ROOM_NAME.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboEXECUTE_ROOM_NAME.Properties.View);
                }
                cboEXECUTE_ROOM_NAME.Focus();
                txtKeyWord.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ShowHisKskDriverCreate(V_HIS_KSK_DRIVER data)
        {
            try
            {

                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisKskDriverCreate").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisKskDriverCreate'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS_KSK_DRIVER dt = new HIS_KSK_DRIVER();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_KSK_DRIVER>(dt, data);
                    listArgs.Add(dt);
                    listArgs.Add((HIS.Desktop.Common.RefeshReference)FillDataToGrid);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }
                    WaitingManager.Hide();
                    ((Form)extenceInstance).ShowDialog();

                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.currentModule.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkSignFileCertUtil.Name)
                        {
                            SerialNumber = item.VALUE;

                            chkSignFileCertUtil.Checked = !String.IsNullOrWhiteSpace(SerialNumber);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }
        #endregion
        #region EvenGridView
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_KSK_DRIVER data = (V_HIS_KSK_DRIVER)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging.pagingGrid.CurrentPage - 1) * (ucPaging.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                    {
                        try
                        {
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao TDL_PATIENT_DOB", ex);
                        }
                    }

                    else if (e.Column.FieldName == "SYNC_RESULT_TYPE_STR")
                    {
                        try
                        {
                            if (data.SYNC_RESULT_TYPE == 0)
                            {
                                e.Value = "";
                            }
                            else if (data.SYNC_RESULT_TYPE == 1)
                            {
                                e.Value = "Chưa đồng bộ";
                            }
                            else if (data.SYNC_RESULT_TYPE == 2)
                            {
                                e.Value = "Đã đồng bộ";
                            }
                            else if (data.SYNC_RESULT_TYPE == 4)
                            {
                                e.Value = "Có chỉnh sửa";
                            }
                            else
                            {
                                e.Value = "Đồng bộ lỗi";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao SYNC_RESULT_TYPE", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CONCLUSION_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.CONCLUSION_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CONCLUSION_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CONCLUSION_STR")
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(data.CONCLUSION))
                            {
                                if (data.CONCLUSION == "A0-1")
                                    e.Value = "Đủ điều kiện sức khỏe lái xe";
                                else if (data.CONCLUSION == "A0-2")
                                    e.Value = "Không đủ điều kiện sức khỏe lái xe";
                                else
                                    e.Value = "Đạt tiêu chuẩn sức khỏe lái xe, nhưng yêu cầu khám lại";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CONCLUSION", ex);

                        }
                    }
                    else if (e.Column.FieldName == "SYNC_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.SYNC_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao SYNC_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "DRUG_TYPE_STR")
                    {
                        try
                        {
                            if (data.DRUG_TYPE != null)
                            {
                                if (data.DRUG_TYPE == 1)
                                {
                                    e.Value = "Âm tính";
                                }
                                else
                                {
                                    e.Value = "Dương tính";
                                }
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DRUG_TYPE", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CONCENTRATION_STR")
                    {
                        try
                        {
                            string concentrationType = "";

                            if (data.CONCENTRATION_TYPE != null)
                            {
                                if (data.CONCENTRATION_TYPE == 1)
                                    concentrationType = " mg/1 lít khí thở";
                                else
                                {
                                    concentrationType = " mg/100ml máu";
                                }
                            }
                            if (data.CONCENTRATION != null)
                            {
                                e.Value = data.CONCENTRATION.ToString() + concentrationType;
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CONCENTRATION_TYPE", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #region EventClickPress
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnRefresh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnRefresh.Focus();
                    btnRefresh_Click(null, null);
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
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtKskDriverCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
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
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        #region EvenBarManager
        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region EventItemButton
        private void repositoryItemButtonEdit_EDIT_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_KSK_DRIVER)gridView1.GetFocusedRow();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));
                if (row != null)
                {
                    if (row.SYNC_RESULT_TYPE == 2)
                    {
                        Inventec.Common.Logging.LogSystem.Error("______Đồng bộ");
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Hồ sơ đã đồng bộ bản có muốn chỉnh sửa hay không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ShowHisKskDriverCreate(row);
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Error("______Không Đồng bộ");
                        ShowHisKskDriverCreate(row);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_ASYN_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool rs = false;
                List<KskDriverSyncSDO> listKskDriveSync = new List<KskDriverSyncSDO>();
                var row = (V_HIS_KSK_DRIVER)gridView1.GetFocusedRow();
                WaitingManager.Show();
                if (row != null)
                {
                    if (chkSignFileCertUtil.Checked)
                    {
                        if (certificate == null && !String.IsNullOrWhiteSpace(SerialNumber))
                        {
                            certificate = Inventec.Common.SignFile.CertUtil.GetBySerial(SerialNumber, requirePrivateKey: true, validOnly: false);
                            if (certificate == null)
                            {
                                chkSignFileCertUtil.Checked = false;
                                if (MessageBox.Show("Không lấy được thông tin chứng thư hoặc chứng thư không hợp lệ. Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                {
                                    return;
                                }
                            }
                        }

                        KskDataProcess data = new KskDataProcess();
                        KskDriverSyncSDO sdo = new KskDriverSyncSDO();
                        sdo.KskDriveId = row.ID;
                        sdo.SyncData = data.MakeData(row, certificate, (string xmlData, string element, string serialNumber) =>
                        {
                            if (VerifyServiceSignProcessorIsRunning())
                            {
                                SignProcessorClient signProcessorClient = new SignProcessorClient();
                                return signProcessorClient.StringBase64SignXml(xmlData, element, serialNumber);
                            }
                            else
                                return null;
                        });
                        listKskDriveSync.Add(sdo);
                    }
                    else
                    {
                        KskDriverSyncSDO sdo = new KskDriverSyncSDO();
                        sdo.KskDriveId = row.ID;
                        listKskDriveSync.Add(sdo);
                    }
                    if (listKskDriveSync != null && listKskDriveSync.Count > 0)
                    {
                        rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisKskDriver/Sync", ApiConsumer.ApiConsumers.MosConsumer, listKskDriveSync, SessionManager.ActionLostToken, param);
                    }
                    if (rs)
                    {
                        FillDataToGrid();
                        WaitingManager.Hide();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, rs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region InitComboExecuteRoom
        private void cboEXECUTE_ROOM_NAME_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.executeRoomSelecteds != null && this.executeRoomSelecteds.Count > 0)
                {
                    foreach (var item in this.executeRoomSelecteds)
                    {
                        roomName += item.EXECUTE_ROOM_NAME + ", ";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }
        private List<HIS_EXECUTE_ROOM> GetExecuteRoom()
        {
            List<HIS_EXECUTE_ROOM> list = new List<HIS_EXECUTE_ROOM>();
            try
            {
                CommonParam param = new CommonParam();
                HisExecuteRoomFilter filter = new HisExecuteRoomFilter();
                filter.IS_ACTIVE = 1;
                filter.IS_EXAM = true;
                list = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
            return list;
        }
        private void InitComboExecuteRoom()
        {
            try
            {
                cboEXECUTE_ROOM_NAME.Properties.DataSource = GetExecuteRoom();
                cboEXECUTE_ROOM_NAME.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                cboEXECUTE_ROOM_NAME.Properties.ValueMember = "ROOM_ID";
                DevExpress.XtraGrid.Columns.GridColumn column = cboEXECUTE_ROOM_NAME.Properties.View.Columns.AddField("EXECUTE_ROOM_NAME");
                column.VisibleIndex = 1;
                column.Width = 200;
                column.Caption = "Tất cả";
                cboEXECUTE_ROOM_NAME.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboEXECUTE_ROOM_NAME.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboExecuteRoomCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboEXECUTE_ROOM_NAME.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboEXECUTE_ROOM_NAME.Properties.Tag = gridCheck;
                cboEXECUTE_ROOM_NAME.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboEXECUTE_ROOM_NAME.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboEXECUTE_ROOM_NAME.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Event_Check(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                executeRoomSelecteds = new List<HIS_EXECUTE_ROOM>();
                if (gridCheckMark != null)
                {
                    List<HIS_EXECUTE_ROOM> erSelectedNews = new List<HIS_EXECUTE_ROOM>();
                    foreach (HIS_EXECUTE_ROOM er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.EXECUTE_ROOM_NAME);
                            erSelectedNews.Add(er);
                        }
                    }
                    this.executeRoomSelecteds = new List<HIS_EXECUTE_ROOM>();
                    this.executeRoomSelecteds.AddRange(erSelectedNews);
                }
                this.cboEXECUTE_ROOM_NAME.Text = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion



        private void LoadDicRefresh()
        {
            try
            {
                if (GlobalVariables.DicRefreshData == null)
                {
                    GlobalVariables.DicRefreshData = new Dictionary<string, RefeshReference>();
                }
                GlobalVariables.DicRefreshData.Add(currentModule.RoomId.ToString(), (HIS.Desktop.Common.RefeshReference)FillDataToGrid);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalVariables.DicRefreshData),
                    GlobalVariables.DicRefreshData));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (gridView1.GetSelectedRows().Count() > 0)
                {
                    btnDongBo.Enabled = true;
                }
                else
                {
                    btnDongBo.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSignFileCertUtil_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                timer.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private void btnDongBo_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool rs = false;
                List<KskDriverSyncSDO> listKskDriveSync = new List<KskDriverSyncSDO>();
                WaitingManager.Show();
                var rowHandles = gridView1.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    if (chkSignFileCertUtil.Checked)
                    {
                        if (certificate == null && !String.IsNullOrWhiteSpace(SerialNumber))
                        {
                            certificate = Inventec.Common.SignFile.CertUtil.GetBySerial(SerialNumber, requirePrivateKey: true, validOnly: false);
                            if (certificate == null)
                            {
                                chkSignFileCertUtil.Checked = false;
                                if (MessageBox.Show("Không lấy được thông tin chứng thư hoặc chứng thư không hợp lệ. Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                {
                                    return;
                                }
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug(SerialNumber + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => certificate), certificate));
                        KskDataProcess data = new KskDataProcess();
                        foreach (var i in rowHandles)
                        {
                            var row = (V_HIS_KSK_DRIVER)gridView1.GetRow(i);
                            if (row != null)
                            {
                                KskDriverSyncSDO sdo = new KskDriverSyncSDO();
                                sdo.KskDriveId = row.ID;
                                sdo.SyncData = data.MakeData(row, certificate, (string xmlData, string element, string serialNumber) =>
                                {
                                    if (VerifyServiceSignProcessorIsRunning())
                                    {
                                        SignProcessorClient signProcessorClient = new SignProcessorClient();
                                        return signProcessorClient.StringBase64SignXml(xmlData, element, serialNumber);
                                    }
                                    else
                                        return null;
                                });
                                listKskDriveSync.Add(sdo);
                            }
                        }
                    }
                    else
                    {
                        foreach (var i in rowHandles)
                        {
                            var row = (V_HIS_KSK_DRIVER)gridView1.GetRow(i);
                            if (row != null)
                            {
                                KskDriverSyncSDO sdo = new KskDriverSyncSDO();
                                sdo.KskDriveId = row.ID;
                                listKskDriveSync.Add(sdo);
                            }
                        }
                    }
                    if (listKskDriveSync != null && listKskDriveSync.Count > 0)
                    {
                        rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisKskDriver/Sync", ApiConsumer.ApiConsumers.MosConsumer, listKskDriveSync, SessionManager.ActionLostToken, param);
                    }
                    if (rs)
                    {
                        FillDataToGrid();
                    }
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, rs);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }        

    }
}
