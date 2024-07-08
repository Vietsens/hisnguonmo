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
using HIS.UC.FormType.Core.F37.Validation;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.UC.FormType.Core.F37
{
    public partial class UCF37 : DevExpress.XtraEditors.XtraUserControl
    {
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        List<DataGet> Availables = new List<DataGet>();
        List<DataGet> Selecteds = new List<DataGet>();

        public UCF37(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
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

                Availables = this.ProcessInputCode(this.config.JSON_OUTPUT); 
                this.gcolCode.Caption = "Mã " + this.config.DESCRIPTION;

                this.gcolName.Caption = "Tên " + this.config.DESCRIPTION;
                this.gcolCheck.Image = imageCollection1.Images[0];

                this.gcolCodeSelect.Caption = "Mã " + this.config.DESCRIPTION;

                this.gcolNameSelect.Caption = "Tên " + this.config.DESCRIPTION;
                this.gcolSelectedCheck.Image = imageCollection1.Images[0];
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

        private List<DataGet> ProcessInputCode(string jsonOutput)
        {
            List<DataGet> result = new List<DataGet>();
            try
            {
                FormTypeConfig.ReportTypeCode = this.config.REPORT_TYPE_CODE;
                
                List<DataGet> dataSource = HisMultiGetByString.GetByString(FilterConfig.HisFilterTypes(this.config.JSON_OUTPUT.Replace("CODE", "ID")), null);
                if (dataSource != null && dataSource.Count>0)
                {
                    result = dataSource.ToList();
                }
                var splitSub = jsonOutput.Split(new string[] { ":" }, StringSplitOptions.None);
                List<string> tempAvailableCodes = new List<string>();
                if (splitSub != null && splitSub.Count() > 1)
                {
                    string value = splitSub[1].Replace("\"", "");
                    tempAvailableCodes = value.Split(',').ToList();
                    if (result.Count > 0)
                    {
                        result = result.Where(o => tempAvailableCodes.Contains(o.CODE)).ToList();
                    }
                    else
                    {
                        foreach (var item in tempAvailableCodes)
                        {
                            result.Add(new DataGet() { CODE = item });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<DataGet>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SetTitle()
        {
            try
            {
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    lcAvailable.Text = lcAvailable.Text + this.config.DESCRIPTION;
                    lcSelected.Text = lcSelected.Text + this.config.DESCRIPTION;
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
                HIS.UC.FormType.Core.F37.Validation.F37GrandParentValidationRule validRule = new HIS.UC.FormType.Core.F37.Validation.F37GrandParentValidationRule();
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
                //F37ChildValidationRule validRule = new F37ChildValidationRule();
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
                var splitSub = this.config.JSON_OUTPUT.Split(new string[] { ":" }, StringSplitOptions.None);

                value = splitSub[0] + ":\"" + string.Join(",",Selecteds.Where(o=>o.IsChecked).Select(o=>o.CODE).ToList())+"\"";
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
                    if (!this.Selecteds.Exists(o => o.CODE == item.CODE))
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
                    if (!this.Selecteds.Exists(o => o.CODE == item.CODE))
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
                    if (!this.Availables.Exists(o => o.CODE == item.CODE))
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
                foreach (var item in this.Selecteds.Where(o => true== true).ToList())
                {
                    if (!this.Availables.Exists(o => o.CODE == item.CODE))
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

        private void ProcessButtonCheckAllgvAvailable(GridHitInfo hi, GridView view,List<DataGet> listAll)
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
    }
}
