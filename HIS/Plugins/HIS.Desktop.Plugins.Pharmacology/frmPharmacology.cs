using DevExpress.XtraEditors.Repository;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
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

namespace HIS.Desktop.Plugins.Pharmacology
{
    public partial class frmPharmacology : HIS.Desktop.Utility.FormBase
    {
        private const string API_HIS_PHARMACOLOGY_GET = "api/HisPharmacology/Get";
        private const string API_HIS_PHARMACOLOGY_CREATE = "api/HisPharmacology/Create";
        private const string API_HIS_PHARMACOLOGY_UPDATE = "api/HisPharmacology/Update";
        private const string API_HIS_PHARMACOLOGY_DELETE = "api/HisPharmacology/Delete";
        private const string API_HIS_PHARMACOLOGY_CHANGE_LOCK = "api/HisPharmacology/ChangeLock";
        private const short IS_ACTIVE__TRUE = 1;

        //Cot du lieu
        private const string FIELD_NAME__PHARMACOLOGY_CODE = "PHARMACOLOGY_CODE";
        private const string FIELD_NAME__PHARMACOLOGY_NAME = "PHARMACOLOGY_NAME";
        private const string FIELD_NAME__IS_ACTIVE = "IS_ACTIVE";
        private const string FIELD_NAME__CREATE_TIME = "CREATE_TIME";
        private const string FIELD_NAME__CREATOR = "CREATOR";
        private const string FIELD_NAME__MODIFY_TIME = "MODIFY_TIME";
        private const string FIELD_NAME__MODIFIER = "MODIFIER";

        //Cot khong bind du lieu, chi de nhan dien trong cac event cua luoi
        private const string FIELD_NAME__NUM_ORDER = "NUM_ORDER";
        private const string FIELD_NAME__ACT_DELETE = "ACT_DELETE";
        private const string FIELD_NAME__ACT_LOCK = "ACT_LOCK";
        private const string FIELD_NAME__ACT_ACIN = "ACT_ACIN";

        private List<HIS_PHARMACOLOGY> pharmacologies = new List<HIS_PHARMACOLOGY>();

        //Nut xoa dang khoa: chi khac ban goc o cho khong bam duoc
        private RepositoryItemButtonEdit repositoryItemButtonDeleteDis;

        public frmPharmacology(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            InitEvents();
        }

