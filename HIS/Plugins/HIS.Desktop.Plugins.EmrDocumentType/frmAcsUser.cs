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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
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
using SAR.Desktop.Plugins.SarPrintType;
using HIS.Desktop.Plugins.EmrDocumentType.ADO;
using HIS.Desktop.Plugins.EmrDocumentType;

namespace HIS.Desktop.Plugins.EmrDocumentType
{
    public partial class frmAcsUser : HIS.Desktop.Utility.FormBase
    {
        List<AcsUserADO> userAdoCheck;
        DelegateRefreshAcsUser delegateUsers;
        string loginNames;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;

        public frmAcsUser()
        {
            InitializeComponent();
        }
        //public void funData(TextEdit txtFormUser)
        //{
        //    txtLoginName.Text = txtFormUser.Text.Trim();
        //}

        public frmAcsUser(DelegateRefreshAcsUser _delegateUsers, string _loginNames)
        {
            InitializeComponent();
            this.delegateUsers = _delegateUsers;
            this.loginNames = _loginNames;



        }

        private void frmSecondaryDisease_Load(object sender, EventArgs e)
        {
            try
            {
                txtLoginNames.Text = this.loginNames;
                //ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AcsUser.Resources.Lang", typeof(HIS.Desktop.Plugins.AcsUser.frmAcsUser).Assembly);
                //Language_secondaryDisease();
                string[] tks = this.loginNames.Split(AcsUserUtil.seperator.ToCharArray());
                var users = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(p => p.IS_ACTIVE == 1).ToList();
                userAdoCheck = (from m in users select new AcsUserADO(m, tks)).ToList();
                dataTotal = (userAdoCheck.Count);
                limit = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                FillDataToGrid();
                gridViewSecondaryDisease.ClearSelection();
                
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
                FillDataToGridUser(new CommonParam(0, (ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : limit)));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridUser, param, (ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : limit));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridUser(object param)
        {
            try
            {
                gridControlSecondaryDisease.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                var query = userAdoCheck.AsQueryable();
                string keyword = txtKeyword.Text.Trim();
                keyword = Inventec.Common.String.Convert.UnSignVNese(keyword.Trim().ToLower());
                if (!String.IsNullOrEmpty(keyword))
                {
                    query = query.Where(o =>
                        Inventec.Common.String.Convert.UnSignVNese((o.LOGINNAME ?? "").ToLower()).Contains(keyword)
                        || Inventec.Common.String.Convert.UnSignVNese((o.USERNAME ?? "").ToLower()).Contains(keyword)
                        );
                }
                query = query.OrderByDescending(o => o.IsChecked).ThenBy(o => o.LOGINNAME);
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

        //private void Language_secondaryDisease()
        //{
        //    try
        //    {
        //        grdColCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_GRDCOL_CODE", ResourceLanguageManager.LanguagefrmSecondaryDisease, LanguageManager.GetCulture());
        //        grdColName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_GRDCOL_NAME", ResourceLanguageManager.LanguagefrmSecondaryDisease, LanguageManager.GetCulture());
        //        txtUserNames.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_ICD_CODE__NULL_TEXT", ResourceLanguageManager.LanguagefrmSecondaryDisease, LanguageManager.GetCulture());
        //        txtLoginNames.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_ICD_NAME__NULL_TEXT", ResourceLanguageManager.LanguagefrmSecondaryDisease, LanguageManager.GetCulture());
        //        btnChoose.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_BTN_CHOOSE", ResourceLanguageManager.LanguagefrmSecondaryDisease, LanguageManager.GetCulture());
        //        txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_GRID_ICD__FIND_NULL_PROMPT_TEXT", ResourceLanguageManager.LanguagefrmSecondaryDisease, LanguageManager.GetCulture());
        //        gridViewSecondaryDisease.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_GRID_ICD__FIND_NULL_PROMPT_TEXT", ResourceLanguageManager.LanguagefrmSecondaryDisease, LanguageManager.GetCulture());
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtLoginNames.Text))
                {
                    txtLoginNames.Text = AddSeperateToResult(txtLoginNames.Text);
                    if (this.delegateUsers != null)
                        this.delegateUsers(txtLoginNames.Text);

                    this.Close();
                }
                else
                {
                    this.delegateUsers(null);
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
                    if (!data.EndsWith(AcsUserUtil.seperator))
                        data = data + AcsUserUtil.seperator;

                    if (!data.StartsWith(AcsUserUtil.seperator))
                        data = AcsUserUtil.seperator + data;

                    if (data.StartsWith(AcsUserUtil.seperator))
                    {
                        data = data.Remove(0, 1);
                    }
                    if (data.EndsWith(AcsUserUtil.seperator))
                    {
                        data = data.Remove(data.Count() - 1, 1);
                    }
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
                    var disease = (AcsUserADO)gridViewSecondaryDisease.GetFocusedRow();
                    if (disease != null)
                    {
                        disease.IsChecked = !disease.IsChecked;
                        SetCheckedLoginNamesToControl();
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
                var disease = (AcsUserADO)gridViewSecondaryDisease.GetFocusedRow();
                if (disease != null)
                {
                    disease.IsChecked = !disease.IsChecked;
                    gridControlSecondaryDisease.RefreshDataSource();
                    SetCheckedLoginNamesToControl();
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


        private void gridViewSecondaryDisease_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IsChecked")
                {
                    SetCheckedLoginNamesToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetCheckedLoginNamesToControl()
        {
            try
            {
                string loginNames = AcsUserUtil.seperator;
                var checkList = userAdoCheck.Where(o => o.IsChecked == true).ToList();
                foreach (var item in checkList)
                {
                    loginNames += item.LOGINNAME + AcsUserUtil.seperator;
                }
                txtLoginNames.Text = loginNames;

                if (loginNames.Equals(AcsUserUtil.seperator))
                {
                    txtLoginNames.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        string processLoginNameChanged(string olduserNames, string newuserNames)
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
                result = newuserNames;

                if (!String.IsNullOrEmpty(olduserNames))
                {
                    var arrNames = olduserNames.Split(new string[] { AcsUserUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newuserNames.Contains(AcsUserUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = userAdoCheck.Where(o => o.IsChecked == false &&
                                    AcsUserUtil.AddSeperateToKey(item).Equals(AcsUserUtil.AddSeperateToKey(o.LOGINNAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + AcsUserUtil.seperator;
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
                else if (e.KeyCode == Keys.Enter)
                {
                    //WaitingManager.Show();
                    FillDataToGrid();
                    //gridViewSecondaryDisease.BeginUpdate();
                    //var icdSearch = userAdoCheck.Where(o =>
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
    }
}
