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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.ConnectionTest.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using LIS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class frmUpdateCondition : FormBase
    {
        private int positionHandle = -1;

        private V_LIS_SAMPLE sample;
        List<LIS_SAMPLE_CONDITION> sampleConditions = new List<LIS_SAMPLE_CONDITION>();
        public frmUpdateCondition(Inventec.Desktop.Common.Modules.Module module, V_LIS_SAMPLE data)
            : base(module)
        {
            InitializeComponent();
            this.sample = data;
        }

        private void frmUpdateCondition_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetCaptionByLanguageKey();
                this.ValidControl();
                this.LoadComboCondition();
                lblBarcode.Text = sample.BARCODE ?? "";
                lblPatientCode.Text = sample.PATIENT_CODE ?? "";
                lblPatientName.Text = (sample.LAST_NAME ?? "") + " " + (sample.FIRST_NAME ?? "");
                lblServiceReqCode.Text = sample.SERVICE_REQ_CODE ?? "";
                lblTreatmentCode.Text = sample.TREATMENT_CODE ?? "";
                cboSampleCondition.EditValue = sample.SAMPLE_CONDITION_ID;
                WaitingManager.Hide();
                this.KeyPreview = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboCondition()
        {
            try
            {

                LisSampleConditionFilter filter = new LisSampleConditionFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.sampleConditions = new BackendAdapter(new CommonParam()).Get<List<LIS_SAMPLE_CONDITION>>("api/LisSampleCondition/Get", ApiConsumers.LisConsumer, filter, null);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SAMPLE_CONDITION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SAMPLE_CONDITION_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SAMPLE_CONDITION_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboSampleCondition, this.sampleConditions, controlEditorADO);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ConditionValidationRule rule = new ConditionValidationRule();
                rule.txtSampleConditionCode = txtSampleConditionCode;
                rule.cboSampleCondition = cboSampleCondition;
                dxValidationProvider1.SetValidationRule(txtSampleConditionCode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
       
        private void txtSampleConditionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtSampleConditionCode.Text))
                    {
                        string txt = txtSampleConditionCode.Text.ToLower().Trim();
                        var listData = this.sampleConditions != null ? this.sampleConditions.Where(o => o.SAMPLE_CONDITION_CODE.ToLower().Contains(txt)).ToList() : null;
                        if (listData != null && listData.Count == 1)
                        {
                            cboSampleCondition.EditValue = listData[0].ID;
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
                        }
                        else
                        {
                            cboSampleCondition.Focus();
                            cboSampleCondition.ShowPopup();
                        }
                    }
                    else
                    {
                        cboSampleCondition.Focus();
                        cboSampleCondition.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleCondition_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
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

        private void cboSampleCondition_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtSampleConditionCode.Text = "";
                if (cboSampleCondition.EditValue != null)
                {
                    var con = this.sampleConditions.FirstOrDefault(o => o.ID == Convert.ToInt64(cboSampleCondition.EditValue));
                    if (con != null)
                    {
                        txtSampleConditionCode.Text = con.SAMPLE_CONDITION_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!btnSave.Enabled || this.sample == null || !dxValidationProvider1.Validate()) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                LisSampleConditionSDO sdo = new LisSampleConditionSDO();
                sdo.SampleId = this.sample.ID;
                sdo.SampleConditionId = Convert.ToInt64(cboSampleCondition.EditValue); ;

                LIS_SAMPLE rowBe = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Condition", ApiConsumers.LisConsumer, sdo, null);
                if (rowBe != null)
                {
                    success = true;
                    sample.SAMPLE_CONDITION_ID = rowBe.SAMPLE_CONDITION_ID;
                }
                WaitingManager.Hide();


                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (success)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmUpdateCondition
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition = new ResourceManager("HIS.Desktop.Plugins.ConnectionTest.Resources.Lang", typeof(frmUpdateCondition).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmUpdateCondition.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmUpdateCondition.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
                this.cboSampleCondition.Properties.NullText = Inventec.Common.Resource.Get.Value("frmUpdateCondition.cboSampleCondition.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
                this.lciServiceReqCode.Text = Inventec.Common.Resource.Get.Value("frmUpdateCondition.lciServiceReqCode.Text", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
                this.lciBarcode.Text = Inventec.Common.Resource.Get.Value("frmUpdateCondition.lciBarcode.Text", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
                this.lciTreatmentCode.Text = Inventec.Common.Resource.Get.Value("frmUpdateCondition.lciTreatmentCode.Text", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("frmUpdateCondition.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmUpdateCondition.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
                this.lciSampleCondition.Text = Inventec.Common.Resource.Get.Value("frmUpdateCondition.lciSampleCondition.Text", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmUpdateCondition.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
                this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("frmUpdateCondition.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmUpdateCondition.Text", Resources.ResourceLanguageManager.LanguageResource__frmUpdateCondition, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

