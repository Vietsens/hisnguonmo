/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Panel danh sách bên trái (tham khảo HIS.Desktop.Plugins.EnterKskInfomantionQD831):
 * lọc theo mã ĐT/tên BN + khoảng ngày -> grid V_HIS_TREATMENT; click 1 dòng -> nạp lại
 * form chi tiết theo điều trị đó (KHÔNG mở form mới). Nạp danh sách bằng luồng nền để
 * không chặn luồng load dữ liệu chi tiết.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm
{
    public partial class frmInfectiousDiseaseReport
    {
        #region Declare — list panel (control dựng ở Designer.cs)
        private bool listInited = false;

        /// <summary>Dòng danh sách điều trị (giữ tham chiếu V_HIS_TREATMENT để nạp lại chi tiết).</summary>
        private class ListRowADO
        {
            public int STT { get; set; }
            public long ID { get; set; }
            public string TREATMENT_CODE { get; set; }
            public string PATIENT_NAME { get; set; }
            public string ICD_CODE { get; set; }
            public V_HIS_TREATMENT Source { get; set; }
        }
        #endregion

        #region Load list
        /// <summary>Khởi tạo mặc định + nạp danh sách (gọi cuối Load, sau khi chi tiết đã đổ xong).</summary>
        private void InitListPanelData()
        {
            if (listInited) return;
            listInited = true;
            try
            {
                DateTime today = DateTime.Now.Date;
                dteListFrom.EditValue = today;
                dteListTo.EditValue = today;
                LoadListInBackground();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void txtListKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                try { LoadListSync(); } catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            }
        }

        /// <summary>Nạp đồng bộ — dùng cho nút Tìm/Enter.</summary>
        private void LoadListSync()
        {
            try
            {
                string kw; long tfrom, tto;
                ReadListFilter(out kw, out tfrom, out tto);
                var rows = FetchListRows(kw, tfrom, tto);
                BindListRows(rows);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Nạp danh sách bằng luồng nền khi mở form (không chặn load chi tiết).</summary>
        private void LoadListInBackground()
        {
            try
            {
                string kw; long tfrom, tto;
                ReadListFilter(out kw, out tfrom, out tto);   // đọc trên UI thread
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var rows = FetchListRows(kw, tfrom, tto);   // API + build trên thread nền
                        if (this.IsHandleCreated && !this.IsDisposed)
                            this.BeginInvoke(new Action(() => BindListRows(rows)));
                    }
                    catch (Exception exBg) { Inventec.Common.Logging.LogSystem.Error(exBg); }
                });
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Đọc tham số lọc từ control (BẮT BUỘC trên UI thread).</summary>
        private void ReadListFilter(out string kw, out long tfrom, out long tto)
        {
            kw = (txtListKeyword.Text ?? "").Trim();
            tfrom = 0; tto = 0;
            if (dteListFrom.EditValue != null && dteListFrom.DateTime != DateTime.MinValue)
                tfrom = Inventec.Common.TypeConvert.Parse.ToInt64(dteListFrom.DateTime.ToString("yyyyMMdd") + "000000");
            if (dteListTo.EditValue != null && dteListTo.DateTime != DateTime.MinValue)
                tto = Inventec.Common.TypeConvert.Parse.ToInt64(dteListTo.DateTime.ToString("yyyyMMdd") + "235959");
        }

        /// <summary>Gọi API + lọc + dựng danh sách (KHÔNG chạm UI — an toàn thread nền).</summary>
        private List<ListRowADO> FetchListRows(string kw, long tfrom, long tto)
        {
            var infectiousCodes = GetInfectiousIcdCodes();
            var filter = new HisTreatmentViewFilter();
            if (!string.IsNullOrEmpty(kw))
                filter.KEY_WORD = kw;   // tìm server-side theo từ khóa
            if (tfrom > 0) filter.IN_TIME_FROM = tfrom;
            if (tto > 0) filter.IN_TIME_TO = tto;
            filter.ORDER_FIELD = "IN_TIME";
            filter.ORDER_DIRECTION = "DESC";
            filter.ICD_CODE_OR_ICD_SUB_CODEs = infectiousCodes.ToList();
            var param = new CommonParam();
            var data = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(
                "api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param)
                ?? new List<V_HIS_TREATMENT>();
            SessionManager.ProcessTokenLost(param);

            // CHỈ giữ điều trị có ICD chính (ICD_CODE) là bệnh truyền nhiễm (HIS_ICD.IS_INFECTIOUS = 1),
            // dựa trên cache BackendDataWorker.Get<V_HIS_ICD>().
            
            // Từ khóa đã lọc phía server qua KEY_WORD (không lọc lại ở client).

            var rows = new List<ListRowADO>();
            int stt = 0;
            foreach (var o in data)
            {
                stt++;
                rows.Add(new ListRowADO
                {
                    STT = stt,
                    ID = o.ID,
                    TREATMENT_CODE = o.TREATMENT_CODE,
                    PATIENT_NAME = o.TDL_PATIENT_NAME,
                    ICD_CODE = o.ICD_CODE,
                    Source = o
                });
            }
            return rows;
        }

        /// <summary>Tập mã ICD bệnh truyền nhiễm (IS_INFECTIOUS=1, IS_ACTIVE=1) từ cache V_HIS_ICD.</summary>
        private HashSet<string> GetInfectiousIcdCodes()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (var o in BackendDataWorker.Get<V_HIS_ICD>())
                {
                    if (o == null || string.IsNullOrEmpty(o.ICD_CODE)) continue;
                    if (o.IS_INFECTIOUS == 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        set.Add(o.ICD_CODE);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return set;
        }

        private void BindListRows(List<ListRowADO> rows)
        {
            try
            {
                gvList.BeginUpdate();
                grdList.DataSource = rows;
                gvList.EndUpdate();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion

        #region Chọn dòng -> nạp lại chi tiết
        private void gvList_Click(object sender, EventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null) return;
                Point pt = view.GridControl.PointToClient(Control.MousePosition);
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit = view.CalcHitInfo(pt);
                if (hit == null || !hit.InRow || hit.RowHandle < 0) return;

                var row = view.GetRow(hit.RowHandle) as ListRowADO;
                if (row == null || row.Source == null) return;
                ReloadForTreatment(row.Source);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Nạp lại toàn form chi tiết theo 1 điều trị khác (không mở form mới).</summary>
        private void ReloadForTreatment(V_HIS_TREATMENT v)
        {
            try
            {
                if (v == null || v.ID <= 0) return;
                if (treatment != null && treatment.ID == v.ID) return;   // đã nạp điều trị này

                this.SuspendLayout();
                try
                {
                    this.treatment = MapVToTreatment(v);

                    // Reset trạng thái đối soát ECDS trước khi nạp lại.
                    this.ecdsCaseId = null;
                    this.ecdsCaseCode = null;
                    this.hisEcdsCaseId = 0;

                    ClearInputControls();
                    FillDataFromHis();   // header + tab + đối soát theo điều trị mới
                }
                finally { this.ResumeLayout(false); }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private HIS_TREATMENT MapVToTreatment(V_HIS_TREATMENT v)
        {
            var t = new HIS_TREATMENT();
            try
            {
                t.ID = v.ID;
                t.TREATMENT_CODE = v.TREATMENT_CODE;
                t.PATIENT_ID = v.PATIENT_ID;
                t.TDL_PATIENT_CODE = v.TDL_PATIENT_CODE;
                t.TDL_PATIENT_NAME = v.TDL_PATIENT_NAME;
                t.TDL_PATIENT_DOB = v.TDL_PATIENT_DOB;
                t.TDL_PATIENT_GENDER_ID = v.TDL_PATIENT_GENDER_ID;
                t.TDL_PATIENT_GENDER_NAME = v.TDL_PATIENT_GENDER_NAME;
                t.ICD_CODE = v.ICD_CODE;
                t.ICD_NAME = v.ICD_NAME;
                t.IN_TIME = v.IN_TIME;
                t.OUT_TIME = v.OUT_TIME;
                t.LAST_DEPARTMENT_ID = v.LAST_DEPARTMENT_ID;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return t;
        }
        #endregion
    }
}
