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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MediStockSummaryByExpireDate.ADO;
using HIS.Desktop.Plugins.MediStockSummaryByExpireDate.Base;
using HIS.UC.HisBloodTypeInStock;
using HIS.UC.HisMateInStockByExpireDate;
using HIS.UC.HisMediInStockByExpireDate;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockSummaryByExpireDate
{
    public partial class UCMediStockSummaryByExpireDate : HIS.Desktop.Utility.UserControlBase
    {
        List<HisMedicineInStockSDO> lstMediInStocks { get; set; }
        List<HisMaterialInStockSDO> lstMateInStocks { get; set; }
        List<HisBloodTypeInStockSDO> lstBlood { get; set; }

        HisMediInStockByExpireDateProcessor hisMediInStockProcessor;
        HisMateInStockByExpireDateProcessor hisMateInStockProcessor;
        HisBloodTypeInStockProcessor hisBloodProcessor;

        UserControl ucMedicineInfo;
        UserControl ucMaterialInfo;
        UserControl ucBloodInfo;

        V_HIS_MEDI_STOCK currentMediStock = null;
        List<long> mediStockIds = new List<long>();
        bool isCheck;

        long RoomId;
        long RoomTypeId;

        string ModuleLinkName = "HIS.Desktop.Plugins.MediStockSummaryByExpireDate";
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        List<V_HIS_MEDI_STOCK> dataMediStocks = new List<V_HIS_MEDI_STOCK>();

        public UCMediStockSummaryByExpireDate(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                WaitingManager.Show();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.RoomId = _moduleData.RoomId;
                this.RoomTypeId = _moduleData.RoomTypeId;
                InitMedicineTree();
                InitHisMaterialInStockTree();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCMediStockSummaryByExpireDate_Load(object sender, EventArgs e)
        {
            try
            {
                dateEdit1.Enabled = false;
                InitCbo();
                SetCaptionByLanguageKey();
                LoadDataGridMediStock();

                // Đăng ký event TRƯỚC khi restore state
                chkMediStock.CheckedChanged += chkMediStock_CheckedChanged;
                cboExpriedDate.SelectedIndexChanged += cboExpriedDate_SelectedIndexChanged;

                // Restore control state SAU khi đã load xong kho
                InitControlState();

                // Chỉ gọi ShowUCControl nếu đã chọn Thuốc hoặc Vật tư VÀ đã có kho
                if ((chkMedicine.Checked || chkMaterial.Checked) && this.mediStockIds != null && this.mediStockIds.Count > 0)
                {
                    ShowUCControl();
                }

                // Ensure control state is saved when the form is closing or this control is disposed
                try
                {
                    var form = this.FindForm();
                    if (form != null)
                    {
                        form.FormClosing += (s, ev) => { try { SaveControlState(); } catch { } };
                    }
                }
                catch { }
                this.Disposed += (s, ev) => { try { SaveControlState(); } catch { } };
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLinkName);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == cboExpriedDate.Name)
                        {
                            switch (item.VALUE)
                            {
                                case "0":
                                    cboExpriedDate.SelectedIndex = 0;
                                    break;
                                case "1":
                                    cboExpriedDate.SelectedIndex = 1;
                                    break;
                                case "2":
                                    cboExpriedDate.SelectedIndex = 2;
                                    break;
                                case "3":
                                    cboExpriedDate.SelectedIndex = 3;
                                    break;
                                case "4":
                                    cboExpriedDate.SelectedIndex = 4;
                                    dateEdit1.Enabled = true;
                                    dateEdit1.DateTime = DateTime.Now;
                                    break;

                            }
                        }
                        if (item.KEY == chkMediStock.Name)
                        {
                            chkMediStock.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkAlertExpired.Name)
                        {
                            chkAlertExpired.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkMedicine.Name)
                        {
                            chkMedicine.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkMaterial.Name)
                        {
                            chkMaterial.Checked = item.VALUE == "1";
                        }

                    }
                }

                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveControlState()
        {
            try
            {
                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                // helper to add or update entry with MODULE_LINK set
                Action<string, string> upsert = (key, value) =>
                {
                    var existing = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == ModuleLinkName);
                    if (existing != null)
                    {
                        existing.VALUE = value;
                    }
                    else
                    {
                        var rdo = new HIS.Desktop.Library.CacheClient.ControlStateRDO
                        {
                            KEY = key,
                            VALUE = value,
                            MODULE_LINK = ModuleLinkName
                        };
                        this.currentControlStateRDO.Add(rdo);
                    }
                };

                upsert(cboExpriedDate.Name, cboExpriedDate.SelectedIndex.ToString());
                upsert(chkMediStock.Name, chkMediStock.Checked ? "1" : "0");
                upsert(chkAlertExpired.Name, chkAlertExpired.Checked ? "1" : "0");
                upsert(chkMedicine.Name, chkMedicine.Checked ? "1" : "0");
                upsert(chkMaterial.Name, chkMaterial.Checked ? "1" : "0");

                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMediStock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!isNotLoadWhileChangeControlStateInFirst)
                {
                    SaveControlState();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpriedDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!isNotLoadWhileChangeControlStateInFirst)
                {
                    SaveControlState();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MediStockSummaryByExpireDate.Resources.Lang", typeof(HIS.Desktop.Plugins.MediStockSummaryByExpireDate.UCMediStockSummaryByExpireDate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMaterial.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.chkMaterial.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMedicine.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.chkMedicine.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAlertExpired.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.chkAlertExpired.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefesh.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.btnRefesh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewMediStock.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.gridViewMediStock.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummaryByExpireDate.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMedicineTree()
        {
            try
            {
                hisMediInStockProcessor = new HisMediInStockByExpireDateProcessor();
                HisMediInStockByExpireDateInitADO ado = new HisMediInStockByExpireDateInitADO();
                ado.HisMediInStockByExpireDateColumns = new List<HisMediInStockByExpireDateColumn>();
                ado.IsShowSearchPanel = true;
                ado.HisMediInStockByExpireDate_CustomDrawNodeCell = medicineType_CustomDrawNodeCell;
                ado.HisMediInStockByExpireDate_CustomUnboundColumnData = medicineType_CustomUnboundColumnData;

                //Column mã
                HisMediInStockByExpireDateColumn mediTypeCodeNameCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_TYPE_CODE", 150, false);
                mediTypeCodeNameCol.VisibleIndex = 0;
                ado.HisMediInStockByExpireDateColumns.Add(mediTypeCodeNameCol);
                //Column tên
                HisMediInStockByExpireDateColumn mediTypeNameCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_TYPE_NAME", 200, false);
                mediTypeNameCol.VisibleIndex = 1;
                ado.HisMediInStockByExpireDateColumns.Add(mediTypeNameCol);

                //Column hoạt chất
                HisMediInStockByExpireDateColumn hoatChatCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_ACTIVE_INGR_BHYT_NAME", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "ACTIVE_INGR_BHYT_NAME", 150, false);
                hoatChatCol.VisibleIndex = 2;
                ado.HisMediInStockByExpireDateColumns.Add(hoatChatCol);
                //Column hàm lượng
                HisMediInStockByExpireDateColumn hamLuongCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_CONCENTRA", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "CONCENTRA", 100, false);
                hamLuongCol.VisibleIndex = 3;
                ado.HisMediInStockByExpireDateColumns.Add(hamLuongCol);

                //Column đơn vị tính
                HisMediInStockByExpireDateColumn serviceUnitNameCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERVICE_UNIT_NAME", 100, false);
                serviceUnitNameCol.VisibleIndex = 4;
                ado.HisMediInStockByExpireDateColumns.Add(serviceUnitNameCol);

                //Column đơn vị qui đổi
                HisMediInStockByExpireDateColumn serviceUnitNameQuiDoiCol = new HisMediInStockByExpireDateColumn("Đơn vị quy đổi", "CONVERT_UNIT_NAME", 60, false);
                serviceUnitNameQuiDoiCol.VisibleIndex = 5;
                ado.HisMediInStockByExpireDateColumns.Add(serviceUnitNameQuiDoiCol);

                //Column tỉ lệ qui đổi
                HisMediInStockByExpireDateColumn serviceConvertRatioCol = new HisMediInStockByExpireDateColumn("Tỷ lệ quy đổi", "CONVERT_RATIO", 60, false);
                serviceConvertRatioCol.VisibleIndex = 6;
                serviceConvertRatioCol.Format = new DevExpress.Utils.FormatInfo();
                serviceConvertRatioCol.Format.FormatString = "#,##0.00";
                serviceConvertRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMediInStockByExpireDateColumns.Add(serviceConvertRatioCol);

                //Column số lượng tồn
                HisMediInStockByExpireDateColumn totalAmountCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TotalAmount", 100, false);
                totalAmountCol.VisibleIndex = 7;
                totalAmountCol.Format = new DevExpress.Utils.FormatInfo();
                totalAmountCol.Format.FormatString = "#,##0.00";
                totalAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMediInStockByExpireDateColumns.Add(totalAmountCol);

                //Column Tồn theo đơn vị quy đổi (lấy số lượng tồn x tỷ lệ quy đổi)
                HisMediInStockByExpireDateColumn totalAmountConvertCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_AMOUNT_CONVERT", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TotalAmountConvert_Display", 100, false);
                totalAmountConvertCol.VisibleIndex = 8;
                totalAmountConvertCol.Format = new DevExpress.Utils.FormatInfo();
                totalAmountConvertCol.Format.FormatString = "#,##0.00";
                totalAmountConvertCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMediInStockByExpireDateColumns.Add(totalAmountConvertCol);

                //Column số lượng khả dụng
                HisMediInStockByExpireDateColumn availableAmoutCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_AVAILABLE_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AvailableAmount", 100, false);
                availableAmoutCol.VisibleIndex = 9;
                availableAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                availableAmoutCol.Format.FormatString = "#,##0.00";
                availableAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMediInStockByExpireDateColumns.Add(availableAmoutCol);

                //Column Khả dụng theo đơn vị quy đổi (lấy số lượng khả dụng x tỷ lệ quy đổi)
                HisMediInStockByExpireDateColumn availableAmoutConvertCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_AVAILABLE_CONVERT_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AvailableAmountConvert_Display", 100, false);
                availableAmoutConvertCol.VisibleIndex = 10;
                availableAmoutConvertCol.Format = new DevExpress.Utils.FormatInfo();
                availableAmoutConvertCol.Format.FormatString = "#,##0.00";
                availableAmoutConvertCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMediInStockByExpireDateColumns.Add(availableAmoutConvertCol);

                // Cơ số
                HisMediInStockByExpireDateColumn baseAmountCol = new HisMediInStockByExpireDateColumn("Cơ số", "BaseAmount_Display", 80, false);
                baseAmountCol.VisibleIndex = 11;
                baseAmountCol.Format = new DevExpress.Utils.FormatInfo();
                baseAmountCol.Format.FormatString = "#,##0.00";
                baseAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMediInStockByExpireDateColumns.Add(baseAmountCol);

                // Số lượng cần bù
                HisMediInStockByExpireDateColumn compensationAmountCol = new HisMediInStockByExpireDateColumn("Số lượng cần bù", "CompensationBaseAmount_Display", 80, false);
                compensationAmountCol.VisibleIndex = 12;
                compensationAmountCol.Format = new DevExpress.Utils.FormatInfo();
                compensationAmountCol.Format.FormatString = "#,##0.00";
                compensationAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMediInStockByExpireDateColumns.Add(compensationAmountCol);

                //Column giá nhập
                HisMediInStockByExpireDateColumn impPriceCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_IMP_PRICE", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMP_PRICE_DISPLAY", 100, false);
                impPriceCol.VisibleIndex = 13;
                impPriceCol.Format = new DevExpress.Utils.FormatInfo();
                impPriceCol.Format.FormatString = "#,##0.0000";
                impPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMediInStockByExpireDateColumns.Add(impPriceCol);
                //Column VAT %
                HisMediInStockByExpireDateColumn impVatRatioCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_IMP_VAT_RATIO", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMP_VAT_RATIO_DISPLAY", 100, false);
                impVatRatioCol.VisibleIndex = 14;
                ado.HisMediInStockByExpireDateColumns.Add(impVatRatioCol);

                //Column thành tiền
                HisMediInStockByExpireDateColumn totalPriceCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_PRICE", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TOTAL_PRICE", 100, false);
                totalPriceCol.VisibleIndex = 15;
                totalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                totalPriceCol.Format.FormatString = "#,##0.0000";
                totalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMediInStockByExpireDateColumns.Add(totalPriceCol);
                //Column hạn sử dụng
                //HisMediInStockByExpireDateColumn expriredDateCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_EXPIRED_DATE", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "EXPIRED_DATE_DISPLAY", 100, false);
                //expriredDateCol.VisibleIndex = 8;
                //ado.HisMediInStockByExpireDateColumns.Add(expriredDateCol);
                //Column gói thầu
                HisMediInStockByExpireDateColumn bidNumberCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_BID_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BID_NUMBER", 100, false);
                bidNumberCol.VisibleIndex = 16;
                ado.HisMediInStockByExpireDateColumns.Add(bidNumberCol);

                //Column số lô
                HisMediInStockByExpireDateColumn packageNumberCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_PACKAGE_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "PACKAGE_NUMBER", 100, false);
                packageNumberCol.VisibleIndex = 17;
                ado.HisMediInStockByExpireDateColumns.Add(packageNumberCol);

                //Column kho
                HisMediInStockByExpireDateColumn mediStockCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDI_STOCK", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDI_STOCK_NAME", 100, false);
                mediStockCol.VisibleIndex = 18;
                ado.HisMediInStockByExpireDateColumns.Add(mediStockCol);

                //Column số đăng ký
                HisMediInStockByExpireDateColumn registerNumberCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_REGISTER_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "REGISTER_NUMBER", 100, false);
                registerNumberCol.VisibleIndex = 19;
                ado.HisMediInStockByExpireDateColumns.Add(registerNumberCol);

                //Column nhà cung cấp
                HisMediInStockByExpireDateColumn supplierNameCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_SUPPLIER_MIN_IN_STOCK", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SUPPLIER_NAME", 100, false);
                supplierNameCol.VisibleIndex = 20;
                ado.HisMediInStockByExpireDateColumns.Add(supplierNameCol);
                //Column số lượng cảnh báo
                //HisMediInStockByExpireDateColumn alertMinInStockCol = new HisMediInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_ALERT_MIN_IN_STOCK", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "ALERT_MIN_IN_STOCK", 100, false);
                //alertMinInStockCol.VisibleIndex = 13;
                //alertMinInStockCol.Format = new DevExpress.Utils.FormatInfo();
                //alertMinInStockCol.Format.FormatString = "#,##0.00";
                //alertMinInStockCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                //ado.HisMediInStockByExpireDateColumns.Add(alertMinInStockCol);

                this.ucMedicineInfo = (UserControl)hisMediInStockProcessor.Run(ado);

                if (this.ucMedicineInfo != null)
                {
                    this.panelControlMediMate.Controls.Add(this.ucMedicineInfo);
                    this.ucMedicineInfo.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitHisMaterialInStockTree()
        {
            try
            {
                hisMateInStockProcessor = new HisMateInStockByExpireDateProcessor();
                HisMateInStockByExpireDateInitADO ado = new HisMateInStockByExpireDateInitADO();
                ado.HisMateInStockByExpireDateColumns = new List<HisMateInStockByExpireDateColumn>();
                ado.IsShowSearchPanel = true;
                ado.HisMateInStockByExpireDate_CustomDrawNodeCell = materialType_CustomDrawNodeCell;
                ado.HisMateInStockByExpireDate_CustomUnboundColumnData = materialType_CustomUnboundColumnData;

                //Column mã
                HisMateInStockByExpireDateColumn mediTypeCodeNameCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MATERIAL_TYPE_CODE", 150, false);
                mediTypeCodeNameCol.VisibleIndex = 0;
                ado.HisMateInStockByExpireDateColumns.Add(mediTypeCodeNameCol);
                //Column tên
                HisMateInStockByExpireDateColumn mediTypeNameCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MATERIAL_TYPE_NAME", 200, false);
                mediTypeNameCol.VisibleIndex = 1;
                ado.HisMateInStockByExpireDateColumns.Add(mediTypeNameCol);
                //Column đơn vị tính
                HisMateInStockByExpireDateColumn serviceUnitNameCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERVICE_UNIT_NAME", 100, false);
                serviceUnitNameCol.VisibleIndex = 2;
                ado.HisMateInStockByExpireDateColumns.Add(serviceUnitNameCol);
                //Column số lượng tồn
                HisMateInStockByExpireDateColumn totalAmountCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TotalAmount", 100, false);
                totalAmountCol.VisibleIndex = 3;
                totalAmountCol.Format = new DevExpress.Utils.FormatInfo();
                totalAmountCol.Format.FormatString = "#,##0.00";
                totalAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMateInStockByExpireDateColumns.Add(totalAmountCol);
                //Column số lượng khả dụng
                HisMateInStockByExpireDateColumn availableAmoutCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_AVAILABLE_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AvailableAmount", 100, false);
                availableAmoutCol.VisibleIndex = 4;
                availableAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                availableAmoutCol.Format.FormatString = "#,##0.00";
                availableAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMateInStockByExpireDateColumns.Add(availableAmoutCol);

                // Cơ số
                HisMateInStockByExpireDateColumn baseAmountCol = new HisMateInStockByExpireDateColumn("Cơ số", "BaseAmount_Display", 80, false);
                baseAmountCol.VisibleIndex = 5;
                baseAmountCol.Format = new DevExpress.Utils.FormatInfo();
                baseAmountCol.Format.FormatString = "#,##0.00";
                baseAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMateInStockByExpireDateColumns.Add(baseAmountCol);

                // Số lượng cần bù
                HisMateInStockByExpireDateColumn compensationAmountCol = new HisMateInStockByExpireDateColumn("Số lượng cần bù", "CompensationBaseAmount_Display", 80, false);
                compensationAmountCol.VisibleIndex = 6;
                compensationAmountCol.Format = new DevExpress.Utils.FormatInfo();
                compensationAmountCol.Format.FormatString = "#,##0.00";
                compensationAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMateInStockByExpireDateColumns.Add(compensationAmountCol);

                //Column giá nhập
                HisMateInStockByExpireDateColumn impPriceCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_IMP_PRICE", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMP_PRICE_DISPLAY", 100, false);
                impPriceCol.VisibleIndex = 7;
                impPriceCol.Format = new DevExpress.Utils.FormatInfo();
                impPriceCol.Format.FormatString = "#,##0.0000";
                impPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMateInStockByExpireDateColumns.Add(impPriceCol);

                //Column VAT %
                HisMateInStockByExpireDateColumn impVatRatioCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_IMP_VAT_RATIO", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMP_VAT_RATIO_DISPLAY", 100, false);
                impVatRatioCol.VisibleIndex = 8;
                ado.HisMateInStockByExpireDateColumns.Add(impVatRatioCol);

                //Column thành tiền
                HisMateInStockByExpireDateColumn totalPriceCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_PRICE", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TOTAL_PRICE", 100, false);
                totalPriceCol.VisibleIndex = 9;
                totalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                totalPriceCol.Format.FormatString = "#,##0.0000";
                totalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMateInStockByExpireDateColumns.Add(totalPriceCol);

                //Column hạn sử dụng
                //HisMateInStockByExpireDateColumn expriredDateCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_EXPIRED_DATE", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "EXPIRED_DATE_DISPLAY", 100, false);
                //expriredDateCol.VisibleIndex = 8;
                //ado.HisMateInStockByExpireDateColumns.Add(expriredDateCol);
                //Column gói thầu
                HisMateInStockByExpireDateColumn bidNumberCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_BID_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BID_NUMBER", 100, false);
                bidNumberCol.VisibleIndex = 10;
                ado.HisMateInStockByExpireDateColumns.Add(bidNumberCol);

                //Column số lô
                HisMateInStockByExpireDateColumn packageNumberCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_PACKAGE_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "PACKAGE_NUMBER", 100, false);
                packageNumberCol.VisibleIndex = 11;
                ado.HisMateInStockByExpireDateColumns.Add(packageNumberCol);

                //Column kho
                HisMateInStockByExpireDateColumn mediStockCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDI_STOCK", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDI_STOCK_NAME", 100, false);
                mediStockCol.VisibleIndex = 11;
                ado.HisMateInStockByExpireDateColumns.Add(mediStockCol);

                //Column số đăng ký
                HisMateInStockByExpireDateColumn registerNumberCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_REGISTER_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "REGISTER_NUMBER", 100, false);
                registerNumberCol.VisibleIndex = 12;
                ado.HisMateInStockByExpireDateColumns.Add(registerNumberCol);

                //Column nhà cung cấp
                HisMateInStockByExpireDateColumn supplierNameCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_SUPPLIER_MIN_IN_STOCK", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SUPPLIER_NAME", 100, false);
                supplierNameCol.VisibleIndex = 13;
                ado.HisMateInStockByExpireDateColumns.Add(supplierNameCol);
                //Column số lượng cảnh báo
                //HisMateInStockByExpireDateColumn alertMinInStockCol = new HisMateInStockByExpireDateColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_ALERT_MIN_IN_STOCK", Base.ResourceLangManager.LanguageUCMediStockSummaryByExpireDate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "ALERT_MIN_IN_STOCK", 100, false);
                //alertMinInStockCol.VisibleIndex = 13;
                //alertMinInStockCol.Format = new DevExpress.Utils.FormatInfo();
                //alertMinInStockCol.Format.FormatString = "#,##0.00";
                //alertMinInStockCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                //ado.HisMateInStockByExpireDateColumns.Add(alertMinInStockCol);

                this.ucMaterialInfo = (UserControl)hisMateInStockProcessor.Run(ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowUCControl()
        {
            try
            {
                long timeToTal = 0;
                long startTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd000000"));
                int key = 0;
                int count = 0;

                if (chkAlertExpired.Checked)
                {
                    key = 4;
                    timeToTal = Inventec.Common.TypeConvert.Parse.ToInt64(
                       Convert.ToDateTime(DateTime.Now.AddYears(10)).ToString("yyyyMMdd235959"));
                }
                else
                {
                    if (cboExpriedDate.SelectedIndex == 0)
                    {
                        key = 0;
                        timeToTal = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd235959"));
                    }
                    else if (cboExpriedDate.SelectedIndex == 1)
                    {
                        key = 1;
                        timeToTal = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(DateTime.Now.AddDays(29)).ToString("yyyyMMdd235959"));

                    }
                    else if (cboExpriedDate.SelectedIndex == 2)
                    {
                        key = 2;
                        timeToTal = Inventec.Common.TypeConvert.Parse.ToInt64(
                       Convert.ToDateTime(DateTime.Now.AddDays(89)).ToString("yyyyMMdd235959"));

                    }
                    else if (cboExpriedDate.SelectedIndex == 3)
                    {
                        key = 3;
                        timeToTal = Inventec.Common.TypeConvert.Parse.ToInt64(
                       Convert.ToDateTime(DateTime.Now.AddDays(179)).ToString("yyyyMMdd235959"));
                    }
                    else if (cboExpriedDate.SelectedIndex == 4)
                    {
                        key = 4;
                        if (dateEdit1.EditValue != null && dateEdit1.DateTime != DateTime.MinValue)
                        {
                            timeToTal = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyyMMdd235959"));
                        }
                    }
                }
                WaitingManager.Show();
                if (this.mediStockIds == null || this.mediStockIds.Count == 0)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn kho", "Thông báo");
                    return;
                }
                bool isIncludeBaseAmount = (this.mediStockIds.Count == 1 && BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Any(a => a.ID == mediStockIds.FirstOrDefault() && a.IS_CABINET == 1));
                CommonParam param = new CommonParam();
                this.panelControlMediMate.Controls.Clear();
                if (chkMedicine.Checked)
                {
                    if (this.ucMedicineInfo != null)
                    {
                        this.panelControlMediMate.Controls.Add(this.ucMedicineInfo);
                        this.ucMedicineInfo.Dock = DockStyle.Fill;

                        //Thuốc
                        MOS.Filter.HisMedicineStockViewFilter mediFilter = new MOS.Filter.HisMedicineStockViewFilter();
                        if (chkMediStock.Checked)
                        {
                            mediFilter.INCLUDE_MEDI_STOCK = true;
                        }
                        else
                        {
                            mediFilter.INCLUDE_MEDI_STOCK = false;
                        }

                        mediFilter.INCLUDE_EMPTY = chkViewLineZero.Checked;
                        mediFilter.MEDI_STOCK_IDs = this.mediStockIds;
                        mediFilter.INCLUDE_BASE_AMOUNT = isIncludeBaseAmount;
                        var lstMediInStocks = new BackendAdapter(param).Get<List<List<HisMedicineInStockSDO>>>(HisRequestUriStore.HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE_BY_EXPIRED_DATE, ApiConsumers.MosConsumer, mediFilter, param);
                        List<long> _MedicineTypeIds = new List<long>();
                        List<V_HIS_MEDI_STOCK> lstMediStock = new List<V_HIS_MEDI_STOCK>();
                        if (chkMediStock.Checked)
                        {
                            List<long> MediStockIds = lstMediInStocks.SelectMany(x => x).Select(x => x.MEDI_STOCK_ID ?? 0).Distinct().ToList();
                            lstMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => MediStockIds.Contains(p.ID)).ToList();
                        }

                        if (lstMediInStocks != null && lstMediInStocks.Count > 0 && timeToTal > 0)
                        {

                            if (key == 0 || key == 4)
                            {
                                lstMediInStocks = lstMediInStocks.Select(o => o.Where(p => p.EXPIRED_DATE != null && p.EXPIRED_DATE < timeToTal).ToList()).ToList();
                            }
                            else
                            {
                                lstMediInStocks = lstMediInStocks.Select(o => o.Where(p => p.EXPIRED_DATE != null && p.EXPIRED_DATE >= startTime && p.EXPIRED_DATE <= timeToTal).ToList()).ToList();
                            }
                        }
                        // --- BẮT ĐẦU ĐOẠN CODE ĐÃ TỐI ƯU ---
                        if (chkAlertExpired.Checked && lstMediInStocks != null && lstMediInStocks.Count > 0)
                        {
                            var medicineTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>();
                            DateTime nowDate = DateTime.Today;

                            // List chứa kết quả cuối cùng (giữ nguyên cấu trúc Group của API)
                            var newGroupedList = new List<List<HisMedicineInStockSDO>>();
                            var finalVisibleTypeIds = new HashSet<long>(); // Danh sách ID để hiển thị cây

                            // Duyệt qua từng nhóm NGÀY (theo cấu trúc trả về từ API)
                            foreach (var group in lstMediInStocks)
                            {
                                if (group == null || group.Count == 0) continue;

                                // BƯỚC 1: Tìm các thuốc con (LÁ) trong nhóm này thỏa mãn điều kiện
                                var validLeavesInGroup = new List<HisMedicineInStockSDO>();
                                foreach (var stock in group)
                                {
                                    if (string.IsNullOrWhiteSpace(stock.MEDICINE_TYPE_NAME) || stock.EXPIRED_DATE == null) continue;
                                    var type = medicineTypes.FirstOrDefault(mt => mt.ID == stock.MEDICINE_TYPE_ID);

                                    // Chỉ kiểm tra điều kiện ngày tháng với THUỐC (IS_LEAF = 1)
                                    if (type != null && type.ALERT_EXPIRED_DATE != null && type.IS_LEAF == 1)
                                    {
                                        string expDateStr = stock.EXPIRED_DATE.Value.ToString();
                                        if (expDateStr.Length >= 8)
                                        {
                                            DateTime expDate;
                                            if (DateTime.TryParseExact(expDateStr.Substring(0, 8), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out expDate))
                                            {
                                                int daysRemaining = (expDate - nowDate).Days;
                                                // Nếu thỏa mãn điều kiện cảnh báo -> Giữ lại
                                                if (type.ALERT_EXPIRED_DATE.Value >= daysRemaining)
                                                {
                                                    validLeavesInGroup.Add(stock);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (validLeavesInGroup.Count == 0) continue;

                                var neededTypeIds = new HashSet<long>();
                                foreach (var leaf in validLeavesInGroup)
                                {
                                    neededTypeIds.Add(leaf.MEDICINE_TYPE_ID);
                                    var currentType = medicineTypes.FirstOrDefault(mt => mt.ID == leaf.MEDICINE_TYPE_ID);
                                    while (currentType != null && currentType.PARENT_ID != null)
                                    {
                                        neededTypeIds.Add(currentType.PARENT_ID.Value);
                                        currentType = medicineTypes.FirstOrDefault(x => x.ID == currentType.PARENT_ID);
                                    }
                                    if (currentType != null) neededTypeIds.Add(currentType.ID);
                                }

                                var filteredGroup = new List<HisMedicineInStockSDO>();

                                filteredGroup.AddRange(validLeavesInGroup);

                                foreach (var stock in group)
                                {
                                    if (validLeavesInGroup.Contains(stock)) continue;

                                    var type = medicineTypes.FirstOrDefault(mt => mt.ID == stock.MEDICINE_TYPE_ID);

                                    if (type != null && neededTypeIds.Contains(type.ID) && type.IS_LEAF != 1)
                                    {
                                        filteredGroup.Add(stock);
                                    }
                                }

                                if (filteredGroup.Count > 0)
                                {
                                    newGroupedList.Add(filteredGroup);
                                    foreach (var item in filteredGroup) finalVisibleTypeIds.Add(item.MEDICINE_TYPE_ID);
                                }
                            }

                            lstMediInStocks = newGroupedList;
                            _MedicineTypeIds = finalVisibleTypeIds.ToList();
                        }
                        else
                        {
                            if (lstMediInStocks != null && lstMediInStocks.Count > 0)
                            {
                                var dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS == 1).ToList();
                                if (dataMediStocks != null && dataMediStocks.Count == this.mediStockIds.Count)
                                {
                                    var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                                    if (dataMediTypes != null && dataMediTypes.Count > 0)
                                    {
                                        _MedicineTypeIds = dataMediTypes.Select(o => o.ID).Distinct().ToList();
                                    }
                                    else
                                    {
                                        lstMediInStocks = new List<List<HisMedicineInStockSDO>>();
                                    }
                                }
                                else
                                {
                                    var dataMediStocksBUSINESS = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS != 1).ToList();
                                    if (dataMediStocksBUSINESS != null && dataMediStocksBUSINESS.Count == this.mediStockIds.Count)
                                    {
                                        var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                                        if (dataMediTypes != null && dataMediTypes.Count > 0)
                                        {
                                            _MedicineTypeIds = dataMediTypes.Select(o => o.ID).Distinct().ToList();
                                        }
                                        else
                                        {
                                            lstMediInStocks = new List<List<HisMedicineInStockSDO>>();
                                        }
                                    }
                                }
                            }
                        }

                        List<List<HisMedicineInStockSDO>> lstParent = new List<List<HisMedicineInStockSDO>>();
                        for (int i = 0; i < lstMediInStocks.Count; i++)
                        {
                            if (lstMediInStocks[i] != null && lstMediInStocks[i].Count > 0)
                            {
                                lstParent.Add(lstMediInStocks[i]);

                            }
                        }

                        hisMediInStockProcessor.Reload(ucMedicineInfo, lstParent, _MedicineTypeIds, lstMediStock);
                        var list = hisMediInStockProcessor.GetListTreeView(ucMedicineInfo);
                        count = list != null && list.Count > 0 ? list.Where(o => o.IS_MEDI_MATE).ToList().Count() : 0;

                    }
                }
                else if (chkMaterial.Checked)
                {
                    if (this.ucMaterialInfo != null)
                    {
                        this.panelControlMediMate.Controls.Add(this.ucMaterialInfo);
                        this.ucMaterialInfo.Dock = DockStyle.Fill;

                        //Vật tư
                        MOS.Filter.HisMaterialStockViewFilter mateFilter = new MOS.Filter.HisMaterialStockViewFilter();
                        if (chkMediStock.Checked)
                        {
                            mateFilter.INCLUDE_MEDI_STOCK = true;
                        }
                        else
                        {
                            mateFilter.INCLUDE_MEDI_STOCK = false;
                        }

                        mateFilter.INCLUDE_EMPTY = chkViewLineZero.Checked;
                        mateFilter.MEDI_STOCK_IDs = this.mediStockIds;
                        mateFilter.INCLUDE_BASE_AMOUNT = isIncludeBaseAmount;
                        var lstMateInStocks = new BackendAdapter(param).Get<List<List<HisMaterialInStockSDO>>>(HisRequestUriStore.HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE_EXPRIRED_DATE, ApiConsumers.MosConsumer, mateFilter, param);

                        List<long> _MaterialTypeIds = new List<long>();
                        List<V_HIS_MEDI_STOCK> lstMediStock = new List<V_HIS_MEDI_STOCK>();
                        if (chkMediStock.Checked)
                        {
                            List<long> MediStockIds = lstMateInStocks.SelectMany(x => x).Select(x => x.MEDI_STOCK_ID ?? 0).Distinct().ToList();
                            lstMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => MediStockIds.Contains(p.ID)).ToList();
                        }
                        if (lstMateInStocks != null && lstMateInStocks.Count > 0 && timeToTal > 0)
                        {
                            if (key == 0 || key == 4)
                            {
                                lstMateInStocks = lstMateInStocks.Select(o => o.Where(p => p.EXPIRED_DATE != null && p.EXPIRED_DATE < timeToTal).ToList()).ToList();
                            }
                            else
                            {
                                lstMateInStocks = lstMateInStocks.Select(o => o.Where(p => p.EXPIRED_DATE != null && p.EXPIRED_DATE >= startTime && p.EXPIRED_DATE <= timeToTal).ToList()).ToList();
                            }

                        }
                        if (chkAlertExpired.Checked && lstMateInStocks != null && lstMateInStocks.Count > 0)
                        {
                            var materialTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                            DateTime nowDate = DateTime.Today;

                            lstMateInStocks = lstMateInStocks
                                // flatten
                                .SelectMany(stockList => stockList)
                                .Where(material =>
                                {
                                    if (material.EXPIRED_DATE == null)
                                        return false;

                                    var materialType = materialTypes
                                        .FirstOrDefault(mt => mt.ID == material.MATERIAL_TYPE_ID);

                                    if (materialType == null || materialType.ALERT_EXPIRED_DATE == null)
                                        return false;

                                    string expDateStr = material.EXPIRED_DATE.Value.ToString();
                                    if (expDateStr.Length < 8)
                                        return false;

                                    expDateStr = expDateStr.Substring(0, 8);

                                    if (!DateTime.TryParseExact(
                                            expDateStr,
                                            "yyyyMMdd",
                                            null,
                                            System.Globalization.DateTimeStyles.None,
                                            out DateTime expDate))
                                        return false;

                                    int daysRemaining = (expDate - nowDate).Days;

                                    return materialType.ALERT_EXPIRED_DATE.Value >= daysRemaining;
                                })

                                // tránh trùng vật tư
                                .GroupBy(x => x.ID)
                                .Select(g => g.First())

                                // group lại theo EXPIRED_DATE để build tree node cha (GIỐNG THUỐC)
                                .GroupBy(x => x.EXPIRED_DATE)
                                .Select(g => g.ToList())
                                .ToList();

                            // MaterialTypeIds cho node cha
                            _MaterialTypeIds = lstMateInStocks
                                .SelectMany(x => x)
                                .Select(x => x.MATERIAL_TYPE_ID)
                                .Distinct()
                                .ToList();
                        }


                        if (lstMateInStocks != null && lstMateInStocks.Count > 0)
                        {
                            var dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS == 1).ToList();
                            if (dataMediStocks != null && dataMediStocks.Count == this.mediStockIds.Count)
                            {
                                var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                                if (dataMateTypes != null && dataMateTypes.Count > 0)
                                {
                                    // lstMateInStocks = lstMateInStocks.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MATERIAL_TYPE_ID)).ToList();
                                    _MaterialTypeIds = dataMateTypes.Select(o => o.ID).Distinct().ToList();
                                }
                                else
                                {
                                    lstMateInStocks = new List<List<HisMaterialInStockSDO>>();
                                }
                            }
                            else
                            {
                                var dataMediStocksBUSINESS = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS != 1).ToList();
                                if (dataMediStocksBUSINESS != null && dataMediStocksBUSINESS.Count == this.mediStockIds.Count)
                                {
                                    var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                                    if (dataMateTypes != null && dataMateTypes.Count > 0)
                                    {
                                        _MaterialTypeIds = dataMateTypes.Select(o => o.ID).Distinct().ToList();
                                    }
                                    else
                                    {
                                        lstMateInStocks = new List<List<HisMaterialInStockSDO>>();
                                    }
                                }
                            }
                        }


                        List<List<HisMaterialInStockSDO>> lstParent = new List<List<HisMaterialInStockSDO>>();
                        if (lstMateInStocks != null && lstMateInStocks.Count > 0)
                        {
                            for (int i = 0; i < lstMateInStocks.Count; i++)
                            {
                                if (lstMateInStocks[i] != null && lstMateInStocks[i].Count > 0)
                                {
                                    lstParent.Add(lstMateInStocks[i]);
                                }
                            }
                        }

                        hisMateInStockProcessor.Reload(ucMaterialInfo, lstParent, _MaterialTypeIds, lstMediStock);
                        var list = hisMateInStockProcessor.GetListTreeView(ucMaterialInfo);
                        count = list != null && list.Count > 0 ? list.Where(o => o.IS_MEDI_MATE).ToList().Count() : 0;
                    }
                }
                txtCount.Text = count.ToString();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataGridMediStock()
        {
            try
            {
                WaitingManager.Show();
                gridControlMediStock.DataSource = null;
                var _WorkPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.RoomId);
                if (_WorkPlace != null)
                {

                    var datas = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(p => p.LOGINNAME.Trim() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim() && _WorkPlace.BranchId == p.BRANCH_ID).ToList();

                    if (datas != null)
                    {
                        List<long> roomIds = datas.Select(p => p.ROOM_ID).ToList();
                        dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => roomIds.Contains(p.ROOM_ID)).ToList();
                    }

                    //var _MediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => p.ROOM_ID == _WorkPlace.RoomId).OrderBy(p => p.MEDI_STOCK_NAME).ToList();
                    if (dataMediStocks != null && dataMediStocks.Count > 0)
                    {
                        gridControlMediStock.DataSource = dataMediStocks;
                        var data = dataMediStocks.FirstOrDefault(p => p.ROOM_ID == this.RoomId);
                        if (data != null)
                        {
                            this.mediStockIds.Clear();
                            this.mediStockIds.Add(data.ID);
                            int index = dataMediStocks.FindIndex(a => a.ID == data.ID);
                            gridViewMediStock.SelectRow(index);
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStock_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_MEDI_STOCK)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataGridMediStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataGridMediStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (gridViewMediStock.FocusedRowHandle < 0)
                    return;
                ProcessClickViewMediStock(sender, gridViewMediStock.FocusedRowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStock_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                ProcessClickViewMediStock(sender, e.RowHandle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessClickViewMediStock(object sender, int rowHandle)
        {
            try
            {
                WaitingManager.Show();
                this.currentMediStock = null;
                this.currentMediStock = (V_HIS_MEDI_STOCK)((IList)((BaseView)sender).DataSource)[rowHandle];
                //ShowUCControl();
                ///FillDataToControlBySelectMediStock();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControlBySelectMediStock()
        {
            try
            {
                if (this.currentMediStock != null)
                {
                    CommonParam param = new CommonParam();
                    //Thuốc
                    MOS.Filter.HisMedicineStockViewFilter mediFilter = new MOS.Filter.HisMedicineStockViewFilter();
                    mediFilter.MEDI_STOCK_ID = this.currentMediStock.ID;
                    lstMediInStocks = new List<HisMedicineInStockSDO>();
                    lstMediInStocks = new BackendAdapter(param).Get<List<HisMedicineInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE, ApiConsumers.MosConsumer, mediFilter, param);

                    //Vật tư
                    MOS.Filter.HisMaterialStockViewFilter mateFilter = new MOS.Filter.HisMaterialStockViewFilter();
                    mateFilter.MEDI_STOCK_ID = this.currentMediStock.ID;
                    lstMateInStocks = new List<HisMaterialInStockSDO>();
                    lstMateInStocks = new BackendAdapter(param).Get<List<HisMaterialInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE, ApiConsumers.MosConsumer, mateFilter, param);

                    if (true)
                    {
                        //hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocks);
                    }
                    else
                    {
                        // hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocks);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStock_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                //List<V_HIS_MEDI_STOCK> lstMediStock = new List<V_HIS_MEDI_STOCK>();
                //if (gridViewMediStock.RowCount > 0)
                //{
                //    for (int i = 0; i < gridViewMediStock.SelectedRowsCount; i++)
                //    {
                //        if (gridViewMediStock.GetSelectedRows()[i] >= 0)
                //        {
                //            lstMediStock.Add((V_HIS_MEDI_STOCK)gridViewMediStock.GetRow(gridViewMediStock.GetSelectedRows()[i]));
                //        }
                //    }
                //}
                if (gridViewMediStock.RowCount > 0)
                {
                    this.mediStockIds.Clear();
                    for (int i = 0; i < gridViewMediStock.SelectedRowsCount; i++)
                    {
                        if (gridViewMediStock.GetSelectedRows()[i] >= 0)
                        {
                            this.mediStockIds.Add(Inventec.Common.TypeConvert.Parse.ToInt64(gridViewMediStock.GetRowCellValue(gridViewMediStock.GetSelectedRows()[i], "ID").ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // === THAY THẾ TOÀN BỘ 3 METHODS CỦA BẠN BẰNG ĐOẠN NÀY ===

        private void chkMedicine_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkMedicine.Checked)
                {
                    if (!isNotLoadWhileChangeControlStateInFirst)
                    {
                        SaveControlState();
                    }

                    // Kiểm tra đã chọn kho chưa trước khi hiển thị
                    if (this.mediStockIds != null && this.mediStockIds.Count > 0)
                    {
                        ShowUCControl();
                    }

                    isCheck = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMaterial_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkMaterial.Checked)
                {
                    if (!isNotLoadWhileChangeControlStateInFirst)
                    {
                        SaveControlState();
                    }

                    // Kiểm tra đã chọn kho chưa trước khi hiển thị
                    if (this.mediStockIds != null && this.mediStockIds.Count > 0)
                    {
                        ShowUCControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void chkAlertExpired_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!isNotLoadWhileChangeControlStateInFirst)
                {
                    SaveControlState();
                    ShowUCControl();
                }
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
                //PrintProcess(PrintType.IN_PHIEU_TONG_HOP_TON_KHO_T_VT_M);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            try
            {
                ShowUCControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click_1(object sender, EventArgs e)
        {
            try
            {
                this.mediStockIds = new List<long>();
                chkAlertExpired.Checked = false;
                LoadDataGridMediStock();
                hisMediInStockProcessor.Reload(ucMedicineInfo, null, null, null);
                hisMateInStockProcessor.Reload(ucMaterialInfo, null, null, null);
                hisBloodProcessor.Reload(ucBloodInfo, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSearch()
        {
            try
            {
                btnSearch_Click_1(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnRefesh()
        {
            try
            {
                btnRefesh_Click_1(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkMaterial.Checked)
                {
                    hisMateInStockProcessor.ExportExcel(ucMaterialInfo);
                }
                else if (chkMedicine.Checked)
                {
                    hisMediInStockProcessor.ExportExcel(ucMedicineInfo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCbo()
        {
            try
            {
                cboExpriedDate.Properties.Items.AddRange(new object[] { "Đã hết hạn", "< 1 tháng", "< 3 tháng", "< 6 tháng", "Trước thời điểm" });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpriedDate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboExpriedDate.EditValue != null && cboExpriedDate.SelectedIndex == 4)
                {
                    dateEdit1.Enabled = true;
                    dateEdit1.DateTime = DateTime.Now;
                    dateEdit1.ShowPopup();
                }
                else
                {
                    dateEdit1.Enabled = false;
                    dateEdit1.EditValue = null;
                    ShowUCControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpriedDate_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboExpriedDate.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = cboExpriedDate.SelectedIndex.ToString();
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = cboExpriedDate.Name;
                    csAddOrUpdate.VALUE = cboExpriedDate.SelectedIndex.ToString();
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);


                if (cboExpriedDate.EditValue == null)
                {
                    dateEdit1.EditValue = null;
                    dateEdit1.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dateEdit1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                ShowUCControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpriedDate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboExpriedDate.EditValue = null;
                    dateEdit1.EditValue = null;
                    dateEdit1.Enabled = false;
                    ShowUCControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dateEdit1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {

                    ShowUCControl();

                }

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
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchClick(strValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchClick(string keyword)
        {
            try
            {
                List<V_HIS_MEDI_STOCK> listResult;

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim();

                    listResult = this.dataMediStocks.Where(o => (!string.IsNullOrEmpty(o.DEPARTMENT_NAME) && o.DEPARTMENT_NAME.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                                                             || (!string.IsNullOrEmpty(o.MEDI_STOCK_CODE) && o.MEDI_STOCK_CODE.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                                                             || (!string.IsNullOrEmpty(o.MEDI_STOCK_NAME) && o.MEDI_STOCK_NAME.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)).OrderBy(o => o.MEDI_STOCK_CODE).Distinct().ToList();
                }
                else
                {
                    listResult = this.dataMediStocks.OrderBy(o => o.MEDI_STOCK_CODE).ToList();
                }

                gridControlMediStock.BeginUpdate();
                gridControlMediStock.DataSource = listResult;
                gridControlMediStock.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
