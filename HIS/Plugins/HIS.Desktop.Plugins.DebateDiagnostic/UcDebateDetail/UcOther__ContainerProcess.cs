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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Core;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Plugins.DebateDiagnostic.ADO;

namespace HIS.Desktop.Plugins.DebateDiagnostic.UcDebateDetail
{
    public partial class UcOther : UserControl
    {
        int popupHeight = 400;
        public async Task InitPopupMedicineType()
        {
            try
            {
                Action myaction = () =>
                {
                    var _currentMedicineTypeAlls = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    if (currentMedicineTypeAlls == null || currentMedicineTypeAlls.Count > 0)
                    {
                        this.currentMedicineTypeAlls = _currentMedicineTypeAlls != null ? _currentMedicineTypeAlls.Where(o => o.IS_ACTIVE == 1).Select(o => new MedicineTypeADO(o)).ToList() : null;
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;


                popupHeight = (this.currentMedicineTypeAlls != null && this.currentMedicineTypeAlls.Count > 15) ? 400 : 200;
                gridViewContainerMedicineType.BeginUpdate();
                gridViewContainerMedicineType.Columns.Clear();
                popupControlContainerMedicineType.Width = 450;
                popupControlContainerMedicineType.Height = popupHeight;
                int columnIndex = 1;
                AddFieldColumnIntoComboExt(gridViewContainerMedicineType, "IsChecked", " ", 30, columnIndex++, true, null, true);
                AddFieldColumnIntoComboExt(gridViewContainerMedicineType, "MEDICINE_TYPE_CODE", "Mã", 90, columnIndex++, true);
                AddFieldColumnIntoComboExt(gridViewContainerMedicineType, "MEDICINE_TYPE_NAME", "Tên", 270, columnIndex++, true);

                gridViewContainerMedicineType.GridControl.DataSource = this.currentMedicineTypeAlls;

                gridViewContainerMedicineType.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewContainerMedicineType.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public async Task InitPopupActiveIngredient()
        {
            try
            {
                Action myaction = () =>
                {
                    var _currentActiveIngredientAlls = BackendDataWorker.Get<HIS_ACTIVE_INGREDIENT>();
                    if (currentActiveIngredientAlls == null || currentActiveIngredientAlls.Count > 0)
                    {
                        this.currentActiveIngredientAlls = _currentActiveIngredientAlls != null ? _currentActiveIngredientAlls.Where(o => o.IS_ACTIVE == 1).Select(o => new ActiveIngredientADO(o)).ToList() : null;
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;


                popupHeight = (this.currentActiveIngredientAlls != null && this.currentActiveIngredientAlls.Count > 15) ? 400 : 200;
                gridViewContainerActiveIngredient.BeginUpdate();
                gridViewContainerActiveIngredient.Columns.Clear();
                popupControlContainerActiveIngredient.Width = 450;
                popupControlContainerActiveIngredient.Height = popupHeight;
                int columnIndex = 1;
                AddFieldColumnIntoComboExt(gridViewContainerActiveIngredient, "IsChecked", " ", 30, columnIndex++, true, null, true);
                AddFieldColumnIntoComboExt(gridViewContainerActiveIngredient, "ACTIVE_INGREDIENT_CODE", "Mã", 90, columnIndex++, true);
                AddFieldColumnIntoComboExt(gridViewContainerActiveIngredient, "ACTIVE_INGREDIENT_NAME", "Tên", 270, columnIndex++, true);

                gridViewContainerActiveIngredient.GridControl.DataSource = this.currentActiveIngredientAlls;

                gridViewContainerActiveIngredient.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewContainerActiveIngredient.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddFieldColumnIntoComboExt(Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn gridView, string FieldName, string Caption, int Width, int VisibleIndex, bool FixedWidth, DevExpress.Data.UnboundColumnType? UnboundType = null, bool allowEdit = false)
        {
            DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
            col2.FieldName = FieldName;
            col2.Caption = Caption;
            col2.Width = Width;
            col2.VisibleIndex = VisibleIndex;
            col2.OptionsColumn.FixedWidth = FixedWidth;
            if (UnboundType != null)
                col2.UnboundType = UnboundType.Value;
            col2.OptionsColumn.AllowEdit = allowEdit;
            if (FieldName == "IsChecked")
            {
                col2.ColumnEdit = GenerateRepositoryItemCheckEdit();
                col2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                col2.OptionsFilter.AllowFilter = false;
                col2.OptionsFilter.AllowAutoFilter = false;
                //col2.ImageAlignment = StringAlignment.Center;
                //col2.Image = imageCollection1.Images[0];
                col2.OptionsColumn.AllowEdit = false;
            }

            gridView.Columns.Add(col2);
        }

        DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit GenerateRepositoryItemCheckEdit()
        {
            DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            return repositoryItemCheckEdit1;
        }
    }
}
