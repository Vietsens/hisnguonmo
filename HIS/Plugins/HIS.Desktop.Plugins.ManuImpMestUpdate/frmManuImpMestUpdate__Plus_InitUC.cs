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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ManuImpMestUpdate.ADO;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Config;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Resources;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Validation;
using HIS.Desktop.Utility;
using HIS.UC.MaterialType;
using HIS.UC.MedicineType;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ManuImpMestUpdate
{
    public partial class frmManuImpMestUpdate : HIS.Desktop.Utility.FormBase
    {
        private void InitMedicineTypeTree()
        {
            try
            {
                listBids = new List<V_HIS_BID>();
                GetBids(ref listBids);
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.medicineProcessor = new MedicineTypeProcessor();
                MedicineTypeInitADO ado = new MedicineTypeInitADO();
                ado.IsShowSearchPanel = true;
                ado.MedicineTypeClick = medicineTypeTree_Click;
                ado.MedicineTypeRowEnter = medicineTypeTree_Click;
                ado.MedicineTypeColumns = new List<MedicineTypeColumn>();
                ado.IsAutoWidth = true;
                ado.IsShowBid = true;
                //ado.listBids = listBids;
                ado.cboBid_EditValueChanged = cboBid_EditValueChangedUCMedicine;
                ado.Keyword_NullValuePrompt = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TXT_KEYWORD__NULL_VALUE");

                //MedicineTypeCode
                MedicineTypeColumn colMedicineTypeCode = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_MEDICINE_TYPE_CODE"),
                    "MEDICINE_TYPE_CODE", 70, false);
                colMedicineTypeCode.VisibleIndex = 0;
                ado.MedicineTypeColumns.Add(colMedicineTypeCode);

                //MedicineTypeName
                MedicineTypeColumn colMedicineTypeName = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_MEDICINE_TYPE_NAME"),
                    "MEDICINE_TYPE_NAME", 250, false);
                colMedicineTypeName.VisibleIndex = 1;
                ado.MedicineTypeColumns.Add(colMedicineTypeName);

                //ServiceUnitName
                MedicineTypeColumn colServiceUnitName = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_SERVICE_UNIT_NAME"),
                    "SERVICE_UNIT_NAME", 50, false);
                colServiceUnitName.VisibleIndex = 2;
                ado.MedicineTypeColumns.Add(colServiceUnitName);

                //NationalName
                MedicineTypeColumn colNationalName = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_NATIONAL_NAME"),
                    "NATIONAL_NAME", 100, false);
                colServiceUnitName.VisibleIndex = 3;
                ado.MedicineTypeColumns.Add(colNationalName);

                //ManufactureName
                MedicineTypeColumn colManufactureName = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_MANUFACTURE_NAME"),
                    "MANUFACTURER_NAME", 120, false);
                colManufactureName.VisibleIndex = 4;
                ado.MedicineTypeColumns.Add(colManufactureName);

                //RegisterNumber
                MedicineTypeColumn colRegisterNumber = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_REGISTER_NUMBER"),
                    "REGISTER_NUMBER", 70, false);
                colRegisterNumber.VisibleIndex = 5;
                ado.MedicineTypeColumns.Add(colRegisterNumber);

                //ActiveAngrBhytName
                MedicineTypeColumn colActiveAngrBhytName = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_ACTIVE_INGR_BHYT_NAME"),
                    "ACTIVE_INGR_BHYT_NAME", 120, false);
                colActiveAngrBhytName.VisibleIndex = 6;
                ado.MedicineTypeColumns.Add(colActiveAngrBhytName);

                this.ucMedicineTypeTree = (UserControl)medicineProcessor.Run(ado);
                if (this.ucMedicineTypeTree != null)
                {
                    this.xtraTabPageMedicine.Controls.Add(this.ucMedicineTypeTree);
                    this.ucMedicineTypeTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetBids(ref List<V_HIS_BID> bids)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBidViewFilter filter = new HisBidViewFilter();
                filter.IS_ACTIVE = 1;

                bids = new BackendAdapter(param).Get<List<V_HIS_BID>>("api/HisBid/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitMaterialTypeTree()
        {
            try
            {
                listBids = new List<V_HIS_BID>();
                GetBids(ref listBids);
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                this.materialProcessor = new MaterialTypeTreeProcessor();
                MaterialTypeInitADO ado = new MaterialTypeInitADO();
                ado.IsShowSearchPanel = true;
                ado.IsShowBid = true;
                //ado.listBids = listBids;
                ado.cboBid_EditValueChanged = cboBid_EditValueChangedUCMaterial;
                ado.MaterialTypeClick = materialTypeTree_Click;
                ado.MaterialTypeRowEnter = materialTypeTree_Click;
                ado.MaterialTypeColumns = new List<MaterialTypeColumn>();
                ado.IsAutoWidth = true;
                ado.Keyword_NullValuePrompt = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TXT_KEYWORD__NULL_VALUE");

                //MaterialTypeCode
                MaterialTypeColumn colMaterialTypeCode = new MaterialTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MATERIAL__COLUMN_MATERIAL_TYPE_CODE"),
                    "MATERIAL_TYPE_CODE", 70, false);
                colMaterialTypeCode.VisibleIndex = 0;
                ado.MaterialTypeColumns.Add(colMaterialTypeCode);

                //MedicineTypeName
                MaterialTypeColumn colMaterialTypeName = new MaterialTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MATERIAL__COLUMN_MATERIAL_TYPE_NAME"),
                    "MATERIAL_TYPE_NAME", 250, false);
                colMaterialTypeName.VisibleIndex = 1;
                ado.MaterialTypeColumns.Add(colMaterialTypeName);

                //ServiceUnitName
                MaterialTypeColumn colServiceUnitName = new MaterialTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MATERIAL__COLUMN_SERVICE_UNIT_NAME"),
                    "SERVICE_UNIT_NAME", 50, false);
                colServiceUnitName.VisibleIndex = 2;
                ado.MaterialTypeColumns.Add(colServiceUnitName);

                //NationalName
                MaterialTypeColumn colNationalName = new MaterialTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MATERIAL__COLUMN_NATIONAL_NAME"),
                    "NATIONAL_NAME", 120, false);
                colServiceUnitName.VisibleIndex = 3;
                ado.MaterialTypeColumns.Add(colNationalName);

                //ManufactureName
                MaterialTypeColumn colManufactureName = new MaterialTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TREE_MATERIAL__COLUMN_MANUFACTURE_NAME"),
                    "MANUFACTURER_NAME", 150, false);
                colManufactureName.VisibleIndex = 4;
                ado.MaterialTypeColumns.Add(colManufactureName);

                this.ucMaterialTypeTree = (UserControl)materialProcessor.Run(ado);
                if (this.ucMaterialTypeTree != null)
                {
                    this.xtraTabPageMaterial.Controls.Add(this.ucMaterialTypeTree);
                    this.ucMaterialTypeTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineTypeTree_Click(UC.MedicineType.ADO.MedicineTypeADO data)
        {
            try
            {
                WaitingManager.Show();
                if (btnCancel.Enabled == true)
                {
                    SetEnableButton(false);
                    ResetValueControlDetail();
                    SetFocuTreeMediMate();
                    SetEnableButton(false);
                }
                this.currrentServiceAdo = null;
                if (data != null)
                {
                    this.currrentServiceAdo = new ADO.VHisServiceADO((V_HIS_MEDICINE_TYPE)data);

                    if (medicineProcessor.GetBid(ucMedicineTypeTree) != null && listBidMedicine != null && listBidMedicine.Count > 0)
                    {
                        var bidMedi = listBidMedicine.FirstOrDefault(o => o.MEDICINE_TYPE_ID == data.ID);
                        if (bidMedi != null)
                        {
                            this.currrentServiceAdo.BID_GROUP_CODE = bidMedi.BID_GROUP_CODE;
                            this.currrentServiceAdo.BID_NUM_ORDER = bidMedi.BID_NUM_ORDER;
                            this.currrentServiceAdo.BID_PACKAGE_CODE = bidMedi.BID_PACKAGE_CODE;
                            this.currrentServiceAdo.BID_YEAR = bidMedi.BID_YEAR;
                            this.currrentServiceAdo.BID_NUMBER = bidMedi.BID_NUMBER;
                        }
                    }

                    spinAmount.Focus();
                    spinAmount.SelectAll();

                    if (data.IS_SALE_EQUAL_IMP_PRICE == 1)
                    {
                        chkImprice.Checked = true;
                        chkImprice.Enabled = false;
                    }
                    else
                    {
                        chkImprice.Checked = false;
                        chkImprice.Enabled = true;
                    }
                }

                if (!this.currrentServiceAdo.IsAllowMissingPkgInfo)
                {
                    if (this.currrentServiceAdo.IsRequireHsd)
                    {
                        ValidControlExpiredDate1(dtExpiredDate);
                        ValidControlExpiredDate1(txtExpiredDate);
                        lciExpiredDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                    }
                    else
                    {
                        DevExpress.XtraEditors.DXErrorProvider.ValidationRule validationRule = new DevExpress.XtraEditors.DXErrorProvider.ValidationRule();
                        dxValidationProvider2.SetValidationRule(dtExpiredDate, validationRule);
                        dxValidationProvider2.SetValidationRule(txtExpiredDate, validationRule);
                        lciExpiredDate.AppearanceItemCaption.ForeColor = Color.Black;
                    }
                }

                fillInfoBidType();//Gan thong tin goi thau theo loai thuoc
                if (this.currentManuImpMest != null)
                {
                    var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.currentManuImpMest.MEDI_STOCK_ID);

                    SetValueByServiceAdo();
                }
                // SetValueByServiceAdo();
                LoadServicePatyByAdo();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialTypeTree_Click(UC.MaterialType.ADO.MaterialTypeADO data)
        {
            try
            {
                WaitingManager.Show();
                if (btnCancel.Enabled == true)
                {
                    SetEnableButton(false);
                    ResetValueControlDetail();
                    SetFocuTreeMediMate();
                    SetEnableButton(false);
                }
                this.currrentServiceAdo = null;
                if (data != null)
                {
                    this.currrentServiceAdo = new ADO.VHisServiceADO((V_HIS_MATERIAL_TYPE)data);

                    if (materialProcessor.GetBid(ucMaterialTypeTree) != null && listBidMaterial != null && listBidMaterial.Count > 0)
                    {
                        var bidMate = listBidMaterial.FirstOrDefault(o => o.MATERIAL_TYPE_ID == data.ID);
                        if (bidMate != null)
                        {
                            this.currrentServiceAdo.BID_GROUP_CODE = bidMate.BID_GROUP_CODE;
                            this.currrentServiceAdo.BID_NUM_ORDER = bidMate.BID_NUM_ORDER;
                            this.currrentServiceAdo.BID_PACKAGE_CODE = bidMate.BID_PACKAGE_CODE;
                            this.currrentServiceAdo.BID_YEAR = bidMate.BID_YEAR;
                            this.currrentServiceAdo.BID_NUMBER = bidMate.BID_NUMBER;
                        }
                    }

                    spinAmount.Focus();
                    spinAmount.SelectAll();
                    if (data.IS_SALE_EQUAL_IMP_PRICE == 1)
                    {
                        chkImprice.Checked = true;
                        chkImprice.Enabled = false;
                    }
                    else
                    {
                        chkImprice.Checked = false;
                        chkImprice.Enabled = true;
                    }
                }

                if (!this.currrentServiceAdo.IsAllowMissingPkgInfo)
                {
                    if (this.currrentServiceAdo.IsRequireHsd)
                    {
                        ValidControlExpiredDate1(dtExpiredDate);
                        ValidControlExpiredDate1(txtExpiredDate);
                        lciExpiredDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                    }
                    else
                    {
                        DevExpress.XtraEditors.DXErrorProvider.ValidationRule validationRule = new DevExpress.XtraEditors.DXErrorProvider.ValidationRule();
                        dxValidationProvider2.SetValidationRule(dtExpiredDate, validationRule);
                        dxValidationProvider2.SetValidationRule(txtExpiredDate, validationRule);
                        lciExpiredDate.AppearanceItemCaption.ForeColor = Color.Black;
                    }
                }
                fillInfoBidType();
                if (this.currentManuImpMest != null)
                {
                    var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.currentManuImpMest.MEDI_STOCK_ID);

                    SetValueByServiceAdo();
                }
                //SetValueByServiceAdo();
                LoadServicePatyByAdo();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
