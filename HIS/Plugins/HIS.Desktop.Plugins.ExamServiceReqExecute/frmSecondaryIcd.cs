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
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Plugins.Library.CheckIcd;
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

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class frmSecondaryIcd : FormBase
    {
        List<IcdADO> icdAdoChecks;
        DelegateRefeshIcdChandoanphu delegateIcds;
        string icdCodes;
        string icdNames;
        string icdMainCode;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        CheckIcdManager checkIcdManager;
        public frmSecondaryIcd()
        {
            InitializeComponent();
        }
        public frmSecondaryIcd(DelegateRefeshIcdChandoanphu delegateIcds, string icdCodes, string icdNames, long _limit, CheckIcdManager checkIcdManager,string icdMainCode)
        {
            InitializeComponent();
            try
            {
                this.delegateIcds = delegateIcds;
                this.icdCodes = icdCodes;
                this.icdNames = icdNames;
                this.icdMainCode = icdMainCode;
                string[] codes = this.icdCodes.Split(IcdUtil.seperator.ToCharArray());
                icdAdoChecks = (from m in BackendDataWorker.Get<V_HIS_ICD>() where m.IS_ACTIVE == 1 && m.IS_TRADITIONAL != 1 select new IcdADO(m, codes)).ToList();
                limit = (int)_limit;
                this.checkIcdManager = checkIcdManager;
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
                SetCaptionByLanguageKey();
                //Language_secondaryDisease();
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

                if (ResourceLangManager.LanguageUCExamServiceReqExecute == null)
                {
                    ResourceLangManager.InitResourceLanguageManager();
                }
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.layoutControl1.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.txtIcdCodes.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.txtIcdCodes.Properties.NullValuePrompt", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.txtKeyword.Properties.NullValuePrompt", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.txtIcdNames.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.txtIcdNames.Properties.NullValuePrompt", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.btnChoose.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridViewSecondaryDisease.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.gridViewSecondaryDisease.OptionsFind.FindNullPrompt", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.gridColumn1.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.grdColCode.Caption = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.grdColCode.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.grdColName.Caption = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.grdColName.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblIcdText.Text = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.lblIcdText.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.bar2.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.bbtnChoose.Caption = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.bbtnChoose.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.bbtnClose.Caption = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.bbtnClose.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSecondaryIcd.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void Language_secondaryDisease()
        {
            try
            {
                if (ResourceLangManager.LanguageUCExamServiceReqExecute == null)
                {
                    ResourceLangManager.InitResourceLanguageManager();
                }
                grdColCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_GRDCOL_CODE", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                grdColName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_GRDCOL_NAME", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                txtIcdNames.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_ICD_CODE__NULL_TEXT", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                txtIcdCodes.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_ICD_NAME__NULL_TEXT", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                btnChoose.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_DISEASE_BTN_CHOOSE", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_GRID_ICD__FIND_NULL_PROMPT_TEXT", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                gridViewSecondaryDisease.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_SECONDARY_GRID_ICD__FIND_NULL_PROMPT_TEXT", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
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
                Inventec.Common.Logging.LogSystem.Info("btnChoose_Click.1");
                if (!string.IsNullOrEmpty(txtIcdCodes.Text.Trim()))
                {
                    txtIcdNames.Text = AddSeperateToResult(txtIcdNames.Text.Trim());
                    txtIcdCodes.Text = AddSeperateToResult(txtIcdCodes.Text.Trim());
                    Inventec.Common.Logging.LogSystem.Info("btnChoose_Click.2");
                    if (this.delegateIcds != null)
                        this.delegateIcds(txtIcdCodes.Text, txtIcdNames.Text.Trim());
                }
                else
                {
                    if (this.delegateIcds != null)
                        this.delegateIcds("", "");
                }
                Inventec.Common.Logging.LogSystem.Info("btnChoose_Click.3");
                this.Close();
                Inventec.Common.Logging.LogSystem.Info("btnChoose_Click.4");
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
                    //if (!data.EndsWith(IcdUtil.seperator))
                    //    data = data + IcdUtil.seperator;

                    //if (!data.StartsWith(IcdUtil.seperator))
                    //    data = IcdUtil.seperator + data;
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
                    int rowHandle = e.RowHandle;
                    var row = gridViewSecondaryDisease.GetRow(rowHandle) as IcdADO;
                    string icdCode = row.ICD_CODE;
                    SetCheckedIcdsToControl(icdCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckedIcdsToControl(string dataChoose = null)
        {
            try
            {
                string icdNames = null;// = IcdUtil.seperator;
                string icdCodes = null;// = IcdUtil.seperator;
                string icdName__Olds = txtIcdNames.Text.Trim();
                var checkList = icdAdoChecks.Where(o => o.IsChecked == true).ToList();
                var data = checkList.Select(o => o.ICD_CODE);
                string result = string.Join(";", data);

                int count = 0;
                foreach (var item in checkList)
                {
                    count++;
                    bool next = true;
                    string messErr = null;
                    if (!checkIcdManager.ProcessCheckIcd(null, result, ref messErr, false))
                    {
                        if (count == checkList.Count)
                        {
                            XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                        }
                        if (!string.IsNullOrEmpty(dataChoose))
                        {
                            if (item.ICD_CODE == dataChoose)
                            {
                                //item.IsChecked = false;
                                next = false;
                            }
                        }
                        //continue;
                    }
                    //break;
                    if (next == true)
                    {
                        if (count == checkList.Count)
                        {
                            icdCodes += item.ICD_CODE;
                            icdNames += item.ICD_NAME;
                        }
                        else
                        {
                            icdCodes += item.ICD_CODE + IcdUtil.seperator;
                            icdNames += item.ICD_NAME + IcdUtil.seperator;
                        }
                    }
                }

                txtIcdNames.Text = icdNames;
                txtIcdCodes.Text = icdCodes;
                //if (icdNames.Equals(IcdUtil.seperator))
                //{
                //    txtIcdNames.Text = "";
                //}
                //if (icdCodes.Equals(IcdUtil.seperator))
                //{
                //    txtIcdCodes.Text = "";
                //}

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
                icdNames = null;
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
                ucPaging1 = null;
                emptySpaceItem1 = null;
                layoutControlItem3 = null;
                txtIcdCodes = null;
                layoutControlItem2 = null;
                txtKeyword = null;
                repositoryItemCheckEdit1 = null;
                gridColumn1 = null;
                lblIcdText = null;
                txtIcdNames = null;
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

    public delegate void DelegateRefeshIcdChandoanphu(string icdCodes, string icdNames);
}
