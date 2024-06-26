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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.ExpMestOtherExport.ADO;
using HIS.Desktop.Plugins.ExpMestOtherExport.Resources;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestOtherExport.ImportExcel
{
    public partial class frmExcelError : Form
    {
        List<ImportADO> errors;
        public frmExcelError(List<ImportADO> imports)
        {
            InitializeComponent();
            errors = imports;
        }

        private void frmExcelError_Load(object sender, EventArgs e)
        {
            try
            {
                errors = errors != null ? errors.Where(o => o.MessageErrors != null && o.MessageErrors.Count > 0).ToList() : null;
                gridControlImportError.BeginUpdate();
                gridControlImportError.DataSource = errors;
                gridControlImportError.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImportError_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ImportADO data = (ImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (data == null)
                    {
                        return;
                    }
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "TypeName")
                    {
                        if (data.TYPE_ID == 1)
                        {
                            e.Value = "Thuốc";
                        }
                        else if (data.TYPE_ID == 2)
                        {
                            e.Value = "Vật Tư";
                        }
                        else if (data.TYPE_ID == 3)
                        {
                            e.Value = "Máu";
                        }
                        else
                        {
                            e.Value = "Không xác định";
                        }
                    }
                    else if (e.Column.FieldName == "MESSAGE")
                    {
                        e.Value = String.Join(";", data.MessageErrors);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportError_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportError.Enabled) return;
                this.CreateReport();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void CreateReport()
        {
            try
            {
                List<string> expCode = new List<string>();

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "ImportXuatKhacLoi.xlsx");

                //chọn đường dẫn
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.KhongTimThayMauIn, templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng kiểm tra lại mẫu. (" + templateFile + ")");
                        return;
                    }

                    ProcessData(ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                if (MessageBox.Show(Resources.ResourceMessage.XuLyThanhCongBanMuonMoFileKhong,
                                    Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        MessageManager.Show(new CommonParam(), false);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ProcessData(ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                errors.ForEach(o => o.MESSAGE_ERROR = String.Join(";", o.MessageErrors));
                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "ExportResult", errors);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }


    }
}
