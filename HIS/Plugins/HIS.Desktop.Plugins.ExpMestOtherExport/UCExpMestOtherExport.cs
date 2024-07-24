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
using HIS.UC.ExpMestMedicineGrid;
using MOS.SDO;
using HIS.UC.ExpMestMedicineGrid.ADO;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.ExpMestOtherExport.ADO;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMaterialGrid.ADO;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestOtherExport.Validation;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.ExpMestOtherExport.Base;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.Plugins.ExpMestOtherExport.Config;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraBars;
using HIS.Desktop.Plugins.ExpMestOtherExport.Config;
using System.IO;

namespace HIS.Desktop.Plugins.ExpMestOtherExport
{
    public partial class UCExpMestOtherExport : HIS.Desktop.Utility.UserControlBase
    {
        const long TYPE_MEDICINE = 1;
        const long TYPE_MATERIAL = 2;
        const long TYPE_BLOOD = 3;

        ExpMestMedicineProcessor expMestMediProcessor = null;
        ExpMestMaterialProcessor expMestMateProcessor = null;

        UserControl ucMediInStock = null;
        UserControl ucMateInStock = null;
        UserControl ucExpMestMedi = null;
        UserControl ucExpMestMate = null;

        List<HisMedicineInStockSDO> listMediInStock;
        List<HisMaterialInStockSDO> listMateInStock;
        List<V_HIS_BLOOD> listBloodInStock;
        List<HIS_BLOOD_GIVER> lstBloodGiver = new List<HIS_BLOOD_GIVER>();

        Dictionary<long, Dictionary<long, MediMateTypeADO>> dicTypeAdo = new Dictionary<long, Dictionary<long, MediMateTypeADO>>();
        MediMateTypeADO currentMediMate = null;

        MediMateTypeADO mediMateRow = null;

        HisExpMestResultSDO resultSdo = null;
        bool isUpdate = false;
        public int Action;

        long roomId;
        long roomTypeId;
        long expMestId;

        decimal totalMoney;
        decimal discountMoney;
        decimal resultMoney;

        bool isClick = false;

        V_HIS_MEDI_STOCK mediStock = null;
        int positionHandleControl = -1;

        PopupMenu menu;

