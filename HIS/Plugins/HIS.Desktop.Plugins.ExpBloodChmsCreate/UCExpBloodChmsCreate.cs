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
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using MOS.EFMODEL.DataModels;
using HIS.UC.ExpMestMaterialGrid.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using HIS.Desktop.Plugins.ExpBloodChmsCreate.Validation;
using MOS.SDO;
using HIS.Desktop.Plugins.ExpBloodChmsCreate.ADO;
using DevExpress.Utils.Menu;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.ExpBloodChmsCreate
{
    public partial class UCExpBloodChmsCreate : UserControl
    {
        List<V_HIS_MEDI_STOCK> listExpMediStock = new List<V_HIS_MEDI_STOCK>();
        List<V_HIS_MEDI_STOCK> listImpMediStock = new List<V_HIS_MEDI_STOCK>();
        Dictionary<long, HisBloodTypeInStockSDO> dicExpBltyInStock = new Dictionary<long, HisBloodTypeInStockSDO>();
        Dictionary<long, MediMateTypeADO> dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();
        MediMateTypeADO currentMediMate = null;
        HisChmsExpMestResultSDO resultSdo = null;
        List<HisBloodTypeInStockSDO> listBloodTypeInStock;
        bool isUpdate = false;
        bool isSupplement = false;
        long roomId;
        long roomTypeId;
        int numOrder = 1;
        int positionHandleControl = -1;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public UCExpBloodChmsCreate(Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.roomTypeId = moduleData.RoomTypeId;
                this.roomId = moduleData.RoomId;
                this.currentModule = moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Load combo

        private void InitComboBloodRH()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboChooseRH, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBloodABO()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboChooseABO, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRespositoryBloodRH()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboRH, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRespositoryBloodABO()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboABO, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void LoadDataToGridBloodType(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK mediStock)
        {
            try
            {
                listBloodTypeInStock = new List<HisBloodTypeInStockSDO>();
                dicExpBltyInStock = new Dictionary<long, HisBloodTypeInStockSDO>();
                if (mediStock != null)
                {
                    HisBloodTypeStockViewFilter bltyFilter = new HisBloodTypeStockViewFilter();
                    bltyFilter.MEDI_STOCK_ID = mediStock.ID;
                    bltyFilter.IS_LEAF = true;
                    listBloodTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisBloodTypeInStockSDO>>("api/HisBloodType/GetInStockBloodType", ApiConsumers.MosConsumer, bltyFilter, null);
                }

                if (listBloodTypeInStock != null)
                {
                    foreach (var item in listBloodTypeInStock)
                    {
                        dicExpBltyInStock[item.Id] = item;
                    }
                }

                gridControlBloodType__BloodPage.BeginUpdate();
                gridControlBloodType__BloodPage.DataSource = listBloodTypeInStock;
                gridControlBloodType__BloodPage.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void FillDataToGridExpMest()
        {
            try
            {
                List<V_HIS_EXP_MEST_BLTY> listBlood = null;

                if (this.resultSdo != null)
                {
                    listBlood = this.resultSdo.ExpBlties;
                }
                if (listBlood != null && listBlood.Count > 0)
                {
                    listBlood = listBlood.OrderBy(o => o.ID).ToList();
                }
                gridControlBloodTypeList.BeginUpdate();
                gridControlBloodTypeList.DataSource = listBlood;
                gridControlBloodTypeList.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpBloodChmsCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                //LoadKeyUCLanguage();
                SetCaptionByLanguageKey();
                ValidationSingleControl(cboChooseABO);
                ValidationSingleControl(cboChooseRH);
                ValidControl();
                InitComboBloodABO();
                InitComboBloodRH();
                InitComboRespositoryBloodABO();
                InitComboRespositoryBloodRH();
                LoadDataToComboImpMediStock();
                LoadDataToComboExpMediStock();
                FillDataToCboMediStock(true);
                SetDefaultImpMediStock();
                GenerateMenuPrint();
                cboExpMediStock.Focus();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExpBloodChmsCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.ExpBloodChmsCreate.UCExpBloodChmsCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChooseRH.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.cboChooseRH.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChooseABO.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.cboChooseABO.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.ToolTip = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.ToolTip = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ColumnVolume.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.ColumnVolume.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExpMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.cboExpMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboImpMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.cboImpMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestChmsDetail_Delete.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn_ExpMestChmsDetail_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestChmsDetail_MediMateTypeName.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn_ExpMestChmsDetail_MediMateTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestChmsDetail_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn_ExpMestChmsDetail_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestChmsDetail_Amount.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn_ExpMestChmsDetail_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboABO.NullText = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.cboABO.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRH.NullText = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.cboRH.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutExpAmount.Text = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.layoutExpAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutNote.Text = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.layoutNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutImpMediStock.Text = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.layoutImpMediStock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.layoutDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutExpMediStock.Text = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.layoutExpMediStock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCExpBloodChmsCreate.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetValueControlDetail()
        {
            try
            {
                spinExpAmount.Value = 0;
                txtNote.Text = "";
                if (this.currentMediMate != null)
                {
                    btnAdd.Enabled = true;
                    spinExpAmount.Properties.MaxValue = this.currentMediMate.AVAILABLE_AMOUNT ?? 0;
                    spinExpAmount.Focus();
                    spinExpAmount.SelectAll();
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

        private void ResetGridControlDetail()
        {
            try
            {
                dicMediMateAdo.Clear();
                gridControlExpBloodChmsDetail.BeginUpdate();
                gridControlExpBloodChmsDetail.DataSource = null;
                gridControlExpBloodChmsDetail.EndUpdate();
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
                this.isSupplement = false;
                dicMediMateAdo.Clear();
                this.currentMediMate = null;
                this.resultSdo = null;
                isUpdate = false;
                ResetValueControlDetail();
                ResetGridControlDetail();
                SetEnableCboMediStockAndButton(true);
                //ddBtnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboImpMediStock()
        {
            try
            {

                cboImpMediStock.Properties.DataSource = null;
                cboImpMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboImpMediStock.Properties.ValueMember = "ID";
                cboImpMediStock.Properties.ForceInitialize();
                cboImpMediStock.Properties.Columns.Clear();
                cboImpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 50));
                cboImpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboImpMediStock.Properties.ShowHeader = false;
                cboImpMediStock.Properties.ImmediatePopup = true;
                cboImpMediStock.Properties.DropDownRows = 10;
                cboImpMediStock.Properties.PopupWidth = 250;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboExpMediStock()
        {
            try
            {
                cboExpMediStock.Properties.DataSource = null;
                cboExpMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboExpMediStock.Properties.ValueMember = "ID";
                cboExpMediStock.Properties.ForceInitialize();
                cboExpMediStock.Properties.Columns.Clear();
                cboExpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 50));
                cboExpMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboExpMediStock.Properties.ShowHeader = false;
                cboExpMediStock.Properties.ImmediatePopup = true;
                cboExpMediStock.Properties.DropDownRows = 10;
                cboExpMediStock.Properties.PopupWidth = 250;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultImpMediStock()
        {
            try
            {
                var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_TYPE_ID == this.roomTypeId && o.ROOM_ID == this.roomId);
                if (mediStock != null)
                {
                    cboImpMediStock.EditValue = mediStock.ID;
                }
                //FillDataToCboExpMestByImpMest(mediStock);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToCboMediStock(bool isEditImp)
        {
            try
            {
                List<V_HIS_MEDI_STOCK> hisImpMest = new List<V_HIS_MEDI_STOCK>();
                List<V_HIS_MEDI_STOCK> hisExpMest = new List<V_HIS_MEDI_STOCK>();
                if (cboImpMediStock.EditValue != null)
                {
                    var impMest = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMediStock.EditValue));
                    if (impMest != null)
                    {
                        var listMestId = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == impMest.ROOM_ID).Select(s => s.MEDI_STOCK_ID).ToList();
                        if (listMestId != null && listMestId.Count > 0)
                        {
                            hisExpMest = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => listMestId.Contains(o.ID)).ToList();
                        }
                    }
                }

                if (cboExpMediStock.EditValue != null)
                {
                    var expMest = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue));
                    if (expMest != null)
                    {
                        var listRoomId = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.MEDI_STOCK_ID == expMest.ID).Select(s => s.ROOM_ID).ToList();
                        if (listRoomId != null && listRoomId.Count > 0)
                        {
                            hisImpMest = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => listRoomId.Contains(o.ROOM_ID)).ToList();
                        }
                    }
                }

                if (cboExpMediStock.EditValue != null && cboImpMediStock.EditValue != null)
                {
                    if (isEditImp)
                    {
                        if (hisExpMest == null || hisExpMest.Count <= 0 || hisExpMest.FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue)) == null)
                        {
                            cboExpMediStock.EditValue = null;
                            listExpMediStock = hisExpMest;
                        }
                    }
                    else
                    {
                        if (hisImpMest == null || hisImpMest.Count <= 0 || hisImpMest.FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMediStock.EditValue)) == null)
                        {
                            cboImpMediStock.EditValue = null;
                            listImpMediStock = hisImpMest;
                        }
                    }
                }
                else if (cboExpMediStock.EditValue == null && cboImpMediStock.EditValue == null)
                {
                    listImpMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                    listExpMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                }
                else if (cboImpMediStock.EditValue != null)
                {
                    listExpMediStock = hisExpMest;
                }
                else
                {
                    listImpMediStock = hisImpMest;
                }

                cboImpMediStock.Properties.DataSource = listImpMediStock;
                cboExpMediStock.Properties.DataSource = listExpMediStock;
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
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__PRINT_MENU__ITEM_PHIEU_XUAT_CHUYEN_KHO", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPrintPhieuXuatChuyenKho)));
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__PRINT_MENU__ITEM_PHIEU_THUOC_GAY_NGHIEN_HUONG_THAN", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPrintPhieuGayNghienHuongThan)));
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__PRINT_MENU__ITEM_PHIEU_KHONG_PHAI_THUOC_GAY_NGHIEN_HUONG_THAN", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPrintPhieuKhongPhaiGayNghienHuongThan)));

                //ddBtnPrint.DropDownControl = menu;
                //ddBtnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetEnableCboMediStockAndButton(bool enable)
        {
            try
            {
                cboExpMediStock.Enabled = enable;
                cboImpMediStock.Enabled = enable;
                txtExpMediStock.Enabled = enable;
                txtImpMediStock.Enabled = enable;
                btnPrint.Enabled = !enable;
                btnSave.Enabled = enable;
                //btnUpdate.Enabled = !enable;
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
                ValidControlImpMediStock();
                ValidControlExpMediStock();
                ValidControlExpAmount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlImpMediStock()
        {
            try
            {
                ImpMediStockValidationRule impMestRule = new ImpMediStockValidationRule();
                impMestRule.cboImpMediStock = cboImpMediStock;
                dxValidationProvider2.SetValidationRule(cboImpMediStock, impMestRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider2.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void ValidControlExpMediStock()
        {
            try
            {
                ExpMediStockValidationRule expMestRule = new ExpMediStockValidationRule();
                expMestRule.cboExpMediStock = cboExpMediStock;
                dxValidationProvider2.SetValidationRule(cboExpMediStock, expMestRule);
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
                ExpAmountValidationRule expAmountRule = new ExpAmountValidationRule();
                expAmountRule.spinExpAmount = spinExpAmount;
                dxValidationProvider2.SetValidationRule(spinExpAmount, expAmountRule);
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

        public void BtnSave()
        {
            try
            {
                gridViewExpBloodChmsDetail.PostEditor();
                btnSave_Click(null, null);
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

        public void BtnUpdate()
        {
            try
            {
                gridViewExpBloodChmsDetail.PostEditor();
                btnUpdate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string keyword = txtSearch.Text.Trim();
                    BindingList<HisBloodTypeInStockSDO> listResult = null;
                    if (!String.IsNullOrEmpty(keyword.Trim()))
                    {
                        List<HisBloodTypeInStockSDO> rearchResult = new List<HisBloodTypeInStockSDO>();

                        rearchResult = listBloodTypeInStock.Where(o =>
                                                        ((o.BloodTypeCode ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                        || (o.BloodTypeName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                        || (o.Amount.ToString() ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                        || (o.Volume.ToString() ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower()))
                                                        ).Distinct().ToList();

                        listResult = new BindingList<HisBloodTypeInStockSDO>(rearchResult);
                    }
                    else
                    {
                        listResult = new BindingList<HisBloodTypeInStockSDO>(listBloodTypeInStock);
                    }
                    gridControlBloodType__BloodPage.DataSource = listResult;
                    //trvService.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewBloodType__BloodPage_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var data = (HisBloodTypeInStockSDO)gridViewBloodType__BloodPage.GetFocusedRow();
                if (data != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(data);
                }
                ResetValueControlDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                onClickPrintPhieuXuatChuyenKho(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
