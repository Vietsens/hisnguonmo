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
using HIS.UC.MedicineType;
using HIS.UC.MaterialType;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.ImpMestCreate.Validation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using DevExpress.Utils.Menu;
using HIS.Desktop.Plugins.ImpMestCreate.Config;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Adapter;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.IO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.MedicineType.ADO;
using HIS.UC.MaterialType.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Data;
using System.Text.RegularExpressions;
using HIS.Desktop.Utility;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.ImpMestCreate.Base;
using MOS.SDO;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using HIS.Desktop.LocalStorage.HisConfig;
using System.Threading;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Desktop.CustomControl;

namespace HIS.Desktop.Plugins.ImpMestCreate
{
    public partial class UCImpMestCreate : UserControlBase
    {
        MedicineTypeProcessor medicineProcessor = null;
        MaterialTypeTreeProcessor materialProcessor = null;
        UserControl ucMedicineTypeTree = null;
        UserControl ucMaterialTypeTree = null;

        V_HIS_MEDI_STOCK medistock = null;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        Inventec.Common.ExcelImport.Import import;

        List<V_HIS_BID_1> listBids = null;
        V_HIS_BID_1 currentBid = null;

        List<HIS_MEDICAL_CONTRACT> listContracts = null;
        List<HIS_MEDICAL_CONTRACT> ContractOfBidAndSup = null;
        HIS_MEDICAL_CONTRACT currentContract = null;
        HIS_MEDICAL_CONTRACT oldContract = null;

        HIS_SUPPLIER currentSupplier = null;
        List<HIS_SUPPLIER> listSupplier = new List<HIS_SUPPLIER>();
        Dictionary<string, V_HIS_BID_MEDICINE_TYPE> dicBidMedicine = new Dictionary<string, V_HIS_BID_MEDICINE_TYPE>();
        Dictionary<string, V_HIS_BID_MATERIAL_TYPE> dicBidMaterial = new Dictionary<string, V_HIS_BID_MATERIAL_TYPE>();

        Dictionary<string, V_HIS_MEDI_CONTRACT_METY> dicContractMety = new Dictionary<string, V_HIS_MEDI_CONTRACT_METY>();
        Dictionary<string, V_HIS_MEDI_CONTRACT_MATY> dicContractMaty = new Dictionary<string, V_HIS_MEDI_CONTRACT_MATY>();

        V_HIS_MEDI_CONTRACT_METY MedicalContractMety = new V_HIS_MEDI_CONTRACT_METY();
        V_HIS_MEDI_CONTRACT_MATY MedicalContractMaty = new V_HIS_MEDI_CONTRACT_MATY();

        List<V_HIS_MEDI_STOCK> listMediStock = new List<V_HIS_MEDI_STOCK>();
        List<HIS_IMP_MEST_TYPE> listImpMestType = new List<HIS_IMP_MEST_TYPE>();

        HIS_IMP_MEST_TYPE currentImpMestType = null;

        List<MaterialTypeADO> listMaterialType = new List<MaterialTypeADO>();
        List<MedicineTypeADO> listMedicineType = new List<MedicineTypeADO>();        

        VHisServiceADO currrentServiceAdo = null;
        ResultImpMestADO resultADO = null;
        List<VHisServicePatyADO> listServicePatyAdo = new List<VHisServicePatyADO>();
        private Dictionary<long, List<VHisServicePatyADO>> dicServicePaty = new Dictionary<long, List<VHisServicePatyADO>>();
        private List<VHisServiceADO> listServiceADO = new List<VHisServiceADO>();
        long medistockID;
        long roomTypeId;
        long roomId;

        List<HIS_SALE_PROFIT_CFG> _HisSaleProfitCfgs { get; set; }
        List<V_HIS_BID_MEDICINE_TYPE> listBidMedicine = new List<V_HIS_BID_MEDICINE_TYPE>();
        List<V_HIS_BID_MATERIAL_TYPE> listBidMaterial = new List<V_HIS_BID_MATERIAL_TYPE>();

        internal bool IsShowMessDocument = false;//#21617
        internal bool IsDisableChkImprice = false;
        internal bool IsValidateInfoBid = false;
        internal bool IsAllowDuplicateDocument = false;//#24171
        internal bool _VACCINE_EXP_PRICE_OPTION = false;//#87604
        internal string _SALE_RETURN_CODE = "";

        decimal VatConfig = 0;
        int positionHandleControl = -1;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isImpMestCreateChange = false;
        bool isInit = true;
        bool isSave = false;

        long _ImpSourceId = 0;

        int theRequiredWidth = 500, theRequiredHeight = 150;
        bool isShowContainerSupplier = false;
        bool isShowContainerSupplierForChoose = false;
        bool isShow = true;

        bool IsAllowedToEnableMedicinesInformation = false;
        bool IsShowingApprovalBid = false;
        bool IsNCC = false;
        bool IsBID = false;
        bool IsHasValueChooice = false;
        bool IsLoadGridMedicine = false;
        bool IsLoadGridMaterial = false;
        bool IsLoadContract = false;
        bool IsChangeTabPage = false;
        bool IsChangeBid = false;

