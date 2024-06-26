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
using HIS.Desktop.ApiConsumer;
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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.RegisterV2
{
    public partial class frmPatientChoice : HIS.Desktop.Utility.FormBase
    {
        UpdatePatientInfo updatePatientInfo;
        List<HisPatientSDO> currentListPatient;
        Dictionary<long, string> dicGender;
        Dictionary<long, string> dicWorkPlace;

        public frmPatientChoice(List<HisPatientSDO> currentListPatient, UpdatePatientInfo updatePatientInfo)
        {
            this.currentListPatient = currentListPatient;
            this.updatePatientInfo = updatePatientInfo;
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PopupPatientInformation_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                //SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
                dicGender = new Dictionary<long, string>();
                dicWorkPlace = new Dictionary<long, string>();
                var genders = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>();
                if (genders != null)
                {
                    foreach (var item in genders)
                    {
                        dicGender.Add(item.ID, item.GENDER_NAME);
                    }
                }
                var wordPlaces = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>();
                if (wordPlaces != null)
                {
                    foreach (var item in wordPlaces)
                    {
                        dicWorkPlace.Add(item.ID, item.WORK_PLACE_NAME);
                    }
                }
                grdInformation.DataSource = currentListPatient;
                gridView1.FocusedRowHandle = 0;
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
                Resources.ResourceLanguageManager.LanguagefrmPatientChoice = new ResourceManager("HIS.Desktop.Plugins.RegisterV2.Resources.Lang", typeof(HIS.Desktop.Plugins.RegisterV2.frmPatientChoice).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblDescription.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.lblDescription.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnClose.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.btnClose.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grdChoose.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdChoose.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grdCode.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdCode.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grdName.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdName.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grdDate.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdDate.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grdGender.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdGender.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grdAddress.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdAddress.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.Text", Resources.ResourceLanguageManager.LanguagefrmPatientChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RegisterV2.Resources.Lang", typeof(frmPatientChoice).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblDescription.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.lblDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnClose.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.btnClose.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdChoose.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdChoose.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCode.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdName.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdDate.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdGender.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdGender.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdAddress.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.grdAddress.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmPatientChoice.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPatientChoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                            e.Value = dicGender.ContainsKey(dataRow.GENDER_ID) ? dicGender[dataRow.GENDER_ID] : "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot GENDER_NAME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_WORK_PLACE_NAME")
                    {
                        try
                        {
                            if (dataRow.WORK_PLACE_ID == null)
                            {
                                e.Value = dataRow.WORK_PLACE;
                            }
                            else if (dataRow.WORK_PLACE_ID.HasValue && dataRow.WORK_PLACE_ID.Value > 0)
                            {
                                e.Value = dicWorkPlace.ContainsKey(dataRow.WORK_PLACE_ID.Value) ? dicWorkPlace[dataRow.WORK_PLACE_ID.Value] : "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot TDL_PATIENT_WORK_PLACE_NAME", ex);
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
                HisPatientWarningSDO patientWarningSDO = new BackendAdapter(param).Get<List<HisPatientWarningSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, patient.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).SingleOrDefault();
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
                var patient = (HisPatientSDO)gridView1.GetFocusedRow();
                if (patient != null)
                {
                    ProcessSelectedPatientSdo(ref patient);
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
                    var patient = (HisPatientSDO)gridView1.GetFocusedRow();
                    if (patient != null)
                    {
                        ProcessSelectedPatientSdo(ref patient);
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
                var patient = (HisPatientSDO)gridView1.GetFocusedRow();
                if (patient != null)
                {
                    ProcessSelectedPatientSdo(ref patient);
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
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                dicWorkPlace = null;
                dicGender = null;
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
                gridColumn3 = null;
                gridColumn2 = null;
                gridColumn1 = null;
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
