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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MediStockSummary.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockSummary.CreateReport
{
    public partial class frmMediCardByDateReport : HIS.Desktop.Utility.FormBase
    {
        string ReportTypeCode;

        int positionHandleControl = -1;

        MOS.SDO.HisMedicineInStockSDO MedicineType;
        MOS.SDO.HisMaterialInStockSDO MaterialType;
        MOS.SDO.HisBloodInStockSDO BloodType;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK mediStock;

        internal FilterADO _ReportFilter { get; set; }
        long RoomId;
        long? MediMateId;
        public frmMediCardByDateReport()
        {
            InitializeComponent();
        }

        public frmMediCardByDateReport(long roomId, string reportTypeCode, MOS.SDO.HisMedicineInStockSDO medicineType, MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock, bool isMedicine)
        {
            InitializeComponent();
            try
            {
                this.RoomId = roomId;
                this.MedicineType = medicineType;
                this.mediStock = medistock;
                this.ReportTypeCode = reportTypeCode;
                if (isMedicine)
                {
                    this.MediMateId = medicineType.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmMediCardByDateReport(long roomId, string reportTypeCode, MOS.SDO.HisMaterialInStockSDO materialType, MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock, bool isMaterial)
        {
            InitializeComponent();
            try
            {
                this.RoomId = roomId;
                this.MaterialType = materialType;
                this.mediStock = medistock;
                this.ReportTypeCode = reportTypeCode;
                if (isMaterial)
                {
                    this.MediMateId = materialType.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmMediCardByDateReport(long roomId, string reportTypeCode, MOS.SDO.HisBloodInStockSDO bloodType, MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock, bool isBlood)
        {
            InitializeComponent();
            try
            {
                this.RoomId = roomId;
                this.BloodType = bloodType;
                this.mediStock = medistock;
                this.ReportTypeCode = reportTypeCode;
                if (isBlood)
                {
                    this.MediMateId = BloodType.Id;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void frmMediCardByDateReport_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetIconFrm();
                if (this.MedicineType != null)
                {
                    if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_THUOC_THEO_NGAY)
                    {
                        this.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.form.Text1", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());//"Điều kiện lọc - Chi tiết thẻ kho thuốc theo ngày";
                    }
                    else if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_THUOC_THEO_NGAY)
                    {
                        this.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.form.Text2", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());//"Điều kiện lọc - Tổng hợp thẻ kho thuốc theo ngày";
                    }
                }
                else if (this.MaterialType != null)
                {
                    if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_VAT_TU_THEO_NGAY)
                    {
                        this.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.form.Text3", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());//"Điều kiện lọc - Chi tiết thẻ kho vật tư theo ngày";
                    }
                    else if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_VAT_TU_THEO_NGAY)
                    {
                        this.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.form.Text4", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());//"Điều kiện lọc - Tổng hợp thẻ kho vật tư theo ngày";
                    }
                }
                else if (this.BloodType != null)
                {
                    if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_MAU_THEO_NGAY)
                    {
                        this.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.form.Text5", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());//"Điều kiện lọc - Chi tiết thẻ kho máu theo ngày";
                    }
                    else if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_MAU_THEO_NGAY)
                    {
                        this.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.form.Text6", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());//"Điều kiện lọc - Tổng hợp thẻ kho máu theo ngày";
                    }
                }
                //dtTimeFrom.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //dtTimeTo.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                dtTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                if (mediStock != null)
                {
                    lblMediStockCode.Text = mediStock.MEDI_STOCK_CODE;
                    lblMediStockName.Text = mediStock.MEDI_STOCK_NAME;
                }
                if (this.MedicineType != null)
                {
                    var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == this.MedicineType.MEDICINE_TYPE_ID);
                    if (data != null)
                    {
                        this.MedicineType.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                        this.MedicineType.NATIONAL_NAME = data.NATIONAL_NAME;
                        this.MedicineType.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                    }
                    lciMediMateCode.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.medicine.Text1", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    lciMediMateName.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.medicine.Text2", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    lblReportTypeCode.Text = this.MedicineType.MEDICINE_TYPE_CODE;
                    lblReportTypeName.Text = this.MedicineType.MEDICINE_TYPE_NAME;

                    if (this.MediMateId.HasValue)
                    {
                        this.lciExpiredDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.lciPackageNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.lblPackageNumber.Text = this.MedicineType.PACKAGE_NUMBER;
                        this.lblExpiredDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(this.MedicineType.EXPIRED_DATE ?? 0));
                    }
                }
                else if (this.MaterialType != null)
                {
                    var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == this.MaterialType.MATERIAL_TYPE_ID);
                    if (data != null)
                    {
                        this.MaterialType.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                        this.MaterialType.NATIONAL_NAME = data.NATIONAL_NAME;
                        this.MaterialType.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                    }
                    lciMediMateCode.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.material.Text1", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    lciMediMateName.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.material.Text2", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    lblReportTypeCode.Text = this.MaterialType.MATERIAL_TYPE_CODE;
                    lblReportTypeName.Text = this.MaterialType.MATERIAL_TYPE_NAME;

                    if (this.MediMateId.HasValue)
                    {
                        this.lciExpiredDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.lciPackageNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.lblPackageNumber.Text = this.MaterialType.PACKAGE_NUMBER;
                        this.lblExpiredDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(this.MaterialType.EXPIRED_DATE ?? 0));
                    }
                }
                else if (this.BloodType != null)
                {
                    lciMediMateCode.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.blood.Text1", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    lciMediMateName.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.blood.Text2", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    lblReportTypeCode.Text = this.BloodType.BloodTypeCode;
                    lblReportTypeName.Text = this.BloodType.BloodTypeName;

                    if (this.MediMateId.HasValue)
                    {
                        this.lciExpiredDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.lciPackageNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        //this.lblExpiredDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(this.BloodType.EXPIRED_DATE ?? 0));
                    }
                }
                ValidControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmMediCardByDateReport
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MediStockSummary.Resources.Lang", typeof(frmMediCardByDateReport).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMediMateCode.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.lciMediMateCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMediMateName.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.lciMediMateName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPackageNumber.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.lciPackageNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpiredDate.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.lciExpiredDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.barButtonItem__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMediCardByDateReport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();

                this._ReportFilter = new FilterADO();
                _ReportFilter.MEDI_STOCK_ID = this.mediStock.ID;
                _ReportFilter.MediMateId = this.MediMateId;
                if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                {
                    //_ReportFilter.TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    _ReportFilter.TIME_FROM = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeFrom.DateTime) ?? 0;

                }
                if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                {
                    //_ReportFilter.TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                    _ReportFilter.TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeTo.DateTime) ?? 0;

                }

                if (this.MedicineType != null)
                {
                    _ReportFilter.MEDICINE_TYPE_ID = this.MedicineType.MEDICINE_TYPE_ID;
                }
                else if (this.MaterialType != null)
                {
                    _ReportFilter.MATERIAL_TYPE_ID = this.MaterialType.MATERIAL_TYPE_ID;
                }
                else if (this.BloodType != null)
                {
                    _ReportFilter.BLOOD_TYPE_ID = this.BloodType.BloodTypeId;
                }

                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                if (this.MedicineType != null)
                {
                    if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_THUOC_THEO_NGAY)
                    {
                        richEditorMain.RunPrintTemplate(KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_THUOC_THEO_NGAY, DelegateRunPrinter);
                    }
                    else if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_THUOC_THEO_NGAY)
                    {
                        richEditorMain.RunPrintTemplate(KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_THUOC_THEO_NGAY, DelegateRunPrinter);
                    }
                }
                else if (this.MaterialType != null)
                {
                    if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_VAT_TU_THEO_NGAY)
                    {
                        richEditorMain.RunPrintTemplate(KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_VAT_TU_THEO_NGAY, DelegateRunPrinter);
                    }
                    else if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_VAT_TU_THEO_NGAY)
                    {
                        richEditorMain.RunPrintTemplate(KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_VAT_TU_THEO_NGAY, DelegateRunPrinter);
                    }
                }
                else if (this.BloodType != null)
                {
                    if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_MAU_THEO_NGAY)
                    {
                        richEditorMain.RunPrintTemplate(KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_MAU_THEO_NGAY, DelegateRunPrinter);
                    }
                    else if (ReportTypeCode == KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_MAU_THEO_NGAY)
                    {
                        richEditorMain.RunPrintTemplate(KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_MAU_THEO_NGAY, DelegateRunPrinter);
                    }
                }
                // success = new BackendAdapter(param).Post<bool>("api/MrsReport/Create", ApiConsumers.MrsConsumer, sarReport, param);
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
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_THUOC_THEO_NGAY:
                        Mrs00067 pro = new Mrs00067(this.RoomId);
                        pro.LoadDataPrint217(this._ReportFilter, printTypeCode, fileName, ref result);
                        break;
                    case KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_THUOC_THEO_NGAY:
                        Mrs00075 pro75 = new Mrs00075(this.RoomId);
                        pro75.LoadDataPrint218(this._ReportFilter, printTypeCode, fileName, ref result);
                        break;
                    case KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_VAT_TU_THEO_NGAY:
                        Mrs00085 pro85 = new Mrs00085(this.RoomId);
                        pro85.LoadDataPrint220(this._ReportFilter, printTypeCode, fileName, ref result);
                        break;
                    case KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_VAT_TU_THEO_NGAY:
                        Mrs00076 pro76 = new Mrs00076(this.RoomId);
                        pro76.LoadDataPrint219(this._ReportFilter, printTypeCode, fileName, ref result);
                        break;
                    case KeyConfigReport.REPORT_TYPE_CODE_TONG_HOP_THE_KHO_MAU_THEO_NGAY:
                        Mps482 pro482 = new Mps482(this.RoomId);
                        pro482.LoadDataPrint482(this._ReportFilter, printTypeCode, fileName, ref result);
                        break;
                    case KeyConfigReport.REPORT_TYPE_CODE_CHI_TIET_THE_KHO_MAU_THEO_NGAY:
                        Mps483 pro483 = new Mps483(this.RoomId);
                        pro483.LoadDataPrint483(this._ReportFilter, printTypeCode, fileName, ref result);
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
    }
}
