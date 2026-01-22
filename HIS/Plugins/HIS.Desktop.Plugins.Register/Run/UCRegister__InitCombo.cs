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
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        private List<MOS.EFMODEL.DataModels.HIS_CUSTOMER_SOURCE_DT> allCustomerSourceDetails;
        private List<MOS.EFMODEL.DataModels.HIS_CUSTOMER_SOURCE_DT> filteredCustomerSourceDetails;

        private async Task InitComboCustomerSourceDetail()
        {
            try
            {
                // Load tất cả nguồn khách chi tiết
                allCustomerSourceDetails = BackendDataWorker
                    .Get<MOS.EFMODEL.DataModels.HIS_CUSTOMER_SOURCE_DT>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToList();

                // Khởi tạo combo với GridCheckMarksSelection để cho phép chọn nhiều
                this.InitComboCustomerSourceDetailGridCheck();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCustomerSourceDetailGridCheck()
        {
            try
            {
                // Sử dụng GridLookUpEdit với CheckBoxRowSelect
                if (allCustomerSourceDetails == null)
                {
                    allCustomerSourceDetails = new List<MOS.EFMODEL.DataModels.HIS_CUSTOMER_SOURCE_DT>();
                }

                cboCustomerSourceDetail.Properties.DataSource = allCustomerSourceDetails;
                cboCustomerSourceDetail.Properties.DisplayMember = "USERNAME";
                cboCustomerSourceDetail.Properties.ValueMember = "LOGINNAME";
                cboCustomerSourceDetail.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboCustomerSourceDetail.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboCustomerSourceDetail.Properties.ImmediatePopup = true;
                cboCustomerSourceDetail.Properties.AutoComplete = false;

                // Thiết lập View
                var gridView = cboCustomerSourceDetail.Properties.View;
                gridView.OptionsSelection.MultiSelect = true;
                gridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;

                // Thêm các cột
                gridView.Columns.Clear();

                DevExpress.XtraGrid.Columns.GridColumn colLoginName = new DevExpress.XtraGrid.Columns.GridColumn();
                colLoginName.FieldName = "LOGINNAME";
                colLoginName.Caption = "Mã";
                colLoginName.Width = 100;
                colLoginName.Visible = true;
                colLoginName.VisibleIndex = 0;
                gridView.Columns.Add(colLoginName);

                DevExpress.XtraGrid.Columns.GridColumn colUserName = new DevExpress.XtraGrid.Columns.GridColumn();
                colUserName.FieldName = "USERNAME";
                colUserName.Caption = "Tên";
                colUserName.Width = 250;
                colUserName.Visible = true;
                colUserName.VisibleIndex = 1;
                gridView.Columns.Add(colUserName);

                // Tooltip
                cboCustomerSourceDetail.ToolTip = "Nguồn khách chi tiết";

                // ✅ THÊM: Event khi tick/untick checkbox trong GridLookUpEdit
                gridView.SelectionChanged += GridViewCustomerSourceDetail_SelectionChanged;

                // Đăng ký event khi đóng popup để cập nhật display text
                cboCustomerSourceDetail.Properties.CloseUp += CboCustomerSourceDetail_CloseUp;

                // Xử lý custom display text
                cboCustomerSourceDetail.Properties.CustomDisplayText += CboCustomerSourceDetail_CustomDisplayText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridViewCustomerSourceDetail_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                // Refresh ngay lập tức khi tick/untick checkbox
                cboCustomerSourceDetail.RefreshEditValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboCustomerSourceDetail_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                var gridView = cboCustomerSourceDetail.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gridView != null)
                {
                    var selectedRows = gridView.GetSelectedRows();
                    var userNames = new List<string>();

                    foreach (int rowHandle in selectedRows)
                    {
                        if (rowHandle >= 0)
                        {
                            var row = gridView.GetRow(rowHandle) as MOS.EFMODEL.DataModels.HIS_CUSTOMER_SOURCE_DT;
                            if (row != null)
                            {
                                userNames.Add(row.USERNAME);
                            }
                        }
                    }

                    if (userNames.Count > 0)
                    {
                        e.DisplayText = string.Join(", ", userNames);
                    }
                    else
                    {
                        e.DisplayText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboCustomerSourceDetail_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            try
            {
                // Khi đóng popup, refresh display text
                cboCustomerSourceDetail.RefreshEditValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCustomerSource_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Clear selection trước
                var gridView = cboCustomerSourceDetail.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gridView != null)
                {
                    gridView.ClearSelection();
                }

                cboCustomerSourceDetail.EditValue = null;

                if (cboCustomerSource.EditValue != null)
                {
                    string customerSourceCode = cboCustomerSource.EditValue.ToString();
                    var customerSource = BackendDataWorker
                        .Get<MOS.EFMODEL.DataModels.HIS_CUSTOMER_SOURCE>()
                        .FirstOrDefault(o => o.CUSTOMER_SOURCE_CODE == customerSourceCode);

                    if (customerSource != null)
                    {
                        // 1. Lọc danh sách theo CUSTOMER_SOURCE_ID
                        filteredCustomerSourceDetails = allCustomerSourceDetails
                            .Where(o => o.CUSTOMER_SOURCE_ID == customerSource.ID)
                            .ToList();

                        // 2. Set datasource
                        cboCustomerSourceDetail.Properties.DataSource = filteredCustomerSourceDetails;
                        cboCustomerSourceDetail.Properties.View.RefreshData();

                        // 3. Nếu có default thì tự động tick các item default
                        if (!string.IsNullOrEmpty(customerSource.DEFAULT_DETAIL_LOGINNAMES))
                        {
                            LoadDefaultCustomerSourceDetail(customerSource.DEFAULT_DETAIL_LOGINNAMES);
                        }
                    }
                }
                else
                {
                    // Reset về tất cả nếu không chọn nguồn khách
                    cboCustomerSourceDetail.Properties.DataSource = allCustomerSourceDetails;
                    cboCustomerSourceDetail.Properties.View.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDefaultCustomerSourceDetail(string defaultLoginNames)
        {
            try
            {
                if (string.IsNullOrEmpty(defaultLoginNames)) return;

                var loginNameList = defaultLoginNames.Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

                var gridView = cboCustomerSourceDetail.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gridView != null)
                {
                    gridView.ClearSelection();
                    for (int i = 0; i < gridView.DataRowCount; i++)
                    {
                        var row = gridView.GetRow(i) as MOS.EFMODEL.DataModels.HIS_CUSTOMER_SOURCE_DT;
                        if (row != null && loginNameList.Contains(row.LOGINNAME))
                        {
                            gridView.SelectRow(i);
                        }
                    }
                    // Cập nhật hiển thị text
                    cboCustomerSourceDetail.RefreshEditValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateCustomerSourceDetailDisplayText()
        {
            try
            {
                cboCustomerSourceDetail.RefreshEditValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadConfigOweTypeDefault(List<MOS.EFMODEL.DataModels.HIS_OWE_TYPE> oweTypes)
        {
            try
            {
                if (!String.IsNullOrEmpty(AppConfigs.OweTypeDefault))
                {
                    var oweType = oweTypes.SingleOrDefault(o => o.OWE_TYPE_CODE == AppConfigs.OweTypeDefault);
                    if (oweType == null) throw new ArgumentNullException("Khong tim thay HIS_OWE_TYPE theo OWE_TYPE_CODE = " + AppConfigs.OweTypeDefault);

                    cboOweType.EditValue = oweType.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisibleControlBill()
        {
            try
            {
                if (HisConfigCFG.IsVisibleBill == "1")
                {
                    lcibtnBill.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitTypeFind()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemPatientCode = new DXMenuItem(typeCodeFind__MaBN, new EventHandler(btnCodeFind_Click));
                itemPatientCode.Tag = "patientCode";
                menu.Items.Add(itemPatientCode);

                DXMenuItem itemProgramCode = new DXMenuItem(typeCodeFind__MaCT, new EventHandler(btnCodeFind_Click));
                itemProgramCode.Tag = "programCode";
                menu.Items.Add(itemProgramCode);

                DXMenuItem itemAppointmentCode = new DXMenuItem(typeCodeFind__MaHK, new EventHandler(btnCodeFind_Click));
                itemAppointmentCode.Tag = "appointmentCode";
                menu.Items.Add(itemAppointmentCode);

                DXMenuItem itemSoThe = new DXMenuItem(typeCodeFind__SoThe, new EventHandler(btnCodeFind_Click));
                itemSoThe.Tag = "cardCode";
                menu.Items.Add(itemSoThe);

                DXMenuItem itemMaNV = new DXMenuItem(typeCodeFind__MaNV, new EventHandler(btnCodeFind_Click));
                itemMaNV.Tag = "employeeCode";
                menu.Items.Add(itemMaNV);

                DXMenuItem itemCCCD = new DXMenuItem(typeCodeFind__CCCDCMND, new EventHandler(btnCodeFind_Click));
                itemCCCD.Tag = "CccdCmnd";
                menu.Items.Add(itemCCCD);

                btnCodeFind.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlPatientInfo()
        {
            try
            {
                if (AppConfigs.TiepDon_HienThiMotSoThongTinThemBenhNhan == 1)
                {
                    lcitxtMilitaryRankCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lcicboMilitaryRank.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciCareer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciCboCareer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciBtnPatientExtend.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciWorkPlace.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    gboxMoreInformation.Height = 70;
                    //lciPhone.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //lciPhone.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //lcitxtHomePerson.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //lcitxtRelativeAddress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //lcitxtCorrelated.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //lciHNCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task FillDataToControlsForm()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FillDataToControlsForm Start!");
                var paties = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(p => p.IS_ACTIVE == 1 && p.IS_NOT_USE_FOR_PATIENT != 1).ToList();

                this.InitComboCommon(this.cboPatientType, paties, "ID", "PATIENT_TYPE_NAME", "PATIENT_TYPE_CODE");
                this.InitComboCommon(this.cboTHX, workingCommuneADO, "ID", "RENDERER_PDC_NAME", "SEARCH_CODE_COMMUNE");
                this.ChangeDataSourceAddress();
                this.InitComboCommon(this.cboCareer, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "ID", "CAREER_NAME", "CAREER_CODE");
                cboCareer.Properties.ImmediatePopup = true;
                this.InitComboCommon(this.cboEthnic, BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().Where(e => e.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "ETHNIC_NAME", "ETHNIC_NAME", "ETHNIC_CODE");
                this.InitComboCommon(this.cboNational, BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "NATIONAL_NAME", "NATIONAL_NAME", "NATIONAL_CODE");
                this.InitComboCommon(this.cboAge, BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO>(), "Id", "MoTa", "");
                this.InitComboCommon(this.cboGender, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>(), "ID", "GENDER_NAME", "GENDER_CODE");
                this.InitComboCommon(this.cboProvinceKS, SdaProvinces.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE), "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");

                var classify = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>().Where(p => p.IS_ACTIVE == 1).ToList();
                this.InitComboCommon(this.cboPatientClassify, classify, "ID", "PATIENT_CLASSIFY_NAME", "PATIENT_CLASSIFY_CODE");
                this.InitComboCustomerSource();
                this.InitComboCustomerSourceDetail();
                await this.InitEmergencyTime();
                await this.InitTreatmentType();
                await this.InitOweType();
                await this.InitCashierRoom();
                await this.InitMilitaryRank();
                this.InitWorkPlaceControl();
                await this.InitComboHisHospitalizeReason();
                Inventec.Common.Logging.LogSystem.Debug("FillDataToControlsForm Finished!");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboCustomerSource()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_CUSTOMER_SOURCE> datas = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_CUSTOMER_SOURCE>())
                {
                    datas = BackendDataWorker
                        .Get<MOS.EFMODEL.DataModels.HIS_CUSTOMER_SOURCE>()
                        .Where(o => o.IS_ACTIVE == 1)
                        .ToList();
                }
                else
                {
                    datas = BackendDataWorker
                        .Get<MOS.EFMODEL.DataModels.HIS_CUSTOMER_SOURCE>()
                        .Where(o => o.IS_ACTIVE == 1)
                        .ToList();
                }
                this.InitComboCommon(
                    this.cboCustomerSource,
                    datas,
                    "CUSTOMER_SOURCE_CODE",
                    "CUSTOMER_SOURCE_NAME",
                    "CUSTOMER_SOURCE_CODE"
                );
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task InitComboHisHospitalizeReason()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON> datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>().Where(o=>o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                this.InitComboCommon(this.cboHosReason, datas, "ID", "HOSPITALIZE_REASON_NAME", "HOSPITALIZE_REASON_CODE");
                //this.cboHosReason.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task InitMilitaryRank()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK> datas = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>>("api/HisMilitaryRank/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                this.InitComboCommon(this.cboMilitaryRank, datas, "ID", "MILITARY_RANK_NAME", "MILITARY_RANK_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitCashierRoom()
        {
            try
            {
                LogSystem.Debug("InitCashierRoom. 1");
                List<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM> dataCashierRooms = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>())
                {
                    dataCashierRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataCashierRooms = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>>("api/HisCashierRoom/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataCashierRooms != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM), dataCashierRooms, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                var roomIds = WorkPlace.GetRoomIds();
                if (roomIds == null || roomIds.Count == 0)
                    throw new ArgumentNullException("Nguoi dung khong chon phong thu ngan nao");

                dataCashierRooms = dataCashierRooms.Where(o => roomIds.Contains(o.ROOM_ID)).ToList();

                this.InitComboCommon(this.cboCashierRoom, dataCashierRooms, "ID", "CASHIER_ROOM_NAME", "CASHIER_ROOM_CODE");

                if (dataCashierRooms != null && dataCashierRooms.Count == 1)
                {
                    this.cboCashierRoom.EditValue = dataCashierRooms.First().ID;
                }

                LogSystem.Debug("InitCashierRoom. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitOweType()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_OWE_TYPE> dataOweTypes = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>())
                {
                    dataOweTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataOweTypes = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>>("api/HisOweType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataOweTypes != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_OWE_TYPE), dataOweTypes, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                this.InitComboCommon(this.cboOweType, dataOweTypes, "ID", "OWE_TYPE_NAME", "OWE_TYPE_CODE");
                this.LoadConfigOweTypeDefault(dataOweTypes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitEmergencyTime()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME> dataEmergencyWtimes = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>())
                {
                    dataEmergencyWtimes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataEmergencyWtimes = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>>("api/HisEmergencyWtime/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataEmergencyWtimes != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME), dataEmergencyWtimes, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                this.InitComboCommon(this.cboEmergencyTime, dataEmergencyWtimes, "ID", "EMERGENCY_WTIME_NAME", "EMERGENCY_WTIME_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitTreatmentType()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> dataTreatmentTypes = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>())
                {
                    dataTreatmentTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataTreatmentTypes = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>>("api/HisTreatmentType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (dataTreatmentTypes != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE), dataTreatmentTypes, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                dataTreatmentTypes = dataTreatmentTypes.Where(p => p.IS_ALLOW_RECEPTION == 1).ToList();

                this.InitComboCommon(this.cboTreatmentType, dataTreatmentTypes, "ID", "TREATMENT_TYPE_NAME", 70, "TREATMENT_TYPE_CODE", 30);

                this.cboTreatmentType.EditValue = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPortToCombo()
        {
            try
            {
                //string[] list = Inventec.Common.Rs232.Connector.GetPortNames();
                //CboListPort.Properties.DataSource = list;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

    }
}
