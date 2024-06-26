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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.CareCreate.ADO;
using HIS.Desktop.Plugins.CareCreate.Base;
using HIS.Desktop.Plugins.CareCreate.Resources;
using HIS.Desktop.Plugins.CareCreate.Validate.ValidationRule;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.CareCreate
{
    public partial class frmHisCareCreate : FormBase
    {
        private long currentTreatmentId;
        private V_HIS_TREATMENT_4 currentTreatment = null;
        private HIS_CARE_SUM currentCareSum = null;
        private HIS_CARE currentCare = null;
        private HIS_TRACKING currentTracking = null;
        private List<HIS_TRACKING> trackings = null;
        MOS.EFMODEL.DataModels.V_HIS_ROOM requestRoom;

        private HIS_DHST editDhst = null;

        private List<HIS_DHST> lstCurrentDhst = null;
        private List<HIS_CARE_DETAIL> lstCurrentCareDetail = null;

        private List<HIS_CARE_TEMP> lstCareTemp = null;
        private List<HIS_CARE_TYPE> lstCareType = null;
        private List<HIS_RATION_TIME> lstRationTime = null;

        Inventec.Desktop.Common.Modules.Module Module;
        internal List<MPS.Processor.Mps000069.PDO.CreatorADO> _CreatorADOs = null;
        internal List<MPS.Processor.Mps000069.PDO.CareDescription> _careDescription { get; set; }

        internal List<MPS.Processor.Mps000069.PDO.InstructionDescription> _instructionDescription { get; set; }
        internal List<HIS_AWARENESS> lstHisAwareness = new List<HIS_AWARENESS>();
        internal List<HIS_CARE_TYPE> lstHisCareType = new List<HIS_CARE_TYPE>();
        internal List<HIS_CARE> careByTreatmentHasIcd { get; set; }
        internal List<MPS.Processor.Mps000069.PDO.CareViewPrintADO> lstCareViewPrintADO { get; set; }
        internal List<MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO> lstCareDetailViewPrintADO { get; set; }
        DelegateSelectData delegateSelectData;
        internal int action = -1;
        int positionHandleControl = -1;
        private bool isInit = true;

        internal static long IsDefaultTracking;
        internal static long IsMineCheckedByDefault;

        bool isFirst = true;
        HisTreatmentBedRoomLViewFilter DataTransferTreatmentBedRoomFilter { get; set; }

        public frmHisCareCreate(Inventec.Desktop.Common.Modules.Module module, long treatmentId, DelegateSelectData _delegateSelectData, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(module)
        {
            InitializeComponent();
            this.currentTreatmentId = treatmentId;
            this.Module = module;
            this.delegateSelectData = _delegateSelectData;
            this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
            this.action = GlobalVariables.ActionAdd;
            ddBtnPrint.Enabled = false;
        }

        public frmHisCareCreate(Inventec.Desktop.Common.Modules.Module module, V_HIS_TREATMENT_4 treatment, DelegateSelectData _delegateSelectData, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(module)
        {
            InitializeComponent();
            this.currentTreatmentId = treatment.ID;
            this.currentTreatment = treatment;
            this.Module = module;
            this.delegateSelectData = _delegateSelectData;
            this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
            this.action = GlobalVariables.ActionAdd;
            ddBtnPrint.Enabled = false;
        }

        public frmHisCareCreate(Inventec.Desktop.Common.Modules.Module module, HIS_CARE_SUM careSum, DelegateSelectData _delegateSelectData, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(module)
        {
            InitializeComponent();
            this.currentTreatmentId = careSum.TREATMENT_ID;
            this.currentCareSum = careSum;
            this.Module = module;
            this.delegateSelectData = _delegateSelectData;
            this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
            this.action = GlobalVariables.ActionAdd;
            ddBtnPrint.Enabled = false;
        }

        public frmHisCareCreate(Inventec.Desktop.Common.Modules.Module module, HIS_CARE care, DelegateSelectData _delegateSelectData, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(module)
        {
            InitializeComponent();
            this.currentTreatmentId = care.TREATMENT_ID;
            this.currentCare = care;
            this.Module = module;
            this.delegateSelectData = _delegateSelectData;
            this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
            this.action = GlobalVariables.ActionEdit;
            ddBtnPrint.Enabled = true;
        }

        public frmHisCareCreate(Inventec.Desktop.Common.Modules.Module module, HIS_TRACKING tracking, DelegateSelectData _delegateSelectData, HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter = null)
            : base(module)
        {
            InitializeComponent();
            this.currentTreatmentId = tracking.TREATMENT_ID;
            this.currentTracking = tracking;
            this.Module = module;
            this.delegateSelectData = _delegateSelectData;
            this.DataTransferTreatmentBedRoomFilter = dataTransferTreatmentBedRoomFilter;
            this.action = GlobalVariables.ActionAdd;
            ddBtnPrint.Enabled = false;
        }

        private void frmHisCareCreate_Load(object sender, EventArgs e)
        {
            try
            {
                IsDefaultTracking = HisConfigs.Get<long>(SdaConfigKeys.CONFIG_KEY__IS_DEFAULT_TRACKING);
                IsMineCheckedByDefault = HisConfigs.Get<long>(SdaConfigKeys.CONFIG_KEY__IS_MINE_CHECKED_BY_DEFAULT);
                dtExecuteTime.EditValue = DateTime.Now;
                LoadData();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCareTemp()
        {
            try
            {
                HisCareTempFilter tempFilter = new HisCareTempFilter();
                tempFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                tempFilter.ORDER_FIELD = "CARE_TEMP_NAME";
                tempFilter.ORDER_DIRECTION = "ASC";

                lstCareTemp = new BackendAdapter(new CommonParam()).Get<List<HIS_CARE_TEMP>>("api/HisCareTemp/Get", ApiConsumer.ApiConsumers.MosConsumer, tempFilter, null);

                lstCareTemp = lstCareTemp != null ? lstCareTemp.Where(o => o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || o.IS_PUBLIC == 1).ToList() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCareType()
        {
            try
            {
                HisCareTypeFilter careTypeFilter = new HisCareTypeFilter();
                careTypeFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                careTypeFilter.ORDER_DIRECTION = "DESC";
                careTypeFilter.ORDER_FIELD = "CARE_TYPE_CODE";
                this.lstCareType = new BackendAdapter(new CommonParam()).Get<List<HIS_CARE_TYPE>>(HisRequestUriStore.HIS_CARE_TYPE_GET, ApiConsumers.MosConsumer, careTypeFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadRationTime()
        {
            try
            {
                HisRationTimeFilter timeFilter = new HisRationTimeFilter();
                timeFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.lstRationTime = new BackendAdapter(new CommonParam()).Get<List<HIS_RATION_TIME>>("api/HisRationTime/Get", ApiConsumers.MosConsumer, timeFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboCareTemp()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CARE_TEMP_CODE", "", 100, 2));
                columnInfos.Add(new ColumnInfo("CARE_TEMP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CARE_TEMP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboCareTemp, this.lstCareTemp, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCareType()
        {
            try
            {
                repositoryItemCbo_CareDetail_CareType.DataSource = this.lstCareType;
                repositoryItemCbo_CareDetail_CareType.DisplayMember = "CARE_TYPE_NAME";
                repositoryItemCbo_CareDetail_CareType.ValueMember = "ID";
                repositoryItemCbo_CareDetail_CareType.NullText = "";

                repositoryItemCbo_CareDetail_CareType.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repositoryItemCbo_CareDetail_CareType.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repositoryItemCbo_CareDetail_CareType.ImmediatePopup = true;
                repositoryItemCbo_CareDetail_CareType.View.Columns.Clear();

                GridColumn aColumnCode = repositoryItemCbo_CareDetail_CareType.View.Columns.AddField("CARE_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = false;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = repositoryItemCbo_CareDetail_CareType.View.Columns.AddField("CARE_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 400;

                repositoryItemCbo_CareDetail_CareType.View.OptionsView.ShowColumnHeaders = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidExecuteTime();
                ValidateSpinCanNang();
                ValidateSpinChieuCao();
                ValidateSpinDiemDau();
                ValidateSpinDhstMach();
                ValidateSpinDhstNhietDo();
                ValidateSpinDhstHuyetAp_Max();
                ValidateSpinDhstHuyetAp_Min();
                ValidateSpinDhstNhipTho();
                ValidateSpinDhstCanNang();
                ValidateSpinDhstChieuCao();
                ValidateSpinDhstVongNguc();
                ValidateSpinDhstVongBung();
                ValidateSpinDhstSpo2();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateSpinDhstSpo2()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtDhstSpo2;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider2.SetValidationRule(txtDhstSpo2, spinEdit);
        }

        private void ValidateSpinDhstVongBung()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtDhstVongBung;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider2.SetValidationRule(txtDhstVongBung, spinEdit);
        }

        private void ValidateSpinDhstVongNguc()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtDhstVongNguc;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider2.SetValidationRule(txtDhstVongNguc, spinEdit);
        }

        private void ValidateSpinDhstChieuCao()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtDhstChieuCao;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider2.SetValidationRule(txtDhstChieuCao, spinEdit);
        }

        private void ValidateSpinDhstCanNang()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtDhstCanNang;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider2.SetValidationRule(txtDhstCanNang, spinEdit);
        }

        private void ValidateSpinDhstNhipTho()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtDhstNhipTho;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider2.SetValidationRule(txtDhstNhipTho, spinEdit);
        }

        private void ValidateSpinDhstHuyetAp_Min()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtDhstHuyetAp_Min;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider2.SetValidationRule(txtDhstHuyetAp_Min, spinEdit);
        }

        private void ValidateSpinDhstHuyetAp_Max()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtDhstHuyetAp_Max;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider2.SetValidationRule(txtDhstHuyetAp_Max, spinEdit);
        }

        private void ValidateSpinDhstNhietDo()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtDhstNhietDo;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider2.SetValidationRule(txtDhstNhietDo, spinEdit);
        }

        private void ValidateSpinDhstMach()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtDhstMach;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider2.SetValidationRule(txtDhstMach, spinEdit);
        }

        private void ValidateSpinDiemDau()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtDiemDau;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(txtDiemDau, spinEdit);
        }

        private void ValidateSpinChieuCao()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtTheTrang_ChieuCao;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(txtTheTrang_ChieuCao, spinEdit);
        }

        private void ValidateSpinCanNang()
        {
            SpinEditValidationRule spinEdit = new SpinEditValidationRule();
            spinEdit.spinEdit = txtTheTrang_CanNang;
            spinEdit.ErrorText = ResourceMessage.SpinEdit__Dhst__KhongDuocNhapSoAm;
            spinEdit.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(txtTheTrang_CanNang, spinEdit);
        }

        private void ValidExecuteTime()
        {
            CareExecuteTime__ValidationRule oDobDateRule = new CareExecuteTime__ValidationRule();
            oDobDateRule.dtExecuteTime = dtExecuteTime;
            oDobDateRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.NguoiDungNhapNgayKhongHopLe);
            oDobDateRule.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(dtExecuteTime, oDobDateRule);
        }

        private void LoadPatientInfor()
        {
            try
            {
                HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                treatFilter.ID = this.currentTreatmentId;

                List<HIS_TREATMENT> lstTreatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatFilter, null);

                HIS_TREATMENT treatment = lstTreatment != null ? lstTreatment.FirstOrDefault() : null;

                if (treatment != null)
                {
                    lblPatientCode.Text = treatment.TDL_PATIENT_CODE;
                    lblPatientName.Text = treatment.TDL_PATIENT_NAME;
                    lblGender.Text = treatment.TDL_PATIENT_GENDER_NAME;

                    if (treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        lblDob.Text = treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                HIS_CARE defaultCare = null;
                HIS_DHST defaultDhst = null;

                if (this.currentCare != null)
                {
                    defaultCare = this.currentCare;
                }
                else if (this.currentTracking != null)
                {
                    this.dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentTracking.TRACKING_TIME).Value;
                    HisCareFilter careFilter = new HisCareFilter();
                    careFilter.TRACKING_ID = this.currentTracking.ID;
                    List<HIS_CARE> careTrackings = new BackendAdapter(new CommonParam()).Get<List<HIS_CARE>>(HisRequestUriStore.HIS_CARE_GET, ApiConsumers.MosConsumer, careFilter, null);
                    this.currentCare = careTrackings != null ? careTrackings.FirstOrDefault() : null;
                    defaultCare = this.currentCare;

                    if (defaultCare == null)
                    {
                        HisDhstFilter dhstFilter = new HisDhstFilter();
                        dhstFilter.TRACKING_ID = this.currentTracking.ID;
                        List<HIS_DHST> hisDhsts = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, null);
                        defaultDhst = hisDhsts != null ? hisDhsts.OrderByDescending(o => o.EXECUTE_TIME).FirstOrDefault() : null;
                    }

                    if (this.currentCare != null)
                    {
                        this.action = GlobalVariables.ActionEdit;
                        ddBtnPrint.Enabled = true;
                    }

                }
                else if (this.currentTreatmentId > 0)
                {
                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.TREATMENT_ID = this.currentTreatmentId;
                    List<HIS_DHST> hisDhsts = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, null);
                    defaultDhst = hisDhsts != null ? hisDhsts.OrderByDescending(o => o.EXECUTE_TIME).FirstOrDefault() : null;

                }

                this.requestRoom = GetRequestRoom(this.Module.RoomId);

                this.InitComboTracking(this.currentTreatmentId);

                this.LoadRationTime();
                this.LoadPatientInfor();
                this.LoadCareType();
                this.LoadCareTemp();

                this.Defaultspin();

                this.InitMenuToButtonPrint();
                this.InitComboCareType();
                this.InitComboUser();
                this.InitComboCareTemp();

                this.LoadDhst();
                this.LoadCareDetail(null);

                this.FillDefaultDhst(defaultDhst);

                this.btnDhstAdd.Enabled = true;
                this.btnDhstEdit.Enabled = false;

                this.FillDefaultCare(defaultCare, null);
                this.FillDefaultNutrition(defaultCare);

                this.FocusDefault();
                this.ValidControl();

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

        private void FocusDefault()
        {
            try
            {
                txtCareTempCode.Focus();
                txtCareTempCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void Defaultspin()
        {
            try
            {
                txtTheTrang_CanNang.EditValue = null;
                txtTheTrang_ChieuCao.EditValue = null;
                txtDiemDau.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDhst()
        {
            try
            {
                this.lstCurrentDhst = new List<HIS_DHST>();
                if (this.currentCare != null)
                {
                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.CARE_ID = this.currentCare.ID;

                    List<HIS_DHST> dhsts = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, null);
                    if (dhsts != null) this.lstCurrentDhst = dhsts.OrderByDescending(o => o.EXECUTE_TIME).ToList();

                }

                gridControlDhst.BeginUpdate();
                gridControlDhst.DataSource = this.lstCurrentDhst;
                gridControlDhst.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCareDetail(HIS_CARE_TEMP careTemp)
        {
            try
            {
                this.lstCurrentCareDetail = new List<HIS_CARE_DETAIL>();
                if (careTemp != null)
                {
                    HisCareTempDetailFilter detailFilter = new HisCareTempDetailFilter();
                    detailFilter.CARE_TEMP_ID = careTemp.ID;

                    List<HIS_CARE_TEMP_DETAIL> details = new BackendAdapter(new CommonParam()).Get<List<HIS_CARE_TEMP_DETAIL>>("api/HisCareTempDetail/Get", ApiConsumers.MosConsumer, detailFilter, null);
                    if (details != null)
                    {
                        foreach (HIS_CARE_TEMP_DETAIL item in details)
                        {
                            HIS_CARE_DETAIL d = new HIS_CARE_DETAIL();
                            d.ID = item.CARE_TEMP_ID;
                            d.CARE_TYPE_ID = item.CARE_TYPE_ID;
                            d.CONTENT = item.CONTENT;
                            this.lstCurrentCareDetail.Add(d);
                        }
                    }
                }
                else if (this.currentCare != null)
                {
                    HisCareDetailFilter detailFilter = new HisCareDetailFilter();
                    detailFilter.CARE_ID = this.currentCare.ID;

                    List<HIS_CARE_DETAIL> details = new BackendAdapter(new CommonParam()).Get<List<HIS_CARE_DETAIL>>("api/HisCareDetail/Get", ApiConsumers.MosConsumer, detailFilter, null);
                    if (details != null) this.lstCurrentCareDetail = details;

                }

                if (this.lstCurrentCareDetail.Count == 0)
                {
                    this.lstCurrentCareDetail.Add(new HIS_CARE_DETAIL());
                }

                gridControlCareType.BeginUpdate();
                gridControlCareType.DataSource = this.lstCurrentCareDetail;
                gridControlCareType.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDefaultDhst(HIS_DHST defaultDhst)
        {
            try
            {
                this.dtDhstThoiGianDo.DateTime = this.dtExecuteTime.DateTime;
                if (defaultDhst != null)
                {
                    this.editDhst = defaultDhst;

                    this.txtDhstCanNang.EditValue = defaultDhst.WEIGHT;
                    this.txtDhstChieuCao.EditValue = defaultDhst.HEIGHT;
                    this.txtDhstHuyetAp_Min.EditValue = defaultDhst.BLOOD_PRESSURE_MIN;
                    this.txtDhstHuyetAp_Max.EditValue = defaultDhst.BLOOD_PRESSURE_MAX;
                    this.txtDhstKhac.Text = defaultDhst.NOTE;
                    this.txtDhstMach.EditValue = defaultDhst.PULSE;
                    this.txtDhstNhietDo.EditValue = defaultDhst.TEMPERATURE;
                    this.txtDhstNhipTho.EditValue = defaultDhst.BREATH_RATE;
                    this.txtDhstSpo2.EditValue = defaultDhst.SPO2;
                    this.txtDhstVongBung.EditValue = defaultDhst.BELLY;
                    this.txtDhstVongNguc.EditValue = defaultDhst.CHEST;

                    if (defaultDhst.INFUTION_INTO.HasValue)
                    {
                        if (defaultDhst.INFUTION_INTO.Value == DhstConstant.INFUTION_INTO__TRUYEN)
                        {
                            checkDhstDichVao_Truyen.Checked = true;
                        }
                        else if (defaultDhst.INFUTION_INTO.Value == DhstConstant.INFUTION_INTO__AN)
                        {
                            checkDhstDichVao_An.Checked = true;
                        }
                        else if (defaultDhst.INFUTION_INTO.Value == DhstConstant.INFUTION_INTO__KHAC)
                        {
                            checkDhstDichVao_Khac.Checked = true;
                        }
                    }

                    if (defaultDhst.INFUTION_OUT.HasValue)
                    {
                        if (defaultDhst.INFUTION_OUT.Value == DhstConstant.INFUTION_OUT__NUOC_TIEU)
                        {
                            checkDhstDichRa_NuocTieu.Checked = true;
                        }
                        else if (defaultDhst.INFUTION_OUT.Value == DhstConstant.INFUTION_OUT__DAN_LUU)
                        {
                            checkDhstDichRa_DanLuu.Checked = true;
                        }
                        else if (defaultDhst.INFUTION_OUT.Value == DhstConstant.INFUTION_OUT__PHAN)
                        {
                            checkDhstDichRa_Phan.Checked = true;
                        }
                    }

                    this.btnDhstAdd.Enabled = false;
                    this.btnDhstEdit.Enabled = true;
                }
                else
                {
                    this.txtDhstCanNang.EditValue = null;
                    this.txtDhstChieuCao.EditValue = null;
                    this.txtDhstHuyetAp_Min.EditValue = null;
                    this.txtDhstHuyetAp_Max.EditValue = null;
                    this.txtDhstKhac.Text = "";
                    this.txtDhstMach.EditValue = null;
                    this.txtDhstNhietDo.EditValue = null;
                    this.txtDhstNhipTho.EditValue = null;
                    this.txtDhstSpo2.EditValue = null;
                    this.txtDhstVongBung.EditValue = null;
                    this.txtDhstVongNguc.EditValue = null;

                    checkDhstDichVao_Truyen.Checked = false;
                    checkDhstDichVao_An.Checked = false;
                    checkDhstDichVao_Khac.Checked = false;
                    checkDhstDichRa_NuocTieu.Checked = false;
                    checkDhstDichRa_DanLuu.Checked = false;
                    checkDhstDichRa_Phan.Checked = false;

                    this.btnDhstAdd.Enabled = true;
                    this.btnDhstEdit.Enabled = false;
                }
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider2, dxErrorProvider1);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDefaultCare(HIS_CARE defaultCare, HIS_CARE_TEMP careTemp)
        {
            try
            {
                if (defaultCare != null)
                {
                    if (defaultCare.EXECUTE_TIME.HasValue)
                        dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(defaultCare.EXECUTE_TIME.Value).Value;

                    if (defaultCare.ALLERGY_HISTORY.HasValue)
                    {
                        if (defaultCare.ALLERGY_HISTORY.Value == CareConstant.ALLERGY_HISTORY__CO)
                        {
                            checkTienSuDiUng_Co.Checked = true;
                        }
                        else if (defaultCare.ALLERGY_HISTORY.Value == CareConstant.ALLERGY_HISTORY__KHONG)
                        {
                            checkTienSuDiUng_Khong.Checked = true;
                        }
                        else
                        {
                            checkTienSuDiUng_Co.Checked = false;
                            checkTienSuDiUng_Khong.Checked = false;
                        }
                    }
                    else
                    {
                        checkTienSuDiUng_Co.Checked = false;
                        checkTienSuDiUng_Khong.Checked = false;
                    }

                    if (defaultCare.AWARENESS_CONDITION.HasValue)
                    {
                        if (defaultCare.AWARENESS_CONDITION.Value == CareConstant.AWARENESS_CONDITION__HON_ME)
                        {
                            checkYThuc_HonMe.Checked = true;
                        }
                        else if (defaultCare.AWARENESS_CONDITION.Value == CareConstant.AWARENESS_CONDITION__LO_MO)
                        {
                            checkYThuc_LoMo.Checked = true;
                        }
                        else if (defaultCare.AWARENESS_CONDITION.Value == CareConstant.AWARENESS_CONDITION__TINH)
                        {
                            checkYThuc_Tinh.Checked = true;
                        }
                        else
                        {
                            checkYThuc_HonMe.Checked = false;
                            checkYThuc_LoMo.Checked = false;
                            checkYThuc_Tinh.Checked = false;
                            txtYThuc_Khac.Text = defaultCare.AWARENESS ?? "";
                        }
                    }
                    else
                    {
                        checkYThuc_HonMe.Checked = false;
                        checkYThuc_LoMo.Checked = false;
                        checkYThuc_Tinh.Checked = false;
                        txtYThuc_Khac.Text = defaultCare.AWARENESS ?? "";
                    }

                    if (defaultCare.BODY_CONDITION.HasValue)
                    {
                        if (defaultCare.BODY_CONDITION.Value == CareConstant.BODY_CONDITION__BINH_THUONG)
                        {
                            checkToanTrang_BinhThuong.Checked = true;
                        }
                        else if (defaultCare.BODY_CONDITION.Value == CareConstant.BODY_CONDITION__MET)
                        {
                            checkToanTrang_Met.Checked = true;
                        }
                        else
                        {
                            checkToanTrang_BinhThuong.Checked = false;
                            checkToanTrang_Met.Checked = false;
                        }
                    }
                    else
                    {
                        checkToanTrang_BinhThuong.Checked = false;
                        checkToanTrang_Met.Checked = false;
                    }

                    if (defaultCare.CAN_ULCERS.HasValue)
                    {
                        if (defaultCare.CAN_ULCERS.Value == CareConstant.CAN_ULCERS__CO)
                        {
                            checkNguyCoLoet_Co.Checked = true;
                        }
                        else if (defaultCare.CAN_ULCERS.Value == CareConstant.CAN_ULCERS__KHONG)
                        {
                            checkNguyCoLoet_Khong.Checked = true;
                        }
                        else
                        {
                            checkNguyCoLoet_Co.Checked = false;
                            checkNguyCoLoet_Khong.Checked = false;
                        }
                    }
                    else
                    {
                        checkNguyCoLoet_Co.Checked = false;
                        checkNguyCoLoet_Khong.Checked = false;
                    }

                    txtDienBienKhac.Text = defaultCare.CARE_DESCRIPTION ?? "";

                    if (defaultCare.DEFECATE_CONDITION.HasValue)
                    {
                        if (defaultCare.DEFECATE_CONDITION.Value == CareConstant.DEFECATE_CONDITION__BINH_THUONG)
                        {
                            checkDaiTien_BinhThuong.Checked = true;
                        }
                        else if (defaultCare.DEFECATE_CONDITION.Value == CareConstant.DEFECATE_CONDITION__BAT_THUONG)
                        {
                            checkDaiTien_BatThuong.Checked = true;
                        }
                        else
                        {
                            checkDaiTien_BinhThuong.Checked = false;
                            checkDaiTien_BatThuong.Checked = false;
                            txtDaiTien_Khac.Text = defaultCare.DEJECTA ?? "";
                        }
                    }
                    else
                    {
                        checkDaiTien_BinhThuong.Checked = false;
                        checkDaiTien_BatThuong.Checked = false;
                        txtDaiTien_Khac.Text = defaultCare.DEJECTA ?? "";
                    }

                    if (defaultCare.DIGEST_CONDITION.HasValue)
                    {
                        if (defaultCare.DIGEST_CONDITION.Value == CareConstant.DIGEST_CONDITION__TU_AN)
                        {
                            checkTieuHoa_TuAn.Checked = true;
                        }
                        else if (defaultCare.DIGEST_CONDITION.Value == CareConstant.DIGEST_CONDITION__QUA_SONDE)
                        {
                            checkTieuHoa_QuaSonde.Checked = true;
                        }
                        else if (defaultCare.DIGEST_CONDITION.Value == CareConstant.DIGEST_CONDITION__NON)
                        {
                            checkTieuHoa_Non.Checked = true;
                        }
                        else
                        {
                            checkTieuHoa_TuAn.Checked = false;
                            checkTieuHoa_QuaSonde.Checked = false;
                            checkTieuHoa_Non.Checked = false;
                        }
                    }
                    else
                    {
                        checkTieuHoa_TuAn.Checked = false;
                        checkTieuHoa_QuaSonde.Checked = false;
                        checkTieuHoa_Non.Checked = false;
                    }

                    if (defaultCare.EDUCATION_CONDITION.HasValue)
                    {
                        if (defaultCare.EDUCATION_CONDITION.Value == CareConstant.EDUCATION_CONDITION__CO)
                        {
                            checkGiaoDucSucKhoe_Co.Checked = true;
                        }
                        else if (defaultCare.EDUCATION_CONDITION.Value == CareConstant.EDUCATION_CONDITION__KHONG)
                        {
                            checkGiaoDucSucKhoe_Khong.Checked = true;
                        }
                        else
                        {
                            checkGiaoDucSucKhoe_Co.Checked = false;
                            checkGiaoDucSucKhoe_Khong.Checked = false;
                            txtGiaoDucSucKhoe_Khac.Text = defaultCare.EDUCATION ?? "";
                        }
                    }
                    else
                    {
                        checkGiaoDucSucKhoe_Co.Checked = false;
                        checkGiaoDucSucKhoe_Khong.Checked = false;
                        txtGiaoDucSucKhoe_Khac.Text = defaultCare.EDUCATION ?? "";
                    }

                    txtDanhGia.Text = defaultCare.EVALUTE_CARE ?? "";

                    if (defaultCare.FUNCTION_CONDITION.HasValue)
                    {
                        if (defaultCare.FUNCTION_CONDITION.Value == CareConstant.FUNCTION_CONDITION__DAU)
                        {
                            checkCoNang_Dau.Checked = true;
                        }
                        else if (defaultCare.FUNCTION_CONDITION.Value == CareConstant.FUNCTION_CONDITION__NGUA)
                        {
                            checkCoNang_Ngua.Checked = true;
                        }
                        else if (defaultCare.FUNCTION_CONDITION.Value == CareConstant.FUNCTION_CONDITION__RAT)
                        {
                            checkCoNang_Rat.Checked = true;
                        }
                        else
                        {
                            checkCoNang_Dau.Checked = false;
                            checkCoNang_Ngua.Checked = false;
                            checkCoNang_Rat.Checked = false;
                        }
                    }
                    else
                    {
                        checkCoNang_Dau.Checked = false;
                        checkCoNang_Ngua.Checked = false;
                        checkCoNang_Rat.Checked = false;
                    }

                    checkThuocBoSung.Checked = defaultCare.HAS_ADD_MEDICINE == CareConstant.IS_TRUE;
                    checkThayBang.Checked = defaultCare.HAS_CHANGE_BANDAGE == CareConstant.IS_TRUE;
                    checkCatChi.Checked = defaultCare.HAS_CUT_SUTURE == CareConstant.IS_TRUE;
                    checkCDHA.Checked = defaultCare.HAS_DIIM == CareConstant.IS_TRUE;
                    checkThuocUong.Checked = defaultCare.HAS_DRINK_MEDICINE == CareConstant.IS_TRUE;
                    checkTDCN.Checked = defaultCare.HAS_FUEX == CareConstant.IS_TRUE;
                    checkThuocTiemTruyen.Checked = defaultCare.HAS_INFUTION_MEDICINE == CareConstant.IS_TRUE;
                    checkThuocThuongQuy.Checked = defaultCare.HAS_MEDICINE == CareConstant.IS_TRUE;
                    checkPHCN.Checked = defaultCare.HAS_REHABILITATION == CareConstant.IS_TRUE;
                    checkXetNghiem.Checked = defaultCare.HAS_TEST == CareConstant.IS_TRUE;
                    checkThuocBoiDap.Checked = defaultCare.HAS_TOPICAL_MEDICINE == CareConstant.IS_TRUE;

                    txtTheTrang_CanNang.EditValue = defaultCare.WEIGHT;
                    txtTheTrang_ChieuCao.EditValue = defaultCare.HEIGHT;

                    txtCanLamSangKhac.Text = defaultCare.INSTRUCTION_DESCRIPTION ?? "";

                    if (defaultCare.MUCOCUTANEOUS_CONDITION.HasValue)
                    {
                        if (defaultCare.MUCOCUTANEOUS_CONDITION.Value == CareConstant.MUCOCUTANEOUS_CONDITION__BINH_THUONG)
                        {
                            checkDaNiemMac_BinhThuong.Checked = true;
                        }
                        else if (defaultCare.MUCOCUTANEOUS_CONDITION.Value == CareConstant.MUCOCUTANEOUS_CONDITION__HONG)
                        {
                            checkDaNiemMac_Hong.Checked = true;
                        }
                        else if (defaultCare.MUCOCUTANEOUS_CONDITION.Value == CareConstant.MUCOCUTANEOUS_CONDITION__NHOT)
                        {
                            checkDaNiemMac_Nhot.Checked = true;
                        }
                        else
                        {
                            checkDaNiemMac_BinhThuong.Checked = false;
                            checkDaNiemMac_Hong.Checked = false;
                            checkDaNiemMac_Nhot.Checked = false;
                            txtDaNiemMac_Khac.Text = defaultCare.MUCOCUTANEOUS ?? "";
                        }
                    }
                    else
                    {
                        checkDaNiemMac_BinhThuong.Checked = false;
                        checkDaNiemMac_Hong.Checked = false;
                        checkDaNiemMac_Nhot.Checked = false;
                        txtDaNiemMac_Khac.Text = defaultCare.MUCOCUTANEOUS ?? "";
                    }

                    if (defaultCare.NEUROLOGICAL_CONDITION.HasValue)
                    {
                        if (defaultCare.NEUROLOGICAL_CONDITION.Value == CareConstant.NEUROLOGICAL_CONDITION__CO_GIAT)
                        {
                            checkThanKinh_CoGiat.Checked = true;
                        }
                        else if (defaultCare.NEUROLOGICAL_CONDITION.Value == CareConstant.NEUROLOGICAL_CONDITION__TANG_TLC)
                        {
                            checkThanKinh_TangTLC.Checked = true;
                        }
                        else
                        {
                            checkThanKinh_CoGiat.Checked = false;
                            checkThanKinh_TangTLC.Checked = false;
                        }
                    }
                    else
                    {
                        checkThanKinh_CoGiat.Checked = false;
                        checkThanKinh_TangTLC.Checked = false;
                    }

                    txtDinhDuong.Text = defaultCare.NUTRITION ?? "";
                    txtChamSocKhac.Text = defaultCare.OTHER_CARE ?? "";
                    txtDanhGia.Text = defaultCare.EVALUTE_CARE ?? "";
                    txtDiemDau.EditValue = defaultCare.PAIN_SCORE;

                    if (defaultCare.RESPIRATORY_CONDITION.HasValue)
                    {
                        if (defaultCare.RESPIRATORY_CONDITION.Value == CareConstant.RESPIRATORY_CONDITION__THO_OXY)
                        {
                            checkHoHap_ThoOxy.Checked = true;
                        }
                        else if (defaultCare.RESPIRATORY_CONDITION.Value == CareConstant.RESPIRATORY_CONDITION__TU_THO)
                        {
                            checkHoHap_TuTho.Checked = true;
                        }
                        else
                        {
                            checkHoHap_ThoOxy.Checked = false;
                            checkHoHap_TuTho.Checked = false;
                        }
                    }
                    else
                    {
                        checkHoHap_ThoOxy.Checked = false;
                        checkHoHap_TuTho.Checked = false;
                    }

                    if (defaultCare.SANITARY_CONDITION.HasValue)
                    {
                        if (defaultCare.SANITARY_CONDITION.Value == CareConstant.SANITARY_CONDITION__TAM_THUONG)
                        {
                            checkVeSinh_TamThuong.Checked = true;
                        }
                        else if (defaultCare.SANITARY_CONDITION.Value == CareConstant.SANITARY_CONDITION__TAM_THUOC)
                        {
                            checkVeSinh_TamThuoc.Checked = true;
                        }
                        else
                        {
                            checkVeSinh_TamThuong.Checked = false;
                            checkVeSinh_TamThuoc.Checked = false;
                            txtVeSinh_Khac.Text = defaultCare.SANITARY ?? "";
                        }
                    }
                    else
                    {
                        checkVeSinh_TamThuong.Checked = false;
                        checkVeSinh_TamThuoc.Checked = false;
                        txtVeSinh_Khac.Text = defaultCare.SANITARY ?? "";
                    }

                    if (defaultCare.SKIN_DAMAGE_CONDITION.HasValue)
                    {
                        if (defaultCare.SKIN_DAMAGE_CONDITION.Value == CareConstant.SKIN_DAMAGE_CONDITION__GIAM)
                        {
                            checkTonThuongDa_Giam.Checked = true;
                        }
                        else if (defaultCare.SKIN_DAMAGE_CONDITION.Value == CareConstant.SKIN_DAMAGE_CONDITION__KHO)
                        {
                            checkTonThuongDa_Kho.Checked = true;
                        }
                        else if (defaultCare.SKIN_DAMAGE_CONDITION.Value == CareConstant.SKIN_DAMAGE_CONDITION__ON_DINH)
                        {
                            checkTonThuongDa_OnDinh.Checked = true;
                        }
                        else if (defaultCare.SKIN_DAMAGE_CONDITION.Value == CareConstant.SKIN_DAMAGE_CONDITION__TANG)
                        {
                            checkTonThuongDa_Tang.Checked = true;
                        }
                        else if (defaultCare.SKIN_DAMAGE_CONDITION.Value == CareConstant.SKIN_DAMAGE_CONDITION__UOT)
                        {
                            checkTonThuongDa_Uot.Checked = true;
                        }
                        else
                        {
                            checkTonThuongDa_Giam.Checked = false;
                            checkTonThuongDa_Kho.Checked = false;
                            checkTonThuongDa_OnDinh.Checked = false;
                            checkTonThuongDa_Tang.Checked = false;
                            checkTonThuongDa_Uot.Checked = false;
                        }
                    }
                    else
                    {
                        checkTonThuongDa_Giam.Checked = false;
                        checkTonThuongDa_Kho.Checked = false;
                        checkTonThuongDa_OnDinh.Checked = false;
                        checkTonThuongDa_Tang.Checked = false;
                        checkTonThuongDa_Uot.Checked = false;
                    }

                    if (defaultCare.TUTORIAL_CONDITION.HasValue)
                    {
                        if (defaultCare.TUTORIAL_CONDITION.Value == CareConstant.TUTORIAL_CONDITION__CO)
                        {
                            checkPhoBienNoiQuy_Co.Checked = true;
                        }
                        else if (defaultCare.TUTORIAL_CONDITION.Value == CareConstant.TUTORIAL_CONDITION__KHONG)
                        {
                            checkPhoBienNoiQuy_Khong.Checked = true;
                        }
                        else
                        {
                            checkPhoBienNoiQuy_Co.Checked = false;
                            checkPhoBienNoiQuy_Khong.Checked = false;
                            txtPhoBienNoiQuy_Khac.Text = defaultCare.TUTORIAL ?? "";
                        }
                    }
                    else
                    {
                        checkPhoBienNoiQuy_Co.Checked = false;
                        checkPhoBienNoiQuy_Khong.Checked = false;
                        txtPhoBienNoiQuy_Khac.Text = defaultCare.TUTORIAL ?? "";
                    }

                    if (defaultCare.URINE_CONDITION.HasValue)
                    {
                        if (defaultCare.URINE_CONDITION.Value == CareConstant.URINE_CONDITION__BAT_THUONG)
                        {
                            checkTieuTien_BatThuong.Checked = true;
                        }
                        else if (defaultCare.URINE_CONDITION.Value == CareConstant.URINE_CONDITION__BINH_THUONG)
                        {
                            checkTieuTien_BinhThuong.Checked = true;
                        }
                        else
                        {
                            checkTieuTien_BatThuong.Checked = false;
                            checkTieuTien_BinhThuong.Checked = false;
                            txtTieuTien_Khac.Text = defaultCare.URINE ?? "";
                        }
                    }
                    else
                    {
                        checkTieuTien_BatThuong.Checked = false;
                        checkTieuTien_BinhThuong.Checked = false;
                        txtTieuTien_Khac.Text = defaultCare.URINE ?? "";
                    }
                }
                else if (careTemp != null)
                {
                    if (careTemp.ALLERGY_HISTORY.HasValue)
                    {
                        if (careTemp.ALLERGY_HISTORY.Value == CareConstant.ALLERGY_HISTORY__CO)
                        {
                            checkTienSuDiUng_Co.Checked = true;
                        }
                        else if (careTemp.ALLERGY_HISTORY.Value == CareConstant.ALLERGY_HISTORY__KHONG)
                        {
                            checkTienSuDiUng_Khong.Checked = true;
                        }
                        else
                        {
                            checkTienSuDiUng_Co.Checked = false;
                            checkTienSuDiUng_Khong.Checked = false;
                        }
                    }
                    else
                    {
                        checkTienSuDiUng_Co.Checked = false;
                        checkTienSuDiUng_Khong.Checked = false;
                    }

                    if (careTemp.AWARENESS_CONDITION.HasValue)
                    {
                        if (careTemp.AWARENESS_CONDITION.Value == CareConstant.AWARENESS_CONDITION__HON_ME)
                        {
                            checkYThuc_HonMe.Checked = true;
                        }
                        else if (careTemp.AWARENESS_CONDITION.Value == CareConstant.AWARENESS_CONDITION__LO_MO)
                        {
                            checkYThuc_LoMo.Checked = true;
                        }
                        else if (careTemp.AWARENESS_CONDITION.Value == CareConstant.AWARENESS_CONDITION__TINH)
                        {
                            checkYThuc_Tinh.Checked = true;
                        }
                        else
                        {
                            checkYThuc_HonMe.Checked = false;
                            checkYThuc_LoMo.Checked = false;
                            checkYThuc_Tinh.Checked = false;
                            txtYThuc_Khac.Text = careTemp.AWARENESS ?? "";
                        }
                    }
                    else
                    {
                        checkYThuc_HonMe.Checked = false;
                        checkYThuc_LoMo.Checked = false;
                        checkYThuc_Tinh.Checked = false;
                        txtYThuc_Khac.Text = careTemp.AWARENESS ?? "";
                    }

                    if (careTemp.BODY_CONDITION.HasValue)
                    {
                        if (careTemp.BODY_CONDITION.Value == CareConstant.BODY_CONDITION__BINH_THUONG)
                        {
                            checkToanTrang_BinhThuong.Checked = true;
                        }
                        else if (careTemp.BODY_CONDITION.Value == CareConstant.BODY_CONDITION__MET)
                        {
                            checkToanTrang_Met.Checked = true;
                        }
                        else
                        {
                            checkToanTrang_BinhThuong.Checked = false;
                            checkToanTrang_Met.Checked = false;
                        }
                    }
                    else
                    {
                        checkToanTrang_BinhThuong.Checked = false;
                        checkToanTrang_Met.Checked = false;
                    }

                    if (careTemp.CAN_ULCERS.HasValue)
                    {
                        if (careTemp.CAN_ULCERS.Value == CareConstant.CAN_ULCERS__CO)
                        {
                            checkNguyCoLoet_Co.Checked = true;
                        }
                        else if (careTemp.CAN_ULCERS.Value == CareConstant.CAN_ULCERS__KHONG)
                        {
                            checkNguyCoLoet_Khong.Checked = true;
                        }
                        else
                        {
                            checkNguyCoLoet_Co.Checked = false;
                            checkNguyCoLoet_Khong.Checked = false;
                        }
                    }
                    else
                    {
                        checkNguyCoLoet_Co.Checked = false;
                        checkNguyCoLoet_Khong.Checked = false;
                    }

                    txtDienBienKhac.Text = careTemp.CARE_DESCRIPTION ?? "";

                    if (careTemp.DEFECATE_CONDITION.HasValue)
                    {
                        if (careTemp.DEFECATE_CONDITION.Value == CareConstant.DEFECATE_CONDITION__BINH_THUONG)
                        {
                            checkDaiTien_BinhThuong.Checked = true;
                        }
                        else if (careTemp.DEFECATE_CONDITION.Value == CareConstant.DEFECATE_CONDITION__BAT_THUONG)
                        {
                            checkDaiTien_BatThuong.Checked = true;
                        }
                        else
                        {
                            checkDaiTien_BinhThuong.Checked = false;
                            checkDaiTien_BatThuong.Checked = false;
                            txtDaiTien_Khac.Text = careTemp.DEJECTA ?? "";
                        }
                    }
                    else
                    {
                        checkDaiTien_BinhThuong.Checked = false;
                        checkDaiTien_BatThuong.Checked = false;
                        txtDaiTien_Khac.Text = careTemp.DEJECTA ?? "";
                    }

                    if (careTemp.DIGEST_CONDITION.HasValue)
                    {
                        if (careTemp.DIGEST_CONDITION.Value == CareConstant.DIGEST_CONDITION__TU_AN)
                        {
                            checkTieuHoa_TuAn.Checked = true;
                        }
                        else if (careTemp.DIGEST_CONDITION.Value == CareConstant.DIGEST_CONDITION__QUA_SONDE)
                        {
                            checkTieuHoa_QuaSonde.Checked = true;
                        }
                        else if (careTemp.DIGEST_CONDITION.Value == CareConstant.DIGEST_CONDITION__NON)
                        {
                            checkTieuHoa_Non.Checked = true;
                        }
                        else
                        {
                            checkTieuHoa_TuAn.Checked = false;
                            checkTieuHoa_QuaSonde.Checked = false;
                            checkTieuHoa_Non.Checked = false;
                        }
                    }
                    else
                    {
                        checkTieuHoa_TuAn.Checked = false;
                        checkTieuHoa_QuaSonde.Checked = false;
                        checkTieuHoa_Non.Checked = false;
                    }

                    if (careTemp.EDUCATION_CONDITION.HasValue)
                    {
                        if (careTemp.EDUCATION_CONDITION.Value == CareConstant.EDUCATION_CONDITION__CO)
                        {
                            checkGiaoDucSucKhoe_Co.Checked = true;
                        }
                        else if (careTemp.EDUCATION_CONDITION.Value == CareConstant.EDUCATION_CONDITION__KHONG)
                        {
                            checkGiaoDucSucKhoe_Khong.Checked = true;
                        }
                        else
                        {
                            checkGiaoDucSucKhoe_Co.Checked = false;
                            checkGiaoDucSucKhoe_Khong.Checked = false;
                            txtGiaoDucSucKhoe_Khac.Text = careTemp.EDUCATION ?? "";
                        }
                    }
                    else
                    {
                        checkGiaoDucSucKhoe_Co.Checked = false;
                        checkGiaoDucSucKhoe_Khong.Checked = false;
                        txtGiaoDucSucKhoe_Khac.Text = careTemp.EDUCATION ?? "";
                    }

                    txtDanhGia.Text = careTemp.EVALUTE_CARE ?? "";

                    if (careTemp.FUNCTION_CONDITION.HasValue)
                    {
                        if (careTemp.FUNCTION_CONDITION.Value == CareConstant.FUNCTION_CONDITION__DAU)
                        {
                            checkCoNang_Dau.Checked = true;
                        }
                        else if (careTemp.FUNCTION_CONDITION.Value == CareConstant.FUNCTION_CONDITION__NGUA)
                        {
                            checkCoNang_Ngua.Checked = true;
                        }
                        else if (careTemp.FUNCTION_CONDITION.Value == CareConstant.FUNCTION_CONDITION__RAT)
                        {
                            checkCoNang_Rat.Checked = true;
                        }
                        else
                        {
                            checkCoNang_Dau.Checked = false;
                            checkCoNang_Ngua.Checked = false;
                            checkCoNang_Rat.Checked = false;
                        }
                    }
                    else
                    {
                        checkCoNang_Dau.Checked = false;
                        checkCoNang_Ngua.Checked = false;
                        checkCoNang_Rat.Checked = false;
                    }

                    checkThuocBoSung.Checked = careTemp.HAS_ADD_MEDICINE == CareConstant.IS_TRUE;
                    checkThayBang.Checked = careTemp.HAS_CHANGE_BANDAGE == CareConstant.IS_TRUE;
                    checkCatChi.Checked = careTemp.HAS_CUT_SUTURE == CareConstant.IS_TRUE;
                    checkCDHA.Checked = careTemp.HAS_DIIM == CareConstant.IS_TRUE;
                    checkThuocUong.Checked = careTemp.HAS_DRINK_MEDICINE == CareConstant.IS_TRUE;
                    checkTDCN.Checked = careTemp.HAS_FUEX == CareConstant.IS_TRUE;
                    checkThuocTiemTruyen.Checked = careTemp.HAS_INFUTION_MEDICINE == CareConstant.IS_TRUE;
                    checkThuocThuongQuy.Checked = careTemp.HAS_MEDICINE == CareConstant.IS_TRUE;
                    checkPHCN.Checked = careTemp.HAS_REHABILITATION == CareConstant.IS_TRUE;
                    checkXetNghiem.Checked = careTemp.HAS_TEST == CareConstant.IS_TRUE;
                    checkThuocBoiDap.Checked = careTemp.HAS_TOPICAL_MEDICINE == CareConstant.IS_TRUE;

                    txtTheTrang_CanNang.EditValue = careTemp.WEIGHT;
                    txtTheTrang_ChieuCao.EditValue = careTemp.HEIGHT;

                    txtCanLamSangKhac.Text = careTemp.INSTRUCTION_DESCRIPTION ?? "";

                    if (careTemp.MUCOCUTANEOUS_CONDITION.HasValue)
                    {
                        if (careTemp.MUCOCUTANEOUS_CONDITION.Value == CareConstant.MUCOCUTANEOUS_CONDITION__BINH_THUONG)
                        {
                            checkDaNiemMac_BinhThuong.Checked = true;
                        }
                        else if (careTemp.MUCOCUTANEOUS_CONDITION.Value == CareConstant.MUCOCUTANEOUS_CONDITION__HONG)
                        {
                            checkDaNiemMac_Hong.Checked = true;
                        }
                        else if (careTemp.MUCOCUTANEOUS_CONDITION.Value == CareConstant.MUCOCUTANEOUS_CONDITION__NHOT)
                        {
                            checkDaNiemMac_Nhot.Checked = true;
                        }
                        else
                        {
                            checkDaNiemMac_BinhThuong.Checked = false;
                            checkDaNiemMac_Hong.Checked = false;
                            checkDaNiemMac_Nhot.Checked = false;
                            txtDaNiemMac_Khac.Text = careTemp.MUCOCUTANEOUS ?? "";
                        }
                    }
                    else
                    {
                        checkDaNiemMac_BinhThuong.Checked = false;
                        checkDaNiemMac_Hong.Checked = false;
                        checkDaNiemMac_Nhot.Checked = false;
                        txtDaNiemMac_Khac.Text = careTemp.MUCOCUTANEOUS ?? "";
                    }

                    if (careTemp.NEUROLOGICAL_CONDITION.HasValue)
                    {
                        if (careTemp.NEUROLOGICAL_CONDITION.Value == CareConstant.NEUROLOGICAL_CONDITION__CO_GIAT)
                        {
                            checkThanKinh_CoGiat.Checked = true;
                        }
                        else if (careTemp.NEUROLOGICAL_CONDITION.Value == CareConstant.NEUROLOGICAL_CONDITION__TANG_TLC)
                        {
                            checkThanKinh_TangTLC.Checked = true;
                        }
                        else
                        {
                            checkThanKinh_CoGiat.Checked = false;
                            checkThanKinh_TangTLC.Checked = false;
                        }
                    }
                    else
                    {
                        checkThanKinh_CoGiat.Checked = false;
                        checkThanKinh_TangTLC.Checked = false;
                    }

                    txtDinhDuong.Text = careTemp.NUTRITION ?? "";
                    txtChamSocKhac.Text = careTemp.OTHER_CARE ?? "";
                    txtDanhGia.Text = careTemp.EVALUTE_CARE ?? "";
                    txtDiemDau.EditValue = careTemp.PAIN_SCORE;

                    if (careTemp.RESPIRATORY_CONDITION.HasValue)
                    {
                        if (careTemp.RESPIRATORY_CONDITION.Value == CareConstant.RESPIRATORY_CONDITION__THO_OXY)
                        {
                            checkHoHap_ThoOxy.Checked = true;
                        }
                        else if (careTemp.RESPIRATORY_CONDITION.Value == CareConstant.RESPIRATORY_CONDITION__TU_THO)
                        {
                            checkHoHap_TuTho.Checked = true;
                        }
                        else
                        {
                            checkHoHap_ThoOxy.Checked = false;
                            checkHoHap_TuTho.Checked = false;
                        }
                    }
                    else
                    {
                        checkHoHap_ThoOxy.Checked = false;
                        checkHoHap_TuTho.Checked = false;
                    }

                    if (careTemp.SANITARY_CONDITION.HasValue)
                    {
                        if (careTemp.SANITARY_CONDITION.Value == CareConstant.SANITARY_CONDITION__TAM_THUONG)
                        {
                            checkVeSinh_TamThuong.Checked = true;
                        }
                        else if (careTemp.SANITARY_CONDITION.Value == CareConstant.SANITARY_CONDITION__TAM_THUOC)
                        {
                            checkVeSinh_TamThuoc.Checked = true;
                        }
                        else
                        {
                            checkVeSinh_TamThuong.Checked = false;
                            checkVeSinh_TamThuoc.Checked = false;
                            txtVeSinh_Khac.Text = careTemp.SANITARY ?? "";
                        }
                    }
                    else
                    {
                        checkVeSinh_TamThuong.Checked = false;
                        checkVeSinh_TamThuoc.Checked = false;
                        txtVeSinh_Khac.Text = careTemp.SANITARY ?? "";
                    }

                    if (careTemp.SKIN_DAMAGE_CONDITION.HasValue)
                    {
                        if (careTemp.SKIN_DAMAGE_CONDITION.Value == CareConstant.SKIN_DAMAGE_CONDITION__GIAM)
                        {
                            checkTonThuongDa_Giam.Checked = true;
                        }
                        else if (careTemp.SKIN_DAMAGE_CONDITION.Value == CareConstant.SKIN_DAMAGE_CONDITION__KHO)
                        {
                            checkTonThuongDa_Kho.Checked = true;
                        }
                        else if (careTemp.SKIN_DAMAGE_CONDITION.Value == CareConstant.SKIN_DAMAGE_CONDITION__ON_DINH)
                        {
                            checkTonThuongDa_OnDinh.Checked = true;
                        }
                        else if (careTemp.SKIN_DAMAGE_CONDITION.Value == CareConstant.SKIN_DAMAGE_CONDITION__TANG)
                        {
                            checkTonThuongDa_Tang.Checked = true;
                        }
                        else if (careTemp.SKIN_DAMAGE_CONDITION.Value == CareConstant.SKIN_DAMAGE_CONDITION__UOT)
                        {
                            checkTonThuongDa_Uot.Checked = true;
                        }
                        else
                        {
                            checkTonThuongDa_Giam.Checked = false;
                            checkTonThuongDa_Kho.Checked = false;
                            checkTonThuongDa_OnDinh.Checked = false;
                            checkTonThuongDa_Tang.Checked = false;
                            checkTonThuongDa_Uot.Checked = false;
                        }
                    }
                    else
                    {
                        checkTonThuongDa_Giam.Checked = false;
                        checkTonThuongDa_Kho.Checked = false;
                        checkTonThuongDa_OnDinh.Checked = false;
                        checkTonThuongDa_Tang.Checked = false;
                        checkTonThuongDa_Uot.Checked = false;
                    }

                    if (careTemp.TUTORIAL_CONDITION.HasValue)
                    {
                        if (careTemp.TUTORIAL_CONDITION.Value == CareConstant.TUTORIAL_CONDITION__CO)
                        {
                            checkPhoBienNoiQuy_Co.Checked = true;
                        }
                        else if (careTemp.TUTORIAL_CONDITION.Value == CareConstant.TUTORIAL_CONDITION__KHONG)
                        {
                            checkPhoBienNoiQuy_Khong.Checked = true;
                        }
                        else
                        {
                            checkPhoBienNoiQuy_Co.Checked = false;
                            checkPhoBienNoiQuy_Khong.Checked = false;
                            txtPhoBienNoiQuy_Khac.Text = careTemp.TUTORIAL ?? "";
                        }
                    }
                    else
                    {
                        checkPhoBienNoiQuy_Co.Checked = false;
                        checkPhoBienNoiQuy_Khong.Checked = false;
                        txtPhoBienNoiQuy_Khac.Text = careTemp.TUTORIAL ?? "";
                    }

                    if (careTemp.URINE_CONDITION.HasValue)
                    {
                        if (careTemp.URINE_CONDITION.Value == CareConstant.URINE_CONDITION__BAT_THUONG)
                        {
                            checkTieuTien_BatThuong.Checked = true;
                        }
                        else if (careTemp.URINE_CONDITION.Value == CareConstant.URINE_CONDITION__BINH_THUONG)
                        {
                            checkTieuTien_BinhThuong.Checked = true;
                        }
                        else
                        {
                            checkTieuTien_BatThuong.Checked = false;
                            checkTieuTien_BinhThuong.Checked = false;
                            txtTieuTien_Khac.Text = careTemp.URINE ?? "";
                        }
                    }
                    else
                    {
                        checkTieuTien_BatThuong.Checked = false;
                        checkTieuTien_BinhThuong.Checked = false;
                        txtTieuTien_Khac.Text = careTemp.URINE ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDefaultNutrition(HIS_CARE defaultCare)
        {
            try
            {
                if (defaultCare == null && dtExecuteTime.EditValue != null && dtExecuteTime.DateTime != DateTime.MaxValue && dtExecuteTime.DateTime != DateTime.MinValue)
                {
                    HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                    reqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN;
                    reqFilter.TREATMENT_ID = this.currentTreatmentId;
                    reqFilter.INTRUCTION_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(dtExecuteTime.DateTime.ToString("yyyyMMdd") + "000000");
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
                                var a = this.lstRationTime != null ? this.lstRationTime.FirstOrDefault(o => o.ID == group.First().RATION_TIME_ID) : null;
                                if (ss != null && ss.Count > 0)
                                {
                                    LogSystem.Info(LogUtil.TraceData("ss", ss));
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
                                this.txtDinhDuong.Text = (sang ?? "") + ";" + (trua ?? "") + ";" + (chieu ?? "");
                                if (khac != null && khac.Count > 0)
                                {
                                    this.txtDinhDuong.Text += String.Join(";", khac);
                                }
                            }
                            else
                            {
                                this.txtDinhDuong.Text = String.Join(";", nutrition);
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

        private void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                SAR_PRINT_TYPE ketQuaCS = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == "Mps000069");
                if (ketQuaCS != null && ketQuaCS.IS_ACTIVE == CareConstant.IS_TRUE)
                {
                    DXMenuItem itemPhieuChamSoc = new DXMenuItem("Phiếu chăm sóc", new EventHandler(OnClickInPhieuChamSoc));
                    itemPhieuChamSoc.Tag = HIS.Desktop.Plugins.CareCreate.CareCreate.PrintTypeCare.IN_KET_QUA_CHAM_SOC;
                    menu.Items.Add(itemPhieuChamSoc);
                }

                SAR_PRINT_TYPE csCapI = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == "Mps000427");
                if (csCapI != null && csCapI.IS_ACTIVE == CareConstant.IS_TRUE)
                {
                    DXMenuItem itemPhieuChamSocCap1 = new DXMenuItem("Phiếu chăm sóc cấp 1", new EventHandler(OnClickInPhieuChamSoc));
                    itemPhieuChamSocCap1.Tag = HIS.Desktop.Plugins.CareCreate.CareCreate.PrintTypeCare.IN_PHIEU_CHAM_SOC_CAP_I;
                    menu.Items.Add(itemPhieuChamSocCap1);
                }

                DXMenuItem itemInChamSocQy7 = new DXMenuItem("Phiếu chăm sóc _ Y lệnh", new EventHandler(OnClickInPhieuChamSoc));
                itemInChamSocQy7.Tag = HIS.Desktop.Plugins.CareCreate.CareCreate.PrintTypeCare.IN_PHIEU_CHAM_SOC_QY7;
                menu.Items.Add(itemInChamSocQy7);

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuChamSoc(object sender, EventArgs e)
        {
            try
            {
                var bbtnItem = sender as DXMenuItem;
                HIS.Desktop.Plugins.CareCreate.CareCreate.PrintTypeCare type = (HIS.Desktop.Plugins.CareCreate.CareCreate.PrintTypeCare)(bbtnItem.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void PrintProcess(HIS.Desktop.Plugins.CareCreate.CareCreate.PrintTypeCare printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case HIS.Desktop.Plugins.CareCreate.CareCreate.PrintTypeCare.IN_KET_QUA_CHAM_SOC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KET_QUA_CHAM_SOC__MPS000069, DelegateRunPrinterCare);
                        break;
                    case HIS.Desktop.Plugins.CareCreate.CareCreate.PrintTypeCare.IN_PHIEU_CHAM_SOC_QY7:
                        richEditorMain.RunPrintTemplate("Mps000229", DelegateRunPrinterCare);
                        break;
                    case HIS.Desktop.Plugins.CareCreate.CareCreate.PrintTypeCare.IN_PHIEU_CHAM_SOC_CAP_I:
                        richEditorMain.RunPrintTemplate("Mps000427", DelegateRunPrinterCare);
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

        bool DelegateRunPrinterCare(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KET_QUA_CHAM_SOC__MPS000069:
                        LoadBieuMauPhieuYCKetQuaChamSoc(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000229":
                        LoadBieuMauPhieuQy7(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000427":
                        LoadBieuMauPhieuChamSocCapI(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauPhieuChamSocCapI(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                var chartControl1 = ChartDhstProcess.GenerateChartImage(LoadListChartADO());
                var chartControl2 = ChartDhstProcess.GenerateChartImageAll(LoadListChartADO());

                HIS_TREATMENT Treatment = new HIS_TREATMENT();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = currentTreatmentId;
                Treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), printTypeCode, this.Module.RoomId);

                MPS.Processor.Mps000427.PDO.Mps000427PDO mps000427RDO = new MPS.Processor.Mps000427.PDO.Mps000427PDO(
                   this.currentCare,
                   Treatment,
                   this.lstCurrentDhst,
                   ChartDhstProcess.GetChartImage(chartControl1, 0),
                   ChartDhstProcess.GetChartImage(chartControl1, 1),
                   ChartDhstProcess.GetChartImageAll(chartControl2)
                               );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000427RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000427RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<ChartADO> LoadListChartADO()
        {
            List<ChartADO> lstChartAdo = new List<ChartADO>();

            List<HIS_DHST> lst = lstCurrentDhst.OrderByDescending(o => o.EXECUTE_TIME).ToList();
            foreach (var dhst in lst)
            {
                ChartADO chartAdo = new ChartADO();
                chartAdo.Date = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dhst.EXECUTE_TIME.ToString());
                chartAdo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dhst.EXECUTE_TIME ?? 0);
                chartAdo.PULSE = dhst.PULSE;
                chartAdo.TEMPERATURE = dhst.TEMPERATURE;
                lstChartAdo.Add(chartAdo);
            }
            return lstChartAdo;
        }

        private void LoadBieuMauPhieuYCKetQuaChamSoc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                this._CreatorADOs = new List<MPS.Processor.Mps000069.PDO.CreatorADO>();
                var currentPatient = PrintGlobalStore.getPatient(currentTreatmentId);
                MPS.Processor.Mps000069.PDO.PatientADO patientADO69 = new MPS.Processor.Mps000069.PDO.PatientADO();
                if (currentPatient != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000069.PDO.PatientADO>(patientADO69, currentPatient);
                }

                var departmentTran = PrintGlobalStore.getDepartmentTran(currentTreatmentId);

                HIS_TREATMENT Treatment = new HIS_TREATMENT();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = currentTreatmentId;
                Treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                LoadDataToRadioGroupAwareness(ref lstHisAwareness);
                LoadDataToGridControlCareDetail(ref lstHisCareType);

                AddCareData(currentTreatmentId);
                //
                MPS.Processor.Mps000069.PDO.Mps000069ADO mps000069ADO = new MPS.Processor.Mps000069.PDO.Mps000069ADO();
                if (Treatment != null)
                {
                    if (!string.IsNullOrEmpty(Treatment.ICD_CODE))
                    {
                        mps000069ADO.ICD_CODE = Treatment.ICD_CODE;
                        var icd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ICD_CODE == Treatment.ICD_CODE).FirstOrDefault();
                        if (icd != null && icd.ICD_NAME != Treatment.ICD_NAME)
                        {
                            mps000069ADO.ICD_MAIN_TEXT = Treatment.ICD_NAME;
                        }
                        else
                        {
                            mps000069ADO.ICD_NAME = icd.ICD_NAME;
                        }

                    }

                    mps000069ADO.ICD_TEXT = Treatment.ICD_TEXT;
                    mps000069ADO.ICD_SUB_CODE = Treatment.ICD_SUB_CODE;
                }

                mps000069ADO.DEPARTMENT_NAME = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.Module.RoomId).DepartmentName;
                mps000069ADO.ROOM_NAME = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.Module.RoomId).RoomName;
                MOS.Filter.HisTreatmentBedRoomViewFilter bedRoomFilter = new HisTreatmentBedRoomViewFilter();
                bedRoomFilter.TREATMENT_ID = this.currentTreatmentId;
                bedRoomFilter.ORDER_FIELD = "CREATE_TIME";
                bedRoomFilter.ORDER_DIRECTION = "DESC";
                var rsBedName = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, bedRoomFilter, param).FirstOrDefault();
                if (rsBedName != null)
                {
                    mps000069ADO.BED_NAME = rsBedName.BED_NAME;
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), printTypeCode, this.Module.RoomId);
                MPS.Processor.Mps000069.PDO.Mps000069PDO mps000069RDO = new MPS.Processor.Mps000069.PDO.Mps000069PDO(
                    patientADO69,
                    mps000069ADO,
                    Treatment,
                    careByTreatmentHasIcd,
                    lstCareViewPrintADO,
                    lstCareDetailViewPrintADO,
                     this._CreatorADOs,
                     this._careDescription,
                     this._instructionDescription
                                );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000069RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000069RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void AddCareData(long currentTreatmentId)
        {
            try
            {
                if (currentTreatmentId > 0)
                {
                    WaitingManager.Show();
                    this._careDescription = new List<MPS.Processor.Mps000069.PDO.CareDescription>();
                    this.lstCareViewPrintADO = new List<MPS.Processor.Mps000069.PDO.CareViewPrintADO>();
                    this.lstCareDetailViewPrintADO = new List<MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO>();

                    #region ------
                    for (int i = 0; i < 19; i++)
                    {
                        MPS.Processor.Mps000069.PDO.CareViewPrintADO careViewPrintSDO = new MPS.Processor.Mps000069.PDO.CareViewPrintADO();
                        switch (i)
                        {
                            case 0:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.NgayThang;// "Ngày tháng";
                                break;
                            case 1:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.Gio;// "Giờ";
                                break;
                            case 2:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.YThuc;// "Ý thức";
                                break;
                            case 3:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DaNiemMac;//"Da, niêm mạc";
                                break;
                            case 4:
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.Mach;// "Mạch (lần/phút)";
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DauHieuSinhTon;// "Dấu hiệu sinh tồn";
                                break;
                            case 5:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DauHieuSinhTon;// "Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.NhietDo;// "Nhiệt độ (độ C)";
                                break;
                            case 6:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DauHieuSinhTon;//"Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.HuyetAp;//"Huyết áp (mmHg)";
                                break;
                            case 7:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DauHieuSinhTon;// "Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.NhipTho;//"Nhịp thở (lần/phút)";
                                break;
                            case 8:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.DauHieuSinhTon;// "Dấu hiệu sinh tồn";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.Khac;//"khác";
                                break;
                            case 9:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.NuocTieu;//"Nước tiểu (ml)";
                                break;
                            case 10:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.Phan;// "Phân (g)";
                                break;
                            case 11:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.CanNang;//"Cân nặng (kg)";
                                break;
                            case 12:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.ThucHienYLenh;//"Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.ThuocThuongQuy;//"Thuốc thường quy";
                                break;
                            case 13:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.ThucHienYLenh;//"Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.ThuocBoSung;//"Thuốc bổ sung";
                                break;
                            case 14:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.ThucHienYLenh;//"Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.XetNghiem;//"Xét nghiệm";
                                break;
                            case 15:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.ThucHienYLenh;//"Thực hiện y lệnh";
                                careViewPrintSDO.CARE_TITLE2 = ResourceMessage.CheDoAn;//"Chế độ ăn";
                                break;
                            case 16:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.VeSinh;//"Vệ sinh/thay quần áo-ga";
                                break;
                            case 17:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.HuongDanNoiQuy;//"HD nội quy";
                                break;
                            case 18:
                                careViewPrintSDO.CARE_TITLE1 = ResourceMessage.GiaoDucSucKhoe;//"Giáo dục sức khỏe";
                                break;
                            default:
                                break;
                        }

                        careViewPrintSDO.CARE_1 = "";
                        careViewPrintSDO.CARE_2 = "";
                        careViewPrintSDO.CARE_3 = "";
                        careViewPrintSDO.CARE_4 = "";
                        careViewPrintSDO.CARE_5 = "";
                        careViewPrintSDO.CARE_6 = "";
                        careViewPrintSDO.CARE_7 = "";
                        careViewPrintSDO.CARE_8 = "";
                        careViewPrintSDO.CARE_9 = "";
                        careViewPrintSDO.CARE_10 = "";
                        careViewPrintSDO.CARE_11 = "";
                        careViewPrintSDO.CARE_12 = "";
                        this.lstCareViewPrintADO.Add(careViewPrintSDO);
                    }

                    CommonParam paramGet = new CommonParam();
                    List<MOS.EFMODEL.DataModels.HIS_CARE> lstHisCareByTreatment = new List<HIS_CARE>();
                    if (this.currentCare != null)
                    {
                        lstHisCareByTreatment.Add(this.currentCare);
                    }
                    if (lstHisCareByTreatment != null && lstHisCareByTreatment.Count > 0)
                    {
                        lstHisCareByTreatment = lstHisCareByTreatment.Skip(0).Take(6).ToList();
                        careByTreatmentHasIcd = new List<HIS_CARE>();
                        careByTreatmentHasIcd = lstHisCareByTreatment;

                        foreach (var item in lstHisCareByTreatment)
                        {
                            MOS.Filter.HisDhstFilter hisDHSTFilter = new HisDhstFilter();
                            hisDHSTFilter.CARE_ID = item.ID;

                            List<MOS.EFMODEL.DataModels.HIS_DHST> hisDHST = new BackendAdapter(paramGet).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, hisDHSTFilter, paramGet);
                            if (hisDHST != null && hisDHST.Count > 0)
                            {
                                item.HIS_DHST = hisDHST.FirstOrDefault();
                            }
                        }
                    }
                    for (int i = 0; i < lstCareViewPrintADO.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], Inventec.Common.DateTime.Convert.TimeNumberToDateString(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0));
                                }
                                break;
                            case 1:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0) == null ? "" : Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(lstHisCareByTreatment[j].EXECUTE_TIME ?? 0).Value.ToString("HH:mm"));
                                }
                                break;
                            case 2:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    var awarenest = lstHisAwareness.FirstOrDefault(o => o.ID == lstHisCareByTreatment[j].AWARENESS_ID);
                                    //if (awarenest != null)
                                    //{
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].AWARENESS);
                                    //}
                                }
                                break;
                            case 3:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].MUCOCUTANEOUS);//da\
                                }
                                break;
                            case 4:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null && lstHisCareByTreatment[j].HIS_DHST.PULSE.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], ((long)lstHisCareByTreatment[j].HIS_DHST.PULSE).ToString());//mach 
                                    }
                                }
                                break;
                            case 5:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null && lstHisCareByTreatment[j].HIS_DHST.TEMPERATURE.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HIS_DHST.TEMPERATURE.ToString());//nhiet do
                                    }
                                }
                                break;
                            case 6:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        string strBloodPressure = "";
                                        strBloodPressure += (lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MAX.HasValue ? ((long)lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MAX).ToString() : "");
                                        strBloodPressure += (lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MIN.HasValue ? "/" + ((long)lstHisCareByTreatment[j].HIS_DHST.BLOOD_PRESSURE_MIN).ToString() : "");
                                        pi.SetValue(lstCareViewPrintADO[i], strBloodPressure);//huyet ap
                                    }
                                }
                                break;
                            case 7:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null&& lstHisCareByTreatment[j].HIS_DHST.BREATH_RATE.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], ((long)lstHisCareByTreatment[j].HIS_DHST.BREATH_RATE).ToString());//nhip tho
                                    }
                                }
                                break;
                            case 8:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        pi.SetValue(lstCareViewPrintADO[i], (lstHisCareByTreatment[j].HIS_DHST.NOTE));//khác
                                    }
                                }
                                break;
                            case 9:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].URINE);//Nước tiểu (ml)
                                }
                                break;
                            case 10:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].DEJECTA);//Phân (g) (ml)
                                }
                                break;
                            case 11:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)//Cân nặng
                                {
                                    if (lstHisCareByTreatment[j].HIS_DHST != null && lstHisCareByTreatment[j].HIS_DHST.WEIGHT.HasValue)
                                    {
                                        System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                        var weight = ((long)lstHisCareByTreatment[j].HIS_DHST.WEIGHT).ToString();
                                        pi.SetValue(lstCareViewPrintADO[i], weight.Trim() == "0" ? "" : weight);
                                    }
                                }
                                break;
                            case 12:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_MEDICINE == 1 ? "X" : "");//Thuốc thường quy
                                }
                                break;
                            case 13:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_ADD_MEDICINE == 1 ? "X" : "");//Thuốc bổ sung
                                }
                                break;
                            case 14:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].HAS_TEST == 1 ? "X" : "");//Xét nghiệm
                                }
                                break;
                            case 15:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].NUTRITION);//Chế độ ăn
                                }
                                break;
                            case 16:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].SANITARY);//Vệ sinh/thay quần áo-ga
                                }
                                break;
                            case 17:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].TUTORIAL);//HD nội quy
                                }
                                break;
                            case 18:
                                for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareViewPrintADO).GetProperty("CARE_" + (j + 1));
                                    pi.SetValue(lstCareViewPrintADO[i], lstHisCareByTreatment[j].EDUCATION);//Giáo dục sức khỏe
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion

                    //Diễn biến

                    MPS.Processor.Mps000069.PDO.CareDescription _careDescriptionPDO = new MPS.Processor.Mps000069.PDO.CareDescription();
                    _careDescriptionPDO.CARE_DESCRIPTION = ResourceMessage.DienBien;
                    this._careDescription.Add(_careDescriptionPDO);

                    for (int h = 0; h < _careDescription.Count; h++)
                    {
                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                        {
                            System.Reflection.PropertyInfo piDescription = typeof(MPS.Processor.Mps000069.PDO.CareDescription).GetProperty("CARE_DESCRIPTION_" + (j + 1));
                            string dd = lstHisCareByTreatment[j].CARE_DESCRIPTION;
                            piDescription.SetValue(_careDescription[h], lstHisCareByTreatment[j].CARE_DESCRIPTION);

                        }
                    }

                    MPS.Processor.Mps000069.PDO.InstructionDescription _instruction = new MPS.Processor.Mps000069.PDO.InstructionDescription();
                    _instruction.INSTRUCTION_DESCRIPTION = "Y lệnh";
                    this._instructionDescription.Add(_instruction);

                    for (int h = 0; h < _instructionDescription.Count; h++)
                    {
                        for (int j = 0; j < lstHisCareByTreatment.Count; j++)
                        {
                            System.Reflection.PropertyInfo piDescription = typeof(MPS.Processor.Mps000069.PDO.InstructionDescription).GetProperty("INSTRUCTION_DESCRIPTION_" + (j + 1));
                            string dd = lstHisCareByTreatment[j].INSTRUCTION_DESCRIPTION;
                            piDescription.SetValue(_instructionDescription[h], lstHisCareByTreatment[j].INSTRUCTION_DESCRIPTION);
                        }
                    }


                    for (int k = 0; k < lstHisCareByTreatment.Count; k++)
                    {
                        //In 1 cai, check lại với in nhiều//xuandv
                        MPS.Processor.Mps000069.PDO.CreatorADO _creator = new MPS.Processor.Mps000069.PDO.CreatorADO();
                        _creator.CARE_ID = lstHisCareByTreatment[k].ID;

                        System.Reflection.PropertyInfo piCreator = typeof(MPS.Processor.Mps000069.PDO.CreatorADO).GetProperty("CREATOR_" + (k + 1));
                        piCreator.SetValue(_creator, lstHisCareByTreatment[k].CREATOR);

                        var userName = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(p => p.LOGINNAME == lstHisCareByTreatment[k].CREATOR).USERNAME;
                        System.Reflection.PropertyInfo piUser = typeof(MPS.Processor.Mps000069.PDO.CreatorADO).GetProperty("USER_NAME_" + (k + 1));
                        piUser.SetValue(_creator, userName);

                        this._CreatorADOs.Add(_creator);
                        //Add Theo dõi - Chăm sóc
                        MOS.Filter.HisCareDetailViewFilter hisCareDetailFilter = new MOS.Filter.HisCareDetailViewFilter();
                        hisCareDetailFilter.CARE_ID = lstHisCareByTreatment[k].ID;

                        List<MOS.EFMODEL.DataModels.V_HIS_CARE_DETAIL> lstHisCareDetail = new BackendAdapter(paramGet).Get<List<V_HIS_CARE_DETAIL>>(HisRequestUriStore.HIS_CARE_DETAIL_GETVIEW, ApiConsumers.MosConsumer, hisCareDetailFilter, paramGet);


                        if (lstHisCareDetail != null && lstHisCareDetail.Count > 0)
                        {
                            // var careTypeIds = lstHisCareDetail.Select(o => o.CARE_TYPE_ID).Distinct().ToArray();
                            foreach (var caty in lstHisCareDetail)
                            {
                                if (!this.lstCareDetailViewPrintADO.Any(o => o.CARE_TYPE_ID == caty.CARE_TYPE_ID))
                                {
                                    MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO careDetailViewPrintSDO = new MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO();
                                    careDetailViewPrintSDO.CARE_TYPE_ID = caty.CARE_TYPE_ID;
                                    careDetailViewPrintSDO.CARE_TITLE = ResourceMessage.TheoDoiChamSoc;// "Theo dõi - Chăm sóc";
                                    careDetailViewPrintSDO.CARE_DETAIL = caty.CARE_TYPE_NAME;
                                    careDetailViewPrintSDO.CARE_DETAIL_1 = "";
                                    careDetailViewPrintSDO.CARE_DETAIL_2 = "";
                                    careDetailViewPrintSDO.CARE_DETAIL_3 = "";
                                    careDetailViewPrintSDO.CARE_DETAIL_4 = "";
                                    careDetailViewPrintSDO.CARE_DETAIL_5 = "";
                                    careDetailViewPrintSDO.CARE_DETAIL_6 = "";
                                    this.lstCareDetailViewPrintADO.Add(careDetailViewPrintSDO);
                                }
                            }


                            foreach (var item in this.lstCareDetailViewPrintADO)
                            {
                                var careDetailForOnes = lstHisCareDetail.Where(o => o.CARE_TYPE_ID == item.CARE_TYPE_ID).ToList();
                                if (careDetailForOnes != null && careDetailForOnes.Count > 0)
                                {
                                    System.Reflection.PropertyInfo pi = typeof(MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO).GetProperty("CARE_DETAIL_" + (k + 1));
                                    pi.SetValue(item, careDetailForOnes[0].CONTENT);
                                }
                            }
                        }

                    }

                    int countCaTyPrint = 6 - this.lstCareDetailViewPrintADO.Count;
                    if (countCaTyPrint > 0)
                    {
                        for (int i = 0; i < countCaTyPrint; i++)
                        {
                            MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO careDetailViewPrintSDO = new MPS.Processor.Mps000069.PDO.CareDetailViewPrintADO();
                            careDetailViewPrintSDO.CARE_TITLE = ResourceMessage.TheoDoiChamSoc;// "Theo dõi - Chăm sóc";
                            careDetailViewPrintSDO.CARE_DETAIL_1 = "";
                            careDetailViewPrintSDO.CARE_DETAIL_2 = "";
                            careDetailViewPrintSDO.CARE_DETAIL_3 = "";
                            careDetailViewPrintSDO.CARE_DETAIL_4 = "";
                            careDetailViewPrintSDO.CARE_DETAIL_5 = "";
                            careDetailViewPrintSDO.CARE_DETAIL_6 = "";
                            this.lstCareDetailViewPrintADO.Add(careDetailViewPrintSDO);
                        }
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

        internal void LoadDataToRadioGroupAwareness(ref List<MOS.EFMODEL.DataModels.HIS_AWARENESS> lstHisAwareness)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisAwarenessFilter hisAwarenessFilter = new MOS.Filter.HisAwarenessFilter();
                lstHisAwareness = new BackendAdapter(param).Get<List<HIS_AWARENESS>>(HisRequestUriStore.HIS_AWARENESS_GET, ApiConsumers.MosConsumer, hisAwarenessFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadDataToGridControlCareDetail(ref List<MOS.EFMODEL.DataModels.HIS_CARE_TYPE> lstHisCareType)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisCareTypeFilter hisCareTypeFilter = new MOS.Filter.HisCareTypeFilter();
                hisCareTypeFilter.ORDER_DIRECTION = "DESC";
                hisCareTypeFilter.ORDER_FIELD = "MODIFY_TIME";
                lstHisCareType = new BackendAdapter(param).Get<List<HIS_CARE_TYPE>>(HisRequestUriStore.HIS_CARE_TYPE_GET, ApiConsumers.MosConsumer, hisCareTypeFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBieuMauPhieuQy7(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                var _Treatment = PrintGlobalStore.getTreatment(this.currentTreatmentId);
                MOS.Filter.HisTreatmentBedRoomViewFilter filter = new HisTreatmentBedRoomViewFilter();
                filter.TREATMENT_ID = this.currentTreatmentId;
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                V_HIS_TREATMENT_BED_ROOM _TreatmetnbedRoom = new V_HIS_TREATMENT_BED_ROOM();
                var TreatmetnbedRooms = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filter, null);
                if (TreatmetnbedRooms != null && TreatmetnbedRooms.Count > 0)
                {
                    _TreatmetnbedRoom = TreatmetnbedRooms.FirstOrDefault();
                }
                List<HIS_CARE> _CareChecks = new List<HIS_CARE>();
                _CareChecks.Add(this.currentCare);
                if (_CareChecks != null && _CareChecks.Count > 0)
                {
                    _CareChecks = _CareChecks.OrderBy(p => p.EXECUTE_TIME).ToList();
                }
                var mps000229ADO = new MPS.Processor.Mps000229.PDO.Mps000229ADO();

                mps000229ADO.DEPARTMENT_NAME = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == this.Module.RoomId).DepartmentName;
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode, this.Module.RoomId);

                MPS.Processor.Mps000229.PDO.Mps000229PDO mps000229RDO = new MPS.Processor.Mps000229.PDO.Mps000229PDO(
                  _Treatment,
                  _CareChecks,
                  _TreatmetnbedRoom,
                  mps000229ADO
                    );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000229RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000229RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void UpdateThoiGianDo()
        {
            try
            {
                if (dtExecuteTime.EditValue != null && dtExecuteTime.DateTime != DateTime.MinValue && this.lstCurrentDhst != null && this.lstCurrentDhst.Count > 0)
                {
                    string date = dtExecuteTime.DateTime.ToString("yyyyMMdd");
                    lstCurrentDhst.ForEach(o =>
                        {
                            if (o.EXECUTE_TIME.HasValue)
                            {
                                o.EXECUTE_TIME = Convert.ToInt64(date + o.EXECUTE_TIME.Value.ToString().Substring(8, 6));
                            }
                        });

                    gridControlDhst.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Load người chỉ định
        private async Task InitComboUser()
        {
            try
            {
                List<ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                {
                    datas = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new BackendAdapter(paramCommon).GetAsync<List<ACS_USER>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

                //Nguoi chi dinh
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboNguoiThucHien_Username, datas, controlEditorADO);

                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var data = datas.Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).FirstOrDefault();

                if (this.currentCare != null)
                {
                    cboNguoiThucHien_Username.EditValue = this.currentCare.EXECUTE_LOGINNAME;
                    txtNguoiThucHien_Loginname.Text = this.currentCare.EXECUTE_LOGINNAME ?? "";
                }
                else if (data != null)
                {
                    cboNguoiThucHien_Username.EditValue = data.LOGINNAME;
                    txtNguoiThucHien_Loginname.Text = data.LOGINNAME ?? "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCareTemp_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtExecuteTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCareTemp_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                HIS_CARE_TEMP temp = null;
                if (cboCareTemp.EditValue != null)
                {
                    temp = this.lstCareTemp != null ? this.lstCareTemp.FirstOrDefault(o => o.ID == Convert.ToInt64(cboCareTemp.EditValue)) : null;
                }

                if (temp != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("temp: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => temp), temp));
                    WaitingManager.Show();
                    txtCareTempCode.Text = temp.CARE_TEMP_CODE ?? "";
                    LoadCareDetail(temp);
                    FillDefaultCare(null, temp);

                    WaitingManager.Hide();
                }
                else
                {
                    txtCareTempCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExecuteTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExecuteTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkTienSuDiUng_Co.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExecuteTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit) return;
                WaitingManager.Show();
                this.FillDefaultNutrition(null);
                this.UpdateThoiGianDo();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTienSuDiUng_Co_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTienSuDiUng_Co.Checked)
                {
                    checkTienSuDiUng_Khong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTienSuDiUng_Khong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTienSuDiUng_Khong.Checked)
                {
                    checkTienSuDiUng_Co.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkYThuc_Tinh_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkYThuc_Tinh.Checked)
                {
                    checkYThuc_HonMe.Checked = false;
                    checkYThuc_LoMo.Checked = false;
                    txtYThuc_Khac.Text = "";
                    txtYThuc_Khac.Enabled = false;
                }
                else
                {
                    txtYThuc_Khac.Enabled = (!checkYThuc_HonMe.Checked && !checkYThuc_LoMo.Checked && !checkYThuc_Tinh.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkYThuc_LoMo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkYThuc_LoMo.Checked)
                {
                    checkYThuc_HonMe.Checked = false;
                    checkYThuc_Tinh.Checked = false;
                    txtYThuc_Khac.Text = "";
                    txtYThuc_Khac.Enabled = false;
                }
                else
                {
                    txtYThuc_Khac.Enabled = (!checkYThuc_HonMe.Checked && !checkYThuc_LoMo.Checked && !checkYThuc_Tinh.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkYThuc_HonMe_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkYThuc_HonMe.Checked)
                {
                    checkYThuc_LoMo.Checked = false;
                    checkYThuc_Tinh.Checked = false;
                    txtYThuc_Khac.Text = "";
                    txtYThuc_Khac.Enabled = false;
                }
                else
                {
                    txtYThuc_Khac.Enabled = (!checkYThuc_HonMe.Checked && !checkYThuc_LoMo.Checked && !checkYThuc_Tinh.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkToanTrang_Met_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkToanTrang_Met.Checked)
                {
                    checkToanTrang_BinhThuong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkToanTrang_BinhThuong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkToanTrang_BinhThuong.Checked)
                {
                    checkToanTrang_Met.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDaNiemMac_Hong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkDaNiemMac_Hong.Checked)
                {
                    checkDaNiemMac_BinhThuong.Checked = false;
                    checkDaNiemMac_Nhot.Checked = false;
                    txtDaNiemMac_Khac.Text = "";
                    txtDaNiemMac_Khac.Enabled = false;
                }
                else
                {
                    txtDaNiemMac_Khac.Enabled = (!checkDaNiemMac_Hong.Checked && !checkDaNiemMac_BinhThuong.Checked && !checkDaNiemMac_Nhot.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDaNiemMac_Nhot_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkDaNiemMac_Nhot.Checked)
                {
                    checkDaNiemMac_BinhThuong.Checked = false;
                    checkDaNiemMac_Hong.Checked = false;
                    txtDaNiemMac_Khac.Text = "";
                    txtDaNiemMac_Khac.Enabled = false;
                }
                else
                {
                    txtDaNiemMac_Khac.Enabled = (!checkDaNiemMac_Hong.Checked && !checkDaNiemMac_BinhThuong.Checked && !checkDaNiemMac_Nhot.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDaNiemMac_BinhThuong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkDaNiemMac_BinhThuong.Checked)
                {
                    checkDaNiemMac_Hong.Checked = false;
                    checkDaNiemMac_Nhot.Checked = false;
                    txtDaNiemMac_Khac.Text = "";
                    txtDaNiemMac_Khac.Enabled = false;
                }
                else
                {
                    txtDaNiemMac_Khac.Enabled = (!checkDaNiemMac_Hong.Checked && !checkDaNiemMac_BinhThuong.Checked && !checkDaNiemMac_Nhot.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkCoNang_Ngua_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkCoNang_Ngua.Checked)
                {
                    checkCoNang_Dau.Checked = false;
                    checkCoNang_Rat.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkCoNang_Dau_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkCoNang_Dau.Checked)
                {
                    checkCoNang_Ngua.Checked = false;
                    checkCoNang_Rat.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkCoNang_Rat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkCoNang_Rat.Checked)
                {
                    checkCoNang_Ngua.Checked = false;
                    checkCoNang_Dau.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTonThuongDa_Tang_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTonThuongDa_Tang.Checked)
                {
                    checkTonThuongDa_Giam.Checked = false;
                    checkTonThuongDa_Uot.Checked = false;
                    checkTonThuongDa_OnDinh.Checked = false;
                    checkTonThuongDa_Kho.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTonThuongDa_Giam_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTonThuongDa_Giam.Checked)
                {
                    checkTonThuongDa_Kho.Checked = false;
                    checkTonThuongDa_OnDinh.Checked = false;
                    checkTonThuongDa_Tang.Checked = false;
                    checkTonThuongDa_Uot.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTonThuongDa_OnDinh_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTonThuongDa_OnDinh.Checked)
                {
                    checkTonThuongDa_Kho.Checked = false;
                    checkTonThuongDa_Giam.Checked = false;
                    checkTonThuongDa_Tang.Checked = false;
                    checkTonThuongDa_Uot.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTonThuongDa_Kho_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTonThuongDa_Kho.Checked)
                {
                    checkTonThuongDa_Giam.Checked = false;
                    checkTonThuongDa_OnDinh.Checked = false;
                    checkTonThuongDa_Tang.Checked = false;
                    checkTonThuongDa_Uot.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTonThuongDa_Uot_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTonThuongDa_Uot.Checked)
                {
                    checkTonThuongDa_Tang.Checked = false;
                    checkTonThuongDa_OnDinh.Checked = false;
                    checkTonThuongDa_Kho.Checked = false;
                    checkTonThuongDa_Giam.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkNguyCoLoet_Co_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkNguyCoLoet_Co.Checked)
                {
                    checkNguyCoLoet_Khong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkNguyCoLoet_Khong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkNguyCoLoet_Khong.Checked)
                {
                    checkNguyCoLoet_Co.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkHoHap_TuTho_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkHoHap_TuTho.Checked)
                {
                    checkHoHap_ThoOxy.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkHoHap_ThoOxy_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkHoHap_ThoOxy.Checked)
                {
                    checkHoHap_TuTho.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkThanKinh_CoGiat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkThanKinh_CoGiat.Checked)
                {
                    checkThanKinh_TangTLC.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkThanKinh_TangTLC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkThanKinh_TangTLC.Checked)
                {
                    checkThanKinh_CoGiat.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDaiTien_BinhThuong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkDaiTien_BinhThuong.Checked)
                {
                    checkDaiTien_BatThuong.Checked = false;
                    txtDaiTien_Khac.Text = "";
                    txtDaiTien_Khac.Enabled = false;
                }
                else
                {
                    txtDaiTien_Khac.Enabled = (!checkDaiTien_BatThuong.Checked && !checkDaiTien_BinhThuong.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDaiTien_BatThuong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkDaiTien_BatThuong.Checked)
                {
                    checkDaiTien_BinhThuong.Checked = false;
                    txtDaiTien_Khac.Text = "";
                    txtDaiTien_Khac.Enabled = false;
                }
                else
                {
                    txtDaiTien_Khac.Enabled = (!checkDaiTien_BatThuong.Checked && !checkDaiTien_BinhThuong.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTieuTien_BinhThuong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTieuTien_BinhThuong.Checked)
                {
                    checkTieuTien_BatThuong.Checked = false;
                    txtTieuTien_Khac.Text = "";
                    txtTieuTien_Khac.Enabled = false;
                }
                else
                {
                    txtTieuTien_Khac.Enabled = (!checkTieuTien_BatThuong.Checked && !checkTieuTien_BinhThuong.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTieuTien_BatThuong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTieuTien_BinhThuong.Checked)
                {
                    checkTieuTien_BatThuong.Checked = false;
                    txtTieuTien_Khac.Text = "";
                    txtTieuTien_Khac.Enabled = false;
                }
                else
                {
                    txtTieuTien_Khac.Enabled = (!checkTieuTien_BatThuong.Checked && !checkTieuTien_BinhThuong.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTieuHoa_TuAn_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTieuHoa_TuAn.Checked)
                {
                    checkTieuHoa_QuaSonde.Checked = false;
                    checkTieuHoa_Non.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTieuHoa_QuaSonde_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTieuHoa_QuaSonde.Checked)
                {
                    checkTieuHoa_Non.Checked = false;
                    checkTieuHoa_TuAn.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTieuHoa_Non_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkTieuHoa_Non.Checked)
                {
                    checkTieuHoa_QuaSonde.Checked = false;
                    checkTieuHoa_TuAn.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                HIS_CARE_DETAIL data = null;
                if (e.RowHandle > -1)
                {
                    data = (HIS_CARE_DETAIL)gridViewCareType.GetRow(e.RowHandle);
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Add_Delete")
                    {
                        if (e.RowHandle == 0)
                        {
                            e.RepositoryItem = repositoryItemBtn_CareDetail_Add;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtn_CareDetail_Delete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCareType_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
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

        private void gridViewCareType_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                HIS_CARE_DETAIL data = gridViewCareType.GetFocusedRow() as HIS_CARE_DETAIL;
                if (gridViewCareType.FocusedColumn.FieldName == "CARE_TYPE_ID" && gridViewCareType.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = gridViewCareType.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        if (editor.EditValue == null)//xemlai...
                        {
                            string error = this.GetError(gridViewCareType.FocusedRowHandle, gridViewCareType.FocusedColumn);
                            if (error == string.Empty) return;
                            gridViewCareType.SetColumnError(gridViewCareType.FocusedColumn, error);
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
                    ADO.HisCareDetailADO data = (ADO.HisCareDetailADO)gridViewCareType.GetRow(rowHandle);
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

        private void gridViewCareType_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    GridColumn onOrderCol = gridViewCareType.Columns["CONTENT"];
                    gridViewCareType.ClearColumnErrors();
                    var data = (HIS_CARE_DETAIL)gridViewCareType.GetRow(e.RowHandle);
                    if (data != null && data.CARE_TYPE_ID > 0)
                    {
                        if (String.IsNullOrEmpty(data.CONTENT))
                        {
                            // e.Valid = false;
                            gridViewCareType.SetColumnError(onOrderCol, "Trường dữ liệu bắt buộc");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlCareType_ProcessGridKey(object sender, KeyEventArgs e)
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
                        dtDhstThoiGianDo.Focus();
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

        private void checkPhoBienNoiQuy_Co_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkPhoBienNoiQuy_Co.Checked)
                {
                    checkPhoBienNoiQuy_Khong.Checked = false;
                    txtPhoBienNoiQuy_Khac.Text = "";
                    txtPhoBienNoiQuy_Khac.Enabled = false;
                }
                else
                {
                    txtPhoBienNoiQuy_Khac.Enabled = (!checkPhoBienNoiQuy_Khong.Checked && !checkPhoBienNoiQuy_Co.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkPhoBienNoiQuy_Khong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkPhoBienNoiQuy_Khong.Checked)
                {
                    checkPhoBienNoiQuy_Co.Checked = false;
                    txtPhoBienNoiQuy_Khac.Text = "";
                    txtPhoBienNoiQuy_Khac.Enabled = false;
                }
                else
                {
                    txtPhoBienNoiQuy_Khac.Enabled = (!checkPhoBienNoiQuy_Khong.Checked && !checkPhoBienNoiQuy_Co.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkVeSinh_TamThuong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkVeSinh_TamThuong.Checked)
                {
                    checkVeSinh_TamThuoc.Checked = false;
                    txtVeSinh_Khac.Text = "";
                    txtVeSinh_Khac.Enabled = false;
                }
                else
                {
                    txtVeSinh_Khac.Enabled = (!checkVeSinh_TamThuoc.Checked && !checkVeSinh_TamThuong.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkVeSinh_TamThuoc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkVeSinh_TamThuoc.Checked)
                {
                    checkVeSinh_TamThuong.Checked = false;
                    txtVeSinh_Khac.Text = "";
                    txtVeSinh_Khac.Enabled = false;
                }
                else
                {
                    txtVeSinh_Khac.Enabled = (!checkVeSinh_TamThuong.Checked && !checkVeSinh_TamThuoc.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkGiaoDucSucKhoe_Co_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkGiaoDucSucKhoe_Co.Checked)
                {
                    checkGiaoDucSucKhoe_Khong.Checked = false;
                    txtGiaoDucSucKhoe_Khac.Text = "";
                    txtGiaoDucSucKhoe_Khac.Enabled = false;
                }
                else
                {
                    txtGiaoDucSucKhoe_Khac.Enabled = (!checkGiaoDucSucKhoe_Khong.Checked && !checkGiaoDucSucKhoe_Co.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkGiaoDucSucKhoe_Khong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkGiaoDucSucKhoe_Khong.Checked)
                {
                    checkGiaoDucSucKhoe_Co.Checked = false;
                    txtGiaoDucSucKhoe_Khac.Text = "";
                    txtGiaoDucSucKhoe_Khac.Enabled = false;
                }
                else
                {
                    txtGiaoDucSucKhoe_Khac.Enabled = (!checkGiaoDucSucKhoe_Khong.Checked && !checkGiaoDucSucKhoe_Co.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void txtDanhGia_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNguoiThucHien_Loginname_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboNguoiThucHien_Username.EditValue = null;
                        this.cboNguoiThucHien_Username.Focus();
                        this.cboNguoiThucHien_Username.ShowPopup();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboNguoiThucHien_Username.EditValue = searchResult[0].LOGINNAME;
                            this.txtNguoiThucHien_Loginname.Text = searchResult[0].LOGINNAME;
                            SendKeys.Send("{TAB}");
                        }
                        else
                        {
                            this.cboNguoiThucHien_Username.EditValue = null;
                            this.cboNguoiThucHien_Username.Focus();
                            this.cboNguoiThucHien_Username.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNguoiThucHien_Username_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboNguoiThucHien_Username.EditValue != null)
                    {
                        this.txtNguoiThucHien_Loginname.Text = cboNguoiThucHien_Username.EditValue.ToString();
                    }
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDhstThoiGianDo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtDhstCanNang.Focus();
                    txtDhstCanNang.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDhstMach_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDhstChieuCao.Focus();
                    txtDhstChieuCao.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDhstNhietDo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDhstVongNguc.Focus();
                    txtDhstVongNguc.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDhstHuyetApp_Min_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDhstVongBung.Focus();
                    txtDhstVongBung.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDhstHuyetAp_Max_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDhstHuyetAp_Min.Focus();
                    txtDhstHuyetAp_Min.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDhstNhipTho_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDhstSpo2.Focus();
                    txtDhstSpo2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDhstCanNang_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDhstMach.Focus();
                    txtDhstMach.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDhstChieuCao_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDhstNhietDo.Focus();
                    txtDhstNhietDo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDhstVongNguc_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDhstHuyetAp_Max.Focus();
                    txtDhstHuyetAp_Max.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDhstVongBung_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDhstNhipTho.Focus();
                    txtDhstNhipTho.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDhstSpo2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDhstKhac.Focus();
                    txtDhstKhac.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDhstKhac_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichVao_Truyen_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkDhstDichVao_Truyen.Checked)
                {
                    checkDhstDichVao_Khac.Checked = false;
                    checkDhstDichVao_An.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichVao_An_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkDhstDichVao_An.Checked)
                {
                    checkDhstDichVao_Khac.Checked = false;
                    checkDhstDichVao_Truyen.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichVao_Khac_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkDhstDichVao_Khac.Checked)
                {
                    checkDhstDichVao_Truyen.Checked = false;
                    checkDhstDichVao_An.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichRa_NuocTieu_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkDhstDichRa_NuocTieu.Checked)
                {
                    checkDhstDichRa_DanLuu.Checked = false;
                    checkDhstDichRa_Phan.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichRa_DanLuu_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkDhstDichRa_DanLuu.Checked)
                {
                    checkDhstDichRa_Phan.Checked = false;
                    checkDhstDichRa_NuocTieu.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichRa_Phan_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkDhstDichRa_Phan.Checked)
                {
                    checkDhstDichRa_DanLuu.Checked = false;
                    checkDhstDichRa_NuocTieu.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDhstAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider2.Validate())
                    return;

                if (!btnDhstAdd.Enabled) return;

                if (dtDhstThoiGianDo.DateTime.Date != dtExecuteTime.DateTime.Date)
                {
                    XtraMessageBox.Show(ResourceMessage.NgayDoPhaiBangNgayXuLyPhieu, ResourceMessage.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                HIS_DHST dhst = new HIS_DHST();

                this.SetFromControlDhst(ref dhst);

                #region thông báo
                MessageManager.ShowAlert(this, ResourceMessage.ThongBao, ResourceMessage.XulyThanhCong);
                #endregion

                if (this.lstCurrentDhst == null) this.lstCurrentDhst = new List<HIS_DHST>();
                this.lstCurrentDhst.Add(dhst);
                gridControlDhst.BeginUpdate();
                gridControlDhst.DataSource = this.lstCurrentDhst;
                gridControlDhst.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFromControlDhst(ref HIS_DHST dhst)
        {
            dhst.EXECUTE_ROOM_ID = this.currentModuleBase.RoomId;
            dhst.TREATMENT_ID = this.currentTreatmentId;
            if (txtDhstVongBung.EditValue != null)
            {
                dhst.BELLY = (decimal)txtDhstVongBung.EditValue;
            }
            else
            {
                dhst.BELLY = null;
            }

            if (txtDhstHuyetAp_Max.EditValue != null && txtDhstHuyetAp_Max.EditValue.ToString() != "")
            {
                dhst.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(txtDhstHuyetAp_Max.EditValue.ToString());
            }
            else
            {
                dhst.BLOOD_PRESSURE_MAX = null;
            }
            if (txtDhstHuyetAp_Min.EditValue != null && txtDhstHuyetAp_Min.EditValue.ToString() != "")
            {
                dhst.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(txtDhstHuyetAp_Min.EditValue.ToString());
            }
            else
            {
                dhst.BLOOD_PRESSURE_MIN = null;
            }
            if (txtDhstCanNang.EditValue != null)
            {
                dhst.WEIGHT = (decimal)txtDhstCanNang.EditValue;
            }
            else
            {
                dhst.WEIGHT = null;
            }

            if (txtDhstChieuCao.EditValue != null)
            {
                dhst.HEIGHT = (decimal)txtDhstChieuCao.EditValue;
            }
            else
            {
                dhst.HEIGHT = null;
            }
            if (txtDhstMach.EditValue != null && txtDhstMach.EditValue.ToString() != "")
            {
                dhst.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(txtDhstMach.EditValue.ToString());
            }
            else
            {
                dhst.PULSE = null;
            }
            if (txtDhstVongNguc.EditValue != null && txtDhstVongNguc.EditValue.ToString() != "")
            {
                dhst.CHEST = (decimal)txtDhstVongNguc.EditValue;
            }
            else
            {
                dhst.CHEST = null;
            }
            if (txtDhstSpo2.EditValue != null && txtDhstSpo2.EditValue.ToString() != "")
            {
                dhst.SPO2 = (decimal)txtDhstSpo2.EditValue;
            }
            else
            {
                dhst.SPO2 = null;
            }
            if (txtDhstNhietDo.EditValue != null && txtDhstNhietDo.EditValue.ToString() != "")
            {
                dhst.TEMPERATURE = (decimal)txtDhstNhietDo.EditValue;
            }
            else
            {
                dhst.TEMPERATURE = null;
            }
            if (txtDhstNhipTho.EditValue != null && txtDhstNhipTho.EditValue.ToString() != "")
            {
                dhst.BREATH_RATE = (decimal)txtDhstNhipTho.EditValue;
            }
            else
            {
                dhst.BREATH_RATE = null;
            }
            dhst.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            dhst.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
            if (dtDhstThoiGianDo.EditValue != null && dtDhstThoiGianDo.DateTime != DateTime.MinValue)
            {
                dhst.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDhstThoiGianDo.DateTime);
            }
            else
            {
                dhst.EXECUTE_TIME = null;
            }

            dhst.NOTE = txtDhstKhac.Text;

            //dịch vào
            if (checkDhstDichVao_Truyen.Checked)
            {
                dhst.INFUTION_INTO = 1;
            }
            else if (checkDhstDichVao_An.Checked)
            {
                dhst.INFUTION_INTO = 2;
            }
            else if (checkDhstDichVao_Khac.Checked)
            {
                dhst.INFUTION_INTO = 3;
            }

            // dịch ra
            if (checkDhstDichRa_NuocTieu.Checked)
            {
                dhst.INFUTION_OUT = 1;
            }
            else if (checkDhstDichRa_DanLuu.Checked)
            {
                dhst.INFUTION_OUT = 2;
            }
            else if (checkDhstDichRa_Phan.Checked)
            {
                dhst.INFUTION_OUT = 3;
            }
        }

        private void btnDhstEdit_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider2.Validate())
                    return;

                if (!btnDhstEdit.Enabled || this.editDhst == null) return;
                if (dtDhstThoiGianDo.DateTime.Date != dtExecuteTime.DateTime.Date)
                {
                    XtraMessageBox.Show(ResourceMessage.NgayDoPhaiBangNgayXuLyPhieu, ResourceMessage.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                this.SetFromControlDhst(ref this.editDhst);
                #region thông báo
                MessageManager.ShowAlert(this, ResourceMessage.ThongBao, ResourceMessage.XulyThanhCong);
                #endregion
                gridControlDhst.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDhstNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnDhstNew.Enabled) return;
                this.editDhst = null;
                this.FillDefaultDhst(null);
                btnDhstAdd.Enabled = true;
                btnDhstEdit.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtn_CareDetail_Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_CARE_DETAIL careDetail = new HIS_CARE_DETAIL();
                if (this.lstCurrentCareDetail == null) this.lstCurrentCareDetail = new List<HIS_CARE_DETAIL>();
                this.lstCurrentCareDetail.Add(careDetail);
                gridControlCareType.BeginUpdate();
                gridControlCareType.DataSource = this.lstCurrentCareDetail;
                gridControlCareType.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtn_CareDetail_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_CARE_DETAIL detail = (HIS_CARE_DETAIL)gridViewCareType.GetFocusedRow();
                if (detail != null)
                {
                    this.lstCurrentCareDetail.Remove(detail);
                    gridControlCareType.BeginUpdate();
                    gridControlCareType.DataSource = this.lstCurrentCareDetail;
                    gridControlCareType.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDhst_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_DHST pData = (HIS_DHST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.EXECUTE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "BLOOD_PRESSURE_STR")
                    {
                        if (pData.BLOOD_PRESSURE_MAX == null && pData.BLOOD_PRESSURE_MIN == null)
                        {
                            e.Value = null;
                        }
                        else
                        {
                            e.Value = pData.BLOOD_PRESSURE_MAX + "/" + pData.BLOOD_PRESSURE_MIN;
                        }
                    }
                    else if (e.Column.FieldName == "INFUTION_IN_STR")
                    {
                        string DichVao = "";
                        if (pData.INFUTION_INTO == 1)
                        {
                            DichVao = "Truyền";
                        }
                        else if (pData.INFUTION_INTO == 2)
                        {
                            DichVao = "Ăn";
                        }
                        else if (pData.INFUTION_INTO == 3)
                        {
                            DichVao = "Khác";
                        }
                        e.Value = DichVao;
                    }
                    else if (e.Column.FieldName == "INFUTION_OUT_STR")
                    {
                        string DichRa = "";
                        if (pData.INFUTION_OUT == 1)
                        {
                            DichRa = "Nước tiểu";
                        }
                        else if (pData.INFUTION_OUT == 2)
                        {
                            DichRa = "dẫn lưu";
                        }
                        else if (pData.INFUTION_OUT == 3)
                        {
                            DichRa = "Phân";
                        }
                        e.Value = DichRa;
                    }

                }
                gridControlDhst.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtn_Dhst_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (HIS_DHST)gridViewDhst.GetFocusedRow();
                    if (rowData != null)
                    {
                        this.lstCurrentDhst.Remove(rowData);
                        this.FillDefaultDhst(null);

                        gridControlDhst.BeginUpdate();
                        gridControlDhst.DataSource = this.lstCurrentDhst;
                        gridControlDhst.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDhst_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (HIS_DHST)gridViewDhst.GetFocusedRow();
                if (rowData != null)
                {
                    FillDefaultDhst(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRation_Click(object sender, EventArgs e)
        {
            try
            {
                // popup yêu cầu xem
                List<object> _listObj = new List<object>();
                _listObj.Add(this.currentTreatmentId);
                if (DataTransferTreatmentBedRoomFilter != null)
                    _listObj.Add(DataTransferTreatmentBedRoomFilter);
                WaitingManager.Hide();
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignNutrition", Module.RoomId, Module.RoomTypeId, _listObj);
                ProcessLoadTxtDinhDuong();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessLoadTxtDinhDuong()
        {
            try
            {
                if (dtExecuteTime.EditValue != null && dtExecuteTime.DateTime != DateTime.MaxValue && dtExecuteTime.DateTime != DateTime.MinValue)
                {
                    HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                    reqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN;
                    reqFilter.TREATMENT_ID = this.currentTreatmentId;
                    reqFilter.INTRUCTION_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(dtExecuteTime.DateTime.ToString("yyyyMMdd") + "000000");
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
                                var a = this.lstRationTime.FirstOrDefault(o => o.ID == group.First().RATION_TIME_ID);
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
                                this.txtDinhDuong.Text = (sang ?? "") + ";" + (trua ?? "") + ";" + (chieu ?? "");
                                if (khac != null && khac.Count > 0)
                                {
                                    this.txtDinhDuong.Text += String.Join(";", khac);
                                }
                            }
                            else
                            {
                                this.txtDinhDuong.Text = String.Join(";", nutrition);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                if (gridViewCareType.IsEditing)
                    gridViewCareType.CloseEditor();

                if (gridViewCareType.FocusedRowModified)
                    gridViewCareType.UpdateCurrentRow();

                if (gridViewCareType.HasColumnErrors)
                    return;

                MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                treatmentFilter.ID = this.currentTreatmentId;
                var rsTreatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, null);
                if (rsTreatment != null && rsTreatment.Count > 0)
                {
                    DateTime trackingTime = dtExecuteTime.DateTime;
                    DateTime inTimeTreatment = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rsTreatment[0].IN_TIME) ?? DateTime.Now;
                    if (trackingTime < inTimeTreatment)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.Validate_Date_Time, ResourceMessage.ThongBao);
                        dtExecuteTime.Focus();
                        dtExecuteTime.SelectAll();
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
                if (this.lstCurrentCareDetail == null || this.lstCurrentCareDetail.Count == 0)
                    throw new ArgumentNullException("Du lieu dau vao khong hop le. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstCurrentCareDetail), lstCurrentCareDetail));

                var groupCareType = from p in this.lstCurrentCareDetail
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

        private void SaveCareProcess()
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_CARE hisCare = new MOS.EFMODEL.DataModels.HIS_CARE();

                if (this.action == GlobalVariables.ActionEdit)
                {
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_CARE, MOS.EFMODEL.DataModels.HIS_CARE>();
                    hisCare = Mapper.Map<MOS.EFMODEL.DataModels.HIS_CARE, MOS.EFMODEL.DataModels.HIS_CARE>(this.currentCare);
                }
                ProcessDataCare(ref hisCare);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("hisCare: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => hisCare), hisCare));
                SaveDataCare(hisCare);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveDataCare(MOS.EFMODEL.DataModels.HIS_CARE hisCare)
        {
            if (hisCare == null) throw new ArgumentNullException("hisCare is null");
            MOS.EFMODEL.DataModels.HIS_CARE rsHisCare = null;
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();

                switch (this.action)
                {
                    case GlobalVariables.ActionEdit:
                        rsHisCare = new BackendAdapter(param).Post<HIS_CARE>("api/HisCare/UpdateWithDhst", ApiConsumers.MosConsumer, hisCare, param);
                        break;
                    case GlobalVariables.ActionAdd:
                        rsHisCare = new BackendAdapter(param).Post<HIS_CARE>(HisRequestUriStore.HIS_CARE_CREATE, ApiConsumers.MosConsumer, hisCare, param);
                        break;
                    default:
                        break;
                }

                if (rsHisCare != null && rsHisCare.ID > 0)
                {
                    success = true;
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_CARE, MOS.EFMODEL.DataModels.HIS_CARE>();
                    this.currentCare = Mapper.Map<MOS.EFMODEL.DataModels.HIS_CARE, MOS.EFMODEL.DataModels.HIS_CARE>(rsHisCare);
                    ddBtnPrint.Enabled = true;
                    if (this.delegateSelectData != null)
                        this.delegateSelectData(this.currentCare);
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void ProcessDataCare(ref HIS_CARE hisCare)
        {
            try
            {
                if (hisCare == null)
                {
                    hisCare = new HIS_CARE();
                }

                if (currentCareSum != null)
                {
                    hisCare.CARE_SUM_ID = currentCareSum.ID;
                }

                if (currentTracking != null && currentTracking.ID > 0)
                {
                    hisCare.TRACKING_ID = currentTracking.ID;
                }

                hisCare.EXECUTE_ROOM_ID = this.Module.RoomId;

                hisCare.EXECUTE_LOGINNAME = txtNguoiThucHien_Loginname.Text.Trim();
                hisCare.EXECUTE_USERNAME = cboNguoiThucHien_Username.Text;

                hisCare.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExecuteTime.DateTime);
                hisCare.TREATMENT_ID = this.currentTreatmentId;
                hisCare.HIS_CARE_DETAIL = new List<HIS_CARE_DETAIL>();
                hisCare.HIS_DHST1 = new List<HIS_DHST>();

                if (checkTienSuDiUng_Co.Checked)
                {
                    hisCare.ALLERGY_HISTORY = 1;
                }
                else if (checkTienSuDiUng_Khong.Checked)
                {
                    hisCare.ALLERGY_HISTORY = 0;
                }

                if (checkYThuc_Tinh.Checked)
                {
                    hisCare.AWARENESS_CONDITION = 1;
                }
                else if (checkYThuc_LoMo.Checked)
                {
                    hisCare.AWARENESS_CONDITION = 2;
                }
                else if (checkYThuc_HonMe.Checked)
                {
                    hisCare.AWARENESS_CONDITION = 3;
                }
                else if (!checkYThuc_Tinh.Checked && !checkYThuc_LoMo.Checked && !checkYThuc_HonMe.Checked)
                {
                    hisCare.AWARENESS_CONDITION = 4;
                    hisCare.AWARENESS = txtYThuc_Khac.Text.Trim();
                }

                if (checkToanTrang_BinhThuong.Checked)
                {
                    hisCare.BODY_CONDITION = 1;
                }
                else if (checkToanTrang_Met.Checked)
                {
                    hisCare.BODY_CONDITION = 2;
                }

                if (!string.IsNullOrEmpty(txtTheTrang_CanNang.Text))
                {
                    try
                    {
                        hisCare.WEIGHT = decimal.Parse(txtTheTrang_CanNang.Text.Trim());
                    }
                    catch
                    {
                        hisCare.WEIGHT = null;
                    }
                }

                if (!string.IsNullOrEmpty(txtTheTrang_ChieuCao.Text))
                {
                    try
                    {
                        hisCare.HEIGHT = decimal.Parse(txtTheTrang_ChieuCao.Text.Trim());
                    }
                    catch
                    {
                        hisCare.HEIGHT = null;
                    }
                }

                if (checkDaNiemMac_BinhThuong.Checked)
                {
                    hisCare.MUCOCUTANEOUS_CONDITION = 1;
                }
                else if (checkDaNiemMac_Hong.Checked)
                {
                    hisCare.MUCOCUTANEOUS_CONDITION = 2;
                }
                else if (checkDaNiemMac_Nhot.Checked)
                {
                    hisCare.MUCOCUTANEOUS_CONDITION = 3;
                }
                else if (!checkDaNiemMac_BinhThuong.Checked && !checkDaNiemMac_Hong.Checked && !checkDaNiemMac_Nhot.Checked)
                {
                    hisCare.MUCOCUTANEOUS_CONDITION = 4;
                    hisCare.MUCOCUTANEOUS = txtDaNiemMac_Khac.Text.Trim();
                }

                if (checkCoNang_Ngua.Checked)
                {
                    hisCare.FUNCTION_CONDITION = 1;
                }
                else if (checkCoNang_Dau.Checked)
                {
                    hisCare.FUNCTION_CONDITION = 2;
                }
                else if (checkCoNang_Rat.Checked)
                {
                    hisCare.FUNCTION_CONDITION = 3;
                }

                if (checkTonThuongDa_Tang.Checked)
                {
                    hisCare.SKIN_DAMAGE_CONDITION = 1;
                }
                else if (checkTonThuongDa_Giam.Checked)
                {
                    hisCare.SKIN_DAMAGE_CONDITION = 2;
                }
                else if (checkTonThuongDa_OnDinh.Checked)
                {
                    hisCare.SKIN_DAMAGE_CONDITION = 3;
                }
                else if (checkTonThuongDa_Kho.Checked)
                {
                    hisCare.SKIN_DAMAGE_CONDITION = 4;
                }
                else if (checkTonThuongDa_Uot.Checked)
                {
                    hisCare.SKIN_DAMAGE_CONDITION = 5;
                }

                if (!string.IsNullOrEmpty(txtDiemDau.Text))
                {
                    try
                    {
                        hisCare.PAIN_SCORE = long.Parse(txtDiemDau.Text.Trim());
                    }
                    catch
                    {
                        hisCare.PAIN_SCORE = null;
                    }
                }

                if (checkNguyCoLoet_Co.Checked)
                {
                    hisCare.CAN_ULCERS = 1;
                }
                else if (checkNguyCoLoet_Khong.Checked)
                {
                    hisCare.CAN_ULCERS = 2;
                }

                if (checkHoHap_TuTho.Checked)
                {
                    hisCare.RESPIRATORY_CONDITION = 1;
                }
                else if (checkHoHap_ThoOxy.Checked)
                {
                    hisCare.RESPIRATORY_CONDITION = 2;
                }

                if (checkThanKinh_CoGiat.Checked)
                {
                    hisCare.NEUROLOGICAL_CONDITION = 1;
                }
                else if (checkThanKinh_TangTLC.Checked)
                {
                    hisCare.NEUROLOGICAL_CONDITION = 2;
                }

                if (checkDaiTien_BinhThuong.Checked)
                {
                    hisCare.DEFECATE_CONDITION = 1;
                }
                else if (checkDaiTien_BatThuong.Checked)
                {
                    hisCare.DEFECATE_CONDITION = 2;
                }
                else if (!checkDaiTien_BinhThuong.Checked && !checkDaiTien_BatThuong.Checked)
                {
                    hisCare.DEFECATE_CONDITION = 3;
                    hisCare.DEJECTA = txtDaiTien_Khac.Text.Trim();
                }

                if (checkTieuTien_BinhThuong.Checked)
                {
                    hisCare.URINE_CONDITION = 1;
                }
                else if (checkTieuTien_BatThuong.Checked)
                {
                    hisCare.URINE_CONDITION = 2;
                }
                else if (!checkTieuTien_BinhThuong.Checked && !checkTieuTien_BatThuong.Checked)
                {
                    hisCare.URINE_CONDITION = 3;
                    hisCare.URINE = txtDaiTien_Khac.Text.Trim();
                }

                if (checkTieuHoa_TuAn.Checked)
                {
                    hisCare.DIGEST_CONDITION = 1;
                }
                else if (checkTieuHoa_QuaSonde.Checked)
                {
                    hisCare.DIGEST_CONDITION = 2;
                }
                else if (checkTieuHoa_Non.Checked)
                {
                    hisCare.DIGEST_CONDITION = 3;
                }

                hisCare.CARE_DESCRIPTION = txtDienBienKhac.Text.Trim();

                if (checkPhoBienNoiQuy_Co.Checked)
                {
                    hisCare.TUTORIAL_CONDITION = 1;
                }
                else if (checkPhoBienNoiQuy_Khong.Checked)
                {
                    hisCare.TUTORIAL_CONDITION = 0;
                }
                else if (!checkPhoBienNoiQuy_Co.Checked && !checkPhoBienNoiQuy_Khong.Checked)
                {
                    hisCare.TUTORIAL_CONDITION = 3;
                    hisCare.TUTORIAL = txtPhoBienNoiQuy_Khac.Text.Trim();
                }

                if (checkVeSinh_TamThuong.Checked)
                {
                    hisCare.SANITARY_CONDITION = 1;
                }
                else if (checkVeSinh_TamThuoc.Checked)
                {
                    hisCare.SANITARY_CONDITION = 2;
                }
                else if (!checkVeSinh_TamThuong.Checked && !checkVeSinh_TamThuoc.Checked)
                {
                    hisCare.SANITARY_CONDITION = 3;
                    hisCare.SANITARY = txtVeSinh_Khac.Text.Trim();
                }

                if (checkGiaoDucSucKhoe_Co.Checked)
                {
                    hisCare.EDUCATION_CONDITION = 1;
                }
                else if (checkGiaoDucSucKhoe_Khong.Checked)
                {
                    hisCare.EDUCATION_CONDITION = 0;
                }
                else if (!checkGiaoDucSucKhoe_Co.Checked && !checkGiaoDucSucKhoe_Khong.Checked)
                {
                    hisCare.EDUCATION_CONDITION = 2;
                    hisCare.EDUCATION = txtGiaoDucSucKhoe_Khac.Text.Trim();
                }

                hisCare.NUTRITION = txtDinhDuong.Text.Trim();

                if (checkThuocBoiDap.Checked)
                {
                    hisCare.HAS_TOPICAL_MEDICINE = 1;
                }

                if (checkThuocUong.Checked)
                {
                    hisCare.HAS_DRINK_MEDICINE = 1;
                }

                if (checkThuocTiemTruyen.Checked)
                {
                    hisCare.HAS_INFUTION_MEDICINE = 1;
                }

                if (checkThuocThuongQuy.Checked)
                {
                    hisCare.HAS_MEDICINE = 1;
                }

                if (checkThuocBoSung.Checked)
                {
                    hisCare.HAS_ADD_MEDICINE = 1;
                }

                if (checkXetNghiem.Checked)
                {
                    hisCare.HAS_TEST = 1;
                }

                if (checkPHCN.Checked)
                {
                    hisCare.HAS_REHABILITATION = 1;
                }

                if (checkCDHA.Checked)
                {
                    hisCare.HAS_DIIM = 1;
                }

                if (checkTDCN.Checked)
                {
                    hisCare.HAS_FUEX = 1;
                }

                hisCare.INSTRUCTION_DESCRIPTION = txtCanLamSangKhac.Text.Trim();

                if (checkThayBang.Checked)
                {
                    hisCare.HAS_CHANGE_BANDAGE = 1;
                }

                if (checkCatChi.Checked)
                {
                    hisCare.HAS_CUT_SUTURE = 1;
                }

                hisCare.OTHER_CARE = txtChamSocKhac.Text.Trim();

                hisCare.EVALUTE_CARE = txtDanhGia.Text.Trim();

                Inventec.Common.Logging.LogSystem.Info("lstCurrentCareDetail123: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstCurrentCareDetail), lstCurrentCareDetail));

                if (this.lstCurrentCareDetail != null && this.lstCurrentCareDetail.Count > 0)
                {
                    List<HIS_CARE_DETAIL> lstCareDetail = new List<HIS_CARE_DETAIL>();
                    foreach (var item in this.lstCurrentCareDetail)
                    {
                        if (!string.IsNullOrEmpty(item.CONTENT) && item.CARE_TYPE_ID > 0)
                        {
                            hisCare.HIS_CARE_DETAIL.Add(item);
                        }
                    }
                }

                if (this.lstCurrentDhst != null && this.lstCurrentDhst.Count > 0)
                {
                    hisCare.HIS_DHST1 = this.lstCurrentDhst;
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
                if (!dxValidationProvider1.Validate())
                    return;

                if (gridViewCareType.IsEditing)
                    gridViewCareType.CloseEditor();

                if (gridViewCareType.FocusedRowModified)
                    gridViewCareType.UpdateCurrentRow();

                if (gridViewCareType.HasColumnErrors)
                    return;

                if (CheckTemp(lstCurrentCareDetail))
                {
                    MOS.EFMODEL.DataModels.HIS_CARE_TEMP care = new HIS_CARE_TEMP();
                    ProcessDataCareTemp(ref care);
                    Inventec.Common.Logging.LogSystem.Info("HIS_CARE_TEMP: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => care), care));
                    if (care == null)
                    {
                        return;
                    }

                    frmCareTemp frmCareTemp = new frmCareTemp(this.Module, care);
                    frmCareTemp.ShowDialog();

                    cboCareTemp.EditValue = null;
                    txtCareTempCode.Text = "";
                    LoadCareTemp();
                    InitComboCareTemp();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataCareTemp(ref HIS_CARE_TEMP hisCare)
        {
            try
            {
                if (hisCare == null)
                {
                    hisCare = new HIS_CARE_TEMP();
                }

                hisCare.HIS_CARE_TEMP_DETAIL = new List<HIS_CARE_TEMP_DETAIL>();

                if (checkTienSuDiUng_Co.Checked)
                {
                    hisCare.ALLERGY_HISTORY = 1;
                }
                else if (checkTienSuDiUng_Khong.Checked)
                {
                    hisCare.ALLERGY_HISTORY = 0;
                }

                if (checkYThuc_Tinh.Checked)
                {
                    hisCare.AWARENESS_CONDITION = 1;
                }
                else if (checkYThuc_LoMo.Checked)
                {
                    hisCare.AWARENESS_CONDITION = 2;
                }
                else if (checkYThuc_HonMe.Checked)
                {
                    hisCare.AWARENESS_CONDITION = 3;
                }
                else if (!checkYThuc_Tinh.Checked && !checkYThuc_LoMo.Checked && !checkYThuc_HonMe.Checked)
                {
                    hisCare.AWARENESS_CONDITION = 4;
                    hisCare.AWARENESS = txtYThuc_Khac.Text.Trim();
                }

                if (checkToanTrang_BinhThuong.Checked)
                {
                    hisCare.BODY_CONDITION = 1;
                }
                else if (checkToanTrang_Met.Checked)
                {
                    hisCare.BODY_CONDITION = 2;
                }

                if (checkDaNiemMac_BinhThuong.Checked)
                {
                    hisCare.MUCOCUTANEOUS_CONDITION = 1;
                }
                else if (checkDaNiemMac_Hong.Checked)
                {
                    hisCare.MUCOCUTANEOUS_CONDITION = 2;
                }
                else if (checkDaNiemMac_Nhot.Checked)
                {
                    hisCare.MUCOCUTANEOUS_CONDITION = 3;
                }
                else if (!checkDaNiemMac_BinhThuong.Checked && !checkDaNiemMac_Hong.Checked && !checkDaNiemMac_Nhot.Checked)
                {
                    hisCare.MUCOCUTANEOUS_CONDITION = 4;
                    hisCare.MUCOCUTANEOUS = txtDaNiemMac_Khac.Text.Trim();
                }

                if (checkCoNang_Ngua.Checked)
                {
                    hisCare.FUNCTION_CONDITION = 1;
                }
                else if (checkCoNang_Dau.Checked)
                {
                    hisCare.FUNCTION_CONDITION = 2;
                }
                else if (checkCoNang_Rat.Checked)
                {
                    hisCare.FUNCTION_CONDITION = 3;
                }

                if (checkTonThuongDa_Tang.Checked)
                {
                    hisCare.SKIN_DAMAGE_CONDITION = 1;
                }
                else if (checkTonThuongDa_Giam.Checked)
                {
                    hisCare.SKIN_DAMAGE_CONDITION = 2;
                }
                else if (checkTonThuongDa_OnDinh.Checked)
                {
                    hisCare.SKIN_DAMAGE_CONDITION = 3;
                }
                else if (checkTonThuongDa_Kho.Checked)
                {
                    hisCare.SKIN_DAMAGE_CONDITION = 4;
                }
                else if (checkTonThuongDa_Uot.Checked)
                {
                    hisCare.SKIN_DAMAGE_CONDITION = 5;
                }

                //if (!string.IsNullOrEmpty(txtDiemDau.Text))
                //{
                //    try
                //    {
                //        hisCare.PAIN_SCORE = long.Parse(txtDiemDau.Text.Trim());
                //    }
                //    catch
                //    {
                //        hisCare.PAIN_SCORE = null;
                //    }
                //}

                if (checkNguyCoLoet_Co.Checked)
                {
                    hisCare.CAN_ULCERS = 1;
                }
                else if (checkNguyCoLoet_Khong.Checked)
                {
                    hisCare.CAN_ULCERS = 2;
                }

                if (checkHoHap_TuTho.Checked)
                {
                    hisCare.RESPIRATORY_CONDITION = 1;
                }
                else if (checkHoHap_ThoOxy.Checked)
                {
                    hisCare.RESPIRATORY_CONDITION = 2;
                }

                if (checkThanKinh_CoGiat.Checked)
                {
                    hisCare.NEUROLOGICAL_CONDITION = 1;
                }
                else if (checkThanKinh_TangTLC.Checked)
                {
                    hisCare.NEUROLOGICAL_CONDITION = 2;
                }

                if (checkDaiTien_BinhThuong.Checked)
                {
                    hisCare.DEFECATE_CONDITION = 1;
                }
                else if (checkDaiTien_BatThuong.Checked)
                {
                    hisCare.DEFECATE_CONDITION = 2;
                }
                else if (!checkDaiTien_BinhThuong.Checked && !checkDaiTien_BatThuong.Checked)
                {
                    hisCare.DEFECATE_CONDITION = 3;
                    hisCare.DEJECTA = txtDaiTien_Khac.Text.Trim();
                }

                if (checkTieuTien_BinhThuong.Checked)
                {
                    hisCare.URINE_CONDITION = 1;
                }
                else if (checkTieuTien_BatThuong.Checked)
                {
                    hisCare.URINE_CONDITION = 2;
                }
                else if (!checkTieuTien_BinhThuong.Checked && !checkTieuTien_BatThuong.Checked)
                {
                    hisCare.URINE_CONDITION = 3;
                    hisCare.URINE = txtDaiTien_Khac.Text.Trim();
                }

                if (checkTieuHoa_TuAn.Checked)
                {
                    hisCare.DIGEST_CONDITION = 1;
                }
                else if (checkTieuHoa_QuaSonde.Checked)
                {
                    hisCare.DIGEST_CONDITION = 2;
                }
                else if (checkTieuHoa_Non.Checked)
                {
                    hisCare.DIGEST_CONDITION = 3;
                }

                hisCare.CARE_DESCRIPTION = txtDienBienKhac.Text.Trim();

                if (checkPhoBienNoiQuy_Co.Checked)
                {
                    hisCare.TUTORIAL_CONDITION = 1;
                }
                else if (checkPhoBienNoiQuy_Khong.Checked)
                {
                    hisCare.TUTORIAL_CONDITION = 0;
                }
                else if (!checkPhoBienNoiQuy_Co.Checked && !checkPhoBienNoiQuy_Khong.Checked)
                {
                    hisCare.TUTORIAL_CONDITION = 3;
                    hisCare.TUTORIAL = txtPhoBienNoiQuy_Khac.Text.Trim();
                }

                if (checkVeSinh_TamThuong.Checked)
                {
                    hisCare.SANITARY_CONDITION = 1;
                }
                else if (checkVeSinh_TamThuoc.Checked)
                {
                    hisCare.SANITARY_CONDITION = 2;
                }
                else if (!checkVeSinh_TamThuong.Checked && !checkVeSinh_TamThuoc.Checked)
                {
                    hisCare.SANITARY_CONDITION = 3;
                    hisCare.SANITARY = txtVeSinh_Khac.Text.Trim();
                }

                if (checkGiaoDucSucKhoe_Co.Checked)
                {
                    hisCare.EDUCATION_CONDITION = 1;
                }
                else if (checkGiaoDucSucKhoe_Khong.Checked)
                {
                    hisCare.EDUCATION_CONDITION = 0;
                }
                else if (!checkGiaoDucSucKhoe_Co.Checked && !checkGiaoDucSucKhoe_Khong.Checked)
                {
                    hisCare.EDUCATION_CONDITION = 2;
                    hisCare.EDUCATION = txtGiaoDucSucKhoe_Khac.Text.Trim();
                }

                hisCare.NUTRITION = txtDinhDuong.Text.Trim();

                if (checkThuocBoiDap.Checked)
                {
                    hisCare.HAS_TOPICAL_MEDICINE = 1;
                }

                if (checkThuocUong.Checked)
                {
                    hisCare.HAS_DRINK_MEDICINE = 1;
                }

                if (checkThuocTiemTruyen.Checked)
                {
                    hisCare.HAS_INFUTION_MEDICINE = 1;
                }

                if (checkThuocThuongQuy.Checked)
                {
                    hisCare.HAS_MEDICINE = 1;
                }

                if (checkThuocBoSung.Checked)
                {
                    hisCare.HAS_ADD_MEDICINE = 1;
                }

                if (checkXetNghiem.Checked)
                {
                    hisCare.HAS_TEST = 1;
                }

                if (checkPHCN.Checked)
                {
                    hisCare.HAS_REHABILITATION = 1;
                }

                if (checkCDHA.Checked)
                {
                    hisCare.HAS_DIIM = 1;
                }

                if (checkTDCN.Checked)
                {
                    hisCare.HAS_FUEX = 1;
                }

                hisCare.INSTRUCTION_DESCRIPTION = txtCanLamSangKhac.Text.Trim();

                if (checkThayBang.Checked)
                {
                    hisCare.HAS_CHANGE_BANDAGE = 1;
                }

                if (checkCatChi.Checked)
                {
                    hisCare.HAS_CUT_SUTURE = 1;
                }

                hisCare.OTHER_CARE = txtChamSocKhac.Text.Trim();

                hisCare.EVALUTE_CARE = txtDanhGia.Text.Trim();

                Inventec.Common.Logging.LogSystem.Info("lstCurrentCareDetail: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstCurrentCareDetail), lstCurrentCareDetail));

                if (this.lstCurrentCareDetail != null)
                {
                    foreach (var item in this.lstCurrentCareDetail)
                    {
                        if (item.CARE_TYPE_ID > 0 && !string.IsNullOrEmpty(item.CONTENT))
                        {
                            HIS_CARE_TEMP_DETAIL details = new HIS_CARE_TEMP_DETAIL();
                            details.CARE_TYPE_ID = item.CARE_TYPE_ID;
                            details.CONTENT = item.CONTENT;
                            hisCare.HIS_CARE_TEMP_DETAIL.Add(details);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckTemp(List<HIS_CARE_DETAIL> HisCareDetails)
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

        private void ddBtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ddBtnPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboNguoiThucHien_Username_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDhstThoiGianDo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDhstCanNang.Focus();
                    txtDhstCanNang.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichVao_Truyen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkDhstDichVao_An.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichVao_An_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkDhstDichVao_Khac.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichVao_Khac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkDhstDichRa_NuocTieu.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichRa_NuocTieu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkDhstDichRa_DanLuu.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichRa_DanLuu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkDhstDichRa_Phan.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDhstDichRa_Phan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnDhstAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnDhstAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnDhstEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnDhstEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnDhstNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnDhstNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bbtnSaveTemp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveTemp_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTienSuDiUng_Co_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkTienSuDiUng_Khong.Focus();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("checkTienSuDiUng_Khong: " + checkTienSuDiUng_Khong);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTienSuDiUng_Khong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkYThuc_Tinh_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkYThuc_LoMo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkYThuc_HonMe_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtYThuc_Khac_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkToanTrang_Met_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkToanTrang_BinhThuong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtTheTrang_CanNang_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTheTrang_ChieuCao_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDaNiemMac_Hong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkDaNiemMac_Nhot_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkDaNiemMac_BinhThuong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDaNiemMac_Khac_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkCoNang_Ngua_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkCoNang_Dau_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkCoNang_Rat_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTonThuongDa_Tang_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTonThuongDa_Giam_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkTonThuongDa_OnDinh_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTonThuongDa_Kho_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTonThuongDa_Uot_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiemDau_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkNguyCoLoet_Co_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkNguyCoLoet_Khong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkHoHap_TuTho_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkHoHap_ThoOxy_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkThanKinh_CoGiat_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkThanKinh_TangTLC_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkDaiTien_BinhThuong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkDaiTien_BatThuong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDaiTien_Khac_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTieuTien_BinhThuong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkTieuTien_BatThuong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTieuTien_Khac_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTieuHoa_TuAn_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTieuHoa_QuaSonde_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkTieuHoa_Non_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDienBienKhac_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkPhoBienNoiQuy_Co.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkPhoBienNoiQuy_Co_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkPhoBienNoiQuy_Khong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtPhoBienNoiQuy_Khac_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkVeSinh_TamThuong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkVeSinh_TamThuoc_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtVeSinh_Khac_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkGiaoDucSucKhoe_Co_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkGiaoDucSucKhoe_Khong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGiaoDucSucKhoe_Khac_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                    txtDinhDuong.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDinhDuong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkThuocBoiDap_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkThuocUong_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkThuocTiemTruyen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkThuocThuongQuy_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkThuocBoSung_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkXetNghiem_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkPHCN_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkCDHA_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkTDCN_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtCanLamSangKhac_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void checkThayBang_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkCatChi_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtChamSocKhac_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCareTemp_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExecuteTime.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCareTempCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtCareTempCode.Text))
                    {
                        string code = txtCareTempCode.Text.Trim();

                        List<HIS_CARE_TEMP> lstData = this.lstCareTemp != null ? this.lstCareTemp.Where(o => (o.CARE_TEMP_CODE != null && o.CARE_TEMP_CODE.Contains(code))).ToList() : null;

                        if (lstData.Count == 1)
                        {
                            cboCareTemp.EditValue = lstData[0].ID;
                            dtExecuteTime.Focus();
                        }
                        else
                        {
                            cboCareTemp.Focus();
                            cboCareTemp.ShowPopup();
                        }
                    }
                    else
                    {
                        cboCareTemp.Focus();
                        cboCareTemp.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider2_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        private void btnConnectBloodPressure_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_DHST data = HIS.Desktop.Plugins.Library.ConnectBloodPressure.ConnectBloodPressureProcessor.GetData();
                if (data != null)
                {
                    if (data.EXECUTE_TIME != null)
                        dtDhstThoiGianDo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtDhstThoiGianDo.EditValue = DateTime.Now;

                    if (data.BLOOD_PRESSURE_MAX.HasValue)
                    {
                        txtDhstHuyetAp_Max.EditValue = data.BLOOD_PRESSURE_MAX;
                    }
                    if (data.BLOOD_PRESSURE_MIN.HasValue)
                    {
                        txtDhstHuyetAp_Min.EditValue = data.BLOOD_PRESSURE_MIN;
                    }
                    if (data.BREATH_RATE.HasValue)
                    {
                        txtDhstNhipTho.EditValue = data.BREATH_RATE;
                    }
                    if (data.HEIGHT.HasValue)
                    {
                        txtDhstChieuCao.EditValue = data.HEIGHT;
                    }
                    if (data.CHEST.HasValue)
                    {
                        txtDhstVongNguc.EditValue = data.CHEST;
                    }
                    if (data.BELLY.HasValue)
                    {
                        txtDhstVongBung.EditValue = data.BELLY;
                    }
                    if (data.PULSE.HasValue)
                    {
                        txtDhstMach.EditValue = data.PULSE;
                    }
                    if (data.TEMPERATURE.HasValue)
                    {
                        txtDhstNhietDo.EditValue = data.TEMPERATURE;
                    }
                    if (data.WEIGHT.HasValue)
                    {
                        txtDhstCanNang.EditValue = data.WEIGHT;
                    }
                    if (!String.IsNullOrWhiteSpace(data.NOTE))
                    {
                        txtDhstKhac.Text = data.NOTE;
                    }
                    if (data.SPO2.HasValue)
                        txtDhstSpo2.Value = (data.SPO2.Value * 100);
                    else
                        txtDhstSpo2.EditValue = null;
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
                if (isFirst && this.currentTracking != null)
                {
                    isFirst = false;
                    cboTracking.EditValue = this.currentTracking.ID;
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

                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId));

                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
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
                    this.currentTracking = this.trackings != null && this.trackings.Count > 0 ? this.trackings.FirstOrDefault(o => o.ID == (long)cboTracking.EditValue) : new HIS_TRACKING();
                }
                else
                {
                    this.currentTracking = new HIS_TRACKING();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            } 
        }
    }
}
