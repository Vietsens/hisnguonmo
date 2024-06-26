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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using HIS.Desktop.Plugins.InImpMestEdit.ADO;
using HIS.Desktop.Plugins.InImpMestEdit.Config;
using HIS.Desktop.Plugins.InImpMestEdit.Validation;
using HIS.Desktop.Utility;
using HIS.UC.MaterialType;
using HIS.UC.MedicineType;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InImpMestEdit
{
    public partial class FormInImpMestEdit : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private long impMestId;
        private System.Globalization.CultureInfo cultureLang;
        private HIS_IMP_MEST currentImpMest;
        private HIS_INIT_IMP_MEST currentInitImpMest;
        //private HIS_INVE_IMP_MEST currentInveImpMest;
        //private HIS_OTHER_IMP_MEST currentOtherImpMest;

        private MedicineTypeProcessor medicineProcessor = null;
        private MaterialTypeTreeProcessor materialProcessor = null;
        private UserControl ucMedicineTypeTree = null;
        private UserControl ucMaterialTypeTree = null;

        private Dictionary<long, List<VHisServicePatyADO>> dicServicePaty = new Dictionary<long, List<VHisServicePatyADO>>();
        private List<VHisServicePatyADO> listServicePatyAdo = new List<VHisServicePatyADO>();
        private List<VHisServiceADO> listServiceADO = new List<VHisServiceADO>();
        private VHisServiceADO currrentServiceAdo = null;
        private ResultImpMestADO resultADO = null;
        private List<V_HIS_MEDI_STOCK> listMediStock = new List<V_HIS_MEDI_STOCK>();
        private List<HIS_IMP_MEST_TYPE> listImpMestType = new List<HIS_IMP_MEST_TYPE>();
        private HIS_IMP_MEST_TYPE currentImpMestType = null;
        private int positionHandleControl = -1;
        private bool isload;
        #endregion

        #region Construct
        public FormInImpMestEdit(Inventec.Desktop.Common.Modules.Module moduleData)
		:base(moduleData)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormInImpMestEdit(Inventec.Desktop.Common.Modules.Module moduleData, long impMestId)	
            : this(moduleData)
        {
            try
            {
                // TODO: Complete member initialization
                this.moduleData = moduleData;
                this.impMestId = impMestId;
                this.Text = moduleData.text;
                InitMedicineTypeTree();
                InitMaterialTypeTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        #region load
        #region Initialize
        private void InitMedicineTypeTree()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.medicineProcessor = new MedicineTypeProcessor();
                MedicineTypeInitADO ado = new MedicineTypeInitADO();
                ado.IsShowSearchPanel = true;
                ado.MedicineTypeClick = medicineTypeTree_Click;
                ado.MedicineTypeRowEnter = medicineTypeTree_Click;
                ado.MedicineTypeColumns = new List<MedicineTypeColumn>();
                ado.IsAutoWidth = true;
                ado.Keyword_NullValuePrompt = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TXT_KEYWORD__NULL_VALUE");

                //MedicineTypeCode
                MedicineTypeColumn colMedicineTypeCode = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_MEDICINE_TYPE_CODE"),
                    "MEDICINE_TYPE_CODE", 70, false);
                colMedicineTypeCode.VisibleIndex = 0;
                ado.MedicineTypeColumns.Add(colMedicineTypeCode);

                //MedicineTypeName
                MedicineTypeColumn colMedicineTypeName = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_MEDICINE_TYPE_NAME"),
                    "MEDICINE_TYPE_NAME", 250, false);
                colMedicineTypeName.VisibleIndex = 1;
                ado.MedicineTypeColumns.Add(colMedicineTypeName);

                //ServiceUnitName
                MedicineTypeColumn colServiceUnitName = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_SERVICE_UNIT_NAME"),
                    "SERVICE_UNIT_NAME", 50, false);
                colServiceUnitName.VisibleIndex = 2;
                ado.MedicineTypeColumns.Add(colServiceUnitName);

                //NationalName
                MedicineTypeColumn colNationalName = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_NATIONAL_NAME"),
                    "NATIONAL_NAME", 100, false);
                colServiceUnitName.VisibleIndex = 3;
                ado.MedicineTypeColumns.Add(colNationalName);

                //ManufactureName
                MedicineTypeColumn colManufactureName = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_MANUFACTURE_NAME"),
                    "MANUFACTURER_NAME", 120, false);
                colManufactureName.VisibleIndex = 4;
                ado.MedicineTypeColumns.Add(colManufactureName);

                //RegisterNumber
                MedicineTypeColumn colRegisterNumber = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_REGISTER_NUMBER"),
                    "REGISTER_NUMBER", 70, false);
                colRegisterNumber.VisibleIndex = 5;
                ado.MedicineTypeColumns.Add(colRegisterNumber);

                //ActiveAngrBhytName
                MedicineTypeColumn colActiveAngrBhytName = new MedicineTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MEDICINE__COLUMN_ACTIVE_INGR_BHYT_NAME"),
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

        private void InitMaterialTypeTree()
        {
            try
            {
                var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                this.materialProcessor = new MaterialTypeTreeProcessor();
                MaterialTypeInitADO ado = new MaterialTypeInitADO();
                ado.IsShowSearchPanel = true;
                ado.MaterialTypeClick = materialTypeTree_Click;
                ado.MaterialTypeRowEnter = materialTypeTree_Click;
                ado.MaterialTypeColumns = new List<MaterialTypeColumn>();
                ado.IsAutoWidth = true;
                ado.Keyword_NullValuePrompt = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TXT_KEYWORD__NULL_VALUE");

                //MaterialTypeCode
                MaterialTypeColumn colMaterialTypeCode = new MaterialTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MATERIAL__COLUMN_MATERIAL_TYPE_CODE"),
                    "MATERIAL_TYPE_CODE", 70, false);
                colMaterialTypeCode.VisibleIndex = 0;
                ado.MaterialTypeColumns.Add(colMaterialTypeCode);

                //MedicineTypeName
                MaterialTypeColumn colMaterialTypeName = new MaterialTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MATERIAL__COLUMN_MATERIAL_TYPE_NAME"),
                    "MATERIAL_TYPE_NAME", 250, false);
                colMaterialTypeName.VisibleIndex = 1;
                ado.MaterialTypeColumns.Add(colMaterialTypeName);

                //ServiceUnitName
                MaterialTypeColumn colServiceUnitName = new MaterialTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MATERIAL__COLUMN_SERVICE_UNIT_NAME"),
                    "SERVICE_UNIT_NAME", 50, false);
                colServiceUnitName.VisibleIndex = 2;
                ado.MaterialTypeColumns.Add(colServiceUnitName);

                //NationalName
                MaterialTypeColumn colNationalName = new MaterialTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MATERIAL__COLUMN_NATIONAL_NAME"),
                    "NATIONAL_NAME", 120, false);
                colServiceUnitName.VisibleIndex = 3;
                ado.MaterialTypeColumns.Add(colNationalName);

                //ManufactureName
                MaterialTypeColumn colManufactureName = new MaterialTypeColumn(KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TREE_MATERIAL__COLUMN_MANUFACTURE_NAME"),
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
        #endregion

        #region Data
        private void FormInImpMestEdit_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                LoadCurrentImpMest();

                SetIcon();

                LoadKeysFromlanguage();

                SetDefaultValueControl();

                CurrentDaTaLoad();

                ValidControls();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadCurrentImpMest()
        {
            try
            {
                if (this.impMestId <= 0)
                {
                    MessageBox.Show(Resources.ResourceMessage.MaNhapKhongHopLe);
                    this.Close();
                }

                listServiceADO = new List<VHisServiceADO>();

                CommonParam param = new CommonParam();

                HisImpMestFilter filter = new HisImpMestFilter();
                filter.ID = this.impMestId;
                var impMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GET, ApiConsumers.MosConsumer, filter, param);
                if (impMest != null && impMest.Count == 1)
                {
                    currentImpMest = impMest.FirstOrDefault();
                    cboImpMestType.EditValue = currentImpMest.IMP_MEST_TYPE_ID;
                    cboMediStock.EditValue = currentImpMest.MEDI_STOCK_ID;
                    txtDescription.Text = currentImpMest.DESCRIPTION;
                    if (currentImpMest.IMP_MEST_TYPE_ID == HisImpMestTypeCFG.HisImpMestTypeId__Init)
                    {
                        HisInitImpMestFilter initFilter = new HisInitImpMestFilter();
                        initFilter.IMP_MEST_ID = currentImpMest.ID;
                        var init = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_INIT_IMP_MEST>>(HisRequestUriStore.HIS_INIT_IMP_MEST_GET, ApiConsumers.MosConsumer, initFilter, param);
                        if (init != null && init.Count == 1)
                        {
                            currentInitImpMest = init.FirstOrDefault();
                        }
                    }
                    else
                    {
                        MessageBox.Show(Resources.ResourceMessage.MaNhapKhongHopLe);
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show(Resources.ResourceMessage.MaNhapKhongHopLe);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void CreateThreadLoadDataByPackageService()
        //{
        //    Thread thread = new System.Threading.Thread(LoadDataByPackageServiceNewThread);
        //    thread.Priority = ThreadPriority.Normal;
        //    try
        //    {
        //        thread.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //        thread.Abort();
        //    }
        //}

        //private void LoadDataByPackageServiceNewThread()
        //{
        //    try
        //    {
        //        if (this.InvokeRequired)
        //        {
        //            this.Invoke(new MethodInvoker(delegate { LoadCurrentData(); }));
        //        }
        //        else
        //        {
        //            LoadCurrentData();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.btnAdd.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__BTN_ADD");
                this.btnCancel.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__BTN_CANCEL");
                //this.btnNew.Text = KeyLanguage(
                //    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__BTN_NEW");
                this.btnPrint.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__BTN_PRINT");
                this.btnSave.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__BTN_SAVE");
                this.btnUpdate.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__BTN_UPDATE");
                //this.chkSellByImpPrice.Text = KeyLanguage(
                //    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__CHK_SELL_BY_IMP_PRICE");
                this.gridColumn_ImpMestDetail_Amount.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_AMOUNT");
                this.gridColumn_ImpMestDetail_ExpiredDate.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_EXPIRED_DATE");
                this.gridColumn_ImpMestDetail_ImpVatRatio.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_IMP_VAT_RATIO");
                this.gridColumn_ImpMestDetail_PackageNumber.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_PACKAGE_NUMBER");
                this.gridColumn_ImpMestDetail_Price.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_PRICE");
                this.gridColumn_ImpMestDetail_Stt.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_STT");
                this.gridColumn_ImpMestDetail_TypeName.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_TYPE_NAME");
                this.gridColumn_ServicePaty_ExpPrice.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_EXP_PRICE");
                this.gridColumn_ServicePaty_NotSell.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_NOT_SELL");
                this.gridColumn_ServicePaty_PatientTypeName.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_PATINET_TYPE_NAME");
                this.gridColumn_ServicePaty_PriceAndVat.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_PRICE_VAT");
                this.gridColumn_ServicePaty_VatRatio.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_VAT_RATIO");
                this.lciAmount.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_AMOUNT");
                this.lciDescription.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_DESCRIPTION");
                this.lciDescriptionPaty.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_DESCRIPTION_PATY");
                this.lciExpiredDate.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_EXPIRED_DATE");
                this.lciImpMestType.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_IMP_MEST_TYPE");
                this.lciImpPrice.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_IMP_PRICE");
                this.lciImpSource.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_IMP_SOURCE");
                this.lciMediStock.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_MEDI_STOCK");
                this.lciPackageNumber.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_PACKAGE_NUMBER");
                this.lciPriceVat.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_PRICE_VAT");
                this.lciTotalFeePrice.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_TOTAL_FEE_PRICE");
                this.lciTotalPrice.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_TOTAL_PRICE");
                this.lciTotalVatPrice.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_TOTAL_VAT_PRICE");
                this.lciVat.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__LCI_VAT");
                this.repositoryItemButtonDelete.Buttons[0].ToolTip = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_BTN_DELETE");
                this.repositoryItemButtonEdit.Buttons[0].ToolTip = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__GC_BTN_EDIT");
                this.xtraTabPageMaterial.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TAB_MATERIAL");
                this.xtraTabPageMedicine.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_IN_IMP_MEST_EDIT__TAB_MEDICINE");
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
                SetDataSourceGridControlMediMate();

                ResetValueControlCommon();

                ResetValueControlDetail();

                LoadImpMestTypeAllow();

                LoadDataToComboImpMestType();

                LoadDataToComboMediStock();

                LoadDataToCboImpSource();

                SetDefaultImpMestType();

                //SetDefaultValueMediStock();

                SetFocuTreeMediMate();

                SetEnableButton(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSourceGridControlMediMate()
        {
            try
            {
                var listMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_LEAF == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_LEAF__TRUE && o.IS_STOP_IMP != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_STOP_IMP__TRUE).ToList();

                this.medicineProcessor.Reload(this.ucMedicineTypeTree, listMedicineType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetValueControlCommon()
        {
            try
            {
                this.resultADO = null;
                listMediStock = new List<V_HIS_MEDI_STOCK>();
                listServiceADO = new List<VHisServiceADO>();
                gridControlImpMestDetail.DataSource = null;
                cboImpMestType.EditValue = null;
                cboImpMestType.Enabled = true;
                cboImpSource.EditValue = null;
                cboMediStock.EditValue = null;
                cboMediStock.Properties.DataSource = null;
                spinImpVatRatio.Value = 0;
                btnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetValueControlDetail()
        {
            try
            {
                this.currrentServiceAdo = null;
                this.listServicePatyAdo = new List<VHisServicePatyADO>();
                spinAmount.Value = 0;
                spinImpPrice.Value = 0;
                spinImpVatRatio.Value = 0;
                //chkSellByImpPrice.Checked = false;
                dtExpiredDate.EditValue = null;
                txtExpiredDate.Text = "";
                txtPackageNumber.Text = "";
                btnAdd.Enabled = false;
                gridControlServicePaty.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadImpMestTypeAllow()
        {
            try
            {
                var listImpMestTypeId = new List<long>()
                {
                    HisImpMestTypeCFG.HisImpMestTypeId__Init,
                    //HisImpMestTypeCFG.HisImpMestTypeId__Inve,
                    //HisImpMestTypeCFG.HisImpMestTypeId__Other
                 };

                //if (HisImpMestTypeAuthorziedCFG.ImpMestType_IsAuthorized)
                //{
                //    HisImpMestTypeUserFilter impMestTypeUserFilter = new HisImpMestTypeUserFilter();
                //    impMestTypeUserFilter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //    var listData = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_IMP_MEST_TYPE_USER>>("api/HisImpMestTypeUser/Get", ApiConsumers.MosConsumer, impMestTypeUserFilter, null);
                //    if (listData != null && listData.Count > 0)
                //    {
                //        var listId = listData.Select(s => s.IMP_MEST_TYPE_ID).ToList();
                //        listImpMestTypeId = listImpMestTypeId.Where(o => listId.Contains(o)).ToList();
                //    }
                //    else
                //    {
                //        listImpMestTypeId.Clear();
                //    }
                //}
                listImpMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o => listImpMestTypeId.Contains(o.ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboImpMestType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("IMP_MEST_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("IMP_MEST_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("IMP_MEST_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboImpMestType, listImpMestType, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboMediStock()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMediStock, BackendDataWorker.Get<V_HIS_MEDI_STOCK>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboImpSource()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("IMP_SOURCE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("IMP_SOURCE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("IMP_SOURCE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboImpSource, BackendDataWorker.Get<HIS_IMP_SOURCE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultImpMestType()
        {
            try
            {
                //listMediStock = new List<V_HIS_MEDI_STOCK>();
                //this.currentImpMestType = null;
                //this.currentImpMestType = listImpMestType.FirstOrDefault(o => o.ID == currentImpMest.IMP_MEST_TYPE_ID);
                //if (this.currentImpMestType != null)
                //{
                //    cboImpMestType.EditValue = this.currentImpMestType.ID;
                //    listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_ALLOW_IMP_SUPPLIER == IMSys.DbConfig.HIS_RS.HIS_MEDI_STOCK.IS_ALLOW_IMP_SUPPLIER__TRUE).ToList();
                //}
                //cboMediStock.Properties.DataSource = listMediStock;
                cboMediStock.EditValue = currentImpMest.MEDI_STOCK_ID;
                cboImpMestType.EditValue = currentImpMest.IMP_MEST_TYPE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void SetDefaultValueMediStock()
        //{
        //    try
        //    {
        //        if (!cboMediStock.Enabled || cboMediStock.Properties.DataSource == null || currentImpMest == null)
        //            return;
        //        var medistock = listMediStock.FirstOrDefault(o => o.ID == currentImpMest.MEDI_STOCK_ID);
        //        if (medistock != null)
        //        {
        //            cboMediStock.EditValue = medistock.ID;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void SetFocuTreeMediMate()
        {
            try
            {
                if (xtraTabControl.SelectedTabPageIndex == 0)
                {
                    medicineProcessor.FocusKeyword(this.ucMedicineTypeTree);
                }
                else
                {
                    materialProcessor.FocusKeyword(this.ucMaterialTypeTree);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButton(bool enable)
        {
            try
            {
                if (enable)
                {
                    btnAdd.Visible = false;
                    btnAdd.Enabled = false;
                    //lciAdd.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    btnUpdate.Enabled = true;
                    btnCancel.Enabled = true;
                    btnUpdate.Visible = true;
                    btnCancel.Visible = true;
                }
                else
                {
                    btnAdd.Visible = true;
                    btnAdd.Enabled = true;
                    //lciAdd.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    btnUpdate.Enabled = false;
                    btnCancel.Enabled = false;
                    btnUpdate.Visible = false;
                    btnCancel.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void LoadCurrentData()
        //{
        //    try
        //    {
        //        CommonParam param = new CommonParam();
        //        HisImpMestMaterialViewFilter impMateFilter = new HisImpMestMaterialViewFilter();
        //        impMateFilter.IMP_MEST_ID = this.impMestId;
        //        var impMaterial = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, impMateFilter, param);
        //        if (impMaterial != null && impMaterial.Count > 0)
        //        {
        //            if (cboImpSource.EditValue == null)
        //            {
        //                HisMaterialFilter ma = new HisMaterialFilter();
        //                ma.ID = impMaterial.First().MATERIAL_ID;
        //                var hisMaterial = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MATERIAL>>(Base.GlobalStore.HIS_MATERIAL_GET, ApiConsumers.MosConsumer, ma, param);
        //                if (hisMaterial != null && hisMaterial.Count > 0)
        //                {
        //                    cboImpSource.EditValue = hisMaterial.FirstOrDefault().IMP_SOURCE_ID;
        //                }
        //            }

        //            foreach (var item in impMaterial)
        //            {
        //                var material = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
        //                if (material == null) continue;
        //                VHisServiceADO ado = new VHisServiceADO(material);

        //                ado.EXPIRED_DATE = item.EXPIRED_DATE;
        //                ado.IMP_AMOUNT = item.AMOUNT;
        //                ado.IMP_PRICE = item.IMP_PRICE;
        //                ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
        //                ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
        //                ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;

        //                if (!dicServicePaty.ContainsKey(item.SERVICE_ID) || dicServicePaty[item.SERVICE_ID].Count == 0)
        //                {
        //                    Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

        //                    foreach (var patient in BackendDataWorker.Get<HIS_PATIENT_TYPE>())
        //                    {
        //                        VHisServicePatyADO serviceAdo = new VHisServicePatyADO();
        //                        serviceAdo.PATIENT_TYPE_NAME = patient.PATIENT_TYPE_NAME;
        //                        serviceAdo.PATIENT_TYPE_ID = patient.ID;
        //                        serviceAdo.PATIENT_TYPE_CODE = patient.PATIENT_TYPE_CODE;
        //                        serviceAdo.IsNotSell = true;
        //                        serviceAdo.SERVICE_TYPE_ID = material.SERVICE_TYPE_ID;
        //                        serviceAdo.SERVICE_ID = material.SERVICE_ID;
        //                        dicPaty[patient.ID] = serviceAdo;
        //                    }

        //                    HisMaterialPatyFilter matePatyFilter = new HisMaterialPatyFilter();
        //                    matePatyFilter.MATERIAL_ID = item.MATERIAL_ID;
        //                    var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL_PATY>>(Base.GlobalStore.HIS_MATERIAL_PATY_GET, ApiConsumers.MosConsumer, matePatyFilter, null);//, "serviceId", material.SERVICE_ID, "treatmentTime", null);

        //                    if (listServicePaty != null && listServicePaty.Count > 0)
        //                    {
        //                        foreach (var service in listServicePaty)
        //                        {
        //                            if (dicPaty.ContainsKey(service.PATIENT_TYPE_ID))
        //                            {
        //                                var pa = dicPaty[service.PATIENT_TYPE_ID];
        //                                if (!pa.IsSetExpPrice)
        //                                {
        //                                    pa.PRICE = service.EXP_PRICE;
        //                                    pa.VAT_RATIO = service.EXP_VAT_RATIO;
        //                                    pa.ExpVatRatio = service.EXP_VAT_RATIO * 100;
        //                                    pa.ExpPriceVat = ado.IMP_PRICE * (1 + ado.IMP_VAT_RATIO);
        //                                    pa.PercentProfit = ((pa.PRICE / pa.ExpPriceVat) - 1) * 100;
        //                                    pa.IsNotSell = false;
        //                                    pa.IsNotEdit = true;
        //                                    pa.IsSetExpPrice = true;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    dicServicePaty[item.SERVICE_ID] = dicPaty.Select(s => s.Value).ToList();
        //                }
        //                var listData = dicServicePaty[item.SERVICE_ID];
        //                ado.VHisServicePatys = listData;

        //                listServiceADO.Add(ado);
        //            }
        //        }

        //        HisImpMestMedicineViewFilter impMediFilter = new HisImpMestMedicineViewFilter();
        //        impMediFilter.IMP_MEST_ID = this.impMestId;
        //        var impMedicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMediFilter, param);
        //        if (impMedicine != null && impMedicine.Count > 0)
        //        {
        //            if (cboImpSource.EditValue == null)
        //            {
        //                HisMedicineFilter ma = new HisMedicineFilter();
        //                ma.ID = impMedicine.First().MEDICINE_ID;
        //                var hisMedicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MEDICINE>>(Base.GlobalStore.HIS_MEDICINE_GET, ApiConsumers.MosConsumer, ma, param);
        //                if (hisMedicine != null && hisMedicine.Count > 0)
        //                {
        //                    cboImpSource.EditValue = hisMedicine.FirstOrDefault().IMP_SOURCE_ID;
        //                }
        //            }

        //            foreach (var item in impMedicine)
        //            {
        //                var medicine = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
        //                if (medicine == null) continue;
        //                VHisServiceADO ado = new VHisServiceADO(medicine);

        //                ado.EXPIRED_DATE = item.EXPIRED_DATE;
        //                ado.IMP_AMOUNT = item.AMOUNT;
        //                ado.IMP_PRICE = item.IMP_PRICE;
        //                ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
        //                ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
        //                ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;

        //                if (!dicServicePaty.ContainsKey(item.SERVICE_ID) || dicServicePaty[item.SERVICE_ID].Count == 0)
        //                {
        //                    Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

        //                    foreach (var patient in BackendDataWorker.Get<HIS_PATIENT_TYPE>())
        //                    {
        //                        VHisServicePatyADO serviceAdo = new VHisServicePatyADO();
        //                        serviceAdo.PATIENT_TYPE_NAME = patient.PATIENT_TYPE_NAME;
        //                        serviceAdo.PATIENT_TYPE_ID = patient.ID;
        //                        serviceAdo.PATIENT_TYPE_CODE = patient.PATIENT_TYPE_CODE;
        //                        serviceAdo.IsNotSell = true;
        //                        serviceAdo.SERVICE_TYPE_ID = medicine.SERVICE_TYPE_ID;
        //                        serviceAdo.SERVICE_ID = medicine.SERVICE_ID;
        //                        dicPaty[patient.ID] = serviceAdo;
        //                    }

        //                    HisMedicinePatyFilter matePatyFilter = new HisMedicinePatyFilter();
        //                    matePatyFilter.MEDICINE_ID = item.MEDICINE_ID;
        //                    var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE_PATY>>(Base.GlobalStore.HIS_MEDICINE_PATY_GET, ApiConsumers.MosConsumer, matePatyFilter, null);

        //                    if (listServicePaty != null && listServicePaty.Count > 0)
        //                    {
        //                        foreach (var service in listServicePaty)
        //                        {
        //                            if (dicPaty.ContainsKey(service.PATIENT_TYPE_ID))
        //                            {
        //                                var pa = dicPaty[service.PATIENT_TYPE_ID];
        //                                if (!pa.IsSetExpPrice)
        //                                {
        //                                    pa.PRICE = service.EXP_PRICE;
        //                                    pa.VAT_RATIO = service.EXP_VAT_RATIO;
        //                                    pa.ExpVatRatio = service.EXP_VAT_RATIO * 100;
        //                                    pa.ExpPriceVat = ado.IMP_PRICE * (1 + ado.IMP_VAT_RATIO);
        //                                    pa.PercentProfit = ((pa.PRICE / pa.ExpPriceVat) - 1) * 100;
        //                                    pa.IsNotSell = false;
        //                                    pa.IsNotEdit = true;
        //                                    pa.IsSetExpPrice = true;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    dicServicePaty[item.SERVICE_ID] = dicPaty.Select(s => s.Value).ToList();
        //                }
        //                var listData = dicServicePaty[item.SERVICE_ID];
        //                ado.VHisServicePatys = listData;

        //                listServiceADO.Add(ado);
        //            }
        //        }

        //        gridControlImpMestDetail.BeginUpdate();
        //        gridControlImpMestDetail.DataSource = listServiceADO;
        //        gridControlImpMestDetail.EndUpdate();
        //        CalculTotalPrice();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void CurrentDaTaLoad()
        {
            try
            {
                List<VHisServiceADO> material = new List<VHisServiceADO>();
                List<VHisServiceADO> medicines = new List<VHisServiceADO>();

                CreateThreadLoadCurrentData(material, medicines);
                listServiceADO.AddRange(material);
                listServiceADO.AddRange(medicines);

                if (cboImpSource.EditValue == null)
                {
                    if (medicines != null && medicines.Count > 0)
                    {
                        HisMedicineFilter ma = new HisMedicineFilter();
                        ma.ID = medicines.First().HisMedicine.ID;
                        var hisMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>(Base.GlobalStore.HIS_MEDICINE_GET, ApiConsumers.MosConsumer, ma, null);
                        if (hisMedicine != null && hisMedicine.Count > 0)
                        {
                            cboImpSource.EditValue = hisMedicine.FirstOrDefault().IMP_SOURCE_ID;
                        }
                    }
                    else if (material != null && material.Count > 0)
                    {
                        HisMaterialFilter ma = new HisMaterialFilter();
                        ma.ID = material.First().HisMaterial.ID;
                        var hisMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>(Base.GlobalStore.HIS_MATERIAL_GET, ApiConsumers.MosConsumer, ma, null);
                        if (hisMaterial != null && hisMaterial.Count > 0)
                        {
                            cboImpSource.EditValue = hisMaterial.FirstOrDefault().IMP_SOURCE_ID;
                        }
                    }
                }

                gridControlImpMestDetail.BeginUpdate();
                gridControlImpMestDetail.DataSource = listServiceADO;
                gridControlImpMestDetail.EndUpdate();
                CalculTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadCurrentData(List<VHisServiceADO> material, List<VHisServiceADO> medicines)
        {
            Thread threadMaterial = new System.Threading.Thread(ThreadLoadMaterialData);
            Thread threadMedicine = new System.Threading.Thread(ThreadLoadMedicineData);
            threadMaterial.Priority = ThreadPriority.Normal;
            threadMedicine.Priority = ThreadPriority.Normal;
            try
            {
                threadMaterial.Start(material);
                threadMedicine.Start(medicines);

                threadMaterial.Join();
                threadMedicine.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMaterial.Abort();
                threadMedicine.Abort();
            }
        }

        private void ThreadLoadMaterialData(object obj)
        {
            try
            {
                ThreadLoadMaterialData((List<VHisServiceADO>)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadMedicineData(object obj)
        {
            try
            {
                ThreadLoadMedicineData((List<VHisServiceADO>)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadMaterialData(List<VHisServiceADO> data)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisImpMestMaterialViewFilter impMateFilter = new HisImpMestMaterialViewFilter();
                impMateFilter.IMP_MEST_ID = this.impMestId;
                var impMaterial = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, impMateFilter, param);
                if (impMaterial != null && impMaterial.Count > 0)
                {
                    foreach (var item in impMaterial)
                    {
                        var material = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                        if (material == null) continue;
                        VHisServiceADO ado = new VHisServiceADO(material);

                        ado.EXPIRED_DATE = item.EXPIRED_DATE;
                        ado.IMP_AMOUNT = item.AMOUNT;
                        ado.IMP_PRICE = item.IMP_PRICE;
                        ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        ado.HisMaterial.ID = item.MATERIAL_ID;

                        if (!dicServicePaty.ContainsKey(item.SERVICE_ID) || dicServicePaty[item.SERVICE_ID].Count == 0)
                        {
                            Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

                            foreach (var patient in BackendDataWorker.Get<HIS_PATIENT_TYPE>())
                            {
                                VHisServicePatyADO serviceAdo = new VHisServicePatyADO();
                                serviceAdo.PATIENT_TYPE_NAME = patient.PATIENT_TYPE_NAME;
                                serviceAdo.PATIENT_TYPE_ID = patient.ID;
                                serviceAdo.PATIENT_TYPE_CODE = patient.PATIENT_TYPE_CODE;
                                serviceAdo.IsNotSell = true;
                                serviceAdo.SERVICE_TYPE_ID = material.SERVICE_TYPE_ID;
                                serviceAdo.SERVICE_ID = material.SERVICE_ID;
                                dicPaty[patient.ID] = serviceAdo;
                            }

                            HisMaterialPatyFilter matePatyFilter = new HisMaterialPatyFilter();
                            matePatyFilter.MATERIAL_ID = item.MATERIAL_ID;
                            var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL_PATY>>(Base.GlobalStore.HIS_MATERIAL_PATY_GET, ApiConsumers.MosConsumer, matePatyFilter, null);//, "serviceId", material.SERVICE_ID, "treatmentTime", null);

                            if (listServicePaty != null && listServicePaty.Count > 0)
                            {
                                foreach (var service in listServicePaty)
                                {
                                    if (dicPaty.ContainsKey(service.PATIENT_TYPE_ID))
                                    {
                                        var pa = dicPaty[service.PATIENT_TYPE_ID];
                                        if (!pa.IsSetExpPrice)
                                        {
                                            pa.ID = service.ID;
                                            pa.PRICE = service.EXP_PRICE;
                                            pa.VAT_RATIO = service.EXP_VAT_RATIO;
                                            pa.ExpVatRatio = service.EXP_VAT_RATIO * 100;
                                            pa.ExpPriceVat = ado.IMP_PRICE * (1 + ado.IMP_VAT_RATIO);
                                            pa.PercentProfit = pa.ExpPriceVat != 0 ? ((pa.PRICE / pa.ExpPriceVat) - 1) * 100 : 0;
                                            pa.IsNotSell = false;
                                            pa.IsNotEdit = true;
                                            pa.IsSetExpPrice = true;
                                        }
                                    }
                                    if (ado.HisMaterialPatys == null) ado.HisMaterialPatys = new List<HIS_MATERIAL_PATY>();
                                    ado.HisMaterialPatys.Add(service);
                                }
                            }
                            dicServicePaty[item.SERVICE_ID] = dicPaty.Select(s => s.Value).ToList();
                        }
                        var listData = dicServicePaty[item.SERVICE_ID];
                        ado.VHisServicePatys = listData;

                        data.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadMedicineData(List<VHisServiceADO> data)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisImpMestMedicineViewFilter impMediFilter = new HisImpMestMedicineViewFilter();
                impMediFilter.IMP_MEST_ID = this.impMestId;
                var impMedicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMediFilter, param);
                if (impMedicine != null && impMedicine.Count > 0)
                {
                    foreach (var item in impMedicine)
                    {
                        var medicine = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                        if (medicine == null) continue;
                        VHisServiceADO ado = new VHisServiceADO(medicine);

                        ado.EXPIRED_DATE = item.EXPIRED_DATE;
                        ado.IMP_AMOUNT = item.AMOUNT;
                        ado.IMP_PRICE = item.IMP_PRICE;
                        ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                        ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                        ado.HisMedicine.ID = item.MEDICINE_ID;

                        if (!dicServicePaty.ContainsKey(item.SERVICE_ID) || dicServicePaty[item.SERVICE_ID].Count == 0)
                        {
                            Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

                            foreach (var patient in BackendDataWorker.Get<HIS_PATIENT_TYPE>())
                            {
                                VHisServicePatyADO serviceAdo = new VHisServicePatyADO();
                                serviceAdo.PATIENT_TYPE_NAME = patient.PATIENT_TYPE_NAME;
                                serviceAdo.PATIENT_TYPE_ID = patient.ID;
                                serviceAdo.PATIENT_TYPE_CODE = patient.PATIENT_TYPE_CODE;
                                serviceAdo.IsNotSell = true;
                                serviceAdo.SERVICE_TYPE_ID = medicine.SERVICE_TYPE_ID;
                                serviceAdo.SERVICE_ID = medicine.SERVICE_ID;
                                dicPaty[patient.ID] = serviceAdo;
                            }

                            HisMedicinePatyFilter matePatyFilter = new HisMedicinePatyFilter();
                            matePatyFilter.MEDICINE_ID = item.MEDICINE_ID;
                            var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE_PATY>>(Base.GlobalStore.HIS_MEDICINE_PATY_GET, ApiConsumers.MosConsumer, matePatyFilter, null);

                            if (listServicePaty != null && listServicePaty.Count > 0)
                            {
                                foreach (var service in listServicePaty)
                                {
                                    if (dicPaty.ContainsKey(service.PATIENT_TYPE_ID))
                                    {
                                        var pa = dicPaty[service.PATIENT_TYPE_ID];
                                        if (!pa.IsSetExpPrice)
                                        {
                                            pa.ID = service.ID;
                                            pa.PRICE = service.EXP_PRICE;
                                            pa.VAT_RATIO = service.EXP_VAT_RATIO;
                                            pa.ExpVatRatio = service.EXP_VAT_RATIO * 100;
                                            pa.ExpPriceVat = ado.IMP_PRICE * (1 + ado.IMP_VAT_RATIO);
                                            pa.PercentProfit = pa.ExpPriceVat != 0 ? ((pa.PRICE / pa.ExpPriceVat) - 1) * 100 : 0;
                                            pa.IsNotSell = false;
                                            pa.IsNotEdit = true;
                                            pa.IsSetExpPrice = true;
                                        }
                                    }
                                    if (ado.HisMedicinePatys == null) ado.HisMedicinePatys = new List<HIS_MEDICINE_PATY>();
                                    ado.HisMedicinePatys.Add(service);
                                }
                            }
                            dicServicePaty[item.SERVICE_ID] = dicPaty.Select(s => s.Value).ToList();
                        }
                        var listData = dicServicePaty[item.SERVICE_ID];
                        ado.VHisServicePatys = listData;

                        data.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Validate
        private void ValidControls()
        {
            try
            {
                ValidControlImpMestType();
                ValidControlMediStock();
                ValidControlImpAmount();
                ValidControlImPrice();
                ValidControlImpVatRatio();
                ValidControlExpiredDate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImpMestType()
        {
            try
            {
                ImpMestTypeValidationRule impMestTypeRule = new ImpMestTypeValidationRule();
                impMestTypeRule.cboImpMestType = cboImpMestType;
                dxValidationProvider1.SetValidationRule(cboImpMestType, impMestTypeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlMediStock()
        {
            try
            {
                MediStockValidationRule mediStockRule = new MediStockValidationRule();
                mediStockRule.cboMediStock = cboMediStock;
                dxValidationProvider1.SetValidationRule(cboMediStock, mediStockRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImpAmount()
        {
            try
            {
                ImpAmountValidationRule impAmountRule = new ImpAmountValidationRule();
                impAmountRule.spinImpAmount = spinAmount;
                dxValidationProvider2.SetValidationRule(spinAmount, impAmountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImPrice()
        {
            try
            {
                ImpPriceValidationRule impPriceRule = new ImpPriceValidationRule();
                impPriceRule.spinImpPrice = spinImpPrice;
                dxValidationProvider2.SetValidationRule(spinImpPrice, impPriceRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImpVatRatio()
        {
            try
            {
                ImpVatRatioValidationRule impVatRule = new ImpVatRatioValidationRule();
                impVatRule.spinImpVatRatio = spinImpVatRatio;
                dxValidationProvider2.SetValidationRule(spinImpVatRatio, impVatRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpiredDate()
        {
            try
            {
                ExpiredDateValidationRule expiredDateRule = new ExpiredDateValidationRule();
                expiredDateRule.txtExpiredDate = txtExpiredDate;
                expiredDateRule.dtExpiredDate = dtExpiredDate;
                dxValidationProvider2.SetValidationRule(txtExpiredDate, expiredDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region Event
        private string KeyLanguage(string key)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Resource.Get.Value(
                    key,
                    Resources.ResourceLanguageManager.LanguageFormInImpMestEdit,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void LoadServicePatyByAdo()
        {
            try
            {
                if (this.currrentServiceAdo != null)//&& CheckAllowAdd())
                {
                    spinAmount.Value = 0;
                    btnAdd.Enabled = true;
                    spinImpPrice.Value = currrentServiceAdo.IMP_PRICE;
                    spinImpVatRatio.Value = currrentServiceAdo.IMP_VAT_RATIO * 100;
                }
                else
                {
                    spinAmount.Value = 0;
                    spinImpPrice.Value = 0;
                    spinImpVatRatio.Value = 0;
                    btnAdd.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueByServiceAdo()
        {
            try
            {
                if (this.currrentServiceAdo != null)// && CheckAllowAdd())
                {
                    if (!dicServicePaty.ContainsKey(this.currrentServiceAdo.SERVICE_ID) || dicServicePaty[this.currrentServiceAdo.SERVICE_ID].Count == 0)
                    {
                        Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

                        var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_PATY>>(HisRequestUriStore.HIS_SERVICE_PATY_GET_APPLIED_VIEW, ApiConsumers.MosConsumer, null, null, "serviceId", this.currrentServiceAdo.SERVICE_ID, "treatmentTime", null);

                        foreach (var item in BackendDataWorker.Get<HIS_PATIENT_TYPE>())
                        {
                            VHisServicePatyADO ado = new VHisServicePatyADO();
                            ado.PATIENT_TYPE_NAME = item.PATIENT_TYPE_NAME;
                            ado.PATIENT_TYPE_ID = item.ID;
                            ado.PATIENT_TYPE_CODE = item.PATIENT_TYPE_CODE;
                            ado.IsNotSell = true;
                            ado.SERVICE_TYPE_ID = this.currrentServiceAdo.SERVICE_TYPE_ID;
                            ado.SERVICE_ID = this.currrentServiceAdo.SERVICE_ID;
                            dicPaty[item.ID] = ado;
                        }

                        if (listServicePaty != null && listServicePaty.Count > 0)
                        {
                            foreach (var item in listServicePaty)
                            {
                                if (dicPaty.ContainsKey(item.PATIENT_TYPE_ID))
                                {
                                    var ado = dicPaty[item.PATIENT_TYPE_ID];
                                    if (!ado.IsSetExpPrice)
                                    {
                                        ado.PRICE = item.PRICE;
                                        ado.VAT_RATIO = item.VAT_RATIO;
                                        ado.ExpVatRatio = item.VAT_RATIO * 100;
                                        ado.IsNotSell = false;
                                        ado.IsNotEdit = true;
                                        ado.IsSetExpPrice = true;
                                    }
                                }
                            }
                        }
                        dicServicePaty[this.currrentServiceAdo.SERVICE_ID] = dicPaty.Select(s => s.Value).ToList();
                    }
                    var listData = dicServicePaty[this.currrentServiceAdo.SERVICE_ID];
                    AutoMapper.Mapper.CreateMap<VHisServicePatyADO, VHisServicePatyADO>();
                    listServicePatyAdo = AutoMapper.Mapper.Map<List<VHisServicePatyADO>>(listData);
                }
                else
                {
                    listServicePatyAdo.Clear();
                }
                gridControlServicePaty.BeginUpdate();
                gridControlServicePaty.DataSource = listServicePatyAdo;
                gridControlServicePaty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalculTotalPrice()
        {
            try
            {
                decimal totalprice = 0;
                decimal totalfeePrice = 0;
                decimal totalvatPrice = 0;
                if (listServiceADO != null && listServiceADO.Count > 0)
                {
                    totalfeePrice = listServiceADO.Sum(s => (s.IMP_AMOUNT * s.IMP_PRICE));
                    totalvatPrice = Math.Round(listServiceADO.Sum(s => (s.IMP_AMOUNT * s.IMP_PRICE * s.IMP_VAT_RATIO)), 0);
                    totalprice = totalfeePrice + totalvatPrice;
                }
                lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalprice, 0);
                lblTotalFeePrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalfeePrice, 0);
                lblTotalVatPrice.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalvatPrice, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckAllowAdd(ref string messageError)
        {
            try
            {
                if (cboImpMestType.EditValue == null)
                {
                    messageError = Resources.ResourceMessage.NguoiDungChuaChonLoaiNhap;
                    return false;
                }

                if (cboMediStock.EditValue == null)
                {
                    messageError = Resources.ResourceMessage.NguoiDungChuaChonKhoNhap;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void SetEnableControlCommon()
        {
            try
            {
                bool enable = (listServiceADO.Count > 0);
                cboImpMestType.Enabled = !enable;
                cboMediStock.Enabled = !enable;
                cboImpSource.Enabled = !enable;
                txtDescription.Enabled = !enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddData(ref VHisServiceADO vHisServiceADO)
        {
            try
            {
                vHisServiceADO.IMP_AMOUNT = spinAmount.Value;
                vHisServiceADO.IMP_PRICE = spinImpPrice.Value;
                vHisServiceADO.IMP_VAT_RATIO = spinImpVatRatio.Value / 100;
                vHisServiceADO.ImpVatRatio = spinImpVatRatio.Value;
                if (dtExpiredDate.EditValue != null && dtExpiredDate.DateTime != DateTime.MinValue)
                {
                    vHisServiceADO.EXPIRED_DATE = Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "235959");
                }
                else
                {
                    vHisServiceADO.EXPIRED_DATE = null;
                }

                if (!String.IsNullOrWhiteSpace(txtPackageNumber.Text))
                {
                    vHisServiceADO.PACKAGE_NUMBER = txtPackageNumber.Text;
                }
                else
                {
                    vHisServiceADO.PACKAGE_NUMBER = null;
                }

                AutoMapper.Mapper.CreateMap<VHisServicePatyADO, VHisServicePatyADO>();
                vHisServiceADO.VHisServicePatys = AutoMapper.Mapper.Map<List<VHisServicePatyADO>>(listServicePatyAdo);

                if (vHisServiceADO.IsMedicine)
                {
                    vHisServiceADO.HisMedicine.AMOUNT = vHisServiceADO.IMP_AMOUNT;
                    vHisServiceADO.HisMedicine.IMP_PRICE = vHisServiceADO.IMP_PRICE;
                    vHisServiceADO.HisMedicine.IMP_VAT_RATIO = vHisServiceADO.IMP_VAT_RATIO;
                    vHisServiceADO.HisMedicine.PACKAGE_NUMBER = vHisServiceADO.PACKAGE_NUMBER;
                    vHisServiceADO.HisMedicine.EXPIRED_DATE = vHisServiceADO.EXPIRED_DATE;
                    if (vHisServiceADO.HisMedicinePatys == null)
                    {
                        vHisServiceADO.HisMedicinePatys = new List<HIS_MEDICINE_PATY>();
                    }

                    foreach (var paty in vHisServiceADO.VHisServicePatys)
                    {
                        if (!paty.IsNotSell)
                        {
                            var mediPaty = vHisServiceADO.HisMedicinePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == paty.PATIENT_TYPE_ID);
                            if (mediPaty != null)
                            {
                                mediPaty.EXP_PRICE = paty.PRICE;
                                mediPaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                            }
                            else
                            {
                                mediPaty = new HIS_MEDICINE_PATY();
                                mediPaty.PATIENT_TYPE_ID = paty.PATIENT_TYPE_ID;
                                mediPaty.EXP_PRICE = paty.PRICE;
                                mediPaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                vHisServiceADO.HisMedicinePatys.Add(mediPaty);
                            }
                        }
                    }
                }
                else
                {
                    vHisServiceADO.HisMaterial.AMOUNT = vHisServiceADO.IMP_AMOUNT;
                    vHisServiceADO.HisMaterial.IMP_PRICE = vHisServiceADO.IMP_PRICE;
                    vHisServiceADO.HisMaterial.IMP_VAT_RATIO = vHisServiceADO.IMP_VAT_RATIO;
                    vHisServiceADO.HisMaterial.PACKAGE_NUMBER = vHisServiceADO.PACKAGE_NUMBER;
                    vHisServiceADO.HisMaterial.EXPIRED_DATE = vHisServiceADO.EXPIRED_DATE;
                    if (vHisServiceADO.HisMaterialPatys == null)
                    {
                        vHisServiceADO.HisMaterialPatys = new List<HIS_MATERIAL_PATY>();
                    }

                    foreach (var paty in vHisServiceADO.VHisServicePatys)
                    {
                        if (!paty.IsNotSell)
                        {
                            var matePaty = vHisServiceADO.HisMaterialPatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == paty.PATIENT_TYPE_ID);
                            if (matePaty != null)
                            {
                                matePaty.EXP_PRICE = paty.PRICE;
                                matePaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                            }
                            else
                            {
                                matePaty = new HIS_MATERIAL_PATY();
                                matePaty.PATIENT_TYPE_ID = paty.PATIENT_TYPE_ID;
                                matePaty.EXP_PRICE = paty.PRICE;
                                matePaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                vHisServiceADO.HisMaterialPatys.Add(matePaty);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private object getImpMestTypeSDORequest(HIS_IMP_MEST_TYPE impMestType, long impMestSttId)
        {
            try
            {
                List<HisMaterialWithPatySDO> hisMaterialSdos = new List<HisMaterialWithPatySDO>();
                List<HisMedicineWithPatySDO> hisMedicineSdos = new List<HisMedicineWithPatySDO>();
                long? impSourceId = null;
                if (cboImpSource.EditValue != null)
                {
                    var impSource = BackendDataWorker.Get<HIS_IMP_SOURCE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpSource.EditValue));
                    if (impSource != null)
                    {
                        impSourceId = impSource.ID;
                    }
                }

                //long mediStockId = 0;
                //if (cboMediStock.EditValue != null)
                //{
                //    var mediStock = listMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboMediStock.EditValue));
                //    if (mediStock != null)
                //    {
                //        mediStockId = mediStock.ID;
                //    }
                //}

                foreach (var ado in listServiceADO)
                {
                    if (ado.IsMedicine)
                    {
                        HisMedicineWithPatySDO mediSdo = new HisMedicineWithPatySDO();
                        mediSdo.Medicine = ado.HisMedicine;

                        mediSdo.Medicine.AMOUNT = ado.IMP_AMOUNT;
                        mediSdo.Medicine.IMP_PRICE = ado.IMP_PRICE;
                        mediSdo.Medicine.IMP_VAT_RATIO = ado.IMP_VAT_RATIO;
                        mediSdo.Medicine.IMP_SOURCE_ID = impSourceId;
                        mediSdo.Medicine.PACKAGE_NUMBER = ado.PACKAGE_NUMBER;
                        mediSdo.Medicine.EXPIRED_DATE = ado.EXPIRED_DATE;
                        mediSdo.MedicinePaties = ado.HisMedicinePatys;
                        hisMedicineSdos.Add(mediSdo);
                    }
                    else
                    {
                        HisMaterialWithPatySDO mateSdo = new HisMaterialWithPatySDO();
                        mateSdo.Material = ado.HisMaterial;
                        mateSdo.Material.AMOUNT = ado.IMP_AMOUNT;
                        mateSdo.Material.IMP_PRICE = ado.IMP_PRICE;
                        mateSdo.Material.IMP_VAT_RATIO = ado.IMP_VAT_RATIO;
                        mateSdo.Material.IMP_SOURCE_ID = impSourceId;
                        mateSdo.Material.PACKAGE_NUMBER = ado.PACKAGE_NUMBER;
                        mateSdo.Material.EXPIRED_DATE = ado.EXPIRED_DATE;
                        mateSdo.MaterialPaties = ado.HisMaterialPatys;
                        hisMaterialSdos.Add(mateSdo);
                    }
                }

                if (impMestType.ID == HisImpMestTypeCFG.HisImpMestTypeId__Init)
                {
                    HisInitImpMestSDO initSdo = new HisInitImpMestSDO();
                    if (resultADO != null)
                    {
                        initSdo = resultADO.HisInitSDO;
                    }
                    else
                    {
                        initSdo.ImpMest = currentImpMest;
                        initSdo.InitImpMest = currentInitImpMest;
                    }
                    initSdo.ImpMest.REQ_ROOM_ID = currentImpMest.REQ_ROOM_ID;
                    initSdo.ImpMest.MEDI_STOCK_ID = currentImpMest.MEDI_STOCK_ID;
                    initSdo.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    initSdo.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    initSdo.ImpMest.DESCRIPTION = txtDescription.Text;
                    initSdo.Materials = hisMaterialSdos;
                    initSdo.Medicines = hisMedicineSdos;
                    return initSdo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private void ProcessSaveSuccess()
        {
            try
            {
                if (this.resultADO != null)
                {
                    cboImpMestType.EditValue = this.resultADO.ImpMestTypeId;
                    cboImpMestType.Enabled = false;
                    if (this.resultADO.HisMedicineSDOs != null && this.resultADO.HisMedicineSDOs.Count > 0)
                    {
                        foreach (var item in this.resultADO.HisMedicineSDOs)
                        {
                            var ado = listServiceADO.FirstOrDefault(o => o.IsMedicine && o.MEDI_MATE_ID == item.Medicine.MEDICINE_TYPE_ID && o.IMP_AMOUNT == item.Medicine.AMOUNT && o.IMP_PRICE == item.Medicine.IMP_PRICE && o.IMP_VAT_RATIO == item.Medicine.IMP_VAT_RATIO && o.PACKAGE_NUMBER == item.Medicine.PACKAGE_NUMBER && o.EXPIRED_DATE == item.Medicine.EXPIRED_DATE);
                            if (ado != null)
                            {
                                ado.HisMedicine = item.Medicine;
                                ado.HisMedicinePatys = item.MedicinePaties;
                                foreach (var mediPaty in ado.HisMedicinePatys)
                                {
                                    var patyAdo = ado.VHisServicePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == mediPaty.PATIENT_TYPE_ID);
                                    if (patyAdo != null)
                                    {
                                        patyAdo.PRICE = mediPaty.EXP_PRICE;
                                        patyAdo.VAT_RATIO = mediPaty.EXP_VAT_RATIO;
                                        patyAdo.ExpVatRatio = mediPaty.EXP_VAT_RATIO * 100;
                                    }
                                }
                            }
                        }
                    }

                    if (this.resultADO.HisMaterialSDOs != null && this.resultADO.HisMaterialSDOs.Count > 0)
                    {
                        foreach (var item in this.resultADO.HisMaterialSDOs)
                        {
                            var ado = listServiceADO.FirstOrDefault(o => !o.IsMedicine && o.MEDI_MATE_ID == item.Material.MATERIAL_TYPE_ID && o.IMP_AMOUNT == item.Material.AMOUNT && o.IMP_PRICE == item.Material.IMP_PRICE && o.IMP_VAT_RATIO == item.Material.IMP_VAT_RATIO && o.PACKAGE_NUMBER == item.Material.PACKAGE_NUMBER && o.EXPIRED_DATE == item.Material.EXPIRED_DATE);
                            if (ado != null)
                            {
                                ado.HisMaterial = item.Material;
                                ado.HisMaterialPatys = item.MaterialPaties;
                                foreach (var matePaty in ado.HisMaterialPatys)
                                {
                                    var patyAdo = ado.VHisServicePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == matePaty.PATIENT_TYPE_ID);
                                    if (patyAdo != null)
                                    {
                                        patyAdo.PRICE = matePaty.EXP_PRICE;
                                        patyAdo.VAT_RATIO = matePaty.EXP_VAT_RATIO;
                                        patyAdo.ExpVatRatio = matePaty.EXP_VAT_RATIO * 100;
                                    }
                                }
                            }
                        }
                    }
                    gridControlImpMestDetail.BeginUpdate();
                    gridControlImpMestDetail.DataSource = listServiceADO;
                    gridControlImpMestDetail.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckData(CommonParam param)
        {
            bool valid = false;
            try
            {
                if (listServiceADO != null && listServiceADO.Count > 0)
                {
                    foreach (var item in listServiceADO)
                    {
                        string messageErr = "";
                        bool result = false;
                        if (item.IsMedicine == true)
                        {
                            messageErr = String.Format(Resources.ResourceMessage.CanhBaoThuoc, item.MEDI_MATE_NAME);
                        }
                        else
                        {
                            messageErr = String.Format(Resources.ResourceMessage.CanhBaoVatTu, item.MEDI_MATE_NAME);
                        }

                        if (item.IMP_AMOUNT <= 0)
                        {
                            result = true;
                            messageErr += Resources.ResourceMessage.SoLuongKhongDuocNhoHon0;
                        }

                        if (result)
                        {
                            param.Messages.Add(messageErr + "; ");
                        }
                    }
                }
                else
                {
                    param.Messages.Add(Resources.ResourceMessage.KhongCoThuocVatTu);
                }

                if (param.Messages.Count > 0)
                {
                    valid = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                valid = true;
            }
            return valid;
        }
        #endregion

        #region Action
        #region other
        private void medicineTypeTree_Click(UC.MedicineType.ADO.MedicineTypeADO data)
        {
            try
            {
                WaitingManager.Show();
                this.currrentServiceAdo = null;
                if (data != null)
                {
                    this.currrentServiceAdo = new ADO.VHisServiceADO((V_HIS_MEDICINE_TYPE)data);
                }
                SetValueByServiceAdo();
                LoadServicePatyByAdo();
                spinAmount.Focus();
                spinAmount.SelectAll();
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
                this.currrentServiceAdo = null;
                if (data != null)
                {
                    this.currrentServiceAdo = new ADO.VHisServiceADO((V_HIS_MATERIAL_TYPE)data);
                }
                SetValueByServiceAdo();
                LoadServicePatyByAdo();
                spinAmount.Focus();
                spinAmount.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider2_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region left
        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpPrice.Focus();
                    spinImpPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinImpVatRatio.Focus();
                    spinImpVatRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPrice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spinImpPriceVAT.Value = (spinImpPrice.Value * (1 + spinImpVatRatio.Value / 100));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpVatRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExpiredDate.Focus();
                    txtExpiredDate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpVatRatio_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spinImpPriceVAT.Value = (spinImpPrice.Value * (1 + spinImpVatRatio.Value / 100));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Hạn Sử Dụng
        private void txtExpiredDate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtExpiredDate.EditValue = dt;
                        dtExpiredDate.Update();
                        //chkSellByImpPrice.Focus();
                    }
                    //else
                    //{
                    dtExpiredDate.Visible = true;
                    dtExpiredDate.Focus();
                    dtExpiredDate.ShowPopup();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = Resources.ResourceMessage.NguoiDungNhapNgayKhongHopLe;
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpiredDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                    //chkSellByImpPrice.Focus();
                    txtPackageNumber.Focus();
                    txtPackageNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_Leave(object sender, EventArgs e)
        {
            try
            {
                dtExpiredDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                //chkSellByImpPrice.Focus();
                txtPackageNumber.Focus();
                txtPackageNumber.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpiredDate.Text))
                    {
                        dtExpiredDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                        //chkSellByImpPrice.Focus();
                        txtPackageNumber.Focus();
                        txtPackageNumber.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtExpiredDate.EditValue = dt;
                        dtExpiredDate.Update();
                        //chkSellByImpPrice.Focus();
                    }
                    //else
                    //{
                    dtExpiredDate.Visible = true;
                    dtExpiredDate.Focus();
                    dtExpiredDate.ShowPopup();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();

                if (!String.IsNullOrEmpty(currentValue))
                {
                    int day = Int16.Parse(currentValue.Substring(0, 2));
                    int month = Int16.Parse(currentValue.Substring(3, 2));
                    int year = Int16.Parse(currentValue.Substring(6, 4));
                    if (day < 0 || day > 31 || month < 0 || month > 12 || year < 1000 || year > DateTime.Now.Year)
                    {
                        //e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpiredDate_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtExpiredDate.Visible = false;
                    txtExpiredDate.Text = dtExpiredDate.DateTime == DateTime.MinValue ? null : dtExpiredDate.DateTime.ToString("dd/MM/yyyy");
                    //chkSellByImpPrice.Focus();
                    txtPackageNumber.Focus();
                    txtPackageNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpiredDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpiredDate.Visible = false;
                    dtExpiredDate.Update();
                    txtExpiredDate.Text = dtExpiredDate.DateTime == DateTime.MinValue ? null : dtExpiredDate.DateTime.ToString("dd/MM/yyyy");
                    //chkSellByImpPrice.Focus();
                    txtPackageNumber.Focus();
                    txtPackageNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        //private void chkSellByImpPrice_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (chkSellByImpPrice.Checked)
        //        {
        //            if (listServicePatyAdo != null && listServicePatyAdo.Count > 0)
        //            {
        //                foreach (var item in listServicePatyAdo)
        //                {
        //                    if (item.IsNotSell)
        //                        continue;
        //                    item.PRICE = spinImpPrice.Value;
        //                    item.ExpVatRatio = spinImpVatRatio.Value;
        //                    item.VAT_RATIO = item.ExpVatRatio / 100;
        //                }
        //                gridControlServicePaty.RefreshDataSource();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void chkSellByImpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            txtPackageNumber.Focus();
        //            txtPackageNumber.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void txtPackageNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServicePaty_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;

                var data = (VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (e.Column.FieldName == "ExpVatRatio")
                    {
                        data.VAT_RATIO = data.ExpVatRatio / 100;
                        data.ExpPriceVat = data.ExpPriceVat * (1 + data.VAT_RATIO);
                        data.PRICE = (1 + data.PercentProfit / 100) * data.ExpPriceVat;
                        //data.PRICE = data.ExpPriceVat * (1 + data.PercentProfit / 100);
                    }
                    else if (e.Column.FieldName == "PercentProfit")
                    {
                        data.PRICE = (1 + data.PercentProfit / 100) * data.ExpPriceVat;
                    }
                    else if (e.Column.FieldName == "PRICE")
                    {
                        if (data.PRICE < data.ExpPriceVat)
                        {
                            data.PRICE = data.ExpPriceVat;
                        }
                        if (data.ExpPriceVat > 0)
                        {
                            data.PercentProfit = 100 * (data.PRICE - data.ExpPriceVat) / data.ExpPriceVat;
                        }
                        else
                        {
                            data.PercentProfit = 0;
                        }
                    }
                    else if (e.Column.FieldName == "IsNotSell")
                    {
                        gridControlServicePaty.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServicePaty_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "PercentProfit")
                        {
                            try
                            {
                                if (data.IsNotSell)
                                {
                                    e.RepositoryItem = repositoryItemSpinPercentProfitD;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemSpinPercentProfitE;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "ExpVatRatio")
                        {
                            try
                            {
                                if (data.IsNotSell)
                                {
                                    e.RepositoryItem = repositoryItemSpinExpVatRatioDisable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemSpinExpVatRatio;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }

                        else if (e.Column.FieldName == "PRICE")
                        {
                            try
                            {
                                if (data.IsNotSell)
                                {
                                    e.RepositoryItem = repositoryItemSpinExpPriceD;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemSpinExpPriceE;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IsNotSell")
                        {
                            try
                            {
                                if (data.IsNotEdit)
                                {
                                    e.RepositoryItem = repositoryItemCheckIsNotSellDisable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemCheckIsNotSell;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServicePaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            //try
            //{
            //    if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
            //    {
            //        var data = (VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
            //        if (data != null)
            //        {
            //            if (e.Column.FieldName == "EXP_PRICE_VAT")
            //            {
            //                try
            //                {
            //                    e.Value = data.PRICE * (1 + data.VAT_RATIO);
            //                }
            //                catch (Exception ex)
            //                {
            //                    Inventec.Common.Logging.LogSystem.Error(ex);
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void gridViewServicePaty_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsNotSell)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraTabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (xtraTabControl.SelectedTabPageIndex == 1)
                {
                    if (!isload)
                    {
                        WaitingManager.Show();
                        var listMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_LEAF == IMSys.DbConfig.HIS_RS.HIS_MATERIAL_TYPE.IS_LEAF__TRUE && o.IS_STOP_IMP != IMSys.DbConfig.HIS_RS.HIS_MATERIAL_TYPE.IS_STOP_IMP__TRUE).ToList();

                        this.materialProcessor.Reload(this.ucMaterialTypeTree, listMaterialType);
                        isload = true;
                        WaitingManager.Hide();
                    }
                }
                SetFocuTreeMediMate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region right
        private void cboImpMestType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(cboImpMestType.Text))
                    {
                        this.currentImpMestType = null;
                        listMediStock = new List<V_HIS_MEDI_STOCK>();
                        var key = cboImpMestType.Text.ToLower();
                        var listData = listImpMestType.Where(o => o.IMP_MEST_TYPE_CODE.ToLower().Contains(key) || o.IMP_MEST_TYPE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                            this.currentImpMestType = listData.First();
                            cboImpMestType.EditValue = this.currentImpMestType.ID;
                        }
                    }

                    if (this.currentImpMestType != null)
                    {
                        if (this.currentImpMestType.ID == HisImpMestTypeCFG.HisImpMestTypeId__Manu)
                        {
                            listMediStock = listMediStock.Where(o => o.IS_ALLOW_IMP_SUPPLIER == IMSys.DbConfig.HIS_RS.HIS_MEDI_STOCK.IS_ALLOW_IMP_SUPPLIER__TRUE).ToList();
                        }
                        cboMediStock.Properties.DataSource = listMediStock;
                        cboMediStock.Focus();
                        cboMediStock.ShowPopup();
                    }
                    else
                    {
                        cboMediStock.Properties.DataSource = listMediStock;
                        cboMediStock.Focus();
                        cboMediStock.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMediStock.Properties.DataSource == null)
                    {
                        cboImpSource.Focus();
                        cboImpSource.ShowPopup();
                    }
                    else
                    {
                        bool valid = false;
                        if (!String.IsNullOrEmpty(cboMediStock.Text))
                        {
                            var key = cboMediStock.Text.ToLower();
                            var listData = listMediStock.Where(o => o.MEDI_STOCK_CODE.ToLower().Contains(key) || o.MEDI_STOCK_NAME.ToLower().Contains(key)).ToList();
                            if (listData != null && listData.Count == 1)
                            {
                                valid = true;
                                cboMediStock.EditValue = listData.First().ID;
                                cboImpSource.Focus();
                                cboImpSource.ShowPopup();
                            }
                        }
                        if (!valid)
                        {
                            cboMediStock.Focus();
                            cboMediStock.ShowPopup();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboMediStock.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    cboImpSource.Focus();
                    cboImpSource.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpSource_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpSource_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SetFocuTreeMediMate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestDetail_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                {
                    return;
                }
                var data = (VHisServiceADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (e.Column.FieldName == "IMP_PRICE")
                    {
                        if (data.IsMedicine)
                        {
                            data.HisMedicine.IMP_PRICE = data.IMP_PRICE;
                        }
                        else
                        {
                            data.HisMaterial.IMP_PRICE = data.IMP_PRICE;
                        }
                    }
                    else if (e.Column.FieldName == "ImpVatRatio")
                    {
                        data.IMP_VAT_RATIO = data.ImpVatRatio / 100;
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        if (gridViewImpMestDetail.EditingValue is DateTime)
                        {
                            var dt = (DateTime)gridViewImpMestDetail.EditingValue;
                            if (dt == null || dt == DateTime.MinValue)
                            {
                                data.EXPIRED_DATE = null;
                            }
                            else
                            {
                                data.EXPIRED_DATE = Convert.ToInt64(dt.ToString("yyyyMMdd") + "235959");
                            }
                        }
                        else
                        {
                            data.EXPIRED_DATE = null;
                        }

                        if (data.IsMedicine)
                        {
                            data.HisMedicine.EXPIRED_DATE = data.EXPIRED_DATE;
                        }
                        else
                        {
                            data.HisMaterial.EXPIRED_DATE = data.EXPIRED_DATE;
                        }
                    }
                    else if (e.Column.FieldName == "PACKAGE_NUMBER")
                    {
                        if (data.IsMedicine)
                        {
                            data.HisMedicine.PACKAGE_NUMBER = data.PACKAGE_NUMBER;
                        }
                        else
                        {
                            data.HisMaterial.PACKAGE_NUMBER = data.PACKAGE_NUMBER;
                        }
                    }
                    gridControlImpMestDetail.RefreshDataSource();
                    CalculTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisServiceADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region click
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Focus();
                if (!btnAdd.Enabled)
                    return;
                string messageError = "";
                if (!CheckAllowAdd(ref messageError))
                {
                    MessageManager.Show(messageError);
                    return;
                }

                positionHandleControl = -1;
                if (!dxValidationProvider2.Validate() || this.currrentServiceAdo == null)
                    return;

                WaitingManager.Show();

                AddData(ref this.currrentServiceAdo);

                listServiceADO.Add(this.currrentServiceAdo);
                //dicServiceAdo[this.currrentServiceAdo.SERVICE_ID] = this.currrentServiceAdo;
                gridControlImpMestDetail.BeginUpdate();
                gridControlImpMestDetail.DataSource = listServiceADO;
                gridControlImpMestDetail.EndUpdate();
                CalculTotalPrice();
                SetEnableControlCommon();
                ResetValueControlDetail();
                SetEnableButton(false);
                SetFocuTreeMediMate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                btnUpdate.Focus();
                positionHandleControl = -1;
                if (!btnUpdate.Enabled || !dxValidationProvider2.Validate() || this.currrentServiceAdo == null)
                    return;

                WaitingManager.Show();

                AddData(ref this.currrentServiceAdo);

                //dicServiceAdo[this.currrentServiceAdo.SERVICE_ID] = this.currrentServiceAdo;
                gridControlImpMestDetail.BeginUpdate();
                gridControlImpMestDetail.DataSource = listServiceADO;
                gridControlImpMestDetail.EndUpdate();
                CalculTotalPrice();
                ResetValueControlDetail();
                SetEnableButton(false);
                SetFocuTreeMediMate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCancel.Enabled)
                    return;
                SetEnableButton(false);
                ResetValueControlDetail();
                SetFocuTreeMediMate();
                SetEnableButton(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Focus();
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.listServiceADO.Count <= 0)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                if (CheckData(param))
                {
                    MessageManager.Show(this.ParentForm, param, success);
                    return;
                }

                HIS_IMP_MEST_TYPE impMestType = null;
                if (cboImpMestType.EditValue != null)
                {
                    impMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMestType.EditValue));
                }
                if (impMestType != null)
                {
                    var sdo = getImpMestTypeSDORequest(impMestType, HisImpMestSttCFG.HisImpMestSttId__Request);
                    if (sdo != null)
                    {
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisInitImpMestSDO>(HisRequestUriStore.HIS_INIT_IMP_MEST_UPDATE, ApiConsumers.MosConsumer, sdo, param);

                        if (rs != null)
                        {
                            success = true;
                            this.resultADO = new ResultImpMestADO(rs);
                            this.ProcessSaveSuccess();
                            btnPrint.Enabled = true;
                            btnSave.Enabled = false;
                        }
                    }
                }
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this.ParentForm, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!btnPrint.Enabled || this.resultADO == null)
                return;
            onClickBienBanKiemNhapTuNcc(null, null);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!btnNew.Enabled)
                //    return;
                //WaitingManager.Show();
                //ResetValueControlDetail();
                //ResetValueControlCommon();
                //SetEnableControlCommon();
                //SetDefaultImpMestType();
                //SetDefaultValueMediStock();
                //SetFocuTreeMediMate();
                //cboImpMestType.Focus();
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                this.currrentServiceAdo = (VHisServiceADO)gridViewImpMestDetail.GetFocusedRow();
                if (this.currrentServiceAdo != null)
                {
                    SetEnableButton(true);
                    spinAmount.Value = currrentServiceAdo.IMP_AMOUNT;
                    spinImpPrice.Value = currrentServiceAdo.IMP_PRICE;
                    spinImpVatRatio.Value = currrentServiceAdo.ImpVatRatio;
                    if (currrentServiceAdo.EXPIRED_DATE.HasValue)
                    {
                        dtExpiredDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currrentServiceAdo.EXPIRED_DATE.Value) ?? DateTime.MinValue;
                        txtExpiredDate.Text = dtExpiredDate.DateTime.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        dtExpiredDate.EditValue = null;
                        txtExpiredDate.Text = "";
                    }
                    txtPackageNumber.Text = currrentServiceAdo.PACKAGE_NUMBER;
                    AutoMapper.Mapper.CreateMap<VHisServicePatyADO, VHisServicePatyADO>();
                    listServicePatyAdo = AutoMapper.Mapper.Map<List<VHisServicePatyADO>>(currrentServiceAdo.VHisServicePatys);

                    //bool sellByImpPrice = false;
                    //if (listServicePatyAdo != null)
                    //{
                    //    foreach (var item in listServicePatyAdo)
                    //    {
                    //        if (!item.IsNotSell)
                    //        {
                    //            if (item.PRICE == currrentServiceAdo.IMP_PRICE && item.VAT_RATIO == currrentServiceAdo.IMP_VAT_RATIO)
                    //            {
                    //                sellByImpPrice = true;
                    //            }
                    //            else
                    //            {
                    //                sellByImpPrice = false;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}
                    //chkSellByImpPrice.Checked = sellByImpPrice;

                    gridControlServicePaty.BeginUpdate();
                    gridControlServicePaty.DataSource = listServicePatyAdo;
                    gridControlServicePaty.EndUpdate();
                    spinAmount.Focus();
                    spinAmount.SelectAll();
                }
                else
                {
                    SetEnableButton(false);
                    SetValueByServiceAdo();
                    LoadServicePatyByAdo();
                    SetFocuTreeMediMate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (VHisServiceADO)gridViewImpMestDetail.GetFocusedRow();
                listServiceADO.Remove(data);
                gridControlImpMestDetail.BeginUpdate();
                gridControlImpMestDetail.DataSource = listServiceADO;
                gridControlImpMestDetail.EndUpdate();
                CalculTotalPrice();
                SetEnableControlCommon();
                if (listServiceADO.Count == 0)
                {
                    this.currentImpMestType = null;
                    this.SetValueByServiceAdo();
                    this.LoadServicePatyByAdo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region print
        private void onClickBienBanKiemNhapTuNcc(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                if (this.resultADO == null)
                    return;
                store.RunPrintTemplate(MPS.Processor.Mps000170.PDO.Mps000170PDO.printTypeCode, InBienBanNhap);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool InBienBanNhap(string printTypeCode, string fileName)
        {
            bool result = true;
            try
            {
                WaitingManager.Show();
                if (resultADO.ImpMestTypeId == HisImpMestTypeCFG.HisImpMestTypeId__Init && resultADO.HisInitSDO != null)
                {
                    HisInitImpMestViewFilter initImpMestFilter = new HisInitImpMestViewFilter();
                    initImpMestFilter.IMP_MEST_ID = this.resultADO.HisInitSDO.ImpMest.ID;
                    var hisInitImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_INIT_IMP_MEST>>(Base.GlobalStore.HIS_INIT_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, initImpMestFilter, null);
                    if (hisInitImpMests != null && hisInitImpMests.Count != 1)
                    {
                        throw new NullReferenceException("Khong lay duoc InitImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.HisInitSDO), resultADO.HisInitSDO));
                    }
                    var initImpMest = hisInitImpMests.FirstOrDefault();

                    HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                    mediFilter.IMP_MEST_ID = this.resultADO.HisInitSDO.ImpMest.ID;
                    var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                    HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                    mateFilter.IMP_MEST_ID = this.resultADO.HisInitSDO.ImpMest.ID;
                    var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);
                    MPS.Processor.Mps000170.PDO.Mps000170PDO Mps000170RDO = new MPS.Processor.Mps000170.PDO.Mps000170PDO(initImpMest, hisImpMestMedicines, hisImpMestMaterials);

                    PrintData(printTypeCode, fileName, Mps000170RDO, ref result);
                }
                //else if (resultADO.ImpMestTypeId == HisImpMestTypeCFG.HisImpMestTypeId__Inve && resultADO.HisInveSDO != null)
                //{
                //    HisInveImpMestViewFilter InveImpMestFilter = new HisInveImpMestViewFilter();
                //    InveImpMestFilter.IMP_MEST_ID = this.resultADO.HisInveSDO.ImpMest.ID;
                //    var hisInveImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_INVE_IMP_MEST>>(Base.GlobalStore.HIS_INVE_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, InveImpMestFilter, null);
                //    if (hisInveImpMests != null && hisInveImpMests.Count != 1)
                //    {
                //        throw new NullReferenceException("Khong lay duoc inveImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.HisInveSDO), resultADO.HisInveSDO));
                //    }
                //    var inveImpMest = hisInveImpMests.FirstOrDefault();

                //    HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                //    mediFilter.IMP_MEST_ID = this.resultADO.HisInveSDO.ImpMest.ID;
                //    var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                //    HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                //    mateFilter.IMP_MEST_ID = this.resultADO.HisInveSDO.ImpMest.ID;
                //    var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);

                //    MPS.Processor.Mps000170.PDO.Mps000170PDO Mps000170RDO = new MPS.Processor.Mps000170.PDO.Mps000170PDO(inveImpMest, hisImpMestMedicines, hisImpMestMaterials);

                //    PrintData(printTypeCode, fileName, Mps000170RDO, ref result);
                //}
                //else if (resultADO.ImpMestTypeId == HisImpMestTypeCFG.HisImpMestTypeId__Other && resultADO.HisOtherSDO != null)
                //{
                //    HisOtherImpMestViewFilter otherImpMestFilter = new HisOtherImpMestViewFilter();
                //    otherImpMestFilter.IMP_MEST_ID = this.resultADO.HisOtherSDO.ImpMest.ID;
                //    var hisOtherImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_OTHER_IMP_MEST>>(Base.GlobalStore.HIS_OTHER_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, otherImpMestFilter, null);
                //    if (hisOtherImpMests != null && hisOtherImpMests.Count != 1)
                //    {
                //        throw new NullReferenceException("Khong lay duoc otherImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.HisOtherSDO), resultADO.HisOtherSDO));
                //    }
                //    var otherImpMest = hisOtherImpMests.FirstOrDefault();

                //    HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                //    mediFilter.IMP_MEST_ID = this.resultADO.HisOtherSDO.ImpMest.ID;
                //    var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                //    HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                //    mateFilter.IMP_MEST_ID = this.resultADO.HisOtherSDO.ImpMest.ID;
                //    var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);
                //    MPS.Processor.Mps000170.PDO.Mps000170PDO Mps000170RDO = new MPS.Processor.Mps000170.PDO.Mps000170PDO(otherImpMest, hisImpMestMedicines, hisImpMestMaterials);

                //    PrintData(printTypeCode, fileName, Mps000170RDO, ref result);
                //}
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void PrintData(string printTypeCode, string fileName, object data, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region shotcut
        private void barButtonItemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnUpdate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
