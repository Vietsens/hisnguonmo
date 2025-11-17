using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.CheckIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.SecondaryIcd
{
    public partial class frmSecondaryIcd : Form
    {
        List<IcdADO> icdAdoChecks;
        DelegateRefeshIcdChandoanphu delegateIcds;
        string icdCodes;
        string icdNames;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        HIS.Desktop.Plugins.Library.CheckIcd.CheckIcdManager checkIcd;
        HIS_TREATMENT treatment;

        private Dictionary<string, string> codeToFullNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public frmSecondaryIcd(DelegateRefeshIcdChandoanphu delegateIcds, string icdCodes, string icdNames, int _limit, List<HIS_ICD> listIcd, HIS_TREATMENT hisTreatment = null)
        {
            InitializeComponent();
            try
            {
                this.delegateIcds = delegateIcds;
                this.icdCodes = icdCodes;
                this.icdNames = icdNames;
                string[] codes = this.icdCodes.Split(IcdUtil.seperator.ToCharArray());
                var icds = BackendDataWorker.Get<V_HIS_ICD>().Where(o => listIcd.Exists(p => p.ID == o.ID)).ToList();
                icdAdoChecks = (from m in icds select new IcdADO(m, codes)).ToList();
                limit = _limit;
                treatment = hisTreatment;

                InitializeMapping();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmSecondaryIcd(DelegateRefeshIcdChandoanphu delegateIcds, string icdCodes, string icdNames, int _limit, List<V_HIS_ICD> listIcd, HIS_TREATMENT hisTreatment = null)
        {
            InitializeComponent();
            try
            {
                this.delegateIcds = delegateIcds;
                this.icdCodes = icdCodes;
                this.icdNames = icdNames;
                string[] codes = this.icdCodes.Split(IcdUtil.seperator.ToCharArray());
                icdAdoChecks = (from m in listIcd select new IcdADO(m, codes)).ToList();
                limit = _limit;
                treatment = hisTreatment;

                InitializeMapping();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSecondaryDisease_Load(object sender, EventArgs e)
        {
            try
            {
                txtIcdCodes.Text = this.icdCodes;
                txtIcdNames.Text = this.icdNames;
                Language_secondaryDisease();
                dataTotal = (icdAdoChecks.Count);
                FillDataToGrid();
                if (treatment != null)
                {
                    checkIcd = new CheckIcdManager(delegateCheckIcd, treatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void delegateCheckIcd(string icdCodes, string icdNames)
        {
            try
            {
                delegateIcds(icdCodes, icdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                FillDataToGridIcd(new CommonParam(0, (ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : limit)));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridIcd, param, (ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : limit));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridIcd(object param)
        {
            try
            {
                gridControlSecondaryDisease.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                var query = icdAdoChecks.AsQueryable();
                string keyword = txtKeyword.Text.Trim();
                keyword = Inventec.Common.String.Convert.UnSignVNese(keyword.Trim().ToLower());
                if (!String.IsNullOrEmpty(keyword))
                {
                    query = query.Where(o =>
                        Inventec.Common.String.Convert.UnSignVNese((o.ICD_NAME ?? "").ToLower()).Contains(keyword)
                        || o.ICD_CODE.ToLower().Contains(keyword)
                        );
                }
                query = query.OrderByDescending(o => o.IsChecked).ThenBy(o => o.ICD_CODE);
                dataTotal = query.Count();
                var result = query.Skip(start).Take(limit).ToList();
                rowCount = (result == null ? 0 : result.Count);
                gridControlSecondaryDisease.BeginUpdate();
                gridControlSecondaryDisease.DataSource = result;
                gridControlSecondaryDisease.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Language_secondaryDisease()
        {
            try
            {
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.layoutControl1.Text", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.txtIcdCodes.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.txtIcdCodes.Properties.NullValuePrompt", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.txtKeyword.Properties.NullValuePrompt", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.txtIcdNames.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.txtIcdNames.Properties.NullValuePrompt", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.btnChoose.Text", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.gridViewSecondaryDisease.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.gridViewSecondaryDisease.OptionsFind.FindNullPrompt", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.gridColumn1.Caption", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.grdColCode.Caption = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.grdColCode.Caption", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.grdColName.Caption = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.grdColName.Caption", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.lblIcdText.Text = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.lblIcdText.Text", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.bar2.Text", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.bbtnChoose.Caption = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.bbtnChoose.Caption", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.bbtnClose.Caption = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.bbtnClose.Caption", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.Text", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.delegateIcds != null)
                    this.delegateIcds(txtIcdCodes.Text.Trim(), txtIcdNames.Text.Trim());
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlSecondaryDisease_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var disease = (IcdADO)gridViewSecondaryDisease.GetFocusedRow();
                    if (disease != null)
                    {
                        disease.IsChecked = !disease.IsChecked;
                        SetCheckedIcdsToControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlSecondaryDisease_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var disease = (IcdADO)gridViewSecondaryDisease.GetFocusedRow();
                if (disease != null)
                {
                    disease.IsChecked = !disease.IsChecked;
                    gridControlSecondaryDisease.RefreshDataSource();
                    SetCheckedIcdsToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSecondaryDisease_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int keyValue = e.KeyValue;
                if (!e.Shift && keyValue >= (int)Keys.A && keyValue <= (int)Keys.Z)
                {
                    txtKeyword.Text = e.KeyData.ToString();
                    txtKeyword.Focus();
                    txtKeyword.SelectionStart = txtKeyword.Text.Length;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSecondaryDisease_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IsChecked")
                {
                    var row = (IcdADO)gridViewSecondaryDisease.GetFocusedRow();
                    var icd = icdAdoChecks.FirstOrDefault(s => s.ICD_CODE == row.ICD_CODE);
                    if (icd != null)
                    {
                        icd.IsChecked = row.IsChecked;
                    }
                    SetCheckedIcdsToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckedIcdsToControl()
        {
            try
            {
                // BƯỚC 1: Cập nhật  với dữ liệu hiện tại
                var currentCodes = this.txtIcdCodes.Text?.Trim(new char[] { ' ', ';' }).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList() ?? new List<string>();

                var currentNames = this.txtIcdNames.Text?.Trim(new char[] { ' ', ';' })
                                  .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

                // Cập nhật mapping: mã -> tên đầy đủ (cả nội dung nhập thêm nếu có)
                for (int i = 0; i < currentCodes.Count; i++)
                {
                    string code = currentCodes[i];
                    string fullName = i < currentNames.Count ? currentNames[i] : "";

                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        codeToFullNameMap[code] = fullName;
                    }
                }

                // BƯỚC 2: Lấy danh sách mã đang được check
                var currentlyChecked = icdAdoChecks.Where(o => o.IsChecked == true).GroupBy(x => x.ICD_CODE, StringComparer.OrdinalIgnoreCase).Select(g => g.First()).ToList();

                // BƯỚC 3: Sắp xếp - ưu tiên giữ thứ tự cũ
                var ordered = new List<IcdADO>();

                // Giữ thứ tự cũ
                foreach (var code in currentCodes)
                {
                    var found = currentlyChecked.FirstOrDefault(c =>
                        string.Equals(c.ICD_CODE, code, StringComparison.OrdinalIgnoreCase));
                    if (found != null && !ordered.Any(o =>
                        string.Equals(o.ICD_CODE, found.ICD_CODE, StringComparison.OrdinalIgnoreCase)))
                    {
                        ordered.Add(found);
                    }
                }

                // Thêm các mã mới được check (chưa có trong danh sách cũ)
                foreach (var c in currentlyChecked)
                {
                    if (!ordered.Any(o => string.Equals(o.ICD_CODE, c.ICD_CODE, StringComparison.OrdinalIgnoreCase)))
                    {
                        ordered.Add(c);

                        if (!codeToFullNameMap.ContainsKey(c.ICD_CODE))
                        {
                            codeToFullNameMap[c.ICD_CODE] = c.ICD_NAME ?? "";
                        }
                    }
                }

                // BƯỚC 4: Validate với checkIcd
                string messErr = null;
                if (checkIcd != null && ordered.Count > 0 && !checkIcd.ProcessCheckIcd(null,
                    string.Join(";", ordered.Select(s => s.ICD_CODE)), ref messErr, false))
                {
                    XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);

                    if (ordered.Count > 0)
                    {
                        var last = ordered.Last();
                        last.IsChecked = false;
                        ordered.Remove(last);

                        var icdToUncheck = icdAdoChecks.FirstOrDefault(s =>
                            string.Equals(s.ICD_CODE, last.ICD_CODE, StringComparison.OrdinalIgnoreCase));
                        if (icdToUncheck != null)
                        {
                            icdToUncheck.IsChecked = false;
                        }
                    }
                }

                // BƯỚC 5: Tạo danh sách mã và tên mới
                var outCodes = ordered.Select(o => o.ICD_CODE).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var outNames = new List<string>();

                foreach (var code in outCodes)
                {
                    // Ưu tiên lấy từ mapping (có nội dung nhập thêm)
                    if (codeToFullNameMap.TryGetValue(code, out string fullName) && !string.IsNullOrWhiteSpace(fullName))
                    {
                        outNames.Add(fullName);
                    }
                    else
                    {
                        var icdItem = ordered.FirstOrDefault(x =>
                            string.Equals(x.ICD_CODE, code, StringComparison.OrdinalIgnoreCase));
                        string defaultName = icdItem?.ICD_NAME ?? "";
                        outNames.Add(defaultName);

                        codeToFullNameMap[code] = defaultName;
                    }
                }

                // BƯỚC 6: Cập nhật textboxes
                txtIcdCodes.Text = string.Join(";", outCodes);
                txtIcdNames.Text = string.Join(";", outNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // Khởi tạo mapping ban đầu
        private void InitializeMapping()
        {
            try
            {
                var codes = this.icdCodes?.Trim(new char[] { ' ', ';' }).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList() ?? new List<string>();

                var names = this.icdNames?.Trim(new char[] { ' ', ';' })
                           .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

                codeToFullNameMap.Clear();
                for (int i = 0; i < codes.Count; i++)
                {
                    string code = codes[i];
                    string name = i < names.Count ? names[i] : "";

                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        codeToFullNameMap[code] = name;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSecondaryDisease_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    gridViewSecondaryDisease.Focus();
                    gridViewSecondaryDisease.FocusedRowHandle = 0;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnChoose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gridViewSecondaryDisease.IsEditing)
                    gridViewSecondaryDisease.CloseEditor();

                if (gridViewSecondaryDisease.FocusedRowModified)
                    gridViewSecondaryDisease.UpdateCurrentRow();

                btnChoose_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            try
            {
                if (keyData == Keys.Escape)
                {
                    this.Close();
                    return true;
                }

                return base.ProcessDialogKey(keyData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdNames_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
        }
    }
}