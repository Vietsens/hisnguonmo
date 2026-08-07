using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    /// <summary>
    /// Tự lấy thông tin sang các mục nhập liệu.
    /// Nguồn: dấu hiệu sinh tồn của lượt điều trị (mục Xử lý khám), thông tin hành chính
    /// của người bệnh trong hồ sơ và giấy chứng sinh.
    /// Nguyên tắc: chỉ điền vào ô đang trống, chỉ lấy một lần khi mở mục,
    /// chỉ áp dụng khi tạo mới để không ảnh hưởng hồ sơ đã lưu.
    /// </summary>
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Declare

        /// <summary>Đơn vị quy đổi cân nặng từ kg (dấu hiệu sinh tồn) sang gram (mục Trẻ em dưới 6 tuổi).</summary>
        private const int WEIGHT_KG_TO_GRAM = 1000;

        /// <summary>Bản ghi dấu hiệu sinh tồn gần nhất của lượt điều trị.</summary>
        private HIS_DHST latestDhst;

        /// <summary>Đã tra cứu dấu hiệu sinh tồn hay chưa — bảo đảm chỉ gọi API một lần.</summary>
        private bool isDhstLoaded;

        /// <summary>Giấy chứng sinh gần nhất của lượt điều trị.</summary>
        private HIS_BABY latestBaby;

        /// <summary>Đã tra cứu giấy chứng sinh hay chưa — bảo đảm chỉ gọi API một lần.</summary>
        private bool isBabyLoaded;

        /// <summary>Mục Khám thai đã tự lấy dữ liệu hay chưa.</summary>
        private bool isAutoFilledAntenatalVisit;

        /// <summary>Mục Trẻ em dưới 6 tuổi đã tự lấy dữ liệu hay chưa.</summary>
        private bool isAutoFilledChildUnder6;

        /// <summary>Mục Sinh đẻ đã tự lấy dữ liệu hay chưa.</summary>
        private bool isAutoFilledBirthInfo;

        /// <summary>
        /// Đang trong quá trình nạp lại thông tin hồ sơ điều trị. Lúc này hồ sơ điều trị và
        /// hồ sơ bệnh nhân còn là dữ liệu của lần tra cứu trước nên chưa được tự lấy dữ liệu sang.
        /// </summary>
        private bool isFillingDataToForm;

        #endregion

        #region Entry point

        /// <summary>
        /// Xóa trạng thái tự lấy dữ liệu. Gọi khi tạo mới hoặc khi tra cứu sang hồ sơ khác.
        /// </summary>
        private void ResetAutoFillState()
        {
            try
            {
                latestDhst = null;
                isDhstLoaded = false;
                latestBaby = null;
                isBabyLoaded = false;
                isAutoFilledAntenatalVisit = false;
                isAutoFilledChildUnder6 = false;
                isAutoFilledBirthInfo = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đánh dấu toàn bộ các mục đã tự lấy dữ liệu.
        /// Gọi khi mở hồ sơ đã lưu hoặc sao chép hồ sơ để hiển thị đúng dữ liệu hồ sơ nguồn.
        /// </summary>
        private void MarkAutoFillDone()
        {
            try
            {
                isAutoFilledAntenatalVisit = true;
                isAutoFilledChildUnder6 = true;
                isAutoFilledBirthInfo = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tự lấy dữ liệu cho mục đang mở.
        /// </summary>
        private void TryAutoFillForCurrentTab()
        {
            try
            {
                if (xtraTabControl1 == null) return;
                AutoFillDataForTab(xtraTabControl1.SelectedTabPageIndex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tự lấy dữ liệu theo mục đang mở.
        /// Tab 1: Trẻ em dưới 6 tuổi, Tab 2: Khám thai, Tab 3: Sinh đẻ.
        /// Các mục còn lại không có nguồn dữ liệu để lấy sang.
        /// </summary>
        private void AutoFillDataForTab(int tabIndex)
        {
            try
            {
                if (!IsAutoFillAllowed()) return;

                switch (tabIndex)
                {
                    case 1: // Trẻ em dưới 6 tuổi
                        AutoFillChildUnder6();
                        break;
                    case 2: // Khám thai
                        AutoFillAntenatalVisit();
                        break;
                    case 3: // Sinh đẻ
                        AutoFillBirthInfo();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chỉ tự lấy dữ liệu khi đang tạo mới và đã có hồ sơ điều trị kèm hồ sơ bệnh nhân.
        /// Mở hồ sơ đã lưu để xem hoặc sửa thì hiển thị đúng dữ liệu đã lưu, không lấy đè.
        /// </summary>
        private bool IsAutoFillAllowed()
        {
            try
            {
                if (isFillingDataToForm) return false;
                if (ExamServiceEdit != null && ExamServiceEdit.ID > 0) return false;
                if (Treatment == null || Treatment.ID <= 0) return false;
                if (Patient == null || Patient.ID <= 0) return false;
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Xử lý khi người dùng chuyển sang mục khác — tự lấy dữ liệu cho mục vừa mở.
        /// </summary>
        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                TryAutoFillForCurrentTab();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Nguồn dữ liệu

        /// <summary>
        /// Tra cứu bản ghi dấu hiệu sinh tồn gần nhất của lượt điều trị đang mở.
        /// Chỉ gọi API một lần, các lần sau dùng lại kết quả đã tra cứu.
        /// </summary>
        private HIS_DHST GetLatestDhst()
        {
            if (isDhstLoaded) return latestDhst;

            CommonParam param = new CommonParam();
            try
            {
                HisDhstFilter filter = new HisDhstFilter();
                filter.TREATMENT_ID = Treatment.ID;
                filter.ORDER_FIELD = "EXECUTE_TIME";
                filter.ORDER_DIRECTION = "DESC";

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                WaitingManager.Show();
                var dhsts = new BackendAdapter(param).Get<List<HIS_DHST>>(
                    "api/HisDhst/Get",
                    ApiConsumers.MosConsumer,
                    filter,
                    param);
                WaitingManager.Hide();

                // Chỉ đánh dấu đã tra cứu khi gọi API xong — mất kết nối thì lần mở mục sau
                // vẫn thử lại, người dùng không phải bấm Mới (thao tác này xóa dữ liệu đang nhập)
                isDhstLoaded = true;

                if (dhsts != null && dhsts.Count > 0)
                {
                    latestDhst = dhsts
                        .OrderByDescending(o => o.EXECUTE_TIME ?? 0)
                        .ThenByDescending(o => o.ID)
                        .FirstOrDefault();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(
                        "GetLatestDhst: Lượt điều trị chưa ghi nhận dấu hiệu sinh tồn. TreatmentId=" + Treatment.ID);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return latestDhst;
        }

        /// <summary>
        /// Tra cứu giấy chứng sinh gần nhất của lượt điều trị đang mở.
        /// Chỉ gọi API một lần, các lần sau dùng lại kết quả đã tra cứu.
        /// </summary>
        private HIS_BABY GetLatestBaby()
        {
            if (isBabyLoaded) return latestBaby;

            CommonParam param = new CommonParam();
            try
            {
                HisBabyFilter filter = new HisBabyFilter();
                filter.TREATMENT_ID = Treatment.ID;
                filter.ORDER_FIELD = "BORN_TIME";
                filter.ORDER_DIRECTION = "DESC";

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                WaitingManager.Show();
                var babies = new BackendAdapter(param).Get<List<HIS_BABY>>(
                    "api/HisBaby/Get",
                    ApiConsumers.MosConsumer,
                    filter,
                    param);
                WaitingManager.Hide();

                // Chỉ đánh dấu đã tra cứu khi gọi API xong — xem chú thích ở GetLatestDhst
                isBabyLoaded = true;

                if (babies != null && babies.Count > 0)
                {
                    latestBaby = babies
                        .OrderByDescending(o => o.BORN_TIME ?? 0)
                        .ThenByDescending(o => o.ID)
                        .FirstOrDefault();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(
                        "GetLatestBaby: Lượt điều trị chưa có giấy chứng sinh. TreatmentId=" + Treatment.ID);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return latestBaby;
        }

        #endregion

        #region Mục Khám thai

        /// <summary>
        /// Mục Khám thai: tự lấy cân nặng, chiều cao, huyết áp, vòng bụng từ dấu hiệu sinh tồn.
        /// Ô Chiều cao tử cung không có nguồn tương ứng nên để trống cho người dùng nhập tay.
        /// </summary>
        private void AutoFillAntenatalVisit()
        {
            try
            {
                if (isAutoFilledAntenatalVisit) return;

                HIS_DHST dhst = GetLatestDhst();

                // Tra cứu nguồn thất bại thì chưa đánh dấu để lần mở mục sau còn thử lại
                if (!isDhstLoaded) return;

                isAutoFilledAntenatalVisit = true;
                if (dhst == null) return;

                FillSpinEditIfEmpty(spnWeight2, dhst.WEIGHT);
                FillSpinEditIfEmpty(spnHeight2, dhst.HEIGHT);
                FillSpinEditIfEmpty(spnBloodPressureSystolic2, (decimal?)dhst.BLOOD_PRESSURE_MAX);
                FillSpinEditIfEmpty(spnBloodPressureDiastolic2, (decimal?)dhst.BLOOD_PRESSURE_MIN);
                FillSpinEditIfEmpty(spnAbdominalCircumference2, dhst.BELLY);

                Inventec.Common.Logging.LogSystem.Debug(
                    "AutoFillAntenatalVisit: Đã lấy dấu hiệu sinh tồn sang mục Khám thai. DhstId=" + dhst.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Mục Trẻ em dưới 6 tuổi

        /// <summary>
        /// Mục Trẻ em dưới 6 tuổi: tự lấy cân nặng, chiều cao từ dấu hiệu sinh tồn
        /// và số định danh cá nhân từ hồ sơ người bệnh.
        /// Ô Vòng đầu không có nguồn tương ứng nên để trống cho người dùng nhập tay.
        /// </summary>
        private void AutoFillChildUnder6()
        {
            try
            {
                if (isAutoFilledChildUnder6) return;

                FillTextEditIfEmpty(txtCccd1, Patient.CCCD_NUMBER);

                HIS_DHST dhst = GetLatestDhst();

                // Tra cứu nguồn thất bại thì chưa đánh dấu để lần mở mục sau còn thử lại
                if (!isDhstLoaded) return;

                isAutoFilledChildUnder6 = true;
                if (dhst == null) return;

                // Cân nặng trẻ nhập theo gram, dấu hiệu sinh tồn lưu theo kg nên phải quy đổi
                decimal? weightGram = dhst.WEIGHT.HasValue
                    ? (decimal?)Math.Round(dhst.WEIGHT.Value * WEIGHT_KG_TO_GRAM, 0)
                    : null;

                FillSpinEditIfEmpty(spnW1, weightGram);
                FillSpinEditIfEmpty(spnH1, dhst.HEIGHT);

                Inventec.Common.Logging.LogSystem.Debug(
                    "AutoFillChildUnder6: Đã lấy dấu hiệu sinh tồn sang mục Trẻ em dưới 6 tuổi. DhstId=" + dhst.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Mục Sinh đẻ

        /// <summary>
        /// Mục Sinh đẻ: tự lấy thông tin hành chính nơi đẻ cho phần Mẹ
        /// và thông tin trẻ sơ sinh từ giấy chứng sinh cho phần Con.
        /// </summary>
        private void AutoFillBirthInfo()
        {
            try
            {
                if (isAutoFilledBirthInfo) return;

                AutoFillMotherBirthPlace();

                HIS_BABY baby = GetLatestBaby();

                // Tra cứu nguồn thất bại thì chưa đánh dấu để lần mở mục sau còn thử lại.
                // AutoFillMotherBirthPlace chạy lại vẫn an toàn vì chỉ điền vào ô đang trống.
                if (!isBabyLoaded) return;

                isAutoFilledBirthInfo = true;
                if (baby != null && baby.ID > 0)
                {
                    CopyBabyDataToChildTab(baby, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Phần Mẹ: điền Tỉnh / Huyện / Xã / Địa chỉ theo thông tin hành chính của người bệnh
        /// trong hồ sơ. Đây là thông tin điền sẵn để tham khảo, người dùng sửa lại khi nơi đẻ
        /// khác địa chỉ người bệnh. Chỉ điền khi ô đang trống.
        /// </summary>
        private void AutoFillMotherBirthPlace()
        {
            try
            {
                if (addressMother == null) return;

                var currentAddress = addressMother.GetValue();
                bool hasAddress = currentAddress != null
                    && (!string.IsNullOrWhiteSpace(currentAddress.Province_Code)
                        || !string.IsNullOrWhiteSpace(currentAddress.Commune_Code)
                        || !string.IsNullOrWhiteSpace(currentAddress.Address));
                if (hasAddress) return;

                bool hasPatientAddress = !string.IsNullOrWhiteSpace(Patient.PROVINCE_CODE)
                    || !string.IsNullOrWhiteSpace(Patient.COMMUNE_CODE)
                    || !string.IsNullOrWhiteSpace(Patient.ADDRESS);
                if (!hasPatientAddress) return;

                addressMother.SetValue(new ADO.UCAddressADO()
                {
                    Province_Code = Patient.PROVINCE_CODE,
                    Province_Name = Patient.PROVINCE_NAME,
                    District_Code = Patient.DISTRICT_CODE,
                    District_Name = Patient.DISTRICT_NAME,
                    Commune_Code = Patient.COMMUNE_CODE,
                    Commune_Name = Patient.COMMUNE_NAME,
                    Address = Patient.ADDRESS
                });

                Inventec.Common.Logging.LogSystem.Debug(
                    "AutoFillMotherBirthPlace: Đã lấy thông tin hành chính của người bệnh sang mục Sinh đẻ - Mẹ. PatientId=" + Patient.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// Điền giá trị vào SpinEdit đang trống. Nguồn không có giá trị thì để trống,
        /// không điền số 0. Ô đã có giá trị thì giữ nguyên, không ghi đè.
        /// </summary>
        private void FillSpinEditIfEmpty(SpinEdit spinEdit, decimal? value)
        {
            try
            {
                if (spinEdit == null) return;
                if (!value.HasValue || value.Value == 0) return;
                if (!IsSpinEmpty(spinEdit) && spinEdit.Value != 0) return;

                spinEdit.EditValue = value.Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Điền giá trị vào TextEdit đang trống. Ô đã có giá trị thì giữ nguyên, không ghi đè.
        /// </summary>
        private void FillTextEditIfEmpty(TextEdit textEdit, string value)
        {
            try
            {
                if (textEdit == null) return;
                if (string.IsNullOrWhiteSpace(value)) return;
                if (!string.IsNullOrWhiteSpace(textEdit.Text)) return;

                textEdit.Text = value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
