using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.Desktop.Plugins.ExamSpecialist.ExamSpecialist
{

    public partial class frmExamSpecialist : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        long treatmentId = 0;
        int start = 0;
        V_HIS_SPECIALIST_EXAM curentSpecialistExam;
        #endregion
        private Inventec.Desktop.Common.Modules.Module currentModule;
        public frmExamSpecialist()
        {
            InitializeComponent();
        }
        public frmExamSpecialist(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId, V_HIS_SPECIALIST_EXAM curentSpecialistExam)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this.treatmentId = treatmentId;
            this.curentSpecialistExam = curentSpecialistExam;
            try
            {
                this.Text = currentModule.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmExamSpecialist_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.KeyPreview = true;
                LoadComboHisDepartment();
                SetDefaultValueControl();
                toolTipController1.Active = true;
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboHisDepartment()
        {
            CommonParam param = new CommonParam();
            HisDepartmentFilter filter = new HisDepartmentFilter();
            filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
            var data = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null).ToList();
            //Inventec.Common.Logging.LogSystem.Debug("API Result ABOUT ROOM: " + Inventec.Common.Logging.LogUtil.TraceData("Data ROOM:", data));
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("ID", "", 50, 1));
            columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 150, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 250);
            ControlEditorLoader.Load(cboInviteDepartment, data, controlEditorADO);//truyền data vào cbo theo cấu hình controlEditorADO
            ControlEditorLoader.Load(cboExamExcuteDepartment, data, controlEditorADO);
        }

        private void SetDefaultValueControl()
        {
            txtSearch.Text = "";
            InitComboExamSpecialistStt();
            dtIntructionTimeFrom.DateTime = DateTime.Today; // 0h0p0s
            dtIntructionTimeTo.DateTime = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        private void InitComboExamSpecialistStt()
        {
            try
            {
                //var data = BackendDataWorker.Get<>
                List<object> approvalStatusList = new List<object>();
                approvalStatusList.Add(new { STATUS_NAME = "Chưa duyệt", IS_APPROVAL = (short?)0 });
                approvalStatusList.Add(new { STATUS_NAME = "Đã duyệt", IS_APPROVAL = (short?)1 });
                approvalStatusList.Add(new { STATUS_NAME = "Từ chối", IS_APPROVAL = (short?)2 });
                approvalStatusList.Add(new { STATUS_NAME = "Tất cả", IS_APPROVAL = (short?)3 });

                // Cấu hình hiển thị cho combo
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("STATUS_NAME", "", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("STATUS_NAME", "IS_APPROVAL", columnInfos, false, 200); // cấu hình
                ControlEditorLoader.Load(cboExamSpecialistStt, approvalStatusList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FillDataToGrid();
        }
        private void FillDataToGrid()
        {
            try
            {
                int pagingSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;// xác định số dòng /trang
                FillDataToGridTransaction(new CommonParam(0, pagingSize)); // nạp dữ liệu cho trang đầu tiên
                CommonParam param = new CommonParam();
                param.Limit = rowCount;  // giới hạn số dòng
                param.Count = dataTotal; // tính tổng số trang trong phân trang
                ucPaging1.Init(FillDataToGridTransaction, param, pagingSize, this.gridControlExamSpecialist); // khởi tạo phân trang 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTransaction(object param)
        {
            try
            {
                WaitingManager.Show();
                List<V_HIS_SPECIALIST_EXAM> listData = new List<V_HIS_SPECIALIST_EXAM>();
                gridControlExamSpecialist.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisSpecialistExamViewFilter filter = new HisSpecialistExamViewFilter();
                SetFilter(ref filter);
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_SPECIALIST_EXAM>>("api/HisSpecialistExam/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    rowCount = (result.Data == null ? 0 : result.Data.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);

                    if (result.Data != null && result.Data.Count > 0)
                    {
                        listData = result.Data;
                        CommonParam Param = new CommonParam();
                        HisSpecialistExamFilter speFilter = new HisSpecialistExamFilter();
                        var speResult = new Inventec.Common.Adapter.BackendAdapter(Param).GetRO<List<HIS_SPECIALIST_EXAM>>(
                            "api/HisSpecialistExam/Get", ApiConsumers.MosConsumer, speFilter, Param);
                        if (speResult != null && speResult.Data != null && speResult.Data.Count > 0)
                        {
                            var validIds = speResult.Data.Where(d => d.INVITE_TYPE == 1).Select(d => d.ID).ToList();
                            listData = result.Data.Where(v => validIds.Contains(v.ID)).ToList();
                        }
                    }
                    else
                    {
                        listData = null;
                    }
                }
                gridControlExamSpecialist.BeginUpdate();
                gridControlExamSpecialist.DataSource = listData;
                gridControlExamSpecialist.EndUpdate();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisSpecialistExamViewFilter filter)
        {
            try
            {
                if (filter == null) filter = new HisSpecialistExamViewFilter();
                filter.ORDER_FIELD = "INVITE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string codeTreatment = txtTreatmentCode.Text.Trim();
                    if (codeTreatment.Length < 12 && checkDigit(codeTreatment))
                    {
                        codeTreatment = string.Format("{0:000000000000}", Convert.ToInt64(codeTreatment));
                        txtTreatmentCode.Text = codeTreatment;
                    }
                    filter.TREATMENT_CODE = codeTreatment;
                }
                if (!String.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string codePatient = txtPatientCode.Text;
                    if (codePatient.Length < 10 && checkDigit(codePatient))
                    {
                        codePatient = string.Format("{0:0000000000}", Convert.ToInt64(codePatient));
                        txtPatientCode.Text = codePatient;
                    }
                    filter.PATIENT_CODE = codePatient;
                }
                if (!String.IsNullOrEmpty(txtSearch.Text))
                {
                    filter.KEY_WORD = txtSearch.Text.Trim();
                }

                if (cboInviteDepartment.EditValue != null && long.TryParse(cboInviteDepartment.EditValue.ToString(), out long inviteDeptId))
                {
                    filter.INVITE_DEPARMENT_ID = inviteDeptId;
                }
                // Xử lý khoa phòng khám
                if (cboExamExcuteDepartment.EditValue != null && long.TryParse(cboExamExcuteDepartment.EditValue.ToString(), out long examDeptId))
                {
                    filter.EXAM_EXECUTE_DEPARMENT_ID = examDeptId;
                }
                // Xử lý trạng thái duyệt
                if (cboExamSpecialistStt.EditValue != null && long.TryParse(cboExamSpecialistStt.EditValue.ToString(), out long approvalStatus))
                {
                    if (approvalStatus != 3)
                    {
                        filter.IS_APPROVAL = (short?)approvalStatus;
                    }
                }
                // Xử lý thời gian mời
                if (dtIntructionTimeFrom.EditValue != null && dtIntructionTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.INVITE_TIME_FROM = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTimeFrom.DateTime);
                }
                if (dtIntructionTimeTo.EditValue != null && dtIntructionTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.INVITE_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTimeTo.DateTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IS_APPROVAL_STR")
                {
                    //var data = (V_HIS_SPECIALIST_EXAM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool checkDigit(string s)
        {
            bool result = true;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == false) return false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
        private void repositoryItemButtonEditApproval_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (V_HIS_SPECIALIST_EXAM)gridView1.GetFocusedRow();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ApprovalExamSpecialist").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ApprovalExamSpecialist");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance
                        .GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_SPECIALIST_EXAM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (((ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.CurrentPage) - 1) * (ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.PageSize));
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXAM_EXECUTE_STR")
                        {
                            try
                            {
                                e.Value = data.EXAM_EXECUTE_LOGINNAME + "-" + data.EXAM_EXECUTE_USERNAME;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IS__EXAM_BED_STR")
                        {
                            e.Value = data.IS__EXAM_BED == 1 ? true : false;
                        }
                        else if (e.Column.FieldName == "INVITE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.INVITE_TIME);
                        }
                        else if (e.Column.FieldName == "IS_APPROVAL_STR")
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
        private void frmExamSpecialist_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                btnSearch_Click(null, null);
                e.Handled = true;
            }
        }

        private void repositoryItemButtonEditReject_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (V_HIS_SPECIALIST_EXAM)gridView1.GetFocusedRow();
                if (row != null)
                {
                    HIS_SPECIALIST_EXAM datamapper = new HIS_SPECIALIST_EXAM();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SPECIALIST_EXAM>(datamapper, row);
                    frmReject form = new frmReject(datamapper, () => FillDataToGrid());
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInviteDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboInviteDepartment.Properties.Buttons[1].Visible = cboInviteDepartment.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboExamExcuteDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboExamExcuteDepartment.Properties.Buttons[1].Visible = cboExamExcuteDepartment.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboInviteDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (cboInviteDepartment.EditValue != null && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboInviteDepartment.EditValue = null;
                    cboInviteDepartment.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamExcuteDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (cboExamExcuteDepartment.EditValue != null && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboExamExcuteDepartment.EditValue = null;
                    cboExamExcuteDepartment.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboInviteDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
