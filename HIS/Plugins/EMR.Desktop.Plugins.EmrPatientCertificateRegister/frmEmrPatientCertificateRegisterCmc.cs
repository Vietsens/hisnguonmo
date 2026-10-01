using DevExpress.XtraEditors;
using EMR.Desktop.Plugins.EmrPatientCertificateRegister.ADO;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using MOS.SDO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMR.Desktop.Plugins.EmrPatientCertificateRegister
{
    /// <summary>
    /// Man cap phat chung thu so theo nen tang CMC (server cap CTS qua HubCA on-premise hoac CSign theo cau hinh), mo khi cau hinh EMR.HSM.CMC.INTEGRATE_OPTION khac 0.
    /// Khong C06 / khong dung 2ID Storage (chip CCCD doc qua 2ID Deck chi de dien thong tin): lay anh CCCD 2 mat, anh khuon mat, chu ky tay;
    /// server ghep PDF anh 2 mat CCCD, ky bang CKS Vien roi gui CMC cap chung thu so.
    /// </summary>
    public partial class frmEmrPatientCertificateRegisterCmc : HIS.Desktop.Utility.FormBase
    {
        private const string API_REGISTER = "api/EmrPatientCertificate/Register";
        private const string API_CERTIFICATE_GET = "api/EmrPatientCertificate/Get";
        private const string API_HIS_PATIENT = "api/HisPatient/GetSdoAdvance";
        private const string MODULE_CAMERA = "HIS.Desktop.Plugins.Camera";
        // Thiet bi doc chip CCCD 2ID Deck tren may tram (giong nut Doc CCCD o man cap chung thu 2ID)
        private const string API_2ID_DECK_VERIFY = "http://localhost:7000/api/v1/verify";
        private const double MIN_FACE_MATCH_SCORE = 70;
        private const string DATE_FORMAT = "dd/MM/yyyy";
        private const int MAX_PHOTO_SIZE = 1600;
        private const int MAX_SIGNATURE_SIZE = 800;

        private Inventec.Desktop.Common.Modules.Module moduleData;
        private HisPatientSDO currentPatient;
        private byte[] frontImage;
        private byte[] backImage;
        private byte[] faceImage;
        private byte[] signatureImage;
        private bool isFilling;

        public frmEmrPatientCertificateRegisterCmc()
        {
            InitializeComponent();
        }

        public frmEmrPatientCertificateRegisterCmc(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.moduleData = module;
                this.Text = (module != null && !String.IsNullOrWhiteSpace(module.text) ? module.text : "Phát hành chứng thư số") + " - CMC";
                this.Icon = Icon.ExtractAssociatedIcon(Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                this.Release();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #region Su kien

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.LoadPatient();
        }

        private async void btnReadCccd_Click(object sender, EventArgs e)
        {
            await this.ReadCccdFrom2ID();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.LoadPatient();
            }
        }

        // Hai o tim thay the nhau: go vao o nay thi xoa o kia, tranh tim nham theo ma benh nhan cua lan tim truoc
        private void txtSearchCccd_EditValueChanged(object sender, EventArgs e)
        {
            this.ClearOtherSearchBox(this.txtSearchPatientCode);
        }

        private void txtSearchPatientCode_EditValueChanged(object sender, EventArgs e)
        {
            this.ClearOtherSearchBox(this.txtSearchCccd);
        }

        private void btnFrontCapture_Click(object sender, EventArgs e)
        {
            this.TakePicture(1);
        }

        private void btnFrontChoose_Click(object sender, EventArgs e)
        {
            this.ChooseImage(1);
        }

        private void btnBackCapture_Click(object sender, EventArgs e)
        {
            this.TakePicture(2);
        }

        private void btnBackChoose_Click(object sender, EventArgs e)
        {
            this.ChooseImage(2);
        }

        private void btnFaceCapture_Click(object sender, EventArgs e)
        {
            this.TakePicture(3);
        }

        private void btnFaceChoose_Click(object sender, EventArgs e)
        {
            this.ChooseImage(3);
        }

        private void btnSignPad_Click(object sender, EventArgs e)
        {
            this.SignByPad();
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            this.DrawSignature();
        }

        private void btnSignChoose_Click(object sender, EventArgs e)
        {
            this.ChooseImage(4);
        }

        private void btnRelease_Click(object sender, EventArgs e)
        {
            this.Release();
        }

        #endregion

        #region Tai thong tin benh nhan

        private void LoadPatient()
        {
            try
            {
                string cccd = this.txtSearchCccd.Text.Trim();
                string patientCode = this.txtSearchPatientCode.Text.Trim();
                if (String.IsNullOrWhiteSpace(cccd) && String.IsNullOrWhiteSpace(patientCode))
                {
                    XtraMessageBox.Show("Nhập số CCCD hoặc mã bệnh nhân để tìm.", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                if (!String.IsNullOrWhiteSpace(patientCode))
                {
                    filter.PATIENT_CODE__EXACT = patientCode;
                }
                else
                {
                    filter.CCCD_NUMBER__EXACT = cccd;
                }

                WaitingManager.Show();
                List<HisPatientSDO> patients = new BackendAdapter(param).Get<List<HisPatientSDO>>(API_HIS_PATIENT, ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();

                if (patients == null || patients.Count == 0)
                {
                    this.ClearPatientInfo();
                    if (!String.IsNullOrWhiteSpace(cccd))
                    {
                        this.txtCccd.Text = cccd;
                        this.FillFromCertificate(cccd);
                    }
                    XtraMessageBox.Show("Không tìm thấy bệnh nhân trên HIS. Nhập thông tin và lấy ảnh để cấp chứng thư số.", "Thông báo");
                    return;
                }

                this.FillPatient(patients.First());
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi khi tải thông tin bệnh nhân: " + ex.Message, "Thông báo");
            }
        }

        private void ClearOtherSearchBox(TextEdit other)
        {
            if (this.isFilling) return;
            this.isFilling = true;
            try
            {
                other.Text = "";
            }
            finally
            {
                this.isFilling = false;
            }
        }

        /// <summary>
        /// Xoa thong tin va 4 anh cua benh nhan dang hien. Benh nhan moi khong co anh tren HIS thi SetImage khong ghi de,
        /// nen phai xoa truoc de khong gui nham anh CCCD/khuon mat/chu ky cua benh nhan truoc sang CMC.
        /// </summary>
        private void ClearPatientInfo()
        {
            this.currentPatient = null;
            foreach (TextEdit text in new TextEdit[] { this.txtCccd, this.txtOldCccd, this.txtFullName, this.txtNationality, this.txtEthnic,
                this.txtReligion, this.txtPlaceOfOrigin, this.txtPlaceOfResidence, this.txtPlaceOfProvide, this.txtPhone, this.txtEmail })
            {
                text.Text = "";
            }
            this.dtDob.EditValue = null;
            this.dtDateOfProvide.EditValue = null;
            this.dtDateOfExpired.EditValue = null;
            this.cboGender.EditValue = null;
            this.frontImage = null;
            this.backImage = null;
            this.faceImage = null;
            this.signatureImage = null;
            this.picFront.Image = null;
            this.picBack.Image = null;
            this.picFace.Image = null;
            this.picSignature.Image = null;
        }

        private void FillPatient(HisPatientSDO patient)
        {
            this.ClearPatientInfo();
            this.currentPatient = patient;
            this.isFilling = true;
            try
            {
                this.txtSearchPatientCode.Text = patient.PATIENT_CODE;
                this.txtSearchCccd.Text = patient.CCCD_NUMBER;
            }
            finally
            {
                this.isFilling = false;
            }
            this.txtCccd.Text = patient.CCCD_NUMBER;
            this.txtOldCccd.Text = patient.CMND_NUMBER;
            this.txtFullName.Text = patient.VIR_PATIENT_NAME;
            this.dtDob.EditValue = patient.IS_HAS_NOT_DAY_DOB == 1 ? null : TimeNumberToDateTime(patient.DOB);
            this.cboGender.EditValue = patient.HIS_GENDER != null ? patient.HIS_GENDER.GENDER_NAME : null;
            this.txtNationality.Text = !String.IsNullOrWhiteSpace(patient.NATIONAL_NAME) ? patient.NATIONAL_NAME : "Việt Nam";
            this.txtEthnic.Text = patient.ETHNIC_NAME;
            this.txtReligion.Text = patient.RELIGION_NAME;
            this.txtPlaceOfResidence.Text = !String.IsNullOrWhiteSpace(patient.VIR_ADDRESS) ? patient.VIR_ADDRESS : patient.ADDRESS;
            this.txtPlaceOfProvide.Text = patient.CCCD_PLACE;
            this.dtDateOfProvide.EditValue = TimeNumberToDateTime(patient.CCCD_DATE);
            this.txtPhone.Text = !String.IsNullOrWhiteSpace(patient.MOBILE) ? patient.MOBILE : patient.PHONE;
            this.txtEmail.Text = patient.EMAIL;

            // Truong HIS khong luu (que quan, han the...) lay tu lan cap CTS truoc de nguoi dung thay va kiem tra truoc khi gui
            this.FillFromCertificate(patient.CCCD_NUMBER);

            // Anh da chup o tiep don HIS (neu co) - nhan vien co the chon lai
            this.SetImage(1, DownloadFss(patient.CMND_BEFORE_URL), false);
            this.SetImage(2, DownloadFss(patient.CMND_AFTER_URL), false);
            this.SetImage(3, DownloadFss(patient.AVATAR_URL), false);
        }

        /// <summary>Dien cac truong con trong tu ban ghi EMR_PATIENT_CERTIFICATE theo CCCD (lan cap CTS truoc, du lieu doc tu chip)</summary>
        private void FillFromCertificate(string cccd)
        {
            if (String.IsNullOrWhiteSpace(cccd)) return;
            try
            {
                CommonParam param = new CommonParam();
                EMR.Filter.EmrPatientCertificateFilter filter = new EMR.Filter.EmrPatientCertificateFilter();
                filter.CCCD_NUMBER__EXACT = cccd.Trim();
                List<EMR_PATIENT_CERTIFICATE> certificates = new BackendAdapter(param).Get<List<EMR_PATIENT_CERTIFICATE>>(API_CERTIFICATE_GET, ApiConsumers.EmrConsumer, filter, param);
                EMR_PATIENT_CERTIFICATE certificate = certificates != null ? certificates.FirstOrDefault() : null;
                if (certificate == null) return;

                FillIfEmpty(this.txtFullName, certificate.FULL_NAME);
                if (this.dtDob.EditValue == null) this.dtDob.EditValue = TimeNumberToDateTime(certificate.DOB);
                if (String.IsNullOrWhiteSpace(this.cboGender.Text) && !String.IsNullOrWhiteSpace(certificate.GENDER_NAME)) this.cboGender.EditValue = certificate.GENDER_NAME.Trim();
                FillIfEmpty(this.txtNationality, certificate.NATIONALITY);
                FillIfEmpty(this.txtPlaceOfOrigin, certificate.PLACE_ORIGIN);
                FillIfEmpty(this.txtPlaceOfResidence, certificate.PLACE_OF_RESIDENCE);
                FillIfEmpty(this.txtPlaceOfProvide, certificate.PLACE_OF_PROVIDE);
                if (this.dtDateOfProvide.EditValue == null) this.dtDateOfProvide.EditValue = TimeNumberToDateTime(certificate.DATE_OF_PROVIDE);
                if (this.dtDateOfExpired.EditValue == null) this.dtDateOfExpired.EditValue = TimeNumberToDateTime(certificate.DATE_OF_EXPIRY);
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Khong lay duoc chung thu so da luu theo CCCD " + cccd + ". " + ex.Message);
            }
        }

        private static void FillIfEmpty(TextEdit text, string value)
        {
            if (String.IsNullOrWhiteSpace(text.Text) && !String.IsNullOrWhiteSpace(value))
            {
                text.Text = value.Trim();
            }
        }

        private static byte[] DownloadFss(string url)
        {
            if (String.IsNullOrWhiteSpace(url)) return null;
            try
            {
                using (MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(url))
                {
                    return stream != null && stream.Length > 0 ? stream.ToArray() : null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Khong tai duoc anh tu FSS: " + url + ". " + ex.Message);
                return null;
            }
        }

        /// <summary>yyyyMMddHHmmss -> DateTime; khong hop le tra ve null</summary>
        private static DateTime? TimeNumberToDateTime(long? timeNumber)
        {
            if (!timeNumber.HasValue || timeNumber.Value <= 0) return null;
            string s = timeNumber.Value.ToString();
            if (s.Length < 8) return null;
            DateTime result;
            if (DateTime.TryParseExact(s.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }

        #endregion

        #region Doc CCCD qua 2ID Deck

        /// <summary>
        /// Doc chip CCCD qua thiet bi 2ID Deck (API_2ID_DECK_VERIFY), kiem tra giong nut Doc CCCD o man cap chung thu 2ID
        /// (the hop le, so khop khuon mat >= 70, the con han) roi dien thong tin tu chip va lay anh khuon mat.
        /// </summary>
        private async Task ReadCccdFrom2ID()
        {
            try
            {
                WaitingManager.Show();
                string json;
                using (HttpClient client = new HttpClient())
                {
                    LogSystem.Info("Cap CTS CMC: doc CCCD qua 2ID Deck " + API_2ID_DECK_VERIFY);
                    HttpResponseMessage response = await client.GetAsync(API_2ID_DECK_VERIFY);
                    json = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        WaitingManager.Hide();
                        LogSystem.Warn(String.Format("2ID Deck loi HTTP {0}: {1}", (int)response.StatusCode, json));
                        XtraMessageBox.Show(String.IsNullOrWhiteSpace(json) ? "Thiết bị đọc CCCD báo lỗi." : json, "Thông báo");
                        return;
                    }
                }
                WaitingManager.Hide();

                DTO.RootResponse root = JsonConvert.DeserializeObject<DTO.RootResponse>(json);
                DTO.CccdData info = root != null && root.success && root.result != null ? root.result.data : null;
                if (info == null)
                {
                    XtraMessageBox.Show("Không nhận được dữ liệu thẻ CCCD.", "Thông báo");
                    return;
                }
                // Khong ghi noi dung response vao log: co anh va du lieu ca nhan
                LogSystem.Info(String.Format("2ID Deck: CCCD={0}, isPass={1}, score={2}, imageCap={3}, imageChip={4} ky tu", info.identifyNumber, info.isPass, info.score,
                    info.imageCap != null ? info.imageCap.Length : 0, info.imageChip != null ? info.imageChip.Length : 0));
                if (!info.isPass)
                {
                    XtraMessageBox.Show("CCCD không hợp lệ hoặc không xác thực được.", "Thông báo");
                    return;
                }
                if (!info.score.HasValue || info.score.Value < MIN_FACE_MATCH_SCORE)
                {
                    XtraMessageBox.Show("Không xác thực được khuôn mặt.", "Thông báo");
                    return;
                }
                if (String.IsNullOrWhiteSpace(info.identifyNumber) || String.IsNullOrWhiteSpace(info.name))
                {
                    XtraMessageBox.Show("Thiếu thông tin số CCCD hoặc tên.", "Thông báo");
                    return;
                }
                DateTime? expired = ParseCardDate(info.expiredDate);
                if (expired.HasValue && expired.Value.Date < DateTime.Today)
                {
                    XtraMessageBox.Show("Thẻ CCCD đã hết hạn.", "Thông báo");
                    return;
                }

                this.FillFromCard(info);
                XtraMessageBox.Show("Đọc CCCD thành công. Điểm so khớp khuôn mặt: " + info.score.Value.ToString("0.##") + "/100.", "Thông báo");
            }
            catch (HttpRequestException ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
                XtraMessageBox.Show("Không kết nối được thiết bị đọc CCCD (" + API_2ID_DECK_VERIFY + "). Kiểm tra ứng dụng 2ID Deck đã bật trên máy này chưa.", "Thông báo");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi khi đọc CCCD: " + ex.Message, "Thông báo");
            }
        }

        /// <summary>
        /// Dien thong tin doc tu chip (nguon chinh xac nhat cho cac truong tren CCCD). Benh nhan co tren HIS thi lien ket
        /// (ma BN, ho ten bo me, anh CCCD da chup o tiep don); anh khuon mat lay anh chup luc xac thuc, khong co thi anh trong chip.
        /// </summary>
        private void FillFromCard(DTO.CccdData info)
        {
            string cccd = info.identifyNumber.Trim();
            this.ClearPatientInfo();

            HisPatientSDO patient = this.FindHisPatientByCccd(cccd);
            this.isFilling = true;
            try
            {
                this.txtSearchCccd.Text = cccd;
                this.txtSearchPatientCode.Text = patient != null ? patient.PATIENT_CODE : "";
            }
            finally
            {
                this.isFilling = false;
            }
            if (patient != null)
            {
                this.currentPatient = patient;
                this.txtPhone.Text = !String.IsNullOrWhiteSpace(patient.MOBILE) ? patient.MOBILE : patient.PHONE;
                this.txtEmail.Text = patient.EMAIL;
                this.SetImage(1, DownloadFss(patient.CMND_BEFORE_URL), false);
                this.SetImage(2, DownloadFss(patient.CMND_AFTER_URL), false);
                this.SetImage(3, DownloadFss(patient.AVATAR_URL), false);
            }

            this.txtCccd.Text = cccd;
            this.txtOldCccd.Text = info.previousNumber;
            this.txtFullName.Text = info.name;
            this.dtDob.EditValue = ParseCardDate(info.dateOfBirth);
            this.cboGender.EditValue = String.IsNullOrWhiteSpace(info.sex) ? null : info.sex.Trim();
            this.txtNationality.Text = !String.IsNullOrWhiteSpace(info.nationality) ? info.nationality : "Việt Nam";
            this.txtEthnic.Text = info.nation;
            this.txtReligion.Text = info.religion;
            this.txtPlaceOfOrigin.Text = info.hometown;
            this.txtPlaceOfResidence.Text = info.address;
            this.txtPlaceOfProvide.Text = info.issuePlace;
            this.dtDateOfProvide.EditValue = ParseCardDate(info.issueDate);
            this.dtDateOfExpired.EditValue = ParseCardDate(info.expiredDate);
            if (!String.IsNullOrWhiteSpace(info.phone)) this.txtPhone.Text = info.phone;
            if (!String.IsNullOrWhiteSpace(info.email)) this.txtEmail.Text = info.email;

            byte[] face = DecodeBase64(info.imageCap) ?? DecodeBase64(info.imageChip);
            this.SetImage(3, face, false);

            // Truong chip khong co (neu co) lay tu lan cap CTS truoc
            this.FillFromCertificate(cccd);
        }

        private HisPatientSDO FindHisPatientByCccd(string cccd)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                filter.CCCD_NUMBER__EXACT = cccd;
                List<HisPatientSDO> patients = new BackendAdapter(param).Get<List<HisPatientSDO>>(API_HIS_PATIENT, ApiConsumers.MosConsumer, filter, param);
                return patients != null ? patients.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Khong tim duoc benh nhan HIS theo CCCD " + cccd + ". " + ex.Message);
                return null;
            }
        }

        /// <summary>Ngay tren the (dd/MM/yyyy la chinh); chuoi khong phai ngay (vd "Khong thoi han") tra ve null</summary>
        private static DateTime? ParseCardDate(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return null;
            DateTime result;
            string[] formats = new string[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "yyyy-MM-dd", "yyyyMMdd", "ddMMyyyy" };
            if (DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }

        private static byte[] DecodeBase64(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return null;
            string s = value.Trim();
            int comma = s.IndexOf(',');
            if (s.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma > 0)
            {
                s = s.Substring(comma + 1);
            }
            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            try
            {
                return Convert.FromBase64String(s);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        #endregion

        #region Anh

        /// <summary>1: CCCD mat truoc, 2: CCCD mat sau, 3: khuon mat, 4: chu ky</summary>
        private void ChooseImage(int slot)
        {
            try
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Filter = "Ảnh (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        this.SetImage(slot, File.ReadAllBytes(dialog.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                XtraMessageBox.Show("Không đọc được ảnh: " + ex.Message, "Thông báo");
            }
        }

        /// <summary>
        /// Chup anh bang camera qua module dung chung HIS.Desktop.Plugins.Camera (giong nut Chup anh o man cap chung thu 2ID).
        /// Module goi lai DelegateSelectData voi System.Drawing.Image khi nguoi dung xac nhan anh.
        /// </summary>
        private void TakePicture(int slot)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module cameraModule = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws
                    .FirstOrDefault(o => o.ModuleLink == MODULE_CAMERA);
                if (cameraModule == null || !cameraModule.IsPlugin || cameraModule.ExtensionInfo == null)
                {
                    LogSystem.Error("Khong tim thay moduleLink = " + MODULE_CAMERA);
                    XtraMessageBox.Show("Không mở được chức năng chụp ảnh (" + MODULE_CAMERA + "). Kiểm tra tài khoản đã được phân quyền chức năng này chưa.", "Thông báo");
                    return;
                }
                HIS.Desktop.Common.DelegateSelectData onCaptured = (data) => this.SetCameraImage(slot, data);
                List<object> args = new List<object>();
                args.Add(onCaptured);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(cameraModule, 0, 0), args);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                XtraMessageBox.Show("Có lỗi khi mở chức năng chụp ảnh: " + ex.Message, "Thông báo");
            }
        }

        private void SetCameraImage(int slot, object data)
        {
            try
            {
                Image captured = data as Image;
                if (captured == null) return;
                using (Bitmap bitmap = new Bitmap(captured))
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Jpeg);
                    this.SetImage(slot, stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                XtraMessageBox.Show("Không đọc được ảnh vừa chụp: " + ex.Message, "Thông báo");
            }
        }

        /// <param name="showError">false: anh lay tu dong (FSS, thiet bi) khong hop le thi bo qua, khong bao nguoi dung</param>
        private void SetImage(int slot, byte[] data, bool showError = true)
        {
            if (data == null || data.Length == 0) return;
            byte[] normalized;
            try
            {
                normalized = slot == 4 ? NormalizeImage(data, MAX_SIGNATURE_SIZE, true) : NormalizeImage(data, MAX_PHOTO_SIZE, false);
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Anh khong hop le (o " + slot + "). " + ex.Message);
                if (showError) XtraMessageBox.Show("File không phải ảnh hợp lệ.", "Thông báo");
                return;
            }
            Image preview = ToImage(normalized);
            switch (slot)
            {
                case 1: this.frontImage = normalized; this.picFront.Image = preview; break;
                case 2: this.backImage = normalized; this.picBack.Image = preview; break;
                case 3: this.faceImage = normalized; this.picFace.Image = preview; break;
                case 4: this.signatureImage = normalized; this.picSignature.Image = preview; break;
            }
        }

        /// <summary>Thu nho anh qua lon (giam dung luong goi CMC). Anh chup -> JPEG; chu ky -> PNG giu nen trong suot.</summary>
        private static byte[] NormalizeImage(byte[] data, int maxSize, bool keepPng)
        {
            using (MemoryStream input = new MemoryStream(data))
            using (Image image = Image.FromStream(input))
            {
                double scale = Math.Min(1.0, (double)maxSize / Math.Max(image.Width, image.Height));
                bool isJpeg = ImageFormat.Jpeg.Equals(image.RawFormat);
                bool isPng = ImageFormat.Png.Equals(image.RawFormat);
                if (scale >= 1.0 && ((keepPng && isPng) || (!keepPng && isJpeg)))
                {
                    return data;
                }

                int width = Math.Max(1, (int)(image.Width * scale));
                int height = Math.Max(1, (int)(image.Height * scale));
                using (Bitmap bitmap = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        if (!keepPng) g.Clear(Color.White);
                        g.DrawImage(image, 0, 0, width, height);
                    }
                    using (MemoryStream output = new MemoryStream())
                    {
                        if (keepPng)
                        {
                            bitmap.Save(output, ImageFormat.Png);
                        }
                        else
                        {
                            ImageCodecInfo jpeg = ImageCodecInfo.GetImageEncoders().First(o => o.FormatID == ImageFormat.Jpeg.Guid);
                            EncoderParameters parameters = new EncoderParameters(1);
                            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L);
                            bitmap.Save(output, jpeg, parameters);
                        }
                        return output.ToArray();
                    }
                }
            }
        }

        private static Image ToImage(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (Image image = Image.FromStream(stream))
            {
                return new Bitmap(image);
            }
        }

        /// <summary>Ky tren thiet bi SignPad (Inventec.SignPadManager), cung thu muc anh nhu man cap chung thu 2ID</summary>
        private void SignByPad()
        {
            try
            {
                if (IsProcessOpen("Inventec.SignPadManager"))
                {
                    XtraMessageBox.Show("Ứng dụng ký SignPad đang mở.", "Thông báo");
                    return;
                }
                string exe = Path.Combine(Application.StartupPath, "Inventec.SignPadManager.exe");
                if (!File.Exists(exe))
                {
                    XtraMessageBox.Show("Không tìm thấy ứng dụng ký Inventec.SignPadManager.exe.", "Thông báo");
                    return;
                }

                string folder = Path.Combine(Path.Combine(Application.StartupPath, "temp"), DateTime.Now.ToString("ddMMyyyy"), "STPadLibFile");
                if (Directory.Exists(folder))
                {
                    try
                    {
                        Directory.Delete(folder, true);
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Warn(ex);
                    }
                }
                Directory.CreateDirectory(folder);

                using (Process process = Process.Start(new ProcessStartInfo(exe)))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                    }
                }

                string[] files = Directory.GetFiles(folder, "*");
                if (files != null && files.Length > 0)
                {
                    this.SetImage(4, File.ReadAllBytes(files[0]));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                XtraMessageBox.Show("Có lỗi khi ký trên SignPad: " + ex.Message, "Thông báo");
            }
        }

        /// <summary>Ve chu ky bang chuot tren form ve dung chung (Inventec.DrawTools), giong nut Ve o man cap chung thu 2ID</summary>
        private void DrawSignature()
        {
            try
            {
                Inventec.DrawTools.frmDrawTools f = new Inventec.DrawTools.frmDrawTools(null, this.OnSignatureDrawn);
                f.StartPosition = FormStartPosition.CenterParent;
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi khi mở chức năng vẽ chữ ký: " + ex.Message, "Thông báo");
            }
        }

        private void OnSignatureDrawn(Image drawn)
        {
            try
            {
                if (drawn == null) return;
                using (Bitmap bitmap = new Bitmap(drawn))
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    this.SetImage(4, stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                XtraMessageBox.Show("Không đọc được chữ ký vừa vẽ: " + ex.Message, "Thông báo");
            }
        }

        private static bool IsProcessOpen(string name)
        {
            try
            {
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName == name || process.ProcessName == name + ".exe" || process.ProcessName == name + " (32 bit)" || process.ProcessName == name + ".exe (32 bit)")
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug("Loi kiem tra ung dung " + name, ex);
            }
            return false;
        }

        #endregion

        #region Phat hanh

        private void Release()
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                // Kiem tra du cac truong CMC bat buoc (giong EmrPatientCertificateRegisterBehaviorEv.VerifyCmcHubCa), bao 1 lan
                List<string> missing = new List<string>();
                Control firstInvalid = null;
                string cccd = (this.txtCccd.Text ?? "").Trim();
                if (!Regex.IsMatch(cccd, "^[0-9]{12}$"))
                {
                    missing.Add("số CCCD (12 chữ số)");
                    firstInvalid = this.txtCccd;
                }
                RequireValue(missing, ref firstInvalid, this.txtFullName, "họ và tên");
                RequireValue(missing, ref firstInvalid, this.dtDob, "ngày sinh");
                RequireValue(missing, ref firstInvalid, this.cboGender, "giới tính");
                RequireValue(missing, ref firstInvalid, this.txtNationality, "quốc tịch");
                RequireValue(missing, ref firstInvalid, this.txtPlaceOfOrigin, "quê quán");
                RequireValue(missing, ref firstInvalid, this.txtPlaceOfResidence, "nơi thường trú");
                RequireValue(missing, ref firstInvalid, this.txtPlaceOfProvide, "nơi cấp");
                RequireValue(missing, ref firstInvalid, this.dtDateOfProvide, "ngày cấp");
                RequireValue(missing, ref firstInvalid, this.dtDateOfExpired, "ngày có giá trị đến");
                if (String.IsNullOrWhiteSpace(this.txtPhone.Text) && String.IsNullOrWhiteSpace(this.txtEmail.Text))
                {
                    // CMC bat buoc 1 trong 2
                    missing.Add("điện thoại hoặc email");
                    if (firstInvalid == null) firstInvalid = this.txtPhone;
                }
                if (this.frontImage == null) missing.Add("ảnh CCCD mặt trước");
                if (this.backImage == null) missing.Add("ảnh CCCD mặt sau");
                if (this.faceImage == null) missing.Add("ảnh khuôn mặt");
                if (this.signatureImage == null) missing.Add("chữ ký tay");
                if (missing.Count > 0)
                {
                    XtraMessageBox.Show("Vui lòng bổ sung: " + String.Join(", ", missing) + ".", "Thông báo");
                    if (firstInvalid != null) firstInvalid.Focus();
                    return;
                }

                CmcCertificateRegisterSDO sdo = new CmcCertificateRegisterSDO();
                sdo.citizenIdentify = cccd;
                sdo.oldCitizenIdentify = TrimOrNull(this.txtOldCccd.Text);
                sdo.fullName = TrimOrNull(this.txtFullName.Text);
                sdo.dateOfBirth = DateText(this.dtDob);
                sdo.gender = TrimOrNull(this.cboGender.Text);
                sdo.nationality = TrimOrNull(this.txtNationality.Text);
                sdo.ethnic = TrimOrNull(this.txtEthnic.Text);
                sdo.religion = TrimOrNull(this.txtReligion.Text);
                sdo.placeOfOrigin = TrimOrNull(this.txtPlaceOfOrigin.Text);
                sdo.placeOfResidence = TrimOrNull(this.txtPlaceOfResidence.Text);
                sdo.placeOfProvide = TrimOrNull(this.txtPlaceOfProvide.Text);
                sdo.dateOfProvide = DateText(this.dtDateOfProvide);
                sdo.dateOfExpired = DateText(this.dtDateOfExpired);
                sdo.phone = TrimOrNull(this.txtPhone.Text);
                sdo.email = TrimOrNull(this.txtEmail.Text);
                sdo.PatientCode = this.currentPatient != null ? this.currentPatient.PATIENT_CODE : TrimOrNull(this.txtSearchPatientCode.Text);
                if (this.currentPatient != null)
                {
                    sdo.fatherName = this.currentPatient.FATHER_NAME;
                    sdo.motherName = this.currentPatient.MOTHER_NAME;
                }
                sdo.faceImage = Convert.ToBase64String(this.faceImage);
                sdo.signatureImage = Convert.ToBase64String(this.signatureImage);
                sdo.cccdFrontImage = Convert.ToBase64String(this.frontImage);
                sdo.cccdBackImage = Convert.ToBase64String(this.backImage);

                LogSystem.Info(String.Format("Cap CTS CMC: CCCD={0}, PatientCode={1}, anh truoc/sau/khuon mat/chu ky = {2}/{3}/{4}/{5} byte", sdo.citizenIdentify, sdo.PatientCode, this.frontImage.Length, this.backImage.Length, this.faceImage.Length, this.signatureImage.Length));

                WaitingManager.Show();
                EMR_PATIENT_CERTIFICATE result = new BackendAdapter(param).Post<EMR_PATIENT_CERTIFICATE>(API_REGISTER, ApiConsumers.EmrConsumer, sdo, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    success = true;
                    LogSystem.Info(String.Format("Cap CTS CMC thanh cong: ID={0}, CCCD={1}, certAlias={2}", result.ID, result.CCCD_NUMBER, result.SERIAL_NUMBER));
                    XtraMessageBox.Show("Phát hành chứng thư số thành công.\nMã chứng thư (certAlias): " + result.SERIAL_NUMBER, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.btnRelease.Enabled = false;
                    this.Close();
                    return;
                }

                if (param.Messages.Count == 0)
                {
                    param.Messages.Add("Phát hành chứng thư số thất bại. Không có dữ liệu trả về từ server.");
                }
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi khi phát hành chứng thư số: " + ex.Message, "Thông báo");
            }
        }

        private static void RequireValue(List<string> missing, ref Control firstInvalid, BaseEdit editor, string name)
        {
            if (String.IsNullOrWhiteSpace(editor.Text))
            {
                missing.Add(name);
                if (firstInvalid == null) firstInvalid = editor;
            }
        }

        private static string DateText(DateEdit edit)
        {
            return edit.EditValue != null ? edit.DateTime.ToString(DATE_FORMAT, CultureInfo.InvariantCulture) : null;
        }

        private static string TrimOrNull(string value)
        {
            return String.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        #endregion
    }
}
