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
//using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.UC.FormType.Base;
using HIS.UC.FormType.Loader;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using His.UC.LibraryMessage;

namespace HIS.UC.FormType.MediStockComboFilterByDepartmentCombo
{
    public partial class UCMediStockComboFilterByDepartmentCombo : DevExpress.XtraEditors.XtraUserControl
    {
        MediStockComboFilterByDepartmentComboFDO generateRDO;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCMediStockComboFilterByDepartmentCombo(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                //FormTypeConfig.ReportHight += 48;

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
                DepartmentLoader.LoadDataToCombo(cboDepartment);
                MediStockLoader.LoadDataToCombo(cboRoom, null);
                if (this.isValidData)
                {
                    Validation();
                    lblTitleName.ForeColor = Color.Maroon;
                    lciMediStock.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciDepartment.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                SetTitle();
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                
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
                //    lblTitleName.Text = this.config.DESCRIPTION;
                //    lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //}
                //else
                //{
                //    lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //}
                if (this.config != null)
                {
                    lciTitleName.Text = this.config.DESCRIPTION ?? " ";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    var search = txtDepartmentCode.Text.Trim().ToLower();
                    if (String.IsNullOrEmpty(search))
                    {
                        cboDepartment.EditValue = null;
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                    else
                    {
                        var listData = Config.HisFormTypeConfig.HisDepartments.Where(o => o.DEPARTMENT_CODE.ToLower().Contains(search)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            txtDepartmentCode.Text = listData.First().DEPARTMENT_CODE;
                            cboDepartment.EditValue = listData.First().ID;
                            txtRoomCode.Text = "";
                            cboRoom.EditValue = null;
                            cboRoom.Properties.DataSource = Config.HisFormTypeConfig.VHisMediStock.Where(o => o.DEPARTMENT_ID == listData.First().ID).ToList();
                            System.Windows.Forms.SendKeys.Send("{TAB}");
                        }
                        else
                        {
                            cboDepartment.Focus();
                            cboDepartment.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.HisDepartments.FirstOrDefault(f => f.ID == long.Parse(cboDepartment.EditValue.ToString()));
                        if (department != null)
                        {
                            txtDepartmentCode.Text = department.DEPARTMENT_CODE;
                            txtRoomCode.Text = "";
                            cboRoom.EditValue = null;
                            cboRoom.Properties.DataSource = Config.HisFormTypeConfig.VHisMediStock.Where(o => o.DEPARTMENT_ID == department.ID).ToList();
                        }
                    }
                    System.Windows.Forms.SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.HisDepartments.FirstOrDefault(f => f.ID == long.Parse(cboDepartment.EditValue.ToString()));
                        if (department != null)
                        {
                            txtDepartmentCode.Text = department.DEPARTMENT_CODE;
                            txtRoomCode.Text = "";
                            cboRoom.EditValue = null;
                            cboRoom.Properties.DataSource = Config.HisFormTypeConfig.VHisMediStock.Where(o => o.DEPARTMENT_ID == department.ID).ToList();
                        }
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRoomCode_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    var search = txtRoomCode.Text.Trim().ToLower();
                    long department_id = (long)(cboDepartment.EditValue ?? 0);
                    if (!String.IsNullOrEmpty(search))
                    {
                        var mediStocks = Config.HisFormTypeConfig.VHisMediStock.Where(f => f.DEPARTMENT_ID == department_id && f.MEDI_STOCK_CODE.Contains(search)).ToList();
                        if (mediStocks != null)
                        {
                            if (mediStocks.Count == 1)
                            {
                                txtRoomCode.Text = mediStocks[0].MEDI_STOCK_CODE;
                                cboRoom.Properties.DataSource = mediStocks[0].ID;
                                System.Windows.Forms.SendKeys.Send("{TAB}");
                            }
                            else
                            {
                                cboRoom.ShowPopup();
                                cboRoom.Focus();
                            }
                        }
                    }
                    else
                    {
                        cboRoom.ShowPopup();
                        cboRoom.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboRoom.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.VHisMediStock.FirstOrDefault(f => f.ID == long.Parse(cboRoom.EditValue.ToString()));
                        if (department != null)
                        {
                            txtRoomCode.Text = department.MEDI_STOCK_CODE;
                        }
                    }
                    System.Windows.Forms.SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (cboRoom.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.VHisMediStock.FirstOrDefault(f => f.ID == long.Parse(cboRoom.EditValue.ToString()));
                        if (department != null)
                        {
                            txtRoomCode.Text = department.MEDI_STOCK_CODE;
                        }
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Get value in form return in object
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            string value = "";
            try
            {
                long? mediStockId = (long?)cboRoom.EditValue;
                value = String.Format(this.config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(mediStockId));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }

        public void SetValue()
        {
            try
            {
                if (this.config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                {
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,this.config.JSON_OUTPUT, this.report.JSON_FILTER);
                    if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                    {
                        txtRoomCode.Text = (Config.HisFormTypeConfig.HisMediStocks.FirstOrDefault(f => f.ID == Inventec.Common.TypeConvert.Parse.ToInt64(value)) ?? new MOS.EFMODEL.DataModels.HIS_MEDI_STOCK()).MEDI_STOCK_CODE;
                        cboRoom.Properties.DataSource = Config.HisFormTypeConfig.HisMediStocks;
                        cboRoom.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                    }
                }
               
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

        #region Validation
        private void ValidateDepartment()
        {
            try
            {
                HIS.UC.FormType.MediStockComboFilterByDepartmentCombo.Validation.DepartmentValidationRule validRule = new HIS.UC.FormType.MediStockComboFilterByDepartmentCombo.Validation.DepartmentValidationRule();
                validRule.cboDepartment = cboDepartment;
                validRule.txtDepartmentCode = txtDepartmentCode;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtDepartmentCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateRoom()
        {
            try
            {
                HIS.UC.FormType.MediStockComboFilterByDepartmentCombo.Validation.RoomValidationRule validRule = new HIS.UC.FormType.MediStockComboFilterByDepartmentCombo.Validation.RoomValidationRule();
                validRule.cboRoom = cboRoom;
                validRule.txtRoomCode = txtRoomCode;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtRoomCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Validation()
        {
            try
            {
                ValidateDepartment();
                ValidateRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
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
        #endregion

        private void UCMediStockComboFilterByDepartmentCombo_Load(object sender, EventArgs e)
        {
            try
            {
                lciDepartment.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MEDISTOCK_COMBO_FILTER_BY_DEPARTMENT_COMBO_LCI_DEPARTMENT", Resources.ResourceLanguageManager.LanguageUCMediStockComboFilterByDepartmentCombo, Base.LanguageManager.GetCulture());
                lciMediStock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MEDISTOCK_COMBO_FILTER_BY_DEPARTMENT_COMBO_LCI_MEDISTOCK", Resources.ResourceLanguageManager.LanguageUCMediStockComboFilterByDepartmentCombo, Base.LanguageManager.GetCulture());
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

    }
}
