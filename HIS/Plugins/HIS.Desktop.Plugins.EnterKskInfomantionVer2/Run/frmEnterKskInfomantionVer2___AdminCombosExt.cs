/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Combo "Đối tượng" (chọn nhiều) + "Nguồn chi trả" (chọn 1) cho tab KSK dưới 18 tuổi và trẻ em dưới 6 tuổi.
 * Tái sử dụng logic của tab ≥18 (InitObjectCheck/InitObjectCombo/Event_CheckObject...) nhưng KEYED theo control
 * (Dictionary theo combo) để dùng chung cho nhiều tab mà không đụng code ≥18.
 *
 * LƯU DB: đọc/ghi cột KSK_PATIENT_TYPES (chuỗi CSV "1;3;13") + KSK_PAY_SOURCE (short) trên
 * HIS_KSK_UNDER_EIGHTEEN và HIS_KSK_UNDER_SIX (đã có trong MOS.EFMODEL) — xem LoadAdminCombos / SaveAdminCombos.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        // combo Đối tượng -> danh sách mã đã tick (tương tự objectSelecteds của tab ≥18, nhưng theo từng combo).
        private readonly Dictionary<GridLookUpEdit, List<KskCodeNameADO>> objectSelectedsExt = new Dictionary<GridLookUpEdit, List<KskCodeNameADO>>();
        // GridCheckMarksSelection -> combo tương ứng (định tuyến SelectionChanged về đúng combo).
        private readonly Dictionary<GridCheckMarksSelection, GridLookUpEdit> objectCheckOwner = new Dictionary<GridCheckMarksSelection, GridLookUpEdit>();
        // Chống init trùng (mỗi combo chỉ init 1 lần).
        private readonly HashSet<GridLookUpEdit> adminCombosInitedExt = new HashSet<GridLookUpEdit>();

        #region Init datasource + hành vi (tái dùng khuôn tab ≥18)

        /// <summary>Khởi tạo cặp combo Đối tượng (chọn nhiều) + Nguồn chi trả (chọn 1) cho 1 tab. Idempotent.</summary>
        private void InitAdminCombosExt(GridLookUpEdit cboObj, GridLookUpEdit cboPay)
        {
            try
            {
                if (cboObj != null && !adminCombosInitedExt.Contains(cboObj))
                {
                    adminCombosInitedExt.Add(cboObj);
                    InitObjectCheckExt(cboObj);
                    InitObjectComboExt(cboObj);
                }
                if (cboPay != null && !adminCombosInitedExt.Contains(cboPay))
                {
                    adminCombosInitedExt.Add(cboPay);
                    InitPaymentSourceComboExt(cboPay);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Gắn GridCheckMarksSelection + event + nút Xóa cho combo Đối tượng (y hệt InitObjectCheck).</summary>
        private void InitObjectCheckExt(GridLookUpEdit cbo)
        {
            GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
            objectCheckOwner[gridCheck] = cbo;
            objectSelectedsExt[cbo] = new List<KskCodeNameADO>();
            gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_CheckObjectExt);
            cbo.Properties.Tag = gridCheck;
            cbo.Properties.View.OptionsSelection.MultiSelect = true;
            gridCheck.ClearSelection(cbo.Properties.View);
            cbo.CustomDisplayText -= cboObjectExt_CustomDisplayText;
            cbo.CustomDisplayText += cboObjectExt_CustomDisplayText;
            bool hasDelete = false;
            foreach (EditorButton btn in cbo.Properties.Buttons)
                if (btn.Kind == ButtonPredefines.Delete) { hasDelete = true; break; }
            if (!hasDelete)
            {
                EditorButton del = new EditorButton(ButtonPredefines.Delete);
                del.ToolTip = "Xóa giá trị đang chọn";
                cbo.Properties.Buttons.Add(del);
            }
            cbo.ButtonClick -= cboObjectExt_ClearMultiButtonClick;
            cbo.ButtonClick += cboObjectExt_ClearMultiButtonClick;
        }

        /// <summary>Gán DataSource + cột Mã/Tên + MultiSelect cho combo Đối tượng (y hệt InitObjectCombo).</summary>
        private void InitObjectComboExt(GridLookUpEdit cbo)
        {
            cbo.Properties.DataSource = BuildKskObjectList();
            cbo.Properties.DisplayMember = "NAME";
            cbo.Properties.ValueMember = "ID";
            cbo.Properties.NullText = "";
            DevExpress.XtraGrid.Columns.GridColumn colId = cbo.Properties.View.Columns.AddField("ID");
            colId.VisibleIndex = 1; colId.Width = 45; colId.Caption = "Mã";
            DevExpress.XtraGrid.Columns.GridColumn colName = cbo.Properties.View.Columns.AddField("NAME");
            colName.VisibleIndex = 2; colName.Width = 360; colName.Caption = "Tên";
            cbo.Properties.PopupFormWidth = 430;
            cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
            cbo.Properties.View.OptionsSelection.MultiSelect = true;
        }

        /// <summary>Combo Nguồn chi trả: chọn 1, cột Mã + Tên (y hệt InitPaymentSourceCombo).</summary>
        private void InitPaymentSourceComboExt(GridLookUpEdit cbo)
        {
            cbo.Properties.DataSource = BuildKskPaymentSourceList();
            cbo.Properties.DisplayMember = "NAME";
            cbo.Properties.ValueMember = "ID";
            cbo.Properties.NullText = "";
            DevExpress.XtraGrid.Columns.GridColumn colId = cbo.Properties.View.Columns.AddField("ID");
            colId.VisibleIndex = 1; colId.Width = 45; colId.Caption = "Mã";
            DevExpress.XtraGrid.Columns.GridColumn colName = cbo.Properties.View.Columns.AddField("NAME");
            colName.VisibleIndex = 2; colName.Width = 360; colName.Caption = "Tên";
            cbo.Properties.PopupFormWidth = 430;
            cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
        }

        private void Event_CheckObjectExt(object sender, EventArgs e)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                GridLookUpEdit cbo;
                if (!objectCheckOwner.TryGetValue(gridCheckMark, out cbo) || cbo == null) return;
                StringBuilder sb = new StringBuilder();
                List<KskCodeNameADO> selectedNews = new List<KskCodeNameADO>();
                foreach (KskCodeNameADO er in gridCheckMark.Selection)
                {
                    if (er != null)
                    {
                        if (sb.Length > 0) sb.Append(", ");
                        sb.Append(er.NAME);
                        selectedNews.Add(er);
                    }
                }
                objectSelectedsExt[cbo] = selectedNews;
                cbo.Text = sb.ToString();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void cboObjectExt_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                e.DisplayText = "";
                List<KskCodeNameADO> list;
                if (cbo != null && objectSelectedsExt.TryGetValue(cbo, out list) && list != null && list.Count > 0)
                {
                    string name = "";
                    foreach (var item in list) name += item.NAME + "; ";
                    e.DisplayText = name;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void cboObjectExt_ClearMultiButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e == null || e.Button == null || e.Button.Kind != ButtonPredefines.Delete) return;
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo == null) return;
                GridCheckMarksSelection gridCheck = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheck != null) gridCheck.ClearSelection(cbo.Properties.View);
                objectSelectedsExt[cbo] = new List<KskCodeNameADO>();
                cbo.EditValue = null;
                cbo.Text = string.Empty;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Chuỗi mã Đối tượng đã chọn (join ";") của 1 combo — dùng khi lưu.</summary>
        private string GetObjectValueExt(GridLookUpEdit cbo)
        {
            List<KskCodeNameADO> list;
            if (cbo == null || !objectSelectedsExt.TryGetValue(cbo, out list) || list == null) return "";
            return string.Join(";", list.Select(o => o.ID.ToString()).ToArray());
        }

        /// <summary>Tick lại combo Đối tượng theo chuỗi mã đã lưu ("1;3;13").</summary>
        private void SetObjectValueExt(GridLookUpEdit cbo, string codes)
        {
            try
            {
                if (cbo == null) return;
                GridCheckMarksSelection gridCheck = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheck == null) return;
                gridCheck.ClearSelection(cbo.Properties.View);
                objectSelectedsExt[cbo] = new List<KskCodeNameADO>();
                if (!string.IsNullOrEmpty(codes))
                {
                    var ds = cbo.Properties.DataSource as List<KskCodeNameADO>;
                    if (ds != null)
                    {
                        foreach (string c in codes.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var row = ds.FirstOrDefault(o => o.ID.ToString() == c.Trim());
                            if (row != null && !gridCheck.Selection.Contains(row))
                                gridCheck.Selection.Add(row);
                        }
                    }
                }
                gridCheck.OnSelectionChanged();
                if (string.IsNullOrEmpty(codes))
                {
                    cbo.EditValue = null;
                    cbo.Text = string.Empty;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region Load / Save theo tab (STUB DB — chờ cột EFMODEL)

        // Control cboObject3/cboPaymentSource3 (tab dưới 18) và cboObject8/cboPaymentSource8 (trẻ <6)
        // được khai báo trong Designer (giống cboObject/cboPaymentSource tab ≥18).

        /// <summary>Tab dưới 18 tuổi: init combo + đổ giá trị đã lưu. Gọi khi tab được fill (EnsureTabLoaded case 2).</summary>
        private void LoadAdminCombosUnderEighteen()
        {
            InitAdminCombosExt(cboObject3, cboPaymentSource3);
            SetObjectValueExt(cboObject3, currentKskUnderEight != null ? currentKskUnderEight.KSK_PATIENT_TYPES : null);
            cboPaymentSource3.EditValue = (currentKskUnderEight != null && currentKskUnderEight.KSK_PAY_SOURCE != null)
                ? (object)currentKskUnderEight.KSK_PAY_SOURCE : null;
        }

        /// <summary>Tab dưới 18 tuổi: ghi giá trị Đối tượng/Nguồn chi trả vào entity trước khi lưu.</summary>
        private void SaveAdminCombosUnderEighteen(HIS_KSK_UNDER_EIGHTEEN obj)
        {
            if (obj == null) return;
            string types = GetObjectValueExt(cboObject3);
            obj.KSK_PATIENT_TYPES = !string.IsNullOrEmpty(types) ? types : null;
            obj.KSK_PAY_SOURCE = cboPaymentSource3.EditValue != null ? (short?)Convert.ToInt16(cboPaymentSource3.EditValue) : null;
        }

        /// <summary>Tab trẻ em dưới 6 tuổi: init combo + đổ giá trị đã lưu. Gọi khi tab được fill (EnsureTabLoaded case 7).</summary>
        private void LoadAdminCombosUnderSix()
        {
            InitAdminCombosExt(cboObject8, cboPaymentSource8);
            SetObjectValueExt(cboObject8, currentKskUnderSixEf != null ? currentKskUnderSixEf.KSK_PATIENT_TYPES : null);
            cboPaymentSource8.EditValue = (currentKskUnderSixEf != null && currentKskUnderSixEf.KSK_PAY_SOURCE != null)
                ? (object)currentKskUnderSixEf.KSK_PAY_SOURCE : null;
        }

        /// <summary>Tab trẻ em dưới 6 tuổi: ghi giá trị Đối tượng/Nguồn chi trả vào entity trước khi lưu.</summary>
        private void SaveAdminCombosUnderSix(HIS_KSK_UNDER_SIX obj)
        {
            if (obj == null) return;
            string types = GetObjectValueExt(cboObject8);
            obj.KSK_PATIENT_TYPES = !string.IsNullOrEmpty(types) ? types : null;
            obj.KSK_PAY_SOURCE = cboPaymentSource8.EditValue != null ? (short?)Convert.ToInt16(cboPaymentSource8.EditValue) : null;
        }

        #endregion
    }
}
