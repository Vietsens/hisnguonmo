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
using HIS.UC.BloodType;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.ImportBlood.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.ImportBlood.Validation;
using MOS.SDO;
using HIS.Desktop.Common;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Utility;
using System.IO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Popup;
using DevExpress.Utils.Win;
using HIS.Desktop.Plugins.ImportBlood.Properties;
using ACS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using DevExpress.Utils.Menu;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Plugins.ImpMestViewDetail.ADO;

namespace HIS.Desktop.Plugins.ImportBlood
{
    public partial class UCImportBloodPlus : HIS.Desktop.Utility.UserControlBase
    {
        long impMestId;
        long IMP_MEST_TYPE_ID;
        long ImpMestSttId;
        BloodTypeProcessor bloodTypeProcessor;
        UserControl ucBloodType;
        V_HIS_IMP_MEST impMest;
        Inventec.Desktop.Common.Modules.Module moduleData;

        List<V_HIS_BID_BLOOD_TYPE> listBidBlood = new List<V_HIS_BID_BLOOD_TYPE>();
        List<V_HIS_IMP_MEST_MATERIAL> vimpMestMaterials;
        List<V_HIS_IMP_MEST_MEDICINE> vimpMestMedicines;
        List<ImpMestBloodSDODetail> impMestBloods;
        List<ImpMestMaterialSDODetail> impMestMaterials;
        List<ImpMestMedicineSDODetail> impMestMedicines;
        List<V_HIS_EXP_MEST_BLOOD> listExpMestBlood { get; set; }
        List<ExpMestBloodADO> listBloodADO { get; set; }

        List<V_HIS_MEDI_STOCK> listMediStock;
        List<HIS_IMP_MEST_TYPE> listImpMestType;
        HIS_IMP_MEST_TYPE currentImpMestType = null;

        VHisBloodADO currentBlood = null;
        VHisBloodADO newCurrentBlood = null;
        UC.BloodType.ADO.BloodTypeADO bloodTypeADO;
        List<long> ListSupplierId = new List<long>();
        List<HIS_SUPPLIER> listSupplier = new List<HIS_SUPPLIER>();

        Dictionary<string, VHisBloodADO> dicBloodAdo = new Dictionary<string, VHisBloodADO>();

        ResultImpMestADO resultADO = null;

        Inventec.Desktop.Common.Modules.Module currentModule = null;
        DelegateSelectData delegateSelectData = null;

        int actionType = 0; // 0 - xem, 1 - them, 2 - sua

        int positionHandleControl = -1;