        private void InitEvents()
        {
            try
            {
                this.Load += new EventHandler(frmPharmacology_Load);
                this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(gridView1_CustomUnboundColumnData);
                this.gridView1.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(gridView1_CustomColumnDisplayText);
                this.gridView1.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(gridView1_CustomRowCellEdit);
                this.simpleButton1.Click += new EventHandler(btnSearch_Click);
                this.simpleButton3.Click += new EventHandler(btnAdd_Click);
                this.simpleButton2.Click += new EventHandler(btnEdit_Click);
                this.simpleButton4.Click += new EventHandler(btnReload_Click);
                this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(gridView1_FocusedRowChanged);
                this.repositoryItemButtonDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(repositoryItemButtonDelete_ButtonClick);
                this.repositoryItemButtonLock.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(repositoryItemButtonLock_ButtonClick);
                this.repositoryItemButtonUnLock.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(repositoryItemButtonUnLock_ButtonClick);
                //Phim tat dung theo chu tren nut: Ctrl F tim, Ctrl N them, Ctrl S sua, Ctrl R lam lai
                this.KeyPreview = true;
                this.KeyDown += new KeyEventHandler(frmPharmacology_KeyDown);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmPharmacology_Load(object sender, EventArgs e)
        {
            try
            {
                InitGridColumn();
                LoadDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Gan field cho tung cot cua luoi. Cot STT va 3 cot nut khong bind du lieu
        /// </summary>
        private void InitGridColumn()
        {
            try
            {
                gridView1.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
                gridView1.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;

                gridColumn1.FieldName = FIELD_NAME__NUM_ORDER;
                gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
                gridColumn1.OptionsColumn.AllowEdit = false;
                gridColumn1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                gridColumn2.FieldName = FIELD_NAME__ACT_DELETE;
                gridColumn2.UnboundType = DevExpress.Data.UnboundColumnType.Object;

                gridColumn3.FieldName = FIELD_NAME__ACT_LOCK;
                gridColumn3.UnboundType = DevExpress.Data.UnboundColumnType.Object;

                gridColumn4.FieldName = FIELD_NAME__ACT_ACIN;
                gridColumn4.UnboundType = DevExpress.Data.UnboundColumnType.Object;

                gridColumn5.FieldName = FIELD_NAME__PHARMACOLOGY_CODE;
                gridColumn6.FieldName = FIELD_NAME__PHARMACOLOGY_NAME;
                gridColumn7.FieldName = FIELD_NAME__IS_ACTIVE;
                gridColumn8.FieldName = FIELD_NAME__CREATE_TIME;
                gridColumn9.FieldName = FIELD_NAME__CREATOR;
                gridColumn10.FieldName = FIELD_NAME__MODIFY_TIME;
                gridColumn11.FieldName = FIELD_NAME__MODIFIER;

                gridColumn7.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lay full danh sach duoc ly (HIS_PHARMACOLOGY) roi do len luoi.
        /// Co tu khoa o o tim thi loc theo ma hoac ten tren danh sach da lay
        /// </summary>
        private void LoadDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                gridControlPharmacology.DataSource = null;

                CommonParam param = new CommonParam();
                HisPharmacologyFilter filter = new HisPharmacologyFilter();
                filter.ORDER_FIELD = FIELD_NAME__PHARMACOLOGY_CODE;
                filter.ORDER_DIRECTION = "ASC";

                var data = new BackendAdapter(param).Get<List<HIS_PHARMACOLOGY>>(
                    API_HIS_PHARMACOLOGY_GET, ApiConsumers.MosConsumer, filter, param);
                pharmacologies = data ?? new List<HIS_PHARMACOLOGY>();

                string keyword = (textEdit1.Text ?? "").Trim();
                if (!String.IsNullOrEmpty(keyword))
                {
                    pharmacologies = pharmacologies.Where(o =>
                        (!String.IsNullOrEmpty(o.PHARMACOLOGY_CODE) && o.PHARMACOLOGY_CODE.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        || (!String.IsNullOrEmpty(o.PHARMACOLOGY_NAME) && o.PHARMACOLOGY_NAME.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
                }

                gridControlPharmacology.DataSource = pharmacologies;
                gridControlPharmacology.RefreshDataSource();
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToGrid: so dong duoc ly = " + pharmacologies.Count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lam lai: xoa trang 2 o nhap de chuan bi them moi
        /// </summary>
        private void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
                ClearInput();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Them moi 1 dong duoc ly theo ma / ten dang nhap
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                string code, name;
                NormalizeInput(out code, out name);
                if (!ValidInput(code, name, 0))
                    return;

                HIS_PHARMACOLOGY data = new HIS_PHARMACOLOGY();
                data.PHARMACOLOGY_CODE = code;
                data.PHARMACOLOGY_NAME = name;

                WaitingManager.Show();
                HIS_PHARMACOLOGY result = new BackendAdapter(param).Post<HIS_PHARMACOLOGY>(
                    API_HIS_PHARMACOLOGY_CREATE, ApiConsumers.MosConsumer, data, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    success = true;
                    ClearInput();
                    LoadDataToGrid();
                    FocusPharmacology(result.ID);
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Sua ma / ten cua dong duoc ly dang chon. Dong da khoa thi khong cho sua
        /// </summary>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_PHARMACOLOGY rowData = gridView1.GetFocusedRow() as HIS_PHARMACOLOGY;
                if (rowData == null)
                {
                    MessageBox.Show("Hãy chọn dòng dược lý cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!IsActive(rowData.IS_ACTIVE))
                {
                    MessageBox.Show("Dòng đang khóa nên không sửa được. Hãy mở khóa trước khi sửa.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string code, name;
                NormalizeInput(out code, out name);
                if (!ValidInput(code, name, rowData.ID))
                    return;

                //Gui lai dong dang chon, chi thay ma / ten de khong lam mat cac truong khac
                HIS_PHARMACOLOGY data = new HIS_PHARMACOLOGY();
                data.ID = rowData.ID;
                data.PHARMACOLOGY_CODE = code;
                data.PHARMACOLOGY_NAME = name;
                data.GROUP_CODE = rowData.GROUP_CODE;
                data.IS_ACTIVE = rowData.IS_ACTIVE;

                WaitingManager.Show();
                HIS_PHARMACOLOGY result = new BackendAdapter(param).Post<HIS_PHARMACOLOGY>(
                    API_HIS_PHARMACOLOGY_UPDATE, ApiConsumers.MosConsumer, data, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    success = true;
                    LoadDataToGrid();
                    FocusPharmacology(rowData.ID);
                    //Xoa sau khi da dua con tro ve dong vua sua, vi FocusedRowChanged do lai ma / ten vao 2 o nhap
                    ClearInput();
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiem tra ma / ten truoc khi goi api. skipId de bo qua chinh dong dang sua khi kiem tra trung ma
        /// </summary>
        private bool ValidInput(string code, string name, long skipId)
        {
            try
            {
                //Bat buoc nhap: de trong hoac go toan khoang trang deu khong duoc
                if (String.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Mã dược lý là bắt buộc, không được để trống hoặc chỉ có khoảng trắng.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textEdit2.Focus();
                    textEdit2.SelectAll();
                    return false;
                }

                if (String.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Tên dược lý là bắt buộc, không được để trống hoặc chỉ có khoảng trắng.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textEdit3.Focus();
                    textEdit3.SelectAll();
                    return false;
                }

                bool duplicateCode = pharmacologies != null && pharmacologies.Any(o => o.ID != skipId
                    && !String.IsNullOrEmpty(o.PHARMACOLOGY_CODE)
                    && String.Equals(o.PHARMACOLOGY_CODE.Trim(), code, StringComparison.OrdinalIgnoreCase));
                if (duplicateCode)
                {
                    MessageBox.Show("Mã dược lý đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textEdit2.Focus();
                    textEdit2.SelectAll();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //Kiem tra loi dở chừng thì coi nhu khong hop le, khong cho luu tiep
                return false;
            }
            return true;
        }

        /// <summary>
        /// Bo khoang trang thua o 2 o nhap va ghi lai len man hinh
        /// de nguoi dung thay dung gia tri se duoc luu
        /// </summary>
        private void NormalizeInput(out string code, out string name)
        {
            code = (textEdit2.Text ?? "").Trim();
            name = (textEdit3.Text ?? "").Trim();
            try
            {
                if (textEdit2.Text != code)
                {
                    textEdit2.Text = code;
                }

                if (textEdit3.Text != name)
                {
                    textEdit3.Text = name;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClearInput()
        {
            try
            {
                textEdit2.Text = "";
                textEdit3.Text = "";
                textEdit2.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Chon dong nao thi do ma / ten dong do vao o nhap de sua
        /// </summary>
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                HIS_PHARMACOLOGY rowData = gridView1.GetRow(e.FocusedRowHandle) as HIS_PHARMACOLOGY;
                if (rowData == null)
                    return;

                textEdit2.Text = rowData.PHARMACOLOGY_CODE ?? "";
                textEdit3.Text = rowData.PHARMACOLOGY_NAME ?? "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmPharmacology_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!e.Control)
                    return;

                if (e.KeyCode == Keys.F)
                {
                    e.Handled = true;
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.N)
                {
                    e.Handled = true;
                    btnAdd_Click(null, null);
                }
                else if (e.KeyCode == Keys.S)
                {
                    e.Handled = true;
                    btnEdit_Click(null, null);
                }
                else if (e.KeyCode == Keys.R)
                {
                    e.Handled = true;
                    btnReload_Click(null, null);
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
                if (e.IsGetData && e.Column.FieldName == FIELD_NAME__NUM_ORDER)
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Cot trang thai hien theo IS_ACTIVE, cot thoi gian hien theo dinh dang ngay gio
        /// </summary>
        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column == null)
                    return;

                if (e.Column.FieldName == FIELD_NAME__IS_ACTIVE)
                {
                    e.DisplayText = IsActive(e.Value) ? "Đang sử dụng" : "Đã khóa";
                }
                else if (e.Column.FieldName == FIELD_NAME__CREATE_TIME || e.Column.FieldName == FIELD_NAME__MODIFY_TIME)
                {
                    long time = Inventec.Common.TypeConvert.Parse.ToInt64((e.Value ?? "").ToString());
                    e.DisplayText = time > 0 ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(time) : "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dong dang su dung: hien nut Khoa, nut Xoa dung binh thuong.
        /// Dong da khoa: hien nut Mo khoa, nut Xoa bi disable
        /// </summary>
        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column == null)
                    return;

                bool isActive = IsActive(gridView1.GetRowCellValue(e.RowHandle, FIELD_NAME__IS_ACTIVE));
                if (e.Column.FieldName == FIELD_NAME__ACT_LOCK)
                {
                    e.RepositoryItem = isActive ? repositoryItemButtonLock : repositoryItemButtonUnLock;
                }
                else if (e.Column.FieldName == FIELD_NAME__ACT_DELETE)
                {
                    e.RepositoryItem = isActive ? repositoryItemButtonDelete : GetRepositoryItemButtonDeleteDis();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsActive(object value)
        {
            return Inventec.Common.TypeConvert.Parse.ToInt16((value ?? "").ToString()) == IS_ACTIVE__TRUE;
        }

        /// <summary>
        /// Nut xoa o trang thai khong bam duoc, tao 1 lan roi dung lai
        /// </summary>
        private RepositoryItemButtonEdit GetRepositoryItemButtonDeleteDis()
        {
            try
            {
                if (repositoryItemButtonDeleteDis == null)
                {
                    repositoryItemButtonDeleteDis = new RepositoryItemButtonEdit();
                    repositoryItemButtonDeleteDis.Assign(repositoryItemButtonDelete);
                    repositoryItemButtonDeleteDis.Name = "repositoryItemButtonDeleteDis";
                    repositoryItemButtonDeleteDis.ReadOnly = true;
                    if (repositoryItemButtonDeleteDis.Buttons.Count > 0)
                    {
                        repositoryItemButtonDeleteDis.Buttons[0].Enabled = false;
                        repositoryItemButtonDeleteDis.Buttons[0].ToolTip = "Dòng đang khóa nên không xóa được";
                    }
                    gridControlPharmacology.RepositoryItems.Add(repositoryItemButtonDeleteDis);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return repositoryItemButtonDeleteDis;
        }

        /// <summary>
        /// Xoa dong duoc ly dang chon. Dong da khoa thi khong cho xoa
        /// </summary>
        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_PHARMACOLOGY rowData = gridView1.GetFocusedRow() as HIS_PHARMACOLOGY;
                if (rowData == null)
                    return;

                //Chan them lan nua o day cho chac, ngoai luoi nut da bi disable khi dong dang khoa
                if (!IsActive(rowData.IS_ACTIVE))
                {
                    MessageBox.Show("Dòng đang khóa nên không xóa được. Hãy mở khóa trước khi xóa.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                        HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                        "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                success = new BackendAdapter(param).Post<bool>(API_HIS_PHARMACOLOGY_DELETE,
                    ApiConsumers.MosConsumer, rowData.ID, param);
                WaitingManager.Hide();

                if (success)
                    LoadDataToGrid();

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoa dong duoc ly dang chon (dang su dung -> khoa)
        /// </summary>
        private void repositoryItemButtonLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            ChangeLockPharmacology(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong);
        }

        /// <summary>
        /// Mo khoa dong duoc ly dang chon (dang khoa -> su dung lai)
        /// </summary>
        private void repositoryItemButtonUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            ChangeLockPharmacology(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong);
        }

        /// <summary>
        /// Khoa / mo khoa dung cung 1 api ChangeLock, backend tu dao trang thai IS_ACTIVE
        /// </summary>
        private void ChangeLockPharmacology(HIS.Desktop.LibraryMessage.Message.Enum confirmMessage)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_PHARMACOLOGY rowData = gridView1.GetFocusedRow() as HIS_PHARMACOLOGY;
                if (rowData == null)
                    return;

                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(confirmMessage),
                        "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                HIS_PHARMACOLOGY result = new BackendAdapter(param).Post<HIS_PHARMACOLOGY>(
                    API_HIS_PHARMACOLOGY_CHANGE_LOCK, ApiConsumers.MosConsumer, rowData.ID, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    success = true;
                    LoadDataToGrid();
                    FocusPharmacology(rowData.ID);
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dua con tro ve dung dong vua xu ly sau khi load lai luoi
        /// </summary>
        private void FocusPharmacology(long pharmacologyId)
        {
            try
            {
                if (pharmacologies == null || pharmacologies.Count == 0)
                    return;

                int index = pharmacologies.FindIndex(o => o.ID == pharmacologyId);
                if (index < 0)
                    return;

                int rowHandle = gridView1.GetRowHandle(index);
                if (rowHandle >= 0)
                {
                    gridView1.FocusedRowHandle = rowHandle;
                    gridView1.MakeRowVisible(rowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Mo man hinh danh sach hoat chat cua dong duoc ly dang chon
        /// </summary>
        private void repositoryItemButtonCheck_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                frmPharmacologyAcin frmAcin = new frmPharmacologyAcin(GetFocusedPharmacologyId());
                frmAcin.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lay ID cua dong duoc ly dang chon tren luoi
        /// </summary>
        private long GetFocusedPharmacologyId()
        {
            long pharmacologyId = 0;
            try
            {
                HIS_PHARMACOLOGY focusedRow = gridView1.GetFocusedRow() as HIS_PHARMACOLOGY;
                if (focusedRow != null)
                {
                    pharmacologyId = focusedRow.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return pharmacologyId;
        }
    }
}
