/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * vCong60263 - Nhật ký gửi XML 130 lên Cổng giám định BHYT.
 * Mỗi lần gửi của mỗi hồ sơ là một dòng, gửi lại nhiều lần thì có nhiều dòng.
 * KHÔNG lưu và KHÔNG hiển thị họ tên, giấy tờ tuỳ thân hay chẩn đoán của người bệnh.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    public partial class frmXml130SendLog : HIS.Desktop.Utility.FormBase
    {
        //Loại dữ liệu gửi của nhật ký: 1 = XML 130 lên Cổng giám định BHYT
        private const short XML_TYPE_130 = 1;
        //Khoảng tra cứu tối đa một lần, tránh kéo cả năm dữ liệu về máy trạm
        private const int MAX_SEARCH_DAYS = 92;
        private const int PAGE_SIZE = 200;
        private const string EXPORT_SHEET_NAME = "NhatKyGuiXML130";

        //Thứ tự lựa chọn trong ô Kết quả gửi / Hình thức gửi
        private const int FILTER_INDEX_ALL = 0;
        private const int RESULT_INDEX_SUCCESS = 1;
        private const int RESULT_INDEX_FAILED = 2;
        private const int MODE_INDEX_AUTO = 1;
        private const int MODE_INDEX_MANUAL = 2;

        private List<Xml130SendLogADO> currentRows = new List<Xml130SendLogADO>();
        //Khi người dùng bấm vào một mã lượt thì chỉ xem đúng lượt đó
        private string filterRoundCode;

        public frmXml130SendLog()
        {
            InitializeComponent();
            this.SetIcon();
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                //Không lấy được biểu tượng thì vẫn mở được cửa sổ
                LogSystem.Warn(ex);
            }
        }

        private void frmXml130SendLog_Load(object sender, EventArgs e)
        {
            try
            {
                this.InitFilterControls();
                this.ShowKeepDaysNote();
                //Mở lên là tra ngay khoảng mặc định (hôm nay) cho đỡ một thao tác
                this.LoadData();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitFilterControls()
        {
            cboResult.Properties.Items.Clear();
            cboResult.Properties.Items.AddRange(new object[] { "Tất cả", "Thành công", "Thất bại" });
            cboResult.SelectedIndex = FILTER_INDEX_ALL;

            cboMode.Properties.Items.Clear();
            cboMode.Properties.Items.AddRange(new object[] { "Tất cả", "Tự động theo lịch", "Gửi tay" });
            cboMode.SelectedIndex = FILTER_INDEX_ALL;

            dtFrom.DateTime = DateTime.Today;
            dtTo.DateTime = DateTime.Now;
        }

        /// <summary>Nhắc người dùng nhật ký chỉ giữ trong bao nhiêu ngày, để không tra ra khoảng trống mà tưởng mất dữ liệu.</summary>
        private void ShowKeepDaysNote()
        {
            try
            {
                int keepDays;
                if (!Int32.TryParse(HisConfigCFG.XML130_SEND_LOG_KEEP_DAYS, NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out keepDays) || keepDays <= 0)
                {
                    keepDays = 90;
                }
                lblKeepDays.Text = String.Format(CultureInfo.InvariantCulture,
                    "(Nhật ký lưu trong {0} ngày gần nhất)", keepDays);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                lblKeepDays.Text = String.Empty;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //Bấm Tra cứu là bỏ lọc theo một lượt, quay về điều kiện trên thanh lọc
                this.filterRoundCode = null;
                this.LoadData();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                DateTime from = dtFrom.DateTime;
                DateTime to = dtTo.DateTime;
                if (to < from)
                {
                    XtraMessageBox.Show("Khoảng thời gian không hợp lệ: Đến ngày đang nhỏ hơn Từ ngày.",
                        Resources.ResourceMessageLang.ThongBao);
                    return;
                }
                if ((to - from).TotalDays > MAX_SEARCH_DAYS)
                {
                    XtraMessageBox.Show(
                        String.Format("Chỉ tra cứu tối đa {0} ngày một lần.", MAX_SEARCH_DAYS),
                        Resources.ResourceMessageLang.ThongBao);
                    return;
                }

                HisTreatmentXmlLogFilter filter = new HisTreatmentXmlLogFilter();
                filter.XML_TYPE = XML_TYPE_130;
                filter.SEND_TIME_FROM = Convert.ToInt64(from.ToString("yyyyMMdd000000", CultureInfo.InvariantCulture));
                //Lấy TRỌN ngày kết thúc: ô chọn ngày trả giờ 00:00 nên nếu lấy nguyên giờ của ô
                //thì toàn bộ dòng của ngày cuối sẽ bị loại khỏi kết quả.
                filter.SEND_TIME_TO = Convert.ToInt64(to.ToString("yyyyMMdd235959", CultureInfo.InvariantCulture));

                if (cboResult.SelectedIndex == RESULT_INDEX_SUCCESS)
                {
                    filter.SEND_RESULT = Xml130SendLogADO.SEND_RESULT_SUCCESS;
                }
                else if (cboResult.SelectedIndex == RESULT_INDEX_FAILED)
                {
                    filter.SEND_RESULT = 0;
                }

                if (cboMode.SelectedIndex == MODE_INDEX_AUTO)
                {
                    filter.SEND_MODE = Xml130SendLogADO.SEND_MODE_AUTO;
                }
                else if (cboMode.SelectedIndex == MODE_INDEX_MANUAL)
                {
                    filter.SEND_MODE = Xml130SendLogADO.SEND_MODE_MANUAL;
                }

                string treatmentCode = (txtTreatmentCode.Text ?? String.Empty).Trim();
                if (!String.IsNullOrEmpty(treatmentCode))
                {
                    filter.TREATMENT_CODE = treatmentCode;
                }
                if (!String.IsNullOrEmpty(this.filterRoundCode))
                {
                    filter.SEND_ROUND_CODE = this.filterRoundCode;
                }
                filter.ORDER_FIELD = "SEND_TIME";
                filter.ORDER_DIRECTION = "DESC";

                CommonParam param = new CommonParam();
                param.Start = 0;
                param.Limit = PAGE_SIZE;

                Cursor.Current = Cursors.WaitCursor;
                List<HIS_TREATMENT_XML_LOG> raws = new BackendAdapter(param)
                    .Post<List<HIS_TREATMENT_XML_LOG>>("api/HisTreatmentXmlLog/GetView",
                        ApiConsumers.MosConsumer, filter, param);

                this.currentRows = new List<Xml130SendLogADO>();
                if (raws != null)
                {
                    int stt = 1;
                    foreach (HIS_TREATMENT_XML_LOG raw in raws)
                    {
                        this.currentRows.Add(Xml130SendLogADO.From(raw, stt));
                        stt++;
                    }
                }

                gridControlLog.DataSource = this.currentRows;
                this.ShowSummary(param);

                if (this.currentRows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu trong khoảng đã chọn.",
                        Resources.ResourceMessageLang.ThongBao);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                XtraMessageBox.Show("Không tra cứu được nhật ký gửi. Xem nhật ký kỹ thuật của phần mềm để biết chi tiết.",
                    Resources.ResourceMessageLang.ThongBao);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void ShowSummary(CommonParam param)
        {
            try
            {
                int total = this.currentRows.Count;
                int success = this.currentRows.Count(o => o.IS_SUCCESS);
                int failed = total - success;
                int rounds = this.currentRows
                    .Where(o => !String.IsNullOrEmpty(o.SEND_ROUND_CODE))
                    .Select(o => o.SEND_ROUND_CODE)
                    .Distinct()
                    .Count();

                string text = String.Format(CultureInfo.InvariantCulture,
                    "Tổng {0} lần gửi · {1} thành công · {2} thất bại · {3} lượt gửi",
                    total, success, failed, rounds);

                //Máy chủ trả về tổng số dòng khớp điều kiện; nhiều hơn một trang thì nói rõ để người dùng thu hẹp khoảng tra
                if (param != null && param.Count.HasValue && param.Count.Value > total)
                {
                    text += String.Format(CultureInfo.InvariantCulture,
                        " (hiển thị {0}/{1} dòng gần nhất, thu hẹp khoảng thời gian để xem tiếp)",
                        total, param.Count.Value);
                }
                if (!String.IsNullOrEmpty(this.filterRoundCode))
                {
                    text += " · đang lọc theo lượt " + this.filterRoundCode;
                }
                lblSummary.Text = text;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Tô màu dòng gửi lỗi để nhìn là thấy ngay.</summary>
        private void gridViewLog_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                Xml130SendLogADO row = gridViewLog.GetRow(e.RowHandle) as Xml130SendLogADO;
                if (row != null && !row.IS_SUCCESS)
                {
                    e.Appearance.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bấm đúp vào cột Mã lượt gửi thì lọc theo đúng lượt đó;
        /// bấm đúp các cột khác thì mở cửa sổ xem trọn nội dung Cổng trả về.
        /// </summary>
        private void gridViewLog_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Xml130SendLogADO row = gridViewLog.GetFocusedRow() as Xml130SendLogADO;
                if (row == null)
                {
                    return;
                }
                if (gridViewLog.FocusedColumn != null
                    && gridViewLog.FocusedColumn.FieldName == "SEND_ROUND_CODE"
                    && !String.IsNullOrEmpty(row.SEND_ROUND_CODE))
                {
                    this.filterRoundCode = row.SEND_ROUND_CODE;
                    this.LoadData();
                    return;
                }
                this.ShowFullDescription(row);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ShowFullDescription(Xml130SendLogADO row)
        {
            try
            {
                string content = String.Format(CultureInfo.InvariantCulture,
                    "Mã điều trị: {0}{4}Thời gian gửi: {1}{4}Kết quả: {2}{4}Mã kiểm tra: {3}{4}{4}Nội dung Cổng giám định trả về:{4}{5}",
                    row.TREATMENT_CODE, row.SEND_TIME_TEXT, row.SEND_RESULT_TEXT, row.CHECK_CODE,
                    Environment.NewLine,
                    String.IsNullOrEmpty(row.SEND_DESC) ? "(Cổng không trả về nội dung)" : row.SEND_DESC);
                XtraMessageBox.Show(content, "Chi tiết lần gửi");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.currentRows == null || this.currentRows.Count == 0)
                {
                    XtraMessageBox.Show("Chưa có dòng nhật ký nào để kết xuất.",
                        Resources.ResourceMessageLang.ThongBao);
                    return;
                }

                string filePath = this.ChooseExportFilePath();
                if (String.IsNullOrEmpty(filePath))
                {
                    return; //người dùng bấm Huỷ
                }

                this.WriteRowsToFile(filePath);
                XtraMessageBox.Show("Đã kết xuất " + this.currentRows.Count + " dòng ra tệp:" + Environment.NewLine + filePath,
                    Resources.ResourceMessageLang.ThongBao);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                XtraMessageBox.Show("Không kết xuất được tệp. Xem nhật ký kỹ thuật của phần mềm để biết chi tiết.",
                    Resources.ResourceMessageLang.ThongBao);
            }
        }

        private string ChooseExportFilePath()
        {
            try
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                    dialog.DefaultExt = "xlsx";
                    dialog.AddExtension = true;
                    dialog.OverwritePrompt = true;
                    dialog.FileName = "NhatKyGuiXML130_"
                        + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + ".xlsx";
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        return dialog.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return null;
        }

        private void WriteRowsToFile(string filePath)
        {
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();
            Aspose.Cells.Worksheet sheet = workbook.Worksheets[0];
            sheet.Name = EXPORT_SHEET_NAME;

            string[] headers = new string[]
            {
                "STT", "Thời gian gửi", "Mã điều trị", "Mã bệnh nhân", "Hình thức gửi",
                "Kết quả", "Mã kiểm tra", "Nội dung Cổng trả về", "Tài khoản", "Máy thực hiện", "Mã lượt gửi"
            };
            for (int col = 0; col < headers.Length; col++)
            {
                sheet.Cells[0, col].PutValue(headers[col]);
            }

            int row = 1;
            foreach (Xml130SendLogADO item in this.currentRows)
            {
                sheet.Cells[row, 0].PutValue(item.STT);
                sheet.Cells[row, 1].PutValue(item.SEND_TIME_TEXT);
                sheet.Cells[row, 2].PutValue(item.TREATMENT_CODE);
                sheet.Cells[row, 3].PutValue(item.PATIENT_CODE);
                sheet.Cells[row, 4].PutValue(item.SEND_MODE_TEXT);
                sheet.Cells[row, 5].PutValue(item.SEND_RESULT_TEXT);
                sheet.Cells[row, 6].PutValue(item.CHECK_CODE);
                sheet.Cells[row, 7].PutValue(item.SEND_DESC);
                sheet.Cells[row, 8].PutValue(item.CREATOR);
                sheet.Cells[row, 9].PutValue(item.MACHINE_NAME);
                sheet.Cells[row, 10].PutValue(item.SEND_ROUND_CODE);
                row++;
            }

            //Dòng tổng hợp ngay dưới danh sách để người nhận tệp đối chiếu được
            row++;
            sheet.Cells[row, 1].PutValue(lblSummary.Text);

            sheet.AutoFitColumns();
            //Cột nội dung lỗi rất dài, để tự co thì bảng không đọc được
            sheet.Cells.SetColumnWidth(7, 80);
            workbook.Save(filePath);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
