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
using ACS.EFMODEL.DataModels;
using Core.ServiceCombo;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PharmacyCashier.ADO;
using HIS.Desktop.Plugins.PharmacyCashier.Config;
using HIS.Desktop.Plugins.PharmacyCashier.Util;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PharmacyCashier
{
    public partial class frmPharmacyCashier : FormBase
    {

        internal static void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboAge()
        {
            try
            {
                InitComboCommon(this.cboAge, BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO>(), "Id", "MoTa", 0, "", 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboGender()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboPatientGender, BackendDataWorker.Get<HIS_GENDER>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMediStockByModuleData()
        {
            try
            {
                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboCashierRoom()
        {
            try
            {
                long branchId;
                branchId = WorkPlace.WorkPlaceSDO.FirstOrDefault().BranchId;
                var userRoomIds = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()
                    && o.BRANCH_ID == branchId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN).Select(s => s.ROOM_ID).ToList();

                listCashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().Where(o => userRoomIds.Contains(o.ROOM_ID) && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CASHIER_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCashierRoom, listCashierRoom, controlEditorADO);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboPayForm()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPayForm, BackendDataWorker.Get<HIS_PAY_FORM>(), controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultCashierRoom()
        {
            try
            {
                if (cboCashierRoom.EditValue == null)
                {
                    var data = listCashierRoom != null ? listCashierRoom.FirstOrDefault() : null;
                    if (data != null)
                    {
                        cboCashierRoom.EditValue = data.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultPayForm()
        {
            try
            {
                if (cboPayForm.EditValue == null)
                {
                    string code = String.IsNullOrEmpty(ConfigApplicationWorker.Get<string>(HisConfigCFG.HFS_KEY__PAY_FORM_CODE)) ? GlobalVariables.HIS_PAY_FORM_CODE__CONSTANT : ConfigApplicationWorker.Get<string>(HisConfigCFG.HFS_KEY__PAY_FORM_CODE);
                    var data = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.PAY_FORM_CODE == code);
                    if (data != null)
                    {
                        txtPayFormCode.Text = data.PAY_FORM_CODE;
                        cboPayForm.EditValue = data.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultAccountBook()
        {
            try
            {
                if (cboReceiptAccountBook.EditValue == null || !listAccountBook.Any(a => a.ID == Convert.ToInt64(cboReceiptAccountBook.EditValue)))
                {
                    var data = listAccountBook.Where(o => o.BILL_TYPE_ID == 1).FirstOrDefault();
                    if (data != null)
                    {
                        txtReceiptAccountBookCode.Text = data.ACCOUNT_BOOK_CODE;
                        cboReceiptAccountBook.EditValue = data.ID;
                    }
                }
                else
                {
                    var data = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboReceiptAccountBook.EditValue));
                    if (data != null)
                    {
                        SetDataToDicNumOrderInAccountBook(data, spinReceiptNumOrder);
                    }
                }
                if (cboInvoicePresAccountBook.EditValue == null || !listAccountBook.Any(a => a.ID == Convert.ToInt64(cboInvoicePresAccountBook.EditValue)))
                {
                    var data = listAccountBook.Where(o => o.BILL_TYPE_ID == 2).FirstOrDefault();
                    if (data != null)
                    {
                        txtInvoicePresAccountBookCode.Text = data.ACCOUNT_BOOK_CODE;
                        cboInvoicePresAccountBook.EditValue = data.ID;
                    }
                }
                else
                {
                    var data = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoicePresAccountBook.EditValue));
                    if (data != null)
                    {
                        SetDataToDicNumOrderInAccountBook(data, spinInvoicePresNumOrder);
                    }
                }
                if (cboInvoiceServiceAccountBook.EditValue == null || !listAccountBook.Any(a => a.ID == Convert.ToInt64(cboInvoiceServiceAccountBook.EditValue)))
                {
                    var data = listAccountBook.Where(o => o.BILL_TYPE_ID == 2).FirstOrDefault();
                    if (data != null)
                    {
                        txtInvoiceServiceAccountBookCode.Text = data.ACCOUNT_BOOK_CODE;
                        cboInvoiceServiceAccountBook.EditValue = data.ID;
                    }
                }
                else
                {
                    var data = this.listAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoiceServiceAccountBook.EditValue));
                    if (data != null)
                    {
                        SetDataToDicNumOrderInAccountBook(data, spinInvoiceServiceNumOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultPatientType(bool isHold = false)
        {
            try
            {
                List<HIS_PATIENT_TYPE> sources = cboPatientType.Properties.DataSource as List<HIS_PATIENT_TYPE>;
                if (sources != null && sources.Count > 0)
                {
                    if (this.treatment != null && sources.Any(a => a.ID == this.treatment.TDL_PATIENT_TYPE_ID))
                    {
                        cboPatientType.EditValue = this.treatment.TDL_PATIENT_TYPE_ID;
                    }
                    else if (cboPatientType.EditValue == null)
                    {
                        HIS_PATIENT_TYPE data = sources.FirstOrDefault(o => o.ID == HisConfigCFG.PATIENT_TYPE_ID__IS_FEE);
                        if (data == null)
                        {
                            data = sources.FirstOrDefault();
                        }
                        cboPatientType.EditValue = data.ID;
                    }
                }

                sources = cboServicePatientType.Properties.DataSource as List<HIS_PATIENT_TYPE>;
                if (sources != null && sources.Count > 0)
                {
                    if (this.treatment != null && !isHold && sources.Any(a => a.ID == this.treatment.TDL_PATIENT_TYPE_ID))
                    {
                        if (cboServicePatientType.EditValue == null || Convert.ToInt64(cboServicePatientType.EditValue) != this.treatment.TDL_PATIENT_TYPE_ID)
                            cboServicePatientType.EditValue = this.treatment.TDL_PATIENT_TYPE_ID;
                    }
                    else
                    {
                        cboServicePatientType.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboAccountBook()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (String.IsNullOrWhiteSpace(loginName))
                {
                    MessageBox.Show("Không thanh toán được, mời bạn chọn lại");
                    return;
                }
                this.listAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
                List<long> ids = new List<long>();
                HisAccountBookViewFilter accBookFilter = new HisAccountBookViewFilter();
                accBookFilter.LOGINNAME = loginName;
                accBookFilter.IS_OUT_OF_BILL = false;
                accBookFilter.FOR_BILL = true;
                accBookFilter.ORDER_DIRECTION = "DESC";
                accBookFilter.ORDER_FIELD = "ID";
                accBookFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                if (cboCashierRoom.EditValue != null)
                {
                    accBookFilter.CASHIER_ROOM_ID = Convert.ToInt64(cboCashierRoom.EditValue);
                }

                listAccountBook = new BackendAdapter(new CommonParam()).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, accBookFilter, null);
                var listReceipts = this.listAccountBook != null ? this.listAccountBook.Where(o => o.BILL_TYPE_ID == 1).ToList() : null;
                var listInvoices = this.listAccountBook != null ? this.listAccountBook.Where(o => o.BILL_TYPE_ID == 2).ToList() : null;

                List<ColumnInfo> columnInfoReceipts = new List<ColumnInfo>();
                columnInfoReceipts.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfoReceipts.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorReceiptADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfoReceipts, false, 350);
                ControlEditorLoader.Load(cboReceiptAccountBook, listReceipts, controlEditorReceiptADO);

                List<ColumnInfo> columnInfoInvoicePress = new List<ColumnInfo>();
                columnInfoInvoicePress.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfoInvoicePress.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorInvoicePresADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfoInvoicePress, false, 350);
                ControlEditorLoader.Load(cboInvoicePresAccountBook, listInvoices, controlEditorInvoicePresADO);

                List<ColumnInfo> columnInfoInvoiceServs = new List<ColumnInfo>();
                columnInfoInvoiceServs.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfoInvoiceServs.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorServADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfoInvoiceServs, false, 350);
                ControlEditorLoader.Load(cboInvoiceServiceAccountBook, listInvoices, controlEditorServADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboPatientType()
        {
            try
            {
                //thì chỉ hiển thị ra các đối tượng có trong chính sách giá/vật tư (medicine_paty và material_paty)
                //--> Khi đó, với danh mục thuốc/vật tư của nhà thuốc, người dùng chỉ khai báo chính sách giá cho đối tượng mua thuốc
                List<long> patientTypeIds = new List<long>();
                List<HIS_MEDICINE_PATY> medicinePatys = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_PATY>();
                if (medicinePatys != null && medicinePatys.Count > 0)
                {
                    patientTypeIds.AddRange(medicinePatys.Select(o => o.PATIENT_TYPE_ID));
                }

                List<HIS_MATERIAL_PATY> materialPatys = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_PATY>();
                if (materialPatys != null && materialPatys.Count > 0)
                {
                    patientTypeIds.AddRange(materialPatys.Select(o => o.PATIENT_TYPE_ID));
                }
                //Bỏ trùng
                patientTypeIds = patientTypeIds.Where(o => o != HisConfigCFG.PatientTypeId__BHYT).Distinct().ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 120, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 270);
                ControlEditorLoader.Load(cboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => patientTypeIds.Contains(o.ID)).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboSerivcePatientType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 120, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 270);
                ControlEditorLoader.Load(cboServicePatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BuildInStockContainer()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.RebuildMediMatyInStockContainer(); }));
                }
                else
                {
                    this.RebuildMediMatyInStockContainer();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadMetyMatyInStock()
        {
            try
            {
                this.listInStockADOs = new List<ADO.MetyMatyInStockADO>();
                HisMedicineTypeStockViewFilter medicineFilter = new HisMedicineTypeStockViewFilter();
                medicineFilter.MEDI_STOCK_ID = mediStock.ID;
                medicineFilter.IS_AVAILABLE = true;
                medicineFilter.IS_ACTIVE = true;
                List<HisMedicineTypeInStockSDO> mediInStocks = new BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>("api/HisMedicineType/GetInStockMedicineType", ApiConsumers.MosConsumer, medicineFilter, null);

                HisMaterialTypeStockViewFilter mateFilter = new HisMaterialTypeStockViewFilter();
                mateFilter.MEDI_STOCK_ID = mediStock.ID;
                mateFilter.IS_AVAILABLE = true;
                mateFilter.IS_ACTIVE = true;
                List<HisMaterialTypeInStockSDO> mateInStocks = new BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>("api/HisMaterialType/GetInStockMaterialType", ApiConsumers.MosConsumer, mateFilter, null);

                if (mediInStocks != null && mediInStocks.Count > 0)
                {
                    foreach (var sdo in mediInStocks)
                    {
                        if (sdo.IsBusiness != 1) continue;
                        MetyMatyInStockADO ado = new MetyMatyInStockADO(sdo);
                        listInStockADOs.Add(ado);
                    }
                }

                if (mateInStocks != null && mateInStocks.Count > 0)
                {
                    foreach (var sdo in mateInStocks)
                    {
                        if (sdo.IsBusiness != 1) continue;
                        MetyMatyInStockADO ado = new MetyMatyInStockADO(sdo);
                        listInStockADOs.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task RebuildMediMatyInStockContainer()
        {
            try
            {
                await this.LoadMetyMatyInStock();

                gridViewInStockMetyMaty.BeginUpdate();
                gridViewInStockMetyMaty.Columns.Clear();
                popupControlContainerInStock.Width = theRequiredWidth;
                popupControlContainerInStock.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MedicineTypeCode";
                col1.Caption = "Mã";
                col1.Width = 80;
                col1.VisibleIndex = 1;
                gridViewInStockMetyMaty.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MedicineTypeName";
                col2.Caption = "Tên thuốc/vật tư";
                col2.Width = 250;
                col2.VisibleIndex = 2;
                gridViewInStockMetyMaty.Columns.Add(col2);


                DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                col3.FieldName = "AvailableAmount";
                col3.Caption = "Khả dụng";
                col3.Width = 90;
                col3.VisibleIndex = 3;
                col3.DisplayFormat.FormatString = "#,##0.00";
                col3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                gridViewInStockMetyMaty.Columns.Add(col3);

                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "ServiceUnitName";
                col4.Caption = "Đơn vị tính";
                col4.Width = 60;
                col4.VisibleIndex = 4;
                gridViewInStockMetyMaty.Columns.Add(col4);


                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "Concentra";
                col5.Caption = "Hàm lượng";
                col5.Width = 100;
                col5.VisibleIndex = 5;
                gridViewInStockMetyMaty.Columns.Add(col5);


                DevExpress.XtraGrid.Columns.GridColumn col6 = new DevExpress.XtraGrid.Columns.GridColumn();
                col6.FieldName = "ActiveIngrBhytName";
                col6.Caption = "Hoạt chất";
                col6.Width = 160;
                col6.VisibleIndex = 6;
                gridViewInStockMetyMaty.Columns.Add(col6);


                //DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                //col7.FieldName = "IMP_PRICE_DISPLAY";
                //col7.Caption = "Giá nhập";
                //col7.Width = 100;
                //col7.VisibleIndex = 7;
                //col7.DisplayFormat.FormatString = "#,##0.00";
                //col7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                //col7.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                //gridViewInStockMetyMaty.Columns.Add(col7);


                //Phuc vu cho tim kiem khong dau

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "MedicineTypeCodeUnsign";
                col12.Width = 80;
                col12.VisibleIndex = -1;
                gridViewInStockMetyMaty.Columns.Add(col12);

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "MedicineTypeNameUnsign";
                col13.Width = 80;
                col13.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col13.VisibleIndex = -1;
                gridViewInStockMetyMaty.Columns.Add(col13);

                DevExpress.XtraGrid.Columns.GridColumn col14 = new DevExpress.XtraGrid.Columns.GridColumn();
                col14.FieldName = "ActiveIngrBhytNameUnsign";
                col14.VisibleIndex = -1;
                col14.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewInStockMetyMaty.Columns.Add(col14);

                gridViewInStockMetyMaty.GridControl.DataSource = this.listInStockADOs;
                gridViewInStockMetyMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewInStockMetyMaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDictionaryNumOrderAccountBook(long accountBookId, SpinEdit spinNumOrder)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBookId))
                {
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBookId] = spinNumOrder.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDictionaryNumOrderAccountBook(long accountBookId, long numOrder)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBookId))
                {
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBookId] = numOrder;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook, SpinEdit spinNumOrder)
        {
            try
            {
                bool enable = (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1);

                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                {
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                    {
                        HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
                    }

                    CommonParam param = new CommonParam();
                    MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new HisAccountBookViewFilter();
                    hisAccountBookViewFilter.ID = accountBook.ID;
                    var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                    if (accountBooks != null && accountBooks.Count > 0)
                    {
                        var accountBookNew = accountBooks.FirstOrDefault();
                        decimal num = 0;
                        if ((accountBookNew.CURRENT_NUM_ORDER ?? 0) > 0)
                        {
                            num = accountBookNew.CURRENT_NUM_ORDER ?? 0;
                        }
                        else
                        {
                            num = (decimal)accountBookNew.FROM_NUM_ORDER - 1;
                        }
                        HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, num);
                        spinNumOrder.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                    }
                }
                else
                {
                    spinNumOrder.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                }

                if (enable)
                {
                    spinNumOrder.Properties.Buttons[0].Visible = true;
                }
                else
                {
                    spinNumOrder.Properties.Buttons[0].Visible = false;
                }
                if (spinNumOrder == spinReceiptNumOrder)
                {
                    this.enableSpinReceipt = enable;
                }
                else if (spinNumOrder == spinInvoicePresNumOrder)
                {
                    this.enableSpinInvoicePres = enable;
                }
                else if (spinNumOrder == spinInvoiceServiceNumOrder)
                {
                    this.enableSpinInvoiceService = enable;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSereServ()
        {
            try
            {
                listSereServAdo = new List<SereServADO>();
                //if ((serviceReq != null || (serviceReqByTreatmentList != null && serviceReqByTreatmentList.Count > 0)) && !this.isNotAllowPres)//nambg
                if ((serviceReq != null || (serviceReqByTreatmentList != null && serviceReqByTreatmentList.Count > 0)))
                {
                    Dictionary<long, List<HIS_SERE_SERV_BILL>> dicSereServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();

                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    if (serviceReq != null)
                    {
                        ssBillFilter.TDL_TREATMENT_ID = serviceReq.TREATMENT_ID;
                    }
                    else if (serviceReqByTreatmentList != null && serviceReqByTreatmentList.Count > 0)
                    {
                        ssBillFilter.TDL_TREATMENT_ID = serviceReqByTreatmentList.FirstOrDefault().TREATMENT_ID;
                    }

                    ssBillFilter.IS_NOT_CANCEL = true;
                    var listSSBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                    if (listSSBill != null && listSSBill.Count > 0)
                    {
                        foreach (var item in listSSBill)
                        {
                            if (item.IS_CANCEL == ConstantUtil.IS_TRUE)
                                continue;
                            if (!dicSereServBill.ContainsKey(item.SERE_SERV_ID))
                                dicSereServBill[item.SERE_SERV_ID] = new List<HIS_SERE_SERV_BILL>();
                            dicSereServBill[item.SERE_SERV_ID].Add(item);
                        }
                    }

                    HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                    if (serviceReq != null)
                    {
                        ssFilter.TDL_TREATMENT_ID = serviceReq.TREATMENT_ID;
                    }
                    else if (serviceReqByTreatmentList != null && serviceReqByTreatmentList.Count > 0)
                    {
                        ssFilter.TDL_TREATMENT_ID = serviceReqByTreatmentList.FirstOrDefault().TREATMENT_ID;
                    }

                    //ssFilter.TDL_SERVICE_REQ_TYPE_IDs = new List<long>{IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA,
                    //    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G,
                    //    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL,
                    //    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH,
                    //    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC,
                    //    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS,
                    //    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN,
                    //    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT,
                    //    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA,
                    //    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN,
                    //    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT,
                    //    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                    //    };

                    List<V_HIS_SERE_SERV_5> sereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);

                    if (sereServs != null && sereServs.Count > 0)
                    {
                        foreach (var item in sereServs)
                        {
                            if (item.IS_NO_PAY == ConstantUtil.IS_TRUE || item.VIR_TOTAL_PATIENT_PRICE <= 0 || item.IS_NO_EXECUTE == ConstantUtil.IS_TRUE)
                                continue;
                            if (dicSereServBill.ContainsKey(item.ID)) continue;
                            SereServADO ado = new SereServADO(item);
                            listSereServAdo.Add(ado);
                        }
                    }
                }
                listSereServAdo = listSereServAdo.OrderBy(o => o.TDL_SERVICE_CODE).ToList();
                if (listSereServAdo.Count > 0)
                {
                    gridColSereServ_Select.Image = imageListIcon.Images[5];
                }
                SetDataSourceTreeList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSourceTreeList()
        {
            try
            {
                gridControlSereServ.BeginUpdate();
                gridControlSereServ.DataSource = this.listSereServAdo;
                gridControlSereServ.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboUser()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.InitComboUser(); }));
                }
                else
                {
                    this.InitComboUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitComboUser()
        {
            try
            {
                List<ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS_USER>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboPresUser, datas, controlEditorADO);
                this.SetDefaultPresUser();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultPresUser()
        {
            try
            {
                if (this.serviceReq != null)
                {
                    this.cboPresUser.EditValue = this.serviceReq.REQUEST_LOGINNAME;
                    this.txtPresLoginname.Text = this.serviceReq.REQUEST_LOGINNAME;
                }
                else
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var data = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == 1 && o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).FirstOrDefault();
                    if (data != null)
                    {
                        this.cboPresUser.EditValue = data.LOGINNAME;
                        this.txtPresLoginname.Text = data.LOGINNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GenerateMenuPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                menu.Items.Add(new DXMenuItem("Phiếu xuất bán thuốc", new EventHandler(onClickInPhieuXuatBan)));
                menu.Items.Add(new DXMenuItem("Hóa đơn dịch vụ", new EventHandler(onClickInHoaDonDichVu)));
                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BuildServiceContainer()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.RebuildAssignServiceContainer(); }));
                }
                else
                {
                    this.RebuildAssignServiceContainer();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAssignServiceAdo()
        {
            try
            {
                this.listAsignServiceAdo = new List<LocalStorage.BackendData.ADO.ServiceADO>();

                if (cboServicePatientType.EditValue != null && this.treatment != null)
                {
                    long patientTypeId = Convert.ToInt64(cboServicePatientType.EditValue);
                    long treatmentTime = treatment.IN_TIME;
                    long intructionTime = 0;
                    if (dtIntructionTime.EditValue != null && dtIntructionTime.DateTime != DateTime.MinValue)
                    {
                        intructionTime = Convert.ToInt64(dtIntructionTime.DateTime.ToString("yyyyMMddHHmmss"));
                    }

                    List<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO> services = ServiceByPatientTypeDataWorker.GetByPatientType(patientTypeId);
                    if (services != null && services.Count > 0)
                    {
                        Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs = null;
                        if (dicServicePaty.ContainsKey(patientTypeId))
                        {
                            servicePatyInBranchs = dicServicePaty[patientTypeId];
                        }
                        else
                        {
                            servicePatyInBranchs = BackendDataWorker.Get<V_HIS_SERVICE_PATY>()
                                .Where(o =>
                               o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                               && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()
                               && serviceTypeIdAllows.Contains(o.SERVICE_TYPE_ID)
                               && o.PATIENT_TYPE_ID == patientTypeId)
                              .GroupBy(o => o.SERVICE_ID)
                              .ToDictionary(o => o.Key, o => o.ToList());
                            dicServicePaty[patientTypeId] = servicePatyInBranchs;
                        }

                        foreach (var item in services)
                        {

                            item.PRICE = null;
                            item.VAT_RATIO = null;
                            item.PRICE_VAT = null;
                            if (!servicePatyInBranchs.ContainsKey(item.ID)) continue;
                            var sPatys = servicePatyInBranchs[item.ID];
                            V_HIS_SERVICE_PATY paty = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePatyInBranchs[item.ID], BranchDataWorker.GetCurrentBranchId(), null, null, null, intructionTime, treatmentTime, item.ID, patientTypeId, null);
                            if (paty != null)
                            {
                                if (item.ID == 3442 && patientTypeId == 1)
                                {
                                    LogSystem.Info(LogUtil.TraceData("Paty", paty));
                                }
                                item.PRICE = paty.PRICE;
                                item.VAT_RATIO = paty.VAT_RATIO;
                                item.PRICE_VAT = paty.PRICE * (1 + paty.VAT_RATIO);
                                this.listAsignServiceAdo.Add(item);
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

        private async Task RebuildAssignServiceContainer()
        {
            try
            {
                await this.LoadAssignServiceAdo();

                gridViewAssignService.BeginUpdate();
                gridViewAssignService.Columns.Clear();
                popupControlContainerService.Width = theRequiredWidth;
                popupControlContainerService.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "SERVICE_CODE";
                col1.Caption = "Mã";
                col1.Width = 80;
                col1.VisibleIndex = 1;
                gridViewAssignService.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "SERVICE_NAME";
                col2.Caption = "Dịch vụ";
                col2.Width = 250;
                col2.VisibleIndex = 2;
                gridViewAssignService.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                col3.FieldName = "SERVICE_TYPE_NAME";
                col3.Caption = "Loại dịch vụ";
                col3.Width = 70;
                col3.VisibleIndex = 3;
                gridViewAssignService.Columns.Add(col3);


                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "PRICE_VAT";
                col4.Caption = "Giá";
                col4.Width = 90;
                col4.VisibleIndex = 4;
                col4.DisplayFormat.FormatString = "#,##0.00";
                col4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                gridViewAssignService.Columns.Add(col4);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "SERVICE_UNIT_NAME";
                col5.Caption = "Đơn vị tính";
                col5.Width = 60;
                col5.VisibleIndex = 5;
                gridViewAssignService.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "SERVICE_CODE_FOR_SEARCH";
                col13.Width = 80;
                col13.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col13.VisibleIndex = -1;
                gridViewAssignService.Columns.Add(col13);

                DevExpress.XtraGrid.Columns.GridColumn col14 = new DevExpress.XtraGrid.Columns.GridColumn();
                col14.FieldName = "SERVICE_NAME_FOR_SEARCH";
                col14.VisibleIndex = -1;
                col14.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewAssignService.Columns.Add(col14);

                gridViewAssignService.GridControl.DataSource = this.listAsignServiceAdo;
                gridViewAssignService.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewAssignService.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
