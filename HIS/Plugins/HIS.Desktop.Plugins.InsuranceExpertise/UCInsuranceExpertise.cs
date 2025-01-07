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
using HIS.UC.SereServTree;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using MOS.Filter;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.InsuranceExpertise.Config;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using MOS.SDO;
using HIS.Desktop.Plugins.InsuranceExpertise.Base;
using His.Bhyt.ExportXml.Base;
using System.IO;
using HIS.Desktop.Utility;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Common.Adapter;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.Plugins.InsuranceExpertise.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using ACS.SDO;

namespace HIS.Desktop.Plugins.InsuranceExpertise
{
    public partial class UCInsuranceExpertise : HIS.Desktop.Utility.UserControlBase
    {
        SereServTreeProcessor ssTreeProcessor;
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_TREATMENT_1 currentTreatment = null;
        HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter = null;
        List<V_HIS_TREATMENT_1> listSelectTreatment = new List<V_HIS_TREATMENT_1>();
        List<V_HIS_SERE_SERV_5> listSereServ = new List<V_HIS_SERE_SERV_5>();
        List<HIS_PATIENT_TYPE_ALTER> listPatientTypeAlter = new List<HIS_PATIENT_TYPE_ALTER>();
        List<HIS_HEIN_APPROVAL> listHeinApproval = new List<HIS_HEIN_APPROVAL>();
        List<V_HIS_TREATMENT_1> listTreatment = new List<V_HIS_TREATMENT_1>();

        List<V_HIS_SERE_SERV_TEIN> hisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
        List<V_HIS_SERE_SERV_PTTT> hisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();
        List<HIS_DHST> listDhst = new List<HIS_DHST>();
        List<HIS_TRACKING> hisTrackings = new List<HIS_TRACKING>();
        List<V_HIS_TREATMENT_3> hisTreatments = new List<V_HIS_TREATMENT_3>();
        List<V_HIS_SERE_SERV_2> ListSereServ = new List<V_HIS_SERE_SERV_2>();
        List<HIS_EKIP_USER> ListEkipUser = new List<HIS_EKIP_USER>();
        List<V_HIS_BED_LOG> ListBedlog = new List<V_HIS_BED_LOG>();
        List<V_HIS_HEIN_APPROVAL> listViewHeinApproval = new List<V_HIS_HEIN_APPROVAL>();
        List<TreatmentImportADO> listTreatmentImport;
        List<HIS_TREATMENT_TYPE> treatmentTypeSelecteds = new List<HIS_TREATMENT_TYPE>();
        HIS_BRANCH _Branch;
        long patientTypeIdBhyt = 0;
        CommonParam param = new CommonParam();

        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;

        long RoomId;
        long RoomTypeId;
        V_HIS_CASHIER_ROOM cashierRoom = null;

        bool isInitXmlLocalData = false;

        int positionHandleControl = -1;

        UserControl ucSereServTree;
        DateTime currentTimer;
        TimerSDO timeSync { get; set; }

        public UCInsuranceExpertise(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                InitUcTreeSereServ();
                this.RoomId = _moduleData.RoomId;
                this.RoomTypeId = _moduleData.RoomTypeId;
                this.currentModule = _moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcTreeSereServ()
        {
            try
            {
                ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowSearchPanel = false;
                ado.IsCreateParentNodeWithSereServExpend = false;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                //Column tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__TREE_SERE_SERV__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 200, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);

                //Column Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__TREE_SERE_SERV__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AMOUNT_PLUS", 40, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(amountCol);

                //Column đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__TREE_SERE_SERV__COLUMN_VIR_PRICE", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_PRICE", 110, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.Format.FormatString = "#,##0.0000";
                virPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virPriceCol);

                //Column thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PRICE", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PRICE", 110, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPriceCol);

                //Column đồng chi trả
                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__TREE_SERE_SERV__COLUMN_VIR_TOTAL_HEIN_PRICE", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_HEIN_PRICE", 110, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.Format.FormatString = "#,##0.0000";
                virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                //Column bệnh nhân trả
                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PATIENT_PRICE", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PATIENT_PRICE", 110, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);

                //Column chiết khấu
                SereServTreeColumn virDiscountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__TREE_SERE_SERV__COLUMN_DISCOUNT", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "DISCOUNT", 110, false);
                virDiscountCol.VisibleIndex = 6;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.Format.FormatString = "#,##0.0000";
                virDiscountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virDiscountCol);

                //Column hao phí
                SereServTreeColumn virIsExpendCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__TREE_SERE_SERV__COLUMN_IS_EXPEND", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IsExpend", 60, false);
                virIsExpendCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(virIsExpendCol);

