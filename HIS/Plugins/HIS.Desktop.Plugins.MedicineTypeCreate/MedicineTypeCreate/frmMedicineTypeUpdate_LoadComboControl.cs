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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDA.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Utilities.Extensions;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.MedicineTypeCreate
{
    public partial class frmMedicineTypeCreate : HIS.Desktop.Utility.FormBase
    {
        private void LoadProvinceCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMedicineTypeParent.Properties.DataSource = null;
                    cboMedicineTypeParent.EditValue = null;
                    txtMedicineTypeParentCode.Text = "";
                    if (nationalProcessor != null)
                        nationalProcessor.SetValue(ucNational, null);
                    //cboNational.Properties.DataSource = null;
                    //cboNational.EditValue = null;
                    //txtNationalCode.Text = "";
                    //cboNational.EditValue = null;
                    //cboNational.Focus();
                    //cboNational.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboNational);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceUnit(string _serviceUnitCode)
        {
            try
            {
                List<HIS_SERVICE_UNIT> listResult = new List<HIS_SERVICE_UNIT>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>().Where(o => (o.SERVICE_UNIT_CODE != null && o.SERVICE_UNIT_CODE.StartsWith(_serviceUnitCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboServiceUnit, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboServiceUnit.EditValue = listResult[0].ID;
                    txtServiceUnitCode.Text = listResult[0].SERVICE_UNIT_CODE;
                    txtMedicineTypeParentCode.Focus();
                    txtMedicineTypeParentCode.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboServiceUnit.EditValue = null;
                    cboServiceUnit.Focus();
                    cboServiceUnit.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboServiceUnit);
                }
                else
                {
                    cboServiceUnit.EditValue = null;
                    cboServiceUnit.Focus();
                    cboServiceUnit.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMedicineTypeParent(string _medicineTypeParentCode)
        {
            try
            {
                List<HIS_MEDICINE_TYPE> listResult = new List<HIS_MEDICINE_TYPE>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE>().Where(o => (o.MEDICINE_TYPE_CODE != null && o.MEDICINE_TYPE_CODE.StartsWith(_medicineTypeParentCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboMedicineTypeParent, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboMedicineTypeParent.EditValue = listResult[0].ID;
                    txtMedicineTypeParentCode.Text = listResult[0].MEDICINE_TYPE_CODE;
                    cboHeinServiceType.Focus();
                    cboHeinServiceType.SelectAll();
                    
                }
                else if (listResult.Count > 1)
                {
                    cboMedicineTypeParent.EditValue = null;
                    cboMedicineTypeParent.Focus();
                    cboMedicineTypeParent.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboServiceUnit);
                }
                else
                {
                    cboMedicineTypeParent.EditValue = null;
                    cboMedicineTypeParent.Focus();
                    cboMedicineTypeParent.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMedicineLine(string _medicineLineCode)
        {
            try
            {
                List<HIS_MEDICINE_LINE> listResult = new List<HIS_MEDICINE_LINE>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_LINE>().Where(o => (o.MEDICINE_LINE_CODE != null && o.MEDICINE_LINE_CODE.StartsWith(_medicineLineCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_LINE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_LINE_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_LINE_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboMedicineLine, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboMedicineLine.EditValue = listResult[0].ID;
                   
                }
                else if (listResult.Count > 1)
                {
                    cboMedicineLine.EditValue = null;
                    cboMedicineLine.Focus();
                    cboMedicineLine.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboServiceUnit);
                }
                else
                {
                    cboMedicineLine.EditValue = null;
                    cboMedicineLine.Focus();
                    cboMedicineLine.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadHeinServiceType(string _medicineLineCode)
        {
            try
            {
                List<HIS_HEIN_SERVICE_TYPE> listResult = new List<HIS_HEIN_SERVICE_TYPE>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_TYPE>().Where(o => (o.HEIN_SERVICE_TYPE_CODE != null && o.HEIN_SERVICE_TYPE_CODE.StartsWith(_medicineLineCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_LINE_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboMedicineLine, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboHeinServiceType.EditValue = listResult[0].ID;
                   
                    txtServiceUnitCode.Focus();
                    txtServiceUnitCode.SelectAll();
                    //if (nationalProcessor != null && ucNational != null)
                    //{
                    //    nationalProcessor.FocusControl(ucNational);
                    //}
                }
                else if (listResult.Count > 1)
                {
                    cboHeinServiceType.EditValue = null;
                    cboHeinServiceType.Focus();
                    cboHeinServiceType.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboServiceUnit);
                }
                else
                {
                    cboHeinServiceType.EditValue = null;
                    cboHeinServiceType.Focus();
                    cboHeinServiceType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMedicineUseForm(string _medicineUseFormCode)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM> listResult = new List<HIS_MEDICINE_USE_FORM>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().Where(o => (o.MEDICINE_USE_FORM_CODE != null && o.MEDICINE_USE_FORM_CODE.StartsWith(_medicineUseFormCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboMedicineUseForm, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboMedicineUseForm.EditValue = listResult[0].ID;
                    txtMedicineUseFormCode.Text = listResult[0].MEDICINE_USE_FORM_CODE;
                    txtConcentra.Focus();
                    txtConcentra.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboMedicineUseForm.EditValue = null;
                    cboMedicineUseForm.Focus();
                    cboMedicineUseForm.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboServiceUnit);
                }
                else
                {
                    cboMedicineUseForm.EditValue = null;
                    cboMedicineUseForm.Focus();
                    cboMedicineUseForm.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadManufacturer(string _manufacturerCode)
        {
            try
            {
                List<HIS_MANUFACTURER> listResult = new List<HIS_MANUFACTURER>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MANUFACTURER>().Where(o => (o.MANUFACTURER_CODE != null && o.MANUFACTURER_CODE.StartsWith(_manufacturerCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MANUFACTURER_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MANUFACTURER_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MANUFACTURER_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboManufacture, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboManufacture.EditValue = listResult[0].ID;
                    txtManufactureCode.Text = listResult[0].MANUFACTURER_CODE;
                    txtPackingTypeCode.Focus();
                    txtPackingTypeCode.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboManufacture.EditValue = null;
                    cboManufacture.Focus();
                    cboManufacture.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboServiceUnit);
                }
                else
                {
                    cboManufacture.EditValue = null;
                    cboManufacture.Focus();
                    cboManufacture.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMedicineGroup(string _medicineGroupCode)
        {
            try
            {
                List<HIS_MEDICINE_GROUP> listResult = new List<HIS_MEDICINE_GROUP>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_GROUP>().Where(o => (o.MEDICINE_GROUP_CODE != null && o.MEDICINE_GROUP_CODE.StartsWith(_medicineGroupCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_GROUP_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MANUFACTURER_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboManufacture, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboMedicineGroup.EditValue = listResult[0].ID;
                  
                }
                else if (listResult.Count > 1)
                {
                    cboMedicineGroup.EditValue = null;
                    cboMedicineGroup.Focus();
                    cboMedicineGroup.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboServiceUnit);
                }
                else
                {
                    cboMedicineGroup.EditValue = null;
                    cboMedicineGroup.Focus();
                    cboMedicineGroup.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 350;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 350;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                cbo.Properties.View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.True;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
