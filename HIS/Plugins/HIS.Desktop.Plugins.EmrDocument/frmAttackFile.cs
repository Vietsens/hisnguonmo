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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.TDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.EmrDocument.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Runtime.InteropServices;
using WIA;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.SignLibrary.DTO;
using System.Drawing.Drawing2D;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.EmrDocument
{
    public partial class frmAttackFile : Form
    {
        #region Reclare

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;


        V_EMR_DOCUMENT curentDocument;
        DelegateSelectData dlgGetImageFromModuleCamera;
        DelegateReturnMutilObject lstdlgGetImageFromModuleCamera;
        long _TreatmentId = 0;
        string loginName = null;
        string[] fullfileNameAttack;
        AttackADO fileNameAttack;
        AttackADO currentFileAttack;
        List<AttackADO> ListfileNameAttack = new List<AttackADO>();
        Action actRefesh;

        /// <summary>
        /// Độ phân giải và chất lượng JPEG khi làm phẳng file pdf đính kèm.
        /// Đặt bằng đúng độ phân giải máy scan của chính màn hình này (xem AdjustScannerSettings).
        /// Thư viện tự làm là 300 DPI/chất lượng 100, cho ra file to gấp gần 7 lần mà in ra không khác gì.
        /// Muốn nét hơn thì nâng lên 200/85, file sẽ to gấp khoảng 1,7 lần mức này.
        /// </summary>
        private const int ATTACH_PDF_RENDER_DPI = 150;
        private const int ATTACH_PDF_RENDER_QUALITY = 80;

        private static bool asposeLicenseChecked = false;

        private List<string> _tempFilesToDelete = new List<string>();
        private PdfDocument _doc;
        private const string formatJpeg = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
        internal string _deviceId;
        private string _lastItem;
        #endregion

        #region Construct
        public frmAttackFile(long treatmentId, string loginName, Action _actRefesh)
            : this(null, treatmentId, loginName, _actRefesh)
        {
        }

        public frmAttackFile(V_EMR_DOCUMENT document, long treatmentId, string loginName, Action _actRefesh)
        {
            InitializeComponent();
            this.curentDocument = document;
            this._TreatmentId = treatmentId;
            this.loginName = loginName;
            this.actRefesh = _actRefesh;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private Method
        private void frmAttackFile_Load(object sender, EventArgs e)
        {
            try
            {
                Config.ConfigKey.GetConfigKey();
                //cho phép đọc file pdf chỉ đặt mật khẩu chủ sở hữu (chặn sửa/in), nếu không PdfReader ném lỗi
                //và cả lần đính kèm bị hỏng dù người dùng vẫn mở xem được file đó
                iTextSharp.text.pdf.PdfReader.unethicalreading = true;
                this.SetCaptionByLanguageKey();
                this.imageview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.pdfview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                InitComboDocumentType();

                InitControlState();
                txtDocumentName.Focus();
                txtDocumentName.SelectAll();
                LoadCboTextGroup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrintDupicate.Name)
                        {
                            chkPrintDupicate.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCboTextGroup()
        {
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;
                EmrDocumentGroupFilter filter = new EmrDocumentGroupFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.IS_LEAF = true;
                var datas = new BackendAdapter(new CommonParam()).Get<List<EMR_DOCUMENT_GROUP>>("api/EmrDocumentGroup/Get", ApiConsumers.EmrConsumer, filter, null);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.CboDocumentGroup, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        //private string GeneratePdfFileFromImage()
        //{
        //    string output = Path.GetTempFileName();
        //    try
        //    {
        //        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(this.pteAnhChupFileDinhKem.Image, BaseColor.BLACK);
        //        using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
        //        {
        //            using (Document doc = new Document(image))
        //            {
        //                using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
        //                {
        //                    doc.Open();
        //                    image.SetAbsolutePosition(0, 0);
        //                    writer.DirectContent.AddImage(image);
        //                    doc.Close();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return output;
        //}

        /// <summary>
        /// nối nhiếu file ảnh thành 1 file pdf
        /// </summary>
        /// <returns></returns>
        private string GeneratePdfFileFromImages()
        {
            string output = Path.GetTempFileName();
            try
            {

                using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (Document doc = new Document())
                    {
                        try
                        {
                            PdfWriter.GetInstance(doc, fs);

                            doc.Open();
                            foreach (var item in this.ListfileNameAttack)
                            {
                                string extensionc = System.IO.Path.GetExtension(item.FullName);
                                if ((extensionc ?? "").ToLower() != ".pdf")
                                {
                                    iTextSharp.text.Image image;
                                    image = iTextSharp.text.Image.GetInstance(item.image, BaseColor.BLACK);
                                    if (image.Height > image.Width)
                                    {
                                        float percentage = 0.0f;
                                        percentage = doc.PageSize.Height / image.Height;
                                        image.ScalePercent(percentage * 90);
                                    }
                                    else
                                    {
                                        float percentage = 0.0f;
                                        percentage = doc.PageSize.Width / image.Width;
                                        image.ScalePercent(percentage * 90);
                                    }
                                    doc.NewPage();
                                    doc.Add(image);
                                }
                                else
                                {
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                        finally
                        {
                            doc.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return output;
        }
        /// <summary>
        /// Ghép toàn bộ file đính kèm (pdf + ảnh) thành 1 file pdf để gửi lên server.
        /// Mỗi dòng được xử lý độc lập: 1 file lỗi chỉ bị bỏ qua chứ không làm hỏng cả lần lưu.
        /// </summary>
        /// <param name="fileFails">Tên các file không ghép được</param>
        /// <param name="mergedCount">Số file đã ghép thành công</param>
        /// <returns>Đường dẫn file pdf kết quả, null nếu không ghép được file nào</returns>
        private string CombineMultiplePDFs(out List<string> fileFails, out int mergedCount)
        {
            fileFails = new List<string>();
            mergedCount = 0;

            //Bước 1: quy mọi dòng đính kèm về danh sách file pdf trung gian
            List<KeyValuePair<string, string>> pdfParts = new List<KeyValuePair<string, string>>();
            List<string> tempParts = new List<string>();
            foreach (var item in this.ListfileNameAttack)
            {
                if (item == null) continue;
                string displayName = !String.IsNullOrEmpty(item.FILE_NAME) ? item.FILE_NAME : item.FullName;
                try
                {
                    if (!String.IsNullOrEmpty(item.FullName)
                        && (System.IO.Path.GetExtension(item.FullName) ?? "").ToLower() == ".pdf"
                        && File.Exists(item.FullName))
                    {
                        pdfParts.Add(new KeyValuePair<string, string>(displayName, item.FullName));
                    }
                    else if (item.image != null)
                    {
                        string imagePdf = CreatePdfFromImage(item.image);
                        pdfParts.Add(new KeyValuePair<string, string>(displayName, imagePdf));
                        tempParts.Add(imagePdf);
                    }
                    else
                    {
                        //dòng không có cả file pdf lẫn ảnh, thường do convert file pdf nguồn thất bại
                        fileFails.Add(displayName);
                        Inventec.Common.Logging.LogSystem.Warn("File dinh kem khong co du lieu de ghep____FILE_NAME:" + item.FILE_NAME + "____FullName:" + item.FullName);
                    }
                }
                catch (Exception ex)
                {
                    fileFails.Add(displayName);
                    Inventec.Common.Logging.LogSystem.Warn("Khong chuyen duoc file dinh kem sang pdf____" + displayName, ex);
                }
            }

            if (pdfParts.Count == 0) return null;

            //Bước 2: nối các file pdf trung gian lại thành 1 file
            string outFile = Path.GetTempFileName();
            Document document = new Document();
            FileStream outStream = null;
            try
            {
                outStream = new FileStream(outFile, FileMode.Create, FileAccess.Write, FileShare.None);
                PdfCopy writer = new PdfCopy(document, outStream);
                document.Open();

                foreach (var part in pdfParts)
                {
                    try
                    {
                        AppendPdfToWriter(writer, document, part.Value);
                        mergedCount++;
                    }
                    catch (Exception ex)
                    {
                        fileFails.Add(part.Key);
                        Inventec.Common.Logging.LogSystem.Warn("Khong noi duoc file vao ban ghep____" + part.Key + "____" + part.Value, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                mergedCount = 0;
            }
            finally
            {
                //document.Close() ghi phần cuối file pdf, chỉ gọi được khi đã có ít nhất 1 trang
                try
                {
                    if (mergedCount > 0) document.Close();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    mergedCount = 0;
                }
                try
                {
                    if (outStream != null) outStream.Dispose();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                foreach (var tempPart in tempParts)
                {
                    try
                    {
                        if (File.Exists(tempPart)) File.Delete(tempPart);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
            }

            if (mergedCount == 0)
            {
                try
                {
                    if (File.Exists(outFile)) File.Delete(outFile);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return null;
            }
            return outFile;
        }

        /// <summary>
        /// Nối toàn bộ trang của 1 file pdf vào bản ghép đang mở.
        /// </summary>
        private void AppendPdfToWriter(PdfCopy writer, Document document, string pdfFilePath)
        {
            PdfReader reader = new PdfReader(pdfFilePath);
            try
            {
                reader.ConsolidateNamedDestinations();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    PdfImportedPage page = writer.GetImportedPage(reader, i);
                    document.NewPage();
                    writer.NewPage();
                    writer.AddPage(page);
                }

                PRAcroForm form = reader.AcroForm;
                if (form != null)
                {
                    try
                    {
                        writer.CopyDocumentFields(reader);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Tạo 1 file pdf tạm chứa đúng 1 ảnh, co theo khổ giấy.
        /// </summary>
        private string CreatePdfFromImage(System.Drawing.Image source)
        {
            string outputImagePdf = Path.GetTempFileName();
            try
            {
                Document imageDocument = new Document();
                using (FileStream imageStream = new FileStream(outputImagePdf, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    PdfWriter.GetInstance(imageDocument, imageStream);
                    imageDocument.Open();

                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(source, BaseColor.BLACK);
                    float percentage = image.Height > image.Width
                        ? imageDocument.PageSize.Height / image.Height
                        : imageDocument.PageSize.Width / image.Width;
                    image.ScalePercent(percentage * 90);

                    imageDocument.NewPage();
                    imageDocument.Add(image);
                    imageDocument.Close();
                }
            }
            catch
            {
                try
                {
                    if (File.Exists(outputImagePdf)) File.Delete(outputImagePdf);
                }
                catch { }
                throw;
            }
            return outputImagePdf;
        }
        private string GetBase64FileData(string outFile)
        {
            string b64Data = "";
            try
            {
                MemoryStream streamData = new MemoryStream();
                if (!String.IsNullOrEmpty(outFile))
                {
                    using (FileStream file = new FileStream(outFile, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        streamData.Write(bytes, 0, (int)file.Length);
                    }
                }

                streamData.Position = 0;
                b64Data = Convert.ToBase64String(streamData.ToArray());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return b64Data;
        }

        private MemoryStream GetMemoryStreamFileData(string outFile)
        {
            MemoryStream streamData = null;
            try
            {
                if (!String.IsNullOrEmpty(outFile))
                {
                    //ReadAllBytes đọc đủ file trong mọi trường hợp; cách cũ dùng file.Read(...) một lần
                    //và bỏ qua số byte thực đọc được, thiếu byte nào là phần đuôi thành số 0 -> pdf hỏng
                    byte[] bytes = File.ReadAllBytes(outFile);
                    streamData = new MemoryStream(bytes, 0, bytes.Length, false, true);
                    streamData.Position = 0;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                streamData = null;
            }
            return streamData;
        }

        private void InitComboDocumentType()
        {
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_CODE", "", 80, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_TYPE_NAME", "ID", columnInfos, false, 230);
                ControlEditorLoader.Load(cboDocumentType, GetDocumentType(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE> GetDocumentType()
        {
            List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE> result = new List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE>();
            try
            {
                CommonParam param = new CommonParam();
                EMR.Filter.EmrDocumentTypeFilter filter = new EMR.Filter.EmrDocumentTypeFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_FIELD = "DOCUMENT_TYPE_CODE";
                filter.ORDER_DIRECTION = "ASC";

                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE>>("api/EmrDocumentType/Get", ApiConsumers.EmrConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private EMR_TREATMENT GetTreatmentById(long treatmentId)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                EmrTreatmentFilter filter = new EmrTreatmentFilter();
                filter.ID = treatmentId;
                return new BackendAdapter(paramCommon).Get<List<EMR_TREATMENT>>(EmrRequestUriStore.EMR_TREATMENT_GET, ApiConsumers.EmrConsumer, filter, paramCommon).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private void FillImageFromModuleCamereToUC(object dataImage)
        {
            try
            {
                if (dataImage != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("dataImage: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataImage), dataImage));
                    pteAnhChupFileDinhKem.Image = (System.Drawing.Image)dataImage;
                    var check = this.ListfileNameAttack.OrderByDescending(o => o.Dem).FirstOrDefault();

                    Inventec.Common.Logging.LogSystem.Info("dem max: " + check);
                    int dem = 0;
                    if (check == null || check.Dem == 0)
                    {
                        dem = 1;
                    }
                    else
                    {
                        dem = check.Dem + 1;
                    }
                    fileNameAttack = new AttackADO();
                    this.fileNameAttack.FILE_NAME = "Ảnh chụp " + dem.ToString() + ".jpg";
                    this.fileNameAttack.FullName = "Ảnh chụp " + dem.ToString() + ".jpg";
                    this.fileNameAttack.image = (System.Drawing.Image)dataImage;
                    this.fileNameAttack.Dem = dem;

                    pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    this.ListfileNameAttack.Add(this.fileNameAttack);
                    Inventec.Common.Logging.LogSystem.Info("dữ liệu ảnh chụp: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListfileNameAttack), this.ListfileNameAttack));

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = this.ListfileNameAttack.ToList();
                    gridView2.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOpenFileInComputer_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Multiselect = true;
                openFile.Filter = "Ảnh(*.jpg, *.Png, *.jpeg, *.bmp,*.gif,*.pdf)|*.jpg;*.png;*.jpeg;*.bmp;*.gif;*.pdf";
                openFile.DefaultExt = ".jpg;.png;.jpeg;.bmp;.gif;.pdf";

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    this.fullfileNameAttack = openFile.FileNames;

                    List<string> fileFails = new List<string>();
                    if (this.fullfileNameAttack != null)
                    {
                        foreach (var item in this.fullfileNameAttack)
                        {
                            //Mỗi file đọc độc lập, 1 file lỗi không được làm dừng vòng lặp
                            //khiến những file chọn sau đó bị mất
                            try
                            {
                                AttackADO attackADO = LoadAttackFile(item);
                                if (attackADO == null)
                                {
                                    fileFails.Add(System.IO.Path.GetFileName(item));
                                    continue;
                                }
                                this.fileNameAttack = attackADO;
                                this.ListfileNameAttack.Add(attackADO);
                            }
                            catch (Exception exItem)
                            {
                                fileFails.Add(System.IO.Path.GetFileName(item));
                                Inventec.Common.Logging.LogSystem.Warn("Khong doc duoc file dinh kem____" + item, exItem);
                            }
                        }
                    }

                    if (fileFails.Count > 0)
                    {
                        MessageBox.Show(this, "Không đọc được các file sau, vui lòng kiểm tra lại:" + Environment.NewLine
                            + "- " + String.Join(Environment.NewLine + "- ", fileFails),
                            "Đính kèm file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                gridView2.BeginUpdate();
                //phải gán bằng list mới, gán lại chính tham chiếu cũ thì grid bỏ qua và không hiện dòng vừa thêm
                this.gridView2.GridControl.DataSource = this.ListfileNameAttack.ToList();
                gridView2.EndUpdate();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fullfileNameAttack), fullfileNameAttack));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListfileNameAttack), ListfileNameAttack));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đọc 1 file người dùng chọn từ máy tính thành 1 dòng đính kèm.
        /// </summary>
        /// <returns>null nếu file không dùng được</returns>
        private AttackADO LoadAttackFile(string filePath)
        {
            if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Inventec.Common.Logging.LogSystem.Warn("File dinh kem khong ton tai____" + filePath);
                return null;
            }

            AttackADO result = new AttackADO();
            result.FILE_NAME = System.IO.Path.GetFileName(filePath);
            result.EXTENSION = (System.IO.Path.GetExtension(filePath) ?? "").TrimStart('.');

            if ((System.IO.Path.GetExtension(filePath) ?? "").ToLower() == ".pdf")
            {
                //Làm phẳng file pdf (dựng từng trang thành ảnh rồi ghép lại) để khâu ghép/ký phía sau
                //không vướng các thành phần pdf lạ. Làm phẳng hỏng thì dùng lại chính file gốc.
                string joinPdfPathFile = FlattenPdfFile(filePath);
                if (!String.IsNullOrEmpty(joinPdfPathFile))
                {
                    result.FullName = joinPdfPathFile;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong lam phang duoc file pdf, dung lai chinh file goc____" + filePath);
                    result.FullName = filePath;
                }
                //file pdf không đọc ra ảnh xem trước, khi lưu sẽ được nối trực tiếp bằng PdfReader
            }
            else
            {
                result.FullName = filePath;
                result.image = System.Drawing.Image.FromFile(filePath);
            }

            return result;
        }

        /// <summary>
        /// Làm phẳng 1 file pdf: dựng lại từng trang thành ảnh rồi ghép thành pdf mới.
        /// Vẫn dùng đúng hàm ghép của SignLibrary, chỉ tự dựng ảnh ở độ phân giải vừa phải
        /// thay vì để thư viện dựng ở 300 DPI/chất lượng 100 (file phình lên gấp 7 lần).
        /// </summary>
        /// <returns>Đường dẫn pdf mới, chuỗi rỗng nếu không làm được</returns>
        private string FlattenPdfFile(string filePath)
        {
            //Số trang thật, lấy bằng iTextSharp để đối chiếu - iTextSharp không bị giới hạn bản quyền như Aspose
            int expectedPages = 0;
            try
            {
                PdfReader sourceReader = new PdfReader(filePath);
                try { expectedPages = sourceReader.NumberOfPages; }
                finally { sourceReader.Close(); }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Khong doc duoc so trang file pdf____" + filePath, ex);
            }

            string joinPdfPathFile = "";

            //Cách 1: tự dựng ảnh ở ATTACH_PDF_RENDER_DPI rồi nhờ thư viện ghép lại
            try
            {
                List<ImageOfPageDTO> pageImages = RenderPdfPagesToImages(filePath, expectedPages);
                if (pageImages != null && pageImages.Count > 0)
                {
                    //tham số chiều cao trang thư viện không dùng đến nên truyền 0
                    Inventec.Common.SignLibrary.PdfDocumentProcess.SplitOnePageToImageAndJoinToNewOnePdf(filePath, 0, ref joinPdfPathFile, pageImages);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lam phang file pdf o " + ATTACH_PDF_RENDER_DPI + " DPI that bai____" + filePath, ex);
                joinPdfPathFile = "";
            }

            //Cách 2: để thư viện tự làm hết như trước đây, nếu cách 1 không ra file dùng được
            if (!IsUsablePdfFile(joinPdfPathFile, expectedPages))
            {
                joinPdfPathFile = "";
                try
                {
                    Inventec.Common.SignLibrary.PdfDocumentProcess.SplitOnePageToImageAndJoinToNewOnePdf(filePath, 0, ref joinPdfPathFile);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SplitOnePageToImageAndJoinToNewOnePdf loi____" + filePath, ex);
                    joinPdfPathFile = "";
                }
            }

            LogSystem.Debug("joinPdfPathFile:" + joinPdfPathFile);
            return IsUsablePdfFile(joinPdfPathFile, expectedPages) ? joinPdfPathFile : "";
        }

        /// <summary>
        /// Dựng từng trang pdf thành ảnh JPEG ngay trong bộ nhớ, không sinh file ảnh tạm trên đĩa.
        /// </summary>
        /// <param name="expectedPages">Số trang thật, để phát hiện Aspose bị cắt bớt trang. 0 = không kiểm tra</param>
        /// <returns>null nếu không dựng đủ số trang</returns>
        private List<ImageOfPageDTO> RenderPdfPagesToImages(string filePath, int expectedPages)
        {
            EnsureAsposeLicense();

            List<ImageOfPageDTO> result = new List<ImageOfPageDTO>();
            Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(filePath);
            foreach (Aspose.Pdf.Page page in pdfDocument.Pages)
            {
                using (MemoryStream pageStream = new MemoryStream())
                {
                    Aspose.Pdf.Devices.JpegDevice jpegDevice = new Aspose.Pdf.Devices.JpegDevice(
                        new Aspose.Pdf.Devices.Resolution(ATTACH_PDF_RENDER_DPI), ATTACH_PDF_RENDER_QUALITY);
                    jpegDevice.Process(page, pageStream);

                    ImageOfPageDTO pageImage = new ImageOfPageDTO();
                    pageImage.ImageContent = pageStream.ToArray();
                    pageImage.PageNumber = page.Number;
                    //giữ đúng khổ trang gốc, chỉ mật độ điểm ảnh là thấp hơn
                    pageImage.Width = (float)page.Rect.Width;
                    pageImage.Height = (float)page.Rect.Height;
                    result.Add(pageImage);
                }
            }

            if (expectedPages > 0 && result.Count != expectedPages)
            {
                //Aspose chạy chế độ dùng thử chỉ xử lý 4 trang đầu, dựng thiếu trang là mất dữ liệu
                Inventec.Common.Logging.LogSystem.Warn(String.Format(
                    "Dung anh thieu trang, bo cach tu dung anh____{0}____dung duoc {1}/{2} trang", filePath, result.Count, expectedPages));
                return null;
            }
            return result;
        }

        /// <summary>
        /// Đặt license cho Aspose. SignLibrary có sẵn license nhưng hàm đặt là internal nên phải gọi qua reflection.
        /// Không đặt được thì Aspose chạy chế độ dùng thử: đóng dấu chìm và chỉ xử lý 4 trang đầu.
        /// </summary>
        private static void EnsureAsposeLicense()
        {
            if (asposeLicenseChecked) return;
            asposeLicenseChecked = true;
            try
            {
                System.Reflection.Assembly signLibrary = typeof(Inventec.Common.SignLibrary.PdfDocumentProcess).Assembly;
                Type licenceProcess = signLibrary.GetType("Inventec.Common.SignLibrary.License.LicenceProcess");
                System.Reflection.MethodInfo setLicense = licenceProcess != null
                    ? licenceProcess.GetMethod("SetLicenseForAspose", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)
                    : null;
                if (setLicense == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong tim thay LicenceProcess.SetLicenseForAspose");
                    return;
                }
                setLicense.Invoke(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// File pdf có mở được và đủ số trang mong đợi hay không.
        /// </summary>
        /// <param name="expectedPages">0 = chỉ cần mở được</param>
        private bool IsUsablePdfFile(string filePath, int expectedPages)
        {
            try
            {
                if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath) || new FileInfo(filePath).Length == 0)
                    return false;

                PdfReader reader = new PdfReader(filePath);
                try
                {
                    if (reader.NumberOfPages <= 0) return false;
                    if (expectedPages > 0 && reader.NumberOfPages != expectedPages)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(String.Format(
                            "File pdf sinh ra thieu trang____{0}____co {1}/{2} trang", filePath, reader.NumberOfPages, expectedPages));
                        return false;
                    }
                    return true;
                }
                finally { reader.Close(); }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        private void btnOpenCamera_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Camera").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Camera");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    this.dlgGetImageFromModuleCamera = this.FillImageFromModuleCamereToUC;
                    listArgs.Add(this.dlgGetImageFromModuleCamera);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAttackFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;
                //if (pteAnhChupFileDinhKem.Image != null) 
                if (this.ListfileNameAttack != null && this.ListfileNameAttack.Count > 0)
                {


                    DocumentTDO docCreate = new DocumentTDO();
                    docCreate.DocumentName = txtDocumentName.Text;
                    docCreate.DocumentTypeId = cboDocumentType.EditValue != null ? (long?)cboDocumentType.EditValue : null;
                    docCreate.DocumentGroupId = CboDocumentGroup.EditValue != null ? (long?)CboDocumentGroup.EditValue : null;
                    if (this.curentDocument != null)
                    {
                        docCreate.TreatmentCode = this.curentDocument.TREATMENT_CODE;
                    }
                    else
                    {
                        EMR_TREATMENT treatemnt = GetTreatmentById(this._TreatmentId);
                        docCreate.TreatmentCode = treatemnt != null ? treatemnt.TREATMENT_CODE : "";
                    }
                    docCreate.HisCode = "";//TODO
                    docCreate.IsCapture = true;
                    //string output = GeneratePdfFileFromImage();

                    //docCreate.OriginalVersion = new VersionTDO();
                    //docCreate.OriginalVersion.Base64Data = GetBase64FileData(output);

                    List<string> fileFails;
                    int mergedCount;
                    string output = CombineMultiplePDFs(out fileFails, out mergedCount);
                    if (String.IsNullOrEmpty(output) || mergedCount == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Ghep file dinh kem that bai____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListfileNameAttack), this.ListfileNameAttack));
                        MessageBox.Show(this, "Không tạo được file đính kèm từ danh sách đã chọn, vui lòng kiểm tra lại các file.",
                            "Đính kèm file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (fileFails.Count > 0)
                    {
                        //báo rõ file nào bị bỏ qua thay vì lưu thiếu mà người dùng không biết
                        if (MessageBox.Show(this, "Các file sau không ghép được và sẽ bị bỏ qua:" + Environment.NewLine
                            + "- " + String.Join(Environment.NewLine + "- ", fileFails) + Environment.NewLine + Environment.NewLine
                            + "Bạn có muốn tiếp tục lưu không?",
                            "Đính kèm file", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        {
                            try
                            {
                                if (File.Exists(output)) File.Delete(output);
                            }
                            catch { }
                            return;
                        }
                    }

                    //log kích thước để soi được trường hợp server từ chối vì file quá lớn
                    //(mỗi trang pdf sau khi rasterize 300 DPI phình lên khoảng 1-2 MB)
                    long outputLength = 0;
                    try { outputLength = new FileInfo(output).Length; }
                    catch (Exception exLen) { Inventec.Common.Logging.LogSystem.Warn(exLen); }
                    Inventec.Common.Logging.LogSystem.Info(String.Format(
                        "Dinh kem: so file={0}, so file da ghep={1}, so file bo qua={2}, kich thuoc file gui len={3} bytes ({4:0.00} MB)",
                        this.ListfileNameAttack.Count, mergedCount, fileFails.Count, outputLength, outputLength / 1024.0 / 1024.0));

                    Inventec.Core.FileHolder file = new Inventec.Core.FileHolder();
                    file.FileName = output;
                    file.Content = GetMemoryStreamFileData(output);
                    if (file.Content == null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Khong doc duoc noi dung file da ghep____" + output);
                        MessageBox.Show(this, "Không đọc được nội dung file đính kèm vừa tạo, vui lòng thử lại.",
                            "Đính kèm file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    //TODO

                    if (docCreate != null)
                    {
                        CommonParam param = new CommonParam();
                        var resultData = new BackendAdapter(param).PostWithFile<DocumentTDO>(EMR.URI.EmrDocument.CREATE_WITH_FILE, ApiConsumers.EmrConsumer, docCreate, new List<Inventec.Core.FileHolder>() { file }, param);
                        //var resultData = new BackendAdapter(param).PostWithFile<DocumentTDO>(EMR.URI.EmrDocument.CREATE_WITH_FILE, ApiConsumers.EmrConsumer, docCreate, files, param);

                        Inventec.Common.Logging.LogSystem.Debug("Goi api tao van ban " + (resultData != null ? "thanh cong" : "that bai") + "____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => docCreate), docCreate) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => output), output) + "____Ket qua tra ve:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        MessageManager.Show(this, param, resultData != null);
                        if (resultData != null)
                        {
                            if (this.actRefesh != null) this.actRefesh();
                            try
                            {
                                if (File.Exists(output))
                                {
                                    File.Delete(output);
                                }
                            }
                            catch { }
                            this.Close();
                        }
                        else
                        {
                            //Inventec.Common.Logging.LogSystem.Debug("Goi api tao van ban that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => docCreate), docCreate) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => output), output) + "____Ket qua tra ve:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn file ảnh từ máy tính hoặc chụp ảnh từ camera");
                }

                this.fileNameAttack = null;
                this.fullfileNameAttack = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //trước đây lỗi chỉ ghi log, người dùng bấm lưu mà không thấy phản hồi gì
                MessageBox.Show(this, "Lưu file đính kèm thất bại, vui lòng kiểm tra lại các file đã chọn.",
                    "Đính kèm file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pteAnhChupFileDinhKem_ImageChanged(object sender, EventArgs e)
        {
            try
            {
                var rowData = (AttackADO)gridView2.GetFocusedRow();
                if (rowData != null)
                {

                    PictureEdit pedit = sender as PictureEdit;
                    string imageLocal = pedit.GetLoadedImageLocation();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => imageLocal), imageLocal));
                    if (!String.IsNullOrEmpty(imageLocal))
                    {
                        int lIndex = imageLocal.LastIndexOf("\\");
                        //this.fileNameAttack = imageLocal.Substring(lIndex > 0 ? lIndex + 1 : lIndex);
                        rowData.FILE_NAME = imageLocal.Substring(lIndex > 0 ? lIndex + 1 : lIndex);
                    }
                    else if (String.IsNullOrEmpty(rowData.FILE_NAME) && !String.IsNullOrEmpty(rowData.FullName))
                    {
                        //chỉ điền khi chưa có tên, không ghi đè tên file gốc bằng tên file pdf tạm
                        rowData.FILE_NAME = System.IO.Path.GetFileName(rowData.FullName);
                    }
                    //txtDocumentName.Text = this.fileNameAttack;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDocumentType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboDocumentType.Properties.Buttons[1].Visible = cboDocumentType.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboDocumentType.EditValue != null)
                    {
                        txtDocumentName.Focus();
                        txtDocumentName.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDocumentType.ClosePopup();
                    if (cboDocumentType.EditValue != null)
                    {
                        txtDocumentName.Focus();
                        txtDocumentName.SelectAll();
                    }
                }
                else
                    cboDocumentType.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDocumentName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboDocumentGroup.Focus();
                    CboDocumentGroup.ShowPopup();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                AttackADO data = (AttackADO)gridView2.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.ListfileNameAttack.Remove(data);

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = (this.ListfileNameAttack != null ? this.ListfileNameAttack.ToList() : null);
                    gridView2.EndUpdate();

                    pteAnhChupFileDinhKem.Image = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    AttackADO AttackTDO = (AttackADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (AttackTDO != null)
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
                LogSystem.Warn(ex);
            }
        }

        private void gridView2_Click(object sender, EventArgs e)
        {
            try
            {
                currentFileAttack = (AttackADO)gridView2.GetFocusedRow();
                if (currentFileAttack != null)
                {
                    if ((System.IO.Path.GetExtension(currentFileAttack.FullName) ?? "").ToLower() == ".pdf")
                    {
                        this.imageview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.imageview2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.pdfview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                        pdfViewer1.LoadDocument(currentFileAttack.FullName);

                        btnRotateLeft.Enabled = false;
                        btnRotateRight.Enabled = false;
                    }
                    else
                    {
                        this.imageview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.imageview2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.pdfview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        pteAnhChupFileDinhKem.Image = currentFileAttack.image;
                        pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                        btnRotateLeft.Enabled = true;
                        btnRotateRight.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void CboDocumentGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAttackFile.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboDocumentGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (CboDocumentGroup.EditValue != null)
                    {
                        btnAttackFile.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void CboDocumentGroup_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CboDocumentGroup.Properties.Buttons[1].Visible = CboDocumentGroup.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboDocumentGroup_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboDocumentGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAttackFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAttackFile_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            try
            {
                ShowScan();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class WIA_DPS_DOCUMENT_HANDLING_SELECT
        {
            public const uint FEEDER = 0x00000001;
            public const uint FLATBED = 0x00000002;
            public const uint DUPLEX = 0x004;
        }
        class WIA_DPS_DOCUMENT_HANDLING_STATUS
        {
            public const uint FEED_READY = 0x00000001;
        }
        class WIA_PROPERTIES
        {
            public const uint WIA_RESERVED_FOR_NEW_PROPS = 1024;
            public const uint WIA_DIP_FIRST = 2;
            public const uint WIA_DPA_FIRST = WIA_DIP_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPC_FIRST = WIA_DPA_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            //
            // Scanner only device properties (DPS)
            //
            public const uint WIA_DPS_FIRST = WIA_DPC_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPS_DOCUMENT_HANDLING_STATUS = WIA_DPS_FIRST + 13;
            public const uint WIA_DPS_DOCUMENT_HANDLING_SELECT = WIA_DPS_FIRST + 14;
        }

        public static List<StreamToPdfADO> ScanDuplex(WIA.Device device)
        {
            try
            {
                List<StreamToPdfADO> ret = new List<StreamToPdfADO>();
                device.Properties["3088"].set_Value(5);
                //SetDeviceProperty(ref device, 3096,1);
                WIA.Item items = device.Items[1];
                //items.Properties["6146"].set_Value(2);
                // A4 at 150 DPI is ~1241 x 1754 px. Using 1700px height can cut off the bottom of the page.
                AdjustScannerSettings(items, 150, 0, 0, 1250, 1754, 0, 0, 1);

                ICommonDialog dlg = new WIA.CommonDialog();
                while (true)
                {
                    try
                    {
                        WIA.ImageFile image = (WIA.ImageFile)dlg.ShowTransfer(items);
                        if (image != null && image.FileData != null)
                        {
                            StreamToPdfADO ado = new StreamToPdfADO();
                            string fileName = Path.GetTempFileName();
                            File.Delete(fileName);
                            image.SaveFile(fileName);
                            ado.Url = fileName;
                            ret.Add(ado);
                        }
                    }
                    catch
                    {
                        break;
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("Exception from HRESULT: 0x80210067"))
                {
                    MessageBox.Show("Máy scan có thể không hỗ trợ in 2 mặt, vui lòng kiểm tra lại.");
                }
                return null;
            }
        }
        public static List<StreamToPdfADO> Scan(WIA.Device device)
        {
            List<StreamToPdfADO> ret = new List<StreamToPdfADO>();
            try
            {

                var scannerItem = device.Items[1];
                // A4 at 150 DPI is ~1241 x 1754 px. Using 1700px height can cut off the bottom of the page.
                AdjustScannerSettings(scannerItem, 150, 0, 0, 1250, 1754, 0, 0, 1);

                ICommonDialog dlg = new WIA.CommonDialog();

                try
                {
                    WIA.ImageFile imageFile = (WIA.ImageFile)dlg.ShowTransfer(scannerItem, formatJpeg, false);
                    if (imageFile != null && imageFile.FileData != null)
                    {
                        StreamToPdfADO ado = new StreamToPdfADO();
                        string fileName = Path.GetTempFileName();
                        File.Delete(fileName);
                        imageFile.SaveFile(fileName);
                        //
                        ado.Url = fileName;
                        ret.Add(ado);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return ret;
        }
        public void ShowScan()
        {
            try
            {
                DeviceManager deviceManager = new DeviceManager();
                DeviceInfo firstScannerAvailable = null;
                if (deviceManager.DeviceInfos.Count == 0)
                {
                    MessageBox.Show("Vui lòng kết nối đến máy Scan với máy tính");
                    return;
                }

                for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
                {
                    if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                        continue;
                    firstScannerAvailable = deviceManager.DeviceInfos[i];
                    break;
                }
                var device = firstScannerAvailable.Connect();

                List<StreamToPdfADO> streams = new List<StreamToPdfADO>();
                if (chkPrintDupicate.Checked)
                {
                    streams = ScanDuplex(device);
                }
                else
                {
                    streams = Scan(device);
                }

                if (streams != null && streams.Count() > 0)
                {
                    var check = this.ListfileNameAttack.OrderByDescending(o => o.Dem).FirstOrDefault();

                    Inventec.Common.Logging.LogSystem.Info("dem max: " + check);
                    int dem = (check == null ? 0 : check.Dem);

                    //máy scan 2 mặt/nạp giấy tự động trả về bao nhiêu trang phải nhận hết bấy nhiêu.
                    //Trước đây chỉ xử lý đúng 1 hoặc 2 trang, quét từ 3 trang trở lên là mất sạch.
                    this.imageview2.Visibility = streams.Count > 1
                        ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                        : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    for (int i = 0; i < streams.Count; i++)
                    {
                        try
                        {
                            dem++;
                            AttackADO scanADO = new AttackADO();
                            scanADO.image = System.Drawing.Image.FromFile(streams[i].Url);
                            scanADO.FILE_NAME = "Ảnh " + dem.ToString() + ".png";
                            scanADO.FullName = streams[i].Url;
                            scanADO.Dem = dem;
                            this.ListfileNameAttack.Add(scanADO);
                            this.fileNameAttack = scanADO;

                            if (i == 0) pteAnhChupFileDinhKem.Image = scanADO.image;
                            else if (i == 1) pteAnhChupFileDinhKem2.Image = scanADO.image;
                        }
                        catch (Exception exItem)
                        {
                            Inventec.Common.Logging.LogSystem.Error("Khong doc duoc anh scan____" + streams[i].Url, exItem);
                        }
                    }

                    pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = this.ListfileNameAttack.ToList();
                    gridView2.EndUpdate();
                }

                // convert image To Pdf
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Bitmap RotateBitmap(Bitmap bm, float angle)
        {

            Matrix rotate_at_origin = new Matrix();
            rotate_at_origin.Rotate(angle);


            PointF[] points =
            {
                new PointF(0, 0),
                new PointF(bm.Width, 0),
                new PointF(bm.Width, bm.Height),
                new PointF(0, bm.Height),
            };
            rotate_at_origin.TransformPoints(points);
            float xmin, xmax, ymin, ymax;
            GetPointBounds(points, out xmin, out xmax, out ymin, out ymax);

            int wid = (int)Math.Round(xmax - xmin);
            int hgt = (int)Math.Round(ymax - ymin);
            Bitmap result = new Bitmap(wid, hgt);

            Matrix rotate_at_center = new Matrix();
            rotate_at_center.RotateAt(angle,
                new PointF(wid / 2f, hgt / 2f));

            using (Graphics gr = Graphics.FromImage(result))
            {
                gr.InterpolationMode = InterpolationMode.High;

                gr.Clear(bm.GetPixel(0, 0));
                gr.Transform = rotate_at_center;

                int x = (wid - bm.Width) / 2;
                int y = (hgt - bm.Height) / 2;
                gr.DrawImage(bm, x, y);
            }

            return result;
        }

        private void GetPointBounds(PointF[] points, out float xmin, out float xmax, out float ymin, out float ymax)
        {
            xmin = points[0].X;
            xmax = xmin;
            ymin = points[0].Y;
            ymax = ymin;
            foreach (PointF point in points)
            {
                if (xmin > point.X) xmin = point.X;
                if (xmax < point.X) xmax = point.X;
                if (ymin > point.Y) ymin = point.Y;
                if (ymax < point.Y) ymax = point.Y;
            }
        }

        public System.Drawing.Image Resize(System.Drawing.Image img, float percentage, bool isRotate)
        {
            //lấy kích thước ban đầu của bức ảnh
            int originalW = img.Width;
            int originalH = img.Height;

            //tính kích thước cho ảnh mới theo tỷ lệ đưa vào
            int resizedW = (int)(originalW * percentage);
            int resizedH = (int)(originalH * percentage);

            //tạo 1 ảnh Bitmap mới theo kích thước trên
            Bitmap bmp = new Bitmap(resizedW, resizedH);
            //tạo 1 graphic mới từ Bitmap
            Graphics graphic = Graphics.FromImage((System.Drawing.Image)bmp);
            //vẽ lại ảnh ban đầu lên bmp theo kích thước mới
            graphic.DrawImage(img, 0, 0, resizedW, resizedH);
            //giải phóng tài nguyên mà graphic đang giữ
            graphic.Dispose();

            // đổi lại chiều của ảnh thứ 2 do máy scan quét từ trên xuống
            if (isRotate) bmp = RotateBitmap(bmp, 180);

            //return the image
            return (System.Drawing.Image)bmp;
        }

        private string MergeImages(System.Drawing.Image image1, System.Drawing.Image image2, string urlToSave, int space)
        {

            Bitmap bitmap = new Bitmap(Math.Max(image1.Width, image2.Width), image1.Height + image2.Height + space);
            bitmap.SetResolution(72, 72);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Bitmap bm1 = (Bitmap)image1;
                Bitmap bm2 = (Bitmap)image2;
                bm1.SetResolution(72, 72); // <-- Set resolution equal to bitmap2
                bm2.SetResolution(72, 72); // <-- Set resolution equal to bitmap2
                g.Clear(Color.White);
                g.DrawImage(bm2, 0, 0);
                g.DrawImage(bm1, 0, image1.Height + space);
            }
            System.Drawing.Image img = bitmap;

            img.Save(urlToSave);
            img.Dispose();
            return urlToSave;
        }

        public static List<string> GetDevices()
        {
            List<string> devices = new List<string>();
            WIA.DeviceManager manager = new WIA.DeviceManager();
            foreach (WIA.DeviceInfo info in manager.DeviceInfos)
            {
                devices.Add(info.DeviceID);
            }
            return devices;
        }
        private static void SetDeviceProperty(ref WIA.Device device, int propertyID, int propertyValue)
        {
            foreach (Property p in device.Properties)
            {
                if (p.PropertyID == propertyID)
                {
                    object value = propertyValue;
                    p.set_Value(ref value);
                    break;
                }
            }
        }

        private static void AdjustScannerSettings(IItem scannnerItem, int scanResolutionDPI, int scanStartLeftPixel, int scanStartTopPixel, int scanWidthPixels, int scanHeightPixels, int brightnessPercents, int contrastPercents, int colorMode)
        {
            const string WIA_SCAN_COLOR_MODE = "6146";
            const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
            const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
            const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
            const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
            const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
            const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";
            const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
            const string WIA_SCAN_CONTRAST_PERCENTS = "6155";
            try
            {
                SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
                SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
                SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
                SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);
                SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, scanWidthPixels);
                SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, scanHeightPixels);
                SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
                SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
                SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
            }
            catch (Exception ex)
            {
                try
                {

                    Inventec.Common.Logging.LogSystem.Error(String.Format("Gắn lại giá trị theo máy scan: \r\n WIA_HORIZONTAL_SCAN_RESOLUTION_DPI {0} \r\n WIA_VERTICAL_SCAN_RESOLUTION_DPI {1} \r\n WIA_HORIZONTAL_SCAN_SIZE_PIXELS {2}\r\n WIA_VERTICAL_SCAN_SIZE_PIXELS {3}____ {4} ", scannnerItem.Properties[WIA_HORIZONTAL_SCAN_RESOLUTION_DPI].get_Value(), scannnerItem.Properties[WIA_VERTICAL_SCAN_RESOLUTION_DPI].get_Value(), scannnerItem.Properties[WIA_HORIZONTAL_SCAN_SIZE_PIXELS].get_Value(), scannnerItem.Properties[WIA_VERTICAL_SCAN_SIZE_PIXELS].get_Value(), ex));
                    SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scannnerItem.Properties[WIA_HORIZONTAL_SCAN_RESOLUTION_DPI].get_Value());
                    SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scannnerItem.Properties[WIA_HORIZONTAL_SCAN_RESOLUTION_DPI].get_Value());
                    SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
                    SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);

                    SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, (int)(scanWidthPixels * ((int)scannnerItem.Properties[WIA_HORIZONTAL_SCAN_RESOLUTION_DPI].get_Value() / scanResolutionDPI)) + 50);
                    SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, (int)(scanHeightPixels * ((int)scannnerItem.Properties[WIA_HORIZONTAL_SCAN_RESOLUTION_DPI].get_Value() / scanResolutionDPI)) + 50);
                    SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
                    SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
                    SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
                }
                catch (Exception exE)
                {
                    Inventec.Common.Logging.LogSystem.Error(exE);
                }

            }
        }

        private static void SetWIAProperty(IProperties properties, object propName, object propValue)
        {
            Property prop = properties.get_Item(ref propName);
            prop.set_Value(ref propValue);
        }

        private void chkPrintDupicate_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == chkPrintDupicate.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintDupicate.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintDupicate.Name;
                    csAddOrUpdate.VALUE = (chkPrintDupicate.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
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
        ///Hàm xét ngôn ngữ cho giao diện frmAttackFile
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmAttackFile = new ResourceManager("HIS.Desktop.Plugins.EmrDocument.Resources.Lang", typeof(frmAttackFile).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.toolTipItem2.Text = Inventec.Common.Resource.Get.Value("toolTipItem2.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.chkPrintDupicate.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAttackFile.chkPrintDupicate.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.bbtnAttackFile.Caption = Inventec.Common.Resource.Get.Value("frmAttackFile.bbtnAttackFile.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.btnScan.ToolTip = Inventec.Common.Resource.Get.Value("frmAttackFile.btnScan.ToolTip", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.CboDocumentGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAttackFile.CboDocumentGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmAttackFile.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmAttackFile.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmAttackFile.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.cboDocumentType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAttackFile.cboDocumentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.btnAttackFile.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.btnAttackFile.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.pteAnhChupFileDinhKem.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAttackFile.pteAnhChupFileDinhKem.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.btnCapture.ToolTip = Inventec.Common.Resource.Get.Value("frmAttackFile.btnCapture.ToolTip", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.lciFortxtDocumentName.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.lciFortxtDocumentName.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        System.Drawing.Image Rotate90(System.Drawing.Image image, bool isLeft)
        {
            System.Drawing.Image rotatedImage = null;
            if (image != null)
            {
                rotatedImage = new Bitmap(image);
                rotatedImage.RotateFlip(isLeft ? RotateFlipType.Rotate270FlipNone : RotateFlipType.Rotate90FlipNone);
            }
            return rotatedImage;
        }

        private void btnRotateLeft_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentFileAttack == null || !btnRotateLeft.Enabled) return;
                currentFileAttack.image = Rotate90(currentFileAttack.image, true);
                pteAnhChupFileDinhKem.Image = currentFileAttack.image;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRotateRight_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentFileAttack == null || !btnRotateRight.Enabled) return;
                currentFileAttack.image = Rotate90(currentFileAttack.image, false);
                pteAnhChupFileDinhKem.Image = currentFileAttack.image;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