                //Column vat (%)
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__TREE_SERE_SERV__COLUMN_VAT_RATIO", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VAT", 100, false);
                virVatRatioCol.VisibleIndex = 8;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                //Column mã dịch vụ
                SereServTreeColumn serviceCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__TREE_SERE_SERV__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                //Column Mã yêu cầu
                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__TREE_SERE_SERV__COLUMN_SERVICE_REQ_CODE", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);

                this.ucSereServTree = (UserControl)ssTreeProcessor.Run(ado);
                if (this.ucSereServTree != null)
                {
                    this.panelControlSereServTree.Controls.Add(this.ucSereServTree);
                    this.ucSereServTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCInsuranceExpertise_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                LoadKeyUCLanguage();
                ValidControl();
                LoadCashierRoom();
                SetDefaultValueControl();
                SetDisableButton();
                InitComboTreatmentType();
                FillDataToGridTreatment();
                LoadDataEmployeestoXML();
                GetTimeSystem();
                RegisterTimer(currentModule.ModuleLink, "timer1", timer1.Interval, timer1_Tick);
                StartTimer(currentModule.ModuleLink, "timer1");
                if (Config.HisConfigCFG.OptionStoreBordereauCode == "2")
                {
                    lcStoreBordereauCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dxValidationProvider1.SetValidationRule(txtStoreBordereauCode, null);
                }
                else if (Config.HisConfigCFG.OptionStoreBordereauCode == "1")
                {
                    esOption2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciDteOption2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciTxtStoreCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void GetTimeSystem()
        {
            try
            {
                timeSync = new BackendAdapter(new CommonParam()).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, new CommonParam());
                currentTimer = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeSync.LocalTime) ?? DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboTreatmentType()
        {
            try
            {
                try
                {
                    GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboTreatmentType.Properties);
                    gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboTreatmentType);
                    cboTreatmentType.Properties.Tag = gridCheck;
                    cboTreatmentType.Properties.View.OptionsSelection.MultiSelect = true;

                    CommonParam param = new CommonParam();
                    HisTreatmentTypeFilter filter = new HisTreatmentTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var data = new BackendAdapter(param).Get<List<HIS_TREATMENT_TYPE>>("api/HisTreatmentType/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                    if (data != null)
                    {
                        cboTreatmentType.Properties.DataSource = data;
                        cboTreatmentType.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
                        cboTreatmentType.Properties.ValueMember = "ID";
                        DevExpress.XtraGrid.Columns.GridColumn col2 = cboTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_CODE");
                        col2.VisibleIndex = 1;
                        col2.Width = 100;
                        col2.Caption = "";
                        DevExpress.XtraGrid.Columns.GridColumn col3 = cboTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_NAME");
                        col3.VisibleIndex = 2;
                        col3.Width = 250;
                        col3.Caption = "";

                        cboTreatmentType.Properties.PopupFormWidth = 200;
                        cboTreatmentType.Properties.View.OptionsView.ShowColumnHeaders = false;
                        cboTreatmentType.Properties.View.OptionsSelection.MultiSelect = true;
                        GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.ClearSelection(cboTreatmentType.Properties.View);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboTreatmentType(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                treatmentTypeSelecteds = new List<HIS_TREATMENT_TYPE>();
                if (gridCheckMark != null)
                {
                    foreach (HIS_TREATMENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        treatmentTypeSelecteds.Add(er);
                        if (er == null)
                            continue;
                        typeName += er.TREATMENT_TYPE_NAME + ",";
                    }
                    cboTreatmentType.Text = typeName;
                    cboTreatmentType.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCashierRoom()
        {
            try
            {
                cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == this.RoomId && o.ROOM_TYPE_ID == this.RoomTypeId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0) ?? DateTime.MinValue;
                dtFeeLockTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                dtFeeLockTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;
                txtKeyword.Text = "";
                btnLockHein.Enabled = false;
                btnUnLockHein.Enabled = false;
                if (cboStatus.Items.Count == 0)
                {
                    cboStatus.Items.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__COMBO_STATUS__ITEM_ALL", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                    cboStatus.Items.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__COMBO_STATUS__ITEM_APPROVALED", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                    cboStatus.Items.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__COMBO_STATUS__ITEM_NOT_APPROVAL", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));

                    cboStatus.Items.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__COMBO_STATUS__ITEM_IS_UNLOCK_HEIN", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));

                    cboStatus.Items.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__COMBO_STATUS__ITEM_IS_LOCK_HEIN", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));

                    cboStatus.Items.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__COMBO_STATUS__ITEM_HAS_APPROVAL_NOT_XML", Base.ResourceLangManager.LanguageUCInsuranceExpertise, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));

                }
                cboStatus.SelectedIndex = 0;
                if (cboTimeFrom.Items.Count == 0)
                {
                    cboTimeFrom.Items.Add(Base.ResourceMessageLang.ThoiGianKhoaVienPhiTu);
                    cboTimeFrom.Items.Add(Base.ResourceMessageLang.ThoiGianKetThucDieuTriTu);
                    cboTimeFrom.Items.Add(Base.ResourceMessageLang.ThoiGianDuyetKhoaBHYTTu);
                }
                cboTimeFrom.SelectedIndex = 0;
                checkCurrentBranch.Checked = true;
                if (TreatmentBranchCFG.IsShowAllBranch)
                {
                    checkCurrentBranch.Enabled = true;
                }
                else
                {
                    checkCurrentBranch.Enabled = false;
                }
                //Id bhyt
                if (this.patientTypeIdBhyt <= 0)
                {
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisPatientTypeCFG.PATIENT_TYPE_CODE__BHYT);
                    if (patientType != null)
                    {
                        patientTypeIdBhyt = patientType.ID;
                    }
                }

                this._Branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());

                dtHeinLockTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0) ?? DateTime.MinValue;

                if (HisConfigCFG.isGenerateStoreBordereauCodeWhenLockHein == "1" || HisConfigCFG.isGenerateStoreBordereauCodeWhenLockHein == "2")
                    lcStoreBordereauCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                else
                    lcStoreBordereauCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                GridCheckMarksSelection gridCheckMarkPart = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                gridCheckMarkPart.ClearSelection(cboTreatmentType.Properties.View);
                cboTreatmentType.Text = "";

            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataEmployeestoXML()
        {
            try
            {
                CommonParam paramGet = new CommonParam();
                MOS.Filter.HisEmployeeFilter filter = new HisEmployeeFilter();
                His.Bhyt.ExportXml.Base.GlobalConfigStore.ListEmployees = new Inventec.Common.Adapter.BackendAdapter(paramGet).Get<List<HIS_EMPLOYEE>>("/api/HisEmployee/Get", ApiConsumers.MosConsumer, filter, paramGet);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment()
        {
            try
            {
                FillDataToGridTreatment(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTreatment, param, (int)ConfigApplications.NumPageSize, this.gridControlTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment(object param)
        {
            try
            {
                listTreatment = new List<V_HIS_TREATMENT_1>();
                ResetLocalData();
                gridControlTreatment.DataSource = null;
                gridControlHeinCard.DataSource = null;
                gridControlHeinApproval.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisTreatmentView1Filter treatFilter = new HisTreatmentView1Filter();
                treatFilter.ORDER_FIELD = "FEE_LOCK_TIME";
                treatFilter.ORDER_DIRECTION = "ACS";
                treatFilter.IS_PAUSE = true;
                treatFilter.HAS_PATY_ALTER_BHYT = true;
                if (cboStatus.SelectedIndex == 1)
                {
                    treatFilter.HAS_HEIN_APPROVAL = true;
                }
                else if (cboStatus.SelectedIndex == 2)
                {
                    treatFilter.HAS_HEIN_APPROVAL = false;
                }
                else if (cboStatus.SelectedIndex == 3)
                {
                    treatFilter.IS_LOCK_HEIN = false;
                }
                else if (cboStatus.SelectedIndex == 4)
                {
                    treatFilter.IS_LOCK_HEIN = true;
                }
                else if (cboStatus.SelectedIndex == 5)
                {
                    treatFilter.HAS_NO_XML_URL_HEIN_APPROVAL = true;
                    treatFilter.HAS_HEIN_APPROVAL = true;
                }
                if (this.treatmentTypeSelecteds != null && this.treatmentTypeSelecteds.Count() > 0)
                {
                    treatFilter.TDL_TREATMENT_TYPE_IDs = treatmentTypeSelecteds.Select(o => o.ID).ToList();
                }
                if (checkCurrentBranch.Checked)
                {
                    treatFilter.BRANCH_ID = WorkPlace.GetBranchId();
                }
                else
                {
                    treatFilter.BRANCH_ID = null;
                }

                if (!String.IsNullOrEmpty(txtFindTreatmentCode.Text))
                {
                    string code = txtFindTreatmentCode.Text.Trim();
                    if (code.Length <= 12)
                    {
                        try
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtFindTreatmentCode.Text = code;
                            treatFilter.TREATMENT_CODE__EXACT = code;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                }
                else if (!string.IsNullOrEmpty(txtStorebcode.Text.Trim()) && lciTxtStoreCode.Visible)
                {
                    //string code = txtStorebcode.Text.Trim();
                    //if (code.Length < 5)
                    //{
                    //    code = string.Format("{0:00000}", Convert.ToInt64(code));
                    //    txtStorebcode.Text = code;
                    //}
                    treatFilter.STORE_BORDEREAU_CODE__EXACT = txtStorebcode.Text.Trim();
                }
                else if (!String.IsNullOrEmpty(txtKeyword.Text))
                {
                    treatFilter.KEY_WORD__VIR_PATIENT_NAME__PATIENT_CODE__TREATMENT_CODE__GENDER_NAME = txtKeyword.Text;
                }

                if (String.IsNullOrEmpty(treatFilter.TREATMENT_CODE__EXACT))
                {
                    if (cboTimeFrom.SelectedIndex == 0)
                    {
                        if (dtFeeLockTimeFrom.EditValue != null && dtFeeLockTimeFrom.DateTime != DateTime.MinValue)
                        {
                            treatFilter.FEE_LOCK_TIME_FROM = Convert.ToInt64(dtFeeLockTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                        }

                        if (dtFeeLockTimeTo.EditValue != null && dtFeeLockTimeTo.DateTime != DateTime.MinValue)
                        {
                            treatFilter.FEE_LOCK_TIME_TO = Convert.ToInt64(dtFeeLockTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");
                        }
                    }
                    else if (cboTimeFrom.SelectedIndex == 1)
                    {
                        if (dtFeeLockTimeFrom.EditValue != null && dtFeeLockTimeFrom.DateTime != DateTime.MinValue)
                        {
                            treatFilter.OUT_TIME_FROM = Convert.ToInt64(dtFeeLockTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                        }

                        if (dtFeeLockTimeTo.EditValue != null && dtFeeLockTimeTo.DateTime != DateTime.MinValue)
                        {
                            treatFilter.OUT_TIME_TO = Convert.ToInt64(dtFeeLockTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");
                        }
                    }
                    else if (cboTimeFrom.SelectedIndex == 2)
                    {
                        if (dtFeeLockTimeFrom.EditValue != null && dtFeeLockTimeFrom.DateTime != DateTime.MinValue)
                        {
                            treatFilter.HEIN_LOCK_TIME_FROM = Convert.ToInt64(dtFeeLockTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                        }

                        if (dtFeeLockTimeTo.EditValue != null && dtFeeLockTimeTo.DateTime != DateTime.MinValue)
                        {
                            treatFilter.HEIN_LOCK_TIME_TO = Convert.ToInt64(dtFeeLockTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");
                        }
                    }

                }

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_1>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW_1, ApiConsumers.MosConsumer, treatFilter, paramCommon);
                if (result != null)
                {
                    listTreatment = (List<V_HIS_TREATMENT_1>)result.Data;
                    rowCount = (listTreatment == null ? 0 : listTreatment.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }
                SetDisableButton();
                gridControlTreatment.BeginUpdate();
                gridControlTreatment.DataSource = listTreatment;
                gridControlTreatment.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetLocalData()
        {
            try
            {
                this.currentPatientTypeAlter = null;
                this.currentTreatment = null;
                this.listSelectTreatment = new List<V_HIS_TREATMENT_1>();
                this.listHeinApproval = new List<HIS_HEIN_APPROVAL>();
                this.listPatientTypeAlter = new List<HIS_PATIENT_TYPE_ALTER>();
                this.listSereServ = new List<V_HIS_SERE_SERV_5>();
                FillDataToSereServTree(this.listSereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDisableButton()
        {
            try
            {
                btnLockHein.Enabled = false;
                btnUnLockHein.Enabled = false;
                if (HisConfigCFG.OptionStoreBordereauCode == "1" && (HisConfigCFG.isGenerateStoreBordereauCodeWhenLockHein == "1" || HisConfigCFG.isGenerateStoreBordereauCodeWhenLockHein == "2"))
                    layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                else
                    layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;


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
                ValidControlFindTreatmentCode();
                if (HisConfigCFG.isGenerateStoreBordereauCodeWhenLockHein == "1" || HisConfigCFG.isGenerateStoreBordereauCodeWhenLockHein == "2")
                {
                    this.lcStoreBordereauCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lcStoreBordereauCode.AppearanceItemCaption.Options.UseForeColor = true;
                    ValidationControlTxtStoreBordereauCode();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlFindTreatmentCode()
        {
            try
            {

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

        private void LoadKeyUCLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Base.ResourceLangManager.LanguageUCInsuranceExpertise = new ResourceManager("HIS.Desktop.Plugins.InsuranceExpertise.Resources.Lang", typeof(UCInsuranceExpertise).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutControl1.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.btnLuuTru.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.btnLuuTru.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.btnLuuTru.ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.btnLuuTru.ToolTip", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.btnDownload.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.btnDownload.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.btnDownload.ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.btnDownload.ToolTip", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.btnImport.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.checkCurrentBranch.Properties.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.checkCurrentBranch.Properties.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.txtFindTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.txtFindTreatmentCode.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.btnUnLockHein.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.btnUnLockHein.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.btnLockHein.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.btnLockHein.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.btnFind.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridCol_HeinApproval_Cancel.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridCol_HeinApproval_Cancel.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridCol_HeinApproval_DownXml.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridCol_HeinApproval_DownXml.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn1.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_HeinApproval_HeinApprovalCode.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_HeinApproval_HeinApprovalCode.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_HeinApproval_ExecuteTime.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_HeinApproval_ExecuteTime.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_HeinApproval_ExecuteUserName.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_HeinApproval_ExecuteUserName.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_HeinCardNumber_Approval.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_HeinCardNumber_Approval.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_HeinCard_HeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_HeinCard_HeinCardNumber.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_HeinCard_HeinCardFromTime.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_HeinCard_HeinCardFromTime.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_HeinCard_HeinCardToTime.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_HeinCard_HeinCardToTime.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_Treatment_Stt.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_Treatment_Stt.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_Treatment_ImageStatus.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_Treatment_ImageStatus.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumnTreatment_GiamDinh.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumnTreatment_GiamDinh.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumnTreatment_GiamDinh.ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumnTreatment_GiamDinh.ToolTip", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumnTreatment_DownloadXML.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumnTreatment_DownloadXML.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumnTreatment_XMLView.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumnTreatment_XMLView.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_Treatment_IsLockHein.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_Treatment_IsLockHein.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_Treatment_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_Treatment_TreatmentCode.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_Treatment_PatientCode.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_Treatment_PatientCode.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_Treatment_VirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_Treatment_VirPatientName.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn2.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_Treatment_GenderName.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_Treatment_GenderName.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_Treatment_Dob.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_Treatment_Dob.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_Treatment_InTime.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_Treatment_InTime.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.gridColumn_Treatment_OutTime.Caption = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_Treatment_OutTime.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.cboTreatmentType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.cboTreatmentType.Properties.NullText", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutFeeLockTimeFrom.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutFeeLockTimeFrom.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutFeeLockTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutFeeLockTimeFrom.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutFeeLockTimeTo.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutFeeLockTimeTo.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutStatus.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutStatus.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutVirTotalPrice.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutVirTotalPrice.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutVirTotalHeinPrice.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutVirTotalHeinPrice.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutExecuteTime.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutExecuteTime.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutControlItem8.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutControlItem8.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutControlItem8.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutVirTotalPatientPrice.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutVirTotalPatientPrice.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutControlItem9.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutControlItem9.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.layoutControlItem15.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.lcStoreBordereauCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.lcStoreBordereauCode.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.lcStoreBordereauCode.Text = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.lcStoreBordereauCode.Text", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                //Repository Button
                this.repositoryItemImgApprovaled.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__REPOSITORY_STATUS_APPROVALED", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItemImgIsLockHein.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__REPOSITORY_STATUS_LOCK_HEIN", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItemImgIsUnlockHein.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__REPOSITORY_STATUS_UNLOCK_HEIN", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItemImgNotApproval.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__REPOSITORY_STATUS_NOT_APPROVAL", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItemBtnApprovalOne.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn_HeinCardNumber_Approval.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItemBtnCancelApproval.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridCol_HeinApproval_Cancel.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItemBtnCancelApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridCol_HeinApproval_Cancel.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItemBtnDownXml.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridCol_HeinApproval_DownXml.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItemBtnDownXmlDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridCol_HeinApproval_DownXml.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItemViewXmlEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn1.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItemViewXmlDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn1.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());

                this.repositoryItem_XMlViewDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn1.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItem_XMLView.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumn1.Caption", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItem_DowloadDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__repositoryItem_Dowload", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItem_Dowload.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_INSURANCE_EXPERTISE__repositoryItem_Dowload", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());
                this.repositoryItemButtonEdit_GiamDinhHSDT.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UCInsuranceExpertise.gridColumnTreatment_GiamDinh.ToolTip", Base.ResourceLangManager.LanguageUCInsuranceExpertise, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnFind()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnApproval()
        {
            try
            {
                repositoryItemButtonEdit_GiamDinhHSDT_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnLockHein()
        {
            try
            {
                btnLockHein_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnUnlockHein()
        {
            try
            {
                btnUnLockHein_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnFocusTreatmentCode()
        {
            try
            {
                txtFindTreatmentCode.Focus();
                txtFindTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = gridViewTreatment.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        var treatment1 = (V_HIS_TREATMENT_1)view.GetRow(hi.RowHandle);

                        this.currentTreatment = treatment1;
                        if (treatment1 != null && !String.IsNullOrEmpty(treatment1.XML4210_URL))
                        {
                            if (hi.Column.FieldName == "DOWNLOAD_XML")
                            {
                                repositoryItem_Dowload_ButtonClick(null, null);
                            }
                            else if (hi.Column.FieldName == "VIEW_XML")
                            {
                                repositoryItem_XMLView_ButtonClick(null, null);
                            }
                        }
                        ClickTreatment(treatment1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ClickTreatment(V_HIS_TREATMENT_1 treatment1)
        {

            try
            {
                dteStoreTime.DateTime = DateTime.Now;
                nextStoreBordereauCode = null;
                if (treatment1 != null)
                {
                    if (treatment1.STORE_BORDEREAU_TIME != null)
                    {
                        dteStoreTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatment1.STORE_BORDEREAU_TIME ?? 0) ?? DateTime.Now;
                    }
                    if (!string.IsNullOrEmpty(treatment1.STORE_BORDEREAU_CODE))
                    {
                        if (HisConfigCFG.OptionStoreBordereauCode == "1")
                            txtStoreBordereauCode.Text = treatment1.STORE_BORDEREAU_CODE;
                        else if (HisConfigCFG.OptionStoreBordereauCode == "2")
                            txtStoreBordereauCodeOption2.Text = treatment1.STORE_BORDEREAU_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        this.listTreatmentImport = import.GetWithCheck<TreatmentImportADO>(0);
                        if (this.listTreatmentImport != null && this.listTreatmentImport.Count > 0)
                        {
                            if (this.listTreatmentImport.Exists(o =>
                                string.IsNullOrEmpty(o.IN_TIME_STR.Trim())
                                && string.IsNullOrEmpty(o.OUT_TIME_STR.Trim())
                                && string.IsNullOrEmpty(o.TDL_HEIN_CARD_NUMBER.Trim())
                                && string.IsNullOrEmpty(o.TDL_PATIENT_CODE.Trim())
                                && string.IsNullOrEmpty(o.TDL_PATIENT_NAME.Trim())
                                && string.IsNullOrEmpty(o.TREATMENT_CODE.Trim())
                                ))
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.DuLieuLocKhongDuocDeTrong);
                                return;
                            }

                            string error = "";
                            List<HisTreatmentView1ImportFilter.TreatmentImportFilter> processImport = ProcessDataImport(this.listTreatmentImport, ref error);

                            if (!string.IsNullOrEmpty(error))
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show(error, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao);
                                return;
                            }
                            else if (processImport == null)
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.CoLoiKhiLayDuLieuLoc, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao);
                                return;
                            }
                            else
                            {
                                CommonParam param = new CommonParam();
                                HisTreatmentView1ImportFilter filter = new HisTreatmentView1ImportFilter();
                                filter.TreatmentImportFilters = processImport;
                                filter.ORDER_DIRECTION = "DESC";
                                filter.ORDER_FIELD = "TREATMENT_CODE";

                                var rsApi = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetByImportView1", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                                gridControlTreatment.BeginUpdate();
                                gridControlTreatment.DataSource = rsApi;
                                gridControlTreatment.EndUpdate();
                                WaitingManager.Hide();

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HisTreatmentView1ImportFilter.TreatmentImportFilter> ProcessDataImport(List<TreatmentImportADO> treatmentImport, ref string error)
        {
            List<HisTreatmentView1ImportFilter.TreatmentImportFilter> result = new List<HisTreatmentView1ImportFilter.TreatmentImportFilter>();
            try
            {
                foreach (var item in treatmentImport)
                {
                    HisTreatmentView1ImportFilter.TreatmentImportFilter filter = new HisTreatmentView1ImportFilter.TreatmentImportFilter();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentView1ImportFilter.TreatmentImportFilter>(filter, item);

                    if (!string.IsNullOrEmpty(item.IN_TIME_STR))
                    {
                        try
                        {
                            var dateTime = Convert.ToDateTime(item.IN_TIME_STR);
                            filter.IN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateTime);
                        }
                        catch (Exception ex)
                        {
                            error += string.Format(Base.ResourceMessageLang.NgayVaoKhongHopLe, item.IN_TIME_STR);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.OUT_TIME_STR))
                    {
                        try
                        {
                            var dateTime = Convert.ToDateTime(item.OUT_TIME_STR);
                            filter.OUT_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateTime);
                        }
                        catch (Exception ex)
                        {
                            error += string.Format(Base.ResourceMessageLang.NgayRaKhongHopLe, item.OUT_TIME_STR);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TDL_PATIENT_CODE))
                    {
                        if (item.TDL_PATIENT_CODE.Length < 10 && checkDigit(item.TDL_PATIENT_CODE))
                        {
                            filter.TDL_PATIENT_CODE = string.Format("{0:0000000000}", Convert.ToInt64(item.TDL_PATIENT_CODE));
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TREATMENT_CODE))
                    {
                        if (item.TREATMENT_CODE.Length < 12 && checkDigit(item.TREATMENT_CODE))
                        {
                            filter.TREATMENT_CODE = string.Format("{0:000000000000}", Convert.ToInt64(item.TREATMENT_CODE));
                        }
                    }

                    result.Add(filter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            if (result.Count == 0)
                return null;
            return result;
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

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath
                + "/Tmp/Imp", "IMPORT_TREATMENT_XML.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_TREATMENT_XML";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName, true);
                        DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.TaiFileThanhCong);
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.KhongTimThayFileImport);
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.TaiFileThatBai);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnLuuTru()
        {
            try
            {
                if (HisConfigCFG.OptionStoreBordereauCode == "1")
                {
                    if (!btnLuuTru.Enabled) return;
                    BtnLuuTru_Click(null, null);
                }
                else if (HisConfigCFG.OptionStoreBordereauCode == "2")
                {
                    if (!btnLuuTruOption2.Enabled) return;
                    btnLuuTruOption2_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        StoreBordereauCodeSDO nextStoreBordereauCode = null;
        private void GetNextStoreBordereauCode()
        {
            try
            {
                GetStoreBordereauCodeSDO sdoGetBordereau = new GetStoreBordereauCodeSDO();
                if (HisConfigCFG.OptionStoreBordereauCode == "2")
                {
                    sdoGetBordereau.StoreBordereauTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteStoreTime.DateTime) ?? 0;
                }
                else if (HisConfigCFG.OptionStoreBordereauCode == "1")
                    sdoGetBordereau.StoreBordereauTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHeinLockTime.DateTime) ?? 0;
                sdoGetBordereau.TreatmentId = this.currentTreatment.ID;

                nextStoreBordereauCode = new BackendAdapter(new CommonParam()).Get<StoreBordereauCodeSDO>("api/HisTreatment/GetNextStoreBordereauCode", ApiConsumers.MosConsumer, sdoGetBordereau, null);
                if (nextStoreBordereauCode != null)
                {
                    if (HisConfigCFG.OptionStoreBordereauCode == "1")
                        txtStoreBordereauCode.Text = nextStoreBordereauCode.StoreBordereauCode;
                    else if (HisConfigCFG.OptionStoreBordereauCode == "2")
                        txtStoreBordereauCodeOption2.Text = nextStoreBordereauCode.StoreBordereauCode;
                }
                else
                {
                    txtStoreBordereauCode.Text = txtStoreBordereauCodeOption2.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void BtnLuuTru_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam paramStoreBordereauCode = new CommonParam();
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                bool success = false;
                nextStoreBordereauCode = null;
                StoreBordereauCodeSDO sdo = new StoreBordereauCodeSDO();
                sdo.TreatmentId = this.currentTreatment.ID;
                if (!String.IsNullOrWhiteSpace(txtStoreBordereauCode.Text))
                {
                    string code = txtStoreBordereauCode.Text.Trim();
                    if (code.Length < 5)
                    {
                        code = string.Format("{0:00000}", Convert.ToInt64(code));
                        txtStoreBordereauCode.Text = code;
                    }
                    sdo.StoreBordereauCode = code;
                }
                sdo.StoreBordereauTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHeinLockTime.DateTime) ?? 0;
                sdo.StoreBordereauSeedCode = string.Format("{0}_{1}_", this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU ? "1" : "0", this.currentTreatment.HEIN_LOCK_TIME != null ? this.currentTreatment.HEIN_LOCK_TIME.ToString().Substring(0, 8) : "");
                var rs = new BackendAdapter(paramStoreBordereauCode).Post<HIS_TREATMENT>("api/HisTreatment/SetStoreBordereauCode", ApiConsumers.MosConsumer, sdo, paramStoreBordereauCode);
                if (rs != null)
                {
                    success = true;
                    foreach (var item in listTreatment)
                    {
                        if (item.ID == this.currentTreatment.ID)
                        {
                            item.STORE_BORDEREAU_CODE = rs.STORE_BORDEREAU_CODE;
                            break;
                        }
                    }
                    gridControlTreatment.BeginUpdate();
                    gridControlTreatment.DataSource = listTreatment;
                    gridControlTreatment.EndUpdate();
                    btnLuuTru.Enabled = false;
                }
                else
                {
                    GetNextStoreBordereauCode();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, paramStoreBordereauCode, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationControlTxtStoreBordereauCode()
        {
            try
            {
                ValidationControlTextEditStoreBordereauCode validRule = new ValidationControlTextEditStoreBordereauCode();
                validRule.txtStoreBordereauCode = txtStoreBordereauCode;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtStoreBordereauCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtStoreBordereauCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timer1_Tick()
        {
            try
            {
                if (dtHeinLockTime.SelectionStart > 0)
                {
                    StopTimer(currentModule.ModuleLink, "timer1");
                }
                else
                {
                    currentTimer = currentTimer.AddSeconds(1);
                    dtHeinLockTime.DateTime = currentTimer;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtHeinLockTime_Leave(object sender, EventArgs e)
        {
            try
            {
                nextStoreBordereauCode = null;
                if (currentTreatment != null)
                {
                    if (!string.IsNullOrEmpty(currentTreatment.STORE_BORDEREAU_CODE))
                    {
                        if (HisConfigCFG.OptionStoreBordereauCode == "1")
                            txtStoreBordereauCode.Text = currentTreatment.STORE_BORDEREAU_CODE;
                        else if (HisConfigCFG.OptionStoreBordereauCode == "2")
                            txtStoreBordereauCodeOption2.Text = currentTreatment.STORE_BORDEREAU_CODE;
                    }
                    else
                    {
                        GetNextStoreBordereauCode();
                    }
                }
                currentTimer = dtHeinLockTime.DateTime;
                dtHeinLockTime.SelectionStart = 0;
                StartTimer(currentModule.ModuleLink, "timer1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtHeinLockTime_Click(object sender, EventArgs e)
        {
            try
            {
                StopTimer(currentModule.ModuleLink, "timer1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLuuTruOption2_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam paramStoreBordereauCode = new CommonParam();
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                bool success = false;
                StoreBordereauCodeSDO sdo = new StoreBordereauCodeSDO();
                sdo.TreatmentId = this.currentTreatment.ID;
                if (!btnLuuTruOption2.Text.Equals("Hủy LT (F5)"))
                {
                    if (!String.IsNullOrWhiteSpace(txtStoreBordereauCodeOption2.Text))
                    {
                        string code = txtStoreBordereauCodeOption2.Text.Trim();
                        if (code.Length < 5)
                        {
                            code = string.Format("{0:00000}", Convert.ToInt64(code));
                            txtStoreBordereauCodeOption2.Text = code;
                        }
                        sdo.StoreBordereauCode = code;
                    }
                    sdo.StoreBordereauTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteStoreTime.DateTime) ?? 0;
                    sdo.StoreBordereauSeedCode = nextStoreBordereauCode != null && !string.IsNullOrEmpty(nextStoreBordereauCode.StoreBordereauSeedCode) ? nextStoreBordereauCode.StoreBordereauSeedCode : null;
                }
                var rs = new BackendAdapter(paramStoreBordereauCode).Post<HIS_TREATMENT>("api/HisTreatment/SetStoreBordereauCode", ApiConsumers.MosConsumer, sdo, paramStoreBordereauCode);
                if (rs != null)
                {
                    success = true;
                    foreach (var item in listTreatment)
                    {
                        if (item.ID == this.currentTreatment.ID)
                        {
                            item.STORE_BORDEREAU_CODE = rs.STORE_BORDEREAU_CODE;
                            item.STORE_BORDEREAU_TIME = rs.STORE_BORDEREAU_TIME;
                            break;
                        }
                    }
                    gridControlTreatment.BeginUpdate();
                    gridControlTreatment.DataSource = listTreatment;
                    gridControlTreatment.EndUpdate();
                    nextStoreBordereauCode = null;
                    dteStoreTime.DateTime = DateTime.Now;
                    if (currentTreatment.STORE_BORDEREAU_TIME != null)
                    {
                        dteStoreTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatment.STORE_BORDEREAU_TIME ?? 0) ?? DateTime.Now;
                    }
                    if (btnLuuTruOption2.Text.Equals("Hủy LT (F5)"))
                    {
                        GetNextStoreBordereauCode();
                    }
                    ChangeEnableButtonOption2();
                }
                else
                {
                    GetNextStoreBordereauCode();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, paramStoreBordereauCode, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStorebcode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    FillDataToGridTreatment();
                    if (listTreatment != null && listTreatment.Count == 1)
                    {
                        this.currentTreatment = listTreatment.First();
                        ClickTreatment(currentTreatment);
                        FillDataToGridHeinCardAndHeinApproval();
                    }
                    gridControlTreatment.Focus();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dteStoreTime_Leave(object sender, EventArgs e)
        {

            try
            {
                if (dteStoreTime.EditValue != dteStoreTime.OldEditValue)
                {
                    if (string.IsNullOrEmpty(this.currentTreatment.STORE_BORDEREAU_CODE))
                        GetNextStoreBordereauCode();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