        public UCExpMestOtherExport(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                HisConfigCFG.LoadConfig();
                this.roomTypeId = module.RoomTypeId;
                this.roomId = module.RoomId;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCExpMestOtherExport(Inventec.Desktop.Common.Modules.Module module, long expMestId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                HisConfigCFG.LoadConfig();
                this.roomTypeId = module.RoomTypeId;
                this.roomId = module.RoomId;
                this.expMestId = expMestId;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestOtherExport_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (this.expMestId == 0)
                    EnableControlPriceAndAVT(false);

                InitExpMestMateGrid();
                InitExpMestMediGrid();

                this.Action = GlobalDataStore.ActionAdd;

                LoadMediStockByRoomId();

                if (this.mediStock != null)
                {
                    LoadKeyUCLanguage();
                    ValidControl();
                    ResetValueControlCommon();
                    ResetValueControlDetail();
                    FillDataToTreeMediMate();
                    LoadDataToCboSampleForm();
                    LoadDataToCboReason();

                    if (expMestId > 0)
                    {
                        btnNew.Enabled = false;
                        InitResultSdoByExpMestId();
                    }
                    GenerateMenuPrint();
                }
                else
                {
                    btnAdd.Enabled = false;
                    btnNew.Enabled = false;
                    btnSave.Enabled = false;
                    ddBtnPrint.Enabled = false;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlExpAmount();
                ValidControlExpMestReason();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpAmount()
        {
            try
            {
                ExpAmountValidationRule amountRule = new ExpAmountValidationRule();
                amountRule.spinAmount = spinAmount;
                dxValidationProvider2.SetValidationRule(spinAmount, amountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpMestReason()
        {
            try
            {
                ControlEditValidationRule controlRule = new ControlEditValidationRule();
                controlRule.editor = cboReason;
                controlRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboReason, controlRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboReason()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestReasonFilter filter = new HisExpMestReasonFilter();
                List<HIS_EXP_MEST_REASON> expMestReason = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EXP_MEST_REASON>>(
                    "api/HisExpMestReason/Get", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                expMestReason = expMestReason.Where(rs => rs.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboReason, expMestReason, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeyUCLanguage()
        {
            try
            {
                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var langManager = Resources.ResourceLanguageManager.LanguageUCExpMestOtherExport;
                ////Button
                //this.btnAdd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__BTN_ADD", langManager, cul);
                //this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__BTN_NEW", langManager, cul);
                //this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__BTN_SAVE", langManager, cul);
                //this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__BTN_PRINT", langManager, cul);

                ////Layout

                ////GridControl Detail
                //this.gridColumn_ExpMestDetail_ExpAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_CONTROL__COLUMN_AMOUNT", langManager, cul);
                //this.gridColumn_ExpMestDetail_ManufacturerName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_CONTROL__COLUMN_MANUFACTURER_NAME", langManager, cul);
                //this.gridColumn_ExpMestDetail_MediMateTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_CONTROL__COLUMN_MEDI_MATE_TYPE_NAME", langManager, cul);
                //this.gridColumn_ExpMestDetail_NationalName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_CONTROL__COLUMN_NATIONAL_NAME", langManager, cul);
                //this.gridColumn_ExpMestDetail_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__GRID_CONTROL__COLUMN_SERVICE_UNIT_NAME", langManager, cul);

                ////Xtra Tab
                //this.xtraTabPageExpMestMate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__XTRA_TAB_MATERIAL", langManager, cul);
                //this.xtraTabPageExpMestMedi.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__XTRA_TAB_MEDICINE", langManager, cul);
                //this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__XTRA_TAB_MATERIAL", langManager, cul);
                //this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__XTRA_TAB_MEDICINE", langManager, cul);

                ////Repository Button
                //this.repositoryItemBtnDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__REPOSITORY_BTN_DELETE", langManager, cul);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControl1.Text", langManager, cul);
                this.xtraTabPageExpMediMate.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.xtraTabPageExpMediMate.Text", langManager, cul);
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControl7.Text", langManager, cul);
                this.gridColumn_ExpMestDetail_Delete.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn_ExpMestDetail_Delete.Caption", langManager, cul);
                this.gridColumn_ExpMestDetail_MediMateTypeName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn_ExpMestDetail_MediMateTypeName.Caption", langManager, cul);
                this.gridColumn_ExpMestDetail_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn_ExpMestDetail_ServiceUnitName.Caption", langManager, cul);
                this.gridColumn_ExpMestDetail_ExpAmount.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn_ExpMestDetail_ExpAmount.Caption", langManager, cul);
                this.gridColumn_ExpMestDetail_NationalName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn_ExpMestDetail_NationalName.Caption", langManager, cul);
                this.gridColumn_ExpMestDetail_ManufacturerName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn_ExpMestDetail_ManufacturerName.Caption", langManager, cul);
                this.xtraTabPageExpBloodSend.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.xtraTabPageExpBloodSend.Text", langManager, cul);
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControl8.Text", langManager, cul);
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn8.Caption", langManager, cul);
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn14.Caption", langManager, cul);
                this.gridColumn14.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn14.ToolTip", langManager, cul);
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn9.Caption", langManager, cul);
                this.gridColumn9.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn9.ToolTip", langManager, cul);
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn10.Caption", langManager, cul);
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn10.ToolTip", langManager, cul);
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn11.Caption", langManager, cul);
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn12.Caption", langManager, cul);
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn13.Caption", langManager, cul);
                //this.cboSampleForm.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.cboSampleForm.Properties.NullText", langManager, cul);
                //this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControl5.Text", langManager, cul);
                //this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControl4.Text", langManager, cul);
                this.cboReason.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.cboReason.Properties.NullText", langManager, cul);
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.ddBtnPrint.Text", langManager, cul);
                //this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControl3.Text", langManager, cul);
                //this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControl2.Text", langManager, cul);
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.btnNew.Text", langManager, cul);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.btnSave.Text", langManager, cul);
                this.xtraTabPageExpMestMedi.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.xtraTabPageExpMestMedi.Text", langManager, cul);
                this.xtraTabPageExpMestMate.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.xtraTabPageExpMestMate.Text", langManager, cul);
                this.xtraTabPageBloodExp.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.xtraTabPageBloodExp.Text", langManager, cul);
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControl9.Text", langManager, cul);
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn15.Caption", langManager, cul);
                this.gridColumn15.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn15.ToolTip", langManager, cul);
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn16.Caption", langManager, cul);
                this.gridColumn16.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn16.ToolTip", langManager, cul);
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn17.Caption", langManager, cul);
                this.gridColumn17.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn17.ToolTip", langManager, cul);
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn18.Caption", langManager, cul);
                this.gridColumn18.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn18.ToolTip", langManager, cul);
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn19.Caption", langManager, cul);
                this.gridColumn19.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn19.ToolTip", langManager, cul);
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.btnAdd.Text", langManager, cul);
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.xtraTabPageMedicine.Text", langManager, cul);
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.xtraTabPageMaterial.Text", langManager, cul);
                this.xtraTabPageBlood.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.xtraTabPageBlood.Text", langManager, cul);
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControl6.Text", langManager, cul);

                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn2.Caption", langManager, cul);
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn3.Caption", langManager, cul);
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn3.ToolTip", langManager, cul);
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn4.Caption", langManager, cul);
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn4.ToolTip", langManager, cul);
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn5.Caption", langManager, cul);
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn5.ToolTip", langManager, cul);
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn6.Caption", langManager, cul);
                this.gridColumn6.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn6.ToolTip", langManager, cul);
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn7.Caption", langManager, cul);
                this.gridColumn7.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.gridColumn7.ToolTip", langManager, cul);
                this.lciExpMediStock.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.lciExpMediStock.Text", langManager, cul);
                //this.lciSampleForm.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.lciSampleForm.Text", langManager, cul);
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.lciDescription.Text", langManager, cul);
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.lciAmount.Text", langManager, cul);
                this.lciNote.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.lciNote.Text", langManager, cul);
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControlItem5.Text", langManager, cul);
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControlItem3.Text", langManager, cul);
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.layoutControlItem6.Text", langManager, cul);
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("UCExpMestOtherExport.btnAdd.Text", langManager, cul);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GenerateMenuPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__PRINT_MENU__ITEM_IN_PHIEU_XUAT_KHAC", Resources.ResourceLanguageManager.LanguageUCExpMestOtherExport, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInPhieuXuatKhac)));
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__PRINT_MENU__ITEM_IN_PHIEU_XUAT_KHAC_MAU", Resources.ResourceLanguageManager.LanguageUCExpMestOtherExport, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInPhieuXuatKhacMau)));
                //menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__PRINT_MENU__ITEM_IN_HUONG_DAN_SU_DUNG", Resources.ResourceLanguageManager.LanguageUCExpMestOtherExport, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInHuongDanSuDung)));

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInPhieuXuatKhac(object sender, EventArgs e)
        {
            try
            {
                if (this.resultSdo == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEU_MAU_PHIEU_XUAT_KHAC__MPS000165, deletePrintTemplate);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool deletePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEU_MAU_PHIEU_XUAT_KHAC__MPS000165:
                            InPhieuXuatKhac(ref result, printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuXuatKhac(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (this.resultSdo == null)
                    return;
                CommonParam param = new CommonParam();
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this.resultSdo.ExpMest.ID;
                V_HIS_EXP_MEST expMest = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = null;
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = null;

                if (this.resultSdo.ExpMedicines != null)
                {
                    HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                    medicineFilter.IDs = this.resultSdo.ExpMedicines.Select(o => o.ID).ToList();
                    expMestMedicines = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }

                if (this.resultSdo.ExpMaterials != null)
                {
                    HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter();
                    materialFilter.IDs = this.resultSdo.ExpMaterials.Select(o => o.ID).ToList();
                    expMestMaterials = new BackendAdapter(param)
                         .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, materialFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }


                MPS.Processor.Mps000165.PDO.Mps000165PDO rdo = new MPS.Processor.Mps000165.PDO.Mps000165PDO(expMest, expMestMedicines, expMestMaterials);


                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(expMest.TDL_TREATMENT_CODE, printTypeCode, roomId);

                PrintData.EmrInputADO = inputADO;

                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }

        private void onClickInPhieuXuatKhacMau(object sender, EventArgs e)
        {
            try
            {
                if (this.resultSdo == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeWorker.PRINT_TYPE_CODE__BIEU_MAU_PHIEU_XUAT_KHAC_MAU__MPS000203, deletePrintTemplateBlood);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool deletePrintTemplateBlood(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeWorker.PRINT_TYPE_CODE__BIEU_MAU_PHIEU_XUAT_KHAC_MAU__MPS000203:
                            InPhieuXuatKhacMau(ref result, printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuXuatKhacMau(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (this.resultSdo == null)
                    return;

                List<V_HIS_EXP_MEST_BLOOD> listExpBloods = null;

                CommonParam param = new CommonParam();
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this.resultSdo.ExpMest.ID;
                V_HIS_EXP_MEST expMest = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();

                if (this.resultSdo.ExpBloods != null && this.resultSdo.ExpBloods.Count > 0)
                {
                    HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                    bloodFilter.IDs = this.resultSdo.ExpBloods.Select(o => o.ID).ToList();
                    listExpBloods = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, bloodFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    var bloods = dicTypeAdo.ContainsKey(TYPE_BLOOD) ? dicTypeAdo[TYPE_BLOOD].Select(s => s.Value).Where(o => o.IsBlood == true).ToList() : null;

                    if (bloods != null)
                    {
                        foreach (var item in bloods)
                        {
                            listExpBloods.FirstOrDefault(o => o.BLOOD_ID == item.BLOOD_ID).DESCRIPTION = item.ExpBlood.Description;
                            //listExpBloods.FirstOrDefault(o => o.BLOOD_ID == item.BLOOD_ID). = item.BLOOD_ABO_CODE;
                        }
                    }
                    var distinctBloodGiveIds = listExpBloods
                       .Where(s => s.BLOOD_GIVE_ID != null)
                       .Select(s => s.BLOOD_GIVE_ID)
                       .Distinct()
                       .ToList();
                    lstBloodGiver = BackendDataWorker.Get<HIS_BLOOD_GIVER>().Where(s => distinctBloodGiveIds.Contains(s.ID)).ToList();

                    MPS.Processor.Mps000203.PDO.Mps000203ADO mps000203Ado = new MPS.Processor.Mps000203.PDO.Mps000203ADO();
                    if (this.resultSdo.ExpMest.EXP_MEST_REASON_ID != null)
                        mps000203Ado.EXP_REASON_NAME = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().FirstOrDefault(o => o.ID == this.resultSdo.ExpMest.EXP_MEST_REASON_ID).EXP_MEST_REASON_NAME;
                    if (this.resultSdo.ExpMest.IMP_MEDI_STOCK_ID != null)
                    {
                        mps000203Ado.IMP_MEDI_STOCK_CODE = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.resultSdo.ExpMest.IMP_MEDI_STOCK_ID).MEDI_STOCK_CODE;
                        mps000203Ado.IMP_MEDI_STOCK_NAME = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.resultSdo.ExpMest.IMP_MEDI_STOCK_ID).MEDI_STOCK_NAME;
                    }

                    MPS.Processor.Mps000203.PDO.Mps000203PDO mps000203PDO = new MPS.Processor.Mps000203.PDO.Mps000203PDO
                    (
                     expMest,
                     listExpBloods,
                     mps000203Ado,
                     lstBloodGiver
                      );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000203PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000203PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(expMest.TDL_TREATMENT_CODE, printTypeCode, roomId);

                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }

        private void ResetValueControlDetail()
        {
            try
            {
                this.currentMediMate = null;
                spinAmount.Value = 0;
                txtNote.Text = "";
                spinPriceVAT.EditValue = null;
                spinExpPrice.EditValue = null;
                btnAdd.Enabled = false;
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
                if (this.mediStock != null)
                {
                    txtExpMediStock.Text = this.mediStock.MEDI_STOCK_NAME;
                }
                else
                {
                    txtExpMediStock.Text = "";
                }
                txtRecevingPlace.Text = "";
                txtRecipient.Text = "";
                dicTypeAdo.Clear();
                btnAdd.Text = "Thêm (Ctr A)";
                isClick = false;
                gridControlExpBlood.BeginUpdate();
                gridControlExpBlood.DataSource = null;
                gridControlExpBlood.EndUpdate();
                this.currentMediMate = null;
                this.isUpdate = false;
                //txtSampleForm.Text = "";
                //cboSampleForm.EditValue = null;
                cboReason.EditValue = null;
                cboReason.Properties.Buttons[1].Visible = false;
                txtDescription.Text = "";
                spinEditDiscountPercent.Value = 0;
                ddBtnPrint.Enabled = false;
                spinExpPrice.EditValue = null;
                spinPriceVAT.EditValue = null;
                txtKeyworkBloodInStock.Text = "";
                txtKeyworkMaterialInStock.Text = "";
                txtKeyworkMedicineInStock.Text = "";
                gridControlExpMestMedicine.DataSource = null;
                gridControlExpMestMaterial.DataSource = null;
                gridControlBloodExp.DataSource = null;
                gridControlMaterialReuse.DataSource = null;
                gridControlMaterialAdd.DataSource = null;
                gridControlMaterialReuseRs.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboSampleForm()
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("EXP_MEST_TEMPLATE_CODE", "", 100, 1));
                //columnInfos.Add(new ColumnInfo("EXP_MEST_TEMPLATE_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_TEMPLATE_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboSampleForm, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToTreeMediMate()
        {
            try
            {
                listMateInStock = null;
                listMediInStock = null;
                listBloodInStock = null;
                if (mediStock != null)
                {
                    List<Action> methods = new List<Action>();
                    methods.Add(LoadMedicineTypeFromStock);
                    methods.Add(LoadMaterialTypeFromStock);
                    methods.Add(LoadBloodFromStock);
                    ThreadCustomManager.MultipleThreadWithJoin(methods);
                }
                gridControlMedicineInStock.DataSource = listMediInStock;
                gridControlMaterialInStock.DataSource = listMateInStock;
                gridControlBlood.DataSource = listBloodInStock;
                gridControlMaterialReuse.DataSource = this._vMaterialBeans;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMediStockByRoomId()
        {
            try
            {
                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_TYPE_ID == this.roomTypeId && o.ROOM_ID == this.roomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataGridExpMestDetail()
        {
            try
            {
                List<MediMateTypeADO> listMateMedi = new List<MediMateTypeADO>();
                List<MediMateTypeADO> listBlood = new List<MediMateTypeADO>();
                //List<MediMateTypeADO> listMaterialReuses = new List<MediMateTypeADO>();
                if (dicTypeAdo.ContainsKey(TYPE_MEDICINE))
                {
                    listMateMedi.AddRange(dicTypeAdo[TYPE_MEDICINE].Select(s => s.Value).ToList());
                }
                if (dicTypeAdo.ContainsKey(TYPE_MATERIAL))
                {
                    listMateMedi.AddRange(dicTypeAdo[TYPE_MATERIAL].Select(s => s.Value).ToList());
                }
                if (dicTypeAdo.ContainsKey(TYPE_BLOOD))
                {
                    listBlood.AddRange(dicTypeAdo[TYPE_BLOOD].Select(s => s.Value).ToList());
                }
                //if (dicTypeAdo.ContainsKey(TYPE_MATERIAL_REUSE))
                //{
                //    listMaterialReuses.AddRange(dicTypeAdo[TYPE_MATERIAL_REUSE].Select(s => s.Value).ToList());
                //}
                gridControlExpMestDetail.BeginUpdate();
                gridControlExpMestDetail.DataSource = listMateMedi;
                gridControlExpMestDetail.EndUpdate();

                gridControlExpBlood.BeginUpdate();
                gridControlExpBlood.DataSource = listBlood;
                gridControlExpBlood.EndUpdate();

                //gridControlMaterialAdd.BeginUpdate();
                //gridControlMaterialAdd.DataSource = listMaterialReuses;
                //gridControlMaterialAdd.EndUpdate();

                SetDataToLabels_MoneyCalculated();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToLabels_MoneyCalculated()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetDataToLabels_MoneyCalculated():");
                List<MediMateTypeADO> listMateMedi = (List<MediMateTypeADO>)gridControlExpMestDetail.DataSource ?? new List<MediMateTypeADO>();
                List<MediMateTypeADO> listBlood = (List<MediMateTypeADO>)gridControlExpBlood.DataSource ?? new List<MediMateTypeADO>();
                List<MediMateTypeADO> listMaterialReuses = (List<MediMateTypeADO>)gridControlMaterialAdd.DataSource ?? new List<MediMateTypeADO>();

                var discount = spinEditDiscountPercent.Value / 100;

                this.totalMoney = listMateMedi.Sum(o => o.EXP_PRICE * (1 + o.EXP_VAT_RATIO) * o.EXP_AMOUNT) ?? 0 + listMaterialReuses.Sum(o => o.EXP_PRICE * (1 - o.EXP_VAT_RATIO) * o.EXP_AMOUNT) ?? 0;
                this.discountMoney = totalMoney * discount;
                this.resultMoney = totalMoney - discountMoney;

                if (totalMoney != 0)
                    lblTotalMoney.Text = String.Format("{0:#,##0}", totalMoney);
                else
                    lblTotalMoney.Text = "0";
                if (discountMoney != 0)
                    lblDiscountMoney.Text = String.Format("{0:#,##0}", discountMoney);
                else
                    lblDiscountMoney.Text = "0";
                if (resultMoney != 0)
                    lblResultMoney.Text = String.Format("{0:#,##0}", resultMoney);
                else
                    lblResultMoney.Text = "0";
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

        private void FillDataToGridExpMest()
        {
            try
            {
                if (this.resultSdo != null)
                {
                    if (dicTypeAdo.ContainsKey(TYPE_MATERIAL))
                    {
                        gridControlExpMestMaterial.DataSource = dicTypeAdo[TYPE_MATERIAL].Select(s => s.Value).Where(o => o.IsBlood == false && o.IsMedicine == false).ToList();
                    }
                    else
                    {
                        gridControlExpMestMaterial.DataSource = null;
                    }
                    if (dicTypeAdo.ContainsKey(TYPE_MEDICINE))
                    {
                        gridControlExpMestMedicine.DataSource = dicTypeAdo[TYPE_MEDICINE].Select(s => s.Value).Where(o => o.IsMedicine == true).ToList();
                    }
                    else
                    {
                        gridControlExpMestMedicine.DataSource = null;
                    }
                    var dataReuses = (List<V_HIS_MATERIAL_BEAN_1>)gridControlMaterialAdd.DataSource;
                    if (dataReuses != null && dataReuses.Count > 0)
                    {
                        gridControlMaterialReuseRs.DataSource = dataReuses;
                    }
                    else
                    {
                        gridControlMaterialReuseRs.DataSource = null;
                    }
                    if (dicTypeAdo.ContainsKey(TYPE_BLOOD))
                    {
                        gridControlBloodExp.DataSource = dicTypeAdo[TYPE_BLOOD].Select(s => s.Value).Where(o => o.IsBlood == true).ToList();
                    }
                    else
                    {
                        gridControlBloodExp.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnAdd()
        {
            try
            {
                if (btnAdd.Enabled)
                    btnAdd_Click(null, null);
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
                if (btnSave.Enabled)
                {
                    gridViewExpMestDetail.PostEditor();
                    btnSave_Click(null, null);
                }
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
                if (btnNew.Enabled)
                    btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleForm_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //try
            //{
            //    if (e.Button.Kind == ButtonPredefines.Delete)
            //    {
            //        cboSampleForm.Properties.Buttons[1].Visible = false;
            //        cboSampleForm.EditValue = null;
            //        txtSampleForm.Text = "";
            //        txtSampleForm.Focus();
            //        gridControlExpMestDetail.DataSource = null;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinExpPrice.Enabled)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNote_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleForm_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void cboReason_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboReason.EditValue != null && cboReason.EditValue != cboReason.OldEditValue)
                    {
                        HIS_EXP_MEST_REASON reason = BackendDataWorker.Get<HIS_EXP_MEST_REASON>()
                            .SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboReason.EditValue.ToString()));
                        if (reason != null)
                        {
                            if (reason.ID == HisConfigCFG.HisExpMestReasonId__ThanhLy)
                            {
                                EnableControlPriceAndAVT(true);
                            }
                            else
                            {
                                EnableControlPriceAndAVT(false);
                            }
                            cboReason.Properties.Buttons[1].Visible = true;
                            txtRecipient.Focus();
                            txtRecipient.SelectAll();
                        }
                    }
                    else
                    {
                        txtRecipient.Focus();
                        txtRecipient.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboReason_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboReason.EditValue != null)
                    {
                        HIS_EXP_MEST_REASON gt = BackendDataWorker.Get<HIS_EXP_MEST_REASON>()
                            .SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboReason.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtRecipient.Focus();
                            txtRecipient.SelectAll();
                            //txtSampleForm.Focus();
                            cboReason.ShowPopup();
                        }
                    }
                    else
                    {
                        cboReason.ShowPopup();
                    }
                }
                else
                {
                    cboReason.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReason_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboReason.Properties.Buttons[1].Visible = false;
                    cboReason.EditValue = null;
                    EnableControlPriceAndAVT(false);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAddBlood_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                xtraTabControlExpMediMate.SelectedTabPage = xtraTabPageExpBloodSend;

                this.currentMediMate = null;
                var data = (V_HIS_BLOOD)gridViewBlood.GetFocusedRow();
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                    this.currentMediMate.EXP_AMOUNT = 1;

                    if (this.currentMediMate.IsMedicine == false && this.currentMediMate.IsBlood == true)
                    {
                        this.currentMediMate.ExpBlood = new ExpBloodSDO();
                        this.currentMediMate.ExpBlood.BloodId = this.currentMediMate.BLOOD_ID;
                        this.currentMediMate.ExpBlood.Description = txtDescription.Text;
                    }
                    if (!dicTypeAdo.ContainsKey(TYPE_BLOOD))
                    {
                        dicTypeAdo[TYPE_BLOOD] = new Dictionary<long, MediMateTypeADO>();
                    }
                    var dic = dicTypeAdo[TYPE_BLOOD];
                    dic[this.currentMediMate.BLOOD_ID] = this.currentMediMate;
                    this.FillDataGridExpMestDetail();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewBlood_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var data = (MOS.EFMODEL.DataModels.V_HIS_BLOOD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }


        //private void gridViewExpMestDetail_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        //{

        //}

        //private void gridViewExpMestDetail_ShownEditor(object sender, EventArgs e)
        //{

        //}

        //private void gridViewExpMestDetail_CellValueChanged(object sender, CellValueChangedEventArgs e)
        //{

        //}

        //private void gridViewExpMestDetail_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        //{

        //}

        private void gridViewExpBlood_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //try
            //{
            //    if (e.RowHandle < 0 || e.Column.FieldName != "EXP_AMOUNT")
            //        return;
            //    var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
            //    if (data != null)
            //    {
            //        if (data.IsMedicine)
            //        {
            //            data.ExpMedicine.Amount = data.EXP_AMOUNT;
            //        }
            //        else
            //        {
            //            data.ExpMaterial.Amount = data.EXP_AMOUNT;
            //        }
            //        if (data.EXP_AMOUNT > data.AVAILABLE_AMOUNT)
            //        {
            //            data.IsGreatAvailable = true;
            //        }
            //        else
            //        {
            //            data.IsGreatAvailable = false;
            //        }
            //    }
            //    gridControlExpMestDetail.RefreshDataSource();
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void gridViewExpBlood_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            //try
            //{
            //    e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void gridViewExpBlood_ShownEditor(object sender, EventArgs e)
        {
            //try
            //{
            //    if (gridViewExpMestDetail.FocusedRowHandle < 0 || gridViewExpMestDetail.FocusedColumn.FieldName != "EXP_AMOUNT")
            //        return;
            //    var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[gridViewExpMestDetail.FocusedRowHandle];
            //    if (data != null)
            //    {
            //        bool valid = true;
            //        string message = "";
            //        if (data.EXP_AMOUNT <= 0)
            //        {
            //            valid = false;
            //            message = LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.SoLuongKhongDuocBeHonKhong);
            //        }
            //        else if (data.EXP_AMOUNT > data.AVAILABLE_AMOUNT)
            //        {
            //            valid = false;
            //            message = LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.ChiDinhThuoc_SoLuongXuatLonHonSpoLuongKhadungTrongKho);
            //        }
            //        if (!valid)
            //        {
            //            gridViewExpMestDetail.SetColumnError(gridViewExpMestDetail.FocusedColumn, message);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void gridViewExpBlood_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
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

        private void repositoryItemBtnDeleteBlood_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlBloodExp_Click(object sender, EventArgs e)
        {

        }


        private void repositoryItemButtonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (MediMateTypeADO)gridViewExpBlood.GetFocusedRow();
                if (data != null)
                {
                    if (data.IsBlood)
                    {
                        dicTypeAdo[TYPE_BLOOD].Remove(data.MEDI_MATE_TYPE_ID);
                    }
                    else if (data.IsMedicine)
                    {
                        dicTypeAdo[TYPE_MEDICINE].Remove(data.MEDI_MATE_TYPE_ID);
                    }
                    else
                    {
                        dicTypeAdo[TYPE_MATERIAL].Remove(data.MEDI_MATE_TYPE_ID);
                    }
                }
                FillDataGridExpMestDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void xtraTabControlMain_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (e.Page == xtraTabPageBlood || e.Page == xtraTabPageReuse)
                {
                    spinAmount.Enabled = false;
                    txtNote.Enabled = false;
                    btnAdd.Enabled = false;
                    spinExpPrice.Enabled = false;
                    spinPriceVAT.Enabled = false;
                }
                else
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64((cboReason.EditValue ?? 0).ToString()) == HisConfigCFG.HisExpMestReasonId__ThanhLy)
                    {
                        spinExpPrice.Enabled = true;
                        spinPriceVAT.Enabled = true;
                    }
                    else
                    {
                        spinExpPrice.Enabled = false;
                        spinPriceVAT.Enabled = false;
                    }
                    spinAmount.Enabled = true;
                    txtNote.Enabled = true;
                    btnAdd.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewBlood_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAddBlood_ButtonClick(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewMedicineInStock_Click(object sender, EventArgs e)
        {
            try
            {
                var data = gridViewMedicineInStock.GetFocusedRow() as HisMedicineInStockSDO;
                this.currentMediMate = null;
                if (data != null)
                {
                    btnAdd.Text = "Thêm (Ctr A)";
                    isClick = false;
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                SetValueByMediMateADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterialInStock_Click(object sender, EventArgs e)
        {
            try
            {
                var data = gridViewMaterialInStock.GetFocusedRow() as HisMaterialInStockSDO;
                this.currentMediMate = null;
                if (data != null)
                {
                    btnAdd.Text = "Thêm (Ctr A)";
                    isClick = false;
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                SetValueByMediMateADO();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyworkMedicineInStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtKeyworkMedicineInStock.Text) && listMediInStock != null)
                {
                    List<HisMedicineInStockSDO> medicineInStocks = listMediInStock.Where(o =>
                        o.MEDICINE_TYPE_NAME.ToUpper().Contains(txtKeyworkMedicineInStock.Text.Trim().ToUpper())
                        || (!string.IsNullOrEmpty(o.ACTIVE_INGR_BHYT_NAME) && o.ACTIVE_INGR_BHYT_NAME.ToUpper().Contains(txtKeyworkMedicineInStock.Text.Trim().ToUpper()))
                        || o.MEDICINE_TYPE_CODE.ToUpper().Contains(txtKeyworkMedicineInStock.Text.Trim().ToUpper())).ToList();
                    gridControlMedicineInStock.DataSource = medicineInStocks;
                }
                else
                {
                    gridControlMedicineInStock.DataSource = listMediInStock;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyworkMaterialInStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtKeyworkMaterialInStock.Text) && listMateInStock != null)
                {
                    List<HisMaterialInStockSDO> materialInStocks = listMateInStock.Where(o =>
                        o.MATERIAL_TYPE_NAME.ToUpper().Contains(txtKeyworkMaterialInStock.Text.Trim().ToUpper())
                        || o.MATERIAL_TYPE_CODE.ToUpper().Contains(txtKeyworkMaterialInStock.Text.Trim().ToUpper())).ToList();
                    gridControlMaterialInStock.DataSource = materialInStocks;
                }
                else
                {
                    gridControlMaterialInStock.DataSource = listMateInStock;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyworkBloodInStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Do có tính năng quét QR nên sẽ chuyển đoạn code này sang nhấn nút Enter

                //if (!String.IsNullOrEmpty(txtKeyworkBloodInStock.Text) && listBloodInStock != null)
                //{
                //    List<V_HIS_BLOOD> bloodInStocks = listBloodInStock.Where(o =>
                //        o.BLOOD_CODE.ToUpper().Contains(txtKeyworkBloodInStock.Text.Trim().ToUpper())
                //        || o.BLOOD_TYPE_NAME.ToUpper().Contains(txtKeyworkBloodInStock.Text.Trim().ToUpper())).ToList();
                //    gridControlBlood.DataSource = bloodInStocks;
                //}
                //else
                //{
                //    gridControlBlood.DataSource = listBloodInStock;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewMedicineInStock_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisMedicineInStockSDO dataRow = (HisMedicineInStockSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                    {
                        if (dataRow.EXPIRED_DATE.HasValue)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)dataRow.EXPIRED_DATE.Value);
                        }
                    }
                    if (e.Column.FieldName == "CONCENTRA")
                    {
                        if (dataRow.CONCENTRA != null)
                        {
                            e.Value = dataRow.CONCENTRA;
                        }
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterialInStock_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisMaterialInStockSDO data = (HisMaterialInStockSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                    {
                        if (data.EXPIRED_DATE.HasValue)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.EXPIRED_DATE.Value);
                        }
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpBlood_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

        }

        private void gridControlExpMestDetail_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MediMateTypeADO)gridViewExpMestDetail.GetFocusedRow();
                if (row != null)
                {
                    if (row.IsMedicine)
                    {
                        xtraTabControlMain.SelectedTabPage = xtraTabPageMedicine;
                    }
                    else
                    {
                        xtraTabControlMain.SelectedTabPage = xtraTabPageMaterial;
                    }

                    isClick = true;
                    btnAdd.Enabled = true;
                    this.currentMediMate = row;
                    btnAdd.Text = "Cập nhật (Ctr A)";
                    spinAmount.EditValue = row.EXP_AMOUNT;
                    txtNote.Text = row.NOTE;
                    spinPriceVAT.EditValue = row.EXP_VAT_RATIO * 100;
                    spinExpPrice.EditValue = row.EXP_PRICE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemSpinExpAmount_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    var row = (MediMateTypeADO)gridViewExpMestDetail.GetFocusedRow();
                    if (row != null)
                    {
                        spinAmount.EditValue = row.EXP_AMOUNT;
                        txtNote.Text = row.NOTE;
                        spinPriceVAT.EditValue = row.EXP_VAT_RATIO;
                        spinExpPrice.EditValue = row.EXP_PRICE;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtKeyworkMedicineInStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridViewMedicineInStock.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtKeyworkMaterialInStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridViewMaterialInStock.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewMedicineInStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridViewMedicineInStock_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewMaterialInStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridViewMaterialInStock_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private string DecodeTime(string input)
        {
            string result = null;
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    result = input.Split('/')[2] + input.Split('/')[1] + input.Split('/')[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void txtKeyworkBloodInStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (listBloodInStock != null && listBloodInStock.Count > 0 && !string.IsNullOrEmpty(txtKeyworkBloodInStock.Text.Trim()) && (((List<V_HIS_BLOOD>)gridViewBlood.GridControl.DataSource) != null && ((List<V_HIS_BLOOD>)gridViewBlood.GridControl.DataSource).Count != 0))
                    {
                        var key = txtKeyworkBloodInStock.Text.Trim();
                        if (string.IsNullOrEmpty(key))
                        {
                            gridControlBlood.DataSource = listBloodInStock;
                        }
                        else if (!String.IsNullOrEmpty(key) && !key.Contains("|") && listBloodInStock != null)
                        {
                            gridControlBlood.DataSource = listBloodInStock.Where(o =>
                                o.BLOOD_CODE.ToUpper().Contains(txtKeyworkBloodInStock.Text.Trim().ToUpper())
                                || o.BLOOD_TYPE_NAME.ToUpper().Contains(txtKeyworkBloodInStock.Text.Trim().ToUpper())).ToList();
                            return;
                        }
                        //else if (!key.Contains("|"))
                        //{
                        //    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu QR không hợp lê vui lòng kiểm tra lại!");
                        //    return;
                        //}
                        //VjUwMDUyMjQwODExNzAwfE8gK3x8MTkvMTEvMjAyMnwzMC8xMi8yMDIyfEU3NDI2VjAwfEtI4buQSSBI4buSTkcgQ+G6plUgVOG7qiAzNTBtbCBNw4FVIFRPw4BOIFBI4bqmTnwoIDIgLSA2wrBDICk=
                        //V50052240811700|O +||19/11/2022|30/12/2022|E7426V00|KHỐI HỒNG CẦU TỪ 350ml MÁU TOÀN PHẦN|( 2 - 6°C )
                        //<Mã vạch>|<Nhóm máu> <Rh>||<Thời gian đóng gói>|<Hạn sử dụng>|<Mã loại máu>|<Tên loại máu>|<điều kiện bảo quản>

                        try
                        {
                            var QR = key.Split('|');
                            var packingTime = DecodeTime(QR[3] ?? "0");
                            var expriredTime = DecodeTime(QR[4] ?? "0");
                            List<V_HIS_BLOOD> bloodInStocks = listBloodInStock.Where(o =>
                            o.BLOOD_CODE.ToUpper().Contains(QR[0].Trim().ToUpper())
                            && o.BLOOD_ABO_CODE.ToUpper().Contains(QR[1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToUpper())
                            && o.BLOOD_RH_CODE.ToUpper().Contains(QR[1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1].Trim().ToUpper())
                            && o.BLOOD_TYPE_CODE.ToUpper().Contains(QR[5].Trim().ToUpper())
                            && o.BLOOD_TYPE_NAME.ToUpper().Contains(QR[6].Trim().ToUpper())
                            && o.PACKING_TIME != null && o.PACKING_TIME.ToString().Substring(0, 8).Equals(packingTime)
                            && o.EXPIRED_DATE != null && o.EXPIRED_DATE == Int64.Parse(expriredTime + "000000")).ToList();

                            gridControlBlood.DataSource = bloodInStocks;
                            if (bloodInStocks != null && bloodInStocks.Count == 1)
                            {
                                gridViewBlood.FocusedRowHandle = 0;
                                btnAddBlood_ButtonClick(null, null);
                            }
                            else
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Không có loại máu tương ứng");
                            }
                        }
                        catch (Exception ex)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu QR không hợp lê vui lòng kiểm tra lại!");
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }

                    }
                    gridViewBlood.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinExpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinPriceVAT.Enabled)
                    {
                        spinPriceVAT.Focus();
                        spinPriceVAT.SelectAll();
                    }
                    else
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spinPriceVAT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtNote.Enabled)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    else
                    {
                        btnAdd.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridControlExpBlood_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MediMateTypeADO)gridViewExpBlood.GetFocusedRow();
                if (row != null)
                {
                    if (row.IsBlood)
                    {
                        xtraTabControlMain.SelectedTabPage = xtraTabPageBlood;
                    }

                    //isClick = true;
                    //btnAdd.Enabled = true;
                    //this.currentMediMate = row;
                    btnAdd.Text = "Thêm (Ctr A)";
                    //spinAmount.EditValue = null;
                    //spinAmount.Enabled = false;
                    //txtNote.Text = "";
                    //txtNote.Enabled = false;
                    //spinPriceVAT.EditValue = null;
                    //spinPriceVAT.Enabled = false;
                    //spinExpPrice.EditValue = null;
                    //spinExpPrice.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlMedicineInStock_Click(object sender, EventArgs e)
        {

        }

        private void gridControlBlood_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (V_HIS_BLOOD)gridViewBlood.GetFocusedRow();
                if (row != null)
                {
                    btnAdd.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void xtraTabControlExpMediMate_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (xtraTabControlExpMediMate.SelectedTabPage == xtraTabPageExpBloodSend)
                    btnAdd.Text = "Thêm (Ctrl A)";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ddBtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager1);
                // Add item and show
                menu.ItemLinks.Clear();

                BarButtonItem itemMediMate = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__PRINT_MENU__ITEM_IN_PHIEU_XUAT_KHAC", Resources.ResourceLanguageManager.LanguageUCExpMestOtherExport, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                itemMediMate.ItemClick += new ItemClickEventHandler(onClickInPhieuXuatKhac);
                menu.AddItems(new BarItem[] { itemMediMate });

                BarButtonItem itemBlood = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_OTHER_EXPORT__PRINT_MENU__ITEM_IN_PHIEU_XUAT_KHAC_MAU", Resources.ResourceLanguageManager.LanguageUCExpMestOtherExport, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                itemBlood.ItemClick += new ItemClickEventHandler(onClickInPhieuXuatKhacMau);
                menu.AddItems(new BarItem[] { itemBlood });

                menu.ShowPopup(Cursor.Position);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnDownloadTemplate.Enabled) return;
                var source = System.IO.Path.Combine(Application.StartupPath
                + "/Tmp/Imp", "IMPORT_EXP_OTHER.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_EXP_OTHER";
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

        private void repositoryItemButton__Add_Reuse_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                xtraTabControlExpMediMate.SelectedTabPage = xtraTabPageMaterialReuse;

                this.currentMediMate = null;
                var data = (V_HIS_MATERIAL_BEAN_1)gridViewMaterialReuse.GetFocusedRow();
                var dataAdds = (List<V_HIS_MATERIAL_BEAN_1>)gridControlMaterialAdd.DataSource;
                if (data != null)
                {
                    if (dataAdds == null)
                        dataAdds = new List<V_HIS_MATERIAL_BEAN_1>();
                    dataAdds.Add(data);

                    gridControlMaterialAdd.BeginUpdate();
                    gridControlMaterialAdd.DataSource = dataAdds;
                    gridControlMaterialAdd.EndUpdate();


                    var dataOdds = (List<V_HIS_MATERIAL_BEAN_1>)gridControlMaterialReuse.DataSource;

                    if (dataOdds != null && dataOdds.Count > 0)
                    {
                        dataOdds.Remove(data);

                        gridControlMaterialReuse.BeginUpdate();
                        gridControlMaterialReuse.DataSource = dataOdds;
                        gridControlMaterialReuse.EndUpdate();
                    }

                    //this.currentMediMate = new ADO.MediMateTypeADO(data);
                    //this.currentMediMate.EXP_AMOUNT = 1;

                    //if (this.currentMediMate.ExpMaterial != null)
                    //{
                    //    this.currentMediMate.ExpMaterial.Description = txtDescription.Text;
                    //}
                    //if (!dicTypeAdo.ContainsKey(TYPE_MATERIAL_REUSE))
                    //{
                    //    dicTypeAdo[TYPE_MATERIAL_REUSE] = new Dictionary<long, MediMateTypeADO>();
                    //}
                    //var dic = dicTypeAdo[TYPE_MATERIAL_REUSE];
                    //dic[this.currentMediMate.MEDI_MATE_TYPE_ID] = this.currentMediMate;
                    //this.FillDataGridExpMestDetail();

                    //List<V_HIS_MATERIAL_BEAN_1> datas = this._vMaterialBeans.ToList();
                    //if (datas != null && datas.Count > 0)
                    //{
                    //    var dataNews = (List<MediMateTypeADO>)gridControlMaterialAdd.DataSource;
                    //    foreach (var item in dataNews)
                    //    {
                    //        var dataRemove = this._vMaterialBeans.FirstOrDefault(p => p.MATERIAL_TYPE_ID == item.MEDI_MATE_TYPE_ID && p.SERIAL_NUMBER == item.SERIAL_NUMBER);
                    //        if (dataRemove != null)
                    //            datas.Remove(dataRemove);
                    //    }

                    //    datas = datas.OrderBy(p => p.MATERIAL_TYPE_CODE).ToList();
                    //    gridControlMaterialReuse.BeginUpdate();
                    //    gridControlMaterialReuse.DataSource = datas;
                    //    gridControlMaterialReuse.EndUpdate();
                    //}

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__DeleteMaterialReuse_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_HIS_MATERIAL_BEAN_1)gridViewMaterialAdd.GetFocusedRow();
                if (data != null)
                {
                    //dicTypeAdo[TYPE_MATERIAL_REUSE].Remove(data.MEDI_MATE_TYPE_ID);
                    var dataAdds = (List<V_HIS_MATERIAL_BEAN_1>)gridControlMaterialAdd.DataSource;
                    if (dataAdds == null)
                        dataAdds = new List<V_HIS_MATERIAL_BEAN_1>();
                    dataAdds.Remove(data);

                    gridControlMaterialAdd.BeginUpdate();
                    gridControlMaterialAdd.DataSource = dataAdds;
                    gridControlMaterialAdd.EndUpdate();


                    var dataOdds = (List<V_HIS_MATERIAL_BEAN_1>)gridControlMaterialReuse.DataSource;

                    if (dataOdds != null && dataOdds.Count > 0)
                    {
                        dataOdds.Add(data);

                        dataOdds = dataOdds.OrderBy(p => p.MATERIAL_TYPE_CODE).ToList();

                        gridControlMaterialReuse.BeginUpdate();
                        gridControlMaterialReuse.DataSource = dataOdds;
                        gridControlMaterialReuse.EndUpdate();
                    }
                }
                //FillDataGridExpMestDetail();

                //var dataNew = this._vMaterialBeans.FirstOrDefault(p => p.MATERIAL_TYPE_ID == data.MEDI_MATE_TYPE_ID && p.SERIAL_NUMBER == data.SERIAL_NUMBER);
                //List<V_HIS_MATERIAL_BEAN_1> datas = (List<V_HIS_MATERIAL_BEAN_1>)gridControlMaterialReuse.DataSource;
                //if (dataNew != null)
                //{
                //    if (datas == null)
                //        datas = new List<V_HIS_MATERIAL_BEAN_1>();
                //    datas.Add(dataNew);
                //    datas = datas.OrderBy(p => p.MATERIAL_TYPE_CODE).ToList();
                //    gridControlMaterialReuse.BeginUpdate();
                //    gridControlMaterialReuse.DataSource = datas;
                //    gridControlMaterialReuse.EndUpdate();
                //  }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearchReuse_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtSearchReuse.Text) && this._vMaterialBeans != null)
                {
                    string str = txtSearchReuse.Text.Trim().ToUpper();
                    List<V_HIS_MATERIAL_BEAN_1> _dataSearchs = this._vMaterialBeans.Where(o =>
                        o.MATERIAL_TYPE_CODE.ToUpper().Contains(str)
                        || o.MATERIAL_TYPE_NAME.ToUpper().Contains(str)
                        || o.SERIAL_NUMBER.ToUpper().Contains(str)
                        ).ToList();
                    gridControlMaterialReuse.DataSource = _dataSearchs;
                }
                else
                {
                    gridControlMaterialReuse.DataSource = this._vMaterialBeans;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearchReuse_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridViewMaterialReuse.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinEditDiscountPercent_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                SetDataToLabels_MoneyCalculated();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinEditDiscountPercent_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    xtraTabControlMain.SelectedTabPage.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnExpFromExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (btnExpFromExcel.Enabled)
                {
                    btnExpFromExcel_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
