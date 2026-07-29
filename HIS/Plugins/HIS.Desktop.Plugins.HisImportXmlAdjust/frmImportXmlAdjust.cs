using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using EMR.WCF.DCO;
using HIS.Desktop.ADO;
using HIS.Desktop.Common;
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
        string[] DATE_FORMATS = new string[] { "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss", "d/M/yyyy HH:mm", "d/M/yyyy H:mm", "dd/MM/yyyy", "yyyyMMddHHmm", "yyyyMMddHHmmss", "yyyyMMdd" };

        SettingSignADO SettingSignADO;
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
                if (_XmlAdjustAdos == null || _XmlAdjustAdos.Count == 0) return;
                if (_XmlAdjustAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR))) return;

                string savePath = "";
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Chọn thư mục lưu file XML TT12 (mỗi hồ sơ 1 file)";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    savePath = fbd.SelectedPath;
                }
                if (string.IsNullOrEmpty(savePath)) return;

                WaitingManager.Show();

                // Mỗi bệnh nhân (MA_LK) = 1 file XML riêng (đúng tài liệu: 1 hồ sơ/lần)
                var hoSoList = BuildHoSoList();
                int okCount = 0, failCount = 0;
                string timestamp = DateTime.Now.ToString("ddMMyyyy_HHmmss");
                foreach (var xml in hoSoList)
                {
                    try
                    {
                        string maLk = GetMaLk(xml);
                        string fileName = string.Format("HOSO_DIEUCHINH_GD_{0}_{1}.xml", SafeFileName(maLk), timestamp);
                        string saveFilePath = Path.Combine(savePath, fileName);
                        var rs = CreateXmlFile(xml);
                        if (rs == null) { failCount++; continue; }
                        using (FileStream file = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write))
                        {
                            rs.WriteTo(file);
                        }
                        rs.Close();
                        if (chkSign.Checked)
                        {
                            SignFile(fileName, saveFilePath);
                        }
                        okCount++;
                    }
                    catch (Exception exItem)
                    {
                        failCount++;
                        Inventec.Common.Logging.LogSystem.Error(exItem);
                    }
                }

                WaitingManager.Hide();
                XtraMessageBox.Show(
                    string.Format("Đã xuất {0} file XML (mỗi hồ sơ 1 file).{1}", okCount,
                        failCount > 0 ? "\r\nLỗi: " + failCount + " hồ sơ." : ""),
                    "Thông báo", MessageBoxButtons.OK,
                    failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private static MemoryStream CreateXmlFile<T>(T input)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = new UTF8Encoding(false)
                };

                var ms = new MemoryStream();
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    // Khai báo xsd/xsi ở thẻ root như mẫu MAU_09.signed
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("xsd", "http://www.w3.org/2001/XMLSchema");
                    ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
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
                if (_XmlAdjustAdos == null || _XmlAdjustAdos.Count == 0) return;
                if (_XmlAdjustAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR))) return;

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

                // Ký số nếu người dùng chọn (cổng yêu cầu XML có chữ ký đơn vị)
                if (chkSign.Checked)
                {
                    SignFile(fileName, tempFile);
                }

                byte[] bytes = File.ReadAllBytes(tempFile);
                string xmlContent = RemoveByteOrderMark(Encoding.UTF8.GetString(bytes));
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "[DAY_CONG_09BH] XML INPUT (đã ký={0}, MA_LK={1}, độ dài={2}):{3}{4}",
                    chkSign.Checked, GetMaLk(xml), xmlContent.Length, Environment.NewLine, xmlContent));
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

        public bool SignFile(string fullFileName, string saveFilePath)
        {
            try
            {
                if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                {
                    MessageBox.Show("Không có thông tin Usb Token ký số");
                    return false;
                }

                string currentDirectory = Directory.GetCurrentDirectory();
                string tempFolderPath = Path.Combine(currentDirectory, "Temp");
                Directory.CreateDirectory(tempFolderPath);
                string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                File.Create(tempFilePath).Close();
                string pathAfterFileSign = null;

                if (SettingSignADO.IsHsm)
                {
                    var xmlBase64 = SourceFileSignApi(ReadFileContent(saveFilePath));
                    if (string.IsNullOrEmpty(xmlBase64))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Ký HSM thất bại");
                        return false;
                    }
                    var xmlBytes = Convert.FromBase64String(xmlBase64);
                    File.WriteAllBytes(tempFilePath, xmlBytes);
                    pathAfterFileSign = tempFilePath;
                }
                else
                {
                    WcfSignDCO wcfSignDCO = new WcfSignDCO
                    {
                        SerialNumber = SettingSignADO.SerialNumber,
                        OutputFile = tempFilePath,
                        PIN = "",
                        SourceFile = saveFilePath,
                        fieldSigned = "CHUKYDONVI"
                    };
                    string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                    SignProcessorClient signProcessorClient = new SignProcessorClient();
                    if (!VerifyServiceSignProcessorIsRunning())
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Service ký số không chạy");
                    }
                    var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                    if (wcfSignResultDCO == null || !wcfSignResultDCO.Success)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Ký file thất bại: " + (wcfSignResultDCO != null ? wcfSignResultDCO.Message : ""));
                        return false;
                    }
                    pathAfterFileSign = wcfSignResultDCO.OutputFile;
                }

                if (!string.IsNullOrEmpty(pathAfterFileSign) && File.Exists(pathAfterFileSign))
                {
                    File.Copy(pathAfterFileSign, saveFilePath, true);
                }
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
                if (Directory.Exists(tempFolderPath) && Directory.GetFiles(tempFolderPath).Length == 0 && Directory.GetDirectories(tempFolderPath).Length == 0)
                {
                    Directory.Delete(tempFolderPath);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }

        private string SourceFileSignApi(string xmlBase64Source)
        {
            string result = null;
            try
            {
                CommonParam param = new CommonParam();
                EMR.SDO.SignXmlBhytSDO signXmlBhytSDO = new EMR.SDO.SignXmlBhytSDO();
                signXmlBhytSDO.XmlBase64 = xmlBase64Source;
                signXmlBhytSDO.TagStoreSignatureValue = "CHUKYDONVI";
                signXmlBhytSDO.ConfigData = new EMR.SDO.XmlConfigDataSDO()
                {
                    HsmSerialNumber = SettingSignADO.SerialNumber,
                    HsmType = SettingSignADO.Id,
                    HsmUserCode = SettingSignADO.Name,
                    Password = SettingSignADO.Password,
                    SecretKey = SettingSignADO.SercetKey,
                    IdentityNumber = SettingSignADO.CccdNumber
                };
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<string>("api/EmrSign/SignXmlBhyt", HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer, signXmlBhytSDO, param);
                if (param != null && param.Messages != null && param.Messages.Count > 0)
                {
                    string message = string.Join(Environment.NewLine, param.Messages);
                    XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Inventec.Common.Logging.LogSystem.Warn(message);
                }
            }
            catch (Exception ex)
            {
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
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == string.Format("{0}.exe", name))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool VerifyServiceSignProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("EMR.SignProcessor"))
                    {
                        valid = true;
                    }
                    else
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {
                            Process.Start(startInfo);
                            Thread.Sleep(500);
                            valid = true;
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        #endregion
    }
}
