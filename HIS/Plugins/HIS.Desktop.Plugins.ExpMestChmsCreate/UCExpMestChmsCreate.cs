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
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using HIS.Desktop.Plugins.ExpMestChmsCreate.Validation;
using MOS.SDO;
using HIS.Desktop.Plugins.ExpMestChmsCreate.ADO;
using DevExpress.Utils.Menu;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.Desktop.Plugins.ExpMestChmsCreate
{
    using DevExpress.XtraGrid;
    using DevExpress.XtraGrid.Views.Grid;
	using HIS.Desktop.LocalStorage.HisConfig;
	using HIS.Desktop.Utilities.Extensions;
    public partial class UCExpMestChmsCreate : HIS.Desktop.Utility.UserControlBase
    {

        List<V_HIS_MEDI_STOCK> listCurrentMediStock = new List<V_HIS_MEDI_STOCK>();
        V_HIS_MEDI_STOCK currentMediStock = null;
        V_HIS_MEDI_STOCK stock = null;
        V_HIS_MEDI_STOCK mestRoom = null;

        List<V_HIS_MEDI_STOCK> listExpMediStock = new List<V_HIS_MEDI_STOCK>();
        List<V_HIS_MEDI_STOCK> listImpMediStock = new List<V_HIS_MEDI_STOCK>();

        Dictionary<long, MediMateTypeADO> dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();

        MediMateTypeADO currentMediMate = null;
        List<MediMateTypeADO> currentMediMate_ = new List<MediMateTypeADO>();
        HisExpMestResultSDO resultSdo = null;
        HisExpMestResultSDO resultSdo_1 = new HisExpMestResultSDO();
        List<HisExpMestResultSDO> resultSdo_ = null;
        List<long> medicineTypeIds;
        List<long> materialTypeIds;
        bool isUpdate = false;

        bool isSupplement = false;

        long roomId;
        long roomTypeId;

        int positionHandleControl = -1;
        string KeyOddPolicyOption { get; set; }

        MOS.EFMODEL.DataModels.V_HIS_EXP_MEST _ExpMestChmsUpdate;
        //mau
        List<HisBloodTypeInStockADO> listBloodTypeInStock { get; set; }
        List<HisMedicineInStockADO> listMediInStock { get; set; }
        List<HisMaterialInStockADO> listMateInStock { get; set; }

        public List<HIS_CONFIG> lstConfig;

        bool IsReasonRequired { get; set; }
        public UCExpMestChmsCreate(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.roomTypeId = module.RoomTypeId;
                this.roomId = module.RoomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCExpMestChmsCreate(Inventec.Desktop.Common.Modules.Module _moduleData, MOS.EFMODEL.DataModels.V_HIS_EXP_MEST _expMestChms)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.roomTypeId = _moduleData.RoomTypeId;
                this.roomId = _moduleData.RoomId;
                this._ExpMestChmsUpdate = _expMestChms;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestChmsCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                KeyOddPolicyOption = HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__ODDPOLICY);
                IsReasonRequired = HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__IS_REASON_REQUIRED) == "1";
                LoadKeyUCLanguage();
                ValidControl();
                LoadDataToComboImpMediStock();
                LoadDataToComboReasonRequired();
                // LoadDataToComboExpMediStock();
                LoadKhoXuat();
                LoadListCurrentImpMediStock();
                radioImport.Checked = true;
                LoadDataToCboMediStock();
                InitComboBloodABO();
                InitComboBloodRH();
                InitComboRespositoryBloodABO();
                InitComboRespositoryBloodRH();
                txtExpMediStock.Focus();
                gridColumnMedicine_Chon.Image = imageCollectionExpMest.Images[1];
                gridColumnMaterial_Chon.Image = imageCollectionExpMest.Images[1];
                GetConfig();
                ValidControlMaxLength1();
                ValidControlMaxLength2();
                if (this._ExpMestChmsUpdate != null)
                {
                    isUpdate = true;
                    mestRoom = listExpMediStock.FirstOrDefault(o => o.ID == this._ExpMestChmsUpdate.MEDI_STOCK_ID);
                    cboExpMediStock.EditValue = this._ExpMestChmsUpdate.MEDI_STOCK_ID;
                    if (mestRoom != null)
                    {
                        txtExpMediStock.Properties.Buttons[1].Visible = true;
                        txtExpMediStock.Text = mestRoom.MEDI_STOCK_NAME;
                    }
                    else
                    {
                        txtExpMediStock.Properties.Buttons[1].Visible = false;
                        txtExpMediStock.Text = "";
                    }

                    ResetGridControlDetail();

                    //LoadDataToTreeList(mestRoom);
                    FillDataToTrees();

                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetConfig()
        {
            try
            {
                lstConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE")).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDafaultPlanningExport(bool isPlanning)
        {
            try
            {
                //gridViewMaterial.Columns.Clear();
                //gridViewMedicine.Columns.Clear();

                gridColumn_Medicine_MedicineTypeCode.VisibleIndex = 0;
                gridColumn_Medicine_MedicineTypeCode.Visible = true;
                gridColumn_Medicine_MedicineTypeName.VisibleIndex = 1;
                gridColumn_Medicine_MedicineTypeName.Visible = true;
                gridColumn_Medicine_ServiceUnitName.VisibleIndex = 2;
                gridColumn_Medicine_ServiceUnitName.Visible = true;
                gridColumn_Medicine_Concentra.VisibleIndex = 3;
                gridColumn_Medicine_Concentra.Visible = true;
                gridColumn_MedicineNationalName.VisibleIndex = 4;
                gridColumn_MedicineNationalName.Visible = true;
                gridColumnMedicine_ManufactureName.VisibleIndex = 5;
                gridColumnMedicine_ManufactureName.Visible = true;
                gridColumn_Medicine_KhaDung.VisibleIndex = 6;
                gridColumn_Medicine_KhaDung.Visible = true;
                gridColumnMedicine_TonKhoNhan.Visible = true;
                gridColumnMedicine_TonKhoNhan.VisibleIndex = 7;
                gridColumnMedicine_SLDaXuat.Visible = true;
                gridColumnMedicine_SLDaXuat.VisibleIndex = 8;
                gridColumnMedicine_SLDat.Visible = true;
                gridColumnMedicine_SLDat.VisibleIndex = 9;
                gridColumnMedicine_Xoa.Visible = true;
                gridColumnMedicine_Xoa.VisibleIndex = 10;
                gridColumnMedicine_Chon.Visible = true;
                gridColumnMedicine_Chon.VisibleIndex = 11;
                gridColumnMedicine_HoatChat.VisibleIndex = 12;
                gridColumnMedicine_HoatChat.Visible = true;
                gridColumnMedicine_RegisterNumber.VisibleIndex = 13;
                gridColumnMedicine_RegisterNumber.Visible = true;
                gridColumnMedicine_HanSuDung.Visible = true;
                gridColumnMedicine_HanSuDung.VisibleIndex = 14;
                gridColumnMedicine_SoLo.Visible = true;
                gridColumnMedicine_SoLo.VisibleIndex = 15;

                // vat tu
                gridColumnMaterial_TonKhoNhan.Visible = true;
                gridColumnMaterial_TonKhoNhan.VisibleIndex = 7;
                gridColumnMaterial_Chon.Visible = true;
                gridColumnMaterial_Chon.VisibleIndex = 11;
                gridColumnMaterial_HanSuDung.Visible = true;
                gridColumnMaterial_HanSuDung.VisibleIndex = 12;
                gridColumnMaterial_SLDat.Visible = true;
                gridColumnMaterial_SLDat.VisibleIndex = 9;
                gridColumnMaterial_SLDaXuat.Visible = true;
                gridColumnMaterial_SLDaXuat.VisibleIndex = 8;
                gridColumnMaterial_SoLo.Visible = true;
                gridColumnMaterial_SoLo.VisibleIndex = 11;
                gridColumnMaterial_Xoa.Visible = true;
                gridColumnMaterial_Xoa.VisibleIndex = 10;
                gridColumn_Material_MaterialTypeCode.VisibleIndex = 0;
                gridColumn_Material_MaterialTypeCode.Visible = true;
                gridColumn_Material_MaterialTypeName.VisibleIndex = 1;
                gridColumn_Material_MaterialTypeName.Visible = true;
                gridColumn_Material_ServiceUnitName.VisibleIndex = 2;
                gridColumn_Material_ServiceUnitName.Visible = true;
                gridColumn_Material_KhaDung.VisibleIndex = 6;
                gridColumn_Material_KhaDung.Visible = true;
                gridColumn_Material_Concentra.VisibleIndex = 3;
                gridColumn_Material_Concentra.Visible = true;
                gridColumnMaterial_ManufacturerName.VisibleIndex = 5;
                gridColumnMaterial_ManufacturerName.Visible = true;
                gridColumnMaterial_NationalName.VisibleIndex = 4;
                gridColumnMaterial_NationalName.Visible = true;
                gridColumnMaterial_RegisterNumber.Visible = true;
                gridColumnMaterial_RegisterNumber.VisibleIndex = 13;

                if (isPlanning)
                {
                    // control
                    chkHienThiLo.CheckState = CheckState.Unchecked;
                    chkHienThiLo.Enabled = false;
                    spinExpAmount.Enabled = false;
                    spinExpAmount.EditValue = null;
                    txtNote.Text = "";
                    txtNote.Enabled = false;
                    dtFromTime.Enabled = true;
                    dtFromTime.DateTime = DateTime.Now.AddDays(-7);
                    dtToTime.Enabled = true;
                    dtToTime.DateTime = DateTime.Now;
                    btnSearch.Enabled = true;
                    btnAdd.Enabled = true;

                    // grid thuoc
                    //gridViewMedicine.Columns.Add(gridColumn_Medicine_MedicineTypeCode);
                    //gridViewMedicine.Columns.Add(gridColumn_Medicine_MedicineTypeName);
                    //gridViewMedicine.Columns.Add(gridColumn_Medicine_ServiceUnitName);
                    //gridViewMedicine.Columns.Add(gridColumn_Medicine_Concentra);
                    //gridViewMedicine.Columns.Add(gridColumn_MedicineNationalName);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_ManufactureName);
                    //gridViewMedicine.Columns.Add(gridColumn_Medicine_KhaDung);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_TonKhoNhan);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_SLDaXuat);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_SLDat);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_Xoa);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_Chon);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_HoatChat);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_RegisterNumber);


                    gridColumn_Medicine_MedicineTypeCode.VisibleIndex = 0;
                    gridColumn_Medicine_MedicineTypeCode.Visible = true;
                    gridColumn_Medicine_MedicineTypeName.VisibleIndex = 1;
                    gridColumn_Medicine_MedicineTypeName.Visible = true;
                    gridColumn_Medicine_ServiceUnitName.VisibleIndex = 2;
                    gridColumn_Medicine_ServiceUnitName.Visible = true;
                    gridColumn_Medicine_Concentra.VisibleIndex = 3;
                    gridColumn_Medicine_Concentra.Visible = true;
                    gridColumn_MedicineNationalName.VisibleIndex = 4;
                    gridColumn_MedicineNationalName.Visible = true;
                    gridColumnMedicine_ManufactureName.VisibleIndex = 5;
                    gridColumnMedicine_ManufactureName.Visible = true;
                    gridColumn_Medicine_KhaDung.VisibleIndex = 6;
                    gridColumn_Medicine_KhaDung.Visible = true;
                    gridColumnMedicine_TonKhoNhan.Visible = true;
                    gridColumnMedicine_TonKhoNhan.VisibleIndex = 7;
                    gridColumnMedicine_SLDaXuat.Visible = true;
                    gridColumnMedicine_SLDaXuat.VisibleIndex = 8;
                    gridColumnMedicine_SLDat.Visible = true;
                    gridColumnMedicine_SLDat.VisibleIndex = 9;
                    gridColumnMedicine_Xoa.Visible = true;
                    gridColumnMedicine_Xoa.VisibleIndex = 10;
                    gridColumnMedicine_Chon.Visible = true;
                    gridColumnMedicine_Chon.VisibleIndex = 11;
                    gridColumnMedicine_HoatChat.VisibleIndex = 12;
                    gridColumnMedicine_HoatChat.Visible = true;
                    gridColumnMedicine_RegisterNumber.VisibleIndex = 13;
                    gridColumnMedicine_RegisterNumber.Visible = true;
                    gridColumnMedicine_HanSuDung.Visible = true;
                    gridColumnMedicine_HanSuDung.VisibleIndex = 14;
                    gridColumnMedicine_SoLo.Visible = false;
                    gridColumnMedicine_SoLo.VisibleIndex = -1;

                    // grid vat tu
                    //gridViewMaterial.Columns.Add(gridColumn_Material_MaterialTypeCode);
                    //gridViewMaterial.Columns.Add(gridColumn_Material_MaterialTypeName);
                    //gridViewMaterial.Columns.Add(gridColumn_Material_ServiceUnitName);
                    //gridViewMaterial.Columns.Add(gridColumn_Material_Concentra);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_NationalName);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_ManufacturerName);
                    //gridViewMaterial.Columns.Add(gridColumn_Material_KhaDung);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_TonKhoNhan);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_SLDaXuat);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_SLDat);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_Xoa);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_Chon);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_RegisterNumber);


                    gridColumnMaterial_TonKhoNhan.Visible = true;
                    gridColumnMaterial_Chon.Visible = true;
                    gridColumnMaterial_HanSuDung.Visible = true;
                    gridColumnMaterial_SLDat.Visible = true;
                    gridColumnMaterial_SLDaXuat.Visible = true;
                    gridColumnMaterial_SoLo.Visible = false;
                    gridColumnMaterial_Xoa.Visible = true;
                    gridColumn_Material_MaterialTypeCode.Visible = true;
                    gridColumn_Material_MaterialTypeName.Visible = true;
                    gridColumn_Material_ServiceUnitName.Visible = true;
                    gridColumn_Material_KhaDung.Visible = true;
                    gridColumn_Material_Concentra.Visible = true;
                    gridColumnMaterial_ManufacturerName.Visible = true;
                    gridColumnMaterial_NationalName.Visible = true;
                    gridColumnMaterial_RegisterNumber.Visible = true;



                    // tab
                    xtraTabPageBlood.PageVisible = false;
                }
                else
                {
                    // control
                    chkHienThiLo.Enabled = true;
                    spinExpAmount.Enabled = true;
                    spinExpAmount.EditValue = null;
                    txtNote.Text = "";
                    txtNote.Enabled = true;
                    dtFromTime.Enabled = false;
                    dtFromTime.EditValue = null;
                    dtToTime.Enabled = false;
                    dtToTime.EditValue = null;
                    btnSearch.Enabled = false;
                    btnAdd.Enabled = false;

                    // grid thuoc
                    // set lại VisibleIndex do số cột bị giảm
                    gridColumn_Medicine_MedicineTypeCode.VisibleIndex = 0;
                    gridColumn_Medicine_MedicineTypeName.VisibleIndex = 1;
                    gridColumn_Medicine_ServiceUnitName.VisibleIndex = 2;
                    gridColumn_Medicine_KhaDung.VisibleIndex = 3;
                    gridColumnMedicine_TonKhoNhapExport.VisibleIndex = 4;
                    gridColumn_Medicine_Concentra.VisibleIndex = 5;
                    gridColumnMedicine_SoLo.VisibleIndex = 6;
                    gridColumnMedicine_HanSuDung.VisibleIndex = 7;
                    gridColumn_MedicineNationalName.VisibleIndex = 8;
                    gridColumnMedicine_ManufactureName.VisibleIndex = 9;
                    gridColumnMedicine_HoatChat.VisibleIndex = 10;
                    gridColumnMaterial_RegisterNumber.VisibleIndex = 11;

                    //gridViewMedicine.Columns.Add(gridColumn_Medicine_MedicineTypeCode);
                    //gridViewMedicine.Columns.Add(gridColumn_Medicine_MedicineTypeName);
                    //gridViewMedicine.Columns.Add(gridColumn_Medicine_ServiceUnitName);
                    //gridViewMedicine.Columns.Add(gridColumn_Medicine_KhaDung);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_TonKhoNhapExport);//#29784 
                    //gridViewMedicine.Columns.Add(gridColumn_Medicine_Concentra);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_SoLo);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_HanSuDung);
                    //gridViewMedicine.Columns.Add(gridColumn_MedicineNationalName);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_ManufactureName);
                    //gridViewMedicine.Columns.Add(gridColumnMedicine_HoatChat);
                    //gridViewMedicine.Columns.Add(gridColumnMaterial_RegisterNumber);

                    gridColumn_Medicine_MedicineTypeCode.Visible = true;
                    gridColumn_Medicine_MedicineTypeName.Visible = true;
                    gridColumn_Medicine_ServiceUnitName.Visible = true;
                    gridColumn_Medicine_Concentra.Visible = true;
                    gridColumn_MedicineNationalName.Visible = true;
                    gridColumnMedicine_ManufactureName.Visible = true;
                    gridColumn_Medicine_KhaDung.Visible = true;
                    gridColumnMedicine_TonKhoNhan.Visible = false;
                    gridColumnMedicine_SLDaXuat.Visible = false;
                    gridColumnMedicine_SLDat.Visible = false;
                    gridColumnMedicine_Xoa.Visible = false;
                    gridColumnMedicine_Chon.Visible = false;
                    gridColumnMedicine_HoatChat.Visible = true;
                    gridColumnMedicine_RegisterNumber.Visible = true;
                    gridColumnMedicine_HanSuDung.Visible = true;
                    gridColumnMedicine_SoLo.Visible = true;



                    // grid vat tu
                    // set lại VisibleIndex do số cột bị giảm
                    gridColumn_Material_MaterialTypeCode.VisibleIndex = 0;
                    gridColumn_Material_MaterialTypeName.VisibleIndex = 1;
                    gridColumn_Material_ServiceUnitName.VisibleIndex = 2;
                    gridColumn_Material_KhaDung.VisibleIndex = 3;
                    gridColumnMaterial_TonKhoNhap.VisibleIndex = 4;
                    gridColumn_Material_Concentra.VisibleIndex = 5;
                    gridColumnMaterial_SoLo.VisibleIndex = 6;
                    gridColumnMaterial_HanSuDung.VisibleIndex = 7;
                    gridColumnMaterial_NationalName.VisibleIndex = 8;
                    gridColumnMaterial_ManufacturerName.VisibleIndex = 9;
                    gridColumnMaterial_RegisterNumber.VisibleIndex = 10;


                    gridColumnMaterial_TonKhoNhan.Visible = false;
                    gridColumnMaterial_Chon.Visible = false;
                    gridColumnMaterial_HanSuDung.Visible = true;
                    gridColumnMaterial_SLDat.Visible = false;
                    gridColumnMaterial_SLDaXuat.Visible = false;
                    gridColumnMaterial_SoLo.Visible = true;
                    gridColumnMaterial_Xoa.Visible = false;
                    gridColumn_Material_MaterialTypeCode.Visible = true;
                    gridColumn_Material_MaterialTypeName.Visible = true;
                    gridColumn_Material_ServiceUnitName.Visible = true;
                    gridColumn_Material_KhaDung.Visible = true;
                    gridColumn_Material_Concentra.Visible = true;
                    gridColumnMaterial_ManufacturerName.Visible = true;
                    gridColumnMaterial_NationalName.Visible = true;
                    gridColumnMaterial_RegisterNumber.Visible = true;


                    //gridViewMaterial.Columns.Add(gridColumn_Material_MaterialTypeCode);
                    //gridViewMaterial.Columns.Add(gridColumn_Material_MaterialTypeName);
                    //gridViewMaterial.Columns.Add(gridColumn_Material_ServiceUnitName);
                    //gridViewMaterial.Columns.Add(gridColumn_Material_KhaDung);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_TonKhoNhap);//#29784 
                    //gridViewMaterial.Columns.Add(gridColumn_Material_Concentra);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_SoLo);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_HanSuDung);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_NationalName);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_ManufacturerName);
                    //gridViewMaterial.Columns.Add(gridColumnMaterial_RegisterNumber);

                    // tab
                    xtraTabPageBlood.PageVisible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadListCurrentImpMediStock()
        {
            try
            {
                listCurrentMediStock = new List<V_HIS_MEDI_STOCK>();
                var listUserRoom = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__KHO).ToList();
                if (listUserRoom != null && listUserRoom.Count > 0)
                {
                    var roomIds = listUserRoom.Select(s => s.ROOM_ID).ToList();
                    listCurrentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => roomIds.Contains(o.ROOM_ID) && o.IS_ACTIVE == 1).ToList();
                }
                currentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_TYPE_ID == this.roomTypeId && o.ROOM_ID == this.roomId && o.IS_ACTIVE == 1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTrees()
        {
            try
            {
                listMateInStock = null;
                listMediInStock = null;
                listBloodTypeInStock = null;
                if (chkPlanningExport.CheckState == CheckState.Unchecked)// mestRoom != null &&
                {
                    List<Action> methods = new List<Action>();
                    methods.Add(LoadMedicineTypeFromStock);
                    methods.Add(LoadMaterialTypeFromStock);
                    methods.Add(LoadBloodTypeFromStock);
                    Inventec.Common.ThreadCustom.ThreadCustomManager.MultipleThreadWithJoin(methods);
                }
                else if (cboImpMediStock.EditValue != null && mestRoom != null && chkPlanningExport.CheckState == CheckState.Checked)
                {
                    List<Action> methods = new List<Action>();
                    methods.Add(GetDataMedicinePlanning);
                    methods.Add(GetDataMaterialPlanning);
                    Inventec.Common.ThreadCustom.ThreadCustomManager.MultipleThreadWithJoin(methods);
                }
                gridControlBloodType__BloodPage.BeginUpdate();
                gridControlBloodType__BloodPage.DataSource = listBloodTypeInStock;
                gridControlBloodType__BloodPage.EndUpdate();
                gridViewBloodType__BloodPage.ExpandAllGroups();

                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = listMediInStock;
                gridControlMedicine.EndUpdate();
                gridViewMedicine.ExpandAllGroups();

                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = listMateInStock;
                gridControlMaterial.EndUpdate();
                gridViewMaterial.ExpandAllGroups();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToTreeList(V_HIS_MEDI_STOCK mestRoom) // ko dung
        {
            try
            {
                List<HisMedicineTypeInStockSDO> listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                List<HisMaterialTypeInStockSDO> listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                listBloodTypeInStock = new List<HisBloodTypeInStockADO>();



                #region --- Code Cu ----
                if (mestRoom != null)
                {
                    HisMedicineTypeStockViewFilter mediFilter = new HisMedicineTypeStockViewFilter();
                    mediFilter.MEDI_STOCK_ID = mestRoom.ID;
                    mediFilter.IS_AVAILABLE = true;
                    mediFilter.IS_ACTIVE = true;
                    if (stock.IS_GOODS_RESTRICT != null && stock.IS_GOODS_RESTRICT != 0 && medicineTypeIds != null && medicineTypeIds.Count > 0)
                    {
                        mediFilter.MEDICINE_TYPE_IDs = medicineTypeIds;
                    }
                    mediFilter.IS_LEAF = true;
                    listMediTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK, ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (listMediTypeInStock != null && listMediTypeInStock.Count > 0)
                    {
                        if (mestRoom.IS_BUSINESS == 1)
                        {
                            var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                            if (dataMediTypes != null && dataMediTypes.Count > 0)
                            {
                                listMediTypeInStock = listMediTypeInStock.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                            }
                        }
                        else
                        {
                            var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                            if (dataMediTypes != null && dataMediTypes.Count > 0)
                            {
                                listMediTypeInStock = listMediTypeInStock.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                listMediTypeInStock = new List<HisMedicineTypeInStockSDO>();
                            }
                        }
                    }

                    HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                    mateFilter.MEDI_STOCK_ID = mestRoom.ID;
                    mateFilter.IS_AVAILABLE = true;
                    mateFilter.IS_ACTIVE = true;
                    if (stock.IS_GOODS_RESTRICT != null && stock.IS_GOODS_RESTRICT != 0 && materialTypeIds != null && materialTypeIds.Count > 0)
                    {
                        mateFilter.MATERIAL_TYPE_IDs = materialTypeIds;
                    }
                    mateFilter.IS_LEAF = true;
                    listMateTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK, ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    if (listMateTypeInStock != null && listMateTypeInStock.Count > 0)
                    {
                        if (mestRoom.IS_BUSINESS == 1)
                        {
                            var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                            if (dataMateTypes != null && dataMateTypes.Count > 0)
                            {
                                listMateTypeInStock = listMateTypeInStock.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                            }
                        }
                        else
                        {
                            var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                            if (dataMateTypes != null && dataMateTypes.Count > 0)
                            {
                                listMateTypeInStock = listMateTypeInStock.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.Id)).ToList();
                            }
                            else
                            {
                                listMateTypeInStock = new List<HisMaterialTypeInStockSDO>();
                            }
                        }
                    }
                    //Mau
                    listBloodTypeInStock = new List<HisBloodTypeInStockADO>();
                    HisBloodTypeStockViewFilter bltyFilter = new HisBloodTypeStockViewFilter();
                    bltyFilter.MEDI_STOCK_ID = mestRoom.ID;
                    bltyFilter.IS_LEAF = true;
                    bltyFilter.IS_AVAILABLE = true;
                    bltyFilter.IS_ACTIVE = 1;

                    var listBloodTypeInStockSDO = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisBloodTypeInStockSDO>>("api/HisBloodType/GetInStockBloodType", ApiConsumers.MosConsumer, bltyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    listBloodTypeInStock = new List<HisBloodTypeInStockADO>();
                    if (listBloodTypeInStockSDO != null && listBloodTypeInStockSDO.Count() > 0)
                    {
                        foreach (var item in listBloodTypeInStockSDO)
                        {
                            HisBloodTypeInStockADO ado = new HisBloodTypeInStockADO(item);
                            listBloodTypeInStock.Add(ado);
                        }
                    }


                }
                #endregion

                //if (listMediTypeInStock != null)
                //{
                //    foreach (var item in listMediTypeInStock)
                //    {
                //        dicExpMetyInStock[item.Id] = item;
                //    }
                //}
                //if (listMateTypeInStock != null)
                //{
                //    foreach (var item in listMateTypeInStock)
                //    {
                //        dicExpMatyInStock[item.Id] = item;
                //    }
                //}
                //if (listBloodTypeInStock != null)
                //{
                //    foreach (var item in listBloodTypeInStock)
                //    {
                //        dicExpBloodInStock[item.Id] = item;
                //    }
                //}

                //Review Mau
                gridControlBloodType__BloodPage.BeginUpdate();
                gridControlBloodType__BloodPage.DataSource = listBloodTypeInStock;
                gridControlBloodType__BloodPage.EndUpdate();

                //this.mediInStockProcessor.Reload(this.ucMediInStock, listMediTypeInStock);
                //this.mateInStockProcessor.Reload(this.ucMateInStock, listMateTypeInStock);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_MEST_METY_DEPA> LoadMestMetyDepa()
        {
            List<HIS_MEST_METY_DEPA> mestMatyDepaAll = null;
            List<HIS_MEST_METY_DEPA> mestMetyDepaList = null;
            try
            {
                if (!BackendDataWorker.IsExistsKey<HIS_MEST_METY_DEPA>())
                {
                    MOS.Filter.HisMestMetyDepaFilter metyDepaFilter = new HisMestMetyDepaFilter();
                    metyDepaFilter.IS_ACTIVE = 1;
                    mestMatyDepaAll = new BackendAdapter(new CommonParam()).Get<List<HIS_MEST_METY_DEPA>>("api/HisMestMetyDepa/Get", ApiConsumer.ApiConsumers.MosConsumer, metyDepaFilter, null);

                    if (mestMatyDepaAll != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA), mestMatyDepaAll, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                else
                {
                    mestMatyDepaAll = BackendDataWorker.Get<HIS_MEST_METY_DEPA>().Where(o => o.IS_ACTIVE == 1).ToList();
                }

                if (cboImpMediStock.EditValue != null)
                {
                    var impMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboImpMediStock.EditValue.ToString()));

                    mestMetyDepaList = mestMatyDepaAll.Where(o => o.DEPARTMENT_ID == impMediStock.DEPARTMENT_ID && o.IS_JUST_PRESCRIPTION != 1).ToList();
                    if (mestRoom != null && mestMetyDepaList != null && mestMetyDepaList.Count > 0)
                    {
                        mestMetyDepaList = mestMetyDepaList.Where(o => !o.MEDI_STOCK_ID.HasValue || o.MEDI_STOCK_ID == mestRoom.ID).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                mestMetyDepaList = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mestMetyDepaList;
        }

        private void LoadMedicineTypeFromStock()
        {
            try
            {
                var mestMetyDepaList = LoadMestMetyDepa();

                var medicineTypeList = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                CommonParam param = new CommonParam();
                HisMedicine2StockFilter medicineFilter = new HisMedicine2StockFilter();
                if (chkPlanningExport.Checked == true)
                {
                    mestRoom = listExpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue));
                    medicineFilter.FIRST_MEDI_STOCK_ID = mestRoom.ID;
                }
                else
                {
                    if (radioExport.Checked)
                    {
                        Hismedi.Clear();
                    }
                    if (Hismedi != null && Hismedi.Count > 0)
                    {
                        medicineFilter.FIRST_MEDI_STOCK_IDs = Hismedi.Select(o => o.ID).ToList();
                    }
                    else
                    {
                        if(mestRoom!=null)
                            medicineFilter.FIRST_MEDI_STOCK_ID = mestRoom.ID;
                    }
                }
                if (cboImpMediStock.EditValue != null)
                {
                    medicineFilter.SECOND_MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboImpMediStock.EditValue.ToString());
                }
                medicineFilter.IS_LEAF = 1;
                medicineFilter.MEDICINE_TYPE_IS_ACTIVE = true;
                medicineFilter.ORDER_DIRECTION = "ASC";
                medicineFilter.ORDER_FIELD = "MEDICINE_TYPE_NAME";

                this.listMediInStock = new List<HisMedicineInStockADO>();
                Inventec.Common.Logging.LogSystem.Warn("Begin call HisMedicine/GetIn2StockMedicine");
                Inventec.Common.Logging.LogSystem.Debug("medicineFilter đầu vào" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineFilter), medicineFilter));
                List<HisMedicineIn2StockSDO> _datas = new BackendAdapter(param).Get<List<HisMedicineIn2StockSDO>>("/api/HisMedicine/GetIn2StockMedicine",
ApiConsumers.MosConsumer, medicineFilter, param);
                Inventec.Common.Logging.LogSystem.Warn("End call HisMedicine/GetIn2StockMedicine");

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisMedicine/GetIn2StockMedicine medicineFilter", medicineFilter));
                decimal SecondAvailableAmount = 0, SecondTotalAmount = 0;
                // bỏ các thuốc chặn theo khoa
                if (_datas != null && _datas.Count > 0 && mestMetyDepaList != null && mestMetyDepaList.Count > 0)
                {
                    _datas = _datas.Where(o => !mestMetyDepaList.Select(p => p.MEDICINE_TYPE_ID).Contains(o.MEDICINE_TYPE_ID)).ToList();
                }

                if (_datas != null && _datas.Count > 0 && !chkHienThiLo.Checked)
                {
                    var dataGroups = _datas.GroupBy(p => p.MEDI_STOCK_ID).Select(p => p.ToList()).ToList();

                    //Inventec.Common.Logging.LogSystem.Debug("datas______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _datas.Count), _datas.Count));
                    //Inventec.Common.Logging.LogSystem.Debug("datas.GroupBy______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataGroups), dataGroups));
                    foreach (var item in dataGroups)
                    {
                        var mediGroups = item.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                        foreach (var item1 in mediGroups)
                        {
                            SecondAvailableAmount = 0;
                            SecondTotalAmount = 0;

                            SecondAvailableAmount = item1.Sum(o => o.SecondAvailableAmount ?? 0);
                            SecondTotalAmount = item1.Sum(o => o.SecondTotalAmount ?? 0);
                            bool check = false;
                            var checkmestRoom_ = mestRoom_.FirstOrDefault(o => o.ID == item1.FirstOrDefault().MEDI_STOCK_ID);
                            if (checkmestRoom_ != null)
                            {
                                check = checkmestRoom_.IS_BUSINESS == 1 ? true : false;
                            }

                            if (((item1.Sum(p => p.AvailableAmount) > 0 && !chkIsNotAvailableButHaveInStock.Checked) || (item1.Sum(p => p.SecondTotalAmount) > 0 && chkIsNotAvailableButHaveInStock.Checked))
                       && IS_GOODS_RESTRICT(item1.FirstOrDefault())
                       && IsCheckMedicine(
                       medicineTypeList,
                       check,
                       item1.FirstOrDefault().MEDICINE_TYPE_ID))
                            {
                                HisMedicineIn2StockSDO sdo = new HisMedicineIn2StockSDO();
                                sdo = item1.FirstOrDefault();
                                sdo.AvailableAmount = item1.Sum(p => p.AvailableAmount);
                                sdo.TotalAmount = item1.Sum(p => p.TotalAmount);
                                sdo.SecondAvailableAmount = SecondAvailableAmount;
                                sdo.SecondTotalAmount = SecondTotalAmount;
                                sdo.EXPIRED_DATE = item1.FirstOrDefault().EXPIRED_DATE;
                                sdo.ID = 0;
                                sdo.MEDI_STOCK_ID = item1.FirstOrDefault().MEDI_STOCK_ID;
                                HisMedicineInStockADO ado = new HisMedicineInStockADO(sdo, item1);
                                this.listMediInStock.Add(ado);
                            }
                        }
                        var a = this.listMediInStock;
                    }
                }
                else
                {
                    if (chkPlanningExport.Checked == true)
                    {

                        var datas = _datas.Where(o =>
                       ((o.AvailableAmount > 0 && !chkIsNotAvailableButHaveInStock.Checked) || (o.SecondTotalAmount > 0 && chkIsNotAvailableButHaveInStock.Checked))
                       && IS_GOODS_RESTRICT(o)
                       && IsCheckMedicine(medicineTypeList, this.mestRoom.IS_BUSINESS == 1 ? true : false, o.MEDICINE_TYPE_ID)
                       ).ToList();

                        foreach (var item in datas)
                        {
                            HisMedicineInStockADO ado = new HisMedicineInStockADO(item, null);
                            ado.SecondAvailableAmount = _datas.Where(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID).Sum(o => o.SecondAvailableAmount);
                            ado.SecondTotalAmount = _datas.Where(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID).Sum(o => o.SecondTotalAmount);
                            this.listMediInStock.Add(ado);
                        }

                    }
                    else
                    {
                        foreach (var items in mestRoom_)
                        {
                            var datas = _datas.Where(o =>
                           ((o.AvailableAmount > 0 && !chkIsNotAvailableButHaveInStock.Checked) || (o.SecondTotalAmount > 0 && chkIsNotAvailableButHaveInStock.Checked))
                           && IS_GOODS_RESTRICT(o)
                           && IsCheckMedicine(medicineTypeList, items.IS_BUSINESS == 1 ? true : false, o.MEDICINE_TYPE_ID)
                           ).ToList();

                            foreach (var item in datas)
                            {
                                HisMedicineInStockADO ado = new HisMedicineInStockADO(item, null);
                                ado.SecondAvailableAmount = _datas.Where(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID).Sum(o => o.SecondAvailableAmount);
                                ado.SecondTotalAmount = _datas.Where(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID).Sum(o => o.SecondTotalAmount);
                                this.listMediInStock.Add(ado);
                            }
                        }
                    }


                }
                Inventec.Common.Logging.LogSystem.Warn("End LoadMedicineTypeFromStock");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IS_GOODS_RESTRICT(HisMedicineIn2StockSDO data)
        {
            bool result = false;
            try
            {
                if (stock.IS_GOODS_RESTRICT != null
                    && stock.IS_GOODS_RESTRICT != 0
                    && medicineTypeIds != null
                    && medicineTypeIds.Count > 0)
                {
                    if (data != null
                    && medicineTypeIds.Equals(data.MEDICINE_TYPE_ID))
                    {
                        result = true;
                    }
                }
                else result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool IS_GOODS_RESTRICT(HisMedicineTypeInStockSDO data)
        {
            bool result = false;
            try
            {
                if (stock.IS_GOODS_RESTRICT != null
                    && stock.IS_GOODS_RESTRICT != 0
                    && medicineTypeIds != null
                    && medicineTypeIds.Count > 0)
                {
                    if (data != null
                    && medicineTypeIds.Equals(data.Id))
                    {
                        result = true;
                    }
                }
                else result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool IS_GOODS_RESTRICT(HisMaterialTypeInStockSDO data)
        {
            bool result = false;
            try
            {
                if (stock.IS_GOODS_RESTRICT != null
                    && stock.IS_GOODS_RESTRICT != 0
                    && materialTypeIds != null
                    && materialTypeIds.Count > 0)
                {
                    if (data != null
                    && materialTypeIds.Equals(data.Id))
                    {
                        result = true;
                    }
                }
                else result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool IsCheckMedicine(List<V_HIS_MEDICINE_TYPE> _medicineTypes, bool isBusiness, long medicineTypeId)
        {
            bool result = false;
            try
            {
                var data = _medicineTypes.FirstOrDefault(p => p.ID == medicineTypeId);
                if (data != null)// && data.IS_REUSABLE != 1)
                {
                    if (isBusiness)
                    {
                        if (data.IS_BUSINESS == 1)
                            result = true;
                    }
                    else
                    {
                        if (data.IS_BUSINESS != 1)
                            result = true;
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

        private List<HIS_MEST_MATY_DEPA> LoadMestMatyDepa()
        {

            List<HIS_MEST_MATY_DEPA> mestMatyDepaList = null;
            List<HIS_MEST_MATY_DEPA> mestMatyDepaAll = null;
            try
            {
                if (!BackendDataWorker.IsExistsKey<HIS_MEST_MATY_DEPA>())
                {
                    MOS.Filter.HisMestMatyDepaFilter metyDepaFilter = new HisMestMatyDepaFilter();
                    metyDepaFilter.IS_ACTIVE = 1;
                    mestMatyDepaAll = new BackendAdapter(new CommonParam()).Get<List<HIS_MEST_MATY_DEPA>>("api/HisMestMatyDepa/Get", ApiConsumer.ApiConsumers.MosConsumer, metyDepaFilter, null);

                    if (mestMatyDepaAll != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MEST_MATY_DEPA), mestMatyDepaAll, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                else
                {
                    mestMatyDepaAll = BackendDataWorker.Get<HIS_MEST_MATY_DEPA>().Where(o => o.IS_ACTIVE == 1).ToList();
                }

                if (cboImpMediStock.EditValue != null)
                {
                    var impMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboImpMediStock.EditValue.ToString()));

                    mestMatyDepaList = mestMatyDepaAll.Where(o => o.DEPARTMENT_ID == impMediStock.DEPARTMENT_ID && o.IS_JUST_PRESCRIPTION != 1).ToList();
                    if (mestRoom != null && mestMatyDepaList != null && mestMatyDepaList.Count > 0)
                    {
                        mestMatyDepaList = mestMatyDepaList.Where(o => !o.MEDI_STOCK_ID.HasValue || o.MEDI_STOCK_ID == mestRoom.ID).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                mestMatyDepaList = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mestMatyDepaList;
        }

        private void LoadMaterialTypeFromStock()
        {
            try
            {
                var mestMatyDepaList = LoadMestMatyDepa();

                var materialTypeList = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                HisMaterial2StockFilter mateFilter = new HisMaterial2StockFilter();

                if (chkPlanningExport.Checked == true)
                {
                    mestRoom = listExpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue));
                    mateFilter.FIRST_MEDI_STOCK_ID = mestRoom.ID;
                }
                else
                {
                    if (radioExport.Checked)
                    {
                        Hismedi.Clear();
                    }
                    if (Hismedi != null && Hismedi.Count > 0)
                    {
                        mateFilter.FIRST_MEDI_STOCK_IDs = Hismedi.Select(o => o.ID).ToList();
                    }
                    else
                    {
                        if (mestRoom!=null)
                        {
                            mateFilter.FIRST_MEDI_STOCK_ID = mestRoom.ID;
                        }
                    }
                }
                if (cboImpMediStock.EditValue != null)
                {
                    mateFilter.SECOND_MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboImpMediStock.EditValue.ToString());
                }
                mateFilter.MATERIAL_TYPE_IS_ACTIVE = true;
                mateFilter.IS_LEAF = 1;
                mateFilter.ORDER_DIRECTION = "ASC";
                mateFilter.ORDER_FIELD = "MATERIAL_TYPE_NAME";

                CommonParam param = new CommonParam();
                this.listMateInStock = new List<HisMaterialInStockADO>();
                List<HisMaterialIn2StockSDO> _datas = new BackendAdapter(param).Get<List<HisMaterialIn2StockSDO>>("/api/HisMaterial/GetIn2StockMaterial", ApiConsumers.MosConsumer, mateFilter, param);
                decimal SecondAvailableAmount = 0, SecondTotalAmount = 0;
                // Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("_datas.Where(o => o.EXPIRED_DATE != null____" + Inventec.Common.Logging.LogUtil.GetMemberName(() => _datas.Where(o => o.EXPIRED_DATE != null)), _datas.Where(o => o.EXPIRED_DATE != null)));
                if (_datas != null && _datas.Count > 0 && mestMatyDepaList != null && mestMatyDepaList.Count > 0)
                {
                    _datas = _datas.Where(o => !mestMatyDepaList.Select(p => p.MATERIAL_TYPE_ID).Contains(o.MATERIAL_TYPE_ID)).ToList();
                }
                if (_datas != null && _datas.Count > 0 && !chkHienThiLo.Checked)
                {
                    var dataGroups = _datas.GroupBy(p => p.MEDI_STOCK_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        var mediGroups = item.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                        foreach (var item1 in mediGroups)
                        {
                            SecondAvailableAmount = 0;
                            SecondTotalAmount = 0;

                            SecondAvailableAmount = item1.Sum(o => o.SecondAvailableAmount ?? 0);
                            SecondTotalAmount = item1.Sum(o => o.SecondTotalAmount ?? 0);
                            bool check = false;
                            var checkmestRoom_ = mestRoom_.FirstOrDefault(o => o.ID == item1.FirstOrDefault().MEDI_STOCK_ID);
                            if (checkmestRoom_ != null)
                            {
                                check = checkmestRoom_.IS_BUSINESS == 1 ? true : false;
                            }
                            if (((item1.Sum(p => p.AvailableAmount) > 0 && !chkIsNotAvailableButHaveInStock.Checked) || (item1.Sum(p => p.SecondAvailableAmount) > 0 && chkIsNotAvailableButHaveInStock.Checked))
                           && IS_GOODS_RESTRICT(item1.FirstOrDefault())
                           && IsCheckMaterial(
                           materialTypeList,
                           check,
                           item1.FirstOrDefault().MATERIAL_TYPE_ID))
                            {
                                //Inventec.Common.Logging.LogSystem.Info("\r\n  item1.FirstOrDefault().EXPIRED_DATE______:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item1.FirstOrDefault().EXPIRED_DATE), item1.FirstOrDefault().EXPIRED_DATE));
                                HisMaterialIn2StockSDO sdo = new HisMaterialIn2StockSDO();
                                sdo = item1.FirstOrDefault();
                                sdo.AvailableAmount = item1.Sum(p => p.AvailableAmount);
                                sdo.TotalAmount = item1.Sum(p => p.TotalAmount);
                                sdo.SecondAvailableAmount = SecondAvailableAmount;
                                sdo.SecondTotalAmount = SecondTotalAmount;
                                sdo.EXPIRED_DATE = item1.FirstOrDefault().EXPIRED_DATE;
                                sdo.MEDI_STOCK_ID = item1.FirstOrDefault().MEDI_STOCK_ID;
                                //sdo.PACKAGE_NUMBER = null;
                                sdo.ID = 0;
                                HisMaterialInStockADO ado = new HisMaterialInStockADO(sdo, item1);
                                this.listMateInStock.Add(ado);
                            }

                        }

                    }
                }
                else
                {
                    if (_datas != null && _datas.Count() > 0)
                    {
                        if (chkPlanningExport.Checked == true)
                        {
                            var datas = _datas.Where(o =>
                      ((o.AvailableAmount > 0 && !chkIsNotAvailableButHaveInStock.Checked) || (o.SecondAvailableAmount > 0 && chkIsNotAvailableButHaveInStock.Checked))
                      && IS_GOODS_RESTRICT(o)
                      && IsCheckMaterial(materialTypeList, this.mestRoom.IS_BUSINESS == 1 ? true : false, o.MATERIAL_TYPE_ID)
                      ).ToList();
                            foreach (var item in datas)
                            {
                                HisMaterialInStockADO ado = new HisMaterialInStockADO(item, null);
                                ado.SecondAvailableAmount = _datas.Where(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID).Sum(o => o.SecondAvailableAmount);
                                ado.SecondTotalAmount = _datas.Where(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID).Sum(o => o.SecondTotalAmount);
                                this.listMateInStock.Add(ado);
                            }
                        }
                        else
                        {
                            foreach (var items in mestRoom_)
                            {
                                var datas = _datas.Where(o =>
                      ((o.AvailableAmount > 0 && !chkIsNotAvailableButHaveInStock.Checked) || (o.SecondAvailableAmount > 0 && chkIsNotAvailableButHaveInStock.Checked))
                      && IS_GOODS_RESTRICT(o)
                      && IsCheckMaterial(materialTypeList, items.IS_BUSINESS == 1 ? true : false, o.MATERIAL_TYPE_ID)
                      ).ToList();
                                foreach (var item in datas)
                                {
                                    HisMaterialInStockADO ado = new HisMaterialInStockADO(item, null);
                                    ado.SecondAvailableAmount = _datas.Where(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID).Sum(o => o.SecondAvailableAmount);
                                    ado.SecondTotalAmount = _datas.Where(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID).Sum(o => o.SecondTotalAmount);
                                    this.listMateInStock.Add(ado);
                                }
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBloodTypeFromStock()
        {
            try
            {
                //Mau
                listBloodTypeInStock = new List<HisBloodTypeInStockADO>();
                HisBloodTypeStockViewFilter bltyFilter = new HisBloodTypeStockViewFilter();
                if (chkPlanningExport.Checked == true)
                {
                    bltyFilter.MEDI_STOCK_ID = mestRoom.ID;
                }
                else
                {
                    if (radioExport.Checked)
                    {
                        Hismedi.Clear();
                    }
                    if (Hismedi != null && Hismedi.Count > 0)
                    {
                        bltyFilter.MEDI_STOCK_IDs = Hismedi.Select(o => o.ID).ToList();
                    }
                    else
                    {
                        if(mestRoom!=null)
                            bltyFilter.MEDI_STOCK_ID = mestRoom.ID;
                    }

                }
                bltyFilter.IS_LEAF = true;
                bltyFilter.IS_AVAILABLE = true;
                bltyFilter.IS_ACTIVE = 1;
                listBloodTypeInStock = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisBloodTypeInStockADO>>("api/HisBloodType/GetInStockBloodType", ApiConsumers.MosConsumer, bltyFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IS_GOODS_RESTRICT(HisMaterialIn2StockSDO data)
        {
            bool result = false;
            try
            {
                if (stock.IS_GOODS_RESTRICT != null
                    && stock.IS_GOODS_RESTRICT != 0
                    && materialTypeIds != null
                    && materialTypeIds.Count > 0)
                {
                    if (data != null
                    && materialTypeIds.Equals(data.MATERIAL_TYPE_ID))
                    {
                        result = true;
                    }
                }
                else result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool IsCheckMaterial(List<V_HIS_MATERIAL_TYPE> _materialTypes, bool isBusiness, long materialTypeId)
        {
            bool result = false;
            try
            {
                var data = _materialTypes.FirstOrDefault(p => p.ID == materialTypeId);
                if (data != null)// && data.IS_REUSABLE != 1)
                {
                    if (isBusiness)
                    {
                        if (data.IS_BUSINESS == 1)
                            result = true;
                    }
                    else
                    {
                        if (data.IS_BUSINESS != 1)
                            result = true;
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

        private void ResetValueControlDetail()
        {
            try
            {
                spinExpAmount.Value = 0;
                txtNote.Text = "";
                if ((this.currentMediMate != null && !isSupplement) || chkPlanningExport.CheckState == CheckState.Checked)
                {
                    btnAdd.Enabled = true;
                    //spinExpAmount.Properties.MaxValue = this.currentMediMate.AVAILABLE_AMOUNT ?? 0;
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
                gridControlExpMestChmsDetail.BeginUpdate();
                gridControlExpMestChmsDetail.DataSource = null;
                gridControlExpMestChmsDetail.EndUpdate();

                //gridControlMedicine.BeginUpdate();
                //gridControlMedicine.DataSource = null;
                //gridControlMedicine.EndUpdate();

                //gridControlMaterial.BeginUpdate();
                //gridControlMaterial.DataSource = null;
                //gridControlMaterial.EndUpdate();
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
                xtraTabControlMain.SelectedTabPageIndex = 0;
                this.currentMediMate = null;
                this.resultSdo = null;
                isUpdate = false;
                ResetValueControlDetail();
                ResetGridControlDetail();
                ddBtnPrint.Enabled = false;
                txtDescription.Text = "";
                txtSearchMedicine.Text = "";
                txtSearchMaterial.Text = "";
                chkIsNotAvailableButHaveInStock.CheckState = CheckState.Unchecked;
                cboReasonRequired.EditValue = null;
                dxValidationProvider1.RemoveControlError(txtDescription);
                dxValidationProvider1.RemoveControlError(txtImpMediStock);
                dxValidationProvider1.RemoveControlError(txtExpMediStock);

                dxValidationProvider2.RemoveControlError(txtNote);
                dxValidationProvider2.RemoveControlError(spinExpAmount);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
                GridCheckMarksSelection gridCheckMark = cboExpMediStock.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboExpMediStock.Properties.View);
                    mestRoom_.Clear();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ResetValueControlCommon1()
        {
            try
            {
                this.isSupplement = false;
                xtraTabControlMain.SelectedTabPageIndex = 0;
                this.currentMediMate = null;
                this.resultSdo = null;
                isUpdate = false;
                ResetValueControlDetail();
                //ResetGridControlDetail();
                ddBtnPrint.Enabled = false;
                txtDescription.Text = "";
                txtSearchMedicine.Text = "";
                txtSearchMaterial.Text = "";

                dxValidationProvider1.RemoveControlError(txtDescription);
                dxValidationProvider2.RemoveControlError(txtNote);
                dxValidationProvider2.RemoveControlError(spinExpAmount);

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
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboImpMediStock, null, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboReasonRequired()
        {
            try
            {
                var reason = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboReasonRequired, reason, controlEditorADO);
                cboReasonRequired.Properties.ImmediatePopup = true;
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
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboExpMediStock, null, controlEditorADO);

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
                var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_TYPE_ID == this.roomTypeId && o.ROOM_ID == this.roomId && o.IS_ACTIVE == 1);
                if (mediStock != null)
                {
                    cboImpMediStock.EditValue = mediStock.ID;
                }
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
                    var impMest = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMediStock.EditValue) && o.IS_ACTIVE == 1);
                    if (impMest != null)
                    {
                        var listMestId = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == impMest.ROOM_ID).Select(s => s.MEDI_STOCK_ID).ToList();
                        if (listMestId != null && listMestId.Count > 0)
                        {
                            if (impMest.ROOM_ID == this.roomId)
                            {
                                hisExpMest = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => listMestId.Contains(o.ID) && o.IS_ACTIVE == 1).ToList();
                            }
                            else
                            {
                                hisExpMest = listCurrentMediStock.Where(o => listMestId.Contains(o.ID)).ToList();
                            }
                        }
                    }
                }

                if (cboExpMediStock.EditValue != null)
                {
                    var expMest = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue) && o.IS_ACTIVE == 1);
                    if (expMest != null)
                    {
                        var listRoomId = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.MEDI_STOCK_ID == expMest.ID).Select(s => s.ROOM_ID).ToList();
                        if (listRoomId != null && listRoomId.Count > 0)
                        {
                            if (expMest.ROOM_ID == this.roomId)
                            {
                                hisImpMest = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => listRoomId.Contains(o.ROOM_ID) && o.IS_ACTIVE == 1).ToList();
                            }
                            else
                            {
                                hisImpMest = listCurrentMediStock.Where(o => listRoomId.Contains(o.ROOM_ID)).ToList();
                            }
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
                    listImpMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == 1).ToList();
                    listExpMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == 1).ToList();
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

        private void LoadDataToCboMediStock()
        {
            try
            {
                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = null;
                gridControlMedicine.EndUpdate();

                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = null;
                gridControlMaterial.EndUpdate();

                currentMediMate_.Clear();
                listImpMediStock = new List<V_HIS_MEDI_STOCK>();
                listExpMediStock = new List<V_HIS_MEDI_STOCK>();
                if (radioImport.Checked)
                {
                    listImpMediStock = listCurrentMediStock;
                    cboImpMediStock.EditValue = null;
                    cboImpMediStock.Enabled = false;
                    txtImpMediStock.Enabled = false;
                    cboExpMediStock.EditValue = null;
                    txtExpMediStock.Text = "";
                    cboExpMediStock.Enabled = true;
                    txtExpMediStock.Enabled = true;
                    cboImpMediStock.Properties.DataSource = listImpMediStock;

                    if (currentMediStock != null)
                    {


                        cboImpMediStock.EditValue = this.currentMediStock.ID;
                        var listMediStockId = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == currentMediStock.ROOM_ID).Select(s => s.MEDI_STOCK_ID).ToList();
                        if (listMediStockId != null && listMediStockId.Count > 0)
                        {
                            listExpMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => listMediStockId.Contains(o.ID) && o.IS_ACTIVE == 1).ToList();
                        }
                        if (listExpMediStock != null && listExpMediStock.Count > 0)
                        {
                            listExpMediStock = GetMediStock(listExpMediStock, currentMediStock);
                        }
                    }
                    cboExpMediStock.Properties.DataSource = listExpMediStock;

                    chkPlanningExport.Enabled = false;
                    chkPlanningExport.CheckState = CheckState.Unchecked;
                    SetDafaultPlanningExport(false);
                }
                else
                {
                    mestRoom_.Clear();
                    //currentMediStock = new V_HIS_MEDI_STOCK();
                    //currentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_TYPE_ID == this.roomTypeId && o.ROOM_ID == this.roomId && o.IS_ACTIVE == 1);
                    mestRoom_.Add(currentMediStock);
                    listExpMediStock = listCurrentMediStock;
                    cboImpMediStock.EditValue = null;
                    cboImpMediStock.Enabled = true;
                    txtImpMediStock.Enabled = true;
                    txtImpMediStock.Text = "";
                    cboExpMediStock.EditValue = null;
                    cboExpMediStock.Enabled = false;
                    txtExpMediStock.Enabled = false;
                    cboExpMediStock.Properties.DataSource = listExpMediStock;

                    if (this.currentMediStock != null)
                    {
                        cboExpMediStock.EditValue = this.currentMediStock.ID;
                        var listRoomId = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.MEDI_STOCK_ID == this.currentMediStock.ID).Select(s => s.ROOM_ID).ToList();
                        if (listRoomId != null && listRoomId.Count > 0)
                        {
                            listImpMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => listRoomId.Contains(o.ROOM_ID) && o.IS_ACTIVE == 1).ToList();
                        }
                        if (listImpMediStock != null && listImpMediStock.Count > 0)
                        {
                            listImpMediStock = GetMediStock(listImpMediStock, currentMediStock);
                        }
                        if (currentMediStock.IS_PLANNING_TRANS_AS_DEFAULT == 1)
                        {
                            chkPlanningExport.Enabled = true;
                            //chkPlanningExport.CheckState = CheckState.Checked;
                            //  SetDafaultPlanningExport(true);
                        }
                        else
                        {
                            chkPlanningExport.Enabled = true;
                            // chkPlanningExport.CheckState = CheckState.Unchecked;
                            //SetDafaultPlanningExport(false);
                        }
                    }
                    cboImpMediStock.Properties.DataSource = listImpMediStock;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_MEDI_STOCK> GetMediStock(List<V_HIS_MEDI_STOCK> _MediStocks, V_HIS_MEDI_STOCK _medi)
        {
            List<V_HIS_MEDI_STOCK> result = new List<V_HIS_MEDI_STOCK>();
            try
            {
                if (_medi.IS_BUSINESS == 1)
                    result = _MediStocks.Where(p => p.IS_BUSINESS == 1 && p.ID != _medi.ID).ToList();
                else
                    result = _MediStocks.Where(p => (p.IS_BUSINESS != 1 || p.IS_BUSINESS == null) && p.ID != _medi.ID).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void SetEnableCboMediStockAndButton(bool enable)
        {
            try
            {
                cboExpMediStock.Enabled = enable;
                cboImpMediStock.Enabled = enable;
                txtExpMediStock.Enabled = enable;
                txtImpMediStock.Enabled = enable;
                btnSave.Enabled = enable;
                btnUpdate.Enabled = !enable;
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
                ValidationSingleControl(cboChooseABO);
                ValidationSingleControl(cboChooseRH);
                ValidationSingleControlProvider1(txtImpMediStock);
                ValidationSingleControlProvider1(txtExpMediStock);
                lciReasonRequired.AppearanceItemCaption.ForeColor = Color.Black;
                if (IsReasonRequired)
                {
                    lciReasonRequired.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationSingleControl(cboReasonRequired);
                }
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
                TxtExpMediStockValidationRule txtimpMestRule = new TxtExpMediStockValidationRule();
                txtimpMestRule.txtText = txtImpMediStock;
                dxValidationProvider2.SetValidationRule(txtImpMediStock, txtimpMestRule);
                impMestRule.cboImpMediStock = cboImpMediStock;
                dxValidationProvider1.SetValidationRule(cboImpMediStock, impMestRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExpMediStock()
        {
            try
            {
                ExpMediStockValidationRule expMestRule = new ExpMediStockValidationRule();
                expMestRule.cboExpMediStock = cboExpMediStock;
                dxValidationProvider1.SetValidationRule(cboExpMediStock, expMestRule);
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
        private void ValidationSingleControlProvider1(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                dxValidationProvider1.SetValidationRule(control, validRule);
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

        private void LoadKeyUCLanguage()
        {
            try
            {

                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //Button
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_ADD", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_NEW", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_SAVE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.btnUpdate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_UPDATE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_PRINT", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                //this.btnSupplement.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_SUPPLEMENT", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                //this.btnCastrate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__BTN_CASTRATE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);

                //Layout
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_DESCRIPTION", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.layoutExpAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_EXP_AMOUNT", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.layoutExpMediStock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_EXP_MEDI_STOCK", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.layoutImpMediStock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_IMP_MEDI_STOCK", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.layoutNote.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__LAYOUT_NOTE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);

                //GridControl Detail
                this.gridColumn_ExpMestChmsDetail_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_ManufactureName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_MANUFACTURER_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_MediMateTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_MEDI_MATE_TYPE_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_NationalName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_NATIONAL_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.gridColumn_ExpMestChmsDetail_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__GRID_CONTROL__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);

                //Xtra Tab
                // this.xtraTabPageExpMestMate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__XTRA_TAB_MATERIAL", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                // this.xtraTabPageExpMestMedi.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__XTRA_TAB_MEDICINE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__XTRA_TAB_MATERIAL", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__XTRA_TAB_MEDICINE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);

                //Repository Button
                this.repositoryItemBtnDeleteDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXP_MEST_CHMS_CREATE__REPOSITORY_BTN_DELETE", Base.ResourceLangManager.LanguageUCExpMestChmsCreate, cul);
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
                gridViewExpMestChmsDetail.PostEditor();
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

        public void BtnPrintt()
        {
            try
            {
                if (!ddBtnPrint.Enabled)
                    return;
                ddBtnPrint_Click(null, null);
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
                gridViewExpMestChmsDetail.PostEditor();
                btnUpdate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ddBtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                onClickPrintPhieuXuatChuyenKho(null, null);
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
                    BindingList<HisBloodTypeInStockADO> listResult = null;
                    if (!String.IsNullOrEmpty(keyword.Trim()))
                    {
                        List<HisBloodTypeInStockADO> rearchResult = new List<HisBloodTypeInStockADO>();

                        rearchResult = listBloodTypeInStock.Where(o =>
                                                        ((o.BloodTypeCode ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                        || (o.BloodTypeName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                        || (o.Amount.ToString() ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                        || (o.Volume.ToString() ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower()))
                                                        ).Distinct().ToList();

                        listResult = new BindingList<HisBloodTypeInStockADO>(rearchResult);
                    }
                    else
                    {
                        listResult = new BindingList<HisBloodTypeInStockADO>(listBloodTypeInStock);
                    }
                    gridControlBloodType__BloodPage.DataSource = listResult;
                    gridViewMedicine.ExpandAllGroups();
                    gridViewMaterial.ExpandAllGroups();
                    gridViewBloodType__BloodPage.ExpandAllGroups();
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
                var data = (HisBloodTypeInStockADO)gridViewBloodType__BloodPage.GetFocusedRow();
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

        private void InitComboBloodRH()
        {
            try
            {
                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboChooseRH, data, controlEditorADO);
                cboChooseRH.EditValue = (data != null && data.Count > 0) ? data[0].ID : 0;
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
                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboChooseABO, data, controlEditorADO);
                cboChooseABO.EditValue = (data != null && data.Count > 0) ? data[0].ID : 0;
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

        private void gridViewExpMestChmsDetail_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);
                bool IsMedicine = (bool)vw.GetRowCellValue(e.RowHandle, "IsMedicine");
                bool IsBlood = (bool)vw.GetRowCellValue(e.RowHandle, "IsBlood");
                if (IsMedicine)
                {
                    e.Appearance.ForeColor = Color.Black;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else if (IsBlood)
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else
                {
                    e.Appearance.ForeColor = Color.Blue;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpMediStock_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboImpMediStock.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHienThiLo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkHienThiLo.CheckState == CheckState.Checked)
                {
                    chkPlanningExport.CheckState = CheckState.Unchecked;
                    chkPlanningExport.Enabled = false;
                    SetDafaultPlanningExport(false);
                }
                else if (radioImport.CheckState == CheckState.Unchecked)
                {
                    chkPlanningExport.Enabled = true;
                }
                FillDataToTrees();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisMedicineInStockADO data = (HisMedicineInStockADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    //Inventec.Common.Logging.LogSystem.Info("data_______: " +Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    // Inventec.Common.Logging.LogSystem.Info("\r\n data.EXPIRED_DATE_______: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.EXPIRED_DATE), data.EXPIRED_DATE));
                    if (e.Column.FieldName == "EXPIRED_DATE_STR" && data != null)
                    {
                        if (chkHienThiLo.Checked)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE.ToString());
                        }
                        else
                        {
                            e.Value = data.EXPIRED_DATE_STRS;
                        }
                    }

                    if (e.Column.FieldName == "TEN_KHO_STR")
                    {
                        if (radioImport.Checked == true)
                        {
                            if (data != null && data.MEDI_STOCK_ID != null)
                            {
                                e.Value = listExpMediStock.FirstOrDefault(o => o.ID == data.MEDI_STOCK_ID).MEDI_STOCK_NAME;

                            }
                            else
                            {
                                e.Value = "";
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

        private void gridViewMedicine_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                this.currentMediMate = null;
                var row = (HisMedicineInStockADO)gridViewMedicine.GetFocusedRow();
                if (row != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(row);
                    ResetValueControlDetail();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisMaterialInStockADO data = (HisMaterialInStockADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "EXPIRED_DATE_STR" && data != null && data.EXPIRED_DATE > 0)
                    {
                        if (this.chkHienThiLo.Checked)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE.ToString());
                        }
                        else
                        {
                            e.Value = data.EXPIRED_DATE_STRS;
                        }

                    }
                    if (e.Column.FieldName == "TEN_KHO_STR_R")
                    {
                        //if (radioImport.Checked == true)
                        //{
                        if (data.MEDI_STOCK_ID.HasValue)
                        {
                            e.Value = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == data.MEDI_STOCK_ID && o.IS_ACTIVE == 1).MEDI_STOCK_NAME;
                            //Inventec.Common.Logging.LogSystem.Info("e.Value: " +Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => e.Value), e.Value));
                            //   Inventec.Common.Logging.LogSystem.Info("e.Value: " + e.Value);
                        }
                        else
                        {
                            e.Value = "";
                        }
                        //}


                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                this.currentMediMate = null;
                var row = (HisMaterialInStockADO)gridViewMaterial.GetFocusedRow();
                if (row != null)
                {
                    this.currentMediMate = new ADO.MediMateTypeADO(row);
                    Inventec.Common.Logging.LogSystem.Debug("Nam PPP" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentMediMate), this.currentMediMate));
                    ResetValueControlDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchMedicine_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtSearchMaterial_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void SearchMedicine(string keyword)
        {
            try
            {
                List<HisMedicineInStockADO> rearchResult = new List<HisMedicineInStockADO>();
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    rearchResult = this.listMediInStock.Where(o =>
                                                    ((o.MEDICINE_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MEDICINE_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ACTIVE_INGR_BHYT_NAME ?? "").ToString().ToUpper().Contains(keyword.Trim().ToUpper())
                                                    )
                                                    ).Distinct().ToList();


                }
                else
                {
                    rearchResult = this.listMediInStock.ToList();
                }
                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = rearchResult;
                gridControlMedicine.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SearchMaterial(string keyword)
        {
            try
            {
                List<HisMaterialInStockADO> rearchResult = new List<HisMaterialInStockADO>();
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    rearchResult = this.listMateInStock.Where(o =>
                                                    ((o.MATERIAL_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MATERIAL_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();

                }
                else
                {
                    rearchResult = this.listMateInStock.ToList();
                }
                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = rearchResult;
                gridControlMaterial.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearchMedicine_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchMedicine(strValue);
                if (e.KeyCode == Keys.Down)
                {
                    gridViewMedicine.Focus();
                }
                gridViewExpMestChmsDetail.ExpandAllGroups();
                gridViewBloodType__BloodPage.ExpandAllGroups();
                gridViewMedicine.ExpandAllGroups();
                gridViewMaterial.ExpandAllGroups();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchMaterial_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchMaterial(strValue);
                gridViewMedicine.ExpandAllGroups();
                gridViewMaterial.ExpandAllGroups();
                gridViewBloodType__BloodPage.ExpandAllGroups();
                if (e.KeyCode == Keys.Down)
                {
                    gridViewMaterial.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlMaxLength1()
        {
            try
            {
                ControlMaxLengthValidationRule maxLengthRule = new ControlMaxLengthValidationRule();
                maxLengthRule.editor = this.txtDescription;
                maxLengthRule.maxLength = 500;
                maxLengthRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtDescription, maxLengthRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlMaxLength2()
        {
            try
            {
                if (txtNote.Enabled)
                {
                    ControlMaxLengthValidationRule maxLengthRule = new ControlMaxLengthValidationRule();
                    maxLengthRule.editor = txtNote;
                    maxLengthRule.maxLength = 200;
                    maxLengthRule.ErrorType = ErrorType.Warning;
                    dxValidationProvider2.SetValidationRule(txtNote, maxLengthRule);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsNotAvailableButHaveInStock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboExpMediStock != null && cboImpMediStock.EditValue != null)
                {
                    WaitingManager.Show();
                    FillDataToTrees();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void gridViewExpMestChmsDetail_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = Convert.ToString(this.gridViewExpMestChmsDetail.GetGroupRowValue(e.RowHandle, this.gridColumnKho) ?? "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void gridViewExpMestChmsDetail_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;
                if (e.Column.FieldName == "MEDI_STOCK_STR" && e.IsForGroupRow)
                {
                    string rowValue = Convert.ToString(view.GetGroupRowValue(e.GroupRowHandle, e.Column));
                    e.DisplayText = "" + rowValue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestChmsDetail_CustomColumnGroup(object sender, CustomColumnSortEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "MEDI_STOCK_STR")
                {
                    e.Result = 1;
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridControlExpMestChmsDetail_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                GridControl grid = sender as GridControl;
                GridView view = grid.FocusedView as GridView;
                if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
                {
                    if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && view.FocusedColumn.VisibleIndex == 0))
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        // grid.SelectNextControl(btnChoice, e.Modifiers == Keys.None, false, false, true);
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    if (this.gridViewExpMestChmsDetail.IsEditing)
                        this.gridViewExpMestChmsDetail.CloseEditor();

                    if (this.gridViewExpMestChmsDetail.FocusedRowModified)
                        this.gridViewExpMestChmsDetail.UpdateCurrentRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMedicine_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = Convert.ToString(this.gridViewMedicine.GetGroupRowValue(e.RowHandle, this.gridColumnKHOX) ?? "");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<V_HIS_MEDI_STOCK> mestRoom_ = new List<V_HIS_MEDI_STOCK>();
        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue1, string DisplayValue2, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue1;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue1);

                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "Mã kho";


                DevExpress.XtraGrid.Columns.GridColumn col3 = cbo.Properties.View.Columns.AddField(DisplayValue2);
                col3.VisibleIndex = 1;
                col3.Width = 200;
                col3.Caption = "Tên kho";

                cbo.Properties.PopupFormWidth = 300;
                // cbo.Properties.View.OptionsView.ShowColumnHeaders = false;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
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

        List<V_HIS_MEDI_STOCK> Hismedi = new List<V_HIS_MEDI_STOCK>();

        private void SelectionGrid__ExpMediStock(object sender, EventArgs e)
        {
            try
            {

                Hismedi = new List<V_HIS_MEDI_STOCK>();
                foreach (V_HIS_MEDI_STOCK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        Hismedi.Add(rv);
                }
                if (Hismedi != null && Hismedi.Count > 0)
                {
                    mestRoom_.Clear();
                    if (Hismedi != null && Hismedi.Count > 0)
                    {
                        foreach (var items in Hismedi)
                        {
                            V_HIS_MEDI_STOCK mestRoomK = new V_HIS_MEDI_STOCK();
                            mestRoomK = listExpMediStock.FirstOrDefault(o => o.ID == items.ID);
                            mestRoom_.Add(mestRoomK);

                        }
                        txtExpMediStock.Properties.Buttons[1].Visible = true;
                        txtExpMediStock.Text = "";
                        mestRoom_.Select(o => o.MEDI_STOCK_NAME).ToList();
                        txtExpMediStock.Text = string.Join(",", mestRoom_.Select(o => o.MEDI_STOCK_NAME).ToList());
                        dxValidationProvider1.RemoveControlError(txtExpMediStock);
                    }

                    FillDataToTrees();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadKhoXuat()
        {

            try
            {
                if (chkPlanningExport.Checked == true)
                {
                    // LoadDataToComboExpMediStock();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 200, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(cboImpMediStock, listExpMediStock, controlEditorADO);

                }
                else
                {
                    InitCombo(cboExpMediStock, listExpMediStock, "MEDI_STOCK_CODE", "MEDI_STOCK_NAME", "ID");
                    InitCheck(cboExpMediStock, SelectionGrid__ExpMediStock);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridViewMaterial_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = Convert.ToString(this.gridViewMaterial.GetGroupRowValue(e.RowHandle, this.gridColumnKHOVatTu) ?? "");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBloodType__BloodPage_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

            if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
            {
                HisBloodTypeInStockADO data = (HisBloodTypeInStockADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                //  Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("e.Value_____" + Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                if (e.Column.FieldName == "TEN_KHO_STR_MAU")
                {
                    if (radioImport.Checked == true)
                    {
                        if (data != null && data.MediStockId != null)
                        {
                            e.Value = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == data.MediStockId).MEDI_STOCK_NAME;
                            //  Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("e.Value_____" + Inventec.Common.Logging.LogUtil.GetMemberName(() => e.Value), e.Value));
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                }
            }
        }

        private void gridViewBloodType__BloodPage_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = Convert.ToString(this.gridViewBloodType__BloodPage.GetGroupRowValue(e.RowHandle, this.gridColumnKhoMau) ?? "");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomDrawGroupRow(object sender, RowCellCustomDrawEventArgs e)
        {

        }
    }
}
