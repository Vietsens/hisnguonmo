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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using System.Linq;
using His.UC.UCHein.Base;

namespace His.UC.UCHein.Design
{
    public partial class frmPatientChoice : Form
    {
        FillDataPatientSDOToRegisterForm updatePatientInfo;
        List<HisPatientSDO> currentListPatient;
        List<MOS.EFMODEL.DataModels.HIS_GENDER> Genders;
        Dictionary<long, string> dicGender;

        public frmPatientChoice(List<HisPatientSDO> currentListPatient, FillDataPatientSDOToRegisterForm updatePatientInfo, List<MOS.EFMODEL.DataModels.HIS_GENDER> genders)
        {
            this.currentListPatient = currentListPatient;
            this.updatePatientInfo = updatePatientInfo;
            this.Genders = genders;
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void PopupPatientInformation_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetCaptionByLanguageKeyNew();
                if (this.Genders != null)
                {
                    foreach (var item in this.Genders)
                    {
                        dicGender.Add(item.ID, item.GENDER_NAME);
                    }
                }
                this.grdInformation.DataSource = this.currentListPatient;
                this.gridView1.FocusedRowHandle = 0;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmPatientChoice = new ResourceManager("His.UC.UCHein.Design.Resources.Lang", typeof(His.UC.UCHein.Design.frmPatientChoice).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Base.LanguageManager.GetCulture());
                this.lblDescription.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.lblDescription.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Base.LanguageManager.GetCulture());
                this.btnClose.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.btnClose.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Base.LanguageManager.GetCulture());
                this.grdChoose.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdChoose.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Base.LanguageManager.GetCulture());
                this.grdCode.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdCode.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Base.LanguageManager.GetCulture());
                this.grdName.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdName.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Base.LanguageManager.GetCulture());
                this.grdDate.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdDate.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Base.LanguageManager.GetCulture());
                this.grdGender.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdGender.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Base.LanguageManager.GetCulture());
                this.grdAddress.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdAddress.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Base.LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Base.LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Base.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmPatientChoice
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmPatientChoice = new ResourceManager("His.UC.UCHein.Resources.Lang", typeof(frmPatientChoice).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, LanguageManager.GetCulture());
                this.lblDescription.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.lblDescription.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, LanguageManager.GetCulture());
                this.btnClose.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.btnClose.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, LanguageManager.GetCulture());
                this.grdChoose.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdChoose.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, LanguageManager.GetCulture());
                this.grdCode.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdCode.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, LanguageManager.GetCulture());
                this.grdName.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdName.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, LanguageManager.GetCulture());
                this.grdDate.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdDate.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, LanguageManager.GetCulture());
                this.grdGender.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdGender.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, LanguageManager.GetCulture());
                this.grdAddress.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdAddress.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    HisPatientSDO dataRow = (HisPatientSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.DOB);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "GENDER_NAME")
                    {
                        try
                        {
                            e.Value = dicGender[dataRow.GENDER_ID];
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot GENDER_NAME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectedPatientSdo(ref HisPatientSDO patient)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientWarningSDO patientWarningSDO = new BackendAdapter(param).Get<List<HisPatientWarningSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumerStore.MosConsumer, patient.ID, param).SingleOrDefault();
                if (patientWarningSDO == null) throw new ArgumentNullException("patientWarningSDO");

                patient.PreviousPrescriptions = patientWarningSDO.PreviousPrescriptions;
                patient.PreviousDebtTreatments = patientWarningSDO.PreviousDebtTreatments;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                var patient = (HisPatientSDO)this.gridView1.GetFocusedRow();
                if (patient != null)
                {
                    this.ProcessSelectedPatientSdo(ref patient);
                    this.updatePatientInfo(patient);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdInformation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var patient = (HisPatientSDO)this.gridView1.GetFocusedRow();
                    if (patient != null)
                    {
                        this.ProcessSelectedPatientSdo(ref patient);
                        this.updatePatientInfo(patient);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdInformation_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var patient = (HisPatientSDO)this.gridView1.GetFocusedRow();
                if (patient != null)
                {
                    this.ProcessSelectedPatientSdo(ref patient);
                    this.updatePatientInfo(patient);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmPatientChoice_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                dicGender = null;
                Genders = null;
                currentListPatient = null;
                updatePatientInfo = null;
                this.btnClose.Click -= new System.EventHandler(this.btnClose_Click);
                this.grdInformation.DoubleClick -= new System.EventHandler(this.grdInformation_DoubleClick);
                this.grdInformation.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.grdInformation_PreviewKeyDown);
                this.gridView1.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
                this.gridView1.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridView1_MouseDown);
                this.Load -= new System.EventHandler(this.PopupPatientInformation_Load);
                gridView1.GridControl.DataSource = null;
                grdInformation.DataSource = null;
                layoutControlItem3 = null;
                lblDescription = null;
                grdCode = null;
                repositoryItemCheckEdit1 = null;
                layoutControlItem2 = null;
                lciPatientInformation = null;
                radianChoose = null;
                grdAddress = null;
                grdGender = null;
                grdDate = null;
                grdName = null;
                grdChoose = null;
                gridView1 = null;
                grdInformation = null;
                btnClose = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
