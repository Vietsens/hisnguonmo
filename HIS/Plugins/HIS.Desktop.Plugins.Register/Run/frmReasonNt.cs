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
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
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
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_HOSPITALIZE_REASON>>("/api/HisHospitalize/Get",ApiConsumers.MosConsumer,filter,paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null)
                    {
                        grvReason.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
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
                    delegateData(selectedData.HOSPITALIZE_REASON_CODE, selectedData.HOSPITALIZE_REASON_NAME);
                }
                else delegateData("", "");
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
    }
    public delegate void DelegateLydoNT(string code, string name);
}