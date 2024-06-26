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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ContactDeclaration.Properties;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ContactDeclaration.Choice
{
    public partial class frmOtherChoice : HIS.Desktop.Utility.FormBase
    {
        UpdateContactPoint updateContactPoint;

        List<HIS_CONTACT_POINT> lstHisContactPoint = new List<HIS_CONTACT_POINT>();
        Dictionary<long, string> dicGender;
        //V_HIS_CONTACT_POINT CurrentContactPoint = new V_HIS_CONTACT_POINT();


        public frmOtherChoice(List<HIS_CONTACT_POINT> _lstHisContactPoint, UpdateContactPoint _updateContactPoint)
        {
            InitializeComponent();
            try
            {
                this.lstHisContactPoint = _lstHisContactPoint;
                this.updateContactPoint = _updateContactPoint;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmOtherChoice_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();

                dicGender = new Dictionary<long, string>();
                var genders = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>();
                if (genders != null)
                {
                    foreach (var item in genders)
                    {
                        dicGender.Add(item.ID, item.GENDER_NAME);
                    }
                }

                gridControl1.BeginUpdate();
                gridControl1.DataSource = lstHisContactPoint;
                gridControl1.EndUpdate();


                WaitingManager.Hide();
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
                Resources.ResourceLanguageManager.LanguagefrmOtherChoice = new ResourceManager("HIS.Desktop.Plugins.ContactDeclaration.Resources.Lang", typeof(HIS.Desktop.Plugins.ContactDeclaration.Choice.frmOtherChoice).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmOtherChoice.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmOtherChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lblDescription.Text = Inventec.Common.Resource.Get.Value("frmOtherChoice.lblDescription.Text", Resources.ResourceLanguageManager.LanguagefrmOtherChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.btnClose.Text = Inventec.Common.Resource.Get.Value("frmOtherChoice.btnClose.Text", Resources.ResourceLanguageManager.LanguagefrmOtherChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmOtherChoice.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguagefrmOtherChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmOtherChoice.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguagefrmOtherChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmOtherChoice.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguagefrmOtherChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmOtherChoice.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguagefrmOtherChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmOtherChoice.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguagefrmOtherChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmOtherChoice.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguagefrmOtherChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmOtherChoice.Text", Resources.ResourceLanguageManager.LanguagefrmOtherChoice, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    HIS_CONTACT_POINT dataRow = (HIS_CONTACT_POINT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.DOB ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DOB", ex);
                        }
                    }
                    else if (e.Column.FieldName == "GENDER_NAME")
                    {
                        try
                        {
                            e.Value = dicGender[dataRow.GENDER_ID ?? 0];
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

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (HIS_CONTACT_POINT)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    this.updateContactPoint(rowData);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControl1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (HIS_CONTACT_POINT)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        this.updateContactPoint(rowData);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
