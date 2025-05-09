using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
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

namespace HIS.Desktop.Plugins.ExamSpecialist.ExamSpecialist
{

    public partial class frmExamSpecialist : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        long treatmentId = 0;
        int start = 0;
        #endregion
        private Inventec.Desktop.Common.Modules.Module currentModule;
        public frmExamSpecialist()
        {
            InitializeComponent();
        }
        public frmExamSpecialist(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this.treatmentId = treatmentId;
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
                LoadComboHisDepartment();
                SetDefaultValueControl();
                //FillDataToGrid(); 
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
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 50, 1));
            columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 150, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "DEPARTMENT_CODE", columnInfos, false, 250);
            ControlEditorLoader.Load(cboInviteDepartment, data, controlEditorADO);//truyền data vào cbo theo cấu hình controlEditorADO
            ControlEditorLoader.Load(cboExamExcuteDepartment, data, controlEditorADO);
        }

        private void SetDefaultValueControl()
        {
            txtSearch.Text = "";
            InitComboExamSpecialistStt();
            dtIntructionTimeFrom.DateTime = DateTime.Now;
            dtIntructionTimeTo.DateTime = DateTime.Now;
            throw new NotImplementedException();
        }

        private void InitComboExamSpecialistStt()
        {
            try
            {
                //var data = BackendDataWorker.Get<HIS_SPECIALIST_EXAM_S>

                List<object> approvalStatusList = new List<object>();
                approvalStatusList.Add(new { ID = 0, STATUS_NAME = "Chưa duyệt", IS_APPROVAL = (short?)null });
                approvalStatusList.Add(new { ID = 1, STATUS_NAME = "Đã duyệt", IS_APPROVAL = (short?)1 });
                approvalStatusList.Add(new { ID = 2, STATUS_NAME = "Từ chối", IS_APPROVAL = (short?)2 });
                approvalStatusList.Add(new { ID = 3, STATUS_NAME = "Tất cả", IS_APPROVAL = (short?)3 });

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
                Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 1");
                WaitingManager.Show();
                List<V_HIS_SPECIALIST_EXAM> listData = new List<V_HIS_SPECIALIST_EXAM>();
                gridControlExamSpecialist.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisSpecialistExamFilter filter = new HisSpecialistExamFilter();
                SetFilter(ref filter);
                Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 2");
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<HIS_SERVICE_REQ>>((HisConfigCFG.IsUseGetDynamic ? RequestUriStore.HIS_SERVICE_REQ_GET_DYNAMIC : RequestUriStore.HIS_SERVICE_REQ_GET), ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 3");
                if (result != null)
                {
                    rowCount = (result.Data == null ? 0 : result.Data.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    if (result.Data != null && result.Data.Count > 0)
                    {
                        foreach (var item in result.Data)
                        {
                            ADO.ServiceReqADO ado = new ADO.ServiceReqADO(item);
                            listData.Add(ado);
                        }
                    }
                    else
                    {
                        listData = null;
                    }
                    this.gridColumn_ServiceReq_Choose.Image = this.imageListCheck.Images[4];
                }

                gridControlServiceReq.BeginUpdate();
                gridControlServiceReq.DataSource = listData;
                gridControlServiceReq.EndUpdate();

                grdSereServServiceReq.BeginUpdate();
                grdSereServServiceReq.DataSource = null;
                grdSereServServiceReq.EndUpdate();
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("FillDataToGridTransaction. 4");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisSpecialistExamFilter filter)
        {
            try
            {
                bool IsNotDate = false;
                //bool IsNotServiceReq = false;
                if (filter == null) filter = new HisSpecialistExamFilter();
                filter.ORDER_FIELD = "INTRUCTION_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string codeTreatment = txtTreatmentCode.Text.Trim();
                    string codePatient = txtPatientCode.Text.Trim();
                    if (codeTreatment.Length < 12 && checkDigit(codeTreatment))
                    {
                        codeTreatment = string.Format("{0:000000000000}", Convert.ToInt64(codeTreatment));
                        txtTreatmentCode.Text = codeTreatment;
                    }
                    if (codePatient.Length < 12 && checkDigit(codePatient))
                    {
                        codePatient = string.Format("{0:000000000000}", Convert.ToInt64(codePatient));
                        txtPatientCode.Text = codePatient;
                    }
                    filter.PATIENT_CODE = codePatient;
                    filter.TREATMENT_CODE = codeTreatment; 
                }
                else
                {
                    IsNotDate = true;
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
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ApprovalExamSpecialist").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ApprovalExamSpecialist");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.treatmentId);
                    //listArgs.Add((HIS.Desktop.Common.DelegateSelectData)DelegateSelectDataContentSubclinical);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance
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
    }
}
