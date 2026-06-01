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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.ADO;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Config;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne
{
    public partial class frmTransactionBillTwoInOne
    {
        #region Fields Reciept grid
        private GridControl grdRecieptDiscount;
        private GridView gvRecieptDiscount;
        private GridColumn gcRecieptDiscount;
        private GridColumn gcRecieptDiscountRatio;
        private GridColumn gcRecieptDiscountReason;
        private GridColumn gcRecieptDiscountDelete;
        private RepositoryItemSpinEdit repRecieptDiscountSpin;
        private RepositoryItemSpinEdit repRecieptDiscountRatioSpin;
        private RepositoryItemTextEdit repRecieptDiscountReason;
        private RepositoryItemButtonEdit repRecieptDiscountDelete;
        private BindingList<TransactionDiscountADO> bindRecieptDiscount;
        #endregion

        #region Fields Invoice grid
        private GridControl grdInvoiceDiscount;
        private GridView gvInvoiceDiscount;
        private GridColumn gcInvoiceDiscount;
        private GridColumn gcInvoiceDiscountRatio;
        private GridColumn gcInvoiceDiscountReason;
        private GridColumn gcInvoiceDiscountDelete;
        private RepositoryItemSpinEdit repInvoiceDiscountSpin;
        private RepositoryItemSpinEdit repInvoiceDiscountRatioSpin;
        private RepositoryItemTextEdit repInvoiceDiscountReason;
        private RepositoryItemButtonEdit repInvoiceDiscountDelete;
        private BindingList<TransactionDiscountADO> bindInvoiceDiscount;

        // Cờ chặn tái nhập: khi tự SetRowCellValue (đ->% hoặc %->đ) sẽ kích hoạt lại CellValueChanged
        // -> đệ quy vô hạn -> StackOverflow -> app chết. Cờ này chặn lần fire thứ 2.
        private bool isSyncingRecieptDiscount = false;
        private bool isSyncingInvoiceDiscount = false;
        #endregion

        private void InitGridDiscountIfEnable()
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount) return;

                BuildGridRecieptDiscount();
                AttachRecieptGridIntoLayout();

                BuildGridInvoiceDiscount();
                AttachInvoiceGridIntoLayout();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Build Reciept grid
        private void BuildGridRecieptDiscount()
        {
            this.bindRecieptDiscount = new BindingList<TransactionDiscountADO>();
            this.bindRecieptDiscount.AllowNew = false;
            this.bindRecieptDiscount.AllowRemove = true;
            this.bindRecieptDiscount.AllowEdit = true;
            this.bindRecieptDiscount.Add(new TransactionDiscountADO());   // 1 dòng trống ban đầu; nhập đủ -> tự sinh dòng mới

            this.grdRecieptDiscount = new GridControl();
            this.grdRecieptDiscount.Name = "grdRecieptDiscount";
            this.grdRecieptDiscount.MenuManager = this.barManager1;

            this.gvRecieptDiscount = new GridView();
            this.gvRecieptDiscount.Name = "gvRecieptDiscount";
            this.gvRecieptDiscount.OptionsView.ShowGroupPanel = false;
            this.gvRecieptDiscount.OptionsView.ShowIndicator = true;
            this.gvRecieptDiscount.OptionsView.ColumnAutoWidth = true;   // như cũ; chỉ cột Lý do FixedWidth
            this.gvRecieptDiscount.OptionsView.AnimationType = GridAnimationType.NeverAnimate;
            // KHÔNG dùng New Item Row "*" của DevExpress (từng gây mất dòng); tự thêm dòng trống cuối bằng EnsureTrailingEmptyRow.
            this.gvRecieptDiscount.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvRecieptDiscount.OptionsBehavior.Editable = true;
            this.gvRecieptDiscount.OptionsCustomization.AllowSort = false;
            this.gvRecieptDiscount.OptionsCustomization.AllowFilter = false;
            this.gvRecieptDiscount.OptionsCustomization.AllowGroup = false;
            this.gvRecieptDiscount.OptionsCustomization.AllowColumnMoving = false;
            this.gvRecieptDiscount.OptionsFind.AllowFindPanel = false;
            this.gvRecieptDiscount.OptionsSelection.EnableAppearanceFocusedCell = false;

            this.grdRecieptDiscount.MainView = this.gvRecieptDiscount;
            this.grdRecieptDiscount.ViewCollection.Add(this.gvRecieptDiscount);

            BuildRecieptColumns();
            BuildRecieptRepositoryItems();

            this.grdRecieptDiscount.DataSource = this.bindRecieptDiscount;

            this.gvRecieptDiscount.CellValueChanged += GvRecieptDiscount_CellValueChanged;
        }

        private void BuildRecieptColumns()
        {
            // đ/% giữ như cũ (auto theo grid); CHỈ cột "Lý do" fix cứng width.
            this.gcRecieptDiscount = new GridColumn();
            this.gcRecieptDiscount.Caption = "Chiết khấu (đ)";
            this.gcRecieptDiscount.FieldName = "DISCOUNT";
            this.gcRecieptDiscount.Visible = true;
            this.gcRecieptDiscount.VisibleIndex = 0;
            this.gcRecieptDiscount.Width = 140;

            this.gcRecieptDiscountRatio = new GridColumn();
            this.gcRecieptDiscountRatio.Caption = "Chiết khấu (%)";
            this.gcRecieptDiscountRatio.FieldName = "DISCOUNT_RATIO";
            this.gcRecieptDiscountRatio.Visible = true;
            this.gcRecieptDiscountRatio.VisibleIndex = 1;
            this.gcRecieptDiscountRatio.Width = 100;

            this.gcRecieptDiscountReason = new GridColumn();
            this.gcRecieptDiscountReason.Caption = "Lý do";
            this.gcRecieptDiscountReason.FieldName = "REASON";
            this.gcRecieptDiscountReason.Visible = true;
            this.gcRecieptDiscountReason.VisibleIndex = 2;
            this.gcRecieptDiscountReason.Width = 360;   // fix cứng, không co giãn
            this.gcRecieptDiscountReason.OptionsColumn.FixedWidth = true;

            // Cột nút X xóa dòng — unbound, cố định rộng ~30px (không auto-width), không caption.
            this.gcRecieptDiscountDelete = new GridColumn();
            this.gcRecieptDiscountDelete.Caption = "";
            this.gcRecieptDiscountDelete.FieldName = "DELETE_ROW";
            this.gcRecieptDiscountDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcRecieptDiscountDelete.Visible = true;
            this.gcRecieptDiscountDelete.VisibleIndex = 3;
            this.gcRecieptDiscountDelete.Width = 30;
            this.gcRecieptDiscountDelete.OptionsColumn.FixedWidth = true;   // giữ rộng cố định khi ColumnAutoWidth
            this.gcRecieptDiscountDelete.OptionsColumn.ShowCaption = false;

            this.gvRecieptDiscount.Columns.AddRange(new GridColumn[]
            {
                this.gcRecieptDiscount,
                this.gcRecieptDiscountRatio,
                this.gcRecieptDiscountReason,
                this.gcRecieptDiscountDelete
            });
        }

        private void BuildRecieptRepositoryItems()
        {
            this.repRecieptDiscountSpin = BuildSpinEditor("repRecieptDiscountSpin");
            AttachSpinFormat(this.repRecieptDiscountSpin);

            this.repRecieptDiscountRatioSpin = BuildSpinEditor("repRecieptDiscountRatioSpin");
            this.repRecieptDiscountRatioSpin.IsFloatValue = true;    // cho phép % thập phân
            this.repRecieptDiscountRatioSpin.MaxValue = 100;
            FormatControl(2, this.repRecieptDiscountRatioSpin);      // hiển thị 2 chữ số thập phân

            this.repRecieptDiscountReason = BuildReasonEditor("repRecieptDiscountReason");

            this.repRecieptDiscountDelete = BuildDeleteButtonEditor("repRecieptDiscountDelete");
            this.repRecieptDiscountDelete.ButtonClick += RepRecieptDiscountDelete_ButtonClick;

            this.grdRecieptDiscount.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[]
            {
                this.repRecieptDiscountSpin,
                this.repRecieptDiscountRatioSpin,
                this.repRecieptDiscountReason,
                this.repRecieptDiscountDelete
            });

            this.gcRecieptDiscount.ColumnEdit = this.repRecieptDiscountSpin;
            this.gcRecieptDiscountRatio.ColumnEdit = this.repRecieptDiscountRatioSpin;
            this.gcRecieptDiscountReason.ColumnEdit = this.repRecieptDiscountReason;
            this.gcRecieptDiscountDelete.ColumnEdit = this.repRecieptDiscountDelete;
        }

        /// <summary>Click nút X (viện phí) -> XÓA dòng đó; luôn giữ 1 dòng trống cuối để nhập tiếp.</summary>
        private void RepRecieptDiscountDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                int rowHandle = this.gvRecieptDiscount.FocusedRowHandle;
                if (rowHandle < 0) return;

                var row = this.gvRecieptDiscount.GetRow(rowHandle) as TransactionDiscountADO;
                if (row != null && this.bindRecieptDiscount != null)
                {
                    this.gvRecieptDiscount.CloseEditor();
                    this.bindRecieptDiscount.Remove(row);
                    EnsureTrailingEmptyRow(this.bindRecieptDiscount);
                    UpdateRecieptAmountAfterDiscount();
                    this.CalcuCanThu(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AttachRecieptGridIntoLayout()
        {
            // Hàng "Chiết khấu" gốc chia nhiều cột (đ/%/Lý do). Khi ẩn item con -> cột co lại nên grid
            // không kéo hết bề ngang (LayoutControl khóa item theo cụm cột). Giải pháp: ẩn TOÀN BỘ hàng cũ
            // rồi thêm 1 item MỚI vào group -> item không có hàng xóm ngang nên chiếm TRỌN bề ngang group.
            this.lciNotReciept.BeginUpdate();
            try
            {
                this.lciRecieptDiscountPrice.Visibility = LayoutVisibility.Never;   // "Chiết khấu:" + spin (đ) cũ
                this.lciRecieptDiscountRatio.Visibility = LayoutVisibility.Never;   // spin (%)
                this.lciRecieptReason.Visibility = LayoutVisibility.Never;          // ô "Lý do" cũ
                this.layoutControlItem2.Visibility = LayoutVisibility.Never;        // nhãn "đ"
                this.layoutControlItem31.Visibility = LayoutVisibility.Never;       // nhãn "%"

                this.grdRecieptDiscount.MinimumSize = new Size(0, 40);

                // Item full bề ngang chứa grid -> kéo sát 2 viền, tự co giãn theo group.
                var lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
                lciGrid.Name = "lciRecieptDiscountGrid";
                lciGrid.Text = "Chiết khấu:";
                lciGrid.TextSize = new Size(90, 20);
                lciGrid.TextToControlDistance = 5;
                lciGrid.AppearanceItemCaption.Options.UseTextOptions = true;
                lciGrid.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                lciGrid.SizeConstraintsType = SizeConstraintsType.Custom;
                lciGrid.MinSize = new Size(0, 48);
                lciGrid.MaxSize = new Size(0, 48);   // CHỈ cao 1 dòng (header + 1 dòng); dòng dư -> cuộn dọc
                lciGrid.Control = this.grdRecieptDiscount;

                // AddItem (không baseItem) -> item mới thành 1 HÀNG full bề ngang ở cuối group.
                this.lcgReceiptGroup.AddItem(lciGrid);
            }
            finally
            {
                this.lciNotReciept.EndUpdate();
            }
        }
        #endregion

        #region Build Invoice grid
        private void BuildGridInvoiceDiscount()
        {
            this.bindInvoiceDiscount = new BindingList<TransactionDiscountADO>();
            this.bindInvoiceDiscount.AllowNew = false;
            this.bindInvoiceDiscount.AllowRemove = true;
            this.bindInvoiceDiscount.AllowEdit = true;
            this.bindInvoiceDiscount.Add(new TransactionDiscountADO());   // 1 dòng trống ban đầu; nhập đủ -> tự sinh dòng mới

            this.grdInvoiceDiscount = new GridControl();
            this.grdInvoiceDiscount.Name = "grdInvoiceDiscount";
            this.grdInvoiceDiscount.MenuManager = this.barManager1;

            this.gvInvoiceDiscount = new GridView();
            this.gvInvoiceDiscount.Name = "gvInvoiceDiscount";
            this.gvInvoiceDiscount.OptionsView.ShowGroupPanel = false;
            this.gvInvoiceDiscount.OptionsView.ShowIndicator = true;
            this.gvInvoiceDiscount.OptionsView.ColumnAutoWidth = true;   // như cũ; chỉ cột Lý do FixedWidth
            this.gvInvoiceDiscount.OptionsView.AnimationType = GridAnimationType.NeverAnimate;
            // KHÔNG dùng New Item Row "*" của DevExpress (từng gây mất dòng); tự thêm dòng trống cuối bằng EnsureTrailingEmptyRow.
            this.gvInvoiceDiscount.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvInvoiceDiscount.OptionsBehavior.Editable = true;
            this.gvInvoiceDiscount.OptionsCustomization.AllowSort = false;
            this.gvInvoiceDiscount.OptionsCustomization.AllowFilter = false;
            this.gvInvoiceDiscount.OptionsCustomization.AllowGroup = false;
            this.gvInvoiceDiscount.OptionsCustomization.AllowColumnMoving = false;
            this.gvInvoiceDiscount.OptionsFind.AllowFindPanel = false;
            this.gvInvoiceDiscount.OptionsSelection.EnableAppearanceFocusedCell = false;

            this.grdInvoiceDiscount.MainView = this.gvInvoiceDiscount;
            this.grdInvoiceDiscount.ViewCollection.Add(this.gvInvoiceDiscount);

            BuildInvoiceColumns();
            BuildInvoiceRepositoryItems();

            this.grdInvoiceDiscount.DataSource = this.bindInvoiceDiscount;

            this.gvInvoiceDiscount.CellValueChanged += GvInvoiceDiscount_CellValueChanged;
        }

        private void BuildInvoiceColumns()
        {
            // đ/% giữ như cũ (auto theo grid); CHỈ cột "Lý do" fix cứng width.
            this.gcInvoiceDiscount = new GridColumn();
            this.gcInvoiceDiscount.Caption = "Chiết khấu (đ)";
            this.gcInvoiceDiscount.FieldName = "DISCOUNT";
            this.gcInvoiceDiscount.Visible = true;
            this.gcInvoiceDiscount.VisibleIndex = 0;
            this.gcInvoiceDiscount.Width = 140;

            this.gcInvoiceDiscountRatio = new GridColumn();
            this.gcInvoiceDiscountRatio.Caption = "Chiết khấu (%)";
            this.gcInvoiceDiscountRatio.FieldName = "DISCOUNT_RATIO";
            this.gcInvoiceDiscountRatio.Visible = true;
            this.gcInvoiceDiscountRatio.VisibleIndex = 1;
            this.gcInvoiceDiscountRatio.Width = 100;

            this.gcInvoiceDiscountReason = new GridColumn();
            this.gcInvoiceDiscountReason.Caption = "Lý do";
            this.gcInvoiceDiscountReason.FieldName = "REASON";
            this.gcInvoiceDiscountReason.Visible = true;
            this.gcInvoiceDiscountReason.VisibleIndex = 2;
            this.gcInvoiceDiscountReason.Width = 360;   // fix cứng, không co giãn
            this.gcInvoiceDiscountReason.OptionsColumn.FixedWidth = true;

            // Cột nút X xóa dòng — unbound, cố định rộng ~30px (không auto-width), không caption.
            this.gcInvoiceDiscountDelete = new GridColumn();
            this.gcInvoiceDiscountDelete.Caption = "";
            this.gcInvoiceDiscountDelete.FieldName = "DELETE_ROW";
            this.gcInvoiceDiscountDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcInvoiceDiscountDelete.Visible = true;
            this.gcInvoiceDiscountDelete.VisibleIndex = 3;
            this.gcInvoiceDiscountDelete.Width = 30;
            this.gcInvoiceDiscountDelete.OptionsColumn.FixedWidth = true;
            this.gcInvoiceDiscountDelete.OptionsColumn.ShowCaption = false;

            this.gvInvoiceDiscount.Columns.AddRange(new GridColumn[]
            {
                this.gcInvoiceDiscount,
                this.gcInvoiceDiscountRatio,
                this.gcInvoiceDiscountReason,
                this.gcInvoiceDiscountDelete
            });
        }

        private void BuildInvoiceRepositoryItems()
        {
            this.repInvoiceDiscountSpin = BuildSpinEditor("repInvoiceDiscountSpin");
            AttachSpinFormat(this.repInvoiceDiscountSpin);

            this.repInvoiceDiscountRatioSpin = BuildSpinEditor("repInvoiceDiscountRatioSpin");
            this.repInvoiceDiscountRatioSpin.IsFloatValue = true;    // cho phép % thập phân
            this.repInvoiceDiscountRatioSpin.MaxValue = 100;
            FormatControl(2, this.repInvoiceDiscountRatioSpin);      // hiển thị 2 chữ số thập phân

            this.repInvoiceDiscountReason = BuildReasonEditor("repInvoiceDiscountReason");

            this.repInvoiceDiscountDelete = BuildDeleteButtonEditor("repInvoiceDiscountDelete");
            this.repInvoiceDiscountDelete.ButtonClick += RepInvoiceDiscountDelete_ButtonClick;

            this.grdInvoiceDiscount.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[]
            {
                this.repInvoiceDiscountSpin,
                this.repInvoiceDiscountRatioSpin,
                this.repInvoiceDiscountReason,
                this.repInvoiceDiscountDelete
            });

            this.gcInvoiceDiscount.ColumnEdit = this.repInvoiceDiscountSpin;
            this.gcInvoiceDiscountRatio.ColumnEdit = this.repInvoiceDiscountRatioSpin;
            this.gcInvoiceDiscountReason.ColumnEdit = this.repInvoiceDiscountReason;
            this.gcInvoiceDiscountDelete.ColumnEdit = this.repInvoiceDiscountDelete;
        }

        /// <summary>Click nút X (dịch vụ) -> XÓA dòng đó; luôn giữ 1 dòng trống cuối để nhập tiếp.</summary>
        private void RepInvoiceDiscountDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                int rowHandle = this.gvInvoiceDiscount.FocusedRowHandle;
                if (rowHandle < 0) return;

                var row = this.gvInvoiceDiscount.GetRow(rowHandle) as TransactionDiscountADO;
                if (row != null && this.bindInvoiceDiscount != null)
                {
                    this.gvInvoiceDiscount.CloseEditor();
                    this.bindInvoiceDiscount.Remove(row);
                    EnsureTrailingEmptyRow(this.bindInvoiceDiscount);
                    UpdateInvoiceAmountAfterDiscount();
                    this.CalcuCanThu(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AttachInvoiceGridIntoLayout()
        {
            // Ẩn TOÀN BỘ hàng "Chiết khấu" gốc rồi thêm 1 item MỚI full bề ngang group (xem giải thích bên reciept).
            this.layoutControl5.BeginUpdate();
            try
            {
                this.lciInvoiceDiscountPrice.Visibility = LayoutVisibility.Never;   // "Chiết khấu:" + spin (đ) cũ
                this.lciInvoiceDiscountRatio.Visibility = LayoutVisibility.Never;   // spin (%)
                this.lciInvoiceReason.Visibility = LayoutVisibility.Never;          // ô "Lý do" cũ
                this.layoutControlItem33.Visibility = LayoutVisibility.Never;       // nhãn "đ"
                this.layoutControlItem35.Visibility = LayoutVisibility.Never;       // nhãn "%"

                this.grdInvoiceDiscount.MinimumSize = new Size(0, 40);

                var lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
                lciGrid.Name = "lciInvoiceDiscountGrid";
                lciGrid.Text = "Chiết khấu:";
                lciGrid.TextSize = new Size(70, 20);
                lciGrid.TextToControlDistance = 5;
                lciGrid.AppearanceItemCaption.Options.UseTextOptions = true;
                lciGrid.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                lciGrid.SizeConstraintsType = SizeConstraintsType.Custom;
                lciGrid.MinSize = new Size(0, 48);
                lciGrid.MaxSize = new Size(0, 48);   // CHỈ cao 1 dòng (header + 1 dòng); dòng dư -> cuộn dọc
                lciGrid.Control = this.grdInvoiceDiscount;

                this.lcgInvoiceGroup.AddItem(lciGrid);
            }
            finally
            {
                this.layoutControl5.EndUpdate();
            }
        }
        #endregion

        #region Builders
        private RepositoryItemSpinEdit BuildSpinEditor(string name)
        {
            var spin = new RepositoryItemSpinEdit();
            spin.Name = name;
            spin.MinValue = 0;
            spin.IsFloatValue = true;
            spin.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            spin.Buttons.Clear();
            return spin;
        }

        private void AttachSpinFormat(RepositoryItemSpinEdit spin)
        {
            try
            {
                FormatControl(ConfigApplications.NumberSeperator, spin);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private RepositoryItemTextEdit BuildReasonEditor(string name)
        {
            var txt = new RepositoryItemTextEdit();
            txt.Name = name;
            txt.MaxLength = 250;
            return txt;
        }

        /// <summary>Tạo editor nút X (xóa dòng) — chỉ hiện nút, hiện ở MỌI dòng (kể cả không focus).</summary>
        private RepositoryItemButtonEdit BuildDeleteButtonEditor(string name)
        {
            var btn = new RepositoryItemButtonEdit();
            btn.Name = name;
            // HideTextEditor: ẩn ô text -> CHỈ còn nút, và nút tự hiện ở MỌI dòng grid (cách chuẩn làm cột nút).
            btn.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            btn.Buttons.Clear();
            btn.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(
                DevExpress.XtraEditors.Controls.ButtonPredefines.Delete));  
            return btn;
        }

        #endregion

        #region Cell value changed
        private void GvRecieptDiscount_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            // CHẶN tái nhập: SetRowCellValue bên dưới sẽ fire lại event này -> nếu không chặn sẽ đệ quy vô hạn.
            if (this.isSyncingRecieptDiscount) return;
            try
            {
                var row = this.gvRecieptDiscount.GetRow(e.RowHandle) as TransactionDiscountADO;
                if (row == null) return;

                this.isSyncingRecieptDiscount = true;

                if (e.Column == this.gcRecieptDiscount)
                {
                    if (this.totalReciept > 0)
                    {
                        // Gán thẳng property -> INotifyPropertyChanged tự cập nhật cell %. KHÔNG dùng
                        // SetRowCellValue vì nó commit SỚM New Item Row (sinh dòng thừa + mất dòng đang nhập).
                        // Làm tròn 2 chữ số thập phân (không phải số nguyên).
                        row.DISCOUNT_RATIO = Math.Round(((row.DISCOUNT ?? 0) / this.totalReciept) * 100m, 2, MidpointRounding.AwayFromZero);
                    }
                }
                else if (e.Column == this.gcRecieptDiscountRatio)
                {
                    if (this.totalReciept > 0)
                    {
                        row.DISCOUNT = Math.Round(((row.DISCOUNT_RATIO ?? 0) * this.totalReciept) / 100m, 4);
                    }
                }

                UpdateRecieptAmountAfterDiscount();
                this.CalcuCanThu(true);

                // Dòng vừa nhập có dữ liệu -> sinh thêm 1 dòng trống ở cuối để nhập tiếp.
                // BeginInvoke: thêm dòng SAU khi event/edit hiện tại kết thúc -> tránh sửa BindingList giữa lúc đang edit.
                this.BeginInvoke(new Action(() => EnsureTrailingEmptyRow(this.bindRecieptDiscount)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.isSyncingRecieptDiscount = false;
            }
        }

        private void GvInvoiceDiscount_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            // CHẶN tái nhập: SetRowCellValue bên dưới sẽ fire lại event này -> nếu không chặn sẽ đệ quy vô hạn.
            if (this.isSyncingInvoiceDiscount) return;
            try
            {
                var row = this.gvInvoiceDiscount.GetRow(e.RowHandle) as TransactionDiscountADO;
                if (row == null) return;

                this.isSyncingInvoiceDiscount = true;

                if (e.Column == this.gcInvoiceDiscount)
                {
                    if (this.totalInvoice > 0)
                    {
                        // Gán thẳng property -> INotifyPropertyChanged tự cập nhật cell %. KHÔNG dùng SetRowCellValue.
                        // Làm tròn 2 chữ số thập phân (không phải số nguyên).
                        row.DISCOUNT_RATIO = Math.Round(((row.DISCOUNT ?? 0) / this.totalInvoice) * 100m, 2, MidpointRounding.AwayFromZero);
                    }
                }
                else if (e.Column == this.gcInvoiceDiscountRatio)
                {
                    if (this.totalInvoice > 0)
                    {
                        row.DISCOUNT = Math.Round(((row.DISCOUNT_RATIO ?? 0) * this.totalInvoice) / 100m, 4);
                    }
                }

                UpdateInvoiceAmountAfterDiscount();
                this.CalcuCanThu(true);

                // Dòng vừa nhập có dữ liệu -> sinh thêm 1 dòng trống ở cuối để nhập tiếp.
                // BeginInvoke: thêm dòng SAU khi event/edit hiện tại kết thúc -> tránh sửa BindingList giữa lúc đang edit.
                this.BeginInvoke(new Action(() => EnsureTrailingEmptyRow(this.bindInvoiceDiscount)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.isSyncingInvoiceDiscount = false;
            }
        }
        #endregion

        #region Public helpers
        private static bool IsEmptyDiscountRow(TransactionDiscountADO item)
        {
            if (item == null) return true;
            return (item.DISCOUNT ?? 0) == 0
                && (item.DISCOUNT_RATIO ?? 0) == 0
                && string.IsNullOrWhiteSpace(item.REASON);
        }

        /// <summary>
        /// Luôn giữ 1 dòng trống ở CUỐI grid để nhập tiếp:
        /// khi dòng cuối đã có dữ liệu -> thêm 1 dòng trống mới (cơ chế "nhập đủ -> sinh dòng").
        /// </summary>
        private void EnsureTrailingEmptyRow(BindingList<TransactionDiscountADO> bind)
        {
            try
            {
                if (bind == null) return;
                if (bind.Count == 0 || !IsEmptyDiscountRow(bind[bind.Count - 1]))
                {
                    bind.Add(new TransactionDiscountADO());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal decimal GetTotalRecieptDiscount()
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount || this.bindRecieptDiscount == null) return 0;
                return this.bindRecieptDiscount.Where(o => !IsEmptyDiscountRow(o)).Sum(o => o.DISCOUNT ?? 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        internal decimal GetTotalInvoiceDiscount()
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount || this.bindInvoiceDiscount == null) return 0;
                return this.bindInvoiceDiscount.Where(o => !IsEmptyDiscountRow(o)).Sum(o => o.DISCOUNT ?? 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        internal string GetJoinedRecieptReason()
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount || this.bindRecieptDiscount == null) return "";
                var reasons = this.bindRecieptDiscount
                    .Where(o => !IsEmptyDiscountRow(o) && !string.IsNullOrWhiteSpace(o.REASON))
                    .Select(o => o.REASON);
                return string.Join(";", reasons);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        internal string GetJoinedInvoiceReason()
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount || this.bindInvoiceDiscount == null) return "";
                var reasons = this.bindInvoiceDiscount
                    .Where(o => !IsEmptyDiscountRow(o) && !string.IsNullOrWhiteSpace(o.REASON))
                    .Select(o => o.REASON);
                return string.Join(";", reasons);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        internal List<HIS_TRANSACTION_DISCOUNT> BuildRecieptDiscountList(long treatmentId)
        {
            var rs = new List<HIS_TRANSACTION_DISCOUNT>();
            try
            {
                if (!HisConfig.EnableMultiDiscount || this.bindRecieptDiscount == null) return rs;
                foreach (var item in this.bindRecieptDiscount)
                {
                    if (IsEmptyDiscountRow(item)) continue;
                    rs.Add(new HIS_TRANSACTION_DISCOUNT
                    {
                        ID = (item.ID.HasValue && item.ID.Value > 0) ? item.ID.Value : 0,
                        TRANSACTION_ID = 0,
                        DISCOUNT = item.DISCOUNT ?? 0,   // số tiền CK chính xác (decimal) — đây là giá trị tính tiền
                        // % lưu DB kiểu long -> làm tròn số nguyên (backend KHÔNG lưu %thập phân). Số tiền ở trên vẫn chính xác theo % user nhập.
                        DISCOUNT_RATIO = (long?)Math.Round(item.DISCOUNT_RATIO ?? 0, 0, MidpointRounding.AwayFromZero),
                        REASON = item.REASON ?? "",
                        TREATMENT_ID = treatmentId
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        internal List<HIS_TRANSACTION_DISCOUNT> BuildInvoiceDiscountList(long treatmentId)
        {
            var rs = new List<HIS_TRANSACTION_DISCOUNT>();
            try
            {
                if (!HisConfig.EnableMultiDiscount || this.bindInvoiceDiscount == null) return rs;
                foreach (var item in this.bindInvoiceDiscount)
                {
                    if (IsEmptyDiscountRow(item)) continue;
                    rs.Add(new HIS_TRANSACTION_DISCOUNT
                    {
                        ID = (item.ID.HasValue && item.ID.Value > 0) ? item.ID.Value : 0,
                        TRANSACTION_ID = 0,
                        DISCOUNT = item.DISCOUNT ?? 0,   // số tiền CK chính xác (decimal) — đây là giá trị tính tiền
                        // % lưu DB kiểu long -> làm tròn số nguyên (backend KHÔNG lưu %thập phân). Số tiền ở trên vẫn chính xác theo % user nhập.
                        DISCOUNT_RATIO = (long?)Math.Round(item.DISCOUNT_RATIO ?? 0, 0, MidpointRounding.AwayFromZero),
                        REASON = item.REASON ?? "",
                        TREATMENT_ID = treatmentId
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        internal void ResetGridDiscount()
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount) return;

                if (this.bindRecieptDiscount != null)
                {
                    this.bindRecieptDiscount.Clear();
                    this.bindRecieptDiscount.Add(new TransactionDiscountADO());   // giữ 1 dòng cố định
                }
                if (this.bindInvoiceDiscount != null)
                {
                    this.bindInvoiceDiscount.Clear();
                    this.bindInvoiceDiscount.Add(new TransactionDiscountADO());   // giữ 1 dòng cố định
                }
                UpdateRecieptAmountAfterDiscount();
                UpdateInvoiceAmountAfterDiscount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetEnableGridRecieptDiscount(bool enable)
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount || this.grdRecieptDiscount == null) return;
                this.grdRecieptDiscount.Enabled = enable;
                if (!enable && this.bindRecieptDiscount != null)
                {
                    this.bindRecieptDiscount.Clear();
                }
                else if (enable)
                {
                    EnsureTrailingEmptyRow(this.bindRecieptDiscount);   // mở lại -> đảm bảo có 1 dòng trống để nhập
                }
                UpdateRecieptAmountAfterDiscount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetEnableGridInvoiceDiscount(bool enable)
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount || this.grdInvoiceDiscount == null) return;
                this.grdInvoiceDiscount.Enabled = enable;
                if (!enable && this.bindInvoiceDiscount != null)
                {
                    this.bindInvoiceDiscount.Clear();
                }
                else if (enable)
                {
                    EnsureTrailingEmptyRow(this.bindInvoiceDiscount);   // mở lại -> đảm bảo có 1 dòng trống để nhập
                }
                UpdateInvoiceAmountAfterDiscount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void UpdateRecieptAmountAfterDiscount()
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount) return;
                if (this.lblRecieptAmount == null) return;
                decimal net = this.totalReciept - GetTotalRecieptDiscount();
                this.lblRecieptAmount.Text = Inventec.Common.Number.Convert.NumberToString(net, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void UpdateInvoiceAmountAfterDiscount()
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount) return;
                if (this.lblInvoiceAmount == null) return;
                decimal net = this.totalInvoice - GetTotalInvoiceDiscount();
                this.lblInvoiceAmount.Text = Inventec.Common.Number.Convert.NumberToString(net, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
