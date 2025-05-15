using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;

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
            InitCombo();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                long idtam = 0;
                if (currentHisSpecialistExam != null)
                {
                    idtam = currentHisSpecialistExam.ID;
                }
                //Get data HIS_SPECIALIST_EXAM
                CommonParam param = new CommonParam();
                MOS.Filter.HisSpecialistExamViewFilter filterHSE = new HisSpecialistExamViewFilter();
                long fromTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                long toTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                filterHSE.INVITE_TYPE = 2;
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
                if (!string.IsNullOrEmpty(treatmentCode))
                {
                    filterHSE.TREATMENT_CODE = treatmentCode;
                }
                // Mã bệnh nhân (10 số, tự động thêm số 0 ở đầu)
                string patientCode = txtPatientcode.Text.Trim();
                if (!string.IsNullOrEmpty(patientCode) && patientCode.Length < 10)
                {
                    patientCode = patientCode.PadLeft(10, '0');
                    txtPatientcode.Text = patientCode;
                }
                if (!string.IsNullOrEmpty(patientCode))
                {
                    filterHSE.PATIENT_CODE = patientCode;
                }


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

                var data = new BackendAdapter(param).Get<List<V_HIS_SPECIALIST_EXAM>>("api/HisSpecialistExam/GetView", ApiConsumers.MosConsumer, filterHSE, param);
                gridControl1.DataSource = data;
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                gridView1.RefreshData();

                try
                {
                    if (idtam != 0)
                    {
                        var index = gridView1.LocateByValue("ID", idtam);
                        gridView1.FocusedRowHandle = index;
                        gridView1.SelectRow(index);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                
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

        private void InitCombo()
        {
            try
            {
                if (lstDepart == null || lstDepart.Count == 0) return;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ID", "", 50, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboDepartment, lstDepart, controlEditorADO);
                ControlEditorLoader.Load(cboExecuteDepartment, lstDepart, controlEditorADO);
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
            cboStatus.Properties.PopulateColumns();
            cboStatus.Properties.Columns["Value"].Visible = false;
            cboStatus.EditValue = statusList[0].Value;
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
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)LoadData);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();                        
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

                    if (currentHisSpecialistExam.IS_APPROVAL == 2)
                    {
                        frmRA.RejectReason = currentHisSpecialistExam.REJECT_APPROVAL_REASON; 
                    }
                    if (frmRA.ShowDialog() == DialogResult.OK)
                    {
                        CommonParam param = new CommonParam();
                        HIS_SPECIALIST_EXAM datamapper = new HIS_SPECIALIST_EXAM();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SPECIALIST_EXAM>(datamapper, currentHisSpecialistExam);
                        datamapper.REJECT_APPROVAL_REASON = frmRA.RejectReason;
                        datamapper.IS_APPROVAL = 2; // Từ chối:  IS_APPROVAL= 2

                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => datamapper), datamapper));
                        var rs = new BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>("api/HisSpecialistExam/Update", ApiConsumers.MosConsumer, datamapper, param);
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

        private void txtTreatmentcode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) LoadData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_SPECIALIST_EXAM data = (MOS.EFMODEL.DataModels.V_HIS_SPECIALIST_EXAM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "INVITE_TIME_CUS")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INVITE_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "IS_APPROVAL_CUS")
                        {
                            if (data.IS_APPROVAL == null)
                                e.Value = "Chưa duyệt";
                            else if (data.IS_APPROVAL == 1)
                                e.Value = "Đã duyệt";
                            else if (data.IS_APPROVAL == 2)
                                e.Value = "Từ chối duyệt";
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IS_APPROVAL_CUS")
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    V_HIS_SPECIALIST_EXAM data = (V_HIS_SPECIALIST_EXAM)view.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (data.IS_APPROVAL == 1) // Đã duyệt
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (data.IS_APPROVAL == 2) // Từ chối
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }
                        else // Chưa duyệt
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }
                if (e.Column.FieldName == "Debate")
                {
                    var rowData = gridView1.GetRow(e.RowHandle) as V_HIS_SPECIALIST_EXAM;
                    if (rowData != null && rowData.IS_APPROVAL != 1)
                    {
                        e.Handled = true; // Ngăn không vẽ ô này
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExecuteDepartment_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //try
            //{
            //    if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
            //    {
            //        cboExecuteDepartment.EditValue = null;
            //     }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void cboDepartment_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatus_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                if (e.Value == null)
                {
                    e.DisplayText = "Tất cả";
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
