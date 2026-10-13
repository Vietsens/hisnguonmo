using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.AssignService.Config;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    /// <summary>
    /// Chi dinh che pham mau theo tui (cau hinh MOS.HIS_SERVICE_REQ.SPLIT_BLOOD_SERVICE_BY_BAG):
    /// - Dong che pham mau (dich vu loai XN, nhom BHYT Mau) co nut "Xet nghiem tui mau" de chon xet nghiem rieng cho tung tui
    /// - Khi luu, moi tui gui thanh 1 dong che pham so luong 1 + cac xet nghiem cua tui, cung 1 ma tui => backend tao moi tui 1 y lenh
    /// </summary>
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        private class BloodBagState
        {
            public long RoomId { get; set; }
            public bool IsCustomized { get; set; }
            public List<BloodBagADO> Bags { get; set; }
        }

        private const string BLOOD_BAG_FIELD_NAME = "BloodBagUnb";
        private Dictionary<long, BloodBagState> dicBloodBagStates = new Dictionary<long, BloodBagState>();
        private Dictionary<long, bool> dicIsBloodProductService = new Dictionary<long, bool>();
        private GridColumn gridColumnBloodBag;
        private RepositoryItemButtonEdit repositoryItemBtnBloodBag;
        private RepositoryItemTextEdit repositoryItemBloodBagNone;

        /// <summary>
        /// Dich vu la che pham mau: loai Xet nghiem va nhom BHYT "Mau"
        /// </summary>
        private bool IsBloodProductService(long serviceId)
        {
            bool result = false;
            try
            {
                if (this.dicIsBloodProductService.TryGetValue(serviceId, out result))
                    return result;
                V_HIS_SERVICE service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == serviceId);
                result = service != null
                    && service.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                    && service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__MAU;
                this.dicIsBloodProductService[serviceId] = result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool IsBloodBagRow(SereServADO row)
        {
            return HisConfigCFG.IsSplitBloodServiceByBag && row != null && row.IsChecked && this.IsBloodProductService(row.SERVICE_ID);
        }

        /// <summary>
        /// Them cot nut "XN tui mau" vao luoi dich vu da chon (chi khi bat cau hinh)
        /// </summary>
        private void InitBloodBagColumn()
        {
            try
            {
                if (!HisConfigCFG.IsSplitBloodServiceByBag || this.gridColumnBloodBag != null)
                    return;

                this.repositoryItemBtnBloodBag = new RepositoryItemButtonEdit();
                this.repositoryItemBtnBloodBag.TextEditStyle = TextEditStyles.DisableTextEditor;
                this.repositoryItemBtnBloodBag.ButtonClick += repositoryItemBtnBloodBag_ButtonClick;
                this.repositoryItemBloodBagNone = new RepositoryItemTextEdit();
                this.repositoryItemBloodBagNone.ReadOnly = true;
                this.gridControlServiceProcess.RepositoryItems.AddRange(new RepositoryItem[] { this.repositoryItemBtnBloodBag, this.repositoryItemBloodBagNone });

                this.gridColumnBloodBag = new GridColumn();
                this.gridColumnBloodBag.Name = "gridColumnBloodBag";
                this.gridColumnBloodBag.Caption = "XN túi máu";
                this.gridColumnBloodBag.ToolTip = "Chọn xét nghiệm cho từng túi máu";
                this.gridColumnBloodBag.FieldName = BLOOD_BAG_FIELD_NAME;
                this.gridColumnBloodBag.UnboundType = UnboundColumnType.String;
                this.gridColumnBloodBag.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                this.gridColumnBloodBag.OptionsFilter.AllowFilter = false;
                this.gridColumnBloodBag.OptionsFilter.AllowAutoFilter = false;
                this.gridColumnBloodBag.Width = 100;
                this.gridViewServiceProcess.Columns.Add(this.gridColumnBloodBag);
                this.gridColumnBloodBag.Visible = true;
                this.gridColumnBloodBag.VisibleIndex = this.gridColumn_InstructionNote.VisibleIndex >= 0 ? this.gridColumn_InstructionNote.VisibleIndex + 1 : 8;

                this.gridViewServiceProcess.CustomUnboundColumnData += gridViewServiceProcess_BloodBag_CustomUnboundColumnData;
                this.gridViewServiceProcess.CustomRowCellEdit += gridViewServiceProcess_BloodBag_CustomRowCellEdit;
                this.gridViewServiceProcess.ShowingEditor += gridViewServiceProcess_BloodBag_ShowingEditor;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_BloodBag_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column != this.gridColumnBloodBag || !e.IsGetData)
                    return;
                SereServADO row = e.Row as SereServADO;
                if (!this.IsBloodBagRow(row))
                {
                    e.Value = "";
                    return;
                }
                int bagCount = row.AMOUNT >= 1 && row.AMOUNT % 1 == 0 ? (int)row.AMOUNT : 0;
                BloodBagState state;
                bool customized = this.dicBloodBagStates.TryGetValue(row.SERVICE_ID, out state) && state.IsCustomized;
                e.Value = bagCount > 0 ? string.Format("{0} túi{1}", bagCount, customized ? " · đã chỉnh" : "") : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_BloodBag_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column != this.gridColumnBloodBag || e.RowHandle < 0)
                    return;
                SereServADO row = this.gridViewServiceProcess.GetRow(e.RowHandle) as SereServADO;
                e.RepositoryItem = this.IsBloodBagRow(row) ? (RepositoryItem)this.repositoryItemBtnBloodBag : this.repositoryItemBloodBagNone;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_BloodBag_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                if (this.gridViewServiceProcess.FocusedColumn != this.gridColumnBloodBag)
                    return;
                SereServADO row = this.gridViewServiceProcess.GetFocusedRow() as SereServADO;
                if (!this.IsBloodBagRow(row))
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnBloodBag_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                SereServADO row = this.gridViewServiceProcess.GetFocusedRow() as SereServADO;
                if (!this.IsBloodBagRow(row))
                    return;
                if (row.AMOUNT < 1 || row.AMOUNT % 1 != 0)
                {
                    XtraMessageBox.Show(string.Format("Số lượng chế phẩm máu {0} phải là số nguyên từ 1.", row.TDL_SERVICE_NAME), "Thông báo");
                    return;
                }
                if (row.TDL_EXECUTE_ROOM_ID <= 0)
                {
                    XtraMessageBox.Show(string.Format("Chọn phòng thực hiện cho chế phẩm máu {0} trước khi chọn xét nghiệm.", row.TDL_SERVICE_NAME), "Thông báo");
                    return;
                }

                BloodBagState state = this.GetBloodBagState(row);
                frmBloodBagService frm = new frmBloodBagService(row.TDL_SERVICE_NAME, state.Bags, () => this.BuildDefaultBloodBagTests(row), this.currentPatientTypes);
                if (frm.ShowDialog(this) == DialogResult.OK && frm.ResultBags != null)
                {
                    state.Bags = frm.ResultBags;
                    state.IsCustomized = true;
                    this.dicBloodBagStates[row.SERVICE_ID] = state;
                    this.gridControlServiceProcess.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lay lua chon xet nghiem theo tui cua dong che pham, dong bo so tui theo so luong:
        /// tang thi them tui mac dinh, giam thi bo tui cuoi; doi phong thuc hien thi lam lai mac dinh
        /// </summary>
        private BloodBagState GetBloodBagState(SereServADO row)
        {
            int bagCount = (int)row.AMOUNT;
            BloodBagState state;
            if (!this.dicBloodBagStates.TryGetValue(row.SERVICE_ID, out state) || state == null || state.RoomId != row.TDL_EXECUTE_ROOM_ID || state.Bags == null)
            {
                state = new BloodBagState() { RoomId = row.TDL_EXECUTE_ROOM_ID, IsCustomized = false, Bags = new List<BloodBagADO>() };
                this.dicBloodBagStates[row.SERVICE_ID] = state;
            }
            while (state.Bags.Count < bagCount)
            {
                state.Bags.Add(new BloodBagADO() { Tests = this.BuildDefaultBloodBagTests(row) });
            }
            while (state.Bags.Count > bagCount)
            {
                state.Bags.RemoveAt(state.Bags.Count - 1);
            }
            return state;
        }

        /// <summary>
        /// Danh sach xet nghiem co the chon cho 1 tui: dich vu loai XN thuc hien duoc tai phong cua che pham
        /// va dich vu di kem cua che pham (tick san). Doi tuong thanh toan mac dinh nhu cach xac dinh dich vu di kem.
        /// </summary>
        private List<BloodBagTestADO> BuildDefaultBloodBagTests(SereServADO row)
        {
            List<BloodBagTestADO> result = new List<BloodBagTestADO>();
            try
            {
                long defaultPatientTypeId = this.currentHisPatientTypeAlter != null ? this.currentHisPatientTypeAlter.PATIENT_TYPE_ID : row.PATIENT_TYPE_ID;
                List<long> candidatePatientTypeIds = new List<long>() { defaultPatientTypeId };
                if (this.currentPatientTypeAllows != null)
                {
                    candidatePatientTypeIds.AddRange(this.currentPatientTypeAllows.Where(o => o.PATIENT_TYPE_ID == defaultPatientTypeId).Select(o => o.PATIENT_TYPE_ALLOW_ID));
                }
                candidatePatientTypeIds = candidatePatientTypeIds.Distinct().ToList();

                long? treatmentTypeId = this.currentTreatment != null ? this.currentTreatment.TDL_TREATMENT_TYPE_ID : null;
                List<V_HIS_SERVICE_FOLLOW> follows = BackendDataWorker.Get<V_HIS_SERVICE_FOLLOW>()
                    .Where(o => o.SERVICE_ID == row.SERVICE_ID
                        && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && (string.IsNullOrEmpty(o.TREATMENT_TYPE_IDS) || !treatmentTypeId.HasValue || ("," + o.TREATMENT_TYPE_IDS + ",").Contains("," + treatmentTypeId.Value + ",")))
                    .ToList();

                HashSet<long> roomServiceIds = new HashSet<long>(BackendDataWorker.Get<V_HIS_SERVICE_ROOM>()
                    .Where(o => o.ROOM_ID == row.TDL_EXECUTE_ROOM_ID && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .Select(o => o.SERVICE_ID));
                HashSet<long> followIds = new HashSet<long>(follows.Select(o => o.FOLLOW_ID));

                List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                        && (roomServiceIds.Contains(o.ID) || followIds.Contains(o.ID))
                        && !this.IsBloodProductService(o.ID))
                    .ToList();

                foreach (V_HIS_SERVICE service in services)
                {
                    V_HIS_SERVICE_FOLLOW follow = follows.FirstOrDefault(o => o.FOLLOW_ID == service.ID);
                    List<long> allowIds = candidatePatientTypeIds.Where(pt => BranchDataWorker.HasServicePatyWithListPatientType(service.ID, new List<long>() { pt })).ToList();
                    BloodBagTestADO test = new BloodBagTestADO();
                    test.ServiceId = service.ID;
                    test.ServiceCode = service.SERVICE_CODE;
                    test.ServiceName = service.SERVICE_NAME;
                    test.IsDefault = follow != null;
                    test.IsChecked = follow != null;
                    test.IsExpend = follow != null ? follow.IS_EXPEND : null;
                    test.Amount = follow != null && follow.AMOUNT > 0 ? follow.AMOUNT : 1;
                    test.AllowPatientTypeIds = allowIds;
                    test.PatientTypeId = allowIds.Contains(defaultPatientTypeId) ? defaultPatientTypeId : (allowIds.Count > 0 ? (long?)allowIds.First() : null);
                    result.Add(test);
                }
                result = result.OrderByDescending(o => o.IsDefault).ThenBy(o => o.ServiceName).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Khi luu: thay moi dong che pham mau bang N dong (N = so tui), moi dong so luong 1 kem cac xet nghiem cua tui,
        /// tat ca mang cung ma tui. Tra ve false neu du lieu khong hop le (da hien thong bao) => dung luu.
        /// </summary>
        private bool ExpandBloodBags(AssignServiceSDO serviceReqSDO, List<SereServADO> dataSereServModel)
        {
            try
            {
                if (!HisConfigCFG.IsSplitBloodServiceByBag || serviceReqSDO == null || serviceReqSDO.ServiceReqDetails == null || dataSereServModel == null)
                    return true;
                List<SereServADO> bloodRows = dataSereServModel.Where(o => this.IsBloodProductService(o.SERVICE_ID)).ToList();
                if (bloodRows.Count == 0)
                    return true;

                foreach (SereServADO row in bloodRows)
                {
                    if (row.AMOUNT < 1 || row.AMOUNT % 1 != 0)
                    {
                        XtraMessageBox.Show(string.Format("Số lượng chế phẩm máu {0} phải là số nguyên từ 1.", row.TDL_SERVICE_NAME), "Thông báo");
                        return false;
                    }
                    ServiceReqDetailSDO mainSdo = serviceReqSDO.ServiceReqDetails.FirstOrDefault(o => o.ServiceId == row.SERVICE_ID && string.IsNullOrEmpty(o.BloodBagCode));
                    if (mainSdo == null)
                        continue;

                    BloodBagState state = this.GetBloodBagState(row);
                    int bagCount = (int)row.AMOUNT;
                    int insertAt = serviceReqSDO.ServiceReqDetails.IndexOf(mainSdo);
                    serviceReqSDO.ServiceReqDetails.Remove(mainSdo);
                    string doctorNote = string.IsNullOrWhiteSpace(mainSdo.InstructionNote) ? "" : mainSdo.InstructionNote.Trim();

                    List<ServiceReqDetailSDO> bagDetails = new List<ServiceReqDetailSDO>();
                    for (int i = 0; i < bagCount; i++)
                    {
                        string bagCode = Guid.NewGuid().ToString("N");
                        ServiceReqDetailSDO bagMain = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceReqDetailSDO>(Newtonsoft.Json.JsonConvert.SerializeObject(mainSdo));
                        bagMain.Amount = 1;
                        bagMain.MultipleExecute = null;
                        bagMain.BloodBagCode = bagCode;
                        bagMain.InstructionNote = string.Format("{0} - túi {1}/{2}", row.TDL_SERVICE_NAME, i + 1, bagCount) + (doctorNote.Length > 0 ? " · " + doctorNote : "");
                        bagDetails.Add(bagMain);

                        foreach (BloodBagTestADO test in state.Bags[i].Tests.Where(o => o.IsChecked))
                        {
                            if (!test.PatientTypeId.HasValue)
                            {
                                XtraMessageBox.Show(string.Format("Túi {0} của {1}: xét nghiệm {2} chưa có đối tượng thanh toán có giá.", i + 1, row.TDL_SERVICE_NAME, test.ServiceName), "Thông báo");
                                return false;
                            }
                            ServiceReqDetailSDO testSdo = new ServiceReqDetailSDO();
                            testSdo.EkipInfos = new List<EkipSDO>();
                            testSdo.ServiceId = test.ServiceId;
                            testSdo.Amount = test.Amount > 0 ? test.Amount : 1;
                            testSdo.PatientTypeId = test.PatientTypeId.Value;
                            testSdo.IsExpend = test.IsExpend;
                            testSdo.RoomId = mainSdo.RoomId;
                            testSdo.BloodBagCode = bagCode;
                            if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                            {
                                testSdo.PrimaryPatientTypeId = mainSdo.PrimaryPatientTypeId;
                            }
                            bagDetails.Add(testSdo);
                        }
                    }
                    serviceReqSDO.ServiceReqDetails.InsertRange(insertAt < 0 ? serviceReqSDO.ServiceReqDetails.Count : insertAt, bagDetails);
                }

                //Xet nghiem vua nam trong tui mau vua duoc tick rieng o danh sach dich vu => chan de tranh chi dinh trung
                HashSet<long> bagTestIds = new HashSet<long>(serviceReqSDO.ServiceReqDetails
                    .Where(o => !string.IsNullOrEmpty(o.BloodBagCode) && !this.IsBloodProductService(o.ServiceId))
                    .Select(o => o.ServiceId));
                List<long> duplicateIds = serviceReqSDO.ServiceReqDetails
                    .Where(o => string.IsNullOrEmpty(o.BloodBagCode) && bagTestIds.Contains(o.ServiceId))
                    .Select(o => o.ServiceId).Distinct().ToList();
                if (duplicateIds.Count > 0)
                {
                    string names = string.Join(", ", dataSereServModel.Where(o => duplicateIds.Contains(o.SERVICE_ID)).Select(o => o.TDL_SERVICE_NAME).Distinct());
                    XtraMessageBox.Show(string.Format("Xét nghiệm {0} đã có trong túi máu. Bỏ chọn ở danh sách dịch vụ hoặc trong bảng \"Xét nghiệm túi máu\".", names), "Thông báo");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }
    }
}
