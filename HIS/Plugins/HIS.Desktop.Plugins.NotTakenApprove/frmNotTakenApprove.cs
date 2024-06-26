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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.NotTakenApprove.Base;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.NotTakenApprove
{
    public partial class frmNotTakenApprove : FormBase
    {

        private V_HIS_MEDI_STOCK mediStock = null;

        private int exported_start = 0;
        private int exported_limit = 0;
        private int unexport_start = 0;
        private int unexport_limit = 0;

        private int exported_rowCount = 0;
        private int exported_dataTotal = 0;
        private int unexport_rowCount = 0;
        private int unexport_dataTotal = 0;

        List<HIS_EXP_MEST_TYPE> _TypeSelecteds;
        List<HIS_EXP_MEST_TYPE> expMestTypes;
        private List<long> listTypeId = new List<long>()
        {
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT,
        };

        public frmNotTakenApprove(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            Base.ResourceLangManager.InitResourceLanguageManager();
            this.SetResourceKeyLanguage();
        }

        private void SetResourceKeyLanguage()
        {
            try
            {
                var resources = Base.ResourceLangManager.LangFrmNotTakenApprove;
                var lang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                btnSearch.Text = Inventec.Common.Resource.Get.Value("btnSearch.Text", resources, lang);
                btnApprove.Text = Inventec.Common.Resource.Get.Value("btnApprove.Text", resources, lang);


                lciApproveTime.Text = Inventec.Common.Resource.Get.Value("lciApproveTime.Text", resources, lang);
                lciApproveTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("lciApproveTime.Tooltip", resources, lang);
                lciMediStock.Text = Inventec.Common.Resource.Get.Value("lciMediStock.Text", resources, lang);
                gridViewExpMestExported.ViewCaption = Inventec.Common.Resource.Get.Value("gridViewExpMestExported.ViewCaption", resources, lang);
                gridViewExpMestUnexport.ViewCaption = Inventec.Common.Resource.Get.Value("gridViewExpMestUnexport.ViewCaption", resources, lang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmNotTakenApprove_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadMediStock();
                SetDefaultTime();
                LoadDataToGridExported();
                LoadDataToGridUnexport();

                expMestTypes = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>();
                InitCheck(cboType, SelectionGrid__Type);
                InitCombo(cboType, expMestTypes.Where(o => listTypeId.Contains(o.ID)).ToList(), "EXP_MEST_TYPE_NAME", "ID");
                btnApprove.Enabled = this.mediStock != null;
                btnSearch.Enabled = this.mediStock != null;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMediStock()
        {
            try
            {
                mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
                if (mediStock == null)
                {
                    XtraMessageBox.Show(ResourceMessageManager.PhongLamViecKhongPhaiLaKho, ResourceMessageManager.ThongBao, DefaultBoolean.True);
                }
                else
                {
                    lblMediStock.Text = String.Format("{0} - {1}", this.mediStock.MEDI_STOCK_CODE ?? "", this.mediStock.MEDI_STOCK_NAME ?? "");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultTime()
        {
            try
            {
                dtApproveTimeFrom.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                dtApproveTimeTo.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridExported()
        {
            try
            {
                int pageSize;
                if (ucPagingExported.pagingGrid != null)
                {
                    pageSize = ucPagingExported.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }

                GetDataExported(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = exported_rowCount;
                param.Count = exported_dataTotal;
                ucPagingExported.Init(GetDataExported, param, pageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataExported(object param)
        {
            try
            {
                try
                {
                    if (mediStock == null) return;
                    List<HIS_EXP_MEST> listData = new List<HIS_EXP_MEST>();
                    exported_start = ((CommonParam)param).Start ?? 0;
                    exported_limit = ((CommonParam)param).Limit ?? 0;
                    CommonParam paramCommon = new CommonParam(exported_start, exported_limit);
                    HisExpMestFilter filter = new HisExpMestFilter();
                    filter.ORDER_FIELD = "EXP_MEST_CODE";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.MEDI_STOCK_ID = this.mediStock.ID;
                    filter.EXP_MEST_TYPE_IDs = new List<long>();
                    filter.EXP_MEST_TYPE_IDs.AddRange(new List<long>
                {
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT
                });
                    filter.EXP_MEST_TYPE_IDs = _TypeSelecteds.Select(o => o.ID).ToList();
                    //filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                    //filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK;
                    filter.CREATE_TIME_FROM = Convert.ToInt64(dtApproveTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                    filter.CREATE_TIME_TO = Convert.ToInt64(dtApproveTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");

                    var result = new BackendAdapter(paramCommon).GetRO<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (result != null)
                    {
                        listData = (List<HIS_EXP_MEST>)result.Data;
                        exported_rowCount = (listData == null ? 0 : listData.Count);
                        exported_dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    }

                    gridControlExpMestExported.BeginUpdate();
                    gridControlExpMestExported.DataSource = listData;
                    gridControlExpMestExported.EndUpdate();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridUnexport()
        {
            try
            {
                int pageSize;
                if (ucPagingUnexport.pagingGrid != null)
                {
                    pageSize = ucPagingUnexport.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }

                GetDataUnexport(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = unexport_rowCount;
                param.Count = unexport_dataTotal;
                ucPagingUnexport.Init(GetDataUnexport, param, pageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataUnexport(object param)
        {
            try
            {
                try
                {
                    if (mediStock == null) return;
                    List<HIS_EXP_MEST> listData = new List<HIS_EXP_MEST>();
                    unexport_start = ((CommonParam)param).Start ?? 0;
                    unexport_limit = ((CommonParam)param).Limit ?? 0;
                    CommonParam paramCommon = new CommonParam(unexport_start, unexport_limit);
                    HisExpMestFilter filter = new HisExpMestFilter();
                    filter.ORDER_FIELD = "EXP_MEST_CODE";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.MEDI_STOCK_ID = this.mediStock.ID;
                    filter.IS_NOT_TAKEN = false;
                    filter.EXP_MEST_STT_IDs = new List<long>()
                    {
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                    };
                    filter.EXP_MEST_TYPE_IDs = new List<long>()
                    {
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT
                    };
                    filter.EXP_MEST_TYPE_IDs = _TypeSelecteds.Select(o => o.ID).ToList();
                    filter.HAS_AGGR_EXP_MEST_ID = false;
                    filter.CREATE_TIME_FROM = Convert.ToInt64(dtApproveTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                    filter.CREATE_TIME_TO = Convert.ToInt64(dtApproveTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");

                    var result = new BackendAdapter(paramCommon).GetRO<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (result != null)
                    {
                        listData = (List<HIS_EXP_MEST>)result.Data;
                        unexport_rowCount = (listData == null ? 0 : listData.Count);
                        unexport_dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    }

                    gridControlExpMestUnexport.BeginUpdate();
                    gridControlExpMestUnexport.DataSource = listData;
                    gridControlExpMestUnexport.EndUpdate();
                    gridViewExpMestUnexport.SelectAll();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Type(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_EXP_MEST_TYPE> sgSelectedNews = new List<HIS_EXP_MEST_TYPE>();
                    foreach (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.EXP_MEST_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this._TypeSelecteds = new List<HIS_EXP_MEST_TYPE>();
                    this._TypeSelecteds.AddRange(sgSelectedNews);
                }

                this.cboType.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtApproveTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtApproveTimeTo.Focus();
                    btnSearch.PerformClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtApproveTimeFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtApproveTimeTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtApproveTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    btnSearch.Focus();
                    btnSearch.PerformClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtApproveTimeTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch.PerformClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestExported_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + exported_start;
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            if (data.TDL_PATIENT_DOB.HasValue)
                            {
                                if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB.HasValue && data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB.Value == 1)
                                {
                                    e.Value = data.TDL_PATIENT_DOB.Value.ToString().Substring(0, 4);
                                }
                                else
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB.Value);
                                }
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        else if (e.Column.FieldName == "EXP_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.LAST_EXP_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestUnexport_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + exported_start;
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            if (data.TDL_PATIENT_DOB.HasValue)
                            {
                                if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB.HasValue && data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB.Value == 1)
                                {
                                    e.Value = data.TDL_PATIENT_DOB.Value.ToString().Substring(0, 4);
                                }
                                else
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB.Value);
                                }
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "EXP_MEST_TYPE_DISPLAY")
                        {
                            e.Value = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>().FirstOrDefault(o => o.ID == data.EXP_MEST_TYPE_ID).EXP_MEST_TYPE_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewExpMestUnexport.PostEditor();
                if (!btnApprove.Enabled || this.mediStock == null) return;
                int[] selecteds = gridViewExpMestUnexport.GetSelectedRows();

                if (selecteds == null || selecteds.Length <= 0)
                {
                    XtraMessageBox.Show(ResourceMessageManager.BanChuaChonPhieuXuatCanDuyetKhongLay, ResourceMessageManager.ThongBao, DefaultBoolean.True);
                    return;
                }

                if (XtraMessageBox.Show(ResourceMessageManager.BanMuonDuyetKhongLay, ResourceMessageManager.CanhBao, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;

                btnApprove.Enabled = false;
                btnSearch.Enabled = false;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                ApproveNotTakenPresSDO data = new ApproveNotTakenPresSDO();
                data.MediStockId = this.mediStock.ID;
                data.RequestRoomId = this.currentModuleBase.RoomId;
                data.ExpMestIds = new List<long>();
                foreach (int i in selecteds)
                {
                    HIS_EXP_MEST s = (HIS_EXP_MEST)gridViewExpMestUnexport.GetRow(i);
                    if (s != null)
                    {
                        data.ExpMestIds.Add(s.ID);
                    }
                }
                bool result = new BackendAdapter(param).Post<bool>("api/HisExpMest/ApproveNotTaken", ApiConsumers.MosConsumer, data, param);
                if (result)
                {
                    LoadDataToGridExported();
                    LoadDataToGridUnexport();
                }
                Inventec.Common.Logging.LogSystem.Debug("_ResultUpdate: " + result);
                btnApprove.Enabled = true;
                btnSearch.Enabled = true;
                WaitingManager.Hide();
                MessageManager.Show(this, param, result);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                btnApprove.Enabled = true;
                btnSearch.Enabled = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSearch.Enabled) return;
                WaitingManager.Show();
                exported_start = 0;
                exported_limit = 0;
                unexport_start = 0;
                unexport_limit = 0;
                exported_rowCount = 0;
                exported_dataTotal = 0;
                unexport_rowCount = 0;
                unexport_dataTotal = 0;
                LoadDataToGridExported();
                LoadDataToGridUnexport();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnApprove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnApprove.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string typeName = "";
                if (_TypeSelecteds != null && _TypeSelecteds.Count > 0)
                {
                    foreach (var item in _TypeSelecteds)
                    {
                        typeName += item.EXP_MEST_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
