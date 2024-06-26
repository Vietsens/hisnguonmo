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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisMedicalContractCreate.Run
{
    public partial class FormHisMedicalContractCreate : FormBase
    {
        #region LoadDataControl
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabMedicalContractMety.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.tabMedicalContractMety.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabMedicalContractMaty.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.MedicalContractMaty.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabMedicineType.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.tabBidMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabMaterialType.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.tabBidMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkClearControl.Properties.Caption = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.chkClearControl.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSupplier.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.cboSupplier.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSupplier.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciSupplier.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBid.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.cboBid.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBid.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciBid.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedicalContractCode.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciMedicalContractCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedicalContractName.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciMedicalContractName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNote.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDocumentSupplier.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.cboDocumentSupplier.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDocumentSupplier.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciDocumentSupplier.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDocumentSupplier.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciDocumentSupplier.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciVentureAgreening.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciVentureAgreening.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciVentureAgreening.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciVentureAgreening.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciValidFromDate.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciValidFromDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciValidToDate.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciValidToDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpPrice.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciImpPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpVat.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciImpVat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpiredDate.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciExpiredDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //    this.lciYearLifespan.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciYearLifespan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMonthLifespan.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciYearLifespan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDayLifespan.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciDayLifespan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRegisterNumber.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciRegisterNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNationalName.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.cboNationalName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkEditNational.Properties.Caption = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.chkEditNational.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConcentra.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciConcentra.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConcentra.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciConcentra.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciManufacturerCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciManufacturerCode.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciManufacturerCode.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciManufacturerCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboManufacturerName.Properties.NullText = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.cboManufacturerName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //    this.lblNgay.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lblNgay.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPriceVat.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciPriceVat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciContractPrice.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciContractPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPnNationalName.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciPnNationalName.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPnNationalName.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.lciPnNationalName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnAdd.Caption = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.barBtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("FormHisMedicalContractCreate.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadAsyncDataToCbo()
        {
            try
            {
                Task t1 = LoadAsyncDataToCboSupplier();
                Task t2 = LoadAsynceDataToCboBid();
                Task t3 = LoadAsyncDataToCboNationnal();
                Task t4 = LoadDataToCboManufacturer();

                await t1;
                await t2;
                await t3;
                await t4;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataToCboManufacturer()
        {
            try
            {
                List<HIS_MANUFACTURER> dataManufacturer = null;
                Task t = new Task(
                    () =>
                    {
                        dataManufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().Where(o => o.IS_ACTIVE == 1).ToList();
                    }
                );
                t.Start();
                await t;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MANUFACTURER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MANUFACTURER_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MANUFACTURER_NAME", "ID", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboManufacturerName, dataManufacturer, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAsyncDataToCboNationnal()
        {
            try
            {
                List<SDA_NATIONAL> dataNational = null;
                Task t = new Task(
                    () =>
                    {
                        dataNational = BackendDataWorker.Get<SDA_NATIONAL>().Where(o => o.IS_ACTIVE == 1).ToList();
                    }
                );
                t.Start();
                await t;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NATIONAL_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("NATIONAL_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NATIONAL_NAME", "ID", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboNationalName, dataNational, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAsynceDataToCboBid()
        {
            try
            {
                Task t = new Task(
                    () =>
                    {
                        CommonParam param = new CommonParam();
                        HisBidFilter filter = new HisBidFilter();
                        filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        this.ListBid = new BackendAdapter(param).Get<List<V_HIS_BID_1>>("api/HisBid/GetView1", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    }
                );
                t.Start();
                await t;

                string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                ListBid = ListBid.Where(o => (!o.VALID_TO_TIME.HasValue || (o.VALID_TO_TIME.HasValue && o.VALID_TO_TIME.Value > Inventec.Common.DateTime.Get.Now()))
                    && (String.IsNullOrWhiteSpace(o.ALLOW_UPDATE_LOGINNAMES) || (("," + o.ALLOW_UPDATE_LOGINNAMES + ",").Contains("," + loginname + ",")) || o.CREATOR.Equals(loginname))).ToList();

                ReloadCboBid(ListBid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadCboBid(List<V_HIS_BID_1> ListBid)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BID_NAME", "", 80, 1));
                columnInfos.Add(new ColumnInfo("BID_NUMBER", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BID_NUMBER", "ID", columnInfos, false, 400);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboBid, ListBid, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAsyncDataToCboSupplier()
        {
            try
            {
                List<HIS_SUPPLIER> dataSupplier = null;
                //Task t = new Task(
                //    () => {
                dataSupplier = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1).ToList();
                //    }    
                //);
                //t.Start();
                //await t;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SUPPLIER_CODE", "", 80, 1));
                columnInfos.Add(new ColumnInfo("SUPPLIER_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SUPPLIER_NAME", "ID", columnInfos, false, 400);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboSupplier, dataSupplier, controlEditorADO);
                ControlEditorLoader.Load(cboDocumentSupplier, dataSupplier, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                this.cboBid.EditValue = null;
                this.cboDocumentSupplier.EditValue = null;
                this.cboSupplier.EditValue = null;
                this.dtValidFromDate.EditValue = null;
                this.dtValidToDate.EditValue = null;
                this.txtMedicalContractCode.EditValue = null;
                this.txtMedicalContractName.EditValue = null;
                this.txtVentureAgreening.EditValue = null;
                this.txtNote.EditValue = null;

                SetDefaultValueControlDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControlDetail()
        {
            try
            {
                this.cboNationalName.EditValue = null;
                this.cboManufacturerName.EditValue = null;
                this.dtExpiredDate.EditValue = null;
                this.spAmount.EditValue = null;
                this.spContractPrice.EditValue = null;
                this.spDayLifespan.EditValue = null;
                this.spImpPrice.EditValue = null;
                this.spImpVat.EditValue = null;
                this.spHourLifespan.EditValue = null;
                this.spMonthLifespan.EditValue = null;
                this.spPriceVat.EditValue = null;
                this.txtConcentra.EditValue = null;
                this.txtManufacturerCode.EditValue = null;
                this.txtNationalName.EditValue = null;
                this.txtRegisterNumber.EditValue = null;
                this.dtImportDate.EditValue = null;
                this.dteDiscountFromDate.EditValue = null;
                this.dteDiscountToDate.EditValue = null;
                this.chkEditNational.Checked = false;
                //this.txtNationalName.Visible = false;
                //this.cboNationalName.Visible = true;

                this.spHourLifespan.Enabled = true;
                this.spMonthLifespan.Enabled = true;
                this.spDayLifespan.Enabled = true;

                txtNhomThau.EditValue = null;
                txtQDThau.EditValue = null;
                txtGhiChu.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task FillAsyncDataToGridMedicineType()
        {
            try
            {
                await this.taskLoadAsyncMediStock;
                //
                if (medicalContractId > 0)
                {
                    await this.taskLoadAsyncMedicalContract;
                }

                FillDataToGridMedicineType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task FillAsyncDataToGridMaterialType()
        {
            try
            {
                await this.taskLoadAsyncMediStock;
                //
                if (medicalContractId > 0)
                {
                    await this.taskLoadAsyncMedicalContract;
                }

                FillDataToGridMaterialType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task GetAsyncDataMedicineType()
        {
            try
            {
                Task tsMedicine = Task.Factory.StartNew(() =>
                {
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();

                });
                await tsMedicine;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMedicineType()
        {
            try
            {
                if (ListHisMedicineTypeAllData == null || ListHisMedicineTypeAllData.Count == 0)
                    ListHisMedicineTypeAllData = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o =>
                        o.IS_STOP_IMP != 1
                        && o.IS_LEAF == 1
                        && o.IS_ACTIVE == 1).ToList();
                List<V_HIS_MEDICINE_TYPE> listMedicineType = ListHisMedicineTypeAllData;
                if (this.chkIsOnlyShowByBusiness.Checked)
                {
                    if (this.currentStock != null && this.currentStock.IS_BUSINESS == (short)1)
                    {
                        listMedicineType = ListHisMedicineTypeAllData != null ? ListHisMedicineTypeAllData.Where(o => o.IS_BUSINESS == (short)1).ToList() : null;
                    }
                    else
                    {
                        listMedicineType = ListHisMedicineTypeAllData != null ? ListHisMedicineTypeAllData.Where(o => o.IS_BUSINESS != (short)1).ToList() : null;
                    }
                }
                this.ListHisMedicineType = listMedicineType;
                Inventec.Common.Logging.LogSystem.Debug("ListHisMedicineType_____");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task GetAsyncDataMaterialType()
        {
            try
            {
                Task tsMaterial = Task.Factory.StartNew(() =>
                {
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                });
                await tsMaterial;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMaterialType()
        {
            try
            {
                if(ListHisMaterialTypeAllData == null || ListHisMaterialTypeAllData.Count == 0)
                    ListHisMaterialTypeAllData = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o =>
                        o.IS_STOP_IMP != 1
                        && o.IS_LEAF == 1
                        && o.IS_ACTIVE == 1).ToList();
                List<V_HIS_MATERIAL_TYPE> listMaterialType = ListHisMaterialTypeAllData;
                if (this.chkIsOnlyShowByBusiness.Checked)
                {
                    if (this.currentStock != null && this.currentStock.IS_BUSINESS == (short)1)
                    {
                        listMaterialType = ListHisMaterialTypeAllData != null ? ListHisMaterialTypeAllData.Where(o => o.IS_BUSINESS == (short)1).ToList() : null;
                    }
                    else
                    {
                        listMaterialType = ListHisMaterialTypeAllData != null ? ListHisMaterialTypeAllData.Where(o => o.IS_BUSINESS != (short)1).ToList() : null;
                    }
                }
                this.ListHisMaterialType = listMaterialType;
                Inventec.Common.Logging.LogSystem.Debug("ListHisMaterialType_____");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region InitMetyMaty
        private void InitializeMedicineType()
        {
            try
            {
                medicineTypeProcessor = new UC.MedicineType.MedicineTypeProcessor();
                HIS.UC.MedicineType.MedicineTypeInitADO ado = new UC.MedicineType.MedicineTypeInitADO();
                ado.IsShowSearchPanel = true;
                ado.MedicineTypeClick = MedicineType_Click;
                ado.MedicineTypeRowEnter = MedicineType_RowEnter;
                ado.MedicineType_CustomUnboundColumnData = MedicineType_CustomUnboundColumnData;
                ado.MedicineTypeColumns = new List<HIS.UC.MedicineType.MedicineTypeColumn>();
                ado.ParentFieldName = "ParentField";
                ado.KeyFieldName = "KeyField";

                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__KEY_WORD_NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());

                //Column MedicineTypeCode
                HIS.UC.MedicineType.MedicineTypeColumn GcMedicineTypeCode = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "MEDICINE_TYPE_CODE", 80, false);
                GcMedicineTypeCode.VisibleIndex = 0;
                ado.MedicineTypeColumns.Add(GcMedicineTypeCode);

                //Column MedicineTypeName
                HIS.UC.MedicineType.MedicineTypeColumn GcMedicineTypeName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "MEDICINE_TYPE_NAME", 150, false);
                GcMedicineTypeName.VisibleIndex = 1;
                ado.MedicineTypeColumns.Add(GcMedicineTypeName);

                //Column ServiceUnitName
                HIS.UC.MedicineType.MedicineTypeColumn GcServiceUnitName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "SERVICE_UNIT_NAME_STR", 80, false);
                GcServiceUnitName.VisibleIndex = 2;
                GcServiceUnitName.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.MedicineTypeColumns.Add(GcServiceUnitName);

                //Column ManufacturerName
                HIS.UC.MedicineType.MedicineTypeColumn GcManufacturerName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 3;
                ado.MedicineTypeColumns.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.MedicineType.MedicineTypeColumn GcNationalName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 4;
                ado.MedicineTypeColumns.Add(GcNationalName);

                //Column UseFromName
                HIS.UC.MedicineType.MedicineTypeColumn GcUseFromName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MEDICINE__GC_MEDICINE_USE_FROM_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "MEDICINE_USE_FORM_NAME", 80, false);
                GcUseFromName.VisibleIndex = 5;
                ado.MedicineTypeColumns.Add(GcUseFromName);

                //Column AvtiveIngrBhytName
                HIS.UC.MedicineType.MedicineTypeColumn GcAvtiveIngrBhytName = new HIS.UC.MedicineType.MedicineTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "ACTIVE_INGR_BHYT_NAME", 100, false);
                GcAvtiveIngrBhytName.VisibleIndex = 6;
                ado.MedicineTypeColumns.Add(GcAvtiveIngrBhytName);

                //column so dang ky 
                HIS.UC.MedicineType.MedicineTypeColumn colSodangky = new UC.MedicineType.MedicineTypeColumn("Số đăng ký", "REGISTER_NUMBER", 70, false);
                colSodangky.VisibleIndex = 7;
                ado.MedicineTypeColumns.Add(colSodangky);

                // Số lượng (thầu)
                UC.MedicineType.MedicineTypeColumn colSoluong = new UC.MedicineType.MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_AMOUNT_IN_BID", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "AMOUNT_IN_BID", 90, false);
                colSoluong.VisibleIndex = 8;
                colSoluong.ToolTip = "Số lượng thầu";
                colSoluong.Format = new DevExpress.Utils.FormatInfo();
                colSoluong.Format.FormatString = "#,##0.";
                colSoluong.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MedicineTypeColumns.Add(colSoluong);

                // Nhóm thầu (thầu)
                UC.MedicineType.MedicineTypeColumn colNhomThau = new UC.MedicineType.MedicineTypeColumn("Nhóm thầu", "BID_GROUP_CODE", 90, false);
                colNhomThau.VisibleIndex = 9;
                colNhomThau.ToolTip = "Nhóm thầu";
                colNhomThau.Format = new DevExpress.Utils.FormatInfo();
                colNhomThau.Format.FormatString = "#,##0.";
                colNhomThau.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MedicineTypeColumns.Add(colNhomThau);

                // Số lượng hợp đồng
                UC.MedicineType.MedicineTypeColumn colSoluongHopDong = new UC.MedicineType.MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_AMOUNT_IN_CONTRACT", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "AMOUNT_IN_CONTRACT", 90, false);
                colSoluongHopDong.VisibleIndex = 10;
                colSoluongHopDong.ToolTip = "Số lượng đã có hợp đồng";
                colSoluongHopDong.Format = new DevExpress.Utils.FormatInfo();
                colSoluongHopDong.Format.FormatString = "#,##0.";
                colSoluongHopDong.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MedicineTypeColumns.Add(colSoluongHopDong);

                // Giá nhập (thầu)
                UC.MedicineType.MedicineTypeColumn colGianhap = new UC.MedicineType.MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_IMP_PRICE_IN_BID", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "IMP_PRICE_IN_BID_STR", 100, false);
                colGianhap.VisibleIndex = 11;
                colGianhap.Format = new DevExpress.Utils.FormatInfo();
                colGianhap.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                colGianhap.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MedicineTypeColumns.Add(colGianhap);

                // VAT (thầu)
                UC.MedicineType.MedicineTypeColumn colVAT = new UC.MedicineType.MedicineTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_VAT_IN_BID", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "IMP_VAT_RATIO_IN_BID_STR", 70, false);
                colVAT.VisibleIndex = 12;
                colVAT.Format = new DevExpress.Utils.FormatInfo();
                colVAT.Format.FormatString = "#,##0.";
                colVAT.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MedicineTypeColumns.Add(colVAT);

                this.ucMedicineType = (UserControl)medicineTypeProcessor.Run(ado);
                if (this.ucMedicineType != null)
                {
                    this.pnMedicineType.Controls.Add(this.ucMedicineType);
                    this.ucMedicineType.Dock = DockStyle.Fill;
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string AddStringByConfig(int num)
        {
            string str = null;
            try
            {
                if (num > 0)
                {
                    for (int i = 1; i <= num; i++)
                    {
                        str += "0";
                    }
                }
                else
                {
                    return str = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return str = "";
            }
            return str;
        }

        private void MedicineType_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    UC.MedicineType.ADO.MedicineTypeADO currentRow = e.Row as UC.MedicineType.ADO.MedicineTypeADO;
                    if (currentRow == null) return;
                    if (e.Column.FieldName == "SERVICE_UNIT_NAME_STR")
                    {
                        if (currentRow.IMP_UNIT_ID.HasValue)
                            e.Value = currentRow.IMP_UNIT_NAME;
                        else
                            e.Value = currentRow.SERVICE_UNIT_NAME;
                    }
                    else if (e.Column.FieldName == "IMP_VAT_RATIO_IN_BID_STR")
                    {
                        if (currentRow.IMP_VAT_RATIO_IN_BID.HasValue)
                        {
                            e.Value = currentRow.IMP_VAT_RATIO_IN_BID.Value;
                        }
                        else if (currentRow.IMP_VAT_RATIO.HasValue)
                        {
                            e.Value = currentRow.IMP_VAT_RATIO;
                        }
                    }
                    else if (e.Column.FieldName == "IMP_PRICE_IN_BID_STR")
                    {
                        if (currentRow.IMP_PRICE_IN_BID.HasValue)
                        {
                            e.Value = currentRow.IMP_PRICE_IN_BID.Value;
                        }
                        else if (currentRow.IMP_PRICE.HasValue)
                        {
                            e.Value = currentRow.IMP_PRICE;
                        }
                    }
                    else if (e.Column.FieldName == "BID_GROUP_CODE")
                    {
                        if (currentRow.BidGroupCode != null)
                        {
                            e.Value = currentRow.BidGroupCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedicineType_Click(UC.MedicineType.ADO.MedicineTypeADO data)
        {
            try
            {
                this.bidGroupCodeSelected = null;
                this.medicineType = null;
                if (data != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    this.medicineType = data;
                    this.EditMedicine = new ADO.MetyMatyADO();
                    this.ActionType = GlobalVariables.ActionAdd;
                    dtExpiredDate.Enabled = true;
                    dtImportDate.Enabled = true;
                    SetValueForAdd();
                    if (!string.IsNullOrEmpty(data.BidGroupCode))
                    {
                        this.bidGroupCodeSelected = data.BidGroupCode;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedicineType_RowEnter(UC.MedicineType.ADO.MedicineTypeADO data)
        {
            try
            {
                MedicineType_Click(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitializeMaterialType()
        {
            try
            {
                materialTypeProcessor = new UC.MaterialType.MaterialTypeTreeProcessor();
                HIS.UC.MaterialType.MaterialTypeInitADO ado = new UC.MaterialType.MaterialTypeInitADO();
                ado.IsShowSearchPanel = true;
                ado.MaterialTypeClick = MaterialType_Click;
                ado.MaterialTypeRowEnter = MaterialType_RowEnter;
                ado.MaterialType_CustomUnboundColumnData = MaterialType_CustomUnboundColumnData;
                ado.MaterialTypeColumns = new List<HIS.UC.MaterialType.MaterialTypeColumn>();
                ado.ParentFieldName = "ParentField";
                ado.KeyFieldName = "KeyField";

                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__KEY_WORD_NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());

                //Column MaterialTypeCode
                HIS.UC.MaterialType.MaterialTypeColumn GcMaterialTypeCode = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MATERIAL__GC_MATERIAL_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "MATERIAL_TYPE_CODE", 80, false);
                GcMaterialTypeCode.VisibleIndex = 0;
                ado.MaterialTypeColumns.Add(GcMaterialTypeCode);

                //Column MaterialTypeName
                HIS.UC.MaterialType.MaterialTypeColumn GcMaterialTypeName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GV_MATERIAL__GC_MATERIAL_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "MATERIAL_TYPE_NAME", 150, false);
                GcMaterialTypeName.VisibleIndex = 1;
                ado.MaterialTypeColumns.Add(GcMaterialTypeName);

                //Column ServiceUnitName
                HIS.UC.MaterialType.MaterialTypeColumn GcServiceUnitName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "SERVICE_UNIT_NAME_STR", 80, false);
                GcServiceUnitName.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                GcServiceUnitName.VisibleIndex = 2;
                ado.MaterialTypeColumns.Add(GcServiceUnitName);

                //Column ManufacturerName
                HIS.UC.MaterialType.MaterialTypeColumn GcManufacturerName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "MANUFACTURER_NAME", 150, false);
                GcManufacturerName.VisibleIndex = 3;
                ado.MaterialTypeColumns.Add(GcManufacturerName);

                //Column NationalName
                HIS.UC.MaterialType.MaterialTypeColumn GcNationalName = new HIS.UC.MaterialType.MaterialTypeColumn
                    (Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_BID_CREATE__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture()), "NATIONAL_NAME", 80, false);
                GcNationalName.VisibleIndex = 4;
                ado.MaterialTypeColumns.Add(GcNationalName);

                // Số lượng (thầu)
                UC.MaterialType.MaterialTypeColumn colSoluong = new UC.MaterialType.MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_AMOUNT_IN_BID", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "AMOUNT_IN_BID", 90, false);
                colSoluong.VisibleIndex = 5;
                colSoluong.ToolTip = "Số lượng đã có hợp đồng";
                colSoluong.Format = new DevExpress.Utils.FormatInfo();
                colSoluong.Format.FormatString = "#,##0.";
                colSoluong.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(colSoluong);

                // Nhóm thầu (thầu)
                UC.MaterialType.MaterialTypeColumn colNhomThau = new UC.MaterialType.MaterialTypeColumn("Nhóm thầu", "BidGroupCode", 90, false);
                colNhomThau.VisibleIndex = 6;
                colNhomThau.ToolTip = "Nhóm thầu";
                colNhomThau.Format = new DevExpress.Utils.FormatInfo();
                colNhomThau.Format.FormatString = "#,##0.";
                colNhomThau.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(colNhomThau);

                // Số lượng hợp đồng
                UC.MaterialType.MaterialTypeColumn colSoluongHopDong = new UC.MaterialType.MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_AMOUNT_IN_CONTRACT", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "AMOUNT_IN_CONTRACT", 90, false);
                colSoluongHopDong.VisibleIndex = 7;
                colSoluongHopDong.ToolTip = "Số lượng đã có hợp đồng";
                colSoluongHopDong.Format = new DevExpress.Utils.FormatInfo();
                colSoluongHopDong.Format.FormatString = "#,##0.";
                colSoluongHopDong.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(colSoluongHopDong);

                // Giá nhập (thầu)
                UC.MaterialType.MaterialTypeColumn colGianhap = new UC.MaterialType.MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_IMP_PRICE_IN_BID", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "IMP_PRICE_IN_BID_STR", 100, false);
                colGianhap.VisibleIndex = 8;
                colGianhap.Format = new DevExpress.Utils.FormatInfo();
                colGianhap.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                colGianhap.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(colGianhap);

                // VAT (thầu)
                UC.MaterialType.MaterialTypeColumn colVAT = new UC.MaterialType.MaterialTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMP_MEST_CREATE__TREE_MEDICINE__COLUMN_VAT_IN_BID", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), "IMP_VAT_RATIO_IN_BID_STR", 70, false);
                colVAT.VisibleIndex = 9;
                colVAT.Format = new DevExpress.Utils.FormatInfo();
                colVAT.Format.FormatString = "#,##0.";
                colVAT.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.MaterialTypeColumns.Add(colVAT);

                this.ucMaterialType = (UserControl)materialTypeProcessor.Run(ado);
                if (this.ucMaterialType != null)
                {
                    this.pnMaterialType.Controls.Add(this.ucMaterialType);
                    this.ucMaterialType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MaterialType_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    UC.MaterialType.ADO.MaterialTypeADO currentRow = e.Row as UC.MaterialType.ADO.MaterialTypeADO;
                    if (currentRow == null) return;
                    if (e.Column.FieldName == "SERVICE_UNIT_NAME_STR")
                    {
                        if (currentRow.IMP_UNIT_ID.HasValue)
                            e.Value = currentRow.IMP_UNIT_NAME;
                        else
                            e.Value = currentRow.SERVICE_UNIT_NAME;
                    }
                    else if (e.Column.FieldName == "IMP_VAT_RATIO_IN_BID_STR")
                    {
                        if (currentRow.IMP_VAT_RATIO_IN_BID.HasValue)
                        {
                            e.Value = currentRow.IMP_VAT_RATIO_IN_BID.Value;
                        }
                        else if (currentRow.IMP_VAT_RATIO.HasValue)
                        {
                            e.Value = currentRow.IMP_VAT_RATIO;
                        }
                    }
                    else if (e.Column.FieldName == "IMP_PRICE_IN_BID_STR")
                    {
                        if (currentRow.IMP_PRICE_IN_BID.HasValue)
                        {
                            e.Value = currentRow.IMP_PRICE_IN_BID.Value;
                        }
                        else if (currentRow.IMP_PRICE.HasValue)
                        {
                            e.Value = currentRow.IMP_PRICE;
                        }
                    }
                    else if (e.Column.FieldName == "BID_GROUP_CODE")
                    {
                        if (currentRow.BidGroupCode != null)
                        {
                            e.Value = currentRow.BidGroupCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MaterialType_Click(UC.MaterialType.ADO.MaterialTypeADO data)
        {
            try
            {
                this.materialType = new UC.MaterialType.ADO.MaterialTypeADO();
                if (data != null)
                {
                    this.materialType = data;
                    this.EditMaterial = new ADO.MetyMatyADO();
                    this.ActionType = GlobalVariables.ActionAdd;
                    dtExpiredDate.Enabled = true;
                    dtImportDate.Enabled = true;
                    SetValueForAdd();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    if (!string.IsNullOrEmpty(data.BidGroupCode))
                    {
                        this.bidGroupCodeSelected = data.BidGroupCode;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MaterialType_RowEnter(UC.MaterialType.ADO.MaterialTypeADO data)
        {
            try
            {
                MaterialType_Click(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitGridEdit()
        {
            try
            {
                ucContractMety = new UC_LoadEdit(GridEdit_Click, GridDelete_ButtonClick, true);
                ucContractMety.Dock = DockStyle.Fill;
                ucContractMaty = new UC_LoadEdit(GridEdit_Click, GridDelete_ButtonClick, false);
                ucContractMaty.Dock = DockStyle.Fill;
                this.pnContractMety.Controls.Add(ucContractMety);
                this.pnContractMaty.Controls.Add(ucContractMaty);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void GridEdit_Click(ADO.MetyMatyADO data)
        {
            try
            {
                SetDefaultValueControlDetail();
                this.ActionType = GlobalVariables.ActionEdit;

                if (xtraTabContractMetyMaty.SelectedTabPage == tabMedicalContractMety)
                {
                    this.EditMedicine = data;
                    this.medicineType = new UC.MedicineType.ADO.MedicineTypeADO();
                    this.medicineType.ID = data.ID;
                    xtraTabMetyMate.SelectedTabPage = tabMedicineType;
                    EnableLeftControl(true);
                    txtRegisterNumber.Enabled = true;
                }
                else if (xtraTabContractMetyMaty.SelectedTabPage == tabMedicalContractMaty)
                {
                    this.EditMaterial = data;
                    this.materialType = new UC.MaterialType.ADO.MaterialTypeADO();
                    this.materialType.ID = data.ID;
                    xtraTabMetyMate.SelectedTabPage = tabMaterialType;
                    EnableLeftControl(true);
                    txtRegisterNumber.Enabled = false;
                }

                spAmount.EditValue = data.AMOUNT;
                spImpPrice.EditValue = data.IMP_PRICE;
                spImpVat.EditValue = (data.IMP_VAT_RATIO ?? 0) * 100;
                spContractPrice.EditValue = data.CONTRACT_PRICE;
                txtRegisterNumber.Text = data.REGISTER_NUMBER;
                txtConcentra.Text = data.CONCENTRA;
                txtRegisterNumber.Text = data.REGISTER_NUMBER;

                txtQDThau.Text = data.BID_NUMBER;
                txtNhomThau.Text = data.BID_GROUP_CODE;
                txtGhiChu.Text = data.NOTE;

                spMonthLifespan.EditValue = data.MonthLifespan;
                spDayLifespan.EditValue = data.DayLifespan;
                spHourLifespan.EditValue = data.HourLifespan;

                if (!string.IsNullOrEmpty(data.BID_GROUP_CODE))
                {
                    this.bidGroupCodeSelected = data.BID_GROUP_CODE;
                }
                else
                {
                    this.bidGroupCodeSelected = null;
                }

                long bidId = Inventec.Common.TypeConvert.Parse.ToInt64((cboBid.EditValue ?? 0).ToString());
                if (bidId > 0)
                {
                    spMonthLifespan.Enabled = false;
                    spDayLifespan.Enabled = false;
                    spHourLifespan.Enabled = false;
                    lciHour.Enabled = false;
                    lciPnNationalName.Enabled = false;
                    dtExpiredDate.Enabled = false;
                    txtRegisterNumber.Enabled = false;
                    txtNationalName.Enabled = false;
                    chkEditNational.Enabled = false;
                    txtConcentra.Enabled = false;
                    txtManufacturerCode.Enabled = false;
                    cboManufacturerName.Enabled = false;
                    txtNhomThau.Enabled = false;
                    txtQDThau.Enabled = false;
                    txtGhiChu.Enabled = false;
                }

                var manu = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.ID == data.MANUFACTURER_ID);
                if (manu != null)
                {
                    cboManufacturerName.EditValue = manu.ID;
                    txtManufacturerCode.Text = data.MANUFACTURER_CODE;
                }

                var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_NAME == data.NATIONAL_NAME).ToList();
                if (national != null && national.Count() > 0)
                {
                    txtNationalName.Text = national[0].NATIONAL_NAME;
                    cboNationalName.EditValue = national[0].ID;
                    txtNationalName.Visible = false;
                    cboNationalName.Visible = true;
                    chkEditNational.CheckState = CheckState.Unchecked;
                }
                else
                {
                    txtNationalName.Text = data.NATIONAL_NAME;
                    cboNationalName.EditValue = null;
                    txtNationalName.Visible = true;
                    cboNationalName.Visible = false;
                    chkEditNational.CheckState = CheckState.Checked;
                }

                if (data.EXPIRED_DATE.HasValue)
                {
                    dtExpiredDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXPIRED_DATE.Value);
                }
                if (data.IMP_EXPIRED_DATE.HasValue)
                {
                    dtImportDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.IMP_EXPIRED_DATE.Value);
                }
                if (data.DISCOUNT_FROM_DATE.HasValue)
                    dteDiscountFromDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.DISCOUNT_FROM_DATE.Value);
                if (data.DISCOUNT_TO_DATE.HasValue)
                    dteDiscountToDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.DISCOUNT_TO_DATE.Value);
                spAmount.Focus();
                spAmount.SelectAll();
                dxValidationProviderMetyMate.RemoveControlError(spAmount);
                dxValidationProviderMetyMate.RemoveControlError(spImpPrice);
                dxValidationProviderMetyMate.RemoveControlError(spImpVat);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void GridDelete_ButtonClick(ADO.MetyMatyADO data)
        {
            try
            {
                if (xtraTabContractMetyMaty.SelectedTabPage == tabMedicalContractMety)
                {
                    this.ListMedicineADO.Remove(data);
                }
                else if (xtraTabContractMetyMaty.SelectedTabPage == tabMedicalContractMaty)
                {
                    this.ListMaterialADO.Remove(data);
                }

                UpdateGrid();
                SetDefaultValueControlDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region LoadDataEdit
        Task taskLoadAsyncMedicalContract { get; set; }
        private async Task LoadAsyncCurrentDataEdit()
        {
            if (medicalContractId > 0)
            {
                //Thread ThreadMedicalContract = new Thread(LoadMedicalContract);
                //Thread ThreadMedicine = new Thread(LoadMedicineType);
                //Thread ThreadMaterial = new Thread(LoadMaterialType);
                try
                {
                    //ThreadMedicalContract.Start();
                    //ThreadMedicine.Start();
                    //ThreadMaterial.Start();

                    //ThreadMedicalContract.Join();
                    //ThreadMedicine.Join();
                    //ThreadMaterial.Join();
                    this.taskLoadAsyncMedicalContract = LoadAsyncMedicalContract();
                    Task t2 = LoadAsyncMedicineType();
                    Task t3 = LoadAsyncMaterialType();

                    await this.taskLoadAsyncMedicalContract;
                    await t2;
                    await t3;
                }
                catch (Exception ex)
                {
                    //ThreadMedicalContract.Abort();
                    //ThreadMedicine.Abort();
                    //ThreadMaterial.Abort();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }

        bool isLoadingAsyncMedicalContract = false;
        private async Task LoadAsyncMedicalContract()
        {
            try
            {
                Task t = new Task(
                    () =>
                    {
                        LoadMedicalContract();
                    }
                );
                t.Start();
                await t;
                await taskLoadAsyncDataToCbo;

                this.isLoadingAsyncMedicalContract = true;
                cboSupplier.Enabled = false;
                cboSupplier.EditValue = CurrentContact.SUPPLIER_ID;
                cboBid.Enabled = false;
                cboBid.EditValue = CurrentContact.BID_ID;
                txtMedicalContractCode.Text = CurrentContact.MEDICAL_CONTRACT_CODE;
                txtMedicalContractName.Text = CurrentContact.MEDICAL_CONTRACT_NAME;
                cboDocumentSupplier.EditValue = CurrentContact.DOCUMENT_SUPPLIER_ID;
                txtVentureAgreening.Text = CurrentContact.VENTURE_AGREENING;
                txtNote.Text = CurrentContact.NOTE;

                if (CurrentContact.VALID_FROM_DATE.HasValue)
                {
                    dtValidFromDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(CurrentContact.VALID_FROM_DATE ?? 0) ?? DateTime.Now;
                }
                else
                {
                    dtValidFromDate.EditValue = null;
                }

                if (CurrentContact.VALID_TO_DATE.HasValue)
                {
                    dtValidToDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(CurrentContact.VALID_TO_DATE ?? 0) ?? DateTime.Now;
                }
                else
                {
                    dtValidToDate.EditValue = null;
                }
                this.isLoadingAsyncMedicalContract = false;
            }
            catch (Exception ex)
            {
                this.isLoadingAsyncMedicalContract = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAsyncMedicineType()
        {
            try
            {
                Task t = new Task(
                    () =>
                    {
                        LoadMedicineType();
                    }
                );
                t.Start();
                await t;
                ucContractMety.Reload(this.ListMedicineADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAsyncMaterialType()
        {
            try
            {
                Task t = new Task(
                    () =>
                    {
                        LoadMaterialType();
                    }
                );
                t.Start();
                await t;
                ucContractMaty.Reload(this.ListMaterialADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMedicalContract()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicalContractFilter filter = new HisMedicalContractFilter();
                filter.ID = this.medicalContractId;
                var medicalContract = new BackendAdapter(param).Get<List<HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                if (medicalContract != null && medicalContract.Count > 0)
                {
                    CurrentContact = medicalContract.FirstOrDefault();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("LoadMedicalContract()_CurrentContact", CurrentContact));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMaterialType()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMediContractMatyFilter filter = new HisMediContractMatyFilter();
                filter.MEDICAL_CONTRACT_ID = this.medicalContractId;
                mediContractMaty = new BackendAdapter(param).Get<List<HIS_MEDI_CONTRACT_MATY>>("api/HisMediContractMaty/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                if (mediContractMaty != null && mediContractMaty.Count > 0)
                {
                    this.ListMaterialADO = new List<ADO.MetyMatyADO>();
                    foreach (var item in mediContractMaty)
                    {
                        ADO.MetyMatyADO ado = new ADO.MetyMatyADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MetyMatyADO>(ado, item);

                        ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;

                        var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                        if (maty != null)
                        {
                            ado.MEDICINE_TYPE_CODE = maty.MATERIAL_TYPE_CODE;
                            ado.MEDICINE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                            ado.SERVICE_UNIT_NAME = maty.IMP_UNIT_ID.HasValue ? maty.IMP_UNIT_NAME : maty.SERVICE_UNIT_NAME;
                            if (maty.IS_BUSINESS == (short)1)
                                this.hasBusiness = true;
                            else
                                this.hasNotBusiness = true;
                        }

                        var manu = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.ID == item.MANUFACTURER_ID);
                        if (manu != null)
                        {
                            ado.MANUFACTURER_CODE = manu.MANUFACTURER_CODE;
                            ado.MANUFACTURER_ID = manu.ID;
                            ado.MANUFACTURER_NAME = manu.MANUFACTURER_NAME;
                        }

                        ado.ID = item.MATERIAL_TYPE_ID;
                        ado.CONTRACT_MATY_METY_ID = item.ID;
                        ado.EXPIRED_DATE = item.EXPIRED_DATE;

                        ado.MonthLifespan = item.MONTH_LIFESPAN;
                        ado.DayLifespan = item.DAY_LIFESPAN;
                        ado.HourLifespan = item.HOUR_LIFESPAN;


                        ado.NOTE = item.NOTE;
                        ado.BID_GROUP_CODE = item.BID_GROUP_CODE;
                        ado.BID_NUMBER = item.BID_NUMBER;
                        ado.DISCOUNT_FROM_DATE = item.DISCOUNT_FROM_DATE;
                        ado.DISCOUNT_TO_DATE = item.DISCOUNT_TO_DATE;
                        ado.BID_METY_MATY_ID = item.BID_MATERIAL_TYPE_ID;
                        if (item.BID_MATERIAL_TYPE_ID != null)
                        {
                            CommonParam paramBid = new CommonParam();
                            HisBidMaterialTypeFilter filterBid = new HisBidMaterialTypeFilter();
                            filterBid.ID = item.BID_MATERIAL_TYPE_ID;
                            var BID_GROUP_CODE = new BackendAdapter(paramBid).Get<List<HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, paramBid).FirstOrDefault().BID_GROUP_CODE;
                            if (!string.IsNullOrEmpty(BID_GROUP_CODE))
                            {
                                ado.BID_GROUP_CODE = BID_GROUP_CODE;
                            }
                        }

                        this.ListMaterialADO.Add(ado);
                        ListMaterialADOTemp.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMedicineType()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMediContractMetyFilter filter = new HisMediContractMetyFilter();
                filter.MEDICAL_CONTRACT_ID = this.medicalContractId;
                mediContractMety = new BackendAdapter(param).Get<List<HIS_MEDI_CONTRACT_METY>>("api/HisMediContractMety/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                if (mediContractMety != null && mediContractMety.Count > 0)
                {
                    this.ListMedicineADO = new List<ADO.MetyMatyADO>();
                    foreach (var item in mediContractMety)
                    {
                        ADO.MetyMatyADO ado = new ADO.MetyMatyADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.MetyMatyADO>(ado, item);

                        ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;

                        var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                        if (mety != null)
                        {
                            ado.MEDICINE_TYPE_CODE = mety.MEDICINE_TYPE_CODE;
                            ado.MEDICINE_TYPE_NAME = mety.MEDICINE_TYPE_NAME;
                            ado.SERVICE_UNIT_NAME = mety.IMP_UNIT_ID.HasValue ? mety.IMP_UNIT_NAME : mety.SERVICE_UNIT_NAME;
                            ado.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                            ado.ACTIVE_INGR_BHYT_NAME = mety.ACTIVE_INGR_BHYT_NAME;
                            if (mety.IS_BUSINESS == (short)1)
                                this.hasBusiness = true;
                            else
                                this.hasNotBusiness = true;
                        }

                        var manu = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.ID == item.MANUFACTURER_ID);
                        if (manu != null)
                        {
                            ado.MANUFACTURER_CODE = manu.MANUFACTURER_CODE;
                            ado.MANUFACTURER_ID = manu.ID;
                            ado.MANUFACTURER_NAME = manu.MANUFACTURER_NAME;
                        }

                        ado.ID = item.MEDICINE_TYPE_ID;
                        ado.CONTRACT_MATY_METY_ID = item.ID;
                        ado.EXPIRED_DATE = item.EXPIRED_DATE;

                        ado.MonthLifespan = item.MONTH_LIFESPAN;
                        ado.DayLifespan = item.DAY_LIFESPAN;
                        ado.HourLifespan = item.HOUR_LIFESPAN;

                        ado.BID_METY_MATY_ID = item.BID_MEDICINE_TYPE_ID;
                        ado.REGISTER_NUMBER = item.MEDICINE_REGISTER_NUMBER;

                        ado.NOTE = item.NOTE;
                        ado.BID_GROUP_CODE = item.BID_GROUP_CODE;
                        ado.BID_NUMBER = item.BID_NUMBER;
                        ado.DISCOUNT_FROM_DATE = item.DISCOUNT_FROM_DATE;
                        ado.DISCOUNT_TO_DATE = item.DISCOUNT_TO_DATE;
                        if (item.BID_MEDICINE_TYPE_ID != null)
                        {
                            CommonParam paramBid = new CommonParam();
                            HisBidMedicineTypeFilter filterBid = new HisBidMedicineTypeFilter();
                            filterBid.ID = item.BID_MEDICINE_TYPE_ID;
                            var BID_GROUP_CODE = new BackendAdapter(paramBid).Get<List<HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/Get", ApiConsumer.ApiConsumers.MosConsumer, filterBid, paramBid).FirstOrDefault().BID_GROUP_CODE;
                            if (!string.IsNullOrEmpty(BID_GROUP_CODE))
                            {
                                ado.BID_GROUP_CODE = BID_GROUP_CODE;
                            }
                        }

                        this.ListMedicineADO.Add(ado);
                        ListMedicineADOTemp.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region type

        private void ReloadDataBid(long supplierId)
        {
            try
            {
                if (supplierId > 0)
                {
                    var listBids = this.ListBid.Where(o => ("," + o.SUPPLIER_IDS + ",").Contains("," + supplierId + ",")).ToList();
                    ReloadCboBid(listBids);
                    if ((listBids == null || listBids.Count == 0) || (cboBid.EditValue != null && listBids != null && listBids.FirstOrDefault(o=>o.ID == Int64.Parse(cboBid.EditValue.ToString())) == null))
                        cboBid.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadDataGridMetyMaty()
        {
            try
            {
                if (xtraTabMetyMate.SelectedTabPage == tabMedicineType)
                {
                    Inventec.Common.Logging.LogSystem.Info("tabMedicineType_______");
                    backgroundWorker1.RunWorkerAsync();
                }
                if (xtraTabMetyMate.SelectedTabPage == tabMaterialType)
                {
                    Inventec.Common.Logging.LogSystem.Info("tabMaterialType_______");
                    backgroundWorker2.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
