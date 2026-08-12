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
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Cua so ket qua kiem tra ho so tren he thong tien giam dinh.
    /// Luoi tren: moi ho so mot dong. Chon mot dong thi luoi duoi hien chi tiet loi
    /// cua ho so do theo ba nhom.
    ///
    /// Tham chieu: PTTK_53286 muc B.4.2
    /// </summary>
    public partial class frmTienGiamDinhResult : HIS.Desktop.Utility.FormBase
    {
        private readonly List<TienGiamDinhResultADO> results;

        public frmTienGiamDinhResult(List<TienGiamDinhResultADO> results)
        {
            InitializeComponent();
            this.results = results ?? new List<TienGiamDinhResultADO>();
            this.SetIcon();
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void frmTienGiamDinhResult_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                FillDataToGrid();
                this.gridViewSummary.FocusedRowChanged += gridViewSummary_FocusedRowChanged;
                //Hien chi tiet cua dong dau tien ngay khi mo
                FillDetailOfFocusedRow();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Dat ten hien thi theo ngon ngu dang dung</summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.ExportXmlQD130.Resources.Lang",
                    typeof(frmTienGiamDinhResult).Assembly);

                this.Text = GetLangValue("frmTienGiamDinhResult.Text", this.Text);
                this.btnClose.Text = GetLangValue("frmTienGiamDinhResult.btnClose.Text", this.btnClose.Text);

                this.gridColTreatmentCode.Caption = GetLangValue("frmTienGiamDinhResult.gridColTreatmentCode.Caption", this.gridColTreatmentCode.Caption);
                this.gridColPatientName.Caption = GetLangValue("frmTienGiamDinhResult.gridColPatientName.Caption", this.gridColPatientName.Caption);
                this.gridColErrorCount.Caption = GetLangValue("frmTienGiamDinhResult.gridColErrorCount.Caption", this.gridColErrorCount.Caption);
                this.gridColStatusName.Caption = GetLangValue("frmTienGiamDinhResult.gridColStatusName.Caption", this.gridColStatusName.Caption);

                this.gridColGroupName.Caption = GetLangValue("frmTienGiamDinhResult.gridColGroupName.Caption", this.gridColGroupName.Caption);
                this.gridColSeverityName.Caption = GetLangValue("frmTienGiamDinhResult.gridColSeverityName.Caption", this.gridColSeverityName.Caption);
                this.gridColErrorCode.Caption = GetLangValue("frmTienGiamDinhResult.gridColErrorCode.Caption", this.gridColErrorCode.Caption);
                this.gridColDescription.Caption = GetLangValue("frmTienGiamDinhResult.gridColDescription.Caption", this.gridColDescription.Caption);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Doc chuoi hien thi, khong doc duoc thi giu nguyen gia tri dang co</summary>
        private string GetLangValue(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key,
                    Resources.ResourceLanguageManager.LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return defaultValue;
        }

        private void FillDataToGrid()
        {
            try
            {
                //Dua ho so co loi len dau de nguoi dung thay ngay cai can sua
                List<TienGiamDinhResultADO> ordered = this.results
                    .OrderByDescending(o => o.Status == EnumTienGiamDinhStatus.Critical)
                    .ThenByDescending(o => o.Status == EnumTienGiamDinhStatus.Warning)
                    .ThenByDescending(o => o.TotalErrorCount)
                    .ToList();

                this.gridViewSummary.BeginUpdate();
                try
                {
                    this.gridControlSummary.DataSource = ordered;
                }
                finally
                {
                    this.gridViewSummary.EndUpdate();
                }

                this.lblSummary.Text = BuildSummaryText();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Dong tong ket duoi cung: bao nhieu ho so, bao nhieu bi chan</summary>
        private string BuildSummaryText()
        {
            try
            {
                int total = this.results.Count;
                int critical = this.results.Count(o => o.Status == EnumTienGiamDinhStatus.Critical);
                int warning = this.results.Count(o => o.Status == EnumTienGiamDinhStatus.Warning);
                int failed = this.results.Count(o => o.Status == EnumTienGiamDinhStatus.CheckFailed);

                return string.Format(Resources.ResourceMessageLang.TienGiamDinhTongKetKetQua,
                    total, critical, warning, failed);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return "";
        }

        private void gridViewSummary_FocusedRowChanged(object sender,
            DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                FillDetailOfFocusedRow();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Hien chi tiet loi cua ho so dang chon o luoi tren</summary>
        private void FillDetailOfFocusedRow()
        {
            try
            {
                TienGiamDinhResultADO current =
                    this.gridViewSummary.GetFocusedRow() as TienGiamDinhResultADO;

                this.gridViewDetail.BeginUpdate();
                try
                {
                    this.gridControlDetail.DataSource = current == null
                        ? new List<TienGiamDinhErrorADO>()
                        : current.Errors;
                }
                finally
                {
                    this.gridViewDetail.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                LogSystem.Warn(ex);
            }
        }
    }
}
