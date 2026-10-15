using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.PatientInfo
{
    /// <summary>
    /// Dong thong tin hanh chinh benh nhan tren cung form duyet.
    /// </summary>
    internal static class PatientInfoHeader
    {
        internal static HIS_TREATMENT GetTreatment(long? treatmentId)
        {
            HIS_TREATMENT result = null;
            try
            {
                if (!treatmentId.HasValue || treatmentId.Value <= 0)
                    return null;
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = treatmentId.Value;
                var treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param);
                result = treatments != null ? treatments.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static string BuildText(V_HIS_SPECIALIST_EXAM exam, HIS_TREATMENT treatment)
        {
            if (exam == null)
                return "";
            string dob = "";
            if (exam.TDL_PATIENT_DOB.HasValue && exam.TDL_PATIENT_DOB.Value > 0)
            {
                bool isOnlyYear = treatment != null && treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1;
                dob = isOnlyYear
                    ? exam.TDL_PATIENT_DOB.Value.ToString().Substring(0, 4)
                    : Inventec.Common.DateTime.Convert.TimeNumberToDateString(exam.TDL_PATIENT_DOB.Value);
            }
            string departmentName = "";
            if (treatment != null && treatment.LAST_DEPARTMENT_ID.HasValue)
            {
                var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == treatment.LAST_DEPARTMENT_ID.Value);
                departmentName = department != null ? department.DEPARTMENT_NAME : "";
            }
            string bed = String.Join(" - ", new[] { exam.BED_ROOM_NAME, exam.BED_NAME }.Where(o => !String.IsNullOrWhiteSpace(o)));

            const string SEPARATOR = "      ";
            StringBuilder sb = new StringBuilder();
            sb.Append(Item("Họ tên", exam.TDL_PATIENT_NAME)).Append(SEPARATOR)
                .Append(Item("Ngày sinh", dob)).Append(SEPARATOR)
                .Append(Item("Giới tính", exam.TDL_PATIENT_GENDER_NAME)).Append(SEPARATOR)
                .Append(Item("Mã BN", exam.PATIENT_CODE)).Append(SEPARATOR)
                .Append(Item("Mã điều trị", exam.TREATMENT_CODE)).Append(SEPARATOR)
                .Append(Item("Khoa điều trị", departmentName))
                .Append("<br>")
                .Append(Item("Buồng, Giường lúc mời", bed));
            return sb.ToString();
        }

        /// <summary>
        /// Gan dong thong tin len dau form, phia tren control chinh (Dock Fill) cua form.
        /// </summary>
        internal static LabelControl AddToForm(Form form, Control fillControl, string text)
        {
            LabelControl lbl = new LabelControl();
            lbl.Name = "lblPatientInfo";
            lbl.AllowHtmlString = true;
            lbl.AutoSizeMode = LabelAutoSizeMode.Vertical;
            lbl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            lbl.Appearance.ForeColor = Color.FromArgb(0, 50, 110);
            lbl.Appearance.Options.UseForeColor = true;
            lbl.Padding = new Padding(10, 6, 10, 6);
            lbl.Dock = DockStyle.Top;
            lbl.Text = text;
            form.Controls.Add(lbl);
            //Thu tu dock: control Dock Top phai dock truoc control Fill
            lbl.BringToFront();
            fillControl.BringToFront();
            return lbl;
        }

        private static string Item(string caption, string value)
        {
            return String.Format("<b>{0}:</b> {1}", caption, Escape(value));
        }

        private static string Escape(string value)
        {
            if (String.IsNullOrEmpty(value))
                return "";
            return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}
