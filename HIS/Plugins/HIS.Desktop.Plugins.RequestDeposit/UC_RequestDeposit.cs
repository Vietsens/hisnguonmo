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
using HIS.UC.ListDepositRequest;
using MOS.EFMODEL.DataModels;
using HIS.UC.ListDepositRequest.ADO;
using HIS.UC.ListDepositRequest.Reload;
using HIS.UC.ListDepositRequest.GetSelectRow;
using HIS.UC.ListDepositRequest.Run;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using Inventec.Common.RichEditor.Base;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Number;
using HIS.Desktop.Print;
using Inventec.Common.RichEditor.DAL;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.RequestDeposit
{
    public partial class UC_RequestDeposit : UserControl
    {
        ListDepositRequestProcessor listDepositReqProcessor = null;
        UserControl ucRequestDeposit = null;
        List<V_HIS_DEPOSIT_REQ> listDepositReq = new List<V_HIS_DEPOSIT_REQ>();
        List<V_HIS_DEPOSIT_REQ> currentlistDepositReq = new List<V_HIS_DEPOSIT_REQ>();
        V_HIS_DEPOSIT_REQ depositReq = new V_HIS_DEPOSIT_REQ();
        private V_HIS_DEPOSIT_REQ depositReqPrint { get; set; }
        V_HIS_DEPOSIT_REQ currentdepositReq = new V_HIS_DEPOSIT_REQ();
        ListDepositRequestInitADO listDepositRequestADO = new ListDepositRequestInitADO();
       
        internal int action = -1;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentID;
        bool isUpdate = false;

        bool isSupplement = false;

        long roomId;
        long roomTypeId;

        int positionHandleControl = -1;
        public UC_RequestDeposit(Inventec.Desktop.Common.Modules.Module module, long treatmentID)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.treatmentID = treatmentID;              
                //getDataSelectRow();
                InitListDepositReqGrid();              
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UC_RequestDeposit_Load(object sender, EventArgs e)
        {
            try
            {
                getDataDepositReq(this.treatmentID);
                FillDataToGridDepositReq();
                this.action = GlobalVariables.ActionAdd;
                EnableControlChanged(action);
                txtAmount.Focus();
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        void Grid_DeleteClick(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                CommonParam param= new CommonParam();
                bool success = false;
                WaitingManager.Show();
                //currentdepositReq = null;
                if (data != null)
                {                   
                    var process = new V_HIS_DEPOSIT_REQ();
                    process = data;
                    var result = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_DEPOSIT_REQ_DELETE, ApiConsumer.ApiConsumers.MosConsumer, process.ID, param);
                    if (result)
                    {
                        success = true;
                        getDataDepositReq(this.treatmentID);
                        FillDataToGridDepositReq();
                    }
                    WaitingManager.Hide();

                    #region Show message
                    ResultManager.ShowMessage(param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void Grid_PrintClick(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                if (data != null)
                {              
                    depositReqPrint = data;
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__YEU_CAU_TAM_UNG__MPS000091, delegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void Grid_RowCellClick(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                currentdepositReq = null;
                if (data != null)
                {                 
                    currentdepositReq = data;
                    txtAmount.Text = ((long)(data.AMOUNT)).ToString();
                    txtGhiChu.Text = data.DESCRIPTION;
                    this.action = GlobalVariables.ActionEdit;
                    EnableControlChanged(action);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitListDepositReqGrid()
        {
            try
            {
                //var culture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //var langManager = Base.ResourceLangManager.LanguageUCExpMestChmsCreate;

                this.listDepositReqProcessor = new ListDepositRequestProcessor();
                ListDepositRequestInitADO ado = new ListDepositRequestInitADO();
                ado.ListDepositReqGrid_CustomUnboundColumnData = depositReqGrid__CustomUnboundColumnData;
                ado.ListDepositReqGrid_RowCellClick = Grid_RowCellClick;
                ado.ListDepositReqGrid_CustomRowCellEdit = gridView_CustomRowCellEdit;
                ado._btnDelete_Click = Grid_DeleteClick;
                ado._btnPrint_Click = Grid_PrintClick;
                
                //ado.ListDepositReqGrid_KeyUp = Grid_KeyUp;
                ado.IsShowSearchPanel = false;
                ado.IsShowPagingPanel = true;
                ado.ListDepositReqColumn = new List<ListDepositRequestColumn>();

                //ListDepositRequestColumn colSTT = new ListDepositRequestColumn("STT", "STT", 40, false,true);
                //colSTT.VisibleIndex = 0;
                //colSTT.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListDepositReqColumn.Add(colSTT);

                //ListDepositRequestColumn colDepositReqCode = new ListDepositRequestColumn("Mã yêu cầu", "DEPOSIT_REQ_CODE", 100, false,true);
                //colDepositReqCode.VisibleIndex = 1;   
                //ado.ListDepositReqColumn.Add(colDepositReqCode);

                //ListDepositRequestColumn colDelete = new ListDepositRequestColumn("Xóa yêu cầu", "DELETE", 20, true, false);
                //colDelete.VisibleIndex = 2;
                ////colDelete.ColumnEdit = btnDeleteE;
                //colDelete.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListDepositReqColumn.Add(colDelete);

                //ListDepositRequestColumn colPrint = new ListDepositRequestColumn("In yêu cầu", "PRINT", 20, true, false);
                //colPrint.VisibleIndex = 3;
                ////colPrint.ColumnEdit = btnPrintE;
                //colPrint.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                //ado.ListDepositReqColumn.Add(colPrint);

                ListDepositRequestColumn colAmount = new ListDepositRequestColumn("Số tiền", "AMOUNT_DISPLAY", 120, false,true);
                colAmount.VisibleIndex = 4;
                colAmount.Format = new DevExpress.Utils.FormatInfo();
                colAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                colAmount.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                colAmount.Format.FormatString = "#,##0.00";
                ado.ListDepositReqColumn.Add(colAmount);

                ListDepositRequestColumn colDescription = new ListDepositRequestColumn("Ghi chú", "DESCRIPTION", 200, false,true);
                colDescription.VisibleIndex = 5;
                ado.ListDepositReqColumn.Add(colDescription);

                ListDepositRequestColumn colRoomName = new ListDepositRequestColumn("Phòng yêu cầu", "ROOM_NAME", 120, false,true);
                colRoomName.VisibleIndex = 6;
                ado.ListDepositReqColumn.Add(colRoomName);

                ListDepositRequestColumn colDepartName = new ListDepositRequestColumn("Khoa yêu cầu", "DEPARTMENT_NAME", 120, false, true);
                colDepartName.VisibleIndex = 7;
                ado.ListDepositReqColumn.Add(colDepartName);

                ListDepositRequestColumn colCreateTime = new ListDepositRequestColumn("Thời gian tạo", "CREATE_TIME_DISPLAY", 120, false, true);
                colCreateTime.VisibleIndex = 8;
                colCreateTime.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListDepositReqColumn.Add(colCreateTime);

                ListDepositRequestColumn colCreator = new ListDepositRequestColumn("Người tạo", "CREATOR", 80, false,true);
                colCreator.VisibleIndex = 9;
                ado.ListDepositReqColumn.Add(colCreator);

                ListDepositRequestColumn colModifyTime = new ListDepositRequestColumn("Thời gian sửa", "MODIFY_TIME_DISPLAY", 120, false, true);
                colModifyTime.VisibleIndex = 10;
                colModifyTime.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
                ado.ListDepositReqColumn.Add(colModifyTime);

                ListDepositRequestColumn colModifier = new ListDepositRequestColumn("Người sửa", "MODIFIER", 80, false,true);
                colModifier.VisibleIndex = 11;
                ado.ListDepositReqColumn.Add(colModifier);

                this.ucRequestDeposit = (UserControl)this.listDepositReqProcessor.Run(ado);
                if (this.ucRequestDeposit != null)
                {
                    this.panelControl1.Controls.Add(this.ucRequestDeposit);
                    this.ucRequestDeposit.Dock = DockStyle.Fill; 
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }  
    }
}
