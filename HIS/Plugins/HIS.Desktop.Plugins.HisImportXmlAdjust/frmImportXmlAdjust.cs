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
        // Thẻ mà Reference thứ hai của chữ ký trỏ tới (đường HSM gửi tên thẻ này xuống backend)
        const string TAG_TT_HOSO = "TT_HOSO";
        const string XMLDSIG_NAMESPACE = "http://www.w3.org/2000/09/xmldsig#";
        const string TEMP_FOLDER_NAME = "HisImportXmlAdjust";
        // true  = tự ký trong plugin theo profile cổng 09/BH (2 Reference + SigningTime) - Sign\Xml09BHSigner.cs
        // false = quay lại ký qua service EMR.SignProcessor (SignXml130, 1 Reference URI="") để đối chiếu
        const bool USE_LOCAL_SIGNER_09BH = true;
        string[] DATE_FORMATS = new string[] { "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss", "d/M/yyyy HH:mm", "d/M/yyyy H:mm", "dd/MM/yyyy", "yyyyMMddHHmm", "yyyyMMddHHmmss", "yyyyMMdd" };

        SettingSignADO SettingSignADO;
        // Đặt bởi CheckSignPrerequisite để SignFile không phải quét lại danh sách process cho từng hồ sơ
        private bool signServiceVerified;
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
                        if (string.IsNullOrEmpty(item.XML1_ID) && string.IsNullOrEmpty(item.EXPENSE_ID))
                            error += string.Format(MessageImport.ThieuTruongDL, "XML1_ID hoặc ID chi phí");
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

                    // Chi phí (có ID chi phí): SOBANG_XML chỉ nhận 2 hoặc 3 (tài liệu 09/BH mục 9)
                    if (!string.IsNullOrEmpty(item.EXPENSE_ID))
                    {
                        if (item.XML_TABLE_NUMBER != "2" && item.XML_TABLE_NUMBER != "3")
                            error += "Số bảng XML chỉ nhận giá trị 2 hoặc 3. ";
                    }

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
        /// Dựng danh sách hồ sơ điều chỉnh - mỗi MA_LK là 1 XmlHoSoDieuChinhGD chứa đúng 1 TT_HOSO
        /// (đúng tài liệu: mỗi lần gửi 01 hồ sơ, chữ ký ký 1 TT_HOSO).
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

            // Gom theo LINK_CODE → mỗi LINK_CODE = 1 hồ sơ = 1 file XML riêng
            var groupByLinkCode = _XmlAdjustAdos.GroupBy(o => o.LINK_CODE ?? "").ToList();

            foreach (var group in groupByLinkCode)
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
                        KY_QT = first != null && first.IN_DATE.HasValue ? first.IN_DATE.Value.ToString("yyyyMM") : DateTime.Now.ToString("yyyyMM"),
                        TRANGTHAI = first != null ? first.STATUS ?? "1" : "1"
                    },
                    TT_DIEUCHINH = new XmlTTDieuChinh()
                };

                var items = group.ToList();

                // DS_XML1_DIEUCHINH: dòng không có EXPENSE_ID
                var xml1DcList = items.Where(o => string.IsNullOrEmpty(o.EXPENSE_ID)).ToList();
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
                ttHoSo.TT_DIEUCHINH.DS_XML1_DIEUCHINH = dsXml1;

                // DSCP_DIEUCHINH: dòng có EXPENSE_ID
                var cpDcList = items.Where(o => !string.IsNullOrEmpty(o.EXPENSE_ID)).ToList();
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
                ttHoSo.TT_DIEUCHINH.DSCP_DIEUCHINH = dsCp;

                var xmlData = new XmlHoSoDieuChinhGD
                {
                    TT_HOSO = new List<XmlTTHoSo> { ttHoSo },
                    ChuKyDonVi = ""
                };
                result.Add(xmlData);
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
                    Indent = false,
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

        private void btnPushPortal_Click(object sender, EventArgs e)
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

                // Cấu hình cổng: ưu tiên key riêng 09/BH, nếu trống dùng lại config XML130
                string connectionInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY_09BH_CONNECTION_INFO);
                if (string.IsNullOrEmpty(connectionInfo))
                    connectionInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY_CONNECTION_INFO);
                string address = null, portalUsername = null, portalPassword = null;
                if (!string.IsNullOrEmpty(connectionInfo))
                {
                    var parts = connectionInfo.Split('|');
                    if (parts.Length > 0) address = parts[0];
                    if (parts.Length > 1) portalUsername = parts[1];
                    if (parts.Length > 2) portalPassword = parts[2];
                }
                if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(portalUsername) || string.IsNullOrEmpty(portalPassword))
                {
                    XtraMessageBox.Show("Chưa cấu hình thông tin kết nối cổng BHXH (key HIS.HSDC_09BH.CONNECTION_INFO hoặc HIS.QD_130_BYT.CONNECTION_INFO).",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var hoSoList = BuildHoSoList();
                if (hoSoList.Count == 0) return;

                if (XtraMessageBox.Show(
                    string.Format("Bạn có chắc chắn muốn đẩy {0} hồ sơ điều chỉnh mẫu 09/BH lên cổng giám định BHXH?", hoSoList.Count),
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();

                var api = new Portal.Portal09BHApi();
                int okCount = 0;
                var errLines = new List<string>();
                var okLines = new List<string>();

                foreach (var xml in hoSoList)
                {
                    string maLk = GetMaLk(xml);
                    string maCsKCB = GetMaCsKCB(xml);
                    string kyQt = GetKyQt(xml);
                    // Mã tỉnh = 2 ký tự đầu mã CSKCB (chuẩn BHYT: 2 số tỉnh + số cơ sở)
                    string maTinh = (maCsKCB != null && maCsKCB.Length >= 2) ? maCsKCB.Substring(0, 2) : "";

                    string err;
                    string fileBase64 = SignXmlToBase64(xml, out err);
                    if (string.IsNullOrEmpty(fileBase64))
                    {
                        errLines.Add(string.Format("MA_LK {0}: {1}", maLk, err ?? "không tạo được XML"));
                        continue;
                    }

                    var rs = api.Send(address, portalUsername, portalPassword, maCsKCB, kyQt, maTinh, fileBase64);
                    if (rs.Success)
                    {
                        okCount++;
                        okLines.Add(string.Format("MA_LK {0}: {1}", maLk, rs.MaGiaoDich));
                    }
                    else
                    {
                        errLines.Add(string.Format("MA_LK {0}: {1}", maLk,
                            !string.IsNullOrEmpty(rs.ThongDiep) ? rs.ThongDiep : rs.ErrorMessage));
                    }
                }

                WaitingManager.Hide();

                var sb = new StringBuilder();
                sb.AppendLine(string.Format("Đẩy cổng xong: {0}/{1} hồ sơ thành công.", okCount, hoSoList.Count));
                if (okLines.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("Thành công (mã giao dịch):");
                    sb.AppendLine(string.Join(Environment.NewLine, okLines));
                }
                if (errLines.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("Hồ sơ lỗi:");
                    sb.AppendLine(string.Join(Environment.NewLine, errLines));
                }
                XtraMessageBox.Show(sb.ToString(), "Kết quả đẩy cổng", MessageBoxButtons.OK,
                    errLines.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi xảy ra khi đẩy cổng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            if (!SettingSignADO.IsHsm)
            {
                if (!VerifyServiceSignProcessorIsRunning())
                {
                    error = "Không khởi động được service ký số EMR.SignProcessor. Kiểm tra thư mục Integrate\\EMR.SignProcessor, hoặc bỏ tích \"Ký số\" nếu chỉ muốn xuất file chưa ký.";
                    return false;
                }
                signServiceVerified = true;
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

                string tempFolderPath = Path.Combine(Path.GetTempPath(), TEMP_FOLDER_NAME);
                Directory.CreateDirectory(tempFolderPath);
                tempFilePath = Path.Combine(tempFolderPath, Guid.NewGuid().ToString("N") + "_" + SafeFileName(fullFileName));
                string pathAfterFileSign = null;

                if (SettingSignADO.IsHsm)
                {
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
                else if (USE_LOCAL_SIGNER_09BH)
                {
                    // Tự ký trong plugin để ra ĐÚNG profile cổng yêu cầu (2 Reference + SigningTime).
                    // Service EMR.SignProcessor chỉ biết SignXml130 = 1 Reference URI="" nên không dùng ở đây.
                    if (!Sign.Xml09BHSigner.SignFile(saveFilePath, SettingSignADO.SerialNumber, out signError))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("SignFile: " + signError);
                        return false;
                    }
                    // Tự kiểm lại chữ ký vừa tạo trước khi cho đi tiếp
                    if (!IsSignedXmlFile(saveFilePath, true, out signError))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("SignFile: " + signError);
                        return false;
                    }
                    return true;
                }
                else
                {
                    // Đã kiểm ở CheckSignPrerequisite thì không quét lại process cho từng hồ sơ
                    if (!signServiceVerified && !VerifyServiceSignProcessorIsRunning())
                    {
                        signError = "Service ký số EMR.SignProcessor không chạy.";
                        Inventec.Common.Logging.LogSystem.Warn("SignFile: " + signError);
                        return false;
                    }

                    WcfSignDCO wcfSignDCO = new WcfSignDCO
                    {
                        SerialNumber = SettingSignADO.SerialNumber,
                        OutputFile = tempFilePath,
                        PIN = "",
                        SourceFile = saveFilePath,
                        fieldSigned = TAG_CHUKYDONVI
                    };
                    string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                    SignProcessorClient signProcessorClient = new SignProcessorClient();
                    try
                    {
                        var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                        if (wcfSignResultDCO == null || !wcfSignResultDCO.Success)
                        {
                            signError = "Ký file thất bại: " + (wcfSignResultDCO != null && !string.IsNullOrEmpty(wcfSignResultDCO.Message)
                                ? wcfSignResultDCO.Message : "service ký số không trả về kết quả");
                            Inventec.Common.Logging.LogSystem.Warn("SignFile: " + signError);
                            return false;
                        }
                        pathAfterFileSign = wcfSignResultDCO.OutputFile;
                    }
                    finally
                    {
                        CloseSignClient(signProcessorClient);
                    }
                }

                // Chỉ ghi đè file thật khi kết quả ký thực sự có chữ ký -> không bao giờ để file chưa ký đi tiếp.
                // Kiểm luôn phần mật mã cho CẢ HAI đường (USB và HSM): backend đã ký HSM ra đúng profile
                // 09/BH và chữ ký verify được (đối chiếu giống từng byte với đường USB), nên chữ ký HSM
                // không kiểm chứng được giờ là LỖI THẬT, phải chặn chứ không chỉ ghi log cảnh báo.
                if (!IsSignedXmlFile(pathAfterFileSign, true, out signError))
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
        /// Xác nhận file XML sau khi ký: tồn tại, không rỗng, đọc được và có thẻ Signature nằm trong CHUKYDONVI.
        /// verifyCryptography = true thì kiểm luôn toàn vẹn chữ ký (digest + SignatureValue) bằng SignedXml.CheckSignature.
        /// LƯU Ý: CheckSignature chỉ chứng minh chữ ký khớp nội dung file, KHÔNG chứng minh đúng profile chữ ký
        /// mà cổng BHXH yêu cầu cho mẫu 09/BH (2 Reference + SigningTime) - việc đó thuộc mục A3, chưa sửa.
        /// </summary>
        private static bool IsSignedXmlFile(string filePath, bool verifyCryptography, out string error)
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
                XmlNodeList chuKyNodes = doc.GetElementsByTagName(TAG_CHUKYDONVI);
                XmlElement chuKy = (chuKyNodes != null && chuKyNodes.Count > 0) ? chuKyNodes[0] as XmlElement : null;
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

                if (verifyCryptography)
                {
                    try
                    {
                        var signedXml = new System.Security.Cryptography.Xml.SignedXml(doc);
                        signedXml.LoadXml(signature);
                        if (!signedXml.CheckSignature())
                        {
                            error = "Chữ ký không khớp nội dung file (digest hoặc SignatureValue sai).";
                            return false;
                        }
                    }
                    catch (Exception exVerify)
                    {
                        error = "Không kiểm tra được chữ ký: " + exVerify.Message;
                        Inventec.Common.Logging.LogSystem.Warn(exVerify);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                error = "File kết quả ký số không đọc được: " + ex.Message;
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
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
