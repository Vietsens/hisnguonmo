using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using System.Text.RegularExpressions;
using HIS.Desktop.Controls.Session;
using MOS.SDO;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.BedRoomPartial.Popup
{
    public partial class frmSpecialty : Form
    {
        string DoctorLogin { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        L_HIS_TREATMENT_BED_ROOM treatmentBedRoomRow { get; set; }
        List<HIS_EMPLOYEE> lstEmployee { get; set; }
        List<HIS_DEPARTMENT> lstDepartment { get; set; }
        HIS_SPECIALIST_EXAM currentSpecialist {  get; set; }
        HIS_TRACKING currentTracking { get; set; }
        public frmSpecialty(L_HIS_TREATMENT_BED_ROOM treatmentBedRoomRow, Inventec.Desktop.Common.Modules.Module currentModule)
        {
            InitializeComponent();
            this.treatmentBedRoomRow = treatmentBedRoomRow;
            this.currentModule = currentModule;
        }

        private void frmSpecialty_Load(object sender, EventArgs e)
        {
            
            LoadcboEmployee();
            LoadcboDepartment();
            //LoadData();
        }

        private void LoadData()
        {
            try
            {
                dateInveteTime.DateTime = DateTime.Now;

                var USER = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (USER != null && lstEmployee != null)
                {
                    var selectedInviteDoctor = lstEmployee.FirstOrDefault(o => o.LOGINNAME == USER);
                    cboEmployeeInvite.EditValue = selectedInviteDoctor.ID;
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

                
                DataToCombocboEmployee(cboEmployeeInvite, listADO);
                DataToCombocboEmployee(cboExamination, listADO);
                    
                LoadData();
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

        private void ProcessSaveData(ref CommonParam param)
        {
            try
            {
                WaitingManager.Show();
                HIS_SPECIALIST_EXAM hIS_SPECIALIST_EXAM = new HIS_SPECIALIST_EXAM();
                MOS.SDO.WorkPlaceSDO workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace((currentModule));

                if (currentSpecialist != null)
                {
                    AutoMapper.Mapper.CreateMap<HIS_SPECIALIST_EXAM, HIS_SPECIALIST_EXAM>();
                    hIS_SPECIALIST_EXAM = AutoMapper.Mapper.Map<HIS_SPECIALIST_EXAM>(currentSpecialist);
                }


                hIS_SPECIALIST_EXAM.INVITE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dateInveteTime.DateTime.ToString("yyyyMMddHHmmss"));

                if (workPlace != null)
                {
                    hIS_SPECIALIST_EXAM.INVITE_DEPARMENT_ID = workPlace.DepartmentId;

                }

                if (cboEmployeeInvite.EditValue != null)
                {
                    var selectedInviteLogin = cboEmployeeInvite.EditValue.ToString();
                    var selectedInviteDoctor = lstEmployee.FirstOrDefault(o => o.ID.ToString() == selectedInviteLogin);
                    if (selectedInviteDoctor != null)
                    {
                        hIS_SPECIALIST_EXAM.INVITE_DOCTOR_LOGINNAME = selectedInviteDoctor.LOGINNAME;
                        hIS_SPECIALIST_EXAM.INVITE_DOCTOR_USERNAME = selectedInviteDoctor.TDL_USERNAME;
                    }
                }
                
                if (cboDepartment.EditValue != null)
                {
                    var selectedDepartment = cboDepartment.EditValue.ToString();
                    var selectedDepartments = lstDepartment.FirstOrDefault(o => o.ID.ToString() == selectedDepartment);
                    if (selectedDepartments != null)
                    {
                        hIS_SPECIALIST_EXAM.EXAM_EXECUTE_DEPARMENT_ID = selectedDepartments.ID;
                    }
                }

                if(cboExamination.EditValue != null){
                    var selectedExecuteLogin = cboExamination.EditValue.ToString();
                    var selectedExecuteDoctor = lstEmployee.FirstOrDefault(o => o.ID.ToString() == selectedExecuteLogin);
                    if (selectedExecuteDoctor != null)
                    {
                        hIS_SPECIALIST_EXAM.EXAM_EXECUTE_LOGINNAME = selectedExecuteDoctor.LOGINNAME;
                        hIS_SPECIALIST_EXAM.EXAM_EXECUTE_USERNAME = selectedExecuteDoctor.TDL_USERNAME;
                    }
                    
                }

                if (chkExamBed.Checked)
                    hIS_SPECIALIST_EXAM.IS__EXAM_BED = 1;
                else
                    hIS_SPECIALIST_EXAM.IS__EXAM_BED = null;

                hIS_SPECIALIST_EXAM.INVITE_CONTENT = txtContent.Text;
                hIS_SPECIALIST_EXAM.INVITE_TYPE = 1;
                if (treatmentBedRoomRow != null)
                {
                    hIS_SPECIALIST_EXAM.TREATMENT_CODE = treatmentBedRoomRow.TREATMENT_CODE;
                    hIS_SPECIALIST_EXAM.PATIENT_CODE = treatmentBedRoomRow.TDL_PATIENT_CODE;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_NAME = treatmentBedRoomRow.TDL_PATIENT_NAME;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_DOB = treatmentBedRoomRow.TDL_PATIENT_DOB;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_GENDER_NAME = treatmentBedRoomRow.TDL_PATIENT_GENDER_NAME;
                    hIS_SPECIALIST_EXAM.TDL_PATIENT_ADDRESS = treatmentBedRoomRow.TDL_PATIENT_ADDRESS;
                    hIS_SPECIALIST_EXAM.TREATMENT_ID = treatmentBedRoomRow.TREATMENT_ID;
                    hIS_SPECIALIST_EXAM.TREATMENT_BED_ROOM_ID = treatmentBedRoomRow.ID;
                }

                HIS_SPECIALIST_EXAM rs = new HIS_SPECIALIST_EXAM();
                
                Inventec.Common.Logging.LogSystem.Warn("HIS_SPECIALIST_EXAM ____hIS_SPECIALIST_EXAM"
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hIS_SPECIALIST_EXAM), hIS_SPECIALIST_EXAM));
                rs = new BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>(HisRequestUriStore.HIS_SPECIALIST_EXAM_CREATE, ApiConsumers.MosConsumer, hIS_SPECIALIST_EXAM, param);

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, rs!= null);
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

        private void barbtnAddSpecialty_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSpecialty_Click(this, EventArgs.Empty);
        }

        private void btnSpecialty_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            ProcessSaveData(ref param);
        }
    }

}
