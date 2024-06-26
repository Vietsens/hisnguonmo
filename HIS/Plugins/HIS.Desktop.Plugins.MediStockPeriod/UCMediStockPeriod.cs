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
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.MediStockPeriod.CreatePeriod;
using HIS.UC.HisMestPeriodMety;
using HIS.UC.HisMestPeriodMaterial;
using Inventec.Desktop.Common.Message;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.BackendData;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.UC.HisMestPeriodBlty;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.Plugins.MediStockPeriod.ADO;
using MOS.SDO;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.MediStockPeriod.Popup;
using System.IO;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.Utils;

namespace HIS.Desktop.Plugins.MediStockPeriod
{
    public partial class UCMediStockPeriod : UserControl
    {
        #region Declare variables
        //internal V_HIS_MEDI_STOCK_PERIOD HisMediStockPeriod;
        internal List<HisMestPeriodMediAdo> ListMestPeriodMety { get; set; }
        internal List<HisMestPeriodMateAdo> ListMestPeriodMaty { get; set; }
        internal List<HisMestPeriodBloodAdo> ListMestPeriodBloods { get; set; }
        internal List<MediStockPeriodADO> ListMediStockCheck { get; set; }

        internal List<MediStockPeriodADO> ListMediStockDataSource { get; set; }
        internal List<MediStockADO> MediStock__Seleced { get; set; }

        HisMestPeriodMetyProcessor hisMestPeriodMetyProcessor;
        HisMestPeriodMaterialProcessor hisMestPeriodMatyProcessor;
        HisMestPeriodBltyProcessor hisMestPeriodBltyProcessor;
        List<MediStockADO> _MediStockProcess = new List<MediStockADO>();

        UserControl ucMetyInfo;
        UserControl ucMatyInfo;
        UserControl ucBltyInfo;
        Inventec.Desktop.Common.Modules.Module currentModule;
        #endregion

        #region Construct - OnLoad
        public UCMediStockPeriod()
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                InitMedicineTree();
                InitMaterialTree();
                InitBloodTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCMediStockPeriod(Inventec.Desktop.Common.Modules.Module _module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = _module;
                //InitMedicineTree();
                //InitMaterialTree();
                //InitBloodTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitMedicineTree()
        {
            try
            {
                hisMestPeriodMetyProcessor = new HisMestPeriodMetyProcessor();
                HisMestPeriodMetyInitADO ado = new HisMestPeriodMetyInitADO();
                ado.HisMestPeriodMetyColumns = new List<HisMestPeriodMetyColumn>();
                ado.HisMestPeriodMetyNodeCellStyle = medicineType_NodeCellStyle;
                ado.IsShowSearchPanel = true;
                //ado.HisMestPeriodMety_CustomUnboundColumnData = HisMestPeriodMety_CustomUnboundColumnData;

                //Column mã
                HisMestPeriodMetyColumn mediTypeCodeNameCol = new HisMestPeriodMetyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_TYPE_CODE", 90, false);
                mediTypeCodeNameCol.VisibleIndex = 0;
                ado.HisMestPeriodMetyColumns.Add(mediTypeCodeNameCol);
                //Column tên
                HisMestPeriodMetyColumn mediTypeNameCol = new HisMestPeriodMetyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_TYPE_NAME", 190, false);
                mediTypeNameCol.VisibleIndex = 1;
                ado.HisMestPeriodMetyColumns.Add(mediTypeNameCol);
                //Column đơn vị tính
                HisMestPeriodMetyColumn serviceUnitNameCol = new HisMestPeriodMetyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERVICE_UNIT_NAME", 60, false);
                serviceUnitNameCol.VisibleIndex = 2;
                ado.HisMestPeriodMetyColumns.Add(serviceUnitNameCol);
                //Column số lượng đầu kỳ
                HisMestPeriodMetyColumn beginAmountCol = new HisMestPeriodMetyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_BEGIN_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BEGIN_AMOUNT", 100, false);
                beginAmountCol.VisibleIndex = 3;
                beginAmountCol.Format = new DevExpress.Utils.FormatInfo();
                beginAmountCol.Format.FormatString = "#,##0.00";
                beginAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodMetyColumns.Add(beginAmountCol);
                //Column số lượng nhập
                HisMestPeriodMetyColumn inAmoutCol = new HisMestPeriodMetyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_IN_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IN_AMOUNT", 80, false);
                inAmoutCol.VisibleIndex = 4;
                inAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                inAmoutCol.Format.FormatString = "#,##0.00";
                inAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodMetyColumns.Add(inAmoutCol);
                //Column số lượng xuất
                HisMestPeriodMetyColumn outAmoutCol = new HisMestPeriodMetyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_OUT_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "OUT_AMOUNT", 80, false);
                outAmoutCol.VisibleIndex = 5;
                outAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                outAmoutCol.Format.FormatString = "#,##0.00";
                outAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodMetyColumns.Add(outAmoutCol);
                //Column số lượng cuối kỳ
                HisMestPeriodMetyColumn virEndAmoutCol = new HisMestPeriodMetyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_VIR_END_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_END_AMOUNT", 100, false);
                virEndAmoutCol.VisibleIndex = 6;
                virEndAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                virEndAmoutCol.Format.FormatString = "#,##0.00";
                virEndAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodMetyColumns.Add(virEndAmoutCol);
                //Column số lượng tồn kho
                HisMestPeriodMetyColumn inventoryAmoutCol = new HisMestPeriodMetyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_INVENTORY_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "INVENTORY_AMOUNT", 100, false);
                inventoryAmoutCol.VisibleIndex = 7;
                inventoryAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                inventoryAmoutCol.Format.FormatString = "#,##0.00";
                inventoryAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodMetyColumns.Add(inventoryAmoutCol);
                //Column nước sản xuất
                HisMestPeriodMetyColumn nationalNameCol = new HisMestPeriodMetyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_NATIONAL_NAME", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "NATIONAL_NAME", 80, false);
                nationalNameCol.VisibleIndex = 8;
                ado.HisMestPeriodMetyColumns.Add(nationalNameCol);
                //Column hãng sản xuất
                HisMestPeriodMetyColumn manufacturerNameCol = new HisMestPeriodMetyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_MANUFACTURER_NAME", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MANUFACTURER_NAME", 80, false);
                manufacturerNameCol.VisibleIndex = 9;
                ado.HisMestPeriodMetyColumns.Add(manufacturerNameCol);

