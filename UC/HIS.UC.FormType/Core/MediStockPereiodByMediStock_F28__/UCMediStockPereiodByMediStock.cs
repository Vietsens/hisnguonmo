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
using DevExpress.XtraEditors;
using DevExpress.Utils;
using HIS.UC.FormType.Loader;
using His.UC.LibraryMessage;
using Inventec.Core;

namespace HIS.UC.FormType.MediStockPereiodByMediStock
{
    public partial class UCMediStockPereiodByMediStock : DevExpress.XtraEditors.XtraUserControl
    {
        private List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> selectedDepartments;
        bool isValidData = false;
        private WaitDialogForm waitLoad;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        int positionHandleControl = -1;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCMediStockPereiodByMediStock(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 60;
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
        //Nạp dữ liệu lên các UC:
        void Init()
        {
            try
            {
                if (this.isValidData)
                {
                    ValidateDepartment();
                    layoutControlItem7.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                
                //DepartmentLoader.LoadDataToCombo(comboBoxEdit1);
                //MediStockLoader.LoadDataToCombo(comboBoxEdit2);
                MediStockPeriodLoader.LoadDataToCombo(comboBoxEdit3);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateDepartment()
        {
            try
            {
                HIS.UC.FormType.DepartmentCombo.Validation.DepartmentValidationRule validRule = new HIS.UC.FormType.DepartmentCombo.Validation.DepartmentValidationRule();
                validRule.cboDepartment = comboBoxEdit3;
                validRule.txtDepartmentCode = textEdit3;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit3, validRule);
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
                if (radioButton1.Checked == true)
                {
                    long? mediStockPeriodId = (long?)comboBoxEdit3.EditValue;
                    long? mediStockId = null;
                    value = String.Format(this.config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(mediStockPeriodId), ConvertUtils.ConvertToObjectFilter(mediStockId));
                }
                else
                {
                    long? mediStockPeriodId = null;

                    List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_PERIOD> FillHisMediStockPeriod = Config.HisFormTypeConfig.VHisMediStockPeriod.Where(o => o.ID == (long)comboBoxEdit3.EditValue).ToList();

                    long? mediStockId = (long?)FillHisMediStockPeriod.Select(o => o.MEDI_STOCK_ID).ToList()[0];
                    value = String.Format(this.config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(mediStockPeriodId), ConvertUtils.ConvertToObjectFilter(mediStockId));
                }
            }
            catch (Exception ex)
            {
                value = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
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

        // Đặt giá trị mã kho và tên kho khi kết thúc chọn kho:
        private void comboBoxEdit1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            //try
            //{
            //    if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
            //    {
            //        if (comboBoxEdit1.EditValue != null)
            //        {
            //            var department = FormTypeConfig.HisDepartments.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit1.EditValue.ToString()));
            //            if (department != null)
            //            {
            //                textEdit1.Text = department.DEPARTMENT_CODE;
            //                textEdit2.Text = "";
            //                comboBoxEdit2.EditValue = null;
            //                comboBoxEdit2.Properties.DataSource = FormTypeConfig.VHisMediStocks.Where(o => o.DEPARTMENT_ID == department.ID).ToList();
            //            }
            //        }
            //        System.Windows.Forms.SendKeys.Send("{TAB}");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void comboBoxEdit3_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (comboBoxEdit3.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.VHisMediStockPeriod.FirstOrDefault(f => f.ID == long.Parse(comboBoxEdit3.EditValue.ToString()));
                        if (department != null)
                        {
                            textEdit3.Text = department.MEDI_STOCK_PERIOD_NAME;

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

        private void UCMediStockPereiodByMediStock_Load(object sender, EventArgs e)
        {
            try
            {
                //layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MEDISTOCK_PERIOD_BY_MEDISTOCK_LAYOUT_CONTROL_ITEM5", Resources.ResourceLanguageManager.LanguageUCMediStockPereiodByMediStock, Base.LanguageManager.GetCulture());
                //layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MEDISTOCK_PERIOD_BY_MEDISTOCK_LAYOUT_CONTROL_ITEM6", Resources.ResourceLanguageManager.LanguageUCMediStockPereiodByMediStock, Base.LanguageManager.GetCulture());
                layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MEDISTOCK_PERIOD_BY_MEDISTOCK_LAYOUT_CONTROL_ITEM7", Resources.ResourceLanguageManager.LanguageUCMediStockPereiodByMediStock, Base.LanguageManager.GetCulture());
                if (this.report != null)
                {
                    //SetValue();
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
                bool showCbo = true;
                if (!String.IsNullOrEmpty(textEdit3.Text.Trim()))
                {
                    string code = textEdit3.Text.Trim().ToLower();
                    var listData = Config.HisFormTypeConfig.VHisMediStockPeriod.Where(o => o.MEDI_STOCK_PERIOD_NAME.ToLower().Contains(code)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.MEDI_STOCK_PERIOD_NAME.ToLower() == code).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        textEdit3.Text = result.First().MEDI_STOCK_PERIOD_NAME;
                        comboBoxEdit3.EditValue = result.First().ID;
                    }
                }
                if (showCbo)
                {
                    comboBoxEdit3.Focus();
                    comboBoxEdit3.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
