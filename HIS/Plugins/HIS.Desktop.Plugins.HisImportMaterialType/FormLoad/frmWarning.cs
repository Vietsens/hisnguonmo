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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportMaterialType.FormLoad
{
    public partial class frmWarning : Form
    {
        #region Declare
        HIS.Desktop.Common.DelegateRefreshData currentDelegate;
        List<ADO.MaterialTypeImportADO> currentMaterialTypeImportAdos;

        /// <summary>Danh sách ĐVT mới sẽ tạo</summary>
        List<HIS_SERVICE_UNIT> listServiceUnitNew;
        /// <summary>Danh sách hãng SX mới sẽ tạo</summary>
        List<HIS_MANUFACTURER> listManufacturerNew;

        /// <summary>Chế độ nhập theo tên (BẬT). false = theo mã (TẮT: nhập mã tay)</summary>
        bool importByName;
        #endregion

        #region Constructor
        public frmWarning(List<ADO.MaterialTypeImportADO> data, HIS.Desktop.Common.DelegateRefreshData dele)
        {
            InitializeComponent();
            try
            {
                this.currentMaterialTypeImportAdos = data;
                this.currentDelegate = dele;
                this.importByName = Config.ImportByNameCFG.IsImportByName();
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event form
        private void frmWarning_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureColumnsByMode();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceUnit_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewManufacturer_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.DisplayError;
                view.SetColumnError(view.FocusedColumn, e.ErrorText, ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                if (importByName) return; // BẬT: mã ẩn, không validate

                GridView view = sender as GridView;
                string field = view.FocusedColumn.FieldName;
                int maxLen = (field == "SERVICE_UNIT_CODE") ? 3 : ((field == "MANUFACTURER_CODE") ? 6 : 0);

                if (field == "SERVICE_UNIT_CODE" || field == "MANUFACTURER_CODE")
                {
                    if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Trường dữ liệu bắt buộc nhập";
                    }
                    else if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(e.Value.ToString(), maxLen))
                    {
                        e.Valid = false;
                        e.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
                    }
                    else
                    {
                        e.Valid = true;
                    }
                }
                else if (field == "SERVICE_UNIT_NAME" || field == "MANUFACTURER_NAME")
                {
                    if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Trường dữ liệu bắt buộc nhập";
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Commit ô đang sửa (nếu có)
                gridViewServiceUnit.CloseEditor();
                gridViewServiceUnit.UpdateCurrentRow();
                gridViewManufacturer.CloseEditor();
                gridViewManufacturer.UpdateCurrentRow();

                bool hasServiceUnit = listServiceUnitNew != null && listServiceUnitNew.Count > 0;
                bool hasManufacturer = listManufacturerNew != null && listManufacturerNew.Count > 0;

                if (!hasServiceUnit && !hasManufacturer)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có dữ liệu cần bổ sung", "Thông báo");
                    return;
                }

                // Chế độ TẮT: bắt buộc nhập mã hợp lệ (mã tự sinh chỉ áp dụng khi BẬT)
                if (!importByName)
                {
                    if (hasServiceUnit && !CheckValidServiceUnit(listServiceUnitNew)) return;
                    if (hasManufacturer && !CheckValidManufacturer(listManufacturerNew)) return;
                }

                CommonParam param = new CommonParam();
                bool success = true;

                WaitingManager.Show();

                if (hasServiceUnit)
                {
                    var rsUnit = new BackendAdapter(param).Post<List<HIS_SERVICE_UNIT>>(
                        HisRequestUriStore.MOSHIS_HIS_SERVICE_UNIT_CREATE_LIST,
                        ApiConsumers.MosConsumer, listServiceUnitNew, param);
                    if (rsUnit != null && rsUnit.Count > 0)
                        BackendDataWorker.Reset<HIS_SERVICE_UNIT>();
                    else
                        success = false;
                }

                if (success && hasManufacturer)
                {
                    var rsManu = new BackendAdapter(param).Post<List<HIS_MANUFACTURER>>(
                        HisRequestUriStore.MOSHIS_HIS_MANUFACTURER_CREATE_LIST,
                        ApiConsumers.MosConsumer, listManufacturerNew, param);
                    if (rsManu != null && rsManu.Count > 0)
                        BackendDataWorker.Reset<HIS_MANUFACTURER>();
                    else
                        success = false;
                }

                WaitingManager.Hide();

                if (success && this.currentDelegate != null)
                    this.currentDelegate();

                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);

                if (success)
                    this.Close();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Method
        /// <summary>
        /// Cấu hình cột theo config: BẬT -> ẩn cột mã, read-only (mã tự sinh);
        /// TẮT -> hiện cột mã + tên cho nhập tay.
        /// </summary>
        private void ConfigureColumnsByMode()
        {
            try
            {
                if (importByName)
                {
                    colServiceUnitCode.Visible = false;
                    colManufacturerCode.Visible = false;
                    gridViewServiceUnit.OptionsBehavior.Editable = false;
                    gridViewManufacturer.OptionsBehavior.Editable = false;
                    lcgServiceUnit.Text = "Đơn vị tính mới (mã tự sinh)";
                    lcgManufacturer.Text = "Hãng sản xuất mới (mã tự sinh)";
                }
                else
                {
                    colServiceUnitCode.Visible = true;
                    colManufacturerCode.Visible = true;
                    gridViewServiceUnit.OptionsBehavior.Editable = true;
                    gridViewManufacturer.OptionsBehavior.Editable = true;
                    lcgServiceUnit.Text = "Đơn vị tính mới (nhập mã)";
                    lcgManufacturer.Text = "Hãng sản xuất mới (nhập mã)";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                // ĐVT mới — distinct (BẬT theo tên, TẮT theo mã)
                listServiceUnitNew = new List<HIS_SERVICE_UNIT>();
                var seenUnit = new HashSet<string>();
                foreach (var item in currentMaterialTypeImportAdos.Where(o => o.IS_LESS_SERVICE_UNIT))
                {
                    string code = (item.SERVICE_UNIT_CODE ?? "").Trim();
                    string name = (item.SERVICE_UNIT_NAME ?? "").Trim();
                    if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name)) continue;
                    string key = importByName ? NormalizeName(name) : (code.ToUpperInvariant() + "|" + NormalizeName(name));
                    if (seenUnit.Add(key))
                    {
                        listServiceUnitNew.Add(new HIS_SERVICE_UNIT
                        {
                            SERVICE_UNIT_CODE = importByName ? null : code,
                            SERVICE_UNIT_NAME = name
                        });
                    }
                }

                // Hãng SX mới — distinct (BẬT theo tên, TẮT theo mã)
                listManufacturerNew = new List<HIS_MANUFACTURER>();
                var seenManu = new HashSet<string>();
                foreach (var item in currentMaterialTypeImportAdos.Where(o => o.IS_LESS_MANUFACTURER))
                {
                    string code = (item.MANUFACTURER_CODE ?? "").Trim();
                    string name = (item.MANUFACTURER_NAME ?? "").Trim();
                    if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name)) continue;
                    string key = importByName ? NormalizeName(name) : (code.ToUpperInvariant() + "|" + NormalizeName(name));
                    if (seenManu.Add(key))
                    {
                        listManufacturerNew.Add(new HIS_MANUFACTURER
                        {
                            MANUFACTURER_CODE = importByName ? null : code,
                            MANUFACTURER_NAME = name
                        });
                    }
                }

                gridControlServiceUnit.BeginUpdate();
                gridControlServiceUnit.DataSource = listServiceUnitNew;
                gridControlServiceUnit.EndUpdate();

                gridControlManufacturer.BeginUpdate();
                gridControlManufacturer.DataSource = listManufacturerNew;
                gridControlManufacturer.EndUpdate();

                lcgServiceUnit.Visibility = (listServiceUnitNew.Count > 0)
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lcgManufacturer.Visibility = (listManufacturerNew.Count > 0)
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Kiểm tra mã + tên ĐVT (chế độ TẮT): bắt buộc, đúng độ dài, không trùng.</summary>
        private bool CheckValidServiceUnit(List<HIS_SERVICE_UNIT> data)
        {
            try
            {
                if (data.Any(o => string.IsNullOrWhiteSpace(o.SERVICE_UNIT_NAME)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Phải nhập tên đơn vị tính", "Thông báo");
                    return false;
                }
                if (data.Any(o => string.IsNullOrWhiteSpace(o.SERVICE_UNIT_CODE)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Phải nhập mã đơn vị tính", "Thông báo");
                    return false;
                }
                var overLen = data.Where(o => Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(o.SERVICE_UNIT_CODE, 3)).ToList();
                if (overLen.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Mã đơn vị tính: " + string.Join(",", overLen.Select(o => o.SERVICE_UNIT_CODE)) + " vượt quá ký tự cho phép (tối đa 3)", "Thông báo");
                    return false;
                }
                var existCode = data.Where(o => BackendDataWorker.Get<HIS_SERVICE_UNIT>().Any(p => p.SERVICE_UNIT_CODE == o.SERVICE_UNIT_CODE)).ToList();
                if (existCode.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Mã đơn vị tính: " + string.Join(",", existCode.Select(o => o.SERVICE_UNIT_CODE)) + " đã tồn tại", "Thông báo");
                    return false;
                }
                var dupInList = data.GroupBy(o => o.SERVICE_UNIT_CODE).Where(g => g.Count() >= 2).Select(g => g.Key).ToList();
                if (dupInList.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Mã đơn vị tính: " + string.Join(",", dupInList) + " bị trùng", "Thông báo");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return true;
        }

        /// <summary>Kiểm tra mã + tên hãng SX (chế độ TẮT): bắt buộc, đúng độ dài, không trùng.</summary>
        private bool CheckValidManufacturer(List<HIS_MANUFACTURER> data)
        {
            try
            {
                if (data.Any(o => string.IsNullOrWhiteSpace(o.MANUFACTURER_NAME)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Phải nhập tên hãng sản xuất", "Thông báo");
                    return false;
                }
                if (data.Any(o => string.IsNullOrWhiteSpace(o.MANUFACTURER_CODE)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Phải nhập mã hãng sản xuất", "Thông báo");
                    return false;
                }
                var overLen = data.Where(o => Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(o.MANUFACTURER_CODE, 6)).ToList();
                if (overLen.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Mã hãng sản xuất: " + string.Join(",", overLen.Select(o => o.MANUFACTURER_CODE)) + " vượt quá ký tự cho phép (tối đa 6)", "Thông báo");
                    return false;
                }
                var existCode = data.Where(o => BackendDataWorker.Get<HIS_MANUFACTURER>().Any(p => p.MANUFACTURER_CODE == o.MANUFACTURER_CODE)).ToList();
                if (existCode.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Mã hãng sản xuất: " + string.Join(",", existCode.Select(o => o.MANUFACTURER_CODE)) + " đã tồn tại", "Thông báo");
                    return false;
                }
                var dupInList = data.GroupBy(o => o.MANUFACTURER_CODE).Where(g => g.Count() >= 2).Select(g => g.Key).ToList();
                if (dupInList.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Mã hãng sản xuất: " + string.Join(",", dupInList) + " bị trùng", "Thông báo");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return true;
        }

        /// <summary>Chuẩn hóa tên để dedup (BẬT): Trim, gộp khoảng trắng, bỏ hoa/thường.</summary>
        private string NormalizeName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name)) return "";
                var parts = name.Trim().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                return string.Join(" ", parts).ToLowerInvariant();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return (name ?? "").Trim().ToLowerInvariant();
            }
        }
        #endregion
    }
}
