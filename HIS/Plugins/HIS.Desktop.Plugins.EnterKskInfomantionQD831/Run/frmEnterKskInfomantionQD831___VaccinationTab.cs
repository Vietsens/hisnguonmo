/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using DevExpress.XtraGrid;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.Run
{
    /// <summary>
    /// Tab "C. Tiêm chủng" (UI dựng trong Designer): 3 panel, mỗi panel 1 GridControl.
    /// Panel 1 & 2: Loại vắc xin (khóa) | Chưa chủng ngừa (check) | Đã chủng ngừa - ngày (dd/MM/yyyy) |
    ///              Phản ứng sau tiêm | Ngày hẹn tiêm (dd/MM/yyyy). Panel 1 có footer "Số mũi vắc xin uốn ván...".
    /// Panel 3: Nội dung (khóa) | Chưa tiêm (check) | Tháng thai | Phản ứng sau tiêm | Ngày hẹn tiêm.
    /// File này chỉ gán DataSource (data) cho grid — UI là ở Designer.
    /// </summary>
    public partial class frmEnterKskInfomantionQD831
    {
        internal class TcVaccineRow
        {
            public long VaccineTypeId { get; set; } // HIS_VACCINE_TYPE.ID
            public string VaccineCode { get; set; } // HIS_VACCINE_TYPE.VACCINE_TYPE_CODE
            public short VaccineGroup { get; set; }  // TYPE_VACCINE (1/2/3) -> HIS_HEALTH_VACCINATION.VACCINE_GROUP
            public string VaccineName { get; set; }
            public bool IsNotVaccinated { get; set; }
            public DateTime? VaccinatedTime { get; set; }
            public string Reaction { get; set; }
            public DateTime? AppointmentTime { get; set; }
        }

        internal class TcContentRow
        {
            public long VaccineTypeId { get; set; } // HIS_VACCINE_TYPE.ID
            public string VaccineCode { get; set; } // HIS_VACCINE_TYPE.VACCINE_TYPE_CODE
            public short VaccineGroup { get; set; }  // TYPE_VACCINE (3) -> HIS_HEALTH_VACCINATION.VACCINE_GROUP
            public string VaccineName { get; set; }
            public bool IsNotVaccinated { get; set; }
            public string PregnancyMonth { get; set; }
            public string Reaction { get; set; }
            public DateTime? AppointmentTime { get; set; }
        }

        /// <summary>
        /// (KHÔNG gọi lúc Load — UI grid/cột đã nằm trong Designer.)
        /// Gọi hàm này khi có danh mục vắc xin thực tế để gán DataSource cho 3 grid tab Tiêm chủng.
        /// </summary>
        private void LoadTiemChungData()
        {
            try
            {
                var all = BackendDataWorker.Get<HIS_VACCINE_TYPE>();
                var src = all != null
                    ? all.Where(v => v.IS_DELETE == null || v.IS_DELETE == 0).OrderBy(v => v.ID).ToList()
                    : new List<HIS_VACCINE_TYPE>();

                // TYPE_VACCINE: 1 = Tiêm chủng cơ bản cho trẻ em, 2 = Tiêm chủng ngoài TCMR, 3 = UV cho phụ nữ có thai
                this.gcTcVaccine1.DataSource = BuildTcVaccineRows(src, 1);
                this.gcTcVaccine2.DataSource = BuildTcVaccineRows(src, 2);
                this.gcTcVaccine3.DataSource = BuildTcContentRows(src, 3);

                // Header wrap 2 dòng, căn giữa (trừ tên), cột "Phản ứng sau tiêm" = MemoEdit rộng + nhiều dòng.
                ConfigVaccineGrid(this.gcTcVaccine1, "VaccineName");
                ConfigVaccineGrid(this.gcTcVaccine2, "VaccineName");
                ConfigVaccineGrid(this.gcTcVaccine3, "VaccineName");

                // Logic "Chưa chủng ngừa/Chưa tiêm" <-> các cột khác (khóa/xóa/đồng bộ).
                WireVaccineGridLogic(this.gcTcVaccine1, "IsNotVaccinated", "VaccinatedTime", new string[] { "VaccinatedTime", "Reaction" });
                WireVaccineGridLogic(this.gcTcVaccine2, "IsNotVaccinated", "VaccinatedTime", new string[] { "VaccinatedTime", "Reaction" });
                WireVaccineGridLogic(this.gcTcVaccine3, "IsNotVaccinated", "PregnancyMonth", new string[] { "PregnancyMonth", "Reaction" });
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private bool suppressVaccineCellEvent = false;

        /// <summary>
        /// Nối logic cho 1 grid tiêm chủng:
        ///  - Khi checkField (Chưa chủng ngừa/Chưa tiêm) = true: xóa + khóa (tô xám) các cột lockedFields.
        ///  - Khi nhập doneField (Ngày đã chủng ngừa / Tháng thai) có giá trị: tự bỏ tích checkField.
        ///  - "Ngày hẹn tiêm" KHÔNG khóa, KHÔNG tự xóa (theo yêu cầu).
        /// </summary>
        private void WireVaccineGridLogic(DevExpress.XtraGrid.GridControl grid, string checkField, string doneField, string[] lockedFields)
        {
            try
            {
                if (grid == null) return;
                var gv = grid.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gv == null) return;

                gv.CellValueChanged += (s, e) =>
                {
                    if (suppressVaccineCellEvent) return;
                    try
                    {
                        suppressVaccineCellEvent = true;
                        if (e.Column.FieldName == checkField)
                        {
                            bool chua = e.Value != null && Convert.ToBoolean(e.Value);
                            if (chua)
                                foreach (var f in lockedFields)
                                    if (gv.Columns[f] != null) gv.SetRowCellValue(e.RowHandle, gv.Columns[f], null);
                        }
                        else if (e.Column.FieldName == doneField)
                        {
                            bool hasVal = e.Value != null && !string.IsNullOrEmpty(Convert.ToString(e.Value).Trim());
                            if (hasVal && gv.Columns[checkField] != null)
                                gv.SetRowCellValue(e.RowHandle, gv.Columns[checkField], false);
                        }
                    }
                    catch (Exception ex) { LogSystem.Warn(ex); }
                    finally { suppressVaccineCellEvent = false; try { gv.RefreshRow(e.RowHandle); } catch { } }
                };

                gv.ShowingEditor += (s, e) =>
                {
                    try
                    {
                        var view = s as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (view == null || view.FocusedColumn == null) return;
                        if (Array.IndexOf(lockedFields, view.FocusedColumn.FieldName) < 0) return;
                        var val = view.GetRowCellValue(view.FocusedRowHandle, checkField);
                        if (val != null && Convert.ToBoolean(val)) e.Cancel = true; // đang "chưa" -> khóa ô
                    }
                    catch { }
                };

                gv.RowCellStyle += (s, e) =>
                {
                    try
                    {
                        if (Array.IndexOf(lockedFields, e.Column.FieldName) < 0) return;
                        var val = gv.GetRowCellValue(e.RowHandle, checkField);
                        if (val != null && Convert.ToBoolean(val))
                        {
                            e.Appearance.BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            e.Appearance.Options.UseBackColor = true;
                        }
                    }
                    catch { }
                };
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>
        /// Cấu hình cột grid tiêm chủng:
        ///  - Header cho wrap 2 dòng; căn giữa header (trừ cột tên).
        ///  - Cột ngày/check/tháng thai: hẹp lại (FixedWidth) + căn giữa cell.
        ///  - Cột "Phản ứng sau tiêm" (mô tả): MemoEdit nhiều dòng, giãn rộng (không FixedWidth), cell căn trái.
        ///  - RowAutoHeight = true để dòng tự cao khi memo nhiều dòng.
        /// </summary>
        private void ConfigVaccineGrid(DevExpress.XtraGrid.GridControl grid, string nameField)
        {
            try
            {
                if (grid == null) return;
                var gv = grid.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gv == null) return;

                // Chiều cao dòng CỐ ĐỊNH (min) để ô mô tả đã cao sẵn ~2-3 dòng, không cần gõ mới cao.
                gv.OptionsView.RowAutoHeight = false;
                gv.RowHeight = 46;
                gv.ColumnPanelRowHeight = 34;          // header đủ chỗ 2 dòng
                // In đậm header.
                gv.Appearance.HeaderPanel.FontStyleDelta = System.Drawing.FontStyle.Bold;
                gv.Appearance.HeaderPanel.Options.UseFont = true;

                // Cột mô tả -> MemoEdit (nhiều dòng), giãn rộng.
                var colDesc = gv.Columns["Reaction"];
                if (colDesc != null)
                {
                    var repoMemo = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    repoMemo.WordWrap = true;
                    grid.RepositoryItems.Add(repoMemo);
                    colDesc.ColumnEdit = repoMemo;
                    colDesc.OptionsColumn.FixedWidth = false;
                }

                // Cột thời gian/check/tháng thai hẹp lại để chừa chỗ cho cột mô tả.
                SetColWidthFixed(gv, "IsNotVaccinated", 74);
                SetColWidthFixed(gv, "IsNotVaccinated", 74);
                SetColWidthFixed(gv, "VaccinatedTime", 88);
                SetColWidthFixed(gv, "PregnancyMonth", 70);
                SetColWidthFixed(gv, "AppointmentTime", 88);

                foreach (DevExpress.XtraGrid.Columns.GridColumn col in gv.Columns)
                {
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    col.AppearanceHeader.TextOptions.HAlignment = (col.FieldName == nameField)
                        ? DevExpress.Utils.HorzAlignment.Near : DevExpress.Utils.HorzAlignment.Center;
                    // Căn giữa cell cho các cột hẹp; cột tên + cột mô tả để căn trái cho dễ đọc.
                    if (col.FieldName != nameField && col.FieldName != "Reaction")
                    {
                        col.AppearanceCell.Options.UseTextOptions = true;
                        col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    }
                }
                gv.OptionsView.ColumnAutoWidth = true; // cột mô tả (không fixed) chiếm phần rộng còn lại
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private void SetColWidthFixed(DevExpress.XtraGrid.Views.Grid.GridView gv, string field, int width)
        {
            var c = gv.Columns[field];
            if (c == null) return;
            c.Width = width;
            c.MinWidth = width;
            c.OptionsColumn.FixedWidth = true;
        }

        private BindingList<TcVaccineRow> BuildTcVaccineRows(List<HIS_VACCINE_TYPE> src, int typeVaccine)
        {
            var rows = new BindingList<TcVaccineRow>();
            foreach (var v in src.Where(v => (v.TYPE_VACCINE ?? 0) == typeVaccine))
                rows.Add(new TcVaccineRow { VaccineTypeId = v.ID, VaccineCode = (v.VACCINE_TYPE_CODE ?? "").Trim(), VaccineGroup = (short)typeVaccine, VaccineName = (v.VACCINE_TYPE_NAME ?? "").Trim() });
            return rows;
        }

        private BindingList<TcContentRow> BuildTcContentRows(List<HIS_VACCINE_TYPE> src, int typeVaccine)
        {
            var rows = new BindingList<TcContentRow>();
            foreach (var v in src.Where(v => (v.TYPE_VACCINE ?? 0) == typeVaccine))
                rows.Add(new TcContentRow { VaccineTypeId = v.ID, VaccineCode = (v.VACCINE_TYPE_CODE ?? "").Trim(), VaccineGroup = (short)typeVaccine, VaccineName = (v.VACCINE_TYPE_NAME ?? "").Trim() });
            return rows;
        }

        /// <summary>Gom PartC (tiêm chủng) -> List&lt;HIS_HEALTH_VACCINATION&gt;. Mỗi loại vắc xin = 1 dòng. KHÔNG POST.</summary>
        private List<HIS_HEALTH_VACCINATION> CollectHealthVaccinations()
        {
            var list = new List<HIS_HEALTH_VACCINATION>();
            try
            {
                CollectVaccineGrid(this.gcTcVaccine1, list);
                CollectVaccineGrid(this.gcTcVaccine2, list);
                CollectContentGrid(this.gcTcVaccine3, list);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
            return list;
        }

        /// <summary>Grid 1/2 (TcVaccineRow): chưa chủng ngừa / ngày đã chủng / phản ứng / ngày hẹn.</summary>
        private void CollectVaccineGrid(GridControl grid, List<HIS_HEALTH_VACCINATION> list)
        {
            var rows = grid != null ? grid.DataSource as BindingList<TcVaccineRow> : null;
            if (rows == null) return;
            foreach (var r in rows)
            {
                if (r == null) continue;
                var h = NewHealthVaccination(r.VaccineCode, r.VaccineName, r.VaccineGroup);
                h.IS_NOT_VACCINATED = (short)(r.IsNotVaccinated ? 1 : 0);
                h.VACCINATED_TIME = DateToNum(r.VaccinatedTime);
                h.REACTION = Nz(r.Reaction);
                h.APPOINTMENT_TIME = DateToNum(r.AppointmentTime);
                list.Add(h);
            }
        }

        /// <summary>Grid 3 (TcContentRow — UV thai): chưa tiêm / tháng thai / phản ứng / ngày hẹn.</summary>
        private void CollectContentGrid(GridControl grid, List<HIS_HEALTH_VACCINATION> list)
        {
            var rows = grid != null ? grid.DataSource as BindingList<TcContentRow> : null;
            if (rows == null) return;
            foreach (var r in rows)
            {
                if (r == null) continue;
                var h = NewHealthVaccination(r.VaccineCode, r.VaccineName, r.VaccineGroup);
                h.IS_NOT_VACCINATED = (short)(r.IsNotVaccinated ? 1 : 0);
                h.PREGNANCY_MONTH = ParseShortStr(r.PregnancyMonth);
                h.REACTION = Nz(r.Reaction);
                h.APPOINTMENT_TIME = DateToNum(r.AppointmentTime);
                list.Add(h);
            }
        }

        /// <summary>Tạo 1 HIS_HEALTH_VACCINATION với loại vắc xin + liên kết y lệnh (các trường trạng thái set ở caller).</summary>
        private HIS_HEALTH_VACCINATION NewHealthVaccination(string vaccineCode, string vaccineName, short vaccineGroup)
        {
            var h = new HIS_HEALTH_VACCINATION
            {
                VACCINE_CODE = Nz(vaccineCode),
                VACCINE_NAME = Nz(vaccineName),
                VACCINE_GROUP = vaccineGroup,
                IS_ACTIVE = 1,
                IS_DELETE = 0
            };
            if (currentServiceReq != null)
            {
                h.PATIENT_ID = currentServiceReq.TDL_PATIENT_ID;
                h.SERVICE_REQ_ID = currentServiceReq.ID;
                h.TREATMENT_ID = currentServiceReq.TREATMENT_ID;
            }
            return h;
        }

        private static string Nz(string s)
        {
            var t = (s ?? "").Trim();
            return t.Length > 0 ? t : null;
        }

        private static long? DateToNum(DateTime? dt)
        {
            if (!dt.HasValue) return null;
            try { return Convert.ToInt64(dt.Value.ToString("yyyyMMddHHmmss")); }
            catch { return null; }
        }

        private static short? ParseShortStr(string s)
        {
            short v;
            return short.TryParse((s ?? "").Trim(), out v) ? (short?)v : null;
        }

    }
}
