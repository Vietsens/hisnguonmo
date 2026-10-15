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
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MedicineTypeCreate.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.MedicineTypeCreate
{
    public partial class frmMedicineTypeCreate : HIS.Desktop.Utility.FormBase
    {
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
        /// Opens the Supplier catalog screen from the plus button of cboNCC.
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
                InitServiceUnit();

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

                List<long> selectedIds = new List<long>();
                if (this.lstSupplier != null)
                {
                    selectedIds.AddRange(this.lstSupplier.Select(o => o.ID));
                }
                if (!selectedIds.Contains(newSupplier.ID))
                {
                    selectedIds.Add(newSupplier.ID);
                }

                BackendDataWorker.Reset<HIS_SUPPLIER>();

                List<SupplierADO> listADO = new List<SupplierADO>();
                foreach (var item in BackendDataWorker.Get<HIS_SUPPLIER>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                {
                    SupplierADO ado = new SupplierADO();
                    ado.ID = item.ID;
                    ado.SUPPLIER_CODE = item.SUPPLIER_CODE;
                    ado.SUPPLIER_NAME = item.SUPPLIER_NAME;
                    ado.SUPPLIER_NAME_UNSIGN = convertToUnSign3(item.SUPPLIER_NAME);
                    ado.isChecked = false;
                    listADO.Add(ado);
                }

                this.lstSupplierChecked = listADO.Select(x => new SupplierADO
                {
                    ID = x.ID,
                    SUPPLIER_CODE = x.SUPPLIER_CODE,
                    SUPPLIER_NAME = x.SUPPLIER_NAME,
                    SUPPLIER_NAME_UNSIGN = x.SUPPLIER_NAME_UNSIGN,
                    isChecked = false
                }).ToList();

                cboNCC.Properties.DataSource = null;
                cboNCC.Properties.DataSource = listADO;
                cboNCC.Properties.View.RefreshData();

                GridCheckMarksSelection gridCheckNCC = cboNCC.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckNCC != null)
                {
                    gridCheckNCC.ClearSelection(cboNCC.Properties.View);
                    if (selectedIds.Count > 0)
                    {
                        ProcessSelectNCC(string.Join(",", selectedIds), gridCheckNCC);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
