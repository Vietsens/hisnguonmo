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
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockInventory
{
    public partial class frmMediStockInventory
    {

        private void btnDownloadFile_Click(object sender, EventArgs e)
        {
            try
            {

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_MEDISTOCK_INVENTORY.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_MEDISTOCK_INVENTORY";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {

        }

        //private void Import()
        //{
        //    try
        //    {
        //        string dongLoi = "Dòng lỗi";

        //        OpenFileDialog ofd = new OpenFileDialog();
        //        ofd.Multiselect = false;
        //        if (ofd.ShowDialog() == DialogResult.OK)
        //        {
        //            WaitingManager.Show();

        //            var import = new Inventec.Common.ExcelImport.Import();
        //            if (import.ReadFileExcel(ofd.FileName))
        //            {
        //                var hisServiceImport = import.GetWithCheck<BedADO>(0);
        //                if (hisServiceImport != null && hisServiceImport.Count > 0)
        //                {
        //                    List<BedADO> listAfterRemove = new List<BedADO>();


        //                    foreach (var item in hisServiceImport)
        //                    {
        //                        bool checkNull = string.IsNullOrEmpty(item.BED_CODE)
        //                            && string.IsNullOrEmpty(item.BED_NAME)
        //                            && string.IsNullOrEmpty(item.BED_ROOM_CODE)
        //                            && string.IsNullOrEmpty(item.BED_TYPE_CODE)
        //                            ;

        //                        if (!checkNull)
        //                        {
        //                            listAfterRemove.Add(item);
        //                        }
        //                    }
        //                    WaitingManager.Hide();

        //                    this._CurrentAdos = listAfterRemove;

        //                    if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
        //                    {
        //                        btnShowLineError.Enabled = true;
        //                        this._BedAdos = new List<BedADO>();
        //                        addServiceToProcessList(_CurrentAdos, ref this._BedAdos);
        //                        SetDataSource(this._BedAdos);
        //                    }

        //                    //btnSave.Enabled = true;
        //                }
        //                else
        //                {
        //                    WaitingManager.Hide();
        //                    DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
        //                }
        //            }
        //            else
        //            {
        //                WaitingManager.Hide();
        //                DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void SaveImport()
        //{
        //    try
        //    {
        //        bool success = false;
        //        WaitingManager.Show();
        //        List<HIS_BED> datas = new List<HIS_BED>();

        //        if (this._BedAdos != null && this._BedAdos.Count > 0)
        //        {
        //            foreach (var item in this._BedAdos)
        //            {
        //                HIS_BED ado = new HIS_BED();
        //                ado.BED_CODE = item.BED_CODE;
        //                ado.BED_NAME = item.BED_NAME;
        //                ado.BED_ROOM_ID = item.BED_ROOM_ID;
        //                ado.BED_TYPE_ID = item.BED_TYPE_ID;
        //                ado.MAX_CAPACITY = item.MAX_CAPACITY;
        //                ado.X = item.X;
        //                ado.Y = item.Y;
        //                datas.Add(ado);
        //            }
        //        }
        //        else
        //        {
        //            WaitingManager.Hide();
        //            DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
        //            return;
        //        }

        //        CommonParam param = new CommonParam();
        //        var dataImports = new BackendAdapter(param).Post<List<HIS_BED>>("api/HisBed/CreateList", ApiConsumers.MosConsumer, datas, param);
        //        WaitingManager.Hide();
        //        if (dataImports != null && dataImports.Count > 0)
        //        {
        //            success = true;
        //            btnImport.Enabled = false;
        //            LoadDataBed();
        //            if (this.delegateRefresh != null)
        //            {
        //                this.delegateRefresh();
        //            }
        //        }

        //        MessageManager.Show(this.ParentForm, param, success);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
    }
}
