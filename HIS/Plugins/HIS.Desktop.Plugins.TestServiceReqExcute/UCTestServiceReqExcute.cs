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
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using MOS.TDO;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Common.Controls.EditorLoader;
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors.Repository;
using MOS.SDO;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Globalization;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Plugins.TestServiceReqExcute.ADO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils.Drawing;
using HIS.Desktop.Utility;
using Newtonsoft.Json;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.TestServiceReqExcute.Resources;

namespace HIS.Desktop.Plugins.TestServiceReqExcute
{
    public partial class UCTestServiceReqExcute : HIS.Desktop.Utility.UserControlBase
    {
        internal V_HIS_SERVICE_REQ currentServiceReq;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        internal List<HIS_SERE_SERV> lstSereServ { get; set; }
        internal List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders { get; set; }
        internal List<ADO.HisSereServTeinSDO> lstHisSereServTeinSDO { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_TEIN> lstSereServTein { get; set; }
        internal List<ADO.HisSereServTeinSDO> HisSereServTeinSDOs { get; set; }
        internal List<ACS_USER> lstAcsUser = new List<ACS_USER>();
        internal bool _IsKeyPrintNow = false;
        internal bool _IsPreiew = false;
        internal bool Is_ALLOW_FINISH = true;
        int positionHandle = -1;

        internal List<HIS_SERVICE_MACHINE> _ServiceMachines { get; set; }
        internal List<HIS_MACHINE> _Machines { get; set; }
        NumberStyles style;
        long genderId = 0;
        List<V_HIS_TEST_INDEX_RANGE> testIndexRangeAll;

        bool check;
        bool checkNoiTru;
        bool checkNguoiChiDinh = false;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentBySessionControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.TestServiceReqExcute";

        public UCTestServiceReqExcute()
        {
            InitializeComponent();

        }

        public UCTestServiceReqExcute(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_SERVICE_REQ currentServiceReq)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.currentServiceReq = currentServiceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCTestServiceReqExcute_Load(object sender, EventArgs e)
        {
            try
            {
                AppConfigKeys.GetConfigKey();
                style = NumberStyles.Any;
                genderId = LoadGenderId();
                this.testIndexRangeAll = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>();
                SetCaptionByLanguageKey();
                LoadUser();
                InitComboUser(cboUserAssign);
                InitComboUserSampler();
                SetDefaultlabel();
                LoadDataMachine();
                LoadDataToGrid();
                SetEnableButtonKeDon();
                InitControlState();
                ValidControl();
                //RegisterTimer(currentModule.ModuleLink, "timerMachine", timer1.Interval, timer1_Tick);
                //StartTimer(currentModule.ModuleLink, "timerMachine");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                if ((AppConfigKeys.HisServiceReqSampleInfoOption == "1" && this.lstSereServ.Count > 0 && this.lstSereServ.Exists(o => o.PATIENT_TYPE_ID == 1)) || AppConfigKeys.HisServiceReqSampleInfoOption == "2")
                {
                    lciSampler.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationSingleControl(cboSampler, dxValidationProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboUser(GridLookUpEdit grd)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 50, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "ID", columnInfos, false, 150);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(grd, lstAcsUser, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboUserSampler()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 50, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "ID", columnInfos, false, 150);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboSampler, lstAcsUser, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultlabel()
        {
            try
            {
                if (this.currentServiceReq != null)
                {
                    var roomName = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentServiceReq.REQUEST_ROOM_ID);
                    if (roomName != null)
                    {
                        lblRoomId.Text = roomName.ROOM_NAME;
                    }
                    else
                        lblRoomId.Text = "";

                    var DepartmentName = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.currentServiceReq.REQUEST_DEPARTMENT_ID);
                    if (DepartmentName != null)
                    {
                        lblDepartmentId.Text = DepartmentName.DEPARTMENT_NAME;
                    }
                    else
                        lblDepartmentId.Text = "";
                    lblICDName.Text = this.currentServiceReq.ICD_NAME;
                    CommonParam param = new CommonParam();
                    HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                    filter.ID = currentServiceReq.ID;
                    var data = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (data.FirstOrDefault().SAMPLE_TIME != null)
                    {
                        dtTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.FirstOrDefault().SAMPLE_TIME ?? 0) ?? DateTime.Now;
                    }
                    if (data.FirstOrDefault().SERVICE_REQ_STT_ID == 3)
                    {
                        checkNguoiChiDinh = true;

                        var userAssign = lstAcsUser.FirstOrDefault(o => o.LOGINNAME == data.FirstOrDefault().EXECUTE_LOGINNAME && o.USERNAME == data.FirstOrDefault().EXECUTE_USERNAME);
                        if (userAssign != null)
                        {
                            cboUserAssign.EditValue = userAssign.ID;
                            txtUserAssign.Text = userAssign.LOGINNAME;
                        }
                    }
                    if (!string.IsNullOrEmpty(data.FirstOrDefault().SAMPLER_LOGINNAME) && !string.IsNullOrEmpty(data.FirstOrDefault().SAMPLER_USERNAME))
                    {
                        var userSampler = lstAcsUser.FirstOrDefault(o => o.LOGINNAME == data.FirstOrDefault().SAMPLER_LOGINNAME && o.USERNAME == data.FirstOrDefault().SAMPLER_USERNAME);
                        if (userSampler != null)
                        {
                            cboSampler.EditValue = userSampler.ID;
                            txtSampler.Text = userSampler.LOGINNAME;
                        }
                    }
                    else
                    {
                        string logginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        string username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                        cboSampler.EditValue = lstAcsUser.Where(o => o.LOGINNAME == logginname && o.USERNAME == username).FirstOrDefault().ID;
                    }
                    if (data.FirstOrDefault().FINISH_TIME != null && data.FirstOrDefault().FINISH_TIME > 0)
                    {
                        dtTimeReturn.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.FirstOrDefault().FINISH_TIME ?? 0) ?? DateTime.MinValue;
                    }
                    else
                    {
                        dtTimeReturn.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0) ?? DateTime.MinValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TestServiceReqExcute.Resources.Lang", typeof(HIS.Desktop.Plugins.TestServiceReqExcute.UCTestServiceReqExcute).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFinish.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.btnFinish.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCollSTT.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdCollSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIndexCode.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColIndexCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIndexName.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColIndexName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCollDonvitinh.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdCollDonvitinh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColVallue.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColVallue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColValueNormal.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColValueNormal.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMinValue.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColMinValue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMaxValue.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColMaxValue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLevel.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColLevel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsParent.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColIsParent.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNote.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColNote.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnKeDonThuoc.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.btnKeDonThuoc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableButtonKeDon()
        {

            try
            {
                if (this.currentServiceReq != null)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.ID = this.currentServiceReq.TREATMENT_ID;

                    var rsApi = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (rsApi != null && rsApi.Count > 0 && rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        checkNoiTru = false;
                        this.BtnRefreshForFormOther();
                    }
                    else
                    {
                        checkNoiTru = true;
                        this.BtnRefreshForFormOther();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataToGrid()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServViewFilter filter = new MOS.Filter.HisSereServViewFilter();
                filter.SERVICE_REQ_ID = currentServiceReq.ID;
                filter.HAS_EXECUTE = true;
                lstSereServ = new List<HIS_SERE_SERV>();
                lstSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filter, param);
                List<long> lstSereServIds = new List<long>();
                List<long> _ServiceIds = new List<long>();
                if (lstSereServ != null && lstSereServ.Count > 0)
                {
                    lstSereServ = lstSereServ.OrderBy(o => o.ID).ThenBy(p => p.TDL_SERVICE_NAME).ToList();
                    lstSereServIds = lstSereServ.Select(p => p.ID).ToList();
                    _ServiceIds = lstSereServ.Select(p => p.SERVICE_ID).ToList();
                    //Lấy cấu hình dịch vụ máy
                    _ServiceMachines = new List<HIS_SERVICE_MACHINE>();
                    MOS.Filter.HisServiceMachineFilter _machineFilter = new HisServiceMachineFilter();
                    _machineFilter.SERVICE_IDs = _ServiceIds;
                    _machineFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    _ServiceMachines = new BackendAdapter(param).Get<List<HIS_SERVICE_MACHINE>>("api/HisServiceMachine/Get", ApiConsumers.MosConsumer, _machineFilter, param);
                    //Lay ss dc luu trong ss ext
                    MOS.Filter.HisSereServExtFilter ssFilter = new HisSereServExtFilter();
                    ssFilter.SERE_SERV_IDs = lstSereServIds;
                    ssFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    var _SereServExts = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("/api/HisSereServExt/Get", ApiConsumers.MosConsumer, ssFilter, param);

                    //Lay HIS_TEST_INDEX  #2170
                    MOS.Filter.HisTestIndexViewFilter _TestIndexFilter = new HisTestIndexViewFilter();
                    _TestIndexFilter.SERVICE_IDs = _ServiceIds;
                    _TestIndexFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var _TestIndexs = new BackendAdapter(param).Get<List<V_HIS_TEST_INDEX>>("/api/HisTestIndex/GetView", ApiConsumers.MosConsumer, _TestIndexFilter, param);

                    HisSereServTeinViewFilter sereSerTeinFilter = new HisSereServTeinViewFilter();
                    sereSerTeinFilter.SERE_SERV_IDs = lstSereServIds;
                    sereSerTeinFilter.ORDER_FIELD = "NUM_ORDER";
                    sereSerTeinFilter.ORDER_DIRECTION = "DESC";
                    sereSerTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var lstSereServTeinItem = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>(HisRequestUriStore.HIS_SERE_SERV_TEIN_GET, ApiConsumers.MosConsumer, sereSerTeinFilter, param);


                    lstHisSereServTeinSDO = new List<ADO.HisSereServTeinSDO>();

                    //MOS.Filter.HisServiceFilter _ServiceFilter = new MOS.Filter.HisServiceFilter();
                    //_ServiceFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    //_ServiceFilter.IDs = _ServiceIds;
                    //List<HIS_SERVICE> _Services = new BackendAdapter(param).Get<List<HIS_SERVICE>>(HisRequestUriStore.HIS_SERVICE_GET, ApiConsumers.MosConsumer, _ServiceFilter, param);
                    _SereServNumOders = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                    _SereServNumOders.AddRange((from r in lstSereServ select new MPS.Processor.Mps000014.PDO.SereServNumOder(r, BackendDataWorker.Get<V_HIS_SERVICE>())).ToList());

                    _SereServNumOders = _SereServNumOders.OrderByDescending(p => p.SERVICE_NUM_ODER).ThenBy(p => p.TDL_SERVICE_NAME).ToList();
                    foreach (var item in _SereServNumOders)
                    {
                        ADO.HisSereServTeinSDO hisSereServTeinSDO = new ADO.HisSereServTeinSDO();
                        hisSereServTeinSDO.IS_PARENT = 1;
                        hisSereServTeinSDO.TEST_INDEX_CODE = item.TDL_SERVICE_CODE;
                        hisSereServTeinSDO.TEST_INDEX_NAME = item.TDL_SERVICE_NAME;
                        hisSereServTeinSDO.SERE_SERV_ID = item.ID;
                        hisSereServTeinSDO.SERVICE_ID = item.SERVICE_ID;
                        hisSereServTeinSDO.SERVICE_CODE = item.TDL_SERVICE_CODE;
                        hisSereServTeinSDO.SERVICE_NAME = item.TDL_SERVICE_NAME;
                        hisSereServTeinSDO.HisService_MIN_PROCESS_TIME = item.HisService_MIN_PROCESS_TIME;
                        hisSereServTeinSDO.HisService_MAX_PROCESS_TIME = item.HisService_MAX_PROCESS_TIME;
                        hisSereServTeinSDO.HisService_MAX_TOTAL_PROCESS_TIME = item.HisService_MAX_TOTAL_PROCESS_TIME;
                        hisSereServTeinSDO.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID;

                        if (_ServiceMachines != null && _ServiceMachines.Count > 0)
                        {
                            var ssMachine = _ServiceMachines.Where(p => p.SERVICE_ID == item.SERVICE_ID).ToList();
                            if (ssMachine != null && ssMachine.Count == 1)
                            {
                                hisSereServTeinSDO.MACHINE_ID = ssMachine[0].MACHINE_ID;
                            }
                        }

                        if (_SereServExts != null && _SereServExts.Count > 0)
                        {
                            var data = _SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                            if (data != null)
                            {
                                hisSereServTeinSDO.MACHINE_ID = data.MACHINE_ID;
                            }
                        }


                        var listSereServTeinByServServ = lstSereServTeinItem.Where(o => o.SERE_SERV_ID == item.ID).ToList();
                        var machineSereServ = lstSereServTeinItem.Where(o => o.MACHINE_ID.HasValue && o.SERE_SERV_ID == item.ID).ToList();
                        if (machineSereServ != null && machineSereServ.Count > 0 && machineSereServ[0].MACHINE_ID != null)
                            hisSereServTeinSDO.MACHINE_ID = machineSereServ[0].MACHINE_ID;

                        if (listSereServTeinByServServ != null
                            && listSereServTeinByServServ.Count == 1
                            && listSereServTeinByServServ[0].IS_NOT_SHOW_SERVICE == 1)
                        {
                            hisSereServTeinSDO.HAS_ONE_CHILD = 1;
                            if (!String.IsNullOrEmpty(listSereServTeinByServServ[0].VALUE))
                            {
                                hisSereServTeinSDO.VALUE = listSereServTeinByServServ[0].VALUE;
                            }
                            // hisSereServTeinSDO.DESCRIPTION = listSereServTeinByServServ[0].DESCRIPTION;
                            hisSereServTeinSDO.SERE_SERV_ID = listSereServTeinByServServ[0].SERE_SERV_ID;
                            hisSereServTeinSDO.TEST_INDEX_ID = listSereServTeinByServServ[0].TEST_INDEX_ID;
                            hisSereServTeinSDO.TEST_INDEX_CODE = listSereServTeinByServServ[0].TEST_INDEX_CODE;
                            hisSereServTeinSDO.TEST_INDEX_NAME = listSereServTeinByServServ[0].TEST_INDEX_NAME;
                            hisSereServTeinSDO.TEST_INDEX_UNIT_NAME = listSereServTeinByServServ[0].TEST_INDEX_UNIT_NAME;
                            hisSereServTeinSDO.RESULT_CODE = listSereServTeinByServServ[0].RESULT_CODE;
                            hisSereServTeinSDO.IS_IMPORTANT = listSereServTeinByServServ[0].IS_IMPORTANT;
                            if (!String.IsNullOrEmpty(listSereServTeinByServServ[0].NOTE))
                            {
                                hisSereServTeinSDO.NOTE = listSereServTeinByServServ[0].NOTE;
                            }
                            else
                            {
                                if (_SereServExts != null && _SereServExts.Count > 0)
                                {
                                    var ExtNote = _SereServExts.Where(o => o.SERE_SERV_ID == listSereServTeinByServServ[0].SERE_SERV_ID).FirstOrDefault();
                                    if (ExtNote != null)
                                        hisSereServTeinSDO.NOTE = ExtNote.INSTRUCTION_NOTE;
                                }
                            }
                            hisSereServTeinSDO.LEAVEN = listSereServTeinByServServ[0].LEAVEN;
                            lstHisSereServTeinSDO.Add(hisSereServTeinSDO);
                        }
                        else if (listSereServTeinByServServ != null && (listSereServTeinByServServ.Count > 1 || (listSereServTeinByServServ.Count == 1 && listSereServTeinByServServ[0].IS_NOT_SHOW_SERVICE != 1)))
                        {
                            lstHisSereServTeinSDO.Add(hisSereServTeinSDO);
                            //var dataTeins = listSereServTeinByServServ.Where(p => p.SERE_SERV_ID == item.ID).ToList();
                            List<V_HIS_TEST_INDEX> dataTestIndexs = new List<V_HIS_TEST_INDEX>();
                            if (_TestIndexs != null && _TestIndexs.Count > 0)
                            {
                                dataTestIndexs = _TestIndexs.Where(p => p.TEST_SERVICE_TYPE_ID == item.SERVICE_ID).ToList();
                                if (listSereServTeinByServServ != null && listSereServTeinByServServ.Count > 0 && dataTestIndexs != null && dataTestIndexs.Count > 0)
                                {
                                    dataTestIndexs = dataTestIndexs.Where(p => !listSereServTeinByServServ.Select(o => o.TEST_INDEX_ID).Contains(p.ID)).ToList();
                                }
                            }

                            foreach (var ssTein in listSereServTeinByServServ)
                            {
                                ADO.HisSereServTeinSDO hisSereServTein = new ADO.HisSereServTeinSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisSereServTeinSDO>(hisSereServTein, item);
                                hisSereServTein.IS_PARENT = 0;
                                hisSereServTein.HAS_ONE_CHILD = 0;
                                if (!String.IsNullOrEmpty(ssTein.VALUE))
                                {
                                    hisSereServTein.VALUE = ssTein.VALUE;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(hisSereServTeinSDO.VALUE))
                                    {
                                        var data = _TestIndexs.Where(o => o.TEST_INDEX_CODE == hisSereServTeinSDO.TEST_INDEX_CODE).FirstOrDefault();
                                        if (data != null && !string.IsNullOrEmpty(data.DEFAULT_VALUE))
                                        {
                                            hisSereServTein.VALUE = data.DEFAULT_VALUE;
                                        }
                                    }
                                }
                                //hisSereServTein.DESCRIPTION = ssTein.DESCRIPTION;
                                hisSereServTein.SERE_SERV_ID = ssTein.SERE_SERV_ID;
                                hisSereServTein.TEST_INDEX_ID = ssTein.TEST_INDEX_ID;
                                hisSereServTein.TEST_INDEX_CODE = "        " + ssTein.TEST_INDEX_CODE;
                                hisSereServTein.TEST_INDEX_NAME = ssTein.TEST_INDEX_NAME;
                                hisSereServTein.TEST_INDEX_UNIT_NAME = ssTein.TEST_INDEX_UNIT_NAME;
                                hisSereServTein.RESULT_CODE = ssTein.RESULT_CODE;
                                if (!String.IsNullOrEmpty(ssTein.NOTE))
                                {
                                    hisSereServTein.NOTE = ssTein.NOTE;
                                }
                                else
                                {
                                    if (_SereServExts != null && _SereServExts.Count > 0)
                                    {
                                        var ExtNote = _SereServExts.Where(o => o.SERE_SERV_ID == ssTein.SERE_SERV_ID).FirstOrDefault();
                                        if (ExtNote != null)
                                            hisSereServTein.NOTE = ExtNote.INSTRUCTION_NOTE;
                                    }
                                }
                                hisSereServTein.LEAVEN = ssTein.LEAVEN;
                                hisSereServTein.IS_IMPORTANT = ssTein.IS_IMPORTANT;
                                hisSereServTein.SERVICE_CODE = item.TDL_SERVICE_CODE;
                                hisSereServTein.SERVICE_NAME = item.TDL_SERVICE_NAME;
                                lstHisSereServTeinSDO.Add(hisSereServTein);
                            }
                            foreach (var itemTestIndex in dataTestIndexs)
                            {
                                ADO.HisSereServTeinSDO hisSereServTein = new ADO.HisSereServTeinSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisSereServTeinSDO>(hisSereServTein, item);
                                hisSereServTein.IS_PARENT = 0;
                                hisSereServTein.HAS_ONE_CHILD = 0;
                                //hisSereServTein.DESCRIPTION = itemTestIndex.DEFAULT_VALUE;
                                // hisSereServTein.SERE_SERV_ID = itemTestIndex.SERE_SERV_ID;
                                hisSereServTein.TEST_INDEX_ID = itemTestIndex.ID;
                                hisSereServTein.TEST_INDEX_CODE = "        " + itemTestIndex.TEST_INDEX_CODE;
                                hisSereServTein.TEST_INDEX_NAME = itemTestIndex.TEST_INDEX_NAME;
                                hisSereServTein.TEST_INDEX_UNIT_NAME = itemTestIndex.TEST_INDEX_UNIT_NAME;
                                hisSereServTein.IS_IMPORTANT = itemTestIndex.IS_IMPORTANT;
                                lstHisSereServTeinSDO.Add(hisSereServTein);
                            }
                        }
                        else if (_TestIndexs != null && _TestIndexs.Count > 0)
                        {
                            var dataTestIndexs = _TestIndexs.Where(p => p.TEST_SERVICE_TYPE_ID == item.SERVICE_ID).ToList();
                            if (dataTestIndexs != null
                                && dataTestIndexs.Count == 1
                                && dataTestIndexs[0].IS_NOT_SHOW_SERVICE == 1)
                            {
                                ADO.HisSereServTeinSDO hisSereServTein = new ADO.HisSereServTeinSDO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisSereServTeinSDO>(hisSereServTein, item);
                                hisSereServTein.IS_PARENT = 0;
                                hisSereServTein.HAS_ONE_CHILD = 0;
                                //hisSereServTein.DESCRIPTION = dataTestIndexs[0].DEFAULT_VALUE;
                                hisSereServTein.TEST_INDEX_ID = dataTestIndexs[0].ID;
                                hisSereServTein.TEST_INDEX_CODE = "        " + dataTestIndexs[0].TEST_INDEX_CODE;
                                hisSereServTein.TEST_INDEX_NAME = dataTestIndexs[0].TEST_INDEX_NAME;
                                hisSereServTein.TEST_INDEX_UNIT_NAME = dataTestIndexs[0].TEST_INDEX_UNIT_NAME;
                                hisSereServTein.IS_IMPORTANT = dataTestIndexs[0].IS_IMPORTANT;
                                lstHisSereServTeinSDO.Add(hisSereServTein);
                            }
                            else if (dataTestIndexs != null && dataTestIndexs.Count > 0)
                            {
                                if (dataTestIndexs.Count == 1)
                                    hisSereServTeinSDO.HAS_ONE_CHILD = 1;
                                lstHisSereServTeinSDO.Add(hisSereServTeinSDO);
                                foreach (var itemTestIndex in dataTestIndexs)
                                {
                                    ADO.HisSereServTeinSDO hisSereServTein = new ADO.HisSereServTeinSDO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisSereServTeinSDO>(hisSereServTein, item);
                                    hisSereServTein.IS_PARENT = 0;
                                    hisSereServTein.HAS_ONE_CHILD = 0;
                                    //hisSereServTein.DESCRIPTION = itemTestIndex.DEFAULT_VALUE;
                                    hisSereServTein.TEST_INDEX_ID = itemTestIndex.ID;
                                    hisSereServTein.TEST_INDEX_CODE = "        " + itemTestIndex.TEST_INDEX_CODE;
                                    hisSereServTein.TEST_INDEX_NAME = itemTestIndex.TEST_INDEX_NAME;
                                    hisSereServTein.TEST_INDEX_UNIT_NAME = itemTestIndex.TEST_INDEX_UNIT_NAME;
                                    hisSereServTein.IS_IMPORTANT = itemTestIndex.IS_IMPORTANT;
                                    lstHisSereServTeinSDO.Add(hisSereServTein);
                                }
                            }
                        }
                        else
                        {
                            lstHisSereServTeinSDO.Add(hisSereServTeinSDO);
                        }
                    }
                }
                // gán test index range
                if (lstHisSereServTeinSDO != null && lstHisSereServTeinSDO.Count > 0)
                {

                    var testIndexRangeAll = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>();
                    foreach (var hisSereServTeinSDO in lstHisSereServTeinSDO)
                    {
                        V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                        testIndexRange = GetTestIndexRange(this.currentServiceReq.TDL_PATIENT_DOB, this.genderId, hisSereServTeinSDO.TEST_INDEX_ID ?? 0, ref this.testIndexRangeAll);
                        if (testIndexRange != null)
                        {
                            ProcessMaxMixValue(hisSereServTeinSDO, testIndexRange);
                        }
                    }
                }

                gridControlSereServTein.DataSource = lstHisSereServTeinSDO;

                gridViewSereServTein.FocusedRowHandle = 1;

                gridViewSereServTein.FocusedColumn = gridViewSereServTein.VisibleColumns[3];

                // gridViewSereServTein.ShowEditor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long LoadGenderId()
        {
            long genderId = 0;
            try
            {
                CommonParam param = new CommonParam();
                if (currentServiceReq != null && currentServiceReq.TDL_PATIENT_GENDER_ID != null)
                {
                    genderId = currentServiceReq.TDL_PATIENT_GENDER_ID ?? 0;
                }
                else if (currentServiceReq != null && !String.IsNullOrWhiteSpace(currentServiceReq.TDL_PATIENT_CODE))
                {
                    HisPatientFilter patientFilter = new HisPatientFilter();
                    patientFilter.PATIENT_CODE = currentServiceReq.TDL_PATIENT_CODE;
                    var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                    if (patients != null && patients.Count > 0)
                    {
                        genderId = patients.FirstOrDefault().GENDER_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                genderId = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return genderId;
        }

        private async Task LoadUser()
        {
            try
            {
                List<ACS_USER> listResult = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                this.lstAcsUser = listResult;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(cboResultApprover, listResult, controlEditorADO);
                ACS_USER user = null;
                if (currentServiceReq != null && !String.IsNullOrWhiteSpace(currentServiceReq.RESULT_APPROVER_LOGINNAME))
                {
                    user = listResult.FirstOrDefault(o => o.LOGINNAME == currentServiceReq.RESULT_APPROVER_LOGINNAME);
                }

                if (user != null)
                {
                    cboResultApprover.EditValue = user.LOGINNAME;
                    txtResultApproverLoginname.Text = user.LOGINNAME;
                }
                else
                {
                    cboResultApprover.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    txtResultApproverLoginname.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AssignNormal(ref ADO.HisSereServTeinSDO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                decimal value;
                if (ti != null && testIndexRange != null)
                {
                    ti.IS_NORMAL = null;
                    ti.IS_HIGHER = null;
                    ti.IS_LOWER = null;
                    if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
                    {
                        if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE) && ti.VALUE.ToUpper() == ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_NORMAL = true;
                        }
                        else if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE) && ti.VALUE.ToUpper() != ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_LOWER = true;
                            ti.IS_HIGHER = true;
                        }
                    }
                    else
                    {
                        if (!Decimal.TryParse((ti.VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value))
                            return;

                        if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {

                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE <= value && ti.MAX_VALUE != null && value < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }

                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && value < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE <= value)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {

                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE <= value && ti.MAX_VALUE != null && value <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && value < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE < value)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {

                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE < value && ti.MAX_VALUE != null && value <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && value < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE < value)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null && ti.MIN_VALUE != null && ti.MAX_VALUE != null)
                        {
                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE < value && ti.MAX_VALUE != null && value < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && value <= ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE <= value)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMaxMixValue(ADO.HisSereServTeinSDO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                Decimal minValue = 0, maxValue = 0;
                if (ti != null && testIndexRange != null)
                {
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out minValue))
                        {
                            ti.MIN_VALUE = minValue;
                        }
                        else
                        {
                            ti.MIN_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out maxValue))
                        {
                            ti.MAX_VALUE = maxValue;
                        }
                        else
                        {
                            ti.MAX_VALUE = null;
                        }
                    }

                    ti.IS_NORMAL = null;
                    ti.IS_HIGHER = null;
                    ti.IS_LOWER = null;
                    ti.DESCRIPTION = "";

                    if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
                    {
                        ti.DESCRIPTION = testIndexRange.NORMAL_VALUE;
                        if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE) && ti.VALUE.ToUpper() == ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_NORMAL = true;
                        }
                        else if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE) && ti.VALUE.ToUpper() != ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_LOWER = true;
                            ti.IS_HIGHER = true;
                        }
                    }
                    else
                    {
                        if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            ti.DESCRIPTION = "";
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }

                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE <= Convert.ToInt64(ti.VALUE.Trim()) && ti.MAX_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE <= Convert.ToInt64(ti.VALUE.Trim()))
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            ti.DESCRIPTION = "";
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE <= Convert.ToInt64(ti.VALUE.Trim()) && ti.MAX_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE < Convert.ToInt64(ti.VALUE.Trim()))
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            ti.DESCRIPTION = "";
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE < Convert.ToInt64(ti.VALUE.Trim()) && ti.MAX_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE < Convert.ToInt64(ti.VALUE.Trim()))
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            ti.DESCRIPTION = "";
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }
                            if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && ti.MIN_VALUE < Convert.ToInt64(ti.VALUE.Trim()) && ti.MAX_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MIN_VALUE != null && Convert.ToInt64(ti.VALUE.Trim()) <= ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE) && ti.MAX_VALUE != null && ti.MAX_VALUE <= Convert.ToInt64(ti.VALUE.Trim()))
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMaxMixValue(ADO.HisSereServTeinSDO ti, string description)
        {
            try
            {
                if (ti != null && !String.IsNullOrWhiteSpace(description))
                {
                    string[] values = description.Split('(', ' ', '-', ')');
                    values = values != null ? values.Where(o => !String.IsNullOrWhiteSpace(o)).ToArray() : null;
                    Decimal minValue, maxValue;

                    if (values != null && values.Length > 1 && Decimal.TryParse((values[0] ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out minValue) && Decimal.TryParse((values[1] ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out maxValue))
                    {
                        ti.MAX_VALUE = maxValue;
                        ti.MIN_VALUE = minValue;
                    }
                    else
                    {
                        ti.MIN_VALUE = null;
                        ti.MAX_VALUE = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE GetTestIndexRange(long dob, long genderId, long testIndexId, ref List<V_HIS_TEST_INDEX_RANGE> testIndexRanges)
        {
            MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
            try
            {
                if (testIndexRanges != null && testIndexRanges.Count > 0)
                {
                    double age = 0;
                    List<V_HIS_TEST_INDEX_RANGE> query = new List<V_HIS_TEST_INDEX_RANGE>();
                    var ageDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob);
                    foreach (var item in testIndexRanges)
                    {
                        if (item.TEST_INDEX_ID == testIndexId)
                        {
                            if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__YEAR) // Năm
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays / 365;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__MONTH) // Tháng
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays / 30;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__DAY) // Ngày
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__HOUR) // Giờ
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalHours;
                            }

                            if (((item.AGE_FROM.HasValue && item.AGE_FROM.Value <= age) || !item.AGE_FROM.HasValue)
                                 && ((item.AGE_TO.HasValue && item.AGE_TO.Value >= age) || !item.AGE_TO.HasValue))
                            {
                                query.Add(item);
                            }
                        }
                    }
                    if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1).ToList();
                    }
                    else if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        query = query.Where(o => o.IS_FEMALE == 1).ToList();
                    }
                    testIndexRange = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return testIndexRange;
        }

        private void gridViewSereServTein_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                // long configKey = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.HIS_DESKTOP_PLUGINS_TEST_CHECKVALUEMAXLENGOPTION));
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    ADO.HisSereServTeinSDO data = (ADO.HisSereServTeinSDO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "VALUE")
                        {
                            //  long is_parent = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSereServTein.GetRowCellValue(e.RowHandle, "IS_PARENT") ?? "").ToString());
                            if (data.IS_PARENT == 1 && data.HAS_ONE_CHILD == 0)
                            {
                                e.RepositoryItem = repositoryItemTextValue_Disable;
                            }
                            else
                            {

                                e.RepositoryItem = repositoryItemButtonEdit_Value;
                            }
                        }
                        else if (e.Column.FieldName == "ColumnButtonForValue")
                        {
                            if (data.IS_PARENT == 1 && data.HAS_ONE_CHILD == 0)
                            {
                                e.RepositoryItem = null;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonEditPopup_Value;
                            }
                        }
                        else if (e.Column.FieldName == "ColumnButtonForNote")
                        {
                            if (data.IS_PARENT == 1 && data.HAS_ONE_CHILD == 0)
                            {
                                e.RepositoryItem = null;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonEditPopup_Note;
                            }
                        }

                        else if (e.Column.FieldName == "NOTE")
                        {
                            if (data.IS_PARENT == 1 && data.HAS_ONE_CHILD == 0)
                            {
                                e.RepositoryItem = repositoryItemTextEditNote_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEdit_Note;
                            }
                        }
                        if (e.Column.FieldName == "LEAVEN")
                        {
                            //  long is_parent = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSereServTein.GetRowCellValue(e.RowHandle, "IS_PARENT") ?? "").ToString());
                            if (data.IS_PARENT == 1 && data.HAS_ONE_CHILD == 0)
                            {
                                e.RepositoryItem = repositoryItemTextEditCanNguyen_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEdit_CanNguyen;
                            }
                        }
                        else if (e.Column.FieldName == "ColumnButtonForCanNguyen")
                        {
                            if (data.IS_PARENT == 1 && data.HAS_ONE_CHILD == 0)
                            {
                                e.RepositoryItem = null;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonEditPopup_CanNguyen;
                            }
                        }
                        else if (e.Column.FieldName == "MACHINE_ID")
                        {
                            if (data.IS_PARENT == 1)
                            {
                                // long is_MACHINE = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSereServTein.GetRowCellValue(e.RowHandle, "MACHINE_ID") ?? "0").ToString());
                                if (data.MACHINE_ID != null)
                                {
                                    e.RepositoryItem = repositoryItemGridLookUp_Machine;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemGridLookUp__Btn;
                                }

                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemTextValue_Disable;
                            }
                        }
                        else if (e.Column.FieldName == "SAMPLE_TIME_Str")
                        {
                            if (data.IS_PARENT == 1)
                            {
                                e.RepositoryItem = repositoryItemDateEdit_Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemTextEditNote_Disable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCheckNormal(ref ADO.HisSereServTeinSDO hisSereServTeinSDO)
        {
            try
            {
                V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                testIndexRange = GetTestIndexRange(this.currentServiceReq.TDL_PATIENT_DOB, this.genderId, hisSereServTeinSDO.TEST_INDEX_ID ?? 0, ref this.testIndexRangeAll);
                if (testIndexRange != null)
                {
                    AssignNormal(ref hisSereServTeinSDO, testIndexRange);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServTein_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                //if (e.RowHandle > 0)
                //{
                ADO.HisSereServTeinSDO data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (e.Column.FieldName == "VALUE")
                    {
                        ProcessCheckNormal(ref data);
                    }

                    if (data.IS_PARENT == 1 || data.HAS_ONE_CHILD == 1)
                    {
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }

                    if (data.IS_LOWER == true && data.IS_HIGHER == true)
                    {
                        e.Appearance.ForeColor = Color.Green;
                    }
                    else if (data.IS_LOWER == true)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                    else if (data.IS_HIGHER == true)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                    }

                    if (e.Column.FieldName == "TEST_INDEX_NAME" && data.IS_IMPORTANT > 0)
                    {
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                        e.Appearance.ForeColor = Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckMachine(HisSereServTeinSDO asereServ, ref bool IsReturn)
        {
            try
            {
                long machineId = asereServ.MACHINE_ID ?? 0;
                Inventec.Common.Logging.LogSystem.Info(machineId.ToString());

                if ((AppConfigKeys.IsMachineWarningOption == "1" || AppConfigKeys.IsMachineWarningOption == "2") && ((AppConfigKeys.IsPatientTypeOption == "1" && asereServ.PATIENT_TYPE_ID == AppConfigKeys.PatientTypeId__BHYT) || AppConfigKeys.IsPatientTypeOption != "1") && GlobalVariables.MachineCounterSdos != null && GlobalVariables.MachineCounterSdos.Count > 0)
                {
                    var machine = GlobalVariables.MachineCounterSdos.FirstOrDefault(o => o.ID == machineId);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => machine), machine));
                    if (machine != null && machine.MAX_SERVICE_PER_DAY.HasValue && machine.TOTAL_PROCESSED_SERVICE_TEIN.HasValue && machine.TOTAL_PROCESSED_SERVICE_TEIN.Value >= machine.MAX_SERVICE_PER_DAY.Value)
                    {
                        string mess = string.Format(ResourceMessage.CanhBaoMayVuotQuaSoluongXuLy, machine.MACHINE_NAME, machine.MAX_SERVICE_PER_DAY);
                        if (AppConfigKeys.IsMachineWarningOption == "2")
                            mess = string.Format(ResourceMessage.ChanMayVuotQuaSoluongXuLy, machine.MACHINE_NAME, machine.MAX_SERVICE_PER_DAY);
                        if ((AppConfigKeys.IsMachineWarningOption == "1" && DevExpress.XtraEditors.XtraMessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) || (AppConfigKeys.IsMachineWarningOption == "2" && DevExpress.XtraEditors.XtraMessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.OK) == DialogResult.OK))
                        {
                            //asereServ.MACHINE_ID = null;
                            gridViewSereServTein.GridControl.RefreshDataSource();
                            IsReturn = true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                int length = Encoding.UTF8.GetByteCount(txtValueRangeIntoPopup.Text);
                long configKey = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.HIS_DESKTOP_PLUGINS_TEST_CHECKVALUEMAXLENGOPTION));
                if (!ValidTimeReturn())
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian trả kết quả nhỏ hơn thời gian y lệnh", "Thông báo");
                    return;
                }

                if (gridViewSereServTein.HasColumnErrors)
                    return;
                HisTestResultTDO hisTestResultSDO = new MOS.TDO.HisTestResultTDO();
                hisTestResultSDO.TestIndexDatas = new List<HisTestIndexResultTDO>();
                string mess = "";
                bool IsReturn = false;
                bool valid = ProcessTestServiceReqExecute(hisTestResultSDO, ref mess, ref IsReturn);
                if (configKey == 1)
                {
                    if (!string.IsNullOrEmpty(mess))
                    {
                        string mes_ = "Chỉ số" + mess + "có giá trị vượt quá độ dài cho phép 50 ký tự";
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(mes_, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                        {
                            return;
                        }
                    }
                }
                else if (configKey == 2)
                {
                    if (!string.IsNullOrEmpty(mess))
                    {
                        string mes_ = "Chỉ số" + mess + " có giá trị vượt quá độ dài cho phép 50 ký tự. Bạn có muốn tiếp tục thực hiện không?";
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(mes_, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        {
                            return;
                        }

                    }
                }
                else if (configKey == 0)
                {

                }

                if (!valid)
                    return;

                bool validTestIndexData = true;
                validTestIndexData = validTestIndexData && (hisTestResultSDO != null);
                validTestIndexData = validTestIndexData && (hisTestResultSDO.TestIndexDatas != null && hisTestResultSDO.TestIndexDatas.Count > 0);
                if (validTestIndexData)
                {
                    SaveTestServiceReq(hisTestResultSDO);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa nhập giá trị chỉ số", "Thông báo");
                }
                //if (chkPrint.Checked)
                //{
                //    gridViewSereServTein.PostEditor();
                //    _IsKeyPrintNow = true;
                //    btnPrint_Click(null, null);
                //}
                if (chkKy.Checked || chkPrint.Checked || chkPreviewPrint.Checked)
                {
                    if (chkPrint.Checked)
                    {
                        _IsKeyPrintNow = true;
                    }
                    gridViewSereServTein.PostEditor();
                    btnPrint_Click(null, null);
                }
                if (chkFinish.Checked)
                {
                    if (this.Is_ALLOW_FINISH)
                    {
                        btnFinish_Click(null, null);
                    }
                    else if (AppConfigKeys.Is_ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn không là Bác sỹ nên không được phép kết thúc", "Thông báo");
                    }
                }
                if (chkClose.Checked)
                {
                    TabControlBaseProcess.CloseSelectedTabPage(SessionManager.GetTabControlMain());
                }
                //if (chkPreviewPrint.Checked)
                //{
                //    btnPrint_Click(null, null);
                //}
                //if (chkKy.Checked && (!chkPrint.Checked && !chkPreviewPrint.Checked))
                //{
                //    gridViewSereServTein.PostEditor();
                //    PrintProcessForKy(PrintTypeTest.IN_PHIEU_KET_QUA_XET_NGHIEM);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public static DateTime? ConvertDateTimeStringToSystemTime(string datetime)
        {
            DateTime? result = null;
            try
            {
                if (!String.IsNullOrEmpty(datetime))
                {
                    //datetime = datetime.Replace("", "");
                    int day = Int16.Parse(datetime.Substring(0, 2));
                    int month = Int16.Parse(datetime.Substring(3, 2));
                    int year = Int16.Parse(datetime.Substring(6, 4));
                    int hour = Int16.Parse(datetime.Substring(11, 2));
                    int minute = Int16.Parse(datetime.Substring(14, 2));
                    result = new DateTime(year, month, day, hour, minute, 0);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

        private bool ProcessTestServiceReqExecute(HisTestResultTDO hisTestResultSDO, ref string mess, ref bool IsReturn)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessTestServiceReqExecute().Started!");
                List<ADO.HisSereServTeinSDO> listSereServ = gridControlSereServTein.DataSource as List<ADO.HisSereServTeinSDO>;
                if (listSereServ != null && listSereServ.Count > 0)
                {
                    Dictionary<string, string> dicServiceInvalidMinProcessTime = new Dictionary<string, string>();
                    Dictionary<string, string> dicServiceInvalidMaxProcessTime = new Dictionary<string, string>();
                    Dictionary<string, string> dicServiceInvalid = new Dictionary<string, string>();
                    List<string> listServiceNotHasMachine = new List<string>();

                    List<ADO.HisSereServTeinSDO> _SereServParents = listSereServ.Where(o => o.IS_PARENT == 1).ToList();
                    if (AppConfigKeys.SubclinicalMachineOption == "1" || AppConfigKeys.SubclinicalMachineOption == "2")
                    {
                        foreach (var item in _SereServParents)
                        {
                            if (item.MACHINE_ID == null)
                            {
                                if (!String.IsNullOrEmpty(item.SERVICE_NAME) && !listServiceNotHasMachine.Contains(item.SERVICE_NAME))
                                {
                                    listServiceNotHasMachine.Add(item.SERVICE_NAME);
                                }
                            }
                            CheckMachine(item, ref IsReturn);
                            if (IsReturn)
                                break;
                        }
                    }
                    else if (AppConfigKeys.SubclinicalMachineOption == "3" || AppConfigKeys.SubclinicalMachineOption == "4")
                    {
                        foreach (var item in _SereServParents)
                        {
                            if (item.PATIENT_TYPE_ID == AppConfigKeys.PatientTypeId__BHYT && item.MACHINE_ID == null)
                            {
                                if (!String.IsNullOrEmpty(item.SERVICE_NAME) && !listServiceNotHasMachine.Contains(item.SERVICE_NAME))
                                {
                                    listServiceNotHasMachine.Add(item.SERVICE_NAME);
                                }
                            }
                            CheckMachine(item, ref IsReturn);
                            if (IsReturn)
                                break;
                        }
                    }
                    if (IsReturn)
                        return false;
                    if (!string.IsNullOrEmpty(mess))
                    {
                        return false;
                    }

                    TimeSpan? processTime = null;
                    TimeSpan? processTimeService = null;

                    if (dtTimeReturn.EditValue != null)
                    {
                        var timeReturn = Convert.ToDateTime(dtTimeReturn.EditValue);
                        if (this.currentServiceReq != null && this.currentServiceReq.START_TIME > 0)
                        {
                            processTime = timeReturn - (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentServiceReq.START_TIME ?? 0));
                            processTimeService = timeReturn - (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentServiceReq.INTRUCTION_TIME));
                        }
                    }
                    // 50264=



                    List<ADO.HisSereServTeinSDO> lstSereServTein = listSereServ.Where(o => o.IS_PARENT != 1 || (o.IS_PARENT == 1 && o.HAS_ONE_CHILD == 1)).ToList();
                    if (lstSereServTein != null && lstSereServTein.Count > 0)
                    {
                        foreach (var item in lstSereServTein)
                        {
                            if (String.IsNullOrWhiteSpace(item.VALUE))
                                continue;

                            bool isContinue = false;
                            V_HIS_SERVICE service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                            var arrMinProcessTimeExceptPatyIds = (service.MIN_PROC_TIME_EXCEPT_PATY_IDS ?? "").Split(',');
                            List<long> minProcessTimeExceptPatyIds = arrMinProcessTimeExceptPatyIds != null ? arrMinProcessTimeExceptPatyIds.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o)).ToList() : new List<long>();
                            var arrMaxProcessTimeExceptPatyIds = (service.MAX_PROC_TIME_EXCEPT_PATY_IDS ?? "").Split(',');
                            List<long> maxProcessTimeExceptPatyIds = arrMaxProcessTimeExceptPatyIds != null ? arrMaxProcessTimeExceptPatyIds.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o)).ToList() : new List<long>();

                            if (!minProcessTimeExceptPatyIds.Contains(item.PATIENT_TYPE_ID ?? -1) && processTime != null && item.HisService_MIN_PROCESS_TIME > 0)
                            {
                                if (((TimeSpan)processTime).TotalMinutes < item.HisService_MIN_PROCESS_TIME)
                                {
                                    if (!String.IsNullOrEmpty(item.SERVICE_CODE) && !dicServiceInvalidMinProcessTime.ContainsKey(item.SERVICE_CODE))
                                    {
                                        dicServiceInvalidMinProcessTime.Add(item.SERVICE_CODE, item.HisService_MIN_PROCESS_TIME.ToString());
                                    }
                                    isContinue = true;
                                }
                            }
                            if (!maxProcessTimeExceptPatyIds.Contains(item.PATIENT_TYPE_ID ?? -1) && processTime != null && item.HisService_MAX_PROCESS_TIME > 0)
                            {
                                if (((TimeSpan)processTime).TotalMinutes > item.HisService_MAX_PROCESS_TIME)
                                {
                                    if (!String.IsNullOrEmpty(item.SERVICE_CODE) && !dicServiceInvalidMaxProcessTime.ContainsKey(item.SERVICE_CODE))
                                    {
                                        dicServiceInvalidMaxProcessTime.Add(item.SERVICE_CODE, item.HisService_MAX_PROCESS_TIME.ToString());
                                    }
                                    isContinue = true;
                                }
                            }
                            if (isContinue)
                                continue;
                            // 50264
                            if (processTimeService != null && item.HisService_MAX_TOTAL_PROCESS_TIME != null && item.HisService_MAX_TOTAL_PROCESS_TIME > 0 && (string.IsNullOrEmpty(service.TOTAL_TIME_EXCEPT_PATY_IDS) || !service.TOTAL_TIME_EXCEPT_PATY_IDS.Split(',').Contains(item.PATIENT_TYPE_ID.ToString())))
                            {
                                if (((TimeSpan)processTimeService).TotalMinutes > item.HisService_MAX_TOTAL_PROCESS_TIME)
                                {
                                    if (!String.IsNullOrEmpty(item.SERVICE_NAME) && !dicServiceInvalid.ContainsKey(item.SERVICE_NAME))
                                    {
                                        dicServiceInvalid.Add(item.SERVICE_NAME, item.HisService_MAX_TOTAL_PROCESS_TIME.ToString());
                                    }
                                }
                            }

                            HisTestIndexResultTDO sdo = new HisTestIndexResultTDO();
                            sdo.TestIndexCode = item.TEST_INDEX_CODE != null ? item.TEST_INDEX_CODE.TrimStart() : null;
                            sdo.Value = item.VALUE;
                            sdo.Note = item.NOTE;
                            sdo.Leaven = item.LEAVEN;
                            int length = Encoding.UTF8.GetByteCount(sdo.Value);
                            if (length > 50)
                            {
                                mess += item.TEST_INDEX_NAME + "; ";
                            }
                            if (_SereServParents != null && _SereServParents.Count > 0)
                            {
                                var dataMachine = _SereServParents.FirstOrDefault(p => p.SERE_SERV_ID == item.SERE_SERV_ID);
                                if (dataMachine != null)
                                {
                                    sdo.MachineId = dataMachine.MACHINE_ID;
                                }
                            }
                            hisTestResultSDO.TestIndexDatas.Add(sdo);
                        }
                    }

                    if (dicServiceInvalidMinProcessTime.Count > 0)
                    {
                        List<string> listStr = new List<string>();
                        foreach (var item in dicServiceInvalidMinProcessTime)
                        {
                            listStr.Add(String.Format("{0} ít hơn {1} phút", item.Key, item.Value));
                        }
                        if (listStr.Count > 0)
                        {
                            string message = String.Format("Bệnh nhân có thời gian thực hiện xét nghiệm dịch vụ:\n{0}", String.Join(",\n", listStr));
                            DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                            return false;
                        }
                    }
                    if (dicServiceInvalidMaxProcessTime.Count > 0)
                    {
                        List<string> listStr = new List<string>();
                        foreach (var item in dicServiceInvalidMaxProcessTime)
                        {
                            listStr.Add(String.Format("{0} lớn hơn {1} phút", item.Key, item.Value));
                        }
                        if (listStr.Count > 0)
                        {
                            string message = String.Format("Bệnh nhân có thời gian thực hiện xét nghiệm dịch vụ:\n{0}", String.Join(",\n", listStr));
                            DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                            return false;
                        }
                    }
                    //
                    if (dicServiceInvalid.Count() > 0)
                    {
                        string msg = "";
                        var dicServiceInvalidGroup = dicServiceInvalid.GroupBy(o => o.Value);
                        if (AppConfigKeys.ProcessTimeMustBeGreaterThanTotalProcessTime == "1")
                        {
                            foreach (var item in dicServiceInvalidGroup)
                            {
                                msg += string.Format("Không cho phép trả kết quả dịch vụ {0} sau {1} phút tính từ thời điểm ra y lệnh {2}\r\n", string.Join(",", item.Select(o => o.Key).ToList()), item.First().Value, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentServiceReq.INTRUCTION_TIME));
                            }
                            DevExpress.XtraEditors.XtraMessageBox.Show(msg, "Thông báo");
                            return false;
                        }
                        else if (AppConfigKeys.ProcessTimeMustBeGreaterThanTotalProcessTime == "2")
                        {
                            foreach (var item in dicServiceInvalidGroup)
                            {
                                msg += string.Format("Trả kết quả dịch vụ {0} vượt quá {1} phút tính từ thời điểm ra y lệnh {2}\r\n", string.Join(",", item.Select(o => o.Key).ToList()), item.First().Value, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentServiceReq.INTRUCTION_TIME));
                            }

                            if (DevExpress.XtraEditors.XtraMessageBox.Show(msg + "Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return false;
                            }
                        }
                    }
                    if (listServiceNotHasMachine.Count() > 0)
                    {
                        if (AppConfigKeys.SubclinicalMachineOption == "1")
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Dịch vụ {0} chưa có thông tin máy trả kết quả. Bạn có muốn tiếp tục?", string.Join(", ", listServiceNotHasMachine)), "Thông báo",
                              MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                return false;
                            }
                        }
                        else if (AppConfigKeys.SubclinicalMachineOption == "2")
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Dịch vụ {0} chưa có thông tin máy trả kết quả.", string.Join(", ", listServiceNotHasMachine)), "Thông báo");
                            return false;
                        }
                        else if (AppConfigKeys.SubclinicalMachineOption == "3")
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Dịch vụ {0} chưa có thông tin máy cận lâm sàng. Bạn có muốn tiếp tục không?", string.Join(", ", listServiceNotHasMachine)), "Thông báo",
                              MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                return false;
                            }
                        }
                        else if (AppConfigKeys.SubclinicalMachineOption == "4")
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Dịch vụ {0} chưa có thông tin máy cận lâm sàng.", string.Join(", ", listServiceNotHasMachine)), "Thông báo");
                            return false;
                        }
                    }
                }

                if (dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue)
                {
                    hisTestResultSDO.SampleTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                         Convert.ToDateTime(dtTime.EditValue).ToString("yyyyMMddHHmm") + "00");

                }
                else
                {
                    hisTestResultSDO.SampleTime = null;
                }

                if (cboResultApprover.EditValue != null)
                {
                    hisTestResultSDO.ApproverLoginname = cboResultApprover.EditValue.ToString();
                    ACS_USER u = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == hisTestResultSDO.ApproverLoginname);
                    if (u != null)
                    {
                        hisTestResultSDO.ApproverUsername = u.USERNAME;
                    }
                    else
                    {
                        hisTestResultSDO.ApproverUsername = null;
                    }
                }
                else
                {
                    hisTestResultSDO.ApproverLoginname = null;
                    hisTestResultSDO.ApproverUsername = null;
                }

                hisTestResultSDO.IsUpdateApprover = true;
                hisTestResultSDO.ServiceReqCode = currentServiceReq.SERVICE_REQ_CODE;
                hisTestResultSDO.ServiceReqId = currentServiceReq.ID;
                if (cboSampler.EditValue != null)
                {
                    var user = lstAcsUser.FirstOrDefault(o => o.ID == Convert.ToInt64(cboSampler.EditValue));
                    hisTestResultSDO.SampleLoginname = user != null ? user.LOGINNAME : null;
                    hisTestResultSDO.SampleUsername = user != null ? user.USERNAME : null;
                }
                else
                {
                    hisTestResultSDO.SampleLoginname = null;
                    hisTestResultSDO.SampleUsername = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        void SaveTestServiceReq(HisTestResultTDO hisTestResultSDO)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("Save_Input:--", hisTestResultSDO));

                success = new BackendAdapter(param).Post<bool>("/api/HisTestServiceReq/UpdateResult", ApiConsumers.MosConsumer, hisTestResultSDO, param);
                if (success)
                {
                    success = true;
                    currentServiceReq.RESULT_APPROVER_LOGINNAME = hisTestResultSDO.ApproverLoginname;
                    currentServiceReq.RESULT_APPROVER_USERNAME = hisTestResultSDO.ApproverUsername;
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(PrintTypeTest.IN_PHIEU_KET_QUA_XET_NGHIEM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckResultFinish())
                    return;

                if (!ValidTimeReturn())
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian trả kết quả nhỏ hơn thời gian y lệnh", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                if (currentServiceReq != null)
                {
                    if (!string.IsNullOrEmpty(cboUserAssign.EditValue.ToString()) && !string.IsNullOrEmpty(txtUserAssign.Text))
                    {
                        currentServiceReq.EXECUTE_LOGINNAME = lstAcsUser.Where(o => o.ID == Convert.ToInt64(cboUserAssign.EditValue)).FirstOrDefault().LOGINNAME;
                        currentServiceReq.EXECUTE_USERNAME = lstAcsUser.Where(o => o.ID == Convert.ToInt64(cboUserAssign.EditValue)).FirstOrDefault().USERNAME;
                    }
                    if (dtTimeReturn.EditValue != null)
                    {
                        currentServiceReq.FINISH_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                           Convert.ToDateTime(dtTimeReturn.EditValue).ToString("yyyyMMddHHmmss"));
                    }
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("Finish_Input:--", currentServiceReq));
                    var serviceReqFinish = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("/api/HisServiceReq/FinishWithTime", ApiConsumers.MosConsumer, currentServiceReq, param);
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("Finish_Out:--", serviceReqFinish));

                    if (serviceReqFinish != null)
                    {
                        success = true;
                        btnFinish.Enabled = false;
                        btnSave.Enabled = false;
                        //btnSaveAndPrint.Enabled = false;
                        //btnSaveInEnd.Enabled = false;
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckResultFinish()
        {
            bool rs = true;
            try
            {
                HisTestResultTDO hisTestResultSDO = new MOS.TDO.HisTestResultTDO();
                hisTestResultSDO.TestIndexDatas = new List<HisTestIndexResultTDO>();
                List<ADO.HisSereServTeinSDO> listSereServ = gridControlSereServTein.DataSource as List<ADO.HisSereServTeinSDO>;
                if (listSereServ != null && listSereServ.Count > 0)
                {
                    List<ADO.HisSereServTeinSDO> _SereServParents = listSereServ.Where(o => o.IS_PARENT == 1).ToList();
                    List<ADO.HisSereServTeinSDO> lstSereServTein = listSereServ.Where(o => o.IS_PARENT != 1 || (o.IS_PARENT == 1 && o.HAS_ONE_CHILD == 1)).ToList();
                    if (lstSereServTein != null && lstSereServTein.Count > 0)
                    {
                        foreach (var item in lstSereServTein)
                        {
                            if (String.IsNullOrWhiteSpace(item.VALUE))
                                continue;


                            HisTestIndexResultTDO sdo = new HisTestIndexResultTDO();
                            sdo.TestIndexCode = item.TEST_INDEX_CODE != null ? item.TEST_INDEX_CODE.TrimStart() : null;
                            sdo.Value = item.VALUE;
                            sdo.Note = item.NOTE;
                            sdo.Leaven = item.LEAVEN;
                            int length = Encoding.UTF8.GetByteCount(sdo.Value);
                            if (_SereServParents != null && _SereServParents.Count > 0)
                            {
                                var dataMachine = _SereServParents.FirstOrDefault(p => p.SERE_SERV_ID == item.SERE_SERV_ID);
                                if (dataMachine != null)
                                {
                                    sdo.MachineId = dataMachine.MACHINE_ID;
                                }
                            }
                            hisTestResultSDO.TestIndexDatas.Add(sdo);
                        }
                    }
                }
                rs = rs && (hisTestResultSDO != null);
                rs = rs && (hisTestResultSDO.TestIndexDatas != null && hisTestResultSDO.TestIndexDatas.Count > 0);
                if (!rs)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa nhập giá trị chỉ số", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return rs;
        }

        private enum KeType
        {
            KeTuTruc,
            KeNoiTru,
        }

        bool ValidTimeReturn()
        {
            if ((AppConfigKeys.IsStartTimeMustBeGreaterThanInstructionTime == "1" || AppConfigKeys.IsStartTimeMustBeGreaterThanInstructionTime == "2") &&  dtTimeReturn.DateTime != null && Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeReturn.DateTime) < this.currentServiceReq.INTRUCTION_TIME)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private void btnKeDonThuoc_Click(KeType type)
        {
            try
            {
                if (this.currentServiceReq != null)
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(this.currentServiceReq.TREATMENT_ID, 0, 0);

                        if (type == KeType.KeTuTruc)
                        {
                            assignServiceADO.IsCabinet = true;
                            assignServiceADO.ServiceReqId = this.currentServiceReq.ID;
                        }
                        else
                            assignServiceADO.IsExecutePTTT = true;

                        assignServiceADO.PatientDob = this.currentServiceReq.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = this.currentServiceReq.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = this.currentServiceReq.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.TreatmentCode = this.currentServiceReq.TREATMENT_CODE;
                        assignServiceADO.TreatmentId = this.currentServiceReq.TREATMENT_ID;
                        listArgs.Add(assignServiceADO);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnKeTuTruc_Click(object sender, EventArgs e)
        {
            btnKeDonThuoc_Click(KeType.KeTuTruc);
        }

        private void btnKeNoiTru_Click(object sender, EventArgs e)
        {
            btnKeDonThuoc_Click(KeType.KeNoiTru);
        }

        public void Save()
        {
            try
            {
                gridViewSereServTein.PostEditor();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Print()
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Finish()
        {
            try
            {
                if (this.Is_ALLOW_FINISH)
                {
                    btnFinish_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void KeDonThuoc()
        {
            try
            {
                btnKeDonThuoc_Click(KeType.KeNoiTru);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnVatTuHaoPhi_Click(object sender, EventArgs e)
        {
            try
            {
                if (_adoVatTuHaoPhi != null)
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisServiceReqMaty").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisServiceReqMaty");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(_adoVatTuHaoPhi.SERE_SERV_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemTextValue_Enable_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    GridView view = gridControlSereServTein.FocusedView as GridView;
                    if (view != null)
                    {
                        var data = (List<ADO.HisSereServTeinSDO>)view.GridControl.DataSource;
                        if (data.Count == 0)
                            return;
                        for (int i = view.FocusedRowHandle + 1; i < data.Count; i++)
                            if (data[i].IS_PARENT != 1 || data[i].HAS_ONE_CHILD != 0)
                            {
                                view.FocusedColumn = view.Columns[grdColVallue.FieldName];
                                view.FocusedRowHandle = i;
                                //view.ShowEditor();
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataComboUserName(object control, List<ACS_USER> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;//.ToUpper();
                    //LoadCombo(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void btnSaveAndPrint_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        gridViewSereServTein.PostEditor();
        //        btnSave_Click(null, null);
        //        _IsKeyPrintNow = true;
        //        btnPrint_Click(null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void btnSaveInEnd_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        gridViewSereServTein.PostEditor();
        //        btnSave_Click(null, null);
        //        _IsKeyPrintNow = true;
        //        btnPrint_Click(null, null);
        //        btnFinish_Click(null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //public void LuuVaIn()
        //{
        //    try
        //    {
        //        btnSaveAndPrint_Click(null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //public void LuuVaInVaKetThuc()
        //{
        //    try
        //    {
        //        btnSaveInEnd_Click(null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void LoadDataMachine()
        {
            try
            {

                this._Machines = new List<HIS_MACHINE>();
                MOS.Filter.HisMachineFilter filter = new HisMachineFilter();
                this._Machines = new BackendAdapter(new CommonParam()).Get<List<HIS_MACHINE>>("api/HisMachine/Get", ApiConsumers.MosConsumer, filter, null);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "Mã máy", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "Tên máy", 250, 2));
                columnInfos.Add(new ColumnInfo("TOTAL_PROCESSED_SERVICE_TEIN", "Đã xử lý", 150, 3));
                columnInfos.Add(new ColumnInfo("MAX_SERVICE_PER_DAY", "Tối đa", 150, 4));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, true, 700);

                List<HisMachineCounterSDO> lstData = new List<HisMachineCounterSDO>();
                if (GlobalVariables.MachineCounterSdos != null && GlobalVariables.MachineCounterSdos.Count > 0)
                {
                    lstData = GlobalVariables.MachineCounterSdos.Where(o => _Machines.Select(s => s.ID).Contains(o.ID)).ToList();
                }
                else
                {
                    foreach (var item in _Machines.ToList())
                    {
                        HisMachineCounterSDO sdo = new HisMachineCounterSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MACHINE>(sdo, item);
                        lstData.Add(sdo);
                    }
                }
                ControlEditorLoader.Load(this.repositoryItemGridLookUp_Machine, lstData, controlEditorADO);
                ControlEditorLoader.Load(this.repositoryItemGridLookUp__Btn, lstData, controlEditorADO);
                // repositoryItemGridLookUp_Machine.Buttons[1].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServTein_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ADO.HisSereServTeinSDO data = view.GetFocusedRow() as ADO.HisSereServTeinSDO;
                if (view.FocusedColumn.FieldName == "MACHINE_ID" && view.ActiveEditor is GridLookUpEdit && data.IS_PARENT == 1)
                {
                    gridViewSereServTein.ClearColumnErrors();
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    FillDataMachineCombo(data, editor);
                    //editor.EditValue = data != null ? data.MACHINE_ID : 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataMachineCombo(ADO.HisSereServTeinSDO data, DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                var _machineIds = this._ServiceMachines.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).Select(o => o.MACHINE_ID).ToList();
                List<HIS_MACHINE> dataMachines = new List<HIS_MACHINE>();
                if (_machineIds != null && _machineIds.Count > 0)
                {
                    dataMachines = this._Machines.Where(o => _machineIds.Contains(o.ID)).ToList();
                    dataMachines = dataMachines.Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }


                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "Mã máy", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "Tên máy", 250, 2));
                columnInfos.Add(new ColumnInfo("TOTAL_PROCESSED_SERVICE_TEIN", "Đã xử lý", 150, 3));
                columnInfos.Add(new ColumnInfo("MAX_SERVICE_PER_DAY", "Tối đa", 150, 4));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, true, 700);

                List<HisMachineCounterSDO> lstData = new List<HisMachineCounterSDO>();

                if (GlobalVariables.MachineCounterSdos != null && GlobalVariables.MachineCounterSdos.Count > 0)
                {
                    lstData = GlobalVariables.MachineCounterSdos.Where(o => dataMachines.Select(s => s.ID).Contains(o.ID)).ToList();
                }
                else
                {
                    foreach (var item in dataMachines.ToList())
                    {
                        HisMachineCounterSDO sdo = new HisMachineCounterSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MACHINE>(sdo, item);
                        lstData.Add(sdo);
                    }
                }
                ControlEditorLoader.Load(cbo, lstData, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUp_Machine_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var view = sender as GridLookUpEdit;
                    view.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServTein_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServTein_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    GridView view = sender as GridView;
                    GridColumn onOrderCol = view.Columns["MACHINE_ID"];
                    //Int64 inSt = (Int64)view.GetRowCellValue(e.RowHandle, onOrderCol);
                    //if (inSt <= 0)
                    //{
                    //    e.Valid = false;
                    //    view.SetColumnError(onOrderCol, "Trường dữ liệu bắt buộc", ErrorType.Warning);
                    //}
                    var data = (ADO.HisSereServTeinSDO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null && data.IS_PARENT == 1)
                    {
                        if (data.MACHINE_ID <= 0 || data.MACHINE_ID == null)
                        {
                            e.Valid = false;
                            view.SetColumnError(onOrderCol, "Trường dữ liệu bắt buộc", ErrorType.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        ADO.HisSereServTeinSDO _adoVatTuHaoPhi = new ADO.HisSereServTeinSDO();

        private void gridViewSereServTein_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                _adoVatTuHaoPhi = new ADO.HisSereServTeinSDO();
                _adoVatTuHaoPhi = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (_adoVatTuHaoPhi != null && _adoVatTuHaoPhi.SERE_SERV_ID > 0)
                {
                    check = true;
                    BtnRefreshForFormOther();
                }
                else
                {
                    check = false;
                    BtnRefreshForFormOther();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemTextValue_Enable_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                TextEdit txtValue = sender as TextEdit;
                if (txtValue != null && !String.IsNullOrWhiteSpace(txtValue.Text))
                {

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServTein_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "VALUE")
                {
                    this.gridViewSereServTein_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServTein_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewSereServTein.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlSereServTein.DataSource as List<ADO.HisSereServTeinSDO>;
                var row = listDatas[index];
                if (e.ColumnName == "VALUE" && !String.IsNullOrWhiteSpace(row.VALUE))
                {
                    int length = Encoding.UTF8.GetByteCount(row.VALUE);
                    long configKey = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.HIS_DESKTOP_PLUGINS_TEST_CHECKVALUEMAXLENGOPTION));

                    if (configKey == 1)
                    {
                        if (length > 50)
                        {
                            string mes = "Độ dài lớn hơn 50";
                            e.Info.ErrorType = ErrorType.Warning;
                            e.Info.ErrorText = mes;
                            //if (DevExpress.XtraEditors.XtraMessageBox.Show(mes, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                            //{

                            //}
                        }
                        else
                        {
                            e.Info.ErrorType = (ErrorType)(ErrorType.None);
                            e.Info.ErrorText = "";
                        }
                    }
                    else if (configKey == 2)
                    {
                        if (length > 50)
                        {
                            string mes = "Độ dài lớn hơn 50";
                            e.Info.ErrorType = ErrorType.Warning;
                            e.Info.ErrorText = mes;
                            //if (DevExpress.XtraEditors.XtraMessageBox.Show(mes, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            //{
                            //    return;
                            //}
                        }
                        else
                        {
                            e.Info.ErrorType = (ErrorType)(ErrorType.None);
                            e.Info.ErrorText = "";
                        }
                    }
                    else if (configKey == 0)
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
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

        private void btnAssignPaan_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.currentServiceReq != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();

                        listArgs.Add(this.currentServiceReq);
                        listArgs.Add(this.currentServiceReq.TREATMENT_ID);
                        listArgs.Add(this.currentServiceReq.ID);
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

        private void txtResultApproverLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboResultApprover.EditValue = null;
                        this.cboResultApprover.Focus();
                        this.cboResultApprover.ShowPopup();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboResultApprover.EditValue = searchResult[0].LOGINNAME;
                            this.txtResultApproverLoginname.Text = searchResult[0].LOGINNAME;
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
                        }
                        else
                        {
                            this.cboResultApprover.EditValue = null;
                            this.cboResultApprover.Focus();
                            this.cboResultApprover.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultApprover_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboResultApprover.EditValue != null)
                    {
                        ACS_USER data = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboResultApprover.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtResultApproverLoginname.Text = data.LOGINNAME;
                        }
                    }

                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultApprover_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboResultApprover.EditValue != null)
                    {
                        ACS_USER data = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboResultApprover.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtResultApproverLoginname.Text = data.LOGINNAME;
                            SendKeys.Send("{TAB}");
                        }
                    }
                }
                else
                {
                    this.cboResultApprover.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultApprover_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboResultApprover.EditValue = null;
                    txtResultApproverLoginname.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void drBtnOther_Click(object sender, EventArgs e)
        {
            try
            {
                drBtnOther.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkKy.Name)
                        {
                            chkKy.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkInTach.Name)
                        {
                            chkInTach.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkPrint.Name)
                        {
                            chkPrint.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkFinish.Name)
                        {
                            chkFinish.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkPreviewPrint.Name)
                        {
                            chkPreviewPrint.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkClose.Name)
                        {
                            chkClose.Checked = item.VALUE == "1";
                        }
                    }
                }
                InfomationADO ado = new InfomationADO();
                this.currentBySessionControlStateRDO = controlStateWorker.GetDataBySession(moduleLink);
                if (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0 && !this.checkNguoiChiDinh)
                {

                    foreach (var item in this.currentBySessionControlStateRDO)
                    {
                        if (item.KEY == "InfomationADO")
                        {
                            if (!string.IsNullOrEmpty(item.VALUE))
                            {
                                ado = JsonConvert.DeserializeObject<InfomationADO>(item.VALUE);
                            }
                        }
                    }
                    if (ado != null)
                    {
                        txtUserAssign.Text = ado.LOGIN_NAME_ASSIGN;
                        cboUserAssign.EditValue = ado.USER_NAME_ASSIGN;
                    }
                }
                else if ((this.currentBySessionControlStateRDO == null || this.currentBySessionControlStateRDO.Count == 0) && !this.checkNguoiChiDinh)
                {
                    string logginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    string username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    txtUserAssign.Text = logginname;
                    cboUserAssign.EditValue = lstAcsUser.Where(o => o.LOGINNAME == logginname && o.USERNAME == username).FirstOrDefault().ID;
                }
                if (AppConfigKeys.Is_ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR)
                {
                    string logginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var employee = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.LOGINNAME == logginname).SingleOrDefault();
                    if (employee != null && employee.IS_DOCTOR != 1)
                    {
                        btnFinish.Enabled = false;
                        this.Is_ALLOW_FINISH = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void chkKy_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkKy.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkKy.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkKy.Name;
                    csAddOrUpdate.VALUE = (chkKy.Checked ? "1" : "");
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

        private void chkInTach_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkInTach.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInTach.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkInTach.Name;
                    csAddOrUpdate.VALUE = (chkInTach.Checked ? "1" : "");
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

        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //chkPreviewPrint.Checked = false;
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrint.Name;
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
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

        private void chkPreviewPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //chkPrint.Checked = false;
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPreviewPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPreviewPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPreviewPrint.Name;
                    csAddOrUpdate.VALUE = (chkPreviewPrint.Checked ? "1" : "");
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

        private void chkFinish_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkFinish.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkFinish.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkFinish.Name;
                    csAddOrUpdate.VALUE = (chkFinish.Checked ? "1" : "");
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

        private void chkClose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkClose.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkClose.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkClose.Name;
                    csAddOrUpdate.VALUE = (chkClose.Checked ? "1" : "");
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

        private void ButtonEditPopup_Value_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                ADO.HisSereServTeinSDO testLisResultADO = new ADO.HisSereServTeinSDO();
                var focusSereServ = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (focusSereServ != null && focusSereServ is ADO.HisSereServTeinSDO)
                {
                    testLisResultADO = (ADO.HisSereServTeinSDO)focusSereServ;
                }
                txtValueRangeIntoPopup.Text = testLisResultADO.VALUE;

                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerRangeValue.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOKForValueRange_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                popupControlContainerRangeValue.HidePopup();
                data.VALUE = txtValueRangeIntoPopup.Text;
                gridViewSereServTein.UpdateCurrentRow();
                gridControlSereServTein.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelForValueRange_Click(object sender, EventArgs e)
        {
            txtValueRangeIntoPopup.Text = "";
            popupControlContainerRangeValue.HidePopup();
        }

        private void ButtonEditPopup_Note_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HisSereServTeinSDO testLisResultADO = new HisSereServTeinSDO();
                var data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (data != null && data is HisSereServTeinSDO)
                {
                    testLisResultADO = (HisSereServTeinSDO)data;
                }

                txtNoteIntoPopup.Text = testLisResultADO.NOTE;
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerNote.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOKForNote_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                popupControlContainerNote.HidePopup();
                data.NOTE = txtNoteIntoPopup.Text;
                gridViewSereServTein.UpdateCurrentRow();
                gridControlSereServTein.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelForNote_Click(object sender, EventArgs e)
        {
            txtNoteIntoPopup.Text = "";
            popupControlContainerNote.HidePopup();
        }

        private void gridViewSereServTein_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "")
            {
                //GridCellInfo cell = (GridCellInfo)e.Cell;
                //if ((cell.State & GridRowCellState.FocusedCell) != 0) return;
                //e.DefaultDraw();
                //BaseView view = (BaseView)sender;
                //GridViewAppearances paintAppearance = (GridViewAppearances)view.GetViewInfo().PaintAppearance;
                //Rectangle linesBounds = new Rectangle(e.Bounds.X - 1, e.Bounds.Y - 1, e.Bounds.Width + 1, e.Bounds.Height + 1);
                //GraphicsInfoArgs bandInfo = (GraphicsInfoArgs)cell.ColumnInfo.Tag;
                //if (bandInfo.Bounds.Right == cell.Bounds.Right)
                //    e.Graphics.DrawLine(paintAppearance.VertLine.GetBackPen(e.Cache), linesBounds.Right, linesBounds.Top, linesBounds.Right, linesBounds.Bottom);
                //if (cell.RowInfo.Bounds.Bottom == cell.Bounds.Bottom)
                //    e.Graphics.DrawLine(paintAppearance.HorzLine.GetBackPen(e.Cache), linesBounds.Left, linesBounds.Bottom, linesBounds.Right, linesBounds.Bottom);  
            }
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HisSereServTeinSDO testLisResultADO = new HisSereServTeinSDO();
                var data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (data != null && data is HisSereServTeinSDO)
                {
                    testLisResultADO = (HisSereServTeinSDO)data;
                }

                txtNoteIntoPopup.Text = testLisResultADO.NOTE;
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerNote.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_Value_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                ADO.HisSereServTeinSDO testLisResultADO = new ADO.HisSereServTeinSDO();
                var focusSereServ = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (focusSereServ != null && focusSereServ is ADO.HisSereServTeinSDO)
                {
                    testLisResultADO = (ADO.HisSereServTeinSDO)focusSereServ;
                }
                txtValueRangeIntoPopup.Text = testLisResultADO.VALUE;

                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerRangeValue.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNoteIntoPopup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space && ShortcutReplace.IsUseShortcutReplaceKeyBase)
                {
                    HIS.Desktop.Utility.ShortcutReplace.ReplaceValue(txtNoteIntoPopup);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtValueRangeIntoPopup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space && ShortcutReplace.IsUseShortcutReplaceKeyBase)
                {
                    HIS.Desktop.Utility.ShortcutReplace.ReplaceValue(txtValueRangeIntoPopup);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOkforCanNguyen_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                popupControlContainerCanNguyen.HidePopup();
                data.LEAVEN = txtCanNguyenIntoPopup.Text;
                gridViewSereServTein.UpdateCurrentRow();
                gridControlSereServTein.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelForCanNguyen_Click(object sender, EventArgs e)
        {
            txtCanNguyenIntoPopup.Text = "";
            popupControlContainerCanNguyen.HidePopup();
        }

        private void txtCanNguyenIntoPopup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space && ShortcutReplace.IsUseShortcutReplaceKeyBase)
                {
                    HIS.Desktop.Utility.ShortcutReplace.ReplaceValue(txtCanNguyenIntoPopup);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditPopup_CanNguyen_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HisSereServTeinSDO testLisResultADO = new HisSereServTeinSDO();
                var data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (data != null && data is HisSereServTeinSDO)
                {
                    testLisResultADO = (HisSereServTeinSDO)data;
                }

                txtCanNguyenIntoPopup.Text = testLisResultADO.LEAVEN;
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerCanNguyen.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_CanNguyen_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HisSereServTeinSDO testLisResultADO = new HisSereServTeinSDO();
                var data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (data != null && data is HisSereServTeinSDO)
                {
                    testLisResultADO = (HisSereServTeinSDO)data;
                }

                txtCanNguyenIntoPopup.Text = testLisResultADO.LEAVEN;
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerCanNguyen.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!chkPrint.Checked)
                {
                    chkPreviewPrint.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPreviewPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!chkPreviewPrint.Checked)
                {
                    chkPrint.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServTein_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (HisSereServTeinSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "SAMPLE_TIME_Str")
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(data.SAMPLE_TIME))
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Int64.Parse(data.SAMPLE_TIME));
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServTein_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                {
                    return;
                }
                var data = (HisSereServTeinSDO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "SAMPLE_TIME_Str")
                {
                    if (gridViewSereServTein.EditingValue is DateTime)
                    {
                        var dt = (DateTime)gridViewSereServTein.EditingValue;
                        if (dt == null || dt == DateTime.MinValue)
                        {
                            data.SAMPLE_TIME = null;
                        }
                        else
                        {
                            data.SAMPLE_TIME = dt.ToString("yyyyMMddHHmmss");
                        }

                    }
                }
                else if (e.Column.FieldName == "MACHINE_ID")
                {
                    if (data.IS_PARENT == 1)
                    {
                        if (data.MACHINE_ID != null)
                        {
                            repositoryItemGridLookUp_Machine_Closed(null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Combo)
                {
                    //if (dtTime.EditValue == null)
                    //{
                    //    DateEdit control = sender as DateEdit;
                    //    control.DateTime = DateTime.Now;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMddHHmm") + "________________");
                if (dtTime.EditValue != null)
                {
                    DateEdit control = sender as DateEdit;
                    var data = Convert.ToDateTime(dtTime.EditValue).ToString("yyyyMMddHHmm");
                    string time = "";
                    Inventec.Common.Logging.LogSystem.Warn(data + data.Length);
                    if (data.Contains("0000"))
                    {
                        var dateNow = Convert.ToDateTime(DateTime.Now).ToString("HHmm");
                        time = data.Substring(0, 9) + dateNow;
                    }
                    if (!string.IsNullOrEmpty(time))
                    {
                        control.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Int64.Parse(time)) ?? DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCTestServiceReqExcute_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtUserAssign.Text) && cboUserAssign.EditValue != null && !this.checkNguoiChiDinh)
                {
                    SaveData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveData()
        {
            try
            {
                InfomationADO ado = new InfomationADO();
                ado.LOGIN_NAME_ASSIGN = lstAcsUser.Where(o => o.ID == Convert.ToInt64(cboUserAssign.EditValue)).FirstOrDefault().LOGINNAME;
                ado.USER_NAME_ASSIGN = Convert.ToInt64(cboUserAssign.EditValue);
                string textJson = JsonConvert.SerializeObject(ado);

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == "InfomationADO" && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValue != null)
                {
                    csAddOrUpdateValue.VALUE = !checkNguoiChiDinh ? textJson : "";
                }
                else
                {
                    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValue.KEY = "InfomationADO";
                    csAddOrUpdateValue.VALUE = !checkNguoiChiDinh ? textJson : "";
                    csAddOrUpdateValue.MODULE_LINK = moduleLink;
                    if (this.currentBySessionControlStateRDO == null)
                        this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
                }
                this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUserAssign_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboUserAssign.EditValue != null)
                {
                    txtUserAssign.Text = lstAcsUser.Where(o => o.ID == Convert.ToInt64(cboUserAssign.EditValue)).FirstOrDefault().LOGINNAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtUserAssign_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtUserAssign.EditValue.ToString()))
                    {
                        var user = lstAcsUser.Where(o => o.LOGINNAME.Contains(txtUserAssign.Text) || o.USERNAME.Contains(txtUserAssign.Text));
                        if (user != null)
                        {
                            cboUserAssign.EditValue = user.FirstOrDefault().ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSampler_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtSampler.EditValue.ToString()))
                    {
                        var user = lstAcsUser.Where(o => o.LOGINNAME.Equals(txtSampler.Text.ToLower()));
                        if (user != null)
                        {
                            cboSampler.EditValue = user.FirstOrDefault().ID;
                        }
                    }
                    else
                    {
                        cboSampler.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSampler_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboSampler.EditValue != null)
                {
                    txtSampler.Text = lstAcsUser.FirstOrDefault(o => o.ID == Convert.ToInt64(cboSampler.EditValue)).LOGINNAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        private async void timer1_Tick()
        {
            try
            {
                LogSystem.Debug("ReloadMachineCounter. 1");
                HisMachineCounterFilter filter = new HisMachineCounterFilter();
                List<HisMachineCounterSDO> sdos = await new BackendAdapter(new CommonParam()).GetAsync<List<HisMachineCounterSDO>>("api/HisMachine/GetCounter", ApiConsumers.MosConsumer, filter, null);
                GlobalVariables.MachineCounterSdos = sdos;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalVariables.MachineCounterSdos), GlobalVariables.MachineCounterSdos));
                LoadDataMachine();
                LogSystem.Debug("ReloadMachineCounter. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUp_Machine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {

                var asereServ = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                bool returnTmp = false;
                CheckMachine(asereServ, ref returnTmp);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
