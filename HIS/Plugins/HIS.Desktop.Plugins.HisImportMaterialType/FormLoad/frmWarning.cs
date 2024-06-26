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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.HisImportMaterialType.FormLoad
{
    public partial class frmWarning : Form
    {
        HIS.Desktop.Common.DelegateRefreshData currentDelegate;
        List<ADO.MaterialTypeImportADO> currentMaterialTypeImportAdos;

        public frmWarning(List<ADO.MaterialTypeImportADO> data, HIS.Desktop.Common.DelegateRefreshData dele)
        {
            InitializeComponent();
            try
            {
                this.currentMaterialTypeImportAdos = data;
                this.currentDelegate = dele;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmWarning_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #region Event grid Manufacturer

        private void gridViewManufacturer_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                //Loại xử lý khi xảy ra exception Hiển thị. k cho nhập
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.DisplayError;
                //Show thông báo lỗi ở cột
                gridViewManufacturer.SetColumnError(gridViewManufacturer.FocusedColumn, e.ErrorText, ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewManufacturer_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view.FocusedColumn.FieldName == "MANUFACTURER_CODE")
                {
                    if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Trường dữ liệu bắt buộc nhập";
                    }
                    else if (e.Value != null && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(e.Value.ToString(), 6))
                    {
                        e.Valid = false;
                        e.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
                    }
                    else
                    {
                        e.Valid = true;
                    }
                }
                if (view.FocusedColumn.FieldName == "MANUFACTURER_NAME")
                {
                    if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Trường dữ liệu bắt buộc nhập";
                    }
                    else if (e.Value != null && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(e.Value.ToString(), 100))
                    {
                        e.Valid = false;
                        e.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
                    }
                    else
                    {
                        e.Valid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewManufacturer_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_MANUFACTURER data = (HIS_MANUFACTURER)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        //if (e.Column.FieldName == "MANUFACTURER_CODE")
                        //{
                        //    if (string.IsNullOrEmpty(data.MANUFACTURER_CODE))
                        //    {
                        //        gridViewNational.SetColumnError(e.Column, "Trường dữ liệu bắt buộc", ErrorType.Warning);
                        //    }
                        //    if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(data.MANUFACTURER_CODE, 6))
                        //    {
                        //        gridViewNational.SetColumnError(e.Column, "Trường dữ liệu vượt quá ký tự cho phép", ErrorType.Warning);
                        //    }
                        //}
                        //if (e.Column.FieldName == "MANUFACTURER_NAME")
                        //{
                        //    if (string.IsNullOrEmpty(data.MANUFACTURER_NAME))
                        //    {
                        //        gridViewNational.SetColumnError(e.Column, "Trường dữ liệu bắt buộc", ErrorType.Warning);
                        //    }
                        //    if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(data.MANUFACTURER_NAME, 100))
                        //    {
                        //        gridViewNational.SetColumnError(e.Column, "Trường dữ liệu vượt quá ký tự cho phép", ErrorType.Warning);
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        #region Event form

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success2 = false;
                bool checkManufacturer = false;
                bool checkTick = false;
                List<HIS_MANUFACTURER> hisManuFacturer = new List<HIS_MANUFACTURER>();
               
                if (gridControlManufacturer.DataSource != null)
                {
                    var hasCheck = gridViewManufacturer.GetSelectedRows();
                    if (hasCheck.Count() > 0)
                    {
                        checkTick = true;

                        foreach (var item in hasCheck)
                        {
                            HIS_MANUFACTURER manufacturer = (HIS_MANUFACTURER)gridViewManufacturer.GetRow(item);
                            hisManuFacturer.Add(manufacturer);
                        }

                        if (!CheckValidManufacturer(hisManuFacturer))
                        {
                            checkManufacturer = false;
                            return;
                        }
                        checkManufacturer = true;
                    }
                }

                WaitingManager.Show();
                var rsManufacturer = new BackendAdapter(param).Post<List<HIS_MANUFACTURER>>("api/HisManufacturer/CreateList", ApiConsumer.ApiConsumers.MosConsumer, hisManuFacturer, param);
                if (rsManufacturer != null && rsManufacturer.Count > 0)
                {
                    BackendDataWorker.Reset<HIS_MANUFACTURER>();
                    btnAdd.Enabled = false;
                    success2 = true;
                }

                WaitingManager.Hide();

                if (success2 && this.currentDelegate != null)
                    this.currentDelegate();

                if (!checkTick)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Phải chọn thông tin cần bổ sung hoặc bỏ qua", "Thông báo");
                    return;
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success2);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Method

        private void FillDataToGrid()
        {
            try
            {
                var checkManufacturer = this.currentMaterialTypeImportAdos.Where(o => o.IS_LESS_MANUFACTURER).ToList();
                if (checkManufacturer != null && checkManufacturer.Count > 0)
                {
                    List<HIS_MANUFACTURER> hisManuFacturer = new List<HIS_MANUFACTURER>();
                    foreach (var item in checkManufacturer)
                    {
                        if (!hisManuFacturer.Exists(o => o.MANUFACTURER_CODE == item.MANUFACTURER_CODE && o.MANUFACTURER_NAME == item.MANUFACTURER_NAME))
                        {
                            var manufacturer = new HIS_MANUFACTURER();
                            manufacturer.MANUFACTURER_CODE = item.MANUFACTURER_CODE;
                            manufacturer.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                            hisManuFacturer.Add(manufacturer);
                        }
                    }

                    gridControlManufacturer.BeginUpdate();
                    gridControlManufacturer.DataSource = hisManuFacturer;
                    gridControlManufacturer.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool CheckValidManufacturer(List<HIS_MANUFACTURER> data)
        {
            bool rs = true;
            try
            {
                if (data != null)
                {
                    var maxLengthCode = data.Where(o => Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(o.MANUFACTURER_CODE, 6)).ToList();
                    if (maxLengthCode != null && maxLengthCode.Count > 0)
                    {
                        string mess = "Mã hãng sản xuất: " + String.Join(",", maxLengthCode.Select(o => o.MANUFACTURER_CODE).ToArray()) + " vượt quá ký tự cho phép";
                        DevExpress.XtraEditors.XtraMessageBox.Show(mess, "Thông báo");
                        return false;
                    }

                    var maxLengthName = data.Where(o => Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(o.MANUFACTURER_NAME, 100)).ToList();
                    if (maxLengthName != null && maxLengthName.Count > 0)
                    {
                        string mess = "Tên hãng sản xuất: " + String.Join(",", maxLengthName.Select(o => o.MANUFACTURER_NAME).ToArray()) + " vượt quá ký tự cho phép";
                        DevExpress.XtraEditors.XtraMessageBox.Show(mess, "Thông báo");
                        return false;
                    }

                    var nullCode = data.Where(o => string.IsNullOrEmpty(o.MANUFACTURER_CODE)).ToList();
                    if (nullCode != null && nullCode.Count > 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Phải nhập trường mã hãng sản xuất", "Thông báo");
                        return false;
                    }

                    var nullName = data.Where(o => string.IsNullOrEmpty(o.MANUFACTURER_NAME)).ToList();
                    if (nullName != null && nullName.Count > 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Phải nhập trường tên hãng sản xuất", "Thông báo");
                        return false;
                    }

                    var existCode = data.Where(o => BackendDataWorker.Get<HIS_MANUFACTURER>().Select(p => p.MANUFACTURER_CODE).Contains(o.MANUFACTURER_CODE)).ToList();
                    if (existCode != null && existCode.Count > 0)
                    {
                        string mess = "Mã hãng sản xuất: " + String.Join(",", existCode.Select(o => o.MANUFACTURER_CODE).ToArray()) + " bị trùng";
                        DevExpress.XtraEditors.XtraMessageBox.Show(mess, "Thông báo");
                        return false;
                    }

                    List<string> sameCode = new List<string>();
                    foreach (var item in data)
                    {
                        var exists = data.Where(o => o.MANUFACTURER_CODE == item.MANUFACTURER_CODE).ToList();
                        if (exists != null && exists.Count >= 2)
                        {
                            sameCode = exists.Select(o => o.MANUFACTURER_CODE).ToList();
                        }
                    }

                    if (sameCode != null && sameCode.Count > 0)
                    {
                        var same = sameCode.Distinct().ToArray();
                        string mess = "Mã hãng sản xuất: " + String.Join(",", same) + " bị trùng ";
                        DevExpress.XtraEditors.XtraMessageBox.Show(mess, "Thông báo");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return rs;
        }

        #endregion
    }
}
