using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.KskServiceEditList.ADO;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskServiceEditList
{
    /// <summary>
    /// 58013 - Kết quả sửa dịch vụ theo từng bệnh nhân, từng thao tác; xuất Excel để xử lý riêng các trường hợp lỗi.
    /// </summary>
    public partial class frmKskServiceEditResult : HIS.Desktop.Utility.FormBase
    {
        private KskServiceEditBatchResultADO result;
        private List<KskServiceEditResultRowADO> rows = new List<KskServiceEditResultRowADO>();

        public frmKskServiceEditResult(Inventec.Desktop.Common.Modules.Module module, KskServiceEditBatchResultADO result)
            : base(module)
        {
            InitializeComponent();
            this.result = result;
        }

        private void frmKskServiceEditResult_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetIcon();
                this.SetCaptionByLanguageKey();
                this.lblSummary.Text = this.BuildSummaryText();
                this.rows = this.BuildRows();
                this.gridViewResult.BeginUpdate();
                try
                {
                    this.gridControlResult.DataSource = this.rows;
                }
                finally
                {
                    this.gridViewResult.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.KskServiceEditList.Resources.Lang", typeof(frmKskServiceEditResult).Assembly);
                this.Text = GetLang("frmKskServiceEditResult.Text");
                this.lciSummary.Text = GetLang("frmKskServiceEditResult.lciSummary.Text");
                this.btnExport.Text = GetLang("frmKskServiceEditResult.btnExport.Text");
                this.btnClose.Text = GetLang("frmKskServiceEditResult.btnClose.Text");
                this.gcResStt.Caption = GetLang("frmKskServiceEditResult.gcResStt.Caption");
                this.gcResTreatmentCode.Caption = GetLang("frmKskServiceEditResult.gcResTreatmentCode.Caption");
                this.gcResPatientName.Caption = GetLang("frmKskServiceEditResult.gcResPatientName.Caption");
                this.gcResAction.Caption = GetLang("frmKskServiceEditResult.gcResAction.Caption");
                this.gcResServiceCode.Caption = GetLang("frmKskServiceEditResult.gcResServiceCode.Caption");
                this.gcResServiceName.Caption = GetLang("frmKskServiceEditResult.gcResServiceName.Caption");
                this.gcResResult.Caption = GetLang("frmKskServiceEditResult.gcResResult.Caption");
                this.gcResDescription.Caption = GetLang("frmKskServiceEditResult.gcResDescription.Caption");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLang(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        }

        private string BuildSummaryText()
        {
            KskServiceEditSummarySDO s = this.result.Summary;
            string text = String.Format(Resources.ResourceMessage.TongHopKetQua,
                s.TotalTreatment, s.SuccessTreatment, s.ErrorTreatment,
                s.AddedCount, s.SkippedDuplicateCount, s.DeletedCount, s.NotDeletedCount,
                s.ChangedRoomCount, s.NotChangedRoomCount);
            if (this.result.NotProcessedCount > 0)
            {
                text += Environment.NewLine + String.Format(Resources.ResourceMessage.ChuaXuLyDoMatPhien, this.result.NotProcessedCount);
            }
            return text;
        }

        /// <summary>
        /// Tính sẵn chuỗi hiển thị; sắp xếp lỗi lên đầu để người dùng xử lý
        /// </summary>
        private List<KskServiceEditResultRowADO> BuildRows()
        {
            return this.result.Results.Select(o => new KskServiceEditResultRowADO
            {
                TreatmentCode = o.TreatmentCode,
                PatientName = o.PatientName,
                ActionDisplay = this.GetActionDisplay(o.Action),
                ServiceCode = o.ServiceCode,
                ServiceName = o.ServiceName,
                ResultType = o.ResultType,
                ResultDisplay = this.GetResultDisplay(o.ResultType),
                Description = o.Descriptions != null ? String.Join("; ", o.Descriptions.Distinct()) : ""
            })
            .OrderByDescending(o => o.ResultType == (short)EnumKskServiceEditResultType.Error)
            .ThenBy(o => o.TreatmentCode)
            .ToList();
        }

        private string GetActionDisplay(string action)
        {
            if (action == KskServiceEditAction.ADD) return Resources.ResourceMessage.ThaoTacThem;
            if (action == KskServiceEditAction.DELETE) return Resources.ResourceMessage.ThaoTacXoa;
            if (action == KskServiceEditAction.CHANGE_ROOM) return Resources.ResourceMessage.ThaoTacDoiPhong;
            return "";
        }

        private string GetResultDisplay(short resultType)
        {
            switch ((EnumKskServiceEditResultType)resultType)
            {
                case EnumKskServiceEditResultType.Success: return Resources.ResourceMessage.KetQuaThanhCong;
                case EnumKskServiceEditResultType.Skipped: return Resources.ResourceMessage.KetQuaBoQua;
                case EnumKskServiceEditResultType.Error: return Resources.ResourceMessage.KetQuaLoi;
                default: return "";
            }
        }

        private void gridViewResult_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column == this.gcResStt)
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewResult_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.Column != this.gcResResult) return;
                KskServiceEditResultRowADO row = this.gridViewResult.GetRow(e.RowHandle) as KskServiceEditResultRowADO;
                if (row == null) return;
                if (row.ResultType == (short)EnumKskServiceEditResultType.Error) e.Appearance.ForeColor = Color.Red;
                else if (row.ResultType == (short)EnumKskServiceEditResultType.Success) e.Appearance.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "Excel (*.xlsx)|*.xlsx";
                    dialog.FileName = "KetQuaSuaDichVuKsk_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    if (dialog.ShowDialog() != DialogResult.OK) return;

                    this.gridControlResult.ExportToXlsx(dialog.FileName);
                    if (XtraMessageBox.Show(Resources.ResourceMessage.XuatFileThanhCongMoFile,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(dialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
