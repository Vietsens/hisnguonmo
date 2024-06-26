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

namespace HIS.Desktop.Plugins.Register
{
    public partial class frmPatientChoice : HIS.Desktop.Utility.FormBase
    {
        UpdatePatientInfo updatePatientInfo;
        List<HisPatientSDO> currentListPatient;
        Dictionary<long, string> dicGender;

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
                SetCaptionByLanguageKey();
                SetIconFrm();
                dicGender = new Dictionary<long, string>();
                var genders = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>();
                if (genders != null)
                {
                    foreach (var item in genders)
                    {
                        dicGender.Add(item.ID, item.GENDER_NAME);
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

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmPatientChoice = new ResourceManager("HIS.Desktop.Plugins.Register.Resources.Lang", typeof(HIS.Desktop.Plugins.Register.frmPatientChoice).Assembly);

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
                HisPatientWarningSDO patientWarningSDO = new BackendAdapter(param).Get<HisPatientWarningSDO>(RequestUriStore.HIS_PATIENT_GETSPREVIOUSWARNING, ApiConsumers.MosConsumer, patient.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (patientWarningSDO == null) throw new ArgumentNullException("patientWarningSDO");

                patient.PreviousPrescriptions = patientWarningSDO.PreviousPrescriptions;
                patient.PreviousDebtTreatments = patientWarningSDO.PreviousDebtTreatments;
                patient.TodayFinishTreatments = patientWarningSDO.TodayFinishTreatments;
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

    }
}
