/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisPatientPackage.Resources;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisPatientPackage
{
    public partial class UcHisPatientPackage : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare

        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;

        /// <summary>Cache giới tính (ID -> tên) để resolve nhanh, KHÔNG gọi trong vòng lặp.</summary>
        private Dictionary<long, string> genderDict;

        /// <summary>Cột mũi tên thể hiện trạng thái (thêm bằng code, đặt ngay sau STT, vẽ tam giác đổi màu).</summary>
        private DevExpress.XtraGrid.Columns.GridColumn colStatusArrow;

        /// <summary>Trạng thái mở/thu nhóm "Thời gian tạo".</summary>
        private bool timeExpanded = true;

        /// <summary>Dòng gói đang in — dùng để DelegatePrintMps000514 truy cập ID/PATIENT_ID khi callback.</summary>
        private HIS.Desktop.Plugins.HisPatientPackage.ADO.PatientPackageADO currentPrintRow;

        /// <summary>5 repo "disabled" — cùng icon với repo enabled nhưng Buttons[0].Enabled=false (DevExpress tự grey-out).</summary>
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoEditDis, repoDeleteDis, repoPrintDis, repoPayDis, repoRefundDis;

        private DevExpress.XtraEditors.DateEdit dteToDate;

        private bool needsRefreshOnReturn;

        #endregion

        #region Constructor / Load

        public UcHisPatientPackage(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.currentModule = module;
            WireEvents();
        }

        private void UcHisPatientPackage_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                InitGridButtonIcons();
                InitStatusArrowColumn();
                InitComboTimeType();
                InitDteToDate();
                SetDefaultControl();
                ApplyTimeTypeUi();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tạo dteToDate runtime + label "đến ngày:" (giống Hồ sơ điều trị).
        /// Chỉ hiện khi loại = "Khoảng ngày". Đặt xếp dọc DƯỚI dteDate (cùng X).
        /// Validate khi dteToDate change: KHÔNG cho < dteDate.
        /// </summary>
        private void InitDteToDate()
        {
            try
            {
                if (dteToDate != null) return;
                dteToDate = new DevExpress.XtraEditors.DateEdit();
                dteToDate.Name = "dteToDate";
                dteToDate.EditValue = DateTime.Now.Date.AddHours(23).AddMinutes(59);
                dteToDate.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                dteToDate.Properties.Mask.UseMaskAsDisplayFormat = true;
                dteToDate.Size = dteDate.Size;
                dteToDate.Anchor = dteDate.Anchor;
                // Đặt dteToDate xếp dọc DƯỚI dteDate, cùng X (đè vị trí btnPrev/Next cũ — buttons sẽ ẩn khi Range).
                dteToDate.Location = new System.Drawing.Point(
                    dteDate.Location.X,
                    dteDate.Location.Y + dteDate.Height + 4);
                dteToDate.Visible = false;
                this.panelControlLeft.Controls.Add(dteToDate);
                dteToDate.BringToFront();

                // Validate: đến ngày KHÔNG được nhỏ hơn từ ngày -> revert + warn.
                dteToDate.EditValueChanged += dteToDate_EditValueChanged;

                // Label "đến ngày:" — đặt phía trên dteToDate (label nhỏ, sát phải cạnh ô).
                if (panelControlLeft.Controls.Find("lblToDate", false).Length == 0)
                {
                    var lbl = new DevExpress.XtraEditors.LabelControl();
                    lbl.Name = "lblToDate";
                    lbl.Text = "đến ngày:";
                    lbl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
                    lbl.Size = new System.Drawing.Size(56, 14);
                    // Đặt sát bên trái dteToDate, căn giữa dọc.
                    lbl.Location = new System.Drawing.Point(
                        dteToDate.Location.X - 60,
                        dteToDate.Location.Y + 3);
                    lbl.Visible = false;
                    panelControlLeft.Controls.Add(lbl);
                    lbl.BringToFront();
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Flag chặn đệ quy khi handler tự sửa EditValue.</summary>
        private bool isAdjustingDateRange;

        /// <summary>
        /// Validate khi USER đổi "Đến ngày": nếu Đến < Từ -> revert Đến = Từ + 23:59 + cảnh báo.
        /// User chủ động chọn ngày sai -> chặn cứng + thông báo.
        /// </summary>
        private void dteToDate_EditValueChanged(object sender, EventArgs e)
        {
            if (isAdjustingDateRange) return;
            try
            {
                // CHỈ validate khi đang ở chế độ "Khoảng ngày" (idx=3).
                if (cboTimeType.SelectedIndex != 3) return;
                if (dteDate.EditValue == null || dteToDate == null || dteToDate.EditValue == null) return;
                DateTime fromD = Convert.ToDateTime(dteDate.EditValue);
                DateTime toD = Convert.ToDateTime(dteToDate.EditValue);
                if (toD < fromD)
                {
                    isAdjustingDateRange = true;
                    try { dteToDate.EditValue = fromD.Date.AddHours(23).AddMinutes(59); }
                    finally { isAdjustingDateRange = false; }
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Đến ngày không được nhỏ hơn Từ ngày.",
                        "Cảnh báo",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Khi USER đổi "Từ ngày" trong chế độ Range: nếu Từ > Đến -> auto-shift Đến = Từ + 23:59 (im lặng).
        /// Đây là hành vi tự nhiên (mở rộng khoảng tới ngày mới), KHÔNG phải lỗi user, nên không nag.
        /// </summary>
        private void dteDate_EditValueChanged(object sender, EventArgs e)
        {
            if (isAdjustingDateRange) return;
            try
            {
                if (cboTimeType.SelectedIndex != 3) return;
                if (dteDate.EditValue == null || dteToDate == null || dteToDate.EditValue == null) return;
                DateTime fromD = Convert.ToDateTime(dteDate.EditValue);
                DateTime toD = Convert.ToDateTime(dteToDate.EditValue);
                if (toD < fromD)
                {
                    isAdjustingDateRange = true;
                    try { dteToDate.EditValue = fromD.Date.AddHours(23).AddMinutes(59); }
                    finally { isAdjustingDateRange = false; }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Áp UI theo loại thời gian (giống pattern màn Hồ sơ điều trị):
        ///   0 Trong ngày  -> dteDate dd/MM/yyyy,         ẩn dteToDate + lblToDate, hiện btnPrev/Next.
        ///   1 Trong tháng -> dteDate MM/yyyy,            ẩn dteToDate + lblToDate, hiện btnPrev/Next.
        ///   2 Trong năm   -> dteDate yyyy,               ẩn dteToDate + lblToDate, hiện btnPrev/Next.
        ///   3 Khoảng ngày -> dteDate + dteToDate "dd/MM/yyyy HH:mm" (giống Hồ sơ điều trị),
        ///                    hiện lblToDate "đến ngày:", ẨN btnPrev/Next.
        ///                    Default time: Từ ngày = 00:00, Đến ngày = 23:59.
        /// </summary>
        private void ApplyTimeTypeUi()
        {
            // Bọc cả thân hàm bằng flag — set mask/EditValue khi chuyển mode sẽ trigger
            // EditValueChanged của dteDate/dteToDate. KHÔNG được để validate fire trong lúc này.
            bool wasAdjusting = isAdjustingDateRange;
            isAdjustingDateRange = true;
            try
            {
                int idx = cboTimeType.SelectedIndex;
                bool isRange = (idx == 3);

                string mask;
                if (idx == 1) mask = "MM/yyyy";
                else if (idx == 2) mask = "yyyy";
                else if (isRange) mask = "dd/MM/yyyy HH:mm";
                else mask = "dd/MM/yyyy";

                dteDate.Properties.Mask.EditMask = mask;

                // Khi chuyển sang Range -> set giờ 00:00 cho Từ ngày (nếu chưa có giờ).
                if (isRange && dteDate.EditValue != null)
                {
                    DateTime fromD = Convert.ToDateTime(dteDate.EditValue);
                    if (fromD.Hour == 0 && fromD.Minute == 0 && fromD.Second == 0)
                    {
                        // Đã 00:00 -> giữ nguyên.
                    }
                    else
                    {
                        // Chuyển từ chế độ khác sang Range -> reset về 00:00 ngày đó.
                        dteDate.EditValue = fromD.Date;
                    }
                }

                if (dteToDate != null)
                {
                    dteToDate.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    dteToDate.Visible = isRange;
                    if (isRange)
                    {
                        // Khi bật Range: đảm bảo có giá trị mặc định = ngày Từ + 23:59.
                        if (dteToDate.EditValue == null)
                        {
                            DateTime baseD = dteDate.EditValue != null
                                ? Convert.ToDateTime(dteDate.EditValue).Date
                                : DateTime.Now.Date;
                            dteToDate.EditValue = baseD.AddHours(23).AddMinutes(59);
                        }
                        dteToDate.BringToFront();
                    }
                }

                // Label "đến ngày:" — hiện/ẩn cùng dteToDate.
                var lbls = panelControlLeft.Controls.Find("lblToDate", false);
                if (lbls.Length > 0)
                {
                    lbls[0].Visible = isRange;
                    if (isRange) lbls[0].BringToFront();
                }

                // Range mode: ẩn Prev/Next để dteToDate có chỗ hiển thị (cùng vị trí Y).
                btnPrevDate.Visible = !isRange;
                btnNextDate.Visible = !isRange;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            finally { isAdjustingDateRange = wasAdjusting; }
        }

        /// <summary>Đăng ký các sự kiện (grid, nút, repository) — tách khỏi Designer.</summary>
        private void WireEvents()
        {
            try
            {
                this.gridView.CustomRowCellEdit += gridView_CustomRowCellEdit;
                this.gridView.RowStyle += gridView_RowStyle;
                this.gridView.CustomDrawCell += gridView_CustomDrawCell;

                // Dùng gridControl.MouseClick (WinForms native event) — fire CHẮC CHẮN lần click đầu,
                // bypass hoàn toàn button repository click chain (RowCellClick / EditorShowMode đôi khi
                // không fire khi repository absorb click). Dispatch theo column trong handler chung.
                this.gridControl.MouseClick += gridControl_MouseClick;

                this.btnSearch.Click += btnSearch_Click;
                this.btnRefresh.Click += btnRefresh_Click;
                this.btnPrevDate.Click += btnPrevDate_Click;
                this.btnNextDate.Click += btnNextDate_Click;
                this.btnToggleTime.Click += btnToggleTime_Click;
                this.cboTimeType.SelectedIndexChanged += cboTimeType_SelectedIndexChanged;

                // Range mode: auto-shift Đến = Từ + 23:59 khi user kéo Từ vượt Đến.
                this.dteDate.EditValueChanged += dteDate_EditValueChanged;

                this.txtPatientCode.KeyDown += txtFilter_KeyDown;
                this.txtKeyword.KeyDown += txtFilter_KeyDown;

                // Khi UC trở lại visible (user quay về tab Danh sách sau khi đóng/save tab con) -> refresh.
                this.VisibleChanged += UcHisPatientPackage_VisibleChanged;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Refresh grid khi user quay về tab Danh sách gói sau khi mở tab plugin con (Sửa / Thanh toán
        /// / Hoàn ứng). Chỉ refresh khi needsRefreshOnReturn=true (do OpenModuleByLink set).
        /// </summary>
        private void UcHisPatientPackage_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this.Visible) return;
                if (!needsRefreshOnReturn) return;
                needsRefreshOnReturn = false;
                FillDataToGrid();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        #endregion

        #region Language

        /// <summary>Gán caption/tooltip theo ngôn ngữ cho toàn bộ control + cột grid.</summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.HisPatientPackage.Resources.Lang",
                    typeof(UcHisPatientPackage).Assembly);

                this.txtPatientCode.Properties.NullValuePrompt = Lang("UcHisPatientPackage.txtPatientCode.Properties.NullValuePrompt");
                this.txtKeyword.Properties.NullValuePrompt = Lang("UcHisPatientPackage.txtKeyword.Properties.NullValuePrompt");
                this.lblTime.Text = Lang("UcHisPatientPackage.lciTime.Text");
                this.btnSearch.Text = Lang("UcHisPatientPackage.btnSearch.Text");
                this.btnRefresh.Text = Lang("UcHisPatientPackage.btnRefresh.Text");

                this.colSTT.Caption = Lang("UcHisPatientPackage.colSTT.Caption");
                this.colPatientCode.Caption = Lang("UcHisPatientPackage.colPatientCode.Caption");
                this.colPatientName.Caption = Lang("UcHisPatientPackage.colPatientName.Caption");
                this.colDob.Caption = Lang("UcHisPatientPackage.colDob.Caption");
                this.colGender.Caption = Lang("UcHisPatientPackage.colGender.Caption");
                this.colPackageName.Caption = Lang("UcHisPatientPackage.colPackageName.Caption");
                this.colStatus.Caption = Lang("UcHisPatientPackage.colStatus.Caption");
                this.colAddress.Caption = Lang("UcHisPatientPackage.colAddress.Caption");
                this.colCreateTime.Caption = Lang("UcHisPatientPackage.colCreateTime.Caption");
                this.colCreator.Caption = Lang("UcHisPatientPackage.colCreator.Caption");
                this.colModifyTime.Caption = Lang("UcHisPatientPackage.colModifyTime.Caption");
                this.colModifier.Caption = Lang("UcHisPatientPackage.colModifier.Caption");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string Lang(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key, ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        #endregion

        #region Init controls / icons

        /// <summary>
        /// Nạp icon cho các nút trong grid — TÁI SỬ DỤNG icon từ các màn hiện có:
        /// delete (AllergyCard), print/hoa-don (TransactionList), lock/unlock + refund/cancelTran (ServiceReqList).
        /// Edit + Pay dùng DevExpress Image Gallery (không có file PNG rời tương ứng).
        /// </summary>
        private void InitGridButtonIcons()
        {
            try
            {
                Image imgEdit   = LoadGalleryImage("images/edit/edit_16x16.png");
                Image imgDelete = LoadEmbeddedImage("delete_16x16.png");
                Image imgPrint  = LoadGalleryImage("images/print/printer_16x16.png");
                Image imgPay    = LoadGalleryImage("images/miscellaneous/currency_16x16.png");
                Image imgRefund = LoadGalleryImage("images/history/undo_16x16.png");

                SetRepoButton(this.repoEdit,   imgEdit,   Lang("UcHisPatientPackage.Tip.Edit"));
                SetRepoButton(this.repoDelete, imgDelete, Lang("UcHisPatientPackage.Tip.Delete"));
                // Tooltip mô tả ACTION khi click, KHÔNG phải state hiện tại:
                //   repoLock   icon "khóa đóng" hiện trên dòng IS_ACTIVE=0 -> click sẽ MỞ KHÓA -> tooltip = Unlock.
                //   repoUnlock icon "khóa mở"   hiện trên dòng IS_ACTIVE=1 -> click sẽ KHÓA    -> tooltip = Lock.
                SetRepoButton(this.repoLock,   LoadEmbeddedImage("lock_16x16.png"),   Lang("UcHisPatientPackage.Tip.Unlock"));
                SetRepoButton(this.repoUnlock, LoadEmbeddedImage("unlock_16x16.gif"), Lang("UcHisPatientPackage.Tip.Lock"));
                SetRepoButton(this.repoPay,    imgPay,    Lang("UcHisPatientPackage.Tip.Pay"));
                // Hoàn tiền: dùng icon "undo" (mũi tên cong xanh) — rõ nghĩa "hoàn về", thay icon refresh2 vòng tròn xanh lá.
                SetRepoButton(this.repoRefund, imgRefund, Lang("UcHisPatientPackage.Tip.Refund"));
                SetRepoButton(this.repoPrint,  imgPrint,  Lang("UcHisPatientPackage.Tip.Print"));

                // 5 repo "disabled" — cùng icon nhưng Buttons[0].Enabled=false; DevExpress tự render grayscale + chặn click.
                this.repoEditDis   = CreateDisabledRepo(imgEdit);
                this.repoDeleteDis = CreateDisabledRepo(imgDelete);
                this.repoPrintDis  = CreateDisabledRepo(imgPrint);
                this.repoPayDis    = CreateDisabledRepo(imgPay);
                this.repoRefundDis = CreateDisabledRepo(imgRefund);
                this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
                    this.repoEditDis, this.repoDeleteDis, this.repoPrintDis, this.repoPayDis, this.repoRefundDis });

                // repoEmpty: ô hoàn toàn trống (chỉ dùng khi không có hành động nào cần hiển thị).
                if (this.repoEmpty.Buttons.Count > 0)
                    this.repoEmpty.Buttons[0].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetRepoButton(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repo, Image image, string toolTip)
        {
            try
            {
                if (repo == null || repo.Buttons.Count == 0) return;
                repo.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
                if (image != null) repo.Buttons[0].Image = image;
                repo.Buttons[0].ToolTip = toolTip;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tạo repo "disabled" — cùng icon với repo enabled nhưng Buttons[0].Enabled=false.
        /// DevExpress tự render icon grayscale + chặn click. Dùng cho ô không cho phép theo trạng thái.
        /// </summary>
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit CreateDisabledRepo(Image image)
        {
            DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repo =
                new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            try
            {
                repo.AutoHeight = false;
                repo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                repo.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
                if (image != null) repo.Buttons[0].Image = image;
                repo.Buttons[0].Enabled = false;   // KEY: greyed-out + chặn click
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return repo;
        }

        /// <summary>Nạp ảnh nhúng (EmbeddedResource) trong Resources/Image theo tên file.</summary>
        private Image LoadEmbeddedImage(string fileName)
        {
            try
            {
                Assembly asm = typeof(UcHisPatientPackage).Assembly;
                string resName = asm.GetManifestResourceNames()
                    .FirstOrDefault(o => o.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(resName)) return null;
                using (Stream s = asm.GetManifestResourceStream(resName))
                {
                    return s == null ? null : Image.FromStream(s);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Nạp ảnh từ DevExpress Image Gallery theo URI.</summary>
        private Image LoadGalleryImage(string uri)
        {
            try
            {
                return DevExpress.Images.ImageResourceCache.Default.GetImage(uri);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// 4 items theo pattern màn Hồ sơ điều trị: Trong ngày / Trong tháng / Trong năm / Khoảng ngày.
        /// </summary>
        private void InitComboTimeType()
        {
            try
            {
                this.cboTimeType.Visible = true;
                this.cboTimeType.Properties.Items.Clear();
                this.cboTimeType.Properties.Items.AddRange(new object[] {
                    Lang("UcHisPatientPackage.TimeType.Day"),
                    Lang("UcHisPatientPackage.TimeType.Month"),
                    Lang("UcHisPatientPackage.TimeType.Year"),
                    Lang("UcHisPatientPackage.TimeType.Range") });
                this.cboTimeType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                this.txtPatientCode.Text = "";
                this.txtKeyword.Text = "";
                this.cboTimeType.SelectedIndex = 0;
                this.dteDate.EditValue = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