                this.ucMetyInfo = (UserControl)hisMestPeriodMetyProcessor.Run(ado);

                if (this.ucMetyInfo != null)
                {
                    // this.panelControlMetyAndMaty.Controls.Add(this.ucMetyInfo);
                    this.ucMetyInfo.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitMaterialTree()
        {
            try
            {
                hisMestPeriodMatyProcessor = new HisMestPeriodMaterialProcessor();
                HisMestPeriodMaterialInitADO ado = new HisMestPeriodMaterialInitADO();
                ado.HisMestPeriodMaterialColumns = new List<HisMestPeriodMaterialColumn>();
                ado.HisMestPeriodMaterialNodeCellStyle = materialType_NodeCellStyle;
                ado.IsShowSearchPanel = true;
                //ado.HisMestPeriodMety_CustomUnboundColumnData = HisMestPeriodMety_CustomUnboundColumnData;

                //Column mã
                HisMestPeriodMaterialColumn mediTypeCodeNameCol = new HisMestPeriodMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MATERIAL_TYPE_CODE", 90, false);
                mediTypeCodeNameCol.VisibleIndex = 0;
                ado.HisMestPeriodMaterialColumns.Add(mediTypeCodeNameCol);
                //Column tên
                HisMestPeriodMaterialColumn mediTypeNameCol = new HisMestPeriodMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MATERIAL_TYPE_NAME", 190, false);
                mediTypeNameCol.VisibleIndex = 1;
                ado.HisMestPeriodMaterialColumns.Add(mediTypeNameCol);
                //Column đơn vị tính
                HisMestPeriodMaterialColumn serviceUnitNameCol = new HisMestPeriodMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERVICE_UNIT_NAME", 60, false);
                serviceUnitNameCol.VisibleIndex = 2;
                ado.HisMestPeriodMaterialColumns.Add(serviceUnitNameCol);
                //Column số lượng đầu kỳ
                HisMestPeriodMaterialColumn beginAmountCol = new HisMestPeriodMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_BEGIN_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BEGIN_AMOUNT", 100, false);
                beginAmountCol.VisibleIndex = 3;
                beginAmountCol.Format = new DevExpress.Utils.FormatInfo();
                beginAmountCol.Format.FormatString = "#,##0.00";
                beginAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodMaterialColumns.Add(beginAmountCol);
                //Column số lượng nhập
                HisMestPeriodMaterialColumn inAmoutCol = new HisMestPeriodMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_IN_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IN_AMOUNT", 80, false);
                inAmoutCol.VisibleIndex = 4;
                inAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                inAmoutCol.Format.FormatString = "#,##0.00";
                inAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodMaterialColumns.Add(inAmoutCol);
                //Column số lượng xuất
                HisMestPeriodMaterialColumn outAmoutCol = new HisMestPeriodMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_OUT_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "OUT_AMOUNT", 80, false);
                outAmoutCol.VisibleIndex = 5;
                outAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                outAmoutCol.Format.FormatString = "#,##0.00";
                outAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodMaterialColumns.Add(outAmoutCol);
                //Column số lượng cuối kỳ
                HisMestPeriodMaterialColumn virEndAmoutCol = new HisMestPeriodMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_VIR_END_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_END_AMOUNT", 100, false);
                virEndAmoutCol.VisibleIndex = 6;
                virEndAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                virEndAmoutCol.Format.FormatString = "#,##0.00";
                virEndAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodMaterialColumns.Add(virEndAmoutCol);
                //Column số lượng tồn kho
                HisMestPeriodMaterialColumn inventoryAmoutCol = new HisMestPeriodMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_INVENTORY_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "INVENTORY_AMOUNT", 100, false);
                inventoryAmoutCol.VisibleIndex = 7;
                inventoryAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                inventoryAmoutCol.Format.FormatString = "#,##0.00";
                inventoryAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodMaterialColumns.Add(inventoryAmoutCol);
                //Column nước sản xuất
                HisMestPeriodMaterialColumn nationalNameCol = new HisMestPeriodMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_NATIONAL_NAME", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "NATIONAL_NAME", 80, false);
                nationalNameCol.VisibleIndex = 8;
                ado.HisMestPeriodMaterialColumns.Add(nationalNameCol);
                //Column hãng sản xuất
                HisMestPeriodMaterialColumn manufacturerNameCol = new HisMestPeriodMaterialColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_MANUFACTURER_NAME", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MANUFACTURER_NAME", 80, false);
                manufacturerNameCol.VisibleIndex = 9;
                ado.HisMestPeriodMaterialColumns.Add(manufacturerNameCol);