        internal HIS_SUPPLIER currentSupplierForEdit;
        long oldSupplierId = 0;
        internal int d = 0;
        private string moduleLink = "HIS.Desktop.Plugins.ImpMestCreate";
        private bool IsSetBhytInfoFromTypeByDefault;
        private bool IsEditImpPriceVAT;
        private bool IsEditImpPrice;
        private bool IsShowThueXuat;
        private decimal? GiaBan;
        private List<HIS_IMP_SOURCE> lstImpSource = new List<HIS_IMP_SOURCE>();
        private List<HIS_MEDICINE_USE_FORM> lstMedicineUseForm = new List<HIS_MEDICINE_USE_FORM>();
        private List<V_HIS_MEDICINE_TYPE> listMedicineTypeTemp = new List<V_HIS_MEDICINE_TYPE>();
        private List<V_HIS_MATERIAL_TYPE> listMaterialTypeTemp = new List<V_HIS_MATERIAL_TYPE>();
        private List<ACS_USER> lstAcsUser = new List<ACS_USER>();
        bool IsRequiedTemperature = false;
        HIS_MEDICINE currentMedicine;
        HIS_MATERIAL currentMaterial;
        HIS_CONFIG configWarningDiff;
        System.Windows.Forms.Timer TimerFocusImpAmount = new System.Windows.Forms.Timer();
        private string IdentityMaterialOption { get; set; }
        public UCImpMestCreate(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                this.currentModule = module;
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCImpMestCreate(Inventec.Desktop.Common.Modules.Module module, long roomTypeId, long roomId)
            : this(module, roomTypeId, roomId, 0)
        {
        }

        public UCImpMestCreate(Inventec.Desktop.Common.Modules.Module module, long roomTypeId, long roomId, long _expMestId)
            : base(module)
        {
            InitializeComponent();
            this.currentModule = module;
            layoutImpPrice1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem32.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            isImpMestCreateChange = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.ChangeInterface") == "1" ? true : false;
            if (isImpMestCreateChange)
            {
                layoutImpPrice1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem32.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutImpPrice.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            else
            {
                layoutImpPrice1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem32.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutImpPrice.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.roomTypeId = roomTypeId;
                this.roomId = roomId;
                LoadExpMestUpdate(_expMestId);
                InitMedicineTypeTree();
                InitMaterialTypeTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCImpMestCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                listMedicineType = new List<MedicineTypeADO>();
                listMaterialType = new List<MaterialTypeADO>();
                listBids = new List<V_HIS_BID_1>();
                listContracts = new List<HIS_MEDICAL_CONTRACT>();
                //listSupplier = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1).ToList();
                TimerFocusImpAmount.Interval = 100;
                TimerFocusImpAmount.Tick += new System.EventHandler(this.TimerFocusImpAmount_Tick);
                //listBids = new List<V_HIS_BID>();
                //listContracts = new List<HIS_MEDICAL_CONTRACT>();
                this.IdentityMaterialOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_IMP_MEST.IDENTITY_MATERIAL_OPTION");
                this.IsShowingApprovalBid = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.IsShowingApprovalBid") == "1" ? true : false;
                //GetContracts(ref listContracts);
                LoadDataByBidTask();
                //LoadDataByBid();
                //GetContracts(ref listContracts);
                GetImpSource();
                GetMedicineUseForm();
                LoadReceiver();
                LoadSaleProfits();
                LoadManufacturer();
                LoadDosageForm();
                LoadNation();
                CreateThreadLoadData();
                InitComboGoiThau();
                LoadDataToComboSupplier(listSupplier);

                LoadKeyUCLanguage();
                SetFormat();
                InitControlState();
                ValidControl();
                ResetValueControlCommon();
                ResetValueControlDetail();

                LoadDataToComboMediStock();

                SetFocuTreeMediMate();
                SetEnableButton(false);
                cboImpMestType.Focus();

                ValidGoiThauNewControl();

                _dicMedicineTypes = new Dictionary<long, List<V_HIS_BID_MEDICINE_TYPE>>();
                _dicMaterialTypes = new Dictionary<long, List<V_HIS_BID_MATERIAL_TYPE>>();

                InitMenuToButtonPrint(null);

                //GetSaleProfits();

                GetFontSizeForm();
                ValidatecboDosage(cboDosageForm, 1024);
                this.IsShowMessDocument = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate__IsShowMessDocument") == "0" ? false : true;
                this.IsAllowDuplicateDocument = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AllowDuplicateDocumentNumberInTheSameSupplier") == "1" ? true : false;
                this.IsDisableChkImprice = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate__IsDisableChkImprice") == "1" ? true : false;
                this.IsSetBhytInfoFromTypeByDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_MEDICINE.IS_SET_BHYT_INFO_FROM_TYPE_BY_DEFAULT") == "1" ? true : false;
                this.IsValidateInfoBid = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.ValidBid") == "1" ? true : false;
                this.IsAllowedToEnableMedicinesInformation = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.IsAllowedToEnableMedicinesInformation") != "1" ? true : false;
                this._SALE_RETURN_CODE = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_IMP_MEST_SOURCE.SALE_RETURN_CODE");
                this._VACCINE_EXP_PRICE_OPTION = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERE_SERV.VACCINE_EXP_PRICE_OPTION") == "1" ? true : false;
                if (string.IsNullOrEmpty(this._SALE_RETURN_CODE))
                    this._SALE_RETURN_CODE = "BNT";

                //DataToComboNation(cboNationals);
                IsShowThueXuat = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.TaxRatioOption") == "1" ? true : false;
                if (IsShowThueXuat)
                {
                    layoutControlItem27.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutControlItem27.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                //ninhdd để cuối nếu ép kiểu lỗi cũng ko ảnh hưởng.
                this.VatConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<decimal>("HIS.Desktop.Plugins.ImpMestCreate__DefaultImpVAT");

                //SetDataByExpMestUp();
                this.isInit = false;
                InitRestoreLayoutGridViewFromXml(gridViewImpMestDetail);
                WaitingManager.Hide();
                TaskAll();
                VisibleColumn();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TimerFocusImpAmount_Tick(object sender, EventArgs e)
        {
            try
            {
                TimerFocusImpAmount.Stop();
                spinImpAmount.Focus();
                spinImpAmount.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadData()
        {
            Thread threadLoadMedicineType = new Thread(new ThreadStart(LoadMedicineType));
            Thread threadLoadMaterialType = new Thread(new ThreadStart(LoadMaterialType));
            Thread threadGetBids = new Thread(new ThreadStart(GetBid));
            Thread threadGetSupplier = new Thread(new ThreadStart(GetSupplier));
            Thread threadGetContract = new Thread(new ThreadStart(GetContract));

            try
            {

                threadGetBids.Start();
                threadGetSupplier.Start();
                threadGetContract.Start();
                threadLoadMedicineType.Start();
                threadLoadMaterialType.Start();

                threadGetBids.Join();
                threadGetSupplier.Join();
                threadGetContract.Join();
                threadLoadMedicineType.Join();
                threadLoadMaterialType.Join();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadGetBids.Start();
                threadGetSupplier.Abort();
                threadGetContract.Abort();
                threadLoadMedicineType.Abort();
                threadLoadMaterialType.Abort();
            }
        }

        private void VisibleColumn()
        {
            try
            {
                long tp = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AutoRoundExpPriceOption"));
                if (tp == 1 || tp == 2 || tp == 3)
                {
                    gridColumn_ServicePaty_VatRatio.VisibleIndex = -1;
                }
                else
                {
                    gridColumn_ServicePaty_VatRatio.VisibleIndex = 1;
                }
            }
            catch (Exception ex)
            {
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

        private void txtBidYear_KeyDown(object sender, KeyEventArgs e)
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

        private void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath + "/Tmp/Imp", "IMPORT_MEDI_MATE.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_MEDI_MATE";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName, true);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thành công");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thất bại");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidBidControlMaxlength(DevExpress.XtraEditors.TextEdit control, int maxlength, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule _rule = new ControlMaxLengthValidationRule();
                _rule.editor = control;
                _rule.maxLength = maxlength;
                _rule.IsRequired = IsRequired;
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider2.SetValidationRule(control, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtQD_KeyDown(object sender, KeyEventArgs e)
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

        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.currrentServiceAdo != null && this.currrentServiceAdo.IsReusable)
                {
                    if ((SpMaxReuseCount.EditValue == null || String.IsNullOrWhiteSpace(SpMaxReuseCount.Text)
                        || SpMaxReuseCount.Value < 1) && (String.IsNullOrWhiteSpace(TxtSerialNumber.Text)))
                    {
                        chkImprice.Enabled = true;
                    }
                    else
                    {
                        chkImprice.Enabled = false;
                        chkImprice.Checked = false;

                        //Tinh lai gia ban
                        ReUpdatePrice();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SpMaxReuseCount_KeyUp(object sender, KeyEventArgs e)
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

        private void TxtSerialNumber_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    long num = Inventec.Common.TypeConvert.Parse.ToInt64(spinImpAmount.Text);
                    Form.FormSerial se = new Form.FormSerial(TxtSerialNumber.Text, num, (Form.DelegateReturnSerial)SetSerial);
                    se.ShowDialog();
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    TxtSerialNumber.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSerial(string data)
        {
            TxtSerialNumber.Text = data;
        }

        private void TxtSerialNumber_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.currrentServiceAdo != null && (this.currrentServiceAdo.IsReusable || this.currrentServiceAdo.IsIdentity))
                {
                    if ((SpMaxReuseCount.EditValue == null || String.IsNullOrWhiteSpace(SpMaxReuseCount.Text) || SpMaxReuseCount.Value < 1) && (String.IsNullOrWhiteSpace(TxtSerialNumber.Text)))
                    {
                        chkImprice.Enabled = true;
                    }
                    else
                    {
                        chkImprice.Enabled = false;
                        chkImprice.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinEditGiaVeSinh_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ReUpdatePrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinEditTTCoVAT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void spinEditTTChuaVAT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void spinImpAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spinEditTTCoVAT.Value = (spinImpAmount.Value * spinImpPrice.Value * (1 + spinImpVatRatio.Value / 100));
                if (this.currrentServiceAdo != null)
                {
                    this.currrentServiceAdo.DOCUMENT_PRICE = Inventec.Common.TypeConvert.Parse.ToInt32(spinEditTTCoVAT.Value.ToString());
                }

                spinEditTTChuaVAT.Value = (spinImpAmount.Value * spinImpPrice.Value);
                spinEditTTChuaVAT1.Value = (spinImpAmount.Value * spinImpPrice1.Value);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpSource_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboImpSource.Enabled)
                    return;
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboImpSource.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPrice_KeyUp(object sender, KeyEventArgs e)
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

        private void checkOutBid_KeyDown(object sender, KeyEventArgs e)
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

        private void txtkyHieuHoaDon_KeyDown(object sender, KeyEventArgs e)
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

        private void txtkyHieuHoaDon_Leave(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinEditTTCoVAT_Leave(object sender, EventArgs e)
        {
            try
            {
                if (spinImpAmount.Value > 0 && spinEditTTCoVAT.Value > 0)
                {
                    spinImpPrice.Value = spinEditTTCoVAT.Value / spinImpAmount.Value / (1 + spinImpVatRatio.Value / 100);
                    spinImpPrice1.Value = spinEditTTCoVAT.Value / spinImpAmount.Value / (1 + spinImpVatRatio.Value / 100);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinEditTTChuaVAT_Leave(object sender, EventArgs e)
        {
            try
            {
                if (spinImpAmount.Value > 0 && spinEditTTChuaVAT.Value > 0)
                {
                    spinImpPrice.Value = spinEditTTChuaVAT.Value / spinImpAmount.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dropDownButton__Print_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.resultADO == null && !dropDownButton__Print.Enabled)
                    return;
                if (this.resultADO.HisManuSDO.ImpMest.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT
                    && this.resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    dropDownButton__Print.ShowDropDown();
                }
                else
                {
                    onClickBienBanKiemNhapTuNcc(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentNumber_Leave(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNhaCC_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (checkOutBid.Enabled && checkOutBid.Visible)
                    {
                        checkOutBid.Focus();
                        checkOutBid.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Supplier_RowClick(object data)
        {
            try
            {
                this.currentSupplierForEdit = new HIS_SUPPLIER();
                this.currentSupplierForEdit = data as HIS_SUPPLIER;
                if (this.currentSupplierForEdit != null)
                {
                    //this.txtNhaCC.Text = this.currentSupplierForEdit.SUPPLIER_NAME;
                    this.checkOutBid.Focus();
                }

                WaitingManager.Show();

                ResetValueControlDetail();

                this._HisBidBySuppliers = new List<V_HIS_BID_1>();

                this.currentSupplier = null;
                if (this.currentSupplierForEdit != null && !checkOutBid.Checked)
                {
                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, true);
                    this.currentSupplier = this.currentSupplierForEdit;
                    CommonParam param = new CommonParam();
                    Inventec.Common.Logging.LogSystem.Error("this.currentSupplier.ID_____" + this.currentSupplier.ID);
                    List<V_HIS_BID_1> bid = new List<V_HIS_BID_1>();

                    if (currentSupplier.ID > 0)
                    {
                        bid = listBids.Where(o => o.SUPPLIER_IDS != null && ("," + o.SUPPLIER_IDS + ",").Contains("," + currentSupplier.ID + ",")).ToList();
                        this._HisBidBySuppliers = bid;
                    }
                    else
                    {
                        bid = listBids;
                        this.currentSupplier = null;
                    }

                    if (IsShowingApprovalBid)
                    {
                        bid = bid.Where(o => o.IS_ACTIVE == 1 && o.APPROVAL_TIME != null).ToList();
                    }

                    ProcessBidByType();

                    medicineProcessor.ReloadBid(this.ucMedicineTypeTree, bid);
                    materialProcessor.ReloadBid(this.ucMaterialTypeTree, bid);
                    if (this.currentBid != null && currentBid.ID > 0)
                    {
                        materialProcessor.SetEditValueBid(this.ucMedicineTypeTree, currentBid.ID);
                        medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, currentBid.ID);
                    }
                    else
                    {
                        materialProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                        medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    }

                }
                else
                {
                    medicineProcessor.ReloadBid(this.ucMedicineTypeTree, null);
                    materialProcessor.ReloadBid(this.ucMaterialTypeTree, null);
                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, false);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, false);

                    materialProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                }

                if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                {
                    if (medicineProcessor.GetBid(this.ucMedicineTypeTree) != null)
                    {
                        LoadDataByBidMedicine();
                        SetDataSourceGridControlMediMateMedicine();
                    }
                }
                else if ((xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial))
                {
                    if (materialProcessor.GetBid(this.ucMaterialTypeTree) != null)
                    {
                        LoadDataByBidMaterial();
                        SetDataSourceGridControlMediMateMaterial();
                    }
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerSupplier_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNationals_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboNationals.Properties.Buttons[1].Visible)
                        return;

                    cboNationals.EditValue = null;
                    txtNationalMainText.Text = "";
                    cboNationals.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNationals_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboNationals.EditValue != null)
                        this.ChangecboNationl();
                    else
                        SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboNationl()
        {
            try
            {
                cboNationals.Properties.Buttons[1].Visible = true;
                SDA.EFMODEL.DataModels.SDA_NATIONAL national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboNationals.EditValue ?? 0).ToString()));
                if (national != null)
                {
                    txtNationalMainText.Text = national.NATIONAL_NAME;
                    if (chkEditNational.Checked)
                    {
                        txtNognDoHL.Focus();
                        txtNognDoHL.SelectAll();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNationals_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboNationals.ClosePopup();
                    cboNationals.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboNationals.ClosePopup();
                    if (cboNationals.EditValue != null)
                        this.ChangecboNationl();
                    else
                        cboNationals.ShowPopup();
                }
                else
                    cboNationals.ShowPopup();

                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNationalMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditNational.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditNational_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditNational.Checked == true)
                {
                    cboNationals.Visible = false;
                    if (cboHangSX.Enabled && cboHangSX.Visible)
                    {
                        cboHangSX.Focus();
                    }
                    else { SendKeys.Send("{TAB}"); }
                }
                else if (chkEditNational.Checked == false)
                {
                    cboNationals.Visible = true;
                    if (cboHangSX.Enabled && cboHangSX.Visible)
                    {
                        cboHangSX.Focus();
                    }
                    else { SendKeys.Send("{TAB}"); }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditNational_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHangSX_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboHangSX.Properties.Buttons[1].Visible)
                        return;
                    cboHangSX.EditValue = null;
                    cboHangSX.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHangSX_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHangSX_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboHangSX.ClosePopup();
                    cboHangSX.SelectAll();
                }
                else
                    cboHangSX.ShowPopup();

                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboImpSource_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboImpSource.EditValue != null)
                {
                    var data = BackendDataWorker.Get<HIS_IMP_SOURCE>().FirstOrDefault(p => p.IS_ACTIVE == 1 && p.ID == (long)cboImpSource.EditValue);
                    if (data != null && data.IMP_SOURCE_CODE == this._SALE_RETURN_CODE)
                    {
                        HIS.Desktop.Plugins.ImpMestCreate.Form.frmImpSourceReturn frm = new Form.frmImpSourceReturn((DelegateSelectData)ReloadBefoChooseBNT);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadBefoChooseBNT(List<object> datas)
        {
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    List<ExpMestMedicineADO> _Medicines = new List<ExpMestMedicineADO>();
                    List<ExpMestMaterialADO> _Materials = new List<ExpMestMaterialADO>();
                    foreach (var item in datas)
                    {
                        if (item is List<ExpMestMedicineADO>)
                        {
                            _Medicines = (List<ExpMestMedicineADO>)item;
                        }
                        else if (item is List<ExpMestMaterialADO>)
                        {
                            _Materials = (List<ExpMestMaterialADO>)item;
                        }
                    }

                    if (listServiceADO == null)
                        listServiceADO = new List<VHisServiceADO>();

                    List<long> _serviceId = new List<long>();
                    _serviceId.AddRange(_Medicines.Select(p => p.SERVICE_ID).Distinct().ToList());
                    _serviceId.AddRange(_Materials.Select(p => p.SERVICE_ID).Distinct().ToList());
                    List<V_HIS_SERVICE_PATY> dataServicePatys = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(p => _serviceId.Contains(p.SERVICE_ID)).ToList();

                    #region --- Thuoc ---
                    if (_Medicines != null && _Medicines.Count > 0)
                    {
                        MOS.Filter.HisMedicinePatyFilter _patyFilter = new HisMedicinePatyFilter();
                        _patyFilter.MEDICINE_IDs = _Medicines.Select(p => p.MEDICINE_ID ?? 0).Distinct().ToList();
                        var dataMedicinePatys = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE_PATY>>("api/HisMedicinePaty/Get", ApiConsumers.MosConsumer, _patyFilter, null);

                        MOS.Filter.HisMedicineViewFilter _mediFilter = new HisMedicineViewFilter();
                        _mediFilter.IDs = _patyFilter.MEDICINE_IDs;
                        var dataMedicines = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICINE>>("api/HisMedicine/GetView", ApiConsumers.MosConsumer, _mediFilter, null);

                        foreach (var item in _Medicines)
                        {
                            var mediType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            if (mediType != null)
                            {
                                VHisServiceADO ado = new VHisServiceADO(mediType);

                                ado.IMP_AMOUNT = item.YCT_AMOUNT;
                                ado.IMP_PRICE = item.PRICE ?? 0;
                                ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                                ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                                ado.EXPIRED_DATE = item.EXPIRED_DATE;
                                ado.DOCUMENT_PRICE = (long)Math.Round((ado.IMP_AMOUNT * ado.IMP_PRICE * (1 + ado.IMP_VAT_RATIO)), 0, MidpointRounding.AwayFromZero);
                                ado.TAX_RATIO = item.TAX_RATIO;
                                ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;

                                ado.NATIONAL_NAME = item.NATIONAL_NAME;
                                ado.CONCENTRA = item.CONCENTRA;
                                ado.REGISTER_NUMBER = item.REGISTER_NUMBER;
                                ado.MANUFACTURER_ID = item.MANUFACTURER_ID;
                                ado.BID_PRICE = ado.IMP_PRICE;

                                #region ------ THUOC --------
                                ado.HisMedicine.AMOUNT = ado.IMP_AMOUNT;
                                ado.HisMedicine.IMP_PRICE = ado.IMP_PRICE;
                                ado.HisMedicine.IMP_VAT_RATIO = ado.IMP_VAT_RATIO;
                                ado.HisMedicine.DOCUMENT_PRICE = ado.DOCUMENT_PRICE;
                                ado.HisMedicine.PACKAGE_NUMBER = ado.PACKAGE_NUMBER;
                                ado.HisMedicine.EXPIRED_DATE = ado.EXPIRED_DATE;
                                ado.HisMedicine.TAX_RATIO = ado.TAX_RATIO;
                                ado.BidId = item.BID_ID;

                                var medicine = dataMedicines.FirstOrDefault(p => p.ID == item.MEDICINE_ID);
                                if (medicine != null)
                                {
                                    ado.TT_THAU = medicine.TT_THAU;
                                    ado.TDL_BID_NUMBER = medicine.TDL_BID_NUMBER;
                                    ado.TDL_BID_EXTRA_CODE = medicine.TDL_BID_EXTRA_CODE;
                                    ado.TDL_BID_GROUP_CODE = medicine.TDL_BID_GROUP_CODE;
                                    ado.TDL_BID_NUM_ORDER = medicine.TDL_BID_NUM_ORDER;
                                    ado.TDL_BID_PACKAGE_CODE = medicine.TDL_BID_PACKAGE_CODE;
                                    ado.TDL_BID_YEAR = medicine.TDL_BID_YEAR;

                                    ado.packingTypeName = medicine.PACKING_TYPE_NAME;
                                    ado.heinServiceBhytName = medicine.HEIN_SERVICE_BHYT_NAME;
                                    ado.activeIngrBhytName = medicine.ACTIVE_INGR_BHYT_NAME;
                                    ado.HisMedicine.DOSAGE_FORM = medicine.DOSAGE_FORM;
                                    ado.medicineUseFormId = medicine.MEDICINE_USE_FORM_ID;
                                    ado.MEDICAL_CONTRACT_ID = medicine.MEDICAL_CONTRACT_ID;
                                    ado.CONTRACT_PRICE = medicine.CONTRACT_PRICE;

                                    if (medicine.IS_SALE_EQUAL_IMP_PRICE == 1)
                                    {
                                        ado.BanBangGiaNhap = false;
                                        ado.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = null;

                                        ado.HisMedicinePatys = new List<HIS_MEDICINE_PATY>();
                                        ado.VHisServicePatys = new List<VHisServicePatyADO>();

                                        int row = 1;
                                        var patientTypeS = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(p => p.IS_ACTIVE == 1).ToList();
                                        foreach (var itemPatientType in patientTypeS)
                                        {
                                            VHisServicePatyADO adoPaty = new VHisServicePatyADO();
                                            adoPaty.PATIENT_TYPE_NAME = itemPatientType.PATIENT_TYPE_NAME;
                                            adoPaty.PATIENT_TYPE_ID = itemPatientType.ID;
                                            adoPaty.PATIENT_TYPE_CODE = itemPatientType.PATIENT_TYPE_CODE;
                                            adoPaty.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                                            adoPaty.SERVICE_ID = item.SERVICE_ID;
                                            adoPaty.ID = row;

                                            adoPaty.ExpPrice = item.EXP_PRICE ?? 0;
                                            adoPaty.VAT_RATIO = item.EXP_VAT_RATIO ?? 0;
                                            adoPaty.ExpVatRatio = adoPaty.VAT_RATIO * 100;
                                            adoPaty.ExpPriceVat = adoPaty.ExpPrice * (1 + adoPaty.VAT_RATIO);
                                            adoPaty.PRICE = adoPaty.ExpPrice;
                                            adoPaty.PercentProfit = 0;

                                            row++;
                                            ado.VHisServicePatys.Add(adoPaty);

                                            HIS_MEDICINE_PATY mediPaty = new HIS_MEDICINE_PATY();
                                            mediPaty.PATIENT_TYPE_ID = adoPaty.PATIENT_TYPE_ID;
                                            mediPaty.EXP_PRICE = adoPaty.ExpPrice;
                                            mediPaty.EXP_VAT_RATIO = adoPaty.VAT_RATIO;
                                            ado.HisMedicinePatys.Add(mediPaty);
                                        }
                                    }
                                    else
                                    {
                                        ado.BanBangGiaNhap = false;
                                        ado.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = null;

                                        if (ado.HisMedicinePatys == null)
                                        {
                                            ado.HisMedicinePatys = new List<HIS_MEDICINE_PATY>();
                                        }

                                        var dataPatys = dataMedicinePatys.Where(p => p.MEDICINE_ID == item.MEDICINE_ID).ToList();
                                        if (dataPatys != null && dataPatys.Count > 0)
                                        {
                                            ado.HisMedicinePatys.AddRange(dataPatys);

                                            ado.VHisServicePatys = new List<VHisServicePatyADO>();

                                            int row = 1;
                                            foreach (var itemPaty in dataPatys)
                                            {
                                                VHisServicePatyADO adoPaty = new VHisServicePatyADO();
                                                var dataPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(p => p.ID == itemPaty.PATIENT_TYPE_ID);
                                                adoPaty.PATIENT_TYPE_NAME = dataPatientType.PATIENT_TYPE_NAME;
                                                adoPaty.PATIENT_TYPE_ID = itemPaty.PATIENT_TYPE_ID;
                                                adoPaty.PATIENT_TYPE_CODE = dataPatientType.PATIENT_TYPE_CODE;
                                                adoPaty.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                                                adoPaty.SERVICE_ID = item.SERVICE_ID;
                                                adoPaty.ID = row;

                                                adoPaty.ExpPrice = itemPaty.EXP_PRICE;
                                                adoPaty.VAT_RATIO = itemPaty.EXP_VAT_RATIO;
                                                adoPaty.ExpVatRatio = itemPaty.EXP_VAT_RATIO * 100;
                                                adoPaty.ExpPriceVat = itemPaty.EXP_PRICE * (1 + itemPaty.EXP_VAT_RATIO);
                                                adoPaty.PRICE = itemPaty.EXP_PRICE * (1 + itemPaty.EXP_VAT_RATIO);
                                                adoPaty.PercentProfit = 0;

                                                row++;
                                                ado.VHisServicePatys.Add(adoPaty);
                                            }
                                        }
                                    }
                                }

                                ado.HisMedicine.TDL_BID_GROUP_CODE = ado.TDL_BID_GROUP_CODE;
                                ado.HisMedicine.TDL_BID_NUM_ORDER = ado.TDL_BID_NUM_ORDER;
                                ado.HisMedicine.TDL_BID_YEAR = ado.TDL_BID_YEAR;
                                ado.HisMedicine.TDL_BID_PACKAGE_CODE = ado.TDL_BID_PACKAGE_CODE;
                                ado.HisMedicine.TDL_BID_NUMBER = ado.TDL_BID_NUMBER;
                                ado.HisMedicine.BID_ID = ado.BidId;//xuandv them
                                ado.HisMedicine.TDL_BID_EXTRA_CODE = ado.TDL_BID_EXTRA_CODE;

                                if (item.SUPPLIER_ID > 0)
                                    ado.SupplierId = item.SUPPLIER_ID ?? 0;

                                ado.HisMedicine.NATIONAL_NAME = ado.NATIONAL_NAME;
                                ado.HisMedicine.CONCENTRA = ado.CONCENTRA;
                                ado.HisMedicine.MEDICINE_REGISTER_NUMBER = ado.REGISTER_NUMBER;
                                ado.HisMedicine.MANUFACTURER_ID = ado.MANUFACTURER_ID;
                                ado.HisMedicine.MEDICAL_CONTRACT_ID = ado.MEDICAL_CONTRACT_ID;

                                ado.HisMedicine.PACKING_TYPE_NAME = ado.packingTypeName;
                                ado.HisMedicine.HEIN_SERVICE_BHYT_NAME = ado.heinServiceBhytName;
                                ado.HisMedicine.ACTIVE_INGR_BHYT_NAME = ado.activeIngrBhytName;
                                ado.HisMedicine.DOSAGE_FORM = ado.HisMedicine.DOSAGE_FORM;
                                ado.HisMedicine.MEDICINE_USE_FORM_ID = ado.medicineUseFormId;
                                listServiceADO.Add(ado);
                                #endregion
                            }
                        }
                    }
                    #endregion

                    #region --- VatTu ---
                    if (_Materials != null && _Materials.Count > 0)
                    {
                        MOS.Filter.HisMaterialPatyFilter _patyFilter = new HisMaterialPatyFilter();
                        _patyFilter.MATERIAL_IDs = _Materials.Select(p => p.MATERIAL_ID ?? 0).Distinct().ToList();

                        _patyFilter.IS_ACTIVE = 1;
                        var dataMedicinePatys = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL_PATY>>("api/HisMaterialPaty/Get", ApiConsumers.MosConsumer, _patyFilter, null);

                        MOS.Filter.HisMaterialViewFilter _mateFilter = new HisMaterialViewFilter();
                        _mateFilter.IDs = _patyFilter.MATERIAL_IDs;
                        _mateFilter.IS_ACTIVE = 1;
                        var dataMaterials = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MATERIAL>>("api/HisMaterial/GetView", ApiConsumers.MosConsumer, _mateFilter, null);

                        foreach (var item in _Materials)
                        {
                            var mateType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                            if (mateType != null)
                            {
                                VHisServiceADO ado = new VHisServiceADO(mateType);
                                ado.IMP_AMOUNT = item.YCT_AMOUNT;
                                ado.IMP_PRICE = item.PRICE ?? 0;
                                ado.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                                ado.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                                ado.EXPIRED_DATE = item.EXPIRED_DATE;
                                ado.TAX_RATIO = item.TAX_RATIO;
                                ado.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                                ado.NATIONAL_NAME = item.NATIONAL_NAME;
                                ado.CONCENTRA = mateType.CONCENTRA;
                                ado.MANUFACTURER_ID = item.MANUFACTURER_ID;
                                ado.BID_PRICE = ado.IMP_PRICE;
                                #region ------ VT --------

                                ado.HisMaterial.AMOUNT = ado.IMP_AMOUNT;
                                ado.HisMaterial.IMP_PRICE = ado.IMP_PRICE;
                                ado.HisMaterial.IMP_VAT_RATIO = ado.IMP_VAT_RATIO;
                                ado.HisMaterial.DOCUMENT_PRICE = ado.DOCUMENT_PRICE;
                                ado.HisMaterial.PACKAGE_NUMBER = ado.PACKAGE_NUMBER;
                                ado.HisMaterial.EXPIRED_DATE = ado.EXPIRED_DATE;
                                ado.HisMaterial.TAX_RATIO = ado.TAX_RATIO;
                                ado.BidId = item.BID_ID;

                                var material = dataMaterials.FirstOrDefault(p => p.ID == item.MATERIAL_ID);
                                if (material != null)
                                {
                                    ado.TDL_BID_NUMBER = material.TDL_BID_NUMBER;
                                    ado.TDL_BID_EXTRA_CODE = material.TDL_BID_EXTRA_CODE;
                                    ado.TDL_BID_GROUP_CODE = material.TDL_BID_GROUP_CODE;
                                    ado.TDL_BID_NUM_ORDER = material.TDL_BID_NUM_ORDER;
                                    ado.TDL_BID_PACKAGE_CODE = material.TDL_BID_PACKAGE_CODE;
                                    ado.TDL_BID_YEAR = material.TDL_BID_YEAR;
                                    ado.TT_THAU = material.TT_THAU;
                                    ado.MEDICAL_CONTRACT_ID = material.MEDICAL_CONTRACT_ID;
                                    ado.CONTRACT_PRICE = material.CONTRACT_PRICE;
                                    ado.packingTypeName = material.BID_MATERIAL_TYPE_CODE;
                                    ado.heinServiceBhytName = material.BID_MATERIAL_TYPE_NAME;
                                    ado.REGISTER_NUMBER = material.MATERIAL_REGISTER_NUMBER;
                                    if (material.IS_SALE_EQUAL_IMP_PRICE == 1)
                                    {
                                        ado.BanBangGiaNhap = false;
                                        ado.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = null;

                                        ado.HisMaterialPatys = new List<HIS_MATERIAL_PATY>();
                                        ado.VHisServicePatys = new List<VHisServicePatyADO>();

                                        int row = 1;
                                        var patientTypeS = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(p => p.IS_ACTIVE == 1).ToList();
                                        foreach (var itemPatientType in patientTypeS)
                                        {
                                            VHisServicePatyADO adoPaty = new VHisServicePatyADO();
                                            adoPaty.PATIENT_TYPE_NAME = itemPatientType.PATIENT_TYPE_NAME;
                                            adoPaty.PATIENT_TYPE_ID = itemPatientType.ID;
                                            adoPaty.PATIENT_TYPE_CODE = itemPatientType.PATIENT_TYPE_CODE;
                                            adoPaty.IsNotSell = false;
                                            adoPaty.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                                            adoPaty.SERVICE_ID = item.SERVICE_ID;
                                            adoPaty.ID = row;

                                            adoPaty.ExpPrice = item.EXP_PRICE ?? 0;
                                            adoPaty.VAT_RATIO = item.EXP_VAT_RATIO ?? 0;
                                            adoPaty.ExpVatRatio = adoPaty.VAT_RATIO * 100;
                                            adoPaty.ExpPriceVat = adoPaty.ExpPrice * (1 + adoPaty.VAT_RATIO);
                                            adoPaty.PRICE = adoPaty.ExpPrice;
                                            adoPaty.PercentProfit = 0;

                                            row++;
                                            ado.VHisServicePatys.Add(adoPaty);

                                            HIS_MATERIAL_PATY mediPaty = new HIS_MATERIAL_PATY();
                                            mediPaty.PATIENT_TYPE_ID = adoPaty.PATIENT_TYPE_ID;
                                            mediPaty.EXP_PRICE = adoPaty.ExpPrice;
                                            mediPaty.EXP_VAT_RATIO = adoPaty.VAT_RATIO;
                                            ado.HisMaterialPatys.Add(mediPaty);
                                        }
                                    }
                                    else
                                    {
                                        ado.BanBangGiaNhap = false;
                                        ado.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = null;

                                        if (ado.HisMaterialPatys == null)
                                        {
                                            ado.HisMaterialPatys = new List<HIS_MATERIAL_PATY>();
                                        }
                                        var dataPatys = dataMedicinePatys.Where(p => p.MATERIAL_ID == item.MATERIAL_ID).ToList();

                                        ado.HisMaterialPatys.AddRange(dataPatys);

                                        ado.VHisServicePatys = new List<VHisServicePatyADO>();

                                        int row = 1;
                                        foreach (var itemPaty in dataPatys)
                                        {
                                            VHisServicePatyADO adoPaty = new VHisServicePatyADO();
                                            var dataPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(p => p.ID == itemPaty.PATIENT_TYPE_ID);
                                            adoPaty.PATIENT_TYPE_NAME = dataPatientType.PATIENT_TYPE_NAME;
                                            adoPaty.PATIENT_TYPE_ID = itemPaty.PATIENT_TYPE_ID;
                                            adoPaty.PATIENT_TYPE_CODE = dataPatientType.PATIENT_TYPE_CODE;
                                            adoPaty.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                                            adoPaty.SERVICE_ID = item.SERVICE_ID;
                                            adoPaty.ID = row;

                                            adoPaty.ExpPrice = itemPaty.EXP_PRICE;
                                            adoPaty.VAT_RATIO = itemPaty.EXP_VAT_RATIO;
                                            adoPaty.ExpVatRatio = itemPaty.EXP_VAT_RATIO * 100;
                                            adoPaty.ExpPriceVat = itemPaty.EXP_PRICE * (1 + itemPaty.EXP_VAT_RATIO);
                                            adoPaty.PRICE = itemPaty.EXP_PRICE * (1 + itemPaty.EXP_VAT_RATIO);
                                            adoPaty.PercentProfit = 0;

                                            row++;
                                            ado.VHisServicePatys.Add(adoPaty);
                                        }
                                    }
                                }

                                ado.HisMaterial.TDL_BID_GROUP_CODE = ado.TDL_BID_GROUP_CODE;
                                ado.HisMaterial.TDL_BID_NUM_ORDER = ado.TDL_BID_NUM_ORDER;
                                ado.HisMaterial.TDL_BID_YEAR = ado.TDL_BID_YEAR;
                                ado.HisMaterial.TDL_BID_PACKAGE_CODE = ado.TDL_BID_PACKAGE_CODE;
                                ado.HisMaterial.TDL_BID_NUMBER = ado.TDL_BID_NUMBER;
                                ado.HisMaterial.BID_ID = ado.BidId;//xuandv them
                                ado.HisMaterial.TDL_BID_EXTRA_CODE = ado.TDL_BID_EXTRA_CODE;

                                if (item.SUPPLIER_ID > 0)
                                    ado.SupplierId = item.SUPPLIER_ID ?? 0;

                                ado.HisMaterial.NATIONAL_NAME = ado.NATIONAL_NAME;
                                ado.HisMaterial.CONCENTRA = ado.CONCENTRA;
                                ado.HisMaterial.MANUFACTURER_ID = ado.MANUFACTURER_ID;
                                ado.HisMaterial.MEDICAL_CONTRACT_ID = ado.MEDICAL_CONTRACT_ID;
                                ado.HisMaterial.BID_MATERIAL_TYPE_CODE = ado.packingTypeName;
                                ado.HisMaterial.BID_MATERIAL_TYPE_NAME = ado.heinServiceBhytName;
                                //ado.HisMaterial.MATERIAL_REGISTER_NUMBER = ado.REGISTER_NUMBER;

                                listServiceADO.Add(ado);
                                #endregion
                            }
                        }
                    }
                    #endregion
                    Inventec.Common.Logging.LogSystem.Debug("09_______AA" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listServiceADO), listServiceADO));
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

        private void cboGoiThau_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboGoiThau.EditValue != null)
                    {
                        cboGoiThau.Properties.Buttons[1].Visible = true;
                        long bidId = (long)cboGoiThau.EditValue;
                        txtBidNumOrder.Enabled = false;
                        txtBidYear.Enabled = false;
                        txtBidNumber.Enabled = false;
                        txtBidGroupCode.Enabled = false;
                        txtBid.Enabled = false;
                        txtBidNumOrder.Text = "";
                        txtBidYear.Text = "";
                        txtBidNumber.Text = "";
                        txtBidGroupCode.Text = "";
                        txtBid.Text = "";
                        this.currentBid = listBids.FirstOrDefault(o => o.ID == bidId);

                        MethodProcessBefoClick();
                    }
                    else
                    {
                        cboGoiThau.Properties.Buttons[1].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNognDoHL_KeyDown(object sender, KeyEventArgs e)
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

        private void txtSoDangKy_KeyDown(object sender, KeyEventArgs e)
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

        private void chkWarningOldBid_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkWarningOldBid.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkWarningOldBid.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkWarningOldBid.Name;
                    csAddOrUpdate.VALUE = (chkWarningOldBid.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkNoProfitBhyt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkNoProfitBhyt.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkNoProfitBhyt.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkNoProfitBhyt.Name;
                    csAddOrUpdate.VALUE = (chkNoProfitBhyt.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPrice1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!spinImpPrice1.Visible || layoutImpPrice1.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    return;
                if (IsEditImpPriceVAT) return;

                IsEditImpPrice = true;
                if (this.currrentServiceAdo != null)
                    this.currrentServiceAdo.IMP_PRICE = spinImpPrice1.Value;
                spinImpPriceVAT.Value = (spinImpPrice1.Value * (1 + spinImpVatRatio.Value / 100));

                spinEditTTCoVAT.Value = Math.Round((spinImpAmount.Value * spinImpPrice1.Value * (1 + spinImpVatRatio.Value / 100)), ConfigApplications.NumberSeperator, MidpointRounding.AwayFromZero);
                spinEditTTChuaVAT1.Value = (spinImpAmount.Value * spinImpPrice1.Value);
                if (this.currrentServiceAdo != null && this.currrentServiceAdo.IsReusable)
                {
                    this.currrentServiceAdo.IMP_PRICE = spinImpPrice1.Value;
                    ReUpdatePrice();
                }

                IsEditImpPrice = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPrice1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void spinEditTTChuaVAT1_Leave(object sender, EventArgs e)
        {
            try
            {
                if (spinImpAmount.Value > 0 && spinEditTTChuaVAT1.Value > 0)
                {
                    spinImpPrice1.Value = spinEditTTChuaVAT1.Value / spinImpAmount.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinEditTTChuaVAT1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void spinCanImpAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void cboGoiThau_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtNognDoHL.Enabled && txtNognDoHL.Visible)
                    {
                        if (cboGoiThau.EditValue != null)
                        {
                            if (txtSoDangKy.Enabled && txtSoDangKy.Visible)
                            {
                                txtSoDangKy.Focus();
                                txtSoDangKy.SelectAll();
                            }
                            else { SendKeys.Send("{TAB}"); }
                        }
                        else if (txtBidGroupCode.Enabled && txtBidGroupCode.Visible)
                        {
                            txtBidGroupCode.Focus();
                            txtBidGroupCode.SelectAll();
                        }
                        else { SendKeys.Send("{TAB}"); }
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHangSX_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void spinImpPrice_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
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

        private void spinEditGiaVeSinh_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd1.Enabled)
                    {
                        btnAdd1.Focus();
                        e.Handled = true;
                    }
                    else if (btnUpdate1.Enabled)
                    {
                        btnUpdate1.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRecieve_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRecieve.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRecieve_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtDescription.Enabled && txtDescription.Visible)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                }
                else
                {
                    cboRecieve.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRecieve_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboRecieve.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboRecieve.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.cboRecieve.Text = data.USERNAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool IsCheckTemperature()
        {
            bool rs = true;
            try
            {
                if (IsRequiedTemperature)
                {
                    rs = false;
                    var storageCondition = BackendDataWorker.Get<HIS_STORAGE_CONDITION>().FirstOrDefault(o => o.ID == currrentServiceAdo.STORAGE_CONDITION_ID);
                    if (storageCondition != null)
                    {
                        if (storageCondition.FROM_TEMPERATURE.HasValue && storageCondition.FROM_TEMPERATURE.Value > spnTemperature.Value)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Thông tin Nhiệt độ nhỏ hơn nhiệt độ bảo quản {0}°C. Bạn có muốn tiếp tục?", storageCondition.FROM_TEMPERATURE.Value), Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) == DialogResult.Yes)
                                return true;
                            else
                                return false;
                        }
                        if (storageCondition.TO_TEMPERATURE.HasValue && storageCondition.TO_TEMPERATURE.Value < spnTemperature.Value)
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Thông tin Nhiệt độ lớn hơn nhiệt độ bảo quản {0}°C. Bạn có muốn tiếp tục?", storageCondition.TO_TEMPERATURE.Value), Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) == DialogResult.Yes)
                                return true;
                            else
                                return false;
                        }
                    }
                    rs = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private void btnAdd1_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.gridViewServicePaty.IsEditing)
                    this.gridViewServicePaty.CloseEditor();

                if (this.gridViewServicePaty.FocusedRowModified)
                    this.gridViewServicePaty.UpdateCurrentRow();
                if (!btnAdd1.Enabled)   
                    return;
                btnAdd1.Focus();
                positionHandleControl = -1;
                if (!dxValidationProvider2.Validate() || this.currrentServiceAdo == null)
                    return;

                string messageError = "";
                if (!CheckAllowAdd(ref messageError))
                {
                    MessageManager.Show(messageError);
                    return;
                }
                if (!ShowMessValidPrice()) return;

                if (ShowMessValidDocumentAndDate())
                    return;

                if (!CheckExpiredDate())
                {
                    return;
                }

                if (!CheckBhytMedicineInfo())
                {
                    return;
                }

                if (!CheckOutBid(this.currrentServiceAdo))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.ThuocVatTuKhongNamTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        return;
                }
                if (this.currentContract == null && this.CheckInBid(this.currrentServiceAdo) && spinCanImpAmount.Value < spinImpAmount.Value)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.SoLuongNhapLonHonSoLuongKhaNhapTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                }

                if ((this.currrentServiceAdo.BidImpPrice.HasValue && this.currrentServiceAdo.BidImpPrice.Value != spinImpPrice.Value) && (this.currrentServiceAdo.BidImpVatRatio.HasValue && this.currrentServiceAdo.BidImpVatRatio.Value != (spinImpVatRatio.Value / 100)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.GiaVatNhapKhacVoiGiaVatNhapTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                }
                else if (this.currrentServiceAdo.BidImpPrice.HasValue && this.currrentServiceAdo.BidImpPrice.Value != spinImpPrice.Value)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.GiaNhapKhacVoiGiaNhapTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                }
                else if (this.currrentServiceAdo.BidImpVatRatio.HasValue && this.currrentServiceAdo.BidImpVatRatio.Value != (spinImpVatRatio.Value / 100))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.VatNhapKhacVoiVatNhapTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                }


                if (this.currentContract != null)
                {
                    long timeNow = Inventec.Common.DateTime.Get.StartDay() ?? 0;
                    if (this.currentContract.VALID_TO_DATE.HasValue && timeNow > this.currentContract.VALID_TO_DATE.Value)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessageManager.ThoiGianNhapVuotHieuLucHopDong, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentContract.VALID_TO_DATE.Value)), Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                        return;
                    }

                    if (this.MedicalContractMety != null && xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                    {
                        if (spinCanImpAmount.Value < spinImpAmount.Value)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.SoLuongNhapLonHonSoLuongTrongHopDong, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                            return;
                        }

                        if (spinImpPriceVAT.Value > spinEditGiaTrongThau.Value)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.GiaSauVatLonHonGiaHopDong, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                            return;
                        }

                        if (this.MedicalContractMety.IMP_EXPIRED_DATE.HasValue && this.MedicalContractMety.IMP_EXPIRED_DATE.Value < timeNow)
                        {
                            string mess = string.Format(ResourceMessageManager.ThoiGianNhapVuotHanNhapThuocVatTu, string.Format("{0} ({1})", ResourceMessageManager.TieuDeThuoc, this.currrentServiceAdo.MEDI_MATE_NAME));
                            DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                            return;
                        }
                    }

                    if (this.MedicalContractMaty != null && xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                    {
                        if (spinCanImpAmount.Value < spinImpAmount.Value)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.SoLuongNhapLonHonSoLuongTrongHopDong, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                            return;
                        }

                        if (spinImpPriceVAT.Value > spinEditGiaTrongThau.Value)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.GiaSauVatLonHonGiaHopDong, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                            return;
                        }

                        if (this.MedicalContractMaty.IMP_EXPIRED_DATE.HasValue && this.MedicalContractMaty.IMP_EXPIRED_DATE.Value < timeNow)
                        {
                            string mess = string.Format(ResourceMessageManager.ThoiGianNhapVuotHanNhapThuocVatTu, string.Format("{0} ({1})", ResourceMessageManager.TieuDeVatTu, this.currrentServiceAdo.MEDI_MATE_NAME));
                            DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                            return;
                        }
                    }
                }
                if (!String.IsNullOrWhiteSpace(txtPackageNumber.Text))
                {
                    this.currrentServiceAdo.PACKAGE_NUMBER = txtPackageNumber.Text;
                }
                else
                {
                    this.currrentServiceAdo.PACKAGE_NUMBER = "";
                }
                this.currrentServiceAdo.TDL_BID_GROUP_CODE = txtBidGroupCode.Text;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfig.AllowDuplicate), HisConfig.AllowDuplicate));
                if (!HisConfig.AllowDuplicate)
                {
                    var dataThayThe = listServiceADO.FirstOrDefault(o => o.SERVICE_ID == this.currrentServiceAdo.SERVICE_ID && o.TDL_BID_GROUP_CODE == this.currrentServiceAdo.TDL_BID_GROUP_CODE && o.PACKAGE_NUMBER == this.txtPackageNumber.Text.ToString().Trim());
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataThayThe), dataThayThe));
                    if (dataThayThe != null)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.ThuocVatTuDaCoTrongDanhSachNhap, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return;
                        }
                        else
                        {
                            listServiceADO.RemoveAll(o => o == dataThayThe);
                        }
                    }
                }
                if (dtExpiredDate.EditValue != null && dtExpiredDate.DateTime != DateTime.MinValue)
                {
                    this.currrentServiceAdo.EXPIRED_DATE = Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "235959");
                    DateTime? _EXPIRED_DATE = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currrentServiceAdo.EXPIRED_DATE ?? 0);
                    long impMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboImpMestType.EditValue ?? "0").ToString());
                    if (_EXPIRED_DATE < DateTime.Now && impMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                    {
                        string _tittle = "";
                        if (this.currrentServiceAdo.IsMedicine)
                            _tittle = Base.ResourceMessageManager.TieuDeThuoc;
                        else
                            _tittle = Base.ResourceMessageManager.TieuDeVatTu;

                        string mess = _tittle + " <color=red>" + this.currrentServiceAdo.MEDI_MATE_CODE + "</color> " + Base.ResourceMessageManager.TieuDeDaHetHanSuDung + "\n" + Base.ResourceMessageManager.TieuDeBanCoMuonTiepTuc;
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return;
                        }
                    }
                }

