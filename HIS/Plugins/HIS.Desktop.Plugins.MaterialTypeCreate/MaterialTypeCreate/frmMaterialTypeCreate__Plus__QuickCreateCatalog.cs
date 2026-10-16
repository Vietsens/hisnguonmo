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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.MaterialTypeCreate.MaterialTypeCreate
{
    public partial class frmMaterialTypeCreate : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Plus button of cboSupplier: opens the Supplier catalog screen.
        /// cboSupplier has no other editor button than Combo, so only Plus is handled here.
        /// </summary>
        private void cboSupplier_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    OpenSupplierModule();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Opens the Service unit catalog screen from the plus button of cboServiceUnit.
        /// Same pattern as the manufacturer plus button: the catalog screen returns
        /// the created record through RefreshDataToServiceUnitCombo.
        /// </summary>
        private void OpenServiceUnitModule()
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add((DelegateSelectData)RefreshDataToServiceUnitCombo);
                if (this.module == null)
                {
                    CallModule callModule = new CallModule(CallModule.HisServiceUnit, 0, 0, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisServiceUnit, this.module.RoomId, this.module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Opens the Supplier catalog screen from the plus button of cboSupplier.
        /// </summary>
        private void OpenSupplierModule()
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add((DelegateSelectData)RefreshDataToSupplierCombo);
                if (this.module == null)
                {
                    CallModule callModule = new CallModule(CallModule.HisSupplier, 0, 0, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisSupplier, this.module.RoomId, this.module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Reloads the service unit list and selects the record created in the catalog screen.
        /// </summary>
        private void RefreshDataToServiceUnitCombo(object serviceUnit)
        {
            try
            {
                HIS_SERVICE_UNIT newServiceUnit = serviceUnit as HIS_SERVICE_UNIT;
                if (newServiceUnit == null) return;

                BackendDataWorker.Reset<HIS_SERVICE_UNIT>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboServiceUnit, BackendDataWorker.Get<HIS_SERVICE_UNIT>(), controlEditorADO);

                cboServiceUnit.EditValue = newServiceUnit.ID;
                txtServiceUnitCode.Text = newServiceUnit.SERVICE_UNIT_CODE;
                cboServiceUnit.Properties.Buttons[1].Visible = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Reloads the supplier list, keeps the suppliers already ticked by the user
        /// and ticks the record created in the catalog screen.
        /// </summary>
        private void RefreshDataToSupplierCombo(object supplier)
        {
            try
            {
                HIS_SUPPLIER newSupplier = supplier as HIS_SUPPLIER;
                if (newSupplier == null) return;

                GridCheckMarksSelection gridCheckMark = cboSupplier.Properties.Tag as GridCheckMarksSelection;

                List<long> selectedIds = new List<long>();
                if (gridCheckMark != null)
                {
                    foreach (object item in gridCheckMark.Selection)
                    {
                        HIS_SUPPLIER selected = item as HIS_SUPPLIER;
                        if (selected != null && !selectedIds.Contains(selected.ID))
                        {
                            selectedIds.Add(selected.ID);
                        }
                    }
                }
                if (!selectedIds.Contains(newSupplier.ID))
                {
                    selectedIds.Add(newSupplier.ID);
                }

                HisSupplierFilter filter = new HisSupplierFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var allSuppliers = new BackendAdapter(new CommonParam())
                    .Get<List<HIS_SUPPLIER>>("api/HisSupplier/Get", ApiConsumers.MosConsumer, filter, null);
                if (allSuppliers == null || allSuppliers.Count == 0) return;

                allSuppliers = allSuppliers
                    .OrderByDescending(o => selectedIds.Contains(o.ID))
                    .ThenBy(o => o.SUPPLIER_CODE)
                    .ToList();
                cboSupplier.Properties.DataSource = allSuppliers;

                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboSupplier.Properties.View);
                    List<HIS_SUPPLIER> selectedItems = allSuppliers.Where(o => selectedIds.Contains(o.ID)).ToList();
                    if (selectedItems.Count > 0)
                    {
                        gridCheckMark.SelectAll(selectedItems);
                        this.Supplier__Selected = selectedItems;
                    }
                    else
                    {
                        this.Supplier__Selected = new List<HIS_SUPPLIER>();
                    }
                }

                cboSupplier.Refresh();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
