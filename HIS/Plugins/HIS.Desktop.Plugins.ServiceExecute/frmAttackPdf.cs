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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class frmAttackPdf : Form
    {
        List<SereServFileADO> DataList;
        List<SereServFileADO> DataListDefault; 
        Action<List<SereServFileADO>> actionSendListFile { get; set; }
        public frmAttackPdf(List<SereServFileADO> DataList, Action<List<SereServFileADO>> actionSendListFile)
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                this.DataList = DataList;
                this.DataListDefault = DataList;
                this.actionSendListFile = actionSendListFile;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void frmAttackPdf_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => DataList), DataList));
                gridControl1.DataSource = DataList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            try
            {
                if (e.Column.FieldName == "NUM_ORDER")
                {
                    var rowData = (SereServFileADO)gridView1.GetFocusedRow();
                    if (rowData.NUM_ORDER != null && rowData.NUM_ORDER > DataList.Count)
                    {
                        rowData.NUM_ORDER = DataList.Count;
                    }

                    HIS_SERE_SERV_FILE success = null;
                    if (rowData != null && rowData.ID > 0)
                    {
                        CommonParam param = new CommonParam();
                        HIS_SERE_SERV_FILE sereServInput = new HIS_SERE_SERV_FILE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_FILE>(sereServInput, rowData);
                        success = new BackendAdapter(param).Post<HIS_SERE_SERV_FILE>("api/HisSereServFile/Update", ApiConsumers.MosConsumer, sereServInput, param);
                    }
                    //{
                    var otherRow = DataList.FirstOrDefault(o => o.SERE_SERV_FILE_NAME != rowData.SERE_SERV_FILE_NAME && o.NUM_ORDER == rowData.NUM_ORDER);
                    if (otherRow != null && rowData.OldNumOrder != 0)
                    {
                        otherRow.NUM_ORDER = rowData.OldNumOrder;
                    }
                    //}
                    DataList = DataList.OrderBy(o => o.NUM_ORDER).ToList();
                    gridControl1.DataSource = null;
                    gridControl1.DataSource = DataList;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (SereServFileADO)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        if (rowData.ID > 0)
                        {
                            success = new BackendAdapter(param).Post<bool>("api/HisSereServFile/Delete", ApiConsumers.MosConsumer, rowData, param);
                            if (success)
                            {
                                DataList = DataList.Where(o => o.ID != rowData.ID).ToList();
                            }
                        }
                        else
                        {
                            DataList = DataList.Where(o => o.SERE_SERV_FILE_NAME != rowData.SERE_SERV_FILE_NAME).ToList();
                        }
                        int index = 1;
                        DataList.ForEach(o =>
                        {
                            var OldNumOrder = o.NUM_ORDER;
                            o.NUM_ORDER = index++;
                            if (o.ID > 0 && OldNumOrder != o.NUM_ORDER)
                            {
                                HIS_SERE_SERV_FILE obj = null;
                                if (rowData != null && rowData.ID > 0)
                                {
                                    HIS_SERE_SERV_FILE sereServInput = new HIS_SERE_SERV_FILE();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_FILE>(sereServInput, rowData);
                                    obj = new BackendAdapter(param).Post<HIS_SERE_SERV_FILE>("api/HisSereServFile/Update", ApiConsumers.MosConsumer, sereServInput, param);
                                }
                            }
                        });
                        gridControl1.DataSource = null;
                        gridControl1.DataSource = DataList;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                var rowData = (SereServFileADO)gridView1.GetFocusedRow();
                string output = Utils.GenerateTempFileWithin();
                if (rowData != null && rowData.ID > 0)
                {
                    var streamSource = Inventec.Fss.Client.FileDownload.GetFile(rowData.URL);
                    streamSource.Position = 0;
                    Utils.ByteToFile(Utils.StreamToByte(streamSource), output);
                }
                else
                {
                    output = rowData.URL;
                }

                Inventec.Common.DocumentViewer.Template.frmPdfViewer DocumentView = new Inventec.Common.DocumentViewer.Template.frmPdfViewer(output);

                DocumentView.Text = "Xem văn bản";

                DocumentView.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnFolder_Click(object sender, EventArgs e)
        {

            try
            {
                if (DataList.Count > 5)
                {
                    XtraMessageBox.Show("Giới hạn tối đa 5 tệp đính kèm.");
                    return;
                }
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Multiselect = true;
                openFile.Filter = "Pdf files (*.pdf)|*.pdf";
                openFile.DefaultExt = ".pdf";
                if (openFile.ShowDialog() == DialogResult.OK)
                {

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => openFile.FileNames), openFile.FileNames));
                    foreach (var item in openFile.FileNames)
                    {
                        if (DataList.Count >= 5)
                            break;
                        var data = new SereServFileADO()
                        {
                            SERE_SERV_FILE_NAME = item.Substring(item.LastIndexOf("\\") + 1),
                            NUM_ORDER = DataList.Count + 1,
                            URL = item
                        };
                        var nameExist = DataList.Where(o => o.URL == data.URL);
                        if (nameExist != null && nameExist.Count() > 0)
                        {
                            data.SERE_SERV_FILE_NAME = data.SERE_SERV_FILE_NAME.Substring(0, data.SERE_SERV_FILE_NAME.LastIndexOf(".")) + "_" + (nameExist.Count() + 1) + ".pdf";
                        }
                        data.IsNow = true;
                        DataList.Add(data);
                    }
                }
                DataList = DataList.OrderBy(o => o.NUM_ORDER).ToList();
                gridControl1.DataSource = null;
                gridControl1.DataSource = DataList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        bool IsSave = false;
        private void btnSave_Click(object sender, EventArgs e)
        {
            IsSave = true;
            actionSendListFile(DataList);
            this.Close();
        }

        private void repStt_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                var rowData = (SereServFileADO)gridView1.GetFocusedRow();
                SpinEdit spin = sender as SpinEdit;
                if (spin != null)
                {
                    rowData.OldNumOrder = Convert.ToInt64(spin.OldEditValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmAttackPdf_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!IsSave)
                actionSendListFile(DataList.Where(o=>!o.IsNow).ToList());
        }
    }
}
