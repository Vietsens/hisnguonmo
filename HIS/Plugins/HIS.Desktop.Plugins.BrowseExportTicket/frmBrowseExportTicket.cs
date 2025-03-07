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
using ACS.EFMODEL.DataModels;
using AutoMapper;
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.BrowseExportTicket.ADO;
using HIS.Desktop.Plugins.BrowseExportTicket.Config;
using HIS.Desktop.Plugins.BrowseExportTicket.Resources;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Plugins.TestServiceReqExcute.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MOS.TDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BrowseExportTicket
{
    public partial class frmBrowseExportTicket : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long expMestId = 0;
        long requestRoomId = 0;
        long mediStockId = 0;
        long expMestTypeID = 0;
        long idGrid = 1;
        List<long> materialTypeIds;
        List<long> medicineTypeIds;
        List<long> medicineAdoIds;
        List<long> materialAdoIds;
        List<ACS_CONTROL> controlAcs;
        private string BtnExport = "HIS000004";

        int positionHandle = -1;

        List<V_HIS_EXP_MEST_BLTY_REQ_1> listExpMestBlty;
        List<V_HIS_BLOOD> listBlood = new List<V_HIS_BLOOD>();
        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Inventec.Common.SignLibrary.ADO.InputADO();
        Dictionary<string, V_HIS_BLOOD> dicBloodCode = new Dictionary<string, V_HIS_BLOOD>();

        Dictionary<long, V_HIS_BLOOD> dicCurrentBlood = new Dictionary<long, V_HIS_BLOOD>();
        Dictionary<long, V_HIS_BLOOD> dicShowBlood = new Dictionary<long, V_HIS_BLOOD>();

        Dictionary<long, VHisBloodADO> dicBloodAdo = new Dictionary<long, VHisBloodADO>();
        V_HIS_EXP_MEST_BLTY_REQ_1 currentBlty = null;
        List<BloodVolumeADO> bloodVolume;

        HIS_EXP_MEST resultExpMest = null;
        HIS_EXP_MEST ChmsExpMest = new HIS_EXP_MEST();

        bool checkBtnRefresh = true;

        List<V_HIS_MEDICINE_TYPE> _MedicineTypes = new List<V_HIS_MEDICINE_TYPE>();
        List<V_HIS_MATERIAL_TYPE> _MaterialTypes = new List<V_HIS_MATERIAL_TYPE>();

        List<HisMedicineTypeInStockSDO> listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
        List<HisMaterialTypeInStockSDO> listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
        List<HisBloodTypeInStockSDO> listBloodTypeInStock = new List<HisBloodTypeInStockSDO>();
        List<V_HIS_EXP_MEST_METY_REQ> listExpMestMedicine { get; set; }
        List<V_HIS_EXP_MEST_MATY_REQ> listExpMestMaterial { get; set; }
        List<ExpMestMedicineADO> listMedicineADO { get; set; }
        List<ExpMestMaterialADO> listMaterialADO { get; set; }

        List<ExpMestMaterialADO> _MaterialADOReuses { get; set; }
        HisMediStockReplaceSDO replaceSDO = null;

        DelegateSelectData delegateSelectData = null;

        string AllowExportBloodOverRequestCFG = "";
        HisExpMestResultSDO rsSave = null;
        CabinetBaseResultSDO cabinetBaseResultSDO = null;
        V_HIS_EXP_MEST_4 expMest;
        CallApiType callApiType;
        string printerName = "";
        enum CallApiType
        {
            cabinet,
            Other
        }

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.BrowseExportTicket";

        public bool ShowTestResult;
        NumberStyles style;
        internal List<HIS_MACHINE> _Machines { get; set; }
        internal List<HIS_SERVICE_MACHINE> _ServiceMachines { get; set; }
        internal List<HIS_SERE_SERV> lstSereServ { get; set; }
        internal List<HisSereServTeinSDO> lstHisSereServTeinSDO { get; set; }
        internal List<V_HIS_SERVICE_REQ> _ServiceReqs;
        List<V_HIS_TEST_INDEX_RANGE> testIndexRangeAll;

        public frmBrowseExportTicket()
        {
            InitializeComponent();
            HisConfig.LoadConfig();
        }

        public frmBrowseExportTicket(Inventec.Desktop.Common.Modules.Module currentModule, long expMestId, DelegateSelectData _delegateSelectData)
            : base(currentModule)
        {
            InitializeComponent();
            SetIcon();
            try
            {
                this.currentModule = currentModule;
                this.expMestId = expMestId;
                this.requestRoomId = currentModule.RoomId;
                delegateSelectData = _delegateSelectData;
                HisConfig.LoadConfig();
                this.callApiType = CallApiType.Other;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// mở duyệt từ danh sách bổ sung/thu hồi cơ số
        /// </summary>
        /// <param name="currentModule">module</param>
        /// <param name="_expMest">phiếu xuất view 4</param>
        /// <param name="_delegateSelectData">delegate</param>
        public frmBrowseExportTicket(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_EXP_MEST_4 _expMest, DelegateSelectData _delegateSelectData)
            : base(currentModule)
        {
            InitializeComponent();
            SetIcon();
            try
            {
                this.currentModule = currentModule;
                this.expMestId = _expMest.ID;
                this.expMest = _expMest;
                this.callApiType = CallApiType.cabinet;
                this.requestRoomId = currentModule.RoomId;
                delegateSelectData = _delegateSelectData;
                HisConfig.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmBrowseExportTicket_Load(object sender, EventArgs e)
        {
            try
            {
                style = NumberStyles.Any;
                GetConfig();
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
                LoadDataMachine();
                if (this.expMestId > 0)
                    GetExpMestById(this.expMestId);

                LoadListMatyMetyId();
                SetDefaultValue();
                LoadDataToGridLookUp();
                loadDataToGridMedicine();
                loadDataToGridMaterial();

                InitControlState();
                this.LoadDataAutoReplace();
                this.ValidControl();
                FillDataToGridExpMestBlty();
                this.LoadDataBloodAndPatyMediStockId();
                this.ProcessDataBlood();
                this.FillDataToGridBlood();
                this.FillDataToGridExpMestBlood();
                this.SetControlByExpMestBlty();
                frmExpMestBlood_Plus_GridLookup();

                ShowTestResult = HisConfigs.Get<string>("MOS.HIS_SERVISE_REQ.ALLOW_PROCESSING_TEST_SERVICE_REQ_WHEN_APPROVE_BLOOD") == "1";

                HideTestResult();

                if (listExpMestMedicine != null && listExpMestMedicine.Count > 0)
                {
                    this.xtraTabControl1.SelectedTabPageIndex = 0;
                }
                else if (__MATERIAL_BEAN_1s != null && __MATERIAL_BEAN_1s.Count > 0)
                {
                    this.xtraTabControl1.SelectedTabPageIndex = 4;
                }
                else if (listExpMestMaterial != null && listExpMestMaterial.Count > 0)
                {
                    this.xtraTabControl1.SelectedTabPageIndex = 1;
                }
                else if (listExpMestBlty != null && listExpMestBlty.Count > 0)
                {
                    this.xtraTabControl1.SelectedTabPageIndex = 2;
                }
                else
                {
                    this.xtraTabControl1.SelectedTabPageIndex = 0;
                }
                InitMenuToButtonPrint();
                SetDefaultValue();
                EnableControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetExpMestById(long expMestId)
        {
            try
            {
                HisExpMestView4Filter expMestFilter = new HisExpMestView4Filter();
                expMestFilter.ID = expMestId;
                this.expMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_4>>("api/HisExpMest/GetView4", ApiConsumers.MosConsumer, expMestFilter, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HideTestResult()
        {
            try
            {
                if (ShowTestResult)
                {
                    layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableControl()
        {
            try
            {
                cboBloodType.Enabled = !HisConfig.IsNotAllowEditBloodInformation;
                gridLookUpBloodAboCode.Enabled = !HisConfig.IsNotAllowEditBloodInformation;
                gridLookUpBloodRhCode.Enabled = !HisConfig.IsNotAllowEditBloodInformation;
                gridLookUpVolume.Enabled = !HisConfig.IsNotAllowEditBloodInformation;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleColumnBCS()
        {
            try
            {
                // nếu là bù cơ số thì cho phép duyệt thuốc/ vật tư khác
                if (this.ChmsExpMest != null && this.ChmsExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS && HisConfig.IS_ALLOW_REPLACE == "1")
                {
                    gridColumnMedicineReplaceMedicne.Visible = true;
                    gridColumnMedicineReplaceOpen.Visible = true;
                    gridColumnMaterial_ReplaceOpen.Visible = true;
                    gridColMaterialTypeName.Visible = true;
                    gridColumnMedicine_TTAmount.Visible = true;
                    gridColumnMaterial_TTAmount.Visible = true;
                }
                else
                {
                    gridColumnMedicineReplaceMedicne.Visible = false;
                    gridColumnMedicineReplaceOpen.Visible = false;
                    gridColumnMaterial_ReplaceOpen.Visible = false;
                    gridColMaterialTypeName.Visible = false;
                    gridColumnMedicine_TTAmount.Visible = false;
                    gridColumnMaterial_TTAmount.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                btnActualExport.Enabled = false;
                Inventec.Common.Logging.LogSystem.Debug("rsSave2");
                if (this.ChmsExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                {
                    layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    if (this.ChmsExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        btnPrint.Enabled = true;
                    }
                    else
                    {
                        btnPrint.Enabled = false;
                    }
                }
                else
                {
                    layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleColumnMTMuoiAndMTAntiGlobulin()
        {
            try
            {
                // hỉ hiển thị nếu loại xuất của phiếu xuất là đơn máu
                if (this.currentBlty.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                {
                    gridColumn_ExpMestBlood_MTMuoi.Visible = true;
                    gridColumn_ExpMestBlood_MTAntiGlobulin.Visible = true;
                }
                else
                {
                    gridColumn_ExpMestBlood_MTMuoi.Visible = false;
                    gridColumn_ExpMestBlood_MTAntiGlobulin.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataByExpMest()
        {
            try
            {
                if (this.expMest != null && this.expMest.ID > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestMetyReqFilter metyFilter = new HisExpMestMetyReqFilter();
                    metyFilter.EXP_MEST_ID = this.expMest.ID;
                    _DataMetys = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyFilter, param);

                    MOS.Filter.HisExpMestMatyReqFilter matyFilter = new HisExpMestMatyReqFilter();
                    matyFilter.EXP_MEST_ID = this.expMest.ID;
                    _DataMatys = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, metyFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_EXP_MEST chmsExpMest { get; set; }
        List<HIS_MEDICINE> _Medicines { get; set; }
        List<HIS_MATERIAL> _Materials { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HCs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_VTs { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_HCs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _DataMetys { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _DataMatys { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TC = new List<HIS_EXP_MEST_METY_REQ>();

        private void onClickPrint(object sender, EventArgs e)
        {
            try
            {
                LoadDataByExpMest();
                if (
                    !((this._DataMetys != null && this._DataMetys.Count > 0)
                    || (this._DataMatys != null && this._DataMatys.Count > 0))
                    )
                    return;

                #region TT Chung
                WaitingManager.Show();
                chmsExpMest = new V_HIS_EXP_MEST();
                _Medicines = new List<HIS_MEDICINE>();
                _Materials = new List<HIS_MATERIAL>();
                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                _ExpMestMetyReq_HCs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TC = new List<HIS_EXP_MEST_METY_REQ>();

                long _expMestId = this.expMest.ID;
                HisExpMestViewFilter chmsFilter = new HisExpMestViewFilter();
                chmsFilter.ID = _expMestId;
                var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                chmsExpMest = listChmsExpMest.First();

                CommonParam param = new CommonParam();

                long _EXP_MEST_STT_ID = chmsExpMest.EXP_MEST_STT_ID;

                if (_EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                    || _EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                {
                    MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                    mediFilter.EXP_MEST_ID = _expMestId;
                    _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
                    if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                    {
                        List<long> _MedicineIds = _ExpMestMedicines.Select(p => p.MEDICINE_ID ?? 0).ToList();
                        MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = _MedicineIds;
                        _Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                    }

                    MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                    matyFilter.EXP_MEST_ID = _expMestId;
                    _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);
                    if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                    {
                        List<long> _MaterialIds = _ExpMestMaterials.Select(p => p.MATERIAL_ID ?? 0).ToList();
                        MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = _MaterialIds;
                        _Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                    }
                }

                var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);
                bool tc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC);


                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("DataMetys___:", _DataMetys));

                foreach (var item in this._DataMetys)
                {
                    var dataType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                    if (dataType != null)
                    {
                        if (dataType.IS_CHEMICAL_SUBSTANCE == 1)
                        {
                            _ExpMestMetyReq_HCs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                        {
                            _ExpMestMetyReq_GNs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                        {
                            _ExpMestMetyReq_HTs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                        {
                            _ExpMestMetyReq_TDs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                        {
                            _ExpMestMetyReq_PXs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                        {
                            _ExpMestMetyReq_COs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                        {
                            _ExpMestMetyReq_DTs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                        {
                            _ExpMestMetyReq_KSs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                        {
                            _ExpMestMetyReq_LAOs.Add(item);
                        }

                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC && tc)
                        {
                            _ExpMestMetyReq_TC.Add(item);
                        }
                        else
                        {
                            _ExpMestMetyReq_Ts.Add(item);
                        }
                    }
                }

                foreach (var item in this._DataMatys)
                {
                    var dataMaty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                    if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE != null)
                    {
                        _ExpMestMatyReq_HCs.Add(item);
                    }
                    else
                        _ExpMestMatyReq_VTs.Add(item);
                }

                WaitingManager.Hide();
                #endregion

                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                if (this.expMest.CHMS_TYPE_ID == 2)
                {
                    store.RunPrintTemplate("Mps000346", delegatePrintTemplate);
                }
                else if (this.expMest.CHMS_TYPE_ID == 1)
                {
                    store.RunPrintTemplate("Mps000347", delegatePrintTemplate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegatePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode))
                {
                    switch (printTypeCode)
                    {
                        case "Mps000346":
                            Mps000346(ref result, printTypeCode, fileName);
                            break;
                        case "Mps000347":
                            Mps000347(ref result, printTypeCode, fileName);
                            break;
                        case "Mps000421":
                            InPhieuHieuTruyenMauVaChePham(printTypeCode, fileName, ref result, false);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Mps000346(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                #region ---Thuoc Thuong ----
                if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                     chmsExpMest,
                     _ExpMestMedicines,
                     _ExpMestMaterials,
                     _ExpMestMetyReq_Ts,
                     null,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                     _Medicines,
                     _Materials,
                     "THUỐC THƯỜNG"
                      );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---Vat tu Thuong ----
                if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        _ExpMestMaterials,
                        null,
                        _ExpMestMatyReq_VTs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                        _Medicines,
                        _Materials,
                        "VẬT TƯ THƯỜNG"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HuongThan ----
                if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                     chmsExpMest,
                     _ExpMestMedicines,
                     null,
                     _ExpMestMetyReq_HTs,
                     null,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     null,
                     _Medicines,
                     null,
                     "HƯỚNG THẦN"
                      );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- GayNghien ----
                if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_GNs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "GÂY NGHIỆN"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HoaChat ----
                if ((_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0) || (_ExpMestMetyReq_HCs != null && _ExpMestMetyReq_HCs.Count > 0))
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        _ExpMestMaterials,
                        _ExpMestMetyReq_HCs,
                        _ExpMestMatyReq_HCs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                        _Medicines,
                        _Materials,
                        "HÓA CHẤT"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- TD ----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_TDs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "ĐỘC"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- PX ----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_PXs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "PHÓNG XẠ"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- CO ----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_COs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "CORTICOID"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- DT ----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_DTs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "DỊCH TRUYỀN"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- KS ----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_KSs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "KHÁNG SINH"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- LAO ----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_LAOs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "LAO"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ----- TC --------
                if (_ExpMestMetyReq_TC != null && _ExpMestMetyReq_TC.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_TC,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "TIỀN CHẤT"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000347(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                #region ---Thuoc Thuong ----
                if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        _ExpMestMaterials,
                        _ExpMestMetyReq_Ts,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                        _Medicines,
                        _Materials,
                        "THUỐC THƯỜNG",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---Vat tu Thuong ----
                if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        _ExpMestMaterials,
                        null,
                        _ExpMestMatyReq_VTs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                        _Medicines,
                        _Materials,
                        "VẬT TƯ THƯỜNG",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HuongThan ----
                if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_HTs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "HƯỚNG THẦN",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- GayNghien ----
                if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_GNs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "GÂY NGHIỆN",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HoaChat ----
                if ((_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0) || (_ExpMestMetyReq_HCs != null && _ExpMestMetyReq_HCs.Count > 0))
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        _ExpMestMaterials,
                        _ExpMestMetyReq_HCs,
                        _ExpMestMatyReq_HCs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                        _Medicines,
                        _Materials,
                        "HÓA CHẤT",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- TD ----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_TDs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "ĐỘC",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- PX ----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_PXs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "PHÓNG XẠ",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- CO ----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_COs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "CORTICOID",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- DT ----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_DTs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "DỊCH TRUYỀN",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- KS ----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_KSs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "KHÁNG SINH",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- LAO ----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_LAOs,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "LAO",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- TIỀN CHẤT ----
                if (_ExpMestMetyReq_TC != null && _ExpMestMetyReq_TC.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_TC,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "TIỀN CHẤT",
                        HisConfig.ODER_OPTION
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuHieuTruyenMauVaChePham(string printTypeCode, string fileName, ref bool result, bool isBloodPrintTrans)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                HIS_EXP_MEST data = null;
                List<V_HIS_EXP_MEST_BLOOD> listBloods;
                if (this.rsSave != null)
                {
                    data = this.rsSave.ExpMest;
                }
                else if (this.cabinetBaseResultSDO != null)
                {
                    data = this.cabinetBaseResultSDO.ExpMest;
                }

                if (data != null)
                {
                    ProcessPrint(printTypeCode);

                    MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                    treatmentFilter.ID = data.TDL_TREATMENT_ID;
                    var treatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();

                    HisPatientViewFilter Filter = new HisPatientViewFilter();
                    Filter.ID = data.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, Filter, param).FirstOrDefault();

                    V_HIS_EXP_MEST curExpMest = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(curExpMest, data);
                    listBloods = new List<V_HIS_EXP_MEST_BLOOD>();

                    HisTransfusionSumViewFilter filter = new HisTransfusionSumViewFilter();
                    filter.EXP_MEST_BLOOD_IDs = listBloods.Select(o => o.ID).ToList();
                    List<V_HIS_TRANSFUSION_SUM> v_HIS_TRANSFUSION_SUMs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSFUSION_SUM>>("/api/HisTransfusionSum/GetView", ApiConsumers.MosConsumer, filter, null);

                    HisTransfusionFilter filterTrans = new HisTransfusionFilter();
                    filterTrans.IDs = v_HIS_TRANSFUSION_SUMs.Select(o => o.ID).ToList();
                    List<HIS_TRANSFUSION> hIS_TRANSFUSION = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSFUSION>>("/api/HisTransfusion/GetView", ApiConsumers.MosConsumer, filterTrans, null);

                    if (this.rsSave.ExpBloods != null)
                    {
                        foreach (var item in this.rsSave.ExpBloods)
                        {
                            MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                            bloodFilter.ID = item.ID;
                            var blood = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumer.ApiConsumers.MosConsumer, bloodFilter, new CommonParam()).FirstOrDefault();
                            listBloods.Add(blood);
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listBloods), listBloods));
                    List<V_HIS_EXP_BLTY_SERVICE> ExpBltyService = new List<V_HIS_EXP_BLTY_SERVICE>();
                    HisExpBltyServiceViewFilter BltyServicefilter = new HisExpBltyServiceViewFilter();
                    BltyServicefilter.EXP_MEST_ID = data.ID;
                    ExpBltyService = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_BLTY_SERVICE>>("api/HisExpBltyService/GetView", ApiConsumer.ApiConsumers.MosConsumer, BltyServicefilter, new CommonParam());
                    HisExpMestFilter chmsFilter = new HisExpMestFilter();
                    chmsFilter.ID = expMestId;
                    List<HIS_EXP_MEST> ChmsExpMests = new List<HIS_EXP_MEST>();
                    ChmsExpMests = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>(
                        "api/HisExpMest/Get", ApiConsumers.MosConsumer, chmsFilter, null);
                    WaitingManager.Hide();
                    MPS.Processor.Mps000421.PDO.Mps000421PDO pdo = new MPS.Processor.Mps000421.PDO.Mps000421PDO(
                     treatment,
                     patients,
                     curExpMest,
                     listBloods,
                     ExpBltyService,
                     v_HIS_TRANSFUSION_SUMs,
                     ChmsExpMests,
                     hIS_TRANSFUSION
                     );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;


                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2 || isBloodPrintTrans)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPrint(String printTypeCode)
        {
            try
            {
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.expMest != null ? this.expMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetConfig()
        {
            try
            {
                this.AllowExportBloodOverRequestCFG = HisConfigs.Get<string>("HIS.Desktop.Plugins.BrowseExportTicket.AllowExportBloodOverRequest");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadListMatyMetyId()
        {
            try
            {
                HisExpMestFilter chmsFilter = new HisExpMestFilter();
                chmsFilter.ID = expMestId;
                List<HIS_EXP_MEST> ChmsExpMests = new List<HIS_EXP_MEST>();
                ChmsExpMests = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>(
                    "api/HisExpMest/Get", ApiConsumers.MosConsumer, chmsFilter, null);
                if (ChmsExpMests != null && ChmsExpMests.Count > 0)
                {
                    ChmsExpMest = ChmsExpMests.FirstOrDefault();
                    VisibleColumnBCS();
                    this.txtDescription.Text = ChmsExpMest.DESCRIPTION;
                }

                this.mediStockId = ChmsExpMest.MEDI_STOCK_ID;
                this.expMestTypeID = ChmsExpMest.EXP_MEST_TYPE_ID;
                var stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == ChmsExpMest.MEDI_STOCK_ID);

                HisMediStockExtyFilter extyFilter = new HisMediStockExtyFilter();
                extyFilter.MEDI_STOCK_ID = stock.ID;
                var listMediStockExty = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_STOCK_EXTY>>("api/HisMediStockExty/Get", ApiConsumers.MosConsumer, extyFilter, null).ToList();
                if (listMediStockExty != null && listMediStockExty.Count > 0)
                {
                    var dataFinish = listMediStockExty.FirstOrDefault(o => o.EXP_MEST_TYPE_ID == ChmsExpMest.EXP_MEST_TYPE_ID);
                    if (dataFinish != null && dataFinish.IS_AUTO_EXECUTE == 1)
                    {
                        chkFinish.Checked = true;
                        chkFinish.Enabled = false;

                        if (dataFinish.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                        {
                            chkFinish.Enabled = true;
                        }
                    }
                    else
                    {
                        chkFinish.Checked = false;
                        chkFinish.Enabled = false;
                        lciFinish.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                }
                else
                {
                    chkFinish.Checked = false;
                    chkFinish.Enabled = false;
                    lciFinish.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (stock.IS_GOODS_RESTRICT == 1)
                {
                    List<V_HIS_MEDI_STOCK_MATY> material = new List<V_HIS_MEDI_STOCK_MATY>();
                    List<V_HIS_MEDI_STOCK_METY> medicine = new List<V_HIS_MEDI_STOCK_METY>();
                    HisMediStockMatyViewFilter matyFilter = new HisMediStockMatyViewFilter();
                    matyFilter.MEDI_STOCK_ID = stock.ID;
                    material = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK_MATY>>(
                        "api/HisMediStockMaty/GetView", ApiConsumers.MosConsumer, matyFilter, null);
                    material = material.Where(o => o.IS_GOODS_RESTRICT == 1).ToList();
                    if (material != null && material.Count > 0)
                    {
                        materialTypeIds = material.Select(o => o.MATERIAL_TYPE_ID).ToList();
                    }

                    HisMediStockMetyViewFilter metyFilter = new HisMediStockMetyViewFilter();
                    metyFilter.MEDI_STOCK_ID = stock.ID;
                    medicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_STOCK_METY>>(
                       "api/HisMediStockMety/GetView", ApiConsumers.MosConsumer, metyFilter, null);
                    medicine = medicine.Where(o => o.IS_GOODS_RESTRICT == 1).ToList();
                    if (medicine != null && medicine.Count > 0)
                    {
                        medicineTypeIds = medicine.Select(o => o.MEDICINE_TYPE_ID).ToList();
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
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkNotAutoClose.Name)
                        {
                            chkNotAutoClose.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkBloodTransPrint.Name)
                        {
                            chkBloodTransPrint.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkExpiryDate.Name)
                        {
                            chkExpiryDate.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkShowBloodType.Name)
                        {
                            chkShowBloodType.Checked = item.VALUE == "1";
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

        private void LoadDataToGridLookUp()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                V_HIS_MEDI_STOCK medistock = null;
                if (this.ChmsExpMest != null)
                    medistock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.ChmsExpMest.MEDI_STOCK_ID);
                else
                    medistock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ROOM_ID == this.currentModule.RoomId);

                HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                mediFilter.MEDI_STOCK_ID = medistock.ID;
                mediFilter.IS_LEAF = true;
                mediFilter.MEDICINE_TYPE_IDs = medicineTypeIds;
                bool isThuHoiCoSo = (ChmsExpMest != null
                    && ChmsExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK
                    && ChmsExpMest.CHMS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.CHMS_TYPE__ID__REDUCTION);

                if (HisConfig.IS_DONT_PRES_EXPIRED_ITEM && (!isThuHoiCoSo))
                {
                    mediFilter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                }

                listMediTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK, ApiConsumers.MosConsumer, mediFilter, null);

                HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                mateFilter.MEDI_STOCK_ID = medistock.ID;
                mateFilter.IS_LEAF = true;
                if (materialTypeIds != null && materialTypeIds.Count > 0)
                {
                    mateFilter.MATERIAL_TYPE_IDs = materialTypeIds;
                }

                if (HisConfig.IS_DONT_PRES_EXPIRED_ITEM && (!isThuHoiCoSo))
                {
                    mateFilter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                }

                listMateTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, mateFilter, null);
                _MedicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                _MaterialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();

                InitComboMedicine(this.repositoryItemGridMedicineTypeName, _MedicineTypes);
                InitComboMedicine(this.repositoryItemMedicine__ReadOnly, _MedicineTypes);
                InitComboMaterial(this.repositoryItemGridMaterialTypeName, _MaterialTypes);
                InitComboMaterial(this.repositoryItemGrid_Material___ReadOnly, _MaterialTypes);
                InitComboMT(this.repositoryItemMTMuoi);
                InitComboMT(this.repositoryItemMTAntiGlobulin);
                InitComboTCAC(this.repositoryItemTAC);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMedicine(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo, List<V_HIS_MEDICINE_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("AvailableAmount", "", 100, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMaterial(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo, List<V_HIS_MATERIAL_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("AvailableAmount", "", 100, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMT(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo)
        {
            try
            {
                List<MTADO> data = new List<MTADO>();
                data.Add(new MTADO { ID = 5, VALUE = "Âm tính" });
                data.Add(new MTADO { ID = 6, VALUE = "0.5+" });
                data.Add(new MTADO { ID = 1, VALUE = "1+" });
                data.Add(new MTADO { ID = 2, VALUE = "2+" });
                data.Add(new MTADO { ID = 3, VALUE = "3+" });
                data.Add(new MTADO { ID = 4, VALUE = "4+" });
                data.Add(new MTADO { ID = 7, VALUE = "5+" });
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("VALUE", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("VALUE", "ID", columnInfos, false, 155);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTCAC(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo)
        {
            try
            {
                List<TCAC> data = new List<TCAC>();
                data.Add(new TCAC { ID = 0, VALUE = "Âm tính" });
                data.Add(new TCAC { ID = 0.5, VALUE = "0.5+" });
                data.Add(new TCAC { ID = 1, VALUE = "1+" });
                data.Add(new TCAC { ID = 2, VALUE = "2+" });
                data.Add(new TCAC { ID = 3, VALUE = "3+" });
                data.Add(new TCAC { ID = 4, VALUE = "4+" });
                data.Add(new TCAC { ID = 5, VALUE = "5+" });
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("VALUE", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("VALUE", "ID", columnInfos, false, 155);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        // load dữ liệu vào grid thuốc
        private void loadDataToGridMedicine()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                MOS.Filter.HisExpMestMetyReqViewFilter expMestmedicineFilter = new HisExpMestMetyReqViewFilter();
                expMestmedicineFilter.EXP_MEST_ID = expMestId;
                listExpMestMedicine = new List<V_HIS_EXP_MEST_METY_REQ>();
                listExpMestMedicine = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/GetView", ApiConsumers.MosConsumer, expMestmedicineFilter, param);
                listMedicineADO = new List<ExpMestMedicineADO>();
                List<ExpMestMedicineADO> listExpMestMetyADO = new List<ExpMestMedicineADO>();
                foreach (var itemMedicine in listExpMestMedicine)
                {
                    ExpMestMedicineADO ado = new ExpMestMedicineADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineADO>(ado, itemMedicine);
                    if (listMediTypeInStock == null)
                    {
                        listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                    }
                    var dataMedicine = _MedicineTypes.FirstOrDefault(o => o.ID == itemMedicine.MEDICINE_TYPE_ID);

                    var rs = listMediTypeInStock.Where(p => p.Id == itemMedicine.MEDICINE_TYPE_ID).ToList();
                    ado.CURRENT_DD_AMOUNT = ado.DD_AMOUNT ?? 0;
                    if (rs != null && rs.Count > 0)
                    {
                        ado.SUM_IN_STOCK = rs.Sum(s => s.AvailableAmount ?? 0);
                        ado.TON_KHO = rs.Sum(s => s.TotalAmount ?? 0);
                        ado.MEDICINE_TYPE_CODE = rs.First().MedicineTypeCode;
                        ado.CONCENTRA = rs.First().Concentra;
                        ado.IsReplace = false;
                        ado.MEDICINE_TYPE_NAME = rs.First().MedicineTypeName;
                        ado.MEDICINE_TYPE_ID = rs.First().Id;
                        ado.ACTIVE_INGR_BHYT_NAME = rs.First().ActiveIngrBhytName;
                        ado.SERVICE_UNIT_NAME = rs.First().ServiceUnitName;

                        if ((ado.AMOUNT - (ado.DD_AMOUNT ?? 0)) > (ado.SUM_IN_STOCK ?? 0))
                        {
                            ado.YCD_AMOUNT = ado.SUM_IN_STOCK ?? 0;
                        }
                        else
                        {
                            ado.YCD_AMOUNT = ado.AMOUNT - (ado.DD_AMOUNT ?? 0);
                        }

                        ado.isCheck = true;
                        if (ado.DD_AMOUNT == ado.AMOUNT)
                        {
                            ado.isCheck = false;
                        }

                        if (dataMedicine != null && dataMedicine.IS_ALLOW_EXPORT_ODD == 1)
                        {
                            ado.IS_ALLOW_EXPORT_ODD = true;
                        }
                        else
                            ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;
                    }
                    else if (dataMedicine != null)
                    {
                        ado.MEDICINE_TYPE_CODE = dataMedicine.MEDICINE_TYPE_CODE;
                        ado.CONCENTRA = dataMedicine.CONCENTRA;
                        ado.MEDICINE_TYPE_NAME = dataMedicine.MEDICINE_TYPE_NAME;
                        ado.ACTIVE_INGR_BHYT_NAME = dataMedicine.ACTIVE_INGR_BHYT_NAME;
                        ado.SERVICE_UNIT_NAME = dataMedicine.SERVICE_UNIT_NAME;
                    }

                    ado.Action = GlobalVariables.ActionView;
                    listExpMestMetyADO.Add(ado);
                }

                if (listExpMestMetyADO != null && listExpMestMetyADO.Count() > 0)
                    listMedicineADO.AddRange(listExpMestMetyADO);

                List<ExpMestMedicineADO> expMestMetyOtherADOs = new List<ExpMestMedicineADO>();
                if (this.ChmsExpMest != null && this.ChmsExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS && HisConfig.IS_ALLOW_REPLACE == "1")
                {
                    MOS.Filter.HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
                    filter.EXP_MEST_ID = this.ChmsExpMest.ID;
                    List<HIS_EXP_MEST_MEDICINE> expMestMedicineRss = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (expMestMedicineRss != null && expMestMedicineRss.Count() > 0)
                    {
                        foreach (var itemMedicine in expMestMedicineRss)
                        {
                            var reqSdo = listExpMestMetyADO.FirstOrDefault(o => o.ID == itemMedicine.EXP_MEST_METY_REQ_ID);
                            if (reqSdo == null || reqSdo.MEDICINE_TYPE_ID == itemMedicine.TDL_MEDICINE_TYPE_ID)
                                continue;
                            ExpMestMedicineADO ado = new ExpMestMedicineADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineADO>(ado, itemMedicine);
                            ado.DD_AMOUNT = itemMedicine.AMOUNT;
                            ado.AMOUNT = 0;
                            ado.MEDICINE_ID = itemMedicine.MEDICINE_ID;
                            ado.IsApproved = true;
                            reqSdo.DD_AMOUNT = (reqSdo.DD_AMOUNT ?? 0) - itemMedicine.AMOUNT;
                            reqSdo.TT_AMOUNT = (reqSdo.TT_AMOUNT ?? 0) + itemMedicine.AMOUNT;

                            ado.REPLACE_MEDICINE_TYPE_ID = reqSdo.ID;
                            ado.REPLACE_MEDICINE_TYPE_NAME = reqSdo.MEDICINE_TYPE_NAME;

                            if (listMediTypeInStock == null)
                            {
                                listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                            }
                            var dataMedicine = _MedicineTypes.FirstOrDefault(o => itemMedicine.TDL_MEDICINE_TYPE_ID != null && o.ID == itemMedicine.TDL_MEDICINE_TYPE_ID);

                            var rs = listMediTypeInStock.Where(p => itemMedicine.TDL_MEDICINE_TYPE_ID != null && p.Id == itemMedicine.TDL_MEDICINE_TYPE_ID).ToList();
                            if (rs != null && rs.Count > 0)
                            {
                                ado.SUM_IN_STOCK = rs.Sum(s => s.AvailableAmount ?? 0);
                                ado.MEDICINE_TYPE_CODE = rs.First().MedicineTypeCode;
                                ado.CONCENTRA = rs.First().Concentra;
                                ado.IsReplace = true;
                                ado.MEDICINE_TYPE_NAME = rs.First().MedicineTypeName;
                                ado.MEDICINE_TYPE_ID = rs.First().Id;
                                ado.ACTIVE_INGR_BHYT_NAME = rs.First().ActiveIngrBhytName;
                                ado.SERVICE_UNIT_NAME = rs.First().ServiceUnitName;
                                ado.YCD_AMOUNT = 0;
                                ado.isCheck = false;

                                if (dataMedicine != null && dataMedicine.IS_ALLOW_EXPORT_ODD == 1)
                                {
                                    ado.IS_ALLOW_EXPORT_ODD = true;
                                }
                                else
                                    ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;

                            }
                            else if (dataMedicine != null)
                            {
                                ado.MEDICINE_TYPE_CODE = dataMedicine.MEDICINE_TYPE_CODE;
                                ado.CONCENTRA = dataMedicine.CONCENTRA;
                                ado.MEDICINE_TYPE_NAME = dataMedicine.MEDICINE_TYPE_NAME;
                                ado.ACTIVE_INGR_BHYT_NAME = dataMedicine.ACTIVE_INGR_BHYT_NAME;
                                ado.SERVICE_UNIT_NAME = dataMedicine.SERVICE_UNIT_NAME;
                            }
                            ado.Action = GlobalVariables.ActionView;
                            expMestMetyOtherADOs.Add(ado);
                        }
                    }

                    if (expMestMetyOtherADOs != null && expMestMetyOtherADOs.Count() > 0)
                        listMedicineADO.AddRange(expMestMetyOtherADOs);
                }

                if (listMedicineADO != null && listMedicineADO.Count > 0)
                {
                    foreach (var item in listMedicineADO)
                    {
                        var check = expMestMetyOtherADOs != null && expMestMetyOtherADOs.Count() > 0 ? expMestMetyOtherADOs.FirstOrDefault(o => o.REPLACE_MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID) : null;
                        if (check != null)
                        {
                            item.DD_AMOUNT = item.DD_AMOUNT - check.DD_AMOUNT;
                            item.YCD_AMOUNT = item.YCD_AMOUNT - check.YCD_AMOUNT;
                        }
                    }

                    medicineAdoIds = listMedicineADO.Select(o => o.MEDICINE_TYPE_ID).ToList();
                    bindingSource1.DataSource = listMedicineADO;
                    gridControlMedicine.BeginUpdate();
                    gridControlMedicine.DataSource = bindingSource1;
                    gridControlMedicine.EndUpdate();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // load dữ liệu vào grid vật tư
        private void loadDataToGridMaterial()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                MOS.Filter.HisExpMestMatyReqViewFilter expMestMaterialFilter = new HisExpMestMatyReqViewFilter();
                expMestMaterialFilter.EXP_MEST_ID = expMestId;
                listExpMestMaterial = new List<V_HIS_EXP_MEST_MATY_REQ>();
                listExpMestMaterial = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, param);
                listMaterialADO = new List<ExpMestMaterialADO>();

                List<ExpMestMaterialADO> expMestMatyADOs = new List<ExpMestMaterialADO>();
                foreach (var itemMaterial in listExpMestMaterial)
                {
                    ExpMestMaterialADO ado = new ExpMestMaterialADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMaterialADO>(ado, itemMaterial);
                    ado.Action = GlobalVariables.ActionView;
                    if (listMateTypeInStock == null)
                    {
                        listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                    }

                    var rs = listMateTypeInStock.Where(p => p.Id == itemMaterial.MATERIAL_TYPE_ID).ToList();
                    var dataMaterials = _MaterialTypes.FirstOrDefault(o => o.ID == itemMaterial.MATERIAL_TYPE_ID);
                    ado.CURRENT_DD_AMOUNT = ado.DD_AMOUNT ?? 0;
                    if (rs != null && rs.Count() > 0)
                    {
                        ado.SUM_IN_STOCK = rs.Sum(s => s.AvailableAmount ?? 0);
                        ado.TON_KHO = rs.Sum(s => s.TotalAmount ?? 0);
                        ado.MATERIAL_TYPE_CODE = rs.First().MaterialTypeCode;
                        ado.MATERIAL_TYPE_NAME = rs.First().MaterialTypeName;
                        ado.IsReplace = false;
                        ado.SERVICE_UNIT_NAME = rs.First().ServiceUnitName;
                        if ((ado.AMOUNT - (ado.DD_AMOUNT ?? 0)) > (ado.SUM_IN_STOCK ?? 0))
                        {
                            ado.YCD_AMOUNT = (ado.SUM_IN_STOCK ?? 0);
                        }
                        else
                        {
                            ado.YCD_AMOUNT = ado.AMOUNT - (ado.DD_AMOUNT ?? 0);
                        }

                        ado.isCheckMaterial = true;
                        if (dataMaterials != null && dataMaterials.IS_ALLOW_EXPORT_ODD == 1)
                        {
                            ado.IS_ALLOW_EXPORT_ODD = true;
                        }
                        else
                            ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;

                        ado.IS_REUSABLE = dataMaterials != null ? dataMaterials.IS_REUSABLE : null;
                        ado.IS_IDENTITY_MANAGEMENT = dataMaterials != null ? dataMaterials.IS_IDENTITY_MANAGEMENT : null;
                    }
                    else if (dataMaterials != null)
                    {
                        ado.MATERIAL_TYPE_CODE = dataMaterials.MATERIAL_TYPE_CODE;
                        ado.MATERIAL_TYPE_NAME = dataMaterials.MATERIAL_TYPE_NAME;
                        ado.SERVICE_UNIT_NAME = dataMaterials.SERVICE_UNIT_NAME;
                        ado.IS_REUSABLE = dataMaterials.IS_REUSABLE;
                        ado.IS_IDENTITY_MANAGEMENT = dataMaterials.IS_IDENTITY_MANAGEMENT;
                    }

                    expMestMatyADOs.Add(ado);
                }

                if (expMestMatyADOs != null && expMestMatyADOs.Count() > 0)
                    listMaterialADO.AddRange(expMestMatyADOs);

                // nếu là bù cơ số thì cho phép duyệt thuốc/ vật tư khác
                List<ExpMestMaterialADO> expMestMatyOtherADOs = new List<ExpMestMaterialADO>();
                if (this.ChmsExpMest != null && this.ChmsExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS && HisConfig.IS_ALLOW_REPLACE == "1")
                {
                    MOS.Filter.HisExpMestMaterialFilter filter = new HisExpMestMaterialFilter();
                    filter.EXP_MEST_ID = this.ChmsExpMest.ID;
                    List<HIS_EXP_MEST_MATERIAL> expMestMaterialRss = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (expMestMaterialRss != null && expMestMaterialRss.Count() > 0)
                    {
                        foreach (var itemMaterial in expMestMaterialRss)
                        {
                            var reqSdo = expMestMatyADOs.FirstOrDefault(o => o.ID == itemMaterial.EXP_MEST_MATY_REQ_ID);
                            if (reqSdo == null || reqSdo.MATERIAL_TYPE_ID == itemMaterial.TDL_MATERIAL_TYPE_ID)
                                continue;

                            ExpMestMaterialADO ado = new ExpMestMaterialADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMaterialADO>(ado, itemMaterial);
                            ado.DD_AMOUNT = itemMaterial.AMOUNT;
                            ado.AMOUNT = 0;
                            reqSdo.DD_AMOUNT = (reqSdo.DD_AMOUNT ?? 0) - itemMaterial.AMOUNT;
                            reqSdo.TT_AMOUNT = (reqSdo.TT_AMOUNT ?? 0) + itemMaterial.AMOUNT;
                            ado.REPLACE_MATERIAL_TYPE_ID = reqSdo.ID;
                            ado.REPLACE_MATERIAL_TYPE_NAME = reqSdo.MATERIAL_TYPE_NAME;
                            ado.IsApproved = true;
                            ado.Action = GlobalVariables.ActionView;
                            if (listMateTypeInStock == null)
                            {
                                listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                            }

                            var rs = listMateTypeInStock.Where(p => itemMaterial.TDL_MATERIAL_TYPE_ID != null && p.Id == itemMaterial.TDL_MATERIAL_TYPE_ID).ToList();
                            var dataMaterials = _MaterialTypes.FirstOrDefault(o => itemMaterial.TDL_MATERIAL_TYPE_ID != null && o.ID == itemMaterial.TDL_MATERIAL_TYPE_ID);
                            if (rs != null)
                            {
                                ado.SUM_IN_STOCK = rs.Sum(s => s.AvailableAmount ?? 0);
                                ado.MATERIAL_TYPE_CODE = rs.First().MaterialTypeCode;
                                ado.MATERIAL_TYPE_ID = rs.First().Id;
                                ado.MATERIAL_TYPE_NAME = rs.First().MaterialTypeName;
                                ado.IsReplace = true;
                                ado.SERVICE_UNIT_NAME = rs.First().ServiceUnitName;
                                ado.YCD_AMOUNT = 0;
                                ado.isCheckMaterial = false;
                                if (ado.DD_AMOUNT == ado.AMOUNT)
                                {
                                    ado.isCheckMaterial = false;
                                }
                                if (dataMaterials != null && dataMaterials.IS_ALLOW_EXPORT_ODD == 1)
                                {
                                    ado.IS_ALLOW_EXPORT_ODD = true;
                                }
                                else
                                    ado.YCD_AMOUNT = (int)ado.YCD_AMOUNT;
                                ado.IS_REUSABLE = dataMaterials != null ? dataMaterials.IS_REUSABLE : null;
                                ado.IS_IDENTITY_MANAGEMENT = dataMaterials != null ? dataMaterials.IS_IDENTITY_MANAGEMENT : null;
                            }
                            else if (dataMaterials != null)
                            {
                                ado.MATERIAL_TYPE_CODE = dataMaterials.MATERIAL_TYPE_CODE;
                                ado.MATERIAL_TYPE_NAME = dataMaterials.MATERIAL_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = dataMaterials.SERVICE_UNIT_NAME;
                                ado.IS_REUSABLE = dataMaterials.IS_REUSABLE;
                                ado.IS_IDENTITY_MANAGEMENT = dataMaterials.IS_IDENTITY_MANAGEMENT;
                            }

                            expMestMatyOtherADOs.Add(ado);
                        }
                    }

                    if (expMestMatyOtherADOs != null && expMestMatyOtherADOs.Count() > 0)
                        listMaterialADO.AddRange(expMestMatyOtherADOs);
                }

                if (listMaterialADO != null && listMaterialADO.Count > 0)
                {
                    foreach (var item in listMaterialADO)
                    {
                        var check = expMestMatyOtherADOs != null && expMestMatyOtherADOs.Count() > 0 ? expMestMatyOtherADOs.FirstOrDefault(o => o.REPLACE_MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID) : null;
                        if (check != null)
                        {
                            item.DD_AMOUNT = item.DD_AMOUNT - check.DD_AMOUNT;
                            item.YCD_AMOUNT = item.YCD_AMOUNT - check.YCD_AMOUNT;
                        }
                    }

                    _MaterialADOReuses = new List<ExpMestMaterialADO>();
                    var _dataMaterials = listMaterialADO.Where(p => p.IS_REUSABLE != 1 && p.IS_IDENTITY_MANAGEMENT != 1).ToList();
                    _MaterialADOReuses = listMaterialADO.Where(p => p.IS_REUSABLE == 1 || p.IS_IDENTITY_MANAGEMENT == 1).ToList();
                    if (_dataMaterials != null && _dataMaterials.Count > 0)
                    {
                        materialAdoIds = _dataMaterials.Select(o => o.MATERIAL_TYPE_ID).ToList();
                        bindingSource2.DataSource = _dataMaterials;
                        gridControlMaterial.BeginUpdate();
                        gridControlMaterial.DataSource = bindingSource2;
                        gridControlMaterial.EndUpdate();
                    }

                    if (_MaterialADOReuses != null && _MaterialADOReuses.Count > 0)
                    {
                        gridControlMaterialV2.BeginUpdate();
                        gridControlMaterialV2.DataSource = _MaterialADOReuses;
                        gridControlMaterialV2.EndUpdate();

                        InitComboLoaiVatTuTSD(_MaterialADOReuses);

                        __MATERIAL_BEAN_1s = new List<V_HIS_MATERIAL_BEAN_1>();
                        MOS.Filter.HisMaterialBeanView1Filter _beanFilter = new HisMaterialBeanView1Filter();
                        _beanFilter.MATERIAL_TYPE_IDs = _MaterialADOReuses.Select(o => o.MATERIAL_TYPE_ID).ToList();
                        _beanFilter.MEDI_STOCK_ID = this.mediStockId;
                        _beanFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        _beanFilter.IS_REUSABLE__OR__IS_IDENTITY_MANAGEMENT = true;

                        __MATERIAL_BEAN_1s = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MATERIAL_BEAN_1>>("api/HisMaterialBean/GetView1", ApiConsumers.MosConsumer, _beanFilter, null);
                        if (__MATERIAL_BEAN_1s != null && __MATERIAL_BEAN_1s.Count > 0)
                        {
                            __MATERIAL_BEAN_1s = __MATERIAL_BEAN_1s.Where(p => p.IS_ACTIVE == 1 && !string.IsNullOrEmpty(p.SERIAL_NUMBER)).ToList();
                        }

                        gridControlMaterialReuse.DataSource = __MATERIAL_BEAN_1s;
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Load du lieu tu dong thay the
        private void LoadDataAutoReplace()
        {
            try
            {
                if (HisConfig.IS_ALLOW_REPLACE == "1" && HisConfig.IS_AUTO_REPLACE == "1")
                {
                    var medicineSources = (List<ExpMestMedicineADO>)bindingSource1.DataSource;
                    var materialSources = (List<ExpMestMaterialADO>)bindingSource2.DataSource;
                    HisMediStockReplaceSDOFilter filter = new HisMediStockReplaceSDOFilter();
                    filter.MediStockId = this.mediStockId;
                    filter.MaterialTypeIds = materialSources != null ? materialSources.Where(o => !o.IsReplace && !o.IsApproved).Select(s => s.MATERIAL_TYPE_ID).ToList() : null;
                    filter.MedicineTypeIds = medicineSources != null ? medicineSources.Where(o => !o.IsReplace && !o.IsApproved).Select(s => s.MEDICINE_TYPE_ID).ToList() : null;

                    this.replaceSDO = new BackendAdapter(new CommonParam()).Get<HisMediStockReplaceSDO>("api/HisMediStock/GetReplaceSDO", ApiConsumers.MosConsumer, filter, null);

                    if (this.replaceSDO != null)
                    {
                        var lisMediReqs = medicineSources != null ? medicineSources.Where(o => !o.IsReplace && !o.IsApproved).ToList() : null;
                        var lisMateReqs = materialSources != null ? materialSources.Where(o => !o.IsReplace && !o.IsApproved).ToList() : null;

                        if (lisMediReqs != null && lisMediReqs.Count > 0 && replaceSDO.MedicineReplaces != null && replaceSDO.MedicineReplaces.Count > 0)
                        {
                            foreach (var item in lisMediReqs)
                            {
                                decimal needApprove = item.AMOUNT - item.CURRENT_DD_AMOUNT;
                                if (item.IsReplace || item.IsApproved || item.TON_KHO > 0 || needApprove <= 0) continue;
                                L_HIS_EXP_MEST_MEDICINE replace = this.replaceSDO.MedicineReplaces != null ? this.replaceSDO.MedicineReplaces.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID) : null;
                                if (replace == null) continue;
                                var inStock = listMediTypeInStock.FirstOrDefault(p => p.Id == replace.REPLACE_MEDICINE_TYPE_ID);
                                if (inStock == null || (inStock.AvailableAmount ?? 0) <= 0) continue;
                                ExpMestMedicineADO replaceMedicine = new ExpMestMedicineADO();
                                replaceMedicine.REPLACE_MEDICINE_TYPE_ID = item.ID;
                                replaceMedicine.REPLACE_MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                replaceMedicine.MEDICINE_TYPE_CODE = inStock.MedicineTypeCode;
                                replaceMedicine.MEDICINE_TYPE_NAME = inStock.MedicineTypeName;
                                replaceMedicine.MEDICINE_TYPE_ID = inStock.Id;
                                replaceMedicine.ACTIVE_INGR_BHYT_NAME = inStock.ActiveIngrBhytName;
                                replaceMedicine.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                                replaceMedicine.SUM_IN_STOCK = inStock.AvailableAmount;
                                replaceMedicine.isCheck = true;
                                replaceMedicine.IsReplace = true;
                                replaceMedicine.AMOUNT = 0;
                                decimal vailable = (inStock.AvailableAmount ?? 0);
                                if (vailable >= needApprove)
                                {
                                    replaceMedicine.YCD_AMOUNT = needApprove;
                                }
                                else
                                {
                                    replaceMedicine.YCD_AMOUNT = vailable;
                                }
                                // nếu đã tồn tại thuốc thay thế thì remove 
                                medicineSources.RemoveAll(o => o.REPLACE_MEDICINE_TYPE_ID == item.ID && !o.IsApproved);
                                medicineSources.Add(replaceMedicine);

                                item.YCD_AMOUNT = ((item.AMOUNT - replaceMedicine.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMedicine.YCD_AMOUNT));
                                if (item.YCD_AMOUNT <= 0)
                                {
                                    item.isCheck = false;
                                }
                            }
                        }

                        if (lisMateReqs != null && lisMateReqs.Count > 0 && replaceSDO.MaterialReplaces != null && replaceSDO.MaterialReplaces.Count > 0)
                        {
                            foreach (var item in lisMateReqs)
                            {
                                decimal needApprove = item.AMOUNT - item.CURRENT_DD_AMOUNT;
                                if (item.IsReplace || item.IsApproved || item.TON_KHO > 0 || needApprove <= 0) continue;
                                L_HIS_EXP_MEST_MATERIAL replace = this.replaceSDO.MaterialReplaces != null ? this.replaceSDO.MaterialReplaces.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID) : null;
                                if (replace == null) continue;
                                var inStock = listMateTypeInStock.FirstOrDefault(p => p.Id == replace.REPLACE_MATERIAL_TYPE_ID);
                                if (inStock == null || (inStock.AvailableAmount ?? 0) <= 0) continue;
                                ExpMestMaterialADO replaceMaterial = new ExpMestMaterialADO();
                                replaceMaterial.REPLACE_MATERIAL_TYPE_ID = item.ID;
                                replaceMaterial.REPLACE_MATERIAL_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                                replaceMaterial.MATERIAL_TYPE_CODE = inStock.MaterialTypeCode;
                                replaceMaterial.MATERIAL_TYPE_NAME = inStock.ManufacturerName;
                                replaceMaterial.MATERIAL_TYPE_ID = inStock.Id;
                                replaceMaterial.SERVICE_UNIT_NAME = inStock.ServiceUnitName;
                                replaceMaterial.SUM_IN_STOCK = inStock.AvailableAmount;
                                replaceMaterial.isCheckMaterial = true;
                                replaceMaterial.IsReplace = true;
                                replaceMaterial.AMOUNT = 0;
                                decimal vailable = (inStock.AvailableAmount ?? 0);
                                if (vailable >= needApprove)
                                {
                                    replaceMaterial.YCD_AMOUNT = needApprove;
                                }
                                else
                                {
                                    replaceMaterial.YCD_AMOUNT = vailable;
                                }
                                // nếu đã tồn tại thuốc thay thế thì remove 
                                materialSources.RemoveAll(o => o.REPLACE_MATERIAL_TYPE_ID == item.ID && !o.IsApproved);
                                materialSources.Add(replaceMaterial);

                                item.YCD_AMOUNT = ((item.AMOUNT - replaceMaterial.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMaterial.YCD_AMOUNT));
                                if (item.YCD_AMOUNT <= 0)
                                {
                                    item.isCheckMaterial = false;
                                }
                            }
                        }

                        listMedicineADO = medicineSources;
                        bindingSource1.DataSource = listMedicineADO;
                        gridControlMedicine.BeginUpdate();
                        gridControlMedicine.DataSource = bindingSource1.DataSource;
                        gridControlMedicine.EndUpdate();

                        listMaterialADO = materialSources;
                        bindingSource2.DataSource = listMaterialADO;
                        gridControlMaterial.BeginUpdate();
                        gridControlMaterial.DataSource = bindingSource2.DataSource;
                        gridControlMaterial.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // load dữ liệu vào grid máu
        private void gridViewMedicine_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExpMestMedicineADO dataRow = (ExpMestMedicineADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "isCheck")
                        {
                            if (dataRow.isCheck == true)
                            {
                                e.Value = true;
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

        private void repositoryItemButton__Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ExpMestMedicineADO add = new ExpMestMedicineADO();
                add.ID_GRID = idGrid++;
                add.Action = GlobalVariables.ActionAdd;
                add.isCheck = false;
                this.listMedicineADO.Add(add);
                gridControlMedicine.DataSource = null;
                gridControlMedicine.DataSource = this.listMedicineADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var row = (ExpMestMedicineADO)gridViewMedicine.GetFocusedRow();
                if (row != null && row.MEDICINE_TYPE_ID > 0)
                {
                    var dataGrid = (List<ExpMestMedicineADO>)gridControlMedicine.DataSource;
                    foreach (var item in dataGrid)
                    {
                        if (item.ID_GRID == row.ID_GRID)
                        {
                            var rs = listMediTypeInStock.FirstOrDefault(p => p.Id == row.MEDICINE_TYPE_ID);
                            item.SUM_IN_STOCK = rs.AvailableAmount;
                            gridControlMedicine.RefreshDataSource();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExpMestMaterialADO dataRow = (ExpMestMaterialADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "isCheckMaterial")
                        {
                            if (dataRow.isCheckMaterial == true)
                            {
                                e.Value = true;
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

        private void repositoryItemButton_Material_Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ExpMestMaterialADO add = new ExpMestMaterialADO();
                add.ID_GRID = idGrid++;
                add.Action = GlobalVariables.ActionAdd;
                this.listMaterialADO.Add(add);
                gridControlMaterial.DataSource = null;
                gridControlMaterial.DataSource = this.listMaterialADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var row = (ExpMestMaterialADO)gridViewMaterial.GetFocusedRow();
                if (row != null && row.MATERIAL_TYPE_ID > 0)
                {
                    var dataGrid = (List<ExpMestMaterialADO>)gridControlMaterial.DataSource;
                    foreach (var item in dataGrid)
                    {
                        if (item.ID_GRID == row.ID_GRID)
                        {
                            var rs = listMediTypeInStock.FirstOrDefault(p => p.Id == row.MATERIAL_TYPE_ID);
                            item.SUM_IN_STOCK = rs.AvailableAmount;
                            gridControlMaterial.RefreshDataSource();
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
                //get data 2 grid bỏ bản ghi nào mà có medicine_type_id null or = 0,, material_type_id cũng thế
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                HisExpMestApproveSDO data = new HisExpMestApproveSDO();
                data.ExpMestId = this.expMestId;
                data.ReqRoomId = this.requestRoomId;
                if (chkFinish.Checked == true)
                {
                    data.IsFinish = true;
                }
                else
                {
                    data.IsFinish = false;
                }

                data.Description = txtDescription.Text;
                data.Materials = new List<ExpMaterialTypeSDO>();
                data.Medicines = new List<ExpMedicineTypeSDO>();
                data.Bloods = new List<ExpBloodSDO>();
                data.SerialNumbers = new List<PresMaterialBySerialNumberSDO>();

                #region//Lay thuoc
                var medicineTypeAdos = bindingSource1.DataSource as List<ExpMestMedicineADO>;
                if (medicineTypeAdos != null && medicineTypeAdos.Count > 0)
                {
                    var medicineTypeGroups = medicineTypeAdos.Where(o => o.MEDICINE_TYPE_ID > 0 && o.isCheck == true)
                        .GroupBy(o => new { o.MEDICINE_TYPE_ID, o.REPLACE_MEDICINE_TYPE_ID });
                    foreach (var medicineTypeGroup in medicineTypeGroups)
                    {
                        ExpMestMedicineADO expMestMedicine = medicineTypeGroup.First();
                        ExpMedicineTypeSDO medicineTypeAdo = new ExpMedicineTypeSDO();
                        medicineTypeAdo.Amount = medicineTypeGroup.Sum(o => o.YCD_AMOUNT);
                        medicineTypeAdo.MedicineTypeId = expMestMedicine.MEDICINE_TYPE_ID;
                        medicineTypeAdo.Description = expMestMedicine.DESCRIPTION;
                        if (expMestMedicine.REPLACE_MEDICINE_TYPE_ID != null && expMestMedicine.REPLACE_MEDICINE_TYPE_ID > 0)
                        {
                            medicineTypeAdo.ExpMestMetyReqId = expMestMedicine.REPLACE_MEDICINE_TYPE_ID;
                        }
                        else if (expMestMedicine.ID > 0)
                        {
                            medicineTypeAdo.ExpMestMetyReqId = expMestMedicine.ID;
                        }

                        data.Medicines.Add(medicineTypeAdo);
                        if (medicineAdoIds != null && (!expMestMedicine.REPLACE_MEDICINE_TYPE_ID.HasValue || expMestMedicine.REPLACE_MEDICINE_TYPE_ID.Value <= 0))
                        {
                            if (medicineAdoIds.Contains(medicineTypeAdo.MedicineTypeId) && medicineTypeAdo.Amount > (expMestMedicine.AMOUNT - (expMestMedicine.CURRENT_DD_AMOUNT + expMestMedicine.CURRENT_YC_AMOUNT)))
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng thuốc lớn hơn yêu cầu: {0}", expMestMedicine.MEDICINE_TYPE_NAME), "Thông báo");
                                return;
                            }
                        }

                        if (medicineTypeAdo.Amount > expMestMedicine.SUM_IN_STOCK)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng thuốc lớn hơn tồn kho: {0}", expMestMedicine.MEDICINE_TYPE_NAME), "Thông báo");
                            return;
                        }

                        if (medicineTypeAdo.Amount <= 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng thuốc phải lớn hơn 0", "Thông báo");
                            return;
                        }
                    }
                }
                #endregion

                #region//lấy vật tư
                var materialTypeAdos = bindingSource2.DataSource as List<ExpMestMaterialADO>;
                if (materialTypeAdos != null && materialTypeAdos.Count > 0)
                {
                    var materialTypeGroups = materialTypeAdos.Where(o => o.MATERIAL_TYPE_ID > 0 && o.isCheckMaterial == true)
                        .GroupBy(o => new { o.MATERIAL_TYPE_ID, o.REPLACE_MATERIAL_TYPE_ID });
                    foreach (var materialTypeGroup in materialTypeGroups)
                    {
                        ExpMestMaterialADO expMestMaterial = materialTypeGroup.First();
                        ExpMaterialTypeSDO materialTypeAdo = new ExpMaterialTypeSDO();
                        materialTypeAdo.Amount = materialTypeGroup.Sum(o => o.YCD_AMOUNT);
                        materialTypeAdo.MaterialTypeId = expMestMaterial.MATERIAL_TYPE_ID;
                        materialTypeAdo.Description = expMestMaterial.DESCRIPTION;
                        if (expMestMaterial.REPLACE_MATERIAL_TYPE_ID != null && expMestMaterial.REPLACE_MATERIAL_TYPE_ID > 0)
                        {
                            materialTypeAdo.ExpMestMatyReqId = expMestMaterial.REPLACE_MATERIAL_TYPE_ID;
                        }
                        if (expMestMaterial.ID > 0)
                        {
                            materialTypeAdo.ExpMestMatyReqId = expMestMaterial.ID;
                        }

                        data.Materials.Add(materialTypeAdo);
                        if (materialAdoIds != null && (!expMestMaterial.REPLACE_MATERIAL_TYPE_ID.HasValue || expMestMaterial.REPLACE_MATERIAL_TYPE_ID.Value <= 0))
                        {
                            if (materialAdoIds.Contains(materialTypeAdo.MaterialTypeId)
                                && materialTypeAdo.Amount > (expMestMaterial.AMOUNT - (expMestMaterial.CURRENT_DD_AMOUNT + expMestMaterial.CURRENT_YC_AMOUNT)))
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng vật tư lớn hơn yêu cầu: {0}", expMestMaterial.MATERIAL_TYPE_NAME), "Thông báo");
                                return;
                            }
                        }

                        if (materialTypeAdo.Amount > expMestMaterial.SUM_IN_STOCK)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Số lượng vật tư lớn hơn tồn kho: {0}", expMestMaterial.MATERIAL_TYPE_NAME), "Thông báo");
                            return;
                        }

                        if (materialTypeAdo.Amount <= 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng vật tư phải lớn hơn 0", "Thông báo");
                            return;
                        }
                    }
                }
                #endregion

                #region //lấy máu
                List<string> bloodTypeCheck = new List<string>();
                if (listExpMestBlty != null && listExpMestBlty.Count > 0 && listExpMestBlty.Sum(o => o.AMOUNT) >= listExpMestBlty.Sum(o => o.DD_AMOUNT))
                {
                    foreach (var expMestBlty in listExpMestBlty)
                    {
                        var count = dicBloodAdo.Select(o => o.Value).ToList().Where(o => o.ExpMestBltyId == expMestBlty.ID).ToList().Count();
                        if (count <= 0)
                        {
                            bloodTypeCheck.Add(expMestBlty.BLOOD_TYPE_NAME);
                        }
                    }

                    bool valid = false;
                    if (bloodTypeCheck.Count > 0)
                    {
                        WaitingManager.Hide();
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.LoaiMauKhongCoTuiMauNaoTrongDanhSachXuat, bloodTypeCheck.Count, String.Join(",", bloodTypeCheck)), ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        {
                            WaitingManager.Hide();
                            return;
                        }
                        valid = true;
                    }

                    if (!valid && dicBloodAdo.Count < (listExpMestBlty.Sum(s => s.AMOUNT) - listExpMestBlty.Sum(o => o.DD_AMOUNT)))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.SoLuongTuiMauXuatNhoHonSoLuongYeuCau, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        {
                            WaitingManager.Hide();
                            return;
                        }
                    }

                    if (!valid && dicBloodAdo.Count > (listExpMestBlty.Sum(s => s.AMOUNT) - listExpMestBlty.Sum(o => o.DD_AMOUNT)))
                    {
                        WaitingManager.Hide();
                        if (this.AllowExportBloodOverRequestCFG == "1")
                        {
                            if (MessageBox.Show("Số lượng máu lớn hơn yêu cầu, Bạn có muốn tiếp tục ?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return;
                            }
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng máu lớn hơn yêu cầu", "Thông báo");
                            return;
                        }
                    }

                    foreach (var dic in dicBloodAdo)
                    {
                        ExpBloodSDO sdo = new ExpBloodSDO();
                        var bloodType = BackendDataWorker.Get<HIS_BLOOD_TYPE>().Where(o => o.ID == dic.Value.BLOOD_TYPE_ID).First();

                        sdo.PatientBloodAboCode = this.currentBlty.BLOOD_ABO_CODE;
                        sdo.PatientBloodRhCode = this.currentBlty.BLOOD_RH_CODE;
                        if (bloodType.IS_RED_BLOOD_CELLS == 1)
                        {
                            sdo.BloodId = dic.Value.ID;
                            sdo.ExpMestBltyReqId = dic.Value.ExpMestBltyId;
                            sdo.AntiGlobulinEnvi = dic.Value.ANTI_GLOBULIN_ENVI;
                            sdo.SaltEnvi = dic.Value.SALT_ENVI;
                            sdo.AcSelfEnvidence = dic.Value.AC_SELF_ENVIDENCE;
                        }
                        else
                        {
                            sdo.BloodId = dic.Value.ID;
                            sdo.ExpMestBltyReqId = dic.Value.ExpMestBltyId;
                            sdo.AntiGlobulinEnviTwo = dic.Value.ANTI_GLOBULIN_ENVI;
                            sdo.SaltEnviTwo = dic.Value.SALT_ENVI;
                            sdo.AcSelfEnvidenceSecond = dic.Value.AC_SELF_ENVIDENCE;
                        }

                        data.Bloods.Add(sdo);
                    }
                }
                #endregion

                #region//lấy vật tư tái sử dụng
                if (this._MaterialBeanShowAdds != null && this._MaterialBeanShowAdds.Count > 0)
                {
                    var materialTypeGroups = this._MaterialBeanShowAdds.GroupBy(o => o.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var materialTypeGroup in materialTypeGroups)
                    {
                        V_HIS_MATERIAL_BEAN_1 expMestMaterial = materialTypeGroup.First();
                        ExpMaterialTypeSDO materialTypeAdo = new ExpMaterialTypeSDO();

                        materialTypeAdo.Amount = materialTypeGroup.Sum(o => o.AMOUNT);
                        materialTypeAdo.MaterialTypeId = expMestMaterial.MATERIAL_TYPE_ID;

                        var dataMaterial = _MaterialADOReuses.FirstOrDefault(p => p.MATERIAL_TYPE_ID == expMestMaterial.MATERIAL_TYPE_ID);

                        foreach (var itemSr in materialTypeGroup)
                        {
                            PresMaterialBySerialNumberSDO _seri = new PresMaterialBySerialNumberSDO();
                            _seri.SerialNumber = itemSr.SERIAL_NUMBER;
                            _seri.MediStockId = this.mediStockId;
                            if (dataMaterial != null && dataMaterial.ID > 0)
                                _seri.ExpMestMatyReqId = dataMaterial.ID;
                            data.SerialNumbers.Add(_seri);
                        }

                        if (materialTypeAdo.Amount > (dataMaterial.AMOUNT - (dataMaterial.DD_AMOUNT ?? 0)))
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng vật tư lớn hơn yêu cầu", "Thông báo");
                            return;
                        }

                        if (materialTypeAdo.Amount > dataMaterial.SUM_IN_STOCK)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng vật tư lớn hơn tồn kho", "Thông báo");
                            return;
                        }

                        if (materialTypeAdo.Amount <= 0)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng vật tư phải lớn hơn 0", "Thông báo");
                            return;
                        }
                    }
                }
                #endregion

                if (ShowTestResult)
                {
                    #region lưu kết quả
                    int length = Encoding.UTF8.GetByteCount(txtValueRangeIntoPopup.Text);
                    long configKey = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.HIS_DESKTOP_PLUGINS_TEST_CHECKVALUEMAXLENGOPTION));

                    if (gridViewSereServTein.HasColumnErrors)
                        return;


                    data.TestResults = new List<HisTestResultTDO>();
                    if (_ServiceReqs != null && _ServiceReqs.Count() > 0)
                    {
                        foreach (var item in _ServiceReqs)
                        {
                            string mess = "";
                            HisTestResultTDO hisTestResultSDO = new MOS.TDO.HisTestResultTDO();
                            hisTestResultSDO.TestIndexDatas = new List<HisTestIndexResultTDO>();
                            ProcessTestServiceReqExecute(ref hisTestResultSDO, ref mess, item);

                            if (!string.IsNullOrEmpty(mess))
                            {
                                if (configKey == 1)
                                {
                                    string mes_ = "Chỉ số" + mess + "có giá trị vượt quá độ dài cho phép 50 ký tự";
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(mes_, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                                        continue;
                                }
                                else if (configKey == 2)
                                {
                                    string mes_ = "Chỉ số" + mess + " có giá trị vượt quá độ dài cho phép 50 ký tự. Bạn có muốn tiếp tục thực hiện không?";
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(mes_, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                                        continue;
                                }
                            }
                            data.TestResults.Add(hisTestResultSDO);
                        }
                    }



                    //bool valid = true;
                    //valid = valid && (hisTestResultSDO != null);
                    //valid = valid && (hisTestResultSDO.TestIndexDatas != null && hisTestResultSDO.TestIndexDatas.Count > 0);
                    //if (valid)
                    //{
                    //     SaveTestServiceReq(hisTestResultSDO);
                    //}
                    //else
                    //{
                    //    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa nhập giá trị chỉ số", "Thông báo");
                    //}


                    #endregion
                }

                if (data.Bloods.Count == 0
                    && data.Medicines.Count == 0
                    && data.Materials.Count == 0
                    && data.SerialNumbers.Count == 0)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn thuốc, vật tư, máu", "Thông báo");
                    return;
                }

                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Info("Input api/HisExpMest/Approve: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));

                // duyệt từ danh sách bổ sung/ thu hồi cơ số
                if (ChmsExpMest.CHMS_TYPE_ID.HasValue) //#30262
                {
                    this.cabinetBaseResultSDO = new Inventec.Common.Adapter.BackendAdapter(param).Post<CabinetBaseResultSDO>(
                    "api/HisExpMest/BaseApprove", ApiConsumers.MosConsumer, data, param);
                }
                else
                {
                    rsSave = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(
                    "api/HisExpMest/Approve", ApiConsumers.MosConsumer, data, param);
                }

                if (rsSave != null || this.cabinetBaseResultSDO != null)
                {
                    if (!ChmsExpMest.CHMS_TYPE_ID.HasValue && ChmsExpMest.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                        && this.ChmsExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                    {
                        WaitingManager.Hide();
                        this.Close();
                        Inventec.Common.Logging.LogSystem.Debug("Close");
                    }

                    success = true;
                    if (rsSave != null)
                        this.delegateSelectData(rsSave);
                    else
                        this.delegateSelectData(this.cabinetBaseResultSDO);
                    LoadDataToGridLookUp();
                    FillDataToGridExpMestBlty();
                    loadDataToGridMaterial();
                    loadDataToGridMedicine();
                }

                if (success)
                {
                    if (chkBloodTransPrint.Checked && rsSave.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                        store.RunPrintTemplate("Mps000421", delegateProcessPrint);
                    }

                    if (!chkNotAutoClose.Checked)
                    {
                        this.Close();
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                        Inventec.Common.Logging.LogSystem.Debug("Close1");
                    }
                    else
                    {
                        btnSave.Enabled = false;
                        if (this.rsSave.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                             && this.rsSave.ExpMest.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                             && this.rsSave.ExpMest.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT
                            && controlAcs != null
                            && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == this.BtnExport) != null)
                        {
                            btnActualExport.Enabled = true;
                        }
                        if (this.rsSave.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            btnPrint.Enabled = true;
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.rsSave.ExpMest), this.rsSave.ExpMest));
                        Inventec.Common.Logging.LogSystem.Debug("Not Close");
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessTestServiceReqExecute(ref HisTestResultTDO hisTestResultSDO, ref string mess, V_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                List<HisSereServTeinSDO> listSereServ = gridControlSereServTein.DataSource as List<HisSereServTeinSDO>;
                if (listSereServ != null && listSereServ.Count > 0)
                {
                    List<HisSereServTeinSDO> _SereServParents = listSereServ.Where(o => o.IS_PARENT == 1).ToList();

                    if (!string.IsNullOrEmpty(mess))
                    {
                        return;
                    }

                    List<HisSereServTeinSDO> lstSereServTein = listSereServ.Where(o => o.IS_PARENT != 1 || (o.IS_PARENT == 1 && o.HAS_ONE_CHILD == 1)).ToList();
                    if (lstSereServTein != null && lstSereServTein.Count > 0)
                    {
                        foreach (var item in lstSereServTein)
                        {
                            if (String.IsNullOrWhiteSpace(item.VALUE) || item.TDL_SERVICE_REQ_ID != serviceReq.ID)
                                continue;

                            HisTestIndexResultTDO sdo = new HisTestIndexResultTDO();
                            sdo.TestIndexCode = item.TEST_INDEX_CODE.TrimStart();
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
                }
                hisTestResultSDO.SampleTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMddHHmm") + "00");
                hisTestResultSDO.ApproverLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();


                hisTestResultSDO.IsUpdateApprover = true;
                hisTestResultSDO.ServiceReqCode = serviceReq.SERVICE_REQ_CODE;
                hisTestResultSDO.ServiceReqId = serviceReq.ID;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateProcessPrint(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case "Mps000421":
                        InPhieuTruyenMau(ref result, printCode, fileName);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuTruyenMau(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();

                MPS.ProcessorBase.Core.PrintData printData = null;

                HisExpMestBloodViewFilter bloodViewFilter = new HisExpMestBloodViewFilter();
                bloodViewFilter.EXP_MEST_ID = this.expMestId;
                List<V_HIS_EXP_MEST_BLOOD> expMestBloods = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>("/api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, bloodViewFilter, null);

                HisExpBltyServiceViewFilter bltyServiceFilter = new HisExpBltyServiceViewFilter();
                bltyServiceFilter.EXP_MEST_ID = this.expMestId;
                List<V_HIS_EXP_BLTY_SERVICE> expBltyServices = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_BLTY_SERVICE>>("/api/HisExpBltyService/GetView", ApiConsumers.MosConsumer, bltyServiceFilter, null);

                V_HIS_TREATMENT treatment = null;
                V_HIS_PATIENT patients = null;
                HIS_EXP_MEST data = null;
                V_HIS_EXP_MEST curExpMest = new V_HIS_EXP_MEST();

                if (this.rsSave != null)
                {
                    data = this.rsSave.ExpMest;
                }
                else if (this.cabinetBaseResultSDO != null)
                {
                    data = this.cabinetBaseResultSDO.ExpMest;
                }

                if (data != null)
                {

                    HisTreatmentViewFilter treatFilter = new HisTreatmentViewFilter();
                    treatFilter.ID = data.TDL_TREATMENT_ID.Value;
                    List<V_HIS_TREATMENT> lstTreatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("/api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatFilter, null);
                    treatment = lstTreatment != null ? lstTreatment.FirstOrDefault() : null;

                    HisPatientViewFilter Filter = new HisPatientViewFilter();
                    Filter.ID = data.TDL_PATIENT_ID;
                    patients = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, Filter, null).FirstOrDefault();

                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(curExpMest, data);
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.expMest != null ? this.expMest.TDL_TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);

                HisTransfusionSumViewFilter filter = new HisTransfusionSumViewFilter();
                filter.EXP_MEST_BLOOD_IDs = expMestBloods.Select(o => o.ID).ToList();
                List<V_HIS_TRANSFUSION_SUM> v_HIS_TRANSFUSION_SUMs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSFUSION_SUM>>("/api/HisTransfusionSum/GetView", ApiConsumers.MosConsumer, filter, null);

                HisTransfusionFilter filterTrans = new HisTransfusionFilter();
                filterTrans.IDs = v_HIS_TRANSFUSION_SUMs.Select(o => o.ID).ToList();
                List<HIS_TRANSFUSION> hIS_TRANSFUSION = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSFUSION>>("/api/HisTransfusion/GetView", ApiConsumers.MosConsumer, filterTrans, null);

                HisExpMestFilter chmsFilter = new HisExpMestFilter();
                chmsFilter.ID = expMestId;
                List<HIS_EXP_MEST> ChmsExpMests = new List<HIS_EXP_MEST>();
                ChmsExpMests = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>(
                    "api/HisExpMest/Get", ApiConsumers.MosConsumer, chmsFilter, null);

                WaitingManager.Hide();
                foreach (var item in expMestBloods)
                {
                    List<V_HIS_EXP_MEST_BLOOD> list = new List<V_HIS_EXP_MEST_BLOOD>();
                    list.Add(item);
                    List<V_HIS_EXP_BLTY_SERVICE> listService = expBltyServices != null ? expBltyServices.ToList() : null;

                    MPS.Processor.Mps000421.PDO.Mps000421PDO mps000421PDO = new MPS.Processor.Mps000421.PDO.Mps000421PDO(
                        treatment,
                        patients,
                        curExpMest,
                        list,
                        listService,
                        v_HIS_TRANSFUSION_SUMs,  
                        ChmsExpMests,
                        hIS_TRANSFUSION);

                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000421PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");

                    if (printData != null)
                    {
                        printData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(printData);
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (ExpMestMedicineADO)gridViewMedicine.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "MEDICINE_TYPE_ID")
                        {
                            if (data.Action == GlobalVariables.ActionAdd)
                                e.RepositoryItem = repositoryItemGridMedicineTypeName;
                            else
                                e.RepositoryItem = repositoryItemMedicine__ReadOnly;
                        }
                        else if (e.Column.FieldName == "add")
                        {
                            if (expMestTypeID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                                e.Column.OptionsColumn.AllowEdit = true;
                        }
                        else if (e.Column.FieldName == "REPLACE_OPEN")
                        {
                            if (data.isCheck && !data.IsReplace)
                                e.RepositoryItem = ButtonEdit_ThayTheEnable;
                            else
                                e.RepositoryItem = ButtonEdit_ThayTheDisable;
                        }
                        else if (e.Column.FieldName == "isCheck")
                        {
                            if (data.IsApproved)
                                e.RepositoryItem = repositoryItemCheckMedicine_Disable;
                            else
                                e.RepositoryItem = repositoryItemCheckEdit_Medicine;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (ExpMestMaterialADO)gridViewMaterial.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "MATERIAL_TYPE_ID")
                        {
                            if (data.Action == GlobalVariables.ActionAdd)
                                e.RepositoryItem = repositoryItemGridMaterialTypeName;
                            else
                                e.RepositoryItem = repositoryItemGrid_Material___ReadOnly;
                        }
                        else if (e.Column.FieldName == "add")
                        {
                            if (this.expMestTypeID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                                e.Column.OptionsColumn.AllowEdit = true;
                        }
                        else if (e.Column.FieldName == "REPLACE_OPEN")
                        {
                            if (data.isCheckMaterial && !data.IsReplace)
                                e.RepositoryItem = ButtonEdit_Material_ThayTheE;
                            else
                                e.RepositoryItem = ButtonEdit_Material_ThayTheD;
                        }
                        else if (e.Column.FieldName == "isCheckMaterial")
                        {
                            if (data.IsApproved)
                                e.RepositoryItem = repositoryItemCheckMaterial_Disable;
                            else
                                e.RepositoryItem = repositoryItemCheckEdit_Material;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridMedicineTypeName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewMedicine.PostEditor();
                if (gridViewMedicine.EditingValue == null)
                    return;
                var data = listMediTypeInStock.FirstOrDefault(o => o.Id == (long)gridViewMedicine.EditingValue);
                if (data != null)
                {
                    if (medicineAdoIds != null)
                    {
                        if (medicineAdoIds.Contains(data.Id))
                        {
                            gridViewMedicine.EditingValue = null;
                            DevExpress.XtraEditors.XtraMessageBox.Show("Loại thuốc này đã yêu cầu", "Thông báo");
                            return;
                        }
                    }

                    gridViewMedicine.SetFocusedRowCellValue("MEDICINE_TYPE_CODE", data.MedicineTypeCode);
                    gridViewMedicine.SetFocusedRowCellValue("SERVICE_UNIT_NAME", data.ServiceUnitName);
                    gridViewMedicine.SetFocusedRowCellValue("SUM_IN_STOCK", data.AvailableAmount);
                    gridViewMedicine.SetFocusedRowCellValue("ACTIVE_INGR_BHYT_NAME", data.ActiveIngrBhytName);
                    gridViewMedicine.SetFocusedRowCellValue("gridColumn1", true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridMaterialTypeName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewMaterial.PostEditor();
                if (gridViewMaterial.EditingValue == null)
                    return;
                var data = listMateTypeInStock.FirstOrDefault(o => o.Id == (long)gridViewMaterial.EditingValue);
                if (data != null)
                {
                    if (materialAdoIds != null)
                    {
                        if (materialAdoIds.Contains(data.Id))
                        {
                            gridViewMaterial.EditingValue = null;
                            DevExpress.XtraEditors.XtraMessageBox.Show("Loại vật tư này đã yêu cầu", "Thông báo");
                            return;
                        }
                    }

                    gridViewMaterial.SetFocusedRowCellValue("MATERIAL_TYPE_CODE", data.MaterialTypeCode);
                    gridViewMaterial.SetFocusedRowCellValue("SERVICE_UNIT_NAME", data.ServiceUnitName);
                    gridViewMaterial.SetFocusedRowCellValue("SUM_IN_STOCK", data.AvailableAmount);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnSave.Enabled)
                    return;
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridMedicineTypeName_Click(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit edit = sender as GridLookUpEdit;
                if (edit != null)
                {
                    edit.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridMaterialTypeName_Click(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit edit = sender as GridLookUpEdit;
                if (edit != null)
                {
                    edit.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }

                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider2_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }

                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (ExpMestMedicineADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if ((data.AMOUNT - (data.DD_AMOUNT ?? 0)) > data.SUM_IN_STOCK || data.SUM_IN_STOCK == null)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }

                        if (data.IsReplace)// thuốc thay thế
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (ExpMestMaterialADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if ((data.AMOUNT - (data.DD_AMOUNT ?? 0)) > data.SUM_IN_STOCK || data.SUM_IN_STOCK == null)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }

                        if (data.IsReplace)// vat tu thay thế
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region button deleteCombo
        private void cboBloodType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBloodType.EditValue = null;
                    fillDataGridViewBlood();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpVolume_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    gridLookUpVolume.EditValue = null;
                    fillDataGridViewBlood();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodAboCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    gridLookUpBloodAboCode.EditValue = null;
                    fillDataGridViewBlood();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodRhCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    gridLookUpBloodRhCode.EditValue = null;
                    fillDataGridViewBlood();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridViewMedicine_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CellValueChanged_1(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (gridViewMedicine.FocusedRowHandle < 0 || gridViewMedicine.FocusedColumn.FieldName != "YCD_AMOUNT")
                    return;
                var data = (ExpMestMedicineADO)((IList)((BaseView)sender).DataSource)[gridViewMedicine.FocusedRowHandle];
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.YCD_AMOUNT <= 0)
                    {
                        valid = false;
                        message = ResourceMessage.SpinEdit__SoLuongDuyetPhaiLonHon0;
                    }
                    else if (data.YCD_AMOUNT > (data.AMOUNT - data.DD_AMOUNT))
                    {
                        valid = false;
                        message = String.Format(ResourceMessage.SoLuongDuyetLonHonSoLuongYeuCau, data.YCD_AMOUNT, (data.AMOUNT - data.DD_AMOUNT));
                    }
                    else if (!data.IS_ALLOW_EXPORT_ODD)
                    {
                        decimal x = Math.Abs(Math.Round(data.YCD_AMOUNT, 3) - Math.Floor(data.YCD_AMOUNT));
                        if (x > 0)
                        {
                            valid = false;
                            message = ResourceMessage.KhongChoPhepDuyetLe;
                        }
                    }
                    if (!valid)
                        gridViewMedicine.SetColumnError(gridViewMedicine.FocusedColumn, message);
                    else
                        gridViewMedicine.ClearColumnErrors();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CellValueChanged_1(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (gridViewMaterial.FocusedRowHandle < 0 || gridViewMaterial.FocusedColumn.FieldName != "YCD_AMOUNT")
                    return;
                var data = (ExpMestMaterialADO)((IList)((BaseView)sender).DataSource)[gridViewMaterial.FocusedRowHandle];
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.YCD_AMOUNT <= 0)
                    {
                        valid = false;
                        message = ResourceMessage.SpinEdit__SoLuongDuyetPhaiLonHon0;
                    }
                    else if (data.YCD_AMOUNT > (data.AMOUNT - data.DD_AMOUNT))
                    {
                        valid = false;
                        message = String.Format(ResourceMessage.SoLuongDuyetLonHonSoLuongYeuCau, data.YCD_AMOUNT, (data.AMOUNT - data.DD_AMOUNT));
                    }
                    else if (!data.IS_ALLOW_EXPORT_ODD)
                    {
                        decimal x = Math.Abs(Math.Round(data.YCD_AMOUNT, 3) - Math.Floor(data.YCD_AMOUNT));
                        if (x > 0)
                        {
                            valid = false;
                            message = ResourceMessage.KhongChoPhepDuyetLe;
                        }
                    }
                    if (!valid)
                        gridViewMaterial.SetColumnError(gridViewMaterial.FocusedColumn, message);
                    else
                        gridViewMedicine.ClearColumnErrors();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //VatTuTSD
        List<V_HIS_MATERIAL_BEAN_1> __MATERIAL_BEAN_1s { get; set; }
        private void gridViewMaterialV2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
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

        ExpMestMaterialADO _MaterialReuse { get; set; }

        private void gridViewMaterialV2_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                _MaterialReuse = new ExpMestMaterialADO();
                this.lblThucXuatCho.Text = "";
                _MaterialReuse = (ExpMestMaterialADO)gridViewMaterialV2.GetFocusedRow();
                if (_MaterialReuse != null)
                {
                    this.lblThucXuatCho.Text = _MaterialReuse.MATERIAL_TYPE_CODE + " - " + _MaterialReuse.MATERIAL_TYPE_NAME;
                    this.cboMaterial.EditValue = _MaterialReuse.MATERIAL_TYPE_ID;
                    this.txtMaterialCode.Text = _MaterialReuse.MATERIAL_TYPE_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboLoaiVatTuTSD(List<ExpMestMaterialADO> datas)
        {
            try
            {
                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 100, 1));
                columnInfo.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 250, 2));
                ControlEditorADO ado = new ControlEditorADO("MATERIAL_TYPE_NAME", "MATERIAL_TYPE_ID", columnInfo, false, 350);
                ControlEditorLoader.Load(cboMaterial, datas, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterialReuse_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
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

        private void txtMaterialCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadMaterialTypeByStr(txtMaterialCode.Text.Trim());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMaterialTypeByStr(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMaterial.Focus();
                    cboMaterial.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MATERIAL>().Where(o => o.MATERIAL_TYPE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMaterial.EditValue = data[0].ID;
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.MATERIAL_TYPE_CODE == searchCode);
                            if (search != null)
                            {
                                cboMaterial.EditValue = search.ID;
                            }
                            else
                            {
                                cboMaterial.EditValue = null;
                                cboMaterial.Focus();
                                cboMaterial.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboMaterial.EditValue = null;
                        cboMaterial.Focus();
                        cboMaterial.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMaterial_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMaterial.EditValue = null;
                    txtMaterialCode.Text = "";
                    txtMaterialCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMaterial_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMaterial.EditValue != null)
                {
                    var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMaterial.EditValue.ToString()));
                    if (data != null)
                    {
                        txtMaterialCode.Text = data.MATERIAL_TYPE_CODE;
                    }
                }

                ReLoadGridMaterialBean("");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<V_HIS_MATERIAL_BEAN_1> _MaterialBeanShows { get; set; }
        List<V_HIS_MATERIAL_BEAN_1> _MaterialBeanShowAdds { get; set; }
        private void ReLoadGridMaterialBean(string str)
        {
            try
            {
                _MaterialBeanShows = new List<V_HIS_MATERIAL_BEAN_1>();
                List<V_HIS_MATERIAL_BEAN_1> datas = new List<V_HIS_MATERIAL_BEAN_1>();
                if (__MATERIAL_BEAN_1s != null && __MATERIAL_BEAN_1s.Count > 0)
                {
                    if (cboMaterial.EditValue != null)
                    {
                        datas = __MATERIAL_BEAN_1s.Where(p => p.MATERIAL_TYPE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMaterial.EditValue.ToString())).ToList();
                    }
                    else
                    {
                        datas = __MATERIAL_BEAN_1s.ToList();
                    }
                }

                if (!string.IsNullOrEmpty(str) && datas != null && datas.Count > 0)
                {
                    datas = datas.Where(p => !string.IsNullOrEmpty(p.SERIAL_NUMBER) && p.SERIAL_NUMBER.Contains(str)).ToList();
                }
                if (_MaterialBeanShowAdds != null && _MaterialBeanShowAdds.Count > 0)
                {
                    datas = datas.Where(o => !_MaterialBeanShowAdds.Exists(p => p.MATERIAL_ID == o.MATERIAL_ID && p.MATERIAL_TYPE_ID == o.MATERIAL_TYPE_ID && p.SERIAL_NUMBER == o.SERIAL_NUMBER)).ToList();
                }
                _MaterialBeanShows.AddRange(datas);
                gridControlMaterialReuse.BeginUpdate();
                gridControlMaterialReuse.DataSource = datas;
                gridControlMaterialReuse.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSeriNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ReLoadGridMaterialBean(txtSeriNumber.Text.Trim());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__AddMaterial_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (this._MaterialReuse == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn loại vật tư", "Thông báo");
                    return;
                }

                var data = (V_HIS_MATERIAL_BEAN_1)gridViewMaterialReuse.GetFocusedRow();
                if (data != null)
                {
                    if (data.MATERIAL_TYPE_ID != this._MaterialReuse.MATERIAL_TYPE_ID)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Loại vật tư không cùng loại đang được chọn",
                   MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }

                    if (_MaterialBeanShowAdds == null)
                        _MaterialBeanShowAdds = new List<V_HIS_MATERIAL_BEAN_1>();
                    var count = _MaterialBeanShowAdds.Where(o => o.MATERIAL_TYPE_ID == data.MATERIAL_TYPE_ID).ToList().Sum(p => p.AMOUNT);
                    if (count >= this._MaterialReuse.AMOUNT)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Đã đủ số lượng yêu cầu", ResourceMessage.TieuDeCuaSoThongBaoLaThongBao);
                        return;
                    }

                    _MaterialBeanShows.Remove(data);
                    _MaterialBeanShowAdds.Add(data);
                    ReloadGridControls();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadGridControls()
        {
            try
            {
                gridControlMaterialBean.BeginUpdate();
                gridControlMaterialBean.DataSource = _MaterialBeanShowAdds;
                gridControlMaterialBean.EndUpdate();

                gridControlMaterialReuse.BeginUpdate();
                gridControlMaterialReuse.DataSource = _MaterialBeanShows;
                gridControlMaterialReuse.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Delete_Reuse_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_HIS_MATERIAL_BEAN_1)gridViewMaterialBean.GetFocusedRow();
                if (data != null)
                {
                    _MaterialBeanShows.Add(data);
                    _MaterialBeanShowAdds.Remove(data);
                    ReloadGridControls();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterialBean_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
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

        private void ButtonEdit_Material_ThayTheE_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                focusMaterial = (ExpMestMaterialADO)gridViewMaterial.GetFocusedRow();
                if (focusMaterial != null)
                {
                    this.ReplaceForm(focusMaterial);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEdit_ThayTheEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                focusMedicine = (ExpMestMedicineADO)gridViewMedicine.GetFocusedRow();
                if (focusMedicine != null)
                {
                    this.ReplaceForm(focusMedicine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        ExpMestMedicineADO focusMedicine;

        private void FillDataAfterSave(object prescription)
        {
            try
            {
                if (prescription != null && prescription is MediMatyTypeADO)
                {
                    var expMestMedicineSelect = (MediMatyTypeADO)prescription;

                    var dataSource = (List<ExpMestMedicineADO>)bindingSource1.DataSource;
                    // thuốc thay thế
                    ExpMestMedicineADO replaceMedicine = new ExpMestMedicineADO();
                    replaceMedicine.REPLACE_MEDICINE_TYPE_ID = focusMedicine.ID;
                    replaceMedicine.REPLACE_MEDICINE_TYPE_NAME = focusMedicine.MEDICINE_TYPE_NAME;
                    replaceMedicine.MEDICINE_TYPE_CODE = expMestMedicineSelect.MedicineTypeCode;
                    replaceMedicine.MEDICINE_TYPE_NAME = expMestMedicineSelect.MedicineTypeName;
                    replaceMedicine.MEDICINE_TYPE_ID = expMestMedicineSelect.Id;
                    replaceMedicine.ACTIVE_INGR_BHYT_NAME = expMestMedicineSelect.ActiveIngrBhytName;
                    replaceMedicine.SERVICE_UNIT_NAME = expMestMedicineSelect.ServiceUnitName;
                    replaceMedicine.SUM_IN_STOCK = expMestMedicineSelect.AvailableAmount;
                    replaceMedicine.isCheck = true;
                    replaceMedicine.IsReplace = true;
                    replaceMedicine.AMOUNT = 0;
                    replaceMedicine.YCD_AMOUNT = expMestMedicineSelect.YCD_AMOUNT;
                    // nếu đã tồn tại thuốc thay thế thì remove 
                    dataSource.RemoveAll(o => o.REPLACE_MEDICINE_TYPE_ID == focusMedicine.ID && !o.IsApproved);
                    dataSource.Add(replaceMedicine);

                    foreach (var item in dataSource)
                    {
                        if (item.ID == focusMedicine.ID)
                        {
                            item.YCD_AMOUNT = ((item.AMOUNT - replaceMedicine.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMedicine.YCD_AMOUNT));
                            item.CURRENT_YC_AMOUNT = replaceMedicine.YCD_AMOUNT;
                        }
                        if (item.YCD_AMOUNT <= 0)
                        {
                            item.isCheck = false;
                        }
                    }

                    listMedicineADO = dataSource;
                    bindingSource1.DataSource = listMedicineADO;
                    gridControlMedicine.BeginUpdate();
                    gridControlMedicine.DataSource = bindingSource1.DataSource;
                    gridControlMedicine.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataAfterSaveMaterial(object prescription)
        {
            try
            {
                if (prescription != null && prescription is MediMatyTypeADO)
                {
                    var expMestMaterialSelect = (MediMatyTypeADO)prescription;

                    var dataSource = (List<ExpMestMaterialADO>)bindingSource2.DataSource;
                    // thuốc thay thế
                    ExpMestMaterialADO replaceMaterial = new ExpMestMaterialADO();
                    replaceMaterial.REPLACE_MATERIAL_TYPE_ID = focusMaterial.ID;
                    replaceMaterial.REPLACE_MATERIAL_TYPE_NAME = focusMaterial.MATERIAL_TYPE_NAME;
                    replaceMaterial.MATERIAL_TYPE_CODE = expMestMaterialSelect.MedicineTypeCode;
                    replaceMaterial.MATERIAL_TYPE_NAME = expMestMaterialSelect.MedicineTypeName;
                    replaceMaterial.MATERIAL_TYPE_ID = expMestMaterialSelect.Id;
                    replaceMaterial.SERVICE_UNIT_NAME = expMestMaterialSelect.ServiceUnitName;
                    replaceMaterial.SUM_IN_STOCK = expMestMaterialSelect.AvailableAmount;
                    replaceMaterial.isCheckMaterial = true;
                    replaceMaterial.IsReplace = true;
                    replaceMaterial.AMOUNT = 0;
                    replaceMaterial.YCD_AMOUNT = expMestMaterialSelect.YCD_AMOUNT;
                    // nếu đã tồn tại thuốc thay thế thì remove 
                    dataSource.RemoveAll(o => o.REPLACE_MATERIAL_TYPE_ID == focusMaterial.ID && !o.IsApproved);
                    dataSource.Add(replaceMaterial);
                    foreach (var item in dataSource)
                    {
                        if (item.ID == focusMaterial.ID)
                        {
                            item.YCD_AMOUNT = ((item.AMOUNT - replaceMaterial.YCD_AMOUNT) >= item.YCD_AMOUNT ? item.YCD_AMOUNT : (item.AMOUNT - replaceMaterial.YCD_AMOUNT));
                            item.CURRENT_YC_AMOUNT = replaceMaterial.YCD_AMOUNT;
                        }
                        if (item.YCD_AMOUNT <= 0)
                        {
                            item.isCheckMaterial = false;
                        }
                    }

                    listMaterialADO = dataSource;
                    bindingSource2.DataSource = listMaterialADO;
                    gridControlMaterial.BeginUpdate();
                    gridControlMaterial.DataSource = bindingSource2.DataSource;
                    gridControlMaterial.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReplaceForm(ExpMestMedicineADO expMestFocus)
        {
            try
            {
                if (this.currentModule == null)
                    return;

                frmReplace frm = new frmReplace(expMestFocus, this.currentModule.RoomId, FillDataAfterSave);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReplaceForm(ExpMestMaterialADO expMestFocus)
        {
            try
            {
                if (this.currentModule == null)
                    return;

                frmReplace frm = new frmReplace(expMestFocus, this.currentModule.RoomId, FillDataAfterSaveMaterial);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        ExpMestMaterialADO focusMaterial;

        private void repositoryItemCheckEdit_Medicine_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewMedicine.PostEditor();
                gridControlMedicine.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckEdit_Material_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewMaterial.PostEditor();
                gridControlMaterial.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPrint_Click(object sender, EventArgs e)
        {
            cboPrint.ShowDropDown();
        }

        internal enum PrintType
        {
            MPS000346,
            MPS000347
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                if (this.expMest != null)
                {
                    if (this.expMest.CHMS_TYPE_ID == 1)
                    {
                        DXMenuItem itemXuatBuCoSoTuTruc = new DXMenuItem("Phiếu bổ sung cơ số", new EventHandler(OnClickInPhieuXuatKho));
                        itemXuatBuCoSoTuTruc.Tag = PrintType.MPS000347;
                        menu.Items.Add(itemXuatBuCoSoTuTruc);
                    }
                    else
                    {
                        DXMenuItem itemXuatBuThuocLe = new DXMenuItem("Phiếu thu hồi cơ số", new EventHandler(OnClickInPhieuXuatKho));
                        itemXuatBuThuocLe.Tag = PrintType.MPS000346;
                        menu.Items.Add(itemXuatBuThuocLe);
                    }

                    cboPrint.DropDownControl = menu;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuXuatKho(object sender, EventArgs e)
        {
            try
            {
                //LoadSpecificExpMest();
                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.MPS000347:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__MPS000347, DelegateRunPrinter);
                        break;
                    case PrintType.MPS000346:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__MPS000346, DelegateRunPrinter);
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

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                LoadDataByExpMest();
                if (
                    !((this._DataMetys != null && this._DataMetys.Count > 0)
                    || (this._DataMatys != null && this._DataMatys.Count > 0))
                    )
                    return false;

                #region TT Chung
                WaitingManager.Show();
                chmsExpMest = new V_HIS_EXP_MEST();
                _Medicines = new List<HIS_MEDICINE>();
                _Materials = new List<HIS_MATERIAL>();
                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                _ExpMestMetyReq_HCs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TC = new List<HIS_EXP_MEST_METY_REQ>();

                long _expMestId = this.expMest.ID;
                HisExpMestViewFilter chmsFilter = new HisExpMestViewFilter();
                chmsFilter.ID = _expMestId;
                var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                chmsExpMest = listChmsExpMest.First();

                CommonParam param = new CommonParam();

                long _EXP_MEST_STT_ID = chmsExpMest.EXP_MEST_STT_ID;
                if (_EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                    || _EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                {
                    MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                    mediFilter.EXP_MEST_ID = _expMestId;
                    _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
                    if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                    {
                        List<long> _MedicineIds = _ExpMestMedicines.Select(p => p.MEDICINE_ID ?? 0).ToList();
                        MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = _MedicineIds;
                        _Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                    }

                    MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                    matyFilter.EXP_MEST_ID = _expMestId;
                    _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);
                    if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                    {
                        List<long> _MaterialIds = _ExpMestMaterials.Select(p => p.MATERIAL_ID ?? 0).ToList();
                        MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = _MaterialIds;
                        _Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                    }
                }

                var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);
                bool tc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC);

                foreach (var item in this._DataMetys)
                {
                    var dataType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                    if (dataType != null)
                    {
                        if (dataType.IS_CHEMICAL_SUBSTANCE == 1)
                        {
                            _ExpMestMetyReq_HCs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                        {
                            _ExpMestMetyReq_GNs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                        {
                            _ExpMestMetyReq_HTs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                        {
                            _ExpMestMetyReq_TDs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                        {
                            _ExpMestMetyReq_PXs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                        {
                            _ExpMestMetyReq_COs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                        {
                            _ExpMestMetyReq_DTs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                        {
                            _ExpMestMetyReq_KSs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                        {
                            _ExpMestMetyReq_LAOs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC && tc)
                        {
                            _ExpMestMetyReq_TC.Add(item);
                        }
                        else
                        {

                            _ExpMestMetyReq_Ts.Add(item);
                        }
                    }
                }

                foreach (var item in this._DataMatys)
                {
                    var dataMaty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                    if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE != null)
                    {
                        _ExpMestMatyReq_HCs.Add(item);
                    }
                    else
                        _ExpMestMatyReq_VTs.Add(item);
                }

                WaitingManager.Hide();
                #endregion

                switch (printTypeCode)
                {
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__MPS000346:
                        Mps000346(ref result, printTypeCode, fileName);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__MPS000347:
                        Mps000347(ref result, printTypeCode, fileName);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void gridViewExMestBlood_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                VHisBloodADO data = view.GetFocusedRow() as VHisBloodADO;
                if (view.FocusedColumn.FieldName == "SALT_ENVI_STR" && view.ActiveEditor is GridLookUpEdit)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        var dataSource = editor.Properties.DataSource;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataSource), dataSource));
                        editor.EditValue = data.SALT_ENVI;
                    }
                    else
                        editor.EditValue = null;
                }

                if (view.FocusedColumn.FieldName == "ANTI_GLOBULIN_ENVI_STR" && view.ActiveEditor is GridLookUpEdit)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        var dataSource1 = editor.Properties.DataSource;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataSource1), dataSource1));
                        editor.EditValue = data.ANTI_GLOBULIN_ENVI;
                    }
                    else
                        editor.EditValue = null;
                }

                if (view.FocusedColumn.FieldName == "AC_SELF_ENVIDENCE_STR" && view.ActiveEditor is GridLookUpEdit)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        var dataSource1 = editor.Properties.DataSource;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataSource1), dataSource1));
                        editor.EditValue = data.AC_SELF_ENVIDENCE;
                    }
                    else
                        editor.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnActualExport_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                HIS_EXP_MEST row = null;
                if (this.rsSave != null)
                {
                    row = this.rsSave.ExpMest;
                }
                else if (this.cabinetBaseResultSDO != null)
                {
                    row = this.cabinetBaseResultSDO.ExpMest;
                }


                if (row != null)
                {
                    if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                    {
                        bool IsFinish = false;
                        if (row.IS_EXPORT_EQUAL_APPROVE == 1)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Đã xuất hết số lượng duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else if (row.IS_EXPORT_EQUAL_APPROVE == null || row.IS_EXPORT_EQUAL_APPROVE != 1)
                        {
                            HisExpMestMetyReqFilter expMestMetyReqFilter = new HisExpMestMetyReqFilter();
                            expMestMetyReqFilter.EXP_MEST_ID = row.ID;

                            var listExpMestMetyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, expMestMetyReqFilter, param);

                            HisExpMestMatyReqFilter expMestMatyReqFilter = new HisExpMestMatyReqFilter();
                            expMestMatyReqFilter.EXP_MEST_ID = row.ID;

                            var listExpMestMatyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, expMestMatyReqFilter, param);

                            List<AmountADO> amountAdo = new List<AmountADO>();

                            if (listExpMestMetyReq != null && listExpMestMetyReq.Count > 0)
                            {
                                foreach (var item in listExpMestMetyReq)
                                {
                                    var ado = new AmountADO(item);
                                    amountAdo.Add(ado);
                                }
                            }

                            if (listExpMestMatyReq != null && listExpMestMatyReq.Count > 0)
                            {
                                foreach (var item in listExpMestMatyReq)
                                {
                                    var ado = new AmountADO(item);
                                    amountAdo.Add(ado);
                                }
                            }

                            if (amountAdo != null && amountAdo.Count > 0)
                            {
                                var dataAdo = amountAdo.Where(o => o.Amount > o.Dd_Amount || o.Dd_Amount == null).ToList();
                                IsFinish = true;
                            }
                        }

                        HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                        sdo.ExpMestId = row.ID;
                        sdo.ReqRoomId = this.currentModule.RoomId;
                        sdo.IsFinish = IsFinish;
                        if (this.rsSave.ExpMest != null)
                        {
                            var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresult != null)
                            {
                                success = true;
                            }
                        }
                    }
                    else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                    {
                        HisExpMestSDO sdo = new HisExpMestSDO();
                        sdo.ExpMestId = row.ID;
                        sdo.ReqRoomId = this.currentModule.RoomId;

                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            ("api/HisExpMest/InPresExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiresult != null)
                        {
                            success = true;
                        }
                    }
                    else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                    {
                        HisExpMestSDO sdo = new HisExpMestSDO();
                        sdo.ExpMestId = row.ID;
                        sdo.ReqRoomId = this.currentModule.RoomId;

                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                            ("api/HisExpMest/AggrExamExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiresult != null)
                        {
                            success = true;
                        }
                    }
                    else
                    {
                        HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                        sdo.ExpMestId = row.ID;
                        sdo.ReqRoomId = this.currentModule.RoomId;
                        sdo.IsFinish = true;
                        object apiresult = null;
                        bool isExpMestSttDone = false;
                        if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK && row.CHMS_TYPE_ID.HasValue)
                        {
                            apiresult = new BackendAdapter(param).Post<CabinetBaseResultSDO>("api/HisExpMest/BaseExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresult != null)
                            {
                                success = true;
                                var dt = apiresult as CabinetBaseResultSDO;
                                isExpMestSttDone = dt.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                            }

                        }
                        else
                        {
                            apiresult = new BackendAdapter(param).Post<HIS_EXP_MEST>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresult != null)
                            {
                                success = true;
                                var dt = apiresult as HIS_EXP_MEST;
                                isExpMestSttDone = dt.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("apiresult____", apiresult));
                        if (chkBloodTransPrint.Checked && isExpMestSttDone)
                        {
                            Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                            store.RunPrintTemplate("Mps000421", delegateProcessPrint);
                        }

                    }

                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                    if (success)
                    {
                        btnActualExport.Enabled = false;
                        btnPrint.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void chkNotAutoClose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkNotAutoClose.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkNotAutoClose.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkNotAutoClose.Name;
                    csAddOrUpdate.VALUE = (chkNotAutoClose.Checked ? "1" : "");
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

        private void bbtnActualExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (!btnActualExport.Enabled)
                    return;

                btnActualExport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnPrint_ItemClick(object sender, ItemClickEventArgs e)
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                store.RunPrintTemplate("Mps000421", delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAssignPresDTT_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expMest), expMest) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ChmsExpMest), ChmsExpMest));

                    List<object> listArgs = new List<object>();
                    HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(ChmsExpMest.TDL_TREATMENT_ID.Value, 0, 0);
                    assignServiceADO.IsCabinet = true;
                    assignServiceADO.PatientDob = (ChmsExpMest.TDL_PATIENT_DOB ?? 0);
                    assignServiceADO.PatientName = ChmsExpMest.TDL_PATIENT_NAME;
                    assignServiceADO.GenderName = ChmsExpMest.TDL_PATIENT_GENDER_NAME;
                    assignServiceADO.TreatmentCode = ChmsExpMest.TDL_TREATMENT_CODE;
                    assignServiceADO.TreatmentId = ChmsExpMest.TDL_TREATMENT_ID.Value;
                    listArgs.Add(assignServiceADO);

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

        private void btnAssignService_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expMest), expMest) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ChmsExpMest), ChmsExpMest));
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ChmsExpMest.TDL_TREATMENT_ID.Value);
                    HIS.Desktop.ADO.AssignServiceADO assignServiceADO = new HIS.Desktop.ADO.AssignServiceADO(ChmsExpMest.TDL_TREATMENT_ID.Value, 0, 0);
                    assignServiceADO.PatientDob = (ChmsExpMest.TDL_PATIENT_DOB ?? 0);
                    assignServiceADO.PatientName = ChmsExpMest.TDL_PATIENT_NAME;
                    assignServiceADO.GenderName = ChmsExpMest.TDL_PATIENT_GENDER_NAME;


                    var roomByExpmest = BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(o => o.ID == ChmsExpMest.REQ_ROOM_ID);
                    if (roomByExpmest == null)
                    {
                        MessageManager.Show("Không lấy được phòng làm việc theo phòng chỉ định máu");
                        return;
                    }
                    assignServiceADO.ExamRegisterRoomId = roomByExpmest.ID;
                    listArgs.Add(assignServiceADO);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, roomByExpmest.ID, roomByExpmest.ROOM_TYPE_ID));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, roomByExpmest.ID, roomByExpmest.ROOM_TYPE_ID), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBloodTransPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkBloodTransPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkBloodTransPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkBloodTransPrint.Name;
                    csAddOrUpdate.VALUE = (chkBloodTransPrint.Checked ? "1" : "");
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServTein_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                // long configKey = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.HIS_DESKTOP_PLUGINS_TEST_CHECKVALUEMAXLENGOPTION));
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HisSereServTeinSDO data = (HisSereServTeinSDO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
                                    e.RepositoryItem = repositoryItemGridLookUp_Btn;
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
                var listDatas = this.gridControlSereServTein.DataSource as List<HisSereServTeinSDO>;
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

        private void gridViewSereServTein_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                //if (e.RowHandle > 0)
                //{
                HisSereServTeinSDO data = (HisSereServTeinSDO)gridViewSereServTein.GetRow(e.RowHandle);
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

        private void ProcessCheckNormal(ref HisSereServTeinSDO hisSereServTeinSDO)
        {
            try
            {
                long? serviceReqId = hisSereServTeinSDO.TDL_SERVICE_REQ_ID;
                var serviceReqs = _ServiceReqs.Where(o => o.ID == serviceReqId);

                if (serviceReqs != null && serviceReqs.Count() > 0)
                {
                    V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                    testIndexRange = GetTestIndexRange(serviceReqs.First().TDL_PATIENT_DOB, GetGenderId(serviceReqs.First()), hisSereServTeinSDO.TEST_INDEX_ID ?? 0, ref this.testIndexRangeAll);
                    if (testIndexRange != null)
                    {
                        AssignNormal(ref hisSereServTeinSDO, testIndexRange);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AssignNormal(ref HisSereServTeinSDO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
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

        private void gridViewSereServTein_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisSereServTeinSDO data = view.GetFocusedRow() as HisSereServTeinSDO;
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
        private void LoadDataMachine()
        {
            try
            {
                this._Machines = new List<HIS_MACHINE>();
                MOS.Filter.HisMachineFilter filter = new HisMachineFilter();
                this._Machines = new BackendAdapter(new CommonParam()).Get<List<HIS_MACHINE>>("api/HisMachine/Get", ApiConsumers.MosConsumer, filter, null);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.repositoryItemGridLookUp_Machine, _Machines, controlEditorADO);
                ControlEditorLoader.Load(this.repositoryItemGridLookUp_Btn, _Machines, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataMachineCombo(HisSereServTeinSDO data, GridLookUpEdit cbo)
        {
            try
            {
                if (_ServiceMachines != null && _ServiceMachines.Count > 0)
                {
                    var _machineIds = this._ServiceMachines.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).Select(o => o.MACHINE_ID).ToList();
                    List<HIS_MACHINE> dataMachines = new List<HIS_MACHINE>();
                    if (_machineIds != null && _machineIds.Count > 0)
                    {
                        dataMachines = this._Machines.Where(o => _machineIds.Contains(o.ID)).ToList();
                    }
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MACHINE_CODE", "", 150, 1));
                    columnInfos.Add(new ColumnInfo("MACHINE_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(cbo, dataMachines, controlEditorADO);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditPopup_Value_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HisSereServTeinSDO testLisResultADO = new HisSereServTeinSDO();
                var focusSereServ = (HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (focusSereServ != null && focusSereServ is HisSereServTeinSDO)
                {
                    testLisResultADO = (HisSereServTeinSDO)focusSereServ;
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

        private void ButtonEditPopup_Note_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HisSereServTeinSDO testLisResultADO = new HisSereServTeinSDO();
                var data = (HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (data != null && data is HisSereServTeinSDO)
                {
                    testLisResultADO = (HisSereServTeinSDO)data;
                }

                txtNoteIntoPopup.Text = testLisResultADO.NOTE;
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerNote.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom + 600));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_Note_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HisSereServTeinSDO testLisResultADO = new HisSereServTeinSDO();
                var data = (HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (data != null && data is HisSereServTeinSDO)
                {
                    testLisResultADO = (HisSereServTeinSDO)data;
                }

                txtNoteIntoPopup.Text = testLisResultADO.NOTE;
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerNote.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom + 600));
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
                HisSereServTeinSDO testLisResultADO = new HisSereServTeinSDO();
                var focusSereServ = (HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (focusSereServ != null && focusSereServ is HisSereServTeinSDO)
                {
                    testLisResultADO = (HisSereServTeinSDO)focusSereServ;
                }
                txtValueRangeIntoPopup.Text = testLisResultADO.VALUE;

                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerRangeValue.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom + 600));
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
                var data = (HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (data != null && data is HisSereServTeinSDO)
                {
                    testLisResultADO = (HisSereServTeinSDO)data;
                }

                txtCanNguyenIntoPopup.Text = testLisResultADO.LEAVEN;
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerCanNguyen.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom + 600));
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
                var data = (HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
                if (data != null && data is HisSereServTeinSDO)
                {
                    testLisResultADO = (HisSereServTeinSDO)data;
                }

                txtCanNguyenIntoPopup.Text = testLisResultADO.LEAVEN;
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerCanNguyen.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom + 600));
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
                var data = (HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
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

        private void btnOKForNote_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
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

        private void btnOKForValueRange_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (HisSereServTeinSDO)gridViewSereServTein.GetFocusedRow();
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

        private void chkExpiryDate_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkExpiryDate.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkExpiryDate.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkExpiryDate.Name;
                    csAddOrUpdate.VALUE = (chkExpiryDate.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                if (this.listBlood != null && this.listBlood.Count > 0)
                {
                    fillDataGridViewBlood();
                }
                else
                {
                    FillDataToGridBlood();
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridLookUp_Machine_ButtonClick(object sender, ButtonPressedEventArgs e)
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

        private void repositoryItemMTMuoi_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    VHisBloodADO bloodADO = (VHisBloodADO)gridViewExMestBlood.GetFocusedRow();
                    if (bloodADO != null)
                    {
                        bloodADO.SALT_ENVI = null;
                        this.FillDataToGridExpMestBlood();
                        //this.gridControlExpMestBlood.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemMTAntiGlobulin_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    VHisBloodADO bloodADO = (VHisBloodADO)gridViewExMestBlood.GetFocusedRow();
                    if (bloodADO != null)
                    {
                        bloodADO.ANTI_GLOBULIN_ENVI = null;
                        this.FillDataToGridExpMestBlood();
                        //this.gridControlExpMestBlood.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemTAC_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    VHisBloodADO bloodADO = (VHisBloodADO)gridViewExMestBlood.GetFocusedRow();
                    if (bloodADO != null)
                    {
                        bloodADO.AC_SELF_ENVIDENCE = null;
                        this.FillDataToGridExpMestBlood();
                        //this.gridControlExpMestBlood.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkShowBloodType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkShowBloodType.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkShowBloodType.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkShowBloodType.Name;
                    csAddOrUpdate.VALUE = (chkShowBloodType.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                if (this.listBlood != null && this.listBlood.Count > 0)
                {
                    fillDataGridViewBlood();
                }
                else
                {
                    FillDataToGridBlood();
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
