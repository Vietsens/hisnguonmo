using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovaleDebateList
{
    public partial class frmApprovaleDebateList : HIS.Desktop.Utility.FormBase
    {
        List<HIS_DEPARTMENT> lstDepart;
        Inventec.Desktop.Common.Modules.Module currentModule;
        internal long roomId;
        internal long roomTypeId;
        internal V_HIS_SPECIALIST_EXAM currentHisSpecialistExam { get; set; }

        public frmApprovaleDebateList()
        {
            InitializeComponent();            
        }

        public frmApprovaleDebateList(Inventec.Desktop.Common.Modules.Module moduleData)
           : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = moduleData;
                this.roomId = moduleData.RoomId;
                this.roomTypeId = moduleData.RoomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmApprovaleDebateList_Load(object sender, EventArgs e)
        {
            SetIcon();
            SetDeFaultConTrol();
            LoadDepartment();
            InitStatusCombo();
            InitCombo(cboDepartment, lstDepart, "DEPARTMENT_NAME", "DEPARTMENT_CODE");
            InitCombo(cboExecuteDepartment, lstDepart, "DEPARTMENT_NAME", "DEPARTMENT_CODE");
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                //Get data HIS_SPECIALIST_EXAM
                CommonParam param = new CommonParam();
                //MOS.Filter.HisSpecialistExamFilter filterHSE = new HisSpecialistExamFilter();

                MOS.Filter.HisSpecialistExamViewFilter filterHSE = new HisSpecialistExamViewFilter();
                long fromTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                long toTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                //filterHSE.INVITE_TYPE = 2; không có trong filter
                filterHSE.INVITE_TIME_FROM = fromTime; 
                filterHSE.INVITE_TIME_TO = toTime;

                //Trạng thái duyệt
                filterHSE.IS_APPROVAL = cboStatus.EditValue != null ? Convert.ToInt64(cboStatus.EditValue) : (long?)null;

                // Mã điều trị (12 số, tự động thêm số 0 ở đầu)
                string treatmentCode = txtTreatmentcode.Text.Trim();
                if (!string.IsNullOrEmpty(treatmentCode) && treatmentCode.Length < 12)
                {
                    treatmentCode = treatmentCode.PadLeft(12, '0');
                    txtTreatmentcode.Text = treatmentCode;
                }
                filterHSE.TREATMENT_CODE = treatmentCode;

                // Mã bệnh nhân (10 số, tự động thêm số 0 ở đầu)
                string patientCode = txtPatientcode.Text.Trim();
                if (!string.IsNullOrEmpty(patientCode) && patientCode.Length < 10)
                {
                    patientCode = patientCode.PadLeft(10, '0');
                    txtPatientcode.Text = patientCode;
                }
                filterHSE.PATIENT_CODE = patientCode;

                // Khoa phòng điều trị
                if (cboDepartment.EditValue != null && long.TryParse(cboDepartment.EditValue.ToString(), out long departmentId))
                    filterHSE.INVITE_DEPARMENT_ID = departmentId;

                // Khoa phòng hội chẩn
                if (cboExecuteDepartment.EditValue != null && long.TryParse(cboExecuteDepartment.EditValue.ToString(), out long executeDepartmentId))
                    filterHSE.EXAM_EXECUTE_DEPARMENT_ID = executeDepartmentId;

                // Từ khóa tìm kiếm
                if (!string.IsNullOrEmpty(txtKeyword.Text))
                {
                    filterHSE.KEY_WORD = txtKeyword.Text?.Trim();
                }

                var data = new BackendAdapter(param).Get<List<V_HIS_SPECIALIST_EXAM>>("api/HisSpecialistExamView/Get", ApiConsumers.MosConsumer, filterHSE, param);
                V_HIS_SPECIALIST_EXAM tt = new V_HIS_SPECIALIST_EXAM();
                gridControl1.DataSource = data;
                gridView1.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadDepartment()
        {
            try
            {
                lstDepart = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void InitCombo(GridLookUpEdit cbo, List<HIS_DEPARTMENT> data, string DisplayValue, string ValueMember)
        {
            try
            {
                if (data == null || data.Count == 0) return;

                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.View.Columns.AddVisible(DisplayValue, "Tất cả");
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDeFaultConTrol()
        {
            try
            {
                txtTreatmentcode.Text = "";
                txtPatientcode.Text = "";
                txtKeyword.Text = "";
                dtTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                dtTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitStatusCombo()
        {
            List<StatusModel> statusList = new List<StatusModel>
                {
                    new StatusModel { Value = null, Display = "Tất cả" },
                    new StatusModel { Value = 0, Display = "Chưa duyệt" },
                    new StatusModel { Value = 1, Display = "Đã duyệt" },
                    new StatusModel { Value = 2, Display = "Từ chối" }
                };

            cboStatus.Properties.DataSource = statusList;
            cboStatus.Properties.DisplayMember = "Display";
            cboStatus.Properties.ValueMember = "Value";
            cboStatus.Properties.ShowHeader = false;
            cboStatus.EditValue = null;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            currentHisSpecialistExam = gridView1.GetFocusedRow() as V_HIS_SPECIALIST_EXAM;
        }

        // Duyệt hội chẩn
        private void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentHisSpecialistExam != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ApprovaleDebate").FirstOrDefault();
                    if (moduleData == null) throw new ArgumentNullException("khong tim thay moduleLink = HIS.Desktop.Plugins.ApprovaleDebate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        listArgs.Add(currentHisSpecialistExam);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    } 
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // Tạo biên bản hội chẩn
        private void btnDebate_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentHisSpecialistExam != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DebateDiagnostic").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.DebateDiagnostic");

                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                        filter.TREATMENT_ID = currentHisSpecialistExam.TREATMENT_ID;
                        filter.ORDER_DIRECTION = "DESC";
                        filter.ORDER_FIELD = "MODIFY_TIME";
                        HIS_SERVICE_REQ rs = new HIS_SERVICE_REQ();
                        rs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                        List<object> listArgs = new List<object>();
                        listArgs.Add(rs);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
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

        // Từ chối duyệt
        private void btnNo_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentHisSpecialistExam == null)
                {
                    MessageBox.Show("Vui lòng chọn một dòng dữ liệu.");
                    return;
                }

                using (frmRejectApproval frmRA = new frmRejectApproval())
                {
                    if (frmRA.ShowDialog() == DialogResult.OK)
                    {
                        CommonParam param = new CommonParam();
                        currentHisSpecialistExam.REJECT_APPROVAL_REASON = frmRA.RejectReason;
                        currentHisSpecialistExam.IS_APPROVAL = 2; // Từ chối:  IS_APPROVAL= 2
                        var rs = new BackendAdapter(param).Post<HIS_INFUSION_SUM>("api/HisSpecialistExam/Update", ApiConsumers.MosConsumer, currentHisSpecialistExam, param);
                        if (rs != null)
                        {
                            LoadData();
                        }
                        MessageManager.Show(this, param, rs != null);                       
                        SessionManager.ProcessTokenLost(param);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Debate")
                {
                    var rowData = gridView1.GetRow(e.RowHandle) as V_HIS_SPECIALIST_EXAM;
                    if (rowData != null && rowData.IS_APPROVAL != 1)
                    {
                        e.RepositoryItem = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class StatusModel
    {
        public int? Value { get; set; }
        public string Display { get; set; }
    }
}
