using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisHolidayPolicies.Resources;
using HIS.Desktop.Plugins.HisHolidayPolicies.Validation;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.HisHolidayPolicies
{
    public partial class frmHisDayPolicies : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        HIS_HOLIDAY_POLICIES currentModule;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<HIS_PATIENT_TYPE> patientTypes; // Danh sách đối tượng thanh toán
        #endregion

        #region
        public frmHisDayPolicies(Inventec.Desktop.Common.Modules.Module moduleData)
            :base (moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                this.KeyPreview = true;
                this.gridViewPolicies.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewPolicies_FocusedRowChanged);
                try
                {
                    string iconPath = System.IO.Path.Combine(
                        HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                        System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void frmHisDayPolicies_Load(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void Show()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            // Ẩn barManager1 bằng cách đặt visibility của các bar thành Never
            foreach (DevExpress.XtraBars.Bar bar in barManager1.Bars)
            {
                bar.Visible = false;
            }
            LoadGridLookUpEditData();
            FillDataToControl();
            SetDefaultValue();
            SetDefaultFocus();
            EnableControlChanged(this.ActionType);
            InitTabIndex();
            SetResourceByLanguageKey();
            SetToolTips();
            ValidateForm();

            rdoDayOfWeek.Checked = true;
            UpdateComboBoxVisibility();
        }

        private void SetToolTips()
        {
            try
            {
                toolTipController1.SetToolTip(gluDayOfWeek, "Thứ trong tuần");
                toolTipController1.SetToolTip(gluDayOfYear, "Ngày trong năm");
                toolTipController1.SetToolTip(gluMonthOfYear, "Tháng trong năm");
                toolTipController1.SetToolTip(dtDay, "Ngày cụ thể trong năm");
                toolTipController1.SetToolTip(gluPatientType, "Các đối tượng thanh toán cho phép thanh toán ngoài giờ");  
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lỗi khi thiết lập tooltip: ", ex);
            }
        }

        private void SetResourceByLanguageKey()
        {
            try
            {
                // Khởi tạo ResourceManager để load tài nguyên ngôn ngữ
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager(
                    "HIS.Desktop.Plugins.HisHolidayPolicies.Resources.Lang",
                    typeof(HIS.Desktop.Plugins.HisHolidayPolicies.frmHisDayPolicies).Assembly);

                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.layoutControl1.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()); 

                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.bar2.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.bbtnEdit.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()) ?? "Sửa (Ctrl S)";

                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.bbtnAdd.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()) ?? "Thêm (Ctrl N)";

                this.bbtnRefresh.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.bbtnRefresh.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()) ?? "Làm lại (Ctrl R)";
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.bbtnSearch.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.btnAdd.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.btnEdit.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.btnRefresh.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.btnSearch.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.rdoDayOfWeek.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.rdoDayOfWeek.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.rdoDayOfYear.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.rdoDayOfYear.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.rdoHoliday.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.rdoHoliday.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.colSTT.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colSTT.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colDayOfWeek.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colDayOfWeek.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colDayOfYear.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colDayOfYear.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colMonthOfYear.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colMonthOfYear.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colHoliday.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colHoliday.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colPatientType.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colPatientType.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colCreator.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colModifier.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colActive.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colActive.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colCreateTime.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colModifyTime.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colMa.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colMa.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colTen.Caption = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.colTen.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.layoutControlItem3.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()); // Mã:
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.layoutControlItem2.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()); // Tên:
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.layoutControlItem4.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()); // Thứ:
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.layoutControlItem5.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()); // Ngày:
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.layoutControlItem6.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()); // Tháng:
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.layoutControlItem7.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()); // Ngày cụ thể:
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.layoutControlItem8.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()); // Đối tượng thanh toán:
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisDayPolicies.txtSearch.Properties.NullValuePrompt",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                else
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmHisHolidayPolicies.Text",
                        Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lỗi khi thiết lập ngôn ngữ: ", ex);
            }
        }

        private void LoadGridLookUpEditData()
        {
            try
            {
                LoadStaticGridLookUpEditData();
                LoadPatientTypeGridLookUpEditData();
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Lỗi khi tải dữ liệu cho GridLookUpEdit", ex);
            }
        }
         
        private void LoadStaticGridLookUpEditData()
        {
            try
            {
                var dayOfWeekData = new List<string> { "Chủ nhật", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7" };
                gluDayOfWeek.Properties.DataSource = dayOfWeekData;
                gluDayOfWeek.Properties.DisplayMember = "Value";
                gluDayOfWeek.Properties.ValueMember = "Value";
                gluDayOfWeek.Properties.View.OptionsView.ShowColumnHeaders = false;

                var dayOfYearData = Enumerable.Range(1, 31).Select(i => i.ToString()).ToList();
                gluDayOfYear.Properties.DataSource = dayOfYearData;
                gluDayOfYear.Properties.DisplayMember = null;
                gluDayOfYear.Properties.ValueMember = null;

                var monthOfYearData = Enumerable.Range(1, 12).Select(i => i.ToString()).ToList();
                gluMonthOfYear.Properties.DataSource = monthOfYearData;
                gluMonthOfYear.Properties.DisplayMember = null;
                gluMonthOfYear.Properties.ValueMember = null;

                dtDay.EditValue = null; 
                dtDay.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                dtDay.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                dtDay.Properties.Mask.EditMask = "dd/MM/yyyy";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadPatientTypeGridLookUpEditData()
        {
            try
            {
                patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (patientTypes == null || patientTypes.Count == 0)
                {
                    patientTypes = new List<HIS_PATIENT_TYPE>
                    {
                        new HIS_PATIENT_TYPE { ID = 0, PATIENT_TYPE_NAME = "Không có dữ liệu" }
                    };
                    LogSystem.Warn("Không tìm thấy dữ liệu");
                }

                gluPatientType.Properties.DataSource = patientTypes;
                gluPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME"; 
                gluPatientType.Properties.ValueMember = "ID"; 

                gluPatientType.Properties.View.Columns.Clear(); 
                var colPatientTypeName = gluPatientType.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                colPatientTypeName.Caption = "Tên đối tượng thanh toán"; 
                colPatientTypeName.Visible = true; 
                colPatientTypeName.VisibleIndex = 0; 
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Lỗi khi tải dữ liệu đối tượng thanh toán từ BackendData", ex);
                patientTypes = new List<HIS_PATIENT_TYPE>
                {
                    new HIS_PATIENT_TYPE { ID = 0, PATIENT_TYPE_NAME = "Lỗi khi tải dữ liệu" }
                };
                gluPatientType.Properties.DataSource = patientTypes;
                gluPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                gluPatientType.Properties.ValueMember = "ID";

                gluPatientType.Properties.View.Columns.Clear();
                var colPatientTypeName = gluPatientType.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                colPatientTypeName.Caption = "Tên đối tượng thanh toán";
                colPatientTypeName.Visible = true;
                colPatientTypeName.VisibleIndex = 0;
            }
        }

        private void UpdateComboBoxVisibility()
        {
            try
            {
                if (rdoDayOfWeek.Checked)
                {
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; 
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                }
                else if (rdoDayOfYear.Checked)
                {
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; 
                    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; 
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                }
                else if (rdoHoliday.Checked)
                {
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; 
                }
                else
                {
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;  
                }

                    ValidateForm();


                if (!rdoDayOfWeek.Checked) gluDayOfWeek.EditValue = null;
                if (!rdoDayOfYear.Checked)
                {
                    gluDayOfYear.EditValue = null;
                    gluMonthOfYear.EditValue = null;
                }
                if (!rdoHoliday.Checked) dtDay.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lỗi khi cập nhật trạng thái hiển thị của ComboBox: ", ex);
            }
        }

        private void ValidateForm()
        {
            try
            {

                ValidationControlTextEditCode();
                ValidationControlTextEditName();

                ValidationControlGridLookUpEdit(gluDayOfWeek, layoutControlItem4.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always);
                ValidationControlGridLookUpEdit(gluDayOfYear, layoutControlItem5.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always);
                ValidationControlGridLookUpEdit(gluMonthOfYear, layoutControlItem6.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always);
                ValidationControlDateEdit(dtDay, layoutControlItem7.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lỗi khi thiết lập validation: ", ex);
            }
        }

        private void ValidationControlTextEditCode()
        {
            try
            {
                ValidationControlTextEditCode validRule = new ValidationControlTextEditCode();
                validRule.txtMa = txtMa;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtMa, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditName()
        {
            try
            {
                ValidationControlTextEditName validRule = new ValidationControlTextEditName();
                validRule.txtTen = txtTen;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTen, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlGridLookUpEdit(GridLookUpEdit gridLookUpEdit, bool isVisible)
        {
            try
            {
                ValidationControlGridLookUpEdit validRule = new ValidationControlGridLookUpEdit();
                validRule.gridLookUpEdit = gridLookUpEdit;
                validRule.isVisible = isVisible;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(gridLookUpEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlDateEdit(DateEdit dateEdit, bool isVisible)
        {
            try
            {
                ValidationControlDateEdit validRule = new ValidationControlDateEdit();
                validRule.dateEdit = dateEdit;
                validRule.isVisible = isVisible;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dateEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitTabIndex()
        {
            try
            {
                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, layoutControl1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            try
            {
                if (!layoutControlEditor.IsInitialized) return;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null && itemOrderTab.Key.Contains(be.Name))
                            {
                                be.TabIndex = itemOrderTab.Value;
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                txtMa.Text = "";
                txtTen.Text = "";
                txtSearch.Text = "";
                gluDayOfWeek.EditValue = null;
                gluDayOfYear.EditValue = null;
                gluMonthOfYear.EditValue = null;
                dtDay.EditValue = null;
                gluPatientType.EditValue = null;
                rdoDayOfWeek.Checked = true; 
                UpdateComboBoxVisibility();  
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Lỗi khi đặt giá trị mặc định: ", ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtMa.Focus();
                txtMa.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<HIS_HOLIDAY_POLICIES>> apiResult = null;
                HisHolidayPoliciesFilter filter = new HisHolidayPoliciesFilter();
                //SetFilterNavBar(ref filter);
                filter.KEY_WORD = txtSearch.Text.Trim();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridViewPolicies.BeginDataUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_HOLIDAY_POLICIES>>(
                    HisRequestUriStore.HIS_DAY_POLICIES_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<HIS_HOLIDAY_POLICIES>)apiResult.Data;
                    if (data != null)
                    {
                        gridControl1.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewPolicies.EndUpdate();

                SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SaveProcess();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
                SaveProcess();

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
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
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }
                WaitingManager.Show();
                HIS_HOLIDAY_POLICIES updateDTO = new HIS_HOLIDAY_POLICIES();
                if (this.currentModule != null && this.currentModule.ID > 0)
                {
                    LoadCurrent(this.currentModule.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                Inventec.Common.Logging.LogSystem.Error( Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO), updateDTO));

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<HIS_HOLIDAY_POLICIES>(
                        HisRequestUriStore.HIS_DAY_POLICIES_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<HIS_HOLIDAY_POLICIES>(
                        HisRequestUriStore.HIS_DAY_POLICIES_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                    }
                }

                if (success)
                {
                    SetFocusEditor();
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_HOLIDAY_POLICIES currentDTO)
        {
            try
            {
                currentDTO.HOLIDAY_POLICIES_CODE = txtMa.Text.Trim();
                currentDTO.HOLIDAY_POLICIES_NAME = txtTen.Text.Trim();
                currentDTO.PATIENT_TYPE_ID = gluPatientType.EditValue != null ? (long?)gluPatientType.EditValue : null;

                currentDTO.DAY_OF_WEEK = null;
                currentDTO.DAY_OF_YEAR = null;
                currentDTO.HOLIDAY = null;

                if (rdoDayOfWeek.Checked)
                {
                    currentDTO.HOLIDAY_POLICIES_TYPE = 1;
                    currentDTO.DAY_OF_WEEK = gluDayOfWeek.EditValue != null ? (short?)(gluDayOfWeek.Properties.GetIndexByKeyValue(gluDayOfWeek.EditValue) + 1) : null;
                }
                else if (rdoDayOfYear.Checked)
                {
                    currentDTO.HOLIDAY_POLICIES_TYPE = 2;
                    if (gluMonthOfYear.EditValue != null && gluDayOfYear.EditValue != null)
                    {
                        string monthStr = gluMonthOfYear.EditValue.ToString().PadLeft(2, '0');
                        string dayStr = gluDayOfYear.EditValue.ToString().PadLeft(2, '0');
                        string dayOfYearStr = $"{monthStr}{dayStr}";
                        currentDTO.DAY_OF_YEAR = (short?)short.Parse(dayOfYearStr);
                    }
                }
                else if (rdoHoliday.Checked)
                {
                    currentDTO.HOLIDAY_POLICIES_TYPE = 3;
                    if (dtDay.EditValue != null)
                    {
                        DateTime date = Convert.ToDateTime(dtDay.EditValue);
                        currentDTO.HOLIDAY = (int?)(date.Year * 10000 + date.Month * 100 + date.Day);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref HIS_HOLIDAY_POLICIES currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisHolidayPoliciesFilter filter = new HisHolidayPoliciesFilter { ID = currentId };
                currentDTO = new BackendAdapter(param).Get<List<HIS_HOLIDAY_POLICIES>>(
                    HisRequestUriStore.HIS_DAY_POLICIES_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFocusEditor()
        {
            try
            {
                txtMa.Focus();
                txtMa.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!layoutControl1.IsInitialized) return;
                layoutControl1.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl1.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit formatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            formatFrm.ResetText();
                            formatFrm.EditValue = null;
                        }
                    }
                    gluDayOfWeek.EditValue = null;
                    gluDayOfYear.EditValue = null;
                    gluMonthOfYear.EditValue = null;
                    dtDay.EditValue = null;
                    gluPatientType.EditValue = null;
                    rdoDayOfWeek.Checked = true;
                    UpdateComboBoxVisibility();

                    dxValidationProvider1.RemoveControlError(txtMa);
                    dxValidationProvider1.RemoveControlError(txtTen);
                    dxValidationProvider1.RemoveControlError(gluDayOfWeek);
                    dxValidationProvider1.RemoveControlError(gluDayOfYear);
                    dxValidationProvider1.RemoveControlError(gluMonthOfYear);
                    dxValidationProvider1.RemoveControlError(dtDay);
                }
                finally
                {
                    layoutControl1.EndUpdate();
                }
                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoDayOfWeek_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxVisibility();
        }

        private void rdoDayOfYear_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxVisibility();
        }

        private void rdoHoliday_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxVisibility();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            btnEdit.Enabled = false;
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_HOLIDAY_POLICIES)gridViewPolicies.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage
                    (HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                    "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_DAY_POLICIES_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToControl();
                            currentModule = ((List<HIS_HOLIDAY_POLICIES>)gridControl1.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void frmHisDayPolicies_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.S:
                            if (btnEdit.Enabled)
                            {
                                btnEdit_Click(sender, e);
                                e.Handled = true;
                            }
                            break;
                        case Keys.N:
                            if (btnAdd.Enabled)
                            {
                                btnAdd_Click(sender, e);
                                e.Handled = true;
                            }
                            break;
                        case Keys.R:
                            if (btnRefresh.Enabled)
                            {
                                btnRefresh_Click(sender, e);
                                e.Handled = true;
                            }
                            break;
                        case Keys.F:
                            btnSearch_Click(null, null);
                            e.Handled = true;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lỗi khi xử lý phím tắt: ", ex);
            }

        }

        private void gridViewPolicies_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                var rowData = (HIS_HOLIDAY_POLICIES)gridViewPolicies.GetFocusedRow();
                if (rowData != null)
                {
                    currentModule = rowData; 
                    FillDataToEditorControls(rowData); 
                    this.ActionType = GlobalVariables.ActionEdit; 
                    EnableControlChanged(this.ActionType); 
                    btnEdit.Enabled = (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE); 
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1); 
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lỗi khi chọn dòng trong gridViewPolicies: ", ex);
            }
        }

        private void FillDataToEditorControls(HIS_HOLIDAY_POLICIES data)
        {
            try
            {
                if (data != null)
                {
                    txtMa.Text = data.HOLIDAY_POLICIES_CODE;
                    txtTen.Text = data.HOLIDAY_POLICIES_NAME;
                    gluPatientType.EditValue = data.PATIENT_TYPE_ID;

                    if (data.HOLIDAY_POLICIES_TYPE == 1) 
                    {
                        rdoDayOfWeek.Checked = true;
                        if (data.DAY_OF_WEEK.HasValue && gluDayOfWeek.Properties.DataSource is List<string> dayOfWeekData)
                        {
                            int index = (int)data.DAY_OF_WEEK - 1; 
                            if (index >= 0 && index < dayOfWeekData.Count)
                            {
                                gluDayOfWeek.EditValue = dayOfWeekData[index];
                            }
                            else
                            {
                                gluDayOfWeek.EditValue = null;
                            }
                        }
                        else
                        {
                            gluDayOfWeek.EditValue = null;
                        }
                    }
                    else if (data.HOLIDAY_POLICIES_TYPE == 2) 
                    {
                        rdoDayOfYear.Checked = true;
                        if (data.DAY_OF_YEAR.HasValue)
                        {
                            string dayOfYearStr = data.DAY_OF_YEAR.Value.ToString("D4"); 
                            int month = int.Parse(dayOfYearStr.Substring(0, 2));
                            int day = int.Parse(dayOfYearStr.Substring(2, 2));
                            gluMonthOfYear.EditValue = month.ToString(); 
                            gluDayOfYear.EditValue = day.ToString(); 
                        }
                    }
                    else if (data.HOLIDAY_POLICIES_TYPE == 3) 
                    {
                        rdoHoliday.Checked = true;
                        if (data.HOLIDAY.HasValue)
                        {
                            int holidayValue = data.HOLIDAY.Value;
                            int year = holidayValue / 10000;
                            int month = (holidayValue % 10000) / 100;
                            int day = holidayValue % 100;
                            dtDay.EditValue = new DateTime(year, month, day);
                        }
                    }
                    UpdateComboBoxVisibility();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lỗi khi điền dữ liệu vào các control: ", ex);
            }
        }

        private void gridViewPolicies_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_HOLIDAY_POLICIES pData = (HIS_HOLIDAY_POLICIES)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "DAY_WEEK")
                    {
                        if (pData.DAY_OF_WEEK.HasValue)
                        {
                            string dayOfWeekStr;
                            switch (pData.DAY_OF_WEEK.Value)
                            {
                                case 1:
                                    dayOfWeekStr = "Chủ nhật";
                                    break;
                                case 2:
                                    dayOfWeekStr = "Thứ 2";
                                    break;
                                case 3:
                                    dayOfWeekStr = "Thứ 3";
                                    break;
                                case 4:
                                    dayOfWeekStr = "Thứ 4";
                                    break;
                                case 5:
                                    dayOfWeekStr = "Thứ 5";
                                    break;
                                case 6:
                                    dayOfWeekStr = "Thứ 6";
                                    break;
                                case 7:
                                    dayOfWeekStr = "Thứ 7";
                                    break;
                                default:
                                    dayOfWeekStr = "Không xác định";
                                    break;
                            }
                            e.Value = dayOfWeekStr;
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                    else if (e.Column.FieldName == "DAY_YEAR" && pData.HOLIDAY_POLICIES_TYPE == 2)
                    {
                        if (pData.DAY_OF_YEAR.HasValue)
                        {
                            string dayOfYearStr = pData.DAY_OF_YEAR.Value.ToString("D4");
                            string day = dayOfYearStr.Substring(2, 2);
                            string month = dayOfYearStr.Substring(0, 2);
                            e.Value = $"{day}"; 
                        }
                    }
                    else if (e.Column.FieldName == "MONTH_OF_YEAR" && pData.HOLIDAY_POLICIES_TYPE == 2)
                    {
                        if (pData.DAY_OF_YEAR.HasValue)
                        {
                            string dayOfYearStr = pData.DAY_OF_YEAR.Value.ToString("D4");
                            e.Value = dayOfYearStr.Substring(0, 2); 
                        }
                    }
                    else if (e.Column.FieldName == "HOLIDAY_STR" && pData.HOLIDAY_POLICIES_TYPE == 3)
                    {
                        if (pData.HOLIDAY.HasValue)
                        {
                            int holidayValue = pData.HOLIDAY.Value;
                            int year = holidayValue / 10000;
                            int month = (holidayValue % 10000) / 100;
                            int day = holidayValue % 100;
                            e.Value = new DateTime(year, month, day).ToString("dd/MM/yyyy");
                        }
                    }
                    else if (e.Column.FieldName == "PATIENT_TYPE_NAME")
                    {
                        if (pData.PATIENT_TYPE_ID.HasValue)
                        {
                            var patientType = patientTypes?.FirstOrDefault(pt => pt.ID == pData.PATIENT_TYPE_ID.Value);
                            if (patientType != null)
                            {
                                e.Value = patientType.PATIENT_TYPE_NAME;
                            }
                            else
                            {
                                LogSystem.Warn($"Không tìm thấy PATIENT_TYPE_ID = {pData.PATIENT_TYPE_ID} trong danh sách patientTypes.");
                            }
                        }
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == 1)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }    
                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool notHandler = false;
            try
            {
                HIS_HOLIDAY_POLICIES data = (HIS_HOLIDAY_POLICIES)gridViewPolicies.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage
                    (HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_HOLIDAY_POLICIES>(HisRequestUriStore.HIS_DAY_POLICIES_CHANGE_LOCK,
                        ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (rs != null)
                    {
                        notHandler = true;
                        FillDataToControl();
                    }
                    
                    MessageManager.Show(this, param, notHandler);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool notHandler = false;
            try
            { 
                HIS_HOLIDAY_POLICIES data = (HIS_HOLIDAY_POLICIES)gridViewPolicies.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage
                    (HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_HOLIDAY_POLICIES data1 = new HIS_HOLIDAY_POLICIES();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_HOLIDAY_POLICIES>(HisRequestUriStore.HIS_DAY_POLICIES_CHANGE_LOCK,
                        ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (rs != null)
                    {
                        notHandler = true;
                        FillDataToControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPolicies_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_HOLIDAY_POLICIES data = (HIS_HOLIDAY_POLICIES)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnLock : btnUnlock);
                        gridViewPolicies.RefreshRowCell(e.RowHandle, e.Column);
                    }    
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDelete : btnEnable); 
                    }    
                }    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPolicies_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                HIS_HOLIDAY_POLICIES data = (HIS_HOLIDAY_POLICIES)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewPolicies.Focus();
                    gridViewPolicies.FocusedRowHandle = 0;
                    var rowData = (HIS_HOLIDAY_POLICIES)gridViewPolicies.GetFocusedRow();
                    if (rowData != null)
                    {
                        currentModule = rowData;
                        FillDataToEditorControls(rowData);
                        this.ActionType = GlobalVariables.ActionEdit;
                        EnableControlChanged(this.ActionType);
                        btnEdit.Enabled = (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                        positionHandle = -1;
                        Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewPolicies.Focus();
                    gridViewPolicies.FocusedRowHandle = 0;
                    var rowData = (HIS_HOLIDAY_POLICIES)gridViewPolicies.GetFocusedRow();
                    if (rowData != null)
                    {
                        currentModule = rowData;
                        FillDataToEditorControls(rowData);
                        this.ActionType = GlobalVariables.ActionEdit;
                        EnableControlChanged(this.ActionType);
                        btnEdit.Enabled = (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                        positionHandle = -1;
                        Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
