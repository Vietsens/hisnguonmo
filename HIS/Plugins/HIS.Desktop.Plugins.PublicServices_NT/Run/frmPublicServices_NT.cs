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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PublicServices_NT.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PublicServices_NT
{
    public partial class frmPublicServices_NT : HIS.Desktop.Utility.FormBase
    {
        private long _treatmentId;
        private L_HIS_TREATMENT_BED_ROOM _TreatmentBedRoom { get; set; }
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private List<PatientTypeSelectADO> patientTypeSelectADOs;
        private List<PatientTypeSelectADO> patientTypeHasSelecteds;
        private int congKhaiDichVu_DaySize;
        private bool isShowPatientList = false;
        private List<MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM> treatmentBedRoomList;
        HIS_DEPARTMENT department = null;

        Dictionary<long, HIS_EXP_MEST> dicExpMest = new Dictionary<long, HIS_EXP_MEST>();
        Dictionary<long, HIS_SERVICE_REQ> dicServiceReq = new Dictionary<long, HIS_SERVICE_REQ>();

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.PublicServices_NT";

        private const int MaxReqFilter = 100;

        public frmPublicServices_NT()
        {
            InitializeComponent();
        }

        public frmPublicServices_NT(Inventec.Desktop.Common.Modules.Module currentModule,
            L_HIS_TREATMENT_BED_ROOM curentTreatment, bool isShowPatientList)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this._TreatmentBedRoom = curentTreatment;
            this._treatmentId = curentTreatment.TREATMENT_ID;
            this.isShowPatientList = isShowPatientList;
        }

        public frmPublicServices_NT(Inventec.Desktop.Common.Modules.Module currentModule,
           V_HIS_TREATMENT_4 curentTreatment)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this._treatmentId = curentTreatment.ID;
            LoadTreatmentBedRoom();
        }

        private void frmPublicServices_NT_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.isShowPatientList)
                {
                    layoutControlItem_btnSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem_GridPatient.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem_txtKeyword.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem_GridControlPatient.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.Width = 880;
                    MOS.Filter.HisTreatmentBedRoomLViewFilter filter = new HisTreatmentBedRoomLViewFilter();
                    filter.BED_ROOM_ID = _TreatmentBedRoom.BED_ROOM_ID;
                    filter.IS_IN_ROOM = true;
                    treatmentBedRoomList = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetLView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);

                    if (treatmentBedRoomList != null && treatmentBedRoomList.Count() > 0)
                    {
                        var index = treatmentBedRoomList.FindIndex(x => x.TREATMENT_ID == this._treatmentId);
                        if (index > -1)
                        {
                            var item = treatmentBedRoomList[index];
                            treatmentBedRoomList[index] = treatmentBedRoomList[0];
                            treatmentBedRoomList[0] = item;
                        }                        
                    }

                    gridControlPatient.DataSource = treatmentBedRoomList != null && treatmentBedRoomList.Count > 0 ? treatmentBedRoomList.Where(o => !o.REMOVE_TIME.HasValue).ToList() : null;
                    layoutControlItem_btnNextPatient.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutControlItem_btnSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem_GridPatient.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem_txtKeyword.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem_GridControlPatient.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.Width = 440;
                    layoutControlItem_btnNextPatient.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;

                }
                SetCaptionByLanguageKey();
                InitControlState();
                rdoAllDay.Checked = true;
                dtFrom.EditValue = DateTime.Now;
                dtTo.EditValue = DateTime.Now;
                InitGridPatientType();
                congKhaiDichVu_DaySize = HisConfigs.Get<int>(HisConfigCFG.CONFIG_KEY__CONG_KHAI_DICH_VU__DAY_SIZE);
                congKhaiDichVu_DaySize = (congKhaiDichVu_DaySize == 0 ? 10 : congKhaiDichVu_DaySize);

                var departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                this.department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == departmentId);
                if (this.department != null)
                {
                    rdoRequestDepartment__Current.Text = department.DEPARTMENT_NAME;
                    rdoRequestDepartment__Current.CheckState = CheckState.Checked;
                }
                WaitingManager.Hide();
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
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(currentModule.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkLayCaThuocVTNgoaiKho.Name)
                        {
                            chkLayCaThuocVTNgoaiKho.Checked = item.VALUE == "1";
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
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

        private void InitGridPatientType()
        {
            try
            {
                this.patientTypeSelectADOs = new List<PatientTypeSelectADO>();

                CommonParam param = new CommonParam();
                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.TREATMENT_ID = this._treatmentId;
                var pts = BackendDataWorker.Get<HIS_PATIENT_TYPE>().ToDictionary(o => o.ID, o => o);
                var ssResults = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, param);
                if (ssResults != null && ssResults.Count > 0)
                {
                    var grPatientType = ssResults.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                    foreach (var paty in grPatientType)
                    {
                        if (pts.ContainsKey(paty.Key))
                        {
                            PatientTypeSelectADO ado = new PatientTypeSelectADO()
                            {
                                ID = pts[paty.Key].ID,
                                PATIENT_TYPE_CODE = pts[paty.Key].PATIENT_TYPE_CODE,
                                PATIENT_TYPE_NAME = pts[paty.Key].PATIENT_TYPE_NAME,
                            };

                            this.patientTypeSelectADOs.Add(ado);
                        }
                        else
                        {
                            PatientTypeSelectADO ado = new PatientTypeSelectADO()
                            {
                                ID = paty.Key,
                                PATIENT_TYPE_CODE = "KH",
                                PATIENT_TYPE_NAME = "Khác",
                            };

                            this.patientTypeSelectADOs.Add(ado);
                        }
                    }
                }

                gridViewPatientType.BeginUpdate();
                gridViewPatientType.GridControl.DataSource = this.patientTypeSelectADOs;
                gridViewPatientType.EndUpdate();

                gridViewPatientType.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatmentBedRoom()
        {
            try
            {
                MOS.Filter.HisTreatmentBedRoomLViewFilter filter = new HisTreatmentBedRoomLViewFilter();
                filter.TREATMENT_ID = this._treatmentId;

                var datas = new BackendAdapter(null).Get<List<L_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetLView", ApiConsumers.MosConsumer, filter, null);
                if (datas != null && datas.Count > 0)
                {
                    this._TreatmentBedRoom = datas.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<PatientTypeSelectADO> GetPatientTypeGridSelected()
        {
            List<PatientTypeSelectADO> ptSelecteds = new List<PatientTypeSelectADO>();
            try
            {
                int[] selectRows = gridViewPatientType.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var ptSelectADO = (PatientTypeSelectADO)gridViewPatientType.GetRow(selectRows[i]);
                        ptSelecteds.Add(ptSelectADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ptSelecteds;
        }

        private bool GetAllSereServV2()
        {
            bool result = true;
            try
            {
                this._Datas = new List<Service_NT_ADO>();
                dicExpMest = new Dictionary<long, HIS_EXP_MEST>();
                dicServiceReq = new Dictionary<long, HIS_SERVICE_REQ>();
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                //1.Get ServiceReq là đơn nt và tt, theo khoa yc
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = this._treatmentId;
                if (rdoRequestDepartment__Current.Checked)
                {
                    serviceReqFilter.REQUEST_DEPARTMENT_ID = this.department.ID;
                }

                if (!rdoAllDay.Checked)
                {
                    if (dtFrom.EditValue != null && dtFrom.DateTime != DateTime.MinValue)
                    {
                        serviceReqFilter.USE_TIME_OR_INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtFrom.EditValue ?? "0").ToString()).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtTo.EditValue != null && dtTo.DateTime != DateTime.MinValue)
                    {
                        serviceReqFilter.USE_TIME_OR_INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtTo.EditValue ?? "0").ToString()).ToString("yyyyMMdd") + "235959");
                    }
                }

                var _currentServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(
                    HisRequestUriStore.HIS_SERVICE_REQ_GET,
                    ApiConsumers.MosConsumer,
                    serviceReqFilter,
                    param);

                List<long> _expMestIds = new List<long>();
                List<long> _expMestServiceReqIds = new List<long>();
                List<long> _serviceReqId_T_VTs = new List<long>();
                List<long> _serviceReqId_SVs = new List<long>();
                if (_currentServiceReqs != null && _currentServiceReqs.Count > 0)
                {
                    foreach (var itemSer in _currentServiceReqs)
                    {
                        if (!dicServiceReq.ContainsKey(itemSer.ID))
                        {
                            dicServiceReq[itemSer.ID] = new HIS_SERVICE_REQ();
                            dicServiceReq[itemSer.ID] = itemSer;
                        }
                    }

                    _serviceReqId_SVs = _currentServiceReqs.Where(p =>
                        p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                        && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM
                        && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                        && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                        ).Select(p => p.ID).ToList();

                    _serviceReqId_T_VTs = _currentServiceReqs.Where(p => !_serviceReqId_SVs.Exists(e => e == p.ID)).Select(p => p.ID).ToList();

                    if (_serviceReqId_SVs != null && _serviceReqId_SVs.Count > 0)
                    {
                        MOS.Filter.HisSereServViewFilter _ssFiler = new HisSereServViewFilter();
                        _ssFiler.TREATMENT_ID = this._treatmentId;
                        //_ssFiler.SERVICE_REQ_IDs = _serviceReqId_SVs;
                        _ssFiler.PATIENT_TYPE_IDs = this.patientTypeHasSelecteds.Select(o => o.ID).ToList();
                        if (!chkHaoPhi.Checked)
                        {
                            _ssFiler.IS_EXPEND = false;
                        }
                        var _SereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, _ssFiler, param);
                        if (_SereServs != null && _SereServs.Count > 0)
                        {
                            _SereServs = _SereServs.Where(o => _serviceReqId_SVs.Contains(o.SERVICE_REQ_ID ?? 0)).ToList();

                            List<HIS_SERE_SERV_EXT> _SereServsExt = GetSsExtBySsId(_SereServs.Select(o => o.ID).ToList());

                            foreach (var item in _SereServs)
                            {
                                //Review
                                Service_NT_ADO ado = new Service_NT_ADO();
                                ado.SERVICE_NAME = item.TDL_SERVICE_NAME;
                                //ado.INTRUCTION_DATE = item.TDL_INTRUCTION_DATE;
                                //Dangth
                                ado.INTRUCTION_DATE = dicServiceReq.ContainsKey(item.SERVICE_REQ_ID ?? 0)
                                ? (dicServiceReq[item.SERVICE_REQ_ID ?? 0].USE_TIME ?? item.TDL_INTRUCTION_DATE)
                                : item.TDL_INTRUCTION_DATE;
                                //Dangth
                                ado.SERVICE_TYPE_ID = item.TDL_SERVICE_TYPE_ID;
                                ado.SERVICE_ID = item.SERVICE_ID;
                                ado.PRICE = item.PRICE;
                                ado.AMOUNT = item.AMOUNT;
                                ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                ado.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID;

                                if (_SereServsExt != null && _SereServsExt.Count > 0)
                                {
                                    var InstructionNote = _SereServsExt.FirstOrDefault(o => o.SERE_SERV_ID == item.ID);
                                    ado.INSTRUCTION_NOTE = InstructionNote != null ? InstructionNote.INSTRUCTION_NOTE : "";
                                }

                                //xuandv  --- #17301
                                if (rdoRequestDepartment__Current.Checked)
                                {
                                    if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                                        && item.TDL_EXECUTE_DEPARTMENT_ID == this.department.ID)
                                    {
                                        this._Datas.Add(ado);
                                    }
                                    else if (item.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                                        && item.TDL_REQUEST_DEPARTMENT_ID == this.department.ID)
                                    {
                                        this._Datas.Add(ado);
                                    }
                                }
                                else
                                {
                                    this._Datas.Add(ado);
                                }
                            }
                        }
                    }

                    if (_serviceReqId_T_VTs != null && _serviceReqId_T_VTs.Count > 0)
                    {
                        int skip = 0;
                        while (_serviceReqId_T_VTs.Count - skip > 0)
                        {
                            var listIds = _serviceReqId_T_VTs.Skip(skip).Take(MaxReqFilter).ToList();
                            skip += MaxReqFilter;

                            MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                            expMestFilter.SERVICE_REQ_IDs = listIds;
                            var _currentExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                            if (_currentExpMests != null && _currentExpMests.Count > 0)
                            {
                                List<long> expMestIds = new List<long>();
                                if (rdoRequestDepartment__Current.Checked)
                                {
                                    expMestIds = _currentExpMests.Where(p => p.REQ_DEPARTMENT_ID == this.department.ID).Select(p => p.ID).ToList();
                                }
                                else
                                {
                                    expMestIds = _currentExpMests.Select(p => p.ID).ToList();
                                }

                                if (expMestIds != null && expMestIds.Count > 0)
                                {
                                    _expMestIds.AddRange(expMestIds);
                                }

                                foreach (var item in _currentExpMests)
                                {
                                    if (!dicExpMest.ContainsKey(item.ID))
                                    {
                                        dicExpMest[item.ID] = new HIS_EXP_MEST();
                                        dicExpMest[item.ID] = item;
                                    }
                                }
                            }
                        }
                    }

                    _expMestServiceReqIds.AddRange(_currentServiceReqs.Select(o => o.ID).ToList());


                    WaitingManager.Hide();
                }
                else
                {
                    WaitingManager.Hide();
                    result = false;
                    return result;
                }

                CreateThreadLoadData(_expMestIds);
                if (this.chkLayCaThuocVTNgoaiKho.Checked)
                {
                    CreateThreadLoadDataNgoaiKho(_expMestServiceReqIds);

                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
                result = false;
            }
            return result;
        }

        private List<HIS_SERE_SERV_EXT> GetSsExtBySsId(List<long> list)
        {
            List<HIS_SERE_SERV_EXT> result = null;
            try
            {
                if (list != null && list.Count > 0)
                {
                    result = new List<HIS_SERE_SERV_EXT>();
                    int skip = 0;
                    while (list.Count - skip > 0)
                    {
                        var listIds = list.Skip(skip).Take(MaxReqFilter).ToList();
                        skip += MaxReqFilter;

                        CommonParam param = new CommonParam();
                        HisSereServExtFilter SSExtfilter = new HisSereServExtFilter();
                        SSExtfilter.SERE_SERV_IDs = listIds;
                        var apiResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, SSExtfilter, param);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            result.AddRange(apiResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void btnPublicByDate_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(PrintType.PHIEU_CONG_KHAI_DICH_VU);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long TimeNumberToDateNumber(long? timeNumber)
        {
            long result = 0;
            try
            {
                string dateNumberString = timeNumber.ToString().Substring(0, 8);
                result = Inventec.Common.TypeConvert.Parse.ToInt64(dateNumberString);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void barButtonItemTao_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPublic.Focus();
                btnPublicByDate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatientType_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                //PatientTypeSelectADO patientTypeSelectADO = this.gridViewPatientType.GetFocusedRow() as PatientTypeSelectADO;
                //if (patientTypeSelectADO != null)
                //{

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoRequestDepartment__Current_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoRequestDepartment__Current.Checked)
                {
                    rdoRequestDepartment__All.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoRequestDepartment__All_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoRequestDepartment__All.Checked)
                {
                    rdoRequestDepartment__Current.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM> dataSource = null;
                if (!String.IsNullOrWhiteSpace(txtKeyword.Text) && treatmentBedRoomList != null && treatmentBedRoomList.Count > 0)
                {
                    string keyword = txtKeyword.Text.Trim().ToLower();
                    dataSource = treatmentBedRoomList
                        .Where(o => !o.REMOVE_TIME.HasValue &&
                            ((!String.IsNullOrWhiteSpace(o.TDL_PATIENT_CODE) && o.TDL_PATIENT_CODE.ToLower().Contains(keyword))
                                || (!String.IsNullOrWhiteSpace(o.TREATMENT_CODE) && o.TREATMENT_CODE.ToLower().Contains(keyword))
                                || (!String.IsNullOrWhiteSpace(o.TDL_PATIENT_NAME) && o.TDL_PATIENT_NAME.ToLower().Contains(keyword)))
                        ).ToList();
                }
                else
                    dataSource = treatmentBedRoomList;

                gridViewPatient.BeginDataUpdate();
                gridControlPatient.DataSource = null;
                gridControlPatient.DataSource = dataSource;
                gridViewPatient.EndDataUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void btnNextPatient_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewPatient.MoveNext();
                var patientFocus = (L_HIS_TREATMENT_BED_ROOM)gridViewPatient.GetFocusedRow();
                this._treatmentId = patientFocus.TREATMENT_ID;
                this._TreatmentBedRoom = patientFocus;
                InitGridPatientType();
                btnPublicByDate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnNextPatient_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnNextPatient_Click(null, null);
        }

        private void txtFocusKeyword_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoAllDay_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoAllDay.Checked)
                {
                    dtFrom.Enabled = false;
                    dtTo.Enabled = false;
                }
                else
                {
                    dtFrom.Enabled = true;
                    dtTo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatient_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
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
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPatient_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var focus = (L_HIS_TREATMENT_BED_ROOM)gridViewPatient.GetFocusedRow();
                if (focus != null)
                {
                    this._treatmentId = focus.TREATMENT_ID;
                    this._TreatmentBedRoom = focus;
                    InitGridPatientType();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkLayCaThuocVTNgoaiKho_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkLayCaThuocVTNgoaiKho.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkLayCaThuocVTNgoaiKho.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkLayCaThuocVTNgoaiKho.Name;
                    csAddOrUpdate.VALUE = (chkLayCaThuocVTNgoaiKho.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
