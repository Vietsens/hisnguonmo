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
                    cboBacSiMoi.EditValue = selectedInviteDoctor.ID;
                }
                cboPhongKham.EditValue = dataDp.DEPARTMENT_ID;
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
                else if (specialistExam != null)
                {
                    dteNgayMoi.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)specialistExam.INVITE_TIME);
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
                    cboPhongKham.EditValue = specialistExam.EXAM_EXECUTE_DEPARMENT_ID;
                    memContent.Text = specialistExam.INVITE_CONTENT;
                    chkExamInBed.Checked = specialistExam.IS__EXAM_BED == 1 ? true : false;
                    //cboBacSiKham.EditValue = lstEmployee.FirstOrDefault(o => o.LOGINNAME == specialistExam.EXAM_EXECUTE_LOGINNAME)?.ID;


                    GridCheckMarksSelection gridCheckChiSo = cboBacSiKham.Properties.Tag as GridCheckMarksSelection;
                    if (specialistExam.EXAM_EXECUTE_LOGINNAME != null)
                    {
                        ProcessSelectBS(specialistExam.EXAM_EXECUTE_LOGINNAME, gridCheckChiSo);
                    }
                    cboBacSiMoi.EditValue = lstEmployee.FirstOrDefault(o => o.LOGINNAME == specialistExam.INVITE_DOCTOR_LOGINNAME)?.ID;
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
                List<EmployeeADO> listADO = new List<EmployeeADO>();
                Action myaction = () => {
                    lstEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DOCTOR == 1).ToList();
                    foreach (var item in lstEmployee)
                    {
                        EmployeeADO Emp = new EmployeeADO();
                        Emp.ID = item.ID;
                        Emp.LOGINNAME = item.LOGINNAME;
                        Emp.TDL_USERNAME = item.TDL_USERNAME;
                        Emp.EMPLOYEE_NAME_UNSIGN = convertToUnSign3(item.LOGINNAME);
                        listADO.Add(Emp);
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                lstEmployee2 = BackendDataWorker.Get<HIS_EMPLOYEE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DOCTOR == 1)
                    .ToList();

                DataToCombocboEmployee(cboBacSiMoi, listADO);

                InitComboEmployee(listADO);
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

                //column.Caption = "Tất cả";
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
        private async Task LoadcboDepartment()
        {
            try
            {
                List<DepartmentADO> listADO = new List<DepartmentADO>();
                Action myaction = () => {
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
                };
                Task task = new Task(myaction);
                task.Start();

                DataToCombocboDepartment(cboDepartment, listADO);
                cboDepartment.Enabled = false;
                DataToCombocboDepartment(cboPhongKham, listADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DataToCombocboDepartment(Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo, List<DepartmentADO> listADO)
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
                WaitingManager.Show();
                HIS_SPECIALIST_EXAM hIS_SPECIALIST_EXAM = new HIS_SPECIALIST_EXAM();
                saveData(hIS_SPECIALIST_EXAM);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void saveData(HIS_SPECIALIST_EXAM hIS_SPECIALIST_EXAM)
        {
            try
            {
                bool success = false;
                string codeCheckCD = "";
                string nameCheckCD = "";
                MOS.SDO.WorkPlaceSDO workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace((moduleData));
                if (specialistExam != null)
                {
                    AutoMapper.Mapper.CreateMap<HIS_SPECIALIST_EXAM, HIS_SPECIALIST_EXAM>();
                    hIS_SPECIALIST_EXAM = AutoMapper.Mapper.Map<HIS_SPECIALIST_EXAM>(specialistExam);
                }
                hIS_SPECIALIST_EXAM.INVITE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dteNgayMoi.DateTime.ToString("yyyyMMddHHmmss"));

                if (cboDepartment.EditValue != null)
                {
                    hIS_SPECIALIST_EXAM.INVITE_DEPARMENT_ID = Convert.ToInt64(cboDepartment.EditValue);
                }
                hIS_SPECIALIST_EXAM.ROOM_ID = moduleData.RoomId;
                if (cboPhongKham.EditValue != null)
                {
                    var selectedDepartment = cboPhongKham.EditValue.ToString();
                    var selectedDepartments = lstDepartment.FirstOrDefault(o => o.ID.ToString() == selectedDepartment);
                    if (selectedDepartments != null)
                    {
                        hIS_SPECIALIST_EXAM.EXAM_EXECUTE_DEPARMENT_ID = selectedDepartments.ID;
                    }
                }
                var USER = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (USER != null && lstEmployee2 != null)
                {
                    var selectedInviteDoctor = lstEmployee2.FirstOrDefault(o => o.LOGINNAME == USER);

                    if (selectedInviteDoctor != null)
                    {
                        hIS_SPECIALIST_EXAM.INVITE_DOCTOR_LOGINNAME = selectedInviteDoctor.LOGINNAME;
                        hIS_SPECIALIST_EXAM.INVITE_DOCTOR_USERNAME = selectedInviteDoctor.TDL_USERNAME;
                    }
                }
                if (lstEmployee != null && lstEmployee.Count > 0)
                {
                    hIS_SPECIALIST_EXAM.EXAM_EXECUTE_LOGINNAME =
                        string.Join(",", lstEmployee.Select(e => e.LOGINNAME));

                    hIS_SPECIALIST_EXAM.EXAM_EXECUTE_USERNAME =
                        string.Join(",", lstEmployee.Select(e => e.TDL_USERNAME));
                }

                if (ucIcd != null)
                {
                    var icdValue = icdProcessor.GetValue(ucIcd);
                    if (icdValue != null && icdValue is HIS.UC.Icd.ADO.IcdInputADO)
                    {
                        hIS_SPECIALIST_EXAM.ICD_CODE = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        hIS_SPECIALIST_EXAM.ICD_NAME = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                        codeCheckCD = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        nameCheckCD = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }

                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)
                    {
                        hIS_SPECIALIST_EXAM.ICD_SUB_CODE = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        hIS_SPECIALIST_EXAM.ICD_TEXT = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_TEXT;
                        codeCheckCD += ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        nameCheckCD += ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }

                if (chkExamInBed.Checked)
                    hIS_SPECIALIST_EXAM.IS__EXAM_BED = 1;
                else
                    hIS_SPECIALIST_EXAM.IS__EXAM_BED = null;

                hIS_SPECIALIST_EXAM.INVITE_CONTENT = memContent.Text;
                hIS_SPECIALIST_EXAM.INVITE_TYPE = 2;
                if (bedRoom != null)
                {
                    hIS_SPECIALIST_EXAM.TREATMENT_CODE = bedRoom.TREATMENT_CODE;
                    hIS_SPECIALIST_EXAM.PATIENT_CODE = bedRoom.TDL_PATIENT_CODE;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_NAME = bedRoom.TDL_PATIENT_NAME;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_DOB = bedRoom.TDL_PATIENT_DOB;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_GENDER_NAME = bedRoom.TDL_PATIENT_GENDER_NAME;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_ADDRESS = bedRoom.TDL_PATIENT_ADDRESS;
                    hIS_SPECIALIST_EXAM.TREATMENT_ID = bedRoom.TREATMENT_ID;
                    hIS_SPECIALIST_EXAM.TREATMENT_BED_ROOM_ID = bedRoom.ID;
                }
                else if (specialistExam != null)
                {
                    hIS_SPECIALIST_EXAM.TREATMENT_CODE = specialistExam.TREATMENT_CODE;
                    hIS_SPECIALIST_EXAM.PATIENT_CODE = specialistExam.PATIENT_CODE;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_NAME = specialistExam.TDL_PATIENT_NAME;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_DOB = specialistExam.TDL_PATIENT_DOB;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_GENDER_NAME = specialistExam.TDL_PATIENT_GENDER_NAME;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_ADDRESS = specialistExam.TDL_PATIENT_ADDRESS;
                    hIS_SPECIALIST_EXAM.TREATMENT_ID = specialistExam.TREATMENT_ID;
                    hIS_SPECIALIST_EXAM.TREATMENT_BED_ROOM_ID = specialistExam.TREATMENT_BED_ROOM_ID;
                }
                HIS_SPECIALIST_EXAM rs = new HIS_SPECIALIST_EXAM();
                CommonParam param = new CommonParam();

                Inventec.Common.Logging.LogSystem.Warn("HIS_SPECIALIST_EXAM ____hIS_SPECIALIST_EXAM"
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hIS_SPECIALIST_EXAM), hIS_SPECIALIST_EXAM));
                if (!isEditMode)
                {
                    rs = new BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>(RequestUriStore.EXAM_CREATE, ApiConsumers.MosConsumer, hIS_SPECIALIST_EXAM, param);
                    if (rs != null)
                    {
                        isEditMode = true;
                        btnSua.Enabled = isEditMode;
                        btnThem.Enabled = !isEditMode;
                        btnLamLai.Enabled = !isEditMode;
                        specialistExam = rs;
                        success = true;
                    }
                }
                else
                {
                    rs = new BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>(RequestUriStore.EXAM_UPDATE, ApiConsumers.MosConsumer, hIS_SPECIALIST_EXAM, param);
                    if (rs != null)
                    {
                        success = true;
                    }
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLamLai_Click(object sender, EventArgs e)
        {
            try
            {
                MOS.SDO.WorkPlaceSDO workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace((moduleData));

                dteNgayMoi.DateTime = DateTime.Now;
                cboDepartment.EditValue = bedRoom.LAST_DEPARTMENT_ID;
                cboPhongKham.EditValue = workPlace.DepartmentId; 
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
                dxValidationProvider1.SetValidationRule(cboPhongKham, null);
                List<HIS_EMPLOYEE> lst1 = new List<HIS_EMPLOYEE>();
                List<EmployeeADO> listADO = new List<EmployeeADO>();
                Action myaction = () => {
                    lst1 = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DOCTOR == 1 && o.DEPARTMENT_ID == Convert.ToInt64(cboPhongKham.EditValue)).ToList();
                    foreach (var item in lst1)
                    {
                        EmployeeADO Emp = new EmployeeADO();
                        Emp.ID = item.ID;
                        Emp.LOGINNAME = item.LOGINNAME;
                        Emp.TDL_USERNAME = item.TDL_USERNAME;
                        Emp.EMPLOYEE_NAME_UNSIGN = convertToUnSign3(item.LOGINNAME);
                        listADO.Add(Emp);
                    }
                };
                Task task = new Task(myaction);
                task.Start();
                cboBacSiKham.Properties.DataSource = listADO;
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
                    List<HIS_EMPLOYEE> erSelectedNews = new List<HIS_EMPLOYEE>();
                    foreach (HIS_EMPLOYEE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.TDL_USERNAME);
                            erSelectedNews.Add(er);
                        }
                    }
                    this.lstEmployee = new List<HIS_EMPLOYEE>();
                    this.lstEmployee.AddRange(erSelectedNews);
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
