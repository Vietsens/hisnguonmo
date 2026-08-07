using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using EMR.WCF.DCO;
using HIS.Desktop.ADO;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.HisImportXmlAdjust.ADO;
using HIS.Desktop.Plugins.HisImportXmlAdjust.Message;
using HIS.Desktop.Plugins.HisImportXmlAdjust.XML;
using HIS.UC.SettingSignInfo;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Common.SignLibrary.ServiceSign;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.HisImportXmlAdjust
{
    public partial class frmImportXmlAdjust : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        Inventec.Desktop.Common.Modules.Module currentModule;
        RefeshReference delegateRefresh;
        List<XmlAdjustADO> _XmlAdjustAdos;
        const string DATE_FORMAT = "dd/MM/yyyy HH:mm";
        const string CONFIG_KEY_CONNECTION_INFO = "HIS.QD_130_BYT.CONNECTION_INFO";
        // Cấu hình riêng cổng 09/BH (address|username|password). Nếu trống thì dùng lại config XML130.
        const string CONFIG_KEY_09BH_CONNECTION_INFO = "HIS.HSDC_09BH.CONNECTION_INFO";
        const string TAG_CHUKYDONVI = "CHUKYDONVI";
        const string TAG_TT_HOSO = "TT_HOSO";
        const string XMLDSIG_NAMESPACE = "http://www.w3.org/2000/09/xmldsig#";
        const string TEMP_FOLDER_NAME = "HisImportXmlAdjust";
        // "thẻ chứa chữ ký|thẻ được Reference trỏ tới". Dấu '|' chính là dấu hiệu để thư viện ký số
        // (Inventec.Common.SignFile.SignData) hiểu đây là hồ sơ mẫu 09/BH và ký bằng SignXml09BH, KHÔNG phải XML130.
        const string FIELD_SIGNED_09BH = TAG_CHUKYDONVI + "|" + TAG_TT_HOSO;
        // true  = ưu tiên ký qua service EMR.SignProcessor bằng hàm ký riêng mẫu 09/BH (SignXml09BH); nếu service
        //         không dùng được / trả về chữ ký sai profile thì tự động lùi về ký tại máy trạm (Sign\Xml09BHSigner.cs)
        // false = ký thẳng tại máy trạm, không gọi service
        const bool PREFER_SERVICE_SIGNER_09BH = true;
        string[] DATE_FORMATS = new string[] { "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss", "d/M/yyyy HH:mm", "d/M/yyyy H:mm", "dd/MM/yyyy", "yyyyMMddHHmm", "yyyyMMddHHmmss", "yyyyMMdd" };

        SettingSignADO SettingSignADO;
        // Đặt bởi CheckSignPrerequisite để SignFile không phải quét lại danh sách process cho từng hồ sơ
        private bool signServiceVerified;
        // null = chưa biết, true/false = service ký số có/không có operation riêng SignXml09BH.
        // Dò một lần rồi nhớ lại để không ném ActionNotSupportedException cho từng hồ sơ.
        private bool? signService09BHSupported;
        private bool isNotLoadWhileChangeControlStateInFirst;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;

        public frmImportXmlAdjust()
        {
            InitializeComponent();
        }

        public frmImportXmlAdjust(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this.currentModule = _module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmImportXmlAdjust(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this.currentModule = _module;
                this.delegateRefresh = _delegateRefresh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmImportXmlAdjust_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                btnImport.Enabled = false;
                btnPushPortal.Enabled = false;
                btnShowLineError.Enabled = false;
                InitControlState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled)
                    btnImport_Click(null, null);
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
                this.currentControlStateRDO = controlStateWorker.GetData(this.currentModule != null ? this.currentModule.ModuleLink : "");
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkSign.Name)
                        {
                            SettingSignADO = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingSignADO>(item.VALUE);
                            chkSign.Checked = SettingSignADO != null && !string.IsNullOrEmpty(SettingSignADO.SerialNumber);
                        }
                    }
                }
                else
                {
                    chkSign.Checked = false;
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                isNotLoadWhileChangeControlStateInFirst = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Tải file mẫu

        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_09BH.xlsx");
                CommonParam param = new CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_09BH";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog.FileName, true);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("Không tìm thấy file mẫu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Import Excel

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Excel Files|*.xls;*.xlsx";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _XmlAdjustAdos = new List<XmlAdjustADO>();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var rawData = import.GetWithCheck<XmlAdjustADO>(0);
                        if (rawData != null && rawData.Count > 0)
                        {
                            List<XmlAdjustADO> listAfterRemove = new List<XmlAdjustADO>();
                            foreach (var item in rawData)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.XML1_ID)
                                    && string.IsNullOrEmpty(item.EXPENSE_ID)
                                    && string.IsNullOrEmpty(item.XML_TABLE_NUMBER)
                                    && string.IsNullOrEmpty(item.LINK_CODE)
                                    && string.IsNullOrEmpty(item.XML_ORDER)
                                    && string.IsNullOrEmpty(item.PATIENT_CODE)
                                    && string.IsNullOrEmpty(item.PATIENT_NAME)
                                    && string.IsNullOrEmpty(item.HEIN_CARD_NUMBER)
                                    && string.IsNullOrEmpty(item.IN_DATE_STR)
                                    && string.IsNullOrEmpty(item.OUT_DATE_STR)
                                    && string.IsNullOrEmpty(item.ORDER_DATE_STR)
                                    && string.IsNullOrEmpty(item.ORIGINAL_FIELD)
                                    && string.IsNullOrEmpty(item.ORIGINAL_VALUE)
                                    && string.IsNullOrEmpty(item.ORIGINAL_REASON)
                                    && string.IsNullOrEmpty(item.REJECT_REASON)
                                    && string.IsNullOrEmpty(item.ADJUST_FIELD)
                                    && string.IsNullOrEmpty(item.ADJUST_VALUE)
                                    && string.IsNullOrEmpty(item.ADJUST_REASON)
                                    && string.IsNullOrEmpty(item.STATUS);
                                if (!checkNull)
                                    listAfterRemove.Add(item);
                            }

                            if (listAfterRemove.Count > 0)
                            {
                                ProcessValidation(listAfterRemove);
                                _XmlAdjustAdos = listAfterRemove;
                                SetDataSource(_XmlAdjustAdos);
                                CheckErrorLine();
                            }
                            else
                            {
                                XtraMessageBox.Show("File không có dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("File không có dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Không đọc được file Excel!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi xảy ra khi đọc file!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessValidation(List<XmlAdjustADO> dataList)
        {
            try
            {
                int index = 1;
                foreach (var item in dataList)
                {
                    string error = "";

                    // Kiểm tra bắt buộc: dòng có dữ liệu thì XML1_ID hoặc EXPENSE_ID phải có
                    bool hasAnyData = !string.IsNullOrEmpty(item.XML_TABLE_NUMBER)
                        || !string.IsNullOrEmpty(item.LINK_CODE)
                        || !string.IsNullOrEmpty(item.XML_ORDER)
                        || !string.IsNullOrEmpty(item.PATIENT_CODE)
                        || !string.IsNullOrEmpty(item.PATIENT_NAME)
                        || !string.IsNullOrEmpty(item.HEIN_CARD_NUMBER)
                        || !string.IsNullOrEmpty(item.IN_DATE_STR)
                        || !string.IsNullOrEmpty(item.OUT_DATE_STR)
                        || !string.IsNullOrEmpty(item.ORDER_DATE_STR)
                        || !string.IsNullOrEmpty(item.ORIGINAL_FIELD)
                        || !string.IsNullOrEmpty(item.ORIGINAL_VALUE)
                        || !string.IsNullOrEmpty(item.ORIGINAL_REASON)
                        || !string.IsNullOrEmpty(item.REJECT_REASON)
                        || !string.IsNullOrEmpty(item.ADJUST_FIELD)
                        || !string.IsNullOrEmpty(item.ADJUST_VALUE)
                        || !string.IsNullOrEmpty(item.ADJUST_REASON)
                        || !string.IsNullOrEmpty(item.STATUS);

                    if (hasAnyData)
                    {
                        // TẠM THỜI BỎ kiểm tra bắt buộc XML1_ID / ID chi phí (2 thẻ này không bắt buộc theo
                        // tài liệu 09/BH mục 7 và 9). Mở lại khi có yêu cầu siết dữ liệu đầu vào.
                        //if (string.IsNullOrEmpty(item.XML1_ID) && string.IsNullOrEmpty(item.EXPENSE_ID))
                        //    error += string.Format(MessageImport.ThieuTruongDL, "XML1_ID hoặc ID chi phí");
                        if (string.IsNullOrEmpty(item.PATIENT_CODE))
                            error += string.Format(MessageImport.ThieuTruongDL, "Mã bệnh nhân");
                        if (string.IsNullOrEmpty(item.PATIENT_NAME))
                            error += string.Format(MessageImport.ThieuTruongDL, "Họ tên");
                        if (string.IsNullOrEmpty(item.HEIN_CARD_NUMBER))
                            error += string.Format(MessageImport.ThieuTruongDL, "Mã thẻ");
                    }

                    if (!string.IsNullOrEmpty(item.IN_DATE_STR))
                    {
                        DateTime inDate;
                        if (DateTime.TryParseExact(item.IN_DATE_STR.Trim(), DATE_FORMATS,
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out inDate))
                        {
                            item.IN_DATE = inDate;
                        }
                        else
                        {
                            error += string.Format(MessageImport.DinhDangThoiGianSai, "Ngày vào");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.OUT_DATE_STR))
                    {
                        DateTime outDate;
                        if (DateTime.TryParseExact(item.OUT_DATE_STR.Trim(), DATE_FORMATS,
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out outDate))
                        {
                            item.OUT_DATE = outDate;
                        }
                        else
                        {
                            error += string.Format(MessageImport.DinhDangThoiGianSai, "Ngày ra");
                        }
                    }

                    // Parse ORDER_DATE
                    if (!string.IsNullOrEmpty(item.ORDER_DATE_STR))
                    {
                        DateTime orderDate;
                        if (DateTime.TryParseExact(item.ORDER_DATE_STR.Trim(), DATE_FORMATS,
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out orderDate))
                        {
                            item.ORDER_DATE = orderDate;
                        }
                        else
                        {
                            error += string.Format(MessageImport.DinhDangThoiGianSai, "Ngày y lệnh");
                        }
                    }

                    // Số bảng XML quyết định dòng vào phần nào của TT_DIEUCHINH:
                    //   1 (hoặc bỏ trống) = điều chỉnh thông tin hành chính -> DS_XML1_DIEUCHINH
                    //   2, 3              = điều chỉnh/huỷ chi phí          -> DSCP_DIEUCHINH (mục 9 chỉ nhận 2 hoặc 3)
                    string soBangXml = (item.XML_TABLE_NUMBER ?? "").Trim();
                    if (soBangXml.Length > 0 && soBangXml != "1" && soBangXml != "2" && soBangXml != "3")
                        error += "Số bảng XML chỉ nhận giá trị 1, 2 hoặc 3. ";

                    if (string.IsNullOrEmpty(item.ADJUST_FIELD))
                        error += string.Format(MessageImport.ThongTinDieuChinhBatBuoc, "Trường thông tin điều chỉnh");
                    if (string.IsNullOrEmpty(item.ADJUST_VALUE))
                        error += string.Format(MessageImport.ThongTinDieuChinhBatBuoc, "Thông tin điều chỉnh");
                    if (string.IsNullOrEmpty(item.ADJUST_REASON))
                        error += string.Format(MessageImport.ThongTinDieuChinhBatBuoc, "Lý do điều chỉnh");

                    item.ERROR = error;
                    item.ID = index++;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Grid

        private void SetDataSource(List<XmlAdjustADO> dataSource)
        {
            try
            {
                gridControlData.DataSource = null;
                gridControlData.DataSource = dataSource;
                gridViewData.BestFitColumns();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckErrorLine()
        {
            try
            {
                if (_XmlAdjustAdos == null || _XmlAdjustAdos.Count == 0)
                {
                    btnImport.Enabled = false;
                    btnPushPortal.Enabled = false;
                    btnShowLineError.Enabled = false;
                    return;
                }

                var hasError = _XmlAdjustAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                btnImport.Enabled = !hasError;
                btnPushPortal.Enabled = !hasError;
                btnShowLineError.Enabled = hasError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (_XmlAdjustAdos == null) return;

                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    SetDataSource(_XmlAdjustAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList());
                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    SetDataSource(_XmlAdjustAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "ERROR_")
                {
                    string error = (gridViewData.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (!string.IsNullOrEmpty(error))
                    {
                        e.RepositoryItem = repositoryItemButton_ER;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemTextEdit_Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "ERROR_" && e.IsGetData)
                {
                    var data = (XmlAdjustADO)((GridView)sender).GetRow(e.ListSourceRowIndex);
                    if (data != null && !string.IsNullOrEmpty(data.ERROR))
                    {
                        e.Value = data.ERROR;
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_ER_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (XmlAdjustADO)gridViewData.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.ERROR))
                {
                    string errorDisplay = row.ERROR.Replace("|", "\r\n");
                    XtraMessageBox.Show(errorDisplay, "Chi tiết lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (XmlAdjustADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (XtraMessageBox.Show("Bạn có chắc chắn muốn xóa dòng này?", "Xác nhận",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        _XmlAdjustAdos.Remove(row);
                        SetDataSource(_XmlAdjustAdos);
                        CheckErrorLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Xuất XML TT12

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (_XmlAdjustAdos == null || _XmlAdjustAdos.Count == 0)
                {
                    XtraMessageBox.Show("Chưa có dữ liệu. Hãy Import file Excel trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_XmlAdjustAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR)))
                {
                    XtraMessageBox.Show("Dữ liệu còn dòng lỗi. Hãy bấm \"Dòng lỗi\" để xem và sửa file Excel trước khi xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra điều kiện ký MỘT LẦN, trước khi mở hộp thoại chọn thư mục
                string signPrepareError;
                if (!CheckSignPrerequisite(false, out signPrepareError))
                {
                    XtraMessageBox.Show(signPrepareError, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string savePath = "";
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Chọn thư mục lưu file XML TT12 (mỗi hồ sơ 1 file)";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        savePath = fbd.SelectedPath;
                    }
                }
                if (string.IsNullOrEmpty(savePath)) return;

                WaitingManager.Show();

                // Mỗi bệnh nhân (MA_LK) = 1 file XML riêng (đúng tài liệu: 1 hồ sơ/lần)
                var hoSoList = BuildHoSoList();
                int okCount = 0;
                var errLines = new List<string>();
                var unsignedLines = new List<string>();
                string timestamp = DateTime.Now.ToString("ddMMyyyy_HHmmss");
                foreach (var xml in hoSoList)
                {
                    string maLk = GetMaLk(xml);
                    string saveFilePath = null;
                    try
                    {
                        string fileName = string.Format("HOSO_DIEUCHINH_GD_{0}_{1}.xml", SafeFileName(maLk), timestamp);
                        saveFilePath = Path.Combine(savePath, fileName);
                        var rs = CreateXmlFile(xml);
                        if (rs == null)
                        {
                            errLines.Add(string.Format("MA_LK {0}: không tạo được nội dung XML", maLk));
                            continue;
                        }
                        using (rs)
                        using (FileStream file = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write))
                        {
                            rs.WriteTo(file);
                        }

                        if (chkSign.Checked)
                        {
                            string signError;
                            if (!SignFile(fileName, saveFilePath, out signError))
                            {
                                // Đổi tên file chưa ký để không thể lẫn với file đã ký rồi gửi cổng
                                string unsignedPath = RenameToUnsigned(saveFilePath);
                                unsignedLines.Add(string.Format("MA_LK {0}: {1}{2}", maLk, signError,
                                    unsignedPath != null ? " (file để lại: " + Path.GetFileName(unsignedPath) + ")" : ""));
                                continue;
                            }
                        }
                        okCount++;
                    }
                    catch (Exception exItem)
                    {
                        errLines.Add(string.Format("MA_LK {0}: {1}", maLk, exItem.Message));
                        Inventec.Common.Logging.LogSystem.Error(exItem);
                        // File có thể đã bị ghi dở -> xóa để không để lại XML cắt cụt mang tên hợp lệ
                        try
                        {
                            if (!string.IsNullOrEmpty(saveFilePath) && File.Exists(saveFilePath))
                                File.Delete(saveFilePath);
                        }
                        catch { }
                    }
                }

                WaitingManager.Hide();

                var sb = new StringBuilder();
                sb.AppendLine(string.Format("Đã xuất {0}/{1} file XML{2}.", okCount, hoSoList.Count,
                    chkSign.Checked ? " (đã ký số)" : " (KHÔNG ký số)"));
                if (unsignedLines.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine(string.Format("{0} hồ sơ KÝ SỐ THẤT BẠI - file đã được đổi tên thành *_CHUAKY.xml, KHÔNG gửi cổng, hãy ký lại:", unsignedLines.Count));
                    sb.AppendLine(string.Join(Environment.NewLine, unsignedLines));
                }
                if (errLines.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("Hồ sơ lỗi:");
                    sb.AppendLine(string.Join(Environment.NewLine, errLines));
                }
                XtraMessageBox.Show(sb.ToString(), "Kết quả xuất XML", MessageBoxButtons.OK,
                    (errLines.Count > 0 || unsignedLines.Count > 0) ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi xảy ra khi xuất XML!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Dựng danh sách hồ sơ điều chỉnh - mỗi hồ sơ là 1 XmlHoSoDieuChinhGD chứa đúng 1 TT_HOSO
        /// (đúng tài liệu: mỗi lần gửi 01 hồ sơ, chữ ký ký 1 TT_HOSO).
        ///
        /// Phạm vi 1 hồ sơ = 1 XML1 gốc: gom theo MA_LK, trong mỗi MA_LK còn tách tiếp theo XML1_ID.
        /// Tài liệu mục 5 quy định TT_HOSO chỉ có 1 TT_XML1 và 1 TT_DIEUCHINH, mà TT_DIEUCHINH chỉ chứa
        /// 1 DS_XML1_DIEUCHINH + 1 DSCP_DIEUCHINH - nhiều dòng điều chỉnh thì thêm TT_XML1_DC/CHIPHI bên trong,
        /// KHÔNG lặp lại thẻ DS_XML1_DIEUCHINH. Vì vậy 2 XML1_ID khác nhau bắt buộc là 2 hồ sơ riêng.
        /// </summary>
        private List<XmlHoSoDieuChinhGD> BuildHoSoList()
        {
            var result = new List<XmlHoSoDieuChinhGD>();

            // Lấy thông tin chi nhánh
            string maCSKCB = "";
            string thuTruongDV = "";
            var branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
            if (branch != null)
            {
                maCSKCB = branch.HEIN_MEDI_ORG_CODE ?? "";
                thuTruongDV = branch.DIRECTOR_USERNAME ?? "";
            }

            // Tên tài khoản đăng nhập
            string nguoiLapBieu = "";
            try
            {
                nguoiLapBieu = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName() ?? "";
            }
            catch { }

            // Gom theo LINK_CODE (MA_LK), rồi tách tiếp theo XML1_ID → mỗi nhóm = 1 hồ sơ = 1 file XML riêng
            var hoSoGroups = new List<List<XmlAdjustADO>>();
            foreach (var linkGroup in _XmlAdjustAdos.GroupBy(o => o.LINK_CODE ?? ""))
            {
                hoSoGroups.AddRange(SplitByXml1Id(linkGroup));
            }

            foreach (var group in hoSoGroups)
            {
                var first = group.FirstOrDefault();

                var ttHoSo = new XmlTTHoSo
                {
                    Id = "Id-" + Guid.NewGuid().ToString("N"),
                    TT_MAU = new XmlTTMau
                    {
                        MAU_SO = "09/BH",
                        MA_CSKCB = maCSKCB,
                        NGUOILAPBIEU = nguoiLapBieu,
                        THUTRUONG_DV = thuTruongDV,
                        NGAYTHANGNAM = DateTime.Now.ToString("yyyyMMdd")
                    },
                    TT_XML1 = new XmlTTXml1
                    {
                        XML1_ID = first != null ? first.XML1_ID ?? "" : "",
                        MA_LK = first != null ? first.LINK_CODE ?? "" : "",
                        MA_BN = first != null ? first.PATIENT_CODE ?? "" : "",
                        HO_TEN = first != null ? first.PATIENT_NAME ?? "" : "",
                        MA_THE = first != null ? first.HEIN_CARD_NUMBER ?? "" : "",
                        NGAY_VAO = first != null && first.IN_DATE.HasValue ? first.IN_DATE.Value.ToString("yyyyMMddHHmm") : "",
                        NGAY_RA = first != null && first.OUT_DATE.HasValue ? first.OUT_DATE.Value.ToString("yyyyMMddHHmm") : "",
                        KY_QT = first != null && first.OUT_DATE.HasValue ? first.OUT_DATE.Value.ToString("yyyyMM") : DateTime.Now.ToString("yyyyMM"),
                        TRANGTHAI = first != null ? first.STATUS ?? "1" : "1"
                    },
                    TT_DIEUCHINH = new XmlTTDieuChinh()
                };

                var items = group;

                // DS_XML1_DIEUCHINH: dòng thuộc bảng XML1 (số bảng XML = 1 hoặc bỏ trống)
                var xml1DcList = items.Where(o => IsXml1AdjustRow(o)).ToList();
                var dsXml1 = new XmlDsXml1DieuChinh { Items = new List<XmlTTXml1DC>() };
                int stt = 1;
                foreach (var item in xml1DcList)
                {
                    dsXml1.Items.Add(new XmlTTXml1DC
                    {
                        STT = stt.ToString(),
                        TRUONG_TT_GOC = item.ORIGINAL_FIELD ?? "",
                        TT_GOC = item.ORIGINAL_VALUE ?? "",
                        TRUONG_TT_DIEUCHINH = item.ADJUST_FIELD ?? "",
                        TT_DIEUCHINH = item.ADJUST_VALUE ?? "",
                        LYDO_DIEUCHINH = item.ADJUST_REASON ?? ""
                    });
                    stt++;
                }
                // Không có dòng nào thì bỏ hẳn thẻ, tránh gửi lên thẻ rỗng <DS_XML1_DIEUCHINH />
                ttHoSo.TT_DIEUCHINH.DS_XML1_DIEUCHINH = dsXml1.Items.Count > 0 ? dsXml1 : null;

                // DSCP_DIEUCHINH: dòng thuộc bảng chi phí (số bảng XML khác 1 - tài liệu mục 9 nhận 2 hoặc 3)
                var cpDcList = items.Where(o => !IsXml1AdjustRow(o)).ToList();
                var dsCp = new XmlDsCpDieuChinh { Items = new List<XmlChiPhi>() };
                stt = 1;
                foreach (var item in cpDcList)
                {
                    dsCp.Items.Add(new XmlChiPhi
                    {
                        STT = stt.ToString(),
                        SOBANG_XML = item.XML_TABLE_NUMBER ?? "",
                        ID_CP = item.EXPENSE_ID ?? "",
                        STT_XML = item.XML_ORDER ?? "",
                        NGAY_YL = item.ORDER_DATE.HasValue ? item.ORDER_DATE.Value.ToString("yyyyMMddHHmm") : "",
                        TRANGTHAI = item.STATUS ?? "",
                        TRUONG_TT_GOC = item.ORIGINAL_FIELD ?? "",
                        TT_GOC = item.ORIGINAL_VALUE ?? "",
                        LYDO = item.ORIGINAL_REASON ?? "",
                        TUCHOI = item.REJECT_REASON ?? "",
                        TRUONG_TT_DIEUCHINH = item.ADJUST_FIELD ?? "",
                        TT_DIEUCHINH = item.ADJUST_VALUE ?? "",
                        LYDO_DIEUCHINH = item.ADJUST_REASON ?? ""
                    });
                    stt++;
                }
                ttHoSo.TT_DIEUCHINH.DSCP_DIEUCHINH = dsCp.Items.Count > 0 ? dsCp : null;

                var xmlData = new XmlHoSoDieuChinhGD
                {
                    TT_HOSO = new List<XmlTTHoSo> { ttHoSo },
                    ChuKyDonVi = ""
                };
                result.Add(xmlData);
            }

            return result;
        }

        /// <summary>
        /// Dòng Excel thuộc phần điều chỉnh thông tin hành chính (DS_XML1_DIEUCHINH/TT_XML1_DC) hay phần
        /// điều chỉnh - huỷ chi phí (DSCP_DIEUCHINH/CHIPHI). Căn cứ cột "Số bảng XML" theo tài liệu mục 8, 9:
        ///   1  -> bảng XML1, là thông tin hành chính của lượt KCB
        ///   2, 3 -> bảng chi phí (mục 9 chỉ nhận 2 hoặc 3)
        /// Bỏ trống thì coi là bảng XML1: thông tin hành chính không thuộc bảng chi phí nào, mà tạo thẻ CHIPHI
        /// thiếu SOBANG_XML thì cổng chắc chắn trả lỗi 124.
        /// </summary>
        private static bool IsXml1AdjustRow(XmlAdjustADO ado)
        {
            if (ado == null) return true;
            string soBangXml = (ado.XML_TABLE_NUMBER ?? "").Trim();
            return soBangXml.Length == 0 || soBangXml == "1";
        }

        /// <summary>
        /// Tách các dòng Excel của 1 MA_LK thành từng hồ sơ theo XML1_ID (1 hồ sơ = 1 XML1 gốc).
        /// - Cả nhóm chỉ có tối đa 1 XML1_ID: giữ nguyên 1 hồ sơ (các dòng chi phí bỏ trống XML1_ID vẫn đi cùng).
        /// - Có từ 2 XML1_ID trở lên: mỗi XML1_ID 1 hồ sơ; dòng bỏ trống XML1_ID không xác định được thuộc hồ sơ
        ///   gốc nào nên tách riêng và ghi log để người dùng bổ sung XML1_ID trong file Excel.
        /// </summary>
        private static List<List<XmlAdjustADO>> SplitByXml1Id(IEnumerable<XmlAdjustADO> items)
        {
            var result = new List<List<XmlAdjustADO>>();
            var list = items.ToList();
            if (list.Count == 0) return result;

            var xml1Ids = list.Where(o => !string.IsNullOrEmpty(o.XML1_ID))
                              .Select(o => o.XML1_ID.Trim())
                              .Distinct()
                              .ToList();

            if (xml1Ids.Count <= 1)
            {
                result.Add(list);
                return result;
            }

            Inventec.Common.Logging.LogSystem.Warn(string.Format(
                "[DAY_CONG_09BH] MA_LK {0} có {1} XML1_ID khác nhau ({2}) -> tách thành {1} hồ sơ riêng.",
                list[0].LINK_CODE ?? "", xml1Ids.Count, string.Join(", ", xml1Ids.ToArray())));

            foreach (var xml1Id in xml1Ids)
            {
                string id = xml1Id;
                result.Add(list.Where(o => !string.IsNullOrEmpty(o.XML1_ID) && o.XML1_ID.Trim() == id).ToList());
            }

            var noXml1Id = list.Where(o => string.IsNullOrEmpty(o.XML1_ID)).ToList();
            if (noXml1Id.Count > 0)
            {
                Inventec.Common.Logging.LogSystem.Warn(string.Format(
                    "[DAY_CONG_09BH] MA_LK {0}: {1} dòng bỏ trống XML1_ID -> tách thành hồ sơ riêng, hãy bổ sung XML1_ID nếu thuộc hồ sơ gốc cụ thể.",
                    list[0].LINK_CODE ?? "", noXml1Id.Count));
                result.Add(noXml1Id);
            }

            return result;
        }

        private static string GetMaLk(XmlHoSoDieuChinhGD x)
        {
            if (x != null && x.TT_HOSO != null && x.TT_HOSO.Count > 0 && x.TT_HOSO[0].TT_XML1 != null)
                return x.TT_HOSO[0].TT_XML1.MA_LK ?? "";
            return "";
        }

        private static string GetKyQt(XmlHoSoDieuChinhGD x)
        {
            if (x != null && x.TT_HOSO != null && x.TT_HOSO.Count > 0 && x.TT_HOSO[0].TT_XML1 != null)
                return x.TT_HOSO[0].TT_XML1.KY_QT ?? "";
            return "";
        }

        private static string GetMaCsKCB(XmlHoSoDieuChinhGD x)
        {
            if (x != null && x.TT_HOSO != null && x.TT_HOSO.Count > 0 && x.TT_HOSO[0].TT_MAU != null)
                return x.TT_HOSO[0].TT_MAU.MA_CSKCB ?? "";
            return "";
        }

        private static string SafeFileName(string s)
        {
            if (string.IsNullOrEmpty(s)) return "NA";
            foreach (var c in Path.GetInvalidFileNameChars())
                s = s.Replace(c, '_');
            return s;
        }

        /// <summary>
        /// Đổi tên file XML chưa ký thành *_CHUAKY.xml. Giữ lại nội dung để tra lỗi nhưng tên khác hẳn file đã ký
        /// nên không thể vô tình gửi lên cổng. Trả về đường dẫn mới, null nếu không đổi được.
        /// </summary>
        private static string RenameToUnsigned(string saveFilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(saveFilePath) || !File.Exists(saveFilePath)) return null;
                string dir = Path.GetDirectoryName(saveFilePath);
                string target = Path.Combine(dir, Path.GetFileNameWithoutExtension(saveFilePath) + "_CHUAKY.xml");
                if (File.Exists(target)) File.Delete(target);
                File.Move(saveFilePath, target);
                return target;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private static MemoryStream CreateXmlFile<T>(T input)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var settings = new XmlWriterSettings
                {
                    // Indent = false: toàn bộ XML nằm trên MỘT dòng, không thụt lề, không xuống dòng
                    // giữa các thẻ. Ngoài việc đúng dạng file mà cổng giám định tiếp nhận, cách này còn
                    // an toàn hơn cho chữ ký: không sinh ra node whitespace nào giữa các thẻ nên digest
                    // của TT_HOSO không phụ thuộc vào cách trình bày. Ký số đọc file với
                    // PreserveWhitespace = true nên GIỮ NGUYÊN đúng byte đã ghi ở đây.
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = new UTF8Encoding(false)
                };

                var ms = new MemoryStream();
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    // Thẻ root phải TRƠN, không khai báo xsd/xsi: file mẫu mà cổng đã tiếp nhận
                    // (hsdc09_24664226_11_SIGNED.xml) là <HOSO_DIEUCHINH_GD> không kèm namespace nào.
                    // Namespace lạ ở node cha là nguyên nhân đã biết gây lỗi chữ ký (mã 125).
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    xmlSerializer.Serialize(writer, input, ns);
                }

                var resultMs = new MemoryStream(ms.ToArray());
                return resultMs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        #endregion

        #region Đẩy cổng

        /// <summary>
        /// Đẩy hồ sơ điều chỉnh mẫu 09/BH lên cổng giám định BHXH theo tài liệu MoTaAPI_GuiHoSoDieuChinh09BH.
        /// Trình tự đúng tài liệu:
        ///   B1. Kiểm tra dữ liệu Excel + điều kiện ký số (cổng chỉ nhận XML có CHUKYDONVI ký vào TT_HOSO)
        ///   B2. Đọc cấu hình kết nối cổng (address|username|password|maTinh)
        ///   B3. Dựng hồ sơ (mỗi MA_LK = 01 hồ sơ) và kiểm tra nội dung theo mục 5-9 để loại trước các lỗi 123/124/202/204
        ///   B4. Lấy token phiên làm việc MỘT LẦN cho cả lô (mục I) - token dùng lại, hết hạn thì client tự lấy mới
        ///   B5. Ký số từng hồ sơ -> base64 -> POST api/HSDCTT12/GuiHoSoDieuChinh09BH (mục II)
        ///   B6. Tổng hợp mã giao dịch / mã lỗi trả về cho người dùng
        /// </summary>
        private void btnPushPortal_Click(object sender, EventArgs e)
        {
            try
            {
                // ===== B1. Dữ liệu đầu vào =====
                if (_XmlAdjustAdos == null || _XmlAdjustAdos.Count == 0)
                {
                    XtraMessageBox.Show("Chưa có dữ liệu. Hãy Import file Excel trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_XmlAdjustAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR)))
                {
                    XtraMessageBox.Show("Dữ liệu còn dòng lỗi. Hãy bấm \"Dòng lỗi\" để xem và sửa file Excel trước khi đẩy cổng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Đẩy cổng BẮT BUỘC có chữ ký đơn vị -> kiểm tra một lần trước khi chạy cả lô
                string signPrepareError;
                if (!CheckSignPrerequisite(true, out signPrepareError))
                {
                    XtraMessageBox.Show(signPrepareError, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ===== B2. Cấu hình kết nối cổng =====
                string configError;
                Portal09BHConnection connection = GetPortalConnection(out configError);
                if (connection == null)
                {
                    XtraMessageBox.Show(configError, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ===== B3. Dựng hồ sơ + kiểm tra nội dung theo tài liệu mục 5-9 =====
                var hoSoList = BuildHoSoList();
                if (hoSoList == null || hoSoList.Count == 0)
                {
                    XtraMessageBox.Show("Không dựng được hồ sơ điều chỉnh từ dữ liệu hiện có.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string maCsKCB = GetMaCsKCB(hoSoList[0]);
                if (string.IsNullOrEmpty(maCsKCB))
                {
                    XtraMessageBox.Show("Chi nhánh đang làm việc chưa khai báo mã cơ sở KCB (HEIN_MEDI_ORG_CODE). Cổng sẽ trả lỗi mã CSKCB không đúng.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var validHoSos = new List<XmlHoSoDieuChinhGD>();
                var invalidLines = new List<string>();
                foreach (var xml in hoSoList)
                {
                    string validateError = XML.HoSo09BHValidator.Validate(xml, maCsKCB);
                    if (string.IsNullOrEmpty(validateError))
                        validHoSos.Add(xml);
                    else
                        invalidLines.Add(string.Format("MA_LK {0}: {1}", GetMaLk(xml), validateError));
                }

                if (invalidLines.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(string.Format(
                        "[DAY_CONG_09BH] {0}/{1} hồ sơ không hợp lệ theo cấu trúc 09/BH:{2}{3}",
                        invalidLines.Count, hoSoList.Count, Environment.NewLine, string.Join(Environment.NewLine, invalidLines)));

                    if (validHoSos.Count == 0)
                    {
                        XtraMessageBox.Show(
                            "Không có hồ sơ nào hợp lệ để đẩy cổng. Hãy sửa dữ liệu rồi thử lại:" + Environment.NewLine + Environment.NewLine
                            + BuildLimitedLines(invalidLines),
                            "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (XtraMessageBox.Show(string.Format(
                            "Có {0}/{1} hồ sơ không hợp lệ, sẽ KHÔNG được gửi:{2}{3}{4}{4}Bạn có muốn tiếp tục đẩy {5} hồ sơ hợp lệ không?",
                            invalidLines.Count, hoSoList.Count, Environment.NewLine, BuildLimitedLines(invalidLines),
                            Environment.NewLine, validHoSos.Count),
                            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        return;
                }
                else
                {
                    if (XtraMessageBox.Show(
                        string.Format("Bạn có chắc chắn muốn đẩy {0} hồ sơ điều chỉnh mẫu 09/BH lên cổng giám định BHXH?", validHoSos.Count),
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;
                }

                WaitingManager.Show();

                // ===== B4. Lấy token phiên làm việc (tài liệu mục I) - dùng chung cho cả lô =====
                var api = new Portal.Portal09BHApi();
                var token = api.EnsureToken(connection.Address, connection.Username, connection.Password, maCsKCB);
                if (token == null || !token.HasToken())
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show(
                        "Không lấy được token phiên làm việc từ cổng BHXH." + Environment.NewLine
                        + (token != null && !string.IsNullOrEmpty(token.ErrorMessage) ? token.ErrorMessage : ""),
                        "Lỗi đăng nhập cổng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ===== B5. Ký số + gửi từng hồ sơ (tài liệu mục II) =====
                string maTinh = !string.IsNullOrEmpty(connection.MaTinh) ? connection.MaTinh : GetMaTinhFromMaCsKCB(maCsKCB);
                int okCount = 0;
                var errLines = new List<string>();
                var okLines = new List<string>();

                foreach (var xml in validHoSos)
                {
                    string maLk = GetMaLk(xml);
                    string kyQt = GetKyQt(xml);

                    string err;
                    string fileBase64 = SignXmlToBase64(xml, out err);
                    if (string.IsNullOrEmpty(fileBase64))
                    {
                        errLines.Add(string.Format("MA_LK {0}: {1}", maLk, err ?? "không tạo được XML"));
                        continue;
                    }

                    var rs = api.Send(connection.Address, connection.Username, connection.Password, maCsKCB, kyQt, maTinh, fileBase64);
                    if (rs.Success)
                    {
                        okCount++;
                        okLines.Add(string.Format("MA_LK {0}: {1}{2}", maLk, rs.MaGiaoDich,
                            !string.IsNullOrEmpty(rs.ThoiGianTiepNhan)
                                ? " - tiếp nhận " + FormatThoiGianTiepNhan(rs.ThoiGianTiepNhan)
                                : ""));
                    }
                    else
                    {
                        errLines.Add(string.Format("MA_LK {0}: [{1}] {2}", maLk,
                            !string.IsNullOrEmpty(rs.MaKetQua) ? rs.MaKetQua : "-",
                            !string.IsNullOrEmpty(rs.ErrorMessage) ? rs.ErrorMessage : rs.ThongDiep));
                    }
                }

                WaitingManager.Hide();

                // ===== B6. Tổng hợp kết quả =====
                var sb = new StringBuilder();
                sb.AppendLine(string.Format("Đẩy cổng xong: {0}/{1} hồ sơ thành công.", okCount, validHoSos.Count));
                if (okLines.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("Thành công (mã giao dịch):");
                    sb.AppendLine(BuildLimitedLines(okLines));
                }
                if (errLines.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("Hồ sơ lỗi:");
                    sb.AppendLine(BuildLimitedLines(errLines));
                }
                if (invalidLines.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine(string.Format("{0} hồ sơ không hợp lệ đã bị bỏ qua (chưa gửi cổng):", invalidLines.Count));
                    sb.AppendLine(BuildLimitedLines(invalidLines));
                }
                XtraMessageBox.Show(sb.ToString(), "Kết quả đẩy cổng", MessageBoxButtons.OK,
                    (errLines.Count > 0 || invalidLines.Count > 0) ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi xảy ra khi đẩy cổng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Thông tin kết nối cổng đọc từ config: address|username|password|maTinh (maTinh tùy chọn).</summary>
        private class Portal09BHConnection
        {
            public string Address { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string MaTinh { get; set; }
        }

        /// <summary>
        /// Đọc cấu hình kết nối cổng 09/BH.
        /// - Ưu tiên key riêng HIS.HSDC_09BH.CONNECTION_INFO, cấu trúc: address|username|password|maTinh
        ///   (phần tử thứ 4 là mã tỉnh, tùy chọn - bỏ trống thì suy từ 2 ký tự đầu mã CSKCB).
        /// - Trống thì dùng lại HIS.QD_130_BYT.CONNECTION_INFO nhưng CHỈ lấy 3 phần đầu: từ phần tử thứ 4 trở đi
        ///   key này mang ý nghĩa khác (typeXml, xml130Api, xmlGdykApi...) của chức năng xuất XML 130.
        /// Trả về null kèm error khi thiếu thông tin bắt buộc.
        /// </summary>
        private Portal09BHConnection GetPortalConnection(out string error)
        {
            error = null;
            try
            {
                bool isConfig09BH = true;
                string connectionInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY_09BH_CONNECTION_INFO);
                if (string.IsNullOrEmpty(connectionInfo))
                {
                    isConfig09BH = false;
                    connectionInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY_CONNECTION_INFO);
                }

                var connection = new Portal09BHConnection();
                if (!string.IsNullOrEmpty(connectionInfo))
                {
                    var parts = connectionInfo.Split('|');
                    if (parts.Length > 0) connection.Address = parts[0].Trim();
                    if (parts.Length > 1) connection.Username = parts[1].Trim();
                    if (parts.Length > 2) connection.Password = parts[2];
                    if (isConfig09BH && parts.Length > 3) connection.MaTinh = parts[3].Trim();
                }

                if (string.IsNullOrEmpty(connection.Address) || string.IsNullOrEmpty(connection.Username) || string.IsNullOrEmpty(connection.Password))
                {
                    error = "Chưa cấu hình thông tin kết nối cổng BHXH (key " + CONFIG_KEY_09BH_CONNECTION_INFO
                        + " hoặc " + CONFIG_KEY_CONNECTION_INFO + "). Cấu trúc: địa chỉ|tài khoản|mật khẩu|mã tỉnh.";
                    return null;
                }
                return connection;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                error = "Không đọc được cấu hình kết nối cổng BHXH: " + ex.Message;
                return null;
            }
        }

        /// <summary>Mã tỉnh = 2 ký tự đầu mã CSKCB (chuẩn BHYT: 2 số tỉnh + số cơ sở).</summary>
        private static string GetMaTinhFromMaCsKCB(string maCsKCB)
        {
            return (!string.IsNullOrEmpty(maCsKCB) && maCsKCB.Length >= 2) ? maCsKCB.Substring(0, 2) : "";
        }

        /// <summary>
        /// Đổi thoiGianTiepNhan cổng trả về (14 ký tự yyyyMMddHHmmss - tài liệu mục II.3) sang dd/MM/yyyy HH:mm:ss.
        /// Sai định dạng thì trả nguyên chuỗi gốc.
        /// </summary>
        private static string FormatThoiGianTiepNhan(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            DateTime parsed;
            if (DateTime.TryParseExact(value.Trim(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                return parsed.ToString("dd/MM/yyyy HH:mm:ss");
            return value;
        }

        /// <summary>Ghép danh sách dòng thông báo, cắt bớt để hộp thoại không quá dài (chi tiết đầy đủ đã có trong log).</summary>
        private static string BuildLimitedLines(List<string> lines)
        {
            const int MAX_LINE = 20;
            if (lines == null || lines.Count == 0) return "";
            if (lines.Count <= MAX_LINE)
                return string.Join(Environment.NewLine, lines);
            var shown = lines.GetRange(0, MAX_LINE);
            return string.Join(Environment.NewLine, shown) + Environment.NewLine
                + string.Format("... và {0} hồ sơ khác (xem chi tiết trong log).", lines.Count - MAX_LINE);
        }

        /// <summary>
        /// Ghi 1 hồ sơ ra file tạm → (ký số nếu chọn) → trả về chuỗi base64 nội dung XML.
        /// </summary>
        private string SignXmlToBase64(XmlHoSoDieuChinhGD xml, out string error)
        {
            error = null;
            string tempFile = null;
            try
            {
                var ms = CreateXmlFile(xml);
                if (ms == null) { error = "Không tạo được XML"; return null; }

                string fileName = string.Format("HOSO_DIEUCHINH_GD_{0}.xml", Guid.NewGuid().ToString("N"));
                string tempFolder = Path.Combine(Path.GetTempPath(), "HisImportXmlAdjust");
                Directory.CreateDirectory(tempFolder);
                tempFile = Path.Combine(tempFolder, fileName);
                using (FileStream file = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                {
                    ms.WriteTo(file);
                }
                ms.Close();

                // Cổng yêu cầu XML có chữ ký đơn vị -> ký thất bại thì KHÔNG được gửi hồ sơ này
                if (chkSign.Checked)
                {
                    string signError;
                    if (!SignFile(fileName, tempFile, out signError))
                    {
                        error = "ký số thất bại - " + signError;
                        return null;
                    }
                }
                else
                {
                    error = "chưa bật ký số, hồ sơ không có chữ ký đơn vị";
                    return null;
                }

                byte[] bytes = File.ReadAllBytes(tempFile);
                string xmlContent = RemoveByteOrderMark(Encoding.UTF8.GetString(bytes));
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "[DAY_CONG_09BH] XML INPUT (đã ký=true, MA_LK={0}, độ dài={1}):{2}{3}",
                    GetMaLk(xml), xmlContent.Length, Environment.NewLine, xmlContent));
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlContent));
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tempFile) && File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                catch { }
            }
        }

        #endregion

        #region Ký số

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                    return;

                isChkSignFileCertUtil();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void isChkSignFileCertUtil()
        {
            try
            {
                if (chkSign.Checked == true)
                {
                    frmSetting frm = new frmSetting(SettingSignADO, (result) =>
                    {
                        SettingSignADO = (SettingSignADO)result;
                    });
                    frm.ShowDialog();
                    if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                        chkSign.Checked = false;
                }
                else
                {
                    SettingSignADO = null;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSign.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSign.Name;
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                    csAddOrUpdate.MODULE_LINK = this.currentModule.ModuleLink;
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

        /// <summary>
        /// Kiểm tra điều kiện ký số MỘT LẦN trước khi chạy cả lô, để không bung hộp thoại trong vòng lặp hồ sơ.
        /// signRequired = true khi nghiệp vụ bắt buộc phải có chữ ký (đẩy cổng).
        /// </summary>
        private bool CheckSignPrerequisite(bool signRequired, out string error)
        {
            error = null;
            signServiceVerified = false;
            if (!chkSign.Checked)
            {
                if (!signRequired) return true;
                error = "Cổng giám định BHXH chỉ tiếp nhận XML có chữ ký đơn vị. Hãy tích \"Ký số\" và chọn chứng thư trước khi đẩy cổng.";
                return false;
            }
            if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
            {
                error = "Đã tích \"Ký số\" nhưng chưa chọn chứng thư/Usb Token. Hãy chọn lại chứng thư hoặc bỏ tích \"Ký số\".";
                return false;
            }
            if (!SettingSignADO.IsHsm && PREFER_SERVICE_SIGNER_09BH)
            {
                // Service ký số là đường ưu tiên chứ không bắt buộc: không khởi động được thì ký tại máy trạm bằng
                // chứng thư trong kho chứng thư của Windows, nên chỉ ghi log cảnh báo, không chặn cả lô.
                signServiceVerified = VerifyServiceSignProcessorIsRunning();
                if (!signServiceVerified)
                    Inventec.Common.Logging.LogSystem.Warn(
                        "CheckSignPrerequisite: không khởi động được service ký số EMR.SignProcessor (Integrate\\EMR.SignProcessor) -> sẽ ký tại máy trạm.");
            }
            return true;
        }

        /// <summary>
        /// Ký số file XML tại saveFilePath (ghi đè chính file đó khi ký thành công).
        /// CHỈ trả về true khi file kết quả thực sự có thẻ Signature bên trong CHUKYDONVI.
        /// </summary>
        public bool SignFile(string fullFileName, string saveFilePath, out string signError)
        {
            signError = null;
            string tempFilePath = null;
            try
            {
                if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                {
                    signError = "Chưa có thông tin chứng thư/Usb Token ký số.";
                    Inventec.Common.Logging.LogSystem.Warn("SignFile: " + signError);
                    return false;
                }

                string pathAfterFileSign = null;

                if (SettingSignADO.IsHsm)
                {
                    string tempFolderPath = Path.Combine(Path.GetTempPath(), TEMP_FOLDER_NAME);
                    Directory.CreateDirectory(tempFolderPath);
                    tempFilePath = Path.Combine(tempFolderPath, Guid.NewGuid().ToString("N") + "_" + SafeFileName(fullFileName));

                    string sourceBase64 = ReadFileContent(saveFilePath);
                    if (string.IsNullOrEmpty(sourceBase64))
                    {
                        signError = "File XML nguồn không đọc được hoặc không hợp lệ: " + saveFilePath;
                        Inventec.Common.Logging.LogSystem.Warn("SignFile: " + signError);
                        return false;
                    }
                    string apiMessage;
                    var xmlBase64 = SourceFileSignApi(sourceBase64, out apiMessage);
                    if (string.IsNullOrEmpty(xmlBase64))
                    {
                        signError = "Ký HSM thất bại (api/EmrSign/SignXml09Bh"
                            + ", HSM=" + SettingSignADO.Id + ", serial=" + SettingSignADO.SerialNumber + ")"
                            + (!string.IsNullOrEmpty(apiMessage) ? ": " + apiMessage : " - không trả về dữ liệu.");
                        Inventec.Common.Logging.LogSystem.Warn("SignFile: " + signError);
                        return false;
                    }
                    File.WriteAllBytes(tempFilePath, Convert.FromBase64String(xmlBase64));
                    pathAfterFileSign = tempFilePath;
                }
                else
                {
                    // Ký bằng chứng thư/USB Token. Ưu tiên service ký số EMR.SignProcessor vì hàm ký nằm ở thư viện
                    // dùng chung (Inventec.Common.SignFile.SignData.SignXml09BH) - dùng lại được cho các chức năng
                    // khác. Service không dùng được hoặc trả về chữ ký sai profile thì lùi về ký tại máy trạm.
                    if (PREFER_SERVICE_SIGNER_09BH)
                    {
                        string serviceError;
                        if (SignFileByService09BH(fullFileName, saveFilePath, out serviceError))
                            return true;
                        Inventec.Common.Logging.LogSystem.Warn(
                            "SignFile: không ký được mẫu 09/BH qua service ký số (" + serviceError + ") -> ký tại máy trạm.");
                    }

                    if (!Sign.Xml09BHSigner.SignFile(saveFilePath, SettingSignADO.SerialNumber, out signError))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("SignFile: " + signError);
                        return false;
                    }
                    // Tự kiểm lại chữ ký vừa tạo trước khi cho đi tiếp
                    if (!IsSignedFileAcceptable(saveFilePath, true, out signError))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("SignFile: " + signError);
                        return false;
                    }
                    return true;
                }

                // Đến đây chỉ còn đường HSM. Chỉ ghi đè file thật khi kết quả ký thực sự có chữ ký -> không bao giờ
                // để file chưa ký đi tiếp. Kiểm luôn phần mật mã cho CẢ HAI đường (USB và HSM): backend đã ký HSM
                // ra đúng profile 09/BH qua api/EmrSign/SignXml09Bh và chữ ký verify được (đối chiếu giống từng byte
                // với đường USB), nên chữ ký HSM không kiểm chứng được giờ là LỖI THẬT, phải chặn chứ không chỉ cảnh báo.
                if (!IsSignedFileAcceptable(pathAfterFileSign, true, out signError))
                {
                    Inventec.Common.Logging.LogSystem.Warn("SignFile: " + signError);
                    return false;
                }

                File.Copy(pathAfterFileSign, saveFilePath, true);
                return true;
            }
            catch (Exception ex)
            {
                signError = ex.Message;
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                        File.Delete(tempFilePath);
                }
                catch { }
            }
        }

        /// <summary>
        /// Ký hồ sơ mẫu 09/BH qua service ký số EMR.SignProcessor.
        /// Gọi operation riêng cho mẫu 09/BH là SignXml09BH; service bản cũ chưa có operation này thì lùi về
        /// SignXml130 nhưng vẫn truyền fieldSigned = "CHUKYDONVI|TT_HOSO" - Inventec.Common.SignFile.SignData nhận
        /// dấu '|' và tự chuyển sang SignXml09BH, nên vẫn ra đúng profile mà không phải đổi contract của service.
        /// Kết quả CHỈ được chấp nhận khi chữ ký khớp nội dung VÀ đúng profile mẫu 09/BH; sai thì trả false để
        /// caller lùi về ký tại máy trạm, tuyệt đối không ghi đè file thật bằng chữ ký sai profile (cổng trả lỗi 125).
        /// </summary>
        private bool SignFileByService09BH(string fullFileName, string saveFilePath, out string error)
        {
            error = null;
            string tempFilePath = null;
            try
            {
                // Đã kiểm ở CheckSignPrerequisite thì không quét lại process cho từng hồ sơ
                if (!signServiceVerified)
                {
                    if (!VerifyServiceSignProcessorIsRunning())
                    {
                        error = "service ký số EMR.SignProcessor không chạy";
                        return false;
                    }
                    signServiceVerified = true;
                }

                string tempFolderPath = Path.Combine(Path.GetTempPath(), TEMP_FOLDER_NAME);
                Directory.CreateDirectory(tempFolderPath);
                tempFilePath = Path.Combine(tempFolderPath, Guid.NewGuid().ToString("N") + "_" + SafeFileName(fullFileName));

                WcfSignDCO wcfSignDCO = new WcfSignDCO
                {
                    SerialNumber = SettingSignADO.SerialNumber,
                    OutputFile = tempFilePath,
                    PIN = "",
                    SourceFile = saveFilePath,
                    fieldSigned = FIELD_SIGNED_09BH
                };
                string jsonData = JsonConvert.SerializeObject(wcfSignDCO);

                string pathAfterFileSign = null;
                if (signService09BHSupported != false)
                {
                    bool notSupported;
                    pathAfterFileSign = CallSignService(jsonData, true, out error, out notSupported);
                    if (notSupported)
                    {
                        signService09BHSupported = false;
                        Inventec.Common.Logging.LogSystem.Warn(string.Format(
                            "SignFileByService09BH: service ký số chưa có operation SignXml09BH -> dùng SignXml130 với fieldSigned = {0}.",
                            FIELD_SIGNED_09BH));
                    }
                    else if (!string.IsNullOrEmpty(pathAfterFileSign))
                    {
                        signService09BHSupported = true;
                    }
                }
                if (string.IsNullOrEmpty(pathAfterFileSign))
                {
                    bool notSupported;
                    pathAfterFileSign = CallSignService(jsonData, false, out error, out notSupported);
                }
                if (string.IsNullOrEmpty(pathAfterFileSign))
                    return false;

                // Thư viện ký của service có thể còn bản cũ (chỉ biết profile XML130) nhưng vẫn trả Success = true,
                // nên bắt buộc soi lại hình dạng chữ ký trước khi cho ghi đè file thật.
                if (!IsSignedFileAcceptable(pathAfterFileSign, true, out error))
                    return false;

                File.Copy(pathAfterFileSign, saveFilePath, true);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                        File.Delete(tempFilePath);
                }
                catch { }
            }
        }

        /// <summary>
        /// Gọi service ký số. useOperation09BH = true dùng operation riêng SignXml09BH, false dùng SignXml130.
        /// Trả về đường dẫn file kết quả, null nếu không ký được.
        /// notSupported = true khi service không khai báo operation vừa gọi (bản service cũ).
        /// </summary>
        private string CallSignService(string jsonData, bool useOperation09BH, out string error, out bool notSupported)
        {
            error = null;
            notSupported = false;
            string operationName = useOperation09BH ? "SignXml09BH" : "SignXml130";
            SignProcessorClient signProcessorClient = new SignProcessorClient();
            try
            {
                var wcfSignResultDCO = useOperation09BH
                    ? signProcessorClient.SignXml09BH(jsonData)
                    : signProcessorClient.SignXml130(jsonData);
                if (wcfSignResultDCO == null || !wcfSignResultDCO.Success)
                {
                    error = string.Format("ký file thất bại qua {0}: {1}", operationName,
                        wcfSignResultDCO != null && !string.IsNullOrEmpty(wcfSignResultDCO.Message)
                            ? wcfSignResultDCO.Message : "service ký số không trả về kết quả");
                    return null;
                }
                return wcfSignResultDCO.OutputFile;
            }
            catch (System.ServiceModel.ActionNotSupportedException ex)
            {
                // Service chưa được cập nhật operation này. Kênh WCF đã fault -> caller phải tạo client mới.
                notSupported = true;
                error = "service ký số không có operation " + operationName;
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            catch (Exception ex)
            {
                error = string.Format("lỗi khi gọi {0}: {1}", operationName, ex.Message);
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            finally
            {
                CloseSignClient(signProcessorClient);
            }
        }

        /// <summary>
        /// Nơi DUY NHẤT kiểm file XML sau khi ký, nạp file MỘT LẦN rồi chạy cả 3 phép kiểm:
        ///   1. file tồn tại, không rỗng, đọc được và có thẻ Signature nằm trong CHUKYDONVI
        ///   2. đúng profile mẫu 09/BH  - <see cref="GetProfile09BHError"/>
        ///   3. chữ ký khớp nội dung    - <see cref="GetCryptographyError"/>
        /// Phép 1 luôn là lỗi chặn (không bao giờ để file chưa ký đi tiếp).
        /// strict = true  (ta tự ký bằng chứng thư/USB Token) -> phép 2 và 3 cũng là lỗi chặn.
        /// strict = false (chữ ký HSM do backend tạo)         -> phép 2 và 3 chỉ ghi log cảnh báo, không chặn hồ sơ.
        /// </summary>
        private static bool IsSignedFileAcceptable(string filePath, bool strict, out string error)
        {
            error = null;
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    error = "Không tìm thấy file kết quả ký số.";
                    return false;
                }
                if (new FileInfo(filePath).Length == 0)
                {
                    error = "File kết quả ký số rỗng.";
                    return false;
                }

                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true; // bắt buộc để CheckSignature tính đúng digest
                doc.Load(filePath);

                XmlElement chuKy = FirstElementByTagName(doc, TAG_CHUKYDONVI);
                if (chuKy == null)
                {
                    error = "File kết quả ký số không có thẻ " + TAG_CHUKYDONVI + ".";
                    return false;
                }
                XmlNodeList signatures = chuKy.GetElementsByTagName("Signature", XMLDSIG_NAMESPACE);
                XmlElement signature = (signatures != null && signatures.Count > 0) ? signatures[0] as XmlElement : null;
                if (signature == null)
                {
                    error = "File kết quả không có chữ ký trong thẻ " + TAG_CHUKYDONVI + ".";
                    return false;
                }

                string profileError = GetProfile09BHError(doc, signature);
                string cryptoError = GetCryptographyError(doc, signature);
                if (strict)
                {
                    error = profileError ?? cryptoError;
                    return error == null;
                }

                if (!string.IsNullOrEmpty(profileError))
                    Inventec.Common.Logging.LogSystem.Warn("IsSignedFileAcceptable (HSM) - " + profileError);
                if (!string.IsNullOrEmpty(cryptoError))
                    Inventec.Common.Logging.LogSystem.Warn("IsSignedFileAcceptable (HSM) - chữ ký chưa kiểm chứng được: " + cryptoError);
                return true;
            }
            catch (Exception ex)
            {
                error = "File kết quả ký số không đọc được: " + ex.Message;
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Soi hình dạng chữ ký xem có ĐÚNG profile mẫu 09/BH mà cổng giám định BHXH tiếp nhận hay không:
        /// đúng 2 Reference (một trỏ vào Object chứa SigningTime, một trỏ vào thẻ TT_HOSO theo attribute Id) và
        /// KHÔNG có Transform enveloped-signature. Chữ ký đúng về mật mã nhưng sai profile (ví dụ ký bằng đường
        /// XML130) vẫn bị cổng trả lỗi chữ ký nên phải chặn ngay tại máy trạm.
        /// Trả về null khi đúng profile, ngược lại là mô tả lỗi.
        /// </summary>
        private static string GetProfile09BHError(XmlDocument doc, XmlElement signature)
        {
            try
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("ds", XMLDSIG_NAMESPACE);

                XmlNodeList references = signature.SelectNodes("ds:SignedInfo/ds:Reference", nsmgr);
                int referenceCount = references == null ? 0 : references.Count;
                if (referenceCount != 2)
                    return string.Format(
                        "Chữ ký không đúng profile mẫu 09/BH: cần 2 Reference, file đang có {0}.", referenceCount);

                if (signature.SelectSingleNode(
                        ".//ds:Transform[@Algorithm='" + XMLDSIG_NAMESPACE + "enveloped-signature']", nsmgr) != null)
                    return "Chữ ký không đúng profile mẫu 09/BH: còn Transform enveloped-signature (đây là profile XML130).";

                // SignatureProperties/SigningTime nằm ngoài namespace xmldsig -> dò theo local-name
                XmlNode signingTime = signature.SelectSingleNode(
                    "ds:Object/*[local-name()='SignatureProperties']/*[local-name()='SignatureProperty']/*[local-name()='SigningTime']",
                    nsmgr);
                if (signingTime == null || string.IsNullOrEmpty(signingTime.InnerText))
                    return "Chữ ký không đúng profile mẫu 09/BH: thiếu SigningTime trong thẻ Object của chữ ký.";

                XmlElement ttHoSo = FirstElementByTagName(doc, TAG_TT_HOSO);
                string tagId = ttHoSo != null ? ttHoSo.GetAttribute("Id") : null;
                if (string.IsNullOrEmpty(tagId))
                    return "Chữ ký không đúng profile mẫu 09/BH: thẻ " + TAG_TT_HOSO + " không có attribute Id để Reference trỏ tới.";

                bool hasObjectReference = false;
                bool hasTagReference = false;
                foreach (XmlNode reference in references)
                {
                    XmlElement referenceElement = reference as XmlElement;
                    string uri = referenceElement != null ? referenceElement.GetAttribute("URI") : "";
                    if (uri == "#" + tagId)
                        hasTagReference = true;
                    else if (uri.StartsWith("#Object-"))
                        hasObjectReference = true;
                }
                if (!hasObjectReference || !hasTagReference)
                    return string.Format(
                        "Chữ ký không đúng profile mẫu 09/BH: thiếu Reference trỏ vào {0}.",
                        !hasObjectReference && !hasTagReference ? "Object chứa SigningTime và thẻ " + TAG_TT_HOSO
                            : (!hasObjectReference ? "Object chứa SigningTime" : "thẻ " + TAG_TT_HOSO));
                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "Không kiểm tra được profile chữ ký mẫu 09/BH: " + ex.Message;
            }
        }

        /// <summary>
        /// Kiểm toàn vẹn chữ ký (digest + SignatureValue) bằng SignedXml.CheckSignature.
        /// Trả về null khi chữ ký khớp nội dung, ngược lại là mô tả lỗi.
        /// LƯU Ý: CheckSignature chỉ chứng minh chữ ký khớp nội dung file, KHÔNG chứng minh đúng profile chữ ký
        /// mà cổng BHXH yêu cầu cho mẫu 09/BH - phần đó do <see cref="GetProfile09BHError"/> kiểm.
        /// </summary>
        private static string GetCryptographyError(XmlDocument doc, XmlElement signature)
        {
            try
            {
                var signedXml = new System.Security.Cryptography.Xml.SignedXml(doc);
                signedXml.LoadXml(signature);
                if (!signedXml.CheckSignature())
                    return "Chữ ký không khớp nội dung file (digest hoặc SignatureValue sai).";
                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "Không kiểm tra được chữ ký: " + ex.Message;
            }
        }

        private static XmlElement FirstElementByTagName(XmlDocument doc, string name)
        {
            XmlNodeList nodes = doc.GetElementsByTagName(name);
            return (nodes != null && nodes.Count > 0) ? nodes[0] as XmlElement : null;
        }

        private static void CloseSignClient(SignProcessorClient client)
        {
            if (client == null) return;
            try
            {
                if (client.State == System.ServiceModel.CommunicationState.Faulted)
                    client.Abort();
                else
                    // binding không cấu hình closeTimeout -> mặc định 60s/hồ sơ; overload có timeout nằm ở ICommunicationObject
                    ((System.ServiceModel.ICommunicationObject)client).Close(TimeSpan.FromSeconds(3));
            }
            catch
            {
                try { client.Abort(); }
                catch { }
            }
        }

        /// <summary>Gọi API ký HSM. Thông điệp lỗi trả ra qua apiMessage để caller gộp lại, KHÔNG bung hộp thoại trong vòng lặp.</summary>
        private string SourceFileSignApi(string xmlBase64Source, out string apiMessage)
        {
            apiMessage = null;
            string result = null;
            try
            {
                CommonParam param = new CommonParam();
                // Dùng api/EmrSign/SignXml09Bh (KHÔNG dùng SignXmlBhyt nữa): SignXmlBhyt ký theo profile
                // SignXml130 = 1 Reference URI="" + Transform enveloped-signature, không có SigningTime,
                // nên cổng giám định BHXH không nhận cho mẫu 09/BH. Endpoint mới ký ra đúng profile
                // 2 Reference + SigningTime, đã đối chiếu giống từng byte với Sign\Xml09BHSigner.cs (đường USB).
                ADO.SignXml09BhADO signXml09BhADO = new ADO.SignXml09BhADO();
                signXml09BhADO.XmlBase64 = xmlBase64Source;
                signXml09BhADO.SignatureTagName = TAG_CHUKYDONVI;
                signXml09BhADO.ReferenceTagName = TAG_TT_HOSO;
                signXml09BhADO.ConfigData = new EMR.SDO.XmlConfigDataSDO()
                {
                    HsmSerialNumber = SettingSignADO.SerialNumber,
                    HsmType = SettingSignADO.Id,
                    HsmUserCode = SettingSignADO.Name,
                    Password = SettingSignADO.Password,
                    SecretKey = SettingSignADO.SercetKey,
                    IdentityNumber = SettingSignADO.CccdNumber
                };
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<string>("api/EmrSign/SignXml09Bh", HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer, signXml09BhADO, SessionManager.ActionLostToken, param);
                if (param != null && param.Messages != null && param.Messages.Count > 0)
                {
                    apiMessage = string.Join(" | ", param.Messages);
                    Inventec.Common.Logging.LogSystem.Warn("SourceFileSignApi: " + apiMessage);
                }
            }
            catch (Exception ex)
            {
                apiMessage = ex.Message;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string ReadFileContent(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    XmlDocument xmlDocument = new XmlDocument();
                    try
                    {
                        xmlDocument.LoadXml(RemoveByteOrderMark(Encoding.UTF8.GetString(fileBytes)));
                        return Convert.ToBase64String(Encoding.UTF8.GetBytes(RemoveByteOrderMark(Encoding.UTF8.GetString(fileBytes))));
                    }
                    catch (Exception)
                    {
                        xmlDocument.LoadXml(Encoding.UTF8.GetString(fileBytes));
                        return Convert.ToBase64String(fileBytes);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private string RemoveByteOrderMark(string xml)
        {
            string byteOrderMark = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (xml.StartsWith(byteOrderMark))
            {
                xml = xml.Remove(0, byteOrderMark.Length);
            }
            return xml;
        }

        public string AppFilePathSignService()
        {
            try
            {
                return Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "EMR.SignProcessor"), "EMR.SignProcessor.exe");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        private bool IsProcessOpen(string name)
        {
            // Process.GetProcessesByName không bao giờ chứa phần mở rộng .exe -> chỉ so tên thuần
            Process[] processes = null;
            try
            {
                processes = Process.GetProcessesByName(name);
                return processes.Length > 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            finally
            {
                if (processes != null)
                {
                    foreach (var p in processes)
                    {
                        try { p.Dispose(); }
                        catch { }
                    }
                }
            }
        }

        internal bool VerifyServiceSignProcessorIsRunning()
        {
            try
            {
                // Service đang chạy thì hợp lệ, kể cả khi cơ sở cài EMR.SignProcessor ngoài thư mục Integrate
                if (IsProcessOpen("EMR.SignProcessor"))
                    return true;

                string exeSignPath = AppFilePathSignService();
                if (!File.Exists(exeSignPath))
                {
                    Inventec.Common.Logging.LogSystem.Warn("Không tìm thấy service ký số: " + exeSignPath);
                    return false;
                }

                Process.Start(new ProcessStartInfo { FileName = exeSignPath });
                // Chờ host WCF mở, kiểm lại thay vì tin tưởng Sleep cố định
                for (int i = 0; i < 20; i++)
                {
                    Thread.Sleep(250);
                    if (IsProcessOpen("EMR.SignProcessor"))
                        return true;
                }
                Inventec.Common.Logging.LogSystem.Warn("Đã gọi khởi động EMR.SignProcessor nhưng process không xuất hiện sau 5 giây.");
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        #endregion
    }
}