                this.ucMatyInfo = (UserControl)hisMestPeriodMatyProcessor.Run(ado);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitBloodTree()
        {
            try
            {
                hisMestPeriodBltyProcessor = new HisMestPeriodBltyProcessor();
                HisMestPeriodBltyInitADO ado = new HisMestPeriodBltyInitADO();
                ado.HisMestPeriodBltyColumns = new List<HisMestPeriodBltyColumn>();
                ado.HisMestPeriodBltyNodeCellStyle = bltyType_NodeCellStyle;
                ado.IsShowSearchPanel = true;

                //Column mã
                HisMestPeriodBltyColumn mediTypeCodeNameCol = new HisMestPeriodBltyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BLOOD_TYPE_CODE", 90, false);
                mediTypeCodeNameCol.VisibleIndex = 0;
                ado.HisMestPeriodBltyColumns.Add(mediTypeCodeNameCol);
                //Column tên
                HisMestPeriodBltyColumn mediTypeNameCol = new HisMestPeriodBltyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BLOOD_TYPE_NAME", 190, false);
                mediTypeNameCol.VisibleIndex = 1;
                ado.HisMestPeriodBltyColumns.Add(mediTypeNameCol);
                //Column đơn vị tính
                HisMestPeriodBltyColumn serviceUnitNameCol = new HisMestPeriodBltyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERVICE_UNIT_NAME", 60, false);
                serviceUnitNameCol.VisibleIndex = 2;
                ado.HisMestPeriodBltyColumns.Add(serviceUnitNameCol);
                //Column dung tích
                HisMestPeriodBltyColumn volumeCol = new HisMestPeriodBltyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_VOLUME", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VOLUME", 100, false);
                volumeCol.VisibleIndex = 3;
                volumeCol.Format = new DevExpress.Utils.FormatInfo();
                volumeCol.Format.FormatString = "#,##0.00";
                volumeCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodBltyColumns.Add(volumeCol);
                //Column số lượng đầu kỳ
                HisMestPeriodBltyColumn beginAmountCol = new HisMestPeriodBltyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_BEGIN_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BEGIN_AMOUNT", 100, false);
                beginAmountCol.VisibleIndex = 4;
                beginAmountCol.Format = new DevExpress.Utils.FormatInfo();
                beginAmountCol.Format.FormatString = "#,##0.00";
                beginAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodBltyColumns.Add(beginAmountCol);
                //Column số lượng nhập
                HisMestPeriodBltyColumn inAmoutCol = new HisMestPeriodBltyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_IN_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IN_AMOUNT", 80, false);
                inAmoutCol.VisibleIndex = 5;
                inAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                inAmoutCol.Format.FormatString = "#,##0.00";
                inAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodBltyColumns.Add(inAmoutCol);
                //Column số lượng xuất
                HisMestPeriodBltyColumn outAmoutCol = new HisMestPeriodBltyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_OUT_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "OUT_AMOUNT", 80, false);
                outAmoutCol.VisibleIndex = 6;
                outAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                outAmoutCol.Format.FormatString = "#,##0.00";
                outAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodBltyColumns.Add(outAmoutCol);
                //Column số lượng cuối kỳ
                HisMestPeriodBltyColumn virEndAmoutCol = new HisMestPeriodBltyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_VIR_END_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_END_AMOUNT", 100, false);
                virEndAmoutCol.VisibleIndex = 7;
                virEndAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                virEndAmoutCol.Format.FormatString = "#,##0.00";
                virEndAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodBltyColumns.Add(virEndAmoutCol);
                //Column số lượng tồn kho
                HisMestPeriodBltyColumn inventoryAmoutCol = new HisMestPeriodBltyColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_PERIOD__MEDICINE_IN_STOCK__COLUMN_INVENTORY_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockPeriod, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "INVENTORY_AMOUNT", 100, false);
                inventoryAmoutCol.VisibleIndex = 8;
                inventoryAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                inventoryAmoutCol.Format.FormatString = "#,##0.00";
                inventoryAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMestPeriodBltyColumns.Add(inventoryAmoutCol);

