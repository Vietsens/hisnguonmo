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
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Drawing;
using System;
using HIS.UC.FormType.UserMaterialType;
using HIS.UC.FormType.Loader;
using Inventec.Core;
using System.Collections.Generic;
using HIS.UC.FormType.MaterialTypeCombo.Validation;
using MOS.EFMODEL.DataModels;
using His.UC.LibraryMessage;

namespace HIS.UC.FormType.MaterialTypeCombo
{
    public partial class UCMaterialTypeCombo : DevExpress.XtraEditors.XtraUserControl
    {
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        long? medicineTypeId = 0;
        long? materialTypeId = 0;
        long? fmediStockId = 0;
        List<V_HIS_MEDICINE_BEAN> listmedicine = new List<V_HIS_MEDICINE_BEAN>();
        List<V_HIS_MATERIAL_BEAN> listmaterial = new List<V_HIS_MATERIAL_BEAN>();
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCMaterialTypeCombo(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 100;

                this.config = config;
                SetTitle();
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

        void Init()
        {
            try
            {
                MaterialTypeLoader.LoadDataToCombo1(comboBoxEdit1);
                MaterialTypeLoader.LoadDataToCombo(comboBoxEdit4);
                DepartmentLoader.LoadDataToCombo(comboBoxEdit3);
                MediStockLoader.LoadDataToCombo(comboBoxEdit2);
                if (this.isValidData)
                {
                    Validation();
                    layoutControlItem2.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutControlItem4.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutControlItem6.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutControlItem8.AppearanceItemCaption.ForeColor = Color.Maroon;
                    //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                    
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
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    layoutControlItem2.Text = this.config.DESCRIPTION;
                }
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void txtRoomCode_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {

        //        if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
        //        {
        //            if (comboBoxEdit1.EditValue != null)
        //            {
        //                var department = FormTypeConfig.HisAcsUsers.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit1.EditValue.ToString()));
        //                if (department != null)
        //                {
        //                    textEdit1.Text = department.LOGINNAME;
        //                }
        //            }
        //            System.Windows.Forms.SendKeys.Send("{TAB}");
        //}
        //    }
        //    catch (exception ex)
        //    {
        //        inventec.common.logging.logsystem.error(ex);
        //    }
        //}

        private void cboRoom_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (comboBoxEdit1.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.VHisRooms.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit1.EditValue.ToString()));
                        if (department != null)
                        {
                            textEdit1.Text = department.ROOM_CODE;
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

        public string GetValue()
        {
            string value = "";
            try
            {
                value = String.Format(this.config.JSON_OUTPUT, Newtonsoft.Json.JsonConvert.SerializeObject(medicineTypeId), Newtonsoft.Json.JsonConvert.SerializeObject(materialTypeId), Newtonsoft.Json.JsonConvert.SerializeObject(fmediStockId));
            }
            catch (Exception ex)
            {
                value = null;
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
                    var jsOutputSub = this.config.JSON_OUTPUT.Split(new string[] { "," }, StringSplitOptions.None);
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,jsOutputSub[0], this.report.JSON_FILTER);
                    if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                    {
                        comboBoxEdit1.Properties.DataSource = Config.HisFormTypeConfig.VHisMedicineTypes;
                        comboBoxEdit1.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                    }
                    if (jsOutputSub.Count() > 1)
                    {
                        value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,jsOutputSub[1], this.report.JSON_FILTER);
                        if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                        {
                            comboBoxEdit4.Properties.DataSource = Config.HisFormTypeConfig.VHisMaterialTypes;
                            comboBoxEdit4.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                        }
                    }
                    if (jsOutputSub.Count() > 2)
                    {
                        value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,jsOutputSub[2], this.report.JSON_FILTER);
                        if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                        {
                            comboBoxEdit2.Properties.DataSource = Config.HisFormTypeConfig.VHisMediStock;
                            comboBoxEdit2.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                        }
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
                if (this.isValidData != null && this.isValidData)
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
                DepartmentValidationRule validRule = new DepartmentValidationRule();
                validRule.textEdit2 = textEdit2;
                validRule.comboBoxEdit2 = comboBoxEdit2;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit2, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateMediStock()
        {
            try
            {
                MediStockValidationRule validRule = new MediStockValidationRule();
                validRule.textEdit3 = textEdit3;
                validRule.comboBoxEdit3 = comboBoxEdit3;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit3, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void ValidateMaterial()
        //{
        //    try
        //    {
        //        MaterialTypeValidationRule validRule = new MaterialTypeValidationRule();
        //        validRule.textEdit4 = textEdit4;
        //        validRule.comboBoxEdit4 = comboBoxEdit4;
        //        validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
        //        validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
        //        dxValidationProvider1.SetValidationRule(textEdit4, validRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        //private void ValidateMedicine()
        //{
        //		try
        //		{
        //				MedicineTypeValidationRule validRule = new MedicineTypeValidationRule();
        //				validRule.textEdit1 = textEdit1;
        //				validRule.comboBoxEdit1 = comboBoxEdit1;
        //				validRule.textEdit4 = textEdit4;
        //				validRule.comboBoxEdit4 = comboBoxEdit4;
        //				validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
        //				validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
        //				dxValidationProvider1.SetValidationRule(textEdit1, validRule);
        //				dxValidationProvider1.SetValidationRule(textEdit4, validRule);
        //		}
        //		catch (Exception ex)
        //		{
        //				Inventec.Common.Logging.LogSystem.Error(ex);
        //		}
        //}

        private void Validation()
        {
            try
            {
                // ValidateMedicine();
                //ValidateMaterial();
                ValidateMediStock();
                ValidateDepartment();
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
        #endregion

        private void comboBoxEdit1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (comboBoxEdit1.EditValue != null)
                    {
                        //SetTitle();
                        {
                            medicineTypeId = Config.HisFormTypeConfig.VHisMedicineBeans.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit1.EditValue.ToString())).MEDICINE_TYPE_ID;
                            textEdit1.Text = Config.HisFormTypeConfig.VHisMedicineBeans.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit1.EditValue.ToString())).MEDICINE_TYPE_CODE;
                            comboBoxEdit4.Properties.DataSource = null;
                            textEdit4.Text = "";
                            comboBoxEdit4.EditValue = null;
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

        private void comboBoxEdit4_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (comboBoxEdit4.EditValue != null)
                    {
                        //SetTitle();
                        {
                            materialTypeId = Config.HisFormTypeConfig.VHisMaterialBeans.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit4.EditValue.ToString())).MATERIAL_TYPE_ID;
                            textEdit4.Text = Config.HisFormTypeConfig.VHisMaterialBeans.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit4.EditValue.ToString())).MATERIAL_TYPE_CODE;
                            comboBoxEdit1.Properties.DataSource = null;
                            textEdit1.Text = "";
                            comboBoxEdit1.EditValue = null;
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

        private void comboBoxEdit3_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (comboBoxEdit3.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.HisDepartments.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit3.EditValue.ToString()));
                        if (department != null)
                        {
                            textEdit3.Text = department.DEPARTMENT_CODE;
                            textEdit2.Text = "";
                            MediStockLoader.LoadDataToCombo(comboBoxEdit2);
                            comboBoxEdit2.Properties.DataSource = Config.HisFormTypeConfig.VHisMediStock.Where(o => o.DEPARTMENT_ID == department.ID).ToList();
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

        private void comboBoxEdit2_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (comboBoxEdit2.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.VHisMediStock.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit2.EditValue.ToString()));
                        if (department != null)
                        {
                            textEdit2.Text = department.MEDI_STOCK_CODE;
                            fmediStockId = department.ID;
                            textEdit1.Text = "";
                            textEdit4.Text = "";
                            comboBoxEdit1.EditValue = null;
                            comboBoxEdit4.EditValue = null;

                            //SetTitle();
                            listmedicine = Config.HisFormTypeConfig.VHisMedicineBeanInStocks(department.ID).GroupBy(o => o.MEDICINE_TYPE_ID).Select(p => p.First()).ToList();
                            listmaterial = Config.HisFormTypeConfig.VHisMaterialBeanInStocks(department.ID).GroupBy(o => o.MATERIAL_TYPE_ID).Select(p => p.First()).ToList();
                            comboBoxEdit1.Properties.DataSource = listmedicine;
                            comboBoxEdit4.Properties.DataSource = listmaterial;
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

        private void UCMaterialTypeCombo_Load(object sender, EventArgs e)
        {
            try
            {
                //layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MATERIAL_TYPE_COMBO_LAYOUT_CONTROL_ITEM6", Resources.ResourceLanguageManager.LanguageUCMaterialTypeCombo, Base.LanguageManager.GetCulture());
                //layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MATERIAL_TYPE_COMBO_LAYOUT_CONTROL_ITEM4", Resources.ResourceLanguageManager.LanguageUCMaterialTypeCombo, Base.LanguageManager.GetCulture());
                //layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MATERIAL_TYPE_COMBO_LAYOUT_CONTROL_ITEM2", Resources.ResourceLanguageManager.LanguageUCMaterialTypeCombo, Base.LanguageManager.GetCulture());
                if (this.report != null)
                {
                    SetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void textEdit3_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    var search = textEdit3.Text.Trim().ToLower();
                    if (!String.IsNullOrEmpty(search))
                    {
                        var mediStocks = Config.HisFormTypeConfig.HisDepartments.Where(f => f.DEPARTMENT_CODE.ToLower().Contains(search)).ToList();
                        if (mediStocks != null)
                        {
                            if (mediStocks.Count == 1)
                            {
                                textEdit3.Text = mediStocks[0].DEPARTMENT_CODE;
                                comboBoxEdit3.EditValue = mediStocks[0].ID;
                                System.Windows.Forms.SendKeys.Send("{TAB}");
                            }
                            else
                            {
                                comboBoxEdit3.ShowPopup();
                                comboBoxEdit3.Focus();
                            }
                        }
                    }
                    else
                    {
                        comboBoxEdit3.ShowPopup();
                        comboBoxEdit3.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void textEdit2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    var search = textEdit2.Text.Trim().ToLower();
                    if (!String.IsNullOrEmpty(search))
                    {
                        var mediStocks = Config.HisFormTypeConfig.VHisMediStock.Where(f => f.MEDI_STOCK_CODE.ToLower().Contains(search)).ToList();
                        if (mediStocks != null)
                        {
                            if (mediStocks.Count == 1)
                            {
                                textEdit2.Text = mediStocks[0].MEDI_STOCK_CODE;
                                comboBoxEdit2.EditValue = mediStocks[0].ID;
                                System.Windows.Forms.SendKeys.Send("{TAB}");
                            }
                            else
                            {
                                comboBoxEdit2.ShowPopup();
                                comboBoxEdit2.Focus();
                            }
                        }
                    }
                    else
                    {
                        comboBoxEdit2.ShowPopup();
                        comboBoxEdit2.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void textEdit1_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    var search = textEdit1.Text.Trim().ToLower();
                    if (!String.IsNullOrEmpty(search))
                    {
                        var mediStocks = Config.HisFormTypeConfig.VHisMedicineTypes.Where(f => f.MEDICINE_TYPE_CODE.ToLower().Contains(search)).ToList();
                        if (mediStocks != null)
                        {
                            if (mediStocks.Count == 1)
                            {
                                textEdit1.Text = mediStocks[0].MEDICINE_TYPE_CODE;
                                comboBoxEdit1.EditValue = mediStocks[0].ID;
                                System.Windows.Forms.SendKeys.Send("{TAB}");
                            }
                            else
                            {
                                comboBoxEdit1.ShowPopup();
                                comboBoxEdit1.Focus();
                            }
                        }
                    }
                    else
                    {
                        comboBoxEdit1.ShowPopup();
                        comboBoxEdit1.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void textEdit4_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    var search = textEdit4.Text.Trim().ToLower();
                    if (!String.IsNullOrEmpty(search))
                    {
                        var mediStocks = Config.HisFormTypeConfig.VHisMaterialTypes.Where(f => f.MATERIAL_TYPE_CODE.ToLower().Contains(search)).ToList();
                        if (mediStocks != null)
                        {
                            if (mediStocks.Count == 1)
                            {
                                textEdit4.Text = mediStocks[0].MATERIAL_TYPE_CODE;
                                comboBoxEdit4.EditValue = mediStocks[0].ID;
                                System.Windows.Forms.SendKeys.Send("{TAB}");
                            }
                            else
                            {
                                comboBoxEdit4.ShowPopup();
                                comboBoxEdit4.Focus();
                            }
                        }
                    }
                    else
                    {
                        comboBoxEdit4.ShowPopup();
                        comboBoxEdit4.Focus();
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

