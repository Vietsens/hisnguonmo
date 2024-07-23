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
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Controls.Session;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class frmReasonNt : DevExpress.XtraEditors.XtraForm
    {
        DelegateLydoNT delegateData;
        public frmReasonNt(DelegateLydoNT reasonNT)
        {
            this.delegateData = reasonNT;
            InitializeComponent();
        }

        private void frmReasonNt_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                FillDataToControl();
                if (this.txtReasonName != null) txtReasonName.Focus();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #region load language

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmReasonNT = new ResourceManager("HIS.Desktop.Plugins.Register.Resources.Lang", typeof(frmReasonNt).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmReasonNt.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmReasonNT, LanguageManager.GetCulture());
                this.txtSearchvalue.Properties.NullText = Inventec.Common.Resource.Get.Value("frmReasonNt.txtSearchvalue.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmReasonNT, LanguageManager.GetCulture());
                this.txtSearchvalue.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmReasonNt.txtSearchvalue.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmReasonNT, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmReasonNt.btnSave.Text", Resources.ResourceLanguageManager.LanguagefrmReasonNT, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmReasonNt.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguagefrmReasonNT, LanguageManager.GetCulture());
                this.HOSPITALIZE_REASON_CODE.Caption = Inventec.Common.Resource.Get.Value("frmReasonNt.HOSPITALIZE_REASON_CODE.Caption", Resources.ResourceLanguageManager.LanguagefrmReasonNT, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmReasonNt.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguagefrmReasonNT, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmReasonNt.Text", Resources.ResourceLanguageManager.LanguagefrmReasonNT, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        public void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();


                int pageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, pageSize, this.grcReason);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                grcReason.BeginUpdate();
                MOS.Filter.HisHospitalizeReasonFilter filter = new MOS.Filter.HisHospitalizeReasonFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                SetFilter(ref filter);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>> apiResult = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_HOSPITALIZE_REASON>>("/api/HisHospitalizeReason/Get",ApiConsumers.MosConsumer,filter,paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null)
                    {
                        grcReason.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                grcReason.EndUpdate();
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                
                LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref MOS.Filter.HisHospitalizeReasonFilter filter)
        {
            try
            {
                filter.KEY_WORD = this.txtSearchvalue.Text.Trim();
            }
            catch (Exception ex)
            {
                
                LogSystem.Error(ex);
            }
        }

        private void grvReason_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
            }
            catch (Exception ex)
            {
                
                LogSystem.Error(ex);
            }
        }
        private HIS_HOSPITALIZE_REASON selectedData = new HIS_HOSPITALIZE_REASON();
        private void grvReason_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var rowData = grvReason.GetFocusedRow();
                if (rowData != null && rowData is HIS_HOSPITALIZE_REASON)
                {
                    selectedData = (HIS_HOSPITALIZE_REASON)rowData;
                    this.txtReasonCode.Text = selectedData.HOSPITALIZE_REASON_CODE;
                    this.txtReasonName.Text = selectedData.HOSPITALIZE_REASON_NAME;
                }
            }
            catch (Exception ex)
            {
                
                LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedData != null)
                {
                    delegateData(selectedData);
                    this.Close();
                }
                else
                {
                    delegateData(null);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                
                LogSystem.Error(ex);
            }
        }

        private void frmReasonNt_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.S)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                
                LogSystem.Error(ex);
            }
        }

        private void txtSearchvalue_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                
               LogSystem.Error(ex);
            }
        }

        private void txtReasonName_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.txtReasonCode.Text = "";
                this.txtReasonName.Text = "";
                this.selectedData = null;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtReasonName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtReasonName.Text))
                {
                    txtReasonCode.Text = "";
                    this.selectedData = null;
                }
                    
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtReasonCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtReasonCode.Text))
                {
                    txtReasonCode.Text = "";
                    this.selectedData = null;
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
    }
    public delegate void DelegateLydoNT(HIS_HOSPITALIZE_REASON data);
}
