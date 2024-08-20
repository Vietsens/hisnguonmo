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
        List<HIS_DEPARTMENT> listDepartmentAlert;
        List<HIS_DEPARTMENT> listDepartmentRecive;
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
                listDepartmentAlert = new List<HIS_DEPARTMENT>();
                listDepartmentRecive = new List<HIS_DEPARTMENT>();
                SetCaptionByLanguageKey();
                LoadDefault();

                FillDataToControl();
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
        bool isRecive = false;
        private void FillDataToControl()
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
                    isRecive = false;
                    LoadPaging(new CommonParam(0, pageSize));
                    CommonParam param = new CommonParam();
                    param.Limit = rowCountAlert;
                    param.Count = dataTotalAlert;
                    ucPaging1.Init(LoadPaging, param, pageSize, gridControlDepartmentAlert);
                    // loa danh sach khoa nhan bao dong
                    int pageSize2 = 0;
                    if (ucPaging2.pagingGrid != null)
                    {
                        pageSize2 = ucPaging2.pagingGrid.PageSize;
                    }
                    else
                    {
                        pageSize2 = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                    }
                    isRecive = true;
                    LoadPaging(new CommonParam(0, pageSize2));
                    CommonParam paramRecive = new CommonParam();
                    param.Limit = rowCountRecive;
                    param.Count = dataTotalRecive;
                    ucPaging2.Init(LoadPaging, paramRecive, pageSize2, gridControlDepartmentRecive);

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

        private void LoadPaging(object param)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Load data to list");
                CommonParam paramCommon;

                var startPage =  ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                paramCommon = new CommonParam(startPage, limit);
                this.gridControlDepartmentAlert.BeginUpdate();
                this.gridControlDepartmentRecive.BeginUpdate();
                
                Inventec.Core.ApiResultObject<List<HIS_DEPARTMENT>> apiResult = null;
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_DEPARTMENT>>("/api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null)
                    {
                        
                        gridControlDepartmentAlert.DataSource = data;
                        gridControlDepartmentRecive.DataSource = data;
                        rowCountAlert = (data == null ? 0 : data.Count);
                        dataTotalAlert = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        rowCountRecive = (data == null ? 0 : data.Count);
                        dataTotalRecive = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                    }
                }
                gridControlDepartmentAlert.EndUpdate();
                gridControlDepartmentRecive.EndUpdate();
            }
            catch (Exception ex)
            {
                
                LogSystem.Warn(ex);
            }
        }

        private void LoadDefault()
        {
            try
            {
                List<Model.DepartmentType> listType = new List<Model.DepartmentType>();
                listType.Add(new Model.DepartmentType() { ID = 1, DEPARTMENT_TYPE_CODE = "DEPARTMENT-CREATE", DEPARTMENT_TYPE_NAME = "Khoa Báo Động" });
                listType.Add(new Model.DepartmentType() { ID = 2, DEPARTMENT_TYPE_CODE = "DEPARTMENT-RECIVE", DEPARTMENT_TYPE_NAME = "Khoa Nhận" });
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_TYPE_NAME", "", 190, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_TYPE_NAME", "ID", columnInfos, false, 100);
                ControlEditorLoader.Load(cbotype, listType, controlEditorADO);
                cbotype.EditValue = 1;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region GRID VIEW
        private void gridViewDepartmentAlert_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.FieldName == "SELECT_MANY") // Đảm bảo đây là cột bạn muốn thêm checkbox
                {
                    //e.Info.InnerElements.Clear();  // Xóa nội dung cũ
                    //e.Painter.DrawObject(e.Info);  // Vẽ lại caption gốc mà không có text

                    //// Thiết lập vị trí và kích thước của checkbox
                    //Rectangle checkBoxRect = e.Bounds;
                    //checkBoxRect.Width = 15;
                    //checkBoxRect.Height = 15;
                    //checkBoxRect.X = e.Bounds.X + (e.Bounds.Width - checkBoxRect.Width) / 2;
                    //checkBoxRect.Y = e.Bounds.Y + (e.Bounds.Height - checkBoxRect.Height) / 2;

                    //// Lấy trạng thái checkbox hiện tại
                    //bool isChecked = e.Column.Tag != null && (bool)e.Column.Tag;

                    //// Vẽ checkbox lên header
                    //DevExpress.XtraEditors.Drawing.CheckEditPainter painter = new DevExpress.XtraEditors.Drawing.CheckEditPainter();
                    //DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo viewInfo = new DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo(new RepositoryItemCheckEdit());
                    //viewInfo.EditValue = isChecked;
                    //DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(viewInfo, e.Cache, checkBoxRect);
                    //painter.Draw(args);

                    //e.Handled = true;  // Chặn vẽ tiếp theo của DevExpress
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void cbotype_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if(cbotype.EditValue != null )
                {
                    EnableControlSelect(Convert.ToInt64(cbotype.EditValue));
                    ClearDataSelected(gridViewDepartmentAlert,listDepartmentAlert, checkboxStates);
                    ClearDataSelected(gridViewDepartmentRecive,listDepartmentRecive, checkboxStatesRecive);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ClearDataSelected(GridView control,List<HIS_DEPARTMENT>list,Dictionary<long,bool> dic)
        {
            try
            {
                // Duyệt qua các dòng đã chọn và xóa chúng khỏi GridView
                foreach (var row in list)
                {
                    long rowKey = row.ID;

                    // Xóa dòng khỏi GridView
                    control.DeleteRow(control.GetRowHandle(list.IndexOf(row)));

                    // Xóa trạng thái checkbox từ từ điển
                    if (dic.ContainsKey(rowKey))
                    {
                        dic.Remove(rowKey);
                    }
                }

                // Xóa tất cả các dòng đã chọn trong danh sách
                list.Clear();

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
        }

        private void DisableCheckbox(GridView gridView)
        {
            RepositoryItemCheckEdit checkEdit = (RepositoryItemCheckEdit)gridView.Columns["SELECT_MANY"].ColumnEdit;
            checkEdit.ReadOnly = true; // Không cho phép chỉnh sửa (disable)
            checkEdit.AllowFocused = false;
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
        #region GRID DEPARTMENT ALERT
        private Dictionary<long, bool> checkboxStates = new Dictionary<long, bool>();
        private void gridViewDepartmentAlert_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "SELECT_MANY") // Thay "CheckColumn" bằng tên cột checkbox của bạn
                {
                    if (e.Value == null)
                        return;
                    
                    HIS_DEPARTMENT rowData = (HIS_DEPARTMENT)gridViewDepartmentAlert.GetRow(e.RowHandle);
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


                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void UpdateSelectedRows(HIS_DEPARTMENT rowData, bool isChecked)
        {
            if (isChecked)
            {
                if (!listDepartmentAlert.Contains(rowData))
                {
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
                if (e.Column.FieldName == "SELECT_MANY") // Thay "CheckColumn" bằng tên cột checkbox của bạn
                {
                    var rowData = gridViewDepartmentAlert.GetRow(e.RowHandle) as HIS_DEPARTMENT;
                    if (rowData != null && checkboxStates.ContainsKey(rowData.ID))
                    {
                        e.CellValue = checkboxStates[rowData.ID];
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
                if (e.Column.FieldName == "SELECT_MANY")
                {
                    var rowData = (HIS_DEPARTMENT)e.Row;
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
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
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

                    HIS_DEPARTMENT rowData = (HIS_DEPARTMENT)gridViewDepartmentRecive.GetRow(e.RowHandle);
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


                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void UpdateSelectedRowsRecive(HIS_DEPARTMENT rowData, bool isChecked)
        {
            if (isChecked)
            {
                if (!listDepartmentRecive.Contains(rowData))
                {
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
                    var rowData = gridViewDepartmentRecive.GetRow(e.RowHandle) as HIS_DEPARTMENT;
                    if (rowData != null && checkboxStatesRecive.ContainsKey(rowData.ID))
                    {
                        e.CellValue = checkboxStatesRecive[rowData.ID];
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
                if (e.Column.FieldName == "SELECT_MANY")
                {
                    var rowData = (HIS_DEPARTMENT)e.Row;
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
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var alert = listDepartmentAlert.ToList();
                var recive = listDepartmentRecive.ToList();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
    }
}