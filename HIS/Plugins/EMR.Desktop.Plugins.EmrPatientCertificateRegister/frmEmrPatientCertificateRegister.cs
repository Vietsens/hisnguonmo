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
using AutoMapper;
using DevExpress.XtraEditors;
using DevExpress.XtraExport.Helpers;
using EMR.Desktop.Plugins.EmrPatientCertificateRegister.ADO;
using EMR.Desktop.Plugins.EmrPatientCertificateRegister.DTO;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.EmrConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMR.Desktop.Plugins.EmrPatientCertificateRegister
{
    public partial class frmEmrPatientCertificateRegister : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module ModuleData;
        private string LoginName;
        DelegateSelectData dlgGetImageFromModuleCamera;
        AttackADO fileNameAttack;
        List<AttackADO> ListfileNameAttack = new List<AttackADO>();
        ImageADO currentImageADO;
        public CccdData currentCCCDInfo { get; set; }
        public frmEmrPatientCertificateRegister()
        {
            InitializeComponent();
        }

        public frmEmrPatientCertificateRegister(Inventec.Desktop.Common.Modules.Module module, long documentId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.ModuleData = module;
                this.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim();
                this.Text = module.text;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmEmrPatientCertificateRegister_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDefaultImage();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool isDefaultImageLoaded = false;

        private void LoadDefaultImage()
        {
            try
            {
                string defaultPath = GetPathDefault();
                if (System.IO.File.Exists(defaultPath))
                {
                    using (var img = Image.FromFile(defaultPath))
                    {

                        picCCCD.Image = new Bitmap(img);
                        picSignPatient.Image = new Bitmap(img);

                        isDefaultImageLoaded = true;
                        Inventec.Common.Logging.LogSystem.Info("LoadDefaultImage: set isDefaultImageLoaded = true");
                    }
                }
                else
                {
                    picCCCD.Image = null;
                    picSignPatient.Image = null;
                    isDefaultImageLoaded = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetPathDefault()
        {
            string imageDefaultPath = string.Empty;
            try
            {
                string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                imageDefaultPath = System.IO.Path.Combine(localPath, "Img", "ImageStorage", "notImage.jpg");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return imageDefaultPath;
        }
        bool IsProcessOpen(string name)
        {
            try
            {
                foreach (Process clsProcess in Process.GetProcesses())
                {
                    if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(String.Format("Xảy ra lỗi khi kiểm tra ứng dụng {0}.", name), ex);
            }

            return false;
        }
        private void btnSigningSheet_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsProcessOpen("Inventec.SignPadManager"))
                {
                    string pathSaveFolder = Path.Combine(Path.Combine(Application.StartupPath, "temp"), DateTime.Now.ToString("ddMMyyyy"), "STPadLibFile");
                    if (!Directory.Exists(pathSaveFolder))
                    {
                        Directory.CreateDirectory(pathSaveFolder);
                    }
                    DirectoryInfo dicInfo = new DirectoryInfo(pathSaveFolder);

                    string[] fileImage = Directory.GetFiles(dicInfo.FullName, "*");
                    if (fileImage != null && fileImage.Length > 0)
                    {
                        try
                        {
                            dicInfo.Delete(true);
                        }
                        catch (Exception exx)
                        {
                            LogSystem.Error(exx);
                        }
                    }

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = Application.StartupPath + @"\Inventec.SignPadManager.exe";
                    Process.Start(startInfo);

                    while (true)
                    {
                        if (IsProcessOpen("Inventec.SignPadManager"))
                        {
                            Inventec.Common.Logging.LogSystem.Info("btnUploadImageUsingSigDevice_Click.1");
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Info("btnUploadImageUsingSigDevice_Click.2");
                            fileImage = Directory.GetFiles(dicInfo.FullName, "*");
                            if (fileImage != null && fileImage.Length > 0)
                            {
                                picSignPatient.Image = Image.FromFile(fileImage[0]);
                                Inventec.Common.Logging.LogSystem.Info("btnUploadImageUsingSigDevice_Click.3");
                                break;
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Info("btnUploadImageUsingSigDevice_Click.4");
                                break;
                            }
                        }
                    }
                }           
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private Bitmap ResizeSignImage(string imageFile = "")
        {
            Size size = new Size();
            if (picSignPatient.Image != null)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => picSignPatient.Image.Size.Width), picSignPatient.Image.Size.Width)
                     + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => picSignPatient.Image.Size.Height), picSignPatient.Image.Size.Height)
                     + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => imageFile), imageFile));

                size = picSignPatient.Image.Size;
                if (picSignPatient.Image.Size.Width > 600 || picSignPatient.Image.Size.Height > 600)
                {
                    int heightD = (int)((double)((double)600 / (double)(picSignPatient.Image.Size.Width)) * (double)(picSignPatient.Image.Size.Height));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heightD), heightD));
                    size = new Size(600, heightD);
                    Inventec.Common.Logging.LogSystem.Debug("Anh chu ky qua lon sẽ bi resize lai ve kich thuoc (" + size.Width + ", " + size.Height + ")" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => size), size));
                }
            }

            Bitmap b1 = !String.IsNullOrEmpty(imageFile) ? (Bitmap)Image.FromFile(imageFile) : (Bitmap)picSignPatient.Image.Clone();
            if (!String.IsNullOrEmpty(imageFile) && b1 != null)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => imageFile), imageFile));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("b1.Size", b1.Size));
                int heightD = b1.Size.Height;
                int wightD = b1.Size.Width;
                if (b1.Size.Width > 600 || b1.Size.Height > 600)
                {
                    wightD = 600;
                    heightD = (int)((double)((double)600 / (double)(b1.Size.Width)) * (double)(b1.Size.Height));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heightD), heightD));
                }
                size = new Size(wightD, heightD);
                Inventec.Common.Logging.LogSystem.Debug("Anh chu ky qua lon sẽ bi resize lai ve kich thuoc (" + size.Width + ", " + size.Height + ")" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => size), size));
            }

            int destWidth = size.Width;
            int destHeight = size.Height;
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(b1, 0, 0, destWidth, destHeight);
            g.Dispose();
            b.MakeTransparent();

            //}

            return b;
        }
        private void btnChoosePicture_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "*.png|*.png|*.jpg|*.jpg|*.jpeg|*.jpeg|*.bmp|*.bmp|*.gif|*.gif|*.ico|*.ico|All file|*.*";
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bImage = ResizeSignImage(openFile.FileName);
                    picSignPatient.Image = bImage;
                    isDefaultImageLoaded = false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void FillImageFromModuleCamereToUC(object dataImage)
        {
            try
            {                
                if (dataImage != null)
                {
                    var originalImg = (System.Drawing.Image)dataImage;
                    Bitmap bmp = new Bitmap((System.Drawing.Image)dataImage);
                    Bitmap processed = ProcessSignatureImage(bmp);
                    picSignPatient.Image = processed;
                    picSignPatient.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;

                    var check = this.ListfileNameAttack.OrderByDescending(o => o.Dem).FirstOrDefault();
                    int dem = (check == null || check.Dem == 0) ? 1 : check.Dem + 1;

                    fileNameAttack = new AttackADO()
                    {
                        FILE_NAME = "Ảnh chụp " + dem + ".jpg",
                        FullName = "Ảnh chụp " + dem + ".jpg",
                        image = (System.Drawing.Image)processed.Clone(),
                        Dem = dem
                    };

                    this.ListfileNameAttack.Add(this.fileNameAttack);

                    Inventec.Common.Logging.LogSystem.Info("dữ liệu ảnh chụp: " +
                        Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => ListfileNameAttack),
                            this.ListfileNameAttack));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private Bitmap ProcessSignatureImage(Bitmap original)
        {
            // Bước 1: Crop vùng có chữ ký
            Rectangle cropRect = GetBoundingBox(original);
            Bitmap cropped = original.Clone(cropRect, original.PixelFormat);

            // Bước 2: Chuyển nền trắng sang trong suốt
            Bitmap transparent = MakeTransparentSignature(cropped);
            transparent.MakeTransparent();
            return transparent;
        }

        private Rectangle GetBoundingBox(Bitmap bmp)
        {
            int xMin = bmp.Width, xMax = 0, yMin = bmp.Height, yMax = 0;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    // Xác định pixel có mực (không phải trắng)
                    if (!(c.R > 240 && c.G > 240 && c.B > 240))
                    {
                        if (x < xMin) xMin = x;
                        if (x > xMax) xMax = x;
                        if (y < yMin) yMin = y;
                        if (y > yMax) yMax = y;
                    }
                }
            }

            if (xMax < xMin || yMax < yMin)
                return new Rectangle(0, 0, bmp.Width, bmp.Height); // fallback

            int width = xMax - xMin + 1;
            int height = yMax - yMin + 1;
            return new Rectangle(xMin, yMin, width, height);
        }

        private Bitmap MakeTransparentSignature(Bitmap bmp)
        {
            Bitmap transparent = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    if (c.R > 240 && c.G > 240 && c.B > 240)
                        transparent.SetPixel(x, y, Color.FromArgb(0, 255, 255, 255)); // trong suốt
                    else
                        transparent.SetPixel(x, y, Color.FromArgb(255, c)); // giữ màu
                }
            }
            return transparent;
        }


        private void btnTakePicture_Click(object sender, EventArgs e)
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
                    isDefaultImageLoaded = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void picDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string defaultImagePath = GetPathDefault();

                if (File.Exists(defaultImagePath))
                {
                    picSignPatient.Image = Image.FromFile(defaultImagePath);
                }
                else
                {
                    picSignPatient.Image = null;
                    picSignPatient.Properties.NullText = "Chưa có chữ ký";
                }
                picSignPatient.Refresh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private Image ResizeImageToFit(Image originalImage, int targetWidth, int targetHeight)
        {
            try
            {
                var destRect = new Rectangle(0, 0, targetWidth, targetHeight);
                var destImage = new Bitmap(targetWidth, targetHeight);

                // Giữ độ phân giải ảnh gốc
                destImage.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    // Thiết lập chất lượng cao để ảnh không bị mờ
                    graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    // Vẽ ảnh vào bitmap đích, fill full khung
                    graphics.Clear(Color.White); // nền trắng để tránh viền đen
                    graphics.DrawImage(originalImage, destRect, 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel);
                }

                return destImage;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lỗi resize ảnh: " + ex.Message);
                return originalImage;
            }
        }

        private void SaveImageProcess(System.Drawing.Image imageData)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("SaveImageProcess: Đã chỉnh sửa ảnh");

                if (imageData == null)
                {
                    MessageBox.Show("Ảnh rỗng, không thể lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Image finalImage;
                if (imageData.Width != 853 || imageData.Height != 319)
                {
                    finalImage = ResizeImageToFit(imageData, 853, 400);
                    Inventec.Common.Logging.LogSystem.Info("Ảnh đã được resize về 853x319");
                }
                else
                {
                    finalImage = new Bitmap(imageData);
                    Inventec.Common.Logging.LogSystem.Info("Ảnh đúng kích thước khung, không cần resize");
                }

                picSignPatient.Image = finalImage;

                isDefaultImageLoaded = false;

                MessageBox.Show("Lưu ảnh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show("Có lỗi khi lưu ảnh: " + ex.Message);
            }
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("btnDraw_Click");

                Image baseImage = null;

                // Mở form vẽ
                Inventec.DrawTools.frmDrawTools f = new Inventec.DrawTools.frmDrawTools(baseImage, SaveImageProcess);
                f.StartPosition = FormStartPosition.CenterParent;
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private EMR_PATIENT_CERTIFICATE CheckPatientCertificateByCCCD(string cccdNumber)
        {
            try
            {
                var param = new CommonParam();

                var checkSdo = new EmrPatientCertificateCheckSDO
                {
                    CccdNumber = cccdNumber
                };

                var certInfo = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<EMR_PATIENT_CERTIFICATE>(
                        "api/EmrPatientCertificate/Check",
                        ApiConsumers.EmrConsumer,
                        checkSdo,
                        param
                    );

                Inventec.Common.Logging.LogSystem.Info("Kết quả kiểm tra chứng thư CCCD: " + Inventec.Common.Logging.LogUtil.TraceData("certInfo", certInfo));

                return certInfo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lỗi khi gọi API kiểm tra chứng thư CCCD", ex);
                return null;
            }
        }
        private async void btnReadCCCD_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Message.WaitingManager.Show();

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("http://localhost:7000/api/v1/verify");
                    if (!response.IsSuccessStatusCode)
                    {
                        XtraMessageBox.Show("Không kết nối được thiết bị CCCD.", "Thông báo");
                        return;
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var root = JsonConvert.DeserializeObject<RootResponse>(json);

                    if (root == null || root.success != true || root.result?.data == null)
                    {
                        XtraMessageBox.Show("Không nhận được dữ liệu thẻ CCCD.", "Thông báo");
                        return;
                    }

                    var info = root.result.data;
                    if (info.isPass != true)
                    {
                        XtraMessageBox.Show("CCCD không hợp lệ hoặc không xác thực được.", "Thông báo");
                        return;
                    }
                    if (info.score == null || info.score < 70)
                    {
                        XtraMessageBox.Show("Không xác thực được khuôn mặt.", "Thông báo");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(info.identifyNumber) || string.IsNullOrWhiteSpace(info.name))
                    {
                        XtraMessageBox.Show("Thiếu thông tin số CCCD hoặc tên.", "Thông báo");
                        return;
                    }

                    // Giải mã ảnh
                    Image parsedImage = null;
                    byte[] imgBytes = null;
                    Func<string, byte[]> TryDecodeBase64 = delegate (string b64)
                    {
                        if (string.IsNullOrWhiteSpace(b64)) return null;
                        var s = b64.Trim();
                        int commaIndex = s.IndexOf(',');
                        if (commaIndex > 0 && s.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                            s = s.Substring(commaIndex + 1);
                        s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                        try { return Convert.FromBase64String(s); } catch { return null; }
                    };
                    imgBytes = TryDecodeBase64(info.imageChip)
                        ?? TryDecodeBase64(info.imageFront)
                        ?? TryDecodeBase64(info.imageCap)
                        ?? TryDecodeBase64(info.dg2DataBase64)
                        ?? TryDecodeBase64(info.dg13DataBase64)
                        ?? TryDecodeBase64(info.dg14DataBase64)
                        ?? TryDecodeBase64(info.dg1DataBase64);

                    if (imgBytes == null || imgBytes.Length == 0)
                    {
                        XtraMessageBox.Show("Không lấy được ảnh CCCD.", "Thông báo");
                        return;
                    }
                    try
                    {
                        using (var ms = new MemoryStream(imgBytes))
                        {
                            parsedImage = Image.FromStream(ms);
                        }
                    }
                    catch
                    {
                        XtraMessageBox.Show("Không thể đọc ảnh CCCD.", "Thông báo");
                        return;
                    }

                    // Kiểm tra ngày hết hạn nếu có
                    DateTime expiredDate;
                    if (!string.IsNullOrWhiteSpace(info.expiredDate) &&
                        DateTime.TryParse(info.expiredDate, out expiredDate) &&
                        expiredDate < DateTime.Now)
                    {
                        XtraMessageBox.Show("Thẻ CCCD đã hết hạn.", "Thông báo");
                        return;
                    }

                    // Nếu qua hết các điều kiện trên thì mới hiện thành công
                    picCCCD.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
                    picCCCD.Image = parsedImage;
                    lblCCCD.Text = info.identifyNumber ?? "";
                    lblPatientName.Text = info.name ?? "";
                    lblDOB.Text = info.dateOfBirth ?? "";
                    lblGender.Text = info.sex ?? "";
                    lblAdress.Text = info.address ?? "";
                    lblIssueDate.Text = info.issueDate ?? "";
                    lblExpiredDate.Text = info.expiredDate ?? "";
                    lblMatch.Text = info.score?.ToString("0.##") + "/100";
                    lblStatus.Text = info.isPass == true ? "Hợp lệ" : "Không hợp lệ";
                    lblMatch.ForeColor = info.isPass == true ? Color.Green : Color.Red;
                    lblStatus.ForeColor = info.isPass == true ? Color.Green : Color.Red;

                    this.currentCCCDInfo = info;
                    btnRelease.Enabled = true;

                    XtraMessageBox.Show("Đọc CCCD thành công!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);                   
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Có lỗi khi quét CCCD: " + ex.Message, "Thông báo");
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
        }
        private void btnRelease_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();

                if (this.currentCCCDInfo == null)
                {
                    param.Messages.Add("Vui lòng quét CCCD trước khi phát hành chứng thư.");
                    MessageManager.Show(this.ParentForm, param, success);
                    return;
                }

                if (picSignPatient.Image == null)
                {
                    param.Messages.Add("Vui lòng ký và lưu chữ ký trước khi phát hành chứng thư.");
                    MessageManager.Show(this.ParentForm, param, success);
                    return;
                }
                string signatureBase64 = "";
                using (MemoryStream ms = new MemoryStream())
                {
                    picSignPatient.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    signatureBase64 = Convert.ToBase64String(ms.ToArray());
                }
                HisPatientSDO currentPatient = new HisPatientSDO();
                HisPatientAdvanceFilter hisPatientFilter = new HisPatientAdvanceFilter();
                hisPatientFilter.CCCD_NUMBER__EXACT = this.currentCCCDInfo.identifyNumber;

                var lstPatient = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<HisPatientSDO>>(
                        "api/HisPatient/GetSdoAdvance",
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                        hisPatientFilter,
                        param
                    );
                if (lstPatient != null && lstPatient.Count > 0)
                {
                    currentPatient = lstPatient.FirstOrDefault();
                }
                string placeOfResidence = string.IsNullOrWhiteSpace(currentCCCDInfo.address) ? currentCCCDInfo.hometown : currentCCCDInfo.address;

                EmrPatientCertificateRegisterSDO sdo = new EMR.SDO.EmrPatientCertificateRegisterSDO()
                {
                    citizenIdentify = currentCCCDInfo.identifyNumber,
                    oldCitizenIdentify = currentCCCDInfo.previousNumber,
                    fullName = currentCCCDInfo.name,
                    dateOfBirth = currentCCCDInfo.dateOfBirth,
                    dateOfExpired = currentCCCDInfo.expiredDate,
                    gender = currentCCCDInfo.sex,
                    nationality = currentCCCDInfo.nationality,
                    ethnic = currentCCCDInfo.nation,
                    religion = currentCCCDInfo.religion,
                    placeOfOrigin = currentCCCDInfo.hometown,
                    placeOfResidence = placeOfResidence,
                    placeOfProvide = currentCCCDInfo.issuePlace,
                    personalIdentification = currentCCCDInfo.character,
                    dateOfProvide = currentCCCDInfo.issueDate,
                    fatherName = currentCCCDInfo.fatherName,
                    motherName = currentCCCDInfo.motherName,
                    coupleName = currentCCCDInfo.partnerName,
                    otherName = currentCCCDInfo.otherName,
                    faceImage = currentCCCDInfo.imageChip,
                    signatureImage = signatureBase64,
                    email = currentPatient.EMAIL,
                    phone = currentPatient.PHONE,
                    PatientCode = currentPatient.PATIENT_CODE,
                    sodData = currentCCCDInfo.sodData,
                    dg1DataB64 = currentCCCDInfo.dg1DataBase64,
                    dg2DataB64 = currentCCCDInfo.dg2DataBase64,
                    dg13DataB64 = currentCCCDInfo.dg13DataBase64,
                    dg14DataB64 = currentCCCDInfo.dg14DataBase64
                };

                var result = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Post<EMR_PATIENT_CERTIFICATE>(
                        "api/EmrPatientCertificate/Register",
                        ApiConsumers.EmrConsumer,
                        sdo,
                        param
                    );

                if (result != null)
                {
                    WaitingManager.Show();
                    success = true;
                    Inventec.Common.Logging.LogSystem.Info("Phát hành chứng thư thành công. ID = " + result.ID);
                    XtraMessageBox.Show("Phát hành chứng thư thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    WaitingManager.Hide();
                    btnRelease.Enabled = false;
                    this.Close();
                }
                else
                {
                    param.Messages.Add("Phát hành chứng thư thất bại. Không có dữ liệu trả về từ server.");
                }

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi khi phát hành chứng thư: " + ex.Message, "Thông báo");
            }
        }
    }
}
