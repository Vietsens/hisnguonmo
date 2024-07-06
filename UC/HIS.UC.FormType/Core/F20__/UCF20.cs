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
using Inventec.Common.Controls.EditorLoader;

namespace HIS.UC.FormType.F20
{
    public partial class UCF20 : DevExpress.XtraEditors.XtraUserControl
    {
        private Dictionary<int, List<DataGet>> dicParentDataAll = new Dictionary<int, List<DataGet>>();
        private List<DataGet> childDataAll = null;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        //string FDO = null;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        string[] limitCodes = null;
        string[] Filters = null;
        DevExpress.XtraEditors.GridLookUpEdit[] cbo = null;
        DevExpress.XtraLayout.LayoutControlItem[] lci = null;
        public UCF20(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
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
                this.LoadControl();

                SetTitle();
                if (this.report != null)
                {
                    SetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadControl()
        {
            try
            {
                //string Limit = dicFilter.FirstOrDefault(o => o.Key.Contains("_LIMIT_CODE")).Value;
                this.limitCodes = FilterConfig.GetLimitCodes(this.config.JSON_OUTPUT);
                FilterConfig.GetListfilter(this.config.JSON_OUTPUT, ref this.Filters);
                string[] jsonDescriptions = (this.config.DESCRIPTION ?? "").Split(',');
                cbo = new DevExpress.XtraEditors.GridLookUpEdit[]
                    {
                    this.cboParent,
                    this.cboParent1,
                    this.cboParent2,
                    this.cboParent3,
                    this.cboParent4
                    };
                lci = new DevExpress.XtraLayout.LayoutControlItem[]
                    {
                    this.lciParent,
                    this.lciParent1,
                    this.lciParent2,
                    this.lciParent3,
                    this.lciParent4
                    };
                for (int i = 0; i < Filters.Length; i++)
                {
                    if (i == 0)
                    {
                        this.InitGridChild("\"" + Filters[i], jsonDescriptions[i]);
                    }
                    else
                    {
                        this.InitCombo("\"" + Filters[i], cbo[i - 1], lci[i - 1], jsonDescriptions[i], i);

                    }
                }
                for (int i = Filters.Length - 1; i < lci.Length; i++)
                {
                    lci[i].Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InitCombo(string jsonOutput, GridLookUpEdit gridLookUpEdit, DevExpress.XtraLayout.LayoutControlItem layoutControlItem, string description, int i)
        {
            try
            {
                var FDO = FilterConfig.HisFilterTypes(jsonOutput);
                string output0 = "";
                dicParentDataAll[i] = HisMultiGetByString.GetByStringLimit(FDO, (i == this.Filters.Length - 1 ? this.limitCodes : null), ref output0);

                InitComboParent(gridLookUpEdit, description, dicParentDataAll[i]);
                layoutControlItem.Text = description;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboParent(DevExpress.XtraEditors.GridLookUpEdit cbo, string description, List<DataGet> data)
        {


            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("CODE", "Mã " + description, 100, 1));
            columnInfos.Add(new ColumnInfo("NAME", "Tên " + description, 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 350);
            ControlEditorLoader.Load(cbo, data, controlEditorADO);
        }

        private void InitGridChild(string jsonOutput, string description)
        {
            gvChild.BeginUpdate();
            var FDO = FilterConfig.HisFilterTypes(jsonOutput);
            string output0 = "";
            childDataAll = HisMultiGetByString.GetByStringLimit(FDO, null, ref output0);
            gvChild.GridControl.DataSource = childDataAll.OrderByDescending(o => (o.IsChecked)).ToList();
            this.gridColumn1.Caption = "Mã " + description;
            this.gridColumn4.Caption = "Tên " + description;
            this.gridColumn9.Image = imageCollection1.Images[0];
            gvChild.EndUpdate();
        }

        private void SetValue()
        {
            try
            {
                if (this.config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                {
                    for (int i = 0; i < Filters.Length; i++)
                    {
                        if (i == 0)
                        {
                            string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, "\"" + Filters[i], this.report.JSON_FILTER);
                            List<long> Ids = null;
                            if (value != null && value != "null" && value != "")
                            {
                                try
                                {
                                    Ids = (value.Substring(1)).Substring(0, value.Substring(1).Length - 1).Split(new string[] { "," }, StringSplitOptions.None)
                                        .Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o)).ToList();
                                }
                                catch (Exception)
                                {

                                    Ids = null;
                                }
                                if (Ids != null)
                                {
                                    if (this.childDataAll != null)
                                    {
                                        foreach (long id in Ids)
                                        {
                                            var RowCheck = childDataAll.FirstOrDefault(o => o.ID == id);
                                            if (RowCheck != null)
                                            {
                                                RowCheck.IsChecked = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, "\"" + Filters[i], this.report.JSON_FILTER);

                            if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                            {
                                cbo[i - 1].EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
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
                List<long> ROOM_IDs = new List<long>();

                if (childDataAll != null && childDataAll.Count > 0)
                {
                    ROOM_IDs = childDataAll.Where(o => (cboParent.EditValue == null || o.PARENT == (long)cboParent.EditValue) && o.IsChecked == true).Select(s => s.ID).ToList();

                    if (ROOM_IDs.Count > ManagerConstant.MAX_REQUEST_LENGTH_PARAM)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show(string.Format("Điều kiện lọc \"" + this.config.DESCRIPTION + "\"" + " quá giới hạn cho phép (Giới hạn: {0})", ManagerConstant.MAX_REQUEST_LENGTH_PARAM));
                        return "error";
                    }
                }
                for (int i = 0; i < Filters.Length; i++)
                {
                    if (i == 0)
                    {
                        value = this.config.JSON_OUTPUT.Replace("{" + i.ToString() + "}", Newtonsoft.Json.JsonConvert.SerializeObject(ROOM_IDs));
                    }
                    else
                    {
                        value = value.Replace("{" + i.ToString() + "}", ConvertUtils.ConvertToObjectFilter((long?)cbo[i - 1].EditValue).ToString());
                    }
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
                    //var rows = gridViewDepartment.GetSelectedRows();
                    //var rowRooms = gridViewRoom.GetSelectedRows();
                    //if (rows == null || rows.Count() == 0 || rowRooms == null || rowRooms.Count() == 0)
                    //{
                    //    result = false;
                    //}
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
                    gvChild.BeginUpdate();
                    gvChild.GridControl.DataSource = childDataAll.Where(o => cboParent.EditValue == null || o.PARENT == (long)cboParent.EditValue).OrderByDescending(p => checkdata(p, txtFind.Text)).ThenByDescending(o => (o.IsChecked)).ToList();
                    gvChild.EndUpdate();
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

                    result = (data.CODE != null && data.CODE.ToLower().Contains(seach.ToLower())) || (data.NAME != null && data.NAME.ToLower().Contains(seach.ToLower()));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void gvChild_MouseDown(object sender, MouseEventArgs e)
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
                            ProcessButtonCheckAllChild(hi);
                        }

                    }
                    else if (hi.HitTest == GridHitTest.RowCell)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            ProcessButtonCheckChild(hi);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessButtonCheckChild(GridHitInfo hi)
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

        private void ProcessButtonCheckAllChild(GridHitInfo hi)
        {
            try
            {
                bool isCheckAll = false;
                if (childDataAll != null && childDataAll.Count > 0)
                {
                    var CheckedNum = childDataAll.Where(o => o.IsChecked == true).Count();
                    var ServiceNum = childDataAll.Count();
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
                        foreach (var item in childDataAll)
                        {
                            item.IsChecked = true;
                        }
                        isCheckAll = false;
                    }
                    else
                    {
                        foreach (var item in childDataAll)
                        {
                            item.IsChecked = false;
                        }
                        isCheckAll = true;
                    }

                    gcChild.BeginUpdate();
                    gcChild.DataSource = childDataAll.OrderByDescending(o => (o.IsChecked)).ToList();
                    gcChild.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboParent.EditValue != null)
                    {
                        gcChild.BeginUpdate();
                        gcChild.DataSource = childDataAll.Where(o => (long)cboParent.EditValue == o.PARENT).OrderByDescending(o => (o.IsChecked)).ToList();
                        gcChild.EndUpdate();
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboParent.Focus();
                        cboParent.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboParent1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboParent1.EditValue != null)
                    {
                        cboParent.Properties.BeginUpdate();
                        cboParent.Properties.DataSource = dicParentDataAll[1].Where(o => (long)cboParent1.EditValue == o.PARENT).ToList();
                        cboParent.Properties.EndUpdate();
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboParent1.Focus();
                        cboParent1.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboParent2_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboParent2.EditValue != null)
                    {
                        cboParent1.Properties.BeginUpdate();
                        cboParent1.Properties.DataSource = dicParentDataAll[2].Where(o => (long)cboParent2.EditValue == o.PARENT).ToList();
                        cboParent1.Properties.EndUpdate();
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboParent2.Focus();
                        cboParent2.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboParent3_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboParent3.EditValue != null)
                    {
                        cboParent2.Properties.BeginUpdate();
                        cboParent2.Properties.DataSource = dicParentDataAll[3].Where(o => (long)cboParent3.EditValue == o.PARENT).ToList();
                        cboParent2.Properties.EndUpdate();
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboParent3.Focus();
                        cboParent3.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboParent4_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboParent4.EditValue != null)
                    {
                        cboParent3.Properties.BeginUpdate();
                        cboParent3.Properties.DataSource = dicParentDataAll[4].Where(o => (long)cboParent4.EditValue == o.PARENT).ToList();
                        cboParent3.Properties.EndUpdate();
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboParent4.Focus();
                        cboParent4.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
