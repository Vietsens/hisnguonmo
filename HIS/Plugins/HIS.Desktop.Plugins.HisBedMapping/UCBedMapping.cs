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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraMap;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisBedMapping.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisBedMapping
{
    public partial class UCBedMapping : UserControlBase
    {
        GridHitInfo hitInfo = null;
        private static int do_rong = 200;
        private static int do_cao = 130;

        List<V_HIS_BED> datas = new List<V_HIS_BED>();
        List<Bed> listBed = new List<Bed>();
        List<V_HIS_BED> bedHasXY = null;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        public UCBedMapping()
            : base(null)
        {
            InitializeComponent();
        }

        public UCBedMapping(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCBedMapping_Load(object sender, EventArgs e)
        {
            InItBedRoomCombo();
            //InitBedRoom(this.datas);
        }

        private void FillDataToGridBed(long bedRoomId)
        {
            try
            {
                MOS.Filter.HisBedViewFilter filter = new MOS.Filter.HisBedViewFilter();
                filter.IS_ACTIVE = 1;
                filter.BED_ROOM_ID = bedRoomId;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "BED_CODE";
                var bedList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BED>>("api/HisBed/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null).ToList();
                this.datas = bedList.Where(o => String.IsNullOrWhiteSpace(o.X) && String.IsNullOrWhiteSpace(o.Y)).ToList();
                gridControlBed.BeginUpdate();
                gridControlBed.DataSource = this.datas;
                gridControlBed.EndUpdate();
                this.listBed = new List<Bed>();
                bedHasXY = bedList.Where(o => !String.IsNullOrWhiteSpace(o.X) || !String.IsNullOrWhiteSpace(o.Y)).ToList();
                xtraScrollableControl1.Controls.Clear();
                InitBedRoom(bedHasXY);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraScrollableControl1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Point p = xtraScrollableControl1.PointToClient(new Point(e.X, e.Y));
                Control c = this.xtraScrollableControl1.GetChildAtPoint(p);
                Bed b = null;
                if (c != null && c is Bed)
                    b = (Bed)c;

                V_HIS_BED d = (V_HIS_BED)e.Data.GetData(typeof(V_HIS_BED));
                Bed old = listBed.FirstOrDefault(o => o.Tag == d);

                Bed seat = new Bed();
                seat.Id = d.ID;
                seat.Text = String.Format("{0} - {1}", d.BED_CODE, d.BED_NAME);
                seat.Font = new Font(seat.Font.Name, 12);
                seat.TextAlign = ContentAlignment.MiddleLeft;
                seat.BackgroundImage = imageCollection.Images[6];
                seat.BackgroundImageLayout = ImageLayout.Center;
                seat.ForeColor = Color.White;
                seat.Tag = d;
                seat.MouseMove += seat_MouseMove;

                int x = 0;
                int y = 0;
                int x1 = p.X / do_rong;
                int x2 = p.X % do_rong;
                if (x2 > 0) x = x1;
                else x = x1 - 1;

                int y1 = p.Y / do_cao;
                int y2 = p.Y % do_cao;
                if (y2 > 0) y = y1;
                else y = y1 - 1;
                seat.GridX = x;
                seat.GridY = y;


                seat.Location = new System.Drawing.Point(x * do_rong + (x > 0 ? (x) * 10 : 0), y * do_cao);
                seat.Size = new System.Drawing.Size(do_rong, do_cao);
                seat.TabIndex = 0;
                seat.TabStop = false;

                if (b != null)
                {
                    List<Bed> olds = listBed.Where(o => o.GridY == b.GridY && o.GridX >= b.GridX).ToList();
                    olds.ForEach(o =>
                    {
                        o.GridX++;
                        o.Location = new System.Drawing.Point(o.GridX * do_rong + (o.GridX > 0 ? (o.GridX) * 10 : 0), o.GridY * do_cao);
                    });
                }
                if (old != null)
                    listBed.Remove(old);

                listBed.Add(seat);

                if (old != null)
                {
                    List<Bed> olds = listBed.Where(o => o.GridY == old.GridY && o.GridX >= old.GridX).ToList();
                    olds.ForEach(o =>
                    {
                        o.GridX--;
                        o.Location = new System.Drawing.Point(o.GridX * do_rong + (o.GridX > 0 ? (o.GridX) * 10 : 0), o.GridY * do_cao);
                    });
                }

                if (datas.Contains(d))
                    datas.Remove(d);
                gridControlBed.BeginUpdate();
                gridControlBed.DataSource = datas;
                gridControlBed.EndUpdate();
                if (old != null)
                    this.xtraScrollableControl1.Controls.Remove(old);
                this.xtraScrollableControl1.Controls.Add(seat);
                xtraScrollableControl1.Refresh();
            }
            catch (Exception ex)
            {

            }
        }

        void seat_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left) return;
                Bed b = (Bed)sender;
                b.DoDragDrop(b.Tag, DragDropEffects.Move);
            }
            catch (Exception ex)
            {

            }
        }

        private void gridControl1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void gridControl1_MouseDown(object sender, MouseEventArgs e)
        {
            hitInfo = gridViewBed.CalcHitInfo(new Point(e.X, e.Y));
        }

        private void gridControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (hitInfo == null) return;
            if (e.Button != MouseButtons.Left) return;
            Rectangle dragRect = new Rectangle(new Point(
                hitInfo.HitPoint.X - SystemInformation.DragSize.Width / 2,
                hitInfo.HitPoint.Y - SystemInformation.DragSize.Height / 2), SystemInformation.DragSize);
            if (!(hitInfo.RowHandle == GridControl.InvalidRowHandle) && !dragRect.Contains(new Point(e.X, e.Y)))
            {
                V_HIS_BED data = (V_HIS_BED)gridViewBed.GetRow(hitInfo.RowHandle);
                gridControlBed.DoDragDrop(data, DragDropEffects.Move);
            }
        }

        private void xtraScrollableControl1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void xtraScrollableControl1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void gridControl1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void gridControl1_MouseUp(object sender, MouseEventArgs e)
        {
            this.hitInfo = null;
        }

        private void gridControl1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                V_HIS_BED d = (V_HIS_BED)e.Data.GetData(typeof(V_HIS_BED));
                if (datas.Contains(d))
                {
                    return;
                }
                Bed b = listBed.FirstOrDefault(o => o.Tag == d);

                List<Bed> olds = listBed.Where(o => o.GridX >= b.GridX && o.GridY == b.GridY).ToList();
                olds.ForEach(o =>
                {
                    o.GridX--;
                    o.Location = new System.Drawing.Point(o.GridX * do_rong + (o.GridX > 0 ? (o.GridX) * 10 : 0), o.GridY * do_cao);
                });

                datas.Add((V_HIS_BED)b.Tag);
                listBed.Remove(b);
                this.xtraScrollableControl1.Controls.Remove(b);
                this.xtraScrollableControl1.Refresh();
                gridControlBed.BeginUpdate();
                gridControlBed.DataSource = datas;
                gridControlBed.EndUpdate();

            }
            catch (Exception ex)
            {

            }
        }

        private void InItBedRoomCombo()
        {
            try
            {
                var currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                var bedRoomData = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID).ToList();
                var bedRoom = bedRoomData.Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BED_ROOM_CODE", "", 80, 1));
                columnInfos.Add(new ColumnInfo("BED_ROOM_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BED_ROOM_NAME", "ID", columnInfos, false, 280);
                ControlEditorLoader.Load(this.cboBedRoom, bedRoom, controlEditorADO);
                var currentBed = bedRoom.FirstOrDefault(o => o.ROOM_ID == currentRoom.ID);
                cboBedRoom.EditValue = currentBed.ID;
                txtBedRoom.Text = currentBed.BED_ROOM_CODE;
                FillDataToGridBed(currentBed.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBedRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var bedRoom = BackendDataWorker.Get<V_HIS_BED_ROOM>().FirstOrDefault(o => o.BED_ROOM_CODE.ToUpper() == txtBedRoom.Text.Trim().ToUpper());
                    if (bedRoom != null)
                    {
                        cboBedRoom.EditValue = bedRoom.ID;
                        txtBedRoom.Text = bedRoom.BED_ROOM_CODE;
                        FillDataToGridBed(bedRoom.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBedRoom.EditValue != null)
                    {
                        var bedRoom = BackendDataWorker.Get<V_HIS_BED_ROOM>().FirstOrDefault(o => o.ID == (long)cboBedRoom.EditValue);
                        if (bedRoom != null)
                        {
                            txtBedRoom.Text = bedRoom.BED_ROOM_CODE;
                            FillDataToGridBed(bedRoom.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void InitBedRoom(List<V_HIS_BED> lstBed)
        {
            try
            {
                if (lstBed != null && lstBed.Count > 0)
                {
                    lstBed = lstBed.OrderBy(o => o.BED_CODE).ToList();
                    foreach (var bed in lstBed)
                    {
                        var seat = new Bed();
                        seat.Id = bed.ID;
                        seat.Font = new Font(seat.Font.Name, 12);
                        seat.TextAlign = ContentAlignment.MiddleLeft;
                        seat.BackgroundImage = imageCollection.Images[6];
                        seat.BackgroundImageLayout = ImageLayout.Center;
                        //seat.BackColor = Color.RosyBrown;

                        seat.ForeColor = Color.White;

                        //List<V_HIS_BED_LOG_3> bedLog = new List<V_HIS_BED_LOG_3>();
                        //if (listbedcurr != null && listbedcurr.Count > 0)
                        //{
                        //    bedLog = listbedcurr.Where(o => o.BED_ID == bed.ID).ToList();
                        //}

                        //seat.Text = "  " + bed.BED_NAME + "\n\r";

                        //if (bedLog != null && bedLog.Count > 0)
                        //{
                        //    List<string> names = new List<string>();
                        //    foreach (var item in bedLog)
                        //    {
                        //        names.Add(string.Format("  {0}", item.TDL_PATIENT_NAME));
                        //    }
                        //    seat.Text += String.Join("\n\r", names);
                        //    //seat.BackColor = Color.Red;
                        //    seat.BackgroundImage = imageCollection.Images[2];
                        //}

                        int x = 0;
                        int y = 0;
                        x = Inventec.Common.TypeConvert.Parse.ToInt32(bed.X);
                        y = Inventec.Common.TypeConvert.Parse.ToInt32(bed.Y);

                        //if (x <= 0 && y <= 0)// không có tọa độ được thiết lập
                        //{
                        //    GetMinXY(ref x, ref y);
                        //}
                        //else if (x > 0 && y > 0)// có tọa độ thiết lập
                        //{
                        //    x--; y--;
                        //}
                        //else if (x <= 0 && y > 0)// có 1 tọa độ;
                        //{
                        //    x = 0; y--;
                        //}
                        //else if (x > 0 && y <= 0)// có 1 tọa độ;
                        //{
                        //    x--; y = 0;
                        //}
                        seat.Tag = bed;
                        seat.GridX = x;
                        seat.GridY = y;
                        seat.Location = new System.Drawing.Point(seat.GridX * do_rong + (seat.GridX > 0 ? (seat.GridX) * 10 : 0), seat.GridY * do_cao);
                        seat.Size = new System.Drawing.Size(do_rong, do_cao);
                        seat.MouseMove += seat_MouseMove;
                        seat.Text = String.Format("{0} - {1}", bed.BED_CODE, bed.BED_NAME);
                        seat.TabIndex = 0;
                        seat.TabStop = false;
                        this.listBed.Add(seat);
                        xtraScrollableControl1.Controls.Add(seat);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetMinXY(ref int minX, ref int minY)
        {
            try
            {
                Bed[,] ChoNgoi = new Bed[do_cao + 1, do_rong + 1];
                int maxX = 0;
                int maxY = 0;
                foreach (var item in this.xtraScrollableControl1.Controls)
                {
                    if (item is Bed)
                    {
                        var a = item as Bed;
                        ChoNgoi[a.GridX, a.GridY] = a;
                        if (a.GridY > maxY)
                        {
                            maxY = a.GridY;
                        }

                        if (a.GridX > maxX)
                        {
                            maxX = a.GridX;
                        }
                    }
                }

                for (int j = 0; j <= maxY; j++)
                {
                    minY = j;
                    for (int i = 0; i <= maxX; i++)
                    {
                        minX = i;

                        if (ChoNgoi[minX, minY] == null) break;
                        else minX += 1;
                        if (minX > do_cao) break;
                        if (ChoNgoi[minX, minY] == null) break;
                    }

                    if (minX > do_cao)
                    {
                        minX = 1;
                        minY += 1;
                    }

                    if (minY > do_rong)
                    {
                        if (minX > do_cao)
                        {
                            minX = (int)do_cao + 1;
                            minY = (int)do_rong + 1;
                        }
                    }

                    if (ChoNgoi[minX, minY] == null) break;
                }
            }
            catch (Exception ex)
            {
                minX = 0;
                minY = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSave()
        {
            if (btnSave.Enabled)
            {
                btnSave_Click(null, null);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<HIS_BED> ListBedInput = new List<HIS_BED>();
                bool success = false;
                if (this.listBed != null && this.listBed.Count > 0)
                {
                    foreach (var item in this.listBed)
                    {
                        HIS_BED bed = new HIS_BED();
                        if (item.GridX < 0 || item.GridY < 0)
                            continue;
                        else
                        {
                            bed.ID = item.Id;
                            bed.X = item.GridX + "";
                            bed.Y = item.GridY + "";
                            ListBedInput.Add(bed);
                        }
                    }

                    var bedRemoveList = bedHasXY != null && bedHasXY.Count > 0 ? bedHasXY.Where(o => !this.listBed.Select(p => p.Id).Contains(o.ID)).ToList() : null;

                    if (bedRemoveList != null && bedRemoveList.Count > 0)
                    {
                        foreach (var item in bedRemoveList)
                        {
                            HIS_BED bed = new HIS_BED();

                            bed.ID = item.ID;
                            bed.X = null;
                            bed.Y = null;
                            ListBedInput.Add(bed);
                        }
                    }
                }
                if (ListBedInput == null || ListBedInput.Count == 0)
                {
                    MessageManager.Show("Chưa có giường để cập nhật");
                    return;

                }
                var rsHisBed = new BackendAdapter(param).Post<List<HIS_BED>>("api/HisBed/UpdateMap", ApiConsumers.MosConsumer, ListBedInput, param);
                if (rsHisBed != null && rsHisBed.Count > 0)
                {
                    success = true;
                }
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

}
