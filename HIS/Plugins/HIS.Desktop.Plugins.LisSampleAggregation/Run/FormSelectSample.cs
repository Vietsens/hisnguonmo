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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Modules;
using LIS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.LisSampleAggregation.Run
{
    public partial class FormSelectSample : FormBase
    {
        List<LIS_SAMPLE> DataChoose = new List<LIS_SAMPLE>();
        Action<List<LIS_SAMPLE>> ActionChoose;

        public FormSelectSample(Module currentModule, List<LIS_SAMPLE> data, Action<List<LIS_SAMPLE>> actionChoose)
            : base(currentModule)
        {
            InitializeComponent();
            this.DataChoose = data;
            this.ActionChoose = actionChoose;
        }

        private void FormSelectSample_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();

                this.gridControl1.DataSource = DataChoose;
                this.gridControl1.RefreshDataSource();
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
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.Text = Inventec.Common.Resource.Get.Value("FormSelectSample.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("FormSelectSample.btnChoose.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_IsAggr.Caption = Inventec.Common.Resource.Get.Value("FormSelectSample.gc_IsAggr.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PatientName.Caption = Inventec.Common.Resource.Get.Value("FormSelectSample.gc_PatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("FormSelectSample.gc_ServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("FormSelectSample.gc_TreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == gc_PatientName.FieldName)
                        {
                            e.Value = data.LAST_NAME + " " + data.FIRST_NAME;
                        }
                        else if (e.Column.FieldName == gc_IsAggr.FieldName)
                        {
                            e.Value = data.AGGREGATE_SAMPLE_ID.HasValue ? Resources.ResourceLanguageManager.Gop : Resources.ResourceLanguageManager.ChuaGop;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnChoose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnChoose_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                List<LIS_SAMPLE> selectedData = new List<LIS_SAMPLE>();

                var index = gridView1.GetSelectedRows();
                foreach (var rowHandle in index)
                {
                    var row = (LIS_SAMPLE)gridView1.GetRow(rowHandle);
                    if (row != null)
                    {
                        selectedData.Add(row);
                    }
                }

                if (selectedData.Count > 0)
                {
                    if (selectedData.Exists(o => o.AGGREGATE_SAMPLE_ID.HasValue))
                    {
                        XtraMessageBox.Show(Resources.ResourceLanguageManager.MauDaGop, Resources.ResourceLanguageManager.ThongBao);
                    }
                    else if (this.ActionChoose != null)
                    {
                        this.ActionChoose(selectedData);
                        this.Close();
                    }
                }
                else
                {
                    XtraMessageBox.Show(Resources.ResourceLanguageManager.BanChuaChonMau);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
