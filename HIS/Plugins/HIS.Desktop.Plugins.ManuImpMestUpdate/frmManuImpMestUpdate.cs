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
using Inventec.Common.Logging;
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
        #region Declare
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private long impMestId;
        private System.Globalization.CultureInfo cultureLang;
        private HIS_IMP_MEST currentImpMest;
        private HIS_IMP_MEST currentManuImpMest;//HIS_MANU_IMP_MEST

        private MedicineTypeProcessor medicineProcessor = null;
        private MaterialTypeTreeProcessor materialProcessor = null;
        private UserControl ucMedicineTypeTree = null;
        private UserControl ucMaterialTypeTree = null;

        List<V_HIS_BID> listBids = null;
        V_HIS_BID currentBid = null;
        private HIS_SUPPLIER currentSupplier = null;

        private Dictionary<long, List<VHisServicePatyADO>> dicServicePaty = new Dictionary<long, List<VHisServicePatyADO>>();
        private List<VHisServicePatyADO> listServicePatyAdo = new List<VHisServicePatyADO>();
        private List<VHisServiceADO> listServiceADO = new List<VHisServiceADO>();
        private VHisServiceADO currrentServiceAdo = null;
        private ResultImpMestADO resultADO = null;
        private List<V_HIS_MEDI_STOCK> listMediStock = new List<V_HIS_MEDI_STOCK>();
        private List<HIS_IMP_MEST_TYPE> listImpMestType = new List<HIS_IMP_MEST_TYPE>();
        private List<HIS_SUPPLIER> listSupplier = new List<HIS_SUPPLIER>();
        private List<HIS_BID> listBid = new List<HIS_BID>();
        private HIS_IMP_MEST_TYPE currentImpMestType = null;
        private int positionHandleControl = -1;
        private int positionHandleControlLeft = -1;

        List<long> listSupplierIds = new List<long>();
        List<HIS_BID_MEDICINE_TYPE> bidMedicineTypes { get; set; }
        List<HIS_BID_MATERIAL_TYPE> bidMaterialTypes { get; set; }
        List<V_HIS_BID_MEDICINE_TYPE> listBidMedicine;
        List<V_HIS_BID_MATERIAL_TYPE> listBidMaterial;

        #endregion

        public frmManuImpMestUpdate(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmManuImpMestUpdate(Inventec.Desktop.Common.Modules.Module moduleData, long impMestId)
            : this(moduleData)
        {
            try
            {
                // TODO: Complete member initialization
                this.moduleData = moduleData;
                this.impMestId = impMestId;
                this.Text = moduleData.text;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmManuImpMestEdit_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetIcon();

                cboSupplier.Enabled = false;
                //Inventec.Common.Logging.LogSystem.Info("Bat dau khoi tao UC medicineType va materialType InitMedicineTypeTree");
                InitMedicineTypeTree();
                InitMaterialTypeTree();
                //Inventec.Common.Logging.LogSystem.Info("ket thuc khoi tao UC medicineType va materialType InitMedicineTypeTree");
                //Inventec.Common.Logging.LogSystem.Error(" BD ---- ResetValueControlCommon      < 1");
                ResetValueControlCommon();
                //Inventec.Common.Logging.LogSystem.Error(" KT ---- ResetValueControlCommon      < 2");

                //Inventec.Common.Logging.LogSystem.Info("Bat dau goi api HisImpMest/Get LoadCurrentImpMest");
                LoadCurrentImpMest();
                //Inventec.Common.Logging.LogSystem.Info("ket thuc goi api HisImpMest/Get LoadCurrentImpMest");

                //Inventec.Common.Logging.LogSystem.Info("Bat dau goi thread CreateThreadLoadDataByPackageService");
                CreateThreadLoadDataByPackageService();
                //Inventec.Common.Logging.LogSystem.Info("ket thuc goi thread CreateThreadLoadDataByPackageService");

                LoadKeysFromlanguage();

                SetDefaultValueControl();


                ValidControls();

                //LoadCurrentData();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void cboBid_EditValueChangedUCMedicine(long? bidId)
        {
            try
            {
                try
                {
                    if (bidId == null)
                    {
                        WaitingManager.Show();
                        this.currentBid = null;
                        SetDataSourceGridControlMediMateMedicine();
                        WaitingManager.Hide();
                    }
                    else
                    {
                        WaitingManager.Show();
                        this.currentBid = null;
                        if (bidId > 0)
                        {
                            txtSttThau.Enabled = false;
                            txtNhomThau.Enabled = false;
                            txtGoiThau.Enabled = true;

                            txtSttThau.Text = "";
                            txtNhomThau.Text = "";
                            txtGoiThau.Text = "";

                            this.currentBid = listBids.FirstOrDefault(o => o.ID == bidId);
                        }

                        SetDataSourceGridControlMediMateMedicine();
                        WaitingManager.Hide();
                    }
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboBid_EditValueChangedUCMaterial(long? bidId)
        {
            try
            {
                if (bidId == null)
                {
                    WaitingManager.Show();
                    this.currentBid = null;
                    SetDataSourceGridControlMediMateMaterial();
                    WaitingManager.Hide();
                }
                else
                {
                    WaitingManager.Show();
                    this.currentBid = null;
                    if (bidId > 0)
                    {
                        txtSttThau.Enabled = false;
                        txtNhomThau.Enabled = false;
                        txtGoiThau.Enabled = true;

                        txtSttThau.Text = "";
                        txtNhomThau.Text = "";
                        txtGoiThau.Text = "";
                        this.currentBid = listBids.FirstOrDefault(o => o.ID == bidId);
                    }

                    SetDataSourceGridControlMediMateMaterial();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSourceGridControlMediMateMedicine()
        {
            try
            {
                if (this.currentBid != null)
                {
                    listBidMedicine = new List<V_HIS_BID_MEDICINE_TYPE>();
                    var medistock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == currentImpMest.MEDI_STOCK_ID);
                    HisBidMedicineTypeViewFilter mediFilter = new HisBidMedicineTypeViewFilter();
                    mediFilter.BID_ID = this.currentBid.ID;
                    listBidMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MEDICINE_TYPE>>(HisRequestUriStore.HIS_BID_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);
                    List<long> listSupplierIds = new List<long>();

                    if (listBidMedicine != null && listBidMedicine.Count > 0)
                    {
                        if (currentSupplier != null)
                        {
                            listBidMedicine = listBidMedicine.Where(o => o.SUPPLIER_ID == currentSupplier.ID).ToList();
                        }
                    }

                    var listMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_LEAF == 1 && o.IS_STOP_IMP == null).ToList();

                    if (medistock.IS_BUSINESS == 1)
                    {
                        listMedicineType = listMedicineType.Where(o => o.IS_BUSINESS == 1).ToList();
                    }
                    else
                    {
                        listMedicineType = listMedicineType.Where(o => o.IS_BUSINESS != 1).ToList();
                    }

                    if (listBidMedicine.Count > 0)
                    {
                        var listId = listBidMedicine.Select(s => s.MEDICINE_TYPE_ID).ToList();
                        listMedicineType = listMedicineType.Where(o => listId.Contains(o.ID)).ToList();
                    }
                    else
                    {
                        listMedicineType = new List<V_HIS_MEDICINE_TYPE>();
                    }

                    this.medicineProcessor.Reload(this.ucMedicineTypeTree, listMedicineType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSourceGridControlMediMateMaterial()
        {
            try
            {
                if (this.currentBid != null)
                {
                    listBidMaterial = new List<V_HIS_BID_MATERIAL_TYPE>();
                    var medistock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == currentImpMest.MEDI_STOCK_ID);
                    HisBidMaterialTypeViewFilter mateFilter = new HisBidMaterialTypeViewFilter();
                    mateFilter.BID_ID = this.currentBid.ID;
                    listBidMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MATERIAL_TYPE>>(HisRequestUriStore.HIS_BID_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);

                    if (listBidMaterial != null && listBidMaterial.Count > 0)
                    {
                        if (currentSupplier != null)
                        {
                            listBidMaterial = listBidMaterial.Where(o => o.SUPPLIER_ID == currentSupplier.ID).ToList();
                        }
                    }

                    var listMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_LEAF == 1 && o.IS_STOP_IMP == null).ToList();
                    if (medistock.IS_BUSINESS == 1)
                    {
                        listMaterialType = listMaterialType.Where(o => o.IS_BUSINESS == 1).ToList();
                    }
                    else
                    {
                        listMaterialType = listMaterialType.Where(o => o.IS_BUSINESS != 1).ToList();
                    }

                    if (this.currentBid != null)
                    {
                        if (listBidMaterial.Count > 0)
                        {
                            var listId = listBidMaterial.Select(s => s.MATERIAL_TYPE_ID).ToList();
                            listMaterialType = listMaterialType.Where(s => listId.Contains(s.ID)).ToList();
                        }
                        else
                        {
                            listMaterialType = new List<V_HIS_MATERIAL_TYPE>();
                        }
                    }
                    this.materialProcessor.Reload(this.ucMaterialTypeTree, listMaterialType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                //filter.IMP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;
                var impMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GET, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (impMest != null && impMest.Count == 1)
                {
                    this.resultADO = new ResultImpMestADO();
                    this.resultADO.hisInveImpMestSDO = new HisImpMestInveSDO();
                    this.resultADO.hisInveImpMestSDO.ImpMest = new HIS_IMP_MEST();
                    this.resultADO.hisInveImpMestSDO.ImpMest = impMest.FirstOrDefault();

                    this.resultADO.hisInitImpMestSDO = new HisImpMestInitSDO();
                    this.resultADO.hisInitImpMestSDO.ImpMest = new HIS_IMP_MEST();
                    this.resultADO.hisInitImpMestSDO.ImpMest = impMest.FirstOrDefault();

                    this.resultADO.hisManuImpMestSDO = new HisImpMestManuSDO();
                    this.resultADO.hisManuImpMestSDO.ImpMest = new HIS_IMP_MEST();
                    this.resultADO.hisManuImpMestSDO.ImpMest = impMest.FirstOrDefault();

                    this.resultADO.hisOtherImpMestSDO = new HisImpMestOtherSDO();
                    this.resultADO.hisOtherImpMestSDO.ImpMest = new HIS_IMP_MEST();
                    this.resultADO.hisOtherImpMestSDO.ImpMest = impMest.FirstOrDefault();

                    this.resultADO.ImpMestTypeId = impMest.FirstOrDefault().IMP_MEST_TYPE_ID;
                    currentImpMest = impMest.FirstOrDefault();
                    cboImpMestType.EditValue = currentImpMest.IMP_MEST_TYPE_ID;
                    cboMediStock.EditValue = currentImpMest.MEDI_STOCK_ID;
                    txtDescription.Text = currentImpMest.DESCRIPTION;
                    currentManuImpMest = currentImpMest;
                    spinDiscountPrice.Value = currentManuImpMest.DISCOUNT ?? 0;
                    spinDiscountRatio.Value = currentManuImpMest.DISCOUNT_RATIO * 100 ?? 0;
                    spinDocumentPrice.Value = currentManuImpMest.DOCUMENT_PRICE ?? 0;
                    txtDeliver.Text = currentManuImpMest.DELIVERER;
                    txtDocumentNumber.Text = currentManuImpMest.DOCUMENT_NUMBER;
                    dtDocumentDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentManuImpMest.DOCUMENT_DATE ?? 0);
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

        private void CreateThreadLoadDataByPackageService()
        {
            //Thread thread1 = new System.Threading.Thread(LoadDataImpMestNewThread);
            Thread thread2 = new System.Threading.Thread(LoadDataByPackageServiceNewThread);
            //thread1.Priority = ThreadPriority.Normal;
            thread2.Priority = ThreadPriority.Normal;
            try
            {
                //  thread1.Start();
                thread2.Start();

                // thread1.Join();
                //thread2.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                // thread1.Abort();
                thread2.Abort();
            }
        }

        private void LoadDataByPackageServiceNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { LoadCurrentData(); }));
                }
                else
                {
                    LoadCurrentData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataImpMestNewThread()
        {
            try
            {
                LoadCurrentImpMest();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__BTN_ADD");
                this.btnCancel.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__BTN_CANCEL");
                this.btnPrint.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__BTN_PRINT");
                this.btnSave.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__BTN_SAVE");
                this.btnUpdate.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__BTN_UPDATE");

                this.gridColumn_ImpMestDetail_Amount.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_AMOUNT");
                this.gridColumn_ImpMestDetail_ExpiredDate.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_EXPIRED_DATE");
                this.gridColumn_ImpMestDetail_ImpVatRatio.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_IMP_VAT_RATIO");
                this.gridColumn_ImpMestDetail_PackageNumber.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_PACKAGE_NUMBER");
                this.gridColumn_ImpMestDetail_Price.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_PRICE");
                this.gridColumn_ImpMestDetail_Stt.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_STT");
                this.gridColumn_ImpMestDetail_TypeName.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_TYPE_NAME");
                this.gridColumn_ServicePaty_ExpPrice.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_EXP_PRICE");
                this.gridColumn_ServicePaty_NotSell.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_NOT_SELL");
                this.gridColumn_ServicePaty_PatientTypeName.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_PATINET_TYPE_NAME");
                this.gridColumn_ServicePaty_PriceAndVat.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_PRICE_VAT");
                this.gridColumn_ServicePaty_VatRatio.Caption = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_VAT_RATIO");
                this.lciAmount.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_AMOUNT");
                this.lciDescription.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_DESCRIPTION");
                this.lciDescriptionPaty.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_DESCRIPTION_PATY");
                this.lciExpiredDate.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_EXPIRED_DATE");
                this.lciImpMestType.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_IMP_MEST_TYPE");
                this.lciImpPrice.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_IMP_PRICE");
                this.lciImpSource.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_IMP_SOURCE");
                this.lciMediStock.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_MEDI_STOCK");
                this.lciPackageNumber.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_PACKAGE_NUMBER");
                this.lciPriceVat.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_PRICE_VAT");
                this.lciTotalFeePrice.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_TOTAL_FEE_PRICE");
                this.lciTotalPrice.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_TOTAL_PRICE");
                this.lciTotalVatPrice.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_TOTAL_VAT_PRICE");
                this.lciVat.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__LCI_VAT");
                this.repositoryItemButtonDelete.Buttons[0].ToolTip = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_BTN_DELETE");
                this.repositoryItemButtonEdit.Buttons[0].ToolTip = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__GC_BTN_EDIT");
                this.xtraTabPageMaterial.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TAB_MATERIAL");
                this.xtraTabPageMedicine.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__TAB_MEDICINE");
                this.lciSupplier.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__SUPPLIER");
                this.lciDocumentNumber.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__DOCUMENT_NUMBER");
                this.lciDocumentDate.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__DOCUMENT_DATE");
                this.checkOutBid.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__CHECK_OUT_BID");
                this.lciDeliver.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__DELIVER");
                this.lciDiscountRatio.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__DISCOUNT_RATIO");
                this.lciDiscountPrice.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__DISCOUNT_PRICE");
                this.lciDocumentPrice.Text = KeyLanguage(
                    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__DOCUMENT_PRICE");
                //this.lciDescription.Text = KeyLanguage(
                //    "IVT_LANGUAGE_KEY__FORM_MANU_IMP_MEST_EDIT__DESCRIPTION");
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

                //ResetValueControlCommon();

                ResetValueControlDetail();

                LoadImpMestTypeAllow();

                LoadSupplier();

                LoadBid();

                LoadDataToComboImpMestType();

                LoadDataToComboMediStock();

                LoadDataToCboImpSource();

                LoadDataToComboBid();

                SetDefaultImpMestType();

                SetDefaultValueMediStock();

                SetDefaultSupplier();

                SetDefaultBid();

                LoadDataToComboSupplier();

                SetFocuTreeMediMate();

                SetEnableButton(false);

                SetControlEnableImMestTypeManu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlEnableImMestTypeManu()
        {
            try
            {
                if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    //cboSupplier.Enabled = true;
                    //cboBid.Enabled = true;
                    // checkOutBid.Enabled = true;
                    //checkOutBid.Checked = false;
                    txtDocumentNumber.Enabled = true;
                    dtDocumentDate.Enabled = true;
                    //txtDocumentDate.Enabled = true;
                    txtDeliver.Enabled = true;
                    //txtBidNumOrder.Text = "";
                    txtDescription.Enabled = true;
                    spinDiscountPrice.Enabled = true;
                    spinDiscountRatio.Enabled = true;
                    spinDocumentPrice.Enabled = true;

                }
                else if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    //cboBid.Enabled = true;
                    //cboBid.EditValue = null;
                    //checkOutBid.Enabled = true;
                    // checkOutBid.Checked = false;

                    //txtBidNumOrder.Text = "";
                    cboSupplier.EditValue = null;
                    cboSupplier.Enabled = false;
                    txtDocumentNumber.Text = "";
                    txtDocumentNumber.Enabled = false;
                    dtDocumentDate.EditValue = null;
                    dtDocumentDate.Enabled = false;
                    //txtDocumentDate.Enabled = false;
                    txtDeliver.Text = "";
                    txtDeliver.Enabled = false;
                    //txtDescription.Text = "";
                    //txtDescription.Enabled = false;
                    spinDiscountPrice.Value = 0;
                    spinDiscountPrice.Enabled = false;
                    spinDiscountRatio.Value = 0;
                    spinDiscountRatio.Enabled = false;
                    spinDocumentPrice.Value = 0;
                    spinDocumentPrice.Enabled = false;

                    //enable cho control
                    spinAmount.Value = 0;
                    spinAmount.Enabled = true;
                    spinImpPrice.Value = 0;
                    spinImpPrice.Enabled = true;
                    spinImpVatRatio.Value = 0;
                    spinImpVatRatio.Enabled = true;
                    spinImpPriceVAT.Value = 0;
                    spinImpPriceVAT.Enabled = true;
                    txtExpiredDate.Text = "";
                    txtExpiredDate.Enabled = true;
                    txtPackageNumber.Text = "";
                    txtPackageNumber.Enabled = true;
                    btnAdd.Enabled = true;
                    cboImpSource.EditValue = null;
                    cboImpSource.Enabled = true;
                    //txtBid.Text = "";
                    // txtBid.Enabled = true;
                    // txtBidNumber.Text = "";
                    // txtBidNumber.Enabled = true;
                    //txtDescription.Text = "";
                    txtDescription.Enabled = true;
                    //  txtBidNumOrder.Enabled = true;

                    btnSave.Enabled = true;
                }
                else if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                {
                    //cboBid.EditValue = null;
                    //cboBid.Enabled = false;
                    cboSupplier.EditValue = null;
                    cboSupplier.Enabled = false;
                    checkOutBid.Enabled = false;
                    txtDocumentNumber.Text = "";
                    txtDocumentNumber.Enabled = false;
                    dtDocumentDate.EditValue = null;
                    dtDocumentDate.Enabled = false;
                    //txtDocumentDate.Enabled = false;
                    txtDeliver.Text = "";
                    txtDeliver.Enabled = false;
                    //txtBidNumOrder.Text = "";
                    //txtDescription.Text = "";
                    //txtDescription.Enabled = false;
                    spinDiscountPrice.Value = 0;
                    spinDiscountPrice.Enabled = false;
                    spinDiscountRatio.Value = 0;
                    spinDiscountRatio.Enabled = false;
                    spinDocumentPrice.Value = 0;
                    spinDocumentPrice.Enabled = false;

                    //enable cho control
                    spinAmount.Value = 0;
                    spinAmount.Enabled = true;
                    spinImpPrice.Value = 0;
                    spinImpPrice.Enabled = true;
                    spinImpVatRatio.Value = 0;
                    spinImpVatRatio.Enabled = true;
                    spinImpPriceVAT.Value = 0;
                    spinImpPriceVAT.Enabled = true;
                    txtExpiredDate.Text = "";
                    txtExpiredDate.Enabled = true;
                    txtPackageNumber.Text = "";
                    txtPackageNumber.Enabled = true;
                    btnAdd.Enabled = true;
                    cboImpSource.EditValue = null;
                    cboImpSource.Enabled = true;
                    //txtBid.Text = "";
                    //txtBid.Enabled = true;
                    //txtBidNumber.Text = "";
                    //txtBidNumber.Enabled = true;
                    //txtDescription.Text = "";
                    txtDescription.Enabled = true;
                    // txtBidNumOrder.Enabled = true;

                    btnSave.Enabled = true;
                }
                else
                {
                    // txtBidNumOrder.Text = "";
                    //cboBid.EditValue = null;
                    //cboBid.Enabled = false;
                    cboSupplier.EditValue = null;
                    cboSupplier.Enabled = false;
                    checkOutBid.Enabled = false;
                    txtDocumentNumber.Text = "";
                    txtDocumentNumber.Enabled = false;
                    dtDocumentDate.EditValue = null;
                    dtDocumentDate.Enabled = false;
                    //txtDocumentDate.Enabled = false;
                    txtDeliver.Text = "";
                    txtDeliver.Enabled = false;
                    //txtDescription.Text = "";
                    //txtDescription.Enabled = false;
                    spinDiscountPrice.Value = 0;
                    spinDiscountPrice.Enabled = false;
                    spinDiscountRatio.Value = 0;
                    spinDiscountRatio.Enabled = false;
                    spinDocumentPrice.Value = 0;
                    spinDocumentPrice.Enabled = false;

                    //enable cho control
                    spinAmount.Value = 0;
                    spinAmount.Enabled = true;
                    spinImpPrice.Value = 0;
                    spinImpPrice.Enabled = true;
                    spinImpVatRatio.Value = 0;
                    spinImpVatRatio.Enabled = true;
                    spinImpPriceVAT.Value = 0;
                    spinImpPriceVAT.Enabled = true;
                    txtExpiredDate.Text = "";
                    txtExpiredDate.Enabled = true;
                    txtPackageNumber.Text = "";
                    txtPackageNumber.Enabled = true;
                    btnAdd.Enabled = true;
                    cboImpSource.EditValue = null;
                    cboImpSource.Enabled = true;
                    //txtDescription.Text = "";
                    txtDescription.Enabled = true;
                    //  txtBidNumOrder.Enabled = true;

                    btnSave.Enabled = true;
                }
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
                var medistock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == currentImpMest.MEDI_STOCK_ID);
                CommonParam param = new CommonParam();
                bidMedicineTypes = new List<HIS_BID_MEDICINE_TYPE>();
                bidMaterialTypes = new List<HIS_BID_MATERIAL_TYPE>();
                var listMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_LEAF == 1 && o.IS_STOP_IMP != 1).ToList();

                if (medistock.IS_BUSINESS == 1)
                {
                    listMedicineType = listMedicineType.Where(o => o.IS_BUSINESS == 1).ToList();
                }
                else
                {
                    listMedicineType = listMedicineType.Where(o => o.IS_BUSINESS != 1).ToList();
                }

                var listMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_LEAF == 1 && o.IS_STOP_IMP != 1).ToList();

                if (medistock.IS_BUSINESS == 1)
                {
                    listMaterialType = listMaterialType.Where(o => o.IS_BUSINESS == 1).ToList();
                }
                else
                {
                    listMaterialType = listMaterialType.Where(o => o.IS_BUSINESS != 1).ToList();
                }

                //Inventec.Common.Logging.LogSystem.Info("bat dau reload uc medicineType, materialType medicineProcessor.Reload");
                this.medicineProcessor.Reload(this.ucMedicineTypeTree, listMedicineType);
                this.materialProcessor.Reload(this.ucMaterialTypeTree, listMaterialType);
                //Inventec.Common.Logging.LogSystem.Info("ket thuc reload uc medicineType, materialType medicineProcessor.Reload");
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
                cboSupplier.EditValue = null;
                cboMediStock.Properties.DataSource = null;
                //btnPrint.Enabled = false;
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

                txtSttThau.Text = "";
                txtGoiThau.Text = "";
                txtNhomThau.Text = "";
                chkImprice.Checked = false;
                chkImprice.Enabled = true;
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

        private void SetDefaultImpMestType()
        {
            try
            {
                listMediStock = new List<V_HIS_MEDI_STOCK>();
                this.currentImpMestType = null;
                this.currentImpMestType = listImpMestType.FirstOrDefault(o => o.ID == this.currentManuImpMest.IMP_MEST_TYPE_ID);
                if (this.currentImpMestType != null)
                {
                    cboImpMestType.EditValue = this.currentImpMestType.ID;
                    listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                    //.Where(o => o.IS_ALLOW_IMP_SUPPLIER == 1).ToList();
                }
                cboMediStock.Properties.DataSource = listMediStock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultSupplier()
        {
            try
            {
                if (this.currentManuImpMest != null)
                {
                    cboSupplier.EditValue = this.currentManuImpMest.SUPPLIER_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultBid()
        {
            try
            {
                //if (this.currentManuImpMest != null)
                //{
                //    cboBid.EditValue = this.currentManuImpMest.BID_ID;
                //    if (cboBid.EditValue == null)
                //    {
                //        checkOutBid.Checked = true;
                //    }
                //    else
                //    {
                //        cboSupplier.Enabled = false;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueMediStock()
        {
            try
            {
                if (!cboMediStock.Enabled || cboMediStock.Properties.DataSource == null || moduleData.RoomId <= 0)
                    return;
                var medistock = listMediStock.FirstOrDefault(o => o.ID == currentImpMest.MEDI_STOCK_ID);
                if (medistock != null)
                {
                    cboMediStock.EditValue = medistock.ID;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFocuTreeMediMate()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("go to Focus F2");
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
                if (this.currrentServiceAdo != null && chkImprice.Checked == false)// && CheckAllowAdd())
                {
                    //if (!dicServicePaty.ContainsKey(this.currrentServiceAdo.SERVICE_ID) || dicServicePaty[this.currrentServiceAdo.SERVICE_ID].Count == 0)
                    //{
                    Dictionary<long, VHisServicePatyADO> dicPaty = new Dictionary<long, VHisServicePatyADO>();

                    var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_PATY>>(HisRequestUriStore.HIS_SERVICE_PATY_GET_APPLIED_VIEW, ApiConsumers.MosConsumer, null, null, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, "serviceId", this.currrentServiceAdo.SERVICE_ID, "treatmentTime", null);

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
                    //}
                    var listData = dicServicePaty[this.currrentServiceAdo.SERVICE_ID];
                    AutoMapper.Mapper.CreateMap<VHisServicePatyADO, VHisServicePatyADO>();
                    listServicePatyAdo = AutoMapper.Mapper.Map<List<VHisServicePatyADO>>(listData);
                    foreach (var item in listServicePatyAdo)
                    {
                        item.ExpPriceVat = item.PRICE * (1 + item.VAT_RATIO);
                    }
                }
                else
                {
                    listServicePatyAdo.Clear();
                }

                listServicePatyAdo = listServicePatyAdo.OrderByDescending(o => o.IsNotSell == false).ToList();
                gridControlServicePaty.BeginUpdate();
                gridControlServicePaty.DataSource = listServicePatyAdo;
                gridControlServicePaty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateServicePatyByImpPrice()
        {
            try
            {
                foreach (var item in listServicePatyAdo)
                {
                    item.PRICE = spinImpPriceVAT.Value;
                    item.ExpVatRatio = spinImpVatRatio.Value;
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
                if (currrentServiceAdo != null && currrentServiceAdo.IsRequireHsd)
                {
                    if (dtExpiredDate.EditValue == null || String.IsNullOrEmpty(txtExpiredDate.Text))
                    {
                        messageError = Resources.ResourceMessage.ChuaNhapHanSuDung;
                        return false;
                    }
                }
                if (dtExpiredDate.EditValue != null)
                {
                    if ((Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "235959")) < (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0))//fromTime < DateTime.Now)
                    {
                        messageError = Resources.ResourceMessage.HanSuDungKhongDuocNhoHonThoiGianHienTai;
                        return false;
                    }
                }
                if (xtraTabControl.SelectedTabPage == xtraTabPageMedicine)
                {
                    if (medicineProcessor.GetBid(this.ucMedicineTypeTree) == null && !checkOutBid.Checked)
                    {
                        messageError = Resources.ResourceMessage.LoaiNhapLaNhapTuNhaCungCaoNguoiDungPhaiChonGoiThauHoacTichVaoNgoaiThau;
                        return false;
                    }
                }

                if (xtraTabControl.SelectedTabPage == xtraTabPageMaterial)
                {
                    if (materialProcessor.GetBid(this.ucMaterialTypeTree) == null && !checkOutBid.Checked)
                    {
                        messageError = Resources.ResourceMessage.LoaiNhapLaNhapTuNhaCungCaoNguoiDungPhaiChonGoiThauHoacTichVaoNgoaiThau;
                        return false;
                    }
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
                cboSupplier.Enabled = !enable;
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
                List<VHisServicePatyADO> CheckData = new List<VHisServicePatyADO>();
                if (listServicePatyAdo != null)
                {
                    CheckData = listServicePatyAdo.Where(o => o.IsNotSell == false).ToList();
                }

                WaitingManager.Hide();
                if (chkImprice.Checked == false && CheckData.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Cần nhập chính sách giá hoặc tích giá bán bằng giá nhập", "Thông báo");
                    return;
                }

                vHisServiceADO.IMP_AMOUNT = spinAmount.Value;
                vHisServiceADO.IMP_PRICE = spinImpPrice.Value;
                vHisServiceADO.IMP_VAT_RATIO = spinImpVatRatio.Value / 100;
                vHisServiceADO.ImpVatRatio = spinImpVatRatio.Value;

                if (dtExpiredDate.EditValue != null && dtExpiredDate.DateTime != DateTime.MinValue)
                {
                    vHisServiceADO.EXPIRED_DATE = Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "235959");
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
                if (vHisServiceADO.VHisServicePatys == null)
                {
                    vHisServiceADO.VHisServicePatys = new List<VHisServicePatyADO>();
                }

                if (vHisServiceADO.IsMedicine)
                {
                    vHisServiceADO.HisMedicine.ID = vHisServiceADO.HisMedicine.ID;
                    vHisServiceADO.HisMedicine.MEDICINE_TYPE_ID = vHisServiceADO.HisMedicine.MEDICINE_TYPE_ID;

                    if (medicineProcessor.GetBid(ucMedicineTypeTree) != null)
                    {
                        vHisServiceADO.HisMedicine.TDL_BID_GROUP_CODE = this.currrentServiceAdo.BID_GROUP_CODE;
                        vHisServiceADO.HisMedicine.TDL_BID_NUM_ORDER = this.currrentServiceAdo.BID_NUM_ORDER;
                        vHisServiceADO.HisMedicine.TDL_BID_PACKAGE_CODE = this.currrentServiceAdo.BID_PACKAGE_CODE;
                        vHisServiceADO.HisMedicine.TDL_BID_YEAR = this.currrentServiceAdo.BID_YEAR;
                        vHisServiceADO.HisMedicine.TDL_BID_NUMBER = this.currrentServiceAdo.BID_NUMBER;
                    }
                    else
                    {
                        vHisServiceADO.HisMedicine.TDL_BID_GROUP_CODE = txtNhomThau.Text;
                        vHisServiceADO.HisMedicine.TDL_BID_NUM_ORDER = txtSttThau.Text;
                        vHisServiceADO.HisMedicine.TDL_BID_PACKAGE_CODE = txtGoiThau.Text;
                        vHisServiceADO.BID_GROUP_CODE = txtNhomThau.Text;
                        vHisServiceADO.BID_NUM_ORDER = txtSttThau.Text;
                        vHisServiceADO.BID_PACKAGE_CODE = txtGoiThau.Text;
                    }

                    if (chkImprice.Checked == true)
                    {
                        vHisServiceADO.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = 1;
                    }
                    else
                    {
                        vHisServiceADO.HisMedicine.IS_SALE_EQUAL_IMP_PRICE = null;
                    }
                    vHisServiceADO.HisMedicine.AMOUNT = vHisServiceADO.IMP_AMOUNT;
                    vHisServiceADO.HisMedicine.IMP_PRICE = vHisServiceADO.IMP_PRICE;
                    vHisServiceADO.HisMedicine.IMP_VAT_RATIO = vHisServiceADO.IMP_VAT_RATIO;
                    vHisServiceADO.HisMedicine.PACKAGE_NUMBER = vHisServiceADO.PACKAGE_NUMBER;
                    vHisServiceADO.HisMedicine.EXPIRED_DATE = vHisServiceADO.EXPIRED_DATE;
                    vHisServiceADO.BidId = medicineProcessor.GetBid(this.ucMedicineTypeTree);
                    if (vHisServiceADO.HisMedicinePatys == null)
                    {
                        vHisServiceADO.HisMedicinePatys = new List<HIS_MEDICINE_PATY>();
                    }
                    //vHisServiceADO.HisMedicinePatys = new List<HIS_MEDICINE_PATY>();
                    foreach (var paty in vHisServiceADO.VHisServicePatys)
                    {
                        if (!paty.IsNotSell)
                        {
                            var mediPaty = vHisServiceADO.HisMedicinePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == paty.PATIENT_TYPE_ID);
                            if (mediPaty != null)
                            {
                                mediPaty.EXP_PRICE = paty.PRICE;
                                mediPaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                mediPaty.ID = paty.ID;
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
                    vHisServiceADO.HisMaterial.ID = vHisServiceADO.HisMaterial.ID;
                    vHisServiceADO.HisMaterial.MATERIAL_TYPE_ID = vHisServiceADO.HisMaterial.MATERIAL_TYPE_ID;

                    if (materialProcessor.GetBid(ucMaterialTypeTree) != null)
                    {
                        vHisServiceADO.HisMaterial.TDL_BID_GROUP_CODE = this.currrentServiceAdo.BID_GROUP_CODE;
                        vHisServiceADO.HisMaterial.TDL_BID_NUM_ORDER = this.currrentServiceAdo.BID_NUM_ORDER;
                        vHisServiceADO.HisMaterial.TDL_BID_PACKAGE_CODE = this.currrentServiceAdo.BID_PACKAGE_CODE;
                        vHisServiceADO.HisMaterial.TDL_BID_YEAR = this.currrentServiceAdo.BID_YEAR;
                        vHisServiceADO.HisMaterial.TDL_BID_NUMBER = this.currrentServiceAdo.BID_NUMBER;
                    }
                    else
                    {
                        vHisServiceADO.HisMaterial.TDL_BID_GROUP_CODE = txtNhomThau.Text;
                        vHisServiceADO.HisMaterial.TDL_BID_NUM_ORDER = txtSttThau.Text;
                        vHisServiceADO.HisMaterial.TDL_BID_PACKAGE_CODE = txtGoiThau.Text;
                        vHisServiceADO.BID_GROUP_CODE = txtNhomThau.Text;
                        vHisServiceADO.BID_NUM_ORDER = txtSttThau.Text;
                        vHisServiceADO.BID_PACKAGE_CODE = txtGoiThau.Text;
                    }
                    vHisServiceADO.BidId = materialProcessor.GetBid(this.ucMaterialTypeTree);

                    if (chkImprice.Checked == true)
                    {
                        vHisServiceADO.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = 1;
                    }
                    else
                    {
                        vHisServiceADO.HisMaterial.IS_SALE_EQUAL_IMP_PRICE = null;
                    }

                    vHisServiceADO.HisMaterial.AMOUNT = vHisServiceADO.IMP_AMOUNT;
                    vHisServiceADO.HisMaterial.IMP_PRICE = vHisServiceADO.IMP_PRICE;
                    vHisServiceADO.HisMaterial.IMP_VAT_RATIO = vHisServiceADO.IMP_VAT_RATIO;
                    vHisServiceADO.HisMaterial.PACKAGE_NUMBER = vHisServiceADO.PACKAGE_NUMBER;
                    vHisServiceADO.HisMaterial.EXPIRED_DATE = vHisServiceADO.EXPIRED_DATE;
                    if (vHisServiceADO.HisMaterialPatys == null)
                    {
                        vHisServiceADO.HisMaterialPatys = new List<HIS_MATERIAL_PATY>();
                    }
                    // vHisServiceADO.HisMedicinePatys = new List<HIS_MEDICINE_PATY>();
                    foreach (var paty in vHisServiceADO.VHisServicePatys)
                    {
                        if (!paty.IsNotSell)
                        {
                            var matePaty = vHisServiceADO.HisMaterialPatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == paty.PATIENT_TYPE_ID);
                            if (matePaty != null)
                            {
                                matePaty.EXP_PRICE = paty.PRICE;
                                matePaty.EXP_VAT_RATIO = paty.VAT_RATIO;
                                matePaty.ID = paty.ID;
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

        private object GetImpMestTypeSDORequest(HIS_IMP_MEST_TYPE impMestType, long impMestSttId)
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

                long mediStockId = 0;
                if (cboMediStock.EditValue != null)
                {
                    var mediStock = listMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboMediStock.EditValue));
                    if (mediStock != null)
                    {
                        mediStockId = mediStock.ID;
                    }
                }

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
                        mediSdo.MedicinePaties = ado.HisMedicinePatys;
                        mediSdo.Medicine.EXPIRED_DATE = ado.EXPIRED_DATE;
                        mediSdo.Medicine.IS_SALE_EQUAL_IMP_PRICE = ado.HisMedicine.IS_SALE_EQUAL_IMP_PRICE;
                        mediSdo.Medicine.TDL_BID_GROUP_CODE = ado.BID_GROUP_CODE;
                        mediSdo.Medicine.TDL_BID_NUM_ORDER = ado.BID_NUM_ORDER;
                        mediSdo.Medicine.TDL_BID_NUMBER = ado.BID_NUMBER;
                        mediSdo.Medicine.TDL_BID_PACKAGE_CODE = ado.BID_PACKAGE_CODE;
                        mediSdo.Medicine.TDL_BID_YEAR = ado.BID_YEAR;
                        mediSdo.Medicine.BID_ID = ado.BidId;
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
                        mateSdo.Material.EXPIRED_DATE = ado.EXPIRED_DATE;
                        mateSdo.Material.PACKAGE_NUMBER = ado.PACKAGE_NUMBER;
                        mateSdo.Material.IS_SALE_EQUAL_IMP_PRICE = ado.HisMaterial.IS_SALE_EQUAL_IMP_PRICE;
                        mateSdo.Material.TDL_BID_GROUP_CODE = ado.BID_GROUP_CODE;
                        mateSdo.Material.TDL_BID_NUM_ORDER = ado.BID_NUM_ORDER;
                        mateSdo.Material.TDL_BID_NUMBER = ado.BID_NUMBER;
                        mateSdo.Material.TDL_BID_PACKAGE_CODE = ado.BID_PACKAGE_CODE;
                        mateSdo.Material.TDL_BID_YEAR = ado.BID_YEAR;
                        mateSdo.Material.BID_ID = ado.BidId;
                        mateSdo.MaterialPaties = ado.HisMaterialPatys;
                        hisMaterialSdos.Add(mateSdo);
                    }
                }

                if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    //Review
                    HisImpMestManuSDO hisManuImpMestSDO = new HisImpMestManuSDO();
                    hisManuImpMestSDO.ImpMest = new HIS_IMP_MEST();
                    //hisManuImpMestSDO.ManuBloods = new List<HisBloodWithPatySDO>();
                    hisManuImpMestSDO.ManuMaterials = new List<HisMaterialWithPatySDO>();
                    hisManuImpMestSDO.ManuMedicines = new List<HisMedicineWithPatySDO>();
                    if (resultADO != null)
                    {
                        hisManuImpMestSDO = resultADO.hisManuImpMestSDO;
                    }
                    else
                    {
                        hisManuImpMestSDO.ImpMest = currentImpMest;
                        //hisManuImpMestSDO.ManuImpMest = currentManuImpMest;
                    }

                    hisManuImpMestSDO.ImpMest.REQ_ROOM_ID = this.moduleData.RoomId;
                    hisManuImpMestSDO.ImpMest.MEDI_STOCK_ID = mediStockId;
                    hisManuImpMestSDO.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    hisManuImpMestSDO.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    hisManuImpMestSDO.ImpMest.DESCRIPTION = txtDescription.Text;
                    hisManuImpMestSDO.ImpMest.DELIVERER = txtDeliver.Text;
                    hisManuImpMestSDO.ImpMest.DISCOUNT = spinDiscountPrice.Value;
                    hisManuImpMestSDO.ImpMest.DISCOUNT_RATIO = spinDiscountRatio.Value / 100;
                    hisManuImpMestSDO.ImpMest.DOCUMENT_PRICE = spinDocumentPrice.Value;
                    if (cboSupplier.EditValue != null)
                    {
                        hisManuImpMestSDO.ImpMest.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString());
                    }
                    hisManuImpMestSDO.ImpMest.DOCUMENT_NUMBER = txtDocumentNumber.Text;
                    if (dtDocumentDate.EditValue != null && dtDocumentDate.DateTime != DateTime.MinValue)
                    {
                        hisManuImpMestSDO.ImpMest.DOCUMENT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtDocumentDate.EditValue).ToString("yyyyMMddHHmm") + "00");
                    }
                    hisManuImpMestSDO.ManuMaterials = hisMaterialSdos;
                    hisManuImpMestSDO.ManuMedicines = hisMedicineSdos;
                    return hisManuImpMestSDO;
                }
                else if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    //Review
                    HisImpMestInitSDO hisInitImpMestSDO = new HisImpMestInitSDO();
                    hisInitImpMestSDO.ImpMest = new HIS_IMP_MEST();
                    //hisManuImpMestSDO.ManuBloods = new List<HisBloodWithPatySDO>();
                    hisInitImpMestSDO.InitMaterials = new List<HisMaterialWithPatySDO>();
                    hisInitImpMestSDO.InitMedicines = new List<HisMedicineWithPatySDO>();
                    if (resultADO != null)
                    {
                        hisInitImpMestSDO = resultADO.hisInitImpMestSDO;
                    }
                    else
                    {
                        hisInitImpMestSDO.ImpMest = currentImpMest;
                        //hisManuImpMestSDO.ManuImpMest = currentManuImpMest;
                    }

                    hisInitImpMestSDO.ImpMest.REQ_ROOM_ID = this.moduleData.RoomId;
                    hisInitImpMestSDO.ImpMest.MEDI_STOCK_ID = mediStockId;
                    hisInitImpMestSDO.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    hisInitImpMestSDO.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    hisInitImpMestSDO.ImpMest.DESCRIPTION = txtDescription.Text;
                    hisInitImpMestSDO.ImpMest.DELIVERER = txtDeliver.Text;
                    hisInitImpMestSDO.ImpMest.DISCOUNT = spinDiscountPrice.Value;
                    hisInitImpMestSDO.ImpMest.DISCOUNT_RATIO = spinDiscountRatio.Value / 100;
                    hisInitImpMestSDO.ImpMest.DOCUMENT_PRICE = spinDocumentPrice.Value;
                    if (cboSupplier.EditValue != null)
                    {
                        hisInitImpMestSDO.ImpMest.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString());
                    }
                    hisInitImpMestSDO.ImpMest.DOCUMENT_NUMBER = txtDocumentNumber.Text;
                    if (dtDocumentDate.EditValue != null && dtDocumentDate.DateTime != DateTime.MinValue)
                    {
                        hisInitImpMestSDO.ImpMest.DOCUMENT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtDocumentDate.EditValue).ToString("yyyyMMddHHmm") + "00");
                    }
                    hisInitImpMestSDO.InitMaterials = hisMaterialSdos;
                    hisInitImpMestSDO.InitMedicines = hisMedicineSdos;
                    return hisInitImpMestSDO;
                }

                else if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                {
                    //Review
                    HisImpMestInveSDO hisInveImpMestSDO = new HisImpMestInveSDO();
                    hisInveImpMestSDO.ImpMest = new HIS_IMP_MEST();
                    //hisManuImpMestSDO.ManuBloods = new List<HisBloodWithPatySDO>();
                    hisInveImpMestSDO.InveMaterials = new List<HisMaterialWithPatySDO>();
                    hisInveImpMestSDO.InveMedicines = new List<HisMedicineWithPatySDO>();
                    if (resultADO != null)
                    {
                        hisInveImpMestSDO = resultADO.hisInveImpMestSDO;
                    }
                    else
                    {
                        hisInveImpMestSDO.ImpMest = currentImpMest;
                        //hisManuImpMestSDO.ManuImpMest = currentManuImpMest;
                    }

                    //else
                    //{
                    //    hisInveImpMestSDO.ImpMest.BID_ID = null;
                    //}
                    hisInveImpMestSDO.ImpMest.REQ_ROOM_ID = this.moduleData.RoomId;
                    hisInveImpMestSDO.ImpMest.MEDI_STOCK_ID = mediStockId;
                    hisInveImpMestSDO.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    hisInveImpMestSDO.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    hisInveImpMestSDO.ImpMest.DESCRIPTION = txtDescription.Text;
                    hisInveImpMestSDO.ImpMest.DELIVERER = txtDeliver.Text;
                    hisInveImpMestSDO.ImpMest.DISCOUNT = spinDiscountPrice.Value;
                    hisInveImpMestSDO.ImpMest.DISCOUNT_RATIO = spinDiscountRatio.Value / 100;
                    hisInveImpMestSDO.ImpMest.DOCUMENT_PRICE = spinDocumentPrice.Value;
                    if (cboSupplier.EditValue != null)
                    {
                        hisInveImpMestSDO.ImpMest.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString());
                    }
                    hisInveImpMestSDO.ImpMest.DOCUMENT_NUMBER = txtDocumentNumber.Text;
                    if (dtDocumentDate.EditValue != null && dtDocumentDate.DateTime != DateTime.MinValue)
                    {
                        hisInveImpMestSDO.ImpMest.DOCUMENT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtDocumentDate.EditValue).ToString("yyyyMMddHHmm") + "00");
                    }
                    hisInveImpMestSDO.InveMaterials = hisMaterialSdos;
                    hisInveImpMestSDO.InveMedicines = hisMedicineSdos;
                    return hisInveImpMestSDO;
                }
                else if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                {
                    //Review
                    HisImpMestOtherSDO hisOtherImpMestSDO = new HisImpMestOtherSDO();
                    hisOtherImpMestSDO.ImpMest = new HIS_IMP_MEST();
                    //hisManuImpMestSDO.ManuBloods = new List<HisBloodWithPatySDO>();
                    hisOtherImpMestSDO.OtherMaterials = new List<HisMaterialWithPatySDO>();
                    hisOtherImpMestSDO.OtherMedicines = new List<HisMedicineWithPatySDO>();
                    if (resultADO != null)
                    {
                        hisOtherImpMestSDO = resultADO.hisOtherImpMestSDO;
                    }
                    else
                    {
                        hisOtherImpMestSDO.ImpMest = currentImpMest;
                        //hisManuImpMestSDO.ManuImpMest = currentManuImpMest;
                    }

                    //else
                    //{
                    //    hisInveImpMestSDO.ImpMest.BID_ID = null;
                    //}
                    hisOtherImpMestSDO.ImpMest.REQ_ROOM_ID = this.moduleData.RoomId;
                    hisOtherImpMestSDO.ImpMest.MEDI_STOCK_ID = mediStockId;
                    hisOtherImpMestSDO.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    hisOtherImpMestSDO.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    hisOtherImpMestSDO.ImpMest.DESCRIPTION = txtDescription.Text;
                    hisOtherImpMestSDO.ImpMest.DELIVERER = txtDeliver.Text;
                    hisOtherImpMestSDO.ImpMest.DISCOUNT = spinDiscountPrice.Value;
                    hisOtherImpMestSDO.ImpMest.DISCOUNT_RATIO = spinDiscountRatio.Value / 100;
                    hisOtherImpMestSDO.ImpMest.DOCUMENT_PRICE = spinDocumentPrice.Value;
                    if (cboSupplier.EditValue != null)
                    {
                        hisOtherImpMestSDO.ImpMest.SUPPLIER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString());
                    }
                    hisOtherImpMestSDO.ImpMest.DOCUMENT_NUMBER = txtDocumentNumber.Text;
                    if (dtDocumentDate.EditValue != null && dtDocumentDate.DateTime != DateTime.MinValue)
                    {
                        hisOtherImpMestSDO.ImpMest.DOCUMENT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtDocumentDate.EditValue).ToString("yyyyMMddHHmm") + "00");
                    }
                    hisOtherImpMestSDO.OtherMaterials = hisMaterialSdos;
                    hisOtherImpMestSDO.OtherMedicines = hisMedicineSdos;
                    return hisOtherImpMestSDO;
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

        private bool CheckAddUpdateMedicineMaterial()
        {
            if (this.currentManuImpMest != null)
            {
                var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.currentManuImpMest.MEDI_STOCK_ID);
                //if (mediStock != null && mediStock.IS_BUSINESS == 1)
                //{
                //}
                //else
                //    return true;
            }
            bool result = false;
            try
            {
                this.positionHandleControlLeft = -1;
                if (!dxValidationProviderLeft.Validate())
                    result = false;
                for (int i = 0; i < gridViewServicePaty.RowCount; i++)
                {
                    int rowHandle = gridViewServicePaty.GetVisibleRowHandle(i);
                    if (gridViewServicePaty.IsDataRow(rowHandle))
                    {
                        if (Convert.ToDouble(gridViewServicePaty.GetRowCellValue(rowHandle, "PRICE").ToString()) < 0)
                        {
                            result = false;
                            break;
                        }
                        else if (Convert.ToDouble(gridViewServicePaty.GetRowCellValue(rowHandle, "ExpVatRatio").ToString()) < 0 || Convert.ToDouble(gridViewServicePaty.GetRowCellValue(rowHandle, "ExpVatRatio").ToString()) > 100)
                        {
                            result = false;
                            break;
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

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
                positionHandleControlLeft = -1;
                if (!dxValidationProviderLeft.Validate() || this.currrentServiceAdo == null)//|| CheckAddUpdateMedicineMaterial() == false
                    return;

                WaitingManager.Show();

                AddData(ref this.currrentServiceAdo);

                var dataThayThe = listServiceADO.FirstOrDefault(o => o.SERVICE_ID == this.currrentServiceAdo.SERVICE_ID);
                if (dataThayThe != null)
                {
                    WaitingManager.Hide();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.ThuocVatTuDaCoTrongDanhSachNhap, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }

                }

                List<VHisServicePatyADO> CheckData = new List<VHisServicePatyADO>();
                if (listServicePatyAdo != null)
                {
                    CheckData = listServicePatyAdo.Where(o => o.IsNotSell == false).ToList();
                }

                if (chkImprice.Checked == false && CheckData.Count == 0)
                {
                    return;
                }

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
                if (!btnUpdate.Enabled || !dxValidationProviderLeft.Validate() || this.currrentServiceAdo == null)//|| CheckAddUpdateMedicineMaterial() == false
                    return;
                string messageError = "";
                if (!CheckAllowAdd(ref messageError))
                {
                    MessageManager.Show(messageError);
                    return;
                }
                WaitingManager.Show();
                AddData(ref this.currrentServiceAdo);
                List<VHisServicePatyADO> CheckData = new List<VHisServicePatyADO>();
                if (listServicePatyAdo != null)
                {
                    CheckData = listServicePatyAdo.Where(o => o.IsNotSell == false).ToList();
                }

                if (chkImprice.Checked == false && CheckData.Count == 0)
                {
                    return;
                }
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

        private bool CheckDocumentNumber(string documentNumber)
        {
            bool result = false;
            try
            {
                long supplierId = 0;

                if (cboSupplier.EditValue != null)
                {
                    supplierId = Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString());
                }
                MOS.Filter.HisImpMestViewFilter manuImpMestViewFilter = new HisImpMestViewFilter();
                manuImpMestViewFilter.DOCUMENT_NUMBER__EXACT = documentNumber;
                var manuImpMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, manuImpMestViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, new CommonParam());
                if (manuImpMests != null && manuImpMests.Count > 0 && !string.IsNullOrEmpty(documentNumber))
                {
                    foreach (var item in manuImpMests)
                    {
                        if (item.DOCUMENT_NUMBER.Equals(documentNumber.Trim()) && item.ID != this.impMestId && supplierId == item.SUPPLIER_ID)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Focus();
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.listServiceADO.Count <= 0 || CheckSaveUpdateMedicineMaterial() == false)
                    return;

                if (CheckDocumentNumberV2(txtDocumentNumber.Text))
                {
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
                if (impMestType != null)
                {
                    var sdo = GetImpMestTypeSDORequest(impMestType, IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST);
                    if (sdo != null)
                    {
                        if (sdo.GetType() == typeof(HisImpMestManuSDO))
                        {
                            var data = (HisImpMestManuSDO)sdo;
                            if (data.ManuMaterials != null && data.ManuMaterials.Count > 0)
                            {
                                data.ManuMaterials.ForEach(
                                t => t.MaterialPaties = t.Material != null && t.Material.IS_SALE_EQUAL_IMP_PRICE == 1 ? null : t.MaterialPaties
                            );
                            }
                            if (data.ManuMedicines != null && data.ManuMedicines.Count > 0)
                            {
                                data.ManuMedicines.ForEach(
                                t => t.MedicinePaties = t.Medicine != null && t.Medicine.IS_SALE_EQUAL_IMP_PRICE == 1 ? null : t.MedicinePaties
                            );
                            }
                            HisImpMestManuSDO rs = null;
                            Inventec.Common.Logging.LogSystem.Error("BD goi api---------------------");
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestManuSDO>("api/HisImpMest/ManuUpdate", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                            Inventec.Common.Logging.LogSystem.Error("KT goi api");
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                            }
                        }
                        else if (sdo.GetType() == typeof(HisImpMestInitSDO))
                        {
                            var data = (HisImpMestInitSDO)sdo;
                            if (data.InitMaterials != null && data.InitMaterials.Count > 0)
                            {
                                data.InitMaterials.ForEach(
                                t => t.MaterialPaties = t.Material != null && t.Material.IS_SALE_EQUAL_IMP_PRICE == 1 ? null : t.MaterialPaties
                            );
                            }
                            if (data.InitMedicines != null && data.InitMedicines.Count > 0)
                            {
                                data.InitMedicines.ForEach(
                                t => t.MedicinePaties = t.Medicine != null && t.Medicine.IS_SALE_EQUAL_IMP_PRICE == 1 ? null : t.MedicinePaties
                            );
                            }

                            HisImpMestInitSDO rs = null;
                            Inventec.Common.Logging.LogSystem.Error("BD goi api---------------------");
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInitSDO>("api/HisImpMest/InitUpdate", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                            Inventec.Common.Logging.LogSystem.Error("KT goi api");
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                            }
                        }
                        else if (sdo.GetType() == typeof(HisImpMestInveSDO))
                        {
                            var data = (HisImpMestInveSDO)sdo;
                            if (data.InveMaterials != null && data.InveMaterials.Count > 0)
                            {
                                data.InveMaterials.ForEach(
                                t => t.MaterialPaties = t.Material != null && t.Material.IS_SALE_EQUAL_IMP_PRICE == 1 ? null : t.MaterialPaties
                            );
                            }
                            if (data.InveMedicines != null && data.InveMedicines.Count > 0)
                            {
                                data.InveMedicines.ForEach(
                                t => t.MedicinePaties = t.Medicine != null && t.Medicine.IS_SALE_EQUAL_IMP_PRICE == 1 ? null : t.MedicinePaties
                            );
                            }

                            HisImpMestInveSDO rs = null;
                            Inventec.Common.Logging.LogSystem.Error("BD goi api---------------------");
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInveSDO>("api/HisImpMest/InveUpdate", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                            Inventec.Common.Logging.LogSystem.Error("KT goi api");
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                            }
                        }
                        else if (sdo.GetType() == typeof(HisImpMestOtherSDO))
                        {
                            var data = (HisImpMestOtherSDO)sdo;
                            if (data.OtherMaterials != null && data.OtherMaterials.Count > 0)
                            {
                                data.OtherMaterials.ForEach(
                                t => t.MaterialPaties = t.Material != null && t.Material.IS_SALE_EQUAL_IMP_PRICE == 1 ? null : t.MaterialPaties
                            );
                            }
                            if (data.OtherMedicines != null && data.OtherMedicines.Count > 0)
                            {
                                data.OtherMedicines.ForEach(
                                t => t.MedicinePaties = t.Medicine != null && t.Medicine.IS_SALE_EQUAL_IMP_PRICE == 1 ? null : t.MedicinePaties
                            );
                            }

                            HisImpMestOtherSDO rs = null;
                            Inventec.Common.Logging.LogSystem.Error("BD goi api---------------------");
                            rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestOtherSDO>("api/HisImpMest/OtherUpdate", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                            Inventec.Common.Logging.LogSystem.Error("KT goi api");
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                            }
                        }
                        else
                        {
                            return;
                        }
                        if (success)
                        {
                            this.ProcessSaveSuccess();
                            btnPrint.Enabled = true;
                            //btnSave.Enabled = false;
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
            OnClickBienBanKiemNhapTuNcc(null, null);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {

                WaitingManager.Show();
                ResetValueControlDetail();
                ResetValueControlCommon();
                SetEnableControlCommon();
                SetDefaultImpMestType();
                SetDefaultValueMediStock();
                SetFocuTreeMediMate();
                cboImpMestType.Focus();
                WaitingManager.Hide();
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

                    if (currrentServiceAdo.HisMedicine != null)
                    {
                        chkImprice.Checked = currrentServiceAdo.HisMedicine.IS_SALE_EQUAL_IMP_PRICE == 1 ? true : false;
                        txtSttThau.Text = currrentServiceAdo.HisMedicine.TDL_BID_NUM_ORDER;
                        txtGoiThau.Text = currrentServiceAdo.HisMedicine.TDL_BID_PACKAGE_CODE;
                        txtNhomThau.Text = currrentServiceAdo.HisMedicine.TDL_BID_GROUP_CODE;
                        if (!checkOutBid.Checked)
                        {
                            medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, currrentServiceAdo.BidId);
                            txtSttThau.Text = currrentServiceAdo.BID_NUM_ORDER;
                            txtGoiThau.Text = currrentServiceAdo.BID_PACKAGE_CODE;
                            txtNhomThau.Text = currrentServiceAdo.BID_GROUP_CODE;
                        }
                    }
                    else if (currrentServiceAdo.HisMaterial != null)
                    {
                        chkImprice.Checked = currrentServiceAdo.HisMaterial.IS_SALE_EQUAL_IMP_PRICE == 1 ? true : false;
                        txtSttThau.Text = currrentServiceAdo.HisMaterial.TDL_BID_NUM_ORDER;
                        txtGoiThau.Text = currrentServiceAdo.HisMaterial.TDL_BID_PACKAGE_CODE;
                        txtNhomThau.Text = currrentServiceAdo.HisMaterial.TDL_BID_GROUP_CODE;
                        if (!checkOutBid.Checked)
                        {
                            materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, currrentServiceAdo.BidId);
                            txtSttThau.Text = currrentServiceAdo.BID_NUM_ORDER;
                            txtGoiThau.Text = currrentServiceAdo.BID_PACKAGE_CODE;
                            txtNhomThau.Text = currrentServiceAdo.BID_GROUP_CODE;
                        }
                    }
                    if (chkImprice.Checked == true)
                    {
                        if (currrentServiceAdo.HisMedicine != null)
                        {
                            var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.ID == currrentServiceAdo.HisMedicine.MEDICINE_TYPE_ID).ToList();
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
                        else if (currrentServiceAdo.HisMaterial != null)
                        {
                            var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.ID == currrentServiceAdo.HisMaterial.MATERIAL_TYPE_ID).ToList();
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

                    if (this.currentManuImpMest != null)
                    {
                        var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.currentManuImpMest.MEDI_STOCK_ID);

                        AutoMapper.Mapper.CreateMap<VHisServicePatyADO, VHisServicePatyADO>();
                        listServicePatyAdo = AutoMapper.Mapper.Map<List<VHisServicePatyADO>>(currrentServiceAdo.VHisServicePatys);
                        bool sellByImpPrice = false;
                        if (chkImprice.Checked == true)
                        {
                            listServicePatyAdo = null;
                        }
                        if (listServicePatyAdo != null)
                        {
                            foreach (var item in listServicePatyAdo)
                            {
                                if (!item.IsNotSell)
                                {
                                    if (item.PRICE == currrentServiceAdo.IMP_PRICE && item.VAT_RATIO == currrentServiceAdo.IMP_VAT_RATIO)
                                    {
                                        sellByImpPrice = true;
                                    }
                                    else
                                    {
                                        sellByImpPrice = false;
                                        break;
                                    }
                                }
                            }
                        }


                        gridControlServicePaty.BeginUpdate();
                        gridControlServicePaty.DataSource = listServicePatyAdo;
                        gridControlServicePaty.EndUpdate();
                        spinAmount.Focus();
                        spinAmount.SelectAll();
                    }
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
                ResetValueControlDetail();
                SetEnableButton(false);
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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ManuImpMestUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.ManuImpMestUpdate.frmManuImpMestUpdate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkOutBid.Properties.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.checkOutBid.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemAdd.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.barButtonItemAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemCancel.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.barButtonItemCancel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemNew.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.barButtonItemNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemPrint.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.barButtonItemPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemUpdate.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.barButtonItemUpdate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSupplier.Properties.NullText = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.cboSupplier.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUpdate.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.btnUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.cboImpSource.Properties.NullText = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.cboImpSource.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.cboMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboImpMestType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.cboImpMestType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServicePaty_PatientTypeName.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ServicePaty_PatientTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServicePaty_ExpPrice.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ServicePaty_ExpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServicePaty_VatRatio.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ServicePaty_VatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServicePaty_PriceAndVat.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ServicePaty_PriceAndVat.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServicePaty_NotSell.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ServicePaty_NotSell.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.xtraTabPageMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.xtraTabPageMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_Stt.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ImpMestDetail_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_Delete.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ImpMestDetail_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_Edit.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ImpMestDetail_Edit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_TypeName.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ImpMestDetail_TypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_Amount.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ImpMestDetail_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_Price.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ImpMestDetail_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_ImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ImpMestDetail_ImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ImpMestDetail_ExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.gridColumn_ImpMestDetail_PackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPriceVat.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciPriceVat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPackageNumber.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciPackageNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescriptionPaty.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciDescriptionPaty.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpMestType.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciImpMestType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMediStock.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciMediStock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpSource.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciImpSource.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpPrice.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciImpPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciVat.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciVat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTotalVatPrice.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciTotalVatPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTotalPrice.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciTotalPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTotalFeePrice.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciTotalFeePrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpiredDate.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciExpiredDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSupplier.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciSupplier.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDeliver.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciDeliver.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDiscountRatio.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciDiscountRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDocumentPrice.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciDocumentPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDiscountPrice.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciDiscountPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDocumentNumber.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciDocumentNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDocumentDate.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciDocumentDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.lciDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmManuImpMestUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckSaveUpdateMedicineMaterial()
        {
            bool result = false;
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    result = false;
                for (int i = 0; i < gridViewImpMestDetail.RowCount; i++)
                {
                    int rowHandle = gridViewImpMestDetail.GetVisibleRowHandle(i);
                    if (gridViewImpMestDetail.IsDataRow(rowHandle))
                    {
                        if (Convert.ToDouble(gridViewImpMestDetail.GetRowCellValue(rowHandle, "IMP_AMOUNT").ToString()) <= 0)
                        {
                            result = false;
                            break;
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void checkOutBid_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkOutBid.Checked)
                {
                    this.currentBid = null;
                    medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, null);
                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, false);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, false);
                    //LoadDataToComboSupplier(listSupplier);
                    //SetDataSourceGridControlMediMate();

                }
                else
                {
                    //medicineProcessor.SetEditValueBid(this.ucMedicineTypeTree, null);
                    //materialProcessor.SetEditValueBid(this.ucMaterialTypeTree, null);
                    medicineProcessor.EnableBid(this.ucMedicineTypeTree, true);
                    materialProcessor.EnableBid(this.ucMaterialTypeTree, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtDocumentNumber_Leave(object sender, EventArgs e)
        {
            try
            {
                CheckDocumentNumberV2(txtDocumentNumber.Text.Trim());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckDocumentNumberV2(string _document)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(_document))
                {
                    MOS.Filter.HisImpMestFilter manuImpMestViewFilter = new HisImpMestFilter();
                    manuImpMestViewFilter.DOCUMENT_NUMBER__EXACT = _document;
                    var manuImpMests = new BackendAdapter(new CommonParam()).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, manuImpMestViewFilter, new CommonParam());

                    // khong check phieu hien tai
                    manuImpMests = (manuImpMests != null && manuImpMests.Count() > 0) ? manuImpMests.Where(o => o.ID != impMestId).ToList() : manuImpMests;

                    if (manuImpMests != null && manuImpMests.Count > 0)
                    {
                        WaitingManager.Hide();

                        long _SupplierId = Inventec.Common.TypeConvert.Parse.ToInt64((cboSupplier.EditValue ?? "").ToString());
                        //#21142
                        var dataChecks = manuImpMests.Where(p => p.SUPPLIER_ID == _SupplierId
                            && p.DOCUMENT_NUMBER.Equals(_document.Trim())).ToList();

                        Inventec.Common.Logging.LogSystem.Info("dataChecks: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataChecks), dataChecks));

                        if (_SupplierId > 0 && dataChecks != null && dataChecks.Count > 0)
                        {
                            string mess = string.Format("Đã tồn tại mã phiếu nhập '{0}' có số chứng từ '{1}', Không thể nhập nhà cung cấp với số chứng từ này", string.Join(",", dataChecks.Select(p => p.IMP_MEST_CODE).ToList()), _document);
                            DevExpress.XtraEditors.XtraMessageBox.Show(mess, ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao);
                            result = true;
                        }
                        else
                        {
                            string mess = string.Format("Đã tồn tại mã phiếu nhập '{0}' có số chứng từ '{1}',", string.Join(",", manuImpMests.Select(p => p.IMP_MEST_CODE).ToList()), _document);
                            DevExpress.XtraEditors.XtraMessageBox.Show(mess, ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao);
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void txtDocumentNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void bbtnFocusSearchPanel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SetFocuTreeMediMate();
        }
    }
}
