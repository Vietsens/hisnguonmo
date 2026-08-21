/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tab "Ksk dưới 18 tuổi" — mục "b. Tiêm chủng": đặt giá trị MẶC ĐỊNH điền sẵn cho lưới
 * (gridControl1 / VaccineTypeADO -> HIS_KSK_UNEI_VATY.CONDITION_TYPE).
 *
 * Control: 1 CheckEdit 3 trạng thái (Properties.AllowGrayed = true) dùng như toggle —
 * mỗi lần bấm xoay vòng Không -> Có -> Không nhớ -> Không, khớp đúng 3 cột của lưới.
 * DevExpress 15.2 ToggleSwitch chỉ có 2 trạng thái (OnText/OffText) nên không dùng được.
 * Caption đổi theo trạng thái để người dùng đọc được nghĩa, không phải đoán ô xám.
 *
 * Lựa chọn được nhớ theo MÁY qua ControlStateWorker (khuôn frmAutoClsSetting.cs).
 * Máy chưa từng đặt -> KHÔNG áp mặc định (giữ nguyên hành vi cũ).
 *
 * Chỉ áp cho bản ghi MỚI (SetDafaultGrid) và chỉ điền dòng chưa tích gì — không ghi đè
 * dữ liệu đã lưu của bác sĩ.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.Library.CacheClient;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private const string VACCINE_DEFAULT_MODULE_LINK = "HIS.Desktop.Plugins.EnterKskInfomantionVer2";
        private const string VACCINE_DEFAULT_STATE_KEY = "chkDefaultVaccine3";

        private ControlStateWorker vaccineDefaultStateWorker;
        private List<ControlStateRDO> vaccineDefaultStates;
        private bool vaccineDefaultInitializing;

        #region Khởi tạo + lưu trạng thái

        /// <summary>Đọc mặc định đã lưu ở máy và dựng lại trạng thái toggle. Idempotent.</summary>
        private void InitDefaultVaccineToggle()
        {
            try
            {
                if (vaccineDefaultStateWorker != null) return;
                vaccineDefaultInitializing = true;
                vaccineDefaultStateWorker = new ControlStateWorker();
                vaccineDefaultStates = vaccineDefaultStateWorker.GetData(VACCINE_DEFAULT_MODULE_LINK)
                                       ?? new List<ControlStateRDO>();

                var saved = vaccineDefaultStates.FirstOrDefault(
                    o => o.KEY == VACCINE_DEFAULT_STATE_KEY && o.MODULE_LINK == VACCINE_DEFAULT_MODULE_LINK);
                // Chưa từng đặt -> để Unchecked nhưng KHÔNG coi là mặc định "Không" (xem DefaultVaccineCondition).
                if (saved != null && chkDefaultVaccine3 != null)
                {
                    if (saved.VALUE == "1") chkDefaultVaccine3.CheckState = CheckState.Checked;
                    else if (saved.VALUE == "3") chkDefaultVaccine3.CheckState = CheckState.Indeterminate;
                    else chkDefaultVaccine3.CheckState = CheckState.Unchecked;
                }
                UpdateDefaultVaccineCaption();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            finally { vaccineDefaultInitializing = false; }
        }

        private void chkDefaultVaccine3_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                // Phải lưu ControlState TRƯỚC rồi mới đổi caption: caption đọc DefaultVaccineCondition()
                // nên nếu đổi trước thì lần bấm đầu tiên vẫn hiện "(chưa đặt)".
                if (vaccineDefaultInitializing || vaccineDefaultStateWorker == null)
                {
                    UpdateDefaultVaccineCaption();
                    return;
                }

                string value = CheckStateToCondition(chkDefaultVaccine3.CheckState).ToString();
                var item = vaccineDefaultStates.FirstOrDefault(
                    o => o.KEY == VACCINE_DEFAULT_STATE_KEY && o.MODULE_LINK == VACCINE_DEFAULT_MODULE_LINK);
                if (item != null) item.VALUE = value;
                else vaccineDefaultStates.Add(new ControlStateRDO()
                {
                    KEY = VACCINE_DEFAULT_STATE_KEY,
                    VALUE = value,
                    MODULE_LINK = VACCINE_DEFAULT_MODULE_LINK
                });
                vaccineDefaultStateWorker.SetData(vaccineDefaultStates);
                UpdateDefaultVaccineCaption();

                // Người dùng CHỦ ĐỘNG bấm -> áp ngay xuống lưới đang mở để thấy kết quả.
                ApplyDefaultVaccineToGridNow();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void UpdateDefaultVaccineCaption()
        {
            try
            {
                if (chkDefaultVaccine3 == null) return;
                // Máy chưa từng bấm -> nói thật là "chưa đặt", tránh hiểu nhầm ô trống = mặc định "Không".
                if (DefaultVaccineCondition() == null)
                {
                    chkDefaultVaccine3.Properties.Caption = "Mặc định: (chưa đặt)";
                    return;
                }
                switch (CheckStateToCondition(chkDefaultVaccine3.CheckState))
                {
                    case 1: chkDefaultVaccine3.Properties.Caption = "Mặc định: Có"; break;
                    case 3: chkDefaultVaccine3.Properties.Caption = "Mặc định: Không nhớ"; break;
                    default: chkDefaultVaccine3.Properties.Caption = "Mặc định: Không"; break;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Checked = Có (1), Indeterminate = Không nhớ (3), Unchecked = Không (2).</summary>
        private long CheckStateToCondition(CheckState state)
        {
            if (state == CheckState.Checked) return 1;
            if (state == CheckState.Indeterminate) return 3;
            return 2;
        }

        #endregion

        #region Áp mặc định xuống lưới

        /// <summary>
        /// Giá trị mặc định đang chọn (1 Có / 2 Không / 3 Không nhớ), null nếu máy này chưa
        /// từng đặt mặc định -> không điền sẵn gì (giữ hành vi cũ).
        /// </summary>
        private long? DefaultVaccineCondition()
        {
            try
            {
                if (chkDefaultVaccine3 == null || vaccineDefaultStates == null) return null;
                var saved = vaccineDefaultStates.FirstOrDefault(
                    o => o.KEY == VACCINE_DEFAULT_STATE_KEY && o.MODULE_LINK == VACCINE_DEFAULT_MODULE_LINK);
                if (saved == null || string.IsNullOrEmpty(saved.VALUE)) return null;
                return CheckStateToCondition(chkDefaultVaccine3.CheckState);
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>Điền mặc định cho các dòng CHƯA tích gì (không ghi đè dòng đã có giá trị).</summary>
        private void ApplyDefaultVaccine(List<ADO.VaccineTypeADO> rows)
        {
            try
            {
                long? condition = DefaultVaccineCondition();
                if (condition == null || rows == null || rows.Count == 0) return;
                foreach (var row in rows)
                {
                    if (row == null || row.IS_YES || row.IS_NO || row.IS_FORGOT) continue;
                    row.IS_YES = condition == 1;
                    row.IS_NO = condition == 2;
                    row.IS_FORGOT = condition == 3;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Bấm toggle = hành động chủ động -> đặt lại TOÀN BỘ các dòng đang hiển thị theo mặc định
        /// vừa chọn (ghi đè cả dòng đã tích), rồi bind lại để 3 cột check vẽ lại.
        /// Khác với ApplyDefaultVaccine (chạy tự động lúc dựng lưới, chỉ điền dòng trống).
        /// </summary>
        private void ApplyDefaultVaccineToGridNow()
        {
            try
            {
                if (chkDefaultVaccine3 == null || gridControl1 == null) return;
                var rows = gridControl1.DataSource as List<ADO.VaccineTypeADO>;
                if (rows == null || rows.Count == 0) return;

                long condition = CheckStateToCondition(chkDefaultVaccine3.CheckState);
                foreach (var row in rows)
                {
                    if (row == null) continue;
                    row.IS_YES = condition == 1;
                    row.IS_NO = condition == 2;
                    row.IS_FORGOT = condition == 3;
                }

                // VaccineTypeADO không có INotifyPropertyChanged -> phải bind lại như khuôn ReloadGrid,
                // RefreshData() đơn thuần không đủ cho cột dùng RepositoryItemCheckEdit.
                int focus = gridView1 != null ? gridView1.FocusedRowHandle : -1;
                gridControl1.DataSource = new List<ADO.VaccineTypeADO>();
                gridControl1.DataSource = rows;
                if (gridView1 != null && focus >= 0 && focus < rows.Count) gridView1.FocusedRowHandle = focus;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion
    }
}
