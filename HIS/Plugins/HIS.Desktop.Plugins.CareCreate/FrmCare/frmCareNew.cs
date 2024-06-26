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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.CareCreate.ADO;
using HIS.Desktop.Plugins.CareCreate.Base;
using HIS.Desktop.Plugins.CareCreate.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.CareCreate
{
    public partial class CareCreate : HIS.Desktop.Utility.FormBase
    {
        #region Declaretion
        internal List<HIS_AWARENESS> lstHisAwareness = new List<HIS_AWARENESS>();
        internal List<HIS_CARE_TYPE> lstHisCareType = new List<HIS_CARE_TYPE>();
        internal HIS_CARE hisCareCurrent = null;
        internal V_HIS_TREATMENT currentHisTreatment { get; set; }
        internal List<ADO.HisCareDetailADO> lstHisCareDetailADO { get; set; }
        long currentTreatmentId;
        internal int action = -1;
        internal HIS_DHST currentDhst { get; set; }
        DelegateRefeshData refeshData;

        internal HIS_TRACKING currentHisTracking { get; set; }
        internal HIS_CARE_SUM currentCareSum { get; set; }
        List<ADO.HisCareDetailADO> lstHisCareDetailSDO;

        int positionHandleControl = -1;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<HisCareTempADO> careTemps;
        DelegateSelectData delegateSelectData;

        List<HIS_RATION_TIME> ListRatioTime = new List<HIS_RATION_TIME>();
        V_HIS_TREATMENT_4 Treatment;
        private List<HIS_TRACKING> trackings = null;
        MOS.EFMODEL.DataModels.V_HIS_ROOM requestRoom;
        internal static long IsDefaultTracking;
        internal static long IsMineCheckedByDefault;
        bool isFirst = true;
        HisTreatmentBedRoomLViewFilter DataTransferTreatmentBedRoomFilter { get; set; }
        #endregion

        public CareCreate()
        {
            InitializeComponent();
            //LoadKeysFromlanguage();
        }

        public CareCreate(Inventec.Desktop.Common.Modules.Module currentModule, long TreatmentId, DelegateSelectData _delegateSelectData, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(currentModule)
        {
            try
            {
                InitializeComponent();
                // this.refeshData = refeshData;
                this.currentTreatmentId = TreatmentId;
                this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
                this.action = GlobalVariables.ActionAdd;
                btnCboPrint.Enabled = false;
                this.currentModule = currentModule;
                this.delegateSelectData = _delegateSelectData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public CareCreate(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_TREATMENT_4 _Treatment, DelegateSelectData _delegateSelectData, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(currentModule)
        {
            try
            {
                InitializeComponent();
                this.Treatment = _Treatment;
                this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
                this.action = GlobalVariables.ActionAdd;
                btnCboPrint.Enabled = false;
                this.currentModule = currentModule;
                this.delegateSelectData = _delegateSelectData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public CareCreate(Inventec.Desktop.Common.Modules.Module currentModule, HIS_CARE_SUM careSum, DelegateSelectData _delegateSelectData, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(currentModule)
        {
            try
            {
                InitializeComponent();
                this.currentCareSum = careSum;
                this.currentTreatmentId = careSum.TREATMENT_ID;
                this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
                this.action = GlobalVariables.ActionAdd;
                btnCboPrint.Enabled = false;
                //btnDhst.Enabled = false;
                this.currentModule = currentModule;
                this.delegateSelectData = _delegateSelectData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public CareCreate(Inventec.Desktop.Common.Modules.Module currentModule, MOS.EFMODEL.DataModels.HIS_CARE hisCare, DelegateSelectData _delegateSelectData, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(currentModule)
        {
            try
            {
                InitializeComponent();
                this.hisCareCurrent = hisCare;
                this.currentTreatmentId = hisCare.TREATMENT_ID;
                this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
                this.action = GlobalVariables.ActionEdit;
                btnCboPrint.Enabled = true;
                //btnDhst.Enabled = true;
                this.currentModule = currentModule;
                this.delegateSelectData = _delegateSelectData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public CareCreate(Inventec.Desktop.Common.Modules.Module currentModule, MOS.EFMODEL.DataModels.HIS_TRACKING hisTracking, DelegateSelectData _delegateSelectData, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(currentModule)
        {
            try
            {
                InitializeComponent();
                this.currentHisTracking = hisTracking;
                this.currentTreatmentId = hisTracking.TREATMENT_ID;
                this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
                this.action = GlobalVariables.ActionAdd;
                btnCboPrint.Enabled = false;
                //btnDhst.Enabled = false;
                this.currentModule = currentModule;
                this.delegateSelectData = _delegateSelectData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CareCreate_Load(object sender, EventArgs e)
        {
            try
            {
                IsDefaultTracking = HisConfigs.Get<long>(SdaConfigKeys.CONFIG_KEY__IS_DEFAULT_TRACKING);
                IsMineCheckedByDefault = HisConfigs.Get<long>(SdaConfigKeys.CONFIG_KEY__IS_MINE_CHECKED_BY_DEFAULT);

                SetCaptionByLanguageKey();

                SetIconFrm();

                this.requestRoom = GetRequestRoom(this.currentModule.RoomId);

                this.InitComboTracking(this.currentTreatmentId);

                careTemps = LoadCareTemp();
                InitComboCareTemp(careTemps);

                ValidControl();

                ResourceLangManager.InitResourceLanguageManager();

                LoadDataToForm(this);

                //LoadComboAwareness();
                //LoadComboSanitary();
                //LoadComboMucocutaneous();

                dtExcuteTime.EditValue = DateTime.Now;

                dtDoTime.EditValue = DateTime.Now;

                InitGroupAwareness();

                LoadDataToComboCareType(repositoryItemGridCareTypeName, lstHisCareType);

                grdColCareTypeName.ColumnEdit = repositoryItemGridCareTypeName;

                if (this.currentHisTracking != null)
                {
                    LoadDataDefaultByTracking();
                }
                if (this.action == GlobalVariables.ActionAdd)
                {
                    LoadDataHisDhstCreate(this, this.currentTreatmentId);
                }
                LoadPatientInfor(this.currentTreatmentId);

                if (this.action == GlobalVariables.ActionEdit && this.hisCareCurrent != null)
                {
                    LoadDataHisCareEdit(this, this.hisCareCurrent);
                    LoadDataHisDhstEdit(this, this.hisCareCurrent);
                }
                else
                {
                    InitCareDetail();
                }
                InitMenuToButtonPrint();

                HisRationTimeFilter timeFilter = new HisRationTimeFilter();
                timeFilter.IS_ACTIVE = 1;
                ListRatioTime = new BackendAdapter(new CommonParam()).Get<List<HIS_RATION_TIME>>("api/HisRationTime/Get", ApiConsumers.MosConsumer, timeFilter, null);

                ProcessLoadTxtNutrition();

                if (this.Treatment != null && this.Treatment.IS_PAUSE == 1)
                {
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_ROOM GetRequestRoom(long requestRoomId)
        {
            V_HIS_ROOM result = new V_HIS_ROOM();
            try
            {
                if (requestRoomId > 0)
                {
                    result = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == requestRoomId);
                }
            }
            catch (Exception ex)
            {
                result = new V_HIS_ROOM();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        //private void LoadComboMucocutaneous()
        //{
        //    try
        //    {
        //        List<MucocutaneousADO> data = new List<MucocutaneousADO>();
        //        data.Add(new MucocutaneousADO { ID = 1, NAME = "Hồng" });
        //        data.Add(new MucocutaneousADO { ID = 2, NAME = "Nhợt nhạt" });
        //        data.Add(new MucocutaneousADO { ID = 3, NAME = "Bình thường" });
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("NAME", "", 380, 1));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "NAME", columnInfos, false, 380);
        //        ControlEditorLoader.Load(cboMucocutaneous, data, controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        //private void LoadComboAwareness()
        //{
        //    try
        //    {
        //        List<AwarenessADO> data = new List<AwarenessADO>();
        //        data.Add(new AwarenessADO { ID = 1, NAME = "Tỉnh" });
        //        data.Add(new AwarenessADO { ID = 2, NAME = "Lơ mơ" });
        //        data.Add(new AwarenessADO { ID = 3, NAME = "Hôn mê" });
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("NAME", "", 380, 1));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "NAME", columnInfos, false, 380);
        //        ControlEditorLoader.Load(cboAwareness, data, controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        //private void LoadComboSanitary()
        //{
        //    try
        //    {
        //        List<SanitaryADO> data = new List<SanitaryADO>();
        //        data.Add(new SanitaryADO { ID = 1, NAME = "Tắm thường" });
        //        data.Add(new SanitaryADO { ID = 2, NAME = "Tắm thuốc" });
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("NAME", "", 380, 1));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "NAME", columnInfos, false, 380);
        //        ControlEditorLoader.Load(cboSanitary, data, controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void LoadPatientInfor(long treatmentId)
        {
            try
            {
                MOS.Filter.HisTreatmentFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentFilter();
                hisTreatmentFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                hisTreatmentFilter.ORDER_DIRECTION = "DESC";
                hisTreatmentFilter.ORDER_FIELD = "MODIFY_TIME";
                hisTreatmentFilter.ID = treatmentId;

                var hisTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, hisTreatmentFilter, param).FirstOrDefault();

                if (hisTreatment != null)
                {
                    txtPatientCode.Text = hisTreatment.TDL_PATIENT_CODE;
                    txtPatientName.Text = hisTreatment.TDL_PATIENT_NAME;
                    txtGender.Text = hisTreatment.TDL_PATIENT_GENDER_NAME;
                    if (hisTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtDOB.Text = hisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        txtDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(hisTreatment.TDL_PATIENT_DOB).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HisCareTempADO> LoadCareTemp()
        {
            List<HisCareTempADO> result = new List<HisCareTempADO>();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisCareTempFilter Filter = new MOS.Filter.HisCareTempFilter();
                Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                Filter.ORDER_FIELD = "CARE_TEMP_NAME";
                Filter.ORDER_DIRECTION = "ASC";
                var careTemps = new BackendAdapter(param).Get<List<HIS_CARE_TEMP>>("api/HisCareTemp/Get", ApiConsumer.ApiConsumers.MosConsumer, Filter, param);
                if (careTemps != null && careTemps.Count() > 0)
                {
                    careTemps = careTemps.Where(o => o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || o.IS_PUBLIC == 1).ToList();
                }
                foreach (var item in careTemps)
                {
                    HisCareTempADO ado = new HisCareTempADO();
                    AutoMapper.Mapper.CreateMap<HIS_CARE_TEMP, HisCareTempADO>();
                    ado = AutoMapper.Mapper.Map<HisCareTempADO>(item);
                    ado.CREATE_TIME_STR = item.CREATE_TIME != null ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(item.CREATE_TIME ?? 0) : "";
                    result.Add(ado);
                }
            }
            catch (Exception ex)
            {
                result = new List<HisCareTempADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InitComboCareTemp(List<HisCareTempADO> db)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CARE_TEMP_CODE", "", 100, 2));
                columnInfos.Add(new ColumnInfo("CARE_TEMP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CARE_TEMP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboCareTemp, db, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderControl__ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CareCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.CareCreate.CareCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCareNew.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmCareNew.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmCareNew.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmCareNew.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcTrackCare.Text = Inventec.Common.Resource.Get.Value("frmCareNew.grcTrackCare.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmCareNew.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewCareDetail.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmCareNew.gridViewCareDetail.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCareTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmCareNew.grdColCareTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCareTypeName.Caption = Inventec.Common.Resource.Get.Value("frmCareNew.grdColCareTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCareTypeContent.Caption = Inventec.Common.Resource.Get.Value("frmCareNew.grdColCareTypeContent.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAdd.Caption = Inventec.Common.Resource.Get.Value("frmCareNew.grdColAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemButtonEdit_Add.NullText = Inventec.Common.Resource.Get.Value("frmCareNew.repositoryItemButtonEdit_Add.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCareTypeId.Caption = Inventec.Common.Resource.Get.Value("frmCareNew.grdColCareTypeId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemButtonEdit_Delete.NullText = Inventec.Common.Resource.Get.Value("frmCareNew.repositoryItemButtonEdit_Delete.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboCareType.NullText = Inventec.Common.Resource.Get.Value("frmCareNew.repositoryItemcboCareType.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupControl2.Text = Inventec.Common.Resource.Get.Value("frmCareNew.groupControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmCareNew.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHasTest.Properties.Caption = Inventec.Common.Resource.Get.Value("frmCareNew.chkHasTest.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHasRehabiliration.Text = Inventec.Common.Resource.Get.Value("frmCareNew.chkHasRehabiliration.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHasRehabiliration.ToolTip = Inventec.Common.Resource.Get.Value("frmCareNew.chkHasRehabiliration.Properties.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHasRehabiliration.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmCareNew.chkHasRehabiliration.Properties.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHasAddMedicine.Properties.Caption = Inventec.Common.Resource.Get.Value("frmCareNew.chkHasAddMedicine.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHasMedicine.Properties.Caption = Inventec.Common.Resource.Get.Value("frmCareNew.chkHasMedicine.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHasMedicine.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciHasMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHasAddMedicine.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciHasAddMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHasTest.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciHasTest.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNutrition.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciNutrition.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grVitalSigns.Text = Inventec.Common.Resource.Get.Value("frmCareNew.grVitalSigns.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmCareNew.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciPulse.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciPulse.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciTemperature.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciTemperature.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciBloodPressure.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciBloodPressure.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciBreathRate.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciBreathRate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciWeight.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciWeight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("frmCareNew.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExcuteTime.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciExcuteTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciAwareness.Text = Inventec.Common.Resource.Get.Value("frmCareNew.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMucocutaneous.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciMucocutaneous.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciUrine.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciUrine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciSanitary.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciSanitary.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTutorial.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciTutorial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEducation.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciEducation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDejecta.Text = Inventec.Common.Resource.Get.Value("frmCareNew.lciDejecta.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmCareNew.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmCareNew.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmCareNew.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCareNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.BtnNutrition.Text = Inventec.Common.Resource.Get.Value("frmCareNew.BtnNutrition.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExcuteTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ProcessLoadTxtNutrition();
                    cboAwareness.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHandwritten_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMucocutaneous.Focus();
                    cboMucocutaneous.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAwareness_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMucocutaneous.Focus();
                    cboMucocutaneous.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMucocutaneous_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtUrine.Focus();
                    txtUrine.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtUrine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboSanitary.Focus();
                    cboSanitary.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDejecta_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHasMedicine.Properties.FullFocusRect = true;
                    chkHasMedicine.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTutorial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEducation.Focus();
                    txtEducation.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEducation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDejecta.Focus();
                    txtDejecta.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasMedicine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (chkHasMedicine.Checked)
                    //    chkHasMedicine.CheckState = CheckState.Unchecked;
                    //else
                    //    chkHasMedicine.CheckState = CheckState.Checked;
                    chkHasAddMedicine.Properties.FullFocusRect = true;
                    chkHasAddMedicine.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasAddMedicine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkHasAddMedicine.Checked)
                        chkHasAddMedicine.CheckState = CheckState.Unchecked;
                    else
                        chkHasAddMedicine.CheckState = CheckState.Checked;
                    chkHasTest.Properties.FullFocusRect = true;
                    chkHasTest.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasTest_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkHasTest.Checked)
                        chkHasTest.CheckState = CheckState.Unchecked;
                    else
                        chkHasTest.CheckState = CheckState.Checked;
                    txtNutrition.Focus();
                    txtNutrition.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNutrition_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridControlCareDetail.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAwareness_KeyUp(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        txtMucocutaneous.Focus();
            //        txtMucocutaneous.SelectAll();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void cboAwareness_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboMucocutaneous.Focus();
                    cboMucocutaneous.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ADO.HisCareDetailADO data_ServiceSDO = (ADO.HisCareDetailADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data_ServiceSDO != null)
                    {
                        //if (e.Column.FieldName == "PRICE_DISPLAY")
                        //{
                        //    if (data_ServiceSDO.PATIENT_TYPE_ID != 0)
                        //    {
                        //        var data_ServicePrice = EXE.APP.GlobalStore.ListServicePaty.Where(o => o.SERVICE_ID == data_ServiceSDO.SERVICE_ID && o.PATIENT_TYPE_ID == data_ServiceSDO.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                        //        if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                        //        {
                        //            e.Value = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound((data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO)));
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareDetail_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ADO.HisCareDetailADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (ADO.HisCareDetailADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "BtnAddDelete")
                    {
                        if (data.Action == GlobalVariables.ActionAdd)
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_Add;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_Delete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareDetail_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ADO.HisCareDetailADO data = view.GetFocusedRow() as ADO.HisCareDetailADO;
                if (view.FocusedColumn.FieldName == "CARE_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        if (editor.EditValue == null)//xemlai...
                        {
                            string error = GetError(gridViewCareDetail.FocusedRowHandle, gridViewCareDetail.FocusedColumn);
                            if (error == string.Empty) return;
                            gridViewCareDetail.SetColumnError(gridViewCareDetail.FocusedColumn, error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetError(int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
        {
            try
            {
                if (column.FieldName == "CARE_TYPE_ID")
                {
                    ADO.HisCareDetailADO data = (ADO.HisCareDetailADO)gridViewCareDetail.GetRow(rowHandle);
                    if (data == null)
                        return string.Empty;

                    if (data.CARE_TYPE_ID <= 0)
                    {
                        return "Không có thông tin loại chăm sóc.";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return string.Empty;
        }

        private void SetError(BaseEditViewInfo cellInfo, string errorIconText)
        {
            try
            {
                if (errorIconText == string.Empty)
                {
                    cellInfo.ErrorIconText = null;
                    cellInfo.ShowErrorIcon = false;
                    return;
                }
                cellInfo.ErrorIconText = errorIconText;
                cellInfo.ShowErrorIcon = true;
                cellInfo.FillBackground = true;
                cellInfo.ErrorIcon = DXErrorProvider.GetErrorIconInternal(ErrorType.Critical);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_Add_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                ADO.HisCareDetailADO hisCareDetail = new ADO.HisCareDetailADO();
                hisCareDetail.Action = GlobalVariables.ActionEdit;
                this.lstHisCareDetailADO.Add(hisCareDetail);
                gridControlCareDetail.DataSource = null;
                gridControlCareDetail.DataSource = this.lstHisCareDetailADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var hisCareDetail = (ADO.HisCareDetailADO)gridViewCareDetail.GetFocusedRow();
                if (hisCareDetail != null)
                {
                    this.lstHisCareDetailADO.Remove(hisCareDetail);
                    gridControlCareDetail.DataSource = null;
                    gridControlCareDetail.DataSource = this.lstHisCareDetailADO;
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
                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;

                if (gridViewCareDetail.IsEditing)
                    gridViewCareDetail.CloseEditor();

                if (gridViewCareDetail.FocusedRowModified)
                    gridViewCareDetail.UpdateCurrentRow();

                if (gridViewCareDetail.HasColumnErrors)
                    return;

                MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                treatmentFilter.ID = this.currentTreatmentId;
                var rsTreatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, null);
                if (rsTreatment != null && rsTreatment.Count > 0)
                {
                    DateTime trackingTime = dtExcuteTime.DateTime;
                    DateTime inTimeTreatment = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rsTreatment[0].IN_TIME) ?? DateTime.Now;
                    if (trackingTime < inTimeTreatment)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.Validate_Date_Time, ResourceMessage.ThongBao);
                        dtExcuteTime.Focus();
                        dtExcuteTime.SelectAll();
                        return;
                    }
                }

                if (Check())
                {
                    SaveCareProcess();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool Check()
        {
            bool result = true;
            try
            {
                CommonParam param = new CommonParam();
                if (this.lstHisCareDetailADO == null || this.lstHisCareDetailADO.Count == 0)
                    throw new ArgumentNullException("Du lieu dau vao khong hop le. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstHisCareDetailADO), lstHisCareDetailADO));

                var groupCareType = from p in this.lstHisCareDetailADO
                                    group p by p.CARE_TYPE_ID into g
                                    select new { Key = g.Key, CareDetail = g.ToList() };
                if (groupCareType != null && groupCareType.Count() > 0)
                {
                    foreach (var item in groupCareType)
                    {
                        if (item.CareDetail.Count > 1)
                        {
                            result = false;
                            param.Messages.Add(ResourceMessage.ChamSoc_DuLieuChiTietKhongTheCungLoaiChamSoc);
                            break;
                        }
                    }
                }

                #region Show message
                MessageManager.Show(param, null);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private bool CheckTemp(List<HisCareDetailADO> HisCareDetails)
        {
            bool result = true;
            try
            {
                CommonParam param = new CommonParam();
                if (HisCareDetails == null || HisCareDetails.Count == 0)
                    throw new ArgumentNullException("Du lieu dau vao khong hop le. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisCareDetails), HisCareDetails));

                var groupCareType = from p in HisCareDetails
                                    group p by p.CARE_TYPE_ID into g
                                    select new { Key = g.Key, CareDetail = g.ToList() };
                if (groupCareType != null && groupCareType.Count() > 0)
                {
                    foreach (var item in groupCareType)
                    {
                        if (item.CareDetail.Count > 1)
                        {
                            result = false;
                            param.Messages.Add(ResourceMessage.ChamSoc_DuLieuChiTietKhongTheCungLoaiChamSoc);
                            break;
                        }
                    }
                }

                #region Show message
                MessageManager.Show(param, null);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void repositoryItemGridCareTypeName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CareTypeAdd").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.CareTypeAdd");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        moduleData.RoomId = this.currentModule.RoomId;
                        moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                        listArgs.Add(moduleData);
                        listArgs.Add((HIS.Desktop.Common.DelegateReturnSuccess)ProcessAfterSave);
                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
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

        void ProcessAfterSave(bool succsess)
        {
            try
            {
                if (!succsess)
                    return;
                LoadDataToGridControlCareDetail(ref lstHisCareType);
                LoadDataToComboCareType(repositoryItemGridCareTypeName, lstHisCareType);
                grdColCareTypeName.ColumnEdit = repositoryItemGridCareTypeName;
                var data = (ADO.HisCareDetailADO)gridViewCareDetail.GetFocusedRow();
                if (gridViewCareDetail.EditingValue != null)
                {
                    data.CARE_TYPE_ID = lstHisCareType[0].ID;
                    //xem lại
                }
                else
                {
                    data.CARE_TYPE_ID = lstHisCareType[0].ID;
                }
                gridViewCareDetail.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
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
                // btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //todo
            try
            {
                onClickPrintCare();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlCareDetail_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    GridControl grid = sender as GridControl;
                    GridView view = grid.FocusedView as GridView;
                    if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && view.FocusedColumn.VisibleIndex == 0))
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        dtDoTime.Focus();
                        //grid.SelectNextControl(btnAdd, e.Modifiers == Keys.None, false, false, true);
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    GridControl grid = sender as GridControl;
                    GridView view = grid.FocusedView as GridView;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasAddMedicine_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (chkHasAddMedicine.Checked)
                    //    chkHasAddMedicine.CheckState = CheckState.Unchecked;
                    //else
                    //    chkHasAddMedicine.CheckState = CheckState.Checked;
                    chkHasTest.Properties.FullFocusRect = true;
                    chkHasTest.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasTest_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (chkHasTest.Checked)
                    //    chkHasTest.CheckState = CheckState.Unchecked;
                    //else
                    //    chkHasTest.CheckState = CheckState.Checked;
                    // chkHasTest.Properties.FullFocusRect = true;
                    chkHasRehabiliration.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridCareTypeName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    GridControl grid = sender as GridControl;
                    this.repositoryItemtxtContent.AllowFocused = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_Add_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    repositoryItemButtonEdit_Add_ButtonClick(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_Delete_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    repositoryItemButtonEdit_Delete_ButtonClick(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckDHST(long _treatmentId, ref HIS_DHST dhstRef)
        {
            try
            {
                if (_treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisDhstFilter dhstFilter = new MOS.Filter.HisDhstFilter();
                    dhstFilter.TREATMENT_ID = _treatmentId;
                    var getDhst = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);

                    if (getDhst != null && getDhst.Count > 0)
                    {
                        var dhstTracking = getDhst.FirstOrDefault(p => p.TRACKING_ID != null);
                        if (dhstTracking != null)
                        {
                            dhstRef = dhstTracking;
                        }
                        else
                        {
                            MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                            filter.TREATMENT_ID = _treatmentId;
                            filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                            var serviceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param);
                            if (serviceReqs != null && serviceReqs.Count > 0)
                            {
                                List<long> dhstIds = serviceReqs.Where(p => p.DHST_ID != null).Select(p => p.DHST_ID ?? 0).ToList();
                                if (dhstIds != null && dhstIds.Count > 0)
                                {
                                    var dhstServiceReq = getDhst.FirstOrDefault(p => p.ID == dhstIds.First());
                                    dhstRef = dhstServiceReq;
                                }
                            }
                        }
                    }
                    else
                        dhstRef = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDhst_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_DHST dhstData = new HIS_DHST();
                CheckDHST(this.currentTreatmentId, ref dhstData);

                HIS.Desktop.Plugins.CareCreate.DHST.frmDhst frmDhst = new DHST.frmDhst(this.hisCareCurrent, dhstData, this.currentModule);
                frmDhst.ShowDialog();
                //

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareDetail_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    GridView view = sender as GridView;
                    GridColumn onOrderCol = view.Columns["CONTENT"];
                    view.ClearColumnErrors();
                    var data = (ADO.HisCareDetailADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null && data.CARE_TYPE_ID > 0)
                    {
                        if (String.IsNullOrEmpty(data.CONTENT))
                        {
                            // e.Valid = false;
                            view.SetColumnError(onOrderCol, "Trường dữ liệu bắt buộc");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareDetail_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                //Suppress displaying the error message box
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HisCareDetailADO> LoadCareTempDetail(long CareTempId)
        {
            List<ADO.HisCareDetailADO> lstHisCareDetailSDO = new List<ADO.HisCareDetailADO>();
            int i = 0;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisCareTempDetailFilter filter = new MOS.Filter.HisCareTempDetailFilter();
                filter.CARE_TEMP_ID = CareTempId;
                var CareTempDetail = new BackendAdapter(param).Get<List<HIS_CARE_TEMP_DETAIL>>("api/HisCareTempDetail/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                foreach (var item_CareDetail in CareTempDetail)
                {
                    ADO.HisCareDetailADO hisCareDetailSDO = new ADO.HisCareDetailADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisCareDetailADO>(hisCareDetailSDO, item_CareDetail);
                    if (i == 0)
                        hisCareDetailSDO.Action = GlobalVariables.ActionAdd;
                    else
                        hisCareDetailSDO.Action = GlobalVariables.ActionEdit;
                    lstHisCareDetailSDO.Add(hisCareDetailSDO);
                    i++;
                }

                if (lstHisCareDetailSDO != null && lstHisCareDetailSDO.Count > 0)
                {
                    gridControlCareDetail.DataSource = lstHisCareDetailSDO;
                    lstHisCareDetailADO = lstHisCareDetailSDO;
                }
                else
                {
                    lstHisCareDetailADO = new List<HisCareDetailADO>();
                    ADO.HisCareDetailADO hisCareDetailSDO = new ADO.HisCareDetailADO();
                    hisCareDetailSDO.Action = GlobalVariables.ActionAdd;
                    lstHisCareDetailADO.Add(hisCareDetailSDO);
                    gridViewCareDetail.BeginDataUpdate();
                    gridControlCareDetail.DataSource = null;
                    gridControlCareDetail.DataSource = lstHisCareDetailADO;
                    gridViewCareDetail.EndDataUpdate();
                }
            }
            catch (Exception ex)
            {
                lstHisCareDetailSDO = new List<HisCareDetailADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return lstHisCareDetailSDO;
        }

        private void cboCareTemp_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCareTemp.EditValue != null)
                    {
                        var CareTemp = this.careTemps.First(o => o.ID == Int64.Parse(cboCareTemp.EditValue.ToString()));
                        LoadDataHisCareTemp(CareTemp);
                        if (CareTemp != null)
                        {
                            txtCareTempCode.Text = CareTemp.CARE_TEMP_CODE;
                            dtExcuteTime.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;

                if (gridViewCareDetail.IsEditing)
                    gridViewCareDetail.CloseEditor();

                if (gridViewCareDetail.FocusedRowModified)
                    gridViewCareDetail.UpdateCurrentRow();

                if (gridViewCareDetail.HasColumnErrors)
                    return;

                if (CheckTemp(lstHisCareDetailADO))
                {
                    MOS.EFMODEL.DataModels.HIS_CARE_TEMP care = new HIS_CARE_TEMP();
                    ProcessDataCare(ref care);
                    if (care == null)
                    {
                        return;
                    }
                    List<object> listArgs = new List<object>();
                    listArgs.Add(care);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisCareTemp", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    cboCareTemp.EditValue = null;
                    txtCareTempCode.Text = "";
                    careTemps = LoadCareTemp();
                    InitComboCareTemp(careTemps);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCareTempCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadCareTempCombo(strValue);
                    var CareTemp = this.careTemps.First(o => o.ID == Int64.Parse(cboCareTemp.EditValue.ToString()));
                    LoadDataHisCareTemp(CareTemp);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCareTempCombo(string _accountBookCode)
        {
            try
            {
                List<HisCareTempADO> listResult = new List<HisCareTempADO>();
                listResult = this.careTemps.Where(o => (o.CARE_TEMP_CODE != null && o.CARE_TEMP_CODE.StartsWith(_accountBookCode))).ToList();

                if (listResult.Count == 1)
                {
                    cboCareTemp.EditValue = listResult[0].ID;
                    txtCareTempCode.Text = listResult[0].CARE_TEMP_CODE;
                    dtExcuteTime.Focus();
                }
                else if (listResult.Count > 1)
                {
                    cboCareTemp.EditValue = null;
                    cboCareTemp.Focus();
                    cboCareTemp.ShowPopup();
                }
                else
                {
                    cboCareTemp.EditValue = null;
                    cboCareTemp.Focus();
                    cboCareTemp.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSaveTemp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSaveTemp_Click(null, null);
        }

        private void btnCboPrint_Click(object sender, EventArgs e)
        {
            btnCboPrint.ShowDropDown();
        }

        private void BtnNutrition_Click(object sender, EventArgs e)
        {
            try
            {
                // popup yêu cầu xem
                List<object> _listObj = new List<object>();
                _listObj.Add(this.currentTreatmentId);
                if (DataTransferTreatmentBedRoomFilter != null)
                    _listObj.Add(DataTransferTreatmentBedRoomFilter);
                WaitingManager.Hide();
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignNutrition", currentModule.RoomId, currentModule.RoomTypeId, _listObj);
                ProcessLoadTxtNutrition();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessLoadTxtNutrition()
        {
            try
            {
                if (dtExcuteTime.EditValue != null && dtExcuteTime.DateTime != DateTime.MaxValue && dtExcuteTime.DateTime != DateTime.MinValue)
                {
                    HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                    reqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN;
                    reqFilter.TREATMENT_ID = this.currentTreatmentId;
                    reqFilter.INTRUCTION_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(dtExcuteTime.DateTime.ToString("yyyyMMdd") + "000000");
                    var apiResult = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, reqFilter, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {

                        List<string> nutrition = new List<string>();
                        HisSereServRationViewFilter seseFilter = new HisSereServRationViewFilter();
                        seseFilter.SERVICE_REQ_IDs = apiResult.Select(s => s.ID).ToList();
                        var ssApiResult = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_RATION>>("/api/HisSereServRation/GetView", ApiConsumers.MosConsumer, reqFilter, null);
                        if (ssApiResult != null && ssApiResult.Count > 0)
                        {
                            var groupTime = apiResult.GroupBy(g => g.RATION_TIME_ID).ToList();
                            foreach (var group in groupTime)
                            {
                                var ss = ssApiResult.Where(o => group.Select(s => s.ID).Contains(o.SERVICE_REQ_ID)).ToList();
                                var a = this.ListRatioTime.FirstOrDefault(o => o.ID == group.First().RATION_TIME_ID);
                                if (ss != null && ss.Count > 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ss), ss));
                                    nutrition.Add(string.Format("{0}:{1}", (a != null ? a.RATION_TIME_NAME : ""), string.Join(",", ss.Select(s => s.SERVICE_CODE).Distinct().ToList())));
                                }
                            }
                        }

                        if (nutrition != null && nutrition.Count > 0)
                        {
                            string sang = nutrition.FirstOrDefault(o => o.ToLower().Contains("sáng"));
                            string trua = nutrition.FirstOrDefault(o => o.ToLower().Contains("trưa"));
                            string chieu = nutrition.FirstOrDefault(o => o.ToLower().Contains("chiều"));
                            string dem = nutrition.FirstOrDefault(o => o.ToLower().Contains("đêm"));
                            List<string> khac = nutrition.Where(o => !o.ToLower().Contains("đêm") && !o.ToLower().Contains("chiều") && !o.ToLower().Contains("trưa") && !o.ToLower().Contains("sáng")).ToList();

                            if (!String.IsNullOrWhiteSpace(sang) || !String.IsNullOrWhiteSpace(trua) || !String.IsNullOrWhiteSpace(chieu) || !String.IsNullOrWhiteSpace(dem) || (khac != null && khac.Count > 0))
                            {
                                this.txtNutrition.Text = (sang ?? "") + ";" + (trua ?? "") + ";" + (chieu ?? "");
                                if (khac != null && khac.Count > 0)
                                {
                                    this.txtNutrition.Text += String.Join(";", khac);
                                }
                            }
                            else
                            {
                                this.txtNutrition.Text = String.Join(";", nutrition);
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

        private void dtExcuteTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    ProcessLoadTxtNutrition();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHasRehabiliration_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNutrition.Focus();
                    txtNutrition.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void fillDataToBmiAndLeatherArea()
        {
            try
            {
                decimal bmi = 0;
                if (spinHeight.Value != null && spinHeight.Value != 0)
                {
                    bmi = (spinWeight.Value) / ((spinHeight.Value / 100) * (spinHeight.Value / 100));
                }
                double leatherArea = 0.007184 * Math.Pow((double)spinHeight.Value, 0.725) * Math.Pow((double)spinWeight.Value, 0.425);
                s.Text = Math.Round(bmi, 2) + "";
                lblLeatherArea.Text = Math.Round(leatherArea, 2) + "";
                if (bmi < 16)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("frmCareNew.SKINNY.III", ResourceLangManager.LanguageCareCreate, LanguageManager.GetCulture());
                }
                else if (16 <= bmi && bmi < 17)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("frmCareNew.SKINNY.II", ResourceLangManager.LanguageCareCreate, LanguageManager.GetCulture());
                }
                else if (17 <= bmi && bmi < (decimal)18.5)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("frmCareNew.SKINNY.I", ResourceLangManager.LanguageCareCreate, LanguageManager.GetCulture());
                }
                else if ((decimal)18.5 <= bmi && bmi < 25)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("frmCareNew.BMIDISPLAY.NORMAL", ResourceLangManager.LanguageCareCreate, LanguageManager.GetCulture());
                }
                else if (25 <= bmi && bmi < 30)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("frmCareNew.BMIDISPLAY.OVERWEIGHT", ResourceLangManager.LanguageCareCreate, LanguageManager.GetCulture());
                }
                else if (30 <= bmi && bmi < 35)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("frmCareNew.BMIDISPLAY.OBESITY.I", ResourceLangManager.LanguageCareCreate, LanguageManager.GetCulture());
                }
                else if (35 <= bmi && bmi < 40)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("frmCareNew.BMIDISPLAY.OBESITY.II", ResourceLangManager.LanguageCareCreate, LanguageManager.GetCulture());
                }
                else if (40 < bmi)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("frmCareNew.BMIDISPLAY.OBESITY.III", ResourceLangManager.LanguageCareCreate, LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinWeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                fillDataToBmiAndLeatherArea();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                fillDataToBmiAndLeatherArea();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDoTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSPO2.Focus();
                    spinSPO2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSPO2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinPulse.Focus();
                    spinPulse.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinPulse_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTemperature.Focus();
                    spinTemperature.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTemperature_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBloodPressureMax.Focus();
                    spinBloodPressureMax.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodPressureMax_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBloodPressureMin.Focus();
                    spinBloodPressureMin.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodPressureMin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBreathRate.Focus();
                    spinBreathRate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBreathRate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinWeight.Focus();
                    spinWeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinWeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinChest.Focus();
                    spinChest.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChest_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinHeight.Focus();
                    spinHeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBelly.Focus();
                    spinBelly.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBelly_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBloodWay.Focus();
                    spinBloodWay.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodWay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMucocutaneous_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtUrine.Focus();
                    txtUrine.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSanitary_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTutorial.Focus();
                    txtTutorial.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinBelly_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnConnectBloodPressure_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_DHST data = HIS.Desktop.Plugins.Library.ConnectBloodPressure.ConnectBloodPressureProcessor.GetData();
                if (data != null)
                {
                    if (data.EXECUTE_TIME != null)
                        dtDoTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtDoTime.EditValue = DateTime.Now;

                    if (data.BLOOD_PRESSURE_MAX.HasValue)
                    {
                        spinBloodPressureMax.EditValue = data.BLOOD_PRESSURE_MAX;
                    }
                    if (data.BLOOD_PRESSURE_MIN.HasValue)
                    {
                        spinBloodPressureMin.EditValue = data.BLOOD_PRESSURE_MIN;
                    }
                    if (data.BREATH_RATE.HasValue)
                    {
                        spinBreathRate.EditValue = data.BREATH_RATE;
                    }
                    if (data.HEIGHT.HasValue)
                    {
                        spinHeight.EditValue = data.HEIGHT;
                    }
                    if (data.CHEST.HasValue)
                    {
                        spinChest.EditValue = data.CHEST;
                    }
                    if (data.BELLY.HasValue)
                    {
                        spinBelly.EditValue = data.BELLY;
                    }
                    if (data.PULSE.HasValue)
                    {
                        spinPulse.EditValue = data.PULSE;
                    }
                    if (data.TEMPERATURE.HasValue)
                    {
                        spinTemperature.EditValue = data.TEMPERATURE;
                    }
                    if (data.WEIGHT.HasValue)
                    {
                        spinWeight.EditValue = data.WEIGHT;
                    }
                    if (!String.IsNullOrWhiteSpace(data.NOTE))
                    {
                        txtNote.Text = data.NOTE;
                    }
                    if (data.SPO2.HasValue)
                        spinSPO2.Value = (data.SPO2.Value * 100);
                    else
                        spinSPO2.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTracking_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    //cboTracking.Properties.Buttons[1].Visible = true;
                    cboTracking.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.currentTreatmentId);

                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));

                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();

                        //Load lại tracking
                        InitComboTracking(this.currentTreatmentId);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTracking_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTracking.EditValue != null)
                {
                    this.currentHisTracking = this.trackings != null && this.trackings.Count > 0 ? this.trackings.FirstOrDefault(o => o.ID == (long)cboTracking.EditValue) : new HIS_TRACKING();
                }
                else
                {
                    this.currentHisTracking = new HIS_TRACKING();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        async Task InitComboTracking(long treatmentId)
        {
            try
            {
                List<TrackingAdo> result = new List<TrackingAdo>();

                CommonParam param = new CommonParam();
                MOS.Filter.HisTrackingFilter trackingFilter = new HisTrackingFilter();
                trackingFilter.TREATMENT_ID = treatmentId;
                trackingFilter.DEPARTMENT_ID = this.requestRoom != null ? (long?)this.requestRoom.DEPARTMENT_ID : null;

                this.trackings = await new BackendAdapter(param).GetAsync<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumer.ApiConsumers.MosConsumer, trackingFilter, param);
                this.trackings = this.trackings != null && this.trackings.Count > 0 ? this.trackings.OrderByDescending(o => o.TRACKING_TIME).ToList() : trackings;
                foreach (var tracking in this.trackings)
                {
                    var trackingAdo = new TrackingAdo(tracking);
                    result.Add(trackingAdo);
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TrackingTimeStr", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TrackingTimeStr", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboTracking, result, controlEditorADO);

                LoadTrackingDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTrackingDefault()
        {
            try
            {
                if (isFirst && this.currentHisTracking != null)
                {
                    isFirst = false;
                    cboTracking.EditValue = this.currentHisTracking.ID;
                }
                else
                {
                    List<HIS_TRACKING> lstTracking = new List<HIS_TRACKING>();
                    if (this.trackings != null && this.trackings.Count > 0)
                    {
                        lstTracking = this.trackings.Where(o => o.TRACKING_TIME.ToString().StartsWith(DateTime.Now.ToString("yyyyMMdd"))).ToList();
                    }

                    if (lstTracking != null && lstTracking.Count > 0)
                    {

                        lstTracking = lstTracking.OrderByDescending(o => o.TRACKING_TIME).ToList();

                        if (IsDefaultTracking == 1)
                        {
                            if (IsMineCheckedByDefault != 1)
                            {
                                cboTracking.EditValue = lstTracking.FirstOrDefault().ID;
                            }
                            else
                            {
                                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                cboTracking.EditValue = lstTracking.Where(o => o.CREATOR == loginName).OrderByDescending(p => p.TRACKING_TIME).FirstOrDefault().ID;
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
    }
}
