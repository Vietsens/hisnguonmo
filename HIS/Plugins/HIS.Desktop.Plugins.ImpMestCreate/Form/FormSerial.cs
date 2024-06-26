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
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestCreate.Form
{
    internal delegate void DelegateReturnSerial(string data);

    internal partial class FormSerial : FormBase
    {
        class SerialData
        {
            public string SERIAL_NUMBER { get; set; }
            public bool ADD { get; set; }
        }

        List<SerialData> ListData = new List<SerialData>();
        long num;
        DelegateReturnSerial delegateSerial;

        internal FormSerial(string serial, long rownum, DelegateReturnSerial returnSerial)
        {
            InitializeComponent();
            try
            {
                if (!String.IsNullOrWhiteSpace(serial))
                {
                    string[] splitSerial = serial.Split(';');
                    foreach (var item in splitSerial)
                    {
                        ListData.Add(new SerialData() { SERIAL_NUMBER = item });
                    }
                }
                this.num = rownum;

                if (returnSerial != null)
                {
                    delegateSerial = returnSerial;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormSerial_Load(object sender, EventArgs e)
        {
            try
            {
                if (ListData.Count < num)
                {
                    var count = num - ListData.Count;
                    for (int i = 0; i < count; i++)
                    {
                        ListData.Add(new SerialData());
                    }
                }

                if (ListData.Count == 0)
                {
                    ListData.Add(new SerialData());
                }

                ListData.First().ADD = true;

                gridControl1.BeginUpdate();
                gridControl1.DataSource = ListData;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                BtnAdd.Focus();

                if (ListData.Exists(o => String.IsNullOrWhiteSpace(o.SERIAL_NUMBER)))
                {
                    MessageBox.Show("Bạn chưa nhập đủ số serial.");
                    return;
                }

                //if (ListData.Count > num)
                //{
                //    MessageBox.Show("Số lượng serial nhiều hơn số lần dùng tối đa, vui lòng kiểm tra lại.");
                //    return;
                //}

                //if (ListData.Count < num)
                //{
                //    MessageBox.Show("Số lượng serial ít hơn số lần dùng tối đa, vui lòng kiểm tra lại.");
                //    return;
                //}

                delegateSerial(string.Join(";", ListData.Select(s => s.SERIAL_NUMBER).ToList()));
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (SerialData)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ListData.Add(new SerialData());

                gridControl1.BeginUpdate();
                gridControl1.DataSource = ListData;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnRemove_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SerialData)gridView1.GetFocusedRow();
                ListData.Remove(row);

                if (ListData == null || ListData.Count <= 0)
                {
                    ListData = new List<SerialData>();
                    ListData.Add(new SerialData() { ADD = true });
                }

                gridControl1.BeginUpdate();
                gridControl1.DataSource = ListData;
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "DELETE")
                    {
                        var data = (SerialData)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.RowHandle];
                        if (data != null)
                        {
                            try
                            {
                                if (data.ADD)
                                {
                                    e.RepositoryItem = repositoryItemBtnAdd;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemBtnRemove;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControl1_ProcessGridKey(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                (gridControl1.FocusedView as DevExpress.XtraGrid.Views.Base.ColumnView).FocusedRowHandle++;
                e.Handled = true;
            }
        }

        private void barButtonAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
