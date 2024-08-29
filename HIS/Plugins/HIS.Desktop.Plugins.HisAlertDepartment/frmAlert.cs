using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraGrid;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisAlertDepartment.Model;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.Desktop.Plugins.HisAlertDepartment
{
    public partial class frmAlert : DevExpress.XtraEditors.XtraForm
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        int rowCountAlert = 0;
        int dataTotalAlert = 0;
        int startPageAlert = 0; 
        //revice
        int rowCountRecive = 0;
        int dataTotalRecive = 0;
        int startPageRecive = 0;
        List<HIS_ALERT_DEPARTMENT> listAlertDepartment;
        List<DepartmentDTO> listDepartmentAlert;
        List<DepartmentDTO> listDepartmentRecive;
        List<DepartmentDTO> listDepartmentSourceAlert;
        List<DepartmentDTO> listDepartmentSourceRecive;
        long TYPE;
        public frmAlert(Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;

        }


        #region LOAD DATA
        private void frmAlert_Load(object sender, EventArgs e)
        {
            try
            {

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                listDepartmentAlert = new List<DepartmentDTO>();
                listDepartmentRecive = new List<DepartmentDTO>();
                listDepartmentSourceAlert = new List<DepartmentDTO>();
                listDepartmentSourceRecive = new List<DepartmentDTO>();
                SetCaptionByLanguageKey();
                
                LoadDefault();
                FillDataToControl1();
                FillDataToControl2();
                TYPE = Convert.ToInt16(cbotype.EditValue);
                //FillDataToControl(gridControlDepartmentRecive,ucPaging2);

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }



        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisAlertDepartment.Resources.Lang", typeof(frmAlert).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAlert.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchValue2.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAlert.txtSearchValue2.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchValue2.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAlert.txtSearchValue2.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind2.Text = Inventec.Common.Resource.Get.Value("frmAlert.btnFind2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind1.Text = Inventec.Common.Resource.Get.Value("frmAlert.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchValue1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAlert.txtSearchValue1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchValue1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAlert.txtSearchValue1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAlert.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemCheckEdit3.Caption = Inventec.Common.Resource.Get.Value("frmAlert.repositoryItemCheckEdit3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmAlert.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmAlert.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmAlert.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmAlert.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemCheckEdit2.Caption = Inventec.Common.Resource.Get.Value("frmAlert.repositoryItemCheckEdit2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CHECK_MANY.Caption = Inventec.Common.Resource.Get.Value("frmAlert.CHECK_MANY.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmAlert.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmAlert.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmAlert.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmAlert.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbotype.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAlert.cbotype.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                
                if(this.moduleData != null)
                {
                    this.Text = this.moduleData.text.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region LOAD FIRST DATA

        private void FillDataToControl1()
        {
            try
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
                    //load danh sach khoa baop dong
                    LoadPagingAlert(new CommonParam(0, pageSize));
                    CommonParam param = new CommonParam();
                    param.Limit = rowCountAlert;
                    param.Count = dataTotalAlert;
                    ucPaging1.Init(LoadPagingAlert, param, pageSize, gridControlDepartmentAlert);
                    WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                
                LogSystem.Warn(ex);
            }
        }
        private void FillDataToControl2()
        {
            try
            {
                try
                {

                    WaitingManager.Show();


                    int pageSize2 = 0;
                    if (ucPaging2.pagingGrid != null)
                    {
                        pageSize2 = ucPaging2.pagingGrid.PageSize;
                    }
                    else
                    {
                        pageSize2 = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                    }
                    LoadPagingRecive(new CommonParam(0, pageSize2));
                    CommonParam paramRecive = new CommonParam();
                    paramRecive.Limit = rowCountRecive;
                    paramRecive.Count = dataTotalRecive;
                    ucPaging2.Init(LoadPagingRecive, paramRecive, pageSize2, gridControlDepartmentRecive);

                    WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void LoadPagingAlert(object param)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Load data to list alert");
                CommonParam paramCommon;
                var startPage =  ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                paramCommon = new CommonParam(startPage, limit);
                this.gridControlDepartmentAlert.BeginUpdate();
                Inventec.Core.ApiResultObject<List<HIS_DEPARTMENT>> apiResult = null;
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                if (!string.IsNullOrEmpty(txtSearchValue1.Text)) filter.KEY_WORD = txtSearchValue1.Text;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_DEPARTMENT>>("/api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    AutoMapper.Mapper.CreateMap<HIS_DEPARTMENT, DepartmentDTO>();
                    listDepartmentSourceAlert = AutoMapper.Mapper.Map<List<DepartmentDTO>>(data);
                    if (data != null)
                    {
                        gridControlDepartmentAlert.DataSource = listDepartmentSourceAlert;
                        rowCountAlert = (data == null ? 0 : data.Count);
                        dataTotalAlert = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                    }
                }
                gridControlDepartmentAlert.EndUpdate();
            }
            catch (Exception ex)
            {
                
                LogSystem.Warn(ex);
            }
        }
        private void LoadPagingRecive(object param)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Load data to list recive");
                CommonParam paramCommon;
                var startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                paramCommon = new CommonParam(startPage, limit);
                this.gridControlDepartmentRecive.BeginUpdate();
                Inventec.Core.ApiResultObject<List<HIS_DEPARTMENT>> apiResult = null;
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                if (!string.IsNullOrEmpty(txtSearchValue2.Text)) filter.KEY_WORD = txtSearchValue2.Text;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_DEPARTMENT>>("/api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    AutoMapper.Mapper.CreateMap<HIS_DEPARTMENT, DepartmentDTO>();
                    listDepartmentSourceRecive = AutoMapper.Mapper.Map<List<DepartmentDTO>>(data);
                    
                    if (data != null)
                    {
                        gridControlDepartmentRecive.DataSource = listDepartmentSourceRecive;
                        rowCountRecive = (data == null ? 0 : data.Count);
                        dataTotalRecive = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);


                    }
                }
                gridControlDepartmentRecive.EndUpdate();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        private void LoadDefault()
        {
            try
            {
                List<Model.DepartmentType> listType = new List<Model.DepartmentType>();
                listType.Add(new Model.DepartmentType() { ID = 2, DEPARTMENT_TYPE_CODE = "DEPARTMENT-CREATE", DEPARTMENT_TYPE_NAME = "Khoa Báo Động" });
                listType.Add(new Model.DepartmentType() { ID = 1, DEPARTMENT_TYPE_CODE = "DEPARTMENT-RECIVE", DEPARTMENT_TYPE_NAME = "Khoa Nhận" });
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_TYPE_NAME", "", 190, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_TYPE_NAME", "ID", columnInfos, false, 100);
                ControlEditorLoader.Load(cbotype, listType, controlEditorADO);
                cbotype.EditValue = 2;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void LoadDataWithDepartment(DepartmentDTO rowData,bool check)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisAlertDepartmentFilter filter = new HisAlertDepartmentFilter();
                if (TYPE == 2)
                {
                    filter.DEPARTMENT_ID = rowData.ID;
                }
                else filter.RECEIVE_DEPARTMENT_ID = rowData.ID;
                var rs = new BackendAdapter(param).Get<List<HIS_ALERT_DEPARTMENT>>("/api/HisAlertDepartment/Get", ApiConsumers.MosConsumer, filter, param);
                if(rs != null)
                {
                    listAlertDepartment = rs;
                    if(TYPE == 2 )RefeshDataSelected(rs.Select(s=>s.RECEIVE_DEPARTMENT_ID).ToList(),gridViewDepartmentRecive,rs,check);
                    else RefeshDataSelected(rs.Select(s => s.DEPARTMENT_ID).ToList(), gridViewDepartmentAlert,rs,check);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void RefeshDataSelected(List<long> listID, GridView control,List<HIS_ALERT_DEPARTMENT> listDepartment,bool check)
        {
            try
            {
                var newList = new List<DepartmentDTOWithCheck>();
                for (int i = 0; i < control.RowCount; i++)
                {
                    var row = control.GetRow(i) as DepartmentDTO; 
                    // thêm dữ liệu vào 1 danh sách mới để sắp xếp cái được chọn lên đầu tiên

                    var newRow = new DepartmentDTOWithCheck();
                    Inventec.Common.Mapper.DataObjectMapper.Map<DepartmentDTO>(newRow, row);
                    newRow.Check = listID.Contains(row.ID) ? check : false;
                    newList.Add(newRow);
                    //
                    if (row != null && listID.Contains(row.ID))
                    {
                        // Đánh dấu IS_CHECK là true nếu ID có trong danh sách
                        control.SetRowCellValue(i, "SELECT_MANY", check);
                        var de = listDepartment.FirstOrDefault(s => (TYPE == 2 && s.RECEIVE_DEPARTMENT_ID == row.ID)||(TYPE == 1 && s.DEPARTMENT_ID == row.ID));
                        control.SetRowCellValue(i, "CHECK_ALERT", de!= null && de.IS_MEDICAL == 1 ? true : false);
                        control.SetRowCellValue(i, "CHECK_SECURITY",de != null&& de.IS_SECURITY == 1 ? true : false);
                    }
                    else
                    {
                        // Nếu không có trong danh sách thì bỏ đánh dấu
                        control.SetRowCellValue(i, "SELECT_MANY", false);
                        control.SetRowCellValue(i, "CHECK_ALERT", false);
                        control.SetRowCellValue(i, "CHECK_SECURITY",false);
                        control.SetRowCellValue(i, "SORT_ORDER", 0);
                    }
                   

                }
                var orderedList = newList.OrderByDescending(x => x.Check).ToList();

                // Set DataSource cho GridView
                control.GridControl.DataSource = orderedList;
                // Load lại grid để áp dụng thay đổi
                control.RefreshData();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region HANDLE CHANGE
        private void cbotype_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if(cbotype.EditValue != null )
                {
                    LogSystem.Debug("_____________Loai:"+ Convert.ToInt64(cbotype.EditValue));
                    LogSystem.Debug("_____________Loai: _______1 : chon nhieu khoa tao - 1 khoa nhan;______2; chon 1 khoa tao - nhieu khoa nhan ");

                    EnableControlSelect(Convert.ToInt64(cbotype.EditValue));
                    dicIsMedicalAlert.Clear();
                    dicIsSecurityAlert.Clear();
                    dicIsMedicalRecive.Clear();
                    dicIsSecurityRecive.Clear();

                    ClearDataSelected(gridViewDepartmentAlert,listDepartmentAlert, checkboxStates,dicIsMedicalAlert,dicIsSecurityAlert);
                    ClearDataSelected(gridViewDepartmentRecive,listDepartmentRecive, checkboxStatesRecive,dicIsMedicalRecive,dicIsSecurityRecive);
                    TYPE = Convert.ToInt64(cbotype.EditValue);
                    
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ClearDataSelected(GridView control, List<DepartmentDTO> list, Dictionary<long, bool> dic, Dictionary<long, bool> dicMedical, Dictionary<long, bool> dicSecurity)
        {
            try
            {
                // Lấy danh sách các row đã chọn
                int[] selectedRowHandles = control.GetSelectedRows();

                foreach (int rowHandle in selectedRowHandles)
                {
                    if (rowHandle >= 0)
                    {
                        var row = (DepartmentDTO)control.GetRow(rowHandle);
                        long rowKey = row.ID;

                        // Xóa dòng khỏi GridView
                        control.DeleteRow(rowHandle);

                        // Xóa trạng thái checkbox từ từ điển
                        dic?.Remove(rowKey);
                        dicMedical?.Remove(rowKey);
                        dicSecurity?.Remove(rowKey);

                        // Xóa dòng khỏi danh sách nếu cần
                        list.Remove(row);
                    }
                }

                // Làm mới lại GridView
                control.RefreshData();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }


        private void EnableControlSelect(long value)
        {
            try
            {
                if(value == 1)//chon khoa tao bao dong
                {

                    // Danh sách khoa báo động
                    EnableCheckbox(gridViewDepartmentAlert);
                    DisableRadio(gridViewDepartmentAlert);

                    // Kích hoạt radio trong GridView Khoa nhận
                    EnableRadio(gridViewDepartmentRecive);
                    DisableCheckbox(gridViewDepartmentRecive);
                }
                else
                {
                    // Kích hoạt checkbox trong GridView Khoa nhận
                    EnableCheckbox(gridViewDepartmentRecive);
                    DisableRadio(gridViewDepartmentRecive);

                    // Kích hoạt radio trong GridView Khoa báo động
                    EnableRadio(gridViewDepartmentAlert);
                    DisableCheckbox(gridViewDepartmentAlert);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void EnableCheckbox(GridView gridView)
        {
            RepositoryItemCheckEdit checkEdit = (RepositoryItemCheckEdit)gridView.Columns["SELECT_MANY"].ColumnEdit;
            checkEdit.ReadOnly = false; // Cho phép chỉnh sửa (enable)
            checkEdit.AllowFocused = true;
            checkEdit.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
            //check security
            RepositoryItemCheckEdit checkEditSe = (RepositoryItemCheckEdit)gridView.Columns["CHECK_SECURITY"].ColumnEdit;
            checkEditSe.ReadOnly = false; // Không cho phép chỉnh sửa (disable)
            checkEditSe.AllowFocused = true;
            checkEditSe.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
            //check medical
            RepositoryItemCheckEdit checkEditAL = (RepositoryItemCheckEdit)gridView.Columns["CHECK_ALERT"].ColumnEdit;
            checkEditAL.ReadOnly = false; // Không cho phép chỉnh sửa (disable)
            checkEditAL.AllowFocused = true;
            checkEditAL.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
        }

        private void DisableCheckbox(GridView gridView)
        {
            RepositoryItemCheckEdit checkEdit = (RepositoryItemCheckEdit)gridView.Columns["SELECT_MANY"].ColumnEdit;
            checkEdit.ReadOnly = true; // Không cho phép chỉnh sửa (disable)
            checkEdit.AllowFocused = false;
            checkEdit.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style5;
            //check security
            RepositoryItemCheckEdit checkEditSe = (RepositoryItemCheckEdit)gridView.Columns["CHECK_SECURITY"].ColumnEdit;
            checkEditSe.ReadOnly = true; // Không cho phép chỉnh sửa (disable)
            checkEditSe.AllowFocused = false;
            checkEditSe.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style5;
            //check medical
            RepositoryItemCheckEdit checkEditAL = (RepositoryItemCheckEdit)gridView.Columns["CHECK_ALERT"].ColumnEdit;
            checkEditAL.ReadOnly = true; // Không cho phép chỉnh sửa (disable)
            checkEditAL.AllowFocused = false;
            checkEditAL.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style5;
        }


        private void EnableRadio(GridView gridView)
        {
            RepositoryItemCheckEdit radioEdit = (RepositoryItemCheckEdit)gridView.Columns["SELECT_ONE"].ColumnEdit;
            radioEdit.ReadOnly = false; // Cho phép chỉnh sửa (enable)
            radioEdit.AllowFocused = true;
        }

        private void DisableRadio(GridView gridView)
        {
            RepositoryItemCheckEdit radioEdit = (RepositoryItemCheckEdit)gridView.Columns["SELECT_ONE"].ColumnEdit;
            radioEdit.ReadOnly = true; // Không cho phép chỉnh sửa (disable)
            radioEdit.AllowFocused = false;
            
        }
        #endregion



        #region GRID DEPARTMENT ALERT
        private Dictionary<long, bool> checkboxStates = new Dictionary<long, bool>();
        private void gridViewDepartmentAlert_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DepartmentDTO rowData = (DepartmentDTO)gridViewDepartmentAlert.GetRow(e.RowHandle);
                if (e.Column.FieldName == "SELECT_MANY") // Thay "CheckColumn" bằng tên cột checkbox của bạn
                {
                    if (e.Value == null)
                        return;


                    bool isChecked = (bool)e.Value; // Kiểm tra xem checkbox có được chọn không
                    long rowKey = rowData.ID;
                    if (checkboxStates.ContainsKey(rowKey))
                    {
                        checkboxStates[rowKey] = isChecked;
                    }
                    else
                    {
                        checkboxStates.Add(rowKey, isChecked);
                    }

                    // Cập nhật danh sách các dòng đã chọn
                    UpdateSelectedRows(rowData, isChecked);
                    // Nếu bỏ chọn SELECT_MANY, tự động bỏ chọn CHECK_ALERT và CHECK_SECURITY
                    if (!isChecked)
                    {
                        dicIsMedicalAlert[rowKey] = false;
                        dicIsSecurityAlert[rowKey] = false;

                        // Cập nhật lại giá trị của cột CHECK_ALERT và CHECK_SECURITY trên lưới
                        gridViewDepartmentAlert.SetRowCellValue(e.RowHandle, "CHECK_ALERT", false);
                        gridViewDepartmentAlert.SetRowCellValue(e.RowHandle, "CHECK_SECURITY", false);
                        gridControlDepartmentAlert.RefreshDataSource();
                    }

                }
                
                else if (e.Column.FieldName == "CHECK_ALERT" || e.Column.FieldName == "CHECK_SECURITY")
                {
                    bool isChecked = (bool)e.Value;
                    long rowKey = rowData.ID;
                    if(listDepartmentAlert.Count == 0)
                    {
                        dicIsMedicalAlert[rowKey] = false;
                        dicIsSecurityAlert[rowKey] = false;
                    }
                    // Update the corresponding dictionary based on the column field name
                    if (e.Column.FieldName == "CHECK_ALERT")
                    {
                        dicIsMedicalAlert[rowKey] = isChecked;
                    }
                    else if (e.Column.FieldName == "CHECK_SECURITY")
                    {
                        dicIsSecurityAlert[rowKey] = isChecked;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        
        Dictionary<long, bool> dicIsMedicalAlert = new Dictionary<long, bool>();
        Dictionary<long, bool> dicIsSecurityAlert = new Dictionary<long, bool>();
        private void UpdateSelectedRows(DepartmentDTO rowData, bool isChecked)
        {
            if (isChecked)
            {
                if (!listDepartmentAlert.Contains(rowData))
                {
                    if (Convert.ToInt16(cbotype.EditValue) == 2) listDepartmentAlert.Clear();
                    listDepartmentAlert.Add(rowData);
                }
            }
            else
            {
                if (listDepartmentAlert.Contains(rowData))
                {
                    listDepartmentAlert.Remove(rowData);
                }
            }
        }


        private void gridViewDepartmentAlert_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                var rowData = gridViewDepartmentAlert.GetRow(e.RowHandle) as DepartmentDTO;
                if (e.Column.FieldName == "SELECT_MANY") // Thay "CheckColumn" bằng tên cột checkbox của bạn
                {
                    
                    if (rowData != null && checkboxStates.ContainsKey(rowData.ID))
                    {
                        e.CellValue = checkboxStates[rowData.ID];
                    }
                }

                if (e.Column.FieldName == "CHECK_ALERT")
                {
                    
                    if (rowData != null && dicIsMedicalAlert.ContainsKey(rowData.ID))
                    {
                        e.CellValue = dicIsMedicalAlert[rowData.ID];
                    }
                }
                if (e.Column.FieldName == "CHECK_SECURITY")
                {
                    if (rowData != null && dicIsSecurityAlert.ContainsKey(rowData.ID))
                    {
                        e.CellValue = dicIsSecurityAlert[rowData.ID];
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void gridViewDepartmentAlert_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                var rowData = (DepartmentDTO)e.Row;
                if (e.Column.FieldName == "SELECT_MANY")
                {
                    
                    if (e.IsGetData)
                    {
                        if (rowData != null && checkboxStates.ContainsKey(rowData.ID))
                        {
                            e.Value = checkboxStates[rowData.ID];
                        }
                        else
                        {
                            e.Value = false;
                        }
                    }
                    if (e.IsSetData)
                    {
                        bool isChecked = (bool)e.Value;
                        long rowKey = rowData.ID;
                        if (checkboxStates.ContainsKey(rowKey))
                        {
                            checkboxStates[rowKey] = isChecked;
                        }
                        else
                        {
                            checkboxStates.Add(rowKey, isChecked);
                        }

                        UpdateSelectedRows(rowData, isChecked);
                    }
                }
                if (e.Column.FieldName == "CHECK_ALERT")
                {

                    if (e.IsGetData)
                    {
                        if (rowData != null && dicIsMedicalAlert.ContainsKey(rowData.ID))
                        {
                            e.Value = dicIsMedicalAlert[rowData.ID];
                        }
                        else
                        {
                            e.Value = false;
                        }
                    }
                    if (e.IsSetData)
                    {
                        bool isChecked = (bool)e.Value;
                        long rowKey = rowData.ID;
                        if (dicIsMedicalAlert.ContainsKey(rowKey))
                        {
                            dicIsMedicalAlert[rowKey] = isChecked;
                        }
                        else
                        {
                            dicIsMedicalAlert.Add(rowKey, isChecked);
                        }
                    }
                }
                if (e.Column.FieldName == "CHECK_SECURITY")
                {

                    if (e.IsGetData)
                    {
                        if (rowData != null && dicIsSecurityAlert.ContainsKey(rowData.ID))
                        {
                            e.Value = dicIsSecurityAlert[rowData.ID];
                        }
                        else
                        {
                            e.Value = false;
                        }
                    }
                    if (e.IsSetData)
                    {
                        bool isChecked = (bool)e.Value;
                        long rowKey = rowData.ID;
                        if (dicIsSecurityAlert.ContainsKey(rowKey))
                        {
                            dicIsSecurityAlert[rowKey] = isChecked;
                        }
                        else
                        {
                            dicIsSecurityAlert.Add(rowKey, isChecked);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        

        private void repositoryItemCheckEdit2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                DepartmentDTO rowData = (DepartmentDTO)gridViewDepartmentAlert.GetFocusedRow();
                listDepartmentRecive.Clear();
                CheckEdit check = sender as CheckEdit;
                rowData.SELECT_ONE = check.Checked;
                foreach (var department in listDepartmentSourceAlert)
                {
                    if (department.ID != rowData.ID)
                    {
                        department.SELECT_ONE = false;
                    }
                }
                UpdateSelectedRows(rowData, check.Checked);
                if (rowData != null) LoadDataWithDepartment(rowData, check.Checked);
                // Refresh lại dữ liệu trong grid để cập nhật thay đổi
                if (listDepartmentAlert.Count == 0)
                {
                    ClearDataSelected(gridViewDepartmentRecive, listDepartmentRecive, checkboxStatesRecive,dicIsMedicalRecive,dicIsSecurityRecive);
                }
                gridViewDepartmentAlert.RefreshData();

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion


        private bool CheckEnable(GridView control,string key)
        {
            
            try
            {
                RepositoryItemCheckEdit checkEdit = (RepositoryItemCheckEdit)control.Columns[key].ColumnEdit;
                if (checkEdit.ReadOnly == true) return false;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
                return false;
            }
            return true;
        }

        #region GRID DEPARTMENT RECIVE
        private Dictionary<long, bool> checkboxStatesRecive = new Dictionary<long, bool>();  
        private void gridViewDepartmentRecive_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "SELECT_MANY") // Thay "CheckColumn" bằng tên cột checkbox của bạn
                {
                    if (e.Value == null)
                        return;
                    //if (!CheckEnable(gridViewDepartmentRecive, "SELECT_MANY")) return;
                    DepartmentDTO rowData = (DepartmentDTO)gridViewDepartmentRecive.GetRow(e.RowHandle);
                    bool isChecked = (bool)e.Value; // Kiểm tra xem checkbox có được chọn không
                    long rowKey = rowData.ID;
                    if (checkboxStatesRecive.ContainsKey(rowKey))
                    {
                        checkboxStatesRecive[rowKey] = isChecked;
                    }
                    else
                    {
                        checkboxStatesRecive.Add(rowKey, isChecked);
                    }

                    // Cập nhật danh sách các dòng đã chọn
                    UpdateSelectedRowsRecive(rowData, isChecked);
                    if (!isChecked)
                    {
                        dicIsMedicalAlert[rowKey] = false;
                        dicIsSecurityAlert[rowKey] = false;

                        // Cập nhật lại giá trị của cột CHECK_ALERT và CHECK_SECURITY trên lưới
                        gridViewDepartmentRecive.SetRowCellValue(e.RowHandle, "CHECK_ALERT", false);
                        gridViewDepartmentRecive.SetRowCellValue(e.RowHandle, "CHECK_SECURITY", false);
                        
                    }

                }
                
                if (e.Column.FieldName == "CHECK_ALERT" || e.Column.FieldName == "CHECK_SECURITY")
                {
                    DepartmentDTO rowData = (DepartmentDTO)gridViewDepartmentRecive.GetRow(e.RowHandle);
                    bool isChecked = (bool)e.Value;
                    long rowKey = rowData.ID;
                    if (listDepartmentRecive.Count == 0)
                    {
                        dicIsMedicalRecive[rowKey] = false;
                        dicIsSecurityRecive[rowKey] = false;
                    }
                    // Update the corresponding dictionary based on the column field name
                    if (e.Column.FieldName == "CHECK_ALERT")
                    {
                        dicIsMedicalRecive[rowKey] = isChecked;
                    }
                    else if (e.Column.FieldName == "CHECK_SECURITY")
                    {
                        dicIsSecurityRecive[rowKey] = isChecked;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        Dictionary<long, bool> dicIsMedicalRecive = new Dictionary<long, bool>();
        Dictionary<long, bool> dicIsSecurityRecive = new Dictionary<long, bool>();
        private void UpdateSelectedRowsRecive(DepartmentDTO rowData, bool isChecked)
        {
            if (isChecked)
            {
                if (!listDepartmentRecive.Contains(rowData))
                {
                    if (Convert.ToInt16(cbotype.EditValue) == 1) listDepartmentRecive.Clear();
                    listDepartmentRecive.Add(rowData);

                }
            }
            else
            {
                if (listDepartmentRecive.Contains(rowData))
                {
                    listDepartmentRecive.Remove(rowData);
                }
            }
        }

        private void gridViewDepartmentRecive_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "SELECT_MANY") // Thay "CheckColumn" bằng tên cột checkbox của bạn
                {
                    var rowData = gridViewDepartmentRecive.GetRow(e.RowHandle) as DepartmentDTO;
                    if (rowData != null && checkboxStatesRecive.ContainsKey(rowData.ID))
                    {
                        e.CellValue = checkboxStatesRecive[rowData.ID];
                    }
                }
                
                if (e.Column.FieldName == "CHECK_ALERT") 
                {
                    var rowData = gridViewDepartmentRecive.GetRow(e.RowHandle) as DepartmentDTO;
                    if (rowData != null && dicIsMedicalRecive.ContainsKey(rowData.ID))
                    {
                        e.CellValue = dicIsMedicalRecive[rowData.ID];
                    }
                }
                if (e.Column.FieldName == "CHECK_SECURITY")
                {
                    var rowData = gridViewDepartmentRecive.GetRow(e.RowHandle) as DepartmentDTO;
                    if (rowData != null && dicIsSecurityRecive.ContainsKey(rowData.ID))
                    {
                        e.CellValue = dicIsSecurityRecive[rowData.ID];
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void gridViewDepartmentRecive_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                var rowData = (DepartmentDTO)e.Row;
                if (e.Column.FieldName == "SELECT_MANY")
                {
                    if (!CheckEnable(gridViewDepartmentRecive, "SELECT_MANY")) return;
                    if (e.IsGetData)
                    {
                        if (rowData != null && checkboxStatesRecive.ContainsKey(rowData.ID))
                        {
                            e.Value = checkboxStatesRecive[rowData.ID];
                        }
                        else
                        {
                            e.Value = false;
                        }
                    }
                    if (e.IsSetData)
                    {
                        bool isChecked = (bool)e.Value;
                        long rowKey = rowData.ID;
                        if (checkboxStatesRecive.ContainsKey(rowKey))
                        {
                            checkboxStatesRecive[rowKey] = isChecked;
                        }
                        else
                        {
                            checkboxStatesRecive.Add(rowKey, isChecked);
                        }

                        UpdateSelectedRowsRecive(rowData, isChecked);
                    }
                }


                if (e.Column.FieldName == "CHECK_ALERT")
                {
                    if (e.IsGetData)
                    {
                        if (rowData != null && dicIsMedicalRecive.ContainsKey(rowData.ID))
                        {
                            e.Value = dicIsMedicalRecive[rowData.ID];
                        }
                        else
                        {
                            e.Value = false;
                        }
                    }
                    if (e.IsSetData)
                    {
                        bool isChecked = (bool)e.Value;
                        long rowKey = rowData.ID;
                        if (dicIsMedicalRecive.ContainsKey(rowKey))
                        {
                            dicIsMedicalRecive[rowKey] = isChecked;
                        }
                        else
                        {
                            dicIsMedicalRecive.Add(rowKey, isChecked);
                        }

                        
                    }
                }
                if (e.Column.FieldName == "CHECK_SECURITY")
                {
                    if (e.IsGetData)
                    {
                        if (rowData != null && dicIsSecurityRecive.ContainsKey(rowData.ID))
                        {
                            e.Value = dicIsSecurityRecive[rowData.ID];
                        }
                        else
                        {
                            e.Value = false;
                        }
                    }
                    if (e.IsSetData)
                    {
                        bool isChecked = (bool)e.Value;
                        long rowKey = rowData.ID;
                        if (dicIsSecurityRecive.ContainsKey(rowKey))
                        {
                            dicIsSecurityRecive[rowKey] = isChecked;
                        }
                        else
                        {
                            dicIsSecurityRecive.Add(rowKey, isChecked);
                        }

                       
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void repositoryItemCheckEdit3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //Cột chọn 1 dòng
                // Xử ly check chọn
                DepartmentDTO rowData = (DepartmentDTO)gridViewDepartmentRecive.GetFocusedRow();
                listDepartmentAlert.Clear();
                CheckEdit check = sender as CheckEdit;
                rowData.SELECT_ONE = check.Checked;
                foreach (var department in listDepartmentSourceRecive)
                {
                    if (department.ID != rowData.ID)
                    {
                        department.SELECT_ONE = false;
                    }
                }
                UpdateSelectedRowsRecive(rowData, check.Checked);
                if(rowData != null) LoadDataWithDepartment(rowData,check.Checked);
                // Refresh lại dữ liệu trong grid để cập nhật thay đổi
                if(listDepartmentRecive.Count == 0)
                {
                    ClearDataSelected(gridViewDepartmentAlert, listDepartmentAlert, checkboxStates,dicIsMedicalAlert,dicIsSecurityAlert);
                }
                gridViewDepartmentRecive.RefreshData();

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

       



        #endregion


        #region HANDLE SAVE UPDATE
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var commonIDs = listDepartmentAlert.Select(s => s.ID)
                                   .Intersect(listDepartmentRecive.Select(s => s.ID))
                                   .ToList();
                if (commonIDs.Any())
                {
                    LogSystem.Debug("___DU lieu trung:" + LogUtil.TraceData("Trung", commonIDs));
                    MessageBox.Show(this, "Khoa đang chọn không được giống khoa gửi yêu cầu", "Thông báo", MessageBoxButtons.OK);
                    return;
                }
                
                if(TYPE == 2)
                {
                    if (listDepartmentAlert.Count == 0 || listDepartmentRecive.Count == 0) return;
                    foreach (var alert in listDepartmentRecive)
                    {
                        // Check if the ID is present in either dicIsMedicalAlert or dicIsSecurityAlert
                        bool isInMedicalAlert = dicIsMedicalRecive.ContainsKey(alert.ID) && dicIsMedicalRecive[alert.ID] == true;
                        bool isInSecurityAlert = dicIsSecurityRecive.ContainsKey(alert.ID) && dicIsSecurityRecive[alert.ID] == true;

                        // If the ID is not found in both dictionaries, show a warning message
                        if (!isInMedicalAlert && !isInSecurityAlert)
                        {
                            MessageBox.Show(this, "Khoa đang chọn phải có ít nhất 1 trong 2 thông tin “báo động y khoa” hoặc “báo động an ninh”", "Thông báo", MessageBoxButtons.OK);
                            return;
                        }
                    }
                }
                else
                {
                    if (listDepartmentRecive.Count == 0 || listDepartmentAlert.Count == 0) return;
                    foreach (var alert in listDepartmentAlert)
                    {
                        // Check if the ID is present in either dicIsMedicalAlert or dicIsSecurityAlert
                        bool isInMedicalAlert = dicIsMedicalAlert.ContainsKey(alert.ID) && dicIsMedicalAlert[alert.ID] == true;
                        bool isInSecurityAlert = dicIsSecurityAlert.ContainsKey(alert.ID) && dicIsSecurityAlert[alert.ID] == true;

                        // If the ID is not found in both dictionaries, show a warning message
                        if (!isInMedicalAlert && !isInSecurityAlert)
                        {
                            MessageBox.Show(this, "Khoa đang chọn phải có ít nhất 1 trong 2 thông tin “báo động y khoa” hoặc “báo động an ninh”", "Thông báo", MessageBoxButtons.OK);
                            return;
                        }
                    }
                }

                LogSystem.Debug("_________Start save data");
                ProcessSave(TYPE);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ProcessSave(long type)
        {
            try
            {

                CommonParam param = new CommonParam();
                HisAlertDepartmentFilter filter = new HisAlertDepartmentFilter();
                if(type == 1)
                {
                    //filter.DEPARTMENT_IDs = listDepartmentAlert.Select(s => s.ID).ToList();
                    filter.RECEIVE_DEPARTMENT_IDs = listDepartmentRecive.Select(s => s.ID).ToList();
                }
                else
                {
                    filter.DEPARTMENT_IDs = listDepartmentAlert.Select(s => s.ID).ToList();
                    //filter.RECEIVE_DEPARTMENT_IDs = listDepartmentRecive.Select(s => s.ID).ToList();
                }
                bool success = false;
                List<HIS_ALERT_DEPARTMENT> rs = new BackendAdapter(param).Get<List<HIS_ALERT_DEPARTMENT>>("/api/HisAlertDepartment/Get", ApiConsumers.MosConsumer, filter, param);
                if(rs != null && rs.Count > 0)
                {
                    LogSystem.Debug("____________Loai:"+type);
                    if (type == 2)
                    {
                        var exitsRecive = rs.Where(s => listDepartmentRecive.Select(o=>o.ID).Contains(s.RECEIVE_DEPARTMENT_ID));// lay ra cac khoa da duoc chon truoc do
                        if( exitsRecive.Any() )
                        {
                            List<HIS_ALERT_DEPARTMENT> listDTO = new List<HIS_ALERT_DEPARTMENT>();
                            //khoa nao duoc chon roi thi update
                            foreach (var record in exitsRecive)
                            {
                                var recive = listDepartmentRecive.FirstOrDefault(s => s.ID == record.RECEIVE_DEPARTMENT_ID);
                                if (dicIsMedicalRecive.ContainsKey(recive.ID) && dicIsMedicalRecive[recive.ID] == true)
                                {
                                    record.IS_MEDICAL = 1;
                                }
                                if (dicIsSecurityRecive.ContainsKey(recive.ID) && dicIsSecurityRecive[recive.ID] == true)
                                {
                                    record.IS_SECURITY = 1;
                                }
                                listDTO.Add(record);
                                record.DEPARTMENT_ID = listDepartmentAlert.First().ID;
                                
                            }
                            Save(listDTO, GlobalVariables.ActionEdit, ref success);
                        }
                        var NotexitsRecive = listDepartmentRecive.Where(s => !rs.Select(o => o.RECEIVE_DEPARTMENT_ID).Contains(s.ID));// lay ra cac khoa chua duoc chon truoc do
                        if(NotexitsRecive.Any() )
                        {
                            List<HIS_ALERT_DEPARTMENT> listDTO = new List<HIS_ALERT_DEPARTMENT>();
                            //khoa nao duoc chon ma chua co thi them moi
                            foreach (var rc in NotexitsRecive)
                            {
                                HIS_ALERT_DEPARTMENT record = new HIS_ALERT_DEPARTMENT();
                                record.DEPARTMENT_ID = listDepartmentAlert.First().ID;
                                record.RECEIVE_DEPARTMENT_ID = rc.ID;
                                if (dicIsMedicalRecive.ContainsKey(rc.ID) && dicIsMedicalRecive[rc.ID] == true)
                                {
                                    record.IS_MEDICAL = 1;
                                }
                                if (dicIsSecurityRecive.ContainsKey(rc.ID) && dicIsSecurityRecive[rc.ID] == true)
                                {
                                    record.IS_SECURITY = 1;
                                }
                                listDTO.Add(record);
                            }
                            Save(listDTO, GlobalVariables.ActionAdd, ref success);
                        }
                        var rsNotexit = rs.Where(s => !listDepartmentRecive.Select(o => o.ID).Contains(s.RECEIVE_DEPARTMENT_ID));
                        if(rsNotexit.Any())
                        {
                            // khoa nao dc bo chon thi xoa di
                            Delete(rsNotexit.Select(s => s.ID).ToList(), ref success);
                        }
                    }
                    else
                    {

                        var exitsRecive = rs.Where(s=> listDepartmentAlert.Select(o=>o.ID).Contains(s.DEPARTMENT_ID));// lay ra cac khoa da duoc chon truoc do
                        if (exitsRecive.Any())
                        {
                            List<HIS_ALERT_DEPARTMENT> listDTO = new List<HIS_ALERT_DEPARTMENT>();
                            //khoa nao duoc chon roi thi update
                            foreach (var record in exitsRecive)
                            {
                                var alert = listDepartmentAlert.FirstOrDefault(s => s.ID == record.DEPARTMENT_ID);
                                if (dicIsMedicalAlert.ContainsKey(alert.ID) && dicIsMedicalAlert[alert.ID] == true)
                                {
                                    record.IS_MEDICAL = 1;
                                }
                                if (dicIsSecurityAlert.ContainsKey(alert.ID) && dicIsSecurityAlert[alert.ID] == true)
                                {
                                    record.IS_SECURITY = 1;
                                }
                                listDTO.Add(record);
                                //
                                
                            }
                            Save(listDTO, GlobalVariables.ActionEdit, ref success);
                        }
                        var NotexitsRecive = listDepartmentAlert.Where(s => !rs.Select(o => o.DEPARTMENT_ID).Contains(s.ID));// lay ra cac khoa chua duoc chon truoc do
                        if (NotexitsRecive.Any())
                        {
                            List<HIS_ALERT_DEPARTMENT> listDTO = new List<HIS_ALERT_DEPARTMENT>();
                            //khoa nao duoc chon ma chua co thi update
                            foreach (var rc in NotexitsRecive)
                            {
                                HIS_ALERT_DEPARTMENT record = new HIS_ALERT_DEPARTMENT();
                                record.RECEIVE_DEPARTMENT_ID = listDepartmentRecive.First().ID;
                                record.RECEIVE_DEPARTMENT_ID = rc.ID;
                                if (dicIsMedicalAlert.ContainsKey(rc.ID) && dicIsMedicalAlert[rc.ID] == true)
                                {
                                    record.IS_MEDICAL = 1;
                                }
                                if (dicIsSecurityAlert.ContainsKey(rc.ID) && dicIsSecurityAlert[rc.ID] == true)
                                {
                                    record.IS_SECURITY = 1;
                                }
                                listDTO.Add(record);
                            }
                            Save(listDTO, GlobalVariables.ActionAdd, ref success);
                        }
                        var rsNotexit = rs.Where(s => !listDepartmentAlert.Select(o => o.ID).Contains(s.DEPARTMENT_ID));
                        if (rsNotexit.Any())
                        {
                            // khoa nao dc bo chon thi xoa di
                            Delete(rsNotexit.Select(s => s.ID).ToList(), ref success);
                        }

                    }
                    

                }
                else
                {
                    LogSystem.Debug("Chua co du lieu.______Them moi");
                    List<HIS_ALERT_DEPARTMENT> listDTO = new List<HIS_ALERT_DEPARTMENT>();
                    if (type == 1)
                    {
                        foreach (var alert in listDepartmentAlert)
                        {
                            HIS_ALERT_DEPARTMENT record = new HIS_ALERT_DEPARTMENT();
                            record.DEPARTMENT_ID = alert.ID;
                            record.IS_MEDICAL = dicIsMedicalAlert.ContainsKey(alert.ID) ? (short?)(dicIsMedicalAlert[alert.ID] ? 1 : 0) : null;
                            record.IS_SECURITY = dicIsSecurityAlert.ContainsKey(alert.ID) ? (short?)(dicIsSecurityAlert[alert.ID] ? 1 : 0) : null;
                            record.RECEIVE_DEPARTMENT_ID = listDepartmentRecive.Select(s => s.ID).FirstOrDefault();
                            listDTO.Add(record);
                        }
                    }
                    else
                    {
                        foreach (var revice in listDepartmentRecive)
                        {
                            HIS_ALERT_DEPARTMENT record = new HIS_ALERT_DEPARTMENT();
                            record.IS_MEDICAL = dicIsMedicalRecive.ContainsKey(revice.ID) ? (short?)(dicIsMedicalRecive[revice.ID] ? 1 : 0) : null;
                            record.IS_SECURITY = dicIsSecurityRecive.ContainsKey(revice.ID) ? (short?)(dicIsSecurityRecive[revice.ID] ? 1 : 0) : null;
                            record.DEPARTMENT_ID = listDepartmentAlert.Select(s => s.ID).FirstOrDefault();
                            record.RECEIVE_DEPARTMENT_ID = revice.ID;
                            listDTO.Add(record);
                        }

                    }
                    Save(listDTO, GlobalVariables.ActionAdd, ref success);

                }
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void Delete(List<long> listID,ref bool success)
        {
            try
            { 
                if (listID == null) return;
                CommonParam param = new CommonParam();
                success  = new BackendAdapter(param).Post<bool>("/api/HisAlertDepartment/DeleteList", ApiConsumers.MosConsumer, listID, param);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void Save(List<HIS_ALERT_DEPARTMENT> listDTO,int action,ref bool success)
        {
            try
            {
                if (listDTO == null) return;
                CommonParam param = new CommonParam();
                List<HIS_ALERT_DEPARTMENT> result = new List<HIS_ALERT_DEPARTMENT>();
                if(action == GlobalVariables.ActionEdit)
                {
                    LogSystem.Debug("___DU lieu UPDATE:" + LogUtil.TraceData("___DATA", listDTO));
                    result = new BackendAdapter(param).Post<List<HIS_ALERT_DEPARTMENT>>("/api/HisAlertDepartment/UpdateList", ApiConsumers.MosConsumer, listDTO, param);
                    if(result != null)
                    {
                        success = true;
                    }
                }
                else if (action == GlobalVariables.ActionAdd)
                {
                    result = new BackendAdapter(param).Post<List<HIS_ALERT_DEPARTMENT>>("/api/HisAlertDepartment/CreateList", ApiConsumers.MosConsumer, listDTO, param);
                    if (result != null)
                    {
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl1();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl2();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void frmAlert_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.Control && e.KeyCode == Keys.F)
                {
                    btnFind2.PerformClick();
                }
                if (e.Control && e.KeyCode == Keys.D)
                {
                    btnFind1.PerformClick();
                }
                if(e.Control && e.KeyCode == Keys.S)
                {
                    btnSave.PerformClick();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtSearchValue1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter)
                {
                    FillDataToControl1();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtSearchValue2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToControl2();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }



        //Vẽ ô checkbox trên header cho các cột
        bool isHeaderCheckBoxChecked = false;
        bool isHeaderCheckBoxCheckedAlert = false;
        bool isHeaderCheckBoxCheckedSecurity = false;
        private void gridViewDepartmentAlert_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.FieldName == "SELECT_MANY") // Thay YourColumnName bằng tên cột của bạn
                {
                    
                    e.Info.InnerElements.Clear(); // Xóa các phần tử hiện tại trong tiêu đề
                    e.Painter.DrawObject(e.Info); // Vẽ tiêu đề mà không có nội dung

                    // Tạo một ô vuông để chứa checkbox
                    Rectangle rect = e.Bounds;
                    rect.Inflate(-1, -1);

                    // Tạo và cấu hình CheckEditViewInfo
                    CheckEditViewInfo checkInfo = new CheckEditViewInfo(new RepositoryItemCheckEdit());
                    checkInfo.EditValue = isHeaderCheckBoxChecked;
                    checkInfo.Bounds = rect;
                    checkInfo.CalcViewInfo(e.Graphics);

                    // Sử dụng CheckEditPainter để vẽ checkbox
                    CheckEditPainter painter = new CheckEditPainter();
                    ControlGraphicsInfoArgs args = new ControlGraphicsInfoArgs(checkInfo, e.Cache, rect);
                    painter.Draw(args);
                    
                    e.Handled = true; // Ngăn việc vẽ tiêu đề mặc định
                }
                if(e.Column != null && e.Column.FieldName == "CHECK_ALERT")
                {
                    e.Info.InnerElements.Clear();
                    e.Info.Caption = "";
                    e.Painter.DrawObject(e.Info);
                    // Tạo một ô vuông để chứa checkbox
                    Rectangle checkBoxRect = e.Bounds;
                    checkBoxRect.Width = 15; // Đặt chiều rộng của vùng để checkbox

                    // Tạo và cấu hình CheckEditViewInfo cho checkbox
                    CheckEditViewInfo checkInfo = new CheckEditViewInfo(new RepositoryItemCheckEdit());
                    checkInfo.EditValue = isHeaderCheckBoxCheckedAlert;
                    checkInfo.Bounds = checkBoxRect;
                    checkInfo.CalcViewInfo(e.Graphics);

                    // Sử dụng CheckEditPainter để vẽ checkbox
                    CheckEditPainter painter = new CheckEditPainter();
                    ControlGraphicsInfoArgs args = new ControlGraphicsInfoArgs(checkInfo, e.Cache, checkBoxRect);
                    painter.Draw(args);

                    // Điều chỉnh vùng vẽ của tiêu đề để vẽ nội dung còn lại
                    Rectangle textRect = e.Bounds;
                    textRect.X += checkBoxRect.Width; // Đẩy text sang phải sau checkbox

                    e.Appearance.DrawString(e.Cache, "Báo động y khoa", textRect); // Vẽ nội dung tiêu đề cột
                    e.Handled = true; // Ngăn việc vẽ tiêu đề mặc định
                }
                if (e.Column != null && e.Column.FieldName == "CHECK_SECURITY")
                {
                    e.Info.InnerElements.Clear();
                    e.Info.Caption = "";
                    e.Painter.DrawObject(e.Info);
                    // Tạo một ô vuông để chứa checkbox
                    Rectangle checkBoxRect = e.Bounds;
                    checkBoxRect.Width = 15; // Đặt chiều rộng của vùng để checkbox

                    // Tạo và cấu hình CheckEditViewInfo cho checkbox
                    CheckEditViewInfo checkInfo = new CheckEditViewInfo(new RepositoryItemCheckEdit());
                    checkInfo.EditValue = isHeaderCheckBoxCheckedSecurity;
                    checkInfo.Bounds = checkBoxRect;
                    checkInfo.CalcViewInfo(e.Graphics);

                    // Sử dụng CheckEditPainter để vẽ checkbox
                    CheckEditPainter painter = new CheckEditPainter();
                    ControlGraphicsInfoArgs args = new ControlGraphicsInfoArgs(checkInfo, e.Cache, checkBoxRect);
                    painter.Draw(args);

                    // Điều chỉnh vùng vẽ của tiêu đề để vẽ nội dung còn lại
                    Rectangle textRect = e.Bounds;
                    textRect.X += checkBoxRect.Width; // Đẩy text sang phải sau checkbox

                    e.Appearance.DrawString(e.Cache, "Báo động an ninh", textRect); // Vẽ nội dung tiêu đề cột
                    
                    e.Handled = true; // Ngăn việc vẽ tiêu đề mặc định
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void gridViewDepartmentAlert_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(e.Location);
            if (info.InColumn && info.Column.FieldName == "SELECT_MANY") // Thay YourColumnName bằng tên cột của bạn
            {
                if (!CheckEnable(gridViewDepartmentAlert, "SELECT_MANY")) return;
                isHeaderCheckBoxChecked = !isHeaderCheckBoxChecked;

                for (int i = 0; i < view.RowCount; i++)
                {
                    view.SetRowCellValue(i, view.Columns["SELECT_MANY"], isHeaderCheckBoxChecked);
                }

                view.Invalidate(); // Yêu cầu GridView vẽ lại để cập nhật checkbox ở tiêu đề
            }
            if (info.InColumn && info.Column.FieldName == "CHECK_ALERT") // Thay YourColumnName bằng tên cột của bạn
            {
                //if (!CheckEnable(gridViewDepartmentAlert, "SELECT_MANY")) return;
                isHeaderCheckBoxCheckedAlert = !isHeaderCheckBoxCheckedAlert;

                for (int i = 0; i < view.RowCount; i++)
                {
                    view.SetRowCellValue(i, view.Columns["CHECK_ALERT"], isHeaderCheckBoxCheckedAlert);
                }

                view.Invalidate(); // Yêu cầu GridView vẽ lại để cập nhật checkbox ở tiêu đề
            }
            if (info.InColumn && info.Column.FieldName == "CHECK_SECURITY") // Thay YourColumnName bằng tên cột của bạn
            {
                //if (!CheckEnable(gridViewDepartmentAlert, "SELECT_MANY")) return;
                isHeaderCheckBoxCheckedSecurity = !isHeaderCheckBoxCheckedSecurity;

                for (int i = 0; i < view.RowCount; i++)
                {
                    view.SetRowCellValue(i, view.Columns["CHECK_SECURITY"], isHeaderCheckBoxCheckedSecurity);
                }

                view.Invalidate(); // Yêu cầu GridView vẽ lại để cập nhật checkbox ở tiêu đề
            }
        }
        bool isHeaderCheckBoxCheckedRecive = false;
        bool isHeaderCheckBoxCheckedAlertRecive = false;
        bool isHeaderCheckBoxCheckedSecurityRecive = false;
        private void gridViewDepartmentRecive_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.FieldName == "SELECT_MANY") // Thay YourColumnName bằng tên cột của bạn
                {

                    e.Info.InnerElements.Clear(); // Xóa các phần tử hiện tại trong tiêu đề
                    e.Painter.DrawObject(e.Info); // Vẽ tiêu đề mà không có nội dung

                    // Tạo một ô vuông để chứa checkbox
                    Rectangle rect = e.Bounds;
                    rect.Inflate(-1, -1);

                    // Tạo và cấu hình CheckEditViewInfo
                    CheckEditViewInfo checkInfo = new CheckEditViewInfo(new RepositoryItemCheckEdit());
                    checkInfo.EditValue = isHeaderCheckBoxCheckedRecive;
                    checkInfo.Bounds = rect;
                    checkInfo.CalcViewInfo(e.Graphics);

                    // Sử dụng CheckEditPainter để vẽ checkbox
                    CheckEditPainter painter = new CheckEditPainter();
                    ControlGraphicsInfoArgs args = new ControlGraphicsInfoArgs(checkInfo, e.Cache, rect);
                    painter.Draw(args);

                    e.Handled = true; // Ngăn việc vẽ tiêu đề mặc định
                }
                if (e.Column != null && e.Column.FieldName == "CHECK_ALERT")
                {
                    e.Info.InnerElements.Clear();
                    e.Info.Caption = "";
                    e.Painter.DrawObject(e.Info);
                    // Tạo một ô vuông để chứa checkbox
                    Rectangle checkBoxRect = e.Bounds;
                    checkBoxRect.Width = 15; // Đặt chiều rộng của vùng để checkbox

                    // Tạo và cấu hình CheckEditViewInfo cho checkbox
                    CheckEditViewInfo checkInfo = new CheckEditViewInfo(new RepositoryItemCheckEdit());
                    checkInfo.EditValue = isHeaderCheckBoxCheckedAlertRecive;
                    checkInfo.Bounds = checkBoxRect;
                    checkInfo.CalcViewInfo(e.Graphics);

                    // Sử dụng CheckEditPainter để vẽ checkbox
                    CheckEditPainter painter = new CheckEditPainter();
                    ControlGraphicsInfoArgs args = new ControlGraphicsInfoArgs(checkInfo, e.Cache, checkBoxRect);
                    painter.Draw(args);

                    // Điều chỉnh vùng vẽ của tiêu đề để vẽ nội dung còn lại
                    Rectangle textRect = e.Bounds;
                    textRect.X += checkBoxRect.Width; // Đẩy text sang phải sau checkbox

                    e.Appearance.DrawString(e.Cache, "Báo động y khoa", textRect); // Vẽ nội dung tiêu đề cột
                    e.Handled = true; // Ngăn việc vẽ tiêu đề mặc định
                }
                if (e.Column != null && e.Column.FieldName == "CHECK_SECURITY")
                {
                    //
                    e.Info.InnerElements.Clear();
                    e.Info.Caption = "";
                    e.Painter.DrawObject(e.Info);
                    // Tạo một ô chữ nhật để chứa checkbox
                    Rectangle checkBoxRect = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 5, 15, 15); // Vị trí và kích thước checkbox

                    // Tạo và cấu hình CheckEditViewInfo cho checkbox
                    CheckEditViewInfo checkInfo = new CheckEditViewInfo(new RepositoryItemCheckEdit());
                    checkInfo.EditValue = isHeaderCheckBoxCheckedSecurityRecive;
                    checkInfo.Bounds = checkBoxRect;
                    checkInfo.CalcViewInfo(e.Graphics);

                    // Vẽ checkbox
                    CheckEditPainter painter = new CheckEditPainter();
                    ControlGraphicsInfoArgs args = new ControlGraphicsInfoArgs(checkInfo, e.Cache, checkBoxRect);
                    painter.Draw(args);

                    // Tạo vùng cho text
                    Rectangle textRect = new Rectangle(checkBoxRect.Right + 5, e.Bounds.Y, e.Bounds.Width - checkBoxRect.Width - 10, e.Bounds.Height);

                    // Vẽ nội dung tiêu đề (text) từ biến caption
                    string caption = "Báo động an ninh";
                    e.Appearance.DrawString(e.Cache, caption, textRect);

                    // Ngăn việc vẽ tiêu đề mặc định thêm lần nữa
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void gridViewDepartmentRecive_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(e.Location);
            if (info.InColumn && info.Column.FieldName == "SELECT_MANY") // Thay YourColumnName bằng tên cột của bạn
            {
                if (!CheckEnable(gridViewDepartmentRecive, "SELECT_MANY")) return;
                isHeaderCheckBoxCheckedRecive = !isHeaderCheckBoxCheckedRecive;

                for (int i = 0; i < view.RowCount; i++)
                {
                    view.SetRowCellValue(i, view.Columns["SELECT_MANY"], isHeaderCheckBoxCheckedRecive);
                }

                view.Invalidate(); // Yêu cầu GridView vẽ lại để cập nhật checkbox ở tiêu đề
            }
            if (info.InColumn && info.Column.FieldName == "CHECK_ALERT") // Thay YourColumnName bằng tên cột của bạn
            {
                //if (!CheckEnable(gridViewDepartmentAlert, "SELECT_MANY")) return;
                isHeaderCheckBoxCheckedAlertRecive = !isHeaderCheckBoxCheckedAlertRecive;

                for (int i = 0; i < view.RowCount; i++)
                {
                    view.SetRowCellValue(i, view.Columns["CHECK_ALERT"], isHeaderCheckBoxCheckedAlertRecive);
                }

                view.Invalidate(); // Yêu cầu GridView vẽ lại để cập nhật checkbox ở tiêu đề
            }
            if (info.InColumn && info.Column.FieldName == "CHECK_SECURITY") // Thay YourColumnName bằng tên cột của bạn
            {
                //if (!CheckEnable(gridViewDepartmentAlert, "SELECT_MANY")) return;
                isHeaderCheckBoxCheckedSecurityRecive = !isHeaderCheckBoxCheckedSecurityRecive;

                for (int i = 0; i < view.RowCount; i++)
                {
                    view.SetRowCellValue(i, view.Columns["CHECK_SECURITY"], isHeaderCheckBoxCheckedSecurityRecive);
                }

                view.Invalidate(); // Yêu cầu GridView vẽ lại để cập nhật checkbox ở tiêu đề
            }
        }
    }
}