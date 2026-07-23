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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
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

        /// <summary>Danh sách ĐVT mới sẽ tạo (mã để trống, backend tự sinh)</summary>
        List<HIS_SERVICE_UNIT> listServiceUnitNew;
        /// <summary>Danh sách hãng SX mới sẽ tạo (mã để trống, backend tự sinh)</summary>
        List<HIS_MANUFACTURER> listManufacturerNew;
        #endregion

        #region Constructor
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
        #endregion

        #region Event form
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

        private void gridViewServiceUnit_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
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
                {
                    e.Value = e.ListSourceRowIndex + 1;
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
                bool hasServiceUnit = listServiceUnitNew != null && listServiceUnitNew.Count > 0;
                bool hasManufacturer = listManufacturerNew != null && listManufacturerNew.Count > 0;

                if (!hasServiceUnit && !hasManufacturer)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có dữ liệu cần bổ sung", "Thông báo");
                    return;
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
                    {
                        BackendDataWorker.Reset<HIS_SERVICE_UNIT>();
                    }
                    else
                    {
                        success = false;
                    }
                }

                // Tạo hàng loạt Hãng sản xuất mới (mã để trống -> backend tự sinh)
                if (success && hasManufacturer)
                {
                    var rsManu = new BackendAdapter(param).Post<List<HIS_MANUFACTURER>>(
                        HisRequestUriStore.MOSHIS_HIS_MANUFACTURER_CREATE_LIST,
                        ApiConsumers.MosConsumer, listManufacturerNew, param);
                    if (rsManu != null && rsManu.Count > 0)
                    {
                        BackendDataWorker.Reset<HIS_MANUFACTURER>();
                    }
                    else
                    {
                        success = false;
                    }
                }

                WaitingManager.Hide();

                if (success && this.currentDelegate != null)
                {
                    this.currentDelegate();
                }

                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);

                if (success)
                {
                    this.Close();
                }
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
        private void FillDataToGrid()
        {
            try
            {
                // ĐVT mới — distinct theo tên chuẩn hóa
                listServiceUnitNew = new List<HIS_SERVICE_UNIT>();
                var seenUnit = new HashSet<string>();
                foreach (var item in currentMaterialTypeImportAdos.Where(o => o.IS_LESS_SERVICE_UNIT && !string.IsNullOrWhiteSpace(o.SERVICE_UNIT_NAME)))
                {
                    string key = NormalizeName(item.SERVICE_UNIT_NAME);
                    if (seenUnit.Add(key))
                    {
                        listServiceUnitNew.Add(new HIS_SERVICE_UNIT { SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME.Trim() });
                    }
                }

                // Hãng SX mới — distinct theo tên chuẩn hóa
                listManufacturerNew = new List<HIS_MANUFACTURER>();
                var seenManu = new HashSet<string>();
                foreach (var item in currentMaterialTypeImportAdos.Where(o => o.IS_LESS_MANUFACTURER && !string.IsNullOrWhiteSpace(o.MANUFACTURER_NAME)))
                {
                    string key = NormalizeName(item.MANUFACTURER_NAME);
                    if (seenManu.Add(key))
                    {
                        listManufacturerNew.Add(new HIS_MANUFACTURER { MANUFACTURER_NAME = item.MANUFACTURER_NAME.Trim() });
                    }
                }

                gridControlServiceUnit.BeginUpdate();
                gridControlServiceUnit.DataSource = listServiceUnitNew;
                gridControlServiceUnit.EndUpdate();

                gridControlManufacturer.BeginUpdate();
                gridControlManufacturer.DataSource = listManufacturerNew;
                gridControlManufacturer.EndUpdate();

                // Ẩn nhóm không có dữ liệu để giao diện gọn
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

        /// <summary>
        /// Chuẩn hóa tên để dedup: Trim, gộp khoảng trắng thừa, không phân biệt hoa/thường.
        /// </summary>
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
