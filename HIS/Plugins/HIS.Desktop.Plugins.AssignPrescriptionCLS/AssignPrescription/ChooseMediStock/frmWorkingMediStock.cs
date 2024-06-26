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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmWorkingMediStock : FormBase
    {
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStocks;
        List<MediStockADO> currentMediStockADOs;
        Action<List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>> ChooseMediStock;
        bool statecheckColumn = false;

        public frmWorkingMediStock(List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> mediStocks, Action<List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>> chooseMediStock)
            : base()
        {
            InitializeComponent();
            this.ChooseMediStock = chooseMediStock;
            this.currentMediStocks = mediStocks;
        }

        private void frmWorkingMediStock_Load(object sender, EventArgs e)
        {
            try
            {
                this.currentMediStockADOs = (from m in currentMediStocks select new MediStockADO(m)).Distinct().ToList();
                gridControlRooms.DataSource = this.currentMediStockADOs;
                this.gridViewRooms.FocusedRowHandle = 0;
                this.SetCheckAllColumn(this.statecheckColumn);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRooms_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var room = (MediStockADO)this.gridViewRooms.GetFocusedRow();
                if (room != null)
                {
                    var roomInList = this.currentMediStockADOs.FirstOrDefault(o => o.ID == room.ID);
                    if (roomInList != null)
                    {
                        roomInList.IsChecked = room.IsChecked;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewRooms_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }

                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    else if (hi.InColumnPanel)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            statecheckColumn = !statecheckColumn;
                            this.SetCheckAllColumn(statecheckColumn);
                            this.GridCheckChange(statecheckColumn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRooms_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (this.gridViewRooms.IsEditing)
                        this.gridViewRooms.CloseEditor();

                    if (this.gridViewRooms.FocusedRowModified)
                        this.gridViewRooms.UpdateCurrentRow();

                    var room = (MediStockADO)this.gridViewRooms.GetFocusedRow();
                    if (room != null)
                    {
                        this.gridViewRooms.BeginUpdate();
                        var roomAlls = this.gridControlRooms.DataSource as List<MediStockADO>;
                        if (roomAlls != null)
                        {
                            foreach (var ro in roomAlls)
                            {
                                if (ro.ID == room.ID)
                                {
                                    ro.IsChecked = !ro.IsChecked;
                                }
                            }

                            var roomInList = this.currentMediStockADOs.FirstOrDefault(o => o.ID == room.ID);
                            if (roomInList != null)
                            {
                                roomInList.IsChecked = room.IsChecked;
                            }

                            this.gridViewRooms.GridControl.DataSource = roomAlls;
                        }
                        this.gridViewRooms.EndUpdate();
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    btnOK_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewRooms_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                gridControlRooms.BeginUpdate();
                var userRoomADO = (MediStockADO)this.gridViewRooms.GetFocusedRow();
                if (userRoomADO != null)
                {
                    userRoomADO.IsChecked = !userRoomADO.IsChecked;
                    this.gridControlRooms.RefreshDataSource();
                }
                gridControlRooms.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SetCheckAllColumn(bool state)
        {
            try
            {
                this.gridColumnCheck.ImageAlignment = StringAlignment.Center;
                this.gridColumnCheck.Image = (state ? this.imageCollection1.Images[1] : this.imageCollection1.Images[0]);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void GridCheckChange(bool checkedAll)
        {
            try
            {
                foreach (var item in this.currentMediStockADOs)
                {
                    item.IsChecked = checkedAll;
                }
                this.gridViewRooms.BeginUpdate();
                this.gridViewRooms.GridControl.DataSource = this.currentMediStockADOs;
                this.gridViewRooms.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> roomCheckeds = new List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>();
                if (this.currentMediStockADOs != null)
                {
                    if (this.Check(param, ref roomCheckeds))
                    {
                        this.ChangeLockButtonWhileProcess(false);
                        if (this.ChooseMediStock != null)
                            this.ChooseMediStock(roomCheckeds);
                        this.ChangeLockButtonWhileProcess(true);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void ChangeLockButtonWhileProcess(bool isLock)
        {
            try
            {
                this.btnOK.Enabled = isLock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool Check(CommonParam param, ref List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> roomCheckeds)
        {
            bool valid = true;
            try
            {
                var roomIdCheckeds = this.currentMediStockADOs.Where(o => o.IsChecked).Select(o => o.ID).ToList();
                if (roomIdCheckeds == null || roomIdCheckeds.Count == 0)
                {
                    MessageManager.Show(HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources.ResourceMessage.KhongChonPhongLamViec);
                    return false;
                }

                roomCheckeds = this.currentMediStocks.Where(o => roomIdCheckeds.Contains(o.ID)).ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return valid;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnOK_Click(null, null);
        }
    }
}
