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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.TrackingCreate.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TrackingCreate
{
    /// <summary>
    /// Man hinh chon che do an (Tai lieu 3427). Mo bang F2 tai o Theo doi cham soc,
    /// hoac mo noi tiep sau man hinh chon loai cham soc (F1) khi tick "Chon che do an".
    /// Dong y -> tra ve chuoi cac dong "Che do an: ..." qua dataSelect; dong man hinh hoac
    /// khong tick gi thi khong goi dataSelect, o Theo doi cham soc giu nguyen.
    /// </summary>
    public partial class frmChonCheDoAn : Form
    {
        HIS.Desktop.Common.DelegateSelectData dataSelect;

        /// <summary>Toan bo che do an dang hoat dong, giu trang thai tick khi loc tim kiem</summary>
        List<DietADO> listDiet = new List<DietADO>();

        /// <summary>Bo dem thu tu tick, chi tang</summary>
        int checkCounter = 0;

        public frmChonCheDoAn(HIS.Desktop.Common.DelegateSelectData dataSelect)
        {
            InitializeComponent();
            this.dataSelect = dataSelect;
        }

        private void frmChonCheDoAn_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetIconFrm();
                LoadDataDiet();
                FillDataToGrid();
                txtKeyWord.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDiet()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDietFilter filter = new MOS.Filter.HisDietFilter();
                //Che do an bi khoa khong hien thi de chon
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                List<HIS_DIET> data = new BackendAdapter(param).Get<List<HIS_DIET>>("api/HisDiet/Get", ApiConsumers.MosConsumer, filter, param);

                listDiet = (data ?? new List<HIS_DIET>())
                    .OrderBy(o => o.DIET_CODE)
                    .Select(o => new DietADO(o))
                    .ToList();
            }
            catch (Exception ex)
            {
                listDiet = new List<DietADO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Loc theo Ma che do an hoac Ap dung dieu tri (vd "DN", "roi loan nuot"), go khong dau van ra.
        /// Loc tren listDiet trong bo nho nen cac dong da tick van giu nguyen trang thai.
        /// </summary>
        private void FillDataToGrid()
        {
            try
            {
                string needle = NormalizeForSearch(txtKeyWord.Text);
                List<DietADO> source = listDiet;
                if (needle.Length > 0)
                {
                    source = listDiet.Where(o => NormalizeForSearch(o.DIET_CODE).Contains(needle)
                        || NormalizeForSearch(o.TREATMENT_APPLY).Contains(needle)).ToList();
                }

                grcCheDoAn.BeginUpdate();
                grcCheDoAn.DataSource = source;
                grcCheDoAn.EndUpdate();

                //Loc lai thi tap dong hien thi doi -> ve lai o tick tren tieu de cot
                grdCheDoAn.InvalidateColumnHeader(grdColCheck);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tick lan dau thi lay so thu tu moi (++checkCounter) -> dong tick truoc dung truoc khi chen
        /// vao to dieu tri. Bo tick roi tick lai thi xep xuong cuoi, vi do la lan tick moi nhat.
        /// </summary>
        private void ToggleCheck(DietADO row)
        {
            if (row == null) return;

            row.IsChecked = !row.IsChecked;
            row.CheckOrder = row.IsChecked ? ++checkCounter : 0;
            grdCheDoAn.RefreshData();
            grdCheDoAn.InvalidateColumnHeader(grdColCheck);
        }

        /// <summary>
        /// Cac dong dang HIEN THI tren luoi (da qua loc tim kiem), theo thu tu hien thi.
        /// </summary>
        private List<DietADO> GetVisibleRows()
        {
            List<DietADO> result = new List<DietADO>();
            for (int i = 0; i < grdCheDoAn.RowCount; i++)
            {
                DietADO row = grdCheDoAn.GetRow(i) as DietADO;
                if (row != null) result.Add(row);
            }
            return result;
        }

        /// <summary>
        /// O tick tren tieu de cot = tat ca dong dang hien thi deu da tick.
        /// Khong luu co rieng, moi lan ve tinh lai tu du lieu nen luon khop voi luoi.
        /// </summary>
        private bool IsAllVisibleChecked()
        {
            List<DietADO> visible = GetVisibleRows();
            return visible.Count > 0 && visible.All(o => o.IsChecked);
        }

        /// <summary>
        /// Ve o tick "chon tat ca" vao tieu de cot tick.
        /// </summary>
        private void grdCheDoAn_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != grdColCheck) return;

                e.Info.InnerElements.Clear();
                e.Painter.DrawObject(e.Info);

                DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)repositoryItemCheck.CreateViewInfo();
                DevExpress.XtraEditors.Drawing.CheckEditPainter painter = (DevExpress.XtraEditors.Drawing.CheckEditPainter)repositoryItemCheck.CreatePainter();
                info.EditValue = IsAllVisibleChecked();
                info.Bounds = e.Bounds;
                info.CalcViewInfo(e.Graphics);
                painter.Draw(new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, e.Cache, e.Bounds));

                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bam vao tieu de cot tick -> tick/bo tick tat ca dong dang hien thi (da qua loc tim kiem).
        /// Dong da tick tu truoc giu nguyen thu tu cu, dong moi tick noi tiep phia sau theo thu tu tren luoi.
        /// </summary>
        private void grdCheDoAn_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left) return;

                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit = grdCheDoAn.CalcHitInfo(e.Location);
                if (!hit.InColumnPanel || hit.Column != grdColCheck) return;

                bool checkAll = !IsAllVisibleChecked();
                foreach (DietADO row in GetVisibleRows())
                {
                    if (checkAll)
                    {
                        if (!row.IsChecked)
                        {
                            row.IsChecked = true;
                            row.CheckOrder = ++checkCounter;
                        }
                    }
                    else
                    {
                        row.IsChecked = false;
                        row.CheckOrder = 0;
                    }
                }
                grdCheDoAn.RefreshData();
                grdCheDoAn.InvalidateColumnHeader(grdColCheck);

                ((DevExpress.Utils.DXMouseEventArgs)e).Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdCheDoAn_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                //Bam vao bat ky o nao cua dong cung tick/bo tick
                ToggleCheck(grdCheDoAn.GetRow(e.RowHandle) as DietADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grdCheDoAn_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    ToggleCheck(grdCheDoAn.GetFocusedRow() as DietADO);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    btnDongY_Click(null, null);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_EditValueChanged(object sender, EventArgs e)
        {
            FillDataToGrid();
        }

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter)
                {
                    grcCheDoAn.Focus();
                    grdCheDoAn.FocusedRowHandle = 0;
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmChonCheDoAn_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
                else if (e.Control && e.KeyCode == Keys.S)
                {
                    btnDongY_Click(null, null);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDongY_Click(object sender, EventArgs e)
        {
            try
            {
                List<DietADO> checkedList = listDiet.Where(o => o.IsChecked).OrderBy(o => o.CheckOrder).ToList();
                if (checkedList.Count > 0 && dataSelect != null)
                {
                    dataSelect(string.Join("\r\n", checkedList.Select(o => BuildDietLine(o))));
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// "Chế độ ăn: Mã-Thành phần dinh dưỡng/24h-Dạng chế biến-Áp dụng điều trị".
        /// Truong trong thi bo qua, khong de thua dau "-"; noi dung xuong dong trong danh muc gop thanh 1 dong.
        /// </summary>
        private string BuildDietLine(DietADO diet)
        {
            List<string> parts = new List<string> { diet.DIET_CODE, diet.NUTRITION_INFO, diet.PROCESSING_FORM, diet.TREATMENT_APPLY }
                .Select(o => JoinOneLine(o))
                .Where(o => o.Length > 0)
                .ToList();
            return "Chế độ ăn: " + string.Join("-", parts);
        }

        private string JoinOneLine(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return "";
            return Regex.Replace(value, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Bo dau tieng Viet va chuyen ve chu thuong de so sanh (giong danh muc HisDiet).
        /// </summary>
        private string NormalizeForSearch(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            string formD = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder(formD.Length);
            foreach (char c in formD)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            // "đ" khong tach dau bang FormD nen phai doi tay
            return sb.ToString().Normalize(NormalizationForm.FormC).Replace('đ', 'd');
        }

        void SetIconFrm()
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
                Resources.ResourceLanguageManager.LanguageResource__frmChonCheDoAn = new ResourceManager("HIS.Desktop.Plugins.TrackingCreate.Resources.Lang", typeof(frmChonCheDoAn).Assembly);
                ResourceManager res = Resources.ResourceLanguageManager.LanguageResource__frmChonCheDoAn;

                this.Text = Inventec.Common.Resource.Get.Value("frmChonCheDoAn.Text", res, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmChonCheDoAn.txtKeyWord.Properties.NullValuePrompt", res, LanguageManager.GetCulture());
                this.btnDongY.Text = Inventec.Common.Resource.Get.Value("frmChonCheDoAn.btnDongY.Text", res, LanguageManager.GetCulture());                this.grdColDietCode.Caption = Inventec.Common.Resource.Get.Value("frmChonCheDoAn.grdColDietCode.Caption", res, LanguageManager.GetCulture());
                this.grdColNutritionInfo.Caption = Inventec.Common.Resource.Get.Value("frmChonCheDoAn.grdColNutritionInfo.Caption", res, LanguageManager.GetCulture());
                this.grdColProcessingForm.Caption = Inventec.Common.Resource.Get.Value("frmChonCheDoAn.grdColProcessingForm.Caption", res, LanguageManager.GetCulture());
                this.grdColTreatmentApply.Caption = Inventec.Common.Resource.Get.Value("frmChonCheDoAn.grdColTreatmentApply.Caption", res, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
