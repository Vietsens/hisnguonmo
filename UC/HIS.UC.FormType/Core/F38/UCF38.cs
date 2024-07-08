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
//using System.Windows.Forms;
using HIS.UC.FormType.Loader;
using HIS.UC.FormType.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using MOS.EFMODEL.DataModels;
using His.UC.LibraryMessage;
using HIS.UC.FormType.HisMultiGetString;
using DevExpress.XtraGrid.Columns;
using Inventec.Common.Logging;
using HIS.UC.FormType.Core.F38.Validation;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.UC.FormType.PagingGet;
using Inventec.Core;
using DevExpress.XtraEditors.Controls;

namespace HIS.UC.FormType.Core.F38
{
    public partial class UCF38 : DevExpress.XtraEditors.XtraUserControl
    {
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        List<DataGet> Availables = new List<DataGet>();
        List<DataGet> Selecteds = new List<DataGet>();
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        string FDO = "";
        string[] descriptions;
        public UCF38(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();

                this.config = config;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this.isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Init()
        {
            try
            {
                descriptions = (this.config.DESCRIPTION ?? "").Split(',');
                if (descriptions.Length > 1)
                {
                    this.lcParentCode.Text = descriptions[1];
                }
                this.gcolCode.Caption = "Mã " + descriptions[0];

                this.gcolName.Caption = "Tên " + descriptions[0];
                this.gcolCheck.Image = imageCollection1.Images[0];

                this.gcolCodeSelect.Caption = "Mã " + descriptions[0];

                this.gcolNameSelect.Caption = "Tên " + descriptions[0];
                this.txtKeyWord.Properties.NullValuePrompt = "Tìm kiếm theo mã " + descriptions[0];
                this.gcolSelectedCheck.Image = imageCollection1.Images[0];
                FillDataToCbo();
                FillDataToGrid();
                UpdateGrid();
                SetTitle();
                if (this.report != null)
                {
                    SetValue();
                }

                Validation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToCbo()
        {
            try
            {
                string[] jsonOuts = this.config.JSON_OUTPUT.Split(',');
                if (jsonOuts.Length <= 1)
                {
                    this.lcParentCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lcParentName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    return;
                }
                string jsonOutParent = jsonOuts[1];
                FDO = FilterConfig.HisFilterTypes(jsonOuts[1]);
                cboParentName.Properties.DataSource = HisMultiGetByString.GetByString(FDO, null);
                cboParentName.Properties.DisplayMember = "NAME";
                cboParentName.Properties.ValueMember = "ID";

                cboParentName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboParentName.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboParentName.Properties.ImmediatePopup = true;
                cboParentName.ForceInitialize();
                cboParentName.Properties.View.Columns.Clear();
                cboParentName.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboParentName.Properties.View.Columns.AddField("CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = false;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboParentName.Properties.View.Columns.AddField("NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = false;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
                aColumnCode.Visible = true;
                aColumnName.Visible = true;
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
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = 0;
                }
                GridPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(GridPaging, param, pageSize, this.gvAvailable.GridControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }


        private void UpdateGrid()
        {
            try
            {
                gvAvailable.BeginUpdate();

                gvAvailable.GridControl.DataSource = Availables;

                gvAvailable.EndUpdate();

                gvSelected.BeginUpdate();

                gvSelected.GridControl.DataSource = Selecteds;
                gvSelected.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(startPage, limit);
                long? parentId = null;
                if (cboParentName.EditValue is long)
                {
                    parentId = long.Parse(cboParentName.EditValue.ToString());
                }
                IPagingGet behavior = PagingGetFactory.MakeIPagingGet(paramCommon, this.config.JSON_OUTPUT, this.txtKeyWord.Text, parentId);
                ApiResultObject<List<HIS.UC.FormType.HisMultiGetString.DataGet>> apiResult = behavior != null ? behavior.Run() : new ApiResultObject<List<HIS.UC.FormType.HisMultiGetString.DataGet>>();
                Availables = apiResult.Data;
                this.pageSize = apiResult.Param != null ? apiResult.Param.Limit ?? 0 : 0;
                {
                    if (Availables != null && Availables.Count > 0)
                    {
                        gvAvailable.BeginUpdate();

                        gvAvailable.GridControl.DataSource = Availables;

                        gvAvailable.EndUpdate();
                        rowCount = (Availables == null ? 0 : Availables.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gvAvailable.GridControl.DataSource = null;
                        rowCount = (Availables == null ? 0 : Availables.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTitle()
        {
            try
            {
                if (this.config != null && !String.IsNullOrEmpty(descriptions[0]))
                {
                    lcAvailable.Text = lcAvailable.Text;
                    lcSelected.Text = lcSelected.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Validation()
        {
            try
            {
                if (this.isValidData)
                {
                    layoutControlItem2.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateServiceType();
                    //layoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;
                    //ValidateService();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateServiceType()
        {
            try
            {
                HIS.UC.FormType.Core.F38.Validation.F38GrandParentValidationRule validRule = new HIS.UC.FormType.Core.F38.Validation.F38GrandParentValidationRule();
                //validRule.cboServiceType = cboServiceType;
                //validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //dxValidationProvider1.SetValidationRule(cboServiceType, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateService()
        {
            try
            {
                //F38ChildValidationRule validRule = new F38ChildValidationRule();
                //validRule.cboService = cboService;
                //validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                //validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //dxValidationProvider1.SetValidationRule(cboService, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetValue()
        {
            string value = "";
            try
            {
                long? parentId = null;
                if (cboParentName.EditValue is long)
                {
                    parentId = long.Parse(cboParentName.EditValue.ToString());
                }
                value = String.Format(this.config.JSON_OUTPUT, Newtonsoft.Json.JsonConvert.SerializeObject(Selecteds.Where(o => o.IsChecked).Select(o => o.ID).ToList()), ConvertUtils.ConvertToObjectFilter(parentId));
            }
            catch (Exception ex)
            {
                value = null;
                LogSystem.Warn(ex);
            }

            return value;
        }

        public void SetValue()
        {
            try
            {
                //if (this.config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                //{

                //    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, this.config.JSON_OUTPUT, this.report.JSON_FILTER);

                //    if (value != null && value != "null")
                //    {
                //        this.meSelectedCodes.Text = value.Replace("\"", "").Replace(",","\r\n");

                //    }

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool Valid()
        {
            bool result = true;
            try
            {
                if (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    this.positionHandleControl = -1;
                    result = dxValidationProvider1.Validate();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void dxValidationProvider1_ValidationFailed_1(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in this.Availables.Where(o => o.IsChecked == true).ToList())
                {
                    if (!this.Selecteds.Exists(o => o.ID == item.ID))
                    {
                        this.Selecteds.Add(item);
                    }
                }
                this.Availables = this.Availables.Where(o => o.IsChecked != true).ToList();
                UpdateGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in this.Availables.Where(o => true == true).ToList())
                {
                    if (!this.Selecteds.Exists(o => o.ID == item.ID))
                    {
                        this.Selecteds.Add(item);
                    }
                }
                this.Availables.Clear();
                UpdateGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {

                foreach (var item in this.Selecteds.Where(o => o.IsChecked == true).ToList())
                {
                    if (!this.Availables.Exists(o => o.ID == item.ID))
                    {
                        this.Availables.Add(item);
                    }
                }
                this.Selecteds = this.Selecteds.Where(o => o.IsChecked != true).ToList();
                UpdateGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in this.Selecteds.Where(o => true == true).ToList())
                {
                    if (!this.Availables.Exists(o => o.ID == item.ID))
                    {
                        this.Availables.Add(item);
                    }
                }
                this.Selecteds.Clear();
                UpdateGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gvAvailable_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
                            ProcessButtonCheckAllgvAvailable(hi, view, Availables);
                        }

                    }
                    else if (hi.HitTest == GridHitTest.RowCell)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            ProcessButtonCheckgvAvailable(hi, view, Availables);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessButtonCheckgvAvailable(GridHitInfo hi, GridView view, List<DataGet> listAll)
        {
            //try
            //{
            //    DataGet dataRow = (DataGet)hi.View.GetRow(hi.RowHandle);
            //    if (dataRow != null && dataRow.IsChecked == false)
            //    {
            //        grdRoom.BeginUpdate();
            //        grdRoom.DataSource = childDataAll.OrderByDescending(o => o.PARENT == dataRow.ID).ThenByDescending(o => (o.IsChecked)).ToList();
            //        grdRoom.EndUpdate();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void ProcessButtonCheckAllgvAvailable(GridHitInfo hi, GridView view, List<DataGet> listAll)
        {
            try
            {
                bool isCheckAll = false;
                if (listAll != null && listAll.Count > 0)
                {
                    var CheckedNum = listAll.Where(o => o.IsChecked == true).Count();
                    var ServiceNum = listAll.Count();
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
                        foreach (var item in listAll)
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
                        foreach (var item in listAll)
                        {
                            if (item.ID != null)
                            {
                                item.IsChecked = false;
                            }
                        }
                        isCheckAll = true;
                    }

                    view.BeginUpdate();
                    view.GridControl.DataSource = listAll.OrderByDescending(o => (o.IsChecked)).ToList();
                    view.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gvSelected_MouseDown(object sender, MouseEventArgs e)
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
                            ProcessButtonCheckAllgvAvailable(hi, view, Selecteds);
                        }

                    }
                    else if (hi.HitTest == GridHitTest.RowCell)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            ProcessButtonCheckgvAvailable(hi, view, Selecteds);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtParentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {

                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtParentCode.Text.Trim()))
                    {
                        string code = txtParentCode.Text.Trim().ToLower();
                        var listData = HisMultiGetByString.GetByString(FDO, null).Where(o => o.CODE.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.CODE.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtParentCode.Text = result.First().CODE;
                            cboParentName.EditValue = result.First().ID;
                        }
                    }
                    if (showCbo)
                    {
                        cboParentName.Focus();
                        cboParentName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboParentName_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboParentName.EditValue = null;
                    cboParentName.Properties.Buttons[1].Visible = false;
                    txtParentCode.Text = null;
                    txtParentCode.Focus();
                    txtParentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParentName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboParentName.EditValue != null)
                    {
                        var parent = HisMultiGetByString.GetByString(FDO, null).FirstOrDefault(f => f.ID == long.Parse(cboParentName.EditValue.ToString()));
                        if (parent != null)
                        {
                            txtParentCode.Text = parent.CODE;
                        }

                        cboParentName.Properties.Buttons[1].Visible = true;
                        this.FillDataToGrid();
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboParentName.Focus();
                        cboParentName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboParentName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (cboParentName.EditValue != null)
                    {
                        var parent = HisMultiGetByString.GetByString(FDO, null).FirstOrDefault(f => f.ID == long.Parse(cboParentName.EditValue.ToString()));
                        if (parent != null)
                        {
                            txtParentCode.Text = parent.CODE;
                        }
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboParentName.Focus();
                        cboParentName.ShowPopup();
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
