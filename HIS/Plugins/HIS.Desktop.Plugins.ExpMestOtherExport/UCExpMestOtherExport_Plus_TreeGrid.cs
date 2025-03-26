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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Plugins.ExpMestOtherExport.ADO;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMaterialGrid.ADO;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestOtherExport
{
    public partial class UCExpMestOtherExport : HIS.Desktop.Utility.UserControlBase
    {

        private void InitExpMestMediGrid()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Resources.ResourceLanguageManager.LanguageUCExpMestOtherExport;

                this.expMestMediProcessor = new ExpMestMedicineProcessor();
                ExpMestMedicineInitADO ado = new ExpMestMedicineInitADO();
                ado.ExpMestMedicineGrid_CustomUnboundColumnData = expMestMediGrid__CustomUnboundColumnData;
                ado.IsShowSearchPanel = false;
                ado.ListExpMestMedicineColumn = new List<ExpMestMedicineColumn>();

                ExpMestMedicineColumn colMedicineTypeName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MEDICINE__COLUMN_MEDICINE_TYPE_NAME", langManager, culture), "MEDI_MATE_TYPE_NAME_CONCENTRA", 170, false);
                colMedicineTypeName.VisibleIndex = 0;
                ado.ListExpMestMedicineColumn.Add(colMedicineTypeName);

                ExpMestMedicineColumn colServiceUnitName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MEDICINE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "SERVICE_UNIT_NAME", 40, false);
                colServiceUnitName.VisibleIndex = 1;
                ado.ListExpMestMedicineColumn.Add(colServiceUnitName);

                ExpMestMedicineColumn colAmount = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MEDICINE__COLUMN_AMOUNT", langManager, culture), "AMOUNT", 100, false);
                colAmount.VisibleIndex = 2;
                colAmount.Format = new DevExpress.Utils.FormatInfo();
                colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAmount.Format.FormatString = "#,##0.00";
                ado.ListExpMestMedicineColumn.Add(colAmount);

                ExpMestMedicineColumn colExpiredDate = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MEDICINE__COLUMN_EXPIRED_DATE", langManager, culture), "EXPIRED_DATE_STR", 90, false);
                colExpiredDate.VisibleIndex = 3;
                colExpiredDate.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListExpMestMedicineColumn.Add(colExpiredDate);

                ExpMestMedicineColumn colPackageNumber = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MEDICINE__COLUMN_PACKAGE_NUMBER", langManager, culture), "PACKAGE_NUMBER", 60, false);
                colPackageNumber.VisibleIndex = 4;
                ado.ListExpMestMedicineColumn.Add(colPackageNumber);

                ExpMestMedicineColumn colNationalName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MEDICINE__COLUMN_NATIONAL_NAME", langManager, culture), "NATIONAL_NAME", 100, false);
                colNationalName.VisibleIndex = 5;
                ado.ListExpMestMedicineColumn.Add(colNationalName);

                ExpMestMedicineColumn colManufacturerName = new ExpMestMedicineColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MEDICINE__COLUMN_MANUFACTURER_NAME", langManager, culture), "MANUFACTURER_NAME", 100, false);
                colManufacturerName.VisibleIndex = 6;
                ado.ListExpMestMedicineColumn.Add(colManufacturerName);

                this.ucExpMestMedi = (UserControl)this.expMestMediProcessor.Run(ado);
                if (this.ucExpMestMedi != null)
                {
                    this.xtraTabPageExpMestMedi.Controls.Add(this.ucExpMestMedi);
                    this.ucExpMestMedi.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitExpMestMateGrid()
        {
            try
            {
                //var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //var langManager = Resources.ResourceLanguageManager.LanguageUCExpMestOtherExport;

                //this.expMestMateProcessor = new ExpMestMaterialProcessor();
                //ExpMestMaterialInitADO ado = new ExpMestMaterialInitADO();
                //ado.ExpMestMaterialGrid_CustomUnboundColumnData = expMestMateGrid__CustomUnboundColumnData;
                //ado.IsShowSearchPanel = false;
                //ado.ListExpMestMaterialColumn = new List<ExpMestMaterialColumn>();

                //ExpMestMaterialColumn colMaterialTypeName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MATERIAL__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MATERIAL_TYPE_NAME", 170, false);
                //colMaterialTypeName.VisibleIndex = 0;
                //ado.ListExpMestMaterialColumn.Add(colMaterialTypeName);

                //ExpMestMaterialColumn colServiceUnitName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MATERIAL__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "SERVICE_UNIT_NAME", 40, false);
                //colServiceUnitName.VisibleIndex = 1;
                //ado.ListExpMestMaterialColumn.Add(colServiceUnitName);

                //ExpMestMaterialColumn colAmount = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MATERIAL__COLUMN_AMOUNT", langManager, culture), "AMOUNT", 100, false);
                //colAmount.VisibleIndex = 2;
                //colAmount.Format = new DevExpress.Utils.FormatInfo();
                //colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                //colAmount.Format.FormatString = "#,##0.00";
                //ado.ListExpMestMaterialColumn.Add(colAmount);

                //ExpMestMaterialColumn colExpiredDate = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MATERIAL__COLUMN_EXPIRED_DATE", langManager, culture), "EXPIRED_DATE_STR", 90, false);
                //colExpiredDate.VisibleIndex = 3;
                //colExpiredDate.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListExpMestMaterialColumn.Add(colExpiredDate);

                //ExpMestMaterialColumn colPackageNumber = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MATERIAL__COLUMN_PACKAGE_NUMBER", langManager, culture), "PACKAGE_NUMBER", 60, false);
                //colPackageNumber.VisibleIndex = 4;
                //ado.ListExpMestMaterialColumn.Add(colPackageNumber);

                //ExpMestMaterialColumn colNationalName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MATERIAL__COLUMN_NATIONAL_NAME", langManager, culture), "NATIONAL_NAME", 100, false);
                //colNationalName.VisibleIndex = 5;
                //ado.ListExpMestMaterialColumn.Add(colNationalName);

                //ExpMestMaterialColumn colManufacturerName = new ExpMestMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_EXP_MEST_MATERIAL__COLUMN_MANUFACTURER_NAME", langManager, culture), "MANUFACTURER_NAME", 100, false);
                //colManufacturerName.VisibleIndex = 6;
                //ado.ListExpMestMaterialColumn.Add(colManufacturerName);

                //this.ucExpMestMate = (UserControl)this.expMestMateProcessor.Run(ado);
                //if (this.ucExpMestMate != null)
                //{
                //    this.xtraTabPageExpMestMate.Controls.Add(this.ucExpMestMate);
                //    this.ucExpMestMate.Dock = DockStyle.Fill;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitMaterialTree()
        {
            try
            {
                //var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //var langManager = Resources.ResourceLanguageManager.LanguageUCExpMestOtherExport;

                //this.mateInStockProcessor = new UC.HisMaterialInStock.MaterialInStockProcessor();
                //MaterialTypeInStockInitADO ado = new MaterialTypeInStockInitADO();
                //ado.IsShowButtonAdd = false;
                //ado.IsShowCheckNode = false;
                //ado.IsShowSearchPanel = true;
                //ado.IsAutoWidth = true;
                //ado.MaterialTypeInStockClick = materialInStockTree_Click;
                //ado.MaterialTypeInStockRowEnter = materialInStockTree_EnterRow;
                //ado.MaterialTypeInStockColumns = new List<MaterialTypeInStockColumn>();
                //ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__TXT_KEYWORD__NULL_VALUE", langManager, culture);

                //MaterialTypeInStockColumn colMaterialTypeCode = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_CODE", langManager, culture), "MaterialTypeCode", 70, false);
                //colMaterialTypeCode.VisibleIndex = 0;
                //ado.MaterialTypeInStockColumns.Add(colMaterialTypeCode);

                //MaterialTypeInStockColumn colMaterialTypeName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MATERIAL_TREE__COLUMN_MATERIAL_TYPE_NAME", langManager, culture), "MaterialTypeName", 300, false);
                //colMaterialTypeName.VisibleIndex = 1;
                //ado.MaterialTypeInStockColumns.Add(colMaterialTypeName);

                //MaterialTypeInStockColumn colServiceUnitName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MATERIAL_TREE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "ServiceUnitName", 60, false);
                //colServiceUnitName.VisibleIndex = 2;
                //ado.MaterialTypeInStockColumns.Add(colServiceUnitName);

                //MaterialTypeInStockColumn colAvailableAmount = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MATERIAL_TREE__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 110, false);
                //colAvailableAmount.VisibleIndex = 3;
                //colAvailableAmount.Format = new DevExpress.Utils.FormatInfo();
                //colAvailableAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                //colAvailableAmount.Format.FormatString = "#,##0.00";
                //ado.MaterialTypeInStockColumns.Add(colAvailableAmount);

                //MaterialTypeInStockColumn colNationalName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MATERIAL_TREE__COLUMN_NATIONAL_NAME", langManager, culture), "NationalName", 120, false);
                //colNationalName.VisibleIndex = 4;
                //ado.MaterialTypeInStockColumns.Add(colNationalName);

                //MaterialTypeInStockColumn colManufacturerName = new MaterialTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MATERIAL_TREE__COLUMN_MANUFACTURER_NAME", langManager, culture), "ManufacturerName", 150, false);
                //colManufacturerName.VisibleIndex = 5;
                //ado.MaterialTypeInStockColumns.Add(colManufacturerName);

                //this.ucMateInStock = (UserControl)this.mateInStockProcessor.Run(ado);
                //if (this.ucMateInStock != null)
                //{
                //    this.xtraTabPageMaterial.Controls.Add(this.ucMateInStock);
                //    this.ucMateInStock.Dock = DockStyle.Fill;
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitMedicineTree()
        {
            try
            {
                //var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //var langManager = Resources.ResourceLanguageManager.LanguageUCExpMestOtherExport;
                //this.mediInStockProcessor = new MedicineInStockProcessor();
                //HisMedicineInStockInitADO ado = new HisMedicineInStockInitADO();
                //ado.IsShowButtonAdd = false;
                //ado.IsShowCheckNode = false;
                //ado.IsShowSearchPanel = true;
                //ado.IsAutoWidth = true;
                //ado.gridView_Click = medicineInStockTree_CLick;
                //ado.MedicineTypeInStockRowEnter = medicineInStockTree_RowEnter;
                //ado.MedicineInStockColumns = new List<MedicineInStockColumn>();

                //MedicineInStockColumn colMedicineTypeCode = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MEDICINE_TREE__COLUMN_MEDICINE_TYPE_CODE", langManager, culture), "MedicineTypeCode", 70, false);
                //colMedicineTypeCode.VisibleIndex = 0;
                //ado.MedicineInStockColumns.Add(colMedicineTypeCode);

                //MedicineInStockColumn colMedicineTypeName = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MEDICINE_TREE__COLUMN_MEDICINE_TYPE_NAME", langManager, culture), "MedicineTypeName", 250, false);
                //colMedicineTypeName.VisibleIndex = 1;
                //ado.MedicineInStockColumns.Add(colMedicineTypeName);

                //MedicineInStockColumn colServiceUnitName = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MEDICINE_TREE__COLUMN_SERVICE_UNIT_NAME", langManager, culture), "ServiceUnitName", 50, false);
                //colServiceUnitName.VisibleIndex = 2;
                //ado.MedicineInStockColumns.Add(colServiceUnitName);

                //MedicineInStockColumn colAvailableAmount = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MEDICINE_TREE__COLUMN_AVAILABLE_AMOUNT", langManager, culture), "AvailableAmount", 100, false);
                //colAvailableAmount.VisibleIndex = 3;
                //colAvailableAmount.Format = new DevExpress.Utils.FormatInfo();
                //colAvailableAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                //colAvailableAmount.Format.FormatString = "#,##0.00";
                //ado.MedicineInStockColumns.Add(colAvailableAmount);

                //MedicineInStockColumn colNationalName = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MEDICINE_TREE__COLUMN_NATIONAL_NAME", langManager, culture), "NationalName", 100, false);
                //colNationalName.VisibleIndex = 4;
                //ado.MedicineInStockColumns.Add(colNationalName);

                //MedicineInStockColumn colManufacturerName = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MEDICINE_TREE__COLUMN_MANUFACTURER_NAME", langManager, culture), "ManufacturerName", 120, false);
                //colManufacturerName.VisibleIndex = 5;
                //ado.MedicineInStockColumns.Add(colManufacturerName);

                //MedicineInStockColumn colActiveIngrBhytName = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MEDICINE_TREE__COLUMN_ACTIVE_INGR_BHYT_NAME", langManager, culture), "ActiveIngrBhytName", 120, false);
                //colActiveIngrBhytName.VisibleIndex = 6;
                //ado.MedicineInStockColumns.Add(colActiveIngrBhytName);

                //MedicineInStockColumn colRegisterNumber = new MedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__MEDICINE_TREE__COLUMN_REGISTER_NUMBER", langManager, culture), "RegisterNumber", 70, false);
                //colRegisterNumber.VisibleIndex = 7;
                //ado.MedicineInStockColumns.Add(colRegisterNumber);

                //this.ucMediInStock = (UserControl)this.mediInStockProcessor.Run(ado);
                //if (this.ucMediInStock != null)
                //{
                //    this.xtraTabPageMedicine.Controls.Add(this.ucMediInStock);
                //    this.ucMediInStock.Dock = DockStyle.Fill;
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void expMestMediGrid__CustomUnboundColumnData(V_HIS_EXP_MEST_MEDICINE data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void expMestMateGrid__CustomUnboundColumnData(V_HIS_EXP_MEST_MATERIAL data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void medicineInStockTree_RowEnter(MedicineTypeInStockADO data)
        //{
        //    try
        //    {
        //        WaitingManager.Show();
        //        this.currentMediMate = null;
        //        if (data != null)
        //        {
        //            this.currentMediMate = new ADO.MediMateTypeADO(data);
        //        }
        //        //SetEnableControlPriceByCheckBox();
        //        SetValueByMediMateADO();
        //        WaitingManager.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void medicineInStockTree_CLick(MedicineTypeInStockADO data)
        //{
        //    try
        //    {
        //        WaitingManager.Show();
        //        this.currentMediMate = null;
        //        if (data != null)
        //        {
        //            this.currentMediMate = new ADO.MediMateTypeADO(data);
        //        }
        //        //SetEnableControlPriceByCheckBox();
        //        SetValueByMediMateADO();
        //        WaitingManager.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void materialInStockTree_EnterRow(MaterialTypeInStockADO data)
        //{
        //    try
        //    {
        //        WaitingManager.Show();
        //        this.currentMediMate = null;
        //        if (data != null)
        //        {
        //            this.currentMediMate = new ADO.MediMateTypeADO(data);
        //        }
        //        //SetEnableControlPriceByCheckBox();
        //        SetValueByMediMateADO();
        //        WaitingManager.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void materialInStockTree_Click(MaterialTypeInStockADO data)
        //{
        //    try
        //    {
        //        WaitingManager.Show();
        //        this.currentMediMate = null;
        //        if (data != null)
        //        {
        //            this.currentMediMate = new ADO.MediMateTypeADO(data);
        //        }
        //        //SetEnableControlPriceByCheckBox();
        //        SetValueByMediMateADO();
        //        WaitingManager.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}


        private void gridViewExpMestDetail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column.FieldName != "EXP_AMOUNT")
                    return;
                var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (data.IsMedicine)
                    {
                        data.ExpMedicine.Amount = data.EXP_AMOUNT;
                    }
                    else
                    {
                        data.ExpMaterial.Amount = data.EXP_AMOUNT;
                    }
                    if (data.EXP_AMOUNT > data.AVAILABLE_AMOUNT)
                    {
                        data.IsGreatAvailable = true;
                    }
                    else
                    {
                        data.IsGreatAvailable = false;
                    }
                }
                gridControlExpMestDetail.RefreshDataSource();
                SetDataToLabels_MoneyCalculated();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                if (gridViewExpMestDetail.FocusedRowHandle < 0 || gridViewExpMestDetail.FocusedColumn.FieldName != "EXP_AMOUNT")
                    return;
                var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[gridViewExpMestDetail.FocusedRowHandle];
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.EXP_AMOUNT <= 0)
                    {
                        valid = false;
                        message = LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.SoLuongKhongDuocBeHonKhong);
                    }
                    else if (data.EXP_AMOUNT > data.AVAILABLE_AMOUNT)
                    {
                        valid = false;
                        message = "Số lượng xuất lớn hơn số lượng trong kho";
                    }
                    if (!valid)
                    {
                        gridViewExpMestDetail.SetColumnError(gridViewExpMestDetail.FocusedColumn, message);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsNotHasMest)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (data.IsGreatAvailable)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueByMediMateADO()
        {
            try
            {
                spinAmount.Value = 0;
                txtNote.Text = "";
                if (this.currentMediMate != null)
                {
                    btnAdd.Enabled = true;
                    //spinAmount.Properties.MaxValue = this.currentMediMate.AVAILABLE_AMOUNT ?? 0;
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                    spinExpPrice.EditValue = this.currentMediMate.IMP_PRICE;
                    spinPriceVAT.EditValue = (this.currentMediMate.IMP_VAT_RATIO * 100);
                }
                else
                {
                    btnAdd.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
