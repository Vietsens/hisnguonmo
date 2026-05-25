using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.InviteConsultation.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InviteConsultation.InviteConsultation
{
    public partial class frmInviteConsultation : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        L_HIS_TREATMENT_BED_ROOM bedRoom;
        HIS_SPECIALIST_EXAM specialistExam;
        V_HIS_SERVICE_REQ serviceReq;
        bool isEditMode = false;
        string DoctorLogin { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        List<HIS_EMPLOYEE> lstEmployee { get; set; }
        List<HIS_EMPLOYEE> lstEmployee2 { get; set; }

        List<HIS_ICD> lstICD { get; set; }
        IcdProcessor icdProcessor;
        UserControl ucIcd;
        SecondaryIcdProcessor subIcdProcessor;
        UserControl ucSecondaryIcd;
        List<HIS_DEPARTMENT> lstDepartment { get; set; }
        List<HIS_DEPARTMENT> lstSelectedDepartments = new List<HIS_DEPARTMENT>();
        List<EmployeeADO> lstAllDoctors = new List<EmployeeADO>();
        internal IcdProcessor inIcdProcessor;
        internal UserControl ucInIcd;

        internal SecondaryIcdProcessor subInIcdProcessor;
        internal UserControl ucSecondaryInIcd;
        string AutoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("HIS.Desktop.Plugins.AutoCheckIcd");
        long id;
        public frmInviteConsultation(Inventec.Desktop.Common.Modules.Module module, L_HIS_TREATMENT_BED_ROOM lBedRoom, HIS_SPECIALIST_EXAM hisExam, bool isEdit) : base(module)
        {
            try
            {
                InitializeComponent();
                this.moduleData = module;
                this.bedRoom = lBedRoom;
                this.specialistExam = hisExam;
                this.isEditMode = isEdit;
                dteNgayMoi.DateTime = DateTime.Now;
                lstICD = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().Where(i => i.IS_ACTIVE == 1 && i.IS_TRADITIONAL != 1).OrderBy(o => o.ICD_CODE).ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public frmInviteConsultation(Inventec.Desktop.Common.Modules.Module module, V_HIS_SERVICE_REQ serviceReqData) : base(module)
        {
            try
            {
                InitializeComponent();
                this.moduleData = module;
                this.serviceReq = serviceReqData;
                this.isEditMode = false;
                dteNgayMoi.DateTime = DateTime.Now;
                lstICD = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().Where(i => i.IS_ACTIVE == 1 && i.IS_TRADITIONAL != 1).OrderBy(o => o.ICD_CODE).ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmInviteConsultation_Load(object sender, EventArgs e)
        {
            try
            {
                LoadcboEmployee();
                LoadcboDepartment();
                //LoadComboICD();
                InitUcIcd();
                InitUcSecondaryIcd();
                LoadData();
                ValidationControl();
                btnThem.Enabled = !isEditMode;
                btnSua.Enabled = isEditMode;
                btnLamLai.Enabled = !isEditMode;
                cboBacSiKham.EditValue = null;
                // Hook Popup event to suppress filter UI every time popup opens (after Mark column is created)
                cboBacSiKham.Popup += CheckPicker_Popup;
                cboPhongKham.Popup += CheckPicker_Popup;
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.IsUCCause = false;
                ado.Width = 440;
                ado.Height = 24;
                //Check "Không nhập ICD" (IS_ALLOW_NO_ICD trong HIS_ROOM bằng 1)
                CommonParam paramCommon = new CommonParam();
                HisRoomFilter filter = new HisRoomFilter();
                filter.ID = moduleData.RoomId;
                var resultData = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_ROOM>>("api/HisRoom/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                ado.DataIcds = lstICD;
                ado.AutoCheckIcd = AutoCheckIcd == "1";
                ucIcd = (UserControl)icdProcessor.Run(ado);

                if (ucIcd != null)
                {
                    this.panelIcd.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), lstICD);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.Width = 440;
                ado.Height = 24;
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                if (this.inIcdProcessor != null && this.ucInIcd != null)
                {
                    var icdValue = this.inIcdProcessor.GetValue(this.ucInIcd);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private void LoadData()
        {
            try
            {
                dteNgayMoi.DateTime = DateTime.Now;
                cboDepartment.EditValue = bedRoom != null ? bedRoom.LAST_DEPARTMENT_ID : null;
                var dataDp = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(d => d.ID == moduleData.RoomId);
                var USER = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (USER != null && lstEmployee2 != null)
                {
                    var selectedInviteDoctor = lstEmployee2.FirstOrDefault(o => o.LOGINNAME == USER);
                    if (selectedInviteDoctor != null)
                    {
                        cboBacSiMoi.EditValue = selectedInviteDoctor.ID;
                    }

                }
                if (bedRoom != null)
                {
                    HIS.UC.Icd.ADO.IcdInputADO ado = new HIS.UC.Icd.ADO.IcdInputADO
                    {
                        ICD_CODE = bedRoom.ICD_CODE,
                        ICD_NAME = bedRoom.ICD_NAME
                    };
                    ((UCIcd)this.ucIcd).Reload(ado);

                    HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subAdo = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO
                    {
                        ICD_SUB_CODE = bedRoom.ICD_SUB_CODE,
                        ICD_TEXT = bedRoom.ICD_TEXT
                    };
                    subIcdProcessor.Reload(ucSecondaryIcd, subAdo);
                }
                else if (serviceReq != null)
                {
                    ProcessSelectPhongKham(serviceReq.EXECUTE_DEPARTMENT_ID);

                    HIS.UC.Icd.ADO.IcdInputADO ado = new HIS.UC.Icd.ADO.IcdInputADO
                    {
                        ICD_CODE = serviceReq.ICD_CODE,
                        ICD_NAME = serviceReq.ICD_NAME
                    };
                    ((UCIcd)this.ucIcd).Reload(ado);

                    HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subAdo = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO
                    {
                        ICD_SUB_CODE = serviceReq.ICD_SUB_CODE,
                        ICD_TEXT = serviceReq.ICD_TEXT
                    };
                    subIcdProcessor.Reload(ucSecondaryIcd, subAdo);

                    chkExamInBed.Checked = false;
                    chkExamInBed.Enabled = false;
                }
                else if (specialistExam != null)
                {
                    if (specialistExam.INVITE_TIME.HasValue)
                    {
                        dteNgayMoi.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)specialistExam.INVITE_TIME);
                    }

                    HIS.UC.Icd.ADO.IcdInputADO ado = new HIS.UC.Icd.ADO.IcdInputADO
                    {
                        ICD_CODE = specialistExam.ICD_CODE,
                        ICD_NAME = specialistExam.ICD_NAME
                    };
                    ((UCIcd)this.ucIcd).Reload(ado);

                    HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subAdo = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO
                    {
                        ICD_SUB_CODE = specialistExam.ICD_SUB_CODE,
                        ICD_TEXT = specialistExam.ICD_TEXT
                    };
                    subIcdProcessor.Reload(ucSecondaryIcd, subAdo);

                    ProcessSelectPhongKham(specialistExam.EXAM_EXECUTE_DEPARMENT_ID);
                    cboPhongKham.Enabled = false;
                    memContent.Text = specialistExam.INVITE_CONTENT;
                    chkExamInBed.Checked = specialistExam.IS__EXAM_BED == 1;

                    if (!string.IsNullOrEmpty(specialistExam.EXAM_EXECUTE_LOGINNAME))
                    {
                        GridCheckMarksSelection gridCheckChiSo = cboBacSiKham.Properties.Tag as GridCheckMarksSelection;
                        ProcessSelectBS(specialistExam.EXAM_EXECUTE_LOGINNAME, gridCheckChiSo);
                    }

                    if (lstEmployee2 != null && !string.IsNullOrEmpty(specialistExam.INVITE_DOCTOR_LOGINNAME))
                    {
                        cboBacSiMoi.EditValue = lstEmployee2.FirstOrDefault(o => o.LOGINNAME == specialistExam.INVITE_DOCTOR_LOGINNAME)?.ID;
                    }
                    cboDepartment.EditValue = specialistExam.INVITE_DEPARMENT_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private async Task LoadcboEmployee()
        {
            try
            {
                var deptDict = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToDictionary(o => o.ID);

                lstEmployee2 = BackendDataWorker.Get<HIS_EMPLOYEE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DOCTOR == 1)
                    .ToList();

                lstAllDoctors = new List<EmployeeADO>();
                foreach (var item in lstEmployee2)
                {
                    EmployeeADO Emp = new EmployeeADO();
                    Emp.ID = item.ID;
                    Emp.LOGINNAME = item.LOGINNAME;
                    Emp.TDL_USERNAME = item.TDL_USERNAME;
                    Emp.DEPARTMENT_ID = item.DEPARTMENT_ID;
                    Emp.EMPLOYEE_NAME_UNSIGN = convertToUnSign3(item.LOGINNAME);
                    HIS_DEPARTMENT dept;
                    if (item.DEPARTMENT_ID.HasValue && deptDict.TryGetValue(item.DEPARTMENT_ID.Value, out dept))
                    {
                        Emp.DEPARTMENT_NAME = dept.DEPARTMENT_NAME;
                    }
                    lstAllDoctors.Add(Emp);
                }

                DataToCombocboEmployee(cboBacSiMoi, lstAllDoctors);

                InitComboEmployee(new List<EmployeeADO>());
                InitComboEmployeeCheck();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DataToCombocboEmployee(Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo, List<EmployeeADO> listADO)
        {
            try
            {
                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "TDL_USERNAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(300, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("TDL_USERNAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("EMPLOYEE_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["EMPLOYEE_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboEmployee(List<EmployeeADO> listADO)
        {
            try
            {
                cboBacSiKham.Properties.DataSource = listADO;
                cboBacSiKham.Properties.DisplayMember = "TDL_USERNAME";
                cboBacSiKham.Properties.ValueMember = "ID";
                cboBacSiKham.Properties.NullText = "";
                cboBacSiKham.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                cboBacSiKham.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cboBacSiKham.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
                // Disable header filter button + auto-filter row to avoid Mark column being filtered (causes lost checks)
                cboBacSiKham.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.Default;
                cboBacSiKham.Properties.View.OptionsView.ShowAutoFilterRow = false;
                cboBacSiKham.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
                cboBacSiKham.Properties.View.OptionsView.ShowDetailButtons = false;
                cboBacSiKham.Properties.View.OptionsView.ShowGroupPanel = false;
                cboBacSiKham.Properties.View.OptionsView.ShowIndicator = false;
                cboBacSiKham.Properties.View.OptionsCustomization.AllowFilter = false;

                DevExpress.XtraGrid.Columns.GridColumn column = cboBacSiKham.Properties.View.Columns.AddField("LOGINNAME");
                column.Caption = "Mã";
                column.Visible = true;
                column.VisibleIndex = 1;
                column.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn columnCode = cboBacSiKham.Properties.View.Columns.AddField("TDL_USERNAME");
                columnCode.Caption = "Tên";
                columnCode.Visible = true;
                columnCode.VisibleIndex = 2;
                columnCode.Width = 100;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cboBacSiKham.Properties.View.Columns.AddField("EMPLOYEE_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cboBacSiKham.Properties.View.Columns["EMPLOYEE_NAME_UNSIGN"].Width = 0;

                DevExpress.XtraGrid.Columns.GridColumn columnDept = cboBacSiKham.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                columnDept.Caption = "Khoa";
                columnDept.Visible = false;
                columnDept.GroupIndex = 0;
                columnDept.Width = 150;

                cboBacSiKham.Properties.View.OptionsBehavior.AutoExpandAllGroups = true;
                cboBacSiKham.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboBacSiKham.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboEmployeeCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboBacSiKham.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboBacSiKham.Properties.Tag = gridCheck;
                cboBacSiKham.Properties.View.OptionsSelection.MultiSelect = true;
                // Disable filter on the Mark column added by GridCheckMarksSelection
                DisableFilterOnMarkColumn(cboBacSiKham.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView);
                GridCheckMarksSelection gridCheckMark = cboBacSiKham.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBacSiKham.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Disable filtering on the "Mark" column that GridCheckMarksSelection adds.
        /// Without this, clicking the auto-filter row or header filter on Mark column applies
        /// a filter like [Mark] = 'Unchecked' which hides selected rows and breaks save logic.
        /// </summary>
        private void DisableFilterOnMarkColumn(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            try
            {
                if (view == null) return;
                var markCol = view.Columns["Mark"];
                if (markCol != null)
                {
                    markCol.OptionsFilter.AllowAutoFilter = false;
                    markCol.OptionsFilter.AllowFilter = false;
                    markCol.OptionsColumn.ShowInCustomizationForm = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Popup handler — fires each time the popup opens.
        /// At this point GridCheckMarksSelection has already created the Mark column,
        /// so we can defensively suppress all filter UI to prevent the [Mark] = 'Checked'/'Unchecked' bug.
        /// </summary>
        private void CheckPicker_Popup(object sender, EventArgs e)
        {
            try
            {
                var cbo = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if (cbo == null) return;
                SuppressFilterUI(cbo.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Aggressively disable all filter mechanisms on a GridView used in a check picker:
        /// - Hide auto-filter row, hide filter panel, clear active filters
        /// - Disable filter on every column (including the Mark column added by GridCheckMarksSelection)
        /// </summary>
        private void SuppressFilterUI(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            try
            {
                if (view == null) return;

                view.OptionsView.ShowAutoFilterRow = false;
                view.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                view.OptionsCustomization.AllowFilter = false;
                view.ActiveFilterEnabled = false;
                view.ClearColumnsFilter();

                foreach (DevExpress.XtraGrid.Columns.GridColumn col in view.Columns)
                {
                    col.OptionsFilter.AllowFilter = false;
                    col.OptionsFilter.AllowAutoFilter = false;
                    col.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task LoadcboDepartment()
        {
            try
            {
                List<DepartmentADO> listADO = new List<DepartmentADO>();
                lstDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                foreach (var item in lstDepartment)
                {
                    DepartmentADO Depa = new DepartmentADO();
                    Depa.ID = item.ID;
                    Depa.DEPARTMENT_CODE = item.DEPARTMENT_CODE;
                    Depa.DEPARTMENT_NAME = item.DEPARTMENT_NAME;
                    Depa.DEPARTMENT_NAME_UNSIGN = convertToUnSign3(item.DEPARTMENT_NAME);
                    listADO.Add(Depa);
                }

                DataToCombocboDepartment(cboDepartment, listADO);
                cboDepartment.Enabled = false;
                DataToCombocboDepartment(cboPhongKham, listADO);
                InitComboPhongKhamCheck();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboPhongKhamCheck()
        {
            try
            {
                cboPhongKham.Properties.View.OptionsSelection.MultiSelect = true;
                // Disable filter to avoid Mark column being filtered (causes lost checks)
                cboPhongKham.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.Default;
                cboPhongKham.Properties.View.OptionsView.ShowAutoFilterRow = false;
                cboPhongKham.Properties.View.OptionsCustomization.AllowFilter = false;

                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboPhongKham.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_CheckPhongKham);
                cboPhongKham.Properties.Tag = gridCheck;
                // Disable filter on the Mark column added by GridCheckMarksSelection
                DisableFilterOnMarkColumn(cboPhongKham.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView);
                GridCheckMarksSelection gridCheckMark = cboPhongKham.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboPhongKham.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Event_CheckPhongKham(object sender, EventArgs e)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                lstSelectedDepartments = new List<HIS_DEPARTMENT>();
                if (gridCheckMark != null)
                {
                    var seenIds = new HashSet<long>();
                    foreach (var row in gridCheckMark.Selection)
                    {
                        // Cast to base type HIS_DEPARTMENT — DepartmentADO inherits from it
                        HIS_DEPARTMENT dep = row as HIS_DEPARTMENT;
                        if (dep != null && seenIds.Add(dep.ID))
                        {
                            lstSelectedDepartments.Add(dep);
                        }
                    }
                }
                cboPhongKham.Text = string.Join(", ", lstSelectedDepartments.Select(d => d.DEPARTMENT_NAME));
                ReloadDoctorsBySelectedDepartments();
                if (lstSelectedDepartments.Count > 0)
                {
                    dxValidationProvider1.SetValidationRule(cboPhongKham, null);
                }
                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => lstSelectedDepartments),
                        lstSelectedDepartments.Select(d => new { d.ID, d.DEPARTMENT_NAME }).ToList()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhongKham_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                if (lstSelectedDepartments != null && lstSelectedDepartments.Count > 0)
                {
                    e.DisplayText = string.Join(", ", lstSelectedDepartments.Select(d => d.DEPARTMENT_NAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadDoctorsBySelectedDepartments()
        {
            try
            {
                List<EmployeeADO> filtered;
                if (lstSelectedDepartments != null && lstSelectedDepartments.Count > 0)
                {
                    HashSet<long> deptIds = new HashSet<long>(lstSelectedDepartments.Select(d => d.ID));
                    filtered = lstAllDoctors
                        .Where(o => o.DEPARTMENT_ID.HasValue && deptIds.Contains(o.DEPARTMENT_ID.Value))
                        .ToList();
                }
                else
                {
                    filtered = new List<EmployeeADO>();
                }
                cboBacSiKham.Properties.DataSource = filtered;

                GridCheckMarksSelection gridCheckMark = cboBacSiKham.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBacSiKham.Properties.View);
                }
                lstEmployee = new List<HIS_EMPLOYEE>();
                cboBacSiKham.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DataToCombocboDepartment(DevExpress.XtraEditors.GridLookUpEdit cbo, List<DepartmentADO> listADO)
        {
            try
            {
                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "DEPARTMENT_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(300, 250);


                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("DEPARTMENT_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("DEPARTMENT_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["DEPARTMENT_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControl()
        {
            try
            {
                HIS.Desktop.Plugins.InviteConsultation.Validation.ValidationGridlookup validRule = new HIS.Desktop.Plugins.InviteConsultation.Validation.ValidationGridlookup();
                validRule.cboKhoa = cboPhongKham;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboPhongKham, validRule);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }      

        

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                SaveProcess(ref param);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                SaveProcess(ref param);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SaveProcess(ref CommonParam param)
        {
            try
            {
                if (!btnSua.Enabled && !btnThem.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;
                if (!isEditMode && (lstSelectedDepartments == null || lstSelectedDepartments.Count == 0))
                {
                    dxErrorProvider1.SetError(cboPhongKham,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc),
                        ErrorType.Warning);
                    return;
                }
                WaitingManager.Show();
                saveData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Build base HIS_SPECIALIST_EXAM (INVITE_TYPE = 2 = hội chẩn) for EDIT mode — preserves existing
        /// record fields via AutoMapper then overwrites with UI form values.
        /// Department-specific fields (EXAM_EXECUTE_DEPARMENT_ID, EXAM_EXECUTE_LOGINNAME) are filled in saveData.
        /// </summary>
        private HIS_SPECIALIST_EXAM BuildBaseSpecialistExam()
        {
            HIS_SPECIALIST_EXAM examData = new HIS_SPECIALIST_EXAM();
            if (specialistExam != null)
            {
                AutoMapper.Mapper.CreateMap<HIS_SPECIALIST_EXAM, HIS_SPECIALIST_EXAM>();
                examData = AutoMapper.Mapper.Map<HIS_SPECIALIST_EXAM>(specialistExam);
            }
            FillSpecialistExamFields(examData);
            return examData;
        }

        /// <summary>
        /// Fill UI form values into a HIS_SPECIALIST_EXAM target (works with both base type and
        /// inherited SDO HisSpecialistExamCreateAutoTrackingFeSDO).
        /// </summary>
        private void FillSpecialistExamFields(HIS_SPECIALIST_EXAM examData)
        {
            examData.INVITE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dteNgayMoi.DateTime.ToString("yyyyMMddHHmmss"));

            if (cboDepartment.EditValue != null)
            {
                examData.INVITE_DEPARMENT_ID = Convert.ToInt64(cboDepartment.EditValue);
            }
            examData.ROOM_ID = moduleData.RoomId;

            var USER = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            if (USER != null && lstEmployee2 != null)
            {
                var selectedInviteDoctor = lstEmployee2.FirstOrDefault(o => o.LOGINNAME == USER);
                if (selectedInviteDoctor != null)
                {
                    examData.INVITE_DOCTOR_LOGINNAME = selectedInviteDoctor.LOGINNAME;
                    examData.INVITE_DOCTOR_USERNAME = selectedInviteDoctor.TDL_USERNAME;
                }
            }

            if (ucIcd != null)
            {
                var icdValue = icdProcessor.GetValue(ucIcd);
                if (icdValue != null && icdValue is HIS.UC.Icd.ADO.IcdInputADO)
                {
                    examData.ICD_CODE = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    examData.ICD_NAME = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                }
            }

            if (ucSecondaryIcd != null)
            {
                var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                if (subIcd != null && subIcd is HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)
                {
                    examData.ICD_SUB_CODE = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                    examData.ICD_TEXT = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_TEXT;
                }
            }

            examData.IS__EXAM_BED = chkExamInBed.Checked ? (short?)1 : null;
            examData.INVITE_CONTENT = memContent.Text;
            examData.INVITE_TYPE = 2; // Mời hội chẩn (vs 1 = khám chuyên khoa)

            if (bedRoom != null)
            {
                examData.TREATMENT_CODE = bedRoom.TREATMENT_CODE;
                examData.PATIENT_CODE = bedRoom.TDL_PATIENT_CODE;
                examData.TDL_PATIENT_NAME = bedRoom.TDL_PATIENT_NAME;
                examData.TDL_PATIENT_DOB = bedRoom.TDL_PATIENT_DOB;
                examData.TDL_PATIENT_GENDER_NAME = bedRoom.TDL_PATIENT_GENDER_NAME;
                examData.TDL_PATIENT_ADDRESS = bedRoom.TDL_PATIENT_ADDRESS;
                examData.TREATMENT_ID = bedRoom.TREATMENT_ID;
                examData.TREATMENT_BED_ROOM_ID = bedRoom.ID;
            }
            else if (serviceReq != null)
            {
                examData.TREATMENT_CODE = serviceReq.TREATMENT_CODE;
                examData.PATIENT_CODE = serviceReq.TDL_PATIENT_CODE;
                examData.TDL_PATIENT_NAME = serviceReq.TDL_PATIENT_NAME;
                examData.TDL_PATIENT_DOB = serviceReq.TDL_PATIENT_DOB;
                examData.TDL_PATIENT_GENDER_NAME = serviceReq.TDL_PATIENT_GENDER_NAME;
                examData.TDL_PATIENT_ADDRESS = serviceReq.TDL_PATIENT_ADDRESS;
                examData.TREATMENT_ID = serviceReq.TREATMENT_ID;
                examData.TREATMENT_BED_ROOM_ID = null;
            }
            else if (specialistExam != null)
            {
                examData.TREATMENT_CODE = specialistExam.TREATMENT_CODE;
                examData.PATIENT_CODE = specialistExam.PATIENT_CODE;
                examData.TDL_PATIENT_NAME = specialistExam.TDL_PATIENT_NAME;
                examData.TDL_PATIENT_DOB = specialistExam.TDL_PATIENT_DOB;
                examData.TDL_PATIENT_GENDER_NAME = specialistExam.TDL_PATIENT_GENDER_NAME;
                examData.TDL_PATIENT_ADDRESS = specialistExam.TDL_PATIENT_ADDRESS;
                examData.TREATMENT_ID = specialistExam.TREATMENT_ID;
                examData.TREATMENT_BED_ROOM_ID = specialistExam.TREATMENT_BED_ROOM_ID;
            }
        }

        /// <summary>
        /// Build "Mời hội chẩn lúc {HH:mm dd/MM/yyyy} Khoa phòng mời hội chẩn: {Khoa A, Khoa B, ...}".
        /// Truyền lên API qua field MedicalInstruction để backend gán vào HIS_TRACKING.MEDICAL_INSTRUCTION.
        /// </summary>
        private string BuildMedicalInstruction()
        {
            string timeStr = dteNgayMoi.DateTime.ToString("HH:mm dd/MM/yyyy");
            string deptNames = string.Join(", ", lstSelectedDepartments.Select(d => d.DEPARTMENT_NAME));
            return string.Format("Mời hội chẩn lúc {0} Khoa phòng mời hội chẩn: {1}", timeStr, deptNames);
        }

        /// <summary>
        /// Frontend tự tạo tờ điều trị A (HIS_TRACKING) cho bản ghi Mời hội chẩn đầu tiên.
        /// Theo PTTK_38078 B.4.1:
        ///   - Thời gian = INVITE_TIME (thời gian mời, giờ phút)
        ///   - Bác sĩ = BS tạo phiếu mời (current user)
        ///   - Diễn biến (CONTENT) = Nội dung mời (INVITE_CONTENT)
        ///   - Y lệnh (MEDICAL_INSTRUCTION) = chuỗi tổng hợp "Mời hội chẩn lúc ... Khoa phòng mời hội chẩn: ..."
        /// Sau khi tạo tracking thành công, update HIS_SPECIALIST_EXAM.TRACKING_ID = newId.
        /// </summary>
        private void CreateTrackingForFirstExam(HIS_SPECIALIST_EXAM firstExam, string medicalInstruction, CommonParam param)
        {
            try
            {
                if (firstExam == null || !firstExam.TREATMENT_ID.HasValue) return;

                HIS_TRACKING tracking = new HIS_TRACKING();
                tracking.TREATMENT_ID = firstExam.TREATMENT_ID.Value;
                tracking.TRACKING_TIME = firstExam.INVITE_TIME ?? 0;
                tracking.CONTENT = firstExam.INVITE_CONTENT;        // Diễn biến = Nội dung mời
                tracking.MEDICAL_INSTRUCTION = medicalInstruction;  // Y lệnh = chuỗi tổng hợp

                // BS tạo phiếu mời: backend tự lấy từ token (không set trên HIS_TRACKING ở client)

                // Khoa của tờ điều trị = khoa điều trị bệnh nhân (cùng cboDepartment đã set)
                if (firstExam.INVITE_DEPARMENT_ID.HasValue)
                {
                    tracking.DEPARTMENT_ID = firstExam.INVITE_DEPARMENT_ID.Value;
                }
                else
                {
                    var workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(moduleData);
                    if (workPlace != null && workPlace.DepartmentId > 0)
                    {
                        tracking.DEPARTMENT_ID = workPlace.DepartmentId;
                    }
                }

                // Copy ICD từ phiếu mời
                tracking.ICD_CODE = firstExam.ICD_CODE;
                tracking.ICD_NAME = firstExam.ICD_NAME;
                tracking.ICD_SUB_CODE = firstExam.ICD_SUB_CODE;
                tracking.ICD_TEXT = firstExam.ICD_TEXT;

                MOS.SDO.HisTrackingSDO trackingSDO = new MOS.SDO.HisTrackingSDO();
                trackingSDO.Tracking = tracking;
                trackingSDO.ServiceReqs = new List<MOS.SDO.TrackingServiceReq>();
                trackingSDO.WorkingRoomId = moduleData.RoomId;

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => trackingSDO), trackingSDO));

                var createdTracking = new BackendAdapter(param).Post<HIS_TRACKING>(
                    HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_TRACKING_CREATE,
                    ApiConsumers.MosConsumer, trackingSDO, param);

                if (createdTracking != null && createdTracking.ID > 0)
                {
                    // Update first HIS_SPECIALIST_EXAM with TRACKING_ID link
                    HIS_SPECIALIST_EXAM examUpdate = new HIS_SPECIALIST_EXAM();
                    AutoMapper.Mapper.CreateMap<HIS_SPECIALIST_EXAM, HIS_SPECIALIST_EXAM>();
                    examUpdate = AutoMapper.Mapper.Map<HIS_SPECIALIST_EXAM>(firstExam);
                    examUpdate.TRACKING_ID = createdTracking.ID;
                    new BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>(
                        RequestUriStore.EXAM_UPDATE, ApiConsumers.MosConsumer, examUpdate, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(
                    "CreateTrackingForFirstExam thất bại."
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => firstExam), firstExam),
                    ex);
            }
        }

        private void saveData()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;

                if (isEditMode)
                {
                    HIS_SPECIALIST_EXAM updateData = BuildBaseSpecialistExam();
                    if (specialistExam != null)
                    {
                        updateData.ID = specialistExam.ID;
                        updateData.EXAM_EXECUTE_DEPARMENT_ID = specialistExam.EXAM_EXECUTE_DEPARMENT_ID;
                    }
                    if (lstEmployee != null && lstEmployee.Count > 0)
                    {
                        updateData.EXAM_EXECUTE_LOGINNAME = string.Join(",", lstEmployee.Select(e => e.LOGINNAME));
                        updateData.EXAM_EXECUTE_USERNAME = string.Join(",", lstEmployee.Select(e => e.TDL_USERNAME));
                    }

                    Inventec.Common.Logging.LogSystem.Debug(
                        Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => updateData), updateData));

                    var rs = new BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>(
                        RequestUriStore.EXAM_UPDATE, ApiConsumers.MosConsumer, updateData, param);
                    if (rs != null)
                    {
                        success = true;
                        specialistExam = rs;
                    }
                }
                else
                {
                    string medicalInstruction = BuildMedicalInstruction();
                    HIS_SPECIALIST_EXAM lastCreated = null;
                    HIS_SPECIALIST_EXAM firstCreated = null;
                    int successCount = 0;

                    for (int i = 0; i < lstSelectedDepartments.Count; i++)
                    {
                        var dept = lstSelectedDepartments[i];

                        // SDO inherits HIS_SPECIALIST_EXAM → fill all fields directly
                        HisSpecialistExamCreateAutoTrackingFeSDO sdo = new HisSpecialistExamCreateAutoTrackingFeSDO();
                        FillSpecialistExamFields(sdo);
                        sdo.EXAM_EXECUTE_DEPARMENT_ID = dept.ID;

                        // Only doctors of this department go into this record
                        var doctorsOfDept = (lstEmployee ?? new List<HIS_EMPLOYEE>())
                            .Where(e => e.DEPARTMENT_ID == dept.ID)
                            .ToList();
                        if (doctorsOfDept.Count > 0)
                        {
                            sdo.EXAM_EXECUTE_LOGINNAME = string.Join(",", doctorsOfDept.Select(e => e.LOGINNAME));
                            sdo.EXAM_EXECUTE_USERNAME = string.Join(",", doctorsOfDept.Select(e => e.TDL_USERNAME));
                        }

                        sdo.IsAutoCreateTracking = (short)(i == 0 ? 1 : 0);
                        sdo.MedicalInstruction = (i == 0) ? medicalInstruction : null;
                        // Set MEDICAL_INSTRUCTION trên TẤT CẢ records — vì backend đang tạo
                        // 1 HIS_TRACKING per HIS_SPECIALIST_EXAM, mỗi tracking cần có chuỗi
                        // để hiển thị đúng trong "Phương pháp xử lý" của tờ điều trị.
                        sdo.MEDICAL_INSTRUCTION = medicalInstruction;

                        Inventec.Common.Logging.LogSystem.Debug(
                            Inventec.Common.Logging.LogUtil.TraceData(
                                Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));

                        var rs = new BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>(
                            RequestUriStore.EXAM_CREATE, ApiConsumers.MosConsumer, sdo, param);
                        if (rs != null)
                        {
                            successCount++;
                            lastCreated = rs;
                            if (i == 0) firstCreated = rs;
                        }
                    }

                    if (successCount == lstSelectedDepartments.Count && lastCreated != null)
                    {
                        success = true;
                        // Frontend tự tạo tờ điều trị A cho bản ghi đầu tiên (per PTTK_38078 B.4.1)
                        // — workaround vì backend chưa hỗ trợ auto-create HIS_TRACKING từ Mời hội chẩn.
                        // Nếu backend đã update theo B.3.1 và auto-tạo, có thể bỏ block này
                        // (firstCreated.TRACKING_ID sẽ có sẵn và skip tạo lần 2).
                        if (firstCreated != null
                            && (!firstCreated.TRACKING_ID.HasValue || firstCreated.TRACKING_ID.Value <= 0))
                        {
                            CreateTrackingForFirstExam(firstCreated, medicalInstruction, param);
                        }

                        // Multi-record save: KHÔNG switch sang edit-1-record mode (sẽ mất N-1 khoa hiển thị).
                        // Giữ nguyên UI: tất cả khoa + BS user đã chọn vẫn hiển thị để user biết đã lưu gì.
                        // Disable nút Thêm để tránh save duplicate. User bấm "Làm lại" để tạo phiếu mời mới,
                        // hoặc đóng form. Muốn sửa từng record → vào "Danh sách duyệt hội chẩn".
                        btnThem.Enabled = false;
                        btnSua.Enabled = false;
                        btnLamLai.Enabled = true;
                    }
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

        private void btnLamLai_Click(object sender, EventArgs e)
        {
            try
            {
                MOS.SDO.WorkPlaceSDO workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace((moduleData));

                dteNgayMoi.DateTime = DateTime.Now;
                cboDepartment.EditValue = bedRoom != null ? bedRoom.LAST_DEPARTMENT_ID : (long?)null;
                ClearPhongKhamSelection();
                ProcessSelectPhongKham(serviceReq != null ? serviceReq.EXECUTE_DEPARTMENT_ID : workPlace.DepartmentId);
                cboBacSiKham.EditValue = null;
                chkExamInBed.Checked = false;
                memContent.Text = string.Empty;
                HIS.UC.Icd.ADO.IcdInputADO ado = new HIS.UC.Icd.ADO.IcdInputADO
                {
                    ICD_CODE = null,
                    ICD_NAME = null
                };
                ((UCIcd)this.ucIcd).Reload(ado);

                HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subAdo = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO
                {
                    ICD_SUB_CODE = null,
                    ICD_TEXT = null
                };
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnThem.Enabled)
                    btnThem_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSua.Enabled)
                    btnSua_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnLamLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnLamLai.Enabled)
                    btnLamLai_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboPhongKham_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (lstSelectedDepartments != null && lstSelectedDepartments.Count > 0)
                {
                    dxValidationProvider1.SetValidationRule(cboPhongKham, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void Event_Check(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                lstEmployee = new List<HIS_EMPLOYEE>();
                if (gridCheckMark != null)
                {
                    var seenIds = new HashSet<long>();
                    foreach (var row in gridCheckMark.Selection)
                    {
                        HIS_EMPLOYEE er = row as HIS_EMPLOYEE;
                        if (er != null && seenIds.Add(er.ID))
                        {
                            if (sb.Length > 0) sb.Append(", ");
                            sb.Append(er.TDL_USERNAME);
                            lstEmployee.Add(er);
                        }
                    }
                }
                this.cboBacSiKham.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBacSiKham_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.lstEmployee != null && this.lstEmployee.Count > 0)
                {
                    foreach (var item in this.lstEmployee)
                    {
                        roomName += item.TDL_USERNAME + ",";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void ClearPhongKhamSelection()
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboPhongKham.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboPhongKham.Properties.View);
                }
                lstSelectedDepartments = new List<HIS_DEPARTMENT>();
                cboPhongKham.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectPhongKham(long? deptId)
        {
            try
            {
                if (!deptId.HasValue) return;
                GridCheckMarksSelection gridCheckMark = cboPhongKham.Properties.Tag as GridCheckMarksSelection;
                List<DepartmentADO> ds = cboPhongKham.Properties.DataSource as List<DepartmentADO>;
                if (gridCheckMark == null || ds == null) return;
                var row = ds.FirstOrDefault(o => o.ID == deptId.Value);
                if (row != null)
                {
                    gridCheckMark.SelectAll(new List<DepartmentADO> { row });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<EmployeeADO> listHisSuimIndexDefault = new List<EmployeeADO>();
        private void ProcessSelectBS(string p, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<EmployeeADO> ds = cboBacSiKham.Properties.DataSource as List<EmployeeADO>;
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<EmployeeADO> selects = new List<EmployeeADO>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.LOGINNAME.ToString() == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                            listHisSuimIndexDefault.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