        public UCImportBloodPlus()
            : base(null)
        {
            InitializeComponent();
            try
            {
                throw new ArgumentNullException("Module is NULL");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCImportBloodPlus(Inventec.Desktop.Common.Modules.Module module, long impMestId, DelegateSelectData _delegateSelectData)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.InitGridBloodType();
                this.currentModule = module;
                this.impMestId = impMestId;
                delegateSelectData = _delegateSelectData;
                this.actionType = 2;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCImportBloodPlus(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = module;
                this.InitGridBloodType();
                this.actionType = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitGridBloodType()
        {
            try
            {
                var lang = Base.ResourceLangManager.LanguageUCImportBlood;
                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.bloodTypeProcessor = new BloodTypeProcessor();
                BloodTypeInitADO ado = new BloodTypeInitADO();
                ado.BloodTypeClick = gridBloodType_Click;
                ado.BloodTypeRowEnter = gridBloodType_RowEnter;
                ado.BloodTypeRowClick = gridBloodType_RowClick;
                ado.IsAutoWidth = false;
                ado.IsShowSearchPanel = true;
                ado.Keyword_NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__TXT_KEYWORD__NULL_VALUE", lang, cul);
                ado.BloodTypeColumns = new List<BloodTypeColumn>();

                BloodTypeColumn colBloodTypeCode = new BloodTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD_TYPE__COLUMN_BLOOD_TYPE_CODE", lang, cul), "BLOOD_TYPE_CODE", 100, false);
                colBloodTypeCode.VisibleIndex = 0;
                ado.BloodTypeColumns.Add(colBloodTypeCode);

                BloodTypeColumn colBloodTypeName = new BloodTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD_TYPE__COLUMN_BLOOD_TYPE_NAME", lang, cul), "BLOOD_TYPE_NAME", 300, false);
                colBloodTypeName.VisibleIndex = 1;
                ado.BloodTypeColumns.Add(colBloodTypeName);

                BloodTypeColumn colVolume = new BloodTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD_TYPE__COLUMN_VOLUME", lang, cul), "VOLUME", 100, false);
                colVolume.VisibleIndex = 2;
                ado.BloodTypeColumns.Add(colVolume);

                BloodTypeColumn colElement = new BloodTypeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD_TYPE__COLUMN_ELEMENT", lang, cul), "ELEMENT", 100, false);
                colElement.VisibleIndex = 3;
                ado.BloodTypeColumns.Add(colElement);

                this.ucBloodType = (UserControl)this.bloodTypeProcessor.Run(ado);
                if (this.ucBloodType != null)
                {
                    this.panelControlBloodType.Controls.Add(this.ucBloodType);
                    this.ucBloodType.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool CheckMediStockIsBlood()
        {
            bool result = false;
            try
            {
                if (this.currentModule != null)
                {
                    var checkMedistock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
                    if (checkMedistock != null && checkMedistock.IS_BLOOD == 1)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void EnableAllControl(bool input)
        {
            try
            {
                layoutControlGroupAllControl.Enabled = input;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCImportBloodPlus_Load(object sender, EventArgs e)
        {
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(MeShow));
                //thread.Priority = System.Threading.ThreadPriority.Highest;
                thread.IsBackground = true;
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                try
                {
                    WaitingManager.Show();
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    thread.Abort();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MeShow()
        {
            try
            {
                Invoke(new Action(() =>
                {
                    cboPrint.Enabled = false;
                    if (!CheckMediStockIsBlood())
                    {
                        MessageManager.Show(Base.ResourceMessageLang.KhoKhongPhaiLaKhoMau);
                        EnableAllControl(false);
                        return;
                    }
                    else
                    {
                        EnableAllControl(true);
                    }
                    WaitingManager.Show();
                    this.LoadKeyUCLanguage();
                    listSupplier = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1).ToList();
                    this.ValidControl();
                    this.LoadDataToComboBloodAbo();
                    this.LoadDataToComboBloodRh();
                    this.LoadDataToCboImpSource();
                    this.LoadDataToComboImpMestType();
                    this.LoadDataToComboMediStock();
                    this.LoadDataToComboSupplier();
                    this.FillDataToGridBloodType();
                    this.SetDefaultImpMestType();
                    this.SetDataSourceMediStock();
                    this.SetDefaultValueMediStock();
                    this.SetControlEnableImMestTypeManu();
                    this.SetEnableButtonAdd(true);
                    this.SetControlValueByBloodType(true);
                    ////Dangth
                    //cboPrint.Enabled = false;
                    cboMediStock.Enabled = false;
                    txtMediStock.Enabled = false;
                    this.bloodTypeProcessor.FocusKeyword(this.ucBloodType);
                    AllowImpMest();
                    if (this.actionType == 2)
                    {
                        var impMest = LoadImpMestByID(this.impMestId);
                        FillDataToCommonControl(impMest);
                        if (this._isHienMau)
                        {
                            SetDicBloodGiverData_FromCurrentImpMest();

                            SetDataSourceGridBlood_BloodGiver();
                            EnableControlEdit(this.actionType);
                            FillDataToResult_ForHienMau(impMest);
                        }
                        else
                        {
                            var bloods = FillDataToGridImpMestBlood();
                            FillDataToDicBlood(bloods);
                            SetDataSourceGridBlood();
                            EnableControlEdit(this.actionType);
                            FillDataToResult(impMest, bloods);
                        }

                        this.InitMenuToButtonPrint(impMest);
                    }
                    WaitingManager.Hide();
                }));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableControlEdit(int actionType)
        {
            try
            {
                if (actionType == 2)
                {
                    layoutImpMestType.Enabled = false;
                    layoutMediStock.Enabled = false;
                    lciSupplier.Enabled = false;
                    lciBtnNew.Enabled = false;
                    lciBtnSaveDraft.Enabled = false;
                }
                else
                {
                    layoutImpMestType.Enabled = true;
                    layoutMediStock.Enabled = true;
                    lciSupplier.Enabled = true;
                    lciBtnNew.Enabled = true;
                    lciBtnSaveDraft.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToCommonControl(HIS_IMP_MEST impMest)
        {
            try
            {
                if (impMest != null)
                {
                    var impMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().FirstOrDefault(o => o.ID == impMest.IMP_MEST_TYPE_ID);
                    if (impMestType != null)
                    {
                        txtImpMestType.Text = impMestType.IMP_MEST_TYPE_NAME;
                        cboImpMestType.EditValue = impMestType.ID;
                    }
                    var medistock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == impMest.MEDI_STOCK_ID);
                    if (medistock != null)
                    {
                        txtMediStock.Text = medistock.MEDI_STOCK_NAME;
                        cboMediStock.EditValue = medistock.ID;
                    }

                    txtImpSource.Text = "";
                    cboImpSource.EditValue = null;// impSource được gán theo blood nên mặc định null
                    cboSupplier.EditValue = impMest.SUPPLIER_ID;
                    if (impMest.DOCUMENT_DATE.HasValue)
                    {
                        txtDocumentDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(impMest.DOCUMENT_DATE.Value);
                        dtDocumentDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(impMest.DOCUMENT_DATE.Value);
                    }

                    txtDocumentNumber.Text = impMest.DOCUMENT_NUMBER;

                    if (impMest.DISCOUNT_RATIO.HasValue)
                        spinDiscountRatio.Value = impMest.DISCOUNT_RATIO.Value * 100;
                    else
                        spinDiscountRatio.EditValue = null;

                    if (impMest.DISCOUNT.HasValue)
                        spinDiscountPrice.Value = impMest.DISCOUNT.Value;
                    else
                        spinDiscountPrice.EditValue = null;

                    if (impMest.DOCUMENT_PRICE.HasValue)
                        spinDocumentPrice.Value = impMest.DOCUMENT_PRICE.Value;
                    else
                        spinDiscountPrice.EditValue = null;

                    txtDeliever.Text = impMest.DELIVERER;
                    txtDescription.Text = impMest.DESCRIPTION;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToResult(HIS_IMP_MEST impMest, List<HIS_BLOOD> listBlood)
        {
            try
            {
                if (impMest == null)
                    throw new ArgumentNullException("impMest is null");

                resultADO = new ResultImpMestADO();// in an
                if (impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    HisImpMestManuSDO manuSDO = new HisImpMestManuSDO();
                    manuSDO.ImpMest = new HIS_IMP_MEST();
                    manuSDO.ManuBloods = new List<HIS_BLOOD>();
                    manuSDO.ImpMest = impMest;
                    manuSDO.ManuBloods = listBlood;
                    resultADO = new ResultImpMestADO(manuSDO);
                }
                else if (impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                {
                    HisImpMestOtherSDO otherSDO = new HisImpMestOtherSDO();
                    otherSDO.ImpMest = new HIS_IMP_MEST();
                    otherSDO.OtherBloods = new List<HIS_BLOOD>();
                    otherSDO.ImpMest = impMest;
                    otherSDO.OtherBloods = listBlood;
                    resultADO = new ResultImpMestADO(otherSDO);

                }
                else if (impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    HisImpMestInitSDO initSDO = new HisImpMestInitSDO();
                    initSDO.ImpMest = new HIS_IMP_MEST();
                    initSDO.InitBloods = new List<HIS_BLOOD>();
                    initSDO.ImpMest = impMest;
                    initSDO.InitBloods = listBlood;
                    resultADO = new ResultImpMestADO(initSDO);
                }
                else if (impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                {
                    HisImpMestInveSDO inveSDO = new HisImpMestInveSDO();
                    inveSDO.ImpMest = new HIS_IMP_MEST();
                    inveSDO.InveBloods = new List<HIS_BLOOD>();
                    inveSDO.ImpMest = impMest;
                    inveSDO.InveBloods = listBlood;
                    resultADO = new ResultImpMestADO(inveSDO);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("loai nhap khong cho phep sua");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToResult_ForHienMau(HIS_IMP_MEST impMest)
        {
            try
            {
                if (impMest == null)
                    throw new ArgumentNullException("impMest is null");

                resultADO = new ResultImpMestADO();// in an
                if (impMest.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HM)
                {
                    HisImpMestDonationSDO donationSDO = new HisImpMestDonationSDO();
                    donationSDO.ImpMest = new HIS_IMP_MEST();
                    donationSDO.ImpMest = impMest;
                    donationSDO.DonationDetail = new List<DonationDetailSDO>();
                    resultADO = new ResultImpMestADO(donationSDO);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("loai nhap khong cho phep sua");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToDicBlood(List<HIS_BLOOD> bloods)
        {
            try
            {
                if (bloods == null || bloods.Count() == 0)
                    return;
                dicBloodAdo = new Dictionary<string, VHisBloodADO>();

                foreach (var item in bloods)
                {
                    VHisBloodADO bloodADO = new VHisBloodADO();
                    var bloodType = BackendDataWorker.Get<HIS_BLOOD_TYPE>().FirstOrDefault(o => o.ID == item.BLOOD_TYPE_ID);

                    if (bloodType != null)
                    {
                        bloodADO.BLOOD_TYPE_ID = bloodType.ID;
                        bloodADO.BLOOD_TYPE_CODE = bloodType.BLOOD_TYPE_CODE;
                        bloodADO.BLOOD_TYPE_NAME = bloodType.BLOOD_TYPE_NAME;
                        if (bloodADO.BLOOD_VOLUME_ID > 0)
                        {
                            bloodADO.VOLUME = BackendDataWorker.Get<HIS_BLOOD_VOLUME>().FirstOrDefault(o => o.ID == bloodType.BLOOD_VOLUME_ID).VOLUME;
                        }
                    }

                    var bloodAbo = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == item.BLOOD_ABO_ID);
                    if (bloodAbo != null)
                    {
                        bloodADO.BLOOD_ABO_ID = bloodAbo.ID;
                        bloodADO.BLOOD_ABO_CODE = bloodAbo.BLOOD_ABO_CODE;
                    }

                    if (item.BLOOD_RH_ID != null)
                    {
                        var bloodRh = BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == item.BLOOD_RH_ID);
                        if (bloodRh != null)
                        {
                            bloodADO.BLOOD_RH_ID = bloodRh.ID;
                            bloodADO.BLOOD_RH_CODE = bloodRh.BLOOD_RH_CODE;
                        }
                    }

                    bloodADO.IMP_PRICE = item.IMP_PRICE;
                    bloodADO.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                    bloodADO.ImpVatRatio = item.IMP_VAT_RATIO * 100;
                    bloodADO.PACKING_TIME = item.PACKING_TIME;
                    if (item.IMP_SOURCE_ID != null)
                    {
                        var impSource = BackendDataWorker.Get<HIS_IMP_SOURCE>().FirstOrDefault(o => o.ID == item.IMP_SOURCE_ID);
                        if (impSource != null)
                        {
                            bloodADO.IMP_SOURCE_ID = impSource.ID;
                            bloodADO.IMP_SOURCE_CODE = impSource.IMP_SOURCE_CODE;
                            bloodADO.IMP_SOURCE_NAME = impSource.IMP_SOURCE_NAME;
                        }
                    }
                    bloodADO.GIVE_CODE = item.GIVE_CODE;
                    bloodADO.GIVE_NAME = item.GIVE_NAME;
                    bloodADO.PACKAGE_NUMBER = item.PACKAGE_NUMBER;
                    bloodADO.IS_INFECT = item.IS_INFECT;
                    bloodADO.EXPIRED_DATE = item.EXPIRED_DATE;
                    bloodADO.BLOOD_CODE = item.BLOOD_CODE;
                    bloodADO.ID = item.ID;
                    dicBloodAdo[bloodADO.BLOOD_CODE + "_" + bloodADO.BLOOD_TYPE_CODE] = bloodADO;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_IMP_MEST LoadImpMestByID(long impMestId)
        {
            HIS_IMP_MEST result = new HIS_IMP_MEST();
            try
            {
                HisImpMestFilter impMestFilter = new HisImpMestFilter();
                impMestFilter.ID = this.impMestId;
                impMestFilter.IS_ACTIVE = 1;
                var listImpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, impMestFilter, null);
                if (listImpMest == null || listImpMest.Count != 1)
                {
                    throw new Exception("Khong lay duoc impMest theo id: " + impMestId);
                }
                result = listImpMest.FirstOrDefault();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //Load dữ liệu vào grid máu
        private List<HIS_BLOOD> FillDataToGridImpMestBlood()
        {
            List<HIS_BLOOD> result = new List<HIS_BLOOD>();
            try
            {
                CommonParam param = new CommonParam();
                if (impMestId > 0)
                {
                    MOS.Filter.HisImpMestBloodFilter filter = new HisImpMestBloodFilter();
                    filter.IMP_MEST_ID = impMestId;
                    filter.IS_ACTIVE = 1;
                    var impMestBloods = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                    if (impMestBloods == null || impMestBloods.Count() == 0)
                        return null;

                    List<long> bloodIds = impMestBloods.Select(o => o.BLOOD_ID).ToList();

                    MOS.Filter.HisBloodFilter bloodFilter = new HisBloodFilter();
                    bloodFilter.IDs = bloodIds;
                    bloodFilter.IS_ACTIVE = 1;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_BLOOD>>("api/HisBlood/Get", ApiConsumer.ApiConsumers.MosConsumer, bloodFilter, param);


                }
            }
            catch (Exception ex)
            {
                return null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadDataToComboBloodAbo()
        {
            try
            {
                cboBloodAbo.Properties.DataSource = BackendDataWorker.Get<HIS_BLOOD_ABO>().Where(o => o.IS_ACTIVE == 1).ToList();
                cboBloodAbo.Properties.DisplayMember = "BLOOD_ABO_CODE";
                cboBloodAbo.Properties.ValueMember = "ID";
                cboBloodAbo.Properties.ForceInitialize();
                cboBloodAbo.Properties.Columns.Clear();
                cboBloodAbo.Properties.Columns.Add(new LookUpColumnInfo("BLOOD_ABO_CODE", "", 50));
                cboBloodAbo.Properties.ShowHeader = false;
                cboBloodAbo.Properties.ImmediatePopup = true;
                cboBloodAbo.Properties.DropDownRows = 10;
                cboBloodAbo.Properties.PopupWidth = 50;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboBloodRh()
        {
            try
            {
                cboBloodRh.Properties.DataSource = BackendDataWorker.Get<HIS_BLOOD_RH>().Where(o => o.IS_ACTIVE == 1).ToList();
                cboBloodRh.Properties.DisplayMember = "BLOOD_RH_CODE";
                cboBloodRh.Properties.ValueMember = "ID";
                cboBloodRh.Properties.ForceInitialize();
                cboBloodRh.Properties.Columns.Clear();
                cboBloodRh.Properties.Columns.Add(new LookUpColumnInfo("BLOOD_RH_CODE", "", 50));
                cboBloodRh.Properties.ShowHeader = false;
                cboBloodRh.Properties.ImmediatePopup = true;
                cboBloodRh.Properties.DropDownRows = 10;
                cboBloodRh.Properties.PopupWidth = 50;

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
                cboImpSource.Properties.DataSource = BackendDataWorker.Get<HIS_IMP_SOURCE>().Where(o => o.IS_ACTIVE == 1).ToList();
                cboImpSource.Properties.DisplayMember = "IMP_SOURCE_NAME";
                cboImpSource.Properties.ValueMember = "ID";
                cboImpSource.Properties.ForceInitialize();
                cboImpSource.Properties.Columns.Clear();
                cboImpSource.Properties.Columns.Add(new LookUpColumnInfo("IMP_SOURCE_CODE", "", 40));
                cboImpSource.Properties.Columns.Add(new LookUpColumnInfo("IMP_SOURCE_NAME", "", 80));
                cboImpSource.Properties.ShowHeader = false;
                cboImpSource.Properties.ImmediatePopup = true;
                cboImpSource.Properties.DropDownRows = 10;
                cboImpSource.Properties.PopupWidth = 120;

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
                var listImpMestTypeId = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HM
                 };
                listImpMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o => listImpMestTypeId.Contains(o.ID) && o.IS_ACTIVE == 1).ToList();
                cboImpMestType.Properties.DataSource = listImpMestType;
                cboImpMestType.Properties.DisplayMember = "IMP_MEST_TYPE_NAME";
                cboImpMestType.Properties.ValueMember = "ID";
                cboImpMestType.Properties.ForceInitialize();
                cboImpMestType.Properties.Columns.Clear();
                cboImpMestType.Properties.Columns.Add(new LookUpColumnInfo("IMP_MEST_TYPE_CODE", "", 50));
                cboImpMestType.Properties.Columns.Add(new LookUpColumnInfo("IMP_MEST_TYPE_NAME", "", 100));
                cboImpMestType.Properties.ShowHeader = false;
                cboImpMestType.Properties.ImmediatePopup = true;
                cboImpMestType.Properties.DropDownRows = 10;
                cboImpMestType.Properties.PopupWidth = 150;

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
                cboMediStock.Properties.DataSource = null;
                cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStock.Properties.ValueMember = "ID";
                cboMediStock.Properties.ForceInitialize();
                cboMediStock.Properties.Columns.Clear();
                cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 50));
                cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 100));
                cboMediStock.Properties.ShowHeader = false;
                cboMediStock.Properties.ImmediatePopup = true;
                cboMediStock.Properties.DropDownRows = 10;
                cboMediStock.Properties.PopupWidth = 150;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboSupplier()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SUPPLIER_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SUPPLIER_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SUPPLIER_NAME", "ID", columnInfos, false, 350);
                var dataSource = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1 && o.IS_BLOOD == 1).ToList();
                ControlEditorLoader.Load(cboSupplier, dataSource, controlEditorADO);
                if (dataSource != null && dataSource.Count() == 1)
                {
                    cboSupplier.EditValue = dataSource.FirstOrDefault().ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridBloodTypeByBid(long bidId)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<V_HIS_BLOOD_TYPE> bloodTypes = null;
                MOS.Filter.HisBidBloodTypeFilter bidBloodTypeFilter = new HisBidBloodTypeFilter();
                bidBloodTypeFilter.BID_ID = bidId;
                var bidBloodTypes = new BackendAdapter(param).Get<List<HIS_BID_BLOOD_TYPE>>("api/HisBidBloodType/Get", ApiConsumer.ApiConsumers.MosConsumer, bidBloodTypeFilter, param);
                if (bidBloodTypes != null && bidBloodTypes.Count > 0)
                {
                    List<long> BloodTypeIds = bidBloodTypes.Select(o => o.BLOOD_TYPE_ID).Distinct().ToList();
                    bloodTypes = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().Where(o => BloodTypeIds.Contains(o.ID) && o.IS_LEAF == 1).ToList();

                }
                this.bloodTypeProcessor.Reload(this.ucBloodType, bloodTypes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridBloodType()
        {
            try
            {
                this.bloodTypeProcessor.Reload(this.ucBloodType, BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().Where(o => o.IS_LEAF == 1 && o.IS_ACTIVE == 1).ToList());
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
                this.currentImpMestType = listImpMestType.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && o.IS_ACTIVE == 1);
                if (this.currentImpMestType != null)
                {
                    cboImpMestType.EditValue = this.currentImpMestType.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSourceMediStock()
        {
            try
            {
                listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                cboMediStock.Properties.DataSource = listMediStock.Where(o => o.IS_BLOOD == 1 && o.IS_ACTIVE == 1).ToList();
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
                if (!cboMediStock.Enabled || cboMediStock.Properties.DataSource == null || this.currentModule == null)
                {
                    cboMediStock.EditValue = null;
                }
                var medistock = listMediStock.FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
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

        private void SetControlEnableImMestTypeManu()
        {
            try
            {
                if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HM)
                {
                    this._isHienMau = true;
                    if (this.isFirstLoad_BloodGiverForm)
                    {
                        ThreadLoadDataBloodGiverForm();
                        this.isFirstLoad_BloodGiverForm = false;
                    }
                    lcgBloodGiver.Expanded = true;
                    lcgBloodGiver.ExpandOnDoubleClick = true;
                    lcgBloodGiver.ExpandButtonVisible = true;
                    layoutGiveCode.Enabled = false;
                    layoutGiveName.Enabled = false;
                    SetDataSourceGridBlood_BloodGiver();
                    //cboPrint.Enabled = false;
                    btnExportExcel.Enabled = false;
                }
                else
                {
                    this._isHienMau = false;
                    lcgBloodGiver.Expanded = false;
                    lcgBloodGiver.ExpandOnDoubleClick = false;
                    lcgBloodGiver.ExpandButtonVisible = false;
                    layoutGiveCode.Enabled = true;
                    layoutGiveName.Enabled = true;
                    SetDataSourceGridBlood();
                    //cboPrint.Enabled = true;
                    btnExportExcel.Enabled = true;
                }

                if (this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    panelControlDocumentDate.Enabled = true;
                    //txtSupplier.Enabled = true;
                    //cboSupplier.Enabled = true;                    
                    //txtBid.Enabled = true;
                    //cboBid.Enabled = true;
                    txtDocumentNumber.Enabled = true;
                    //dtDocumentDate.Enabled = true;
                    //txtDocumentDate.Enabled = true;
                    txtDeliever.Enabled = true;
                    txtDescription.Enabled = true;
                    spinDiscountPrice.Enabled = true;
                    spinDiscountRatio.Enabled = true;
                    spinDocumentPrice.Enabled = true;
                    lciSupplier.Enabled = true;
                    txtDocumentNumber.Text = "";
                    dtDocumentDate.EditValue = null;
                }
                else
                {
                    panelControlDocumentDate.Enabled = false;
                    //txtSupplier.Enabled = false;
                    cboSupplier.EditValue = null;
                    //cboSupplier.Enabled = false;
                    //txtBid.Enabled = false;
                    //cboBid.Enabled = false;
                    txtDocumentNumber.Text = "";
                    txtDocumentNumber.Enabled = false;
                    dtDocumentDate.EditValue = null;
                    lciSupplier.Enabled = false;
                    //dtDocumentDate.Enabled = false;
                    //txtDocumentDate.Enabled = false;
                    txtDeliever.Text = "";
                    txtDeliever.Enabled = false;
                    txtDescription.Text = "";
                    txtDescription.Enabled = true;
                    spinDiscountPrice.Value = 0;
                    spinDiscountPrice.Enabled = false;
                    spinDocumentPrice.Value = 0;
                    spinDocumentPrice.Enabled = false;
                    spinDiscountRatio.Value = 0;
                    spinDiscountRatio.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void AllowImpMest()
        {
            try
            {
                V_HIS_MEDI_STOCK medistock = null;
                if (currentModule != null)
                {
                    medistock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId);
                }
                if (medistock != null && medistock.IS_ALLOW_IMP_SUPPLIER != 1 && this.currentImpMestType != null && this.currentImpMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    this.EnableAllControl(false);
                    MessageManager.Show(Base.ResourceMessageLang.KhoKhongChoPhepNhapTuNhaCungCap);
                }
                else
                {
                    btnSave.Enabled = true;
                    btnSaveDraft.Enabled = true;
                    //cboPrint.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButtonAdd(bool enable)
        {
            try
            {
                if (enable)
                {
                    layoutBtnAdd.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutBtnCancel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutBtnUpdate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    btnAdd.Enabled = true;
                    btnCancel.Enabled = false;
                    btnUpdate.Enabled = false;
                }
                else
                {
                    layoutBtnAdd.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutBtnCancel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutBtnUpdate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    btnAdd.Enabled = false;
                    btnCancel.Enabled = true;
                    btnUpdate.Enabled = true;
                }
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
                    messageError = Base.ResourceMessageLang.NguoiDungChuaChonLoaiNhap;
                    return false;
                }
                if (cboMediStock.EditValue == null)
                {
                    messageError = Base.ResourceMessageLang.NguoiDungChuaChonKhoNhap;
                    return false;
                }
                if (Convert.ToInt64(cboImpMestType.EditValue) != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                    return true;
                //if (cboSupplier.EditValue == null)
                //{
                //    messageError = Base.ResourceMessageLang.LoaiNhapLaNhapTuNhaCungCapNguoiDungPhaiChonNhaCungCap;
                //    return false;
                //}
                //if (currrentServiceAdo != null && currrentServiceAdo.IsRequireHsd)
                //{
                //    if (dtExpiredDate.EditValue == null || String.IsNullOrEmpty(txtExpiredDate.Text))
                //    {
                //        messageError = Base.ResourceMessageLang.ChuaNhapHanSuDung;
                //        return false;
                //    }
                //}
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void SetTextAfterAdd(VHisBloodADO data)
        {
            try
            {
                if (currentBlood != null)
                {
                    txtBloodAboCode.Text = currentBlood.BLOOD_ABO_CODE;
                    txtPackageNumber.Text = currentBlood.PACKAGE_NUMBER;
                    spinImpPrice.EditValue = currentBlood.IMP_PRICE;
                    spinImpVatRatio.EditValue = currentBlood.IMP_VAT_RATIO;
                    txtBloodCode.Text = "";
                    this.SetEnableButtonAdd(true);
                    txtBloodCode.Focus();
                    txtGiveCode.Text = this.currentBlood.GIVE_CODE;
                    txtGiveName.Text = this.currentBlood.GIVE_NAME;
                    checkIsInfect.Checked = (this.currentBlood.IS_INFECT == 1);
                    if (this.currentBlood.BLOOD_ABO_ID > 0)
                    {
                        cboBloodAbo.EditValue = this.currentBlood.BLOOD_ABO_ID;
                    }
                    if (this.currentBlood.BLOOD_RH_ID.HasValue && this.currentBlood.BLOOD_RH_ID.Value > 0)
                    {
                        cboBloodRh.EditValue = this.currentBlood.BLOOD_RH_ID.Value;
                    }
                    spinImpPrice.Value = this.currentBlood.IMP_PRICE;
                    spinImpVatRatio.Value = this.currentBlood.ImpVatRatio;
                    txtPackageNumber.Text = this.currentBlood.PACKAGE_NUMBER;
                    if (this.currentBlood.PACKING_TIME != null)
                    {
                        dtPackingTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentBlood.PACKING_TIME ?? 0) ?? DateTime.Now;
                    }
                    if (this.currentBlood.EXPIRED_DATE != null)
                    {
                        dtExpiredDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentBlood.EXPIRED_DATE ?? 0) ?? DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            //dtExpiredDate.EditValue = DateTimeHelper.(txtPackingTime.Text);
            //currentBlood.EXPIRED_DATE
        }

        private void SetDataSourceGridBlood()
        {
            try
            {
                gridColumn_ForGroupingBloodDonation.GroupIndex = -1;
                gridColumn_ImpMestBlood_Stt.Visible = true;
                gridColumn_ImpMestBlood_Stt.VisibleIndex = 0;
                gridColumn_ImpMestBlood_Stt.Width = 30;
                gridColumn_ImpMestBlood_Edit.Visible = true;
                gridColumn_ImpMestBlood_Edit.VisibleIndex = 2;
                gridColumn_ImpMestBlood_Edit.Width = 25;
                gridColumn_ImpMestBlood_Delete.VisibleIndex = 1;
                gridColumn_ImpMestBlood_Delete.Width = 25;

                gridControlImpMestBlood.BeginUpdate();
                if (dicBloodAdo != null && dicBloodAdo.Count > 0)
                {
                    var lst = dicBloodAdo.Select(s => s.Value).ToList();
                    lst.Reverse();
                    gridControlImpMestBlood.DataSource = lst;
                }
                else
                {
                    gridControlImpMestBlood.DataSource = null;
                }
                gridControlImpMestBlood.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridBlood()
        {
            try
            {
                if (true)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlBloodAbo();
                ValidControlBloodCode();
                ValidControlImpPrice();
                ValidControlBloodRh();
                ValidControlImpVatRatio();
                //ValidControlDocumentDate();
                ValidControlImpMestType();
                ValidControlMediStock();
                ValidControlSupplier();
                ValidControlPackingTime();
                ValidControlExpiredDate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidControlBloodRh()
        {
            try
            {
                BloodRhValidationRule bloodRh = new BloodRhValidationRule();
                bloodRh.cboBloodRh = cboBloodRh;
                dxValidationProvider1.SetValidationRule(cboBloodRh, bloodRh);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlBloodAbo()
        {
            try
            {
                BloodAboValidationRule bloodAboRule = new BloodAboValidationRule();
                bloodAboRule.txtBloodAboCode = txtBloodAboCode;
                bloodAboRule.cboBloodAbo = cboBloodAbo;
                dxValidationProvider1.SetValidationRule(txtBloodAboCode, bloodAboRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlBloodCode()
        {
            try
            {
                BloodCodeValidationRule bloodCodeRule = new BloodCodeValidationRule();
                bloodCodeRule.txtBloodCode = txtBloodCode;
                dxValidationProvider1.SetValidationRule(txtBloodCode, bloodCodeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlPackingTime()
        {
            try
            {
                PackingTimeValidationRule packingTimeRule = new PackingTimeValidationRule();
                packingTimeRule.txtPackingTime = txtPackingTime;
                packingTimeRule.dtPackingTime = dtPackingTime;
                dxValidationProvider1.SetValidationRule(txtPackingTime, packingTimeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImpPrice()
        {
            try
            {
                ImpPriceValidationRule impPriceRule = new ImpPriceValidationRule();
                impPriceRule.spinImpPrice = spinImpPrice;
                dxValidationProvider1.SetValidationRule(spinImpPrice, impPriceRule);
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
                ImpVatRatioValidationRule impVatRatioRule = new ImpVatRatioValidationRule();
                impVatRatioRule.spinImpVatRatio = spinImpVatRatio;
                dxValidationProvider1.SetValidationRule(spinImpVatRatio, impVatRatioRule);
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
                impMestTypeRule.txtImpMestType = txtImpMestType;
                impMestTypeRule.cboImpMestType = cboImpMestType;
                dxValidationProvider2.SetValidationRule(txtImpMestType, impMestTypeRule);
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
                expiredDateRule.dtExpiredDate = dtExpiredDate;
                dxValidationProvider1.SetValidationRule(dtExpiredDate, expiredDateRule);
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
                mediStockRule.txtMediStock = txtMediStock;
                mediStockRule.cboMediStock = cboMediStock;
                dxValidationProvider2.SetValidationRule(txtMediStock, mediStockRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlSupplier()
        {
            try
            {
                SupplierValidationRule supplierRule = new SupplierValidationRule();
                supplierRule.cboSupplier = cboSupplier;
                supplierRule.cboImpMestType = cboImpMestType;
                dxValidationProvider2.SetValidationRule(cboSupplier, supplierRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDocumentDate()
        {
            try
            {
                DocumentDateValidationRule docDateRule = new DocumentDateValidationRule();
                docDateRule.txtDocumentDate = txtDocumentDate;
                docDateRule.dtDocumentDate = dtDocumentDate;
                dxValidationProvider2.SetValidationRule(txtDocumentDate, docDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        private void dxValidationProvider2_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        private void LoadKeyUCLanguage()
        {
            try
            {
                //Button
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__BTN_ADD", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__BTN_NEW", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Dangth
                this.cboPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__BTN_PRINT", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__BTN_SAVE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSaveDraft.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__BTN_SAVE_DRAFT", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__BTN_CANCEL", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout
                //this.layoutBidInfo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_BID_INFO", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutBidNumOrder.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_BID_NUM_ORDER", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutBloodAbo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_BLOOD_ABO", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutBloodCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_BLOOD_CODE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutBloodRh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_BLOOD_RH", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDeliever.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_DELIVER", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_DESCRIPTION", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDiscountPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_DISCOUNT", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDiscountRatio.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_DISCOUNT_RATIO", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDocumentDate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_DOCUMENT_DATE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDocumentNumber.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_DOCUMENT_NUMBER", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutGiveCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_GIVE_CODE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutGiveName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_GIVE_NAME", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutImpMestType.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_IMP_MEST_TYPE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutImpPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_IMP_PRICE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutImpSource.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_IMP_SOURCE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutImpVatRatio.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_IMP_VAT_RATIO", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutMediStock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_MEDI_STOCK", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutPackageNumber.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_PACKAGE_NUMBER", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutPackingTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_PACKING_TIME", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciSupplier.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_SUPPLIER", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //CheckEdit
                this.checkIsInfect.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__LAYOUT_IS_INFECT", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //GridControl Blood
                this.gridColumn_ImpMestBlood_BidNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_BID_NUMBER", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_BidNumOrder.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_BID_NUM_ORDER", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_BloodAbo.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_BLOOD_ABO_CODE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_BloodCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_BLOOD_CODE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_BloodRh.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_BLOOD_RH_CODE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_BloodTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_BLOOD_TYPE_NAME", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_GiveName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_GIVE_NAME", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_IMP_PRICE", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_ImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_IMP_VAT_RATIO", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_PACKAGE_NUMBER", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_PackingTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_PACKING_TIME", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_STT", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ImpMestBlood_Volume.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__GRID_BLOOD__COLUMN_VOLUME", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Repository Button
                this.repositoryItemBtnDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__REPOSITORY__BTN_DELETE_BLOOD", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repositoryItemBtnEdit.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_IMPORT_BLOOD__REPOSITORY__BTN_EDIT_BLOOD", Base.ResourceLangManager.LanguageUCImportBlood, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //Dangth
        internal enum PrintType
        {
            BIEN_BAN_KIEM_NHAP_TU_NCC,
            PHIEU_NHAP_THUOC_VAT_TU_TU_NCC,
            PHIEU_NHAP_MAU_TU_NCC,
            BAR_CODE,
            PHIEU_NHAP_THU_HOI,
            PHIEU_NHAP_CHUYEN_KHO,
            PHIEU_NHAP_CHUYEN_KHO_THUOC_GAY_NGHIEN_HUONG_THAN,
            PHIEU_NHAP_CHUYEN_KHO_KHONG_PHAI_THUOC_GAY_NGHIEN_HUONG_THAN,
            PHIEU_NHAP_KIEM_KE_DAU_KY_KHAC,
            Mps000214_Nhap_Hao_Phi_Tra_Lai,
            Mps000213_Don_Mau_Tra_Lai,
            Mps000084_Don_Noi_Tru_Tra_Lai,
            Mps000230_Bu_Thuoc_Le,
            Mps000221_Bu_Co_So_Tu_Truc,
            Mps000244_Phieu_San_Xuat_thuoc,
            Mps000092_Phieu_Xuat_Ban,
            IN_TEM_THEO_SO_SERI
        }

        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

        public void InitMenuToButtonPrint(HIS_IMP_MEST hIS_IMP_MEST)
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                // nhap tu nha cung cap
                if (hIS_IMP_MEST.IMP_MEST_TYPE_ID != null && hIS_IMP_MEST.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    //btnHoiDongKiemNhap.Enabled = true;
                    DXMenuItem itemBienBankiemNhapNCC = new DXMenuItem("Biên bản kiểm nhập từ nhà cung cấp", new EventHandler(OnClickInPhieuNhap));
                    itemBienBankiemNhapNCC.Tag = PrintType.BIEN_BAN_KIEM_NHAP_TU_NCC;
                    menu.Items.Add(itemBienBankiemNhapNCC);

                    DXMenuItem itemPhieuNhapMauTuNCC = new DXMenuItem("Phiếu nhập máu từ nhà cung cấp", new EventHandler(OnClickInPhieuNhap));
                    itemPhieuNhapMauTuNCC.Tag = PrintType.PHIEU_NHAP_MAU_TU_NCC;
                    menu.Items.Add(itemPhieuNhapMauTuNCC);
                }
                else
                {
                    cboPrint.Enabled = false;
                }

                cboPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void OnClickInPhieuNhap(object sender, EventArgs e)
        {
            try
            {
                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcess(type);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.BIEN_BAN_KIEM_NHAP_TU_NCC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BienBanKiemNhapTuNhaCungCap_MPS000085, DelegateRunPrinter);
                        break;                 
                    case PrintType.PHIEU_NHAP_MAU_TU_NCC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuNhapMauTuNhaCungCap_MPS000149, DelegateRunPrinter);
                        break;                    
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BienBanKiemNhapTuNhaCungCap_MPS000085:
                        InBienBanKiemNhapTuNhaCungCap(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuNhapMauTuNhaCungCap_MPS000149:
                        InPhieuNhapMauTuNCC(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private void InBienBanKiemNhapTuNhaCungCap(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                MOS.Filter.HisImpMestViewFilter impMestFilter = new MOS.Filter.HisImpMestViewFilter();
                impMestFilter.ID = this.impMestId;
                var rs = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, impMestFilter, param);
                rs = rs.OrderBy(p => p.ID).ToList();
                MOS.Filter.HisImpMestBloodFilter bloodFilter = new MOS.Filter.HisImpMestBloodFilter();
                bloodFilter.IMP_MEST_ID = this.impMestId;
                var bloodData = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_BLOOD>>("/api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, bloodFilter, param);
                bloodData = bloodData.OrderBy(o => o.ID).ToList();


                WaitingManager.Show();

                var ImpMestMaterialPrints = MapImpMestMaterialFromSDO(this.impMestMaterials);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.Processor.Mps000085.PDO.Mps000085PDO mps0000085RDO = new MPS.Processor.Mps000085.PDO.Mps000085PDO(
                 this.impMest,  
                 null,
                 null,
                 rs,
                 null,
                 null,
                 null,
                 null,
                 bloodData
                  );
                WaitingManager.Hide();

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps0000085RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps0000085RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps0000085RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps0000085RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private List<V_HIS_IMP_MEST_MATERIAL> MapImpMestMaterialFromSDO(List<ImpMestMaterialSDODetail> input)
        {
            List<V_HIS_IMP_MEST_MATERIAL> result = new List<V_HIS_IMP_MEST_MATERIAL>();
            try
            {
                foreach (var item in input)
                {
                    V_HIS_IMP_MEST_MATERIAL impMestMedicine = new V_HIS_IMP_MEST_MATERIAL();
                    AutoMapper.Mapper.CreateMap<ImpMestMedicineSDODetail, V_HIS_IMP_MEST_MATERIAL>();
                    impMestMedicine = AutoMapper.Mapper.Map<V_HIS_IMP_MEST_MATERIAL>(item);
                    result.Add(impMestMedicine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void InPhieuNhapMauTuNCC(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                WaitingManager.Show();

                MOS.Filter.HisImpMestBloodFilter filter = new HisImpMestBloodFilter();
                filter.IMP_MEST_ID = impMestId;
                filter.IS_ACTIVE = 1;
                var impMestBloodPrint = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", 
                    ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                MPS.Processor.Mps000149.PDO.Mps000149PDO pdo = new MPS.Processor.Mps000149.PDO.Mps000149PDO(
                 this.impMest,
                 impMestBloodPrint
                );
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.impMest != null ? this.impMest.TDL_TREATMENT_CODE : ""), printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public List<V_HIS_IMP_MEST_BLOOD> MapImpMestBloodFromSDO(List<ImpMestBloodSDODetail> input)
        {
            List<V_HIS_IMP_MEST_BLOOD> result = new List<V_HIS_IMP_MEST_BLOOD>();
            try
            {
                foreach (var item in input)
                {
                    V_HIS_IMP_MEST_BLOOD blood = new V_HIS_IMP_MEST_BLOOD();
                    AutoMapper.Mapper.CreateMap<ImpMestBloodSDODetail, V_HIS_IMP_MEST_BLOOD>();
                    blood = AutoMapper.Mapper.Map<V_HIS_IMP_MEST_BLOOD>(item);
                    result.Add(blood);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        public void BtnAdd()
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

        public void BtnNew()
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

        public void BtnSave()
        {
            try
            {
                gridViewImpMestBlood.PostEditor();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnSaveDraft()
        {
            try
            {
                gridViewImpMestBlood.PostEditor();
                btnSaveDraft_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnUpdate()
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

        public void BtnCancel()
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
        public void BtnBlood()
        {
            try
            {
                btnExportExcel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void BtnPrint()
        {
            try
            {
                btnPrint_Click(null, null);
                //cboPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void CboPrint()
        {
            try
            {
                cboPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }   

        private void cboSupplier_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboSupplier.EditValue != null && cboSupplier.EditValue != cboSupplier.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_SUPPLIER gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtDocumentDate.Focus();
                        }
                    }
                    else
                    {
                        txtDocumentDate.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSupplier_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboSupplier.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_SUPPLIER gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboSupplier.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtDocumentDate.Focus();
                        }
                    }
                    else
                    {
                        cboSupplier.ShowPopup();
                    }
                }
                else
                {
                    cboSupplier.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSupplier_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboSupplier.Text == null)
                {
                    cboSupplier.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpiredDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBloodCode.Focus();
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
                    txtDeliever.Focus();
                    txtDeliever.SelectAll();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestBlood_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                BaseEditViewInfo info = ((DevExpress.XtraGrid.Views.Grid.ViewInfo.GridCellInfo)e.Cell).ViewInfo;
                string error = GetError(e.RowHandle, e.Column);
                SetError(info, error);
                info.CalcViewInfo(e.Graphics);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestBlood_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                string error = GetError(gridViewImpMestBlood.FocusedRowHandle, gridViewImpMestBlood.FocusedColumn);
                if (error == string.Empty) return;
                gridViewImpMestBlood.SetColumnError(gridViewImpMestBlood.FocusedColumn, error, ErrorType.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region BloodGiver-Form
        #endregion

        private void btnNew_BloodGiver_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessBtnNew_BloodGiver();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUpdate_BloodGiver_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessUpdate_BloodGiver();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_BloodGiver_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessAdd_BloodGiver();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImport_BloodGiver_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessImport_BloodGiver();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDownloadSampleFile_BloodGiver_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_BLOOD_GIVER.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_BLOOD_GIVER";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file " + fileName, "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkInWorkPlace_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkInWorkPlace.Checked)
                {
                    dxValidationProvider3.SetValidationRule(txtWorkPlace_BloodGiver, new ValidationRule());
                    lciWorkPlace.AppearanceItemCaption.ForeColor = Color.Black;
                }
                else
                {
                    ValidationSingleControl3(txtWorkPlace_BloodGiver);
                    lciWorkPlace.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkInPermanentAddress_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkInPermanentAddress.Checked)
                {
                    dxValidationProvider3.SetValidationRule(cboProvinceBlood_BloodGiver, new ValidationRule());
                    lciProvinceBlood.AppearanceItemCaption.ForeColor = Color.Black;
                    dxValidationProvider3.SetValidationRule(cboDistrictBlood_BloodGiver, new ValidationRule());
                    lciDistrictBlood.AppearanceItemCaption.ForeColor = Color.Black;
                }
                else
                {
                    ValidationSingleControl3(cboProvinceBlood_BloodGiver);
                    lciProvinceBlood.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationSingleControl3(cboDistrictBlood_BloodGiver);
                    lciDistrictBlood.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCMNDNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsLetterOrDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNational_BloodGiver_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                InitComboProvince();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboProvinceBlood_BloodGiver_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                InitComboDistrict();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboProvince_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                InitComboDistrict();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDistrictBlood_BloodGiver_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                InitComboCommune();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDistrict_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                InitComboCommune();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtVirAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string maTHX = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim();
                    if (String.IsNullOrEmpty(maTHX))
                    {
                        InitComboVirAddress();
                        return;
                    }
                    InitComboVirAddress();
                    this.cboVirAddress.EditValue = null;
                    List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> listResult = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>()
                                                                                            .Where(o => (o.SEARCH_CODE_COMMUNE != null
                                                                                                    && o.SEARCH_CODE_COMMUNE.ToUpper().StartsWith(maTHX.ToUpper()))).ToList();
                    if (listResult != null && listResult.Count >= 1)
                    {
                        var dataNoCommunes = listResult.Where(o => o.ID < 0).ToList();
                        if (dataNoCommunes != null && dataNoCommunes.Count > 1)
                        {
                            InitComboVirAddress(listResult);
                        }
                        else if (dataNoCommunes != null && dataNoCommunes.Count == 1)
                        {
                            this.cboVirAddress.Properties.Buttons[1].Visible = true;
                            this.cboVirAddress.EditValue = dataNoCommunes[0].ID_RAW;
                            this.txtVirAddress.Text = dataNoCommunes[0].SEARCH_CODE_COMMUNE;

                            var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == dataNoCommunes[0].DISTRICT_CODE);
                            if (district != null)
                            {
                                var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_PROVINCE>().SingleOrDefault(o => o.ID == district.PROVINCE_ID);
                                cboProvince.EditValue = province != null ? (long?)province.ID : null;
                                cboDistrict.EditValue = district.ID;
                                cboCommune.EditValue = dataNoCommunes[0].ID;
                            }
                        }
                        else if (listResult.Count == 1)
                        {
                            InitComboVirAddress();
                            this.cboVirAddress.Properties.Buttons[1].Visible = true;
                            this.cboVirAddress.EditValue = listResult[0].ID_RAW;
                            this.txtVirAddress.Text = listResult[0].SEARCH_CODE_COMMUNE;

                            var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == listResult[0].DISTRICT_CODE);
                            if (district != null)
                            {
                                var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_PROVINCE>().SingleOrDefault(o => o.ID == district.PROVINCE_ID);
                                cboProvince.EditValue = province != null ? (long?)province.ID : null;
                                cboDistrict.EditValue = district.ID;
                                cboCommune.EditValue = listResult[0].ID;
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

        private void gridViewImpMestBlood_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = Convert.ToString(this.gridViewImpMestBlood.GetGroupRowValue(e.RowHandle, this.gridColumn_ForGroupingBloodDonation) ?? "");

                e.Info.Paint.FillRectangle(e.Graphics, e.Appearance.GetBackBrush(e.Cache), e.Bounds);
                e.Painter.DrawObject(e.Info);

                Rectangle r = GetButtonBounds(info);
                DrawButton(e.Graphics, r);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestBlood_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
                if (hitInfo.InGroupRow && hitInfo.HitTest != GridHitTest.RowGroupButton)
                {
                    GridViewInfo vi = gridViewImpMestBlood.GetViewInfo() as GridViewInfo;
                    GridGroupRowInfo info = vi.RowsInfo.GetInfoByHandle(hitInfo.RowHandle) as GridGroupRowInfo;
                    Rectangle buttonBounds = GetButtonBounds(info);

                    int childRowHandle = view.GetChildRowHandle(hitInfo.RowHandle, 0);
                    var dataChildRow = (VHisBloodADO)gridViewImpMestBlood.GetRow(childRowHandle);

                    if (dataChildRow != null)
                    {
                        if (buttonBounds.Contains(e.Location))
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Bạn có muốn xóa hồ sơ {0}?", dataChildRow.GIVE_CODE), Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                return;
                            }
                            if (this.dicHisBloodGiver != null && this.dicHisBloodGiver.ContainsKey(dataChildRow.GIVE_CODE))
                            {
                                this.dicHisBloodGiver.Remove(dataChildRow.GIVE_CODE);
                            }
                            if (this.dicHisBloodGiver_BloodAdo != null && this.dicHisBloodGiver_BloodAdo.ContainsKey(dataChildRow.GIVE_CODE))
                            {
                                this.dicHisBloodGiver_BloodAdo.Remove(dataChildRow.GIVE_CODE);
                            }
                            SetDataSourceGridBlood_BloodGiver();

                            if (this.updatingBloodGiverADO != null && this.updatingBloodGiverADO.GIVE_CODE == dataChildRow.GIVE_CODE)
                            {
                                this.updatingBloodGiverADO = null;
                                if (this.bloodGiverActionType == ActionType.Update)
                                {
                                    SetDefaultDataBloodGiverForm();
                                    this.bloodGiverActionType = ActionType.Add;
                                    EnableControlsByActionType_BloodGiverForm();
                                }
                            }
                        }
                        else
                        {
                            if (this.dicHisBloodGiver != null && this.dicHisBloodGiver.ContainsKey(dataChildRow.GIVE_CODE))
                            {
                                this.updatingBloodGiverADO = this.dicHisBloodGiver[dataChildRow.GIVE_CODE];
                                FillDataToBloodGiverForm(this.dicHisBloodGiver[dataChildRow.GIVE_CODE]);
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

        private void gridViewImpMestBlood_GroupRowExpanding(object sender, DevExpress.XtraGrid.Views.Base.RowAllowEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                int childRowHandle = view.GetChildRowHandle(e.RowHandle, 0);
                var dataChildRow = (VHisBloodADO)gridViewImpMestBlood.GetRow(childRowHandle);
                if (dataChildRow != null && dataChildRow.IsEmptyRow)
                    e.Allow = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImpMestBlood_GroupRowExpanded(object sender, DevExpress.XtraGrid.Views.Base.RowEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                var groupRowCount = view.DataController.GroupRowCount;
                int rHandle = -1;
                while (view.IsValidRowHandle(rHandle))
                {
                    int childRowHandle = gridViewImpMestBlood.GetChildRowHandle(rHandle, 0);
                    var dataChildRow = (VHisBloodADO)gridViewImpMestBlood.GetRow(childRowHandle);
                    if (dataChildRow != null && dataChildRow.IsEmptyRow)
                    {
                        gridViewImpMestBlood.SetRowExpanded(rHandle, false);
                    }

                    rHandle--;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboVirAddress_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboVirAddress.EditValue = null;
                    this.cboVirAddress.Properties.Buttons[1].Visible = false;
                    this.txtVirAddress.Text = "";

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboVirAddress_Closed(object sender, ClosedEventArgs e)
        {

        }

        private void cboVirAddress_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboVirAddress.EditValue != null)
                {
                    this.cboVirAddress.Properties.Buttons[1].Visible = true;
                    HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.ID_RAW == (this.cboVirAddress.EditValue ?? "").ToString());
                    if (commune != null)
                    {
                        this.txtVirAddress.Text = commune.SEARCH_CODE_COMMUNE;

                        var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);
                        if (district != null)
                        {
                            var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_PROVINCE>().SingleOrDefault(o => o.ID == district.PROVINCE_ID);
                            cboProvince.EditValue = province != null ? (long?)province.ID : null;
                            cboDistrict.EditValue = district.ID;
                            cboCommune.EditValue = commune.ID;
                        }
                    }
                }
                else
                {
                    this.cboVirAddress.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboVirAddress_Popup(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit edit = sender as GridLookUpEdit;
                if (edit != null)
                {
                    PopupGridLookUpEditForm f = (edit as IPopupControl).PopupWindow as PopupGridLookUpEditForm;
                    if (f != null)
                    {
                        int newPopupFormWidth = cboVirAddress.Width;
                        f.Width = newPopupFormWidth;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Ngày sinh
        private void txtDOB_BloodGiver_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtDOB_BloodGiver.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtDOB_BloodGiver.EditValue = dt;
                        this.dtDOB_BloodGiver.Update();
                    }
                    this.dtDOB_BloodGiver.Visible = true;
                    this.dtDOB_BloodGiver.ShowPopup();
                    this.dtDOB_BloodGiver.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDOB_BloodGiver_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtDOB_BloodGiver.Text)) return;

                string dob = "";
                if (this.txtDOB_BloodGiver.Text.Contains("/"))
                    dob = PatientDobUtil.PatientDobToDobRaw(this.txtDOB_BloodGiver.Text);

                if (!String.IsNullOrWhiteSpace(dob))
                {
                    this.txtDOB_BloodGiver.Text = dob;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDOB_BloodGiver_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                Helpers.DateUtil.DateValidObject dateValidObject = Helpers.DateUtil.ValidPatientDob(this.txtDOB_BloodGiver.Text);
                if (dateValidObject != null)
                {
                    e.ErrorText = dateValidObject.Message;
                }

                AutoValidate = AutoValidate.EnableAllowFocusChange;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDOB_BloodGiver_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDOB_BloodGiver_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Helpers.DateUtil.DateValidObject dateValidObject = Helpers.DateUtil.ValidPatientDob(this.txtDOB_BloodGiver.Text);
                    if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                    {
                        this.txtDOB_BloodGiver.Text = dateValidObject.Age.ToString();
                    }
                    else if (String.IsNullOrEmpty(dateValidObject.Message))
                    {
                        if (!dateValidObject.HasNotDayDob)
                        {
                            this.txtDOB_BloodGiver.Text = dateValidObject.OutDate;
                            //this.dtDOB_BloodGiver.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                            //this.dtDOB_BloodGiver.Update();
                        }
                    }

                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtDOB_BloodGiver.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtDOB_BloodGiver.EditValue = dt;
                        this.dtDOB_BloodGiver.Update();
                    }
                    else
                    {
                        this.dtDOB_BloodGiver.EditValue = null;
                        this.dtDOB_BloodGiver.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDOB_BloodGiver_Validated(object sender, EventArgs e)
        {

        }

        private void txtDOB_BloodGiver_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtDOB_BloodGiver.Text.Trim()))
                    return;
                Helpers.DateUtil.DateValidObject dateValidObject = Helpers.DateUtil.ValidPatientDob(this.txtDOB_BloodGiver.Text);
                if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                {
                    this.txtDOB_BloodGiver.Text = dateValidObject.Age.ToString();
                }
                else if (String.IsNullOrEmpty(dateValidObject.Message) && !String.IsNullOrWhiteSpace(dateValidObject.OutDate))
                {
                    if (!dateValidObject.HasNotDayDob)
                    {
                        this.txtDOB_BloodGiver.Text = dateValidObject.OutDate;
                        this.dtDOB_BloodGiver.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                        this.dtDOB_BloodGiver.Update();
                    }
                }
                else
                {
                    e.Cancel = true;
                    return;
                }

                //this.isNotPatientDayDob = dateValidObject.HasNotDayDob;
                if (
                    ((this.txtDOB_BloodGiver.EditValue ?? "").ToString() != (this.txtDOB_BloodGiver.OldEditValue ?? "").ToString())
                    && (!String.IsNullOrEmpty(dateValidObject.OutDate))
                    )
                {
                    this.dxValidationProvider3.RemoveControlError(this.txtDOB_BloodGiver);
                    this.txtDOB_BloodGiver.ErrorText = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDOB_BloodGiver_Leave(object sender, EventArgs e)
        {
            try
            {
                //DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtDOB_BloodGiver.Text);
                //if (dt != null && dt.Value != DateTime.MinValue)
                //{
                //    this.dtDOB_BloodGiver.EditValue = dt;
                //    this.dtDOB_BloodGiver.Update();
                //}
                //else
                //{
                //    this.dtDOB_BloodGiver.EditValue = null;
                //    this.dtDOB_BloodGiver.Update();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDOB_BloodGiver_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtDOB_BloodGiver.Visible = false;

                    if (!String.IsNullOrWhiteSpace(txtDOB_BloodGiver.Text)
                        && (txtDOB_BloodGiver.Text.Length == 6 || txtDOB_BloodGiver.Text.Length == 7)
                        && (txtDOB_BloodGiver.Text == dtDOB_BloodGiver.DateTime.ToString("MM/yyyy") || txtDOB_BloodGiver.Text == dtDOB_BloodGiver.DateTime.ToString("MMyyyy"))
                        && dtDOB_BloodGiver.DateTime.Day == 1)
                    {
                        this.txtDOB_BloodGiver.Text = dtDOB_BloodGiver.DateTime.ToString("MM/yyyy");
                    }
                    else if (dtDOB_BloodGiver.EditValue == null && dtDOB_BloodGiver.DateTime == DateTime.MinValue)
                    {
                        this.txtDOB_BloodGiver.Text = "";
                        return;
                    }
                    else
                    {
                        this.txtDOB_BloodGiver.Text = dtDOB_BloodGiver.DateTime.ToString("dd/MM/yyyy");
                    }
                    string strDob = dtDOB_BloodGiver.DateTime.ToString("dd/MM/yyyy");
                    this.dtDOB_BloodGiver.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDOB_BloodGiver_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtDOB_BloodGiver.Visible = true;
                    this.dtDOB_BloodGiver.Update();
                    if (!String.IsNullOrWhiteSpace(txtDOB_BloodGiver.Text)
                        && (txtDOB_BloodGiver.Text.Length == 6 || txtDOB_BloodGiver.Text.Length == 7)
                        && (txtDOB_BloodGiver.Text == dtDOB_BloodGiver.DateTime.ToString("MM/yyyy") || txtDOB_BloodGiver.Text == dtDOB_BloodGiver.DateTime.ToString("MMyyyy"))
                        && dtDOB_BloodGiver.DateTime.Day == 1)
                    {
                        this.txtDOB_BloodGiver.Text = dtDOB_BloodGiver.DateTime.ToString("MM/yyyy");
                    }
                    else
                    {
                        this.txtDOB_BloodGiver.Text = dtDOB_BloodGiver.DateTime.ToString("dd/MM/yyyy");
                    }
                    string strDob = dtDOB_BloodGiver.DateTime.ToString("dd/MM/yyyy");
                    this.dtDOB_BloodGiver.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDOB_BloodGiver_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (!String.IsNullOrWhiteSpace(txtDOB_BloodGiver.Text)
                //    && (txtDOB_BloodGiver.Text.Length == 6 || txtDOB_BloodGiver.Text.Length == 7)
                //    && (txtDOB_BloodGiver.Text == dtDOB_BloodGiver.DateTime.ToString("MM/yyyy") || txtDOB_BloodGiver.Text == dtDOB_BloodGiver.DateTime.ToString("MMyyyy"))
                //    && dtDOB_BloodGiver.DateTime.Day == 1)
                //{
                //    this.txtDOB_BloodGiver.Text = dtDOB_BloodGiver.DateTime.ToString("MM/yyyy");
                //}
                //else if (dtDOB_BloodGiver.EditValue == null && dtDOB_BloodGiver.DateTime == DateTime.MinValue)
                //{
                //    this.txtDOB_BloodGiver.Text = "";
                //    return;
                //}
                //else
                //{
                //    this.txtDOB_BloodGiver.Text = dtDOB_BloodGiver.DateTime.ToString("dd/MM/yyyy");
                //}
                DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtDOB_BloodGiver.Text);

                if (dtDOB_BloodGiver.EditValue == null && dtDOB_BloodGiver.DateTime == DateTime.MinValue
                    && !String.IsNullOrWhiteSpace(this.txtDOB_BloodGiver.Text)
                    && (dt == null || dt.Value == DateTime.MinValue))
                {
                    this.txtDOB_BloodGiver.Text = "";
                    return;
                }
                else
                {
                    this.txtDOB_BloodGiver.Text = dtDOB_BloodGiver.DateTime.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider3_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDownLoadTempFile_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath + "/Tmp/Imp", "IMPORT_BLOOD.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_BLOOD";
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
        private void ProcessListBloodTypeToProcessList(ref List<VHisBloodADO> ImpMestListProcessor)
        {
            try
            {
                if (ImpMestListProcessor == null || ImpMestListProcessor.Count == 0)
                    return;
                ImpMestListProcessor = ImpMestListProcessor.Where(o => !string.IsNullOrEmpty(o.BLOOD_TYPE_CODE) || !string.IsNullOrEmpty(o.BLOOD_ABO_CODE) || o.VOLUME > 0 || !string.IsNullOrEmpty(o.BLOOD_CODE) || o.IMP_VAT_RATIO > 0 || !string.IsNullOrEmpty(o.PACKING_TIME_excel) || !string.IsNullOrEmpty(o.BLOOD_RH_CODE) || !string.IsNullOrEmpty(o.EXPIRED_DATE_excel)).ToList();
                foreach (var bloodGiverImport in ImpMestListProcessor)
                {
                    bloodGiverImport.ErrorDescriptions = new List<string>();
                    var bloodType = BackendDataWorker.Get<HIS_BLOOD_TYPE>();
                    //BLOOD_TYPE_CODE
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.BLOOD_TYPE_CODE))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Mã loại máu'");
                    }
                    else
                    {
                        bloodGiverImport.BLOOD_TYPE_CODE = bloodGiverImport.BLOOD_TYPE_CODE.Trim();
                        var bt = bloodType.FirstOrDefault(o => o.BLOOD_TYPE_CODE == bloodGiverImport.BLOOD_TYPE_CODE);
                        if (bt != null)
                        {
                            bloodGiverImport.BLOOD_TYPE_ID = bt.ID;
                            bloodGiverImport.BLOOD_TYPE_NAME = bt.BLOOD_TYPE_NAME;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Mã loại máu' không chính xác");
                        }
                    }
                    //BLOOD_ABO_CODE
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.BLOOD_ABO_CODE))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Nhóm máu'");
                    }
                    else
                    {
                        var bloodABO = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.BLOOD_ABO_CODE != null && o.BLOOD_ABO_CODE.Trim().ToUpper() == (bloodGiverImport.BLOOD_ABO_CODE ?? "").ToUpper());
                        if (bloodABO != null)
                        {
                            bloodGiverImport.BLOOD_ABO_ID = bloodABO.ID;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Nhóm máu' không chính xác");
                        }
                    }
                    if (bloodGiverImport.IMP_VAT_RATIO > 0)
                        bloodGiverImport.ImpVatRatio = bloodGiverImport.IMP_VAT_RATIO;
                    //BLOOD_CODE 
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.BLOOD_CODE))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Mã vạch'");
                    }

                    //BLOOD_RH_CODE
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.BLOOD_RH_CODE))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'RH'");
                    }
                    else
                    {
                        bloodGiverImport.BLOOD_RH_CODE = bloodGiverImport.BLOOD_RH_CODE.Trim();
                        var bloodRH = BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.BLOOD_RH_CODE != null && o.BLOOD_RH_CODE.Trim().ToUpper() == (bloodGiverImport.BLOOD_RH_CODE ?? "").ToUpper());
                        if (bloodRH != null)
                        {
                            bloodGiverImport.BLOOD_RH_ID = bloodRH.ID;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'RH' không chính xác");
                        }
                    }
                    //PACKING_TIME_str
                    if (bloodGiverImport.PACKING_TIME == null || bloodGiverImport.PACKING_TIME == 0)
                    {
                        if (!String.IsNullOrWhiteSpace(bloodGiverImport.PACKING_TIME_excel))
                        {
                            bloodGiverImport.PACKING_TIME_excel = bloodGiverImport.PACKING_TIME_excel.Trim();
                            if (Helpers.DateTimeUtil.IsValidDateStr(bloodGiverImport.PACKING_TIME_excel))
                            {
                                System.DateTime? time = bloodGiverImport.PACKING_TIME_excel.Length == 10 ? Helpers.DateTimeUtil.DateStrToSystemDateTime(bloodGiverImport.PACKING_TIME_excel) : Helpers.DateTimeUtil.DateTimeStrToSystemDateTime(bloodGiverImport.PACKING_TIME_excel);
                                if (time != null && time != DateTime.MinValue)
                                {
                                    bloodGiverImport.PACKING_TIME = Helpers.DateTimeUtil.SystemDateTimeToTimeNumber(time);
                                }
                            }
                            else
                            {
                                bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Thời gian đóng gói' không đúng định dạng");
                            }
                        }
                    }
                    //EXPIRED_DATE_str
                    if (bloodGiverImport.EXPIRED_DATE == null || bloodGiverImport.EXPIRED_DATE == 0)
                        if (String.IsNullOrWhiteSpace(bloodGiverImport.EXPIRED_DATE_excel))
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Hạn sử dụng'");
                        }
                        else
                        {
                            bloodGiverImport.EXPIRED_DATE_excel = bloodGiverImport.EXPIRED_DATE_excel.Trim();
                            if (Helpers.DateTimeUtil.IsValidDateStr(bloodGiverImport.EXPIRED_DATE_excel))
                            {
                                System.DateTime? time = bloodGiverImport.EXPIRED_DATE_excel.Length == 10 ? Helpers.DateTimeUtil.DateStrToSystemDateTime(bloodGiverImport.EXPIRED_DATE_excel) : Helpers.DateTimeUtil.DateTimeStrToSystemDateTime(bloodGiverImport.EXPIRED_DATE_excel);
                                if (time != null && time != DateTime.MinValue)
                                {
                                    if (Int64.Parse((time ?? DateTime.Now).ToString("yyyyMMdd").Substring(0, 8)) >= Int64.Parse(DateTime.Now.ToString("yyyyMMdd")))
                                        bloodGiverImport.EXPIRED_DATE = Helpers.DateTimeUtil.SystemDateTimeToTimeNumber(time);
                                    else
                                        bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Hạn sử dụng' phải lớn hơn ngày hiện tại");
                                }
                            }
                            else
                            {
                                bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Hạn sử dụng' không đúng định dạng");
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportExcel.Enabled)
                {
                    return;
                }
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {

                        var ImpMestListProcessor = import.Get<ADO.VHisBloodADO>(0);
                        if (ImpMestListProcessor != null && ImpMestListProcessor.Count > 0)
                        {
                            ProcessListBloodTypeToProcessList(ref ImpMestListProcessor);
                            dicBloodAdo = new Dictionary<string, VHisBloodADO>();
                            foreach (var item in ImpMestListProcessor)
                            {
                                dicBloodAdo[item.BLOOD_CODE + "_" + item.BLOOD_TYPE_CODE] = item;
                            }
                            try
                            {
                                gridColumn_ForGroupingBloodDonation.GroupIndex = -1;
                                gridColumn_ImpMestBlood_Stt.Visible = true;
                                gridColumn_ImpMestBlood_Stt.VisibleIndex = 0;
                                gridColumn_ImpMestBlood_Stt.Width = 30;
                                gridColumn_ImpMestBlood_Edit.Visible = true;
                                gridColumn_ImpMestBlood_Edit.VisibleIndex = 2;
                                gridColumn_ImpMestBlood_Edit.Width = 25;
                                gridColumn_ImpMestBlood_Delete.VisibleIndex = 1;
                                gridColumn_ImpMestBlood_Delete.Width = 25;

                                gridControlImpMestBlood.BeginUpdate();
                                if (dicBloodAdo != null && dicBloodAdo.Count > 0)
                                {
                                    var lst = dicBloodAdo.Select(s => s.Value).ToList();
                                    gridControlImpMestBlood.DataSource = lst;
                                }
                                else
                                {
                                    gridControlImpMestBlood.DataSource = null;
                                }
                                gridControlImpMestBlood.EndUpdate();
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                            WaitingManager.Hide();
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Xảy ra lỗi khi import từ file");
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Xảy ra lỗi khi import từ file");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                DevExpress.XtraEditors.XtraMessageBox.Show("Xảy ra lỗi khi import từ file");
            }
        }

        private void btnHoiDongKiemNhap_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.impMestId > 0)
                {
                    Inventec.Desktop.Common.Message.WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module currentModule = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisRoleUser").FirstOrDefault();
                    if (currentModule == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisRoleUser");
                    if (currentModule.IsPlugin && currentModule.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        
                        listArgs.Add(LoadImpMestByID(this.impMestId));
                        listArgs.Add(this.impMest);
                        listArgs.Add(this.impMestBloods);
                        listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(currentModule, currentModule.RoomId, currentModule.RoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(currentModule, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
                    }
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
