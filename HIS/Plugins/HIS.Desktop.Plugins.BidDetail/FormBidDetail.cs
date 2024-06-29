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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.BidDetail
{
    public partial class FormBidDetail : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        MOS.EFMODEL.DataModels.HIS_BID bidDetail;
        List<MOS.EFMODEL.DataModels.V_HIS_BID_MEDICINE_TYPE> bidMedicineTypes;
        List<MOS.EFMODEL.DataModels.V_HIS_BID_MATERIAL_TYPE> bidMaterialTypes;
        List<MOS.EFMODEL.DataModels.V_HIS_BID_BLOOD_TYPE> bidBloodTypes;
        List<MPS.Processor.Mps000119.PDO.HisBidMetyADO> bidprintAdo;
        Inventec.Desktop.Common.Modules.Module moduleData;
        Dictionary<long, V_HIS_MEDICINE_TYPE> dicMedicineType;
        Dictionary<long, V_HIS_MATERIAL_TYPE> dicMaterialType;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        long roomId = 0;
        long roomTypeId = 0;
        decimal? adjustAmount;
        short? isActive ;
        #endregion

        #region Construct
        public FormBidDetail(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.currentModule = moduleData;
            this.roomId = moduleData.RoomId;
            this.roomTypeId = moduleData.RoomTypeId;
            try
            {
                SetIcon();
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                SetPrintTypeToMps();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormBidDetail(MOS.EFMODEL.DataModels.HIS_BID data, Inventec.Desktop.Common.Modules.Module moduleData)
            : this(moduleData)
        {
            try
            {
                this.bidDetail = data;
                bidprintAdo = new List<MPS.Processor.Mps000119.PDO.HisBidMetyADO>();
                this.Text = moduleData.text;
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BidDetail.Resources.Lang", typeof(HIS.Desktop.Plugins.BidDetail.FormBidDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormBidDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPrint.Text = Inventec.Common.Resource.Get.Value("FormBidDetail.cboPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("FormBidDetail.xtraTabPageMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                #region<GvMedicine>
                this.GvMedicine_GcStt.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //Bổ sung cột mã hoạt chất, tên hoạt chất, mã đường dùng, hàm lượng/nồng độ
                this.GvMedicine_GcActiveIngrBhytCode.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcActiveIngrBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcMedicineUseFormID.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcMedicineUseFormID.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcConcentra.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcConcentra.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.GvMedicine_GcAmount.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcImpPrice.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcBidNumber.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcBidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcBidNumOrder.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcBidNumOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //Số đăng ký
                this.GvMedicine_GcRegisterNumber.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcRegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.GvMedicine_GcSupplierName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcSupplierName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //Hãng sản xuất, Nước sản xuất
                this.GvMedicine_GcManufacturerName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcManufacturerName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcNationalName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcNationalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.GvMedicine_GcCreateTime.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcCreator.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcModifyTime.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMedicine_GcModifier.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMedicine_GcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                #endregion

                #region<GvMaterial>
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("FormBidDetail.xtraTabPageMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcStt.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcMaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcMaterialTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcMaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcMaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //Bổ sung cột mã hoạt chất, tên hoạt chất, mã đường dùng, hàm lượng/nồng độ
                this.GvMaterial_GcActiveIngrBhytCode.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcActiveIngrBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.GvMaterial_GcMaduongdung.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcMaduongdung.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcConcentra.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcConcentra.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.GvMaterial_GcAmount.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcImpPrice.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcSerViceUnitName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcSerViceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcBidNumber.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcBidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcBidNumOrder.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcBidNumOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //Số đăng ký
                this.GvMaterial_GcRegisterNumber.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcRegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.GvMaterial_GcSupplierName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcSupplierName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //Hãng sản xuất, Nước sản xuất
                this.GvMaterial_GcManufacturerName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcManufacturerName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcNationalName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcNationalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.GvMaterial_GcCreateTime.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcCreator.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcModifyTime.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvMaterial_GcModifier.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvMaterial_GcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                #endregion

                #region<GvBlood>
                this.xtraTabPageBlood.Text = Inventec.Common.Resource.Get.Value("FormBidDetail.xtraTabPageBlood.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.GvBlood_GcSTT.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcBloodTypeCode.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcBloodTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcBloodTypeName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcBloodTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //Bổ sung cột mã hoạt chất, tên hoạt chất, mã đường dùng, hàm lượng/nồng độ
                this.GvBlood_GcActiveIngrBhytCode.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcActiveIngrBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.GvBlood_GvBlood_GcMaduongdung.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcMaduongdung.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcConcentra.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcConcentra.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.GvBlood_GcAmount.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcImpPrice.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcBidNumber.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcBidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcBidNumOrder.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcBidNumOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //Số đăng ký
                this.GvBlood_GcRegisterNumber.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcRegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.GvBlood_GcSupplierName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcSupplierName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //Hãng sản xuất, Nước sản xuất
                this.GvBlood_GcManufacturerName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcManufacturerName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcNationalName.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcNationalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.GvBlood_GcCreateTime.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcCreator.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcModifyTime.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvBlood_GcModifier.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.GvBlood_GcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                #endregion

                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormBidDetail.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCPrint.Caption = Inventec.Common.Resource.Get.Value("FormBidDetail.bbtnRCPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormBidDetail.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormBidDetail_Load(object sender, EventArgs e)
        {
            try
            {
                this.dicMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().ToDictionary(o => o.ID, o => o);
                this.dicMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().ToDictionary(o => o.ID, o => o);
                //Gan ngon ngu
                //LoadKeysFromlanguage();
                SetCaptionByLanguageKey();

                //Load du lieu
                FillDataToGrid();

                //Hien thi tab chua du lieu
                ShowTab();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetPrintTypeToMps()
        {
            try
            {
                if (MPS.PrintConfig.PrintTypes == null || MPS.PrintConfig.PrintTypes.Count == 0)
                {
                    MPS.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                }
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
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //layout
                this.cboPrint.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__CBO_PRINT",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.xtraTabPageBlood.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__XTRA_TAB_BLOOD",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__XTRA_TAB_MATERIAL",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__XTRA_TAB_MEDICINE",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                //gridView
                this.GvBlood_GcSupplierName.Caption = this.GvMaterial_GcSupplierName.Caption = this.GvMedicine_GcSupplierName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_SUPPLIER_NAME",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcAmount.Caption = this.GvMaterial_GcAmount.Caption = this.GvMedicine_GcAmount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_AMOUNT",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcBidNumber.Caption = this.GvMaterial_GcBidNumber.Caption = this.GvMedicine_GcBidNumOrder.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_BID_NUMBER",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcBidNumOrder.Caption = this.GvMaterial_GcBidNumOrder.Caption = this.GvMedicine_GcBidNumOrder.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_BID_NUM_ORDER",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcBloodTypeCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GV_BLOOD__GC_BLOOD_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcBloodTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GV_BLOOD__GC_BLOOD_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcCreateTime.Caption = this.GvMaterial_GcCreateTime.Caption = this.GvMedicine_GcCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcCreator.Caption = this.GvMaterial_GcCreator.Caption = this.GvMedicine_GcCreator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_CREATOR",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcImpPrice.Caption = this.GvMaterial_GcImpPrice.Caption = this.GvMedicine_GcImpPrice.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_IMP_PRICE",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcModifier.Caption = this.GvMaterial_GcModifier.Caption = this.GvMedicine_GcModifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_MODIFIER",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcModifyTime.Caption = this.GvMaterial_GcModifyTime.Caption = this.GvMedicine_GcModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcServiceUnitName.Caption = this.GvMaterial_GcSerViceUnitName.Caption = this.GvMedicine_GcServiceUnitName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcSTT.Caption = this.GvMaterial_GcStt.Caption = this.GvMedicine_GcStt.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_STT",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);

                this.GvMaterial_GcMaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GV_MATERIAL__GC_MATERIAL_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvMaterial_GcMaterialTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GV_MATERIAL__GC_MATERIAL_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvMedicine_GcMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvMedicine_GcMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
                this.GvBlood_GcImpVatRatio.Caption = this.GvMaterial_GcImpVatRatio.Caption = this.GvMedicine_GcImpVatRatio.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__GC_IMP_VAT_RATIO",
                    Resources.ResourceLanguageManager.LanguageFormBidDetail,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                this.bidBloodTypes = new List<MOS.EFMODEL.DataModels.V_HIS_BID_BLOOD_TYPE>();
                this.bidMaterialTypes = new List<MOS.EFMODEL.DataModels.V_HIS_BID_MATERIAL_TYPE>();
                this.bidMedicineTypes = new List<MOS.EFMODEL.DataModels.V_HIS_BID_MEDICINE_TYPE>();
                FillDataToGridMedicine();
                FillDataToGridMaterial();
                FillDataToGridBlood();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void FillDataToGridMedicine()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.bidDetail == null)
                {
                    return;
                }
                MOS.Filter.HisBidMedicineTypeViewFilter filter = new MOS.Filter.HisBidMedicineTypeViewFilter();
                filter.BID_ID = this.bidDetail.ID;
                bidMedicineTypes = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_BID_MEDICINE_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_BID_MEDICINE_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (bidMedicineTypes != null && bidMedicineTypes.Count > 0)
                {
                    foreach (var item in bidMedicineTypes)
                    {
                        MPS.Processor.Mps000119.PDO.HisBidMetyADO bidAdo = new MPS.Processor.Mps000119.PDO.HisBidMetyADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_BID_MEDICINE_TYPE>(bidAdo, item);
                        bidAdo.TotalMoney = (item.AMOUNT) * (item.IMP_PRICE ?? 0) * ((item.IMP_VAT_RATIO ?? 0) + 1);
                        bidAdo.TypeName = Inventec.Common.Resource.Get.Value(
                            "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__TYPE_NAME_MEDICINE",
                            Resources.ResourceLanguageManager.LanguageFormBidDetail,
                            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        bidprintAdo.Add(bidAdo);
                    }
                }

                gridControlMedicine.DataSource = bidMedicineTypes;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bidMedicineTypes), bidMedicineTypes));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridMaterial()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.bidDetail == null)
                {
                    return;
                }
                MOS.Filter.HisBidMaterialTypeViewFilter filter = new MOS.Filter.HisBidMaterialTypeViewFilter();
                filter.BID_ID = this.bidDetail.ID;
                bidMaterialTypes = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_BID_MATERIAL_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_BID_MATERIAL_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (bidMaterialTypes != null && bidMaterialTypes.Count > 0)
                {
                    foreach (var item in bidMaterialTypes)
                    {
                        MPS.Processor.Mps000119.PDO.HisBidMetyADO bidAdo = new MPS.Processor.Mps000119.PDO.HisBidMetyADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_BID_MEDICINE_TYPE>(bidAdo, item);
                        if (item.MATERIAL_TYPE_NAME == null && item.MATERIAL_TYPE_MAP_NAME != null)
                        {
                            bidAdo.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_MAP_NAME;
                        }
                        else
                        {
                            bidAdo.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                        }

                        bidAdo.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                        bidAdo.TotalMoney = (item.AMOUNT) * (item.IMP_PRICE ?? 0) * ((item.IMP_VAT_RATIO ?? 0) + 1);
                        bidAdo.TypeName = Inventec.Common.Resource.Get.Value(
                            "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__TYPE_NAME_MATERIAL",
                            Resources.ResourceLanguageManager.LanguageFormBidDetail,
                            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        bidprintAdo.Add(bidAdo);
                    }
                }
                gridControlMaterial.DataSource = bidMaterialTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridBlood()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.bidDetail == null)
                {
                    return;
                }
                MOS.Filter.HisBidBloodTypeViewFilter filter = new MOS.Filter.HisBidBloodTypeViewFilter();
                filter.BID_ID = this.bidDetail.ID;
                bidBloodTypes = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_BID_BLOOD_TYPE>>(ApiConsumer.HisRequestUriStore.HIS_BID_BLOOD_TYPE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (bidBloodTypes != null && bidBloodTypes.Count > 0)
                {
                    foreach (var item in bidBloodTypes)
                    {
                        MPS.Processor.Mps000119.PDO.HisBidMetyADO bidAdo = new MPS.Processor.Mps000119.PDO.HisBidMetyADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_BID_MEDICINE_TYPE>(bidAdo, item);
                        bidAdo.MEDICINE_TYPE_CODE = item.BLOOD_TYPE_CODE;
                        bidAdo.MEDICINE_TYPE_NAME = item.BLOOD_TYPE_NAME;
                        bidAdo.TotalMoney = (item.AMOUNT) * (item.IMP_PRICE ?? 0) * ((item.IMP_VAT_RATIO ?? 0) + 1);
                        bidAdo.TypeName = Inventec.Common.Resource.Get.Value(
                            "IVT_LANGUAGE_KEY__FORM_BID_DETAIL__TYPE_NAME_BLOOD",
                            Resources.ResourceLanguageManager.LanguageFormBidDetail,
                            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        bidprintAdo.Add(bidAdo);
                    }
                }
                gridControlBlood.DataSource = bidBloodTypes;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowTab()
        {
            try
            {
                if (bidMedicineTypes != null && bidMedicineTypes.Count > 0) xtraTabControl1.SelectedTabPageIndex = 0;
                else if (bidMaterialTypes != null && bidMaterialTypes.Count > 0) xtraTabControl1.SelectedTabPageIndex = 1;
                else if (bidBloodTypes != null && bidBloodTypes.Count > 0) xtraTabControl1.SelectedTabPageIndex = 2;
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
                    MOS.EFMODEL.DataModels.V_HIS_BID_MEDICINE_TYPE data = (MOS.EFMODEL.DataModels.V_HIS_BID_MEDICINE_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            e.Value = data.IMP_VAT_RATIO;
                        }
                        if (e.Column.FieldName == "UseFormID")
                        {
                            if (this.dicMedicineType != null)
                            {
                                var medicineType = this.dicMedicineType[data.MEDICINE_TYPE_ID];
                                e.Value = medicineType != null ? medicineType.MEDICINE_USE_FORM_ID.ToString() : "";
                            }
                        }
                        if (e.Column.FieldName == "SERVICE_UNIT_NAME_STR")
                        {
                            if (this.dicMedicineType != null)
                            {
                                var medicineType = this.dicMedicineType[data.MEDICINE_TYPE_ID];

                                e.Value = medicineType.IMP_UNIT_ID.HasValue ? medicineType.IMP_UNIT_NAME : medicineType.SERVICE_UNIT_NAME;
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

        private void gridViewMaterial_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_BID_MATERIAL_TYPE data = (MOS.EFMODEL.DataModels.V_HIS_BID_MATERIAL_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            e.Value = data.IMP_VAT_RATIO;
                        }
                        if (e.Column.FieldName == "MATERIAL_TYPE_CODE_STR")
                        {
                            //if (data.MATERIAL_TYPE_ID == null)
                            //{
                                e.Value = data.MATERIAL_TYPE_CODE;
                            //}
                        }
                        if (e.Column.FieldName == "MATERIAL_TYPE_NAME_STR")
                        {
                          //  if (data.MATERIAL_TYPE_ID == null)
                           // {
                                e.Value = data.MATERIAL_TYPE_NAME;
                          //  }

                        }
                        if (e.Column.FieldName == "MATERIAL_TYPE_MAP_NAME")
                        {
                            if (data.MATERIAL_TYPE_MAP_NAME != null)
                            {
                                e.Value = data.MATERIAL_TYPE_MAP_NAME;
                            }
                          
                        }
                        if (e.Column.FieldName == "SERVICE_UNIT_NAME_STR")
                        {
                            if (this.dicMaterialType != null)

                            {
                                if (data.MATERIAL_TYPE_ID != null)
                                {
                                    var materialType = this.dicMaterialType[data.MATERIAL_TYPE_ID ?? 0];
                                    e.Value = materialType.IMP_UNIT_ID.HasValue ? materialType.IMP_UNIT_NAME : materialType.SERVICE_UNIT_NAME;
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

        private void gridViewBlood_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_BID_BLOOD_TYPE data = (MOS.EFMODEL.DataModels.V_HIS_BID_BLOOD_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            e.Value = data.IMP_VAT_RATIO;
                        }
                        if (e.Column.FieldName == "AMOUNT_DISPLAY")
                        {
                            e.Value = data.AMOUNT;
                        }
                        if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                        {
                            e.Value = data.IMP_PRICE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region in
        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = true;
            try
            {
                WaitingManager.Show();
                MPS.Processor.Mps000119.PDO.Mps000119PDO Mps000119PDO = new MPS.Processor.Mps000119.PDO.Mps000119PDO(bidDetail, bidprintAdo);
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000119PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000119PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null);
                }
                WaitingManager.Hide();
                MPS.MpsPrinter.Run(PrintData);
                //MPS.Core.Mps000119.Mps000119RDO mps119Rdo = new MPS.Core.Mps000119.Mps000119RDO(bidDetail, bidprintAdo);
                //if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                //{
                //    result = MPS.Printer.Run(printTypeCode, fileName, mps119Rdo, MPS.Printer.PreviewType.PrintNow);
                //}
                //else
                //    result = MPS.Printer.Run(printTypeCode, fileName, mps119Rdo);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void cboPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditor = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), PrintStoreLocation.ROOT_PATH);
                richEditor.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__ChiTietGoiThau__MPS000119, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cboPrint_Click(null, null);
        }
        #endregion
      
        private Dictionary<int, Rectangle> imageRectangles = new Dictionary<int, Rectangle>();
        private void gridViewMedicine_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {

                //if (bidMedicineTypes != null && bidMedicineTypes.Count > 0)
                //{
                //    var listData = bidMedicineTypes;
                //    foreach (var item in listData)
                //    {
                //        if (e.Column.FieldName == "ADJUST_AMOUNTT")
                //        {
                //            e.Graphics.FillRectangle(new SolidBrush(e.Appearance.BackColor), e.Bounds);
                //            //var itaem = bidMedicineTypes[e.RowHandle];
                //            // Lấy hình ảnh từ tài nguyên
                //            Image image = HIS.Desktop.Plugins.BidDetail.Properties.Resources.dieutiet;

                //            // Tính toán vị trí và kích thước của hình ảnh
                //            int imageWidth = image.Width;
                //            int imageHeight = image.Height;
                //            int imageX = e.Bounds.Right - imageWidth; // Đặt ảnh bên phải của ô
                //            int imageY = e.Bounds.Y + (e.Bounds.Height - imageHeight) / 2; // Đặt ảnh ở giữa theo chiều dọc

                //            // Vẽ hình ảnh vào ô trước khi vẽ văn bản
                //            e.Graphics.DrawImage(image, imageX, imageY, imageWidth, imageHeight);

                //            // Giải phóng tài nguyên hình ảnh sau khi đã sử dụng
                //            image.Dispose();
                //            Rectangle imageRect = new Rectangle(imageX, imageY, imageWidth, imageHeight);
                //            imageRectangles[e.RowHandle] = imageRect;
                //            //string text = item.ADJUST_AMOUNT.ToString();
                //            adjustAmount = item.ADJUST_AMOUNT;

                //            string text = adjustAmount.HasValue ? adjustAmount.Value.ToString("N0") : "";
                //            SizeF textSize = e.Graphics.MeasureString(text, e.Appearance.Font);
                //            int textX = e.Bounds.Left; // Vị trí X của văn bản
                //            int textY = e.Bounds.Y + (e.Bounds.Height - (int)textSize.Height) / 2; // Vị trí Y của văn bản
                //            Rectangle textRect = new Rectangle(textX, textY, (int)textSize.Width, (int)textSize.Height);

                //            e.Appearance.DrawString(e.Cache, text, textRect);
                //            e.Handled = true;

                //        }
                //    }

                //}
                if (bidMedicineTypes != null && bidMedicineTypes.Count > 0)
                {
                    // Kiểm tra xem cột hiện tại có phải là "ADJUST_AMOUNTT" hay không
                    if (e.Column.FieldName == "ADJUST_AMOUNTT")
                    {
                        // Lấy đối tượng dữ liệu tương ứng với dòng hiện tại
                        var item = bidMedicineTypes[e.RowHandle];

                        // Xóa văn bản và hình ảnh cũ trong ô hiện tại
                        e.Graphics.FillRectangle(new SolidBrush(e.Appearance.BackColor), e.Bounds);

                        // Lấy hình ảnh từ tài nguyên
                        Image image = HIS.Desktop.Plugins.BidDetail.Properties.Resources.dieutiet;

                        // Tính toán vị trí và kích thước của hình ảnh
                        int imageWidth = image.Width;
                        int imageHeight = image.Height;
                        int imageX = e.Bounds.Right - imageWidth; // Đặt ảnh bên phải của ô
                        int imageY = e.Bounds.Y + (e.Bounds.Height - imageHeight) / 2; // Đặt ảnh ở giữa theo chiều dọc

                        // Vẽ hình ảnh vào ô trước khi vẽ văn bản
                        e.Graphics.DrawImage(image, imageX, imageY, imageWidth, imageHeight);

                        // Giải phóng tài nguyên hình ảnh sau khi đã sử dụng
                        image.Dispose();

                        Rectangle imageRect = new Rectangle(imageX, imageY, imageWidth, imageHeight);
                        imageRectangles[e.RowHandle] = imageRect;

                        // Vẽ văn bản vào ô hiện tại
                        var adjustAmount = item.ADJUST_AMOUNT;
                        string text = adjustAmount.HasValue ? adjustAmount.Value.ToString("N0") : "";
                        SizeF textSize = e.Graphics.MeasureString(text, e.Appearance.Font);
                        int textX = e.Bounds.Left; // Vị trí X của văn bản
                        int textY = e.Bounds.Y + (e.Bounds.Height - (int)textSize.Height) / 2; // Vị trí Y của văn bản
                        Rectangle textRect = new Rectangle(textX, textY, (int)textSize.Width, (int)textSize.Height);

                        // Vẽ văn bản sau khi đã vẽ hình ảnh
                        e.Appearance.DrawString(e.Cache, text, textRect);

                        e.Handled = true;
                    }
                }

                

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridViewMedicine_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
                Rectangle imageRect;
                var rowMdc = (MOS.EFMODEL.DataModels.V_HIS_BID_MEDICINE_TYPE)gridViewMedicine.GetFocusedRow();
                if (rowMdc.IS_ACTIVE == 0)
                {
                    GvMedicine_GcActiveIngrBhytCode.OptionsColumn.AllowEdit = false;
                    GvMedicine_GcActiveIngrBhytName.OptionsColumn.AllowEdit = false;
                    GvMedicine_GcMedicineUseFormID.OptionsColumn.AllowEdit = false;
                    GvMedicine_GcConcentra.OptionsColumn.AllowEdit = false;
                    GvMedicine_GcImpVatRatio.OptionsColumn.AllowEdit = false;
                    GvMedicine_GcRegisterNumber.OptionsColumn.AllowEdit = false;
                    GvMedicine_GcSupplierName.OptionsColumn.AllowEdit = false;
                    GvMedicine_GcManufacturerName.OptionsColumn.AllowEdit = false;
                    GvMedicine_GcNationalName.OptionsColumn.AllowEdit = false;
                }
                else
                {
                    GvMedicine_GcActiveIngrBhytCode.OptionsColumn.AllowEdit = true;
                    GvMedicine_GcActiveIngrBhytName.OptionsColumn.AllowEdit = true;
                    GvMedicine_GcMedicineUseFormID.OptionsColumn.AllowEdit = true;
                    GvMedicine_GcConcentra.OptionsColumn.AllowEdit = true;
                    GvMedicine_GcImpVatRatio.OptionsColumn.AllowEdit = true;
                    GvMedicine_GcRegisterNumber.OptionsColumn.AllowEdit = true;
                    GvMedicine_GcSupplierName.OptionsColumn.AllowEdit = true;
                    GvMedicine_GcManufacturerName.OptionsColumn.AllowEdit = true;
                    GvMedicine_GcNationalName.OptionsColumn.AllowEdit = true;
                }
                  if (hitInfo.InRowCell && hitInfo.Column.FieldName == "ADJUST_AMOUNTT")
                {
                    if (imageRectangles.TryGetValue(hitInfo.RowHandle, out imageRect))
                    {

                        if (imageRect.Contains(e.Location))
                        {
                            if (rowMdc == null || rowMdc.IS_ACTIVE == 0)
                            {
                                return;
                            }
                           
                            MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE dataMdc = new MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE();

                            List<object> listArgs = new List<object>();
                            if (rowMdc != null)
                            {
                                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE>(dataMdc, rowMdc);
                                listArgs.Add(dataMdc);
                                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)GetAdjustAmount);
                            }
                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.BidRegulation", roomId, roomTypeId, listArgs);
                            FillDataToGrid();
                        }
                    }
                }
              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void GetAdjustAmount(object data)
        {
            adjustAmount = data as decimal?;
        }

        private void gridViewMaterial_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (bidMaterialTypes != null && bidMaterialTypes.Count > 0)
                {
                    // Kiểm tra xem cột hiện tại có phải là "ADJUST_AMOUNTT" hay không
                    if (e.Column.FieldName == "ADJUST_AMOUNTT")
                    {
                        // Lấy đối tượng dữ liệu tương ứng với dòng hiện tại
                        var item = bidMaterialTypes[e.RowHandle];

                        // Xóa văn bản và hình ảnh cũ trong ô hiện tại
                        e.Graphics.FillRectangle(new SolidBrush(e.Appearance.BackColor), e.Bounds);

                        // Lấy hình ảnh từ tài nguyên
                        Image image = HIS.Desktop.Plugins.BidDetail.Properties.Resources.dieutiet;

                        // Tính toán vị trí và kích thước của hình ảnh
                        int imageWidth = image.Width;
                        int imageHeight = image.Height;
                        int imageX = e.Bounds.Right - imageWidth; // Đặt ảnh bên phải của ô
                        int imageY = e.Bounds.Y + (e.Bounds.Height - imageHeight) / 2; // Đặt ảnh ở giữa theo chiều dọc

                        // Vẽ hình ảnh vào ô trước khi vẽ văn bản
                        e.Graphics.DrawImage(image, imageX, imageY, imageWidth, imageHeight);

                        // Giải phóng tài nguyên hình ảnh sau khi đã sử dụng
                        image.Dispose();

                        Rectangle imageRect = new Rectangle(imageX, imageY, imageWidth, imageHeight);
                        imageRectangles[e.RowHandle] = imageRect;

                        // Vẽ văn bản vào ô hiện tại
                        var adjustAmount = item.ADJUST_AMOUNT;
                        string text = adjustAmount.HasValue ? adjustAmount.Value.ToString("N0") : "";
                        SizeF textSize = e.Graphics.MeasureString(text, e.Appearance.Font);
                        int textX = e.Bounds.Left; // Vị trí X của văn bản
                        int textY = e.Bounds.Y + (e.Bounds.Height - (int)textSize.Height) / 2; // Vị trí Y của văn bản
                        Rectangle textRect = new Rectangle(textX, textY, (int)textSize.Width, (int)textSize.Height);

                        // Vẽ văn bản sau khi đã vẽ hình ảnh
                        e.Appearance.DrawString(e.Cache, text, textRect);

                        e.Handled = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
                Rectangle imageRect;
                var rowMtr = (MOS.EFMODEL.DataModels.V_HIS_BID_MATERIAL_TYPE)gridViewMaterial.GetFocusedRow();
                if (rowMtr.IS_ACTIVE == 0)
                {
                    GvMaterial_GcMaterialTypeCode.OptionsColumn.AllowEdit = false;
                    GvMaterial_GcMaterialTypeName.OptionsColumn.AllowEdit = false;
                    gridColumn3.OptionsColumn.AllowEdit = false;
                    GvMaterial_GcConcentra.OptionsColumn.AllowEdit = false;
                    GvMaterial_GcImpVatRatio.OptionsColumn.AllowEdit = false;
                    gridColumn7.OptionsColumn.AllowEdit = false;
                    GvMaterial_GcSupplierName.OptionsColumn.AllowEdit = false;
                    GvMaterial_GcManufacturerName.OptionsColumn.AllowEdit = false;
                    GvMaterial_GcNationalName.OptionsColumn.AllowEdit = false;
                }
                else
                {
                    GvMaterial_GcMaterialTypeCode.OptionsColumn.AllowEdit = true;
                    GvMaterial_GcMaterialTypeName.OptionsColumn.AllowEdit = true;
                    gridColumn3.OptionsColumn.AllowEdit = true;
                    GvMaterial_GcConcentra.OptionsColumn.AllowEdit = true;
                    GvMaterial_GcImpVatRatio.OptionsColumn.AllowEdit = true;
                    gridColumn7.OptionsColumn.AllowEdit = true;
                    GvMaterial_GcSupplierName.OptionsColumn.AllowEdit = true;
                    GvMaterial_GcManufacturerName.OptionsColumn.AllowEdit = true;
                    GvMaterial_GcNationalName.OptionsColumn.AllowEdit = true;
                }
                if (hitInfo.InRowCell && hitInfo.Column.FieldName == "ADJUST_AMOUNTT")
                {
                    if (imageRectangles.TryGetValue(hitInfo.RowHandle, out imageRect))
                    {

                        if (imageRect.Contains(e.Location))
                        {
                           
                            if (rowMtr == null || rowMtr.IS_ACTIVE == 0)
                            {
                                return;
                            }
                            MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE dataMtr = new MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE();

                            List<object> listArgs = new List<object>();
                          
                            if (rowMtr != null)
                            {
                                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE>(dataMtr, rowMtr);
                                listArgs.Add(dataMtr);
                                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)GetAdjustAmount);
                            }
                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.BidRegulation", roomId, roomTypeId, listArgs);
                            FillDataToGrid();
                        }
                    }
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
                if (e.RowHandle >= 0)
                {
                    V_HIS_BID_MEDICINE_TYPE data = (V_HIS_BID_MEDICINE_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__FALSE ? btnLock : btnUnLock);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            CommonParam param = new CommonParam();
            HIS_BID_MEDICINE_TYPE success = new HIS_BID_MEDICINE_TYPE();
            bool notHandler = false;
            try
            {
                if (bidDetail.IS_ACTIVE == 1)
                {
                    V_HIS_BID_MEDICINE_TYPE dataRow = (V_HIS_BID_MEDICINE_TYPE)gridViewMedicine.GetFocusedRow();
                    if (MessageBox.Show("Bạn có chắc muốn bỏ khóa dữ liệu?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        HIS_BID_MEDICINE_TYPE data = new HIS_BID_MEDICINE_TYPE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE>(data, dataRow);
                        data.ID = dataRow.ID;
                        data.IS_ACTIVE = 1;
                        WaitingManager.Show();
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BID_MEDICINE_TYPE>("api/HisBidMedicineType/ChangeLock", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                        WaitingManager.Hide();
                        if (success != null)
                        {
                            notHandler = true;
                            FillDataToGrid();
                        }
                        MessageManager.Show(this, param, notHandler);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Thầu {0} đang bị khóa, không cho phép sửa chi tiết thầu", bidDetail.BID_NUMBER));
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }

        private void btnUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_BID_MEDICINE_TYPE success = new HIS_BID_MEDICINE_TYPE();
            bool notHandler = false;
            try
            {
                if (bidDetail.IS_ACTIVE == 1)
                {
                    V_HIS_BID_MEDICINE_TYPE dataRow = (V_HIS_BID_MEDICINE_TYPE)gridViewMedicine.GetFocusedRow();
                    if (MessageBox.Show("Bạn có chắc muốn khóa khóa dữ liệu?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        HIS_BID_MEDICINE_TYPE data = new HIS_BID_MEDICINE_TYPE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE>(data, dataRow);
                        data.ID = dataRow.ID;
                        data.IS_ACTIVE = 0;
                        WaitingManager.Show();
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BID_MEDICINE_TYPE>("api/HisBidMedicineType/ChangeLock", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                        //api/HisBidMedicineType/Update
                        WaitingManager.Hide();
                        if (success != null)
                        {
                            success.IS_ACTIVE = isActive;
                            notHandler = true;
                            FillDataToGrid();
                        }
                        MessageManager.Show(this, param, notHandler);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Thầu {0} đang bị khóa, không cho phép sửa chi tiết thầu", bidDetail.BID_NUMBER));
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }

        private void gridViewMaterial_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_BID_MATERIAL_TYPE data = (V_HIS_BID_MATERIAL_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__FALSE ? btnLockMaterial : btnUnLockMaterial);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLockMaterial_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_BID_MATERIAL_TYPE success = new HIS_BID_MATERIAL_TYPE();
            bool notHandler = false;
            try
            {
                if (bidDetail.IS_ACTIVE == 1)
                {
                    V_HIS_BID_MATERIAL_TYPE dataRow = (V_HIS_BID_MATERIAL_TYPE)gridViewMaterial.GetFocusedRow();
                    if (MessageBox.Show("Bạn có chắc muốn bỏ khóa dữ liệu", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        HIS_BID_MATERIAL_TYPE data = new HIS_BID_MATERIAL_TYPE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE>(data, dataRow);
                        data.ID = dataRow.ID;
                        data.IS_ACTIVE = 1;
                        WaitingManager.Show();
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BID_MATERIAL_TYPE>("api/HisBidMaterialType/ChangeLock", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                        WaitingManager.Hide();
                        if (success != null)
                        {
                            notHandler = true;
                            FillDataToGrid();
                        }
                        MessageManager.Show(this, param, notHandler);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Thầu {0} đang bị khóa, không cho phép sửa chi tiết thầu", bidDetail.BID_NUMBER));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnLockMaterial_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_BID_MATERIAL_TYPE success = new HIS_BID_MATERIAL_TYPE();
            bool notHandler = false;
            try
            {
                if (bidDetail.IS_ACTIVE == 1)
                {
                    V_HIS_BID_MATERIAL_TYPE dataRow = (V_HIS_BID_MATERIAL_TYPE)gridViewMaterial.GetFocusedRow();
                    if (MessageBox.Show("Bạn có chắc muốn khóa khóa dữ liệu?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        HIS_BID_MATERIAL_TYPE data = new HIS_BID_MATERIAL_TYPE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE>(data, dataRow);
                        data.ID = dataRow.ID;
                        data.IS_ACTIVE = 0;
                        WaitingManager.Show();
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BID_MATERIAL_TYPE>("api/HisBidMaterialType/ChangeLock", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                        WaitingManager.Hide();
                        if (success != null)
                        {
                            notHandler = true;
                            FillDataToGrid();
                        }
                        MessageManager.Show(this, param, notHandler);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Thầu {0} đang bị khóa, không cho phép sửa chi tiết thầu", bidDetail.BID_NUMBER));
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


       
        #region Public method

        #endregion

    }
}
