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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.Filter;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using HIS.Desktop.Controls.Session;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Print;
using Inventec.Common.RichEditor.DAL;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.DepositRequestList;
using HIS.UC.DepositRequestList.ADO;
using HIS.UC.DepositRequestList.GetFocusRow;
using HIS.UC.DepositRequestList.Run;
using HIS.UC.DepositRequestList.Reload;
//using Inventec.Common.WebApiClient;

namespace HIS.UC.DepositRequestList.Run
{
    public partial class UC_DepositRequestList : UserControl
    {
        #region Declare
       
        DepositRequestInitADO DepositRequestInitADO = new DepositRequestInitADO();
        private V_HIS_TREATMENT treatment { get; set; }
        private V_HIS_DEPOSIT_REQ depositReqPrint { get; set; }
        private List<V_HIS_DEPOSIT_REQ> depositReqs { get; set; }
        gridViewDeposit_CustomUnboundColumnData grvDeposit_CustomUnboundColumnData;
        gridviewHandler grv_btnDelete_Click;
        gridviewHandler grv_btnPrint_Click;
        gridviewHandler updateSingleRow;
        gridViewDeposit_CustomRowCellEdit grvDeposit_CustomRowCellEdit;
        gridViewDeposit_CustomDrawCell grvDeposit_CustomDrawCell;
        List<ColumnButtonEditADO> columnButtonEdits;
        List<DepositRequestADO> DepositRequestADOs = new List<DepositRequestADO>();
        long treatmentID;
        #endregion

        #region Contructor
        public UC_DepositRequestList()
        {
            InitializeComponent();
        }

