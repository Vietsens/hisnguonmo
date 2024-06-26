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
using HIS.UC.DepositRequestList;
using MOS.EFMODEL.DataModels;
using HIS.UC.DepositRequestList;
using HIS.UC.DepositRequestList.GetFocusRow;
using HIS.UC.DepositRequestList.Reload;
using HIS.UC.DepositRequestList.ADO;
using HIS.UC.DepositRequestList.Run;
using MOS.Filter;
using MOS.EFMODEL;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Controls.ValidationRule;




namespace HIS.Desktop.Plugins.DepositReq
{
    public partial class UC_DepositReq : UserControl
    {
        HIS.UC.DepositRequestList.ADO.DepositRequestInitADO DepositRequestInitADO = new HIS.UC.DepositRequestList.ADO.DepositRequestInitADO();
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_TREATMENT treatment;
        UserControl ucDepositRq;
        DepositRequestInitADO ado= new DepositRequestInitADO();
        DepositRequestListProcessor depositReqProcessor;
        long treatmentID;    
        int positionHandleControl = -1;
        internal int action = -1;
        HIS_DEPOSIT_REQ depositReq { get; set; }
        V_HIS_DEPOSIT_REQ depositReqView;
        private List<V_HIS_DEPOSIT_REQ> depositReqs { get; set; }
        public UC_DepositReq()
        {
            InitializeComponent();
            loaddata();
        }

