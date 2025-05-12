using HIS.Desktop.Plugins.ApprovalExamSpecialist.ValidateRule;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.Run
{
    public partial class frmApprovalExamSpecialist : FormBase
    {
        MOS.EFMODEL.DataModels.HIS_TRACKING currentVHisTracking = null;
        MOS.EFMODEL.DataModels.HIS_SPECIALIST_EXAM currentVHisSpecialist = null;
        private Inventec.Desktop.Common.Modules.Module currentModule;
        internal long treatmentId;
        internal string treatmentCode;

        long wkRoomId { get; set; }

        long wkRoomTypeId = 0;

        DHisSereServ2 TreeClickData;
        UCTreeListService ucToDieuTri, ucCDHA, ucXN, ucDichVu, ucSieuAm, ucPhauThuat, ucGiaiPhau;
        public frmApprovalExamSpecialist()
            :base(null)
        {
            InitializeComponent();
        }
        public frmApprovalExamSpecialist(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentID)
           : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.treatmentId = treatmentID;
                this.currentModule = currentModule;
                this.wkRoomId = this.currentModule != null ? this.currentModule.RoomId : 0;
                this.wkRoomTypeId = this.currentModule != null ? this.currentModule.RoomTypeId : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmApprovalExamSpecialist_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                AddUc();
                SetDefaultValueControl();
                SetValidateRule();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            dtTrackingTime.DateTime = DateTime.Now;
            txtNoiDungKham.Text = "";
            txtYLenhKham.Text = "";
        }
        private void AddUc()
        {
            try
            {
                ucToDieuTri = new UCTreeListService(imageCollection1, currentModule);
                ucCDHA = new UCTreeListService(imageCollection1, currentModule);
                ucXN = new UCTreeListService(imageCollection1, currentModule);
                ucDichVu = new UCTreeListService(imageCollection1, currentModule);
                ucSieuAm = new UCTreeListService(imageCollection1, currentModule);
                ucPhauThuat = new UCTreeListService(imageCollection1, currentModule);
                ucGiaiPhau = new UCTreeListService(imageCollection1, currentModule);

                panelControl1.Controls.Add(ucToDieuTri);
                ucToDieuTri.Dock = DockStyle.Fill;

                panelControl2.Controls.Add(ucCDHA);
                ucCDHA.Dock = DockStyle.Fill;

                panelControl3.Controls.Add(ucXN);
                ucXN.Dock = DockStyle.Fill;

                panelControl4.Controls.Add(ucDichVu);
                ucDichVu.Dock = DockStyle.Fill;

                panelControl5.Controls.Add(ucSieuAm);
                ucSieuAm.Dock = DockStyle.Fill;

                panelControl6.Controls.Add(ucPhauThuat);
                ucPhauThuat.Dock = DockStyle.Fill;

                panelControl7.Controls.Add(ucGiaiPhau);
                ucGiaiPhau.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetValidateRule()
        {
            ValidateMaxLength validateMaxLengthNoiDung = new ValidateMaxLength();
            validateMaxLengthNoiDung.textEdit = txtNoiDungKham;
            validateMaxLengthNoiDung.maxLength = 4000;
            validateMaxLengthNoiDung.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            validateMaxLengthNoiDung.isValidNull = true;
            dxValidationProviderEditorInfo.SetValidationRule(txtNoiDungKham, validateMaxLengthNoiDung);

            ValidateMaxLength validateMaxLengthYLenh = new ValidateMaxLength();
            validateMaxLengthYLenh.textEdit = txtYLenhKham;
            validateMaxLengthYLenh.maxLength = 4000;
            validateMaxLengthYLenh.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            validateMaxLengthYLenh.isValidNull = true;
            dxValidationProviderEditorInfo.SetValidationRule(txtYLenhKham, validateMaxLengthYLenh);

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                string doctorName = txtBacSiKham.Text?.Trim();
                string examContent = txtNoiDungKham.Text?.Trim();
                string examInstruction = txtYLenhKham.Text?.Trim();


                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                HIS_SPECIALIST_EXAM exam = null;

                HIS_TRACKING tracking = new HIS_TRACKING()
                {
                    TRACKING_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtTrackingTime.EditValue).ToString("yyyyMMdd") + "000000"),
                    CONTENT = examContent,
                    INSTRUCTION = examInstruction
                };

                this.ProcessSaveSpecialistExam(ref exam);
                tracking = new BackendAdapter(param).Post<HIS_TRACKING>(
                    HisRequestUriStore.HIS_TRACKING_CREATE, ApiConsumers.MosConsumer, tracking, param);

                if (exam != null && tracking != null)
                {
                    success = true;
                    MessageBox.Show("Lưu dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show("Lỗi trong quá trình lưu dữ liệu!\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ProcessSaveSpecialistExam(ref HIS_SPECIALIST_EXAM exam)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSpecialistExamFilter Filter = new HisSpecialistExamFilter();

                Filter.ID = currentVHisSpecialist.ID;
                var SpecialID = new BackendAdapter(new CommonParam()).Get<List<HIS_SPECIALIST_EXAM>>("api/HisSpecialistExam/Get", ApiConsumers.MosConsumer, Filter, new CommonParam());
                if (SpecialID != null && SpecialID.Count > 0)
                {
                    HIS_SPECIALIST_EXAM hisSpecialistResult = SpecialID.FirstOrDefault();
                    if (hisSpecialistResult.EXAM_TIME > 0)
                    {
                        dtTrackingTime.DateTime = Convert.ToDateTime(hisSpecialistResult.EXAM_TIME);
                    }
                    else
                    {
                        dtTrackingTime.DateTime = DateTime.Now;
                    }
                    if (!String.IsNullOrEmpty(hisSpecialistResult.EXAM_EXECUTE_CONTENT))
                    {
                        txtNoiDungKham.Text = hisSpecialistResult.EXAM_EXECUTE_CONTENT;
                    }
                    else
                    {
                        txtNoiDungKham.Text = "";
                    }
                    if (!String.IsNullOrEmpty(hisSpecialistResult.EXAM_EXCUTE))
                    {
                        txtYLenhKham.Text = hisSpecialistResult.EXAM_EXCUTE;
                    }
                    else
                    {
                        txtYLenhKham.Text = "";
                    }

                }
                // Lấy thông tin từ các control
                exam.EXAM_EXECUTE_CONTENT = txtNoiDungKham.Text?.Trim();
                exam.EXAM_EXCUTE = txtYLenhKham.Text?.Trim();
                exam.IS_APPROVAL = 1;
                // Gọi API để lưu
                exam = new BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>(
                    HisRequestUriStore.HIS_SPEC,
                    ApiConsumers.MosConsumer,
                    exam,
                    param);

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }


        private void FillDataToControl()
        {
            HIS_TRACKING ado = new HIS_TRACKING();
            HisTrackingFilter hisTrackingFilter = new HisTrackingFilter();

            hisTrackingFilter.TREATMENT_ID = currentVHisTracking.ID;
            var TrackingId = new BackendAdapter(new CommonParam()).Get<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, hisTrackingFilter, new CommonParam());
            if (TrackingId != null && TrackingId.Count > 0)
            {
                HIS_TRACKING hisTrackingResult = TrackingId.FirstOrDefault();
                if (hisTrackingResult.TRACKING_TIME > 0)
                {
                    dtTrackingTime.DateTime = Convert.ToDateTime(hisTrackingResult.TRACKING_TIME);
                }
                else
                {
                    dtTrackingTime.DateTime = DateTime.Now;
                }

            }
        }
    }
}
