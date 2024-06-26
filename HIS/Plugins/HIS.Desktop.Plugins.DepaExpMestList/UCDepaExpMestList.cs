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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.DepaExpMestList
{
  public partial class UCDepaExpMestList : UserControl
  {
    #region Declare
    int rowCount = 0;
    int dataTotal = 0;
    int startPage = 0;
    long roomId = 0;
    long roomTypeId = 0;
    ToolTipControlInfo lastInfo;
    GridColumn lastColumn = null;
    int lastRowHandle = -1;
    MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
    private string LoggingName = "";
    #endregion

    #region Construct
    public UCDepaExpMestList()
    {
      InitializeComponent();
      try
      {
        LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
        gridControl.ToolTipController = toolTipController;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    public UCDepaExpMestList(long roomId, long roomTypeId)
      : this()
    {
      try
      {
        medistock = Base.GlobalStore.ListMediStock.FirstOrDefault(o => o.ROOM_ID == roomId && o.ROOM_TYPE_ID == roomTypeId);
        this.roomId = roomId;
        this.roomTypeId = roomTypeId;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void UCDepaExpMestList_Load(object sender, EventArgs e)
    {
      try
      {
        //Gan ngon ngu
        LoadKeysFromlanguage();

        FillDataToNavBarStatus();

        //Gan gia tri mac dinh
        SetDefaultValueControl();

        //Load du lieu
        FillDataToGrid();

        //focus truong du lieu dau tien
        txtExpMestCode.Focus();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
    #endregion

    #region Private method
    private void LoadKeysFromlanguage()
    {
      try
      {
        Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DepaExpMestList.Resources.Lang", typeof(HIS.Desktop.Plugins.DepaExpMestList.UCDepaExpMestList).Assembly);

        ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
        this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.txtExpMestCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.txtExpMestCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.navBarControlFilter.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.navBarControlFilter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.navBarGroupCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.lciCreateTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.lciCreateTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.layoutControlStatus.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.layoutControlStatus.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.navBarGroupStatus.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.navBarGroupStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.STT.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.IMP_MEST_STT_NAME.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.IMP_MEST_STT_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.repositoryItemPictureEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcExpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcMediStockName.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcMediStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcImpMediStockName.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcImpMediStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcReqName.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcReqName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcApprovalName.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcApprovalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcApprovalTime.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcApprovalTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcExpLoginName.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcExpLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcExpTime.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcExpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcCreator.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.GcModifier.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.GcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.navBarGroupExpTime.Caption = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.navBarGroupExpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.lciExpTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.lciExpTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        this.lciExpTimeTo.Text = Inventec.Common.Resource.Get.Value("UCDepaExpMestList.lciExpTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void FillDataToNavBarStatus()
    {
      try
      {
        navBarControlFilter.BeginUpdate();
        int d = 0;
        foreach (var item in Base.GlobalStore.HisExpMestStts)
        {
          navBarGroupStatus.GroupClientHeight += 25;
          DevExpress.XtraEditors.CheckEdit checkEdit = new DevExpress.XtraEditors.CheckEdit();
          layoutControlStatus.Controls.Add(checkEdit);
          checkEdit.Location = new System.Drawing.Point(50, 2 + (d * 23));
          checkEdit.Name = item.ID.ToString();
          checkEdit.Properties.Caption = item.EXP_MEST_STT_NAME;
          checkEdit.Size = new System.Drawing.Size(150, 19);
          checkEdit.StyleController = this.layoutControlStatus;
          checkEdit.TabIndex = 4 + d;
          checkEdit.EnterMoveNextControl = false;
          d++;
        }
        navBarControlFilter.EndUpdate();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
        navBarControlFilter.EndUpdate();
      }
    }

    private void SetDefaultValueControl()
    {
      try
      {
        txtExpMestCode.Text = "";
        txtKeyWord.Text = "";
        dtCreateTimeFrom.EditValue = DateTime.Now;
        dtCreateTimeTo.EditValue = DateTime.Now;
        dtExpTimeFrom.EditValue = null;
        dtExpTimeTo.EditValue = null;
        txtExpMestCode.Focus();
        SetDefaultValueStatus();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void SetDefaultValueStatus()
    {
      try
      {
        if (layoutControlStatus.Controls.Count > 0)
        {
          for (int i = 0; i < layoutControlStatus.Controls.Count; i++)
          {
            if (layoutControlStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
            {
              var checkEdit = layoutControlStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
              checkEdit.Checked = false;
            }
          }
        }
        navBarGroupStatus.Expanded = true;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void FillDataToGrid()
    {
      try
      {
        WaitingManager.Show();
        GridPaging(new CommonParam(0, (int)ConfigApplications.NumPageSize));
        CommonParam param = new CommonParam();
        param.Limit = rowCount;
        param.Count = dataTotal;
        ucPaging.Init(GridPaging, param, (int)ConfigApplications.NumPageSize);
        WaitingManager.Hide();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
        WaitingManager.Hide();
      }
    }

    private void GridPaging(object param)
    {
      try
      {
        startPage = ((CommonParam)param).Start ?? 0;
        int limit = ((CommonParam)param).Limit ?? 0;
        CommonParam paramCommon = new CommonParam(startPage, limit);
        ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_DEPA_EXP_MEST>> apiResult = null;
        MOS.Filter.HisDepaExpMestViewFilter filter = new MOS.Filter.HisDepaExpMestViewFilter();


        SetFilter(ref filter);
        gridView.BeginUpdate();
        apiResult = new Inventec.Common.Adapter.BackendAdapter
            (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_DEPA_EXP_MEST>>
            (ApiConsumer.HisRequestUriStore.HIS_DEPA_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
        if (apiResult != null)
        {
          var data = apiResult.Data;
          if (data != null && data.Count > 0)
          {
            gridControl.DataSource = data;
            rowCount = (data == null ? 0 : data.Count);
            dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
          }
          else
          {
            gridControl.DataSource = null;
            rowCount = (data == null ? 0 : data.Count);
            dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
          }
        }
        gridView.EndUpdate();

        #region Process has exception
        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
        #endregion
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void SetFilter(ref MOS.Filter.HisDepaExpMestViewFilter filter)
    {
      try
      {
        filter.ORDER_FIELD = "EXP_MEST_MODIFY_TIME";
        filter.ORDER_DIRECTION = "DESC";
        filter.KEY_WORD = txtKeyWord.Text.Trim();
        V_HIS_ROOM room = new V_HIS_ROOM();
        var rooms = BackendDataWorker.Get<V_HIS_ROOM>();
        if (rooms != null && rooms.Count > 0)
        {
          room = rooms.FirstOrDefault(o => o.ID == roomId);
        }

        if (room != null)
        {
          filter.DEPARTMENT_ID = room.DEPARTMENT_ID;
        }

        if (roomId == 0)
        {
          filter.CREATOR = LoggingName;
        }
        else
        {
          filter.DATA_DOMAIN_FILTER = true;
          filter.WORKING_ROOM_ID = roomId;
        }

        if (!String.IsNullOrEmpty(txtExpMestCode.Text))
        {
          string code = txtExpMestCode.Text.Trim();
          if (code.Length < 12 && checkDigit(code))
          {
            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
            txtExpMestCode.Text = code;
          }
          filter.EXP_MEST_CODE__EXACT = code;
        }

        if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
          filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
              Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

        if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
          filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
              Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

        if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue)
          filter.EXP_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
              Convert.ToDateTime(dtExpTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

        if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue)
          filter.EXP_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
              Convert.ToDateTime(dtExpTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

        SetFilterStatus(ref filter);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void SetFilterStatus(ref MOS.Filter.HisDepaExpMestViewFilter filter)
    {
      try
      {
        if (layoutControlStatus.Controls.Count > 0)
        {
          for (int i = 0; i < layoutControlStatus.Controls.Count; i++)
          {
            if (layoutControlStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
            {
              var checkEdit = layoutControlStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
              if (checkEdit.Checked)
              {
                if (filter.EXP_MEST_STT_IDs == null)
                  filter.EXP_MEST_STT_IDs = new List<long>();
                filter.EXP_MEST_STT_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private bool checkDigit(string s)
    {
      bool result = false;
      try
      {
        for (int i = 0; i < s.Length; i++)
        {
          if (char.IsDigit(s[i]) == true) result = true;
          else result = false;
        }
        return result;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
        return result;
      }
    }

    private void btnSearch_Click(object sender, EventArgs e)
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

    private void btnRefresh_Click(object sender, EventArgs e)
    {
      try
      {
        SetDefaultValueControl();
        FillDataToGrid();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void txtExpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
      try
      {
        if (e.KeyCode == Keys.Enter)
        {
          if (!String.IsNullOrEmpty(txtExpMestCode.Text))
          {
            FillDataToGrid();
            txtExpMestCode.SelectAll();
          }
          else
          {
            txtKeyWord.Focus();
            txtKeyWord.SelectAll();
          }
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
      try
      {
        if (e.KeyCode == Keys.Enter) btnSearch_Click(null, null);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
    {
      try
      {
        if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
        {
          if (dtCreateTimeFrom.EditValue != null)
          {
            dtCreateTimeTo.Focus();
            dtCreateTimeTo.ShowPopup();
          }
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
    {
      try
      {
        if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
        {
          SendKeys.Send("{TAB}");
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void toolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
    {
      try
      {
        if (e.Info == null && e.SelectedControl == gridControl)
        {
          DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
          GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
          if (info.InRowCell)
          {
            if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
            {
              lastColumn = info.Column;
              lastRowHandle = info.RowHandle;

              string text = "";
              if (info.Column.FieldName == "EXP_MEST_STT_NAME")
              {
                text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
              }
              else if (info.Column.FieldName == "EXP_MEST_STT_ICON")
              {
                text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
              }
              lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
            }
            e.Info = lastInfo;
          }
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Warn(ex);
      }
    }

    private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
    {
      try
      {
        if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
        {
          var data = (MOS.EFMODEL.DataModels.V_HIS_DEPA_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
          if (data != null)
          {
            if (e.Column.FieldName == "STT")
            {
              e.Value = e.ListSourceRowIndex + 1 + startPage;
            }
            else if (e.Column.FieldName == "EXP_MEST_STT_ICON")// trạng thái
            {
              if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT) //tam thoi
              {
                e.Value = imageListStatus.Images[0];
              }
              else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST) //yeu cau
              {
                e.Value = imageListStatus.Images[1];
              }
              else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED) // tu choi duyet
              {
                e.Value = imageListStatus.Images[2];
              }
              else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__APPROVED) // duyet
              {
                e.Value = imageListStatus.Images[3];
              }
              else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__EXPORTED) // da xuat
              {
                e.Value = imageListStatus.Images[4];
              }
              else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__AGGREGATE) //Tong hop
              {
                e.Value = imageListStatus.Images[5];
              }
            }
            else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
            {
              e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
            }
            else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
            {
              e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
            }
            else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
            {
              e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
            }
            else if (e.Column.FieldName == "EXP_TIME_DISPLAY")
            {
              e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXP_TIME ?? 0);
            }
            else if (e.Column.FieldName == "APPROVAL_LOGINNAME_DISPLAY")
            {
              string APPROVAL_LOGINNAME = data.APPROVAL_LOGINNAME;
              string APPROVAL_USERNAME = data.APPROVAL_USERNAME;
              e.Value = DisplayName(APPROVAL_LOGINNAME, APPROVAL_USERNAME);
            }
            else if (e.Column.FieldName == "EXP_LOGINNAME_DISPLAY")
            {
              string IMP_LOGINNAME = data.EXP_LOGINNAME;
              string IMP_USERNAME = data.EXP_USERNAME;
              e.Value = DisplayName(IMP_LOGINNAME, IMP_USERNAME);
            }
            else if (e.Column.FieldName == "REQ_LOGINNAME_DISPLAY")
            {
              string Req_loginName = data.REQ_LOGINNAME;
              string Req_UserName = data.REQ_USERNAME;
              e.Value = DisplayName(Req_loginName, Req_UserName);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private string DisplayName(string loginname, string username)
    {
      string value = "";
      try
      {
        if (String.IsNullOrEmpty(loginname) && String.IsNullOrEmpty(username))
        {
          value = "";
        }
        else if (loginname != "" && username == "")
        {
          value = loginname;
        }
        else if (loginname == "" && username != "")
        {
          value = username;
        }
        else if (loginname != "" && username != "")
        {
          value = string.Format("{0} - {1}", loginname, username);
        }
        return value;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
        return value;
      }
    }

    private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
    {
      try
      {
        if (e.RowHandle >= 0)
        {
          long statusIdCheckForButtonEdit = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_STT_ID") ?? "").ToString());
          long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
          long impMediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "IMP_MEDI_STOCK_ID") ?? "").ToString());
          long expMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_TYPE_ID") ?? "").ToString());
          string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
          if (e.Column.FieldName == "EDIT_DISPLAY") // sửa
          {
            if ((creator == LoggingName) &&
                (statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT ||
                statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED ||
                statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST))
            {
              e.RepositoryItem = ButtonEditEnable;
            }
            else
              e.RepositoryItem = ButtonEditDisable;
          }
          else if (e.Column.FieldName == "DISCARD_DISPLAY") //hủy
          {
            if ((creator == LoggingName) &&
                (statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT ||
                statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED ||
                statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST))
            {
              e.RepositoryItem = ButtonDiscardEnable;
            }
            else
              e.RepositoryItem = ButtonDiscardDisable;
          }
          else if (e.Column.FieldName == "APPROVAL_DISPLAY") //duyet
          {
            if (medistock != null && medistock.ID == mediStockId &&
                (statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED ||
                statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST))
            {
              e.RepositoryItem = ButtonApprovalEnable;
            }
            else
              e.RepositoryItem = ButtonApprovalDisable;
          }
          else if (e.Column.FieldName == "DIS_APPROVAL")// Từ chối duyệt
          {
            if (medistock != null && medistock.ID == mediStockId &&
                (statusIdCheckForButtonEdit != Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT &&
                statusIdCheckForButtonEdit != Base.HisExpMestSttCFG.EXP_MEST_STT_ID__EXPORTED &&
                statusIdCheckForButtonEdit != Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED))
            {
              e.RepositoryItem = ButtonDisApprovalEnable;
            }
            else
              e.RepositoryItem = ButtonDisApprovalDisable;
          }
          else if (e.Column.FieldName == "EXPORT_DISPLAY")// thực xuất
          {
            if (medistock != null && medistock.ID == mediStockId &&
                statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__APPROVED)
            {
              e.RepositoryItem = ButtonActualExportEnable;
            }
            else
              e.RepositoryItem = ButtonActualExportDisable;
          }
          else if (e.Column.FieldName == "CHMS_IMP_MEST_CREATE")// Tạo yêu cầu nhập
          {
            if (medistock != null && medistock.ID == impMediStockId &&
                statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__EXPORTED)
            {
              decimal ExistsImp = (decimal)(gridView.GetRowCellValue(e.RowHandle, "EXISTS_IMP") ?? 0);
              if (ExistsImp == 0)
              {
                e.RepositoryItem = ButtonChmsImpEnable;
              }
              else
                e.RepositoryItem = ButtonChmsImpDisable;
            }
            else
              e.RepositoryItem = ButtonChmsImpDisable;
          }
          else if (e.Column.FieldName == "REQUEST_DISPLAY")// Hủy duyệt
          {
            if (medistock != null && medistock.ID == mediStockId &&
                (statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__APPROVED ||
                statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED))
            {
              e.RepositoryItem = ButtonRequest;
            }
            else
              e.RepositoryItem = ButtonRequestDisable;
          }
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
    #endregion

    #region Public method
    public void Search()
    {
      try
      {
        btnSearch_Click(null, null);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Warn(ex);
      }
    }

    public void Refreshs()
    {
      try
      {
        btnRefresh_Click(null, null);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Warn(ex);
      }
    }

    public void FocusExpCode()
    {
      try
      {
        txtExpMestCode.Focus();
        txtExpMestCode.SelectAll();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
    #endregion
  }
}
