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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using HIS.UC.FormType.Loader;
using HIS.UC.FormType.HisMultiGetString;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.UC.FormType.Base;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraGrid.Columns;

namespace HIS.UC.FormType.MultipleRoomComboCheckFilterByDepartmentComboCheck
{
    public partial class UCMultipleRoomComboCheckFilterByDepartmentComboCheck : DevExpress.XtraEditors.XtraUserControl
    {
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        string[] limitCodes = null;
        string[] Filters = null;
        string jsonOutput;
        private Dictionary<int, List<DataGet>> dicDataAll = new Dictionary<int, List<DataGet>>();

        public UCMultipleRoomComboCheckFilterByDepartmentComboCheck(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                //FormTypeConfig.ReportHight += 250;

                InitializeComponent();
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this.config = config;
                this.isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void Init()
        {
            try
            {
                FormTypeConfig.ReportTypeCode = this.config.REPORT_TYPE_CODE;
                this.jsonOutput = this.config.JSON_OUTPUT;
                string Output0 = "";
                FilterConfig.GetValueOutput0(this.jsonOutput, ref Output0);
                FilterConfig.RemoveStrOutput0(ref jsonOutput);

                this.limitCodes = FilterConfig.GetLimitCodes(this.jsonOutput);
                FilterConfig.GetListfilter(this.jsonOutput, ref this.Filters);
                string[] jsonDescriptions = (this.config.DESCRIPTION ?? "").Split(',');

                if (this.Filters != null && this.Filters.Length > 0)
                {
                    var FDO = FilterConfig.HisFilterTypes("\"" + this.Filters[0]);
                    dicDataAll[0] = HisMultiGetByString.GetByStringLimit(FDO, this.limitCodes, ref Output0);
                    string iDescription = "";
                    if (jsonDescriptions != null && jsonDescriptions.Length > 0)
                    {
                        iDescription = jsonDescriptions[0];
                    }
                    this.InitGridDepartment(iDescription);
                }
                else
                {
                    throw new Exception("this.Filters.Length < 1");
                }

                if (this.Filters != null && this.Filters.Length > 1)
                {
                    var FDO = FilterConfig.HisFilterTypes("\"" + this.Filters[1]);
                    dicDataAll[1] = HisMultiGetByString.GetByStringLimit(FDO, null, ref Output0);
                    string iDescription = "";
                    if (jsonDescriptions != null && jsonDescriptions.Length > 1)
                    {
                        iDescription = jsonDescriptions[1];
                    }
                    this.InitGridRoom(iDescription);
                }
                else
                {
                    throw new Exception("this.Filters.Length < 2");
                }


                GvSetDefaultValue(this.gridViewDepartment, this.dicDataAll[0], Output0, 0);

                SetTitle();
                if (this.report != null)
                {
                    SetValue();
                }
                UpdateDataRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GvSetDefaultValue(GridView gridView, List<DataGet> datasource, string Output0, int indexFilter)
        {
            try
            {
                //Khong select neu da tung select truoc do
                if (datasource != null && datasource.Where(o => o.IsChecked == true).ToList().Count > 0) return;

                List<string> Codes = null;
                this.StringToListCode(Output0, ref Codes);
                this.ReSelectGridViewByCodes(gridView, datasource, Codes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void StringToListCode(string Output0, ref List<string> Codes)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Output0)) return;
                Codes = Output0.Split(',').ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReSelectGridViewByCodes(GridView gridView, List<DataGet> datasource, List<string> Codes)
        {
            try
            {
                if (datasource == null)
                {
                    return;
                }
                if (Codes != null && Codes.Count > 0)
                {
                    datasource.ForEach(o => o.IsChecked = Codes.Contains(o.CODE));
                }
                gridView.GridControl.BeginUpdate();
                gridView.GridControl.DataSource = datasource.OrderBy(o => !o.IsChecked).ToList();
                gridView.GridControl.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReSelectGridViewByIds(GridView gridView, List<DataGet> datasource, List<long> Ids)
        {
            try
            {
                if (Ids != null && Ids.Count > 0)
                {
                    datasource.ForEach(o => o.IsChecked = Ids.Contains(o.ID));
                }
                gridView.GridControl.BeginUpdate();
                gridView.GridControl.DataSource = datasource.OrderBy(o => !o.IsChecked).ToList();
                gridView.GridControl.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitGridDepartment(string description)
        {
            this.gridViewDepartment.BeginUpdate();

            this.gridColumn2.Caption = "Mã ";

            this.gridColumn3.Caption = "Tên " + description;
            this.gridColumn10.Image = this.imageCollection1.Images[0];

            this.gridViewDepartment.GridControl.DataSource = dicDataAll[0].OrderByDescending(o => (o.IsChecked)).ToList();

            this.gridViewDepartment.EndUpdate();
        }

        private void InitGridRoom(string description)
        {
            this.gridViewRoom.BeginUpdate();

            this.gridColumn1.Caption = "Mã ";

            this.gridColumn4.Caption = "Tên " + description;
            this.gridColumn9.Image = this.imageCollection1.Images[0];

            this.gridViewRoom.GridControl.DataSource = dicDataAll[1].OrderByDescending(o => (o.IsChecked)).ToList();

            this.gridViewRoom.EndUpdate();
        }

        private void SetValue()
        {
            try
            {
                if (this.jsonOutput != null && this.report.JSON_FILTER != null)
                {
                    if (Filters != null && Filters.Length > 0)
                    {
                        string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, "\"" + Filters[0], this.report.JSON_FILTER);
                        if (FilterConfig.IsCodeField(Filters[0]))
                        {
                            List<string> codes = new List<string>();
                            try
                            {
                                codes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(value);
                            }
                            catch (Exception)
                            {

                                codes = new List<string>();
                            }
                            if (codes != null)
                            {
                                this.ReSelectGridViewByCodes(gridViewDepartment, dicDataAll[0], codes);
                            }
                        }
                        else
                        {
                            List<long> Ids = null;
                            try
                            {
                                Ids = Newtonsoft.Json.JsonConvert.DeserializeObject<List<long>>(value);
                            }
                            catch (Exception)
                            {

                                Ids = null;
                            }
                            if (Ids != null)
                            {
                                this.ReSelectGridViewByIds(gridViewDepartment, dicDataAll[0], Ids);
                            }
                        }
                    }
                    if (Filters != null && Filters.Length > 1)
                    {
                        string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, "\"" + Filters[1], this.report.JSON_FILTER);

                        if (FilterConfig.IsCodeField(Filters[1]))
                        {
                            List<string> codes = new List<string>();
                            try
                            {
                                codes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(value);
                            }
                            catch (Exception)
                            {

                                codes = new List<string>();
                            }
                            if (codes != null)
                            {
                                this.ReSelectGridViewByCodes(gridViewRoom, grdRoom.DataSource as List<DataGet>, codes);
                            }
                        }
                        else
                        {
                            List<long> Ids = null;
                            try
                            {
                                Ids = Newtonsoft.Json.JsonConvert.DeserializeObject<List<long>>(value);
                            }
                            catch (Exception)
                            {

                                Ids = null;
                            }
                            if (Ids != null)
                            {
                                this.ReSelectGridViewByIds(gridViewRoom, grdRoom.DataSource as List<DataGet>, Ids);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetTitle()
        {
            try
            {
                //if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                //{
                //    lciTitleName.Text = this.config.DESCRIPTION;
                //    lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //}
                //else
                //{
                //    lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //}
                if (this.config != null)
                {
                    //lciTitleName.Text = this.config.DESCRIPTION ?? " ";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        public string GetValue()
        {
            string value = "";
            try
            {
                List<long> DEPARTMENT_IDs = new List<long>();
                List<string> DEPARTMENT_CODEs = new List<string>();
                List<long> ROOM_IDs = new List<long>();
                List<string> ROOM_CODEs = new List<string>();
                if ((this.grdDepartment.DataSource as List<DataGet>) != null && (this.grdDepartment.DataSource as List<DataGet>).Count > 0)
                {
                    DEPARTMENT_IDs = (this.grdDepartment.DataSource as List<DataGet>).Where(o => o.IsChecked == true).Select(s => s.ID).ToList();
                    
                    DEPARTMENT_CODEs = (this.grdDepartment.DataSource as List<DataGet>).Where(o => o.IsChecked == true).Select(s => s.CODE).ToList();
                }
                if (this.isValidData && DEPARTMENT_IDs.Count == 0 && DEPARTMENT_CODEs.Count == 0)
                {
                    WaitingManager.Hide();
                    string[] jsonDescriptions = (this.config.DESCRIPTION ?? "").Split(',');
                    string iDescription = "";
                    if (jsonDescriptions != null && jsonDescriptions.Length > 0)
                    {
                        iDescription = jsonDescriptions[0];
                    }
                    System.Windows.Forms.MessageBox.Show("Chưa chọn " + iDescription);
                }

                if ((this.grdRoom.DataSource as List<DataGet>) != null && (this.grdRoom.DataSource as List<DataGet>).Count > 0)
                {
                    ROOM_IDs = (this.grdRoom.DataSource as List<DataGet>).Where(o => o.IsChecked == true).Select(s => s.ID).ToList();

                   
                    ROOM_CODEs = (this.grdRoom.DataSource as List<DataGet>).Where(o => o.IsChecked == true).Select(s => s.CODE).ToList();
                }
                if (this.isValidData && ROOM_IDs.Count == 0 && ROOM_CODEs.Count == 0)
                {
                    WaitingManager.Hide();
                    string[] jsonDescriptions = (this.config.DESCRIPTION ?? "").Split(',');
                    string iDescription = "";
                    if (jsonDescriptions != null && jsonDescriptions.Length > 1)
                    {
                        iDescription = jsonDescriptions[1];
                    }
                    System.Windows.Forms.MessageBox.Show("Chưa chọn " + iDescription);
                }
                if (!(Filters != null && Filters.Length > 1 && FilterConfig.IsCodeField(Filters[1])))
                {
                    if (DEPARTMENT_IDs.Count > ManagerConstant.MAX_REQUEST_LENGTH_PARAM)
                    {
                        DEPARTMENT_IDs = new List<long>();
                    }
                    if (ROOM_IDs.Count > ManagerConstant.MAX_REQUEST_LENGTH_PARAM)
                    {
                        ROOM_IDs = new List<long>();
                    }
                    value = String.Format(this.jsonOutput, Newtonsoft.Json.JsonConvert.SerializeObject(DEPARTMENT_IDs), Newtonsoft.Json.JsonConvert.SerializeObject(ROOM_IDs));
                }
                else
                {
                    if (DEPARTMENT_CODEs.Count > ManagerConstant.MAX_REQUEST_LENGTH_PARAM)
                    {
                        DEPARTMENT_CODEs = new List<string>();
                    }
                    if (ROOM_CODEs.Count > ManagerConstant.MAX_REQUEST_LENGTH_PARAM)
                    {
                        ROOM_CODEs = new List<string>();
                    }
                    value = String.Format(this.jsonOutput, Newtonsoft.Json.JsonConvert.SerializeObject(DEPARTMENT_CODEs), Newtonsoft.Json.JsonConvert.SerializeObject(ROOM_CODEs));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }

        public bool Valid()
        {
            bool result = true;
            try
            {
                if (this.isValidData)
                {
                    if ((this.grdRoom.DataSource as List<DataGet>) != null && (this.grdRoom.DataSource as List<DataGet>).Where(o => o.IsChecked).ToList().Count > 0
                        && (this.grdDepartment.DataSource as List<DataGet>) != null && (this.grdDepartment.DataSource as List<DataGet>).Where(o => o.IsChecked).ToList().Count > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    UpdateDataDepartment();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtFind__PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    UpdateDataRoom();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool checkdata(DataGet data, string seach)
        {
            bool result = false;
            try
            {
                if (data != null && !String.IsNullOrEmpty(data.CODE))
                {
                    if (String.IsNullOrEmpty(seach))
                        return true;
                    string keyWord = Inventec.Common.String.Convert.UnSignVNese(seach.ToLower());
                    result = (data.CODE != null && data.CODE.ToLower().Contains(keyWord)) || (data.NAME != null && Inventec.Common.String.Convert.UnSignVNese(data.NAME.ToLower()).Contains(keyWord));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void gridViewRoom_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            ProcessButtonCheckAllRoom(hi);
                        }

                    }
                    else if (hi.HitTest == GridHitTest.RowCell)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            ProcessButtonCheckRoom(hi);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessButtonCheckRoom(GridHitInfo hi)
        {
            try
            {
                DataGet dataRow = (DataGet)hi.View.GetRow(hi.RowHandle);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessButtonCheckAllRoom(GridHitInfo hi)
        {
            try
            {
                bool isCheckAll = false;
                if ((this.grdRoom.DataSource as List<DataGet>) != null && (this.grdRoom.DataSource as List<DataGet>).Count > 0)
                {
                    var CheckedNum = (this.grdRoom.DataSource as List<DataGet>).Where(o => o.IsChecked == true).Count();
                    var ServiceNum = (this.grdRoom.DataSource as List<DataGet>).Count();
                    if ((CheckedNum > 0 && CheckedNum < ServiceNum) || CheckedNum == 0)
                    {
                        isCheckAll = true;
                        hi.Column.Image = imageCollection1.Images[1];
                    }

                    if (CheckedNum == ServiceNum)
                    {
                        isCheckAll = false;
                        hi.Column.Image = imageCollection1.Images[0];
                    }

                    if (isCheckAll)
                    {
                        foreach (var item in (this.grdRoom.DataSource as List<DataGet>))
                        {
                            if (item.ID != null)
                            {
                                item.IsChecked = true;
                            }
                        }
                        isCheckAll = false;
                    }
                    else
                    {
                        foreach (var item in (this.grdRoom.DataSource as List<DataGet>))
                        {
                            if (item.ID != null)
                            {
                                item.IsChecked = false;
                            }
                        }
                        isCheckAll = true;
                    }

                    grdRoom.BeginUpdate();
                    grdRoom.DataSource = (this.grdRoom.DataSource as List<DataGet>).OrderByDescending(o => (o.IsChecked)).ToList();
                    grdRoom.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDepartment_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            ProcessButtonCheckAllDepartment(hi);
                        }

                    }
                    else if (hi.HitTest == GridHitTest.RowCell)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            ProcessButtonCheckDepartment(hi);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessButtonCheckDepartment(GridHitInfo hi)
        {
            try
            {

                DataGet dataRow = (DataGet)hi.View.GetRow(hi.RowHandle);
                if (dataRow != null)
                {
                    (this.grdDepartment.DataSource as List<DataGet>).ForEach(o => o.IsChecked = (dataRow.ID == o.ID) ? !o.IsChecked : o.IsChecked);

                    grdDepartment.BeginUpdate();
                    grdDepartment.DataSource = (this.grdDepartment.DataSource as List<DataGet>);
                    grdDepartment.EndUpdate();
                    UpdateDataRoom();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessButtonCheckAllDepartment(GridHitInfo hi)
        {
            try
            {
                bool isCheckAll = false;
                if ((this.grdDepartment.DataSource as List<DataGet>) != null && (this.grdDepartment.DataSource as List<DataGet>).Count > 0)
                {
                    var CheckedNum = (this.grdDepartment.DataSource as List<DataGet>).Where(o => o.IsChecked == true).Count();
                    var ServiceNum = (this.grdDepartment.DataSource as List<DataGet>).Count();
                    if ((CheckedNum > 0 && CheckedNum < ServiceNum) || CheckedNum == 0)
                    {
                        isCheckAll = true;
                        hi.Column.Image = imageCollection1.Images[1];
                    }

                    if (CheckedNum == ServiceNum)
                    {
                        isCheckAll = false;
                        hi.Column.Image = imageCollection1.Images[0];
                    }

                    if (isCheckAll)
                    {
                        foreach (var item in (this.grdDepartment.DataSource as List<DataGet>))
                        {
                            if (item.ID != null)
                            {
                                item.IsChecked = true;
                            }
                        }
                        isCheckAll = false;
                    }
                    else
                    {
                        foreach (var item in (this.grdDepartment.DataSource as List<DataGet>))
                        {
                            if (item.ID != null)
                            {
                                item.IsChecked = false;
                            }
                        }
                        isCheckAll = true;
                    }

                    grdDepartment.BeginUpdate();
                    grdDepartment.DataSource = (this.grdDepartment.DataSource as List<DataGet>).OrderByDescending(o => (o.IsChecked)).ToList();
                    grdDepartment.EndUpdate();
                    UpdateDataRoom();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDataRoom()
        {
            try
            {
                List<DataGet> newDataSource = dicDataAll[1].Where(o => 1 == 1).ToList();
                List<DataGet> dpSelects = new List<DataGet>();
                if (this.grdDepartment.DataSource != null)
                {
                    dpSelects = (this.grdDepartment.DataSource as List<DataGet>).Where(o => o.IsChecked == true).ToList();
                    if (dpSelects.Count > 0)
                    {
                        newDataSource = newDataSource.Where(o => dpSelects.Exists(p => p.ID == o.PARENT)).ToList();
                    }
                }
                if (grdRoom.DataSource != null)
                {
                    List<DataGet> rowSelects = (grdRoom.DataSource as List<DataGet>).Where(o => o.IsChecked == true).ToList();
                    newDataSource.ForEach(o => o.IsChecked = rowSelects.Exists(p => p.ID == o.ID));
                    newDataSource = newDataSource.OrderBy(o => !o.IsChecked).ToList();
                }
                if (!string.IsNullOrWhiteSpace(this.txtFind_.Text))
                {
                    newDataSource = newDataSource.Where(o => o.IsChecked || checkdata(o, this.txtFind_.Text)).ToList();
                    newDataSource = newDataSource.OrderBy(o => !checkdata(o, this.txtFind_.Text)).ToList();
                }

                if (dpSelects != null && dpSelects.Count > 0)
                {
                    newDataSource.ForEach(o => o.IsChecked = true);
                }

                grdRoom.BeginUpdate();
                grdRoom.DataSource = newDataSource;
                grdRoom.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDataDepartment()
        {
            try
            {
                List<DataGet> newDataSource = dicDataAll[0].Where(o => 1 == 1).ToList();
                if (grdDepartment.DataSource != null)
                {
                    List<DataGet> rowSelects = (grdDepartment.DataSource as List<DataGet>).Where(o => o.IsChecked == true).ToList();
                    newDataSource.ForEach(o => o.IsChecked = rowSelects.Exists(p => p.ID == o.ID));
                    newDataSource = newDataSource.OrderBy(o => !o.IsChecked).ToList();
                }
                if (!string.IsNullOrWhiteSpace(this.txtFind.Text))
                {
                    newDataSource = newDataSource.Where(o => o.IsChecked || checkdata(o, this.txtFind.Text)).ToList();
                    newDataSource = newDataSource.OrderBy(o => !checkdata(o, this.txtFind.Text)).ToList();
                }
                grdDepartment.BeginUpdate();
                grdDepartment.DataSource = newDataSource;
                grdDepartment.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
