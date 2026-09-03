using DevExpress.XtraRichEdit;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisProductInfo
{
    public partial class frmProductInfo : FormBase
    {
        //Thu muc luu file tren FSS
        const string FSS_STORE_LOCATION = "ProductInfo";

        //Cho phep ca PDF lan Word. Muc dau gop ca hai de nguoi dung khong phai doi bo loc.
        const string FILE_FILTER =
            "Tệp thông tin sản phẩm (*.pdf, *.doc, *.docx)|*.pdf;*.doc;*.docx"
            + "|Tệp PDF (*.pdf)|*.pdf"
            + "|Tệp Word (*.doc, *.docx)|*.doc;*.docx";

        Inventec.Desktop.Common.Modules.Module module = null;
        ProductInfoADO data = null;
        HIS_PRODUCT_INFO currentProductInfo = null;

        //Noi dung tep nguoi dung vua chon. Null = chua chon tep moi trong phien nay.
        byte[] selectedFileContent = null;
        string selectedFileName = "";

        //true = dang o che do go noi dung, Luu se cat chuoi RTF.
        //false = dang o che do xem tep, Luu se tai tep len va cat duong dan.
        bool isTextMode = false;

        //PdfViewer doc lazy tren stream nen phai giu stream song suot vong doi tai lieu, khong duoc dispose som.
        //RichEditControl thi nap thang vao document model nen khong can giu stream.
        MemoryStream currentDocumentStream = null;

        public frmProductInfo(Inventec.Desktop.Common.Modules.Module _module, ProductInfoADO ado)
            : base(_module)
        {
            InitializeComponent();
            this.module = _module;
            this.data = ado;
        }

        private bool AllowEdit
        {
            get { return this.data != null && this.data.ProductInfoOpen == 1; }
        }

        private void frmProductInfo_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaption();
                SetDefaultValue();
                LoadDataToControl();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetCaption()
        {
            try
            {
                if (this.module != null && this.module.text != null)
                {
                    this.Text = this.module.text.ToString();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.btnChooseFile.Enabled = AllowEdit;
                this.btnEditContent.Enabled = AllowEdit;
                this.btnSave.Enabled = AllowEdit;
                this.btnDelete.Enabled = false;
                this.pdfViewer1.Visible = false;
                this.richEditContent.Visible = false;
                this.richEditContent.ReadOnly = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadDataToControl()
        {
            try
            {
                if (this.data == null || this.data.MedicineTypeId <= 0) return;

                CommonParam param = new CommonParam();
                MOS.Filter.HisProductInfoFilter filter = new MOS.Filter.HisProductInfoFilter();
                filter.MEDICINE_TYPE_ID = this.data.MedicineTypeId;

                WaitingManager.Show();
                try
                {
                    var rs = new BackendAdapter(param).Get<List<HIS_PRODUCT_INFO>>("api/HisProductInfo/Get", ApiConsumers.MosConsumer, filter, param);
                    this.currentProductInfo = (rs != null && rs.Count > 0) ? rs.FirstOrDefault() : null;
                }
                finally
                {
                    WaitingManager.Hide();
                }

                string stored = (this.currentProductInfo != null) ? this.currentProductInfo.PRODUCT_INFO : null;
                if (string.IsNullOrWhiteSpace(stored))
                {
                    //Chua co gi: mo san o go noi dung cho nguoi dung nhap luon nhu ban cu.
                    if (AllowEdit) EnterTextMode("");
                }
                else
                {
                    LoadStoredContent(stored);
                }
                EnableControlChange();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hien thi noi dung dang luu trong cot PRODUCT_INFO.
        /// Gia tri co the la duong dan tep tren FSS, hoac chuoi RTF do nguoi dung go tay.
        /// </summary>
        private void LoadStoredContent(string storedValue)
        {
            try
            {
                //Noi dung go tay nam thang trong cot, khong phai duong dan tep.
                if (IsLegacyRichTextContent(storedValue))
                {
                    if (AllowEdit) EnterTextMode(storedValue);
                    else ShowRichText(storedValue, true);
                    return;
                }

                //FileDownload.GetFile khong bao gio tra null: tep mat, FSS chet, URL sai deu bi goi lai thanh
                //exception va nem ra, nen bat buoc phai bat o day thi thong bao ben duoi moi chay toi.
                WaitingManager.Show();
                MemoryStream stream = null;
                try
                {
                    stream = Inventec.Fss.Client.FileDownload.GetFile(storedValue);
                }
                catch (Exception ex)
                {
                    LogSystem.Error("Khong tai duoc file tu FSS. storedValue = " + storedValue, ex);
                    stream = null;
                }
                finally
                {
                    WaitingManager.Hide();
                }

                if (stream == null || stream.Length <= 0)
                {
                    LogSystem.Warn("Khong tai duoc file tu FSS. storedValue = " + storedValue);
                    MessageBox.Show(this,
                        "Không tải được tệp thông tin sản phẩm đã lưu. Vui lòng liên hệ quản trị hệ thống.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool shown;
                if (IsWordName(storedValue))
                {
                    shown = ShowWord(stream.ToArray(), storedValue);
                    stream.Dispose();
                }
                else
                {
                    stream.Position = 0;
                    shown = ShowPdf(stream);
                }

                if (!shown)
                {
                    MessageBox.Show(this, "Tệp thông tin sản phẩm đã lưu không mở được.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nap noi dung tep vao o xem tuong ung voi dinh dang. Tra ve false neu khong doc duoc,
        /// khi do tai lieu dang hien thi duoc giu nguyen.
        /// </summary>
        private bool ShowFile(byte[] content, string fileName)
        {
            if (IsWordName(fileName))
            {
                return ShowWord(content, fileName);
            }
            return ShowPdf(new MemoryStream(content));
        }

        private bool ShowPdf(MemoryStream stream)
        {
            MemoryStream previous = this.currentDocumentStream;
            try
            {
                this.pdfViewer1.LoadDocument(stream);
            }
            catch (Exception ex)
            {
                //PDF hong hoac co mat khau thi LoadDocument nem exception ngay tai day,
                //tai lieu dang mo van con nguyen nen tuyet doi khong duoc nuot loi.
                LogSystem.Error(ex);
                stream.Dispose();
                return false;
            }
            this.currentDocumentStream = stream;
            if (previous != null)
            {
                previous.Dispose();
            }
            this.isTextMode = false;
            this.richEditContent.ReadOnly = true;
            SetActiveViewer(true);
            return true;
        }

        private bool ShowWord(byte[] content, string fileName)
        {
            try
            {
                //RichEditControl nap thang vao document model nen dong stream ngay sau do van an toan.
                using (MemoryStream stream = new MemoryStream(content))
                {
                    this.richEditContent.LoadDocument(stream, ResolveWordFormat(fileName));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
            this.isTextMode = false;
            this.richEditContent.ReadOnly = true;
            SetActiveViewer(false);
            return true;
        }

        private bool ShowRichText(string rtf, bool readOnly)
        {
            try
            {
                this.richEditContent.RtfText = rtf ?? "";
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
            this.richEditContent.ReadOnly = readOnly;
            SetActiveViewer(false);
            return true;
        }

        /// <summary>
        /// Chuyen sang che do go noi dung: mo khoa o soan thao, bo tep dang chon.
        /// </summary>
        private void EnterTextMode(string rtf)
        {
            try
            {
                this.selectedFileContent = null;
                this.selectedFileName = "";
                this.isTextMode = true;
                ShowRichText(rtf, false);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnEditContent_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.isTextMode)
                {
                    this.richEditContent.Focus();
                    return;
                }

                //Dang xem tep ma chuyen sang go tay thi khi Luu se thay the tep, phai hoi truoc.
                bool hasStoredFile = this.currentProductInfo != null
                    && !string.IsNullOrWhiteSpace(this.currentProductInfo.PRODUCT_INFO)
                    && !IsLegacyRichTextContent(this.currentProductInfo.PRODUCT_INFO);

                if (hasStoredFile || this.selectedFileContent != null)
                {
                    if (MessageBox.Show(this,
                            "Chuyển sang nhập nội dung. Khi bấm Lưu, nội dung nhập tay sẽ thay thế tệp đang lưu. Tiếp tục?",
                            "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    {
                        return;
                    }
                }

                //Dang xem Word thi giu lai noi dung de sua tiep; dang xem PDF thi khong chuyen duoc, mo trang.
                bool keepCurrentText = this.richEditContent.Visible;
                EnterTextMode(keepCurrentText ? this.richEditContent.RtfText : "");
                this.richEditContent.Focus();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hai o xem nam chong len nhau trong panelViewer, moi luc chi bat mot cai.
        /// </summary>
        private void SetActiveViewer(bool isPdf)
        {
            try
            {
                this.pdfViewer1.Visible = isPdf;
                this.richEditContent.Visible = !isPdf;
                if (isPdf)
                {
                    this.pdfViewer1.BringToFront();
                }
                else
                {
                    this.richEditContent.BringToFront();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private static DocumentFormat ResolveWordFormat(string fileName)
        {
            if (EndsWith(fileName, ".docx")) return DocumentFormat.OpenXml;
            if (EndsWith(fileName, ".rtf")) return DocumentFormat.Rtf;
            return DocumentFormat.Doc;
        }

        private void EnableControlChange()
        {
            try
            {
                this.btnDelete.Enabled = AllowEdit && this.currentProductInfo != null;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFile = new OpenFileDialog())
                {
                    openFile.Filter = FILE_FILTER;
                    openFile.Multiselect = false;
                    if (openFile.ShowDialog(this) != DialogResult.OK) return;

                    string fileName = Path.GetFileName(openFile.FileName);
                    if (!IsSupportedName(fileName))
                    {
                        MessageBox.Show(this,
                            "Chỉ hỗ trợ tệp PDF hoặc Word. Vui lòng chọn tệp khác.",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    byte[] content = File.ReadAllBytes(openFile.FileName);
                    if (content == null || content.Length <= 0)
                    {
                        MessageBox.Show(this, "Tệp đã chọn rỗng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    //Doc het ra byte[] roi nap tu MemoryStream de khong giu khoa tren tep goc cua nguoi dung.
                    //Chi ghi nhan tep SAU khi hien thi duoc, de khong bao gio luu len server mot tep chua he xem duoc.
                    if (!ShowFile(content, fileName))
                    {
                        MessageBox.Show(this,
                            "Tệp không đọc được: có thể tệp đã hỏng hoặc đang được bảo vệ bằng mật khẩu. Vui lòng chọn tệp khác.",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    this.selectedFileContent = content;
                    this.selectedFileName = fileName;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                MessageBox.Show(this, "Không đọc được tệp đã chọn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessSave();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ProcessSave()
        {
            try
            {
                if (this.isTextMode) SaveTextContent();
                else SaveFileContent();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                MessageBox.Show(this, "Lưu thông tin sản phẩm không thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Che do go tay: cat thang chuoi RTF vao cot, giong het cach lam cu.
        /// </summary>
        private void SaveTextContent()
        {
            if (string.IsNullOrWhiteSpace(this.richEditContent.Text))
            {
                MessageBox.Show(this, "Vui lòng nhập nội dung trước khi lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            CommonParam param = new CommonParam();
            bool success = false;
            string oldStoredValue = (this.currentProductInfo != null) ? this.currentProductInfo.PRODUCT_INFO : null;

            WaitingManager.Show();
            try
            {
                success = SaveToBackend(this.richEditContent.RtfText, param);

                //Truoc do dang luu tep ma nay chuyen sang go tay thi phai xoa tep cu, khong thi FSS dan rac.
                if (success && !string.IsNullOrWhiteSpace(oldStoredValue) && !IsLegacyRichTextContent(oldStoredValue))
                {
                    DeleteFromStore(oldStoredValue);
                }
            }
            finally
            {
                WaitingManager.Hide();
            }

            EnableControlChange();
            MessageManager.Show(this, param, success);
        }

        /// <summary>
        /// Che do tep: tai tep len FSS roi cat duong dan tra ve vao cot.
        /// </summary>
        private void SaveFileContent()
        {
            if (this.selectedFileContent == null || this.selectedFileContent.Length <= 0)
            {
                MessageBox.Show(this, "Vui lòng chọn tệp trước khi lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            CommonParam param = new CommonParam();
            bool success = false;
            bool uploaded = false;
            string oldStoredValue = (this.currentProductInfo != null) ? this.currentProductInfo.PRODUCT_INFO : null;
            string newUrl = null;

            //Chi hien thong bao sau khi da tat man cho, tranh hop thoai bi lop cho che khuat.
            WaitingManager.Show();
            try
            {
                //FileUpload.UploadFile khong bao gio tra null: FSS chet/timeout/sai cau hinh deu nem
                //FileUploadException. Khong bat o day thi bam Luu se ket thuc im lang, nguoi dung tuong da luu duoc.
                FileUploadInfo uploadInfo = null;
                try
                {
                    uploadInfo = UploadToStore();
                }
                catch (Exception ex)
                {
                    LogSystem.Error("Upload file thong tin san pham len FSS that bai.", ex);
                }

                if (uploadInfo == null || string.IsNullOrWhiteSpace(uploadInfo.Url))
                {
                    LogSystem.Warn("Upload file thong tin san pham len FSS that bai.");
                }
                else
                {
                    uploaded = true;
                    newUrl = uploadInfo.Url;
                    success = SaveToBackend(newUrl, param);
                }

                //Doi tu Word sang PDF (hoac nguoc lai) thi ten tep khac phan mo rong nen sinh tep moi,
                //phai xoa tep cu di khong thi FSS dan rac.
                if (success && !string.IsNullOrWhiteSpace(oldStoredValue)
                    && !IsLegacyRichTextContent(oldStoredValue)
                    && !string.Equals(oldStoredValue, newUrl, StringComparison.OrdinalIgnoreCase))
                {
                    DeleteFromStore(oldStoredValue);
                }
            }
            finally
            {
                WaitingManager.Hide();
            }

            EnableControlChange();
            if (!uploaded)
            {
                MessageBox.Show(this, "Tải tệp lên máy chủ không thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageManager.Show(this, param, success);
        }

        /// <summary>
        /// Ghi gia tri xuong cot PRODUCT_INFO. Gia tri la chuoi RTF hoac duong dan tep tuy che do.
        /// </summary>
        private bool SaveToBackend(string value, CommonParam param)
        {
            if (this.currentProductInfo != null)
            {
                //update
                HIS_PRODUCT_INFO updateData = this.currentProductInfo;
                updateData.PRODUCT_INFO = value;
                LogSystem.Debug("du lieu gui len API update. " + LogUtil.TraceData("updateData", updateData));
                var rs = new BackendAdapter(param).Post<HIS_PRODUCT_INFO>("api/HisProductInfo/Update", ApiConsumers.MosConsumer, updateData, param);
                if (rs != null)
                {
                    this.currentProductInfo = rs;
                    return true;
                }
                return false;
            }

            //create
            HIS_PRODUCT_INFO createData = new HIS_PRODUCT_INFO();
            createData.MEDICINE_TYPE_ID = this.data.MedicineTypeId;
            createData.PRODUCT_INFO = value;
            LogSystem.Debug("du lieu gui len API create. " + LogUtil.TraceData("createData", createData));
            var rsCreate = new BackendAdapter(param).Post<HIS_PRODUCT_INFO>("api/HisProductInfo/Create", ApiConsumers.MosConsumer, createData, param);
            if (rsCreate != null)
            {
                this.currentProductInfo = rsCreate;
                return true;
            }
            return false;
        }

        private FileUploadInfo UploadToStore()
        {
            using (MemoryStream stream = new MemoryStream(this.selectedFileContent))
            {
                //keepOriginalFile = true de FSS giu dung ten minh gui len, nho do lan sua sau ghi de len chinh tep cu.
                return Inventec.Fss.Client.FileUpload.UploadFile(
                    GlobalVariables.APPLICATION_CODE,
                    FSS_STORE_LOCATION,
                    stream,
                    ResolveUploadFileName(),
                    true);
            }
        }

        /// <summary>
        /// Sua ban ghi da co va van giu nguyen dinh dang thi dung lai dung ten tep cu de ghi de,
        /// tranh de lai tep rac tren FSS. Truong hop con lai sinh ten moi theo phan mo rong tep vua chon.
        /// </summary>
        private string ResolveUploadFileName()
        {
            string extension = GetExtension(this.selectedFileName);
            try
            {
                if (this.currentProductInfo != null && !IsLegacyRichTextContent(this.currentProductInfo.PRODUCT_INFO))
                {
                    string existing = this.currentProductInfo.PRODUCT_INFO;
                    if (!string.IsNullOrWhiteSpace(existing))
                    {
                        string leaf = existing.Split('\\', '/').LastOrDefault();
                        if (!string.IsNullOrWhiteSpace(leaf) && EndsWith(leaf, extension))
                        {
                            return leaf;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return Guid.NewGuid().ToString("N") + Inventec.Common.DateTime.Get.Now() + extension;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.currentProductInfo == null) return;
                if (MessageBox.Show(this, "Bạn có chắc chắn muốn xóa dữ liệu?", "Thông báo", MessageBoxButtons.OKCancel) != DialogResult.OK) return;

                CommonParam param = new CommonParam();
                string deletedValue = this.currentProductInfo.PRODUCT_INFO;
                LogSystem.Debug("Goi den api xoa du lieu: ID = " + this.currentProductInfo.ID);

                bool rs = false;
                WaitingManager.Show();
                try
                {
                    rs = new BackendAdapter(param).Post<bool>("api/HisProductInfo/Delete", ApiConsumers.MosConsumer, this.currentProductInfo.ID, param);
                }
                finally
                {
                    WaitingManager.Hide();
                }

                MessageManager.Show(this, param, rs);
                if (rs)
                {
                    DeleteFromStore(deletedValue);
                    this.currentProductInfo = null;
                    this.selectedFileContent = null;
                    this.selectedFileName = "";
                    ClearViewer();
                    if (AllowEdit) EnterTextMode("");
                    EnableControlChange();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xoa tep tren FSS sau khi da xoa ban ghi thanh cong. Loi o buoc nay chi ghi log, khong chan nguoi dung.
        /// </summary>
        private void DeleteFromStore(string fileUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileUrl) || IsLegacyRichTextContent(fileUrl)) return;
                bool isDeleted = Inventec.Fss.Client.FileDelete.DeleteFile(GlobalVariables.APPLICATION_CODE, fileUrl);
                LogSystem.Debug("Delete file " + fileUrl + " ___ RESULT: " + isDeleted);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ClearViewer()
        {
            //Tach lam nhieu buoc: loi o mot o xem khong duoc chan viec don o xem con lai va giai phong stream.
            try
            {
                this.pdfViewer1.CloseDocument();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            try
            {
                this.richEditContent.RtfText = "";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            try
            {
                if (this.currentDocumentStream != null)
                {
                    this.currentDocumentStream.Dispose();
                    this.currentDocumentStream = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            try
            {
                this.pdfViewer1.Visible = false;
                this.richEditContent.Visible = false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// FormBase goi hook nay trong OnFormClosing va goi TRUOC this.Dispose(true) (FormBase.cs:286 va :291),
        /// nen day la cho duy nhat con dong duoc tai lieu va giai phong stream khi cac o xem chua bi huy.
        /// Dat o OnFormClosed thi control da bi dispose, CloseDocument nem loi va stream khong bao gio duoc giai phong.
        /// </summary>
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                ClearViewer();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            base.ProcessDisposeModuleDataAfterClose();
        }

        private static bool IsSupportedName(string name)
        {
            return IsWordName(name) || EndsWith(name, ".pdf");
        }

        private static bool IsWordName(string name)
        {
            return EndsWith(name, ".doc") || EndsWith(name, ".docx") || EndsWith(name, ".rtf");
        }

        private static bool EndsWith(string value, string suffix)
        {
            return !string.IsNullOrEmpty(value) && value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetExtension(string fileName)
        {
            try
            {
                string extension = Path.GetExtension(fileName);
                if (!string.IsNullOrWhiteSpace(extension)) return extension.ToLowerInvariant();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return ".pdf";
        }

        /// <summary>
        /// Noi dung go tay duoc luu thang duoi dang chuoi RTF trong cot PRODUCT_INFO,
        /// khong the dem gia tri do di tai tep tren FSS.
        /// </summary>
        private static bool IsLegacyRichTextContent(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            string trimmed = value.TrimStart();
            if (trimmed.StartsWith("{\\rtf", StringComparison.OrdinalIgnoreCase)) return true;
            //Duong dan tep khong bao gio dai nhu vay va khong chua xuong dong.
            return value.Length > 500 || value.IndexOf('\n') >= 0 || value.IndexOf('\r') >= 0;
        }
    }
}
