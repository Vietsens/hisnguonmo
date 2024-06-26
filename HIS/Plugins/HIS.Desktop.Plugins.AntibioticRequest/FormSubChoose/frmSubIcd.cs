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

namespace HIS.Desktop.Plugins.AntibioticRequest
{
    public partial class frmSubIcd : Form
    {
        List<IcdADO> icdAdoChecks;
        DelegateRefeshIcdChandoanphu delegateIcds;
        string icdCodes;
        string icdNames;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;

        public frmSubIcd(DelegateRefeshIcdChandoanphu delegateIcds, string icdCodes, string icdNames, int _limit, List<HIS_ICD> listIcd)
        {
            InitializeComponent();
            try
            {
                this.delegateIcds = delegateIcds;
                this.icdCodes = icdCodes;
                this.icdNames = icdNames;
                string[] codes = this.icdCodes.Split(IcdUtil.seperator.ToCharArray());
                icdAdoChecks = (from m in BackendDataWorker.Get<HIS_ICD>() select new IcdADO(m, codes)).ToList();
                limit = _limit;
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
                txtIcdNames.Text = this.icdNames;
                Language_secondaryDisease();
                //CreateThreadLoadIcd(SecondaryIcdProcessor.HisIcds);
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
                gridControlSecondaryDisease.BeginUpdate();
                gridControlSecondaryDisease.DataSource = result;
                gridControlSecondaryDisease.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Language_secondaryDisease()
        {
            try
            {
                grdColCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_GRDCOL_CODE", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                grdColName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_GRDCOL_NAME", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                txtIcdNames.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_ICD_CODE__NULL_TEXT", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                txtIcdCodes.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_ICD_NAME__NULL_TEXT", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                btnChoose.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_BTN_CHOOSE", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_GRID_ICD__FIND_NULL_PROMPT_TEXT", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
                gridViewSecondaryDisease.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_GRID_ICD__FIND_NULL_PROMPT_TEXT", Resources.ResourceMessage.LanguagefrmSecondaryIcd, LanguageManager.GetCulture());
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
                    txtIcdNames.Text = AddSeperateToResult(txtIcdNames.Text);
                    txtIcdCodes.Text = AddSeperateToResult(txtIcdCodes.Text);

                    if (this.delegateIcds != null)
                        this.delegateIcds(txtIcdCodes.Text, txtIcdNames.Text);
                    this.Close();
                }
                this.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string AddSeperateToResult(string data)
        {
            string result = "";
            try
            {
                data = data.Trim();
                if (!String.IsNullOrEmpty(data))
                {
                    if (!data.EndsWith(IcdUtil.seperator))
                        data = data + IcdUtil.seperator;

                    if (!data.StartsWith(IcdUtil.seperator))
                        data = IcdUtil.seperator + data;
                }
                result = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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
                    //if (this.delegateDisease != null)
                    //    this.delegateDisease(disease);
                    //this.Close();
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
                int keyValue = e.KeyValue;
                if (!e.Shift && keyValue >= (int)Keys.A && keyValue <= (int)Keys.Z)
                {
                    txtKeyword.Text = e.KeyData.ToString();
                    txtKeyword.Focus();
                    txtKeyword.SelectionStart = txtKeyword.Text.Length;
                }
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
                string icdNames = IcdUtil.seperator;
                string icdCodes = IcdUtil.seperator;
                string icdName__Olds = txtIcdNames.Text;
                var checkList = icdAdoChecks.Where(o => o.IsChecked == true).ToList();
                foreach (var item in checkList)
                {
                    icdCodes += item.ICD_CODE + IcdUtil.seperator;
                    icdNames += item.ICD_NAME + IcdUtil.seperator;
                }

                txtIcdNames.Text = processIcdNameChanged(icdName__Olds, icdNames);
                txtIcdCodes.Text = icdCodes;
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdNames.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdCodes.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string processIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = icdAdoChecks.Where(o => o.IsChecked == false &&
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    gridViewSecondaryDisease.Focus();
                    gridViewSecondaryDisease.FocusedRowHandle = 0;
                }
                else //if (e.KeyCode == Keys.Enter)
                {
                    //WaitingManager.Show();
                    //FillDataToGrid();
                    //gridViewSecondaryDisease.BeginUpdate();
                    //var icdSearch = icdAdoChecks.Where(o =>
                    //    o.ICD_CODE.ToLower().Contains(txtKeyword.Text.Trim().ToLower())
                    //    || o.ICD_NAME.ToLower().Contains(txtKeyword.Text.Trim().ToLower())
                    //    ).ToList();
                    //if (icdSearch == null)
                    //{
                    //    icdSearch = new List<IcdADO>();
                    //}
                    //gridViewSecondaryDisease.GridControl.DataSource = icdSearch.OrderBy(o => o.ICD_CODE).ToList();
                    //gridViewSecondaryDisease.EndUpdate();
                    //WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnChoose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gridViewSecondaryDisease.IsEditing)
                    gridViewSecondaryDisease.CloseEditor();

                if (gridViewSecondaryDisease.FocusedRowModified)
                    gridViewSecondaryDisease.UpdateCurrentRow();

                btnChoose_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            try
            {
                if (keyData == Keys.Escape)
                {
                    this.Close();
                    return true;
                }

                return base.ProcessDialogKey(keyData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
