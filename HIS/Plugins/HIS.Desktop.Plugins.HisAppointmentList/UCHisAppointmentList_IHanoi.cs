using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using His.Bhyt.InsuranceExpertise.LDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryHein.Bhyt.HeinHasBirthCertificate;
using MOS.LibraryHein.Bhyt.HeinJoin5Year;
using MOS.LibraryHein.Bhyt.HeinLevel;
using MOS.LibraryHein.Bhyt.HeinPaid6Month;
using MOS.LibraryHein.Bhyt.HeinRightRoute;
using MOS.SDO;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAppointmentList
{
    /// <summary>
    /// Lich hen dat qua app iHanoi (vCong 62017): cot CCCD / nguon dang ky / dich vu / phong,
    /// loc theo nguon dang ky va nut Duyet kham (kiem tra the BHYT tren cong roi goi api/HisAppointment/Approve).
    /// </summary>
    public partial class UCHisAppointmentList
    {
        private const short REGISTER_SOURCE__IHANOI = 2;
        private const int REGISTER_SOURCE_INDEX__IHANOI = 1;
        private const string GOV_RESULT__OK = "000";
        private const string GOV_RESULT__NEW_CARD = "004";
        private const string PATIENT_TYPE_CODE__BHYT_CFG = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        private const string PATIENT_TYPE_CODE__HOSPITAL_FEE_CFG = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";

        private RepositoryItemButtonEdit repositoryItemButtonEdit_APPROVE;
        private RepositoryItemButtonEdit repositoryItemButtonEdit_APPROVE_Dis;

        private void InitIHanoiControls()
        {
            try
            {
                this.cboRegisterSource.Properties.Items.Clear();
                this.cboRegisterSource.Properties.Items.AddRange(new object[] { "Tất cả nguồn đăng ký", "Đặt qua app iHanoi" });
                this.cboRegisterSource.SelectedIndex = 0;

                this.repositoryItemButtonEdit_APPROVE = this.CreateApproveButton(true);
                this.repositoryItemButtonEdit_APPROVE.ButtonClick += this.repositoryItemButtonEdit_APPROVE_ButtonClick;
                this.repositoryItemButtonEdit_APPROVE_Dis = this.CreateApproveButton(false);
                this.gridControl1.RepositoryItems.AddRange(new RepositoryItem[] { this.repositoryItemButtonEdit_APPROVE, this.repositoryItemButtonEdit_APPROVE_Dis });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private RepositoryItemButtonEdit CreateApproveButton(bool enabled)
        {
            RepositoryItemButtonEdit item = new RepositoryItemButtonEdit();
            item.AutoHeight = false;
            item.TextEditStyle = TextEditStyles.HideTextEditor;
            item.Buttons.Clear();
            EditorButton button = new EditorButton(ButtonPredefines.Glyph);
            button.Caption = "Duyệt";
            button.Enabled = enabled;
            button.ToolTip = enabled ? "Duyệt khám" : "";
            item.Buttons.Add(button);
            return item;
        }

        private void SetFilterIHanoi(HisAppointmentFilter filter)
        {
            if (this.cboRegisterSource.SelectedIndex == REGISTER_SOURCE_INDEX__IHANOI)
            {
                filter.REGISTER_SOURCE = REGISTER_SOURCE__IHANOI;
            }
        }

        private void ResetIHanoiControls()
        {
            this.cboRegisterSource.SelectedIndex = 0;
        }

        /// <summary>Gia tri cac cot khong rang buoc cua lich hen iHanoi. Tra ve false neu khong phai cot cua phan nay.</summary>
        private bool GetIHanoiUnboundValue(string fieldName, HIS_APPOINTMENT data, ref object value)
        {
            switch (fieldName)
            {
                case "REGISTER_SOURCE_str":
                    value = data.REGISTER_SOURCE == REGISTER_SOURCE__IHANOI ? "App iHanoi" : "";
                    return true;
                case "EXAM_SERVICE_str":
                    if (data.EXAM_SERVICE_ID.HasValue)
                    {
                        V_HIS_SERVICE service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == data.EXAM_SERVICE_ID.Value);
                        value = service != null ? service.SERVICE_NAME : null;
                    }
                    return true;
                case "EXAM_ROOM_str":
                    if (data.EXAM_ROOM_ID.HasValue)
                    {
                        V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == data.EXAM_ROOM_ID.Value);
                        value = room != null ? room.ROOM_NAME : null;
                    }
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>Nut Duyet kham chi bat voi lich chua xu ly da co dich vu va phong kham.</summary>
        private RepositoryItem GetApproveRepositoryItem(HIS_APPOINTMENT data)
        {
            bool canApprove = data.STATUS == 0 && data.EXAM_SERVICE_ID.HasValue && data.EXAM_ROOM_ID.HasValue;
            return canApprove ? this.repositoryItemButtonEdit_APPROVE : this.repositoryItemButtonEdit_APPROVE_Dis;
        }

        private async void repositoryItemButtonEdit_APPROVE_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HIS_APPOINTMENT row = gridView1.GetFocusedRow() as HIS_APPOINTMENT;
                if (row == null || row.STATUS != 0) return;

                HIS_PATIENT_TYPE bhyt = this.GetPatientType(PATIENT_TYPE_CODE__BHYT_CFG);
                HIS_PATIENT_TYPE fee = this.GetPatientType(PATIENT_TYPE_CODE__HOSPITAL_FEE_CFG);
                if (fee == null)
                {
                    XtraMessageBox.Show("Chưa cấu hình đối tượng thu phí (" + PATIENT_TYPE_CODE__HOSPITAL_FEE_CFG + ")", "Thông báo");
                    return;
                }

                HisAppointmentApproveSDO sdo = new HisAppointmentApproveSDO();
                sdo.AppointmentId = row.ID;
                sdo.RequestRoomId = this.currentModule.RoomId;
                sdo.PatientTypeId = fee.ID;

                string patientName = row.VIR_PATIENT_NAME;
                if (row.IS_HEIN == 1 && bhyt != null && !String.IsNullOrWhiteSpace(row.HEIN_CARD_NUMBER))
                {
                    WaitingManager.Show();
                    ResultHistoryLDO card = await this.CheckHeinCard(row);
                    WaitingManager.Hide();

                    if (this.IsValidCard(card))
                    {
                        string message = String.Format("Thẻ BHYT {0} hợp lệ trên cổng BHXH (hạn {1} - {2}, nơi ĐKBĐ {3}).\n\nDuyệt khám BHYT cho bệnh nhân {4}?",
                            this.GetCardNumber(card), card.gtTheTu, card.gtTheDen, card.maDKBD, patientName);
                        if (XtraMessageBox.Show(message, "Duyệt khám", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                        sdo.PatientTypeId = bhyt.ID;
                        sdo.HisPatientTypeAlter = this.MakeHeinPatientTypeAlter(card);
                    }
                    else
                    {
                        string reason = card == null ? "Không kiểm tra được thẻ trên cổng BHXH."
                            : (!String.IsNullOrWhiteSpace(card.message) ? card.message : card.ghiChu) + (String.IsNullOrWhiteSpace(card.maKetQua) ? "" : " (mã " + card.maKetQua + ")");
                        string message = String.Format("Thẻ BHYT {0} không hợp lệ: {1}\n\nChọn \"Yes\" để chuyển sang khám thu phí, \"No\" để hủy duyệt.", row.HEIN_CARD_NUMBER, reason);
                        if (XtraMessageBox.Show(message, "Duyệt khám", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    }
                }
                else
                {
                    if (XtraMessageBox.Show("Duyệt khám (thu phí) cho bệnh nhân " + patientName + "?", "Duyệt khám", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HIS_APPOINTMENT result = new BackendAdapter(param).Post<HIS_APPOINTMENT>("api/HisAppointment/Approve", ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, result != null);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (result != null)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_PATIENT_TYPE GetPatientType(string configKey)
        {
            string code = HisConfigs.Get<string>(configKey);
            return String.IsNullOrWhiteSpace(code) ? null : BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
        }

        /// <summary>Kiem tra the tren cong BHXH bang thu vien dung chung voi Tiep don (khong hien thong bao trong thu vien).</summary>
        private async System.Threading.Tasks.Task<ResultHistoryLDO> CheckHeinCard(HIS_APPOINTMENT row)
        {
            try
            {
                HeinCardData card = new HeinCardData();
                card.HeinCardNumber = row.HEIN_CARD_NUMBER;
                card.PatientName = row.VIR_PATIENT_NAME;
                card.Dob = row.IS_HAS_NOT_DAY_DOB == 1 ? row.DOB.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(row.DOB);
                card.Gender = row.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? "1" : "2";
                card.Address = row.VIR_ADDRESS ?? row.ADDRESS;

                HeinGOVManager manager = new HeinGOVManager("Cổng BHXH trả về mã lỗi {0}");
                ResultDataADO result = await manager.Check(card, null, false, card.Address, DateTime.Now, false, false);
                return result != null ? result.ResultHistoryLDO : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>Hop le giong Kiosk: ma 000, hoac co the moi thay the cu.</summary>
        private bool IsValidCard(ResultHistoryLDO card)
        {
            if (card == null || String.IsNullOrWhiteSpace(card.maKetQua)) return false;
            return card.maKetQua == GOV_RESULT__OK
                || (!String.IsNullOrEmpty(card.maTheMoi) && (card.maThe != card.maTheMoi || card.maKetQua == GOV_RESULT__NEW_CARD));
        }

        private string GetCardNumber(ResultHistoryLDO card)
        {
            return !String.IsNullOrWhiteSpace(card.maTheMoi) ? card.maTheMoi : card.maThe;
        }

        /// <summary>Thong tin the theo ket qua cong BHXH. Tuyen tinh giong dang ky qua tong dai (BE HisServiceReqExamRegisterDkk).</summary>
        private HIS_PATIENT_TYPE_ALTER MakeHeinPatientTypeAlter(ResultHistoryLDO card)
        {
            bool useNewCard = !String.IsNullOrWhiteSpace(card.maTheMoi) && !String.IsNullOrWhiteSpace(card.gtTheTuMoi);
            HIS_BRANCH branch = BranchDataWorker.Branch;
            string mediOrgCode = useNewCard && !String.IsNullOrWhiteSpace(card.maDKBDMoi) ? card.maDKBDMoi : card.maDKBD;

            HIS_PATIENT_TYPE_ALTER alter = new HIS_PATIENT_TYPE_ALTER();
            alter.HEIN_CARD_NUMBER = this.GetCardNumber(card);
            alter.HEIN_CARD_FROM_TIME = ToTimeNumber(useNewCard ? card.gtTheTuMoi : card.gtTheTu);
            alter.HEIN_CARD_TO_TIME = ToTimeNumber(useNewCard ? card.gtTheDenMoi : card.gtTheDen);
            alter.HEIN_MEDI_ORG_CODE = mediOrgCode;
            HIS_MEDI_ORG mediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == mediOrgCode);
            alter.HEIN_MEDI_ORG_NAME = mediOrg != null ? mediOrg.MEDI_ORG_NAME : mediOrgCode;
            alter.ADDRESS = card.diaChi;
            alter.LIVE_AREA_CODE = card.maKV;
            alter.LEVEL_CODE = branch != null ? branch.HEIN_LEVEL_CODE : null;
            alter.RIGHT_ROUTE_CODE = branch != null && (branch.HEIN_MEDI_ORG_CODE == mediOrgCode || branch.HEIN_LEVEL_CODE == HeinLevelCode.COMMUNE || branch.HEIN_LEVEL_CODE == HeinLevelCode.DISTRICT)
                ? HeinRightRouteCode.TRUE : HeinRightRouteCode.FALSE;
            long? join5Year = ToTimeNumber(card.ngayDu5Nam);
            alter.JOIN_5_YEAR = join5Year.HasValue && join5Year.Value <= Inventec.Common.DateTime.Get.Now().Value ? HeinJoin5YearCode.TRUE : HeinJoin5YearCode.FALSE;
            alter.JOIN_5_YEAR_TIME = join5Year;
            alter.PAID_6_MONTH = HeinPaid6MonthCode.FALSE;
            alter.HAS_BIRTH_CERTIFICATE = HeinHasBirthCertificateCode.FALSE;
            return alter;
        }

        /// <summary>"dd/MM/yyyy" -> yyyyMMdd000000.</summary>
        private static long? ToTimeNumber(string date)
        {
            DateTime value;
            if (String.IsNullOrWhiteSpace(date) || !DateTime.TryParseExact(date.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
            {
                return null;
            }
            return Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(value.Date);
        }

        /// <summary>Lich da tao ho so dieu tri (duyet kham) thi khong cho hoan tac.</summary>
        private bool IsBlockUndo(HIS_APPOINTMENT row)
        {
            if (row != null && row.TREATMENT_ID.HasValue)
            {
                XtraMessageBox.Show("Lịch hẹn đã duyệt khám và tạo hồ sơ điều trị, không hoàn tác được.", "Thông báo");
                return true;
            }
            return false;
        }
    }
}
