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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {

        private async Task InitComboNhaThuoc()
        {
            try
            {
                List<V_HIS_MEDI_STOCK> mediStocks = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                mediStockAllows = mediStocks != null ? mediStocks.Where(o => o.IS_ACTIVE == 1 && o.IS_DRUG_STORE == 1 && o.IS_BUSINESS == 1 && o.IS_TRADITIONAL_MEDICINE == 1).ToList() : new List<V_HIS_MEDI_STOCK>();

                if (HisConfigCFG.IsAutoCreateSaleExpMest || HisConfigCFG.DrugStoreComboboxOption)
                {
                    InitializeComboNhaThuocNoCheck(mediStockAllows);

                    if (this.actionType == GlobalVariables.ActionEdit)
                    {
                        cboNhaThuoc.Enabled = false;
                    }
                    else 
                    {
                        cboNhaThuoc.Enabled = true;
                    }
                }
                else
                {
                    //    //- Khi key cấu hình "MOS.HIS_SERVICE_REQ.IS_AUTO_CREATE_SALE_EXP_MEST" ko được bật (giá trị khác 1):
                    //    //+ Sửa phần combobox "Nhà thuốc" hiển thị dưới dạng listcheckbox, cho phép chọn nhiều nhà thuốc
                    //    //+ Dữ liệu tồn hiển thị theo từng thuốc/vật tư và giá bán (last_exp_price * (1 + last_exp_vat_ratio)), chứ không tách theo kho.
                    //    //#22821
                    InitializeComboNhaThuocHasCheck(mediStockAllows);
                    //    if (this.currentMediStockNhaThuocSelecteds != null && this.currentMediStockNhaThuocSelecteds.Count > 0)
                    //    {
                    //        this.cboNhaThuoc.ShowPopup();
                    //        this.cboNhaThuoc.ClosePopup();
                    //    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitGridCheckMarkNhaThuocCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(this.cboNhaThuoc.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__NhaThuoc);
                this.cboNhaThuoc.Properties.Tag = gridCheck;
                this.cboNhaThuoc.Properties.View.OptionsSelection.MultiSelect = true;
                this.cboNhaThuoc.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboNhaThuoc_CustomDisplayText);
                this.cboNhaThuoc.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboNhaThuoc_Closed);
                GridCheckMarksSelection gridCheckMark = this.cboNhaThuoc.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(this.cboNhaThuoc.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__NhaThuoc(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                currentMediStockNhaThuocSelecteds = new List<V_HIS_MEDI_STOCK>();
                foreach (MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.MEDI_STOCK_NAME.ToString());
                        currentMediStockNhaThuocSelecteds.Add(rv);
                    }
                }
                this.cboNhaThuoc.Text = sb.ToString();
                Inventec.Common.Logging.LogSystem.Debug("SelectionGrid__NhaThuoc" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStockNhaThuocSelecteds), currentMediStockNhaThuocSelecteds));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNhaThuoc_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    WaitingManager.Show();

                    if (this.currentMediStockNhaThuocSelecteds != null && this.currentMediStockNhaThuocSelecteds.Count > 0)
                        RebuildNhaThuocMediMatyWithInControlContainer();
                    else
                        RebuildMedicineTypeWithInControlContainer();

                    Inventec.Common.Logging.LogSystem.Debug("cboNhaThuoc_Closed. 2" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentMediStockNhaThuocSelecteds), this.currentMediStockNhaThuocSelecteds));
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNhaThuoc_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.MEDI_STOCK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboNhaThuocHasCheck(List<V_HIS_MEDI_STOCK> serviceTypes)
        {
            try
            {
                var datas = serviceTypes;
                if (datas != null)
                {
                    cboNhaThuoc.Properties.DataSource = datas;
                    cboNhaThuoc.Properties.DisplayMember = "MEDI_STOCK_NAME";
                    cboNhaThuoc.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboNhaThuoc.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    cboNhaThuoc.Properties.PopupFormWidth = 200;
                    cboNhaThuoc.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboNhaThuoc.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboNhaThuoc.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboNhaThuoc.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitializeComboNhaThuocHasCheck(List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> listMediStock)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitializeComboNhaThuocHasCheck.1");
                if (cboNhaThuoc.Properties.View.Columns.Count <= 0)
                {
                    InitGridCheckMarkNhaThuocCheck();
                    Inventec.Common.Logging.LogSystem.Debug("InitializeComboNhaThuocHasCheck.2");
                    InitComboNhaThuocHasCheck(listMediStock);
                }
                else
                {
                    cboNhaThuoc.Properties.DataSource = listMediStock;
                }
                Inventec.Common.Logging.LogSystem.Debug("InitializeComboNhaThuocHasCheck.3");

                List<long> medistockIdDefaults = new List<long>();
                if (this.oldServiceReq != null && this.oldServiceReq.EXECUTE_ROOM_ID > 0)
                {
                    var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == this.oldServiceReq.EXECUTE_ROOM_ID).FirstOrDefault();

                    if (mediStock != null && mediStock.IS_BUSINESS == GlobalVariables.CommonNumberTrue)
                    {
                        medistockIdDefaults.Add(mediStock.ID);
                    }
                }

                if ((medistockIdDefaults == null || medistockIdDefaults.Count <= 0) && !String.IsNullOrEmpty(this.requestRoom.DEFAULT_DRUG_STORE_IDS) && listMediStock != null && listMediStock.Count > 0)
                {
                    var split = this.requestRoom.DEFAULT_DRUG_STORE_IDS.Split(',').ToList();
                    foreach (var item in split)
                    {
                        medistockIdDefaults.Add(long.Parse(item));
                    }
                }

                List<V_HIS_MEDI_STOCK> defaultDrugStore = (medistockIdDefaults != null && medistockIdDefaults.Count > 0) ? listMediStock.Where(o => medistockIdDefaults.Contains(o.ID)).ToList() : null;

                if (defaultDrugStore != null && defaultDrugStore.Count > 0)
                {
                    if (this.currentMediStockNhaThuocSelecteds == null)
                        this.currentMediStockNhaThuocSelecteds = new List<V_HIS_MEDI_STOCK>();

                    this.currentMediStockNhaThuocSelecteds.AddRange(defaultDrugStore);
                    GridCheckMarksSelection gridCheckMark = cboNhaThuoc.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        //var data = listMediStock.Where(o => o.ID == defaultDrugStore.ID).ToList();
                        gridCheckMark.SelectAll(defaultDrugStore);
                        Inventec.Common.Logging.LogSystem.Debug("InitializeComboNhaThuocHasCheck.4");
                    }
                    //else
                    //{
                    //    cboNhaThuoc.EditValue = defaultDrugStore.ID;
                    Inventec.Common.Logging.LogSystem.Debug("InitializeComboNhaThuocHasCheck.5");
                    //}
                }
                Inventec.Common.Logging.LogSystem.Debug("InitializeComboNhaThuocHasCheck.6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitializeComboNhaThuocNoCheck(List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> mediStockAllows)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitializeComboNhaThuocNoCheck.1____"+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediStockAllows), mediStockAllows));
                if (cboNhaThuoc.Properties.View.Columns.Count <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("InitializeComboNhaThuocNoCheck.2");
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(cboNhaThuoc, mediStockAllows, controlEditorADO);
                    Inventec.Common.Logging.LogSystem.Debug("InitializeComboNhaThuocNoCheck.3");
                }
                else
                {
                    cboNhaThuoc.Properties.DataSource = mediStockAllows;
                    Inventec.Common.Logging.LogSystem.Debug("InitializeComboNhaThuocNoCheck.4");
                }

                long medistockIdDefault = 0;
                if (this.oldServiceReq != null && this.oldServiceReq.EXECUTE_ROOM_ID > 0)
                {
                    var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == this.oldServiceReq.EXECUTE_ROOM_ID).FirstOrDefault();

                    if (mediStock != null && mediStock.IS_BUSINESS == GlobalVariables.CommonNumberTrue)
                    {
                        medistockIdDefault = mediStock.ID;
                    }
                }

                //if (medistockIdDefault == 0 && this.requestRoom.DEFAULT_DRUG_STORE_ID > 0 && mediStockAllows != null && mediStockAllows.Count > 0)
                //{
                //    medistockIdDefault = this.requestRoom.DEFAULT_DRUG_STORE_ID.Value;
                //}

                V_HIS_MEDI_STOCK defaultDrugStore = medistockIdDefault > 0 ? mediStockAllows.Where(o => o.ID == medistockIdDefault).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medistockIdDefault), medistockIdDefault) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => defaultDrugStore), defaultDrugStore));
                if (defaultDrugStore != null)
                {
                    if (this.currentMediStockNhaThuocSelecteds == null)
                        this.currentMediStockNhaThuocSelecteds = new List<V_HIS_MEDI_STOCK>();
                    this.currentMediStockNhaThuocSelecteds.Add(defaultDrugStore);

                    cboNhaThuoc.EditValue = defaultDrugStore.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientType(GridLookUpEdit cboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTracking(GridLookUpEdit cbo)
        {
            try
            {
                cboPhieuDieuTri.Properties.Buttons[1].Visible = false;
                if (trackingADOs == null)
                    return;

                if (cbo.EditValue == null)
                {
                    if (this.actionType == GlobalVariables.ActionEdit)
                    {
                        cbo.EditValue = null;
                        cboPhieuDieuTri.Properties.Buttons[1].Visible = false;
                    }
                    //else
                    //{
                    //    //Neu la nhieu ngay thi khong gan gia tri mac dinh
                    //    if (this.ucDateProcessor.GetChkMultiDateState(this.ucDate) == false)
                    //    {
                    //        cbo.EditValue = trackingADOs[0].ID; //Set mac dinh cai dau tien
                    //        cboPhieuDieuTri.Properties.Buttons[1].Visible = true;
                    //    }
                    //}
                }
                else
                {
                    if (this.ucDateProcessor.GetChkMultiDateState(this.ucDate))
                    {
                        cbo.EditValue = null;
                        cboPhieuDieuTri.Properties.Buttons[1].Visible = false;
                    }
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TrackingTimeStr", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TrackingTimeStr", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, trackingADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRepositoryPatientType(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemcboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(repositoryItemcboPatientType, data, controlEditorADO);
                }
                else
                    ControlEditorLoader.Load(repositoryItemcboPatientType, currentPatientTypeWithPatientTypeAlter, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRepositoryEquipmentSet(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemcbo, List<MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EQUIPMENT_SET_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EQUIPMENT_SET_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(repositoryItemcbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExpMestTemplate()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_TEMPLATE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_TEMPLATE_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_TEMPLATE_NAME", "ID", columnInfos, false, 400);
                var expMestTemplates = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                expMestTemplates = expMestTemplates.Where(o => (o.CREATOR == loginName || (o.IS_PUBLIC ?? -1) == GlobalVariables.CommonNumberTrue)
                    && o.IS_ACTIVE == GlobalVariables.CommonNumberTrue).ToList();
                ControlEditorLoader.Load(cboExpMestTemplate, expMestTemplates, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Load người chỉ định
        private void InitComboUser()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);

                List<ACS.EFMODEL.DataModels.ACS_USER> datas = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                List<HIS_EMPLOYEE> employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>();
                if (HisConfigCFG.UserMustHaveDiploma)
                {
                    var loginnames = employees.Where(o => !string.IsNullOrEmpty(o.DIPLOMA)).Select(o => o.LOGINNAME).ToList();
                    datas = datas != null ? datas.Where(o => loginnames.Contains(o.LOGINNAME)).ToList() : null;
                }

                if (HisConfigCFG.IsShowingInTheSameDepartment)
                {
                    var loginnames = employees.Where(o => o.DEPARTMENT_ID == this.currentWorkPlace.DepartmentId).Select(o => o.LOGINNAME).ToList();
                    datas = datas != null ? datas.Where(o => loginnames.Contains(o.LOGINNAME)).ToList() : null;
                }

                ControlEditorLoader.Load(cboUser, datas, controlEditorADO);

                var data = datas != null ? datas.Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).FirstOrDefault() : null;
                if (data != null)
                {
                    this.cboUser.EditValue = data.LOGINNAME;
                    this.txtLoginName.Text = data.LOGINNAME;
                }

                //Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
                //- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn
                //- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
                this.cboUser.Enabled = (HisConfigCFG.ShowRequestUser == GlobalVariables.CommonStringTrue);
                this.txtLoginName.Enabled = (HisConfigCFG.ShowRequestUser == GlobalVariables.CommonStringTrue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExecuteRoom(DevExpress.XtraEditors.GridLookUpEdit excuteRoomCombo, List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(excuteRoomCombo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Đường dùng thuốc
        private void InitComboMedicineUseForm(object control, List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 250);
                if (data != null)
                {
                    data = data.OrderBy(o => o.NUM_ORDER).ToList();
                }
                else
                    data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().OrderBy(o => o.NUM_ORDER).ToList();
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMediStockComboByConfig()
        {
            try
            {
                this.cboMediStockExport.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboMediStockExport_CustomDisplayText);

                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboMediStockExport.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(cboMediStockExport__SelectionChange);
                if (cboMediStockExport.Properties.Tag == null)
                    cboMediStockExport.Properties.Tag = gridCheck;

                //cboMediStockExport.Properties.View.OptionsSelection.MultiSelect = true;
                //GridCheckMarksSelection gridCheckMark = cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //{
                //    gridCheckMark.ClearSelection(cboMediStockExport.Properties.View);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gọi api load danh sách kho
        /// Lọc theo các cấu hình
        /// </summary>
        /// <param name="patienTypeId"></param>
        private void InitComboMediStockAllow(long patienTypeId)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> mestRoomTemps = new List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>();
                List<V_HIS_MEST_ROOM> mestRooms = new List<V_HIS_MEST_ROOM>();
                var ExecuteRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => o.ROOM_ID == (this.currentModule != null ? this.currentModule.RoomId : 0) && o.IS_EXAM == 1).FirstOrDefault();
                if (HisConfigCFG.MestRoomOption == 1 && ExecuteRoom != null)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqFilter ReqFilter = new HisServiceReqFilter();
                    ReqFilter.TREATMENT_ID = treatmentId;
                    ReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                    ReqFilter.IS_MAIN_EXAM = true;

                    var ServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, ReqFilter, param).FirstOrDefault();

                    if (ServiceReq != null)
                    {
                        mestRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == ServiceReq.EXECUTE_ROOM_ID).ToList();
                    }
                }
                else
                {

                    mestRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == GetRoomId()).ToList();
                }
                List<long> mediStockId__Actives = new List<long>();

                if (this.chkYhct.Checked)
                {
                    mediStockId__Actives = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o =>
                    o.IS_ACTIVE == null
                    || o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.IS_TRADITIONAL_MEDICINE == 1 ).Select(o => o.ID).ToList();

                }
                else 
                {
                    mediStockId__Actives = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o =>
                   o.IS_ACTIVE == null
                   || o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                   && (o.IS_TRADITIONAL_MEDICINE == 1 || (o.IS_NEW_MEDICINE != 1 && o.IS_TRADITIONAL_MEDICINE != 1))).Select(o => o.ID).ToList();

                }

                mestRooms = mestRooms.Where(o => mediStockId__Actives != null && mediStockId__Actives.Contains(o.MEDI_STOCK_ID)).ToList();
                List<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE> lstMestPatientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>();
                List<long> mediStockInMestPatientTypeIds = null;
                if (patienTypeId > 0)
                    mediStockInMestPatientTypeIds = lstMestPatientType.Where(o => o.PATIENT_TYPE_ID == patienTypeId).Select(o => o.MEDI_STOCK_ID).Distinct().ToList();
                else
                {
                    if (this.currentPatientTypeWithPatientTypeAlter != null)
                    {
                        var patientTypeIdAllows = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).ToList();
                        mediStockInMestPatientTypeIds = lstMestPatientType.Where(o => patientTypeIdAllows != null && patientTypeIdAllows.Contains(o.PATIENT_TYPE_ID)).Select(o => o.MEDI_STOCK_ID).Distinct().ToList();
                    }

                    //Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentPatientTypeWithPatientTypeAlter), this.currentPatientTypeWithPatientTypeAlter));
                    // Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstMestPatientType), lstMestPatientType));
                }

                mestRoomTemps = mestRooms
                    .Where(o => mediStockInMestPatientTypeIds != null && mediStockInMestPatientTypeIds.Contains(o.MEDI_STOCK_ID)).ToList();
                Inventec.Common.Logging.LogSystem.Debug("Loc kho theo mestPatientTypeIds: so luong kho tim thay = " + mestRoomTemps.Count);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediStockInMestPatientTypeIds), mediStockInMestPatientTypeIds) + "|" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentPatientTypeWithPatientTypeAlter), this.currentPatientTypeWithPatientTypeAlter));

                this.FilterMestRoomByIsCabinet(ref mestRoomTemps);
                this.FilterMestRoomByBhytHeadCode(ref mestRoomTemps);

                Inventec.Common.Logging.LogSystem.Debug("So luong kho tim thay: " + mestRoomTemps.Count);
                if (mestRoomTemps == null || mestRoomTemps.Count == 0)
                    Inventec.Common.Logging.LogSystem.Debug("Ke don thuoc, khong tim thay kho. Du lieu truyen vao RoomId = " + GetRoomId() + " patienTypeId = " + this.currentHisPatientTypeAlter.PATIENT_TYPE_ID);

                this.InitializeComboMestRoomCheck(mestRoomTemps);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Cấu hình hệ thống trên ccc để hiển thị tủ trực hay không
        /// Cấu hình hệ thống = true thì khi người dùng mở module này lên, danh sách lọc ko hiển thị các kho là tủ trực (muốn kê tủ trực thì bấm nút Tủ trực).
        /// Cấu hình hệ thống = false (mặc định) thì load tất cả các kho như bình thường.
        /// </summary>
        /// <param name="mestRoomTemps"></param>
        private void FilterMestRoomByIsCabinet(ref List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> mestRoomTemps)
        {
            try
            {
                //if (HisConfigCFG.IsCabinet == GlobalVariables.CommonStringTrue)
                //{
                var mediStockId__Cabinets = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => (o.IS_CABINET ?? 0) == GlobalVariables.CommonNumberTrue).Select(o => o.ID).ToList();
                if (GlobalStore.IsCabinet)
                {
                    mestRoomTemps = mestRoomTemps
                        .Where(o => mediStockId__Cabinets != null && mediStockId__Cabinets.Count > 0 && mediStockId__Cabinets.Contains(o.MEDI_STOCK_ID)).ToList();
                    rdOpionGroup.Properties.Items.Remove(rdOpionGroup.Properties.Items[1]);
                }
                else
                {
                    mestRoomTemps = mestRoomTemps
                        .Where(o => mediStockId__Cabinets == null || mediStockId__Cabinets.Count == 0 || !mediStockId__Cabinets.Contains(o.MEDI_STOCK_ID)).ToList();
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Loc kho theo dieu kien la tu truc hay khong. Du lieu truyen vao (co phai la tu truc hay khong): " + Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.IsCabinet), GlobalStore.IsCabinet) + "____so luong kho tim thay = " + mestRoomTemps.Count);
                Inventec.Common.Logging.LogSystem.Debug("Loc kho theo dieu kien la tu truc hay khong (IsCabinet) => so luong kho tim thay = " + mestRoomTemps.Count);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ngoài cách lọc hiện tại, bổ sung thêm điều kiện sau (Cac dau the BHYT duoc xuat (quan y 7)):
        /// Nếu có cấu hình Cac dau the BHYT khong duoc xuat (quan y 7) -> lọc bỏ các đầu thẻ này đi
        /// Nếu BN thuộc đối tượng BHYT, thì cần kiểm tra xem trường đầu mã thẻ của kho (bhyt_head_code) có được cấu hình không.
        /// + Nếu ko cấu hình > cho phép chọn
        /// + Nếu được cấu hình > check xem d/s đầu mã thẻ được khai báo có chứa đầu mã thẻ BHYT của BN hay không. Nếu có thì mới cho phép chọn
        /// --> Kho có khai báo Đầu mã thẻ BHYT -> bệnh nhân có đầu mã thẻ thuộc mã khai báo -> khi kê mặc định chỉ check kho được khai báo, các kho khác uncheck.
        /// --> Kho có khai báo Đầu mã thẻ BHYT không cho phép -> bệnh nhân có đầu mã thẻ thuộc mã khai báo -> không cho kê kho đó, không hiển thị kho đó
        /// </summary>
        /// <param name="mestRoomTemps"></param>
        private void FilterMestRoomByBhytHeadCode(ref List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> mestRoomTemps)
        {
            try
            {
                currentMediStockByHeaderCard = new List<V_HIS_MEST_ROOM>();
                currentMediStockByNotInHeaderCard = new List<V_HIS_MEST_ROOM>();
                if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                    && !String.IsNullOrEmpty(this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER)
                    && mestRoomTemps != null && mestRoomTemps.Count > 0)
                {
                    LogSystem.Debug("So th benh nhan: HEIN_CARD_NUMBER = " + this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER);
                    List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> listMediStockTemp = new List<V_HIS_MEST_ROOM>();
                    List<long> listMediStockIdNotInTemp = new List<long>();
                    List<string> bhytHeadCodes = new List<string>();
                    List<string> notInBhytHeadCodes = new List<string>();
                    foreach (var mst in mestRoomTemps)
                    {
                        var mediStockId__One = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == mst.MEDI_STOCK_ID);
                        if (mediStockId__One != null)
                        {
                            LogSystem.Debug("Kho : " + mediStockId__One.MEDI_STOCK_NAME + "(" + mediStockId__One.MEDI_STOCK_CODE + "), BHYT_HEAD_CODE = " + mediStockId__One.BHYT_HEAD_CODE + ", NOT_IN_BHYT_HEAD_CODE = " + mediStockId__One.NOT_IN_BHYT_HEAD_CODE);
                            if (!String.IsNullOrEmpty(mediStockId__One.BHYT_HEAD_CODE) && !bhytHeadCodes.Contains(mediStockId__One.BHYT_HEAD_CODE))
                                bhytHeadCodes.Add(mediStockId__One.BHYT_HEAD_CODE);

                            if (!String.IsNullOrEmpty(mediStockId__One.NOT_IN_BHYT_HEAD_CODE) && !notInBhytHeadCodes.Contains(mediStockId__One.NOT_IN_BHYT_HEAD_CODE))
                                notInBhytHeadCodes.Add(mediStockId__One.NOT_IN_BHYT_HEAD_CODE);

                            var listIn = !String.IsNullOrEmpty(mediStockId__One.BHYT_HEAD_CODE) ? mediStockId__One.BHYT_HEAD_CODE.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries).Where(o => !String.IsNullOrEmpty(o.Trim())).ToList() : null;
                            var listNotIn = !String.IsNullOrEmpty(mediStockId__One.NOT_IN_BHYT_HEAD_CODE) ? mediStockId__One.NOT_IN_BHYT_HEAD_CODE.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries).Where(o => !String.IsNullOrEmpty(o.Trim())).ToList() : null;

                            bool accept = (listIn != null && listIn.Where(o => this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER.StartsWith(o.Trim())).Any());
                            bool noAccept = (listNotIn != null && listNotIn.Where(o => this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER.StartsWith(o.Trim())).Any());

                            if (accept && noAccept)
                            {
                                bool founded = false;
                                for (int i = 0; i < listIn.Count; i++)
                                {
                                    for (int j = 0; j < listNotIn.Count; j++)
                                    {
                                        if (listIn[i].Trim().Contains(listNotIn[j].Trim()))
                                        {
                                            currentMediStockByHeaderCard.Add(mst);
                                            LogSystem.Info("1 => currentMediStockByHeaderCard.Add(mst);, code = " + mst.MEDI_STOCK_CODE);
                                            founded = true;
                                        }
                                        if (founded)
                                            break;
                                    }
                                    if (founded)
                                        break;
                                }

                                if (!founded)
                                {
                                    LogSystem.Info("2 => currentMediStockByNotInHeaderCard.Add(mst);, code = " + mst.MEDI_STOCK_CODE);
                                    currentMediStockByNotInHeaderCard.Add(mst);
                                }
                            }
                            else
                            {
                                if (accept)
                                {
                                    currentMediStockByHeaderCard.Add(mst);
                                }

                                if (noAccept)
                                {
                                    currentMediStockByNotInHeaderCard.Add(mst);
                                }
                            }
                        }
                    }

                    if (currentMediStockByNotInHeaderCard != null && currentMediStockByNotInHeaderCard.Count > 0)
                    {
                        mestRoomTemps = mestRoomTemps.Except(currentMediStockByNotInHeaderCard).ToList();
                        Inventec.Common.Logging.LogSystem.Debug("Cac kho co cau hinh dau ma the khong chap nhan (count = " + currentMediStockByNotInHeaderCard.Count + "): " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStockByNotInHeaderCard), currentMediStockByNotInHeaderCard));
                        Inventec.Common.Logging.LogSystem.Debug("Danh sach kho sau khi loc cac dau ma the khong cho phep: " + Inventec.Common.Logging.LogUtil.TraceData("mestRoomTemps", mestRoomTemps));
                    }

                    if (currentMediStockByHeaderCard != null && currentMediStockByHeaderCard.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Cac kho co cau hinh dau ma the chap nhan (count = " + currentMediStockByHeaderCard.Count + "): " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStockByHeaderCard), currentMediStockByHeaderCard));
                    }

                    //Chỉ lấy các kho thỏa mãn các điều kiện lọc ở trên (đầu mã thẻ chấp nhận và đầu mã thẻ không chấp nhận)
                    if (mestRoomTemps == null || mestRoomTemps.Count == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong tim thay danh sach kho hop le. Nguyen nhan có the do dau ma the bhyt cua benh nhan " + (this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(0, 2)) + " khong nam trong danh sach cac dau ma the duoc chap nhan (" + String.Join(",", bhytHeadCodes) + "); hoac do dau ma the bhyt cua BN nam trong danh sach cac dau ma the khong duoc chap nhan (" + String.Join(",", notInBhytHeadCodes) + ")");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitializeComboMestRoom(List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> listMediStock)
        {
            try
            {
                this.cboMediStockExport.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMediStockExport_TabMedicine_Closed);
                this.cboMediStockExport.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboMediStockExport_TabMedicine_KeyUp);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "MEDI_STOCK_ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMediStockExport, listMediStock, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitializeComboMestRoomCheck(List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> listMediStock)
        {
            try
            {
                if (cboMediStockExport.Properties.View.Columns == null || cboMediStockExport.Properties.View.Columns.Count == 0)
                {
                    //cboMediStockExport.Properties.View.Columns.Clear();
                    InitMediStockComboByConfig();
                    cboMediStockExport.Properties.DisplayMember = "MEDI_STOCK_NAME";
                    cboMediStockExport.Properties.ValueMember = "MEDI_STOCK_ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboMediStockExport.Properties.View.Columns.AddField("MEDI_STOCK_CODE");
                    col2.VisibleIndex = 1;
                    col2.Width = 100;
                    col2.Caption = "Mã phòng khám";
                    DevExpress.XtraGrid.Columns.GridColumn col3 = cboMediStockExport.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
                    col3.VisibleIndex = 2;
                    col3.Width = 200;
                    col3.Caption = "Tên phòng khám";
                    cboMediStockExport.Properties.PopupFormWidth = 320;
                    cboMediStockExport.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboMediStockExport.Properties.View.OptionsSelection.MultiSelect = true;
                    this.cboMediStockExport.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMediStockExport_TabMedicine_Closed);
                    this.cboMediStockExport.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboMediStockExport_TabMedicine_KeyUp);
                }
                long keDonThuocMotHoacNhieuKho = 0;
                bool hasLoadedMediStock = false;
                List<long> mediStockIdDefaultByEmployees = null;
                cboMediStockExport.Properties.DataSource = listMediStock;
                List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> mediStockTemps = new List<V_HIS_MEST_ROOM>();
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    keDonThuocMotHoacNhieuKho = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__CHE_DO_KE_DON_THUOC__MOT_HOAC_NHIEU_KHO);
                    //Trường hợp có cấu hình mặc định check chọn vào các kho hoặc danh sách kho chỉ có một kho => mặc định check vào kho
                    if ((currentMediStockByHeaderCard != null && currentMediStockByHeaderCard.Count > 0))
                    {
                        mediStockTemps.AddRange(currentMediStockByHeaderCard);
                        hasLoadedMediStock = true;
                    }
                    else if (keDonThuocMotHoacNhieuKho == 1 || listMediStock.Count == 1)
                    {
                        mediStockTemps.AddRange(listMediStock);
                        hasLoadedMediStock = true;
                    }
                    else
                    {
                        //#19539
                        //Khi tắt key cấu hình CONFIG_KEY__CHE_DO_KE_DON_THUOC__MOT_HOAC_NHIEU_KHO, thì kiểm tra trong danh sách các kho được thiết lập tương ứng với phòng làm việc (thiết lập "kho xuất - phòng"), có những kho nào được khai báo kho mặc định trong chức năng "Nhân viên" (default_medi_stock_ids trong his_employee) thì tự động check chọn các kho đấy.
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        CommonParam param = new CommonParam();
                        HisEmployeeFilter employeeFilter = new HisEmployeeFilter();
                        employeeFilter.IS_ACTIVE = GlobalVariables.CommonNumberTrue;
                        employeeFilter.LOGINNAME__EXACT = loginName;
                        List<HIS_EMPLOYEE> employees = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumers.MosConsumer, employeeFilter, param);

                        if (employees != null && employees.Count > 0 && !String.IsNullOrEmpty(employees[0].DEFAULT_MEDI_STOCK_IDS))
                        {
                            mediStockIdDefaultByEmployees = GetMediStockIdByDefaultEmployee(employees[0].DEFAULT_MEDI_STOCK_IDS);
                            if (mediStockIdDefaultByEmployees != null && mediStockIdDefaultByEmployees.Count > 0)
                            {
                                hasLoadedMediStock = true;
                                mediStockTemps = listMediStock != null ? listMediStock.Where(o => mediStockIdDefaultByEmployees.Contains(o.MEDI_STOCK_ID)).ToList() : null;
                            }
                        }
                    }
                }
                if (this.currentMediStock == null)
                    this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => keDonThuocMotHoacNhieuKho), keDonThuocMotHoacNhieuKho) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediStockTemps.Count), mediStockTemps.Count) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hasLoadedMediStock), hasLoadedMediStock) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediStockIdDefaultByEmployees), mediStockIdDefaultByEmployees));
                if (hasLoadedMediStock)
                {
                    this.currentMediStock.AddRange(mediStockTemps);
                    GridCheckMarksSelection gridCheckMark = cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.SelectAll(this.currentMediStock);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<long> GetMediStockIdByDefaultEmployee(string default_medi_stock_ids)
        {
            List<long> results = new List<long>();
            try
            {
                var rs = default_medi_stock_ids.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (rs != null && rs.Count() > 0)
                {
                    results = rs.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o)).Where(o => o > 0).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return results;
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, List<ColumnInfo> columnInfos, bool showHeader, int popupWidth)
        {
            try
            {
                //if (!String.IsNullOrEmpty(displayMemberCode))
                //{
                //    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                //    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                //}
                //if (!String.IsNullOrEmpty(displayMember))
                //{
                //    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                //    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                //}
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, showHeader, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Load lý do xuất
        private void InitComboExpMestReason()
        {
            try
            {
                if (actionType == GlobalVariables.ActionEdit)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 150, 1));
                    columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, 400);
                    ControlEditorLoader.Load(cboExpMestReason, this.lstExpMestReasons, controlEditorADO);

                    if (this.oldExpMest != null)
                    {
                        cboExpMestReason.EditValue = this.oldExpMest.EXP_MEST_REASON_ID;
                    }
                    //else if (this.Histreatment != null)
                    //{
                    //    HIS_EXME_REASON_CFG ExmeReasonCfg = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXME_REASON_CFG>().FirstOrDefault(o => o.PATIENT_CLASSIFY_ID == this.Histreatment.TDL_PATIENT_CLASSIFY_ID && o.TREATMENT_TYPE_ID == this.Histreatment.TDL_TREATMENT_TYPE_ID);

                    //    if (ExmeReasonCfg != null)
                    //    {
                    //        cboExpMestReason.EditValue = ExmeReasonCfg.EXP_MEST_REASON_ID;
                    //    }
                    //    else
                    //    {
                    //        cboExpMestReason.EditValue = null;
                    //    }
                    //}
                    else
                    {
                        cboExpMestReason.EditValue = null;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComborepositoryItemCustomGridLookUpReasion()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "Mã", 50, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "Tên", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, true, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(this.repositoryItemCustomGridLookUpReasion, this.lstExpMestReasons, controlEditorADO);

                if (HisConfigCFG.IsReasonRequired)
                {
                    this.repositoryItemCustomGridLookUpReasion.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