                if (CheckLifespan(this.currrentServiceAdo))
                {
                    return;
                }

                long impMestTypeIdV2 = Inventec.Common.TypeConvert.Parse.ToInt64((cboImpMestType.EditValue ?? "0").ToString());
                if (impMestTypeIdV2 == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    long timeHieuLucTu = dtHieuLucTu.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHieuLucTu.DateTime) ?? 0 : 0;
                    long timeHieuLucDen = dtHieuLucDen.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHieuLucDen.DateTime) ?? 0 : 0;
                    long dateNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    bool _21299 = false;
                    if (timeHieuLucTu > 0 && timeHieuLucDen > 0)
                    {
                        if (dateNow < timeHieuLucTu || dateNow > timeHieuLucDen)
                            _21299 = true;
                    }
                    else if (timeHieuLucTu > 0 && timeHieuLucDen == 0)
                    {
                        if (dateNow < timeHieuLucTu)
                            _21299 = true;
                    }
                    else if (timeHieuLucDen > 0 && timeHieuLucTu == 0)
                    {
                        if (dateNow > timeHieuLucDen)
                            _21299 = true;
                    }

                    if (_21299)
                    {
                        string _showMess = string.Format("Đã ngoài thời gian hiệu lực thầu ({0} -- {1}). Bạn có muốn thực hiện không?", Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(dtHieuLucTu.DateTime), Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(dtHieuLucDen.DateTime));
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(_showMess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            return;
                    }
                    else if (chkWarningOldBid.Checked)
                    {
                        //cảnh báo với thuốc/vật tư đầu tiên được bổ sung.
                        bool cboBidgrid = true;
                        if (xtraTabControlMain.SelectedTabPageIndex == 0)
                        {
                            cboBidgrid = medicineProcessor.GetBidEnable(ucMedicineTypeTree);
                        }
                        else
                        {
                            cboBidgrid = materialProcessor.GetBidEnable(ucMaterialTypeTree);
                        }

                        if (cboGoiThau.Enabled || cboBidgrid || txtBidNumber.Enabled)
                        {
                            V_HIS_BID_1 bidNew = new V_HIS_BID_1();

                            //gọi api check thông tin thầu
                            if (currrentServiceAdo.IsMedicine && _dicBidMedicineTypes.ContainsKey(currrentServiceAdo.MEDI_MATE_ID))
                            {
                                var lstBidMedicineType = _dicBidMedicineTypes[currrentServiceAdo.MEDI_MATE_ID].Where(p => p.SUPPLIER_ID == this.currentSupplierForEdit.ID).ToList();
                                var listBid = listBids.Where(o => lstBidMedicineType.Exists(m => m.BID_ID == o.ID)).ToList();
                                if (listBid != null && listBid.Count > 0 && timeHieuLucDen > 0)
                                {
                                    bidNew = listBid.Where(o => o.VALID_TO_TIME.HasValue && o.VALID_TO_TIME > timeHieuLucDen).OrderByDescending(o => o.VALID_TO_TIME).FirstOrDefault();
                                }
                            }
                            else if (_dicBidMaterialTypes.ContainsKey(currrentServiceAdo.MEDI_MATE_ID))
                            {
                                var lstBidMaterialType = _dicBidMaterialTypes[currrentServiceAdo.MEDI_MATE_ID].Where(p => p.SUPPLIER_ID == this.currentSupplierForEdit.ID).ToList();
                                var listBid = listBids.Where(o => lstBidMaterialType.Exists(m => m.BID_ID == o.ID)).ToList();
                                if (listBid != null && listBid.Count > 0 && timeHieuLucDen > 0)
                                {
                                    bidNew = listBid.Where(o => o.VALID_TO_TIME.HasValue && o.VALID_TO_TIME > timeHieuLucDen).OrderByDescending(o => o.VALID_TO_TIME).FirstOrDefault();
                                }
                            }

                            //tồn tại hạn thầu mới hơn
                            if (bidNew != null && bidNew.ID > 0 && DevExpress.XtraEditors.XtraMessageBox.Show("Tồn tại thầu " + bidNew.BID_NUMBER + " có hạn thầu mới hơn. Bạn có chắc muốn thực hiện không?", Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                try
                                {
                                    if (cboGoiThau.Enabled)
                                    {
                                        cboGoiThau.Focus();
                                    }
                                    else if (currrentServiceAdo.IsMedicine && medicineProcessor.GetBidEnable(ucMedicineTypeTree))
                                    {
                                        medicineProcessor.FocusBid(ucMedicineTypeTree);
                                    }
                                    else if (!currrentServiceAdo.IsMedicine && materialProcessor.GetBidEnable(ucMaterialTypeTree))
                                    {
                                        materialProcessor.FocusBid(ucMaterialTypeTree);
                                    }
                                    else
                                    {
                                        txtBidNumber.Focus();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                                return;
                            }
                        }
                    }
                }
                if (!IsCheckTemperature())
                {
                    return;
                }

                //WaitingManager.Show();

                this.currrentServiceAdo.TEMPERATURE = spnTemperature.EditValue != null ? (decimal?)spnTemperature.Value : null;
                this.currrentServiceAdo.TDL_BID_NUM_ORDER = txtBidNumOrder.Text.Trim();
                this.currrentServiceAdo.TDL_BID_PACKAGE_CODE = txtBid.Text.Trim();
                this.currrentServiceAdo.TDL_BID_YEAR = txtBidYear.Text.Trim();
                this.currrentServiceAdo.TDL_BID_NUMBER = txtBidNumber.Text.Trim();
                this.currrentServiceAdo.TDL_BID_EXTRA_CODE = txtBidExtraCode.Text.Trim();

                this.currrentServiceAdo.packingTypeName = this.txtPackingJoinBid.Text.Trim();
                this.currrentServiceAdo.heinServiceBhytName = this.txtHeinServiceBidMateType.Text.Trim();
                this.currrentServiceAdo.activeIngrBhytName = this.txtActiveIngrBhytName.Text.Trim();
                

                if (this.cboMedicineUseForm.EditValue != null)
                {
                    this.currrentServiceAdo.medicineUseFormId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboMedicineUseForm.EditValue.ToString());
                }

                this.currrentServiceAdo.CanImpAmount = spinCanImpAmount.Value;
                this.currrentServiceAdo.IMP_AMOUNT = spinImpAmount.Value;
                this.currrentServiceAdo.GiaBan = GiaBan;
                if (spinImpPrice1.Enabled && spinImpPrice1.Visible)
                {
                    this.currrentServiceAdo.IMP_PRICE = spinImpPrice1.Value;
                }
                if (spinImpPrice.Enabled && spinImpPrice.Visible)
                {
                    this.currrentServiceAdo.IMP_PRICE = spinImpPrice.Value;
                }
                this.currrentServiceAdo.IMP_VAT_RATIO = spinImpVatRatio.Value / 100;
                this.currrentServiceAdo.ImpVatRatio = spinImpVatRatio.Value;
                this.currrentServiceAdo.BID_PRICE = spinEditGiaTrongThau.Value;
                this.currrentServiceAdo.IMP_PRICE_PREVIOUS = spinEditGiaNhapLanTruoc.Value;

                if (chkImprice.Checked)
                {
                    if (this.currrentServiceAdo.GiaBan > 0 && this.currrentServiceAdo.IMP_PRICE_PREVIOUS != this.currrentServiceAdo.GiaBan)
                    {
                        this.currrentServiceAdo.WarningPrice = string.Format("{0} {1} có giá bán lần trước = {2} khác với giá bán hiện tại. Bạn có muốn tiếp tục thêm ?",
                            this.currrentServiceAdo.IsMedicine ? "Thuốc" : "Vật tư", this.currrentServiceAdo.MEDI_MATE_NAME, string.Format("{0:#,0}", this.currrentServiceAdo.GiaBan));

                        WaitingManager.Hide();
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(this.currrentServiceAdo.WarningPrice, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return;
                        }
                    }

                }

                if (this.currentContract != null)
                {
                    this.currrentServiceAdo.MEDICAL_CONTRACT_ID = this.currentContract.ID;
                    this.currrentServiceAdo.CONTRACT_PRICE = spinEditGiaTrongThau.Value;
                }

                this.currrentServiceAdo.DOCUMENT_PRICE = Inventec.Common.TypeConvert.Parse.ToInt64((spinEditTTCoVAT.EditValue ?? "0").ToString());
                this.currrentServiceAdo.TAX_RATIO = IsShowThueXuat ? (spinEditThueXuat.Value / 100) : 0;
                if (this.currrentServiceAdo.DOCUMENT_PRICE <= 0)
                {
                    this.currrentServiceAdo.DOCUMENT_PRICE = (long)Math.Round((this.currrentServiceAdo.IMP_AMOUNT * this.currrentServiceAdo.IMP_PRICE * (1 + this.currrentServiceAdo.IMP_VAT_RATIO)), 0, MidpointRounding.AwayFromZero);
                }

                if (dtHieuLucTu.EditValue != null && dtHieuLucTu.DateTime != DateTime.MinValue)
                {
                    this.currrentServiceAdo.VALID_FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHieuLucTu.DateTime);// Convert.ToInt64(dtHieuLucTu.DateTime.ToString("yyyyMMdd") + "000000");
                }

                if (dtHieuLucDen.EditValue != null && dtHieuLucDen.DateTime != DateTime.MinValue)
                {
                    this.currrentServiceAdo.VALID_TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHieuLucDen.DateTime);// Convert.ToInt64(dtHieuLucDen.DateTime.ToString("yyyyMMdd") + "235959");
                }
                this.currrentServiceAdo.NATIONAL_NAME = this.txtNationalMainText.Text.Trim();
                this.currrentServiceAdo.CONCENTRA = this.txtNognDoHL.Text;
                this.currrentServiceAdo.REGISTER_NUMBER = this.txtSoDangKy.Text;

                if (cboHangSX.EditValue != null)
                {
                    this.currrentServiceAdo.MANUFACTURER_ID = (long)cboHangSX.EditValue;
                }
                else
                    this.currrentServiceAdo.MANUFACTURER_ID = null;

                AutoMapper.Mapper.CreateMap<VHisServicePatyADO, VHisServicePatyADO>();
                this.currrentServiceAdo.VHisServicePatys = AutoMapper.Mapper.Map<List<VHisServicePatyADO>>(listServicePatyAdo);

                #region ------ THUOC --------
                long tp_ = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AmountDecimalNumber"));
                long tp = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AutoRoundExpPriceOption"));
                if (this.currrentServiceAdo.IsMedicine)
                {
                    this.currrentServiceAdo.HisMedicine.AMOUNT = this.currrentServiceAdo.IMP_AMOUNT;

                    if (cboDosageForm.EditValue != null)
                    {
                        this.currrentServiceAdo.DOSAGE_FORM = cboDosageForm.Text;
                    }
                    else if (this.currrentServiceAdo.MEDICINE_LINE_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT)
                    {
                        XtraMessageBox.Show("Dạng bào chế không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    this.currrentServiceAdo.HisMedicine.IMP_PRICE = this.currrentServiceAdo.IMP_PRICE;
                    this.currrentServiceAdo.HisMedicine.IMP_VAT_RATIO = this.currrentServiceAdo.IMP_VAT_RATIO;
                    this.currrentServiceAdo.HisMedicine.TAX_RATIO = this.currrentServiceAdo.TAX_RATIO;
                    this.currrentServiceAdo.HisMedicine.DOCUMENT_PRICE = (long)Math.Round((this.currrentServiceAdo.IMP_AMOUNT * this.currrentServiceAdo.IMP_PRICE * (1 + this.currrentServiceAdo.IMP_VAT_RATIO)), 0, MidpointRounding.AwayFromZero);
                    //this.currrentServiceAdo.HisMedicine.DOCUMENT_PRICE = this.currrentServiceAdo.DOCUMENT_PRICE;
                    this.currrentServiceAdo.HisMedicine.PACKAGE_NUMBER = this.currrentServiceAdo.PACKAGE_NUMBER;
                    this.currrentServiceAdo.HisMedicine.EXPIRED_DATE = this.currrentServiceAdo.EXPIRED_DATE;
                    if (checkOutBid.Checked == true || (checkOutBid.Enabled == false && medicineProcessor.GetBidEnable(this.ucMedicineTypeTree) == false && medicineProcessor.GetBid(this.ucMedicineTypeTree) == null))
                    {
                        this.currrentServiceAdo.HisMedicine.TDL_BID_GROUP_CODE = this.currrentServiceAdo.TDL_BID_GROUP_CODE;
                        this.currrentServiceAdo.HisMedicine.TDL_BID_NUM_ORDER = this.currrentServiceAdo.TDL_BID_NUM_ORDER;
                        this.currrentServiceAdo.HisMedicine.TDL_BID_YEAR = this.currrentServiceAdo.TDL_BID_YEAR;
                        this.currrentServiceAdo.HisMedicine.TDL_BID_PACKAGE_CODE = this.currrentServiceAdo.TDL_BID_PACKAGE_CODE;
                        this.currrentServiceAdo.HisMedicine.TDL_BID_NUMBER = this.currrentServiceAdo.TDL_BID_NUMBER;
                    }

                    this.currrentServiceAdo.BidId = medicineProcessor.GetBid(this.ucMedicineTypeTree);
                    if (this.currentBid != null && this.currrentServiceAdo.BidId == null)
                    {
                        this.currrentServiceAdo.BidId = this.currentBid.ID;
                    }
                    List<string> lstRs = new List<string>();
                    lstRs.Add(this.currrentServiceAdo.TDL_BID_NUMBER);
                    lstRs.Add(this.currrentServiceAdo.TDL_BID_PACKAGE_CODE);
                    lstRs.Add(this.currrentServiceAdo.TDL_BID_GROUP_CODE);
                    lstRs.Add(this.currrentServiceAdo.TDL_BID_YEAR);
                    lstRs.Add(this.currrentServiceAdo.BidId.HasValue ? listBids.FirstOrDefault(o => o.ID == this.currrentServiceAdo.BidId).BID_APTHAU_CODE : "");

                    this.currrentServiceAdo.TT_THAU = this.currrentServiceAdo.HisMedicine.TT_THAU = string.Join(";", lstRs.Where(o => !string.IsNullOrEmpty(o)).ToList());

                    this.currrentServiceAdo.HisMedicine.MEDICAL_CONTRACT_ID = this.currrentServiceAdo.MEDICAL_CONTRACT_ID;
                    this.currrentServiceAdo.HisMedicine.CONTRACT_PRICE = this.currrentServiceAdo.CONTRACT_PRICE;

                    if (this.currentSupplierForEdit != null)
                        this.currrentServiceAdo.SupplierId = this.currentSupplierForEdit.ID;

                    if (chkImprice.Checked == true)
                    {
                        this.currrentServiceAdo.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = 1;
                    }
                    else
                    {
                        this.currrentServiceAdo.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = null;
                    }
                    // thinhdt
                    //check warning
                    // neu canh bao ma chon co -> thi them du lieu duoc nhap
                    // neu chon khong thi map du lieu lo thuoc/vat tu lay ra duoc

                    this.currrentServiceAdo.HisMedicine.MEDICINE_REGISTER_NUMBER = this.currrentServiceAdo.REGISTER_NUMBER;
                    this.currrentServiceAdo.HisMedicine.HEIN_SERVICE_BHYT_NAME = this.currrentServiceAdo.heinServiceBhytName;
                    this.currrentServiceAdo.HisMedicine.PACKING_TYPE_NAME = this.currrentServiceAdo.packingTypeName;

                    this.currrentServiceAdo.HisMedicine.NATIONAL_NAME = this.currrentServiceAdo.NATIONAL_NAME;
                    this.currrentServiceAdo.HisMedicine.CONCENTRA = this.currrentServiceAdo.CONCENTRA;

                    this.currrentServiceAdo.HisMedicine.MANUFACTURER_ID = this.currrentServiceAdo.MANUFACTURER_ID;


                    this.currrentServiceAdo.HisMedicine.ACTIVE_INGR_BHYT_NAME = this.currrentServiceAdo.activeIngrBhytName;
                    this.currrentServiceAdo.HisMedicine.DOSAGE_FORM = this.currrentServiceAdo.DOSAGE_FORM;
                    this.currrentServiceAdo.HisMedicine.MEDICINE_USE_FORM_ID = this.currrentServiceAdo.medicineUseFormId;

                    //                    if (this.currrentServiceAdo.HisMedicinePatys == null)
                    //                    {
                    this.currrentServiceAdo.HisMedicinePatys = new List<HIS_MEDICINE_PATY>();
                    //                    }


                    List<string> message1 = new List<string>();

                    foreach (var paty in this.currrentServiceAdo.VHisServicePatys)
                    {

                        if (!paty.IsNotSell)
                        {
                            var mediPaty = this.currrentServiceAdo.HisMedicinePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == paty.PATIENT_TYPE_ID);
                            if (this.currentContract != null && this.currentContract.ID > 0)
                            {
                                if (mediPaty != null)
                                {
                                    mediPaty.EXP_PRICE = (paty.PRICE / (1 + paty.ExpVatRatio / 100));

                                    mediPaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                }
                                else
                                {
                                    mediPaty = new HIS_MEDICINE_PATY();
                                    mediPaty.PATIENT_TYPE_ID = paty.PATIENT_TYPE_ID;
                                    mediPaty.EXP_PRICE = (paty.PRICE / (1 + paty.ExpVatRatio / 100));
                                    mediPaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                    if (tp == 1 || tp == 2 || tp == 3)
                                    {
                                        mediPaty.EXP_VAT_RATIO = 0;
                                        mediPaty.EXP_PRICE = paty.PRICE;
                                    }
                                    this.currrentServiceAdo.HisMedicinePatys.Add(mediPaty);
                                }
                            }
                            else
                            {
                                if (mediPaty != null)
                                {
                                    mediPaty.EXP_PRICE = paty.ExpPrice * (1 + (paty.PercentProfit / (decimal)100));
                                    mediPaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                }
                                else
                                {
                                    mediPaty = new HIS_MEDICINE_PATY();
                                    mediPaty.PATIENT_TYPE_ID = paty.PATIENT_TYPE_ID;
                                    mediPaty.EXP_PRICE = paty.ExpPrice * (1 + (paty.PercentProfit / (decimal)100));//nambg
                                    mediPaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                    if (tp == 1 || tp == 2 || tp == 3)
                                    {
                                        mediPaty.EXP_VAT_RATIO = 0;
                                        mediPaty.EXP_PRICE = paty.PRICE;
                                    }
                                    this.currrentServiceAdo.HisMedicinePatys.Add(mediPaty);
                                }
                            }


                            if (paty.PRICE != paty.PRE_PRICE_Str && paty.PRE_PRICE_Str > 0)
                            {

                                message1.Add(string.Format("{0} {1} có giá bán lần trước = {2} của đối tượng {3} khác với giá bán hiện tại.",
                                                 this.currrentServiceAdo.IsMedicine ? "Thuốc" : "Vật tư", this.currrentServiceAdo.MEDI_MATE_NAME, string.Format("{0:#,0}", paty.PRE_PRICE_Str), paty.PATIENT_TYPE_NAME));
                            }
                        }
                        //switch (tp)
                        //{
                        //    case 1:
                        //        paty.PRICE = Math.Round(paty.PRICE, (int)tp_);
                        //        paty.ExpPrice = Math.Round(paty.ExpPrice, (int)tp_);
                        //        break;
                        //    case 2:
                        //        paty.PRICE = RoundDown(paty.PRICE, tp_);
                        //        paty.ExpPrice = RoundDown(paty.ExpPrice, (int)tp_);
                        //        break;
                        //    case 3:
                        //        paty.PRICE = RoundUp(paty.PRICE, tp_);
                        //        paty.ExpPrice = RoundUp(paty.ExpPrice, (int)tp_);
                        //        break;

                        //}
                    }
                    this.currrentServiceAdo.WarningPrice = (message1 != null && message1.Count > 0) ? (string.Join("\n", message1) + "\n Bạn có muốn tiếp tục thêm ?") : "";
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currrentServiceAdo), currrentServiceAdo));
                    if (this.currrentServiceAdo.HisMedicine.IS_SALE_EQUAL_IMP_PRICE == null && this.currrentServiceAdo.HisMedicinePatys.Count == 0)
                    {
                        //WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Cần nhập chính sách giá hoặc tích giá bán bằng giá nhập", "Thông báo");
                        return;
                    }
                    else if (this.currrentServiceAdo.HisMedicine.IS_SALE_EQUAL_IMP_PRICE == null || this.currrentServiceAdo.HisMedicine.IS_SALE_EQUAL_IMP_PRICE == 1)
                    {
                        if (!string.IsNullOrEmpty(this.currrentServiceAdo.WarningPrice))
                        {
                            //WaitingManager.Hide();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(this.currrentServiceAdo.WarningPrice, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                return;
                            }
                        }
                    }

                }
                #endregion

                #region ------ VATTU -------
                else
                {
                    this.currrentServiceAdo.HisMaterial.INFORMATION_BID = this.cboInformationBid.SelectedIndex != -1 ? (long?)(this.cboInformationBid.SelectedIndex + 1) : null;
                    this.currrentServiceAdo.HisMaterial.AMOUNT = this.currrentServiceAdo.IMP_AMOUNT;
                    this.currrentServiceAdo.HisMaterial.IMP_PRICE = this.currrentServiceAdo.IMP_PRICE;
                    this.currrentServiceAdo.HisMaterial.DOCUMENT_PRICE = this.currrentServiceAdo.DOCUMENT_PRICE;
                    this.currrentServiceAdo.HisMaterial.DOCUMENT_PRICE = (long)Math.Round((this.currrentServiceAdo.IMP_AMOUNT * this.currrentServiceAdo.IMP_PRICE * (1 + this.currrentServiceAdo.IMP_VAT_RATIO)), 0, MidpointRounding.AwayFromZero);
                    this.currrentServiceAdo.HisMaterial.IMP_VAT_RATIO = this.currrentServiceAdo.IMP_VAT_RATIO;
                    this.currrentServiceAdo.HisMaterial.TAX_RATIO = this.currrentServiceAdo.TAX_RATIO;
                    this.currrentServiceAdo.HisMaterial.PACKAGE_NUMBER = this.currrentServiceAdo.PACKAGE_NUMBER;
                    this.currrentServiceAdo.HisMaterial.EXPIRED_DATE = this.currrentServiceAdo.EXPIRED_DATE;
                    if (checkOutBid.Checked == true || (checkOutBid.Enabled == false && materialProcessor.GetBidEnable(this.ucMaterialTypeTree) == false && materialProcessor.GetBid(this.ucMaterialTypeTree) == null))
                    {
                        this.currrentServiceAdo.HisMaterial.TDL_BID_GROUP_CODE = this.currrentServiceAdo.TDL_BID_GROUP_CODE;
                        this.currrentServiceAdo.HisMaterial.TDL_BID_NUM_ORDER = this.currrentServiceAdo.TDL_BID_NUM_ORDER;
                        this.currrentServiceAdo.HisMaterial.TDL_BID_YEAR = this.currrentServiceAdo.TDL_BID_YEAR;
                        this.currrentServiceAdo.HisMaterial.TDL_BID_PACKAGE_CODE = this.currrentServiceAdo.TDL_BID_PACKAGE_CODE;
                        this.currrentServiceAdo.HisMaterial.TDL_BID_NUMBER = this.currrentServiceAdo.TDL_BID_NUMBER;
                    }

                    this.currrentServiceAdo.TT_THAU = this.currrentServiceAdo.HisMaterial.TT_THAU = this.currrentServiceAdo.HisMaterial.INFORMATION_BID == 3 ? string.Format("{0}{1}", !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_EXTRA_CODE) ? this.currrentServiceAdo.TDL_BID_EXTRA_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_YEAR) ? this.currrentServiceAdo.TDL_BID_YEAR : null) : this.currrentServiceAdo.HisMaterial.INFORMATION_BID == 4 ? string.Format("{0}{1}{2}", !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_EXTRA_CODE) ? this.currrentServiceAdo.TDL_BID_EXTRA_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_PACKAGE_CODE) ? this.currrentServiceAdo.TDL_BID_PACKAGE_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_YEAR) ? this.currrentServiceAdo.TDL_BID_YEAR : null) : string.Format("{0}{1}{2}{3}", !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_EXTRA_CODE) ? this.currrentServiceAdo.TDL_BID_EXTRA_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_PACKAGE_CODE) ? this.currrentServiceAdo.TDL_BID_PACKAGE_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_GROUP_CODE) ? this.currrentServiceAdo.TDL_BID_GROUP_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_YEAR) ? this.currrentServiceAdo.TDL_BID_YEAR : null);

                    this.currrentServiceAdo.BidId = materialProcessor.GetBid(this.ucMaterialTypeTree);
                    if (this.currentBid != null && this.currrentServiceAdo.BidId == null)
                    {
                        this.currrentServiceAdo.BidId = this.currentBid.ID;
                    }

                    this.currrentServiceAdo.HisMaterial.MEDICAL_CONTRACT_ID = this.currrentServiceAdo.MEDICAL_CONTRACT_ID;
                    this.currrentServiceAdo.HisMaterial.CONTRACT_PRICE = this.currrentServiceAdo.CONTRACT_PRICE;

                    if (this.currentSupplierForEdit != null)
                        this.currrentServiceAdo.SupplierId = this.currentSupplierForEdit.ID;

                    if (chkImprice.Checked == true)
                    {
                        this.currrentServiceAdo.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = 1;
                    }
                    else
                    {
                        this.currrentServiceAdo.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = null;
                    }
                    // thinhdt
                    //check warning
                    // neu canh bao ma chon co -> thi them du lieu duoc nhap
                    // neu chon khong thi map du lieu lo thuoc/vat tu lay ra duoc
                    this.currrentServiceAdo.HisMaterial.MATERIAL_REGISTER_NUMBER = this.currrentServiceAdo.REGISTER_NUMBER;
                    this.currrentServiceAdo.HisMaterial.NATIONAL_NAME = this.currrentServiceAdo.NATIONAL_NAME;
                    this.currrentServiceAdo.HisMaterial.CONCENTRA = this.currrentServiceAdo.CONCENTRA;

                    this.currrentServiceAdo.HisMaterial.MANUFACTURER_ID = this.currrentServiceAdo.MANUFACTURER_ID;

                    this.currrentServiceAdo.HisMaterial.BID_MATERIAL_TYPE_CODE = this.currrentServiceAdo.packingTypeName;
                    this.currrentServiceAdo.HisMaterial.BID_MATERIAL_TYPE_NAME = this.currrentServiceAdo.heinServiceBhytName;

                    //                    if (this.currrentServiceAdo.HisMaterialPatys == null)
                    //                    {
                    this.currrentServiceAdo.HisMaterialPatys = new List<HIS_MATERIAL_PATY>();
                    //                    }
                    List<string> message2 = new List<string>();
                    foreach (var paty in this.currrentServiceAdo.VHisServicePatys)
                    {

                        if (!paty.IsNotSell)
                        {
                            if (this.currentContract != null && this.currentContract.ID > 0)
                            {
                                var matePaty = this.currrentServiceAdo.HisMaterialPatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == paty.PATIENT_TYPE_ID);
                                if (matePaty != null)
                                {
                                    matePaty.EXP_PRICE = (paty.PRICE / (1 + paty.ExpVatRatio / 100));

                                    matePaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                }
                                else
                                {
                                    matePaty = new HIS_MATERIAL_PATY();
                                    matePaty.PATIENT_TYPE_ID = paty.PATIENT_TYPE_ID;
                                    matePaty.EXP_PRICE = (paty.PRICE / (1 + paty.ExpVatRatio / 100));
                                    matePaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                    if (tp == 1 || tp == 2 || tp == 3)
                                    {
                                        matePaty.EXP_VAT_RATIO = 0;
                                        matePaty.EXP_PRICE = paty.PRICE;
                                    }
                                    this.currrentServiceAdo.HisMaterialPatys.Add(matePaty);
                                }
                            }
                            else
                            {
                                var matePaty = this.currrentServiceAdo.HisMaterialPatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == paty.PATIENT_TYPE_ID);
                                if (matePaty != null)
                                {
                                    matePaty.EXP_PRICE = (paty.IsReusable ? paty.PRICE : paty.ExpPrice) * (1 + (paty.PercentProfit / (decimal)100));
                                    matePaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                }
                                else
                                {
                                    matePaty = new HIS_MATERIAL_PATY();
                                    matePaty.PATIENT_TYPE_ID = paty.PATIENT_TYPE_ID;
                                    matePaty.EXP_PRICE = (paty.IsReusable ? paty.PRICE : paty.ExpPrice) * (1 + (paty.PercentProfit / (decimal)100));
                                    matePaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                    if (tp == 1 || tp == 2 || tp == 3)
                                    {
                                        matePaty.EXP_VAT_RATIO = 0;
                                        matePaty.EXP_PRICE = paty.PRICE;
                                    }
                                    this.currrentServiceAdo.HisMaterialPatys.Add(matePaty);
                                }
                            }
                            if (paty.PRICE != paty.PRE_PRICE_Str && paty.PRE_PRICE_Str > 0)
                            {
                                message2.Add(string.Format("{0} {1} có giá bán lần trước = {2} của đối tượng {3} khác với giá bán hiện tại.",
                                                this.currrentServiceAdo.IsMedicine ? "Thuốc" : "Vật tư", this.currrentServiceAdo.MEDI_MATE_NAME, string.Format("{0:#,0}", paty.PRE_PRICE_Str), paty.PATIENT_TYPE_NAME));
                            }
                        }
                        //switch (tp)
                        //{
                        //    case 1:
                        //        paty.PRICE = Math.Round(paty.PRICE, (int)tp_);
                        //        paty.ExpPrice = Math.Round(paty.ExpPrice, (int)tp_);
                        //        break;
                        //    case 2:
                        //        paty.PRICE = RoundDown(paty.PRICE, tp_);
                        //        paty.ExpPrice = RoundDown(paty.ExpPrice, (int)tp_);
                        //        break;
                        //    case 3:
                        //        paty.PRICE = RoundUp(paty.PRICE, tp_);
                        //        paty.ExpPrice = RoundUp(paty.ExpPrice, (int)tp_);
                        //        break;

                        //}

                    }
                    this.currrentServiceAdo.WarningPrice = (message2 != null && message2.Count > 0) ? (string.Join("\n", message2) + "\n Bạn có muốn tiếp tục thêm ?") : "";

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currrentServiceAdo), currrentServiceAdo));
                    if (this.currrentServiceAdo.HisMaterial.IS_SALE_EQUAL_IMP_PRICE == null && this.currrentServiceAdo.HisMaterialPatys.Count == 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Cần nhập chính sách giá hoặc tích giá bán bằng giá nhập", "Thông báo");
                        return;
                    }
                    else if (this.currrentServiceAdo.HisMaterial.IS_SALE_EQUAL_IMP_PRICE == null || this.currrentServiceAdo.HisMaterial.IS_SALE_EQUAL_IMP_PRICE == 1)
                    {
                        if (!string.IsNullOrEmpty(this.currrentServiceAdo.WarningPrice))
                        {
                            WaitingManager.Hide();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(this.currrentServiceAdo.WarningPrice, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                return;
                            }
                        }
                    }

                    if ((this.currrentServiceAdo.IsReusable || this.currrentServiceAdo.IsIdentity))
                    {
                        this.currrentServiceAdo.SerialNumbers = new List<ImpMestMaterialReusableSDO>();
                        if (this.IdentityMaterialOption != "2")
                        {
                            var serial = TxtSerialNumber.Text.Split(';');
                            long countSerial = serial.Count();
                            if (spinImpAmount.Value != countSerial)
                            {
                                //TODO  ktra lai co can hay k 
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng vật tư " + (this.currrentServiceAdo.IsReusable ? "tái sử dụng" : "đích danh") + " khác số lượng số seri được tạo", Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                                TxtSerialNumber.Focus();
                                return;
                            }

                            foreach (var itemStr in serial)
                            {
                                ImpMestMaterialReusableSDO sdo = new ImpMestMaterialReusableSDO();
                                sdo.ReusCount = (long)SpMaxReuseCount.Value;
                                sdo.SerialNumber = itemStr;
                                this.currrentServiceAdo.SerialNumbers.Add(sdo);
                            }
                            this.currrentServiceAdo.SERIAL_NUMBER = TxtSerialNumber.Text;
                        }
                        else
                        {
                            //vật tư đích danh và tái sử dụng luôn có chi tiết nhập là 1
                            //để có chi tiết nhập là 1 thì cần truyền lên serial rỗng để backend tạo
                            ImpMestMaterialReusableSDO sdo = new ImpMestMaterialReusableSDO();
                            sdo.ReusCount = (long)SpMaxReuseCount.Value;
                            this.currrentServiceAdo.SerialNumbers.Add(sdo);
                            if (this.currrentServiceAdo.IMP_AMOUNT > 1)
                            {
                                for (int i = 1; i < currrentServiceAdo.IMP_AMOUNT; i++)
                                {
                                    ImpMestMaterialReusableSDO sdo1 = new ImpMestMaterialReusableSDO();
                                    sdo1.ReusCount = (long)SpMaxReuseCount.Value;
                                    this.currrentServiceAdo.SerialNumbers.Add(sdo1);
                                }
                            }
                        }
                        if (SpMaxReuseCount.EditValue != null)
                            this.currrentServiceAdo.MAX_REUSE_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(SpMaxReuseCount.EditValue.ToString() ?? "");
                        if (SpPricingMaxReuseCount.EditValue != null)
                            this.currrentServiceAdo.PRICING_MAX_REUSE_COUNT = SpPricingMaxReuseCount.Value;
                        this.currrentServiceAdo.VS_PRICE = (decimal)spinEditGiaVeSinh.Value;
                    }
                }
                #endregion

                listServiceADO.Add(this.currrentServiceAdo);
                Inventec.Common.Logging.LogSystem.Debug("1__________AAA" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listServiceADO), listServiceADO));
                gridControlImpMestDetail.BeginUpdate();
                gridControlImpMestDetail.DataSource = listServiceADO;
                gridControlImpMestDetail.EndUpdate();
                dxValidationProvider2.RemoveControlError(dtExpiredDate);
                dxValidationProvider2.RemoveControlError(txtExpiredDate);
                CalculTotalPrice();
                SetEnableControlCommonAdd();
                ResetValueControlDetail(true);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.listServiceADO.Count <= 0 ||
                !this.CheckDocumentNumberV2(txtDocumentNumber.Text.Trim(), txtkyHieuHoaDon.Text))
                    return;

                //Check thieu chinh sách giá
                var checkThieuChinhSachGia = this.listServiceADO.Where(o => o.CheckImportThieuChinhSachGia && !o.BanBangGiaNhap).ToList();
                if (checkThieuChinhSachGia != null && checkThieuChinhSachGia.Count > 0)
                {
                    var message = "Thuốc/vật tư có mã: " + string.Join(",", checkThieuChinhSachGia.Select(o => o.MEDI_MATE_CODE)) + " thiếu chính sách giá";
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo");
                    return;
                }

                long impMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboImpMestType.EditValue ?? "0").ToString());
                if (impMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    var checkDates = this.listServiceADO.Where(o => o.EXPIRED_DATE != null).ToList();
                    foreach (var itemDate in checkDates)
                    {
                        DateTime? _EXPIRED_DATE = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(itemDate.EXPIRED_DATE ?? 0);

                        if (_EXPIRED_DATE < DateTime.Now)
                        {
                            string _tittle = "";
                            if (itemDate.IsMedicine)
                                _tittle = Base.ResourceMessageManager.TieuDeThuoc;
                            else
                                _tittle = Base.ResourceMessageManager.TieuDeVatTu;

                            string mess = _tittle + " <color=red>" + itemDate.MEDI_MATE_CODE + "</color> " + Base.ResourceMessageManager.TieuDeDaHetHanSuDung + "\n" + Base.ResourceMessageManager.TieuDeBanCoMuonTiepTuc;
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                return;
                            }
                        }

                        if (CheckLifespan(itemDate))
                        {
                            return;
                        }
                    }
                }

                var checkData = this.listServiceADO.Where(o => o.Errors != null && o.Errors.Count > 0).ToList();
                if (checkData != null && checkData.Count > 0)
                {
                    string message = "";
                    foreach (var item in checkData)
                    {
                        message += "Thuốc/vật tư (" + item.MEDI_MATE_CODE + ") ";
                        string check = "";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.SaiGiaNhap))
                            check += "sai giá nhập,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.SaiSoLuong))
                            check += "sai số lượng,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.SaiVat))
                            check += "sai Vat,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.ThieuGiaNhap))
                            check += "thiếu giá nhập,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.ThieuVat))
                            check += "thiếu Vat,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.ThieuSoLuong))
                            check += "thiếu số lượng,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLengthGoiThau))
                            check += "gói thầu vượt quá ký tự cho phép,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLengthNhomThau))
                            check += "nhóm thầu vượt quá ký tự cho phép,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLengthMaHangSX))
                            check += "mã hãng sản xuất vượt quá ký tự cho phép,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLengthQuocGia))
                            check += "quốc gia vượt quá ký tự cho phép,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLengthSoDangKy))
                            check += "số đăng ký vượt quá ký tự cho phép,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.MaxLenthNongDoHamLuong))
                            check += "nồng độ/ hàm lượng vượt quá ký tự cho phép,";
                        if (item.Errors.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Error.ThieuDangBaoChe))
                            check += "Dạng bào chế không được để trống,";
                        message += check + "\n";
                    }
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo");
                    return;
                }

                var checkWarm = this.listServiceADO.Where(o => o.Warms != null && o.Warms.Count > 0).ToList();
                if (checkWarm != null && checkWarm.Count > 0)
                {
                    string message = "";
                    foreach (var item in checkWarm)
                    {
                        message += "Thuốc/vật tư (" + item.MEDI_MATE_CODE + ") ";
                        string check = "";
                        if (item.Warms.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.HanDungKhongHopLe))
                            check += "hạn sử dụng không hợp lệ,";
                        if (item.Warms.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.KhongTonTai))
                            check += "mã đường dùng không tồn tại,";
                        if (item.Warms.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.ThangKhongHopLe))
                            check += "tuổi thọ tháng không hợp lệ,";
                        if (item.Warms.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.TuoiThoThangVuotQuaDoDaiChoPhep))
                            check += "tuổi thọ tháng vượt quá độ dài cho phép (19),";
                        if (item.Warms.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.NgayKhongHopLe))
                            check += "tuổi thọ ngày không hợp lệ,";
                        if (item.Warms.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.TuoiThoNgayVuotQuaDoDaiChoPhep))
                            check += "tuổi thọ ngày vượt quá độ dài cho phép (19),";
                        if (item.Warms.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.GioKhongHopLe))
                            check += "tuổi thọ giờ không hợp lệ,";
                        if (item.Warms.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.TuoiThoGioVuotQuaDoDaiChoPhep))
                            check += "tuổi thọ giờ vượt quá độ dài cho phép (19),";
                        if (item.Warms.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.KhongCoTuoiTho))
                            check += "không có tuổi thọ,";
                        if (item.Warms.Exists(o => o == HIS.Desktop.Plugins.ImpMestCreate.ADO.VHisServiceADO.Warm.KhongHopLe))
                            check += "có tuổi thọ không hợp lệ,";
                        message += check + "\n";
                    }
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, "Cảnh báo");
                }

                bool success = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                HIS.Desktop.Plugins.ImpMestCreate.Save.ISaveInit iSaveInit = HIS.Desktop.Plugins.ImpMestCreate.Save.SaveFactory.MakeIServiceRequestRegister(param, this.listServiceADO, this, dicBidMedicine, dicBidMaterial, this.roomId, this.resultADO);
                this.resultADO = iSaveInit.Run() as ResultImpMestADO;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO), resultADO));
                //WaitingManager.Hide();
                if (this.resultADO != null)
                {
                    this._currentImpMestUp = this.resultADO.ImpMestUpdate;
                    success = true;
                }

                if (success)
                {
                    if (this.resultADO.HisManuSDO != null && this.resultADO.HisManuSDO.ImpMest != null)
                    {
                        txtImpMestCode.Text = this.resultADO.HisManuSDO.ImpMest.IMP_MEST_CODE;
                    }
                    else if (this.resultADO.HisInitSDO != null && this.resultADO.HisInitSDO.ImpMest != null)
                    {
                        txtImpMestCode.Text = this.resultADO.HisInitSDO.ImpMest.IMP_MEST_CODE;
                    }
                    else if (this.resultADO.HisInveSDO != null && this.resultADO.HisInveSDO.ImpMest != null)
                    {
                        txtImpMestCode.Text = this.resultADO.HisInveSDO.ImpMest.IMP_MEST_CODE;
                    }
                    else if (this.resultADO.HisOtherSDO != null && this.resultADO.HisOtherSDO.ImpMest != null)
                    {
                        txtImpMestCode.Text = this.resultADO.HisOtherSDO.ImpMest.IMP_MEST_CODE;
                    }

                    btnHoiDongKiemNhap.Enabled = true;
                    btnSaveDraft.Enabled = false;
                    btnSave.Enabled = false;
                    dropDownButton__Print.Enabled = true;
                    InitMenuToButtonPrint(this.resultADO);
                    isSave = true;
                }
                BackendDataWorker.Reset<V_HIS_MATERIAL>(); 
                BackendDataWorker.Reset<V_HIS_MEDICINE>();
                MessageManager.Show(this.ParentForm, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate() || this.listServiceADO.Count <= 0)
                    return;
                if (CheckDocumentNumber(txtDocumentNumber.Text, txtkyHieuHoaDon.Text))
                {
                    WaitingManager.Hide();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.SoChungTuDaDuocSuDung, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_IMP_MEST_TYPE impMestType = null;
                if (cboImpMestType.EditValue != null)
                {
                    impMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMestType.EditValue));
                }
                if (impMestType == null)
                    return;

                var sdo = getImpMestTypeSDORequest(impMestType, IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT);
                if (sdo != null)
                {
                    if (sdo.GetType() == typeof(HisImpMestManuSDO))
                    {
                        HisImpMestManuSDO rs = null;
                        if (resultADO == null)
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestManuSDO>("api/HisImpMest/ManuCreate", ApiConsumers.MosConsumer, sdo, param);
                        }
                        else
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestManuSDO>("api/HisImpMest/ManuUpdate", ApiConsumers.MosConsumer, sdo, param);
                        }
                        if (rs != null)
                        {
                            success = true;
                            this.resultADO = new ResultImpMestADO(rs);
                            txtImpMestCode.Text = rs.ImpMest.IMP_MEST_CODE;
                        }
                    }
                    else if (sdo.GetType() == typeof(HisImpMestInitSDO))
                    {
                        HisImpMestInitSDO rs = null;
                        if (resultADO == null)
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInitSDO>("api/HisImpMest/InitCreate", ApiConsumers.MosConsumer, sdo, param);
                        }
                        else
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInitSDO>("api/HisImpMest/InitUpdate", ApiConsumers.MosConsumer, sdo, param);
                        }
                        if (rs != null)
                        {
                            success = true;
                            this.resultADO = new ResultImpMestADO(rs);
                            txtImpMestCode.Text = rs.ImpMest.IMP_MEST_CODE;
                        }
                    }
                    else if (sdo.GetType() == typeof(HisImpMestInveSDO))
                    {
                        HisImpMestInveSDO rs = null;
                        if (resultADO == null)
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInveSDO>("api/HisImpMest/InveCreate", ApiConsumers.MosConsumer, sdo, param);
                        }
                        else
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInveSDO>("api/HisImpMest/InveUpdate", ApiConsumers.MosConsumer, sdo, param);
                        }
                        if (rs != null)
                        {
                            success = true;
                            this.resultADO = new ResultImpMestADO(rs);
                            txtImpMestCode.Text = rs.ImpMest.IMP_MEST_CODE;
                        }
                    }
                    else if (sdo.GetType() == typeof(HisImpMestOtherSDO))
                    {
                        HisImpMestOtherSDO rs = null;
                        if (resultADO == null)
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestOtherSDO>("api/HisImpMest/OtherCreate", ApiConsumers.MosConsumer, sdo, param);
                        }
                        else
                        {
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestOtherSDO>("api/HisImpMest/OtherUpdate", ApiConsumers.MosConsumer, sdo, param);
                        }
                        if (rs != null)
                        {
                            success = true;
                            this.resultADO = new ResultImpMestADO(rs);
                            txtImpMestCode.Text = rs.ImpMest.IMP_MEST_CODE;
                        }
                    }
                    if (success)
                    {
                        this.ProcessSaveSuccess();
                        dropDownButton__Print.Enabled = true;
                        btnSaveDraft.Enabled = false;
                        btnSave.Enabled = true;
                        btnImportExcel.Enabled = false;
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

                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string messageError = "";
                if (!btnImportExcel.Enabled)
                    return;
                if (!CheckAllowAdd(ref messageError))
                {
                    MessageManager.Show(messageError);
                    return;
                }
                if (!CheckExpiredDate())
                    return;

                CommonParam param = new CommonParam();
                bool success = false;
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() != DialogResult.OK || !(ofd.FileName.EndsWith(".xlsx")))
                    return;
                WaitingManager.Show();
                import = new Inventec.Common.ExcelImport.Import();
                if (import.ReadFileExcel(ofd.FileName))
                {
                    var listImport = import.Get<ImportMediMateADO>(0);
                    if (listImport == null || listImport.Count == 0)
                    {
                        param.Messages.Add(Base.ResourceMessageManager.DuLieuDocTuFileExcelRong);
                    }
                    else
                    {
                        success = true;
                        ProcessListImportADO(ref success, ref param, listImport);
                        SetEnableControlCommon();
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled)
                    return;
                WaitingManager.Show();
                this._currentImpMestUp = null;
                ColorBlack();
                dxValidationProvider2.RemoveControlError(dtExpiredDate);
                dxValidationProvider2.RemoveControlError(txtExpiredDate);
                dxValidationProvider2.RemoveControlError(spinImpAmount);
                dxValidationProvider2.RemoveControlError(cboGoiThau);
                dxValidationProvider2.RemoveControlError(txtBidGroupCode);
                dxValidationProvider2.RemoveControlError(txtBidNumber);
                dxValidationProvider2.RemoveControlError(txtBidYear);
                dxValidationProvider2.RemoveControlError(txtBidNumOrder);
                dxValidationProvider2.RemoveControlError(txtBid);
                dxValidationProvider2.RemoveControlError(SpMaxReuseCount);
                dxValidationProvider2.RemoveControlError(TxtSerialNumber);
                dxValidationProvider2.RemoveControlError(spinEditGiaVeSinh);
                dxValidationProvider1.RemoveControlError(dtDocumentDate);
                dxValidationProvider1.RemoveControlError(txtDocumentDate);
                dxValidationProvider1.RemoveControlError(txtDocumentNumber);
                dxValidationProvider2.RemoveControlError(txtPackageNumber);
                dxValidationProvider2.RemoveControlError(txtHeinServiceBidMateType);

                this.currentContract = null;
                SetValueByContractMety(null);
                SetValueByContractMaty(null);
                ResetValueControlDetail();
                ResetValueControlCommon();
                SetEnableControlCommon();
                SetDefaultImpMestType();
                SetControlEnableImMestTypeManu();
                LoadDataToComboSupplier(listSupplier);
                SetDefaultValueMediStock();
                SetFocuTreeMediMate();
                cboImpMestType.Focus();
                //btnHoiDongKiemNhap.Enabled = true;
                btnSave.Enabled = true;
                btnSaveDraft.Enabled = true;
                txtNhaCC.EditValue = null;
                this.currentSupplierForEdit = null;
                medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, null);

                medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);
                materialProcessor.SetEditValueContract(this.ucMaterialTypeTree, null);
                this.layoutControlItem7.Text = "Giá trong thầu";
                spinImpVatRatio.Enabled = true;
                SetEnableButton(false);
                resetKeyWord();
                this.currentBid = null;
                SetDataSourceGridControlMediMate();
                this._HisBidBySuppliers = null;
                this.cboGoiThau.EditValue = null;
                this.oldSupplierId = 0;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUpdate1_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnUpdate1.Enabled || !dxValidationProvider2.Validate() || this.currrentServiceAdo == null)
                    return;

                if (!CheckBhytMedicineInfo())
                {
                    return;
                }

                //Kiểm tra ngoài gói thầu
                if (!CheckOutBid(this.currrentServiceAdo))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.ThuocVatTuKhongNamTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        return;
                }

                //Kiểm tra trong gói thầu và số lượng khả nhập
                if (this.currentContract == null && this.CheckInBid(this.currrentServiceAdo) && spinCanImpAmount.Value < spinImpAmount.Value)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.SoLuongNhapLonHonSoLuongKhaNhapTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                }

                //Kiểm tra giá nhập, vat nhập so với giá và vat trong gói thầu
                if ((this.currrentServiceAdo.BidImpPrice.HasValue && this.currrentServiceAdo.BidImpPrice.Value != spinImpPrice1.Value) && (this.currrentServiceAdo.BidImpVatRatio.HasValue && this.currrentServiceAdo.BidImpVatRatio.Value != (spinImpVatRatio.Value / 100)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.GiaVatNhapKhacVoiGiaVatNhapTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                }
                else if (this.currrentServiceAdo.BidImpPrice.HasValue && this.currrentServiceAdo.BidImpPrice.Value != spinImpPrice1.Value)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.GiaNhapKhacVoiGiaNhapTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                }

                if ((this.currrentServiceAdo.BidImpPrice.HasValue && this.currrentServiceAdo.BidImpPrice.Value != spinImpPrice.Value) && (this.currrentServiceAdo.BidImpVatRatio.HasValue && this.currrentServiceAdo.BidImpVatRatio.Value != (spinImpVatRatio.Value / 100)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.GiaVatNhapKhacVoiGiaVatNhapTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                }
                else if (this.currrentServiceAdo.BidImpPrice.HasValue && this.currrentServiceAdo.BidImpPrice.Value != spinImpPrice.Value)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.GiaNhapKhacVoiGiaNhapTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                }
                else if (this.currrentServiceAdo.BidImpVatRatio.HasValue && this.currrentServiceAdo.BidImpVatRatio.Value != (spinImpVatRatio.Value / 100))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.VatNhapKhacVoiVatNhapTrongGoiThau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                }

                if (!CheckExpiredDate())
                {
                    return;
                }

                if (this.currentContract != null)
                {

                    if (this.MedicalContractMety != null && xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                    {
                        if (spinCanImpAmount.Value < spinImpAmount.Value)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.SoLuongNhapLonHonSoLuongTrongHopDong, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                            return;
                        }

                        if (spinImpPriceVAT.Value > spinEditGiaTrongThau.Value)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.GiaSauVatLonHonGiaHopDong, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                            return;
                        }
                    }

                    if (this.MedicalContractMaty != null && xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                    {
                        if (spinCanImpAmount.Value < spinImpAmount.Value)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.SoLuongNhapLonHonSoLuongTrongHopDong, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                            return;
                        }

                        if (spinImpPriceVAT.Value > spinEditGiaTrongThau.Value)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.GiaSauVatLonHonGiaHopDong, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                            return;
                        }
                    }
                }

                long impMestTypeIdV2 = Inventec.Common.TypeConvert.Parse.ToInt64((cboImpMestType.EditValue ?? "0").ToString());
                if (impMestTypeIdV2 == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    long timeHieuLucTu = dtHieuLucTu.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHieuLucTu.DateTime) ?? 0 : 0;
                    long timeHieuLucDen = dtHieuLucDen.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHieuLucDen.DateTime) ?? 0 : 0;
                    long dateNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    bool _21299 = false;
                    if (timeHieuLucTu > 0 && timeHieuLucDen > 0)
                    {
                        if (dateNow < timeHieuLucTu || dateNow > timeHieuLucDen)
                            _21299 = true;
                    }
                    else if (timeHieuLucTu > 0 && timeHieuLucDen == 0)
                    {
                        if (dateNow < timeHieuLucTu)
                            _21299 = true;
                    }
                    else if (timeHieuLucDen > 0 && timeHieuLucTu == 0)
                    {
                        if (dateNow > timeHieuLucDen)
                            _21299 = true;
                    }
                    if (_21299)
                    {
                        string _showMess = string.Format("Đã ngoài thời gian hiệu lực thầu ({0} -- {1}). Bạn có muốn thực hiện không?", Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(dtHieuLucTu.DateTime), Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(dtHieuLucDen.DateTime));
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(_showMess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            return;
                    }
                }

                if (ShowMessValidDocumentAndDate())
                    return;

                if (!IsCheckTemperature())
                {
                    return;
                }

                //WaitingManager.Show();

                
                this.currrentServiceAdo.TEMPERATURE = spnTemperature.EditValue != null ? (decimal?)spnTemperature.Value : null;

                this.currrentServiceAdo.TDL_BID_GROUP_CODE = txtBidGroupCode.Text;
                this.currrentServiceAdo.TDL_BID_NUM_ORDER = txtBidNumOrder.Text;
                this.currrentServiceAdo.TDL_BID_YEAR = txtBidYear.Text;
                this.currrentServiceAdo.TDL_BID_NUMBER = txtBidNumber.Text;
                this.currrentServiceAdo.TDL_BID_EXTRA_CODE = txtBidExtraCode.Text;
                this.currrentServiceAdo.TDL_BID_PACKAGE_CODE = txtBid.Text;

                this.currrentServiceAdo.packingTypeName = this.txtPackingJoinBid.Text;
                this.currrentServiceAdo.heinServiceBhytName = this.txtHeinServiceBidMateType.Text;
                this.currrentServiceAdo.activeIngrBhytName = this.txtActiveIngrBhytName.Text;
                //this.currrentServiceAdo.dosageForm = this.cboDosageForm.Text;
                if (this.cboMedicineUseForm.EditValue != null)
                {
                    this.currrentServiceAdo.medicineUseFormId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboMedicineUseForm.EditValue.ToString());
                }

                this.currrentServiceAdo.IMP_AMOUNT = spinImpAmount.Value;
                if (spinImpPrice1.Enabled && spinImpPrice1.Visible)
                {
                    this.currrentServiceAdo.IMP_PRICE = spinImpPrice1.Value;
                }
                if (spinImpPrice.Enabled && spinImpPrice.Visible)
                {
                    this.currrentServiceAdo.IMP_PRICE = spinImpPrice.Value;
                }
                this.currrentServiceAdo.IMP_VAT_RATIO = spinImpVatRatio.Value / 100;
                this.currrentServiceAdo.TAX_RATIO = spinEditThueXuat.Value / 100;
                this.currrentServiceAdo.Errors = null;
                this.currrentServiceAdo.ImpVatRatio = spinImpVatRatio.Value;

                this.currrentServiceAdo.DOCUMENT_PRICE = Inventec.Common.TypeConvert.Parse.ToInt64((spinEditTTCoVAT.Value).ToString());

                if (dtExpiredDate.EditValue != null && dtExpiredDate.DateTime != DateTime.MinValue)
                {
                    this.currrentServiceAdo.EXPIRED_DATE = Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "235959");
                    DateTime? _EXPIRED_DATE = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currrentServiceAdo.EXPIRED_DATE ?? 0);
                    long impMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboImpMestType.EditValue ?? "0").ToString());
                    if (_EXPIRED_DATE < DateTime.Now && impMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                    {
                        string _tittle = "";
                        if (this.currrentServiceAdo.IsMedicine)
                            _tittle = Base.ResourceMessageManager.TieuDeThuoc;
                        else
                            _tittle = Base.ResourceMessageManager.TieuDeVatTu;

                        string mess = _tittle + " <color=red>" + this.currrentServiceAdo.MEDI_MATE_CODE + "</color> " + Base.ResourceMessageManager.TieuDeDaHetHanSuDung + "\n" + Base.ResourceMessageManager.TieuDeBanCoMuonTiepTuc;
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return;
                        }
                    }
                }

                if (CheckLifespan(this.currrentServiceAdo))
                {
                    return;
                }

                if (!String.IsNullOrWhiteSpace(txtPackageNumber.Text))
                {
                    this.currrentServiceAdo.PACKAGE_NUMBER = txtPackageNumber.Text;
                }
                else
                {
                    this.currrentServiceAdo.PACKAGE_NUMBER = null;
                }
                AutoMapper.Mapper.CreateMap<VHisServicePatyADO, VHisServicePatyADO>();
                this.currrentServiceAdo.VHisServicePatys = AutoMapper.Mapper.Map<List<VHisServicePatyADO>>(listServicePatyAdo);

                if (listServicePatyAdo == null)
                {
                    this.currrentServiceAdo.VHisServicePatys = new List<VHisServicePatyADO>();
                }

                if (this.currrentServiceAdo.VHisServicePatys.Count == 0)
                {
                    this.currrentServiceAdo.HisMedicinePatys = null;
                    this.currrentServiceAdo.HisMaterialPatys = null;
                }
                List<VHisServicePatyADO> CheckData = new List<VHisServicePatyADO>();

                if (listServicePatyAdo != null)
                {
                    CheckData = this.currrentServiceAdo.VHisServicePatys.Where(o => o.IsNotSell == false).ToList();
                }

                this.currrentServiceAdo.NATIONAL_NAME = this.txtNationalMainText.Text.Trim();
                this.currrentServiceAdo.CONCENTRA = this.txtNognDoHL.Text;
                this.currrentServiceAdo.REGISTER_NUMBER = this.txtSoDangKy.Text;
                
                
                if (cboHangSX.EditValue != null)
                {
                    this.currrentServiceAdo.MANUFACTURER_ID = (long)cboHangSX.EditValue;
                }
                else
                    this.currrentServiceAdo.MANUFACTURER_ID = null;

                if (this.currrentServiceAdo.IsMedicine)
                {
                    if (CheckData.Count == 0 && chkImprice.Checked == false)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Cần nhập chính sách giá hoặc tích giá bán bằng giá nhập", "Thông báo");
                        return;
                    }
                    if (chkImprice.Checked == true)
                    {
                        this.currrentServiceAdo.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = 1;
                        if (this.currrentServiceAdo.GiaBan > 0 && this.currrentServiceAdo.IMP_PRICE_PREVIOUS != this.currrentServiceAdo.GiaBan)
                        {
                            this.currrentServiceAdo.WarningPrice = string.Format("{0} {1} có giá bán lần trước = {2} khác với giá bán hiện tại. Bạn có muốn tiếp tục thêm ?",
                                this.currrentServiceAdo.IsMedicine ? "Thuốc" : "Vật tư", this.currrentServiceAdo.MEDI_MATE_NAME, string.Format("{0:#,0}", this.currrentServiceAdo.GiaBan));

                            //WaitingManager.Hide();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(this.currrentServiceAdo.WarningPrice, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                return;
                            }
                        }

                    }
                    else
                    {
                        this.currrentServiceAdo.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = null;
                    }

                    
                    this.currrentServiceAdo.HisMedicine.AMOUNT = this.currrentServiceAdo.IMP_AMOUNT;
                    this.currrentServiceAdo.HisMedicine.IMP_PRICE = this.currrentServiceAdo.IMP_PRICE;
                    this.currrentServiceAdo.HisMedicine.DOCUMENT_PRICE = this.currrentServiceAdo.DOCUMENT_PRICE;
                    this.currrentServiceAdo.HisMedicine.IMP_VAT_RATIO = this.currrentServiceAdo.IMP_VAT_RATIO;
                    this.currrentServiceAdo.HisMedicine.TAX_RATIO = this.currrentServiceAdo.TAX_RATIO;
                    this.currrentServiceAdo.HisMedicine.PACKAGE_NUMBER = this.currrentServiceAdo.PACKAGE_NUMBER;
                    this.currrentServiceAdo.HisMedicine.EXPIRED_DATE = this.currrentServiceAdo.EXPIRED_DATE;

                    this.currrentServiceAdo.HisMedicine.TDL_BID_GROUP_CODE = this.currrentServiceAdo.TDL_BID_GROUP_CODE;
                    this.currrentServiceAdo.HisMedicine.TDL_BID_NUM_ORDER = this.currrentServiceAdo.TDL_BID_NUM_ORDER;
                    this.currrentServiceAdo.HisMedicine.TDL_BID_PACKAGE_CODE = this.currrentServiceAdo.TDL_BID_PACKAGE_CODE;

                    this.currrentServiceAdo.HisMedicine.NATIONAL_NAME = this.currrentServiceAdo.NATIONAL_NAME;
                    this.currrentServiceAdo.HisMedicine.CONCENTRA = this.currrentServiceAdo.CONCENTRA;
                    this.currrentServiceAdo.HisMedicine.MEDICINE_REGISTER_NUMBER = this.currrentServiceAdo.REGISTER_NUMBER;
                    this.currrentServiceAdo.HisMedicine.MANUFACTURER_ID = this.currrentServiceAdo.MANUFACTURER_ID;

                    this.currrentServiceAdo.HisMedicine.PACKING_TYPE_NAME = this.currrentServiceAdo.packingTypeName;
                    this.currrentServiceAdo.HisMedicine.HEIN_SERVICE_BHYT_NAME = this.currrentServiceAdo.heinServiceBhytName;
                    this.currrentServiceAdo.HisMedicine.ACTIVE_INGR_BHYT_NAME = this.currrentServiceAdo.activeIngrBhytName;
                    if (cboDosageForm.EditValue != null)
                    {
                        this.currrentServiceAdo.DOSAGE_FORM = cboDosageForm.Text;
                        this.currrentServiceAdo.HisMedicine.DOSAGE_FORM = this.currrentServiceAdo.DOSAGE_FORM;
                    }
                    else if (this.currrentServiceAdo.MEDICINE_LINE_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT)
                    {
                        XtraMessageBox.Show("Dạng bào chế không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    //this.currrentServiceAdo.HisMedicine.DOSAGE_FORM = this.currrentServiceAdo.DOSAGE_FORM;

                    this.currrentServiceAdo.HisMedicine.MEDICINE_USE_FORM_ID = this.currrentServiceAdo.medicineUseFormId;

                    List<string> lstRs = new List<string>();
                    lstRs.Add(this.currrentServiceAdo.TDL_BID_NUMBER);
                    lstRs.Add(this.currrentServiceAdo.TDL_BID_PACKAGE_CODE);
                    lstRs.Add(this.currrentServiceAdo.TDL_BID_GROUP_CODE);
                    lstRs.Add(this.currrentServiceAdo.TDL_BID_YEAR);
                    lstRs.Add(this.currrentServiceAdo.BidId.HasValue ? listBids.FirstOrDefault(o => o.ID == this.currrentServiceAdo.BidId).BID_APTHAU_CODE : "");

                    this.currrentServiceAdo.TT_THAU = this.currrentServiceAdo.HisMedicine.TT_THAU = string.Join(";", lstRs.Where(o => !string.IsNullOrEmpty(o)).ToList());
                    if (this.currrentServiceAdo.HisMedicinePatys == null)
                    {
                        this.currrentServiceAdo.HisMedicinePatys = new List<HIS_MEDICINE_PATY>();
                    }

                    List<string> mes1 = new List<string>();
                    foreach (var paty in this.currrentServiceAdo.VHisServicePatys)
                    {
                        if (!paty.IsNotSell)
                        {
                            var mediPaty = this.currrentServiceAdo.HisMedicinePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == paty.PATIENT_TYPE_ID);
                            if (mediPaty != null)
                            {
                                mediPaty.EXP_PRICE = paty.PRICE / (1 + paty.VAT_RATIO); //nambg
                                mediPaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                            }
                            else
                            {
                                mediPaty = new HIS_MEDICINE_PATY();
                                mediPaty.PATIENT_TYPE_ID = paty.PATIENT_TYPE_ID;
                                mediPaty.EXP_PRICE = paty.PRICE / (1 + paty.VAT_RATIO);
                                mediPaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                this.currrentServiceAdo.HisMedicinePatys.Add(mediPaty);
                            }
                            if (paty.PRICE != paty.PRE_PRICE_Str && paty.PRE_PRICE_Str > 0)
                            {
                                mes1.Add(string.Format("{0} {1} có giá bán lần trước = {2} của đối tượng {3} khác với giá bán hiện tại.",
                                                this.currrentServiceAdo.IsMedicine ? "Thuốc" : "Vật tư", this.currrentServiceAdo.MEDI_MATE_NAME, string.Format("{0:#,0}", paty.PRE_PRICE_Str), paty.PATIENT_TYPE_NAME));
                            }
                        }
                    }
                    this.currrentServiceAdo.WarningPrice = (mes1 != null && mes1.Count > 0) ? (string.Join("\n", mes1) + "\n Bạn có muốn tiếp tục thêm ?") : "";
                    if (CheckData.Count != 0 && chkImprice.Checked == false)
                    {
                        if (!string.IsNullOrEmpty(this.currrentServiceAdo.WarningPrice))
                        {
                            //WaitingManager.Hide();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(this.currrentServiceAdo.WarningPrice, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                return;
                            }
                        }
                    }

                }
                else
                {
                    if (CheckData.Count == 0 && chkImprice.Checked == false)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Cần nhập chính sách giá hoặc tích giá bán bằng giá nhập", "Thông báo");
                        return;
                    }

                    if (chkImprice.Checked == true)
                    {
                        this.currrentServiceAdo.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = 1;
                        if (this.currrentServiceAdo.GiaBan > 0 && this.currrentServiceAdo.IMP_PRICE_PREVIOUS != this.currrentServiceAdo.GiaBan)
                        {
                            this.currrentServiceAdo.WarningPrice = string.Format("{0} {1} có giá bán lần trước = {2} khác với giá bán hiện tại. Bạn có muốn tiếp tục thêm ?",
                                this.currrentServiceAdo.IsMedicine ? "Thuốc" : "Vật tư", this.currrentServiceAdo.MEDI_MATE_NAME, string.Format("{0:#,0}", this.currrentServiceAdo.GiaBan));

                            //WaitingManager.Hide();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(this.currrentServiceAdo.WarningPrice, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                return;
                            }
                        }

                        this.currrentServiceAdo.BanBangGiaNhap = true;
                    }
                    else
                    {
                        this.currrentServiceAdo.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = null;
                        this.currrentServiceAdo.BanBangGiaNhap = false;
                    }

                    this.currrentServiceAdo.HisMaterial.AMOUNT = this.currrentServiceAdo.IMP_AMOUNT;
                    this.currrentServiceAdo.HisMaterial.AMOUNT = this.currrentServiceAdo.IMP_AMOUNT;
                    this.currrentServiceAdo.HisMaterial.IMP_PRICE = this.currrentServiceAdo.IMP_PRICE;
                    this.currrentServiceAdo.HisMaterial.DOCUMENT_PRICE = this.currrentServiceAdo.DOCUMENT_PRICE;
                    this.currrentServiceAdo.HisMaterial.IMP_VAT_RATIO = this.currrentServiceAdo.IMP_VAT_RATIO;
                    this.currrentServiceAdo.HisMaterial.TAX_RATIO = this.currrentServiceAdo.TAX_RATIO;
                    this.currrentServiceAdo.HisMaterial.PACKAGE_NUMBER = this.currrentServiceAdo.PACKAGE_NUMBER;
                    this.currrentServiceAdo.HisMaterial.EXPIRED_DATE = this.currrentServiceAdo.EXPIRED_DATE;

                    this.currrentServiceAdo.HisMaterial.TDL_BID_GROUP_CODE = this.currrentServiceAdo.TDL_BID_GROUP_CODE;
                    this.currrentServiceAdo.HisMaterial.TDL_BID_NUM_ORDER = this.currrentServiceAdo.TDL_BID_NUM_ORDER;
                    this.currrentServiceAdo.HisMaterial.TDL_BID_PACKAGE_CODE = this.currrentServiceAdo.TDL_BID_PACKAGE_CODE;

                    this.currrentServiceAdo.HisMaterial.NATIONAL_NAME = this.currrentServiceAdo.NATIONAL_NAME;
                    this.currrentServiceAdo.HisMaterial.CONCENTRA = this.currrentServiceAdo.CONCENTRA;
                    this.currrentServiceAdo.HisMaterial.MATERIAL_REGISTER_NUMBER = this.currrentServiceAdo.REGISTER_NUMBER;
                    this.currrentServiceAdo.HisMaterial.MANUFACTURER_ID = this.currrentServiceAdo.MANUFACTURER_ID;

                    this.currrentServiceAdo.HisMaterial.BID_MATERIAL_TYPE_CODE = this.currrentServiceAdo.packingTypeName;
                    this.currrentServiceAdo.HisMaterial.BID_MATERIAL_TYPE_NAME = this.currrentServiceAdo.heinServiceBhytName;
                    this.currrentServiceAdo.HisMaterial.INFORMATION_BID = this.cboInformationBid.SelectedIndex != -1 ? (long?)(this.cboInformationBid.SelectedIndex + 1) : null;
                    this.currrentServiceAdo.TT_THAU = this.currrentServiceAdo.HisMaterial.TT_THAU = this.currrentServiceAdo.HisMaterial.INFORMATION_BID == 3 ? string.Format("{0}{1}", !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_EXTRA_CODE) ? this.currrentServiceAdo.TDL_BID_EXTRA_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_YEAR) ? this.currrentServiceAdo.TDL_BID_YEAR : null) : this.currrentServiceAdo.HisMaterial.INFORMATION_BID == 4 ? string.Format("{0}{1}{2}", !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_EXTRA_CODE) ? this.currrentServiceAdo.TDL_BID_EXTRA_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_PACKAGE_CODE) ? this.currrentServiceAdo.TDL_BID_PACKAGE_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_YEAR) ? this.currrentServiceAdo.TDL_BID_YEAR : null) : string.Format("{0}{1}{2}{3}", !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_EXTRA_CODE) ? this.currrentServiceAdo.TDL_BID_EXTRA_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_PACKAGE_CODE) ? this.currrentServiceAdo.TDL_BID_PACKAGE_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_GROUP_CODE) ? this.currrentServiceAdo.TDL_BID_GROUP_CODE + ";" : null, !string.IsNullOrEmpty(this.currrentServiceAdo.TDL_BID_YEAR) ? this.currrentServiceAdo.TDL_BID_YEAR : null);
                    if (this.currrentServiceAdo.HisMaterialPatys == null)
                    {
                        this.currrentServiceAdo.HisMaterialPatys = new List<HIS_MATERIAL_PATY>();
                    }

                    List<string> mes2 = new List<string>();
                    foreach (var paty in this.currrentServiceAdo.VHisServicePatys)
                    {
                        if (!paty.IsNotSell)
                        {
                            var matePaty = this.currrentServiceAdo.HisMaterialPatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == paty.PATIENT_TYPE_ID);
                            if (matePaty != null)
                            {
                                matePaty.EXP_PRICE = paty.PRICE / (1 + paty.VAT_RATIO);
                                matePaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                            }
                            else
                            {
                                matePaty = new HIS_MATERIAL_PATY();
                                matePaty.PATIENT_TYPE_ID = paty.PATIENT_TYPE_ID;
                                matePaty.EXP_PRICE = paty.PRICE / (1 + paty.VAT_RATIO);
                                matePaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                this.currrentServiceAdo.HisMaterialPatys.Add(matePaty);
                            }
                            if (paty.PRICE != paty.PRE_PRICE_Str && paty.PRE_PRICE_Str > 0)
                            {
                                mes2.Add(string.Format("{0} {1} có giá bán lần trước = {2} của đối tượng {3} khác với giá bán hiện tại.",
                                                this.currrentServiceAdo.IsMedicine ? "Thuốc" : "Vật tư", this.currrentServiceAdo.MEDI_MATE_NAME, string.Format("{0:#,0}", paty.PRE_PRICE_Str), paty.PATIENT_TYPE_NAME));
                            }
                        }
                    }
                    this.currrentServiceAdo.WarningPrice = (mes2 != null && mes2.Count > 0) ? (string.Join("\n", mes2) + "\n Bạn có muốn tiếp tục thêm ?") : "";
                    if (CheckData.Count != 0 && chkImprice.Checked == false)
                    {
                        if (!string.IsNullOrEmpty(this.currrentServiceAdo.WarningPrice))
                        {
                            //WaitingManager.Hide();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(this.currrentServiceAdo.WarningPrice, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                return;
                            }
                        }
                    }


                    if (this.currrentServiceAdo.IsReusable || this.currrentServiceAdo.IsIdentity)
                    {
                        this.currrentServiceAdo.SerialNumbers = new List<ImpMestMaterialReusableSDO>();
                        if (this.IdentityMaterialOption != "2")
                        {
                            var serial = TxtSerialNumber.Text.Split(';');
                            long countSerial = serial.Count();
                            if (spinImpAmount.Value != countSerial)
                            {
                                //TODO  ktra lai co can hay k 
                                DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng vật tư " + (this.currrentServiceAdo.IsReusable ? "tái sử dụng" : "đích danh") + " khác số lượng số seri được tạo", Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, DevExpress.Utils.DefaultBoolean.True);
                                TxtSerialNumber.Focus();
                                return;
                            }
                            foreach (var itemStr in serial)
                            {
                                ImpMestMaterialReusableSDO sdo = new ImpMestMaterialReusableSDO();
                                sdo.ReusCount = (long)SpMaxReuseCount.Value;
                                sdo.SerialNumber = itemStr;
                                this.currrentServiceAdo.SerialNumbers.Add(sdo);
                            }

                            this.currrentServiceAdo.SERIAL_NUMBER = TxtSerialNumber.Text;
                        }
                        else
                        {
                            //vật tư đích danh và tái sử dụng luôn có chi tiết nhập là 1
                            //để có chi tiết nhập là 1 thì cần truyền lên serial rỗng để backend tạo
                            ImpMestMaterialReusableSDO sdo = new ImpMestMaterialReusableSDO();
                            sdo.ReusCount = (long)SpMaxReuseCount.Value;
                            this.currrentServiceAdo.SerialNumbers.Add(sdo);
                            if (this.currrentServiceAdo.IMP_AMOUNT > 1)
                            {
                                for (int i = 1; i < currrentServiceAdo.IMP_AMOUNT; i++)
                                {
                                    ImpMestMaterialReusableSDO sdo1 = new ImpMestMaterialReusableSDO();
                                    sdo1.ReusCount = (long)SpMaxReuseCount.Value;
                                    this.currrentServiceAdo.SerialNumbers.Add(sdo1);
                                }
                            }
                        }
                        if (SpMaxReuseCount.EditValue != null)
                            this.currrentServiceAdo.MAX_REUSE_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(SpMaxReuseCount.EditValue.ToString() ?? "");
                        if (SpPricingMaxReuseCount.EditValue != null)
                            this.currrentServiceAdo.PRICING_MAX_REUSE_COUNT = SpPricingMaxReuseCount.Value;
                        this.currrentServiceAdo.VS_PRICE = (decimal)spinEditGiaVeSinh.Value;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("2________AAA" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listServiceADO), listServiceADO));
                gridControlImpMestDetail.BeginUpdate();
                gridControlImpMestDetail.DataSource = listServiceADO;
                gridControlImpMestDetail.EndUpdate();
                CalculTotalPrice();
                ResetValueControlDetail();
                SetEnableButton(false);
                SetFocuTreeMediMate();
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                //WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCancel1.Enabled)
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

        private void repositoryItemBtnDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
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
                    this.SetValueByServiceAdo();
                    this.LoadServicePatyByAdo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                this.currrentServiceAdo = (VHisServiceADO)gridViewImpMestDetail.GetFocusedRow();
                if (this.currrentServiceAdo != null)
                {
                    #region
                    SetEnableButton(true);
                    if (this.currrentServiceAdo.IsMedicine)
                    {
                        xtraTabControlMain.SelectedTabPage = xtraTabPageMedicine;
                    }
                    else
                    {
                        xtraTabControlMain.SelectedTabPage = xtraTabPageMaterial;
                    }

                    ChangeColorMedicine(this.currrentServiceAdo);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currrentServiceAdo), currrentServiceAdo));
                    if (this.currrentServiceAdo.MEDICAL_CONTRACT_ID.HasValue)
                    {
                        if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                        {
                            medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);
                            medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);
                            medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, this.currrentServiceAdo.MEDICAL_CONTRACT_ID);

                            if (listMedicineType != null && listMedicineType.Count > 0)
                            {
                                var medicine = listMedicineType.FirstOrDefault(o => o.ID == this.currrentServiceAdo.HisMedicine.MEDICINE_TYPE_ID);

                                if (this.currentContract != null)
                                {
                                    this.spinEditGiaTrongThau.Enabled = true;
                                    this.spinImpPrice.Enabled = true;
                                    this.cboNationals.Enabled = true;
                                    this.cboHangSX.Enabled = true;
                                    this.txtSoDangKy.Enabled = true;
                                    this.SpMaxReuseCount.Enabled = false;
                                    this.spinImpPriceVAT.Enabled = false;

                                    if (currrentServiceAdo.ImpVatRatio > 0)
                                    {
                                        spinImpVatRatio.Enabled = false;
                                    }
                                    else
                                    {
                                        spinImpVatRatio.Enabled = true;
                                    }

                                    if (currrentServiceAdo.CONTRACT_PRICE.HasValue)
                                    {
                                        spinEditGiaTrongThau.Value = currrentServiceAdo.CONTRACT_PRICE.Value;
                                    }

                                    if (dicContractMety.Count > 0 && medicine != null)
                                    {
                                        var listvalue = dicContractMety.Select(s => s.Value).ToList();
                                        this.MedicalContractMety = listvalue.FirstOrDefault(o => o.ID == medicine.MEDI_CONTRACT_METY_ID.Value);
                                        cboDosageForm.EditValue = MedicalContractMety.DOSAGE_FORM;
                                        spinCanImpAmount.Value = this.MedicalContractMety.AMOUNT - (this.MedicalContractMety.IN_AMOUNT ?? 0) + currrentServiceAdo.IMP_AMOUNT + (currrentServiceAdo.ADJUST_AMOUNT ?? 0);
                                    }
                                }
                            }
                        }
                        else if (xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                        {
                            materialProcessor.SetEditValueContract(this.ucMaterialTypeTree, null);
                            materialProcessor.EnableBid(this.ucMaterialTypeTree, true);
                            materialProcessor.SetEditValueContract(this.ucMaterialTypeTree, this.currrentServiceAdo.MEDICAL_CONTRACT_ID);

                            if (listMaterialType != null && listMaterialType.Count > 0)
                            {
                                var material = listMaterialType.FirstOrDefault(o => o.ID == this.currrentServiceAdo.HisMaterial.MATERIAL_TYPE_ID);

                                if (this.currentContract != null)
                                {
                                    this.spinEditGiaTrongThau.Enabled = true;
                                    this.spinImpPrice.Enabled = true;
                                    this.cboNationals.Enabled = true;
                                    this.cboHangSX.Enabled = true;
                                    this.txtSoDangKy.Enabled = true;
                                    this.SpMaxReuseCount.Enabled = false;
                                    this.spinImpPriceVAT.Enabled = false;

                                    if (currrentServiceAdo.ImpVatRatio > 0)
                                    {
                                        spinImpVatRatio.Enabled = false;
                                    }
                                    else
                                    {
                                        spinImpVatRatio.Enabled = true;
                                    }

                                    if (currrentServiceAdo.CONTRACT_PRICE.HasValue)
                                    {
                                        spinEditGiaTrongThau.Value = currrentServiceAdo.CONTRACT_PRICE.Value;
                                    }

                                    if (dicContractMaty.Count > 0 && material != null)
                                    {
                                        decimal amountMap = 0;
                                        if (this.currrentServiceAdo.MAP_MEDI_MATE_ID.HasValue && this.listServiceADO != null && this.listServiceADO.Count > 0)
                                        {
                                            var listMap = this.listServiceADO.Where(o => o.MAP_MEDI_MATE_ID == this.currrentServiceAdo.MAP_MEDI_MATE_ID).ToList();
                                            if (listMap != null && listMap.Count > 0)
                                            {
                                                amountMap = listMap.Sum(s => s.IMP_AMOUNT);
                                            }
                                        }

                                        var listvalue = dicContractMaty.Select(s => s.Value).ToList();
                                        this.MedicalContractMaty = listvalue.FirstOrDefault(o => o.ID == material.MEDI_CONTRACT_MATY_ID.Value);
                                        if (!isSave)
                                        {
                                            spinCanImpAmount.Value = this.MedicalContractMaty.AMOUNT - (this.MedicalContractMaty.IN_AMOUNT ?? 0) - amountMap + (currrentServiceAdo.ADJUST_AMOUNT ?? 0);
                                        }
                                        else
                                        {
                                            spinCanImpAmount.Value = this.MedicalContractMaty.AMOUNT - (this.MedicalContractMaty.IN_AMOUNT ?? 0) + (currrentServiceAdo.ADJUST_AMOUNT ?? 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        medicineProcessor.EnableContract(this.ucMedicineTypeTree, false);
                        materialProcessor.EnableContract(this.ucMaterialTypeTree, false);

                        if (this.currrentServiceAdo.BidId.HasValue)
                        {
                            if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                            {
                                medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                                medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);

                                medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, this.currrentServiceAdo.BidId);

                                V_HIS_BID_MEDICINE_TYPE bidMediType = new V_HIS_BID_MEDICINE_TYPE();

                                if (this.currentBid != null && this._dicMedicineTypes != null && this._dicMedicineTypes.ContainsKey(this.currentBid.ID))
                                {
                                    bidMediType = this._dicMedicineTypes[this.currentBid.ID].FirstOrDefault(p => p.MEDICINE_TYPE_ID == this.currrentServiceAdo.MEDI_MATE_ID && p.BID_GROUP_CODE == this.currrentServiceAdo.TDL_BID_GROUP_CODE);
                                }

                                if ((bidMediType == null || bidMediType.ID == 0) && dicBidMedicine.ContainsKey(Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)))
                                {
                                    bidMediType = dicBidMedicine[Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)];
                                }

                                if (bidMediType != null && bidMediType.ID > 0)
                                {
                                    spinCanImpAmount.Value = bidMediType.AMOUNT - (bidMediType.IN_AMOUNT ?? 0) + (bidMediType.AMOUNT * bidMediType.IMP_MORE_RATIO ?? 0) + (currrentServiceAdo.ADJUST_AMOUNT ?? 0);
                                }
                            }
                            else if (xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                            {
                                materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, null);
                                materialProcessor.EnableBid(this.ucMaterialTypeTree, true);

                                materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, this.currrentServiceAdo.BidId);

                                V_HIS_BID_MATERIAL_TYPE bidMateType = new V_HIS_BID_MATERIAL_TYPE();
                                if (this._dicMaterialTypes != null && this._dicMaterialTypes.ContainsKey(this.currentBid.ID))
                                {
                                    bidMateType = this._dicMaterialTypes[this.currentBid.ID].FirstOrDefault(p => p.MATERIAL_TYPE_ID == currrentServiceAdo.MEDI_MATE_ID && p.BID_GROUP_CODE == currrentServiceAdo.TDL_BID_GROUP_CODE);
                                }

                                if (this.currrentServiceAdo.MAP_MEDI_MATE_ID.HasValue && this._dicMaterialTypes != null && this._dicMaterialTypes.ContainsKey(this.currentBid.ID))
                                {
                                    bidMateType = this._dicMaterialTypes[this.currentBid.ID].FirstOrDefault(p => p.MATERIAL_TYPE_ID == currrentServiceAdo.MAP_MEDI_MATE_ID && p.BID_GROUP_CODE == this.currrentServiceAdo.TDL_BID_GROUP_CODE);
                                }

                                if ((bidMateType == null || bidMateType.ID == 0) && dicBidMaterial.ContainsKey(Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)))
                                {
                                    bidMateType = dicBidMaterial[Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)];
                                }

                                decimal amountMap = 0;
                                if (this.currrentServiceAdo.MAP_MEDI_MATE_ID.HasValue && this.listServiceADO != null && this.listServiceADO.Count > 0)
                                {
                                    var listMap = this.listServiceADO.Where(o => o.MAP_MEDI_MATE_ID == this.currrentServiceAdo.MAP_MEDI_MATE_ID).ToList();
                                    if (listMap != null && listMap.Count > 0)
                                    {
                                        amountMap = listMap.Sum(s => s.IMP_AMOUNT);
                                    }
                                }

                                if (bidMateType != null && bidMateType.ID > 0)
                                {
                                    if (!isSave)
                                    {

                                        spinCanImpAmount.Value = bidMateType.AMOUNT - (bidMateType.IN_AMOUNT ?? 0) - amountMap + (bidMateType.AMOUNT * bidMateType.IMP_MORE_RATIO ?? 0) + (currrentServiceAdo.ADJUST_AMOUNT ?? 0);
                                    }
                                    else
                                    {
                                        spinCanImpAmount.Value = bidMateType.AMOUNT - (bidMateType.IN_AMOUNT ?? 0) + (bidMateType.AMOUNT * bidMateType.IMP_MORE_RATIO ?? 0) + (currrentServiceAdo.ADJUST_AMOUNT ?? 0);
                                    }

                                }

                            }
                        }
                        else
                        {
                            medicineProcessor.EnableBid(this.ucMedicineTypeTree, false);
                            materialProcessor.EnableBid(this.ucMaterialTypeTree, false);
                            spinCanImpAmount.Value = this.currrentServiceAdo.CanImpAmount;
                        }
                    }
                    VisibleLayoutTemperature();
                    if (this.currrentServiceAdo.TEMPERATURE != null)
                        spnTemperature.Value = this.currrentServiceAdo.TEMPERATURE ?? 0;
                    else
                        spnTemperature.EditValue = null;
                    spinImpAmount.Value = this.currrentServiceAdo.IMP_AMOUNT;
                    spinImpPrice1.Value = this.currrentServiceAdo.IMP_PRICE;
                    spinImpPrice.Value = this.currrentServiceAdo.IMP_PRICE;
                    spinImpVatRatio.Value = this.currrentServiceAdo.ImpVatRatio;
                    spinEditThueXuat.Value = this.currrentServiceAdo.TAX_RATIO * 100 ?? 0;
                    spinEditTTCoVAT.Value = (long)Math.Round((this.currrentServiceAdo.IMP_AMOUNT * this.currrentServiceAdo.IMP_PRICE * (1 + this.currrentServiceAdo.ImpVatRatio / 100)), 0, MidpointRounding.AwayFromZero);

                    txtBidGroupCode.Text = this.currrentServiceAdo.TDL_BID_GROUP_CODE;
                    txtBidNumOrder.Text = this.currrentServiceAdo.TDL_BID_NUM_ORDER;
                    txtBidYear.Text = this.currrentServiceAdo.TDL_BID_YEAR;
                    txtBidNumber.Text = this.currrentServiceAdo.TDL_BID_NUMBER;
                    txtBidExtraCode.Text = this.currrentServiceAdo.TDL_BID_EXTRA_CODE;
                    txtBid.Text = this.currrentServiceAdo.TDL_BID_PACKAGE_CODE;
                    this.txtPackingJoinBid.Text = this.currrentServiceAdo.packingTypeName;
                    this.txtHeinServiceBidMateType.Text = this.currrentServiceAdo.heinServiceBhytName;
                    this.txtActiveIngrBhytName.Text = this.currrentServiceAdo.activeIngrBhytName;
                    
                    this.cboMedicineUseForm.EditValue = this.currrentServiceAdo.medicineUseFormId;
                    this.txtSoDangKy.Text = this.currrentServiceAdo.REGISTER_NUMBER;
                    this.cboInformationBid.SelectedIndex = currrentServiceAdo.IsMedicine ? -1 : (int)(currrentServiceAdo.HisMaterial.INFORMATION_BID ?? 0) - 1;

                    this.cboInformationBid.Enabled = true;
                    if (dicBidMaterial.ContainsKey(Base.StaticMethod.GetTypeKey(this.currrentServiceAdo.MEDI_MATE_ID, this.currrentServiceAdo.TDL_BID_GROUP_CODE)))
                    {
                        var bidMate = dicBidMaterial[Base.StaticMethod.GetTypeKey(currrentServiceAdo.MEDI_MATE_ID, currrentServiceAdo.TDL_BID_GROUP_CODE)];
                        if (bidMate != null && this.currrentServiceAdo.HisMaterial.INFORMATION_BID.HasValue && this.currrentServiceAdo.HisMaterial.INFORMATION_BID == bidMate.INFORMATION_BID)
                        {
                            this.cboInformationBid.Enabled = false;
                        }
                    }
                    if (!string.IsNullOrEmpty(this.currrentServiceAdo.NATIONAL_NAME))
                    {
                        var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(p => (this.currrentServiceAdo.NATIONAL_NAME ?? "").Trim().ToLower() == (p.NATIONAL_NAME ?? "").Trim().ToLower()).FirstOrDefault();
                        if (national != null)
                        {
                            cboNationals.EditValue = national.ID;
                            chkEditNational.Checked = false;
                            txtNationalMainText.Text = national.NATIONAL_NAME;
                        }
                        else
                        {
                            chkEditNational.Checked = true;
                            txtNationalMainText.Text = this.currrentServiceAdo.NATIONAL_NAME;
                        }
                    }
                    else
                    {
                        cboNationals.EditValue = null;
                        txtNationalMainText.Text = null;
                        chkEditNational.Checked = false;
                    }
                    this.txtNognDoHL.Text = this.currrentServiceAdo.CONCENTRA;
                    if (this.currrentServiceAdo.IsMedicine)
                    {
                        cboDosageForm.EditValue = this.currrentServiceAdo.HisMedicine.DOSAGE_FORM;
                    }
                    cboHangSX.EditValue = this.currrentServiceAdo.MANUFACTURER_ID;    

                    if (this.currrentServiceAdo.HisMedicine != null)
                    {
                        chkImprice.Checked = this.currrentServiceAdo.HisMedicine.IS_SALE_EQUAL_IMP_PRICE == 1 ? true : false;
                    }
                    else if (this.currrentServiceAdo.HisMaterial != null)   
                    {
                        chkImprice.Checked = this.currrentServiceAdo.HisMaterial.IS_SALE_EQUAL_IMP_PRICE == 1 ? true : false;
                    }

                    if (this.currentContract == null)
                    {
                        spinImpVatRatio.Enabled = true;
                        spinEditGiaTrongThau.Value = this.currrentServiceAdo.BID_PRICE;
                    }
                    spinEditGiaNhapLanTruoc.Value = this.currrentServiceAdo.IMP_PRICE_PREVIOUS;
                    GiaBan = this.currrentServiceAdo.GiaBan;

                    if (this.currrentServiceAdo.BidId > 0)
                    {
                        cboGoiThau.EditValue = this.currrentServiceAdo.BidId;
                    }

                    if (chkImprice.Checked == true)
                    {
                        if (this.currrentServiceAdo.HisMedicine != null)
                        {
                            var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.ID == this.currrentServiceAdo.HisMedicine.MEDICINE_TYPE_ID).ToList();
                            var medicine = data.FirstOrDefault();
                            if (medicine.IS_SALE_EQUAL_IMP_PRICE == 1)
                            {
                                chkImprice.Enabled = false;
                            }
                            else
                            {
                                chkImprice.Enabled = true;
                            }
                        }
                        else if (this.currrentServiceAdo.HisMaterial != null)
                        {
                            var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.ID == this.currrentServiceAdo.HisMaterial.MATERIAL_TYPE_ID).ToList();
                            var material = data.FirstOrDefault();
                            if (material.IS_SALE_EQUAL_IMP_PRICE == 1)
                            {
                                chkImprice.Enabled = false;
                            }
                            else
                            {
                                chkImprice.Enabled = true;
                            }
                        }
                    }
                    else
                    {
                        chkImprice.Enabled = true;
                    }

                    if (this.currrentServiceAdo.EXPIRED_DATE.HasValue)
                    {
                        dtExpiredDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currrentServiceAdo.EXPIRED_DATE.Value) ?? DateTime.MinValue;
                        txtExpiredDate.Text = dtExpiredDate.DateTime.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        dtExpiredDate.EditValue = null;
                        txtExpiredDate.Text = "";
                    }
                    if (this.currrentServiceAdo.VALID_FROM_TIME.HasValue)
                    {
                        dtHieuLucTu.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currrentServiceAdo.VALID_FROM_TIME.Value) ?? DateTime.MinValue;
                    }
                    else
                    {
                        dtHieuLucTu.EditValue = null;
                    }
                    if (this.currrentServiceAdo.VALID_TO_TIME.HasValue)
                    {
                        dtHieuLucDen.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currrentServiceAdo.VALID_TO_TIME.Value) ?? DateTime.MinValue;
                    }
                    else
                    {
                        dtHieuLucDen.EditValue = null;
                    }
                    txtPackageNumber.Text = this.currrentServiceAdo.PACKAGE_NUMBER;
                    AutoMapper.Mapper.CreateMap<VHisServicePatyADO, VHisServicePatyADO>();
                    listServicePatyAdo = AutoMapper.Mapper.Map<List<VHisServicePatyADO>>(this.currrentServiceAdo.VHisServicePatys);
                    if (chkImprice.Checked == true)
                    {
                        listServicePatyAdo = null;
                    }

                    DevExpress.XtraEditors.DXErrorProvider.ValidationRule validationRule = new DevExpress.XtraEditors.DXErrorProvider.ValidationRule();
                    dxValidationProvider2.SetValidationRule(dtExpiredDate, validationRule);
                    dxValidationProvider2.SetValidationRule(txtExpiredDate, validationRule);
                    layoutExpiredDate.AppearanceItemCaption.ForeColor = Color.Black;
                    this.SoloValidBidControl(txtPackageNumber, layoutPackageNumber, false);

                    //#35237 :ko bắt buộc nhập số lô, HSD
                    if (!this.currrentServiceAdo.IsAllowMissingPkgInfo)
                    {
                        if (this.currrentServiceAdo.IsRequireHsd)
                        {
                            ValidControlExpiredDate1(dtExpiredDate);
                            ValidControlExpiredDate1(txtExpiredDate);
                            layoutExpiredDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                        }

                        if (this.currrentServiceAdo.IsMedicine)
                        {
                            if (this.cboImpMestType.EditValue != null
                            && Inventec.Common.TypeConvert.Parse.ToInt64(this.cboImpMestType.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC
                            && (this.currrentServiceAdo.MEDICINE_LINE_ID == null
                            || (this.currrentServiceAdo.MEDICINE_LINE_ID > 0
                            && this.currrentServiceAdo.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__TTD))
                            )
                            {
                                ValidControlExpiredDate1(dtExpiredDate);
                                ValidControlExpiredDate1(txtExpiredDate);
                                SoloValidBidControl(txtPackageNumber, layoutPackageNumber, true);
                                layoutExpiredDate.AppearanceItemCaption.ForeColor = Color.Maroon;
                            }
                            else
                            {
                                SoloValidBidControl(txtPackageNumber, layoutPackageNumber, false);
                            }
                        }
                        else
                        {
                            this.SoloValidBidControl(txtPackageNumber, layoutPackageNumber, false);
                        }
                    }

                    EventProcessMaterialReUse();


                    gridControlServicePaty.BeginUpdate();
                    UpdateExpPrice();
                    long tp_ = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AmountDecimalNumber"));
                    long tp = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AutoRoundExpPriceOption"));
                    if (listServicePatyAdo != null && listServicePatyAdo.Count > 0)
                    {
                        foreach (var item in listServicePatyAdo)
                        {
                            if (tp == 1)
                            {
                                item.PRICE = Math.Round(item.PRICE, (int)tp_);
                            }
                            else if (tp == 2)
                            {
                                item.PRICE = RoundDown(item.PRICE, tp_);
                            }
                            else if (tp == 3)
                            {
                                item.PRICE = RoundUp(item.PRICE, tp_);
                            }
                            //else
                            //{
                            //    item.PRICE = Inventec.Common.Number.Convert.NumberToString(item.PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            //}
                        }
                    }

                    gridControlServicePaty.DataSource = listServicePatyAdo;
                    gridControlServicePaty.EndUpdate();
                    spinImpAmount.Focus();
                    spinImpAmount.SelectAll();
                    #endregion
                }
                else
                {
                    SetEnableButton(false);
                    SetValueByServiceAdo();
                    LoadServicePatyByAdo();
                    SetFocuTreeMediMate();
                }
                if (IsHasValueChooice && IsNCC)
                {
                    SetEnableControl(this.IsAllowedToEnableMedicinesInformation);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHoiDongKiemNhap_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisRoleUser").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisRoleUser");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST ado = new HIS_IMP_MEST();
                    if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                    {
                        ado.ID = this.resultADO.HisInitSDO.ImpMest.ID;
                        ado.IMP_MEST_TYPE_ID = this.resultADO.HisInitSDO.ImpMest.IMP_MEST_TYPE_ID;
                        ado.IMP_MEST_STT_ID = this.resultADO.HisInitSDO.ImpMest.IMP_MEST_STT_ID;
                    }
                    else if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                    {
                        ado.ID = this.resultADO.HisInveSDO.ImpMest.ID;
                        ado.IMP_MEST_TYPE_ID = this.resultADO.HisInveSDO.ImpMest.IMP_MEST_TYPE_ID;
                        ado.IMP_MEST_STT_ID = this.resultADO.HisInveSDO.ImpMest.IMP_MEST_STT_ID;
                    }
                    else if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                    {
                        ado.ID = this.resultADO.HisOtherSDO.ImpMest.ID;
                        ado.IMP_MEST_TYPE_ID = this.resultADO.HisOtherSDO.ImpMest.IMP_MEST_TYPE_ID;
                        ado.IMP_MEST_STT_ID = this.resultADO.HisOtherSDO.ImpMest.IMP_MEST_STT_ID;
                    }
                    else if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                    {
                        ado.ID = this.resultADO.HisManuSDO.ImpMest.ID;
                        ado.IMP_MEST_TYPE_ID = this.resultADO.HisManuSDO.ImpMest.IMP_MEST_TYPE_ID;
                        ado.IMP_MEST_STT_ID = this.resultADO.HisManuSDO.ImpMest.IMP_MEST_STT_ID;
                    }
                    listArgs.Add(ado);
                    listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    WaitingManager.Hide();
                    ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinImpPrice1.Enabled && spinImpPrice1.Visible)
                    {
                        spinImpPrice1.Focus();
                        spinImpPrice1.SelectAll();
                    }
                    else if (spinEditTTChuaVAT.Enabled && spinEditTTChuaVAT.Visible)
                    {
                        spinEditTTChuaVAT.Focus();
                        spinEditTTChuaVAT.SelectAll();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
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
                if (!spinImpPrice.Visible || layoutImpPrice.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    return;
                if (IsEditImpPriceVAT) return;

                IsEditImpPrice = true;
                if (this.currrentServiceAdo != null)
                    this.currrentServiceAdo.IMP_PRICE = spinImpPrice.Value;
                spinImpPriceVAT.Value = (spinImpPrice.Value * (1 + spinImpVatRatio.Value / 100));

                spinEditTTCoVAT.Value = Math.Round((spinImpAmount.Value * spinImpPrice.Value * (1 + spinImpVatRatio.Value / 100)), ConfigApplications.NumberSeperator, MidpointRounding.AwayFromZero);
                spinEditTTChuaVAT.Value = (spinImpAmount.Value * spinImpPrice.Value);
                if (this.currrentServiceAdo != null && this.currrentServiceAdo.IsReusable)
                {
                    this.currrentServiceAdo.IMP_PRICE = spinImpPrice.Value;
                    ReUpdatePrice();
                }

                IsEditImpPrice = false;
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
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinEditGiaTrongThau_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinEditGiaTrongThau.Enabled && spinEditGiaTrongThau.Visible)
                {
                    spinImpPrice.Value = (spinEditGiaTrongThau.Value / (1 + spinImpVatRatio.Value / 100));
                    if (spinImpPrice1.Enabled && spinImpPrice1.Visible)
                    {
                        spinImpPrice1.Value = (spinEditGiaTrongThau.Value / (1 + spinImpVatRatio.Value / 100));
                    }
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
                if (IsEditImpPriceVAT) return;
                if (IsEditImpPrice) return;

                if (this.currrentServiceAdo != null)
                    this.currrentServiceAdo.IMP_VAT_RATIO = spinImpVatRatio.Value / 100;
                if (this._currentImpMestUp != null && this._currentImpMestUp.ID > 0)
                    this.currrentServiceAdo.IMP_VAT_RATIO = spinImpVatRatio.Value / 100;
                if (this.currentContract != null && this.currentContract.ID > 0)
                {
                    if (spinImpPrice1.Enabled && spinImpPrice1.Visible)
                    {
                        spinImpPrice1.Value = (spinEditGiaTrongThau.Value / (1 + spinImpVatRatio.Value / 100));
                    }
                    else
                    {
                        spinImpPrice.Value = (spinEditGiaTrongThau.Value / (1 + spinImpVatRatio.Value / 100));
                    }
                    LoadServicePatyByAdo();
                }

                if (spinImpPrice1.Enabled && spinImpPrice1.Visible)
                    spinImpPriceVAT.Value = (spinImpPrice1.Value * (1 + spinImpVatRatio.Value / 100));
                else
                    spinImpPriceVAT.Value = (spinImpPrice.Value * (1 + spinImpVatRatio.Value / 100));

                if (spinImpPrice1.Enabled && spinImpPrice1.Visible)
                {
                    spinEditTTCoVAT.Value = Math.Round((spinImpAmount.Value * spinImpPrice1.Value * (1 + spinImpVatRatio.Value / 100)), ConfigApplications.NumberSeperator, MidpointRounding.AwayFromZero);
                }
                else
                {
                    spinEditTTCoVAT.Value = Math.Round((spinImpAmount.Value * spinImpPrice.Value * (1 + spinImpVatRatio.Value / 100)), ConfigApplications.NumberSeperator, MidpointRounding.AwayFromZero);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPriceVAT_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!IsEditImpPrice)
                {
                    IsEditImpPriceVAT = true;
                    if (!spinImpPrice1.Enabled || !spinImpPrice1.Visible)
                    {
                        spinImpPrice.Value = spinImpPriceVAT.Value / ((1 + spinImpVatRatio.Value / 100));
                    }
                    IsEditImpPriceVAT = false;
                }
                if (this.currrentServiceAdo != null)
                    this.currrentServiceAdo.IMP_PRICE = spinImpPrice.Visible ? spinImpPrice.Value : spinImpPrice1.Visible ? spinImpPrice1.Value : 0;
                LoadServicePatyByAdo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Hạn Sử Dụng
        private void txtExpiredDate_ButtonClick(object sender, ButtonPressedEventArgs e)
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
                        dtExpiredDate.Visible = true;
                        dtExpiredDate.Focus();
                        dtExpiredDate.ShowPopup();
                    }
                    else
                    {
                        dtExpiredDate.Visible = true;
                        dtExpiredDate.Focus();
                        dtExpiredDate.ShowPopup();
                    }
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
                string strError = Base.ResourceMessageManager.NguoiDungNhapNgayKhongHopLe;
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
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
                    SendKeys.Send("{TAB}");
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtExpiredDate.EditValue = dt;
                        dtExpiredDate.Update();

                        SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        dtExpiredDate.Visible = true;
                        dtExpiredDate.Focus();
                        dtExpiredDate.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpiredDate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtExpiredDate.Visible = false;
                    txtExpiredDate.Text = dtExpiredDate.DateTime.ToString("dd/MM/yyyy");
                    if (!cboNationals.Visible)
                    {
                        SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboNationals.Focus();
                    }
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
                    txtExpiredDate.Text = dtExpiredDate.DateTime.ToString("dd/MM/yyyy");
                    if (!cboNationals.Visible)
                    {
                        SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboNationals.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void chkImprice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadServicePatyByAdo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkImprice_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinEditTTChuaVAT1.Enabled && spinEditTTChuaVAT1.Visible)
                    {
                        spinEditTTChuaVAT1.Focus();
                        spinEditTTChuaVAT1.SelectAll();
                    }
                    else if (spinImpPrice.Enabled && spinImpPrice.Visible)
                    {
                        spinImpPrice.Focus();
                        spinImpPrice.SelectAll();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackageNumber_KeyDown(object sender, KeyEventArgs e)
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

        private void txtBidNumOrder_KeyDown(object sender, KeyEventArgs e)
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

        private void txtBid_KeyDown(object sender, KeyEventArgs e)
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

        private void txtBidNumber_KeyDown(object sender, KeyEventArgs e)
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

                    SetControlEnableImMestTypeManu();
                    SetDefaultCheckNoBid();
                    if (this.currentImpMestType != null)
                    {
                        if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                        {
                            listMediStock = listMediStock.Where(o => o.IS_ALLOW_IMP_SUPPLIER == 1).ToList();
                        }
                        cboMediStock.Properties.DataSource = listMediStock;
                        cboImpSource.Focus();
                        cboImpSource.SelectAll();
                    }
                    else
                    {
                        cboMediStock.Properties.DataSource = null;
                        cboImpMestType.Focus();
                        cboImpMestType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpMestType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.currentImpMestType = null;
                    listMediStock = new List<V_HIS_MEDI_STOCK>();
                    this.currentBid = null;

                    ColorBlack();

                    if (cboImpMestType.EditValue != null)
                    {
                        this.currentImpMestType = listImpMestType.FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMestType.EditValue));

                        if (this.currentImpMestType != null)
                        {
                            listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                            if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                            {
                                listMediStock = listMediStock.Where(o => o.IS_ALLOW_IMP_SUPPLIER == 1).ToList();
                                cboImpSource.EditValue = this._ImpSourceId;
                            }
                            VisibleLayoutTemperature();
                        }

                    }


                    LoadDataToComboSupplier(listSupplier);
                    SetControlEnableImMestTypeManu();
                    txtNhaCC_Closed(null, null);
                    //Load Lại trạng thái các control khi thay đổi Loại nhập không phải là ncc và đầu kỳ
                    if (this.currentImpMestType != null && (this.currentImpMestType.ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && this.currentImpMestType.ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK))
                    {
                        SetControlEnableImMestTypeManu();
                    }


                    if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && medistock.IS_ALLOW_IMP_SUPPLIER == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Kho không cho phép nhập từ nhà cung cấp", "Thông báo");
                        return;
                    }

                    if (this.currentImpMestType != null)
                    {
                        if (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                        {
                            listMediStock = listMediStock.Where(o => o.IS_ALLOW_IMP_SUPPLIER == 1).ToList();
                        }
                    }

                    SetDefaultCheckNoBid();
                    cboMediStock.Properties.DataSource = listMediStock;
                    cboImpSource.Focus();
                    cboImpSource.SelectAll();
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
                        SendKeys.Send("{TAB}");
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
                                cboImpSource.SelectAll();
                            }
                        }

                        if (!valid)
                        {
                            cboMediStock.Focus();
                            cboMediStock.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    cboImpSource.Focus();
                    cboImpSource.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpSource_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboImpSource.Text))
                    {
                        var key = cboImpSource.Text.ToLower();
                        var listData = BackendDataWorker.Get<HIS_IMP_SOURCE>().Where(o => o.IMP_SOURCE_CODE.ToLower().Contains(key) || o.IMP_SOURCE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboImpSource.EditValue = listData.First().ID;
                            SendKeys.Send("{TAB}");
                        }
                    }

                    if (!valid)
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpSource_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSupplier_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateCboGoiThau()
        {
            try
            {
                ValidGoiThauNewControl();
                if (checkInOutBid.Checked)
                {
                    ValidBidCheckValidate();
                    lciGoiThau.AppearanceItemCaption.ForeColor = Color.Black;
                    dxValidationProvider2.SetValidationRule(cboGoiThau, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkOutBid_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkInOutBid.Checked && checkOutBid.Checked)
                    checkInOutBid.Checked = false;

                //#17094
                ValidBidCheckValidate();

                ColorBlack();

                Contract_RowClick();

                if (checkOutBid.Checked)
                {
                    this.currentBid = null;
                    medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, null);
                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, false);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, false);

                    txtBidGroupCode.Text = "";
                    txtBidYear.Text = "";
                    txtBidNumber.Text = "";
                    txtBidNumOrder.Text = "";
                    txtBid.Text = "";
                }
                else
                {
                    if (cboImpMestType.EditValue != null
                       && ((long)cboImpMestType.EditValue == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || (long)cboImpMestType.EditValue == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK))
                    {
                        medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);
                        materialProcessor.EnableBid(this.ucMaterialTypeTree, true);
                    }
                    else
                    {
                        medicineProcessor.EnableBid(this.ucMedicineTypeTree, false);
                        materialProcessor.EnableBid(this.ucMaterialTypeTree, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDocumentDate.Focus();
                    txtDocumentDate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Ngày Chứng Từ
        private void txtDocumentDate_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtDocumentDate.EditValue = dt;
                        dtDocumentDate.Update();
                        dtDocumentDate.Visible = true;
                        dtDocumentDate.Focus();
                        dtDocumentDate.ShowPopup();
                    }
                    else
                    {
                        dtDocumentDate.Visible = true;
                        dtDocumentDate.Focus();
                        dtDocumentDate.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = Base.ResourceMessageManager.NguoiDungNhapNgayKhongHopLe;
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDocumentDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                    txtkyHieuHoaDon.Focus();
                    txtkyHieuHoaDon.SelectAll();

                    Contract_RowClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_Leave(object sender, EventArgs e)
        {
            try
            {
                dtDocumentDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                txtkyHieuHoaDon.Focus();
                txtkyHieuHoaDon.SelectAll();

                Contract_RowClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtDocumentDate.Text))
                    {
                        dtDocumentDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                        txtkyHieuHoaDon.Focus();
                        txtkyHieuHoaDon.SelectAll();

                        Contract_RowClick();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtDocumentDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtDocumentDate.EditValue = dt;
                        dtDocumentDate.Update();
                        txtkyHieuHoaDon.Focus();
                        txtkyHieuHoaDon.SelectAll();
                    }
                    else
                    {
                        dtDocumentDate.Visible = true;
                        dtDocumentDate.Focus();
                        dtDocumentDate.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDocumentDate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtDocumentDate.Visible = false;
                    txtDocumentDate.Text = dtDocumentDate.DateTime.ToString("dd/MM/yyyy");
                    txtkyHieuHoaDon.Focus();
                    txtkyHieuHoaDon.SelectAll();

                    Contract_RowClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDocumentDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDocumentDate.Visible = false;
                    dtDocumentDate.Update();
                    txtDocumentDate.Text = dtDocumentDate.DateTime.ToString("dd/MM/yyyy");
                    txtkyHieuHoaDon.Focus();
                    txtkyHieuHoaDon.SelectAll();

                    Contract_RowClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtDeliverer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRecieve.Focus();
                    cboRecieve.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDocumentPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDeliverer.Focus();
                    txtDeliverer.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServicePaty_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
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
                        data.ExpPriceVat = data.ExpPrice * (1 + data.VAT_RATIO);
                        data.PRICE = (1 + data.PercentProfit / 100) * data.ExpPriceVat;
                    }
                    else if (e.Column.FieldName == "PercentProfit")
                    {
                        data.PRICE = (1 + data.PercentProfit / 100) * data.ExpPriceVat;
                    }
                    else if (e.Column.FieldName == "PRICE")
                    {
                        if (data.ExpPriceVat > 0)
                            data.PercentProfit = 100 * (data.PRICE - data.ExpPriceVat) / data.ExpPriceVat;
                        else
                        {
                            data.ExpPriceVat = data.PRICE / (decimal)((decimal)1 + (data.PercentProfit / (decimal)100));
                            data.ExpPrice = data.ExpPriceVat / (decimal)((decimal)1 + data.VAT_RATIO);
                        }
                    }
                    else if (e.Column.FieldName == "IsNotSell")
                    {
                        if (data.IsReusable)
                            ReUpdatePrice(true);
                        gridControlServicePaty.RefreshDataSource();
                    }


                    long tp_ = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AmountDecimalNumber"));
                    long tp = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AutoRoundExpPriceOption"));

                    if (tp == 1)
                    {
                        data.PRICE = Math.Round(data.PRICE, (int)tp_);
                    }
                    if (tp == 2)
                    {
                        data.PRICE = RoundDown(data.PRICE, tp_);
                    }
                    if (tp == 3)
                    {
                        data.PRICE = RoundUp(data.PRICE, tp_);
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.PRICE), data.PRICE));
                    gridControlServicePaty.RefreshDataSource();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public decimal MathRound(decimal decimalPlaces, int key)
        {
            decimal result = 0;
            try
            {
                switch (key)
                {
                    case 1:
                        result = Math.Round(decimalPlaces, (int)key);
                        break;
                    case 2:
                        result = RoundDown(decimalPlaces, key);
                        break;
                    case 3:
                        result = RoundUp(decimalPlaces, key);
                        break;
                    default:
                        result = decimalPlaces;
                        break;
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public decimal RoundDown(decimal i, double decimalPlaces)
        {
            var power = Convert.ToDecimal(Math.Pow(10, decimalPlaces));
            return Math.Floor(i * power) / power;
        }

        public decimal RoundUp(decimal i, double decimalPlaces)
        {
            var power = Convert.ToDecimal(Math.Pow(10, decimalPlaces));
            return Math.Ceiling(i * power) / power;
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
                                if (data.IsNotSell || HisConfig.DisableSaleProfitCFG)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServicePaty_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
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
                        else if (e.Column.FieldName == "DISCOUNT_FROM_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.DISCOUNT_FROM_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DISCOUNT_TO_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.DISCOUNT_TO_DATE ?? 0);
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
                        else if (e.Column.FieldName == "PRICE_VAT_STR")
                        {
                            try
                            {
                                decimal tt = data.IMP_PRICE * (1 + data.IMP_VAT_RATIO);
                                e.Value = Inventec.Common.Number.Convert.NumberToString(tt, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "medicineUseFormId_STR")
                        {
                            try
                            {
                                var useForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.ID == data.medicineUseFormId);
                                e.Value = useForm != null ? useForm.MEDICINE_USE_FORM_NAME : "";
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

        private void gridViewImpMestDetail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
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

                        CalculTotalPrice();
                    }
                    else if (e.Column.FieldName == "ImpVatRatio")
                    {
                        data.IMP_VAT_RATIO = data.ImpVatRatio / 100;

                        CalculTotalPrice();
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
                            else if ((Convert.ToInt64(dt.ToString("yyyyMMdd") + "235959")) < (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Hạn sử dụng không được nhỏ hơn ngày hiện tại");
                                return;
                            }
                            else
                            {
                                data.EXPIRED_DATE = Convert.ToInt64(dt.ToString("yyyyMMdd") + "235959");
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
                    else if (e.Column.FieldName == "TT_THAU")
                    {
                        if (data.IsMedicine)
                        {
                            data.HisMedicine.TT_THAU = data.TT_THAU;
                        }
                        else
                        {
                            data.HisMaterial.TT_THAU = data.TT_THAU;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraTabControlMain_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                IsChangeTabPage = true;
                IsLoadGridMedicine = true;
                IsLoadGridMaterial = true;
                // if ((this.currentSupplierForEdit != null && currentSupplierForEdit.ID != null || (this.currentImpMestType != null && (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK || this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC))) && !checkOutBid.Checked)
                if ((this.currentSupplierForEdit != null && currentSupplierForEdit.ID != null || (this.currentImpMestType != null && (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK || this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC))) && !checkOutBid.Checked)
                {
                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, true);
                    this.currentSupplier = this.currentSupplierForEdit;
                    if (currentSupplier != null)
                    {
                        _HisBidBySuppliers = listBids.Where(o => o.SUPPLIER_IDS != null && ("," + o.SUPPLIER_IDS + ",").Contains("," + currentSupplier.ID + ",")).ToList();
                    }
                }
                else
                {
                    medicineProcessor.ReloadBid(this.ucMedicineTypeTree, null);
                    materialProcessor.ReloadBid(this.ucMaterialTypeTree, null);
                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, false);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, false);
                    materialProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                }
                if (this.currentSupplier != null)
                {
                    if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                    {
                        if (_HisBidBySuppliers.Count == 0)
                        {
                            _HisBidBySuppliers = listBids;
                        }
                        medicineProcessor.ReloadBid(this.ucMedicineTypeTree, this._HisBidBySuppliers);
                        if (currentBid != null)
                            medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, currentBid.ID);
                        else
                            medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);

                        medicineProcessor.ReloadContract(this.ucMedicineTypeTree, this.ContractOfBidAndSup);
                        if (this.currentContract != null)
                        {
                            medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, currentContract.ID);
                        }
                        else
                        {
                            medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);
                        }
                    }
                    else if (xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                    {
                        if (_HisBidBySuppliers.Count == 0)
                        {
                            _HisBidBySuppliers = listBids;
                        }
                        materialProcessor.ReloadBid(this.ucMaterialTypeTree, this._HisBidBySuppliers);
                        if (currentBid != null)
                            materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, currentBid.ID);
                        else
                            materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, null);

                        materialProcessor.ReloadContract(this.ucMaterialTypeTree, this.ContractOfBidAndSup);
                        if (this.currentContract != null)
                        {
                            materialProcessor.SetEditValueContract(this.ucMaterialTypeTree, currentContract.ID);
                        }
                        else
                        {
                            materialProcessor.SetEditValueContract(this.ucMaterialTypeTree, null);
                        }

                    }
                }
                else
                {
                    if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                    {
                        medicineProcessor.ReloadBid(this.ucMedicineTypeTree, this.listBids);
                        if (currentBid != null)
                            medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, currentBid.ID);
                        else
                            medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    }
                    else if (xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                    {
                        materialProcessor.ReloadBid(this.ucMaterialTypeTree, this.listBids);
                        if (currentBid != null)
                            materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, currentBid.ID);
                        else
                            materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, null);
                    }

                    medicineProcessor.ReloadContract(this.ucMedicineTypeTree, null);
                    materialProcessor.ReloadContract(this.ucMaterialTypeTree, null);
                    medicineProcessor.EnableContract(this.ucMedicineTypeTree, false);
                    materialProcessor.EnableContract(this.ucMaterialTypeTree, false);
                    materialProcessor.SetEditValueContract(this.ucMaterialTypeTree, null);
                    medicineProcessor.SetEditValueContract(this.ucMedicineTypeTree, null);
                    this.layoutControlItem7.Text = "Giá trong thầu";
                }

                ChangeLableMetyMaty();

                SetFocuTreeMediMate();
                if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine && IsLoadGridMedicine)
                {
                    if (listBidMedicine != null && listBidMedicine.Count > 0)
                    {
                        dicBidMedicine.Clear();
                        List<V_HIS_BID_MEDICINE_TYPE> lstBidMedi = listBidMedicine.Where(o => o.BID_ID == currentBid.ID).ToList();
                        if (txtNhaCC.EditValue != null)
                        {
                            lstBidMedi = lstBidMedi.Where(o => o.SUPPLIER_ID == currentSupplier.ID).ToList();
                        }
                        foreach (var item in lstBidMedi)
                        {
                            dicBidMedicine[Base.StaticMethod.GetTypeKey(item.MEDICINE_TYPE_ID, item.BID_GROUP_CODE)] = item;
                        }
                    }
                    SetDataSourceGridControlMediMateMedicine();
                }
                else if (xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial && IsLoadGridMaterial)
                {
                    if (listBidMaterial != null && listBidMaterial.Count > 0)
                    {
                        dicBidMaterial.Clear();
                        List<V_HIS_BID_MATERIAL_TYPE> lstBidMate = listBidMaterial.Where(o => o.BID_ID == currentBid.ID).ToList();
                        if (txtNhaCC.EditValue != null)
                        {
                            lstBidMate = lstBidMate.Where(o => o.SUPPLIER_ID == currentSupplier.ID).ToList();
                        }
                        foreach (var item in lstBidMate)
                        {
                            dicBidMaterial[Base.StaticMethod.GetTypeKey(item.MATERIAL_TYPE_ID ?? 0, item.BID_GROUP_CODE)] = item;
                        }
                    }
                    SetDataSourceGridControlMediMateMaterial();
                }
                IsChangeTabPage = false;
                if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                {
                    layoutControlItem35.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    cboInformationBid.SelectedIndex = -1;
                }
                else
                {
                    layoutControlItem35.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackingJoinBid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtHeinServiceBidMateType_KeyDown(object sender, KeyEventArgs e)
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

        private void dtHieuLucDen_KeyDown(object sender, KeyEventArgs e)
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

        private void spinImpPriceVAT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void spinEditGiaTrongThau_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void spinEditGiaNhapLanTruoc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void dtHieuLucTu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtNhaCC_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (txtNhaCC.EditValue != null && oldSupplierId != long.Parse(txtNhaCC.EditValue.ToString()))
                {
                    this.oldSupplierId = long.Parse(txtNhaCC.EditValue.ToString());
                    if (currentImpMestType != null && (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK))
                        txtNhaCC.Properties.Buttons[1].Visible = true;
                    var supplier = listSupplier.FirstOrDefault(o => o.ID == oldSupplierId);
                    //long bidId = (long)cboGoiThau.EditValue;
                    //var bid = listBids.FirstOrDefault(o => o.ID == bidId);
                    if (currentBid != null)
                    {
                        HisBidMedicineTypeViewFilter mediFilter = new HisBidMedicineTypeViewFilter();
                        mediFilter.BID_ID = this.currentBid.ID;
                        mediFilter.IS_ACTIVE = 1;
                        listBidMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MEDICINE_TYPE>>(HisRequestUriStore.HIS_BID_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                        HisBidMaterialTypeViewFilter mateFilter = new HisBidMaterialTypeViewFilter();
                        mateFilter.BID_ID = this.currentBid.ID;
                        mateFilter.IS_ACTIVE = 1;
                        listBidMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MATERIAL_TYPE>>(HisRequestUriStore.HIS_BID_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);

                        this.currentSupplierForEdit = supplier;
                        //if(cboBi)
                        if (listBidMedicine != null && listBidMedicine.Count > 0)
                        {
                            dicBidMedicine.Clear();
                            List<V_HIS_BID_MEDICINE_TYPE> lstBidMedi = listBidMedicine.Where(o => o.BID_ID == currentBid.ID).ToList();
                            lstBidMedi = lstBidMedi.Where(o => o.SUPPLIER_ID == currentSupplierForEdit.ID).ToList();

                            foreach (var item in lstBidMedi)
                            {
                                dicBidMedicine[Base.StaticMethod.GetTypeKey(item.MEDICINE_TYPE_ID, item.BID_GROUP_CODE)] = item;
                            }
                            SetDataSourceGridControlMediMateMedicine();
                        }

                        if (listBidMaterial != null && listBidMaterial.Count > 0)
                        {
                            dicBidMaterial.Clear();
                            List<V_HIS_BID_MATERIAL_TYPE> lstBidMate = listBidMaterial.Where(o => o.BID_ID == currentBid.ID).ToList();
                            lstBidMate = lstBidMate.Where(o => o.SUPPLIER_ID == currentSupplierForEdit.ID).ToList();

                            foreach (var item in lstBidMate)
                            {
                                dicBidMaterial[Base.StaticMethod.GetTypeKey(item.MATERIAL_TYPE_ID ?? 0, item.BID_GROUP_CODE)] = item;
                            }
                            SetDataSourceGridControlMediMateMaterial();
                        }
                        return;

                    }
                    Supplier_RowClick(supplier);
                    Contract_RowClick();
                }
                else if (txtNhaCC.EditValue == null && oldSupplierId != 0 && currentImpMestType != null && (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK))
                {
                    oldSupplierId = 0;
                    txtNhaCC.Properties.Buttons[1].Visible = false;
                    Supplier_RowClick(new HIS_SUPPLIER());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServicePaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisServicePatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        long tp = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.Desktop.Plugins.ImpMestCreate.AutoRoundExpPriceOption"));
                        if (e.Column.FieldName == "PRICE")
                        {
                            try
                            {
                                if (tp == 1 || tp == 2 || tp == 3)
                                {

                                }
                                else
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(data.PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
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

        private void gridControlServicePaty_Click(object sender, EventArgs e)
        {

        }

        private void SetEnableControl(bool enable)
        {
            try
            {
                layoutControlItem26.Enabled = !enable;
                cboNationals.Enabled = !enable;
                chkEditNational.Enabled = !enable;
                cboHangSX.Enabled = !enable;
                cboDosageForm.Enabled = !enable;
                cboMedicineUseForm.Enabled = !enable;
                txtSoDangKy.Enabled = !enable;
                txtActiveIngrBhytName.Enabled = !enable;
                txtPackingJoinBid.Enabled = !enable;
                txtHeinServiceBidMateType.Enabled = !enable;
                cboDosageForm.Enabled = !enable;
                txtNognDoHL.Enabled = !enable;
                IsBID = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkInOutBid_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkOutBid.Checked && checkInOutBid.Checked)
                    checkOutBid.Checked = false;
                ValidateCboGoiThau();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == checkInOutBid.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (checkInOutBid.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = checkInOutBid.Name;
                    csAddOrUpdate.VALUE = (checkInOutBid.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGoiThau_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboGoiThau.EditValue != null)
                {
                    cboGoiThau.Properties.Buttons[1].Visible = true;
                    long bidId = (long)cboGoiThau.EditValue;
                    var bid = this.listBids != null ? this.listBids.Where(o => o.ID == bidId).FirstOrDefault() : null;
                    if (bid != null)
                    {
                        txtBidExtraCode.Text = bid.BID_EXTRA_CODE;
                    }
                    else
                    {
                        txtBidExtraCode.Text = "";
                    }
                }
                else
                {
                    cboGoiThau.Properties.Buttons[1].Visible = false;
                    txtBidExtraCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNhaCC_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete && currentImpMestType != null && (this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC || this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK))
                {
                    txtNhaCC.EditValue = null;
                    txtNhaCC_Closed(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNhaCC_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void SpPricingMaxReuseCount_EditValueChanged(object sender, EventArgs e)
        {
            if (this.currrentServiceAdo != null && this.currrentServiceAdo.IsReusable)
            {
                if (SpPricingMaxReuseCount.EditValue != null)

                    //Tinh lai gia ban
                    ReUpdatePrice();
            }
        }

        private void cboInformationBid_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboInformationBid.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboDosageForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Plus)
            {
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisDosageForm").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisDosageForm");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    //MOS.EFMODEL.DataModels.HIS_DOSAGE_FORM ado = new HIS_DOSAGE_FORM();
                    listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
                    WaitingManager.Hide();
                    //((System.Windows.Forms.Form)extenceInstance).ShowDialog();
                    //cboDosageForm.EditValue = null;
                    //LoadDosageForm();
                    var form = (System.Windows.Forms.Form)extenceInstance;
                    var oldValue = cboDosageForm.EditValue; // Lưu lại EditValue hiện tại
                    form.ShowDialog();

                    LoadDosageForm(); // Load lại danh sách sau khi form đóng
                         
                    // Nếu EditValue cũ còn tồn tại trong datasource mới, gán lại
                    var dataSource = cboDosageForm.Properties.DataSource as IList<object>;
                    if (dataSource != null && dataSource.Any(x => GetValueMember(x)?.ToString() == oldValue?.ToString()))
                    {
                        cboDosageForm.EditValue = oldValue;
                    }
                    else
                    {
                        cboDosageForm.Text = "";
                        cboDosageForm.EditValue = null;
                    }    
                }
            }
            if (e.Button.Kind == ButtonPredefines.Delete) 
            {
                cboDosageForm.Text = "";
                cboDosageForm.EditValue = null;
            }
        }

        private object GetValueMember(object dataItem)
        {
            if (dataItem == null) return null;
            var propertyInfo = dataItem.GetType().GetProperty(cboDosageForm.Properties.ValueMember);
            return propertyInfo?.GetValue(dataItem, null);
        }

        private void VisibleLayoutTemperature()
        {
            try
            {
                IsRequiedTemperature = false;
                dxValidationProvider2.SetValidationRule(spnTemperature, null);
                lciTemperature.AppearanceItemCaption.ForeColor = Color.Black;
                lciTemperature.Visibility = currrentServiceAdo != null && currrentServiceAdo.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (lciTemperature.Visible && currrentServiceAdo != null && currrentServiceAdo.STORAGE_CONDITION_ID != null)
                {
                    var storageCondition = BackendDataWorker.Get<HIS_STORAGE_CONDITION>().FirstOrDefault(o => o.ID == currrentServiceAdo.STORAGE_CONDITION_ID);
                    if (storageCondition != null && (storageCondition.FROM_TEMPERATURE.HasValue || storageCondition.TO_TEMPERATURE.HasValue))
                    {
                        IsRequiedTemperature = true;
                        lciTemperature.AppearanceItemCaption.ForeColor = Color.Maroon;
                        TemperatureValidationRule valid = new TemperatureValidationRule();
                        valid.spn = spnTemperature;
                        dxValidationProvider2.SetValidationRule(spnTemperature, valid);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDosageForm_Leave(object sender, EventArgs e)
        {
            
        }

        private void cboDosageForm_Validating(object sender, CancelEventArgs e)
        {
        }

        private void ValidatecboDosage(CustomGridLookUpEditWithFilterMultiColumn txt, int maxLength)
        {
            try
            {
                BidMaxLengthValidationRule valid = new BidMaxLengthValidationRule();
                valid.txtBid = txt;
                valid.maxlength = maxLength;
                valid.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txt, valid);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


    }
}
