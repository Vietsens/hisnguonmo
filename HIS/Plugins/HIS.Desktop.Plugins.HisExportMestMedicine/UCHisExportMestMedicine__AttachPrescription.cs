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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.ADO;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisExportMestMedicine
{
    /// <summary>
    /// Viec 57853: cot icon "Don dinh kem" tren luoi Danh sach xuat — chi phieu xuat ban.
    /// Icon mau = da co don dinh kem, icon xam = chua co; bam icon -> danh sach don cua phieu (xem/in, bo sung, xoa theo trang thai).
    /// Danh dau ca trang bang 1 lan goi API (EMR GetView theo TREATMENT_CODEs = EXP_MEST_CODE).
    /// Cot chi tao khi bat config -> tat config thi luoi giu nguyen nhu cu.
    /// </summary>
    public partial class UCHisExportMestMedicine : UserControlBase
    {
        private const string ATTACH_PRESCRIPTION__FIELD_NAME = "ATTACH_PRESCRIPTION_DISPLAY";
        private const string IMAGE__ATTACH = "images/mail/attach_16x16.png";
        private const string IMAGE__ATTACH_GRAY = "grayscaleimages/mail/attach_16x16.png";

        private bool isAttachPrescriptionEnable = false;
        private GridColumn gcAttachPrescription;
        private RepositoryItemButtonEdit repoBtnAttachPrescriptionHas;
        private RepositoryItemButtonEdit repoBtnAttachPrescriptionNone;
        private RepositoryItemTextEdit repoAttachPrescriptionEmpty;

        /// <summary>EXP_MEST_CODE cua cac phieu ban tren trang hien tai da co don dinh kem</summary>
        private HashSet<string> expMestCodesHasAttach = new HashSet<string>();

        /// <summary>Tao cot icon canh cot "Thanh toan" (goi trong Load, truoc khi nap luoi)</summary>
        private void InitAttachPrescriptionColumn()
        {
            try
            {
                this.isAttachPrescriptionEnable = ExpMestAttachFileProcessor.IsEnable();
                if (!this.isAttachPrescriptionEnable || this.gcAttachPrescription != null)
                    return;

                this.repoBtnAttachPrescriptionHas = CreateAttachButton(IMAGE__ATTACH, ExpMestAttachFileProcessor.GetGridToolTip(true));
                this.repoBtnAttachPrescriptionNone = CreateAttachButton(IMAGE__ATTACH_GRAY, ExpMestAttachFileProcessor.GetGridToolTip(false));
                this.repoAttachPrescriptionEmpty = new RepositoryItemTextEdit();
                this.repoAttachPrescriptionEmpty.ReadOnly = true;
                this.gridControl.RepositoryItems.AddRange(new RepositoryItem[] {
                    this.repoBtnAttachPrescriptionHas, this.repoBtnAttachPrescriptionNone, this.repoAttachPrescriptionEmpty });

                this.gridView.BeginUpdate();
                try
                {
                    this.gcAttachPrescription = new GridColumn();
                    this.gcAttachPrescription.Name = "gcAttachPrescription";
                    this.gcAttachPrescription.FieldName = ATTACH_PRESCRIPTION__FIELD_NAME;
                    this.gcAttachPrescription.Caption = ExpMestAttachFileProcessor.GetGridColumnCaption();
                    this.gcAttachPrescription.ToolTip = ExpMestAttachFileProcessor.GetGridColumnCaption();
                    this.gcAttachPrescription.OptionsColumn.ShowCaption = false;
                    this.gcAttachPrescription.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                    this.gcAttachPrescription.OptionsFilter.AllowFilter = false;
                    this.gcAttachPrescription.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    this.gcAttachPrescription.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    this.gcAttachPrescription.Width = 20;
                    this.gridView.Columns.Add(this.gcAttachPrescription);
                    this.gcAttachPrescription.VisibleIndex = this.gridColumn_Bill.VisibleIndex + 1;
                }
                finally
                {
                    this.gridView.EndUpdate();
                }
                this.gridView.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridView_CustomRowCellEdit_AttachPrescription);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private RepositoryItemButtonEdit CreateAttachButton(string imagePath, string toolTip)
        {
            RepositoryItemButtonEdit repo = new RepositoryItemButtonEdit();
            repo.AutoHeight = false;
            repo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            repo.Buttons.Clear();
            DevExpress.XtraEditors.Controls.EditorButton btn = new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph);
            try
            {
                btn.Image = DevExpress.Images.ImageResourceCache.Default.GetImage(imagePath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            if (btn.Image == null)
                btn.Caption = "...";
            btn.ToolTip = toolTip;
            repo.Buttons.Add(btn);
            repo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnAttachPrescription_ButtonClick);
            return repo;
        }

        /// <summary>Nap danh dau cho trang vua tai — goi trong GridPaging (1 API/trang)</summary>
        private void LoadAttachPrescriptionMarks(List<V_HIS_EXP_MEST_2> data)
        {
            try
            {
                if (!this.isAttachPrescriptionEnable)
                    return;
                List<ExpMestAttachInfoADO> saleExpMests = data != null
                    ? data.Where(o => o.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN && !String.IsNullOrEmpty(o.EXP_MEST_CODE))
                        .Select(o => new ExpMestAttachInfoADO(o)).ToList()
                    : new List<ExpMestAttachInfoADO>();
                this.expMestCodesHasAttach = saleExpMests.Any()
                    ? ExpMestAttachFileProcessor.GetExpMestCodesHasAttach(saleExpMests)
                    : new HashSet<string>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>RowCellEdit chi tra HashSet (O(1)) — khong goi API/LINQ theo dong</summary>
        private void gridView_CustomRowCellEdit_AttachPrescription(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column != this.gcAttachPrescription)
                    return;
                V_HIS_EXP_MEST_2 row = this.gridView.GetRow(e.RowHandle) as V_HIS_EXP_MEST_2;
                if (row == null || row.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                {
                    e.RepositoryItem = this.repoAttachPrescriptionEmpty;
                    return;
                }
                e.RepositoryItem = this.expMestCodesHasAttach.Contains(row.EXP_MEST_CODE ?? "")
                    ? this.repoBtnAttachPrescriptionHas
                    : this.repoBtnAttachPrescriptionNone;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repoBtnAttachPrescription_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_EXP_MEST_2 row = this.gridView.GetFocusedRow() as V_HIS_EXP_MEST_2;
                if (row == null || row.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                    return;
                ExpMestAttachFileProcessor.ShowAttachList(new ExpMestAttachInfoADO(row), this.roomId, () => RefreshAttachPrescriptionMark(row));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Sau khi them/xoa don cua 1 phieu -> cap nhat lai icon cua rieng dong do</summary>
        private void RefreshAttachPrescriptionMark(V_HIS_EXP_MEST_2 row)
        {
            try
            {
                if (row == null || String.IsNullOrEmpty(row.EXP_MEST_CODE)) return;
                var codes = ExpMestAttachFileProcessor.GetExpMestCodesHasAttach(new List<ExpMestAttachInfoADO> { new ExpMestAttachInfoADO(row) });
                if (codes.Contains(row.EXP_MEST_CODE))
                    this.expMestCodesHasAttach.Add(row.EXP_MEST_CODE);
                else
                    this.expMestCodesHasAttach.Remove(row.EXP_MEST_CODE);
                this.gridView.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
