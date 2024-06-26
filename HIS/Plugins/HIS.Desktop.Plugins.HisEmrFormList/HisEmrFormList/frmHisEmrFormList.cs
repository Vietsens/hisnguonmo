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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.HisEmrFormList.HisEmrFormList
{
    public partial class frmHisEmrFormList : HIS.Desktop.Utility.FormBase
    {
        List<HIS_EMR_FORM> _lstHisEmrForm = new List<HIS_EMR_FORM>();
        Inventec.Desktop.Common.Modules.Module moduleData;

        public frmHisEmrFormList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmHisEmrFormList_Load(object sender, EventArgs e)
        {
            try
            {
                 SetDefaultFocus();
                 FillDataToControl();
                 //set ngon ngu
                 SetCaptionByLanguagekey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void SetCaptionByLanguagekey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisEmrFormList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisEmrFormList.HisEmrFormList.frmHisEmrFormList).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChoiceAll.Text = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.btnChoiceAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUnchoiceAll.Text = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.btnUnchoiceAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisEmrFormList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisEmrFormFilter filter = new HisEmrFormFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                _lstHisEmrForm = new BackendAdapter(param).Get<List<HIS_EMR_FORM>>("/api/HisEmrForm/Get", ApiConsumers.MosConsumer, filter, param);

                gridView1.BeginUpdate();
                gridView1.GridControl.DataSource = _lstHisEmrForm;
                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisEmrFormFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_EMR_FORM data = (HIS_EMR_FORM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Value = data.IS_ACTIVE == 1 ? true : false;
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.MODIFY_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChoiceAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnChoiceAll.Enabled) return;
                if (this._lstHisEmrForm != null)
                {
                    this._lstHisEmrForm.ForEach(o => o.IS_ACTIVE = 1);
                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnchoiceAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnUnchoiceAll.Enabled) return;
                if (this._lstHisEmrForm != null)
                {
                    this._lstHisEmrForm.ForEach(o => o.IS_ACTIVE = 0);
                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                List<HIS_EMR_FORM> lstHisEmrForms = this._lstHisEmrForm.Where(o =>
                    o.EMR_FORM_CODE.ToUpper().Contains(this.txtSearch.Text.ToUpper())
                    || o.EMR_FORM_NAME.ToUpper().Contains(this.txtSearch.Text.ToUpper())
                    ).ToList();

                this.gridControl1.DataSource = lstHisEmrForms;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var rowData = (HIS_EMR_FORM)gridView1.GetFocusedRow();
                HIS_EMR_FORM dataCreat = new HIS_EMR_FORM();
                dataCreat = rowData;
                this._lstHisEmrForm.Remove(rowData);
                if (rowData.IS_ACTIVE == 1)
                {
                    dataCreat.IS_ACTIVE = 0;
                }
                else
                {
                    dataCreat.IS_ACTIVE = 1;
                }

                this._lstHisEmrForm.Add(dataCreat);

                gridControl1.BeginUpdate();
                gridControl1.DataSource = this._lstHisEmrForm.OrderByDescending(o => o.IS_ACTIVE);
                gridControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        { 
            try
            {
                btnSave_Click(null,null);
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
                if (!btnSave.Enabled || this._lstHisEmrForm == null || this._lstHisEmrForm.Count <= 0) return;
                CommonParam param = new CommonParam();

                bool resultData = false;
                if (_lstHisEmrForm != null && _lstHisEmrForm.Count > 0)
                {
                    resultData = new BackendAdapter(param).Post<bool>("api/HisEmrForm/ChangeActive", ApiConsumers.MosConsumer, _lstHisEmrForm, param);

                    Inventec.Common.Logging.LogSystem.Info("output: " + resultData+" input: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _lstHisEmrForm), _lstHisEmrForm) );

                    if (resultData)
                    {
                        FillDataToControl();
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this, param, resultData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null,null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }
    }
}
