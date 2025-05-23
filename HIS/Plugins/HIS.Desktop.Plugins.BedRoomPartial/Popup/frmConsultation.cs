using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Controls.Session;

namespace HIS.Desktop.Plugins.BedRoomPartial.Popup
{
    public partial class frmConsultation : HIS.Desktop.Utility.FormBase
    {
        L_HIS_TREATMENT_BED_ROOM treatmentBedRoomRow { get; set; }
        List<HIS_DEPARTMENT> lstDepartment { get; set; }
        HIS_SPECIALIST_EXAM currentConsultation { get; set; }
        HIS_TRACKING currentTracking { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;

        public frmConsultation(L_HIS_TREATMENT_BED_ROOM treatmentBedRoomRow, Inventec.Desktop.Common.Modules.Module currentModule)
        {
            InitializeComponent();
            this.treatmentBedRoomRow = treatmentBedRoomRow;
            this.currentModule = currentModule;
        }

        private void frmConsultation_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            SetIcon();
            dateEdit1.DateTime = DateTime.Now;
            LoadcboDepartment();
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
                cboDepartment.EditValue = treatmentBedRoomRow.LAST_DEPARTMENT_ID;
                InitComboConsultationRoomCheck();
                InitComboConsultationRoom(listADO);
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
        private void InitComboConsultationRoom(List<DepartmentADO> listADO)
        {
            try
            {
                cboConsultation.Properties.DataSource = listADO;
                cboConsultation.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboConsultation.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn column = cboConsultation.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                column.VisibleIndex = 1;
                column.Width = 200;

                DevExpress.XtraGrid.Columns.GridColumn columnCode = cboConsultation.Properties.View.Columns.AddField("DEPARTMENT_CODE");
                columnCode.VisibleIndex = 2;
                columnCode.Width = 100;
                columnCode.Visible = true;
                columnCode.Caption = "";
                columnCode.OptionsColumn.ShowCaption = false;

                column.Caption = "Tất cả";
                cboConsultation.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboConsultation.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboConsultationRoomCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboConsultation.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboConsultation.Properties.Tag = gridCheck;
                cboConsultation.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboConsultation.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboConsultation.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Event_Check(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                lstDepartment = new List<HIS_DEPARTMENT>();
                if (gridCheckMark != null)
                {
                    List<HIS_DEPARTMENT> erSelectedNews = new List<HIS_DEPARTMENT>();
                    foreach (HIS_DEPARTMENT er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.DEPARTMENT_NAME);
                            erSelectedNews.Add(er);
                        }
                    }
                    this.lstDepartment = new List<HIS_DEPARTMENT>();
                    this.lstDepartment.AddRange(erSelectedNews);
                }
                this.cboConsultation.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboConsultation_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.lstDepartment != null && this.lstDepartment.Count > 0)
                {
                    foreach (var item in this.lstDepartment)
                    {
                        roomName += item.DEPARTMENT_NAME + ", ";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnConsultation_Click(this, EventArgs.Empty);
        }
        private void saveDate(HIS_SPECIALIST_EXAM HIS_SPECIALIST_EXAM)
        {
            if (dateEdit1.EditValue != null)
            {
                HIS_SPECIALIST_EXAM.INVITE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dateEdit1.DateTime.ToString("yyyyMMddHHmmss"));
            }

            if (cboDepartment.EditValue != null)
            {
                HIS_SPECIALIST_EXAM.INVITE_DEPARMENT_ID = Convert.ToInt64(cboDepartment.EditValue);
            }

            if (memoEdit1.Text != null)
            {
                HIS_SPECIALIST_EXAM.INVITE_CONTENT = memoEdit1.Text;
            }

            HIS_SPECIALIST_EXAM.INVITE_TYPE = 2;

            if (treatmentBedRoomRow != null)
            {
                HIS_SPECIALIST_EXAM.TREATMENT_CODE = treatmentBedRoomRow.TREATMENT_CODE;
                HIS_SPECIALIST_EXAM.PATIENT_CODE = treatmentBedRoomRow.TDL_PATIENT_CODE;
                HIS_SPECIALIST_EXAM.TDL_PATIENT_NAME = treatmentBedRoomRow.TDL_PATIENT_NAME;
                HIS_SPECIALIST_EXAM.TDL_PATIENT_DOB = treatmentBedRoomRow.TDL_PATIENT_DOB;
                HIS_SPECIALIST_EXAM.TDL_PATIENT_GENDER_NAME = treatmentBedRoomRow.TDL_PATIENT_GENDER_NAME;
                HIS_SPECIALIST_EXAM.TDL_PATIENT_ADDRESS = treatmentBedRoomRow.TDL_PATIENT_ADDRESS;
                HIS_SPECIALIST_EXAM.TREATMENT_ID = treatmentBedRoomRow.TREATMENT_ID;
                HIS_SPECIALIST_EXAM.TREATMENT_BED_ROOM_ID = treatmentBedRoomRow.ID;
            }

            HIS_SPECIALIST_EXAM rs = new HIS_SPECIALIST_EXAM();
            CommonParam param = new CommonParam();

            Inventec.Common.Logging.LogSystem.Warn("HIS_SPECIALIST_EXAM ____hIS_SPECIALIST_EXAM"
              + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HIS_SPECIALIST_EXAM), HIS_SPECIALIST_EXAM));
            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>(HisRequestUriStore.HIS_SPECIALIST_EXAM_CREATE, ApiConsumers.MosConsumer, HIS_SPECIALIST_EXAM, param);

            WaitingManager.Hide();
            #region Hien thi message thong bao
            MessageManager.Show(this, param, rs != null);
            #endregion

            #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
            SessionManager.ProcessTokenLost(param);
            #endregion
        }
        private void ProcessSaveData()
        {

            try
            {
                WaitingManager.Show();

                HIS_SPECIALIST_EXAM HIS_SPECIALIST_EXAM = new HIS_SPECIALIST_EXAM();

                if(lstDepartment != null && lstDepartment.Count>0)
                {
                    foreach (var item in lstDepartment)
                    {
                        HIS_SPECIALIST_EXAM.EXAM_EXECUTE_DEPARMENT_ID = item.ID;

                        saveDate(HIS_SPECIALIST_EXAM);
                    }
                }
                else
                {
                    saveDate(HIS_SPECIALIST_EXAM);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnConsultation_Click(object sender, EventArgs e)
        {
            ProcessSaveData();
        }
    }
}
