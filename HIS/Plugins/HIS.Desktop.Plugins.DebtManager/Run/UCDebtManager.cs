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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using AutoMapper;
using HIS.Desktop.Common;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors;
using HIS.Desktop.Utility;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using System.Threading;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.Plugins.DebtManager.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill;

namespace HIS.Desktop.Plugins.DebtManager
{
    public partial class UCDebtManager : HIS.Desktop.Utility.UserControlBase
    {
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        CommonParam param = new CommonParam();
        int rowCount = 0;
        int dataTotal = 0;
        int startPage;
        int limit;
        internal List<V_HIS_TRANSACTION> currentTransaction { get; set; }
        internal List<V_HIS_TRANSACTION_1> currentTransaction1 { get; set; }
        V_HIS_TRANSACTION _transaction;
        List<V_HIS_TRANSACTION> listData;
        HIS_PATIENT _patient;
        List<Transaction1ADO> _Transaction1ADOs = new List<Transaction1ADO>();
        bool isCheckAll = true;
        List<ADO.Transaction1ADO> VTranSaction1;
        internal Inventec.Desktop.Common.Modules.Module currentModule;

        public UCDebtManager()
        {
            InitializeComponent();
        }

        public UCDebtManager(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCDebtManager_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();

                SetCaptionByLanguageKey();

                this.gridControlTracsaction1.ToolTipController = this.toolTipController1;
                this.gridControlTransaction.ToolTipController = this.toolTipController1;
                SetDataDefaultUC();
                btnDebt.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DebtManager.Resources.Lang", typeof(HIS.Desktop.Plugins.DebtManager.UCDebtManager).Assembly);
                this.gridColSTT.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.gridColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSTT1.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.gridColSTT1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.txtTranactionCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCDebtManager.txtTranactionCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCDebtManager.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCDebtManager.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtFromTime.Text = Inventec.Common.Resource.Get.Value("UCDebtManager.dtFromTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtToTime.Text = Inventec.Common.Resource.Get.Value("UCDebtManager.dtToTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCDebtManager.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDebt.Text = Inventec.Common.Resource.Get.Value("UCDebtManager.btnDebt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //GridControlTransactionView1

                this.gridColumnCheck.ToolTip = Inventec.Common.Resource.Get.Value("UCDebtManager.gridColumnCheck.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDelete1.ToolTip = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColDelete1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTransactionPrint1.ToolTip = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColTransactionPrint1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColAmount1.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.gridColAmount1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTrasactionCode1.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColTrasactionCode1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentCode1.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColTreatmentCode1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientName1.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColPatientName1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCancelTime1.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColCancelTime1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTranactionTime1.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColTranactionTime1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentCode1.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColTreatmentCode1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientCode1.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColPatientCode1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTdlPatientDob.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColTdlPatientDob1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColTdlPatientGenderName.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.gridColTdlPatientGenderName1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCancelUsername1.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColCancelUsername1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCashierUsername1.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColCashierUsername1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //GridControlTransactionView
                this.grdColDelete.ToolTip = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColDelete.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTransactionPrint.ToolTip = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColTransactionPrint.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAmount.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTransactionCode.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColTransactionCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTdlTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColTdlTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCancelUsername.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColCancelUsername.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCancelTime.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColCancelTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTdlPatientDob.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.grdColTdlPatientDob.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColTdlPatientGenderName.Caption = Inventec.Common.Resource.Get.Value("UCDebtManager.gridColTdlPatientGenderName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                dtFromTime.EditValue = DateTime.Now;
                dtToTime.EditValue = DateTime.Now;
                //gridControlTracsaction1.DataSource = null;
                //gridControlTransaction.DataSource = null;
                FillDataToGridTransaction();
                FillDataToGridTransaction1();

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDataDefaultUC()
        {
            try
            {
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<V_HIS_TRANSACTION_1> _TransactionChecks { get; set; }

        private void SelectCheckTransaction()
        {
            try
            {
                _TransactionChecks = new List<V_HIS_TRANSACTION_1>();
                if (this._Transaction1ADOs != null && this._Transaction1ADOs.Count > 0)
                {
                    _Transaction1ADOs.AddRange(this._Transaction1ADOs.Where(p => p.IsCheck == true).ToList());
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }



        private void gridViewTransaction1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    V_HIS_TRANSACTION_1 data = (V_HIS_TRANSACTION_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "TRANSACTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TRANSACTION_TIME);
                    }
                    else if (e.Column.FieldName == "DEBT_COLLECTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.DEBT_COLLECTION_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB ?? 0);
                    }
                    else if (e.Column.FieldName == "WORK_PLACE")
                    {
                        e.Value = !String.IsNullOrWhiteSpace(data.KSK_WORK_PLACE_NAME) ? data.KSK_WORK_PLACE_NAME : (!String.IsNullOrWhiteSpace(data.TDL_PATIENT_WORK_PLACE_NAME) ? data.TDL_PATIENT_WORK_PLACE_NAME : data.TDL_PATIENT_WORK_PLACE);
                    }
                    //IsCheck
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTransaction_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    MOS.EFMODEL.DataModels.V_HIS_TRANSACTION data = (MOS.EFMODEL.DataModels.V_HIS_TRANSACTION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }

                    else if (e.Column.FieldName == "TRANSACTION_TIME_DISPLAY_Str")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TRANSACTION_TIME);
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_DOB_Str")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void gridViewTransaction1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    V_HIS_TRANSACTION_1 data = (V_HIS_TRANSACTION_1)gridViewTransaction1.GetRow(e.RowHandle);
                    //DEBT_BILL_ID == nul => chưa thu nợ
                    //DEBT_BILL_ID != nul => đã thu nợ
                    FormatSpint(gridColAmount1, data.AMOUNT);

                    if (e.Column.FieldName == "IsCheck")
                    {
                        if (data.DEBT_BILL_ID > 0 || data.IS_CANCEL == 1)
                        {
                            e.RepositoryItem = repositoryItemCheck_D;
                        }
                        else
                            e.RepositoryItem = repositoryItemCheck_E;
                    }
                    if (e.Column.FieldName == "DELETE_DISPLAY_STR")
                    {
                        if (data.DEBT_BILL_ID == null && data.IS_CANCEL != 1)
                            e.RepositoryItem = btnDelete;
                        else
                            e.RepositoryItem = btnDeleteVisible;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTransaction_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    MOS.EFMODEL.DataModels.V_HIS_TRANSACTION data = (MOS.EFMODEL.DataModels.V_HIS_TRANSACTION)gridViewTransaction1.GetRow(e.RowHandle);
                    //if (e.Column.FieldName == "grdColDelete")
                    //    e.RepositoryItem = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryItemButtonEdit_Delete : repositoryItemButtonEdit_DeleteDisable;

                    if (e.Column.FieldName == "DELETE_DISPLAY")
                    {
                        if (data.IS_CANCEL == 1)
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_DeleteDisable;
                        }
                        else
                            e.RepositoryItem = repositoryItemButtonEdit_Delete;
                    }
                    if (e.Column.FieldName == "PRINT_DISPLAY")
                    {
                        if (data.IS_CANCEL == 1)
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_DeleteDisable;
                        }
                        else
                            e.RepositoryItem = buttonEditPrint;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlAggrImpMest_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    var row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2)gridViewAggrImpMest.GetFocusedRow();
            //    if (row != null)
            //    {
            //        this.aggrImpMestId = row.ID;
            //        //LoadDataDetailAggrImpMest();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogSystem.Error(ex);
            //}
        }

        private void repositoryItemButtonEdit_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewTransaction.FocusedRowHandle >= 0)
                {
                    var data = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                    if (data == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Data thuc hien huy giao dich null: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                        return;
                    }

                    if (!this.CancelElectronicBill(data))
                    {
                        return;
                    }

                    //if (data.TRANSACTION_TYPE_CODE == SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_TRANSACTION_TYPE__TRANSACTION_TYPE_CODE__BILL))
                    //{
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionCancel").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionCancel'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data);
                        listArgs.Add(this.currentModule);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("extenceInstance is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                        FillDataToGridControlTransaction();
                    }
                    else
                    {
                        MessageManager.Show("ChucNangNayChuaDuocHoTroTrongPhienBanNay");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewTransaction1.FocusedRowHandle >= 0)
                {
                    var data = (ADO.Transaction1ADO)gridViewTransaction1.GetFocusedRow();
                    if (data == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Data thuc hien huy giao dich null: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                        return;
                    }
                    //if (data.TREATMENT_ID != null)
                    //{
                    //    MessageManager.Show("XuLyThatBai.HoSoDieuTriDaDuocKhoaTaiChinh");
                    //    return;
                    //}

                    if (!this.CancelElectronicBill1(data))
                    {
                        return;
                    }

                    //if (data.TRANSACTION_TYPE_CODE == SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_TRANSACTION_TYPE__TRANSACTION_TYPE_CODE__BILL))
                    //{
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionCancel").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionCancel'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        V_HIS_TRANSACTION data1 = new V_HIS_TRANSACTION();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TRANSACTION>(data1, data);
                        listArgs.Add(data1);
                        listArgs.Add(this.currentModule);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("extenceInstance is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                        LoadPagingTranaction1(new CommonParam(startPage, limit));
                        LoadPagingTranaction(new CommonParam(startPage, limit));
                    }
                    else
                    {
                        MessageManager.Show("ChucNangNayChuaDuocHoTroTrongPhienBanNay");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void buttonEditPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                _transaction = new V_HIS_TRANSACTION();
                _transaction = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                if (_transaction.IS_CANCEL == 1)
                {
                    MessageBox.Show("Không thể in giao dịch đã hủy");
                }
                else
                {
                    PrintTransaction type = new PrintTransaction();
                    type = PrintTransaction.IN_PHIEU_THU_CONG_NO;
                    listData = new List<V_HIS_TRANSACTION>();
                    listData.Add(_transaction);
                    PrintProcess(type);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDebt_Click(object sender, EventArgs e)
        {
            try
            {
                string messages = "";
                if (gridViewTransaction1.RowCount > 0)
                {
                    if (this.VTranSaction1 != null && this.VTranSaction1.Count > 0)
                    {
                        var TranSactionCheck = this.VTranSaction1.Where(o => o.IsCheck == true).ToList();
                        var data = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();
                        if (TranSactionCheck.Count > 0 && TranSactionCheck != null)
                        {
                            WaitingManager.Show();
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionDebtCollect").FirstOrDefault();
                            if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("HIS.Desktop.Plugins.TransactionDebtCollect");
                            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add((RefeshReference)bbtnFind);
                                listArgs.Add(TranSactionCheck.Select(o => o.ID).ToList());
                                listArgs.Add(TranSactionCheck.Select(o => o.TREATMENT_ID).FirstOrDefault());
                                //listArgs.Add((HIS.Desktop.Common.RefeshReference)SetDataDefaultUC);
                                listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                                ((Form)extenceInstance).ShowDialog();
                            }
                            WaitingManager.Hide();
                        }
                        else
                            messages = "Chưa chọn công nợ";
                    }
                    else
                        messages = "Dữ liệu rỗng";
                    if (!String.IsNullOrEmpty(messages))
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(messages, "Thông báo");
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDataDefaultUC();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnFind()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        public void bbtnDebt()
        {
            try
            {
                btnDebt_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormatSpint(DevExpress.XtraGrid.Columns.GridColumn grdColName, decimal number)
        {
            try
            {
                int munberSeperator = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator;
                int munbershowDecimal = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.Desktop.Plugins.ShowDecimalOption");
                string Format = "";
                if (munbershowDecimal == 0)
                {
                    if (munberSeperator == 0)
                    {
                        Format = "#,##0";
                    }
                    else
                    {
                        Format = "#,##0.";
                        for (int i = 0; i < munberSeperator; i++)
                        {
                            Format += "0";
                        }
                    }

                }
                else if (munbershowDecimal == 1)
                {
                    if (number != null)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToDecimal(number.ToString() ?? "") % 1 > 0)
                        {
                            if (munberSeperator == 0)
                            {
                                Format = "#,##0";
                            }
                            else
                            {
                                Format = "#,##0.";
                                for (int i = 0; i < munberSeperator; i++)
                                {
                                    Format += "0";
                                }
                            }
                        }
                        else
                        {
                            Format = "#,##0";
                        }
                    }
                }
                grdColName.DisplayFormat.FormatString = Format;
                grdColName.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridTransaction1()
        {
            try
            {
                LoadPagingTranaction1(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingTransaction1.Init(LoadPagingTranaction1, param, (int)ConfigApplications.NumPageSize, this.gridControlTracsaction1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridTransaction()
        {
            try
            {
                LoadPagingTranaction(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingTransaction.Init(LoadPagingTranaction, param, (int)ConfigApplications.NumPageSize, this.gridControlTransaction);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControlTransaction()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPagingTransaction.pagingGrid != null)
                {
                    numPageSize = ucPagingTransaction.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPagingTranaction(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                //ucPaging.Init(loadPaging, param);
                ucPagingTransaction.Init(LoadPagingTranaction, param, numPageSize, gridControlTransaction);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadPagingTranaction1(object param)
        {
            try
            {

                startPage = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(startPage, limit);

                MOS.Filter.HisTransactionView1Filter filterSearch = new HisTransactionView1Filter();
                SetFilter(ref filterSearch);

                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION_1>> apiResult = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION_1>>
                  ("api/HisTransaction/GetView1", ApiConsumers.MosConsumer, filterSearch, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION_1>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        VTranSaction1 = new List<Transaction1ADO>();
                        foreach (var item in data)
                        {
                            Transaction1ADO dataitem = new Transaction1ADO();

                            Inventec.Common.Mapper.DataObjectMapper.Map<Transaction1ADO>(dataitem, item);
                            dataitem.IsCheck = false;
                            VTranSaction1.Add(dataitem);
                        }

                    }
                    else
                    {
                        VTranSaction1 = null;
                    }
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    gridControlTracsaction1.BeginUpdate();
                    gridViewTransaction1.GridControl.DataSource = VTranSaction1;
                    gridControlTracsaction1.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadPagingTranaction(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;

                CommonParam paramCommon = new CommonParam(startPage, limit);
                HisTransactionViewFilter filter = new HisTransactionViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.TRANSACTION_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filter.IS_DEBT_COLLECTION = true;
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION>> apiResult = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION>>
                  ("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION>)apiResult.Data;
                    gridViewTransaction.BeginUpdate();
                    gridViewTransaction.GridControl.DataSource = data;
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    gridViewTransaction.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtTranactionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    FillDataToGridTransaction1();
                    FillDataToGridTransaction();
                    WaitingManager.Hide();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    FillDataToGridTransaction1();
                    FillDataToGridTransaction();
                    WaitingManager.Hide();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTransaction1_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {

        }

        private void gridViewTransaction1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                ADO.Transaction1ADO dataRow = (Transaction1ADO)gridViewTransaction1.GetRow(e.RowHandle);
                if (dataRow != null)
                {
                    // đã thu nợ --> hiển thị màu xanh
                    if (dataRow.DEBT_BILL_ID > 0)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                    // chưa thu nợ --> hiển thị màu đỏ
                    if (dataRow.DEBT_BILL_ID == null)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    if (dataRow.IS_CANCEL == 1)
                    {
                        //Giao dịch đã bị hủy => bị gạch
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);

                    }


                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTransaction_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {

                V_HIS_TRANSACTION dataRow = (V_HIS_TRANSACTION)gridViewTransaction.GetRow(e.RowHandle);
                if (dataRow != null)
                {
                    if (dataRow.ID > 0)
                    {
                        e.Appearance.ForeColor = Color.Blue;

                    }
                    if (dataRow.IS_CANCEL == 1)
                    {
                        //Giao dịch đã bị hủy => bị gạch
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {

                    FillDataToGridTransaction1();
                    FillDataToGridTransaction();



                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTransaction_CustomRowCellEdit_1(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_TRANSACTION)gridViewTransaction.GetRow(e.RowHandle);
                    FormatSpint(grdColAmount, data.AMOUNT);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "DELETE_DISPLAY")
                        {
                            if (data.IS_CANCEL == 1)
                            {
                                e.RepositoryItem = repositoryItemButtonEdit_DeleteDisable;
                            }
                            else
                                e.RepositoryItem = repositoryItemButtonEdit_Delete;
                        }
                        if (e.Column.FieldName == "PRINT_DISPLAY")
                        {
                            if (data.IS_CANCEL == 1)
                            {
                                buttonEditPrint.ReadOnly = true;

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

        internal enum PrintTransaction
        {
            IN_PHIEU_THU_CONG_NO,
            IN_PHIEU_XAC_NHAN_CONG_NO,
        }

        void PrintProcess(PrintTransaction printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTransaction.IN_PHIEU_XAC_NHAN_CONG_NO:
                        richEditorMain.RunPrintTemplate("Mps000369", DelegateRunPrinterTransaction);
                        break;
                    case PrintTransaction.IN_PHIEU_THU_CONG_NO:
                        richEditorMain.RunPrintTemplate("Mps000370", DelegateRunPrinterTransaction);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinterTransaction(string printTransaction, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTransaction)
                {
                    case "Mps000369":
                        InPhieuXacNhanCongNo(printTransaction, fileName, ref result);
                        break;
                    case "Mps000370":
                        InThuCongNo(printTransaction, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void InThuCongNo(string printTransaction, string fileName, ref bool result)
        {
            try
            {
                if (this._transaction == null)
                    return;
                WaitingManager.Show();
                //transactionPrint = (V_HIS_TRANSACTION)gridViewTransaction.GetFocusedRow();

                //if (listData == null || listData.Count == 0)
                //{
                //    throw new Exception("listData: null");
                //}
                HisTransactionFilter transactionFilter = new HisTransactionFilter();
                transactionFilter.DEBT_BILL_ID = _transaction.ID;
                CommonParam param = new CommonParam();
                var transaction = new BackendAdapter(param).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, transactionFilter, param);
                if (transaction == null || transaction.Count <= 0)
                {
                    throw new Exception("Khong lay duoc transaction theo TRANSACTION_ID: " + this._transaction.ID);
                }

                List<HIS_SERE_SERV> listSereServ = new List<HIS_SERE_SERV>();
                HisSereServDebtViewFilter ssDebtFilter = new HisSereServDebtViewFilter();
                ssDebtFilter.DEBT_IDs = transaction.Select(s => s.ID).ToList();
                var hisSSDebts = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/GetView", ApiConsumers.MosConsumer, ssDebtFilter, null);
                if (hisSSDebts != null || hisSSDebts.Count > 0)
                {
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.IDs = hisSSDebts.Select(s => s.SERE_SERV_ID).ToList();
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                }

                List<HIS_DEBT_GOODS> listDebtGood = new List<HIS_DEBT_GOODS>();

                HisDebtGoodsFilter debtGoodFilter = new HisDebtGoodsFilter();
                debtGoodFilter.DEBT_IDs = transaction.Select(s => s.ID).ToList();
                listDebtGood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_DEBT_GOODS>>("api/HisDebtGoods/Get", ApiConsumers.MosConsumer, debtGoodFilter, null);

                //HisSereServDebtViewFilter ssDebtFilter = new HisSereServDebtViewFilter();
                //ssDebtFilter.DEBT_IDs = transaction.Select(s => s.ID).ToList();
                //param = new CommonParam();
                //var hisSSDebts = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/GetView", ApiConsumers.MosConsumer, ssDebtFilter, param);
                //if (hisSSDebts == null || hisSSDebts.Count <= 0)
                //{
                //    Inventec.Common.Logging.LogSystem.Error("param:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                //    Inventec.Common.Logging.LogSystem.Error("ssDebtFilter:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ssDebtFilter), param));
                //    throw new Exception("Khong lay duoc hisSSDebts theo DEBT_ID: " + transaction.First().ID);

                //}

                //HisSereServFilter ssFilter = new HisSereServFilter();
                //ssFilter.IDs = hisSSDebts.Select(s => s.SERE_SERV_ID).ToList();
                //var listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                ////HIS_PATY_ALTER_BHYT patyAlterBhyt = null;

                HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                patyAlterAppliedFilter.TreatmentId = this._transaction.TREATMENT_ID.Value;
                var patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);
                if (patientTypeAlter == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Khong lay duoc PatientTypeAlterApplied: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._transaction.TREATMENT_CODE), this._transaction.TREATMENT_CODE));
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this._transaction.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                HIS_PATIENT patient = new HIS_PATIENT();
                if (this._transaction.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.ID = this._transaction.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.First();
                    }
                }

                MPS.Processor.Mps000370.PDO.Mps000370PDO pdo = new MPS.Processor.Mps000370.PDO.Mps000370PDO(
                    _transaction,
                    patient,
                    listSereServ,
                    departmentTran,
                    patientTypeAlter,
                    hisSSDebts,
                    BackendDataWorker.Get<HIS_SERVICE_TYPE>(),
                    listDebtGood
                    );

                MPS.ProcessorBase.Core.PrintData printData = null;

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTransaction))
                {
                    printerName = GlobalVariables.dicPrinter[printTransaction];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_transaction != null ? _transaction.TREATMENT_CODE : ""), printTransaction, currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTransaction, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTransaction, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXacNhanCongNo(string printTransaction, string fileName, ref bool result)
        {
            try
            {
                if (this._transaction == null)
                    return;
                WaitingManager.Show();

                HisSereServDebtViewFilter ssDebtFilter = new HisSereServDebtViewFilter();
                ssDebtFilter.DEBT_ID = this._transaction.ID;
                var hisSSDebts = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/GetView", ApiConsumers.MosConsumer, ssDebtFilter, null);
                if (hisSSDebts == null || hisSSDebts.Count <= 0)
                {
                    throw new Exception("Khong lay duoc hisSSDebts theo DEBT_ID: " + this._transaction.ID);
                }

                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.IDs = hisSSDebts.Select(s => s.SERE_SERV_ID).ToList();
                var listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                //HIS_PATY_ALTER_BHYT patyAlterBhyt = null;

                HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                patyAlterAppliedFilter.TreatmentId = this._transaction.TREATMENT_ID.Value;
                var patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);
                if (patientTypeAlter == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Khong lay duoc PatientTypeAlterApplied: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._transaction.TREATMENT_CODE), this._transaction.TREATMENT_CODE));
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this._transaction.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                HIS_PATIENT patient = new HIS_PATIENT();
                if (this._transaction.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.ID = this._transaction.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.First();
                    }
                }

                MPS.Processor.Mps000369.PDO.Mps000369PDO pdo = new MPS.Processor.Mps000369.PDO.Mps000369PDO(
                    _transaction,
                    patient,
                    listSereServ,
                    departmentTran,
                    patientTypeAlter,
                    hisSSDebts,
                    BackendDataWorker.Get<HIS_SERVICE_TYPE>()
                    );

                MPS.ProcessorBase.Core.PrintData printData = null;

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTransaction))
                {
                    printerName = GlobalVariables.dicPrinter[printTransaction];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_transaction != null ? _transaction.TREATMENT_CODE : ""), printTransaction, currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTransaction, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTransaction, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref MOS.Filter.HisTransactionView1Filter filter)
        {
            try
            {
                if (!checkIncludeCancel.Checked)
                {
                    filter.IS_CANCEL = false;
                }

                if (!string.IsNullOrEmpty(txtTranactionCode.Text))
                {
                    string code = txtTranactionCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTranactionCode.Text = code;
                    }
                    filter.TRANSACTION_CODE__EXACT = code;
                }

                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code1 = txtTreatmentCode.Text.Trim();
                    if (code1.Length < 12 && checkDigit(code1))
                    {
                        code1 = string.Format("{0:000000000000}", Convert.ToInt64(code1));
                        txtTreatmentCode.Text = code1;
                    }
                    filter.TREATMENT_CODE__EXACT = code1;
                }

                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyword.Text.Trim();

                if (cboStatus.SelectedIndex == 0)
                {
                    filter.HAS_DEBT_BILL_ID = null;
                }
                else if (cboStatus.SelectedIndex == 1)
                {
                    filter.HAS_DEBT_BILL_ID = false;
                }
                else if (cboStatus.SelectedIndex == 2)
                {
                    filter.HAS_DEBT_BILL_ID = true;
                }

                if (dtFromTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                    filter.TRANSACTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtFromTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                    filter.TRANSACTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "235959");


                filter.TRANSACTION_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO };
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbuttonEditPrint1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                _transaction = new V_HIS_TRANSACTION();
                var _transaction1 = (Transaction1ADO)gridViewTransaction1.GetFocusedRow();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TRANSACTION>(_transaction, _transaction1);
                PrintTransaction type = new PrintTransaction();
                type = PrintTransaction.IN_PHIEU_XAC_NHAN_CONG_NO;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewMobaImpMest_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            int rowHandle = gridViewTransaction1.GetVisibleRowHandle(hi.RowHandle);
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            if (checkEdit == null)
                                return;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo1 = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo1.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo1.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
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
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            if (this.VTranSaction1 != null)
                            {
                                var CountCheck = this.VTranSaction1.Where(o => o.IsCheck == true).ToList();
                                var Count = this.VTranSaction1.Count;
                                if (CountCheck != null && CountCheck.Count >= 0 && CountCheck.Count < Count)
                                {
                                    gridColumnCheck.Image = imageListIcon.Images[5];
                                    isCheckAll = true;
                                }
                                else if (CountCheck.Count == Count)
                                {
                                    gridColumnCheck.Image = imageListIcon.Images[6];
                                    isCheckAll = false;
                                }
                            }
                            if (isCheckAll == true)
                            {
                                foreach (var item in this.VTranSaction1)
                                {
                                    if (item.CANCEL_USERNAME == null)
                                        item.IsCheck = true;
                                    else
                                        item.IsCheck = true;
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                foreach (var item in this.VTranSaction1)
                                {
                                    item.IsCheck = false;
                                }
                                isCheckAll = true;
                            }
                            gridViewTransaction1.BeginUpdate();
                            gridViewTransaction1.GridControl.DataSource = this.VTranSaction1;
                            gridViewTransaction1.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewTransaction1_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (this.VTranSaction1 != null && this.VTranSaction1.Count > 0)
                {
                    bool enable = false;
                    var TranSactionCheck = this.VTranSaction1.Where(o => o.IsCheck == true && o.DEBT_BILL_ID == null && o.IS_CANCEL == null).ToList();
                    var dataGroup = TranSactionCheck.GroupBy(o => o.TREATMENT_ID).ToList();
                    //var unload = VTranSaction1.Where(o => o.CASHIER_LOGINNAME == null).ToList();

                    //if (dataGroup.Count > 0 && TranSactionCheck.Count > 0 && dataGroup.Count == TranSactionCheck.Count)
                    //{
                    //    enable = true;
                    //}
                    if (dataGroup.Count == 1)
                    {
                        enable = true;
                    }
                    btnDebt.Enabled = enable;

                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private bool CancelElectronicBill1(V_HIS_TRANSACTION_1 transaction)
        {
            bool result = true;
            try
            {
                if (transaction != null && !String.IsNullOrEmpty(transaction.INVOICE_CODE) && !String.IsNullOrEmpty(transaction.INVOICE_SYS))
                {
                    string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.ELECTRONIC_BILL__CONFIG);
                    if (transaction.INVOICE_SYS != ProviderType.VIETTEL)
                    {
                        ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                        dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(transaction.INVOICE_CODE);
                        dataInput.InvoiceCode = transaction.INVOICE_CODE;
                        dataInput.NumOrder = transaction.NUM_ORDER;
                        dataInput.SymbolCode = transaction.SYMBOL_CODE;
                        dataInput.TemplateCode = transaction.TEMPLATE_CODE;
                        dataInput.TransactionTime = transaction.TRANSACTION_TIME;
                        dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                        ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                        ElectronicBillResult electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.CANCEL_INVOICE);

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => electronicBillResult), electronicBillResult));
                        if (electronicBillResult != null && !electronicBillResult.Success)
                        {

                            string mes = "";
                            if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                            {
                                foreach (var item in electronicBillResult.Messages)
                                {
                                    mes += item + ";";
                                }
                            }
                            DialogResult myResult;
                            myResult = MessageBox.Show("Hủy hóa đơn điện tử thất bại." + mes + ". Bạn có muốn tiếp tục hủy giao dịch trên HIS?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (myResult != DialogResult.OK)
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private bool CancelElectronicBill(V_HIS_TRANSACTION transaction)
        {
            bool result = true;
            try
            {
                if (transaction != null && !String.IsNullOrEmpty(transaction.INVOICE_CODE) && !String.IsNullOrEmpty(transaction.INVOICE_SYS))
                {
                    string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.ELECTRONIC_BILL__CONFIG);
                    if (transaction.INVOICE_SYS != ProviderType.VIETTEL)
                    {
                        ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                        dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(transaction.INVOICE_CODE);
                        dataInput.InvoiceCode = transaction.INVOICE_CODE;
                        dataInput.NumOrder = transaction.NUM_ORDER;
                        dataInput.SymbolCode = transaction.SYMBOL_CODE;
                        dataInput.TemplateCode = transaction.TEMPLATE_CODE;
                        dataInput.TransactionTime = transaction.TRANSACTION_TIME;
                        dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                        ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                        ElectronicBillResult electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.CANCEL_INVOICE);

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => electronicBillResult), electronicBillResult));
                        if (electronicBillResult != null && !electronicBillResult.Success)
                        {
                            string mes = "";
                            if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                            {
                                foreach (var item in electronicBillResult.Messages)
                                {
                                    mes += item + ";";
                                }
                            }
                            DialogResult myResult;
                            myResult = MessageBox.Show("Hủy hóa đơn điện tử thất bại." + mes + ". Bạn có muốn tiếp tục hủy giao dịch trên HIS?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (myResult != DialogResult.OK)
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void gridViewTransaction1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (Transaction1ADO)gridViewTransaction1.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "DELETE_DISPLAY_STR")
                        {
                            if (data.IS_CANCEL == 1)
                            {
                                e.Appearance.ForeColor = Color.Gray; //Giao dịch đã bị hủy => Màu nâu
                                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);

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

        private void gridControlTracsaction1_Click(object sender, EventArgs e)
        {

        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    FillDataToGridTransaction1();
                    FillDataToGridTransaction();
                    WaitingManager.Hide();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtFromTime.Focus();
                    dtFromTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtFromTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtToTime.Focus();
                    dtToTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtToTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCDebtManager_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control)
                {
                    if (e.Control && e.KeyCode == Keys.F)
                    {
                        btnSearch_Click(null, null);
                    }
                    if (e.Control && e.KeyCode == Keys.T)
                    {
                        btnDebt_Click(null, null);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCDebtManager_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.F:
                            this.btnFind_Click(null, null);
                            break;
                        case Keys.T:
                            this.btnDebt_Click(null, null);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTranactionCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    FillDataToGridControlTransaction();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCDebtManager_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                FillDataToGridTransaction1();
                FillDataToGridTransaction();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnDebt_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnDebt_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
