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
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisTrackingList.ADO;
using HIS.Desktop.Plugins.HisTrackingList.Config;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTrackingList.Run
{
    public partial class frmHisTrackingList : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;
        //Bổ sung nghiệp vụ kí điện tử
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.HisTrackingList";
        const string timer1 = "timer1";

        internal List<V_HIS_TRACKING> vHisTrackingPrint { get; set; }
        internal List<V_HIS_TRACKING> _TrackingPrints { get; set; }
        internal HIS_DHST currentDhst { get; set; }
        CommonParam param = new CommonParam();
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;

        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        int pageSize = 0;

        internal List<V_HIS_TRACKING> vHisTrackingList { get; set; }
        SDA_CONFIG_APP _currentConfigApp;
        SDA_CONFIG_APP_USER currentConfigAppUser;
        ConfigADO _ConfigADO;
        string keyDoNotShowExpendMaterial = HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_DO_NOT_SHOW_EXPEND_MATERIAL) ?? "";

        string FullTemplateFileName;
        List<DocumentTrackingADO> documentTrackingADOs;
        List<V_EMR_DOCUMENT> listEmrDocument = new List<V_EMR_DOCUMENT>();
        HisTreatmentBedRoomLViewFilter DataTransferTreatmentBedRoomFilter { get; set; }
        private bool IsFirstLoadForm { get; set; }
        private bool IsLoad { get; set; }
        public string saveFilePath = "";

        //Task CheckGetDataForPrint { get; set; }
        public frmHisTrackingList()
        {
            InitializeComponent();
        }

        public frmHisTrackingList(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmHisTrackingList(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId, HIS_DHST currentDhst, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                this.currentDhst = currentDhst;
                this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisTrackingList_Load(object sender, EventArgs e)
        {
            try
            {
                RegisterTimer(this.currentModule.ModuleLink, timer1, 888, timer1_Tick);
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
                SetIconFrm();
                InitControlState();
                SetCaptionByLanguageKey();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                dtFromTime.EditValue = null;
                dtToTime.EditValue = DateTime.Now;

                LoadDataTrackingList();
                if (lcgEmrDocument.Expanded)
                {
                    LoadDataForPrint();
                }
                //load danh sách mẫu
                ProcessDataPopupTemplate();

                //Lưu thông tin checkbox ở máy trạm
                LoadConfigHisAcc();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void LoadDataForPrint()
        {
            try
            {
                IsFirstLoadForm = true;
                IsLoad = true;
                _Treatment = new HIS_TREATMENT();

                _TreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();

                //_ServiceReqs = new List<HIS_SERVICE_REQ>();
                dicServiceReqs = new Dictionary<long, HIS_SERVICE_REQ>();

                _SereServs = new List<HIS_SERE_SERV>();
                dicSereServs = new Dictionary<long, List<HIS_SERE_SERV>>();

                //_ExpMests = new List<HIS_EXP_MEST>();
                dicExpMests = new Dictionary<long, HIS_EXP_MEST>();

                _ExpMestMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                dicExpMestMedicines = new Dictionary<long, List<HIS_EXP_MEST_MEDICINE>>();

                _ExpMestMaterials = new List<HIS_EXP_MEST_MATERIAL>();
                dicExpMestMaterials = new Dictionary<long, List<HIS_EXP_MEST_MATERIAL>>();

                dicServiceReqMetys = new Dictionary<long, List<HIS_SERVICE_REQ_METY>>();
                dicServiceReqMatys = new Dictionary<long, List<HIS_SERVICE_REQ_MATY>>();

                //TH
                this._ImpMests_input = new List<HIS_IMP_MEST>();
                this._ImpMestMedis = new List<V_HIS_IMP_MEST_MEDICINE>();
                this._ImpMestMates = new List<V_HIS_IMP_MEST_MATERIAL>();
                this._ImpMestBloods = new List<V_HIS_IMP_MEST_BLOOD>();

                this._SereServExts = new List<HIS_SERE_SERV_EXT>();
                this._SereServRation = new List<V_HIS_SERE_SERV_RATION>();

                //thuốc, vật tư trả lại
                this._MobaImpMests = new List<V_HIS_IMP_MEST_2>();
                this._ImpMestMedicines_TL = new List<V_HIS_IMP_MEST_MEDICINE>();
                this._ImpMestMaterial_TL = new List<V_HIS_IMP_MEST_MATERIAL>();
                this._ImpMestBlood_TL = new List<V_HIS_IMP_MEST_BLOOD>();

                this.ListCares = new List<HIS_CARE>();
                this.ListCareDetails = new List<V_HIS_CARE_DETAIL>();
                this.ListDhst = new List<HIS_DHST>();
                this.ExpMestBltyReq2 = new List<V_HIS_EXP_MEST_BLTY_REQ_2>();

                IsNotShowOutMediAndMate = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.IsNotShowOutMediAndMate") == "1");

                if (this.treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    //danh sach yeu cau
                    bool IncludeMaterial = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MATERIAL) == "1";
                    bool IncludeMoveBackMediMate = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MEDI_MATE_TH) == "1";
                    bool BloodPresOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_CREATE_BLOOD_PRES_OPTION) == "1";
                    TrackingDataInputSDO sdo = new TrackingDataInputSDO();
                    sdo.IncludeMaterial = IncludeMaterial;
                    sdo.TreatmentId = treatmentId;
                    sdo.IncludeMoveBackMediMate = IncludeMoveBackMediMate;
                    sdo.IncludeBloodPres = BloodPresOption;
                    var TrackingDataSDO = await new BackendAdapter(param).GetAsync<HisTrackingDataSDO>("api/HisTracking/GetData", ApiConsumers.MosConsumer, sdo, param);

                    if (TrackingDataSDO != null)
                    {
                        _Treatment = TrackingDataSDO.Treatment;
                        _TreatmentBedRooms = TrackingDataSDO.TreatmentBedRooms;
                        _MobaImpMests = TrackingDataSDO.vImpMests2;
                        _ImpMestMedicines_TL = TrackingDataSDO.vImpMestMedicines;
                        _ImpMestMaterial_TL = TrackingDataSDO.vImpMestMaterials;
                        _ImpMestBlood_TL = TrackingDataSDO.vImpMestBloods;
                        ListDhst = TrackingDataSDO.HisDHSTs;
                        _SereServRation = TrackingDataSDO.vSereServRations;
                        ExpMestBltyReq2 = TrackingDataSDO.vExpMestBityReqs2;
                        ListCares = TrackingDataSDO.HisCares;
                        _SereServExts = TrackingDataSDO.SereServExts;
                        if (TrackingDataSDO.CareDetails != null && TrackingDataSDO.CareDetails.Count > 0)
                            ListCareDetails.AddRange(TrackingDataSDO.CareDetails);

                        #region ProcessList
                        //LIST<HIS_SERVICE_REQ>
                        var _ServiceReqs = TrackingDataSDO.ServiceReqs;
                        foreach (var item in _ServiceReqs)
                        {
                            if (!dicServiceReqs.ContainsKey(item.ID))
                            {
                                dicServiceReqs[item.ID] = new HIS_SERVICE_REQ();
                                dicServiceReqs[item.ID] = item;
                            }
                        }
                        //LIST<HIS_EXP_MEST>
                        var expMestDatas = TrackingDataSDO.ExpMests;
                        foreach (var item in expMestDatas)
                        {
                            if (!dicExpMests.ContainsKey(item.SERVICE_REQ_ID ?? 0))
                            {
                                dicExpMests[item.SERVICE_REQ_ID ?? 0] = new HIS_EXP_MEST();
                                dicExpMests[item.SERVICE_REQ_ID ?? 0] = (item);
                            }
                            else
                                dicExpMests[item.SERVICE_REQ_ID ?? 0] = (item);
                        }
                        if (IsNotShowOutMediAndMate)
                        {
                            //List<HIS_SERVICE_REQ_METY>
                            var metyDatas = TrackingDataSDO.ServiceReqMetys;
                            foreach (var item in metyDatas)
                            {
                                if (!dicServiceReqMetys.ContainsKey(item.SERVICE_REQ_ID))
                                {
                                    dicServiceReqMetys[item.SERVICE_REQ_ID] = new List<HIS_SERVICE_REQ_METY>();
                                    dicServiceReqMetys[item.SERVICE_REQ_ID].Add(item);
                                }
                                else
                                    dicServiceReqMetys[item.SERVICE_REQ_ID].Add(item);
                            }
                            //List<HIS_SERVICE_REQ_MATY>
                            var matyDatas = TrackingDataSDO.ServiceReqMatys;
                            foreach (var item in matyDatas)
                            {
                                if (!dicServiceReqMatys.ContainsKey(item.SERVICE_REQ_ID))
                                {
                                    dicServiceReqMatys[item.SERVICE_REQ_ID] = new List<HIS_SERVICE_REQ_MATY>();
                                    dicServiceReqMatys[item.SERVICE_REQ_ID].Add(item);
                                }
                                else
                                    dicServiceReqMatys[item.SERVICE_REQ_ID].Add(item);
                            }
                        }

                        //List<HIS_SERE_SERV>
                        _SereServs = TrackingDataSDO.SereServs;
                        if (_SereServs != null && _SereServs.Count > 0)
                        {
                            foreach (var item in _SereServs)
                            {
                                if (!dicSereServs.ContainsKey(item.SERVICE_REQ_ID ?? 0))
                                {
                                    dicSereServs[item.SERVICE_REQ_ID ?? 0] = new List<HIS_SERE_SERV>();
                                }

                                dicSereServs[item.SERVICE_REQ_ID ?? 0].Add(item);
                            }
                        }

                        //List<HIS_EXP_MEST_MEDICINE>
                        this._ExpMestMedicines = TrackingDataSDO.ExpMestMedicines;
                        if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                        {
                            var dataGroups = this._ExpMestMedicines.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MEDICINE_TYPE_ID, p.EXP_MEST_ID, p.TUTORIAL }).Select(p => p.ToList()).ToList();
                            foreach (var item in dataGroups)
                            {
                                HIS_EXP_MEST_MEDICINE ado = new HIS_EXP_MEST_MEDICINE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MEDICINE>(ado, item[0]);
                                ado.AMOUNT = item.Sum(p => p.AMOUNT);
                                ado.TH_AMOUNT = item.Sum(p => p.TH_AMOUNT);
                                if (!dicExpMestMedicines.ContainsKey(ado.EXP_MEST_ID ?? 0))
                                {
                                    dicExpMestMedicines[ado.EXP_MEST_ID ?? 0] = new List<HIS_EXP_MEST_MEDICINE>();
                                    dicExpMestMedicines[ado.EXP_MEST_ID ?? 0].Add(ado);
                                }
                                else
                                    dicExpMestMedicines[item[0].EXP_MEST_ID ?? 0].Add(ado);
                            }
                        }

                        //List<HIS_EXP_MEST_MATERIAL>
                        this._ExpMestMaterials = TrackingDataSDO.ExpMestMaterials;
                        if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                        {
                            if (this.keyDoNotShowExpendMaterial == "1")
                                this._ExpMestMaterials = this._ExpMestMaterials.Where(o => o.IS_EXPEND != 1).ToList();

                            var dataGroups = this._ExpMestMaterials.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MATERIAL_TYPE_ID, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                            foreach (var item in dataGroups)
                            {
                                HIS_EXP_MEST_MATERIAL ado = new HIS_EXP_MEST_MATERIAL();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MATERIAL>(ado, item[0]);
                                ado.AMOUNT = item.Sum(p => p.AMOUNT);
                                ado.TH_AMOUNT = item.Sum(p => p.TH_AMOUNT);
                                if (!dicExpMestMaterials.ContainsKey(ado.EXP_MEST_ID ?? 0))
                                {
                                    dicExpMestMaterials[ado.EXP_MEST_ID ?? 0] = new List<HIS_EXP_MEST_MATERIAL>();
                                    dicExpMestMaterials[ado.EXP_MEST_ID ?? 0].Add(ado);
                                }
                                else
                                    dicExpMestMaterials[item[0].EXP_MEST_ID ?? 0].Add(ado);
                            }
                        }
                        IsLoad = false;
                        #endregion

                    }
                    //this.CheckGetDataForPrint = Task.WhenAll(taskall);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisTrackingList.Resources.Lang", typeof(frmHisTrackingList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTemplate.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.btnTemplate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__Search.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.barButton__Search.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__New.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.barButton__New.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__Print.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.barButton__Print.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintDocumentSigned.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.chkPrintDocumentSigned.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSign.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.chkSign.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisTrackingList.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcgEmrDocument.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.lcgEmrDocument.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisTrackingList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void LoadDataTrackingList()
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
                ucPaging1.Init(ucPagingData, param, (int)pageSize, gridControlTrackings);
                GetEmrDocument();
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
                WaitingManager.Show();
                gridControlTrackings.DataSource = null;
                vHisTrackingList = new List<V_HIS_TRACKING>();

                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                MOS.Filter.HisTrackingViewFilter trackingFilter = new MOS.Filter.HisTrackingViewFilter();
                trackingFilter.TREATMENT_ID = this.treatmentId;
                trackingFilter.ORDER_FIELD = "TRACKING_TIME";
                trackingFilter.ORDER_DIRECTION = "DESC";
                if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                {
                    trackingFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                {
                    trackingFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "235959");
                }

                var result = new BackendAdapter(paramCommon).GetRO<List<V_HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GETVIEW, ApiConsumers.MosConsumer, trackingFilter, paramCommon);
                if (result != null)
                {
                    vHisTrackingList = (List<V_HIS_TRACKING>)result.Data;
                    //vHisTrackingList = vHisTrackingList.OrderBy(p => p.TRACKING_TIME).ToList();
                    rowCount = (vHisTrackingList == null ? 0 : vHisTrackingList.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }

                gridControlTrackings.BeginUpdate();
                gridControlTrackings.DataSource = vHisTrackingList;
                gridControlTrackings.EndUpdate();
                gridViewTrackings.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewTrackings.OptionsSelection.EnableAppearanceFocusedRow = false;
                //gridViewTrackings.BestFitColumns();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTrackings_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_TRACKING data = (V_HIS_TRACKING)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data == null)
                        return;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start;
                    }
                    else if (e.Column.FieldName == "TRACKING_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TRACKING_TIME);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME.Value);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME.Value);

                    }
                    else if (e.Column.FieldName == "ICD_MAIN_DISPLAY")
                    {
                        e.Value = data.ICD_NAME;
                    }
                    else if (e.Column.FieldName == "EMR_DOCUMENT_STT_NAME_str")
                    {
                        e.Value = data.EMR_DOCUMENT_STT_NAME;
                    }
                    else if (e.Column.FieldName == "CREATOR_NAME")
                    {
                        var creator = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(p => p.LOGINNAME == data.CREATOR).FirstOrDefault();
                        e.Value = creator != null ? creator.TDL_USERNAME : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTrackings_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    //V_HIS_TRACKING data = (V_HIS_TRACKING)gridViewTrackings.GetRow(e.RowHandle);
                    //if (data == null)
                    //    return;
                    string creator = (gridViewTrackings.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    long DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewTrackings.GetRowCellValue(e.RowHandle, "DEPARTMENT_ID") ?? "0").ToString());
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    long departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                    long? departmentIdCreator = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(p => p.LOGINNAME == loginName).FirstOrDefault().DEPARTMENT_ID;
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
                        if (loginName == creator || CheckLoginAdmin.IsAdmin(loginName) ||
                            (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ConfigKeyss.BtnEdit) != null && departmentId == departmentIdCreator))
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
                LoadDataTrackingList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTrackings_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.RowCell)
                    {
                        long departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        long? departmentIdCreator = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(p => p.LOGINNAME == loginName).FirstOrDefault().DEPARTMENT_ID;
                        if (hi.Column.FieldName == "EDIT")
                        {
                            #region EDIT
                            var row = (V_HIS_TRACKING)gridViewTrackings.GetRow(hi.RowHandle);
                            if (row != null)
                            {
                                var creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                if (creator.Trim() == row.CREATOR.Trim() || CheckLoginAdmin.IsAdmin(loginName)
                                    || (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ConfigKeyss.BtnEdit) != null && departmentId == departmentIdCreator))
                                {
                                    bool isShowMessage = true;
                                    if (!WarningAlreadyExistEmrDocument(row, ref isShowMessage))
                                        return;
                                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                                    if (moduleData == null) throw new Exception("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                    {
                                        HIS_TRACKING ado = new HIS_TRACKING();
                                        Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_TRACKING, MOS.EFMODEL.DataModels.HIS_TRACKING>();
                                        ado = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_TRACKING, MOS.EFMODEL.DataModels.HIS_TRACKING>(row);
                                        List<object> listArgs = new List<object>();
                                        listArgs.Add(ado);
                                        if (DataTransferTreatmentBedRoomFilter != null)
                                            listArgs.Add(DataTransferTreatmentBedRoomFilter);

                                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                                        ((Form)extenceInstance).ShowDialog();

                                        //Load lại tracking
                                        LoadDataTrackingList();

                                        LoadDataForPrint();
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (hi.Column.FieldName == "DELETE")
                        {
                            #region DELETE
                            var row = (V_HIS_TRACKING)gridViewTrackings.GetRow(hi.RowHandle);
                            if (row != null)
                            {
                                var creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                if (creator.Trim() == row.CREATOR.Trim() || CheckLoginAdmin.IsAdmin(loginName))
                                {
                                    bool isShowMessage = true;
                                    if (!WarningAlreadyExistEmrDocument(row, ref isShowMessage))
                                        return;
                                    //if (!isShowMessage || DevExpress.XtraEditors.XtraMessageBox.Show(
                                    //MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong),
                                    //MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                                    //MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    //{

                                    //    //bool success = false;
                                    //    //CommonParam param = new CommonParam();
                                    //    //var rs = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TRACKING_DELETE, ApiConsumers.MosConsumer, row.ID, param);
                                    //    //if (rs)
                                    //    //{
                                    //    //    success = true;
                                    //    //    //Load lại tracking
                                    //    //    LoadDataTrackingList();
                                    //    //}
                                    //    //MessageManager.Show(this.ParentForm, param, success);
                                    //}
                                    bool success = false;
                                    CommonParam param = new CommonParam();
                                    if (HisConfigCFG.Config_TrackingList_CheckServiceReqWhenDeleteTracking == "1")
                                    {
                                        var dataTricking = (V_HIS_TRACKING)gridViewTrackings.GetRow(hi.RowHandle);
                                        if (dataTricking != null)
                                        {
                                            this.vHisTrackingPrint = new List<V_HIS_TRACKING>();
                                            this.vHisTrackingPrint.Add(row);
                                        }
                                        var servicereq = dicServiceReqs.Values.Where(service => vHisTrackingPrint.Any(tracking => tracking.ID == service.TRACKING_ID)
                                        || vHisTrackingPrint.Any(tracking => tracking.ID == service.USED_FOR_TRACKING_ID)).ToList();
                                        if (servicereq.Any())
                                        {

                                            string serviceCodes = string.Join(", ", servicereq.Select(s => s.SERVICE_REQ_CODE));

                                            string message = $"Tờ điều trị có gắn y lệnh {serviceCodes}.";
                                            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }
                                        else
                                        {
                                            var rs = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TRACKING_DELETE, ApiConsumers.MosConsumer, row.ID, param);
                                            if (rs)
                                            {
                                                success = true;
                                                //Load lại tracking
                                                LoadDataTrackingList();
                                            }
                                            MessageManager.Show(this.ParentForm, param, success);
                                        }
                                    }
                                    else if (HisConfigCFG.Config_TrackingList_CheckServiceReqWhenDeleteTracking == "2")
                                    {
                                        var dataTricking = (V_HIS_TRACKING)gridViewTrackings.GetRow(hi.RowHandle);
                                        if (dataTricking != null)     
                                        {
                                            this.vHisTrackingPrint = new List<V_HIS_TRACKING>();
                                            this.vHisTrackingPrint.Add(row);
                                        }

                                        var servicereq = dicServiceReqs.Values.Where(service => vHisTrackingPrint.Any(tracking => tracking.ID == service.TRACKING_ID)
                                        || vHisTrackingPrint.Any(tracking => tracking.ID == service.USED_FOR_TRACKING_ID)).ToList();
                                        if (servicereq.Any())
                                        {
                                            string serviceCodes = string.Join(", ", servicereq.Select(s => s.SERVICE_REQ_CODE));     

                                            string message = $"Tờ điều trị có gắn y lệnh {serviceCodes}. Bạn có muốn tiếp tục?";
                                            DialogResult result = MessageBox.Show(message, "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                                            if (result == DialogResult.Yes)
                                            {
                                                var rs = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TRACKING_DELETE, ApiConsumers.MosConsumer, row.ID, param);
                                                if (rs)
                                                {
                                                    success = true;
                                                    //Load lại tracking
                                                    LoadDataTrackingList();
                                                }
                                                MessageManager.Show(this.ParentForm, param, success);
                                            }
                                        }
                                        else
                                        {
                                            var rs = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TRACKING_DELETE, ApiConsumers.MosConsumer, row.ID, param);
                                            if (rs)
                                            {
                                                success = true;
                                                //Load lại tracking
                                                LoadDataTrackingList();
                                            }
                                            MessageManager.Show(this.ParentForm, param, success);
                                        }
                                    }
                                    else
                                    {
                                        if (!isShowMessage || DevExpress.XtraEditors.XtraMessageBox.Show(
                                            MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong),
                                            MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            
                                            var rs = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TRACKING_DELETE, ApiConsumers.MosConsumer, row.ID, param);
                                            if (rs)
                                            {
                                                success = true;
                                                //Load lại tracking
                                                LoadDataTrackingList();
                                            }
                                            MessageManager.Show(this.ParentForm, param, success);
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (hi.Column.FieldName == "SERVICE_REQ")
                        {
                            #region Add/Remove ServiceReq
                            var row = (V_HIS_TRACKING)gridViewTrackings.GetRow(hi.RowHandle);
                            if (row != null)
                            {
                                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisServiceReqByTracking").FirstOrDefault();
                                if (moduleData == null) throw new Exception("khong tim thay moduleLink = HIS.Desktop.Plugins.HisServiceReqByTracking");
                                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                {
                                    List<object> listArgs = new List<object>();
                                    listArgs.Add(row.ID); ;
                                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                                    ((Form)extenceInstance).ShowDialog();

                                    //Load lại tracking
                                    LoadDataTrackingList();

                                    LoadDataForPrint();
                                }
                            }
                            #endregion
                        }
                        else if (hi.Column.FieldName == "DX$CheckboxSelectorColumn") //tên của cột check chọn
                        {
                            #region Emr document
                            //Inventec.Common.Logging.LogSystem.Info("lcgEmrDocument.Expanded: " + lcgEmrDocument.Expanded);
                            if (lcgEmrDocument.Expanded)
                            {    
                                var row = (V_HIS_TRACKING)gridViewTrackings.GetRow(hi.RowHandle);
                                if (row != null)
                                {
                                    this.vHisTrackingPrint = new List<V_HIS_TRACKING>();
                                    this.vHisTrackingPrint.Add(row);

                                    LoadDataPrint(vHisTrackingPrint);
                                }
                                else
                                {
                                    this.ucViewEmrDocument1.ClearDocument();
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTrackings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (gridViewTrackings.RowCount > 0)
                {
                    vHisTrackingPrint = new List<V_HIS_TRACKING>();
                    for (int i = 0; i < gridViewTrackings.SelectedRowsCount; i++)
                    {
                        if (gridViewTrackings.GetSelectedRows()[i] >= 0)
                        {
                            vHisTrackingPrint.Add((V_HIS_TRACKING)gridViewTrackings.GetRow(gridViewTrackings.GetSelectedRows()[i]));
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Error("timer___STOP");
                    StopTimer(this.currentModule.ModuleLink, timer1);
                    Inventec.Common.Logging.LogSystem.Error("timer___START");
                    StartTimer(this.currentModule.ModuleLink, timer1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void LoadEmrDocument(V_HIS_TRACKING tracking)
        //{
        //    try
        //    {
        //        if (tracking != null)
        //        {
        //            List<V_EMR_DOCUMENT> listData = new List<V_EMR_DOCUMENT>();
        //            string hisCode = "";

        //            hisCode = "HIS_TRACKING:" + tracking.ID;
        //            if (this.listEmrDocument != null && this.listEmrDocument.Count > 0 && !String.IsNullOrWhiteSpace(hisCode))
        //            {
        //                listData = this.listEmrDocument.Where(o => !String.IsNullOrWhiteSpace(o.HIS_CODE) && o.HIS_CODE.Contains(hisCode)).ToList();
        //            }

        //            if ((listData == null || listData.Count <= 0) && !String.IsNullOrWhiteSpace(hisCode))
        //            {
        //                GetEmrDocument();
        //                if (this.listEmrDocument != null && this.listEmrDocument.Count > 0)
        //                {
        //                    listData = this.listEmrDocument.Where(o => !String.IsNullOrWhiteSpace(o.HIS_CODE) && o.HIS_CODE.Contains(hisCode)).ToList();
        //                }
        //            }


        //            if (listData != null && listData.Count > 0)
        //            {
        //                this.ucViewEmrDocument1.ReloadDocument(listData, listData != null && listData.Count > 0);
        //            }
        //            else
        //            {
        //                vHisTrackingPrint = new List<V_HIS_TRACKING>();
        //                vHisTrackingPrint.Add(tracking);
        //                LoadDataPrint();
        //            }
        //        }
        //        else
        //        {
        //            this.ucViewEmrDocument1.ClearDocument();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        private void LoadDataPrint(List<V_HIS_TRACKING> trackingSelecteds = null)
        {
            //Thread thread = null;
            try
            {
                trackingSelecteds = trackingSelecteds.OrderBy(o => o.TRACKING_TIME).ToList();
                if (trackingSelecteds != null && trackingSelecteds.Count > 0)//!String.IsNullOrWhiteSpace(FullTemplateFileName) && 
                {
                    //không get lại khi in. get lại khi tìm kiếm rồi
                    //GetEmrDocument();

                    WaitingManager.Show();
                    List<V_HIS_TRACKING> trackingSelectedNotHasDocument = new List<V_HIS_TRACKING>();
                    List<V_HIS_TRACKING> trackingSelectedHasDocument = new List<V_HIS_TRACKING>();

                    DisposeMemoryStream(documentTrackingADOs);
                    documentTrackingADOs = null;
                    documentTrackingADOs = new List<DocumentTrackingADO>();

                    if (this.IsLoad)
                    {
                        StopTimer(this.currentModule.ModuleLink, timer1);
                        MessageBox.Show("Dữ liệu dịch vụ của hồ sơ chưa tải xong chưa thể xem trước nội dung vui lòng thử lại sau ít phút.");
                        return;
                    }


                    //thread = new System.Threading.Thread(new System.Threading.ThreadStart(MessageThread));
                    //thread.Start();
                    foreach (var tracking in trackingSelecteds)
                    {
                        string hisCode = "HIS_TRACKING:" + tracking.ID;
                        if (this.listEmrDocument != null
                            && this.listEmrDocument.Count > 0
                            && !String.IsNullOrWhiteSpace(hisCode)
                            && this.listEmrDocument.Exists(o => !String.IsNullOrWhiteSpace(o.HIS_CODE) && o.HIS_CODE.Contains(hisCode))
                            )
                        {
                            trackingSelectedHasDocument.Add(tracking);
                        }
                        else
                        {
                            trackingSelectedNotHasDocument.Add(tracking);
                        }
                    }

                    if (trackingSelectedNotHasDocument != null && trackingSelectedNotHasDocument.Count > 0)
                    {
                        //if (this.IsLoad)
                        //{
                        //    StopTimer(this.currentModule.ModuleLink, timer1);
                        //    MessageBox.Show("Dữ liệu dịch vụ của hồ sơ chưa tải xong chưa thể xem trước nội dung vui lòng thử lại sau ít phút.");
                        //    return;
                        //}

                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("trackingSelectedNotHasDocument.Count", trackingSelectedNotHasDocument.Count));
                        List<TrackingListADO> listTrackingMps = new List<TrackingListADO>();

                        int findTracking = 0;
                        TrackingListADO _TrackingListADO = new TrackingListADO();
                        _TrackingListADO.Trackings = new List<V_HIS_TRACKING>();

                        //sắp xếp theo thứ tự thời gian tăng dần
                        trackingSelectedNotHasDocument = trackingSelectedNotHasDocument.OrderBy(o => o.TRACKING_TIME).ToList();
                        foreach (var trackingNo in trackingSelectedNotHasDocument)
                        {
                            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("findTracking", findTracking)
                            //    + Inventec.Common.Logging.LogUtil.TraceData("listTrackingMps.count", (listTrackingMps != null ? listTrackingMps.Count : 0))
                            //     + Inventec.Common.Logging.LogUtil.TraceData("_TrackingPrintExts.count", (_TrackingListADO.Trackings != null ? _TrackingListADO.Trackings.Count : 0))
                            //    + Inventec.Common.Logging.LogUtil.TraceData("(trackingSelectedNotHasDocument.Count - 1)", (trackingSelectedNotHasDocument.Count - 1))
                            //    + Inventec.Common.Logging.LogUtil.TraceData("trackingSelectedHasDocument.count", (trackingSelectedHasDocument != null ? trackingSelectedHasDocument.Count : 0))
                            //    );
                            bool valid = (
                                trackingSelectedNotHasDocument.Count > 1
                                && findTracking + 1 <= (trackingSelectedNotHasDocument.Count - 1)
                                && (trackingSelectedHasDocument == null || trackingSelectedHasDocument.Count == 0
                                    || !trackingSelectedHasDocument.Exists(k => k.TRACKING_TIME > trackingNo.TRACKING_TIME && k.TRACKING_TIME < trackingSelectedNotHasDocument[findTracking + 1].TRACKING_TIME)));
                            // Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
                            if (valid)
                            {
                                _TrackingListADO.Trackings.Add(trackingNo);
                                findTracking += 1;
                                continue;
                            }
                            else
                            {
                                _TrackingListADO.Trackings.Add(trackingNo);

                                AutoMapper.Mapper.CreateMap<TrackingListADO, TrackingListADO>();
                                TrackingListADO trackingListADO = AutoMapper.Mapper.Map<TrackingListADO>(_TrackingListADO);

                                listTrackingMps.Add(trackingListADO);
                            }

                            _TrackingListADO = new TrackingListADO();
                            _TrackingListADO.Trackings = new List<V_HIS_TRACKING>();
                            findTracking += 1;
                        }
                        List<V_HIS_TRACKING> lstTrackingNoHasDocument = new List<V_HIS_TRACKING>();
                        lstTrackingNoHasDocument = listTrackingMps.SelectMany(o => o.Trackings).ToList();
                        if (trackingSelectedHasDocument != null && trackingSelectedHasDocument.Count > 0)
                        {
                            long timeMin = trackingSelectedHasDocument.First().TRACKING_TIME;
                            long timeMax = trackingSelectedHasDocument.Last().TRACKING_TIME;
                            int count = 0;
                            foreach (var item in trackingSelectedHasDocument)
                            {
                                List<V_HIS_TRACKING> lstrs = new List<V_HIS_TRACKING>();
                                count++;
                                if (count == 1)
                                    lstrs = lstTrackingNoHasDocument.Where(o => o.TRACKING_TIME < timeMin).ToList();
                                if (item.TRACKING_TIME > timeMin)
                                    lstrs = lstTrackingNoHasDocument.Where(o => o.TRACKING_TIME > timeMin && o.TRACKING_TIME < item.TRACKING_TIME).ToList();
                                timeMin = item.TRACKING_TIME;
                                AddDocumentTrackingADOs(lstrs);
                                if (count == trackingSelectedHasDocument.Count)
                                {
                                    lstrs = lstTrackingNoHasDocument.Where(o => o.TRACKING_TIME > timeMax).ToList();
                                    AddDocumentTrackingADOs(lstrs);
                                }
                            }
                        }
                        else
                        {
                            AddDocumentTrackingADOs(lstTrackingNoHasDocument);
                        }

                        //do trong hàm Mps000062 có hide cuối nên cần show lại khi ghép tờ điều trị
                        WaitingManager.Show();
                    }

                    //if (thread != null)
                    //    thread.Abort();
                    if (trackingSelectedHasDocument != null && trackingSelectedHasDocument.Count > 0)
                    {
                        var listHisCodes = trackingSelectedHasDocument.Select(o => "HIS_TRACKING:" + o.ID).ToList();
                        var listData = this.listEmrDocument.Where(o => !String.IsNullOrWhiteSpace(o.HIS_CODE) && listHisCodes.Exists(k => o.HIS_CODE.Contains(k))).ToList();
                        if (listData != null && listData.Count > 0)
                        {
                            var documents = GetEmrDocumentFile(listData.Select(o=>o.ID).ToList(), false, null, null);
                            if (documents != null && documents.Count > 0)
                            {
                                foreach (var item in documents)
                                {
                                    try
                                    {
                                        if (item.Extension.ToLower().Equals("pdf"))
                                        {
                                            var emrDocument = listData.FirstOrDefault(o => o.ID == item.DocumentId);
                                            documentTrackingADOs.Add(new DocumentTrackingADO()
                                            {
                                                DocumentFile = new MemoryStream(Convert.FromBase64String(item.Base64Data)),
                                                TRACKING_TIME = (emrDocument != null && emrDocument.DOCUMENT_TIME.HasValue ? emrDocument.DOCUMENT_TIME.Value : GetTackingTimeFromDocument(emrDocument, vHisTrackingList))
                                            });
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
                    if (documentTrackingADOs != null && documentTrackingADOs.Count > 0)
                    {
                        this.ucViewEmrDocument1.ReloadDocument(documentTrackingADOs, documentTrackingADOs != null && documentTrackingADOs.Count > 0);
                        if(this.ucViewEmrDocument1.frmEmr != null && Form.ActiveForm == this.ucViewEmrDocument1.frmEmr)
                        {
                            this.ucViewEmrDocument1.frmEmr.ucViewEmrDocument1.ReloadDocument(documentTrackingADOs, documentTrackingADOs != null && documentTrackingADOs.Count > 0);
                        }
                    }
                    else
                    {
                        this.ucViewEmrDocument1.ClearDocument();
                    }
                }
                else
                {
                    this.ucViewEmrDocument1.ClearDocument();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                //thread.Abort();
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<EmrDocumentFileSDO> GetEmrDocumentFile(List<long> docIds, bool? IsMerge, bool? IsShowPatientSign, bool? IsShowWatermark)
        {
            CommonParam paramCommon = new CommonParam();
            EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
            var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
            emrFilter.IDs = docIds;
            sdo.EmrDocumentViewFilter = emrFilter;
            sdo.IsMerge = IsMerge;
            sdo.IsShowPatientSign = IsShowPatientSign;
            sdo.IsShowWatermark = IsShowWatermark;
            sdo.IsView = true;
            var  room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
            sdo.RoomCode = room != null ? room.ROOM_CODE : null;
            sdo.DepartmentCode = room != null ? room.DEPARTMENT_CODE : null;
            return new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
        }
        private void MessageThread()
        {
            try
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                                    "Dữ liệu dịch vụ của tờ điều trị chưa tải xong chưa thể xem trước nội dung vui lòng thử lại sau ít phút.",
                                    MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                                    MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddDocumentTrackingADOs(List<V_HIS_TRACKING> item)
        {
            try
            {
                WaitingManager.Show();
                MemoryStream streamFile = new MemoryStream();
                bool result = false;
                Mps000062(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_TO_DIEU_TRI__MPS000062, FullTemplateFileName, ref result, true, streamFile, item);
                if ((streamFile != null && streamFile.Length > 0) || (!String.IsNullOrEmpty(this.saveFilePath) && File.Exists(this.saveFilePath)))
                {
                    streamFile.Position = 0;
                    documentTrackingADOs.Add(new DocumentTrackingADO()
                    {
                        DocumentFile = streamFile,
                        TRACKING_TIME = item[0].TRACKING_TIME,
                        FullTemplateFileName = FullTemplateFileName,
                        saveFilePath = saveFilePath
                    });
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static string GenerateTempFileWithin(string fileName, string _extension = "")
        {
            try
            {
                string extension = !System.String.IsNullOrEmpty(_extension) ? _extension : Path.GetExtension(fileName);
                string pathDic = Path.Combine(Path.Combine(Path.Combine(Inventec.Common.TemplaterExport.ApplicationLocationStore.ApplicationPathLocal, "temp"), DateTime.Now.ToString("ddMMyyyy")), "Templates");
                if (!Directory.Exists(pathDic))
                {
                    Directory.CreateDirectory(pathDic);
                }
                return Path.Combine(pathDic, Guid.NewGuid().ToString() + extension);
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message, exception);
                return System.String.Empty;
            }
        }

        private void DisposeMemoryStream(List<DocumentTrackingADO> documentTrackingADOs)
        {
            try
            {
                if (documentTrackingADOs == null || documentTrackingADOs.Count == 0)
                    return;
                long numberOfDocumentFileDisposed = 0;
                foreach (var item in documentTrackingADOs)
                {
                    if (item != null && item.DocumentFile != null)
                    {
                        item.DocumentFile.Dispose();
                        item.DocumentFile = null;
                        numberOfDocumentFileDisposed++;
                    }
                }
                Inventec.Common.Logging.LogAction.Info("_____DisposeMemoryStream : " + numberOfDocumentFileDisposed + " Document file Disposed");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        long GetTackingTimeFromDocument(V_EMR_DOCUMENT document, List<V_HIS_TRACKING> trackingSelecteds)
        {
            long result = 0;
            try
            {
                var tracking = !String.IsNullOrWhiteSpace(document.HIS_CODE) ? trackingSelecteds.Where(o => document.HIS_CODE.Contains("HIS_TRACKING:" + o.ID)).FirstOrDefault() : null;
                result = tracking != null ? tracking.TRACKING_TIME : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //private void LoadDataPrint1()
        //{
        //    try
        //    {
        //        if (!String.IsNullOrWhiteSpace(FullTemplateFileName) && vHisTrackingPrint != null && vHisTrackingPrint.Count > 0)
        //        {
        //            bool result = false;

        //            _TrackingPrints = new List<HIS_TRACKING>();
        //            foreach (var item in vHisTrackingPrint)
        //            {
        //                HIS_TRACKING ado = new HIS_TRACKING();
        //                AutoMapper.Mapper.CreateMap<V_HIS_TRACKING, HIS_TRACKING>();
        //                ado = AutoMapper.Mapper.Map<V_HIS_TRACKING, HIS_TRACKING>(item);
        //                _TrackingPrints.Add(ado);
        //            }
        //            _TrackingPrints = _TrackingPrints.OrderBy(p => p.TRACKING_TIME).ToList();

        //            MemoryStream streamFile = new MemoryStream();

        //            Mps000062(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_TO_DIEU_TRI__MPS000062, FullTemplateFileName, ref result, true, streamFile);

        //            if (result)
        //            {
        //                this.ucViewEmrDocument1.ReloadDocument(streamFile, FullTemplateFileName.Split('.').Last());
        //            }
        //            else
        //            {
        //                this.ucViewEmrDocument1.ClearDocument();
        //            }
        //        }
        //        else
        //        {
        //            this.ucViewEmrDocument1.ClearDocument();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.treatmentId);
                    if (this.currentDhst != null)
                    {
                        listArgs.Add(this.currentDhst);
                    }
                    if (DataTransferTreatmentBedRoomFilter != null)
                        listArgs.Add(DataTransferTreatmentBedRoomFilter);

                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();

                    //Load lại tracking
                    LoadDataTrackingList();

                    LoadDataForPrint();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsFirstLoadForm)
                {
                    LoadDataForPrint();
                    //if (this.CheckGetDataForPrint != null && this.CheckGetDataForPrint.Status != TaskStatus.RanToCompletion)
                    //{
                    //    WaitingManager.Show();
                    //    this.CheckGetDataForPrint.Wait();
                    //    WaitingManager.Hide();
                    //}
                }

                if (vHisTrackingPrint != null && vHisTrackingPrint.Count > 0)
                {
                    long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS.Desktop.Plugins.HisTrackingList.Config.ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_PRINT_MERGE));

                    _TrackingPrints = new List<V_HIS_TRACKING>();
                    _TrackingPrints.AddRange(vHisTrackingPrint);
                    _TrackingPrints = _TrackingPrints.OrderBy(p => p.TRACKING_TIME).ToList();
                    //Review      
                    if (keyPrintMerge == 1 && _TrackingPrints.Count != 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Khi bật cấu hình in gộp tờ điều trị chỉ được phép chọn 1 bản ghi tờ điều trị để in", "Thông báo");
                        return;
                    }

                    if (this.IsLoad)
                    {
                        MessageBox.Show("Dữ liệu dịch vụ của hồ sơ chưa tải xong chưa thể thực hiện in vui lòng thử lại sau ít phút.");
                        return;
                    }

                    PrintProcess(PrintType.IN_TO_DIEU_TRI);
                    ProcessPrint();
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
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

        private void barButton__Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnPrint.Enabled == false)
                    return;
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region<Bổ sung nghiệp vụ kí văn bản điện tử>

        //Gọi EMR
        public void ERMOpen()
        {
            try
            {
                WaitingManager.Show();
                //Inventec.Common.Logging.LogSystem.Info("Begin PrintServiceReq ERMOpen");
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);

                //InCacPhieuChiDinhProcess(richEditorMain);
                WaitingManager.Hide();
                //Inventec.Common.Logging.LogSystem.Info("End PrintServiceReq ERMOpen");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion

        #region<Lưu trạng thái checkbox vào máy trạm>
        private void ProcessPrint()
        {
            try
            {
                ConfigADO ado = new ConfigADO();

                if (chkSign.Checked)
                {
                    ado.IsSign = "1";
                }

                if (chkPrintDocumentSigned.Checked)
                {
                    ado.IsPrintDocumentSigned = "1";
                }

                if (this._ConfigADO != null && (this._ConfigADO.IsSign != ado.IsSign || this._ConfigADO.IsPrintDocumentSigned != ado.IsPrintDocumentSigned))
                {
                    string value = Newtonsoft.Json.JsonConvert.SerializeObject(ado);

                    //Update cònig
                    SDA_CONFIG_APP_USER configAppUserUpdate = new SDA_CONFIG_APP_USER();
                    configAppUserUpdate.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    configAppUserUpdate.VALUE = value;
                    configAppUserUpdate.CONFIG_APP_ID = _currentConfigApp.ID;
                    if (currentConfigAppUser != null)
                        configAppUserUpdate.ID = currentConfigAppUser.ID;
                    string api = configAppUserUpdate.ID > 0 ? "api/SdaConfigAppUser/Update" : "api/SdaConfigAppUser/Create";
                    CommonParam param = new CommonParam();
                    var UpdateResult = new BackendAdapter(param).Post<SDA_CONFIG_APP_USER>(
                            api, ApiConsumers.SdaConsumer, configAppUserUpdate, param);

                    //if (UpdateResult != null)
                    //{
                    //    success = true;
                    //}

                    //MessageManager.Show(this.ParentForm, param, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadConfigHisAcc()
        {
            try
            {
                CommonParam param = new CommonParam();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SDA.Filter.SdaConfigAppFilter configAppFilter = new SDA.Filter.SdaConfigAppFilter();
                configAppFilter.APP_CODE_ACCEPT = GlobalVariables.APPLICATION_CODE;
                configAppFilter.KEY = "CONFIG_KEY__HIS_PLUGINS_TRACKING_LIST__IS_SIGN_IS_PRINT_DOCUMENT_SIGNED";

                _currentConfigApp = new BackendAdapter(param).Get<List<SDA_CONFIG_APP>>("api/SdaConfigApp/Get", ApiConsumers.SdaConsumer, configAppFilter, param).FirstOrDefault();

                string key = "";
                if (_currentConfigApp != null)
                {
                    key = _currentConfigApp.DEFAULT_VALUE;
                    SDA.Filter.SdaConfigAppUserFilter appUserFilter = new SDA.Filter.SdaConfigAppUserFilter();
                    appUserFilter.LOGINNAME = loginName;
                    appUserFilter.CONFIG_APP_ID = _currentConfigApp.ID;
                    currentConfigAppUser = new BackendAdapter(param).Get<List<SDA_CONFIG_APP_USER>>("api/SdaConfigAppUser/Get", ApiConsumers.SdaConsumer, appUserFilter, param).FirstOrDefault();
                    if (currentConfigAppUser != null)
                    {
                        key = currentConfigAppUser.VALUE;
                    }
                }

                if (!string.IsNullOrEmpty(key))
                {
                    _ConfigADO = (ConfigADO)Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigADO>(key);
                    if (_ConfigADO != null)
                    {
                        if (_ConfigADO.IsSign == "1")
                            chkSign.Checked = true;
                        else
                            chkSign.Checked = false;
                        if (_ConfigADO.IsPrintDocumentSigned == "1")
                            chkPrintDocumentSigned.Checked = true;
                        else
                            chkPrintDocumentSigned.Checked = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSign.Checked == true)
            {
                chkPrintDocumentSigned.Enabled = true;
            }
            else
            {
                chkPrintDocumentSigned.Checked = false;
                chkPrintDocumentSigned.Enabled = false;
            }
        }

        private void layoutControl1_GroupExpandChanged(object sender, DevExpress.XtraLayout.Utils.LayoutGroupEventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                if (lcgEmrDocument.Expanded && !IsFirstLoadForm)
                    LoadDataForPrint();
                string name = e.Group.Name;
                string value = "";

                if (e.Group.Name == lcgEmrDocument.Name)
                {
                    value = lcgEmrDocument.Expanded ? "1" : null;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                // Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = name;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
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
                        if (item.KEY == lcgEmrDocument.Name)
                        {
                            lcgEmrDocument.Expanded = item.VALUE == "1";
                        }
                        else if (item.KEY == btnTemplate.Name)
                        {
                            FullTemplateFileName = item.VALUE;
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

        private void GetEmrDocument()
        {
            try
            {
                this.listEmrDocument = new List<V_EMR_DOCUMENT>();
                if (vHisTrackingList != null && vHisTrackingList.Count > 0 && lcgEmrDocument.Expanded)
                {
                    CommonParam param = new CommonParam();
                    var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
                    emrFilter.TREATMENT_CODE__EXACT = vHisTrackingList.First().TREATMENT_CODE;
                    emrFilter.IS_DELETE = false;
                    emrFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__TRACKING;
                    emrFilter.IS_ACTIVE = IMSys.DbConfig.EMR_RS.COMMON.IS_ACTIVE__TRUE;
                    var documents = new BackendAdapter(param).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, emrFilter, param);
                    if (documents != null && documents.Count > 0)
                    {
                        this.listEmrDocument = documents;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTemplate_Click(object sender, EventArgs e)
        {
            btnTemplate.ShowDropDown();
        }

        DXPopupMenu menuTemplate = new DXPopupMenu();
        private void ProcessDataPopupTemplate()
        {
            try
            {
                List<string> listFileTemplate = GetFileTemplate();

                if (listFileTemplate != null && listFileTemplate.Count > 0)
                {
                    menuTemplate = new DXPopupMenu();
                    foreach (var item in listFileTemplate)
                    {
                        string fileName = System.IO.Path.GetFileName(item);
                        Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(item);
                        Image im = icon.ToBitmap();

                        DXMenuCheckItem itemTrackingList = new DXMenuCheckItem(fileName, String.IsNullOrWhiteSpace(FullTemplateFileName) || item == FullTemplateFileName, im, menuItemCheckChange);
                        itemTrackingList.Tag = item;
                        menuTemplate.Items.Add(itemTrackingList);
                        if (String.IsNullOrWhiteSpace(FullTemplateFileName))
                        {
                            FullTemplateFileName = item;
                        }
                    }

                    btnTemplate.DropDownControl = menuTemplate;
                        ucViewEmrDocument1.SetMenu(menuTemplate);
                    //if (ucViewEmrDocument1 != null)
                    //{
                    //    ucViewEmrDocument1.SetMenu(menuTemplate);
                    //}
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool notUpdateWhileChange;

        private void menuItemCheckChange(object sender, EventArgs e)
        {
            try
            {
                if (sender != null && !notUpdateWhileChange)
                {
                    notUpdateWhileChange = true;
                    var menu = sender as DXMenuCheckItem;
                    FullTemplateFileName = menu.Tag != null ? menu.Tag.ToString() : "";

                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == btnTemplate.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = FullTemplateFileName;
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = btnTemplate.Name;
                        csAddOrUpdate.VALUE = FullTemplateFileName;
                        csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);

                    DXPopupMenu menus = btnTemplate.DropDownControl as DXPopupMenu;
                    foreach (DXMenuCheckItem item in menus.Items)
                    {
                        if (item.Tag != menu.Tag)
                        {
                            item.Checked = false;
                        }
                    }
                    LoadDataPrint(vHisTrackingPrint);


                    //if (vHisTrackingPrint != null && vHisTrackingPrint.Count > 0)
                    //{
                    //    if (vHisTrackingPrint.Count == 1)
                    //    {
                    //        LoadEmrDocument(vHisTrackingPrint.FirstOrDefault());
                    //    }
                    //    else
                    //    {
                    //        LoadDataPrint();
                    //    }
                    //}

                    notUpdateWhileChange = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<string> GetFileTemplate()
        {
            List<string> result = null;
            try
            {
                var folderMps = Path.Combine(Inventec.Common.RichEditor.Base.FileLocalStore.Rootpath, "Mps000062");
                if (Directory.Exists(folderMps))
                {
                    string[] fileEntries = Directory.EnumerateFiles(folderMps, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => (s.EndsWith(".xls") || s.EndsWith(".doc") || s.EndsWith(".xlsx") || s.EndsWith(".repx") || s.EndsWith(".docx"))).ToArray();

                    if (fileEntries != null && fileEntries.Count() > 0)
                    {
                        result = fileEntries.ToList();
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

        private void timer1_Tick()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("timer___STICK");
                if (gridViewTrackings.SelectedRowsCount > 0)
                {
                    btnPrint.Enabled = true;
                    if (lcgEmrDocument.Expanded)
                    {
                        LoadDataPrint(vHisTrackingPrint);
                    }
                }
                else
                {
                    btnPrint.Enabled = false;
                    if (lcgEmrDocument.Expanded)
                    {
                        this.ucViewEmrDocument1.ClearDocument();
                    }
                }
                StopTimer(this.currentModule.ModuleLink, timer1);
                Inventec.Common.Logging.LogSystem.Error("timer___STICK_STOP");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisTrackingList_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DisposeMemoryStream(this.documentTrackingADOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool WarningAlreadyExistEmrDocument(V_HIS_TRACKING tracking, ref bool isShowMessage)
        {
            bool isContinue = true;
            try
            {
                long configKeyAutoDelete = 0;
                configKeyAutoDelete = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_HISTRACKINGLIST_AUTO_DELETE_EMR_DOCUMENT_WHEN_EDIT_OR_DELETE_TRACKING));
                if (configKeyAutoDelete != 1)
                    return isContinue;
                if (tracking != null)
                {
                    string tracking_HIS_CODE = "HIS_TRACKING:" + tracking.ID;
                    CommonParam param = new CommonParam();
                    var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
                    emrFilter.TREATMENT_CODE__EXACT = tracking.TREATMENT_CODE;
                    emrFilter.IS_DELETE = false;
                    emrFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__TRACKING;
                    emrFilter.IS_ACTIVE = IMSys.DbConfig.EMR_RS.COMMON.IS_ACTIVE__TRUE;

                    var documents = new BackendAdapter(param).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, emrFilter, param);
                    if (documents != null && documents.Count > 0)
                    {
                        List<V_EMR_DOCUMENT> listDocumentAlready = documents.Where(o => !string.IsNullOrEmpty(o.HIS_CODE) && o.HIS_CODE.Contains(tracking_HIS_CODE)).ToList();
                        if (listDocumentAlready != null && listDocumentAlready.Count() > 0)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Tờ điều trị đã tồn tại văn bản ký, nếu tiếp tục sẽ tự động xóa văn bản đã ký hiện tại. Bạn có muốn tiếp tục?", "Thông báo", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                isShowMessage = false;
                                WaitingManager.Show();
                                CommonParam paramEmr = new CommonParam();
                                var result = false;
                                foreach (var item in listDocumentAlready)
                                {
                                    result = new BackendAdapter(paramEmr).Post<bool>("api/EmrDocument/Delete", ApiConsumers.EmrConsumer, item.ID, paramEmr);
                                    if (!result && isContinue)
                                    {
                                        isContinue = false;
                                    }
                                }
                                if (paramEmr.Messages.Count > 0)
                                {
                                    paramEmr.Messages = paramEmr.Messages.Distinct().ToList();
                                }
                                WaitingManager.Hide();
                                MessageManager.Show(this, paramEmr, isContinue);
                            }
                            else
                            {
                                isContinue = false;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                isContinue = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isContinue;
        }

        private void gridViewTrackings_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                HIS_TRACKING success = new HIS_TRACKING();
                int selectedIndex = gridViewTrackings.FocusedRowHandle;
                if (e.RowHandle >= 0 && e.Column.FieldName == "SHEET_ORDER")
                {
                    //V_HIS_TRACKING dataRow = (V_HIS_TRACKING)gridViewTrackings.GetRow(e.RowHandle);
                    V_HIS_TRACKING dataRow = (V_HIS_TRACKING)gridViewTrackings.GetFocusedRow();
                    if (dataRow != null)
                    {
                        UpdateTrackingInfoSDO data = new UpdateTrackingInfoSDO();
                        // Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_TRACKING>(data, dataRow);
                        data.TrackingId = dataRow.ID;
                        data.SheetOrder = dataRow.SHEET_ORDER;
                        WaitingManager.Show();
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRACKING>("api/HisTracking/UpdateTrackingInfo", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                        WaitingManager.Hide();
                    }
                }
                gridViewTrackings.FocusedRowHandle = selectedIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
