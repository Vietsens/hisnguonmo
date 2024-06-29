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
using HIS.UC.DHST.ADO;
using HIS.UC.DHST.Base;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using MOS.EFMODEL.DataModels;
using System.Resources;

namespace HIS.UC.DHST.Run
{
    public partial class UCDHST : UserControl
    {
        DHSTInitADO dhstInit;
        public long positionHandle = -1;
        bool isRequestVali;
        public UCDHST()
        {
            InitializeComponent();
        }

        public UCDHST(DHSTInitADO _dhstInit)
        {
            InitializeComponent();
            try
            {
                this.dhstInit = _dhstInit;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinPulse_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBloodPressureMax.Focus();
                    spinBloodPressureMax.SelectAll();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBelly_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinUrine.Focus();
                    spinUrine.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodPressureMin_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinWeight.Focus();
                    spinWeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodPressureMax_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBloodPressureMin.Focus();
                    spinBloodPressureMin.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBreathRate_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinChest.Focus();
                    spinChest.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinWeight_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinHeight.Focus();
                    spinHeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHeight_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSPO2.Focus();
                    spinSPO2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChest_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBelly.Focus();
                    spinBelly.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTemperature_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBreathRate.Focus();
                    spinBreathRate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCDHST_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                //Set null
                //Validate
                if (dhstInit != null)
                {
                    InitLanguage();
                    ValidateControl();
                    VisibleControl();
                }

                dtExecuteTime.DateTime = DateTime.Now;
                spinBloodPressureMin.EditValue = null;
                spinBloodPressureMax.EditValue = null;
                spinBreathRate.EditValue = null;
                spinHeight.EditValue = null;
                spinChest.EditValue = null;
                spinBelly.EditValue = null;
                spinPulse.EditValue = null;
                spinTemperature.EditValue = null;
                spinWeight.EditValue = null;
                spinSPO2.EditValue = null;
                txtNote.Text = "";
                spinUrine.EditValue = null;
                spinCapillaryBloodGlucose.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisibleControl()
        {
            try
            {
                if (dhstInit == null) return;

                if (dhstInit.IsVisibleUrineAndCapillaryBloodGlucose)
                {
                    lciUrine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciCapillaryBloodGlucose.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else 
                {
                    lciUrine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciCapillaryBloodGlucose.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void spinHeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                fillDataToBmiAndLeatherArea();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void fillDataToBmiAndLeatherArea()
        {
            try
            {
                decimal bmi = 0;
                if (spinHeight.Value != null && spinHeight.Value != 0)
                {
                    bmi = (spinWeight.Value) / ((spinHeight.Value / 100) * (spinHeight.Value / 100));
                }
                double leatherArea = 0.007184 * Math.Pow((double)spinHeight.Value, 0.725) * Math.Pow((double)spinWeight.Value, 0.425);
                s.Text = Math.Round(bmi, 2) + "";
                lblLeatherArea.Text = Math.Round(leatherArea, 2) + "";
                if (bmi < 16)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.III", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                }
                else if (16 <= bmi && bmi < 17)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.II", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                }
                else if (17 <= bmi && bmi < (decimal)18.5)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.I", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                }
                else if ((decimal)18.5 <= bmi && bmi < 25)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.NORMAL", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                }
                else if (25 <= bmi && bmi < 30)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OVERWEIGHT", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                }
                else if (30 <= bmi && bmi < 35)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.I", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                }
                else if (35 <= bmi && bmi < 40)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.II", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                }
                else if (40 < bmi)
                {
                    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.III", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinWeight_EditValueChanged(object sender, EventArgs e)
        {
            fillDataToBmiAndLeatherArea();
        }

        private void spinBelly_Leave(object sender, EventArgs e)
        {
           
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExecuteTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinPulse.Focus();
                    spinPulse.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spSPO2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTemperature.Focus();
                    spinTemperature.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNote_Leave(object sender, EventArgs e)
        {
            try
            {

                if (dhstInit != null && dhstInit.delegateOutFocus != null)
                {
                    dhstInit.delegateOutFocus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSetDHST_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_DHST data = HIS.Desktop.Plugins.Library.ConnectBloodPressure.ConnectBloodPressureProcessor.GetData();
                if (data != null)
                {
                    if (data.EXECUTE_TIME != null)
                        dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtExecuteTime.EditValue = DateTime.Now;
                    if (data.BLOOD_PRESSURE_MAX.HasValue)
                    {
                        spinBloodPressureMax.EditValue = data.BLOOD_PRESSURE_MAX;
                    }
                    if (data.BLOOD_PRESSURE_MIN.HasValue)
                    {
                        spinBloodPressureMin.EditValue = data.BLOOD_PRESSURE_MIN;
                    }
                    if (data.BREATH_RATE.HasValue)
                    {
                        spinBreathRate.EditValue = data.BREATH_RATE;
                    }
                    if (data.HEIGHT.HasValue)
                    {
                        spinHeight.EditValue = data.HEIGHT;
                    }
                    if (data.CHEST.HasValue)
                    {
                        spinChest.EditValue = data.CHEST;
                    }
                    if (data.BELLY.HasValue)
                    {
                        spinBelly.EditValue = data.BELLY;
                    }
                    if (data.PULSE.HasValue)
                    {
                        spinPulse.EditValue = data.PULSE;
                    }
                    if (data.TEMPERATURE.HasValue)
                    {
                        spinTemperature.EditValue = data.TEMPERATURE;
                    }
                    if (data.WEIGHT.HasValue)
                    {
                        spinWeight.EditValue = data.WEIGHT;
                    }
                    if (!String.IsNullOrWhiteSpace(data.NOTE))
                    {
                        txtNote.Text = data.NOTE;
                    }
                    if (data.SPO2.HasValue)
                        spinSPO2.Value = (data.SPO2.Value * 100);
                    else
                        spinSPO2.EditValue = null;

                    if (data.URINE.HasValue)
                    {
                        spinUrine.EditValue = data.URINE;
                    }

                    if (data.CAPILLARY_BLOOD_GLUCOSE.HasValue)
                    {
                        spinCapillaryBloodGlucose.EditValue = data.CAPILLARY_BLOOD_GLUCOSE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCDHST
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__UCDHST = new ResourceManager("HIS.UC.DHST.Resources.Lang", typeof(UCDHST).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.btnSetDHST.ToolTip = Inventec.Common.Resource.Get.Value("UCDHST.btnSetDHST.ToolTip", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.labelControl12.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl12.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.labelControl10.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl10.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.labelControl9.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl9.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.labelControl8.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl8.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.labelControl7.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl7.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.labelControl6.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl6.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.labelControl5.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl5.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.labelControl4.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl4.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.labelControl3.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl3.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lciMach.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciMach.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lciNhietDo.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciNhietDo.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lciHuyetAp.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciHuyetAp.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lciNhipTho.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciNhipTho.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lciCanNang.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciCanNang.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lciVongBung.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciVongBung.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lciChieuCao.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciChieuCao.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lciVongNguc.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciVongNguc.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lblSPO2.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCDHST.lblSPO2.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lblSPO2.Text = Inventec.Common.Resource.Get.Value("UCDHST.lblSPO2.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lciNote.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciNote.Text", Resources.ResourceLanguageManager.LanguageResource__UCDHST, LanguageManager.GetCulture());
                this.lciUrine.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciUrine.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciCapillaryBloodGlucose.Text = Inventec.Common.Resource.Get.Value("UCDHST.lciCapillaryBloodGlucose.Text", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.lciCapillaryBloodGlucose.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCDHST.lciCapillaryBloodGlucose.OptionsToolTip.ToolTip", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
                this.spinCapillaryBloodGlucose.ToolTip = Inventec.Common.Resource.Get.Value("UCDHST.spinCapillaryBloodGlucose.ToolTip", ResourceLangManager.LanguageUCDHST, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinUrine_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinCapillaryBloodGlucose.Focus();
                    spinCapillaryBloodGlucose.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinCapillaryBloodGlucose_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
