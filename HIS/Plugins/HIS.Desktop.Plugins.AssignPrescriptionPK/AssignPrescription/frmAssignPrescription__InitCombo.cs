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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        List<long> mediStockId__Cabinets;
        List<long> mediStockId__Bloods;

        public List<HtuADO> DataHtuList { get; private set; }
        public List<HtuADO> DataHtuListShow { get; private set; }
        private async Task InitComboHtu(List<MOS.EFMODEL.DataModels.HIS_HTU> data)
        {
            try
            {
                List<HIS_HTU> htus = null;
                if (BackendDataWorker.IsExistsKey<HIS_HTU>())
                {
                    htus = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HTU>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    htus = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_HTU>>("api/HisHtu/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (htus != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_HTU), htus, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (data != null)
                {
                    data = data.OrderBy(o => o.NUM_ORDER).ToList();
                }
                else
                    data = htus.OrderBy(o => o.NUM_ORDER).ToList();
                data.ForEach(o => o.CHECK_ACIN_INTERACTIVE = o.CHECK_ACIN_INTERACTIVE == 0 || o.CHECK_ACIN_INTERACTIVE == null ? Int16.MaxValue : (short)o.CHECK_ACIN_INTERACTIVE);
                AutoMapper.Mapper.CreateMap<HIS_HTU, HtuADO>();
                List<long> IdsCheck = new List<long>();
                if (DataHtuList != null && DataHtuList.Count > 0 && DataHtuList.FirstOrDefault(o => o.IsChecked) != null)
                {
                    IdsCheck.AddRange(DataHtuList.Where(o => o.IsChecked).Select(o => o.ID).ToList());
                }
                DataHtuList = AutoMapper.Mapper.Map<List<HtuADO>>(data);
                DataHtuList.ForEach(o => o.IsChecked = IdsCheck.Exists(p => p == o.ID));
                DataHtuList = DataHtuList.OrderByDescending(o => o.IsChecked ? 1 : 0).ThenByDescending(o=>o.ID).ToList();
                AutoMapper.Mapper.CreateMap<HtuADO, HtuADO>();
                DataHtuListShow = AutoMapper.Mapper.Map<List<HtuADO>>(DataHtuList);
                DataHtuListShow = DataHtuListShow.OrderByDescending(o => o.IsChecked ? 1 : 0).ThenByDescending(o => o.ID).ToList();
                gridControlHtu.DataSource = null;
                gridControlHtu.DataSource = DataHtuList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task InitComboNhaThuoc()
        {
            try
            {
                mediStockAllows = new List<V_HIS_MEDI_STOCK>();
                List<V_HIS_MEDI_STOCK> mediStocks = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDI_STOCK>().ToList();
                if (mediStocks == null)
                    mediStocks = new List<V_HIS_MEDI_STOCK>();
                mediStockAllows = mediStocks.Where(o => o.IS_ACTIVE == 1 && o.IS_DRUG_STORE == 1 && o.IS_BUSINESS == 1).ToList();
                cboNhaThuoc.Enabled = true;
                if (actionType == GlobalVariables.ActionAdd)
                {
                    if (HisConfigCFG.IsAutoCreateSaleExpMest || HisConfigCFG.IsDrugStoreComboboxOption)
                    {
                        InitializeComboNhaThuocNoCheck(mediStockAllows);
                    }
                    else
                    {
                        InitializeComboNhaThuocHasCheck(mediStockAllows);
                    }
                }
                else if (actionType == GlobalVariables.ActionEdit)
                {
                    InitializeComboNhaThuocNoCheck(mediStockAllows);
                    if (HisConfigCFG.IsAutoCreateSaleExpMest || HisConfigCFG.IsDrugStoreComboboxOption)
                        cboNhaThuoc.Enabled = false;
                }
                if (this.currentMediStockNhaThuocSelecteds != null && this.currentMediStockNhaThuocSelecteds.Count > 0)
                {
                    this.cboNhaThuoc.ShowPopup();
                    this.cboNhaThuoc.ClosePopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientType(GridLookUpEdit cboPatientType, object data)
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

        private async Task InitComboEquipment()
        {
            try
            {
                if (!GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                {
                    lciEquipment.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    gridColumnEquipment.Visible = false;
                    gridColumnEquipment.Visible = false;
                    return;
                }

                List<HIS_EQUIPMENT_SET> equipmentSets = null;

                if (BackendDataWorker.IsExistsKey<HIS_EQUIPMENT_SET>())
                {
                    equipmentSets = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EQUIPMENT_SET>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    equipmentSets = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET>>("api/HisEquipmentSet/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (equipmentSets != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET), equipmentSets, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EQUIPMENT_SET_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EQUIPMENT_SET_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboEquipment, equipmentSets, controlEditorADO);
                ControlEditorLoader.Load(repositoryItemGridLookUpEditEquipmentSet__Enabled, equipmentSets, controlEditorADO);
                ControlEditorLoader.Load(repositoryItemGridLookUpEditEquipmentSet__Disabled, equipmentSets, controlEditorADO);
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
                cbo.Properties.Buttons[1].Visible = false;
                if (trackingADOs == null)
                    return;

                if (cbo.EditValue == null)
                {
                    if (this.actionType == GlobalVariables.ActionEdit)
                    {
                        cbo.EditValue = null;
                        cbo.Properties.Buttons[1].Visible = false;
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
                    if (chkMultiIntructionTime.Checked)
                    {
                        cbo.EditValue = null;
                        cbo.Properties.Buttons[1].Visible = false;
                    }
                }
                if (!chkMultiIntructionTime.Checked)
                {
                    cbo.Properties.View.OptionsSelection.MultiSelect = false;
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("TrackingTimeStr", "Thời gian", 250, 1));
                    columnInfos.Add(new ColumnInfo("CREATOR", "Người tạo", 100, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("TrackingTimeStr", "ID", columnInfos, true, 350);
                    ControlEditorLoader.Load(cbo, trackingADOs, controlEditorADO);
                }
                else
                {
                    InitCheck(cbo, SelectionGrid__ToDieuTri);
                    InitCombo(cbo, trackingADOs, "TrackingTimeStr", "CREATOR", "ID");

                    cbo.ShowPopup();
                    cbo.ClosePopup();
                    List<string> intructionDateSelectedProcess = new List<string>();
                    foreach (var item in this.intructionTimeSelecteds)
                    {
                        string intructionDate = item.ToString().Substring(0, 8);
                        intructionDateSelectedProcess.Add(intructionDate);
                    }

                    if (HisConfigCFG.IsDefaultTracking)
                    {
                        var trackingTemps = this.trackingADOs.Where(o => intructionDateSelectedProcess.Contains(o.TRACKING_TIME.ToString().Substring(0, 8))
                            && o.CREATOR.ToUpper() == this.txtLoginName.Text.ToUpper())
                            .GroupBy(o => o.TRACKING_TIME.ToString().Substring(0, 8));

                        List<TrackingADO> LstTrackingADOs = new List<TrackingADO>();

                        foreach (var itemG in trackingTemps)
                        {
                            TrackingADO ado = new TrackingADO();
                            ado = itemG.OrderByDescending(o => o.TRACKING_TIME).FirstOrDefault();
                            LstTrackingADOs.Add(ado);
                        }

                        GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                        if (gridCheckMark != null)
                        {
                            gridCheckMark.SelectAll(LstTrackingADOs);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                cbo.Properties.View.Columns.Clear();
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue1, string DisplayValue2, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue1;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue1);
                col2.VisibleIndex = 1;
                col2.Width = 250;
                col2.Caption = "Thời gian";

                DevExpress.XtraGrid.Columns.GridColumn col3 = cbo.Properties.View.Columns.AddField(DisplayValue2);
                col3.VisibleIndex = 2;
                col3.Width = 100;
                col3.Caption = "Người tạo";

                cbo.Properties.PopupFormWidth = 350;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SelectionGrid__ToDieuTri(object sender, EventArgs e)
        {
            try
            {
                this.Listtrackings = new List<HIS_TRACKING>();
                foreach (HIS_TRACKING rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        HIS_TRACKING tracking = new HIS_TRACKING();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRACKING>(tracking, rv);
                        this.Listtrackings.Add(tracking);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void cboToDieuTri_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        //{
        //    try
        //    {
        //        e.DisplayText = "";
        //        string ToDieuTri = "";
        //        if (this.Listtrackings != null && this.Listtrackings.Count > 0)
        //        {
        //            foreach (var item in this.Listtrackings)
        //            {
        //                ToDieuTri += Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(item.TRACKING_TIME) + ", ";
        //            }
        //        }

        //        e.DisplayText = ToDieuTri;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}


        private async Task InitComboRepositoryExpendType(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemcboExpendType)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXPEND_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXPEND_TYPE_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXPEND_TYPE_NAME", "ID", columnInfos, false, 250);

                ControlEditorLoader.Load(repositoryItemcboExpendType, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitComboRepositoryPatientType(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemcboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
        {
            try
            {
                if (repositoryItemcboPatientType.View.Columns.Count <= 0)
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
                else
                {
                    repositoryItemcboPatientType.DataSource = (data != null ? data : currentPatientTypeWithPatientTypeAlter);
                }
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

        private async Task InitComboExpMestTemplate()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE> datas = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>>("api/HisExpMestTemplate/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_TEMPLATE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_TEMPLATE_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_TEMPLATE_NAME", "ID", columnInfos, false, 400);
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var expMestTemplates = datas.Where(o =>
                    (o.CREATOR == loginName || (o.IS_PUBLIC ?? -1) == GlobalVariables.CommonNumberTrue)
                    && o.IS_ACTIVE == GlobalVariables.CommonNumberTrue
                    //&& (o.IS_KIDNEY == null || o.IS_KIDNEY != GlobalVariables.CommonNumberTrue)//TODO
                    ).ToList();
                ControlEditorLoader.Load(cboExpMestTemplate, expMestTemplates, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Load người chỉ định
        private async Task InitComboUser()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                List<HIS_EMPLOYEE> employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
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
                datas = datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(cboUser, datas, controlEditorADO);

                var data = datas.Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).ToList();
                if (data != null && data.Count > 0)
                {
                    this.cboUser.EditValue = data[0].LOGINNAME;
                    this.txtLoginName.Text = data[0].LOGINNAME;
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
        private async Task InitComboMedicineUseForm(List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM> data)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM> datas = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>>("api/HisMedicineUseForm/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 250);
                if (data != null)
                {
                    data = data.OrderBy(o => o.NUM_ORDER).ToList();
                }
                else
                    data = datas.OrderBy(o => o.NUM_ORDER).ToList();
                ControlEditorLoader.Load(cboMedicineUseForm, data, controlEditorADO);
                ControlEditorLoader.Load(repPicIsAssignPresed, data, controlEditorADO);
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

        private HIS_TREATMENT GetTreatment(long treatmentId)
        {
            HIS_TREATMENT data = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = treatmentId;
                data = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return data;
        }

        /// <summary>
        /// Gọi api load danh sách kho
        /// Lọc theo các cấu hình
        /// </summary>
        /// <param name="patienTypeId"></param>
        private async Task InitComboMediStockAllow(long patienTypeId)
        {
            try
            {

                this.currentWorkingMestRooms = new List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>();
                Action myaction = () =>
                {
                    List<V_HIS_MEST_ROOM> mestRooms = new List<V_HIS_MEST_ROOM>();

                    var ExecuteRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.ROOM_ID == (this.currentModule != null ? this.currentModule.RoomId : 0) && o.IS_EXAM == 1).FirstOrDefault();
                    if (HisConfigCFG.MestRoomOption == 1 && ExecuteRoom != null && !GlobalStore.IsCabinet)
                    {
                        CommonParam paramReq = new CommonParam();
                        HisServiceReqFilter ReqFilter = new HisServiceReqFilter();
                        ReqFilter.TREATMENT_ID = treatmentId;
                        ReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                        ReqFilter.IS_MAIN_EXAM = true;

                        var ServiceReq = new BackendAdapter(paramReq).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, ReqFilter, paramReq).FirstOrDefault();

                        if (ServiceReq != null)
                        {
                            mestRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == ServiceReq.EXECUTE_ROOM_ID).ToList();
                        }
                    }
                    else
                    {
                        mestRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == GetRoomId()).ToList();
                    }
                    List<V_HIS_MEDI_STOCK> medistocks = new List<V_HIS_MEDI_STOCK>();

                    var listmedistocks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>();

                    //currentTreatment = GetTreatment(this.treatmentId);

                    if (currentTreatment != null && currentTreatment.TDL_PATIENT_CLASSIFY_ID != null)
                    {
                        var data = listmedistocks.Where(o => String.IsNullOrEmpty(o.PATIENT_CLASSIFY_IDS) || (!String.IsNullOrEmpty(o.PATIENT_CLASSIFY_IDS) && o.PATIENT_CLASSIFY_IDS.Split(',').ToList().Contains(currentTreatment.TDL_PATIENT_CLASSIFY_ID.ToString()))).ToList();
                        medistocks.AddRange(data);
                    }
                    else
                    {
                        medistocks.AddRange(listmedistocks.Where(o => String.IsNullOrEmpty(o.PATIENT_CLASSIFY_IDS)).ToList());
                    }

                    //var medistocks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>();
                    var mediStockId__Actives = medistocks.Where(
                        o => (o.IS_ACTIVE == null
                            || o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            && ((o.IS_NEW_MEDICINE ?? 0) == 1 || ((o.IS_NEW_MEDICINE ?? 0) != 1 && (o.IS_TRADITIONAL_MEDICINE ?? 0) != 1))).Select(o => o.ID).ToList();
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
                    }

                    this.currentWorkingMestRooms = mestRooms
                        .Where(o => mediStockInMestPatientTypeIds != null && mediStockInMestPatientTypeIds.Contains(o.MEDI_STOCK_ID)).ToList();
                    Inventec.Common.Logging.LogSystem.Debug("Loc kho theo mestPatientTypeIds: so luong kho tim thay = " + this.currentWorkingMestRooms.Count);
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                this.FilterMestRoomByIsCabinet(ref this.currentWorkingMestRooms);
                this.FilterMestRoomByBhytHeadCode(ref this.currentWorkingMestRooms);
                this.FilterMestRoomByMestPatyTrty(ref this.currentWorkingMestRooms);

                Inventec.Common.Logging.LogSystem.Debug("So luong kho tim thay: " + this.currentWorkingMestRooms.Count);
                if (this.currentWorkingMestRooms == null || this.currentWorkingMestRooms.Count == 0)
                    Inventec.Common.Logging.LogSystem.Debug("Ke don thuoc, khong tim thay kho. Du lieu truyen vao RoomId = " + GetRoomId() + " patienTypeId = " + this.currentHisPatientTypeAlter.PATIENT_TYPE_ID);

                this.InitializeComboMestRoomCheck(this.currentWorkingMestRooms);

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
                if (mediStockId__Cabinets == null || mediStockId__Cabinets.Count == 0)
                {
                    mediStockId__Cabinets = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => (o.IS_CABINET ?? 0) == GlobalVariables.CommonNumberTrue).Select(o => o.ID).ToList();
                }
                if (mediStockId__Bloods == null || mediStockId__Bloods.Count == 0)
                {
                    mediStockId__Bloods = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => (o.IS_BLOOD ?? 0) == GlobalVariables.CommonNumberTrue).Select(o => o.ID).ToList();
                }
                mestRoomTemps = mestRoomTemps.Where(o => !mediStockId__Bloods.Contains(o.MEDI_STOCK_ID)).ToList();
                if (GlobalStore.IsCabinet)
                {
                    mestRoomTemps = mestRoomTemps
                        .Where(o => mediStockId__Cabinets != null && mediStockId__Cabinets.Count > 0 && mediStockId__Cabinets.Contains(o.MEDI_STOCK_ID)).ToList();
                    rdOpionGroup.Properties.Items[1].Enabled = false;
                    //rdOpionGroup.Properties.Items.Remove(rdOpionGroup.Properties.Items[1]);
                }
                else
                {
                    rdOpionGroup.Properties.Items[1].Enabled = true;
                    mestRoomTemps = mestRoomTemps
                        .Where(o => mediStockId__Cabinets == null || mediStockId__Cabinets.Count == 0 || !(mediStockId__Cabinets != null && mediStockId__Cabinets.Contains(o.MEDI_STOCK_ID))).ToList();
                }
                if (mestRoomTemps != null)
                {
                    mestRoomTemps = mestRoomTemps.Distinct().ToList();
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
        /// - Dựa vào đối tượng bệnh nhân và diện điều trị của bệnh nhân kiểm tra có dữ liệu HIS_MEST_PATY_TRTY tương ứng không.
        ///   + Nếu không thì không làm gì.
        ///   + Nếu có thì loại các kho xuất tương ứng với dữ liệu thiết lập khỏi combobox kho xuất.
        /// </summary>
        /// <param name="mestRoomTemps"></param>
        private void FilterMestRoomByMestPatyTrty(ref List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> mestRoomTemps)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FilterMestRoomByMestPatyTrty => truoc khi loc theo HIS_MEST_PATY_TRTY. mestRoomTemps.count =" + (mestRoomTemps != null ? mestRoomTemps.Count : 0));
                CommonParam param1 = new CommonParam();
                HisMestPatyTrtyFilter mestPatyTrtyFilter = new HisMestPatyTrtyFilter()
                {
                    PATIENT_TYPE_ID = this.currentHisPatientTypeAlter.PATIENT_TYPE_ID,
                    TREATMENT_TYPE_ID = this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID
                };
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mestPatyTrtyFilter), mestPatyTrtyFilter));
                var mestPatyTrtys = new BackendAdapter(param1)
                  .Get<List<HIS_MEST_PATY_TRTY>>("api/HisMestPatyTrty/Get", ApiConsumers.MosConsumer, mestPatyTrtyFilter, param1);

                if (mestPatyTrtys != null && mestPatyTrtys.Count > 0)
                {
                    var mestExceptIds = mestPatyTrtys.Select(o => o.MEDI_STOCK_ID).Distinct().ToList();
                    mestRoomTemps = mestRoomTemps.Where(o => !mestExceptIds.Contains(o.MEDI_STOCK_ID)).ToList();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mestExceptIds), mestExceptIds));
                }
                Inventec.Common.Logging.LogSystem.Debug("FilterMestRoomByMestPatyTrty => sau khi loc theo HIS_MEST_PATY_TRTY. mestRoomTemps.count =" + (mestRoomTemps != null ? mestRoomTemps.Count : 0));
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
                                            LogSystem.Debug("1 => currentMediStockByHeaderCard.Add(mst);, code = " + mst.MEDI_STOCK_CODE);
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
                                    LogSystem.Debug("2 => currentMediStockByNotInHeaderCard.Add(mst);, code = " + mst.MEDI_STOCK_CODE);
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
                        //Inventec.Common.Logging.LogSystem.Debug("Danh sach kho sau khi loc cac dau ma the khong cho phep: " + Inventec.Common.Logging.LogUtil.TraceData("mestRoomTemps", mestRoomTemps));
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
                this.cboMediStockExport.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboMediStockExport_TabMedicine_KeyUp);

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

        private void InitializeComboNhaThuocNoCheck(List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> mediStockAllows)
        {
            try
            {
                if (cboNhaThuoc.Properties.View.Columns.Count <= 0)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(cboNhaThuoc, mediStockAllows, controlEditorADO);
                }
                else
                {
                    cboNhaThuoc.Properties.DataSource = mediStockAllows;
                }

                long medistockIdDefault = 0;
                if (this.assignPrescriptionEditADO != null && this.oldServiceReq != null && this.oldServiceReq.EXECUTE_ROOM_ID > 0)
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
                    this.cboMediStockExport.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboMediStockExport_TabMedicine_KeyUp);
                }

                bool hasLoadedMediStock = false;
                cboMediStockExport.Properties.DataSource = listMediStock;

                List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> mediStockTemps = new List<V_HIS_MEST_ROOM>();

                List<long> mediStockIdDefaultByEmployees = null;
                long keDonThuocMotHoacNhieuKho = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__CHE_DO_KE_DON_THUOC__MOT_HOAC_NHIEU_KHO);
                if (this.actionType == GlobalVariables.ActionAdd)
                {
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
                if (hasLoadedMediStock && mediStockTemps != null && mediStockTemps.Count > 0)
                {
                    this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                    this.currentMediStock.AddRange(mediStockTemps);
                    GridCheckMarksSelection gridCheckMark = cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        //if (gridCheckMark.Selection.Count == 0)
                        gridCheckMark.SelectAll(this.currentMediStock);
                    }
                    else
                    {
                        if (this.currentMediStock != null && this.currentMediStock.Count > 0)
                            cboMediStockExport.EditValue = this.currentMediStock.First().MEDI_STOCK_ID;
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => keDonThuocMotHoacNhieuKho), keDonThuocMotHoacNhieuKho) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock.Count), currentMediStock.Count) + "____" + (currentMediStock != null ? String.Join(";", currentMediStock.Select(o => o.MEDI_STOCK_ID)) : "") + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hasLoadedMediStock), hasLoadedMediStock) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediStockIdDefaultByEmployees), mediStockIdDefaultByEmployees));
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
                    List<HIS_EXP_MEST_REASON> datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue).ToList();

                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 150, 1));
                    columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, 400);
                    ControlEditorLoader.Load(cboExpMestReason, datas, controlEditorADO);

                    if (this.oldExpMest != null)
                    {
                        cboExpMestReason.EditValue = this.oldExpMest.EXP_MEST_REASON_ID;
                    }
                    //else if (this.VHistreatment != null)
                    //{
                    //    HIS_EXME_REASON_CFG ExmeReasonCfg = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXME_REASON_CFG>().FirstOrDefault(o => o.PATIENT_CLASSIFY_ID == this.VHistreatment.TDL_PATIENT_CLASSIFY_ID && o.TREATMENT_TYPE_ID == this.VHistreatment.TDL_TREATMENT_TYPE_ID);

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
