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
        private RepositoryItemSpinEdit repRecieptDiscountSpin;
        private RepositoryItemSpinEdit repRecieptDiscountRatioSpin;
        private RepositoryItemTextEdit repRecieptDiscountReason;
        private BindingList<TransactionDiscountADO> bindRecieptDiscount;
        #endregion

        #region Fields Invoice grid
        private GridControl grdInvoiceDiscount;
        private GridView gvInvoiceDiscount;
        private GridColumn gcInvoiceDiscount;
        private GridColumn gcInvoiceDiscountRatio;
        private GridColumn gcInvoiceDiscountReason;
        private RepositoryItemSpinEdit repInvoiceDiscountSpin;
        private RepositoryItemSpinEdit repInvoiceDiscountRatioSpin;
        private RepositoryItemTextEdit repInvoiceDiscountReason;
        private BindingList<TransactionDiscountADO> bindInvoiceDiscount;
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
            this.bindRecieptDiscount.AllowNew = true;
            this.bindRecieptDiscount.AllowRemove = true;
            this.bindRecieptDiscount.AllowEdit = true;

            this.grdRecieptDiscount = new GridControl();
            this.grdRecieptDiscount.Name = "grdRecieptDiscount";
            this.grdRecieptDiscount.MenuManager = this.barManager1;

            this.gvRecieptDiscount = new GridView();
            this.gvRecieptDiscount.Name = "gvRecieptDiscount";
            this.gvRecieptDiscount.OptionsView.ShowGroupPanel = false;
            this.gvRecieptDiscount.OptionsView.ShowIndicator = true;
            this.gvRecieptDiscount.OptionsView.ColumnAutoWidth = true;
            this.gvRecieptDiscount.OptionsView.AnimationType = GridAnimationType.NeverAnimate;
            this.gvRecieptDiscount.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
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
            this.gcRecieptDiscountReason.Width = 360;

            this.gvRecieptDiscount.Columns.AddRange(new GridColumn[]
            {
                this.gcRecieptDiscount,
                this.gcRecieptDiscountRatio,
                this.gcRecieptDiscountReason
            });
        }

        private void BuildRecieptRepositoryItems()
        {
            this.repRecieptDiscountSpin = BuildSpinEditor("repRecieptDiscountSpin");
            AttachSpinFormat(this.repRecieptDiscountSpin);

            this.repRecieptDiscountRatioSpin = BuildSpinEditor("repRecieptDiscountRatioSpin");
            this.repRecieptDiscountRatioSpin.IsFloatValue = false;
            this.repRecieptDiscountRatioSpin.MaxValue = 100;
            AttachSpinFormat(this.repRecieptDiscountRatioSpin);

            this.repRecieptDiscountReason = BuildReasonEditor("repRecieptDiscountReason");

            this.grdRecieptDiscount.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[]
            {
                this.repRecieptDiscountSpin,
                this.repRecieptDiscountRatioSpin,
                this.repRecieptDiscountReason
            });

            this.gcRecieptDiscount.ColumnEdit = this.repRecieptDiscountSpin;
            this.gcRecieptDiscountRatio.ColumnEdit = this.repRecieptDiscountRatioSpin;
            this.gcRecieptDiscountReason.ColumnEdit = this.repRecieptDiscountReason;
        }

        private void AttachRecieptGridIntoLayout()
        {
            // Gắn grid vào chính item "Chiết khấu" có sẵn để LayoutControl tự quản lý vị trí/kích thước.
            this.lciNotReciept.BeginUpdate();
            try
            {
                this.lciRecieptDiscountRatio.Visibility = LayoutVisibility.Never;
                this.lciRecieptReason.Visibility = LayoutVisibility.Never;

                this.lciRecieptDiscountPrice.Text = "Chiết khấu:";
                this.lciRecieptDiscountPrice.SizeConstraintsType = SizeConstraintsType.Custom;
                this.lciRecieptDiscountPrice.MinSize = new Size(720, 46);
                this.lciRecieptDiscountPrice.MaxSize = new Size(0, 46);
                this.lciRecieptDiscountPrice.Control = this.grdRecieptDiscount;
                // Dòng QUAN TRỌNG ép grid rộng (item MinSize chưa đủ làm grid nở) -> để thấy đủ cột + dòng "*" + nút X.
                this.grdRecieptDiscount.MinimumSize = new Size(635, 40);

                // Spin chiết khấu cũ giờ mồ côi (item đã chuyển sang chứa grid) -> ẩn.
                this.spinRecieptDiscountPrice.Visible = false;

                // Nhãn "đ"/"%" và nút "..." nằm trong LayoutControlItem riêng -> PHẢI ẩn theo Item.
                this.layoutControlItem2.Visibility = LayoutVisibility.Never;    // nhãn "đ"
                this.layoutControlItem31.Visibility = LayoutVisibility.Never;   // nhãn "%"
                this.layoutControlItem47.Visibility = LayoutVisibility.Never;   // nút "..."
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
            this.bindInvoiceDiscount.AllowNew = true;
            this.bindInvoiceDiscount.AllowRemove = true;
            this.bindInvoiceDiscount.AllowEdit = true;

            this.grdInvoiceDiscount = new GridControl();
            this.grdInvoiceDiscount.Name = "grdInvoiceDiscount";
            this.grdInvoiceDiscount.MenuManager = this.barManager1;

            this.gvInvoiceDiscount = new GridView();
            this.gvInvoiceDiscount.Name = "gvInvoiceDiscount";
            this.gvInvoiceDiscount.OptionsView.ShowGroupPanel = false;
            this.gvInvoiceDiscount.OptionsView.ShowIndicator = true;
            this.gvInvoiceDiscount.OptionsView.ColumnAutoWidth = true;
            this.gvInvoiceDiscount.OptionsView.AnimationType = GridAnimationType.NeverAnimate;
            this.gvInvoiceDiscount.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
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
            this.gcInvoiceDiscountReason.Width = 360;

            this.gvInvoiceDiscount.Columns.AddRange(new GridColumn[]
            {
                this.gcInvoiceDiscount,
                this.gcInvoiceDiscountRatio,
                this.gcInvoiceDiscountReason
            });
        }

        private void BuildInvoiceRepositoryItems()
        {
            this.repInvoiceDiscountSpin = BuildSpinEditor("repInvoiceDiscountSpin");
            AttachSpinFormat(this.repInvoiceDiscountSpin);

            this.repInvoiceDiscountRatioSpin = BuildSpinEditor("repInvoiceDiscountRatioSpin");
            this.repInvoiceDiscountRatioSpin.IsFloatValue = false;
            this.repInvoiceDiscountRatioSpin.MaxValue = 100;
            AttachSpinFormat(this.repInvoiceDiscountRatioSpin);

            this.repInvoiceDiscountReason = BuildReasonEditor("repInvoiceDiscountReason");

            this.grdInvoiceDiscount.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[]
            {
                this.repInvoiceDiscountSpin,
                this.repInvoiceDiscountRatioSpin,
                this.repInvoiceDiscountReason
            });

            this.gcInvoiceDiscount.ColumnEdit = this.repInvoiceDiscountSpin;
            this.gcInvoiceDiscountRatio.ColumnEdit = this.repInvoiceDiscountRatioSpin;
            this.gcInvoiceDiscountReason.ColumnEdit = this.repInvoiceDiscountReason;
        }

        private void AttachInvoiceGridIntoLayout()
        {
            // Gắn grid vào chính item "Chiết khấu" có sẵn để LayoutControl tự quản lý vị trí/kích thước.
            this.layoutControl5.BeginUpdate();
            try
            {
                this.lciInvoiceDiscountRatio.Visibility = LayoutVisibility.Never;
                this.lciInvoiceReason.Visibility = LayoutVisibility.Never;

                this.lciInvoiceDiscountPrice.Text = "Chiết khấu:";
                this.lciInvoiceDiscountPrice.SizeConstraintsType = SizeConstraintsType.Custom;
                this.lciInvoiceDiscountPrice.MinSize = new Size(665, 46);
                this.lciInvoiceDiscountPrice.MaxSize = new Size(0, 46);
                this.lciInvoiceDiscountPrice.Control = this.grdInvoiceDiscount;
                // Dòng QUAN TRỌNG ép grid rộng -> thấy đủ cột + dòng "*" + nút X.
                this.grdInvoiceDiscount.MinimumSize = new Size(612, 40);

                this.spinInvoiceDiscountPrice.Visible = false;

                // Nhãn "đ"/"%" và nút "..." bên dịch vụ cũng nằm trong LayoutControlItem -> ẩn theo Item.
                this.layoutControlItem33.Visibility = LayoutVisibility.Never;   // nhãn "đ"
                this.layoutControlItem35.Visibility = LayoutVisibility.Never;   // nhãn "%"
                this.layoutControlItem49.Visibility = LayoutVisibility.Never;   // nút "..."
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

        #endregion

        #region Cell value changed
        private void GvRecieptDiscount_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var row = this.gvRecieptDiscount.GetRow(e.RowHandle) as TransactionDiscountADO;
                if (row == null) return;

                if (e.Column == this.gcRecieptDiscount)
                {
                    if (this.totalReciept > 0)
                    {
                        row.DISCOUNT_RATIO = Math.Round((row.DISCOUNT / this.totalReciept) * 100m, 0, MidpointRounding.AwayFromZero);
                        this.gvRecieptDiscount.SetRowCellValue(e.RowHandle, this.gcRecieptDiscountRatio, row.DISCOUNT_RATIO);
                    }
                }
                else if (e.Column == this.gcRecieptDiscountRatio)
                {
                    if (this.totalReciept > 0)
                    {
                        row.DISCOUNT = Math.Round((row.DISCOUNT_RATIO * this.totalReciept) / 100m, 4);
                        this.gvRecieptDiscount.SetRowCellValue(e.RowHandle, this.gcRecieptDiscount, row.DISCOUNT);
                    }
                }

                UpdateRecieptAmountAfterDiscount();
                this.CalcuCanThu(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GvInvoiceDiscount_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var row = this.gvInvoiceDiscount.GetRow(e.RowHandle) as TransactionDiscountADO;
                if (row == null) return;

                if (e.Column == this.gcInvoiceDiscount)
                {
                    if (this.totalInvoice > 0)
                    {
                        row.DISCOUNT_RATIO = Math.Round((row.DISCOUNT / this.totalInvoice) * 100m, 0, MidpointRounding.AwayFromZero);
                        this.gvInvoiceDiscount.SetRowCellValue(e.RowHandle, this.gcInvoiceDiscountRatio, row.DISCOUNT_RATIO);
                    }
                }
                else if (e.Column == this.gcInvoiceDiscountRatio)
                {
                    if (this.totalInvoice > 0)
                    {
                        row.DISCOUNT = Math.Round((row.DISCOUNT_RATIO * this.totalInvoice) / 100m, 4);
                        this.gvInvoiceDiscount.SetRowCellValue(e.RowHandle, this.gcInvoiceDiscount, row.DISCOUNT);
                    }
                }

                UpdateInvoiceAmountAfterDiscount();
                this.CalcuCanThu(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Public helpers
        private static bool IsEmptyDiscountRow(TransactionDiscountADO item)
        {
            if (item == null) return true;
            return item.DISCOUNT == 0
                && item.DISCOUNT_RATIO == 0
                && string.IsNullOrWhiteSpace(item.REASON);
        }

        internal decimal GetTotalRecieptDiscount()
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount || this.bindRecieptDiscount == null) return 0;
                return this.bindRecieptDiscount.Where(o => !IsEmptyDiscountRow(o)).Sum(o => o.DISCOUNT);
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
                return this.bindInvoiceDiscount.Where(o => !IsEmptyDiscountRow(o)).Sum(o => o.DISCOUNT);
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
                        DISCOUNT = item.DISCOUNT,
                        DISCOUNT_RATIO = (long?)Math.Round(item.DISCOUNT_RATIO, 0, MidpointRounding.AwayFromZero),
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
                        DISCOUNT = item.DISCOUNT,
                        DISCOUNT_RATIO = (long?)Math.Round(item.DISCOUNT_RATIO, 0, MidpointRounding.AwayFromZero),
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

        internal void AttachTransactionDiscountList(HIS_TRANSACTION transaction, List<HIS_TRANSACTION_DISCOUNT> discounts)
        {
            try
            {
                if (transaction == null) return;
                if (discounts == null || discounts.Count == 0) return;
                transaction.HIS_TRANSACTION_DISCOUNT = discounts;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ResetGridDiscount()
        {
            try
            {
                if (!HisConfig.EnableMultiDiscount) return;

                if (this.bindRecieptDiscount != null)
                {
                    this.bindRecieptDiscount.Clear();
                }
                if (this.bindInvoiceDiscount != null)
                {
                    this.bindInvoiceDiscount.Clear();
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
