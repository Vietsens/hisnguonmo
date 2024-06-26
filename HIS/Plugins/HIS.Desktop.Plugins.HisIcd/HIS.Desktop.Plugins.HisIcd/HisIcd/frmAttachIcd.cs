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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisIcd
{
    public partial class frmAttachIcd : FormBase
    {
        List<IcdADO> icdAdoChecks;
        DelegateRefeshIcdChandoanphu delegateIcds;
        string icdCodes;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;

        public frmAttachIcd()
        {
            InitializeComponent();
        }

        public frmAttachIcd(DelegateRefeshIcdChandoanphu delegateIcds, string icdCodes, long _limit)
        {
            InitializeComponent();
            try
            {
                this.delegateIcds = delegateIcds;
                this.icdCodes = icdCodes;
                string[] codes = this.icdCodes.Split(IcdUtil.seperator.ToCharArray());
                icdAdoChecks = (from m in BackendDataWorker.Get<HIS_ICD>() select new IcdADO(m, codes)).ToList();
                limit = (int)_limit;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSecondaryDisease_Load(object sender, EventArgs e)
        {
            try
            {
                txtIcdCodes.Text = this.icdCodes;
                SetCaptionByLanguageKey();
                dataTotal = (icdAdoChecks.Count);
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                FillDataToGridIcd(new CommonParam(0, (ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : limit)));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridIcd, param, (ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : limit));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridIcd(object param)
        {
            try
            {
                gridControlSecondaryDisease.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                var query = icdAdoChecks.AsQueryable();
                string keyword = txtKeyword.Text.Trim();
                keyword = Inventec.Common.String.Convert.UnSignVNese(keyword.Trim().ToLower());
                if (!String.IsNullOrEmpty(keyword))
                {
                    query = query.Where(o =>
                        Inventec.Common.String.Convert.UnSignVNese((o.ICD_NAME ?? "").ToLower()).Contains(keyword)
                        || o.ICD_CODE.ToLower().Contains(keyword)
                        );
                }
                query = query.OrderByDescending(o => o.IsChecked).ThenBy(o => o.ICD_CODE);
                dataTotal = query.Count();
                var result = query.Skip(start).Take(limit).ToList();
                rowCount = (result == null ? 0 : result.Count);
                gridControlSecondaryDisease.DataSource = result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmSecondaryIcd
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtIcdCodes.Text))
                {
                    this.delegateIcds(txtIcdCodes.Text);
                }
                else
                {
                    this.delegateIcds("");
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlSecondaryDisease_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var disease = (IcdADO)gridViewSecondaryDisease.GetFocusedRow();
                    if (disease != null)
                    {
                        disease.IsChecked = !disease.IsChecked;
                        SetCheckedIcdsToControl();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlSecondaryDisease_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var disease = (IcdADO)gridViewSecondaryDisease.GetFocusedRow();
                if (disease != null)
                {
                    disease.IsChecked = !disease.IsChecked;
                    gridControlSecondaryDisease.RefreshDataSource();
                    SetCheckedIcdsToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSecondaryDisease_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("gridViewSecondaryDisease_KeyDown.1");
                int keyValue = e.KeyValue;
                if (!e.Shift && keyValue >= (int)Keys.A && keyValue <= (int)Keys.Z)
                {
                    txtKeyword.Text = e.KeyData.ToString();
                    txtKeyword.Focus();
                    txtKeyword.SelectionStart = txtKeyword.Text.Length;
                }
                Inventec.Common.Logging.LogSystem.Info("gridViewSecondaryDisease_KeyDown.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSecondaryDisease_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IsChecked")
                {
                    SetCheckedIcdsToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckedIcdsToControl()
        {
            try
            {
                string icdCodes = null;// = IcdUtil.seperator;
                var checkList = icdAdoChecks.Where(o => o.IsChecked == true).ToList();
                int count = 0;
                foreach (var item in checkList)
                {
                    count++;
                    if (count == checkList.Count)
                    {
                        icdCodes += item.ICD_CODE;
                    }
                    else
                    {
                        icdCodes += item.ICD_CODE + IcdUtil.seperator;
                    }
                }
                txtIcdCodes.Text = icdCodes;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridViewSecondaryDisease_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.FieldName == "IsChecked" && hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
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

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("txtKeyword_KeyUp.1");
                if (e.KeyCode == Keys.Down)
                {
                    Inventec.Common.Logging.LogSystem.Debug("txtKeyword_KeyUp.2");
                    gridViewSecondaryDisease.Focus();
                    gridViewSecondaryDisease.FocusedRowHandle = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnChoose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("bbtnChoose_ItemClick.1");
                if (gridViewSecondaryDisease.IsEditing)
                    gridViewSecondaryDisease.CloseEditor();

                if (gridViewSecondaryDisease.FocusedRowModified)
                    gridViewSecondaryDisease.UpdateCurrentRow();
                Inventec.Common.Logging.LogSystem.Debug("bbtnChoose_ItemClick.2");
                btnChoose_Click(null, null);
                Inventec.Common.Logging.LogSystem.Debug("bbtnChoose_ItemClick.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("txtKeyword_EditValueChanged.1");
                FillDataToGrid();
                Inventec.Common.Logging.LogSystem.Debug("txtKeyword_EditValueChanged.2");

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
                limit = 0;
                start = 0;
                dataTotal = 0;
                rowCount = 0;
                icdCodes = null;
                delegateIcds = null;
                icdAdoChecks = null;
                this.txtKeyword.EditValueChanged -= new System.EventHandler(this.txtKeyword_EditValueChanged);
                this.txtKeyword.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyUp);
                this.btnChoose.Click -= new System.EventHandler(this.btnChoose_Click);
                this.gridControlSecondaryDisease.DoubleClick -= new System.EventHandler(this.gridControlSecondaryDisease_DoubleClick);
                this.gridControlSecondaryDisease.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridControlSecondaryDisease_PreviewKeyDown);
                this.gridViewSecondaryDisease.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewSecondaryDisease_CellValueChanged);
                this.gridViewSecondaryDisease.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewSecondaryDisease_KeyDown);
                this.gridViewSecondaryDisease.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewSecondaryDisease_MouseDown);
                this.bbtnChoose.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnChoose_ItemClick);
                this.Load -= new System.EventHandler(this.frmSecondaryDisease_Load);
                gridViewSecondaryDisease.GridControl.DataSource = null;
                gridControlSecondaryDisease.DataSource = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                bbtnClose = null;
                bbtnChoose = null;
                bar2 = null;
                barManager1 = null;
                lciPaging = null;
                ucPaging1 = null;
                layoutControlItem3 = null;
                txtIcdCodes = null;
                layoutControlItem2 = null;
                txtKeyword = null;
                repositoryItemCheckEdit1 = null;
                gridColumn1 = null;
                layoutControlItem1 = null;
                lciGridControl = null;
                grdColName = null;
                grdColCode = null;
                gridViewSecondaryDisease = null;
                gridControlSecondaryDisease = null;
                btnChoose = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }

    public delegate void DelegateRefeshIcdChandoanphu(string icdCodes);
}