        public UC_DepositRequestList(DepositRequestInitADO data)
        {
            InitializeComponent();
            try
            {
               this.treatmentID = data.treatmentID;
               this.grvDeposit_CustomUnboundColumnData = data.gridViewDeposit_CustomUnboundColumnData;
               this.grv_btnDelete_Click = data.btnDelete_Click;
               this.grv_btnPrint_Click = data.btnPrint_Click;
               this.grvDeposit_CustomDrawCell = data.gridViewDeposit_CustomDrawCell;
               this.grvDeposit_CustomRowCellEdit = data.gridViewDeposit_CustomRowCellEdit;
               this.updateSingleRow = data.UpdateSingleRow;
               //LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
#endregion

        #region Event
        private void gridControlDeposit_Click(object sender, EventArgs e)
        {           
           
        }

        private void gridViewDeposit_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {             
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_DEPOSIT_REQ data = (V_HIS_DEPOSIT_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "AMOUNT_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(data.AMOUNT);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {

                MessageBox.Show("d");
                
                //CommonParam param = new CommonParam();
                //bool success = false;
                //var row = (MOS.EFMODEL.DataModels.V_HIS_DEPOSIT_REQ)gridViewDeposit.GetFocusedRow();
                //DialogResult myResult;
                //myResult = MessageBox.Show("Bạn có muốn xóa yêu cầu tạm ứng này ?", "Delete Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                //if (myResult == DialogResult.OK)
                //{
                //    //Check nguoi tao
                //    if (row.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                //    {
                //        WaitingManager.Show();
                //        bool result = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_DEPOSIT_REQ_DELETE, ApiConsumers.MosConsumer, row.ID, null);
                //        WaitingManager.Hide(); 
                //        if (result)
                //        {
                //            success = true;
                //            depositReqs.Remove(row);
                //            gridControlDeposit.RefreshDataSource();
                //        }
                //    }
                //    else
                //    {
                //        MessageBox.Show("Yêu cầu này không phải do bạn tạo!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        return;
                //    }
                //}
                //else return;

                //#region Show message
                //ResultManager.ShowMessage(param, success);
                //#endregion

                //#region Process has exception
                //SessionManager.ProcessTokenLost(param);
                //#endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_DEPOSIT_REQ)gridViewDeposit.GetFocusedRow();
                if (row != null)
                {
                    depositReqPrint = row;
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, UriBaseStore.URI_API_SAR, LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__YEU_CAU_TAM_UNG__MPS000091, delegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool delegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                InYeuCauTamUng(printTypeCode, fileName, depositReqPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        
        #endregion

        #region Method

        private void LoadDataToGridControl()
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                MOS.Filter.HisDepositReqViewFilter depositReqFilter = new MOS.Filter.HisDepositReqViewFilter();
                depositReqFilter.TREATMENT_ID = treatmentID;

                depositReqs = new BackendAdapter(param).Get<List<V_HIS_DEPOSIT_REQ>>(HisRequestUriStore.HIS_DEPOSIT_REQ_GETVIEW, ApiConsumers.MosConsumer, depositReqFilter, null);
                gridControlDeposit.DataSource = depositReqs;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //public void UpdateDepositReqToGrid(HIS_DEPOSIT_REQ depositReqTemp, long action)
        //{
        //    try
        //    {
        //        CommonParam param= new CommonParam();
        //        if (action == GlobalVariables.ActionAdd)
        //        {
        //            HisDepositReqViewFilter filter = new HisDepositReqViewFilter();
        //            filter.ID = depositReqTemp.ID;
        //            V_HIS_DEPOSIT_REQ depositReqView = new BackendAdapter(param).Get<List<V_HIS_DEPOSIT_REQ>>(HisRequestUriStore.HIS_DEPOSIT_REQ_GETVIEW, ApiConsumers.MosConsumer, filter, null).FirstOrDefault();

        //            if (depositReqView != null)
        //            {
        //                depositReqs.Add(depositReqView);
        //            }
        //        }
        //        if (action == GlobalVariables.ActionEdit)
        //        {
        //            HisDepositReqViewFilter filter = new HisDepositReqViewFilter();
        //            filter.ID = depositReqTemp.ID;
        //            V_HIS_DEPOSIT_REQ depositReqView = new BackendAdapter(param).Get<List<V_HIS_DEPOSIT_REQ>>(HisRequestUriStore.HIS_DEPOSIT_REQ_GETVIEW, ApiConsumers.MosConsumer, filter, null).FirstOrDefault();

        //            foreach (var depositReq in depositReqs)
        //            {
        //                if (depositReq.ID == depositReqTemp.ID)
        //                {
        //                    depositReq.AMOUNT = depositReqView.AMOUNT;
        //                    depositReq.DESCRIPTION = depositReqView.DESCRIPTION;
        //                }

        //            }

        //        }
        //        gridControlDeposit.RefreshDataSource();

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        public void Reload()
        {
            try
            {
                //txtKeyword.Text = ""; 
                
                gridControlDeposit.DataSource=null;
                LoadDataToGridControl();
                //this.SereServADOs = (from m in TreeSereServ7ADO.SereServs select new SereServADO(m)).ToList();
                //records = new BindingList<SereServADO>(SereServADOs);
                //trvService.DataSource = records;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }         
        }


        public List<V_HIS_DEPOSIT_REQ> GetFocusRow()
        {

            List<V_HIS_DEPOSIT_REQ> result = new List<V_HIS_DEPOSIT_REQ>();
            try
            {
                result = (List<V_HIS_DEPOSIT_REQ>)gridViewDeposit.GetFocusedRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<V_HIS_DEPOSIT_REQ>();
            }
            return result;
        
        }
        #endregion

        private void UC_DepositRequestList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridViewDeposit_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            /*try
            {
                if (e.RowHandle < 0)
                    return;
                var row = (V_HIS_DEPOSIT_REQ)gridViewDeposit.GetFocusedRow();
                var data = (V_HIS_DEPOSIT_REQ)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null && data is DepositRequestADO)
                {
                    var rowData = data as DepositRequestADO;
                    
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (row.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                        {
                            e.RepositoryItem = btnDelete;
                        }
                        else
                        {
                            e.RepositoryItem = btnDeleteD;
                        }
                        if (e.Column.FieldName == "PRINT")
                        {
                            if (data.DEPOSIT_ID == null)
                            {
                                e.RepositoryItem = btnPrint;
                            }
                            else
                            {
                                e.RepositoryItem = btnPrintD;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }*/
        }

        private void gridViewDeposit_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {

        }  
    }
}
