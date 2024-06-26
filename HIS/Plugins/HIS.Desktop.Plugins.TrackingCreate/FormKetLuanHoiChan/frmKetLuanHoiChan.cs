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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TrackingCreate.FormKetLuanHoiChan
{
    public partial class frmKetLuanHoiChan : Form
    {
        #region Declare
        long _treatmentID;
        HIS.Desktop.Common.DelegateSelectData _dataSelect;
        List<V_HIS_DEBATE> _listDebate;

        #endregion

        #region Construct
        public frmKetLuanHoiChan(long TreatmentID, HIS.Desktop.Common.DelegateSelectData DataSelect)
        {
            InitializeComponent();

            try
            {
                SetIcon();
                this._treatmentID = TreatmentID;
                this._dataSelect = DataSelect;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region form_Load
        private void frmKetLuanHoiChan_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                LoadDataDebate();
                FillDataToGridControlDebate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region private Method
        private void LoadDataDebate()
        {
            try
            {
                if (this._treatmentID <= 0)
                    return;
                CommonParam param = new CommonParam();
                MOS.Filter.HisDebateViewFilter filter = new MOS.Filter.HisDebateViewFilter();
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "DEBATE_TIME";
                //filter.IS_ACTIVE = 1;
                filter.TREATMENT_ID = this._treatmentID;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this._treatmentID", this._treatmentID));
                this._listDebate = new BackendAdapter(param).Get<List<V_HIS_DEBATE>>("api/HisDebate/GetView", ApiConsumers.MosConsumer, filter, param);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this._listDebate", this._listDebate));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            } 
        }

        private void FillDataToGridControlDebate()
        {
            try
            {
                List<V_HIS_DEBATE> listData = new List<V_HIS_DEBATE>();

                if (this._listDebate != null)
                {
                    string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());
                    var query = this._listDebate.AsQueryable();
                    query = query.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.CONCLUSION != null ? o.CONCLUSION.ToLower() : "").Contains(keyword));

                    listData = query.OrderBy(o => o.DEBATE_TIME).ToList();
                }

                gridControlDebate.BeginUpdate();
                gridControlDebate.DataSource = listData;
                gridControlDebate.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            } 
        }

        private void SelectProcess()
        {
            try
            {
                if (this._dataSelect == null)
                    return;
                string selectedConclusionString = "";
                var selectedRows = gridViewDebate.GetSelectedRows();
                if (selectedRows != null && selectedRows.Count() > 0)
                {
                    foreach (var rowHandle in selectedRows)
                    {
                        var row = (V_HIS_DEBATE)gridViewDebate.GetRow(rowHandle);
                        if (row != null)
                        {
                            if (!String.IsNullOrWhiteSpace(selectedConclusionString) && !String.IsNullOrWhiteSpace(row.CONCLUSION))
                                selectedConclusionString += "\r\n";
                            selectedConclusionString += row.CONCLUSION;
                        }
                    }
                }
                this._dataSelect(selectedConclusionString);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.FillDataToGridControlDebate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSelect_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSelect.Enabled)
                {
                    btnSelect_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                SelectProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDebate_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    //MOS.EFMODEL.DataModels.V_HIS_DEBATE pData = (MOS.EFMODEL.DataModels.V_HIS_DEBATE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "DEBATE_TIME_Display")
                    {
                        string debateTime = (view.GetRowCellValue(e.ListSourceRowIndex, "DEBATE_TIME") ?? "").ToString();
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(debateTime));
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmKetLuanHoiChan
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmKetLuanHoiChan = new ResourceManager("HIS.Desktop.Plugins.TrackingCreate.Resources.Lang", typeof(frmKetLuanHoiChan).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControlRoot.Text = Inventec.Common.Resource.Get.Value("frmKetLuanHoiChan.layoutControlRoot.Text", Resources.ResourceLanguageManager.LanguageResource__frmKetLuanHoiChan, LanguageManager.GetCulture());
                this.btnSelect.Text = Inventec.Common.Resource.Get.Value("frmKetLuanHoiChan.btnSelect.Text", Resources.ResourceLanguageManager.LanguageResource__frmKetLuanHoiChan, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmKetLuanHoiChan.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__frmKetLuanHoiChan, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmKetLuanHoiChan.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource__frmKetLuanHoiChan, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmKetLuanHoiChan.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__frmKetLuanHoiChan, LanguageManager.GetCulture());
                this.bbtnSelect.Caption = Inventec.Common.Resource.Get.Value("frmKetLuanHoiChan.bbtnSelect.Caption", Resources.ResourceLanguageManager.LanguageResource__frmKetLuanHoiChan, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmKetLuanHoiChan.Text", Resources.ResourceLanguageManager.LanguageResource__frmKetLuanHoiChan, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