                this.ucBltyInfo = (UserControl)hisMestPeriodBltyProcessor.Run(ado);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCMediStockPeriod_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoadDataToComboMediStock();
                LoadDataMediStockPeriod();
                BtnDeletePeriod.Enabled = false;
                cboMediStock.Focus();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MediStockPeriod.Resources.Lang", typeof(HIS.Desktop.Plugins.MediStockPeriod.UCMediStockPeriod).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKiemKe.Text = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.btnKiemKe.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnCreatePeriod.Text = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.btnCreatePeriod.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkVatTu.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.chkVatTu.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                // this.chkThuoc.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.chkThuoc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkKhongHienDongTangGiam.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.chkKhongHienDongTangGiam.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkKhongHienDongHet.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.chkKhongHienDongHet.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.cboMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSTT.Caption = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.gridColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColMediStockPeriodName.Caption = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.gridColMediStockPeriodName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColCountImpMest.Caption = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.gridColCountImpMest.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColCountExpMest.Caption = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.gridColCountExpMest.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColCoutMediType.Caption = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.gridColCoutMediType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColCountMateType.Caption = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.gridColCountMateType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChooStock.Text = Inventec.Common.Resource.Get.Value("UCMediStockPeriod.lciChooStock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataCboMediStock(DevExpress.XtraEditors.GridLookUpEdit cboMediStock, object data)
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 2));
                //columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 300, 3));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 400);
                //ControlEditorLoader.Load(cboMediStock, data, controlEditorADO);

                cboMediStock.Properties.View.Columns.Clear();
                InitCheck(cboMediStock, SelectionGrid__MediStock);
                InitCombo(cboMediStock, data, "MEDI_STOCK_NAME", "ID");
                TxtKeyWord.Focus();
                TxtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__MediStock(object sender, EventArgs e)
        {
            try
            {
                MediStock__Seleced = new List<MediStockADO>();
                foreach (MediStockADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        MediStock__Seleced.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 350;
                col2.Caption = "Tất cả";
                col2.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                cbo.Properties.PopupFormWidth = 450;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                cbo.Properties.View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.True;

                //GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //{
                //    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboMediStock()
        {
            try
            {
                _MediStockProcess = new List<MediStockADO>();
                var datas = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(p => p.LOGINNAME.Trim() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim()).ToList();
                List<V_HIS_MEDI_STOCK> _MediStockWithUsers = new List<V_HIS_MEDI_STOCK>();
                List<V_HIS_MEDI_STOCK> _MediStockAlls = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => p.IS_ACTIVE == 1).OrderBy(p => p.MEDI_STOCK_NAME).ToList();
                if (datas != null && datas.Count > 0)
                {
                    List<long> roomIds = datas.Select(p => p.ROOM_ID).ToList();
                    _MediStockWithUsers = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => roomIds.Contains(p.ROOM_ID) && p.IS_ACTIVE == 1).OrderBy(p => p.MEDI_STOCK_NAME).ToList();
                }
                if (chkAllMedistock.Checked)
                {
                    foreach (var item in _MediStockAlls)
                    {
                        MediStockADO medi = new MediStockADO(item);
                        medi.IsOutUser = null;
                        var checkMedi = _MediStockWithUsers != null && _MediStockWithUsers.Count() > 0 ? _MediStockWithUsers.FirstOrDefault(o => o.ID == item.ID) : null;
                        if (checkMedi == null)
                            medi.IsOutUser = false;
                        else
                            medi.IsOutUser = true;

                        _MediStockProcess.Add(medi);
                    }
                }
                else if (_MediStockWithUsers != null && _MediStockWithUsers.Count > 0)
                {
                    if (_MediStockWithUsers != null && _MediStockWithUsers.Count > 0)
                    {
                        foreach (var item in _MediStockWithUsers)
                        {
                            MediStockADO medi = new MediStockADO(item);
                            medi.IsOutUser = true;
                            _MediStockProcess.Add(medi);
                        }
                    }
                }

                // var _MediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => p.DEPARTMENT_ID == _WorkPlace.DepartmentId).OrderBy(p => p.MEDI_STOCK_NAME).ToList();

                if (_MediStockProcess != null && _MediStockProcess.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("_MediStockProcess count: " + _MediStockProcess.Count());
                    FillDataCboMediStock(this.cboMediStock, _MediStockProcess);
                    var data = _MediStockProcess.FirstOrDefault(p => p.ROOM_ID == this.currentModule.RoomId);
                    if (data != null)
                    {
                        //this.cboMediStock.EditValue = data.ID;
                        //this.txtCode.Text = data.MEDI_STOCK_CODE;
                        GridCheckMarksSelection gridCheckMark = cboMediStock.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            List<MediStockADO> selectData = new List<MediStockADO>() { data };
                            gridCheckMark.SelectAll(selectData);
                        }
                    }
                    else
                        EnableControl(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControl(bool? IsEnable)
        {
            try
            {
                if (IsEnable.HasValue)
                {
                    //btnCreatePeriod.Enabled = IsEnable.Value;
                    btnSave.Enabled = IsEnable.Value;
                    btnImport.Enabled = IsEnable.Value;
                    btnKiemKe.Enabled = IsEnable.Value;
                }
                else
                {
                    //btnCreatePeriod.Enabled = false;
                    btnSave.Enabled = false;
                    btnImport.Enabled = false;
                    btnKiemKe.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //internal void LoadStockCombo(string searchCode, bool isExpand)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(searchCode))
        //        {
        //            cboMediStock.EditValue = null;
        //            cboMediStock.Focus();
        //            cboMediStock.ShowPopup();
        //        }
        //        else
        //        {
        //            var data = this._MediStockProcess.Where(o => o.MEDI_STOCK_CODE.Contains(searchCode)).ToList();
        //            if (data != null)
        //            {
        //                if (data.Count == 1)
        //                {
        //                    cboMediStock.EditValue = data[0].ID;
        //                    txtCode.Text = data[0].MEDI_STOCK_CODE;
        //                    EnableControl(data[0].IsOutUser);
        //                    LoadDataMediStockPeriod();
        //                }
        //                else
        //                {
        //                    var search = data.FirstOrDefault(m => m.MEDI_STOCK_CODE == searchCode);
        //                    if (search != null)
        //                    {
        //                        cboMediStock.EditValue = search.ID;
        //                        txtCode.Text = search.MEDI_STOCK_CODE;
        //                        LoadDataMediStockPeriod();
        //                        EnableControl(search.IsOutUser);
        //                    }
        //                    else
        //                    {
        //                        cboMediStock.EditValue = null;
        //                        cboMediStock.Focus();
        //                        cboMediStock.ShowPopup();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void LoadDataMediStockPeriod()
        {
            try
            {
                CommonParam param = new CommonParam();
                gridControlMediStockPeriod.DataSource = null;
                gridControlMedicine.DataSource = null;
                gridControlMaterial.DataSource = null;
                gridControlBlood.DataSource = null;

                MOS.Filter.HisMediStockPeriodViewFilter mediStockPeriodFilter = new MOS.Filter.HisMediStockPeriodViewFilter();
                if (MediStock__Seleced != null && MediStock__Seleced.Count > 0)
                {
                    mediStockPeriodFilter.MEDI_STOCK_IDs = MediStock__Seleced.Select(s => s.ID).ToList();
                }
                else
                {
                    mediStockPeriodFilter.MEDI_STOCK_ID = 0;//bằng 0 để không tìm được kỳ nào
                }
                //mediStockPeriodFilter.ORDER_DIRECTION = "DESC";
                //mediStockPeriodFilter.ORDER_FIELD = "MODIFY_TIME";
                if (!String.IsNullOrWhiteSpace(TxtKeyWord.Text))
                {
                    mediStockPeriodFilter.KEY_WORD = TxtKeyWord.Text.Trim();
                }

                var apiResult = new BackendAdapter(param).Get<List<V_HIS_MEDI_STOCK_PERIOD>>(HisRequestUriStore.HIS_MEDI_STOCK_PERIOD_GETVIEW, ApiConsumers.MosConsumer, mediStockPeriodFilter, param);//Check lại
                if (apiResult != null)
                {
                    ListMediStockDataSource = new List<MediStockPeriodADO>();
                    foreach (var item in apiResult)
                    {
                        ListMediStockDataSource.Add(new MediStockPeriodADO(item));
                    }

                    ListMediStockDataSource = ListMediStockDataSource.OrderByDescending(o => o.TO_TIME).ToList();

                    gridControlMediStockPeriod.DataSource = ListMediStockDataSource;
                    gridViewMediStockPeriod.ExpandAllGroups();
                }
                //this.HisMediStockPeriod = new V_HIS_MEDI_STOCK_PERIOD();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowUc(MediStockPeriodADO hisMediStockPeriod)
        {
            try
            {
                if (hisMediStockPeriod == null)
                    return;

                WaitingManager.Show();
                LoadDataMedicine(hisMediStockPeriod);
                LoadDataMaterial(hisMediStockPeriod);
                LoadDataBlood(hisMediStockPeriod);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void ChooAllStock()
        //{
        //    try
        //    {
        //        if (gridViewMediStockPeriod.RowCount > 0)
        //        {
        //            ListMediStockCheck = new List<MediStockPeriodADO>();
        //            for (int i = 0; i < gridViewMediStockPeriod.SelectedRowsCount; i++)
        //            {
        //                if (gridViewMediStockPeriod.GetSelectedRows()[i] >= 0)
        //                {
        //                    ListMediStockCheck.Add((MediStockPeriodADO)gridViewMediStockPeriod.GetRow(gridViewMediStockPeriod.GetSelectedRows()[i]));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void RefeshAfterSave()
        {
            try
            {
                if (this.ListMestPeriodMety != null && this.ListMestPeriodMety.Count > 0)
                {
                    this.ListMestPeriodMety.ForEach(o => o.INVENTORY_AMOUNT = o.KK_AMOUNT);
                }

                if (this.ListMestPeriodMaty != null && this.ListMestPeriodMaty.Count > 0)
                {
                    this.ListMestPeriodMaty.ForEach(o => o.INVENTORY_AMOUNT = o.KK_AMOUNT);
                }

                if (this.ListMestPeriodBloods != null && this.ListMestPeriodBloods.Count > 0)
                {
                    //this.ListMestPeriodBloods.ForEach(o => o.INVENTORY_AMOUNT = o.KK_AMOUNT);
                }
                gridControlMedicine.RefreshDataSource();
                gridControlMaterial.RefreshDataSource();
                gridControlBlood.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Control editor
        //private void txtCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
        //            LoadStockCombo(strValue, false);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void cboMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    TxtKeyWord.Focus();
                    //if (cboMediStock.EditValue != null)
                    //{
                    //    var rs = this._MediStockProcess.Where(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStock.EditValue.ToString())).FirstOrDefault();
                    //    if (rs != null)
                    //    {
                    //        txtCode.Text = rs.MEDI_STOCK_CODE;
                    //        LoadDataMediStockPeriod();
                    //        EnableControl(rs.IsOutUser);
                    //    }
                    //}

                    if (MediStock__Seleced != null && MediStock__Seleced.Count == 1)
                    {
                        EnableControl(MediStock__Seleced.First().IsOutUser);
                    }
                    else
                        EnableControl(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStock_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void SpinKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.') && (e.KeyChar != ',') && (e.KeyChar != '-'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkBlood_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //chkBlood.Checked = !chkThuoc.Checked;
                //if (chkBlood.Checked)
                //{
                //    ShowUc();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkThuoc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (chkThuoc.Checked)
                //{
                //    ShowUc();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkVatTu_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // chkThuoc.Checked = !chkVatTu.Checked;
                //if (chkVatTu.Checked)
                //{
                //    ShowUc();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataMediStockPeriod();
                }
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
                if (e.KeyCode == Keys.Enter)
                    LoadDataMedicineByFilter();
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
                if (e.KeyCode == Keys.Enter)
                    LoadDataMaterialByFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchBlood_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    LoadDataBloodByFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchMedicine_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataMedicineByFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchMaterial_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataMaterialByFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchBlood_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataBloodByFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var data = (HisMestPeriodMediAdo)gridViewMedicine.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.IN_AMOUNT < 0
                        || data.OUT_AMOUNT < 0
                        || data.BEGIN_AMOUNT < 0
                        || data.VIR_END_AMOUNT < 0
                        || (data.INVENTORY_AMOUNT != null && data.INVENTORY_AMOUNT < 0)
                        || data.VIR_END_AMOUNT != (data.BEGIN_AMOUNT + data.IN_AMOUNT - data.OUT_AMOUNT))
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    else if ((data.INVENTORY_AMOUNT != null && data.VIR_END_AMOUNT != data.INVENTORY_AMOUNT))
                    {
                        e.Appearance.ForeColor = Color.Orange;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterial_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var data = (HisMestPeriodMateAdo)gridViewMaterial.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.IN_AMOUNT < 0
                        || data.OUT_AMOUNT < 0
                        || data.BEGIN_AMOUNT < 0
                        || data.VIR_END_AMOUNT < 0
                        || (data.INVENTORY_AMOUNT != null && data.INVENTORY_AMOUNT < 0)
                        || data.VIR_END_AMOUNT != (data.BEGIN_AMOUNT + data.IN_AMOUNT - data.OUT_AMOUNT))
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    else if ((data.INVENTORY_AMOUNT != null && data.VIR_END_AMOUNT != data.INVENTORY_AMOUNT))
                    {
                        e.Appearance.ForeColor = Color.Orange;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBlood_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var data = (HisMestPeriodBloodAdo)gridViewBlood.GetRow(e.RowHandle);
                if (data != null)
                {
                    //if (data.IN_AMOUNT < 0
                    //    || data.OUT_AMOUNT < 0
                    //    || data.BEGIN_AMOUNT < 0
                    //    || data.VIR_END_AMOUNT < 0
                    //    || (data.INVENTORY_AMOUNT != null && data.INVENTORY_AMOUNT < 0)
                    //    || data.VIR_END_AMOUNT != (data.BEGIN_AMOUNT + data.IN_AMOUNT - data.OUT_AMOUNT))
                    //{
                    //    e.Appearance.ForeColor = Color.Red;
                    //}
                    //else if ((data.INVENTORY_AMOUNT != null && data.VIR_END_AMOUNT != data.INVENTORY_AMOUNT))
                    //{
                    //    e.Appearance.ForeColor = Color.Orange;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    HisMestPeriodMediAdo data = e.Row as HisMestPeriodMediAdo;
                    if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(data.EXPIRED_DATE ?? 0));
                    }
                    else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                    {
                        e.Value = (data.IMP_VAT_RATIO * 100);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    HisMestPeriodMateAdo data = e.Row as HisMestPeriodMateAdo;
                    //if (data.ParentNodeId != null)
                    //{
                    if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(data.EXPIRED_DATE ?? 0));
                    }
                    else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                    {
                        e.Value = (data.IMP_VAT_RATIO * 100);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewBlood_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    //HisMestPeriodBloodAdo data = e.Row as HisMestPeriodBloodAdo;
                    ////if (data.ParentNodeId != null)
                    ////{
                    //if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                    //{
                    //    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(data.EXPIRED_DATE ?? 0));
                    //}
                    //else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                    //{
                    //    e.Value = (data.IMP_VAT_RATIO * 100);
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridUpdateCurrentRow()
        {
            try
            {
                if (this.gridViewMedicine.IsEditing)
                    this.gridViewMedicine.CloseEditor();
                if (this.gridViewMedicine.FocusedRowModified)
                    this.gridViewMedicine.UpdateCurrentRow();

                if (this.gridViewMaterial.IsEditing)
                    this.gridViewMaterial.CloseEditor();
                if (this.gridViewMaterial.FocusedRowModified)
                    this.gridViewMaterial.UpdateCurrentRow();

                if (this.gridViewBlood.IsEditing)
                    this.gridViewBlood.CloseEditor();
                if (this.gridViewBlood.FocusedRowModified)
                    this.gridViewBlood.UpdateCurrentRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediStockPeriod_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    var data = (MediStockPeriodADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    //if (e.Column.FieldName == "STT")
                    //{
                    //    e.Value = e.ListSourceRowIndex + 1;
                    //}
                    //else
                    if (e.Column.FieldName == "TO_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TO_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediStockPeriod_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var row = (MediStockPeriodADO)gridViewMediStockPeriod.GetFocusedRow();
                if (row != null)
                {
                    //this.HisMediStockPeriod = row;
                    ShowUc(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStockPeriod_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim();
                    //var data = (V_HIS_MEDI_STOCK_PERIOD)gridViewMediStockPeriod.GetRow(e.RowHandle);
                    string creator = (gridViewMediStockPeriod.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    short? isApproval = null;
                    string approvalStr = (gridViewMediStockPeriod.GetRowCellValue(e.RowHandle, "IS_APPROVE") ?? "").ToString();
                    if (!String.IsNullOrEmpty(approvalStr))
                    {
                        isApproval = Int16.Parse((gridViewMediStockPeriod.GetRowCellValue(e.RowHandle, "IS_APPROVE") ?? "").ToString());
                    }

                    bool? isOutUser = null;
                    string mediStock = (gridViewMediStockPeriod.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString();
                    if (!String.IsNullOrWhiteSpace(mediStock))
                    {
                        try
                        {
                            var medistock = this._MediStockProcess.FirstOrDefault(o => o.ID == long.Parse(mediStock));
                            isOutUser = medistock != null ? medistock.IsOutUser : null;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        if ((creator == loginName || CheckLoginAdmin.IsAdmin(loginName)) && isOutUser == true)
                            e.RepositoryItem = repositoryItem__Delete_E;
                        else
                            e.RepositoryItem = repositoryItem__Delete_D;
                    }
                    else if (e.Column.FieldName == "EDIT")
                    {
                        if ((creator == loginName || CheckLoginAdmin.IsAdmin(loginName)) && isOutUser == true)
                            e.RepositoryItem = repositoryItem__Edit_E;
                        else
                            e.RepositoryItem = repositoryItem__Edit_D;
                    }
                    else if (e.Column.FieldName == "APROVAL_MEDI_STOCK")
                    {
                        if (isOutUser == true)
                        {
                            if (isApproval == 1)
                            {
                                e.RepositoryItem = ButtonEdit_UnApproval;
                            }
                            else
                            {
                                e.RepositoryItem = ButtonEdit_Approval;
                            }
                        }
                    }
                    else if (e.Column.FieldName == "Is_Check")
                    {
                        if ((creator == loginName || CheckLoginAdmin.IsAdmin(loginName)) && isOutUser == true)
                            e.RepositoryItem = repositoryItemCheckEdit;
                        else
                            e.RepositoryItem = repositoryItemCheckEditDisable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItem__Delete_E_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (MediStockPeriodADO)gridViewMediStockPeriod.GetFocusedRow();
                if (data != null && (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn hủy dữ liệu không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisMediStockPeriod/Delete", ApiConsumers.MosConsumer, data.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (success)
                    {
                        LoadDataMediStockPeriod();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<V_HIS_MEST_PERIOD_MEDI> mestPeriodMetiList = null;
        List<V_HIS_MEST_PERIOD_MATE> mestPeriodMateList = null;
        List<long> medistockSelectList = null;
        private void repositoryItembtnPrint_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var hisMediStockPeriod = (MediStockPeriodADO)gridViewMediStockPeriod.GetFocusedRow();
                if (hisMediStockPeriod == null) return;

                // mest_period_medi
                MOS.Filter.HisMestPeriodMediViewFilter mediFilter = new MOS.Filter.HisMestPeriodMediViewFilter();
                mediFilter.MEDI_STOCK_PERIOD_ID = hisMediStockPeriod.ID;
                mestPeriodMetiList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEST_PERIOD_MEDI>>("api/HisMestPeriodMedi/GetView", ApiConsumer.ApiConsumers.MosConsumer, mediFilter, null);
                // mest_period_mate
                MOS.Filter.HisMestPeriodMateViewFilter mateFilter = new MOS.Filter.HisMestPeriodMateViewFilter();
                mateFilter.MEDI_STOCK_PERIOD_ID = hisMediStockPeriod.ID;
                mestPeriodMateList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEST_PERIOD_MATE>>("api/HisMestPeriodMate/GetView", ApiConsumer.ApiConsumers.MosConsumer, mateFilter, null);

                List<long> medistockIds = new List<long>();
                if (mestPeriodMetiList != null && mestPeriodMetiList.Count() > 0)
                {
                    medistockIds.AddRange(mestPeriodMetiList.Where(p => p.MEDI_STOCK_ID.HasValue).Select(o => o.MEDI_STOCK_ID ?? 0).ToList());
                }
                if (mestPeriodMateList != null && mestPeriodMateList.Count() > 0)
                {
                    medistockIds.AddRange(mestPeriodMateList.Where(p => p.MEDI_STOCK_ID.HasValue).Select(o => o.MEDI_STOCK_ID ?? 0).ToList());
                }

                medistockIds = medistockIds != null ? medistockIds.Distinct().ToList() : medistockIds;
                medistockSelectList = new List<long>();
                medistockSelectList = medistockIds;

                WaitingManager.Hide();
                frmChooseMedistock frm = new frmChooseMedistock(medistockIds, delegateSelect);
                frm.ShowDialog();

                PrintProcess(PrintType.IN_BIEN_BAN_KIEM_KE_T_VT_M);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void delegateSelect(object data)
        {
            try
            {
                if (data != null && data is List<long>)
                {
                    medistockSelectList = (List<long>)data;
                }
                else
                {
                    medistockSelectList = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemSpinMediAmountKK_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemSpinMateAmountKK_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemSpinBloodAmountKK_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItem__Edit_E_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (MediStockPeriodADO)gridViewMediStockPeriod.GetFocusedRow();
                if (data != null)
                {
                    frmCreatePeriod frmCreatePeriod = new frmCreatePeriod(this.currentModule, _MediStockProcess, data, LoadDataMediStockPeriod);
                    frmCreatePeriod.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItem__KiemKe_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                // WaitingManager.Show();
                var data = (MediStockPeriodADO)gridViewMediStockPeriod.GetFocusedRow();
                if (data != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisMestInveUser").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisMestInveUser");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
                // WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Button click
        private void btnDropDownStock_Click(object sender, EventArgs e)
        {
            try
            {
                //var btnMenuStockFind = sender as DXMenuItem;
                //btnDropDownStock.Text = btnMenuStockFind.Caption;
                //this.typeStockFind = btnMenuStockFind.Caption;
                //LoadDataMediStockPeriod();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                var hisMediStockPeriod = (MediStockPeriodADO)gridViewMediStockPeriod.GetFocusedRow();
                if (hisMediStockPeriod == null) return;
                ShowUc(hisMediStockPeriod);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCreatePeriod_Click(object sender, EventArgs e)
        {
            try
            {
                frmCreatePeriod frmCreatePeriod = new frmCreatePeriod(this.currentModule, _MediStockProcess, LoadDataMediStockPeriod);
                frmCreatePeriod.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(PrintType.IN_BIEN_BAN_KIEM_KE_T_VT_M);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnKiemKe_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ListMestPeriodMety != null && this.ListMestPeriodMety.Count > 0)
                {
                    this.ListMestPeriodMety.ForEach(o => o.KK_AMOUNT = o.VIR_END_AMOUNT ?? 0);
                }

                if (this.ListMestPeriodMaty != null && this.ListMestPeriodMaty.Count > 0)
                {
                    this.ListMestPeriodMaty.ForEach(o => o.KK_AMOUNT = o.VIR_END_AMOUNT ?? 0);
                }

                if (this.ListMestPeriodBloods != null && this.ListMestPeriodBloods.Count > 0)
                {
                    //TODO
                    //this.ListMestPeriodBloods.ForEach(o => o.KK_AMOUNT = o.VIR_END_AMOUNT ?? 0);
                }
                gridControlMedicine.RefreshDataSource();
                gridControlMaterial.RefreshDataSource();
                gridControlBlood.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var hisMediStockPeriod = (MediStockPeriodADO)gridViewMediStockPeriod.GetFocusedRow();
                if (hisMediStockPeriod == null) return;

                this.GridUpdateCurrentRow();

                MOS.SDO.HisMediStockPeriodInventorySDO mediStockPeriodInventory = new MOS.SDO.HisMediStockPeriodInventorySDO();
                mediStockPeriodInventory.MediStockPeriodId = hisMediStockPeriod.ID;
                mediStockPeriodInventory.Medicines = GetMestPeriodMetyForSave();
                mediStockPeriodInventory.Materials = GetMestPeriodMateForSave();
                //listMestPeriodBloodUpdate = GetMestPeriodBloodForSave();//TODO

                //TODO call api
                //............
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                success = new BackendAdapter(param).Post<bool>(RequestUriStore.HIS_MEST_STOCK_PERIOD_INVENTORY, ApiConsumers.MosConsumer, mediStockPeriodInventory, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediStockPeriodInventory), mediStockPeriodInventory)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));

                WaitingManager.Hide();
                if (success)
                {
                    RefeshAfterSave();
                }
                #region ShowMessager
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HisMestPeriodMediSDO> GetMestPeriodMetyForSave()
        {
            List<HisMestPeriodMediSDO> listMestPeriodMetyUpdate = new List<HisMestPeriodMediSDO>();
            try
            {
                if (this.ListMestPeriodMety != null && this.ListMestPeriodMety.Count > 0)
                {
                    foreach (var item in this.ListMestPeriodMety)
                    {
                        listMestPeriodMetyUpdate.Add(new HisMestPeriodMediSDO()
                        {
                            Id = item.ID,
                            InventoryAmount = item.KK_AMOUNT
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return listMestPeriodMetyUpdate;
        }

        private List<HisMestPeriodMateSDO> GetMestPeriodMateForSave()
        {
            List<HisMestPeriodMateSDO> listMestPeriodMateUpdate = new List<HisMestPeriodMateSDO>();
            try
            {
                if (this.ListMestPeriodMaty != null && this.ListMestPeriodMaty.Count > 0)
                {
                    foreach (var item in this.ListMestPeriodMaty)
                    {
                        listMestPeriodMateUpdate.Add(new HisMestPeriodMateSDO()
                        {
                            Id = item.ID,
                            InventoryAmount = item.KK_AMOUNT
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return listMestPeriodMateUpdate;
        }

        private List<HIS_MEST_PERIOD_BLOOD> GetMestPeriodBloodForSave()
        {
            List<HIS_MEST_PERIOD_BLOOD> listMestPeriodBloodUpdate = new List<HIS_MEST_PERIOD_BLOOD>();
            try
            {
                if (this.ListMestPeriodBloods != null && this.ListMestPeriodBloods.Count > 0)
                {
                    foreach (var item in this.ListMestPeriodBloods)
                    {
                        listMestPeriodBloodUpdate.Add(new HIS_MEST_PERIOD_BLOOD()
                        {
                            MEDI_STOCK_PERIOD_ID = item.MEDI_STOCK_PERIOD_ID,
                            ID = item.ID,
                            //KK_AMOUNT = item.KK_AMOUNT
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return listMestPeriodBloodUpdate;
        }

        #endregion

        #region Public method
        public void TaiLai()
        {
            try
            {
                btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InAn()
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

        public void Save()
        {
            try
            {
                if (btnSave.Enabled)
                    btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Find()
        {
            try
            {
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void ButtonEdit_Approval_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var focus = (MediStockPeriodADO)gridViewMediStockPeriod.GetFocusedRow();
                if (focus != null)
                {
                    frmApproval frm = new frmApproval(focus.ID, true, ReLoadDataApproval, this.currentModule.RoomId);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReLoadDataApproval(object obj)
        {
            if (obj != null)
            {
                var focus = (MediStockPeriodADO)gridViewMediStockPeriod.GetFocusedRow();

                List<MediStockPeriodADO> dataSources = (List<MediStockPeriodADO>)gridControlMediStockPeriod.DataSource;
                foreach (var item in dataSources)
                {
                    if (item.ID == focus.ID)
                    {
                        item.IS_APPROVE = 1;
                    }
                }

                gridControlMediStockPeriod.DataSource = null;
                gridControlMediStockPeriod.DataSource = dataSources;
            }
        }

        private void ReLoadDataUnApproval(object obj)
        {
            if (obj != null)
            {
                var focus = (MediStockPeriodADO)gridViewMediStockPeriod.GetFocusedRow();
                List<MediStockPeriodADO> dataSources = (List<MediStockPeriodADO>)gridControlMediStockPeriod.DataSource;
                foreach (var item in dataSources)
                {
                    if (item.ID == focus.ID)
                    {
                        item.IS_APPROVE = null;
                    }
                }
                gridControlMediStockPeriod.DataSource = null;
                gridControlMediStockPeriod.DataSource = dataSources;
            }
        }

        private void ButtonEdit_UnApproval_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var focus = (MediStockPeriodADO)gridViewMediStockPeriod.GetFocusedRow();
                if (focus != null)
                {
                    frmApproval frm = new frmApproval(focus.ID, false, ReLoadDataUnApproval, this.currentModule.RoomId);
                    frm.ShowDialog();
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
                var source = System.IO.Path.Combine(Application.StartupPath
                + "/Tmp/Imp", "IMPORT_MEDI_STOC_PERIOD.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_MEDI_STOC_PERIOD";
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

        private void chkAllMedistock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataToComboMediStock();
                if (cboMediStock.EditValue != null)
                {
                    LoadDataMediStockPeriod();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStock_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string display = "";
                foreach (var item in this.MediStock__Seleced)
                {
                    if (display.Trim().Length > 0)
                    {
                        display += ", " + item.MEDI_STOCK_NAME;
                    }
                    else
                        display = item.MEDI_STOCK_NAME;
                }
                e.DisplayText = display;
                cboMediStock.ToolTip = display;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediSock_View_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    string isOutStr = (View.GetRowCellValue(e.RowHandle, "IsOutUser") ?? "").ToString();
                    bool? IsOut = null;
                    if (String.IsNullOrWhiteSpace(isOutStr))
                        IsOut = null;
                    else
                        IsOut = Inventec.Common.TypeConvert.Parse.ToBoolean(isOutStr);

                    if (IsOut == false)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataMediStockPeriod();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnDeletePeriod_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.gridViewMediStockPeriod.IsEditing)
                    this.gridViewMediStockPeriod.CloseEditor();
                if (this.gridViewMediStockPeriod.FocusedRowModified)
                    this.gridViewMediStockPeriod.UpdateCurrentRow();

                var lstMediStockPeriod = ListMediStockDataSource.Where(o => o.IsCheck).ToList();
                if (lstMediStockPeriod != null && lstMediStockPeriod.Count > 0 && (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn hủy dữ liệu không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    lstMediStockPeriod = lstMediStockPeriod.OrderByDescending(o => o.TO_TIME).ThenByDescending(o => o.CREATE_TIME ?? 0).ToList();
                    List<string> messages = new List<string>();
                    foreach (var item in lstMediStockPeriod)
                    {
                        CommonParam paramd = new CommonParam();
                        bool result = new Inventec.Common.Adapter.BackendAdapter(paramd).Post<bool>("api/HisMediStockPeriod/Delete", ApiConsumers.MosConsumer, item.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramd);
                        if (!result)
                        {
                            messages.Add(string.Format("{0} - {1}({2})", item.MEDI_STOCK_PERIOD_NAME, item.MEDI_STOCK_NAME, paramd.GetMessage()));
                        }
                    }

                    bool success = true;
                    if (messages.Count == 0)
                    {
                        LoadDataMediStockPeriod();
                    }
                    else
                    {
                        success = false;
                        param.Messages.AddRange(messages);
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn dữ liệu muốn xóa", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStockPeriod_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
        }

        private void repositoryItemCheckEdit_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.gridViewMediStockPeriod.CloseEditor();
                this.gridViewMediStockPeriod.UpdateCurrentRow();

                var dataChecked = ListMediStockDataSource.Where(o => o.IsCheck).ToList();
                if (dataChecked != null && dataChecked.Count > 0)
                {
                    BtnDeletePeriod.Enabled = true;
                }
                else
                {
                    BtnDeletePeriod.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
