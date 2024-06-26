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
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
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

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HTU_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HTU_NAME", "ID", columnInfos, false, 200);
                if (data != null)
                {
                    data = data.OrderBy(o => o.NUM_ORDER).ToList();
                }
                else
                    data = htus.OrderBy(o => o.NUM_ORDER).ToList();
                ControlEditorLoader.Load(cboHtu, data, controlEditorADO);
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
                List<V_HIS_MEDI_STOCK> mediStockAllows = new List<V_HIS_MEDI_STOCK>();
                List<V_HIS_MEDI_STOCK> mediStocks = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDI_STOCK>().ToList();
                if (mediStocks == null)
                    mediStocks = new List<V_HIS_MEDI_STOCK>();

                mediStockAllows = mediStocks.Where(o => o.IS_ACTIVE == 1 && o.IS_BUSINESS == 1).ToList();
                if (!string.IsNullOrWhiteSpace(HisConfigCFG.DefaultDrugStoreCode) && mediStockAllows != null && mediStockAllows.Count > 0)
                {
                    V_HIS_MEDI_STOCK defaultDrugStore = mediStockAllows.Where(o => o.MEDI_STOCK_CODE == HisConfigCFG.DefaultDrugStoreCode).FirstOrDefault();
                    if (defaultDrugStore != null)
                    {
                        cboNhaThuoc.EditValue = defaultDrugStore.ID;
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboNhaThuoc, mediStockAllows, controlEditorADO);
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

        private async Task InitComboRepositoryPatientType(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemcboPatientType, List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> data)
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
                var expMestTemplates = datas.Where(o => (o.CREATOR == loginName || (o.IS_PUBLIC ?? -1) == GlobalVariables.CommonNumberTrue)
                    && o.IS_ACTIVE == GlobalVariables.CommonNumberTrue).ToList();
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

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(cboUser, datas, controlEditorADO);
                string loginName = (this.serviceReqWorking != null ? this.serviceReqWorking.REQUEST_LOGINNAME : Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
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
        private async Task InitComboMedicineUseForm(object control, List<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM> data)
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
                this.currentWorkingMestRooms = new List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>();
                var mestRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == GetRoomId()).ToList();

                var medistocks = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>();
                var mediStockId__Actives = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(
                    o => o.IS_ACTIVE == null
                        || o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
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

                this.FilterMestRoomByIsCabinet(ref this.currentWorkingMestRooms);
                this.FilterMestRoomByBhytHeadCode(ref this.currentWorkingMestRooms);

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
                var mediStockId__Cabinets = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => (o.IS_CABINET ?? 0) == GlobalVariables.CommonNumberTrue).Select(o => o.ID).ToList();
                var mediStockId__Bloods = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => (o.IS_BLOOD ?? 0) == GlobalVariables.CommonNumberTrue).Select(o => o.ID).ToList();

                mestRoomTemps = mestRoomTemps
                    .Where(o =>
                        !mediStockId__Bloods.Contains(o.MEDI_STOCK_ID)
                        && (mediStockId__Cabinets == null || mediStockId__Cabinets.Count == 0 || !mediStockId__Cabinets.Contains(o.MEDI_STOCK_ID))).ToList();

                Inventec.Common.Logging.LogSystem.Debug("Loc kho theo dieu kien la tu truc hay khong (IsCabinet) => so luong kho tim thay = " + mestRoomTemps.Count);
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

                bool hasLoadedMediStock = false;
                cboMediStockExport.Properties.DataSource = listMediStock;
                List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> mediStockTemps = new List<V_HIS_MEST_ROOM>();
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    long keDonThuocMotHoacNhieuKho = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__CHE_DO_KE_DON_THUOC__MOT_HOAC_NHIEU_KHO);
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
                }
                if (this.currentMediStock == null)
                    this.currentMediStock = new List<V_HIS_MEST_ROOM>();

                if (hasLoadedMediStock && mediStockTemps != null && mediStockTemps.Count > 0)
                {
                    this.currentMediStock.AddRange(mediStockTemps);
                    GridCheckMarksSelection gridCheckMark = cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        if (gridCheckMark.Selection.Count == 0)
                            gridCheckMark.SelectAll(this.currentMediStock);
                    }
                    else
                    {
                        if (this.currentMediStock != null && this.currentMediStock.Count > 0)
                            cboMediStockExport.EditValue = this.currentMediStock.First().MEDI_STOCK_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
    }
}