        public UC_DepositReq(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentID)
        {
            InitializeComponent();
            
            try
            {
                this.currentModule = currentModule;
                this.treatmentID = treatmentID;

                this.action = GlobalVariables.ActionEdit;
               // this.treatment = treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UC_DepositReq_Load(object sender, EventArgs e)
        {
           // MeShow();
            loaddata();
            ValidateForm();
            if (this.action == GlobalVariables.ActionEdit)
            {
                if (depositReqView != null)
                {
                    txtSoTien.Text = ((long)depositReqView.AMOUNT).ToString();
                    txtGhiChu.Text = depositReqView.DESCRIPTION;
                }
            }
        }

        //private void EnableControlChanged(int action)
        //{
        //    try
        //    {
        //        btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
        //        btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void SetDefaultFocus()
        //{
        //    try
        //    {
        //        txtSoTien.Focus();
        //        txtSoTien.SelectAll();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Debug(ex);
        //    }
        //}
        //private void SetDefaultValue()
        //{
        //    try
        //    {
        //        this.action = GlobalVariables.ActionAdd;
        //       // txtKeyword.Text = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_DEPOSIT_REQ data)
        //{
        //    try
        //    {
        //        if (data != null)
        //        {
        //            txtSoTien.Text = data.AMOUNT.ToString();
        //            txtGhiChu.Text = data.DESCRIPTION;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_DEPOSIT_REQ data)
        //{
        //    try
        //    {
        //        if (data != null)
        //        {
        //            FillDataToEditorControl(data);
        //            this.action = GlobalVariables.ActionEdit;
        //            EnableControlChanged(this.action);

        //            //Disable nút sửa nếu dữ liệu đã bị khóa
        //            btnEdit.Enabled = (this.depositReq.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

        //            positionHandleControl = -1;
        //            Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        //private void SetFocusEditor()
        //{
        //    try
        //    {
        //        //TODO
        //        txtSoTien.Focus();
        //        txtSoTien.SelectAll();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Debug(ex);
        //    }
        //}

        //public void MeShow()
        //{
        //    try
        //    {
                //loaddata();
                //ValidateForm();
        //        SetDefaultValue();
        //        //EnableControlChanged(this.action);
        //        SetDefaultFocus();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void loaddata()
        {
            CommonParam param= new CommonParam();
            HisTreatmentFilter filter = new HisTreatmentFilter();
            ado.treatmentID = treatmentID;
            filter.ID = treatmentID;
            //treatment = new BackendAdapter(param).Get<V_HIS_TREATMENT>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
            //this.depositReqView = new BackendAdapter(param).Get<V_HIS_DEPOSIT_REQ>(HisRequestUriStore.HIS_DEPOSIT_REQ_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
           // DepositRequestInitADO ado = new DepositRequestInitADO();
            depositReqProcessor = new UC.DepositRequestList.DepositRequestListProcessor(); 
            this.ucDepositRq = (UserControl)depositReqProcessor.Run(ado);
            if (this.ucDepositRq != null)
            {
                this.panelControl1.Controls.Add(this.ucDepositRq);
                this.ucDepositRq.Dock = DockStyle.Fill;
                //this.ucDepositRq.Margin.All(0);
            }
        }


        public void UpdateDepositReqToGrid(HIS_DEPOSIT_REQ depositReqTemp, long action)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (action == GlobalVariables.ActionAdd)
                {
                    HisDepositReqViewFilter filter = new HisDepositReqViewFilter();
                    filter.ID = depositReqTemp.ID;
                    //V_HIS_DEPOSIT_REQ depositReqView = new HisDepositReqLogic().GetView<List<V_HIS_DEPOSIT_REQ>>(filter).FirstOrDefault();
                    if (depositReqView != null)
                    {
                        depositReqs.Add(depositReqView);
                    }
                }
                if (action == GlobalVariables.ActionEdit)
                {
                    HisDepositReqViewFilter filter = new HisDepositReqViewFilter();
                    filter.ID = depositReqTemp.ID;
                    V_HIS_DEPOSIT_REQ depositReqView = new BackendAdapter(param).Get<List<V_HIS_DEPOSIT_REQ>>(HisRequestUriStore.HIS_DEPOSIT_REQ_GETVIEW,ApiConsumer.ApiConsumers.MosConsumer,filter,null).FirstOrDefault();
                    
                    foreach (var depositReq in depositReqs)
                    {
                        if (depositReq.ID == depositReqTemp.ID)
                        {
                            depositReq.AMOUNT = depositReqView.AMOUNT;
                            depositReq.DESCRIPTION = depositReqView.DESCRIPTION;
                        }

                    }

                }
                //gridControlDeposit.RefreshDataSource();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            //this.action = GlobalVariables.ActionAdd;
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {

                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                this.depositReq = new HIS_DEPOSIT_REQ();
                this.depositReq.AMOUNT = Convert.ToDecimal(txtSoTien.Text);
                this.depositReq.DESCRIPTION = txtGhiChu.Text;
                this.depositReq.REQUEST_ROOM_ID = WorkPlace.GetRoomId();
                this.depositReq.REQUEST_DEPARTMENT_ID = WorkPlace.GetDepartmentId();
                if (this.action == GlobalVariables.ActionAdd)
                {
                    this.depositReq.TREATMENT_ID = treatmentID;
                    var dataResult = new BackendAdapter(param).Post<HIS_DEPOSIT_REQ>(HisRequestUriStore.HIS_DEPOSIT_REQ_CREATE, ApiConsumer.ApiConsumers.MosConsumer, this.depositReq, null);
                    if (dataResult != null)
                    {
                        this.depositReq = dataResult;
                        success = true;
                        UpdateDepositReqToGrid(dataResult, action);
                    }               
                }

                WaitingManager.Hide();

                #region Show message
                ResultManager.ShowMessage(param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //this.action = GlobalVariables.ActionEdit;
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {

                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                HisDepositReqViewFilter filter = new HisDepositReqViewFilter();
                filter.TREATMENT_ID = treatmentID;
                depositReqView = new BackendAdapter(param).Get<List<V_HIS_DEPOSIT_REQ>>(HisRequestUriStore.HIS_DEPOSIT_REQ_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                this.depositReq = new HIS_DEPOSIT_REQ();
                this.depositReq.AMOUNT = Convert.ToDecimal(txtSoTien.Text);
                this.depositReq.DESCRIPTION = txtGhiChu.Text;
                this.depositReq.REQUEST_ROOM_ID = WorkPlace.GetRoomId();
                //this.depositReq.REQUEST_DEPARTMENT_ID = WorkPlace.GetDepartmentId(); 
                if (this.action == GlobalVariables.ActionEdit)
                {
                    this.depositReq.TREATMENT_ID = treatmentID;
                    this.depositReq.ID = depositReqView.ID;                   
                    var dataResult = new BackendAdapter(param).Post<HIS_DEPOSIT_REQ>(HisRequestUriStore.HIS_DEPOSIT_REQ_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, this.depositReq, null);
                    if (dataResult != null)
                    {
                        this.depositReq = dataResult;
                        success = true;
                        UpdateDepositReqToGrid(dataResult, action);
                    }
                }
                WaitingManager.Hide();

                #region Show message
                ResultManager.ShowMessage(param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion


            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //SetDefaultValue();
        }

      


    }
}
