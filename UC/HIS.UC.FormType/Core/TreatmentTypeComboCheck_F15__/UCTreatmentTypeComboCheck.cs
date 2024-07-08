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
using HIS.UC.FormType.Loader;
using HIS.UC.FormType.Base;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using His.UC.LibraryMessage;

namespace HIS.UC.FormType.TreatmentTypeComboCheck
{
    public partial class UCTreatmentTypeComboCheck : DevExpress.XtraEditors.XtraUserControl
    {
        private List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> selectedTreatmentTypes = new List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCTreatmentTypeComboCheck(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 25;
                this.config = config;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
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
                TreatmentTypeLoader.LoadDataToCombo(cboTreatmentType);
                SetTitle();
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
               
                InitGridCheckMarksSelection();
                
                if (this.isValidData)
                {
                    Validation();
                    lciTitleName.AppearanceItemCaption.ForeColor = Color.Maroon;
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
                    lciTitleName.Text = this.config.DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void InitGridCheckMarksSelection()
        {
            try
            {
                GridCheckMarksSelection gridCheckMarksSA = new GridCheckMarksSelection(cboTreatmentType.Properties);
                gridCheckMarksSA.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(gridCheckMarks_SelectionChanged);
                cboTreatmentType.Properties.Tag = gridCheckMarksSA;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void gridCheckMarks_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (ActiveControl is GridLookUpEdit)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
                    }
                    (ActiveControl as GridLookUpEdit).Text = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void cboTreatmentType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    if (true)
                    {

                    }
                    sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void cboTreatmentType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    System.Windows.Forms.SendKeys.Send("{TAB}");
                    //if (this.foucusMoveOutServiceTreatmentTypeInfo != null)
                    //    this.foucusMoveOutServiceTreatmentTypeInfo(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Get value in form return in object
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            string value = "";
            List<long> TREATMENT_TYPE_IDs = new List<long>();
            try
            {
                GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    TREATMENT_TYPE_IDs = new List<long>();
                    foreach (MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                    {
                        TREATMENT_TYPE_IDs.Add(rv.ID);
                    }
                }

                value = String.Format(this.config.JSON_OUTPUT, Newtonsoft.Json.JsonConvert.SerializeObject(TREATMENT_TYPE_IDs));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }
        public void SetValue(object sender, EventArgs e)
        {
            try
            {
                if (this.config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                {
                   
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,this.config.JSON_OUTPUT, this.report.JSON_FILTER);
                    List<long> Ids = null;
                    if (value != null && value != "null")
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
                            var listView = Config.HisFormTypeConfig.HisTreatmentTypes;
                            cboTreatmentType.Properties.DataSource = listView;
                            var selectFilter = listView.Where(o => Ids.Contains(o.ID)).ToList();
                            GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                            gridCheckMark.Selection.AddRange(selectFilter);
                            cboTreatmentType.Text = "";
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
                if (this.isValidData)
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
        private void ValidateTreatmentType()
        {
            try
            {
                HIS.UC.FormType.TreatmentTypeComboCheck.Validation.TreatmentTypeValidationRule validRule = new HIS.UC.FormType.TreatmentTypeComboCheck.Validation.TreatmentTypeValidationRule();
                validRule.cboTreatmentType = cboTreatmentType;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboTreatmentType, validRule);
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
                ValidateTreatmentType();
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

        private void UCTreatmentTypeComboCheck_Load(object sender, EventArgs e)
        {
            try
            {
                lciTitleName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_TREATMENT_TYPE_COMBO_CHECK_LCI_TITLE_NAME", Resources.ResourceLanguageManager.LanguageUCTreatmentTypeComboCheck, Base.LanguageManager.GetCulture());
                if (this.report != null)
                {
                    SetValue(sender,e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
