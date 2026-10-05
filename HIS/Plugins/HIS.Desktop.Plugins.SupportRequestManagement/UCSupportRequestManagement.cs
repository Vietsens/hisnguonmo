/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities.RemoteSupport;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupportRequestManagement
{
    /// <summary>
    /// vCong57682 - Man hinh Quan ly yeu cau ho tro.
    /// Pham vi du lieu do phia may chu quyet dinh: nguoi khong co tich "Quan tri"
    /// chi nhan ve yeu cau do chinh minh tao. Co IsAdmin tra ve chi dung de an/hien nut.
    /// </summary>
    public partial class UCSupportRequestManagement : UserControlBase
    {
        const string CONFIG_KEY__SUPPORT_REQUEST_ENABLE = "MOS.HIS_SUPPORT_REQUEST.ENABLE";
        const string CONFIG_KEY__VPLUS_CUSTOMER_INFO = "HIS.Desktop.VPLUS_CUSTOMER_INFO";
        // Duong dan tao yeu cau tren he thong cua cong ty - giu nguyen nhu man Ctrl+F2 dang dung.
        const string CRM_URL__CREATE_REQUEST = "/ords/vplus/viec/viec/";

        Inventec.Desktop.Common.Modules.Module currentModule;
        string currentLoginName = "";
        bool isEnable = false;
        bool isAdmin = false;
        int startPage = 0;
        int rowCount = 0;
        int dataTotal = 0;

        List<MOS.SDO.HisSupportRequestViewSDO> listData = new List<MOS.SDO.HisSupportRequestViewSDO>();
        List<MOS.SDO.HisSupportRequestCmtViewSDO> listCmt = new List<MOS.SDO.HisSupportRequestCmtViewSDO>();
        MOS.SDO.HisSupportRequestViewSDO currentRow = null;

        // Ban "tat" (mau xam) cua cac nut trong luoi. Gan RepositoryItem = null thi luoi quay ve
        // ColumnEdit cua cot -> nut trong y het nut bat, nguoi dung khong phan biet duoc.
        RepositoryItemButtonEdit repCmtEditDisable;
        RepositoryItemButtonEdit repCmtDeleteDisable;
        RepositoryItemButtonEdit repDeleteDisable;

        public UCSupportRequestManagement(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.currentLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                InitDisableButtons();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #region Khoi tao

        private void InitDisableButtons()
        {
            repCmtEditDisable = CreateDisableButton(new EditorButton(ButtonPredefines.Glyph, "Sửa", -1, false, true, false, ImageLocation.MiddleCenter, null));
            repCmtDeleteDisable = CreateDisableButton(new EditorButton(ButtonPredefines.Glyph, "Xoá", -1, false, true, false, ImageLocation.MiddleCenter, null));
            gridCmt.RepositoryItems.AddRange(new RepositoryItem[] { repCmtEditDisable, repCmtDeleteDisable });

            // Chi tat nut thi skin ve chu xam gan giong nut bat -> to mau ro: bat = xanh dam, tat = xam nhat.
            foreach (var rep in new[] { repCmtEdit, repCmtDelete })
            {
                var btn = rep.Buttons[0];
                btn.Appearance.ForeColor = Color.FromArgb(0, 84, 166);
                btn.Appearance.Font = new Font(btn.Appearance.Font, FontStyle.Bold);
                btn.Appearance.Options.UseForeColor = true;
                btn.Appearance.Options.UseFont = true;
            }
            foreach (var rep in new[] { repCmtEditDisable, repCmtDeleteDisable })
            {
                var btn = rep.Buttons[0];
                btn.AppearanceDisabled.ForeColor = Color.FromArgb(190, 190, 190);
                btn.AppearanceDisabled.Options.UseForeColor = true;
            }

            var btnDelete = new EditorButton(ButtonPredefines.Delete);
            btnDelete.Enabled = false;
            repDeleteDisable = CreateDisableButton(btnDelete);
            gridList.RepositoryItems.Add(repDeleteDisable);
        }

        private static RepositoryItemButtonEdit CreateDisableButton(EditorButton button)
        {
            var rep = new RepositoryItemButtonEdit();
            rep.AutoHeight = false;
            rep.Buttons.Clear();
            rep.Buttons.Add(button);
            rep.TextEditStyle = TextEditStyles.HideTextEditor;
            return rep;
        }

        private void UCSupportRequestManagement_Load(object sender, EventArgs e)
        {
            try
            {
                isEnable = HisConfigs.Get<string>(CONFIG_KEY__SUPPORT_REQUEST_ENABLE) == "1";
                if (!isEnable)
                {
                    // Cau hinh tat -> khong goi bat ky dich vu nao.
                    pnlFilter.Enabled = false;
                    pnlDetail.Enabled = false;
                    pnlList.Enabled = false;
                    gridList.DataSource = null;
                    XtraMessageBox.Show("Chức năng yêu cầu hỗ trợ nội bộ chưa được bật tại đơn vị", "Thông báo");
                    return;
                }

                InitFilterControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitFilterControl()
        {
            try
            {
                dtFrom.DateTime = DateTime.Now.Date.AddDays(-7);
                dtTo.DateTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

                // Danh sach 5 trang thai dung san tren giao dien, khong goi dich vu danh muc.
                cboStt.Properties.Items.Clear();
                cboStt.Properties.Items.Add(SupportRequestStt.MOI_TAO, SupportRequestStt.GetName(SupportRequestStt.MOI_TAO), CheckState.Checked, true);
                cboStt.Properties.Items.Add(SupportRequestStt.DANG_XU_LY, SupportRequestStt.GetName(SupportRequestStt.DANG_XU_LY), CheckState.Checked, true);
                cboStt.Properties.Items.Add(SupportRequestStt.HOAN_THANH, SupportRequestStt.GetName(SupportRequestStt.HOAN_THANH), CheckState.Unchecked, true);
                cboStt.Properties.Items.Add(SupportRequestStt.TU_CHOI, SupportRequestStt.GetName(SupportRequestStt.TU_CHOI), CheckState.Unchecked, true);
                cboStt.Properties.Items.Add(SupportRequestStt.DA_CHUYEN_CTY, SupportRequestStt.GetName(SupportRequestStt.DA_CHUYEN_CTY), CheckState.Unchecked, true);

                InitLookUp(cboDept, BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .Where(o => o.IS_ACTIVE == 1)
                    .Select(o => new ComboADO { CODE = o.ID.ToString(), NAME = o.DEPARTMENT_NAME })
                    .OrderBy(o => o.NAME).ToList(), "Khoa");

                InitLookUp(cboBranch, BackendDataWorker.Get<HIS_BRANCH>()
                    .Where(o => o.IS_ACTIVE == 1)
                    .Select(o => new ComboADO { CODE = o.ID.ToString(), NAME = o.BRANCH_NAME })
                    .OrderBy(o => o.NAME).ToList(), "Cơ sở");

                var allEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>()
                    .Where(o => o.IS_ACTIVE == 1 && !string.IsNullOrWhiteSpace(o.LOGINNAME))
                    .Select(o => new ComboADO { CODE = o.LOGINNAME, NAME = string.IsNullOrWhiteSpace(o.TDL_USERNAME) ? o.LOGINNAME : o.TDL_USERNAME })
                    .OrderBy(o => o.NAME).ToList();
                InitEmployeeLookUp(cboRequester, allEmployee, "Người tạo");
                InitEmployeeLookUp(cboReceiver, allEmployee, "Người tiếp nhận");
                InitEmployeeLookUp(cboAssigneeFilter, GetAdminEmployees(), "Người được chỉ định");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nhan vien thuoc bo phan quan tri = co o tich "Quan tri" tren danh muc Nhan vien.
        /// O tich luu gia tri 1 khi bat va DE TRONG khi tat -> phai so bang 1.
        /// </summary>
        internal static List<ComboADO> GetAdminEmployees()
        {
            try
            {
                return BackendDataWorker.Get<HIS_EMPLOYEE>()
                    .Where(o => o.IS_ADMIN == 1 && o.IS_ACTIVE == 1 && !string.IsNullOrWhiteSpace(o.LOGINNAME))
                    .Select(o => new ComboADO { CODE = o.LOGINNAME, NAME = string.IsNullOrWhiteSpace(o.TDL_USERNAME) ? o.LOGINNAME : o.TDL_USERNAME })
                    .OrderBy(o => o.NAME).ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return new List<ComboADO>();
            }
        }

        internal static void InitLookUp(LookUpEdit cbo, List<ComboADO> source, string caption)
        {
            cbo.Properties.DataSource = source;
            cbo.Properties.DisplayMember = "NAME";
            cbo.Properties.ValueMember = "CODE";
            cbo.Properties.Columns.Clear();
            cbo.Properties.Columns.Add(new LookUpColumnInfo("NAME", caption, 100));
            cbo.Properties.NullText = "";
            cbo.EditValue = null;
        }

        /// <summary>
        /// O chon nguoi: hien 2 cot Tai khoan + Ho ten, go chu thi loc kieu "co chua"
        /// (go ten dang nhap hay ho ten deu ra). LookUpEdit cu chi loc theo chu dau cua ho ten nen
        /// go ten dang nhap khong tim thay nguoi da khai ho ten.
        /// GridLookUpEdit 15.2 chi loc tren cot DisplayMember -> dung DISPLAY ghep ca ho ten lan ten dang nhap.
        /// </summary>
        internal static void InitEmployeeLookUp(GridLookUpEdit cbo, List<ComboADO> source, string caption)
        {
            cbo.Properties.DataSource = source;
            cbo.Properties.DisplayMember = "DISPLAY";
            cbo.Properties.ValueMember = "CODE";
            cbo.Properties.PopupFilterMode = PopupFilterMode.Contains;
            cbo.Properties.ImmediatePopup = true;
            cbo.Properties.TextEditStyle = TextEditStyles.Standard;
            cbo.Properties.PopupFormSize = new System.Drawing.Size(Math.Max(cbo.Width, 320), 250);

            var view = cbo.Properties.View;
            view.Columns.Clear();
            view.OptionsView.ShowIndicator = false;
            view.OptionsView.ShowColumnHeaders = true;
            view.OptionsSelection.EnableAppearanceFocusedCell = false;
            var colCode = view.Columns.AddField("CODE");
            colCode.Caption = "Tài khoản";
            colCode.Width = 100;
            colCode.Visible = true;
            colCode.VisibleIndex = 0;
            var colName = view.Columns.AddField("NAME");
            colName.Caption = caption;
            colName.Width = 220;
            colName.Visible = true;
            colName.VisibleIndex = 1;

            cbo.Properties.NullText = "";
            cbo.EditValue = null;
        }

        /// <summary>
        /// Xoa gia tri o chon. Bam nut xoa khi o chua co focus roi click ra cho khac thi GridLookUpEdit
        /// lay lai gia tri cu (da kiem chung tren DevExpress 15.2) -> phai bo co IsModified.
        /// </summary>
        internal static void ClearLookUp(LookUpEditBase cbo)
        {
            cbo.EditValue = null;
            cbo.IsModified = false;
        }

        private void cboFilter_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    ClearLookUp((LookUpEditBase)sender);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Tra cuu danh sach

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();

                int pageSize = ucPaging.pagingGrid != null
                    ? ucPaging.pagingGrid.PageSize
                    : ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                // Thieu khoa cau hinh thi pageSize = 0, may chu se tra ve danh sach rong.
                if (pageSize <= 0) pageSize = 50;

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, pageSize, this.gridList);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                HisSupportRequestViewFilter filter = new HisSupportRequestViewFilter();
                SetFilter(ref filter);

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon)
                    .Post<MOS.SDO.HisSupportRequestViewResultSDO>(
                        ApiUrl.HIS_SUPPORT_REQUEST__GET_VIEW,
                        ApiConsumers.MosConsumer, filter, paramCommon);

                listData = (result != null && result.Data != null)
                    ? result.Data
                    : new List<MOS.SDO.HisSupportRequestViewSDO>();

                // Co quyen quan tri do may chu quyet dinh, giao dien chi dung de an/hien nut.
                isAdmin = result != null && result.IsAdmin;

                rowCount = listData.Count;
                dataTotal = result != null ? result.Count : 0;

                LogSystem.Info(string.Format(
                    "SupportRequest.GetView: Start={0}, Limit={1}, TimeFrom={2}, TimeTo={3}, Stt={4}, DeptId={5}, BranchId={6} => KetQua={7}, SoDong={8}, TongSo={9}, QuanTri={10}",
                    startPage, limit, filter.REQUEST_TIME_FROM, filter.REQUEST_TIME_TO,
                    filter.SUPPORT_REQUEST_STTs == null ? "null" : string.Join(",", filter.SUPPORT_REQUEST_STTs),
                    filter.REQUEST_DEPARTMENT_ID, filter.BRANCH_ID,
                    result == null ? "null" : "co", rowCount, dataTotal, isAdmin));

                ClearDetail();
                gridList.DataSource = null;
                gridList.DataSource = listData;

                ApplyAdminVisibility();

                // Gan nguon du lieu khong phai luc nao cung sinh su kien doi dong:
                // luoi mot dong thi dong dang chon van la dong 0, bam vao no khong doi gi,
                // nen phai tu nap chi tiet cua dong dang duoc chon.
                ShowFocusedRowDetail();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisSupportRequestViewFilter filter)
        {
            try
            {
                filter.REQUEST_TIME_FROM = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFrom.DateTime);
                filter.REQUEST_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTo.DateTime);

                List<short> stts = new List<short>();
                foreach (CheckedListBoxItem item in cboStt.Properties.Items)
                {
                    if (item.CheckState == CheckState.Checked)
                        stts.Add(Convert.ToInt16(item.Value));
                }
                if (stts.Count > 0) filter.SUPPORT_REQUEST_STTs = stts;

                if (cboDept.EditValue != null)
                    filter.REQUEST_DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDept.EditValue.ToString());
                if (cboBranch.EditValue != null)
                    filter.BRANCH_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBranch.EditValue.ToString());

                // Ba o loc theo nguoi chi co tac dung voi nguoi co quyen quan tri.
                // May chu van tu ep lai dieu kien, day chi la de khoi gui du lieu thua.
                if (cboRequester.EditValue != null)
                    filter.REQUEST_LOGINNAME__EXACT = cboRequester.EditValue.ToString();
                if (cboReceiver.EditValue != null)
                    filter.RECEIVE_LOGINNAME__EXACT = cboReceiver.EditValue.ToString();
                if (cboAssigneeFilter.EditValue != null)
                    filter.ASSIGNEE_LOGINNAME__EXACT = cboAssigneeFilter.EditValue.ToString();

                if (!string.IsNullOrWhiteSpace(txtKeyword.Text))
                    filter.KEY_WORD = txtKeyword.Text.Trim();

                // Khong gui ORDER_FIELD: theo thiet ke B.3.2.4 buoc 5, phia may chu
                // tu sap xep theo thoi diem tao giam dan. Mot so dich vu MOS kiem ten cot
                // sap xep theo danh sach cho phep, ten khong khop thi tra ve rong.
                //filter.ORDER_FIELD = "REQUEST_TIME";
                //filter.ORDER_DIRECTION = "DESC";
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FillDataToGrid();
        }

        private void btnMyTask_Click(object sender, EventArgs e)
        {
            try
            {
                cboAssigneeFilter.EditValue = currentLoginName;
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Hien thi chi tiet

        private void gridViewList_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            ShowFocusedRowDetail();
        }

        private void ShowFocusedRowDetail()
        {
            try
            {
                currentRow = gridViewList.GetFocusedRow() as MOS.SDO.HisSupportRequestViewSDO;
                if (currentRow == null)
                {
                    ClearDetail();
                    return;
                }
                FillDetail(currentRow);
                LoadComment(currentRow.ID);
                LoadFile(currentRow.ID);
                RefreshButtonState();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ClearDetail()
        {
            try
            {
                currentRow = null;
                txtTitle.Text = "";
                memContent.Text = "";
                txtContact.Text = "";
                lblAnydesk.Text = "";
                lblModule.Text = "";
                lblAssignee.Text = "";
                memFinishNote.Text = "";
                gridFile.DataSource = null;
                gridCmt.DataSource = null;
                tabComment.Text = "Trao đổi";
                RefreshButtonState();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FillDetail(MOS.SDO.HisSupportRequestViewSDO row)
        {
            try
            {
                txtTitle.Text = row.TITLE;
                memContent.Text = row.CONTENT;
                txtContact.Text = row.CONTACT_INFO;
                lblAnydesk.Text = row.ANYDESK_ID;
                lblModule.Text = row.MODULE_LINK;
                lblAssignee.Text = string.IsNullOrWhiteSpace(row.ASSIGNEE_USERNAME) ? "(chưa chỉ định)" : row.ASSIGNEE_USERNAME;
                memFinishNote.Text = row.FINISH_NOTE;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadComment(long supportRequestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSupportRequestCmtFilter filter = new HisSupportRequestCmtFilter();
                filter.SUPPORT_REQUEST_ID = supportRequestId;
                filter.ORDER_FIELD = "CMT_TIME";
                filter.ORDER_DIRECTION = "ASC";

                listCmt = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<List<MOS.SDO.HisSupportRequestCmtViewSDO>>(
                        ApiUrl.HIS_SUPPORT_REQUEST_CMT__GET_VIEW,
                        ApiConsumers.MosConsumer, filter, param)
                    ?? new List<MOS.SDO.HisSupportRequestCmtViewSDO>();

                gridCmt.DataSource = null;
                gridCmt.DataSource = listCmt;
                tabComment.Text = string.Format("Trao đổi ({0})", listCmt.Count);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadFile(long supportRequestId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSupportRequestFileFilter filter = new HisSupportRequestFileFilter();
                filter.SUPPORT_REQUEST_ID = supportRequestId;

                var files = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<List<HIS_SUPPORT_REQUEST_FILE>>(
                        ApiUrl.HIS_SUPPORT_REQUEST_FILE__GET,
                        ApiConsumers.MosConsumer, filter, param);

                gridFile.DataSource = null;
                gridFile.DataSource = files;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewFile_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var row = gridViewFile.GetFocusedRow() as HIS_SUPPORT_REQUEST_FILE;
                if (row == null || string.IsNullOrWhiteSpace(row.FILE_URL)) return;

                string url = string.Format("{0}{1}",
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS,
                    row.FILE_URL.Replace("\\", "/"));
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                XtraMessageBox.Show("Không mở được tệp đính kèm", "Thông báo");
            }
        }

        #endregion

        #region Dinh dang hien thi

        private void gridViewList_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column == colRequestTime || e.Column == colFinishTime)
                    e.DisplayText = FormatTimeNumber(e.Value);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewCmt_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column == colCmtTime)
                    e.DisplayText = FormatTimeNumber(e.Value);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewFile_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column != colFileSize) return;
                if (e.Value == null) { e.DisplayText = ""; return; }
                long size = Inventec.Common.TypeConvert.Parse.ToInt64(e.Value.ToString());
                e.DisplayText = size >= 1048576
                    ? string.Format("{0:0.#} MB", size / 1048576m)
                    : string.Format("{0:0} KB", Math.Ceiling(size / 1024m));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private static string FormatTimeNumber(object value)
        {
            if (value == null) return "";
            long t = Inventec.Common.TypeConvert.Parse.ToInt64(value.ToString());
            if (t <= 0) return "";
            return Inventec.Common.DateTime.Convert.TimeNumberToTimeString(t);
        }

        /// <summary>Dong he thong (sinh ra khi chuyen viec) hien khac mau va khong cho sua/xoa.</summary>
        private void gridViewCmt_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var row = gridViewCmt.GetRow(e.RowHandle) as MOS.SDO.HisSupportRequestCmtViewSDO;
                if (row != null && row.CMT_TYPE == 2)
                {
                    e.Appearance.BackColor = Color.FromArgb(246, 241, 251);
                    e.Appearance.ForeColor = Color.FromArgb(91, 63, 136);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewCmt_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column != colCmtEdit && e.Column != colCmtDelete) return;
                var row = gridViewCmt.GetRow(e.RowHandle) as MOS.SDO.HisSupportRequestCmtViewSDO;
                // Chi sua/xoa duoc dong do chinh minh viet VA la dong nguoi dung.
                bool allow = row != null && row.IS_MINE && row.CMT_TYPE != 2;
                if (!allow) e.RepositoryItem = e.Column == colCmtEdit ? repCmtEditDisable : repCmtDeleteDisable;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column != colDelete) return;
                var row = gridViewList.GetRow(e.RowHandle) as MOS.SDO.HisSupportRequestViewSDO;
                if (!CanEditByCreator(row)) e.RepositoryItem = repDeleteDisable;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Trang thai nut

        /// <summary>
        /// Nguoi tao chi duoc sua/xoa khi yeu cau con "Moi tao" VA chua co nguoi tiep nhan.
        /// Can cu vao RECEIVE_LOGINNAME, KHONG can cu vao ASSIGNEE_LOGINNAME.
        /// </summary>
        private bool CanEditByCreator(MOS.SDO.HisSupportRequestViewSDO row)
        {
            return row != null
                && row.REQUEST_LOGINNAME == currentLoginName
                && row.SUPPORT_REQUEST_STT == SupportRequestStt.MOI_TAO
                && string.IsNullOrWhiteSpace(row.RECEIVE_LOGINNAME);
        }

        private void ApplyAdminVisibility()
        {
            try
            {
                lblRequester.Visible = isAdmin;
                cboRequester.Visible = isAdmin;
                lblReceiver.Visible = isAdmin;
                cboReceiver.Visible = isAdmin;
                lblAssigneeFilter.Visible = isAdmin;
                cboAssigneeFilter.Visible = isAdmin;
                btnMyTask.Visible = isAdmin;

                btnReceive.Visible = isAdmin;
                btnFinish.Visible = isAdmin;
                btnReject.Visible = isAdmin;
                btnTransfer.Visible = isAdmin;
                btnForward.Visible = isAdmin;

                lblFinishNoteCap.Visible = isAdmin;
                memFinishNote.Visible = isAdmin;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void RefreshButtonState()
        {
            try
            {
                bool hasRow = currentRow != null;
                bool canEdit = CanEditByCreator(currentRow);
                bool closed = hasRow && SupportRequestStt.IsClosed(currentRow.SUPPORT_REQUEST_STT);

                txtTitle.Properties.ReadOnly = !canEdit;
                memContent.Properties.ReadOnly = !canEdit;
                txtContact.Properties.ReadOnly = !canEdit;
                btnSave.Enabled = canEdit;
                btnAddCmt.Enabled = hasRow;

                btnReceive.Enabled = hasRow && isAdmin && currentRow.SUPPORT_REQUEST_STT == SupportRequestStt.MOI_TAO;
                btnFinish.Enabled = hasRow && isAdmin && currentRow.SUPPORT_REQUEST_STT == SupportRequestStt.DANG_XU_LY;
                btnReject.Enabled = btnFinish.Enabled;
                // Chuyen viec chay duoc o ca hai giai doan, mien la viec chua ket thuc.
                btnTransfer.Enabled = hasRow && isAdmin && !closed;
                // Chong chuyen tiep hai lan can cu vao THOI DIEM CHUYEN TIEP, khong phai ma doi chieu,
                // vi he thong cua cong ty khong tra ve ma (PTTK_57682 muc A.3.6, quy tac QT11).
                btnForward.Enabled = hasRow && isAdmin && !closed
                    && !currentRow.FORWARD_TIME.HasValue;
                memFinishNote.Properties.ReadOnly = !(hasRow && isAdmin && !closed);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Thao tac

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentRow == null || !CanEditByCreator(currentRow)) return;
                if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(memContent.Text))
                {
                    XtraMessageBox.Show("Tiêu đề và nội dung không được để trống", "Thông báo");
                    return;
                }

                MOS.SDO.HisSupportRequestUpdateSDO sdo = new MOS.SDO.HisSupportRequestUpdateSDO();
                sdo.Id = currentRow.ID;
                sdo.Title = txtTitle.Text;
                sdo.Content = memContent.Text;
                sdo.ContactInfo = txtContact.Text;

                CallAndRefresh(ApiUrl.HIS_SUPPORT_REQUEST__UPDATE, sdo, false);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = gridViewList.GetFocusedRow() as MOS.SDO.HisSupportRequestViewSDO;
                if (!CanEditByCreator(row)) return;
                if (XtraMessageBox.Show("Bạn có chắc muốn xoá yêu cầu này không?", "Thông báo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                CallAndRefresh(ApiUrl.HIS_SUPPORT_REQUEST__DELETE, row.ID, true);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentRow == null) return;
                CallAndRefresh(ApiUrl.HIS_SUPPORT_REQUEST__RECEIVE, currentRow.ID, false);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            DoFinish(SupportRequestStt.HOAN_THANH);
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            DoFinish(SupportRequestStt.TU_CHOI);
        }

        private void DoFinish(short targetStt)
        {
            try
            {
                if (currentRow == null) return;
                if (targetStt == SupportRequestStt.TU_CHOI && string.IsNullOrWhiteSpace(memFinishNote.Text))
                {
                    XtraMessageBox.Show("Vui lòng nhập lý do từ chối yêu cầu", "Thông báo");
                    memFinishNote.Focus();
                    return;
                }

                MOS.SDO.HisSupportRequestFinishSDO sdo = new MOS.SDO.HisSupportRequestFinishSDO();
                sdo.Id = currentRow.ID;
                sdo.SupportRequestStt = targetStt;
                sdo.FinishNote = memFinishNote.Text;

                CallAndRefresh(ApiUrl.HIS_SUPPORT_REQUEST__FINISH, sdo, false);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentRow == null) return;
                // Viec con "Moi tao" thi duoc phep de trong nguoi nhan (tra ve danh sach chung).
                bool allowEmptyTarget = currentRow.SUPPORT_REQUEST_STT == SupportRequestStt.MOI_TAO;
                string holder = string.IsNullOrWhiteSpace(currentRow.RECEIVE_LOGINNAME)
                    ? currentRow.ASSIGNEE_LOGINNAME
                    : currentRow.RECEIVE_LOGINNAME;

                using (frmTransfer frm = new frmTransfer(currentRow.SUPPORT_REQUEST_CODE, holder, allowEmptyTarget))
                {
                    if (frm.ShowDialog() != DialogResult.OK) return;

                    MOS.SDO.HisSupportRequestTransferSDO sdo = new MOS.SDO.HisSupportRequestTransferSDO();
                    sdo.Id = currentRow.ID;
                    sdo.TargetLoginname = frm.TargetLoginname;
                    sdo.Reason = frm.Reason;

                    CallAndRefresh(ApiUrl.HIS_SUPPORT_REQUEST__TRANSFER, sdo, false);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chuyen yeu cau len cong ty, chay hai pha.
        /// Pha 1 (FE -> cong ty): tai tep tu kho vien, day len kho tep cua cong ty,
        ///   ghep duong dan vao noi dung roi gui sang he thong cua cong ty.
        /// Pha 2 (FE -> HIS): goi dich vu ghi nhan da chuyen tiep.
        /// Pha 1 hong  -> dung han, KHONG goi pha 2.
        /// Pha 2 hong  -> cho chay lai RIENG pha 2, tuyet doi khong gui lai sang cong ty.
        /// </summary>
        private void btnForward_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentRow == null) return;

                int fileCount = currentRow.FILE_COUNT.HasValue ? (int)currentRow.FILE_COUNT.Value : 0;
                string title, content, reason;

                using (frmForward frm = new frmForward(currentRow.SUPPORT_REQUEST_CODE,
                           currentRow.TITLE, currentRow.CONTENT, fileCount))
                {
                    if (frm.ShowDialog() != DialogResult.OK) return;
                    title = frm.ForwardTitle;
                    content = frm.ForwardContent;
                    reason = frm.Reason;
                }

                WaitingManager.Show();
                bool sentToCrm = SendToCrm(title, content, currentRow.ID);
                WaitingManager.Hide();

                if (!sentToCrm) return;

                if (!RecordForwarded(currentRow.ID, reason))
                {
                    // Pha 1 da xong roi, bam lai se tao viec trung ben cong ty.
                    if (XtraMessageBox.Show(
                            "Yêu cầu ĐÃ ĐƯỢC GỬI sang công ty thành công, nhưng chưa ghi nhận được vào hệ thống của bệnh viện.\r\n\r\n"
                          + "Bấm Có để thử ghi nhận lại. KHÔNG gửi lại sang công ty.\r\n"
                          + "Bấm Không thì sau này phải tự chốt lại trạng thái, và tuyệt đối không bấm Chuyển công ty lần nữa.",
                            "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        RecordForwarded(currentRow.ID, reason);
                    }
                }

                long keepId = currentRow.ID;
                FillDataToGrid();
                FocusRowById(keepId);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
                XtraMessageBox.Show("Xảy ra lỗi khi chuyển yêu cầu lên công ty. Vui lòng kiểm tra lại", "Thông báo");
            }
        }

        /// <summary>
        /// Pha 1. He thong cua cong ty tra ve CHUOI RONG khi thanh cong,
        /// tra ve NOI DUNG LOI khi that bai - khong tra ve ma yeu cau nao.
        /// </summary>
        private bool SendToCrm(string title, string content, long supportRequestId)
        {
            try
            {
                string fileContent = UploadFileToCrm(supportRequestId);

                var employee = BackendDataWorker.Get<HIS_EMPLOYEE>()
                    .FirstOrDefault(o => o.LOGINNAME == currentLoginName);

                dynamic crmRequest = new System.Dynamic.ExpandoObject();
                crmRequest.tiêu_đề = title;
                crmRequest.nội_dung = string.Format("{0}\r\n{1}", content, fileContent);
                crmRequest.tổ_chức_yêu_cầu_id = HisConfigs.Get<string>(CONFIG_KEY__VPLUS_CUSTOMER_INFO);
                crmRequest.người_yêu_cầu = employee != null ? employee.VCONG_LOGINNAME : currentLoginName;
                crmRequest.anydesk = string.IsNullOrWhiteSpace(currentRow.ANYDESK_ID) ? "…" : currentRow.ANYDESK_ID;
                crmRequest.thông_tin_liên_lạc = currentRow.CONTACT_INFO;

                CommonParam param = new CommonParam();
                var crmResult = new Inventec.Common.Adapter.BackendAdapter(param)
                    .PostWithouApiParam<string>(CRM_URL__CREATE_REQUEST, ApiConsumers.CrmConsumer, param, crmRequest, 0, null);

                LogSystem.Info("Chuyen yeu cau ho tro len cong ty"
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => crmResult), crmResult));

                // Co noi dung tra ve = thong bao loi.
                if (crmResult != null && !string.IsNullOrEmpty(crmResult.ToString()))
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show(crmResult.ToString(), "Gửi sang công ty thất bại");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
                XtraMessageBox.Show("Không gửi được sang hệ thống của công ty. Yêu cầu giữ nguyên trạng thái cũ", "Thông báo");
                return false;
            }
        }

        /// <summary>
        /// Tai tep tu kho tep cua benh vien roi day len kho tep dung cho he thong cong ty,
        /// tra ve doan duong dan de ghep vao noi dung.
        /// </summary>
        private string UploadFileToCrm(long supportRequestId)
        {
            string result = "";
            try
            {
                CommonParam param = new CommonParam();
                HisSupportRequestFileFilter filter = new HisSupportRequestFileFilter();
                filter.SUPPORT_REQUEST_ID = supportRequestId;

                var files = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<List<HIS_SUPPORT_REQUEST_FILE>>(
                        ApiUrl.HIS_SUPPORT_REQUEST_FILE__GET, ApiConsumers.MosConsumer, filter, param);

                if (files == null || files.Count == 0) return "";

                string fssHospital = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS;
                string fssCrm = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS_FOR_CRM;

                // Tai tep tu kho cua vien ve bo nho.
                List<FileHolder> holders = new List<FileHolder>();
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    foreach (var f in files)
                    {
                        byte[] bytes = client.DownloadData(
                            string.Format("{0}{1}", fssHospital, f.FILE_URL.Replace("\\", "/")));
                        holders.Add(new FileHolder
                        {
                            Content = new System.IO.MemoryStream(bytes),
                            FileName = f.FILE_NAME
                        });
                    }
                }

                // Day ca lo sang kho tep cua cong ty - dung cach man Ctrl+F2 dang lam.
                var uploaded = Inventec.Fss.Client.FileUpload.UploadFile(
                    GlobalVariables.APPLICATION_CODE, "", holders, false, fssCrm);

                if (uploaded == null || uploaded.Count == 0) return "";

                List<string> imgExt = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                result = "\r\n\r\n";
                foreach (var u in uploaded)
                {
                    string fullUrl = string.Format("{0}{1}", fssCrm, u.Url.Replace("\\", "/"));
                    result += imgExt.Exists(o => u.Url.ToLower().EndsWith(o))
                        ? string.Format("<img src=\"{0}\">", fullUrl)
                        : string.Format("<a href=\"{0}\">{1}</a>", fullUrl, u.OriginalName);
                    result += "\r\n";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Pha 2. Ma doi chieu de TRONG vi he thong cua cong ty khong tra ve ma
        /// (xem muc A.3.6 cua PTTK_57682).
        /// </summary>
        private bool RecordForwarded(long id, string reason)
        {
            CommonParam param = new CommonParam();
            try
            {
                MOS.SDO.HisSupportRequestForwardSDO sdo = new MOS.SDO.HisSupportRequestForwardSDO();
                sdo.Id = id;
                sdo.CrmRequestCode = null;
                sdo.FinishNote = reason;

                WaitingManager.Show();
                var result = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<HIS_SUPPORT_REQUEST>(ApiUrl.HIS_SUPPORT_REQUEST__FORWARD, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();

                if (result != null) return true;

                MessageManager.Show(this.ParentForm, param, false);
                return false;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
                return false;
            }
        }

        private void btnAddCmt_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentRow == null) return;
                using (frmSupportRequestCmt frm = new frmSupportRequestCmt(currentRow.ID, null))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                        LoadComment(currentRow.ID);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repCmtEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = gridViewCmt.GetFocusedRow() as MOS.SDO.HisSupportRequestCmtViewSDO;
                if (row == null || !row.IS_MINE || row.CMT_TYPE == 2 || currentRow == null) return;

                using (frmSupportRequestCmt frm = new frmSupportRequestCmt(currentRow.ID, row))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                        LoadComment(currentRow.ID);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repCmtDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = gridViewCmt.GetFocusedRow() as MOS.SDO.HisSupportRequestCmtViewSDO;
                if (row == null || !row.IS_MINE || row.CMT_TYPE == 2 || currentRow == null) return;
                if (XtraMessageBox.Show("Bạn có chắc muốn xoá nội dung trao đổi này không?", "Thông báo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                CommonParam param = new CommonParam();
                WaitingManager.Show();
                var ok = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<bool>(ApiUrl.HIS_SUPPORT_REQUEST_CMT__DELETE, ApiConsumers.MosConsumer, row.ID, param);
                WaitingManager.Hide();

                MessageManager.Show(this.ParentForm, param, ok);
                if (ok) LoadComment(currentRow.ID);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Goi dich vu roi nap lai dong dang chon. Khong nap lai ca danh sach
        /// de khong lam mat vi tri con tro cua nguoi dung.
        /// </summary>
        private void CallAndRefresh(string url, object body, bool resultIsBool)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                long keepId = currentRow != null ? currentRow.ID : 0;

                // Dich vu Delete tra ve bool: phai doc dung gia tri, khong duoc kiem tra khac null
                // vi false cung la mot gia tri hop le va se bi hieu nham thanh thanh cong.
                if (resultIsBool)
                {
                    success = new Inventec.Common.Adapter.BackendAdapter(param)
                        .Post<bool>(url, ApiConsumers.MosConsumer, body, param);
                }
                else
                {
                    success = new Inventec.Common.Adapter.BackendAdapter(param)
                        .Post<HIS_SUPPORT_REQUEST>(url, ApiConsumers.MosConsumer, body, param) != null;
                }

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);

                if (success)
                {
                    FillDataToGrid();
                    FocusRowById(keepId);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
                MessageManager.Show(this.ParentForm, param, false);
            }
        }

        private void FocusRowById(long id)
        {
            try
            {
                if (id <= 0 || listData == null) return;
                for (int i = 0; i < listData.Count; i++)
                {
                    if (listData[i].ID == id)
                    {
                        gridViewList.FocusedRowHandle = gridViewList.GetRowHandle(i);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion
    }

    /// <summary>Dong du lieu dung chung cho cac o chon tren man hinh.</summary>
    public class ComboADO
    {
        public string CODE { get; set; }
        public string NAME { get; set; }

        /// <summary>
        /// Chu hien thi o chon nguoi: "Ho ten (ten dang nhap)". O chon chi loc tren cot hien thi
        /// nen ghep ca hai vao day de go ten nao cung tim thay.
        /// </summary>
        public string DISPLAY
        {
            get { return string.IsNullOrWhiteSpace(NAME) || NAME == CODE ? CODE : NAME + " (" + CODE + ")"; }
        }
    }
}
